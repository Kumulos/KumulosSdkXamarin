using System;
using Com.Kumulos.Abstractions;

namespace Com.Kumulos
{
    public class KumulosImplementation : IKumulos
    {
        public void Initialize(IKSConfig config)
        {
            var iosKSConfig = (KSConfigImplementation)config;

            iOS.Kumulos.InitializeWithConfig(iosKSConfig.Build());
        }

        public string GetInstallId()
        {
            return iOS.Kumulos.InstallId;
        }
    }
}
