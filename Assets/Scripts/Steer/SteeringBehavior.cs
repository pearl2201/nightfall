using System.Runtime.CompilerServices;
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

public interface ISteeringBehaviourBuilder<U, T> where T : Vector<T>
{

    /** Sets the owner of this steering behavior.
     * @return this behavior for chaining. */
    U setOwner(Steerable<T> owner);



    /** Sets the limiter of this steering behavior.
     * @return this behavior for chaining. */
    U setLimiter(Limiter limiter);


    U setEnabled(bool enabled);
}


public abstract class SteeringBehavior<T> where T : Vector<T>
{
    /** The owner of this steering behavior */
    protected Steerable<T> owner;

    /** The limiter of this steering behavior */
    protected Limiter limiter;

    /** A flag indicating whether this steering behavior is enabled or not. */
    public bool enabled;

    /** Creates a {@code SteeringBehavior} for the specified owner. The behavior is enabled and has no explicit limiter, meaning
     * that the owner is used instead.
     *
     * @param owner the owner of this steering behavior */
    public SteeringBehavior(Steerable<T> owner) : this(owner, null, true)
    {

    }

    /** Creates a {@code SteeringBehavior} for the specified owner and limiter. The behavior is enabled.
     *
     * @param owner the owner of this steering behavior
     * @param limiter the limiter of this steering behavior */
    public SteeringBehavior(Steerable<T> owner, Limiter limiter) : this(owner, limiter, true)
    {

    }

    /** Creates a {@code SteeringBehavior} for the specified owner and activation flag. The behavior has no explicit limiter,
     * meaning that the owner is used instead.
     *
     * @param owner the owner of this steering behavior
     * @param enabled a flag indicating whether this steering behavior is enabled or not */
    public SteeringBehavior(Steerable<T> owner, bool enabled) : this(owner, null, enabled)
    {

    }

    /** Creates a {@code SteeringBehavior} for the specified owner, limiter and activation flag.
     *
     * @param owner the owner of this steering behavior
     * @param limiter the limiter of this steering behavior
     * @param enabled a flag indicating whether this steering behavior is enabled or not */
    public SteeringBehavior(Steerable<T> owner, Limiter limiter, bool enabled)
    {
        this.owner = owner;
        this.limiter = limiter;
        this.enabled = enabled;
    }

    /**
     * 计算转向<br>
     * If this behavior is enabled calculates the steering acceleration and writes it to the given steering output. If it is
     * disabled the steering output is set to zero.
     * @param steering the steering acceleration to be calculated.
     * @return the calculated steering acceleration for chaining. */
    public SteeringAcceleration<T> calculateSteering(SteeringAcceleration<T> steering)
    {
        return isEnabled() ? calculateRealSteering(steering) : steering.setZero();
    }

    /** Calculates the steering acceleration produced by this behavior and writes it to the given steering output.
     * <p>
     * This method is called by {@link #calculateSteering(SteeringAcceleration)} when this steering behavior is enabled.
     * @param steering the steering acceleration to be calculated.
     * @return the calculated steering acceleration for chaining. */
    protected abstract SteeringAcceleration<T> calculateRealSteering(SteeringAcceleration<T> steering);

    /** Returns the owner of this steering behavior. */
    public virtual Steerable<T> getOwner()
    {
        return owner;
    }

    /** Returns the limiter of this steering behavior. */
    public Limiter getLimiter()
    {
        return limiter;
    }

    /** Returns true if this steering behavior is enabled; false otherwise. */
    public bool isEnabled()
    {
        return enabled;
    }

    /** Returns the actual limiter of this steering behavior. */
    protected Limiter getActualLimiter()
    {
        return limiter == null ? owner : limiter;
    }

    /** Utility method that creates a new vector.
     * <p>
     * This method is used internally to instantiate vectors of the correct type parameter {@code T}. This technique keeps the API
     * simple and makes the API easier to use with the GWT backend because avoids the use of reflection.
     *
     * @param location the location whose position is used to create the new vector
     * @return the newly created vector */
    protected T newVector(Location<T> location)
    {
        return location.getPosition().cpy().setZero();
    }

    /** Sets the owner of this steering behavior.
 * @return this behavior for chaining. */
    public virtual SteeringBehavior<T> setOwner(Steerable<T> owner)
    {
        this.owner = owner;
        return this;
    }



    /** Sets the limiter of this steering behavior.
     * @return this behavior for chaining. */
    public virtual SteeringBehavior<T> setLimiter(Limiter limiter)
    {
        this.limiter = limiter;
        return this;
    }

    public virtual SteeringBehavior<T> setEnabled(bool enabled)
    {
        this.enabled = enabled;
        return this;
    }
}



// public static class SteeringBehaviorExtensions
// {
//     /** Sets this steering behavior on/off.
//      * @return this behavior for chaining. */
//     public static U setEnabled<U, T>(this U steerBehaviour, bool enabled) where U : SteeringBehavior<T> where T : Vector<T>
//     {
//         steerBehaviour.enabled = enabled;
//         return steerBehaviour;
//     }
// }