using System;
using System.Linq.Expressions;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace webmetal
{

    public abstract class WebMetalTemplate<T> : TemplateBase<T>
    {
        
        public IEncodedString InputFor<ModelT, FieldT>(Expression<Func<ModelT, FieldT>> propertyToSelect, string type = null, object attributes = null)
        {
            return Raw(string.Format("<input type='{0}' value='{1}' name='{2}' {3}>", type, "", "", attributes));
        }

    }

}