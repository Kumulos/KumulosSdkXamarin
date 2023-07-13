using System;
using KumulosSDK.DotNet.Abstractions;
using System.Diagnostics;

namespace KumulosSDK.DotNet
{
    public class Kumulos
    {
        static readonly Lazy<IKSConfig> Config = new Lazy<IKSConfig>(CreateConfig);
        static readonly Lazy<IKumulos> Sdk = new Lazy<IKumulos>(CreateKumulos);

        public static IKSConfig CurrentConfig
        {
            get
            {
                if (Config.Value == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return Config.Value;
            }

        }

        public static IKumulos Current
        {
            get
            {
                if (Sdk.Value == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return Sdk.Value;
            }
        }

        static IKSConfig CreateConfig()
        {
#if IOS
              return new KumulosSDK.DotNet.iOS.KSConfigImplementation();
#elif ANDROID
            return new KumulosSDK.DotNet.Android.KSConfigImplementation();
#else
            Debug.WriteLine("PORTABLE Reached");
            return null;
#endif

        }

        static IKumulos CreateKumulos()
        {
#if IOS
            return new KumulosSDK.DotNet.iOS.KumulosImplementation();
#elif ANDROID
            return new KumulosSDK.DotNet.Android.KumulosImplementation();
#else
            Debug.WriteLine("PORTABLE Reached");
            return null;
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
