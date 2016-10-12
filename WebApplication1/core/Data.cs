using LiteDB;
using WebApplication1.models;

namespace WebApplication1.core
{
    public class Data
    {

        public static LiteDatabase catalog()
        {
            return new LiteDatabase(Global.settings.calatog_file);
        }

        public static LiteCollection<Product> products(LiteDatabase db)
        {
            return db.GetCollection<Product>(Global.settings.product_collection);
        }

    }
}