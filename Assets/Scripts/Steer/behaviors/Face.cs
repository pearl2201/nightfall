

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

/**
 * <br>朝向<br>
 * {@code Face} behavior makes the owner look at its target. It delegates to the {@link ReachOrientation} behavior to perform the
 * rotation but calculates the target orientation first based on target and owner position.
 *
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 *
 * @author davebaol */
public class Face<T> : ReachOrientation<T> where T : Vector<T>
{

    /** Creates a {@code Face} behavior for the specified owner.
     * @param owner the owner of this behavior. */
    public Face(Steerable<T> owner) : this(owner, null)
    {

    }

    /** Creates a {@code Face} behavior for the specified owner and target.
     * @param owner the owner of this behavior
     * @param target the target of this behavior. */
    public Face(Steerable<T> owner, Location<T> target) : base(owner, target)
    {

    }

    protected override SteeringAcceleration<T> calculateRealSteering(SteeringAcceleration<T> steering)
    {
        return face(steering, target.getPosition());
    }

    protected SteeringAcceleration<T> face(SteeringAcceleration<T> steering, T targetPosition)
    {
        // Get the direction to target
        T toTarget = steering.linear.set(targetPosition).sub(owner.getPosition());

        // Check for a zero direction, and return no steering if so
        if (toTarget.isZero(getActualLimiter().getZeroLinearSpeedThreshold())) return steering.setZero();

        // Calculate the orientation to face the target
        float orientation = owner.vectorToAngle(toTarget);

        // Delegate to ReachOrientation
        return reachOrientation(steering, orientation);
    }

    


}
