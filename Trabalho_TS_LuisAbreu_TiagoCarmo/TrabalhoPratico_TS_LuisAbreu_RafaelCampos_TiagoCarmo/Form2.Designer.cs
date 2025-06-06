namespace TrabalhoPratico_TS_LuisAbreu_RafaelCampos_TiagoCarmo
{
    partial class Form2
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
            pictureBox2 = new PictureBox();
            btn_exit = new Button();
            btn_back = new Button();
            colorDialog1 = new ColorDialog();
            btn_send = new Button();
            textBox1 = new TextBox();
            panel1 = new Panel();
            panel2 = new Panel();
            label_login = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.Adobe_Express___file1;
            pictureBox2.Location = new Point(-12, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(264, 112);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            // 
            // btn_exit
            // 
            btn_exit.BackColor = Color.Transparent;
            btn_exit.FlatStyle = FlatStyle.Flat;
            btn_exit.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_exit.ForeColor = Color.FromArgb(162, 125, 93);
            btn_exit.Location = new Point(12, 402);
            btn_exit.Name = "btn_exit";
            btn_exit.Size = new Size(117, 34);
            btn_exit.TabIndex = 14;
            btn_exit.Text = "Exit";
            btn_exit.UseVisualStyleBackColor = false;
            btn_exit.Click += btn_exit_Click;
            // 
            // btn_back
            // 
            btn_back.BackColor = Color.Transparent;
            btn_back.FlatStyle = FlatStyle.Flat;
            btn_back.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_back.ForeColor = Color.FromArgb(162, 125, 93);
            btn_back.Location = new Point(135, 402);
            btn_back.Name = "btn_back";
            btn_back.Size = new Size(117, 34);
            btn_back.TabIndex = 15;
            btn_back.Text = "Logout";
            btn_back.UseVisualStyleBackColor = false;
            btn_back.Click += btn_back_Click;
            // 
            // btn_send
            // 
            btn_send.BackColor = Color.Transparent;
            btn_send.FlatStyle = FlatStyle.Flat;
            btn_send.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_send.ForeColor = Color.FromArgb(162, 125, 93);
            btn_send.Location = new Point(674, 404);
            btn_send.Name = "btn_send";
            btn_send.Size = new Size(117, 34);
            btn_send.TabIndex = 16;
            btn_send.Text = "Send";
            btn_send.UseVisualStyleBackColor = false;
            // 
            // textBox1
            // 
            textBox1.ForeColor = Color.Black;
            textBox1.Location = new Point(301, 404);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(364, 34);
            textBox1.TabIndex = 17;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(162, 125, 93);
            panel1.ForeColor = Color.FromArgb(162, 125, 93);
            panel1.Location = new Point(298, 402);
            panel1.Name = "panel1";
            panel1.Size = new Size(370, 38);
            panel1.TabIndex = 18;
            // 
            // panel2
            // 
            panel2.BackColor = Color.WhiteSmoke;
            panel2.Location = new Point(298, 12);
            panel2.Name = "panel2";
            panel2.Size = new Size(493, 386);
            panel2.TabIndex = 19;
            // 
            // label_login
            // 
            label_login.BackColor = Color.White;
            label_login.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label_login.ForeColor = Color.FromArgb(162, 125, 93);
            label_login.Location = new Point(6, 169);
            label_login.Name = "label_login";
            label_login.Size = new Size(208, 30);
            label_login.TabIndex = 20;
            label_login.Text = "Connected Users:";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(label_login);
            Controls.Add(panel2);
            Controls.Add(textBox1);
            Controls.Add(btn_send);
            Controls.Add(btn_back);
            Controls.Add(btn_exit);
            Controls.Add(pictureBox2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form2";
            Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox2;
        private Button btn_exit;
        private Button btn_back;
        private ColorDialog colorDialog1;
        private Button btn_send;
        private TextBox textBox1;
        private Panel panel1;
        private Panel panel2;
        private Label label_login;
    }
}