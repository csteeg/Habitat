using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Foundation.Alerts.Repositories;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Foundation.Alerts.Pipelines.RequestBegin
{
	public class ItemErrorResolver: HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			var siteContext = Context.Site;
			if (siteContext == null)
				return;
			var database = Context.Database;
			if (database == null)
				return;
			var item = Context.Item;
			if (item != null)
				return;
			var pageContext = Context.Page;
			if (pageContext != null && !string.IsNullOrEmpty(pageContext.FilePath))
				return;


			var statusCode = args.PermissionDenied ? 403 : 404;
			var errorItem = ErrorRepository.GetErrorItem(siteContext, statusCode, database);
			if (errorItem == null)
				return;

			Context.Item = errorItem;
			args.Context.Items[ErrorRepository.HttpContextStatusKey] = statusCode;
		}
	}
}