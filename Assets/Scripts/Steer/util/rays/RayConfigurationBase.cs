/*******************************************************************************
 * Copyright 2014 See AUTHORS file.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ******************************************************************************/

using Pika.Ai.Utils;
using Pika.Base.Mathj.Geometry;



/** {@code RayConfigurationBase} is the base class for concrete ray configurations having a fixed number of rays.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol */
public abstract class RayConfigurationBase<T> : RayConfiguration<T> where T : Vector<T>
{

    protected Steerable<T> owner;
    protected Ray<T>[] rays;

    /** Creates a {@code RayConfigurationBase} for the given owner and the specified number of rays.
	 * @param owner the owner of this configuration
	 * @param numRays the number of rays used by this configuration */


    public RayConfigurationBase(Steerable<T> owner, int numRays)
    {
        this.owner = owner;
        this.rays = new Ray<T>[numRays];
        for (int i = 0; i < numRays; i++)
            this.rays[i] = new Ray<T>(owner.getPosition().cpy().setZero(), owner.getPosition().cpy().setZero());
    }

    /** Returns the owner of this configuration. */
    public Steerable<T> getOwner()
    {
        return owner;
    }

    /** Sets the owner of this configuration. */
    public void setOwner(Steerable<T> owner)
    {
        this.owner = owner;
    }

    /** Returns the rays of this configuration. */
    public Ray<T>[] getRays()
    {
        return rays;
    }

    /** Sets the rays of this configuration. */
    public void setRays(Ray<T>[] rays)
    {
        this.rays = rays;
    }

    public abstract Ray<T>[] updateRays();
}
