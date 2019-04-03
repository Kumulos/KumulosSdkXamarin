﻿using System;
using Com.Kumulos.Abstractions;
using Android.App;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;
using Org.Json;
using Android.Locations;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Android.Gms.Common;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public Crash Crash => throw new NotImplementedException();

        public void Initialize(IKSConfig config)
        {
            var androidConfig = (KSConfigImplementation)config;

            Android.Kumulos.Initialize((Application)Application.Context.ApplicationContext, androidConfig.GetConfig());

            var httpClient = new HttpClient();

            httpClient.MaxResponseContentBufferSize = 256000;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", config.GetApiKey(), config.GetSecretKey())
            )));


            Build = new Build(InstallId, httpClient, config.GetApiKey());
            PushChannels = new PushChannels(InstallId, httpClient);

            try
            {
                LogPreviousCrash();
            }
            catch (Exception e)
            {
                //- Don't cause further exceptions trying to log exceptions.
            }
        }

        public string InstallId
        {
            get
            {
                return Android.Installation.Id(Application.Context.ApplicationContext);
            }
        }

        public string UserIdentifier
        {
            get
            {
                return Android.Kumulos.GetCurrentUserIdentifier(Application.Context.ApplicationContext);
            }
        }

        public void RegisterForRemoteNotifications()
        {
            Android.Kumulos.PushRegister(Application.Context.ApplicationContext);
        }

        public void UnregisterDeviceToken()
        {
            throw new NotImplementedException();
        }

        public void TrackEvent(string eventType, Dictionary<string, object> properties)
        {
            JSONObject props = new JSONObject(properties);
            Android.Kumulos.TrackEvent(Application.Context.ApplicationContext, eventType, props);
        }

        public void TrackEventImmediately(string eventType, Dictionary<string, object> properties)
        {
            JSONObject props = new JSONObject(properties);
            Android.Kumulos.TrackEventImmediately(Application.Context.ApplicationContext, eventType, props);
        }

        public void AssociateUserWithInstall(string userIdentifier)
        {
            Android.Kumulos.AssociateUserWithInstall(Application.Context.ApplicationContext, userIdentifier);
        }

        public void AssociateUserWithInstall(string userIdentifier, Dictionary<string, object> attributes)
        {
            JSONObject attr = new JSONObject(attributes);

            Android.Kumulos.AssociateUserWithInstall(Application.Context.ApplicationContext, userIdentifier, attr);
        }

        public void ClearUserAssociation()
        {
            Android.Kumulos.ClearUserAssociation(Application.Context.ApplicationContext);
        }

        public void LogException(Exception e)
        {
            AttemptToLogException(e, false);
        }

        public void LogUncaughtException(Exception e)
        {
            AttemptToLogException(e, true);
        }

        public void SendLocationUpdate(double lat, double lng)
        {
            Location location = new Location("provider");
            location.Latitude = lat;
            location.Longitude = lng;

            Android.Kumulos.SendLocationUpdate(Application.Context.ApplicationContext, location);
        }

        public void TrackEddystoneBeaconProximity(string namespaceHex, string instanceHex, double distanceMetres)
        {
            Java.Lang.Double dblDistance = new Java.Lang.Double(distanceMetres);
            Android.Kumulos.TrackEddystoneBeaconProximity(Application.Context.ApplicationContext, namespaceHex, instanceHex, dblDistance);
        }

        private void AttemptToLogException(Exception e, bool uncaught)
        {
            try
            {
                var dict = GetDictionaryForException(e, uncaught);
                WriteCrashToDisk(dict);
            }
            catch (Exception ex)
            {
                //- Don't cause further exceptions trying to log exceptions.
            }
        }

        private Dictionary<string, object> GetDictionaryForException(Exception e, bool uncaught)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            var dict = Crash.GetDictionaryForExceptionTracking(e, uncaught);

            var report = (Dictionary<string, object>)dict["report"];
            report.Add("lineNumber", line);

            return dict;
        }

        private void WriteCrashToDisk(Dictionary<string, object> crash)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filename = Path.Combine(documents, "CrashLog.json");
            File.WriteAllText(filename, JsonConvert.SerializeObject(crash, Formatting.None));
        }

        private void LogPreviousCrash()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filename = Path.Combine(documents, "CrashLog.json");

            if (!File.Exists(filename))
            {
                return;
            }

            var text = File.ReadAllText(filename);
            var jsonObj = (JContainer)JsonConvert.DeserializeObject(text);

            var dict = new Dictionary<string, object>
                {
                    { "format", (string)jsonObj["format"] },
                    { "uncaught", (bool)jsonObj["uncaught"] }
                };

            var reportObj = (JContainer)jsonObj["report"];

            var report = new Dictionary<string, object>
            {
                { "stackTrace", (string)reportObj["stackTrace"] },
                { "message", (string)reportObj["message"] },
                { "type", (string)reportObj["type"] },
                { "source", (string)reportObj["source"] },
                { "lineNumber", (int)reportObj["lineNumber"] }
            };

            dict.Add("report", report);

            TrackEvent(Consts.CRASH_REPORT_EVENT_TYPE, dict);

            File.Delete(filename);
        }

        public bool IsGooglePlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context.ApplicationContext);
            return resultCode == ConnectionResult.Success;
        }

        public void TrackNotificationOpen(string notificationId)
        {
            Android.Kumulos.PushTrackOpen(Application.Context.ApplicationContext, notificationId);
        }

        public void RegisterDeviceToken(object NSDataDeviceToken)
        {
            throw new NotImplementedException();
        }

        public void TrackNotificationOpen(object NSDictionaryInfo)
        {
            throw new NotImplementedException();
        }

        public void TrackiBeaconProximity(object CLBeaconObject)
        {
            throw new NotImplementedException("This method should not be called on Android");
        }
    }
}
