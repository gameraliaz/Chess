using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Shatranj_v0._01
{
    public partial class Movementslistform : Form
    {
        SQLiteConnection con;
        public Movementslistform()
        {
            InitializeComponent();
        }
        private void Additems()
        {
            string[] arr = new string[4];
            ListViewItem itm;
            string stm = "SELECT * FROM movements";
            var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                arr[0] = Convert.ToString(rdr.GetInt32(0));
                arr[1] = rdr.GetString(3);
                arr[2] = rdr.GetString(1);
                arr[3] = rdr.GetString(2);
                itm = new ListViewItem(arr);
                MethodInvoker n = new MethodInvoker(() => listView1.Items.Add(itm));
                listView1.Invoke(n);
            }
        }
        private void save(string svn)
        {
            string[] arr = new string[4];
            FileStream fs = new FileStream(svn, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string stm = "SELECT * FROM movements";
            var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                arr[0] = Convert.ToString(rdr.GetInt32(0));
                arr[1] = rdr.GetString(3);
                arr[2] = rdr.GetString(1);
                arr[3] = rdr.GetString(2);
                sw.Write(arr[0] + " ");
                sw.Write(arr[1] + " ");
                sw.Write(arr[2] + " ");
                sw.Write(arr[3]);
                sw.WriteLine();
            }
            rdr.Close();
            sw.Close();
            fs.Close();
        }
        private void SQLConection()
        {
            string cs = @"URI=file:Chess.db";
            con = new SQLiteConnection(cs);
            con.Open();
        }
        private void refresh()
        {
            listView1.Items.Clear();
            Thread t2 = new Thread(new ThreadStart(() =>
            {
                SQLConection();
                Additems();
            }));
            t2.Start();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }
        private void Movementslistform_Load(object sender, EventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(() =>
            {
                MethodInvoker n = new MethodInvoker(() => button1.Enabled = false);
                button1.Invoke(n);
                SQLConection();
                Additems();
                n = new MethodInvoker(() => button1.Enabled = true);
                button1.Invoke(n);
            }));
            t1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = "Time  " + (DateTime.Now.ToString().Split(' ')[1]);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog SV = new SaveFileDialog();
            SV.Filter = "Text file (*.txt)|*.txt";
            SV.DefaultExt = ".txt";
            SV.FileName = "MovementResualt";
            if (SV.ShowDialog() == DialogResult.OK)
            {
                Thread t1 = new Thread(new ThreadStart(() =>
                {
                    SQLConection();
                    save(SV.FileName);
                }));
                t1.Start();
            }
        }
    }
}
