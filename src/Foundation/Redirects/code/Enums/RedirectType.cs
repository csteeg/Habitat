using System.ComponentModel;

namespace Sitecore.Foundation.Redirects.Enums
{
    public enum RedirectType
    {
        [Description("301 Moved Permanently")]
        Permanent = 301,
        [Description("302 Found")]
        Found = 302,
        [Description("Alias")]
        Alias = 0
    }
}
