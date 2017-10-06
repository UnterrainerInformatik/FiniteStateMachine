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

namespace MonoGameStateMachine.Api
{
    [PublicAPI]
    public interface BuilderFluent<TS, TT, TD>
    {
        /// <summary>
        ///     Enables the stack and turns this Finite-State-Machine (FSM) into a Stack-Based-FSM (SBFSM).<br />
        ///     Beware that you will have to specify "ClearsStack()" on some of the states, as otherwise the stack will grow and
        ///     never be cleared.
        /// </summary>
        BuilderFluent<TS, TT, TD> EnableStack();

        /// <summary>
        ///     Sets a global transition to a state.<br />
        ///     This will generate a transision that will be triggered regardless of the current state's position in the graph and
        ///     regardless of the transitions connected to that state.<p />
        ///     Think of it as a 'catch all' transition.<br />
        ///     Usually you would use such global transitions to reset a graph when ESC is pressed or something like that.
        /// </summary>
        /// <param name="state">The state the global transition should lead to.</param>
        GlobalTransitionFluent<TS, TT, TD> GlobalTransitionTo(TS state);

        /// <summary>
        ///     Generates a new state.<br />
        ///     An FSM can have multiple states connected via transitions.<br />
        ///     Only a single transition is active at any given time and that transition will receive 'Update(TData)' calls and
        ///     will be checked for transitions that may be triggered by an input, which, in turn, would activate the next state.
        /// </summary>
        /// <param name="state">The state.</param>
        StateFluent<TS, TT, TD> State(TS state);

        /// <summary>
        ///     Builds this instance of an FSM (or SBFSM).
        /// </summary>
        Fsm<TS, TT> Build();
    }
}