using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Com.Kumulos.Abstractions
{
    abstract public class KumulosBaseImplementation
    {
        public Build Build { get; private set; }

        public PushChannels PushChannels { get; private set; }

        public virtual void Initialize(IKSConfig config)
        {
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

            var dict = GetDictionaryForExceptionTracking(e, uncaught);

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

        protected void LogPreviousCrash()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

        public abstract void TrackEvent(string eventType, Dictionary<string, object> properties);

        public abstract string InstallId { get; }
    }
}
