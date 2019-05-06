/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
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
			this.RradioButtonTiles = new System.Windows.Forms.RadioButton();
			this.RadioButtonBlocks = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(14, 90);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 3;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(99, 90);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 4;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// RradioButtonTiles
			// 
			this.RradioButtonTiles.Checked = true;
			this.RradioButtonTiles.Location = new System.Drawing.Point(29, 19);
			this.RradioButtonTiles.Name = "RradioButtonTiles";
			this.RradioButtonTiles.Size = new System.Drawing.Size(70, 24);
			this.RradioButtonTiles.TabIndex = 1;
			this.RradioButtonTiles.TabStop = true;
			this.RradioButtonTiles.Text = "Tiles";
			this.RradioButtonTiles.UseVisualStyleBackColor = true;
			// 
			// RadioButtonBlocks
			// 
			this.RadioButtonBlocks.Location = new System.Drawing.Point(29, 42);
			this.RadioButtonBlocks.Name = "RadioButtonBlocks";
			this.RadioButtonBlocks.Size = new System.Drawing.Size(70, 24);
			this.RadioButtonBlocks.TabIndex = 2;
			this.RadioButtonBlocks.Text = "Blocks";
			this.RadioButtonBlocks.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RradioButtonTiles);
			this.groupBox1.Controls.Add(this.RadioButtonBlocks);
			this.groupBox1.Location = new System.Drawing.Point(34, 2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(120, 76);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Export Data:";
			// 
			// export_active_tile_block_set_form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(187, 122);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "export_active_tile_block_set_form";
			this.Text = "Options";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton RadioButtonBlocks;
		private System.Windows.Forms.RadioButton RradioButtonTiles;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
	}
}
