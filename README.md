# DynamicFormsEngine
Dynamic forms engine for .NET
Generates bootstrap forms dynamically from a given class.

## Getting Started

Get DynamicFormsEngine package from NuGet
```
Install-Package DynamicFormsEngine
```

In your controller action, generate a new **Form** object and pass the model class type.
```
Form form = new Form();

form = FormGeneratorUtility.GetForm(typeof(ModelClass));
form.Serialize = true;

return View(form);
```

In your view, set your model to **DynamicFormsEngine.Form** (*@model DynamicFormsEngine.Form*) and call HTML renderer with *@Html.Raw(Model.RenderHtml())*

