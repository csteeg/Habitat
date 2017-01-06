using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Sitecore.Foundation.Redirects.Models
{
    [SitecoreType(true, "{b09b6173-afd1-478c-84bf-efb2cac7ed7b}")]
    public class RegexToLinkRedirect : BaseRedirect
    {
        [SitecoreField("{7f1b918b-4982-4e41-9281-747773a4763f}", SitecoreFieldType.SingleLineText, "Redirect fields", IsShared = true, IsRequired = true)]
        public virtual string SourceRegex { get; set; }
    }
}
