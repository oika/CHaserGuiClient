using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Oika.Apps.CHaserGuiClient.Converters
{
    [ValueConversion(typeof(CellKind), typeof(string))]
    public class CellKindToDispCharConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (CellKind)value;
            return val == CellKind.Block ? "■"
                    : val == CellKind.Character ? "C"
                    : val == CellKind.Item ? "◇"
                    : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (string)value;
            return val == "■" ? CellKind.Block
                : val == "C" ? CellKind.Character
                : val == "◇" ? CellKind.Item
                : CellKind.Nothing;

        }
    }
}
