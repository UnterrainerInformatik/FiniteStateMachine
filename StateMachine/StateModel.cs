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
    public class StateModel<TState, TTrigger, TData>
    {
        public event EventHandler<StateChangeArgs<TState, TTrigger, TData>> Entered;
        public event EventHandler<StateChangeArgs<TState, TTrigger, TData>> Exited;
        public event EventHandler<UpdateArgs<TState, TTrigger, TData>> Updated;

        public TState Identifier { get; private set; }
        public bool EndState { get; set; }
        public bool ClearStack { get; set; }

        public Dictionary<TState, Transition<TState, TTrigger, TData>> Transitions { get; } =
            new Dictionary<TState, Transition<TState, TTrigger, TData>>();

        public StateModel(TState identifier)
        {
            Identifier = identifier;
        }

        /// <exception cref="FsmBuilderException">When the handler is null</exception>
        public void AddEnteredHandler(
            EventHandler<StateChangeArgs<TState, TTrigger, TData>> e)
        {
            if (e == null) throw FsmBuilderException.HandlerCannotBeNull();

            Entered += e;
        }

        public void RaiseEntered(StateChangeArgs<TState, TTrigger, TData> e)
        {
            Entered?.Invoke(this, e);
        }

        /// <exception cref="FsmBuilderException">When the handler is null</exception>
        public void AddExitedHandler(
            EventHandler<StateChangeArgs<TState, TTrigger, TData>> e)
        {
            if (e == null) throw FsmBuilderException.HandlerCannotBeNull();

            Exited += e;
        }

        public void RaiseExited(StateChangeArgs<TState, TTrigger, TData> e)
        {
            Exited?.Invoke(this, e);
        }

        /// <exception cref="FsmBuilderException">When the handler is null</exception>
        public void AddUpdatedHandler(
            EventHandler<UpdateArgs<TState, TTrigger, TData>> e)
        {
            if (e == null) throw FsmBuilderException.HandlerCannotBeNull();

            Updated += e;
        }

        public void RaiseUpdated(UpdateArgs<TState, TTrigger, TData> data)
        {
            Updated?.Invoke(this, data);
        }
    }
}