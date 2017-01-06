using System;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Tasks;
using Command = Sitecore.Shell.Framework.Commands.Command;

namespace Valtech.Foundation.Publishing.Commands
{
    public class RunScheduledTaskCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context != null && context.Items.Length > 0)
                RunScheduledTask(context.Items.First());
        }

        private static Item GetItem(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            Assert.ArgumentNotNull(context.Items, "context.Items");
            return context.Items.FirstOrDefault();
        }

        /// <summary>
        /// Hits the ui about the state of this command
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            return !IsScheduleItem(GetItem(context)) ? CommandState.Hidden : CommandState.Enabled;
        }

        /// <summary>
        /// Execute same validation logic as RunScheduledTask only returns wheter the task can be executed
        /// <Remark>the given node must be a schedule node</Remark>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsScheduleItem(Item item)
        {
            var schedule = new ScheduleItem(item);
            Object task = null;

            if (schedule.CommandItem != null)
            {
                var commandItem = item.Database.GetItem(schedule.CommandItem.ID);
                if (commandItem == null)
                {
                    return false;
                }

                var type = Type.GetType(commandItem["Type"]);
                if (type != null)
                    task = Activator.CreateInstance(type);
            }

            return task != null;
        }

        public void RunScheduledTask(Item innerScheduledItem)
        {
            var schedule = new ScheduleItem(innerScheduledItem);

            Object task = null;
            Item commandItem = null;

            if (schedule.CommandItem != null)
            {
                commandItem = innerScheduledItem.Database.GetItem(schedule.CommandItem.ID);
                if (commandItem == null)
                {
                    Context.ClientPage.ClientResponse.ShowError("Command is null", "");
                    return;
                }

                var type = Type.GetType(commandItem["Type"]);
                if (type != null)
                    task = Activator.CreateInstance(type);
            }

            if (task == null)
            {
                Context.ClientPage.ClientResponse.ShowError("Task is null", "");
                return;
            }

            var job = JobManager.GetJob(innerScheduledItem.ID.ToString());
            if (job == null || job.IsDone)
            {
                var options = new JobOptions(innerScheduledItem.ID.ToString(), "schedule", "scheduler",
                                             task,
                                             commandItem["Method"],
                                             new object[] { schedule.Items, schedule.CommandItem, schedule })
                {
                    SiteName = "scheduler"
                };
                JobManager.Start(options);
            }
            else
            {
                Context.ClientPage.ClientResponse.ShowError(string.Format("Task: {0} is already running", commandItem.DisplayName), "");
            }
        }
    }
}
