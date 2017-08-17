using System;
using Foundation;
using Kumulos;
using Newtonsoft.Json.Linq;
using UIKit;
using UserNotifications;

namespace Kumulos.iOS
{
    public static class Push
    {
        public static void RegisterForRemoteNotifications()
        {
            var center = UNUserNotificationCenter.Current;
            center.RequestAuthorization(
            UNAuthorizationOptions.Badge,
            (bool arg1, Foundation.NSError arg2) =>
            {

            });

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        public static void RegisterDeviceToken(NSData deviceToken)
        {
            var deviceTokenString = deviceToken.ToString().Replace("<", "").Replace(">", "").Replace(" ", "");

            var request = new RegisterDeviceToken(deviceTokenString, GetAPNSMode());
            KumulosSDK.Push.RegisterDeviceToken(request);
        }

        private static KumulosPush.ApnsMode GetAPNSMode()
        {
            string mobileProvision = NSBundle.MainBundle.PathForResource("embedded", "mobileprovision");
            string content = System.IO.File.ReadAllText(mobileProvision);

            int start = content.IndexOf("<key>aps-environment</key>");

            string endContent = content.Substring(start);

            int end = endContent.IndexOf("</dict>");


            string parsed = endContent.Substring(0, end);

            return KumulosPush.ApnsMode.Production;
        }
    }

    internal class RegisterDeviceToken : IRegisterDeviceToken
    {
        string deviceToken;
        KumulosPush.ApnsMode mode;

        public RegisterDeviceToken(string deviceToken, KumulosPush.ApnsMode mode)
        {
            this.deviceToken = deviceToken;
            this.mode = mode;
        }

        public JObject getRequestPayload()
        {
            JObject payload = new JObject();
            payload.Add("token", deviceToken);

            payload.Add("type", 1);
            payload.Add("iosTokenType", mode.ToString());

            return payload;
        }
    }
}
