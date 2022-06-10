using System;
using System.Globalization;
using Xamarin.Forms;

namespace MTBrokerMobile.ValueConverters
{
    public class EnumIndexIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (value is Enum) ? (int)value : 0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (value is int) ? Enum.ToObject(targetType, value) : 0;
    }
}
