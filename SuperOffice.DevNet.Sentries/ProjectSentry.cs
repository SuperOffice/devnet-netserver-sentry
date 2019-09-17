using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperOffice.CRM.Data;
using SuperOffice.CRM.Security;
using SuperOffice.Data.SQL;
using SuperOffice.Factory;

namespace SuperOffice.DevNet.SentryPlugin
{
    [SentryPlugin(Constants.SentryEntities.Project, Constants.SentryName.DevNet)]
    public class ProjectSentry : SuperOffice.CRM.Security.ISentryPlugin
    {
        /// <summary>
        /// Storing reference to the sentry the plugin works on behalf of.
        /// </summary>
        SuperOffice.CRM.Security.Sentry _sentry = null;
        SentryRightsManager _rightsManager = null;
        
        public void Init(SuperOffice.CRM.Security.Sentry sentry)
        {
            _sentry = sentry;
            _rightsManager = ClassFactory.Create<SentryRightsManager>();
        }

        public void ModifyFieldRights(SuperOffice.CRM.Security.FieldRights rights)
        {
            if (!Constants.EnableProject)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Project, SentryState.BeforeModified, rights);
        }

        public void ModifyTableRights(SuperOffice.CRM.Security.TableRights rights)
        {
            if (!Constants.EnableProject)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Project, SentryState.BeforeModified, rights);
        }


        #region helpers
        /// <summary>
        /// Casting <see cref="P:SuperOffice.CRM.Security.Sentry.SentryQueryInfo"/>to<see cref = "ProjectSentryQueryInfo" />.
        /// </summary>
        private ProjectSentryQueryInfo QueryInfo { get { return (ProjectSentryQueryInfo)_sentry.SentryQueryInfo; } }

        /// <summary>
        /// Obtain value of a field without trigging sentry calculations.
        /// </summary>
        /// <param name="fieldInfo">Field to get value for</param>
        /// <returns>Value of the field.</returns>
        private object GetFieldValue(FieldInfo fieldInfo)
        {
            using (_sentry.Lookups.BeginIgnoreSentryCheck())
            {
                return _sentry.Lookups.GetFieldValue(fieldInfo);
            }
        }
        #endregion
    }

    ///<summary>
    ///Demonstration of sentry plugin query table updater for table project.
    ///</summary>
    ///<remarks>The field name is forced to be inqueries so rights can be calculated.</remarks>
    [SentryPluginQueryTableUpdater(Constants.SentryEntities.Project)]
    public class SentryPluginQueryTableUpdaterProject : ISentryPluginQueryTableUpdater
    {
        #region ISentryPluginQueryTableUpdater Members
        
        public void ModifySelect(Select sql, TableInfo tableInfo)
        {
            //var currentUser = SuperOffice.SoContext.CurrentPrincipal;
            //var projectMemberTableInfo = sql.GetTableInfos().Where(ti => ti.TableName.Equals("ProjectMember", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() as ProjectMemberTableInfo;


            if (Constants.EnableProject)
            {
                sql.ReturnFields.Add(((ProjectTableInfo)tableInfo).Name);
                //sql.JoinRestriction.InnerJoin(projectMemberTableInfo.PersonId.Equal(currentUser.PersonId));
            }           

        }
        #endregion
    }
}
