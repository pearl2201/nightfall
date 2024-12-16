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
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;

/**
 * 并行<br>
 * A {@code Parallel} is a special branch task that runs all children when
 * stepped. Its actual behavior depends on its {@link orchestrator} and
 * {@link policy}.<br>
 * <br>
 * The execution of the parallel task's children depends on its
 * {@link #orchestrator}:
 * <ul>
 * <li>{@link Orchestrator#Resume}: the parallel task restarts or runs each
 * child every step</li>
 * <li>{@link Orchestrator#Join}: child tasks will run until success or failure
 * but will not re-run until the parallel task has succeeded or failed</li>
 * </ul>
 * 
 * The actual result of the parallel task depends on its {@link #policy}:
 * <ul>
 * <li>{@link Policy#Sequence}: the parallel task fails as soon as one child
 * fails; if all children succeed, then the parallel task succeeds. This is the
 * default policy.</li>
 * <li>{@link Policy#Selector}: the parallel task succeeds as soon as one child
 * succeeds; if all children fail, then the parallel task fails.</li>
 * </ul>
 * 
 * The typical use case: make the game entity react on event while sleeping or
 * wandering.
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author implicit-invocation
 * @author davebaol
 */
public abstract class Parallel<E> : BranchTask<E>
{

    /**
	 * Optional task attribute specifying the parallel policy (defaults to
	 * {@link Policy#Sequence})
	 */
    [BtreeTaskAttribute]
    public IPolicy policy;
    /**
	 * Optional task attribute specifying the execution policy (defaults to
	 * {@link Orchestrator#Resume})
	 */
    [@BtreeTaskAttribute]
    public IOrchestrator orchestrator;

    public bool noRunningTasks;
    public bool lastResult;
    public int currentChildIndex;

    /**
	 * Creates a parallel task with sequence policy, resume orchestrator and no
	 * children
	 */
    public Parallel() : this(new List<BTreeTask<E>>())
    {

    }

    /**
	 * Creates a parallel task with sequence policy, resume orchestrator and the
	 * given children
	 * 
	 * @param tasks
	 *            the children
	 */
    public Parallel(BTreeTask<E>[] tasks) : base(tasks.ToList())
    {

    }

    /**
	 * Creates a parallel task with sequence policy, resume orchestrator and the
	 * given children
	 * 
	 * @param tasks
	 *            the children
	 */
    public Parallel(List<BTreeTask<E>> tasks) : this(Policy.Sequence, tasks)
    {

    }

    /**
	 * Creates a parallel task with the given policy, resume orchestrator and no
	 * children
	 * 
	 * @param policy
	 *            the policy
	 */
    public Parallel(IPolicy policy) : this(policy, new List<BTreeTask<E>>())
    {

    }

    /**
	 * Creates a parallel task with the given policy, resume orchestrator and the
	 * given children
	 * 
	 * @param policy
	 *            the policy
	 * @param tasks
	 *            the children
	 */
    public Parallel(IPolicy policy, BTreeTask<E>[] tasks) : this(policy, tasks.ToList())
    {

    }

    /**
	 * Creates a parallel task with the given policy, resume orchestrator and the
	 * given children
	 * 
	 * @param policy
	 *            the policy
	 * @param tasks
	 *            the children
	 */
    public Parallel(IPolicy policy, List<BTreeTask<E>> tasks) : this(policy, Orchestrator.Resume, tasks)
    {

    }

    /**
	 * Creates a parallel task with the given orchestrator, sequence policy and the
	 * given children
	 * 
	 * @param orchestrator
	 *            the orchestrator
	 * @param tasks
	 *            the children
	 */
    public Parallel(IOrchestrator orchestrator, List<BTreeTask<E>> tasks) : this(Policy.Sequence, orchestrator, tasks)
    {

    }

    /**
	 * Creates a parallel task with the given orchestrator, sequence policy and the
	 * given children
	 * 
	 * @param orchestrator
	 *            the orchestrator
	 * @param tasks
	 *            the children
	 */
    public Parallel(IOrchestrator orchestrator, BTreeTask<E>[] tasks) : this(Policy.Sequence, orchestrator, tasks.ToList())
    {

    }

    /**
	 * 
	 * @param policy
	 * @param orchestrator
	 */
    public Parallel(IPolicy policy, IOrchestrator orchestrator) : this(policy, orchestrator, new List<BTreeTask<E>>())
    {

    }

    /**
	 * Creates a parallel task with the given orchestrator, policy and children
	 * 
	 * @param policy
	 *            the policy
	 * @param orchestrator
	 *            the orchestrator
	 * @param tasks
	 *            the children
	 */
    public Parallel(IPolicy policy, IOrchestrator orchestrator, List<BTreeTask<E>> tasks) : base(tasks)
    {

        this.policy = policy;
        this.orchestrator = orchestrator;
        noRunningTasks = true;
    }


    public override void run()
    {
        orchestrator.execute(this);
    }


    public override void childRunning(BTreeTask<E> task, BTreeTask<E> reporter)
    {
        noRunningTasks = false;
    }


    public override void childSuccess(BTreeTask<E> runningTask)
    {
        lastResult = policy.onChildSuccess(this);
    }


    public override void childFail(BTreeTask<E> runningTask)
    {
        lastResult = policy.onChildFail(this);
    }


    public override void resetTask()
    {
        base.resetTask();
        noRunningTasks = true;
    }

    // 
    // protected BTreeTask<E> copyTo(BTreeTask<E> task) {
    // Parallel<E> parallel = (Parallel<E>) task;
    // parallel.policy = policy; // no need to clone since it is immutable
    // parallel.orchestrator = orchestrator; // no need to clone since it is
    // immutable
    // return base.copyTo(task);
    // }

    public void resetAllChildren()
    {
        for (int i = 0, n = getChildCount(); i < n; i++)
        {
            BTreeTask<E> child = getChild(i);
            child.release();
        }
    }



    /**
     * Called by parallel task each run
     * 
     * @param parallel
     *            The {@link Parallel} task
     */
    public abstract void execute(Parallel<E> parallel);



    public override void release()
    {
        policy = Policy.Sequence;
        orchestrator = Orchestrator.Resume;
        noRunningTasks = true;
        lastResult = default;
        currentChildIndex = 0;
        base.release();
    }


    /**
     * Called by parallel task each time one of its children succeeds.
     * 
     * @param parallel
     *            the parallel task
     * @return {@code bool.TRUE} if parallel must succeed, {@code bool.FALSE}
     *         if parallel must fail and {@code null} if parallel must keep on
     *         running.
     */
    public abstract bool onChildSuccess(Parallel<E> parallel);

    /**
     * Called by parallel task each time one of its children fails.
     * 
     * @param parallel
     *            the parallel task
     * @return {@code bool.TRUE} if parallel must succeed, {@code bool.FALSE}
     *         if parallel must fail and {@code null} if parallel must keep on
     *         running.
     */
    public abstract bool onChildFail(Parallel<E> parallel);

}

public interface IOrchestrator
{
    void execute<E>(Parallel<E> parallel);
}

public class ResumeOrchestrator : IOrchestrator
{
    public void execute<E>(Parallel<E> parallel)
    {
        parallel.noRunningTasks = true;
        parallel.lastResult = default;
        for (parallel.currentChildIndex = 0; parallel.currentChildIndex < parallel.children
                .Count(); parallel.currentChildIndex++)
        {
            BTreeTask<E> child = parallel.children.ElementAt(parallel.currentChildIndex);
            if (child.getStatus() == BTreeTask<E>.Status.RUNNING)
            {
                child.run();
            }
            else
            {
                child.setControl(parallel);
                child.start();
                if (child.checkGuard(parallel))
                    child.run();
                else
                    child.fail();
            }

            if (parallel.lastResult != default)
            { // Current child has finished either with success or fail
                parallel.cancelRunningChildren(parallel.noRunningTasks ? parallel.currentChildIndex + 1 : 0);
                if (parallel.lastResult)
                    parallel.success();
                else
                    parallel.fail();
                return;
            }
        }
        parallel.running();
    }
}

public class JoinOrchestrator : IOrchestrator
{
    public void execute<E>(Parallel<E> parallel)
    {
        parallel.noRunningTasks = true;
        parallel.lastResult = default;
        for (parallel.currentChildIndex = 0; parallel.currentChildIndex < parallel.children
                .Count(); parallel.currentChildIndex++)
        {
            BTreeTask<E> child = parallel.children.ElementAt(parallel.currentChildIndex);

            switch (child.getStatus())
            {
                case BTreeTask<E>.Status.RUNNING:
                    child.run();
                    break;
                case BTreeTask<E>.Status.SUCCEEDED:
                case BTreeTask<E>.Status.FAILED:
                    break;
                default:
                    child.setControl(parallel);
                    child.start();
                    if (child.checkGuard(parallel))
                        child.run();
                    else
                        child.fail();
                    break;
            }

            if (parallel.lastResult != default)
            { // Current child has finished either with success or fail
                parallel.cancelRunningChildren(parallel.noRunningTasks ? parallel.currentChildIndex + 1 : 0);
                parallel.resetAllChildren();
                if (parallel.lastResult)
                    parallel.success();
                else
                    parallel.fail();
                return;
            }
        }
        parallel.running();
    }
}
/**
     * 协调器
     * The enumeration of the child orchestrators supported by the {@link Parallel}
     * task
     */
public static class Orchestrator
{
    /**
     * 重置
     * The default orchestrator - starts or resumes all children every single step
     */
    public static ResumeOrchestrator Resume = new ResumeOrchestrator();
    /**
     * Children execute until they succeed or fail but will not re-run until the
     * parallel task has succeeded or failed
     */
    public static JoinOrchestrator Join = new JoinOrchestrator();
}


public interface IPolicy
{
    bool onChildSuccess<E>(Parallel<E> parallel);

    bool onChildFail<E>(Parallel<E> parallel);
}

public class PolicySequence : IPolicy
{
    public bool onChildSuccess<E>(Parallel<E> parallel)
    {
        if (parallel.orchestrator is JoinOrchestrator)
        {

            return parallel.noRunningTasks
                    && parallel.children.ElementAt(parallel.children.Count() - 1).getStatus() == BTreeTask<E>.Status.SUCCEEDED;
        }
        else if (parallel.orchestrator is JoinOrchestrator)
        {

            return parallel.noRunningTasks && parallel.currentChildIndex == parallel.children.Count() - 1;
        }
        return false;
    }


    public bool onChildFail<E>(Parallel<E> parallel)
    {
        return false;
    }
}

/**
 * 全部成功就成功，有一个失败就失败
 * The sequence policy makes the {@link Parallel} task fail as soon as one child
 * fails; if all children succeed, then the parallel task succeeds. This is the
 * default policy.
 */
public class PolicySelector : IPolicy
{
    public bool onChildSuccess<E>(Parallel<E> parallel)
    {

        return true;
    }


    public bool onChildFail<E>(Parallel<E> parallel)
    {
        return parallel.noRunningTasks && parallel.currentChildIndex == parallel.children.Count() - 1;
    }
}
/** The enumeration of the policies supported by the {@link Parallel} task. */
public static class Policy
{
    public static IPolicy Sequence = new PolicySequence();

    public static IPolicy Selector = new PolicySelector();

}