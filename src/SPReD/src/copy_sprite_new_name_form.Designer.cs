/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2017
 * Time: 18:13
 */
namespace SPReD
{
	partial class copy_sprite_new_name_form
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
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.EditNewName = new System.Windows.Forms.TextBox();
			this.RadionBtnPrefix = new System.Windows.Forms.RadioButton();
			this.RadioBtnPostfix = new System.Windows.Forms.RadioButton();
			this.Append = new System.Windows.Forms.GroupBox();
			this.Append.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(4, 111);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 5;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(85, 111);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 6;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Enter Name:";
			// 
			// EditNewName
			// 
			this.EditNewName.Location = new System.Drawing.Point(12, 23);
			this.EditNewName.MaxLength = 64;
			this.EditNewName.Name = "EditNewName";
			this.EditNewName.Size = new System.Drawing.Size(143, 20);
			this.EditNewName.TabIndex = 1;
			// 
			// RadionBtnPrefix
			// 
			this.RadionBtnPrefix.Location = new System.Drawing.Point(13, 17);
			this.RadionBtnPrefix.Name = "RadionBtnPrefix";
			this.RadionBtnPrefix.Size = new System.Drawing.Size(57, 24);
			this.RadionBtnPrefix.TabIndex = 3;
			this.RadionBtnPrefix.Text = "Prefix";
			this.RadionBtnPrefix.UseVisualStyleBackColor = true;
			// 
			// RadioBtnPostfix
			// 
			this.RadioBtnPostfix.Checked = true;
			this.RadioBtnPostfix.Location = new System.Drawing.Point(76, 17);
			this.RadioBtnPostfix.Name = "RadioBtnPostfix";
			this.RadioBtnPostfix.Size = new System.Drawing.Size(59, 24);
			this.RadioBtnPostfix.TabIndex = 4;
			this.RadioBtnPostfix.TabStop = true;
			this.RadioBtnPostfix.Text = "Postfix";
			this.RadioBtnPostfix.UseVisualStyleBackColor = true;
			// 
			// Append
			// 
			this.Append.Controls.Add(this.RadioBtnPostfix);
			this.Append.Controls.Add(this.RadionBtnPrefix);
			this.Append.Location = new System.Drawing.Point(12, 49);
			this.Append.Name = "Append";
			this.Append.Size = new System.Drawing.Size(143, 47);
			this.Append.TabIndex = 2;
			this.Append.TabStop = false;
			this.Append.Text = "Use it as:";
			// 
			// copy_sprite_new_name_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(167, 140);
			this.Controls.Add(this.EditNewName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.Append);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "copy_sprite_new_name_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Enter Prefix/Postfix";
			this.Append.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.GroupBox Append;
		private System.Windows.Forms.RadioButton RadioBtnPostfix;
		private System.Windows.Forms.RadioButton RadionBtnPrefix;
		private System.Windows.Forms.TextBox EditNewName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
	}
}
