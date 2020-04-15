using System;
using Foundation;
using UserNotifications;

namespace Com.Kumulos.iOS
{
	// @interface KumulosNotificationService : NSObject
	[BaseType(typeof(NSObject))]
	interface KumulosNotificationService
	{
		// +(void)didReceiveNotificationRequest:(UNNotificationRequest * _Nonnull)request withContentHandler:(void (^ _Nonnull)(UNNotificationContent * _Nonnull))contentHandler __attribute__((availability(ios, introduced=10.0)));
		[Static]
		[Export("didReceiveNotificationRequest:withContentHandler:")]
		void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler);
	}
}