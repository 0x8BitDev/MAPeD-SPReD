/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 01.05.2017
 * Time: 15:24
 */
using System;

namespace MAPeD
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
			this.ContextMenuEntitiesTreeGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportScriptEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.descriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cHRBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			this.reorderBanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CHRBankPageBtnsToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.prevPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nextPageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.cHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipHOrizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateGFXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
			this.BlockEditorModeSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BlockEditorModeDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.propertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PropertyPerBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PropertyPerCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.verticalFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rotateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparatorShiftTransp = new System.Windows.Forms.ToolStripSeparator();
			this.shiftTransparencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shiftColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
			this.reserveCHRsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ZXToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.ZXSwapInkPaperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ZXInvertImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optimizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
			this.TilesLockEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reserveBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.builderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowTargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowCoordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.LayoutDeleteAllScreenMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.screensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ScreensAutoUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ScreensShowAllBanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.entitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addEntityToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.entitiesGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addEntityGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteEntityGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.editInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteAllEntitiesInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.patternsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addPatternToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deletePatternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.groupPatternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addPatternGroupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deletePatternGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quickGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator27 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControlTilesLayout = new System.Windows.Forms.TabControl();
			this.TabTiles = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.CBoxTileViewType = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.BtnOptimization = new System.Windows.Forms.Button();
			this.PanelTiles = new System.Windows.Forms.FlowLayoutPanel();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.GrpBoxPalettes = new System.Windows.Forms.GroupBox();
			this.BtnSwapColors = new System.Windows.Forms.Button();
			this.CBoxPalettes = new System.Windows.Forms.ComboBox();
			this.CheckBoxPalettePerCHR = new System.Windows.Forms.CheckBox();
			this.Palette3 = new System.Windows.Forms.PictureBox();
			this.Palette2 = new System.Windows.Forms.PictureBox();
			this.Palette1 = new System.Windows.Forms.PictureBox();
			this.BtnPltDelete = new System.Windows.Forms.Button();
			this.Palette0 = new System.Windows.Forms.PictureBox();
			this.PaletteMain = new System.Windows.Forms.PictureBox();
			this.label4 = new System.Windows.Forms.Label();
			this.BtnPltCopy = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.LabelPalette12 = new System.Windows.Forms.Label();
			this.LabelPalette34 = new System.Windows.Forms.Label();
			this.GrpBoxBlockEditor = new System.Windows.Forms.GroupBox();
			this.PBoxBlockEditor = new System.Windows.Forms.PictureBox();
			this.ContextMenuBlockEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.blockEditModesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.CHRSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.DrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			this.propertyIdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
			this.PropIdPerBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PropIdPerCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.CBoxBlockObjId = new System.Windows.Forms.ComboBox();
			this.BtnBlockReserveCHRs = new System.Windows.Forms.Button();
			this.BtnBlockRotate = new System.Windows.Forms.Button();
			this.LabelObjId = new System.Windows.Forms.Label();
			this.BtnInvInk = new System.Windows.Forms.Button();
			this.BtnSwapInkPaper = new System.Windows.Forms.Button();
			this.BtnEditModeDraw = new System.Windows.Forms.Button();
			this.BtnUpdateGFX = new System.Windows.Forms.Button();
			this.BtnEditModeSelectCHR = new System.Windows.Forms.Button();
			this.BtnBlockHFlip = new System.Windows.Forms.Button();
			this.BtnBlockVFlip = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.GrpBoxTileEditor = new System.Windows.Forms.GroupBox();
			this.BtnTileReserveBlocks = new System.Windows.Forms.Button();
			this.CheckBoxTileEditorLock = new System.Windows.Forms.CheckBox();
			this.PBoxTilePreview = new System.Windows.Forms.PictureBox();
			this.GrpBoxScreenData = new System.Windows.Forms.GroupBox();
			this.LabelScreenResolution = new System.Windows.Forms.Label();
			this.NumericUpDownScrBlocksHeight = new System.Windows.Forms.NumericUpDown();
			this.NumericUpDownScrBlocksWidth = new System.Windows.Forms.NumericUpDown();
			this.BtnScreenDataInfo = new System.Windows.Forms.Button();
			this.RBtnScreenDataBlocks = new System.Windows.Forms.RadioButton();
			this.RBtnScreenDataTiles = new System.Windows.Forms.RadioButton();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.GrpBoxCHRBank = new System.Windows.Forms.GroupBox();
			this.BtnCHRBankNextPage = new System.Windows.Forms.Button();
			this.BtnCHRBankPrevPage = new System.Windows.Forms.Button();
			this.CBoxCHRBanks = new System.Windows.Forms.ComboBox();
			this.BtnDeleteCHRBank = new System.Windows.Forms.Button();
			this.BtnCHRRotate = new System.Windows.Forms.Button();
			this.BtnCHRHFlip = new System.Windows.Forms.Button();
			this.BtnCopyCHRBank = new System.Windows.Forms.Button();
			this.BtnAddCHRBank = new System.Windows.Forms.Button();
			this.BtnCHRVFlip = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.PBoxCHRBank = new System.Windows.Forms.PictureBox();
			this.ContextMenuCHRBank = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.fillWithColorCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
			this.insertLeftCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.PanelBlocks = new System.Windows.Forms.FlowLayoutPanel();
			this.TabLayout = new System.Windows.Forms.TabPage();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.tabControlLayoutTools = new System.Windows.Forms.TabControl();
			this.TabBuilder = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.BtnLayoutDeleteEmptyScreens = new System.Windows.Forms.Button();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.BtnLayoutMoveDown = new System.Windows.Forms.Button();
			this.BtnLayoutMoveUp = new System.Windows.Forms.Button();
			this.ListBoxLayouts = new System.Windows.Forms.ListBox();
			this.LayoutLabel = new System.Windows.Forms.Label();
			this.groupBox13 = new System.Windows.Forms.GroupBox();
			this.CheckBoxShowGrid = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowCoords = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowTargets = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowMarks = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowEntities = new System.Windows.Forms.CheckBox();
			this.BtnLayoutRemoveRightColumn = new System.Windows.Forms.Button();
			this.BtnLayoutRemoveUpRow = new System.Windows.Forms.Button();
			this.BtnLayoutAddDownRow = new System.Windows.Forms.Button();
			this.BtnCreateLayoutWxH = new System.Windows.Forms.Button();
			this.BtnLayoutAddRightColumn = new System.Windows.Forms.Button();
			this.BtnCopyLayout = new System.Windows.Forms.Button();
			this.BtnLayoutAddUpRow = new System.Windows.Forms.Button();
			this.BtnDeleteLayout = new System.Windows.Forms.Button();
			this.BtnLayoutRemoveDownRow = new System.Windows.Forms.Button();
			this.BtnLayoutAddLeftColumn = new System.Windows.Forms.Button();
			this.BtnLayoutRemoveLeftColumn = new System.Windows.Forms.Button();
			this.TabPainter = new System.Windows.Forms.TabPage();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.CheckBoxPainterReplaceTiles = new System.Windows.Forms.CheckBox();
			this.BtnPainterFillWithTile = new System.Windows.Forms.Button();
			this.GrpBoxPainter = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.RBtnMapScaleX2 = new System.Windows.Forms.RadioButton();
			this.RBtnMapScaleX1 = new System.Windows.Forms.RadioButton();
			this.GrpBoxActiveTile = new System.Windows.Forms.GroupBox();
			this.PBoxActiveTile = new System.Windows.Forms.PictureBox();
			this.BtnTilesBlocks = new System.Windows.Forms.Button();
			this.BtnResetTile = new System.Windows.Forms.Button();
			this.TabScreenList = new System.Windows.Forms.TabPage();
			this.splitContainer6 = new System.Windows.Forms.SplitContainer();
			this.CheckBoxLayoutEditorAllBanks = new System.Windows.Forms.CheckBox();
			this.BtnUpdateScreens = new System.Windows.Forms.Button();
			this.LabelLayoutEditorCHRBankID = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.CheckBoxScreensAutoUpdate = new System.Windows.Forms.CheckBox();
			this.ListViewScreens = new System.Windows.Forms.ListView();
			this.TabEntities = new System.Windows.Forms.TabPage();
			this.splitContainer7 = new System.Windows.Forms.SplitContainer();
			this.splitContainer8 = new System.Windows.Forms.SplitContainer();
			this.TreeViewEntities = new System.Windows.Forms.TreeView();
			this.ContextMenuEntitiesTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer9 = new System.Windows.Forms.SplitContainer();
			this.BtnEntityRename = new System.Windows.Forms.Button();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.BtnEntityGroupAdd = new System.Windows.Forms.Button();
			this.BtnEntityGroupDelete = new System.Windows.Forms.Button();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.BtnEntityAdd = new System.Windows.Forms.Button();
			this.BtnEntityDelete = new System.Windows.Forms.Button();
			this.BtnEntitiesEditInstancesMode = new System.Windows.Forms.Button();
			this.CheckBoxEntitySnapping = new System.Windows.Forms.CheckBox();
			this.groupBoxEntityEditor = new System.Windows.Forms.GroupBox();
			this.CheckBoxSelectTargetEntity = new System.Windows.Forms.CheckBox();
			this.PBoxEntityPreview = new System.Windows.Forms.PictureBox();
			this.NumericUpDownEntityUID = new System.Windows.Forms.NumericUpDown();
			this.PBoxColor = new System.Windows.Forms.PictureBox();
			this.LabelEntityProperty = new System.Windows.Forms.Label();
			this.LabelEntityInstanceProperty = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox11 = new System.Windows.Forms.GroupBox();
			this.NumericUpDownEntityHeight = new System.Windows.Forms.NumericUpDown();
			this.NumericUpDownEntityWidth = new System.Windows.Forms.NumericUpDown();
			this.groupBox12 = new System.Windows.Forms.GroupBox();
			this.NumericUpDownEntityPivotY = new System.Windows.Forms.NumericUpDown();
			this.NumericUpDownEntityPivotX = new System.Windows.Forms.NumericUpDown();
			this.BtnEntityLoadBitmap = new System.Windows.Forms.Button();
			this.LabelEntityName = new System.Windows.Forms.Label();
			this.TextBoxEntityProperties = new System.Windows.Forms.TextBox();
			this.TextBoxEntityInstanceProp = new System.Windows.Forms.TextBox();
			this.TabPatterns = new System.Windows.Forms.TabPage();
			this.splitContainer10 = new System.Windows.Forms.SplitContainer();
			this.splitContainer11 = new System.Windows.Forms.SplitContainer();
			this.TreeViewPatterns = new System.Windows.Forms.TreeView();
			this.ContextMenuStripPatternsTreeViewGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.BtnPatternRename = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.BtnPatternGroupAdd = new System.Windows.Forms.Button();
			this.BtnPatternGroupDelete = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.CheckBoxPatternAdd = new System.Windows.Forms.CheckBox();
			this.BtnPatternDelete = new System.Windows.Forms.Button();
			this.BtnPatternReset = new System.Windows.Forms.Button();
			this.PBoxPatternPreview = new System.Windows.Forms.PictureBox();
			this.PBoxLayout = new SkiaSharp.Views.Desktop.SKGLControl();
			this.ContextMenuLayoutEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.LayoutEntityOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutBringFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutSendBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator29 = new System.Windows.Forms.ToolStripSeparator();
			this.LayoutDeleteEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutDeleteScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
			this.LayoutDeleteScreenEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.SetStartScreenMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.SetScreenMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.adjacentScreensDirectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskURToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskRDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskUDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLURToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskURDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLRDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLUDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.AdjScrMaskLURDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuTilesList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.clearTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
			this.insertLeftTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteTileToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator28 = new System.Windows.Forms.ToolStripSeparator();
			this.clearAllTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuBlocksList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteBlockCloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteBlockRefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.clearCHRsBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearRefsBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.clearPropertiesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
			this.insertLeftBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.StatusBar = new System.Windows.Forms.StatusStrip();
			this.StatusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.Project_saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.Project_openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.Import_openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.Project_exportFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.ContextMenuEntitiesTreeEntity = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteAllInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colorDialogEntity = new System.Windows.Forms.ColorDialog();
			this.EntityLoadBitmap_openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.ContextMenuStripPatternItem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuStripGroupItem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuEntitiesTreeGroup.SuspendLayout();
			this.MenuStrip.SuspendLayout();
			this.tabControlTilesLayout.SuspendLayout();
			this.TabTiles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.GrpBoxPalettes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Palette3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette0)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PaletteMain)).BeginInit();
			this.GrpBoxBlockEditor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxBlockEditor)).BeginInit();
			this.ContextMenuBlockEditor.SuspendLayout();
			this.GrpBoxTileEditor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxTilePreview)).BeginInit();
			this.GrpBoxScreenData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownScrBlocksHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownScrBlocksWidth)).BeginInit();
			this.GrpBoxCHRBank.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxCHRBank)).BeginInit();
			this.ContextMenuCHRBank.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.TabLayout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.tabControlLayoutTools.SuspendLayout();
			this.TabBuilder.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox13.SuspendLayout();
			this.TabPainter.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.GrpBoxPainter.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.GrpBoxActiveTile.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxActiveTile)).BeginInit();
			this.TabScreenList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
			this.splitContainer6.Panel1.SuspendLayout();
			this.splitContainer6.Panel2.SuspendLayout();
			this.splitContainer6.SuspendLayout();
			this.TabEntities.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
			this.splitContainer7.Panel1.SuspendLayout();
			this.splitContainer7.Panel2.SuspendLayout();
			this.splitContainer7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).BeginInit();
			this.splitContainer8.Panel1.SuspendLayout();
			this.splitContainer8.Panel2.SuspendLayout();
			this.splitContainer8.SuspendLayout();
			this.ContextMenuEntitiesTree.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer9)).BeginInit();
			this.splitContainer9.Panel1.SuspendLayout();
			this.splitContainer9.Panel2.SuspendLayout();
			this.splitContainer9.SuspendLayout();
			this.groupBox9.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.groupBoxEntityEditor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxEntityPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityUID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PBoxColor)).BeginInit();
			this.groupBox11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityWidth)).BeginInit();
			this.groupBox12.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityPivotY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityPivotX)).BeginInit();
			this.TabPatterns.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer10)).BeginInit();
			this.splitContainer10.Panel1.SuspendLayout();
			this.splitContainer10.Panel2.SuspendLayout();
			this.splitContainer10.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer11)).BeginInit();
			this.splitContainer11.Panel1.SuspendLayout();
			this.splitContainer11.Panel2.SuspendLayout();
			this.splitContainer11.SuspendLayout();
			this.ContextMenuStripPatternsTreeViewGroup.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxPatternPreview)).BeginInit();
			this.ContextMenuLayoutEditor.SuspendLayout();
			this.ContextMenuTilesList.SuspendLayout();
			this.ContextMenuBlocksList.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.ContextMenuEntitiesTreeEntity.SuspendLayout();
			this.ContextMenuStripPatternItem.SuspendLayout();
			this.ContextMenuStripGroupItem.SuspendLayout();
			this.SuspendLayout();
			// 
			// ContextMenuEntitiesTreeGroup
			// 
			this.ContextMenuEntitiesTreeGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addEntityToolStripMenuItem,
									this.toolStripSeparator2,
									this.deleteGroupToolStripMenuItem,
									this.renameGroupToolStripMenuItem});
			this.ContextMenuEntitiesTreeGroup.Name = "ContextMenuStripEntitiesTreeGoup";
			this.ContextMenuEntitiesTreeGroup.Size = new System.Drawing.Size(130, 76);
			// 
			// addEntityToolStripMenuItem
			// 
			this.addEntityToolStripMenuItem.Name = "addEntityToolStripMenuItem";
			this.addEntityToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.addEntityToolStripMenuItem.Text = "&Add Entity";
			this.addEntityToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityAddClick_Event);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(126, 6);
			// 
			// deleteGroupToolStripMenuItem
			// 
			this.deleteGroupToolStripMenuItem.Name = "deleteGroupToolStripMenuItem";
			this.deleteGroupToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.deleteGroupToolStripMenuItem.Text = "&Delete";
			this.deleteGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupDeleteClick_Event);
			// 
			// renameGroupToolStripMenuItem
			// 
			this.renameGroupToolStripMenuItem.Name = "renameGroupToolStripMenuItem";
			this.renameGroupToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.renameGroupToolStripMenuItem.Text = "Re&name";
			this.renameGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupRenameClick_Event);
			// 
			// MenuStrip
			// 
			this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem,
									this.cHRBankToolStripMenuItem,
									this.blocksToolStripMenuItem,
									this.tilesToolStripMenuItem,
									this.builderToolStripMenuItem,
									this.screensToolStripMenuItem,
									this.entitiesToolStripMenuItem,
									this.patternsToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.MenuStrip.Location = new System.Drawing.Point(0, 0);
			this.MenuStrip.Name = "MenuStrip";
			this.MenuStrip.Size = new System.Drawing.Size(1244, 24);
			this.MenuStrip.TabIndex = 0;
			this.MenuStrip.Text = "MenuStrip";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.loadToolStripMenuItem,
									this.saveToolStripMenuItem,
									this.closeToolStripMenuItem,
									this.separatorToolStripMenuItem1,
									this.importToolStripMenuItem,
									this.exportToolStripMenuItem,
									this.exportScriptEditorToolStripMenuItem,
									this.toolStripSeparator1,
									this.descriptionToolStripMenuItem,
									this.statisticsToolStripMenuItem,
									this.toolStripSeparator3,
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
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick_Event);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick_Event);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItemClick_Event);
			// 
			// separatorToolStripMenuItem1
			// 
			this.separatorToolStripMenuItem1.Name = "separatorToolStripMenuItem1";
			this.separatorToolStripMenuItem1.Size = new System.Drawing.Size(208, 6);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.importToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.importToolStripMenuItem.Text = "&Import";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItemClick_Event);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItemClick_Event);
			// 
			// exportScriptEditorToolStripMenuItem
			// 
			this.exportScriptEditorToolStripMenuItem.Name = "exportScriptEditorToolStripMenuItem";
			this.exportScriptEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.X)));
			this.exportScriptEditorToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.exportScriptEditorToolStripMenuItem.Text = "Expo&rt Script Editor";
			this.exportScriptEditorToolStripMenuItem.Click += new System.EventHandler(this.ExportScriptEditorToolStripMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(208, 6);
			// 
			// descriptionToolStripMenuItem
			// 
			this.descriptionToolStripMenuItem.Name = "descriptionToolStripMenuItem";
			this.descriptionToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.descriptionToolStripMenuItem.Text = "&Description";
			this.descriptionToolStripMenuItem.Click += new System.EventHandler(this.DescriptionToolStripMenuItemClick_Event);
			// 
			// statisticsToolStripMenuItem
			// 
			this.statisticsToolStripMenuItem.Name = "statisticsToolStripMenuItem";
			this.statisticsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.statisticsToolStripMenuItem.Text = "S&tatistics";
			this.statisticsToolStripMenuItem.Click += new System.EventHandler(this.StatisticsToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(208, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick_Event);
			// 
			// cHRBankToolStripMenuItem
			// 
			this.cHRBankToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addBankToolStripMenuItem,
									this.deleteBankToolStripMenuItem,
									this.copyBankToolStripMenuItem,
									this.toolStripSeparator18,
									this.reorderBanksToolStripMenuItem,
									this.CHRBankPageBtnsToolStripSeparator,
									this.prevPageToolStripMenuItem,
									this.nextPageToolStripMenuItem,
									this.toolStripSeparator8,
									this.cHRToolStripMenuItem});
			this.cHRBankToolStripMenuItem.Name = "cHRBankToolStripMenuItem";
			this.cHRBankToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
			this.cHRBankToolStripMenuItem.Text = "&CHR Bank";
			// 
			// addBankToolStripMenuItem
			// 
			this.addBankToolStripMenuItem.Name = "addBankToolStripMenuItem";
			this.addBankToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.addBankToolStripMenuItem.Text = "&Add Bank";
			this.addBankToolStripMenuItem.Click += new System.EventHandler(this.BtnAddCHRBankClick_Event);
			// 
			// deleteBankToolStripMenuItem
			// 
			this.deleteBankToolStripMenuItem.Name = "deleteBankToolStripMenuItem";
			this.deleteBankToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.deleteBankToolStripMenuItem.Text = "&Delete Bank";
			this.deleteBankToolStripMenuItem.Click += new System.EventHandler(this.BtnDeleteCHRBankClick_Event);
			// 
			// copyBankToolStripMenuItem
			// 
			this.copyBankToolStripMenuItem.Name = "copyBankToolStripMenuItem";
			this.copyBankToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.copyBankToolStripMenuItem.Text = "&Copy Bank";
			this.copyBankToolStripMenuItem.Click += new System.EventHandler(this.BtnCopyCHRBankClick_Event);
			// 
			// toolStripSeparator18
			// 
			this.toolStripSeparator18.Name = "toolStripSeparator18";
			this.toolStripSeparator18.Size = new System.Drawing.Size(187, 6);
			// 
			// reorderBanksToolStripMenuItem
			// 
			this.reorderBanksToolStripMenuItem.Name = "reorderBanksToolStripMenuItem";
			this.reorderBanksToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.reorderBanksToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.reorderBanksToolStripMenuItem.Text = "&Reorder Banks";
			this.reorderBanksToolStripMenuItem.Click += new System.EventHandler(this.BtnReorderCHRBanksClick_Event);
			// 
			// CHRBankPageBtnsToolStripSeparator
			// 
			this.CHRBankPageBtnsToolStripSeparator.Name = "CHRBankPageBtnsToolStripSeparator";
			this.CHRBankPageBtnsToolStripSeparator.Size = new System.Drawing.Size(187, 6);
			// 
			// prevPageToolStripMenuItem
			// 
			this.prevPageToolStripMenuItem.Name = "prevPageToolStripMenuItem";
			this.prevPageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
			this.prevPageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.prevPageToolStripMenuItem.Text = "&Prev Page";
			this.prevPageToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRBankPrevPageClick_Event);
			// 
			// nextPageToolStripMenuItem
			// 
			this.nextPageToolStripMenuItem.Name = "nextPageToolStripMenuItem";
			this.nextPageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
			this.nextPageToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.nextPageToolStripMenuItem.Text = "&Next Page";
			this.nextPageToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRBankNextPageClick_Event);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(187, 6);
			// 
			// cHRToolStripMenuItem
			// 
			this.cHRToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.flipVerticalToolStripMenuItem,
									this.flipHOrizontalToolStripMenuItem,
									this.rotateToolStripMenuItem});
			this.cHRToolStripMenuItem.Name = "cHRToolStripMenuItem";
			this.cHRToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
			this.cHRToolStripMenuItem.Text = "C&HR";
			// 
			// flipVerticalToolStripMenuItem
			// 
			this.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem";
			this.flipVerticalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.flipVerticalToolStripMenuItem.Text = "Flip &Vertical";
			this.flipVerticalToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRVFlipClick_Event);
			// 
			// flipHOrizontalToolStripMenuItem
			// 
			this.flipHOrizontalToolStripMenuItem.Name = "flipHOrizontalToolStripMenuItem";
			this.flipHOrizontalToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.flipHOrizontalToolStripMenuItem.Text = "Flip &Horizontal";
			this.flipHOrizontalToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRHFlipClick_Event);
			// 
			// rotateToolStripMenuItem
			// 
			this.rotateToolStripMenuItem.Name = "rotateToolStripMenuItem";
			this.rotateToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
			this.rotateToolStripMenuItem.Text = "&Rotate";
			this.rotateToolStripMenuItem.Click += new System.EventHandler(this.BtnCHRRotateClick_Event);
			// 
			// blocksToolStripMenuItem
			// 
			this.blocksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.updateGFXToolStripMenuItem,
									this.toolStripSeparator20,
									this.BlockEditorModeSelectToolStripMenuItem,
									this.BlockEditorModeDrawToolStripMenuItem,
									this.toolStripSeparator11,
									this.propertyToolStripMenuItem,
									this.clearPropertiesToolStripMenuItem,
									this.toolStripSeparator13,
									this.verticalFlipToolStripMenuItem,
									this.horizontalFlipToolStripMenuItem,
									this.rotateToolStripMenuItem1,
									this.toolStripSeparatorShiftTransp,
									this.shiftTransparencyToolStripMenuItem,
									this.shiftColorsToolStripMenuItem,
									this.toolStripSeparator24,
									this.reserveCHRsToolStripMenuItem,
									this.ZXToolStripSeparator,
									this.ZXSwapInkPaperToolStripMenuItem,
									this.ZXInvertImageToolStripMenuItem});
			this.blocksToolStripMenuItem.Name = "blocksToolStripMenuItem";
			this.blocksToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
			this.blocksToolStripMenuItem.Text = "&Blocks";
			// 
			// updateGFXToolStripMenuItem
			// 
			this.updateGFXToolStripMenuItem.Name = "updateGFXToolStripMenuItem";
			this.updateGFXToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
			this.updateGFXToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.updateGFXToolStripMenuItem.Text = "&Update GFX";
			this.updateGFXToolStripMenuItem.Click += new System.EventHandler(this.BtnUpdateGFXClick_Event);
			// 
			// toolStripSeparator20
			// 
			this.toolStripSeparator20.Name = "toolStripSeparator20";
			this.toolStripSeparator20.Size = new System.Drawing.Size(175, 6);
			// 
			// BlockEditorModeSelectToolStripMenuItem
			// 
			this.BlockEditorModeSelectToolStripMenuItem.Name = "BlockEditorModeSelectToolStripMenuItem";
			this.BlockEditorModeSelectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
			this.BlockEditorModeSelectToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.BlockEditorModeSelectToolStripMenuItem.Text = "Mode &Select";
			this.BlockEditorModeSelectToolStripMenuItem.Click += new System.EventHandler(this.SelectCHRToolStripMenuItemClick_Event);
			// 
			// BlockEditorModeDrawToolStripMenuItem
			// 
			this.BlockEditorModeDrawToolStripMenuItem.Name = "BlockEditorModeDrawToolStripMenuItem";
			this.BlockEditorModeDrawToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D)));
			this.BlockEditorModeDrawToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.BlockEditorModeDrawToolStripMenuItem.Text = "Mode &Draw";
			this.BlockEditorModeDrawToolStripMenuItem.Click += new System.EventHandler(this.DrawToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(175, 6);
			// 
			// propertyToolStripMenuItem
			// 
			this.propertyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.PropertyPerBlockToolStripMenuItem,
									this.PropertyPerCHRToolStripMenuItem});
			this.propertyToolStripMenuItem.Name = "propertyToolStripMenuItem";
			this.propertyToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.propertyToolStripMenuItem.Text = "&Property Id per";
			// 
			// PropertyPerBlockToolStripMenuItem
			// 
			this.PropertyPerBlockToolStripMenuItem.Name = "PropertyPerBlockToolStripMenuItem";
			this.PropertyPerBlockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
			this.PropertyPerBlockToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.PropertyPerBlockToolStripMenuItem.Text = "&Block";
			this.PropertyPerBlockToolStripMenuItem.Click += new System.EventHandler(this.PropertyPerBlockToolStripMenuItemClick_Event);
			// 
			// PropertyPerCHRToolStripMenuItem
			// 
			this.PropertyPerCHRToolStripMenuItem.Name = "PropertyPerCHRToolStripMenuItem";
			this.PropertyPerCHRToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
			this.PropertyPerCHRToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
			this.PropertyPerCHRToolStripMenuItem.Text = "&CHR";
			this.PropertyPerCHRToolStripMenuItem.Click += new System.EventHandler(this.PropertyPerCHRToolStripMenuItemClick_Event);
			// 
			// clearPropertiesToolStripMenuItem
			// 
			this.clearPropertiesToolStripMenuItem.Name = "clearPropertiesToolStripMenuItem";
			this.clearPropertiesToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.clearPropertiesToolStripMenuItem.Text = "C&lear Properties";
			this.clearPropertiesToolStripMenuItem.Click += new System.EventHandler(this.clearPropertiesToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator13
			// 
			this.toolStripSeparator13.Name = "toolStripSeparator13";
			this.toolStripSeparator13.Size = new System.Drawing.Size(175, 6);
			// 
			// verticalFlipToolStripMenuItem
			// 
			this.verticalFlipToolStripMenuItem.Name = "verticalFlipToolStripMenuItem";
			this.verticalFlipToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.verticalFlipToolStripMenuItem.Text = "Flip &Vertical";
			this.verticalFlipToolStripMenuItem.Click += new System.EventHandler(this.BtnBlockVFlipClick_Event);
			// 
			// horizontalFlipToolStripMenuItem
			// 
			this.horizontalFlipToolStripMenuItem.Name = "horizontalFlipToolStripMenuItem";
			this.horizontalFlipToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.horizontalFlipToolStripMenuItem.Text = "Flip &Horizontal";
			this.horizontalFlipToolStripMenuItem.Click += new System.EventHandler(this.BtnBlockHFlipClick_Event);
			// 
			// rotateToolStripMenuItem1
			// 
			this.rotateToolStripMenuItem1.Name = "rotateToolStripMenuItem1";
			this.rotateToolStripMenuItem1.Size = new System.Drawing.Size(178, 22);
			this.rotateToolStripMenuItem1.Text = "&Rotate";
			this.rotateToolStripMenuItem1.Click += new System.EventHandler(this.BtnBlockRotateClick_Event);
			// 
			// toolStripSeparatorShiftTransp
			// 
			this.toolStripSeparatorShiftTransp.Name = "toolStripSeparatorShiftTransp";
			this.toolStripSeparatorShiftTransp.Size = new System.Drawing.Size(175, 6);
			// 
			// shiftTransparencyToolStripMenuItem
			// 
			this.shiftTransparencyToolStripMenuItem.Checked = true;
			this.shiftTransparencyToolStripMenuItem.CheckOnClick = true;
			this.shiftTransparencyToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.shiftTransparencyToolStripMenuItem.Name = "shiftTransparencyToolStripMenuItem";
			this.shiftTransparencyToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.shiftTransparencyToolStripMenuItem.Text = "Shift &Transparency";
			// 
			// shiftColorsToolStripMenuItem
			// 
			this.shiftColorsToolStripMenuItem.Name = "shiftColorsToolStripMenuItem";
			this.shiftColorsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
			this.shiftColorsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.shiftColorsToolStripMenuItem.Text = "&Shift Colors";
			this.shiftColorsToolStripMenuItem.Click += new System.EventHandler(this.ShiftColorsToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator24
			// 
			this.toolStripSeparator24.Name = "toolStripSeparator24";
			this.toolStripSeparator24.Size = new System.Drawing.Size(175, 6);
			// 
			// reserveCHRsToolStripMenuItem
			// 
			this.reserveCHRsToolStripMenuItem.Name = "reserveCHRsToolStripMenuItem";
			this.reserveCHRsToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.reserveCHRsToolStripMenuItem.Text = "Reserve &CHRs";
			this.reserveCHRsToolStripMenuItem.Click += new System.EventHandler(this.BtnBlockReserveCHRsClick_Event);
			// 
			// ZXToolStripSeparator
			// 
			this.ZXToolStripSeparator.Name = "ZXToolStripSeparator";
			this.ZXToolStripSeparator.Size = new System.Drawing.Size(175, 6);
			this.ZXToolStripSeparator.Visible = false;
			// 
			// ZXSwapInkPaperToolStripMenuItem
			// 
			this.ZXSwapInkPaperToolStripMenuItem.Name = "ZXSwapInkPaperToolStripMenuItem";
			this.ZXSwapInkPaperToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.ZXSwapInkPaperToolStripMenuItem.Text = "S&wap Ink to Paper";
			this.ZXSwapInkPaperToolStripMenuItem.Visible = false;
			this.ZXSwapInkPaperToolStripMenuItem.Click += new System.EventHandler(this.BtnSwapInkPaperClick_Event);
			// 
			// ZXInvertImageToolStripMenuItem
			// 
			this.ZXInvertImageToolStripMenuItem.Name = "ZXInvertImageToolStripMenuItem";
			this.ZXInvertImageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.I)));
			this.ZXInvertImageToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
			this.ZXInvertImageToolStripMenuItem.Text = "&Invert Image";
			this.ZXInvertImageToolStripMenuItem.Visible = false;
			this.ZXInvertImageToolStripMenuItem.Click += new System.EventHandler(this.BtnInvInkClick_Event);
			// 
			// tilesToolStripMenuItem
			// 
			this.tilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.optimizationToolStripMenuItem,
									this.toolStripSeparator25,
									this.TilesLockEditorToolStripMenuItem,
									this.reserveBlocksToolStripMenuItem});
			this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
			this.tilesToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.tilesToolStripMenuItem.Text = "&Tiles";
			// 
			// optimizationToolStripMenuItem
			// 
			this.optimizationToolStripMenuItem.Name = "optimizationToolStripMenuItem";
			this.optimizationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.optimizationToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.optimizationToolStripMenuItem.Text = "&Optimization";
			this.optimizationToolStripMenuItem.Click += new System.EventHandler(this.BtnOptimizationClick_Event);
			// 
			// toolStripSeparator25
			// 
			this.toolStripSeparator25.Name = "toolStripSeparator25";
			this.toolStripSeparator25.Size = new System.Drawing.Size(183, 6);
			// 
			// TilesLockEditorToolStripMenuItem
			// 
			this.TilesLockEditorToolStripMenuItem.Checked = true;
			this.TilesLockEditorToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.TilesLockEditorToolStripMenuItem.Name = "TilesLockEditorToolStripMenuItem";
			this.TilesLockEditorToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.TilesLockEditorToolStripMenuItem.Text = "&Lock Editor";
			this.TilesLockEditorToolStripMenuItem.Click += new System.EventHandler(this.TilesLockEditorToolStripMenuItemClick_Event);
			// 
			// reserveBlocksToolStripMenuItem
			// 
			this.reserveBlocksToolStripMenuItem.Name = "reserveBlocksToolStripMenuItem";
			this.reserveBlocksToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.reserveBlocksToolStripMenuItem.Text = "Reserve &Blocks";
			this.reserveBlocksToolStripMenuItem.Click += new System.EventHandler(this.BtnTileReserveBlocksClick_Event);
			// 
			// builderToolStripMenuItem
			// 
			this.builderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.createToolStripMenuItem1,
									this.copyToolStripMenuItem,
									this.deleteToolStripMenuItem2,
									this.toolStripSeparator9,
									this.showToolStripMenuItem,
									this.toolStripSeparator10,
									this.LayoutDeleteAllScreenMarksToolStripMenuItem});
			this.builderToolStripMenuItem.Name = "builderToolStripMenuItem";
			this.builderToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.builderToolStripMenuItem.Text = "&Builder";
			// 
			// createToolStripMenuItem1
			// 
			this.createToolStripMenuItem1.Name = "createToolStripMenuItem1";
			this.createToolStripMenuItem1.Size = new System.Drawing.Size(197, 22);
			this.createToolStripMenuItem1.Text = "C&reate WxH";
			this.createToolStripMenuItem1.Click += new System.EventHandler(this.BtnCreateLayoutWxHClick_Event);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.copyToolStripMenuItem.Text = "C&opy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.BtnCopyLayoutClick_Event);
			// 
			// deleteToolStripMenuItem2
			// 
			this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
			this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(197, 22);
			this.deleteToolStripMenuItem2.Text = "&Delete";
			this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.BtnDeleteLayoutClick_Event);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(194, 6);
			// 
			// showToolStripMenuItem
			// 
			this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.LayoutShowMarksToolStripMenuItem,
									this.LayoutShowEntitiesToolStripMenuItem,
									this.LayoutShowTargetsToolStripMenuItem,
									this.LayoutShowCoordsToolStripMenuItem,
									this.LayoutShowGridToolStripMenuItem});
			this.showToolStripMenuItem.Name = "showToolStripMenuItem";
			this.showToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.showToolStripMenuItem.Text = "&Show";
			// 
			// LayoutShowMarksToolStripMenuItem
			// 
			this.LayoutShowMarksToolStripMenuItem.Name = "LayoutShowMarksToolStripMenuItem";
			this.LayoutShowMarksToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.LayoutShowMarksToolStripMenuItem.Text = "&Marks";
			this.LayoutShowMarksToolStripMenuItem.Click += new System.EventHandler(this.LayoutShowMarksToolStripMenuItemClick_Event);
			// 
			// LayoutShowEntitiesToolStripMenuItem
			// 
			this.LayoutShowEntitiesToolStripMenuItem.Name = "LayoutShowEntitiesToolStripMenuItem";
			this.LayoutShowEntitiesToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.LayoutShowEntitiesToolStripMenuItem.Text = "&Entities";
			this.LayoutShowEntitiesToolStripMenuItem.Click += new System.EventHandler(this.LayoutShowEntitiesToolStripMenuItemClick_Event);
			// 
			// LayoutShowTargetsToolStripMenuItem
			// 
			this.LayoutShowTargetsToolStripMenuItem.Name = "LayoutShowTargetsToolStripMenuItem";
			this.LayoutShowTargetsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.LayoutShowTargetsToolStripMenuItem.Text = "&Targets";
			this.LayoutShowTargetsToolStripMenuItem.Click += new System.EventHandler(this.LayoutShowTargetsToolStripMenuItemClick_Event);
			// 
			// LayoutShowCoordsToolStripMenuItem
			// 
			this.LayoutShowCoordsToolStripMenuItem.Name = "LayoutShowCoordsToolStripMenuItem";
			this.LayoutShowCoordsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.LayoutShowCoordsToolStripMenuItem.Text = "&Coordinates";
			this.LayoutShowCoordsToolStripMenuItem.Click += new System.EventHandler(this.LayoutShowCoordsToolStripMenuItemClick_Event);
			// 
			// LayoutShowGridToolStripMenuItem
			// 
			this.LayoutShowGridToolStripMenuItem.Name = "LayoutShowGridToolStripMenuItem";
			this.LayoutShowGridToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
			this.LayoutShowGridToolStripMenuItem.Text = "&Grid";
			this.LayoutShowGridToolStripMenuItem.Click += new System.EventHandler(this.LayoutShowGridToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(194, 6);
			// 
			// LayoutDeleteAllScreenMarksToolStripMenuItem
			// 
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Name = "LayoutDeleteAllScreenMarksToolStripMenuItem";
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Text = "Delete All Screen &Marks";
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteAllScreenMarksToolStripMenuItemClick_Event);
			// 
			// screensToolStripMenuItem
			// 
			this.screensToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.updateToolStripMenuItem,
									this.ScreensAutoUpdateToolStripMenuItem,
									this.ScreensShowAllBanksToolStripMenuItem});
			this.screensToolStripMenuItem.Name = "screensToolStripMenuItem";
			this.screensToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
			this.screensToolStripMenuItem.Text = "&Screens";
			// 
			// updateToolStripMenuItem
			// 
			this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
			this.updateToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.U)));
			this.updateToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.updateToolStripMenuItem.Text = "&Update";
			this.updateToolStripMenuItem.Click += new System.EventHandler(this.BtnUpdateScreensClick_Event);
			// 
			// ScreensAutoUpdateToolStripMenuItem
			// 
			this.ScreensAutoUpdateToolStripMenuItem.Name = "ScreensAutoUpdateToolStripMenuItem";
			this.ScreensAutoUpdateToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.ScreensAutoUpdateToolStripMenuItem.Text = "&Auto Update";
			this.ScreensAutoUpdateToolStripMenuItem.Click += new System.EventHandler(this.ScreensAutoUpdateToolStripMenuItemClick_Event);
			// 
			// ScreensShowAllBanksToolStripMenuItem
			// 
			this.ScreensShowAllBanksToolStripMenuItem.Name = "ScreensShowAllBanksToolStripMenuItem";
			this.ScreensShowAllBanksToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
			this.ScreensShowAllBanksToolStripMenuItem.Text = "&Show All Banks";
			this.ScreensShowAllBanksToolStripMenuItem.Click += new System.EventHandler(this.ScreensShowAllBanksToolStripMenuItemClick_Event);
			// 
			// entitiesToolStripMenuItem
			// 
			this.entitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addEntityToolStripMenuItem1,
									this.deleteEntityToolStripMenuItem,
									this.toolStripSeparator4,
									this.entitiesGroupToolStripMenuItem,
									this.toolStripSeparator6,
									this.editInstancesToolStripMenuItem,
									this.deleteAllEntitiesInstancesToolStripMenuItem});
			this.entitiesToolStripMenuItem.Name = "entitiesToolStripMenuItem";
			this.entitiesToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.entitiesToolStripMenuItem.Text = "&Entities";
			// 
			// addEntityToolStripMenuItem1
			// 
			this.addEntityToolStripMenuItem1.Name = "addEntityToolStripMenuItem1";
			this.addEntityToolStripMenuItem1.Size = new System.Drawing.Size(231, 22);
			this.addEntityToolStripMenuItem1.Text = "&Add";
			this.addEntityToolStripMenuItem1.Click += new System.EventHandler(this.BtnEntityAddClick_Event);
			// 
			// deleteEntityToolStripMenuItem
			// 
			this.deleteEntityToolStripMenuItem.Name = "deleteEntityToolStripMenuItem";
			this.deleteEntityToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.deleteEntityToolStripMenuItem.Text = "&Delete";
			this.deleteEntityToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityDeleteClick_Event);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(228, 6);
			// 
			// entitiesGroupToolStripMenuItem
			// 
			this.entitiesGroupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addEntityGroupToolStripMenuItem,
									this.deleteEntityGroupToolStripMenuItem});
			this.entitiesGroupToolStripMenuItem.Name = "entitiesGroupToolStripMenuItem";
			this.entitiesGroupToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.entitiesGroupToolStripMenuItem.Text = "&Group";
			// 
			// addEntityGroupToolStripMenuItem
			// 
			this.addEntityGroupToolStripMenuItem.Name = "addEntityGroupToolStripMenuItem";
			this.addEntityGroupToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.addEntityGroupToolStripMenuItem.Text = "&Add";
			this.addEntityGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupAddClick_Event);
			// 
			// deleteEntityGroupToolStripMenuItem
			// 
			this.deleteEntityGroupToolStripMenuItem.Name = "deleteEntityGroupToolStripMenuItem";
			this.deleteEntityGroupToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deleteEntityGroupToolStripMenuItem.Text = "&Delete";
			this.deleteEntityGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupDeleteClick_Event);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(228, 6);
			// 
			// editInstancesToolStripMenuItem
			// 
			this.editInstancesToolStripMenuItem.Name = "editInstancesToolStripMenuItem";
			this.editInstancesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
			this.editInstancesToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.editInstancesToolStripMenuItem.Text = "&Edit Instances";
			this.editInstancesToolStripMenuItem.Click += new System.EventHandler(this.BtnEntitiesEditInstancesModeClick_Event);
			// 
			// deleteAllEntitiesInstancesToolStripMenuItem
			// 
			this.deleteAllEntitiesInstancesToolStripMenuItem.Name = "deleteAllEntitiesInstancesToolStripMenuItem";
			this.deleteAllEntitiesInstancesToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
			this.deleteAllEntitiesInstancesToolStripMenuItem.Text = "Delete &Instances of All Entities";
			this.deleteAllEntitiesInstancesToolStripMenuItem.Click += new System.EventHandler(this.EntitiesDeleteInstancesOfAllEntitiesToolStripMenuItemClick_Event);
			// 
			// patternsToolStripMenuItem
			// 
			this.patternsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addPatternToolStripMenuItem1,
									this.deletePatternToolStripMenuItem,
									this.toolStripSeparator5,
									this.groupPatternToolStripMenuItem});
			this.patternsToolStripMenuItem.Name = "patternsToolStripMenuItem";
			this.patternsToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
			this.patternsToolStripMenuItem.Text = "&Patterns";
			// 
			// addPatternToolStripMenuItem1
			// 
			this.addPatternToolStripMenuItem1.Name = "addPatternToolStripMenuItem1";
			this.addPatternToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
			this.addPatternToolStripMenuItem1.Text = "&Add";
			this.addPatternToolStripMenuItem1.Click += new System.EventHandler(this.BtnPatternAddClick_Event);
			// 
			// deletePatternToolStripMenuItem
			// 
			this.deletePatternToolStripMenuItem.Name = "deletePatternToolStripMenuItem";
			this.deletePatternToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deletePatternToolStripMenuItem.Text = "&Delete";
			this.deletePatternToolStripMenuItem.Click += new System.EventHandler(this.BtnPatternDeleteClick_Event);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(104, 6);
			// 
			// groupPatternToolStripMenuItem
			// 
			this.groupPatternToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addPatternGroupToolStripMenuItem1,
									this.deletePatternGroupToolStripMenuItem});
			this.groupPatternToolStripMenuItem.Name = "groupPatternToolStripMenuItem";
			this.groupPatternToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.groupPatternToolStripMenuItem.Text = "&Group";
			// 
			// addPatternGroupToolStripMenuItem1
			// 
			this.addPatternGroupToolStripMenuItem1.Name = "addPatternGroupToolStripMenuItem1";
			this.addPatternGroupToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
			this.addPatternGroupToolStripMenuItem1.Text = "&Add";
			this.addPatternGroupToolStripMenuItem1.Click += new System.EventHandler(this.BtnPatternGroupAddClick_Event);
			// 
			// deletePatternGroupToolStripMenuItem
			// 
			this.deletePatternGroupToolStripMenuItem.Name = "deletePatternGroupToolStripMenuItem";
			this.deletePatternGroupToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deletePatternGroupToolStripMenuItem.Text = "&Delete";
			this.deletePatternGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnPatternGroupDeleteClick_Event);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.quickGuideToolStripMenuItem,
									this.toolStripSeparator27,
									this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// quickGuideToolStripMenuItem
			// 
			this.quickGuideToolStripMenuItem.Name = "quickGuideToolStripMenuItem";
			this.quickGuideToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.quickGuideToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.quickGuideToolStripMenuItem.Text = "Quck Guide";
			this.quickGuideToolStripMenuItem.Click += new System.EventHandler(this.MenuHelpQuickGuideClick_Event);
			// 
			// toolStripSeparator27
			// 
			this.toolStripSeparator27.Name = "toolStripSeparator27";
			this.toolStripSeparator27.Size = new System.Drawing.Size(152, 6);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick_Event);
			// 
			// tabControlTilesLayout
			// 
			this.tabControlTilesLayout.Controls.Add(this.TabTiles);
			this.tabControlTilesLayout.Controls.Add(this.TabLayout);
			this.tabControlTilesLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlTilesLayout.Location = new System.Drawing.Point(0, 24);
			this.tabControlTilesLayout.Name = "tabControlTilesLayout";
			this.tabControlTilesLayout.SelectedIndex = 0;
			this.tabControlTilesLayout.Size = new System.Drawing.Size(1244, 647);
			this.tabControlTilesLayout.TabIndex = 60;
			this.tabControlTilesLayout.SelectedIndexChanged += new System.EventHandler(this.TabCntrlLayoutTilesChanged_Event);
			this.tabControlTilesLayout.DoubleClick += new System.EventHandler(this.TabCntrlDblClick_Event);
			// 
			// TabTiles
			// 
			this.TabTiles.Controls.Add(this.splitContainer1);
			this.TabTiles.Location = new System.Drawing.Point(4, 22);
			this.TabTiles.Name = "TabTiles";
			this.TabTiles.Padding = new System.Windows.Forms.Padding(3);
			this.TabTiles.Size = new System.Drawing.Size(1236, 621);
			this.TabTiles.TabIndex = 0;
			this.TabTiles.Text = "Tiles";
			this.TabTiles.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.Color.Silver;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel1MinSize = 290;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer1.Size = new System.Drawing.Size(1230, 615);
			this.splitContainer1.SplitterDistance = 290;
			this.splitContainer1.TabIndex = 19;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.Color.Silver;
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PanelTiles);
			this.splitContainer2.Size = new System.Drawing.Size(290, 615);
			this.splitContainer2.SplitterDistance = 26;
			this.splitContainer2.TabIndex = 1;
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer4.IsSplitterFixed = true;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.CBoxTileViewType);
			this.splitContainer4.Panel1.Controls.Add(this.label6);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.BtnOptimization);
			this.splitContainer4.Size = new System.Drawing.Size(290, 26);
			this.splitContainer4.SplitterDistance = 144;
			this.splitContainer4.TabIndex = 0;
			// 
			// CBoxTileViewType
			// 
			this.CBoxTileViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxTileViewType.FormattingEnabled = true;
			this.CBoxTileViewType.Items.AddRange(new object[] {
									"Graphics",
									"Property Id",
									"Number",
									"Tiles Usage",
									"Blocks Usage"});
			this.CBoxTileViewType.Location = new System.Drawing.Point(58, 3);
			this.CBoxTileViewType.Name = "CBoxTileViewType";
			this.CBoxTileViewType.Size = new System.Drawing.Size(85, 21);
			this.CBoxTileViewType.TabIndex = 1;
			this.CBoxTileViewType.SelectedIndexChanged += new System.EventHandler(this.CBoxTileViewTypeChanged_Event);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 7);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(62, 19);
			this.label6.TabIndex = 0;
			this.label6.Text = "View Type:";
			// 
			// BtnOptimization
			// 
			this.BtnOptimization.BackColor = System.Drawing.Color.Gainsboro;
			this.BtnOptimization.ForeColor = System.Drawing.SystemColors.ControlText;
			this.BtnOptimization.Location = new System.Drawing.Point(1, 2);
			this.BtnOptimization.Name = "BtnOptimization";
			this.BtnOptimization.Size = new System.Drawing.Size(140, 23);
			this.BtnOptimization.TabIndex = 2;
			this.BtnOptimization.Text = "Optimization";
			this.BtnOptimization.UseVisualStyleBackColor = true;
			this.BtnOptimization.Click += new System.EventHandler(this.BtnOptimizationClick_Event);
			// 
			// PanelTiles
			// 
			this.PanelTiles.AutoScroll = true;
			this.PanelTiles.BackColor = System.Drawing.Color.Silver;
			this.PanelTiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelTiles.Cursor = System.Windows.Forms.Cursors.No;
			this.PanelTiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelTiles.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.PanelTiles.Location = new System.Drawing.Point(0, 0);
			this.PanelTiles.Name = "PanelTiles";
			this.PanelTiles.Size = new System.Drawing.Size(290, 585);
			this.PanelTiles.TabIndex = 3;
			// 
			// splitContainer3
			// 
			this.splitContainer3.BackColor = System.Drawing.Color.Silver;
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer3.IsSplitterFixed = true;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.BackColor = System.Drawing.Color.Silver;
			this.splitContainer3.Panel1.Controls.Add(this.GrpBoxPalettes);
			this.splitContainer3.Panel1.Controls.Add(this.GrpBoxBlockEditor);
			this.splitContainer3.Panel1.Controls.Add(this.GrpBoxTileEditor);
			this.splitContainer3.Panel1.Controls.Add(this.GrpBoxScreenData);
			this.splitContainer3.Panel1.Controls.Add(this.GrpBoxCHRBank);
			this.splitContainer3.Panel1MinSize = 563;
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.groupBox2);
			this.splitContainer3.Size = new System.Drawing.Size(936, 615);
			this.splitContainer3.SplitterDistance = 563;
			this.splitContainer3.TabIndex = 0;
			// 
			// GrpBoxPalettes
			// 
			this.GrpBoxPalettes.Controls.Add(this.BtnSwapColors);
			this.GrpBoxPalettes.Controls.Add(this.CBoxPalettes);
			this.GrpBoxPalettes.Controls.Add(this.CheckBoxPalettePerCHR);
			this.GrpBoxPalettes.Controls.Add(this.Palette3);
			this.GrpBoxPalettes.Controls.Add(this.Palette2);
			this.GrpBoxPalettes.Controls.Add(this.Palette1);
			this.GrpBoxPalettes.Controls.Add(this.BtnPltDelete);
			this.GrpBoxPalettes.Controls.Add(this.Palette0);
			this.GrpBoxPalettes.Controls.Add(this.PaletteMain);
			this.GrpBoxPalettes.Controls.Add(this.label4);
			this.GrpBoxPalettes.Controls.Add(this.BtnPltCopy);
			this.GrpBoxPalettes.Controls.Add(this.label3);
			this.GrpBoxPalettes.Controls.Add(this.label2);
			this.GrpBoxPalettes.Controls.Add(this.label1);
			this.GrpBoxPalettes.Controls.Add(this.LabelPalette12);
			this.GrpBoxPalettes.Controls.Add(this.LabelPalette34);
			this.GrpBoxPalettes.Location = new System.Drawing.Point(3, 342);
			this.GrpBoxPalettes.Name = "GrpBoxPalettes";
			this.GrpBoxPalettes.Size = new System.Drawing.Size(276, 231);
			this.GrpBoxPalettes.TabIndex = 24;
			this.GrpBoxPalettes.TabStop = false;
			this.GrpBoxPalettes.Text = "Palettes:";
			// 
			// BtnSwapColors
			// 
			this.BtnSwapColors.Location = new System.Drawing.Point(181, 157);
			this.BtnSwapColors.Name = "BtnSwapColors";
			this.BtnSwapColors.Size = new System.Drawing.Size(85, 23);
			this.BtnSwapColors.TabIndex = 28;
			this.BtnSwapColors.Text = "Swap Colors";
			this.BtnSwapColors.UseVisualStyleBackColor = true;
			this.BtnSwapColors.Click += new System.EventHandler(this.BtnSwapColorsClick_Event);
			// 
			// CBoxPalettes
			// 
			this.CBoxPalettes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.CBoxPalettes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxPalettes.FormattingEnabled = true;
			this.CBoxPalettes.Location = new System.Drawing.Point(10, 158);
			this.CBoxPalettes.MaxDropDownItems = 16;
			this.CBoxPalettes.Name = "CBoxPalettes";
			this.CBoxPalettes.Size = new System.Drawing.Size(59, 21);
			this.CBoxPalettes.TabIndex = 25;
			this.CBoxPalettes.DropDown += new System.EventHandler(this.CBocPalettesAdjustWidthDropDown_Event);
			this.CBoxPalettes.SelectedIndexChanged += new System.EventHandler(this.CBoxPalettesChanged_Event);
			// 
			// CheckBoxPalettePerCHR
			// 
			this.CheckBoxPalettePerCHR.Location = new System.Drawing.Point(10, 186);
			this.CheckBoxPalettePerCHR.Name = "CheckBoxPalettePerCHR";
			this.CheckBoxPalettePerCHR.Size = new System.Drawing.Size(148, 19);
			this.CheckBoxPalettePerCHR.TabIndex = 29;
			this.CheckBoxPalettePerCHR.Text = "Palette per CHR (MMC5)";
			this.CheckBoxPalettePerCHR.UseVisualStyleBackColor = true;
			this.CheckBoxPalettePerCHR.CheckedChanged += new System.EventHandler(this.CheckBoxPalettePerCHRChecked_Event);
			// 
			// Palette3
			// 
			this.Palette3.BackColor = System.Drawing.Color.Black;
			this.Palette3.Location = new System.Drawing.Point(170, 52);
			this.Palette3.Name = "Palette3";
			this.Palette3.Size = new System.Drawing.Size(80, 20);
			this.Palette3.TabIndex = 7;
			this.Palette3.TabStop = false;
			// 
			// Palette2
			// 
			this.Palette2.BackColor = System.Drawing.Color.Black;
			this.Palette2.Location = new System.Drawing.Point(41, 52);
			this.Palette2.Name = "Palette2";
			this.Palette2.Size = new System.Drawing.Size(80, 20);
			this.Palette2.TabIndex = 7;
			this.Palette2.TabStop = false;
			// 
			// Palette1
			// 
			this.Palette1.BackColor = System.Drawing.Color.Black;
			this.Palette1.Location = new System.Drawing.Point(170, 22);
			this.Palette1.Name = "Palette1";
			this.Palette1.Size = new System.Drawing.Size(80, 20);
			this.Palette1.TabIndex = 7;
			this.Palette1.TabStop = false;
			// 
			// BtnPltDelete
			// 
			this.BtnPltDelete.BackColor = System.Drawing.Color.LemonChiffon;
			this.BtnPltDelete.Location = new System.Drawing.Point(125, 157);
			this.BtnPltDelete.Name = "BtnPltDelete";
			this.BtnPltDelete.Size = new System.Drawing.Size(48, 23);
			this.BtnPltDelete.TabIndex = 27;
			this.BtnPltDelete.Text = "Delete";
			this.BtnPltDelete.UseVisualStyleBackColor = true;
			this.BtnPltDelete.Click += new System.EventHandler(this.BtnPltDeleteClick_Event);
			// 
			// Palette0
			// 
			this.Palette0.BackColor = System.Drawing.Color.Black;
			this.Palette0.Location = new System.Drawing.Point(41, 22);
			this.Palette0.Name = "Palette0";
			this.Palette0.Size = new System.Drawing.Size(80, 20);
			this.Palette0.TabIndex = 7;
			this.Palette0.TabStop = false;
			// 
			// PaletteMain
			// 
			this.PaletteMain.BackColor = System.Drawing.Color.Black;
			this.PaletteMain.Location = new System.Drawing.Point(10, 86);
			this.PaletteMain.Name = "PaletteMain";
			this.PaletteMain.Size = new System.Drawing.Size(256, 64);
			this.PaletteMain.TabIndex = 6;
			this.PaletteMain.TabStop = false;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.Location = new System.Drawing.Point(152, 52);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 20);
			this.label4.TabIndex = 8;
			this.label4.Text = "4:";
			// 
			// BtnPltCopy
			// 
			this.BtnPltCopy.BackColor = System.Drawing.Color.LemonChiffon;
			this.BtnPltCopy.Location = new System.Drawing.Point(74, 157);
			this.BtnPltCopy.Name = "BtnPltCopy";
			this.BtnPltCopy.Size = new System.Drawing.Size(48, 23);
			this.BtnPltCopy.TabIndex = 26;
			this.BtnPltCopy.Text = "Copy";
			this.BtnPltCopy.UseVisualStyleBackColor = true;
			this.BtnPltCopy.Click += new System.EventHandler(this.BtnPltCopyClick_Event);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(152, 23);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(26, 20);
			this.label3.TabIndex = 8;
			this.label3.Text = "2:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(21, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(26, 20);
			this.label2.TabIndex = 8;
			this.label2.Text = "3:";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(21, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 20);
			this.label1.TabIndex = 8;
			this.label1.Text = "1:";
			// 
			// LabelPalette12
			// 
			this.LabelPalette12.BackColor = System.Drawing.Color.Silver;
			this.LabelPalette12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.LabelPalette12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.LabelPalette12.ForeColor = System.Drawing.SystemColors.ControlText;
			this.LabelPalette12.Location = new System.Drawing.Point(5, 23);
			this.LabelPalette12.Name = "LabelPalette12";
			this.LabelPalette12.Size = new System.Drawing.Size(16, 20);
			this.LabelPalette12.TabIndex = 8;
			this.LabelPalette12.Text = "i";
			this.LabelPalette12.Visible = false;
			// 
			// LabelPalette34
			// 
			this.LabelPalette34.BackColor = System.Drawing.Color.Red;
			this.LabelPalette34.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.LabelPalette34.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.LabelPalette34.Location = new System.Drawing.Point(5, 52);
			this.LabelPalette34.Name = "LabelPalette34";
			this.LabelPalette34.Size = new System.Drawing.Size(16, 20);
			this.LabelPalette34.TabIndex = 8;
			this.LabelPalette34.Text = "p";
			this.LabelPalette34.Visible = false;
			// 
			// GrpBoxBlockEditor
			// 
			this.GrpBoxBlockEditor.Controls.Add(this.PBoxBlockEditor);
			this.GrpBoxBlockEditor.Controls.Add(this.CBoxBlockObjId);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnBlockReserveCHRs);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnBlockRotate);
			this.GrpBoxBlockEditor.Controls.Add(this.LabelObjId);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnInvInk);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnSwapInkPaper);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnEditModeDraw);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnUpdateGFX);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnEditModeSelectCHR);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnBlockHFlip);
			this.GrpBoxBlockEditor.Controls.Add(this.BtnBlockVFlip);
			this.GrpBoxBlockEditor.Controls.Add(this.label10);
			this.GrpBoxBlockEditor.Location = new System.Drawing.Point(283, 0);
			this.GrpBoxBlockEditor.Name = "GrpBoxBlockEditor";
			this.GrpBoxBlockEditor.Size = new System.Drawing.Size(276, 364);
			this.GrpBoxBlockEditor.TabIndex = 15;
			this.GrpBoxBlockEditor.TabStop = false;
			this.GrpBoxBlockEditor.Text = "Block Editor:";
			// 
			// PBoxBlockEditor
			// 
			this.PBoxBlockEditor.BackColor = System.Drawing.Color.Black;
			this.PBoxBlockEditor.ContextMenuStrip = this.ContextMenuBlockEditor;
			this.PBoxBlockEditor.Location = new System.Drawing.Point(10, 18);
			this.PBoxBlockEditor.Name = "PBoxBlockEditor";
			this.PBoxBlockEditor.Size = new System.Drawing.Size(256, 256);
			this.PBoxBlockEditor.TabIndex = 5;
			this.PBoxBlockEditor.TabStop = false;
			// 
			// ContextMenuBlockEditor
			// 
			this.ContextMenuBlockEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.blockEditModesToolStripMenuItem,
									this.separatorToolStripMenuItem,
									this.CHRSelectToolStripMenuItem,
									this.DrawToolStripMenuItem,
									this.toolStripSeparator14,
									this.propertyIdToolStripMenuItem,
									this.toolStripSeparator15,
									this.PropIdPerBlockToolStripMenuItem,
									this.PropIdPerCHRToolStripMenuItem});
			this.ContextMenuBlockEditor.Name = "ContextMenuBlockEditor";
			this.ContextMenuBlockEditor.Size = new System.Drawing.Size(153, 154);
			// 
			// blockEditModesToolStripMenuItem
			// 
			this.blockEditModesToolStripMenuItem.Enabled = false;
			this.blockEditModesToolStripMenuItem.Name = "blockEditModesToolStripMenuItem";
			this.blockEditModesToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.blockEditModesToolStripMenuItem.Text = "Edit Modes:";
			// 
			// separatorToolStripMenuItem
			// 
			this.separatorToolStripMenuItem.Name = "separatorToolStripMenuItem";
			this.separatorToolStripMenuItem.Size = new System.Drawing.Size(149, 6);
			// 
			// CHRSelectToolStripMenuItem
			// 
			this.CHRSelectToolStripMenuItem.CheckOnClick = true;
			this.CHRSelectToolStripMenuItem.Name = "CHRSelectToolStripMenuItem";
			this.CHRSelectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.CHRSelectToolStripMenuItem.Text = "Select CHRs";
			this.CHRSelectToolStripMenuItem.Click += new System.EventHandler(this.SelectCHRToolStripMenuItemClick_Event);
			// 
			// DrawToolStripMenuItem
			// 
			this.DrawToolStripMenuItem.CheckOnClick = true;
			this.DrawToolStripMenuItem.Name = "DrawToolStripMenuItem";
			this.DrawToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.DrawToolStripMenuItem.Text = "Draw";
			this.DrawToolStripMenuItem.Click += new System.EventHandler(this.DrawToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator14
			// 
			this.toolStripSeparator14.Name = "toolStripSeparator14";
			this.toolStripSeparator14.Size = new System.Drawing.Size(149, 6);
			// 
			// propertyIdToolStripMenuItem
			// 
			this.propertyIdToolStripMenuItem.Enabled = false;
			this.propertyIdToolStripMenuItem.Name = "propertyIdToolStripMenuItem";
			this.propertyIdToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.propertyIdToolStripMenuItem.Text = "Property Id per";
			// 
			// toolStripSeparator15
			// 
			this.toolStripSeparator15.Name = "toolStripSeparator15";
			this.toolStripSeparator15.Size = new System.Drawing.Size(149, 6);
			// 
			// PropIdPerBlockToolStripMenuItem
			// 
			this.PropIdPerBlockToolStripMenuItem.Name = "PropIdPerBlockToolStripMenuItem";
			this.PropIdPerBlockToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.PropIdPerBlockToolStripMenuItem.Text = "Block";
			this.PropIdPerBlockToolStripMenuItem.Click += new System.EventHandler(this.PropertyPerBlockToolStripMenuItemClick_Event);
			// 
			// PropIdPerCHRToolStripMenuItem
			// 
			this.PropIdPerCHRToolStripMenuItem.Name = "PropIdPerCHRToolStripMenuItem";
			this.PropIdPerCHRToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.PropIdPerCHRToolStripMenuItem.Text = "CHR";
			this.PropIdPerCHRToolStripMenuItem.Click += new System.EventHandler(this.PropertyPerCHRToolStripMenuItemClick_Event);
			// 
			// CBoxBlockObjId
			// 
			this.CBoxBlockObjId.BackColor = System.Drawing.SystemColors.Window;
			this.CBoxBlockObjId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxBlockObjId.FormattingEnabled = true;
			this.CBoxBlockObjId.Items.AddRange(new object[] {
									"0",
									"1",
									"2",
									"3",
									"4",
									"5",
									"6",
									"7",
									"8",
									"9",
									"10",
									"11",
									"12",
									"13",
									"14",
									"15"});
			this.CBoxBlockObjId.Location = new System.Drawing.Point(225, 333);
			this.CBoxBlockObjId.Name = "CBoxBlockObjId";
			this.CBoxBlockObjId.Size = new System.Drawing.Size(40, 21);
			this.CBoxBlockObjId.TabIndex = 25;
			this.CBoxBlockObjId.SelectionChangeCommitted += new System.EventHandler(this.CBoxBlockObjIdChanged_Event);
			// 
			// BtnBlockReserveCHRs
			// 
			this.BtnBlockReserveCHRs.Location = new System.Drawing.Point(10, 334);
			this.BtnBlockReserveCHRs.Name = "BtnBlockReserveCHRs";
			this.BtnBlockReserveCHRs.Size = new System.Drawing.Size(93, 22);
			this.BtnBlockReserveCHRs.TabIndex = 23;
			this.BtnBlockReserveCHRs.Text = "Reserve CHRs";
			this.BtnBlockReserveCHRs.UseVisualStyleBackColor = true;
			this.BtnBlockReserveCHRs.Click += new System.EventHandler(this.BtnBlockReserveCHRsClick_Event);
			// 
			// BtnBlockRotate
			// 
			this.BtnBlockRotate.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnBlockRotate.Location = new System.Drawing.Point(10, 309);
			this.BtnBlockRotate.Name = "BtnBlockRotate";
			this.BtnBlockRotate.Size = new System.Drawing.Size(93, 20);
			this.BtnBlockRotate.TabIndex = 20;
			this.BtnBlockRotate.Text = "Rotate";
			this.BtnBlockRotate.UseVisualStyleBackColor = false;
			this.BtnBlockRotate.Click += new System.EventHandler(this.BtnBlockRotateClick_Event);
			// 
			// LabelObjId
			// 
			this.LabelObjId.Location = new System.Drawing.Point(159, 337);
			this.LabelObjId.Name = "LabelObjId";
			this.LabelObjId.Size = new System.Drawing.Size(64, 16);
			this.LabelObjId.TabIndex = 0;
			this.LabelObjId.Text = "Property Id:";
			// 
			// BtnInvInk
			// 
			this.BtnInvInk.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
			this.BtnInvInk.Location = new System.Drawing.Point(106, 334);
			this.BtnInvInk.Name = "BtnInvInk";
			this.BtnInvInk.Size = new System.Drawing.Size(49, 22);
			this.BtnInvInk.TabIndex = 24;
			this.BtnInvInk.Text = "Invert";
			this.BtnInvInk.UseVisualStyleBackColor = false;
			this.BtnInvInk.Visible = false;
			this.BtnInvInk.Click += new System.EventHandler(this.BtnInvInkClick_Event);
			// 
			// BtnSwapInkPaper
			// 
			this.BtnSwapInkPaper.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
			this.BtnSwapInkPaper.Location = new System.Drawing.Point(106, 309);
			this.BtnSwapInkPaper.Name = "BtnSwapInkPaper";
			this.BtnSwapInkPaper.Size = new System.Drawing.Size(49, 20);
			this.BtnSwapInkPaper.TabIndex = 21;
			this.BtnSwapInkPaper.Text = "I <-> P";
			this.BtnSwapInkPaper.UseVisualStyleBackColor = false;
			this.BtnSwapInkPaper.Visible = false;
			this.BtnSwapInkPaper.Click += new System.EventHandler(this.BtnSwapInkPaperClick_Event);
			// 
			// BtnEditModeDraw
			// 
			this.BtnEditModeDraw.BackColor = System.Drawing.Color.White;
			this.BtnEditModeDraw.Location = new System.Drawing.Point(217, 283);
			this.BtnEditModeDraw.Name = "BtnEditModeDraw";
			this.BtnEditModeDraw.Size = new System.Drawing.Size(49, 20);
			this.BtnEditModeDraw.TabIndex = 19;
			this.BtnEditModeDraw.Text = "Draw";
			this.BtnEditModeDraw.UseVisualStyleBackColor = true;
			this.BtnEditModeDraw.Click += new System.EventHandler(this.DrawToolStripMenuItemClick_Event);
			// 
			// BtnUpdateGFX
			// 
			this.BtnUpdateGFX.BackColor = System.Drawing.Color.Coral;
			this.BtnUpdateGFX.Location = new System.Drawing.Point(158, 308);
			this.BtnUpdateGFX.Name = "BtnUpdateGFX";
			this.BtnUpdateGFX.Size = new System.Drawing.Size(108, 22);
			this.BtnUpdateGFX.TabIndex = 22;
			this.BtnUpdateGFX.Text = "Update GFX";
			this.BtnUpdateGFX.UseVisualStyleBackColor = true;
			this.BtnUpdateGFX.Click += new System.EventHandler(this.BtnUpdateGFXClick_Event);
			// 
			// BtnEditModeSelectCHR
			// 
			this.BtnEditModeSelectCHR.BackColor = System.Drawing.Color.White;
			this.BtnEditModeSelectCHR.Location = new System.Drawing.Point(158, 283);
			this.BtnEditModeSelectCHR.Name = "BtnEditModeSelectCHR";
			this.BtnEditModeSelectCHR.Size = new System.Drawing.Size(54, 20);
			this.BtnEditModeSelectCHR.TabIndex = 18;
			this.BtnEditModeSelectCHR.Text = "Select";
			this.BtnEditModeSelectCHR.UseVisualStyleBackColor = true;
			this.BtnEditModeSelectCHR.Click += new System.EventHandler(this.SelectCHRToolStripMenuItemClick_Event);
			// 
			// BtnBlockHFlip
			// 
			this.BtnBlockHFlip.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnBlockHFlip.Location = new System.Drawing.Point(61, 283);
			this.BtnBlockHFlip.Name = "BtnBlockHFlip";
			this.BtnBlockHFlip.Size = new System.Drawing.Size(42, 20);
			this.BtnBlockHFlip.TabIndex = 17;
			this.BtnBlockHFlip.Text = "HFlip";
			this.BtnBlockHFlip.UseVisualStyleBackColor = false;
			this.BtnBlockHFlip.Click += new System.EventHandler(this.BtnBlockHFlipClick_Event);
			// 
			// BtnBlockVFlip
			// 
			this.BtnBlockVFlip.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnBlockVFlip.Location = new System.Drawing.Point(10, 283);
			this.BtnBlockVFlip.Name = "BtnBlockVFlip";
			this.BtnBlockVFlip.Size = new System.Drawing.Size(42, 20);
			this.BtnBlockVFlip.TabIndex = 16;
			this.BtnBlockVFlip.Text = "VFlip";
			this.BtnBlockVFlip.UseVisualStyleBackColor = false;
			this.BtnBlockVFlip.Click += new System.EventHandler(this.BtnBlockVFlipClick_Event);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(116, 286);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(43, 20);
			this.label10.TabIndex = 0;
			this.label10.Text = "Modes:";
			// 
			// GrpBoxTileEditor
			// 
			this.GrpBoxTileEditor.Controls.Add(this.BtnTileReserveBlocks);
			this.GrpBoxTileEditor.Controls.Add(this.CheckBoxTileEditorLock);
			this.GrpBoxTileEditor.Controls.Add(this.PBoxTilePreview);
			this.GrpBoxTileEditor.Location = new System.Drawing.Point(283, 370);
			this.GrpBoxTileEditor.Name = "GrpBoxTileEditor";
			this.GrpBoxTileEditor.Size = new System.Drawing.Size(148, 203);
			this.GrpBoxTileEditor.TabIndex = 30;
			this.GrpBoxTileEditor.TabStop = false;
			this.GrpBoxTileEditor.Text = "Tile Editor:";
			// 
			// BtnTileReserveBlocks
			// 
			this.BtnTileReserveBlocks.Location = new System.Drawing.Point(10, 174);
			this.BtnTileReserveBlocks.Name = "BtnTileReserveBlocks";
			this.BtnTileReserveBlocks.Size = new System.Drawing.Size(128, 22);
			this.BtnTileReserveBlocks.TabIndex = 32;
			this.BtnTileReserveBlocks.Text = "Reserve Blocks";
			this.BtnTileReserveBlocks.UseVisualStyleBackColor = true;
			this.BtnTileReserveBlocks.Click += new System.EventHandler(this.BtnTileReserveBlocksClick_Event);
			// 
			// CheckBoxTileEditorLock
			// 
			this.CheckBoxTileEditorLock.Location = new System.Drawing.Point(10, 151);
			this.CheckBoxTileEditorLock.Name = "CheckBoxTileEditorLock";
			this.CheckBoxTileEditorLock.Size = new System.Drawing.Size(63, 19);
			this.CheckBoxTileEditorLock.TabIndex = 31;
			this.CheckBoxTileEditorLock.Text = "Locked";
			this.CheckBoxTileEditorLock.UseVisualStyleBackColor = true;
			this.CheckBoxTileEditorLock.CheckedChanged += new System.EventHandler(this.CheckBoxTileEditorLockedChecked_Event);
			// 
			// PBoxTilePreview
			// 
			this.PBoxTilePreview.BackColor = System.Drawing.Color.Black;
			this.PBoxTilePreview.Location = new System.Drawing.Point(10, 16);
			this.PBoxTilePreview.Name = "PBoxTilePreview";
			this.PBoxTilePreview.Size = new System.Drawing.Size(128, 128);
			this.PBoxTilePreview.TabIndex = 16;
			this.PBoxTilePreview.TabStop = false;
			// 
			// GrpBoxScreenData
			// 
			this.GrpBoxScreenData.Controls.Add(this.LabelScreenResolution);
			this.GrpBoxScreenData.Controls.Add(this.NumericUpDownScrBlocksHeight);
			this.GrpBoxScreenData.Controls.Add(this.NumericUpDownScrBlocksWidth);
			this.GrpBoxScreenData.Controls.Add(this.BtnScreenDataInfo);
			this.GrpBoxScreenData.Controls.Add(this.RBtnScreenDataBlocks);
			this.GrpBoxScreenData.Controls.Add(this.RBtnScreenDataTiles);
			this.GrpBoxScreenData.Controls.Add(this.label13);
			this.GrpBoxScreenData.Controls.Add(this.label12);
			this.GrpBoxScreenData.Location = new System.Drawing.Point(437, 370);
			this.GrpBoxScreenData.Name = "GrpBoxScreenData";
			this.GrpBoxScreenData.Size = new System.Drawing.Size(122, 203);
			this.GrpBoxScreenData.TabIndex = 33;
			this.GrpBoxScreenData.TabStop = false;
			this.GrpBoxScreenData.Text = "Screen Data:";
			// 
			// LabelScreenResolution
			// 
			this.LabelScreenResolution.Location = new System.Drawing.Point(14, 118);
			this.LabelScreenResolution.Name = "LabelScreenResolution";
			this.LabelScreenResolution.Size = new System.Drawing.Size(94, 18);
			this.LabelScreenResolution.TabIndex = 41;
			this.LabelScreenResolution.Text = "[WxH]";
			this.LabelScreenResolution.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// NumericUpDownScrBlocksHeight
			// 
			this.NumericUpDownScrBlocksHeight.Location = new System.Drawing.Point(67, 93);
			this.NumericUpDownScrBlocksHeight.Minimum = new decimal(new int[] {
									4,
									0,
									0,
									0});
			this.NumericUpDownScrBlocksHeight.Name = "NumericUpDownScrBlocksHeight";
			this.NumericUpDownScrBlocksHeight.Size = new System.Drawing.Size(45, 20);
			this.NumericUpDownScrBlocksHeight.TabIndex = 39;
			this.NumericUpDownScrBlocksHeight.Value = new decimal(new int[] {
									4,
									0,
									0,
									0});
			this.NumericUpDownScrBlocksHeight.ValueChanged += new System.EventHandler(this.NumericUpDownScrBlocksChanged_Event);
			// 
			// NumericUpDownScrBlocksWidth
			// 
			this.NumericUpDownScrBlocksWidth.Location = new System.Drawing.Point(67, 69);
			this.NumericUpDownScrBlocksWidth.Minimum = new decimal(new int[] {
									4,
									0,
									0,
									0});
			this.NumericUpDownScrBlocksWidth.Name = "NumericUpDownScrBlocksWidth";
			this.NumericUpDownScrBlocksWidth.Size = new System.Drawing.Size(45, 20);
			this.NumericUpDownScrBlocksWidth.TabIndex = 37;
			this.NumericUpDownScrBlocksWidth.Value = new decimal(new int[] {
									4,
									0,
									0,
									0});
			this.NumericUpDownScrBlocksWidth.ValueChanged += new System.EventHandler(this.NumericUpDownScrBlocksChanged_Event);
			// 
			// BtnScreenDataInfo
			// 
			this.BtnScreenDataInfo.Location = new System.Drawing.Point(91, 20);
			this.BtnScreenDataInfo.Name = "BtnScreenDataInfo";
			this.BtnScreenDataInfo.Size = new System.Drawing.Size(20, 20);
			this.BtnScreenDataInfo.TabIndex = 34;
			this.BtnScreenDataInfo.Text = "?";
			this.BtnScreenDataInfo.UseVisualStyleBackColor = true;
			this.BtnScreenDataInfo.Click += new System.EventHandler(this.BtnScreenDataInfoClick_Event);
			// 
			// RBtnScreenDataBlocks
			// 
			this.RBtnScreenDataBlocks.AutoCheck = false;
			this.RBtnScreenDataBlocks.Location = new System.Drawing.Point(13, 43);
			this.RBtnScreenDataBlocks.Name = "RBtnScreenDataBlocks";
			this.RBtnScreenDataBlocks.Size = new System.Drawing.Size(103, 20);
			this.RBtnScreenDataBlocks.TabIndex = 36;
			this.RBtnScreenDataBlocks.Text = "Blocks (2x2)";
			this.RBtnScreenDataBlocks.UseVisualStyleBackColor = true;
			this.RBtnScreenDataBlocks.Click += new System.EventHandler(this.RBtnScreenDataBlocksClick_Event);
			// 
			// RBtnScreenDataTiles
			// 
			this.RBtnScreenDataTiles.AutoCheck = false;
			this.RBtnScreenDataTiles.Checked = true;
			this.RBtnScreenDataTiles.Location = new System.Drawing.Point(13, 20);
			this.RBtnScreenDataTiles.Name = "RBtnScreenDataTiles";
			this.RBtnScreenDataTiles.Size = new System.Drawing.Size(98, 20);
			this.RBtnScreenDataTiles.TabIndex = 35;
			this.RBtnScreenDataTiles.TabStop = true;
			this.RBtnScreenDataTiles.Text = "Tiles (4x4)";
			this.RBtnScreenDataTiles.UseVisualStyleBackColor = true;
			this.RBtnScreenDataTiles.Click += new System.EventHandler(this.RBtnScreenDataTilesClick_Event);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(9, 93);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(58, 20);
			this.label13.TabIndex = 40;
			this.label13.Text = "Blocks H:";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(9, 69);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(58, 20);
			this.label12.TabIndex = 38;
			this.label12.Text = "Blocks W:";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// GrpBoxCHRBank
			// 
			this.GrpBoxCHRBank.Controls.Add(this.BtnCHRBankNextPage);
			this.GrpBoxCHRBank.Controls.Add(this.BtnCHRBankPrevPage);
			this.GrpBoxCHRBank.Controls.Add(this.CBoxCHRBanks);
			this.GrpBoxCHRBank.Controls.Add(this.BtnDeleteCHRBank);
			this.GrpBoxCHRBank.Controls.Add(this.BtnCHRRotate);
			this.GrpBoxCHRBank.Controls.Add(this.BtnCHRHFlip);
			this.GrpBoxCHRBank.Controls.Add(this.BtnCopyCHRBank);
			this.GrpBoxCHRBank.Controls.Add(this.BtnAddCHRBank);
			this.GrpBoxCHRBank.Controls.Add(this.BtnCHRVFlip);
			this.GrpBoxCHRBank.Controls.Add(this.label5);
			this.GrpBoxCHRBank.Controls.Add(this.PBoxCHRBank);
			this.GrpBoxCHRBank.Location = new System.Drawing.Point(3, 0);
			this.GrpBoxCHRBank.Name = "GrpBoxCHRBank";
			this.GrpBoxCHRBank.Size = new System.Drawing.Size(276, 336);
			this.GrpBoxCHRBank.TabIndex = 5;
			this.GrpBoxCHRBank.TabStop = false;
			this.GrpBoxCHRBank.Text = "CHR Bank:";
			// 
			// BtnCHRBankNextPage
			// 
			this.BtnCHRBankNextPage.Location = new System.Drawing.Point(130, 283);
			this.BtnCHRBankNextPage.Name = "BtnCHRBankNextPage";
			this.BtnCHRBankNextPage.Size = new System.Drawing.Size(28, 23);
			this.BtnCHRBankNextPage.TabIndex = 10;
			this.BtnCHRBankNextPage.Text = ">>";
			this.BtnCHRBankNextPage.UseVisualStyleBackColor = true;
			this.BtnCHRBankNextPage.Click += new System.EventHandler(this.BtnCHRBankNextPageClick_Event);
			// 
			// BtnCHRBankPrevPage
			// 
			this.BtnCHRBankPrevPage.Location = new System.Drawing.Point(100, 283);
			this.BtnCHRBankPrevPage.Name = "BtnCHRBankPrevPage";
			this.BtnCHRBankPrevPage.Size = new System.Drawing.Size(28, 23);
			this.BtnCHRBankPrevPage.TabIndex = 9;
			this.BtnCHRBankPrevPage.Text = "<<";
			this.BtnCHRBankPrevPage.UseVisualStyleBackColor = true;
			this.BtnCHRBankPrevPage.Click += new System.EventHandler(this.BtnCHRBankPrevPageClick_Event);
			// 
			// CBoxCHRBanks
			// 
			this.CBoxCHRBanks.BackColor = System.Drawing.SystemColors.Window;
			this.CBoxCHRBanks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxCHRBanks.FormattingEnabled = true;
			this.CBoxCHRBanks.Location = new System.Drawing.Point(164, 309);
			this.CBoxCHRBanks.Name = "CBoxCHRBanks";
			this.CBoxCHRBanks.Size = new System.Drawing.Size(47, 21);
			this.CBoxCHRBanks.TabIndex = 13;
			this.CBoxCHRBanks.SelectedIndexChanged += new System.EventHandler(this.CHRBankChanged_Event);
			// 
			// BtnDeleteCHRBank
			// 
			this.BtnDeleteCHRBank.BackColor = System.Drawing.Color.Wheat;
			this.BtnDeleteCHRBank.Location = new System.Drawing.Point(217, 309);
			this.BtnDeleteCHRBank.Name = "BtnDeleteCHRBank";
			this.BtnDeleteCHRBank.Size = new System.Drawing.Size(48, 20);
			this.BtnDeleteCHRBank.TabIndex = 14;
			this.BtnDeleteCHRBank.Text = "Bank-";
			this.BtnDeleteCHRBank.UseVisualStyleBackColor = false;
			this.BtnDeleteCHRBank.Click += new System.EventHandler(this.BtnDeleteCHRBankClick_Event);
			// 
			// BtnCHRRotate
			// 
			this.BtnCHRRotate.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnCHRRotate.Location = new System.Drawing.Point(10, 309);
			this.BtnCHRRotate.Name = "BtnCHRRotate";
			this.BtnCHRRotate.Size = new System.Drawing.Size(84, 20);
			this.BtnCHRRotate.TabIndex = 8;
			this.BtnCHRRotate.Text = "Rotate";
			this.BtnCHRRotate.UseVisualStyleBackColor = false;
			this.BtnCHRRotate.Click += new System.EventHandler(this.BtnCHRRotateClick_Event);
			// 
			// BtnCHRHFlip
			// 
			this.BtnCHRHFlip.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnCHRHFlip.Location = new System.Drawing.Point(55, 283);
			this.BtnCHRHFlip.Name = "BtnCHRHFlip";
			this.BtnCHRHFlip.Size = new System.Drawing.Size(39, 20);
			this.BtnCHRHFlip.TabIndex = 7;
			this.BtnCHRHFlip.Text = "HFlip";
			this.BtnCHRHFlip.UseVisualStyleBackColor = false;
			this.BtnCHRHFlip.Click += new System.EventHandler(this.BtnCHRHFlipClick_Event);
			// 
			// BtnCopyCHRBank
			// 
			this.BtnCopyCHRBank.BackColor = System.Drawing.Color.LemonChiffon;
			this.BtnCopyCHRBank.Location = new System.Drawing.Point(164, 283);
			this.BtnCopyCHRBank.Name = "BtnCopyCHRBank";
			this.BtnCopyCHRBank.Size = new System.Drawing.Size(48, 20);
			this.BtnCopyCHRBank.TabIndex = 11;
			this.BtnCopyCHRBank.Text = "Copy";
			this.BtnCopyCHRBank.UseVisualStyleBackColor = true;
			this.BtnCopyCHRBank.Click += new System.EventHandler(this.BtnCopyCHRBankClick_Event);
			// 
			// BtnAddCHRBank
			// 
			this.BtnAddCHRBank.BackColor = System.Drawing.Color.Wheat;
			this.BtnAddCHRBank.Location = new System.Drawing.Point(217, 283);
			this.BtnAddCHRBank.Name = "BtnAddCHRBank";
			this.BtnAddCHRBank.Size = new System.Drawing.Size(48, 20);
			this.BtnAddCHRBank.TabIndex = 12;
			this.BtnAddCHRBank.Text = "Bank+";
			this.BtnAddCHRBank.UseVisualStyleBackColor = false;
			this.BtnAddCHRBank.Click += new System.EventHandler(this.BtnAddCHRBankClick_Event);
			// 
			// BtnCHRVFlip
			// 
			this.BtnCHRVFlip.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnCHRVFlip.Location = new System.Drawing.Point(10, 283);
			this.BtnCHRVFlip.Name = "BtnCHRVFlip";
			this.BtnCHRVFlip.Size = new System.Drawing.Size(39, 20);
			this.BtnCHRVFlip.TabIndex = 6;
			this.BtnCHRVFlip.Text = "VFlip";
			this.BtnCHRVFlip.UseVisualStyleBackColor = false;
			this.BtnCHRVFlip.Click += new System.EventHandler(this.BtnCHRVFlipClick_Event);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(123, 313);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(40, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Banks:";
			// 
			// PBoxCHRBank
			// 
			this.PBoxCHRBank.BackColor = System.Drawing.Color.Black;
			this.PBoxCHRBank.ContextMenuStrip = this.ContextMenuCHRBank;
			this.PBoxCHRBank.Location = new System.Drawing.Point(10, 18);
			this.PBoxCHRBank.Name = "PBoxCHRBank";
			this.PBoxCHRBank.Size = new System.Drawing.Size(256, 256);
			this.PBoxCHRBank.TabIndex = 5;
			this.PBoxCHRBank.TabStop = false;
			// 
			// ContextMenuCHRBank
			// 
			this.ContextMenuCHRBank.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyCHRToolStripMenuItem,
									this.pasteCHRToolStripMenuItem,
									this.separatorToolStripMenuItem2,
									this.fillWithColorCHRToolStripMenuItem,
									this.toolStripSeparator23,
									this.insertLeftCHRToolStripMenuItem,
									this.deleteCHRToolStripMenuItem});
			this.ContextMenuCHRBank.Name = "contextMenuCHRBank";
			this.ContextMenuCHRBank.Size = new System.Drawing.Size(150, 126);
			// 
			// copyCHRToolStripMenuItem
			// 
			this.copyCHRToolStripMenuItem.Name = "copyCHRToolStripMenuItem";
			this.copyCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.copyCHRToolStripMenuItem.Text = "Copy";
			this.copyCHRToolStripMenuItem.Click += new System.EventHandler(this.CopyCHRToolStripMenuItemClick_Event);
			// 
			// pasteCHRToolStripMenuItem
			// 
			this.pasteCHRToolStripMenuItem.Name = "pasteCHRToolStripMenuItem";
			this.pasteCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.pasteCHRToolStripMenuItem.Text = "Paste";
			this.pasteCHRToolStripMenuItem.Click += new System.EventHandler(this.PasteCHRToolStripMenuItemClick_Event);
			// 
			// separatorToolStripMenuItem2
			// 
			this.separatorToolStripMenuItem2.Name = "separatorToolStripMenuItem2";
			this.separatorToolStripMenuItem2.Size = new System.Drawing.Size(146, 6);
			// 
			// fillWithColorCHRToolStripMenuItem
			// 
			this.fillWithColorCHRToolStripMenuItem.Name = "fillWithColorCHRToolStripMenuItem";
			this.fillWithColorCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.fillWithColorCHRToolStripMenuItem.Text = "Fill With Color";
			this.fillWithColorCHRToolStripMenuItem.Click += new System.EventHandler(this.FillWithColorCHRToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator23
			// 
			this.toolStripSeparator23.Name = "toolStripSeparator23";
			this.toolStripSeparator23.Size = new System.Drawing.Size(146, 6);
			// 
			// insertLeftCHRToolStripMenuItem
			// 
			this.insertLeftCHRToolStripMenuItem.Name = "insertLeftCHRToolStripMenuItem";
			this.insertLeftCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.insertLeftCHRToolStripMenuItem.Text = "Insert Left";
			this.insertLeftCHRToolStripMenuItem.Click += new System.EventHandler(this.InsertLeftCHRToolStripMenuItemClick_Event);
			// 
			// deleteCHRToolStripMenuItem
			// 
			this.deleteCHRToolStripMenuItem.Name = "deleteCHRToolStripMenuItem";
			this.deleteCHRToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
			this.deleteCHRToolStripMenuItem.Text = "Delete";
			this.deleteCHRToolStripMenuItem.Click += new System.EventHandler(this.DeleteCHRToolStripMenuItemClick_Event);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.Silver;
			this.groupBox2.Controls.Add(this.PanelBlocks);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 3, 6, 6);
			this.groupBox2.Size = new System.Drawing.Size(369, 615);
			this.groupBox2.TabIndex = 42;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Blocks:";
			// 
			// PanelBlocks
			// 
			this.PanelBlocks.AutoScroll = true;
			this.PanelBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelBlocks.Cursor = System.Windows.Forms.Cursors.No;
			this.PanelBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelBlocks.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.PanelBlocks.Location = new System.Drawing.Point(6, 16);
			this.PanelBlocks.Name = "PanelBlocks";
			this.PanelBlocks.Size = new System.Drawing.Size(357, 593);
			this.PanelBlocks.TabIndex = 43;
			// 
			// TabLayout
			// 
			this.TabLayout.BackColor = System.Drawing.Color.Silver;
			this.TabLayout.Controls.Add(this.splitContainer5);
			this.TabLayout.Location = new System.Drawing.Point(4, 22);
			this.TabLayout.Name = "TabLayout";
			this.TabLayout.Padding = new System.Windows.Forms.Padding(3);
			this.TabLayout.Size = new System.Drawing.Size(1236, 621);
			this.TabLayout.TabIndex = 1;
			this.TabLayout.Text = "Layout";
			// 
			// splitContainer5
			// 
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer5.IsSplitterFixed = true;
			this.splitContainer5.Location = new System.Drawing.Point(3, 3);
			this.splitContainer5.Name = "splitContainer5";
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.Controls.Add(this.tabControlLayoutTools);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.Controls.Add(this.PBoxLayout);
			this.splitContainer5.Size = new System.Drawing.Size(1230, 615);
			this.splitContainer5.SplitterDistance = 302;
			this.splitContainer5.TabIndex = 0;
			// 
			// tabControlLayoutTools
			// 
			this.tabControlLayoutTools.Controls.Add(this.TabBuilder);
			this.tabControlLayoutTools.Controls.Add(this.TabPainter);
			this.tabControlLayoutTools.Controls.Add(this.TabScreenList);
			this.tabControlLayoutTools.Controls.Add(this.TabEntities);
			this.tabControlLayoutTools.Controls.Add(this.TabPatterns);
			this.tabControlLayoutTools.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlLayoutTools.Location = new System.Drawing.Point(0, 0);
			this.tabControlLayoutTools.Name = "tabControlLayoutTools";
			this.tabControlLayoutTools.SelectedIndex = 0;
			this.tabControlLayoutTools.Size = new System.Drawing.Size(302, 615);
			this.tabControlLayoutTools.TabIndex = 61;
			this.tabControlLayoutTools.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControlLayoutToolsSelected_Event);
			// 
			// TabBuilder
			// 
			this.TabBuilder.BackColor = System.Drawing.Color.Silver;
			this.TabBuilder.Controls.Add(this.groupBox1);
			this.TabBuilder.Location = new System.Drawing.Point(4, 22);
			this.TabBuilder.Name = "TabBuilder";
			this.TabBuilder.Size = new System.Drawing.Size(294, 589);
			this.TabBuilder.TabIndex = 2;
			this.TabBuilder.Text = "Builder";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.BtnLayoutDeleteEmptyScreens);
			this.groupBox1.Controls.Add(this.groupBox7);
			this.groupBox1.Controls.Add(this.groupBox13);
			this.groupBox1.Controls.Add(this.BtnLayoutRemoveRightColumn);
			this.groupBox1.Controls.Add(this.BtnLayoutRemoveUpRow);
			this.groupBox1.Controls.Add(this.BtnLayoutAddDownRow);
			this.groupBox1.Controls.Add(this.BtnCreateLayoutWxH);
			this.groupBox1.Controls.Add(this.BtnLayoutAddRightColumn);
			this.groupBox1.Controls.Add(this.BtnCopyLayout);
			this.groupBox1.Controls.Add(this.BtnLayoutAddUpRow);
			this.groupBox1.Controls.Add(this.BtnDeleteLayout);
			this.groupBox1.Controls.Add(this.BtnLayoutRemoveDownRow);
			this.groupBox1.Controls.Add(this.BtnLayoutAddLeftColumn);
			this.groupBox1.Controls.Add(this.BtnLayoutRemoveLeftColumn);
			this.groupBox1.Location = new System.Drawing.Point(5, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(284, 323);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// BtnLayoutDeleteEmptyScreens
			// 
			this.BtnLayoutDeleteEmptyScreens.Location = new System.Drawing.Point(192, 134);
			this.BtnLayoutDeleteEmptyScreens.Name = "BtnLayoutDeleteEmptyScreens";
			this.BtnLayoutDeleteEmptyScreens.Size = new System.Drawing.Size(84, 23);
			this.BtnLayoutDeleteEmptyScreens.TabIndex = 17;
			this.BtnLayoutDeleteEmptyScreens.Text = "Clean Up";
			this.BtnLayoutDeleteEmptyScreens.UseVisualStyleBackColor = true;
			this.BtnLayoutDeleteEmptyScreens.Click += new System.EventHandler(this.BtnLayoutDeleteEmptyScreensClick_Event);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.BtnLayoutMoveDown);
			this.groupBox7.Controls.Add(this.BtnLayoutMoveUp);
			this.groupBox7.Controls.Add(this.ListBoxLayouts);
			this.groupBox7.Controls.Add(this.LayoutLabel);
			this.groupBox7.Location = new System.Drawing.Point(100, 13);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(85, 299);
			this.groupBox7.TabIndex = 4;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Layouts:";
			// 
			// BtnLayoutMoveDown
			// 
			this.BtnLayoutMoveDown.Location = new System.Drawing.Point(9, 266);
			this.BtnLayoutMoveDown.Name = "BtnLayoutMoveDown";
			this.BtnLayoutMoveDown.Size = new System.Drawing.Size(67, 23);
			this.BtnLayoutMoveDown.TabIndex = 8;
			this.BtnLayoutMoveDown.Text = "Move Dn";
			this.BtnLayoutMoveDown.UseVisualStyleBackColor = true;
			this.BtnLayoutMoveDown.Click += new System.EventHandler(this.BtnLayoutMoveDownClick_Event);
			// 
			// BtnLayoutMoveUp
			// 
			this.BtnLayoutMoveUp.Location = new System.Drawing.Point(9, 239);
			this.BtnLayoutMoveUp.Name = "BtnLayoutMoveUp";
			this.BtnLayoutMoveUp.Size = new System.Drawing.Size(67, 23);
			this.BtnLayoutMoveUp.TabIndex = 7;
			this.BtnLayoutMoveUp.Text = "Move Up";
			this.BtnLayoutMoveUp.UseVisualStyleBackColor = true;
			this.BtnLayoutMoveUp.Click += new System.EventHandler(this.BtnLayoutMoveUpClick_Event);
			// 
			// ListBoxLayouts
			// 
			this.ListBoxLayouts.BackColor = System.Drawing.Color.LightGray;
			this.ListBoxLayouts.FormattingEnabled = true;
			this.ListBoxLayouts.Location = new System.Drawing.Point(9, 43);
			this.ListBoxLayouts.Name = "ListBoxLayouts";
			this.ListBoxLayouts.Size = new System.Drawing.Size(67, 186);
			this.ListBoxLayouts.TabIndex = 6;
			this.ListBoxLayouts.SelectedIndexChanged += new System.EventHandler(this.ListBoxLayoutsClick_Event);
			// 
			// LayoutLabel
			// 
			this.LayoutLabel.Location = new System.Drawing.Point(6, 19);
			this.LayoutLabel.Name = "LayoutLabel";
			this.LayoutLabel.Size = new System.Drawing.Size(73, 14);
			this.LayoutLabel.TabIndex = 5;
			this.LayoutLabel.Text = "...";
			this.LayoutLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// groupBox13
			// 
			this.groupBox13.Controls.Add(this.CheckBoxShowGrid);
			this.groupBox13.Controls.Add(this.CheckBoxShowCoords);
			this.groupBox13.Controls.Add(this.CheckBoxShowTargets);
			this.groupBox13.Controls.Add(this.CheckBoxShowMarks);
			this.groupBox13.Controls.Add(this.CheckBoxShowEntities);
			this.groupBox13.Location = new System.Drawing.Point(196, 190);
			this.groupBox13.Name = "groupBox13";
			this.groupBox13.Size = new System.Drawing.Size(80, 122);
			this.groupBox13.TabIndex = 18;
			this.groupBox13.TabStop = false;
			this.groupBox13.Text = "Show";
			// 
			// CheckBoxShowGrid
			// 
			this.CheckBoxShowGrid.Location = new System.Drawing.Point(11, 99);
			this.CheckBoxShowGrid.Name = "CheckBoxShowGrid";
			this.CheckBoxShowGrid.Size = new System.Drawing.Size(60, 17);
			this.CheckBoxShowGrid.TabIndex = 23;
			this.CheckBoxShowGrid.Text = "Grid";
			this.CheckBoxShowGrid.UseVisualStyleBackColor = true;
			this.CheckBoxShowGrid.CheckedChanged += new System.EventHandler(this.CheckBoxShowGridChecked_Event);
			// 
			// CheckBoxShowCoords
			// 
			this.CheckBoxShowCoords.Location = new System.Drawing.Point(11, 79);
			this.CheckBoxShowCoords.Name = "CheckBoxShowCoords";
			this.CheckBoxShowCoords.Size = new System.Drawing.Size(63, 17);
			this.CheckBoxShowCoords.TabIndex = 22;
			this.CheckBoxShowCoords.Text = "Coords";
			this.CheckBoxShowCoords.UseVisualStyleBackColor = true;
			this.CheckBoxShowCoords.CheckedChanged += new System.EventHandler(this.CheckBoxShowCoordsChecked_Event);
			// 
			// CheckBoxShowTargets
			// 
			this.CheckBoxShowTargets.Location = new System.Drawing.Point(11, 59);
			this.CheckBoxShowTargets.Name = "CheckBoxShowTargets";
			this.CheckBoxShowTargets.Size = new System.Drawing.Size(63, 17);
			this.CheckBoxShowTargets.TabIndex = 21;
			this.CheckBoxShowTargets.Text = "Targets";
			this.CheckBoxShowTargets.UseVisualStyleBackColor = true;
			this.CheckBoxShowTargets.CheckedChanged += new System.EventHandler(this.CheckBoxShowTargetsChecked_Event);
			// 
			// CheckBoxShowMarks
			// 
			this.CheckBoxShowMarks.Location = new System.Drawing.Point(11, 19);
			this.CheckBoxShowMarks.Name = "CheckBoxShowMarks";
			this.CheckBoxShowMarks.Size = new System.Drawing.Size(60, 17);
			this.CheckBoxShowMarks.TabIndex = 19;
			this.CheckBoxShowMarks.Text = "Marks";
			this.CheckBoxShowMarks.UseVisualStyleBackColor = true;
			this.CheckBoxShowMarks.CheckedChanged += new System.EventHandler(this.CheckBoxShowMarksChecked_Event);
			// 
			// CheckBoxShowEntities
			// 
			this.CheckBoxShowEntities.Location = new System.Drawing.Point(11, 39);
			this.CheckBoxShowEntities.Name = "CheckBoxShowEntities";
			this.CheckBoxShowEntities.Size = new System.Drawing.Size(60, 17);
			this.CheckBoxShowEntities.TabIndex = 20;
			this.CheckBoxShowEntities.Text = "Entities";
			this.CheckBoxShowEntities.UseVisualStyleBackColor = true;
			this.CheckBoxShowEntities.CheckedChanged += new System.EventHandler(this.CheckBoxShowEntitiesChecked_Event);
			// 
			// BtnLayoutRemoveRightColumn
			// 
			this.BtnLayoutRemoveRightColumn.Location = new System.Drawing.Point(236, 105);
			this.BtnLayoutRemoveRightColumn.Name = "BtnLayoutRemoveRightColumn";
			this.BtnLayoutRemoveRightColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveRightColumn.TabIndex = 16;
			this.BtnLayoutRemoveRightColumn.Text = "-R";
			this.BtnLayoutRemoveRightColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveRightColumn.Click += new System.EventHandler(this.BtnLayoutRemoveRightColumnClick_Event);
			// 
			// BtnLayoutRemoveUpRow
			// 
			this.BtnLayoutRemoveUpRow.Location = new System.Drawing.Point(236, 18);
			this.BtnLayoutRemoveUpRow.Name = "BtnLayoutRemoveUpRow";
			this.BtnLayoutRemoveUpRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveUpRow.TabIndex = 10;
			this.BtnLayoutRemoveUpRow.Text = "-U";
			this.BtnLayoutRemoveUpRow.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveUpRow.Click += new System.EventHandler(this.BtnLayoutRemoveTopRowClick_Event);
			// 
			// BtnLayoutAddDownRow
			// 
			this.BtnLayoutAddDownRow.Location = new System.Drawing.Point(192, 47);
			this.BtnLayoutAddDownRow.Name = "BtnLayoutAddDownRow";
			this.BtnLayoutAddDownRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddDownRow.TabIndex = 11;
			this.BtnLayoutAddDownRow.Text = "+D";
			this.BtnLayoutAddDownRow.UseVisualStyleBackColor = true;
			this.BtnLayoutAddDownRow.Click += new System.EventHandler(this.BtnLayoutAddDownRowClick_Event);
			// 
			// BtnCreateLayoutWxH
			// 
			this.BtnCreateLayoutWxH.Location = new System.Drawing.Point(11, 19);
			this.BtnCreateLayoutWxH.Name = "BtnCreateLayoutWxH";
			this.BtnCreateLayoutWxH.Size = new System.Drawing.Size(80, 23);
			this.BtnCreateLayoutWxH.TabIndex = 1;
			this.BtnCreateLayoutWxH.Text = "Create WxH";
			this.BtnCreateLayoutWxH.UseVisualStyleBackColor = true;
			this.BtnCreateLayoutWxH.Click += new System.EventHandler(this.BtnCreateLayoutWxHClick_Event);
			// 
			// BtnLayoutAddRightColumn
			// 
			this.BtnLayoutAddRightColumn.Location = new System.Drawing.Point(192, 105);
			this.BtnLayoutAddRightColumn.Name = "BtnLayoutAddRightColumn";
			this.BtnLayoutAddRightColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddRightColumn.TabIndex = 15;
			this.BtnLayoutAddRightColumn.Text = "+R";
			this.BtnLayoutAddRightColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutAddRightColumn.Click += new System.EventHandler(this.BtnLayoutAddRightColumnClick_Event);
			// 
			// BtnCopyLayout
			// 
			this.BtnCopyLayout.Location = new System.Drawing.Point(11, 48);
			this.BtnCopyLayout.Name = "BtnCopyLayout";
			this.BtnCopyLayout.Size = new System.Drawing.Size(80, 23);
			this.BtnCopyLayout.TabIndex = 2;
			this.BtnCopyLayout.Text = "Copy";
			this.BtnCopyLayout.UseVisualStyleBackColor = true;
			this.BtnCopyLayout.Click += new System.EventHandler(this.BtnCopyLayoutClick_Event);
			// 
			// BtnLayoutAddUpRow
			// 
			this.BtnLayoutAddUpRow.Location = new System.Drawing.Point(192, 18);
			this.BtnLayoutAddUpRow.Name = "BtnLayoutAddUpRow";
			this.BtnLayoutAddUpRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddUpRow.TabIndex = 9;
			this.BtnLayoutAddUpRow.Text = "+U";
			this.BtnLayoutAddUpRow.UseVisualStyleBackColor = true;
			this.BtnLayoutAddUpRow.Click += new System.EventHandler(this.BtnLayoutAddUpRowClick_Event);
			// 
			// BtnDeleteLayout
			// 
			this.BtnDeleteLayout.Location = new System.Drawing.Point(11, 77);
			this.BtnDeleteLayout.Name = "BtnDeleteLayout";
			this.BtnDeleteLayout.Size = new System.Drawing.Size(80, 23);
			this.BtnDeleteLayout.TabIndex = 3;
			this.BtnDeleteLayout.Text = "Delete";
			this.BtnDeleteLayout.UseVisualStyleBackColor = true;
			this.BtnDeleteLayout.Click += new System.EventHandler(this.BtnDeleteLayoutClick_Event);
			// 
			// BtnLayoutRemoveDownRow
			// 
			this.BtnLayoutRemoveDownRow.Location = new System.Drawing.Point(236, 47);
			this.BtnLayoutRemoveDownRow.Name = "BtnLayoutRemoveDownRow";
			this.BtnLayoutRemoveDownRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveDownRow.TabIndex = 12;
			this.BtnLayoutRemoveDownRow.Text = "-D";
			this.BtnLayoutRemoveDownRow.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveDownRow.Click += new System.EventHandler(this.BtnLayoutRemoveBottomRowClick_Event);
			// 
			// BtnLayoutAddLeftColumn
			// 
			this.BtnLayoutAddLeftColumn.Location = new System.Drawing.Point(192, 76);
			this.BtnLayoutAddLeftColumn.Name = "BtnLayoutAddLeftColumn";
			this.BtnLayoutAddLeftColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddLeftColumn.TabIndex = 13;
			this.BtnLayoutAddLeftColumn.Text = "+L";
			this.BtnLayoutAddLeftColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutAddLeftColumn.Click += new System.EventHandler(this.BtnLayoutAddLeftColumnClick_Event);
			// 
			// BtnLayoutRemoveLeftColumn
			// 
			this.BtnLayoutRemoveLeftColumn.Location = new System.Drawing.Point(236, 76);
			this.BtnLayoutRemoveLeftColumn.Name = "BtnLayoutRemoveLeftColumn";
			this.BtnLayoutRemoveLeftColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveLeftColumn.TabIndex = 14;
			this.BtnLayoutRemoveLeftColumn.Text = "-L";
			this.BtnLayoutRemoveLeftColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveLeftColumn.Click += new System.EventHandler(this.BtnLayoutRemoveLeftColumnClick_Event);
			// 
			// TabPainter
			// 
			this.TabPainter.BackColor = System.Drawing.Color.Silver;
			this.TabPainter.Controls.Add(this.groupBox6);
			this.TabPainter.Controls.Add(this.GrpBoxPainter);
			this.TabPainter.Location = new System.Drawing.Point(4, 22);
			this.TabPainter.Name = "TabPainter";
			this.TabPainter.Size = new System.Drawing.Size(294, 589);
			this.TabPainter.TabIndex = 3;
			this.TabPainter.Text = "Painter";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.CheckBoxPainterReplaceTiles);
			this.groupBox6.Controls.Add(this.BtnPainterFillWithTile);
			this.groupBox6.Location = new System.Drawing.Point(5, 126);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(284, 52);
			this.groupBox6.TabIndex = 7;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Selected Screens";
			// 
			// CheckBoxPainterReplaceTiles
			// 
			this.CheckBoxPainterReplaceTiles.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxPainterReplaceTiles.Location = new System.Drawing.Point(144, 19);
			this.CheckBoxPainterReplaceTiles.Name = "CheckBoxPainterReplaceTiles";
			this.CheckBoxPainterReplaceTiles.Size = new System.Drawing.Size(130, 23);
			this.CheckBoxPainterReplaceTiles.TabIndex = 9;
			this.CheckBoxPainterReplaceTiles.Text = "Replace Tiles";
			this.CheckBoxPainterReplaceTiles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxPainterReplaceTiles.UseVisualStyleBackColor = true;
			this.CheckBoxPainterReplaceTiles.CheckedChanged += new System.EventHandler(this.CheckBoxPainterReplaceTilesChecked_Event);
			// 
			// BtnPainterFillWithTile
			// 
			this.BtnPainterFillWithTile.Location = new System.Drawing.Point(10, 19);
			this.BtnPainterFillWithTile.Name = "BtnPainterFillWithTile";
			this.BtnPainterFillWithTile.Size = new System.Drawing.Size(130, 23);
			this.BtnPainterFillWithTile.TabIndex = 8;
			this.BtnPainterFillWithTile.Text = "Fill With Tile";
			this.BtnPainterFillWithTile.UseVisualStyleBackColor = true;
			this.BtnPainterFillWithTile.Click += new System.EventHandler(this.BtnPainterFillWithTileClick_Event);
			// 
			// GrpBoxPainter
			// 
			this.GrpBoxPainter.Controls.Add(this.groupBox3);
			this.GrpBoxPainter.Controls.Add(this.GrpBoxActiveTile);
			this.GrpBoxPainter.Location = new System.Drawing.Point(5, 5);
			this.GrpBoxPainter.Name = "GrpBoxPainter";
			this.GrpBoxPainter.Size = new System.Drawing.Size(284, 115);
			this.GrpBoxPainter.TabIndex = 0;
			this.GrpBoxPainter.TabStop = false;
			this.GrpBoxPainter.Text = "Data Type: ...";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.RBtnMapScaleX2);
			this.groupBox3.Controls.Add(this.RBtnMapScaleX1);
			this.groupBox3.Location = new System.Drawing.Point(10, 14);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(92, 91);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Scene Scale";
			// 
			// RBtnMapScaleX2
			// 
			this.RBtnMapScaleX2.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnMapScaleX2.Location = new System.Drawing.Point(9, 52);
			this.RBtnMapScaleX2.Name = "RBtnMapScaleX2";
			this.RBtnMapScaleX2.Size = new System.Drawing.Size(74, 30);
			this.RBtnMapScaleX2.TabIndex = 3;
			this.RBtnMapScaleX2.Text = "x2";
			this.RBtnMapScaleX2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnMapScaleX2.UseVisualStyleBackColor = true;
			this.RBtnMapScaleX2.CheckedChanged += new System.EventHandler(this.RBtnMapScaleX2CheckedChanged_Event);
			// 
			// RBtnMapScaleX1
			// 
			this.RBtnMapScaleX1.Appearance = System.Windows.Forms.Appearance.Button;
			this.RBtnMapScaleX1.Checked = true;
			this.RBtnMapScaleX1.Location = new System.Drawing.Point(9, 16);
			this.RBtnMapScaleX1.Name = "RBtnMapScaleX1";
			this.RBtnMapScaleX1.Size = new System.Drawing.Size(74, 30);
			this.RBtnMapScaleX1.TabIndex = 2;
			this.RBtnMapScaleX1.TabStop = true;
			this.RBtnMapScaleX1.Text = "x1";
			this.RBtnMapScaleX1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RBtnMapScaleX1.UseVisualStyleBackColor = true;
			this.RBtnMapScaleX1.CheckedChanged += new System.EventHandler(this.RBtnMapScaleX1CheckedChanged_Event);
			// 
			// GrpBoxActiveTile
			// 
			this.GrpBoxActiveTile.Controls.Add(this.PBoxActiveTile);
			this.GrpBoxActiveTile.Controls.Add(this.BtnTilesBlocks);
			this.GrpBoxActiveTile.Controls.Add(this.BtnResetTile);
			this.GrpBoxActiveTile.Location = new System.Drawing.Point(108, 14);
			this.GrpBoxActiveTile.Name = "GrpBoxActiveTile";
			this.GrpBoxActiveTile.Size = new System.Drawing.Size(166, 91);
			this.GrpBoxActiveTile.TabIndex = 4;
			this.GrpBoxActiveTile.TabStop = false;
			this.GrpBoxActiveTile.Text = "...";
			// 
			// PBoxActiveTile
			// 
			this.PBoxActiveTile.BackColor = System.Drawing.Color.Black;
			this.PBoxActiveTile.Location = new System.Drawing.Point(91, 17);
			this.PBoxActiveTile.Name = "PBoxActiveTile";
			this.PBoxActiveTile.Size = new System.Drawing.Size(64, 64);
			this.PBoxActiveTile.TabIndex = 6;
			this.PBoxActiveTile.TabStop = false;
			// 
			// BtnTilesBlocks
			// 
			this.BtnTilesBlocks.Location = new System.Drawing.Point(9, 16);
			this.BtnTilesBlocks.Name = "BtnTilesBlocks";
			this.BtnTilesBlocks.Size = new System.Drawing.Size(75, 37);
			this.BtnTilesBlocks.TabIndex = 5;
			this.BtnTilesBlocks.Text = "Tiles/Blocks";
			this.BtnTilesBlocks.UseVisualStyleBackColor = true;
			this.BtnTilesBlocks.Click += new System.EventHandler(this.BtnTilesBlocksClick_Event);
			// 
			// BtnResetTile
			// 
			this.BtnResetTile.Location = new System.Drawing.Point(9, 59);
			this.BtnResetTile.Name = "BtnResetTile";
			this.BtnResetTile.Size = new System.Drawing.Size(75, 23);
			this.BtnResetTile.TabIndex = 6;
			this.BtnResetTile.Text = "Cancel";
			this.BtnResetTile.UseVisualStyleBackColor = true;
			this.BtnResetTile.Click += new System.EventHandler(this.BtnResetTileClick_Event);
			// 
			// TabScreenList
			// 
			this.TabScreenList.BackColor = System.Drawing.Color.LightGray;
			this.TabScreenList.Controls.Add(this.splitContainer6);
			this.TabScreenList.Location = new System.Drawing.Point(4, 22);
			this.TabScreenList.Name = "TabScreenList";
			this.TabScreenList.Padding = new System.Windows.Forms.Padding(3);
			this.TabScreenList.Size = new System.Drawing.Size(294, 589);
			this.TabScreenList.TabIndex = 0;
			this.TabScreenList.Text = "Screens";
			// 
			// splitContainer6
			// 
			this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer6.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer6.IsSplitterFixed = true;
			this.splitContainer6.Location = new System.Drawing.Point(3, 3);
			this.splitContainer6.Name = "splitContainer6";
			this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer6.Panel1
			// 
			this.splitContainer6.Panel1.Controls.Add(this.CheckBoxLayoutEditorAllBanks);
			this.splitContainer6.Panel1.Controls.Add(this.BtnUpdateScreens);
			this.splitContainer6.Panel1.Controls.Add(this.LabelLayoutEditorCHRBankID);
			this.splitContainer6.Panel1.Controls.Add(this.label9);
			this.splitContainer6.Panel1.Controls.Add(this.CheckBoxScreensAutoUpdate);
			// 
			// splitContainer6.Panel2
			// 
			this.splitContainer6.Panel2.Controls.Add(this.ListViewScreens);
			this.splitContainer6.Size = new System.Drawing.Size(288, 583);
			this.splitContainer6.SplitterDistance = 28;
			this.splitContainer6.TabIndex = 0;
			// 
			// CheckBoxLayoutEditorAllBanks
			// 
			this.CheckBoxLayoutEditorAllBanks.Location = new System.Drawing.Point(218, 5);
			this.CheckBoxLayoutEditorAllBanks.Name = "CheckBoxLayoutEditorAllBanks";
			this.CheckBoxLayoutEditorAllBanks.Size = new System.Drawing.Size(73, 20);
			this.CheckBoxLayoutEditorAllBanks.TabIndex = 4;
			this.CheckBoxLayoutEditorAllBanks.Text = "All Banks";
			this.CheckBoxLayoutEditorAllBanks.UseVisualStyleBackColor = true;
			this.CheckBoxLayoutEditorAllBanks.CheckedChanged += new System.EventHandler(this.CheckBoxLayoutEditorAllBanksCheckChanged_Event);
			// 
			// BtnUpdateScreens
			// 
			this.BtnUpdateScreens.BackColor = System.Drawing.Color.Coral;
			this.BtnUpdateScreens.Location = new System.Drawing.Point(3, 3);
			this.BtnUpdateScreens.Name = "BtnUpdateScreens";
			this.BtnUpdateScreens.Size = new System.Drawing.Size(94, 23);
			this.BtnUpdateScreens.TabIndex = 0;
			this.BtnUpdateScreens.Text = "Update Screens";
			this.BtnUpdateScreens.UseVisualStyleBackColor = true;
			this.BtnUpdateScreens.Click += new System.EventHandler(this.BtnUpdateScreensClick_Event);
			// 
			// LabelLayoutEditorCHRBankID
			// 
			this.LabelLayoutEditorCHRBankID.Location = new System.Drawing.Point(184, 7);
			this.LabelLayoutEditorCHRBankID.Name = "LabelLayoutEditorCHRBankID";
			this.LabelLayoutEditorCHRBankID.Size = new System.Drawing.Size(29, 15);
			this.LabelLayoutEditorCHRBankID.TabIndex = 3;
			this.LabelLayoutEditorCHRBankID.Text = "000";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(151, 7);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(39, 18);
			this.label9.TabIndex = 2;
			this.label9.Text = "Bank:";
			// 
			// CheckBoxScreensAutoUpdate
			// 
			this.CheckBoxScreensAutoUpdate.Location = new System.Drawing.Point(102, 3);
			this.CheckBoxScreensAutoUpdate.Name = "CheckBoxScreensAutoUpdate";
			this.CheckBoxScreensAutoUpdate.Size = new System.Drawing.Size(50, 24);
			this.CheckBoxScreensAutoUpdate.TabIndex = 1;
			this.CheckBoxScreensAutoUpdate.Text = "Auto";
			this.CheckBoxScreensAutoUpdate.UseVisualStyleBackColor = true;
			this.CheckBoxScreensAutoUpdate.CheckedChanged += new System.EventHandler(this.CheckBoxScreensAutoUpdateChanged_Event);
			// 
			// ListViewScreens
			// 
			this.ListViewScreens.BackColor = System.Drawing.Color.Silver;
			this.ListViewScreens.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ListViewScreens.HideSelection = false;
			this.ListViewScreens.Location = new System.Drawing.Point(0, 0);
			this.ListViewScreens.MultiSelect = false;
			this.ListViewScreens.Name = "ListViewScreens";
			this.ListViewScreens.Size = new System.Drawing.Size(288, 551);
			this.ListViewScreens.TabIndex = 5;
			this.ListViewScreens.UseCompatibleStateImageBehavior = false;
			this.ListViewScreens.SelectedIndexChanged += new System.EventHandler(this.ListViewScreensClick_Event);
			// 
			// TabEntities
			// 
			this.TabEntities.BackColor = System.Drawing.Color.Silver;
			this.TabEntities.Controls.Add(this.splitContainer7);
			this.TabEntities.Location = new System.Drawing.Point(4, 22);
			this.TabEntities.Name = "TabEntities";
			this.TabEntities.Padding = new System.Windows.Forms.Padding(3);
			this.TabEntities.Size = new System.Drawing.Size(294, 589);
			this.TabEntities.TabIndex = 1;
			this.TabEntities.Text = "Entities";
			// 
			// splitContainer7
			// 
			this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer7.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer7.IsSplitterFixed = true;
			this.splitContainer7.Location = new System.Drawing.Point(3, 3);
			this.splitContainer7.Name = "splitContainer7";
			this.splitContainer7.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer7.Panel1
			// 
			this.splitContainer7.Panel1.Controls.Add(this.splitContainer8);
			this.splitContainer7.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// splitContainer7.Panel2
			// 
			this.splitContainer7.Panel2.Controls.Add(this.groupBoxEntityEditor);
			this.splitContainer7.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainer7.Size = new System.Drawing.Size(288, 583);
			this.splitContainer7.SplitterDistance = 289;
			this.splitContainer7.TabIndex = 0;
			// 
			// splitContainer8
			// 
			this.splitContainer8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer8.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer8.IsSplitterFixed = true;
			this.splitContainer8.Location = new System.Drawing.Point(0, 0);
			this.splitContainer8.Name = "splitContainer8";
			// 
			// splitContainer8.Panel1
			// 
			this.splitContainer8.Panel1.Controls.Add(this.TreeViewEntities);
			// 
			// splitContainer8.Panel2
			// 
			this.splitContainer8.Panel2.Controls.Add(this.splitContainer9);
			this.splitContainer8.Size = new System.Drawing.Size(288, 289);
			this.splitContainer8.SplitterDistance = 203;
			this.splitContainer8.TabIndex = 0;
			// 
			// TreeViewEntities
			// 
			this.TreeViewEntities.AllowDrop = true;
			this.TreeViewEntities.BackColor = System.Drawing.Color.LightGray;
			this.TreeViewEntities.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TreeViewEntities.ContextMenuStrip = this.ContextMenuEntitiesTree;
			this.TreeViewEntities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TreeViewEntities.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
			this.TreeViewEntities.HideSelection = false;
			this.TreeViewEntities.LabelEdit = true;
			this.TreeViewEntities.Location = new System.Drawing.Point(0, 0);
			this.TreeViewEntities.Name = "TreeViewEntities";
			this.TreeViewEntities.Size = new System.Drawing.Size(203, 289);
			this.TreeViewEntities.TabIndex = 101;
			this.TreeViewEntities.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeViewEntitiesBeforeLabelEdit_Event);
			this.TreeViewEntities.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeViewEntitiesAfterLabelEdit_Event);
			this.TreeViewEntities.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.TreeViewEntitiesDrawNode_Event);
			this.TreeViewEntities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewEntitiesSelect_Event);
			this.TreeViewEntities.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewEntitiesNodeMouseClick_Event);
			this.TreeViewEntities.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown_Event);
			// 
			// ContextMenuEntitiesTree
			// 
			this.ContextMenuEntitiesTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addGroupToolStripMenuItem});
			this.ContextMenuEntitiesTree.Name = "ContextMenuEntitiesTree";
			this.ContextMenuEntitiesTree.Size = new System.Drawing.Size(133, 26);
			// 
			// addGroupToolStripMenuItem
			// 
			this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
			this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
			this.addGroupToolStripMenuItem.Text = "&Add Group";
			this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupAddClick_Event);
			// 
			// splitContainer9
			// 
			this.splitContainer9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer9.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer9.IsSplitterFixed = true;
			this.splitContainer9.Location = new System.Drawing.Point(0, 0);
			this.splitContainer9.Name = "splitContainer9";
			this.splitContainer9.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer9.Panel1
			// 
			this.splitContainer9.Panel1.Controls.Add(this.BtnEntityRename);
			this.splitContainer9.Panel1.Controls.Add(this.groupBox9);
			this.splitContainer9.Panel1.Controls.Add(this.groupBox10);
			// 
			// splitContainer9.Panel2
			// 
			this.splitContainer9.Panel2.Controls.Add(this.BtnEntitiesEditInstancesMode);
			this.splitContainer9.Panel2.Controls.Add(this.CheckBoxEntitySnapping);
			this.splitContainer9.Size = new System.Drawing.Size(81, 289);
			this.splitContainer9.SplitterDistance = 217;
			this.splitContainer9.TabIndex = 0;
			// 
			// BtnEntityRename
			// 
			this.BtnEntityRename.Location = new System.Drawing.Point(12, 171);
			this.BtnEntityRename.Name = "BtnEntityRename";
			this.BtnEntityRename.Size = new System.Drawing.Size(59, 23);
			this.BtnEntityRename.TabIndex = 108;
			this.BtnEntityRename.Text = "Rename";
			this.BtnEntityRename.UseVisualStyleBackColor = true;
			this.BtnEntityRename.Click += new System.EventHandler(this.BtnEntityRenameClick_Event);
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.BtnEntityGroupAdd);
			this.groupBox9.Controls.Add(this.BtnEntityGroupDelete);
			this.groupBox9.Location = new System.Drawing.Point(4, 3);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(74, 80);
			this.groupBox9.TabIndex = 102;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Group";
			// 
			// BtnEntityGroupAdd
			// 
			this.BtnEntityGroupAdd.Location = new System.Drawing.Point(8, 19);
			this.BtnEntityGroupAdd.Name = "BtnEntityGroupAdd";
			this.BtnEntityGroupAdd.Size = new System.Drawing.Size(59, 23);
			this.BtnEntityGroupAdd.TabIndex = 103;
			this.BtnEntityGroupAdd.Text = "Add";
			this.BtnEntityGroupAdd.UseVisualStyleBackColor = true;
			this.BtnEntityGroupAdd.Click += new System.EventHandler(this.BtnEntityGroupAddClick_Event);
			// 
			// BtnEntityGroupDelete
			// 
			this.BtnEntityGroupDelete.Location = new System.Drawing.Point(8, 48);
			this.BtnEntityGroupDelete.Name = "BtnEntityGroupDelete";
			this.BtnEntityGroupDelete.Size = new System.Drawing.Size(59, 23);
			this.BtnEntityGroupDelete.TabIndex = 104;
			this.BtnEntityGroupDelete.Text = "Delete";
			this.BtnEntityGroupDelete.UseVisualStyleBackColor = true;
			this.BtnEntityGroupDelete.Click += new System.EventHandler(this.BtnEntityGroupDeleteClick_Event);
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.BtnEntityAdd);
			this.groupBox10.Controls.Add(this.BtnEntityDelete);
			this.groupBox10.Location = new System.Drawing.Point(4, 85);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(74, 80);
			this.groupBox10.TabIndex = 105;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Entity";
			// 
			// BtnEntityAdd
			// 
			this.BtnEntityAdd.Location = new System.Drawing.Point(8, 19);
			this.BtnEntityAdd.Name = "BtnEntityAdd";
			this.BtnEntityAdd.Size = new System.Drawing.Size(59, 23);
			this.BtnEntityAdd.TabIndex = 106;
			this.BtnEntityAdd.Text = "Add";
			this.BtnEntityAdd.UseVisualStyleBackColor = true;
			this.BtnEntityAdd.Click += new System.EventHandler(this.BtnEntityAddClick_Event);
			// 
			// BtnEntityDelete
			// 
			this.BtnEntityDelete.Location = new System.Drawing.Point(8, 48);
			this.BtnEntityDelete.Name = "BtnEntityDelete";
			this.BtnEntityDelete.Size = new System.Drawing.Size(59, 23);
			this.BtnEntityDelete.TabIndex = 107;
			this.BtnEntityDelete.Text = "Delete";
			this.BtnEntityDelete.UseVisualStyleBackColor = true;
			this.BtnEntityDelete.Click += new System.EventHandler(this.BtnEntityDeleteClick_Event);
			// 
			// BtnEntitiesEditInstancesMode
			// 
			this.BtnEntitiesEditInstancesMode.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnEntitiesEditInstancesMode.Location = new System.Drawing.Point(4, 4);
			this.BtnEntitiesEditInstancesMode.Name = "BtnEntitiesEditInstancesMode";
			this.BtnEntitiesEditInstancesMode.Size = new System.Drawing.Size(74, 37);
			this.BtnEntitiesEditInstancesMode.TabIndex = 109;
			this.BtnEntitiesEditInstancesMode.Text = "Edit Instances";
			this.BtnEntitiesEditInstancesMode.UseVisualStyleBackColor = false;
			this.BtnEntitiesEditInstancesMode.Click += new System.EventHandler(this.BtnEntitiesEditInstancesModeClick_Event);
			// 
			// CheckBoxEntitySnapping
			// 
			this.CheckBoxEntitySnapping.Checked = true;
			this.CheckBoxEntitySnapping.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxEntitySnapping.Location = new System.Drawing.Point(4, 53);
			this.CheckBoxEntitySnapping.Name = "CheckBoxEntitySnapping";
			this.CheckBoxEntitySnapping.Size = new System.Drawing.Size(75, 17);
			this.CheckBoxEntitySnapping.TabIndex = 110;
			this.CheckBoxEntitySnapping.Text = "Snap 8pix";
			this.CheckBoxEntitySnapping.UseVisualStyleBackColor = true;
			this.CheckBoxEntitySnapping.CheckedChanged += new System.EventHandler(this.CheckBoxEntitySnappingChanged_Event);
			// 
			// groupBoxEntityEditor
			// 
			this.groupBoxEntityEditor.Controls.Add(this.CheckBoxSelectTargetEntity);
			this.groupBoxEntityEditor.Controls.Add(this.PBoxEntityPreview);
			this.groupBoxEntityEditor.Controls.Add(this.NumericUpDownEntityUID);
			this.groupBoxEntityEditor.Controls.Add(this.PBoxColor);
			this.groupBoxEntityEditor.Controls.Add(this.LabelEntityProperty);
			this.groupBoxEntityEditor.Controls.Add(this.LabelEntityInstanceProperty);
			this.groupBoxEntityEditor.Controls.Add(this.label8);
			this.groupBoxEntityEditor.Controls.Add(this.groupBox11);
			this.groupBoxEntityEditor.Controls.Add(this.groupBox12);
			this.groupBoxEntityEditor.Controls.Add(this.BtnEntityLoadBitmap);
			this.groupBoxEntityEditor.Controls.Add(this.LabelEntityName);
			this.groupBoxEntityEditor.Controls.Add(this.TextBoxEntityProperties);
			this.groupBoxEntityEditor.Controls.Add(this.TextBoxEntityInstanceProp);
			this.groupBoxEntityEditor.Location = new System.Drawing.Point(3, 3);
			this.groupBoxEntityEditor.Name = "groupBoxEntityEditor";
			this.groupBoxEntityEditor.Size = new System.Drawing.Size(283, 283);
			this.groupBoxEntityEditor.TabIndex = 111;
			this.groupBoxEntityEditor.TabStop = false;
			this.groupBoxEntityEditor.Text = "Active Entity";
			// 
			// CheckBoxSelectTargetEntity
			// 
			this.CheckBoxSelectTargetEntity.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxSelectTargetEntity.BackColor = System.Drawing.Color.Orange;
			this.CheckBoxSelectTargetEntity.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CheckBoxSelectTargetEntity.Location = new System.Drawing.Point(176, 87);
			this.CheckBoxSelectTargetEntity.Name = "CheckBoxSelectTargetEntity";
			this.CheckBoxSelectTargetEntity.Size = new System.Drawing.Size(100, 29);
			this.CheckBoxSelectTargetEntity.TabIndex = 116;
			this.CheckBoxSelectTargetEntity.Text = "Target UID:";
			this.CheckBoxSelectTargetEntity.UseVisualStyleBackColor = false;
			this.CheckBoxSelectTargetEntity.CheckedChanged += new System.EventHandler(this.CheckBoxSelectTargetEntityChanged_Event);
			// 
			// PBoxEntityPreview
			// 
			this.PBoxEntityPreview.BackColor = System.Drawing.Color.Black;
			this.PBoxEntityPreview.Location = new System.Drawing.Point(7, 87);
			this.PBoxEntityPreview.Name = "PBoxEntityPreview";
			this.PBoxEntityPreview.Size = new System.Drawing.Size(163, 190);
			this.PBoxEntityPreview.TabIndex = 1;
			this.PBoxEntityPreview.TabStop = false;
			// 
			// NumericUpDownEntityUID
			// 
			this.NumericUpDownEntityUID.Location = new System.Drawing.Point(36, 17);
			this.NumericUpDownEntityUID.Maximum = new decimal(new int[] {
									255,
									0,
									0,
									0});
			this.NumericUpDownEntityUID.Name = "NumericUpDownEntityUID";
			this.NumericUpDownEntityUID.Size = new System.Drawing.Size(43, 20);
			this.NumericUpDownEntityUID.TabIndex = 112;
			this.NumericUpDownEntityUID.ValueChanged += new System.EventHandler(this.NumericUpDownEntityUIDChanged_Event);
			// 
			// PBoxColor
			// 
			this.PBoxColor.BackColor = System.Drawing.Color.Black;
			this.PBoxColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.PBoxColor.Cursor = System.Windows.Forms.Cursors.Hand;
			this.PBoxColor.Location = new System.Drawing.Point(176, 218);
			this.PBoxColor.Name = "PBoxColor";
			this.PBoxColor.Size = new System.Drawing.Size(23, 23);
			this.PBoxColor.TabIndex = 6;
			this.PBoxColor.TabStop = false;
			this.PBoxColor.Click += new System.EventHandler(this.PBoxColorClick);
			// 
			// LabelEntityProperty
			// 
			this.LabelEntityProperty.Location = new System.Drawing.Point(8, 41);
			this.LabelEntityProperty.Name = "LabelEntityProperty";
			this.LabelEntityProperty.Size = new System.Drawing.Size(75, 14);
			this.LabelEntityProperty.TabIndex = 5;
			this.LabelEntityProperty.Text = "Properties:";
			// 
			// LabelEntityInstanceProperty
			// 
			this.LabelEntityInstanceProperty.Location = new System.Drawing.Point(7, 63);
			this.LabelEntityInstanceProperty.Name = "LabelEntityInstanceProperty";
			this.LabelEntityInstanceProperty.Size = new System.Drawing.Size(75, 14);
			this.LabelEntityInstanceProperty.TabIndex = 5;
			this.LabelEntityInstanceProperty.Text = "Inst. prop.:";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 20);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(32, 14);
			this.label8.TabIndex = 5;
			this.label8.Text = "UID:";
			// 
			// groupBox11
			// 
			this.groupBox11.Controls.Add(this.NumericUpDownEntityHeight);
			this.groupBox11.Controls.Add(this.NumericUpDownEntityWidth);
			this.groupBox11.Location = new System.Drawing.Point(176, 119);
			this.groupBox11.Name = "groupBox11";
			this.groupBox11.Size = new System.Drawing.Size(99, 45);
			this.groupBox11.TabIndex = 117;
			this.groupBox11.TabStop = false;
			this.groupBox11.Text = "Size (W/H):";
			// 
			// NumericUpDownEntityHeight
			// 
			this.NumericUpDownEntityHeight.Location = new System.Drawing.Point(51, 19);
			this.NumericUpDownEntityHeight.Maximum = new decimal(new int[] {
									255,
									0,
									0,
									0});
			this.NumericUpDownEntityHeight.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumericUpDownEntityHeight.Name = "NumericUpDownEntityHeight";
			this.NumericUpDownEntityHeight.Size = new System.Drawing.Size(43, 20);
			this.NumericUpDownEntityHeight.TabIndex = 119;
			this.NumericUpDownEntityHeight.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumericUpDownEntityHeight.ValueChanged += new System.EventHandler(this.NumericUpDownEntityHeightChanged_Event);
			// 
			// NumericUpDownEntityWidth
			// 
			this.NumericUpDownEntityWidth.Location = new System.Drawing.Point(6, 19);
			this.NumericUpDownEntityWidth.Maximum = new decimal(new int[] {
									255,
									0,
									0,
									0});
			this.NumericUpDownEntityWidth.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumericUpDownEntityWidth.Name = "NumericUpDownEntityWidth";
			this.NumericUpDownEntityWidth.Size = new System.Drawing.Size(43, 20);
			this.NumericUpDownEntityWidth.TabIndex = 118;
			this.NumericUpDownEntityWidth.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.NumericUpDownEntityWidth.ValueChanged += new System.EventHandler(this.NumericUpDownEntityWidthChanged_Event);
			// 
			// groupBox12
			// 
			this.groupBox12.Controls.Add(this.NumericUpDownEntityPivotY);
			this.groupBox12.Controls.Add(this.NumericUpDownEntityPivotX);
			this.groupBox12.Location = new System.Drawing.Point(176, 167);
			this.groupBox12.Name = "groupBox12";
			this.groupBox12.Size = new System.Drawing.Size(99, 45);
			this.groupBox12.TabIndex = 120;
			this.groupBox12.TabStop = false;
			this.groupBox12.Text = "Pivot (X/Y):";
			// 
			// NumericUpDownEntityPivotY
			// 
			this.NumericUpDownEntityPivotY.Location = new System.Drawing.Point(51, 19);
			this.NumericUpDownEntityPivotY.Maximum = new decimal(new int[] {
									127,
									0,
									0,
									0});
			this.NumericUpDownEntityPivotY.Minimum = new decimal(new int[] {
									128,
									0,
									0,
									-2147483648});
			this.NumericUpDownEntityPivotY.Name = "NumericUpDownEntityPivotY";
			this.NumericUpDownEntityPivotY.Size = new System.Drawing.Size(43, 20);
			this.NumericUpDownEntityPivotY.TabIndex = 122;
			this.NumericUpDownEntityPivotY.ValueChanged += new System.EventHandler(this.NumericUpDownEntityPivotYChanged_Event);
			// 
			// NumericUpDownEntityPivotX
			// 
			this.NumericUpDownEntityPivotX.Location = new System.Drawing.Point(6, 19);
			this.NumericUpDownEntityPivotX.Maximum = new decimal(new int[] {
									127,
									0,
									0,
									0});
			this.NumericUpDownEntityPivotX.Minimum = new decimal(new int[] {
									128,
									0,
									0,
									-2147483648});
			this.NumericUpDownEntityPivotX.Name = "NumericUpDownEntityPivotX";
			this.NumericUpDownEntityPivotX.Size = new System.Drawing.Size(43, 20);
			this.NumericUpDownEntityPivotX.TabIndex = 121;
			this.NumericUpDownEntityPivotX.ValueChanged += new System.EventHandler(this.NumericUpDownEntityPivotXChanged_Event);
			// 
			// BtnEntityLoadBitmap
			// 
			this.BtnEntityLoadBitmap.Location = new System.Drawing.Point(201, 218);
			this.BtnEntityLoadBitmap.Name = "BtnEntityLoadBitmap";
			this.BtnEntityLoadBitmap.Size = new System.Drawing.Size(75, 23);
			this.BtnEntityLoadBitmap.TabIndex = 123;
			this.BtnEntityLoadBitmap.Text = "Load Bitmap";
			this.BtnEntityLoadBitmap.UseVisualStyleBackColor = true;
			this.BtnEntityLoadBitmap.Click += new System.EventHandler(this.BtnEntityLoadBitmapClick);
			// 
			// LabelEntityName
			// 
			this.LabelEntityName.BackColor = System.Drawing.Color.Gainsboro;
			this.LabelEntityName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.LabelEntityName.Location = new System.Drawing.Point(85, 18);
			this.LabelEntityName.Name = "LabelEntityName";
			this.LabelEntityName.Size = new System.Drawing.Size(190, 17);
			this.LabelEntityName.TabIndex = 113;
			this.LabelEntityName.Text = "ENTITY NAME";
			this.LabelEntityName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// TextBoxEntityProperties
			// 
			this.TextBoxEntityProperties.AccessibleDescription = "";
			this.TextBoxEntityProperties.AccessibleName = "";
			this.TextBoxEntityProperties.BackColor = System.Drawing.SystemColors.Window;
			this.TextBoxEntityProperties.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TextBoxEntityProperties.Location = new System.Drawing.Point(85, 38);
			this.TextBoxEntityProperties.MaxLength = 255;
			this.TextBoxEntityProperties.Name = "TextBoxEntityProperties";
			this.TextBoxEntityProperties.Size = new System.Drawing.Size(190, 20);
			this.TextBoxEntityProperties.TabIndex = 114;
			this.TextBoxEntityProperties.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxEntityPropertiesKeyPress_Event);
			this.TextBoxEntityProperties.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxEntityPropertiesTextKeyUp_Event);
			// 
			// TextBoxEntityInstanceProp
			// 
			this.TextBoxEntityInstanceProp.AccessibleDescription = "";
			this.TextBoxEntityInstanceProp.AccessibleName = "";
			this.TextBoxEntityInstanceProp.BackColor = System.Drawing.SystemColors.Window;
			this.TextBoxEntityInstanceProp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TextBoxEntityInstanceProp.Location = new System.Drawing.Point(85, 60);
			this.TextBoxEntityInstanceProp.MaxLength = 255;
			this.TextBoxEntityInstanceProp.Name = "TextBoxEntityInstanceProp";
			this.TextBoxEntityInstanceProp.Size = new System.Drawing.Size(190, 20);
			this.TextBoxEntityInstanceProp.TabIndex = 115;
			this.TextBoxEntityInstanceProp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxEntityPropertiesKeyPress_Event);
			this.TextBoxEntityInstanceProp.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxEntityInstancePropTextKeyUp_Event);
			// 
			// TabPatterns
			// 
			this.TabPatterns.BackColor = System.Drawing.Color.Silver;
			this.TabPatterns.Controls.Add(this.splitContainer10);
			this.TabPatterns.Location = new System.Drawing.Point(4, 22);
			this.TabPatterns.Name = "TabPatterns";
			this.TabPatterns.Padding = new System.Windows.Forms.Padding(3);
			this.TabPatterns.Size = new System.Drawing.Size(294, 589);
			this.TabPatterns.TabIndex = 4;
			this.TabPatterns.Text = "Patterns";
			// 
			// splitContainer10
			// 
			this.splitContainer10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer10.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer10.IsSplitterFixed = true;
			this.splitContainer10.Location = new System.Drawing.Point(3, 3);
			this.splitContainer10.Name = "splitContainer10";
			this.splitContainer10.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer10.Panel1
			// 
			this.splitContainer10.Panel1.Controls.Add(this.splitContainer11);
			// 
			// splitContainer10.Panel2
			// 
			this.splitContainer10.Panel2.Controls.Add(this.BtnPatternReset);
			this.splitContainer10.Panel2.Controls.Add(this.PBoxPatternPreview);
			this.splitContainer10.Size = new System.Drawing.Size(288, 583);
			this.splitContainer10.SplitterDistance = 289;
			this.splitContainer10.TabIndex = 0;
			// 
			// splitContainer11
			// 
			this.splitContainer11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer11.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer11.IsSplitterFixed = true;
			this.splitContainer11.Location = new System.Drawing.Point(0, 0);
			this.splitContainer11.Name = "splitContainer11";
			// 
			// splitContainer11.Panel1
			// 
			this.splitContainer11.Panel1.Controls.Add(this.TreeViewPatterns);
			// 
			// splitContainer11.Panel2
			// 
			this.splitContainer11.Panel2.Controls.Add(this.BtnPatternRename);
			this.splitContainer11.Panel2.Controls.Add(this.groupBox4);
			this.splitContainer11.Panel2.Controls.Add(this.groupBox5);
			this.splitContainer11.Size = new System.Drawing.Size(288, 289);
			this.splitContainer11.SplitterDistance = 203;
			this.splitContainer11.TabIndex = 0;
			// 
			// TreeViewPatterns
			// 
			this.TreeViewPatterns.BackColor = System.Drawing.Color.LightGray;
			this.TreeViewPatterns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TreeViewPatterns.ContextMenuStrip = this.ContextMenuStripPatternsTreeViewGroup;
			this.TreeViewPatterns.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TreeViewPatterns.LabelEdit = true;
			this.TreeViewPatterns.Location = new System.Drawing.Point(0, 0);
			this.TreeViewPatterns.Name = "TreeViewPatterns";
			this.TreeViewPatterns.Size = new System.Drawing.Size(203, 289);
			this.TreeViewPatterns.TabIndex = 8;
			this.TreeViewPatterns.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.PatternsTreeViewNodeRename_Event);
			this.TreeViewPatterns.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.PatternsTreeViewNodeSelect_Event);
			this.TreeViewPatterns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown_Event);
			// 
			// ContextMenuStripPatternsTreeViewGroup
			// 
			this.ContextMenuStripPatternsTreeViewGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem3});
			this.ContextMenuStripPatternsTreeViewGroup.Name = "ContextMenuStripTreeViewGroup";
			this.ContextMenuStripPatternsTreeViewGroup.Size = new System.Drawing.Size(133, 26);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(132, 22);
			this.toolStripMenuItem3.Text = "Add Group";
			this.toolStripMenuItem3.Click += new System.EventHandler(this.BtnPatternGroupAddClick_Event);
			// 
			// BtnPatternRename
			// 
			this.BtnPatternRename.Location = new System.Drawing.Point(12, 171);
			this.BtnPatternRename.Name = "BtnPatternRename";
			this.BtnPatternRename.Size = new System.Drawing.Size(59, 23);
			this.BtnPatternRename.TabIndex = 111;
			this.BtnPatternRename.Text = "Rename";
			this.BtnPatternRename.UseVisualStyleBackColor = true;
			this.BtnPatternRename.Click += new System.EventHandler(this.BtnPatternRenameClick_Event);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.BtnPatternGroupAdd);
			this.groupBox4.Controls.Add(this.BtnPatternGroupDelete);
			this.groupBox4.Location = new System.Drawing.Point(4, 3);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(74, 80);
			this.groupBox4.TabIndex = 109;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Group";
			// 
			// BtnPatternGroupAdd
			// 
			this.BtnPatternGroupAdd.Location = new System.Drawing.Point(8, 19);
			this.BtnPatternGroupAdd.Name = "BtnPatternGroupAdd";
			this.BtnPatternGroupAdd.Size = new System.Drawing.Size(59, 23);
			this.BtnPatternGroupAdd.TabIndex = 103;
			this.BtnPatternGroupAdd.Text = "Add";
			this.BtnPatternGroupAdd.UseVisualStyleBackColor = true;
			this.BtnPatternGroupAdd.Click += new System.EventHandler(this.BtnPatternGroupAddClick_Event);
			// 
			// BtnPatternGroupDelete
			// 
			this.BtnPatternGroupDelete.Location = new System.Drawing.Point(8, 48);
			this.BtnPatternGroupDelete.Name = "BtnPatternGroupDelete";
			this.BtnPatternGroupDelete.Size = new System.Drawing.Size(59, 23);
			this.BtnPatternGroupDelete.TabIndex = 104;
			this.BtnPatternGroupDelete.Text = "Delete";
			this.BtnPatternGroupDelete.UseVisualStyleBackColor = true;
			this.BtnPatternGroupDelete.Click += new System.EventHandler(this.BtnPatternGroupDeleteClick_Event);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.CheckBoxPatternAdd);
			this.groupBox5.Controls.Add(this.BtnPatternDelete);
			this.groupBox5.Location = new System.Drawing.Point(4, 85);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(74, 80);
			this.groupBox5.TabIndex = 110;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Pattern";
			// 
			// CheckBoxPatternAdd
			// 
			this.CheckBoxPatternAdd.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxPatternAdd.AutoCheck = false;
			this.CheckBoxPatternAdd.BackColor = System.Drawing.Color.Orange;
			this.CheckBoxPatternAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.CheckBoxPatternAdd.Location = new System.Drawing.Point(8, 19);
			this.CheckBoxPatternAdd.Name = "CheckBoxPatternAdd";
			this.CheckBoxPatternAdd.Size = new System.Drawing.Size(59, 23);
			this.CheckBoxPatternAdd.TabIndex = 106;
			this.CheckBoxPatternAdd.Text = "Add";
			this.CheckBoxPatternAdd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CheckBoxPatternAdd.UseVisualStyleBackColor = false;
			this.CheckBoxPatternAdd.CheckedChanged += new System.EventHandler(this.BtnPatternAddChanged_Event);
			this.CheckBoxPatternAdd.Click += new System.EventHandler(this.BtnPatternAddClick_Event);
			// 
			// BtnPatternDelete
			// 
			this.BtnPatternDelete.Location = new System.Drawing.Point(8, 48);
			this.BtnPatternDelete.Name = "BtnPatternDelete";
			this.BtnPatternDelete.Size = new System.Drawing.Size(59, 23);
			this.BtnPatternDelete.TabIndex = 107;
			this.BtnPatternDelete.Text = "Delete";
			this.BtnPatternDelete.UseVisualStyleBackColor = true;
			this.BtnPatternDelete.Click += new System.EventHandler(this.BtnPatternDeleteClick_Event);
			// 
			// BtnPatternReset
			// 
			this.BtnPatternReset.Location = new System.Drawing.Point(210, 264);
			this.BtnPatternReset.Name = "BtnPatternReset";
			this.BtnPatternReset.Size = new System.Drawing.Size(75, 23);
			this.BtnPatternReset.TabIndex = 1;
			this.BtnPatternReset.Text = "Cancel";
			this.BtnPatternReset.UseVisualStyleBackColor = true;
			this.BtnPatternReset.Click += new System.EventHandler(this.BtnPatternResetClick_Event);
			// 
			// PBoxPatternPreview
			// 
			this.PBoxPatternPreview.BackColor = System.Drawing.Color.Black;
			this.PBoxPatternPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PBoxPatternPreview.Location = new System.Drawing.Point(0, 0);
			this.PBoxPatternPreview.Name = "PBoxPatternPreview";
			this.PBoxPatternPreview.Size = new System.Drawing.Size(288, 290);
			this.PBoxPatternPreview.TabIndex = 0;
			this.PBoxPatternPreview.TabStop = false;
			// 
			// PBoxLayout
			// 
			this.PBoxLayout.BackColor = System.Drawing.Color.Black;
			this.PBoxLayout.ContextMenuStrip = this.ContextMenuLayoutEditor;
			this.PBoxLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PBoxLayout.Location = new System.Drawing.Point(0, 0);
			this.PBoxLayout.Name = "PBoxLayout";
			this.PBoxLayout.Size = new System.Drawing.Size(924, 615);
			this.PBoxLayout.TabIndex = 0;
			this.PBoxLayout.VSync = false;
			// 
			// ContextMenuLayoutEditor
			// 
			this.ContextMenuLayoutEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.LayoutEntityOrderToolStripMenuItem,
									this.toolStripSeparator29,
									this.LayoutDeleteEntityToolStripMenuItem,
									this.LayoutDeleteScreenToolStripMenuItem,
									this.toolStripSeparator19,
									this.LayoutDeleteScreenEntitiesToolStripMenuItem,
									this.toolStripSeparator7,
									this.SetStartScreenMarkToolStripMenuItem,
									this.SetScreenMarkToolStripMenuItem,
									this.adjacentScreensDirectionToolStripMenuItem});
			this.ContextMenuLayoutEditor.Name = "ContextMenuLayoutEditor";
			this.ContextMenuLayoutEditor.Size = new System.Drawing.Size(234, 176);
			// 
			// LayoutEntityOrderToolStripMenuItem
			// 
			this.LayoutEntityOrderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.LayoutBringFrontToolStripMenuItem,
									this.LayoutSendBackToolStripMenuItem});
			this.LayoutEntityOrderToolStripMenuItem.Name = "LayoutEntityOrderToolStripMenuItem";
			this.LayoutEntityOrderToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.LayoutEntityOrderToolStripMenuItem.Text = "Entity Order";
			// 
			// LayoutBringFrontToolStripMenuItem
			// 
			this.LayoutBringFrontToolStripMenuItem.Name = "LayoutBringFrontToolStripMenuItem";
			this.LayoutBringFrontToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.LayoutBringFrontToolStripMenuItem.Text = "Bring to Front";
			this.LayoutBringFrontToolStripMenuItem.Click += new System.EventHandler(this.LayoutBringFrontToolStripMenuItemClick_Event);
			// 
			// LayoutSendBackToolStripMenuItem
			// 
			this.LayoutSendBackToolStripMenuItem.Name = "LayoutSendBackToolStripMenuItem";
			this.LayoutSendBackToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.LayoutSendBackToolStripMenuItem.Text = "Send to Back";
			this.LayoutSendBackToolStripMenuItem.Click += new System.EventHandler(this.LayoutSendBackToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator29
			// 
			this.toolStripSeparator29.Name = "toolStripSeparator29";
			this.toolStripSeparator29.Size = new System.Drawing.Size(230, 6);
			// 
			// LayoutDeleteEntityToolStripMenuItem
			// 
			this.LayoutDeleteEntityToolStripMenuItem.Name = "LayoutDeleteEntityToolStripMenuItem";
			this.LayoutDeleteEntityToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.LayoutDeleteEntityToolStripMenuItem.Text = "Delete Selected Entity";
			this.LayoutDeleteEntityToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteEntityToolStripMenuItemClick_Event);
			// 
			// LayoutDeleteScreenToolStripMenuItem
			// 
			this.LayoutDeleteScreenToolStripMenuItem.Name = "LayoutDeleteScreenToolStripMenuItem";
			this.LayoutDeleteScreenToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.LayoutDeleteScreenToolStripMenuItem.Text = "Delete Selected Screen(s)";
			this.LayoutDeleteScreenToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteScreenToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator19
			// 
			this.toolStripSeparator19.Name = "toolStripSeparator19";
			this.toolStripSeparator19.Size = new System.Drawing.Size(230, 6);
			// 
			// LayoutDeleteScreenEntitiesToolStripMenuItem
			// 
			this.LayoutDeleteScreenEntitiesToolStripMenuItem.Name = "LayoutDeleteScreenEntitiesToolStripMenuItem";
			this.LayoutDeleteScreenEntitiesToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.LayoutDeleteScreenEntitiesToolStripMenuItem.Text = "Delete Selected Screen Entities";
			this.LayoutDeleteScreenEntitiesToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteScreenEntitiesToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(230, 6);
			// 
			// SetStartScreenMarkToolStripMenuItem
			// 
			this.SetStartScreenMarkToolStripMenuItem.Name = "SetStartScreenMarkToolStripMenuItem";
			this.SetStartScreenMarkToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.SetStartScreenMarkToolStripMenuItem.Text = "Set Start Screen Mark";
			this.SetStartScreenMarkToolStripMenuItem.Click += new System.EventHandler(this.SetStartScreenMarkToolStripMenuItemClick_Event);
			// 
			// SetScreenMarkToolStripMenuItem
			// 
			this.SetScreenMarkToolStripMenuItem.Name = "SetScreenMarkToolStripMenuItem";
			this.SetScreenMarkToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.SetScreenMarkToolStripMenuItem.Text = "Set Screen Property";
			this.SetScreenMarkToolStripMenuItem.Click += new System.EventHandler(this.SetScreenMarkToolStripMenuItemClick_Event);
			// 
			// adjacentScreensDirectionToolStripMenuItem
			// 
			this.adjacentScreensDirectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.AdjScrMaskNoneToolStripMenuItem,
									this.AdjScrMaskLToolStripMenuItem,
									this.AdjScrMaskUToolStripMenuItem,
									this.AdjScrMaskRToolStripMenuItem,
									this.AdjScrMaskDToolStripMenuItem,
									this.AdjScrMaskLUToolStripMenuItem,
									this.AdjScrMaskURToolStripMenuItem,
									this.AdjScrMaskRDToolStripMenuItem,
									this.AdjScrMaskLDToolStripMenuItem,
									this.AdjScrMaskLRToolStripMenuItem,
									this.AdjScrMaskUDToolStripMenuItem,
									this.AdjScrMaskLURToolStripMenuItem,
									this.AdjScrMaskURDToolStripMenuItem,
									this.AdjScrMaskLRDToolStripMenuItem,
									this.AdjScrMaskLUDToolStripMenuItem,
									this.AdjScrMaskLURDToolStripMenuItem});
			this.adjacentScreensDirectionToolStripMenuItem.Name = "adjacentScreensDirectionToolStripMenuItem";
			this.adjacentScreensDirectionToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.adjacentScreensDirectionToolStripMenuItem.Text = "Adjacent Screens Mask";
			// 
			// AdjScrMaskNoneToolStripMenuItem
			// 
			this.AdjScrMaskNoneToolStripMenuItem.Name = "AdjScrMaskNoneToolStripMenuItem";
			this.AdjScrMaskNoneToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskNoneToolStripMenuItem.Text = "NONE";
			this.AdjScrMaskNoneToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLToolStripMenuItem
			// 
			this.AdjScrMaskLToolStripMenuItem.Name = "AdjScrMaskLToolStripMenuItem";
			this.AdjScrMaskLToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLToolStripMenuItem.Text = "L";
			this.AdjScrMaskLToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskUToolStripMenuItem
			// 
			this.AdjScrMaskUToolStripMenuItem.Name = "AdjScrMaskUToolStripMenuItem";
			this.AdjScrMaskUToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskUToolStripMenuItem.Text = "U";
			this.AdjScrMaskUToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskRToolStripMenuItem
			// 
			this.AdjScrMaskRToolStripMenuItem.Name = "AdjScrMaskRToolStripMenuItem";
			this.AdjScrMaskRToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskRToolStripMenuItem.Text = "R";
			this.AdjScrMaskRToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskDToolStripMenuItem
			// 
			this.AdjScrMaskDToolStripMenuItem.Name = "AdjScrMaskDToolStripMenuItem";
			this.AdjScrMaskDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskDToolStripMenuItem.Text = "D";
			this.AdjScrMaskDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLUToolStripMenuItem
			// 
			this.AdjScrMaskLUToolStripMenuItem.Name = "AdjScrMaskLUToolStripMenuItem";
			this.AdjScrMaskLUToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLUToolStripMenuItem.Text = "LU";
			this.AdjScrMaskLUToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskURToolStripMenuItem
			// 
			this.AdjScrMaskURToolStripMenuItem.Name = "AdjScrMaskURToolStripMenuItem";
			this.AdjScrMaskURToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskURToolStripMenuItem.Text = "UR";
			this.AdjScrMaskURToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskRDToolStripMenuItem
			// 
			this.AdjScrMaskRDToolStripMenuItem.Name = "AdjScrMaskRDToolStripMenuItem";
			this.AdjScrMaskRDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskRDToolStripMenuItem.Text = "RD";
			this.AdjScrMaskRDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLDToolStripMenuItem
			// 
			this.AdjScrMaskLDToolStripMenuItem.Name = "AdjScrMaskLDToolStripMenuItem";
			this.AdjScrMaskLDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLDToolStripMenuItem.Text = "LD";
			this.AdjScrMaskLDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLRToolStripMenuItem
			// 
			this.AdjScrMaskLRToolStripMenuItem.Name = "AdjScrMaskLRToolStripMenuItem";
			this.AdjScrMaskLRToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLRToolStripMenuItem.Text = "LR";
			this.AdjScrMaskLRToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskUDToolStripMenuItem
			// 
			this.AdjScrMaskUDToolStripMenuItem.Name = "AdjScrMaskUDToolStripMenuItem";
			this.AdjScrMaskUDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskUDToolStripMenuItem.Text = "UD";
			this.AdjScrMaskUDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLURToolStripMenuItem
			// 
			this.AdjScrMaskLURToolStripMenuItem.Name = "AdjScrMaskLURToolStripMenuItem";
			this.AdjScrMaskLURToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLURToolStripMenuItem.Text = "LUR";
			this.AdjScrMaskLURToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskURDToolStripMenuItem
			// 
			this.AdjScrMaskURDToolStripMenuItem.Name = "AdjScrMaskURDToolStripMenuItem";
			this.AdjScrMaskURDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskURDToolStripMenuItem.Text = "URD";
			this.AdjScrMaskURDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLRDToolStripMenuItem
			// 
			this.AdjScrMaskLRDToolStripMenuItem.Name = "AdjScrMaskLRDToolStripMenuItem";
			this.AdjScrMaskLRDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLRDToolStripMenuItem.Text = "LRD";
			this.AdjScrMaskLRDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLUDToolStripMenuItem
			// 
			this.AdjScrMaskLUDToolStripMenuItem.Name = "AdjScrMaskLUDToolStripMenuItem";
			this.AdjScrMaskLUDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLUDToolStripMenuItem.Text = "LUD";
			this.AdjScrMaskLUDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// AdjScrMaskLURDToolStripMenuItem
			// 
			this.AdjScrMaskLURDToolStripMenuItem.Name = "AdjScrMaskLURDToolStripMenuItem";
			this.AdjScrMaskLURDToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.AdjScrMaskLURDToolStripMenuItem.Text = "LURD";
			this.AdjScrMaskLURDToolStripMenuItem.Click += new System.EventHandler(this.AdjScrMaskClick_Event);
			// 
			// ContextMenuTilesList
			// 
			this.ContextMenuTilesList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyTileToolStripMenuItem,
									this.pasteTileToolStripMenuItem,
									this.separatorToolStripMenuItem4,
									this.clearTileToolStripMenuItem,
									this.toolStripSeparator21,
									this.insertLeftTileToolStripMenuItem,
									this.deleteTileToolStripMenuItem3,
									this.toolStripSeparator28,
									this.clearAllTilesToolStripMenuItem});
			this.ContextMenuTilesList.Name = "ContextMenuTilesList";
			this.ContextMenuTilesList.Size = new System.Drawing.Size(146, 154);
			// 
			// copyTileToolStripMenuItem
			// 
			this.copyTileToolStripMenuItem.Name = "copyTileToolStripMenuItem";
			this.copyTileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.copyTileToolStripMenuItem.Text = "Copy";
			this.copyTileToolStripMenuItem.Click += new System.EventHandler(this.CopyTileToolStripMenuItemClick_Event);
			// 
			// pasteTileToolStripMenuItem
			// 
			this.pasteTileToolStripMenuItem.Name = "pasteTileToolStripMenuItem";
			this.pasteTileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.pasteTileToolStripMenuItem.Text = "Paste";
			this.pasteTileToolStripMenuItem.Click += new System.EventHandler(this.PasteTileToolStripMenuItemClick_Event);
			// 
			// separatorToolStripMenuItem4
			// 
			this.separatorToolStripMenuItem4.Name = "separatorToolStripMenuItem4";
			this.separatorToolStripMenuItem4.Size = new System.Drawing.Size(142, 6);
			// 
			// clearTileToolStripMenuItem
			// 
			this.clearTileToolStripMenuItem.Name = "clearTileToolStripMenuItem";
			this.clearTileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.clearTileToolStripMenuItem.Text = "Clear Refs";
			this.clearTileToolStripMenuItem.Click += new System.EventHandler(this.ClearTileToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator21
			// 
			this.toolStripSeparator21.Name = "toolStripSeparator21";
			this.toolStripSeparator21.Size = new System.Drawing.Size(142, 6);
			// 
			// insertLeftTileToolStripMenuItem
			// 
			this.insertLeftTileToolStripMenuItem.Name = "insertLeftTileToolStripMenuItem";
			this.insertLeftTileToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.insertLeftTileToolStripMenuItem.Text = "Insert Left";
			this.insertLeftTileToolStripMenuItem.Click += new System.EventHandler(this.InsertLeftTileToolStripMenuItemClick_Event);
			// 
			// deleteTileToolStripMenuItem3
			// 
			this.deleteTileToolStripMenuItem3.Name = "deleteTileToolStripMenuItem3";
			this.deleteTileToolStripMenuItem3.Size = new System.Drawing.Size(145, 22);
			this.deleteTileToolStripMenuItem3.Text = "Delete";
			this.deleteTileToolStripMenuItem3.Click += new System.EventHandler(this.DeleteTileToolStripMenuItem3Click_Event);
			// 
			// toolStripSeparator28
			// 
			this.toolStripSeparator28.Name = "toolStripSeparator28";
			this.toolStripSeparator28.Size = new System.Drawing.Size(142, 6);
			// 
			// clearAllTilesToolStripMenuItem
			// 
			this.clearAllTilesToolStripMenuItem.Name = "clearAllTilesToolStripMenuItem";
			this.clearAllTilesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.clearAllTilesToolStripMenuItem.Text = "Clear All Tiles";
			this.clearAllTilesToolStripMenuItem.Click += new System.EventHandler(this.ClearAllTileToolStripMenuItemClick_Event);
			// 
			// ContextMenuBlocksList
			// 
			this.ContextMenuBlocksList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyBlockToolStripMenuItem,
									this.pasteBlockCloneToolStripMenuItem,
									this.pasteBlockRefsToolStripMenuItem,
									this.separatorToolStripMenuItem3,
									this.clearCHRsBlockToolStripMenuItem,
									this.clearRefsBlockToolStripMenuItem,
									this.toolStripSeparator12,
									this.clearPropertiesToolStripMenuItem1,
									this.toolStripSeparator22,
									this.insertLeftBlockToolStripMenuItem,
									this.deleteBlockToolStripMenuItem});
			this.ContextMenuBlocksList.Name = "contextMenuStripBlocksList";
			this.ContextMenuBlocksList.Size = new System.Drawing.Size(158, 198);
			// 
			// copyBlockToolStripMenuItem
			// 
			this.copyBlockToolStripMenuItem.Name = "copyBlockToolStripMenuItem";
			this.copyBlockToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.copyBlockToolStripMenuItem.Text = "Copy";
			this.copyBlockToolStripMenuItem.Click += new System.EventHandler(this.CopyBlockToolStripMenuItemClick_Event);
			// 
			// pasteBlockCloneToolStripMenuItem
			// 
			this.pasteBlockCloneToolStripMenuItem.Name = "pasteBlockCloneToolStripMenuItem";
			this.pasteBlockCloneToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.pasteBlockCloneToolStripMenuItem.Text = "Paste Clone";
			this.pasteBlockCloneToolStripMenuItem.Click += new System.EventHandler(this.PasteBlockCloneToolStripMenuItemClick_Event);
			// 
			// pasteBlockRefsToolStripMenuItem
			// 
			this.pasteBlockRefsToolStripMenuItem.Name = "pasteBlockRefsToolStripMenuItem";
			this.pasteBlockRefsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.pasteBlockRefsToolStripMenuItem.Text = "Paste Refs";
			this.pasteBlockRefsToolStripMenuItem.Click += new System.EventHandler(this.PasteBlockRefsToolStripMenuItemClickEvent);
			// 
			// separatorToolStripMenuItem3
			// 
			this.separatorToolStripMenuItem3.Name = "separatorToolStripMenuItem3";
			this.separatorToolStripMenuItem3.Size = new System.Drawing.Size(154, 6);
			// 
			// clearCHRsBlockToolStripMenuItem
			// 
			this.clearCHRsBlockToolStripMenuItem.Name = "clearCHRsBlockToolStripMenuItem";
			this.clearCHRsBlockToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.clearCHRsBlockToolStripMenuItem.Text = "Clear CHRs";
			this.clearCHRsBlockToolStripMenuItem.Click += new System.EventHandler(this.ClearCHRsBlockToolStripMenuItemClick_Event);
			// 
			// clearRefsBlockToolStripMenuItem
			// 
			this.clearRefsBlockToolStripMenuItem.Name = "clearRefsBlockToolStripMenuItem";
			this.clearRefsBlockToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.clearRefsBlockToolStripMenuItem.Text = "Clear Refs";
			this.clearRefsBlockToolStripMenuItem.Click += new System.EventHandler(this.ClearRefsBlockToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(154, 6);
			// 
			// clearPropertiesToolStripMenuItem1
			// 
			this.clearPropertiesToolStripMenuItem1.Name = "clearPropertiesToolStripMenuItem1";
			this.clearPropertiesToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
			this.clearPropertiesToolStripMenuItem1.Text = "Clear Properties";
			this.clearPropertiesToolStripMenuItem1.Click += new System.EventHandler(this.clearPropertiesToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator22
			// 
			this.toolStripSeparator22.Name = "toolStripSeparator22";
			this.toolStripSeparator22.Size = new System.Drawing.Size(154, 6);
			// 
			// insertLeftBlockToolStripMenuItem
			// 
			this.insertLeftBlockToolStripMenuItem.Name = "insertLeftBlockToolStripMenuItem";
			this.insertLeftBlockToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.insertLeftBlockToolStripMenuItem.Text = "Insert Left";
			this.insertLeftBlockToolStripMenuItem.Click += new System.EventHandler(this.InsertLeftBlockToolStripMenuItemClick_Event);
			// 
			// deleteBlockToolStripMenuItem
			// 
			this.deleteBlockToolStripMenuItem.Name = "deleteBlockToolStripMenuItem";
			this.deleteBlockToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.deleteBlockToolStripMenuItem.Text = "Delete";
			this.deleteBlockToolStripMenuItem.Click += new System.EventHandler(this.DeleteBlockToolStripMenuItemClick_Event);
			// 
			// StatusBar
			// 
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.StatusBarLabel});
			this.StatusBar.Location = new System.Drawing.Point(0, 671);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.Size = new System.Drawing.Size(1244, 22);
			this.StatusBar.SizingGrip = false;
			this.StatusBar.TabIndex = 4;
			this.StatusBar.Text = "...";
			// 
			// StatusBarLabel
			// 
			this.StatusBarLabel.Name = "StatusBarLabel";
			this.StatusBarLabel.Size = new System.Drawing.Size(16, 17);
			this.StatusBarLabel.Text = "...";
			// 
			// Project_saveFileDialog
			// 
			this.Project_saveFileDialog.DefaultExt = "mapednes";
			this.Project_saveFileDialog.Filter = "MAPeD-NES (*.mapednes)|*.mapednes";
			this.Project_saveFileDialog.Title = "Save Project";
			this.Project_saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ProjectSaveOk_Event);
			// 
			// Project_openFileDialog
			// 
			this.Project_openFileDialog.DefaultExt = "mapednes";
			this.Project_openFileDialog.Filter = "MAPeD-NES (*.mapednes)|*.mapednes";
			this.Project_openFileDialog.Title = "Load Project";
			this.Project_openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ProjectLoadOk_Event);
			// 
			// Import_openFileDialog
			// 
			this.Import_openFileDialog.DefaultExt = "sprednes";
			this.Import_openFileDialog.Filter = "SPReD-NES CHR Bank (*.sprednes)|*.sprednes|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin" +
			"|Tiles/Game Map 2/4 bpp (*.bmp)|*.bmp|Palette (192 bytes) (*.pal)|*.pal";
			this.Import_openFileDialog.Title = "Import Data Into Active CHR Bank";
			this.Import_openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.DataImportOk_Event);
			// 
			// Project_exportFileDialog
			// 
			this.Project_exportFileDialog.DefaultExt = "asm";
			this.Project_exportFileDialog.Filter = "CA65\\NESasm (*.asm)|*.asm|ZX SjASMPlus (*.zxa)|*.zxa|Active Tile\\Block Set (*.bmp" +
			")|*.bmp|Active Layout (*.png)|*.png|Text (*.json)|*.json";
			this.Project_exportFileDialog.Title = "Export Project";
			this.Project_exportFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ProjectExportOk_Event);
			// 
			// ContextMenuEntitiesTreeEntity
			// 
			this.ContextMenuEntitiesTreeEntity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem1,
									this.toolStripMenuItem2,
									this.toolStripSeparator16,
									this.deleteAllInstancesToolStripMenuItem});
			this.ContextMenuEntitiesTreeEntity.Name = "ContextMenuStripEntitiesTreeGoup";
			this.ContextMenuEntitiesTreeEntity.Size = new System.Drawing.Size(177, 76);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(176, 22);
			this.toolStripMenuItem1.Text = "&Delete";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.BtnEntityDeleteClick_Event);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(176, 22);
			this.toolStripMenuItem2.Text = "Re&name";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.BtnEntityRenameClick_Event);
			// 
			// toolStripSeparator16
			// 
			this.toolStripSeparator16.Name = "toolStripSeparator16";
			this.toolStripSeparator16.Size = new System.Drawing.Size(173, 6);
			// 
			// deleteAllInstancesToolStripMenuItem
			// 
			this.deleteAllInstancesToolStripMenuItem.Name = "deleteAllInstancesToolStripMenuItem";
			this.deleteAllInstancesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
			this.deleteAllInstancesToolStripMenuItem.Text = "Delete All &Instances";
			this.deleteAllInstancesToolStripMenuItem.Click += new System.EventHandler(this.DeleteAllInstancesToolStripMenuItemClick_Event);
			// 
			// colorDialogEntity
			// 
			this.colorDialogEntity.Color = System.Drawing.Color.WhiteSmoke;
			this.colorDialogEntity.FullOpen = true;
			// 
			// EntityLoadBitmap_openFileDialog
			// 
			this.EntityLoadBitmap_openFileDialog.DefaultExt = "bmp";
			this.EntityLoadBitmap_openFileDialog.Filter = "Bitmap (*.bmp)|*.bmp";
			this.EntityLoadBitmap_openFileDialog.Title = "Load Entity Image";
			this.EntityLoadBitmap_openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.EntityLoadBitmap_openFileDialogFileOk);
			// 
			// ContextMenuStripPatternItem
			// 
			this.ContextMenuStripPatternItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.deleteToolStripMenuItem,
									this.renameToolStripMenuItem});
			this.ContextMenuStripPatternItem.Name = "ContextMenuStripPatternItem";
			this.ContextMenuStripPatternItem.Size = new System.Drawing.Size(118, 48);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.BtnPatternDeleteClick_Event);
			// 
			// renameToolStripMenuItem
			// 
			this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
			this.renameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.renameToolStripMenuItem.Text = "Rename";
			this.renameToolStripMenuItem.Click += new System.EventHandler(this.BtnPatternRenameClick_Event);
			// 
			// ContextMenuStripGroupItem
			// 
			this.ContextMenuStripGroupItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem4,
									this.toolStripSeparator17,
									this.toolStripMenuItem5,
									this.renameToolStripMenuItem1});
			this.ContextMenuStripGroupItem.Name = "ContextMenuStripPatternsGroup";
			this.ContextMenuStripGroupItem.Size = new System.Drawing.Size(138, 76);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(137, 22);
			this.toolStripMenuItem4.Text = "Add Pattern";
			this.toolStripMenuItem4.Click += new System.EventHandler(this.BtnPatternAddClick_Event);
			// 
			// toolStripSeparator17
			// 
			this.toolStripSeparator17.Name = "toolStripSeparator17";
			this.toolStripSeparator17.Size = new System.Drawing.Size(134, 6);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(137, 22);
			this.toolStripMenuItem5.Text = "Delete";
			this.toolStripMenuItem5.Click += new System.EventHandler(this.BtnPatternGroupDeleteClick_Event);
			// 
			// renameToolStripMenuItem1
			// 
			this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
			this.renameToolStripMenuItem1.Size = new System.Drawing.Size(137, 22);
			this.renameToolStripMenuItem1.Text = "Rename";
			this.renameToolStripMenuItem1.Click += new System.EventHandler(this.BtnPatternRenameClick_Event);
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1244, 693);
			this.Controls.Add(this.tabControlTilesLayout);
			this.Controls.Add(this.MenuStrip);
			this.Controls.Add(this.StatusBar);
			this.Icon = global::MAPeD.Properties.Resources.MAPeD_icon;
			this.KeyPreview = true;
			this.MainMenuStrip = this.MenuStrip;
			this.Name = "MainForm";
			this.Text = "MAPeD-NES";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_Event);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyUp_Event);
			this.ContextMenuEntitiesTreeGroup.ResumeLayout(false);
			this.MenuStrip.ResumeLayout(false);
			this.MenuStrip.PerformLayout();
			this.tabControlTilesLayout.ResumeLayout(false);
			this.TabTiles.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
			this.splitContainer4.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.GrpBoxPalettes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Palette3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette0)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PaletteMain)).EndInit();
			this.GrpBoxBlockEditor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxBlockEditor)).EndInit();
			this.ContextMenuBlockEditor.ResumeLayout(false);
			this.GrpBoxTileEditor.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxTilePreview)).EndInit();
			this.GrpBoxScreenData.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownScrBlocksHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownScrBlocksWidth)).EndInit();
			this.GrpBoxCHRBank.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxCHRBank)).EndInit();
			this.ContextMenuCHRBank.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.TabLayout.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
			this.splitContainer5.ResumeLayout(false);
			this.tabControlLayoutTools.ResumeLayout(false);
			this.TabBuilder.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox7.ResumeLayout(false);
			this.groupBox13.ResumeLayout(false);
			this.TabPainter.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.GrpBoxPainter.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.GrpBoxActiveTile.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxActiveTile)).EndInit();
			this.TabScreenList.ResumeLayout(false);
			this.splitContainer6.Panel1.ResumeLayout(false);
			this.splitContainer6.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
			this.splitContainer6.ResumeLayout(false);
			this.TabEntities.ResumeLayout(false);
			this.splitContainer7.Panel1.ResumeLayout(false);
			this.splitContainer7.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
			this.splitContainer7.ResumeLayout(false);
			this.splitContainer8.Panel1.ResumeLayout(false);
			this.splitContainer8.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).EndInit();
			this.splitContainer8.ResumeLayout(false);
			this.ContextMenuEntitiesTree.ResumeLayout(false);
			this.splitContainer9.Panel1.ResumeLayout(false);
			this.splitContainer9.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer9)).EndInit();
			this.splitContainer9.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
			this.groupBox10.ResumeLayout(false);
			this.groupBoxEntityEditor.ResumeLayout(false);
			this.groupBoxEntityEditor.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxEntityPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityUID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PBoxColor)).EndInit();
			this.groupBox11.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityWidth)).EndInit();
			this.groupBox12.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityPivotY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.NumericUpDownEntityPivotX)).EndInit();
			this.TabPatterns.ResumeLayout(false);
			this.splitContainer10.Panel1.ResumeLayout(false);
			this.splitContainer10.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer10)).EndInit();
			this.splitContainer10.ResumeLayout(false);
			this.splitContainer11.Panel1.ResumeLayout(false);
			this.splitContainer11.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer11)).EndInit();
			this.splitContainer11.ResumeLayout(false);
			this.ContextMenuStripPatternsTreeViewGroup.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxPatternPreview)).EndInit();
			this.ContextMenuLayoutEditor.ResumeLayout(false);
			this.ContextMenuTilesList.ResumeLayout(false);
			this.ContextMenuBlocksList.ResumeLayout(false);
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.ContextMenuEntitiesTreeEntity.ResumeLayout(false);
			this.ContextMenuStripPatternItem.ResumeLayout(false);
			this.ContextMenuStripGroupItem.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.CheckBox CheckBoxPainterReplaceTiles;
		private System.Windows.Forms.Button BtnPainterFillWithTile;
		private System.Windows.Forms.GroupBox groupBox6;
		private SkiaSharp.Views.Desktop.SKGLControl PBoxLayout;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteScreenEntitiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteAllInstancesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
		private System.Windows.Forms.ToolStripMenuItem deleteAllEntitiesInstancesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
		private System.Windows.Forms.CheckBox CheckBoxPatternAdd;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStripGroupItem;
		private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStripPatternItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ContextMenuStrip ContextMenuStripPatternsTreeViewGroup;
		private System.Windows.Forms.ToolStripMenuItem deletePatternGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addPatternGroupToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem groupPatternToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem deletePatternToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addPatternToolStripMenuItem1;
		private System.Windows.Forms.Button BtnPatternReset;
		private System.Windows.Forms.PictureBox PBoxPatternPreview;
		private System.Windows.Forms.Button BtnPatternDelete;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button BtnPatternGroupDelete;
		private System.Windows.Forms.Button BtnPatternGroupAdd;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button BtnPatternRename;
		private System.Windows.Forms.SplitContainer splitContainer11;
		private System.Windows.Forms.SplitContainer splitContainer10;
		private System.Windows.Forms.TreeView TreeViewPatterns;
		private System.Windows.Forms.ToolStripMenuItem patternsToolStripMenuItem;
		private System.Windows.Forms.TabPage TabPatterns;
		private System.Windows.Forms.RadioButton RBtnMapScaleX1;
		private System.Windows.Forms.RadioButton RBtnMapScaleX2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox GrpBoxPainter;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.TabPage TabPainter;
		private System.Windows.Forms.SplitContainer splitContainer9;
		private System.Windows.Forms.SplitContainer splitContainer8;
		private System.Windows.Forms.SplitContainer splitContainer7;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator29;
		private System.Windows.Forms.ToolStripMenuItem LayoutSendBackToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutBringFrontToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutEntityOrderToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer6;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TabPage TabBuilder;
		private System.Windows.Forms.ToolStripMenuItem reorderBanksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearPropertiesToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem clearPropertiesToolStripMenuItem;
		private System.Windows.Forms.Label LabelScreenResolution;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.NumericUpDown NumericUpDownScrBlocksHeight;
		private System.Windows.Forms.NumericUpDown NumericUpDownScrBlocksWidth;
		private System.Windows.Forms.ToolStripMenuItem ZXInvertImageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ZXSwapInkPaperToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ZXToolStripSeparator;
		private System.Windows.Forms.Button BtnInvInk;
		private System.Windows.Forms.Button BtnSwapInkPaper;
		private System.Windows.Forms.Label LabelPalette34;
		private System.Windows.Forms.Label LabelPalette12;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.Button BtnScreenDataInfo;
		private System.Windows.Forms.RadioButton RBtnScreenDataTiles;
		private System.Windows.Forms.RadioButton RBtnScreenDataBlocks;
		private System.Windows.Forms.GroupBox GrpBoxScreenData;
		private System.Windows.Forms.Button BtnPltCopy;
		private System.Windows.Forms.Button BtnPltDelete;
		private System.Windows.Forms.ComboBox CBoxPalettes;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator28;
		private System.Windows.Forms.Button BtnSwapColors;
		private System.Windows.Forms.ToolStripMenuItem statisticsToolStripMenuItem;
		private System.Windows.Forms.Button BtnCreateLayoutWxH;
		private System.Windows.Forms.Button BtnLayoutDeleteEmptyScreens;
		private System.Windows.Forms.ToolStripMenuItem nextPageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem prevPageToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator CHRBankPageBtnsToolStripSeparator;
		private System.Windows.Forms.Button BtnCHRBankPrevPage;
		private System.Windows.Forms.Button BtnCHRBankNextPage;
		private System.Windows.Forms.GroupBox GrpBoxActiveTile;
		private System.Windows.Forms.Button BtnResetTile;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator27;
		private System.Windows.Forms.ToolStripMenuItem quickGuideToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportScriptEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editInstancesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reserveCHRsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator24;
		private System.Windows.Forms.ToolStripMenuItem reserveBlocksToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator25;
		private System.Windows.Forms.Button BtnTileReserveBlocks;
		private System.Windows.Forms.Button BtnBlockReserveCHRs;
		private System.Windows.Forms.ToolStripMenuItem deleteCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertLeftCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator23;
		private System.Windows.Forms.ToolStripMenuItem deleteBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertLeftBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator22;
		private System.Windows.Forms.ToolStripMenuItem deleteTileToolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem insertLeftTileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator21;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
		private System.Windows.Forms.ToolStripMenuItem PropIdPerCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PropIdPerBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem propertyIdToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
		private System.Windows.Forms.ToolStripMenuItem PropertyPerCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PropertyPerBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem propertyToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxPalettePerCHR;
		private System.Windows.Forms.ToolStripMenuItem descriptionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLURDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLUDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLRDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskURDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLURToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskUDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskRDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskURToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLUToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskUToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskLToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AdjScrMaskNoneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem adjacentScreensDirectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutShowCoordsToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxShowCoords;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
		private System.Windows.Forms.ToolStripMenuItem LayoutShowTargetsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutShowEntitiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutShowMarksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteAllScreenMarksToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem SetScreenMarkToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SetStartScreenMarkToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.CheckBox CheckBoxShowMarks;
		private System.Windows.Forms.CheckBox CheckBoxShowTargets;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.CheckBox CheckBoxSelectTargetEntity;
		private System.Windows.Forms.Button BtnCopyLayout;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxEntitySnapping;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteEntityToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxShowEntities;
		private System.Windows.Forms.ToolStripMenuItem ScreensShowAllBanksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ScreensAutoUpdateToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
		private System.Windows.Forms.ToolStripMenuItem shiftColorsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shiftTransparencyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem LayoutShowGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem TilesLockEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optimizationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem horizontalFlipToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticalFlipToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripMenuItem BlockEditorModeDrawToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem BlockEditorModeSelectToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparatorShiftTransp;
		private System.Windows.Forms.ToolStripMenuItem updateGFXToolStripMenuItem;
		private System.Windows.Forms.Button BtnEntitiesEditInstancesMode;
		private System.Windows.Forms.TextBox TextBoxEntityProperties;
		private System.Windows.Forms.Label LabelEntityProperty;
		private System.Windows.Forms.OpenFileDialog EntityLoadBitmap_openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipHOrizontalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipVerticalToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem copyBankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteBankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addBankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cHRToolStripMenuItem;
		private System.Windows.Forms.NumericUpDown NumericUpDownEntityUID;
		private System.Windows.Forms.GroupBox groupBoxEntityEditor;
		private System.Windows.Forms.Label LabelEntityName;
		private System.Windows.Forms.TextBox TextBoxEntityInstanceProp;
		private System.Windows.Forms.Label LabelEntityInstanceProperty;
		private System.Windows.Forms.ColorDialog colorDialogEntity;
		private System.Windows.Forms.NumericUpDown NumericUpDownEntityWidth;
		private System.Windows.Forms.NumericUpDown NumericUpDownEntityHeight;
		private System.Windows.Forms.GroupBox groupBox11;
		private System.Windows.Forms.NumericUpDown NumericUpDownEntityPivotX;
		private System.Windows.Forms.NumericUpDown NumericUpDownEntityPivotY;
		private System.Windows.Forms.GroupBox groupBox12;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.PictureBox PBoxColor;
		private System.Windows.Forms.Button BtnEntityLoadBitmap;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip ContextMenuEntitiesTreeEntity;
		private System.Windows.Forms.Button BtnEntityRename;
		private System.Windows.Forms.Button BtnEntityGroupDelete;
		private System.Windows.Forms.Button BtnEntityGroupAdd;
		private System.Windows.Forms.GroupBox groupBox9;
		private System.Windows.Forms.Button BtnEntityDelete;
		private System.Windows.Forms.Button BtnEntityAdd;
		private System.Windows.Forms.GroupBox groupBox10;
		private System.Windows.Forms.ToolStripMenuItem deleteEntityGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addEntityGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cHRBankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem builderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem screensToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blocksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteEntityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addEntityToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem entitiesGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem entitiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addEntityToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem renameGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteGroupToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuEntitiesTreeGroup;
		private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuEntitiesTree;
		private System.Windows.Forms.PictureBox PBoxEntityPreview;
		private System.Windows.Forms.TreeView TreeViewEntities;
		private System.Windows.Forms.FlowLayoutPanel PanelTiles;
		private System.Windows.Forms.FlowLayoutPanel PanelBlocks;
		private System.Windows.Forms.TabPage TabEntities;
		private System.Windows.Forms.TabPage TabScreenList;
		private System.Windows.Forms.TabControl tabControlLayoutTools;
		private System.Windows.Forms.Button BtnOptimization;
		private System.Windows.Forms.Button BtnTilesBlocks;
		private System.Windows.Forms.CheckBox CheckBoxScreensAutoUpdate;
		private System.Windows.Forms.Button BtnEditModeSelectCHR;
		private System.Windows.Forms.Button BtnEditModeDraw;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label LabelLayoutEditorCHRBankID;
		private System.Windows.Forms.CheckBox CheckBoxLayoutEditorAllBanks;
		private System.Windows.Forms.Button BtnLayoutMoveUp;
		private System.Windows.Forms.Button BtnLayoutMoveDown;
		private System.Windows.Forms.Button BtnDeleteLayout;
		private System.Windows.Forms.ListBox ListBoxLayouts;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.Label LayoutLabel;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteScreenToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuLayoutEditor;
		private System.Windows.Forms.Button BtnLayoutRemoveUpRow;
		private System.Windows.Forms.Button BtnLayoutAddDownRow;
		private System.Windows.Forms.Button BtnLayoutRemoveDownRow;
		private System.Windows.Forms.Button BtnLayoutAddLeftColumn;
		private System.Windows.Forms.Button BtnLayoutRemoveLeftColumn;
		private System.Windows.Forms.Button BtnLayoutAddRightColumn;
		private System.Windows.Forms.Button BtnLayoutRemoveRightColumn;
		private System.Windows.Forms.Button BtnUpdateScreens;
		private System.Windows.Forms.ListView ListViewScreens;
		private System.Windows.Forms.Button BtnLayoutAddUpRow;
		private System.Windows.Forms.ToolStripMenuItem clearRefsBlockToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog Project_exportFileDialog;
		private System.Windows.Forms.ToolStripMenuItem clearAllTilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearTileToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem pasteTileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyTileToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuTilesList;
		private System.Windows.Forms.ToolStripMenuItem clearCHRsBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem3;
		private System.Windows.Forms.Button BtnBlockRotate;
		private System.Windows.Forms.ToolStripMenuItem pasteBlockRefsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteBlockCloneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyBlockToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuBlocksList;
		private System.Windows.Forms.ToolStripMenuItem fillWithColorCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem pasteCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyCHRToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuCHRBank;
		private System.Windows.Forms.OpenFileDialog Import_openFileDialog;
		private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxTileEditorLock;
		private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DrawToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CHRSelectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blockEditModesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuBlockEditor;
		private System.Windows.Forms.PictureBox PBoxActiveTile;
		private System.Windows.Forms.OpenFileDialog Project_openFileDialog;
		private System.Windows.Forms.SaveFileDialog Project_saveFileDialog;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.Button BtnCopyCHRBank;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Button BtnUpdateGFX;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox PBoxTilePreview;
		private System.Windows.Forms.GroupBox GrpBoxTileEditor;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ToolStripStatusLabel StatusBarLabel;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.CheckBox CheckBoxShowGrid;
		private System.Windows.Forms.Button BtnCHRVFlip;
		private System.Windows.Forms.Button BtnCHRHFlip;
		private System.Windows.Forms.Button BtnCHRRotate;
		private System.Windows.Forms.Label LabelObjId;
		private System.Windows.Forms.ComboBox CBoxBlockObjId;
		private System.Windows.Forms.ComboBox CBoxTileViewType;
		private System.Windows.Forms.Button BtnBlockVFlip;
		private System.Windows.Forms.Button BtnBlockHFlip;
		private System.Windows.Forms.GroupBox GrpBoxBlockEditor;
		private System.Windows.Forms.PictureBox PBoxBlockEditor;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button BtnAddCHRBank;
		private System.Windows.Forms.Button BtnDeleteCHRBank;
		private System.Windows.Forms.ComboBox CBoxCHRBanks;
		private System.Windows.Forms.PictureBox PBoxCHRBank;
		private System.Windows.Forms.GroupBox GrpBoxCHRBank;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.PictureBox PaletteMain;
		private System.Windows.Forms.PictureBox Palette0;
		private System.Windows.Forms.PictureBox Palette1;
		private System.Windows.Forms.PictureBox Palette2;
		private System.Windows.Forms.PictureBox Palette3;
		private System.Windows.Forms.GroupBox GrpBoxPalettes;
		private System.Windows.Forms.TabPage TabLayout;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabPage TabTiles;
		private System.Windows.Forms.TabControl tabControlTilesLayout;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip MenuStrip;
	}
}