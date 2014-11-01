using Oika.Apps.CHaserGuiClient.Views;
using Oika.Libs.MeLogg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oika.Apps.CHaserGuiClient.Line
{
    class LineManager : ILineManager
    {
        LineCommunicator communicator;

        readonly ILogger logger = new TextBoxLogger();

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static LineManager()
        {
            var logDir = Path.Combine(App.Current.StartUpPath, "dump");
            LogSettings.Update(new LogSetting.Builder("line", logDir) { MaxFileCount = 10 }.Build());
        }


        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;

        private void raiseConnectionChanged(ConnectionState state)
        {
            var h = ConnectionChanged;
            if (h != null) h(this, new ConnectionChangedEventArgs(state));
        }

        public void Connect(string host, int port, string teamName)
        {
            if (communicator != null) 
            {
                logger.Warn("接続済みです");
                return;
            }

            try
            {
                logger.Info("接続開始：host={0},port={1},teamName={2}", host, port, teamName);
                communicator = new LineCommunicator(host, port, new Logger("line", "sent"), new Logger("line", "recv"));
                communicator.Connect(teamName);
                raiseConnectionChanged(ConnectionState.Connected);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.ToString());
                raiseConnectionChanged(ConnectionState.AbortedWithError);
            }
        }

        public void GetReady(Action<ResponseData> callback)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseData res = null;

                try
                {
                    res = communicator.GetReady();
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex.ToString());
                }

                callback.Invoke(res);
            });
        }

        public void Call(MethodKind method, DirectionKind direction, Action<ResponseData> callback)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseData res = null;

                try
                {
                    res = communicator.Call(method, direction);
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex.ToString());
                }

                callback.Invoke(res);

            });
        }

        public void Close()
        {
            var comm = communicator;
            if (comm == null)
            {
                logger.Warn("接続破棄済みです。");
                return;
            }
            comm.Dispose();
            comm = null;
        }

    }
}
