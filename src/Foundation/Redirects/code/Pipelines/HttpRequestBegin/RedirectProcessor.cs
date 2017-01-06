using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoC.InversionOfControl;
using Sitecore.Diagnostics;
using Sitecore.Foundation.Redirects.Models;
using Sitecore.Foundation.Redirects.Services;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Foundation.Redirects.Pipelines.HttpRequestBegin
{
	public class RedirectProcessor : HttpRequestProcessor
	{
	    private IHttpContextBaseProvider _httpContextBaseProvider;
	    private IRedirectService _redirectService;
	    private ISitecoreContextProvider _sitecoreContextProvider;

	    public RedirectProcessor()
	    {
			_httpContextBaseProvider = IoC.Resolver.Resolve<IHttpContextBaseProvider>();
			_redirectService = IoC.Resolver.Resolve<IRedirectService>();
			_sitecoreContextProvider = IoC.Resolver.Resolve<ISitecoreContextProvider>();
		}

		public override void Process(HttpRequestArgs args)
		{
			Assert.ArgumentNotNull(args, "args");
			if (args.LocalPath == "/layouts/system/visitoridentification") return;

			var originalUrl = new Uri(_httpContextBaseProvider.GetCurrentHttpContext().Request.Url, _httpContextBaseProvider.GetCurrentHttpContext().Request.RawUrl);
			if (originalUrl == null || originalUrl.AbsolutePath.Contains("/sitecore")) return;

			if (_sitecoreContextProvider.GetCurrentContentDatabase() == null) return;

			var currentItem = _sitecoreContextProvider.GetCurrentContextItem();
			BaseRedirect foundRedirect = null;

			foundRedirect = currentItem != null ? _redirectService.GetFor(currentItem.ID.Guid) : _redirectService.GetFor(originalUrl);
		    if (foundRedirect == null && currentItem != null)
		        foundRedirect = this._redirectService.GetFor(originalUrl);

			if (foundRedirect != null && foundRedirect.TargetLink != null && foundRedirect.TargetLink.Url != null)
				_redirectService.ProcessRedirect(foundRedirect, originalUrl);
		}
	}
}