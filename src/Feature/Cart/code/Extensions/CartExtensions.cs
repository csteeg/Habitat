using System.Net;
using System.Web;

namespace Sitecore.Feature.Cart.Extensions
{
	public static class CartExtensions
	{
		public static Commerce.Entities.Carts.Cart CheckCartResult(this Commerce.Entities.Carts.Cart cart, bool allowNull = false)
		{
			if (!allowNull && cart == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "Could not get current cart");

			if (cart?.Properties["_Basket_Errors"] != null)
			{
				throw new HttpException((int)HttpStatusCode.BadRequest, cart.Properties["_Basket_Errors"] + "");
			}

			//TODO: add check if the cart belongs to the user?

			return cart;
		}
	}
}