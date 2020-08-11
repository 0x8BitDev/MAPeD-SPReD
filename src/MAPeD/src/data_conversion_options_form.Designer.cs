/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
			this.RBtnScrAlignBottom = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignCenter = new System.Windows.Forms.RadioButton();
			this.RBtnScrAlignTop = new System.Windows.Forms.RadioButton();
			this.CBoxConvertColors = new System.Windows.Forms.CheckBox();
			this.BtnOk = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RBtnScrAlignBottom);
			this.groupBox1.Controls.Add(this.RBtnScrAlignCenter);
			this.groupBox1.Controls.Add(this.RBtnScrAlignTop);
			this.groupBox1.Location = new System.Drawing.Point(42, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(102, 100);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Screens Align:";
			// 
			// RBtnScrAlignBottom
			// 
			this.RBtnScrAlignBottom.Location = new System.Drawing.Point(12, 70);
			this.RBtnScrAlignBottom.Name = "RBtnScrAlignBottom";
			this.RBtnScrAlignBottom.Size = new System.Drawing.Size(75, 20);
			this.RBtnScrAlignBottom.TabIndex = 3;
			this.RBtnScrAlignBottom.Text = "Bottom";
			this.RBtnScrAlignBottom.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignCenter
			// 
			this.RBtnScrAlignCenter.Location = new System.Drawing.Point(12, 44);
			this.RBtnScrAlignCenter.Name = "RBtnScrAlignCenter";
			this.RBtnScrAlignCenter.Size = new System.Drawing.Size(75, 20);
			this.RBtnScrAlignCenter.TabIndex = 2;
			this.RBtnScrAlignCenter.Text = "Center";
			this.RBtnScrAlignCenter.UseVisualStyleBackColor = true;
			// 
			// RBtnScrAlignTop
			// 
			this.RBtnScrAlignTop.Checked = true;
			this.RBtnScrAlignTop.Location = new System.Drawing.Point(12, 18);
			this.RBtnScrAlignTop.Name = "RBtnScrAlignTop";
			this.RBtnScrAlignTop.Size = new System.Drawing.Size(75, 20);
			this.RBtnScrAlignTop.TabIndex = 1;
			this.RBtnScrAlignTop.TabStop = true;
			this.RBtnScrAlignTop.Text = "Top";
			this.RBtnScrAlignTop.UseVisualStyleBackColor = true;
			// 
			// CBoxConvertColors
			// 
			this.CBoxConvertColors.Checked = true;
			this.CBoxConvertColors.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxConvertColors.Location = new System.Drawing.Point(42, 118);
			this.CBoxConvertColors.Name = "CBoxConvertColors";
			this.CBoxConvertColors.Size = new System.Drawing.Size(104, 24);
			this.CBoxConvertColors.TabIndex = 4;
			this.CBoxConvertColors.Text = "Convert Colors";
			this.CBoxConvertColors.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(16, 155);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 5;
			this.BtnOk.Text = "&Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(97, 155);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "&Cancel";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// data_conversion_options_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(188, 190);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.BtnOk);
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
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.CheckBox CBoxConvertColors;
		private System.Windows.Forms.RadioButton RBtnScrAlignTop;
		private System.Windows.Forms.RadioButton RBtnScrAlignCenter;
		private System.Windows.Forms.RadioButton RBtnScrAlignBottom;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
