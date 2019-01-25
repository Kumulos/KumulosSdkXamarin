﻿using System;
using Com.Kumulos.Abstractions;
using Foundation;
using UIKit;
using UserNotifications;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        private iOS.Kumulos thisRef;
        private Build buildRef;

        public Build Build
        {
            get
            {
                return buildRef;
            }
        }

        public void Initialize(IKSConfig config)
        {
            var iosKSConfig = (KSConfigImplementation)config;

            thisRef = iOS.Kumulos.InitializeWithConfig(iosKSConfig.Build());
            buildRef = new Build(GetInstallId(), config.GetApiKey(), config.GetSecretKey());
        }

        public string GetInstallId()
        {
            return iOS.Kumulos.InstallId;
        }

        public void RegisterForRemoteNotifications()
        { 
            var center = UNUserNotificationCenter.Current;
            center.RequestAuthorization(
                UNAuthorizationOptions.Badge |
                UNAuthorizationOptions.Alert |
                UNAuthorizationOptions.Sound,
                (bool arg1, Foundation.NSError arg2) =>
                {

                });

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        public void RegisterDeviceToken(object NSDataDeviceToken)
        {
            iOS.Kumulos_Push.PushRegisterWithDeviceToken(thisRef, (NSData)NSDataDeviceToken);
        }

        public void TrackNotificationOpen(object NSDictionaryInfo)
        {
            iOS.Kumulos_Push.PushTrackOpenFromNotification(thisRef, (NSDictionary)NSDictionaryInfo);
        }

        public void TrackEvent(string eventType, object properties)
        {
            throw new NotImplementedException();
            //iOS.Kumulos_Analytics.TrackEvent(iOS.Kumulos, eventType, properties);
        }

        public void TrackEventImmediately(string eventType, object properties)
        {
            iOS.Kumulos_Analytics.TrackEventImmediately(thisRef, eventType, (NSDictionary)properties);
}

        public void LogException(Exception e)
        {

            throw new NotImplementedException();
        }

        public void LogUncaughtException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void SendLocationUpdate(decimal lat, decimal lng)
        {
            throw new NotImplementedException();
        }

        public void AssociateUserWithInstall(string userIdentifier, object attributes)
        {
            throw new NotImplementedException();
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, int? distanceMetres)
        {
            throw new NotImplementedException();
        }

        public void TrackiBeaconProximity(string uuid, int major, int minor, int proximity)
        {
            throw new NotImplementedException();
        }


    }
}
