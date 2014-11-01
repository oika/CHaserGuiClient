using Oika.Apps.CHaserGuiClient.ViewModels;
using Oika.Apps.CHaserGuiClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Oika.Apps.CHaserGuiClient.Line
{
    /// <summary>
    /// 通信機能を置き換えるデバッグ用のモッククラスです。
    /// </summary>
    class MockLineManager : ILineManager
    {
        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;


        public void Connect(string host, int port, string teamName)
        {
            var h = ConnectionChanged;
            if (h != null) h(this, new ConnectionChangedEventArgs(ConnectionState.Connected));
        }

        public void GetReady(Action<ResponseData> callback)
        {
            var res = showInputDialog();

            Task.Factory.StartNew(() =>
            {
                callback(res);
            });
        }

        public void Call(MethodKind method, DirectionKind direction, Action<ResponseData> callback)
        {
            var res = showInputDialog();

            Task.Factory.StartNew(() =>
            {
                callback(res);
            });
        }

        public void Close()
        {
            var h = ConnectionChanged;
            if (h != null) h(this, new ConnectionChangedEventArgs(ConnectionState.Disconnected));
        }


        private ResponseData showInputDialog()
        {
            //UIから入力を受け取る
            var dispatcher = App.Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                return _invokeShowInputDialog();
            }
            return dispatcher.Invoke(new Func<ResponseData>(_invokeShowInputDialog)) as ResponseData;
        }

        private ResponseData _invokeShowInputDialog()
        {
            var wnd = new Window
            {
                Width = 200,
                Height = 80,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = App.Current.MainWindow,
                Title = "",
                WindowStyle = WindowStyle.ToolWindow
            };

            var defaultValue = "1" + new string(Enumerable.Repeat('0', ResponseData.TotalLength - 1).ToArray());
            var txt = new TextBox() { Text = defaultValue, FontSize = 24, MaxLength = ResponseData.TotalLength };
            txt.KeyDown += (s, e) =>
            {
                if (e.Key != System.Windows.Input.Key.Enter) return;

                if (!isValidInput(txt.Text))
                {
                    txt.Text = defaultValue;
                    return;
                }

                wnd.DialogResult = true;
            };

            wnd.Content = txt;
            wnd.ShowDialog();

            var val = txt.Text;
            if (!isValidInput(val)) val = defaultValue;

            return new ResponseData(val);
        }


        private static bool isValidInput(string input)
        {
            if (input == null) return false;
            if (input.Length != ResponseData.TotalLength) return false;

            if (input[0] != '0' && input[0] != '1') return false;

            var chars = Enum.GetValues(typeof(CellKind)).Cast<CellKind>().Select(c => ((int)c).ToString()[0]);
            var validCharSet = new HashSet<char>(chars);

            return input.Skip(1).All(c => validCharSet.Contains(c));
        }

    }
}
