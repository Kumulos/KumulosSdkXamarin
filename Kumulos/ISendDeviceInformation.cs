using Newtonsoft.Json.Linq;

namespace Kumulos
{
    public interface ISendDeviceInformation
    {
		JObject getRequestPayload();
	}
}

