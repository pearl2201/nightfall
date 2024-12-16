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
 * 堆栈式状态机，可依次返回之前状态 ，如菜单列表
 * <br>
 * A {@link StateMachine} implementation that keeps track of
 * all previous {@link State}s via a stack. This makes sense for example in case
 * of a hierarchical menu structure where each menu screen is one state and one
 * wants to navigate back to the main menu anytime, via
 * {@link #revertToPreviousState()}.
 * 
 * @param <E>
 *            the type of the entity owning this state machine
 * @param <S>
 *            the type of the states of this state machine
 * 
 * @author Daniel Holderbaum
 * 
 * @fix JiangZhiYong
 */
public class StackStateMachine<E, S> : DefaultStateMachine<E, S> where S : State<E>
{


    private List<S> stateStack;

    /**
	 * Creates a {@code StackStateMachine} with no owner, initial state and global
	 * state.
	 */
    public StackStateMachine() : this(default(E), default(S), default(S))
    {

    }

    /**
	 * Creates a {@code StackStateMachine} for the specified owner.
	 * 
	 * @param owner
	 *            the owner of the state machine
	 */
    public StackStateMachine(E owner) : this(owner, default(S), default(S))
    {

    }

    /**
	 * Creates a {@code StackStateMachine} for the specified owner and initial
	 * state.
	 * 
	 * @param owner
	 *            the owner of the state machine
	 * @param initialState
	 *            the initial state
	 */
    public StackStateMachine(E owner, S initialState) : this(owner, initialState, default(S))
    {

    }

    /**
	 * Creates a {@code StackStateMachine} for the specified owner, initial state
	 * and global state.
	 * 
	 * @param owner
	 *            the owner of the state machine
	 * @param initialState
	 *            the initial state
	 * @param globalState
	 *            the global state
	 */
    public StackStateMachine(E owner, S initialState, S globalState) : base(owner, initialState, globalState)
    {
        
    }


    public override void setInitialState(S state)
    {
        if (stateStack == null)
        {
            stateStack = new List<S>();
        }

        this.stateStack.Clear();
        this.currentState = state;
    }


    public override S getCurrentState()
    {
        return currentState;
    }

    /**
	 * Returns the last state of this state machine. That is the high-most state on
	 * the internal stack of previous states.
	 */

    public override S getPreviousState()
    {
        if (stateStack.Count() == 0)
        {
            return default(S);
        }
        else
        {
            return stateStack.ElementAt(stateStack.Count() - 1);
        }
    }


    public override void changeState(S newState)
    {
        changeState(newState, true);
    }

    /**
	 * Changes the Change state back to the previous state. That is the high-most
	 * state on the internal stack of previous states.
	 * 
	 * @return {@code True} in case there was a previous state that we were able to
	 *         revert to. In case there is no previous state, no state change occurs
	 *         and {@code false} will be returned.
	 */

    public override bool revertToPreviousState()
    {
        if (stateStack.Count() == 0)
        {
            return false;
        }

        S previousState = stateStack[stateStack.Count() - 1];
            stateStack.Remove(previousState);
        changeState(previousState, false);
        return true;
    }

    private void changeState(S newState, bool pushCurrentStateToStack)
    {
        if (pushCurrentStateToStack && currentState != null)
        {
            stateStack.Add(currentState);
        }

        // Call the exit method of the existing state
        if (currentState != null)
            currentState.exit(owner);

        // Change state to the new state
        currentState = newState;

        // Call the entry method of the new state
        currentState.enter(owner);
    }

}
