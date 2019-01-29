﻿using System;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Iid;
using Android.Content.PM;
using Android.OS;

namespace Com.Kumulos.Android
{
    [Service(Exported = false)]
    public class GcmRegistrationIntentService : IntentService
    {
        static object locker = new object();

        public GcmRegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent(Intent intent)
        {
            lock (locker)
            {

                var instanceID = InstanceID.GetInstance(this);
                var token = instanceID.GetToken(getDefaultSenderId(), GoogleCloudMessaging.InstanceIdScope);

                KumulosSDK.Push.RegisterDeviceToken(new RegisterDeviceToken(token));
            }
        }

        private String getDefaultSenderId()
        {
            ApplicationInfo info = null;
            try
            {
                info = PackageManager.GetApplicationInfo(PackageName, PackageInfoFlags.MetaData);
            }
            catch (PackageManager.NameNotFoundException)
            {
                throw new Exception("Unable to read Google project number for GCM registration, aborting!");
            }

            Bundle meta = info.MetaData;
            String senderId = meta.GetString("kumulos_gcm_sender_id");
            if (senderId == string.Empty || senderId == null)
            {
                throw new Exception("Unable to read Google project number for GCM registration, aborting!");
            }
            return senderId;
        }
    }
}