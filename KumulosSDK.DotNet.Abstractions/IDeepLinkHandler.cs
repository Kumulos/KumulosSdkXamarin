using System;

namespace KumulosSDK.DotNet.Abstractions
{
    public interface IDeepLinkHandler
    {
        void Handle(DeepLinkResolution deepLinkResolution, Uri uri, DeepLink deepLink);
    }
}
