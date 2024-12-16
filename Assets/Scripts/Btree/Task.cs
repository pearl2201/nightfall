

using Pika.Base.Mathj.Geometry;
using System;
using System.Linq;


/**
 * 行为树抽象节点任务
 * 
 * @author davebaol
 * @QQ 359135103 2017年11月22日 下午2:33:32
 * @fix JiangZhiYong
 * @param <E>
 *            黑板对象，所属的对象，如NPC
 */
[BtreeTaskConstraint]
public abstract class BTreeTask<E> : IMemoryObject
{
    private const long serialVersionUID = 1L;


    /** 任务状态 */
    protected Status status = Status.FRESH;

    /** 父节点 */
    protected BTreeTask<E> control;

    /** 该节点任务所属的行为树 */
    protected BehaviorTree<E> tree;

    /** 当前任务的防护条件 */
    protected BTreeTask<E> guard;
    /** 节点名称，调试识别 */
    protected String name;

    /**
	 * 添加子任务
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月22日 下午2:54:08
	 * @param childTask
	 * @return 子任务所在下标
	 */
    public int addChild(BTreeTask<E> childTask)
    {
        int index = addChildToTask(childTask);
        if (tree != null && tree.listeners != null)
        {
            tree.notifyChildAdded(childTask, index);
        }
        return index;
    }

    /**
	 * 子任务个数
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月22日 下午4:01:24
	 * @return
	 */
    public abstract int getChildCount();

    /**
	 * 向该任务添加子任务
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月22日 下午3:32:26
	 * @param childTask
	 *            子任务
	 * @return 任务下标
	 */
    protected abstract int addChildToTask(BTreeTask<E> childTask);

    public E getObject()
    {
        if (tree == null)
        {
            throw new Exception("行为树对象未设置");
        }
        return tree.getObject();
    }

    public BTreeTask<E> getGuard()
    {
        return guard;
    }

    public void setGuard(BTreeTask<E> guard)
    {
        this.guard = guard;
    }

    public Status getStatus()
    {
        return status;
    }

    /**
	 * 设置父任务
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月22日 下午4:06:58
	 */
    public void setControl(BTreeTask<E> control)
    {
        this.control = control;
        this.tree = control.tree;
    }

    /**
	 * 检查条件
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月22日 下午4:09:57
	 * @param parentTask
	 *            父任务
	 * @return
	 */
    public bool checkGuard(BTreeTask<E> parentTask)
    {
        // No guard to check
        if (guard == null)
        {
            return true;
        }

        // Check the guard of the guard recursively
        if (!guard.checkGuard(parentTask))
        {
            return false;
        }

        // Use the tree's guard evaluator task to check the guard of this task
        guard.setControl(parentTask.tree.guardEvaluator);
        guard.start();
        guard.run();
        switch (guard.getStatus())
        {
            case Status.SUCCEEDED:
                return true;
            case Status.FAILED:
                return false;
            default:
                throw new Exception("Illegal guard status '" + guard.getStatus()
                        + "'. Guards must either succeed or fail in one step.");
        }
    }

    /** This method will be called once before this task's first run. */
    public virtual void start()
    {
    }

    /**
	 * This method will be called by {@link #success()}, {@link #fail()} or
	 * {@link #cancel()}, meaning that this task's status has just been set to
	 * {@link Status#SUCCEEDED}, {@link Status#FAILED} or {@link Status#CANCELLED}
	 * respectively.
	 */
    public virtual void end()
    {
    }

    /**
	 * This method contains the update logic of this task. The actual implementation
	 * MUST call {@link #running()}, {@link #success()} or {@link #fail()} exactly
	 * once.
	 */
    public abstract void run();

    /**
	 * This method will be called in {@link #run()} to inform control that this task
	 * needs to run again
	 */
    public void running()
    {
        Status previousStatus = status;
        status = Status.RUNNING;
        if (tree.listeners != null && tree.listeners.Count() > 0)
        {
            tree.notifyStatusUpdated(this, previousStatus);
        }

        if (control != null)
        {
            control.childRunning(this, this);
        }

    }

    /**
	 * This method will be called when one of the ancestors of this task needs to
	 * run again
	 * 
	 * @param runningTask
	 *            the task that needs to run again
	 * @param reporter
	 *            the task that reports, usually one of this task's children
	 */
    public abstract void childRunning(BTreeTask<E> runningTask, BTreeTask<E> reporter);

    /**
	 * This method will be called in {@link #run()} to inform control that this task
	 * has finished running with a success result
	 */
    public void success()
    {
        Status previousStatus = status;
        status = Status.SUCCEEDED;
        if (tree.listeners != null && tree.listeners.Count() > 0)
        {
            tree.notifyStatusUpdated(this, previousStatus);
        }

        end();
        if (control != null)
        {
            control.childSuccess(this);
        }
    }

    /**
	 * This method will be called in {@link #run()} to inform control that this task
	 * has finished running with a failure result
	 */
    public void fail()
    {
        Status previousStatus = status;
        status = Status.FAILED;
        if (tree.listeners != null && tree.listeners.Count() > 0)
        {
            tree.notifyStatusUpdated(this, previousStatus);
        }

        end();
        if (control != null)
        {
            control.childFail(this);
        }
    }

    /**
	 * This method will be called when one of the children of this task succeeds
	 * 
	 * @param task
	 *            the task that succeeded
	 */
    public abstract void childSuccess(BTreeTask<E> task);

    /**
	 * This method will be called when one of the children of this task fails
	 * 
	 * @param task
	 *            the task that failed
	 */
    public abstract void childFail(BTreeTask<E> task);

    /**
	 * Terminates this task and all its running children. This method MUST be called
	 * only if this task is running.
	 */
    public void cancel()
    {
        cancelRunningChildren(0);
        Status previousStatus = status;
        status = Status.CANCELLED;
        if (tree.listeners != null && tree.listeners.Count() > 0)
            tree.notifyStatusUpdated(this, previousStatus);
        end();
    }

    /**
	 * Terminates the running children of this task starting from the specified
	 * index up to the end.
	 * 
	 * @param startIndex
	 *            the start index
	 */
    public virtual void cancelRunningChildren(int startIndex)
    {
        for (int i = startIndex, n = getChildCount(); i < n; i++)
        {
            BTreeTask<E> child = getChild(i);
            if (child.status == Status.RUNNING)
            {
                child.cancel();
            }
        }
    }

    /** Returns the child at the given index. */
    public abstract BTreeTask<E> getChild(int i);

    /** Resets this task to make it restart from scratch on next run. */
    public virtual void resetTask()
    {
        if (status == Status.RUNNING)
        {
            cancel();
        }
        for (int i = 0, n = getChildCount(); i < n; i++)
        {
            getChild(i).resetTask();
        }
        status = Status.FRESH;
        tree = null;
        control = null;
    }


    public virtual void release()
    {
        control = null;
        guard = null;
        status = Status.FRESH;
        tree = null;

    }

    /**
	 * 节点状态 <br>
	 * The enumeration of the values that a task's status can have.
	 * 
	 * @author davebaol
	 */
    public enum Status
    {
        /** Means that the task has never run or has been reset. */
        FRESH,
        /** Means that the task needs to run again. */
        RUNNING,
        /** Means that the task returned a failure result. */
        FAILED,
        /** Means that the task returned a success result. */
        SUCCEEDED,
        /** Means that the task has been terminated by an ancestor. */
        CANCELLED
    }

    /** 节点名称，调试识别 */
    public void setName(String name)
    {
        this.name = name;
    }

    public String getName()
    {
        return name;
    }


}
