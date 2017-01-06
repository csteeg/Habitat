using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using System.Collections.Generic;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public interface IOrderService
	{
		Order SubmitVisitorOrder(Cart cart);
		Order GetVisitorOrder(string orderId, string customerId, string shopName);
		IReadOnlyCollection<OrderHeader> GetVisitorOrders(string customerId, string shopName);
		Order VisitorCancelOrder(string orderId, string customerId, string shopName);
	}
}