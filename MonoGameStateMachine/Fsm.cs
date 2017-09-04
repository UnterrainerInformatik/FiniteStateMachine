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
            return new FluentImplementation<TS, TT, GameTime>(startState);
        }
    }

    [PublicAPI]
    public class Fsm<TS, TT, TD> : StateMachine.Fsm<TS, TT, TD>
    {
        public Dictionary<Tuple<TS, TS>, List<Timer>> AfterEntries { get; set; } = new Dictionary<Tuple<TS, TS>, List<Timer>>();
        public Dictionary<Tuple<TS>, List<Timer>> GlobalAfterEntries { get; set; } = new Dictionary<Tuple<TS>, List<Timer>>();

        public Fsm(FsmModel<TS, TT, TD> model) : base(model)
        {
        }

        public Fsm(State<TS, TT, TD> current, bool stackEnabled = false) : base(current, stackEnabled)
        {
        }

        /// <summary>
        ///     Gets you a builder for a Finite-State-Machine (FSM).
        /// </summary>
        /// <param name="startState">The start state's key.</param>
        /// <returns></returns>
        public new static BuilderFluent<TS, TT, DataObject<TD>> Builder(TS startState)
        {
            return new FluentImplementation<TS, TT, DataObject<TD>>(startState);
        }

        public new void Update(TD data)
        {
            List<Timer> l;
            if (!AfterEntries.TryGetValue(Current, out l))
            {
                l = new List<Tuple<float, TimeUnit>>();
                AfterEntries.Add(key, l);
            }
            l.Add(new Tuple<float, TimeUnit>(amount, timeUnit));

            foreach (var e in GlobalAfterEntries)
            {
                var target = e.Key;
                var ts = e.Value;
                foreach (var t in ts)
                {
                    
                }
            }
            if (!GlobalAfterEntries.TryGetValue(key, out l))
            {
                l = new List<Tuple<float, TimeUnit>>();
                GlobalAfterEntries.Add(key, l);
            }

            Model.Current.RaiseUpdated(new UpdateArgs<TS, TT, TD>(this, Current, data));
        }
    }
}