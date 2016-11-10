using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{

    [Serializable]
    public class NumberInput : InputField
    {
        public bool IsInt { get; set; }
        public double? Value { get; set; }

        public override void SetValue(object value)
        {
            Value = Convert.ToDouble(value);
        }

        public override object Response
        {
            get { return Value == null ? null : Value.ToString(); }
        }

        public override bool Validate()
        {
            ClearError();

            if (Response == null)
            {
                if (Required)
                {
                    Error = GetRequiredMessage();
                }
            }

            FireValidated();
            return ErrorIsClear;
        }

        public override string RenderHtml()
        {
            var html = new StringBuilder(Template);
            var inputName = GetHtmlId();

            #region Label
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
            #endregion

            #region Error Holder
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
            #endregion

            #region Textbox
            var txt = new TagBuilder("input");
            txt.Attributes.Add("name", inputName);
            txt.Attributes.Add("id", inputName);
            txt.Attributes.Add("value", Value.ToString());
            txt.Attributes.Add("type", "text");
            txt.AddCssClass("form-control");
            txt.AddCssClass(IsInt ? "number_spinner" : "decimal_spinner");

            #region Data Validation
            txt.Attributes.Add("data-val", "true");
            txt.Attributes.Add("data-val-number", "Please enter a valid number");

            if (Required)
            {
                txt.Attributes.Add("data-val-required", GetRequiredMessage());
            }
            #endregion

            txt.MergeAttributes(_inputHtmlAttributes);
            #endregion

            #region Holder
            var holder = new TagBuilder("div");
            holder.AddCssClass("input-inline input-medium");
            holder.InnerHtml = txt.ToString(TagRenderMode.SelfClosing);
            #endregion

            #region Helper
            string helper = "";
            var inputHelper = new TagBuilder("span");

            if (!string.IsNullOrEmpty(Description))
            {
                inputHelper.AddCssClass("help-block");
                inputHelper.InnerHtml = GetDescription();
                helper = inputHelper.ToString();
            }
            #endregion

            html.Replace(PlaceHolders.Input, holder.ToString() + helper);

            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }
    }
}
