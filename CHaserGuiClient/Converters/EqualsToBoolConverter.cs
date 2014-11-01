using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Oika.Apps.CHaserGuiClient.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class EqualsToBoolConverter : IValueConverter
    {
        /// <summary>
        /// 値の一致をFalseに変換するかどうかを取得または設定します。
        /// 既定値はFalse（値の一致をTrueに変換）です。
        /// </summary>
        public bool EqualsToFalse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool matches;

            if (value == null)
            {
                matches = parameter == null;
            }
            else
            {
                matches = value.Equals(parameter);
            }

            return EqualsToFalse ? !matches : matches;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
