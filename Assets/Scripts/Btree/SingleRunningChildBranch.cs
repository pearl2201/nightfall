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

using Pika.Base.Utils;
using System;
using System.Collections.Generic;
using System.Linq;



/**
 * 只能运行一个子任务的分支任务<br>
 * A {@code SingleRunningChildBranch} task is a branch task that supports only
 * one running child at a time.
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author implicit-invocation
 * @author davebaol
 * @fix JiangZhiYong
 */
public abstract class SingleRunningChildBranch<E> : BranchTask<E>
{

    /** The child in the running status or {@code null} if no child is running. */
    protected BTreeTask<E> runningChild;

    /** The index of the child currently processed. */
    protected int currentChildIndex;

    /**
	 * Array of random children. If it's {@code null} this task is deterministic.
	 */
    protected BTreeTask<E>[] randomChildren;

    /** Creates a {@code SingleRunningChildBranch} task with no children */
    public SingleRunningChildBranch() : base()
    {

    }

    /**
	 * Creates a {@code SingleRunningChildBranch} task with a list of children
	 * 
	 * @param tasks
	 *            list of this task's children, can be empty
	 */
    public SingleRunningChildBranch(List<BTreeTask<E>> tasks) : base(tasks)
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
    }


    public override void childFail(BTreeTask<E> task)
    {
        this.runningChild = null;
    }


    public override void run()
    {
        if (runningChild != null)
        {
            runningChild.run();
        }
        else
        {
            if (currentChildIndex < children.Count())
            {
                if (randomChildren != null)
                {
                    int last = children.Count() - 1;
                    if (currentChildIndex < last)
                    {
                        // Random swap
                        int otherChildIndex = MathUtil.Random(currentChildIndex, last);
                        BTreeTask<E> tmp = randomChildren[currentChildIndex];
                        randomChildren[currentChildIndex] = randomChildren[otherChildIndex];
                        randomChildren[otherChildIndex] = tmp;
                    }
                    runningChild = randomChildren[currentChildIndex];
                }
                else
                {
                    runningChild = children.ElementAt(currentChildIndex);
                }
                runningChild.setControl(this);
                runningChild.start();
                if (!runningChild.checkGuard(this))
                    runningChild.fail();
                else
                    run();
            }
            else
            {
                // Should never happen; this case must be handled by subclasses in childXXX
                // methods
            }
        }
    }


    public override void start()
    {
        this.currentChildIndex = 0;
        runningChild = null;
    }


    public override void cancelRunningChildren(int startIndex)
    {
        base.cancelRunningChildren(startIndex);
        runningChild = null;
    }


    public override void resetTask()
    {
        base.resetTask();
        this.currentChildIndex = 0;
        this.runningChild = null;
        this.randomChildren = null;
    }




    protected BTreeTask<E>[] createRandomChildren()
    {
        BTreeTask<E>[] rndChildren = new BTreeTask<E>[children.Count()];
        Array.Copy(children.ToArray(), 0, rndChildren, 0, children.Count());
        return rndChildren;
    }


    public override void release()
    {
        this.currentChildIndex = 0;
        this.runningChild = null;
        this.randomChildren = null;
        base.release();
    }

}
