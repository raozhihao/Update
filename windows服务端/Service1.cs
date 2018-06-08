using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace windows服务端
{
    public partial class Service1 : ServiceBase
    {
        List<Socket> clinetSockets;
        Socket socket;
       
        public Service1()
        {
            InitializeComponent();
           

        }

        protected void SocketStart()
        {
            clinetSockets = new List<Socket>();
            //创建socket对象
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获取ip地址和端口
            var set = System.Configuration.ConfigurationManager.AppSettings;
            string txtIPAddress = set["ip"];
            string txtPort = set["port"];
            IPAddress ip = IPAddress.Parse(txtIPAddress);
            int port = Convert.ToInt32(txtPort);
            IPEndPoint point = new IPEndPoint(ip, port);
            //绑定ip和端口
            socket.Bind(point);
            //设置最大连接数
            socket.Listen(10);
            //listBox1.Items.Add("服务器已开启,等待客户端连接中.....");

            //开启新线程监听
            Thread serverThread = new Thread(ListenClientConnect);
            serverThread.IsBackground = true;
            serverThread.Start(socket);
        }

        protected override void OnStart(string[] args)
        {
            //ThreadPool.QueueUserWorkItem(h => SocketStart());
            SocketStart();
        }
        byte[] buffer = new byte[1024];
        /// <summary>
        /// 监听传入
        /// </summary>
        /// <param name="ar"></param>
        private void ListenClientConnect(object ar)
        {
            bool flag = true;
            //获得服务器的Socket
            Socket serverSocket = ar as Socket;
            while (flag)
            {
                //获得连入的客户端socket
                Socket clientSocket = serverSocket.Accept();
                clinetSockets.Add(clientSocket);
                //SendMessage(string.Format("客户端{0}已成功连接到服务器\r\n", clientSocket.RemoteEndPoint));
                var mReveiveThread = new Thread(ReceiveClient);
                mReveiveThread.IsBackground = true;
                mReveiveThread.Start(clientSocket);
            }

        }
        private byte[] result = new byte[1024];
        private void ReceiveClient(object obj)
        {
            //获取当前客户端
            //因为每次发送消息的可能并不是同一个客户端，所以需要使用var来实例化一个新的对象
            //可是我感觉这里用局部变量更好一点
            var mClientSocket = (Socket)obj;
            // 循环标志位
            bool flag = true;
            while (flag)
            {
                try
                {
                    //获取数据长度
                    int receiveLength = mClientSocket.Receive(result);
                    //获取客户端消息
                    string clientMessage = Encoding.UTF8.GetString(result, 0, receiveLength);
                    //服务端负责将客户端的消息分发给各个客户端
                    //判断客户端发来的消息是否是预定的标志
                    if (clientMessage == "1")
                    {
                        //通知各客户端
                        this.SendMessage("1");
                    }

                   
                }
                catch
                {
                    //从客户端列表中移除该客户端
                    clinetSockets.Remove(mClientSocket);
                   
                    //断开连接
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    break;
                }
            }
        }

        /// <summary>
        /// 向所有的客户端群发消息
        /// </summary>
        /// <param name="msg">message</param>
        public void SendMessage(string msg)
        {
            //确保消息非空以及客户端列表非空
            if (msg == string.Empty || clinetSockets.Count <= 0) return;
            //向每一个客户端发送消息
            foreach (Socket s in this.clinetSockets)
            {
                (s as Socket).Send(Encoding.UTF8.GetBytes(msg));
            }
        }
        protected override void OnStop()
        {
           
        }
    }
}
