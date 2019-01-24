﻿using System;
using Com.Kumulos.Abstractions;
using Android.Content;
using Android.App;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        public void Initialize(IKSConfig config)
        {
            var androidConfig = (KSConfigImplementation)config;

            Android.Kumulos.Initialize((Application)Application.Context.ApplicationContext, androidConfig.GetConfig());
        }

        public string GetInstallId()
        {
            return Android.Installation.Id(Application.Context.ApplicationContext);
        }

        public void TrackEvent(string eventType, object properties)
        {
            throw new NotImplementedException();
        }

        public void TrackEventImmediately(string eventType, object properties)
        {
            throw new NotImplementedException();
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
