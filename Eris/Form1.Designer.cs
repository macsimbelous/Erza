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
            this.tag_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localization = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.new_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.edit_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.delete_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.search_toolStripTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.find_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.count_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.count_all_toolStripButton = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tag_id,
            this.tag,
            this.localization,
            this.type,
            this.count,
            this.description});
            this.dataGridView1.Location = new System.Drawing.Point(0, 42);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(890, 494);
            this.dataGridView1.TabIndex = 0;
            // 
            // tag_id
            // 
            this.tag_id.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.tag_id.DataPropertyName = "tag_id";
            this.tag_id.FillWeight = 80F;
            this.tag_id.HeaderText = "ИД";
            this.tag_id.MinimumWidth = 80;
            this.tag_id.Name = "tag_id";
            this.tag_id.ReadOnly = true;
            this.tag_id.ToolTipText = "Идентификатор тега";
            this.tag_id.Width = 80;
            // 
            // tag
            // 
            this.tag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.tag.DataPropertyName = "tag";
            this.tag.FillWeight = 58.14433F;
            this.tag.HeaderText = "Тег";
            this.tag.Name = "tag";
            this.tag.ReadOnly = true;
            this.tag.ToolTipText = "Название тега";
            // 
            // localization
            // 
            this.localization.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.localization.DataPropertyName = "localization";
            this.localization.FillWeight = 58.14433F;
            this.localization.HeaderText = "Тег на русском";
            this.localization.Name = "localization";
            this.localization.ReadOnly = true;
            // 
            // type
            // 
            this.type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.type.DataPropertyName = "type";
            this.type.FillWeight = 29.07216F;
            this.type.HeaderText = "Тип";
            this.type.Name = "type";
            this.type.ReadOnly = true;
            this.type.ToolTipText = "Тип тега";
            // 
            // count
            // 
            this.count.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.count.DataPropertyName = "count";
            this.count.HeaderText = "Ссылки";
            this.count.Name = "count";
            this.count.ReadOnly = true;
            this.count.ToolTipText = "Количество ссылок на тег";
            // 
            // description
            // 
            this.description.DataPropertyName = "description";
            this.description.HeaderText = "Описание";
            this.description.Name = "description";
            this.description.ReadOnly = true;
            this.description.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.new_toolStripButton,
            this.edit_toolStripButton,
            this.delete_toolStripButton,
            this.search_toolStripTextBox,
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
            this.new_toolStripButton.Click += new System.EventHandler(this.new_toolStripButton_Click);
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
            this.delete_toolStripButton.Click += new System.EventHandler(this.delete_toolStripButton_Click);
            // 
            // search_toolStripTextBox
            // 
            this.search_toolStripTextBox.Name = "search_toolStripTextBox";
            this.search_toolStripTextBox.Size = new System.Drawing.Size(150, 39);
            // 
            // find_toolStripButton
            // 
            this.find_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.find_toolStripButton.Image = global::Eris.Properties.Resources.Search256;
            this.find_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.find_toolStripButton.Name = "find_toolStripButton";
            this.find_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.find_toolStripButton.Text = "Поиск";
            this.find_toolStripButton.Click += new System.EventHandler(this.find_toolStripButton_Click);
            // 
            // count_toolStripButton
            // 
            this.count_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.count_toolStripButton.Image = global::Eris.Properties.Resources.Charting_Blue_New256;
            this.count_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.count_toolStripButton.Name = "count_toolStripButton";
            this.count_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.count_toolStripButton.Text = "Подсчитать ссылки на тег";
            this.count_toolStripButton.Click += new System.EventHandler(this.count_toolStripButton_Click);
            // 
            // count_all_toolStripButton
            // 
            this.count_all_toolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.count_all_toolStripButton.Image = global::Eris.Properties.Resources.Charting_Blue_Add256;
            this.count_all_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.count_all_toolStripButton.Name = "count_all_toolStripButton";
            this.count_all_toolStripButton.Size = new System.Drawing.Size(36, 36);
            this.count_all_toolStripButton.Text = "Подсчитать ссылки на все теги";
            this.count_all_toolStripButton.Click += new System.EventHandler(this.count_all_toolStripButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 536);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.ToolStripButton find_toolStripButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn tag_id;
        private System.Windows.Forms.DataGridViewTextBoxColumn tag;
        private System.Windows.Forms.DataGridViewTextBoxColumn localization;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn count;
        private System.Windows.Forms.DataGridViewTextBoxColumn description;
        private System.Windows.Forms.ToolStripTextBox search_toolStripTextBox;
    }
}

