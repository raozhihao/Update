using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoNow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Data data = new DemoNow.Data();

        NowUpdate.Now now = new NowUpdate.Now();
        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            int age = Convert.ToInt32(textBox2.Text);
            string phone = textBox3.Text;
            Person p = new Person()
            {
                Age = age,
                Name = name,
                Phone = phone
            };
            List<Person> list = dataGridView1.DataSource as List<Person>;
            list.Add(p);
            bool b = data.Add(list);
            if (b)
            {
                MessageBox.Show("增加成功");
                now.Send();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = data.GetPersonList();
            now.DoSomeThing += RefTable;
        }

        /// <summary>
        /// 刷新表格
        /// </summary>
        private void RefTable()
        {
            dataGridView1.Invoke(new Action(() =>
            {
                dataGridView1.DataSource = data.GetPersonList();
            }));

        }
    }
}
