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
    public partial class PawnUpgradingForm : Form
    {
        int location;
        Game MainForm;
        string Turn;
        public PawnUpgradingForm(int i, Game ob)
        {
            location = i;
            MainForm = ob;
            Turn = MainForm.Turn;
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            SetImage();
        }
        private void SetImage()
        {
            if (Turn == "B")
            {
                Rook.BackgroundImage = MainForm.images[5];
                Knight.BackgroundImage = MainForm.images[4];
                Bishop.BackgroundImage = MainForm.images[3];
                Queen.BackgroundImage = MainForm.images[2];
            }
            else
            {
                Rook.BackgroundImage = MainForm.images[11];
                Knight.BackgroundImage = MainForm.images[10];
                Bishop.BackgroundImage = MainForm.images[9];
                Queen.BackgroundImage = MainForm.images[8];
            }
        }
        private void Rook_Click(object sender, EventArgs e)
        {
            MainForm.SetPawnUpgradeImage(1, location);
            MainForm.Enabled = true;

            this.Close();
        }
        private void Knight_Click(object sender, EventArgs e)
        {
            MainForm.SetPawnUpgradeImage(2, location);
            MainForm.Enabled = true;
            this.Close();
        }
        private void Bishop_Click(object sender, EventArgs e)
        {
            MainForm.SetPawnUpgradeImage(3, location);
            MainForm.Enabled = true;
            this.Close();
        }
        private void Queen_Click(object sender, EventArgs e)
        {
            MainForm.SetPawnUpgradeImage(4, location);
            MainForm.Enabled = true;
            this.Close();
        }
    }
}
