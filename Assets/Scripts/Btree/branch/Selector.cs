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
 * 选择节点<br>
 * A {@code Selector} is a branch task that runs every children until one of
 * them succeeds. If a child task fails, the selector will start and run the
 * next child task.
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author implicit-invocation
 */
public class Selector<E> : SingleRunningChildBranch<E>
{

    /** Creates a {@code Selector} branch with no children. */
    public Selector() : base()
    {

    }

    /**
	 * Creates a {@code Selector} branch with the given children.
	 * 
	 * @param tasks
	 *            the children of this task
	 */
    public Selector(BTreeTask<E>[] tasks) : base(tasks.ToList())
    {
    }

    /**
	 * Creates a {@code Selector} branch with the given children.
	 * 
	 * @param tasks
	 *            the children of this task
	 */
    public Selector(List<BTreeTask<E>> tasks) : base(tasks)
    {

    }


    public override void childFail(BTreeTask<E> runningTask)
    {
        base.childFail(runningTask);
        if (++currentChildIndex < children.Count())
        {
            run(); // Run next child
        }
        else
        {
            fail(); // All children processed, return failure status
        }
    }


    public override void childSuccess(BTreeTask<E> runningTask)
    {
        base.childSuccess(runningTask);
        success(); // Return success status when a child says it succeeded
    }

}
