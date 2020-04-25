using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AdobeSign.Contracts;
using AdobeSign.Logs;
using AdobeSign.Models;
using Newtonsoft.Json;

namespace AdobeSign.Services
{
    public class RestAPI
    {
        private string APIURL = string.Empty;
        private string AccessToken = string.Empty;
        private string ClientID = string.Empty;
        private string ClientSecret = string.Empty;
        private string RefreshToken = string.Empty;
        private DateTime? TokenExpireDateTime = DateTime.UtcNow;
        CustomLogs Err = new CustomLogs();

        public RestAPI(string token_type, string apiURL, string accessToken, string clientId, string clientSecret, string refreshToken, DateTime? tokenExpireDateTime)
        {
            APIURL = apiURL.TrimEnd('/') + "/";

            if (token_type != "")
                AccessToken = token_type + " " + accessToken;
            else
                AccessToken = accessToken;

            ClientID = clientId;
            ClientSecret = clientSecret;
            RefreshToken = refreshToken;
            TokenExpireDateTime = tokenExpireDateTime;
        }


        public async Task<string> GetRestJson(string endpoint, string contentType = "application/json")
        {
            HttpResponseMessage response = await GetResponseMessage(endpoint, contentType);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {

                throw new Exception(await GetError(response));
            }
        }
        public async Task<byte[]> GetRestBytes(string endpoint, string contentType = "*/*")
        {
            HttpResponseMessage response = await GetResponseMessage(endpoint, contentType);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                throw new Exception(await GetError(response));
            }
        }
        public async Task<string> PostRest(string endpoint, HttpContent data, string contentType = "application/json")
        {
            HttpResponseMessage response = await PostResponseMessage(endpoint, data, contentType);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception(await GetError(response));
            }
        }
        public async Task<string> PostRestRefreshToken(string endpoint, HttpContent data, string contentType = "application/json")
        {
            HttpResponseMessage response = await PostResponseMessageRefreshToken(endpoint, data, contentType);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception(await GetError(response));
            }
        }
        public async Task<string> PutRest(string endpoint, HttpContent data, string contentType = "application/json")
        {
            HttpResponseMessage response = await PutResponseMessage(endpoint, data, contentType);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception(await GetError(response));
            }
        }
        public async Task<string> DeleteRestJson(string endpoint, string contentType = "application/json")
        {
            HttpResponseMessage response = await DeleteResponseMessage(endpoint, contentType);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception(await GetError(response));
            }
        }
        #region Private Methods

        private async Task<string> GetError(HttpResponseMessage response)
        {
            var errorString = await response.Content.ReadAsStringAsync();
            var errorCode = JsonConvert.DeserializeObject<ErrorCode>(errorString);
            string errorDescription =  errorCode.code + errorCode.error + System.Environment.NewLine + errorCode.message + errorCode.error_description;
            Err.ErrorLog(Path.Combine(Path.GetTempPath() + "AdobeSignLog-" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt"), "Error : " + errorDescription);
            return errorDescription;
        }


        private async Task<HttpResponseMessage> GetResponseMessage(string endpoint, string contentType)
        {
            endpoint = endpoint.TrimStart('/');
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(APIURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (this.TokenExpireDateTime.HasValue && this.TokenExpireDateTime < DateTime.UtcNow)
            {
                if (this.RefreshToken == null)
                    throw new Exception("Refresh Token is null.");

                RefreshAccessToken();
            }
            client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            return await client.GetAsync(endpoint);
        }


        private async void RefreshAccessToken()
        {

            AccessToken tokenDetails = new AccessToken();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("refresh_token", RefreshToken);
            parameters.Add("client_id", ClientID);
            parameters.Add("client_secret", ClientSecret);
            parameters.Add("grant_type", "refresh_token");
            FormUrlEncodedContent encodedContent = new FormUrlEncodedContent(parameters);
            string json = await PostRestRefreshToken("/oauth/refresh", encodedContent, "application/x-www-form-urlencoded");
            tokenDetails = JsonConvert.DeserializeObject<AccessToken>(json);
            tokenDetails.TokenExpireDate = DateTime.UtcNow.AddSeconds(tokenDetails.expires_in - 600);
            tokenDetails.refresh_token = RefreshToken;
            RestAPI API = new RestAPI(tokenDetails.token_type, APIURL, tokenDetails.access_token, ClientID, ClientSecret, RefreshToken, tokenDetails.TokenExpireDate);
            AdobeObject adobeObject = new AdobeObject(API);
            adobeObject.WriteTokenDetailstoTempFile(tokenDetails);
        }

        private async Task<HttpResponseMessage> PostResponseMessage(string endpoint, HttpContent contents, string contentType)
        {
            endpoint = endpoint.TrimStart('/');
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(APIURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (this.TokenExpireDateTime.HasValue && this.TokenExpireDateTime < DateTime.UtcNow)
            {
                if (this.RefreshToken == null)
                    throw new Exception("Refresh Token is null.");

                RefreshAccessToken();
            }
            if (AccessToken != "")
                client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            return await client.PostAsync(endpoint, contents);
        }
        private async Task<HttpResponseMessage> PostResponseMessageRefreshToken(string endpoint, HttpContent contents, string contentType)
        {
            endpoint = endpoint.TrimStart('/');
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(APIURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            return await client.PostAsync(endpoint, contents);
        }
        private async Task<HttpResponseMessage> PutResponseMessage(string endpoint, HttpContent contents, string contentType)
        {
            endpoint = endpoint.TrimStart('/');
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(APIURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (this.TokenExpireDateTime.HasValue && this.TokenExpireDateTime < DateTime.UtcNow)
            {
                if (this.RefreshToken == null)
                    throw new Exception("Refresh Token is null.");

                RefreshAccessToken();
            }
            if (AccessToken != "")
                client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            return await client.PutAsync(endpoint, contents);
        }

        private async Task<HttpResponseMessage> DeleteResponseMessage(string endpoint, string contentType)
        {
            endpoint = endpoint.TrimStart('/');
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(APIURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (this.TokenExpireDateTime.HasValue && this.TokenExpireDateTime < DateTime.UtcNow)
            {
                if (this.RefreshToken == null)
                    throw new Exception("Refresh Token is null.");

                RefreshAccessToken();
            }
            client.DefaultRequestHeaders.Add("Authorization", AccessToken);
            return await client.DeleteAsync(endpoint);
        }

        #endregion Private Methods


        #region JSON Methods

        internal string SerializeJSon<T>(T t)
        {
            string jsonString = string.Empty;

            DataContractJsonSerializerSettings a = new DataContractJsonSerializerSettings();


            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings()
            {

                DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ss-f:ff"),
                UseSimpleDictionaryFormat = true,

            };

            using (MemoryStream stream = new MemoryStream())
            {
                DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T), settings);
                ds.WriteObject(stream, t);
                byte[] data = stream.ToArray();
                jsonString = Encoding.UTF8.GetString(data, 0, data.Length);
            }

            return jsonString;
        }

        internal T DeserializeJSon<T>(string jsonString)
        {
            T obj;
            dynamic dT = typeof(T);

            if (dT.Name.EndsWith("List"))
                dT = dT.DeclaredProperties[0].PropertyType.GenericTypeArguments[0];

            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings()
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ss-f:ff"),
                UseSimpleDictionaryFormat = true
            };

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), settings);
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                obj = (T)ser.ReadObject(stream);
            }

            return obj;
        }

        #endregion JSON Methods




    }
}
