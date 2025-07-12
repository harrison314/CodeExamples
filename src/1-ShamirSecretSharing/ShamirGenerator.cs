using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace ShamirBc;

internal class ShamirCore
{
    public static SecretShare[] Split(BigInteger secret, int needed, int available, BigInteger prime, SecureRandom random)
    {
        BigInteger[] coeff = new BigInteger[needed];
        coeff[0] = secret;

        for (int i = 1; i < needed; i++)
        {
            BigInteger r;
            for (; ; )
            {
                r = new BigInteger(prime.BitLength, random);
                if (r.CompareTo(BigInteger.Zero) > 0 && r.CompareTo(prime) < 0)
                {
                    break;
                }
            }

            coeff[i] = r;
        }

        SecretShare[] shares = new SecretShare[available];
        for (int x = 1; x <= available; x++)
        {
            BigInteger accum = secret;

            for (int exp = 1; exp < needed; exp++)
            {
                accum = accum.Add(coeff[exp].Multiply(BigInteger.ValueOf(x).ModPow(BigInteger.ValueOf(exp), prime))).Mod(prime);
            }
            shares[x - 1] = new SecretShare(x, accum);
        }

        return shares;
    }

    public static BigInteger Combine(SecretShare[] shares, BigInteger prime)
    {
        BigInteger accum = BigInteger.Zero;
        for (int formula = 0; formula < shares.Length; formula++)
        {
            BigInteger numerator = BigInteger.One;
            BigInteger denominator = BigInteger.One;

            for (int count = 0; count < shares.Length; count++)
            {
                if (formula == count)
                {
                    continue;
                }

                int startposition = shares[formula].Number;
                int nextposition = shares[count].Number;

                numerator = numerator.Multiply(BigInteger.ValueOf(nextposition).Negate()).Mod(prime); // (numerator * -nextposition) % prime;
                denominator = denominator.Multiply(BigInteger.ValueOf(startposition - nextposition)).Mod(prime); // (denominator * (startposition - nextposition)) % prime;
            }
            BigInteger value = shares[formula].Share;
            BigInteger tmp = value.Multiply(numerator).Multiply(ModInverse(denominator, prime));
            accum = prime.Add(accum).Add(tmp).Mod(prime); //  (prime + accum + (value * numerator * modInverse(denominator))) % prime;
        }


        return accum;
    }

    private static BigInteger ModInverse(BigInteger k, BigInteger prime)
    {
        k = k.Mod(prime);
        BigInteger r = (k.CompareTo(BigInteger.Zero) == -1) ? (GcdD(prime, k.Negate()).v3).Negate() : GcdD(prime, k).v3;
        return prime.Add(r).Mod(prime);
    }

    private static (BigInteger v1, BigInteger v2, BigInteger v3) GcdD(BigInteger a, BigInteger b)
    {
        if (b.CompareTo(BigInteger.Zero) == 0)
            return (a, BigInteger.One, BigInteger.Zero);
        else
        {
            BigInteger n = a.Divide(b);
            BigInteger c = a.Mod(b);
            (BigInteger v1, BigInteger v2, BigInteger v3) r = GcdD(b, c);
            return new(r.v1, r.v3, r.v2.Subtract(r.v3.Multiply(n)));
        }
    }
}

public class ShamirGeneratorParams
{
    public SecureRandom SecureRandom
    {
        get;
    }

    public int SecretSize
    {
        get;
    }

    public int AvailableShares
    {
        get;
    }

    public int NeededShares
    {
        get;
    }

    public ShamirGeneratorParams(SecureRandom secureRandom, int secretSize, int availableShares, int neededShares)
    {
        this.SecureRandom = secureRandom;
        this.SecretSize = secretSize;
        this.AvailableShares = availableShares;
        this.NeededShares = neededShares;
    }
}

public class ShamirGeneratorWithSecretParams : ShamirGeneratorParams
{
    public byte[] Secret
    {
        get;
    }

    public ShamirGeneratorWithSecretParams(SecureRandom secureRandom, byte[] secret, int availableShares, int neededShares)
    : base(secureRandom, secret.Length, availableShares, neededShares)
    {
        this.Secret = secret;
    }
}

public class ShamirSharingResult
{
    public byte[] Secret
    {
        get;
        set;
    }

    public byte[] Prime
    {
        get;
        set;
    }

    public SecretShare[] Shares
    {
        get;
        internal set;
    }

    internal ShamirSharingResult()
    {
        this.Secret = default!;
        this.Prime = default!;
        this.Shares = default!;
    }

    public ShamirSharingResult(byte[] secret, byte[] prime, SecretShare[] shares)
    {
        this.Secret = secret;
        this.Prime = prime;
        this.Shares = shares;
    }
}
public class ShamirGenerator
{
    private ShamirGeneratorParams? shamirGeneratorParams;

    public ShamirGenerator()
    {

    }

    public void Init(ShamirGeneratorParams shamirGeneratorParams)
    {
        ArgumentNullException.ThrowIfNull(shamirGeneratorParams);
        this.shamirGeneratorParams = shamirGeneratorParams;
    }

    public ShamirSharingResult Generate()
    {
        if (this.shamirGeneratorParams == null)
        {
            throw new InvalidOperationException("operation is not initialized");
        }

        int sBitlength = this.shamirGeneratorParams.SecretSize * 8;
        BigInteger p;
        BigInteger s;

        if (this.shamirGeneratorParams is ShamirGeneratorWithSecretParams withSecretparams)
        {
            s = new BigInteger(1, withSecretparams.Secret);
            p = BigInteger.ProbablePrime(withSecretparams.Secret.Length * 8, withSecretparams.SecureRandom);
            while (p.CompareTo(s) <= 0)
            {
                p = BigInteger.ProbablePrime(withSecretparams.Secret.Length * 8, withSecretparams.SecureRandom);
            }
        }
        else
        {
            p = BigInteger.ProbablePrime(sBitlength, this.shamirGeneratorParams.SecureRandom);
            s = new BigInteger(sBitlength, 1, this.shamirGeneratorParams.SecureRandom).Mod(p);
        }

        SecretShare[] shares = ShamirCore.Split(s,
            this.shamirGeneratorParams.NeededShares,
            this.shamirGeneratorParams.AvailableShares,
            p,
            this.shamirGeneratorParams.SecureRandom);

        return new ShamirSharingResult()
        {
            Secret = this.PadByteArray(s, this.shamirGeneratorParams.SecretSize),
            Prime = p.ToByteArrayUnsigned(),
            Shares = shares,
        };
    }

    private byte[] PadByteArray(BigInteger i, int size)
    {
        byte[] iByteArray = i.ToByteArrayUnsigned();
        if (iByteArray.Length < size)
        {
            byte[] buffer = new byte[size];
            iByteArray.CopyTo(buffer, size - iByteArray.Length);
            return buffer;
        }
        else
        {
            return iByteArray;
        }
    }
}
public class SecretShare
{
    public int Number { get; }
    public BigInteger Share { get; }

    public SecretShare(int number, BigInteger share)
    {
        this.Number = number;
        this.Share = share;
    }

    public SecretShare(DerSequence seq)
    {
        this.Number = DerInteger.GetInstance(seq[0]).IntValueExact;
        this.Share = DerInteger.GetInstance(seq[1]).Value;
    }

    public Asn1Object ToAsn1Object()
    {
        return new DerSequence(
            new DerInteger(this.Number),
            new DerInteger(this.Share));
    }

    public override bool Equals(object? obj)
    {
        if (obj is SecretShare other)
        {
            return this.Number.Equals(other.Number) && this.Share.Equals(other.Share);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Number, this.Share);
    }

    public override string ToString()
    {
        return $"{this.Number:X}-{this.Share.ToString(16)}";
    }
}
