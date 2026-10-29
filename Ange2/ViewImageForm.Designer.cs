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
            button1 = new System.Windows.Forms.Button();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.ContextMenuStrip = contextMenuStrip1;
            pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBox1.Location = new System.Drawing.Point(0, 0);
            pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(980, 637);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.Resize += pictureBox1_Resize;
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
            listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 204);
            listBox1.FormattingEnabled = true;
            listBox1.Location = new System.Drawing.Point(3, 4);
            listBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            listBox1.Name = "listBox1";
            listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listBox1.Size = new System.Drawing.Size(315, 489);
            listBox1.TabIndex = 1;
            listBox1.DrawItem += listBox1_DrawItem;
            listBox1.DoubleClick += listBox1_DoubleClick;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(tags_count_label);
            groupBox1.Controls.Add(size_label);
            groupBox1.Controls.Add(resolution_label);
            groupBox1.Controls.Add(format_label);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            groupBox1.Location = new System.Drawing.Point(3, 541);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Size = new System.Drawing.Size(315, 92);
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
            AddTag_button.Location = new System.Drawing.Point(7, 3);
            AddTag_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            AddTag_button.Name = "AddTag_button";
            AddTag_button.Size = new System.Drawing.Size(74, 32);
            AddTag_button.TabIndex = 3;
            AddTag_button.Text = "Добавить";
            AddTag_button.UseVisualStyleBackColor = true;
            AddTag_button.Click += AddTag_button_Click;
            // 
            // RemoveTag_button
            // 
            RemoveTag_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            RemoveTag_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            RemoveTag_button.Location = new System.Drawing.Point(87, 3);
            RemoveTag_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            RemoveTag_button.Name = "RemoveTag_button";
            RemoveTag_button.Size = new System.Drawing.Size(73, 32);
            RemoveTag_button.TabIndex = 4;
            RemoveTag_button.Text = "Удалить";
            RemoveTag_button.UseVisualStyleBackColor = true;
            RemoveTag_button.Click += RemoveTag_button_Click;
            // 
            // Search_button
            // 
            Search_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            Search_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
            Search_button.Location = new System.Drawing.Point(215, 2);
            Search_button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Search_button.Name = "Search_button";
            Search_button.Size = new System.Drawing.Size(60, 32);
            Search_button.TabIndex = 6;
            Search_button.Text = "Найти";
            Search_button.UseVisualStyleBackColor = true;
            Search_button.Click += Search_button_Click;
            // 
            // add_to_favorited_button
            // 
            add_to_favorited_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            add_to_favorited_button.Image = (System.Drawing.Image)resources.GetObject("add_to_favorited_button.Image");
            add_to_favorited_button.Location = new System.Drawing.Point(281, 2);
            add_to_favorited_button.Name = "add_to_favorited_button";
            add_to_favorited_button.Size = new System.Drawing.Size(35, 32);
            add_to_favorited_button.TabIndex = 7;
            add_to_favorited_button.UseVisualStyleBackColor = true;
            add_to_favorited_button.Click += add_to_favorited_button_Click;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            button1.Location = new System.Drawing.Point(166, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(43, 32);
            button1.TabIndex = 5;
            button1.Text = "ИИ";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(pictureBox1);
            splitContainer1.Size = new System.Drawing.Size(1305, 637);
            splitContainer1.SplitterDistance = 321;
            splitContainer1.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(panel1, 0, 1);
            tableLayoutPanel1.Controls.Add(listBox1, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(321, 637);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(AddTag_button);
            panel1.Controls.Add(RemoveTag_button);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(Search_button);
            panel1.Controls.Add(add_to_favorited_button);
            panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            panel1.Location = new System.Drawing.Point(0, 497);
            panel1.Margin = new System.Windows.Forms.Padding(0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(321, 40);
            panel1.TabIndex = 3;
            // 
            // ViewImageForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1305, 637);
            Controls.Add(splitContainer1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "ViewImageForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Просмотр";
            FormClosed += ViewImageForm_FormClosed;
            Load += ViewImageForm_Load;
            DpiChanged += ViewImageForm_DpiChanged;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}