using Pika.Base.Mathj.Geometry;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Pika.Ai.Nav
{

    [Serializable]
    [ComVisible(true)]
    public class Rectangle
    {
        private float x, y, width, height;

        /// <summary>
        ///	Empty Shared Field
        /// </summary>
        ///
        /// <remarks>
        ///	An uninitialized Rectangle Structure.
        /// </remarks>

        public static readonly Rectangle Empty;


        /// <summary>
        ///	Ceiling Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Rectangle structure from a RectangleF 
        ///	structure by taking the ceiling of the X, Y, Width,
        ///	and Height properties.
        /// </remarks>

        public static Rectangle Ceiling(Rectangle value)
        {
            float x, y, w, h;
            checked
            {
                x = (float)Math.Ceiling(value.X);
                y = (float)Math.Ceiling(value.Y);
                w = (float)Math.Ceiling(value.Width);
                h = (float)Math.Ceiling(value.Height);
            }

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        ///	FromLTRB Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Rectangle structure from left, top, right,
        ///	and bottom coordinates.
        /// </remarks>

        public static Rectangle FromLTRB(float left, float top,
                          float right, float bottom)
        {
            return new Rectangle(left, top, right - left,
                          bottom - top);
        }

        /// <summary>
        ///	Inflate Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new Rectangle by inflating an existing 
        ///	Rectangle by the specified coordinate values.
        /// </remarks>

        public static Rectangle Inflate(Rectangle rect, int x, int y)
        {
            Rectangle r = new Rectangle(rect.Location, rect.Size);
            r.Inflate(x, y);
            return r;
        }

        /// <summary>
        ///	Inflate Method
        /// </summary>
        ///
        /// <remarks>
        ///	Inflates the Rectangle by a specified width and height.
        /// </remarks>

        public void Inflate(float width, float height)
        {
            Inflate(new Vector2(width, height));
        }

        /// <summary>
        ///	Inflate Method
        /// </summary>
        ///
        /// <remarks>
        ///	Inflates the Rectangle by a specified Size.
        /// </remarks>

        public void Inflate(Vector2 size)
        {
            x -= size.x;
            y -= size.y;
            Width += size.x * 2;
            Height += size.y * 2;
        }

        /// <summary>
        ///	Intersect Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new Rectangle by intersecting 2 existing 
        ///	Rectangles. Returns null if there is no	intersection.
        /// </remarks>

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
                return Empty;

            return FromLTRB(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Top, b.Top),
                Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom));
        }

        /// <summary>
        ///	Intersect Method
        /// </summary>
        ///
        /// <remarks>
        ///	Replaces the Rectangle with the intersection of itself
        ///	and another Rectangle.
        /// </remarks>

        public void Intersect(Rectangle rect)
        {
            var temp = Intersect(this, rect);
            this.x = temp.x;
            this.y = temp.y;
            this.width = temp.width;
            this.height = temp.height;
        }

        /// <summary>
        ///	Round Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Rectangle structure from a RectangleF by
        ///	rounding the X, Y, Width, and Height properties.
        /// </remarks>

        public static Rectangle Round(Rectangle value)
        {
            float x, y, w, h;
            checked
            {
                x = (float)Math.Round(value.X);
                y = (float)Math.Round(value.Y);
                w = (float)Math.Round(value.Width);
                h = (float)Math.Round(value.Height);
            }

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        ///	Truncate Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a Rectangle structure from a RectangleF by
        ///	truncating the X, Y, Width, and Height properties.
        /// </remarks>

        // LAMESPEC: Should this be floor, or a pure cast to int?

        public static Rectangle Truncate(Rectangle value)
        {
            float x, y, w, h;
            checked
            {
                x = (float)value.X;
                y = (float)value.Y;
                w = (float)value.Width;
                h = (float)value.Height;
            }

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        ///	Union Shared Method
        /// </summary>
        ///
        /// <remarks>
        ///	Produces a new Rectangle from the union of 2 existing 
        ///	Rectangles.
        /// </remarks>

        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            return FromLTRB(Math.Min(a.Left, b.Left),
                     Math.Min(a.Top, b.Top),
                     Math.Max(a.Right, b.Right),
                     Math.Max(a.Bottom, b.Bottom));
        }

        /// <summary>
        ///	Equality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Rectangle objects. The return value is
        ///	based on the equivalence of the Location and Size 
        ///	properties of the two Rectangles.
        /// </remarks>

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return ((left.Location == right.Location) &&
                (left.Size == right.Size));
        }

        /// <summary>
        ///	Inequality Operator
        /// </summary>
        ///
        /// <remarks>
        ///	Compares two Rectangle objects. The return value is
        ///	based on the equivalence of the Location and Size 
        ///	properties of the two Rectangles.
        /// </remarks>

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return ((left.Location != right.Location) ||
                (left.Size != right.Size));
        }

        public Vector2 Size => new Vector2(width, height);


        // -----------------------
        // Public Constructors
        // -----------------------

        /// <summary>
        ///	Rectangle Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a Rectangle from Point and Size values.
        /// </remarks>

        public Rectangle(Point location, Vector2 size)
        {
            x = location.X;
            y = location.Y;
            width = size.x;
            height = size.y;
        }

        /// <summary>
        ///	Rectangle Constructor
        /// </summary>
        ///
        /// <remarks>
        ///	Creates a Rectangle from a specified x,y location and
        ///	width and height values.
        /// </remarks>

        public Rectangle(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        ///	Bottom Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the bottom edge of the Rectangle.
        ///	Read only.
        /// </remarks>

        [Browsable(false)]
        public float Top
        {
            get
            {
                return y;
            }
        }

        /// <summary>
        ///	Bottom Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the bottom edge of the Rectangle.
        ///	Read only.
        /// </remarks>

        [Browsable(false)]
        public float Bottom
        {
            get
            {
                return y + height;
            }
        }

        /// <summary>
        ///	Height Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Height of the Rectangle.
        /// </remarks>

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        ///	IsEmpty Property
        /// </summary>
        ///
        /// <remarks>
        ///	Indicates if the width or height are zero. Read only.
        /// </remarks>		
        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return ((x == 0) && (y == 0) && (width == 0) && (height == 0));
            }
        }

        /// <summary>
        ///	Left Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the left edge of the Rectangle.
        ///	Read only.
        /// </remarks>

        [Browsable(false)]
        public float Left
        {
            get
            {
                return X;
            }
        }

        /// <summary>
        ///	Location Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Location of the top-left corner of the Rectangle.
        /// </remarks>

        [Browsable(false)]
        public Point Location
        {
            get
            {
                return new Point(x, y);
            }
            set
            {
                x = value.X;
                y = value.Y;
            }
        }

        /// <summary>
        ///	Right Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the right edge of the Rectangle.
        ///	Read only.
        /// </remarks>

        [Browsable(false)]
        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        /// <summary>
        ///	Size Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Size of the Rectangle.
        /// </remarks>



        /// <summary>
        ///	Top Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Y coordinate of the top edge of the Rectangle.
        ///	Read only.
        /// </remarks>



        /// <summary>
        ///	Width Property
        /// </summary>
        ///
        /// <remarks>
        ///	The Width of the Rectangle.
        /// </remarks>

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        ///	X Property
        /// </summary>
        ///
        /// <remarks>
        ///	The X coordinate of the Rectangle.
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
        ///	The Y coordinate of the Rectangle.
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
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if an x,y coordinate lies within this Rectangle.
        /// </remarks>

        public bool Contains(float x, float y)
        {
            return ((x >= Left) && (x < Right) &&
                (y >= Top) && (y < Bottom));
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Point lies within this Rectangle.
        /// </remarks>

        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        ///	Contains Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Rectangle lies entirely within this 
        ///	Rectangle.
        /// </remarks>

        public bool Contains(Rectangle rect)
        {
            return (rect == Intersect(this, rect));
        }

        /// <summary>
        ///	Equals Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks equivalence of this Rectangle and another object.
        /// </remarks>

        public override bool Equals(object obj)
        {
            if (!(obj is Rectangle))
                return false;

            return (this == (Rectangle)obj);
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
            hash = hash * 31 + width.GetHashCode();
            hash = hash * 31 + height.GetHashCode();
            return hash;


        }

        /// <summary>
        ///	IntersectsWith Method
        /// </summary>
        ///
        /// <remarks>
        ///	Checks if a Rectangle intersects with this one.
        /// </remarks>

        public bool IntersectsWith(Rectangle rect)
        {
            return !((Left >= rect.Right) || (Right <= rect.Left) ||
                (Top >= rect.Bottom) || (Bottom <= rect.Top));
        }

        private bool IntersectsWithInclusive(Rectangle r)
        {
            return !((Left > r.Right) || (Right < r.Left) ||
                (Top > r.Bottom) || (Bottom < r.Top));
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the Rectangle a specified distance.
        /// </remarks>

        public void Offset(float x, float y)
        {
            this.x += x;
            this.y += y;
        }

        /// <summary>
        ///	Offset Method
        /// </summary>
        ///
        /// <remarks>
        ///	Moves the Rectangle a specified distance.
        /// </remarks>

        public void Offset(Point pos)
        {
            x += pos.X;
            y += pos.Y;
        }

        /// <summary>
        ///	ToString Method
        /// </summary>
        ///
        /// <remarks>
        ///	Formats the Rectangle as a string in (x,y,w,h) notation.
        /// </remarks>

        public override string ToString()
        {
            return String.Format("{{X={0},Y={1},Width={2},Height={3}}}",
                         x, y, width, height);
        }

    }
}
