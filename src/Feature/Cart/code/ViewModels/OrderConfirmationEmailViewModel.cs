using Sitecore.Commerce.Entities.Orders;
using Sitecore.Feature.Cart.Models;

namespace Sitecore.Feature.Cart.ViewModels
{
	public class OrderConfirmationEmailViewModel
	{
		public OrderConfirmationEmail DataSource { get; set; }
		public Order Order { get; set; }
	}
}