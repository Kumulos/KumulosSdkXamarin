using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kumulos
{
	public class KumulosStats
	{
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
