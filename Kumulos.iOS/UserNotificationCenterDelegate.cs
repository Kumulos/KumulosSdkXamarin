using System;
using UserNotifications;
using Foundation;
using UIKit;

namespace Kumulos.iOS
{

    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        #region Constructors
        public UserNotificationCenterDelegate()
        {
        }
        #endregion

        #region Override Methods
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
            NSDictionary custom = (NSDictionary)response.Notification.Request.Content.UserInfo["custom"];
            string id = custom["i"].ToString();

            KumulosSDK.Push.TrackPushOpen(id);
        }


		#endregion
	}
}

