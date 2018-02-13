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

namespace StateMachine
{
    public class State<TS, TT>
    {
        public StateModel<TS, TT> Model { get; set; }

        public TS Identifier => Model.Identifier;
		public Dictionary<TS, Transition<TS, TT>> Transitions => Model.Transitions;
		public bool EndState => Model.EndState;

        public bool ClearStack
        {
            get => Model.ClearStack;
			set => Model.ClearStack = value;
		}

        public void RaiseUpdated(UpdateArgs<TS, TT> args) => Model.RaiseUpdated(args);
        public void RaiseEntered(StateChangeArgs<TS, TT> args) => Model.RaiseEntered(args);
        public void RaiseExited(StateChangeArgs<TS, TT> args) => Model.RaiseExited(args);

        public State(StateModel<TS, TT> model)
        {
            Model = model;
        }

        public State(TS identifier) : this(new StateModel<TS, TT>(identifier))
        {
        }

        /// <exception cref="FsmBuilderException"> When the transition is null</exception>
        public State<TS, TT> Add(Transition<TS, TT> t)
        {
            if (t == null) throw FsmBuilderException.TransitionCannotBeNull();

            t.Source = Identifier;
            Model.Transitions.Add(t.Target, t);
            return this;
        }

        public State<TS, TT> AddTransisionOn(TT trigger, TS target)
            => Add(new Transition<TS, TT>(trigger, Identifier, target));

        public State<TS, TT> AddPopTransisionOn(TT trigger) => Add(new Transition<TS, TT>(trigger, Identifier));

        public Transition<TS, TT> Process(TT input)
        {
            foreach (var t in Model.Transitions.Values)
            {
                if (t.Process(this, input))
                {
                    return t;
                }
            }
            return null;
        }

        public override string ToString() => Model.Identifier.ToString();
    }
}