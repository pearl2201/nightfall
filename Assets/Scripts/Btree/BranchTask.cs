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

using System.Collections.Generic;
using System.Linq;



/**
 * 分支任务节点<br>
 * A branch task defines a behavior tree branch, contains logic of starting or
 * running sub-branches and leaves
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author implicit-invocation
 * @author davebaol
 */
[BtreeTaskConstraint(minChildren = 1)]
public abstract class BranchTask<E> : BTreeTask<E>
{

    /** The children of this branch task. */
    public List<BTreeTask<E>> children;

    /** Create a branch task with no children */
    public BranchTask(): this(new List<BTreeTask<E>>())
    {
        
    }

    /**
	 * Create a branch task with a list of children
	 * 
	 * @param tasks
	 *            list of this task's children, can be empty
	 */
    public BranchTask(List<BTreeTask<E>> tasks)
    {
        this.children = tasks;
    }


    protected override int addChildToTask(BTreeTask<E> child)
    {
        children.Add(child);
        return children.Count() - 1;
    }


    public override int getChildCount()
    {
        return children.Count();
    }


    public override BTreeTask<E> getChild(int i)
    {
        return children.ElementAt(i);
    }


    public override void release()
    {
        children.Clear();
        base.release();
    }

}
