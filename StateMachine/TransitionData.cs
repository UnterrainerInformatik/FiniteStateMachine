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

namespace StateMachine
{
    class TransitionData<TState, TTrigger, TData>
    {
        public State<TState, TTrigger, TData> State { get; private set; }
        public TTrigger Input { get; private set; }
        public bool IsGlobal { get; private set; }
        public bool IsPop { get; private set; }

        public static TransitionData<TState, TTrigger, TData> CreateTransitionTo(State<TState, TTrigger, TData> state,
            TTrigger input, bool isPop = false)
        {
            var r = new TransitionData<TState, TTrigger, TData>();
            r.IsGlobal = false;
            r.State = state;
            r.Input = input;
            r.IsPop = isPop;
            return r;
        }

        public static TransitionData<TState, TTrigger, TData> CreateGlobalTo(State<TState, TTrigger, TData> state, bool isPop = false)
        {
            var r = new TransitionData<TState, TTrigger, TData>();
            r.IsGlobal = true;
            r.IsPop = isPop;
            r.State = state;
            return r;
        }
    }
}