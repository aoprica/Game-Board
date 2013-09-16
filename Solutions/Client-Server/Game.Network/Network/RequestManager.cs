using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Game.Network
{
    public class RequestManager
    {
        // privates
        private ConcurrentDictionary<Guid, ConnectionOutboundRequests> outboundRequests;
        private ConcurrentDictionary<Guid, ConnectionInboundRequests> inboundRequests;


        // constructor
        public RequestManager()
        {
            this.outboundRequests = new ConcurrentDictionary<Guid, ConnectionOutboundRequests>();
            this.inboundRequests = new ConcurrentDictionary<Guid, ConnectionInboundRequests>();
        }


        // methods
        public void SendRequest(Request request, Connection connection)
        {
            this.GetOutboundRequests(connection).Add(request);
        }
        public void ProcessResponse(Response response, Connection connection)
        {
            this.GetOutboundRequests(connection).ProcessResponse(response);
        }
        public void ProcessRequest(Request request, Connection connection)
        {
            this.GetInboundRequests(connection).Add(request);
        }


        private ConnectionOutboundRequests GetOutboundRequests(Connection connection)
        {
            if (this.outboundRequests.ContainsKey(connection.ID))
                return this.outboundRequests[connection.ID];

            var ret = new ConnectionOutboundRequests(connection);
            this.outboundRequests.TryAdd(connection.ID, ret);

            return ret;
        }
        private ConnectionInboundRequests GetInboundRequests(Connection connection)
        {
            if (this.inboundRequests.ContainsKey(connection.ID))
                return this.inboundRequests[connection.ID];

            var ret = new ConnectionInboundRequests(connection);
            this.inboundRequests.TryAdd(connection.ID, ret);

            return ret;
        }


        #region ConnectionOutboundRequests
        /// <summary>
        /// Manages outgoing requests for a connection, and the responses received
        /// </summary>
        protected class ConnectionOutboundRequests
        {
            // private vars
            private ConcurrentDictionary<Guid, Request> requests;
            private Connection connection;


            // constructors
            public ConnectionOutboundRequests(Connection connection)
            {
                this.connection = connection;
                this.requests = new ConcurrentDictionary<Guid, Request>();
            }


            // methods
            public void Add(Request request)
            {
                if (!requests.ContainsKey(request.RequestID))
                    requests.TryAdd(request.RequestID, request);

                connection.Send(request.GetSendPacket());
            }
            public void Remove(Request request)
            {
                this.requests.TryRemove(request.RequestID, out request);
            }
            public void ProcessResponse(Response response)
            {
                if (requests.ContainsKey(response.RequestID))
                {
                    // responding to target sent request
                    var req = requests[response.RequestID];

                    // do something with the response for the request
                    req.SetResponse(response);
                    req.MarkComplete();

                    // clean up
                    this.Remove(req);
                }
            }

        }
        #endregion

        #region ConnectionInboundRequests
        /// <summary>
        /// Manages incoming requests for a connection, and send out response
        /// </summary>
        protected class ConnectionInboundRequests
        {
            // private vars
            private ConcurrentDictionary<Guid, Request> requests;
            private Connection connection;


            // constructors
            public ConnectionInboundRequests(Connection connection)
            {
                this.connection = connection;
                this.requests = new ConcurrentDictionary<Guid, Request>();
            }

            
            // methods
            public void Add(Request request)
            {
                if (!requests.ContainsKey(request.RequestID))
                    requests.TryAdd(request.RequestID, request);

                // handle request
                var handler = RequestHandlerFactory.GetHandler(request.Handle);

                handler.Handle(request);

                // send response
                var response = handler.GetResponse();

                connection.Send(response.GetSendPacket());
            }
        } 
        #endregion
    }
}
