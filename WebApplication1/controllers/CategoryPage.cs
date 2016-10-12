using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webmetal;

namespace WebApplication1.controllers
{
    
    public class CategoryPage : Page
    {

        [CustomRoute("{a}/asd/{b}")]
        public string test(int a, int b)
        {
            return "asd";
        }

        public override void index()
        {
            throw new NotImplementedException();
        }
    }
}