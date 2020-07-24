/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 15.07.2020
 * Time: 11:29
 */
namespace SPReD
{
	partial class SMS_export_form
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
			this.CBoxCHRsBpp = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.NumCHRsOffset = new System.Windows.Forms.NumericUpDown();
			this.BtnTilesOffsetInfo = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.NumCHRsOffset)).BeginInit();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(58, 82);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 1;
			this.BtnOk.Text = "&Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "CHRs BPP:";
			// 
			// CBoxCHRsBpp
			// 
			this.CBoxCHRsBpp.DisplayMember = "4 bpp";
			this.CBoxCHRsBpp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxCHRsBpp.FormattingEnabled = true;
			this.CBoxCHRsBpp.Items.AddRange(new object[] {
									"1 bpp",
									"2 bpp",
									"3 bpp",
									"4 bpp"});
			this.CBoxCHRsBpp.Location = new System.Drawing.Point(81, 10);
			this.CBoxCHRsBpp.Name = "CBoxCHRsBpp";
			this.CBoxCHRsBpp.Size = new System.Drawing.Size(64, 21);
			this.CBoxCHRsBpp.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "CHRs offset:";
			// 
			// NumCHRsOffset
			// 
			this.NumCHRsOffset.Location = new System.Drawing.Point(81, 39);
			this.NumCHRsOffset.Maximum = new decimal(new int[] {
									255,
									0,
									0,
									0});
			this.NumCHRsOffset.Name = "NumCHRsOffset";
			this.NumCHRsOffset.Size = new System.Drawing.Size(64, 20);
			this.NumCHRsOffset.TabIndex = 3;
			// 
			// BtnTilesOffsetInfo
			// 
			this.BtnTilesOffsetInfo.Location = new System.Drawing.Point(151, 39);
			this.BtnTilesOffsetInfo.Name = "BtnTilesOffsetInfo";
			this.BtnTilesOffsetInfo.Size = new System.Drawing.Size(20, 20);
			this.BtnTilesOffsetInfo.TabIndex = 4;
			this.BtnTilesOffsetInfo.Text = "?";
			this.BtnTilesOffsetInfo.UseVisualStyleBackColor = true;
			this.BtnTilesOffsetInfo.Click += new System.EventHandler(this.BtnTilesOffsetInfoClick);
			// 
			// SMS_export_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(187, 117);
			this.Controls.Add(this.BtnTilesOffsetInfo);
			this.Controls.Add(this.NumCHRsOffset);
			this.Controls.Add(this.CBoxCHRsBpp);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BtnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SMS_export_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Export Options";
			((System.ComponentModel.ISupportInitialize)(this.NumCHRsOffset)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button BtnTilesOffsetInfo;
		private System.Windows.Forms.NumericUpDown NumCHRsOffset;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox CBoxCHRsBpp;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button BtnOk;
	}
}
