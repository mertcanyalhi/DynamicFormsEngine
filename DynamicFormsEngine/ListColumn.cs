using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DynamicFormsEngine
{
    public class ListColumn
    {
        public string Title { get; set; }

        public string GetTitle()
        {
            return Title != null ? Title : Name;
        }

        public string Width { get; set; }

        public string Name { get; set; }

        public string MappedName { get; set; }

        public string DataFormat { get; set; }

        public string DisplayFormat { get; set; }

        public SerializableDictionary<string, string> PredefinedValues { get; set; }

        public List<string> GetFormatFields()
        {
            List<string> nameFieldNames = new List<string>();

            if (DataFormat != null)
            {
                MatchCollection matches = new Regex("{.*?}").Matches(DataFormat);

                foreach (Match match in matches)
                {
                    nameFieldNames.Add(match.Value.Replace("{", "").Replace("}", ""));
                }
            }

            return nameFieldNames;
        }

        public SortSettings SortSettings { get; set; }

        public FilterSettings FilterSettings { get; set; }

        public int Order { get; set; }

        public bool Visible { get; set; }
    }

    public enum FilterType
    {
        Text = 0,
        Select = 1,
        Date = 2,
        DateRange = 3
    }

    public class FilterSettings
    {
        public bool FilterEnabled { get; set; }
        public FilterType Type { get; set; }
        public SerializableDictionary<string, string> Options { get; set; }
        public string DisplayFormat { get; set; }

        public List<SelectListItem> GetOptions()
        {
            List<SelectListItem> result = new List<SelectListItem>();

            if (Options != null)
            {
                foreach (var o in Options.OrderBy(x => x.Value))
                {
                    result.Add(new SelectListItem()
                    {
                        Text = o.Value.ToString(),
                        Value = o.Key
                    });
                }
            }

            return result;
        }
    }

    public class SortSettings
    {
        public bool SortingEnabled { get; set; }
        public Sort Sort { get; set; }
    }

    public enum Sort
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }
}
