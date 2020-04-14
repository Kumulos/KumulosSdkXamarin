﻿using System;
using UserNotifications;


namespace Com.Kumulos
{
    public class KumulosExtension
    {
        public void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            iOS.KumulosNotificationService.DidReceiveNotificationRequest(request, contentHandler);
           
        }
    }
}
