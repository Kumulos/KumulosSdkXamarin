using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KumulosSDK.DotNet.Abstractions
{
    abstract public class KumulosBaseImplementation
    {
        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public virtual void Initialize(IKSConfig config)
        {
            // 3MB buffer for build
            Build = new Build(InstallId, configureHttpClient(config, 3145728), config.GetApiKey());

            // 256k for push
            PushChannels = new PushChannels((IKumulos)this, configureHttpClient(config, 256000));

            LogPreviousCrashes();
        }

        private HttpClient configureHttpClient(IKSConfig config, long bufferSize)
        {
            var httpClient = new HttpClient();

            httpClient.MaxResponseContentBufferSize = bufferSize;

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", config.GetApiKey(), config.GetSecretKey())
            )));

            return httpClient;
        }

        public void LogException(Exception e)
        {
            AttemptToLogException(e, false);
        }

        public void LogUncaughtException(Exception e)
        {
            AttemptToLogException(e, true);
        }

        private void AttemptToLogException(Exception e, bool uncaught)
        {
            try
            {
                var newCrash = GetJsonObjectForException(e, uncaught);

                var log = GetCrashLog();

                log.Add(newCrash);

                WriteCrashLogToDisk(log);
            }
            catch (Exception ex)
            {
                //- Don't cause further exceptions trying to log exceptions.
            }
        }

        private JObject GetJsonObjectForException(Exception e, bool uncaught)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("format", Consts.CRASH_REPORT_FORMAT);
            dict.Add("uncaught", uncaught);

            Dictionary<string, object> report = new Dictionary<string, object>();
            report.Add("stackTrace", e.StackTrace);
            report.Add("message", e.Message);
            report.Add("type", e.GetType().ToString());
            report.Add("source", e.Source);
            report.Add("lineNumber", line);

            dict.Add("report", report);


            return JObject.FromObject(dict);
        }

        private JArray GetCrashLog()
        {
            var filename = GetCrashFilePath();

            if (!File.Exists(filename))
            {
                return new JArray();
            }

            var text = File.ReadAllText(filename);

            return (JArray)JsonConvert.DeserializeObject(text);
        }

        private void WriteCrashLogToDisk(JArray log)
        {
            var filename = GetCrashFilePath();

            File.WriteAllText(filename, JsonConvert.SerializeObject(log, Formatting.None));
        }

        public Dictionary<string, object> GetDictionaryForExceptionTracking(Exception e, bool uncaught)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("format", Consts.CRASH_REPORT_FORMAT);
            dict.Add("uncaught", uncaught);

            Dictionary<string, object> report = new Dictionary<string, object>();
            report.Add("stackTrace", e.StackTrace);
            report.Add("message", e.Message);
            report.Add("type", e.GetType().ToString());
            report.Add("source", e.Source);

            dict.Add("report", report);

            return dict;
        }

        protected void LogPreviousCrashes()
        {
            string filename = GetCrashFilePath();

            if (!File.Exists(filename))
            {
                return;
            }

            try
            {
                var log = GetCrashLog();
                if (log.Count == 0)
                {
                    return;
                }

                foreach (JObject crash in log)
                {
                    TrackCrash(crash);
                }
            }
            catch (Exception e)
            {
                // Don't cause more crashes trying to log crashes.
            }
            finally
            {
                // Ensure if the file exists that its cleaned out for next session.
                File.Delete(filename);
            }
        }

        private void TrackCrash(JObject jsonObj)
        {
            TrackCrashEvent(jsonObj);
        }

        private string GetCrashFilePath()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documents, "CrashLog.json");
        }

        public abstract void TrackEvent(string eventType, Dictionary<string, object> properties);

        public abstract void TrackCrashEvent(JObject report);

        public abstract string InstallId { get; }
    }
}
