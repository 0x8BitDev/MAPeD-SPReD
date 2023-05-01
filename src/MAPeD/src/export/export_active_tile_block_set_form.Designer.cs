/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 28.12.2018
 * Time: 16:46
 */
namespace MAPeD
{
	partial class export_active_tile_block_set_form
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
			this.RBtnTiles = new System.Windows.Forms.RadioButton();
			this.RBtnBlocks = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.NumericUpDownImgsCntWidth = new System.Windows.Forms.NumericUpDown();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownImgsCntWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(26, 103);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 3;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(111, 103);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 4;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// RBtnTiles
			// 
			this.RBtnTiles.Checked = true;
			this.RBtnTiles.Location = new System.Drawing.Point(15, 19);
			this.RBtnTiles.Name = "RBtnTiles";
			this.RBtnTiles.Size = new System.Drawing.Size(70, 24);
			this.RBtnTiles.TabIndex = 1;
			this.RBtnTiles.TabStop = true;
			this.RBtnTiles.Text = "Tiles";
			this.RBtnTiles.UseVisualStyleBackColor = true;
			// 
			// RBtnBlocks
			// 
			this.RBtnBlocks.Location = new System.Drawing.Point(15, 42);
			this.RBtnBlocks.Name = "RBtnBlocks";
			this.RBtnBlocks.Size = new System.Drawing.Size(70, 24);
			this.RBtnBlocks.TabIndex = 2;
			this.RBtnBlocks.Text = "Blocks";
			this.RBtnBlocks.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RBtnTiles);
			this.groupBox1.Controls.Add(this.RBtnBlocks);
			this.groupBox1.Location = new System.Drawing.Point(7, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(96, 76);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Export:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.NumericUpDownImgsCntWidth);
			this.groupBox2.Location = new System.Drawing.Point(109, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(96, 76);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Output Data:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Rectangle:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(62, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(25, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "x N";
			// 
			// NumericUpDownImgsCntWidth
			// 
			this.NumericUpDownImgsCntWidth.Location = new System.Drawing.Point(15, 41);
			this.NumericUpDownImgsCntWidth.Maximum = new decimal(new int[] {
									256,
									0,
									0,
									0});
			this.NumericUpDownImgsCntWidth.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumericUpDownImgsCntWidth.Name = "NumericUpDownImgsCntWidth";
			this.NumericUpDownImgsCntWidth.Size = new System.Drawing.Size(45, 20);
			this.NumericUpDownImgsCntWidth.TabIndex = 0;
			this.NumericUpDownImgsCntWidth.Value = new decimal(new int[] {
									16,
									0,
									0,
									0});
			// 
			// export_active_tile_block_set_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(212, 135);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "export_active_tile_block_set_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownImgsCntWidth)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.NumericUpDown NumericUpDownImgsCntWidth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton RBtnBlocks;
		private System.Windows.Forms.RadioButton RBtnTiles;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
	}
}
