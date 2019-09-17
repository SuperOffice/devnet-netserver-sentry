using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperOffice.DevNet.SentryPlugin
{
    public static class Constants
    {
        public static bool EnableAppointment { get; set; } = false;
        public static bool EnableContact     { get; set; } = false;
        public static bool EnablePerson      { get; set; } = false;
        public static bool EnableProject     { get; set; } = true;
        public static bool EnableRelation    { get; set; } = false;
        public static bool EnableSale        { get; set; } = false;
        public static bool EnableSelection   { get; set; } = false;

        public static class SentryEntities
        {
            public const string Appointment = "appointment";
            public const string Contact     = "contact";
            public const string Person      = "person";
            public const string Project     = "project";
            public const string Relation    = "relation";
            public const string Sale        = "sale";
            public const string Selection   = "selection";
        }

        public static class SentryName
        {
            public const string DevNet = "DevNetSentry";
        }
    }
}
