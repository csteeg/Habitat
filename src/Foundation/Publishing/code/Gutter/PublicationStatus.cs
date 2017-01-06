using System;
using BoC.InversionOfControl;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor.Gutters;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Valtech.Foundation.Publishing.Services;

namespace Valtech.Foundation.Publishing.Gutter
{
    public class PublicationStatus : GutterRenderer
    {
        protected override GutterIconDescriptor GetIconDescriptor(Item item)
        {
            var sitecoreContextProvider = IoC.Resolver.Resolve<ISitecoreContextProvider>();

            using (new DatabaseSwitcher(sitecoreContextProvider.GetCurrentContentDatabase()))
            {
                var publishService = IoC.Resolver.Resolve<IPublishService>();
                var targets = publishService.GetTargets();
                var itemExists = true;
                var itemDiffers = false;

                foreach (var target in targets)
                {
                    var targetDatabase = Database.GetDatabase(target.DatabaseName);
                    var foundItem = targetDatabase.GetItem(item.ID, item.Language);

                    if (foundItem == null)
                    {
                        itemExists = false;
                    }
                    else if (!RevisionMatch(item, foundItem))
                    {
                        itemDiffers = true;
                    }
                }

                var tooltip = Translate.Text("This item has been published to all targets");
                var icon = "People/16x16/flag_green.png";

                if (itemDiffers)
                {
                    tooltip = Translate.Text("This items changes has not yet been published");
                    icon = "People/16x16/flag_yellow.png";
                }
                else if (!itemExists)
                {
                    tooltip = Translate.Text("This item has not yet been published");
                    icon = "People/16x16/flag_red.png";
                }

                return new GutterIconDescriptor
                {
                    Icon = icon,
                    Tooltip = tooltip
                };
            }
        }

        private bool RevisionMatch(Item sourceItem, Item compareItem)
        {
            return string.Compare(sourceItem[FieldIDs.Revision], compareItem[FieldIDs.Revision],
                                  StringComparison.CurrentCultureIgnoreCase) == 0;
        }
    }
}
