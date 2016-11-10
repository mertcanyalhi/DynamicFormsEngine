using DynamicFormsEngine.Attributes;
using DynamicFormsEngine.Enums;
using DynamicFormsEngine.Fields;
using DynamicFormsEngine.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace DynamicFormsEngine.Utilities
{
    public class FormGeneratorUtility
    {
        private static string GetResource(string resourceKey, Type resourceManagerProvider)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey;
        }


        private static string GetFieldListHtml(List<ListItem> listItems)
        {
            if (listItems == null || listItems.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                var items = listItems.Where(x => x.Selected);

                TagBuilder ul = new TagBuilder("ul");
                ul.AddCssClass("literal-list");
                foreach (var item in items)
                {
                    TagBuilder li = new TagBuilder("li");
                    li.InnerHtml = item.Text;
                    ul.InnerHtml += li.ToString();
                }
                return ul.ToString();
            }
        }

        public static Form GetForm(List<PropertyInfo> properties)
        {
            Form form = new Form();

            foreach (var property in properties)
            {
                DisplayAttribute displayAttribute = (DisplayAttribute)property.GetCustomAttribute(typeof(DisplayAttribute));
                RequiredAttribute requiredAttribute = (RequiredAttribute)property.GetCustomAttribute(typeof(RequiredAttribute));
                EmailAddressAttribute emailAttribute = (EmailAddressAttribute)property.GetCustomAttribute(typeof(EmailAddressAttribute));
                StringLengthAttribute lengthAttribute = (StringLengthAttribute)property.GetCustomAttribute(typeof(StringLengthAttribute));
                RegularExpressionAttribute regexAttribute = (RegularExpressionAttribute)property.GetCustomAttribute(typeof(RegularExpressionAttribute));
                ShowAsListAttribute showAsListAttribute = (ShowAsListAttribute)property.GetCustomAttribute(typeof(ShowAsListAttribute));

                if (displayAttribute.AutoGenerateField)
                {
                    string title = displayAttribute.Name;
                    string description = displayAttribute.Description;
                    string group = displayAttribute.GroupName;
                    string type = null;
                    string typeMessage = null;
                    string lengthMessage = null;
                    int lengthMin = -1;
                    int lengthMax = -1;
                    string regex = null;
                    string regexMessage = null;
                    bool required = true;

                    #region Attributes
                    #region General
                    if (displayAttribute.ResourceType != null)
                    {
                        title = GetResource(title, displayAttribute.ResourceType);
                        description = GetResource(description, displayAttribute.ResourceType);
                        group = GetResource(group, displayAttribute.ResourceType);
                    }
                    #endregion

                    #region Email Type
                    if (emailAttribute != null)
                    {
                        type = "email";
                        typeMessage = emailAttribute.ErrorMessage;

                        if (emailAttribute.ErrorMessageResourceType != null)
                        {
                            typeMessage = GetResource(emailAttribute.ErrorMessageResourceName, emailAttribute.ErrorMessageResourceType);
                            typeMessage = typeMessage == null || typeMessage == emailAttribute.ErrorMessageResourceName ? "" : string.Format(typeMessage, title);
                        }
                    }
                    #endregion

                    #region RegEx
                    if (regexAttribute != null)
                    {
                        regex = regexAttribute.Pattern;
                        regexMessage = regexAttribute.ErrorMessage;

                        if (regexAttribute.ErrorMessageResourceType != null)
                        {
                            regexMessage = GetResource(regexAttribute.ErrorMessageResourceName, regexAttribute.ErrorMessageResourceType);
                        }
                    }
                    #endregion

                    #region Required
                    if (requiredAttribute != null)
                    {
                        required = true;
                    }
                    #endregion

                    #region Length
                    if (lengthAttribute != null)
                    {
                        lengthMin = lengthAttribute.MinimumLength;
                        lengthMax = lengthAttribute.MaximumLength;

                        if (lengthAttribute.ErrorMessageResourceType != null)
                        {
                            lengthMessage = GetResource(lengthAttribute.ErrorMessageResourceName, lengthAttribute.ErrorMessageResourceType);
                            lengthMessage = lengthMessage == null || lengthMessage == lengthAttribute.ErrorMessageResourceName ? "" : string.Format(lengthMessage, title, lengthMax, lengthMin);
                        }
                    }
                    #endregion
                    #endregion

                    #region Property generator
                    if (property.PropertyType == typeof(System.String))
                    {
                        #region String
                        if (showAsListAttribute != null)
                        {
                            #region String as List
                            string placeholder = showAsListAttribute.Placeholder;

                            if (placeholder != null)
                            {
                                var placeholderTemp = placeholder;
                                placeholder = GetResource(placeholderTemp, showAsListAttribute.ResourceType);
                                placeholder = placeholder == placeholderTemp ? placeholderTemp : placeholder;
                            }

                            var s = new Select
                            {
                                Key = property.Name,
                                MappedName = property.Name,
                                Description = description,
                                DisplayOrder = displayAttribute.Order,
                                Title = title,
                                Group = displayAttribute.GroupName,
                                GroupTitle = group,
                                Required = required,
                                Placeholder = placeholder,
                                RequiredMessage = string.Format("{0} is required", title)
                            };

                            MethodInfo info = showAsListAttribute.DataSourceType.GetMethod(showAsListAttribute.DataSourceMethod);
                            List<SelectListItem> selectList = null;

                            if (info != null)
                            {
                                selectList = (List<SelectListItem>)info.Invoke(null, null);
                            }

                            if (selectList != null)
                                foreach (var item in selectList)
                                {
                                    s.Choices.Add(new ListItem()
                                    {
                                        Text = item.Text,
                                        Value = item.Value,
                                        Selected = property.GetValue(null).ToString() == item.Value
                                    });
                                }

                            form.AddFields(s);
                            #endregion
                        }
                        else
                        {
                            var val = property.GetValue(null);
                            #region String
                            var tb = new TextBox
                            {
                                Key = property.Name,
                                MappedName = property.Name,
                                Description = description,
                                DisplayOrder = displayAttribute.Order,
                                Title = title,
                                Group = displayAttribute.GroupName,
                                GroupTitle = group,
                                Required = required,
                                RequiredMessage = string.Format("{0} is required", title),
                                Value = val == null ? null : val.ToString(),
                                Type = type,
                                TypeMessage = typeMessage,
                                LengthMin = lengthMin,
                                LengthMax = lengthMax,
                                LengthMessage = lengthMessage,
                                RegularExpression = regex,
                                RegexMessage = regexMessage
                            };

                            form.AddFields(tb);
                            #endregion
                        }
                        #endregion
                    }
                    else if (property.PropertyType == typeof(System.Int16) ||
                        property.PropertyType == typeof(System.Int32) ||
                        property.PropertyType == typeof(System.Int64))
                    {
                        #region Number
                        type = "number";

                        var tb = new TextBox
                        {
                            Key = property.Name,
                            MappedName = property.Name,
                            Description = description,
                            DisplayOrder = displayAttribute.Order,
                            Title = title,
                            Group = displayAttribute.GroupName,
                            GroupTitle = group,
                            Required = required,
                            RequiredMessage = string.Format("{0} is required", title),
                            Value = property.GetValue(null).ToString(),
                            Type = type,
                            TypeMessage = typeMessage,
                            RegularExpression = regex,
                            RegexMessage = regexMessage
                        };

                        form.AddFields(tb);
                        #endregion
                    }
                    else if (property.PropertyType == typeof(System.Boolean))
                    {
                        #region Boolean
                        var cb = new CheckBox
                        {
                            Key = property.Name,
                            MappedName = property.Name,
                            Description = description,
                            DisplayOrder = displayAttribute.Order,
                            Title = title,
                            Group = displayAttribute.GroupName,
                            GroupTitle = group,
                            Required = required,
                            RequiredMessage = string.Format("{0} is required", title),
                            Checked = (bool)property.GetValue(null)
                        };

                        form.AddFields(cb);
                        #endregion
                    }
                    else if (property.PropertyType.BaseType == typeof(System.Enum))
                    {
                        #region Enum
                        var val = property.GetValue(null);

                        var s = new Select
                        {
                            Key = property.Name,
                            MappedName = property.Name,
                            Description = description,
                            DisplayOrder = displayAttribute.Order,
                            Title = title,
                            Group = displayAttribute.GroupName,
                            GroupTitle = group,
                            MultipleSelection = true,
                            Required = required,
                            RequiredMessage = string.Format("{0} is required", title),
                            Size = 5
                        };

                        foreach (var item in System.Web.Mvc.Html.EnumHelper.GetSelectList(property.PropertyType))
                        {
                            if (item.Value != "0")
                                s.Choices.Add(new ListItem()
                                {
                                    Text = item.Text,
                                    Value = item.Value,
                                    Selected = ((int)val & int.Parse(item.Value)) == int.Parse(item.Value)
                                });
                        }

                        form.AddFields(s);
                        #endregion
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Unhandled property type for form generator: {0}", property.PropertyType));
                    }
                    #endregion
                }
            }

            return form;
        }
    }
}
