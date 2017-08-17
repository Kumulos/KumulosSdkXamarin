using Android.Net;
using Android.OS;
using Android.Text;
using Newtonsoft.Json.Linq;

namespace Kumulos.Droid
{
    public class PushMessage
    {
        public const string EXTRAS_KEY = "com.kumulos.push.message";

        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public bool IsBackground { get; set; }
        public long TimeSent { get; set; }

        public JObject Data { get; set; }
        public string Uri { get; set; }

        public bool HasTitleAndMessage()
        {
            return !TextUtils.IsEmpty(Title) && !TextUtils.IsEmpty(Message);
        }

        public Android.Net.Uri GetUri()
        {
            return Android.Net.Uri.Parse(Uri);
        }
    }
}

