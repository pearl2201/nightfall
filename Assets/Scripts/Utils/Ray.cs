using Pika.Base.Mathj.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pika.Ai.Utils
{

    /** A {@code Ray} is made up of a starting point and an ending point.
     *
     * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
     *
     * @author davebaol */
    public class Ray<T> where T : Vector<T>
    {

        /** The starting point of this ray. */
        public T start;

        /** The ending point of this ray. */
        public T end;

        /** Creates a {@code Ray} with the given {@code start} and {@code end} points.
         * @param start the starting point of this ray
         * @param end the starting point of this ray */
        public Ray(T start, T end)
        {
            this.start = start;
            this.end = end;
        }

        /** Sets this ray from the given ray.
         * @param ray The ray
         * @return this ray for chaining. */
        public Ray<T> set(Ray<T> ray)
        {
            start.set(ray.start);
            end.set(ray.end);
            return this;
        }

        /** Sets this Ray from the given start and end points.
         * @param start the starting point of this ray
         * @param end the starting point of this ray
         * @return this ray for chaining. */
        public Ray<T> set(T start, T end)
        {
            this.start.set(start);
            this.end.set(end);
            return this;
        }
    }

}
