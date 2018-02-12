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
    public class FluentImplementation<TS, TT, TD> : GlobalTransitionBuilderFluent<TS, TT, TD>,
        TransitionStateFluent<TS, TT, TD>
    {
		public Dictionary<Tuple<TS, TS>, List<Timer<TS>>> AfterEntries { get; set; } = new Dictionary<Tuple<TS, TS>, List<Timer<TS>>>();
		public List<Timer<TS>> GlobalAfterEntries { get; set; } = new List<Timer<TS>>();

		protected FsmModel<TS, TT, TD> FsmModel { get; set; } = new FsmModel<TS, TT, TD>();
        protected TS startState;

        protected Tuple<TS> currentState;
        protected Tuple<TS, TS> currentTransition;
        protected Tuple<TS> currentGlobalTransition;

        protected Dictionary<Tuple<TS>, StateModel<TS, TT, TD>> stateModels =
            new Dictionary<Tuple<TS>, StateModel<TS, TT, TD>>();

        protected Dictionary<Tuple<TS, TS>, TransitionModel<TS, TT>> transitionModels =
            new Dictionary<Tuple<TS, TS>, TransitionModel<TS, TT>>();

        protected Dictionary<Tuple<TS>, TransitionModel<TS, TT>> globalTransitionModels =
            new Dictionary<Tuple<TS>, TransitionModel<TS, TT>>();

        public FluentImplementation(TS startState)
        {
            this.startState = startState;
        }

        public Fsm<TS, TT, TD> Build()
        {
            if (FsmModel.States[startState] == null)
            {
                throw FsmBuilderException.StartStateCannotBeNull();
            }

            FsmModel.Current = FsmModel.States[startState];
            return new Fsm<TS, TT, TD>(FsmModel);
        }

		public TransitionStateFluent<TS, TT, TD> After(float amount, TimeUnit timeUnit)
		{
			var key = currentTransition;
			if (!AfterEntries.TryGetValue(key, out var l))
			{
				l = new List<Timer<TS>>();
				AfterEntries.Add(key, l);
			}
			l.Add(new Timer<TS>(key.Item2, amount, timeUnit));
			return this;
		}

		public TransitionStateFluent<TS, TT, TD> AfterGlobal(float amount, TimeUnit timeUnit)
		{
			var key = currentGlobalTransition;
			GlobalAfterEntries.Add(new Timer<TS>(key.Item1, amount, timeUnit));
			return this;
		}

		public StateFluent<TS, TT, TD> State(TS state)
        {
            currentState = Tuple.Create(state);
            if (!FsmModel.States.ContainsKey(state))
            {
                stateModels[currentState] = new StateModel<TS, TT, TD>(state);
                FsmModel.States[state] = new State<TS, TT, TD>(stateModels[currentState]);
            }
            return this;
        }

        public StateFluent<TS, TT, TD> OnEnter(Action<StateChangeArgs<TS, TT, TD>> enter)
        {
            stateModels[currentState].AddEnteredHandler(enter);
            return this;
        }

        public StateFluent<TS, TT, TD> OnExit(Action<StateChangeArgs<TS, TT, TD>> exit)
        {
            stateModels[currentState].AddExitedHandler(exit);
            return this;
        }

        public StateFluent<TS, TT, TD> Update(Action<UpdateArgs<TS, TT, TD>> update)
        {
            stateModels[currentState].AddUpdatedHandler(update);
            return this;
        }

        public GlobalTransitionFluent<TS, TT, TD> GlobalTransitionTo(TS state)
        {
            currentGlobalTransition = Tuple.Create(state);
			if (globalTransitionModels.ContainsKey(currentGlobalTransition)) return this;

			globalTransitionModels[currentGlobalTransition] = new TransitionModel<TS, TT>(startState, state);
			FsmModel.GlobalTransitions[state] =
				new Transition<TS, TT, TD>(globalTransitionModels[currentGlobalTransition]);
			return this;
        }

        public GlobalTransitionBuilderFluent<TS, TT, TD> OnGlobal(TT trigger)
        {
            globalTransitionModels[currentGlobalTransition].Triggers.Add(trigger);
            return this;
        }

        public GlobalTransitionBuilderFluent<TS, TT, TD> IfGlobal(
            Func<IfArgs<TS>, bool> condition)
        {
            globalTransitionModels[currentGlobalTransition].Conditions.Add(condition);
            return this;
        }

        public TransitionFluent<TS, TT, TD> TransitionTo(TS state)
        {
            currentTransition = Tuple.Create(currentState.Item1, state);
            if (!transitionModels.ContainsKey(currentTransition))
            {
                transitionModels[currentTransition] = new TransitionModel<TS, TT>(currentState.Item1,
                    state);
                stateModels[currentState].Transitions[state] =
                    new Transition<TS, TT, TD>(transitionModels[currentTransition]);
            }
            return this;
        }

        public TransitionStateFluent<TS, TT, TD> On(TT trigger)
        {
            transitionModels[currentTransition].Triggers.Add(trigger);
            return this;
        }

        public TransitionStateFluent<TS, TT, TD> If(Func<IfArgs<TS>, bool> condition)
        {
            transitionModels[currentTransition].Conditions.Add(condition);
            return this;
        }

        public StateFluent<TS, TT, TD> ClearsStack()
        {
            FsmModel.States[currentState.Item1].ClearStack = true;
            return this;
        }

        public BuilderFluent<TS, TT, TD> EnableStack()
        {
            FsmModel.StackEnabled = true;
            return this;
        }

        public TransitionFluent<TS, TT, TD> PopTransition()
        {
            TransitionTo(default(TS));
            transitionModels[currentTransition].Pop = true;
            return this;
        }
    }
}