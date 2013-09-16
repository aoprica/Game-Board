using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public class Serializer
    {
        // statics
        public static T Deserialize<T>(string str)
        {
            return fastJSON.JSON.Instance.ToObject<T>(str);
        }
        public static string Serialize<T>(T @object)
        {
            return fastJSON.JSON.Instance.ToJSON(@object);
        }
        public static byte[] SerializeToBytes<T>(T @object)
        {
            return ASCIIEncoding.Unicode.GetBytes(Serialize<T>(@object));
        }
    }
}
