/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 01.02.2019
 * Time: 19:34
 */
namespace MAPeD
{
	partial class description_form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.BtnClose = new System.Windows.Forms.Button();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.CBoxShowAfterLoading = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// BtnClose
			// 
			this.BtnClose.Location = new System.Drawing.Point(210, 233);
			this.BtnClose.Name = "BtnClose";
			this.BtnClose.Size = new System.Drawing.Size(75, 23);
			this.BtnClose.TabIndex = 1;
			this.BtnClose.Text = "&Close";
			this.BtnClose.UseVisualStyleBackColor = true;
			this.BtnClose.Click += new System.EventHandler(this.BtnCloseClick_Event);
			// 
			// richTextBox
			// 
			this.richTextBox.AcceptsTab = true;
			this.richTextBox.Location = new System.Drawing.Point(5, 7);
			this.richTextBox.MaxLength = 2048;
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBox.Size = new System.Drawing.Size(280, 215);
			this.richTextBox.TabIndex = 0;
			this.richTextBox.Text = "";
			// 
			// CBoxShowAfterLoading
			// 
			this.CBoxShowAfterLoading.Checked = true;
			this.CBoxShowAfterLoading.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxShowAfterLoading.Location = new System.Drawing.Point(5, 228);
			this.CBoxShowAfterLoading.Name = "CBoxShowAfterLoading";
			this.CBoxShowAfterLoading.Size = new System.Drawing.Size(160, 24);
			this.CBoxShowAfterLoading.TabIndex = 2;
			this.CBoxShowAfterLoading.Text = "Show after loading a project";
			this.CBoxShowAfterLoading.UseVisualStyleBackColor = true;
			this.CBoxShowAfterLoading.CheckedChanged += new System.EventHandler(this.CBoxShowAfterLoadingChanged_Event);
			// 
			// description_form
			// 
			this.AcceptButton = this.BtnClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(290, 262);
			this.Controls.Add(this.CBoxShowAfterLoading);
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.BtnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "description_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Project Description";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CBoxShowAfterLoading;
		private System.Windows.Forms.RichTextBox richTextBox;
		private System.Windows.Forms.Button BtnClose;
	}
}
