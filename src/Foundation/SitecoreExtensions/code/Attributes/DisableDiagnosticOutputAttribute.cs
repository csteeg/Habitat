using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sitecore.Foundation.SitecoreExtensions.Attributes
{
	public class DisableDiagnosticOutputAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			if (Context.Diagnostics.Profiling)
				Context.Diagnostics.ProfileWorker.RenderOnEndSession = false;
			if (Context.Diagnostics.Tracing)
				Context.Diagnostics.TraceWorker.RenderOnEndSession = false;
			Context.Diagnostics.ShowRenderingInfo = false;
		}
	}
}