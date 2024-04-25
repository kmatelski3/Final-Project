namespace ArtNet
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
            label2 = new Label();
            button1 = new Button();
            panel1 = new Panel();
            label3 = new Label();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            label4 = new Label();
            textBox2 = new TextBox();
            button5 = new Button();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Black", 12F);
            label2.Location = new Point(12, 11);
            label2.Name = "label2";
            label2.Size = new Size(168, 28);
            label2.TabIndex = 2;
            label2.Text = "Global Settings:";
            // 
            // button1
            // 
            button1.Location = new Point(679, 11);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 3;
            button1.Text = "Write";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(64, 64, 64);
            panel1.Location = new Point(2, 83);
            panel1.Name = "panel1";
            panel1.Size = new Size(1800, 10);
            panel1.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(187, 19);
            label3.Name = "label3";
            label3.Size = new Size(49, 20);
            label3.TabIndex = 9;
            label3.Text = "Preset";
            // 
            // button2
            // 
            button2.Location = new Point(579, 11);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 10;
            button2.Text = "Read";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(779, 10);
            button3.Name = "button3";
            button3.Size = new Size(277, 29);
            button3.TabIndex = 11;
            button3.Text = "Remove Universe from Presets";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Gold;
            button4.Font = new Font("Segoe UI", 9F);
            button4.Location = new Point(1062, 10);
            button4.Name = "button4";
            button4.Size = new Size(166, 29);
            button4.TabIndex = 12;
            button4.Text = "RESET ALL PRESETS";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1234, 15);
            label1.Name = "label1";
            label1.Size = new Size(129, 20);
            label1.TabIndex = 13;
            label1.Text = "MBed Drive Letter";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(1369, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(25, 27);
            textBox1.TabIndex = 14;
            textBox1.Text = "E";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1400, 15);
            label4.Name = "label4";
            label4.Size = new Size(115, 20);
            label4.TabIndex = 15;
            label4.Text = "Mbed COM Port";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(1521, 12);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(61, 27);
            textBox2.TabIndex = 16;
            textBox2.Text = "COM4";
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // button5
            // 
            button5.BackColor = Color.DeepSkyBlue;
            button5.Font = new Font("Segoe UI", 9F);
            button5.Location = new Point(1588, 10);
            button5.Name = "button5";
            button5.Size = new Size(166, 29);
            button5.TabIndex = 17;
            button5.Text = "MBED SOFT RESET";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1800, 1100);
            Controls.Add(button5);
            Controls.Add(textBox2);
            Controls.Add(label4);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label3);
            Controls.Add(panel1);
            Controls.Add(button1);
            Controls.Add(label2);
            Name = "Form1";
            Text = "ArtNet Preset Builder";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label2;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button button1;
        private Panel panel1;
        private Label label3;
        private Button button2;
        private Button button3;
        private Button button4;
        private Label label1;
        private TextBox textBox1;
        private Label label4;
        private TextBox textBox2;
        private Button button5;
    }
}
