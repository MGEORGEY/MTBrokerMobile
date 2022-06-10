using MTBrokerMobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MTBrokerMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UploadFilesPage : ContentPage
    {
        public UploadFilesPage()
        {
            InitializeComponent();

            BindingContext = new UploadFilesViewModel(this);
        }
    }
}