using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Shatranj_v0._01
{
    public partial class MultiplayerMenu : Form
    {
        Color fcolor_2_3;
        Color fcolor4;
        FullScreen fullScreen = new FullScreen();
        Thread t1;
        MainMenu m;
        public MultiplayerMenu(MainMenu menu)
        {
            m = menu;
            InitializeComponent();
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        private void MultiplayerMenu_Load(object sender, EventArgs e)
        {
            fullScreen.EnterFullScreenMode(this);
            fcolor_2_3 = label5.ForeColor;
            fcolor4 = label4.ForeColor;
            label1.ForeColor = Color.FromArgb(253, 254, 255);
            changeingcolor();
            label2.Text = "Your IP is: " + GetLocalIPAddress();
        }
        private void changeingcolor()
        {
            t1 = new Thread(new ThreadStart(() =>
            {
                for (; ; )
                {
                    for (int i = 255; i >= 100; i -= 5)
                    {
                        label1.ForeColor = Color.FromArgb(i, label1.ForeColor.G, label1.ForeColor.B);
                        Thread.Sleep(1);
                    }
                    for (int i = 255; i >= 0; i -= 5)
                    {
                        label1.ForeColor = Color.FromArgb(label1.ForeColor.R, i, label1.ForeColor.B);
                        Thread.Sleep(1);
                    }
                    for (int i = 255; i >= 40; i -= 5)
                    {
                        label1.ForeColor = Color.FromArgb(label1.ForeColor.R, label1.ForeColor.G, i);
                        Thread.Sleep(1);
                    }
                    for (int i = 100; i <= 255; i += 5)
                    {
                        label1.ForeColor = Color.FromArgb(i, label1.ForeColor.G, label1.ForeColor.B);
                        Thread.Sleep(1);
                    }
                    for (int i = 0; i <= 255; i += 5)
                    {
                        label1.ForeColor = Color.FromArgb(label1.ForeColor.R, label1.ForeColor.G, i);
                        Thread.Sleep(1);
                    }
                    for (int i = 40; i <= 255; i += 5)
                    {
                        label1.ForeColor = Color.FromArgb(label1.ForeColor.R, i, label1.ForeColor.B);
                        Thread.Sleep(1);
                    }
                    Thread.Sleep(100);
                }
            }));
            t1.Start();
        }
        private void label4_MouseHover(object sender, EventArgs e)
        {
            label4.ForeColor = Color.FromArgb(0, 255, 171);
        }
        private void label4_Click(object sender, EventArgs e)
        {
            m.Show();
            this.Close();
        }
        private void MultiplayerMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = fcolor4;
        }
        private void MultiplayerMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            t1.Abort();
        }
        private void label5_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
                textBox2.Text = "Player";
            Room R = new Room("", textBox2.Text.Trim());
            R.Show();
            this.Hide();
        }
        private void label5_MouseEnter(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.ForeColor = Color.FromArgb(171, 255, 0);
        }
        private void label5_MouseLeave(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.ForeColor = fcolor_2_3;
        }
        private void label3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
                textBox2.Text = "Player";
            Room R = new Room(textBox1.Text.Trim(),textBox2.Text.Trim());
            R.Show();
            this.Hide();
        }
    }
}
