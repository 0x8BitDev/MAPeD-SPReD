/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 11.06.2019
 * Time: 15:53
 */
namespace SPReD
{
	partial class SMS_sprite_flipping_form
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
			this.OkBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.TransformPositionsCBox = new System.Windows.Forms.CheckBox();
			this.CopyCHRDataCBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// OkBtn
			// 
			this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkBtn.Location = new System.Drawing.Point(92, 119);
			this.OkBtn.Name = "OkBtn";
			this.OkBtn.Size = new System.Drawing.Size(75, 23);
			this.OkBtn.TabIndex = 2;
			this.OkBtn.Text = "&Ok";
			this.OkBtn.UseVisualStyleBackColor = true;
			this.OkBtn.Click += new System.EventHandler(this.OkBtnClick);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(173, 119);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 3;
			this.CancelBtn.Text = "&Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtnClick);
			// 
			// TransformPositionsCBox
			// 
			this.TransformPositionsCBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.TransformPositionsCBox.Checked = true;
			this.TransformPositionsCBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TransformPositionsCBox.Location = new System.Drawing.Point(28, 12);
			this.TransformPositionsCBox.Name = "TransformPositionsCBox";
			this.TransformPositionsCBox.Size = new System.Drawing.Size(212, 38);
			this.TransformPositionsCBox.TabIndex = 0;
			this.TransformPositionsCBox.Text = "Transform positions ( uncheck it to fix the NES flipped sprites issue )";
			this.TransformPositionsCBox.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.TransformPositionsCBox.UseVisualStyleBackColor = true;
			// 
			// CopyCHRDataCBox
			// 
			this.CopyCHRDataCBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.CopyCHRDataCBox.Checked = true;
			this.CopyCHRDataCBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CopyCHRDataCBox.Location = new System.Drawing.Point(28, 56);
			this.CopyCHRDataCBox.Name = "CopyCHRDataCBox";
			this.CopyCHRDataCBox.Size = new System.Drawing.Size(212, 47);
			this.CopyCHRDataCBox.TabIndex = 1;
			this.CopyCHRDataCBox.Text = "Copy CHR data ( it helps when several sprites share the same CHR data; all unused" +
			"/empty CHRs will be lost! )";
			this.CopyCHRDataCBox.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.CopyCHRDataCBox.UseVisualStyleBackColor = true;
			// 
			// SMS_sprite_flipping_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(260, 152);
			this.Controls.Add(this.CopyCHRDataCBox);
			this.Controls.Add(this.TransformPositionsCBox);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OkBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SMS_sprite_flipping_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Sprite(s) Flipping";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CopyCHRDataCBox;
		private System.Windows.Forms.CheckBox TransformPositionsCBox;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Button OkBtn;
	}
}
