using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Game.Network;
using SimpleServer.Handlers;
using Game.Server;


namespace SimpleServer
{
    public class GameServer : ServerBase
    {
        private ConnectionDataHandler connectionDataHandler;


        // constructors
        public GameServer(string ip, int port) : base(ip, port)
        {
            // register server request handlers
            RequestHandlerFactory.Register<UserJoinRequest>();

            
            this.connectionDataHandler = new ConnectionDataHandler();
        }

        
        // methods
        protected override void OnConnectionCreated(Connection connection)
        {
            Console.WriteLine("Connection Created");

            connection.DataReceived += new ConnectionDataReceivedEvent(connection_DataReceived);
        }
        protected override void OnConnectionClosed(Connection connection)
        {
            Console.WriteLine("Connection Closed");

            connection.DataReceived -= this.connection_DataReceived;
        }

        
        private void connection_DataReceived(object sender, byte[] data)
        {
            Console.WriteLine("Data Received");

            var context = new ConnectionDataContext((Connection)sender, data);

            this.connectionDataHandler.Handle(context);
        }
    }
}
