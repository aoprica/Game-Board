using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public class ConnectionDataHandlerFactory
    {
        private static ConnectionDataHandler __instance = new ConnectionDataHandler();
        public static ConnectionDataHandler GetInstance()
        {
            return __instance;
        }
    }
}
