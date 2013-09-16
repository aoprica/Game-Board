using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace Game.Network
{
    public class ConnectionListener
    {
        // events
        public event ConnectionEvent ConnectionCreated;


        // private vars
        private TcpListener listener;


        // const
        public ConnectionListener(string ip, int port)
        {
            this.listener = new TcpListener(IPAddress.Parse(ip), port);
        }


        // methods
        public void Start()
        {
           this.listener.Start();

           BeginAcceptConnection(this.listener);
        }
        public void Stop()
        {
            this.listener.Stop();
        }


        private void BeginAcceptConnection(TcpListener listener)
        {
            try
            {
                listener.BeginAcceptTcpClient(DoAcceptConnectionCallback, listener);
            }
            catch (SocketException ex)
            {
                // An error occurred while attempting to access the socket. 
                throw ex;
            }
            catch (ObjectDisposedException ex)
            {
                // The Socket has been closed.
                throw ex;
            }
        }
        private void DoAcceptConnectionCallback(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;
            var tcpClient = (TcpClient)null;

            tcpClient = listener.EndAcceptTcpClient(ar);

            BeginAcceptConnection(listener);

            if (this.ConnectionCreated != null)
                this.ConnectionCreated(this, new Connection(tcpClient));
        }
    }
}
