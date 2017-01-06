using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Payments;
using System.Collections.ObjectModel;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public interface IPaymentService
	{
		ReadOnlyCollection<PaymentOption> GetPaymentOptions(string shopName);
	}
}