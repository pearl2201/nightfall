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

using System;

public class NeutralLimiter : NullLimiter
{
	public override float getMaxLinearSpeed()
	{
		return float.PositiveInfinity;
	}


	public override float getMaxLinearAcceleration()
	{
		return float.PositiveInfinity;
	}


	public override float getMaxAngularSpeed()
	{
		return float.PositiveInfinity;
	}


	public override float getMaxAngularAcceleration()
	{
		return float.PositiveInfinity;
	}
}

/** A {@code NullLimiter} always throws {@link NotSupportedException}. Typically it's used as the base class of partial or
 * immutable limiters.
 * 
 * @author davebaol */
public class NullLimiter : Limiter
{

	/** An immutable limiter whose getters return {@link Float#POSITIVE_INFINITY} and setters throw
	 * {@link NotSupportedException}. */
	public static NullLimiter NEUTRAL_LIMITER = new NeutralLimiter();

	/** Creates a {@code NullLimiter}. */
	public NullLimiter()
	{
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual float getMaxLinearSpeed()
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual void setMaxLinearSpeed(float maxLinearSpeed)
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual float getMaxLinearAcceleration()
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual void setMaxLinearAcceleration(float maxLinearAcceleration)
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual float getMaxAngularSpeed()
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual void setMaxAngularSpeed(float maxAngularSpeed)
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual float getMaxAngularAcceleration()
	{
		throw new NotSupportedException();
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual void setMaxAngularAcceleration(float maxAngularAcceleration)
	{
		throw new NotSupportedException();
	}


	public virtual float getZeroLinearSpeedThreshold()
	{
		return 0.001f;
	}

	/** Guaranteed to throw NotSupportedException.
	 * @throws NotSupportedException always */

	public virtual void setZeroLinearSpeedThreshold(float zeroLinearSpeedThreshold)
	{
		throw new NotSupportedException();
	}
}
