using FastHashes;
using Microsoft.AspNetCore.Http;
using System.Buffers.Text;
using System.Net.Http;
using System.Text;
using TheStoryVault.Services.Contracts;

namespace TheStoryVault.Services.Implementation;

public class CookieApi : ICookieApi
{
    public const string CookieTrackingIdentifier = "CTI";
    public const string CookieTrackingIdentifierDisabled = "-disabled";
    public const string CookieBasketIdentifier = "CBI";

    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ILogger<CookieApi> logger;
    private Guid? basketIdentifier = null;

    public CookieApi(IHttpContextAccessor httpContextAccessor, ILogger<CookieApi> logger)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.logger = logger;
    }

    public Guid? SetTrackingIdentifier(bool setTrackingIdentifier)
    {
        if (setTrackingIdentifier)
        {
            Guid userId = Guid.NewGuid();
            this.GetContext().Response.Cookies.Append(CookieTrackingIdentifier, userId.ToString("D"), this.GetOptions());
            return userId;
        }
        else
        {
            this.GetContext().Response.Cookies.Append(CookieTrackingIdentifier, CookieTrackingIdentifierDisabled, this.GetOptions());
            return null;
        }
    }

    public TrackingIdentifierResult GetTrackingIdentifier()
    {
        if (this.GetContext().Request.Cookies.TryGetValue(CookieTrackingIdentifier, out string? cookieValue))
        {
            if (cookieValue == CookieTrackingIdentifierDisabled)
            {
                return new TrackingIdentifierResult(TrackingIdentifierState.Disabled, this.GetAntonymousIdentifier());
            }
            else if (Guid.TryParse(cookieValue, out Guid userId))
            {
                return new TrackingIdentifierResult(TrackingIdentifierState.Enabled, userId.ToString("D"));
            }
            else
            {
                this.logger.LogWarning("Cookie {CookieName} has invalid value: {CookieValue}", CookieTrackingIdentifier, cookieValue);
            }
        }

        return new TrackingIdentifierResult(TrackingIdentifierState.None, this.GetAntonymousIdentifier());
    }

    private string GetAntonymousIdentifier()
    {
        HttpRequest request = this.httpContextAccessor.HttpContext!.Request;
        string value = $"{request.Headers.UserAgent.FirstOrDefault()}#{this.httpContextAccessor.HttpContext!.Request.Host.Host}";

        FarmHash128 hash = new FarmHash128(125UL, 7896532UL);
        byte[] hashValue = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Base64Url.EncodeToString(hashValue);
    }

    public void SetBasketIdentifier(Guid basketId)
    {
        this.basketIdentifier = basketId;
        this.GetContext().Response.Cookies.Append(CookieBasketIdentifier, basketId.ToString("D"), this.GetOptions());
    }

    public Guid? GetBasketIdentifier()
    {
        if (this.GetContext().Request.Cookies.TryGetValue(CookieBasketIdentifier, out string? cookieValue) && Guid.TryParse(cookieValue, out Guid basketId))
        {
            return basketId;
        }

        return this.basketIdentifier;
    }

    private HttpContext GetContext()
    {
        HttpContext? httpContext = this.httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new Exception("HTTP Context not found");
        }

        return httpContext;
    }

    private CookieOptions GetOptions()
    {
        return new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict
        };
    }
}
