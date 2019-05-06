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
    internal class SentryRightsDispatcher
    {
        const string generalMsg = "SentryPlugin: {0}";
        const string tableString = @"   {0} Table Rights: {1}";
        const string fieldString = @"       {0} Field Rights: {1}";
        int _timeout = 2;

        Util.ThreadedQueueProcessor<SentryTaskInfo> _queue = null;

        [SoInjectConstructor]
        private SentryRightsDispatcher()
        {
            _queue = new Util.ThreadedQueueProcessor<SentryTaskInfo>(ProcessTasks, "RightsDumper", _timeout);
        }

        public void Enqueue(SentryTaskInfo sentryTaskInfo)
        {
            _queue.AddWork(sentryTaskInfo);
        }

        internal void WaitForSentryTasksToComplete()
        {
            _queue.AwaitDisposedWorked();
        }

        private void ProcessTasks(SentryTaskInfo sentryTaskInfo)
        {
            Write("Plugin name: {0}, for {1}, invoked by Username: {2} - ID: {3}, for {4} : {5}", 
                sentryTaskInfo.SentryAddOnName, 
                sentryTaskInfo.SentryName, 
                sentryTaskInfo.AssociateName, 
                sentryTaskInfo.AssociateId,
                sentryTaskInfo.Type.ToString(),
                sentryTaskInfo.State.ToString());

            if (sentryTaskInfo.State == SentryState.Message)
            {
                Write(sentryTaskInfo.Message);
            }

            if (sentryTaskInfo.Type == RightsType.TableRights)
                Write(sentryTaskInfo?.TableRights);
            else
                Write(sentryTaskInfo?.FieldRights);
        }

        private void Write(TableRights rights)
        {
            if(rights != null)
            {
                foreach (var info in rights.Keys.ToArray())
                {
                    Write(tableString, info.TableName, rights[info].Mask);
                }
            }
        }

        private void Write(FieldRights rights)
        {
            if(rights != null)
            {
                foreach (var info in rights.Keys.ToArray())
                {
                    Write(fieldString, info.Name, rights[info].Mask);
                }
            }
        }

        private void Write(string message)
        {
            Write(message, string.Empty);
        }

        private void Write(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(string.Format(generalMsg, string.Format(format, args)));
        }
    }
}
