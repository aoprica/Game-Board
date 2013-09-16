using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Game.Network;

namespace Game.Server
{
    public abstract class ServerBase
    {
        // privates
        private ConnectionListener listener;
        private int port;


        // const
        public ServerBase(string ip, int port)
        {
            this.port = port;

            this.listener = new ConnectionListener(ip, port);

            this.listener.ConnectionCreated += new ConnectionEvent(listener_ConnectionCreated);
        }


        // methods
        public void Start()
        {
            this.listener.Start();
        }
        public void Stop()
        {
            this.listener.Stop();
        }


        private void listener_ConnectionCreated(object sender, Connection connection)
        {
            connection.ConnectionClosed += new ConnectionEvent(connection_ConnectionClosed);

            OnConnectionCreated(connection);
        }
        private void connection_ConnectionClosed(object sender, Connection connection)
        {
            OnConnectionClosed(connection);

            connection.ConnectionClosed -= this.connection_ConnectionClosed;
        }


        protected abstract void OnConnectionCreated(Connection connection);
        protected abstract void OnConnectionClosed(Connection connection);
    }
}

