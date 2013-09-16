using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Network;

namespace Game.Network
{
    public delegate void ReceivedResponseCallback(Response response);

    public class Request
    {
        // statics 
        public static void BindResponseCallback(Request request, ReceivedResponseCallback callback)
        {
            request.responseCallback = callback;
        }


        private ReceivedResponseCallback responseCallback;
        private Response receivedResponse;
        private bool completed;


        // constructors
        public Request()
        {
            this.RequestID = Guid.NewGuid();
            this.Params = new List<RequestParam>();
        }
        public Request(RequestPacket packet)
        {
            this.RequestID = packet.RequestID;
            this.Handle = packet.Handle;
            this.Params = packet.Params;
        }


        public Guid RequestID { get; protected set; }
        public string Handle { get; set; }
        public List<RequestParam> Params { get; protected set; }
        public bool IsCompleted { get { return this.completed; } }


        // methods
        public string GetParamValue(string name)
        {
            foreach (var n in this.Params)
            {
                if (n.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return n.Value;
            }
            return string.Empty;
        }
        /// <summary>
        /// Completes the request
        /// </summary>
        /// <param name="response">Received response</param>
        public void SetResponse(Response response)
        {
            this.receivedResponse = response;

            if (this.responseCallback != null)
                this.responseCallback(response);
        }
        public void MarkComplete()
        {
            this.completed = true;
        }


        public RequestPacket GetPacket()
        {
            var packet = new RequestPacket();
            packet.RequestID = this.RequestID;
            packet.Handle = this.Handle;
            packet.Params = this.Params;

            return packet;
        }
    }


    public class RequestParam
    {
        public string Name;
        public string Value;
    }


    public class RequestPacket
    {
        public Guid RequestID;
        public string Handle;
        public List<RequestParam> Params;
    }
}
