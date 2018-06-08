using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NowUpdate
{
    /// <summary>
    /// 可进行实时更新
    /// </summary>
    public class Now
    {
        Socket clinet;
        /// <summary>
        /// 需要在实时更新时做的事，最好置于程序最开头处，例如Form_Load()
        /// </summary>
        public event Action DoSomeThing;
       
        /// <summary>
        /// 此构造函数最好置于程序最开头处，例如Form_Load()
        /// </summary>
        public Now()
        {
            clinet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var appSet = System.Configuration.ConfigurationManager.AppSettings;
            IPAddress ip = IPAddress.Parse(appSet["ip"]);
            int port = Convert.ToInt32(appSet["port"]);

            IPEndPoint point = new IPEndPoint(ip, port);

            clinet.BeginConnect(point, GetReslut, null);
        }

        private void GetReslut(IAsyncResult ar)
        {
            bool flag = true;
            while (flag)
            {
                byte[] recBuf = new byte[1024];
                //获取返回数据的长度
                int length = clinet.Receive(recBuf);
                //获取监听到的数据
                string reslut = Encoding.UTF8.GetString(recBuf, 0, length);

                if (reslut == "1")
                {
                    //刷新表格数据
                    DoSomeThing?.Invoke();
                } 
            }
        }
        
        /// <summary>
        /// 在对程序进行更新操作后对服务器发送确认消息
        /// </summary>
        public void Send()
        {
            clinet.Send(Encoding.UTF8.GetBytes("1"));
        }
    }
}
