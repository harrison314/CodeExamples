using Org.BouncyCastle.Tls;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BcHttpClient.Client;

public sealed class BouncyCastleHttpMessageHandler : HttpMessageHandler
{

    private readonly DnsCache dns = new();
    private readonly BcConnectionPool pool = new();
    private CookieContainer? cookieContainer;

    public bool UseCookies
    {
        get => this.cookieContainer != null;
        set
        {
            if (value && this.cookieContainer == null)
                this.cookieContainer = new CookieContainer();
            else if (!value)
                this.cookieContainer = null;
        }
    }

    public CookieContainer? CookieContainer
    {
        get => this.cookieContainer;
        set => this.cookieContainer = value;
    }

    public IWebProxy? Proxy
    {
        get;
        set;
    }

    public Func<TlsAuthentication>? TlsAuthenticationFactory
    {
        get;
        set;
    }

    public BouncyCastleHttpMessageHandler()
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string targetHost = request.RequestUri.DnsSafeHost;
        int targetPort = request.RequestUri.Port;

        IPAddress connectIp;
        int connectPort;
        bool useProxy = false;

        if (this.Proxy != null && !this.Proxy.IsBypassed(request.RequestUri))
        {
            Uri? proxyUri = this.Proxy.GetProxy(request.RequestUri);
            if (proxyUri != null)
            {
                connectIp = (await this.dns.ResolveAsync(proxyUri.Host)).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                    ?? (await this.dns.ResolveAsync(proxyUri.Host)).First();
                connectPort = proxyUri.Port;
                useProxy = true;
            }
            else
            {
                IPAddress[] addrs = await this.dns.ResolveAsync(targetHost);
                connectIp = addrs.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addrs.First();
                connectPort = targetPort;
            }
        }
        else
        {
            IPAddress[] addrs = await this.dns.ResolveAsync(targetHost);
            connectIp = addrs.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork) ?? addrs.First();
            connectPort = targetPort;
        }

        if (!this.pool.TryGet(connectIp, out BcConnection? conn))
            conn = new BcConnection(connectIp, connectPort, targetHost, useProxy ? request.RequestUri : null, this.TlsAuthenticationFactory);

        try
        {
            if (this.cookieContainer != null)
            {
                CookieCollection cookies = this.cookieContainer.GetCookies(request.RequestUri);
                if (cookies.Count > 0)
                {
                    string cookieHeader = string.Join("; ", cookies.Cast<Cookie>().Select(c => $"{c.Name}={c.Value}"));
                    request.Headers.Add("Cookie", cookieHeader);
                }
            }

            await this.WriteRequestAsync(conn.Stream, request, useProxy, cancellationToken);
            HttpResponseMessage response = await this.ReadResponseAsync(conn.Stream, cancellationToken);

            if (this.cookieContainer != null)
            {
                if (response.Headers.TryGetValues("X-Set-Cookie", out IEnumerable<string>? setCookieValues))
                {
                    foreach (string setCookieHeader in setCookieValues)
                    {
                        string[] cookies = setCookieHeader.Split('\x00');
                        foreach (string cookieValue in cookies)
                        {
                            try
                            {
                                this.cookieContainer.SetCookies(request.RequestUri, cookieValue);
                            }
                            catch
                            {
                                // Ignore malformed cookie headers
                            }
                        }
                    }
                    response.Headers.Remove("X-Set-Cookie");
                }
            }

            if (response.Headers.ConnectionClose == true)
                conn.Dispose();
            else
                this.pool.Return(connectIp, conn);

            return response;
        }
        catch
        {
            conn.Dispose();
            throw;
        }
    }

    private async Task WriteRequestAsync(Stream stream, HttpRequestMessage req, bool useProxy, CancellationToken ct)
    {
        StringBuilder sb = new StringBuilder();

        string requestLine = useProxy
            ? $"{req.Method} {req.RequestUri} HTTP/1.1\r\n"
            : $"{req.Method} {req.RequestUri.PathAndQuery} HTTP/1.1\r\n";

        sb.Append(requestLine);
        sb.Append($"Host: {req.RequestUri.Host}\r\n");

        if (req.Headers.ConnectionClose ?? false)
        {
            sb.Append("Connection: close\r\n");
        }
        else
        {
            sb.Append("Connection: keep-alive\r\n");
        }

        foreach (KeyValuePair<string, IEnumerable<string>> h in req.Headers)
        {
            //TODO: check with specifikaction
            // sb.Append($"{h.Key}: {string.Join(",", h.Value)}\r\n");
            foreach (string headerValue in h.Value)
            {
                sb.Append($"{h.Key}: {headerValue}\r\n");
            }
        }

        byte[]? bodyBytes = null;

        if (req.Content != null)
        {
            byte[] body = await req.Content.ReadAsByteArrayAsync(ct);
            bodyBytes = body;
            sb.Append($"Content-Length: {body.Length}\r\n");
        }

        sb.Append("\r\n");

        byte[] headerBytes = Encoding.ASCII.GetBytes(sb.ToString());
        await stream.WriteAsync(headerBytes, ct);

        if (bodyBytes != null)
            await stream.WriteAsync(bodyBytes, ct);
    }

    private async Task<HttpResponseMessage> ReadResponseAsync(Stream stream, CancellationToken ct)
    {
        using StreamReader reader = new StreamReader(stream, Encoding.ASCII, false, 8192, leaveOpen: true);

        string? statusLine = await reader.ReadLineAsync(ct);
        string[] parts = statusLine.Split(' ');
        int statusCode = int.Parse(parts[1]);

        HttpResponseMessage response = new HttpResponseMessage((HttpStatusCode)statusCode);

        string? line;
        long contentLength = 0;
        List<string> setCookieHeaders = new List<string>();

        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync(ct)))
        {
            int idx = line.IndexOf(':');
            string name = line[..idx];
            string value = line[(idx + 1)..].Trim();

            if (name.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
            {
                contentLength = long.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (name.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
            {
                // Store Set-Cookie headers separately for cookie processing
                setCookieHeaders.Add(value);
            }
            else
            {
                response.Headers.TryAddWithoutValidation(name, value);
            }
        }

        MemoryStream ms = new MemoryStream();
        if (contentLength > 0)
        {
            byte[] buffer = new byte[Math.Min(81920, (int)contentLength)];
            long bytesRemaining = contentLength;
            while (bytesRemaining > 0)
            {
                int bytesToRead = (int)Math.Min(buffer.Length, bytesRemaining);
                int bytesRead = await reader.BaseStream.ReadAsync(buffer, 0, bytesToRead, ct);
                if (bytesRead == 0)
                    break;
                await ms.WriteAsync(buffer, 0, bytesRead, ct);
                bytesRemaining -= bytesRead;
            }
        }
        ms.Position = 0;

        response.Content = new StreamContent(ms);

        if (setCookieHeaders.Count > 0)
        {
            response.Headers.Add("X-Set-Cookie", string.Join("\x00", setCookieHeaders));
        }

        return response;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.pool.Dispose();
        }

        base.Dispose(disposing);
    }
}