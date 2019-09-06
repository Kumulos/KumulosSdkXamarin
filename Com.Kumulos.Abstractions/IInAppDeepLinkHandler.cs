using Newtonsoft.Json.Linq;

namespace Com.Kumulos.Abstractions
{
    public interface IInAppDeepLinkHandler
    {
        void Handle(JObject data);
    }
}
