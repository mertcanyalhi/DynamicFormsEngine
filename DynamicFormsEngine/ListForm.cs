using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DynamicFormsEngine
{
    [Serializable]
    public class ListForm
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public List<ListColumn> Columns { get; set; }
        public List<Button> RowButtons { get; set; }
        public List<Button> ListButtons { get; set; }

        public List<Button> GetRowButtons(int UserId, IList<int> RoleIds)
        {
            List<Button> result = new List<Button>();

            if (RowButtons != null)
                foreach (var b in RowButtons)
                {
                    result.Add(b);
                }

            return result;
        }

        public List<Button> GetListButtons(int UserId, IList<int> RoleIds)
        {
            List<Button> result = new List<Button>();

            if (ListButtons != null)
                foreach (var b in ListButtons)
                {
                    result.Add(b);
                }

            return result;
        }

        public string SerializeForm()
        {
            var xmlserializer = new XmlSerializer(typeof(ListForm));
            string result;

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.OmitXmlDeclaration = true;

            using (MemoryStream output = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(output, xmlWriterSettings))
                {
                    xmlserializer.Serialize(writer, this);
                }
                result = Encoding.UTF8.GetString(output.ToArray());
            }

            return result;

            return string.Empty;
        }

        public static ListForm DeserializeForm(string serializedString)
        {
            var xmlserializer = new XmlSerializer(typeof(ListForm));
            var stringReader = new StringReader(serializedString);
            using (var reader = XmlReader.Create(stringReader))
            {
                ListForm f = (ListForm)xmlserializer.Deserialize(reader);

                return f;
            }

            return null;
        }
    }
}
