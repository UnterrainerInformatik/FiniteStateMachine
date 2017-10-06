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
using Microsoft.Xna.Framework;
using MonoGameStateMachine.Api;
using StateMachine;
using StateMachine.Events;

namespace MonoGameStateMachine
{
    [PublicAPI]
    public class Fsm<TS, TT> : Fsm<TS, TT, GameTime>
    {
        private GameTime gt = new GameTime();

        public Dictionary<Tuple<TS, TS>, List<Timer<TS>>> AfterEntries { get; set; } = new Dictionary<Tuple<TS, TS>, List<Timer<TS>>>();
        public List<Timer<TS>> GlobalAfterEntries { get; set; } = new List<Timer<TS>>();

        public Fsm(FsmModel<TS, TT, GameTime> model) : base(model)
        {
        }

        public Fsm(State<TS, TT, GameTime> current, bool stackEnabled = false) : base(current, stackEnabled)
        {
        }

        /// <summary>
        ///     Gets you a builder for a Finite-State-Machine (FSM).
        /// </summary>
        /// <param name="startState">The start state's key.</param>
        /// <returns></returns>
        public new static BuilderFluent<TS, TT, GameTime> Builder(TS startState)
        {
            return new FluentImplementation<TS, TT>(startState);
        }

        private void ResetCurrentAfterEntries()
        {
            foreach (var k in Current.Model.Transitions.Keys)
            {
                List<Timer<TS>> currentAfterEntries;
                if (AfterEntries.TryGetValue(new Tuple<TS, TS>(Current.Identifier, k), out currentAfterEntries))
                {
                    for (int i = 0; i < currentAfterEntries.Count; i++)
                    {
                        Timer<TS> e = currentAfterEntries[i];
                        e.Reset();
                        currentAfterEntries[i] = e;
                    }
                }
            }
        }

        protected override void Entered(StateChangeArgs<TS, TT, GameTime> args)
        {
            ResetCurrentAfterEntries();
            base.Entered(args);
        }

        public new void Update(GameTime gameTime)
        {
            // After-entries on transitions.
            foreach (var k in Current.Model.Transitions.Keys)
            {
                List<Timer<TS>> currentAfterEntries;
                if (AfterEntries.TryGetValue(new Tuple<TS, TS>(Current.Identifier, k), out currentAfterEntries))
                {
                    if (CheckAfterEntries(currentAfterEntries, Current.Model.Transitions, gameTime))
                    {
                        return;
                    }
                }
            }

            // Global after-entries.
            if (CheckAfterEntries(GlobalAfterEntries, Model.GlobalTransitions, gameTime))
            {
                return;
            }
            
            Model.Current.RaiseUpdated(new UpdateArgs<TS, TT, GameTime>(this, Current, gameTime));
        }
        
        private bool CheckAfterEntries(List<Timer<TS>> afterEntries,
            Dictionary<TS, Transition<TS, TT, GameTime>> transitions, GameTime g)
        {
            for (int i = 0; i < afterEntries.Count; i++)
            {
                Timer<TS> e = afterEntries[i];
                Transition<TS, TT, GameTime> t;
                if (transitions.TryGetValue(e.Target, out t))
                {
                    if (t.ConditionsMet(Current.Identifier))
                    {
                        double timerMax = e.Time;
                        double? r = e.Tick(g.ElapsedGameTime.TotalMilliseconds);
                        afterEntries[i] = e;
                        if (r.HasValue)
                        {
                            // It triggered.
                            DoTransition(e.Target, default(TT), t.Pop);
                            gt.IsRunningSlowly = g.IsRunningSlowly;
                            gt.TotalGameTime = g.TotalGameTime.Subtract(TimeSpan.FromMilliseconds(timerMax));
                            gt.ElapsedGameTime =
                                g.ElapsedGameTime.Subtract(TimeSpan.FromMilliseconds(timerMax));
                            Update(gt);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}