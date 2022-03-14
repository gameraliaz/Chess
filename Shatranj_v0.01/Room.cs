using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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
    public partial class Room : Form
    {
        SQLiteConnection con;
        string IpAdress = "";
        string[] commends=new string[2];
        string[] scommends=new string[2];
        bool HostGame = false;
        bool pReady = false;
        int readyCount = 0;
        bool serverstarted = false;
        string PName;
        string P2Name;
        TcpListener server;
        TcpClient client;
        Stream stream;
        Game GameForm;
        bool timerstart;
        bool again;
        public Room(string _IpAdress,string Pnick)
        {
            again = false;
            InitializeComponent();
            timerstart = false;
            PName = Pnick;
            HostGame = (_IpAdress == "");
            IpAdress = (HostGame) ? GetLocalIPAddress(): _IpAdress;
            commends[0] =  (HostGame) ? "-The_guest_is_ready" : "-The_host_is_ready" ;
            commends[1] = (HostGame) ? "-The_guest_is_not_ready" : "-The_host_is_not_ready";
            scommends[0] = (!HostGame) ? "-The_guest_is_ready" : "-The_host_is_ready";
            scommends[1] = (!HostGame) ? "-The_guest_is_not_ready" : "-The_host_is_not_ready";
            GameForm = new Game((HostGame)?"0":IpAdress, this);
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
        private void Room_Load(object sender, EventArgs e)
        {
            lblIP.Text = "Room IP : "+IpAdress;
            lblcountReady.Text = "0/1";
            timer1.Enabled = true;
            timer1.Start();
            Thread t2 = new Thread(new ThreadStart(() =>
            {
                if (HostGame)
                    HostStart();
                else
                    GeustConection();
                timerstart = true;
            }));
            t2.Start();
            Thread t3=new Thread(new ThreadStart(()=>
            {
                SQLConection();
                createTableAndColoumn_Scores();
            }));
            t3.Start();
        }
        private void HostStart()
        {
            IPAddress ip = IPAddress.Parse(IpAdress);
            server = new TcpListener(ip, 2500);
            server.Start();
            AddMsgToPanel("Server Started",0);
            client = server.AcceptTcpClient();
            stream = client.GetStream();
            AddMsgToPanel("Someone joined",0);
            Sendmsg("-PlayerNickname_"+PName);
            MethodInvoker m = new MethodInvoker(() => { lblcountReady.Text = "0/2"; });
            lblcountReady.Invoke(m);
        }
        private void GeustConection()
        {
            
            client = new TcpClient();
            while (true)
            {
                try
                {
                    client.Connect(IpAdress, 2500);
                    break;
                }
                catch
                {
                    DialogResult dialogResult = MessageBox.Show("Connection Error, Try Again?", "Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        this.Close();
                    }else
                    {
                        continue;
                    }
                }
            }
            stream = client.GetStream();
            AddMsgToPanel("You joined",0);
            Sendmsg("-PlayerNickname_" + PName);
            MethodInvoker m = new MethodInvoker(() => { lblcountReady.Text = "0/2"; });
            lblcountReady.Invoke(m);

        }
        public void Sendmsg(string SendStr)
        {
            byte[] b = new byte[SendStr.Length];
            b = ASCIIEncoding.ASCII.GetBytes(SendStr);
            stream.Write(b, 0, SendStr.Length);
        }
        private string Recivemsg()
        { 

            string res = "";
            while (true)
            {
                Thread.Sleep(100);
                if (client.Available != 0)
                {
                    byte[] b = new byte[64];
                    stream.Read(b, 0, 64);
                    res += ASCIIEncoding.ASCII.GetString(b);
                }
                return res.Trim();
            }
            
        }
        private string originalStr(string msg)
        {
            char[] msgchar = msg.ToCharArray();
            for(int i=0;i<msg.Length;i++)
            {
                if (msgchar[i] == '\0')
                    return msg.Substring(0, i);
            }
            return msg;
        }
        private void AddMsgToPanel(string msg,Byte whatmsg)
        {
            Button msgControl = new Button();
            msgControl.Text = msg;
            Font f = new Font("Microsoft Sans Serif", 14);
            msgControl.Font = f;
            Color c;
            if (whatmsg == 0)
                c = Color.Orange;
            else if (whatmsg == 1)
                c = Color.Lime;
            else
                c = Color.White;
            msgControl.ForeColor = c;
            msgControl.Dock = DockStyle.Fill;
            RowStyle rs = new RowStyle();
            rs.SizeType = SizeType.Absolute;
            rs.Height = 50;
            MethodInvoker m = new MethodInvoker(() => {
                tlpChat.RowStyles.Add(rs);
                tlpChat.Controls.Add(msgControl, 0, tlpChat.RowStyles.Count - 1);
            });
            tlpChat.Invoke(m);
        }// byte : 0=>system msg      1=> my msg      anynum=>player msg
        private void label3_Click(object sender, EventArgs e)
        {
            AddMsgToPanel(textBox1.Text,1);
            Sendmsg(textBox1.Text);
            textBox1.Text = "";
        }
        private void Room_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
        private void Room_FormClosed(object sender, FormClosingEventArgs e)
        {
            if (lblcountReady.Text.Contains("/2"))
            {
                Sendmsg("(e");
                Thread.Sleep(1000);
            }
            Environment.Exit(0);
        }
        public void gameclosed()
        {
            pReady = false;
            Sendmsg((HostGame) ? "-The_host_is_not_ready" : "-The_guest_is_not_ready");
            readyCount--;
            lblReady.ForeColor = Color.Yellow;
            lblcountReady.Text = "0/2";
            serverstarted = false;
            again = true;
            this.Show();
        }
        private void lblReady_Click(object sender, EventArgs e)
        {
            if (lblcountReady.Text.Contains("/2"))
            {
                pReady = !pReady;
                Sendmsg(scommends[(pReady) ? 0 : 1]);
                readyCount += (pReady) ? 1 : -1;
                lblcountReady.Text = readyCount.ToString() + "/2";
                lblReady.ForeColor = (pReady)?Color.LimeGreen:Color.Yellow;
                if (readyCount == 2 && HostGame)
                {
                    inputnames(PName, P2Name);
                    if (again)
                    {
                        GameForm = new Game((HostGame) ? "0" : IpAdress, this);
                        again = false;
                    }
                    GameForm.Show();
                    this.Hide();
                }
                else
                {
                    serverstarted = true;
                }
            }
        }
        //
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(timerstart)
            {
                string msg = Recivemsg();
                msg = originalStr(msg.Trim());
                if (msg != "")
                {
                    if (msg == commends[0])
                    {
                        readyCount++;
                        lblcountReady.Text = readyCount.ToString() + "/2"; 
                        if (readyCount == 2)
                        {
                            if (HostGame)
                            {
                                inputnames(PName, P2Name);
                                if (again)
                                {
                                    GameForm = new Game((HostGame) ? "0" : IpAdress, this);
                                    again = false;
                                }
                                GameForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                serverstarted = true;
                            }
                        }
                        //msg = commends[0].Replace("-", "").Replace("_"," ");
                    }
                    else if (msg == commends[1])
                    {
                        readyCount--;
                        lblcountReady.Text = readyCount.ToString() + "/2"; 
                        
                    }
                    else if (msg.Split('_')[0] == "-PlayerNickname")
                    {
                        P2Name = msg.Substring(16);
                        if (PName == P2Name)
                        {
                            PName = "H" + PName;
                            P2Name = "G" + P2Name;
                        }
                        else
                        {
                            P2Name = (HostGame) ? msg.Substring(16) : PName;
                            PName = (HostGame) ? PName : msg.Substring(16);
                        }
                    }
                    else if (serverstarted && !HostGame && msg == "+Server_started")
                    {
                        inputnames(PName, P2Name);
                        if (again)
                        {
                            GameForm = new Game((HostGame) ? "0" : IpAdress, this);
                            again = false;
                        }
                        GameForm.Show();
                        this.Hide();
                    }

                    else if(msg== "(e")
                    {
                        if (HostGame)
                        {
                            timerstart = false;
                            server.Server.Close();
                            stream.Close();
                            client.Close();
                            readyCount = pReady ? 1 : 0;
                            AddMsgToPanel("Player lefted us",0);
                            lblcountReady.Text = readyCount+"/1";
                            lblReady.ForeColor = (pReady) ? Color.LimeGreen : Color.Yellow;
                            Thread t = new Thread(new ThreadStart(() =>
                              {
                                  HostStart();
                                  timerstart = true;
                              }));
                            t.Start();
                        }
                        else
                        {
                            timerstart = false;
                            readyCount = pReady ? 1 : 0;
                            lblcountReady.Text = readyCount + "/1";
                            client.Close();
                            stream.Close();
                            MessageBox.Show(" Host was lefted ");
                            Application.Restart();
                        }
                        
                    }
                    else
                    {
                        AddMsgToPanel(msg,2);
                    }
                }
            }
        }
    }
}
