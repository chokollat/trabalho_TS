namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    partial class Form3
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
            label2 = new Label();
            label3 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            label_login = new Label();
            txtRegistoUsername = new TextBox();
            txtRegistoPassword = new TextBox();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            button1 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // label2
            // 
            label2.BackColor = Color.White;
            label2.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(162, 125, 93);
            label2.Location = new Point(269, 286);
            label2.Name = "label2";
            label2.Size = new Size(101, 20);
            label2.TabIndex = 21;
            label2.Text = "Password";
            // 
            // label3
            // 
            label3.BackColor = Color.White;
            label3.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(162, 125, 93);
            label3.Location = new Point(269, 219);
            label3.Name = "label3";
            label3.Size = new Size(118, 20);
            label3.TabIndex = 20;
            label3.Text = "User Name";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Black;
            panel1.Controls.Add(panel2);
            panel1.ForeColor = Color.Black;
            panel1.Location = new Point(274, 282);
            panel1.Name = "panel1";
            panel1.Size = new Size(204, 1);
            panel1.TabIndex = 19;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Black;
            panel2.ForeColor = Color.Black;
            panel2.Location = new Point(0, 61);
            panel2.Name = "panel2";
            panel2.Size = new Size(204, 1);
            panel2.TabIndex = 10;
            // 
            // label_login
            // 
            label_login.AutoSize = true;
            label_login.BackColor = Color.White;
            label_login.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_login.ForeColor = Color.FromArgb(162, 125, 93);
            label_login.Location = new Point(322, 157);
            label_login.Name = "label_login";
            label_login.Size = new Size(131, 37);
            label_login.TabIndex = 18;
            label_login.Text = "Registo";
            // 
            // txtRegistoUsername
            // 
            txtRegistoUsername.Location = new Point(302, 242);
            txtRegistoUsername.Multiline = true;
            txtRegistoUsername.Name = "txtRegistoUsername";
            txtRegistoUsername.Size = new Size(175, 23);
            txtRegistoUsername.TabIndex = 17;
            // 
            // txtRegistoPassword
            // 
            txtRegistoPassword.Location = new Point(302, 309);
            txtRegistoPassword.Multiline = true;
            txtRegistoPassword.Name = "txtRegistoPassword";
            txtRegistoPassword.Size = new Size(175, 23);
            txtRegistoPassword.TabIndex = 16;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = Properties.Resources.padlock2;
            pictureBox4.Location = new Point(274, 309);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(22, 21);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox4.TabIndex = 15;
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.man1;
            pictureBox3.Location = new Point(274, 244);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(22, 21);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 14;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.Adobe_Express___file1;
            pictureBox2.Location = new Point(274, 52);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(235, 80);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 13;
            pictureBox2.TabStop = false;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(162, 125, 93);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(322, 364);
            button1.Name = "button1";
            button1.Size = new Size(129, 31);
            button1.TabIndex = 22;
            button1.Text = "Register";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(panel1);
            Controls.Add(label_login);
            Controls.Add(txtRegistoUsername);
            Controls.Add(txtRegistoPassword);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form3";
            Text = "Form3";
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label3;
        private Panel panel1;
        private Panel panel2;
        private Label label_login;
        private TextBox txtRegistoUsername;
        private TextBox txtRegistoPassword;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private Button button1;
    }
}