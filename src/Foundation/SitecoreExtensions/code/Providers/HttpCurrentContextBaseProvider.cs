using System.Web;

namespace Sitecore.Foundation.SitecoreExtensions.Providers
{
    public class HttpCurrentContextBaseProvider : IHttpContextBaseProvider
    {
        public HttpContextBase GetCurrentHttpContext()
        {
            return HttpContext.Current != null ? new HttpContextWrapper(HttpContext.Current) : null;
        }
    }
}
