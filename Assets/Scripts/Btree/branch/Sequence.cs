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

/**顺序执行任务节点<br> 
 * A {@code Sequence} is a branch task that runs every children until one of them fails. If a child task succeeds, the selector
 * will start and run the next child task.
 * 
 * @param <E> type of the blackboard object that tasks use to read or modify game state
 * 
 * @author implicit-invocation */
public class Sequence<E> : SingleRunningChildBranch<E>
{

    /** Creates a {@code Sequence} branch with no children. */
    public Sequence() : base()
    {

    }

    /** Creates a {@code Sequence} branch with the given children.
	 * 
	 * @param tasks the children of this task */
    public Sequence(List<BTreeTask<E>> tasks) : base(tasks)
    {
        ;
    }

    /** Creates a {@code Sequence} branch with the given children.
	 * 
	 * @param tasks the children of this task */
    public Sequence(BTreeTask<E>[] tasks) : base(tasks.ToList())
    {

    }


    public override void childSuccess(BTreeTask<E> runningTask)
    {
        base.childSuccess(runningTask);
        if (++currentChildIndex < children.Count())
        {
            run(); // Run next child
        }
        else
        {
            success(); // All children processed, return success status
        }
    }


    public override void childFail(BTreeTask<E> runningTask)
    {
        base.childFail(runningTask);
        fail(); // Return failure status when a child says it failed
    }

}
