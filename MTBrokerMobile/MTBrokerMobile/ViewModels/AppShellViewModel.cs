using MTBrokerMobile.StaticHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MTBrokerMobile.ViewModels
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        #region Properties

        public Command LogoutCommand { get; private set; }


        public Command UploadFilesCommand { get; private set; }


        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        //isAdmin
        bool isAdmin = false;
        public bool IsAdmin
        {
            get => isAdmin = StaticVariables.IsUserAllowedToCommit(new List<string> { StaticVariables.AdminRole
    });
            set => SetProperty(ref isAdmin, value);
        }

        public AppShell EntityPage { get; set; }

        #endregion


        #region Constructor
        public AppShellViewModel(AppShell appShell)
        {
            EntityPage = appShell;



            MessagingCenter.Subscribe<object>(this, StaticVariables.RefreshAppShellFlyoutsMessage, (sender) =>
            {
                RefreshPropertyValidity();
            });


            MessagingCenter.Subscribe<object>(this, StaticVariables.ShowUploadFileFlyoutMessage, (sender) =>
            {
                Shell.Current.CurrentItem = (Shell.Current as AppShell).uploadFilesFlyoutItem;
            });


            LogoutCommand = new Command(execute: () =>
            {

                StaticVariables.LogoutUserFromApp();

                RefreshPropertyValidity();

                MessagingCenter.Send<object>(this, StaticVariables.RefreshHomePageMessage);

                EntityPage.FlyoutIsPresented = false;
            });


            UploadFilesCommand = new Command(execute: () =>
            {
                StaticVariables.LogoutUserFromApp();

                RefreshPropertyValidity();

                EntityPage.FlyoutIsPresented = false;
            });

        }
        #endregion


        #region Set Property and Notify Property Changed

        #region Set Property
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #endregion


        void RefreshPropertyValidity()
        {
            OnPropertyChanged(nameof(IsAdmin));
        }

    }
}
