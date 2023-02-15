/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2022
 * Time: 13:20
 */
namespace SPReD
{
	partial class image_import_options_form
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
			this.CheckBoxCropByAlpha = new System.Windows.Forms.CheckBox();
			this.CheckBoxApplyPalette = new System.Windows.Forms.CheckBox();
			this.CBoxPaletteSlot = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.BtnHelp = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(10, 118);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 5;
			this.BtnOk.Text = "&Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(91, 118);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 6;
			this.BtnCancel.Text = "&Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			// 
			// CheckBoxCropByAlpha
			// 
			this.CheckBoxCropByAlpha.Location = new System.Drawing.Point(34, 42);
			this.CheckBoxCropByAlpha.Name = "CheckBoxCropByAlpha";
			this.CheckBoxCropByAlpha.Size = new System.Drawing.Size(104, 24);
			this.CheckBoxCropByAlpha.TabIndex = 3;
			this.CheckBoxCropByAlpha.Text = "Crop by alpha";
			this.CheckBoxCropByAlpha.UseVisualStyleBackColor = true;
			// 
			// CheckBoxApplyPalette
			// 
			this.CheckBoxApplyPalette.Checked = true;
			this.CheckBoxApplyPalette.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxApplyPalette.Location = new System.Drawing.Point(34, 17);
			this.CheckBoxApplyPalette.Name = "CheckBoxApplyPalette";
			this.CheckBoxApplyPalette.Size = new System.Drawing.Size(104, 24);
			this.CheckBoxApplyPalette.TabIndex = 1;
			this.CheckBoxApplyPalette.Text = "Apply palette";
			this.CheckBoxApplyPalette.UseVisualStyleBackColor = true;
			// 
			// CBoxPaletteSlot
			// 
			this.CBoxPaletteSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxPaletteSlot.FormattingEnabled = true;
			this.CBoxPaletteSlot.Location = new System.Drawing.Point(34, 73);
			this.CBoxPaletteSlot.Name = "CBoxPaletteSlot";
			this.CBoxPaletteSlot.Size = new System.Drawing.Size(48, 21);
			this.CBoxPaletteSlot.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(88, 77);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "Palette slot";
			// 
			// BtnHelp
			// 
			this.BtnHelp.Location = new System.Drawing.Point(135, 17);
			this.BtnHelp.Name = "BtnHelp";
			this.BtnHelp.Size = new System.Drawing.Size(20, 20);
			this.BtnHelp.TabIndex = 2;
			this.BtnHelp.Text = "?";
			this.BtnHelp.UseVisualStyleBackColor = true;
			this.BtnHelp.Click += new System.EventHandler(this.BtnHelpClick);
			// 
			// image_import_options_form
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(177, 156);
			this.Controls.Add(this.CBoxPaletteSlot);
			this.Controls.Add(this.BtnHelp);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CheckBoxApplyPalette);
			this.Controls.Add(this.CheckBoxCropByAlpha);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "image_import_options_form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Import Options";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button BtnHelp;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox CBoxPaletteSlot;
		private System.Windows.Forms.CheckBox CheckBoxApplyPalette;
		private System.Windows.Forms.CheckBox CheckBoxCropByAlpha;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
	}
}
