using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Fluent.Api
{
    interface GlobalTransitionFluent<STATE, TRIGGER>
    {
        GlobalTransitionBuilderFluent<STATE, TRIGGER> On(TRIGGER trigger);

        GlobalTransitionBuilderFluent<STATE, TRIGGER> If(Func<bool> condition);
    }
}
