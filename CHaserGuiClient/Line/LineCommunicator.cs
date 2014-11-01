using Oika.Libs.MeLogg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Oika.Apps.CHaserGuiClient.Line
{
    class LineCommunicator : IDisposable
    {
        static readonly Encoding LineEncoding = Encoding.GetEncoding("Shift_JIS");  //わかんないけど

        const string LineBreak = "\r\n";

        readonly string host;
        readonly int port;
        readonly Dumper sentDumper;
        readonly Dumper recvDumper;

        Socket socket;

        public LineCommunicator(string host, int port, ILogger sentDumpLogger, ILogger recvDumpLogger)
        {
            this.host = host;
            this.port = port;
            this.sentDumper = new Dumper(sentDumpLogger);
            this.recvDumper = new Dumper(recvDumpLogger);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string team) 
        {
            socket.Connect(host, port);

            send(team);

            var res = receive(1);
            if (res != "@") throw new InvalidOperationException("受信電文 " + res + " は不正な値です");
        }

        public ResponseData GetReady()
        {
            send("gr");
            var res = receive(ResponseData.TotalLength);
            return new ResponseData(res);
        }

        public ResponseData Call(MethodKind method, DirectionKind direction)
        {
            send(method.ToChar() + direction.ToChar());
            var res = receive(ResponseData.TotalLength);

            send("#");
            
            var resStTern = receive(1);
            if (resStTern != "@") throw new InvalidOperationException("受信電文 " + res + " は不正な値です");

            return new ResponseData(res);
        }


        #region 送受信の実処理

        private void send(string message)
        {
            var bytes = LineEncoding.GetBytes(message + LineBreak);
            socket.Send(bytes);
            sentDumper.Dump(bytes);
        }


        private string receive(int expectedLength)
        {
            expectedLength += LineEncoding.GetByteCount(LineBreak); //改行文の電文長を追加

            var received = new List<byte>();
            var buff = new byte[256];

            while (received.Count < expectedLength)
            {
                var len = socket.Receive(buff);
                if (len == 0) continue;

                received.AddRange(buff.Take(len));
            }

            var bytes = received.ToArray();
            recvDumper.Dump(bytes);

            return LineEncoding.GetString(bytes).TrimEnd(LineBreak.ToCharArray());  //改行を削って返す
        }

        #endregion

        public void Dispose()
        {
            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }
        }
    }
}
