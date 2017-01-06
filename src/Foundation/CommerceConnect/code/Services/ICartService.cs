using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using System.Collections.Generic;

namespace Valtech.Foundation.CommerceConnect.Services
{
    public interface ICartService
    {
        Cart AddToCart(Cart cart, string productId, uint quantity);
        Cart GetCart(string shopName, string userId, string cartId, bool create = false);

	    /// <summary>
	    /// Removes product from the visitor's cart.
	    /// </summary>
	    /// <param name="cart">The cart.</param>
	    /// <param name="cartLineNumber">The cart line number.</param>
	    /// <returns>
	    /// the cart
	    /// </returns>
	    Cart RemoveFromCart(Cart cart, string cartLineNumber);

	    /// <summary>
	    /// Changes the visitor's cart line quantity.
	    /// </summary>
	    /// <param name="cart">The cart.</param>
	    /// <param name="cartLineId">The cart line number.</param>
	    /// <param name="quantity">the new quantity</param>
	    /// <returns>
	    /// the cart
	    /// </returns>
	    Cart ChangeLineQuantity(Cart cart, string cartLineId, uint quantity);

		Cart AddParties(Cart cart, List<Party> parties);

		Cart RemoveParties(Cart cart, List<Party> parties);

		Cart UpdateCart(Cart cart, Cart changes);

		Cart SetShippingInfo(Cart cart, string shippingMethodId);

		Cart SetPaymentInfo(Cart cart, string paymentMethodId);

        Cart SetParties(Cart cart, Party accountingParty, Party buyingCustomerParty);

        Cart SaveCart(Cart cart);

		bool LockCart(Cart cart);

		bool DeleteCart(Cart cart);

        void RemoveFromEaState(Cart cart);

    }
}