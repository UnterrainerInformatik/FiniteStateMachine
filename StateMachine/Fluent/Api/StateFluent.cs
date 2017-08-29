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
using JetBrains.Annotations;
using StateMachine.Events;

namespace StateMachine.Fluent.Api
{
    [PublicAPI]
    public interface StateFluent<TState, TTrigger, TData> : BuilderFluent<TState, TTrigger, TData>
    {
        /// <summary>
        ///     Adds a new transition to the state, you currently describe.
        /// </summary>
        /// <param name="state">The state the transition will lead to.</param>
        TransitionFluent<TState, TTrigger, TData> TransitionTo(TState state);

        /// <summary>
        ///     Adding a transition that, being triggered, will result in the last state on the stack being popped and set to be
        ///     the current one.<br />
        ///     For this to work 'EnableStack' has to be set (this machine has to be a Stack-Based-FSM (SBFSM)).
        /// </summary>
        TransitionFluent<TState, TTrigger, TData> PopTransition();

        /// <summary>
        ///     Called when the state, you currently describe, is entered.
        /// </summary>
        /// <param name="stateChangeArgs">The state change arguments.</param>
        StateFluent<TState, TTrigger, TData> OnEnter(Action<StateChangeArgs<TState, TTrigger, TData>> stateChangeArgs);

        /// <summary>
        ///     Called when the state, you currently describe, is exited.
        /// </summary>
        /// <param name="stateChangeArgs">The state change arguments.</param>
        StateFluent<TState, TTrigger, TData> OnExit(Action<StateChangeArgs<TState, TTrigger, TData>> stateChangeArgs);

        /// <summary>
        ///     Called when the FSM's 'Update(TData)' method is called and the state, you currently describe, is active.
        /// </summary>
        /// <param name="updateArgs">The update arguments.</param>
        StateFluent<TState, TTrigger, TData> Update(Action<UpdateArgs<TState, TTrigger, TData>> updateArgs);

        /// <summary>
        ///     Clears the stack when the state, you currently describe, is entered.
        /// </summary>
        StateFluent<TState, TTrigger, TData> ClearsStack();
    }
}