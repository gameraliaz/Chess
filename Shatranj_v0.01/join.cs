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
    public partial class join : Form
    {
        public join()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
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

        private void join_Load(object sender, EventArgs e)
        {

        }
    }
}
