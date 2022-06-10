using MTBrokerMobile.ApiMVM.UserMngt;
using MTBrokerMobile.StaticHelpers;
using MTBrokerMobile.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MTBrokerMobile.ViewModels.UserAccounts
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #region Properties
        public Command AboutCommand { get; private set; }

        public Command LoginCommand { get; private set; }

        public Command CancelCommand { get; private set; }


        //username or email address
        private string userNameorEmailAddress = string.Empty;
        public string UsernameOrEmailAddress
        {
            get => userNameorEmailAddress;
            set => SetProperty(ref userNameorEmailAddress, value);
        }


        //password
        string password = string.Empty;
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }


        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }


        public LoginPage EntityPage { get; set; }

        #endregion



        #region Constructor
        public LoginViewModel(LoginPage loginPage)
        {
            EntityPage = loginPage;


            AboutCommand = new Command(execute: async () =>
            {
                await EntityPage.DisplayAlert(StaticVariables.AppName, $"A web version of this app is available.", StaticVariables.Okay);
            });


            CancelCommand = new Command(execute: () =>
            {
                IsBusy = false;
                EntityPage.Navigation.PopModalAsync();
            });

            LoginCommand = new Command(execute: async () =>
            {

                if (!StaticVariables.DeviceHasInternetAccess())
                {
                    await EntityPage.DisplayAlert(StaticVariables.NoInternet, StaticVariables.ConnectToInternet, StaticVariables.Cancel);
                    return;
                }

                IsBusy = true;



                var result = await StaticApiHelper.LoginAsync(new LoginMVM
                {
                    UsernameorEmailAddress = UsernameOrEmailAddress,
                    Password = Password
                });


                #region Negative API Result Checks

                if (result == null || result.AppUserCookieCRT is null || result.JwtTokenAndRolesCRT is null)
                {
                    await EntityPage.DisplayAlert(StaticVariables.AppErrorTitle, StaticVariables.AppErrorMessage, StaticVariables.Cancel);
                    IsBusy = false;
                    return;
                }

                if (!(bool)result.SuccessStatusMessageCRT?.Success)
                {
                    await EntityPage.DisplayAlert($"{StaticVariables.AppName}", result.SuccessStatusMessageCRT?.StatusMessage ?? StaticVariables.AppErrorMessage, StaticVariables.Cancel);
                    IsBusy = false;
                    return;
                }
                #endregion


                StaticVariables.SetStringValueForKeyInUserPreferences(StaticVariables.JwtTokenPreferenceKey, result.JwtTokenAndRolesCRT.JwtToken);

                StaticVariables.SetStringValueForKeyInUserPreferences(StaticVariables.NamePreferenceKey, result.AppUserCookieCRT.Name);

                StaticVariables.SetStringValueForKeyInUserPreferences(StaticVariables.UsernamePreferenceKey, result.AppUserCookieCRT.Username);

                StaticVariables.SetStringValueForKeyInUserPreferences(StaticVariables.UserRolesPreferenceKey, JsonConvert.SerializeObject(result.JwtTokenAndRolesCRT.Roles/*, new JsonSerializerOptions(){ Newtonsoft.Json.Formatting.Indented*/));

                MessagingCenter.Send<object>(this, StaticVariables.RefreshAppShellFlyoutsMessage);

                MessagingCenter.Send<object>(string.Empty, StaticVariables.RefreshHomePageMessage);


                IsBusy = false;

                CancelCommand.Execute(null);

                MessagingCenter.Send<object>(string.Empty, StaticVariables.ShowUploadFileFlyoutMessage);

            }, canExecute: () => UsernameOrEmailAddress.Trim().Length > 0 && Password.Length > 0 && !IsBusy);
        }
        #endregion

        private void RefreshCanExecutes()
        {
            LoginCommand.ChangeCanExecute();
        }

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
            RefreshCanExecutes();
        }
        #endregion

        #endregion
    }
}
