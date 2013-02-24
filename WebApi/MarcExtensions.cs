using MARC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace WebApi
{
    public static class MarcExtensions
    {
        public static string GetSubfieldOrEmpty(this DataField field, char c)
        {
            if (field == null)
            {
                return string.Empty;
            }

            var subfield = field.Subfields.FirstOrDefault(f => f.Code == c);
            return subfield != null ? subfield.Data : string.Empty;
        }

        public static DataField GetDataFieldByTag(this Record record, string tag)
        {
            var fields = record.Fields
                .Where(f => f.Tag == tag)
                .Cast<DataField>();

            if (fields.Count() > 1)
            {
                Trace.TraceInformation("More than one element detected with tag=" + tag);
                return fields.First();
            }

            return fields.SingleOrDefault();
        }
    }
}