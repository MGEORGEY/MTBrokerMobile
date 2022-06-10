using MTBrokerMobile.ViewModels;
using MTBrokerMobile.Views;
using Xamarin.Forms;

namespace MTBrokerMobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();


            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));

            Routing.RegisterRoute(nameof(UploadFilesPage), typeof(UploadFilesPage));


            BindingContext = new AppShellViewModel(this);
        }

    }
}
