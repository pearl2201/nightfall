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
 * A {@code DynamicGuardSelector} is a branch task that executes the first child
 * whose guard is evaluated to {@code true}. At every AI cycle, the children's
 * guards are re-evaluated, so if the guard of the running child is evaluated to
 * {@code false}, it is cancelled, and the child with the highest priority
 * starts running. The {@code DynamicGuardSelector} task finishes when no guard
 * is evaluated to {@code true} (thus failing) or when its active child finishes
 * (returning the active child's termination status).
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author davebaol
 */
public class DynamicGuardSelector<E> : BranchTask<E>
{

    /** The child in the running status or {@code null} if no child is running. */
    protected BTreeTask<E> runningChild;

    /** Creates a {@code DynamicGuardSelector} branch with no children. */
    public DynamicGuardSelector() : base()
    {

    }

    /**
	 * Creates a {@code DynamicGuardSelector} branch with the given children.
	 * 
	 * @param tasks
	 *            the children of this task
	 */

    public DynamicGuardSelector(BTreeTask<E>[] tasks) : base(tasks.ToList())
    {

    }

    /**
	 * Creates a {@code DynamicGuardSelector} branch with the given children.
	 * 
	 * @param tasks
	 *            the children of this task
	 */
    public DynamicGuardSelector(List<BTreeTask<E>> tasks) : base(tasks)
    {

    }


    public override void childRunning(BTreeTask<E> task, BTreeTask<E> reporter)
    {
        runningChild = task;
        running(); // Return a running status when a child says it's running
    }


    public override void childSuccess(BTreeTask<E> task)
    {
        this.runningChild = null;
        success();
    }


    public override void childFail(BTreeTask<E> task)
    {
        this.runningChild = null;
        fail();
    }


    public override void run()
    {
        // Check guards
        BTreeTask<E> childToRun = null;
        for (int i = 0, n = children.Count(); i < n; i++)
        {
            BTreeTask<E> child = children.ElementAt(i);
            if (child.checkGuard(this))
            {
                childToRun = child;
                break;
            }
        }

        if (runningChild != null && runningChild != childToRun)
        {
            runningChild.cancel();
            runningChild = null;
        }
        if (childToRun == null)
        {
            fail();
        }
        else
        {
            if (runningChild == null)
            {
                runningChild = childToRun;
                runningChild.setControl(this);
                runningChild.start();
            }
            runningChild.run();
        }
    }


    public override void resetTask()
    {
        base.resetTask();
        this.runningChild = null;
    }

    //	
    //	protected BTreeTask<E> copyTo(BTreeTask<E> task) {
    //		DynamicGuardSelector<E> branch = (DynamicGuardSelector<E>) task;
    //		branch.runningChild = null;
    //
    //		return base.copyTo(task);
    //	}


    public override void release()
    {
        runningChild = null;
        base.release();
    }
}
