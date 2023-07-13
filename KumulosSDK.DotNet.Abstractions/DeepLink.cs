using System;
using Newtonsoft.Json.Linq;

namespace KumulosSDK.DotNet.Abstractions
{
    public class DeepLink
    {
        public DeepLink(Uri uri, DeepLinkContent content, JObject data)
        {
            Uri = uri;
            Data = data;
            Content = content;
        }

        public Uri Uri { get; }
        public DeepLinkContent Content { get; }
        public JObject Data { get; }
    }
}
