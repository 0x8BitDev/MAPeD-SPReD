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
			this.RBtnTilesOrderLine = new System.Windows.Forms.RadioButton();
			this.RBtnTilesOrderRect16xN = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(40, 97);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 3;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(125, 97);
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
			this.groupBox2.Controls.Add(this.RBtnTilesOrderLine);
			this.groupBox2.Controls.Add(this.RBtnTilesOrderRect16xN);
			this.groupBox2.Location = new System.Drawing.Point(110, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(116, 76);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Data Order:";
			// 
			// RBtnTilesOrderLine
			// 
			this.RBtnTilesOrderLine.Location = new System.Drawing.Point(15, 42);
			this.RBtnTilesOrderLine.Name = "RBtnTilesOrderLine";
			this.RBtnTilesOrderLine.Size = new System.Drawing.Size(95, 24);
			this.RBtnTilesOrderLine.TabIndex = 0;
			this.RBtnTilesOrderLine.Text = "Line";
			this.RBtnTilesOrderLine.UseVisualStyleBackColor = true;
			// 
			// RBtnTilesOrderRect16xN
			// 
			this.RBtnTilesOrderRect16xN.Checked = true;
			this.RBtnTilesOrderRect16xN.Location = new System.Drawing.Point(15, 19);
			this.RBtnTilesOrderRect16xN.Name = "RBtnTilesOrderRect16xN";
			this.RBtnTilesOrderRect16xN.Size = new System.Drawing.Size(95, 24);
			this.RBtnTilesOrderRect16xN.TabIndex = 0;
			this.RBtnTilesOrderRect16xN.TabStop = true;
			this.RBtnTilesOrderRect16xN.Text = "Rect (16 x N)";
			this.RBtnTilesOrderRect16xN.UseVisualStyleBackColor = true;
			// 
			// export_active_tile_block_set_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(233, 128);
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
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.RadioButton RBtnTilesOrderRect16xN;
		private System.Windows.Forms.RadioButton RBtnTilesOrderLine;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton RBtnBlocks;
		private System.Windows.Forms.RadioButton RBtnTiles;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
	}
}
