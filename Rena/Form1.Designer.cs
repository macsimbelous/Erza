namespace Rena
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.start_button = new System.Windows.Forms.Button();
            this.radioButton_MD5 = new System.Windows.Forms.RadioButton();
            this.radioButton_SHA1 = new System.Windows.Forms.RadioButton();
            this.radioButton_SHA256 = new System.Windows.Forms.RadioButton();
            this.checkBox_subdir = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.stop_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(351, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 20);
            this.button1.TabIndex = 0;
            this.button1.Text = "Каталог";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(1, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(344, 20);
            this.textBox1.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(1, 102);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(553, 212);
            this.listBox1.TabIndex = 2;
            // 
            // start_button
            // 
            this.start_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.start_button.Location = new System.Drawing.Point(461, 3);
            this.start_button.Name = "start_button";
            this.start_button.Size = new System.Drawing.Size(93, 20);
            this.start_button.TabIndex = 3;
            this.start_button.Text = "Старт";
            this.start_button.UseVisualStyleBackColor = true;
            this.start_button.Click += new System.EventHandler(this.button2_Click);
            // 
            // radioButton_MD5
            // 
            this.radioButton_MD5.AutoSize = true;
            this.radioButton_MD5.Checked = true;
            this.radioButton_MD5.Location = new System.Drawing.Point(12, 52);
            this.radioButton_MD5.Name = "radioButton_MD5";
            this.radioButton_MD5.Size = new System.Drawing.Size(48, 17);
            this.radioButton_MD5.TabIndex = 4;
            this.radioButton_MD5.TabStop = true;
            this.radioButton_MD5.Text = "MD5";
            this.radioButton_MD5.UseVisualStyleBackColor = true;
            // 
            // radioButton_SHA1
            // 
            this.radioButton_SHA1.AutoSize = true;
            this.radioButton_SHA1.Location = new System.Drawing.Point(66, 52);
            this.radioButton_SHA1.Name = "radioButton_SHA1";
            this.radioButton_SHA1.Size = new System.Drawing.Size(53, 17);
            this.radioButton_SHA1.TabIndex = 5;
            this.radioButton_SHA1.TabStop = true;
            this.radioButton_SHA1.Text = "SHA1";
            this.radioButton_SHA1.UseVisualStyleBackColor = true;
            // 
            // radioButton_SHA256
            // 
            this.radioButton_SHA256.AutoSize = true;
            this.radioButton_SHA256.Location = new System.Drawing.Point(125, 52);
            this.radioButton_SHA256.Name = "radioButton_SHA256";
            this.radioButton_SHA256.Size = new System.Drawing.Size(65, 17);
            this.radioButton_SHA256.TabIndex = 6;
            this.radioButton_SHA256.TabStop = true;
            this.radioButton_SHA256.Text = "SHA256";
            this.radioButton_SHA256.UseVisualStyleBackColor = true;
            // 
            // checkBox_subdir
            // 
            this.checkBox_subdir.AutoSize = true;
            this.checkBox_subdir.Location = new System.Drawing.Point(12, 29);
            this.checkBox_subdir.Name = "checkBox_subdir";
            this.checkBox_subdir.Size = new System.Drawing.Size(167, 17);
            this.checkBox_subdir.TabIndex = 7;
            this.checkBox_subdir.Text = "Обрабатывать подкаталоги";
            this.checkBox_subdir.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(1, 73);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(553, 23);
            this.progressBar1.TabIndex = 8;
            // 
            // stop_button
            // 
            this.stop_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stop_button.Location = new System.Drawing.Point(461, 30);
            this.stop_button.Name = "stop_button";
            this.stop_button.Size = new System.Drawing.Size(93, 20);
            this.stop_button.TabIndex = 9;
            this.stop_button.Text = "Стоп";
            this.stop_button.UseVisualStyleBackColor = true;
            this.stop_button.Click += new System.EventHandler(this.stop_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 315);
            this.Controls.Add(this.stop_button);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.checkBox_subdir);
            this.Controls.Add(this.radioButton_SHA256);
            this.Controls.Add(this.radioButton_SHA1);
            this.Controls.Add(this.radioButton_MD5);
            this.Controls.Add(this.start_button);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Rena";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button start_button;
        private System.Windows.Forms.RadioButton radioButton_MD5;
        private System.Windows.Forms.RadioButton radioButton_SHA1;
        private System.Windows.Forms.RadioButton radioButton_SHA256;
        private System.Windows.Forms.CheckBox checkBox_subdir;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button stop_button;
    }
}

