using BoC.InversionOfControl;
using Sitecore.Feature.Search.Repositories;
using Sitecore.Foundation.SitecoreExtensions.Repositories;

namespace Sitecore.Feature.Search.App_Start
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
            if(!_resolver.IsRegistered<ISearchContextRepository>())
                this._resolver.RegisterType<ISearchContextRepository, SearchContextRepository>();
        }
    }
}