using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oika.Apps.CHaserGuiClient
{
    public enum PhaseKind
    {
        Unknown,
        Unstarted,
        WaitingToConnect,
        WaitingToReady,
        MethodSelection,
        WaitingToResponse,
        GameSet,
    }
}
