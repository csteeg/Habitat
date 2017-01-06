using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sitecore.Feature.Cart.ViewModels;
using Valtech.Foundation.CommerceConnect.Services;
using Sitecore.Foundation.SitecoreExtensions.Attributes;
using Valtech.Foundation.CommerceAbstractions.Services;
using Sitecore.Feature.Cart.Extensions;

namespace Sitecore.Feature.Cart.Controllers.Api
{
    [DisableDiagnosticOutput]
    public class CartController : ApiController
    {
        private readonly ICartService _cartService;
	    private readonly IShopContextService _shopContextService;
        private readonly IInventoryService _inventoryService;

        public CartController(ICartService cartService, IShopContextService shopContextService, IInventoryService inventoryService)
        {
            this._cartService = cartService;
	        this._shopContextService = shopContextService;
            this._inventoryService = inventoryService;
        }

		[HttpGet]
		public IHttpActionResult Get(bool includeStockInformation = false)
	    {
		    return Get(null, includeStockInformation);
	    }

		// GET api/<controller>
		[HttpGet]
		public IHttpActionResult Get(string id, bool includeStockInformation = false)
        {
            var result = this._cartService.GetCart(
				_shopContextService.GetCurrentShopName(),
				_shopContextService.GetCurrentUserId(), 
				id ?? _shopContextService.GetCurrentCartId(),
				false);
            if (result != null)
                this._shopContextService.EnsureCartCookie(result.ExternalId);
            var cart = result.CheckCartResult(true);
            if (cart != null && includeStockInformation)
            {
                cart.Properties["StockInformation"] = this._inventoryService.GetStockBySkus(cart.Lines.Select(l => 
                        l.Product.Properties["VariantSku"] as string ?? //TODO: we should not have to access the property variantsku here, the productid should be the variant, the mastercode should be placed in properties
                        l.Product.ProductId));
            }
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
        [HttpPost]
        public IHttpActionResult Post([FromBody]AddToCartViewModel viewModel, string id = null)
        {
			var cart = this._cartService.GetCart(
				_shopContextService.GetCurrentShopName(),
				_shopContextService.GetCurrentUserId(),
				id ?? _shopContextService.GetCurrentCartId(),
				true);
			if (cart == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "Could not get a new cart");

			this._shopContextService.EnsureCartCookie(cart.ExternalId);

            cart = this._cartService.AddToCart(cart, viewModel.ProductId, viewModel.Quantity);

			return new JsonResult<Commerce.Entities.Carts.Cart>(cart.CheckCartResult(),
				new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
        }

        /// <summary>
        /// PUT api/cart
        /// Adds a product to the cart
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel">the add to cart view model</param>
        /// <returns>true if the product was added</returns>
        [HttpPut]
        public IHttpActionResult Put([FromBody]UpdateCartViewModel viewModel, string id = null)
        {
            var cart = this._cartService.GetCart(
                _shopContextService.GetCurrentShopName(),
                _shopContextService.GetCurrentUserId(),
                id ?? _shopContextService.GetCurrentCartId(),
                true);
            if (cart == null)
                throw new HttpException((int)HttpStatusCode.NotFound, "Could not get a new cart");

            this._shopContextService.EnsureCartCookie(cart.ExternalId);

            cart = this._cartService.ChangeLineQuantity(cart, viewModel.CartLineId, viewModel.Quantity);

            return new JsonResult<Commerce.Entities.Carts.Cart>(cart.CheckCartResult(),
                new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
        }

        [HttpDelete]
	    public IHttpActionResult Delete([NotNull][FromBody]string cartLineId, string id = null)
	    {
			var cart = this._cartService.GetCart(
				_shopContextService.GetCurrentShopName(),
				_shopContextService.GetCurrentUserId(),
                id ?? _shopContextService.GetCurrentCartId(),
				true);

		    if (cart == null)
			    return null;

			this._shopContextService.EnsureCartCookie(cart.ExternalId);

			cart = this._cartService.RemoveFromCart(cart, cartLineId);
			return new JsonResult<Commerce.Entities.Carts.Cart>(cart.CheckCartResult(),
				new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
		}
    }

}