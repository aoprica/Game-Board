using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public static class RequestExtensions
    {
        public static string GetSendPacket(this Request obj)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("REQ");
            sb.Append(Serializer.Serialize<RequestPacket>(obj.GetPacket()));

            return sb.ToString();
        }
        public static void Process(this Request obj, Connection connection)
        {
            var requestManager = RequestManagerFactory.GetInstance();

            requestManager.ProcessRequest(obj, connection);
        }
        public static void AddParam(this Request obj, string name, string value)
        {
            if(string.IsNullOrEmpty(name))
                return;

            obj.Params.Add(new RequestParam() { Name = name, Value = value });
        }
        public static void Send(this Request obj, Connection connection)
        {
            Send(obj, connection, null);
        }
        public static void Send(this Request obj, Connection connection, ReceivedResponseCallback callback)
        {
            if (obj.IsCompleted)
                throw new Exception("Unable to send. This request is already completed");

            // bind the callback to handle response received
            Request.BindResponseCallback(obj, callback);

            // send request
            var manager = RequestManagerFactory.GetInstance();
            manager.SendRequest(obj, connection);
        }
    }


    public static class ResponseExtensions
    {
        public static string GetSendPacket(this Response obj)
        {
            var sb = new StringBuilder();

            sb.AppendLine("OK");
            sb.Append(Serializer.Serialize<ResponsePacket>(obj.GetPacket()));

            return sb.ToString();
        }
        public static void Process(this Response obj, Connection connection)
        {
            var requestManager = RequestManagerFactory.GetInstance();

            requestManager.ProcessResponse(obj, connection);
        }
    }
}
