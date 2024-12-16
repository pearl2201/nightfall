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



/**
 * 结果随机<br>
 * The {@code Random} decorator succeeds with the specified probability,
 * regardless of whether the wrapped task fails or succeeds. Also, the wrapped
 * task is optional, meaning that this decorator can act like a leaf task.
 * <p>
 * Notice that if success probability is 1 this task is equivalent to the
 * decorator {@link AlwaysSucceed} and the leaf {@link Success}. Similarly if
 * success probability is 0 this task is equivalent to the decorator
 * {@link AlwaysFail} and the leaf {@link Failure}.
 *
 * @param <E> type of the blackboard object that tasks use to read or modify
 *            game state
 * @author davebaol
 * @fix JiangZhiYong
 */
[BtreeTaskConstraint(minChildren = 0, maxChildren = 1)]
public class Random<E> : Decorator<E>
{
    private const long serialVersionUID = 1L;

    private const float INIT_PRO = 0.5f;

    /**
     * Optional task attribute specifying the random distribution that determines
     * the success probability. It defaults to
     * {@link ConstantFloatDistribution#ZERO_POINT_FIVE}.
     */
    [BtreeTaskAttribute]
    public float success;

    private float p;

    /**
     * Creates a {@code Random} decorator with no child that succeeds or fails with
     * equal probability.
     */
    public Random() : this(INIT_PRO)
    {

    }

    /**
     * Creates a {@code Random} decorator with the given child that succeeds or
     * fails with equal probability.
     *
     * @param task the child task to wrap
     */
    public Random(BTreeTask<E> task) : this(INIT_PRO, task)
    {

    }

    /**
     * Creates a {@code Random} decorator with no child that succeeds with the
     * specified probability.
     *
     * @param success the random distribution that determines success probability
     */
    public Random(float success) : base()
    {

        this.success = success;
    }

    /**
     * Creates a {@code Random} decorator with the given child that succeeds with
     * the specified probability.
     *
     * @param success the random distribution that determines success probability
     * @param task    the child task to wrap
     */
    public Random(float success, BTreeTask<E> task) : base(task)
    {

        this.success = success;
    }

    /**
     * Draws a value from the distribution that determines the success probability.
     * <p>
     * This method is called when the task is entered.
     */

    public void start()
    {
        p = success;
    }


    public override void run()
    {
        if (child != null)
            base.run();
        else
            decide();
    }


    public override void childFail(BTreeTask<E> runningTask)
    {
        decide();
    }


    public override void childSuccess(BTreeTask<E> runningTask)
    {
        decide();
    }

    private void decide()
    {
        if (MathUtil.Random() <= p)
        {
            success();
        }
        else
        {
            fail();
        }
    }



    public override void release()
    {
        this.p = 0;
        this.success = INIT_PRO;
        base.release();
    }
}
