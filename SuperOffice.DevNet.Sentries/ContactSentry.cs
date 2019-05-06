using System;
using SuperOffice.CRM.Data;
using SuperOffice.CRM.Security;
using SuperOffice.Data;
using SuperOffice.Data.SQL;
using SuperOffice.Factory;

namespace SuperOffice.DevNet.SentryPlugin
{
    [SentryPlugin(Constants.SentryEntities.Contact, Constants.SentryName.DevNet)]
    public class ContactSentry : SuperOffice.CRM.Security.ISentryPlugin
    {
        /// <summary>
        /// Storing reference to the sentry the plugin works on behalf of.
        /// </summary>
        Sentry _sentry = null;
        SentryRightsManager _rightsManager = null;

        /// <summary>               
        /// Variable set by <see cref="ModifyTableRights"/> to signal <see cref="ModifyFieldRights"/> that some rights has to be set.
        /// </summary>
        private bool _sunIsNotShining = false;
        
        public void Init(SuperOffice.CRM.Security.Sentry sentry)
        {
            _sentry = sentry;
            _rightsManager = ClassFactory.Create<SentryRightsManager>();
        }

        /// <summary>
        /// Modify rights associated with specified tables.
        /// </summary>
        /// <param name="rights">Rights object to modify.</param>
        /// <remarks>Use this method to change rights across related blocks of information, i.e. table-wide rights.</remarks>
        public void ModifyTableRights(SuperOffice.CRM.Security.TableRights rights)
        {
            if (!Constants.EnableContact)
            {
                _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, "Contact sentry not enabled!");
                return;
            }

            try
            {

                _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, SentryState.BeforeModified, rights);

                string department = GetFieldValue(QueryInfo.MainTable.Department) as string;
                if (!string.IsNullOrEmpty(department) && department.Trim().EndsWith("_") && _sentry.GetRecordOwnershipIndex() != EOwnershipIndex.Owner)
                {
                    _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, "Updating Contact!");

                    _sunIsNotShining = true;
                    TableRights newRights = new TableRights();
                    foreach (TableInfo ti in rights.Keys)
                        newRights[ti] = rights[ti] & RightsFactory.Get(ETableRight.R, "The sun is not shining on trailing '_'");

                    foreach (TableInfo ti in newRights.Keys)
                        rights[ti] = newRights[ti];
                }
                else
                    _sunIsNotShining = false;

                _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, SentryState.AfterModified, rights);

            }
            catch (Exception e)
            {
                _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, e.GetBaseException().Message);
                throw;
            }
        }

        /// <summary>
        /// Modify rights associated with specified fields.
        /// </summary>
        /// <param name="rights">Rights object to modify.</param>
        /// <remarks>Use this method to change rights across fine-grained fields of information.</remarks>
        public void ModifyFieldRights(SuperOffice.CRM.Security.FieldRights rights)
        {
            if (!Constants.EnableContact)
                return;

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, SentryState.BeforeModified, rights);

            if (_sunIsNotShining)
            {
                rights[QueryInfo.MainTable.TextId] = RightsFactory.Get(EFieldRight.None, "But one can still modify department");
            }

            _rightsManager.SignalEvent(Constants.SentryName.DevNet, Constants.SentryEntities.Contact, SentryState.AfterModified, rights);
        }


        #region helpers
        /// <summary>
        /// Casting <see cref="P:SuperOffice.CRM.Security.Sentry.SentryQueryInfo"/>to<see cref = "ContactSentryQueryInfo" />.
        /// </summary>
        private ContactSentryQueryInfo QueryInfo
        {
            get { return (ContactSentryQueryInfo)_sentry.SentryQueryInfo; }
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
    ///Demonstration of sentry plugin query table updater for table contact.
    ///</summary>
    ///<remarks>The field Department is forced to be inqueries so rights can be calculated.</remarks>
    [SentryPluginQueryTableUpdater(Constants.SentryEntities.Contact)]
    public class SentryPluginQueryTableUpdaterContact : ISentryPluginQueryTableUpdater
    {
        #region ISentryPluginQueryTableUpdater Members

        public void ModifySelect(Select sql, TableInfo tableInfo)
        {
            if (Constants.EnableContact)
                sql.ReturnFields.Add(((ContactTableInfo)tableInfo).Department);
        }
        #endregion
    }
}
