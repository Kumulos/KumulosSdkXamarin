using System;
using UserNotifications;
using Foundation;
using UIKit;
using Com.Kumulos.Abstractions;

namespace Com.Kumulos
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        private readonly IKumulos sdkRef;

        public UserNotificationCenterDelegate(IKumulos sdk)
        {
            sdkRef = sdk;
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            NSDictionary custom = (NSDictionary)notification.Request.Content.UserInfo["custom"];
            NSString key = new NSString("u");
            if (custom.ContainsKey(key))
            {
                string url = custom["u"].ToString();
                UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(url));
            }

            // Do something with the notification
            Console.WriteLine("Active Notification: {0}", notification);

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);

        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            NSDictionary userInfo = response.Notification.Request.Content.UserInfo;
            sdkRef.TrackNotificationOpen(userInfo);
        }
    }
}