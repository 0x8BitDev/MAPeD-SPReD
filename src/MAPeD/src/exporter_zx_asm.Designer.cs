/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 29.08.2018
 * Time: 15:20
 */
namespace MAPeD
{
	partial class exporter_zx_sjasm
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.ExpZXAsmTypeMonochrome = new System.Windows.Forms.RadioButton();
			this.ExpZXAsmTypeColor = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.ExpZXAsmGFXDithering = new System.Windows.Forms.CheckBox();
			this.ExpZXAsmTiles4x4 = new System.Windows.Forms.RadioButton();
			this.ExpZXAsmTiles2x2 = new System.Windows.Forms.RadioButton();
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.ExpZXAsmRenderTilesPNG = new System.Windows.Forms.CheckBox();
			this.ExpZXAsmRenderLevelPNG = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.ExpZXAsmColorConversionModes = new System.Windows.Forms.ComboBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ExpZXAsmTypeMonochrome);
			this.groupBox1.Controls.Add(this.ExpZXAsmTypeColor);
			this.groupBox1.Location = new System.Drawing.Point(5, 36);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(106, 65);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Type";
			// 
			// ExpZXAsmTypeMonochrome
			// 
			this.ExpZXAsmTypeMonochrome.Location = new System.Drawing.Point(9, 40);
			this.ExpZXAsmTypeMonochrome.Name = "ExpZXAsmTypeMonochrome";
			this.ExpZXAsmTypeMonochrome.Size = new System.Drawing.Size(90, 15);
			this.ExpZXAsmTypeMonochrome.TabIndex = 2;
			this.ExpZXAsmTypeMonochrome.TabStop = true;
			this.ExpZXAsmTypeMonochrome.Text = "Monochrome";
			this.ExpZXAsmTypeMonochrome.UseVisualStyleBackColor = true;
			// 
			// ExpZXAsmTypeColor
			// 
			this.ExpZXAsmTypeColor.Checked = true;
			this.ExpZXAsmTypeColor.Location = new System.Drawing.Point(9, 19);
			this.ExpZXAsmTypeColor.Name = "ExpZXAsmTypeColor";
			this.ExpZXAsmTypeColor.Size = new System.Drawing.Size(90, 15);
			this.ExpZXAsmTypeColor.TabIndex = 1;
			this.ExpZXAsmTypeColor.TabStop = true;
			this.ExpZXAsmTypeColor.Text = "Color";
			this.ExpZXAsmTypeColor.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ExpZXAsmGFXDithering);
			this.groupBox2.Controls.Add(this.ExpZXAsmTiles4x4);
			this.groupBox2.Controls.Add(this.ExpZXAsmTiles2x2);
			this.groupBox2.Location = new System.Drawing.Point(117, 36);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(158, 65);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tiles";
			// 
			// ExpZXAsmGFXDithering
			// 
			this.ExpZXAsmGFXDithering.Checked = true;
			this.ExpZXAsmGFXDithering.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ExpZXAsmGFXDithering.Location = new System.Drawing.Point(61, 19);
			this.ExpZXAsmGFXDithering.Name = "ExpZXAsmGFXDithering";
			this.ExpZXAsmGFXDithering.Size = new System.Drawing.Size(83, 19);
			this.ExpZXAsmGFXDithering.TabIndex = 7;
			this.ExpZXAsmGFXDithering.Text = "Dithering";
			this.ExpZXAsmGFXDithering.UseVisualStyleBackColor = true;
			// 
			// ExpZXAsmTiles4x4
			// 
			this.ExpZXAsmTiles4x4.Enabled = false;
			this.ExpZXAsmTiles4x4.Location = new System.Drawing.Point(10, 40);
			this.ExpZXAsmTiles4x4.Name = "ExpZXAsmTiles4x4";
			this.ExpZXAsmTiles4x4.Size = new System.Drawing.Size(42, 15);
			this.ExpZXAsmTiles4x4.TabIndex = 5;
			this.ExpZXAsmTiles4x4.Text = "4x4";
			this.ExpZXAsmTiles4x4.UseVisualStyleBackColor = true;
			// 
			// ExpZXAsmTiles2x2
			// 
			this.ExpZXAsmTiles2x2.Checked = true;
			this.ExpZXAsmTiles2x2.Location = new System.Drawing.Point(10, 19);
			this.ExpZXAsmTiles2x2.Name = "ExpZXAsmTiles2x2";
			this.ExpZXAsmTiles2x2.Size = new System.Drawing.Size(42, 15);
			this.ExpZXAsmTiles2x2.TabIndex = 4;
			this.ExpZXAsmTiles2x2.TabStop = true;
			this.ExpZXAsmTiles2x2.Text = "2x2";
			this.ExpZXAsmTiles2x2.UseVisualStyleBackColor = true;
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(117, 189);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 13;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			this.BtnOk.Click += new System.EventHandler(this.event_ok);
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(198, 189);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 14;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.event_cancel);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.ExpZXAsmRenderTilesPNG);
			this.groupBox3.Controls.Add(this.ExpZXAsmRenderLevelPNG);
			this.groupBox3.Location = new System.Drawing.Point(5, 106);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(106, 67);
			this.groupBox3.TabIndex = 8;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Extra Output";
			// 
			// ExpZXAsmRenderTilesPNG
			// 
			this.ExpZXAsmRenderTilesPNG.Location = new System.Drawing.Point(8, 40);
			this.ExpZXAsmRenderTilesPNG.Name = "ExpZXAsmRenderTilesPNG";
			this.ExpZXAsmRenderTilesPNG.Size = new System.Drawing.Size(86, 24);
			this.ExpZXAsmRenderTilesPNG.TabIndex = 10;
			this.ExpZXAsmRenderTilesPNG.Text = "Tiles .png";
			this.ExpZXAsmRenderTilesPNG.UseVisualStyleBackColor = true;
			// 
			// ExpZXAsmRenderLevelPNG
			// 
			this.ExpZXAsmRenderLevelPNG.Location = new System.Drawing.Point(8, 18);
			this.ExpZXAsmRenderLevelPNG.Name = "ExpZXAsmRenderLevelPNG";
			this.ExpZXAsmRenderLevelPNG.Size = new System.Drawing.Size(86, 24);
			this.ExpZXAsmRenderLevelPNG.TabIndex = 9;
			this.ExpZXAsmRenderLevelPNG.Text = "Level .png";
			this.ExpZXAsmRenderLevelPNG.UseVisualStyleBackColor = true;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.ExpZXAsmColorConversionModes);
			this.groupBox4.Location = new System.Drawing.Point(117, 106);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(158, 67);
			this.groupBox4.TabIndex = 11;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Color Conversion Modes";
			// 
			// ExpZXAsmColorConversionModes
			// 
			this.ExpZXAsmColorConversionModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ExpZXAsmColorConversionModes.FormattingEnabled = true;
			this.ExpZXAsmColorConversionModes.Items.AddRange(new object[] {
									"Mode 1",
									"Mode 2"});
			this.ExpZXAsmColorConversionModes.Location = new System.Drawing.Point(7, 18);
			this.ExpZXAsmColorConversionModes.Name = "ExpZXAsmColorConversionModes";
			this.ExpZXAsmColorConversionModes.Size = new System.Drawing.Size(141, 21);
			this.ExpZXAsmColorConversionModes.TabIndex = 12;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.label1);
			this.groupBox5.Location = new System.Drawing.Point(5, 0);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(270, 30);
			this.groupBox5.TabIndex = 15;
			this.groupBox5.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(253, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "MODE: MULTIDIRECTIONAL SCROLLING";
			// 
			// exporter_zx_sjasm
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(281, 222);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "exporter_zx_sjasm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ZX Asm Export Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.ComboBox ExpZXAsmColorConversionModes;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox ExpZXAsmGFXDithering;
		private System.Windows.Forms.CheckBox ExpZXAsmRenderLevelPNG;
		private System.Windows.Forms.CheckBox ExpZXAsmRenderTilesPNG;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.RadioButton ExpZXAsmTiles2x2;
		private System.Windows.Forms.RadioButton ExpZXAsmTiles4x4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton ExpZXAsmTypeColor;
		private System.Windows.Forms.RadioButton ExpZXAsmTypeMonochrome;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
