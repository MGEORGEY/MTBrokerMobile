using MTBrokerMobile.StaticHelpers;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MTBrokerMobile.ValueConverters
{
    internal class DebitOrCreditLabelStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == StaticVariables.DebitSymbol ? Application.Current.Resources["listViewContentDOrCLabelStyles"] : Application.Current.Resources["listViewContentLabelStyles"];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}
