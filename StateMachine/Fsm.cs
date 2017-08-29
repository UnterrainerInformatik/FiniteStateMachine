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
    public class Fsm<TState, TTrigger, TData> : Updatable<TData>
    {
        private FsmModel<TState, TTrigger, TData> Model { get; set; } = new FsmModel<TState, TTrigger, TData>();

        public State<TState, TTrigger, TData> Current => Model.Current;
        public Stack<State<TState, TTrigger, TData>> Stack => Model.Stack;

        /// <exception cref="FsmBuilderException">When the model is null</exception>
        public Fsm(FsmModel<TState, TTrigger, TData> model)
        {
            if (model == null) throw FsmBuilderException.ModelCannotBeNull();

            Model = model;
            if (Model.StackEnabled && !model.Current.ClearStack)
            {
                Model.Stack.Push(model.Current);
            }
        }

        /// <exception cref="FsmBuilderException">When the initial state is null</exception>
        public Fsm(State<TState, TTrigger, TData> current, bool stackEnabled = false)
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
        /// Gets you a builder for a Finite-State-Machine (FSM).
        /// </summary>
        /// <param name="startState">The start state's key.</param>
        /// <returns></returns>
        public static BuilderFluent<TState, TTrigger, TData> Builder(TState startState)
        {
            return new FluentImplementation<TState, TTrigger, TData>(startState);
        }

        /// <exception cref="FsmBuilderException">When the handler is null</exception>
        public Fsm<TState, TTrigger, TData> AddStateChangeHandler(
            EventHandler<StateChangeArgs<TState, TTrigger, TData>> e)
        {
            if (e == null) throw FsmBuilderException.HandlerCannotBeNull();

            Model.StateChanged += e;
            return this;
        }

        /// <exception cref="FsmBuilderException">When the state is null or the state has already been added before</exception>
        public Fsm<TState, TTrigger, TData> Add(State<TState, TTrigger, TData> state)
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
        public Fsm<TState, TTrigger, TData> Add(Transition<TState, TTrigger, TData> t)
        {
            if (t == null) throw FsmBuilderException.TransitionCannotBeNull();

            Model.GlobalTransitions.Add(t.Target, t);
            return this;
        }

        public void TransitionTo(TState state, bool isPop = false)
        {
            State<TState, TTrigger, TData> s;
            if (Model.States.TryGetValue(state, out s))
            {
                DoTransition(state, default(TTrigger), isPop);
            }
        }
        
        private void DoTransition(TState state, TTrigger input, bool isPop)
        {
            if (state == null || input == null) return;

            State<TState, TTrigger, TData> old = Model.Current;
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
                StateChangeArgs<TState, TTrigger, TData> args =
                    new StateChangeArgs<TState, TTrigger, TData>(this, old, Model.Current, input);
                old.RaiseExited(args);
                Model.Current.RaiseEntered(args);
                Model.RaiseStateChanged(args);
            }
        }

        public void Trigger(TTrigger input)
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

            Transition<TState, TTrigger, TData> t = Model.Current.Process(input);
            if (t != null)
            {
                DoTransition(t.Target, input, t.Pop);
            }
        }

        public void Update(TData data)
        {
            Model.Current.RaiseUpdated(new UpdateArgs<TState, TTrigger, TData>(this, Current, data));
        }
    }
}