using System;
using webmetal;

namespace WebApplication1.controllers
{

    public class HomePage : Page
    {

        public override void index()
        {
            response.Write(view("home"));
        }

    }
}