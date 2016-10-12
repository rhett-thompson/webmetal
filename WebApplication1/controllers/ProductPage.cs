using System;
using LiteDB;
using WebApplication1.core;
using WebApplication1.models;
using webmetal;

namespace WebApplication1.controllers
{
    public class ProductPage : Page
    {
        public override void index()
        {
            throw new NotImplementedException();
        }

        [CustomRoute("{slug}")]
        public void single(string slug)
        {

            using (var db = Data.catalog())
            {
                Product product = Data.products(db).FindById(slug);
            }

        }

        [CustomRoute("{slug}/edit")]
        public void edit(string slug)
        {
            using (var db = Data.catalog())
            {
                Product product = Data.products(db).FindById(slug);
            }
        }

        [CustomRoute("{slug}/delete")]
        public void delete(string slug)
        {

        }

        [CustomRoute("{slug}/save")]
        public void save([FormFieldsModel]Product product)
        {

        }

        public void list()
        {

        }

    }
}