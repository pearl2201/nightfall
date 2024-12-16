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

using Pika.Base.Mathj.Geometry;
using System;

/**
 * 分散开<br>
 * {@code Separation} is a group behavior producing a steering acceleration repelling from the other neighbors which are the agents
 * in the immediate area defined by the given {@link Proximity}. The acceleration is calculated by iterating through all the
 * neighbors, examining each one. The vector to each agent under consideration is normalized, multiplied by a strength decreasing
 * according to the inverse square law in relation to distance, and accumulated.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol */
public class Separation<T> : GroupBehavior<T>, ProximityCallback<T> where T : Vector<T>
{

    /** The constant coefficient of decay for the inverse square law force. It controls how fast the separation strength decays with
	 * distance. */
    float decayCoefficient = 1f;

    private T toAgent;
    private T linear;

    /** Creates a {@code Separation} behavior for the specified owner and proximity.
	 * @param owner the owner of this behavior
	 * @param proximity the proximity to detect the owner's neighbors */
    public Separation(Steerable<T> owner, Proximity<T> proximity) : base(owner, proximity)
    {
       

        this.toAgent = newVector(owner);
    }

    protected override SteeringAcceleration<T> calculateRealSteering(SteeringAcceleration<T> steering)
    {
        steering.setZero();

        linear = steering.linear;

        proximity.findNeighbors(this);

        return steering;
    }

    public bool reportNeighbor(Steerable<T> neighbor)
    {

        toAgent.set(owner.getPosition()).sub(neighbor.getPosition());
        float distanceSqr = toAgent.len2();

        if (distanceSqr == 0) return true;

        float maxAcceleration = getActualLimiter().getMaxLinearAcceleration();

        // Calculate the strength of repulsion through inverse square law decay
        float strength = getDecayCoefficient() / distanceSqr;
        if (strength > maxAcceleration) strength = maxAcceleration;

        // Add the acceleration
        // Optimized code for linear.mulAdd(toAgent.nor(), strength);
        linear.mulAdd(toAgent, strength / (float)Math.Sqrt(distanceSqr));

        return true;
    }

    /** Returns the coefficient of decay for the inverse square law force. */
    public float getDecayCoefficient()
    {
        return decayCoefficient;
    }

    /** Sets the coefficient of decay for the inverse square law force. It controls how fast the separation strength decays with
	 * distance.
	 * @param decayCoefficient the coefficient of decay to set */
    public Separation<T> setDecayCoefficient(float decayCoefficient)
    {
        this.decayCoefficient = decayCoefficient;
        return this;
    }


}
