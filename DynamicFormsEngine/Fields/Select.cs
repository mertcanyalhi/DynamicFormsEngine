using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents an html select element.
    /// </summary>
    [Serializable]
    public class Select : ListField
    {
        public string StringFormat { get; set; }

        public List<string> GetFormatFields()
        {
            List<string> nameFieldNames = new List<string>();

            if (StringFormat != null)
            {
                MatchCollection matches = new Regex("{.*?}").Matches(StringFormat);

                foreach (Match match in matches)
                {
                    nameFieldNames.Add(match.Value.Replace("{", "").Replace("}", ""));
                }
            }

            return nameFieldNames;
        }

        private int? _size;

        /// <summary>
        /// The number of options to display at a time.
        /// </summary>
        public int Size
        {
            get
            {
                if (_size == null)
                {
                    _size = 1;
                }

                return (int)_size;
            }
            set { _size = value; }
        }

        public override void SetValue(object value)
        {
            if (Choices != null)
            {
                var _choices = Choices.Where(x => x.Value.ToString() == value.ToString());

                foreach (var c in _choices)
                {
                    c.Selected = true;
                }
            }
        }

        public bool MultipleTags { get; set; }

        private bool _multiple = false;

        /// <summary>
        /// Determines whether the select element will accept multiple selections.
        /// </summary>
        public bool MultipleSelection
        {
            get
            {
                return _multiple;
            }
            set
            {
                _multiple = value;
            }
        }

        /// <summary>
        /// The text to be rendered as the first option in the select list when ShowEmptyOption is set to true.
        /// </summary>
        public string EmptyOption { get; set; }

        public string GetEmptyOption()
        {
            if (EmptyOption == null)
            {
                return "Please select";
            }
            else
            {
                return EmptyOption;
            }
        }
        /// <summary>
        /// Determines whether a valueless option is rendered as the first option in the list.
        /// </summary>
        public bool ShowEmptyOption { get; set; }

        public string Placeholder { get; set; }

        public string GetPlaceholder()
        {
            if (string.IsNullOrWhiteSpace(Placeholder))
            {
                return "Please select";
            }
            else
            {
                return Placeholder;
            }
        }

        public override string RenderHtml()
        {
            var html = new StringBuilder(Template);
            var inputName = GetHtmlId();

            #region Prompt
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

            #region Error
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

            // open select element
            var input = new StringBuilder();
            var select = new TagBuilder("select");
            select.Attributes.Add("id", inputName);
            select.Attributes.Add("name", inputName);

            _inputHtmlAttributes["size"] = Size.ToString();
            if (_multiple)
                _inputHtmlAttributes["multiple"] = "true";

            select.MergeAttributes(_inputHtmlAttributes);

            select.Attributes.Add("data-placeholder", GetPlaceholder());

            select.AddCssClass("form-control");
            select.AddCssClass(_multiple ? (MultipleTags ? "select2" : "bs-select") : "select2me input-large");

            input.Append(select.ToString(TagRenderMode.StartTag));

            // initial empty option
            if (ShowEmptyOption)
            {
                var opt = new TagBuilder("option");
                opt.Attributes.Add("value", null);
                opt.SetInnerText(GetEmptyOption());
                input.Append(opt.ToString());
            }

            // options
            foreach (var choice in _choices)
            {
                var opt = new TagBuilder("option");
                opt.Attributes.Add("value", choice.Value);
                if (choice.Selected)
                    opt.Attributes.Add("selected", "selected");
                opt.MergeAttributes(choice.HtmlAttributes);
                opt.SetInnerText(choice.Text);
                input.Append(opt.ToString());
            }

            // close select element
            input.Append(select.ToString(TagRenderMode.EndTag));

            // add hidden tag, so that a value always gets sent for select tags
            var hidden = new TagBuilder("input");
            hidden.Attributes.Add("type", "hidden");
            hidden.Attributes.Add("id", inputName + "_hidden");
            hidden.Attributes.Add("name", inputName);
            hidden.Attributes.Add("value", string.Empty);

            string helper = "";
            var inputHelper = new TagBuilder("span");

            if (!string.IsNullOrEmpty(Description))
            {
                inputHelper.AddCssClass("help-block");
                inputHelper.InnerHtml = GetDescription();
                helper = inputHelper.ToString();
            }

            html.Replace(PlaceHolders.Input, input.ToString() + hidden.ToString(TagRenderMode.SelfClosing) + helper);

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }
    }
}
