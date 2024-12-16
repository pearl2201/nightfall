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



/** An {@code AngularAccelerationLimiter} provides the maximum magnitude of angular acceleration. All other methods throw an
 * {@link NotSupportedException}.
 * 
 * @author davebaol */
public class AngularAccelerationLimiter : NullLimiter {

	private float maxAngularAcceleration;

	/** Creates an {@code AngularAccelerationLimiter}.
	 * @param maxAngularAcceleration the maximum angular acceleration */
	public AngularAccelerationLimiter (float maxAngularAcceleration) {
		this.maxAngularAcceleration = maxAngularAcceleration;
	}

	/** Returns the maximum angular acceleration. */
	
	public override float getMaxAngularAcceleration () {
		return maxAngularAcceleration;
	}

	/** Sets the maximum angular acceleration. */

	public override void setMaxAngularAcceleration (float maxAngularAcceleration) {
		this.maxAngularAcceleration = maxAngularAcceleration;
	}

}
