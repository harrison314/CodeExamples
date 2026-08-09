using Org.BouncyCastle.Tls;

namespace BcHttpClient.Client;

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