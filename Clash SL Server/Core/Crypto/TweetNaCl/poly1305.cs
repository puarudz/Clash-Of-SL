﻿/*
 * Program : Clash Of SL Server
 * Description : A C# Writted 'Clash of SL' Server Emulator !
 *
 * Authors:  Sky Tharusha <Founder at Sky Production>,
 *           And the Official DARK Developement Team
 *
 * Copyright (c) 2021  Sky Production
 * All Rights Reserved.
 */

namespace CSS.Core.Crypto.TweetNaCl
{
    public class poly1305
    {
        internal readonly int CRYPTO_BYTES = 16;
        internal readonly int CRYPTO_KEYBYTES = 32;

        internal static readonly int[] minusp = new int[] {5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 252};

        public static int crypto_onetimeauth_verify(byte[] h, int hoffset, byte[] inv, int invoffset, long inlen, byte[] k)
        {
            byte[] correct = new byte[16];

            crypto_onetimeauth(correct, 0, inv, invoffset, inlen, k);
            return verify_16.crypto_verify(h, hoffset, correct);
        }

        internal static void add(int[] h, int[] c)
        {
            int j;
            int u = 0;

            for (j = 0; j < 17; ++j)
            {
                u += h[j] + c[j];
                h[j] = u & 255;
                u = (int) ((uint) u >> 8);
            }
        }

        internal static void squeeze(int[] h)
        {
            int u = 0;

            for (int j = 0; j < 16; ++j)
            {
                u += h[j];
                h[j] = u & 255;
                u = (int) ((uint) u >> 8);
            }

            u += h[16];
            h[16] = u & 3;
            u = 5 * ((int) ((uint) u >> 2));

            for (int j = 0; j < 16; ++j)
            {
                u += h[j];
                h[j] = u & 255;
                u = (int) ((uint) u >> 8);
            }

            u += h[16];
            h[16] = u;
        }

        internal static void freeze(int[] h)
        {
            int[] horig = new int[17];

            for (int j = 0; j < 17; ++j)
            {
                horig[j] = h[j];
            }

            add(h, minusp);

            int negative = (int) (-((int) ((uint) h[16] >> 7)));

            for (int j = 0; j < 17; ++j)
            {
                h[j] ^= negative & (horig[j] ^ h[j]);
            }
        }

        internal static void mulmod(int[] h, int[] r)
        {
            int[] hr = new int[17];

            for (int i = 0; i < 17; ++i)
            {
                int u = 0;

                for (int j = 0; j <= i; ++j)
                {
                    u += h[j] * r[i - j];
                }

                for (int j = i + 1; j < 17; ++j)
                {
                    u += 320 * h[j] * r[i + 17 - j];
                }

                hr[i] = u;
            }

            for (int i = 0; i < 17; ++i)
            {
                h[i] = hr[i];
            }

            squeeze(h);
        }

        public static int crypto_onetimeauth(byte[] outv, int outvoffset, byte[] inv, int invoffset, long inlen, byte[] k)
        {
            int j;
            int[] r = new int[17];
            int[] h = new int[17];
            int[] c = new int[17];

            r[0] = k[0] & 0xFF;
            r[1] = k[1] & 0xFF;
            r[2] = k[2] & 0xFF;
            r[3] = k[3] & 15;
            r[4] = k[4] & 252;
            r[5] = k[5] & 0xFF;
            r[6] = k[6] & 0xFF;
            r[7] = k[7] & 15;
            r[8] = k[8] & 252;
            r[9] = k[9] & 0xFF;
            r[10] = k[10] & 0xFF;
            r[11] = k[11] & 15;
            r[12] = k[12] & 252;
            r[13] = k[13] & 0xFF;
            r[14] = k[14] & 0xFF;
            r[15] = k[15] & 15;
            r[16] = 0;

            for (j = 0; j < 17; ++j)
            {
                h[j] = 0;
            }

            while (inlen > 0)
            {
                for (j = 0; j < 17; ++j)
                {
                    c[j] = 0;
                }

                for (j = 0; (j < 16) && (j < inlen); ++j)
                {
                    c[j] = inv[invoffset + j] & 0xff;
                }

                c[j] = 1;
                invoffset += j;
                inlen -= j;
                add(h, c);
                mulmod(h, r);
            }

            freeze(h);

            for (j = 0; j < 16; ++j)
            {
                c[j] = k[j + 16] & 0xFF;
            }

            c[16] = 0;
            add(h, c);

            for (j = 0; j < 16; ++j)
            {
                outv[j + outvoffset] = (byte) h[j];
            }

            return 0;
        }
    }
}