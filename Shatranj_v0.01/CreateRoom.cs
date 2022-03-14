using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    public partial class CreateRoom : Form
    {
        Color fcolor3;
        FullScreen fullScreen = new FullScreen();
        MultiplayerMenu back;
        public CreateRoom(MultiplayerMenu mm)
        {
            back = mm;
            InitializeComponent();
        }
        private void label3_Click(object sender, EventArgs e)
        {
            back.Show();
            this.Close();
        }
        private void label3_MouseHover(object sender, EventArgs e)
        {
            label3.ForeColor = Color.FromArgb(0, 255, 171);
        }
        private void label3_MouseLeave(object sender, EventArgs e)
        {
            label3.ForeColor = fcolor3;
        }
        private void CreateRoom_Load(object sender, EventArgs e)
        {
            fcolor3 = label3.ForeColor;
            fullScreen.EnterFullScreenMode(this);
            label4.Text="Your IP is : "+Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                textBox2.Visible = true;
                textBox2.Text = "";
            }
            else
            {
                textBox2.Text = "";
                textBox2.Visible = false;
            }
        }
        private void label5_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}
