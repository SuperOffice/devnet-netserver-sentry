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
    [SentryPlugin(Constants.SentryEntities.Selection, Constants.SentryName.DevNet)]
    public class SelectionSentry : SuperOffice.CRM.Security.ISentryPlugin
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
            if (!Constants.EnableSelection)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Person, SentryState.BeforeModified, rights);
        }

        public void ModifyTableRights(SuperOffice.CRM.Security.TableRights rights)
        {
            if (!Constants.EnableSelection)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Person, SentryState.BeforeModified, rights);
        }


        #region helpers
        /// <summary>
        /// Casting <see cref="P:SuperOffice.CRM.Security.Sentry.SentryQueryInfo"/>to<see cref = "SelectionSentryQueryInfo" />.
        /// </summary>
        private SelectionSentryQueryInfo QueryInfo
        {
            get { return (SelectionSentryQueryInfo)_sentry.SentryQueryInfo; }
        }

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
    ///Demonstration of sentry plugin query table updater for table selection.
    ///</summary>
    ///<remarks>The field name is forced to be inqueries so rights can be calculated.</remarks>
    [SentryPluginQueryTableUpdater(Constants.SentryEntities.Selection)]
    public class SentryPluginQueryTableUpdaterSelection : ISentryPluginQueryTableUpdater
    {
        #region ISentryPluginQueryTableUpdater Members

        public void ModifySelect(Select sql, TableInfo tableInfo)
        {
            if (Constants.EnableSelection)
                sql.ReturnFields.Add(((SelectionTableInfo)tableInfo).Name);
        }
        #endregion
    }
}
