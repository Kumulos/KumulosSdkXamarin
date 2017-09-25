using CoreLocation;
using Newtonsoft.Json.Linq;

namespace Kumulos.iOS
{
    public class LocationInfo : ISendLocationInformation
    {
        CLLocation location;

        public LocationInfo(CLLocation location)
        {
            this.location = location;
        }

        public JObject getRequestPayload()
        {
			JObject payload = new JObject();

			payload.Add("lat", location.Coordinate.Latitude);
			payload.Add("lng", location.Coordinate.Longitude);

            return payload;
		}
    }
}
