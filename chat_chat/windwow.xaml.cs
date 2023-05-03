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
using System.Diagnostics;

namespace chat_chat
{
    /// <summary>
    /// Логика взаимодействия для windwow.xaml
    /// </summary>
    public partial class windwow : Window
    {
        private Socket socket;
        private Socket socket_server;
        private List<Socket> clients = new List<Socket>();
        List<string> users = new List<string>();
        MainWindow window = new MainWindow();
        string name;
        string ip;
        string user;
        string message_name;
        int c = 0;
        public windwow(string name_, string ip_)
        {
            name = name_;
            ip = ip_;
            InitializeComponent();
            user = "/username" + " " + name;
            if (MainWindow.IsServer == true)
            {

                IPEndPoint ip_point = new IPEndPoint(IPAddress.Any, 8888);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(ip_point);
                socket.Listen(1000);
                ListenClients();

                // тут надо сделать еще один сокет для клиента, чтобы у тебя был и сервак и клиент одновременно


                
                socket_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket_server.ConnectAsync(ip, 8888); //ip - 127.0.0.1


                RecieveMessage(socket);
                SendMessage_serv("/username ADMIN");
            }
            else
            {
                log_chat.Visibility = Visibility.Hidden;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ConnectAsync(ip, 8888); //ip - 127.0.0.1

                
                RecieveMessage(socket);
                SendMessage(socket, user);
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
                list_user_log.Items.Add(client.RemoteEndPoint);
                RecieveMessage(client);
                
            }
        }

        private async Task RecieveMessage(Socket client)
        {
            /*list_user.Items.Clear();*/
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                list_message.Items.Add($"[Весточка от {message} kjhgf]: {message}");
                //MessageBox.Show(message.TrimStart());



                if (MainWindow.IsServer == true)
                {
                    
                    foreach (var item in clients)
                    {
                        if (message.StartsWith("/username"))
                        {
                            /*await SendMessage(item, "/ClearUserChat");
                            Debug.WriteLine("сервер отправил: ClearUserChat по адресу " + item.RemoteEndPoint);*/
                            list_user.Items.Add(message);
                            foreach (var items in list_user.Items)
                            {
                                if (message != item.ToString())
                                {
                                    //list_user.Items.Add(message);
                                    //await SendMessage(item, items.ToString());
                                    SendMessage(item, items.ToString());
                                }
                                
                                
                            }
                        }
                        /*else if (message.StartsWith("/ClearUserChat"))
                        {
                            list_user.Items.Clear();
                        }*/
                        
                    }
                    

                }
                else
                {
                    if (message.StartsWith("/username"))
                    {

                        //users.Add(message);
                        /*foreach (var item in users)
                        {
                            list_user.Items.Add(item);
                        }*/
                        list_user.Items.Add(message);
                        //users.Clear();
                    }
                    /*else if (message.StartsWith("/ClearUserChat"))
                    {
                            //Debug.WriteLine(name + ": клир получен");

                        list_user.Items.Clear();
                    }*/
                    //list_message.Items.Add(message);
                }


                
            }

        }

        private async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }




        private async Task RecieveMessage_serv_us()
        {
            /*list_user.Items.Clear();*/
            
            while (true)
            {
                
                byte[] bytes = new byte[1024];
                await socket_server.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                list_message.Items.Add($"Весточка от ADMIN: {message}");
                
                if (message.StartsWith("/username"))
                {
                    list_user.Items.Add(message);
                }


               

                



            }

        }

        private async Task SendMessage_serv(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await socket_server.SendAsync(bytes, SocketFlags.None);
        }









        private void message_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.IsServer == true)
            {
                SendMessage_serv(vestohka.Text);
            }
            else
            {
                SendMessage(socket, vestohka.Text);
            }
        }

        private void log_chat_Click(object sender, RoutedEventArgs e)
        {
            if (c == 0)
            {
                list_user.Visibility = Visibility.Hidden;
                list_user_log.Visibility = Visibility.Visible;
                c = 1;
            }
            else if(c == 1)
            {
                list_user.Visibility = Visibility.Visible;
                list_user_log.Visibility = Visibility.Hidden;
                c = 0;
            }
        }
    }
}
