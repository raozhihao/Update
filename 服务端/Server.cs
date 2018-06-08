using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 服务端
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 已连接上的客户端集合
        /// </summary>
        List<Socket> clinetSockets;
        /// <summary>
        /// 服务端主Socket
        /// </summary>
        Socket socket;

        /// <summary>
        /// 设置数据缓冲区
        /// </summary>
        private byte[] result = new byte[1024];

        /// <summary>
        /// 开启侦听按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //初始化
            clinetSockets = new List<Socket>();
            //创建socket对象
             socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //获取ip地址和端口(其实应该把它放在配置文件中,客户端的ip和port都放在配置文件中)
            IPAddress ip = IPAddress.Parse(txtIPAddress.Text.Trim());
            int port = Convert.ToInt32(txtPort.Text.Trim());
            IPEndPoint point = new IPEndPoint(ip, port);

            //绑定ip和端口
            socket.Bind(point);
            //设置最大连接数
            socket.Listen(10);

            listBox1.Items.Add("服务器已开启,等待客户端连接中.....");

            //开启新线程监听
            Thread serverThread = new Thread(ListenClientConnect);
            serverThread.IsBackground = true;
            serverThread.Start(socket);

        }

        
        /// <summary>
        /// 监听传入
        /// </summary>
        /// <param name="ar"></param>
        private void ListenClientConnect(object ar)
        {
            //设置标志
            bool flag = true;
            //获得服务器的Socket
            Socket serverSocket = ar as Socket;
            //轮询
            while (flag)
            {
                //获得连入的客户端socket
                Socket clientSocket = serverSocket.Accept();
                //将新加入的客户端加入列表中
                clinetSockets.Add(clientSocket);

                //向listbox中写入消息
                listBox1.Invoke(new Action(() => {
                    listBox1.Items.Add(string.Format("客户端{0}已成功连接到服务器\r\n", clientSocket.RemoteEndPoint));
                }));
                //开启新的线程,进行监听客户端消息
                var mReveiveThread = new Thread(ReceiveClient);
                mReveiveThread.IsBackground = true;
                mReveiveThread.Start(clientSocket);
            }

        }
       
        /// <summary>
        /// 接收客户端传过来的数据
        /// </summary>
        /// <param name="obj"></param>
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
                    if (clientMessage=="1")
                    {
                        //通知各客户端
                        this.SendMessage("1");
                    }

                    //向listbox中写入消息
                    listBox1.Invoke(new Action(() => {
                        listBox1.Items.Add(string.Format("客户端{0}发来消息{1}", mClientSocket.RemoteEndPoint, clientMessage));
                    }));

                }
                catch(Exception e)
                {
                    //从客户端列表中移除该客户端
                    clinetSockets.Remove(mClientSocket);
                    
                    //显示客户端下线消息
                    listBox1.Invoke(new Action(() =>
                    {
                        listBox1.Items.Add(string.Format("服务器发来消息:客户端{0}从服务器断开,断开原因:{1}\r\n", mClientSocket.RemoteEndPoint, e.Message));
                    }));

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

        /// <summary>
        /// 窗体关闭后释放资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
