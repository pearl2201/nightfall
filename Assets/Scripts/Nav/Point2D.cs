using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Pika.Ai.Nav
{
    public class Point2D : Point
    {
        public Point2D(float x, float y) : base(x, y)
        {
        }
    }

    [Serializable]
    [ComVisible(true)]
    public class Point
    {
        // Private x and y coordinate fields.
        private float x, y;

        // -----------------------
        // Public Shared Members
        // -----------------------

        /// <summary>
        ///	Empty Shared Field
        /// </summary>
        ///
        /// <remarks>
        ///	An uninitialized Point Structure.
        /// </remarks>

        public static readonly Point Empty;

        /// <summary>
        ///	Ceiling Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Point structure from a PointF structure by
        ///	taking the ceiling of the X and Y properties.
        /// </remarks>

        public static Point Ceiling(Point value)
        {
            float x, y;
            checked
            {
                x = (float)Math.Ceiling(value.X);
                y = (float)Math.Ceiling(value.Y);
            }

            return new Point(x, y);
        }

        /// <summary>
        ///	Round Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Point structure from a PointF structure by
        ///	rounding the X and Y properties.
        /// </remarks>

        public static Point Round(Point value)
        {
            float x, y;
            checked
            {
                x = (int)Math.Round(value.X);
                y = (int)Math.Round(value.Y);
            }

            return new Point(x, y);
        }

        /// <summary>
        ///	Truncate Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Point structure from a PointF structure by
        ///	truncating the X and Y properties.
        /// </remarks>

        // LAMESPEC: Should this be floor, or a pure cast to int?

        public static Point Truncate(Point value)
        {
            float x, y;
            checked
            {
                x = (int)value.X;
                y = (int)value.Y;
            }

            return new Point(x, y);
        }

        /// <summary>
        ///	Equality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Point objects. The return value is
        ///	based on the equivalence of the X and Y properties 
        ///	of the two points.
        /// </remarks>

        public static bool operator ==(Point left, Point right)
        {
            return ((left.X == right.X) && (left.Y == right.Y));
        }

        /// <summary>
        ///	Inequality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Point objects. The return value is
        ///	based on the equivalence of the X and Y properties 
        ///	of the two points.
        /// </remarks>

        public static bool operator !=(Point left, Point right)
        {
            return ((left.X != right.X) || (left.Y != right.Y));
        }


        // -----------------------
        // Public Constructors
        // -----------------------




        /// <summary>
        ///	Point Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a Point from a specified x,y coordinate pair.
        /// </remarks>

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // -----------------------
        // Public Instance Members
        // -----------------------

        /// <summary>
        ///	IsEmpty Property
        /// </summary>
        ///
        /// <remarks>
        ///	Indicates if both X and Y are zero.
        /// </remarks>

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return ((x == 0) && (y == 0));
            }
        }

        /// <summary>
        ///	X Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the Point.
        /// </remarks>

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        /// <summary>
        ///	Y Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the Point.
        /// </remarks>

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        /// <summary>
        ///	Equals Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks equivalence of this Point and another object.
        /// </remarks>

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
                return false;

            return (this == (Point)obj);
        }

        /// <summary>
        ///	GetHashCode Method
        /// </summary>
        ///
        /// <remarks>
        ///	Calculates a hashing value.
        /// </remarks>

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + x.GetHashCode();
            hash = hash * 31 + y.GetHashCode();
            return hash;
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the Point a specified distance.
        /// </remarks>

        public void Offset(int dx, float dy)
        {
            x += dx;
            y += dy;
        }

        /// <summary>
        ///	ToString Method
        /// </summary>
        ///
        /// <remarks>
        ///	Formats the Point as a string in coordinate notation.
        /// </remarks>

        public override string ToString()
        {
            return string.Format("{{X={0},Y={1}}}", x.ToString(CultureInfo.InvariantCulture),
                y.ToString(CultureInfo.InvariantCulture));
        }
#if NET_2_0
        public static Point Add (Point pt, Size sz)
        {
            return new Point (pt.X + sz.Width, pt.Y + sz.Height);
        }

        public void Offset (Point p)
        {
            Offset (p.X, p.Y);
        }

        public static Point Subtract (Point pt, Size sz)
        {
            return new Point (pt.X - sz.Width, pt.Y - sz.Height);
        }
#endif
    }
}
