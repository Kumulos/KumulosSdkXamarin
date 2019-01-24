using System;
using Android.OS;
using Com.Kumulos;
using Com.Kumulos.Abstractions;
using Org.Json;

namespace Com.Kumulos
{
    public class KSConfigImplementation : Abstractions.IKSConfig
    {
        private string apiKey, secretKey;
        private bool enableCrashReporting;
        private int timeoutSeconds;

        public IKSConfig AddKeys(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;

            return this;
        }

        public IKSConfig EnableCrashReporting()
        {
            enableCrashReporting = true;
            return this;
        }

        public IKSConfig SetSessionIdleTimeout(int timeoutSeconds)
        {
            this.timeoutSeconds = timeoutSeconds;
            return this;
        }

        public Android.KumulosConfig GetConfig()
        {
            var specificConfig = new Android.KumulosConfig.Builder(apiKey, secretKey);


            if (enableCrashReporting)
            {
                specificConfig.EnableCrashReporting();
            }

            JSONObject sdkInfo = new JSONObject();
            sdkInfo.Put("id", Consts.SDK_TYPE);
            sdkInfo.Put("version", "2.0");

            specificConfig.SetSdkInfo(sdkInfo);

            JSONObject runtimeInfo = new JSONObject();
            runtimeInfo.Put("id", Consts.RUNTIME_TYPE);
            runtimeInfo.Put("version", Build.VERSION.Release);

            specificConfig.SetRuntimeInfo(runtimeInfo);

            return specificConfig.Build();
        }
    }
}
