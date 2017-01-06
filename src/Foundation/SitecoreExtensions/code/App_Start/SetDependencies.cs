using BoC.InversionOfControl;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Sitecore.Foundation.SitecoreExtensions.Repositories;
using Sitecore.Foundation.SitecoreExtensions.Services;

namespace Sitecore.Foundation.SitecoreExtensions.App_Start
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
            if (!_resolver.IsRegistered<IRenderingPropertiesRepository>())
                this._resolver.RegisterType<IRenderingPropertiesRepository, RenderingPropertiesRepository>();
            if (!_resolver.IsRegistered<ITrackerService>())
                this._resolver.RegisterType<ITrackerService, TrackerService>();
			if (!_resolver.IsRegistered<ISitecoreContextProvider>())
				this._resolver.RegisterType<ISitecoreContextProvider, SitecoreCurrentContextProvider>();
			if (!_resolver.IsRegistered<IHttpContextBaseProvider>())
				this._resolver.RegisterType<IHttpContextBaseProvider, HttpCurrentContextBaseProvider>();
            if (!_resolver.IsRegistered<ITrackerService>())
                this._resolver.RegisterType<ITrackerService, TrackerService>();
        }
    }
}