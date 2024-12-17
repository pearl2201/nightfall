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




/**寻找某个目标点<br>
 * {@code Seek} behavior moves the owner towards the target position. Given a target, this behavior calculates the linear steering
 * acceleration which will direct the agent towards the target as fast as possible.
 *
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 *
 * @author davebaol */
public class Seek<T> : SteeringBehavior<T> where T : Vector<T>
{

    /** The target to seek */
    protected Location<T> target;

    /** Creates a {@code Seek} behavior for the specified owner.
     * @param owner the owner of this behavior. */
    public Seek(Steerable<T> owner) : this(owner, null)
    {

    }

    /** Creates a {@code Seek} behavior for the specified owner and target.
     * @param owner the owner of this behavior
     * @param target the target agent of this behavior. */
    public Seek(Steerable<T> owner, Location<T> target) : base(owner)
    {

        this.target = target;
    }

    protected override SteeringAcceleration<T> calculateRealSteering(SteeringAcceleration<T> steering)
    {
        // Try to match the position of the character with the position of the target by calculating
        // the direction to the target and by moving toward it as fast as possible.
        var delta = target.getPosition().sub(owner.getPosition()).nor();
        var max = getActualLimiter().getMaxLinearAcceleration();
        delta = delta.scl(max);

        steering.linear.set(delta);

        // No angular acceleration
        steering.angular = 0;

        // Output steering acceleration
        return steering;
    }

    /** Returns the target to seek. */
    public Location<T> getTarget()
    {
        return target;
    }

    /** Sets the target to seek.
     * @return this behavior for chaining. */
    public virtual Seek<T> setTarget(Location<T> target)
    {
        this.target = target;
        return this;
    }



}


