using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Ai.Nav
{
    public abstract class PathIterator
    {
        public const int SEG_CLOSE = 4;
        public const int SEG_CUBICTO = 3;
        public const int SEG_LINETO = 1;
        public const int SEG_MOVETO = 0;
        public const int SEG_QUADTO = 2;
        public const int WIND_EVEN_ODD = 0;
        public const int WIND_NON_ZERO = 1;
        public abstract int currentSegment(double[] coords);
        public abstract int currentSegment(float[] coords);

        public abstract void next();

        public abstract bool isDone();

        public abstract int getWindingRule();
    }
}
