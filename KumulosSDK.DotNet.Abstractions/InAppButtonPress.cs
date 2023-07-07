using Newtonsoft.Json.Linq;

namespace KumulosSDK.DotNet.Abstractions
{
    public class InAppButtonPress
    {
        public InAppButtonPress(int messageId, JObject messageData, JObject deepLinkData)
        {
            MessageId = messageId;
            MessageData = messageData;
            DeepLinkData = deepLinkData;
        }

        public int MessageId { get; private set; }
        public JObject MessageData { get; private set; }
        public JObject DeepLinkData { get; private set; }
    }
}
