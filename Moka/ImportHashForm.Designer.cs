namespace Moka
{
    partial class ImportHashForm
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.Get_button = new System.Windows.Forms.Button();
            this.stop_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tags_textBox = new System.Windows.Forms.TextBox();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.konachan_checkBox = new System.Windows.Forms.CheckBox();
            this.imouto_checkBox = new System.Windows.Forms.CheckBox();
            this.gelbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.danbooru_checkBox = new System.Windows.Forms.CheckBox();
            this.sankaku_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(15, 105);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(636, 290);
            this.listBox1.TabIndex = 0;
            // 
            // Get_button
            // 
            this.Get_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Get_button.Location = new System.Drawing.Point(576, 23);
            this.Get_button.Name = "Get_button";
            this.Get_button.Size = new System.Drawing.Size(75, 23);
            this.Get_button.TabIndex = 2;
            this.Get_button.Text = "Начать";
            this.Get_button.UseVisualStyleBackColor = true;
            this.Get_button.Click += new System.EventHandler(this.Get_button_Click);
            // 
            // stop_button
            // 
            this.stop_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stop_button.Enabled = false;
            this.stop_button.Location = new System.Drawing.Point(576, 49);
            this.stop_button.Name = "stop_button";
            this.stop_button.Size = new System.Drawing.Size(75, 23);
            this.stop_button.TabIndex = 4;
            this.stop_button.Text = "Остановить";
            this.stop_button.UseVisualStyleBackColor = true;
            this.stop_button.Click += new System.EventHandler(this.stop_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Теги";
            // 
            // tags_textBox
            // 
            this.tags_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tags_textBox.Location = new System.Drawing.Point(15, 25);
            this.tags_textBox.Name = "tags_textBox";
            this.tags_textBox.Size = new System.Drawing.Size(551, 20);
            this.tags_textBox.TabIndex = 6;
            // 
            // Cancel_button
            // 
            this.Cancel_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel_button.Location = new System.Drawing.Point(576, 76);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_button.TabIndex = 9;
            this.Cancel_button.Text = "Закрыть";
            this.Cancel_button.UseVisualStyleBackColor = true;
            this.Cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.konachan_checkBox);
            this.groupBox2.Controls.Add(this.imouto_checkBox);
            this.groupBox2.Controls.Add(this.gelbooru_checkBox);
            this.groupBox2.Controls.Add(this.danbooru_checkBox);
            this.groupBox2.Controls.Add(this.sankaku_checkBox);
            this.groupBox2.Location = new System.Drawing.Point(15, 51);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 48);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Сервисы";
            // 
            // konachan_checkBox
            // 
            this.konachan_checkBox.AutoSize = true;
            this.konachan_checkBox.Location = new System.Drawing.Point(156, 19);
            this.konachan_checkBox.Name = "konachan_checkBox";
            this.konachan_checkBox.Size = new System.Drawing.Size(75, 17);
            this.konachan_checkBox.TabIndex = 4;
            this.konachan_checkBox.Text = "Konachan";
            this.konachan_checkBox.UseVisualStyleBackColor = true;
            // 
            // imouto_checkBox
            // 
            this.imouto_checkBox.AutoSize = true;
            this.imouto_checkBox.Location = new System.Drawing.Point(237, 19);
            this.imouto_checkBox.Name = "imouto_checkBox";
            this.imouto_checkBox.Size = new System.Drawing.Size(58, 17);
            this.imouto_checkBox.TabIndex = 3;
            this.imouto_checkBox.Text = "Imouto";
            this.imouto_checkBox.UseVisualStyleBackColor = true;
            // 
            // gelbooru_checkBox
            // 
            this.gelbooru_checkBox.AutoSize = true;
            this.gelbooru_checkBox.Location = new System.Drawing.Point(81, 19);
            this.gelbooru_checkBox.Name = "gelbooru_checkBox";
            this.gelbooru_checkBox.Size = new System.Drawing.Size(69, 17);
            this.gelbooru_checkBox.TabIndex = 2;
            this.gelbooru_checkBox.Text = "Gelbooru";
            this.gelbooru_checkBox.UseVisualStyleBackColor = true;
            // 
            // danbooru_checkBox
            // 
            this.danbooru_checkBox.AutoSize = true;
            this.danbooru_checkBox.Location = new System.Drawing.Point(301, 19);
            this.danbooru_checkBox.Name = "danbooru_checkBox";
            this.danbooru_checkBox.Size = new System.Drawing.Size(73, 17);
            this.danbooru_checkBox.TabIndex = 1;
            this.danbooru_checkBox.Text = "Danbooru";
            this.danbooru_checkBox.UseVisualStyleBackColor = true;
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
            // ImportHashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 407);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Cancel_button);
            this.Controls.Add(this.tags_textBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.stop_button);
            this.Controls.Add(this.Get_button);
            this.Controls.Add(this.listBox1);
            this.Name = "ImportHashForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Импорт хэшей с Данбуры";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportHashForm_FormClosing);
            this.Load += new System.EventHandler(this.ImportHashForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button Get_button;
        private System.Windows.Forms.Button stop_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tags_textBox;
        private System.Windows.Forms.Button Cancel_button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox konachan_checkBox;
        private System.Windows.Forms.CheckBox imouto_checkBox;
        private System.Windows.Forms.CheckBox gelbooru_checkBox;
        private System.Windows.Forms.CheckBox danbooru_checkBox;
        private System.Windows.Forms.CheckBox sankaku_checkBox;
    }
}