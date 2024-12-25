using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace lab1SISprog_Nazarov_MainApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _serverSocket;
        private const int Port = 12345;

        public MainWindow()
        {
            InitializeComponent();
            StartServer();
        }

        private void StartServer()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var clientSocket = _serverSocket.EndAccept(ar);
            var buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, new Tuple<Socket, byte[]>(clientSocket, buffer));
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = (Tuple<Socket, byte[]>)ar.AsyncState;
            var clientSocket = state.Item1;
            var buffer = state.Item2;

            var bytesRead = clientSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                var data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Dispatcher.Invoke(() => ApplyStyle(data));
            }
            clientSocket.Close();
        }

        private void ApplyStyle(string styleData)
        {
            var parts = styleData.Split(';');
            if (parts.Length == 4)
            {
                var controlType = parts[0];
                var background = (Brush)new BrushConverter().ConvertFromString(parts[1]);
                var foreground = (Brush)new BrushConverter().ConvertFromString(parts[2]);
                var fontSize = double.Parse(parts[3]);

                switch (controlType)
                {
                    case "TextBox":
                        textBox1.Background = background;
                        textBox1.Foreground = foreground;
                        textBox1.FontSize = fontSize;
                        break;
                    case "Button":
                        button1.Background = background;
                        button1.Foreground = foreground;
                        button1.FontSize = fontSize;
                        break;
                    case "Label":
                        label1.Background = background;
                        label1.Foreground = foreground;
                        label1.FontSize = fontSize;
                        break;
                }
            }
        }
    }
}
