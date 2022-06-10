using System;
using System.Globalization;
using Xamarin.Forms;

namespace MTBrokerMobile.ValueConverters
{
    public class MutuallyExclusiveVisibConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    }
}
