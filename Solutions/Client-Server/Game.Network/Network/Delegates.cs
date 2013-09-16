using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public delegate void ConnectionEvent(object sender, Connection connection);

    public delegate void ConnectionDataReceivedEvent(object sender, byte[] data);

    public delegate void ConnectionDataSentEvent(object sender, bool hasErrors);
}
