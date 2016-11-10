using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents html to be rendered on the form.
    /// </summary>
    [Serializable]
    public class Literal : Field
    {
        /// <summary>
        /// The html to be rendered on the form.
        /// </summary>
        public string Html { get; set; }

        public string Text { get; set; }

        public object Value { get; set; }

        public string StringFormat { get; set; }

        public string DataFormat { get; set; }

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

        public override string RenderHtml()
        {
            //Set template to columned if title is not null
            if (Title != null)
            {
                var innerWrapper = new TagBuilder("div");
                innerWrapper.AddCssClass("{InputClass}");
                innerWrapper.InnerHtml = PlaceHolders.Literal + PlaceHolders.Error;

                var wrapper = new TagBuilder("div");
                wrapper.AddCssClass("form-group");
                wrapper.Attributes["id"] = PlaceHolders.FieldWrapperId;
                wrapper.InnerHtml = PlaceHolders.Label + innerWrapper;
                Template = wrapper.ToString();
            }

            var html = new StringBuilder(Template);

            var inputName = GetHtmlId();

            // prompt label
            var label = new TagBuilder("label");

            label.SetInnerText(GetTitle());
            label.Attributes.Add("for", inputName);
            label.Attributes.Add("class", _labelClass);
            label.AddCssClass(LabelClass);
            html.Replace(PlaceHolders.Label, label.ToString());

            var literal = new TagBuilder("span");
            literal.AddCssClass("form-control-static");

            if (Text == null && Value != null)
            {
                if (PredefinedValues != null && PredefinedValues.ContainsKey(Value.ToString()))
                {
                    Value = PredefinedValues[Value.ToString()];
                }

                //if (DataFormat != null)
                //{
                //    Value = DynamicHelper.GetCustomFormatField(Value.ToString(), DataFormat, GetFormatFields());
                //}

                if (StringFormat!=null)
                {
                    literal.InnerHtml = string.Format(StringFormat, Value);
                }
                else
                {
                    literal.SetInnerText(Value == null ? null : Value.ToString());
                }
            }
            else if (Text != null)
            {
                if (PredefinedValues != null && PredefinedValues.ContainsKey(Text))
                {
                    Text = PredefinedValues[Text];
                }

                if (StringFormat == null)
                {
                    literal.SetInnerText(Text);
                }
                else
                {
                    literal.InnerHtml = string.Format(StringFormat, Text);
                }
            }

            literal.InnerHtml += Html;
            html.Replace(PlaceHolders.Literal, literal.ToString());

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }

        protected override string BuildDefaultTemplate()
        {
            return PlaceHolders.Literal;

        }
    }
}
