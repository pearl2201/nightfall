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



/**
 * This annotation specifies how many children the task can have. It is applied
 * to the task class.
 * 
 * @author davebaol
 */
[AttributeUsage(AttributeTargets.Class)]
public class BtreeTaskConstraint : System.Attribute
{

    /**
	 * Returns the minimum number of allowed children, defaults to 0.
	 * 
	 * @return the minimum number of allowed children.
	 */
    public int minChildren = 0;

    /**
	 * Returns the maximum number of allowed children, defaults to
	 * {@code Integer.MAX_VALUE}.
	 * 
	 * @return the maximum number of allowed children.
	 */
    public int maxChildren = int.MaxValue;
}
