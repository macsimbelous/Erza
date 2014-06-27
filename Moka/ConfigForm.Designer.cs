namespace Moka
{
    partial class ConfigForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.ConnectionString_textBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.password_textBox = new System.Windows.Forms.TextBox();
            this.login_textBox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.sankaku_checkBox = new System.Windows.Forms.CheckBox();
            this.danbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.gelbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.imouto_checkBox = new System.Windows.Forms.CheckBox();
            this.konachan_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Строка подключения к базе данных";
            // 
            // ConnectionString_textBox
            // 
            this.ConnectionString_textBox.Location = new System.Drawing.Point(15, 25);
            this.ConnectionString_textBox.Name = "ConnectionString_textBox";
            this.ConnectionString_textBox.Size = new System.Drawing.Size(334, 20);
            this.ConnectionString_textBox.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(260, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(341, 160);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(355, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(61, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Создать";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.password_textBox);
            this.groupBox1.Controls.Add(this.login_textBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(206, 100);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Учётная запись Данбуры";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Пароль";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Имя";
            // 
            // password_textBox
            // 
            this.password_textBox.Location = new System.Drawing.Point(9, 71);
            this.password_textBox.Name = "password_textBox";
            this.password_textBox.Size = new System.Drawing.Size(188, 20);
            this.password_textBox.TabIndex = 11;
            // 
            // login_textBox
            // 
            this.login_textBox.Location = new System.Drawing.Point(9, 32);
            this.login_textBox.Name = "login_textBox";
            this.login_textBox.Size = new System.Drawing.Size(188, 20);
            this.login_textBox.TabIndex = 10;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.konachan_checkBox);
            this.groupBox2.Controls.Add(this.imouto_checkBox);
            this.groupBox2.Controls.Add(this.gelbooru_checkBox);
            this.groupBox2.Controls.Add(this.danbooru_checkBox);
            this.groupBox2.Controls.Add(this.sankaku_checkBox);
            this.groupBox2.Location = new System.Drawing.Point(227, 51);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(189, 100);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Импортировать";
            // 
            // sankaku_checkBox
            // 
            this.sankaku_checkBox.AutoSize = true;
            this.sankaku_checkBox.Location = new System.Drawing.Point(6, 19);
            this.sankaku_checkBox.Name = "sankaku_checkBox";
            this.sankaku_checkBox.Size = new System.Drawing.Size(69, 17);
            this.sankaku_checkBox.TabIndex = 0;
            this.sankaku_checkBox.Text = "Sankaku";
            this.sankaku_checkBox.UseVisualStyleBackColor = true;
            // 
            // danbooru_checkBox
            // 
            this.danbooru_checkBox.AutoSize = true;
            this.danbooru_checkBox.Location = new System.Drawing.Point(6, 42);
            this.danbooru_checkBox.Name = "danbooru_checkBox";
            this.danbooru_checkBox.Size = new System.Drawing.Size(73, 17);
            this.danbooru_checkBox.TabIndex = 1;
            this.danbooru_checkBox.Text = "Danbooru";
            this.danbooru_checkBox.UseVisualStyleBackColor = true;
            // 
            // gelbooru_checkBox
            // 
            this.gelbooru_checkBox.AutoSize = true;
            this.gelbooru_checkBox.Location = new System.Drawing.Point(6, 65);
            this.gelbooru_checkBox.Name = "gelbooru_checkBox";
            this.gelbooru_checkBox.Size = new System.Drawing.Size(69, 17);
            this.gelbooru_checkBox.TabIndex = 2;
            this.gelbooru_checkBox.Text = "Gelbooru";
            this.gelbooru_checkBox.UseVisualStyleBackColor = true;
            // 
            // imouto_checkBox
            // 
            this.imouto_checkBox.AutoSize = true;
            this.imouto_checkBox.Location = new System.Drawing.Point(100, 19);
            this.imouto_checkBox.Name = "imouto_checkBox";
            this.imouto_checkBox.Size = new System.Drawing.Size(58, 17);
            this.imouto_checkBox.TabIndex = 3;
            this.imouto_checkBox.Text = "Imouto";
            this.imouto_checkBox.UseVisualStyleBackColor = true;
            // 
            // konachan_checkBox
            // 
            this.konachan_checkBox.AutoSize = true;
            this.konachan_checkBox.Location = new System.Drawing.Point(100, 42);
            this.konachan_checkBox.Name = "konachan_checkBox";
            this.konachan_checkBox.Size = new System.Drawing.Size(75, 17);
            this.konachan_checkBox.TabIndex = 4;
            this.konachan_checkBox.Text = "Konachan";
            this.konachan_checkBox.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 196);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ConnectionString_textBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfigForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройка";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ConnectionString_textBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox password_textBox;
        private System.Windows.Forms.TextBox login_textBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox konachan_checkBox;
        private System.Windows.Forms.CheckBox imouto_checkBox;
        private System.Windows.Forms.CheckBox gelbooru_checkBox;
        private System.Windows.Forms.CheckBox danbooru_checkBox;
        private System.Windows.Forms.CheckBox sankaku_checkBox;
    }
}