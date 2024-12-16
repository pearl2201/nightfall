using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Ai.Quadtree
{
    public class QuadTreeException : Exception
    {
        public QuadTreeException(string msg) : base(msg) { }
    }
}
