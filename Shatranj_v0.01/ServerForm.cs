using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    public partial class ServerForm : Form
    {
        string svip;
        string fromclient;
        string toclient;
        TcpListener server;
        TcpClient client;
        Stream stream;
        bool started;
        string res;
        public ServerForm(string savestr)
        {
            res = "";
            started = false;
            toclient = savestr;
            InitializeComponent();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(() =>
              {
                  IPAddress ip = IPAddress.Parse(svip);
                  server = new TcpListener(ip, 2500);
                  server.Start();
                  client = server.AcceptTcpClient();
                  stream = client.GetStream();
                  started = true;
              }));
            t1.Start();
            timer1.Enabled = true;
            timer1.Start();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] b = new byte[50];
            b = ASCIIEncoding.ASCII.GetBytes(toclient);
            stream.Write(b, 0, toclient.Length);
            toclient = "";
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (started)
            {
                if (client.Available != 0)
                {
                    byte[] b = new byte[50];
                    stream.Read(b, 0, 50);
                    fromclient += ASCIIEncoding.ASCII.GetString(b);
                    res = fromclient;
                    richTextBox1.Text = res;
                }
            }
        }
        private void ServerForm_Load(object sender, EventArgs e)
        {
            svip = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
            label1.Text = svip;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = res;
        }
    }
}

    
