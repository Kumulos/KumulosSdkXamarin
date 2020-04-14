using System;
using UserNotifications;


namespace Com.Kumulos
{
    public class NotificationServiceExtension
    {
        public void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            iOS.KumulosNotificationService.DidReceiveNotificationRequest(request, contentHandler);
           
        }
    }
}
