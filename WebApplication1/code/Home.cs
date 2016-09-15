using System;
using webmetal;

namespace WebApplication1
{

    public class Home : Page
    {
        
        public override void index()
        {
            response.Write(view("home"));
        }
    }
}