using Newtonsoft.Json.Linq;

namespace Com.Kumulos.Abstractions
{
    public interface IINAppDeepLinkHandler
    {
        void Handle(JObject data);
    }
}
