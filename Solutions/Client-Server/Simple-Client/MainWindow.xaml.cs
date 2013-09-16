using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Game.Network;

namespace Simple_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool connected = false;
        private GameClient gameClient;

        public MainWindow()
        {
            InitializeComponent();
        }


        private void JoinGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.connected)
            {
                gameClient = new GameClient(NetUtil.GetIP().ToString(), 1337);
                gameClient.ServerResponseReceived += new ServerResponseEvent(gameClient_ServerResponseReceived);
                gameClient.Disconnected += new ClientStatusEvent(gameClient_Disconnected);

                gameClient.UserID = PlayerNameTextField.Text;
                gameClient.Connect();

                this.ServerResponseText.Text = "Connecting...";
                this.JoinGameButton.IsEnabled = false;
            }
            else
            {
                gameClient.Disconnect();

                this.ServerResponseText.Text = "Disconnecting...";
            }
        }

        private void gameClient_Disconnected(object sender)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ServerResponseText.Text = "You are not connected.";

                this.JoinGameButton.Content = "Join Game";
                this.JoinGameButton.IsEnabled = true;

                this.PlayerNameTextField.IsEnabled = true;

                this.connected = false;

            }), System.Windows.Threading.DispatcherPriority.Render);
        }
        private void gameClient_ServerResponseReceived(object sender, Response response)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ServerResponseText.Text = response.Content;

                this.JoinGameButton.Content = "Disconnect";
                this.JoinGameButton.IsEnabled = true;

                this.PlayerNameTextField.IsEnabled = false;

                this.connected = true;

            }), System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}
