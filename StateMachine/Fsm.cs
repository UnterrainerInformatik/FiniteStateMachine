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

namespace StateMachine
{
    [PublicAPI]
    public class Fsm<TState, TTrigger, TGameTime> : Updatable<TGameTime>
    {
        public event EventHandler<StateChangeArgs<TState, TTrigger, TGameTime>> StateChanged;

        public State<TState, TTrigger, TGameTime> Current { get; set; }

        public Stack<State<TState, TTrigger, TGameTime>> Stack { get; } =
            new Stack<State<TState, TTrigger, TGameTime>>();

        public Fsm(State<TState, TTrigger, TGameTime> current)
        {
            Current = current;
            if (!current.ClearStack)
            {
                Stack.Push(current);
            }
        }

        public Fsm<TState, TTrigger, TGameTime> AddStateChangeHandler(
            EventHandler<StateChangeArgs<TState, TTrigger, TGameTime>> e)
        {
            StateChanged += e;
            return this;
        }

        public void Process(TTrigger input, TGameTime data)
        {
            State<TState, TTrigger, TGameTime> old = Current;
            Transition<TState, TTrigger, TGameTime> t = Current.Process(input, data);

            if (t.Pop)
            {
                Stack.Pop();
                Current = Stack.Peek();
            }
            else
            {
                Current = t.Target;
                Stack.Push(Current);
            }

            if (Current.ClearStack)
            {
                Stack.Clear();
            }

            if (!Current.Equals(old))
            {
                StateChangeArgs<TState, TTrigger, TGameTime> args =
                    new StateChangeArgs<TState, TTrigger, TGameTime>(this, old, Current, input, data);
                old.RaiseLeft(args);
                Current.RaiseEntered(args);
                StateChanged?.Invoke(this, args);
            }
        }

        public void Update(TGameTime gameTime)
        {
            Current.Update(gameTime);
        }
    }
}