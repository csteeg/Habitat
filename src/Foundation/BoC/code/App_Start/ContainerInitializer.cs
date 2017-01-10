using System;
using System.Linq;
using IAppDomainHelper = BoC.Helpers.IAppDomainHelper;
using IContainerInitializer = BoC.InversionOfControl.IContainerInitializer;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace Sitecore.Foundation.BoC
{

    //TODO: if we decide features to be BoC'ed, initialize should happen in the feature itself
    public class ContainerInitializer : IContainerInitializer
    {
        private readonly IDependencyResolver resolver;
        private readonly IAppDomainHelper[] appDomainHelpers;

        public ContainerInitializer(IDependencyResolver resolver, IAppDomainHelper[] appDomainHelpers)
        {
            this.resolver = resolver;
            this.appDomainHelpers = appDomainHelpers;
        }


        public void Execute()
        {
            var habitatTypes = this.appDomainHelpers.SelectMany(h => h.GetTypes(
                t => t.FullName.StartsWith("sitecore.feature", StringComparison.InvariantCultureIgnoreCase) || t.FullName.StartsWith("sitecore.foundation", StringComparison.InvariantCultureIgnoreCase)
                )).ToList();
            var habitatInterfaces = habitatTypes.Where(t => t.IsInterface && !this.resolver.IsRegistered(t));
            foreach (var i in habitatInterfaces)
            {
                foreach (var t in habitatTypes.Where(t => !t.IsInterface && i.IsAssignableFrom(t) && !this.resolver.IsRegistered(t)))
                {
                    try
                    {
                        this.resolver.RegisterType(i, t);
                    }
                    catch (ArgumentException)
                    {
                        //TODO: we should have all features register their own types -> these exceptions will slow down startup
                    }

                }
            }
        }
    }
}