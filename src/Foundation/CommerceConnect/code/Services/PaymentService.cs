using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Services.Payments;
using System.Collections.ObjectModel;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public class PaymentService : IPaymentService
	{
		private readonly PaymentServiceProvider _serviceProvider;

		public PaymentService(PaymentServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public ReadOnlyCollection<PaymentOption> GetPaymentOptions(string shopName)
		{
			var request = new GetPaymentOptionsRequest(shopName, null);
			return _serviceProvider.GetPaymentOptions(request).PaymentOptions;
		}
	}
}