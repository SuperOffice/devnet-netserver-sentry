using SuperOffice.CRM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperOffice.DevNet.SentryPlugin
{
    internal enum RightsType
    {
        None,
        FieldRights,
        TableRights
    }

    public enum SentryState
    {
        BeforeModified,
        AfterModified,
        Message
    }

    internal class SentryTaskInfo
    {
        public int AssociateId { get; set; }
        public string AssociateName { get; set; }
        public string SentryAddOnName { get; set; }
        public string SentryName { get; set; }
        public SentryState State { get; set; }
        public RightsType Type { get; set; }
        public TableRights TableRights { get; set; }
        public FieldRights FieldRights { get; set; }
        public String Message { get; set; }
    }
}
