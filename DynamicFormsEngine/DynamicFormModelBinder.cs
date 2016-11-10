using System;
using System.Linq;
using System.Web.Mvc;
using DynamicFormsEngine.Fields;
using DynamicFormsEngine.Utilities;
using DynamicFormsEngine.Models;
using System.Collections.Generic;

namespace DynamicFormsEngine
{
    class DynamicFormModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var postedForm = controllerContext.RequestContext.HttpContext.Request.Form;
            var postedFiles = controllerContext.RequestContext.HttpContext.Request.Files;

            var allKeys = postedForm.AllKeys.Union(postedFiles.AllKeys);

            var form = (Form)bindingContext.Model;
            if (form == null && !string.IsNullOrEmpty(postedForm[MagicStrings.MvcDynamicSerializedForm]))
            {
                form = SerializationUtility.Deserialize<Form>(postedForm[MagicStrings.MvcDynamicSerializedForm]);
            }

            if (form == null)
                throw new NullReferenceException("The dynamic form object was not found. Be sure to include PlaceHolders.SerializedForm in your form template.");



            foreach (var key in allKeys.Where(x => x.StartsWith(form.FieldPrefix)))
            {
                string fieldKey = key.Remove(0, form.FieldPrefix.Length);
                InputField dynField = form.InputFields.SingleOrDefault(f => f.Key == fieldKey);

                if (dynField == null)
                    continue;

                if (dynField is TextField)
                {
                    var txtField = (TextField)dynField;
                    txtField.Value = postedForm[key];
                }
                else if (dynField is NumberInput)
                {
                    var numField = (NumberInput)dynField;
                    double value;

                    if (Double.TryParse(postedForm[key], out value))
                    {
                        numField.Value = value;
                    }
                    else
                    {
                        numField.Value = null;
                    }
                }
                else if (dynField is DateTimePicker)
                {
                    var dtField = (DateTimePicker)dynField;
                    DateTime value;

                    if (DateTime.TryParse(postedForm[key], out value))
                    {
                        dtField.Value = value;
                    }
                    else
                    {
                        dtField.Value = null;
                    }
                }
                else if (dynField is DatePicker)
                {
                    var dtField = (DatePicker)dynField;
                    DateTime value;

                    if (DateTime.TryParse(postedForm[key], out value))
                    {
                        dtField.Value = value;
                    }
                    else
                    {
                        dtField.Value = null;
                    }
                }
                else if (dynField is ListField)
                {
                    var lstField = (ListField)dynField;
                    
                    // clear all choice selections            
                    foreach (var choice in lstField.Choices)
                        choice.Selected = false;

                    // set current selections
                    foreach (string value in postedForm.GetValues(key))
                    {
                        var choice = lstField.Choices.FirstOrDefault(x => x.Value == value);
                        if (choice != null)
                            choice.Selected = true;
                    }

                    //lstField.Choices.Remove(.Remove(""); what was this for?
                }
                else if (dynField is CheckBox)
                {
                    var chkField = (CheckBox)dynField;
                    chkField.Checked = bool.Parse(postedForm.GetValues(key)[0]);
                }
                else if (dynField is FileUpload)
                {
                    var fileField = (FileUpload)dynField;

                    if (fileField.PostedFiles == null)
                    {
                        fileField.PostedFiles = new List<UploadedFile>();
                    }

                    if (postedForm[key] != null && postedForm[key] != null)
                    {
                        string[] keys = postedForm[key].Split(';');
                        //List<UploadedFile> toBeRemoved = new List<UploadedFile>();

                        foreach (var fileId in keys)
                        {
                            if (fileId.ToUpperInvariant().StartsWith("R:"))
                            {
                                fileField.PostedFiles.RemoveAll(x => x.FileId.ToString().ToUpperInvariant() == fileId.Substring(2).ToUpperInvariant());
                            }
                            else
                            {
                                UploadedFile file = UploadedFile.Files.FirstOrDefault(x => x.FileId.ToString().ToUpperInvariant() == fileId.ToUpperInvariant());

                                if (file != null)
                                {
                                    //toBeRemoved.Add(file);
                                    fileField.PostedFiles.Add(file);
                                }
                            }
                        }

                        //UploadedFile.Files.RemoveAll(x => toBeRemoved.Contains(x));
                        //fileField.PostedFiles.Add(postedFiles[key]);
                    }
                }
                else if (dynField is Hidden)
                {
                    var hiddenField = (Hidden)dynField;
                    hiddenField.Value = postedForm[key];
                }
            }

            form.FireModelBoundEvents();
            return form;
        }
    }
}
