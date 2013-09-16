using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public class Response
    {
        public static Response CreateFrom(Request request)
        {
            var ret = new Response();
            ret.RequestID = request.RequestID;
            return ret;
        }


        protected Response() { }
        public Response(ResponsePacket response)
        {
            this.RequestID = response.RequestID;
            this.Content = response.Content;
        }


        public Guid RequestID { get; protected set; }
        public string Content { get; set; }


        public ResponsePacket GetPacket()
        {
            var packet = new ResponsePacket();
            packet.RequestID = this.RequestID;
            packet.Content = this.Content;

            return packet;
        }
    }


    public class ResponsePacket
    {
        public Guid RequestID;
        public string Content;
    }
}
