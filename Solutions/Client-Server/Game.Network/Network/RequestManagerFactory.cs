using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Network
{
    public class RequestManagerFactory
    {
        private readonly static RequestManager _instance = new RequestManager();
        public static RequestManager GetInstance()
        {
            return _instance;
        }
    }
}
