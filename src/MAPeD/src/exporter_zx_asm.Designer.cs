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
			this.CheckBoxGFXDithering = new System.Windows.Forms.CheckBox();
			this.CheckBoxRLE = new System.Windows.Forms.CheckBox();
			this.RBtnTiles4x4 = new System.Windows.Forms.RadioButton();
			this.RBtnTiles2x2 = new System.Windows.Forms.RadioButton();
			this.BtnOk = new System.Windows.Forms.Button();
			this.BtnCancel = new System.Windows.Forms.Button();
			this.GrpBoxExtraOutput = new System.Windows.Forms.GroupBox();
			this.CheckBoxRenderTilesPNG = new System.Windows.Forms.CheckBox();
			this.CheckBoxRenderLevelPNG = new System.Windows.Forms.CheckBox();
			this.groupBoxColorConversion = new System.Windows.Forms.GroupBox();
			this.CBoxColorConversionModes = new System.Windows.Forms.ComboBox();
			this.NumericUpDownInkFactor = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.RichTextBoxExportDesc = new System.Windows.Forms.RichTextBox();
			this.RBtnPropPerBlock = new System.Windows.Forms.RadioButton();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.RBtnPropPerCHR = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.RBtnTilesDirRows = new System.Windows.Forms.RadioButton();
			this.RBtnTilesDirColumns = new System.Windows.Forms.RadioButton();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.groupBoxEntityCoordinates = new System.Windows.Forms.GroupBox();
			this.RBtnEntityCoordMap = new System.Windows.Forms.RadioButton();
			this.RBtnEntityCoordScreen = new System.Windows.Forms.RadioButton();
			this.CheckBoxExportEntities = new System.Windows.Forms.CheckBox();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.RBtnModeMultidirScroll = new System.Windows.Forms.RadioButton();
			this.RBtnModeBidirScroll = new System.Windows.Forms.RadioButton();
			this.groupBoxLayout = new System.Windows.Forms.GroupBox();
			this.RBtnLayoutMatrix = new System.Windows.Forms.RadioButton();
			this.RBtnLayoutAdjacentScreenIndices = new System.Windows.Forms.RadioButton();
			this.RBtnLayoutAdjacentScreens = new System.Windows.Forms.RadioButton();
			this.CheckBoxExportMarks = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.GrpBoxExtraOutput.SuspendLayout();
			this.groupBoxColorConversion.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownInkFactor)).BeginInit();
			this.groupBox7.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBoxEntityCoordinates.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBoxLayout.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.RBtnTypeMonochrome);
			this.groupBox1.Controls.Add(this.RBtnTypeColor);
			this.groupBox1.Location = new System.Drawing.Point(167, 14);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(111, 65);
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
			this.RBtnTypeColor.Location = new System.Drawing.Point(9, 20);
			this.RBtnTypeColor.Name = "RBtnTypeColor";
			this.RBtnTypeColor.Size = new System.Drawing.Size(90, 15);
			this.RBtnTypeColor.TabIndex = 1;
			this.RBtnTypeColor.TabStop = true;
			this.RBtnTypeColor.Text = "Color";
			this.RBtnTypeColor.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.CheckBoxGFXDithering);
			this.groupBox2.Controls.Add(this.CheckBoxRLE);
			this.groupBox2.Controls.Add(this.RBtnTiles4x4);
			this.groupBox2.Controls.Add(this.RBtnTiles2x2);
			this.groupBox2.Location = new System.Drawing.Point(8, 14);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(153, 65);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tiles";
			// 
			// CheckBoxGFXDithering
			// 
			this.CheckBoxGFXDithering.Checked = true;
			this.CheckBoxGFXDithering.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxGFXDithering.Location = new System.Drawing.Point(72, 40);
			this.CheckBoxGFXDithering.Name = "CheckBoxGFXDithering";
			this.CheckBoxGFXDithering.Size = new System.Drawing.Size(76, 19);
			this.CheckBoxGFXDithering.TabIndex = 6;
			this.CheckBoxGFXDithering.Text = "Dithering";
			this.CheckBoxGFXDithering.UseVisualStyleBackColor = true;
			this.CheckBoxGFXDithering.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// CheckBoxRLE
			// 
			this.CheckBoxRLE.Location = new System.Drawing.Point(72, 20);
			this.CheckBoxRLE.Name = "CheckBoxRLE";
			this.CheckBoxRLE.Size = new System.Drawing.Size(48, 19);
			this.CheckBoxRLE.TabIndex = 6;
			this.CheckBoxRLE.Text = "RLE";
			this.CheckBoxRLE.UseVisualStyleBackColor = true;
			this.CheckBoxRLE.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTiles4x4
			// 
			this.RBtnTiles4x4.Enabled = false;
			this.RBtnTiles4x4.Location = new System.Drawing.Point(14, 40);
			this.RBtnTiles4x4.Name = "RBtnTiles4x4";
			this.RBtnTiles4x4.Size = new System.Drawing.Size(42, 15);
			this.RBtnTiles4x4.TabIndex = 5;
			this.RBtnTiles4x4.Text = "4x4";
			this.RBtnTiles4x4.UseVisualStyleBackColor = true;
			this.RBtnTiles4x4.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTiles2x2
			// 
			this.RBtnTiles2x2.Checked = true;
			this.RBtnTiles2x2.Location = new System.Drawing.Point(14, 20);
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
			this.BtnOk.Location = new System.Drawing.Point(408, 377);
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
			this.BtnCancel.Location = new System.Drawing.Point(489, 377);
			this.BtnCancel.Name = "BtnCancel";
			this.BtnCancel.Size = new System.Drawing.Size(75, 23);
			this.BtnCancel.TabIndex = 22;
			this.BtnCancel.Text = "Cancel";
			this.BtnCancel.UseVisualStyleBackColor = true;
			this.BtnCancel.Click += new System.EventHandler(this.event_cancel);
			// 
			// GrpBoxExtraOutput
			// 
			this.GrpBoxExtraOutput.Controls.Add(this.CheckBoxRenderTilesPNG);
			this.GrpBoxExtraOutput.Controls.Add(this.CheckBoxRenderLevelPNG);
			this.GrpBoxExtraOutput.Location = new System.Drawing.Point(8, 248);
			this.GrpBoxExtraOutput.Name = "GrpBoxExtraOutput";
			this.GrpBoxExtraOutput.Size = new System.Drawing.Size(132, 93);
			this.GrpBoxExtraOutput.TabIndex = 7;
			this.GrpBoxExtraOutput.TabStop = false;
			this.GrpBoxExtraOutput.Text = "Extra Output";
			// 
			// CheckBoxRenderTilesPNG
			// 
			this.CheckBoxRenderTilesPNG.Location = new System.Drawing.Point(14, 41);
			this.CheckBoxRenderTilesPNG.Name = "CheckBoxRenderTilesPNG";
			this.CheckBoxRenderTilesPNG.Size = new System.Drawing.Size(86, 24);
			this.CheckBoxRenderTilesPNG.TabIndex = 9;
			this.CheckBoxRenderTilesPNG.Text = "Tiles .png";
			this.CheckBoxRenderTilesPNG.UseVisualStyleBackColor = true;
			// 
			// CheckBoxRenderLevelPNG
			// 
			this.CheckBoxRenderLevelPNG.Location = new System.Drawing.Point(14, 19);
			this.CheckBoxRenderLevelPNG.Name = "CheckBoxRenderLevelPNG";
			this.CheckBoxRenderLevelPNG.Size = new System.Drawing.Size(86, 24);
			this.CheckBoxRenderLevelPNG.TabIndex = 8;
			this.CheckBoxRenderLevelPNG.Text = "Level .png";
			this.CheckBoxRenderLevelPNG.UseVisualStyleBackColor = true;
			// 
			// groupBoxColorConversion
			// 
			this.groupBoxColorConversion.Controls.Add(this.CBoxColorConversionModes);
			this.groupBoxColorConversion.Controls.Add(this.NumericUpDownInkFactor);
			this.groupBoxColorConversion.Controls.Add(this.label2);
			this.groupBoxColorConversion.Location = new System.Drawing.Point(146, 271);
			this.groupBoxColorConversion.Name = "groupBoxColorConversion";
			this.groupBoxColorConversion.Size = new System.Drawing.Size(132, 70);
			this.groupBoxColorConversion.TabIndex = 10;
			this.groupBoxColorConversion.TabStop = false;
			this.groupBoxColorConversion.Text = "Color Conversion";
			// 
			// CBoxColorConversionModes
			// 
			this.CBoxColorConversionModes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxColorConversionModes.FormattingEnabled = true;
			this.CBoxColorConversionModes.Items.AddRange(new object[] {
									"Mode 1",
									"Mode 2"});
			this.CBoxColorConversionModes.Location = new System.Drawing.Point(11, 42);
			this.CBoxColorConversionModes.Name = "CBoxColorConversionModes";
			this.CBoxColorConversionModes.Size = new System.Drawing.Size(111, 21);
			this.CBoxColorConversionModes.TabIndex = 11;
			// 
			// NumericUpDownInkFactor
			// 
			this.NumericUpDownInkFactor.Location = new System.Drawing.Point(74, 18);
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
			this.label2.Location = new System.Drawing.Point(11, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 17);
			this.label2.TabIndex = 12;
			this.label2.Text = "Ink Factor:";
			// 
			// RichTextBoxExportDesc
			// 
			this.RichTextBoxExportDesc.Location = new System.Drawing.Point(299, 10);
			this.RichTextBoxExportDesc.MaxLength = 2048;
			this.RichTextBoxExportDesc.Name = "RichTextBoxExportDesc";
			this.RichTextBoxExportDesc.ReadOnly = true;
			this.RichTextBoxExportDesc.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.RichTextBoxExportDesc.Size = new System.Drawing.Size(265, 348);
			this.RichTextBoxExportDesc.TabIndex = 20;
			this.RichTextBoxExportDesc.Text = "";
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
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.RBtnPropPerCHR);
			this.groupBox7.Controls.Add(this.RBtnPropPerBlock);
			this.groupBox7.Location = new System.Drawing.Point(8, 201);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(132, 41);
			this.groupBox7.TabIndex = 27;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Property Id per";
			// 
			// RBtnPropPerCHR
			// 
			this.RBtnPropPerCHR.Location = new System.Drawing.Point(72, 16);
			this.RBtnPropPerCHR.Name = "RBtnPropPerCHR";
			this.RBtnPropPerCHR.Size = new System.Drawing.Size(54, 17);
			this.RBtnPropPerCHR.TabIndex = 24;
			this.RBtnPropPerCHR.Text = "CHR";
			this.RBtnPropPerCHR.UseVisualStyleBackColor = true;
			this.RBtnPropPerCHR.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.RBtnTilesDirRows);
			this.groupBox4.Controls.Add(this.RBtnTilesDirColumns);
			this.groupBox4.Location = new System.Drawing.Point(8, 85);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(132, 41);
			this.groupBox4.TabIndex = 23;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Data Order";
			// 
			// RBtnTilesDirRows
			// 
			this.RBtnTilesDirRows.Location = new System.Drawing.Point(80, 17);
			this.RBtnTilesDirRows.Name = "RBtnTilesDirRows";
			this.RBtnTilesDirRows.Size = new System.Drawing.Size(48, 17);
			this.RBtnTilesDirRows.TabIndex = 7;
			this.RBtnTilesDirRows.Text = "Rows";
			this.RBtnTilesDirRows.UseVisualStyleBackColor = true;
			this.RBtnTilesDirRows.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// RBtnTilesDirColumns
			// 
			this.RBtnTilesDirColumns.Checked = true;
			this.RBtnTilesDirColumns.Location = new System.Drawing.Point(14, 17);
			this.RBtnTilesDirColumns.Name = "RBtnTilesDirColumns";
			this.RBtnTilesDirColumns.Size = new System.Drawing.Size(68, 17);
			this.RBtnTilesDirColumns.TabIndex = 6;
			this.RBtnTilesDirColumns.TabStop = true;
			this.RBtnTilesDirColumns.Text = "Columns";
			this.RBtnTilesDirColumns.UseVisualStyleBackColor = true;
			this.RBtnTilesDirColumns.CheckedChanged += new System.EventHandler(this.ParamChanged_Event);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.groupBoxEntityCoordinates);
			this.groupBox8.Controls.Add(this.CheckBoxExportEntities);
			this.groupBox8.Location = new System.Drawing.Point(146, 188);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.groupBox8.Size = new System.Drawing.Size(132, 77);
			this.groupBox8.TabIndex = 26;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Entities";
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
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.RBtnModeMultidirScroll);
			this.groupBox9.Controls.Add(this.RBtnModeBidirScroll);
			this.groupBox9.Location = new System.Drawing.Point(8, 132);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(132, 63);
			this.groupBox9.TabIndex = 25;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Mode";
			// 
			// RBtnModeMultidirScroll
			// 
			this.RBtnModeMultidirScroll.Location = new System.Drawing.Point(14, 16);
			this.RBtnModeMultidirScroll.Name = "RBtnModeMultidirScroll";
			this.RBtnModeMultidirScroll.Size = new System.Drawing.Size(107, 17);
			this.RBtnModeMultidirScroll.TabIndex = 14;
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
			// groupBoxLayout
			// 
			this.groupBoxLayout.Controls.Add(this.RBtnLayoutMatrix);
			this.groupBoxLayout.Controls.Add(this.RBtnLayoutAdjacentScreenIndices);
			this.groupBoxLayout.Controls.Add(this.RBtnLayoutAdjacentScreens);
			this.groupBoxLayout.Controls.Add(this.CheckBoxExportMarks);
			this.groupBoxLayout.Location = new System.Drawing.Point(146, 85);
			this.groupBoxLayout.Name = "groupBoxLayout";
			this.groupBoxLayout.Size = new System.Drawing.Size(132, 97);
			this.groupBoxLayout.TabIndex = 24;
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
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.groupBox7);
			this.groupBox5.Controls.Add(this.groupBox2);
			this.groupBox5.Controls.Add(this.groupBox4);
			this.groupBox5.Controls.Add(this.groupBox1);
			this.groupBox5.Controls.Add(this.groupBox8);
			this.groupBox5.Controls.Add(this.GrpBoxExtraOutput);
			this.groupBox5.Controls.Add(this.groupBox9);
			this.groupBox5.Controls.Add(this.groupBoxColorConversion);
			this.groupBox5.Controls.Add(this.groupBoxLayout);
			this.groupBox5.Location = new System.Drawing.Point(5, 3);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(288, 355);
			this.groupBox5.TabIndex = 28;
			this.groupBox5.TabStop = false;
			// 
			// exporter_zx_sjasm
			// 
			this.AcceptButton = this.BtnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.BtnCancel;
			this.ClientSize = new System.Drawing.Size(572, 412);
			this.Controls.Add(this.BtnCancel);
			this.Controls.Add(this.BtnOk);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.RichTextBoxExportDesc);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "exporter_zx_sjasm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SjASMPlus Export Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.GrpBoxExtraOutput.ResumeLayout(false);
			this.groupBoxColorConversion.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownInkFactor)).EndInit();
			this.groupBox7.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.groupBoxEntityCoordinates.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBoxLayout.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.CheckBox CheckBoxExportMarks;
		private System.Windows.Forms.RadioButton RBtnLayoutAdjacentScreens;
		private System.Windows.Forms.RadioButton RBtnLayoutAdjacentScreenIndices;
		private System.Windows.Forms.RadioButton RBtnLayoutMatrix;
		private System.Windows.Forms.GroupBox groupBoxLayout;
		private System.Windows.Forms.RadioButton RBtnModeBidirScroll;
		private System.Windows.Forms.RadioButton RBtnModeMultidirScroll;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.CheckBox CheckBoxExportEntities;
		private System.Windows.Forms.RadioButton RBtnEntityCoordScreen;
		private System.Windows.Forms.RadioButton RBtnEntityCoordMap;
		private System.Windows.Forms.GroupBox groupBoxEntityCoordinates;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.RadioButton RBtnTilesDirColumns;
		private System.Windows.Forms.RadioButton RBtnTilesDirRows;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton RBtnPropPerCHR;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.RadioButton RBtnPropPerBlock;
		private System.Windows.Forms.CheckBox CheckBoxRLE;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown NumericUpDownInkFactor;
		private System.Windows.Forms.RichTextBox RichTextBoxExportDesc;
		private System.Windows.Forms.ComboBox CBoxColorConversionModes;
		private System.Windows.Forms.GroupBox groupBoxColorConversion;
		private System.Windows.Forms.CheckBox CheckBoxGFXDithering;
		private System.Windows.Forms.CheckBox CheckBoxRenderLevelPNG;
		private System.Windows.Forms.CheckBox CheckBoxRenderTilesPNG;
		private System.Windows.Forms.GroupBox GrpBoxExtraOutput;
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
