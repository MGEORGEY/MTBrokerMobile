using MTBrokerMobile.StaticHelpers;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MTBrokerMobile.ValueConverters
{
    public class DebitOrCreditTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == StaticVariables.DebitSymbol ? StaticVariables.CustomDebitSymbol : (string)value == StaticVariables.CreditSymbol ? StaticVariables.CustomCreditSymbol : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (string)value == StaticVariables.DebitSymbol ? StaticVariables.CustomDebitSymbol : (string)value == StaticVariables.CreditSymbol ? StaticVariables.CustomCreditSymbol : value;
    }
}
