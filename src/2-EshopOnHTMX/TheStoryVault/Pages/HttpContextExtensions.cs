using System.Diagnostics.CodeAnalysis;

namespace TheStoryVault.Pages;

internal static class HttpContextExtensions
{
    public static bool IsHtmxRequest(this HttpContext context)
    {
        return context.Request.Headers.ContainsKey("HX-Request");
    }

    public static bool IsHtmxBoostedRequest(this HttpContext context)
    {
        return context.Request.Headers.ContainsKey("HX-Boosted");
    }

    public static bool TryGetHtmxCurrentUrl(this HttpContext context, [NotNullWhen(true)] out string? url)
    {
        url = null;

        if (context.Request.Headers.TryGetValue("HX-Current-URL", out Microsoft.Extensions.Primitives.StringValues values))
        {
            url = values.SingleOrDefault();
        }

        return (url != null);
    }

    public static bool IsHtmxRestureRequest(this HttpContext context)
    {
        return context.Request.Headers.ContainsKey("HX-History-Restore-Request");
    }

    public static void SendHtmxRefresh(this HttpContext context)
    {
        context.Response.Headers.Append("HX-Refresh", "true");
    }

    public static void SendHtmxRedirect(this HttpContext context, string url)
    {
        context.Response.Headers.Append("HX-Redirect", url);
    }

    public static void SendHtmxPushUrlHistory(this HttpContext context, string url)
    {
        context.Response.Headers.Append("HX-Push-Url", url);
    }

    public static void SendHtmxReplaceUrlHistory(this HttpContext context, string url)
    {
        context.Response.Headers.Append("HX-Replace-Url", url);
    }

    public static void SendHtmxRetarget(this HttpContext context, string cssSelector)
    {
        context.Response.Headers.Append("HX-Retarget", cssSelector);
    }

    public static void SendHtmxReswap(this HttpContext context, string cssSelector)
    {
        context.Response.Headers.Append("HX-Reswap", cssSelector);
    }

    public static void SendHtmxTrigger(this HttpContext context, string eventName, HtmxTriggerType htmxTriggerType)
    {
        string triggerType = htmxTriggerType switch
        {
            HtmxTriggerType.Normal => "hx-trigger",
            HtmxTriggerType.AfterSettle => "hx-trigger-after-settle",
            HtmxTriggerType.AfterSwap => "hx-trigger-after-swap",
            _ => throw new ArgumentOutOfRangeException(nameof(htmxTriggerType), htmxTriggerType, null)
        };
        context.Response.Headers.Append(triggerType, eventName);
    }
}

internal enum HtmxTriggerType
{
    Normal,
    AfterSettle,
    AfterSwap
}
