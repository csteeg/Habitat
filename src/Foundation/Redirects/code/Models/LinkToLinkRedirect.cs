using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace Sitecore.Foundation.Redirects.Models
{
    [SitecoreType(true, "{f23c52bd-66d1-478d-8fda-86e8e462670b}")]
    public class LinkToLinkRedirect : BaseRedirect
    {
        [SitecoreField("{f10e43b5-fc10-4a29-aace-8a0d389b7572}", SitecoreFieldType.GeneralLink, "Redirect fields", IsShared = true, IsRequired = true)]
        public virtual Link SourceLink { get; set; }

        [SitecoreField("{2a1411be-5929-4337-8425-5581bf815420}", SitecoreFieldType.Checkbox, "Redirect fields", IsShared = true)]
        public virtual bool IgnoreQueryString { get; set; }
    }
}
