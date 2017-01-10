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
using System.Threading.Tasks;
using System.Net.Http;
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

    public class SpeechRecognitionAuthentication
    {
        public static readonly string FetchTokenUri = "https://api.cognitive.microsoft.com/sts/v1.0";
        private string subscriptionKey = "e22ecd23dee64dae833dec00feefd891";
        private string token;
        private Timer accessTokenRenewer;

        //Access token expires every 10 minutes. Renew it every 9 minutes only.
        private const int RefreshTokenDuration = 9;

        public SpeechRecognitionAuthentication(string clientID)
        {
            this.token = FetchToken(FetchTokenUri, subscriptionKey);

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
            this.token = FetchToken(FetchTokenUri, this.subscriptionKey);
            Console.WriteLine("Renewed token.");
        }

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

        private string FetchToken(string fetchUri, string subscriptionKey)
        {
            var client = new RestClient("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
            var request = new RestRequest(Method.POST);
            request.AddHeader("postman-token", "f9bb233a-215a-4516-861f-5d3e22c35fbc");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("ocp-apim-subscription-key", "e22ecd23dee64dae833dec00feefd891");
            IRestResponse response = client.Execute(request);
            return response.Content;

            /*            using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                            UriBuilder uriBuilder = new UriBuilder(fetchUri+ "/issueToken");
                            HttpContent content = new StringContent("");

                            var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, content);
                            return await result.Content.ReadAsStringAsync();
                        }*/
        }
    }
}