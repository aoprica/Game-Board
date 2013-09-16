using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Game.Network;


namespace Game.Client
{
    public abstract class ClientBase
    {
        // privates
        private Connection connection;


        // proprties 
        protected Connection Connection { get { return this.connection; } }


        // methods
        public void Send(string content)
        {
            this.connection.Send(content);
        }
        public void Send(byte[] content)
        {
            this.connection.Send(content);
        }
        public void Connect(string ip, int port)
        {
            this.connection = new Connection();

            this.connection.DataReceived += new ConnectionDataReceivedEvent(connection_DataReceived);
            this.connection.ConnectionCreated += new ConnectionEvent(connection_ConnectionCreated);
            this.connection.ConnectionClosed += new ConnectionEvent(connection_ConnectionClosed);

            this.connection.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        }
        public void Disconnect()
        {
            this.connection.Disconnect();
        }
        

        private void connection_DataReceived(object sender, byte[] data)
        {
            OnDataReceived(data);
        }
        private void connection_ConnectionCreated(object sender, Connection connection)
        {
            OnConnectionCreated(connection);
        }
        private void connection_ConnectionClosed(object sender, Connection connection)
        {
            OnConnectionClosed(connection);
        }


        protected abstract void OnDataReceived(byte[] data);
        protected abstract void OnConnectionCreated(Connection connection);
        protected abstract void OnConnectionClosed(Connection connection);
    }
}
