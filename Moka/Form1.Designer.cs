namespace Moka
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Find_button = new System.Windows.Forms.Button();
            this.Find_textBox = new System.Windows.Forms.TextBox();
            this.Result_listBox = new System.Windows.Forms.ListBox();
            this.Remove_button = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.add_tagget_file_button = new System.Windows.Forms.Button();
            this.SlideShow_button = new System.Windows.Forms.Button();
            this.random_checkBox = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.import_hash_button = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Find_button
            // 
            this.Find_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Find_button.Location = new System.Drawing.Point(678, 10);
            this.Find_button.Name = "Find_button";
            this.Find_button.Size = new System.Drawing.Size(114, 23);
            this.Find_button.TabIndex = 0;
            this.Find_button.Text = "Найти";
            this.Find_button.UseVisualStyleBackColor = true;
            this.Find_button.Click += new System.EventHandler(this.Find_button_Click);
            // 
            // Find_textBox
            // 
            this.Find_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Find_textBox.Location = new System.Drawing.Point(12, 12);
            this.Find_textBox.Name = "Find_textBox";
            this.Find_textBox.Size = new System.Drawing.Size(660, 20);
            this.Find_textBox.TabIndex = 1;
            // 
            // Result_listBox
            // 
            this.Result_listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Result_listBox.FormattingEnabled = true;
            this.Result_listBox.Location = new System.Drawing.Point(12, 38);
            this.Result_listBox.Name = "Result_listBox";
            this.Result_listBox.Size = new System.Drawing.Size(660, 329);
            this.Result_listBox.TabIndex = 2;
            this.Result_listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Result_listBox_MouseDoubleClick);
            // 
            // Remove_button
            // 
            this.Remove_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Remove_button.Location = new System.Drawing.Point(678, 113);
            this.Remove_button.Name = "Remove_button";
            this.Remove_button.Size = new System.Drawing.Size(114, 23);
            this.Remove_button.TabIndex = 5;
            this.Remove_button.Text = "Удалить";
            this.Remove_button.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 372);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(803, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(200, 17);
            // 
            // add_tagget_file_button
            // 
            this.add_tagget_file_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.add_tagget_file_button.Location = new System.Drawing.Point(678, 69);
            this.add_tagget_file_button.Name = "add_tagget_file_button";
            this.add_tagget_file_button.Size = new System.Drawing.Size(114, 38);
            this.add_tagget_file_button.TabIndex = 7;
            this.add_tagget_file_button.Text = "Добавить несколько";
            this.add_tagget_file_button.UseVisualStyleBackColor = true;
            this.add_tagget_file_button.Click += new System.EventHandler(this.add_tagget_file_button_Click);
            // 
            // SlideShow_button
            // 
            this.SlideShow_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SlideShow_button.Location = new System.Drawing.Point(678, 159);
            this.SlideShow_button.Name = "SlideShow_button";
            this.SlideShow_button.Size = new System.Drawing.Size(114, 35);
            this.SlideShow_button.TabIndex = 8;
            this.SlideShow_button.Text = "Запустить слайдшоу";
            this.SlideShow_button.UseVisualStyleBackColor = true;
            this.SlideShow_button.Click += new System.EventHandler(this.SlideShow_button_Click);
            // 
            // random_checkBox
            // 
            this.random_checkBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.random_checkBox.AutoSize = true;
            this.random_checkBox.Location = new System.Drawing.Point(678, 200);
            this.random_checkBox.Name = "random_checkBox";
            this.random_checkBox.Size = new System.Drawing.Size(126, 17);
            this.random_checkBox.TabIndex = 9;
            this.random_checkBox.Text = "Случайный порядок";
            this.random_checkBox.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(678, 252);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Настройка";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // import_hash_button
            // 
            this.import_hash_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.import_hash_button.Location = new System.Drawing.Point(678, 223);
            this.import_hash_button.Name = "import_hash_button";
            this.import_hash_button.Size = new System.Drawing.Size(114, 23);
            this.import_hash_button.TabIndex = 11;
            this.import_hash_button.Text = "Импорт Хэшей";
            this.import_hash_button.UseVisualStyleBackColor = true;
            this.import_hash_button.Click += new System.EventHandler(this.import_hash_button_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 394);
            this.Controls.Add(this.import_hash_button);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.random_checkBox);
            this.Controls.Add(this.SlideShow_button);
            this.Controls.Add(this.add_tagget_file_button);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.Remove_button);
            this.Controls.Add(this.Result_listBox);
            this.Controls.Add(this.Find_textBox);
            this.Controls.Add(this.Find_button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Moka";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Find_button;
        private System.Windows.Forms.TextBox Find_textBox;
        private System.Windows.Forms.ListBox Result_listBox;
        private System.Windows.Forms.Button Remove_button;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button add_tagget_file_button;
        private System.Windows.Forms.Button SlideShow_button;
        private System.Windows.Forms.CheckBox random_checkBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button import_hash_button;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}

