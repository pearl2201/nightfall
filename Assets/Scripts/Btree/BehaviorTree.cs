using System;
using System.Collections.Generic;


/**
 * 行为树 <br>
 * 
 * @author implicit-invocation
 * @author davebaol
 * @fix JiangZhiYong
 * @QQ 359135103 2017年11月22日 下午2:43:23
 */
public class BehaviorTree<E> : BTreeTask<E>
{


    /** 根任务 */
    private BTreeTask<E> rootTask;

    /** 行为树所属对象 */
    private E Object;

    /** 监听器 */
    public List<Listener<E>> listeners;
    /**安全监测*/
    public GuardEvaluator<E> guardEvaluator;

    /**
     * Creates a {@code BehaviorTree} with no root task and no blackboard object. Both the root task and the blackboard
     * object must be set before running this behavior tree, see {@link #addChild(Task) addChild()} and
     * {@link #setObject(Object) setObject()} respectively.
     */
    public BehaviorTree() : this(null, default(E))
    {

    }

    /**
     * Creates a behavior tree with a root task and no blackboard object. Both the root task and the blackboard object
     * must be set before running this behavior tree, see {@link #addChild(Task) addChild()} and
     * {@link #setObject(Object) setObject()} respectively.
     * 
     * @param rootTask the root task of this tree. It can be {@code null}.
     */
    public BehaviorTree(BTreeTask<E> rootTask) : this(rootTask, default(E))
    {

    }

    /**
     * Creates a behavior tree with a root task and a blackboard object. Both the root task and the blackboard object
     * must be set before running this behavior tree, see {@link #addChild(Task) addChild()} and
     * {@link #setObject(Object) setObject()} respectively.
     * 
     * @param rootTask the root task of this tree. It can be {@code null}.
     * @param object the blackboard. It can be {@code null}.
     */
    public BehaviorTree(BTreeTask<E> rootTask, E obj)
    {
        this.rootTask = rootTask;
        this.Object = obj;
        this.tree = this;
        this.guardEvaluator = new GuardEvaluator<E>(this);
    }

    /** Returns the blackboard object of this behavior tree. */

    public E getObject()
    {
        return Object;
    }

    /**
     * Sets the blackboard object of this behavior tree.
     * 
     * @param object the new blackboard
     */
    public void setObject(E obj)
    {
        this.Object = obj;
    }

    /**
     * This method will add a child, namely the root, to this behavior tree.
     * 
     * @param child the root task to add
     * @return the index where the root task has been added (always 0).
     * @throws InvalidOperationException if the root task is already set.
     */

    protected override int addChildToTask(BTreeTask<E> child)
    {
        if (this.rootTask != null)
            throw new InvalidOperationException("A behavior tree cannot have more than one root task");
        this.rootTask = child;
        return 0;
    }


    public override int getChildCount()
    {
        return rootTask == null ? 0 : 1;
    }


    public override BTreeTask<E> getChild(int i)
    {
        if (i == 0 && rootTask != null)
        {
            return rootTask;
        }
        throw new IndexOutOfRangeException("index can't be >= size: " + i + " >= " + getChildCount());
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

    /**
     * This method should be called when game entity needs to make decisions: call this in game loop or after a fixed
     * time slice if the game is real-time, or on entity's turn if the game is turn-based
     * 当游戏实体需要做出决策时，应该调用此方法:如果游戏是实时的，则在游戏循环或固定时间片之后调用此方法;如果游戏是基于回合的，则在实体回合调用
     */
    public void step()
    {
        if (rootTask.getStatus() == Status.RUNNING)
        {
            rootTask.run();
        }
        else
        {
            rootTask.setControl(this);
            rootTask.start();
            if (rootTask.checkGuard(this))
            {
                rootTask.run();
            }
            else
            {
                rootTask.fail();
            }
        }
    }

    /**
     * 通知添加子任务
     * 
     * @author JiangZhiYong
     * @QQ 359135103 2017年11月22日 下午4:00:00
     * @param task
     * @param index
     */
    public void notifyChildAdded(BTreeTask<E> task, int index)
    {
        foreach (Listener<E> listener in listeners)
        {
            listener.childAdded(task, index);
        }
    }

    /**
     * 通知任务更新
     * 
     * @author JiangZhiYong
     * @QQ 359135103 2017年11月22日 下午5:04:00
     * @param task
     * @param previousStatus
     */
    public void notifyStatusUpdated(BTreeTask<E> task, Status previousStatus)
    {
        foreach (Listener<E> listener in listeners)
        {
            listener.statusUpdated(task, previousStatus);
        }
    }


    public override void release()
    {
        removeListeners();
        this.rootTask = null;
        this.Object = default;
        this.listeners = null;
        base.release();
    }


    public override void run() { }


    public override void resetTask()
    {
        base.resetTask();
        tree = this;
    }

    /**
     * 添加监听器
     * 
     * @author JiangZhiYong
     * @QQ 359135103 2017年11月22日 下午5:39:57
     * @param listener
     */
    public void addListener(Listener<E> listener)
    {
        if (listeners == null)
        {
            listeners = new List<Listener<E>>();
        }
        listeners.Add(listener);
    }

    public void removeListener(Listener<E> listener)
    {
        if (listeners != null)
        {
            listeners.Remove(listener);
        }
    }

    public void removeListeners()
    {
        if (listeners != null)
        {
            listeners.Clear();
        }
    }


}



public class GuardEvaluator<E> : BTreeTask<E>
{

    // No argument constructor useful for Kryo serialization

    public GuardEvaluator() { }

    public GuardEvaluator(BehaviorTree<E> tree)
    {
        this.tree = tree;
    }


    protected override int addChildToTask(BTreeTask<E> child)
    {
        return 0;
    }


    public override int getChildCount()
    {
        return 0;
    }


    public override BTreeTask<E> getChild(int i)
    {
        return null;
    }


    public override void run() { }


    public override void childSuccess(BTreeTask<E> task) { }


    public override void childFail(BTreeTask<E> task) { }


    public override void childRunning(BTreeTask<E> runningTask, BTreeTask<E> reporter) { }

}
/**
 * 行为树事件
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月22日 下午3:50:50
 * @param <E>
 */
public interface Listener<E>
{

    /**
     * 状态更新
     * 
     * @author JiangZhiYong
     * @QQ 359135103 2017年11月22日 下午3:52:23
     * @param task
     * @param previousStatus 之前状态
     */
    void statusUpdated(BTreeTask<E> task, BTreeTask<E>.Status previousStatus);

    /**
     * 添加子任务
     * 
     * @author JiangZhiYong
     * @QQ 359135103 2017年11月22日 下午3:55:20
     * @param task 子任务
     * @param index 子任务位置
     */
    void childAdded(BTreeTask<E> task, int index);

}
