using BoC.InversionOfControl;
using Valtech.Foundation.Publishing.Services;

namespace Sitecore.Foundation.Publishing.App_Start
{
    public class SetDependencies : IContainerInitializer
    {
        private readonly IDependencyResolver _resolver;

        public SetDependencies(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public void Execute()
        {
			if (!_resolver.IsRegistered<IPublishService>())
				this._resolver.RegisterType<IPublishService, PublishService>();
		}
	}
}