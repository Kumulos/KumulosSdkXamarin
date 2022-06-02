using System;
using Android.Content;
using Com.Kumulos.Abstractions;
using Newtonsoft.Json.Linq;

namespace Com.Kumulos
{
    public class DeepLinkHandlerAbstraction : Java.Lang.Object, Android.IDeferredDeepLinkHandlerInterface
    {
        private IDeepLinkHandler handler;

        public DeepLinkHandlerAbstraction(IDeepLinkHandler handler)
        {
            this.handler = handler;
        }

        void Android.IDeferredDeepLinkHandlerInterface.Handle(Context context, Android.DeferredDeepLinkHelper.DeepLinkResolution resolution, string link, Android.DeferredDeepLinkHelper.DeepLink deepLink)
        {
            handler.Handle(MapResolution(resolution), new Uri(link), MapDeepLink(deepLink));
        }

        private DeepLinkResolution MapResolution(Android.DeferredDeepLinkHelper.DeepLinkResolution r)
        {
            if (r == Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkExpired)
            {
                return DeepLinkResolution.LinkExpired;
            }

            if (r == Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkLimitExceeded)
            {
                return DeepLinkResolution.LinkLimitExceeded;
            }

            if (r == Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkMatched)
            {
                return DeepLinkResolution.LinkMatched;
            }

            if (r == Android.DeferredDeepLinkHelper.DeepLinkResolution.LinkNotFound)
            {
                return DeepLinkResolution.LinkNotFound;
            }

            if (r == Android.DeferredDeepLinkHelper.DeepLinkResolution.LookupFailed)
            {
                return DeepLinkResolution.LookupFailed;
            }

            throw new Exception("Failed to map DeepLinkResolution");
        }

        private DeepLink MapDeepLink(Android.DeferredDeepLinkHelper.DeepLink d)
        {
            if (d == null)
            {
                return null;
            }
            return new DeepLink(new Uri(d.Url), MapDeepLinkContent(d.Content), JObject.Parse(d.Data.ToString()));
        }

        private DeepLinkContent MapDeepLinkContent(Android.DeferredDeepLinkHelper.DeepLinkContent c)
        {
            return new DeepLinkContent(c.Title, c.Description);
        }
    }
}