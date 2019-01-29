using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using Newtonsoft.Json.Linq;
using Android.Net;
using Newtonsoft.Json;

namespace Com.Kumulos.Android
{
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    public class ListenerService : GcmListenerService
    {
        const string TAG = "com.kumulos.ListenerService";

        public override void OnMessageReceived(string from, Bundle data)
        {
            base.OnMessageReceived(from, data);

            string customStr = data.GetString("custom");

            if (null == customStr)
            {
                return;
            }

            string id = string.Empty;
            JObject customData = new JObject();
            string uri = null;

            try
            {
                var customObj = JObject.Parse(customStr);

                id = customObj.GetValue("i").ToString();

                JToken urlToken;
                bool uriRead = customObj.TryGetValue("u", out urlToken);
                if (uriRead)
                {
                    uri = urlToken.ToString();
                }

                customData = (JObject)customObj.GetValue("a");
            }
            catch (MalformedJsonException)
            {
                Log.Error(TAG, "Push received had no ID/data/uri or was incorrectly formatted, ignoring...");
                return;
            }

            string bgn = data.GetString("bgn");
            bool isBackground = (null != bgn && bgn.Equals("1"));

            var pushMessage = new PushMessageImplementation
            {
                Id = id,
                Title = data.GetString("title"),
                Message = data.GetString("alert"),
                IsBackground = isBackground,
                TimeSent = data.GetLong("google.sent_time", 0L),
                Uri = uri,
                Data = customData
            };

            Intent intent = new Intent(PushBroadcastReceiverImplementation.ACTION_PUSH_RECEIVED);

            intent.SetPackage(PackageName);

            string payload = JsonConvert.SerializeObject(pushMessage);
            intent.PutExtra(PushMessageImplementation.EXTRAS_KEY, payload);

            SendBroadcast(intent);
        }
    }
}