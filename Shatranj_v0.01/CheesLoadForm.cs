using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    public partial class LoadForm : Form
    {
        Image Boardimg;
        Game gamef;
        public LoadForm(Game game)
        {
            gamef = game;
           // game.load();
         //   formgame = game;
            InitializeComponent();
        }
        private void Mouse_Hover(object sender, EventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(() =>
              {
                  string name = whatname(sender);
                  string address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
                  string btnname = ((Button)sender).Text;
                  if (File.Exists(address))
                  {
                      string savestr;
                      OpenFileDialog res = new OpenFileDialog();
                      res.Filter = "sv Files|*.sv;*";
                      res.FileName = address;
                      Stream fs = res.OpenFile();
                      StreamReader reader = new StreamReader(fs);
                      savestr = reader.ReadToEnd();
                      reader.Close();
                      pictureBox1.Image = ImageFromBase64String(savestr.Split('^')[1]);
                      MethodInvoker m = new MethodInvoker(() => lblfoe.Text = btnname + " is Full");
                      lblfoe.Invoke(m);
                  }
                  else
                  {
                      pictureBox1.Image = Boardimg;
                      MethodInvoker m = new MethodInvoker(() => lblfoe.Text = btnname + " is Empty");
                      lblfoe.Invoke(m);
                  }
              }));
            t1.Start();
        }
        private void ClickSlot(object sender, EventArgs e)
        {
            string name = whatname(sender);
            string address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
            if (File.Exists(address))
            {
                gamef.load(name);
                this.Close();
            }
            else
            {
                MessageBox.Show("This slot is empty","Empty slot");
            }
        }
        private string whatname(object sender)
        {
            Button btn = (Button)sender;
            switch (btn.Name)
            {
                case "Slot1":
                    return "save1";
                case "Slot2":
                    return "save2";
                case "Slot3":
                    return "save3";
                case "Slot4":
                    return "save4";
                default:
                    return "save5";
            }
        }
        public Image ImageFromBase64String(string base64)
        {
            MemoryStream memory = new MemoryStream(Convert.FromBase64String(base64));
            Image result = Image.FromStream(memory);
            memory.Close();
            return result;
        }
        private void CheesLoadForm_Load(object sender, EventArgs e)
        {
            Boardimg = pictureBox1.Image;
        }
        private void LoadForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
