using Newtonsoft.Json.Linq;

namespace Kumulos
{
	public interface ISendLocationInformation
	{
        JObject getRequestPayload();
	}
}