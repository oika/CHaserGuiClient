using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oika.Apps.CHaserGuiClient.Line
{
    public interface ILineManager
    {
        event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;

        void Connect(string host, int port, string teamName);

        void GetReady(Action<ResponseData> callback);

        void Call(MethodKind method, DirectionKind direction, Action<ResponseData> callback);

        void Close();
    }
}
