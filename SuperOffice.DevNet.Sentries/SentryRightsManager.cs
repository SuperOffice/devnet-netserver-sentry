using SuperOffice.CRM.Security;
using SuperOffice.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperOffice.DevNet.SentryPlugin
{
    [SoInject(InstanceContainers.Database)]
    public class SentryRightsManager
    {
        [SoInjectConstructor]
        private SentryRightsManager()
        {
        }

        /// <summary>
        /// Return the current sentry rights manager instance.
        /// </summary>
        /// <returns>The one and only sentry rights manager.</returns>
        public static SentryRightsManager GetCurrent()
        {
            return ClassFactory.Create<SentryRightsManager>();
        }

        private SentryRightsDispatcher GetDispatcher()
        {
            return ClassFactory.Create<SentryRightsDispatcher>();
        }

        public void SignalEvent(string sentryAddOnName, string sentryName, SentryState state, TableRights tableRights)
        {
            var taskInfo = new SentryTaskInfo()
            {
                AssociateId = SoContext.CurrentPrincipal.AssociateId,
                AssociateName = SoContext.CurrentPrincipal.Associate,
                TableRights = tableRights,
                Type = RightsType.TableRights,
                SentryName = sentryName,
                SentryAddOnName = sentryAddOnName,
                State = state
            };

            // This produces an item and puts it on the queue. 
            // A background thread will dequeue the item and process it in NotifyWebhooks.
            GetDispatcher().Enqueue(taskInfo);

        }

        public void SignalEvent(string sentryAddOnName, string sentryName, SentryState state, FieldRights fieldRights)
        {
            var taskInfo = new SentryTaskInfo()
            {
                AssociateId = SoContext.CurrentPrincipal.AssociateId,
                AssociateName = SoContext.CurrentPrincipal.Associate,
                FieldRights = fieldRights,
                Type = RightsType.FieldRights,
                SentryName = sentryName,
                SentryAddOnName = sentryAddOnName,
                State = state
            };

            // This produces an item and puts it on the queue. 
            // A background thread will dequeue the item and process it in NotifyWebhooks.
            GetDispatcher().Enqueue(taskInfo);
        }

        public void SignalEvent(string sentryAddOnName, string sentryName, string message)
        {
            var taskInfo = new SentryTaskInfo()
            {
                AssociateId = SoContext.CurrentPrincipal.AssociateId,
                AssociateName = SoContext.CurrentPrincipal.Associate,
                SentryName = sentryName,
                SentryAddOnName = sentryAddOnName,
                State = SentryState.Message,
                Message = message
            };

            // This produces an item and puts it on the queue. 
            // A background thread will dequeue the item and process it in NotifyWebhooks.
            GetDispatcher().Enqueue(taskInfo);
        }

        public static void WaitForSentryTasksToComplete()
        {
            var manager = GetCurrent();
            var dispatcher = manager.GetDispatcher();
            dispatcher.WaitForSentryTasksToComplete();
            System.Threading.Thread.Sleep(500);
        }
    }
}
