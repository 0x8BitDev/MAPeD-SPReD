/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 14.12.2018
 * Time: 19:38
 */
namespace MAPeD
{
	partial class import_tiles_form
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
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.checkBoxTiles = new System.Windows.Forms.CheckBox();
			this.checkBoxSkipZeroCHRBlock = new System.Windows.Forms.CheckBox();
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.checkBoxGameLevel = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBox1
			// 
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Enabled = false;
			this.checkBox1.Location = new System.Drawing.Point(34, 12);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(63, 19);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "Blocks";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// checkBoxTiles
			// 
			this.checkBoxTiles.Location = new System.Drawing.Point(34, 37);
			this.checkBoxTiles.Name = "checkBoxTiles";
			this.checkBoxTiles.Size = new System.Drawing.Size(63, 19);
			this.checkBoxTiles.TabIndex = 1;
			this.checkBoxTiles.Text = "Tiles";
			this.checkBoxTiles.UseVisualStyleBackColor = true;
			// 
			// checkBoxSkipZeroCHRBlock
			// 
			this.checkBoxSkipZeroCHRBlock.Location = new System.Drawing.Point(34, 97);
			this.checkBoxSkipZeroCHRBlock.Name = "checkBoxSkipZeroCHRBlock";
			this.checkBoxSkipZeroCHRBlock.Size = new System.Drawing.Size(131, 19);
			this.checkBoxSkipZeroCHRBlock.TabIndex = 2;
			this.checkBoxSkipZeroCHRBlock.Text = "Skip zero CHR\\Block";
			this.checkBoxSkipZeroCHRBlock.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(9, 131);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 3;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(90, 131);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 4;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// checkBoxGameLevel
			// 
			this.checkBoxGameLevel.Location = new System.Drawing.Point(34, 62);
			this.checkBoxGameLevel.Name = "checkBoxGameLevel";
			this.checkBoxGameLevel.Size = new System.Drawing.Size(98, 19);
			this.checkBoxGameLevel.TabIndex = 1;
			this.checkBoxGameLevel.Text = "Game Level";
			this.checkBoxGameLevel.UseVisualStyleBackColor = true;
			this.checkBoxGameLevel.CheckedChanged += new System.EventHandler(this.CheckBoxGameLevelChanged_Event);
			// 
			// import_tiles_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(177, 165);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.checkBoxSkipZeroCHRBlock);
			this.Controls.Add(this.checkBoxGameLevel);
			this.Controls.Add(this.checkBoxTiles);
			this.Controls.Add(this.checkBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "import_tiles_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Import Tiles Options";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox checkBoxGameLevel;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.CheckBox checkBoxSkipZeroCHRBlock;
		private System.Windows.Forms.CheckBox checkBoxTiles;
		private System.Windows.Forms.CheckBox checkBox1;
	}
}
