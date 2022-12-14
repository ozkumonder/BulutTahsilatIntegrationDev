using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using BulutTahsilatIntegration.WinService.Core;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace BulutTahsilatIntegration.WinService.BTIIntegration
{
    public class TigerRestService
    {
        private TigerServiceSettings serviceSettings;
        private TokenHolder tokenHolder;
        private string AccessToken
        {
            get;
            set;
        }
        public bool IsLoggedIn
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public TigerRestService()
        {
            serviceSettings = new TigerServiceSettings
            {
                ClientId = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath).RestClientId.DecryptIt(),
                ClientSecret = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath).RestClientSecret.DecryptIt(),
                ServiceUrl = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath).RestServiceUrl,
                UserName = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath).LogoUserName,
                FirmCode = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath).LogoFirmNumber,
                Password = ConfigHelper.DeserializeDatabaseConfiguration(ConfigHelper.ReadPath).LogoPassword.DecryptIt()
            };
            tokenHolder = BulutTahsilatIntegration.WinService.BTIIntegration.TokenHolder.GetInstance();
        }
        public TigerRestService(TigerServiceSettings tigerServiceSettings)
        {
            serviceSettings = new TigerServiceSettings
            {
                ClientId = tigerServiceSettings.ClientId.DecryptIt(),
                ClientSecret = tigerServiceSettings.ClientSecret.DecryptIt(),
                ServiceUrl = tigerServiceSettings.ServiceUrl,
                UserName = tigerServiceSettings.UserName,
                FirmCode = tigerServiceSettings.FirmCode,
                Password = tigerServiceSettings.Password.DecryptIt()
            };
            tokenHolder = BulutTahsilatIntegration.WinService.BTIIntegration.TokenHolder.GetInstance();
        }
        public static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //server sertifikasý direkt olarak onaylanýyor
            return true;
        }
        /// <summary>
        /// Logo Rest Servis Token üretir.
        /// </summary>
        /// <returns></returns>
        public TokenHolder GetAccessToken()
        {
            string token = null;
            var url = string.Format("{0}/token", this.serviceSettings.ServiceUrl);
            var req = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Accept = "application/json";
            req.Headers.Add("Authorization", string.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(this.serviceSettings.ClientId, ":", this.serviceSettings.ClientSecret)))));
            req.Credentials = CredentialCache.DefaultCredentials;

            //sertifikanýn validate edildiði fonksiyonumuza yönlendiriyoruz
            ServicePointManager.ServerCertificateValidationCallback
                += new System.Net.Security.RemoteCertificateValidationCallback(ValidateCertificate);
            byte[] formData = Encoding.UTF8.GetBytes(string.Concat(new string[] { "grant_type=password&username=", this.serviceSettings.UserName, "&firmno=", this.serviceSettings.FirmCode, "&password=", this.serviceSettings.Password }));
            req.ContentLength = formData.Length;

            try
            {
                using (Stream post = req.GetRequestStream())
                {
                    post.Write(formData, 0, formData.Length);
                }
                using (var resp = req.GetResponse() as HttpWebResponse)
                {
                    token = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                }

                JObject jObject = JObject.Parse(token);
                this.AccessToken = jObject["access_token"].ToString();
                this.tokenHolder.ExpireSeconds = Convert.ToInt32(jObject["expires_in"]);
                this.tokenHolder.Token = this.AccessToken;
                this.tokenHolder.IsLoggedIn = true;
                DateTimeOffset now = DateTimeOffset.Now;
                tokenHolder.ValidUntil = now.AddSeconds((this.tokenHolder.ExpireSeconds - 100));
            }
            catch (WebException webException)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(webException.Message, " ", webException.InnerException)));
                this.tokenHolder.Token = webException.Message;
                this.tokenHolder.IsLoggedIn = false;
                
            }
            catch (Exception exception)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(exception.Message, " ", exception.InnerException)));
            }

            return tokenHolder;
        }
        /// <summary>
        /// Prametre verilen firma no ile login olunur.
        /// </summary>
        /// <param name="firmNo"></param>
        /// <returns></returns>
        public TokenHolder GetAccessToken(string firmNo)
        {
            try
            {
                string token = null;
                var url = string.Format("{0}/token", this.serviceSettings.ServiceUrl);
                var req = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Accept = "application/json";
                req.Headers.Add("Authorization", string.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(this.serviceSettings.ClientId, ":", this.serviceSettings.ClientSecret)))));
                byte[] formData = Encoding.UTF8.GetBytes(string.Concat(new string[] { "grant_type=password&username=", this.serviceSettings.UserName, "&firmno=", firmNo, "&password=", this.serviceSettings.Password }));
                req.ContentLength = formData.Length;
                using (Stream post = req.GetRequestStream())
                {
                    post.Write(formData, 0, formData.Length);
                }
                using (var resp = req.GetResponse() as HttpWebResponse)
                {
                    token = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                }
                JObject jObject = JObject.Parse(token);
                this.AccessToken = jObject["access_token"].ToString();
                this.tokenHolder.ExpireSeconds = Convert.ToInt32(jObject["expires_in"]);
                this.tokenHolder.Token = this.AccessToken;
                this.tokenHolder.IsLoggedIn = true;
                DateTimeOffset now = DateTimeOffset.Now;
                tokenHolder.ValidUntil = now.AddSeconds((this.tokenHolder.ExpireSeconds - 100));
            }
            catch (WebException webException)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(webException.Message, " ", webException.InnerException)));
                this.tokenHolder.Token = webException.Message;
                this.tokenHolder.IsLoggedIn = false;
            }
            catch (Exception exception)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(exception.Message, " ", exception.InnerException)));
            }

            return tokenHolder;
        }
        public TokenHolder GetAccessToken(string userName, string password, string firmNo)
        {
            try
            {
                string token = null;
                var url = string.Format("{0}/token", this.serviceSettings.ServiceUrl);
                var req = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                req.Method = "POST";
                req.ContentType = "application/json";
                req.Accept = "application/json";
                req.Headers.Add("Authorization", string.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(this.serviceSettings.ClientId, ":", this.serviceSettings.ClientSecret)))));
                byte[] formData = Encoding.UTF8.GetBytes(string.Concat(new string[] { "grant_type=password&username=", userName, "&firmno=", firmNo, "&password=", password }));
                req.ContentLength = formData.Length;
                using (Stream post = req.GetRequestStream())
                {
                    post.Write(formData, 0, formData.Length);
                }
                using (var resp = req.GetResponse() as HttpWebResponse)
                {
                    token = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                }
                JObject jObject = JObject.Parse(token);
                this.AccessToken = jObject["access_token"].ToString();
                this.tokenHolder.ExpireSeconds = Convert.ToInt32(jObject["expires_in"]);
                this.tokenHolder.Token = this.AccessToken;
                this.tokenHolder.IsLoggedIn = true;
                DateTimeOffset now = DateTimeOffset.Now;
                tokenHolder.ValidUntil = now.AddSeconds((this.tokenHolder.ExpireSeconds - 100));
            }
            catch (WebException webException)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(webException.Message, " ", webException.InnerException)));
                this.tokenHolder.Token = webException.Message;
                this.tokenHolder.IsLoggedIn = false;
            }
            catch (Exception exception)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(exception.Message, " ", exception.InnerException)));
            }

            return tokenHolder;
        }

        public TokenHolder GetAccessTokenWithRestSharp()
        {
            try
            {
                var url = string.Format("{0}/token", this.serviceSettings.ServiceUrl);
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", string.Concat("Basic ", Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(this.serviceSettings.ClientId, ":", this.serviceSettings.ClientSecret)))));
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json");
                request.AddParameter("application/json", string.Concat(new string[] { "grant_type=password&username=", this.serviceSettings.UserName, "&firmno=", this.serviceSettings.FirmCode, "&password=", this.serviceSettings.Password }), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);

                JObject jObject = JObject.Parse(response.Content);
                this.AccessToken = jObject["access_token"].ToString();
                this.tokenHolder.ExpireSeconds = Convert.ToInt32(jObject["expires_in"]);
                this.tokenHolder.Token = this.AccessToken;
                this.tokenHolder.IsLoggedIn = true;
                DateTimeOffset now = DateTimeOffset.Now;
                tokenHolder.ValidUntil = now.AddSeconds((this.tokenHolder.ExpireSeconds - 100));
            }
            catch (WebException webException)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(webException.Message, " ", webException.InnerException)));
                this.tokenHolder.Token = webException.Message;
                this.tokenHolder.IsLoggedIn = false;
            }
            catch (Exception exception)
            {
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(exception.Message, " ", exception.InnerException)));
            }

            return tokenHolder;
        }
        public JToken HttpPost(string urlParameters, string data)
        {
            JToken jResult;
            string url = string.Format("{0}/{1}", this.serviceSettings.ServiceUrl, urlParameters);
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            request.Timeout = 1080000000;
            request.ContinueTimeout = 1080000000;
            request.ReadWriteTimeout = 1080000000;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", string.Concat("Bearer  ", AccessToken));

            byte[] formData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = formData.Length;
            using (Stream post = request.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }
            try
            {
                using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                {
                    var reader = new StreamReader(resp.GetResponseStream());
                    jResult = JToken.Parse(reader.ReadToEnd());
                }
            }
            catch (WebException webException)
            {
                //throw this.GetErrorFromWebException(webException);
                var response = ((HttpWebResponse)webException.Response);
                var content = new StreamReader(response.GetResponseStream());
                try
                {
                    jResult = content.ReadToEnd();
                }
                catch (Exception exception)
                {
                    jResult = exception.Message;
                    LogHelper.LogError(exception.Message);
                }
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(this.serviceSettings.ServiceUrl, urlParameters, webException), data, "TigerRestService.HttpPost"));
            }

            return jResult;
        }
        public JToken HttpPost(string urlParameters, string data, string accessToken)
        {
            //_logger.Log(LogLevel.Information, "<-------------------------Start HttpPost------------------------------------>");

            JToken jResult;
            string url = string.Format("{0}/{1}", this.serviceSettings.ServiceUrl, urlParameters);
            //_logger.Log(LogLevel.Information, string.Concat(url, "için Post request gönderildi..."));
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            request.Timeout = 1080000000;
            request.ContinueTimeout = 1080000000;
            request.ReadWriteTimeout = 1080000000;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", string.Concat("Bearer  ", accessToken));

            byte[] formData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = formData.Length;
            using (Stream post = request.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }
            try
            {
                // _logger.Log(LogLevel.Information, string.Concat(url, "'e gönderilen request için response çaðýrýldý... "));
                using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                {
                    var reader = new StreamReader(resp.GetResponseStream());
                    jResult = JToken.Parse(reader.ReadToEnd());
                    //_logger.Log(LogLevel.Information, string.Concat(url, "'e gönderilen request için response baþarýlý... ", jResult.ToString()));
                }
            }
            catch (WebException webException)
            {
                //throw this.GetErrorFromWebException(webException);
                try
                {
                    //_logger.Log(LogLevel.Error, string.Concat("WebException", webException.Message));
                    //var response = ((HttpWebResponse)webException.Response);
                    //var content = new StreamReader(response.GetResponseStream());
                    //jResult = content.ReadToEnd();
                    //_logger.Log(LogLevel.Error, string.Concat("WebException:", jResult.ToString()));
                    //jResult = webException.Message;
                    var response = ((HttpWebResponse)webException.Response);
                    StreamReader content = new StreamReader(response.GetResponseStream());
                    jResult = content.ReadToEnd();
                }
                catch (Exception exception)
                {
                    jResult = exception.Message;
                }
                // _logger.Log(LogLevel.Error, string.Concat(" ", GetType(), MethodBase.GetCurrentMethod()?.Name, string.Concat(this.serviceSettings.ServiceUrl, urlParameters, webException), data, "TigerRestService.HttpPost"));
            }
            LogHelper.Log( "<-------------------------Finish HttpPost------------------------------------>");
            return jResult;
        }
        public JToken HttpPatch(string urlParameters, int? dataReference, string data)
        {
            JToken jResult;
            string url = string.Concat(this.serviceSettings.ServiceUrl, "/", urlParameters, "/", dataReference);
            try
            {
                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.Timeout = 1080000000;
                request.ContinueTimeout = 1080000000;
                request.ReadWriteTimeout = 1080000000;
                request.Method = "PATCH";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Headers.Add("Authorization", "Bearer  " + AccessToken);

                byte[] formData = UTF8Encoding.UTF8.GetBytes(data);
                request.ContentLength = formData.Length;

                using (Stream post = request.GetRequestStream())
                {
                    post.Write(formData, 0, formData.Length);
                }

                using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    jResult = JToken.Parse(reader.ReadToEnd());
                }
            }
            catch (WebException webEx)
            {
                var response = ((HttpWebResponse)webEx.Response);
                StreamReader content = new StreamReader(response.GetResponseStream());
                jResult = content.ReadToEnd();
            }
            catch (Exception ex)
            {
                jResult = ex.Message.ToString();
            }
            return jResult;
        }
        public JToken HttpPut(string urlParameters, int? dataReference, string data)
        {
            JToken jTokens = null;
            string url = string.Format("{0}/{1}/{2}", this.serviceSettings.ServiceUrl, urlParameters, dataReference);
            HttpWebRequest req = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            req.Timeout = 1080000000;
            req.ContinueTimeout = 1080000000;
            req.ReadWriteTimeout = 1080000000;
            req.Method = "PUT";
            req.ContentType = "application/json";
            req.Accept = "application/json";
            req.Headers.Add("Authorization", string.Concat("Bearer  ", this.AccessToken));
            byte[] formData = Encoding.UTF8.GetBytes(data);
            req.ContentLength = (long)((int?)formData.Length);
            using (Stream post = req.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }
            try
            {
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(resp.GetResponseStream());
                    JToken jResult = JToken.Parse(reader.ReadToEnd());

                    jTokens = jResult;
                }
            }
            catch (WebException webEx)
            {
                var response = ((HttpWebResponse)webEx.Response);
                StreamReader content = new StreamReader(response.GetResponseStream());
                jTokens = content.ReadToEnd();
            }
            catch (Exception ex)
            {
                jTokens = ex.Message.ToString();
            }
            return jTokens;


        }
        public JToken HttpRequest(string urlParameters, string method = null)
        {
            JToken jTokens = null;
            string url = string.Format("{0}/{1}", this.serviceSettings.ServiceUrl, urlParameters);
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            req.Timeout = 1080000000;
            req.ContinueTimeout = 1080000000;
            req.ReadWriteTimeout = 1080000000;
            req.Method = (method == null ? "GET" : method);
            req.Accept = "application/json, application/octet-stream";
            req.Headers.Add("Authorization", string.Concat("Bearer  ", this.AccessToken));

            try
            {
                string result = null;
                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    var stream = resp.GetResponseStream();
                    result = new StreamReader(stream).ReadToEnd();
                }
                if (string.IsNullOrEmpty(result))
                {
                    jTokens = null;
                }
                else
                {
                    jTokens = JToken.Parse(result);
                }
            }
            catch (WebException webException)
            {
                var response = ((HttpWebResponse)webException.Response);
                var errorContent = new StreamReader(response.GetResponseStream()).ReadToEnd();
                LogHelper.LogError(string.Concat("ERROR: ", GetType(), MethodBase.GetCurrentMethod().Name, string.Concat(webException.Message, " ", webException.InnerException), errorContent));
                jTokens = errorContent;
            }
            return jTokens;
        }
        private Exception GetErrorFromWebException(WebException ex)
        {
            string end = null;
            string str = null;
            if (ex.Response != null)
            {
                using (Stream responseStream = ex.Response.GetResponseStream())
                {
                    using (StreamReader streamReader = new StreamReader(responseStream))
                    {
                        end = streamReader.ReadToEnd();
                        JObject jObject = null;
                        try
                        {
                            jObject = JObject.Parse(end);
                        }
                        catch (Exception exception)
                        {
                            LogHelper.LogError(exception.Message);
                        }
                        if (jObject != null)
                        {
                            JToken ýtem = jObject["error"];
                            if (ýtem != null)
                            {

                            }
                            JToken jToken = jObject["error_description"];
                            if (jToken != null)
                            {
                            }
                            JToken ýtem1 = jObject["Message"];
                            if (ýtem1 != null)
                            {
                            }
                            JToken jToken1 = jObject["ModelState"];
                            if (jToken1 != null)
                            {
                                foreach (KeyValuePair<string, JToken> keyValuePair in (JObject)jToken1)
                                {
                                    if (keyValuePair.Value != null)
                                    {
                                        if (!(keyValuePair.Value is JArray))
                                        {
                                        }
                                        else
                                        {
                                            string empty = string.Empty;
                                            if (!keyValuePair.Key.StartsWith("ValError"))
                                            {
                                                empty = (!keyValuePair.Key.StartsWith("restRecord.") ? keyValuePair.Key : keyValuePair.Key.Substring(11));
                                            }
                                            if (keyValuePair.Value.HasValues)
                                            {
                                                if (!string.IsNullOrWhiteSpace(empty))
                                                {
                                                    empty = string.Concat(" (", empty, ")");
                                                }
                                                foreach (JToken jToken2 in Extensions.Values(keyValuePair.Value))
                                                {
                                                    //criticalUserLevelException.get_Details().Add(string.Concat(jToken2.ToString(), empty));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ex;
        }
    }
}