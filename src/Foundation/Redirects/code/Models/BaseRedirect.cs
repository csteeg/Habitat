using BoC.Persistence.SitecoreGlass.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using Sitecore.Foundation.Redirects.Enums;
using Sitecore.Foundation.SitecoreExtensions.Controls;

namespace Sitecore.Foundation.Redirects.Models
{
    [SitecoreType(true, "{bffa0451-c653-4678-86c8-5e401d12c5be}")]
    public class BaseRedirect: SitecoreItem
    {
        [SitecoreField("{d6dd290d-abb5-422f-8cb0-ab62802853bc}", SitecoreFieldType.NotSet, "Redirect fields", IsShared = true, IsRequired = true, FieldSource = "Sitecore.Foundation.Redirects.Enums.RedirectType, Sitecore.Foundation.Redirects")]
        [SitecoreFieldFieldValue(Sitecore.Foundation.SitecoreExtensions.Constants.FieldIds.FieldType, EnumerableDropList.FieldType.EnumerableDropList)]
        public virtual RedirectType RedirectType { get; set; }

        [SitecoreField("{22e5381c-3425-4698-b50a-e194864dd58b}", SitecoreFieldType.GeneralLink, "Redirect fields", IsShared = true, IsRequired = true)]
        public virtual Link TargetLink { get; set; }
    }
}
