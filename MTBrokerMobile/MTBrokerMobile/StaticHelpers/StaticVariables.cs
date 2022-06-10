using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MTBrokerMobile.StaticHelpers
{
    public static class StaticVariables
    {



        #region API Properties 
        public const string AppName = "MT Broker";


        //localhost
        //10.10.196.88
        //192.168.43.34


        public static string ServerIpAddress { get => "192.168.137.151"; }

        public static string ApiBaseUri { get => $@"https://{ServerIpAddress}:7225/"; }



        private static string MessageApi = "api/messageapi";

        private static string UserAccountApi = "api/useraccountapi";


        #endregion


        #region Symbols

        public static string Close => "x";

        public static string Download => "Download";

        public static string DownloadsFolder => $"{AppName} Downloads";

        public static string OptionsSymbol => "...";

        public static string RemoveEntitySymbol => "_";

        public static string Refresh => "\u27F3";

        #endregion


        #region Internationalization

        #region App Specific
        public static string AllMessages => "All messages";

        public static string ChooseFilesToUpload => "Choose files...";

        public static string UploadFiles => "Upload files";

        public static string ViewAllMessages => "View all messages";

        #endregion


        #region General

        public static string AppErrorMessage => "We are experiencing a challenge completing your request";

        public static string AppErrorTitle => "Error";

        public static string AppVersion => "App version: ";

        public static string AttachFiles => "Attach files";

        public static string AttachedFiles => "Attached files: ";

        public static string Cancel => "Cancel";

        public static string ConfirmPassword => "Confirm password";

        public static string ConnectToInternet => "Connect to the Internet";

        public static string DownloadAll => "Download all";

        public static string DownloadComplete => "File successfully saved.";

        public static string DownloadFailedErrorMessage => "Download failed. No content was received.";

        public static string Email => "Email";

        public static string FileAttachments => "File Attachments";

        public static string Files => "Files: ";

        public static string FileSaveFailedErrorMessage => "File(s) could not be saved";

        public static string Home => "Home";


        public static string Login = "Login";

        public static string Logout => "Logout";

        public static string Name => "Name";

        public static string No => "No";

        public static string NoDataToSaveErrorMessage => "No items found.";

        public static string NoInternet => "Internet";

        public static string Okay => "Okay";

        public static string OpenSavedFileRequest => "Would you like to open the saved file now?";

        public static string Password => "Password";

        public static string Phone => "Phone";

        public static string Save => "Save";

        public static string ServerUnreachableErrorMessage => "Server unreachable. Try again later.";

        public static string TotalFileSize => "Total file size: ";

        public static string UsernameOrEmailAddress => "Username or email address";

        public static string UserNotLoggedIn => "You need to be logged in to continue";

        public static string Yes => "Yes";


        #endregion


        #region MT 940 Specific

        public static string AccountID25 => "Account ID";

        public static string AccOwnerInfo86Info => "Account Owner Info";

        public static string AvailableBalance64Amount => "Closing Available Balance";

        public static string Block1SequenceNumber => "Block 1 Seq. No.";

        public static string Block1SessionNumber => "Block 1 Session. No.";

        public static string ClosingBalance62FAmount => "Closing Balance";

        public static string Continuation => "Continuation";

        public static string CreditSymbol => "C";

        public static string CustomCreditSymbol => "Cr.";

        public static string CustomDebitSymbol => "Dr.";

        public static string Currency => "Currency";

        public static string Date => "Date";

        public static string DebitSymbol => "D";

        public static string DebitOrCredit => $"{DebitSymbol}/{CreditSymbol}";

        public static string FinBranchCode => "FIN Branch Code";

        public static string FinLTCode => "FIN LT Code";

        public static string FinSwiftAddress => "Fin Swift Address";

        public static string OpeningBalance60FAmount => "Opening Balance";

        public static string SendersSwiftAddress => "Sender Swift Address";

        public static string StatementLine61Amount => "Customer Reference Amount";

        public static string StatementLine61CustomerRef => "Customer Reference";

        public static string StatementLine61TrnsactnTypeID => "TRXN Type ID";

        public static string StatementLine61ValueDate => "Value Date";

        public static string StatementOrSeqNo28CMsgSeq => "Message Sequence No.";

        public static string StatementOrSeqNo28CStmntSeq => "Message Statement No.";

        public static string TransactionRefNo20 => "Transaction Reference No.";

        #endregion


        #endregion


        #region Messaging Center

        public static string ShowUploadFileFlyoutMessage => "Show upload files";

        public static string RefreshAppShellFlyoutsMessage => "Refresh flyouts";

        public static string RefreshHomePageMessage => "Refresh home page";

        public static string ShareMT940Message => "Share MT940 Message";

        public static string ShareMultipleMT940Message => "Share Multiple MT940 Message";


        #endregion


        #region User Roles

        public const string AccountHolderRole = "AccountHolderRole";


        public const string AdminRole = "AdminRole";


        public const string BankRole = "BankRole";


        #endregion


        #region API Actions

        #region User Account Actions

        public static string LoginActionName { get => $@"{UserAccountApi}/login"; }


        #endregion

        #region Message Mngt

        public static string GetStoredMT940sActionName { get => $@"{MessageApi}/getStoredMT940s"; }

        public static string ParseMultipleMt940sActionName { get => $@"{MessageApi}/parseMultipleMt940s"; }

        #endregion

        #endregion


        #region Multipart Form File Names
        public static string FileAttachmentsMFFN => "file_attachments";
        #endregion


        #region Preferences

        public static string JwtTokenPreferenceKey { get => "jwtTokenPreferenceKey"; }

        public static string NamePreferenceKey { get => "namePreferenceKey"; }

        public static string UsernamePreferenceKey { get => "usernamePreferenceKey"; }

        public static string UserRolesPreferenceKey { get => "userRolesPreferenceKey"; }


        #endregion


        #region Methods


        #region Connectivity
        //Device Has Internet Access
        public static bool DeviceHasInternetAccess() => Connectivity.NetworkAccess == NetworkAccess.Internet/*true*/;

        #region Http Clients
        public static HttpClient InitializeHttpClient()
        {
            var client = new HttpClient();

#if DEBUG
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    {
                        client = new HttpClient(DependencyService.Get<DependencyServiceInterfaces.IHttpClientHandlerCreationService>().GetInsecureHandler());

                        client.BaseAddress = new Uri(ApiBaseUri);
                        break;
                    }

                default:
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                        client = new HttpClient(new HttpClientHandler());
                        client.BaseAddress = new Uri(ApiBaseUri);
                        break;
                    }
            }
#else


            client = new HttpClient(new HttpClientHandler());
            client.BaseAddress = new Uri(ApiBaseUri);
#endif



            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return client;

        }

        public static HttpClient InitializeHttpClient(string accessToken)
        {
            var client = InitializeHttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            return client;

        }

        #region API Gateway

        #region Get
        public static async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string uri)
        {
            HttpResponseMessage httpResponseMessage;

            using (httpClient)
            {
                httpResponseMessage = await httpClient.GetAsync(uri);
            }
            return httpResponseMessage;
        }
        #endregion


        #region Post
        public static async Task<HttpResponseMessage> PostAsync(HttpClient httpClient, string uri, HttpContent httpContent)
        {
            HttpResponseMessage httpResponseMessage;


            using (httpClient)
            {
                using (httpContent)
                {
                    httpResponseMessage = await httpClient.PostAsync(uri, httpContent);
                }
            }
            return httpResponseMessage;
        }
        #endregion


        #region Delete
        public static async Task<HttpResponseMessage> DeleteAsync(HttpClient httpClient, string uri)
        {
            HttpResponseMessage httpResponseMessage;

            using (httpClient)
            {
                httpResponseMessage = await httpClient.DeleteAsync(uri);
            }
            return httpResponseMessage;
        }
        #endregion


        #region Put
        public static async Task<HttpResponseMessage> PutAsync(HttpClient httpClient, string uri, HttpContent httpContent)
        {
            HttpResponseMessage httpResponseMessage;

            using (httpClient)
            {
                using (httpContent)
                {
                    httpResponseMessage = await httpClient.PutAsync(uri, httpContent);
                }
            }
            return httpResponseMessage;
        }
        #endregion

        #endregion

        #endregion

        #endregion


        #region Get Readable File Size
        public static string GetReadableFileSize(long fileSize)
        {
            // Get absolute value

            long absolute_i = Math.Abs(fileSize);

            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = fileSize >> 50;
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = fileSize >> 40;
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = fileSize >> 30;
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = fileSize >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = fileSize >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = fileSize;
            }
            else
            {
                return fileSize.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = readable / 1024;

            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }
        #endregion


        #region Fetch String Content
        /// <summary>
        /// Returns a StringContent object that is passed to API by HttpClient
        /// </summary>
        /// <param name="objToSerialize">Model object values to be passed to API</param>        
        /// <returns>StringContent</returns>
        public static System.Net.Http.StringContent FetchStringContentForHttpClient(object objToSerialize)
        {
            var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(objToSerialize));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return content;
        }
        #endregion


        #region Fetch Multipart Form Content
        /// <summary>
        /// Returns a MultipartFormDataContent object that is passed to API by HttpClient
        /// </summary>
        /// <param  name="file">Model object values to be passed to API</param>
        /// <param name="httpContent">HttpContent to pass to API</param>
        /// <returns>StringContent</returns>
        public static MultipartFormDataContent FetchStreamMultipartContentForHttpClient(List<KeyValuePair<string, Stream>> streamFileNameKVPs, HttpContent httpContent)
        {
            var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(httpContent);

            streamFileNameKVPs.ForEach(n =>
            {
                if (n.Value?.Length > 0 && !string.IsNullOrEmpty(n.Key) && !string.IsNullOrEmpty(n.Key))
                {
                    multipartFormDataContent.Add(new StreamContent(n.Value), n.Key, n.Key);
                }
            });

            return multipartFormDataContent;
        }
        #endregion


        #region Input Has Blank Values
        public static bool InputHasBlankValues(List<string> inputValues) => inputValues.Any(n => string.IsNullOrEmpty(n.Trim()));
        #endregion


        #region User Accounts
        public static bool UserIsLoggedIn() => GetStringValueFromUserPrefKey(JwtTokenPreferenceKey, string.Empty).Length > 0 || GetUserRolesFromPreferences()?.Count > 0;


        public static List<string> GetUserRolesFromPreferences() => JsonConvert.DeserializeObject<List<string>>(GetStringValueFromUserPrefKey(UserRolesPreferenceKey, string.Empty)) ?? new List<string>();


        public static void LogoutUserFromApp()
        {
            RemoveKeyFromUserPrefernces(JwtTokenPreferenceKey);
            RemoveKeyFromUserPrefernces(NamePreferenceKey);
            RemoveKeyFromUserPrefernces(UsernamePreferenceKey);
            RemoveKeyFromUserPrefernces(UserRolesPreferenceKey);
        }

        public static bool IsUserAllowedToCommit(List<string> permittedRoles)
        {
            var userRoles = GetUserRolesFromPreferences();

            return userRoles.Any(n => permittedRoles.Contains(n));
        }

        #endregion


        #region User Preferences

        #region Check if Key Exists
        public static bool PrefKeyExists(string key) => Preferences.ContainsKey(key);
        #endregion


        #region Retrive Value From a Key
        public static string GetStringValueFromUserPrefKey(string key, string defaultValue) => Preferences.Get(key, defaultValue);
        #endregion


        #region Set a Value for a Key
        public static void SetStringValueForKeyInUserPreferences(string key, string value) => Preferences.Set(key, value);
        #endregion


        #region Remove Key From User Preferences
        public static void RemoveKeyFromUserPrefernces(string key) => Preferences.Remove(key);
        #endregion


        #region Remove All User Preferences
        public static void RemoveAllUserPreferences() => Preferences.Clear();
        #endregion

        #endregion


        #region Split String Camel Case
        public static string SplitCamelCase(this string target) => Regex.Replace(
                Regex.Replace(
                    target,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        #endregion

        #endregion


    }
}
