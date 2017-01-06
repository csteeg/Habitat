using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Sitecore.Foundation.Redirects.Models
{
    [SitecoreType(true, "{d401f9d4-05d1-4281-8b4e-9b7d2f27821f}")]
    public class ItemToLinkRedirect : BaseRedirect
    {
        [SitecoreField("{f1f0b863-fa3a-4be8-ae7d-1365d7881dd0}", SitecoreFieldType.DropTree, "Redirect fields", IsShared = true, IsRequired = true)]
        public virtual Guid SourceItem { get; set; }
    }
}
