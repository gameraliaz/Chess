using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;

namespace Shatranj_v0._01
{
    public partial class Game : Form
    {
        bool restart;
        int[] exist = new int[64];
        bool[] h_light = new bool[64];
        bool[] h_light_KR = new bool[64]; //king rook
        Color[] first_color = new Color[64];
        public Image[] images = new Image[14];
        Button[] numtobtn = new Button[64];
        bool[] first_pawn_move = new bool[64];
        bool[] pawnmoved = new bool[64];
        bool w_king_Moved;
        bool b_king_Moved;
        bool w_big_rook_Moved;
        bool b_big_rook_Moved;
        bool w_small_rook_Moved;
        bool b_small_rook_Moved;
        int[] score = new int[3];
        int Last_Loc;
        public string Turn;
        int white_gametime;
        int movetime;
        int black_gametime;
        int[] ids = new int[2];
        int pr = 0;
        SQLiteConnection con;
        bool loading;
        bool gamestarted;

        // Network
        Room room;
        MainMenu mm;
        string IpAdress = "";
        bool nGame = false;
        bool hGame = false;
        bool ended = false;
        TcpListener server;
        TcpClient client;
        Stream stream;
        public Game(string _IpAdress, object mainform)
        {
            gamestarted = false;
            nGame = (_IpAdress != "");
            hGame = (_IpAdress == "0");
            if (nGame)
                room = (Room)mainform;
            else
                mm = (MainMenu)mainform;
            IpAdress = (hGame) ? GetLocalIPAddress() : _IpAdress;
            white_gametime = 3600;
            black_gametime = 3600;
            movetime = 300;
            score[0] = 3; //win
            score[1] = 1;//draw
            score[2] = 0;//loss
            ids[0] = -1;
            ids[1] = -1;
            loading = true;
            InitializeComponent();
        }
        //network
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
        private void HostStart()
        {
            IPAddress ip = IPAddress.Parse(IpAdress);
            server = new TcpListener(ip, 2501);
            server.Start();
            MethodInvoker m = new MethodInvoker(() => serverinfoconwait.Text = "Server Started");
            room.Sendmsg("+Server_started");
            serverinfoconwait.Invoke(m);
            client = server.AcceptTcpClient();
            stream = client.GetStream();
            gamestarted = true;
        }
        private void GeustConection()
        {

            client = new TcpClient();
            while (true)
            {
                try
                {
                    client.Connect(IpAdress, 2501);
                    break;
                }
                catch
                {
                    DialogResult dialogResult = MessageBox.Show("Connection Error, Try Again?", "Error", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        this.Close();
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            stream = client.GetStream();
            MethodInvoker m = new MethodInvoker(() => serverinfoconwait.Text = "You joined");
            serverinfoconwait.Invoke(m);
            gamestarted = true;
        }
        private void Sendmsg(string SendStr)
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
            for (int i = 0; i < msg.Length; i++)
            {
                if (msgchar[i] == '\0')
                    return msg.Substring(0, i);
            }
            return msg;
        }
        //e network
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread t3 = new Thread(new ThreadStart(loadgame));
            t3.Start();
            load_restart();
            pr += 10;
            //t3.Join();
            //timer1
        }
        private void loadgame()
        {
            while (loading)
            {
                if (pr == 100)
                {
                    Thread.Sleep(500);
                    MethodInvoker n = new MethodInvoker(() => panel1.Controls.Remove(progressBar1));
                    progressBar1.Invoke(n);
                    MethodInvoker nm = new MethodInvoker(() => panel1.Controls.Remove(backgroundloading));
                    panel1.Invoke(nm);
                    Thread t1 = new Thread(new ThreadStart(() =>
                    {
                        if (nGame)
                        {
                            if (hGame)
                                HostStart();
                            else
                                GeustConection();

                        }
                        MessageBox.Show("Player : " + IDInfo(1, ids[0]) + " is White\n Player : " + IDInfo(1, ids[1]) + " is Black");
                        gamestarted = true;

                    }));
                    t1.Start();
                    Thread t2 = new Thread(new ThreadStart(() =>
                    {
                        save("default");
                    }));
                    t2.Start();
                    break;
                }
                MethodInvoker m = new MethodInvoker(() => progressBar1.Value = pr);
                try
                {
                    progressBar1.Invoke(m);
                }
                catch
                {
                    break;
                }
            }
        }
        private void load_restart()
        {
            Thread t3 = new Thread(new ThreadStart(() =>
            {
                string name = "autosave";
                string address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
                if (File.Exists(@address))
                {
                    File.Delete(Path.GetFullPath("./") + @"saves\" + name + ".sv");
                }
                name = "autosave1";
                address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
                if (File.Exists(@address))
                {
                    File.Delete(Path.GetFullPath("./") + @"saves\" + name + ".sv");
                }
                pr += 5;
            }));
            t3.Start();
            set_firstexist();
            set_numtobtn();
            for (int i = 0; i < 64; i++)
                numtobtn[i].Enabled = true;
            pr += 5;
            Thread t2 = new Thread(new ThreadStart(th1));
            t2.Start();

            Thread t1 = new Thread(new ThreadStart(thnext));
            t1.Start();
        }
        private void th1()
        {
            Thread t1 = new Thread(new ThreadStart(img));
            t1.Start();
            Thread t2 = new Thread(new ThreadStart(color));
            t2.Start();
            Turn = "W";
            set_moved();
            pr += 5;
        }
        private void color()
        {
            set_firstcolor();
            pr += 15;
            default_h_light();
            High_light(-1);
            pr += 5;
        }
        private void img()
        {
            set_Images();
            pr += 10;
            set_img();
            pr += 10;
        }
        private void thnext()
        {
            white_gametime = 3600;
            black_gametime = 3600;
            movetime = 300;
            timer1.Enabled = true;
            SQLConection();
            pr += 10;
            createTableAndColoumn_Movements();
            pr += 10;
            playersID();
            pr += 5;
            sameplayer();
            pr += 10;
        }
        // 
        private void Buttons_Func(object sender, EventArgs e)
        {
            int num = btn_number((Button)sender);
            if (nGame && ((hGame && Turn == "W") || (!hGame && Turn == "B")) || !nGame)
            {
                if (h_light[num] == false && h_light_KR[num] == false)
                {
                    if ((Turn == WhatIsthis(num) || WhatIsthis(num) == null))
                    {
                        BefMove((Button)sender);
                        Correct_Hlight(num);
                        if (checker((Button)sender) == 1 || checker((Button)sender) == 7)
                            king_rook_highlight();
                        High_light(num);
                        Last_Loc = btn_number((Button)sender);
                    }
                }
                else
                {
                    if (nGame)
                    {
                        Sendmsg("=" + Last_Loc + "_" + num);
                    }
                    if (h_light_KR[num])
                    {
                        King_Rook_Move(num);
                    }
                    if (exist[Last_Loc] == 6 || exist[Last_Loc] == 12)
                    {
                        en_passant_Move(num);
                    }
                    move(num, Last_Loc);
                    default_h_light();
                    High_light(num);
                    if (KishOMat() == 1)
                    {
                        EndGame(true);
                    }
                    else if (KishOMat() == -1)
                    {
                        EndGame(false);
                    }
                    checkingFirstMove(num);
                    Thread t1 = new Thread(new ThreadStart(() =>
                    {
                        string name = "autosave";
                        string address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
                        if (!File.Exists(@address))
                        {
                            save(name);
                        }
                        else
                        {
                            File.Delete(Path.GetFullPath("./") + @"saves\" + name + "1.sv");
                            File.Move(address, Path.GetFullPath("./") + @"saves\" + name + "1.sv");
                            save(name);
                        }
                    }));
                    t1.Start();

                }
            }
        }
        private void BefMove(Button sender)
        {
            default_h_light();
            if (checker(sender) == 1 || checker(sender) == 7)
            {
                King(sender);
            }
            else if (checker(sender) == 2 || checker(sender) == 8)
            {
                Queen(sender);
            }
            else if (checker(sender) == 3 || checker(sender) == 9)
            {
                Bishop(sender);
            }
            else if (checker(sender) == 4 || checker(sender) == 10)
            {
                Knight(sender);
            }
            else if (checker(sender) == 5 || checker(sender) == 11 || checker(sender) == 13 || checker(sender) == 0)
            {
                Rook(sender);
            }
            else if (checker(sender) == 6 || checker(sender) == 12)
            {
                Pawn(sender);
            }
            else
            {
            }
        }
        private void High_light(int num)
        {
            for (int i = 0; i < 64; i++)
            {
                if (h_light[i] == true)
                {
                    numtobtn[i].BackColor = Color.FromArgb(first_color[i].R + 14, first_color[i].G - 110, first_color[i].B - 99);//zadan
                    if (exist[i] == -1)
                    {
                        numtobtn[i].BackColor = Color.FromArgb(first_color[i].R - 90, first_color[i].G + 38, first_color[i].B + 74);//khali
                        if (num != -1)
                            if (exist[num] == 6 || exist[num] == 12)
                                if (i - num == 7 || num - i == 7 || i - num == 9 || num - i == 9)
                                    numtobtn[i].BackColor = Color.FromArgb(first_color[num].R + 14, first_color[num].G - 110, first_color[num].B - 99); //en passant Hlight
                    }
                }
                else if (h_light_KR[i] == true)
                {
                    numtobtn[i].BackColor = Color.FromArgb(first_color[i].R - 100, first_color[i].G + 38, first_color[i].B);//shah ghale
                }
                else
                    numtobtn[i].BackColor = first_color[i]; // h_light false
            }
            if (kish(WhereIsKing()) == true)
                numtobtn[WhereIsKing()].BackColor = Color.DarkRed; //kish

        }
        private void move(int NewLoc, int OldLoc)
        {
            exist[NewLoc] = exist[OldLoc];
            exist[OldLoc] = -1;
            Thread t1 = new Thread(new ThreadStart(() =>
            {
                numtobtn[NewLoc].BackgroundImage = numtobtn[OldLoc].BackgroundImage;
                numtobtn[OldLoc].BackgroundImage = null;
            }));
            t1.Start();
            if (Turn == "W")
                Turn = "B";
            else
                Turn = "W";
            pawn_upgrading();

            Thread t2 = new Thread(new ThreadStart(() => { saveingMovement(OldLoc, NewLoc); }));
            t2.Start();

            movetime = 300;
        }
        private void King(Button btn)
        {
            int[,] move_position = new int[8, 2];
            int num = btn_number(btn);
            int row = btn_row(num);
            int column = btn_column(num);

            move_position[0, 0] = row;
            move_position[0, 1] = column - 1;

            move_position[1, 0] = row;
            move_position[1, 1] = column + 1;

            move_position[2, 0] = row - 1;
            move_position[2, 1] = column - 1;

            move_position[3, 0] = row - 1;
            move_position[3, 1] = column;

            move_position[4, 0] = row - 1;
            move_position[4, 1] = column + 1;

            move_position[5, 0] = row + 1;
            move_position[5, 1] = column - 1;

            move_position[6, 0] = row + 1;
            move_position[6, 1] = column;

            move_position[7, 0] = row + 1;
            move_position[7, 1] = column + 1;

            pos_checker(move_position, 8);
            exist_checker(num);

        }
        private void Queen(Button btn)
        {
            int[,] move_position = new int[30, 2];
            int num = btn_number(btn);
            int row = btn_row(num);
            int column = btn_column(num);
            int temp = 0;
            //Rook
            for (int i = temp, j = row + 1; j < 8; i++, j++)
            {
                if (WhatIsthis(row_column_to_btnnumber(j, column)) == null)
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(j, column)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                    break;
                }
            }

            for (int i = temp, j = row - 1; j >= 0; i++, j--)
            {
                if (WhatIsthis(row_column_to_btnnumber(j, column)) == null)
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(j, column)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, j = column + 1; j < 8; i++, j++)
            {
                if (WhatIsthis(row_column_to_btnnumber(row, j)) == null)
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(row, j)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, j = column - 1; j >= 0; i++, j--)
            {
                if (WhatIsthis(row_column_to_btnnumber(row, j)) == null)
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(row, j)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                    break;
                }
            }
            //Bishop
            for (int i = temp, c = column + 1, r = row + 1; c <= 7 && r <= 7; c++, r++, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }

            for (int i = temp, c = column - 1, r = row - 1; c >= 0 && r >= 0; c--, r--, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, c = column - 1, r = row + 1; c >= 0 && r <= 7; c--, r++, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, c = column + 1, r = row - 1; c <= 7 && r >= 0; c++, r--, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }
            pos_checker(move_position, temp);
        }
        private void Bishop(Button btn)
        {
            int[,] move_position = new int[15, 2];
            int num = btn_number(btn);
            int row = btn_row(num);
            int column = btn_column(num);
            int temp = 0;
            for (int i = temp, c = column + 1, r = row + 1; c <= 7 && r <= 7; c++, r++, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, c = column - 1, r = row - 1; c >= 0 && r >= 0; c--, r--, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, c = column - 1, r = row + 1; c >= 0 && r <= 7; c--, r++, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, c = column + 1, r = row - 1; c <= 7 && r >= 0; c++, r--, i++)
            {
                if (WhatIsthis(row_column_to_btnnumber(r, c)) == null)
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(r, c)))
                {
                    break;
                }
                else
                {
                    move_position[temp, 0] = r;
                    move_position[temp, 1] = c;
                    temp = i + 1;
                    break;
                }
            }

            pos_checker(move_position, temp);
        }
        private void Knight(Button btn)
        {
            int[,] move_position = new int[8, 2];
            int num = btn_number(btn);
            int row = btn_row(num);
            int column = btn_column(num);
            int temp = 0;
            bool Inboard = false;
            Inboard = row_column_checker(row + 1, column + 2);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row + 1, column + 2);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row + 1;
                    move_position[temp, 1] = column + 2;
                    temp = temp + 1;
                }
            }
            Inboard = row_column_checker(row - 1, column + 2);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row - 1, column + 2);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row - 1;
                    move_position[temp, 1] = column + 2;
                    temp += 1;
                }
            }
            Inboard = row_column_checker(row + 1, column - 2);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row + 1, column - 2);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row + 1;
                    move_position[temp, 1] = column - 2;
                    temp += 1;
                }
            }
            Inboard = row_column_checker(row - 1, column - 2);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row - 1, column - 2);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row - 1;
                    move_position[temp, 1] = column - 2;
                    temp += 1;
                }
            }
            Inboard = row_column_checker(row + 2, column + 1);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row + 2, column + 1);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row + 2;
                    move_position[temp, 1] = column + 1;
                    temp += 1;
                }
            }
            Inboard = row_column_checker(row + 2, column - 1);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row + 2, column - 1);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {

                    move_position[temp, 0] = row + 2;
                    move_position[temp, 1] = column - 1;
                    temp += 1;
                }
            }
            Inboard = row_column_checker(row - 2, column + 1);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row - 2, column + 1);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row - 2;
                    move_position[temp, 1] = column + 1;
                    temp += 1;
                }
            }
            Inboard = row_column_checker(row - 2, column - 1);
            if (Inboard)
            {
                int number = row_column_to_btnnumber(row - 2, column - 1);
                if (WhatIsthis(number) == null || IsFriend(num, number) == false)
                {
                    move_position[temp, 0] = row - 2;
                    move_position[temp, 1] = column - 1;
                    temp += 1;
                }
            }
            pos_checker(move_position, temp);
        }
        private void Rook(Button btn)
        {
            int[,] move_position = new int[15, 2];
            int num = btn_number(btn);
            int row = btn_row(num);
            int column = btn_column(num);
            int temp = 0;
            for (int i = temp, j = row + 1; j < 8; i++, j++)
            {
                if (WhatIsthis(row_column_to_btnnumber(j, column)) == null)
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(j, column)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                    break;
                }
            }

            for (int i = temp, j = row - 1; j >= 0; i++, j--)
            {
                if (WhatIsthis(row_column_to_btnnumber(j, column)) == null)
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(j, column)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = j;
                    move_position[i, 1] = column;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, j = column + 1; j < 8; i++, j++)
            {
                if (WhatIsthis(row_column_to_btnnumber(row, j)) == null)
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(row, j)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                    break;
                }
            }
            for (int i = temp, j = column - 1; j >= 0; i++, j--)
            {
                if (WhatIsthis(row_column_to_btnnumber(row, j)) == null)
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                }
                else if (IsFriend(num, row_column_to_btnnumber(row, j)))
                {
                    break;
                }
                else
                {
                    move_position[i, 0] = row;
                    move_position[i, 1] = j;
                    temp = i + 1;
                    break;
                }
            }
            pos_checker(move_position, temp);
        }
        private void Pawn(Button btn)
        {
            int[,] move_position = new int[4, 2];
            int num = btn_number(btn);
            int row = btn_row(num);
            int column = btn_column(num);
            int temp = 0;
            if (WhatIsthis(num) == "W")
            {
                bool InBoard = row_column_checker(row + 1, column);
                if (InBoard)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row + 1, column)) == null)
                    {
                        move_position[temp, 0] = row + 1;
                        move_position[temp, 1] = column;
                        temp = 1 + temp;
                        // first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row + 2, column);
                if (InBoard == true && first_pawn_move[num] == true && exist[row_column_to_btnnumber(row + 1, column)] == -1)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row + 2, column)) == null)
                    {
                        move_position[temp, 0] = row + 2;
                        move_position[temp, 1] = column;
                        temp = 1 + temp;
                        // first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row + 1, column + 1);
                if (InBoard)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row + 1, column + 1)) == "B")
                    {
                        move_position[temp, 0] = row + 1;
                        move_position[temp, 1] = column + 1;
                        temp = 1 + temp;
                        // first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row + 1, column - 1);
                if (InBoard)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row + 1, column - 1)) == "B")
                    {
                        move_position[temp, 0] = row + 1;
                        move_position[temp, 1] = column - 1;
                        temp = 1 + temp;
                        //  first_pawn_move[num] = false;
                    }
                }
                //onpasan
                InBoard = row_column_checker(row, column + 1);
                if (InBoard)
                {
                    if (exist[row_column_to_btnnumber(row, column + 1)] == 12 && pawnmoved[row_column_to_btnnumber(row, column + 1)] == true)
                    {
                        move_position[temp, 0] = row + 1;
                        move_position[temp, 1] = column + 1;
                        temp = 1 + temp;
                        //  first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row, column - 1);
                if (InBoard)
                {
                    if (exist[row_column_to_btnnumber(row, column - 1)] == 12 && pawnmoved[row_column_to_btnnumber(row, column - 1)] == true)
                    {
                        move_position[temp, 0] = row + 1;
                        move_position[temp, 1] = column - 1;
                        temp = 1 + temp;
                        //  first_pawn_move[num] = false;
                    }
                }
            }
            if (WhatIsthis(num) == "B")
            {
                bool InBoard = row_column_checker(row - 1, column);
                if (InBoard)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row - 1, column)) == null)
                    {
                        move_position[temp, 0] = row - 1;
                        move_position[temp, 1] = column;
                        temp = 1 + temp;
                        //   first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row - 2, column);
                if (InBoard == true && first_pawn_move[num] == true && exist[row_column_to_btnnumber(row - 1, column)] == -1)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row - 2, column)) == null)
                    {
                        move_position[temp, 0] = row - 2;
                        move_position[temp, 1] = column;
                        temp = 1 + temp;
                        //   first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row - 1, column + 1);
                if (InBoard)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row - 1, column + 1)) == "W")
                    {
                        move_position[temp, 0] = row - 1;
                        move_position[temp, 1] = column + 1;
                        temp = 1 + temp;
                        //  first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row - 1, column - 1);
                if (InBoard)
                {
                    if (WhatIsthis(row_column_to_btnnumber(row - 1, column - 1)) == "W")
                    {
                        move_position[temp, 0] = row - 1;
                        move_position[temp, 1] = column - 1;
                        temp = 1 + temp;
                        // first_pawn_move[num] = false;
                    }
                }
                //onpasan
                InBoard = row_column_checker(row, column + 1);
                if (InBoard)
                {
                    if (exist[row_column_to_btnnumber(row, column + 1)] == 6 && pawnmoved[row_column_to_btnnumber(row, column + 1)] == true)
                    {
                        move_position[temp, 0] = row - 1;
                        move_position[temp, 1] = column + 1;
                        temp = 1 + temp;
                        //  first_pawn_move[num] = false;
                    }
                }
                InBoard = row_column_checker(row, column - 1);
                if (InBoard)
                {
                    if (exist[row_column_to_btnnumber(row, column - 1)] == 6 && pawnmoved[row_column_to_btnnumber(row, column - 1)] == true)
                    {
                        move_position[temp, 0] = row - 1;
                        move_position[temp, 1] = column - 1;
                        temp = 1 + temp;
                        //  first_pawn_move[num] = false;
                    }
                }
            }
            pos_checker(move_position, temp);
        }
        private int btn_number(Button btn)
        {
            int num = Convert.ToInt32(btn.Name.Replace("b", ""));
            // MessageBox.Show(btn.Name);
            return num;
        }
        private int checker(Button btn)
        {
            return exist[btn_number(btn)];
        }
        private int row_column_to_btnnumber(int row, int column)
        {
            return (row * 8) + column;
        }
        private int btn_column(int number)
        {
            return number % 8;
        }
        private int btn_row(int number)
        {
            return number / 8;
        }
        private void pos_checker(int[,] pos, int n)
        {
            for (int i = 0; i < n; i++)
                if (pos[i, 0] <= 7 && pos[i, 0] >= 0 && pos[i, 1] <= 7 && pos[i, 1] >= 0)
                {
                    h_light[row_column_to_btnnumber(pos[i, 0], pos[i, 1])] = true;
                }
        }
        private bool row_column_checker(int row, int column)
        {
            if (row <= 7 && row >= 0 && column <= 7 && column >= 0)
                return true;
            else
                return false;
        }
        private void exist_checker(int num)
        {
            for (int i = 0; i < 64; i++)
            {
                if (h_light[i] == true)
                {
                    if (IsFriend(i, num))
                        h_light[i] = false;
                }
            }
        }
        private string WhatIsthis(int num)
        {
            int number = exist[num];
            string result = "0000";
            if (number >= 7 && number <= 13)
            {
                return "B";
            }
            else if (number <= 6 && number >= 0)
            {
                return "W";
            }
            else if (number == -1)
            {
                return null;
            }
            return result;
        }
        private bool IsFriend(int btn_num1, int btn_num2)
        {
            return WhatIsthis(btn_num1) == WhatIsthis(btn_num2);
        }
        private void default_h_light()
        {
            for (int i = 0; i < 64; i++)
            {
                h_light[i] = false;
                h_light_KR[i] = false;
            }
        }
        private bool kish(int kingloc)
        {
            bool[] f_h_light = new bool[64];
            bool[] f_h_light_KR = new bool[64];
            for (int i = 0; i < 64; i++)
            {
                f_h_light[i] = h_light[i];
                f_h_light_KR[i] = h_light_KR[i];
            }
            for (int i = 0; i < 64; i++)
            {
                if (WhatIsthis(i) != Turn && WhatIsthis(i) != null)
                {
                    BefMove(numtobtn[i]);
                    if (h_light[kingloc] == true)
                    {
                        for (int j = 0; j < 64; j++)
                        {
                            h_light[j] = f_h_light[j];
                            h_light_KR[j] = f_h_light_KR[j];
                        }
                        return true;
                    }
                }
            }
            for (int i = 0; i < 64; i++)
            {
                h_light[i] = f_h_light[i];
                h_light_KR[i] = f_h_light_KR[i];
            }
            return false;
        }
        private int WhereIsKing()
        {
            if (Turn == "W")
            {
                for (int i = 0; i < 64; i++)
                {
                    if (exist[i] == 1)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < 64; i++)
                {
                    if (exist[i] == 7)
                        return i;
                }
            }
            return -1;
        }
        private int KishOMat()
        {
            bool[] f_h_light = new bool[64];
            for (int i = 0; i < 64; i++)
            {
                f_h_light[i] = h_light[i];
            }
            for (int j = 0; j < 64; j++)
            {
                if (WhatIsthis(j) == Turn)
                {
                    BefMove(numtobtn[j]);
                    int firstexist = exist[j];
                    for (int i = 0; i < 64; i++)
                    {
                        if (h_light[i])
                        {
                            int secendexist = exist[i];
                            exist[j] = -1;
                            exist[i] = firstexist;
                            if (kish(WhereIsKing()) == true)
                            {
                                h_light[i] = false;
                            }
                            exist[i] = secendexist;
                            exist[j] = firstexist;
                        }
                    }
                    for (int i = 0; i < 64; i++)
                    {
                        if (h_light[i])
                        {
                            for (int h = 0; h < 64; h++)
                            {
                                h_light[h] = f_h_light[h];
                            }
                            return 0;
                        }
                    }
                }
            }
            for (int i = 0; i < 64; i++)
            {
                h_light[i] = f_h_light[i];
            }
            if (kish(WhereIsKing()) == true)
            {
                return 1;
            }
            else return -1;
        }
        private void EndGame(bool kishyapat)
        {
            for (int i = 0; i < 64; i++)
            {
                numtobtn[i].Enabled = false;
            }
            if (Turn == "W")
            {
                if (kishyapat)
                {
                    updatesql(1, 0, 0, score[0]);
                    Turn = "B";
                    updatesql(0, 1, 0, score[2]);
                    Turn = "W";
                    MessageBox.Show(IDInfo(1, ids[1]) + " (Black) won!");
                }
                else
                {
                    updatesql(0, 0, 1, score[1]);
                    Turn = "B";
                    updatesql(0, 0, 1, score[1]);
                    Turn = "W";
                    MessageBox.Show("Draw", "Pot");
                }
                //Application.Restart();
                Restart();

            }
            else
            {
                if (kishyapat)
                {
                    updatesql(1, 0, 0, score[0]);
                    Turn = "W";
                    updatesql(0, 1, 0, score[2]);
                    Turn = "B";
                    MessageBox.Show(IDInfo(1, ids[0]) + " (White) won!");
                }
                else
                {
                    updatesql(0, 0, 1, score[1]);
                    Turn = "W";
                    updatesql(0, 0, 1, score[1]);
                    Turn = "B";
                    MessageBox.Show("Draw", "Pot");
                }
                //Application.Restart();
                Restart();
            }
        }
        private void Restart()
        {
            if (!nGame)
            {
                DialogResult dialogResult = MessageBox.Show("Restart game", "Again?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    load_restart();
                    restart = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    this.Close();
                }
            }
            else
            {

                this.Close();
            }
        }
        private void set_img()
        {
            for (int i = 0; i < 64; i++)
            {
                numtobtn[i].BackgroundImage = null;
                for (int j = 0; j <= 13; j++)
                {
                    if (exist[i] == j)
                        numtobtn[i].BackgroundImage = images[j];
                }
            }
        }
        private void set_Images()
        {
            //white
            if (images[1] == null)
            {
                images[1] = b4.BackgroundImage;   //king
                images[2] = b3.BackgroundImage;   //queen
                images[3] = b2.BackgroundImage;   //bishop
                images[4] = b1.BackgroundImage;   //knight
                images[5] = b0.BackgroundImage;   //rook big
                images[6] = b8.BackgroundImage;   //pawn
                images[0] = b7.BackgroundImage;   //rook small

                //black
                images[7] = b60.BackgroundImage;   //king
                images[8] = b59.BackgroundImage;   //queen
                images[9] = b58.BackgroundImage;   //bishop
                images[10] = b57.BackgroundImage;   //knight
                images[11] = b56.BackgroundImage;   //rook
                images[12] = b55.BackgroundImage;   //pawn
                images[13] = b63.BackgroundImage;   //rook big
            }
        }
        private void set_numtobtn()
        {
            numtobtn[0] = b0;
            numtobtn[1] = b1;
            numtobtn[2] = b2;
            numtobtn[3] = b3;
            numtobtn[4] = b4;
            numtobtn[5] = b5;
            numtobtn[6] = b6;
            numtobtn[7] = b7;
            numtobtn[8] = b8;
            numtobtn[9] = b9;
            numtobtn[10] = b10;
            numtobtn[11] = b11;
            numtobtn[12] = b12;
            numtobtn[13] = b13;
            numtobtn[14] = b14;
            numtobtn[15] = b15;
            numtobtn[16] = b16;
            numtobtn[17] = b17;
            numtobtn[18] = b18;
            numtobtn[19] = b19;
            numtobtn[20] = b20;
            numtobtn[21] = b21;
            numtobtn[22] = b22;
            numtobtn[23] = b23;
            numtobtn[24] = b24;
            numtobtn[25] = b25;
            numtobtn[26] = b26;
            numtobtn[27] = b27;
            numtobtn[28] = b28;
            numtobtn[29] = b29;
            numtobtn[30] = b30;
            numtobtn[31] = b31;
            numtobtn[32] = b32;
            numtobtn[33] = b33;
            numtobtn[34] = b34;
            numtobtn[35] = b35;
            numtobtn[36] = b36;
            numtobtn[37] = b37;
            numtobtn[38] = b38;
            numtobtn[39] = b39;
            numtobtn[40] = b40;
            numtobtn[41] = b41;
            numtobtn[42] = b42;
            numtobtn[43] = b43;
            numtobtn[44] = b44;
            numtobtn[45] = b45;
            numtobtn[46] = b46;
            numtobtn[47] = b47;
            numtobtn[48] = b48;
            numtobtn[49] = b49;
            numtobtn[50] = b50;
            numtobtn[51] = b51;
            numtobtn[52] = b52;
            numtobtn[53] = b53;
            numtobtn[54] = b54;
            numtobtn[55] = b55;
            numtobtn[56] = b56;
            numtobtn[57] = b57;
            numtobtn[58] = b58;
            numtobtn[59] = b59;
            numtobtn[60] = b60;
            numtobtn[61] = b61;
            numtobtn[62] = b62;
            numtobtn[63] = b63;
        }
        private void set_firstcolor()
        {
            if (!restart)
            {
                for (int i = 0; i < 64; i++)
                    first_color[i] = numtobtn[i].BackColor;
            }
        }
        private void set_firstexist()
        {
            for (int i = 0; i < 64; i++)
                exist[i] = -1;
            //White
            exist[0] = 5; //Rook Big
            exist[1] = 4; //Knight
            exist[2] = 3; //Bishop
            exist[3] = 2; //Queen
            exist[4] = 1; //King
            exist[5] = 3;
            exist[6] = 4;
            exist[7] = 0;//Rook Small
            //Black
            exist[56] = 11;//Rook Big
            exist[57] = 10;//Knight
            exist[58] = 9; //Bishop
            exist[59] = 8; //Queen
            exist[60] = 7; //King
            exist[61] = 9;
            exist[62] = 10;
            exist[63] = 13;//Rook Small
            for (int i = 0; i < 64; i++)  //Pawn
            {
                if ((i >= 8 && i <= 15))
                {
                    exist[i] = 6;
                    first_pawn_move[i] = true;
                    pawnmoved[i] = false;
                }
                else if (i >= 48 && i <= 55)
                {
                    exist[i] = 12;
                    first_pawn_move[i] = true;
                    pawnmoved[i] = false;
                }
                else
                {
                    first_pawn_move[i] = false;
                    pawnmoved[i] = false;
                }
            }
        }
        private void set_moved()
        {
            w_king_Moved = false;
            b_king_Moved = false;
            w_big_rook_Moved = false;
            b_big_rook_Moved = false;
            w_small_rook_Moved = false;
            b_small_rook_Moved = false;
        }
        private void checkingFirstMove(int num)
        {
            if (exist[num] == 1)
                if (w_king_Moved == false)
                    w_king_Moved = true;
            if (exist[num] == 7)
                if (b_king_Moved == false)
                    b_king_Moved = true;
            if (exist[num] == 5)
                if (w_big_rook_Moved == false)
                    w_big_rook_Moved = true;
            if (exist[num] == 11)
                if (b_big_rook_Moved == false)
                    b_big_rook_Moved = true;
            if (exist[num] == 13)
                if (b_small_rook_Moved == false)
                    b_small_rook_Moved = true;
            if (exist[num] == 0)
                if (w_small_rook_Moved == false)
                    w_small_rook_Moved = true;
        }
        private int can_king_rook(bool Big) //returns =>   0 = cant king rook | 1 = big rook | 2 = small rook || Inputs => 0 = big | 1=small
        {
            if (Turn == "W" && w_king_Moved == false)
            {
                if (w_big_rook_Moved == false && Big)
                {
                    return 1;
                }
                if (w_small_rook_Moved == false && !Big)
                    return 2;
            }
            if (Turn == "B" && b_king_Moved == false)
            {
                if (b_big_rook_Moved == false && Big)
                    return 1;
                if (b_small_rook_Moved == false && !Big)
                    return 2;
            }
            return 0;
        }
        private void king_rook_highlight()
        {
            if (Turn == "W")
            {
                if (can_king_rook(true) == 1)
                {
                    if (Kishe(2) == false && Kishe(3) == false && exist[2] == -1 && exist[3] == -1 && exist[1] == -1)
                        h_light_KR[WhereIsKing() - 2] = true;
                }
                if (can_king_rook(false) == 2)
                {
                    if (Kishe(5) == false && Kishe(6) == false && exist[5] == -1 && exist[6] == -1)
                        h_light_KR[6] = true;
                }
            }
            else
            {
                if (can_king_rook(false) == 2)
                {
                    if (Kishe(WhereIsKing() - 1) == false && Kishe(WhereIsKing() - 2) == false && exist[WhereIsKing() - 1] == -1 && exist[WhereIsKing() - 2] == -1 && exist[WhereIsKing() - 3] == -1)
                        h_light_KR[WhereIsKing() - 2] = true;
                }
                if (can_king_rook(true) == 1)
                {
                    if (Kishe(WhereIsKing() + 1) == false && Kishe(WhereIsKing() + 2) == false && exist[WhereIsKing() + 1] == -1 && exist[WhereIsKing() + 2] == -1)
                        h_light_KR[WhereIsKing() + 2] = true;
                }
            }
        }
        private bool Kishe(int loc)
        {
            bool[] temp = new bool[64];
            for (int i = 0; i < 64; i++)
            {
                temp[i] = h_light_KR[i];
            }
            int firstexist = exist[WhereIsKing()];
            int wwk = WhereIsKing();
            int secendexist = exist[loc];
            exist[WhereIsKing()] = -1;
            exist[loc] = firstexist;
            if (kish(wwk) == true)
            {
                exist[loc] = secendexist;
                exist[wwk] = firstexist;
                for (int i = 0; i < 64; i++)
                {
                    h_light_KR[i] = temp[i];
                }
                return true;
            }
            else
            {
                exist[loc] = secendexist;
                exist[wwk] = firstexist;
                for (int i = 0; i < 64; i++)
                {
                    h_light_KR[i] = temp[i];
                }
                return false;
            }
        }
        private void en_passant_Move(int num)
        {
            if (Last_Loc - num == 16 || num - Last_Loc == 16)
                pawnmoved[num] = true;
            else
                pawnmoved[num] = false;
            if (Last_Loc - num == 7 || num - Last_Loc == 7 || Last_Loc - num == 9 || num - Last_Loc == 9)
            {
                if (exist[num] == -1)
                {
                    if (Turn == "W")
                    {
                        exist[num - 8] = -1;
                        numtobtn[num - 8].BackgroundImage = null;
                    }
                    if (Turn == "B")
                    {
                        exist[num + 8] = -1;
                        numtobtn[num + 8].BackgroundImage = null;
                    }
                }
            }
        }
        private void King_Rook_Move(int num)
        {
            if (Turn == "W")
            {
                if (num == WhereIsKing() - 2)
                {
                    exist[WhereIsKing() - 1] = exist[0];
                    exist[0] = -1;
                    numtobtn[WhereIsKing() - 1].BackgroundImage = images[exist[WhereIsKing() - 1]];
                    numtobtn[0].BackgroundImage = null;
                }
                else
                {
                    exist[WhereIsKing() + 1] = exist[7];
                    exist[7] = -1;
                    numtobtn[WhereIsKing() + 1].BackgroundImage = images[exist[WhereIsKing() + 1]];
                    numtobtn[7].BackgroundImage = null;
                }
            }
            else
            {
                if (num == WhereIsKing() - 2)
                {
                    exist[WhereIsKing() - 1] = exist[WhereIsKing() - 4];
                    exist[WhereIsKing() - 4] = -1;
                    numtobtn[WhereIsKing() - 1].BackgroundImage = images[exist[WhereIsKing() - 1]];
                    numtobtn[WhereIsKing() - 4].BackgroundImage = null;
                }
                else
                {
                    exist[WhereIsKing() + 1] = exist[63];
                    exist[63] = -1;
                    numtobtn[WhereIsKing() + 1].BackgroundImage = images[exist[WhereIsKing() + 1]];
                    numtobtn[63].BackgroundImage = null;
                }
            }
        }
        private void Correct_Hlight(int num)
        {
            int firstexist = exist[num];
            for (int i = 0; i < 64; i++)
            {
                if (h_light[i])
                {
                    int secendexist = exist[i];
                    exist[num] = -1;
                    exist[i] = firstexist;
                    if (kish(WhereIsKing()) == true)
                    {
                        h_light[i] = false;
                    }
                    exist[i] = secendexist;
                    exist[num] = firstexist;
                }
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Exit game", "Are u sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }
        }
        //save & load
        private void save(string name)
        {
            Thread t1 = new Thread(new ThreadStart(() =>
            {
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
                bool b = false;//!(name == "default" || name == "quicksave" || name == "autosave");
                writer.Write(savestr(b));
                writer.Close();
                fs.Close();
                writer.Dispose();
                fs.Dispose();
                GC.Collect();
            }));
            t1.Start();
        }
        public void load(string name)
        {
            string[] exist_str = new string[64];
            string save_str = "";
            OpenFileDialog res = new OpenFileDialog();
            res.Filter = "sv Files|*.sv;*";
            string address = Path.GetFullPath("./") + @"saves\" + name + ".sv";
            if (!Directory.Exists(Path.GetDirectoryName(address)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(address));
            }
            res.FileName = address;
            Stream fs = res.OpenFile();
            StreamReader reader = new StreamReader(fs);
            save_str = reader.ReadToEnd();
            reader.Close();
            exist_str = save_str.Split('|');
            for (int i = 0; i < 64; i++)
            {
                exist[i] = Convert.ToInt32(exist_str[i]);
            }
            for (int i = 0; i < 64; i++)
            {
                pawnmoved[i] = Convert.ToBoolean(save_str.Split('&')[1].Split('_')[i]);
            }
            Turn = save_str.Split(';')[1].Split('>')[0];
            Last_Loc = Convert.ToInt32(save_str.Split('>')[1].Split('<')[0]);
            w_king_Moved = Convert.ToBoolean(save_str.Split('<')[1].Split('}')[0]);
            b_king_Moved = Convert.ToBoolean(save_str.Split('<')[1].Split('}')[1]);
            w_big_rook_Moved = Convert.ToBoolean(save_str.Split('<')[1].Split('}')[2]);
            b_big_rook_Moved = Convert.ToBoolean(save_str.Split('<')[1].Split('}')[3]);
            w_small_rook_Moved = Convert.ToBoolean(save_str.Split('<')[1].Split('}')[4]);
            b_small_rook_Moved = Convert.ToBoolean(save_str.Split('<')[1].Split('}')[5].Split('(')[0]);
            white_gametime = Convert.ToInt32(save_str.Split('(')[1]);
            black_gametime = Convert.ToInt32(save_str.Split('(')[2]);
            movetime = Convert.ToInt32(save_str.Split('^')[0].Split('(')[3]);
            set_img();
            default_h_light();
            High_light(-1);
            fs.Close();
        }
        private Image screenimg()
        {
            //string address = Path.GetFullPath("./") + @"saves\" + name + ".svim";
            Bitmap controlBitMap = new Bitmap(panel1.Width, panel1.Height);
            Graphics g = Graphics.FromImage(controlBitMap);
            g.CopyFromScreen(PointToScreen(panel1.Location), new Point(0, 0), panel1.Size);
            return controlBitMap;
            //Image img = controlBitMap;
            //img.Save(address,ImageFormat.Png);
            //controlBitMap.Save(address, ImageFormat.Png);
            /*using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(address, FileMode.Create, FileAccess.ReadWrite))
                {
                    controlBitMap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
                memory.Close();
            }
            g.Dispose();
            controlBitMap.Dispose();
            GC.Collect();*/
        }
        private void loadLatestGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /* bool same = true;
             Thread t2 = new Thread(new ThreadStart(() => {

                 int[] temp = new int[64];
                 for (int i = 0; i < 64; i++)
                 {
                     temp[i] = exist[i];
                 }
                 save("test");
                 load("default");
                 for (int i = 0; i < 64; i++)
                 {
                     if (temp[i] != exist[i])
                     {
                         same = false;
                         break;
                     }
                 }
                 load("test");
                 File.Delete(Path.GetFullPath("./") + @"saves\test.sv");
             }));
             t2.Start();*/
            if (!nGame)
            {
                Thread t1 = new Thread(new ThreadStart(() =>
                {
                    // bool rep ;
                    if (File.Exists(Path.GetFullPath("./") + @"saves\autosave1.sv"))
                    {
                        load("autosave1");
                        File.Delete(Path.GetFullPath("./") + @"saves\autosave3.sv");
                        File.Move(Path.GetFullPath("./") + @"saves\autosave1.sv", Path.GetFullPath("./") + @"saves\autosave3.sv");
                        File.Delete(Path.GetFullPath("./") + @"saves\autosave1.sv");
                        File.Move(Path.GetFullPath("./") + @"saves\autosave.sv", Path.GetFullPath("./") + @"saves\autosave1.sv");
                        File.Delete(Path.GetFullPath("./") + @"saves\autosave.sv");
                        File.Move(Path.GetFullPath("./") + @"saves\autosave3.sv", Path.GetFullPath("./") + @"saves\autosave.sv");
                        // rep = true;
                    }
                    else MessageBox.Show("No Data");
                    /*{
                        rep = false;
                    }
                    if(!rep)
                    {
                        t2.Join();
                        if (!same)
                        {
                            load("default");
                        }
                        else
                        {
                           if(File.Exists(Path.GetFullPath("./") + @"saves\autosave.sv"))
                            {
                                load("autosave");
                            }
                            else
                                MessageBox.Show("No Data");
                        }
                    }*/
                }));
                t1.Start();
            }
            else
            {
                MessageBox.Show("You cant do this in multiplayer game :) ");
            }
        }
        private void quickLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!nGame)
            {
                Thread t1 = new Thread(new ThreadStart(() =>
            {
                load("quicksave");
            }));
                t1.Start();
            }
            else
            {
                MessageBox.Show("You cant do this in multiplayer game :) ");
            }
        }
        private void quickSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread t1 = new Thread(new ThreadStart(() =>
              {
                  save("quicksave");
              }));
            t1.Start();
        }
        private void manualSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChessSaveForm saveform = new ChessSaveForm(savestr(true));
            saveform.Show();
        }
        private string savestr(bool imgneed)
        {
            string result = "";
            for (int i = 0; i < 64; i++)
            {
                result += (Convert.ToString(exist[i]) + "|");
            }
            result += ("&");
            for (int i = 0; i < 64; i++)
            {
                result += (Convert.ToString(pawnmoved[i]) + "_");
            }
            result += (";");
            result += (Convert.ToString(Turn));
            result += (">");
            result += (Convert.ToString(Last_Loc) + "<");
            result += (Convert.ToString(w_king_Moved) + "}");
            result += (Convert.ToString(b_king_Moved) + "}");
            result += (Convert.ToString(w_big_rook_Moved) + "}");
            result += (Convert.ToString(b_big_rook_Moved) + "}");
            result += (Convert.ToString(w_small_rook_Moved) + "}");
            result += (Convert.ToString(b_small_rook_Moved) + "(");
            result += (Convert.ToString(white_gametime) + "(");
            result += (Convert.ToString(black_gametime) + "(");
            result += (Convert.ToString(movetime));
            result += ("^");
            if (imgneed)
                result += (ImageToBase64String(screenimg(), ImageFormat.Png));
            return result;
        }
        private void manualLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!nGame)
            {
                LoadForm loadform = new LoadForm(this);
                loadform.Show();
            }
            else
            {
                MessageBox.Show("You cant do this in multiplayer game :) ");
            }
        }
        public string ImageToBase64String(Image image, ImageFormat format)
        {
            MemoryStream memory = new MemoryStream();
            image.Save(memory, format);
            string base64 = Convert.ToBase64String(memory.ToArray());
            memory.Close();
            return base64;
        }
        //pawnupgrading
        private void pawn_upgrading()
        {
            if (Turn == "B")
            {
                for (int i = 56; i < 64; i++)
                {
                    if (exist[i] == 6)
                    {
                        this.Enabled = false;
                        PawnUpgradingForm obj = new PawnUpgradingForm(i, this);
                        obj.Show();

                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (exist[i] == 12)
                    {
                        this.Enabled = false;
                        PawnUpgradingForm obj = new PawnUpgradingForm(i, this);
                        obj.Show();

                        break;
                    }
                }
            }


        }
        public void SetPawnUpgradeImage(int clicked, int location)
        {
            if (nGame)
                Sendmsg(location + "|" + clicked);
            if (clicked == 1)
            {
                if (Turn == "B")
                {
                    numtobtn[location].BackgroundImage = images[5];
                    exist[location] = 5;
                }
                else
                {
                    numtobtn[location].BackgroundImage = images[11];
                    exist[location] = 11;
                }
            }
            else if (clicked == 2)
            {
                if (Turn == "B")
                {
                    numtobtn[location].BackgroundImage = images[4];
                    exist[location] = 4;
                }
                else
                {
                    numtobtn[location].BackgroundImage = images[10];
                    exist[location] = 10;
                }
            }
            else if (clicked == 3)
            {
                if (Turn == "B")
                {
                    numtobtn[location].BackgroundImage = images[3];
                    exist[location] = 3;
                }
                else
                {
                    numtobtn[location].BackgroundImage = images[9];
                    exist[location] = 9;
                }
            }
            else if (clicked == 4)
            {
                if (Turn == "B")
                {
                    numtobtn[location].BackgroundImage = images[2];
                    exist[location] = 2;
                }
                else
                {
                    numtobtn[location].BackgroundImage = images[8];
                    exist[location] = 8;
                }
            }
            High_light(-1);
        }
        //timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            if (gamestarted)
            {
                if (Turn == "W")
                {
                    movetime -= 1;
                    white_gametime -= 1;
                }
                else
                {
                    movetime -= 1;
                    black_gametime -= 1;
                }
                gametimeblack.Text = "WhiteGametime " + Convert.ToString(TimeSpan.FromSeconds(white_gametime));
                gametimewhite.Text = "BlackGametime " + Convert.ToString(TimeSpan.FromSeconds(black_gametime));
                timelbl.Text = "Movetime " + Convert.ToString(TimeSpan.FromSeconds(movetime));

                if (movetime == 0)
                {
                    if (Turn == "W")
                        Turn = "B";
                    else
                        Turn = "W";

                    timer1.Enabled = false;
                    MessageBox.Show("Your move time ended and you lost your turn!!");
                    timer1.Enabled = true;
                    movetime = 300;
                }
                if (white_gametime == 0)
                {
                    timer1.Enabled = false;
                    EndGame(true);
                }

                else if (black_gametime == 0)
                {
                    timer1.Enabled = false;
                    EndGame(false);
                }
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (gamestarted&&nGame)
            {
                string msg = Recivemsg();
                msg = originalStr(msg.Trim());
                if (msg != "")
                {
                    if ((Turn == "B" && hGame) || (Turn == "W" && !hGame))
                    {
                        if (msg.Contains("="))
                        {
                            msg = msg.Replace("=", "");
                            Last_Loc = Convert.ToInt32(msg.Split('_')[0]);
                            int num = Convert.ToInt32(msg.Split('_')[1]);

                            BefMove(numtobtn[Last_Loc]);
                            Correct_Hlight(Last_Loc);
                            if (checker(numtobtn[Last_Loc]) == 1 || checker(numtobtn[Last_Loc]) == 7)
                                king_rook_highlight();
                            High_light(Last_Loc);
                            if (h_light_KR[num])
                            {
                                King_Rook_Move(num);
                            }
                            if (exist[Last_Loc] == 6 || exist[Last_Loc] == 12)
                            {
                                en_passant_Move(num);
                            }
                            move(num, Last_Loc);
                            default_h_light();
                            High_light(num);
                            if (KishOMat() == 1)
                            {
                                EndGame(true);
                            }
                            else if (KishOMat() == -1)
                            {
                                EndGame(false);
                            }
                            checkingFirstMove(num);
                        }
                        else if (msg.Contains('|'))
                        {
                            SetPawnUpgradeImage(Convert.ToInt32(msg.Split('|')[1]), Convert.ToInt32(msg.Split('|')[0]));
                        }
                    }
                    if (msg == "0")
                    {
                        gamestarted = false;
                        if (KishOMat() == 0)
                        {
                            Turn = (Turn == "W" ? "B" : "W");
                            if (KishOMat() == 0)
                            {
                                Turn = (hGame) ? "B" : "W";
                                ended = true;
                                EndGame(true);
                            }
                        }
                        this.Close();
                    }
                }
            }
        }
        //closing
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            loading = false;
            if (nGame)
            {
                
                if (!ended)
                {
                    Sendmsg("0");
                    Thread.Sleep(1000);
                }
                if(hGame)
                    server.Server.Close();
                client.Client.Close();
                stream.Close();
                room.gameclosed();
            }
            else
            {
                mm.Show();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//123 when closing
        {

        }
        private string coloumnnumtochar(int coloumn)
        {
            switch (coloumn)
            {
                case 0:
                    return "A";
                case 1:
                    return "B";
                case 2:
                    return "C";
                case 3:
                    return "D";
                case 4:
                    return "E";
                case 5:
                    return "F";
                case 6:
                    return "G";
                case 7:
                    return "H";
            }
            return "";
        }
        private string What(int loc)
        {
            switch (exist[loc])
            {
                case 0:
                    return "White Rook";
                case 1:
                    return "White King";
                case 2:
                    return "White Queen";
                case 3:
                    return "White Bishop";
                case 4:
                    return "White Knight";
                case 5:
                    return "White Rook";
                case 6:
                    return "White Pawn";
                case 7:
                    return "Black king";
                case 8:
                    return "Black Queen";
                case 9:
                    return "Black Bishop";
                case 10:
                    return "Black Knight";
                case 11:
                    return "Black Rook";
                case 12:
                    return "Black Pawn";
            }
            return "Black Rook";
        }
        //sqlite
        private void SQLConection()
        {
            string cs = @"URI=file:Chess.db";
            con = new SQLiteConnection(cs);
            con.Open();
        }
        private void createTableAndColoumn_Movements()
        {
            var cmd = new SQLiteCommand(con);
            cmd.CommandText = "DROP TABLE IF EXISTS movements";
            cmd.ExecuteNonQuery();
            cmd.CommandText = @"CREATE TABLE movements(id INTEGER PRIMARY KEY,
                    az TEXT, be TEXT, mohreh TEXT)";
            cmd.ExecuteNonQuery();
        }
        private void saveingMovement(int firstloc, int secendloc)
        {
            var cmd = new SQLiteCommand(con);
            int frow = btn_row(firstloc) + 1;
            int fcol = btn_column(firstloc);
            int srow = btn_row(secendloc) + 1;
            int scol = btn_column(secendloc);
            string firstlocstr = coloumnnumtochar(fcol) + Convert.ToString(frow);
            string secendlocstr = coloumnnumtochar(scol) + Convert.ToString(srow);
            cmd.CommandText = "INSERT INTO movements(az, be, mohreh) VALUES('" + firstlocstr + "','" + secendlocstr + "','" + What(secendloc) + "')";
            cmd.ExecuteNonQuery();
        }
        private void playersID()
        {
            string stm = "SELECT * FROM scores";
            var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr.GetBoolean(6))
                {
                    if (ids[0] == -1)
                        ids[0] = rdr.GetInt32(0);
                    else
                        ids[1] = rdr.GetInt32(0);
                    if (ids[0] != -1 && ids[1] != -1)
                        break;
                }
            }
            rdr.Close();
            cmd.CommandText = ("UPDATE scores SET thisgame = FALSE WHERE id =" + ids[1]);
            cmd.ExecuteNonQuery();
            cmd.CommandText = ("UPDATE scores SET thisgame = FALSE WHERE id =" + ids[0]);
            cmd.ExecuteNonQuery();
        }
        private string IDInfo(int whatinfo, int id) //whatinfo => name =1 __ win=2 __ draw =3 __ loss =4 __ scores =5
        {
            string stm = "SELECT * FROM scores";
            var cmd = new SQLiteCommand(stm, con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr.GetInt32(0) == id)
                {
                    if (whatinfo == 1)
                        return rdr.GetString(whatinfo);
                    else
                        return Convert.ToString(rdr.GetInt32(whatinfo));
                }
            }
            return null;
        }
        private void sameplayer()
        {
            for (int i = 0; i <= 1; i++)
            {
                string stm = "SELECT id FROM scores WHERE name = '" + IDInfo(1, ids[i]) + "'";
                var cmd = new SQLiteCommand(stm, con);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr.GetInt32(0) != ids[i])
                    {
                        int temp = rdr.GetInt32(0);
                        rdr.Close();
                        cmd.CommandText = ("DELETE FROM scores WHERE id = " + ids[i]);
                        cmd.ExecuteNonQuery();
                        ids[i] = temp;
                        break;
                    }
                }
            }
        }
        private void updatesql(int win, int loss, int draw, int score)
        {
            for (int i = 0; i <= 1; i++)
            {
                var cmd = new SQLiteCommand(con);
                if (Turn == "W" && i == 1)
                {
                    cmd.CommandText = "UPDATE scores SET win = " + (Convert.ToInt32(IDInfo(2, ids[i])) + win) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE scores SET loss = " + (Convert.ToInt32(IDInfo(4, ids[i])) + loss) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE scores SET draw = " + (Convert.ToInt32(IDInfo(3, ids[i])) + draw) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE scores SET score = " + (Convert.ToInt32(IDInfo(5, ids[i])) + score) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                }
                else if (Turn != "W" && i == 0)
                {
                    cmd.CommandText = "UPDATE scores SET win = " + (Convert.ToInt32(IDInfo(2, ids[i])) + win) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE scores SET loss = " + (Convert.ToInt32(IDInfo(4, ids[i])) + loss) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE scores SET draw = " + (Convert.ToInt32(IDInfo(3, ids[i])) + draw) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "UPDATE scores SET score = " + (Convert.ToInt32(IDInfo(5, ids[i])) + score) + " WHERE id = " + ids[i];
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void drawRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Movementslistform list = new Movementslistform();
            list.Show();
        }
        private void playerScoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RankingListForm rankinglist = new RankingListForm(ids);
            rankinglist.Show();
        }

        
    }
}
