namespace Client
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            listBox1 = new ListBox();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            button3 = new Button();
            comboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(756, 16);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 0;
            button1.Text = "连接";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(756, 485);
            button2.Name = "button2";
            button2.Size = new Size(150, 46);
            button2.TabIndex = 1;
            button2.Text = "发送";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(929, 41);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(240, 438);
            listBox1.TabIndex = 2;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(222, 490);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(506, 38);
            textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.White;
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.Location = new Point(12, 68);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(894, 411);
            textBox2.TabIndex = 4;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(528, 21);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(200, 38);
            textBox3.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(405, 24);
            label1.Name = "label1";
            label1.Size = new Size(86, 31);
            label1.TabIndex = 6;
            label1.Text = "用户名";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(984, 9);
            label2.Name = "label2";
            label2.Size = new Size(110, 31);
            label2.TabIndex = 7;
            label2.Text = "用户列表";
            // 
            // button3
            // 
            button3.Location = new Point(974, 485);
            button3.Name = "button3";
            button3.Size = new Size(150, 46);
            button3.TabIndex = 8;
            button3.Text = "退出";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(12, 489);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(184, 39);
            comboBox1.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1181, 555);
            Controls.Add(comboBox1);
            Controls.Add(button3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(listBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Chatting";
            FormClosing += Form1_FormClosing;
            FormClosed += Form1_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public Button button1;
        private Button button2;
        public ListBox listBox1;
        private TextBox textBox1;
        public TextBox textBox2;
        public TextBox textBox3;
        private Label label1;
        private Label label2;
        private Button button3;
        public ComboBox comboBox1;
    }
}
