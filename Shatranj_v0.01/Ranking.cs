using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shatranj_v0._01
{
    public partial class RankingListForm : Form
    {
        SQLiteConnection con;
        int[] ids;
        public RankingListForm(int[] id)
        {
            ids = id;
            InitializeComponent();
        }
        private void SQLConection()
        {
            string cs = @"URI=file:Chess.db";
            con = new SQLiteConnection(cs);
            con.Open();
        }
        private void Additems()
        {
            string[] arr = new string[6];
            int temp=0;
            ListViewItem itm;
            string stm = "SELECT * FROM scores";
            var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                arr[0] = Convert.ToString(rdr.GetInt32(0));
                arr[1] = rdr.GetString(1);
                arr[2] = Convert.ToString(rdr.GetInt32(2));
                arr[3] = Convert.ToString(rdr.GetInt32(3));
                arr[4] = Convert.ToString(rdr.GetInt32(4));
                arr[5] = Convert.ToString(rdr.GetInt32(5));
                itm = new ListViewItem(arr);
                MethodInvoker n = new MethodInvoker(() => listView1.Items.Add(itm));
                listView1.Invoke(n);
                for (int i = 0; i < 2; i++)
                {
                    if (rdr.GetInt32(0) == ids[i])
                    {
                        MethodInvoker m = new MethodInvoker(() =>
                        {
                            listView1.Items[temp].Selected = true;
                        });
                        listView1.Invoke(m);
                    }
                }
                temp += 1;
            }
        }
        public void refresh()
        {
            listView1.Items.Clear();
            Thread t2 = new Thread(new ThreadStart(() =>
            {
                SQLConection();
                Additems();
            }));
            t2.Start();
        }
        private void Form4_Load(object sender, EventArgs e)
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
        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }
    }
}
