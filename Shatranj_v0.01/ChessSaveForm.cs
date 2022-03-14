using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    public partial class ChessSaveForm : Form
    {
        Image Boardimg;
        string Save;
        public ChessSaveForm(string savestr)
        {
            Save = savestr;
            InitializeComponent();
        }
        public Image ImageFromBase64String(string base64)
        {
            MemoryStream memory = new MemoryStream(Convert.FromBase64String(base64));
            Image result = Image.FromStream(memory);
            memory.Close();
            return result;
        }
        private void ChessSaveForm_SizeChanged(object sender, EventArgs e)
        {/*
            if (pictureBox1.Size.Width / pictureBox1.Size.Height != 1)
            {
                if (pictureBox1.Size.Width / pictureBox1.Size.Height > 1)
                {
                    pictureBox1.Dock = DockStyle.Right;
                }
                else if (pictureBox1.Size.Height / pictureBox1.Size.Width > 1)
                {
                    pictureBox1.Dock = DockStyle.Top;
                }
            }else
                pictureBox1.Dock = DockStyle.Fill;*/
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
            string btnname = ((Button)sender).Text;
            if (File.Exists(address))
            {
                DialogResult res = MessageBox.Show("Replace?", "Slot is full", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    save(name);
                    lblfoe.Text = btnname + " is Full";
                    pictureBox1.Image = ImageFromBase64String(Save.Split('^')[1]);
                }
            }
            else
            {
                save(name);
                lblfoe.Text = btnname + " is Full";
                pictureBox1.Image = ImageFromBase64String(Save.Split('^')[1]);
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
        private void save(string name)
        {
            Thread t1 = new Thread(new ThreadStart(() => {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "sv files (*.sv)|*.sv";
                save.FileName = String.Empty;
                save.DefaultExt = ".sv";
                string address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
                if (!Directory.Exists(Path.GetDirectoryName(address)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(address));
                }
                save.FileName = address;
                FileStream fs = new FileStream(save.FileName, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(Save);
                writer.Close();
                fs.Close();
            }));
            t1.Start();
        }
        private void ChessSaveForm_Load(object sender, EventArgs e)
        {
            Boardimg = pictureBox1.Image;
        }
    }
}
