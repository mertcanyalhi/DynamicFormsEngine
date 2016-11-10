using System;
using System.Text;
using System.Web.Mvc;
using DynamicFormsEngine.Utilities;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents an html textarea element.
    /// </summary>
    [Serializable]
    public class TextArea : TextField
    {
        public override void SetValue(object value)
        {
            Value = Convert.ToString(value);
        }

        public override string RenderHtml()
        {
            var html = new StringBuilder(Template);
            var inputName = GetHtmlId();

            // prompt label
            var label = new TagBuilder("label");
            label.SetInnerText(GetTitle());

            if (Required)
            {
                var required = new TagBuilder("span");
                required.AddCssClass("required");
                required.MergeAttribute("aria-required", "true");
                required.SetInnerText("*");
                label.InnerHtml += required;
            }

            label.Attributes.Add("for", inputName);
            label.Attributes.Add("class", _labelClass);
            label.AddCssClass(LabelClass);
            html.Replace(PlaceHolders.Label, label.ToString());

            // error label
            if (!ErrorIsClear)
            {
                var error = new TagBuilder("label");
                error.Attributes.Add("for", inputName);
                error.Attributes.Add("class", _errorClass);
                error.SetInnerText(Error);
                html.Replace(PlaceHolders.Error, error.ToString());
            }

            // input element
            var txt = new TagBuilder("textarea");
            txt.Attributes.Add("name", inputName);
            txt.Attributes.Add("id", inputName);
            txt.SetInnerText("{Value}");
            txt.AddCssClass("form-control");

            if (_inputHtmlAttributes != null && _inputHtmlAttributes.ContainsKey("class"))
                txt.AddCssClass(_inputHtmlAttributes["class"]);

            #region Data Validation
            txt.Attributes.Add("data-val", "true");

            if (Required)
            {
                txt.Attributes.Add("data-val-required", GetRequiredMessage());
            }

            if (LengthMin != null && LengthMin > -1)
                txt.Attributes.Add("data-val-length-min", LengthMin.ToString());

            if (LengthMax != null && LengthMax > 0)
                txt.Attributes.Add("data-val-length-max", LengthMax.ToString());

            if (LengthMessage != null)
                txt.Attributes.Add("data-val-length", GetLengthMessage());

            if (RegularExpression != null)
                txt.Attributes.Add("data-val-regex-pattern", GetRegex());

            if (RegexMessage != null)
                txt.Attributes.Add("data-val-regex", GetRegexMessage());
            #endregion

            txt.MergeAttributes(_inputHtmlAttributes);

            string helper = "";
            var inputHelper = new TagBuilder("span");

            if (!string.IsNullOrEmpty(Description))
            {
                inputHelper.AddCssClass("help-block");
                inputHelper.InnerHtml = GetDescription();
                helper = inputHelper.ToString();
            }

            html.Replace(PlaceHolders.Input, txt.ToString() + helper);

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);
            html.Replace("{Value}", System.Web.HttpUtility.HtmlEncode(Value));

            return html.ToString();
        }
    }
}