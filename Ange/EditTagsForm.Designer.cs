namespace Ange
{
    partial class EditTagsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditTagsForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ImageTags_listBox = new System.Windows.Forms.ListBox();
            this.AllTags_listBox = new System.Windows.Forms.ListBox();
            this.AddTag_button = new System.Windows.Forms.Button();
            this.DeleteTag_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(509, 576);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ImageTags_listBox
            // 
            this.ImageTags_listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ImageTags_listBox.FormattingEnabled = true;
            this.ImageTags_listBox.Location = new System.Drawing.Point(528, 12);
            this.ImageTags_listBox.Name = "ImageTags_listBox";
            this.ImageTags_listBox.Size = new System.Drawing.Size(211, 576);
            this.ImageTags_listBox.TabIndex = 1;
            // 
            // AllTags_listBox
            // 
            this.AllTags_listBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AllTags_listBox.FormattingEnabled = true;
            this.AllTags_listBox.Location = new System.Drawing.Point(826, 12);
            this.AllTags_listBox.Name = "AllTags_listBox";
            this.AllTags_listBox.Size = new System.Drawing.Size(211, 576);
            this.AllTags_listBox.TabIndex = 2;
            // 
            // AddTag_button
            // 
            this.AddTag_button.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AddTag_button.Location = new System.Drawing.Point(745, 106);
            this.AddTag_button.Name = "AddTag_button";
            this.AddTag_button.Size = new System.Drawing.Size(75, 23);
            this.AddTag_button.TabIndex = 3;
            this.AddTag_button.Text = "Добавить";
            this.AddTag_button.UseVisualStyleBackColor = true;
            this.AddTag_button.Click += new System.EventHandler(this.AddTag_button_Click);
            // 
            // DeleteTag_button
            // 
            this.DeleteTag_button.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteTag_button.Location = new System.Drawing.Point(745, 135);
            this.DeleteTag_button.Name = "DeleteTag_button";
            this.DeleteTag_button.Size = new System.Drawing.Size(75, 23);
            this.DeleteTag_button.TabIndex = 4;
            this.DeleteTag_button.Text = "Удалить";
            this.DeleteTag_button.UseVisualStyleBackColor = true;
            this.DeleteTag_button.Click += new System.EventHandler(this.DeleteTag_button_Click);
            // 
            // EditTagsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 601);
            this.Controls.Add(this.DeleteTag_button);
            this.Controls.Add(this.AddTag_button);
            this.Controls.Add(this.AllTags_listBox);
            this.Controls.Add(this.ImageTags_listBox);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditTagsForm";
            this.Text = "Редактирование тегов изображения";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditTagsForm_FormClosing);
            this.Load += new System.EventHandler(this.EditTagsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox ImageTags_listBox;
        private System.Windows.Forms.ListBox AllTags_listBox;
        private System.Windows.Forms.Button AddTag_button;
        private System.Windows.Forms.Button DeleteTag_button;
    }
}