using System;
namespace Com.Kumulos.Abstractions
{
    public interface IKSConfig
    {
        IKSConfig AddKeys(string apiKey, string secretKey);
        IKSConfig EnableCrashReporting();
        IKSConfig SetSessionIdleTimeout(int timeoutSeconds);

  /*IKSConfig SetRuntimeInfo(NSDictionary info);
        IKSConfig SetSdkInfo(NSDictionary info);
        IKSConfig SetTargetType(KSTargetType type);*/
    }
}
