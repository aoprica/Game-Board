using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public abstract class RequestHandler
    {
        public Response Response { get; protected set; }


        public virtual void Handle(Request request)
        {
            this.Response = Response.CreateFrom(request);

            OnHandle(request);
        }
        public Response GetResponse()
        {
            return this.Response;
        }


        public abstract void OnHandle(Request request);
    }
}
