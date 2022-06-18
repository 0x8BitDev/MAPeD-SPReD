/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 01.03.2022
 * Time: 18:28
 */
namespace SPReD
{
	partial class PCE_export_form
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
			this.label1 = new System.Windows.Forms.Label();
			this.NumCHRsOffset = new System.Windows.Forms.NumericUpDown();
			this.LabelVADDR = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.NumPaletteSlot = new System.Windows.Forms.NumericUpDown();
			this.CheckBoxCommentCHRData = new System.Windows.Forms.CheckBox();
			this.NumVADDR = new System.Windows.Forms.NumericUpDown();
			this.TextBoxDataDir = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.NumCHRsOffset)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumPaletteSlot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumVADDR)).BeginInit();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(102, 127);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 7;
			this.BtnOk.Text = "&Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "CHRs offset:";
			// 
			// NumCHRsOffset
			// 
			this.NumCHRsOffset.Location = new System.Drawing.Point(81, 20);
			this.NumCHRsOffset.Maximum = new decimal(new int[] {
									512,
									0,
									0,
									0});
			this.NumCHRsOffset.Name = "NumCHRsOffset";
			this.NumCHRsOffset.Size = new System.Drawing.Size(61, 20);
			this.NumCHRsOffset.TabIndex = 1;
			this.NumCHRsOffset.ValueChanged += new System.EventHandler(this.NumCHRsOffsetChanged_Event);
			this.NumCHRsOffset.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumCHRsOffsetChanged_Event);
			// 
			// LabelVADDR
			// 
			this.LabelVADDR.Location = new System.Drawing.Point(148, 22);
			this.LabelVADDR.Name = "LabelVADDR";
			this.LabelVADDR.Size = new System.Drawing.Size(61, 23);
			this.LabelVADDR.TabIndex = 2;
			this.LabelVADDR.Text = "VADDR#:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "Palette slot:";
			// 
			// NumPaletteSlot
			// 
			this.NumPaletteSlot.Location = new System.Drawing.Point(81, 46);
			this.NumPaletteSlot.Maximum = new decimal(new int[] {
									15,
									0,
									0,
									0});
			this.NumPaletteSlot.Name = "NumPaletteSlot";
			this.NumPaletteSlot.Size = new System.Drawing.Size(61, 20);
			this.NumPaletteSlot.TabIndex = 5;
			this.NumPaletteSlot.ValueChanged += new System.EventHandler(this.NumCHRsOffsetChanged_Event);
			this.NumPaletteSlot.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumCHRsOffsetChanged_Event);
			// 
			// CheckBoxCommentCHRData
			// 
			this.CheckBoxCommentCHRData.Checked = true;
			this.CheckBoxCommentCHRData.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxCommentCHRData.Location = new System.Drawing.Point(148, 48);
			this.CheckBoxCommentCHRData.Name = "CheckBoxCommentCHRData";
			this.CheckBoxCommentCHRData.Size = new System.Drawing.Size(126, 17);
			this.CheckBoxCommentCHRData.TabIndex = 6;
			this.CheckBoxCommentCHRData.Text = "Comment  CHR data";
			this.CheckBoxCommentCHRData.UseVisualStyleBackColor = true;
			// 
			// NumVADDR
			// 
			this.NumVADDR.Hexadecimal = true;
			this.NumVADDR.Increment = new decimal(new int[] {
									64,
									0,
									0,
									0});
			this.NumVADDR.Location = new System.Drawing.Point(202, 20);
			this.NumVADDR.Maximum = new decimal(new int[] {
									32768,
									0,
									0,
									0});
			this.NumVADDR.Name = "NumVADDR";
			this.NumVADDR.Size = new System.Drawing.Size(61, 20);
			this.NumVADDR.TabIndex = 3;
			this.NumVADDR.ValueChanged += new System.EventHandler(this.NumVADDRChanged_Event);
			this.NumVADDR.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NumVADDRKeyUp_Event);
			// 
			// TextBoxDataDir
			// 
			this.TextBoxDataDir.Location = new System.Drawing.Point(13, 91);
			this.TextBoxDataDir.MaxLength = 256;
			this.TextBoxDataDir.Name = "TextBoxDataDir";
			this.TextBoxDataDir.Size = new System.Drawing.Size(251, 20);
			this.TextBoxDataDir.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 73);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(251, 18);
			this.label2.TabIndex = 4;
			this.label2.Text = "Data directory (e.g. \'data/sprites\'):";
			// 
			// PCE_export_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(278, 162);
			this.Controls.Add(this.TextBoxDataDir);
			this.Controls.Add(this.NumVADDR);
			this.Controls.Add(this.CheckBoxCommentCHRData);
			this.Controls.Add(this.LabelVADDR);
			this.Controls.Add(this.NumPaletteSlot);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.NumCHRsOffset);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PCE_export_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export Options";
			((System.ComponentModel.ISupportInitialize)(this.NumCHRsOffset)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumPaletteSlot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumVADDR)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TextBoxDataDir;
		private System.Windows.Forms.NumericUpDown NumVADDR;
		private System.Windows.Forms.CheckBox CheckBoxCommentCHRData;
		private System.Windows.Forms.NumericUpDown NumPaletteSlot;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label LabelVADDR;
		private System.Windows.Forms.NumericUpDown NumCHRsOffset;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button BtnOk;
	}
}
