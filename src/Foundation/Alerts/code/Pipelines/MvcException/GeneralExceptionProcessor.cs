using System.Web.Mvc;
using Sitecore.Diagnostics;
using Sitecore.Foundation.Alerts.Models;
using Sitecore.Mvc.Pipelines.MvcEvents.Exception;

namespace Sitecore.Foundation.Alerts.Pipelines.MvcException
{
	public class GeneralExceptionProcessor
	{
		public void Process(ExceptionArgs exceptionArgs)
		{
			var exceptionHandled = exceptionArgs?.ExceptionContext?.ExceptionHandled;
			if (exceptionHandled.HasValue && exceptionHandled.Value)
			{
				return;
			}

			if (Context.PageMode.IsNormal && !"true".Equals(exceptionArgs?.ExceptionContext?.HttpContext?.Request?["debug"]))
				return;

			var filterContext = exceptionArgs.ExceptionContext;

			var controllerName = (string)filterContext.RouteData.Values["controller"];
			var actionName = (string)filterContext.RouteData.Values["action"];

			var currentUrl = exceptionArgs?.ExceptionContext?.HttpContext?.Request?.Url?.ToString() ?? "no url retrievable";

			Log.Error(string.Format("Unhandled error in controller: {0}, action: {1}, url: {2}", controllerName, actionName, currentUrl), filterContext.Exception, this);

			var msg = string.Format("Unhandled exception in controller {0} (action: {1})<br /><h4>{2}</h4>----------------------------------------<br /><pre>{3}\r\n{4}</pre>",
				controllerName,
				actionName,
				filterContext.Exception.Message,
				filterContext.HttpContext.IsCustomErrorEnabled ? filterContext.Exception.StackTrace : "",
				filterContext.HttpContext.IsCustomErrorEnabled ? filterContext.Exception.Source : "");
			var model = InfoMessage.Error(msg);


			filterContext.Result = new ViewResult
			{
				ViewName = Constants.InfoMessageView,
				MasterName = null,
				ViewData = new ViewDataDictionary(model),
				TempData = filterContext.Controller.TempData
			};
		    filterContext.ExceptionHandled = true;
		}
	}
}

