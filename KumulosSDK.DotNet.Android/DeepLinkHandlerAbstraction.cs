using System;
using Android.Content;
using KumulosSDK.DotNet.Abstractions;
using Newtonsoft.Json.Linq;

namespace KumulosSDK.DotNet.Android
{
    public class DeepLinkHandlerAbstraction : Java.Lang.Object, Com.Kumulos.Android.IDeferredDeepLinkHandlerInterface
    {
        private IDeepLinkHandler handler;

        public DeepLinkHandlerAbstraction(IDeepLinkHandler handler)
        {
            this.handler = handler;
        }

        void Com.Kumulos.Android.IDeferredDeepLinkHandlerInterface.Handle(Context context, Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution resolution, string link, Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLink deepLink)
        {
            handler.Handle(MapResolution(resolution), new Uri(link), MapDeepLink(deepLink));
        }

        private DeepLinkResolution MapResolution(Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution r)
        {
            if (r == Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkExpired)
            {
                return DeepLinkResolution.LinkExpired;
            }

            if (r == Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkLimitExceeded)
            {
                return DeepLinkResolution.LinkLimitExceeded;
            }

            if (r == Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkMatched)
            {
                return DeepLinkResolution.LinkMatched;
            }

            if (r == Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkNotFound)
            {
                return DeepLinkResolution.LinkNotFound;
            }

            if (r == Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkResolution.LookupFailed)
            {
                return DeepLinkResolution.LookupFailed;
            }

            throw new Exception("Failed to map DeepLinkResolution");
        }

        private DeepLink MapDeepLink(Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLink d)
        {
            if (d == null)
            {
                return null;
            }
            return new DeepLink(new Uri(d.Url), MapDeepLinkContent(d.Content), JObject.Parse(d.Data.ToString()));
        }

        private DeepLinkContent MapDeepLinkContent(Com.Kumulos.Android.DeferredDeepLinkHelper.DeepLinkContent c)
        {
            if (c == null)
            {
                return null;
            }
            return new DeepLinkContent(c.Title, c.Description);
        }
    }
}