using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Feature.Cart.ViewModels;
using Valtech.Foundation.CommerceConnect.Services;
using Sitecore.Foundation.SitecoreExtensions.Attributes;
using Valtech.Foundation.CommerceAbstractions.Services;
using Sitecore.Feature.Cart.Extensions;

namespace Sitecore.Feature.Cart.Controllers.Api
{
    [DisableDiagnosticOutput]
    public class CartShippingController : ApiController
    {
        private readonly ICartService _cartService;
	    private readonly IShopContextService _shopContextService;
        private readonly IShippingService _shippingService;

        public CartShippingController(ICartService cartService, IShopContextService shopContextService, IShippingService shippingService)
        {
            this._cartService = cartService;
	        this._shopContextService = shopContextService;
            this._shippingService = shippingService;
        }

		[HttpGet]
		public IHttpActionResult Get()
	    {
		    return Get(null);
	    }

		// GET api/<controller>
		[HttpGet]
		public IHttpActionResult Get(string id)
        {
            var result = this._cartService.GetCart(
				_shopContextService.GetCurrentShopName(),
				_shopContextService.GetCurrentUserId(), 
				id ?? _shopContextService.GetCurrentCartId(),
				false);
			
            var cart = result.CheckCartResult(true);
			if (cart == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "Could not get the cart");

			return new JsonResult<IEnumerable<ShippingOption>>(_shippingService.GetShippingOptions(cart), 
				new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
        }

        /// <summary>
        /// POST api/cart
        /// Adds a product to the cart
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel">the add to cart view model</param>
        /// <returns>true if the product was added</returns>
        [HttpPut]
        public IHttpActionResult Put([FromBody]int shippingMethodId, string id = null)
        {
			var cart = this._cartService.GetCart(
				_shopContextService.GetCurrentShopName(),
				_shopContextService.GetCurrentUserId(),
				id ?? _shopContextService.GetCurrentCartId(),
				false);
			if (cart == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "Could not get the cart");

			if (cart.BuyerCustomerParty == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "The cart does not have a buyer party yet");

			if (ModelState.IsValid && shippingMethodId > 0 && cart.BuyerCustomerParty != null)
            {
                this._cartService.SetShippingInfo(cart, shippingMethodId + "");

				return new JsonResult<Commerce.Entities.Carts.Cart>(
					this._cartService.GetCart(
						_shopContextService.GetCurrentShopName(),
						_shopContextService.GetCurrentUserId(),
						id ?? _shopContextService.GetCurrentCartId(),
						false)
						.CheckCartResult(),
					new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
			}

			throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid modelstate");
		}

    }

}