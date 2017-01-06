using System;
using BoCDependencyResolver = BoC.Web.Mvc.IoC.BoCDependencyResolver;
using IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;
using IEventAggregator = BoC.EventAggregator.IEventAggregator;

namespace Sitecore.Foundation.BoC.IoC
{

  public class HabitatSpecificResolver : BoCDependencyResolver
  {
        public HabitatSpecificResolver(IDependencyResolver resolver, IEventAggregator eventAggregator) : base(resolver, eventAggregator)
        {
        }

        protected HabitatSpecificResolver(IDependencyResolver resolver) : base(resolver)
        {
        }


        public override object GetService(Type serviceType)
        {
            //sitecore "injects" it's own dependencies in it's controllers (and others), so don't resolve sitecore classes, just construct them
            if (!serviceType.Assembly.FullName.StartsWith("sitecore.feature.", StringComparison.InvariantCultureIgnoreCase)
                && !serviceType.Assembly.FullName.StartsWith("sitecore.foundation.", StringComparison.InvariantCultureIgnoreCase)
                && serviceType.Assembly.FullName.StartsWith("sitecore.", StringComparison.InvariantCultureIgnoreCase))
            {
                return Activator.CreateInstance(serviceType);
            }
            return base.GetService(serviceType);
        }

        public override System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return new HabitatSpecificResolver(this._resolver.CreateChildResolver());
        }
    }
}