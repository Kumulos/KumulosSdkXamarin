using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Com.Kumulos.Abstractions
{
    public class PushChannels
    {
        private readonly HttpClient httpClient;
        private readonly string installId;

        public PushChannels(string installId, HttpClient httpClient)
        {
            this.installId = installId;
            this.httpClient = httpClient;
        }

       public async Task<object> ListChannels()
        {
            var uri = new Uri(string.Format("https://push.kumulos.com/v1/app-installs/{0}/channels", installId));

            HttpResponseMessage request = await httpClient.GetAsync(uri);

            if (request.IsSuccessStatusCode)
            {
                var responseContent = request.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(responseContent);

            }

            return null;
        }

        public async Task<object> CreateChannel(string uuid, bool subscribe, string name, bool showInPortal, Dictionary<string, object> meta)
        {
            var uri = new Uri("https://push.kumulos.com/v1/channels");

            JObject payload = new JObject();
            payload.Add("uuid", uuid);
            payload.Add("showInPortal", showInPortal);

            payload.Add("name", name);


            if (subscribe == true)
            {
                payload.Add("installId", installId);
            }

            var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Content = content;

            HttpResponseMessage request = await httpClient.SendAsync(requestMessage);

            if (request.IsSuccessStatusCode)
            {
                var responseContent = request.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(responseContent);
            }

            return null;
        }

        public async Task<object> Subscribe(string[] uuids)
        {

            return await MakeSubscriptionNetworkRequest(HttpMethod.Post, JsonConvert.SerializeObject(uuids));
        }

        public async Task<object> Unsubscribe(string[] uuids)
        {
            return await MakeSubscriptionNetworkRequest(HttpMethod.Delete, JsonConvert.SerializeObject(uuids));
        }

        public async Task<object> SetSubscriptions(string[] uuids)
        {
            return await MakeSubscriptionNetworkRequest(HttpMethod.Delete, JsonConvert.SerializeObject(uuids));
        }

        public async Task<object> ClearSubscriptions()
        {
            return await MakeSubscriptionNetworkRequest(HttpMethod.Delete, "");
        }

        private async Task<object> MakeSubscriptionNetworkRequest(HttpMethod method, string payload)
        {
            var uri = new Uri(string.Format("https://push.kumulos.com/v1/app-installs/{0}/channels/subscriptions", installId));
            var requestMessage = new HttpRequestMessage(method, uri);

            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            HttpResponseMessage request = await httpClient.SendAsync(requestMessage);
            if (request.IsSuccessStatusCode)
            {
                var responseContent = request.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(responseContent);
            }

            return null;
        }
    }
}