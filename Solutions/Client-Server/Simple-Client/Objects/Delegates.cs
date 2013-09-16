using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Network;

namespace Simple_Client
{
    public delegate void ServerResponseEvent(object sender, Response response);
    public delegate void ClientStatusEvent(object sender);
}
