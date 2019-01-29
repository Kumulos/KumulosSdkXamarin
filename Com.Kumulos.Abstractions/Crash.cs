using System;
using System.Collections.Generic;

namespace Com.Kumulos.Abstractions
{
    public class Crash
    {
        public static Dictionary<string, object> GetDictionaryForExceptionTracking(Exception e, bool uncaught)
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


    }
}
