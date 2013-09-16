using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Network;
using Game.Client;


namespace Simple_Client
{
    public class GameClient : ClientBase
    {
        // events
        public event ServerResponseEvent ServerResponseReceived;
        public event ClientStatusEvent Disconnected;


        // privates
        private string ip;
        private int port;


        // constructors
        public GameClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }


        // properties
        public string UserID { get; set; } 


        // methods
        public void Connect()
        {
            this.Connect(ip, port);
        }


        protected override void OnDataReceived(byte[] data)
        {
            var context = new ConnectionDataContext(this.Connection, data);
            var handler = new ConnectionDataHandler();

            handler.Handle(context);
        }
        protected override void OnConnectionCreated(Connection connection)
        {
            if (!string.IsNullOrEmpty(this.UserID))
            {
                var request = new Request();
                request.Handle = "UserJoinRequest";
                request.AddParam("uid", this.UserID);

                request.Send(connection, userJoinResponse_Callback);
            }
        }
        protected override void OnConnectionClosed(Connection connection)
        {
            if (this.Disconnected != null)
                this.Disconnected(this);
        }


        private void userJoinResponse_Callback(Response response)
        {
            if (this.ServerResponseReceived != null)
                this.ServerResponseReceived(this, response);
        }
    }
}
