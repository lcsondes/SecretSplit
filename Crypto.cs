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
using System.Security.Cryptography;

namespace SecretSplit
{
    static class Crypto
    {
        // Change the constants here
        public static readonly uint MaxBits = 256; // Max number of bits for "secret"
        public static readonly BigInteger FieldPrime = BigInteger.Pow(new BigInteger(2), 521) - 1; // A prime number larger than 2^MaxBits
        // -------------------------

        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static byte[] RandomBytes(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var bytes = new byte[count];
            rng.GetBytes(bytes);
            return bytes;
        }

        public static BigInteger Random()
        {
            // Use 1 more byte to get an unsigned result
            var bytes = new byte[MaxBits / 8 + 1];
            rng.GetBytes(bytes, 0, bytes.Length - 1);
            return new BigInteger(bytes);
        }
    }
}
