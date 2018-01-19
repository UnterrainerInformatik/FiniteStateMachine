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

namespace StateMachine
{
    public class FsmBuilderException : Exception
    {
        private FsmBuilderException(string message) : base(message)
        {
        }

        public static FsmBuilderException StateCanOnlyBeAddedOnce<TS, TT, TD>(
            State<TS, TT, TD> state)
            => new FsmBuilderException($"A state [{state.Identifier}] has already been added. You can only one add" +
                                       " a state with a unique identifier once.");

        public static FsmBuilderException TargetStateCannotBeNull()
            => new FsmBuilderException("The target of a transition cannot be null.");

        public static FsmBuilderException TransitionCannotBeNull()
            => new FsmBuilderException("The transition cannot be null.");

        public static FsmBuilderException StateCannotBeNull() => new FsmBuilderException("The state cannot be null.");

        public static FsmBuilderException StartStateCannotBeNull()
            => new FsmBuilderException("The start state cannot be null.");

        public static FsmBuilderException ModelCannotBeNull() => new FsmBuilderException("The model cannot be null.");

        public static FsmBuilderException TriggerAlreadyDeclared<TT>(TT trigger)
            => new FsmBuilderException($"The transition already contains the trigger [{trigger}]");

        public static FsmBuilderException HandlerCannotBeNull()
            => new FsmBuilderException("The handler you want to add cannot be null.");
    }
}