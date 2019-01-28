using System;
using Com.Kumulos.Abstractions;
using Foundation;
using UIKit;
using UserNotifications;
using System.Net.Http;
using System.Net.Http.Headers;
using CoreLocation;
using System.Collections.Generic;
using System.Linq;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        private iOS.Kumulos thisRef;

        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public void Initialize(IKSConfig config)
        {
            var iosKSConfig = (KSConfigImplementation)config;

            thisRef = iOS.Kumulos.InitializeWithConfig(iosKSConfig.Build());

            var httpClient = new HttpClient();

            httpClient.MaxResponseContentBufferSize = 256000;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", config.GetApiKey(), config.GetSecretKey())
            )));


            Build = new Build(InstallId, httpClient, config.GetApiKey());
            PushChannels = new PushChannels(InstallId, httpClient);
        }

        public string InstallId
        {
            get
            {
                return iOS.Kumulos.InstallId;
            }
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

        public void TrackNotificationOpen(object NSDictionaryUserInfo)
        {
            iOS.Kumulos_Push.PushTrackOpenFromNotification(thisRef, (NSDictionary)NSDictionaryUserInfo);
        }

        public void TrackEvent(string eventType, Dictionary<string, string> properties)
        {
            var nsDict = NSDictionary.FromObjectsAndKeys(properties.Values.ToArray(), properties.Keys.ToArray());

            iOS.Kumulos_Analytics.TrackEvent(thisRef, eventType, nsDict);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, string> properties)
        {
            var nsDict = NSDictionary.FromObjectsAndKeys(properties.Values.ToArray(), properties.Keys.ToArray());

            iOS.Kumulos_Analytics.TrackEventImmediately(thisRef, eventType, nsDict);
        }

        public void LogException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void LogUncaughtException(Exception e)
        {
            throw new NotImplementedException();
        }

        public void SendLocationUpdate(double lat, double lng)
        {
            CLLocation cl = new CLLocation(lat, lng);

            iOS.Kumulos_Location.SendLocationUpdate(thisRef, cl);
        }

        public void AssociateUserWithInstall(string userIdentifier)
        {
            iOS.Kumulos_Analytics.AssociateUserWithInstall(thisRef, userIdentifier);
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, string> attributes)
        {
            var nsDict = NSDictionary.FromObjectsAndKeys(attributes.Values.ToArray(), attributes.Keys.ToArray());

            iOS.Kumulos_Analytics.AssociateUserWithInstall(thisRef, userIdentifier, nsDict);
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, int? distanceMetres)
        {
            throw new NotImplementedException("This method should not be called on iOS");
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            iOS.Kumulos_Location.SendiBeaconProximity(thisRef, (CLBeacon)CLBeaconObject);
        }
    }
}
