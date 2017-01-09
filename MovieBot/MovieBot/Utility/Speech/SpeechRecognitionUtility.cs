using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MovieBot.Utility.Speech
{
    public static class SpeechRecognitionUtility
    {
        public static string DoSpeechReco(Attachment attachment)
        {
            AccessTokenInfo token;
            string headerValue;
            // Note: Sign up at https://microsoft.com/cognitive to get a subscription key.  
            // Use the subscription key as Client secret below.
            SpeechRecognitionAuthentication auth = new SpeechRecognitionAuthentication("movieBot");
            string requestUri = "https://speech.platform.bing.com/recognize";

            //URI Params. Refer to the Speech API documentation for more information.
            requestUri += @"?scenarios=smd";                                // websearch is the other main option.
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";   // You must use this ID.
            requestUri += @"&locale=en-US";                                 // read docs, for other supported languages. 
            requestUri += @"&device.os=wp7";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            string host = @"speech.platform.bing.com";
            string contentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
            var wav = HttpWebRequest.Create(attachment.ContentUrl);
            string responseString = string.Empty;

            try
            {
                token = auth.GetAccessToken();
                Console.WriteLine("Token: {0}\n", token.access_token);

                //Create a header with the access_token property of the returned token
                headerValue = "Bearer " + token.access_token;
                Console.WriteLine("Request Uri: " + requestUri + Environment.NewLine);

                HttpWebRequest request = null;
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.ContentType = contentType;
                request.Headers["Authorization"] = headerValue;

                using (Stream wavStream = wav.GetResponse().GetResponseStream())
                {
                    byte[] buffer = null;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        int count = 0;
                        do
                        {
                            buffer = new byte[1024];
                            count = wavStream.Read(buffer, 0, 1024);
                            requestStream.Write(buffer, 0, count);
                        } while (wavStream.CanRead && count > 0);
                        // Flush
                        requestStream.Flush();
                    }
                    //Get the response from the service.
                    Console.WriteLine("Response:");
                    using (WebResponse response = request.GetResponse())
                    {
                        Console.WriteLine(((HttpWebResponse)response).StatusCode);
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseString = sr.ReadToEnd();
                        }
                        Console.WriteLine(responseString);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            dynamic data = JObject.Parse(responseString);
            return data.header.name;
        }
    }
}