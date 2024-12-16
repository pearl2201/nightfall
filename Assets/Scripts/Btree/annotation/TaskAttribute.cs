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
 * This field level annotation defines a task attribute.
 * 
 * @author davebaol
 */
[AttributeUsage(AttributeTargets.Field)]
public class BtreeTaskAttribute : System.Attribute
{

    /**
	 * Specifies the attribute's name; if empty the name of the field is used
	 * instead.
	 * 
	 * @return the attribute's name or an empty string if the name of the field must
	 *         be used.
	 */
    public string name = "";

    /**
	 * Specifies whether the attribute is required or not.
	 * 
	 * @return {@code true} if the attribute is required; {@code false} if it is
	 *         optional.
	 */
    public bool required = false;

}
