using BoC.InversionOfControl;
using Valtech.Foundation.CommerceAbstractions.Repositories;
using Valtech.Foundation.CommerceAbstractions.Services;

namespace Valtech.Foundation.CommerceAbstractions
{
	public class RegisterDependencies: IContainerInitializer
	{
		private readonly IDependencyResolver _resolver;

		public RegisterDependencies(IDependencyResolver resolver)
		{
			this._resolver = resolver;
		}

		public void Execute()
        {
            this._resolver.RegisterType<IShopContextService, ShopContextService>();
        }
	}
}