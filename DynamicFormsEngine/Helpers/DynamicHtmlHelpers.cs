using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicFormsEngine.Helpers
{

    public static class DynamicHtmlHelpers
    {
        public static string GetLink(int UserInterfaceId)
        {
            return GetLink(string.Format("{{UI:{0}}}", UserInterfaceId));
        }

        public static string GetLink(string Link)
        {
            if (string.IsNullOrWhiteSpace(Link))
            {
                return string.Empty;
            }

            string result = Link;

            List<string> formatFields = new List<string>();

            #region Get formattable fields except for string format fields
            MatchCollection matches = new Regex("{.*?}").Matches(result);

            foreach (Match match in matches)
            {
                string f = match.Value.Replace("{", "").Replace("}", "");

                if (!Regex.IsMatch(f, @"^\d+$"))
                    formatFields.Add(f);
            }
            #endregion

            //foreach (var field in formatFields)
            //{
            //    string val = string.Empty;
                
            //    result = result.Replace(string.Format("{{{0}}}", field), val);
            //}

            return result;
        }

        public static string GetLink(int UserInterfaceId, Dictionary<string, object> values)
        {
            return GetLink(string.Format("{{UI:{0}}}", UserInterfaceId), values);
        }

        public static string GetLink(string Link, Dictionary<string, object> values)
        {
            MatchCollection dataMatches = new Regex("%.*?%").Matches(Link);

            foreach (Match match in dataMatches)
            {
                    string dataField = match.Value;
                    string value = match.Value.Replace("%", "");
                    string[] fieldSplitted = value.Split(':');

                    string colName = fieldSplitted[0];
                    string colFieldName = fieldSplitted.Length > 1 ? fieldSplitted[1] : null;

                    if (values.ContainsKey(colName))
                    {
                        if (values[colName] is IDictionary && colFieldName != null)
                        {
                            var dictVal = ((IDictionary)values[colName])[colFieldName];
                            value = dictVal == null ? value : dictVal.ToString();
                        }
                        else if (values[colName] is IEnumerable<object> && colFieldName != null)
                        {
                            List<string> vals = new List<string>();

                            foreach (var v in (IEnumerable<object>)values[colName])
                            {
                                if (v != null && v is IDictionary && ((IDictionary)v)[colFieldName] != null)
                                {
                                    vals.Add(((IDictionary)v)[colFieldName].ToString());
                                }
                                else if (v != null)
                                {
                                    vals.Add(v.ToString());
                                }
                            }
                        }
                        else if (!(values[colName] is IDictionary || values[colName] is IEnumerable<object>))
                        {
                            value = values[colName] != null ? values[colName].ToString() : string.Empty;
                        }
                    }

                    Link = Link.Replace(dataField, value);
            }

            return GetLink(Link);
        }

    }
}
