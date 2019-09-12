using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Com.Kumulos.Abstractions
{
    abstract public class KumulosBaseImplementation
    {
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
    }
}
