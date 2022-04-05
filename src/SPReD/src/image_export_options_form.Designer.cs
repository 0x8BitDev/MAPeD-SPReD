/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 04.12.2018
 * Time: 16:42
 */
namespace SPReD
{
	partial class image_export_options_form
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.RBtnFormatPCX = new System.Windows.Forms.RadioButton();
			this.RBtnFormatPNG = new System.Windows.Forms.RadioButton();
			this.RBtnFormatBMP = new System.Windows.Forms.RadioButton();
			this.checkBoxAlphaChannel = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(8, 128);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 5;
			this.BtnOk.Text = "&Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(89, 128);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 6;
			this.BtnCancel.Text = "&Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RBtnFormatPCX);
			this.groupBox1.Controls.Add(this.RBtnFormatPNG);
			this.groupBox1.Controls.Add(this.RBtnFormatBMP);
			this.groupBox1.Location = new System.Drawing.Point(27, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(118, 84);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Format";
			// 
			// RBtnFormatPCX
			// 
			this.RBtnFormatPCX.Location = new System.Drawing.Point(17, 54);
			this.RBtnFormatPCX.Name = "RBtnFormatPCX";
			this.RBtnFormatPCX.Size = new System.Drawing.Size(95, 24);
			this.RBtnFormatPCX.TabIndex = 3;
			this.RBtnFormatPCX.Text = "PCX (indexed)";
			this.RBtnFormatPCX.UseVisualStyleBackColor = true;
			this.RBtnFormatPCX.CheckedChanged += new System.EventHandler(this.BtnImgFormatChanged_Event);
			// 
			// RBtnFormatPNG
			// 
			this.RBtnFormatPNG.Location = new System.Drawing.Point(17, 14);
			this.RBtnFormatPNG.Name = "RBtnFormatPNG";
			this.RBtnFormatPNG.Size = new System.Drawing.Size(53, 24);
			this.RBtnFormatPNG.TabIndex = 1;
			this.RBtnFormatPNG.Text = "PNG";
			this.RBtnFormatPNG.UseVisualStyleBackColor = true;
			this.RBtnFormatPNG.CheckedChanged += new System.EventHandler(this.BtnImgFormatChanged_Event);
			// 
			// RBtnFormatBMP
			// 
			this.RBtnFormatBMP.Location = new System.Drawing.Point(17, 34);
			this.RBtnFormatBMP.Name = "RBtnFormatBMP";
			this.RBtnFormatBMP.Size = new System.Drawing.Size(59, 24);
			this.RBtnFormatBMP.TabIndex = 2;
			this.RBtnFormatBMP.Text = "BMP";
			this.RBtnFormatBMP.UseVisualStyleBackColor = true;
			this.RBtnFormatBMP.CheckedChanged += new System.EventHandler(this.BtnImgFormatChanged_Event);
			// 
			// checkBoxAlphaChannel
			// 
			this.checkBoxAlphaChannel.Location = new System.Drawing.Point(37, 95);
			this.checkBoxAlphaChannel.Name = "checkBoxAlphaChannel";
			this.checkBoxAlphaChannel.Size = new System.Drawing.Size(100, 24);
			this.checkBoxAlphaChannel.TabIndex = 4;
			this.checkBoxAlphaChannel.Text = "Alpha channel";
			this.checkBoxAlphaChannel.UseVisualStyleBackColor = true;
			// 
			// image_export_options_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(174, 163);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkBoxAlphaChannel);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "image_export_options_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Export Options";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RadioButton RBtnFormatPCX;
		private System.Windows.Forms.CheckBox checkBoxAlphaChannel;
		private System.Windows.Forms.RadioButton RBtnFormatBMP;
		private System.Windows.Forms.RadioButton RBtnFormatPNG;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
	}
}
