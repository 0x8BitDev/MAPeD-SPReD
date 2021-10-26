/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 24.07.2020
 * Time: 10:25
 */
namespace MAPeD
{
	partial class data_conversion_options_form
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.RBtnScrAlignLeftBottom = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignRightBottom = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignMidBottom = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignLeftCenter = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignRightCenter = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignMidCenter = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignLeftTop = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignRightTop = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignMidTop = new System.Windows.Forms.RadioButton();
			this.CBoxConvertColors = new System.Windows.Forms.CheckBox();
			this.BtnOk = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.CBoxUseFileScreenResolution = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RBtnScrAlignLeftBottom);
			this.groupBox1.Controls.Add(this.RBtnScrAlignRightBottom);
			this.groupBox1.Controls.Add(this.RBtnScrAlignMidBottom);
			this.groupBox1.Controls.Add(this.RBtnScrAlignLeftCenter);
			this.groupBox1.Controls.Add(this.RBtnScrAlignRightCenter);
			this.groupBox1.Controls.Add(this.RBtnScrAlignMidCenter);
			this.groupBox1.Controls.Add(this.RBtnScrAlignLeftTop);
			this.groupBox1.Controls.Add(this.RBtnScrAlignRightTop);
			this.groupBox1.Controls.Add(this.RBtnScrAlignMidTop);
			this.groupBox1.Location = new System.Drawing.Point(16, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(156, 100);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Screen Align:";
			// 
			// RBtnScrAlignLeftBottom
			// 
			this.RBtnScrAlignLeftBottom.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignLeftBottom.Location = new System.Drawing.Point(12, 71);
			this.RBtnScrAlignLeftBottom.Name = "RBtnScrAlignLeftBottom";
			this.RBtnScrAlignLeftBottom.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignLeftBottom.TabIndex = 7;
			this.RBtnScrAlignLeftBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignLeftBottom.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignRightBottom
			// 
			this.RBtnScrAlignRightBottom.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignRightBottom.Location = new System.Drawing.Point(104, 71);
			this.RBtnScrAlignRightBottom.Name = "RBtnScrAlignRightBottom";
			this.RBtnScrAlignRightBottom.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignRightBottom.TabIndex = 9;
			this.RBtnScrAlignRightBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignRightBottom.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignMidBottom
			// 
			this.RBtnScrAlignMidBottom.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignMidBottom.Location = new System.Drawing.Point(58, 71);
			this.RBtnScrAlignMidBottom.Name = "RBtnScrAlignMidBottom";
			this.RBtnScrAlignMidBottom.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignMidBottom.TabIndex = 8;
			this.RBtnScrAlignMidBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignMidBottom.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignLeftCenter
			// 
			this.RBtnScrAlignLeftCenter.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignLeftCenter.Location = new System.Drawing.Point(12, 45);
			this.RBtnScrAlignLeftCenter.Name = "RBtnScrAlignLeftCenter";
			this.RBtnScrAlignLeftCenter.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignLeftCenter.TabIndex = 4;
			this.RBtnScrAlignLeftCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignLeftCenter.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignRightCenter
			// 
			this.RBtnScrAlignRightCenter.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignRightCenter.Location = new System.Drawing.Point(104, 45);
			this.RBtnScrAlignRightCenter.Name = "RBtnScrAlignRightCenter";
			this.RBtnScrAlignRightCenter.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignRightCenter.TabIndex = 6;
			this.RBtnScrAlignRightCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignRightCenter.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignMidCenter
			// 
			this.RBtnScrAlignMidCenter.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignMidCenter.Checked = true;
			this.RBtnScrAlignMidCenter.Location = new System.Drawing.Point(58, 45);
			this.RBtnScrAlignMidCenter.Name = "RBtnScrAlignMidCenter";
			this.RBtnScrAlignMidCenter.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignMidCenter.TabIndex = 5;
			this.RBtnScrAlignMidCenter.TabStop = true;
			this.RBtnScrAlignMidCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignMidCenter.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignLeftTop
			// 
			this.RBtnScrAlignLeftTop.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignLeftTop.Location = new System.Drawing.Point(12, 19);
			this.RBtnScrAlignLeftTop.Name = "RBtnScrAlignLeftTop";
			this.RBtnScrAlignLeftTop.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignLeftTop.TabIndex = 1;
			this.RBtnScrAlignLeftTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignLeftTop.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignRightTop
			// 
			this.RBtnScrAlignRightTop.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignRightTop.Location = new System.Drawing.Point(104, 19);
			this.RBtnScrAlignRightTop.Name = "RBtnScrAlignRightTop";
			this.RBtnScrAlignRightTop.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignRightTop.TabIndex = 3;
			this.RBtnScrAlignRightTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignRightTop.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignMidTop
			// 
			this.RBtnScrAlignMidTop.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnScrAlignMidTop.Location = new System.Drawing.Point(58, 19);
			this.RBtnScrAlignMidTop.Name = "RBtnScrAlignMidTop";
			this.RBtnScrAlignMidTop.Size = new System.Drawing.Size(40, 20);
			this.RBtnScrAlignMidTop.TabIndex = 2;
			this.RBtnScrAlignMidTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnScrAlignMidTop.UseVisualStyleBackColor = true;
			// 
			// CBoxConvertColors
			// 
			this.CBoxConvertColors.Checked = true;
			this.CBoxConvertColors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxConvertColors.Location = new System.Drawing.Point(16, 118);
			this.CBoxConvertColors.Name = "CBoxConvertColors";
			this.CBoxConvertColors.Size = new System.Drawing.Size(104, 20);
			this.CBoxConvertColors.TabIndex = 10;
			this.CBoxConvertColors.Text = "Convert Colors";
			this.CBoxConvertColors.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(16, 177);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 12;
			this.BtnOk.Text = "&Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(97, 177);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 13;
			this.button1.Text = "&Cancel";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// CBoxUseFileScreenResolution
			// 
			this.CBoxUseFileScreenResolution.Checked = true;
			this.CBoxUseFileScreenResolution.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxUseFileScreenResolution.Location = new System.Drawing.Point(16, 142);
			this.CBoxUseFileScreenResolution.Name = "CBoxUseFileScreenResolution";
			this.CBoxUseFileScreenResolution.Size = new System.Drawing.Size(156, 20);
			this.CBoxUseFileScreenResolution.TabIndex = 11;
			this.CBoxUseFileScreenResolution.Text = "Use File Screen Resolution";
			this.CBoxUseFileScreenResolution.UseVisualStyleBackColor = true;
			// 
			// data_conversion_options_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(188, 213);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.CBoxUseFileScreenResolution);
			this.Controls.Add(this.CBoxConvertColors);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "data_conversion_options_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CBoxUseFileScreenResolution;
		private System.Windows.Forms.RadioButton RBtnScrAlignRightTop;
		private System.Windows.Forms.RadioButton RBtnScrAlignLeftTop;
		private System.Windows.Forms.RadioButton RBtnScrAlignRightCenter;
		private System.Windows.Forms.RadioButton RBtnScrAlignLeftCenter;
		private System.Windows.Forms.RadioButton RBtnScrAlignRightBottom;
		private System.Windows.Forms.RadioButton RBtnScrAlignLeftBottom;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.CheckBox CBoxConvertColors;
		private System.Windows.Forms.RadioButton RBtnScrAlignMidTop;
		private System.Windows.Forms.RadioButton RBtnScrAlignMidCenter;
		private System.Windows.Forms.RadioButton RBtnScrAlignMidBottom;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
