using System.Collections.Specialized;
using System.Web;
using Sitecore.Data;
using Sitecore.Foundation.Alerts.Repositories;
using Sitecore.IO;
using Sitecore.Links;

namespace Sitecore.Foundation.Alerts.Pipelines.MvcException
{
	using System.Web.Mvc;
	using Sitecore.Diagnostics;
	using Sitecore.Foundation.Alerts.Exceptions;
	using Sitecore.Foundation.Alerts.Models;
	using Sitecore.Mvc.Pipelines.MvcEvents.Exception;

	public class HttpExceptionProcessor
	{
		public void Process(ExceptionArgs exceptionArgs)
		{
			if (exceptionArgs.ExceptionContext.ExceptionHandled)
			{
				return;
			}

			var httpException = exceptionArgs.ExceptionContext.Exception as HttpException;
			if (httpException == null)
				return;
			var siteContext = Context.Site;
			if (siteContext == null)
				return;
			var httpContext = exceptionArgs.PageContext?.RequestContext?.HttpContext;
			if (httpContext == null)
				return;

			var errorItem = ErrorRepository.GetErrorItem(siteContext, httpException.GetHttpCode(), exceptionArgs.PageContext.Database);
			if (errorItem == null)
				return;
			Log.Error(httpException.Message, httpException, this);

			var urlOptions = LinkManager.GetDefaultUrlOptions();
			urlOptions.AlwaysIncludeServerUrl = false;
			var itemUrl = LinkManager.GetItemUrl(errorItem, urlOptions);

			httpContext.Response.Clear();
			httpContext.Response.StatusCode = httpException.GetHttpCode();
			httpContext.Response.TrySkipIisCustomErrors = true;
			httpContext.Items[ErrorRepository.HttpContextStatusKey] = httpException.GetHttpCode();

			if (httpContext.Request.Url != null && !httpContext.Request.Url.PathAndQuery.StartsWith(itemUrl))
			{
				httpContext.Server.TransferRequest(itemUrl, false, "GET", new NameValueCollection() { {ErrorRepository.HeaderStatusKey, httpException.GetHttpCode().ToString()} });
				exceptionArgs.ExceptionContext.Result = new EmptyResult();
				exceptionArgs.ExceptionContext.ExceptionHandled = true;
				httpContext.Response.End();
			}
		}
	}
}