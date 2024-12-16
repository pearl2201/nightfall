using Pika.Base.Utils;
using System;

namespace Pika.Base.Mathj
{
    public abstract class Interpolation
    {
        /**
	 * @param a
	 *            Alpha value between 0 and 1.
	 */
        public abstract float apply(float a);

        /**
         * @param a
         *            Alpha value between 0 and 1.
         */
        public float apply(float start, float end, float a)
        {
            return start + (end - start) * this.apply(a);
        }

        //

        public class Linear : Interpolation
        {

            public override float apply(float a)
            {
                return a;
            }
        };

        //

        /** Aka "smoothstep". */
        public class smooth : Interpolation
        {

            public override float apply(float a)
            {
                return a * a * (3 - 2 * a);
            }
        };
        public class smooth2 : Interpolation
        {

            public override float apply(float a)
            {
                a = a * a * (3 - 2 * a);
                return a * a * (3 - 2 * a);
            }
        };

        /** By Ken Perlin. */
        public class smoother : Interpolation
        {

            public override float apply(float a)

            {
                return MathUtil.clamp(a * a * a * (a * (a * 6 - 15) + 10), 0, 1);
            }
        };
        public static Interpolation fade = new smoother();

        //

        public static Pow pow2 = new Pow(2);
        /** Slow, then fast. */
        public static PowIn pow2In = new PowIn(2);
        /** Fast, then slow. */
        public static PowOut pow2Out = new PowOut(2);
        public class pow2InInverse : Interpolation
        {

            public override float apply(float a)
            {
                return (float)Math.Sqrt(a);
            }
        };
        public class pow2OutInverse : Interpolation
        {

            public override float apply(float a)
            {
                return 1 - (float)Math.Sqrt(-(a - 1));
            }
        };

        public static Pow pow3 = new Pow(3);
        public static PowIn pow3In = new PowIn(3);
        public static PowOut pow3Out = new PowOut(3);
        public class pow3InInverse : Interpolation
        {

            public override float apply(float a)
            {
                return (float)Math.Pow(a, (1.0 / 3.0));
            }
        };
        public class pow3OutInverse : Interpolation
        {

            public override float apply(float a)
            {
                return 1 - (float)Math.Pow(-(a - 1), (1.0 / 3.0));
            }
        };

        public static Pow pow4 = new Pow(4);
        public static PowIn pow4In = new PowIn(4);
        public static PowOut pow4Out = new PowOut(4);

        public static Pow pow5 = new Pow(5);
        public static PowIn pow5In = new PowIn(5);
        public static PowOut pow5Out = new PowOut(5);

        public class sine : Interpolation
        {

            public override float apply(float a)
            {
                return (1 - MathUtil.cos(a * MathUtil.PI)) / 2;
            }
        };

        public class sineIn : Interpolation
        {

            public override float apply(float a)
            {
                return 1 - MathUtil.cos(a * MathUtil.PI / 2);
            }
        };

        public class sineOut : Interpolation
        {

            public override float apply(float a)
            {
                return MathUtil.sin(a * MathUtil.PI / 2);
            }
        };

        public static Exp exp10 = new Exp(2, 10);
        public static ExpIn exp10In = new ExpIn(2, 10);
        public static ExpOut exp10Out = new ExpOut(2, 10);

        public static Exp exp5 = new Exp(2, 5);
        public static ExpIn exp5In = new ExpIn(2, 5);
        public static ExpOut exp5Out = new ExpOut(2, 5);

        public class circle : Interpolation
        {

            public override float apply(float a)
            {
                if (a <= 0.5f)
                {
                    a *= 2;
                    return (1 - (float)Math.Sqrt(1 - a * a)) / 2;
                }
                a--;
                a *= 2;
                return ((float)Math.Sqrt(1 - a * a) + 1) / 2;
            }
        };

        public class circleIn : Interpolation
        {

            public override float apply(float a)
            {
                return 1 - (float)Math.Sqrt(1 - a * a);
            }
        };

        public class circleOut : Interpolation
        {

            public override float apply(float a)
            {
                a--;
                return (float)Math.Sqrt(1 - a * a);
            }
        };

        public static Elastic elastic = new Elastic(2, 10, 7, 1);
        public static ElasticIn elasticIn = new ElasticIn(2, 10, 6, 1);
        public static ElasticOut elasticOut = new ElasticOut(2, 10, 7, 1);

        public static Swing swing = new Swing(1.5f);
        public static SwingIn swingIn = new SwingIn(2f);
        public static SwingOut swingOut = new SwingOut(2f);

        public static Bounce bounce = new Bounce(4);
        public static BounceIn bounceIn = new BounceIn(4);
        public static BounceOut bounceOut = new BounceOut(4);

        //

        public class Pow : Interpolation
        {
            protected int power;

            public Pow(int power)
            {
                this.power = power;
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                    return (float)Math.Pow(a * 2, power) / 2;
                return (float)Math.Pow((a - 1) * 2, power) / (power % 2 == 0 ? -2 : 2) + 1;
            }
        }

        public class PowIn : Pow
        {
            public PowIn(int power) : base(power)
            {

            }

            public override float apply(float a)
            {
                return (float)Math.Pow(a, power);
            }
        }

        public class PowOut : Pow
        {
            public PowOut(int power) : base(power)
            {

            }

            public override float apply(float a)
            {
                return (float)Math.Pow(a - 1, power) * (power % 2 == 0 ? -1 : 1) + 1;
            }
        }

        //

        public class Exp : Interpolation
        {
            protected float value, power, min, scale;

            public Exp(float value, float power)
            {
                this.value = value;
                this.power = power;
                min = (float)Math.Pow(value, -power);
                scale = 1 / (1 - min);
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                    return ((float)Math.Pow(value, power * (a * 2 - 1)) - min) * scale / 2;
                return (2 - ((float)Math.Pow(value, -power * (a * 2 - 1)) - min) * scale) / 2;
            }
        };

        public class ExpIn : Exp
        {
            public ExpIn(float value, float power) : base(value, power)
            {

            }

            public override float apply(float a)
            {
                return ((float)Math.Pow(value, power * (a - 1)) - min) * scale;
            }
        }

        public class ExpOut : Exp
        {
            public ExpOut(float value, float power) : base(value, power)
            {

            }

            public override float apply(float a)
            {
                return 1 - ((float)Math.Pow(value, -power * a) - min) * scale;
            }
        }

        //

        public class Elastic : Interpolation
        {
            protected float value, power, scale, bounces;

            public Elastic(float value, float power, int bounces, float scale)
            {
                this.value = value;
                this.power = power;
                this.scale = scale;
                this.bounces = bounces * MathUtil.PI * (bounces % 2 == 0 ? 1 : -1);
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                {
                    a *= 2;
                    return (float)Math.Pow(value, power * (a - 1)) * MathUtil.sin(a * bounces) * scale / 2;
                }
                a = 1 - a;
                a *= 2;
                return 1 - (float)Math.Pow(value, power * (a - 1)) * MathUtil.sin((a) * bounces) * scale / 2;
            }
        }

        public class ElasticIn : Elastic
        {
            public ElasticIn(float value, float power, int bounces, float scale) : base(value, power, bounces, scale)
            {

            }

            public override float apply(float a)
            {
                if (a >= 0.99)
                    return 1;
                return (float)Math.Pow(value, power * (a - 1)) * MathUtil.sin(a * bounces) * scale;
            }
        }

        public class ElasticOut : Elastic
        {
            public ElasticOut(float value, float power, int bounces, float scale) : base(value, power, bounces, scale)
            {

            }

            public override float apply(float a)
            {
                if (a == 0)
                    return 0;
                a = 1 - a;
                return (1 - (float)Math.Pow(value, power * (a - 1)) * MathUtil.sin(a * bounces) * scale);
            }
        }

        //

        public class Bounce : BounceOut
        {
            public Bounce(float[] widths, float[] heights) : base(widths, heights)
            {

            }

            public Bounce(int bounces) : base(bounces)
            {

            }

            private float outV(float a)
            {
                float test = a + widths[0] / 2;
                if (test < widths[0])
                    return test / (widths[0] / 2) - 1;
                return base.apply(a);
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                    return (1 - outV(1 - a * 2)) / 2;
                return outV(a * 2 - 1) / 2 + 0.5f;
            }
        }

        public class BounceOut : Interpolation
        {
            protected float[] widths, heights;

            public BounceOut(float[] widths, float[] heights)
            {
                if (widths.Length != heights.Length)
                    throw new InvalidOperationException("Must be the same number of widths and heights.");
                this.widths = widths;
                this.heights = heights;
            }

            public BounceOut(int bounces)
            {
                if (bounces < 2 || bounces > 5)
                    throw new InvalidOperationException("bounces cannot be < 2 or > 5: " + bounces);
                widths = new float[bounces];
                heights = new float[bounces];
                heights[0] = 1;
                switch (bounces)
                {
                    case 2:
                        widths[0] = 0.6f;
                        widths[1] = 0.4f;
                        heights[1] = 0.33f;
                        break;
                    case 3:
                        widths[0] = 0.4f;
                        widths[1] = 0.4f;
                        widths[2] = 0.2f;
                        heights[1] = 0.33f;
                        heights[2] = 0.1f;
                        break;
                    case 4:
                        widths[0] = 0.34f;
                        widths[1] = 0.34f;
                        widths[2] = 0.2f;
                        widths[3] = 0.15f;
                        heights[1] = 0.26f;
                        heights[2] = 0.11f;
                        heights[3] = 0.03f;
                        break;
                    case 5:
                        widths[0] = 0.3f;
                        widths[1] = 0.3f;
                        widths[2] = 0.2f;
                        widths[3] = 0.1f;
                        widths[4] = 0.1f;
                        heights[1] = 0.45f;
                        heights[2] = 0.3f;
                        heights[3] = 0.15f;
                        heights[4] = 0.06f;
                        break;
                }
                widths[0] *= 2;
            }

            public override float apply(float a)
            {
                if (a == 1)
                    return 1;
                a += widths[0] / 2;
                float width = 0, height = 0;
                for (int i = 0, n = widths.Length; i < n; i++)
                {
                    width = widths[i];
                    if (a <= width)
                    {
                        height = heights[i];
                        break;
                    }
                    a -= width;
                }
                a /= width;
                float z = 4 / width * height * a;
                return 1 - (z - z * a) * width;
            }
        }

        public class BounceIn : BounceOut
        {
            public BounceIn(float[] widths, float[] heights) : base(widths, heights)
            {

            }

            public BounceIn(int bounces) : base(bounces)
            {

            }

            public override float apply(float a)
            {
                return 1 - base.apply(1 - a);
            }
        }

        //

        public class Swing : Interpolation
        {
            private float scale;

            public Swing(float scale)
            {
                this.scale = scale * 2;
            }

            public override float apply(float a)
            {
                if (a <= 0.5f)
                {
                    a *= 2;
                    return a * a * ((scale + 1) * a - scale) / 2;
                }
                a--;
                a *= 2;
                return a * a * ((scale + 1) * a + scale) / 2 + 1;
            }
        }

        public class SwingOut : Interpolation
        {
            private float scale;

            public SwingOut(float scale)
            {
                this.scale = scale;
            }

            public override float apply(float a)
            {
                a--;
                return a * a * ((scale + 1) * a + scale) + 1;
            }
        }

        public class SwingIn : Interpolation
        {
            private float scale;

            public SwingIn(float scale)
            {
                this.scale = scale;
            }

            public override float apply(float a)
            {
                return a * a * ((scale + 1) * a - scale);
            }
        }
    }
}