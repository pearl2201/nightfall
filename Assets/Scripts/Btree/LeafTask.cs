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
 * 叶子节点任务 <br>
 * A {@code LeafTask} is a terminal task of a behavior tree, contains action or
 * condition logic, can not have any child.
 * 
 * @param <E>
 *            type of the blackboard object that tasks use to read or modify
 *            game state
 * 
 * @author implicit-invocation
 * @author davebaol
 * @fix JiangZhiYong
 */
[BtreeTaskConstraint(minChildren = 0, maxChildren = 0)]
public abstract class LeafTask<E> : BTreeTask<E> {

	/** Creates a leaf task. */
	public LeafTask() {
	}

	/**
	 * This method contains the update logic of this leaf task. The actual
	 * implementation MUST return one of {@link Status#RUNNING} ,
	 * {@link Status#SUCCEEDED} or {@link Status#FAILED}. Other return values will
	 * cause an {@code InvalidOperationException}.
	 * 
	 * @return the status of this leaf task
	 */
	public abstract Status execute();

	/**
	 * This method contains the update logic of this task. The implementation
	 * delegates the {@link #execute()} method.
	 */

	public override void run() {
		Status result = execute();
		if (result == null) {
			throw new InvalidOperationException("Invalid status 'null' returned by the execute method");
		}
		switch (result) {
		case Status.SUCCEEDED:
			success();
			return;
		case Status.FAILED:
			fail();
			return;
		case Status.RUNNING:
			running();
			return;
		default:
			throw new InvalidOperationException("Invalid status '" + result.ToString() + "' returned by the execute method");
		}
	}

	/**
	 * Always throws {@code InvalidOperationException} because a leaf task cannot have
	 * any children.
	 */

	protected override int addChildToTask(BTreeTask<E> child) {
		throw new InvalidOperationException("A leaf task cannot have any children");
	}

	
	public override int getChildCount() {
		return 0;
	}

	
	public override BTreeTask<E> getChild(int i) {
		throw new IndexOutOfRangeException("A leaf task can not have any child");
	}

	
	public override void childRunning(BTreeTask<E> runningTask, BTreeTask<E> reporter) {
	}

	
	public override void childFail(BTreeTask<E> runningTask) {
	}

	
	public override void childSuccess(BTreeTask<E> runningTask) {
	}

}
