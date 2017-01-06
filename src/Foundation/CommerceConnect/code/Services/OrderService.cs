using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Orders;
using Sitecore.Commerce.Services.Orders;
using System.Collections.Generic;

namespace Valtech.Foundation.CommerceConnect.Services
{
	public class OrderService : IOrderService
	{
		private readonly OrderServiceProvider _serviceProvider;

		public OrderService(OrderServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public Order SubmitVisitorOrder(Cart cart)
		{
			var request = new SubmitVisitorOrderRequest(cart);
			return _serviceProvider.SubmitVisitorOrder(request)?.Order;
		}

		public Order GetVisitorOrder(string orderId, string customerId, string shopName)
		{
			var request = new GetVisitorOrderRequest(orderId, customerId, shopName);
			return _serviceProvider.GetVisitorOrder(request)?.Order;
		}

		public IReadOnlyCollection<OrderHeader> GetVisitorOrders(string customerId, string shopName)
		{
			var request = new GetVisitorOrdersRequest(customerId, shopName);
			return _serviceProvider.GetVisitorOrders(request)?.OrderHeaders;
		}

		public Order VisitorCancelOrder(string orderId, string customerId, string shopName)
		{
			var request = new VisitorCancelOrderRequest(orderId, customerId, shopName);
			return _serviceProvider.VisitorCancelOrder(request)?.CancelledOrder;
		}
	}
}