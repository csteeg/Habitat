using System;
using BoC.InversionOfControl;
using Sitecore.Data.Items;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Sitecore.Foundation.SitecoreExtensions.Pipelines.Response
{
    public class GenerateCacheKey : global::Sitecore.Mvc.Pipelines.Response.RenderRendering.GenerateCacheKey
    {
        public override void Process(RenderRenderingArgs args)
        {
            //Prevent Verbs other then HTTP-GET to be cached
            var currentHttpContextProvider = IoC.Resolver.Resolve<IHttpContextBaseProvider>();
            if (currentHttpContextProvider != null)
            {
                var currentHttpContext = currentHttpContextProvider.GetCurrentHttpContext();
                if (currentHttpContext != null &&
                    ! String.Equals(currentHttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
            // Process normal 
            base.Process(args);
        }
        private Item GetRenderingItem(global::Sitecore.Mvc.Presentation.Rendering rendering)
        {
            return rendering.RenderingItem.Database.SelectSingleItem(rendering.RenderingItem.ID.ToString());
        }

        public bool GetCustomVaryOption(Item rendering, string fieldName)
        {
            return rendering[fieldName].ToBool();
        }

        protected override string GenerateKey(global::Sitecore.Mvc.Presentation.Rendering rendering, global::Sitecore.Mvc.Pipelines.Response.RenderRendering.RenderRenderingArgs args)
        {
            var key = base.GenerateKey(rendering, args);
            var renderingItem = this.GetRenderingItem(rendering);

            if (this.GetCustomVaryOption(renderingItem, "VaryByRenderingId"))
                key = key + this.GetRenderingIdPart(renderingItem);

            if (this.GetCustomVaryOption(renderingItem, "VaryByUrl"))
                key = key + this.GetUrlPart(rendering);

            return key;
        }

        protected virtual string GetRenderingIdPart(Item rendering)
        {
            return string.Format("_#renderingid:{0}", rendering.ID.Guid);
        }

        protected virtual string GetUrlPart(global::Sitecore.Mvc.Presentation.Rendering rendering)
        {
            var httpContextBaseProvider = IoC.Resolver.Resolve<IHttpContextBaseProvider>();
            var httpContext = httpContextBaseProvider.GetCurrentHttpContext();

            return httpContext != null && httpContext.Request.Url != null
                       ? string.Format("_#url:{0}", httpContext.Request.Url.PathAndQuery)
                       : string.Empty;
        }
    }
}
