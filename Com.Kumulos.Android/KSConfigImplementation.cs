﻿using System;
using Com.Kumulos;
using Com.Kumulos.Abstractions;

namespace Com.Kumulos
{
    public class KSConfigImplementation : Abstractions.IKSConfig
    {
        private string apiKey, secretKey;
        private bool enableCrashReporting;
        private int timeoutSeconds;

        public KSConfigImplementation()
        {

        }



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

            return specificConfig.Build();
        }

        private KSConfigImplementation(string apiKey, string secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;
        }
    }
}
