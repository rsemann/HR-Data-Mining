using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace MineradorRH.Extensions
{
    public static class SearchHtmlHelper
    {
        public static void Search<TModel>(this HtmlHelper<TModel> htmlHelper, string actionName, string controllerName, string label)
        {
            htmlHelper.BeginForm(actionName, controllerName);
            htmlHelper.ViewContext.Writer.Write("<div style='margin-bottom: 20px'>" +

                                        "    <label style='float: left'>Digite o " + label + ":</label>" +
                                        "    <div class='col-md-4'>" +
                                                htmlHelper.TextBox("SearchString", null, new { @class = "form-control" }) +
                                        "    </div>" +
                                        "    <input type='submit' value='Procurar' class='btn btn-info' />" +
                                        "</div>"
                                        );
            htmlHelper.BeginForm(actionName, controllerName).EndForm();
        }

        public static MvcHtmlString MyEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Object htmlAttributes)
        {
            string value = html.EditorFor(expression).ToString();

            PropertyInfo[] properties = htmlAttributes.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                int index = value.ToLower().IndexOf(info.Name.ToLower() + "=");
                if (index < 0)
                    value = value.Insert(value.Length - (value.EndsWith("/>") ? 2 : 1), info.Name.ToLower() + "=\"" + info.GetValue(htmlAttributes, null) + "\"");
                else
                    value = value.Insert(index + info.Name.Length + 2, info.GetValue(htmlAttributes, null) + " ");
            }

            return MvcHtmlString.Create(value);
        }
    }
}