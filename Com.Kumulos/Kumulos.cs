using System;
using Com.Kumulos.Abstractions;
using System.Diagnostics;

namespace Com.Kumulos
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
#if PORTABLE
            Debug.WriteLine("PORTABLE Reached");
            return null;
#else
         Debug.WriteLine("Other reached");
         return new KSConfigImplementation();
#endif
}

        static IKumulos CreateKumulos()
        {
#if PORTABLE
            Debug.WriteLine("PORTABLE Reached");
            return null;
#else
         Debug.WriteLine("Other reached");
         return new KumulosImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
        {
            return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}
