using System;
using webmetal;

namespace WebApplication1
{
    public class Home : Page
    {
        public override void render()
        {
            response.Write(view("home"));
        }
    }
}