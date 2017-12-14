/*
  Copyright © 2017 László Csöndes

  This file is part of Secret Splitter.

  Secret Splitter is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  Secret Splitter is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with Secret Splitter. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Numerics;
using System.Text;

namespace SecretSplit
{
    static class StringFormatting
    {
        private static Encoding MyEncoding = Encoding.UTF8;
        private static int BitsForZero = 8; // How many bits does '\0' encode to

        public static BigInteger SecretToNumber(this string str)
        {
            var bitsUsed = (str.Length + 1) * BitsForZero;
            var fillerBits = (int)Crypto.MaxBits - bitsUsed;
            var totalBytes = new byte[Crypto.MaxBits / 8 + 1]; // Leave an extra byte to get an unsigned result
            var strBytes = MyEncoding.GetBytes(str);

            // Copy the secret's bytes to the start of the number
            Array.Copy(strBytes, totalBytes, strBytes.Length);
            // Leave a \0, and fill the rest with random
            Array.Copy(Crypto.RandomBytes(fillerBits / 8), 0, totalBytes, strBytes.Length + BitsForZero / 8, fillerBits / 8);

            return new BigInteger(totalBytes);
        }

        public static string TruncatedAndDecoded(this BigInteger n)
        {
            var bytes = n.ToByteArray();
            var chars = MyEncoding.GetChars(bytes);
            int size = 0;
            for (; size < chars.Length; ++size)
                if (chars[size] == '\0')
                    break;
            if (size == chars.Length)
                throw new ArgumentException("Number does not decode to a valid string");
            return new string(chars, 0, size);
        }

        public static string OutputPart(BigInteger value, int x, int partsNeeded)
            => $"{x}:{partsNeeded}:{Convert.ToBase64String(value.ToByteArray())}";

        public static BigInteger DecodeFunctionValue(this string str)
            => new BigInteger(Convert.FromBase64String(str.Split(':')[2]));

    }
}
