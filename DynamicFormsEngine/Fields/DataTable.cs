using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DynamicFormsEngine.Fields
{

    [Serializable]
    public class DataTable : Field
    {
        public string IdField { get; set; }

        public bool StyleStriped { get; set; }

        public bool StyleBordered { get; set; }

        public bool StyleHover { get; set; }

        public string ServiceList { get; set; }

        public string ServiceDelete { get; set; }

        public string ServiceEdit { get; set; }

        public string ServiceCreate { get; set; }

        public string FormEdit { get; set; }

        public string FormCreate { get; set; }

        public string FormDetails { get; set; }

        public int DefaultPageSize { get; set; }

        public SerializableDictionary<string, string> Parameters { get; set; }

        public List<ListColumn> Columns { get; set; }

        public override string RenderHtml()
        {
            if (Columns==null)
            {
                Columns = new List<ListColumn>();
            }

            #region Template
            if (Title != null)
            {
                var innerWrapper = new TagBuilder("div");
                innerWrapper.AddCssClass("{InputClass}");
                innerWrapper.InnerHtml = PlaceHolders.DataTable;

                var wrapper = new TagBuilder("div");
                wrapper.AddCssClass("form-group");
                wrapper.Attributes["id"] = PlaceHolders.FieldWrapperId;
                wrapper.InnerHtml = PlaceHolders.Label + innerWrapper;
                Template = wrapper.ToString();
            }

            var html = new StringBuilder(Template); 
            #endregion

            var inputName = GetHtmlId();

            #region Label
            var label = new TagBuilder("h3");

            label.SetInnerText(GetTitle());
            label.Attributes.Add("class", _labelClass);
            label.AddCssClass(LabelClass);
            html.Replace(PlaceHolders.Label, label.ToString()); 
            #endregion

            #region Table
            var table = new TagBuilder("table");
            table.AddCssClass("table");

            if (StyleBordered)
                table.AddCssClass("table-bordered");

            if (StyleHover)
                table.AddCssClass("table-hover");

            if (StyleStriped)
                table.AddCssClass("table-striped");

            table.Attributes["id"] = "table" + Key;

            #region thead
            var thead = new TagBuilder("thead");

            #region tr head
            var trHead = new TagBuilder("tr");
            trHead.AddCssClass("heading");
            trHead.Attributes["role"] = "row";

            if (!string.IsNullOrWhiteSpace(ServiceDelete) || !string.IsNullOrWhiteSpace(ServiceEdit) || !string.IsNullOrWhiteSpace(FormCreate) || !string.IsNullOrWhiteSpace(FormEdit) || !string.IsNullOrWhiteSpace(FormDetails))
            {
                trHead.InnerHtml = "<th></th>";
            }

            foreach (var col in Columns)
            {
                trHead.InnerHtml += "<th></th>";
            }
            #endregion

            thead.InnerHtml += trHead.ToString(); 
            #endregion

            table.InnerHtml = thead.ToString();

            html.Replace(PlaceHolders.DataTable, table.ToString()); 
            #endregion

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }

        protected override string BuildDefaultTemplate()
        {
            return PlaceHolders.DataTable;

        }
    }
}
