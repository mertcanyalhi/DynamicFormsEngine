using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DynamicFormsEngine.Fields
{
    /// <summary>
    /// Represents an html input field that will accept a text response from the user.
    /// </summary>
    [Serializable]
    public abstract class TextField : InputField
    {
        private string _regexMessage = "Invalid";

        /// <summary>
        /// A regular expression that will be applied to the user's text respone for validation.
        /// </summary>
        public string RegularExpression { get; set; }
        /// <summary>
        /// The error message that is displayed to the user when their response does no match the regular expression.
        /// </summary>
        public string RegexMessage
        {
            get
            {
                return _regexMessage;
            }
            set
            {
                _regexMessage = value;
            }
        }

        public string Type { get; set; }
        public string TypeMessage { get; set; }
        public int? LengthMin { get; set; }
        public int? LengthMax { get; set; }
        public string LengthMessage { get; set; }

        public string GetLengthMessage()
        {
            if (LengthMessage == null)
            {
                return string.Format("{0} should be at most {1} characters.", GetTitle(), LengthMax);
            }
            else
            {
                return LengthMessage;
            }
        }

        public string GetRegexMessage()
        {
            if (RegexMessage == null)
            {
                return string.Format("{0} is invalid.", GetTitle());
            }
            else
            {
                return RegexMessage;
            }
        }

        public string GetRegex()
        {
            if (RegularExpression == null)
            {
                return string.Empty;
            }
            else
            {
                return RegularExpression
                                    .Replace("%%DecimalSeparator%%", NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
            }
        }

        public string GetTypeMessage()
        {
            if (TypeMessage == null)
            {
                return string.Format("{0} is invalid.", GetTitle());
            }
            else
            {
                return TypeMessage;
            }
        }

        private string _value;
        public string Value
        {
            get
            {
                return _value ?? "";
            }
            set
            {
                _value = value;
            }
        }

        public override object Response
        {
            get { return Value.Trim(); }
        }
        public override bool Validate()
        {
            ClearError();

            if (string.IsNullOrEmpty((string)Response))
            {
                if (Required)
                {
                    // invalid: is required and no response has been given
                    Error = GetRequiredMessage();
                }
            }
            else if ((LengthMin != null && !(((string)Response).Length >= LengthMin)) ||
                     (LengthMax != null && LengthMax >= 0 && ((string)Response).Length > LengthMax))
            {
                Error = GetLengthMessage();
            }
            else
            {
                if (!string.IsNullOrEmpty(RegularExpression))
                {
                    var regex = new Regex(GetRegex());
                    if (!regex.IsMatch(Value))
                    {
                        // invalid: has regex and response doesn't match
                        Error = GetRegexMessage();
                    }
                }
            }

            FireValidated();
            return ErrorIsClear;
        }
    }
}