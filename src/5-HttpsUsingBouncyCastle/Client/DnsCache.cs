using System.Collections.Concurrent;
using System.Net;

namespace BcHttpClient.Client;

public sealed class DnsCache
{
    private readonly ConcurrentDictionary<string, (IPAddress[] Addrs, DateTime Expire)> cache = new();
    private readonly TimeSpan ttl = TimeSpan.FromMinutes(5);

    public async Task<IPAddress[]> ResolveAsync(string host)
    {
        if (this.cache.TryGetValue(host, out (IPAddress[] Addrs, DateTime Expire) entry))
        {
            if (entry.Expire > DateTime.UtcNow)
                return entry.Addrs;
        }

        IPAddress[] addrs = await Dns.GetHostAddressesAsync(host);
        this.cache[host] = (addrs, DateTime.UtcNow + this.ttl);
        return addrs;
    }
}
