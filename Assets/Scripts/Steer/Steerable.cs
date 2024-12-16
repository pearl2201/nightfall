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



/**可转向对象接口<br>
 * A {@code Steerable} is a {@link Location} that gives access to the character's data required by steering system.
 * <p>
 * Notice that there is nothing to connect the direction that a Steerable is moving and the direction it is facing. For
 * instance, a character can be oriented along the x-axis but be traveling directly along the y-axis.
 *
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 *
 * @author davebaol */
public interface Steerable<T> : Location<T>, Limiter where T : Vector<T>
{

    /** Returns the vector indicating the linear velocity of this Steerable. */
    T getLinearVelocity();

    /** Returns the float value indicating the the angular velocity in radians of this Steerable. */
    float getAngularVelocity();

    /** Returns the bounding radius of this Steerable. */
    float getBoundingRadius();

    /** Returns {@code true} if this Steerable is tagged; {@code false} otherwise. */
    bool isTagged();

    /** Tag/untag this Steerable. This is a generic flag utilized in a variety of ways.
     * @param tagged the bool value to set */
    void setTagged(bool tagged);

}