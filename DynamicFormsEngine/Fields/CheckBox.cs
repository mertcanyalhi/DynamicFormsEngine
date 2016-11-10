using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents a single html checkbox input field.
    /// </summary>
    [Serializable]
    public class CheckBox : InputField
    {
        private string _checkedValue = "True";
        private string _uncheckedValue = "False";

        /// <summary>
        /// The text to be used as the user's response when they check the checkbox.
        /// </summary>
        public string CheckedValue
        {
            get
            {
                return _checkedValue;
            }
            set
            {
                _checkedValue = value;
            }
        }
        /// <summary>
        /// The text to be used as the user's response when they do not check the checkbox.
        /// </summary>
        public string UncheckedValue
        {
            get
            {
                return _uncheckedValue;
            }
            set
            {
                _uncheckedValue = value;
            }
        }
        /// <summary>
        /// The state of the checkbox.
        /// </summary>
        public bool Checked { get; set; }

        public override void SetValue(object value)
        {
            Checked = Convert.ToBoolean(value);
        }


        public override object Response
        {
            get
            {
                return Checked ? _checkedValue : _uncheckedValue;
            }
        }

        public CheckBox()
        {
            // give the checkbox a different default prompt class
            _labelClass = "control-label";
        }

        public override bool Validate()
        {
            ClearError();

            if (MappedName==null && Required && !Checked)
            {
                Error = GetRequiredMessage();
            }

            FireValidated();
            return ErrorIsClear;
        }

        public override string RenderHtml()
        {
            var inputName = GetHtmlId();
            var html = new StringBuilder(Template);

            var error = new TagBuilder("label");
            error.Attributes.Add("data-valmsg-for", inputName);
            error.Attributes.Add("data-valmsg-replace", "true");

            // error label
            if (!ErrorIsClear)
            {
                var innerSpan = new TagBuilder("span");
                innerSpan.GenerateId(inputName + "-error");
                innerSpan.SetInnerText(Error);

                error.InnerHtml = innerSpan.ToString();
                error.Attributes.Add("class", _errorClass);
            }

            html.Replace(PlaceHolders.Error, error.ToString());

            // checkbox input
            var chk = new TagBuilder("input");
            chk.Attributes.Add("id", inputName);
            chk.Attributes.Add("name", inputName);
            chk.Attributes.Add("type", "checkbox");
            if (Checked) chk.Attributes.Add("checked", "checked");
            chk.Attributes.Add("value", bool.TrueString);
            chk.MergeAttributes(_inputHtmlAttributes);

            // hidden input (so that value is posted when checkbox is unchecked)
            var hdn = new TagBuilder("input");
            hdn.Attributes.Add("type", "hidden");
            hdn.Attributes.Add("id", inputName + "_hidden");
            hdn.Attributes.Add("name", inputName);
            hdn.Attributes.Add("value", bool.FalseString);


            string helper = "";
            var inputHelper = new TagBuilder("span");

            if (!string.IsNullOrEmpty(Description))
            {
                //inputHelper.AddCssClass("help-block");
                inputHelper.InnerHtml = GetDescription();
                helper = inputHelper.ToString();
            }


            html.Replace(PlaceHolders.Input, chk.ToString(TagRenderMode.SelfClosing) + hdn.ToString(TagRenderMode.SelfClosing) + helper);

            // prompt label
            var label = new TagBuilder("label");
            label.SetInnerText(GetTitle());
            label.Attributes.Add("for", inputName);
            label.AddCssClass(_labelClass);
            label.AddCssClass(LabelClass);
            html.Replace(PlaceHolders.Label, label.ToString());

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass + " checkbox-inline");

            return html.ToString();
        }

        protected override string BuildDefaultTemplate()
        {
            var innerWrapper = new TagBuilder("div");
            innerWrapper.AddCssClass("{InputClass}");
            innerWrapper.InnerHtml = PlaceHolders.Input + PlaceHolders.Error;

            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("form-group");
            wrapper.Attributes["id"] = PlaceHolders.FieldWrapperId;
            wrapper.InnerHtml = PlaceHolders.Label + innerWrapper;

            return wrapper.ToString();
        }
    }
}
