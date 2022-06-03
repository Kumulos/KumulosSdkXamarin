using System;

namespace Com.Kumulos.Abstractions
{
    public interface IDeepLinkHandler
    {
        void Handle(DeepLinkResolution deepLinkResolution, Uri uri, DeepLink deepLink);
    }
}
