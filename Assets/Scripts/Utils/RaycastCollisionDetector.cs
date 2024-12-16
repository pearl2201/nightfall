using Pika.Base.Mathj.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Ai.Utils
{

    /** A {@code RaycastCollisionDetector} finds the closest intersection between a ray and any object in the game world.
     * 
     * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
     * 
     * @author davebaol */
    public interface RaycastCollisionDetector<T> where T : Vector<T>
    {

        /** Casts the given ray to test if it collides with any objects in the game world.
         * @param ray the ray to cast.
         * @return {@code true} in case of collision; {@code false} otherwise. */
        bool collides(Ray<T> ray);

        /** Find the closest collision between the given input ray and the objects in the game world. In case of collision,
         * {@code outputCollision} will contain the collision point and the normal vector of the obstacle at the point of collision.
         * @param outputCollision the output collision.
         * @param inputRay the ray to cast.
         * @return {@code true} in case of collision; {@code false} otherwise. */
        bool findCollision(Collision<T> outputCollision, Ray<T> inputRay);
    }
}
