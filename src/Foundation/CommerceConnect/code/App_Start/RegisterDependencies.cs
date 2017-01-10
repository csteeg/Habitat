namespace Valtech.Foundation.CommerceConnect
{
    using BoC.InversionOfControl;
    using Sitecore.Commerce.Data.Carts;
    using Sitecore.Commerce.Entities;
    using Sitecore.Commerce.Services.Carts;
    using Sitecore.Commerce.Services.Inventory;
    using Sitecore.Commerce.Services.Orders;
    using Sitecore.Commerce.Services.Payments;
    using Sitecore.Commerce.Services.Shipping;
    using Sitecore.Configuration;
    using Valtech.Foundation.CommerceAbstractions.Services;
    using Valtech.Foundation.CommerceConnect.Services;

    public class RegisterDependencies: IContainerInitializer
	{
		private readonly IDependencyResolver _resolver;

		public RegisterDependencies(IDependencyResolver resolver)
		{
			this._resolver = resolver;
		}

		public void Execute()
        {
            this._resolver.RegisterType<ICartService, CartService>();
            this._resolver.RegisterType<IInventoryService, InventoryService>();
			this._resolver.RegisterType<IPricingService, PricingService>();
			this._resolver.RegisterType<IShippingService, ShippingService>();
			this._resolver.RegisterType<IPaymentService, PaymentService>();
			this._resolver.RegisterType<IOrderService, OrderService>();
			this._resolver.RegisterFactory<IEntityFactory>(() => Factory.CreateObject("entityFactory", true) as IEntityFactory);
			this._resolver.RegisterFactory(() => (EaStateCartRepository)Factory.CreateObject("eaStateCartRepository", true));
			this._resolver.RegisterFactory(() => (CartServiceProvider)Factory.CreateObject("cartServiceProvider", true));
			this._resolver.RegisterFactory(() => (OrderServiceProvider)Factory.CreateObject("orderServiceProvider", true));
			this._resolver.RegisterFactory(() => (ShippingServiceProvider)Factory.CreateObject("shippingServiceProvider", true));
			this._resolver.RegisterFactory(() => (PaymentServiceProvider)Factory.CreateObject("paymentServiceProvider", true));
			this._resolver.RegisterFactory(() => (InventoryServiceProvider)Factory.CreateObject("inventoryServiceProvider", true));
        }
	}
}