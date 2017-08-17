using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kumulos
{
    public class KumulosBuild
    {
        private string apiKey;

        public KumulosBuild(string apiKey)
        {
            this.apiKey = apiKey;
        }

        private async Task<JObject> MakeRPCApiCallAsync(string methodName, List<KeyValuePair<string, string>> parameters)
        {
            var uri = new Uri(string.Format("https://api.kumulos.com/b2.2/{0}/{1}.json", apiKey, methodName));
            var postContent = new FormUrlEncodedContent(parameters);

            HttpResponseMessage request = await KumulosSDK.GetHttpClient().PostAsync(uri, postContent);

            if (request.IsSuccessStatusCode)
            {
                
                var responseContent = request.Content.ReadAsStringAsync().Result;
                JObject stuff = (JObject)JsonConvert.DeserializeObject(responseContent);

                return stuff;
            }

            return null;
        }

        public async Task<ApiResponse> CallAPI(string methodName, List<KeyValuePair<string, string>> parameters)
        {

            ApiResponse response = new ApiResponse();
            var result = await MakeRPCApiCallAsync(methodName, parameters);

            response.responseCode = (int)result["responseCode"];
            response.responseMessage = (string)result["responseMessage"];
            response.payload = (object)result["payload"];

            return response;
        }
    }
}
