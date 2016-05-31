using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace webmetal
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class HtmlTableColumn : Attribute
    {

        internal string columnHeading;
        internal string tableID;

        public HtmlTableColumn(string columnHeading = null, string tableID = null)
        {

            this.columnHeading = columnHeading;
            this.tableID = tableID;

        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class Format : Attribute
    {
        
        internal string format;

        public Format(string format)
        {

            this.format = format;

        }

    }

    public abstract class WebMetalTemplate<T> : TemplateBase<T>
    {

        private static string getAttributes(object attributes)
        {
            List<string> attr = new List<string>();
            foreach (PropertyInfo prop in attributes.GetType().GetProperties())
            {
                string v = prop.GetValue(attributes).ToString();
                if (string.IsNullOrEmpty(v))
                    attr.Add(prop.Name);
                else
                    attr.Add(string.Format("{0}='{1}'", prop.Name, v));
            }

            return string.Join(" ", attr);

        }

        private static PropertyInfo getProp<ModelT, FieldT>(ModelT model, Expression<Func<ModelT, FieldT>> propertyToBindTo)
        {
            MemberExpression expression = (MemberExpression)propertyToBindTo.Body;
            return (PropertyInfo)expression.Member;
        }

        private static object getPropValue(PropertyInfo prop, object model)
        {

            Format formatAttribute = prop.GetCustomAttribute<Format>();
            if (formatAttribute != null)
                return string.Format("{0:" + formatAttribute.format + "}", prop.GetValue(model));
            else
                return prop.GetValue(model);

        }

        public IEncodedString InputFor<ModelT, FieldT>(ModelT model, Expression<Func<ModelT, FieldT>> propertyToBindTo, object attributes = null)
        {

            PropertyInfo prop = getProp(model, propertyToBindTo);

            return Raw(string.Format("<input value='{0}' name='{1}' {2}>",
                prop.GetValue(model),
                prop.Name,
                getAttributes(attributes)
                ));

        }

        public IEncodedString TextAreaFor<ModelT, FieldT>(ModelT model, Expression<Func<ModelT, FieldT>> propertyToBindTo, object attributes = null)
        {

            PropertyInfo prop = getProp(model, propertyToBindTo);

            return Raw(string.Format("<textarea name='{0}' {1}>{2}</textarea>",
                prop.Name,
                getAttributes(attributes),
                prop.GetValue(model)));

        }

        public IEncodedString SelectFor<ModelT, FieldT>(ModelT model, Expression<Func<ModelT, FieldT>> propertyToBindTo, IEnumerable<KeyValuePair<string, FieldT>> options, object attributes = null)
        {

            PropertyInfo prop = getProp(model, propertyToBindTo);

            return Raw(string.Format("<select value='{0}' name='{1}' {2}>{3}</select>",
                prop.GetValue(model),
                prop.Name,
                getAttributes(attributes),
                string.Join("\r\n", options.Select(o => string.Format("<option value='{0}'>{1}</option>", o.Value, o.Key)))
                ));

        }

        public IEncodedString SelectFor<ModelT, FieldT>(ModelT model, Expression<Func<ModelT, FieldT>> propertyToBindTo, IEnumerable<FieldT> options, object attributes = null)
        {

            return SelectFor(model, propertyToBindTo, options.Select(o => new KeyValuePair<string, FieldT>(o.ToString(), o)), attributes);

        }

        public IEncodedString TableFor<ModelT>(IEnumerable<ModelT> items, string tableID = null, string caption = null, object attributes = null)
        {

            Type modelType = typeof(ModelT);

            IEnumerable<PropertyInfo> props = modelType.GetProperties().Where(p => p.GetCustomAttributes<HtmlTableColumn>().Count() > 0);
            if (props.Count() == 0)
                props = modelType.GetProperties();

            List<string> headers = new List<string>();
            foreach (PropertyInfo prop in props)
            {
                HtmlTableColumn column = prop.GetCustomAttributes<HtmlTableColumn>(true).Where(c => c.tableID == tableID).FirstOrDefault();

                if(column != null)
                    headers.Add(string.Format("<th>{0}</th>", column.columnHeading ?? prop.Name));
                else
                    headers.Add(string.Format("<th>{0}</th>", prop.Name));
            }

            List<string> rows = new List<string>();
            foreach (ModelT item in items)
            {
                List<string> columns = new List<string>();
                foreach (PropertyInfo prop in props)
                    columns.Add(string.Format("<td>{0}</td>", getPropValue(prop, item)));

                rows.Add(string.Format("<tr>{0}</tr>", string.Join("", columns)));
            }

            return Raw(string.Format("<table {0}>{1}<thead><tr>{2}</tr></thead><tbody>{3}</tbody></table>",
                getAttributes(attributes),
                caption != null ? string.Format("<caption>{0}</caption>", caption) : "",
                string.Join("", headers),
                string.Join("", rows)
              ));

        }
        
    }

}