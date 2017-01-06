using System.Web.Http;
using BoC.Tasks;

namespace Sitecore.Feature.Cart
{
	public class RegisterWebApiRoutes : IBootstrapperTask
	{
		public void Execute()
		{
			GlobalConfiguration.Configure(config =>
			{
				object action = null;
				if (!config.Routes.ContainsKey("Feature.Cart.Api.Cart.Address"))
				{
					config.Routes.MapHttpRoute(
						name: "Feature.Cart.Api.Cart.Address",
						routeTemplate: "services/v1/cart/{id}/address",
						defaults: new
						{
							id = RouteParameter.Optional,
							controller = "CartAddress",
							action = action //set action to null, so sitecore doesn't add action="index" to the route
						}
					);
				}
				if (!config.Routes.ContainsKey("Feature.Cart.Api.Cart.Shipping"))
				{
					config.Routes.MapHttpRoute(
						name: "Feature.Cart.Api.Cart.Shipping",
						routeTemplate: "services/v1/cart/{id}/shipping",
						defaults: new
						{
							id = RouteParameter.Optional,
							controller = "CartShipping",
							action = action //set action to null, so sitecore doesn't add action="index" to the route
						}
					);
				}
				if (!config.Routes.ContainsKey("Feature.Cart.Api.Cart"))
				{
					config.Routes.MapHttpRoute(
						name: "Feature.Cart.Api.Cart",
						routeTemplate: "services/v1/cart/{id}",
						defaults: new
						{
							id = RouteParameter.Optional,
							controller = "Cart",
							action = action //set action to null, so sitecore doesn't add action="index" to the route
						}
					);
				}
			});
		}
	}
}