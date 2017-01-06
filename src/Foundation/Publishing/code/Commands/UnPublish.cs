using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BoC.InversionOfControl;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Valtech.Foundation.Publishing.Services;
using Valtech.Foundation.Publishing.Models;

namespace Valtech.Foundation.Publishing.Commands
{
    public class UnPublish : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items[0] == null)
                return;
            Context.ClientPage.Start((object)this, "Run", new NameValueCollection()
            {
                { "ItemId", context.Items[0].ID.Guid.ToString() },
                { "ItemLanguage", context.Items[0].Language.ToString() },
                { "ItemVersion", context.Items[0].Version.ToString() },
                { "ItemName", context.Items[0].DisplayName },
            });
        }

        public override CommandState QueryState(CommandContext context)
        {
            IPublishService publishService = IoC.Resolver.Resolve<IPublishService>();
            return context.Items == null || !Enumerable.Any<Item>((IEnumerable<Item>)context.Items) || !Enumerable.Any<Target>(publishService.GetTargets()) ? CommandState.Disabled : CommandState.Enabled;
        }

        protected void Run(ClientPipelineArgs args)
        {
            Guid result;
            if (!Guid.TryParse(args.Parameters["ItemId"], out result))
                return;
            if (!args.IsPostBack)
            {
                SheerResponse.Confirm(
                    $"Are you sure that you want to unpublish the item <b>{args.Parameters["ItemName"]}</b> and all children in all languagues?");
                args.WaitForPostBack();
            }
            else
            {
                if (!(args.Result == "yes"))
                    return;
                var item = Database.GetDatabase("master").GetItem(new ID(result));
                if (item == null) return;
                IoC.Resolver.Resolve<IPublishService>().UnPublishItem(item);

                // http://stackoverflow.com/a/15406501/2319865
                // Also use item.ID because that always adds {} around the guid!
                Context.ClientPage.SendMessage(this,
                    string.Format("item:refreshchildren(id={0})", item.Parent.ID));
                Context.ClientPage.ClientResponse.Timer(
                    string.Format("item:updated(id={0})", item.ID), 20);
                Context.ClientPage.ClientResponse.Timer(
                    string.Format("item:load(id={0},language={1},version={2})", item.ID, args.Parameters["ItemLanguage"], args.Parameters["ItemVersion"]), 50);
            }
        }
    }
}
