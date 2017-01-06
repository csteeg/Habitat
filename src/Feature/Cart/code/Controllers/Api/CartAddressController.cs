using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sitecore.Commerce.Entities;
using Sitecore.Commerce.Entities.Carts;
using Sitecore.Feature.Cart.ViewModels;
using Valtech.Foundation.CommerceConnect.Services;
using Sitecore.Foundation.SitecoreExtensions.Attributes;
using Valtech.Foundation.CommerceAbstractions.Services;
using Sitecore.Feature.Cart.Extensions;

namespace Sitecore.Feature.Cart.Controllers.Api
{
    [DisableDiagnosticOutput]
    public class CartAddressController : ApiController
    {
        private readonly ICartService _cartService;
	    private readonly IShopContextService _shopContextService;

        public CartAddressController(ICartService cartService, IShopContextService shopContextService)
        {
            this._cartService = cartService;
	        this._shopContextService = shopContextService;
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
            if (result != null)
                this._shopContextService.EnsureCartCookie(result.ExternalId);
            var cart = result.CheckCartResult(true);
            return new JsonResult<Commerce.Entities.Carts.Cart>(cart, 
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
        public IHttpActionResult Put([FromBody]OrderViewModel viewModel, string id = null)
        {
			var cart = this._cartService.GetCart(
				_shopContextService.GetCurrentShopName(),
				_shopContextService.GetCurrentUserId(),
				id ?? _shopContextService.GetCurrentCartId(),
				false);
			if (cart == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "Could not get the cart");

            if (ModelState.IsValid)
            {
				cart = this._cartService.SetParties(cart,
                    new Party
                    {
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        Address1 = viewModel.Address,
                        ZipPostalCode = viewModel.ZipCode,
                        City = viewModel.City,
                        Country = viewModel.Country,
                        Email = viewModel.Email,
                        PhoneNumber = viewModel.PhoneNumber,
                        IsPrimary = true
                    },
                    string.IsNullOrEmpty(viewModel.DeliveryAddress) ? null :
                        new Party
                        {
                            FirstName = viewModel.FirstName,
                            LastName = viewModel.LastName,
                            Address1 = viewModel.DeliveryAddress,
                            ZipPostalCode = viewModel.DeliveryZipCode,
                            City = viewModel.DeliveryCity,
                            Country = viewModel.DeliveryCountry,
                            Email = viewModel.Email,
                            PhoneNumber = viewModel.PhoneNumber
                        });


				return new JsonResult<Commerce.Entities.Carts.Cart>(cart.CheckCartResult(),
					new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
			}

			throw new HttpException((int)HttpStatusCode.BadRequest, "Invalid modelstate");
		}

    }

}