using System;
using Newtonsoft.Json.Linq;
using UIKit;
using Foundation;
using System.Runtime.InteropServices;

namespace Kumulos.iOS
{
    public class DeviceInfo : ISendDeviceInformation
    {
        private static int SDK_TYPE = 7;
        private static int RUNTIME_TYPE = 2;
        private static int OS_TYPE = 1;

        public DeviceInfo()
        {
        }

        public JObject getRequestPayload()
        {
            JObject payload = new JObject();

            JObject app = new JObject();
            var infoDict = NSBundle.MainBundle.InfoDictionary;
            var sharedApp = UIApplication.SharedApplication;
            var currentDevice = UIDevice.CurrentDevice;

            app.Add("bundle", infoDict["CFBundleIdentifier"].ToString());
            app.Add("version", infoDict["CFBundleShortVersionString"].ToString());

            int target = 2;
#if DEBUG
            target = 1;
#endif

            app.Add("target", target);

            JObject sdk = new JObject();
            sdk.Add("id", SDK_TYPE);
            sdk.Add("version", "1.1.3");

            JObject runtime = new JObject();
            runtime.Add("id", RUNTIME_TYPE);
            runtime.Add("version", ObjCRuntime.Constants.Version);

            JObject os = new JObject();

            os.Add("id", OS_TYPE);
            os.Add("version", UIDevice.CurrentDevice.SystemVersion);

            JObject device = new JObject();

            device.Add("name", GetSystemProperty("hw.machine"));
            device.Add("tz", NSTimeZone.SystemTimeZone.Name);
            device.Add("isSimulator", ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR);

            if (NSLocale.PreferredLanguages.Length >= 1)
            {
                device.Add("locale", NSLocale.PreferredLanguages[0]);
            }

            payload.Add("app", app);
            payload.Add("sdk", sdk);
            payload.Add("runtime", runtime);
            payload.Add("os", os);
            payload.Add("device", device);

            return payload;
        }

        [DllImport(ObjCRuntime.Constants.SystemLibrary)]
		static internal extern int sysctlbyname([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

		public static string GetSystemProperty(string property)
		{
			var pLen = Marshal.AllocHGlobal(sizeof(int));
			sysctlbyname(property, IntPtr.Zero, pLen, IntPtr.Zero, 0);
			var length = Marshal.ReadInt32(pLen);
			var pStr = Marshal.AllocHGlobal(length);
			sysctlbyname(property, pStr, pLen, IntPtr.Zero, 0);
			return Marshal.PtrToStringAnsi(pStr);
		}
    }
}
