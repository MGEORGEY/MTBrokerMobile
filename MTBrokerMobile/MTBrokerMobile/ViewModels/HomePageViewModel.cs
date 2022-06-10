using MTBrokerMobile.StaticHelpers;
using MTBrokerMobile.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MTBrokerMobile.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        #region Properties


        public Command LoginCommand { get; private set; }


        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }


        private bool userIsLoggedIn = false;
        public bool UserIsLoggedIn
        {
            get => userIsLoggedIn = StaticVariables.UserIsLoggedIn();
            set { SetProperty(ref userIsLoggedIn, value); }
        }

        #endregion


        #region Constructor
        public HomePageViewModel(HomePage homePage)
        {

            MessagingCenter.Subscribe<object>(this, StaticVariables.RefreshHomePageMessage, (sender) =>
            {
                OnPropertyChanged(nameof(UserIsLoggedIn));
            });


            LoginCommand = new Command(execute: async () =>
            {
                LoginPage loginPage = new LoginPage();

                Application.Current.ModalPopping += HandleModalPopping;

                //loginPage = new LoginPage();

                await Shell.Current.Navigation.PushModalAsync(loginPage, true);

                async void HandleModalPopping(object sender, ModalPoppingEventArgs e)
                {
                    if (e.Modal == loginPage)
                    {

                        loginPage = null;
                        Application.Current.ModalPopping -= HandleModalPopping;

                        OnPropertyChanged(nameof(UserIsLoggedIn));
                    }
                }
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

    }
}
