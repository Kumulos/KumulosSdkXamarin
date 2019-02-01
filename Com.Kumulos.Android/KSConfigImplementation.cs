using System;
using Android.OS;
using Org.Json;

namespace Com.Kumulos
{
    public class KSConfigImplementation : Abstractions.IKSConfig
    {
        private string apiKey, secretKey;
        private int timeoutSeconds;

        public Abstractions.IKSConfig AddKeys(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;

            return this;
        }

        public Abstractions.IKSConfig EnableCrashReporting()
        {
            throw new NotImplementedException("Native crash reporting is not available on Android - please refer to the integration guide.");
        }

        public Abstractions.IKSConfig SetSessionIdleTimeout(int timeoutSeconds)
        {
            this.timeoutSeconds = timeoutSeconds;
            return this;
        }

        public Android.KumulosConfig GetConfig()
        {
            var specificConfig = new Android.KumulosConfig.Builder(apiKey, secretKey);

            JSONObject sdkInfo = new JSONObject();
            sdkInfo.Put("id", Abstractions.Consts.SDK_TYPE);
            sdkInfo.Put("version", Abstractions.Consts.SDK_VERSION);

            specificConfig.SetSdkInfo(sdkInfo);

            JSONObject runtimeInfo = new JSONObject();
            runtimeInfo.Put("id", Abstractions.Consts.RUNTIME_TYPE);
            runtimeInfo.Put("version", Build.VERSION.Release);

            specificConfig.SetRuntimeInfo(runtimeInfo);

            return specificConfig.Build();
        }

        public string GetApiKey()
        {
            return apiKey;
        }

        public string GetSecretKey()
        {
            return secretKey;
        }
    }
}
