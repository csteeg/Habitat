using System.Web.Http;
using BoC.Tasks;

namespace Sitecore.Feature.Cart
{
    using System.Web.Routing;
    using BoC.InversionOfControl;

    public class RegisterWebApiRoutes : IBootstrapperTask
	{
	    private readonly IDependencyResolver resolver;

	    public RegisterWebApiRoutes(IDependencyResolver resolver)
	    {
	        this.resolver = resolver;
	    }

	    public void Execute()
	    {
	        var routeTable = resolver.IsRegistered<RouteCollection>() ? this.resolver.Resolve<RouteCollection>() : RouteTable.Routes;
	        object action = null;
	        if (routeTable["Feature.Cart.Api.Cart.Address"] == null)
	        {
	            routeTable.MapHttpRoute(
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
	        if (routeTable["Feature.Cart.Api.Cart.Shipping"] == null)
	        {
	            routeTable.MapHttpRoute(
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
	        if (routeTable["Feature.Cart.Api.Cart"] == null)
	        {
	            routeTable.MapHttpRoute(
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

	    }
	}
}