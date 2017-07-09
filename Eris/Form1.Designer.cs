namespace Eris
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.new_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.edit_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.delete_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.find_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.count_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.count_all_toolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView1.Location = new System.Drawing.Point(0, 42);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(890, 494);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.DataPropertyName = "tag_id";
            this.Column1.HeaderText = "Идентификатор тега";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.DataPropertyName = "tag";
            this.Column2.HeaderText = "Тег";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.DataPropertyName = "count";
            this.Column3.HeaderText = "Количество ссылок на тег";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.DataPropertyName = "type";
            this.Column4.HeaderText = "Тип тега";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.new_toolStripButton,
            this.edit_toolStripButton,
            this.delete_toolStripButton,
            this.find_toolStripButton,
            this.count_toolStripButton,
            this.count_all_toolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(890, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // new_toolStripButton
            // 
            this.new_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.new_toolStripButton.Image = global::Eris.Properties.Resources.Add_Green256;
            this.new_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.new_toolStripButton.Name = "new_toolStripButton";
            this.new_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.new_toolStripButton.Text = "Новый тег";
            // 
            // edit_toolStripButton
            // 
            this.edit_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.edit_toolStripButton.Image = global::Eris.Properties.Resources.Edit256;
            this.edit_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.edit_toolStripButton.Name = "edit_toolStripButton";
            this.edit_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.edit_toolStripButton.Text = "Редактировать тег";
            this.edit_toolStripButton.Click += new System.EventHandler(this.edit_toolStripButton_Click);
            // 
            // delete_toolStripButton
            // 
            this.delete_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.delete_toolStripButton.Image = global::Eris.Properties.Resources.Remove__Delete__Red256;
            this.delete_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.delete_toolStripButton.Name = "delete_toolStripButton";
            this.delete_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.delete_toolStripButton.Text = "Удалить тег";
            // 
            // find_toolStripButton
            // 
            this.find_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.find_toolStripButton.Image = global::Eris.Properties.Resources.Search256;
            this.find_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.find_toolStripButton.Name = "find_toolStripButton";
            this.find_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.find_toolStripButton.Text = "Поиск";
            // 
            // count_toolStripButton
            // 
            this.count_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.count_toolStripButton.Image = global::Eris.Properties.Resources.Charting_Blue_New256;
            this.count_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.count_toolStripButton.Name = "count_toolStripButton";
            this.count_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.count_toolStripButton.Text = "Подсчитать ссылки на тег";
            // 
            // count_all_toolStripButton
            // 
            this.count_all_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.count_all_toolStripButton.Image = global::Eris.Properties.Resources.Charting_Blue_Add256;
            this.count_all_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.count_all_toolStripButton.Name = "count_all_toolStripButton";
            this.count_all_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.count_all_toolStripButton.Text = "Подсчитать ссылки на все теги";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 536);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Эрис - редактор тегов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton new_toolStripButton;
        private System.Windows.Forms.ToolStripButton edit_toolStripButton;
        private System.Windows.Forms.ToolStripButton delete_toolStripButton;
        private System.Windows.Forms.ToolStripButton count_toolStripButton;
        private System.Windows.Forms.ToolStripButton count_all_toolStripButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.ToolStripButton find_toolStripButton;
    }
}

