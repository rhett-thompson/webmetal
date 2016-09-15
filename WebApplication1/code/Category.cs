using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webmetal;

namespace WebApplication1.code
{
    
    public class Category : Page
    {

        [CustomRoute("{a}/asd/{b}")]
        public void test(int a, int b)
        {

        }

        public override void index()
        {
            //throw new NotImplementedException();
        }
    }
}