using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BcHttpClient.Client;

public sealed class BcConnectionPool : IDisposable
{
    private readonly ConcurrentDictionary<IPAddress, ConcurrentBag<BcConnection>> pools = new();
    private bool disposed = false;

    public bool TryGet(IPAddress ip, out BcConnection? conn)
    {
        if (this.pools.TryGetValue(ip, out ConcurrentBag<BcConnection>? bag))
        {
            if (bag.TryTake(out BcConnection? c))
            {
                if (c.IsUsable)
                {
                    conn = c;
                    return true;
                }

                c.Dispose();
            }
        }

        conn = null;
        return false;
    }

    public void Return(IPAddress ip, BcConnection conn)
    {
        if (!conn.IsUsable)
        {
            conn.Dispose();
            return;
        }

        ConcurrentBag<BcConnection> bag = this.pools.GetOrAdd(ip, _ => new ConcurrentBag<BcConnection>());
        bag.Add(conn);
    }

    public void Dispose()
    {
        if (this.disposed)
            return;

        foreach (ConcurrentBag<BcConnection> bag in this.pools.Values)
        {
            while (bag.TryTake(out BcConnection? conn))
            {
                conn.Dispose();
            }
        }

        this.pools.Clear();
        this.disposed = true;
    }
}
