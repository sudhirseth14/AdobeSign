using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AdobeSign.Contracts;
using AdobeSign.Logs;
using AdobeSign.Models;
using Newtonsoft.Json;

namespace AdobeSign.Services
{
    public class AdobeObject
    {
        private RestAPI API;
        private string baseendpoint = "/api/rest/v6";
        CustomLogs Err = new CustomLogs();
        public AdobeObject(RestAPI api)
        {
            API = api;
        }

        /// <summary>
        /// Save token details in temp file
        /// </summary>
        /// <returns></returns>
        public void WriteTokenDetailstoTempFile(AccessToken tokenDetails)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(tokenDetails);
                string fileName = "adobesigntokendetails.txt";
                string myTempFile = Path.Combine(Path.GetTempPath(), fileName);
                using (StreamWriter sw = new StreamWriter(myTempFile))
                {
                    sw.WriteLine(jsonContent);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving token details: " + ex.Message.ToString());
            }
        }


        #region users activities apis
        public async Task<string> GetAllUsers()
        {
            string json = await API.GetRestJson(baseendpoint + "/users");
            return json;
        }

        public async Task<UserInfo> GetUserInfo(string userid)
        {
            var endpoint = string.Format(baseendpoint + "/users/{0}", userid);
            string json = await API.GetRestJson(endpoint);
            return JsonConvert.DeserializeObject<UserInfo>(json);
        }


        public async Task<string> GetUserInfoJson(string userid)
        {
            var endpoint = string.Format(baseendpoint + "/users/{0}", userid);
            string json = await API.GetRestJson(endpoint);
            return json;
        }
        public async Task<string> GetUserGroupInfo(string userid)
        {
            var endpoint = string.Format(baseendpoint + "/users/{0}/groups", userid);
            string json = await API.GetRestJson(endpoint);
            return json;
        }

        public async Task<string> UpdateUserState(string inputjson, string userid)
        {
            var jsonContent = inputjson;
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PutRest(baseendpoint + "/users/" + userid + "/state", byteContent);
            return json;
        }

        public async Task<string> UpdateUserDetails(UserInfo userInfo, string userid)
        {
            var jsonContent = JsonConvert.SerializeObject(userInfo);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PutRest(baseendpoint + "/users/" + userid, byteContent);
            return json;
        }

        #endregion user

        #region groups
        public async Task<string> GetAllGroups()
        {
            string json = await API.GetRestJson(baseendpoint + "/groups");
            return json;
        }

        public async Task<string> GetGroupInfo(string groupid)
        {
            var endpoint = string.Format(baseendpoint + "/groups/{0}", groupid);
            string json = await API.GetRestJson(endpoint);
            return json;
        }

        public async Task<string> GetAllUsersOfGroup(string groupid)
        {
            var endpoint = string.Format(baseendpoint + "/groups/{0}/users", groupid);
            string json = await API.GetRestJson(endpoint);
            return json;
        }


        #endregion groups

        #region account
        public async Task<string> GetAccountInfo(string accountid)
        {
            var endpoint = string.Format(baseendpoint + "/accounts/{0}", accountid);
            string json = await API.GetRestJson(endpoint);
            return json;
        }
        #endregion

        #region message
        public async Task<string> GetMessageTemplate(string locale)
        {
            var endpoint = string.Format(baseendpoint + "/messageTemplates?locale={0}", locale);
            string json = await API.GetRestJson(endpoint);
            return json;
        }
        #endregion message

        #region Agreements
        public async Task<string> GetAgreements()
        {
            string json = await API.GetRestJson(baseendpoint + "/agreements");
            return json;
        }


        public async Task<string> GetAgreement(string agreementID)
        {
            //On null or empty agreement id, API is returning all agreements.
            if (string.IsNullOrWhiteSpace(agreementID))
            {
                return null;
            }
            var endpoint = string.Format(baseendpoint + "/agreements/{0}", agreementID);
            string json = await API.GetRestJson(endpoint);
            return json;
            //return JsonConvert.DeserializeObject<AdobeSign.AgreementInfo>(json);
        }


        public async Task<string> GetAgreementSigningURls(string agreementID)
        {
            //On null or empty agreement id, API is returning all agreements.
            if (string.IsNullOrWhiteSpace(agreementID))
            {
                return null;
            }
            var endpoint = string.Format(baseendpoint + "/agreements/{0}/signingUrls", agreementID);
            string json = await API.GetRestJson(endpoint);
            return json;
        }
        public async Task<byte[]> GetAgreementCombinedDocument(string agreementID)
        {
            var endpoint = string.Format("api/rest/v6/agreements/{0}/combinedDocument", agreementID);
            return await API.GetRestBytes(endpoint);
        }


        public async Task<AgreementCreationResponse> CreateAgreement(string inputjson)
        {
            var jsonContent = inputjson;
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PostRest(baseendpoint + "/agreements", byteContent);
            return JsonConvert.DeserializeObject<AgreementCreationResponse>(json);

        }

        public async Task<string> ShareAgreement(string inputjson, string agreementid)
        {
            var jsonContent = inputjson;
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PostRest(baseendpoint + "/agreements/" + agreementid + "/members/share", byteContent);
            return json;

        }

        public async Task<string> SendReminder(string inputjson, string agreementid)
        {
            var jsonContent = inputjson;
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PostRest(baseendpoint + "/agreements/" + agreementid + "/reminders", byteContent);
            return json;

        }
        public async Task<TransientDocumentResponse> AddDocument(string fileName, byte[] fileData, string mimeType = "")
        {
            var content = new MultipartFormDataContent();
            HttpContent fileContent = new ByteArrayContent(fileData);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "File",
                FileName = fileName

            };
            content.Add(fileContent);
            content.Add(new StringContent(fileName), String.Format("\"{0}\"", "File-Name"));
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                content.Add(new StringContent(mimeType), String.Format("\"{0}\"", "Mime-Type"));
            }
            string json = await API.PostRest(baseendpoint + "/transientDocuments", content);
            return JsonConvert.DeserializeObject<TransientDocumentResponse>(json);
        }

        public async Task<string> UpdateAgreementVisibility(string jsonContent, string agreementId)
        {
            var endpoint = string.Format(baseendpoint + "/agreements/{0}/me/visibility", agreementId);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PutRest(endpoint, byteContent);
            return json;
        }

        public async Task<string> UpdateAgreementState(string jsonContent, string agreementId)
        {
            var endpoint = string.Format(baseendpoint + "/agreements/{0}/state", agreementId);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PutRest(endpoint, byteContent);
            return json;
        }


        public async Task<string> DeleteAgreement(string agreementId)
        {
            var endpoint = string.Format(baseendpoint + "/agreements/{0}/documents", agreementId);
            string json = await API.DeleteRestJson(endpoint);
            return json;
        }
        #endregion Agreements


        /// <summary>
        /// Get the access token using authorization code
        /// </summary>
        /// <param name="apiURL">API Uri</param>
        /// <param name="authorization_code">Authorization Code - the authorization code obtained in Authorization Request process</param>
        /// <param name="clientid">Application ID - obtained from OAuth Configuration page / Identifies the application</param>
        /// <param name="client_secret">Client secret key - obtained from OAuth Configuration page / Authenticates the application</param>
        /// <param name="redirectURL">Redirect URL - must match the value used during the Authorization Code step / This value must belong to the set of values specified on the OAuth Configuration page</param>        
        /// <returns>AccessToken object</returns>
        public async Task<AccessToken> GetAccessToken(string apiURL, string authorization_code, string clientid, string client_secret, string redirectURL)
        {
            RestAPI API = new RestAPI("", apiURL, "", clientid, client_secret, "", null);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("code", authorization_code);
            parameters.Add("client_id", clientid);
            parameters.Add("client_secret", client_secret);
            parameters.Add("redirect_uri", redirectURL);
            parameters.Add("grant_type", "authorization_code");
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            string json = await API.PostRest("/oauth/token", encodedContent, "application/x-www-form-urlencoded");
            return API.DeserializeJSon<AccessToken>(json);
        }

        public async Task<AccessToken> ConvertJsonToAccessToken(string json)
        {
            return JsonConvert.DeserializeObject<AccessToken>(json);
        }
        /// <summary>
        /// Get Access Token using refresh token
        /// </summary>
        /// <param name="apiURL">API Uri</param>
        /// <param name="refresh_token">Refresh Token, which can be used to get a fresh Access Token</param>
        /// <param name="clientid">Application ID - obtained from OAuth Configuration page / Identifies the application</param>
        /// <param name="client_secret">Client secret key - obtained from OAuth Configuration page / Authenticates the application</param>
        /// <returns>AccessToken object - Refresh_token property would be null on this call.</returns>
        public async Task<AccessToken> GetAccessTokenByRefreshToken(string apiURL, string refresh_token, string clientid, string client_secret, DateTime? tokenExpireDateTime)
        {
            RestAPI API = new RestAPI("", apiURL, "", clientid, client_secret, refresh_token, tokenExpireDateTime);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("refresh_token", refresh_token);
            parameters.Add("client_id", clientid);
            parameters.Add("client_secret", client_secret);
            parameters.Add("grant_type", "refresh_token");
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            string json = await API.PostRest("/oauth/refresh", encodedContent, "application/x-www-form-urlencoded");
            return API.DeserializeJSon<AccessToken>(json);
        }


        public async Task<bool> RevokedAccessToken(string access_token)
        {
            bool revokedstatus = false;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("token", access_token);
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            string json = await API.PostRest("/oauth/revoke", encodedContent, "application/x-www-form-urlencoded");
            revokedstatus = true;
            return revokedstatus;
        }

        #region workflows
        public async Task<string> GetAllWorkFlows()
        {
            string json = await API.GetRestJson(baseendpoint + "/workflows");
            return json;
        }

        public async Task<string> GetWorkflow(string workflowid)
        {
            var endpoint = string.Format(baseendpoint + "/workflows/{0}", workflowid);
            string json = await API.GetRestJson(endpoint);
            return json;
        }
        #endregion workflows

        #region library

        public async Task<string> GetAllLibraryDocuments()
        {
            string json = await API.GetRestJson(baseendpoint + "/libraryDocuments");
            return json;
        }

        public async Task<string> GetLibraryDocument(string libraryDocumentId)
        {
            var endpoint = string.Format(baseendpoint + "/libraryDocuments/{0}", libraryDocumentId);
            string json = await API.GetRestJson(endpoint);
            return json;
        }

        public async Task<string> UpdateLibrarDocVisibility(string jsonContent, string libraryDocumentId)
        {
            var endpoint = string.Format(baseendpoint + "/libraryDocuments/{0}/me/visibility", libraryDocumentId);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PutRest(endpoint, byteContent);
            return json;
        }

        public async Task<string> UpdateLibrarDocState(string jsonContent, string libraryDocumentId)
        {
            var endpoint = string.Format(baseendpoint + "/libraryDocuments/{0}/state", libraryDocumentId);
            var buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            string json = await API.PutRest(endpoint, byteContent);
            return json;
        }
        #endregion library
    }
}
