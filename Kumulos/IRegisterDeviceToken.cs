using Newtonsoft.Json.Linq;

namespace Kumulos
{
public interface IRegisterDeviceToken
    {
        JObject getRequestPayload();
}
}
