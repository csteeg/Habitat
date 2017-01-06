using System.Web.Mvc;
using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.WFFM.Abstractions.Shared;
using BoC.InversionOfControl;
using Sitecore.Forms.Mvc.Controllers.ModelBinders;
using Sitecore.Forms.Mvc.ViewModels;
using Sitecore.Pipelines;

namespace Sitecore.Foundation.Forms.ModelMetaData
{
    public class RegisterBoCMetaDataProvider : IContainerInitializer
    {
        public RegisterBoCMetaDataProvider()
        {
        }

        public void Execute()
        {
            BoC.Web.Mvc.Init.SetDefaultModelMetaDataProvider.SkipRegisterModelMetadataProvider = true;
        }
    }

    public class AddCustomMetadataProvider
    {
        private readonly IPerRequestStorage perRequestStorage;
        private readonly ICorePipeline corePipeline;

        public AddCustomMetadataProvider()
        {

        }

        public AddCustomMetadataProvider(IPerRequestStorage perRequestStorage, ICorePipeline corePipeline)
        {
            Assert.IsNotNull((object)perRequestStorage, "perRequestStorage");
            Assert.IsNotNull((object)corePipeline, "corePipeline");
            this.perRequestStorage = perRequestStorage;
            this.corePipeline = corePipeline;
        }

        [UsedImplicitly]
        public virtual void Process(PipelineArgs args)
        {
            if (this.perRequestStorage == null || this.corePipeline == null)
                return;
            //IoC.Resolver.RegisterInstance<System.Web.Mvc.ModelMetadataProvider>( we cannot use ioc any more at this time
            ModelMetadataProviders.Current = new PipelineBasedDataAnnotationsModelMetadataProvider(this.perRequestStorage, this.corePipeline);
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(SectionViewModel), new SectionModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(FieldViewModel), new FieldModelBinder());
        }
    }
}