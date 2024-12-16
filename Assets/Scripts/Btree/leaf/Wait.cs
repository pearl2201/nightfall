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
 * 等待执行指定次数<br>
 * {@code Wait} is a leaf that keeps running for the specified amount of time
 * then succeeds.
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author davebaol
 * @fix JiangZhiYong
 */
public class Wait<E> : LeafTask<E>
{
    private const float INIT_SECONDS = 0f;

    /**
	 * Mandatory task attribute specifying the random distribution that determines
	 * the timeout in seconds.
	 */
    [BtreeTask(required = true)]

    public float seconds;

    private float startTime;
    private float timeout;

    /** Creates a {@code Wait} task that immediately succeeds. */
    public Wait() : this(INIT_SECONDS)
    {

    }

    /**
	 * Creates a {@code Wait} task running for the specified number of seconds.
	 * 
	 * @param seconds
	 *            the number of seconds to wait for
	 */
    public Wait(float seconds)
    {
        this.seconds = seconds;
    }

    /**
	 * Draws a value from the distribution that determines the seconds to wait for.
	 * <p>
	 * This method is called when the task is entered. Also, this method internally
	 * calls {@link Timepiece#getTime() GdxAI.getTimepiece().getTime()} to get the
	 * current AI time. This means that
	 * <ul>
	 * <li>if you forget to {@link Timepiece#update(float) update the timepiece}
	 * this task will keep running indefinitely.</li>
	 * <li>the timepiece should be updated before this task runs.</li>
	 * </ul>
	 */

    public override void start()
    {
        timeout = seconds;
        startTime = DateTime.UtcNow.Millisecond;
    }

    /**
	 * Executes this {@code Wait} task.
	 * 
	 * @return {@link Status#SUCCEEDED} if the specified timeout has expired;
	 *         {@link Status#RUNNING} otherwise.
	 */

    public override Status execute()
    {
        return DateTime.UtcNow.Millisecond - startTime < timeout ? Status.RUNNING : Status.SUCCEEDED;
    }

    //
    // protected BTreeTask<E> copyTo(BTreeTask<E> task) {
    // ((Wait<E>) task).seconds = seconds;
    // return task;
    // }


    public override void release()
    {
        seconds = INIT_SECONDS;
        startTime = 0;
        timeout = 0;
        base.release();
    }

}
