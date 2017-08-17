using System;
using Android.Gms.Common;
using Newtonsoft.Json.Linq;

namespace Kumulos.Droid
{
    public static class Push
    {
		public static bool IsPlayServicesAvailable(Android.Content.Context context)
		{
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);
            return resultCode == ConnectionResult.Success;
		}
    }

	internal class RegisterDeviceToken : IRegisterDeviceToken
	{
		string deviceToken;
		
		public RegisterDeviceToken(string deviceToken)
		{
			this.deviceToken = deviceToken;
		}

		public JObject getRequestPayload()
		{
			JObject payload = new JObject();
			payload.Add("token", deviceToken);
			payload.Add("type", 2);

			return payload;
		}
    }
}
