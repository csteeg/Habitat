using BoC.InversionOfControl;
using Sitecore.Foundation.Indexing.Models;
using Sitecore.Foundation.Indexing.Repositories;

namespace Sitecore.Foundation.Indexing.App_Start
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
            if (!_resolver.IsRegistered<ISearchServiceRepository>())
                this._resolver.RegisterType<ISearchServiceRepository, SearchServiceRepository>();
            if (!_resolver.IsRegistered<ISearchSettings>())
                this._resolver.RegisterType<ISearchSettings, SearchSettingsBase>();
        }
    }
}