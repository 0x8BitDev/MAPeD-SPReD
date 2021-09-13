/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
			this.RBtnTypeMonochrome = new System.Windows.Forms.RadioButton();
			this.RBtnTypeColor = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.CheckBoxRLE = new System.Windows.Forms.CheckBox();
			this.CheckBoxGFXDithering = new System.Windows.Forms.CheckBox();
			this.RBtnTiles4x4 = new System.Windows.Forms.RadioButton();
			this.RBtnTiles1x1 = new System.Windows.Forms.RadioButton();
			this.RBtnTiles2x2 = new System.Windows.Forms.RadioButton();
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.CheckBoxRenderTilesPNG = new System.Windows.Forms.CheckBox();
			this.CheckBoxRenderLevelPNG = new System.Windows.Forms.CheckBox();
			this.groupBoxColorConversion = new System.Windows.Forms.GroupBox();
			this.NumericUpDownInkFactor = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.CBoxColorConversionModes = new System.Windows.Forms.ComboBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.GrpBoxEntCoords = new System.Windows.Forms.GroupBox();
			this.RBtnEntMapCoords = new System.Windows.Forms.RadioButton();
			this.RBtnEntScreenCoords = new System.Windows.Forms.RadioButton();
			this.CheckBoxExportEntities = new System.Windows.Forms.CheckBox();
			this.CheckBoxExportMarks = new System.Windows.Forms.CheckBox();
			this.RichTextBoxExportDesc = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBoxColorConversion.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownInkFactor)).BeginInit();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.GrpBoxEntCoords.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RBtnTypeMonochrome);
			this.groupBox1.Controls.Add(this.RBtnTypeColor);
			this.groupBox1.Location = new System.Drawing.Point(5, 36);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(106, 65);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Type";
			// 
			// RBtnTypeMonochrome
			// 
			this.RBtnTypeMonochrome.Location = new System.Drawing.Point(9, 40);
			this.RBtnTypeMonochrome.Name = "RBtnTypeMonochrome";
			this.RBtnTypeMonochrome.Size = new System.Drawing.Size(90, 15);
			this.RBtnTypeMonochrome.TabIndex = 2;
			this.RBtnTypeMonochrome.TabStop = true;
			this.RBtnTypeMonochrome.Text = "Monochrome";
			this.RBtnTypeMonochrome.UseVisualStyleBackColor = true;
			// 
			// RBtnTypeColor
			// 
			this.RBtnTypeColor.Checked = true;
			this.RBtnTypeColor.Location = new System.Drawing.Point(9, 19);
			this.RBtnTypeColor.Name = "RBtnTypeColor";
			this.RBtnTypeColor.Size = new System.Drawing.Size(90, 15);
			this.RBtnTypeColor.TabIndex = 1;
			this.RBtnTypeColor.TabStop = true;
			this.RBtnTypeColor.Text = "Color";
			this.RBtnTypeColor.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.CheckBoxRLE);
			this.groupBox2.Controls.Add(this.CheckBoxGFXDithering);
			this.groupBox2.Controls.Add(this.RBtnTiles4x4);
			this.groupBox2.Controls.Add(this.RBtnTiles1x1);
			this.groupBox2.Controls.Add(this.RBtnTiles2x2);
			this.groupBox2.Location = new System.Drawing.Point(117, 36);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(158, 65);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tiles";
			// 
			// CheckBoxRLE
			// 
			this.CheckBoxRLE.Location = new System.Drawing.Point(104, 40);
			this.CheckBoxRLE.Name = "CheckBoxRLE";
			this.CheckBoxRLE.Size = new System.Drawing.Size(48, 19);
			this.CheckBoxRLE.TabIndex = 6;
			this.CheckBoxRLE.Text = "RLE";
			this.CheckBoxRLE.UseVisualStyleBackColor = true;
			// 
			// CheckBoxGFXDithering
			// 
			this.CheckBoxGFXDithering.Checked = true;
			this.CheckBoxGFXDithering.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxGFXDithering.Location = new System.Drawing.Point(36, 40);
			this.CheckBoxGFXDithering.Name = "CheckBoxGFXDithering";
			this.CheckBoxGFXDithering.Size = new System.Drawing.Size(76, 19);
			this.CheckBoxGFXDithering.TabIndex = 6;
			this.CheckBoxGFXDithering.Text = "Dithering";
			this.CheckBoxGFXDithering.UseVisualStyleBackColor = true;
			// 
			// RBtnTiles4x4
			// 
			this.RBtnTiles4x4.Enabled = false;
			this.RBtnTiles4x4.Location = new System.Drawing.Point(104, 20);
			this.RBtnTiles4x4.Name = "RBtnTiles4x4";
			this.RBtnTiles4x4.Size = new System.Drawing.Size(42, 15);
			this.RBtnTiles4x4.TabIndex = 5;
			this.RBtnTiles4x4.Text = "4x4";
			this.RBtnTiles4x4.UseVisualStyleBackColor = true;
			this.RBtnTiles4x4.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTiles1x1
			// 
			this.RBtnTiles1x1.Location = new System.Drawing.Point(8, 19);
			this.RBtnTiles1x1.Name = "RBtnTiles1x1";
			this.RBtnTiles1x1.Size = new System.Drawing.Size(42, 15);
			this.RBtnTiles1x1.TabIndex = 4;
			this.RBtnTiles1x1.Text = "1x1";
			this.RBtnTiles1x1.UseVisualStyleBackColor = true;
			this.RBtnTiles1x1.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTiles2x2
			// 
			this.RBtnTiles2x2.Checked = true;
			this.RBtnTiles2x2.Location = new System.Drawing.Point(56, 20);
			this.RBtnTiles2x2.Name = "RBtnTiles2x2";
			this.RBtnTiles2x2.Size = new System.Drawing.Size(42, 15);
			this.RBtnTiles2x2.TabIndex = 4;
			this.RBtnTiles2x2.TabStop = true;
			this.RBtnTiles2x2.Text = "2x2";
			this.RBtnTiles2x2.UseVisualStyleBackColor = true;
			this.RBtnTiles2x2.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(363, 264);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 21;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			this.BtnOk.Click += new System.EventHandler(this.event_ok);
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(444, 264);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 22;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.event_cancel);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.CheckBoxRenderTilesPNG);
			this.groupBox3.Controls.Add(this.CheckBoxRenderLevelPNG);
			this.groupBox3.Location = new System.Drawing.Point(5, 106);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(106, 67);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Extra Output";
			// 
			// CheckBoxRenderTilesPNG
			// 
			this.CheckBoxRenderTilesPNG.Location = new System.Drawing.Point(8, 40);
			this.CheckBoxRenderTilesPNG.Name = "CheckBoxRenderTilesPNG";
			this.CheckBoxRenderTilesPNG.Size = new System.Drawing.Size(86, 24);
			this.CheckBoxRenderTilesPNG.TabIndex = 9;
			this.CheckBoxRenderTilesPNG.Text = "Tiles .png";
			this.CheckBoxRenderTilesPNG.UseVisualStyleBackColor = true;
			// 
			// CheckBoxRenderLevelPNG
			// 
			this.CheckBoxRenderLevelPNG.Location = new System.Drawing.Point(8, 18);
			this.CheckBoxRenderLevelPNG.Name = "CheckBoxRenderLevelPNG";
			this.CheckBoxRenderLevelPNG.Size = new System.Drawing.Size(86, 24);
			this.CheckBoxRenderLevelPNG.TabIndex = 8;
			this.CheckBoxRenderLevelPNG.Text = "Level .png";
			this.CheckBoxRenderLevelPNG.UseVisualStyleBackColor = true;
			// 
			// groupBoxColorConversion
			// 
			this.groupBoxColorConversion.Controls.Add(this.NumericUpDownInkFactor);
			this.groupBoxColorConversion.Controls.Add(this.label2);
			this.groupBoxColorConversion.Controls.Add(this.CBoxColorConversionModes);
			this.groupBoxColorConversion.Location = new System.Drawing.Point(117, 106);
			this.groupBoxColorConversion.Name = "groupBoxColorConversion";
			this.groupBoxColorConversion.Size = new System.Drawing.Size(158, 67);
			this.groupBoxColorConversion.TabIndex = 10;
			this.groupBoxColorConversion.TabStop = false;
			this.groupBoxColorConversion.Text = "Color Conversion";
			// 
			// NumericUpDownInkFactor
			// 
			this.NumericUpDownInkFactor.Location = new System.Drawing.Point(82, 42);
			this.NumericUpDownInkFactor.Maximum = new decimal(new int[] {
									10,
									0,
									0,
									0});
			this.NumericUpDownInkFactor.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumericUpDownInkFactor.Name = "NumericUpDownInkFactor";
			this.NumericUpDownInkFactor.Size = new System.Drawing.Size(48, 20);
			this.NumericUpDownInkFactor.TabIndex = 13;
			this.NumericUpDownInkFactor.Value = new decimal(new int[] {
									3,
									0,
									0,
									0});
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(19, 45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 17);
			this.label2.TabIndex = 12;
			this.label2.Text = "Ink Factor:";
			// 
			// CBoxColorConversionModes
			// 
			this.CBoxColorConversionModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxColorConversionModes.FormattingEnabled = true;
			this.CBoxColorConversionModes.Items.AddRange(new object[] {
									"Mode 1",
									"Mode 2"});
			this.CBoxColorConversionModes.Location = new System.Drawing.Point(19, 18);
			this.CBoxColorConversionModes.Name = "CBoxColorConversionModes";
			this.CBoxColorConversionModes.Size = new System.Drawing.Size(111, 21);
			this.CBoxColorConversionModes.TabIndex = 11;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.label1);
			this.groupBox5.Location = new System.Drawing.Point(5, 0);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(270, 30);
			this.groupBox5.TabIndex = 0;
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
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.GrpBoxEntCoords);
			this.groupBox6.Controls.Add(this.CheckBoxExportEntities);
			this.groupBox6.Controls.Add(this.CheckBoxExportMarks);
			this.groupBox6.Location = new System.Drawing.Point(5, 179);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(270, 72);
			this.groupBox6.TabIndex = 14;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Marks / Entities";
			// 
			// GrpBoxEntCoords
			// 
			this.GrpBoxEntCoords.Controls.Add(this.RBtnEntMapCoords);
			this.GrpBoxEntCoords.Controls.Add(this.RBtnEntScreenCoords);
			this.GrpBoxEntCoords.Location = new System.Drawing.Point(112, 14);
			this.GrpBoxEntCoords.Name = "GrpBoxEntCoords";
			this.GrpBoxEntCoords.Size = new System.Drawing.Size(148, 49);
			this.GrpBoxEntCoords.TabIndex = 17;
			this.GrpBoxEntCoords.TabStop = false;
			this.GrpBoxEntCoords.Text = "Coordinates";
			// 
			// RBtnEntMapCoords
			// 
			this.RBtnEntMapCoords.Location = new System.Drawing.Point(85, 19);
			this.RBtnEntMapCoords.Name = "RBtnEntMapCoords";
			this.RBtnEntMapCoords.Size = new System.Drawing.Size(54, 19);
			this.RBtnEntMapCoords.TabIndex = 19;
			this.RBtnEntMapCoords.Text = "Map";
			this.RBtnEntMapCoords.UseVisualStyleBackColor = true;
			this.RBtnEntMapCoords.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnEntScreenCoords
			// 
			this.RBtnEntScreenCoords.Checked = true;
			this.RBtnEntScreenCoords.Location = new System.Drawing.Point(19, 19);
			this.RBtnEntScreenCoords.Name = "RBtnEntScreenCoords";
			this.RBtnEntScreenCoords.Size = new System.Drawing.Size(65, 19);
			this.RBtnEntScreenCoords.TabIndex = 18;
			this.RBtnEntScreenCoords.TabStop = true;
			this.RBtnEntScreenCoords.Text = "Screen";
			this.RBtnEntScreenCoords.UseVisualStyleBackColor = true;
			this.RBtnEntScreenCoords.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// CheckBoxExportEntities
			// 
			this.CheckBoxExportEntities.Checked = true;
			this.CheckBoxExportEntities.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxExportEntities.Location = new System.Drawing.Point(7, 44);
			this.CheckBoxExportEntities.Name = "CheckBoxExportEntities";
			this.CheckBoxExportEntities.Size = new System.Drawing.Size(68, 19);
			this.CheckBoxExportEntities.TabIndex = 16;
			this.CheckBoxExportEntities.Text = "Entities";
			this.CheckBoxExportEntities.UseVisualStyleBackColor = true;
			this.CheckBoxExportEntities.CheckedChanged += new System.EventHandler(this.ExpEntitiesChanged_Event);
			// 
			// CheckBoxExportMarks
			// 
			this.CheckBoxExportMarks.Checked = true;
			this.CheckBoxExportMarks.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxExportMarks.Location = new System.Drawing.Point(7, 19);
			this.CheckBoxExportMarks.Name = "CheckBoxExportMarks";
			this.CheckBoxExportMarks.Size = new System.Drawing.Size(68, 19);
			this.CheckBoxExportMarks.TabIndex = 15;
			this.CheckBoxExportMarks.Text = "Marks";
			this.CheckBoxExportMarks.UseVisualStyleBackColor = true;
			this.CheckBoxExportMarks.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RichTextBoxExportDesc
			// 
			this.RichTextBoxExportDesc.Location = new System.Drawing.Point(281, 9);
			this.RichTextBoxExportDesc.MaxLength = 2048;
			this.RichTextBoxExportDesc.Name = "RichTextBoxExportDesc";
			this.RichTextBoxExportDesc.ReadOnly = true;
			this.RichTextBoxExportDesc.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.RichTextBoxExportDesc.Size = new System.Drawing.Size(238, 242);
			this.RichTextBoxExportDesc.TabIndex = 20;
			this.RichTextBoxExportDesc.Text = "";
			// 
			// exporter_zx_sjasm
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(527, 299);
			this.Controls.Add(this.RichTextBoxExportDesc);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBoxColorConversion);
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
			this.Text = "ZX SjASMPlus Export Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBoxColorConversion.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownInkFactor)).EndInit();
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.GrpBoxEntCoords.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CheckBoxRLE;
		private System.Windows.Forms.RadioButton RBtnTiles1x1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown NumericUpDownInkFactor;
		private System.Windows.Forms.RichTextBox RichTextBoxExportDesc;
		private System.Windows.Forms.CheckBox CheckBoxExportEntities;
		private System.Windows.Forms.RadioButton RBtnEntScreenCoords;
		private System.Windows.Forms.RadioButton RBtnEntMapCoords;
		private System.Windows.Forms.GroupBox GrpBoxEntCoords;
		private System.Windows.Forms.CheckBox CheckBoxExportMarks;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.ComboBox CBoxColorConversionModes;
		private System.Windows.Forms.GroupBox groupBoxColorConversion;
		private System.Windows.Forms.CheckBox CheckBoxGFXDithering;
		private System.Windows.Forms.CheckBox CheckBoxRenderLevelPNG;
		private System.Windows.Forms.CheckBox CheckBoxRenderTilesPNG;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button BtnCancel;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.RadioButton RBtnTiles2x2;
		private System.Windows.Forms.RadioButton RBtnTiles4x4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton RBtnTypeColor;
		private System.Windows.Forms.RadioButton RBtnTypeMonochrome;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
