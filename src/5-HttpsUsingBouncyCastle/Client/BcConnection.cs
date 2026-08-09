using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BcHttpClient.Client;

public sealed class BcConnection : IDisposable
{
    private readonly TcpClient tcp;
    private readonly TlsClientProtocol tls;

    public Stream Stream => this.tls.Stream;
    public bool IsUsable { get; private set; } = true;

    public BcConnection(IPAddress ip, int port, string host, Uri? proxyTargetUri = null, Func<TlsAuthentication>? tlsAuthenticationFactory = null)
    {
        this.tcp = new TcpClient();
        this.tcp.Connect(ip, port);

        NetworkStream net = this.tcp.GetStream();

        if (proxyTargetUri != null && proxyTargetUri.Scheme == "https")
        {
            this.EstablishConnectTunnel(net, host, proxyTargetUri.Port).Wait();
        }

        this.tls = new TlsClientProtocol(net);

        CustomTlsClient client = new CustomTlsClient(tlsAuthenticationFactory);

        this.tls.Connect(client);
    }

    private async Task EstablishConnectTunnel(Stream stream, string host, int port)
    {
        string connectRequest = $"CONNECT {host}:{port} HTTP/1.1\r\nHost: {host}:{port}\r\n\r\n";
        byte[] requestBytes = Encoding.ASCII.GetBytes(connectRequest);
        await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

        using StreamReader reader = new StreamReader(stream, Encoding.ASCII, false, 8192, leaveOpen: true);
        string? statusLine = await reader.ReadLineAsync();
        if (!statusLine?.StartsWith("HTTP/1") == true || !statusLine.Contains("200"))
        {
            throw new InvalidOperationException($"Proxy CONNECT failed: {statusLine}");
        }

        string? line;
        while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
        {
            // Skip headers
        }
    }

    public void Dispose()
    {
        this.IsUsable = false;
        try { this.tls.Close(); } catch { }
        try { this.tcp.Close(); } catch { }
    }
}
