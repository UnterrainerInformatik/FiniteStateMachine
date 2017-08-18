using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine.Fluent.Api
{
    interface BuilderFluent<STATE, TRIGGER>
    {
        GlobalTransitionFluent<STATE, TRIGGER> GlobalTransitionTo(STATE state);

        StateFluent<STATE, TRIGGER> State(STATE state);

        Machine<STATE> Build();
    }
}
