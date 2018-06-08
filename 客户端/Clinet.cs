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

namespace 客户端
{
    public partial class Clinet : Form
    {
        public Clinet()
        {
            InitializeComponent();
        }

        //创建数据对象
        Data get = new 客户端.Data();

        /// <summary>
        /// 客户端的Socket
        /// </summary>
        Socket clinet;

        private void Clinet_Load(object sender, EventArgs e)
        {
            RefTable();
            //创建socket
            CreateSocket();
        }

        /// <summary>
        /// 创建客户端的Socket
        /// </summary>
        private void CreateSocket()
        {
            // 创建socket
            clinet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //连接
            //获得ip和端口(读取配置文件)
            var app = System.Configuration.ConfigurationManager.AppSettings;
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(app["ip"]), Convert.ToInt32(app["port"]));

            //连接服务器
            clinet.Connect(point);

            //开启新线程获取服务器端消息
            Thread thClinet = new Thread(new ThreadStart(CallRec));
            thClinet.IsBackground = true;
            thClinet.Start();
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        private void CallRec()
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
                    RefTable();
                }
            }
        }

        /// <summary>
        /// 刷新表格
        /// </summary>
        private void RefTable()
        {
            dataGridView1.Invoke(new Action(() =>
            {
                dataGridView1.DataSource = get.GetPersonList();
            }));
            
        }

        /// <summary>
        /// 添加数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //获取用户输入
            string name = txtAddName.Text;
            int age = Convert.ToInt32(txtAgeAdd.Text);
            string phone = txtPhoneAdd.Text;

            //实例化一个数据对象
            Person p = new Person() { Name = name, Age = age, Phone = phone };
            //写入数据
            AddList(p);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="p"></param>
        private void AddList(Person p)
        {
            //获取数据集合
            List<Person> list = dataGridView1.DataSource as List<Person>;
            //加入数据
            list.Add(p);
            //加入数据
            bool b = get.Add(list);
            if (b)
            {
                MessageBox.Show("增加成功");
                //增加成功后发送socket信息
                //向服务器发送消息
                clinet.Send(Encoding.UTF8.GetBytes("1"));
            }
            else
            {
                MessageBox.Show("增加失败");
            }
        }
       

        private void Clinet_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }
    }
}
