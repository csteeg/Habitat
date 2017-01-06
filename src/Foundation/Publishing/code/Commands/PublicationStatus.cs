using System.Collections.Generic;
using BoC.InversionOfControl;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor.Gutters;
using Sitecore.Foundation.SitecoreExtensions.Providers;
using Valtech.Foundation.Publishing.Services;
using Valtech.Foundation.Publishing.Models;

namespace Valtech.Foundation.Publishing.Commands
{
    public class PublicationStatus : GutterRenderer
    {
        protected override GutterIconDescriptor GetIconDescriptor(Item item)
        {
            using (new DatabaseSwitcher(IoC.Resolver.Resolve<ISitecoreContextProvider>().GetCurrentContentDatabase()))
            {
                IEnumerable<Target> targets = IoC.Resolver.Resolve<IPublishService>().GetTargets();
                bool flag1 = true;
                bool flag2 = false;
                foreach (Target target in targets)
                {
                    Item compareItem = Database.GetDatabase(target.DatabaseName).GetItem(item.ID, item.Language);
                    if (compareItem == null)
                        flag1 = false;
                    else if (!this.RevisionMatch(item, compareItem))
                        flag2 = true;
                }
                string str1 = Translate.Text("This item has been published to all targets");
                string str2 = "People/16x16/flag_green.png";
                if (flag2)
                {
                    str1 = Translate.Text("This items changes has not yet been published");
                    str2 = "People/16x16/flag_yellow.png";
                }
                else if (!flag1)
                {
                    str1 = Translate.Text("This item has not yet been published");
                    str2 = "People/16x16/flag_red.png";
                }
                return new GutterIconDescriptor()
                {
                    Icon = str2,
                    Tooltip = str1
                };
            }
        }

        private bool RevisionMatch(Item sourceItem, Item compareItem)
        {
            var masterDate = ((DateField)sourceItem.Fields["Date"]).DateTime;
            var webDate = ((DateField)compareItem.Fields["Date"]).DateTime;

            return ((webDate - masterDate).TotalSeconds < 10);
        }
    }
}
