namespace Ange
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tag_radioButton = new System.Windows.Forms.RadioButton();
            this.md5_radioButton = new System.Windows.Forms.RadioButton();
            this.search_button = new System.Windows.Forms.Button();
            this.all_button = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.просмортToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.редактироватьТегиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(10, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(599, 20);
            this.textBox1.TabIndex = 0;
            // 
            // tag_radioButton
            // 
            this.tag_radioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tag_radioButton.AutoSize = true;
            this.tag_radioButton.Checked = true;
            this.tag_radioButton.Location = new System.Drawing.Point(615, 13);
            this.tag_radioButton.Name = "tag_radioButton";
            this.tag_radioButton.Size = new System.Drawing.Size(43, 17);
            this.tag_radioButton.TabIndex = 1;
            this.tag_radioButton.TabStop = true;
            this.tag_radioButton.Text = "Тег";
            this.tag_radioButton.UseVisualStyleBackColor = true;
            // 
            // md5_radioButton
            // 
            this.md5_radioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.md5_radioButton.AutoSize = true;
            this.md5_radioButton.Location = new System.Drawing.Point(664, 13);
            this.md5_radioButton.Name = "md5_radioButton";
            this.md5_radioButton.Size = new System.Drawing.Size(48, 17);
            this.md5_radioButton.TabIndex = 2;
            this.md5_radioButton.TabStop = true;
            this.md5_radioButton.Text = "MD5";
            this.md5_radioButton.UseVisualStyleBackColor = true;
            // 
            // search_button
            // 
            this.search_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.search_button.Location = new System.Drawing.Point(718, 10);
            this.search_button.Name = "search_button";
            this.search_button.Size = new System.Drawing.Size(75, 23);
            this.search_button.TabIndex = 3;
            this.search_button.Text = "Поиск";
            this.search_button.UseVisualStyleBackColor = true;
            this.search_button.Click += new System.EventHandler(this.search_button_Click);
            // 
            // all_button
            // 
            this.all_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.all_button.Location = new System.Drawing.Point(799, 10);
            this.all_button.Name = "all_button";
            this.all_button.Size = new System.Drawing.Size(75, 23);
            this.all_button.TabIndex = 4;
            this.all_button.Text = "Все";
            this.all_button.UseVisualStyleBackColor = true;
            this.all_button.Click += new System.EventHandler(this.all_button_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(0, 41);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(883, 437);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.VirtualMode = true;
            this.listView1.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(200, 150);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 481);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(883, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.просмортToolStripMenuItem,
            this.редактироватьТегиToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(181, 48);
            // 
            // просмортToolStripMenuItem
            // 
            this.просмортToolStripMenuItem.Name = "просмортToolStripMenuItem";
            this.просмортToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.просмортToolStripMenuItem.Text = "Просморт";
            // 
            // редактироватьТегиToolStripMenuItem
            // 
            this.редактироватьТегиToolStripMenuItem.Name = "редактироватьТегиToolStripMenuItem";
            this.редактироватьТегиToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.редактироватьТегиToolStripMenuItem.Text = "Редактировать теги";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 503);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.all_button);
            this.Controls.Add(this.search_button);
            this.Controls.Add(this.md5_radioButton);
            this.Controls.Add(this.tag_radioButton);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Ange";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton tag_radioButton;
        private System.Windows.Forms.RadioButton md5_radioButton;
        private System.Windows.Forms.Button search_button;
        private System.Windows.Forms.Button all_button;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem просмортToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem редактироватьТегиToolStripMenuItem;
    }
}

