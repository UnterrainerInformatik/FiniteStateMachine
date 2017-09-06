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
using JetBrains.Annotations;
using StateMachine.Events;
using StateMachine.Fluent.Api;

namespace StateMachine
{
    [PublicAPI]
    public class Fsm<TS, TT> : Fsm<TS, TT, float>
    {
        public Fsm(FsmModel<TS, TT, float> model) : base(model)
        {
        }

        public Fsm(State<TS, TT, float> current, bool stackEnabled = false) : base(current, stackEnabled)
        {
        }

        public void Update()
        {
            Model.Current.RaiseUpdated(new UpdateArgs<TS, TT, float>(this, Current, 0f));
        }
    }

    [PublicAPI]
    public class Fsm<TS, TT, TD>
    {
        protected FsmModel<TS, TT, TD> Model { get; set; } = new FsmModel<TS, TT, TD>();

        public State<TS, TT, TD> Current => Model.Current;
        public Stack<State<TS, TT, TD>> Stack => Model.Stack;

        /// <exception cref="FsmBuilderException">When the model is null</exception>
        public Fsm(FsmModel<TS, TT, TD> model)
        {
            if (model == null) throw FsmBuilderException.ModelCannotBeNull();

            Model = model;
            if (Model.StackEnabled && !model.Current.ClearStack)
            {
                Model.Stack.Push(model.Current);
            }
        }

        /// <exception cref="FsmBuilderException">When the initial state is null</exception>
        public Fsm(State<TS, TT, TD> current, bool stackEnabled = false)
        {
            Model.StackEnabled = stackEnabled;
            if (current == null) throw FsmBuilderException.StateCannotBeNull();

            Model.Current = current;
            if (Model.StackEnabled && !current.ClearStack)
            {
                Model.Stack.Push(current);
            }
        }

        /// <summary>
        ///     Gets you a builder for a Finite-State-Machine (FSM).
        /// </summary>
        /// <param name="startState">The start state's key.</param>
        /// <returns></returns>
        public static BuilderFluent<TS, TT, TD> Builder(TS startState)
        {
            return new FluentImplementation<TS, TT, TD>(startState);
        }

        /// <exception cref="FsmBuilderException">When the handler is null</exception>
        public Fsm<TS, TT, TD> AddStateChangeHandler(
            EventHandler<StateChangeArgs<TS, TT, TD>> e)
        {
            if (e == null) throw FsmBuilderException.HandlerCannotBeNull();

            Model.StateChanged += e;
            return this;
        }

        /// <exception cref="FsmBuilderException">When the state is null or the state has already been added before</exception>
        public Fsm<TS, TT, TD> Add(State<TS, TT, TD> state)
        {
            if (state == null) throw FsmBuilderException.StateCannotBeNull();
            if (Model.States.ContainsKey(state.Identifier)) throw FsmBuilderException.StateCanOnlyBeAddedOnce(state);

            Model.States.Add(state.Identifier, state);
            return this;
        }

        /// <exception cref="FsmBuilderException">
        ///     When the transition is null or another transition already leads to the same
        ///     target state
        /// </exception>
        public Fsm<TS, TT, TD> Add(Transition<TS, TT, TD> t)
        {
            if (t == null) throw FsmBuilderException.TransitionCannotBeNull();

            Model.GlobalTransitions.Add(t.Target, t);
            return this;
        }

        public void JumpTo(TS state, bool isPop = false)
        {
            State<TS, TT, TD> s;
            if (Model.States.TryGetValue(state, out s))
            {
                DoTransition(state, default(TT), isPop);
            }
        }

        private void DoTransition(TS state, TT input, bool isPop)
        {
            if (state == null || input == null) return;

            State<TS, TT, TD> old = Model.Current;
            if (Model.StackEnabled && isPop)
            {
                Model.Stack.Pop();
                Model.Current = Model.Stack.Peek();
            }
            else
            {
                Model.Current = Model.States[state];
                if (Model.StackEnabled)
                    Model.Stack.Push(Model.Current);
            }

            if (Model.StackEnabled && Model.Current.ClearStack)
            {
                Model.Stack.Clear();
            }

            if (!Model.Current.Equals(old))
            {
                StateChangeArgs<TS, TT, TD> args =
                    new StateChangeArgs<TS, TT, TD>(this, old, Model.Current, input);
                old.RaiseExited(args);
                Model.Current.RaiseEntered(args);
                Model.RaiseStateChanged(args);
            }
        }

        public void Trigger(TT input)
        {
            if (input == null) return;

            foreach (var g in Model.GlobalTransitions.Values)
            {
                if (g.Process(Model.Current, input))
                {
                    DoTransition(g.Target, input, g.Pop);
                    return;
                }
            }

            Transition<TS, TT, TD> t = Model.Current.Process(input);
            if (t != null)
            {
                DoTransition(t.Target, input, t.Pop);
            }
        }

        public void Update(TD data)
        {
            Model.Current.RaiseUpdated(new UpdateArgs<TS, TT, TD>(this, Current, data));
        }
    }
}