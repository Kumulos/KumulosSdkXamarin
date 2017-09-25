using System.Net.Http;
using System.Text;

namespace Kumulos
{
	public class KumulosStats
	{
        public void SendLocationUpdate(ISendLocationInformation request)
        {
			var uri = string.Format("https://stats.kumulos.com/v1/app-installs/{0}/location", KumulosSDK.InstallId);

            var content = new StringContent(request.getRequestPayload().ToString(), Encoding.UTF8, "application/json");

			var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Content = content;

			KumulosSDK.GetHttpClient().SendAsync(requestMessage);
        }

		public void SendDeviceInformation(ISendDeviceInformation request)
		{
			var uri = string.Format("https://stats.kumulos.com/v1/app-installs/{0}", KumulosSDK.InstallId);

			var content = new StringContent(request.getRequestPayload().ToString(), Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);
			requestMessage.Headers.Add("Accept", "application/json");
			requestMessage.Content = content;

			KumulosSDK.GetHttpClient().SendAsync(requestMessage);
		}
	}
}
