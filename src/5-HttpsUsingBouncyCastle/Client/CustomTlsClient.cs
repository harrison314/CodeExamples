using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;

namespace BcHttpClient.Client;

internal class CustomTlsClient : DefaultTlsClient
{
    private readonly Func<TlsAuthentication>? tlsAuthenticationFactory;

    public CustomTlsClient(Func<TlsAuthentication>? tlsAuthenticationFactory)
        : base(new BcTlsCrypto())
    {
        this.tlsAuthenticationFactory = tlsAuthenticationFactory;
    }

    public override TlsAuthentication GetAuthentication()
    {
        if (this.tlsAuthenticationFactory != null)
        {
            return this.tlsAuthenticationFactory.Invoke();
        }

        return new CustomTlsAuthentication();
    }
}
