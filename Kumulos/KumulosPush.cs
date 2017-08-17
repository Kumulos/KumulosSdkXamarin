using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kumulos
{
    public class KumulosPush
    {
        PushChannels channels;

        public KumulosPush()
        {
            channels = new PushChannels();
        }

        public PushChannels Channels { get { return channels; } }

        public enum ApnsMode : int
        {
            Development,
            Production,
            Wildcard
        };

        public void RegisterDeviceToken(IRegisterDeviceToken request)
        {
            var uri = string.Format("https://push.kumulos.com/v1/app-installs/{0}/push-token", KumulosSDK.InstallId);
            var content = new StringContent(request.getRequestPayload().ToString(), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Content = content;

            KumulosSDK.GetHttpClient().SendAsync(requestMessage);
        }

        public void TrackPushOpen(string notificationId)
        {
            var uri = string.Format("https://push.kumulos.com/v1/app-installs/{0}/opens", KumulosSDK.InstallId);

            JObject payload = new JObject();
            payload.Add("id", notificationId);

            var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Content = content;

            KumulosSDK.GetHttpClient().SendAsync(requestMessage);

        }
    }
}
