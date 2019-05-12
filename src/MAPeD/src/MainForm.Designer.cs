/*
 * Created by SharpDevelop.
 * User: $8bIT
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.ContextMenuEntitiesTreeGoup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.renameGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.addEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.MenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.descriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.optimizationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
			this.TilesLockEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reserveBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateGFXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
			this.BlockEditorModeSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BlockEditorModeDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.propertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PropertyPerBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.PropertyPerCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			this.verticalFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.horizontalFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rotateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.shiftTransparencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.shiftColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
			this.reserveCHRsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cHRBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyBankToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			this.cHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipHOrizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rotateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.entitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addEntityToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.addGroupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.renameEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
			this.editInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.screensToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ScreensAutoUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ScreensShowAllBanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
			this.editorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
			this.openPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
			this.ScreenEditModeSingleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ScreenEditModeLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
			this.ScreenEditShowGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.createToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowEntitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowTargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutShowCoordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			this.LayoutDeleteAllEntityInstancesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.LayoutDeleteAllScreenMarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tabControlEditorLayout = new System.Windows.Forms.TabControl();
			this.TabEditor = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.BtnOptimization = new System.Windows.Forms.Button();
			this.CBoxTileViewType = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.PanelTiles = new System.Windows.Forms.FlowLayoutPanel();
			this.tabControlTilesScreens = new System.Windows.Forms.TabControl();
			this.TabTiles = new System.Windows.Forms.TabPage();
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
			this.groupBox3 = new System.Windows.Forms.GroupBox();
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
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.CheckBoxPalettePerCHR = new System.Windows.Forms.CheckBox();
			this.Palette3 = new System.Windows.Forms.PictureBox();
			this.Palette2 = new System.Windows.Forms.PictureBox();
			this.Palette1 = new System.Windows.Forms.PictureBox();
			this.Palette0 = new System.Windows.Forms.PictureBox();
			this.PaletteMain = new System.Windows.Forms.PictureBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.CBoxBlockObjId = new System.Windows.Forms.ComboBox();
			this.BtnBlockReserveCHRs = new System.Windows.Forms.Button();
			this.BtnBlockRotate = new System.Windows.Forms.Button();
			this.LabelObjId = new System.Windows.Forms.Label();
			this.BtnEditModeDraw = new System.Windows.Forms.Button();
			this.BtnUpdateGFX = new System.Windows.Forms.Button();
			this.BtnEditModeSelectCHR = new System.Windows.Forms.Button();
			this.BtnBlockHFlip = new System.Windows.Forms.Button();
			this.BtnBlockVFlip = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.PanelBlocks = new System.Windows.Forms.FlowLayoutPanel();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.BtnTileReserveBlocks = new System.Windows.Forms.Button();
			this.CheckBoxTileEditorLock = new System.Windows.Forms.CheckBox();
			this.PBoxTilePreview = new System.Windows.Forms.PictureBox();
			this.TabScreenEditor = new System.Windows.Forms.TabPage();
			this.BtnPalette = new System.Windows.Forms.Button();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.RBtnScreenEditModeLayout = new System.Windows.Forms.RadioButton();
			this.RBtnScreenEditModeSingle = new System.Windows.Forms.RadioButton();
			this.LabelActiveTile = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.ListBoxScreens = new System.Windows.Forms.ListBox();
			this.CheckBoxScreenShowGrid = new System.Windows.Forms.CheckBox();
			this.BtnDeleteScreen = new System.Windows.Forms.Button();
			this.BtnCopyScreen = new System.Windows.Forms.Button();
			this.BtnCreateScreen = new System.Windows.Forms.Button();
			this.PBoxScreen = new System.Windows.Forms.PictureBox();
			this.PBoxActiveTile = new System.Windows.Forms.PictureBox();
			this.TabLayout = new System.Windows.Forms.TabPage();
			this.tabControlScreensEntities = new System.Windows.Forms.TabControl();
			this.TabScreenList = new System.Windows.Forms.TabPage();
			this.CheckBoxLayoutEditorAllBanks = new System.Windows.Forms.CheckBox();
			this.LabelLayoutEditorCHRBankID = new System.Windows.Forms.Label();
			this.ListViewScreens = new System.Windows.Forms.ListView();
			this.label9 = new System.Windows.Forms.Label();
			this.BtnUpdateScreens = new System.Windows.Forms.Button();
			this.CheckBoxScreensAutoUpdate = new System.Windows.Forms.CheckBox();
			this.TabEntities = new System.Windows.Forms.TabPage();
			this.groupBoxEntitiesTreeView = new System.Windows.Forms.GroupBox();
			this.BtnEntityRename = new System.Windows.Forms.Button();
			this.BtnEntitiesEditInstancesMode = new System.Windows.Forms.Button();
			this.groupBox10 = new System.Windows.Forms.GroupBox();
			this.BtnEntityAdd = new System.Windows.Forms.Button();
			this.BtnEntityDelete = new System.Windows.Forms.Button();
			this.TreeViewEntities = new System.Windows.Forms.TreeView();
			this.ContextMenuEntitiesTree = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox9 = new System.Windows.Forms.GroupBox();
			this.BtnEntityGroupAdd = new System.Windows.Forms.Button();
			this.BtnEntityGroupDelete = new System.Windows.Forms.Button();
			this.CheckBoxEntitySnapping = new System.Windows.Forms.CheckBox();
			this.groupBoxEntityEditor = new System.Windows.Forms.GroupBox();
			this.CheckBoxPickupTargetEntity = new System.Windows.Forms.CheckBox();
			this.PBoxEntityPreview = new System.Windows.Forms.PictureBox();
			this.NumericUpDownEntityUID = new System.Windows.Forms.NumericUpDown();
			this.PBoxColor = new System.Windows.Forms.PictureBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
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
			this.CBoxEntityPreviewScaleX2 = new System.Windows.Forms.CheckBox();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.BtnLayoutMoveDown = new System.Windows.Forms.Button();
			this.BtnLayoutMoveUp = new System.Windows.Forms.Button();
			this.ListBoxLayouts = new System.Windows.Forms.ListBox();
			this.BtnCopyLayout = new System.Windows.Forms.Button();
			this.BtnDeleteLayout = new System.Windows.Forms.Button();
			this.BtnCreateLayout = new System.Windows.Forms.Button();
			this.LayoutLabel = new System.Windows.Forms.Label();
			this.BtnLayoutRemoveRightColumn = new System.Windows.Forms.Button();
			this.BtnLayoutAddRightColumn = new System.Windows.Forms.Button();
			this.BtnLayoutRemoveLeftColumn = new System.Windows.Forms.Button();
			this.BtnLayoutAddLeftColumn = new System.Windows.Forms.Button();
			this.BtnLayoutRemoveDownRow = new System.Windows.Forms.Button();
			this.BtnLayoutAddDownRow = new System.Windows.Forms.Button();
			this.BtnLayoutRemoveUpRow = new System.Windows.Forms.Button();
			this.BtnLayoutAddUpRow = new System.Windows.Forms.Button();
			this.PBoxLayout = new System.Windows.Forms.PictureBox();
			this.ContextMenuLayoutEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.LayoutDeleteScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.LayoutDeleteEntityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
			this.groupBox13 = new System.Windows.Forms.GroupBox();
			this.CheckBoxShowCoords = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowTargets = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowMarks = new System.Windows.Forms.CheckBox();
			this.CheckBoxShowEntities = new System.Windows.Forms.CheckBox();
			this.ContextMenuTilesList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.clearTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
			this.insertLeftTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteTileToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.ContextMenuBlocksList = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteBlockCloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteBlockRefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.separatorToolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.clearCHRsBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearRefsBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
			this.colorDialogEntity = new System.Windows.Forms.ColorDialog();
			this.EntityLoadBitmap_openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.ContextMenuEntitiesTreeGoup.SuspendLayout();
			this.MenuStrip.SuspendLayout();
			this.tabControlEditorLayout.SuspendLayout();
			this.TabEditor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tabControlTilesScreens.SuspendLayout();
			this.TabTiles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxBlockEditor)).BeginInit();
			this.ContextMenuBlockEditor.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxCHRBank)).BeginInit();
			this.ContextMenuCHRBank.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Palette3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette0)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PaletteMain)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxTilePreview)).BeginInit();
			this.TabScreenEditor.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.groupBox6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxScreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.PBoxActiveTile)).BeginInit();
			this.TabLayout.SuspendLayout();
			this.tabControlScreensEntities.SuspendLayout();
			this.TabScreenList.SuspendLayout();
			this.TabEntities.SuspendLayout();
			this.groupBoxEntitiesTreeView.SuspendLayout();
			this.groupBox10.SuspendLayout();
			this.ContextMenuEntitiesTree.SuspendLayout();
			this.groupBox9.SuspendLayout();
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
			this.groupBox7.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PBoxLayout)).BeginInit();
			this.ContextMenuLayoutEditor.SuspendLayout();
			this.groupBox13.SuspendLayout();
			this.ContextMenuTilesList.SuspendLayout();
			this.ContextMenuBlocksList.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.ContextMenuEntitiesTreeEntity.SuspendLayout();
			this.SuspendLayout();
			// 
			// ContextMenuEntitiesTreeGoup
			// 
			this.ContextMenuEntitiesTreeGoup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.deleteGroupToolStripMenuItem,
									this.renameGroupToolStripMenuItem,
									this.toolStripSeparator2,
									this.addEntityToolStripMenuItem});
			this.ContextMenuEntitiesTreeGoup.Name = "ContextMenuStripEntitiesTreeGoup";
			this.ContextMenuEntitiesTreeGoup.Size = new System.Drawing.Size(130, 76);
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
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(126, 6);
			// 
			// addEntityToolStripMenuItem
			// 
			this.addEntityToolStripMenuItem.Name = "addEntityToolStripMenuItem";
			this.addEntityToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
			this.addEntityToolStripMenuItem.Text = "&Add Entity";
			this.addEntityToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityAddClick_Event);
			// 
			// MenuStrip
			// 
			this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem,
									this.tilesToolStripMenuItem,
									this.blocksToolStripMenuItem,
									this.cHRBankToolStripMenuItem,
									this.entitiesToolStripMenuItem,
									this.screensToolStripMenuItem,
									this.layoutToolStripMenuItem,
									this.helpToolStripMenuItem});
			this.MenuStrip.Location = new System.Drawing.Point(0, 0);
			this.MenuStrip.Name = "MenuStrip";
			this.MenuStrip.Size = new System.Drawing.Size(1032, 24);
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
									this.toolStripSeparator1,
									this.descriptionToolStripMenuItem,
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
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.loadToolStripMenuItem.Text = "&Load";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItemClick_Event);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItemClick_Event);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.closeToolStripMenuItem.Text = "&Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItemClick_Event);
			// 
			// separatorToolStripMenuItem1
			// 
			this.separatorToolStripMenuItem1.Name = "separatorToolStripMenuItem1";
			this.separatorToolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.importToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.importToolStripMenuItem.Text = "&Import";
			this.importToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItemClick_Event);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.exportToolStripMenuItem.Text = "&Export";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
			// 
			// descriptionToolStripMenuItem
			// 
			this.descriptionToolStripMenuItem.Name = "descriptionToolStripMenuItem";
			this.descriptionToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.descriptionToolStripMenuItem.Text = "&Description";
			this.descriptionToolStripMenuItem.Click += new System.EventHandler(this.DescriptionToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(145, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick_Event);
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
			// blocksToolStripMenuItem
			// 
			this.blocksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.updateGFXToolStripMenuItem,
									this.toolStripSeparator20,
									this.BlockEditorModeSelectToolStripMenuItem,
									this.BlockEditorModeDrawToolStripMenuItem,
									this.toolStripSeparator11,
									this.propertyToolStripMenuItem,
									this.toolStripSeparator13,
									this.verticalFlipToolStripMenuItem,
									this.horizontalFlipToolStripMenuItem,
									this.rotateToolStripMenuItem1,
									this.toolStripSeparator12,
									this.shiftTransparencyToolStripMenuItem,
									this.shiftColorsToolStripMenuItem,
									this.toolStripSeparator24,
									this.reserveCHRsToolStripMenuItem});
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
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(175, 6);
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
			// cHRBankToolStripMenuItem
			// 
			this.cHRBankToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addBankToolStripMenuItem,
									this.deleteBankToolStripMenuItem,
									this.copyBankToolStripMenuItem,
									this.toolStripSeparator8,
									this.cHRToolStripMenuItem});
			this.cHRBankToolStripMenuItem.Name = "cHRBankToolStripMenuItem";
			this.cHRBankToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
			this.cHRBankToolStripMenuItem.Text = "&CHR Bank";
			// 
			// addBankToolStripMenuItem
			// 
			this.addBankToolStripMenuItem.Name = "addBankToolStripMenuItem";
			this.addBankToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.addBankToolStripMenuItem.Text = "&Add Bank";
			this.addBankToolStripMenuItem.Click += new System.EventHandler(this.BtnAddCHRBankClick_Event);
			// 
			// deleteBankToolStripMenuItem
			// 
			this.deleteBankToolStripMenuItem.Name = "deleteBankToolStripMenuItem";
			this.deleteBankToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.deleteBankToolStripMenuItem.Text = "&Delete Bank";
			this.deleteBankToolStripMenuItem.Click += new System.EventHandler(this.BtnDeleteCHRBankClick_Event);
			// 
			// copyBankToolStripMenuItem
			// 
			this.copyBankToolStripMenuItem.Name = "copyBankToolStripMenuItem";
			this.copyBankToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyBankToolStripMenuItem.Text = "&Copy Bank";
			this.copyBankToolStripMenuItem.Click += new System.EventHandler(this.BtnCopyCHRBankClick_Event);
			// 
			// toolStripSeparator8
			// 
			this.toolStripSeparator8.Name = "toolStripSeparator8";
			this.toolStripSeparator8.Size = new System.Drawing.Size(133, 6);
			// 
			// cHRToolStripMenuItem
			// 
			this.cHRToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.flipVerticalToolStripMenuItem,
									this.flipHOrizontalToolStripMenuItem,
									this.rotateToolStripMenuItem});
			this.cHRToolStripMenuItem.Name = "cHRToolStripMenuItem";
			this.cHRToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
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
			// entitiesToolStripMenuItem
			// 
			this.entitiesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addEntityToolStripMenuItem1,
									this.deleteEntityToolStripMenuItem,
									this.toolStripSeparator4,
									this.addGroupToolStripMenuItem1,
									this.toolStripSeparator6,
									this.renameEntityToolStripMenuItem,
									this.toolStripSeparator26,
									this.editInstancesToolStripMenuItem});
			this.entitiesToolStripMenuItem.Name = "entitiesToolStripMenuItem";
			this.entitiesToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.entitiesToolStripMenuItem.Text = "&Entities";
			// 
			// addEntityToolStripMenuItem1
			// 
			this.addEntityToolStripMenuItem1.Name = "addEntityToolStripMenuItem1";
			this.addEntityToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
			this.addEntityToolStripMenuItem1.Text = "&Add";
			this.addEntityToolStripMenuItem1.Click += new System.EventHandler(this.BtnEntityAddClick_Event);
			// 
			// deleteEntityToolStripMenuItem
			// 
			this.deleteEntityToolStripMenuItem.Name = "deleteEntityToolStripMenuItem";
			this.deleteEntityToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.deleteEntityToolStripMenuItem.Text = "&Delete";
			this.deleteEntityToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityDeleteClick_Event);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(179, 6);
			// 
			// addGroupToolStripMenuItem1
			// 
			this.addGroupToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addToolStripMenuItem,
									this.deleteToolStripMenuItem});
			this.addGroupToolStripMenuItem1.Name = "addGroupToolStripMenuItem1";
			this.addGroupToolStripMenuItem1.Size = new System.Drawing.Size(182, 22);
			this.addGroupToolStripMenuItem1.Text = "&Group";
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.addToolStripMenuItem.Text = "&Add";
			this.addToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupAddClick_Event);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deleteToolStripMenuItem.Text = "&Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupDeleteClick_Event);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(179, 6);
			// 
			// renameEntityToolStripMenuItem
			// 
			this.renameEntityToolStripMenuItem.Name = "renameEntityToolStripMenuItem";
			this.renameEntityToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.renameEntityToolStripMenuItem.Text = "&Rename";
			this.renameEntityToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityRenameClick_Event);
			// 
			// toolStripSeparator26
			// 
			this.toolStripSeparator26.Name = "toolStripSeparator26";
			this.toolStripSeparator26.Size = new System.Drawing.Size(179, 6);
			// 
			// editInstancesToolStripMenuItem
			// 
			this.editInstancesToolStripMenuItem.Name = "editInstancesToolStripMenuItem";
			this.editInstancesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
			this.editInstancesToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
			this.editInstancesToolStripMenuItem.Text = "&Edit Instances";
			this.editInstancesToolStripMenuItem.Click += new System.EventHandler(this.BtnEntitiesEditInstancesModeClick_Event);
			// 
			// screensToolStripMenuItem
			// 
			this.screensToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.updateToolStripMenuItem,
									this.ScreensAutoUpdateToolStripMenuItem,
									this.ScreensShowAllBanksToolStripMenuItem,
									this.toolStripSeparator19,
									this.editorToolStripMenuItem1});
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
			// toolStripSeparator19
			// 
			this.toolStripSeparator19.Name = "toolStripSeparator19";
			this.toolStripSeparator19.Size = new System.Drawing.Size(151, 6);
			// 
			// editorToolStripMenuItem1
			// 
			this.editorToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.createToolStripMenuItem,
									this.copyToolStripMenuItem3,
									this.deleteToolStripMenuItem1,
									this.toolStripSeparator16,
									this.openPaletteToolStripMenuItem,
									this.toolStripSeparator17,
									this.ScreenEditModeSingleToolStripMenuItem,
									this.ScreenEditModeLayoutToolStripMenuItem,
									this.toolStripSeparator18,
									this.ScreenEditShowGridToolStripMenuItem});
			this.editorToolStripMenuItem1.Name = "editorToolStripMenuItem1";
			this.editorToolStripMenuItem1.Size = new System.Drawing.Size(154, 22);
			this.editorToolStripMenuItem1.Text = "E&ditor";
			// 
			// createToolStripMenuItem
			// 
			this.createToolStripMenuItem.Name = "createToolStripMenuItem";
			this.createToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.createToolStripMenuItem.Text = "&C&reate";
			this.createToolStripMenuItem.Click += new System.EventHandler(this.BtnCreateScreenClick_Event);
			// 
			// copyToolStripMenuItem3
			// 
			this.copyToolStripMenuItem3.Name = "copyToolStripMenuItem3";
			this.copyToolStripMenuItem3.Size = new System.Drawing.Size(144, 22);
			this.copyToolStripMenuItem3.Text = "&Copy";
			this.copyToolStripMenuItem3.Click += new System.EventHandler(this.BtnCopyScreenClick_Event);
			// 
			// deleteToolStripMenuItem1
			// 
			this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
			this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(144, 22);
			this.deleteToolStripMenuItem1.Text = "&Delete";
			this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.BtnDeleteScreenClick_Event);
			// 
			// toolStripSeparator16
			// 
			this.toolStripSeparator16.Name = "toolStripSeparator16";
			this.toolStripSeparator16.Size = new System.Drawing.Size(141, 6);
			// 
			// openPaletteToolStripMenuItem
			// 
			this.openPaletteToolStripMenuItem.Name = "openPaletteToolStripMenuItem";
			this.openPaletteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.openPaletteToolStripMenuItem.Text = "&Open Palette";
			this.openPaletteToolStripMenuItem.Click += new System.EventHandler(this.BtnPaletteClick_Event);
			// 
			// toolStripSeparator17
			// 
			this.toolStripSeparator17.Name = "toolStripSeparator17";
			this.toolStripSeparator17.Size = new System.Drawing.Size(141, 6);
			// 
			// ScreenEditModeSingleToolStripMenuItem
			// 
			this.ScreenEditModeSingleToolStripMenuItem.Name = "ScreenEditModeSingleToolStripMenuItem";
			this.ScreenEditModeSingleToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.ScreenEditModeSingleToolStripMenuItem.Text = "Mode &Single";
			this.ScreenEditModeSingleToolStripMenuItem.Click += new System.EventHandler(this.ScreenEditModeSingleToolStripMenuItemClick_Event);
			// 
			// ScreenEditModeLayoutToolStripMenuItem
			// 
			this.ScreenEditModeLayoutToolStripMenuItem.Name = "ScreenEditModeLayoutToolStripMenuItem";
			this.ScreenEditModeLayoutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.ScreenEditModeLayoutToolStripMenuItem.Text = "Mode &Layout";
			this.ScreenEditModeLayoutToolStripMenuItem.Click += new System.EventHandler(this.ScreenEditModeLayoutToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator18
			// 
			this.toolStripSeparator18.Name = "toolStripSeparator18";
			this.toolStripSeparator18.Size = new System.Drawing.Size(141, 6);
			// 
			// ScreenEditShowGridToolStripMenuItem
			// 
			this.ScreenEditShowGridToolStripMenuItem.Checked = true;
			this.ScreenEditShowGridToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.ScreenEditShowGridToolStripMenuItem.Name = "ScreenEditShowGridToolStripMenuItem";
			this.ScreenEditShowGridToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.ScreenEditShowGridToolStripMenuItem.Text = "&Show Grid";
			this.ScreenEditShowGridToolStripMenuItem.Click += new System.EventHandler(this.ScreenEditShowGridToolStripMenuItemClick_Event);
			// 
			// layoutToolStripMenuItem
			// 
			this.layoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.createToolStripMenuItem1,
									this.copyToolStripMenuItem,
									this.deleteToolStripMenuItem2,
									this.toolStripSeparator9,
									this.showToolStripMenuItem,
									this.toolStripSeparator10,
									this.LayoutDeleteAllEntityInstancesToolStripMenuItem,
									this.LayoutDeleteAllScreenMarksToolStripMenuItem});
			this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
			this.layoutToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
			this.layoutToolStripMenuItem.Text = "&Layout";
			// 
			// createToolStripMenuItem1
			// 
			this.createToolStripMenuItem1.Name = "createToolStripMenuItem1";
			this.createToolStripMenuItem1.Size = new System.Drawing.Size(209, 22);
			this.createToolStripMenuItem1.Text = "C&reate";
			this.createToolStripMenuItem1.Click += new System.EventHandler(this.BtnCreateLayoutClick_Event);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.copyToolStripMenuItem.Text = "C&opy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.BtnCopyLayoutClick_Event);
			// 
			// deleteToolStripMenuItem2
			// 
			this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
			this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(209, 22);
			this.deleteToolStripMenuItem2.Text = "&Delete";
			this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.BtnDeleteLayoutClick_Event);
			// 
			// toolStripSeparator9
			// 
			this.toolStripSeparator9.Name = "toolStripSeparator9";
			this.toolStripSeparator9.Size = new System.Drawing.Size(206, 6);
			// 
			// showToolStripMenuItem
			// 
			this.showToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.LayoutShowMarksToolStripMenuItem,
									this.LayoutShowEntitiesToolStripMenuItem,
									this.LayoutShowTargetsToolStripMenuItem,
									this.LayoutShowCoordsToolStripMenuItem});
			this.showToolStripMenuItem.Name = "showToolStripMenuItem";
			this.showToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
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
			// toolStripSeparator10
			// 
			this.toolStripSeparator10.Name = "toolStripSeparator10";
			this.toolStripSeparator10.Size = new System.Drawing.Size(206, 6);
			// 
			// LayoutDeleteAllEntityInstancesToolStripMenuItem
			// 
			this.LayoutDeleteAllEntityInstancesToolStripMenuItem.Name = "LayoutDeleteAllEntityInstancesToolStripMenuItem";
			this.LayoutDeleteAllEntityInstancesToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.LayoutDeleteAllEntityInstancesToolStripMenuItem.Text = "Delete All Entity &Instances";
			this.LayoutDeleteAllEntityInstancesToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteAllEntityInstancesToolStripMenuItemClick_Event);
			// 
			// LayoutDeleteAllScreenMarksToolStripMenuItem
			// 
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Name = "LayoutDeleteAllScreenMarksToolStripMenuItem";
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Text = "Delete All Screen &Marks";
			this.LayoutDeleteAllScreenMarksToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteAllScreenMarksToolStripMenuItemClick_Event);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Info";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick_Event);
			// 
			// tabControlEditorLayout
			// 
			this.tabControlEditorLayout.Controls.Add(this.TabEditor);
			this.tabControlEditorLayout.Controls.Add(this.TabLayout);
			this.tabControlEditorLayout.Location = new System.Drawing.Point(0, 27);
			this.tabControlEditorLayout.Name = "tabControlEditorLayout";
			this.tabControlEditorLayout.SelectedIndex = 0;
			this.tabControlEditorLayout.Size = new System.Drawing.Size(1034, 641);
			this.tabControlEditorLayout.TabIndex = 60;
			this.tabControlEditorLayout.DoubleClick += new System.EventHandler(this.TabCntrlDblClick_Event);
			// 
			// TabEditor
			// 
			this.TabEditor.Controls.Add(this.splitContainer1);
			this.TabEditor.Location = new System.Drawing.Point(4, 22);
			this.TabEditor.Name = "TabEditor";
			this.TabEditor.Padding = new System.Windows.Forms.Padding(3);
			this.TabEditor.Size = new System.Drawing.Size(1026, 615);
			this.TabEditor.TabIndex = 0;
			this.TabEditor.Text = "Editor";
			this.TabEditor.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.Color.Gainsboro;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel1MinSize = 242;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControlTilesScreens);
			this.splitContainer1.Size = new System.Drawing.Size(1020, 609);
			this.splitContainer1.SplitterDistance = 242;
			this.splitContainer1.TabIndex = 19;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.IsSplitterFixed = true;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.BackColor = System.Drawing.Color.LightGray;
			this.splitContainer2.Panel1.Controls.Add(this.BtnOptimization);
			this.splitContainer2.Panel1.Controls.Add(this.CBoxTileViewType);
			this.splitContainer2.Panel1.Controls.Add(this.label6);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PanelTiles);
			this.splitContainer2.Size = new System.Drawing.Size(242, 609);
			this.splitContainer2.SplitterDistance = 26;
			this.splitContainer2.TabIndex = 1;
			// 
			// BtnOptimization
			// 
			this.BtnOptimization.BackColor = System.Drawing.Color.Gainsboro;
			this.BtnOptimization.ForeColor = System.Drawing.SystemColors.ControlText;
			this.BtnOptimization.Location = new System.Drawing.Point(151, 3);
			this.BtnOptimization.Name = "BtnOptimization";
			this.BtnOptimization.Size = new System.Drawing.Size(89, 23);
			this.BtnOptimization.TabIndex = 2;
			this.BtnOptimization.Text = "Optimization";
			this.BtnOptimization.UseVisualStyleBackColor = true;
			this.BtnOptimization.Click += new System.EventHandler(this.BtnOptimizationClick_Event);
			// 
			// CBoxTileViewType
			// 
			this.CBoxTileViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxTileViewType.FormattingEnabled = true;
			this.CBoxTileViewType.Items.AddRange(new object[] {
									"Graphics",
									"Property Id",
									"Number"});
			this.CBoxTileViewType.Location = new System.Drawing.Point(60, 4);
			this.CBoxTileViewType.Name = "CBoxTileViewType";
			this.CBoxTileViewType.Size = new System.Drawing.Size(75, 21);
			this.CBoxTileViewType.TabIndex = 1;
			this.CBoxTileViewType.SelectedIndexChanged += new System.EventHandler(this.CBoxTileViewTypeChanged_Event);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(2, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(62, 19);
			this.label6.TabIndex = 18;
			this.label6.Text = "View Type:";
			// 
			// PanelTiles
			// 
			this.PanelTiles.AutoScroll = true;
			this.PanelTiles.BackColor = System.Drawing.Color.Silver;
			this.PanelTiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelTiles.Cursor = System.Windows.Forms.Cursors.Hand;
			this.PanelTiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PanelTiles.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.PanelTiles.Location = new System.Drawing.Point(0, 0);
			this.PanelTiles.Name = "PanelTiles";
			this.PanelTiles.Size = new System.Drawing.Size(242, 579);
			this.PanelTiles.TabIndex = 3;
			// 
			// tabControlTilesScreens
			// 
			this.tabControlTilesScreens.Controls.Add(this.TabTiles);
			this.tabControlTilesScreens.Controls.Add(this.TabScreenEditor);
			this.tabControlTilesScreens.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControlTilesScreens.Location = new System.Drawing.Point(0, 0);
			this.tabControlTilesScreens.Name = "tabControlTilesScreens";
			this.tabControlTilesScreens.SelectedIndex = 0;
			this.tabControlTilesScreens.Size = new System.Drawing.Size(774, 609);
			this.tabControlTilesScreens.TabIndex = 40;
			this.tabControlTilesScreens.DoubleClick += new System.EventHandler(this.TabCntrlDblClick_Event);
			// 
			// TabTiles
			// 
			this.TabTiles.AutoScroll = true;
			this.TabTiles.BackColor = System.Drawing.Color.Silver;
			this.TabTiles.Controls.Add(this.PBoxBlockEditor);
			this.TabTiles.Controls.Add(this.groupBox3);
			this.TabTiles.Controls.Add(this.groupBox4);
			this.TabTiles.Controls.Add(this.groupBox1);
			this.TabTiles.Controls.Add(this.groupBox2);
			this.TabTiles.Controls.Add(this.groupBox5);
			this.TabTiles.Location = new System.Drawing.Point(4, 22);
			this.TabTiles.Name = "TabTiles";
			this.TabTiles.Padding = new System.Windows.Forms.Padding(3);
			this.TabTiles.Size = new System.Drawing.Size(766, 583);
			this.TabTiles.TabIndex = 0;
			this.TabTiles.Text = "Tiles";
			// 
			// PBoxBlockEditor
			// 
			this.PBoxBlockEditor.BackColor = System.Drawing.Color.Black;
			this.PBoxBlockEditor.ContextMenuStrip = this.ContextMenuBlockEditor;
			this.PBoxBlockEditor.Location = new System.Drawing.Point(298, 24);
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
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.CBoxCHRBanks);
			this.groupBox3.Controls.Add(this.BtnDeleteCHRBank);
			this.groupBox3.Controls.Add(this.BtnCHRRotate);
			this.groupBox3.Controls.Add(this.BtnCHRHFlip);
			this.groupBox3.Controls.Add(this.BtnCopyCHRBank);
			this.groupBox3.Controls.Add(this.BtnAddCHRBank);
			this.groupBox3.Controls.Add(this.BtnCHRVFlip);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.PBoxCHRBank);
			this.groupBox3.Location = new System.Drawing.Point(6, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(276, 336);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "CHR Bank:";
			// 
			// CBoxCHRBanks
			// 
			this.CBoxCHRBanks.BackColor = System.Drawing.SystemColors.Window;
			this.CBoxCHRBanks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CBoxCHRBanks.FormattingEnabled = true;
			this.CBoxCHRBanks.Location = new System.Drawing.Point(164, 309);
			this.CBoxCHRBanks.Name = "CBoxCHRBanks";
			this.CBoxCHRBanks.Size = new System.Drawing.Size(47, 21);
			this.CBoxCHRBanks.TabIndex = 11;
			this.CBoxCHRBanks.SelectedIndexChanged += new System.EventHandler(this.CHRBankChanged_Event);
			// 
			// BtnDeleteCHRBank
			// 
			this.BtnDeleteCHRBank.BackColor = System.Drawing.Color.Wheat;
			this.BtnDeleteCHRBank.Location = new System.Drawing.Point(217, 308);
			this.BtnDeleteCHRBank.Name = "BtnDeleteCHRBank";
			this.BtnDeleteCHRBank.Size = new System.Drawing.Size(48, 23);
			this.BtnDeleteCHRBank.TabIndex = 12;
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
			this.BtnCopyCHRBank.Location = new System.Drawing.Point(164, 280);
			this.BtnCopyCHRBank.Name = "BtnCopyCHRBank";
			this.BtnCopyCHRBank.Size = new System.Drawing.Size(48, 23);
			this.BtnCopyCHRBank.TabIndex = 9;
			this.BtnCopyCHRBank.Text = "Copy Bank";
			this.BtnCopyCHRBank.UseVisualStyleBackColor = true;
			this.BtnCopyCHRBank.Click += new System.EventHandler(this.BtnCopyCHRBankClick_Event);
			// 
			// BtnAddCHRBank
			// 
			this.BtnAddCHRBank.BackColor = System.Drawing.Color.Wheat;
			this.BtnAddCHRBank.Location = new System.Drawing.Point(217, 280);
			this.BtnAddCHRBank.Name = "BtnAddCHRBank";
			this.BtnAddCHRBank.Size = new System.Drawing.Size(48, 23);
			this.BtnAddCHRBank.TabIndex = 10;
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
			this.label5.Location = new System.Drawing.Point(126, 313);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(43, 16);
			this.label5.TabIndex = 15;
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
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.CheckBoxPalettePerCHR);
			this.groupBox4.Controls.Add(this.Palette3);
			this.groupBox4.Controls.Add(this.Palette2);
			this.groupBox4.Controls.Add(this.Palette1);
			this.groupBox4.Controls.Add(this.Palette0);
			this.groupBox4.Controls.Add(this.PaletteMain);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.label3);
			this.groupBox4.Controls.Add(this.label2);
			this.groupBox4.Controls.Add(this.label1);
			this.groupBox4.Location = new System.Drawing.Point(6, 348);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(276, 181);
			this.groupBox4.TabIndex = 22;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Palettes:";
			// 
			// CheckBoxPalettePerCHR
			// 
			this.CheckBoxPalettePerCHR.Location = new System.Drawing.Point(10, 158);
			this.CheckBoxPalettePerCHR.Name = "CheckBoxPalettePerCHR";
			this.CheckBoxPalettePerCHR.Size = new System.Drawing.Size(148, 19);
			this.CheckBoxPalettePerCHR.TabIndex = 23;
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
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.CBoxBlockObjId);
			this.groupBox1.Controls.Add(this.BtnBlockReserveCHRs);
			this.groupBox1.Controls.Add(this.BtnBlockRotate);
			this.groupBox1.Controls.Add(this.LabelObjId);
			this.groupBox1.Controls.Add(this.BtnEditModeDraw);
			this.groupBox1.Controls.Add(this.BtnUpdateGFX);
			this.groupBox1.Controls.Add(this.BtnEditModeSelectCHR);
			this.groupBox1.Controls.Add(this.BtnBlockHFlip);
			this.groupBox1.Controls.Add(this.BtnBlockVFlip);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Location = new System.Drawing.Point(288, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(276, 364);
			this.groupBox1.TabIndex = 13;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Block Editor:";
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
			this.CBoxBlockObjId.Location = new System.Drawing.Point(227, 333);
			this.CBoxBlockObjId.Name = "CBoxBlockObjId";
			this.CBoxBlockObjId.Size = new System.Drawing.Size(39, 21);
			this.CBoxBlockObjId.TabIndex = 21;
			this.CBoxBlockObjId.SelectionChangeCommitted += new System.EventHandler(this.CBoxBlockObjIdChanged_Event);
			// 
			// BtnBlockReserveCHRs
			// 
			this.BtnBlockReserveCHRs.Location = new System.Drawing.Point(10, 334);
			this.BtnBlockReserveCHRs.Name = "BtnBlockReserveCHRs";
			this.BtnBlockReserveCHRs.Size = new System.Drawing.Size(93, 22);
			this.BtnBlockReserveCHRs.TabIndex = 17;
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
			this.BtnBlockRotate.TabIndex = 16;
			this.BtnBlockRotate.Text = "Rotate";
			this.BtnBlockRotate.UseVisualStyleBackColor = false;
			this.BtnBlockRotate.Click += new System.EventHandler(this.BtnBlockRotateClick_Event);
			// 
			// LabelObjId
			// 
			this.LabelObjId.Location = new System.Drawing.Point(160, 337);
			this.LabelObjId.Name = "LabelObjId";
			this.LabelObjId.Size = new System.Drawing.Size(67, 16);
			this.LabelObjId.TabIndex = 22;
			this.LabelObjId.Text = "Property Id:";
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
			this.BtnUpdateGFX.TabIndex = 20;
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
			this.BtnBlockHFlip.TabIndex = 15;
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
			this.BtnBlockVFlip.TabIndex = 14;
			this.BtnBlockVFlip.Text = "VFlip";
			this.BtnBlockVFlip.UseVisualStyleBackColor = false;
			this.BtnBlockVFlip.Click += new System.EventHandler(this.BtnBlockVFlipClick_Event);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(119, 286);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(43, 20);
			this.label10.TabIndex = 22;
			this.label10.Text = "Modes:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.PanelBlocks);
			this.groupBox2.Location = new System.Drawing.Point(570, 6);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(192, 573);
			this.groupBox2.TabIndex = 27;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Blocks:";
			// 
			// PanelBlocks
			// 
			this.PanelBlocks.AutoScroll = true;
			this.PanelBlocks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.PanelBlocks.Cursor = System.Windows.Forms.Cursors.Hand;
			this.PanelBlocks.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.PanelBlocks.Location = new System.Drawing.Point(6, 14);
			this.PanelBlocks.Name = "PanelBlocks";
			this.PanelBlocks.Size = new System.Drawing.Size(180, 553);
			this.PanelBlocks.TabIndex = 28;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.BtnTileReserveBlocks);
			this.groupBox5.Controls.Add(this.CheckBoxTileEditorLock);
			this.groupBox5.Controls.Add(this.PBoxTilePreview);
			this.groupBox5.Location = new System.Drawing.Point(288, 376);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(276, 153);
			this.groupBox5.TabIndex = 24;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Tile Editor:";
			// 
			// BtnTileReserveBlocks
			// 
			this.BtnTileReserveBlocks.Location = new System.Drawing.Point(145, 101);
			this.BtnTileReserveBlocks.Name = "BtnTileReserveBlocks";
			this.BtnTileReserveBlocks.Size = new System.Drawing.Size(122, 22);
			this.BtnTileReserveBlocks.TabIndex = 25;
			this.BtnTileReserveBlocks.Text = "Reserve Blocks";
			this.BtnTileReserveBlocks.UseVisualStyleBackColor = true;
			this.BtnTileReserveBlocks.Click += new System.EventHandler(this.BtnTileReserveBlocksClick_Event);
			// 
			// CheckBoxTileEditorLock
			// 
			this.CheckBoxTileEditorLock.Location = new System.Drawing.Point(145, 129);
			this.CheckBoxTileEditorLock.Name = "CheckBoxTileEditorLock";
			this.CheckBoxTileEditorLock.Size = new System.Drawing.Size(63, 19);
			this.CheckBoxTileEditorLock.TabIndex = 26;
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
			// TabScreenEditor
			// 
			this.TabScreenEditor.AutoScroll = true;
			this.TabScreenEditor.BackColor = System.Drawing.Color.Silver;
			this.TabScreenEditor.Controls.Add(this.BtnPalette);
			this.TabScreenEditor.Controls.Add(this.groupBox8);
			this.TabScreenEditor.Controls.Add(this.LabelActiveTile);
			this.TabScreenEditor.Controls.Add(this.groupBox6);
			this.TabScreenEditor.Controls.Add(this.CheckBoxScreenShowGrid);
			this.TabScreenEditor.Controls.Add(this.BtnDeleteScreen);
			this.TabScreenEditor.Controls.Add(this.BtnCopyScreen);
			this.TabScreenEditor.Controls.Add(this.BtnCreateScreen);
			this.TabScreenEditor.Controls.Add(this.PBoxScreen);
			this.TabScreenEditor.Controls.Add(this.PBoxActiveTile);
			this.TabScreenEditor.Location = new System.Drawing.Point(4, 22);
			this.TabScreenEditor.Name = "TabScreenEditor";
			this.TabScreenEditor.Padding = new System.Windows.Forms.Padding(3);
			this.TabScreenEditor.Size = new System.Drawing.Size(766, 583);
			this.TabScreenEditor.TabIndex = 1;
			this.TabScreenEditor.Text = "Screens";
			// 
			// BtnPalette
			// 
			this.BtnPalette.Location = new System.Drawing.Point(674, 73);
			this.BtnPalette.Name = "BtnPalette";
			this.BtnPalette.Size = new System.Drawing.Size(79, 23);
			this.BtnPalette.TabIndex = 50;
			this.BtnPalette.Text = "PALETTE";
			this.BtnPalette.UseVisualStyleBackColor = true;
			this.BtnPalette.Click += new System.EventHandler(this.BtnPaletteClick_Event);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.RBtnScreenEditModeLayout);
			this.groupBox8.Controls.Add(this.RBtnScreenEditModeSingle);
			this.groupBox8.Location = new System.Drawing.Point(666, 3);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(94, 64);
			this.groupBox8.TabIndex = 47;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Edit Mode:";
			// 
			// RBtnScreenEditModeLayout
			// 
			this.RBtnScreenEditModeLayout.Location = new System.Drawing.Point(15, 37);
			this.RBtnScreenEditModeLayout.Name = "RBtnScreenEditModeLayout";
			this.RBtnScreenEditModeLayout.Size = new System.Drawing.Size(66, 21);
			this.RBtnScreenEditModeLayout.TabIndex = 49;
			this.RBtnScreenEditModeLayout.TabStop = true;
			this.RBtnScreenEditModeLayout.Text = "Layout";
			this.RBtnScreenEditModeLayout.UseVisualStyleBackColor = true;
			this.RBtnScreenEditModeLayout.CheckedChanged += new System.EventHandler(this.RBtnScreenEditModeLayoutChanged_Event);
			// 
			// RBtnScreenEditModeSingle
			// 
			this.RBtnScreenEditModeSingle.Location = new System.Drawing.Point(15, 17);
			this.RBtnScreenEditModeSingle.Name = "RBtnScreenEditModeSingle";
			this.RBtnScreenEditModeSingle.Size = new System.Drawing.Size(66, 20);
			this.RBtnScreenEditModeSingle.TabIndex = 48;
			this.RBtnScreenEditModeSingle.TabStop = true;
			this.RBtnScreenEditModeSingle.Text = "Single";
			this.RBtnScreenEditModeSingle.UseVisualStyleBackColor = true;
			this.RBtnScreenEditModeSingle.CheckedChanged += new System.EventHandler(this.RBtnScreenEditModeSingleChanged_Event);
			// 
			// LabelActiveTile
			// 
			this.LabelActiveTile.Location = new System.Drawing.Point(589, 3);
			this.LabelActiveTile.Name = "LabelActiveTile";
			this.LabelActiveTile.Size = new System.Drawing.Size(72, 14);
			this.LabelActiveTile.TabIndex = 7;
			this.LabelActiveTile.Text = "...";
			this.LabelActiveTile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.ListBoxScreens);
			this.groupBox6.Location = new System.Drawing.Point(588, 177);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(75, 372);
			this.groupBox6.TabIndex = 44;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Screens:";
			// 
			// ListBoxScreens
			// 
			this.ListBoxScreens.BackColor = System.Drawing.Color.LightGray;
			this.ListBoxScreens.FormattingEnabled = true;
			this.ListBoxScreens.Location = new System.Drawing.Point(6, 19);
			this.ListBoxScreens.Name = "ListBoxScreens";
			this.ListBoxScreens.Size = new System.Drawing.Size(63, 342);
			this.ListBoxScreens.TabIndex = 45;
			this.ListBoxScreens.Click += new System.EventHandler(this.ListBoxScreensClick_Event);
			// 
			// CheckBoxScreenShowGrid
			// 
			this.CheckBoxScreenShowGrid.Checked = true;
			this.CheckBoxScreenShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxScreenShowGrid.Location = new System.Drawing.Point(588, 555);
			this.CheckBoxScreenShowGrid.Name = "CheckBoxScreenShowGrid";
			this.CheckBoxScreenShowGrid.Size = new System.Drawing.Size(78, 22);
			this.CheckBoxScreenShowGrid.TabIndex = 46;
			this.CheckBoxScreenShowGrid.Text = "Show Grid";
			this.CheckBoxScreenShowGrid.UseVisualStyleBackColor = true;
			this.CheckBoxScreenShowGrid.CheckedChanged += new System.EventHandler(this.CheckBoxScreenShowGridChecked_Event);
			// 
			// BtnDeleteScreen
			// 
			this.BtnDeleteScreen.Location = new System.Drawing.Point(588, 148);
			this.BtnDeleteScreen.Name = "BtnDeleteScreen";
			this.BtnDeleteScreen.Size = new System.Drawing.Size(75, 23);
			this.BtnDeleteScreen.TabIndex = 43;
			this.BtnDeleteScreen.Text = "Delete";
			this.BtnDeleteScreen.UseVisualStyleBackColor = true;
			this.BtnDeleteScreen.Click += new System.EventHandler(this.BtnDeleteScreenClick_Event);
			// 
			// BtnCopyScreen
			// 
			this.BtnCopyScreen.Location = new System.Drawing.Point(588, 119);
			this.BtnCopyScreen.Name = "BtnCopyScreen";
			this.BtnCopyScreen.Size = new System.Drawing.Size(75, 23);
			this.BtnCopyScreen.TabIndex = 42;
			this.BtnCopyScreen.Text = "Copy";
			this.BtnCopyScreen.UseVisualStyleBackColor = true;
			this.BtnCopyScreen.Click += new System.EventHandler(this.BtnCopyScreenClick_Event);
			// 
			// BtnCreateScreen
			// 
			this.BtnCreateScreen.Location = new System.Drawing.Point(588, 90);
			this.BtnCreateScreen.Name = "BtnCreateScreen";
			this.BtnCreateScreen.Size = new System.Drawing.Size(75, 23);
			this.BtnCreateScreen.TabIndex = 41;
			this.BtnCreateScreen.Text = "Create";
			this.BtnCreateScreen.UseVisualStyleBackColor = true;
			this.BtnCreateScreen.Click += new System.EventHandler(this.BtnCreateScreenClick_Event);
			// 
			// PBoxScreen
			// 
			this.PBoxScreen.BackColor = System.Drawing.Color.Black;
			this.PBoxScreen.Location = new System.Drawing.Point(3, 4);
			this.PBoxScreen.Name = "PBoxScreen";
			this.PBoxScreen.Size = new System.Drawing.Size(576, 576);
			this.PBoxScreen.TabIndex = 0;
			this.PBoxScreen.TabStop = false;
			// 
			// PBoxActiveTile
			// 
			this.PBoxActiveTile.BackColor = System.Drawing.Color.Black;
			this.PBoxActiveTile.Location = new System.Drawing.Point(593, 20);
			this.PBoxActiveTile.Name = "PBoxActiveTile";
			this.PBoxActiveTile.Size = new System.Drawing.Size(64, 64);
			this.PBoxActiveTile.TabIndex = 6;
			this.PBoxActiveTile.TabStop = false;
			// 
			// TabLayout
			// 
			this.TabLayout.BackColor = System.Drawing.Color.Silver;
			this.TabLayout.Controls.Add(this.tabControlScreensEntities);
			this.TabLayout.Controls.Add(this.groupBox7);
			this.TabLayout.Controls.Add(this.BtnCopyLayout);
			this.TabLayout.Controls.Add(this.BtnDeleteLayout);
			this.TabLayout.Controls.Add(this.BtnCreateLayout);
			this.TabLayout.Controls.Add(this.LayoutLabel);
			this.TabLayout.Controls.Add(this.BtnLayoutRemoveRightColumn);
			this.TabLayout.Controls.Add(this.BtnLayoutAddRightColumn);
			this.TabLayout.Controls.Add(this.BtnLayoutRemoveLeftColumn);
			this.TabLayout.Controls.Add(this.BtnLayoutAddLeftColumn);
			this.TabLayout.Controls.Add(this.BtnLayoutRemoveDownRow);
			this.TabLayout.Controls.Add(this.BtnLayoutAddDownRow);
			this.TabLayout.Controls.Add(this.BtnLayoutRemoveUpRow);
			this.TabLayout.Controls.Add(this.BtnLayoutAddUpRow);
			this.TabLayout.Controls.Add(this.PBoxLayout);
			this.TabLayout.Controls.Add(this.groupBox13);
			this.TabLayout.Location = new System.Drawing.Point(4, 22);
			this.TabLayout.Name = "TabLayout";
			this.TabLayout.Padding = new System.Windows.Forms.Padding(3);
			this.TabLayout.Size = new System.Drawing.Size(1026, 615);
			this.TabLayout.TabIndex = 1;
			this.TabLayout.Text = "Layout";
			// 
			// tabControlScreensEntities
			// 
			this.tabControlScreensEntities.Controls.Add(this.TabScreenList);
			this.tabControlScreensEntities.Controls.Add(this.TabEntities);
			this.tabControlScreensEntities.Location = new System.Drawing.Point(0, 6);
			this.tabControlScreensEntities.Name = "tabControlScreensEntities";
			this.tabControlScreensEntities.SelectedIndex = 0;
			this.tabControlScreensEntities.Size = new System.Drawing.Size(326, 609);
			this.tabControlScreensEntities.TabIndex = 60;
			this.tabControlScreensEntities.Selected += new System.Windows.Forms.TabControlEventHandler(this.TabControlScreensEntitiesSelected_Event);
			// 
			// TabScreenList
			// 
			this.TabScreenList.BackColor = System.Drawing.Color.LightGray;
			this.TabScreenList.Controls.Add(this.CheckBoxLayoutEditorAllBanks);
			this.TabScreenList.Controls.Add(this.LabelLayoutEditorCHRBankID);
			this.TabScreenList.Controls.Add(this.ListViewScreens);
			this.TabScreenList.Controls.Add(this.label9);
			this.TabScreenList.Controls.Add(this.BtnUpdateScreens);
			this.TabScreenList.Controls.Add(this.CheckBoxScreensAutoUpdate);
			this.TabScreenList.Location = new System.Drawing.Point(4, 22);
			this.TabScreenList.Name = "TabScreenList";
			this.TabScreenList.Padding = new System.Windows.Forms.Padding(3);
			this.TabScreenList.Size = new System.Drawing.Size(318, 583);
			this.TabScreenList.TabIndex = 0;
			this.TabScreenList.Text = "Screens";
			// 
			// CheckBoxLayoutEditorAllBanks
			// 
			this.CheckBoxLayoutEditorAllBanks.Location = new System.Drawing.Point(248, 6);
			this.CheckBoxLayoutEditorAllBanks.Name = "CheckBoxLayoutEditorAllBanks";
			this.CheckBoxLayoutEditorAllBanks.Size = new System.Drawing.Size(73, 20);
			this.CheckBoxLayoutEditorAllBanks.TabIndex = 63;
			this.CheckBoxLayoutEditorAllBanks.Text = "All Banks";
			this.CheckBoxLayoutEditorAllBanks.UseVisualStyleBackColor = true;
			this.CheckBoxLayoutEditorAllBanks.CheckedChanged += new System.EventHandler(this.CheckBoxLayoutEditorAllBanksCheckChanged_Event);
			// 
			// LabelLayoutEditorCHRBankID
			// 
			this.LabelLayoutEditorCHRBankID.Location = new System.Drawing.Point(218, 9);
			this.LabelLayoutEditorCHRBankID.Name = "LabelLayoutEditorCHRBankID";
			this.LabelLayoutEditorCHRBankID.Size = new System.Drawing.Size(29, 15);
			this.LabelLayoutEditorCHRBankID.TabIndex = 4;
			this.LabelLayoutEditorCHRBankID.Text = "000";
			// 
			// ListViewScreens
			// 
			this.ListViewScreens.BackColor = System.Drawing.Color.Silver;
			this.ListViewScreens.HideSelection = false;
			this.ListViewScreens.Location = new System.Drawing.Point(0, 31);
			this.ListViewScreens.Name = "ListViewScreens";
			this.ListViewScreens.Size = new System.Drawing.Size(319, 557);
			this.ListViewScreens.TabIndex = 64;
			this.ListViewScreens.UseCompatibleStateImageBehavior = false;
			this.ListViewScreens.SelectedIndexChanged += new System.EventHandler(this.ListViewScreensClick_Event);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(158, 9);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(62, 18);
			this.label9.TabIndex = 3;
			this.label9.Text = "CHR Bank:";
			// 
			// BtnUpdateScreens
			// 
			this.BtnUpdateScreens.BackColor = System.Drawing.Color.Coral;
			this.BtnUpdateScreens.Location = new System.Drawing.Point(4, 4);
			this.BtnUpdateScreens.Name = "BtnUpdateScreens";
			this.BtnUpdateScreens.Size = new System.Drawing.Size(94, 23);
			this.BtnUpdateScreens.TabIndex = 61;
			this.BtnUpdateScreens.Text = "Update Screens";
			this.BtnUpdateScreens.UseVisualStyleBackColor = true;
			this.BtnUpdateScreens.Click += new System.EventHandler(this.BtnUpdateScreensClick_Event);
			// 
			// CheckBoxScreensAutoUpdate
			// 
			this.CheckBoxScreensAutoUpdate.Location = new System.Drawing.Point(103, 4);
			this.CheckBoxScreensAutoUpdate.Name = "CheckBoxScreensAutoUpdate";
			this.CheckBoxScreensAutoUpdate.Size = new System.Drawing.Size(50, 24);
			this.CheckBoxScreensAutoUpdate.TabIndex = 62;
			this.CheckBoxScreensAutoUpdate.Text = "Auto";
			this.CheckBoxScreensAutoUpdate.UseVisualStyleBackColor = true;
			this.CheckBoxScreensAutoUpdate.CheckedChanged += new System.EventHandler(this.CheckBoxScreensAutoUpdateChanged_Event);
			// 
			// TabEntities
			// 
			this.TabEntities.BackColor = System.Drawing.Color.Silver;
			this.TabEntities.Controls.Add(this.groupBoxEntitiesTreeView);
			this.TabEntities.Controls.Add(this.groupBoxEntityEditor);
			this.TabEntities.Location = new System.Drawing.Point(4, 22);
			this.TabEntities.Name = "TabEntities";
			this.TabEntities.Padding = new System.Windows.Forms.Padding(3);
			this.TabEntities.Size = new System.Drawing.Size(318, 583);
			this.TabEntities.TabIndex = 1;
			this.TabEntities.Text = "Entities";
			// 
			// groupBoxEntitiesTreeView
			// 
			this.groupBoxEntitiesTreeView.Controls.Add(this.BtnEntityRename);
			this.groupBoxEntitiesTreeView.Controls.Add(this.BtnEntitiesEditInstancesMode);
			this.groupBoxEntitiesTreeView.Controls.Add(this.groupBox10);
			this.groupBoxEntitiesTreeView.Controls.Add(this.TreeViewEntities);
			this.groupBoxEntitiesTreeView.Controls.Add(this.groupBox9);
			this.groupBoxEntitiesTreeView.Controls.Add(this.CheckBoxEntitySnapping);
			this.groupBoxEntitiesTreeView.Location = new System.Drawing.Point(5, 0);
			this.groupBoxEntitiesTreeView.Name = "groupBoxEntitiesTreeView";
			this.groupBoxEntitiesTreeView.Size = new System.Drawing.Size(309, 293);
			this.groupBoxEntitiesTreeView.TabIndex = 100;
			this.groupBoxEntitiesTreeView.TabStop = false;
			// 
			// BtnEntityRename
			// 
			this.BtnEntityRename.Location = new System.Drawing.Point(211, 179);
			this.BtnEntityRename.Name = "BtnEntityRename";
			this.BtnEntityRename.Size = new System.Drawing.Size(82, 23);
			this.BtnEntityRename.TabIndex = 108;
			this.BtnEntityRename.Text = "Rename";
			this.BtnEntityRename.UseVisualStyleBackColor = true;
			this.BtnEntityRename.Click += new System.EventHandler(this.BtnEntityRenameClick_Event);
			// 
			// BtnEntitiesEditInstancesMode
			// 
			this.BtnEntitiesEditInstancesMode.BackColor = System.Drawing.Color.LightSteelBlue;
			this.BtnEntitiesEditInstancesMode.Location = new System.Drawing.Point(203, 230);
			this.BtnEntitiesEditInstancesMode.Name = "BtnEntitiesEditInstancesMode";
			this.BtnEntitiesEditInstancesMode.Size = new System.Drawing.Size(99, 31);
			this.BtnEntitiesEditInstancesMode.TabIndex = 109;
			this.BtnEntitiesEditInstancesMode.Text = "Edit Instances -->";
			this.BtnEntitiesEditInstancesMode.UseVisualStyleBackColor = false;
			this.BtnEntitiesEditInstancesMode.Click += new System.EventHandler(this.BtnEntitiesEditInstancesModeClick_Event);
			// 
			// groupBox10
			// 
			this.groupBox10.Controls.Add(this.BtnEntityAdd);
			this.groupBox10.Controls.Add(this.BtnEntityDelete);
			this.groupBox10.Location = new System.Drawing.Point(203, 93);
			this.groupBox10.Name = "groupBox10";
			this.groupBox10.Size = new System.Drawing.Size(99, 80);
			this.groupBox10.TabIndex = 105;
			this.groupBox10.TabStop = false;
			this.groupBox10.Text = "Entity";
			// 
			// BtnEntityAdd
			// 
			this.BtnEntityAdd.Location = new System.Drawing.Point(8, 19);
			this.BtnEntityAdd.Name = "BtnEntityAdd";
			this.BtnEntityAdd.Size = new System.Drawing.Size(82, 23);
			this.BtnEntityAdd.TabIndex = 106;
			this.BtnEntityAdd.Text = "Add";
			this.BtnEntityAdd.UseVisualStyleBackColor = true;
			this.BtnEntityAdd.Click += new System.EventHandler(this.BtnEntityAddClick_Event);
			// 
			// BtnEntityDelete
			// 
			this.BtnEntityDelete.Location = new System.Drawing.Point(8, 48);
			this.BtnEntityDelete.Name = "BtnEntityDelete";
			this.BtnEntityDelete.Size = new System.Drawing.Size(82, 23);
			this.BtnEntityDelete.TabIndex = 107;
			this.BtnEntityDelete.Text = "Delete";
			this.BtnEntityDelete.UseVisualStyleBackColor = true;
			this.BtnEntityDelete.Click += new System.EventHandler(this.BtnEntityDeleteClick_Event);
			// 
			// TreeViewEntities
			// 
			this.TreeViewEntities.AllowDrop = true;
			this.TreeViewEntities.BackColor = System.Drawing.Color.LightGray;
			this.TreeViewEntities.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.TreeViewEntities.ContextMenuStrip = this.ContextMenuEntitiesTree;
			this.TreeViewEntities.HideSelection = false;
			this.TreeViewEntities.LabelEdit = true;
			this.TreeViewEntities.Location = new System.Drawing.Point(7, 13);
			this.TreeViewEntities.Name = "TreeViewEntities";
			this.TreeViewEntities.Size = new System.Drawing.Size(190, 274);
			this.TreeViewEntities.TabIndex = 101;
			this.TreeViewEntities.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeViewEntitiesLabelEdit_Event);
			this.TreeViewEntities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewEntitiesSelect_Event);
			// 
			// ContextMenuEntitiesTree
			// 
			this.ContextMenuEntitiesTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.addGroupToolStripMenuItem,
									this.removeGroupToolStripMenuItem});
			this.ContextMenuEntitiesTree.Name = "ContextMenuEntitiesTree";
			this.ContextMenuEntitiesTree.Size = new System.Drawing.Size(144, 48);
			// 
			// addGroupToolStripMenuItem
			// 
			this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
			this.addGroupToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.addGroupToolStripMenuItem.Text = "&Add Group";
			this.addGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupAddClick_Event);
			// 
			// removeGroupToolStripMenuItem
			// 
			this.removeGroupToolStripMenuItem.Name = "removeGroupToolStripMenuItem";
			this.removeGroupToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.removeGroupToolStripMenuItem.Text = "&Delete Group";
			this.removeGroupToolStripMenuItem.Click += new System.EventHandler(this.BtnEntityGroupDeleteClick_Event);
			// 
			// groupBox9
			// 
			this.groupBox9.Controls.Add(this.BtnEntityGroupAdd);
			this.groupBox9.Controls.Add(this.BtnEntityGroupDelete);
			this.groupBox9.Location = new System.Drawing.Point(203, 11);
			this.groupBox9.Name = "groupBox9";
			this.groupBox9.Size = new System.Drawing.Size(99, 80);
			this.groupBox9.TabIndex = 102;
			this.groupBox9.TabStop = false;
			this.groupBox9.Text = "Group";
			// 
			// BtnEntityGroupAdd
			// 
			this.BtnEntityGroupAdd.Location = new System.Drawing.Point(8, 19);
			this.BtnEntityGroupAdd.Name = "BtnEntityGroupAdd";
			this.BtnEntityGroupAdd.Size = new System.Drawing.Size(82, 23);
			this.BtnEntityGroupAdd.TabIndex = 103;
			this.BtnEntityGroupAdd.Text = "Add";
			this.BtnEntityGroupAdd.UseVisualStyleBackColor = true;
			this.BtnEntityGroupAdd.Click += new System.EventHandler(this.BtnEntityGroupAddClick_Event);
			// 
			// BtnEntityGroupDelete
			// 
			this.BtnEntityGroupDelete.Location = new System.Drawing.Point(8, 48);
			this.BtnEntityGroupDelete.Name = "BtnEntityGroupDelete";
			this.BtnEntityGroupDelete.Size = new System.Drawing.Size(82, 23);
			this.BtnEntityGroupDelete.TabIndex = 104;
			this.BtnEntityGroupDelete.Text = "Delete";
			this.BtnEntityGroupDelete.UseVisualStyleBackColor = true;
			this.BtnEntityGroupDelete.Click += new System.EventHandler(this.BtnEntityGroupDeleteClick_Event);
			// 
			// CheckBoxEntitySnapping
			// 
			this.CheckBoxEntitySnapping.Checked = true;
			this.CheckBoxEntitySnapping.CheckState = System.Windows.Forms.CheckState.Checked;
			this.CheckBoxEntitySnapping.Location = new System.Drawing.Point(203, 273);
			this.CheckBoxEntitySnapping.Name = "CheckBoxEntitySnapping";
			this.CheckBoxEntitySnapping.Size = new System.Drawing.Size(99, 17);
			this.CheckBoxEntitySnapping.TabIndex = 110;
			this.CheckBoxEntitySnapping.Text = "Snap 8pix";
			this.CheckBoxEntitySnapping.UseVisualStyleBackColor = true;
			this.CheckBoxEntitySnapping.CheckedChanged += new System.EventHandler(this.CheckBoxEntitySnappingChanged_Event);
			// 
			// groupBoxEntityEditor
			// 
			this.groupBoxEntityEditor.Controls.Add(this.CheckBoxPickupTargetEntity);
			this.groupBoxEntityEditor.Controls.Add(this.PBoxEntityPreview);
			this.groupBoxEntityEditor.Controls.Add(this.NumericUpDownEntityUID);
			this.groupBoxEntityEditor.Controls.Add(this.PBoxColor);
			this.groupBoxEntityEditor.Controls.Add(this.label12);
			this.groupBoxEntityEditor.Controls.Add(this.label11);
			this.groupBoxEntityEditor.Controls.Add(this.label8);
			this.groupBoxEntityEditor.Controls.Add(this.groupBox11);
			this.groupBoxEntityEditor.Controls.Add(this.groupBox12);
			this.groupBoxEntityEditor.Controls.Add(this.BtnEntityLoadBitmap);
			this.groupBoxEntityEditor.Controls.Add(this.LabelEntityName);
			this.groupBoxEntityEditor.Controls.Add(this.TextBoxEntityProperties);
			this.groupBoxEntityEditor.Controls.Add(this.TextBoxEntityInstanceProp);
			this.groupBoxEntityEditor.Controls.Add(this.CBoxEntityPreviewScaleX2);
			this.groupBoxEntityEditor.Location = new System.Drawing.Point(5, 295);
			this.groupBoxEntityEditor.Name = "groupBoxEntityEditor";
			this.groupBoxEntityEditor.Size = new System.Drawing.Size(309, 283);
			this.groupBoxEntityEditor.TabIndex = 111;
			this.groupBoxEntityEditor.TabStop = false;
			this.groupBoxEntityEditor.Text = "Active Entity";
			// 
			// CheckBoxPickupTargetEntity
			// 
			this.CheckBoxPickupTargetEntity.Appearance = System.Windows.Forms.Appearance.Button;
			this.CheckBoxPickupTargetEntity.Location = new System.Drawing.Point(203, 87);
			this.CheckBoxPickupTargetEntity.Name = "CheckBoxPickupTargetEntity";
			this.CheckBoxPickupTargetEntity.Size = new System.Drawing.Size(99, 24);
			this.CheckBoxPickupTargetEntity.TabIndex = 116;
			this.CheckBoxPickupTargetEntity.Text = "Target UID:";
			this.CheckBoxPickupTargetEntity.UseVisualStyleBackColor = true;
			this.CheckBoxPickupTargetEntity.CheckedChanged += new System.EventHandler(this.CheckBoxPickupTargetEntityChanged_Event);
			// 
			// PBoxEntityPreview
			// 
			this.PBoxEntityPreview.BackColor = System.Drawing.Color.Black;
			this.PBoxEntityPreview.Location = new System.Drawing.Point(7, 87);
			this.PBoxEntityPreview.Name = "PBoxEntityPreview";
			this.PBoxEntityPreview.Size = new System.Drawing.Size(190, 190);
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
			this.PBoxColor.Location = new System.Drawing.Point(203, 215);
			this.PBoxColor.Name = "PBoxColor";
			this.PBoxColor.Size = new System.Drawing.Size(23, 23);
			this.PBoxColor.TabIndex = 6;
			this.PBoxColor.TabStop = false;
			this.PBoxColor.Click += new System.EventHandler(this.PBoxColorClick);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 41);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(75, 14);
			this.label12.TabIndex = 5;
			this.label12.Text = "Properties:";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(7, 63);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(75, 14);
			this.label11.TabIndex = 5;
			this.label11.Text = "Instance prop:";
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
			this.groupBox11.Location = new System.Drawing.Point(203, 116);
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
			this.groupBox12.Location = new System.Drawing.Point(203, 164);
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
			this.BtnEntityLoadBitmap.Location = new System.Drawing.Point(228, 215);
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
			this.LabelEntityName.Size = new System.Drawing.Size(217, 17);
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
			this.TextBoxEntityProperties.Size = new System.Drawing.Size(217, 20);
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
			this.TextBoxEntityInstanceProp.Size = new System.Drawing.Size(217, 20);
			this.TextBoxEntityInstanceProp.TabIndex = 115;
			this.TextBoxEntityInstanceProp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxEntityPropertiesKeyPress_Event);
			this.TextBoxEntityInstanceProp.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxEntityInstancePropTextKeyUp_Event);
			// 
			// CBoxEntityPreviewScaleX2
			// 
			this.CBoxEntityPreviewScaleX2.Location = new System.Drawing.Point(203, 263);
			this.CBoxEntityPreviewScaleX2.Name = "CBoxEntityPreviewScaleX2";
			this.CBoxEntityPreviewScaleX2.Size = new System.Drawing.Size(67, 17);
			this.CBoxEntityPreviewScaleX2.TabIndex = 124;
			this.CBoxEntityPreviewScaleX2.Text = "Zoom x2";
			this.CBoxEntityPreviewScaleX2.UseVisualStyleBackColor = true;
			this.CBoxEntityPreviewScaleX2.CheckedChanged += new System.EventHandler(this.CheckBoxEntityPreviewScaleX2CheckedChanged);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.BtnLayoutMoveDown);
			this.groupBox7.Controls.Add(this.BtnLayoutMoveUp);
			this.groupBox7.Controls.Add(this.ListBoxLayouts);
			this.groupBox7.Location = new System.Drawing.Point(941, 110);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(79, 242);
			this.groupBox7.TabIndex = 68;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Layouts:";
			// 
			// BtnLayoutMoveDown
			// 
			this.BtnLayoutMoveDown.Location = new System.Drawing.Point(6, 212);
			this.BtnLayoutMoveDown.Name = "BtnLayoutMoveDown";
			this.BtnLayoutMoveDown.Size = new System.Drawing.Size(67, 23);
			this.BtnLayoutMoveDown.TabIndex = 71;
			this.BtnLayoutMoveDown.Text = "Move Dn";
			this.BtnLayoutMoveDown.UseVisualStyleBackColor = true;
			this.BtnLayoutMoveDown.Click += new System.EventHandler(this.BtnLayoutMoveDownClick_Event);
			// 
			// BtnLayoutMoveUp
			// 
			this.BtnLayoutMoveUp.Location = new System.Drawing.Point(6, 185);
			this.BtnLayoutMoveUp.Name = "BtnLayoutMoveUp";
			this.BtnLayoutMoveUp.Size = new System.Drawing.Size(67, 23);
			this.BtnLayoutMoveUp.TabIndex = 70;
			this.BtnLayoutMoveUp.Text = "Move Up";
			this.BtnLayoutMoveUp.UseVisualStyleBackColor = true;
			this.BtnLayoutMoveUp.Click += new System.EventHandler(this.BtnLayoutMoveUpClick_Event);
			// 
			// ListBoxLayouts
			// 
			this.ListBoxLayouts.BackColor = System.Drawing.Color.LightGray;
			this.ListBoxLayouts.FormattingEnabled = true;
			this.ListBoxLayouts.Location = new System.Drawing.Point(6, 19);
			this.ListBoxLayouts.Name = "ListBoxLayouts";
			this.ListBoxLayouts.Size = new System.Drawing.Size(67, 160);
			this.ListBoxLayouts.TabIndex = 69;
			this.ListBoxLayouts.Click += new System.EventHandler(this.ListBoxLayoutsClick_Event);
			// 
			// BtnCopyLayout
			// 
			this.BtnCopyLayout.Location = new System.Drawing.Point(941, 52);
			this.BtnCopyLayout.Name = "BtnCopyLayout";
			this.BtnCopyLayout.Size = new System.Drawing.Size(79, 23);
			this.BtnCopyLayout.TabIndex = 66;
			this.BtnCopyLayout.Text = "Copy";
			this.BtnCopyLayout.UseVisualStyleBackColor = true;
			this.BtnCopyLayout.Click += new System.EventHandler(this.BtnCopyLayoutClick_Event);
			// 
			// BtnDeleteLayout
			// 
			this.BtnDeleteLayout.Location = new System.Drawing.Point(941, 81);
			this.BtnDeleteLayout.Name = "BtnDeleteLayout";
			this.BtnDeleteLayout.Size = new System.Drawing.Size(79, 23);
			this.BtnDeleteLayout.TabIndex = 67;
			this.BtnDeleteLayout.Text = "Delete";
			this.BtnDeleteLayout.UseVisualStyleBackColor = true;
			this.BtnDeleteLayout.Click += new System.EventHandler(this.BtnDeleteLayoutClick_Event);
			// 
			// BtnCreateLayout
			// 
			this.BtnCreateLayout.Location = new System.Drawing.Point(941, 23);
			this.BtnCreateLayout.Name = "BtnCreateLayout";
			this.BtnCreateLayout.Size = new System.Drawing.Size(79, 23);
			this.BtnCreateLayout.TabIndex = 65;
			this.BtnCreateLayout.Text = "Create";
			this.BtnCreateLayout.UseVisualStyleBackColor = true;
			this.BtnCreateLayout.Click += new System.EventHandler(this.BtnCreateLayoutClick_Event);
			// 
			// LayoutLabel
			// 
			this.LayoutLabel.Location = new System.Drawing.Point(941, 6);
			this.LayoutLabel.Name = "LayoutLabel";
			this.LayoutLabel.Size = new System.Drawing.Size(79, 14);
			this.LayoutLabel.TabIndex = 6;
			this.LayoutLabel.Text = "...";
			// 
			// BtnLayoutRemoveRightColumn
			// 
			this.BtnLayoutRemoveRightColumn.Location = new System.Drawing.Point(980, 445);
			this.BtnLayoutRemoveRightColumn.Name = "BtnLayoutRemoveRightColumn";
			this.BtnLayoutRemoveRightColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveRightColumn.TabIndex = 79;
			this.BtnLayoutRemoveRightColumn.Text = "-R";
			this.BtnLayoutRemoveRightColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveRightColumn.Click += new System.EventHandler(this.BtnLayoutRemoveRightColumnClick_Event);
			// 
			// BtnLayoutAddRightColumn
			// 
			this.BtnLayoutAddRightColumn.Location = new System.Drawing.Point(941, 445);
			this.BtnLayoutAddRightColumn.Name = "BtnLayoutAddRightColumn";
			this.BtnLayoutAddRightColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddRightColumn.TabIndex = 78;
			this.BtnLayoutAddRightColumn.Text = "+R";
			this.BtnLayoutAddRightColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutAddRightColumn.Click += new System.EventHandler(this.BtnLayoutAddRightColumnClick_Event);
			// 
			// BtnLayoutRemoveLeftColumn
			// 
			this.BtnLayoutRemoveLeftColumn.Location = new System.Drawing.Point(980, 416);
			this.BtnLayoutRemoveLeftColumn.Name = "BtnLayoutRemoveLeftColumn";
			this.BtnLayoutRemoveLeftColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveLeftColumn.TabIndex = 77;
			this.BtnLayoutRemoveLeftColumn.Text = "-L";
			this.BtnLayoutRemoveLeftColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveLeftColumn.Click += new System.EventHandler(this.BtnLayoutRemoveLeftColumnClick_Event);
			// 
			// BtnLayoutAddLeftColumn
			// 
			this.BtnLayoutAddLeftColumn.Location = new System.Drawing.Point(941, 416);
			this.BtnLayoutAddLeftColumn.Name = "BtnLayoutAddLeftColumn";
			this.BtnLayoutAddLeftColumn.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddLeftColumn.TabIndex = 76;
			this.BtnLayoutAddLeftColumn.Text = "+L";
			this.BtnLayoutAddLeftColumn.UseVisualStyleBackColor = true;
			this.BtnLayoutAddLeftColumn.Click += new System.EventHandler(this.BtnLayoutAddLeftColumnClick_Event);
			// 
			// BtnLayoutRemoveDownRow
			// 
			this.BtnLayoutRemoveDownRow.Location = new System.Drawing.Point(980, 387);
			this.BtnLayoutRemoveDownRow.Name = "BtnLayoutRemoveDownRow";
			this.BtnLayoutRemoveDownRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveDownRow.TabIndex = 75;
			this.BtnLayoutRemoveDownRow.Text = "-D";
			this.BtnLayoutRemoveDownRow.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveDownRow.Click += new System.EventHandler(this.BtnLayoutRemoveBottomRowClick_Event);
			// 
			// BtnLayoutAddDownRow
			// 
			this.BtnLayoutAddDownRow.Location = new System.Drawing.Point(941, 387);
			this.BtnLayoutAddDownRow.Name = "BtnLayoutAddDownRow";
			this.BtnLayoutAddDownRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddDownRow.TabIndex = 74;
			this.BtnLayoutAddDownRow.Text = "+D";
			this.BtnLayoutAddDownRow.UseVisualStyleBackColor = true;
			this.BtnLayoutAddDownRow.Click += new System.EventHandler(this.BtnLayoutAddDownRowClick_Event);
			// 
			// BtnLayoutRemoveUpRow
			// 
			this.BtnLayoutRemoveUpRow.Location = new System.Drawing.Point(980, 358);
			this.BtnLayoutRemoveUpRow.Name = "BtnLayoutRemoveUpRow";
			this.BtnLayoutRemoveUpRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutRemoveUpRow.TabIndex = 73;
			this.BtnLayoutRemoveUpRow.Text = "-U";
			this.BtnLayoutRemoveUpRow.UseVisualStyleBackColor = true;
			this.BtnLayoutRemoveUpRow.Click += new System.EventHandler(this.BtnLayoutRemoveTopRowClick_Event);
			// 
			// BtnLayoutAddUpRow
			// 
			this.BtnLayoutAddUpRow.Location = new System.Drawing.Point(941, 358);
			this.BtnLayoutAddUpRow.Name = "BtnLayoutAddUpRow";
			this.BtnLayoutAddUpRow.Size = new System.Drawing.Size(40, 23);
			this.BtnLayoutAddUpRow.TabIndex = 72;
			this.BtnLayoutAddUpRow.Text = "+U";
			this.BtnLayoutAddUpRow.UseVisualStyleBackColor = true;
			this.BtnLayoutAddUpRow.Click += new System.EventHandler(this.BtnLayoutAddUpRowClick_Event);
			// 
			// PBoxLayout
			// 
			this.PBoxLayout.BackColor = System.Drawing.Color.Black;
			this.PBoxLayout.ContextMenuStrip = this.ContextMenuLayoutEditor;
			this.PBoxLayout.Location = new System.Drawing.Point(332, 6);
			this.PBoxLayout.Name = "PBoxLayout";
			this.PBoxLayout.Size = new System.Drawing.Size(603, 603);
			this.PBoxLayout.TabIndex = 2;
			this.PBoxLayout.TabStop = false;
			// 
			// ContextMenuLayoutEditor
			// 
			this.ContextMenuLayoutEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.LayoutDeleteScreenToolStripMenuItem,
									this.toolStripSeparator5,
									this.LayoutDeleteEntityToolStripMenuItem,
									this.toolStripSeparator7,
									this.SetStartScreenMarkToolStripMenuItem,
									this.SetScreenMarkToolStripMenuItem,
									this.adjacentScreensDirectionToolStripMenuItem});
			this.ContextMenuLayoutEditor.Name = "ContextMenuLayoutEditor";
			this.ContextMenuLayoutEditor.Size = new System.Drawing.Size(196, 126);
			// 
			// LayoutDeleteScreenToolStripMenuItem
			// 
			this.LayoutDeleteScreenToolStripMenuItem.Name = "LayoutDeleteScreenToolStripMenuItem";
			this.LayoutDeleteScreenToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.LayoutDeleteScreenToolStripMenuItem.Text = "Delete Screen";
			this.LayoutDeleteScreenToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteScreenToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(192, 6);
			// 
			// LayoutDeleteEntityToolStripMenuItem
			// 
			this.LayoutDeleteEntityToolStripMenuItem.Name = "LayoutDeleteEntityToolStripMenuItem";
			this.LayoutDeleteEntityToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.LayoutDeleteEntityToolStripMenuItem.Text = "Delete Entity";
			this.LayoutDeleteEntityToolStripMenuItem.Click += new System.EventHandler(this.LayoutDeleteEntityToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(192, 6);
			// 
			// SetStartScreenMarkToolStripMenuItem
			// 
			this.SetStartScreenMarkToolStripMenuItem.Name = "SetStartScreenMarkToolStripMenuItem";
			this.SetStartScreenMarkToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
			this.SetStartScreenMarkToolStripMenuItem.Text = "Set Start Screen Mark";
			this.SetStartScreenMarkToolStripMenuItem.Click += new System.EventHandler(this.SetStartScreenMarkToolStripMenuItemClick_Event);
			// 
			// SetScreenMarkToolStripMenuItem
			// 
			this.SetScreenMarkToolStripMenuItem.Name = "SetScreenMarkToolStripMenuItem";
			this.SetScreenMarkToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
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
			this.adjacentScreensDirectionToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
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
			// groupBox13
			// 
			this.groupBox13.Controls.Add(this.CheckBoxShowCoords);
			this.groupBox13.Controls.Add(this.CheckBoxShowTargets);
			this.groupBox13.Controls.Add(this.CheckBoxShowMarks);
			this.groupBox13.Controls.Add(this.CheckBoxShowEntities);
			this.groupBox13.Location = new System.Drawing.Point(940, 508);
			this.groupBox13.Name = "groupBox13";
			this.groupBox13.Size = new System.Drawing.Size(80, 101);
			this.groupBox13.TabIndex = 80;
			this.groupBox13.TabStop = false;
			this.groupBox13.Text = "Show";
			// 
			// CheckBoxShowCoords
			// 
			this.CheckBoxShowCoords.Location = new System.Drawing.Point(11, 79);
			this.CheckBoxShowCoords.Name = "CheckBoxShowCoords";
			this.CheckBoxShowCoords.Size = new System.Drawing.Size(63, 17);
			this.CheckBoxShowCoords.TabIndex = 84;
			this.CheckBoxShowCoords.Text = "Coords";
			this.CheckBoxShowCoords.UseVisualStyleBackColor = true;
			this.CheckBoxShowCoords.CheckedChanged += new System.EventHandler(this.CheckBoxShowCoordsChecked_Event);
			// 
			// CheckBoxShowTargets
			// 
			this.CheckBoxShowTargets.Location = new System.Drawing.Point(11, 59);
			this.CheckBoxShowTargets.Name = "CheckBoxShowTargets";
			this.CheckBoxShowTargets.Size = new System.Drawing.Size(63, 17);
			this.CheckBoxShowTargets.TabIndex = 83;
			this.CheckBoxShowTargets.Text = "Targets";
			this.CheckBoxShowTargets.UseVisualStyleBackColor = true;
			this.CheckBoxShowTargets.CheckedChanged += new System.EventHandler(this.CheckBoxShowTargetsChecked_Event);
			// 
			// CheckBoxShowMarks
			// 
			this.CheckBoxShowMarks.Location = new System.Drawing.Point(11, 19);
			this.CheckBoxShowMarks.Name = "CheckBoxShowMarks";
			this.CheckBoxShowMarks.Size = new System.Drawing.Size(60, 17);
			this.CheckBoxShowMarks.TabIndex = 81;
			this.CheckBoxShowMarks.Text = "Marks";
			this.CheckBoxShowMarks.UseVisualStyleBackColor = true;
			this.CheckBoxShowMarks.CheckedChanged += new System.EventHandler(this.CheckBoxShowMarksChecked_Event);
			// 
			// CheckBoxShowEntities
			// 
			this.CheckBoxShowEntities.Location = new System.Drawing.Point(11, 39);
			this.CheckBoxShowEntities.Name = "CheckBoxShowEntities";
			this.CheckBoxShowEntities.Size = new System.Drawing.Size(60, 17);
			this.CheckBoxShowEntities.TabIndex = 82;
			this.CheckBoxShowEntities.Text = "Entities";
			this.CheckBoxShowEntities.UseVisualStyleBackColor = true;
			this.CheckBoxShowEntities.CheckedChanged += new System.EventHandler(this.CheckBoxShowEntitiesChecked_Event);
			// 
			// ContextMenuTilesList
			// 
			this.ContextMenuTilesList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.copyTileToolStripMenuItem,
									this.pasteTileToolStripMenuItem,
									this.separatorToolStripMenuItem4,
									this.clearTileToolStripMenuItem,
									this.clearAllTileToolStripMenuItem,
									this.toolStripSeparator21,
									this.insertLeftTileToolStripMenuItem,
									this.deleteTileToolStripMenuItem3});
			this.ContextMenuTilesList.Name = "ContextMenuTilesList";
			this.ContextMenuTilesList.Size = new System.Drawing.Size(144, 148);
			// 
			// copyTileToolStripMenuItem
			// 
			this.copyTileToolStripMenuItem.Name = "copyTileToolStripMenuItem";
			this.copyTileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.copyTileToolStripMenuItem.Text = "Copy";
			this.copyTileToolStripMenuItem.Click += new System.EventHandler(this.CopyTileToolStripMenuItemClick_Event);
			// 
			// pasteTileToolStripMenuItem
			// 
			this.pasteTileToolStripMenuItem.Name = "pasteTileToolStripMenuItem";
			this.pasteTileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.pasteTileToolStripMenuItem.Text = "Paste";
			this.pasteTileToolStripMenuItem.Click += new System.EventHandler(this.PasteTileToolStripMenuItemClick_Event);
			// 
			// separatorToolStripMenuItem4
			// 
			this.separatorToolStripMenuItem4.Name = "separatorToolStripMenuItem4";
			this.separatorToolStripMenuItem4.Size = new System.Drawing.Size(140, 6);
			// 
			// clearTileToolStripMenuItem
			// 
			this.clearTileToolStripMenuItem.Name = "clearTileToolStripMenuItem";
			this.clearTileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.clearTileToolStripMenuItem.Text = "Clear Refs";
			this.clearTileToolStripMenuItem.Click += new System.EventHandler(this.ClearTileToolStripMenuItemClick_Event);
			// 
			// clearAllTileToolStripMenuItem
			// 
			this.clearAllTileToolStripMenuItem.Name = "clearAllTileToolStripMenuItem";
			this.clearAllTileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.clearAllTileToolStripMenuItem.Text = "Clear All Refs";
			this.clearAllTileToolStripMenuItem.Click += new System.EventHandler(this.ClearAllTileToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator21
			// 
			this.toolStripSeparator21.Name = "toolStripSeparator21";
			this.toolStripSeparator21.Size = new System.Drawing.Size(140, 6);
			// 
			// insertLeftTileToolStripMenuItem
			// 
			this.insertLeftTileToolStripMenuItem.Name = "insertLeftTileToolStripMenuItem";
			this.insertLeftTileToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
			this.insertLeftTileToolStripMenuItem.Text = "Insert Left";
			this.insertLeftTileToolStripMenuItem.Click += new System.EventHandler(this.InsertLeftTileToolStripMenuItemClick_Event);
			// 
			// deleteTileToolStripMenuItem3
			// 
			this.deleteTileToolStripMenuItem3.Name = "deleteTileToolStripMenuItem3";
			this.deleteTileToolStripMenuItem3.Size = new System.Drawing.Size(143, 22);
			this.deleteTileToolStripMenuItem3.Text = "Delete";
			this.deleteTileToolStripMenuItem3.Click += new System.EventHandler(this.DeleteTileToolStripMenuItem3Click_Event);
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
									this.toolStripSeparator22,
									this.insertLeftBlockToolStripMenuItem,
									this.deleteBlockToolStripMenuItem});
			this.ContextMenuBlocksList.Name = "contextMenuStripBlocksList";
			this.ContextMenuBlocksList.Size = new System.Drawing.Size(137, 170);
			// 
			// copyBlockToolStripMenuItem
			// 
			this.copyBlockToolStripMenuItem.Name = "copyBlockToolStripMenuItem";
			this.copyBlockToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyBlockToolStripMenuItem.Text = "Copy";
			this.copyBlockToolStripMenuItem.Click += new System.EventHandler(this.CopyBlockToolStripMenuItemClick_Event);
			// 
			// pasteBlockCloneToolStripMenuItem
			// 
			this.pasteBlockCloneToolStripMenuItem.Name = "pasteBlockCloneToolStripMenuItem";
			this.pasteBlockCloneToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteBlockCloneToolStripMenuItem.Text = "Paste Clone";
			this.pasteBlockCloneToolStripMenuItem.Click += new System.EventHandler(this.PasteBlockCloneToolStripMenuItemClick_Event);
			// 
			// pasteBlockRefsToolStripMenuItem
			// 
			this.pasteBlockRefsToolStripMenuItem.Name = "pasteBlockRefsToolStripMenuItem";
			this.pasteBlockRefsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteBlockRefsToolStripMenuItem.Text = "Paste Refs";
			this.pasteBlockRefsToolStripMenuItem.Click += new System.EventHandler(this.PasteBlockRefsToolStripMenuItemClickEvent);
			// 
			// separatorToolStripMenuItem3
			// 
			this.separatorToolStripMenuItem3.Name = "separatorToolStripMenuItem3";
			this.separatorToolStripMenuItem3.Size = new System.Drawing.Size(133, 6);
			// 
			// clearCHRsBlockToolStripMenuItem
			// 
			this.clearCHRsBlockToolStripMenuItem.Name = "clearCHRsBlockToolStripMenuItem";
			this.clearCHRsBlockToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.clearCHRsBlockToolStripMenuItem.Text = "Clear CHRs";
			this.clearCHRsBlockToolStripMenuItem.Click += new System.EventHandler(this.ClearCHRsBlockToolStripMenuItemClick_Event);
			// 
			// clearRefsBlockToolStripMenuItem
			// 
			this.clearRefsBlockToolStripMenuItem.Name = "clearRefsBlockToolStripMenuItem";
			this.clearRefsBlockToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.clearRefsBlockToolStripMenuItem.Text = "Clear Refs";
			this.clearRefsBlockToolStripMenuItem.Click += new System.EventHandler(this.ClearRefsBlockToolStripMenuItemClick_Event);
			// 
			// toolStripSeparator22
			// 
			this.toolStripSeparator22.Name = "toolStripSeparator22";
			this.toolStripSeparator22.Size = new System.Drawing.Size(133, 6);
			// 
			// insertLeftBlockToolStripMenuItem
			// 
			this.insertLeftBlockToolStripMenuItem.Name = "insertLeftBlockToolStripMenuItem";
			this.insertLeftBlockToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.insertLeftBlockToolStripMenuItem.Text = "Insert Left";
			this.insertLeftBlockToolStripMenuItem.Click += new System.EventHandler(this.InsertLeftBlockToolStripMenuItemClick_Event);
			// 
			// deleteBlockToolStripMenuItem
			// 
			this.deleteBlockToolStripMenuItem.Name = "deleteBlockToolStripMenuItem";
			this.deleteBlockToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.deleteBlockToolStripMenuItem.Text = "Delete";
			this.deleteBlockToolStripMenuItem.Click += new System.EventHandler(this.DeleteBlockToolStripMenuItemClick_Event);
			// 
			// StatusBar
			// 
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.StatusBarLabel});
			this.StatusBar.Location = new System.Drawing.Point(0, 671);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.Size = new System.Drawing.Size(1032, 22);
			this.StatusBar.SizingGrip = false;
			this.StatusBar.TabIndex = 27;
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
			this.Import_openFileDialog.Filter = "SPReD-NES (*.sprednes)|*.sprednes|CHR Bank (*.chr,*.bin)|*.chr;*.bin|Tiles (4bpp" +
			") (*.bmp)|*.bmp|Palette (192 bytes) (*.pal)|*.pal";
			this.Import_openFileDialog.Title = "Import Data Into Active CHR Bank";
			this.Import_openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.DataImportOk_Event);
			// 
			// Project_exportFileDialog
			// 
			this.Project_exportFileDialog.DefaultExt = "asm";
			this.Project_exportFileDialog.Filter = "CA65\\NESasm (*.asm)|*.asm|ZX Asm (*.zxa)|*.zxa|Active Tile\\Block Set (*.bmp)|*.bm" +
			"p|Active Layout (*.png)|*.png|Text (*.json)|*.json";
			this.Project_exportFileDialog.Title = "Export Project";
			this.Project_exportFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ProjectExportOk_Event);
			// 
			// ContextMenuEntitiesTreeEntity
			// 
			this.ContextMenuEntitiesTreeEntity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.toolStripMenuItem1,
									this.toolStripMenuItem2});
			this.ContextMenuEntitiesTreeEntity.Name = "ContextMenuStripEntitiesTreeGoup";
			this.ContextMenuEntitiesTreeEntity.Size = new System.Drawing.Size(118, 48);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
			this.toolStripMenuItem1.Text = "&Delete";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.BtnEntityDeleteClick_Event);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(117, 22);
			this.toolStripMenuItem2.Text = "Re&name";
			this.toolStripMenuItem2.Click += new System.EventHandler(this.BtnEntityRenameClick_Event);
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
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1032, 693);
			this.Controls.Add(this.StatusBar);
			this.Controls.Add(this.tabControlEditorLayout);
			this.Controls.Add(this.MenuStrip);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.MenuStrip;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "MAPeD-NES";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyDown_Event);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyUp_Event);
			this.ContextMenuEntitiesTreeGoup.ResumeLayout(false);
			this.MenuStrip.ResumeLayout(false);
			this.MenuStrip.PerformLayout();
			this.tabControlEditorLayout.ResumeLayout(false);
			this.TabEditor.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.tabControlTilesScreens.ResumeLayout(false);
			this.TabTiles.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxBlockEditor)).EndInit();
			this.ContextMenuBlockEditor.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxCHRBank)).EndInit();
			this.ContextMenuCHRBank.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Palette3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Palette0)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PaletteMain)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxTilePreview)).EndInit();
			this.TabScreenEditor.ResumeLayout(false);
			this.groupBox8.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxScreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.PBoxActiveTile)).EndInit();
			this.TabLayout.ResumeLayout(false);
			this.tabControlScreensEntities.ResumeLayout(false);
			this.TabScreenList.ResumeLayout(false);
			this.TabEntities.ResumeLayout(false);
			this.groupBoxEntitiesTreeView.ResumeLayout(false);
			this.groupBox10.ResumeLayout(false);
			this.ContextMenuEntitiesTree.ResumeLayout(false);
			this.groupBox9.ResumeLayout(false);
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
			this.groupBox7.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PBoxLayout)).EndInit();
			this.ContextMenuLayoutEditor.ResumeLayout(false);
			this.groupBox13.ResumeLayout(false);
			this.ContextMenuTilesList.ResumeLayout(false);
			this.ContextMenuBlocksList.ResumeLayout(false);
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.ContextMenuEntitiesTreeEntity.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem editInstancesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator26;
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
		private System.Windows.Forms.ToolStripMenuItem ScreenEditModeSingleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ScreenEditModeLayoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteAllScreenMarksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteAllEntityInstancesToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
		private System.Windows.Forms.ToolStripMenuItem SetScreenMarkToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SetStartScreenMarkToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.CheckBox CheckBoxShowMarks;
		private System.Windows.Forms.CheckBox CheckBoxShowTargets;
		private System.Windows.Forms.GroupBox groupBox13;
		private System.Windows.Forms.CheckBox CheckBoxPickupTargetEntity;
		private System.Windows.Forms.Button BtnCopyLayout;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxEntitySnapping;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem LayoutDeleteEntityToolStripMenuItem;
		private System.Windows.Forms.CheckBox CheckBoxShowEntities;
		private System.Windows.Forms.ToolStripMenuItem ScreensShowAllBanksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ScreensAutoUpdateToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator20;
		private System.Windows.Forms.ToolStripMenuItem shiftColorsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem shiftTransparencyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editorToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
		private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem ScreenEditShowGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
		private System.Windows.Forms.ToolStripMenuItem openPaletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem TilesLockEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optimizationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotateToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem horizontalFlipToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem verticalFlipToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripMenuItem BlockEditorModeDrawToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem BlockEditorModeSelectToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem updateGFXToolStripMenuItem;
		private System.Windows.Forms.Button BtnEntitiesEditInstancesMode;
		private System.Windows.Forms.CheckBox CBoxEntityPreviewScaleX2;
		private System.Windows.Forms.TextBox TextBoxEntityProperties;
		private System.Windows.Forms.Label label12;
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
		private System.Windows.Forms.GroupBox groupBoxEntitiesTreeView;
		private System.Windows.Forms.Label LabelEntityName;
		private System.Windows.Forms.TextBox TextBoxEntityInstanceProp;
		private System.Windows.Forms.Label label11;
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
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cHRBankToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem layoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem screensToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blocksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem renameEntityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteEntityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addEntityToolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem entitiesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addEntityToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem renameGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteGroupToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuEntitiesTreeGoup;
		private System.Windows.Forms.ToolStripMenuItem removeGroupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip ContextMenuEntitiesTree;
		private System.Windows.Forms.PictureBox PBoxEntityPreview;
		private System.Windows.Forms.TreeView TreeViewEntities;
		private System.Windows.Forms.FlowLayoutPanel PanelTiles;
		private System.Windows.Forms.FlowLayoutPanel PanelBlocks;
		private System.Windows.Forms.TabPage TabEntities;
		private System.Windows.Forms.TabPage TabScreenList;
		private System.Windows.Forms.TabControl tabControlScreensEntities;
		private System.Windows.Forms.Button BtnOptimization;
		private System.Windows.Forms.Button BtnPalette;
		private System.Windows.Forms.CheckBox CheckBoxScreensAutoUpdate;
		private System.Windows.Forms.Button BtnEditModeSelectCHR;
		private System.Windows.Forms.Button BtnEditModeDraw;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label LabelLayoutEditorCHRBankID;
		private System.Windows.Forms.CheckBox CheckBoxLayoutEditorAllBanks;
		private System.Windows.Forms.Button BtnLayoutMoveUp;
		private System.Windows.Forms.Button BtnLayoutMoveDown;
		private System.Windows.Forms.RadioButton RBtnScreenEditModeSingle;
		private System.Windows.Forms.RadioButton RBtnScreenEditModeLayout;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Button BtnCreateLayout;
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
		private System.Windows.Forms.PictureBox PBoxLayout;
		private System.Windows.Forms.Button BtnLayoutAddUpRow;
		private System.Windows.Forms.ToolStripMenuItem clearRefsBlockToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog Project_exportFileDialog;
		private System.Windows.Forms.ToolStripMenuItem clearAllTileToolStripMenuItem;
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
		private System.Windows.Forms.Button BtnCopyScreen;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.PictureBox PBoxActiveTile;
		private System.Windows.Forms.Label LabelActiveTile;
		private System.Windows.Forms.OpenFileDialog Project_openFileDialog;
		private System.Windows.Forms.SaveFileDialog Project_saveFileDialog;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.Button BtnCopyCHRBank;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Button BtnUpdateGFX;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.PictureBox PBoxTilePreview;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ToolStripStatusLabel StatusBarLabel;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.Button BtnCreateScreen;
		private System.Windows.Forms.Button BtnDeleteScreen;
		private System.Windows.Forms.CheckBox CheckBoxScreenShowGrid;
		private System.Windows.Forms.ListBox ListBoxScreens;
		private System.Windows.Forms.PictureBox PBoxScreen;
		private System.Windows.Forms.Button BtnCHRVFlip;
		private System.Windows.Forms.Button BtnCHRHFlip;
		private System.Windows.Forms.Button BtnCHRRotate;
		private System.Windows.Forms.Label LabelObjId;
		private System.Windows.Forms.ComboBox CBoxBlockObjId;
		private System.Windows.Forms.ComboBox CBoxTileViewType;
		private System.Windows.Forms.Button BtnBlockVFlip;
		private System.Windows.Forms.Button BtnBlockHFlip;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.PictureBox PBoxBlockEditor;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button BtnAddCHRBank;
		private System.Windows.Forms.Button BtnDeleteCHRBank;
		private System.Windows.Forms.ComboBox CBoxCHRBanks;
		private System.Windows.Forms.PictureBox PBoxCHRBank;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.PictureBox PaletteMain;
		private System.Windows.Forms.PictureBox Palette0;
		private System.Windows.Forms.PictureBox Palette1;
		private System.Windows.Forms.PictureBox Palette2;
		private System.Windows.Forms.PictureBox Palette3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TabPage TabLayout;
		private System.Windows.Forms.TabPage TabScreenEditor;
		private System.Windows.Forms.TabControl tabControlTilesScreens;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabPage TabTiles;
		private System.Windows.Forms.TabPage TabEditor;
		private System.Windows.Forms.TabControl tabControlEditorLayout;
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