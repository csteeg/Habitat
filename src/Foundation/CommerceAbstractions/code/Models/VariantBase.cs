using BoC.Persistence.SitecoreGlass.Models;
using Sitecore.ContentSearch;

namespace Valtech.Foundation.CommerceAbstractions.Models
{
    public class VariantBase : SitecoreItem
    {
        public virtual string SKU { get; set; }
        [IndexField("internal_name")]
        public virtual string InternalName { get; set; }
        [IndexField("display_name")]
        public override string DisplayName { get; set; }
        [IndexField("short_description")]
        public virtual string ShortDescription { get; set; }
        [IndexField("long_description")]
        public virtual string LongDescription { get; set; }
    }
}