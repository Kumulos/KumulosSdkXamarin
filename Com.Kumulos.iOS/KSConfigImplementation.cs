using Com.Kumulos.Abstractions;
using Foundation;

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

        public iOS.KSConfig Build()
        {
            var specificConfig = iOS.KSConfig.ConfigWithAPIKey(apiKey, secretKey);

            if (enableCrashReporting)
            {
                specificConfig.EnableCrashReporting();
            }

            var sdkKeys = new object[] { "id", "version" };
            var sdkValues = new object[] { Consts.SDK_TYPE, "2.0" };

            var sdkInfo = NSDictionary.FromObjectsAndKeys(sdkValues, sdkKeys);

            specificConfig.SetSdkInfo(sdkInfo);

            var runtimeKeys = new object[] { "id", "version" };
            var runtimeValues = new object[] { Consts.RUNTIME_TYPE, ObjCRuntime.Constants.Version };
        
            var runtimeInfo = NSDictionary.FromObjectsAndKeys(runtimeValues, runtimeKeys);

            specificConfig.SetRuntimeInfo(runtimeInfo);

            return specificConfig;
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
