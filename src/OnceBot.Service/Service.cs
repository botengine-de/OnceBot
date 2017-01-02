using BotEngine;
using System.Linq;

namespace OnceBot.Service
{
	public class Service
	{
		readonly Config Config = Config.ConfigDefault;

		public string ClientRequest(string request) =>
			ClientRequest(request?.DeserializeFromString<Request>())?.SerializeToString();

		public Response ClientRequest(Request request)
		{
			if (null == request)
				return null;

			ImageProcessing.ImagePatternMatchSearchResponse ImagePatternMatchSearchResponse = null;

			var ImagePatternMatchSearch = request?.ImagePatternMatchSearch;

			if (null != ImagePatternMatchSearch)
				ImagePatternMatchSearchResponse = new ImageProcessing.ImagePatternMatchSearchResponse
				{
					SetMatch = Config.ImageProcessing.SetMatchInImage(ImagePatternMatchSearch.Raster)?.MapIdFromKey()?.ToArray(),
				};

			return new Response
			{
				ImagePatternMatchSearch = ImagePatternMatchSearchResponse,
			};
		}
	}
}
