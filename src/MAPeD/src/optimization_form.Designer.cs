/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 29.11.2018
 * Time: 16:49
 */
namespace MAPeD
{
	partial class optimization_form
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
			this.label1 = new System.Windows.Forms.Label();
			this.CheckBoxOptimizeTiles = new System.Windows.Forms.CheckBox();
			this.CheckBoxOptimizeBlocks = new System.Windows.Forms.CheckBox();
			this.BtnOk = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.CheckBoxOptimizeCHRs = new System.Windows.Forms.CheckBox();
			this.CheckBoxOptimizeScreens = new System.Windows.Forms.CheckBox();
			this.CheckBoxGlobalOptimization = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "WARNING: This operation removes all unused data!";
			// 
			// CheckBoxOptimizeTiles
			// 
			this.CheckBoxOptimizeTiles.Checked = true;
			this.CheckBoxOptimizeTiles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxOptimizeTiles.Location = new System.Drawing.Point(70, 70);
			this.CheckBoxOptimizeTiles.Name = "CheckBoxOptimizeTiles";
			this.CheckBoxOptimizeTiles.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeTiles.TabIndex = 1;
			this.CheckBoxOptimizeTiles.Text = "Tiles";
			this.CheckBoxOptimizeTiles.UseVisualStyleBackColor = true;
			// 
			// CheckBoxOptimizeBlocks
			// 
			this.CheckBoxOptimizeBlocks.Location = new System.Drawing.Point(70, 91);
			this.CheckBoxOptimizeBlocks.Name = "CheckBoxOptimizeBlocks";
			this.CheckBoxOptimizeBlocks.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeBlocks.TabIndex = 2;
			this.CheckBoxOptimizeBlocks.Text = "Blocks";
			this.CheckBoxOptimizeBlocks.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(116, 182);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 4;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			this.BtnOk.Click += new System.EventHandler(this.BtnOkClick_event);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(63, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(157, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Select the data for optimization:";
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(197, 182);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 5;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.BtnCancelClick_event);
			// 
			// CheckBoxOptimizeCHRs
			// 
			this.CheckBoxOptimizeCHRs.Location = new System.Drawing.Point(70, 112);
			this.CheckBoxOptimizeCHRs.Name = "CheckBoxOptimizeCHRs";
			this.CheckBoxOptimizeCHRs.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeCHRs.TabIndex = 3;
			this.CheckBoxOptimizeCHRs.Text = "CHRs";
			this.CheckBoxOptimizeCHRs.UseVisualStyleBackColor = true;
			// 
			// CheckBoxOptimizeScreens
			// 
			this.CheckBoxOptimizeScreens.Location = new System.Drawing.Point(70, 49);
			this.CheckBoxOptimizeScreens.Name = "CheckBoxOptimizeScreens";
			this.CheckBoxOptimizeScreens.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeScreens.TabIndex = 0;
			this.CheckBoxOptimizeScreens.Text = "Screens";
			this.CheckBoxOptimizeScreens.UseVisualStyleBackColor = true;
			// 
			// CheckBoxGlobalOptimization
			// 
			this.CheckBoxGlobalOptimization.Location = new System.Drawing.Point(14, 140);
			this.CheckBoxGlobalOptimization.Name = "CheckBoxGlobalOptimization";
			this.CheckBoxGlobalOptimization.Size = new System.Drawing.Size(256, 34);
			this.CheckBoxGlobalOptimization.TabIndex = 3;
			this.CheckBoxGlobalOptimization.Text = "Optimize all the data banks (otherwise the active data bank will be optimized)";
			this.CheckBoxGlobalOptimization.UseVisualStyleBackColor = true;
			// 
			// optimization_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(284, 216);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.CheckBoxGlobalOptimization);
			this.Controls.Add(this.CheckBoxOptimizeCHRs);
			this.Controls.Add(this.CheckBoxOptimizeScreens);
			this.Controls.Add(this.CheckBoxOptimizeBlocks);
			this.Controls.Add(this.CheckBoxOptimizeTiles);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "optimization_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Data Optimization";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CheckBoxGlobalOptimization;
		private System.Windows.Forms.CheckBox CheckBoxOptimizeScreens;
		private System.Windows.Forms.CheckBox CheckBoxOptimizeCHRs;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.CheckBox CheckBoxOptimizeBlocks;
		private System.Windows.Forms.CheckBox CheckBoxOptimizeTiles;
		private System.Windows.Forms.Label label1;
	}
}
