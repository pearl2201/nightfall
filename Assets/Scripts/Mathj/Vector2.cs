//https://github.com/jzyong/javalib/blob/main/javalib-base/src/main/java/com/jzy/javalib/base/util/HttpUtil.java
using Pika.Base.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Base.Mathj.Geometry
{
    public class Vector2 : Vector<Vector2>
    {
        public static Vector2 X = new Vector2(1, 0);
        public static Vector2 Y = new Vector2(0, 1);
        public static Vector2 Zero = new Vector2(0, 0);

        /** the x-component of this vector **/
        public float x;
        /** the y-component of this vector **/
        public float y;

        /** Constructs a new vector at (0,0) */
        public Vector2()
        {
        }

        /**
         * Constructs a vector with the given components
         * 
         * @param x
         *            The x-component
         * @param y
         *            The y-component
         */
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /**
         * Constructs a vector from the given vector
         * 
         * @param v
         *            The vector
         */
        public Vector2(Vector2 v)
        {
            set(v);
        }

        // 
        public Vector2 cpy()
        {
            return new Vector2(this);
        }

        public static float len(float x, float y)
        {
            return (float)Math.Sqrt(x * x + y * y);
        }


        public float len()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public static float len2(float x, float y)
        {
            return x * x + y * y;
        }


        public float len2()
        {
            return x * x + y * y;
        }


        public Vector2 set(Vector2 v)
        {
            x = v.x;
            y = v.y;
            return this;
        }

        /**
         * Sets the components of this vector
         * 
         * @param x
         *            The x-component
         * @param y
         *            The y-component
         * @return This vector for chaining
         */
        public Vector2 set(float x, float y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        // 
        public Vector2 sub(Vector2 v)
        {
            x -= v.x;
            y -= v.y;
            return this;
        }

        /**
         * Substracts the other vector from this vector.
         * 
         * @param x
         *            The x-component of the other vector
         * @param y
         *            The y-component of the other vector
         * @return This vector for chaining
         */
        public Vector2 sub(float x, float y)
        {
            this.x -= x;
            this.y -= y;
            return this;
        }


        public Vector2 nor()
        {
            float len = this.len();
            if (len != 0)
            {
                x /= len;
                y /= len;
            }
            return this;
        }


        public Vector2 add(Vector2 v)
        {
            x += v.x;
            y += v.y;
            return this;
        }

        /**
         * Adds the given components to this vector
         * 
         * @param x
         *            The x-component
         * @param y
         *            The y-component
         * @return This vector for chaining
         */
        public Vector2 add(float x, float y)
        {
            this.x += x;
            this.y += y;
            return this;
        }

        public static float dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        // 
        public float dot(Vector2 v)
        {
            return x * v.x + y * v.y;
        }

        public float dot(float ox, float oy)
        {
            return x * ox + y * oy;
        }


        public Vector2 scl(float scalar)
        {
            x *= scalar;
            y *= scalar;
            return this;
        }

        /**
         * Multiplies this vector by a scalar
         * 
         * @return This vector for chaining
         */
        public Vector2 scl(float x, float y)
        {
            this.x *= x;
            this.y *= y;
            return this;
        }


        public Vector2 scl(Vector2 v)
        {
            this.x *= v.x;
            this.y *= v.y;
            return this;
        }

        // 
        public Vector2 mulAdd(Vector2 vec, float scalar)
        {
            this.x += vec.x * scalar;
            this.y += vec.y * scalar;
            return this;
        }

        // 
        public Vector2 mulAdd(Vector2 vec, Vector2 mulVec)
        {
            this.x += vec.x * mulVec.x;
            this.y += vec.y * mulVec.y;
            return this;
        }

        public static float dst(float x1, float y1, float x2, float y2)
        {
            float x_d = x2 - x1;
            float y_d = y2 - y1;
            return (float)Math.Sqrt(x_d * x_d + y_d * y_d);
        }

        // 
        public float dst(Vector2 v)
        {
            float x_d = v.x - x;
            float y_d = v.y - y;
            return (float)Math.Sqrt(x_d * x_d + y_d * y_d);
        }

        /**
         * @param x
         *            The x-component of the other vector
         * @param y
         *            The y-component of the other vector
         * @return the distance between this and the other vector
         */
        public float dst(float x, float y)
        {
            float x_d = x - this.x;
            float y_d = y - this.y;
            return (float)Math.Sqrt(x_d * x_d + y_d * y_d);
        }

        public static float dst2(float x1, float y1, float x2, float y2)
        {
            float x_d = x2 - x1;
            float y_d = y2 - y1;
            return x_d * x_d + y_d * y_d;
        }

        // 
        public float dst2(Vector2 v)
        {
            float x_d = v.x - x;
            float y_d = v.y - y;
            return x_d * x_d + y_d * y_d;
        }

        /**
         * @param x
         *            The x-component of the other vector
         * @param y
         *            The y-component of the other vector
         * @return the squared distance between this and the other vector
         */
        public float dst2(float x, float y)
        {
            float x_d = x - this.x;
            float y_d = y - this.y;
            return x_d * x_d + y_d * y_d;
        }

        // 
        public Vector2 limit(float limit)
        {
            return limit2(limit * limit);
        }

        // 
        public Vector2 limit2(float limit2)
        {
            float len2 = this.len2();
            if (len2 > limit2)
            {
                return scl((float)Math.Sqrt(limit2 / len2));
            }
            return this;
        }

        // 
        public Vector2 clamp(float min, float max)
        {
            float len2 = this.len2();
            if (len2 == 0f)
                return this;
            float max2 = max * max;
            if (len2 > max2)
                return scl((float)Math.Sqrt(max2 / len2));
            float min2 = min * min;
            if (len2 < min2)
                return scl((float)Math.Sqrt(min2 / len2));
            return this;
        }

        // 
        public Vector2 setLength(float len)
        {
            return setLength2(len * len);
        }

        // 
        public Vector2 setLength2(float len2)
        {
            float oldLen2 = this.len2();
            return (oldLen2 == 0 || oldLen2 == len2) ? this : scl((float)Math.Sqrt(len2 / oldLen2));
        }

        /**
         * Converts this {@code Vector2} to a string in the format {@code (x,y)}.
         * 
         * @return a string representation of this object.
         */

        public String toString()
        {
            return "(" + x + "," + y + ")";
        }

        /**
         * Sets this {@code Vector2} to the value represented by the specified string
         * according to the format of {@link #toString()}.
         * 
         * @param v
         *            the string.
         * @return this vector for chaining
         */
        public Vector2 fromString(string v)
        {
            int s = v.IndexOf(',', 1);
            if (s != -1 && v[0] == '(' && v[v.Length - 1] == ')')
            {
                try
                {
                    float x = float.Parse(v.Substring(1, s));
                    float y = float.Parse(v.Substring(s + 1, v.Length - 1));
                    return this.set(x, y);
                }
                catch (FormatException ex)
                {
                    // Throw a GdxRuntimeException
                }
            }
            throw new Exception("Malformed Vector2: " + v);
        }

        /**
         * Left-multiplies this vector by the given matrix
         * 
         * @param mat
         *            the matrix
         * @return this vector
         */
        public Vector2 mul(Matrix3 mat)
        {
            float x = this.x * mat.val[0] + this.y * mat.val[3] + mat.val[6];
            float y = this.x * mat.val[1] + this.y * mat.val[4] + mat.val[7];
            this.x = x;
            this.y = y;
            return this;
        }

        /**
         * Calculates the 2D cross product between this and the given vector.
         * 
         * @param v
         *            the other vector
         * @return the cross product
         */
        public float crs(Vector2 v)
        {
            return this.x * v.y - this.y * v.x;
        }

        /**
         * Calculates the 2D cross product between this and the given vector.
         * 
         * @param x
         *            the x-coordinate of the other vector
         * @param y
         *            the y-coordinate of the other vector
         * @return the cross product
         */
        public float crs(float x, float y)
        {
            return this.x * y - this.y * x;
        }

        /**
         * @return the angle in degrees of this vector (point) relative to the x-axis.
         *         Angles are towards the positive y-axis (typically counter-clockwise)
         *         and between 0 and 360.
         */
        public float angle()
        {
            float angle = (float)Math.Atan2(y, x) * MathUtil.radiansToDegrees;
            if (angle < 0)
                angle += 360;
            return angle;
        }

        /**
         * @return the angle in degrees of this vector (point) relative to the given
         *         vector. Angles are towards the positive y-axis (typically
         *         counter-clockwise.) between -180 and +180
         */
        public float angle(Vector2 reference)
        {
            return (float)Math.Atan2(crs(reference), dot(reference)) * MathUtil.radiansToDegrees;
        }

        /**
         * @return the angle in radians of this vector (point) relative to the x-axis.
         *         Angles are towards the positive y-axis. (typically counter-clockwise)
         */
        public float angleRad()
        {
            return (float)Math.Atan2(y, x);
        }

        /**
         * @return the angle in radians of this vector (point) relative to the given
         *         vector. Angles are towards the positive y-axis. (typically
         *         counter-clockwise.)
         */
        public float angleRad(Vector2 reference)
        {
            return (float)Math.Atan2(crs(reference), dot(reference));
        }

        /**
         * Sets the angle of the vector in degrees relative to the x-axis, towards the
         * positive y-axis (typically counter-clockwise).
         * 
         * @param degrees
         *            The angle in degrees to set.
         */
        public Vector2 setAngle(float degrees)
        {
            return setAngleRad(degrees * MathUtil.degreesToRadians);
        }

        /**
         * Sets the angle of the vector in radians relative to the x-axis, towards the
         * positive y-axis (typically counter-clockwise).
         * 
         * @param radians
         *            The angle in radians to set.
         */
        public Vector2 setAngleRad(float radians)
        {
            this.set(len(), 0f);
            this.rotateRad(radians);

            return this;
        }

        /**
         * Rotates the Vector2 by the given angle, counter-clockwise assuming the y-axis
         * points up.
         * 
         * @param degrees
         *            the angle in degrees
         */
        public Vector2 rotate(float degrees)
        {
            return rotateRad(degrees * MathUtil.degreesToRadians);
        }

        /**
         * Rotates the Vector2 by the given angle, counter-clockwise assuming the y-axis
         * points up.
         * 
         * @param radians
         *            the angle in radians
         */
        public Vector2 rotateRad(float radians)
        {
            float cos = (float)Math.Cos(radians);
            float sin = (float)Math.Sin(radians);

            float newX = this.x * cos - this.y * sin;
            float newY = this.x * sin + this.y * cos;

            this.x = newX;
            this.y = newY;

            return this;
        }

        /**
         * Rotates the Vector2 by 90 degrees in the specified direction, where >= 0 is
         * counter-clockwise and < 0 is clockwise.
         */
        public Vector2 rotate90(int dir)
        {
            float x = this.x;
            if (dir >= 0)
            {
                this.x = -y;
                y = x;
            }
            else
            {
                this.x = y;
                y = -x;
            }
            return this;
        }

        // 
        public Vector2 lerp(Vector2 target, float alpha)
        {
            float invAlpha = 1.0f - alpha;
            this.x = (x * invAlpha) + (target.x * alpha);
            this.y = (y * invAlpha) + (target.y * alpha);
            return this;
        }

        // 
        public Vector2 interpolate(Vector2 target, float alpha, Interpolation interpolation)
        {
            return lerp(target, interpolation.apply(alpha));
        }

        // 
        public Vector2 setToRandomDirection()
        {
            float theta = MathUtil.Random(0f, MathUtil.PI2);
            return this.set(MathUtil.cos(theta), MathUtil.sin(theta));
        }


        public int hashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + MathUtil.floatToIntBits(x);
            result = prime * result + MathUtil.floatToIntBits(y);
            return result;
        }


        public bool equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            Vector2 other = (Vector2)obj;
            if (MathUtil.floatToIntBits(x) != MathUtil.floatToIntBits(other.x))
                return false;
            if (MathUtil.floatToIntBits(y) != MathUtil.floatToIntBits(other.y))
                return false;
            return true;
        }


        public bool epsilonEquals(Vector2 other, float epsilon)
        {
            if (other == null)
                return false;
            if (Math.Abs(other.x - x) > epsilon)
                return false;
            if (Math.Abs(other.y - y) > epsilon)
                return false;
            return true;
        }

        /**
         * Compares this vector with the other vector, using the supplied epsilon for
         * fuzzy equality testing.
         * 
         * @return whether the vectors are the same.
         */
        public bool epsilonEquals(float x, float y, float epsilon)
        {
            if (Math.Abs(x - this.x) > epsilon)
                return false;
            if (Math.Abs(y - this.y) > epsilon)
                return false;
            return true;
        }

        // 
        public bool isUnit()
        {
            return isUnit(0.000000001f);
        }

        // 
        public bool isUnit(float margin)
        {
            return Math.Abs(len2() - 1f) < margin;
        }


        public bool isZero()
        {
            return x == 0 && y == 0;
        }


        public bool isZero(float margin)
        {
            return len2() < margin;
        }

        // 
        public bool isOnLine(Vector2 other)
        {
            return MathUtil.isZero(x * other.y - y * other.x);
        }

        // 
        public bool isOnLine(Vector2 other, float epsilon)
        {
            return MathUtil.isZero(x * other.y - y * other.x, epsilon);
        }

        // 
        public bool isCollinear(Vector2 other, float epsilon)
        {
            return isOnLine(other, epsilon) && dot(other) > 0f;
        }

        // 
        public bool isCollinear(Vector2 other)
        {
            return isOnLine(other) && dot(other) > 0f;
        }

        // 
        public bool isCollinearOpposite(Vector2 other, float epsilon)
        {
            return isOnLine(other, epsilon) && dot(other) < 0f;
        }

        // 
        public bool isCollinearOpposite(Vector2 other)
        {
            return isOnLine(other) && dot(other) < 0f;
        }

        // 
        public bool isPerpendicular(Vector2 vector)
        {
            return MathUtil.isZero(dot(vector));
        }

        // 
        public bool isPerpendicular(Vector2 vector, float epsilon)
        {
            return MathUtil.isZero(dot(vector), epsilon);
        }

        // 
        public bool hasSameDirection(Vector2 vector)
        {
            return dot(vector) > 0;
        }

        // 
        public bool hasOppositeDirection(Vector2 vector)
        {
            return dot(vector) < 0;
        }

        // 
        public Vector2 setZero()
        {
            this.x = 0;
            this.y = 0;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Vector2)) return false;
            Vector2 other = (Vector2)obj;
            return other.x == this.x && other.y == this.y;

        }

        public bool equal(Vector3 other, float precision)
        {
            return other.x == this.x && other.y == this.y && other.z == 0;
        }

        public bool Equals(Vector2 other)
        {
            return other.x == this.x && other.y == this.y;
        }
    }
}
