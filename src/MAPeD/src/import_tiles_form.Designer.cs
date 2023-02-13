/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
			this.CheckBoxBlocks = new System.Windows.Forms.CheckBox();
			this.CheckBoxTiles = new System.Windows.Forms.CheckBox();
			this.CheckBoxSkipZeroCHRBlock = new System.Windows.Forms.CheckBox();
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.CheckBoxGameMap = new System.Windows.Forms.CheckBox();
			this.CheckBoxDeleteEmptyScreens = new System.Windows.Forms.CheckBox();
			this.CheckBoxApplyPalette = new System.Windows.Forms.CheckBox();
			this.BtnApplyPaletteDesc = new System.Windows.Forms.Button();
			this.CheckBoxCHRs = new System.Windows.Forms.CheckBox();
			this.CheckBoxImportPaletteASIS = new System.Windows.Forms.CheckBox();
			this.BtnImportPaletteASIS = new System.Windows.Forms.Button();
			this.labelStartPalette = new System.Windows.Forms.Label();
			this.CBoxStartPalette = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// CheckBoxBlocks
			// 
			this.CheckBoxBlocks.Checked = true;
			this.CheckBoxBlocks.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxBlocks.Location = new System.Drawing.Point(34, 39);
			this.CheckBoxBlocks.Name = "CheckBoxBlocks";
			this.CheckBoxBlocks.Size = new System.Drawing.Size(63, 19);
			this.CheckBoxBlocks.TabIndex = 1;
			this.CheckBoxBlocks.Text = "Blocks";
			this.CheckBoxBlocks.UseVisualStyleBackColor = true;
			this.CheckBoxBlocks.CheckedChanged += new System.EventHandler(this.CheckBoxBlocksChanged);
			// 
			// CheckBoxTiles
			// 
			this.CheckBoxTiles.Location = new System.Drawing.Point(34, 64);
			this.CheckBoxTiles.Name = "CheckBoxTiles";
			this.CheckBoxTiles.Size = new System.Drawing.Size(63, 19);
			this.CheckBoxTiles.TabIndex = 2;
			this.CheckBoxTiles.Text = "Tiles";
			this.CheckBoxTiles.UseVisualStyleBackColor = true;
			this.CheckBoxTiles.CheckedChanged += new System.EventHandler(this.CheckBoxTilesChanged);
			// 
			// CheckBoxSkipZeroCHRBlock
			// 
			this.CheckBoxSkipZeroCHRBlock.Location = new System.Drawing.Point(34, 139);
			this.CheckBoxSkipZeroCHRBlock.Name = "CheckBoxSkipZeroCHRBlock";
			this.CheckBoxSkipZeroCHRBlock.Size = new System.Drawing.Size(130, 19);
			this.CheckBoxSkipZeroCHRBlock.TabIndex = 5;
			this.CheckBoxSkipZeroCHRBlock.Text = "Skip zero CHR\\Block";
			this.CheckBoxSkipZeroCHRBlock.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(22, 255);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 12;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(103, 255);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 13;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// CheckBoxGameMap
			// 
			this.CheckBoxGameMap.Location = new System.Drawing.Point(34, 89);
			this.CheckBoxGameMap.Name = "CheckBoxGameMap";
			this.CheckBoxGameMap.Size = new System.Drawing.Size(98, 19);
			this.CheckBoxGameMap.TabIndex = 3;
			this.CheckBoxGameMap.Text = "Game map";
			this.CheckBoxGameMap.UseVisualStyleBackColor = true;
			this.CheckBoxGameMap.CheckedChanged += new System.EventHandler(this.CheckBoxGameMapChanged);
			// 
			// CheckBoxDeleteEmptyScreens
			// 
			this.CheckBoxDeleteEmptyScreens.Enabled = false;
			this.CheckBoxDeleteEmptyScreens.Location = new System.Drawing.Point(34, 114);
			this.CheckBoxDeleteEmptyScreens.Name = "CheckBoxDeleteEmptyScreens";
			this.CheckBoxDeleteEmptyScreens.Size = new System.Drawing.Size(144, 19);
			this.CheckBoxDeleteEmptyScreens.TabIndex = 4;
			this.CheckBoxDeleteEmptyScreens.Text = "Delete empty screens";
			this.CheckBoxDeleteEmptyScreens.UseVisualStyleBackColor = true;
			this.CheckBoxDeleteEmptyScreens.CheckedChanged += new System.EventHandler(this.CheckBoxGameMapChanged);
			// 
			// CheckBoxApplyPalette
			// 
			this.CheckBoxApplyPalette.Location = new System.Drawing.Point(34, 164);
			this.CheckBoxApplyPalette.Name = "CheckBoxApplyPalette";
			this.CheckBoxApplyPalette.Size = new System.Drawing.Size(118, 19);
			this.CheckBoxApplyPalette.TabIndex = 7;
			this.CheckBoxApplyPalette.Text = "Apply palette (2x2)";
			this.CheckBoxApplyPalette.UseVisualStyleBackColor = true;
			this.CheckBoxApplyPalette.CheckedChanged += new System.EventHandler(this.CheckBoxApplyPaletteChanged);
			// 
			// BtnApplyPaletteDesc
			// 
			this.BtnApplyPaletteDesc.Location = new System.Drawing.Point(158, 163);
			this.BtnApplyPaletteDesc.Name = "BtnApplyPaletteDesc";
			this.BtnApplyPaletteDesc.Size = new System.Drawing.Size(20, 20);
			this.BtnApplyPaletteDesc.TabIndex = 6;
			this.BtnApplyPaletteDesc.Text = "?";
			this.BtnApplyPaletteDesc.UseVisualStyleBackColor = true;
			this.BtnApplyPaletteDesc.Click += new System.EventHandler(this.BtnApplyPaletteDescClick);
			// 
			// CheckBoxCHRs
			// 
			this.CheckBoxCHRs.Checked = true;
			this.CheckBoxCHRs.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxCHRs.Enabled = false;
			this.CheckBoxCHRs.Location = new System.Drawing.Point(34, 14);
			this.CheckBoxCHRs.Name = "CheckBoxCHRs";
			this.CheckBoxCHRs.Size = new System.Drawing.Size(63, 19);
			this.CheckBoxCHRs.TabIndex = 0;
			this.CheckBoxCHRs.Text = "CHRs";
			this.CheckBoxCHRs.UseVisualStyleBackColor = true;
			// 
			// CheckBoxImportPaletteASIS
			// 
			this.CheckBoxImportPaletteASIS.Location = new System.Drawing.Point(34, 189);
			this.CheckBoxImportPaletteASIS.Name = "CheckBoxImportPaletteASIS";
			this.CheckBoxImportPaletteASIS.Size = new System.Drawing.Size(131, 19);
			this.CheckBoxImportPaletteASIS.TabIndex = 9;
			this.CheckBoxImportPaletteASIS.Text = "Import palette \'AS IS\'";
			this.CheckBoxImportPaletteASIS.UseVisualStyleBackColor = true;
			// 
			// BtnImportPaletteASIS
			// 
			this.BtnImportPaletteASIS.Location = new System.Drawing.Point(158, 188);
			this.BtnImportPaletteASIS.Name = "BtnImportPaletteASIS";
			this.BtnImportPaletteASIS.Size = new System.Drawing.Size(20, 20);
			this.BtnImportPaletteASIS.TabIndex = 8;
			this.BtnImportPaletteASIS.Text = "?";
			this.BtnImportPaletteASIS.UseVisualStyleBackColor = true;
			this.BtnImportPaletteASIS.Click += new System.EventHandler(this.BtnImportPaletteASISDescClick);
			// 
			// labelStartPalette
			// 
			this.labelStartPalette.Location = new System.Drawing.Point(95, 216);
			this.labelStartPalette.Name = "labelStartPalette";
			this.labelStartPalette.Size = new System.Drawing.Size(69, 18);
			this.labelStartPalette.TabIndex = 11;
			this.labelStartPalette.Text = "Start Palette";
			// 
			// CBoxStartPalette
			// 
			this.CBoxStartPalette.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxStartPalette.FormattingEnabled = true;
			this.CBoxStartPalette.Location = new System.Drawing.Point(34, 212);
			this.CBoxStartPalette.Name = "CBoxStartPalette";
			this.CBoxStartPalette.Size = new System.Drawing.Size(56, 21);
			this.CBoxStartPalette.TabIndex = 10;
			// 
			// import_tiles_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(202, 291);
			this.Controls.Add(this.BtnApplyPaletteDesc);
			this.Controls.Add(this.labelStartPalette);
			this.Controls.Add(this.CBoxStartPalette);
			this.Controls.Add(this.BtnImportPaletteASIS);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.CheckBoxImportPaletteASIS);
			this.Controls.Add(this.CheckBoxSkipZeroCHRBlock);
			this.Controls.Add(this.CheckBoxApplyPalette);
			this.Controls.Add(this.CheckBoxDeleteEmptyScreens);
			this.Controls.Add(this.CheckBoxGameMap);
			this.Controls.Add(this.CheckBoxCHRs);
			this.Controls.Add(this.CheckBoxTiles);
			this.Controls.Add(this.CheckBoxBlocks);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "import_tiles_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Import Options";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ComboBox CBoxStartPalette;
		private System.Windows.Forms.Label labelStartPalette;
		private System.Windows.Forms.Button BtnImportPaletteASIS;
		private System.Windows.Forms.CheckBox CheckBoxImportPaletteASIS;
		private System.Windows.Forms.CheckBox CheckBoxCHRs;
		private System.Windows.Forms.Button BtnApplyPaletteDesc;
		private System.Windows.Forms.CheckBox CheckBoxApplyPalette;
		private System.Windows.Forms.CheckBox CheckBoxDeleteEmptyScreens;
		private System.Windows.Forms.CheckBox CheckBoxGameMap;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.CheckBox CheckBoxSkipZeroCHRBlock;
		private System.Windows.Forms.CheckBox CheckBoxTiles;
		private System.Windows.Forms.CheckBox CheckBoxBlocks;
	}
}
