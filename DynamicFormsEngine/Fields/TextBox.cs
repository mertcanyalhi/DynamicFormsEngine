using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents an html textbox input element.
    /// </summary>
    [Serializable]
    public class TextBox : TextField
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
            label.AddCssClass(_labelClass);
            label.AddCssClass(LabelClass);
            html.Replace(PlaceHolders.Label, label.ToString());

            var error = new TagBuilder("span");
            error.Attributes.Add("data-valmsg-for", inputName);
            error.Attributes.Add("data-valmsg-replace", "true");

            if (!ErrorIsClear)
            {
                var innerSpan = new TagBuilder("span");
                innerSpan.GenerateId(inputName + "-error");
                innerSpan.SetInnerText(Error);

                error.InnerHtml = innerSpan.ToString();
                error.Attributes.Add("class", _errorClass);
            }
            else
            {
                error.Attributes.Add("class", _noErrorClass);
            }

            html.Replace(PlaceHolders.Error, error.ToString());

            // input element
            var txt = new TagBuilder("input");

            if (string.IsNullOrEmpty(Type))
            {
                txt.Attributes.Add("type", "text"); ;
            }
            else
            {
                txt.Attributes.Add("type", Type);
            }

            txt.Attributes.Add("name", inputName);
            txt.Attributes.Add("id", inputName);
            txt.Attributes.Add("value", Value);
            txt.AddCssClass("form-control text-box single-line");

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

            if (LengthMax != null)
                txt.Attributes.Add("data-val-length", GetLengthMessage());

            if (RegularExpression != null)
                txt.Attributes.Add("data-val-regex-pattern", GetRegex());

            if (RegexMessage != null)
                txt.Attributes.Add("data-val-regex", GetRegexMessage());

            switch (Type)
            {
                case "email":
                    txt.Attributes.Add("data-val-email", GetTypeMessage());
                    break;
            }
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

            html.Replace(PlaceHolders.Input, txt.ToString(TagRenderMode.SelfClosing) + helper);

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }
    }
}
