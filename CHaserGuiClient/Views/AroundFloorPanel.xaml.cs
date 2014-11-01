using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oika.Apps.CHaserGuiClient.Views
{
    /// <summary>
    /// AroundFloorPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class AroundFloorPanel : UserControl
    {
        #region Locationプロパティ

        public static readonly DependencyProperty LocationProperty
            = DependencyProperty.Register("Location", typeof(CellsInfoKind), typeof(AroundFloorPanel),
                                          new FrameworkPropertyMetadata(CellsInfoKind.Unknown, onLocationPropertyChanged));

        public CellsInfoKind Location
        {
            get
            {
                return (CellsInfoKind)GetValue(LocationProperty);
            }
            set
            {
                SetValue(LocationProperty, value);
            }
        }

        /// <summary>
        /// LocationProperty変更時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void onLocationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var obj = sender as AroundFloorPanel;

            obj.updateVisibilities();
        }

        #endregion

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public AroundFloorPanel()
        {
            InitializeComponent();

            //Locationのバインド
            SetBinding(LocationProperty, new Binding("Location") { Mode = BindingMode.OneWay });
        }


        private void updateVisibilities()
        {
            Grid dispGrid;

            switch (Location)
            {
                case CellsInfoKind.Around:
                case CellsInfoKind.LookLeft:
                case CellsInfoKind.LookRight:
                case CellsInfoKind.LookUp:
                case CellsInfoKind.LookDown:
                    dispGrid = gridAround;
                    break;
                case CellsInfoKind.SearchLeft:
                    dispGrid = gridSearchLeft;
                    break;
                case CellsInfoKind.SearchRight:
                    dispGrid = gridSearchRight;
                    break;
                case CellsInfoKind.SearchUp:
                    dispGrid = gridSearchUp;
                    break;
                case CellsInfoKind.SearchDown:
                    dispGrid = gridSearchDown;
                    break;
                default:
                    dispGrid = null;
                    break;
            }

            foreach (var grid in gridBase.Children.OfType<Grid>())
            {
                grid.Visibility = grid.Equals(dispGrid) ? Visibility.Visible : Visibility.Collapsed;
            }

        }

    }
}
