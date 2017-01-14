using System.Threading;
using System.Runtime.Serialization;
using System;
using RestSharp;

namespace MovieBot.Utility.Speech
{
    [DataContract]
    public class @string
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

    /// <summary>
    /// This class allows the sistym to request the Authentication Token to Cognitive Service of Microsoft
    /// </summary>
    public class SpeechRecognitionAuthentication
    {
        private string subscriptionKey = "e22ecd23dee64dae833dec00feefd891";
        private string token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        /// <summary>
        /// Contructor that generate a new Token and Inizialize a Timer that renews the token when the old one expires
        /// </summary>
        /// <param name="clientID"></param>
        public SpeechRecognitionAuthentication(string clientID)
        {
            this.token = FetchToken(subscriptionKey);

            // renew the token every specfied minutes
            accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback),
                                           this,
                                           TimeSpan.FromMinutes(RefreshTokenDuration),
                                           TimeSpan.FromMilliseconds(-1));
        }
        
        public string GetAccessToken()
        {
            return this.token;
        }

        private void RenewAccessToken()
        {
            this.token = FetchToken(this.subscriptionKey);
            Console.WriteLine("Renewed token.");
        }

        /// <summary>
        /// Callback function for renewing the token
        /// </summary>
        /// <param name="stateInfo"></param>
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
                    Console.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// This method actually sends the request to the Authorization URL
        /// </summary>
        /// <param name="fetchUri"></param>
        /// <returns></returns>
        private string FetchToken(string subscriptionKey)
        {
            var client = new RestClient("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "f9bb233a-215a-4516-861f-5d3e22c35fbc");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("ocp-apim-subscription-key", "e22ecd23dee64dae833dec00feefd891");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}