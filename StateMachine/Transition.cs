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

using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace StateMachine
{
    [PublicAPI]
    public class Transition<TState, TTrigger, TData>
    {
        private TransitionModel<TState, TTrigger, TData> Model { get; set; } =
            new TransitionModel<TState, TTrigger, TData>();

        public State<TState, TTrigger, TData> Target => Model.Target;

        public bool Pop => Model.Pop;

        public State<TState, TTrigger, TData> Source
        {
            get { return Model.Source; }
            set { Model.Source = value; }
        }

        public Transition(TransitionModel<TState, TTrigger, TData> model)
        {
            Model = model;
        }

        public Transition()
        {
        }

        /// <exception cref="FsmBuilderException">When target is null</exception>
        public Transition(Collection<TTrigger> triggers, State<TState, TTrigger, TData> target) : this()
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Model.Triggers.AddRange(triggers);
            Model.Target = target;
        }

        /// <exception cref="FsmBuilderException">When target is null</exception>
        public Transition(Collection<TTrigger> triggers, State<TState, TTrigger, TData> target, bool isPop)
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Model.Triggers.AddRange(triggers);
            Model.Target = target;
            Model.Pop = isPop;
        }

        /// <exception cref="FsmBuilderException">When target is null</exception>
        public Transition(TTrigger trigger, State<TState, TTrigger, TData> target) : this()
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Model.Triggers.Add(trigger);
            Model.Target = target;
        }

        /// <exception cref="FsmBuilderException">When target is null</exception>
        public Transition(TTrigger trigger, State<TState, TTrigger, TData> target, bool isPop)
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Model.Triggers.Add(trigger);
            Model.Target = target;
            Model.Pop = isPop;
        }

        /// <exception cref="FsmBuilderException">When trigger has been declared before</exception>
        public void Add(TTrigger trigger)
        {
            if (Model.Triggers.Contains(trigger)) throw FsmBuilderException.TriggerAlreadyDeclared(trigger);

            Model.Triggers.Add(trigger);
        }

        public bool Process(State<TState, TTrigger, TData> from, TTrigger input)
        {
            return Model.Triggers.Contains(input) &&
                   Model.Conditions.TrueForAll(x => x(from.Identifier, Model.Target.Identifier, input));
        }

        public override string ToString()
        {
            return $"{Model.Source}-({Model.Triggers})->{Model.Target}";
        }
    }
}