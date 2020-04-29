using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

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
            var uri = new Uri(string.Format("{0}/b2.2/{1}/{2}.json", Consts.BUILD_SERVICE_BASE_URI, apiKey, methodName));

            var postContent = BuildRequestContent(parameters);

            HttpResponseMessage request = await httpClient.PostAsync(uri, postContent);

            if (request.IsSuccessStatusCode)
            {
                var responseContent = request.Content.ReadAsStringAsync().Result;
                JObject stuff = (JObject)JsonConvert.DeserializeObject(responseContent);

                return stuff;
            }

            return null;
        }

        HttpContent BuildRequestContent(List<KeyValuePair<string, string>> parameters)
        {
            string json = JsonConvert.SerializeObject(parameters);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private void updateSessionToken(JObject response)
        {
            sessionToken = (string)response["sessionToken"];
        }
    }
}