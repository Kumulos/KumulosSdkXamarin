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
        private string sessionToken;

        public KumulosBuild(string apiKey)
        {
            this.apiKey = apiKey;
        }

        private async Task<JObject> MakeRPCApiCallAsync(string methodName, List<KeyValuePair<string, string>> parameters)
        {
            var uri = new Uri(string.Format("https://api.kumulos.com/b2.2/{0}/{1}.json", apiKey, methodName));

            var postContent = BuildRequestParameters(parameters);

            HttpResponseMessage request = await KumulosSDK.GetHttpClient().PostAsync(uri, postContent);

            if (request.IsSuccessStatusCode)
            {
                var responseContent = request.Content.ReadAsStringAsync().Result;
                JObject stuff = (JObject)JsonConvert.DeserializeObject(responseContent);

                return stuff;
            }

            return null;
        }

        FormUrlEncodedContent BuildRequestParameters(List<KeyValuePair<string, string>> parameters) {
            var postParams = new List<KeyValuePair<string, string>>{};

            foreach(KeyValuePair<string, string> parameter in parameters) {
                postParams.Add(new KeyValuePair<string, string>("params[" + parameter.Key + "]", parameter.Value));
            }

            postParams.Add(new KeyValuePair<string, string>("deviceID", KumulosSDK.InstallId));
            postParams.Add(new KeyValuePair<string, string>("installId", KumulosSDK.InstallId));

            if (sessionToken != null) {
                postParams.Add(new KeyValuePair<string, string>("sessionToken", sessionToken));
            }

            return new FormUrlEncodedContent(postParams);
        }

        public async Task<ApiResponse> CallAPI(string methodName, List<KeyValuePair<string, string>> parameters)
        {

            ApiResponse response = new ApiResponse();
            var result = await MakeRPCApiCallAsync(methodName, parameters);

            this.updateSessionToken(result);

            response.responseCode = (int)result["responseCode"];
            response.responseMessage = (string)result["responseMessage"];
            response.payload = (object)result["payload"];

            return response;
        }

        private void updateSessionToken(JObject response) {
            this.sessionToken = (string)response["sessionToken"];
        }
    }
}
