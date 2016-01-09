using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Routing;

namespace HtmlHelpers
{
    public static class HtmlHelpers
    {        
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
    }
}