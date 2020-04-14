using System;
using UserNotifications;


namespace Com.Kumulos
{
    public class NotificationServiceExtension
    {
        static readonly Lazy<NotificationServiceExtension> extension = new Lazy<NotificationServiceExtension>(Create);

        public static NotificationServiceExtension Current
        {
            get
            {
                return extension.Value;
            }

        }

        public void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            iOS.KumulosNotificationService.DidReceiveNotificationRequest(request, contentHandler);
        }

        static NotificationServiceExtension Create()
        {
            return new NotificationServiceExtension();
        }
    }
}
