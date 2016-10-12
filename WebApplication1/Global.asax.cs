using System;
using System.IO;
using Newtonsoft.Json;
using WebApplication1.controllers;
using WebApplication1.models;
using webmetal;

namespace WebApplication1
{
    public class Global : WebMetalApplication
    {

        public static Settings settings;

        protected void Application_Start(object sender, EventArgs e)
        {

            settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Server.MapPath("~/settings.json")));

            init(typeof(HomePage), new string[] { Server.MapPath(settings.templates_directory) });
            
        }
        
    }
}