using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oika.Apps.CHaserGuiClient.MVVM
{
    public class CommonCommand : ICommand
    {
        readonly Action _action;

        public event EventHandler CanExecuteChanged;

        #region コンストラクタ

        public CommonCommand(Action executeAction)
        {
            this._action = executeAction;
        }

        #endregion

        #region ICommandの明示的実装

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            this._action();
        }

        #endregion

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        public void Execute()
        {
            (this as ICommand).Execute(null);
        }
    }
}
