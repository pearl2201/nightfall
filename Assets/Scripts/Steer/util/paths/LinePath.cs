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
using System.Collections.Generic;
using System.Linq;


/** A {@code LinePath} is a path for path following behaviors that is made up of a series of waypoints. Each waypoint is connected
 * to the successor with a {@link Segment}.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol
 * @author Daniel Holderbaum */
public class LinePath<T> : Path<T, LinePathParam> where T : Vector<T>
{

    private List<Segment<T>> segments;
    private bool _isOpen;
    private float pathLength;
    private T nearestPointOnCurrentSegment;
    private T nearestPointOnPath;
    private T tmpB;
    private T tmpC;

    /** Creates a closed {@code LinePath} for the specified {@code waypoints}.
	 * @param waypoints the points making up the path
	 * @throws IllegalArgumentException if {@code waypoints} is {@code null} or has less than two (2) waypoints. */
    public LinePath(List<T> waypoints) : this(waypoints, false)
    {

    }

    /** Creates a {@code LinePath} for the specified {@code waypoints}.
	 * @param waypoints the points making up the path
	 * @param isOpen a flag indicating whether the path is open or not
	 * @throws IllegalArgumentException if {@code waypoints} is {@code null} or has less than two (2) waypoints. */
    public LinePath(List<T> waypoints, bool isOpen)
    {
        this._isOpen = isOpen;
        createPath(waypoints);
        nearestPointOnCurrentSegment = waypoints.ElementAt(0).cpy();
        nearestPointOnPath = waypoints.ElementAt(0).cpy();
        tmpB = waypoints.ElementAt(0).cpy();
        tmpC = waypoints.ElementAt(0).cpy();
    }


    public bool isOpen()
    {
        return _isOpen;
    }


    public float getLength()
    {
        return pathLength;
    }


    public T getStartPoint()
    {
        return segments.ElementAt(0).begin;
    }


    public T getEndPoint()
    {
        return segments.ElementAt(0).end;
    }

    /** Returns the square distance of the nearest point on line segment {@code a-b}, from point {@code c}. Also, the {@code out}
	 * vector is assigned to the nearest point.
	 * @param out the output vector that contains the nearest point on return
	 * @param a the start point of the line segment
	 * @param b the end point of the line segment
	 * @param c the point to calculate the distance from */
    public float calculatePointSegmentSquareDistance(T outV, T a, T b, T c)
    {
        outV.set(a);
        tmpB.set(b);
        tmpC.set(c);

        T ab = tmpB.sub(a);
        float abLen2 = ab.len2();
        if (abLen2 != 0)
        {
            float t = (tmpC.sub(a)).dot(ab) / abLen2;
            outV.mulAdd(ab, MathUtil.clamp(t, 0, 1));
        }

        return outV.dst2(c);
    }


    public LinePathParam createParam()
    {
        return new LinePathParam();
    }

    // We pass the last parameter value to the path in order to calculate the current
    // parameter value. This is essential to avoid nasty problems when lines are close together.
    // We should limit the algorithm to only considering areas of the path close to the previous
    // parameter value. The character is unlikely to have moved far, after all.
    // This technique, assuming the new value is close to the old one, is called coherence, and it is a
    // feature of many geometric algorithms.
    // TODO: Currently coherence is not implemented.

    public float calculateDistance(T agentCurrPos, LinePathParam parameter)
    {
        // Find the nearest segment
        float smallestDistance2 = float.PositiveInfinity;
        Segment<T> nearestSegment = null;
        for (int i = 0; i < segments.Count(); i++)
        {
            Segment<T> segment = segments.ElementAt(i);
            float distance2 = calculatePointSegmentSquareDistance(nearestPointOnCurrentSegment, segment.begin, segment.end,
                agentCurrPos);

            // first point
            if (distance2 < smallestDistance2)
            {
                nearestPointOnPath.set(nearestPointOnCurrentSegment);
                smallestDistance2 = distance2;
                nearestSegment = segment;
                parameter.segmentIndex = i;
            }
        }

        // Distance from path start
        float lengthOnPath = nearestSegment.cumulativeLength - nearestPointOnPath.dst(nearestSegment.end);

        parameter.setDistance(lengthOnPath);

        return lengthOnPath;
    }


    public void calculateTargetPosition(T outV, LinePathParam param, float targetDistance)
    {
        if (_isOpen)
        {
            // Open path support
            if (targetDistance < 0)
            {
                // Clamp target distance to the min
                targetDistance = 0;
            }
            else if (targetDistance > pathLength)
            {
                // Clamp target distance to the max
                targetDistance = pathLength;
            }
        }
        else
        {
            // Closed path support
            if (targetDistance < 0)
            {
                // Backwards
                targetDistance = pathLength + (targetDistance % pathLength);
            }
            else if (targetDistance > pathLength)
            {
                // Forward
                targetDistance = targetDistance % pathLength;
            }
        }

        // Walk through lines to see on which line we are
        Segment<T> desiredSegment = null;
        for (int i = 0; i < segments.Count(); i++)
        {
            Segment<T> segment = segments.ElementAt(i);
            if (segment.cumulativeLength >= targetDistance)
            {
                desiredSegment = segment;
                break;
            }
        }

        // begin-------targetPos-------end
        float distance = desiredSegment.cumulativeLength - targetDistance;

        outV.set(desiredSegment.begin).sub(desiredSegment.end).scl(distance / desiredSegment.length).add(desiredSegment.end);
    }

    /** Sets up this {@link Path} using the given way points.
	 * @param waypoints The way points of this path.
	 * @throws IllegalArgumentException if {@code waypoints} is {@code null} or empty. */
    public void createPath(List<T> waypoints)
    {
        if (waypoints == null || waypoints.Count() < 2)
            throw new NotSupportedException("waypoints cannot be null and must contain at least two (2) waypoints");

        segments = new List<Segment<T>>(waypoints.Count());
        pathLength = 0;
        T curr = waypoints.ElementAt(0);
        T prev = default(T);
        for (int i = 1; i <= waypoints.Count(); i++)
        {
            prev = curr;
            if (i < waypoints.Count())
                curr = waypoints.ElementAt(i);
            else if (_isOpen)
                break; // keep the path open
            else
                curr = waypoints.ElementAt(0); // close the path
            Segment<T> segment = new Segment<T>(prev, curr);
            pathLength += segment.length;
            segment.cumulativeLength = pathLength;
            segments.Add(segment);
        }
    }

    public List<Segment<T>> getSegments()
    {
        return segments;
    }


}

/** A {@code LinePathParam} contains the status of a {@link LinePath}.
    * 
    * @author davebaol */
public class LinePathParam : PathParam
{

    public int segmentIndex;
    public float distance;


    public float getDistance()
    {
        return distance;
    }


    public void setDistance(float distance)
    {
        this.distance = distance;
    }

    /** Returns the index of the current segment along the path */
    public int getSegmentIndex()
    {
        return segmentIndex;
    }

}

/** A {@code Segment} connects two consecutive waypoints of a {@link LinePath}.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol */
public class Segment<T> where T : Vector<T>
{
    public T begin;
    public T end;
    public float length;
    public float cumulativeLength;

    /** Creates a {@code Segment} for the 2 given points.
     * @param begin
     * @param end */
    public Segment(T begin, T end)
    {
        this.begin = begin;
        this.end = end;
        this.length = begin.dst(end);
    }

    /** Returns the start point of this segment. */
    public T getBegin()
    {
        return begin;
    }

    /** Returns the end point of this segment. */
    public T getEnd()
    {
        return end;
    }

    /** Returns the length of this segment. */
    public float getLength()
    {
        return length;
    }

    /** Returns the cumulative length from the first waypoint of the {@link LinePath} this segment belongs to. */
    public float getCumulativeLength()
    {
        return cumulativeLength;
    }
}