#ASP.NET Helpers

##Radio Button Toggle Helper Extension
###Dependecy:
* Bootstrap CSS

###Usage:
```razor
@* Razor *@

@* Simple *@
@Html.RadioButtonToggleFor(Model => Model.YesOrNo)

@* Custom *@
@{
    var toggleButtons = new
    {
        @class = "btn-default",
        labelTrue = "",
        labelFalse = ""
    };
}
@Html.RadioButtonToggleFor(Model => Model.YesOrNo, new { @id = "test", @class = "my-class", toggleButtons = toggleButtons })
```

###HTML Markup:
```html
<div class="radio-button-toggle btn-group" data-toggle="buttons" id="test">
    <label class="btn btn-default radio-button-toggle-true active">
        <input id="YesOrNoTrue" name="YesOrNo" type="radio" value="True">
    </label>
    <label class="btn btn-default radio-button-toggle-false">
        <input checked="checked" id="YesOrNoFalse" name="YesOrNo" type="radio" value="False">
    </label>
</div>
```

###CSS:
```css
.radio-button-toggle.btn-group .radio-button-toggle-true.active {
    background-color: #3276b1 !important;
    color: white;
    cursor: default;
}

.radio-button-toggle.btn-group .radio-button-toggle-false.active {
    background-color: #d7d7d7 !important;
    color: white;
    cursor: default;
}

.radio-button-toggle.btn-group .radio-button-toggle-true,
.radio-button-toggle.btn-group .radio-button-toggle-false {
    color: transparent;
}

    .radio-button-toggle.btn-group .radio-button-toggle-true:not(.active):hover {
        /* uncomment background color to display text on hover */
        /*color: white;*/
    }

    .radio-button-toggle.btn-group .radio-button-toggle-false:not(.active):hover {
        /* uncomment background color to display text on hover */
        /*color: black;*/
    }
```

###Extension Methods and Variables:
```csharp
private static object _radioButtonToggleDivDefaultAttrs = new
{
    @class = "radio-button-toggle btn-group",
    data_toggle = "buttons"
};

private static object _radioButtonToggleDefaultAttrs = new
{
    @class = "btn"
};

private static object _radioButtonToggleDefaultLabel = new
{
    labelTrue = "Yes",
    labelFalse = "No"
};

public static MvcHtmlString RadioButtonToggleFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
{
    var template = @"<div class='radio-button-toggle btn-group' data-toggle='buttons'>
        <label class='btn btn-default radio-button-toggle-true{0}'>
            {1}Yes
        </label>
        <label class='btn btn-default radio-button-toggle-false{2}'>
            {3}No
        </label>
    </div>";

    var data = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

    Func<TModel, TProperty> method = expression.Compile();
    bool value = Convert.ToBoolean(method(helper.ViewData.Model));

    string radioTrueActive = value ? " active" : "";
    string radioFalseActive = !value ? " active" : "";
    MvcHtmlString radioTrue = helper.RadioButton(data.PropertyName, true, value);
    MvcHtmlString radioFalse = helper.RadioButton(data.PropertyName, false, !value);

    var result = string.Format(template, radioTrueActive, radioTrue, radioFalseActive, radioFalse);

    return new MvcHtmlString(result);
}

public static MvcHtmlString RadioButtonToggleFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
{
    var data = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

    Func<TModel, TProperty> method = expression.Compile();
    bool value = Convert.ToBoolean(method(helper.ViewData.Model));

    IDictionary<string, object> attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
    IDictionary<string, object> defaultDivAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(_radioButtonToggleDivDefaultAttrs);
    IDictionary<string, object> buttonLabel = HtmlHelper.AnonymousObjectToHtmlAttributes(_radioButtonToggleDefaultLabel);
    IDictionary<string, object> labelTrueAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(_radioButtonToggleDefaultAttrs);
    IDictionary<string, object> labelFalseAttrs = HtmlHelper.AnonymousObjectToHtmlAttributes(_radioButtonToggleDefaultAttrs);

    IDictionary<string, object> toggleButtons = HtmlHelper.AnonymousObjectToHtmlAttributes(attrs["toggleButtons"]);
    attrs.Remove("toggleButtons");
    buttonLabel["labelTrue"] = toggleButtons["labelTrue"] ?? buttonLabel["labelTrue"];
    buttonLabel["labelFalse"] = toggleButtons["labelFalse"] ?? buttonLabel["labelFalse"];
    labelTrueAttrs["class"] = labelTrueAttrs["class"] + (toggleButtons.Keys.Contains("class") ? " " + toggleButtons["class"] : "");
    labelFalseAttrs["class"] = labelFalseAttrs["class"] + (toggleButtons.Keys.Contains("class") ? " " + toggleButtons["class"] : "");

    attrs["class"] = defaultDivAttrs["class"] + (attrs.Keys.Contains("class") ? " " + attrs["class"] : "");
    attrs["data-toggle"] = defaultDivAttrs["data-toggle"] + (attrs.Keys.Contains("data-toggle") ? " " + attrs["data-toggle"] : "");

    string radioTrueActive = (value ? " active" : "") + " radio-button-toggle-true";
    string radioFalseActive = (!value ? " active" : "") + " radio-button-toggle-false";

    MvcHtmlString radioTrue = helper.RadioButton(data.PropertyName, true, value,
        new { id = data.PropertyName + (string.IsNullOrEmpty(Convert.ToString(buttonLabel["labelTrue"])) ? "True" : buttonLabel["labelTrue"]) });
    MvcHtmlString radioFalse = helper.RadioButton(data.PropertyName, false, !value,
        new { id = data.PropertyName + (string.IsNullOrEmpty(Convert.ToString(buttonLabel["labelFalse"])) ? "False" : buttonLabel["labelFalse"]) });

    TagBuilder div = new TagBuilder("div");
    div.MergeAttributes(attrs);

    TagBuilder labelTrue = new TagBuilder("label");

    labelTrueAttrs["class"] += radioTrueActive;
    labelTrue.MergeAttributes(labelTrueAttrs);
    labelTrue.InnerHtml += radioTrue;
    labelTrue.InnerHtml += buttonLabel["labelTrue"];

    TagBuilder labelFalse = new TagBuilder("label");

    labelFalseAttrs["class"] += radioFalseActive;
    labelFalse.MergeAttributes(labelFalseAttrs);
    labelFalse.InnerHtml += radioFalse;
    labelFalse.InnerHtml += buttonLabel["labelFalse"];

    div.InnerHtml += labelTrue;
    div.InnerHtml += labelFalse;

    return new MvcHtmlString(div.ToString(TagRenderMode.Normal));
}
```