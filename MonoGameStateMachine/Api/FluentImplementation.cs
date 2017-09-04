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
using StateMachine;
using StateMachine.Events;

namespace MonoGameStateMachine.Api
{
    public class FluentImplementation<TS, TT, TD> :
        StateMachine.Fluent.Api.FluentImplementation<TS, TT, TD>,
        TransitionStateFluent<TS, TT, TD>, GlobalTransitionBuilderFluent<TS, TT, TD>
    {
        protected new Dictionary<Tuple<TS, TS>, TransitionModel<TS, TT, TD>> transitionModels =
            new Dictionary<Tuple<TS, TS>, TransitionModel<TS, TT, TD>>();

        public FluentImplementation(TS startState) : base(startState)
        {
        }

        public new Fsm<TS, TT, TD> Build()
        {
            if (FsmModel.States[startState] == null)
            {
                throw FsmBuilderException.StartStateCannotBeNull();
            }

            FsmModel.Current = FsmModel.States[startState];
            Fsm<TS, TT, TD> fsm = new Fsm<TS, TT, TD>(FsmModel);
            return fsm;
        }

        public TransitionStateFluent<TS, TT, TD> After(float amount, TimeUnit timeUnit)
        {
            throw new NotImplementedException();
            return this;
        }

        public TransitionStateFluent<TS, TT, TD> AfterGlobal(float amount, TimeUnit timeUnit)
        {
            throw new NotImplementedException();
            return this;
        }

        public new StateFluent<TS, TT, TD> State(TS state)
        {
            base.State(state);
            return this;
        }

        public new TransitionFluent<TS, TT, TD> TransitionTo(TS state)
        {
            base.TransitionTo(state);
            return this;
        }

        public new TransitionFluent<TS, TT, TD> PopTransition()
        {
            base.PopTransition();
            return this;
        }

        public new TransitionStateFluent<TS, TT, TD> On(TT trigger)
        {
            base.On(trigger);
            return this;
        }

        public new TransitionStateFluent<TS, TT, TD> If(Func<IfArgs<TS, TT>, bool> condition)
        {
            base.If(condition);
            return this;
        }

        public new StateFluent<TS, TT, TD> OnEnter(
            Action<StateChangeArgs<TS, TT, TD>> stateChangeArgs)
        {
            base.OnEnter(stateChangeArgs);
            return this;
        }

        public new StateFluent<TS, TT, TD> OnExit(
            Action<StateChangeArgs<TS, TT, TD>> stateChangeArgs)
        {
            base.OnExit(stateChangeArgs);
            return this;
        }

        public new StateFluent<TS, TT, TD> Update(Action<UpdateArgs<TS, TT, TD>> updateArgs)
        {
            base.Update(updateArgs);
            return this;
        }

        public new StateFluent<TS, TT, TD> ClearsStack()
        {
            base.ClearsStack();
            return this;
        }

        public new BuilderFluent<TS, TT, TD> EnableStack()
        {
            base.EnableStack();
            return this;
        }

        public new GlobalTransitionFluent<TS, TT, TD> GlobalTransitionTo(TS state)
        {
            base.GlobalTransitionTo(state);
            return this;
        }

        public GlobalTransitionBuilderFluent<TS, TT, TD> OnGlobal(TT trigger)
        {
            base.OnGlobal(trigger);
            return this;
        }

        public GlobalTransitionBuilderFluent<TS, TT, TD> IfGlobal(
            Func<IfArgs<TS, TT>, bool> condition)
        {
            base.IfGlobal(condition);
            return this;
        }
    }
}