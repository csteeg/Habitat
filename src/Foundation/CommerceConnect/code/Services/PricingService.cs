using Sitecore.Commerce.Entities.Prices;
using Sitecore.Commerce.Services.Prices;
using System.Collections.Generic;
using System.Linq;
using Valtech.Foundation.CommerceAbstractions.Services;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public class PricingService : IPricingService
	{
		private readonly PricingServiceProvider _serviceProvider;

		public PricingService(PricingServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public string GetBySku(string sku)
		{
			return GetBySku(sku, "List Price Incl Tax");
		}

		/// <summary>
		/// Get price of product
		/// </summary>
		/// <param name="sku"></param>
		/// <param name="type">
		/// Available types:
		/// - List Price
		/// - List Price Incl Tax
		/// - List Price Excl Tax
		/// - Customer Price
		/// - Customer Price Incl Tax
		/// - Customer Price Excl Tax
		/// </param>
		/// <returns></returns>
		public string GetBySku(string sku, string type)
		{
			var request = new GetProductPricesRequest(sku);
			if (request == null)
				return string.Empty;
			var result = _serviceProvider.GetProductPrices(request);
			if (result == null)
				return string.Empty;
			var listPrice = result.Prices.FirstOrDefault(p => p.Key.Equals(type));
			if (listPrice.Equals(new KeyValuePair<string, Price>()) || listPrice.Value == null)
				return string.Empty;
			return result.Prices.FirstOrDefault(p => p.Key.Equals(type)).Value.Description;
		}
	}
}