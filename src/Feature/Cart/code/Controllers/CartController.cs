using BoC.Sitecore.CodeFirstRenderings.DataProviders;
using System.Web.Mvc;
using Sitecore.Foundation.CodeFirstRenderings.Attributes;
using Valtech.Foundation.CommerceAbstractions.Repositories;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using System;
using System.Linq;
using System.Net;
using System.Web;
using Sitecore.Feature.Cart.Models;
using Sitecore.Feature.Cart.ViewModels;
using Valtech.Foundation.CommerceConnect.Services;
using Sitecore.Commerce.Entities;
using System.Collections.Generic;
using Valtech.Foundation.CommerceAbstractions.Services;
using Sitecore.Commerce.Entities.Shipping;
using Sitecore.Commerce.Entities.Payments;
using Sitecore.Commerce.Entities.Carts;
using System.Collections.ObjectModel;
using Valtech.Foundation.CommerceAbstractions.Providers;

namespace Sitecore.Feature.Cart.Controllers
{
    public class CartController : Controller
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly ISitecoreContextProvider _contextProvider;
		private readonly ICartService _cartService;
		private readonly IShopContextService _shopContextService;
		private readonly ICountryService _countryService;
		private readonly IPaymentService _paymentService;
        private readonly IPaymentProvider _paymentProvider;
		private readonly IOrderService _orderService;

		public CartController(
			ICatalogRepository catalogRepository, 
			ISitecoreContextProvider contextProvider, 
			ICartService cartService, 
			IShopContextService shopContextService,
			ICountryService countryService, 
			IPaymentService paymentService, 
			IPaymentProvider paymentProvider,
			IOrderService orderService)
        {
            this._catalogRepository = catalogRepository;
            this._contextProvider = contextProvider;
			this._cartService = cartService;
			this._shopContextService = shopContextService;
			this._countryService = countryService;
			this._paymentService = paymentService;
			this._paymentProvider = paymentProvider;
			this._orderService = orderService;
		}

        [Cacheable(VaryByData = false, VaryByQueryString = true)]
        [JavascriptAsset("/scripts/cart/ko-viewmodels.js")]
        [DataSourceLocation("query:./ancestor-or-self::*[@@templatekey='site root']/Global")]
        [FieldValue(Constants.FieldIds.SupportsLocalDataSource, "1")]
        public ActionResult CartOverview(CartOverview dataSource)
        {
            return View(dataSource);
        }

		[Cacheable(VaryByData = true, VaryByQueryString = true)]
		[JavascriptAsset("/scripts/cart/ko-viewmodels.js")]
		public ActionResult OrderForm()
		{
			var viewModel = new OrderViewModel()
			{
				CountryOptions = new SelectList(_countryService.GetAvailableCountries(), "Name", "Name"),
				PaymentOptions = new List<PaymentOption>(_paymentService.GetPaymentOptions(this._shopContextService.GetCurrentShopName()))
			};

			return View(viewModel);
		}

		[HttpPost]
		[JavascriptAsset("/scripts/cart/ko-viewmodels.js")]
		public ActionResult OrderForm(OrderPostViewModel viewModel)
		{
			if (ModelState.IsValid)
			{
				//Add accounting party
				var cart =this._cartService.GetCart(
					_shopContextService.GetCurrentShopName(),
					_shopContextService.GetCurrentUserId(),
					_shopContextService.GetCurrentCartId(),
					false);

				if (cart == null)
					throw new HttpException((int)HttpStatusCode.NotFound, "Could not get a new cart");

			    if (cart.Shipping.Any() && cart.BuyerCustomerParty != null)
			    {
			        this._shopContextService.EnsureCartCookie(cart.ExternalId);

			        //Start payment
			        _paymentProvider.CreatePayment(cart.ExternalId, int.Parse(viewModel.PaymentOption), true);
			        //_paymentProvider.RequestPayments();
			        return new EmptyResult();
			    }
			}

			return View(new OrderViewModel()
			{
				CountryOptions = new SelectList(_countryService.GetAvailableCountries(), "Name", "Name"),
				PaymentOptions = new List<PaymentOption>(_paymentService.GetPaymentOptions(this._shopContextService.GetCurrentShopName()))
			});
		}

        [Cacheable(VaryByData = true, VaryByQueryString = true)]
        [FieldValue(global::Sitecore.Foundation.SitecoreExtensions.Constants.FieldIds.Caching.VaryByUrl, "1")]
        [DataSourceLocation("query:./ancestor-or-self::*[@@templatekey='site root']/Global")]
        [FieldValue(Constants.FieldIds.SupportsLocalDataSource, "1")]
        [JavascriptAsset("/scripts/cart/ko-viewmodels.js")]
        public ActionResult ProductVariants(string wildcardItem, ProductVariants dataSource)
        {
            if (dataSource == null)
                return View(dataSource);

            SetProductDataSource(wildcardItem, dataSource);
            return View(dataSource);
        }

        private void SetProductDataSource(string wildcardItem, IProductDetailSource dataSource)
        {
            if (dataSource.Product == null && !string.IsNullOrEmpty(wildcardItem))
            {
                var sku = wildcardItem.Split(new[] { Constants.Settings.UrlSkuSplitter }, StringSplitOptions.RemoveEmptyEntries).Last();
                dataSource.Product = this._catalogRepository.GetBySku(sku);
            }
            if (dataSource?.Product == null && this._contextProvider.PageModeIsNormal())
                throw new HttpException((int)HttpStatusCode.NotFound, string.Format("Product with sku {0} not found", wildcardItem));

            if (dataSource.Product == null)
            {
                dataSource.Product = this._catalogRepository.Query().Where(p => p.DisplayOnSite == true).FirstOrDefault();
            }
        }

		[Cacheable(VaryByData =true, VaryByQueryString =true)]
		[FieldValue(global::Sitecore.Foundation.SitecoreExtensions.Constants.FieldIds.Caching.VaryByUrl, "1")]
		[DataSourceLocation("query:./ancestor-or-self::*[@@templatekey='site root']/Global")]
		[FieldValue(Constants.FieldIds.SupportsLocalDataSource, "1")]
		public ActionResult OrderConfirmationEmail(string orderNumber, string orderGuid, OrderConfirmationEmail dataSource)
		{
			if(string.IsNullOrEmpty(orderGuid))
				throw new HttpException((int)HttpStatusCode.NotFound, "No OrderGuid was given in the URL");
			if (dataSource == null)
				throw new HttpException((int)HttpStatusCode.NotFound, "No DataSource was set");

			var order = _orderService.GetVisitorOrder(orderGuid, "0", _shopContextService.GetCurrentShopName());

			if (order == null)
				throw new HttpException((int)HttpStatusCode.NotFound, $"No Order was found for id {orderGuid}");

			return View(new OrderConfirmationEmailViewModel {
				DataSource = dataSource,
				Order = order
			});
		}

		//[Cacheable(VaryByData = true, VaryByQueryString = true)]
		public ActionResult CloseCart(string orderId)
		{
			if (this._contextProvider.PageModeIsNormal())
			{
			    var cart = this._cartService.GetCart(
			        this._shopContextService.GetCurrentShopName(),
			        this._shopContextService.GetCurrentUserId(),
			        this._shopContextService.GetCurrentCartId(),
			        false);
				if (cart != null)
					this._cartService.RemoveFromEaState(cart);

				_shopContextService.DeleteCartCookie();
			    return new EmptyResult();
			}

			return View();
		}
	}
}