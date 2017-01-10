using System;
using System.Collections.Generic;
using System.Reflection;
using BoC;
using BoC.InversionOfControl;
using BoC.InversionOfControl.SimpleInjector;
using Microsoft.Extensions.DependencyInjection;

namespace Sitecore.Foundation.BoC.IoC
{
    using System.Web.Http;
    using SimpleInjector;
    using Sitecore.DependencyInjection;

    public class BoCServiceProviderBuilder : BaseServiceProviderBuilder
    {
        protected override IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection)
        {
            InitBoc.Disable();
            //SetDefaultMvcDependencyResolver.Disabled = true;
            global::BoC.Web.Mvc.Init.SetDefaultMvcDependencyResolver.Disabled = true;

            var appdomainHelper = InitBoc.CreateSitecoreAppDomainHelper();
            
            var resolver = Initializer.CreateDependencyResolver(new[] { appdomainHelper });
            var simpleInjector = resolver as SimpleInjectorDependencyResolver;
            if (simpleInjector != null)
            {
                simpleInjector.AllowLifestyleMismatchVerification(); //sitecore references transient objects from singletons!
            }
            resolver.Populate(serviceCollection);
            Initializer.Execute(resolver, appdomainHelper);

            return global::BoC.InversionOfControl.IoC.Resolver.Resolve<IServiceProvider>();
        }
    }

    public class BocServiceProvider : IServiceProvider
    {
        private readonly IDependencyResolver _resolver;

        public BocServiceProvider(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                return null;
            if (serviceType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(serviceType.GetGenericTypeDefinition()))
            {
                var type = serviceType.GetGenericArguments()[0];

                return IsResolveable(type) ? _resolver.ResolveAll(type) : Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
            }
            return IsResolveable(serviceType) ? _resolver.Resolve(serviceType) : null;
        }

        private bool IsResolveable(Type type)
        {
            if (type == null)
                return false;
            //if the type comes from a sitecore dll it HAS to be registered
            if (type.Assembly.FullName.StartsWith("sitecore.", StringComparison.InvariantCultureIgnoreCase) &&
                !type.Assembly.FullName.StartsWith("sitecore.feature", StringComparison.InvariantCultureIgnoreCase) &&
                !type.Assembly.FullName.StartsWith("sitecore.foundation", StringComparison.InvariantCultureIgnoreCase))
                return _resolver.IsRegistered(type);
            return !type.IsAbstract && !type.IsInterface || _resolver.IsRegistered(type);
        }
    }

    public class BoCServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IDependencyResolver _resolver;

        public BoCServiceScopeFactory(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public IServiceScope CreateScope()
        {
            return new BoCServiceScope(_resolver.CreateChildResolver());
        }
    }

    public class BoCServiceScope : IServiceScope
    {
        private readonly IDependencyResolver _resolver;

        public BoCServiceScope(IDependencyResolver resolver)
        {
            _resolver = resolver;
            ServiceProvider = _resolver.Resolve<IServiceProvider>();
        }

        public void Dispose()
        {
            _resolver?.Dispose();
        }

        public IServiceProvider ServiceProvider { get; }
    }

    public static class BoCDependencyRegistration
    {
        /// <summary>
        /// Populates the container builder with the set of registered service descriptors
        /// and makes <see cref="IServiceProvider"/> and <see cref="IServiceScopeFactory"/>
        /// available in the container.
        /// </summary>
        /// <param name="resolver">
        /// The ContainerBuilder into which the registrations should be made.
        /// </param>
        /// <param name="descriptors">
        /// The set of service descriptors to register in the container.
        /// </param>
        public static void Populate(this IDependencyResolver resolver, IEnumerable<ServiceDescriptor> descriptors)
        {
            resolver.RegisterSingleton<IServiceProvider, BocServiceProvider>();
            resolver.RegisterSingleton<IServiceScopeFactory, BoCServiceScopeFactory>();

            Register(resolver, descriptors);
        }

        /// <summary>
        /// Populates the container builder with the set of registered service descriptors.
        /// </summary>
        /// <param name="resolver">
        /// The ContainerBuilder into which the registrations should be made.
        /// </param>
        /// <param name="descriptors">
        /// The set of service descriptors to register in the container.
        /// </param>
        private static void Register(
            IDependencyResolver resolver,
            IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                if (descriptor.ImplementationType != null)
                {
                    if (descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        resolver.RegisterSingleton(descriptor.ServiceType, descriptor.ImplementationType);
                    }
                    else
                    {
                        resolver.RegisterType(descriptor.ServiceType, descriptor.ImplementationType, GetLifeTimeScope(descriptor.Lifetime));
                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    resolver.RegisterFactory(descriptor.ServiceType, () => descriptor.ImplementationFactory(resolver.Resolve<IServiceProvider>()), GetLifeTimeScope(descriptor.Lifetime));
                }
                else
                {
                    var registerInstanceMethod = resolver.GetType().GetMethod("RegisterInstance", BindingFlags.Instance | BindingFlags.Public);
                    registerInstanceMethod = registerInstanceMethod.MakeGenericMethod(descriptor.ServiceType);
                    registerInstanceMethod.Invoke(resolver, new[] { descriptor.ImplementationInstance });
                }
            }
        }

        private static LifetimeScope GetLifeTimeScope(ServiceLifetime lifecycleKind)
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Scoped:
                    return LifetimeScope.PerHttpRequest;
                //case ServiceLifetime.Transient:
                //case ServiceLifetime.Singleton:
                default:
                    return LifetimeScope.Transient;
            }
        }
    }
}