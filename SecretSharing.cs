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

namespace SecretSplit
{
    static class SecretSharing
    {
        public static string[] SplitSecret(string secret, int neededParts, int totalParts)
        {
            if (neededParts < 1)
                throw new ArgumentOutOfRangeException(nameof(neededParts));
            if (totalParts < 2)
                throw new ArgumentOutOfRangeException(nameof(totalParts));
            if (neededParts > totalParts)
                throw new ArgumentException("Can't require more parts than the total");

            // Construct a (neededParts-1) order polynomial
            var coefficients = new BigInteger[neededParts];
            coefficients[0] = secret.SecretToNumber(); // p(0) = secret
            for (var i = 1; i < coefficients.Length; ++i)
                coefficients[i] = Crypto.Random();

            // Evaluate the polynomial at 1,2,...,totalParts
            var output = new string[totalParts];
            for (var i = 1; i <= output.Length; ++i)
            {
                BigInteger value = coefficients[0];
                for (int k = 1; k < coefficients.Length; ++k)
                {
                    value += coefficients[k] * BigInteger.Pow(new BigInteger(i), k);
                    value %= Crypto.FieldPrime;
                }
                output[i - 1] = StringFormatting.OutputPart(value, i, neededParts);
            }
            return output;
        }

        private static (int[], int) ValidateParts(string[] parts)
        {
            if (parts.Length == 0)
                throw new ArgumentException("No parts given");

            var indices = new int[parts.Length];
            var neededCounts = new int[parts.Length];
            for (var i = 0; i < parts.Length; ++i)
            {
                var split = parts[i].Split(':');
                if (split.Length != 3)
                    throw new ArgumentException($"Part {i + 1} invalid");
                indices[i] = int.Parse(split[0]);
                neededCounts[i] = int.Parse(split[1]);
            }
            var neededCount = neededCounts[0];
            for (var i = 1; i < neededCounts.Length; ++i)
                if (neededCounts[i] != neededCount)
                    throw new ArgumentException($"Part {i + 1} invalid");

            return (indices, neededCount);
        }

        public static string MergeSecret(string[] parts)
        {
            var (indices, neededCount) = ValidateParts(parts);
            var value = BigInteger.Zero;

            for (var p = 0; p < neededCount; ++p)
            {
                var numerator = parts[p].DecodeFunctionValue();
                var denominator = BigInteger.One;
                for (var i = 0; i < neededCount; ++i)
                {
                    if (i == p)
                        continue;
                    (numerator, denominator) = BigMath.MultiplyRational(numerator, denominator, indices[i], indices[i] - indices[p]);
                }
                value += BigMath.RationalToWhole(numerator, denominator);
                value %= Crypto.FieldPrime;
            }

            return value.TruncatedAndDecoded();
        }
    }
}
