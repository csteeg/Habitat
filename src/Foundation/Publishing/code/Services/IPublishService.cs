using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using BoC.Persistence.SitecoreGlass.Models;
using Valtech.Foundation.Publishing.Enums;
using Valtech.Foundation.Publishing.Models;

namespace Valtech.Foundation.Publishing.Services
{
    public interface IPublishService
    {
        bool IsPublished(ISitecoreItem itemToCheck);
        bool IsPublished(Item itemToCheck);
        void PublishItem(ISitecoreItem itemToPublish, bool recursive = false);
        void PublishItem(Item itemToPublish, bool recursive = false);
        void UnPublishItem(Item itemToUnPublish);
        void UnPublishItem(ID itemToUnPublish);
        IEnumerable<Target> GetTargets();
    }
}
