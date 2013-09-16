using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Game.Network
{
    /// <summary>
    /// Clients that connects to the server.
    /// </summary>
    public class Connection
    {
        // events
        public event ConnectionEvent ConnectionClosed;
        public event ConnectionEvent ConnectionCreated;
        public event ConnectionDataReceivedEvent DataReceived;
        public event ConnectionDataSentEvent DataSent;


        // private vars
        private TcpClient tcp;
        private byte[] readBuffer;
        private readonly int bufferSize = 2048;


        // const
        public Connection()
        {
            this.ID = Guid.NewGuid();
            this.tcp = new TcpClient();

            this.readBuffer = new byte[bufferSize];
        }
        public Connection(TcpClient tcpClient)
        {
            this.ID = Guid.NewGuid();
            this.tcp = tcpClient;

            this.readBuffer = new byte[bufferSize];

            
            // already connected; start listening immediately.
            if (this.tcp.Connected)
            {
                this.BeginReadData(this.tcp);
            }
        }


        // properties
        public bool Connected { get; protected set; }
        public Guid ID { get; private set; }
        

        // methods
        public void Connect(string ip, int port)
        {
            Connect(new IPEndPoint(IPAddress.Parse(ip), port));
        }
        public void Connect(IPAddress ip, int port)
        {
            Connect(new IPEndPoint(ip, port));
        }
        public void Connect(IPEndPoint remoteEndPoint)
        {
            if (!tcp.Connected)
            {
                BeginConnect(this.tcp, remoteEndPoint);
            }
            else
            {
                Disconnect(); // disconnect first

                BeginConnect(this.tcp, remoteEndPoint);
            }
        }
        
        public void Disconnect()
        {
            /* :IMPORTANT: Depending on the state of socket connected-ness and whether error, the network stream may not be available.
             * Will throw errors if not handled properly.
             */ 
            if (tcp.Connected)
                tcp.GetStream().Close();  // close network stream

             tcp.Close();              // close client

            if (ConnectionClosed != null)
                ConnectionClosed(this, this);
        }
        
        public void Send(string data)
        {
            Send(data, ASCIIEncoding.Unicode);
        }
        public void Send(string data, Encoding encoding)
        {
            Send(encoding.GetBytes(data));
        }
        public void Send(byte[] data)
        {
            BeginSendData(this.tcp, data);
        }


        private void BeginConnect(TcpClient client, IPEndPoint remoteEndPoint)
        {
            try
            {
                client.BeginConnect(remoteEndPoint.Address, remoteEndPoint.Port, DoBeginConnectCallback, client);
            }
            catch (SocketException ex)
            {
                /*  An error occurred when attempting to access the socket.
                 */
                Log.Error(ex);  // throw ex;
            }
            catch(ObjectDisposedException ex)
            {
                /*  The Socket has been closed. 
                 */
                Log.Error(ex);  // throw ex;
            }
        }
        private void DoBeginConnectCallback(IAsyncResult ar)
        {
            var client = (TcpClient)ar.AsyncState;

            try
            {
                client.EndConnect(ar);
            }
            catch (SocketException ex)
            {
                /*  An error occurred when attempting to access the socket.
                 */
                Log.Error(ex);  // throw ex;
            }
            catch (ObjectDisposedException ex)
            {
                /*  The Socket has been closed. 
                 */
                Log.Error(ex);  // throw ex;
            }


            BeginReadData(client);

            if (ConnectionCreated != null)
                ConnectionCreated(this, this);
        }

        private void BeginSendData(TcpClient client, byte[] data)
        {
            if (client.Connected)
            {
                BeginSendData(client.GetStream(), data);
            }
        }
        private void BeginSendData(NetworkStream stream, byte[] data)
        {
            if (stream.CanWrite)
            {
                try
                {
                    stream.BeginWrite(data, 0, data.Length, DoBeginSendDataCallback, stream);
                }
                catch (IOException ex)
                {
                    /*  The underlying Socket is closed.
                     *  -or-
                     *  There was a failure while reading from the network.
                     *  -or-
                     *  An error occurred when accessing the socket.
                     */
                    Log.Error(ex);  // throw ex;
                }
                catch (ObjectDisposedException ex)
                {
                    /*  The NetworkStream is closed.
                     */
                    Log.Error(ex);  // throw ex;
                }
            }
        }
        private void DoBeginSendDataCallback(IAsyncResult ar)
        {
            var stream = (NetworkStream)ar.AsyncState;

            if (stream.CanWrite)
            {
                try
                {
                    stream.EndWrite(ar);
                }
                catch (IOException ex)
                {
                    /*  The underlying Socket is closed.
                     *  -or-
                     *  An error occurred while writing to the network.
                     *  -or-
                     *  An error occurred when accessing the socket.
                     */
                    Log.Error(ex);  // throw ex;
                }
                catch (ObjectDisposedException ex)
                {
                    /*  The NetworkStream is closed.
                     */
                    Log.Error(ex);  // throw ex;
                }

                if (DataSent != null)
                    DataSent(this, true);
            }
        }

        private void BeginReadData(TcpClient tcp)
        {
            if (tcp.Connected)
            {
                var ns = tcp.GetStream();

                BeginReadData(ns);
            }
        }
        private void BeginReadData(NetworkStream stream) 
        {
            if (stream.CanRead)
            {
                try
                {
                    stream.BeginRead(this.readBuffer, 0, bufferSize, DoBeginReadDataCallback, stream);
                }
                catch (IOException ex)
                {
                    /*  The underlying Socket is closed.
                     *  -or-
                     *  There was a failure while reading from the network.
                     *  -or-
                     *  An error occurred when accessing the socket.
                     */
                    Log.Error(ex);  // throw ex;
                }
                catch (ObjectDisposedException ex)
                {
                    /*  The NetworkStream is closed.
                     */
                    Log.Error(ex);  // throw ex;
                }
            }
        }
        private void DoBeginReadDataCallback(IAsyncResult ar)
        {
            var stream = (NetworkStream)ar.AsyncState;

            if (!stream.CanRead)
            {
                // stream no longer readable; might've been closed after read started.
                Log.Info("Disconnected");
            }
            else
            {
                int numBytesReceived = 0;

                try
                {
                    numBytesReceived = stream.EndRead(ar);
                }
                catch (IOException ex)
                {
                    Disconnect();

                    /*  The underlying Socket is closed.
                     *  -or-
                     *  An error occurred when accessing the socket.
                     */
                    Log.Error(ex);  // throw ex;
                }
                catch (ObjectDisposedException ex)
                {
                    /*  The NetworkStream is closed.
                     */
                    Log.Error(ex);  // throw ex;
                }

                if (numBytesReceived <= 0)
                {
                    /* if the remote host shuts down the Socket connection and all available data has been received, 
                     * the EndRead method completes immediately and returns zero bytes.
                     */
                    Disconnect();
                }
                else
                {
                    BeginReadData(stream);

                    if (DataReceived != null)
                        DataReceived(this, this.readBuffer);
                }
            }
        } 
    }
}
