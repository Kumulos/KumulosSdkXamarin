using System;
using Com.Kumulos.Abstractions;
using Android.App;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public void Initialize(IKSConfig config)
        {
            var androidConfig = (KSConfigImplementation)config;

            Android.Kumulos.Initialize((Application)Application.Context.ApplicationContext, androidConfig.GetConfig());

            var httpClient = new HttpClient();

            httpClient.MaxResponseContentBufferSize = 256000;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", config.GetApiKey(), config.GetSecretKey())
            )));


            Build = new Build(GetInstallId(), httpClient, config.GetApiKey());
            PushChannels = new PushChannels(GetInstallId(), httpClient);
        }

        public string GetInstallId()
        {
            return Android.Installation.Id(Application.Context.ApplicationContext);
        }

        public void RegisterForRemoteNotifications()
        {
            throw new NotImplementedException("This method does not need to be called on Android");

        }

        public void RegisterDeviceToken(object NSDataDeviceToken)
        {
            throw new NotImplementedException("This method does not need to be called on Android");
        }

        public void TrackNotificationOpen(object NSDictionaryInfo)
        {
            throw new NotImplementedException();
        }

        public void TrackEvent(string eventType, Dictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, string> properties)
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

        public void AssociateUserWithInstall(string userIdentifier)
        {
            throw new NotImplementedException();
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, string> attributes)
        {
            throw new NotImplementedException();
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, int? distanceMetres)
        {
            throw new NotImplementedException();
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            throw new NotImplementedException("This method should not be called on Android");
        }
    }
}
