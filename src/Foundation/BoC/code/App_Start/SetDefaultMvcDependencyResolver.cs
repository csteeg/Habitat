using Sitecore.Foundation.BoC.IoC;
using System.Web.Http;
using System.Web.Mvc;
using IBootstrapperTask = BoC.Tasks.IBootstrapperTask;
using IEventAggregator = BoC.EventAggregator.IEventAggregator;
using Initialize_SetDefaultMvcDependencyResolver = BoC.Sitecore.Mvc.Initialize.SetDefaultMvcDependencyResolver;
using InversionOfControl_IDependencyResolver = BoC.InversionOfControl.IDependencyResolver;

namespace Sitecore.Foundation.BoC
{
  public class SetDefaultMvcDependencyResolver : IBootstrapperTask
  {
    public static bool Disabled = false;
    private readonly HabitatSpecificResolver _dependencyResolver;

    public SetDefaultMvcDependencyResolver(InversionOfControl_IDependencyResolver dependencyResolver, IEventAggregator eventAggregator)
    {
      global::BoC.Web.Mvc.Init.SetDefaultMvcDependencyResolver.Disabled = true;
      this._dependencyResolver = new HabitatSpecificResolver(dependencyResolver, eventAggregator);
    }

    public void Execute()
    {
      if (!Disabled)
      {
        Initialize_SetDefaultMvcDependencyResolver.Disabled = true;
        DependencyResolver.SetResolver(this._dependencyResolver);
        GlobalConfiguration.Configuration.DependencyResolver = this._dependencyResolver;
      }
    }
  }
}