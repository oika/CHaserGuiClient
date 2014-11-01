using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Oika.Apps.CHaserGuiClient.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class EqualsToVisibilityConverter : IValueConverter
    {
        public Visibility MatchedVisibility { get; set; }
        public Visibility UnmatchedVisibility { get; set; }

        public EqualsToVisibilityConverter()
        {
            this.MatchedVisibility = Visibility.Visible;
            this.UnmatchedVisibility = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return parameter == null ? MatchedVisibility : UnmatchedVisibility;

            return value.Equals(parameter) ? MatchedVisibility : UnmatchedVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
