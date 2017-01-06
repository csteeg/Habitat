using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using System.Collections.ObjectModel;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public interface IShippingService
	{
		ReadOnlyCollection<ShippingOption> GetShippingOptions(Cart cart);
	}
}