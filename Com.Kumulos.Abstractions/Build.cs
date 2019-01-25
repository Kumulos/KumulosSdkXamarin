using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Com.Kumulos.Abstractions
{
    public class Build
    {
        private readonly HttpClient httpClient = new HttpClient();

        private readonly string installId;
        private readonly string apiKey;

        private string sessionToken = Guid.NewGuid().ToString();

        public Build(string installId, HttpClient httpClient, string apiKey)
        {
            this.installId = installId;
            this.httpClient = httpClient;
            this.apiKey = apiKey;
        }

        public async Task<ApiResponse> CallAPI(string methodName, List<KeyValuePair<string, string>> parameters)
        {
            ApiResponse response = new ApiResponse();
            var result = await MakeRPCApiCallAsync(methodName, parameters);

            updateSessionToken(result);

            response.responseCode = (int)result["responseCode"];
            response.responseMessage = (string)result["responseMessage"];
            response.payload = (object)result["payload"];

            return response;
        }

        private async Task<JObject> MakeRPCApiCallAsync(string methodName, List<KeyValuePair<string, string>> parameters)
        {
            var uri = new Uri(string.Format("https://api.kumulos.com/b2.2/{0}/{1}.json", apiKey, methodName));

            var postContent = BuildRequestParameters(parameters);

            HttpResponseMessage request = await httpClient.PostAsync(uri, postContent);

            if (request.IsSuccessStatusCode)
            {
                var responseContent = request.Content.ReadAsStringAsync().Result;
                JObject stuff = (JObject)JsonConvert.DeserializeObject(responseContent);

                return stuff;
            }

            return null;
        }

        FormUrlEncodedContent BuildRequestParameters(List<KeyValuePair<string, string>> parameters)
        {
            var postParams = new List<KeyValuePair<string, string>> { };

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                postParams.Add(new KeyValuePair<string, string>("params[" + parameter.Key + "]", parameter.Value));
            }

            postParams.Add(new KeyValuePair<string, string>("deviceID", installId));
            postParams.Add(new KeyValuePair<string, string>("installId", installId));

            if (sessionToken != null)
            {
                postParams.Add(new KeyValuePair<string, string>("sessionToken", sessionToken));
            }

            return new FormUrlEncodedContent(postParams);
        }

        private void updateSessionToken(JObject response)
        {
            sessionToken = (string)response["sessionToken"];
        }
    }
}