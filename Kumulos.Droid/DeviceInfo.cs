using System;
using Kumulos;
using Newtonsoft.Json.Linq;
using Android.Content.PM;
using Android.OS;
using Java.Util;
using Android.Content;

namespace Kumulos.Droid
{
    public class DeviceInfo : ISendDeviceInformation
    {
        private static int SDK_TYPE = 7;
        private static int RUNTIME_TYPE = 2;
        private static int OS_TYPE = 3;

        private Context context;

        public DeviceInfo(Context context)
        {
            this.context = context;
        }

        public JObject getRequestPayload()
        {
            JObject payload = new JObject();

            JObject app = new JObject();

            var appContext = Android.App.Application.Context;
            var packageInfo = appContext.PackageManager.GetPackageInfo(appContext.PackageName, 0);

            app.Add("bundle", packageInfo.PackageName);
            app.Add("version", packageInfo.VersionName);

            int target = 2;
#if DEBUG
			target = 1;
#endif

            app.Add("target", target);

            JObject sdk = new JObject();
            sdk.Add("id", SDK_TYPE);
            sdk.Add("version", "2.0.4");

            JObject runtime = new JObject();
            runtime.Add("id", RUNTIME_TYPE);
            runtime.Add("version", Build.VERSION.Release);

            JObject os = new JObject();

            os.Add("id", OS_TYPE);
            os.Add("version", Build.VERSION.Release);

            JObject device = new JObject();

            device.Add("name", Build.Model);
            device.Add("tz", Java.Util.TimeZone.Default.ID);
            device.Add("isSimulator", isSimulator());
            device.Add("locale", Locale.Default.ToLanguageTag());

            payload.Add("app", app);
            payload.Add("sdk", sdk);
            payload.Add("runtime", runtime);
            payload.Add("os", os);
            payload.Add("device", device);

            return payload;
        }

        private bool isSimulator()
        {
            return Build.Fingerprint.StartsWith("generic")
                || Build.Fingerprint.StartsWith("unknown")
                || Build.Model.Contains("google_sdk")
                || Build.Model.Contains("Emulator")
                || Build.Model.Contains("Android SDK built for x86")
                || Build.Manufacturer.Contains("Genymotion")
                || (Build.Brand.StartsWith("generic") && Build.Device.StartsWith("generic"))
                || "google_sdk".Equals(Build.Product)
                || Build.Product.Contains("vbox86p")
                || Build.Device.Contains("Droid4X");
        }
    }
}
