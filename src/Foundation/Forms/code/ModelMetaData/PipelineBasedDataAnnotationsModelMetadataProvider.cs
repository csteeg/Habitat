using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Forms.Mvc.Interfaces;
using Sitecore.Forms.Mvc.Pipelines.CreateMetadata;
using Sitecore.Pipelines;
using Sitecore.WFFM.Abstractions.Shared;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BoC.Web.Mvc.MetaData;

namespace Sitecore.Foundation.Forms.ModelMetaData
{

  public class PipelineBasedDataAnnotationsModelMetadataProvider : ExtraModelMetadataProvider
  {
    public const string StorageKey = "wffm.ModelMetadataKey";
    private readonly IPerRequestStorage perRequestStorage;
    private readonly ICorePipeline corePipeline;

    public PipelineBasedDataAnnotationsModelMetadataProvider(IPerRequestStorage perRequestStorage, ICorePipeline corePipeline)
    {
      Assert.ArgumentNotNull((object)perRequestStorage, "perRequestStorage");
      Assert.ArgumentNotNull((object)corePipeline, "corePipeline");
      this.perRequestStorage = perRequestStorage;
      this.corePipeline = corePipeline;
    }

    public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
    {
      IEnumerable<ModelMetadata> metadataForProperties = base.GetMetadataForProperties(container, containerType);
      foreach (ModelMetadata metaData in metadataForProperties)
        this.RunCreateMetadataPipeline(containerType, metaData);
      return metadataForProperties;
    }

    public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
    {
      ModelMetadata metadataForProperty = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);
      this.RunCreateMetadataPipeline(containerType, metadataForProperty);
      return metadataForProperty;
    }

    public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
    {
      ModelMetadata metadataForType = base.GetMetadataForType(modelAccessor, modelType);
      this.SaveModel(modelAccessor, modelType);
      return metadataForType;
    }

    private void SaveModel(Func<object> modelAccessor, Type modelType)
    {
      if (modelAccessor == null || !typeof(IContainerMetadata).IsAssignableFrom(modelType))
        return;
      object obj = modelAccessor();
      if (obj == null)
        return;
      this.perRequestStorage.Put("wffm.ModelMetadataKey", obj);
    }

    private void RunCreateMetadataPipeline(Type containerType, ModelMetadata metaData)
    {
      object model = this.perRequestStorage.Get("wffm.ModelMetadataKey");
      if (model == null)
        return;
      this.corePipeline.Run("wffm.createMetadata", (PipelineArgs)new CreateMetadataArgs(metaData, containerType, model));
    }
  }
}