using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Fluent.Api
{
    interface TransitionFluent<STATE, TRIGGER>
    {
        TransitionStateFluent<STATE, TRIGGER> On(TRIGGER trigger);
        TransitionStateFluent<STATE, TRIGGER> If(Func<bool> condition);
    }
}
