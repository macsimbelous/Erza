namespace Ange
{
    partial class ViewImageForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewImageForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewinfullscreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edittagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tags_count_label = new System.Windows.Forms.Label();
            this.size_label = new System.Windows.Forms.Label();
            this.resolution_label = new System.Windows.Forms.Label();
            this.format_label = new System.Windows.Forms.Label();
            this.AddTag_button = new System.Windows.Forms.Button();
            this.RemoveTag_button = new System.Windows.Forms.Button();
            this.Search_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Location = new System.Drawing.Point(287, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(820, 575);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewinfullscreenToolStripMenuItem,
            this.edittagsToolStripMenuItem,
            this.nextToolStripMenuItem,
            this.prevToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(155, 114);
            // 
            // viewinfullscreenToolStripMenuItem
            // 
            this.viewinfullscreenToolStripMenuItem.Name = "viewinfullscreenToolStripMenuItem";
            this.viewinfullscreenToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.viewinfullscreenToolStripMenuItem.Text = "На весь экран";
            this.viewinfullscreenToolStripMenuItem.Click += new System.EventHandler(this.viewinfullscreenToolStripMenuItem_Click);
            // 
            // edittagsToolStripMenuItem
            // 
            this.edittagsToolStripMenuItem.Name = "edittagsToolStripMenuItem";
            this.edittagsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.edittagsToolStripMenuItem.Text = "Изменить теги";
            this.edittagsToolStripMenuItem.Click += new System.EventHandler(this.edittagsToolStripMenuItem_Click);
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.nextToolStripMenuItem.Text = "Следуюшее";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // prevToolStripMenuItem
            // 
            this.prevToolStripMenuItem.Name = "prevToolStripMenuItem";
            this.prevToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.prevToolStripMenuItem.Text = "Предыдушее";
            this.prevToolStripMenuItem.Click += new System.EventHandler(this.prevToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.deleteToolStripMenuItem.Text = "Удалить";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(269, 436);
            this.listBox1.TabIndex = 1;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.tags_count_label);
            this.groupBox1.Controls.Add(this.size_label);
            this.groupBox1.Controls.Add(this.resolution_label);
            this.groupBox1.Controls.Add(this.format_label);
            this.groupBox1.Location = new System.Drawing.Point(12, 499);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 88);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Свойства";
            // 
            // tags_count_label
            // 
            this.tags_count_label.AutoSize = true;
            this.tags_count_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tags_count_label.Location = new System.Drawing.Point(6, 64);
            this.tags_count_label.Name = "tags_count_label";
            this.tags_count_label.Size = new System.Drawing.Size(45, 16);
            this.tags_count_label.TabIndex = 3;
            this.tags_count_label.Text = "label3";
            // 
            // size_label
            // 
            this.size_label.AutoSize = true;
            this.size_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.size_label.Location = new System.Drawing.Point(6, 48);
            this.size_label.Name = "size_label";
            this.size_label.Size = new System.Drawing.Size(45, 16);
            this.size_label.TabIndex = 2;
            this.size_label.Text = "label3";
            // 
            // resolution_label
            // 
            this.resolution_label.AutoSize = true;
            this.resolution_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.resolution_label.Location = new System.Drawing.Point(6, 32);
            this.resolution_label.Name = "resolution_label";
            this.resolution_label.Size = new System.Drawing.Size(45, 16);
            this.resolution_label.TabIndex = 1;
            this.resolution_label.Text = "label2";
            // 
            // format_label
            // 
            this.format_label.AutoSize = true;
            this.format_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.format_label.Location = new System.Drawing.Point(6, 16);
            this.format_label.Name = "format_label";
            this.format_label.Size = new System.Drawing.Size(45, 16);
            this.format_label.TabIndex = 0;
            this.format_label.Text = "label1";
            // 
            // AddTag_button
            // 
            this.AddTag_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddTag_button.Location = new System.Drawing.Point(12, 465);
            this.AddTag_button.Name = "AddTag_button";
            this.AddTag_button.Size = new System.Drawing.Size(95, 28);
            this.AddTag_button.TabIndex = 3;
            this.AddTag_button.Text = "Добавить тег";
            this.AddTag_button.UseVisualStyleBackColor = true;
            this.AddTag_button.Click += new System.EventHandler(this.AddTag_button_Click);
            // 
            // RemoveTag_button
            // 
            this.RemoveTag_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveTag_button.Location = new System.Drawing.Point(113, 465);
            this.RemoveTag_button.Name = "RemoveTag_button";
            this.RemoveTag_button.Size = new System.Drawing.Size(87, 28);
            this.RemoveTag_button.TabIndex = 4;
            this.RemoveTag_button.Text = "Удалить тег";
            this.RemoveTag_button.UseVisualStyleBackColor = true;
            this.RemoveTag_button.Click += new System.EventHandler(this.RemoveTag_button_Click);
            // 
            // Search_button
            // 
            this.Search_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Search_button.Location = new System.Drawing.Point(206, 465);
            this.Search_button.Name = "Search_button";
            this.Search_button.Size = new System.Drawing.Size(75, 28);
            this.Search_button.TabIndex = 5;
            this.Search_button.Text = "Найти";
            this.Search_button.UseVisualStyleBackColor = true;
            this.Search_button.Click += new System.EventHandler(this.Search_button_Click);
            // 
            // ViewImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1119, 599);
            this.Controls.Add(this.Search_button);
            this.Controls.Add(this.RemoveTag_button);
            this.Controls.Add(this.AddTag_button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Просмотр";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ViewImageForm_FormClosed);
            this.Load += new System.EventHandler(this.ViewImageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label tags_count_label;
        private System.Windows.Forms.Label size_label;
        private System.Windows.Forms.Label resolution_label;
        private System.Windows.Forms.Label format_label;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewinfullscreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem edittagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prevToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button AddTag_button;
        private System.Windows.Forms.Button RemoveTag_button;
        private System.Windows.Forms.Button Search_button;
    }
}