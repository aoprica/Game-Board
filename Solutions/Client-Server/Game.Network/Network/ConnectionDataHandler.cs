using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Network;
using System.IO;

namespace Game.Network
{
    public class ConnectionDataHandler
    {
        // methods
        public void Handle(ConnectionDataContext context)
        {
            if (ConnectionDataType.Response == context.Type)
            {
                // process received request
                var responsePacket = Serializer.Deserialize<ResponsePacket>(context.Content);
                var response = new Response(responsePacket);

                response.Process(context.Connection);
            }
            else if (ConnectionDataType.Request == context.Type)
            {
                // process received response
                var requestPacket = Serializer.Deserialize<RequestPacket>(context.Content);
                var request = new Request(requestPacket);

                request.Process(context.Connection);
            }
        }
    }
}
