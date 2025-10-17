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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewImageForm));
            pictureBox1 = new System.Windows.Forms.PictureBox();
            contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(components);
            viewinfullscreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            edittagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            prevToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            listBox1 = new System.Windows.Forms.ListBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            tags_count_label = new System.Windows.Forms.Label();
            size_label = new System.Windows.Forms.Label();
            resolution_label = new System.Windows.Forms.Label();
            format_label = new System.Windows.Forms.Label();
            AddTag_button = new System.Windows.Forms.Button();
            RemoveTag_button = new System.Windows.Forms.Button();
            Search_button = new System.Windows.Forms.Button();
            add_to_favorited_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            pictureBox1.Location = new System.Drawing.Point(334, 14);
            pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(957, 664);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { viewinfullscreenToolStripMenuItem, edittagsToolStripMenuItem, nextToolStripMenuItem, prevToolStripMenuItem, deleteToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(152, 114);
            // 
            // viewinfullscreenToolStripMenuItem
            // 
            viewinfullscreenToolStripMenuItem.Name = "viewinfullscreenToolStripMenuItem";
            viewinfullscreenToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            viewinfullscreenToolStripMenuItem.Text = "На весь экран";
            viewinfullscreenToolStripMenuItem.Click += viewinfullscreenToolStripMenuItem_Click;
            // 
            // edittagsToolStripMenuItem
            // 
            edittagsToolStripMenuItem.Name = "edittagsToolStripMenuItem";
            edittagsToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            // 
            // nextToolStripMenuItem
            // 
            nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            nextToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            nextToolStripMenuItem.Text = "Следуюшее";
            nextToolStripMenuItem.Click += nextToolStripMenuItem_Click;
            // 
            // prevToolStripMenuItem
            // 
            prevToolStripMenuItem.Name = "prevToolStripMenuItem";
            prevToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            prevToolStripMenuItem.Text = "Предыдушее";
            prevToolStripMenuItem.Click += prevToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            deleteToolStripMenuItem.Text = "Удалить";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // listBox1
            // 
            listBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            listBox1.FormattingEnabled = true;
            listBox1.Location = new System.Drawing.Point(14, 14);
            listBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            listBox1.Name = "listBox1";
            listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listBox1.Size = new System.Drawing.Size(314, 504);
            listBox1.TabIndex = 1;
            listBox1.DrawItem += listBox1_DrawItem;
            listBox1.DoubleClick += listBox1_DoubleClick;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            groupBox1.Controls.Add(tags_count_label);
            groupBox1.Controls.Add(size_label);
            groupBox1.Controls.Add(resolution_label);
            groupBox1.Controls.Add(format_label);
            groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            groupBox1.Location = new System.Drawing.Point(14, 576);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Size = new System.Drawing.Size(314, 101);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Свойства";
            // 
            // tags_count_label
            // 
            tags_count_label.AutoSize = true;
            tags_count_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            tags_count_label.Location = new System.Drawing.Point(7, 74);
            tags_count_label.Name = "tags_count_label";
            tags_count_label.Size = new System.Drawing.Size(44, 16);
            tags_count_label.TabIndex = 3;
            tags_count_label.Text = "label3";
            // 
            // size_label
            // 
            size_label.AutoSize = true;
            size_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            size_label.Location = new System.Drawing.Point(7, 56);
            size_label.Name = "size_label";
            size_label.Size = new System.Drawing.Size(44, 16);
            size_label.TabIndex = 2;
            size_label.Text = "label3";
            // 
            // resolution_label
            // 
            resolution_label.AutoSize = true;
            resolution_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            resolution_label.Location = new System.Drawing.Point(7, 37);
            resolution_label.Name = "resolution_label";
            resolution_label.Size = new System.Drawing.Size(44, 16);
            resolution_label.TabIndex = 1;
            resolution_label.Text = "label2";
            // 
            // format_label
            // 
            format_label.AutoSize = true;
            format_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            format_label.Location = new System.Drawing.Point(7, 19);
            format_label.Name = "format_label";
            format_label.Size = new System.Drawing.Size(44, 16);
            format_label.TabIndex = 0;
            format_label.Text = "label1";
            // 
            // AddTag_button
            // 
            AddTag_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            AddTag_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            AddTag_button.Location = new System.Drawing.Point(14, 536);
            AddTag_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            AddTag_button.Name = "AddTag_button";
            AddTag_button.Size = new System.Drawing.Size(100, 32);
            AddTag_button.TabIndex = 3;
            AddTag_button.Text = "Добавить тег";
            AddTag_button.UseVisualStyleBackColor = true;
            AddTag_button.Click += AddTag_button_Click;
            // 
            // RemoveTag_button
            // 
            RemoveTag_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            RemoveTag_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            RemoveTag_button.Location = new System.Drawing.Point(123, 536);
            RemoveTag_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            RemoveTag_button.Name = "RemoveTag_button";
            RemoveTag_button.Size = new System.Drawing.Size(90, 32);
            RemoveTag_button.TabIndex = 4;
            RemoveTag_button.Text = "Удалить тег";
            RemoveTag_button.UseVisualStyleBackColor = true;
            RemoveTag_button.Click += RemoveTag_button_Click;
            // 
            // Search_button
            // 
            Search_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            Search_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            Search_button.Location = new System.Drawing.Point(222, 536);
            Search_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Search_button.Name = "Search_button";
            Search_button.Size = new System.Drawing.Size(60, 32);
            Search_button.TabIndex = 5;
            Search_button.Text = "Найти";
            Search_button.UseVisualStyleBackColor = true;
            Search_button.Click += Search_button_Click;
            // 
            // add_to_favorited_button
            // 
            add_to_favorited_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            add_to_favorited_button.Image = (System.Drawing.Image)resources.GetObject("add_to_favorited_button.Image");
            add_to_favorited_button.Location = new System.Drawing.Point(290, 536);
            add_to_favorited_button.Name = "add_to_favorited_button";
            add_to_favorited_button.Size = new System.Drawing.Size(35, 32);
            add_to_favorited_button.TabIndex = 6;
            add_to_favorited_button.UseVisualStyleBackColor = true;
            add_to_favorited_button.Click += add_to_favorited_button_Click;
            // 
            // ViewImageForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1305, 692);
            Controls.Add(add_to_favorited_button);
            Controls.Add(Search_button);
            Controls.Add(RemoveTag_button);
            Controls.Add(AddTag_button);
            Controls.Add(groupBox1);
            Controls.Add(listBox1);
            Controls.Add(pictureBox1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "ViewImageForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Просмотр";
            FormClosed += ViewImageForm_FormClosed;
            Load += ViewImageForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

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
        private System.Windows.Forms.Button add_to_favorited_button;
    }
}