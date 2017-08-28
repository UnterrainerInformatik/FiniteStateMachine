// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Collections.Generic;
using StateMachine.Events;

namespace StateMachine.Fluent.Api
{
    public class FluentImplementation<TState, TTrigger, TData> : GlobalTransitionBuilderFluent<TState, TTrigger, TData>,
        TransitionStateFluent<TState, TTrigger, TData>
    {
        private FsmModel<TState, TTrigger, TData> FsmModel { get; set; } = new FsmModel<TState, TTrigger, TData>();
        private TState startState;

        private Tuple<TState> currentState;
        private Tuple<TState, TState> currentTransition;
        private Tuple<TState> currentGlobalTransition;

        private Dictionary<Tuple<TState>, StateModel<TState, TTrigger, TData>> stateModels =
            new Dictionary<Tuple<TState>, StateModel<TState, TTrigger, TData>>();

        private Dictionary<Tuple<TState, TState>, TransitionModel<TState, TTrigger, TData>> transitionModels =
            new Dictionary<Tuple<TState, TState>, TransitionModel<TState, TTrigger, TData>>();

        private Dictionary<Tuple<TState>, TransitionModel<TState, TTrigger, TData>> globalTransitionModels =
            new Dictionary<Tuple<TState>, TransitionModel<TState, TTrigger, TData>>();

        public FluentImplementation(TState startState)
        {
            this.startState = startState;
        }

        public Fsm<TState, TTrigger, TData> Build()
        {
            if (FsmModel.States[startState] == null)
            {
                throw FsmBuilderException.StartStateCannotBeNull();
            }

            FsmModel.Current = FsmModel.States[startState];
            Fsm<TState, TTrigger, TData> fsm = new Fsm<TState, TTrigger, TData>(FsmModel);
            return fsm;
        }

        public StateFluent<TState, TTrigger, TData> State(TState state)
        {
            currentState = Tuple.Create(state);
            if (!FsmModel.States.ContainsKey(state))
            {
                stateModels[currentState] = new StateModel<TState, TTrigger, TData>(state);
                FsmModel.States[state] = new State<TState, TTrigger, TData>(stateModels[currentState]);
            }
            return this;
        }

        public StateFluent<TState, TTrigger, TData> OnEnter(EventHandler<StateChangeArgs<TState, TTrigger, TData>> enter)
        {
            stateModels[currentState].AddEnteredHandler(enter);
            return this;
        }

        public StateFluent<TState, TTrigger, TData> OnExit(EventHandler<StateChangeArgs<TState, TTrigger, TData>> exit)
        {
            stateModels[currentState].AddExitedHandler(exit);
            return this;
        }

        public StateFluent<TState, TTrigger, TData> Update(EventHandler<UpdateArgs<TState, TTrigger, TData>> update)
        {
            stateModels[currentState].AddUpdatedHandler(update);
            return this;
        }

        public GlobalTransitionFluent<TState, TTrigger, TData> GlobalTransitionTo(TState state)
        {
            currentGlobalTransition = Tuple.Create(state);
            if (!globalTransitionModels.ContainsKey(currentGlobalTransition))
            {
                globalTransitionModels[currentGlobalTransition] =
                    new TransitionModel<TState, TTrigger, TData>(startState, state);
                FsmModel.GlobalTransitions[state] =
                    new Transition<TState, TTrigger, TData>(globalTransitionModels[currentGlobalTransition]);
            }
            return this;
        }

        public GlobalTransitionBuilderFluent<TState, TTrigger, TData> OnGlobal(TTrigger trigger)
        {
            globalTransitionModels[currentGlobalTransition].Triggers.Add(trigger);
            return this;
        }

        public GlobalTransitionBuilderFluent<TState, TTrigger, TData> IfGlobal(
            Func<TState, TState, TTrigger, bool> condition)
        {
            globalTransitionModels[currentGlobalTransition].Conditions.Add(condition);
            return this;
        }

        public TransitionFluent<TState, TTrigger, TData> TransitionTo(TState state)
        {
            currentTransition = Tuple.Create(currentState.Item1, state);
            if (!transitionModels.ContainsKey(currentTransition))
            {
                transitionModels[currentTransition] = new TransitionModel<TState, TTrigger, TData>(currentState.Item1,
                    state);
                stateModels[currentState].Transitions[state] =
                    new Transition<TState, TTrigger, TData>(transitionModels[currentTransition]);
            }
            return this;
        }

        public TransitionStateFluent<TState, TTrigger, TData> On(TTrigger trigger)
        {
            transitionModels[currentTransition].Triggers.Add(trigger);
            return this;
        }

        public TransitionStateFluent<TState, TTrigger, TData> If(Func<TState, TState, TTrigger, bool> condition)
        {
            transitionModels[currentTransition].Conditions.Add(condition);
            return this;
        }

        public StateFluent<TState, TTrigger, TData> ClearsStack()
        {
            FsmModel.States[currentState.Item1].ClearStack = true;
            return this;
        }
    }
}