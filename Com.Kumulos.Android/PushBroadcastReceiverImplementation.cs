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
using Org.Json;

namespace Com.Kumulos.Android
{
    public class PushBroadcastReceiverImplementation : BroadcastReceiver
    {
        public const string TAG = "com.kumulos.push.PUSH_BROADCAST_RECEIVER";

        public const string ACTION_PUSH_RECEIVED = "com.kumulos.push.RECEIVED";
        public const string ACTION_PUSH_OPENED = "com.kumulos.push.OPENED";

        private const string EXTRAS_KEY_TICKLE_ID = "com.kumulos.inapp.tickle.id";
        private const int DEEP_LINK_TYPE_IN_APP = 1;

        const string DEFAULT_CHANNEL_ID = "general";
        protected const string KUMULOS_NOTIFICATION_TAG = "kumulos";

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;

            if (null == action)
            {
                return;
            }

            PushMessage message = (PushMessage)intent.GetParcelableExtra(PushMessage.ExtrasKey);

            switch (action)
            {
                case ACTION_PUSH_RECEIVED:
                    OnPushReceived(context, message);
                    break;
                case ACTION_PUSH_OPENED:
                    OnPushOpened(context, message);
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
        protected virtual void OnPushReceived(Context context, PushMessage pushMessage)
        {
            Log.Info(TAG, "Push received");

            PushTrackDelivered(context, pushMessage);

            MaybeTriggerInAppSync(context, pushMessage);

            if (pushMessage.RunBackgroundHandler())
            {
                RunBackgroundHandler(context, pushMessage);
            }
            else if (!pushMessage.HasTitleAndMessage)
            {
                // Always show Notification if has title + message
                return;
            }

            Notification notification = BuildNotification(context, pushMessage);

            if (null == notification)
            {
                return;
            }

            NotificationManager notificationManager =
                    (NotificationManager)context.GetSystemService(Context.NotificationService);

            if (null == notificationManager)
            {
                return;
            }


            notificationManager.Notify(KUMULOS_NOTIFICATION_TAG, GetNotificationId(pushMessage), notification);
        }

        protected void PushTrackDelivered(Context context, PushMessage pushMessage)
        {
            /* try
             {
                 JSONObject params = new JSONObject();
                 params.put("type", AnalyticsContract.MESSAGE_TYPE_PUSH);
                 params.put("id", pushMessage.getId());

                 Kumulos.trackEvent(context, AnalyticsContract.EVENT_TYPE_MESSAGE_DELIVERED, params);
             }
             catch (JSONException e)
             {
                 Kumulos.log(TAG, e.toString());
             }*/
        }

        protected void MaybeTriggerInAppSync(Context context, PushMessage pushMessage)
        {
            /* if (!KumulosInApp.IsInAppEnabled)
             {
                 return;
             }

            /*  int tickleId = pushMessage.getTickleId();
              if (tickleId == -1)
              {
                  return;
              }

              new Thread(new Runnable()
              {
              public void run()
              {
                  InAppMessageService.fetch(context, false);
              }
          }).start();*/
        }

        private int GetNotificationId(PushMessage pushMessage)
        {
            int tickleId = GetTickleId(pushMessage);
            if (tickleId == -1)
            {
                // TODO fix this in 2038 when we run out of time
                return (int)pushMessage.TimeSent;
            }
            return tickleId;
        }

        private int GetTickleId(PushMessage pushMessage)
        {
            JSONObject data = pushMessage.Data;
            JSONObject deepLink = data.OptJSONObject("k.deepLink");

            if (deepLink == null)
            {
                return -1;
            }

            int linkType = deepLink.OptInt("type", -1);

            if (linkType != DEEP_LINK_TYPE_IN_APP)
            {
                return -1;
            }

            try
            {
                return deepLink.GetJSONObject("data").GetInt("id");
            }
            catch (JSONException e)
            {
                Log.Info(TAG, e.ToString());
                return -1;
            }
        }

private void RunBackgroundHandler(Context context, PushMessage pushMessage)
        {
            Intent serviceIntent = GetBackgroundPushServiceIntent(context, pushMessage);

            if (null == serviceIntent)
            {
                return;
            }

            /*ComponentName component = serviceIntent.GetComponent();
            if (null == component)
            {
                Log.Info(TAG, "Service intent did not specify a component, ignoring.");
                return;
            }

           /* Class <? extends Service > cls = null;
            try
            {
                cls = (Class <? extends Service >) Class.forName(component.getClassName());
            }
            catch (ClassNotFoundException e)
            {
                Kumulos.log(TAG, "Service intent to handle a data push was provided, but it is not for a Service, check: " + component.getClassName());
            }

            if (null != cls)
            {
                context.startService(serviceIntent);
            }*/
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

            try
            {
                Android.Kumulos.PushTrackOpen(context, pushMessage.Id);
            }
            catch (Android.Kumulos.UninitializedException e)
            {
                Log.Info(TAG, "Failed to track the push opening -- Kumulos is not initialized");
            }

            Intent launchIntent = GetPushOpenActivityIntent(context, pushMessage);

            if (null == launchIntent)
            {
                return;
            }

            ComponentName component = launchIntent.Component;
            if (null == component)
            {
                Log.Info(TAG, "Intent to handle push notification open does not specify a component, ignoring. Override PushBroadcastReceiver#onPushOpened to change this behaviour.");
                return;
            }

            if (null != pushMessage.Url)
            {
                launchIntent = new Intent(Intent.ActionView, pushMessage.Url);
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
        protected virtual Notification BuildNotification(Context context, PushMessage pushMessage)
        {
            Intent openIntent = new Intent(ACTION_PUSH_OPENED);
            openIntent.PutExtra(PushMessage.ExtrasKey, pushMessage);
            openIntent.SetPackage(context.PackageName);

            PendingIntent pendingOpenIntent = PendingIntent.GetBroadcast(
                    context,
                    (int)pushMessage.TimeSent,
                    openIntent,
                    PendingIntentFlags.UpdateCurrent | PendingIntentFlags.OneShot);

            Notification.Builder notificationBuilder = new Notification.Builder(context);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                if (null == notificationManager)
                {
                    return null;
                }

                NotificationChannel channel = notificationManager.GetNotificationChannel(DEFAULT_CHANNEL_ID);
                if (null == channel)
                {
                    channel = new NotificationChannel(DEFAULT_CHANNEL_ID, "General", NotificationImportance.Default);
                    notificationManager.CreateNotificationChannel(channel);
                }

                notificationBuilder = new Notification.Builder(context, "general");
            }
            else
            {
                notificationBuilder = new Notification.Builder(context);
            }

            notificationBuilder
                .SetSmallIcon(Resource.Drawable.kumulos_ic_stat_notifications)
                .SetContentTitle(pushMessage.Title)
                .SetContentText(pushMessage.Message)
                .SetAutoCancel(true)
                .SetContentIntent(pendingOpenIntent);



            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                return notificationBuilder.Build();
            }

            return notificationBuilder.Notification;
        }

        /**
     * Used to add Kumulos extras when overriding buildNotification and providing own launch intent
     *
     * @param pushMessage
     * @param launchIntent
     */
        protected static void AddDeepLinkExtras(PushMessage pushMessage, Intent launchIntent)
        {
             /*if (!KumulosInApp.isInAppEnabled)
              {
                  return;
              }

              int tickleId = pushMessage.GetTickleId();
              if (tickleId == -1)
              {
                  return;
              }*/

              //launchIntent.putExtra(EXTRAS_KEY_TICKLE_ID, tickleId);
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
        protected virtual Intent GetPushOpenActivityIntent(Context context, PushMessage pushMessage)
        {
            
            Intent launchIntent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            if (null == launchIntent) { return null; }

            launchIntent.PutExtra(PushMessage.ExtrasKey, pushMessage);
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
        protected virtual Intent GetBackgroundPushServiceIntent(Context context, PushMessage pushMessage)
        {
            return null;
        }
    }
}