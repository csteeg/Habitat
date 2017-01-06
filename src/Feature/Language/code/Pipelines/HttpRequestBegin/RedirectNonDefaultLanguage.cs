using System;
using System.IO;
using System.Web;
using Sitecore.Links;
using Sitecore.Pipelines.HttpRequest;

namespace Sitecore.Feature.Language.Pipelines.HttpRequestBegin
{
	public class RedirectNonDefaultLanguage : HttpRequestProcessor
	{
	    public override void Process(HttpRequestArgs args)
	    {
			var site = Context.Site;
			if (site == null)
			{
				return;
			}
	        if (string.IsNullOrEmpty(args?.Context?.Request?.ApplicationPath))
	            return;

	        var requestPath = args.Context.Request.RawUrl;
	        if (requestPath.StartsWith(args.Context.Request.ApplicationPath, StringComparison.InvariantCultureIgnoreCase))
	            requestPath = requestPath.Substring(args.Context.Request.ApplicationPath.Length);
	        if (!requestPath.StartsWith("/"))
	            requestPath = "/" + requestPath;

			if (args.LocalPath != "/" || requestPath != args.Url.FilePath)
	            return;
			
	        if (Context.Language.Name == site.Language)
	            return;

	        var item = (Context.Database ?? Context.ContentDatabase).GetItem(site.ContentStartPath, Context.Language);
	        if (item == null)
	            return;
	        var url = LinkManager.GetItemUrl(item);
			args.Context.Response.Redirect(url, true);
			args.AbortPipeline();

		}

	}
}