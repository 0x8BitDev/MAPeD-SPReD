/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 01.05.2017
 * Time: 15:24
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace MAPeD
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	/// 
	
	public partial class MainForm : Form
	{
		private readonly progress_form m_progress_form				= null;
		private readonly IProgress< int >	m_progress_val			= null;
		private readonly IProgress< string >	m_progress_status	= null;
		
		private readonly exporter_zx_sjasm 	m_exp_zx_asm	= null;
#if !DEF_ZX
		private readonly exporter_asm 		m_exp_asm		= null;
#endif
#if !DEF_NES
		private readonly swap_colors_form m_swap_colors_form	= null;
#endif
		private readonly data_conversion_options_form m_data_conversion_options_form	= null;
		
		private readonly data_sets_manager m_data_manager	= null;
		private readonly tiles_processor m_tiles_processor	= null;	
		private readonly screen_editor m_screen_editor		= null;
		private readonly layout_editor m_layout_editor		= null;
		
		private readonly tiles_palette_form m_tiles_palette_form		= null;
		private readonly patterns_manager_form m_patterns_manager_form	= null;
		private readonly optimization_form m_optimization_form			= null;
		private readonly object_name_form m_object_name_form			= null;
		private readonly import_tiles_form m_import_tiles_form			= null;
		private readonly screen_mark_form m_screen_mark_form			= null;
		private readonly description_form m_description_form			= null;
		private readonly statistics_form m_statistics_form				= null;
		private readonly create_layout_form m_create_layout_form		= null;
		
		private SPSeD.py_editor		m_py_editor	= null;
		
		private utils.ETileViewType m_view_type	= utils.ETileViewType.tvt_Unknown;

#if DEF_FIXED_LEN_PALETTE16_ARR
		private int	m_palette_copy_ind	= -1;
#endif
		private readonly export_active_tile_block_set_form m_export_active_tile_block_set_form	= null;
		
		private readonly image_preview m_entity_preview	= null;
		
		private int	m_block_copy_item_ind	= -1;
		private int	m_tile_copy_item_ind	= -1;
		
		private readonly imagelist_manager 	m_imagelist_manager	= null;
		private readonly tile_list_manager	m_tile_list_manager	= null;
		
		private static ToolStripStatusLabel	m_status_bar_label = null;
		
		enum ECopyPasteType
		{
			cpt_CHR_bank	= 1,
			cpt_Blocks_list	= 2,
			cpt_Tiles_list	= 4,
			cpt_All			= 7,
		};

		private struct SToolTipData
		{
			public Control m_cntrl;
			public string m_desc;
			
			public SToolTipData( Control _cntrl, string _desc )
			{
				m_cntrl	= _cntrl;
				m_desc 	= _desc;
			}
		};
		
		public MainForm( string[] _args )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			// scale the main window horizontaly
			this.Width += ( 3 * utils.CONST_SCREEN_TILES_SIZE ) - 14;

			m_data_manager 	= new data_sets_manager();
			
			m_progress_val		= new Progress< int >( percent => { progress_bar_value( percent ); } );
			m_progress_status	= new Progress< string >( status => { progress_bar_status( status ); } );
			
			m_progress_form	= new progress_form();
			
			m_exp_zx_asm	= new exporter_zx_sjasm( m_data_manager );
#if !DEF_ZX
			m_exp_asm		= new exporter_asm( m_data_manager );
#endif
#if !DEF_NES
			m_swap_colors_form	= new swap_colors_form();
#endif
			m_data_conversion_options_form	= new data_conversion_options_form();

			m_tiles_processor 	= new tiles_processor(	PBoxCHRBank,
			                                         	GrpBoxCHRBank,
														PBoxBlockEditor,
														PBoxTilePreview,
														PaletteMain,
														Palette0,
														Palette1,
														Palette2,
														Palette3,
														m_data_manager );
			
			m_tiles_processor.subscribe_block_quad_selected_event( MainForm_BlockQuadSelected );

			m_tile_list_manager = new tile_list_manager();
			
			m_imagelist_manager	= new imagelist_manager( PanelTiles, PanelTilesClick_Event, ContextMenuTilesList, PanelBlocks, PanelBlocksClick_Event, ContextMenuBlocksList, ListViewScreens, m_tile_list_manager );
			
			m_screen_editor = new screen_editor( PBoxScreen, m_imagelist_manager.get_tiles_image_list(), m_imagelist_manager.get_blocks_image_list(), PBoxActiveTile, GrpBoxActiveTile );
			m_screen_editor.subscribe_event( m_data_manager );
			
			m_layout_editor = new layout_editor( PBoxLayout, LayoutLabel, m_data_manager.get_tiles_data(), m_imagelist_manager.get_screen_list() );
			m_layout_editor.subscribe_event( m_data_manager );
			m_layout_editor.subscribe_event( m_screen_editor );
			m_layout_editor.EntityInstanceSelected += new EventHandler( MainForm_EntityInstanceSelected );
			
			m_data_manager.SetEntitiesData += new EventHandler( TreeViewEntities_update_data );
			m_data_manager.AddEntity 	+= TreeViewEntities_add_entity;
			m_data_manager.DeleteEntity += TreeViewEntities_delete_entity;
			m_data_manager.AddGroup		+= TreeViewEntities_add_group;
			m_data_manager.DeleteGroup 	+= TreeViewEntities_delete_group;
			
			m_entity_preview = new image_preview( PBoxEntityPreview );
			
			m_screen_editor.subscribe_event( m_layout_editor );
			m_screen_editor.UpdateTileImage += new EventHandler( update_tile_image );
			
			m_status_bar_label = StatusBarLabel;
			
			m_object_name_form					= new object_name_form();
			m_import_tiles_form					= new import_tiles_form();
			m_screen_mark_form					= new screen_mark_form();
			m_description_form					= new description_form();
			m_statistics_form					= new statistics_form( m_data_manager );
			m_create_layout_form				= new create_layout_form();
			m_export_active_tile_block_set_form	= new export_active_tile_block_set_form();

			enable_update_gfx_btn( false );
			enable_update_screens_btn( true );

			m_tiles_palette_form = new tiles_palette_form( m_imagelist_manager.get_tiles_image_list(), ContextMenuTilesList, m_imagelist_manager.get_blocks_image_list(), ContextMenuBlocksList, m_tile_list_manager );
			m_tiles_palette_form.TilesBlocksClosed 	+= new EventHandler( MainForm_TilesBlocksClosed );
			m_tiles_palette_form.TileSelected 		+= new EventHandler( MainForm_TileSelected );
			m_tiles_palette_form.BlockSelected 		+= new EventHandler( MainForm_BlockSelected );
			m_tiles_palette_form.ResetActiveTile 	+= new EventHandler( BtnResetTileClick_Event );
			
			m_patterns_manager_form = new patterns_manager_form( m_imagelist_manager.get_tiles_image_list(), m_imagelist_manager.get_blocks_image_list() );
			m_patterns_manager_form.PatternsManagerClosed 	+= new EventHandler( MainForm_PatternsManagerClosed );
			
			m_patterns_manager_form.subscribe_event( m_screen_editor );
			m_screen_editor.subscribe_event( m_patterns_manager_form );
			
			m_optimization_form = new optimization_form( m_data_manager, progress_bar_show );
			m_optimization_form.UpdateGraphics += new EventHandler( MainForm_UpdateGraphicsAfterOptimization );
			
#if DEF_PALETTE16_PER_CHR
			m_tiles_processor.UpdatePaletteListPos	+= new EventHandler( update_palette_list_pos );
#endif
			m_tiles_processor.NeedGFXUpdate 	+= new EventHandler( enable_update_gfx_btn_Event );
			m_screen_editor.NeedScreensUpdate	+= new EventHandler( enable_update_screens_btn_Event );
			
			TabTiles.Tag 		= new Point( TabTiles.Width,	TabTiles.Height		);
			TabScreenEditor.Tag = new Point( TabTiles.Width,	TabTiles.Height		);
			TabLayout.Tag 		= new Point( TabLayout.Width,	TabLayout.Height	);
			TabMain.Tag 		= new Point( TabLayout.Width,	TabLayout.Height	);

			FormClosing += new FormClosingEventHandler( OnFormClosing );

			// setup tooltips
			{
				SToolTipData[] tooltips = new SToolTipData[]{ 	new SToolTipData( tabControlTilesScreens, "Double Click to detach the tab page" ),
																new SToolTipData( tabControlMainLayout, "Double Click to detach the tab page" ),
																new SToolTipData( CheckBoxScreensAutoUpdate, "Automatically updates the screen list when \"Update GFX\" button is pressed" ),
																new SToolTipData( CheckBoxLayoutEditorAllBanks, "Show screens of all CHR banks" ),
																new SToolTipData( CheckBoxPickupTargetEntity, "Pickup target entity" ),
																new SToolTipData( BtnCopyCHRBank, "Add copy of active CHR bank" ),
																new SToolTipData( BtnAddCHRBank, "Add new CHR bank" ),
																new SToolTipData( BtnDeleteCHRBank, "Delete active CHR Bank" ),																
																new SToolTipData( BtnCHRBankPrevPage, "Previous CHR Bank's page" ),
																new SToolTipData( BtnCHRBankNextPage, "Next CHR Bank's page" ),																
																new SToolTipData( BtnUpdateGFX, "Update tiles\\blocks and screens ( if auto update is enabled )" ),
																new SToolTipData( BtnOptimization, "Delete unused screens\\tiles\\blocks\\CHRs" ),
																new SToolTipData( CheckBoxTileEditorLock, "Enable\\disable tile editing" ),
																new SToolTipData( BtnTilesBlocks, "Arrays of tiles and blocks to build screens" ),
																new SToolTipData( BtnPatterns, "Tiles patterns manager" ),
																new SToolTipData( Palette0, "Shift+1 / Ctrl+1,2,3,4 to select a color" ),
																new SToolTipData( Palette1, "Shift+2 / Ctrl+1,2,3,4 to select a color" ),
#if DEF_ZX
																new SToolTipData( Palette2, "Shift+3" ),
																new SToolTipData( Palette3, "Shift+4" ),
																new SToolTipData( BtnSwapInkPaper, "Swap ink to paper. Visible in \'B/W\' mode." ),
																new SToolTipData( BtnInvInk, "Invert image" ),
#else
																new SToolTipData( Palette2, "Shift+3 / Ctrl+1,2,3,4 to select a color" ),
																new SToolTipData( Palette3, "Shift+4 / Ctrl+1,2,3,4 to select a color" ),																
#endif
																new SToolTipData( CheckBoxShowMarks, "Show screen marks" ),
																new SToolTipData( CheckBoxShowEntities, "Show layout entities" ),
																new SToolTipData( CheckBoxShowTargets, "Show entity targets" ),
																new SToolTipData( CheckBoxShowCoords, "Show coordinates of a selected entity" ),
																new SToolTipData( CheckBoxPalettePerCHR, "MMC5 extended attributes mode" ),																
																new SToolTipData( CBoxPalettes, "Palettes array" ),
#if DEF_FIXED_LEN_PALETTE16_ARR
																new SToolTipData( BtnPltCopy, "Copy palette to selected slot" ),
#else
																new SToolTipData( BtnPltCopy, "Add copy of active palette" ),
#endif
																new SToolTipData( BtnPltDelete, "Delete active palette" ),																
																new SToolTipData( BtnSwapColors, "Swap two selected colors without changing graphics" ),
																new SToolTipData( BtnBlockReserveCHRs, "Make links to empty CHRs" ),
																new SToolTipData( BtnTileReserveBlocks, "Make links to empty blocks" ),
																new SToolTipData( RBtnScreenEditModeSingle, "Single screen edit mode" ),
																new SToolTipData( RBtnScreenEditModeLayout, "Layout's screen editing with moving to adjacent screens" ),
																new SToolTipData( BtnDeleteEmptyScreens, "Delete all one tile filled screens" ),																
																new SToolTipData( BtnCreateLayout, "Create an empty layout with one cell" ),
																new SToolTipData( BtnCreateLayoutWxH, "Create a layout (width x height) filled with empty screens" ),																
																new SToolTipData( BtnLayoutMoveUp, "Move selected layout up" ),
																new SToolTipData( BtnLayoutMoveDown, "Move selected layout down" ),
																new SToolTipData( CBoxBlockObjId, "Assign property to selected block or CHR" ),
																new SToolTipData( LabelObjId, "Assign property to selected block or CHR" ),
																new SToolTipData( LabelEntityProperty, "Entity properties are inherited by all instances" ),
																new SToolTipData( LabelEntityInstanceProperty, "Instance properties are unique to all instances" ),
																new SToolTipData( CheckBoxEntitySnapping, "Snap an entity to 8x8 grid" ),
															};
				SToolTipData data;
				
				for( int i = 0; i < tooltips.Length; i++ )
				{
					data = tooltips[ i ];
					
					( new ToolTip(components) ).SetToolTip( data.m_cntrl, data.m_desc );
				}			
			}
			
#if DEF_NES
			Project_openFileDialog.DefaultExt = utils.CONST_SMS_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( utils.EPlatformType.pt_NES );
			
			BtnSwapColors.Visible = false;
#elif DEF_SMS
			Project_saveFileDialog.DefaultExt = utils.CONST_SMS_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "SMS" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "sms" );

			Project_openFileDialog.DefaultExt = utils.CONST_SMS_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( utils.EPlatformType.pt_SMS );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Replace( "CA65\\NESasm", "WLA-DX" );

			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "NES", "SMS" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "nes", "sms" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "SMS CHR Bank", "SMS CHR Bank 4bpp" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "Map 2/4 bpp", "Map 2/4/8 bpp" );
			
			CheckBoxPalettePerCHR.Visible = false;
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false; 
#elif DEF_PCE
			Project_saveFileDialog.DefaultExt = utils.CONST_PCE_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "PCE" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "pce" );

			Project_openFileDialog.DefaultExt = utils.CONST_PCE_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( utils.EPlatformType.pt_PCE );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Replace( "CA65\\NESasm", "CA65\\PCEAS" );

			Import_openFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp";
			
			CheckBoxPalettePerCHR.Visible = false;
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false;
#elif DEF_ZX
			Project_saveFileDialog.DefaultExt = utils.CONST_ZX_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "ZX" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "zx" );

			Project_openFileDialog.DefaultExt = utils.CONST_ZX_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( utils.EPlatformType.pt_ZX );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Substring( Project_exportFileDialog.Filter.IndexOf( "Z" ) ).Replace( "ZX Asm", "SjASMPlus" );
			Project_exportFileDialog.DefaultExt = "zxa";

			Import_openFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp";
			
			CheckBoxPalettePerCHR.Visible	= false;
			BtnSwapColors.Visible			= false;
			
			BtnPltCopy.Enabled = BtnPltDelete.Enabled = BtnSwapColors.Enabled = false;
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false;
			
			LabelPalette12.Visible	= true;
			LabelPalette34.Visible	= true;
			BtnSwapInkPaper.Visible	= true;
			BtnInvInk.Visible		= true;
			
			ZXToolStripSeparator.Visible = ZXSwapInkPaperToolStripMenuItem.Visible = ZXInvertImageToolStripMenuItem.Visible = true;
			
			CBoxTileViewType.Items.Add( "B/W" );
			CBoxTileViewType.Items.Add( "Inv B/W" );
#endif

#if DEF_FIXED_LEN_PALETTE16_ARR
			BtnPltDelete.Enabled = false;
#endif
			if( utils.CONST_CHR_BANK_PAGES_CNT == 1 )
			{
				BtnCHRBankNextPage.Visible = BtnCHRBankPrevPage.Visible = false;
				prevPageToolStripMenuItem.Visible = nextPageToolStripMenuItem.Visible = false;
			}
			
			if( _args.Length > 0 )
			{
				project_load( _args[0] );
			}
			else
			{
				reset();
				set_status_msg( "Add a new CHR bank to begin. Press the \"Bank+\" button! <F1> - Quick Guide" );
			}
		}

		private string get_all_projects_open_file_filter( utils.EPlatformType _type )
		{
			string filter_str = "";
			string platform_file_ext;
			
			int ind;
			int i 		= ( int )_type;
			int size 	= utils.CONST_PLATFORMS_FILE_EXT_ARR.Length + i;

			for( ; i < size; i++ )
			{
				ind = i % utils.CONST_PLATFORMS_FILE_EXT_ARR.Length;
				
				platform_file_ext = utils.CONST_PLATFORMS_FILE_EXT_ARR[ ind ];
				
				filter_str += utils.CONST_FULL_APP_NAMES_ARR[ ind ] + " (*." + platform_file_ext + ")|*." + platform_file_ext;
				
				if( i != size - 1 )
				{
					filter_str += "|";
				}				
			}
			
			return filter_str;
		}

		private void progress_bar_show( bool _on, string _operation = "", bool _show_progress_bar = true )
		{
			if( _on )
			{
				m_progress_form.Left	= this.Left + ( this.Width >> 1 ) - ( m_progress_form.Width >> 1 );
				m_progress_form.Top		= this.Top + ( this.Height >> 1 ) - ( m_progress_form.Height >> 1 );

				this.Enabled = false;
				
				m_progress_form.progress_bar.Visible = _show_progress_bar;
				
				m_progress_form.operation_label.Text = _operation;

				m_progress_form.Show( this );

				progress_bar_value( 0 );
				progress_bar_status( "Please wait..." );
				
				set_status_msg( "" );
				
				Thread.Sleep( 250 );
			}
			else
			{
				Thread.Sleep( 250 );
				
				m_progress_form.progress_bar.Visible = false;
				
				m_progress_form.Hide();
				this.Enabled = true;
			}
		}
		
		private void progress_bar_value( int _val )
		{
			int val = Math.Min( _val, m_progress_form.progress_bar.Maximum );
			
			m_progress_form.progress_bar.Value = val;
			m_progress_form.progress_bar.Increment( val );
			m_progress_form.progress_bar.Refresh();
		}
		
		private void progress_bar_status( string _status )
		{
			m_progress_form.status_label.Text = _status; 
			m_progress_form.Update();
		}
		
		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			
			if( SPSeD.py_editor.is_active() )
			{
				m_py_editor.Close();
				
				if( SPSeD.py_editor.is_active() )
				{
					return;
				}
				
				m_py_editor = null;
			}
			
		    if( message_box( "All unsaved progress will be lost!\nAre you sure?", "Exit App", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
		    {
				reset();

				m_py_editor = null;
				
		    	e.Cancel = false;
		    }
		}
		
		private void set_title_name( string _msg )
		{
			Text = utils.CONST_FULL_APP_NAME + " " + utils.CONST_APP_VER + ( _msg != null ? " - " + _msg:"" );
		}
		
		public static void set_status_msg( string _msg )
		{
			if( m_status_bar_label != null )
			{
				m_status_bar_label.Text = _msg;
			}
		}
		
		public static DialogResult message_box( string _msg, string _caption, MessageBoxButtons _buttons, MessageBoxIcon _icon = MessageBoxIcon.Warning )
		{
			return MessageBox.Show( _msg, _caption, _buttons, _icon );
		}

		void DescriptionToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_description_form.ShowDialog();
		}
		
		void StatisticsToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_statistics_form.ShowStats();
		}
		
		private void enable_update_gfx_btn_Event( object sender, EventArgs e )
		{
			enable_update_gfx_btn( true );
			
			if( CheckBoxScreensAutoUpdate.Checked == false )
			{
				enable_update_screens_btn( true );
			}
		}
		
		private void enable_update_screens_btn_Event( object sender, EventArgs e )
		{
			enable_update_screens_btn( true );
		}
		
		private void enable_copy_paste_action( bool _on, ECopyPasteType _type )
		{
			if( _type == ECopyPasteType.cpt_CHR_bank || _type == ECopyPasteType.cpt_All )
			{
				pasteCHRToolStripMenuItem.Enabled = _on;
			}
			
			if( _type == ECopyPasteType.cpt_Blocks_list || _type == ECopyPasteType.cpt_All )
			{
				pasteBlockCloneToolStripMenuItem.Enabled 	= _on; 
				pasteBlockRefsToolStripMenuItem.Enabled 	= _on;
			}
			
			if( _type == ECopyPasteType.cpt_Tiles_list || _type == ECopyPasteType.cpt_All )
			{
				pasteTileToolStripMenuItem.Enabled = _on; 
			}
		}

		private void enable_main_UI( bool _on )
		{
			if( utils.CONST_CHR_BANK_PAGES_CNT > 1 )
			{
				BtnCHRBankNextPage.Enabled = BtnCHRBankPrevPage.Enabled = _on;
				prevPageToolStripMenuItem.Enabled = nextPageToolStripMenuItem.Enabled = _on;
			}
			
			PanelBlocks.Enabled = _on;
			
			if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				PanelTiles.Enabled	= _on;
			}
			
			m_tiles_palette_form.enable( _on );
			m_patterns_manager_form.enable( _on );
			
			tabControlScreensEntities.Enabled = _on;
			
			GrpBoxScreenData.Enabled = _on;
		}
		
		private void reset()
		{
			m_data_manager.reset();
			fill_entity_data( null );

			entity_instance.reset_instances_counter();
			
			CBoxTileViewType.SelectedIndex = ( int )utils.ETileViewType.tvt_Graphics;
			
			CheckBoxEntitySnapping.Checked 	= true;
			CheckBoxShowMarks.Checked		= true;
			CheckBoxShowEntities.Checked 	= true;
			CheckBoxShowTargets.Checked 	= true;
			CheckBoxShowCoords.Checked		= true;
			
			CheckBoxPalettePerCHR.Checked	= false;
#if DEF_NES || DEF_SMS
			set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
#else
			set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
#endif
			enable_main_UI( false );
			
			CheckBoxScreenShowGrid.Checked = true;
			
			TilesLockEditorToolStripMenuItem.Checked = CheckBoxTileEditorLock.Checked = true;

			m_tiles_palette_form.reset();
			m_patterns_manager_form.reset();
			
			update_graphics( false );
			
			enable_update_gfx_btn( false );
			enable_update_screens_btn( false );

			m_screen_editor.clear_active_tile_img();
			
			CBoxCHRBanks.Items.Clear();
			CBoxPalettes.Items.Clear();
			ListBoxScreens.Items.Clear();
			ListBoxLayouts.Items.Clear();
			
			CBoxBlockObjId.SelectedIndex = 0;
			CBoxBlockObjId.Tag = null;
			
			ComboBoxEntityZoom.SelectedIndex = 0;

			m_layout_editor.reset( false );
			m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type );
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_All );
			
			SelectCHRToolStripMenuItemClick_Event( null, null );
			
			PropertyPerBlockToolStripMenuItemClick_Event( null, null );
			
			RBtnScreenEditModeSingle.Checked = true;
			
			if( tabControlMainLayout.Contains( TabMain ) )
			{
				tabControlMainLayout.SelectTab( TabMain );
			}
			
			if( tabControlScreensEntities.Contains( TabScreenList ) )
			{
				tabControlScreensEntities.SelectTab( TabScreenList );
			}
			
			if( tabControlTilesScreens.Contains( TabTiles ) )
			{
				tabControlTilesScreens.SelectTab( TabTiles );
			}

			set_title_name( null );
			
			m_description_form.edit_text = "";
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_palette_copy_ind = -1;
			BtnPltCopy.Text = "Copy";
#endif
		}
		
		void ExitToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			Close();
		}
		
		void CloseToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Close Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				reset();
				
				set_status_msg( "Project closed" );
			}
		}

		void CBoxTileViewTypeChanged_Event(object sender, EventArgs e)
		{
			m_view_type = ( utils.ETileViewType )CBoxTileViewType.SelectedIndex;
#if DEF_ZX
			if( m_view_type == utils.ETileViewType.tvt_BW || m_view_type == utils.ETileViewType.tvt_Inv_BW || m_view_type == utils.ETileViewType.tvt_Graphics )
			{
				enable_update_screens_btn( true );
			}
			
			m_tiles_processor.set_view_type( m_view_type );
#endif
			update_graphics( false );
			
			m_screen_editor.clear_active_tile_img();
		}

#region load save import export
		void LoadToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				if( message_box( "Are you sure you want to close the current project?", "Load Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					Project_openFileDialog.ShowDialog();
				}
			}
			else
			{
				Project_openFileDialog.ShowDialog();
			}
		}

		void ProjectLoadOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// LOAD PROJECT...
			project_load( ( ( FileDialog )sender ).FileName );
		}

		async void project_load( string _filename )
		{
			FileStream 		fs = null;
			BinaryReader 	br = null;
			
			try
			{
				string file_ext = Path.GetExtension( _filename ).Substring( 1 );
				
				int load_scr_data_len 	= platform_data_provider.get_scr_tiles_cnt_by_file_ext( file_ext );
				int scr_data_len 		= utils.CONST_SCREEN_TILES_CNT;

				if( load_scr_data_len != scr_data_len )
				{
					if( m_data_conversion_options_form.ShowDialog() == DialogResult.Cancel )
					{
						return;
					}
				}

				progress_bar_show( true, "Project loading..." );

				reset();
				
				fs = new FileStream( _filename, FileMode.Open, FileAccess.Read );
				{
					br = new BinaryReader( fs );
					if( br.ReadUInt32() == utils.CONST_PROJECT_FILE_MAGIC )
					{
						byte ver = br.ReadByte();
						
						if( ver <= utils.CONST_PROJECT_FILE_VER )
						{
							if( ver >= 4 )
							{
								uint pre_flags = br.ReadUInt32();
								
								bool scr_data_tiles4x4 = ( ( pre_flags & utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4 ) == utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4 );
								
								set_screen_data_type( scr_data_tiles4x4 ? data_sets_manager.EScreenDataType.sdt_Tiles4x4:data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
							}
							else
							{
								// early versions always work in the Tiles4x4 mode
								set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
							}
							
							m_data_manager.tiles_data_pos = await Task.Run( () => m_data_manager.load( ver, br, file_ext, m_data_conversion_options_form.screens_align_mode, m_data_conversion_options_form.convert_colors, m_progress_val, m_progress_status ) );
							
							m_data_manager.post_load_update();
							
							uint post_flags = br.ReadUInt32();
#if DEF_NES							
							CheckBoxPalettePerCHR.Checked 	= ( ( post_flags & utils.CONST_IO_DATA_POST_FLAG_MMC5 ) == utils.CONST_IO_DATA_POST_FLAG_MMC5 );
#endif
							property_id_per_block( ( ( post_flags & utils.CONST_IO_DATA_POST_FLAG_PROP_PER_CHR ) == utils.CONST_IO_DATA_POST_FLAG_PROP_PER_CHR ? false:true ) );

							// Load description
							m_description_form.edit_text = br.ReadString();
						}
						else
						{
							throw new Exception( "Invalid file version (" + ver + ")!" );
						}
					}
					else
					{
						throw new Exception( "Invalid file!" );
					}
					
					// update controls
					{
						progress_bar_status( "Data updating..." );
						
						int tiles_cnt = m_data_manager.tiles_data_cnt;
						
						for( int i = 0; i < tiles_cnt; i++ )
						{
							CBoxCHRBanks.Items.Add( m_data_manager.get_tiles_data( i ) );
						}

						CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;
						
						enable_main_UI( true );
						
						update_layouts_list_box();
					}
					
					m_layout_editor.update();
					update_graphics( false );
					
					m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type );
#if !DEF_NES
					palette_group.Instance.active_palette = 0;
#endif
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( _filename ) );
				
				set_status_msg( "Project loaded" );
			}
			catch( Exception _err )
			{
				reset();
				
				set_status_msg( "Project loading error" );
				
				message_box( _err.Message, "Project Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			finally
			{
				if( br != null )
				{
					br.Dispose();
				}
				
				if( fs != null )
				{
					fs.Dispose();
				}
				
				progress_bar_show( false );
				
				if( m_description_form.auto_show() && m_description_form.edit_text.Length > 0 )
				{
					m_description_form.ShowDialog( this );
				}
			}
		}
		
		void SaveToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt == 0 )			
			{
				message_box( "There is no data to save!", "Project Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				Project_saveFileDialog.ShowDialog();
			}
		}
		
		void ProjectSaveOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// SAVE PROJECT...
			String filename = ( ( FileDialog )sender ).FileName;
		
			FileStream 		fs = null;
			BinaryWriter 	bw = null;
			
			try
			{
				progress_bar_show( true, "Project saving...", false );
				
				fs = new FileStream( filename, FileMode.Create, FileAccess.Write );
				{
					bw = new BinaryWriter( fs );
					bw.Write( utils.CONST_PROJECT_FILE_MAGIC );
					bw.Write( utils.CONST_PROJECT_FILE_VER );

					uint pre_flags = ( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ) ? utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4:0;
					bw.Write( pre_flags );

					m_data_manager.save( bw );
					
					uint post_flags = ( uint )( CheckBoxPalettePerCHR.Checked ? utils.CONST_IO_DATA_POST_FLAG_MMC5:0 );
					post_flags |= ( uint )( PropIdPerCHRToolStripMenuItem.Checked ? utils.CONST_IO_DATA_POST_FLAG_PROP_PER_CHR:0 );

					bw.Write( post_flags );
					
					// save description
					bw.Write( m_description_form.edit_text );
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( filename ) );
				
				set_status_msg( "Project saved" );
			}
			catch( Exception _err )
			{
				set_status_msg( "Project saving error" );
				
				message_box( _err.Message, "Project Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			finally
			{
				if( bw != null )
				{
					bw.Dispose();
				}
				
				if( fs != null )
				{
					fs.Dispose();
				}
				
				progress_bar_show( false );
			}
		}
		
		void ImportToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( CBoxCHRBanks.SelectedIndex >= 0 )
			{
				Import_openFileDialog.ShowDialog();
			}
			else
			{
				message_box( "There is no active CHR bank!", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		async void DataImportOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			String filename = ( ( FileDialog )sender ).FileName;
		
			FileStream		fs = null;
			BinaryReader 	br = null;
			
			Bitmap bmp = null;
			
			try
			{
				fs = new FileStream( filename, FileMode.Open, FileAccess.Read );
				{
					br = new BinaryReader( fs );
					
					switch( Path.GetExtension( filename ) )
					{
						case ".bmp":
							{
								bmp = new Bitmap( filename );
#if DEF_NES
								if( bmp.PixelFormat == PixelFormat.Format4bppIndexed )
#else
								if( bmp.PixelFormat == PixelFormat.Format8bppIndexed || bmp.PixelFormat == PixelFormat.Format4bppIndexed )
#endif
								{
									if( m_import_tiles_form.ShowDialog() == DialogResult.OK )
									{
										progress_bar_show( true, "Image Data Importing..." );
										
										await Task.Run( () => m_import_tiles_form.data_processing( bmp, m_data_manager, create_layout_with_empty_screens_beg, m_progress_val, m_progress_status ) );
										
										if( m_import_tiles_form.import_game_map )
										{
											progress_bar_status( "Layout init..." );
											
											// add layout to UI
											create_layout_with_empty_screens_end( m_import_tiles_form.level_layout );

											// update palettes
											if( m_import_tiles_form.apply_palette )
											{
												palette_group plt_grp = palette_group.Instance;
												
												for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
												{
													plt_grp.get_palettes_arr()[ i ].update();
												}				

												// update selected palette color
												plt_grp.active_palette = 0;
											}
											
											if( m_import_tiles_form.delete_empty_screens )
											{
												progress_bar_status( "Empty screens deletion..." );
												
												if( delete_empty_screens() > 0 )
												{
													update_screens_list_box();
													
													ListBoxScreens.SelectedIndex = m_data_manager.scr_data_pos;
												}
											}
											
											progress_bar_status( "Screens data init..." );
											
											// reset the layout mode
											RBtnScreenEditModeSingle.Checked = true;
											
											update_screens( true, false );
											
											m_layout_editor.update_dimension_changes();
										}
										else
										{
											progress_bar_status( "Data updating..." );
										}
										
										update_graphics( true );
									}
								}
								else
								{
#if DEF_NES
									throw new Exception( "The imported image must have 4bpp color depth!" );
#else
									throw new Exception( "The imported image must have indexed color depth 4/8bpp!" );
#endif
								}
								
								bmp.Dispose();
								bmp = null;
							}
							break;
#if DEF_NES || DEF_SMS
#if DEF_NES
						case ".sprednes":
#elif DEF_SMS
						case ".spredsms":
#endif
							{
								if( br.ReadUInt32() == utils.CONST_SPRED_FILE_MAGIC )
								{
									byte ver = br.ReadByte();
									
									if( ver == utils.CONST_SPRED_PROJECT_FILE_VER )
									{
										int CHR_cnt = 0;
										
										if( ( CHR_cnt = m_data_manager.merge_CHR_spred( br ) ) != -1 )
										{
											set_status_msg( string.Format( "Merged: {0} CHR banks", CHR_cnt ) );
										}
									}
									else
									{
										throw new Exception( "Invalid file version (" + ver + ")!" );
									}
								}
								else
								{
									throw new Exception( "Invalid file!" );
								}
			
								update_graphics( true );
							}
							break;

						case ".chr":
						case ".bin":
							{
								int added_CHRs = m_data_manager.merge_CHR_bin( br );
								
								if( added_CHRs > 0 )
								{
									set_status_msg( string.Format( "Merged: {0} CHRs", added_CHRs ) );
								}
								else
								if( added_CHRs < 0 )
								{
									throw new Exception( "Invalid file!" );
								}
								
								update_graphics( true );
							}
							break;
							
						case ".pal":
							{
								if( br.BaseStream.Length == 192 )
								{
									palette_group plt_grp = palette_group.Instance;
									
									plt_grp.load_main_palette( br );
									
									// update selected palette color
									plt_grp.active_palette = 0;									
									
									update_graphics( true );
									update_screens( false );
									
									m_screen_editor.clear_active_tile_img();
								}
								else
								{
									throw new Exception( "The imported palette must be 192 bytes length!" );
								}
							}
							break;
#endif //DEF_NES || DEF_SMS
					}
				}
			}
			catch( Exception _err )
			{
				if( bmp != null )
				{
					bmp.Dispose();
				}

				set_status_msg( "Data import error" );
				
				message_box( _err.Message, "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			finally
			{
				if( br != null )
				{
					br.Dispose();
				}
				
				if( fs != null )
				{
					fs.Dispose();
				}
				
				progress_bar_show( false );
			}
		}

		void ExportToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( CBoxCHRBanks.Items.Count > 0 )
			{
				Project_exportFileDialog.ShowDialog();
			}
			else
			{
				message_box( "There is no data to export!", "Project Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void export_tiles_blocks_data( int _max_data_cnt, Func< int, uint > _act_data_sum, int _data_size, ImageList _image_list, string _filename )
		{
			update_graphics( false );
			
			// get a number of non zero tiles
			int num_active_tiles = 0;
			int i;
			
			uint tile_sum;
			
			for( i = _max_data_cnt - 1; i > 0; i-- )
			{
				tile_sum = _act_data_sum( i );
				
				if( tile_sum != 0 )
				{
					num_active_tiles = i + 1;
					
					break;
				}
			}
			
			if( num_active_tiles == 0 )
			{
				throw new Exception( "There is no data to export!" );
			}
			
			// draw images into bitmap
			Bitmap bmp = new Bitmap( _data_size * num_active_tiles, _data_size );//, PixelFormat.Format24bppRgb );
			
			Graphics gfx = Graphics.FromImage( bmp );
			gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			
			for( i = 0; i < num_active_tiles; i++ )
			{
				gfx.DrawImage( _image_list.Images[ i ], i * _data_size, 0, _data_size, _data_size );
			}
			
			bmp.Save( _filename, ImageFormat.Bmp );							
			
			bmp.Dispose();
			gfx.Dispose();
		}
		
		void ProjectExportOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// EXPORT PROJECT...
			string filename = ( ( FileDialog )sender ).FileName;

			try
			{
				switch( Path.GetExtension( filename ) )
				{
					case ".bmp":
						{
							if( m_export_active_tile_block_set_form.ShowDialog() == DialogResult.OK )
							{
								if( m_export_active_tile_block_set_form.export_tiles )
								{
									export_tiles_blocks_data( utils.CONST_MAX_TILES_CNT, delegate( int _data_n ) 
									                         {
																return m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ).tiles[ _data_n ];
									                         }, 32, m_imagelist_manager.get_tiles_image_list(), filename );
								}
								else
								{
									export_tiles_blocks_data( utils.CONST_MAX_BLOCKS_CNT, delegate( int _data_n ) 
									                         {
																tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
									                         		
									                         	uint block_sum = 0;
									                         	
																for( int j = 0; j < utils.CONST_BLOCK_SIZE; j++ )
																{
																	block_sum += data.blocks[ ( _data_n << 2 ) + j ];
																}
																
																return block_sum;
									                         }, 16, m_imagelist_manager.get_blocks_image_list(), filename );
								}
							}
						}
						break;
						
					case ".png":
						{
							if( ListBoxLayouts.SelectedIndex >= 0 && ListViewScreens.Items.Count > 0 )
							{
								layout_data layout = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
								
								int layout_width	= layout.get_width();
								int layout_height	= layout.get_height();
								
								int scr_ind;
								
								// draw images into bitmap
								Bitmap bmp = new Bitmap( layout_width * utils.CONST_SCREEN_WIDTH_PIXELS, layout_height * utils.CONST_SCREEN_HEIGHT_PIXELS );

								Graphics gfx = Graphics.FromImage( bmp );
								gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
								gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
								
								for( int i = 0; i < layout_height; i++ )
								{
									for( int j = 0; j < layout_width; j++ )
									{
										scr_ind = layout.get_data( j, i ).m_scr_ind;
										
										if( scr_ind != layout_data.CONST_EMPTY_CELL_ID )
										{
											gfx.DrawImage( ListViewScreens.LargeImageList.Images[ scr_ind ], j * utils.CONST_SCREEN_WIDTH_PIXELS, i * utils.CONST_SCREEN_HEIGHT_PIXELS, utils.CONST_SCREEN_WIDTH_PIXELS, utils.CONST_SCREEN_HEIGHT_PIXELS );
										}
									}
								}
								
								bmp.Save( filename, ImageFormat.Png );
								
								bmp.Dispose();
								gfx.Dispose();
							}
							else
							{
								throw new Exception( "There is no active layout!" );
							}
						}
						break;
						
					case ".json":
						{
							using( TextWriter tw = new StreamWriter( filename ) )
							{
								m_data_manager.save_JSON( tw );
							}
						}
						break;
#if !DEF_ZX
					case ".asm":
						{
							m_exp_asm.ShowDialog( filename );
						}
						break;
#endif
					case ".zxa":
						{
							m_exp_zx_asm.ShowDialog( filename );
						}
						break;
				}
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Project Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void ExportScriptEditorToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( !SPSeD.py_editor.is_active() )
			{
				m_py_editor = new SPSeD.py_editor( global::MAPeD.Properties.Resources.MAPeD_icon, new py_api( m_data_manager ), "MAPeD API Doc", System.Text.Encoding.Default.GetString( global::MAPeD.Properties.Resources.MAPeD_Data_Export_Python_API ), "MAPeD_Data_Export_Python_API.html" );
				m_py_editor.Show();
			}
			
			SPSeD.py_editor.set_focus();
		}
		
		void AboutToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			message_box( "Game Maps Editor (" + utils.CONST_PLATFORM + ")\n" + utils.CONST_PLATFORM_DESC + "\n\n" + utils.CONST_APP_VER + " " + utils.build_str + " pfv" + utils.CONST_PROJECT_FILE_VER + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		void MenuHelpQuickGuideClick_Event(object sender, EventArgs e)
		{
			string doc_path = Application.StartupPath.Substring( 0, Application.StartupPath.LastIndexOf( Path.DirectorySeparatorChar ) ) + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + "MAPeD" + Path.DirectorySeparatorChar + "Quick_Guide.html";
			
			//message_box( doc_path, "path", MessageBoxButtons.OK, MessageBoxIcon.Information );//!!!
			
			if( utils.is_win )
			{
				System.Diagnostics.Process.Start( doc_path );
			}
			else
			if( utils.is_linux )
			{
				System.Diagnostics.Process.Start( "xdg-open", doc_path );
			}
			else
			if( utils.is_macos )
			{
				// need to test it...
				System.Diagnostics.Process.Start( "open", doc_path );
			}
		}
		
		public void KeyUp_Event(object sender, KeyEventArgs e)
		{
			m_tiles_processor.key_event( sender, e );
		}

		public void KeyDown_Event(object sender, KeyEventArgs e)
		{
		}
		
		void TabCntrlDblClick_Event(object sender, EventArgs e)
		{
			TabControl tab_cntrl = sender as TabControl;
			
			( new tab_page_container( tab_cntrl, this ) ).Show();
		}
		/*
		void detachTabToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			TabControl tab_cntrl = ( (sender as ToolStripMenuItem ).Owner as ContextMenuStrip ).SourceControl.Parent as TabControl;

			if( !( tab_cntrl.SelectedTab.Parent.Parent is tab_page_container ) )
			{
				( new tab_page_container( tab_cntrl, this ) ).Show();
			}
		}
		*/
#endregion		
// TILES EDITOR **************************************************************************************//
#region tiles editor
		private void enable_update_gfx_btn( bool _on )
		{
			BtnUpdateGFX.Enabled = _on;

			BtnUpdateGFX.UseVisualStyleBackColor = !_on;
		}
		
		void CHRBankChanged_Event(object sender, EventArgs e)
		{
			ComboBox chr_bank_cbox = sender as ComboBox;
			
			// force update of screens if needed
			update_screens_if_needed();

			// reset the layout mode
			RBtnScreenEditModeSingle.Checked = true;
			
			m_data_manager.scr_data_pos 	= -1;
			m_data_manager.tiles_data_pos 	= chr_bank_cbox.SelectedIndex;
			
			palette_group.Instance.active_palette = 0;
			
			// update palettes
			{
				update_palettes_arr( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ), false );
				
				CBoxPalettes.SelectedIndex = 0;
			}
			
			update_graphics( false );
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_All );
			
			// reset the screen editor controls
			{
				m_screen_editor.clear_active_tile_img();
				
				update_screens_list_box();
				
				ListBoxScreens.SelectedIndex = -1;
			}
			
			update_screens_by_bank_id( true, false );
			
			m_patterns_manager_form.set_data( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
		}

		void BtnUpdateGFXClick_Event(object sender, EventArgs e)
		{
			update_graphics( false );
			
			m_screen_editor.clear_active_tile_img();
			
			set_status_msg( "GFX updated" );
		}
		
		private void update_graphics( bool _update_tile_processor_gfx )
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			
			m_imagelist_manager.update_blocks( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );
			m_imagelist_manager.update_tiles( m_view_type, data, m_data_manager.screen_data_type );	// called after update_blocks, because it uses updated gfx of blocks to speed up drawing
			
			m_tile_list_manager.update_all();

			m_screen_editor.update();

			if( CheckBoxScreensAutoUpdate.Checked )
			{
				update_screens( false );
			}
			
			if( _update_tile_processor_gfx )
			{
				m_tiles_processor.update_graphics();
			}
			
			m_tiles_palette_form.update();
			m_patterns_manager_form.update();
			
			enable_update_gfx_btn( false );
//			enable_update_screens_btn( false );			
		}
		
		void BtnCHRBankNextPageClick_Event(object sender, EventArgs e)
		{
			if( utils.CONST_CHR_BANK_PAGES_CNT > 1 )
			{
				m_tiles_processor.CHR_bank_next_page();
			}
		}
		
		void BtnCHRBankPrevPageClick_Event(object sender, EventArgs e)
		{
			if( utils.CONST_CHR_BANK_PAGES_CNT > 1 )
			{
				m_tiles_processor.CHR_bank_prev_page();
			}
		}
		
		void BtnCopyCHRBankClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				if( m_data_manager.tiles_data_copy() == true )
				{
					update_screens_if_needed();
					
					// copy screens of a current CHR bank 
					// to the end of the screen images list
					// before CHR bank switching
					m_imagelist_manager.copy_screens_to_the_end( m_data_manager.get_tiles_data(), m_data_manager.tiles_data_pos );
					
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_cnt - 1 );
					
					CBoxCHRBanks.Items.Add( data );
					CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_cnt - 1;
					
					palette_group.Instance.active_palette = 0;
					
					enable_copy_paste_action( false, ECopyPasteType.cpt_All );
					
					if( ScreensShowAllBanksToolStripMenuItem.Checked )
					{
						update_screens_by_bank_id( true, false );
					}
					
					set_status_msg( "CHR bank copied" );
				}
				else
				{
					set_status_msg( "Failed to copy CHR bank" );
					
					message_box( "Can't copy CHR bank!\nThe maximum allowed number of CHR banks - " + utils.CONST_CHR_BANK_MAX_CNT, "Failed to Copy CHR Bank", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnAddCHRBankClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_create() )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_cnt - 1 );
				
				CBoxCHRBanks.Items.Add( data );
				CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_cnt - 1;
	
				palette_group.Instance.active_palette = 0;
	
				enable_main_UI( true );
				
				enable_copy_paste_action( false, ECopyPasteType.cpt_All );
				
				set_status_msg( "Added CHR bank" );
			}
			else
			{
				set_status_msg( "Failed to create CHR bank" );
				
				message_box( "Can't create CHR bank!\nThe maximum allowed number of CHR banks - " + utils.CONST_CHR_BANK_MAX_CNT, "Failed to Create CHR Bank", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void BtnDeleteCHRBankClick_Event(object sender, EventArgs e)
		{
			if( CBoxCHRBanks.Items.Count > 0 && message_box( "Are you sure?", "Remove CHR Bank", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_cnt == 1 )
				{
					reset();

					set_status_msg( "No data" );
				}
				else
				{
					ListViewScreens.BeginUpdate();
					{
						bool res = false;

						int screens_cnt = m_data_manager.scr_data_cnt;
						
						for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
						{
							m_data_manager.remove_screen_from_layouts( CBoxCHRBanks.SelectedIndex, 0 );
							
							res |= m_imagelist_manager.remove_screen( CBoxCHRBanks.SelectedIndex, screen_n );
						}

						m_data_manager.tiles_data_destroy();
						
						if( res )
						{
							update_screens_labels_by_bank_id();
							
							m_layout_editor.set_active_screen( -1 );
						}

						m_layout_editor.update_dimension_changes();
					}
					ListViewScreens.EndUpdate();

					// update CHR bank labels
					int size;
					{
						CBoxCHRBanks.Items.Clear();

						size = m_data_manager.tiles_data_cnt;

						for( int i = 0; i < size; i++ )
						{
							CBoxCHRBanks.Items.Add( m_data_manager.get_tiles_data( i ) );
						}
					}

					CBoxPalettes.Items.Clear();
					CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;

					enable_copy_paste_action( false, ECopyPasteType.cpt_All );

					palette_group.Instance.active_palette = 0;
					
					set_status_msg( "CHR bank removed" );
				}
			}
		}
		
		void PanelBlocksClick_Event(object sender, EventArgs e)
		{
			select_block( get_sender_index( sender ), true, true );
		}
		
		void select_block( int _id, bool _send_event, bool _update_active_tile )
		{
			if( _send_event )
			{
				m_tiles_processor.block_select_event( _id, m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
			}

			// update an object index
			CBoxBlockObjId.Tag = int.MaxValue;	// don't enable 'Update GFX' button
			CBoxBlockObjId.SelectedIndex = m_tiles_processor.get_block_flags_obj_id();
			CBoxBlockObjId.Tag = null;
			
			if( _update_active_tile )
			{
				update_active_block_img( _id );
			}
		}
		
		void PanelTilesClick_Event(object sender, EventArgs e)
		{
			select_tile( get_sender_index( sender ) );
		}
		
		void select_tile( int _id )
		{
			if( _id >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
				
				m_tiles_processor.tile_select_event( _id, data );
				
				update_active_tile_img( _id );
			}
		}

		private void update_active_tile_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				m_screen_editor.set_active_tile( _ind, screen_editor.EFillMode.efm_Tile );
			}
		}

		private void update_active_block_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				m_screen_editor.set_active_tile( _ind, screen_editor.EFillMode.efm_Block );
			}
		}
		
		void CBoxBlockObjIdChanged_Event(object sender, EventArgs e)
		{
			m_tiles_processor.set_block_flags_obj_id( CBoxBlockObjId.SelectedIndex, PropertyPerBlockToolStripMenuItem.Checked );
			
			if( CBoxBlockObjId.Tag == null && m_view_type == utils.ETileViewType.tvt_ObjectId )
			{
				enable_update_gfx_btn( true );
			}
		}
		
		void BtnBlockVFlipClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.transform_block( utils.ETransformType.tt_vflip );
		}
		
		void BtnBlockHFlipClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.transform_block( utils.ETransformType.tt_hflip );
		}
		
		void BtnBlockRotateClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.transform_block( utils.ETransformType.tt_rotate );
		}
		
		void BtnCHRVFlipClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.transform_selected_CHR( utils.ETransformType.tt_vflip );
		}
		
		void BtnCHRHFlipClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.transform_selected_CHR( utils.ETransformType.tt_hflip );
		}
		
		void BtnCHRRotateClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.transform_selected_CHR( utils.ETransformType.tt_rotate );
		}

		void SelectCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			block_editor_draw_mode( false );
		}
		
		void PropertyPerBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			property_id_per_block( true );
			
			set_status_msg( "Property Id per BLOCK: on" );
		}
		
		void PropertyPerCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			property_id_per_block( false );
			
			set_status_msg( "Property Id per CHR: on" );
		}
		
		void property_id_per_block( bool _on )
		{
			PropIdPerBlockToolStripMenuItem.Checked = PropertyPerBlockToolStripMenuItem.Checked = _on;
			PropIdPerCHRToolStripMenuItem.Checked	= PropertyPerCHRToolStripMenuItem.Checked	= !_on;
		
			if( m_view_type == utils.ETileViewType.tvt_ObjectId )
			{
				update_graphics( false );
			}
			
			select_block( m_tiles_processor.get_selected_block(), true, false );
		}
		
		void DrawToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			block_editor_draw_mode( true );
		}

		void block_editor_draw_mode( bool _on )
		{
			CHRSelectToolStripMenuItem.Checked	= !_on;
			DrawToolStripMenuItem.Checked 		= _on;
			
			BtnEditModeDraw.Enabled 		= !_on;
			BtnEditModeSelectCHR.Enabled 	= _on;
			
			BlockEditorModeDrawToolStripMenuItem.Checked	= _on;
			BlockEditorModeSelectToolStripMenuItem.Checked	= !_on;			
			
			m_tiles_processor.set_block_editor_mode( _on ?  block_editor.EMode.bem_draw:block_editor.EMode.bem_CHR_select );
		}
		
		void TilesLockEditorToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			TilesLockEditorToolStripMenuItem.Checked = CheckBoxTileEditorLock.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void CheckBoxTileEditorLockedChecked_Event(object sender, EventArgs e)
		{
			bool checked_state = ( sender as CheckBox ).Checked;
			
			TilesLockEditorToolStripMenuItem.Checked = checked_state;
			
			m_tiles_processor.tile_editor_locked( checked_state );
			
			BtnTileReserveBlocks.Enabled = !checked_state;
		}

		void CopyCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_tiles_processor.CHR_bank_copy_spr() )
			{
				enable_copy_paste_action( true, ECopyPasteType.cpt_CHR_bank );
			}
		}
		
		void PasteCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.CHR_bank_paste_spr();
			
//!!!		enable_copy_paste_action( false, ECopyPasteType.cpt_CHR_bank );
		}
		
		void FillWithColorCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_tiles_processor.CHR_bank_fill_with_color_spr() == false )
			{
				message_box( "Please, select an active palette", "Fill With Color", MessageBoxButtons.OK );
			}
		}

		void InsertLeftCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

				int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
				
				if( sel_ind >= 0 )
				{
					for( int i = utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1; i > sel_ind; i-- )
					{
						data.from_CHR_bank_to_spr8x8( i - 1, utils.tmp_spr8x8_buff );
						data.from_spr8x8_to_CHR_bank( i, utils.tmp_spr8x8_buff );
					}
					
					Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
					data.from_spr8x8_to_CHR_bank( sel_ind, utils.tmp_spr8x8_buff );
					
					data.inc_blocks_CHRs( sel_ind );
					
					enable_update_screens_btn( true );
					update_graphics( true );
				}
				else
				{
					message_box( "Please, select a CHR!", "Insert CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void DeleteCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

				int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
				
				if( sel_ind >= 0 )
				{
					if( message_box( "Are you sure?", "Delete CHR", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						for( int i = sel_ind; i < utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1; i++ )
						{
							data.from_CHR_bank_to_spr8x8( i + 1, utils.tmp_spr8x8_buff );
							data.from_spr8x8_to_CHR_bank( i, utils.tmp_spr8x8_buff );
						}
						
						Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
						data.from_spr8x8_to_CHR_bank( utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1, utils.tmp_spr8x8_buff );
						
						data.dec_blocks_CHRs( sel_ind );
						
						enable_update_screens_btn( true );
						update_graphics( true );
					}
				}
				else
				{
					message_box( "Please, select a CHR!", "Delete CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		int get_context_menu_sender_index( object sender )
		{
			Control cntrl = ( ( sender as ToolStripDropDownItem ).Owner as ContextMenuStrip ).SourceControl;
			
			return ( cntrl.Tag as tile_list ).selected_tile_ind();
		}

		int get_sender_index( object sender )
		{
			return ( sender as tile_list ).selected_tile_ind();
		}
		
		void CopyBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( ( m_block_copy_item_ind = get_context_menu_sender_index( sender ) ) >= 0 )
			{
				enable_copy_paste_action( true, ECopyPasteType.cpt_Blocks_list );
			}
		}
		
		void PasteBlockCloneToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			paste_block( true, get_context_menu_sender_index( sender ) );
		}
		
		void PasteBlockRefsToolStripMenuItemClickEvent(object sender, EventArgs e)
		{
			paste_block( false, get_context_menu_sender_index( sender ) );
		}

		void paste_block( bool _paste_clone, int _sel_ind )
		{
			if( _sel_ind >= 0 && m_block_copy_item_ind >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
				
				uint 	block_val;
				int 	chr_ind;
				int 	free_chr_ind 	= -1;
				
				if( _paste_clone )
				{
					free_chr_ind = data.get_first_free_spr8x8_id();
	
					if( free_chr_ind + utils.CONST_BLOCK_SIZE > utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
					{
						message_box( "There is no free space in the CHR bank!", "Paste Block", MessageBoxButtons.OK );
						return;
					}
				}
				
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					block_val = data.blocks[ ( m_block_copy_item_ind << 2 ) + i ];
					
					if( _paste_clone )
					{
						chr_ind = tiles_data.get_block_CHR_id( block_val );
						
						data.from_CHR_bank_to_spr8x8( chr_ind, utils.tmp_spr8x8_buff );
						data.from_spr8x8_to_CHR_bank( free_chr_ind + i, utils.tmp_spr8x8_buff );
						
						block_val = tiles_data.set_block_CHR_id( free_chr_ind + i, block_val );
					}
					data.blocks[ ( _sel_ind << 2 ) + i ] = block_val;
				}

				if( _paste_clone )
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} cloned to block #{1:X2}", m_block_copy_item_ind, _sel_ind ) );
				}
				else
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} references are copied to block #{1:X2}", m_block_copy_item_ind, _sel_ind ) );
				}

				enable_update_screens_btn( true );
				
//				update_graphics( true );
				
				// optimized update_graphics
				{
					m_tile_list_manager.copy_tile( tile_list.EType.t_Blocks, m_block_copy_item_ind, _sel_ind );
					
					if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						m_imagelist_manager.update_tiles( m_view_type, data, m_data_manager.screen_data_type );
						m_tile_list_manager.update_tiles( tile_list.EType.t_Tiles );
					}
					
					update_active_block_img( _sel_ind );
					m_screen_editor.update();
					m_patterns_manager_form.update();
					m_tiles_processor.update_graphics();
					
					if( CheckBoxScreensAutoUpdate.Checked )
					{
						update_screens( false );
					}
				}
			}
		}

		void ClearCHRsBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the block's CHRs will be cleared!", "Clear Selected Block CHRs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				clear_block( true, get_context_menu_sender_index( sender ) );
			}
		}
		
		void ClearRefsBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the block's CHR indices will be set to zero!", "Clear Selected Block Refs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				clear_block( false, get_context_menu_sender_index( sender ) );
			}
		}
		
		private void clear_block( bool _clear_CHRs, int _sel_ind )
		{
			if( _sel_ind >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
				
				uint 	block_val;
				int 	chr_ind;
				
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					block_val = data.blocks[ ( _sel_ind << 2 ) + i ];
					{
						chr_ind = tiles_data.get_block_CHR_id( block_val );
						
						if( _clear_CHRs )
						{
							data.fill_CHR_bank_spr8x8_by_color_ind( chr_ind, 0 );
						}
						
						block_val = tiles_data.set_block_CHR_id( 0, block_val );
					}
					data.blocks[ ( _sel_ind << 2 ) + i ] = block_val;
				}

				if( _clear_CHRs )
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} is cleared", _sel_ind ) );
				}
				else
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} references are cleared", _sel_ind ) );
				}
				
				m_screen_editor.clear_active_tile_img();
				
				enable_update_gfx_btn( true );
				enable_update_screens_btn( true );
				
				update_graphics( true );
			}
		}
		
		void InsertLeftBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					int block_ind = sel_ind * utils.CONST_BLOCK_SIZE;
					
					Array.Copy( data.blocks, block_ind, data.blocks, block_ind + utils.CONST_BLOCK_SIZE, data.blocks.Length - block_ind - utils.CONST_BLOCK_SIZE );
					
					data.clear_block( sel_ind );
					
					if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						data.inc_tiles_blocks( ( byte )sel_ind );
					}
					else
					{
						data.inc_screen_blocks( ( byte )sel_ind );
						data.inc_patterns_tiles( ( byte )sel_ind );
					}
					
					m_screen_editor.clear_active_tile_img();
					
					enable_update_screens_btn( true );
					update_graphics( true );
				}
			}
		}
		
		void DeleteBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Delete Block", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
	
					int sel_ind = get_context_menu_sender_index( sender );
					
					if( sel_ind >= 0 )
					{
						int block_ind = sel_ind * utils.CONST_BLOCK_SIZE;
						
						Array.Copy( data.blocks, block_ind + utils.CONST_BLOCK_SIZE, data.blocks, block_ind, data.blocks.Length - block_ind - utils.CONST_BLOCK_SIZE );
						
						if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
						{
							data.dec_tiles_blocks( ( byte )sel_ind );
						}
						else
						{
							data.dec_screen_blocks( ( byte )sel_ind );
							data.dec_patterns_tiles( ( byte )sel_ind );
						}
						data.clear_block( data.tiles.Length - 1 );
						
						m_screen_editor.clear_active_tile_img();
						
						enable_update_screens_btn( true );
						update_graphics( true );
					}
				}
			}
		}
		
		void CopyTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( ( m_tile_copy_item_ind = get_context_menu_sender_index( sender ) ) >= 0 )
			{
				enable_copy_paste_action( true, ECopyPasteType.cpt_Tiles_list );
			}
		}
		
		void PasteTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			int sel_ind = get_context_menu_sender_index( sender );
			
			if( sel_ind >= 0 && m_tile_copy_item_ind >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
				
				for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
				{
					data.set_tile_block( sel_ind, i, data.get_tile_block( m_tile_copy_item_ind, i ) );
				}

				set_status_msg( String.Format( "Tile List: tile #{0:X2} is copied to #{1:X2}", m_tile_copy_item_ind, sel_ind ) );
				
				enable_update_screens_btn( true );
				
//				update_graphics( true );
				
				// optimized update_graphics
				{
					m_tile_list_manager.copy_tile( tile_list.EType.t_Tiles, m_tile_copy_item_ind, sel_ind );
					
					update_active_tile_img( sel_ind );
					m_screen_editor.update();
					m_patterns_manager_form.update();
					m_tiles_processor.update_graphics();
					
					if( CheckBoxScreensAutoUpdate.Checked )
					{
						update_screens( false );
					}
				}
			}
		}
		
		void ClearTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the tile's blocks references will be cleared!", "Clear Selected Tile Refs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

					data.clear_tile( sel_ind );
					
					m_screen_editor.clear_active_tile_img();
	
					set_status_msg( String.Format( "Tile List: tile #{0:X2} references are cleared", sel_ind ) );
					
					enable_update_screens_btn( true );
					update_graphics( true );
				}
			}
		}
		
		void ClearAllTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the blocks references for all the tiles will be set to zero!", "Clear All Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
				
				data.clear_tiles();
				
				m_screen_editor.clear_active_tile_img();
				
				set_status_msg( String.Format( "Tile List: all the tile references are cleared" ) );
				
				enable_update_screens_btn( true );
				update_graphics( true );
			}
		}

		void InsertLeftTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					Array.Copy( data.tiles, sel_ind, data.tiles, sel_ind + 1, data.tiles.Length - sel_ind - 1 );
					
					data.clear_tile( sel_ind );
					data.inc_screen_tiles( ( byte )sel_ind );
					data.inc_patterns_tiles( ( byte )sel_ind );
					
					m_screen_editor.clear_active_tile_img();
					
					enable_update_screens_btn( true );
					update_graphics( true );
				}
			}
		}
		
		void DeleteTileToolStripMenuItem3Click_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Delete Tile", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
	
					int sel_ind = get_context_menu_sender_index( sender );
					
					if( sel_ind >= 0 )
					{
						Array.Copy( data.tiles, sel_ind + 1, data.tiles, sel_ind, data.tiles.Length - sel_ind - 1 );
						
						data.dec_screen_tiles( ( byte )sel_ind );
						data.dec_patterns_tiles( ( byte )sel_ind );
						data.clear_tile( data.tiles.Length - 1 );
						
						m_screen_editor.clear_active_tile_img();
						
						enable_update_screens_btn( true );
						update_graphics( true );
					}
				}
			}
		}

		void BtnOptimizationClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt == 0 )			
			{
				message_box( "There is no data!", "Data Optimization", MessageBoxButtons.OK );
			}
			else
			{
				ListViewScreens.BeginUpdate();
				{
					m_optimization_form.ShowDialog();
					
					if( m_optimization_form.need_update_screen_list )
					{
						progress_bar_show( true, "Updating screens...", false );
						
						// update screens images
						m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, -1 );
						
						if( !CheckBoxLayoutEditorAllBanks.Checked )
						{
							m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, CBoxCHRBanks.SelectedIndex );
						}

						enable_update_screens_btn( false );
						
						m_layout_editor.set_active_screen( -1 );
						m_layout_editor.update();
						
						progress_bar_show( false );
					}
					
					// need to be reset to avoid incorrect tiles array filling
					// when drawing by blocks 2x2 when the 'Tiles (4x4)' mode is active
					m_screen_editor.clear_active_tile_img();
					
					m_screen_editor.update_adjacent_screens();
				}
				ListViewScreens.EndUpdate();
			}
		}
		
		void ShiftColorsToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_tiles_processor.block_shift_colors( shiftTransparencyToolStripMenuItem.Checked );
		}

		void BtnBlockReserveCHRsClick_Event(object sender, EventArgs e)
		{
			if( m_tiles_processor.block_reserve_CHRs( m_tiles_processor.get_selected_block(), m_data_manager ) > 0 )
			{
				enable_update_gfx_btn( true );
				enable_update_screens_btn( true );
			}
		}
		
		void BtnTileReserveBlocksClick_Event(object sender, EventArgs e)
		{
			int block_ind;
			
			if( ( block_ind = m_tiles_processor.tile_reserve_blocks( m_data_manager ) ) >= 0 )
			{
				enable_update_gfx_btn( true );
				enable_update_screens_btn( true );
			}
		}
#endregion		
// SCREEN EDITOR *************************************************************************************//
#region screen editor
		void delete_screen( int _scr_local_ind )
		{
			m_data_manager.scr_data_pos = _scr_local_ind ;
			m_data_manager.screen_data_delete();

			m_data_manager.remove_screen_from_layouts( CBoxCHRBanks.SelectedIndex, _scr_local_ind  );
			
			if( m_imagelist_manager.remove_screen( CBoxCHRBanks.SelectedIndex, _scr_local_ind ) )
			{
				m_layout_editor.set_active_screen( -1 );
				
				update_screens_labels_by_bank_id();
			}
		}

#if DEF_SCREEN_HEIGHT_7d5_TILES
		bool check_empty_screen( uint[] _tiles, screen_data _scr_data )
#else
		bool check_empty_screen( screen_data _scr_data )
#endif
		{
			int tile_n;
#if DEF_SCREEN_HEIGHT_7d5_TILES
			if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				uint tile_ind;
				
				int scr_first_tile_ind	= _scr_data.get_tile( 0 );
				
				for( tile_n = 1; tile_n < utils.CONST_SCREEN_TILES_CNT - utils.CONST_SCREEN_NUM_WIDTH_TILES; tile_n++ )
				{
					if( scr_first_tile_ind != _scr_data.get_tile( tile_n ) )
					{
						break;
					}
				}
				
				if( tile_n != utils.CONST_SCREEN_TILES_CNT - utils.CONST_SCREEN_NUM_WIDTH_TILES )
				{
					return false;
				}
	
				// check the last upper half of the tiles line
				int scr_block_ind	= utils.get_byte_from_uint( _tiles[ _scr_data.get_tile( 0 ) ], 0 );
	
				for( tile_n = utils.CONST_SCREEN_TILES_CNT - utils.CONST_SCREEN_NUM_WIDTH_TILES; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
				{
					tile_ind = _tiles[ _scr_data.get_tile( tile_n ) ];
					
					if( ( scr_block_ind != utils.get_byte_from_uint( tile_ind, 0 ) ||
					    ( scr_block_ind != utils.get_byte_from_uint( tile_ind, 1 ) ) ) )
					{
						break;
					}
				}
				
				if( tile_n == utils.CONST_SCREEN_TILES_CNT )
				{
					return true;
				}
			}
			else
#endif
			{
				int scr_first_tile_ind = _scr_data.get_tile( 0 );
				int num_tiles = utils.get_screen_tiles_cnt_uni( m_data_manager.screen_data_type );
				
				for( tile_n = 1; tile_n < num_tiles; tile_n++ )
				{
					if( scr_first_tile_ind != _scr_data.get_tile( tile_n ) )
					{
						break;
					}
				}
				
				if( tile_n == num_tiles )
				{
					return true;
				}
			}

			return false;
		}
		
		int delete_empty_screens()
		{
			int res = 0;

			tiles_data data	= m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

			int scr_n;
			
			for( scr_n = 0; scr_n < m_data_manager.scr_data_cnt; scr_n++ )
			{
#if DEF_SCREEN_HEIGHT_7d5_TILES
				if( check_empty_screen( data.tiles, data.get_screen_data( scr_n ) ) )
#else
					if( check_empty_screen( data.get_screen_data( scr_n ) ) )
#endif
				{
					delete_screen( scr_n );

					--scr_n;
					
					++res;
				}
			}
			
			return res;
		}

		private void update_screens_list_box()
		{
			ListBoxScreens.Items.Clear();
			
			int size = m_data_manager.scr_data_cnt;
			
			for( int i = 0; i < size; i++ )
			{
				ListBoxScreens.Items.Add( i );
			}
		}

		void BtnCreateScreenClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.screen_data_create() == true )
			{
				if( insert_screen_into_layouts( m_data_manager.scr_data_cnt - 1 ) >= 0 )
				{
					ListBoxScreens.Items.Add( m_data_manager.scr_data_cnt - 1 );
					m_data_manager.scr_data_pos = ListBoxScreens.SelectedIndex = m_data_manager.scr_data_cnt - 1;
					
					enable_update_screens_btn( true );
					
					set_status_msg( "Added screen" );
				}
				else
				{
					m_data_manager.screen_data_delete();
					
					set_status_msg( "Failed to create screen" );
					
					message_box( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT, "Failed to Create Screen", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}
		}

		void BtnCopyScreenClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.screen_data_copy() == true )
			{
				if( insert_screen_into_layouts( m_data_manager.scr_data_cnt - 1 ) >= 0 )
				{
					ListBoxScreens.Items.Add( m_data_manager.scr_data_cnt - 1 );
					m_data_manager.scr_data_pos = ListBoxScreens.SelectedIndex = m_data_manager.scr_data_cnt - 1;
					
					enable_update_screens_btn( true );
					
					set_status_msg( "Screen copied" );
				}
				else
				{
					m_data_manager.screen_data_delete();
					
					set_status_msg( "Failed to copy screen" );
					
					message_box( "Can't copy the screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT, "Failed to Copy Screen", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}
		}
		
		void BtnDeleteScreenClick_Event(object sender, EventArgs e)
		{
			if( ListBoxScreens.SelectedIndex >= 0 && ListBoxScreens.Items.Count > 0 && message_box( "Are you sure?", "Delete Screen", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.screen_data_delete();

				m_data_manager.remove_screen_from_layouts( CBoxCHRBanks.SelectedIndex, ListBoxScreens.SelectedIndex );
				
				if( m_imagelist_manager.remove_screen( CBoxCHRBanks.SelectedIndex, ListBoxScreens.SelectedIndex ) )
				{
					m_layout_editor.update_dimension_changes();

					m_layout_editor.set_active_screen( -1 );
					
					update_screens_labels_by_bank_id();
					
					// reset an entity editor data
					m_layout_editor.reset_entity_instance();
					fill_entity_data( null );
					CheckBoxPickupTargetEntity.Checked = false;
				}
				
				update_screens_list_box();

				ListBoxScreens.SelectedIndex = m_data_manager.scr_data_pos;
				
				if( m_data_manager.scr_data_cnt == 0 )
				{
					update_graphics( false );
				}

				enable_update_screens_btn( true );
				
				set_status_msg( "Screen deleted" );
			}
		}
		
		void BtnDeleteEmptyScreensClick_Event(object sender, EventArgs e)
		{
			if( ListBoxScreens.Items.Count > 0 && message_box( "Delete all one tile filled screens?", "Clean Up", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				int deleted_screens_cnt;
				
				progress_bar_show( true, "Empty screens deletion...", false );
				
				if( ( deleted_screens_cnt = delete_empty_screens() ) > 0 )
				{
					m_layout_editor.update_dimension_changes();
					
					update_screens_list_box();

					if( m_data_manager.scr_data_cnt == 0 )
					{
						update_graphics( false );
					}
					
					enable_update_screens_btn( true );

					ListBoxScreens.SelectedIndex = m_data_manager.scr_data_pos;
					
					set_status_msg( "Clean up: " + deleted_screens_cnt + " screens deleted" );
				}
				else
				{
					set_status_msg( "Clean up: no empty screens found" );
				}
				
				progress_bar_show( false );
			}
		}
		
		void ListBoxScreensClick_Event(object sender, EventArgs e)
		{
			m_data_manager.scr_data_pos = ( sender as ListBox ).SelectedIndex;
		}
		
		void ScreenEditShowGridToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			ScreenEditShowGridToolStripMenuItem.Checked = CheckBoxScreenShowGrid.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void CheckBoxScreenShowGridChecked_Event(object sender, EventArgs e)
		{
			m_screen_editor.draw_grid_flag = ScreenEditShowGridToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
		}
		
		void BtnPatternsClick_Event(object sender, EventArgs e)
		{
			m_patterns_manager_form.visible( true );
			m_patterns_manager_form.update();
			
			BtnPatterns.Enabled = false;
		}

		void MainForm_PatternsManagerClosed(object sender, EventArgs e)
		{
			BtnPatterns.Enabled = true;
		}
		
		void BtnTilesBlocksClick_Event(object sender, EventArgs e)
		{
			m_tiles_palette_form.Visible = true;
			
			BtnTilesBlocks.Enabled = false;
		}
		
		void MainForm_TilesBlocksClosed(object sender, EventArgs e)
		{
			BtnTilesBlocks.Enabled = true;
		}
		
		void MainForm_TileSelected(object sender, EventArgs e)
		{
			select_tile( ( sender as tiles_palette_form ).active_item_id );
		}

		void MainForm_BlockSelected(object sender, EventArgs e)
		{
			select_block( ( sender as tiles_palette_form ).active_item_id, true, true );
		}
		
		void MainForm_BlockQuadSelected(object sender, EventArgs e)
		{
			int sel_block_ind = m_tiles_processor.get_selected_block();
			
			if( PropertyPerCHRToolStripMenuItem.Checked && BlockEditorModeSelectToolStripMenuItem.Checked )
			{
				// show property
				select_block( sel_block_ind, false, false );
			}
		}
		
		private void update_tile_image( object sender, EventArgs e )
		{
			NewTileEventArg event_args = e as NewTileEventArg;
			
			int tile_ind 	= event_args.tile_ind;
			tiles_data data = event_args.data;
			
			m_imagelist_manager.update_tile( tile_ind, m_view_type, data, null, null, m_data_manager.screen_data_type );
			m_tile_list_manager.update_tile( tile_list.EType.t_Tiles, tile_ind );
		}
		
		void MainForm_UpdateGraphicsAfterOptimization(object sender, EventArgs e)
		{
			int last_scr_cnt = ListBoxScreens.Items.Count;
			
			update_screens_list_box();				
			
			if( ListBoxScreens.Items.Count != last_scr_cnt )
			{
				ListBoxScreens.SelectedIndices.Clear();
				m_data_manager.scr_data_pos = -1;
			}
			
			update_graphics( true );
			
			if( CheckBoxScreensAutoUpdate.Checked == false )
			{
				enable_update_screens_btn( true );
			}
		}
		
		void BtnResetTileClick_Event(object sender, EventArgs e)
		{
			m_screen_editor.clear_active_tile_img();
		}
		
		void BtnSwapInkPaperClick_Event(object sender, EventArgs e)
		{
#if DEF_ZX
			m_tiles_processor.zx_swap_ink_paper( true );
#endif
		}
		
		void BtnInvInkClick_Event(object sender, EventArgs e)
		{
#if DEF_ZX
			m_tiles_processor.zx_swap_ink_paper( false );
#endif		
		}
#endregion		
// LAYOUT EDITOR *************************************************************************************//		
#region layout editor
		layout_data create_layout_with_empty_screens_beg( int _scr_width, int _scr_height )
		{
			if( m_data_manager.layout_data_create() == true )
			{
				layout_data layout = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				// create a level layout
				{
					for( int i = 0; i < _scr_width - 1; i++ )
					{
						layout.add_right();
					}
					
					for( int i = 0; i < _scr_height - 1; i++ )
					{
						layout.add_down();
					}
				}
				
				// create screens and fill the layout
				{
					int scr_global_ind;
					layout_screen_data scr_data;
					
					for( int y = 0; y < _scr_height; y++ )
					{
						for( int x = 0; x < _scr_width; x++ )
						{
							m_data_manager.screen_data_create();
							
							scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, m_data_manager.scr_data_cnt - 1 );
							
							if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
							{
								m_data_manager.insert_screen_into_layouts( scr_global_ind );

								scr_data = layout.get_data( x, y );
								scr_data.m_scr_ind = (byte)scr_global_ind;
								layout.set_data( scr_data, x, y );
							}
							else
							{
								// clear all created screens and layout
								int scr_cnt = ( y * _scr_width ) + x + 1;
								
								m_data_manager.scr_data_pos = m_data_manager.scr_data_cnt - 1; 
								
								do
								{
									m_data_manager.screen_data_delete();
								}
								while( --scr_cnt > 0 );
								
								// reset screen pointer
								m_data_manager.scr_data_pos = -1;
								
								m_data_manager.layout_data_delete( false );
								
								throw new Exception( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT );
							}
						}
					}
				}
				
				return layout;
			}
			
			return null;
		}

		bool create_layout_with_empty_screens_end( layout_data _data )
		{
			if( _data != null )
			{
				// create screens and fill the layout
				{
					int scr_local_ind;
					int scr_global_ind;
					
					for( int y = 0; y < _data.get_height(); y++ )
					{
						for( int x = 0; x < _data.get_width(); x++ )
						{
							scr_global_ind = _data.get_data( x, y ).m_scr_ind;
							scr_local_ind = m_data_manager.get_local_screen_ind( m_data_manager.tiles_data_pos, scr_global_ind );
							
							m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type );
							
							ListBoxScreens.Items.Add( ListBoxScreens.Items.Count );
						}
					}
				}
				
				m_data_manager.layouts_data_pos = m_data_manager.layouts_data_cnt - 1;
				
				ListBoxLayouts.Items.Add( m_data_manager.layouts_data_pos );
				ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
				
				palette_group.Instance.set_palette( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
				
				m_data_manager.scr_data_pos = ListBoxScreens.SelectedIndex = m_data_manager.scr_data_cnt - 1;
				
				return true;
			}
			
			return false;
		}
		
		private int insert_screen_into_layouts( int _scr_local_ind )
		{
			int scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, _scr_local_ind );
			
			if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
			{
				m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, _scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type );
				
				palette_group.Instance.set_palette( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
				
				m_data_manager.insert_screen_into_layouts( scr_global_ind );
				
				return scr_global_ind;
			}
			
			return -1;
		}

		private void update_layouts_list_box()
		{
			ListBoxLayouts.Items.Clear();
			
			int size = m_data_manager.layouts_data_cnt;
			
			for( int i = 0; i < size; i++ )
			{
				ListBoxLayouts.Items.Add( i );
			}
		}

		void ScreensAutoUpdateToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			bool on = ( sender as ToolStripMenuItem ).Checked;
			ScreensAutoUpdateToolStripMenuItem.Checked = CheckBoxScreensAutoUpdate.Checked = !on;
			
			set_status_msg( "Screens auto update " + ( !on ? "enabled":"disabled" ) );
		}
		
		void CheckBoxScreensAutoUpdateChanged_Event(object sender, EventArgs e)
		{
			CheckBox obj = sender as CheckBox;
			
			ScreensAutoUpdateToolStripMenuItem.Checked = obj.Checked;
			
			BtnUpdateScreens.Enabled = !obj.Checked;
			
			set_status_msg( "Screens auto update " + ( obj.Checked ? "enabled":"disabled" ) );
		}
		
		private void enable_update_screens_btn( bool _on )
		{
			if( CheckBoxScreensAutoUpdate.Checked == false )
			{
				BtnUpdateScreens.Enabled = _on;
				BtnUpdateScreens.UseVisualStyleBackColor = !_on;
			}
		}
		
		private bool need_update_screens()
		{
			return BtnUpdateScreens.Enabled;
		}

		private bool update_screens_if_needed()
		{
			if( need_update_screens() )
			{
				m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, m_data_manager.tiles_data_pos );
				
				return true;
			}
			
			return false;
		}

		void BtnUpdateScreensClick_Event(object sender, EventArgs e)
		{
			update_screens( true );
		}
		
		void update_screens( bool _disable_upd_scr_btn, bool _show_status_msg = true )
		{
			// update_screens - may change a current palette
			update_screens_by_bank_id( _disable_upd_scr_btn, true );
			
			m_layout_editor.update();
			
			if( _show_status_msg )
			{
				set_status_msg( "Screen list updated" );
			}
		}
		
		void BtnLayoutAddUpRowClick_Event(object sender, EventArgs e)
		{
			layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			if( data != null )
			{
				data.add_up();
				
				m_layout_editor.update_dimension_changes();
			}
		}
		
		void BtnLayoutRemoveTopRowClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1; }, delegate( layout_data _data ) { return _data.remove_up(); }, "Remove Top Row" );
		}
		
		void BtnLayoutAddDownRowClick_Event(object sender, EventArgs e)
		{
			layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			if( data != null )
			{
				data.add_down();
			
				m_layout_editor.update_dimension_changes();
			}
		}
		
		void BtnLayoutRemoveBottomRowClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1; }, delegate( layout_data _data ) { return _data.remove_down(); }, "Remove Bottom Row" );
		}
		
		void BtnLayoutAddLeftColumnClick_Event(object sender, EventArgs e)
		{
			layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			if( data != null )
			{
				data.add_left();
				
				m_layout_editor.update_dimension_changes();
			}
		}
		
		void BtnLayoutRemoveLeftColumnClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1; }, delegate( layout_data _data ) { return _data.remove_left(); }, "Remove Left Column" );
		}
		
		void BtnLayoutAddRightColumnClick_Event(object sender, EventArgs e)
		{
			layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			if( data != null )
			{
				data.add_right();
				
				m_layout_editor.update_dimension_changes();
			}
		}
	
		void BtnLayoutRemoveRightColumnClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1; }, delegate( layout_data _data ) { return _data.remove_right(); }, "Remove Right Column" );				
		}

		void delete_layout_row_column( Func< layout_data, bool > _condition, Func< layout_data, bool > _act, string _caption_msg )
		{
			layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			if( data != null && _condition( data ) && message_box( "Are you sure?", _caption_msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( _act( data ) )
				{
					reset_entity_instance_preview();
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		void LayoutDeleteAllEntityInstancesToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			entity_instance ent_inst = m_layout_editor.get_selected_entity_instance();
			
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "Are you sure?", "Delete All Entity Instances", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_all_entities();
				
				m_layout_editor.reset_entity_instance();
				
				if( ent_inst != null )
				{
					fill_entity_data( null );
					
					CheckBoxPickupTargetEntity.Checked = false;
				}
				
				set_status_msg( "Layout Editor: all entity instances deleted" );
			}
		}
		
		void LayoutDeleteAllScreenMarksToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "Are you sure?", "Delete All Screen Marks", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_all_screen_marks();
				
				set_status_msg( "Layout Editor: all screen marks deleted" );
			}
		}		
		
		void LayoutDeleteScreenToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			entity_instance ent_inst = m_layout_editor.get_selected_entity_instance();
			
			if( m_layout_editor.delete_screen_from_layout() == true )
			{
				if( ent_inst != m_layout_editor.get_selected_entity_instance() )
				{
					fill_entity_data( null );
					
					CheckBoxPickupTargetEntity.Checked = false;
				}
				else
				{
					if( ent_inst != null )
					{
						fill_entity_data( ent_inst.base_entity, ent_inst.properties, ent_inst.name, ent_inst.target_uid );
					}
				}
				
				set_status_msg( "Layout Editor: screen deleted" );
			}
		}
		
		void LayoutDeleteEntityToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_layout_editor.delete_entity_instance_from_layout() == true )
			{
				fill_entity_data( null );
				
				set_status_msg( "Layout Editor: entity deleted" );
			}
		}
		
		void SetStartScreenMarkToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_layout_editor.set_start_screen_mark() == false )
			{
				message_box( "Please, select a valid screen!", "Start Screen Mark Setting Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void SetScreenMarkToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_screen_mark_form.ShowDialog() == DialogResult.OK )
			{
				if( m_layout_editor.set_screen_mark( m_screen_mark_form.mark ) == false )
				{
					message_box( "Please, select a valid screen!", "Screen Property Setting Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void AdjScrMaskClick_Event(object sender, EventArgs e)
		{
			if( m_layout_editor.set_adjacent_screen_mask( ( sender as ToolStripMenuItem ).Text ) == false )
			{
				message_box( "Please, select a valid screen!", "Adjacent Screen Mask Setting Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void ListViewScreensClick_Event(object sender, EventArgs e)
		{
			ListView lv = sender as ListView;
			
			if( lv.SelectedItems.Count > 0 )
			{
				m_layout_editor.set_active_screen( lv.SelectedItems[ 0 ].ImageIndex );
			
				set_status_msg( "Layout Editor: screen - " + lv.SelectedItems[ 0 ].Text );
			}
			else
			{
				m_layout_editor.set_active_screen( layout_data.CONST_EMPTY_CELL_ID );
			
				set_status_msg( "" );
			}
			
			m_layout_editor.update();
		}
		
		void BtnCreateLayoutClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				if( m_data_manager.layout_data_create() == true )
				{
					ListBoxLayouts.Items.Add( m_data_manager.layouts_data_cnt - 1 );
					m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_cnt - 1;
					
					reset_entity_instance_preview();
					
					set_status_msg( "Added layout" );
				}
				else
				{
					set_status_msg( "Failed to create layout" );
					
					message_box( "Can't create layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT, "Failed to Create Layout", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		void BtnCreateLayoutWxHClick_Event(object sender, EventArgs e)
		{
			try
			{
				if( m_data_manager.tiles_data_pos >= 0 && m_create_layout_form.ShowDialog() == DialogResult.OK )
				{
					if( create_layout_with_empty_screens_end( create_layout_with_empty_screens_beg( m_create_layout_form.layout_width, m_create_layout_form.layout_height ) ) != false )
					{
						reset_entity_instance_preview();
						
						m_layout_editor.update_dimension_changes();
							
						set_status_msg( "Added layout " + m_create_layout_form.layout_width + "x" + m_create_layout_form.layout_height );
					}
					else
					{
						throw new Exception( "Can't create layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT );
					}
				}
			}
			catch( Exception _err )
			{
				set_status_msg( "Failed to create layout or screen" );
				
				message_box( _err.Message, "Failed to Create Layout or Screen", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void BtnDeleteLayoutClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 && ListBoxLayouts.Items.Count > 0 && message_box( "Are you sure?", "Delete Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.layout_data_delete();

				update_layouts_list_box();
				
				ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
				
				reset_entity_instance_preview();
					
				set_status_msg( "Layout removed" );
			}
		}
		
		void BtnCopyLayoutClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 && ListBoxLayouts.Items.Count > 0 && message_box( "Are you sure?", "Copy Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.layout_data_copy() == true )
				{
					update_layouts_list_box();
					
					m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
					
					reset_entity_instance_preview();
					
					set_status_msg( "Layout copied" );
				}
				else
				{
					set_status_msg( "Failed to copy layout" );
					
					message_box( "Can't copy layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT, "Failed to Copy Layout", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		void ListBoxLayoutsClick_Event(object sender, EventArgs e)
		{
			m_data_manager.layouts_data_pos = ( sender as ListBox ).SelectedIndex;
			
			reset_entity_instance_preview();
		}

		void reset_entity_instance_preview()
		{
			if( m_layout_editor.mode == layout_editor.EMode.em_EditInstances )
			{
				m_layout_editor.reset_entity_instance();
			
				fill_entity_data( null );
			}
			else
			if( m_layout_editor.mode == layout_editor.EMode.em_PickupTargetEntity )
			{
				layout_editor_set_mode( layout_editor.EMode.em_EditInstances );
			}
		}
		
		void ScreenEditModeSingleToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			bool checked_state = ( sender as ToolStripMenuItem ).Checked;
			
			set_menu_item_screen_edit_layout_mode( checked_state );
			
			RBtnScreenEditModeSingle.Checked = !checked_state;
			RBtnScreenEditModeLayout.Checked = checked_state; 
		}
		
		void ScreenEditModeLayoutToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			bool checked_state = ( sender as ToolStripMenuItem ).Checked;
			
			set_menu_item_screen_edit_layout_mode( !checked_state );
			
			RBtnScreenEditModeLayout.Checked = !checked_state; 
			RBtnScreenEditModeSingle.Checked = checked_state;
		}
		
		void set_menu_item_screen_edit_layout_mode( bool _on )
		{
			ScreenEditModeLayoutToolStripMenuItem.Checked = _on;
			ScreenEditModeSingleToolStripMenuItem.Checked = !_on; 
		}
		
		void enable_screen_edit_controls( bool  _on )
		{
			BtnCreateScreen.Enabled 		= _on;
			BtnCopyScreen.Enabled 			= _on;
			BtnDeleteScreen.Enabled 		= _on;
			BtnDeleteEmptyScreens.Enabled	= _on;
			ListBoxScreens.Enabled			= _on;
			
			ListBoxScreens.SelectedIndex = -1;
		}
		
		void RBtnScreenEditModeSingleChanged_Event(object sender, EventArgs e)
		{
			bool checked_state = ( sender as RadioButton ).Checked;
			
			set_menu_item_screen_edit_layout_mode( !checked_state );
			
			enable_screen_edit_controls( true );
			
			m_screen_editor.mode = screen_editor.EMode.em_Single;
			
			ListViewScreens.Enabled = true;
			
			set_status_msg( "Screen edit mode: Single" );
		}
		
		void RBtnScreenEditModeLayoutChanged_Event(object sender, EventArgs e)
		{
			bool checked_state = ( sender as RadioButton ).Checked;
			
			set_menu_item_screen_edit_layout_mode( checked_state );
			
			enable_screen_edit_controls( false );
			
			m_screen_editor.mode = screen_editor.EMode.em_Layout;
			
			ListViewScreens.SelectedItems.Clear();
			ListViewScreens.Enabled = false;
			m_layout_editor.set_active_screen( -1 );
			m_layout_editor.update();
			
			set_status_msg( "Screen edit mode: Layout" );
		}
		
		void BtnLayoutMoveDownClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.Items.Count > 1 )
			{
				if( ListBoxLayouts.SelectedIndex != -1 )
				{
					if( ListBoxLayouts.SelectedIndex >= 0 && ( ListBoxLayouts.SelectedIndex + 1 ) < ListBoxLayouts.Items.Count )
					{
						m_data_manager.layout_swap( ListBoxLayouts.SelectedIndex, ListBoxLayouts.SelectedIndex + 1 );
						
						++ListBoxLayouts.SelectedIndex;
					}
				}
				else
				{
					message_box( "Please, select a layout!", "Move Layout Down", MessageBoxButtons.OK );
				}
			}
		}
		
		void BtnLayoutMoveUpClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.Items.Count > 1 )
			{
				if( ListBoxLayouts.SelectedIndex != -1 )
				{
					if( ListBoxLayouts.SelectedIndex > 0 )
					{
						m_data_manager.layout_swap( ListBoxLayouts.SelectedIndex, ListBoxLayouts.SelectedIndex - 1 );
						
						--ListBoxLayouts.SelectedIndex;
					}
				}
				else
				{
					message_box( "Please, select a layout!", "Move Layout Up", MessageBoxButtons.OK );
				}
			}
		}
		
		void ScreensShowAllBanksToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			ScreensShowAllBanksToolStripMenuItem.Checked = CheckBoxLayoutEditorAllBanks.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void CheckBoxLayoutEditorAllBanksCheckChanged_Event(object sender, EventArgs e)
		{
			ScreensShowAllBanksToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			update_screens_by_bank_id( true, need_update_screens() );
			
			m_layout_editor.update();
		}
		
		void update_screens_by_bank_id( bool _disable_upd_scr_btn, bool _update_images )
		{
			m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, _update_images, m_view_type, CheckBoxLayoutEditorAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
			
			// renew a palette
			palette_group.Instance.set_palette( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
			
			LabelLayoutEditorCHRBankID.Text = CheckBoxLayoutEditorAllBanks.Checked ? "XXX":CBoxCHRBanks.SelectedIndex.ToString();
		
			if( _disable_upd_scr_btn )
			{
				enable_update_screens_btn( false );
			}
		}
		
		void update_screens_labels_by_bank_id()
		{
			m_imagelist_manager.update_screens_labels( m_data_manager.get_tiles_data(), CheckBoxLayoutEditorAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
		}
		
		void LayoutShowMarksToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			LayoutShowMarksToolStripMenuItem.Checked = CheckBoxShowMarks.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void LayoutShowEntitiesToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			LayoutShowEntitiesToolStripMenuItem.Checked = CheckBoxShowEntities.Checked = !( sender as ToolStripMenuItem ).Checked;
			
			LayoutShowTargetsToolStripMenuItem.Enabled = LayoutShowCoordsToolStripMenuItem.Enabled = LayoutShowEntitiesToolStripMenuItem.Checked; 
		}
		
		void LayoutShowTargetsToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			LayoutShowTargetsToolStripMenuItem.Checked = CheckBoxShowTargets.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void LayoutShowCoordsToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			LayoutShowCoordsToolStripMenuItem.Checked = CheckBoxShowCoords.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void CheckBoxShowMarksChecked_Event(object sender, EventArgs e)
		{
			bool show_marks = ( sender as CheckBox ).Checked;
			
			m_layout_editor.show_marks = LayoutShowMarksToolStripMenuItem.Checked = show_marks;
		}
		
		void CheckBoxShowEntitiesChecked_Event(object sender, EventArgs e)
		{
			bool show_ent = ( sender as CheckBox ).Checked;
			
			m_layout_editor.show_entities = LayoutShowEntitiesToolStripMenuItem.Checked = show_ent;
			
			LayoutShowTargetsToolStripMenuItem.Enabled = LayoutShowCoordsToolStripMenuItem.Enabled = CheckBoxShowTargets.Enabled = CheckBoxShowCoords.Enabled = show_ent; 
		}
		
		void CheckBoxShowTargetsChecked_Event(object sender, EventArgs e)
		{
			m_layout_editor.show_targets = LayoutShowTargetsToolStripMenuItem.Checked = ( sender as CheckBox ).Checked; 
		}
		
		void CheckBoxShowCoordsChecked_Event(object sender, EventArgs e)
		{
			m_layout_editor.show_coords = LayoutShowCoordsToolStripMenuItem.Checked = ( sender as CheckBox ).Checked; 
		}
#endregion		
// ENTITY EDITOR *************************************************************************************//
#region entity editor
		void TreeViewEntitiesLabelEdit_Event(object sender, NodeLabelEditEventArgs e)
		{
			if( e.Label != null )
			{
				if( e.Node.Parent == null )
				{
					m_data_manager.group_rename( e.Node.Text, e.Label );
					e.Node.Name = e.Label;
				}
				else
				{
					m_data_manager.entity_rename( e.Node.Parent.Text, e.Node.Text, e.Label );
					e.Node.Name = e.Label;
					
					fill_entity_data( get_selected_entity() );
				}
			}
		}

		void TreeViewEntitiesSelect_Event(object sender, TreeViewEventArgs e)
		{
			if( tabControlScreensEntities.SelectedTab == TabEntities )
			{
				entity_data ent = get_selected_entity();
				
				if( ent != null )
				{
					layout_editor_set_mode( layout_editor.EMode.em_EditEntities );
				}
				
				fill_entity_data( ent );
			}
		}
		
		void BtnEntityGroupAddClick_Event(object sender, EventArgs e)
		{
			m_object_name_form.Text = "Add Group";
			
			m_object_name_form.edit_str = "GROUP";
			
			if( m_object_name_form.ShowWindow() == DialogResult.OK )
			{
				m_data_manager.group_add( m_object_name_form.edit_str );
				
				set_status_msg( "Added group" );
			}
		}
		
		void BtnEntityGroupDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent == null )
				{
					m_data_manager.group_delete( sel_node.Name );

					update_active_entity();
					
					set_status_msg( "Group deleted" );
				}
				else
				{
					message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewEntities.Nodes.Count > 0 )
				{
					message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		void BtnEntityGroupRenameClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				sel_node.BeginEdit();
			}
			else
			{
				message_box( "Please, select a group!", "Group Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void BtnEntityAddClick_Event(object sender, EventArgs e)
		{
			m_object_name_form.Text = "Add Entity";
			
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node == null )
			{
				message_box( "Please, select a group!", "Entity Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return;
			}
			
			m_object_name_form.edit_str = "entity";
			
			if( m_object_name_form.ShowWindow() == DialogResult.OK )
			{
				m_data_manager.entity_add( m_object_name_form.edit_str, ( sel_node.Parent != null ? sel_node.Parent.Name:sel_node.Name ) );
				
				set_status_msg( "Added entity" );
				
				layout_editor_set_mode( layout_editor.EMode.em_EditEntities );
				
				fill_entity_data( get_selected_entity() );
			}
		}
		
		void BtnEntityDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					m_data_manager.entity_delete( sel_node.Name, sel_node.Parent.Name );

					update_active_entity();
					
					set_status_msg( "Entity deleted" );
				}
				else
				{
					message_box( "Please, select an entity!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewEntities.Nodes.Count > 0 )
				{
					message_box( "Please, select an entity!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnEntityRenameClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				sel_node.BeginEdit();
			}
			else
			{
				message_box( "Please, select an item!", "Item Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void TreeViewEntities_update_data( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;  
			
			TreeViewEntities.BeginUpdate();
			{
				TreeViewEntities.Nodes.Clear();
				
				Dictionary< string, List< entity_data > > ents_data = data_mngr.entities_data;
				
				foreach( var key in data_mngr.entities_data.Keys ) 
				{
					TreeViewEntities_add_group( null, new EventArg2Params( key, null ) );
					          
					( data_mngr.entities_data[ key ] as List< entity_data > ).ForEach( delegate( entity_data _ent ) { TreeViewEntities_add_entity( null, new EventArg2Params( _ent.name, key ) ); } );
				}
			}
			TreeViewEntities.EndUpdate();
		}

		bool TreeViewEntities_add_group( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string name = args.param1 as string;
			
			TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( name, true );
			
			if( nodes_arr.Length > 0 )
			{
				message_box( "An item with the same name (" + name +  ") is already exist!", "Group Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			TreeNode node = new TreeNode( name );
			node.Name = name;
			node.ContextMenuStrip = ContextMenuEntitiesTreeGroup;
				
			TreeViewEntities.BeginUpdate();
			{
				TreeViewEntities.Nodes.Add( node );
				
				TreeViewEntities.SelectedNode = node;
			}
			TreeViewEntities.EndUpdate();
			
			return true;
		}
		
		bool TreeViewEntities_delete_group( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string name = args.param1 as string;
			
			if( TreeViewEntities.Nodes.ContainsKey( name ) )
			{
				TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( name, true );
				
				if( nodes_arr.Length > 0 )
				{
					if( nodes_arr[ 0 ].FirstNode != null )
					{
						if( message_box( "The selected group is not empty!\n\nRemove all child entities?", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
						{
							TreeViewEntities.BeginUpdate();
							{
								TreeViewEntities.Nodes.RemoveByKey( name );
							}
							TreeViewEntities.EndUpdate();
							
							return true;
						}
					}
					else
					{
						TreeViewEntities.BeginUpdate();
						{
							TreeViewEntities.Nodes.RemoveByKey( name );
						}
						TreeViewEntities.EndUpdate();
						
						return true;
					}
				}
			}
			
			return false;
		}
		
		bool TreeViewEntities_add_entity( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string ent_name = args.param1 as string;
			string grp_name = args.param2 as string;
			
			TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( ent_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				message_box( "An item with the same name (" + ent_name +  ") is already exist!", "Entity Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			nodes_arr = TreeViewEntities.Nodes.Find( grp_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				TreeNode node = new TreeNode( ent_name );
				node.Name = ent_name;
				node.ContextMenuStrip = ContextMenuEntitiesTreeEntity;
				
				TreeViewEntities.BeginUpdate();
				{
					nodes_arr[ 0 ].Nodes.Add( node );
					
					nodes_arr[ 0 ].Expand();
					
					TreeViewEntities.SelectedNode = node;
				}
				TreeViewEntities.EndUpdate();
				
				return true;
			}
			
			return false;
		}

		bool TreeViewEntities_delete_entity( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string ent_name = args.param1 as string;
			
			TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( ent_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				if( nodes_arr[ 0 ].Parent == null )
				{
					message_box( "Please, select an entity!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return false;
				}
				
				if( message_box( "Are you sure?", "Delete Entity", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					TreeViewEntities.BeginUpdate();
					{
						nodes_arr[ 0 ].Remove();
					}
					TreeViewEntities.EndUpdate();
					
					return true;
				}
				
				return false;
			}
			
			message_box( "Can't find entity (" + ent_name +  ")!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );			
			
			return false;
		}

		void CheckBoxEntitySnappingChanged_Event(object sender, EventArgs e)
		{
			m_layout_editor.entity_snapping = ( sender as CheckBox ).Checked;
		}
#endregion
// 	ENTITY PROPERTIES EDITOR *************************************************************************//
#region entity properties editor
		void fill_entity_data( entity_data _ent, string _inst_prop = "", string _inst_name = "", int _targ_uid = -1 )
		{
			groupBoxEntityEditor.Enabled = ( _ent != null );
			
			if( _ent != null )
			{
				bool edit_inst_mode = ( m_layout_editor.mode == layout_editor.EMode.em_EditInstances || m_layout_editor.mode == layout_editor.EMode.em_PickupTargetEntity );

				NumericUpDownEntityUID.Value 	= _ent.uid;
				NumericUpDownEntityWidth.Value 	= _ent.width;
				NumericUpDownEntityHeight.Value = _ent.height;
				NumericUpDownEntityPivotX.Value = _ent.pivot_x;
				NumericUpDownEntityPivotY.Value = _ent.pivot_y;
				PBoxColor.BackColor				= _ent.color;
				TextBoxEntityProperties.Text	= _ent.properties;
				LabelEntityName.Text			= edit_inst_mode ? _ent.name + "/" + _inst_name:_ent.name;
				
				TextBoxEntityInstanceProp.Text	= edit_inst_mode ? _inst_prop:_ent.inst_properties;
				
				TextBoxEntityProperties.BackColor = edit_inst_mode ? Color.Gainsboro:Color.FromName( "Window" );
				
				NumericUpDownEntityUID.Enabled 		= !edit_inst_mode;
				NumericUpDownEntityPivotX.Enabled 	= !edit_inst_mode;
				NumericUpDownEntityPivotY.Enabled 	= !edit_inst_mode;
				PBoxColor.Enabled					= !edit_inst_mode;
				TextBoxEntityProperties.Enabled		= !edit_inst_mode;
				BtnEntityLoadBitmap.Enabled			= !edit_inst_mode;
				
				CheckBoxPickupTargetEntity.Enabled	= edit_inst_mode;
				
				NumericUpDownEntityWidth.Enabled = NumericUpDownEntityHeight.Enabled = edit_inst_mode ? false:( _ent.image_flag ? false:true );
			}
			else
			{
				NumericUpDownEntityUID.Value 	= 0;
				NumericUpDownEntityWidth.Value 	= 1;
				NumericUpDownEntityHeight.Value = 1;
				NumericUpDownEntityPivotX.Value = 0;
				NumericUpDownEntityPivotY.Value = 0;
				PBoxColor.BackColor				= utils.CONST_COLOR_ENTITY_PIXBOX_INACTIVE;
				TextBoxEntityProperties.Text	= "space separated decimal values";
				TextBoxEntityInstanceProp.Text	= "space separated decimal values";
				LabelEntityName.Text			= "ENTITY NAME";
			}

			CheckBoxPickupTargetEntity.Text		= "Target UID: " + ( _targ_uid < 0 ? "none":_targ_uid.ToString() );
			
			update_entity_preview( _ent == null );
		}

		void PBoxColorClick(object sender, EventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null && ( ent.image_flag == false || ( ent.image_flag == true && message_box( "Delete the entity image and use the color box instead?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes ) ) )
			{
				if( colorDialogEntity.ShowDialog() == DialogResult.OK )
				{
					ent.image_flag 	= false;
					
					ent.color = colorDialogEntity.Color;
					
					fill_entity_data( ent );
				}
			}
		}
		
		entity_data get_selected_entity()
		{
			entity_data ent = null;
			
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null && sel_node.Parent != null )
			{
				List< entity_data > ent_list = m_data_manager.entities_data[ sel_node.Parent.Name ] as List< entity_data >;
				
				ent_list.ForEach( delegate( entity_data _ent ) { if( _ent.name == sel_node.Name ) { ent = _ent; return; } } );
			}
			
			return ent;
		}
		
		void NumericUpDownEntityUIDChanged_Event(object sender, EventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				ent.uid = ( byte )NumericUpDownEntityUID.Value;
			}
		}
		
		void NumericUpDownEntityWidthChanged_Event(object sender, EventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.width != ( byte )NumericUpDownEntityWidth.Value )
				{
					ent.width = ( byte )NumericUpDownEntityWidth.Value;
					
					update_entity_preview();
				}
			}
		}
		
		void NumericUpDownEntityHeightChanged_Event(object sender, EventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.height != ( byte )NumericUpDownEntityHeight.Value )
				{
					ent.height = ( byte )NumericUpDownEntityHeight.Value;
					
					update_entity_preview();
				}
			}
		}
		
		void NumericUpDownEntityPivotXChanged_Event(object sender, EventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.pivot_x != ( sbyte )NumericUpDownEntityPivotX.Value )
				{
					ent.pivot_x = ( sbyte )NumericUpDownEntityPivotX.Value;
					
					update_entity_preview();
				}
			}
		}
		
		void NumericUpDownEntityPivotYChanged_Event(object sender, EventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.pivot_y != ( sbyte )NumericUpDownEntityPivotY.Value )
				{
					ent.pivot_y = ( sbyte )NumericUpDownEntityPivotY.Value;
					
					update_entity_preview();
				}
			}
		}
		
		void BtnEntityLoadBitmapClick(object sender, EventArgs e)
		{
			EntityLoadBitmap_openFileDialog.ShowDialog();
		}
		
		void EntityLoadBitmap_openFileDialogFileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			String filename = ( ( FileDialog )sender ).FileName;
			
			Bitmap bmp			= null;
			Bitmap unlocked_bmp	= null;
			
			try
			{
				bmp = new Bitmap( filename );
				
				// unlock the bmp
				unlocked_bmp = new Bitmap( bmp, bmp.Width, bmp.Height );
				bmp.Dispose();
				bmp = null;
				
				entity_data ent = get_selected_entity();
				
				if( ent.width != unlocked_bmp.Width || ent.height != unlocked_bmp.Height )
				{
					if( message_box( "Rescale the loaded image to the entity size?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
					{
						Bitmap rescaled_bmp = new Bitmap( unlocked_bmp, new Size( ent.width, ent.height ) );
						
						unlocked_bmp.Dispose();
						unlocked_bmp = rescaled_bmp;
					}
					else
					{
						if( unlocked_bmp.Width > 255 || unlocked_bmp.Height > 255 )
						{
							throw new Exception( "Invalid image size! The width and height must be less than 256!" );
						}
						
						ent.width 	= (byte)unlocked_bmp.Width;
						ent.height 	= (byte)unlocked_bmp.Height;
						ent.pivot_x = 0;
						ent.pivot_y = 0;
					}
				}
				
				if( ent != null )
				{
					ent.bitmap 		= unlocked_bmp;
					ent.image_flag 	= true;
				}

				fill_entity_data( ent );
			}
			catch( Exception _err )
			{
				if( bmp != null )
				{
					bmp.Dispose();
				}

				if( unlocked_bmp != null )
				{
					unlocked_bmp.Dispose();
				}
				
				message_box( _err.Message, "Entity Image Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void TextBoxEntityInstancePropTextKeyUp_Event(object sender, KeyEventArgs e)
		{
			entity_instance ent_inst = m_layout_editor.get_selected_entity_instance();
			
			entity_data ent = get_selected_entity();

			TextBox text_box = sender as TextBox;

			if( ent_inst != null )
			{
				ent_inst.properties = text_box.Text;
			}
			else
			if( ent != null )
			{
				ent.inst_properties = text_box.Text;
			}
		}

		void TextBoxEntityPropertiesTextKeyUp_Event(object sender, KeyEventArgs e)
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				TextBox text_box = sender as TextBox;
				
				ent.properties = text_box.Text;
			}
		}
		
		void TextBoxEntityPropertiesKeyPress_Event(object sender, KeyPressEventArgs e)
		{
		    if( !char.IsControl( e.KeyChar ) && !char.IsDigit( e.KeyChar ) && ( e.KeyChar != ' ' ) )
		    {
		    	e.Handled = true;
		    }
		}

		void ComboBoxEntityZoomChanged_Event(object sender, EventArgs e)
		{
			update_entity_preview();
		}
		
		void update_entity_preview( bool _force_disable = false )
		{
			entity_instance ent_inst = m_layout_editor.get_selected_entity_instance();
			
			entity_data ent = ( ent_inst != null && _force_disable == false ) ? ent_inst.base_entity:get_selected_entity();
			
			if( ent != null )
			{
				m_entity_preview.update( ent.bitmap, ent.width, ent.height, ent.pivot_x, ent.pivot_y, ( int )Math.Pow( 2.0, ComboBoxEntityZoom.SelectedIndex ), true );
			}
			else
			{
				m_entity_preview.update( null, -1, -1, -1, -1, -1, false );
			}
			
			if( CheckBoxShowEntities.Checked )
			{
				m_layout_editor.update();
			}
		}
		
		void BtnEntitiesEditInstancesModeClick_Event(object sender, EventArgs e)
		{
			if( tabControlScreensEntities.SelectedTab == TabEntities )
			{
				layout_editor_set_mode( layout_editor.EMode.em_EditInstances );
			}
		}
		
		void update_active_entity()
		{
			entity_data sel_ent = get_selected_entity();
			
			if( sel_ent == null )
			{
				layout_editor_set_mode( layout_editor.EMode.em_EditInstances );
			}
			else
			{
				m_layout_editor.set_active_entity( sel_ent );
				m_layout_editor.update();
			}
		}
		
		void layout_editor_set_mode( layout_editor.EMode _mode )
		{
			switch( _mode )
			{
				case layout_editor.EMode.em_EditEntities:
					{
						m_layout_editor.reset_entity_instance();
						m_layout_editor.set_active_entity( get_selected_entity() );
						
						m_layout_editor.mode = _mode;
						
						CheckBoxPickupTargetEntity.Checked = false;
					}
					break;
					
				case layout_editor.EMode.em_EditInstances:
					{
						TreeViewEntities.SelectedNode = null;
			
						m_layout_editor.reset_entity_instance();
						m_layout_editor.set_active_entity( null );
						
						m_layout_editor.mode = _mode;
						
						fill_entity_data( get_selected_entity() );
						
						CheckBoxPickupTargetEntity.Checked = false;
					}
					break;
					
				case layout_editor.EMode.em_PickupTargetEntity:
					{
						m_layout_editor.set_active_entity( null );
						
						m_layout_editor.mode = _mode;
					}
					break;
					
				case layout_editor.EMode.em_Screens:
					{
						m_layout_editor.reset_entity_instance();
						m_layout_editor.set_active_entity( null );
						
						m_layout_editor.mode = _mode;
						
						CheckBoxPickupTargetEntity.Checked = false;
					}
					break;
			}
		}
		
		void TabControlScreensEntitiesSelected_Event(object sender, TabControlEventArgs e)
		{
			TabPage curr_tab = ( sender as TabControl ).SelectedTab;
			
			if( curr_tab == TabEntities )
			{
				ListViewScreens.SelectedItems.Clear();
				m_layout_editor.set_active_screen( -1 );
				
				layout_editor_set_mode( layout_editor.EMode.em_EditInstances );
			}
			else
			if( curr_tab == TabScreenList )
			{
				TreeViewEntities.SelectedNode = null;
				fill_entity_data( get_selected_entity() );
				
				layout_editor_set_mode( layout_editor.EMode.em_Screens );
			}
		}
		
		void MainForm_EntityInstanceSelected( object sender, EventArgs e )
		{
			EventArg2Params args = e as EventArg2Params;

			entity_instance ent_inst = args.param1 as entity_instance;
			
			if( m_layout_editor.mode == layout_editor.EMode.em_EditInstances )
			{
				if( ent_inst != null )
				{
					fill_entity_data( ent_inst.base_entity, ent_inst.properties, ent_inst.name, ent_inst.target_uid );
				}
				else
				{
					fill_entity_data( null, null );
				}
			}
			else
			if( m_layout_editor.mode == layout_editor.EMode.em_PickupTargetEntity )
			{
				entity_instance edit_ent_inst = m_layout_editor.get_selected_entity_instance();
				
				if( edit_ent_inst != null )
				{
					edit_ent_inst.target_uid = ( ent_inst != null ) ? ( ( ent_inst != edit_ent_inst ) ? ent_inst.uid:-1 ):-1;
					
					fill_entity_data( edit_ent_inst.base_entity, edit_ent_inst.properties, edit_ent_inst.name, edit_ent_inst.target_uid );
				}
			}
		}
		
		void CheckBoxPickupTargetEntityChanged_Event(object sender, EventArgs e)
		{
			if( ( sender as CheckBox ).Checked )
			{
				layout_editor_set_mode( layout_editor.EMode.em_PickupTargetEntity );
			}
			else
			{
				if( m_layout_editor.mode == layout_editor.EMode.em_PickupTargetEntity )
				{
					m_layout_editor.mode = layout_editor.EMode.em_EditInstances;
				}
			}			
		}
#endregion		
// 	PALETTE ******************************************************************************************//
#region palette
		void CheckBoxPalettePerCHRChecked_Event(object sender, EventArgs e)
		{
			m_tiles_processor.set_block_editor_palette_per_CHR_mode( ( sender as CheckBox ).Checked );
		}
		
		void BtnSwapColorsClick_Event(object sender, EventArgs e)
		{
#if !DEF_NES
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				if( m_swap_colors_form.ShowDialog( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) ) == DialogResult.OK )
				{
					BtnUpdateGFXClick_Event( null, null );
					
					palette_group.Instance.update_selected_color();
				}
			}
#endif
		}
		
		void CBoxPalettesChanged_Event(object sender, EventArgs e)
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			data.palette_pos = CBoxPalettes.SelectedIndex; 
			
			update_palette_related_data( data );
			
			set_status_msg( "Palette changed" );
		}
		
		void BtnPltCopyClick_Event(object sender, EventArgs e)
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			
			if( data != null )
			{
#if DEF_FIXED_LEN_PALETTE16_ARR
				if( m_palette_copy_ind < 0 )
				{
					// copy
					BtnPltCopy.Text = "Paste";
					m_palette_copy_ind = data.palette_pos;
				}
				else
				{
					// paste
					data.palettes_arr[ data.palette_pos ].copy( data.palettes_arr[ m_palette_copy_ind ] );
					
					BtnPltCopy.Text = "Copy";
					m_palette_copy_ind = -1;

					update_palette_related_data( data );
				}
#else
				if( data.palette_copy() )
				{
					palette_group.Instance.set_palette( data );
					
					update_palettes_arr( data, true );
					enable_update_gfx_btn_Event( this, null );
				}
				else
				{
					message_box( "The maximum allowed number of palettes - " + utils.CONST_PALETTES_MAX_CNT, "Copy Active Palette", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
#endif
			}
		}
		
		void BtnPltDeleteClick_Event(object sender, EventArgs e)
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			
			if( data != null )
			{
				if( message_box( "Are you sure?", "Delete Active Palette", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					if( data.palette_delete() )
					{
						palette_group.Instance.set_palette( data );
						
						update_palettes_arr( data, true );
						enable_update_gfx_btn_Event( this, null );
					}
					else
					{
						message_box( "Can't delete the last palette!", "Delete Active Palette", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
		}

		void update_palette_related_data( tiles_data _data )
		{
			palette_group plt_grp = palette_group.Instance;
			plt_grp.set_palette( _data );
			plt_grp.update_selected_color();
			
			enable_update_gfx_btn( true );
			enable_update_screens_btn( true );
			
#if DEF_PALETTE16_PER_CHR
			m_tiles_processor.block_editor_update_sel_CHR_palette();
#endif
			m_tiles_processor.update_graphics();
		}
		
		void update_palettes_arr( tiles_data _data, bool _update_pos )
		{
			CBoxPalettes.Items.Clear();
			
			for( int i = 0; i < _data.palettes_arr.Count; i++ )
			{
				CBoxPalettes.Items.Add( String.Format( "plt#{0:d2}", i ) );
			}

			if( _update_pos )
			{
				CBoxPalettes.SelectedIndex = _data.palette_pos;
			}
		}
		
#if DEF_PALETTE16_PER_CHR
		void update_palette_list_pos( object sender, EventArgs e )
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			
			if( data != null )
			{
				CBoxPalettes.SelectedIndex = data.palette_pos;
			}
		}
#endif
#endregion		
//	SCREEN DATA CONVERTER ****************************************************************************//
#region screen data converter
		void BtnScreenDataInfoClick_Event(object sender, EventArgs e)
		{
			message_box( strings.CONST_SCREEN_DATA_TYPE_INFO, strings.CONST_SCREEN_DATA_TYPE_INFO_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		void RBtnScreenDataTilesClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				RadioButton rbtn = ( RadioButton )sender;
				
				if( !rbtn.Checked )
				{
					if( message_box( strings.CONST_SCREEN_DATA_CONV_BLOCKS2TILES, strings.CONST_SCREEN_DATA_CONV_BLOCKS2TILES_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						progress_bar_show( true, "Blocks (2x2) -> Tiles (4x4)", false );
						
						if( set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Tiles4x4 ) )
						{
							update_graphics( false );
						}
						
						progress_bar_show( false );
					}
				}
			}
		}
		
		void RBtnScreenDataBlocksClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				RadioButton rbtn = ( RadioButton )sender;
				
				if( !rbtn.Checked )
				{
					if( message_box( strings.CONST_SCREEN_DATA_CONV_TILES2BLOCKS, strings.CONST_SCREEN_DATA_CONV_TILES2BLOCKS_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						progress_bar_show( true, "Tiles (4x4) -> Blocks (2x2)", false );
						
						if( set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Blocks2x2 ) )
						{
							update_graphics( false );
						}
						
						progress_bar_show( false );
					}
				}
			}
		}
		
		bool set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			try
			{
				m_data_manager.screen_data_type = _type;
			}
			catch( Exception _err ) 
			{
				set_status_msg( "Screen data coversion canceled!" );
				message_box( _err.Message, "Data Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			switch( _type )
			{
				case data_sets_manager.EScreenDataType.sdt_Tiles4x4:
					{
						RBtnScreenDataTiles.Checked		= true;
						RBtnScreenDataBlocks.Checked	= false;

						PanelTiles.Enabled = true;
						
						GrpBoxTileEditor.Enabled = true;
						
						ScreenDataTypeLabel.Text = "Tiles4x4";
						
						m_tile_list_manager.visible( tile_list.EType.t_Tiles, true );
					}
					break;

				case data_sets_manager.EScreenDataType.sdt_Blocks2x2:
					{
						RBtnScreenDataTiles.Checked		= false;
						RBtnScreenDataBlocks.Checked	= true;
						
						PanelTiles.Enabled = false;
						
						GrpBoxTileEditor.Enabled = false;
						
						ScreenDataTypeLabel.Text = "Blocks2x2";
						
						m_tiles_processor.tile_select_event( -1, null );
						
						m_tile_list_manager.visible( tile_list.EType.t_Tiles, false );
					}
					break;
			}
			
			m_screen_editor.set_screen_data_type( _type );
			m_tiles_palette_form.set_screen_data_type( _type );
			m_optimization_form.set_screen_data_type( _type );
			m_layout_editor.set_screen_data_type( _type );
			
			m_patterns_manager_form.set_data( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
			m_patterns_manager_form.set_screen_data_type( _type );
			
			RBtnScreenEditModeSingle.Checked = true;
			
			return true;
		}
		
#endregion		
	}
}