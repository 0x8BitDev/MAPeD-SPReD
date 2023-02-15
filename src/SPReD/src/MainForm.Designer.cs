/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 13.03.2017
 * Time: 11:24
 */
using System;

namespace SPReD
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.SpriteList = new System.Windows.Forms.ListBox();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ExportASMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ExportCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ExportImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportScriptEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			this.descriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuStrip = new System.Windows.Forms.MenuStrip();
			this.spriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
			this.addPrefixPostfixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removePrefixPostfixToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.createCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createRefToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.applyPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.verticalFlippingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalFlippingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.CHRDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CHRSplitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CHRPackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CHROptimizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.drawModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.verticalFlippingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalFlippingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.centeringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.shiftColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cHRBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
			this.fillWithColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			this.addCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteCHRToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.flipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.swapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.managerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quickGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SpriteLayout = new System.Windows.Forms.PictureBox();
			this.CHRBank = new System.Windows.Forms.PictureBox();
			this.ImportOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.PaletteMain = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.BtnCHRSplit = new System.Windows.Forms.Button();
			this.CBoxCHRPackingType = new System.Windows.Forms.ComboBox();
			this.BtnCHROptimization = new System.Windows.Forms.Button();
			this.BtnCHRPack = new System.Windows.Forms.Button();
			this.BtnMoveItemDown = new System.Windows.Forms.Button();
			this.BtnMoveItemUp = new System.Windows.Forms.Button();
			this.BtnSelectAll = new System.Windows.Forms.Button();
			this.BtnApplyDefaultPalette = new System.Windows.Forms.Button();
			this.BtnOffset = new System.Windows.Forms.Button();
			this.OffsetY = new System.Windows.Forms.NumericUpDown();
			this.OffsetX = new System.Windows.Forms.NumericUpDown();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.CBoxFlipType = new System.Windows.Forms.ComboBox();
			this.BtnSpriteHFlip = new System.Windows.Forms.Button();
			this.BtnSpriteVFlip = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.LayoutGroupBox = new System.Windows.Forms.GroupBox();
			this.BtnZoomOut = new System.Windows.Forms.Button();
			this.BtnZoomIn = new System.Windows.Forms.Button();
			this.BtnCentering = new System.Windows.Forms.Button();
			this.CBoxAxesLayout = new System.Windows.Forms.CheckBox();
			this.CBoxMode8x16 = new System.Windows.Forms.CheckBox();
			this.CBoxSnapLayout = new System.Windows.Forms.CheckBox();
			this.GroupBoxModeName = new System.Windows.Forms.GroupBox();
			this.BtnLayoutModeBuild = new System.Windows.Forms.Button();
			this.BtnLayoutModeDraw = new System.Windows.Forms.Button();
			this.CHRFlippingGroupBox = new System.Windows.Forms.GroupBox();
			this.BtnVFlip = new System.Windows.Forms.Button();
			this.BtnHFlip = new System.Windows.Forms.Button();
			this.BtnDeleteCHR = new System.Windows.Forms.Button();
			this.BtnShiftColors = new System.Windows.Forms.Button();
			this.CBoxGridLayout = new System.Windows.Forms.CheckBox();
			this.CBoxShiftTransp = new System.Windows.Forms.CheckBox();
			this.SpriteLayoutLabel = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.BtnDeleteLastCHR = new System.Windows.Forms.Button();
			this.BtnCHRRotate = new System.Windows.Forms.Button();
			this.BtnCHRHFlip = new System.Windows.Forms.Button();
			this.BtnAddCHR = new System.Windows.Forms.Button();
			this.BtnCHRVFlip = new System.Windows.Forms.Button();
			this.CHRBankLabel = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.CBoxPalettes = new System.Windows.Forms.ComboBox();
			this.Palette3 = new System.Windows.Forms.PictureBox();
			this.BtnSwapColors = new System.Windows.Forms.Button();
			this.Palette2 = new System.Windows.Forms.PictureBox();
			this.Palette1 = new System.Windows.Forms.PictureBox();
			this.Palette0 = new System.Windows.Forms.PictureBox();
			this.Palette3Label = new System.Windows.Forms.Label();
			this.Palette1Label = new System.Windows.Forms.Label();
			this.Palette2Label = new System.Windows.Forms.Label();
			this.Palette0Label = new System.Windows.Forms.Label();
			this.ExportASMSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.ExportCSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.ProjectSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.ProjectOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.SpriteListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			this.addPrefixPostfixToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.removePrefixPostfixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.createRefToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ExportImagesFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.ContextMenuCHRBank = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.CopyCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PasteCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.FillWithColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuSmallPalette = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SpriteLayout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CHRBank)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PaletteMain)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.OffsetY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.OffsetX)).BeginInit();
			this.groupBox7.SuspendLayout();
			this.LayoutGroupBox.SuspendLayout();
			this.GroupBoxModeName.SuspendLayout();
			this.CHRFlippingGroupBox.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Palette3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette0)).BeginInit();
			this.SpriteListContextMenu.SuspendLayout();
			this.ContextMenuCHRBank.SuspendLayout();
			this.ContextMenuSmallPalette.SuspendLayout();
			this.SuspendLayout();
			// 
			// SpriteList
			// 
			this.SpriteList.BackColor = System.Drawing.Color.Gainsboro;
			this.SpriteList.FormattingEnabled = true;
			this.SpriteList.Location = new System.Drawing.Point(10, 18);
			this.SpriteList.Name = "SpriteList";
			this.SpriteList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.SpriteList.Size = new System.Drawing.Size(170, 212);
			this.SpriteList.TabIndex = 0;
			this.SpriteList.Click += new System.EventHandler(this.SpriteListItemClick);
			this.SpriteList.SelectedIndexChanged += new System.EventHandler(this.SpriteListItemClick);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.loadToolStripMenuItem,
									this.saveToolStripMenuItem,
									this.closeToolStripMenuItem,
									this.toolStripSeparator3,
									this.importToolStripMenuItem,
									this.exportToolStripMenuItem,
									this.exportScriptEditorToolStripMenuItem,
									this.toolStripSeparator17,
									this.descriptionToolStripMenuItem,
									this.statisticsToolStripMenuItem,
									this.toolStripSeparator1,
									this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.loadToolStripMenuItem.Text = "&Load";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItemClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(208, 6);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.importToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.importToolStripMenuItem.Text = "&Import";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItemClick);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.ExportASMToolStripMenuItem,
									this.ExportCToolStripMenuItem,
									this.ExportImagesToolStripMenuItem});
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			// 
			// ExportASMToolStripMenuItem
			// 
			this.ExportASMToolStripMenuItem.Name = "ExportASMToolStripMenuItem";
			this.ExportASMToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.ExportASMToolStripMenuItem.Text = "C&A65\\NESasm";
			this.ExportASMToolStripMenuItem.Click += new System.EventHandler(this.ExportASMToolStripMenuItemClick);
			// 
			// ExportCToolStripMenuItem
			// 
			this.ExportCToolStripMenuItem.Name = "ExportCToolStripMenuItem";
			this.ExportCToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.ExportCToolStripMenuItem.Text = "&C";
			this.ExportCToolStripMenuItem.Click += new System.EventHandler(this.ExportCToolStripMenuItemClick);
			// 
			// ExportImagesToolStripMenuItem
			// 
			this.ExportImagesToolStripMenuItem.Name = "ExportImagesToolStripMenuItem";
			this.ExportImagesToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
			this.ExportImagesToolStripMenuItem.Text = "&Images";
			this.ExportImagesToolStripMenuItem.Click += new System.EventHandler(this.ExportImagesToolStripMenuItemClick);
			// 
			// exportScriptEditorToolStripMenuItem
			// 
			this.exportScriptEditorToolStripMenuItem.Name = "exportScriptEditorToolStripMenuItem";
			this.exportScriptEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
			this.exportScriptEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.exportScriptEditorToolStripMenuItem.Text = "Expo&rt Script Editor";
			this.exportScriptEditorToolStripMenuItem.Click += new System.EventHandler(this.ExportScriptEditorToolStripMenuItemClick);
			// 
			// toolStripSeparator17
			// 
			this.toolStripSeparator17.Name = "toolStripSeparator17";
			this.toolStripSeparator17.Size = new System.Drawing.Size(208, 6);
			// 
			// descriptionToolStripMenuItem
			// 
			this.descriptionToolStripMenuItem.Name = "descriptionToolStripMenuItem";
			this.descriptionToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.descriptionToolStripMenuItem.Text = "&Description";
			this.descriptionToolStripMenuItem.Click += new System.EventHandler(this.DescriptionToolStripMenuItemClick);
			// 
			// statisticsToolStripMenuItem
			// 
			this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
			this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.statisticsToolStripMenuItem.Text = "S&tatistics";
			this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.StatisticsToolStripMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(208, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
			// 
			// MenuStrip
			// 
			this.MenuStrip.BackColor = System.Drawing.SystemColors.Control;
			this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem,
									this.spriteToolStripMenuItem,
									this.layoutToolStripMenuItem,
									this.cHRBankToolStripMenuItem,
									this.paletteToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.MenuStrip.Location = new System.Drawing.Point(0, 0);
			this.MenuStrip.Name = "MenuStrip";
			this.MenuStrip.Size = new System.Drawing.Size(772, 24);
			this.MenuStrip.TabIndex = 2;
			this.MenuStrip.Text = "MenuStrip";
			// 
			// spriteToolStripMenuItem
			// 
			this.spriteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.createNewToolStripMenuItem,
									this.deleteToolStripMenuItem1,
									this.toolStripSeparator13,
									this.renameToolStripMenuItem1,
									this.toolStripSeparator19,
									this.addPrefixPostfixToolStripMenuItem,
									this.removePrefixPostfixToolStripMenuItem1,
									this.toolStripSeparator6,
									this.createCopyToolStripMenuItem,
									this.createRefToolStripMenuItem1,
									this.toolStripSeparator5,
									this.selectAllToolStripMenuItem,
									this.applyPaletteToolStripMenuItem,
									this.toolStripSeparator7,
									this.verticalFlippingToolStripMenuItem,
									this.horizontalFlippingToolStripMenuItem,
									this.toolStripSeparator8,
									this.CHRDataToolStripMenuItem});
			this.spriteToolStripMenuItem.Name = "spriteToolStripMenuItem";
			this.spriteToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
			this.spriteToolStripMenuItem.Text = "&Sprite";
			// 
			// createNewToolStripMenuItem
			// 
			this.createNewToolStripMenuItem.Name = "createNewToolStripMenuItem";
			this.createNewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.createNewToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.createNewToolStripMenuItem.Text = "Create &New";
			this.createNewToolStripMenuItem.Click += new System.EventHandler(this.BtnCreateClick);
			// 
			// deleteToolStripMenuItem1
			// 
			this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
			this.deleteToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
			this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
			this.deleteToolStripMenuItem1.Text = "&Delete";
			this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.BtnDeleteClick);
			// 
			// toolStripSeparator13
			// 
			this.toolStripSeparator13.Name = "toolStripSeparator13";
			this.toolStripSeparator13.Size = new System.Drawing.Size(191, 6);
			// 
			// renameToolStripMenuItem1
			// 
			this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
			this.renameToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.renameToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
			this.renameToolStripMenuItem1.Text = "&Rename";
			this.renameToolStripMenuItem1.Click += new System.EventHandler(this.BtnRenameClick);
			// 
			// toolStripSeparator19
			// 
			this.toolStripSeparator19.Name = "toolStripSeparator19";
			this.toolStripSeparator19.Size = new System.Drawing.Size(191, 6);
			// 
			// addPrefixPostfixToolStripMenuItem
			// 
			this.addPrefixPostfixToolStripMenuItem.Name = "addPrefixPostfixToolStripMenuItem";
			this.addPrefixPostfixToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.addPrefixPostfixToolStripMenuItem.Text = "Add Pre&fix/Postfix";
			this.addPrefixPostfixToolStripMenuItem.Click += new System.EventHandler(this.BtnAddPrefixPostfixClick);
			// 
			// removePrefixPostfixToolStripMenuItem1
			// 
			this.removePrefixPostfixToolStripMenuItem1.Name = "removePrefixPostfixToolStripMenuItem1";
			this.removePrefixPostfixToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
			this.removePrefixPostfixToolStripMenuItem1.Text = "Remove Prefix/Postfix";
			this.removePrefixPostfixToolStripMenuItem1.Click += new System.EventHandler(this.BtnRemovePrefixPostfixClick);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(191, 6);
			// 
			// createCopyToolStripMenuItem
			// 
			this.createCopyToolStripMenuItem.Name = "createCopyToolStripMenuItem";
			this.createCopyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.createCopyToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.createCopyToolStripMenuItem.Text = "Create &Copy";
			this.createCopyToolStripMenuItem.Click += new System.EventHandler(this.BtnCreateCopyClick);
			// 
			// createRefToolStripMenuItem1
			// 
			this.createRefToolStripMenuItem1.Name = "createRefToolStripMenuItem1";
			this.createRefToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.createRefToolStripMenuItem1.Size = new System.Drawing.Size(194, 22);
			this.createRefToolStripMenuItem1.Text = "Create R&ef";
			this.createRefToolStripMenuItem1.Click += new System.EventHandler(this.BtnCreateRefClick);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(191, 6);
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.selectAllToolStripMenuItem.Text = "Select &All";
			this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.BtnSelectAllClick);
			// 
			// applyPaletteToolStripMenuItem
			// 
			this.applyPaletteToolStripMenuItem.Name = "applyPaletteToolStripMenuItem";
			this.applyPaletteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.applyPaletteToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.applyPaletteToolStripMenuItem.Text = "Apply &Palette";
			this.applyPaletteToolStripMenuItem.Click += new System.EventHandler(this.BtnApplyDefaultPaletteClick);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(191, 6);
			// 
			// verticalFlippingToolStripMenuItem
			// 
			this.verticalFlippingToolStripMenuItem.Name = "verticalFlippingToolStripMenuItem";
			this.verticalFlippingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.verticalFlippingToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.verticalFlippingToolStripMenuItem.Text = "Flip &Vertical";
			this.verticalFlippingToolStripMenuItem.Click += new System.EventHandler(this.BtnSpriteVFlipClick);
			// 
			// horizontalFlippingToolStripMenuItem
			// 
			this.horizontalFlippingToolStripMenuItem.Name = "horizontalFlippingToolStripMenuItem";
			this.horizontalFlippingToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.horizontalFlippingToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.horizontalFlippingToolStripMenuItem.Text = "Flip &Horizontal";
			this.horizontalFlippingToolStripMenuItem.Click += new System.EventHandler(this.BtnSpriteHFlipClick);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(191, 6);
			// 
			// CHRDataToolStripMenuItem
			// 
			this.CHRDataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.CHRSplitToolStripMenuItem,
									this.CHRPackToolStripMenuItem,
									this.CHROptimizationToolStripMenuItem});
			this.CHRDataToolStripMenuItem.Name = "CHRDataToolStripMenuItem";
			this.CHRDataToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
			this.CHRDataToolStripMenuItem.Text = "CHR Da&ta";
			// 
			// CHRSplitToolStripMenuItem
			// 
			this.CHRSplitToolStripMenuItem.Name = "CHRSplitToolStripMenuItem";
			this.CHRSplitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
			this.CHRSplitToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.CHRSplitToolStripMenuItem.Text = "&Splitting";
			this.CHRSplitToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRSplitClick);
			// 
			// CHRPackToolStripMenuItem
			// 
			this.CHRPackToolStripMenuItem.Name = "CHRPackToolStripMenuItem";
			this.CHRPackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.P)));
			this.CHRPackToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.CHRPackToolStripMenuItem.Text = "&Packing";
			this.CHRPackToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRPackClick);
			// 
			// CHROptimizationToolStripMenuItem
			// 
			this.CHROptimizationToolStripMenuItem.Name = "CHROptimizationToolStripMenuItem";
			this.CHROptimizationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.O)));
			this.CHROptimizationToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.CHROptimizationToolStripMenuItem.Text = "&Optimization";
			this.CHROptimizationToolStripMenuItem.Click += new System.EventHandler(this.BtnCHROptClick);
			// 
			// layoutToolStripMenuItem
			// 
			this.layoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.buildModeToolStripMenuItem,
									this.drawModeToolStripMenuItem,
									this.toolStripSeparator9,
									this.verticalFlippingToolStripMenuItem1,
									this.horizontalFlippingToolStripMenuItem1,
									this.toolStripSeparator10,
									this.centeringToolStripMenuItem,
									this.zoomInToolStripMenuItem,
									this.zoomOutToolStripMenuItem,
									this.toolStripSeparator11,
									this.shiftColorsToolStripMenuItem,
									this.deleteCHRToolStripMenuItem});
			this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
			this.layoutToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
			this.layoutToolStripMenuItem.Text = "&Layout";
			// 
			// buildModeToolStripMenuItem
			// 
			this.buildModeToolStripMenuItem.Name = "buildModeToolStripMenuItem";
			this.buildModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
			this.buildModeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.buildModeToolStripMenuItem.Text = "&Build Mode";
			this.buildModeToolStripMenuItem.Click += new System.EventHandler(this.BtnModeBuildClick);
			// 
			// drawModeToolStripMenuItem
			// 
			this.drawModeToolStripMenuItem.Name = "drawModeToolStripMenuItem";
			this.drawModeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D)));
			this.drawModeToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.drawModeToolStripMenuItem.Text = "&Draw Mode";
			this.drawModeToolStripMenuItem.Click += new System.EventHandler(this.BtnModeDrawClick);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(195, 6);
			// 
			// verticalFlippingToolStripMenuItem1
			// 
			this.verticalFlippingToolStripMenuItem1.Name = "verticalFlippingToolStripMenuItem1";
			this.verticalFlippingToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.V)));
			this.verticalFlippingToolStripMenuItem1.Size = new System.Drawing.Size(198, 22);
			this.verticalFlippingToolStripMenuItem1.Text = "Flip &Vertical";
			this.verticalFlippingToolStripMenuItem1.Click += new System.EventHandler(this.BtnVFlipClick);
			// 
			// horizontalFlippingToolStripMenuItem1
			// 
			this.horizontalFlippingToolStripMenuItem1.Name = "horizontalFlippingToolStripMenuItem1";
			this.horizontalFlippingToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.H)));
			this.horizontalFlippingToolStripMenuItem1.Size = new System.Drawing.Size(198, 22);
			this.horizontalFlippingToolStripMenuItem1.Text = "Flip &Horizontal";
			this.horizontalFlippingToolStripMenuItem1.Click += new System.EventHandler(this.BtnHFlipClick);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(195, 6);
			// 
			// centeringToolStripMenuItem
			// 
			this.centeringToolStripMenuItem.Name = "centeringToolStripMenuItem";
			this.centeringToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.centeringToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.centeringToolStripMenuItem.Text = "&Centering";
			this.centeringToolStripMenuItem.Click += new System.EventHandler(this.BtnCenteringClick);
			// 
			// zoomInToolStripMenuItem
			// 
			this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
			this.zoomInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Z)));
			this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.zoomInToolStripMenuItem.Text = "Zoom In";
			this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.BtnZoomInClick);
			// 
			// zoomOutToolStripMenuItem
			// 
			this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
			this.zoomOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.Z)));
			this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.zoomOutToolStripMenuItem.Text = "Zoom Out";
			this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.BtnZoomOutClick);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(195, 6);
			// 
			// shiftColorsToolStripMenuItem
			// 
			this.shiftColorsToolStripMenuItem.Name = "shiftColorsToolStripMenuItem";
			this.shiftColorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
			this.shiftColorsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.shiftColorsToolStripMenuItem.Text = "Shift Colors";
			this.shiftColorsToolStripMenuItem.Click += new System.EventHandler(this.BtnShiftColorsClick);
			// 
			// deleteCHRToolStripMenuItem
			// 
			this.deleteCHRToolStripMenuItem.Name = "deleteCHRToolStripMenuItem";
			this.deleteCHRToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.deleteCHRToolStripMenuItem.Text = "Delete CHR";
			this.deleteCHRToolStripMenuItem.Click += new System.EventHandler(this.BtnDeleteCHRClick);
			// 
			// cHRBankToolStripMenuItem
			// 
			this.cHRBankToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyToolStripMenuItem,
									this.pasteToolStripMenuItem,
									this.toolStripSeparator15,
									this.fillWithColorToolStripMenuItem1,
									this.toolStripSeparator16,
									this.addCHRToolStripMenuItem,
									this.deleteCHRToolStripMenuItem1,
									this.toolStripSeparator12,
									this.flipVerticalToolStripMenuItem,
									this.flipHorizontalToolStripMenuItem,
									this.rotateToolStripMenuItem});
			this.cHRBankToolStripMenuItem.Name = "cHRBankToolStripMenuItem";
			this.cHRBankToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.cHRBankToolStripMenuItem.Text = "&CHR";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Insert)));
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyCHRToolStripMenuItemClick);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Insert)));
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteCHRToolStripMenuItemClick);
			// 
			// toolStripSeparator15
			// 
			this.toolStripSeparator15.Name = "toolStripSeparator15";
			this.toolStripSeparator15.Size = new System.Drawing.Size(223, 6);
			// 
			// fillWithColorToolStripMenuItem1
			// 
			this.fillWithColorToolStripMenuItem1.Name = "fillWithColorToolStripMenuItem1";
			this.fillWithColorToolStripMenuItem1.Size = new System.Drawing.Size(226, 22);
			this.fillWithColorToolStripMenuItem1.Text = "&Fill With Color";
			this.fillWithColorToolStripMenuItem1.Click += new System.EventHandler(this.FillWithColorToolStripMenuItemClick);
			// 
			// toolStripSeparator16
			// 
			this.toolStripSeparator16.Name = "toolStripSeparator16";
			this.toolStripSeparator16.Size = new System.Drawing.Size(223, 6);
			// 
			// addCHRToolStripMenuItem
			// 
			this.addCHRToolStripMenuItem.Name = "addCHRToolStripMenuItem";
			this.addCHRToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.A)));
			this.addCHRToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
			this.addCHRToolStripMenuItem.Text = "&Add";
			this.addCHRToolStripMenuItem.Click += new System.EventHandler(this.BtnAddCHRClick);
			// 
			// deleteCHRToolStripMenuItem1
			// 
			this.deleteCHRToolStripMenuItem1.Name = "deleteCHRToolStripMenuItem1";
			this.deleteCHRToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.D)));
			this.deleteCHRToolStripMenuItem1.Size = new System.Drawing.Size(226, 22);
			this.deleteCHRToolStripMenuItem1.Text = "&Delete";
			this.deleteCHRToolStripMenuItem1.Click += new System.EventHandler(this.BtnDeleteLastCHRClick);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(223, 6);
			// 
			// flipVerticalToolStripMenuItem
			// 
			this.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem";
			this.flipVerticalToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.V)));
			this.flipVerticalToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
			this.flipVerticalToolStripMenuItem.Text = "Flip &Vertical";
			this.flipVerticalToolStripMenuItem.Click += new System.EventHandler(this.BtnCHREditorVFlipClick);
			// 
			// flipHorizontalToolStripMenuItem
			// 
			this.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem";
			this.flipHorizontalToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.H)));
			this.flipHorizontalToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
			this.flipHorizontalToolStripMenuItem.Text = "Flip &Horizontal";
			this.flipHorizontalToolStripMenuItem.Click += new System.EventHandler(this.BtnCHREditorHFlipClick);
			// 
			// rotateToolStripMenuItem
			// 
			this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
			this.rotateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.R)));
			this.rotateToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
			this.rotateToolStripMenuItem.Text = "&Rotate";
			this.rotateToolStripMenuItem.Click += new System.EventHandler(this.BtnCHREditorRotateClick);
			// 
			// paletteToolStripMenuItem
			// 
			this.paletteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.swapToolStripMenuItem,
									this.managerToolStripMenuItem});
			this.paletteToolStripMenuItem.Name = "paletteToolStripMenuItem";
			this.paletteToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
			this.paletteToolStripMenuItem.Text = "&Palette";
			// 
			// swapToolStripMenuItem
			// 
			this.swapToolStripMenuItem.Name = "swapToolStripMenuItem";
			this.swapToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
									| System.Windows.Forms.Keys.S)));
			this.swapToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.swapToolStripMenuItem.Text = "&Swap Colors";
			this.swapToolStripMenuItem.Click += new System.EventHandler(this.BtnSwapColorsClick);
			// 
			// managerToolStripMenuItem
			// 
			this.managerToolStripMenuItem.Name = "managerToolStripMenuItem";
			this.managerToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
			this.managerToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.managerToolStripMenuItem.Text = "&Manager";
			this.managerToolStripMenuItem.Click += new System.EventHandler(this.PalettesManagerClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.quickGuideToolStripMenuItem,
									this.toolStripSeparator2,
									this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// quickGuideToolStripMenuItem
			// 
			this.quickGuideToolStripMenuItem.Name = "quickGuideToolStripMenuItem";
			this.quickGuideToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.quickGuideToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.quickGuideToolStripMenuItem.Text = "&Quick Guide";
			this.quickGuideToolStripMenuItem.Click += new System.EventHandler(this.MenuHelpQuickGuideClick);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.MenuHelpAboutClick);
			// 
			// SpriteLayout
			// 
			this.SpriteLayout.BackColor = System.Drawing.SystemColors.WindowText;
			this.SpriteLayout.Cursor = System.Windows.Forms.Cursors.Hand;
			this.SpriteLayout.Location = new System.Drawing.Point(10, 18);
			this.SpriteLayout.Name = "SpriteLayout";
			this.SpriteLayout.Size = new System.Drawing.Size(256, 326);
			this.SpriteLayout.TabIndex = 3;
			this.SpriteLayout.TabStop = false;
			// 
			// CHRBank
			// 
			this.CHRBank.BackColor = System.Drawing.SystemColors.WindowText;
			this.CHRBank.Location = new System.Drawing.Point(10, 18);
			this.CHRBank.Name = "CHRBank";
			this.CHRBank.Size = new System.Drawing.Size(256, 256);
			this.CHRBank.TabIndex = 5;
			this.CHRBank.TabStop = false;
			// 
			// ImportOpenFileDialog
			// 
			this.ImportOpenFileDialog.DefaultExt = "*";
			this.ImportOpenFileDialog.Filter = "4 colors image (*.png,*.bmp)|*.png;*.bmp|CHR Bank (*.chr,*.bin)|*.chr;*.bin|Palet" +
			"te (192 bytes) (*.pal)|*.pal";
			this.ImportOpenFileDialog.Multiselect = true;
			this.ImportOpenFileDialog.Title = "Data Import";
			this.ImportOpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ImportOK);
			// 
			// PaletteMain
			// 
			this.PaletteMain.BackColor = System.Drawing.SystemColors.WindowText;
			this.PaletteMain.Location = new System.Drawing.Point(10, 80);
			this.PaletteMain.Name = "PaletteMain";
			this.PaletteMain.Size = new System.Drawing.Size(256, 64);
			this.PaletteMain.TabIndex = 6;
			this.PaletteMain.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox6);
			this.groupBox1.Controls.Add(this.BtnMoveItemDown);
			this.groupBox1.Controls.Add(this.BtnMoveItemUp);
			this.groupBox1.Controls.Add(this.BtnSelectAll);
			this.groupBox1.Controls.Add(this.BtnApplyDefaultPalette);
			this.groupBox1.Controls.Add(this.BtnOffset);
			this.groupBox1.Controls.Add(this.OffsetY);
			this.groupBox1.Controls.Add(this.OffsetX);
			this.groupBox1.Controls.Add(this.SpriteList);
			this.groupBox1.Controls.Add(this.groupBox7);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(10, 28);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(190, 491);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sprite Tools:";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.BtnCHRSplit);
			this.groupBox6.Controls.Add(this.CBoxCHRPackingType);
			this.groupBox6.Controls.Add(this.BtnCHROptimization);
			this.groupBox6.Controls.Add(this.BtnCHRPack);
			this.groupBox6.Location = new System.Drawing.Point(11, 405);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(169, 76);
			this.groupBox6.TabIndex = 5;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "CHR Data";
			// 
			// BtnCHRSplit
			// 
			this.BtnCHRSplit.Location = new System.Drawing.Point(10, 19);
			this.BtnCHRSplit.Name = "BtnCHRSplit";
			this.BtnCHRSplit.Size = new System.Drawing.Size(72, 23);
			this.BtnCHRSplit.TabIndex = 12;
			this.BtnCHRSplit.Text = "Splitting";
			this.BtnCHRSplit.UseVisualStyleBackColor = true;
			this.BtnCHRSplit.Click += new System.EventHandler(this.BtnCHRSplitClick);
			// 
			// CBoxCHRPackingType
			// 
			this.CBoxCHRPackingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxCHRPackingType.FormattingEnabled = true;
			this.CBoxCHRPackingType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.CBoxCHRPackingType.ItemHeight = 13;
			this.CBoxCHRPackingType.Items.AddRange(new object[] {
									"*****",
									"1KB",
									"2KB",
									"4KB"});
			this.CBoxCHRPackingType.Location = new System.Drawing.Point(87, 49);
			this.CBoxCHRPackingType.Name = "CBoxCHRPackingType";
			this.CBoxCHRPackingType.Size = new System.Drawing.Size(72, 21);
			this.CBoxCHRPackingType.TabIndex = 15;
			this.CBoxCHRPackingType.SelectedIndexChanged += new System.EventHandler(this.CBoxCHRPackingTypeChanged);
			// 
			// BtnCHROptimization
			// 
			this.BtnCHROptimization.Location = new System.Drawing.Point(10, 48);
			this.BtnCHROptimization.Name = "BtnCHROptimization";
			this.BtnCHROptimization.Size = new System.Drawing.Size(72, 23);
			this.BtnCHROptimization.TabIndex = 14;
			this.BtnCHROptimization.Text = "Optimization";
			this.BtnCHROptimization.UseVisualStyleBackColor = true;
			this.BtnCHROptimization.Click += new System.EventHandler(this.BtnCHROptClick);
			// 
			// BtnCHRPack
			// 
			this.BtnCHRPack.Location = new System.Drawing.Point(87, 19);
			this.BtnCHRPack.Name = "BtnCHRPack";
			this.BtnCHRPack.Size = new System.Drawing.Size(72, 23);
			this.BtnCHRPack.TabIndex = 13;
			this.BtnCHRPack.Text = "Packing";
			this.BtnCHRPack.UseVisualStyleBackColor = true;
			this.BtnCHRPack.Click += new System.EventHandler(this.BtnCHRPackClick);
			// 
			// BtnMoveItemDown
			// 
			this.BtnMoveItemDown.Location = new System.Drawing.Point(98, 236);
			this.BtnMoveItemDown.Name = "BtnMoveItemDown";
			this.BtnMoveItemDown.Size = new System.Drawing.Size(82, 23);
			this.BtnMoveItemDown.TabIndex = 2;
			this.BtnMoveItemDown.Text = "Down";
			this.BtnMoveItemDown.UseVisualStyleBackColor = true;
			this.BtnMoveItemDown.Click += new System.EventHandler(this.BtnDownClick);
			// 
			// BtnMoveItemUp
			// 
			this.BtnMoveItemUp.Location = new System.Drawing.Point(10, 236);
			this.BtnMoveItemUp.Name = "BtnMoveItemUp";
			this.BtnMoveItemUp.Size = new System.Drawing.Size(82, 23);
			this.BtnMoveItemUp.TabIndex = 1;
			this.BtnMoveItemUp.Text = "Up";
			this.BtnMoveItemUp.UseVisualStyleBackColor = true;
			this.BtnMoveItemUp.Click += new System.EventHandler(this.BtnUpClick);
			// 
			// BtnSelectAll
			// 
			this.BtnSelectAll.Location = new System.Drawing.Point(10, 265);
			this.BtnSelectAll.Name = "BtnSelectAll";
			this.BtnSelectAll.Size = new System.Drawing.Size(82, 23);
			this.BtnSelectAll.TabIndex = 3;
			this.BtnSelectAll.Text = "Select All";
			this.BtnSelectAll.UseVisualStyleBackColor = true;
			this.BtnSelectAll.Click += new System.EventHandler(this.BtnSelectAllClick);
			// 
			// BtnApplyDefaultPalette
			// 
			this.BtnApplyDefaultPalette.Location = new System.Drawing.Point(98, 265);
			this.BtnApplyDefaultPalette.Name = "BtnApplyDefaultPalette";
			this.BtnApplyDefaultPalette.Size = new System.Drawing.Size(82, 23);
			this.BtnApplyDefaultPalette.TabIndex = 4;
			this.BtnApplyDefaultPalette.Text = "Apply Palette";
			this.BtnApplyDefaultPalette.UseVisualStyleBackColor = true;
			this.BtnApplyDefaultPalette.Click += new System.EventHandler(this.BtnApplyDefaultPaletteClick);
			// 
			// BtnOffset
			// 
			this.BtnOffset.Location = new System.Drawing.Point(10, 377);
			this.BtnOffset.Name = "BtnOffset";
			this.BtnOffset.Size = new System.Drawing.Size(51, 23);
			this.BtnOffset.TabIndex = 9;
			this.BtnOffset.Text = "Offset";
			this.BtnOffset.UseVisualStyleBackColor = true;
			this.BtnOffset.Click += new System.EventHandler(this.BtnOffsetClick);
			// 
			// OffsetY
			// 
			this.OffsetY.Location = new System.Drawing.Point(137, 379);
			this.OffsetY.Maximum = new decimal(new int[] {
									256,
									0,
									0,
									0});
			this.OffsetY.Minimum = new decimal(new int[] {
									256,
									0,
									0,
									-2147483648});
			this.OffsetY.Name = "OffsetY";
			this.OffsetY.Size = new System.Drawing.Size(42, 20);
			this.OffsetY.TabIndex = 11;
			// 
			// OffsetX
			// 
			this.OffsetX.Location = new System.Drawing.Point(79, 379);
			this.OffsetX.Maximum = new decimal(new int[] {
									256,
									0,
									0,
									0});
			this.OffsetX.Minimum = new decimal(new int[] {
									256,
									0,
									0,
									-2147483648});
			this.OffsetX.Name = "OffsetX";
			this.OffsetX.Size = new System.Drawing.Size(42, 20);
			this.OffsetX.TabIndex = 10;
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.CBoxFlipType);
			this.groupBox7.Controls.Add(this.BtnSpriteHFlip);
			this.groupBox7.Controls.Add(this.BtnSpriteVFlip);
			this.groupBox7.Controls.Add(this.label7);
			this.groupBox7.Controls.Add(this.label6);
			this.groupBox7.Location = new System.Drawing.Point(11, 293);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(169, 76);
			this.groupBox7.TabIndex = 5;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Flipping";
			// 
			// CBoxFlipType
			// 
			this.CBoxFlipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxFlipType.FormattingEnabled = true;
			this.CBoxFlipType.Items.AddRange(new object[] {
									"LOCAL AXIS",
									"GLOBAL AXIS"});
			this.CBoxFlipType.Location = new System.Drawing.Point(10, 20);
			this.CBoxFlipType.Name = "CBoxFlipType";
			this.CBoxFlipType.Size = new System.Drawing.Size(149, 21);
			this.CBoxFlipType.TabIndex = 6;
			// 
			// BtnSpriteHFlip
			// 
			this.BtnSpriteHFlip.Location = new System.Drawing.Point(88, 47);
			this.BtnSpriteHFlip.Name = "BtnSpriteHFlip";
			this.BtnSpriteHFlip.Size = new System.Drawing.Size(72, 23);
			this.BtnSpriteHFlip.TabIndex = 8;
			this.BtnSpriteHFlip.Text = "HFlip";
			this.BtnSpriteHFlip.UseVisualStyleBackColor = true;
			this.BtnSpriteHFlip.Click += new System.EventHandler(this.BtnSpriteHFlipClick);
			// 
			// BtnSpriteVFlip
			// 
			this.BtnSpriteVFlip.Location = new System.Drawing.Point(9, 47);
			this.BtnSpriteVFlip.Name = "BtnSpriteVFlip";
			this.BtnSpriteVFlip.Size = new System.Drawing.Size(72, 23);
			this.BtnSpriteVFlip.TabIndex = 7;
			this.BtnSpriteVFlip.Text = "VFlip";
			this.BtnSpriteVFlip.UseVisualStyleBackColor = true;
			this.BtnSpriteVFlip.Click += new System.EventHandler(this.BtnSpriteVFlipClick);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(113, 28);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(24, 17);
			this.label7.TabIndex = 7;
			this.label7.Text = "Y:";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(57, 28);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(24, 17);
			this.label6.TabIndex = 7;
			this.label6.Text = "X:";
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.label8.Location = new System.Drawing.Point(121, 382);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(25, 20);
			this.label8.TabIndex = 8;
			this.label8.Text = "Y:";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.label5.Location = new System.Drawing.Point(63, 382);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(25, 20);
			this.label5.TabIndex = 8;
			this.label5.Text = "X:";
			// 
			// LayoutGroupBox
			// 
			this.LayoutGroupBox.Controls.Add(this.SpriteLayout);
			this.LayoutGroupBox.Controls.Add(this.BtnZoomOut);
			this.LayoutGroupBox.Controls.Add(this.BtnZoomIn);
			this.LayoutGroupBox.Controls.Add(this.BtnCentering);
			this.LayoutGroupBox.Controls.Add(this.CBoxAxesLayout);
			this.LayoutGroupBox.Controls.Add(this.CBoxMode8x16);
			this.LayoutGroupBox.Controls.Add(this.CBoxSnapLayout);
			this.LayoutGroupBox.Controls.Add(this.GroupBoxModeName);
			this.LayoutGroupBox.Controls.Add(this.CHRFlippingGroupBox);
			this.LayoutGroupBox.Controls.Add(this.BtnDeleteCHR);
			this.LayoutGroupBox.Controls.Add(this.BtnShiftColors);
			this.LayoutGroupBox.Controls.Add(this.CBoxGridLayout);
			this.LayoutGroupBox.Controls.Add(this.CBoxShiftTransp);
			this.LayoutGroupBox.Controls.Add(this.SpriteLayoutLabel);
			this.LayoutGroupBox.Location = new System.Drawing.Point(206, 28);
			this.LayoutGroupBox.Name = "LayoutGroupBox";
			this.LayoutGroupBox.Size = new System.Drawing.Size(276, 491);
			this.LayoutGroupBox.TabIndex = 8;
			this.LayoutGroupBox.TabStop = false;
			this.LayoutGroupBox.Text = "Layout:";
			// 
			// BtnZoomOut
			// 
			this.BtnZoomOut.Location = new System.Drawing.Point(238, 379);
			this.BtnZoomOut.Name = "BtnZoomOut";
			this.BtnZoomOut.Size = new System.Drawing.Size(28, 23);
			this.BtnZoomOut.TabIndex = 24;
			this.BtnZoomOut.Text = "Z-";
			this.BtnZoomOut.UseVisualStyleBackColor = true;
			this.BtnZoomOut.Click += new System.EventHandler(this.BtnZoomOutClick);
			// 
			// BtnZoomIn
			// 
			this.BtnZoomIn.Location = new System.Drawing.Point(207, 379);
			this.BtnZoomIn.Name = "BtnZoomIn";
			this.BtnZoomIn.Size = new System.Drawing.Size(28, 23);
			this.BtnZoomIn.TabIndex = 23;
			this.BtnZoomIn.Text = "Z+";
			this.BtnZoomIn.UseVisualStyleBackColor = true;
			this.BtnZoomIn.Click += new System.EventHandler(this.BtnZoomInClick);
			// 
			// BtnCentering
			// 
			this.BtnCentering.Location = new System.Drawing.Point(140, 379);
			this.BtnCentering.Name = "BtnCentering";
			this.BtnCentering.Size = new System.Drawing.Size(61, 23);
			this.BtnCentering.TabIndex = 22;
			this.BtnCentering.Text = "Centering";
			this.BtnCentering.UseVisualStyleBackColor = true;
			this.BtnCentering.Click += new System.EventHandler(this.BtnCenteringClick);
			// 
			// CBoxAxesLayout
			// 
			this.CBoxAxesLayout.Checked = true;
			this.CBoxAxesLayout.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxAxesLayout.Location = new System.Drawing.Point(221, 411);
			this.CBoxAxesLayout.Name = "CBoxAxesLayout";
			this.CBoxAxesLayout.Size = new System.Drawing.Size(50, 17);
			this.CBoxAxesLayout.TabIndex = 28;
			this.CBoxAxesLayout.Text = "Axes";
			this.CBoxAxesLayout.UseVisualStyleBackColor = true;
			this.CBoxAxesLayout.CheckedChanged += new System.EventHandler(this.CBoxAxisLayoutCheckedChanged);
			// 
			// CBoxMode8x16
			// 
			this.CBoxMode8x16.Location = new System.Drawing.Point(221, 468);
			this.CBoxMode8x16.Name = "CBoxMode8x16";
			this.CBoxMode8x16.Size = new System.Drawing.Size(51, 17);
			this.CBoxMode8x16.TabIndex = 30;
			this.CBoxMode8x16.Text = "8x16";
			this.CBoxMode8x16.UseVisualStyleBackColor = true;
			this.CBoxMode8x16.CheckedChanged += new System.EventHandler(this.CBox8x16ModeCheckedChanged);
			// 
			// CBoxSnapLayout
			// 
			this.CBoxSnapLayout.Checked = true;
			this.CBoxSnapLayout.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxSnapLayout.Location = new System.Drawing.Point(221, 449);
			this.CBoxSnapLayout.Name = "CBoxSnapLayout";
			this.CBoxSnapLayout.Size = new System.Drawing.Size(51, 17);
			this.CBoxSnapLayout.TabIndex = 30;
			this.CBoxSnapLayout.Text = "Snap";
			this.CBoxSnapLayout.UseVisualStyleBackColor = true;
			this.CBoxSnapLayout.CheckedChanged += new System.EventHandler(this.CBoxSnapLayoutCheckedChanged);
			// 
			// GroupBoxModeName
			// 
			this.GroupBoxModeName.Controls.Add(this.BtnLayoutModeBuild);
			this.GroupBoxModeName.Controls.Add(this.BtnLayoutModeDraw);
			this.GroupBoxModeName.Location = new System.Drawing.Point(10, 373);
			this.GroupBoxModeName.Name = "GroupBoxModeName";
			this.GroupBoxModeName.Size = new System.Drawing.Size(124, 51);
			this.GroupBoxModeName.TabIndex = 16;
			this.GroupBoxModeName.TabStop = false;
			this.GroupBoxModeName.Text = "Modes";
			// 
			// BtnLayoutModeBuild
			// 
			this.BtnLayoutModeBuild.Location = new System.Drawing.Point(8, 19);
			this.BtnLayoutModeBuild.Name = "BtnLayoutModeBuild";
			this.BtnLayoutModeBuild.Size = new System.Drawing.Size(51, 23);
			this.BtnLayoutModeBuild.TabIndex = 17;
			this.BtnLayoutModeBuild.Text = "Build";
			this.BtnLayoutModeBuild.UseVisualStyleBackColor = true;
			this.BtnLayoutModeBuild.Click += new System.EventHandler(this.BtnModeBuildClick);
			// 
			// BtnLayoutModeDraw
			// 
			this.BtnLayoutModeDraw.Location = new System.Drawing.Point(65, 19);
			this.BtnLayoutModeDraw.Name = "BtnLayoutModeDraw";
			this.BtnLayoutModeDraw.Size = new System.Drawing.Size(51, 23);
			this.BtnLayoutModeDraw.TabIndex = 18;
			this.BtnLayoutModeDraw.Text = "Draw";
			this.BtnLayoutModeDraw.UseVisualStyleBackColor = true;
			this.BtnLayoutModeDraw.Click += new System.EventHandler(this.BtnModeDrawClick);
			// 
			// CHRFlippingGroupBox
			// 
			this.CHRFlippingGroupBox.Controls.Add(this.BtnVFlip);
			this.CHRFlippingGroupBox.Controls.Add(this.BtnHFlip);
			this.CHRFlippingGroupBox.Location = new System.Drawing.Point(10, 430);
			this.CHRFlippingGroupBox.Name = "CHRFlippingGroupBox";
			this.CHRFlippingGroupBox.Size = new System.Drawing.Size(124, 51);
			this.CHRFlippingGroupBox.TabIndex = 19;
			this.CHRFlippingGroupBox.TabStop = false;
			this.CHRFlippingGroupBox.Text = "CHR Flipping";
			// 
			// BtnVFlip
			// 
			this.BtnVFlip.Location = new System.Drawing.Point(8, 19);
			this.BtnVFlip.Name = "BtnVFlip";
			this.BtnVFlip.Size = new System.Drawing.Size(51, 23);
			this.BtnVFlip.TabIndex = 20;
			this.BtnVFlip.Text = "VFlip";
			this.BtnVFlip.UseVisualStyleBackColor = true;
			this.BtnVFlip.Click += new System.EventHandler(this.BtnVFlipClick);
			// 
			// BtnHFlip
			// 
			this.BtnHFlip.Location = new System.Drawing.Point(65, 19);
			this.BtnHFlip.Name = "BtnHFlip";
			this.BtnHFlip.Size = new System.Drawing.Size(51, 23);
			this.BtnHFlip.TabIndex = 21;
			this.BtnHFlip.Text = "HFlip";
			this.BtnHFlip.UseVisualStyleBackColor = true;
			this.BtnHFlip.Click += new System.EventHandler(this.BtnHFlipClick);
			// 
			// BtnDeleteCHR
			// 
			this.BtnDeleteCHR.Location = new System.Drawing.Point(140, 459);
			this.BtnDeleteCHR.Name = "BtnDeleteCHR";
			this.BtnDeleteCHR.Size = new System.Drawing.Size(71, 23);
			this.BtnDeleteCHR.TabIndex = 27;
			this.BtnDeleteCHR.Text = "Delete";
			this.BtnDeleteCHR.UseVisualStyleBackColor = true;
			this.BtnDeleteCHR.Click += new System.EventHandler(this.BtnDeleteCHRClick);
			// 
			// BtnShiftColors
			// 
			this.BtnShiftColors.Location = new System.Drawing.Point(140, 429);
			this.BtnShiftColors.Name = "BtnShiftColors";
			this.BtnShiftColors.Size = new System.Drawing.Size(71, 23);
			this.BtnShiftColors.TabIndex = 26;
			this.BtnShiftColors.Text = "Shift Colors";
			this.BtnShiftColors.UseVisualStyleBackColor = true;
			this.BtnShiftColors.Click += new System.EventHandler(this.BtnShiftColorsClick);
			// 
			// CBoxGridLayout
			// 
			this.CBoxGridLayout.Checked = true;
			this.CBoxGridLayout.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CBoxGridLayout.Location = new System.Drawing.Point(221, 430);
			this.CBoxGridLayout.Name = "CBoxGridLayout";
			this.CBoxGridLayout.Size = new System.Drawing.Size(48, 17);
			this.CBoxGridLayout.TabIndex = 29;
			this.CBoxGridLayout.Text = "Grid";
			this.CBoxGridLayout.UseVisualStyleBackColor = true;
			this.CBoxGridLayout.CheckedChanged += new System.EventHandler(this.CBoxGridLayoutCheckedChanged);
			// 
			// CBoxShiftTransp
			// 
			this.CBoxShiftTransp.Location = new System.Drawing.Point(140, 409);
			this.CBoxShiftTransp.Name = "CBoxShiftTransp";
			this.CBoxShiftTransp.Size = new System.Drawing.Size(85, 17);
			this.CBoxShiftTransp.TabIndex = 25;
			this.CBoxShiftTransp.Text = "Shift Transp";
			this.CBoxShiftTransp.UseVisualStyleBackColor = true;
			this.CBoxShiftTransp.CheckedChanged += new System.EventHandler(this.CBoxAxisLayoutCheckedChanged);
			// 
			// SpriteLayoutLabel
			// 
			this.SpriteLayoutLabel.Location = new System.Drawing.Point(10, 344);
			this.SpriteLayoutLabel.Name = "SpriteLayoutLabel";
			this.SpriteLayoutLabel.Size = new System.Drawing.Size(256, 18);
			this.SpriteLayoutLabel.TabIndex = 6;
			this.SpriteLayoutLabel.Text = "...";
			this.SpriteLayoutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.CHRBank);
			this.groupBox3.Controls.Add(this.BtnDeleteLastCHR);
			this.groupBox3.Controls.Add(this.BtnCHRRotate);
			this.groupBox3.Controls.Add(this.BtnCHRHFlip);
			this.groupBox3.Controls.Add(this.BtnAddCHR);
			this.groupBox3.Controls.Add(this.BtnCHRVFlip);
			this.groupBox3.Controls.Add(this.CHRBankLabel);
			this.groupBox3.Location = new System.Drawing.Point(487, 28);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(276, 330);
			this.groupBox3.TabIndex = 9;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "CHR Bank:";
			// 
			// BtnDeleteLastCHR
			// 
			this.BtnDeleteLastCHR.Location = new System.Drawing.Point(41, 299);
			this.BtnDeleteLastCHR.Name = "BtnDeleteLastCHR";
			this.BtnDeleteLastCHR.Size = new System.Drawing.Size(23, 23);
			this.BtnDeleteLastCHR.TabIndex = 32;
			this.BtnDeleteLastCHR.Text = "-";
			this.BtnDeleteLastCHR.UseVisualStyleBackColor = true;
			this.BtnDeleteLastCHR.Click += new System.EventHandler(this.BtnDeleteLastCHRClick);
			// 
			// BtnCHRRotate
			// 
			this.BtnCHRRotate.Location = new System.Drawing.Point(216, 299);
			this.BtnCHRRotate.Name = "BtnCHRRotate";
			this.BtnCHRRotate.Size = new System.Drawing.Size(50, 23);
			this.BtnCHRRotate.TabIndex = 35;
			this.BtnCHRRotate.Text = "Rotate";
			this.BtnCHRRotate.UseVisualStyleBackColor = true;
			this.BtnCHRRotate.Click += new System.EventHandler(this.BtnCHREditorRotateClick);
			// 
			// BtnCHRHFlip
			// 
			this.BtnCHRHFlip.Location = new System.Drawing.Point(171, 299);
			this.BtnCHRHFlip.Name = "BtnCHRHFlip";
			this.BtnCHRHFlip.Size = new System.Drawing.Size(39, 23);
			this.BtnCHRHFlip.TabIndex = 34;
			this.BtnCHRHFlip.Text = "HFlip";
			this.BtnCHRHFlip.UseVisualStyleBackColor = true;
			this.BtnCHRHFlip.Click += new System.EventHandler(this.BtnCHREditorHFlipClick);
			// 
			// BtnAddCHR
			// 
			this.BtnAddCHR.Location = new System.Drawing.Point(11, 299);
			this.BtnAddCHR.Name = "BtnAddCHR";
			this.BtnAddCHR.Size = new System.Drawing.Size(23, 23);
			this.BtnAddCHR.TabIndex = 31;
			this.BtnAddCHR.Text = "+";
			this.BtnAddCHR.UseVisualStyleBackColor = true;
			this.BtnAddCHR.Click += new System.EventHandler(this.BtnAddCHRClick);
			// 
			// BtnCHRVFlip
			// 
			this.BtnCHRVFlip.Location = new System.Drawing.Point(126, 299);
			this.BtnCHRVFlip.Name = "BtnCHRVFlip";
			this.BtnCHRVFlip.Size = new System.Drawing.Size(39, 23);
			this.BtnCHRVFlip.TabIndex = 33;
			this.BtnCHRVFlip.Text = "VFlip";
			this.BtnCHRVFlip.UseVisualStyleBackColor = true;
			this.BtnCHRVFlip.Click += new System.EventHandler(this.BtnCHREditorVFlipClick);
			// 
			// CHRBankLabel
			// 
			this.CHRBankLabel.Location = new System.Drawing.Point(10, 274);
			this.CHRBankLabel.Name = "CHRBankLabel";
			this.CHRBankLabel.Size = new System.Drawing.Size(256, 18);
			this.CHRBankLabel.TabIndex = 6;
			this.CHRBankLabel.Text = "...";
			this.CHRBankLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.CBoxPalettes);
			this.groupBox4.Controls.Add(this.Palette3);
			this.groupBox4.Controls.Add(this.BtnSwapColors);
			this.groupBox4.Controls.Add(this.Palette2);
			this.groupBox4.Controls.Add(this.Palette1);
			this.groupBox4.Controls.Add(this.Palette0);
			this.groupBox4.Controls.Add(this.PaletteMain);
			this.groupBox4.Controls.Add(this.Palette3Label);
			this.groupBox4.Controls.Add(this.Palette1Label);
			this.groupBox4.Controls.Add(this.Palette2Label);
			this.groupBox4.Controls.Add(this.Palette0Label);
			this.groupBox4.Location = new System.Drawing.Point(487, 364);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(276, 155);
			this.groupBox4.TabIndex = 10;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Palettes:";
			// 
			// CBoxPalettes
			// 
			this.CBoxPalettes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.CBoxPalettes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxPalettes.FormattingEnabled = true;
			this.CBoxPalettes.Location = new System.Drawing.Point(216, 47);
			this.CBoxPalettes.MaxDropDownItems = 16;
			this.CBoxPalettes.Name = "CBoxPalettes";
			this.CBoxPalettes.Size = new System.Drawing.Size(50, 21);
			this.CBoxPalettes.TabIndex = 36;
			this.CBoxPalettes.DropDown += new System.EventHandler(this.CBoxPalettesAdjustWidthDropDown);
			this.CBoxPalettes.SelectedIndexChanged += new System.EventHandler(this.CBoxPalettesChanged);
			// 
			// Palette3
			// 
			this.Palette3.BackColor = System.Drawing.SystemColors.WindowText;
			this.Palette3.ContextMenuStrip = this.ContextMenuSmallPalette;
			this.Palette3.Location = new System.Drawing.Point(170, 48);
			this.Palette3.Name = "Palette3";
			this.Palette3.Size = new System.Drawing.Size(80, 20);
			this.Palette3.TabIndex = 7;
			this.Palette3.TabStop = false;
			// 
			// BtnSwapColors
			// 
			this.BtnSwapColors.Location = new System.Drawing.Point(216, 20);
			this.BtnSwapColors.Name = "BtnSwapColors";
			this.BtnSwapColors.Size = new System.Drawing.Size(50, 23);
			this.BtnSwapColors.TabIndex = 35;
			this.BtnSwapColors.Text = "Swap";
			this.BtnSwapColors.UseVisualStyleBackColor = true;
			this.BtnSwapColors.Click += new System.EventHandler(this.BtnSwapColorsClick);
			// 
			// Palette2
			// 
			this.Palette2.BackColor = System.Drawing.SystemColors.WindowText;
			this.Palette2.ContextMenuStrip = this.ContextMenuSmallPalette;
			this.Palette2.Location = new System.Drawing.Point(41, 48);
			this.Palette2.Name = "Palette2";
			this.Palette2.Size = new System.Drawing.Size(80, 20);
			this.Palette2.TabIndex = 7;
			this.Palette2.TabStop = false;
			// 
			// Palette1
			// 
			this.Palette1.BackColor = System.Drawing.SystemColors.WindowText;
			this.Palette1.ContextMenuStrip = this.ContextMenuSmallPalette;
			this.Palette1.Location = new System.Drawing.Point(170, 22);
			this.Palette1.Name = "Palette1";
			this.Palette1.Size = new System.Drawing.Size(80, 20);
			this.Palette1.TabIndex = 7;
			this.Palette1.TabStop = false;
			// 
			// Palette0
			// 
			this.Palette0.BackColor = System.Drawing.SystemColors.WindowText;
			this.Palette0.ContextMenuStrip = this.ContextMenuSmallPalette;
			this.Palette0.Location = new System.Drawing.Point(41, 22);
			this.Palette0.Name = "Palette0";
			this.Palette0.Size = new System.Drawing.Size(80, 20);
			this.Palette0.TabIndex = 7;
			this.Palette0.TabStop = false;
			// 
			// Palette3Label
			// 
			this.Palette3Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Palette3Label.Location = new System.Drawing.Point(152, 48);
			this.Palette3Label.Name = "Palette3Label";
			this.Palette3Label.Size = new System.Drawing.Size(26, 20);
			this.Palette3Label.TabIndex = 8;
			this.Palette3Label.Text = "4:";
			// 
			// Palette1Label
			// 
			this.Palette1Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Palette1Label.Location = new System.Drawing.Point(152, 23);
			this.Palette1Label.Name = "Palette1Label";
			this.Palette1Label.Size = new System.Drawing.Size(26, 20);
			this.Palette1Label.TabIndex = 8;
			this.Palette1Label.Text = "2:";
			// 
			// Palette2Label
			// 
			this.Palette2Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Palette2Label.Location = new System.Drawing.Point(21, 48);
			this.Palette2Label.Name = "Palette2Label";
			this.Palette2Label.Size = new System.Drawing.Size(26, 20);
			this.Palette2Label.TabIndex = 8;
			this.Palette2Label.Text = "3:";
			// 
			// Palette0Label
			// 
			this.Palette0Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Palette0Label.Location = new System.Drawing.Point(21, 23);
			this.Palette0Label.Name = "Palette0Label";
			this.Palette0Label.Size = new System.Drawing.Size(26, 20);
			this.Palette0Label.TabIndex = 8;
			this.Palette0Label.Text = "1:";
			// 
			// ExportASMSaveFileDialog
			// 
			this.ExportASMSaveFileDialog.DefaultExt = "asm";
			this.ExportASMSaveFileDialog.Filter = "CA65\\NESasm (*.asm)|*.asm";
			this.ExportASMSaveFileDialog.Title = "Export ASM: Select File";
			this.ExportASMSaveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ExportASMOK);
			// 
			// ExportCSaveFileDialog
			// 
			this.ExportCSaveFileDialog.DefaultExt = "h";
			this.ExportCSaveFileDialog.Filter = "C (*.h)|*.h";
			this.ExportCSaveFileDialog.Title = "Export C: Select File";
			this.ExportCSaveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ExportCOK);
			// 
			// ProjectSaveFileDialog
			// 
			this.ProjectSaveFileDialog.DefaultExt = "sprednes";
			this.ProjectSaveFileDialog.Filter = "SPReD-NES (*.sprednes)|*.sprednes";
			this.ProjectSaveFileDialog.Title = "Save Project";
			this.ProjectSaveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ProjectSaveOK);
			// 
			// ProjectOpenFileDialog
			// 
			this.ProjectOpenFileDialog.DefaultExt = "sprednes";
			this.ProjectOpenFileDialog.Filter = "SPReD-NES (*.sprednes)|*.sprednes";
			this.ProjectOpenFileDialog.Title = "Load Project";
			this.ProjectOpenFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ProjectLoadOK);
			// 
			// SpriteListContextMenu
			// 
			this.SpriteListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.createToolStripMenuItem,
									this.deleteToolStripMenuItem,
									this.toolStripSeparator14,
									this.renameToolStripMenuItem,
									this.toolStripSeparator18,
									this.addPrefixPostfixToolStripMenuItem1,
									this.removePrefixPostfixToolStripMenuItem,
									this.toolStripSeparator4,
									this.toolStripMenuItem2,
									this.createRefToolStripMenuItem});
			this.SpriteListContextMenu.Name = "SpriteListContextMenu";
			this.SpriteListContextMenu.Size = new System.Drawing.Size(190, 176);
			// 
			// createToolStripMenuItem
			// 
			this.createToolStripMenuItem.Name = "createToolStripMenuItem";
			this.createToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.createToolStripMenuItem.Text = "Create New";
			this.createToolStripMenuItem.Click += new System.EventHandler(this.BtnCreateClick);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.BtnDeleteClick);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			this.toolStripSeparator14.Size = new System.Drawing.Size(186, 6);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.BtnRenameClick);
			// 
			// toolStripSeparator18
			// 
			this.toolStripSeparator18.Name = "toolStripSeparator18";
			this.toolStripSeparator18.Size = new System.Drawing.Size(186, 6);
			// 
			// addPrefixPostfixToolStripMenuItem1
			// 
			this.addPrefixPostfixToolStripMenuItem1.Name = "addPrefixPostfixToolStripMenuItem1";
			this.addPrefixPostfixToolStripMenuItem1.Size = new System.Drawing.Size(189, 22);
			this.addPrefixPostfixToolStripMenuItem1.Text = "Add Prefix/Postfix";
			this.addPrefixPostfixToolStripMenuItem1.Click += new System.EventHandler(this.BtnAddPrefixPostfixClick);
			// 
			// removePrefixPostfixToolStripMenuItem
			// 
			this.removePrefixPostfixToolStripMenuItem.Name = "removePrefixPostfixToolStripMenuItem";
			this.removePrefixPostfixToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.removePrefixPostfixToolStripMenuItem.Text = "Remove Prefix/Postfix";
			this.removePrefixPostfixToolStripMenuItem.Click += new System.EventHandler(this.BtnRemovePrefixPostfixClick);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(186, 6);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(189, 22);
			this.toolStripMenuItem2.Text = "Create Copy";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.BtnCreateCopyClick);
			// 
			// createRefToolStripMenuItem
			// 
			this.createRefToolStripMenuItem.Name = "createRefToolStripMenuItem";
			this.createRefToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
			this.createRefToolStripMenuItem.Text = "Create Ref";
			this.createRefToolStripMenuItem.Click += new System.EventHandler(this.BtnCreateRefClick);
			// 
			// ExportImagesFolderBrowserDialog
			// 
			this.ExportImagesFolderBrowserDialog.Description = "Export Images: Select Folder";
			this.ExportImagesFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.ExportImagesFolderBrowserDialog.Tag = "";
			// 
			// ContextMenuCHRBank
			// 
			this.ContextMenuCHRBank.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.CopyCHRToolStripMenuItem,
									this.PasteCHRToolStripMenuItem,
									this.separatorToolStripMenuItem2,
									this.FillWithColorToolStripMenuItem});
			this.ContextMenuCHRBank.Name = "contextMenuCHRBank";
			this.ContextMenuCHRBank.Size = new System.Drawing.Size(150, 76);
			// 
			// CopyCHRToolStripMenuItem
			// 
			this.CopyCHRToolStripMenuItem.Name = "CopyCHRToolStripMenuItem";
			this.CopyCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.CopyCHRToolStripMenuItem.Text = "Copy";
			this.CopyCHRToolStripMenuItem.Click += new System.EventHandler(this.CopyCHRToolStripMenuItemClick);
			// 
			// PasteCHRToolStripMenuItem
			// 
			this.PasteCHRToolStripMenuItem.Name = "PasteCHRToolStripMenuItem";
			this.PasteCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.PasteCHRToolStripMenuItem.Text = "Paste";
			this.PasteCHRToolStripMenuItem.Click += new System.EventHandler(this.PasteCHRToolStripMenuItemClick);
			// 
			// separatorToolStripMenuItem2
			// 
			this.separatorToolStripMenuItem2.Name = "separatorToolStripMenuItem2";
			this.separatorToolStripMenuItem2.Size = new System.Drawing.Size(146, 6);
			// 
			// FillWithColorToolStripMenuItem
			// 
			this.FillWithColorToolStripMenuItem.Name = "FillWithColorToolStripMenuItem";
			this.FillWithColorToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.FillWithColorToolStripMenuItem.Text = "Fill With Color";
			this.FillWithColorToolStripMenuItem.Click += new System.EventHandler(this.FillWithColorToolStripMenuItemClick);
			// 
			// ContextMenuSmallPalette
			// 
			this.ContextMenuSmallPalette.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyToolStripMenuItem1,
									this.pasteToolStripMenuItem1});
			this.ContextMenuSmallPalette.Name = "ContextMenuSmallPalette";
			this.ContextMenuSmallPalette.Size = new System.Drawing.Size(103, 48);
			// 
			// copyToolStripMenuItem1
			// 
			this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
			this.copyToolStripMenuItem1.Size = new System.Drawing.Size(102, 22);
			this.copyToolStripMenuItem1.Text = "Copy";
			// 
			// pasteToolStripMenuItem1
			// 
			this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
			this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(102, 22);
			this.pasteToolStripMenuItem1.Text = "Paste";
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.Silver;
			this.ClientSize = new System.Drawing.Size(772, 528);
			this.Controls.Add(this.MenuStrip);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.LayoutGroupBox);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox4);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = global::SPReD.Properties.Resources.SPReD_icon;
			this.KeyPreview = true;
			this.MainMenuStrip = this.MenuStrip;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "SPReD";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
			this.MenuStrip.ResumeLayout(false);
			this.MenuStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SpriteLayout)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CHRBank)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PaletteMain)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.OffsetY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.OffsetX)).EndInit();
			this.groupBox7.ResumeLayout(false);
			this.LayoutGroupBox.ResumeLayout(false);
			this.GroupBoxModeName.ResumeLayout(false);
			this.CHRFlippingGroupBox.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Palette3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette0)).EndInit();
			this.SpriteListContextMenu.ResumeLayout(false);
			this.ContextMenuCHRBank.ResumeLayout(false);
			this.ContextMenuSmallPalette.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip ContextMenuSmallPalette;
		private System.Windows.Forms.ToolStripMenuItem removePrefixPostfixToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
		private System.Windows.Forms.ToolStripMenuItem removePrefixPostfixToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
		private System.Windows.Forms.ToolStripMenuItem managerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem swapToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
		private System.Windows.Forms.Button BtnSwapColors;
		private System.Windows.Forms.ComboBox CBoxPalettes;
		private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem quickGuideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportScriptEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CHRDataToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
		private System.Windows.Forms.ToolStripMenuItem descriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
		private System.Windows.Forms.ToolStripMenuItem fillWithColorToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem FillWithColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem PasteCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CopyCHRToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuCHRBank;
		private System.Windows.Forms.CheckBox CBoxMode8x16;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
		private System.Windows.Forms.ToolStripMenuItem addPrefixPostfixToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem addPrefixPostfixToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipHorizontalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipVerticalToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem deleteCHRToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem addCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cHRBankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shiftColorsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem centeringToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripMenuItem horizontalFlippingToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem verticalFlippingToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem drawModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildModeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem layoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CHROptimizationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CHRPackToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CHRSplitToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem horizontalFlippingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticalFlippingToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem applyPaletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.FolderBrowserDialog ExportImagesFolderBrowserDialog;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createRefToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem createCopyToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem createNewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem spriteToolStripMenuItem;
		private System.Windows.Forms.Button BtnCentering;
		private System.Windows.Forms.Button BtnZoomIn;
		private System.Windows.Forms.Button BtnZoomOut;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button BtnLayoutModeDraw;
		private System.Windows.Forms.Button BtnLayoutModeBuild;
		private System.Windows.Forms.GroupBox GroupBoxModeName;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.CheckBox CBoxShiftTransp;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createRefToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip SpriteListContextMenu;
		private System.Windows.Forms.Button BtnCHROptimization;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog ProjectOpenFileDialog;
		private System.Windows.Forms.SaveFileDialog ProjectSaveFileDialog;
		private System.Windows.Forms.Button BtnDeleteCHR;
		private System.Windows.Forms.CheckBox CBoxSnapLayout;
		private System.Windows.Forms.Button BtnCHRVFlip;
		private System.Windows.Forms.Button BtnCHRHFlip;
		private System.Windows.Forms.Button BtnCHRRotate;
		private System.Windows.Forms.Button BtnAddCHR;
		private System.Windows.Forms.Button BtnDeleteLastCHR;
		private System.Windows.Forms.ComboBox CBoxFlipType;
		private System.Windows.Forms.GroupBox CHRFlippingGroupBox;
		private System.Windows.Forms.Button BtnCHRSplit;
		private System.Windows.Forms.ComboBox CBoxCHRPackingType;
		private System.Windows.Forms.Button BtnCHRPack;
		private System.Windows.Forms.Button BtnShiftColors;
		private System.Windows.Forms.SaveFileDialog ExportASMSaveFileDialog;
		private System.Windows.Forms.SaveFileDialog ExportCSaveFileDialog;
		private System.Windows.Forms.Button BtnSpriteVFlip;
		private System.Windows.Forms.Button BtnSpriteHFlip;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ExportImagesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.CheckBox CBoxAxesLayout;
		private System.Windows.Forms.CheckBox CBoxGridLayout;
		private System.Windows.Forms.Label SpriteLayoutLabel;
		private System.Windows.Forms.Button BtnMoveItemDown;
		private System.Windows.Forms.Button BtnMoveItemUp;
		private System.Windows.Forms.Button BtnSelectAll;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button BtnOffset;
		private System.Windows.Forms.NumericUpDown OffsetX;
		private System.Windows.Forms.NumericUpDown OffsetY;
		private System.Windows.Forms.Button BtnVFlip;
		private System.Windows.Forms.Button BtnHFlip;
		private System.Windows.Forms.Button BtnApplyDefaultPalette;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.Label CHRBankLabel;
		private System.Windows.Forms.Label Palette0Label;
		private System.Windows.Forms.Label Palette2Label;
		private System.Windows.Forms.Label Palette1Label;
		private System.Windows.Forms.Label Palette3Label;
		private System.Windows.Forms.PictureBox Palette0;
		private System.Windows.Forms.PictureBox Palette1;
		private System.Windows.Forms.PictureBox Palette2;
		private System.Windows.Forms.PictureBox Palette3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox LayoutGroupBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.PictureBox PaletteMain;
		
		private System.Windows.Forms.OpenFileDialog ImportOpenFileDialog;
		private System.Windows.Forms.PictureBox CHRBank;
		private System.Windows.Forms.PictureBox SpriteLayout;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExportASMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExportCToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip MenuStrip;
		private System.Windows.Forms.ListBox SpriteList;
	}
}
