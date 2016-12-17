using MovieBot.Contract;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace MovieBot.Utility
{
    public static class ConnectionUtility
    {
        public static string CreateGetRequest(string queryString)
        {
            string UrlRequest = "https://moviebot-rage.azurewebsites.net/api/" +
            queryString;
            return (UrlRequest);
        }
        public static WebResponse MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Method = WebRequestMethods.Http.Get;
                request.Accept = "application/json";
                HttpWebResponse response = (HttpWebResponse) request.GetResponse();
                
                if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static T deserialise<T>(WebResponse response)
        {
            if (response != null)
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonString = reader.ReadToEnd();
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                T deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
                return deserialized;
            }
            else
            {
                return default(T);
            }
        }
    }
}