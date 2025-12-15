using System.Net; 
using System.Net.Sockets; 
using Org.BouncyCastle.Tls.Crypto.Impl.BC; 
using Org.BouncyCastle.Security; 
using Org.BouncyCastle.Tls; 
using System.Text; 
using System.Threading.Tasks; 

namespace BouncyCastleExamples; 

internal class CustomTlsClient : DefaultTlsClient 
{ 
    public CustomTlsClient() : base(new BcTlsCrypto())  
    { 

    } 

    public override TlsAuthentication GetAuthentication() 
    { 
        return new CustomTlsAuthentication(); 
    } 
} 

internal class CustomTlsAuthentication : TlsAuthentication 
{ 
    public TlsCredentials GetClientCredentials(CertificateRequest certificateRequest) 
    { 
        return null!; 
    } 

    public void NotifyServerCertificate(TlsServerCertificate serverCertificate) 
    { 
    } 
} 

internal class Program 
{ 
    const string ServerName = "harrison314.github.io"; 
    const string UrlQuery = "/dev_endpoints/simple.json"; 

    static async Task Main(string[] args) 
    { 
        using TcpClient client = new TcpClient(ServerName, 443); 

        TlsClientProtocol protocol = new TlsClientProtocol(client.GetStream()); 
        protocol.Connect(new CustomTlsClient()); 
        using var stream = protocol.Stream; 

        StringBuilder hdr = new StringBuilder(); 
        hdr.AppendFormat("GET {0} HTTP/1.1\r\n", UrlQuery); 
        hdr.AppendFormat("Host: {0}\r\n", ServerName); 
        hdr.Append("Connection: close\r\n"); 
        hdr.AppendLine("\r\n"); 

        var dataToSend = Encoding.UTF8.GetBytes(hdr.ToString()); 
        
        await stream.WriteAsync(dataToSend, 0, dataToSend.Length); 

        using MemoryStream ms = new MemoryStream(); 
        await stream.CopyToAsync(ms); 

        string response = Encoding.UTF8.GetString(ms.ToArray()); 

        Console.WriteLine(response); 
    } 
} 