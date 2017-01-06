using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Services.Shipping;
using System.Collections.ObjectModel;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public class ShippingService : IShippingService
	{
		private readonly ShippingServiceProvider _serviceProvider;

		public ShippingService(ShippingServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ReadOnlyCollection<ShippingOption> GetShippingOptions(Cart cart)
		{
			var request = new GetShippingOptionsRequest() { Cart = cart };
			return _serviceProvider.GetShippingOptions(request).ShippingOptions;
		}
	}
}