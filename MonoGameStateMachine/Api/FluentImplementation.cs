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
using Microsoft.Xna.Framework;
using StateMachine.Events;

namespace MonoGameStateMachine.Api
{
    public class FluentImplementation<TS, TT> : StateMachine.Fluent.Api.FluentImplementation<TS, TT, GameTime>,
        TransitionStateFluent<TS, TT, GameTime>, GlobalTransitionBuilderFluent<TS, TT, GameTime>
    {
        public Dictionary<Tuple<TS, TS>, List<Timer<TS>>> AfterEntries { get; set; } = new Dictionary<Tuple<TS, TS>, List<Timer<TS>>>();
        public List<Timer<TS>> GlobalAfterEntries { get; set; } = new List<Timer<TS>>();

        public FluentImplementation(TS startState) : base(startState)
        {
        }

        public new Fsm<TS, TT> Build()
        {
            base.Build();
            var m = new Fsm<TS, TT>(FsmModel);
            m.AfterEntries = AfterEntries;
            m.GlobalAfterEntries = GlobalAfterEntries;
            return m;
        }

        public TransitionStateFluent<TS, TT, GameTime> After(float amount, TimeUnit timeUnit)
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

        public TransitionStateFluent<TS, TT, GameTime> AfterGlobal(float amount, TimeUnit timeUnit)
        {
            var key = currentGlobalTransition;
            GlobalAfterEntries.Add(new Timer<TS>(key.Item1, amount, timeUnit));
            return this;
        }

        public new StateFluent<TS, TT, GameTime> State(TS state)
        {
            base.State(state);
            return this;
        }

        public new TransitionFluent<TS, TT, GameTime> TransitionTo(TS state)
        {
            base.TransitionTo(state);
            return this;
        }

        public new TransitionFluent<TS, TT, GameTime> PopTransition()
        {
            base.PopTransition();
            return this;
        }

        public new TransitionStateFluent<TS, TT, GameTime> On(TT trigger)
        {
            base.On(trigger);
            return this;
        }

        public new TransitionStateFluent<TS, TT, GameTime> If(Func<IfArgs<TS>, bool> condition)
        {
            base.If(condition);
            return this;
        }

        public new StateFluent<TS, TT, GameTime> OnEnter(
            Action<StateChangeArgs<TS, TT, GameTime>> stateChangeArgs)
        {
            base.OnEnter(stateChangeArgs);
            return this;
        }

        public new StateFluent<TS, TT, GameTime> OnExit(
            Action<StateChangeArgs<TS, TT, GameTime>> stateChangeArgs)
        {
            base.OnExit(stateChangeArgs);
            return this;
        }

        public new StateFluent<TS, TT, GameTime> Update(Action<UpdateArgs<TS, TT, GameTime>> updateArgs)
        {
            base.Update(updateArgs);
            return this;
        }

        public new StateFluent<TS, TT, GameTime> ClearsStack()
        {
            base.ClearsStack();
            return this;
        }

        public new BuilderFluent<TS, TT, GameTime> EnableStack()
        {
            base.EnableStack();
            return this;
        }

        public new GlobalTransitionFluent<TS, TT, GameTime> GlobalTransitionTo(TS state)
        {
            base.GlobalTransitionTo(state);
            return this;
        }

        public new GlobalTransitionBuilderFluent<TS, TT, GameTime> OnGlobal(TT trigger)
        {
            base.OnGlobal(trigger);
            return this;
        }

        public new GlobalTransitionBuilderFluent<TS, TT, GameTime> IfGlobal(
            Func<IfArgs<TS>, bool> condition)
        {
            base.IfGlobal(condition);
            return this;
        }
    }
}