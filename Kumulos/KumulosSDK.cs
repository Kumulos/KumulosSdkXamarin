using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Plugin.Settings.Abstractions;

namespace Kumulos
{
    public class KumulosSDK
    {
        private static KumulosSDK instance;

        HttpClient client;
        string apiKey;
        string secretKey;

        KumulosPush pushService;
        KumulosBuild buildService;
        KumulosStats statsService;

        ISettings appSettings;

        public static void Initialize(ISendDeviceInformation deviceInfo, ISettings appSettings, Dictionary<string, string> config)
        {
            instance = new KumulosSDK(config);
            instance.appSettings = appSettings;

            instance.statsService.SendDeviceInformation(deviceInfo);
        }

        public static KumulosPush Push
        {
            get
            {
                return Instance.pushService;
            }
        }

        public static KumulosBuild Build
        {
            get
            {
                return Instance.buildService;
            }
        }

        internal static KumulosSDK Instance
        {
            get
            {
                if (instance == null) { throw new Exception("KumulosSDK has not been initialized"); }
                return instance;
            }
        }

        public static string InstallId
        {
            get
            {
                var appSettings = Instance.appSettings;
                string storageInstallId = appSettings.GetValueOrDefault("installId", string.Empty);
                if (storageInstallId == string.Empty)
                {
                    Guid installId = Guid.NewGuid();
                    appSettings.AddOrUpdateValue("installId", installId);

                    return installId.ToString();
                }

                return Guid.Parse(storageInstallId).ToString();
            }
        }

        private KumulosSDK(Dictionary<string, string> config)
        {
            if (!config.ContainsKey("apiKey") && !config.ContainsKey("secretKey"))
            {
                throw new Exception("apiKey and secretKey must be set to complete initialization");
            }

            apiKey = config["apiKey"];
            secretKey = config["secretKey"];

            pushService = new KumulosPush();
            buildService = new KumulosBuild(apiKey);
            statsService = new KumulosStats();

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", apiKey, secretKey)
            )));
        }

        internal static HttpClient GetHttpClient()
        {
            return Instance.client;
        }
    }
}
