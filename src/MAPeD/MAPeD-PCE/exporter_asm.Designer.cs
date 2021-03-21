/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 13.09.2018
 * Time: 17:59
 */
namespace MAPeD
{
	partial class exporter_asm
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
#if DEF_PCE
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.BtnCancel = new System.Windows.Forms.Button();
			this.BtnOk = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.RBtnTilesDirRows = new System.Windows.Forms.RadioButton();
			this.RBtnTilesDirColumns = new System.Windows.Forms.RadioButton();
			this.CheckBoxRLE = new System.Windows.Forms.CheckBox();
			this.RBtnTiles4x4 = new System.Windows.Forms.RadioButton();
			this.RBtnTiles2x2 = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.CheckBoxGenerateHFile = new System.Windows.Forms.CheckBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.NumericUpDownCHROffset = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.ComboBoxBAT = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.RBtnPropPerCHR = new System.Windows.Forms.RadioButton();
			this.RBtnPropPerBlock = new System.Windows.Forms.RadioButton();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBoxEntityCoordinates = new System.Windows.Forms.GroupBox();
			this.RBtnEntityCoordMap = new System.Windows.Forms.RadioButton();
			this.RBtnEntityCoordScreen = new System.Windows.Forms.RadioButton();
			this.CheckBoxExportEntities = new System.Windows.Forms.CheckBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.RBtnModeMultidirScroll = new System.Windows.Forms.RadioButton();
			this.RBtnModeBidirScroll = new System.Windows.Forms.RadioButton();
			this.RBtnModeStaticScreen = new System.Windows.Forms.RadioButton();
			this.groupBoxLayout = new System.Windows.Forms.GroupBox();
			this.RBtnLayoutMatrix = new System.Windows.Forms.RadioButton();
			this.RBtnLayoutAdjacentScreenIndices = new System.Windows.Forms.RadioButton();
			this.RBtnLayoutAdjacentScreens = new System.Windows.Forms.RadioButton();
			this.CheckBoxExportMarks = new System.Windows.Forms.CheckBox();
			this.RichTextBoxExportDesc = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownCHROffset)).BeginInit();
			this.groupBox7.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBoxEntityCoordinates.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBoxLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// BtnCancel
			// 
			this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.BtnCancel.Location = new System.Drawing.Point(516, 302);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 37;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.event_cancel);
			// 
			// BtnOk
			// 
			this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.BtnOk.Location = new System.Drawing.Point(435, 302);
			this.BtnOk.Name = "BtnOk";
			this.BtnOk.Size = new System.Drawing.Size(75, 23);
			this.BtnOk.TabIndex = 36;
			this.BtnOk.Text = "Ok";
			this.BtnOk.UseVisualStyleBackColor = true;
			this.BtnOk.Click += new System.EventHandler(this.event_ok);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.CheckBoxRLE);
			this.groupBox1.Controls.Add(this.RBtnTiles4x4);
			this.groupBox1.Controls.Add(this.RBtnTiles2x2);
			this.groupBox1.Location = new System.Drawing.Point(7, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(152, 86);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tiles";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.RBtnTilesDirRows);
			this.groupBox3.Controls.Add(this.RBtnTilesDirColumns);
			this.groupBox3.Location = new System.Drawing.Point(7, 39);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(138, 41);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Data Order";
			// 
			// RBtnTilesDirRows
			// 
			this.RBtnTilesDirRows.Location = new System.Drawing.Point(77, 17);
			this.RBtnTilesDirRows.Name = "RBtnTilesDirRows";
			this.RBtnTilesDirRows.Size = new System.Drawing.Size(53, 17);
			this.RBtnTilesDirRows.TabIndex = 7;
			this.RBtnTilesDirRows.Text = "Rows";
			this.RBtnTilesDirRows.UseVisualStyleBackColor = true;
			this.RBtnTilesDirRows.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTilesDirColumns
			// 
			this.RBtnTilesDirColumns.Checked = true;
			this.RBtnTilesDirColumns.Location = new System.Drawing.Point(13, 17);
			this.RBtnTilesDirColumns.Name = "RBtnTilesDirColumns";
			this.RBtnTilesDirColumns.Size = new System.Drawing.Size(68, 17);
			this.RBtnTilesDirColumns.TabIndex = 6;
			this.RBtnTilesDirColumns.TabStop = true;
			this.RBtnTilesDirColumns.Text = "Columns";
			this.RBtnTilesDirColumns.UseVisualStyleBackColor = true;
			this.RBtnTilesDirColumns.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// CheckBoxRLE
			// 
			this.CheckBoxRLE.Location = new System.Drawing.Point(97, 16);
			this.CheckBoxRLE.Name = "CheckBoxRLE";
			this.CheckBoxRLE.Size = new System.Drawing.Size(50, 18);
			this.CheckBoxRLE.TabIndex = 4;
			this.CheckBoxRLE.Text = "RLE";
			this.CheckBoxRLE.UseVisualStyleBackColor = true;
			this.CheckBoxRLE.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTiles4x4
			// 
			this.RBtnTiles4x4.Checked = true;
			this.RBtnTiles4x4.Location = new System.Drawing.Point(55, 16);
			this.RBtnTiles4x4.Name = "RBtnTiles4x4";
			this.RBtnTiles4x4.Size = new System.Drawing.Size(50, 17);
			this.RBtnTiles4x4.TabIndex = 3;
			this.RBtnTiles4x4.TabStop = true;
			this.RBtnTiles4x4.Text = "4x4";
			this.RBtnTiles4x4.UseVisualStyleBackColor = true;
			this.RBtnTiles4x4.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTiles2x2
			// 
			this.RBtnTiles2x2.Location = new System.Drawing.Point(14, 16);
			this.RBtnTiles2x2.Name = "RBtnTiles2x2";
			this.RBtnTiles2x2.Size = new System.Drawing.Size(50, 17);
			this.RBtnTiles2x2.TabIndex = 2;
			this.RBtnTiles2x2.Text = "2x2";
			this.RBtnTiles2x2.UseVisualStyleBackColor = true;
			this.RBtnTiles2x2.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.CheckBoxGenerateHFile);
			this.groupBox2.Controls.Add(this.groupBox6);
			this.groupBox2.Controls.Add(this.groupBox7);
			this.groupBox2.Controls.Add(this.groupBox5);
			this.groupBox2.Controls.Add(this.groupBox4);
			this.groupBox2.Controls.Add(this.groupBoxLayout);
			this.groupBox2.Controls.Add(this.groupBox1);
			this.groupBox2.Location = new System.Drawing.Point(5, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(305, 284);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			// 
			// CheckBoxGenerateHFile
			// 
			this.CheckBoxGenerateHFile.Location = new System.Drawing.Point(14, 255);
			this.CheckBoxGenerateHFile.Name = "CheckBoxGenerateHFile";
			this.CheckBoxGenerateHFile.Size = new System.Drawing.Size(125, 17);
			this.CheckBoxGenerateHFile.TabIndex = 25;
			this.CheckBoxGenerateHFile.Text = "Generate *.h file";
			this.CheckBoxGenerateHFile.UseVisualStyleBackColor = true;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.NumericUpDownCHROffset);
			this.groupBox6.Controls.Add(this.label2);
			this.groupBox6.Controls.Add(this.ComboBoxBAT);
			this.groupBox6.Controls.Add(this.label1);
			this.groupBox6.Location = new System.Drawing.Point(145, 196);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(152, 76);
			this.groupBox6.TabIndex = 26;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Exrtra";
			// 
			// NumericUpDownCHROffset
			// 
			this.NumericUpDownCHROffset.Location = new System.Drawing.Point(83, 18);
			this.NumericUpDownCHROffset.Maximum = new decimal(new int[] {
									2048,
									0,
									0,
									0});
			this.NumericUpDownCHROffset.Name = "NumericUpDownCHROffset";
			this.NumericUpDownCHROffset.Size = new System.Drawing.Size(57, 20);
			this.NumericUpDownCHROffset.TabIndex = 28;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 17);
			this.label2.TabIndex = 27;
			this.label2.Text = "CHR Offset:";
			// 
			// ComboBoxBAT
			// 
			this.ComboBoxBAT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ComboBoxBAT.FormattingEnabled = true;
			this.ComboBoxBAT.Items.AddRange(new object[] {
									"32x32",
									"64x32",
									"128x32",
									"32x64",
									"64x64",
									"128x64"});
			this.ComboBoxBAT.Location = new System.Drawing.Point(60, 43);
			this.ComboBoxBAT.Name = "ComboBoxBAT";
			this.ComboBoxBAT.Size = new System.Drawing.Size(80, 21);
			this.ComboBoxBAT.TabIndex = 30;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 17);
			this.label1.TabIndex = 29;
			this.label1.Text = "BAT:";
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.RBtnPropPerCHR);
			this.groupBox7.Controls.Add(this.RBtnPropPerBlock);
			this.groupBox7.Location = new System.Drawing.Point(7, 196);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(132, 45);
			this.groupBox7.TabIndex = 22;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Property Id per";
			// 
			// RBtnPropPerCHR
			// 
			this.RBtnPropPerCHR.Location = new System.Drawing.Point(67, 16);
			this.RBtnPropPerCHR.Name = "RBtnPropPerCHR";
			this.RBtnPropPerCHR.Size = new System.Drawing.Size(54, 17);
			this.RBtnPropPerCHR.TabIndex = 24;
			this.RBtnPropPerCHR.Text = "CHR";
			this.RBtnPropPerCHR.UseVisualStyleBackColor = true;
			this.RBtnPropPerCHR.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnPropPerBlock
			// 
			this.RBtnPropPerBlock.Checked = true;
			this.RBtnPropPerBlock.Location = new System.Drawing.Point(14, 16);
			this.RBtnPropPerBlock.Name = "RBtnPropPerBlock";
			this.RBtnPropPerBlock.Size = new System.Drawing.Size(61, 17);
			this.RBtnPropPerBlock.TabIndex = 23;
			this.RBtnPropPerBlock.TabStop = true;
			this.RBtnPropPerBlock.Text = "Block";
			this.RBtnPropPerBlock.UseVisualStyleBackColor = true;
			this.RBtnPropPerBlock.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.groupBoxEntityCoordinates);
			this.groupBox5.Controls.Add(this.CheckBoxExportEntities);
			this.groupBox5.Location = new System.Drawing.Point(165, 115);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.groupBox5.Size = new System.Drawing.Size(132, 77);
			this.groupBox5.TabIndex = 17;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Entities";
			// 
			// groupBoxEntityCoordinates
			// 
			this.groupBoxEntityCoordinates.Controls.Add(this.RBtnEntityCoordMap);
			this.groupBoxEntityCoordinates.Controls.Add(this.RBtnEntityCoordScreen);
			this.groupBoxEntityCoordinates.Location = new System.Drawing.Point(7, 33);
			this.groupBoxEntityCoordinates.Name = "groupBoxEntityCoordinates";
			this.groupBoxEntityCoordinates.Size = new System.Drawing.Size(118, 38);
			this.groupBoxEntityCoordinates.TabIndex = 19;
			this.groupBoxEntityCoordinates.TabStop = false;
			this.groupBoxEntityCoordinates.Text = "Coordinates";
			// 
			// RBtnEntityCoordMap
			// 
			this.RBtnEntityCoordMap.Location = new System.Drawing.Point(68, 13);
			this.RBtnEntityCoordMap.Name = "RBtnEntityCoordMap";
			this.RBtnEntityCoordMap.Size = new System.Drawing.Size(47, 20);
			this.RBtnEntityCoordMap.TabIndex = 21;
			this.RBtnEntityCoordMap.Text = "Map";
			this.RBtnEntityCoordMap.UseVisualStyleBackColor = true;
			this.RBtnEntityCoordMap.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnEntityCoordScreen
			// 
			this.RBtnEntityCoordScreen.Checked = true;
			this.RBtnEntityCoordScreen.Location = new System.Drawing.Point(9, 13);
			this.RBtnEntityCoordScreen.Name = "RBtnEntityCoordScreen";
			this.RBtnEntityCoordScreen.Size = new System.Drawing.Size(60, 20);
			this.RBtnEntityCoordScreen.TabIndex = 20;
			this.RBtnEntityCoordScreen.TabStop = true;
			this.RBtnEntityCoordScreen.Text = "Screen";
			this.RBtnEntityCoordScreen.UseVisualStyleBackColor = true;
			this.RBtnEntityCoordScreen.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// CheckBoxExportEntities
			// 
			this.CheckBoxExportEntities.Checked = true;
			this.CheckBoxExportEntities.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxExportEntities.Location = new System.Drawing.Point(14, 15);
			this.CheckBoxExportEntities.Name = "CheckBoxExportEntities";
			this.CheckBoxExportEntities.Size = new System.Drawing.Size(92, 18);
			this.CheckBoxExportEntities.TabIndex = 18;
			this.CheckBoxExportEntities.Text = "Export";
			this.CheckBoxExportEntities.UseVisualStyleBackColor = true;
			this.CheckBoxExportEntities.CheckedChanged += new System.EventHandler(this.CheckBoxExportEntitiesChanged_Event);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.RBtnModeMultidirScroll);
			this.groupBox4.Controls.Add(this.RBtnModeBidirScroll);
			this.groupBox4.Controls.Add(this.RBtnModeStaticScreen);
			this.groupBox4.Location = new System.Drawing.Point(7, 104);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(152, 88);
			this.groupBox4.TabIndex = 13;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Mode";
			// 
			// RBtnModeMultidirScroll
			// 
			this.RBtnModeMultidirScroll.Checked = true;
			this.RBtnModeMultidirScroll.Location = new System.Drawing.Point(14, 16);
			this.RBtnModeMultidirScroll.Name = "RBtnModeMultidirScroll";
			this.RBtnModeMultidirScroll.Size = new System.Drawing.Size(107, 17);
			this.RBtnModeMultidirScroll.TabIndex = 14;
			this.RBtnModeMultidirScroll.TabStop = true;
			this.RBtnModeMultidirScroll.Text = "Multidir scrolling";
			this.RBtnModeMultidirScroll.UseVisualStyleBackColor = true;
			this.RBtnModeMultidirScroll.CheckedChanged += new System.EventHandler(this.RBtnModeMultidirScrollChanged_Event);
			// 
			// RBtnModeBidirScroll
			// 
			this.RBtnModeBidirScroll.Location = new System.Drawing.Point(14, 35);
			this.RBtnModeBidirScroll.Name = "RBtnModeBidirScroll";
			this.RBtnModeBidirScroll.Size = new System.Drawing.Size(107, 17);
			this.RBtnModeBidirScroll.TabIndex = 15;
			this.RBtnModeBidirScroll.Text = "Bidir scrolling";
			this.RBtnModeBidirScroll.UseVisualStyleBackColor = true;
			this.RBtnModeBidirScroll.CheckedChanged += new System.EventHandler(this.RBtnModeScreenToScreenChanged_Event);
			// 
			// RBtnModeStaticScreen
			// 
			this.RBtnModeStaticScreen.Location = new System.Drawing.Point(14, 54);
			this.RBtnModeStaticScreen.Name = "RBtnModeStaticScreen";
			this.RBtnModeStaticScreen.Size = new System.Drawing.Size(107, 17);
			this.RBtnModeStaticScreen.TabIndex = 16;
			this.RBtnModeStaticScreen.Text = "Static screens";
			this.RBtnModeStaticScreen.UseVisualStyleBackColor = true;
			this.RBtnModeStaticScreen.CheckedChanged += new System.EventHandler(this.RBtnModeStaticScreensChanged_Event);
			// 
			// groupBoxLayout
			// 
			this.groupBoxLayout.Controls.Add(this.RBtnLayoutMatrix);
			this.groupBoxLayout.Controls.Add(this.RBtnLayoutAdjacentScreenIndices);
			this.groupBoxLayout.Controls.Add(this.RBtnLayoutAdjacentScreens);
			this.groupBoxLayout.Controls.Add(this.CheckBoxExportMarks);
			this.groupBoxLayout.Location = new System.Drawing.Point(165, 14);
			this.groupBoxLayout.Name = "groupBoxLayout";
			this.groupBoxLayout.Size = new System.Drawing.Size(132, 97);
			this.groupBoxLayout.TabIndex = 8;
			this.groupBoxLayout.TabStop = false;
			this.groupBoxLayout.Text = "Layout";
			// 
			// RBtnLayoutMatrix
			// 
			this.RBtnLayoutMatrix.Checked = true;
			this.RBtnLayoutMatrix.Location = new System.Drawing.Point(14, 54);
			this.RBtnLayoutMatrix.Name = "RBtnLayoutMatrix";
			this.RBtnLayoutMatrix.Size = new System.Drawing.Size(100, 17);
			this.RBtnLayoutMatrix.TabIndex = 11;
			this.RBtnLayoutMatrix.TabStop = true;
			this.RBtnLayoutMatrix.Text = "Layout matrix";
			this.RBtnLayoutMatrix.UseVisualStyleBackColor = true;
			this.RBtnLayoutMatrix.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnLayoutAdjacentScreenIndices
			// 
			this.RBtnLayoutAdjacentScreenIndices.Location = new System.Drawing.Point(14, 35);
			this.RBtnLayoutAdjacentScreenIndices.Name = "RBtnLayoutAdjacentScreenIndices";
			this.RBtnLayoutAdjacentScreenIndices.Size = new System.Drawing.Size(110, 17);
			this.RBtnLayoutAdjacentScreenIndices.TabIndex = 10;
			this.RBtnLayoutAdjacentScreenIndices.Text = "Adjacent scr inds";
			this.RBtnLayoutAdjacentScreenIndices.UseVisualStyleBackColor = true;
			this.RBtnLayoutAdjacentScreenIndices.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnLayoutAdjacentScreens
			// 
			this.RBtnLayoutAdjacentScreens.Location = new System.Drawing.Point(14, 16);
			this.RBtnLayoutAdjacentScreens.Name = "RBtnLayoutAdjacentScreens";
			this.RBtnLayoutAdjacentScreens.Size = new System.Drawing.Size(110, 17);
			this.RBtnLayoutAdjacentScreens.TabIndex = 9;
			this.RBtnLayoutAdjacentScreens.Text = "Adjacent screens";
			this.RBtnLayoutAdjacentScreens.UseVisualStyleBackColor = true;
			this.RBtnLayoutAdjacentScreens.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// CheckBoxExportMarks
			// 
			this.CheckBoxExportMarks.Checked = true;
			this.CheckBoxExportMarks.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxExportMarks.Location = new System.Drawing.Point(14, 75);
			this.CheckBoxExportMarks.Name = "CheckBoxExportMarks";
			this.CheckBoxExportMarks.Size = new System.Drawing.Size(92, 18);
			this.CheckBoxExportMarks.TabIndex = 12;
			this.CheckBoxExportMarks.Text = "Export marks";
			this.CheckBoxExportMarks.UseVisualStyleBackColor = true;
			this.CheckBoxExportMarks.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RichTextBoxExportDesc
			// 
			this.RichTextBoxExportDesc.Location = new System.Drawing.Point(316, 10);
			this.RichTextBoxExportDesc.Name = "RichTextBoxExportDesc";
			this.RichTextBoxExportDesc.ReadOnly = true;
			this.RichTextBoxExportDesc.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.RichTextBoxExportDesc.Size = new System.Drawing.Size(275, 277);
			this.RichTextBoxExportDesc.TabIndex = 35;
			this.RichTextBoxExportDesc.Text = "";
			// 
			// exporter_pce_asm
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(597, 337);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.RichTextBoxExportDesc);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "exporter_pce_asm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "CA65\\PCEAS Export Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownCHROffset)).EndInit();
			this.groupBox7.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBoxEntityCoordinates.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBoxLayout.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox CheckBoxGenerateHFile;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown NumericUpDownCHROffset;
		private System.Windows.Forms.ComboBox ComboBoxBAT;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton RBtnPropPerBlock;
		private System.Windows.Forms.RadioButton RBtnPropPerCHR;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.RadioButton RBtnLayoutAdjacentScreenIndices;
		private System.Windows.Forms.RadioButton RBtnTilesDirColumns;
		private System.Windows.Forms.RadioButton RBtnTilesDirRows;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox CheckBoxExportMarks;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton RBtnLayoutAdjacentScreens;
		private System.Windows.Forms.RadioButton RBtnLayoutMatrix;
		private System.Windows.Forms.GroupBox groupBoxLayout;
		private System.Windows.Forms.RadioButton RBtnModeStaticScreen;
		private System.Windows.Forms.RichTextBox RichTextBoxExportDesc;
		private System.Windows.Forms.RadioButton RBtnEntityCoordScreen;
		private System.Windows.Forms.RadioButton RBtnEntityCoordMap;
		private System.Windows.Forms.GroupBox groupBoxEntityCoordinates;
		private System.Windows.Forms.RadioButton RBtnModeMultidirScroll;
		private System.Windows.Forms.RadioButton RBtnModeBidirScroll;
		private System.Windows.Forms.CheckBox CheckBoxExportEntities;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox CheckBoxRLE;
		private System.Windows.Forms.RadioButton RBtnTiles2x2;
		private System.Windows.Forms.RadioButton RBtnTiles4x4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button BtnOk;
		private System.Windows.Forms.Button BtnCancel;
#endif	//DEF_PCE
	}
}
