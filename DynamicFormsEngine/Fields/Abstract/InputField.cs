using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace DynamicFormsEngine.Fields
{

    public delegate void ValidatedEventHandler(InputField inputField, InputFieldValidationEventArgs e);

    /// <summary>
    /// Represents a dynamically generated html input field.
    /// </summary>
    [Serializable]
    public abstract class InputField : Field
    {
        public event ValidatedEventHandler Validated;

        public abstract void SetValue(object value);

        protected string _requiredMessage = null;
        protected string _errorClass = "text-danger field-validation-invalid field-validation-error";
        protected string _noErrorClass = "text-danger field-validation-valid";
        protected SerializableDictionary<string, string> _inputHtmlAttributes = new SerializableDictionary<string, string>();

        public string Description { get; set; }

        /// <summary>
        /// String representing the user's response to the field.
        /// </summary>
        public abstract object Response { get; }
        /// <summary>
        /// Whether the field must be completed to be valid.
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// The error message that the end user sees if they do not complete the field.
        /// </summary>
        public string RequiredMessage
        {
            get
            {
                return _requiredMessage;
            }
            set
            {
                _requiredMessage = value;
            }
        }

        public string GetRequiredMessage()
        {
            return RequiredMessage == null ? string.Format("{0} is required.", GetTitle()) : RequiredMessage;
        }

        public string GetDescription()
        {
            return Description == null ? null : Description;
        }
        /// <summary>
        /// The error message that the end user sees.
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// The class attribute of the label element that is used to display an error message to the user.
        /// </summary>
        public string ErrorClass
        {
            get
            {
                return _errorClass;
            }
            set
            {
                _errorClass = value;
            }
        }
        /// <summary>
        /// True if the field is valid; false otherwise.
        /// </summary>
        public bool ErrorIsClear
        {
            get
            {
                return string.IsNullOrEmpty(Error);
            }
        }
        /// <summary>
        /// Collection of html attribute names and values that will be applied to the rendered input elements.
        /// For list fields, these will be applied to every ListItem.
        /// Use the ListItem.HtmlAttributes dictionary for rendering attributes for individual list items.
        /// </summary>
        public SerializableDictionary<string, string> InputHtmlAttributes
        {
            get
            {
                return _inputHtmlAttributes;
            }
            set
            {
                _inputHtmlAttributes = value;
            }
        }
        /// <summary>
        /// Validates the user's response.
        /// </summary>
        /// <returns></returns>
        public abstract bool Validate();
        /// <summary>
        /// Removes the message stored in the Error property.
        /// </summary>
        public void ClearError()
        {
            Error = null;
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

        protected virtual void FireValidated()
        {
            if (Validated != null)
                Validated(this, new InputFieldValidationEventArgs { IsValid = ErrorIsClear });
        }

    }

    public class InputFieldValidationEventArgs : EventArgs
    {
        public bool IsValid { get; set; }
    }

}