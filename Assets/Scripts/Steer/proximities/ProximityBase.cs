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
using System.Collections;
using System.Collections.Generic;


/** {@code ProximityBase} is the base class for any concrete proximity based on an iterable collection of agents.
 * 
 * @param <T> Type of vector, either 2D or 3D, implementing the {@link Vector} interface
 * 
 * @author davebaol */
public abstract class ProximityBase<T> : Proximity<T> where T : Vector<T>
{

    /** The owner of  this proximity. */
    protected Steerable<T> owner;

    /** The collection of the agents handled by this proximity.
	 * <p>
	 * Note that, being this field of type {@code Iterable}, you can either use java or libgdx collections. See
	 * https://github.com/libgdx/gdx-ai/issues/65 */
    protected IEnumerable<Steerable<T>> agents;

    /** Creates a {@code ProximityBase} for the specified owner and list of agents.
	 * @param owner the owner of this proximity
	 * @param agents the list of agents */
    public ProximityBase(Steerable<T> owner, IEnumerable<Steerable<T>> agents)
    {
        this.owner = owner;
        this.agents = agents;
    }


    public Steerable<T> getOwner()
    {
        return owner;
    }


    public void setOwner(Steerable<T> owner)
    {
        this.owner = owner;
    }

    /** Returns the the agents that represent potential neighbors. */
    public IEnumerable<Steerable<T>> getAgents()
    {
        return agents;
    }

    /** Sets the agents that represent potential neighbors. */
    public void setAgents(IEnumerable<Steerable<T>> agents)
    {
        this.agents = agents;
    }

    public abstract int findNeighbors(ProximityCallback<T> callback);
}
