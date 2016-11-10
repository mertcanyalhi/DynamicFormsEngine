using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DynamicFormsEngine.Models
{
    [Serializable]
    public abstract class FormProperties<T>
        where T : FormProperties<T>
    {
        public static T Deserialize(string serializedString)
        {
            if (string.IsNullOrWhiteSpace(serializedString))
                return default(T);

            var xmlserializer = new XmlSerializer(typeof(T));
            var stringReader = new StringReader(serializedString);
            using (var reader = XmlReader.Create(stringReader))
            {
                return (T)xmlserializer.Deserialize(reader);
            }
        }

        public string Serialize()
        {
            string result;

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.OmitXmlDeclaration = true;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (MemoryStream output = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(output, xmlWriterSettings))
                {
                    var xmlserializer = new XmlSerializer(typeof(T));
                    xmlserializer.Serialize(writer, this, ns);
                }
                result = Encoding.UTF8.GetString(output.ToArray());
            }

            return result;
        }
    }

    public class CreateFormProperties : FormProperties<CreateFormProperties>
    {
        public SerializableDictionary<string, string> CustomValues { get; set; }

        public bool RedirectOnSubmit { get; set; }

        public string RedirectTo { get; set; }
    }

    public class CustomFormProperties : FormProperties<CustomFormProperties>
    {
        public bool RedirectOnSubmit { get; set; }

        public string RedirectTo { get; set; }
    }

    public class EditFormProperties : FormProperties<EditFormProperties>
    {
        /// <summary>
        /// Restrict record to user id in order to prohibit access for other users
        /// </summary>
        public bool RestrictToUser { get; set; }

        /// <summary>
        /// User Id field to be checked for restriction. Either an integer or a key field for a many-to-many table.
        /// </summary>
        public string RestrictUserField { get; set; }

        /// <summary>
        /// Restrict record to role ids in order to prohibit access for other users
        /// </summary>
        public bool RestrictToRole { get; set; }

        /// <summary>
        /// Role field to be checked for restriction. Either an integer or a key field for a many-to-many table.
        /// </summary>
        public string RestrictRoleField { get; set; }

        public SerializableDictionary<string, string> CustomValues { get; set; }

        public bool RedirectOnSubmit { get; set; }

        public string RedirectTo { get; set; }
    }

    public class DetailFormProperties : FormProperties<DetailFormProperties>
    {
        /// <summary>
        /// Restrict record to user id in order to prohibit access for other users
        /// </summary>
        public bool RestrictToUser { get; set; }

        /// <summary>
        /// User Id field to be checked for restriction. Either an integer or a key field for a many-to-many table.
        /// </summary>
        public string RestrictUserField { get; set; }

        /// <summary>
        /// Restrict record to role ids in order to prohibit access for other users
        /// </summary>
        public bool RestrictToRole { get; set; }

        /// <summary>
        /// Role field to be checked for restriction. Either an integer or a key field for a many-to-many table.
        /// </summary>
        public string RestrictRoleField { get; set; }
    }

    public class ListFormProperties : FormProperties<ListFormProperties>
    {
        /// <summary>
        /// Restrict record to user id in order to prohibit access for other users
        /// </summary>
        public bool RestrictToUser { get; set; }

        /// <summary>
        /// User Id field to be checked for restriction. Either an integer or a key field for a many-to-many table.
        /// </summary>
        public string RestrictUserField { get; set; }

        /// <summary>
        /// Restrict record to role ids in order to prohibit access for other users
        /// </summary>
        public bool RestrictToRole { get; set; }

        /// <summary>
        /// Role field to be checked for restriction. Key field for a many-to-many table.
        /// </summary>
        public string RestrictRoleField { get; set; }

        public int DefaultPageSize { get; set; }

        public bool AllowRemove { get; set; }
        
        public bool AutoWidth { get; set; }

        public ListFormProperties()
        {
            RestrictToRole = false;
            RestrictToUser = false;
            DefaultPageSize = 10;
            AllowRemove = false;
            AutoWidth = true;
        }
    }
}
