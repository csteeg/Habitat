using BoC.InversionOfControl;
using Sitecore.Foundation.Multisite.Providers;
using Sitecore.Foundation.Multisite.Services;

namespace Sitecore.Foundation.Multisite.App_Start
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
			if (!_resolver.IsRegistered<ISiteDefinitionsProvider>())
				this._resolver.RegisterType<ISiteDefinitionsProvider, SiteDefinitionsProvider>();
			if (!_resolver.IsRegistered<ISiteSettingsProvider>())
				this._resolver.RegisterType<ISiteSettingsProvider, SiteSettingsProvider>();
			if (!_resolver.IsRegistered<IDatasourceProvider>())
				this._resolver.RegisterType<IDatasourceProvider, DatasourceProvider>();
			if (!_resolver.IsRegistered<ICountryService>())
				this._resolver.RegisterType<ICountryService, CountryService>();
		}
	}
}