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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using StateMachine.Fluent;

namespace StateMachine
{
    [PublicAPI]
    public class Transition<TState, TTrigger, TGameTime>
    {
        private TransitionFluent<TState, TTrigger, TGameTime> FluentInterface { get; }

        public List<TTrigger> Triggers { get; set; } = new List<TTrigger>();
        public State<TState, TTrigger, TGameTime> Source { get; set; }
        public State<TState, TTrigger, TGameTime> Target { get; set; }
        public bool Pop { get; set; }

        /// <summary>
        ///     Returns a fluent setter object that allows you to set all the values of the object more conveniently without
        ///     re-creating the instance.
        /// </summary>
        /// <returns>A fluent setter object.</returns>
        public TransitionFluent<TState, TTrigger, TGameTime> Set()
        {
            return FluentInterface;
        }

        public Transition()
        {
            FluentInterface = new TransitionFluent<TState, TTrigger, TGameTime>(this);
        }

        public Transition(Collection<TTrigger> triggers, State<TState, TTrigger, TGameTime> target) : this()
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Triggers.AddRange(triggers);
            Target = target;
        }

        public Transition(Collection<TTrigger> triggers, State<TState, TTrigger, TGameTime> target, bool isPop)
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Triggers.AddRange(triggers);
            Target = target;
            Pop = isPop;
        }

        public Transition(TTrigger trigger, State<TState, TTrigger, TGameTime> target) : this()
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Triggers.Add(trigger);
            Target = target;
        }

        public Transition(TTrigger trigger, State<TState, TTrigger, TGameTime> target, bool isPop)
        {
            if (target == null) throw FsmBuilderException.TargetStateCannotBeNull();

            Triggers.Add(trigger);
            Target = target;
            Pop = isPop;
        }

        public void Add(TTrigger trigger)
        {
            if (Triggers.Contains(trigger)) throw FsmBuilderException.TriggerAlreadyDeclared(trigger);

            Triggers.Add(trigger);
        }

        public bool Process(State<TState, TTrigger, TGameTime> from, TTrigger input)
        {
            return Triggers.Contains(input);
        }

        public override string ToString()
        {
            return $"{Source}-({Triggers})->{Target}";
        }
    }
}