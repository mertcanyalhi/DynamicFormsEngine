using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents a dynamically generated form field.
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = "DynamicFormsEngine.Fields")]
    [XmlInclude(typeof(CheckBox))]
    [XmlInclude(typeof(CheckBoxList))]
    [XmlInclude(typeof(DateTimePicker))]
    [XmlInclude(typeof(DatePicker))]
    [XmlInclude(typeof(FileUpload))]
    [XmlInclude(typeof(Hidden))]
    [XmlInclude(typeof(Literal))]
    [XmlInclude(typeof(NumberInput))]
    [XmlInclude(typeof(RadioList))]
    [XmlInclude(typeof(Select))]
    [XmlInclude(typeof(TextArea))]
    [XmlInclude(typeof(TextBox))]
    [XmlInclude(typeof(DataTable))]
    public abstract class Field
    {
        protected string _key = Guid.NewGuid().ToString().Replace("-", "");
        protected Form _form;
        private bool _display = true;
        private string _template = "";
        protected Dictionary<string, DataItem> _dataDictionary = new Dictionary<string, DataItem>();
        protected string _labelClass = "control-label";
        
        public Field()
        {
            Template = BuildDefaultTemplate();
        }

        protected abstract string BuildDefaultTemplate();

        /// <summary>
        /// Mapped name for mapping operations
        /// </summary>
        public string MappedName { get; set; }

        /// <summary>
        /// Label title
        /// </summary>
        public string Title { get; set; }

        public string GetTitle()
        {
            return Title != null ? Title : _key;
        }

        /// <summary>
        /// Label class for bootstrap (ex: col-md-2)
        /// </summary>
        public string LabelClass { get; set; }

        /// <summary>
        /// Input class for bootstrap (ex: col-md-10)
        /// </summary>
        public string InputClass { get; set; }

        /// <summary>
        /// The fields's HTML template.
        /// </summary>
        public string Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
            }
        }

        /// <summary>
        /// Whether the field should be rendered when Form.RenderHtml() is called.
        /// </summary>
        public bool Display
        {
            get
            {
                return _display;
            }
            set
            {
                _display = value;
            }
        }

        /// <summary>
        /// A friendly field group name. Use this to group fields together for your own purposes.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Group title
        /// </summary>
        public string GroupTitle { get; set; }

        internal Form Form
        {
            get
            {
                return _form;
            }
            set
            {
                _form = value;
            }
        }

        /// <summary>
        /// Used to identify field.
        /// </summary>
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (_form != null)
                    _form.Fields.ValidateKey(value);

                _key = value;
            }
        }

        /// <summary>
        /// An arbitrary dictionary of objects. Use this to attach objects to your fields.
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, DataItem> DataDictionary
        {
            get
            {
                return _dataDictionary;
            }
        }

        /// <summary>
        /// Whether the DataDictionary contains any data objects that will be rendered.
        /// </summary>
        public bool HasClientData
        {
            get
            {
                return _dataDictionary.Where(x => x.Value.ClientSide).Count() > 0;
            }
        }

        /// <summary>
        /// The relative position that the field is rendered to html.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Renders the field as html.
        /// </summary>
        /// <returns>Returns a string containing the rendered html of the Field object.</returns>
        public abstract string RenderHtml();

        /// <summary>
        /// Retrieves a strongly typed object from the DataDictionary;
        /// </summary>
        public T GetDataValue<T>(string key)
        {
            DataItem data;
            if (!_dataDictionary.TryGetValue(key, out data))
            {
                return default(T);
            }
            return (T)data.Value;
        }

        /// <summary>
        /// Adds an object to the data dictionary. By default, the object will not be rendered.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <param name="value">The object to store</param>
        public void AddDataValue(string key, object value)
        {
            AddDataValue(key, value, false);
        }

        /// <summary>
        /// Adds an object to the data dictionary.
        /// </summary>
        /// <param name="key">The dictionary key.</param>
        /// <param name="value">The object to store.</param>
        /// <param name="clientSide">Whether the data will be rendered on the client.</param>
        public void AddDataValue(string key, object value, bool clientSide)
        {
            _dataDictionary.Add(key, new DataItem(value, clientSide));
        }

        /// <summary>
        /// Gets a value that can be associated with an HTML input element's id or name attribute.
        /// </summary>
        public string GetHtmlId()
        {
            string id = _form.FieldPrefix + _key.Trim();

            if (!Regex.IsMatch(id, RegexPatterns.HtmlId))
                throw new Exception("The combination of Form.FieldPrefix + Field.Key does not produce a valid id attribute value for an HTML element. It must begin with a letter and can only contain letters, digits, hyphens, and underscores.");

            return id;
        }

        protected string GetWrapperId()
        {
            return GetHtmlId() + "_wrapper";
        }
    }
}