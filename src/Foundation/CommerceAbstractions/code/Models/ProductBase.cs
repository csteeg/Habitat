using System.Collections.Generic;
using System.Linq;
using BoC.Persistence.SitecoreGlass.Models;
using Glass.Mapper.Sc.Fields;
using Sitecore.ContentSearch;
using Valtech.Foundation.CommerceAbstractions.Services;

namespace Valtech.Foundation.CommerceAbstractions.Models
{
	public class ProductBase: SitecoreItem
	{
		public virtual string SKU { get; set; }
		[IndexField("internal_name")]
		public virtual string InternalName { get; set; }
		[IndexField("display_on_site")]
		public virtual bool DisplayOnSite { get; set; }
		[IndexField("allow_ordering")]
		public virtual bool AllowOrdering { get; set; }
		[IndexField("display_name")]
		public override string DisplayName { get; set; }
		[IndexField("short_description")]
		public virtual string ShortDescription { get; set; }
		[IndexField("long_description")]
		public virtual string LongDescription { get; set; }
		[IndexField("product_bestsortorder")]
		public virtual int BestSortOrder { get; set; }
		//[IndexField(Constants.FieldIndexes.CategoriesList)]
		public virtual IEnumerable<CategoryBase> CategoriesList { get; set; }
		public virtual IEnumerable<ProductBase> RelatedProducts { get; set; }
		public virtual Image ThumbnailImage { get; set; }
		public virtual Image PrimaryImage { get; set; }

		#region Pricing 

		private string _price { get; set; }
		public string GetPrice()
		{
			if (!string.IsNullOrEmpty(_price))
				return _price;

			_price = BoC.InversionOfControl.IoC.Resolver.Resolve<IPricingService>().GetBySku(SKU);
			return _price;
		}
        #endregion

        #region Variants
	    public IEnumerable<VariantBase> Variants
	    {
	        get { return Children.OfType<VariantBase>(); }
	    }
        #endregion
    }
}