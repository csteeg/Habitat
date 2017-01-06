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
			this._resolver.RegisterInstance(Factory.CreateObject("entityFactory", true) as IEntityFactory);
			if (Factory.GetConfigNode("eaStateCartRepository") != null)
				this._resolver.RegisterInstance((EaStateCartRepository)Factory.CreateObject("eaStateCartRepository", true));
			if (Factory.GetConfigNode("cartServiceProvider") != null)
				this._resolver.RegisterInstance((CartServiceProvider)Factory.CreateObject("cartServiceProvider", true));
			if (Factory.GetConfigNode("orderServiceProvider") != null)
				this._resolver.RegisterInstance((OrderServiceProvider)Factory.CreateObject("orderServiceProvider", true));
			if (Factory.GetConfigNode("shippingServiceProvider") != null)
				this._resolver.RegisterInstance((ShippingServiceProvider)Factory.CreateObject("shippingServiceProvider", true));
			if (Factory.GetConfigNode("paymentServiceProvider") != null)
				this._resolver.RegisterInstance((PaymentServiceProvider)Factory.CreateObject("paymentServiceProvider", true));
			if (Factory.GetConfigNode("inventoryServiceProvider") != null)
				this._resolver.RegisterInstance((InventoryServiceProvider)Factory.CreateObject("inventoryServiceProvider", true));
        }
	}
}