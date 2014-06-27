namespace Anew
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
            this.Non_tag_checkBox = new System.Windows.Forms.CheckBox();
            this.tag_textBox = new System.Windows.Forms.TextBox();
            this.new_only_checkBox = new System.Windows.Forms.CheckBox();
            this.ok_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.all_new_checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Non_tag_checkBox
            // 
            this.Non_tag_checkBox.AutoSize = true;
            this.Non_tag_checkBox.Location = new System.Drawing.Point(12, 35);
            this.Non_tag_checkBox.Name = "Non_tag_checkBox";
            this.Non_tag_checkBox.Size = new System.Drawing.Size(167, 17);
            this.Non_tag_checkBox.TabIndex = 0;
            this.Non_tag_checkBox.Text = "Отбирать только  без тегов";
            this.Non_tag_checkBox.UseVisualStyleBackColor = true;
            this.Non_tag_checkBox.CheckedChanged += new System.EventHandler(this.Non_tag_checkBox_CheckedChanged);
            // 
            // tag_textBox
            // 
            this.tag_textBox.Location = new System.Drawing.Point(12, 58);
            this.tag_textBox.Name = "tag_textBox";
            this.tag_textBox.Size = new System.Drawing.Size(211, 20);
            this.tag_textBox.TabIndex = 1;
            // 
            // new_only_checkBox
            // 
            this.new_only_checkBox.AutoSize = true;
            this.new_only_checkBox.Location = new System.Drawing.Point(12, 84);
            this.new_only_checkBox.Name = "new_only_checkBox";
            this.new_only_checkBox.Size = new System.Drawing.Size(98, 17);
            this.new_only_checkBox.TabIndex = 2;
            this.new_only_checkBox.Text = "Только новые";
            this.new_only_checkBox.UseVisualStyleBackColor = true;
            // 
            // ok_button
            // 
            this.ok_button.Location = new System.Drawing.Point(116, 110);
            this.ok_button.Name = "ok_button";
            this.ok_button.Size = new System.Drawing.Size(75, 23);
            this.ok_button.TabIndex = 3;
            this.ok_button.Text = "ОК";
            this.ok_button.UseVisualStyleBackColor = true;
            this.ok_button.Click += new System.EventHandler(this.ok_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel_button.Location = new System.Drawing.Point(197, 110);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(75, 23);
            this.cancel_button.TabIndex = 4;
            this.cancel_button.Text = "Отмена";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // all_new_checkBox
            // 
            this.all_new_checkBox.AutoSize = true;
            this.all_new_checkBox.Location = new System.Drawing.Point(12, 12);
            this.all_new_checkBox.Name = "all_new_checkBox";
            this.all_new_checkBox.Size = new System.Drawing.Size(130, 17);
            this.all_new_checkBox.TabIndex = 5;
            this.all_new_checkBox.Text = "Отбирить все новые";
            this.all_new_checkBox.UseVisualStyleBackColor = true;
            this.all_new_checkBox.CheckedChanged += new System.EventHandler(this.all_new_checkBox_CheckedChanged);
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.ok_button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel_button;
            this.ClientSize = new System.Drawing.Size(284, 143);
            this.Controls.Add(this.all_new_checkBox);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.ok_button);
            this.Controls.Add(this.new_only_checkBox);
            this.Controls.Add(this.tag_textBox);
            this.Controls.Add(this.Non_tag_checkBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ConfigForm";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox Non_tag_checkBox;
        private System.Windows.Forms.TextBox tag_textBox;
        private System.Windows.Forms.CheckBox new_only_checkBox;
        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.CheckBox all_new_checkBox;
    }
}