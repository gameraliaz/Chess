namespace Shatranj_v0._01
{
    partial class MainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
            this.multiplayer_btn = new System.Windows.Forms.Button();
            this.aboutus_btn = new System.Windows.Forms.Button();
            this.exit_btn = new System.Windows.Forms.Button();
            this.dualplayer_btn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // multiplayer_btn
            // 
            this.multiplayer_btn.Location = new System.Drawing.Point(43, 208);
            this.multiplayer_btn.Margin = new System.Windows.Forms.Padding(2);
            this.multiplayer_btn.Name = "multiplayer_btn";
            this.multiplayer_btn.Size = new System.Drawing.Size(202, 65);
            this.multiplayer_btn.TabIndex = 0;
            this.multiplayer_btn.Text = "Network multiplayer";
            this.multiplayer_btn.UseVisualStyleBackColor = true;
            this.multiplayer_btn.Click += new System.EventHandler(this.multiplayer_btn_Click);
            // 
            // aboutus_btn
            // 
            this.aboutus_btn.Location = new System.Drawing.Point(43, 278);
            this.aboutus_btn.Margin = new System.Windows.Forms.Padding(2);
            this.aboutus_btn.Name = "aboutus_btn";
            this.aboutus_btn.Size = new System.Drawing.Size(202, 65);
            this.aboutus_btn.TabIndex = 0;
            this.aboutus_btn.Text = "About Me";
            this.aboutus_btn.UseVisualStyleBackColor = true;
            this.aboutus_btn.Click += new System.EventHandler(this.aboutus_btn_Click);
            // 
            // exit_btn
            // 
            this.exit_btn.Location = new System.Drawing.Point(43, 347);
            this.exit_btn.Margin = new System.Windows.Forms.Padding(2);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(202, 65);
            this.exit_btn.TabIndex = 0;
            this.exit_btn.Text = "Exit";
            this.exit_btn.UseVisualStyleBackColor = true;
            this.exit_btn.Click += new System.EventHandler(this.exit_btn_Click);
            // 
            // dualplayer_btn
            // 
            this.dualplayer_btn.Location = new System.Drawing.Point(43, 138);
            this.dualplayer_btn.Margin = new System.Windows.Forms.Padding(2);
            this.dualplayer_btn.Name = "dualplayer_btn";
            this.dualplayer_btn.Size = new System.Drawing.Size(202, 65);
            this.dualplayer_btn.TabIndex = 0;
            this.dualplayer_btn.Text = "Multiplayer In a device";
            this.dualplayer_btn.UseVisualStyleBackColor = true;
            this.dualplayer_btn.Click += new System.EventHandler(this.dualplayer_btn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Chiller", 50F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(67, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 77);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chess";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(291, 434);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.aboutus_btn);
            this.Controls.Add(this.multiplayer_btn);
            this.Controls.Add(this.dualplayer_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "MainMenu";
            this.Text = "Chess";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button multiplayer_btn;
        private System.Windows.Forms.Button aboutus_btn;
        private System.Windows.Forms.Button exit_btn;
        private System.Windows.Forms.Button dualplayer_btn;
        private System.Windows.Forms.Label label1;
    }
}