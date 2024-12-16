using Pika.Base.Mathj.Geometry;



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




/**逃离到某个坐标点<br>
 * {@code Flee} behavior does the opposite of {@link Seek}. It produces a linear steering force that moves the agent away from a
 * target position.
 *
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 *
 * @author davebaol */
public class Flee<T> : Seek<T> where T : Vector<T>
{

    /** Creates a {@code Flee} behavior for the specified owner.
     * @param owner the owner of this behavior. */
    public Flee(Steerable<T> owner) : this(owner, null)
    {
       
    }

    /** Creates a {@code Flee} behavior for the specified owner and target.
     * @param owner the owner of this behavior
     * @param target the target agent of this behavior. */
    public Flee(Steerable<T> owner, Location<T> target): base(owner, target)
    {
        
    }

    protected override SteeringAcceleration<T> calculateRealSteering(SteeringAcceleration<T> steering)
    {
        // We just do the opposite of seek, i.e. (owner.getPosition() - target.getPosition())
        // instead of (target.getPosition() - owner.getPosition())
        steering.linear.set(owner.getPosition()).sub(target.getPosition()).nor().scl(getActualLimiter().getMaxLinearAcceleration());

        // No angular acceleration
        steering.angular = 0;

        // Output steering acceleration
        return steering;
    }


}

