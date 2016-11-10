using DynamicFormsEngine.Enums;
using DynamicFormsEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicFormsEngine
{
    public class Button
    {
        public string CssClass { get; set; }

        public string Icon { get; set; }

        public string Link { get; set; }

        public string GetLink()
        {
            return DynamicHtmlHelpers.GetLink(Link);
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        public string GetTitle()
        {
            return title != null ? title : "";
        }

        public string MappedName { get; set; }
    }
}
