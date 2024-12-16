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
 * 装饰模式任务<br>
 * 
 * A {@code Decorator} is a wrapper that provides custom behavior for its child.
 * The child can be of any kind (branch task, leaf task, or another decorator).
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author implicit-invocation
 * @author davebaol
 */
[BtreeTaskConstraint(minChildren = 1, maxChildren = 1)]
public abstract class Decorator<E> : BTreeTask<E>
{

    /** The child task wrapped by this decorator */
    protected BTreeTask<E> child;

    /** Creates a decorator with no child task. */
    public Decorator()
    {
    }

    /**
	 * Creates a decorator that wraps the given task.
	 * 
	 * @param child
	 *            the task that will be wrapped
	 */
    public Decorator(BTreeTask<E> child)
    {
        this.child = child;
    }


    protected override int addChildToTask(BTreeTask<E> child)
    {
        if (this.child != null)
            throw new Exception("A decorator task cannot have more than one child");
        this.child = child;
        return 0;
    }


    public override int getChildCount()
    {
        return child == null ? 0 : 1;
    }


    public override BTreeTask<E> getChild(int i)
    {
        if (i == 0 && child != null)
        {
            return child;
        }
        throw new IndexOutOfRangeException("index can't be >= size: " + i + " >= " + getChildCount());
    }


    public override void run()
    {
        if (child.getStatus() == Status.RUNNING)
        {
            child.run();
        }
        else
        {
            child.setControl(this);
            child.start();
            if (child.checkGuard(this))
            {
                child.run();
            }
            else
            {
                child.fail();
            }
        }
    }


    public override void childRunning(BTreeTask<E> runningTask, BTreeTask<E> reporter)
    {
        running();
    }


    public override void childFail(BTreeTask<E> runningTask)
    {
        fail();
    }


    public override void childSuccess(BTreeTask<E> runningTask)
    {
        success();
    }


    public override void release()
    {
        child = null;
        base.release();
    }

}
