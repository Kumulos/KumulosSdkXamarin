using System;

namespace Com.Kumulos
{
    public class KumulosImplementation
    {
        public static void Init(string apiKey, string secretKey)
        {
            var config = iOS.KSConfig.ConfigWithAPIKey(apiKey, secretKey);
            iOS.Kumulos.InitializeWithConfig(config);
        }
    }
}
