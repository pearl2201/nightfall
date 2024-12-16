using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Ai.Nav
{
    public class Rectangle2D : Rectangle
    {
        public Rectangle2D(Point location, Vector2 size) : base(location, size)
        {
        }

        public Rectangle2D(float x, float y, float width, float height) : base(x, y, width, height)
        {
        }
    }
}
