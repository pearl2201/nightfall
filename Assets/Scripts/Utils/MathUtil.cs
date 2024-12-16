using System;
using System.Collections.Generic;
using System.Linq;

namespace Pika.Base.Utils
{
    public static class MathUtil
    {
        public const float nanoToSec = 1 / 1000000000f;

        // ---
        public const float FLOAT_ROUNDING_ERROR = 0.000001f; // 32 bits
        public const int FLOAT_ROUND = 100000; // 32 bits
        public const float PI = 3.1415927f;
        public const float PI2 = PI * 2;

        public const float E = 2.7182818f;

        private const int SIN_BITS = 14; // 16KB. Adjust for accuracy.
        private const int SIN_MASK = ~(-1 << SIN_BITS);
        private const int SIN_COUNT = SIN_MASK + 1;

        private const float radFull = PI * 2;
        private const float degFull = 360;
        private const float radToIndex = SIN_COUNT / radFull;
        private const float degToIndex = SIN_COUNT / degFull;

        /**
         * multiply by this to convert from radians to degrees
         */
        public const float radiansToDegrees = 180f / PI;
        public const float radDeg = radiansToDegrees;
        /**
         * multiply by this to convert from degrees to radians
         */
        public const float degreesToRadians = PI / 180;
        public const float degRad = degreesToRadians;

        private static class Sin
        {

            public static float[] table = new float[SIN_COUNT];
            static Sin()
            {

                for (int i = 0; i < SIN_COUNT; i++)
                {
                    table[i] = (float)Math.Sin((i + 0.5f) / SIN_COUNT * radFull);
                }
                for (int i = 0; i < 360; i += 90)
                {
                    table[(int)(i * degToIndex) & SIN_MASK] = (float)Math.Sin(i * degreesToRadians);

                }
            }

        }



        /**
         * Returns the sine in radians from a lookup table.
         *
         * @return
         */
        public static float sin(float radians)
        {
            return Sin.table[(int)(radians * radToIndex) & SIN_MASK];
        }

        /**
         * Returns the cosine in radians from a lookup table.
         *
         * @return
         */
        public static float cos(float radians)
        {
            return Sin.table[(int)((radians + PI / 2) * radToIndex) & SIN_MASK];
        }

        public static float roundFloat(float value)
        {
            return (float)(Math.Round(value * FLOAT_ROUND)) / FLOAT_ROUND;
        }

        /**
         * Returns the sine in radians from a lookup table.
         *
         * @return
         */
        public static float sinDeg(float degrees)
        {
            return Sin.table[(int)(degrees * degToIndex) & SIN_MASK];
        }

        /**
         * Returns the cosine in radians from a lookup table.
         *
         * @return
         */
        public static float cosDeg(float degrees)
        {
            return Sin.table[(int)((degrees + 90) * degToIndex) & SIN_MASK];
        }

        // ---
        /**
         * Returns atan2 in radians, faster but less accurate than Math.atan2.
         * Average error of 0.00231 radians (0.1323 degrees), largest error of
         * 0.00488 radians (0.2796 degrees).
         *
         * @return
         */
        public static float atan2(float y, float x)
        {
            if (x == 0f)
            {
                if (y > 0f)
                {
                    return PI / 2;
                }
                if (y == 0f)
                {
                    return 0f;
                }
                return -PI / 2;
            }
            float atan, z = y / x;
            if (Math.Abs(z) < 1f)
            {
                atan = z / (1f + 0.28f * z * z);
                if (x < 0f)
                {
                    return atan + (y < 0f ? -PI : PI);
                }
                return atan;
            }
            atan = PI / 2 - z / (z * z + 0.28f);
            return y < 0f ? atan - PI : atan;
        }

        // ---
        public static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /**
         * Returns a Random number between 0 (inclusive) and the specified value
         * (inclusive).
         *
         * @param range
         * @return
         */
        public static int Random(int range)
        {

            return rng.Next(range + 1);
        }

        public static int floatToRawIntBits(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            int result = BitConverter.ToInt32(bytes, 0);
            return result;
        }

        public static int floatToIntBits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /**
         * Returns a Random number between start (inclusive) and end (inclusive).
         *
         * @return
         */
        public static int Random(int start, int end)
        {
            return start + rng.Next(end - start + 1);
        }

        /**
         * Returns a Random number between 0 (inclusive) and the specified value
         * (inclusive).
         *
         * @return
         */
        public static long Random(long range)
        {
            return (long)(rng.NextDouble() * range);
        }

        /**
         * Returns a Random number between start (inclusive) and end (inclusive).
         *
         * @return
         */
        public static long Random(long start, long end)
        {
            return start + (long)(rng.NextDouble() * (end - start));
        }

        /**
         * Returns a Random bool value.
         *
         * @return
         */
        public static bool Randombool()
        {
            return rng.Next(2) == 0;
        }

        /**
         * Returns true if a Random value between 0 and 1 is less than the specified
         * value.
         *
         * @param chance
         * @return
         */
        public static bool Randombool(float chance)
        {
            return MathUtil.Random() < chance;
        }

        /**
         * 根据几率 计算是否生成
         *
         * @param probability
         * @param bound
         * @return
         */
        public static bool Randombool(int probability, int bound)
        {
            if (bound == 0)
            {
                bound = 10000;
            }
            int Random_seed = rng.Next(bound + 1);
            return probability >= Random_seed;
        }

        /**
         * 根据几率 计算是否生成，种子数为10000
         *
         * @param probability
         * @return
         */
        public static bool Randombool(int probability)
        {
            int Random_seed = rng.Next(10000 + 1);
            return probability >= Random_seed;
        }

        /**
         * 从集合中随机一个元素
         *
         * @param <T>
         * @param IEnumerable
         * @return
         */
        public static T Random<T>(IEnumerable<T> IEnumerable)
        {
            if (IEnumerable == null || IEnumerable.Count() == 0)
            {
                return default(T);
            }
            int t = (int)(IEnumerable.Count() * rng.NextDouble());
            return IEnumerable.ElementAt(t);
        }

        /**
         * 从列表中随机num个不重复的数值
         *
         * @param IEnumerable
         * @param num
         * @return
         */
        public static List<T> RandomList<T>(IEnumerable<T> e, int num)
        {
            if (e.Count() < num)
            {
                return new List<T>(e);
            }
            List<T> list = new List<T>(num);
            List<T> RandomList = new List<T>(e);

            while (list.Count() < num)
            {
                int t = (int)(RandomList.Count() * rng.NextDouble());
                list.Add(RandomList[t]);
                RandomList.RemoveAt(t);
            }

            return list;
        }

        public static void RandomIndex<T>(List<T> list)
        {
            if (list == null || list.Count() == 0)
            {
                return;
            }
            int size = list.Count();
            list.Sort((T o1, T o2) => Random(-size, size));
        }

        /**
         * 从集合中随机移除一个元素
         *
         * @param <T>
         * @param IEnumerable
         * @return
         */
        public static T RandomRemove<T>(List<T> e)
        {
            if (e == null || e.Count() == 0)
            {
                return default(T);
            }
            int t = (int)(e.Count() * rng.NextDouble());
            int i = 0;
            var elem = e[t];
            e.RemoveAt(t);
            return elem;
        }

        /**
         * Returns Random number between 0.0 (inclusive) and 1.0 (exclusive).
         *
         * @return
         */
        public static float Random()
        {
            return (float)rng.NextDouble();
        }

        /**
         * Returns a Random number between 0 (inclusive) and the specified value
         * (exclusive).
         *
         * @param range
         * @return
         */
        public static float Random(float range)
        {
            return (float)rng.NextDouble() * range;
        }

        /**
         * Returns a Random number between start (inclusive) and end (exclusive).
         *
         * @return
         */
        public static float Random(float start, float end)
        {
            return start + (float)rng.NextDouble() * (end - start);
        }

        /**
         * Returns -1 or 1, Randomly.
         *
         * @return
         */
        public static int RandomSign()
        {
            return 1 | (rng.Next() >> 31);
        }

        /**
         * Returns a triangularly distributed Random number between -1.0 (exclusive)
         * and 1.0 (exclusive), where values around zero are more likely.
         * <p>
         * This is an optimized version of
         * {@link #RandomTriangular(float, float, float) RandomTriangular(-1, 1, 0)}
         *
         * @return
         */
        public static float RandomTriangular()
        {
            return (float)rng.NextDouble() - (float)rng.NextDouble();
        }

        /**
         * Returns a triangularly distributed Random number between {@code -max}
         * (exclusive) and {@code max} (exclusive), where values around zero are
         * more likely.
         * <p>
         * This is an optimized version of      {@link #RandomTriangular(float, float, float) RandomTriangular(-max, max,
         * 0)}
         *
         * @param max the upper limit
         * @return
         */
        public static float RandomTriangular(float max)
        {
            return (float)(rng.NextDouble() - rng.NextDouble()) * max;
        }

        /**
         * Returns a triangularly distributed Random number between {@code min}
         * (inclusive) and {@code max} (exclusive), where the {@code mode} argument
         * defaults to the midpoint between the bounds, giving a symmetric
         * distribution.
         * <p>
         * This method is equivalent of      {@link #RandomTriangular(float, float, float) RandomTriangular(min, max,
         * (min + max) * .5f)}
         *
         * @param min the lower limit
         * @param max the upper limit
         * @return
         */
        public static float RandomTriangular(float min, float max)
        {
            return RandomTriangular(min, max, (min + max) * 0.5f);
        }

        /**
         * Returns a triangularly distributed Random number between {@code min}
         * (inclusive) and {@code max} (exclusive), where values around {@code mode}
         * are more likely.
         *
         * @param min the lower limit
         * @param max the upper limit
         * @param mode the point around which the values are more likely
         * @return
         */
        public static float RandomTriangular(float min, float max, float mode)
        {
            float u = (float)rng.NextDouble();
            float d = max - min;
            if (u <= (mode - min) / d)
            {
                return min + (float)Math.Sqrt(u * d * (mode - min));
            }
            return max - (float)Math.Sqrt((1 - u) * d * (max - mode));
        }

        // ---
        /**
         * Returns the next power of two. Returns the specified value if the value
         * is already a power of two.
         *
         * @return
         */
        public static int nextPowerOfTwo(int value)
        {
            if (value == 0)
            {
                return 1;
            }
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        public static bool isPowerOfTwo(int value)
        {
            return value != 0 && (value & value - 1) == 0;
        }

        // ---
        public static short clamp(short value, short min, short max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        public static int clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        public static long clamp(long value, long min, long max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        public static float clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        public static double clamp(double value, double min, double max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        // ---
        /**
         * 插值
         * Linearly interpolates between fromValue to toValue on progress position.
         *
         * @return
         */
        public static float lerp(float fromValue, float toValue, float progress)
        {
            return fromValue + (toValue - fromValue) * progress;
        }

        /**
         * Linearly interpolates between two angles in radians. Takes into account
         * that angles wrap at two pi and always takes the direction with the
         * smallest delta angle.
         *
         * @param fromRadians start angle in radians
         * @param toRadians target angle in radians
         * @param progress interpolation value in the range [0, 1]
         * @return the interpolated angle in the range [0, PI2[
         */
        public static float lerpAngle(float fromRadians, float toRadians, float progress)
        {
            float delta = ((toRadians - fromRadians + PI2 + PI) % PI2) - PI;
            return (fromRadians + delta * progress + PI2) % PI2;
        }

        /**
         * Linearly interpolates between two angles in degrees. Takes into account
         * that angles wrap at 360 degrees and always takes the direction with the
         * smallest delta angle.
         *
         * @param fromDegrees start angle in degrees
         * @param toDegrees target angle in degrees
         * @param progress interpolation value in the range [0, 1]
         * @return the interpolated angle in the range [0, 360[
         */
        public static float lerpAngleDeg(float fromDegrees, float toDegrees, float progress)
        {
            float delta = ((toDegrees - fromDegrees + 360 + 180) % 360) - 180;
            return (fromDegrees + delta * progress + 360) % 360;
        }

        // ---
        private const int BIG_ENOUGH_INT = 16 * 1024;
        private const double BIG_ENOUGH_FLOOR = BIG_ENOUGH_INT;
        private const double CEIL = 0.9999999;
        private const double BIG_ENOUGH_CEIL = 16384.999999999996;
        private const double BIG_ENOUGH_ROUND = BIG_ENOUGH_INT + 0.5f;

        public static int floor(float value)
        {
            return (int)(value + BIG_ENOUGH_FLOOR) - BIG_ENOUGH_INT;
        }

        public static int floorPositive(float value)
        {
            return (int)value;
        }

        public static int ceil(float value)
        {
            return BIG_ENOUGH_INT - (int)(BIG_ENOUGH_FLOOR - value);
        }

        public static int ceilPositive(float value)
        {
            return (int)(value + CEIL);
        }

        public static int round(float value)
        {
            return (int)(value + BIG_ENOUGH_ROUND) - BIG_ENOUGH_INT;
        }

        public static int roundPositive(float value)
        {
            return (int)(value + 0.5f);
        }

        public static bool isZero(float value)
        {
            return Math.Abs(value) <= FLOAT_ROUNDING_ERROR;
        }

        public static bool isZero(float value, float tolerance)
        {
            return Math.Abs(value) <= tolerance;
        }

        public static bool isEqual(float a, float b)
        {
            return Math.Abs(a - b) <= FLOAT_ROUNDING_ERROR;
        }

        public static bool isEqual(float a, float b, float tolerance)
        {
            return Math.Abs(a - b) <= tolerance;
        }

        public static float log(float a, float value)
        {
            return (float)(Math.Log(value) / Math.Log(a));
        }

        public static float log2(float value)
        {
            return log(2, value);
        }
    }
}
