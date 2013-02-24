using MARC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WebApi.Models;

namespace WebApi
{
    public class SearchOperations
    {
        private static IList<Record> Records = new List<Record>();

        private static IDictionary<string, Func<Record, string, bool>> ParametersQuery =
            new Dictionary<string, Func<Record, string, bool>>() { 
                { "Title", TitleQuery }, 
                { "Author", AuthorQuery }, 
                { "Editor", EditorQuery }, 
                { "Keyword", KeywordQuery },
                { "DocumentType", (record, value) => false } // TODO
            };

        static SearchOperations()
        {
            string filename = ConfigurationManager.AppSettings["MarcFilePath"];

            // load all into memory for faster access
            Trace.WriteLine("Loading into memory...");
            Load(filename);
            Trace.WriteLine(string.Format("{0} records loaded.", Records.Count));
        }

        public IEnumerable<Book> GetBooksBy(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            IEnumerable<Record> results = Enumerable.Empty<Record>();

            foreach (var item in parameters)
            {
                if (!ParametersQuery.Keys.Contains(item.Key)) 
                {
                    throw new InvalidOperationException("Parameter not allowed.");
                }

                results = results.Union(Records.Where((record) => ParametersQuery[item.Key](record, item.Value)));
            }

            return results.Select(r => Book.Parse(r));
        }

        private static bool TitleQuery(Record record, string value)
        {
            return GenericStringContainsQuery(record, "200", MarcSubfields.Title, value);
        }

        private static bool AuthorQuery(Record record, string value)
        {
            return GenericStringContainsQuery(record, "200", MarcSubfields.Author, value);
        }

        private static bool EditorQuery(Record record, string value)
        {
            return GenericStringContainsQuery(record, "210", MarcSubfields.Editor, value);
        }

        private static bool KeywordQuery(Record record, string value)
        {
            return TitleQuery(record, value)
                || AuthorQuery(record, value)
                || EditorQuery(record, value)
                || KeywordQueryImpl(record, value);
        }

        private static bool KeywordQueryImpl(Record record, string value)
        {
            var field = record.GetDataFieldByTag("606");
            var valueParts = value.Split(new[] {' '});

            if (field == null)
            {
                return false;
            }

            foreach (string subfieldValue in field.GetSubfields().Select(sf => sf.Data)) 
            {
                if (valueParts.Any(vp => subfieldValue.Contains(vp, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool GenericStringContainsQuery(Record record, string tag, char subfield, string value)
        {
            var field = record.GetDataFieldByTag(tag);

            if (field == null) 
            {
                return false;
            }

            var subFieldValue = field.GetSubfieldOrEmpty(subfield);

            if (subFieldValue.Contains(value, StringComparison.InvariantCultureIgnoreCase)) 
            {
                return true;
            }

            return false;
        }

        private static void Load(string filename)
        {
            using (FileMARCReader reader = new FileMARCReader(filename))
            {
                foreach (Record marc in reader)
                {
                    Records.Add(marc);
                }
            }
        }
    }
}