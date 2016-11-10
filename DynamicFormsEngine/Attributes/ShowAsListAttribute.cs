using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicFormsEngine.Attributes
{

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ShowAsListAttribute : System.Attribute
    {
        public Type ResourceType { get; set; }
        public string Placeholder { get; set; }
        public Type DataSourceType { get; set; }
        public string DataSourceMethod { get; set; }

        public ShowAsListAttribute(string Placeholder, Type DataSourceType, string DataSourceMethod, Type ResourceType = null)
        {
            this.Placeholder = Placeholder;
            this.DataSourceType = DataSourceType;
            this.DataSourceMethod = DataSourceMethod;
            this.ResourceType = ResourceType;
        }
    }
}
