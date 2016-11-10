using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{
    [Serializable]
    public class DateTimePicker : InputField
    {
        public DateTime? Value { get; set; }

        public override void SetValue(object value)
        {
            Value = Convert.ToDateTime(value);
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

            #region DateTimePicker
            var inputHolder = new TagBuilder("div");
            inputHolder.AddCssClass("input-group date form_datetime");

            #region Textbox
            var txt = new TagBuilder("input");
            txt.Attributes.Add("type", "text");
            txt.Attributes.Add("name", inputName);
            txt.Attributes.Add("id", inputName);
            if (Value != null) txt.Attributes.Add("value", ((DateTime)Value).ToString("g"));
            txt.Attributes.Add("readonly", "readonly");
            txt.AddCssClass("form-control");

            #region Data Validation
            txt.Attributes.Add("data-val", "true");

            if (Required)
            {
                txt.Attributes.Add("data-val-required", GetRequiredMessage());
            }
            #endregion

            txt.MergeAttributes(_inputHtmlAttributes);
            #endregion

            inputHolder.InnerHtml = txt.ToString(TagRenderMode.SelfClosing) + @"<span class=""input-group-btn""><button class=""btn default date-set"" type=""button""><i class=""fa fa-calendar""></i></button></span>";
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

            html.Replace(PlaceHolders.Input, inputHolder.ToString() + helper);

            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }
    }
}
