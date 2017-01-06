using BoC.Logging;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Commerce.Data.Carts;

namespace Valtech.Foundation.CommerceConnect.Services
{
    public class CartService : ICartService
    {
        private readonly CartServiceProvider _serviceProvider;
        private readonly IEntityFactory _entityFactory;
        private readonly ILogger _logger;
        private readonly EaStateCartRepository _cartRepository;

        public CartService(CartServiceProvider serviceProvider, IEntityFactory entityFactory, ILogger logger, EaStateCartRepository cartRepository)
        {
            _serviceProvider = serviceProvider;
            _entityFactory = entityFactory;
            _logger = logger;
            _cartRepository = cartRepository;
        }

        public Cart AddToCart(Cart cart, string productId, uint quantity)
        {
            Assert.IsNotNullOrEmpty(productId, "ProductId");

            if (cart == null)
                return null;

            var cartLine = new CartLine
            {
                Quantity = quantity,
                Product = _entityFactory.Create<CartProduct>("CartProduct")
            };
            cartLine.Product.ProductId = productId;

            var request = new AddCartLinesRequest(cart, new[] { cartLine });

            var cartResult = _serviceProvider.AddCartLines(request);
            if (cartResult == null || cartResult.Cart == null)
                return null;

            return cartResult?.Cart;
        }

		/// <summary>
		/// Removes product from the visitor's cart.
		/// </summary>
		/// <param name="cart">The cart.</param>
		/// <param name="cartLineNumber">The cart line number.</param>
		/// <returns>
		/// the cart
		/// </returns>
		public Cart RemoveFromCart(Cart cart, string cartLineId)
		{
			Assert.ArgumentNotNull(cart, "cart");
			Assert.ArgumentNotNull(cartLineId, "externalCartLineId");
		    var line = cart.Lines.FirstOrDefault(l => l.ExternalCartLineId == cartLineId);
		    if (line == null)
		        return cart;
			var request = new RemoveCartLinesRequest(cart, new[] { line });

			var cartResult = _serviceProvider.RemoveCartLines(request);
			return cartResult?.Cart;
		}

		/// <summary>
		/// Changes the visitor's cart line quantity.
		/// </summary>
		/// <param name="cart">The cart.</param>
		/// <param name="cartLineId">The cart line number.</param>
		/// <param name="quantity">the new quantity</param>
		/// <returns>
		/// the cart
		/// </returns>
		public Cart ChangeLineQuantity(Cart cart, string cartLineId, uint quantity)
		{
			Assert.ArgumentNotNull(cart, "cart");
			Assert.ArgumentNotNull(cartLineId, "cartLineId");

			if (quantity == 0)
			{
				return RemoveFromCart(cart, cartLineId);
			}

            var line = cart.Lines.FirstOrDefault(l => l.ExternalCartLineId == cartLineId);
            if (line == null)
                return cart;
		    line.Quantity = quantity;
            var updateRequest = new UpdateCartLinesRequest(cart, cart.Lines);
			var cartResult = _serviceProvider.UpdateCartLines(updateRequest);

			return cartResult.Cart;
		}
		
		public Cart GetCart(string shopName, string userId, string cartId, bool create = false)
        {
            if (!create && string.IsNullOrEmpty(cartId))
                return null;
            if (string.IsNullOrEmpty(userId))
                return null;//no anonymous carts in etrade
            try
            {
                var cartResult = string.IsNullOrEmpty(cartId) ? null : _serviceProvider.LoadCart(new LoadCartRequest(shopName, cartId, userId));
                if (cartResult == null && create)
                {
                    var createrequest = new CreateOrResumeCartRequest(shopName, userId, cartId);
                    cartResult = _serviceProvider.CreateOrResumeCart(createrequest);
                }
                if (cartResult == null)
                {
                    _logger.Warn("CartService.GetCart got null result from _serviceProvider.CreateOrResumeCartRequest for user " + userId);
                    return null;
                }
                if (!cartResult.Success)
                {
                    _logger.Warn("CartService.GetCart got invalid result from _serviceProvider.CreateOrResumeCartRequest for user " +
                                 userId);
                    return null;
                }
                if (cartResult.Cart == null)
                {
                    _logger.Warn("CartService.GetCart got null cart from _serviceProvider.CreateOrResumeCartRequest for user " + userId);
                    return null;
                }

	            if (cartResult.Cart != null && string.IsNullOrEmpty(cartResult.Cart.ShopName))
	            {
		            cartResult.Cart.ShopName = shopName;
	            }

                return cartResult.Cart as Cart;
            }
            catch (Exception exc)
            {
                _logger.Error("CartService.GetCart got error while getting cart from _serviceProvider.CreateOrResumeCartRequest for user " + userId, exc);
            }
            return null;
        }

		public Cart AddParties(Cart cart, List<Party> parties)
		{
			var addPartiesRequest = new AddPartiesRequest(cart, parties);
			return _serviceProvider.AddParties(addPartiesRequest)?.Cart;
		}

		public Cart RemoveParties(Cart cart, List<Party> parties)
		{
			var removePartiesRequest = new RemovePartiesRequest(cart, parties);
			return _serviceProvider.RemoveParties(removePartiesRequest)?.Cart;
		}

		public Cart UpdateCart(Cart cart, Cart changes)
		{
			var updateCartRequest = new UpdateCartRequest(cart, changes);
			return _serviceProvider.UpdateCart(updateCartRequest)?.Cart;
		}

		public Cart SaveCart(Cart cart)
		{
			var updateCartRequest = new SaveCartRequest(cart);
			var result = _serviceProvider.SaveCart(updateCartRequest) as CartResult;
		    return result?.Cart ?? cart;
		}

		public Cart SetParties(Cart cart, Party accountingParty, Party buyingCustomerParty)
		{
		    var accountingPartyList = new List<Party> { accountingParty };
			//Add accounting party -> //TODO: should this be updates?
			cart = AddParties(cart, accountingPartyList);

			//Find accounting party in result
			accountingParty = cart.Parties.FirstOrDefault(p => p.PartyId.Equals(accountingParty.PartyId));

			cart.AccountingCustomerParty = new CartParty
			{
				ExternalId = accountingParty.ExternalId,
				PartyID = accountingParty.PartyId
			};

			//Update shipping party
			if (buyingCustomerParty != null)
			{
				//Create new shipping party
			    var shippingPartyList = new List<Party> {buyingCustomerParty};
				cart = AddParties(cart, shippingPartyList);

				//Find shipping party in result
				var shippingParty = cart.Parties.FirstOrDefault(p => p.PartyId.Equals(buyingCustomerParty.PartyId));
				cart.BuyerCustomerParty = new CartParty
				{
					ExternalId = shippingParty.ExternalId,
					PartyID = shippingParty.PartyId
				};
			}
			else
			{
				//Should be same as accounting
				cart.BuyerCustomerParty = new CartParty
				{
					ExternalId = accountingParty.ExternalId,
					PartyID = accountingParty.PartyId
				};
			}

			return SaveCart(cart);
		}

		/// <summary>
		/// In this version we only support 1 shipping option per cart, attached to the buyercustomerparty
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="shippingMethodId"></param>
		/// <returns></returns>
		public Cart SetShippingInfo(Cart cart, string shippingMethodId)
		{
		    if (cart?.BuyerCustomerParty == null)
		        return cart;

		    if (cart.Shipping.Any())
		    {
		        if (cart.Shipping.Count > 1)//we only support 1 shipping option for now!
		        {
		            cart = _serviceProvider.RemoveShippingInfo(new RemoveShippingInfoRequest(cart, cart.Shipping.Skip(1).ToList()))?.Cart;
		        }
		        var shipping = cart?.Shipping?.FirstOrDefault();
		        if (shipping == null)
		            return null;
		        shipping.ShippingMethodID = shippingMethodId;
		        shipping.ShippingProviderID = shippingMethodId; //could be we need this, but for ucommerce we set both to external id for now
		        shipping.PartyID = cart.BuyerCustomerParty.PartyID;
		        return SaveCart(cart);
		    }

			var shippingList = new List<ShippingInfo>
			{
				new ShippingInfo(){ ShippingMethodID = shippingMethodId, ShippingProviderID = shippingMethodId, PartyID = cart.BuyerCustomerParty.PartyID}
			};
			var request = new AddShippingInfoRequest(cart, shippingList);
			return _serviceProvider.AddShippingInfo(request)?.Cart;
		}

		/// <summary>
		/// In this version we only support 1 payment option per cart
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="paymentMethodId"></param>
		/// <returns></returns>
		public Cart SetPaymentInfo(Cart cart, string paymentMethodId)
		{
			if (cart?.BuyerCustomerParty == null)
				return cart;

			if (cart.Payment.Any())
			{
				if (cart.Payment.Count > 1)//we only support 1 payment option for now!
				{
					cart = _serviceProvider.RemovePaymentInfo(new RemovePaymentInfoRequest(cart, cart.Payment.Skip(1).ToList()))?.Cart;
				}
				var payment = cart?.Payment?.FirstOrDefault();
				if (payment == null)
					return null;
				payment.PaymentMethodID = paymentMethodId;
				payment.PaymentProviderID = paymentMethodId; //could be we need this, but for ucommerce we set both to external id for now
				payment.PartyID = cart.BuyerCustomerParty.PartyID;
				return SaveCart(cart);
			}

			var paymentList = new List<PaymentInfo>
			{
				new PaymentInfo(){PaymentMethodID = paymentMethodId, PaymentProviderID = paymentMethodId, PartyID = cart.BuyerCustomerParty.PartyID}
			};
			var request = new AddPaymentInfoRequest(cart, paymentList);
			return _serviceProvider.AddPaymentInfo(request)?.Cart;
		}

        public bool LockCart(Cart cart)
        {
            var request = new LockCartRequest(cart);
			return this._serviceProvider.LockCart(request)?.Success ?? false;
        }

        public bool DeleteCart(Cart cart)
        {
            var request = new DeleteCartRequest(cart);
            return this._serviceProvider.DeleteCart(request)?.Success ?? false;
        }

        public void RemoveFromEaState(Cart cart)
		{
			this._cartRepository.Delete(cart);
		}
	}
}