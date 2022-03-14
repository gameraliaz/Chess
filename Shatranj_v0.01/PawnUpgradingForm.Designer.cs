namespace Shatranj_v0._01
{
    partial class PawnUpgradingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Knight = new System.Windows.Forms.Button();
            this.Bishop = new System.Windows.Forms.Button();
            this.Queen = new System.Windows.Forms.Button();
            this.Rook = new System.Windows.Forms.Button();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game));

            this.SuspendLayout();
            // 
            // Knight
            // 
            this.Knight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Knight.Location = new System.Drawing.Point(138, 12);
            this.Knight.Name = "Knight";
            this.Knight.Size = new System.Drawing.Size(120, 120);
            this.Knight.TabIndex = 0;
            this.Knight.UseVisualStyleBackColor = true;
            this.Knight.Click += new System.EventHandler(this.Knight_Click);
            // 
            // Bishop
            // 
            this.Bishop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Bishop.Location = new System.Drawing.Point(264, 12);
            this.Bishop.Name = "Bishop";
            this.Bishop.Size = new System.Drawing.Size(120, 120);
            this.Bishop.TabIndex = 0;
            this.Bishop.UseVisualStyleBackColor = true;
            this.Bishop.Click += new System.EventHandler(this.Bishop_Click);
            // 
            // Queen
            // 
            this.Queen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Queen.Location = new System.Drawing.Point(390, 12);
            this.Queen.Name = "Queen";
            this.Queen.Size = new System.Drawing.Size(120, 120);
            this.Queen.TabIndex = 0;
            this.Queen.UseVisualStyleBackColor = true;
            this.Queen.Click += new System.EventHandler(this.Queen_Click);
            // 
            // Rook
            // 
            this.Rook.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Rook.Location = new System.Drawing.Point(12, 12);
            this.Rook.Name = "Rook";
            this.Rook.Size = new System.Drawing.Size(120, 120);
            this.Rook.TabIndex = 0;
            this.Rook.UseVisualStyleBackColor = true;
            this.Rook.Click += new System.EventHandler(this.Rook_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(531, 151);
            this.Controls.Add(this.Queen);
            this.Controls.Add(this.Knight);
            this.Controls.Add(this.Bishop);
            this.Controls.Add(this.Rook);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.Opacity = 0.98D;
            this.Text = "Select";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ResumeLayout(false);


        }

        #endregion

        private System.Windows.Forms.Button Rook;
        private System.Windows.Forms.Button Knight;
        private System.Windows.Forms.Button Bishop;
        private System.Windows.Forms.Button Queen;
    }
}