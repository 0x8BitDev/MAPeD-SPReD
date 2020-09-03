/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
			this.CheckBox1 = new System.Windows.Forms.CheckBox();
			this.CheckBoxTiles = new System.Windows.Forms.CheckBox();
			this.CheckBoxSkipZeroCHRBlock = new System.Windows.Forms.CheckBox();
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.CheckBoxGameLevel = new System.Windows.Forms.CheckBox();
			this.CheckBoxDeleteEmptyScreens = new System.Windows.Forms.CheckBox();
			this.CheckBoxApplyPalette = new System.Windows.Forms.CheckBox();
			this.BtnApplyPaletteDesc = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// CheckBox1
			// 
			this.CheckBox1.Checked = true;
			this.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBox1.Enabled = false;
			this.CheckBox1.Location = new System.Drawing.Point(34, 12);
			this.CheckBox1.Name = "CheckBox1";
			this.CheckBox1.Size = new System.Drawing.Size(63, 19);
			this.CheckBox1.TabIndex = 0;
			this.CheckBox1.Text = "Blocks";
			this.CheckBox1.UseVisualStyleBackColor = true;
			// 
			// CheckBoxTiles
			// 
			this.CheckBoxTiles.Location = new System.Drawing.Point(34, 37);
			this.CheckBoxTiles.Name = "CheckBoxTiles";
			this.CheckBoxTiles.Size = new System.Drawing.Size(63, 19);
			this.CheckBoxTiles.TabIndex = 1;
			this.CheckBoxTiles.Text = "Tiles";
			this.CheckBoxTiles.UseVisualStyleBackColor = true;
			// 
			// CheckBoxSkipZeroCHRBlock
			// 
			this.CheckBoxSkipZeroCHRBlock.Location = new System.Drawing.Point(34, 137);
			this.CheckBoxSkipZeroCHRBlock.Name = "CheckBoxSkipZeroCHRBlock";
			this.CheckBoxSkipZeroCHRBlock.Size = new System.Drawing.Size(131, 19);
			this.CheckBoxSkipZeroCHRBlock.TabIndex = 4;
			this.CheckBoxSkipZeroCHRBlock.Text = "Skip zero CHR\\Block";
			this.CheckBoxSkipZeroCHRBlock.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(22, 170);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 5;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(103, 170);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 6;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// CheckBoxGameLevel
			// 
			this.CheckBoxGameLevel.Location = new System.Drawing.Point(34, 62);
			this.CheckBoxGameLevel.Name = "CheckBoxGameLevel";
			this.CheckBoxGameLevel.Size = new System.Drawing.Size(98, 19);
			this.CheckBoxGameLevel.TabIndex = 2;
			this.CheckBoxGameLevel.Text = "Game level";
			this.CheckBoxGameLevel.UseVisualStyleBackColor = true;
			this.CheckBoxGameLevel.CheckedChanged += new System.EventHandler(this.CheckBoxGameLevelChanged_Event);
			// 
			// CheckBoxDeleteEmptyScreens
			// 
			this.CheckBoxDeleteEmptyScreens.Enabled = false;
			this.CheckBoxDeleteEmptyScreens.Location = new System.Drawing.Point(34, 87);
			this.CheckBoxDeleteEmptyScreens.Name = "CheckBoxDeleteEmptyScreens";
			this.CheckBoxDeleteEmptyScreens.Size = new System.Drawing.Size(144, 19);
			this.CheckBoxDeleteEmptyScreens.TabIndex = 3;
			this.CheckBoxDeleteEmptyScreens.Text = "Delete empty screens";
			this.CheckBoxDeleteEmptyScreens.UseVisualStyleBackColor = true;
			this.CheckBoxDeleteEmptyScreens.CheckedChanged += new System.EventHandler(this.CheckBoxGameLevelChanged_Event);
			// 
			// CheckBoxApplyPalette
			// 
			this.CheckBoxApplyPalette.Location = new System.Drawing.Point(34, 112);
			this.CheckBoxApplyPalette.Name = "CheckBoxApplyPalette";
			this.CheckBoxApplyPalette.Size = new System.Drawing.Size(144, 19);
			this.CheckBoxApplyPalette.TabIndex = 3;
			this.CheckBoxApplyPalette.Text = "Apply palette (2x2)";
			this.CheckBoxApplyPalette.UseVisualStyleBackColor = true;
			this.CheckBoxApplyPalette.CheckedChanged += new System.EventHandler(this.CheckBoxApplyPaletteChanged_Event);
			// 
			// BtnApplyPaletteDesc
			// 
			this.BtnApplyPaletteDesc.Location = new System.Drawing.Point(158, 111);
			this.BtnApplyPaletteDesc.Name = "BtnApplyPaletteDesc";
			this.BtnApplyPaletteDesc.Size = new System.Drawing.Size(20, 20);
			this.BtnApplyPaletteDesc.TabIndex = 7;
			this.BtnApplyPaletteDesc.Text = "?";
			this.BtnApplyPaletteDesc.UseVisualStyleBackColor = true;
			this.BtnApplyPaletteDesc.Click += new System.EventHandler(this.BtnApplyPaletteDescClick_Event);
			// 
			// import_tiles_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(202, 205);
			this.Controls.Add(this.BtnApplyPaletteDesc);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.CheckBoxSkipZeroCHRBlock);
			this.Controls.Add(this.CheckBoxApplyPalette);
			this.Controls.Add(this.CheckBoxDeleteEmptyScreens);
			this.Controls.Add(this.CheckBoxGameLevel);
			this.Controls.Add(this.CheckBoxTiles);
			this.Controls.Add(this.CheckBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "import_tiles_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Import Options";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button BtnApplyPaletteDesc;
		private System.Windows.Forms.CheckBox CheckBoxApplyPalette;
		private System.Windows.Forms.CheckBox CheckBoxDeleteEmptyScreens;
		private System.Windows.Forms.CheckBox CheckBoxGameLevel;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.CheckBox CheckBoxSkipZeroCHRBlock;
		private System.Windows.Forms.CheckBox CheckBoxTiles;
		private System.Windows.Forms.CheckBox CheckBox1;
	}
}
