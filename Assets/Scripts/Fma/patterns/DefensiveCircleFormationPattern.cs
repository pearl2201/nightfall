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
using Pika.Base.Utils;
using System;


/**
 * 防御圈阵型<br>
 * The defensive circle posts members around the circumference of a circle, so their backs are to the center of the circle. The
 * circle can consist of any number of members. Although a huge number of members might look silly, this implementation doesn't
 * put any fixed limit.
 *
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * @author davebaol
 */
public class DefensiveCircleFormationPattern<T> : FormationPattern<T> where T : Vector<T>
{
    /**
     * The number of slots currently in the pattern.
     */
    int numberOfSlots;

    /**
     * The radius of one member. This is needed to determine how close we can pack a given number of members around circle.
     */
    float memberRadius;

    /**
     * Creates a {@code DefensiveCircleFormationPattern}
     *
     * @param memberRadius
     */
    public DefensiveCircleFormationPattern(float memberRadius)
    {
        this.memberRadius = memberRadius;
    }


    public void setNumberOfSlots(int numberOfSlots)
    {
        this.numberOfSlots = numberOfSlots;
    }


    public virtual Location<T> calculateSlotLocation(Location<T> outLocation, int slotNumber)
    {
        if (numberOfSlots > 1)
        {
            // Place the slot around the circle based on its slot number
            float angleAroundCircle = (MathUtil.PI2 * slotNumber) / numberOfSlots;

            // The radius depends on the radius of the member,
            // and the number of members in the circle:
            // we want there to be no gap between member's shoulders.
            float radius = memberRadius / (float)Math.Sin(Math.PI / numberOfSlots);

            // Fill location components based on the angle around circle.
            outLocation.angleToVector(outLocation.getPosition(), angleAroundCircle).scl(radius);

            // The members should be facing out
            outLocation.setOrientation(angleAroundCircle);
        }
        else
        {
            outLocation.getPosition().setZero();
            outLocation.setOrientation(MathUtil.PI2 * slotNumber);
        }

        // Return the slot location
        return outLocation;
    }


    public bool supportsSlots(int slotCount)
    {
        // In this case we support any number of slots.
        return true;
    }

}
