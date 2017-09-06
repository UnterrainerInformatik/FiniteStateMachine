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
    public class FsmModel<TS, TT, TD>
    {
        public event EventHandler<StateChangeArgs<TS, TT, TD>> StateChanged;

        public State<TS, TT, TD> Current { get; set; }

        public Stack<State<TS, TT, TD>> Stack { get; } = new Stack<State<TS, TT, TD>>();
        public bool StackEnabled { get; set; }

        public Dictionary<TS, State<TS, TT, TD>> States { get; } = new Dictionary<TS, State<TS, TT, TD>>();

        public Dictionary<TS, Transition<TS, TT, TD>> GlobalTransitions { get; } =
            new Dictionary<TS, Transition<TS, TT, TD>>();

        public void AddStateChangedHandler(
            EventHandler<StateChangeArgs<TS, TT, TD>> e)
        {
            if (e == null) throw FsmBuilderException.HandlerCannotBeNull();
            StateChanged += e;
        }

        public void RaiseStateChanged(StateChangeArgs<TS, TT, TD> e) => StateChanged?.Invoke(this, e);
    }
}