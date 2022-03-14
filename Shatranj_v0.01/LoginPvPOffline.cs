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
    public partial class LoginPvPOffline : Form
    {
        SQLiteConnection con;
        bool play;
        MainMenu m;
        public LoginPvPOffline(MainMenu mf)
        {
            m = mf;
            InitializeComponent();
            SQLConection();
            play = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != textBox2.Text)
            {
                Thread t1 = new Thread(new ThreadStart(() => {
                    createTableAndColoumn_Scores();
                    inputnames(textBox1.Text, textBox2.Text);
                }));
                t1.Start();
                Game dualplayer = new Game("",m);
                t1.Join();
                dualplayer.Show();
                play = true;
                this.Close();
            }
            else MessageBox.Show("You cant input same names in textboxs");
        }
        private void SQLConection()
        {
            string cs = @"URI=file:Chess.db";
            con = new SQLiteConnection(cs);
            con.Open();
        }
        private void createTableAndColoumn_Scores()
        {
            var cmd = new SQLiteCommand(con);
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS scores(id INTEGER PRIMARY KEY, name TEXT, win INT, draw INT, loss INT, score INT, thisgame BOOLEAN)";
            cmd.ExecuteNonQuery();
        }
        private void inputnames(string n1, string n2)
        {
            string stm = "SELECT * FROM scores";
            var cmd = new SQLiteCommand(stm, con);
            cmd.CommandText = @"INSERT INTO scores(name, win, draw, loss, score, thisgame) VALUES('" + n1 + "', 0, 0, 0, 0, True)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"INSERT INTO scores(name, win, draw, loss, score, thisgame) VALUES('" + n2 + "', 0, 0, 0, 0, True)";
            cmd.ExecuteNonQuery();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
            if (!play)
            {
                m.Show();
            }
            GC.Collect();
        }
    }
}
