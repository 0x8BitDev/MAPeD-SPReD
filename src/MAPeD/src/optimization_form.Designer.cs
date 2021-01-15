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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.BtnMatchedBlocksInfo = new System.Windows.Forms.Button();
			this.NumUpDownMatcingPercent = new System.Windows.Forms.NumericUpDown();
			this.BtnCheckMatchedBlocks = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumUpDownMatcingPercent)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(256, 30);
			this.label1.TabIndex = 0;
			this.label1.Text = "WARNING: This operation removes all unused/duplicate data!";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// CheckBoxOptimizeTiles
			// 
			this.CheckBoxOptimizeTiles.Checked = true;
			this.CheckBoxOptimizeTiles.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxOptimizeTiles.Location = new System.Drawing.Point(71, 86);
			this.CheckBoxOptimizeTiles.Name = "CheckBoxOptimizeTiles";
			this.CheckBoxOptimizeTiles.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeTiles.TabIndex = 1;
			this.CheckBoxOptimizeTiles.Text = "Tiles";
			this.CheckBoxOptimizeTiles.UseVisualStyleBackColor = true;
			// 
			// CheckBoxOptimizeBlocks
			// 
			this.CheckBoxOptimizeBlocks.Location = new System.Drawing.Point(71, 107);
			this.CheckBoxOptimizeBlocks.Name = "CheckBoxOptimizeBlocks";
			this.CheckBoxOptimizeBlocks.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeBlocks.TabIndex = 2;
			this.CheckBoxOptimizeBlocks.Text = "Blocks";
			this.CheckBoxOptimizeBlocks.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(119, 266);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 4;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			this.BtnOk.Click += new System.EventHandler(this.BtnOkClick_event);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(64, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(157, 20);
			this.label2.TabIndex = 3;
			this.label2.Text = "Select data for optimization:";
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(200, 266);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 5;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.BtnCancelClick_event);
			// 
			// CheckBoxOptimizeCHRs
			// 
			this.CheckBoxOptimizeCHRs.Location = new System.Drawing.Point(71, 128);
			this.CheckBoxOptimizeCHRs.Name = "CheckBoxOptimizeCHRs";
			this.CheckBoxOptimizeCHRs.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeCHRs.TabIndex = 3;
			this.CheckBoxOptimizeCHRs.Text = "CHRs";
			this.CheckBoxOptimizeCHRs.UseVisualStyleBackColor = true;
			// 
			// CheckBoxOptimizeScreens
			// 
			this.CheckBoxOptimizeScreens.Location = new System.Drawing.Point(71, 65);
			this.CheckBoxOptimizeScreens.Name = "CheckBoxOptimizeScreens";
			this.CheckBoxOptimizeScreens.Size = new System.Drawing.Size(68, 24);
			this.CheckBoxOptimizeScreens.TabIndex = 0;
			this.CheckBoxOptimizeScreens.Text = "Screens";
			this.CheckBoxOptimizeScreens.UseVisualStyleBackColor = true;
			// 
			// CheckBoxGlobalOptimization
			// 
			this.CheckBoxGlobalOptimization.Location = new System.Drawing.Point(15, 156);
			this.CheckBoxGlobalOptimization.Name = "CheckBoxGlobalOptimization";
			this.CheckBoxGlobalOptimization.Size = new System.Drawing.Size(256, 34);
			this.CheckBoxGlobalOptimization.TabIndex = 3;
			this.CheckBoxGlobalOptimization.Text = "Optimize all the data banks (otherwise the active data bank will be optimized)";
			this.CheckBoxGlobalOptimization.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.BtnMatchedBlocksInfo);
			this.groupBox1.Controls.Add(this.NumUpDownMatcingPercent);
			this.groupBox1.Controls.Add(this.BtnCheckMatchedBlocks);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(14, 194);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(256, 58);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Blocks (2x2) Matching Degree";
			// 
			// BtnMatchedBlocksInfo
			// 
			this.BtnMatchedBlocksInfo.Location = new System.Drawing.Point(199, 23);
			this.BtnMatchedBlocksInfo.Name = "BtnMatchedBlocksInfo";
			this.BtnMatchedBlocksInfo.Size = new System.Drawing.Size(20, 20);
			this.BtnMatchedBlocksInfo.TabIndex = 3;
			this.BtnMatchedBlocksInfo.Text = "?";
			this.BtnMatchedBlocksInfo.UseVisualStyleBackColor = true;
			this.BtnMatchedBlocksInfo.Click += new System.EventHandler(this.BtnMatchedBlocksInfoClick_Event);
			// 
			// NumUpDownMatcingPercent
			// 
			this.NumUpDownMatcingPercent.Location = new System.Drawing.Point(53, 24);
			this.NumUpDownMatcingPercent.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumUpDownMatcingPercent.Name = "NumUpDownMatcingPercent";
			this.NumUpDownMatcingPercent.Size = new System.Drawing.Size(58, 20);
			this.NumUpDownMatcingPercent.TabIndex = 0;
			this.NumUpDownMatcingPercent.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			// 
			// BtnCheckMatchedBlocks
			// 
			this.BtnCheckMatchedBlocks.Location = new System.Drawing.Point(118, 22);
			this.BtnCheckMatchedBlocks.Name = "BtnCheckMatchedBlocks";
			this.BtnCheckMatchedBlocks.Size = new System.Drawing.Size(75, 23);
			this.BtnCheckMatchedBlocks.TabIndex = 2;
			this.BtnCheckMatchedBlocks.Text = "Check";
			this.BtnCheckMatchedBlocks.UseVisualStyleBackColor = true;
			this.BtnCheckMatchedBlocks.Click += new System.EventHandler(this.BtnCheckMatchedBlocksClick_Event);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(36, 26);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(20, 19);
			this.label3.TabIndex = 1;
			this.label3.Text = "%";
			// 
			// optimization_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(284, 301);
			this.Controls.Add(this.groupBox1);
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
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumUpDownMatcingPercent)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button BtnMatchedBlocksInfo;
		private System.Windows.Forms.NumericUpDown NumUpDownMatcingPercent;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button BtnCheckMatchedBlocks;
		private System.Windows.Forms.GroupBox groupBox1;
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
