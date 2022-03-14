using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
namespace Shatranj_v0._01
{
    public partial class ClientForm : Form
    {
        TcpClient client;
        Stream stream;
        string fromserver;
        string toserver;
        string ipserver;
        string res;
        public ClientForm(string savestr)
        {
            res = "";
            toserver = savestr;
            InitializeComponent();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] b = new byte[50];
            b = ASCIIEncoding.ASCII.GetBytes(toserver);
            stream.Write(b, 0, toserver.Length);
            toserver = "";
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (client.Available != 0)
            {
                byte[] b = new byte[50];
                stream.Read(b, 0, 50);
                fromserver += ASCIIEncoding.ASCII.GetString(b);
                res = fromserver;
                richTextBox1.Text = res;
            }
        }
        private void ClientForm_Load(object sender, EventArgs e)
        {
            ipserver = "192.168.1.101";
            client = new TcpClient();
            client.Connect(ipserver, 2500);
            stream = client.GetStream();
            timer1.Enabled = true;
            timer1.Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = res;
        }
    }
}