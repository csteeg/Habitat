using System.Web;

namespace Sitecore.Foundation.SitecoreExtensions.Providers
{
    public interface IHttpContextBaseProvider
    {
        HttpContextBase GetCurrentHttpContext();
    }
}
