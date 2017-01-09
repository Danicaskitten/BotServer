using System.IO;
using System.Threading;
using System.Web;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace MovieBot.Utility.Speech
{
    [DataContract]
    public class AccessTokenInfo
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class SpeechRecognitionAuthentication
    {
        public static readonly string AccessUri = "https://oxford-speech.cloudapp.net/token/issueToken";
        private string clientId;
        private string clientSecret = "e22ecd23dee64dae833dec00feefd891";
        private string request;
        private AccessTokenInfo token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public SpeechRecognitionAuthentication(string clientId)
        {
            this.clientId = clientId;

            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope={2}",
                                              HttpUtility.UrlEncode(clientId),
                                              HttpUtility.UrlEncode(clientSecret),
                                              HttpUtility.UrlEncode("https://speech.platform.bing.com"));

            this.token = HttpPost(AccessUri, this.request);

            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }

        //Return the access token
        public AccessTokenInfo GetAccessToken()
        {
            return this.token;
        }

        //Renew the access token
        private void RenewAccessToken()
        {
            AccessTokenInfo newAccessToken = HttpPost(AccessUri, this.request);
            //swap the new token with old one
            //Note: the swap is thread unsafe
            this.token = newAccessToken;
            Console.WriteLine(string.Format("Renewed token for user: {0} is: {1}",
                              this.clientId,
                              this.token.access_token));
        }
        //Call-back when we determine the access token has expired 
        private void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                RenewAccessToken();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed renewing access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    accessTokenRenewer.Change(TimeSpan.FromMinutes(RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Failed to reschedule timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        //Helper function to get new access token
        private AccessTokenInfo HttpPost(string accessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(accessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccessTokenInfo));
                //Get deserialized object from JSON stream
                AccessTokenInfo token = (AccessTokenInfo)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}