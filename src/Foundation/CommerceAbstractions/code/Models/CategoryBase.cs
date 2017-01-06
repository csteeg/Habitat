using System.Collections.Generic;
using BoC.Persistence.SitecoreGlass.Models;
using Glass.Mapper.Sc.Fields;
using Sitecore.ContentSearch;

namespace Valtech.Foundation.CommerceAbstractions.Models
{
	public class CategoryBase : SitecoreItem
	{
		public override string Name { get; set; }
		[IndexField("display_name")]
		public override string DisplayName { get; set; }
		public virtual string Description { get; set; }
		public virtual IEnumerable<ProductBase> Products { get; set; }
		public virtual Image Image { get; set; }
	}
}