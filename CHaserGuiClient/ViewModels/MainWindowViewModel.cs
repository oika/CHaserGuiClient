using Oika.Apps.CHaserGuiClient.Line;
using Oika.Apps.CHaserGuiClient.MVVM;
using Oika.Apps.CHaserGuiClient.Views;
using Oika.Libs.MeLogg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oika.Apps.CHaserGuiClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        readonly ILogger viewLogger = new TextBoxLogger();

        readonly ILineManager line;

        #region 接続設定プロパティ

        private string _teamName;
        /// <summary>
        /// チーム名を取得または設定します。
        /// </summary>
        public string TeamName
        {
            get
            {
                return _teamName;
            }
            set
            {
                if (_teamName == value) return;
                _teamName = value;
                RaisePropertyChanged<MainWindowViewModel, string>(me => me.TeamName);
            }
        }

        private string _host;
        /// <summary>
        /// 接続先ホストアドレスを取得または設定します。
        /// </summary>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                if (_host == value) return;
                _host = value;
                RaisePropertyChanged<MainWindowViewModel, string>(me => me.Host);
                RaisePropertyChanged<MainWindowViewModel, bool>(me => me.CanStart);
            }
        }

        private int _port = 40000;
        /// <summary>
        /// 接続先ポート番号を取得または設定します。
        /// </summary>
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (_port == value) return;
                _port = value;
                RaisePropertyChanged<MainWindowViewModel, int>(me => me.Port);
                RaisePropertyChanged<MainWindowViewModel, bool>(me => me.CanStart);
            }
        }

        #endregion

        #region フェーズ状態プロパティ

        private PhaseKind _currentPhase;

        public PhaseKind CurrentPhase
        {
            get
            {
                return _currentPhase;
            }
            private set
            {
                if (_currentPhase == value) return;
                _currentPhase = value;

                RaisePropertyChanged<MainWindowViewModel, PhaseKind>(me => me.CurrentPhase);
                RaisePropertyChanged<MainWindowViewModel, bool>(me => me.CanGetReady);
                RaisePropertyChanged<MainWindowViewModel, bool>(me => me.IsGameStarted);
            }
        }

        public bool IsGameStarted
        {
            get
            {
                return CurrentPhase != PhaseKind.Unstarted;
            }
        }

        #endregion

        #region 周辺情報コンテクスト

        private AroundFloorPanelContext _floorContext;

        public AroundFloorPanelContext FloorContext
        {
            get
            {
                return _floorContext;
            }
            private set
            {
                if (object.ReferenceEquals(_floorContext, value)) return;
                _floorContext = value;
                RaisePropertyChanged<MainWindowViewModel, AroundFloorPanelContext>(me => me.FloorContext);
            }
        }

        #endregion

        #region メソッド選択項目

        readonly ReadOnlyCollection<MethodKind> _methodItems;
        /// <summary>
        /// メソッド選択項目一覧を取得します。
        /// </summary>
        public ICollection<MethodKind> MethodItems
        {
            get
            {
                return _methodItems;
            }
        }

        readonly ReadOnlyCollection<DirectionKind> _directionItems;
        /// <summary>
        /// 方向指定項目一覧を取得します。
        /// </summary>
        public ICollection<DirectionKind> DirectionItems
        {
            get
            {
                return _directionItems;
            }
        }

        private MethodKind _selectedMethod;
        /// <summary>
        /// 選択中のメソッド種別を取得または設定します。
        /// </summary>
        public MethodKind SelectedMethod
        {
            get
            {
                return _selectedMethod;
            }
            set
            {
                if (_selectedMethod == value) return;
                _selectedMethod = value;
                RaisePropertyChanged<MainWindowViewModel, MethodKind>(me => me.SelectedMethod);
            }
        }

        private DirectionKind _selectedDirection;
        /// <summary>
        /// 選択中の方向指定項目を取得または設定します。
        /// </summary>
        public DirectionKind SelectedDirection
        {
            get
            {
                return _selectedDirection;
            }
            set
            {
                if (_selectedDirection == value) return;
                _selectedDirection = value;
                RaisePropertyChanged<MainWindowViewModel, DirectionKind>(me => me.SelectedDirection);
            }
        }

        #endregion

        #region コマンドと有効状態

        public ICommand StartCommand { get; private set; }

        public ICommand GetReadyCommand { get; private set; }

        public ICommand CallCommand { get; private set; }

        public bool CanStart
        {
            get
            {
                return !string.IsNullOrEmpty(Host) && Port != 0;
            }
        }

        public bool CanGetReady
        {
            get
            {
                return CurrentPhase == PhaseKind.WaitingToReady;
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public MainWindowViewModel()
        {
            this.StartCommand = new CommonCommand(startExecute);
            this.GetReadyCommand = new CommonCommand(getReadyExecute);
            this.CallCommand = new CommonCommand(callExecute);

            this._methodItems = new ReadOnlyCollection<MethodKind>(new[] {
                MethodKind.Look,
                MethodKind.Search,
                MethodKind.Walk,
                MethodKind.Put,
            });

            this._directionItems = new ReadOnlyCollection<DirectionKind>(new[] {
                DirectionKind.Left,
                DirectionKind.Up,
                DirectionKind.Right,
                DirectionKind.Down,
            });

            this.SelectedMethod = MethodKind.Look;
            this.SelectedDirection = DirectionKind.Left;

            this.CurrentPhase = PhaseKind.Unstarted;

            line = App.Current.IsMockMode ? (ILineManager)new MockLineManager() : new LineManager();
            line.ConnectionChanged += onConnectionChanged;
        }

        #region 接続開始コマンド処理

        /// <summary>
        /// Startコマンド実行メソッド
        /// </summary>
        private void startExecute()
        {
            viewLogger.Info("接続要求開始：host={0},port={1}", Host, Port);

            CurrentPhase = PhaseKind.WaitingToConnect;
            line.Connect(Host, Port, TeamName);
        }

        #endregion

        #region GetReadyコマンド処理

        /// <summary>
        /// GetReadyコマンド実行メソッド
        /// </summary>
        private void getReadyExecute()
        {
            CurrentPhase = PhaseKind.WaitingToResponse;

            line.GetReady(res =>
            {
                if (res == null)
                {
                    viewLogger.Info("異常終了");
                    CurrentPhase = PhaseKind.GameSet;
                    line.Close();
                    return;
                }

                FloorContext = new AroundFloorPanelContext(CellsInfoKind.Around, res.Cells);
                if (res.IsGameSet)
                {
                    CurrentPhase = PhaseKind.GameSet;
                    viewLogger.Info("ゲームセット");
                    line.Close();
                    return;
                }
                viewLogger.Info("メソッドを選択してください");
                CurrentPhase = PhaseKind.MethodSelection;
            });
        }

        #endregion

        #region Callコマンド処理

        /// <summary>
        /// Callコマンド実行メソッド
        /// </summary>
        private void callExecute()
        {
            var method = SelectedMethod;
            var dir = SelectedDirection;
            if (method == MethodKind.Unknown || dir == DirectionKind.Unknown)
            {
                Debug.Fail("メソッド、方向不正選択状態");
                viewLogger.Warn("メソッドと方向を選択してください");
                return;
            }

            viewLogger.Detail(">" + method.ToChar() + dir.ToChar());
            CurrentPhase = PhaseKind.WaitingToResponse;

            line.Call(method, dir, res =>
            {
                if (res == null)
                {
                    viewLogger.Info("異常終了");
                    CurrentPhase = PhaseKind.GameSet;
                    line.Close();
                    return;
                }

                FloorContext = new AroundFloorPanelContext(CellsInfoKindExt.FromMethod(method, dir), res.Cells);
                if (res.IsGameSet)
                {
                    CurrentPhase = PhaseKind.GameSet;
                    viewLogger.Info("ゲームセット");
                    line.Close();
                    return;
                }

                CurrentPhase = PhaseKind.WaitingToReady;
                viewLogger.Info("実行結果を確認し、GetReadyを送信してください");
            });
        }


        #endregion

        private void onConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            this.CurrentPhase = e.State == ConnectionState.Connected ? PhaseKind.WaitingToReady : PhaseKind.GameSet;

            viewLogger.Info("接続状態：" + e.State.ToDescription());
        }

    }
}
