using MTBrokerMobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTBrokerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParsedMT940Page : ContentPage
    {
        public ParsedMT940Page()
        {
            InitializeComponent();

            BindingContext = new ParsedMT940ViewModel(this);
        }

        public ParsedMT940Page(bool isInManualMode)
        {
            InitializeComponent();

            BindingContext = new ParsedMT940ViewModel(this, isInManualMode);
        }

    }
}