
using Newtonsoft.Json.Linq;

namespace Com.Kumulos.Abstractions
{
    public interface IDeepLinkHandler
    {
        void Handle(JObject data);
    }
}
