using System;
using System.Collections.Generic;
using System.Linq;
using BoC.EventAggregator;
using BoC.Logging;
using BoC.Persistence;
using BoC.Services;
using BoC.Validation;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Events;
using Sitecore.Globalization;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using BoC.Persistence.SitecoreGlass.Models;
using Valtech.Foundation.Publishing.Models;
using Sitecore.Foundation.SitecoreExtensions.Providers;

namespace Valtech.Foundation.Publishing.Services
{
    public class PublishService : BaseModelService<Target>, IPublishService
    {
        private readonly IRepository<Folder> _folderRepository;
        private readonly ILogger _logger;
        private readonly ISitecoreContextProvider _sitecoreContextProvider;

        public PublishService(IModelValidator validator, IEventAggregator eventAggregator,
            IRepository<Target> repository, IRepository<Folder> folderRepository, ILogger logger,
            ISitecoreContextProvider sitecoreContextProvider)
            : base(validator, eventAggregator, repository)
        {
            _sitecoreContextProvider = sitecoreContextProvider;
            _logger = logger;
            _folderRepository = folderRepository;
        }

        public bool IsPublished(ISitecoreItem itemToCheck)
        {
            var database = _sitecoreContextProvider.GetCurrentContentDatabase();
            var sitecoreItemToCheck = database.GetItem(itemToCheck.SitecorePath);
            return IsPublished(sitecoreItemToCheck);
        }

        public bool IsPublished(Item itemToCheck)
        {
            var targets = GetTargets();
            bool itemExists = true;
            foreach (var target in targets)
            {
                var targetDatabase = Database.GetDatabase(target.DatabaseName);
                using (new DatabaseSwitcher(targetDatabase))
                {
                    var item = targetDatabase.GetItem(itemToCheck.ID);
                    if (item == null)
                    {
                        itemExists = false;
                        break;
                    }
                }
            }
            return itemExists;
        }

        public void PublishItem(ISitecoreItem itemToPublish, bool recursive = false)
        {
            var database = _sitecoreContextProvider.GetCurrentContentDatabase();
            var sitecoreItemToPublish = database.GetItem(itemToPublish.SitecorePath);
            PublishItem(sitecoreItemToPublish, recursive);
        }

        public void PublishItem(Item itemToPublish, bool recursive = false)
        {
            using (new SecurityDisabler())
            {
                var targets = GetTargets();
                var languages = LanguageManager.GetLanguages(_sitecoreContextProvider.GetCurrentContentDatabase());
                PublishManager.PublishItem(itemToPublish, targets.Select(target => Database.GetDatabase(target.DatabaseName)).ToArray(), languages.ToArray(), recursive, false);
            }
        }

        public void UnPublishItem(Item itemToUnPublish)
        {
            if (itemToUnPublish == null) return;
            UnPublishItem(itemToUnPublish.ID);
        }

        public void UnPublishItem(ID itemToUnPublish)
        {
            if (itemToUnPublish.IsNull) return;

            using (new SecurityDisabler())
            {
                var targets = GetTargets();
                var currentDatabase = _sitecoreContextProvider.GetCurrentContentDatabase();
                foreach (var target in targets)
                {
                    var targetDatabase = Database.GetDatabase(target.DatabaseName);
                    using (new DatabaseSwitcher(targetDatabase))
                    {
                        var item = targetDatabase.GetItem(itemToUnPublish);
                        if (item != null)
                        {
                            item.Delete();
                            var publisherForContentSearch = new Publisher(new PublishOptions(currentDatabase, targetDatabase, PublishMode.SingleItem, Language.Invariant, DateTime.Now));
                            Event.RaiseEvent("publish:end", publisherForContentSearch);
                            Event.RaiseEvent("publish:end:remote", publisherForContentSearch);
                        }
                    }
                }
            }
        }

        public IEnumerable<Target> GetTargets()
        {
            using (new DatabaseSwitcher(_sitecoreContextProvider.GetCurrentContentDatabase()))
            {
                var targetsRoot = _folderRepository.Get("/sitecore/system/publishing targets");
                if (targetsRoot == null)
                {
                    _logger.Error("Could not locate targets root at: /sitecore/system/publishing targets");
                    return Enumerable.Empty<Target>();
                }
                return targetsRoot.Children.OfType<Target>();
            }
        }
    }
}
