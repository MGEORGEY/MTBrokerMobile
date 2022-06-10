using MTBrokerMobile.ApiMVM.FileMngt;
using MTBrokerMobile.Models;
using MTBrokerMobile.StaticHelpers;
using MTBrokerMobile.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTBrokerMobile.ViewModels
{
    internal class UploadFilesViewModel : INotifyPropertyChanged
    {
        #region Properties


        public Command ChooseFilesCommand { get; private set; }

        public Command UploadFilesCommand { get; private set; }


        private bool isBusy = false;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }


        private bool userIsLoggedIn = false;
        public bool UserIsLoggedIn
        {
            get => userIsLoggedIn;
            set { SetProperty(ref userIsLoggedIn, value); }
        }


        public List<FileAttach> AttachedFiles = new List<FileAttach>();

        public UploadFilesPage EntityPage { get; set; }

        #endregion


        #region Constructor
        public UploadFilesViewModel(UploadFilesPage uploadFilesPage)
        {
            EntityPage = uploadFilesPage;

            ChooseFilesCommand = new Command(execute: async () =>
            {
                try
                {
                    IsBusy = true;

                    AttachedFiles.Clear();

                    //if iOS fails, use this:
                    //DevicePlatform.iOS, new[] { "UTType.Text" }

                    var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] {"public.text" } }, { DevicePlatform.Android,new[] { "text/plain" } }, { DevicePlatform.UWP, new[] { ".txt" } }
                    });

                    var options = new PickOptions
                    {
                        PickerTitle = "Please select a .txt MT file",
                        FileTypes = customFileType,
                    };
                    var filesToAttach = (await FilePicker.PickMultipleAsync(options)).ToList();


                    for (int fileToAttachIndex = 0; fileToAttachIndex < filesToAttach.Count; fileToAttachIndex++)
                    {
                        AttachedFiles.Add(new FileAttach
                        {
                            ID = fileToAttachIndex,
                            ContentType = filesToAttach[fileToAttachIndex].ContentType,
                            FileName = filesToAttach[fileToAttachIndex].FileName,
                            FilePath = filesToAttach[fileToAttachIndex].FullPath,
                        });


                        var fileStream = await filesToAttach[fileToAttachIndex].OpenReadAsync();

                        AttachedFiles[fileToAttachIndex].FileBytes = fileStream;
                        AttachedFiles[fileToAttachIndex].FileSize = StaticVariables.GetReadableFileSize(fileStream.Length);

                        //EntityPage.stckLytAttachFiles.Children.Add(new FileAttachTemplate(EntityPage, AttachedFiles[fileToAttachIndex]));

                    }


                    //OnPropertyChanged(nameof(FilesAttached));

                    //OnPropertyChanged(nameof(TotalFileSize));

                    //OnPropertyChanged(nameof(IsFilesAttached));


                    IsBusy = false;


                    if (!StaticVariables.DeviceHasInternetAccess())
                    {
                        await EntityPage.DisplayAlert(StaticVariables.NoInternet, StaticVariables.ConnectToInternet, StaticVariables.Cancel);

                        return;
                    }

                    IsBusy = true;



                    var accessToken = StaticVariables.GetStringValueFromUserPrefKey(StaticVariables.JwtTokenPreferenceKey, string.Empty);

                    if (string.IsNullOrEmpty(accessToken.Trim()))
                    {
                        if (await EntityPage.DisplayAlert(StaticVariables.Login, StaticVariables.UserNotLoggedIn, StaticVariables.Login, StaticVariables.Cancel) == true)
                        {
                            await EntityPage.Navigation.PushModalAsync(new LoginPage());
                        }
                        IsBusy = false;
                        return;
                    }




                    var files = new List<ApiUploadFileAttachMVM>();
                    for (int fileIndex = 0; fileIndex < AttachedFiles.Count; fileIndex++)
                    {
                        files.Add(new ApiUploadFileAttachMVM { ContentType = AttachedFiles[fileIndex].ContentType, FileName = AttachedFiles[fileIndex].FileName, FileSize = AttachedFiles[fileIndex].FileSize });
                    }

                    var fileAttachmentKVPs = new List<KeyValuePair<string, Stream>>();

                    for (int fileIndex = 0; fileIndex < AttachedFiles.Count; fileIndex++)
                    {
                        fileAttachmentKVPs.Add(new KeyValuePair<string, Stream>(StaticVariables.FileAttachmentsMFFN, AttachedFiles[fileIndex].FileBytes));
                    }

                    var result = await StaticApiHelper.ParseMultipleMt940sAsync(accessToken, files, fileAttachmentKVPs);

                    if (result == null)
                    {
                        await EntityPage.DisplayAlert(StaticVariables.AppErrorTitle, StaticVariables.ServerUnreachableErrorMessage, StaticVariables.Okay);

                        IsBusy = false;
                        return;
                    }

                    if (!(bool)result.SuccessStatusMessageCRT?.Success)
                    {
                        await EntityPage.DisplayAlert(StaticVariables.AppErrorTitle, result.SuccessStatusMessageCRT?.StatusMessage, StaticVariables.Okay);

                        IsBusy = false;
                        return;
                    }

                    IsBusy = false;

                    // CancelCommand.Execute(null);

                    await EntityPage.Navigation.PushModalAsync(new ParsedMT940Page(true));

                    MessagingCenter.Send<object>(result, StaticVariables.ShareMT940Message);
                }
                catch (Exception exc)
                {
                    IsBusy = false;
                }
            });


            UploadFilesCommand = new Command(execute: async () =>
            {
                ChooseFilesCommand.Execute(null);
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
