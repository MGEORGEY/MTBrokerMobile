using MTBrokerMobile.ApiMVM.FileMngt;
using MTBrokerMobile.ApiMVM.UserMngt;
using MTBrokerMobile.ControllerReturnTypes.MessageMngt;
using MTBrokerMobile.ControllerReturnTypes.UserMngt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTBrokerMobile.StaticHelpers
{
    public static class StaticApiHelper
    {
        #region User Account API

        #region Login
        public static async Task<RegisterAppUserCRT> LoginAsync(LoginMVM loginMVM)
        {
            try
            {
                var httpResponseMessage = await StaticVariables.PostAsync(StaticVariables.InitializeHttpClient(), StaticVariables.LoginActionName, StaticVariables.FetchStringContentForHttpClient(loginMVM));
                var jsonStringFromResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<RegisterAppUserCRT>(jsonStringFromResponseMessage);
                return result;
            }
            catch (Exception exc)
            {
                return new RegisterAppUserCRT();
            }
        }

        #endregion

        #endregion


        #region Message Mngt


        #region Get Stored MT940s
        public static async Task<List<MT940CRT>> GetStoredMT940sAsync(string accessToken)
        {
            try
            {
                var httpResponseMessage = await StaticVariables.GetAsync(StaticVariables.InitializeHttpClient(accessToken), StaticVariables.GetStoredMT940sActionName);
                var jsonStringFromResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<List<MT940CRT>>(jsonStringFromResponseMessage);
                return result;
            }
            catch (Exception)
            {
                return new List<MT940CRT>();
            }
        }
        #endregion


        #region Parse MT940s
        public static async Task<ParseMT940WithStatusCRT> ParseMultipleMt940sAsync(string accessToken, List<ApiUploadFileAttachMVM> apiUploadFileAttachMVMs, List<KeyValuePair<string, Stream>> streamFileNameKVPs)
        {
            try
            {

                ParseMT940WithStatusCRT result;

                using (var httpResponseMessage = await StaticVariables.PostAsync(StaticVariables.InitializeHttpClient(accessToken), StaticVariables.ParseMultipleMt940sActionName, StaticVariables.FetchStreamMultipartContentForHttpClient(streamFileNameKVPs, StaticVariables.FetchStringContentForHttpClient(apiUploadFileAttachMVMs))))
                {
                    var jsonStringFromResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();

                    result = JsonConvert.DeserializeObject<ParseMT940WithStatusCRT>(jsonStringFromResponseMessage);
                }
                return result;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
        #endregion

        #endregion

    }
}
