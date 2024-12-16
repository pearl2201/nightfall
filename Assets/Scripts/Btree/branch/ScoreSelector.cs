


using System.Collections.Generic;
using System.Linq;

/**
 * 分数选择器，分数越高，优先执行
 * 
 * @author JiangZhiYong
 * @mail 359135103@qq.com
 * @param <E>
 *            黑板对象
 */
public abstract class ScoreSelector<E> : Selector<E>
{
    /** 根据分数排序后的任务列表 */
    private List<BTreeTask<E>> scoreChildren = new List<BTreeTask<E>>();


    public override void start()
    {
        base.start();
        calculateScore();
        if (scoreChildren.Count() < 1)
        {
            scoreChildren.AddRange(this.children);
        }
    }


    public override void run()
    {
        if (runningChild != null)
        {
            runningChild.run();
        }
        else
        {
            if (currentChildIndex < scoreChildren.Count())
            {
                runningChild = scoreChildren.ElementAt(currentChildIndex);
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

    /**
	 * 计算子节点的分数 <br>
	 * 节点可根据名称获取
	 */
    protected abstract void calculateScore();


    public override void resetTask()
    {
        base.resetTask();
        this.scoreChildren.Clear();
    }


    public override void release()
    {
        base.release();
        this.scoreChildren.Clear();
    }

    public List<BTreeTask<E>> getScoreChildren()
    {
        return scoreChildren;
    }

}
