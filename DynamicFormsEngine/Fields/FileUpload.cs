using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;
using DynamicFormsEngine.Models;
using System.Runtime.Serialization.Formatters.Binary;

namespace DynamicFormsEngine.Fields
{
    public delegate void FilePostedEventHandler(FileUpload fileUploadField, EventArgs e);

    [Serializable]
    public class FileUpload : InputField
    {
        public event FilePostedEventHandler Posted;

        public override void SetValue(object value)
        {
            if (value is List<UploadedFile>)
                _postedFiles = (List<UploadedFile>)value;
        }

        [NonSerialized]
        private List<UploadedFile> _postedFiles;
        private string _invalidExtensionError = "Invalid File Type";

        public bool Multiple { get; set; }

        public bool ReadOnly { get; set; }

        public bool DisplayFileListEmpty { get; set; }

        public string InvalidExtensionError
        {
            get { return _invalidExtensionError; }
            set { _invalidExtensionError = value; }
        }

        public List<UploadedFile> PostedFiles
        {
            get { return _postedFiles; }
            set { _postedFiles = value; }
        }

        /// <summary>
        /// A comma delimited list of acceptable file extensions.
        /// </summary>
        public string ValidExtensions { get; set; }

        public bool FileWasPosted
        {
            get
            {
                return PostedFiles != null && PostedFiles.Count > 0;
            }
        }

        public override object Response
        {
            get
            {
                object result = null;

                if (PostedFiles != null && PostedFiles.Count > 0)
                {
                    result = PostedFiles;
                }

                return result;
            }
        }

        public override bool Validate()
        {
            ClearError();

            if (Required && !FileWasPosted)
            {
                Error = GetRequiredMessage();
            }
            else if (!string.IsNullOrEmpty(ValidExtensions))
            {
                var exts = ValidExtensions.ToUpperInvariant().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var PostedFile in PostedFiles)
                {
                    if (!exts.Contains(Path.GetExtension(PostedFile.FileName).ToUpperInvariant()))
                    {
                        Error = InvalidExtensionError;
                        break;
                    }
                }
            }

            FireValidated();
            return ErrorIsClear;
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
            label.Attributes.Add("class", _labelClass);
            label.AddCssClass(LabelClass);
            html.Replace(PlaceHolders.Label, label.ToString());

            // error label
            if (!ErrorIsClear)
            {
                var error = new TagBuilder("label");
                error.Attributes.Add("for", inputName);
                error.Attributes.Add("class", _errorClass);
                error.SetInnerText(Error);
                html.Replace(PlaceHolders.Error, error.ToString());
            }

            string uploadedFileList = "<div class='fileupload-uploadedfiles'>";

            if (PostedFiles != null && PostedFiles.Count > 0)
            {
                foreach (var postedFile in PostedFiles)
                {
                    if (ReadOnly)
                    {
                        uploadedFileList += "<div><span class=\"fileupload-file\"><i class=\"fa fa-file-o\"></i></span> <a href=\"{PATH}?downloadfile=" + postedFile.FileId + "\">" + postedFile.FileName + "</a></div>";
                    }
                    else
                    {
                        uploadedFileList += "<div><span class=\"fileupload-file\"><i class=\"fa fa-file-o\"></i></span> <a href=\"{PATH}?downloadfile=" + postedFile.FileId + "\">" + postedFile.FileName + "</a> <span class=\"fileupload-removefile\" data-controlid=\"" + inputName + "\" data-fileid=\"" + postedFile.FileId + "\"><i class=\"fa fa-times\"></i></span></div>";
                    }
                }
            }
            else if (DisplayFileListEmpty)
            {
                uploadedFileList += "<i>No file uploaded</i>";
            }

            uploadedFileList += "</div>";

            var fileUploadContainer = new TagBuilder("div");

            if (!ReadOnly)
            {
                fileUploadContainer.Attributes.Add("id", inputName + "_container");
                fileUploadContainer.Attributes.Add("name", inputName + "_container");
                fileUploadContainer.Attributes.Add("class", "fileupload-container");

                var fileUploadButton = new TagBuilder("span");
                fileUploadButton.AddCssClass("btn btn-success fileinput-button");
                fileUploadButton.InnerHtml = "<i class=\"glyphicon glyphicon-plus\"></i><span>Add files</span>";

                var fileUpload = new TagBuilder("input");
                fileUpload.Attributes.Add("name", inputName + "_file[]");
                fileUpload.Attributes.Add("id", inputName + "_file");
                fileUpload.Attributes.Add("type", "file");
                fileUpload.Attributes.Add("multiple", null);
                fileUpload.Attributes.Add("class", "fileupload");
                fileUpload.Attributes.Add("data-controlname", inputName);
                fileUpload.Attributes.Add("data-maxnumberoffiles", Multiple ? "10" : "1");

                if (!string.IsNullOrWhiteSpace(ValidExtensions))
                {
                    fileUpload.Attributes.Add("data-validextensions", ValidExtensions.Replace(';', '|'));
                }
                else
                {
                    fileUpload.Attributes.Add("data-validextensions", "jpg|jpeg|png|gif|txt|pdf|zip|rar|7z|dwg|dxf");
                }

                if (Required)
                {
                    fileUpload.Attributes.Add("data-val-required", GetRequiredMessage());
                }

                fileUpload.MergeAttributes(_inputHtmlAttributes);

                fileUploadButton.InnerHtml += fileUpload.ToString(TagRenderMode.SelfClosing) + "<input id=\"" + inputName + "\" type=\"hidden\" name=\"" + inputName + "\" />";

                fileUploadContainer.InnerHtml = fileUploadButton.ToString() + "<br /><br /><div id=\"progress\" class=\"progress\"><div class=\"progress-bar progress-bar-success\"></div></div><div id=\"files\" class=\"files\"></div>";
            }
            string helper = "";
            var inputHelper = new TagBuilder("span");

            if (!string.IsNullOrEmpty(Description))
            {
                inputHelper.AddCssClass("help-block");
                inputHelper.InnerHtml = GetDescription();
                helper = inputHelper.ToString();
            }

            html.Replace(PlaceHolders.Input, uploadedFileList + fileUploadContainer.ToString() + helper);

            // wrapper id
            html.Replace(PlaceHolders.FieldWrapperId, GetWrapperId());
            html.Replace("{InputClass}", InputClass);

            return html.ToString();
        }

        internal void FireFilePosted()
        {
            if (FileWasPosted && Posted != null)
                Posted(this, new EventArgs());
        }
    }
}