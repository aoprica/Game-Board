using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Game.Network
{
    public class ConnectionDataContext
    {
        // constructors
        public ConnectionDataContext(Connection connection, byte[] raw)
        {
            this.Connection = connection;
            this.Raw = raw;

            var str = ASCIIEncoding.Unicode.GetString(this.Raw);
            var reader = new StringReader(str.TrimEnd('\0'));

            var str2 = reader.ReadLine();

            if ("REQ" == str2)
                this.Type = ConnectionDataType.Request;
            else if ("OK" == str2 || "BAD" == str2)
                this.Type = ConnectionDataType.Response;
            else
                this.Type = ConnectionDataType.Unknown;

            str2 = reader.ReadToEnd();

            this.Content = str2;
        }


        // properties
        public Connection Connection { get; protected set; }
        public string Content { get; protected set; }
        public byte[] Raw { get; protected set; }
        public ConnectionDataType Type { get; protected set; }
    }

    public enum ConnectionDataType
    {
        Response, Request, Unknown
    }
}
