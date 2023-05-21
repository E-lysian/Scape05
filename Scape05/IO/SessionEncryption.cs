namespace Scape05.IO;

public class SessionEncryption
{
    private static long serverSessionKey;
    private readonly int[] cryptArray;
    private int cryptVar1;
    private int cryptVar2;
    private int cryptVar3;
    private int keyArrayIndex;
    private readonly int[] keySetArray;

    public SessionEncryption(int[] initialKeySet)
    {
        cryptArray = new int[256];
        keySetArray = new int[256];
        Array.Copy(initialKeySet, keySetArray, initialKeySet.Length);

        InitializeKeySet();
    }

    public static long GenerateServerSessionKey()
    {
        var random = new Random();
        serverSessionKey = ((long)(random.Next() * 99999999D) << 32) + (long)(random.Next() * 99999999D);
        return serverSessionKey;
    }

    public int GetNextKey()
    {
        if (keyArrayIndex-- == 0)
        {
            GenerateNextKeySet();
            keyArrayIndex = 255;
        }

        return keySetArray[keyArrayIndex];
    }

    private void GenerateNextKeySet()
    {
        cryptVar2 += ++cryptVar3;

        for (var i = 0; i < 256; i++)
        {
            var j = cryptArray[i];

            switch (i & 3)
            {
                case 0:
                    cryptVar1 ^= cryptVar1 << 13;
                    break;
                case 1:
                    cryptVar1 ^= (int)((uint)cryptVar1 >> 6);
                    break;
                case 2:
                    cryptVar1 ^= cryptVar1 << 2;
                    break;
                case 3:
                    cryptVar1 ^= (int)((uint)cryptVar1 >> 16);
                    break;
            }

            cryptVar1 += cryptArray[(i + 128) & 0xff];
            int k;
            cryptArray[i] = k = cryptArray[(j & 0x3fc) >> 2] + cryptVar1 + cryptVar2;
            keySetArray[i] = cryptVar2 = cryptArray[((k >> 8) & 0x3fc) >> 2] + j;
        }
    }

    private void InitializeKeySet()
    {
        const int initValue = unchecked((int)0x9e3779b9);
        int a = initValue,
            b = initValue,
            c = initValue,
            d = initValue,
            e = initValue,
            f = initValue,
            g = initValue,
            h = initValue;

        for (var i = 0; i < 4; i++)
        {
            a ^= b << 11;
            d += a;
            b += c;
            b ^= (int)((uint)c >> 2);
            e += b;
            c += d;
            c ^= d << 8;
            f += c;
            d += e;
            d ^= (int)((uint)e >> 16);
            g += d;
            e += f;
            e ^= f << 10;
            h += e;
            f += g;
            f ^= (int)((uint)g >> 4);
            a += f;
            g += h;
            g ^= h << 8;
            b += g;
            h += a;
            h ^= (int)((uint)a >> 9);
            c += h;
            a += b;
        }

        for (var j = 0; j < 256; j += 8)
        {
            a += keySetArray[j];
            b += keySetArray[j + 1];
            c += keySetArray[j + 2];
            d += keySetArray[j + 3];
            e += keySetArray[j + 4];
            f += keySetArray[j + 5];
            g += keySetArray[j + 6];
            h += keySetArray[j + 7];
            a ^= b << 11;
            d += a;
            b += c;
            b ^= (int)((uint)c >> 2);
            e += b;
            c += d;
            c ^= d << 8;
            f += c;
            d += e;
            d ^= (int)((uint)e >> 16);
            g += d;
            e += f;
            e ^= f << 10;
            h += e;
            f += g;
            f ^= (int)((uint)g >> 4);
            a += f;
            g += h;
            g ^= h << 8;
            b += g;
            h += a;
            h ^= (int)((uint)a >> 9);
            c += h;
            a += b;
            cryptArray[j] = a;
            cryptArray[j + 1] = b;
            cryptArray[j + 2] = c;
            cryptArray[j + 3] = d;
            cryptArray[j + 4] = e;
            cryptArray[j + 5] = f;
            cryptArray[j + 6] = g;
            cryptArray[j + 7] = h;
        }

        for (var k = 0; k < 256; k += 8)
        {
            a += cryptArray[k];
            b += cryptArray[k + 1];
            c += cryptArray[k + 2];
            d += cryptArray[k + 3];
            e += cryptArray[k + 4];
            f += cryptArray[k + 5];
            g += cryptArray[k + 6];
            h += cryptArray[k + 7];
            a ^= b << 11;
            d += a;
            b += c;
            b ^= (int)((uint)c >> 2);
            e += b;
            c += d;
            c ^= d << 8;
            f += c;
            d += e;
            d ^= (int)((uint)e >> 16);
            g += d;
            e += f;
            e ^= f << 10;
            h += e;
            f += g;
            f ^= (int)((uint)g >> 4);
            a += f;
            g += h;
            g ^= h << 8;
            b += g;
            h += a;
            h ^= (int)((uint)a >> 9);
            c += h;
            a += b;
            cryptArray[k] = a;
            cryptArray[k + 1] = b;
            cryptArray[k + 2] = c;
            cryptArray[k + 3] = d;
            cryptArray[k + 4] = e;
            cryptArray[k + 5] = f;
            cryptArray[k + 6] = g;
            cryptArray[k + 7] = h;
        }

        GenerateNextKeySet();
        keyArrayIndex = 256;
    }
}
