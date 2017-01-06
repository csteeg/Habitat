using Valtech.Foundation.CommerceAbstractions.Models;

namespace Sitecore.Feature.Cart.Models
{
	public interface IProductDetailSource
	{
		ProductBase Product { get; set; }
	}
}