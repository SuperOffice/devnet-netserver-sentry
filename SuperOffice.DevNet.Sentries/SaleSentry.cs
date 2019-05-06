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
    [SentryPlugin(Constants.SentryEntities.Sale, Constants.SentryName.DevNet)]
    public class SaleSentry : SuperOffice.CRM.Security.ISentryPlugin
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
            if (!Constants.EnableSale)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Sale, SentryState.BeforeModified, rights);
        }

        public void ModifyTableRights(SuperOffice.CRM.Security.TableRights rights)
        {
            if (!Constants.EnableSale)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Sale, SentryState.BeforeModified, rights);
        }


        #region helpers
        /// <summary>
        /// Casting <see cref="P:SuperOffice.CRM.Security.Sentry.SentryQueryInfo"/>to<see cref = "SaleSentryQueryInfo" />.
        /// </summary>
        private SaleSentryQueryInfo QueryInfo
        {
            get
            {
                return (SaleSentryQueryInfo)_sentry.SentryQueryInfo;
            }
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
    ///Demonstration of sentry plugin query table updater for table sale.
    ///</summary>
    ///<remarks>The field heading is forced to be inqueries so rights can be calculated.</remarks>
    [SentryPluginQueryTableUpdater(Constants.SentryEntities.Sale)]
    public class SentryPluginQueryTableUpdaterSale : ISentryPluginQueryTableUpdater
    {
        #region ISentryPluginQueryTableUpdater Members

        public void ModifySelect(Select sql, TableInfo tableInfo)
        {
            if (Constants.EnableSale)
                sql.ReturnFields.Add(((SaleTableInfo)tableInfo).Heading);
        }
        #endregion
    }
}
