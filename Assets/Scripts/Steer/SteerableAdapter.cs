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


/** An adapter class for {@link Steerable}. You can derive from this and only override what you are interested in. For example,
 * this comes in handy when you have to create on the fly a target for a particular behavior.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol */
public class SteerableAdapter<T> : Steerable<T> where T : Vector<T>
{


    public virtual float getZeroLinearSpeedThreshold()
    {
        return 0.001f;
    }


    public virtual void setZeroLinearSpeedThreshold(float value)
    {
    }


    public virtual float getMaxLinearSpeed()
    {
        return 0;
    }


    public virtual void setMaxLinearSpeed(float maxLinearSpeed)
    {
    }


    public virtual float getMaxLinearAcceleration()
    {
        return 0;
    }


    public virtual void setMaxLinearAcceleration(float maxLinearAcceleration)
    {
    }


    public virtual float getMaxAngularSpeed()
    {
        return 0;
    }


    public virtual void setMaxAngularSpeed(float maxAngularSpeed)
    {
    }


    public virtual float getMaxAngularAcceleration()
    {
        return 0;
    }


    public virtual void setMaxAngularAcceleration(float maxAngularAcceleration)
    {
    }

    public virtual T getPosition()
    {
        return default(T);
    }


    public virtual float getOrientation()
    {
        return 0;
    }


    public virtual void setOrientation(float orientation)
    {
    }


    public virtual T getLinearVelocity()
    {
        return default(T);
    }


    public virtual float getAngularVelocity()
    {
        return 0;
    }


    public virtual float getBoundingRadius()
    {
        return 0;
    }


    public virtual bool isTagged()
    {
        return false;
    }


    public virtual void setTagged(bool tagged)
    {
    }


    public virtual Location<T> newLocation()
    {
        return null;
    }


    public virtual float vectorToAngle(T vector)
    {
        return 0;
    }


    public virtual T angleToVector(T outVector, float angle)
    {
        return default(T);
    }

}
