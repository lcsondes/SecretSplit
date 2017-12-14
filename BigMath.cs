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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Numerics;

namespace SecretSplit
{
    static class BigMath
    {
        [Conditional("DEBUG")]
        private static void CheckRange(this BigInteger n)
        {
            Contract.Assert(n >= 0);
            Contract.Assert(n < Crypto.FieldPrime);
        }

        private static BigInteger Abs(this BigInteger n)
        {
            return (n % Crypto.FieldPrime + Crypto.FieldPrime) % Crypto.FieldPrime;
        }

        private static BigInteger GCD(BigInteger a, BigInteger b)
        {
            a.CheckRange(); b.CheckRange();

            if (b == 0)
                return a;
            return GCD(b, a % b);
        }

        private static (BigInteger gcd, BigInteger invA, BigInteger invB) GCD2(BigInteger a, BigInteger b)
        {
            if (b == 0)
                return (a, 1, 0);
            var div = BigInteger.DivRem(a, b, out BigInteger rem);
            var (g, iA, iB) = GCD2(b, rem);
            return (g, iB, iA - iB * div);
        }

        public static (BigInteger numerator, BigInteger denominator) MultiplyRational(
            BigInteger numeratorLhs, BigInteger denominatorLhs,
            BigInteger numeratorRhs, BigInteger denominatorRhs)
        {
            denominatorRhs = denominatorRhs.Abs();

            numeratorLhs.CheckRange(); denominatorLhs.CheckRange();
            numeratorRhs.CheckRange(); denominatorRhs.CheckRange();

            if (denominatorLhs == 0)
                throw new ArgumentOutOfRangeException("LHS denominator is zero");
            if (denominatorRhs == 0)
                throw new ArgumentOutOfRangeException("RHS denominator is zero");

            var numerator = (numeratorLhs * numeratorRhs) % Crypto.FieldPrime;
            var denominator = (denominatorLhs * denominatorRhs) % Crypto.FieldPrime;
            var gcd = GCD(numerator, denominator);
            return (numerator / gcd, denominator / gcd);
        }

        private static BigInteger Inverse(BigInteger n)
        {
            n.CheckRange();
            var inverse = GCD2(Crypto.FieldPrime, n).invB.Abs();
            Contract.Assert((n * inverse) % Crypto.FieldPrime == 1);
            return inverse;
        }

        public static BigInteger RationalToWhole(BigInteger numerator, BigInteger denominator)
        {
            numerator.CheckRange(); denominator.CheckRange();

            // Find the multiplicative inverse of the denominator over the field
            var invD = Inverse(denominator);
            return numerator * invD % Crypto.FieldPrime;
        }
    }
}
