using System;
using System.IO;
using System.Text;

namespace webmetal
{

    public abstract class MimeTypeHandler
    {
        
        public abstract byte[] serialize(object response, Type type);
        public abstract object deserialize(Stream inputStream, Type type);

    }
    
    public class TextPlainMimeTypeHandler : MimeTypeHandler
    {
        
        public override object deserialize(Stream inputStream, Type type)
        {
            using (var reader = new StreamReader(inputStream))
                return reader.ReadToEnd();
        }

        public override byte[] serialize(object response, Type type)
        {

            return Encoding.UTF8.GetBytes(response.ToString());

        }

    }

}
