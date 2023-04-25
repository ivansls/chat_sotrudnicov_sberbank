using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chat_chat
{
    /// <summary>
    /// Логика взаимодействия для windwow.xaml
    /// </summary>
    public partial class windwow : Window
    {
        private Socket socket;

        private List<Socket> clients = new List<Socket>();
        public windwow()
        {
            InitializeComponent();
            if (MainWindow.IsServer == true)
            {

                IPEndPoint ip_point = new IPEndPoint(IPAddress.Any, 8888);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ip_point);
                socket.Listen(1000);
                ListenClients();
            }
            else
            {
                log_chat.Visibility = Visibility.Hidden;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ConnectAsync("127.0.0.1", 8888);
            }

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }




        private async Task ListenClients()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                clients.Add(client);
                RecieveMessage(client);
            }
        }

        private async Task RecieveMessage(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                list_message.Items.Add($"[Весточка от {client.RemoteEndPoint}]: {message}");

                if (MainWindow.IsServer == true)
                {
                    
                    foreach (var item in clients)
                    {
                        SendMessage(item, message);
                    }
                }
                else
                {
                    list_message.Items.Add(message);
                }
            }

        }

        private async Task SendMessage_1(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(bytes, SocketFlags.None);
        }

        private async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }

        private void message_Click(object sender, RoutedEventArgs e)
        {
            SendMessage_1(vestohka.Text);
        }
    }
}
