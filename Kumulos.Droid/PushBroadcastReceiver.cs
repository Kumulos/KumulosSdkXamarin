using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using Newtonsoft.Json.Linq;
using Android.Net;
using Java.Lang;
using Android.Runtime;
using Newtonsoft.Json;

namespace Kumulos.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { ACTION_PUSH_RECEIVED, ACTION_PUSH_OPENED })]
    public class PushBroadcastReceiver : BroadcastReceiver
    {
        public const string TAG = "com.kumulos.push.PUSH_BROADCAST_RECEIVER";

        public const string ACTION_PUSH_RECEIVED = "com.kumulos.push.RECEIVED";
        public const string ACTION_PUSH_OPENED = "com.kumulos.push.OPENED";

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;
            PushMessage pushMessage = JsonConvert.DeserializeObject<PushMessage>(intent.GetStringExtra(PushMessage.EXTRAS_KEY));

            switch (action)
            {
                case ACTION_PUSH_RECEIVED:
                    OnPushReceived(context, pushMessage);
                    break;
                case ACTION_PUSH_OPENED:
                    OnPushOpened(context, pushMessage);
                    break;
            }
        }

        /**
		 * Handles showing a notification in the notification drawer when a content push is received.
		 *
		 * @param context
		 * @param pushMessage
		 * @see PushBroadcastReceiver#buildNotification(Context, PushMessage) for customization
		 */
        protected void OnPushReceived(Context context, PushMessage pushMessage)
        {
            Log.Info(TAG, "Push received");

            if (pushMessage.IsBackground)
            {
                Intent serviceIntent = GetBackgroundPushServiceIntent(context, pushMessage);

                if (null == serviceIntent)
                {
                    return;
                }

                context.StartService(serviceIntent);
                return;
            }
            else if (!pushMessage.HasTitleAndMessage())
            {
                // Non-background pushes should always have a title & message otherwise we can't show a notification
                return;
            }

            Notification notification = BuildNotification(context, pushMessage);

            if (null == notification)
            {
                return;
            }

            NotificationManager notificationManager =
                    (NotificationManager)context.GetSystemService(Context.NotificationService);

            // TODO fix this in 2038 when we run out of time
            notificationManager.Notify((int)pushMessage.TimeSent, notification);
        }

        /**
		 * Handles launching the Activity specified by the {#getPushOpenActivityIntent} method when a push
		 * notification is opened from the notifications drawer.
		 *
		 * @param context
		 * @param pushMessage
		 * @see PushBroadcastReceiver#getPushOpenActivityIntent(Context, PushMessage) for customization
		 */
        protected void OnPushOpened(Context context, PushMessage pushMessage)
        {
            Log.Info(TAG, "Push opened");

            KumulosSDK.Push.TrackPushOpen(pushMessage.Id);

            Intent launchIntent = GetPushOpenActivityIntent(context, pushMessage);

            if (null == launchIntent)
            {
                return;
            }


            ComponentName component = launchIntent.Component;

            if (null != pushMessage.GetUri())
            {
                launchIntent = new Intent(Intent.ActionView, pushMessage.GetUri());
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                TaskStackBuilder taskStackBuilder = TaskStackBuilder.Create(context);
                taskStackBuilder.AddParentStack(component);
                taskStackBuilder.AddNextIntent(launchIntent);

                taskStackBuilder.StartActivities();
                return;
            }

            launchIntent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
            context.StartActivity(launchIntent);
        }

        /**
		 * Builds the notification shown in the notification drawer when a content push is received.
		 * <p/>
		 * Defaults to using the application's icon.
		 * <p/>
		 * Override to customize the notification shown.
		 *
		 * @param context
		 * @param pushMessage
		 * @return
		 * @see Kumulos#pushTrackOpen(String) for correctly tracking conversions if you customize the content intent
		 */
        protected Notification BuildNotification(Context context, PushMessage pushMessage)
        {
            Intent openIntent = new Intent(ACTION_PUSH_OPENED);
            openIntent.PutExtra(PushMessage.EXTRAS_KEY, JsonConvert.SerializeObject(pushMessage));
            openIntent.SetPackage(context.PackageName);

            PendingIntent pendingOpenIntent = PendingIntent.GetBroadcast(
                    context,
                    (int)pushMessage.TimeSent,
                    openIntent,
                    PendingIntentFlags.UpdateCurrent | PendingIntentFlags.OneShot);

            Notification.Builder notificationBuilder = new Notification.Builder(context);


            //KumulosConfig config = Kumulos.getConfig();
            //int icon = config != null ? config.getNotificationSmallIconId() : Kumulos.DEFAULT_NOTIFICATION_ICON_ID;

            notificationBuilder
                .SetSmallIcon(Resource.Drawable.common_google_signin_btn_icon_light_focused)
                .SetContentTitle(pushMessage.Title)
                .SetContentText(pushMessage.Message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingOpenIntent);

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBean)
            {
                return notificationBuilder.Build();
            }

            return notificationBuilder.Notification;
        }

        /**
		 * Returns the Intent to launch when a push notification is opened from the notification drawer.
		 * <p/>
		 * The Intent must specify an Activity component or it will be ignored.
		 * <p/>
		 * Override to change the launched Activity when a push notification is opened.
		 *
		 * @param context
		 * @param pushMessage
		 * @return
		 */
        protected Intent GetPushOpenActivityIntent(Context context, PushMessage pushMessage)
        {
            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            launchIntent.PutExtra(PushMessage.EXTRAS_KEY, JsonConvert.SerializeObject(pushMessage));
            return launchIntent;
        }

        /**
		 * If you want a service started when a background data push is received, override this method.
		 * <p/>
		 * The intent must specify a Service component or it will be ignored.
		 * <p/>
		 * Return null to silently ignore the data push. This is the default behaviour.
		 *
		 * @param context
		 * @param pushMessage
		 * @return
		 */
        protected Intent GetBackgroundPushServiceIntent(Context context, PushMessage pushMessage)
        {
            return null;
        }

    }
}
