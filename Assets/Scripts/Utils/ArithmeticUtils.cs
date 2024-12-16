using System;

namespace Pika.Base.Utils
{
    public static class ArithmeticUtils
    {
        /// <summary>
        /// Wraps the given angle to the range [-PI, PI]
        /// </summary>
        /// <param name="a">The angle in radians</param>
        /// <returns>The given angle wrapped to the range [-PI, PI]</returns>
        public static float WrapAngleAroundZero(float a)
        {
            if (a >= 0)
            {
                float rotation = a % (float)(2 * Math.PI);
                if (rotation > (float)Math.PI) rotation -= (float)(2 * Math.PI);
                return rotation;
            }
            else
            {
                float rotation = -a % (float)(2 * Math.PI);
                if (rotation > (float)Math.PI) rotation -= (float)(2 * Math.PI);
                return -rotation;
            }
        }

        /// <summary>
        /// Returns the greatest common divisor of two positive numbers (this precondition is not checked and the
        /// result is undefined if not fulfilled) using the "binary gcd" method which avoids division and modulo operations.
        /// See Knuth 4.5.2 algorithm B. The algorithm is due to Josef Stein (1961).
        /// Special cases:
        /// The result of gcd(x, x), gcd(0, x) and gcd(x, 0) is the value of x.
        /// The invocation gcd(0, 0) is the only one which returns 0.
        /// </summary>
        /// <param name="a">A non negative number.</param>
        /// <param name="b">A non negative number.</param>
        /// <returns>The greatest common divisor.</returns>
        public static int GcdPositive(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        /// <summary>
        /// Returns the least common multiple of the absolute value of two numbers, using the formula
        /// lcm(a, b) = (a / gcd(a, b)) * b.
        /// Special cases:
        /// The invocations lcm(int.MinValue, n) and lcm(n, int.MinValue), where abs(n) is a power
        /// of 2, throw an ArithmeticException, because the result would be 2^31, which is too large for an int value.
        /// The result of lcm(0, x) and lcm(x, 0) is 0 for any x.
        /// </summary>
        /// <param name="a">A non-negative number.</param>
        /// <param name="b">A non-negative number.</param>
        /// <returns>The least common multiple, never negative.</returns>
        /// <exception cref="ArithmeticException">If the result cannot be represented as a non-negative int value.</exception>
        public static int LcmPositive(int a, int b)
        {
            if (a == 0 || b == 0) return 0;

            long lcm = Math.Abs(MulAndCheck((long)a / GcdPositive(a, b), b));
            if (lcm == int.MinValue)
            {
                throw new ArithmeticException($"overflow: lcm({a}, {b}) > 2^31");
            }
            return (int)lcm;
        }

        /// <summary>
        /// Returns the greatest common divisor of the given absolute values. This implementation uses GcdPositive(int, int) and
        /// has the same special cases.
        /// </summary>
        /// <param name="args">Non-negative numbers.</param>
        /// <returns>The greatest common divisor.</returns>
        /// <exception cref="ArgumentException">If fewer than two arguments are provided</exception>
        public static int GcdPositive(params int[] args)
        {
            if (args == null || args.Length < 2) throw new ArgumentException("gcdPositive requires at least two arguments");
            int result = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                result = GcdPositive(result, args[i]);
            }
            return result;
        }

        /// <summary>
        /// Returns the least common multiple of the given absolute values. This implementation uses LcmPositive(int, int) and
        /// has the same special cases.
        /// </summary>
        /// <param name="args">Non-negative numbers.</param>
        /// <returns>The least common multiple, never negative.</returns>
        /// <exception cref="ArithmeticException">If the result cannot be represented as a non-negative int value.</exception>
        /// <exception cref="ArgumentException">If fewer than two arguments are provided</exception>
        public static int LcmPositive(params int[] args)
        {
            if (args == null || args.Length < 2) throw new ArgumentException("lcmPositive requires at least two arguments");
            int result = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                result = LcmPositive(result, args[i]);
            }
            return result;
        }

        /// <summary>
        /// Multiply two integers, checking for overflow.
        /// </summary>
        /// <param name="x">First factor.</param>
        /// <param name="y">Second factor.</param>
        /// <returns>The product x * y.</returns>
        /// <exception cref="ArithmeticException">If the result cannot be represented as an int.</exception>
        public static long MulAndCheck(long x, long y)
        {
            if (x == 0 || y == 0) return 0;
            long m = x * y;
            if (m < int.MinValue || m > int.MaxValue)
            {
                throw new ArithmeticException("overflow");
            }
            return m;
        }
    }
}
