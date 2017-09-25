using Newtonsoft.Json.Linq;
using Android.Locations;

namespace Kumulos.Droid
{
	public class LocationInfo : ISendLocationInformation
	{
        Location location;

        public LocationInfo(Location location)
		{
			this.location = location;
		}

		public JObject getRequestPayload()
		{
			JObject payload = new JObject();

			payload.Add("lat", location.Latitude);
			payload.Add("lng", location.Longitude);

			return payload;
		}
	}
}
