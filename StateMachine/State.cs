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

using JetBrains.Annotations;
using StateMachine.Events;

namespace StateMachine
{
    [PublicAPI]
    public class State<TState, TTrigger, TData>
    {
        private StateModel<TState, TTrigger, TData> Model { get; set; } = new StateModel<TState, TTrigger, TData>();

        public TState Identifier => Model.Identifier;

        public bool ClearStack
        {
            get { return Model.ClearStack; }
            set { Model.ClearStack = value; }
        }

        public bool EndState
        {
            get { return Model.EndState; }
            set { Model.EndState = value; }
        }

        public void RaiseUpdated(UpdateArgs<TState, TTrigger, TData> args) => Model.RaiseUpdated(args);
        public void RaiseEntered(StateChangeArgs<TState, TTrigger, TData> args) => Model.RaiseEntered(args);
        public void RaiseExited(StateChangeArgs<TState, TTrigger, TData> args) => Model.RaiseExited(args);

        public State(StateModel<TState, TTrigger, TData> model)
        {
            Model = model;
        }

        public State(TState identifier)
        {
            Model.Identifier = identifier;
        }

        /// <exception cref="FsmBuilderException"> When the transition is null</exception>
        public State<TState, TTrigger, TData> Add(Transition<TState, TTrigger, TData> t)
        {
            if (t == null) throw FsmBuilderException.TransitionCannotBeNull();

            t.Source = this;
            Model.Transitions.Add(t.Target.Identifier, t);
            return this;
        }

        public Transition<TState, TTrigger, TData> Process(TTrigger input)
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

        public override string ToString()
        {
            return Model.Identifier.ToString();
        }
    }
}