using Sitecore.Foundation.Alerts.Repositories;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Foundation.Alerts.Pipelines.RequestProcessed
{
	public class SetStatusCode: HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			var statusCode = args.Context.Items[ErrorRepository.HttpContextStatusKey] as string ?? args.Context.Request.Headers.Get(ErrorRepository.HeaderStatusKey);
			var httpCode = 0;
			if (!string.IsNullOrEmpty(statusCode) && int.TryParse(statusCode, out httpCode))
			{
				args.Context.Response.StatusCode = httpCode;
				args.Context.Response.TrySkipIisCustomErrors = true;
			}
		}
	}
}