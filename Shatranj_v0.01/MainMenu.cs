using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private void singleplayer_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Comming late");
        }
        private void dualplayer_btn_Click(object sender, EventArgs e)
        {
            LoginPvPOffline loginpage = new LoginPvPOffline(this);
            loginpage.Show();
            this.Hide();
        }
        private void multiplayer_btn_Click(object sender, EventArgs e)
        {
            MultiplayerMenu mpmenu = new MultiplayerMenu(this);
            mpmenu.Show();
            this.Hide();
           // MessageBox.Show("Comming late");
        }
        private void aboutus_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Creator AliSoleimani 9912151022");
        }
        private void exit_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
