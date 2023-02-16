/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 01.05.2017
 * Time: 15:24
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace MAPeD
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	/// 
	
	public partial class MainForm : Form
	{
		private readonly progress_form			m_progress_form;
		private readonly IProgress< int >		m_progress_val;
		private readonly IProgress< string >	m_progress_status;
		
		private bool	m_disable_status_wnd	= false;
		
		private readonly exporter_zx_sjasm 	m_exp_zx_asm;
#if !DEF_ZX
		private readonly exporter_asm 		m_exp_asm;
#endif
#if !DEF_NES
		private readonly swap_colors_form m_swap_colors_form;
#endif
		private readonly data_conversion_options_form m_data_conversion_options_form;
		
		private readonly data_sets_manager	m_data_manager;
		private readonly tiles_processor	m_tiles_processor;	
		private readonly layout_editor_base	m_layout_editor;
		
		private readonly image_preview		m_tile_preview;
		private readonly image_preview		m_pattern_preview;
		
		private readonly Bitmap		m_pattern_image;
		private readonly Graphics	m_pattern_gfx;
		
		private readonly tiles_palette_form		m_tiles_palette_form;
		private readonly optimization_form		m_optimization_form;
		private readonly object_name_form		m_object_name_form;
		private readonly import_tiles_form		m_import_tiles_form;
		private readonly description_form		m_description_form;
		private readonly statistics_form		m_statistics_form;
		private readonly create_layout_form		m_create_layout_form;
		private readonly reorder_CHR_banks_form m_reorder_CHR_banks_form;
		
		private SPSeD.py_editor		m_py_editor	= null;
		
		private utils.e_tile_view_type m_view_type	= utils.e_tile_view_type.UNKNOWN;

#if DEF_FIXED_LEN_PALETTE16_ARR
		private int	m_palette_copy_ind	= -1;
#endif
		private readonly export_active_tile_block_set_form m_export_active_tile_block_set_form;
		
		private readonly image_preview m_entity_preview;
		
		private int	m_block_copy_item_ind	= -1;
		private int	m_tile_copy_item_ind	= -1;
		
		private readonly imagelist_manager 	m_imagelist_manager;
		private readonly tile_list_manager	m_tile_list_manager;
		
		private static ToolStripStatusLabel	m_status_bar_label = null;
		
		enum e_copy_paste_type
		{
			CHRBank		= 1,
			BlocksList	= 2,
			TilesList	= 4,
			All			= 7,
		};

		private struct tooltip_data
		{
			public Control m_cntrl;
			public string m_desc;
			
			public tooltip_data( Control _cntrl, string _desc )
			{
				m_cntrl	= _cntrl;
				m_desc 	= _desc;
			}
		};
		
		private static NativeWindow	m_native_wnd;
		
		private static Form m_form = null;
		
		private static Form form()
		{
			return m_form;
		}
		
		public MainForm( string[] _args )
		{
			m_form = this;
			
			// for thread safe calls, ex: the message_box() can be called from another thread
			m_native_wnd = new NativeWindow();
			m_native_wnd.AssignHandle( this.Handle );
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

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
			
			m_tiles_processor.subscribe_block_quad_selected_event( on_block_quad_selected );

			m_tile_list_manager = new tile_list_manager();
			
			m_imagelist_manager	= new imagelist_manager( PanelTiles, PanelTilesClick, ContextMenuTilesList, PanelBlocks, PanelBlocksClick, ContextMenuBlocksList, ListViewScreens, m_tile_list_manager );
			
			// layout editor init
			{
				m_layout_editor = new layout_editor_base( m_data_manager, PBoxLayout, LayoutLabel, m_imagelist_manager );
				m_layout_editor.subscribe( m_data_manager );
				m_layout_editor.subscribe( layout_editor_base.e_mode.Entities, layout_editor_param.CONST_SUBSCR_ENT_INST_SELECT, on_entity_instance_selected );
				m_layout_editor.subscribe( layout_editor_base.e_mode.Entities, layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION, on_edit_entity_cancel );
				
				m_layout_editor.subscribe( layout_editor_base.e_mode.Screens, layout_editor_param.CONST_SUBSCR_SCR_RESET_SELECTED, on_reset_selected_screen );
				
				m_layout_editor.subscribe( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SUBSCR_PNT_UPDATE_TILE_IMAGE, on_update_tile_image );
				m_layout_editor.subscribe( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION, on_active_tile_cancel );
				
				m_layout_editor.subscribe( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SUBSCR_PTTRN_EXTRACT_END, on_pattern_extract_end );
				m_layout_editor.subscribe( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION, on_pattern_place_cancel );
				
				m_layout_editor.set_param( layout_editor_param.CONST_SET_BASE_SUBSCR_DATA_MNGR, m_data_manager );
				
				m_layout_editor.MapScaleX1 += new EventHandler( on_map_scale_x1 );
				m_layout_editor.MapScaleX2 += new EventHandler( on_map_scale_x2 );
				
				// set the builder mode by default
				m_layout_editor.mode = layout_editor_base.e_mode.Builder;
				
				m_tile_preview = new image_preview( PBoxActiveTile, false );

				layout_screen_data.EntityAdded		+= new EventHandler( on_entities_counter_update );
				layout_screen_data.EntityRemoved	+= new EventHandler( on_entities_counter_update );
			}
			
			// patterns manager
			{
				m_pattern_preview = new image_preview( PBoxPatternPreview, true );
				
				PBoxPatternPreview.MouseEnter += new EventHandler( on_patterns_manager_mouse_enter );
				
				// create graphics for drawing patterns
				int scr_tile_size = utils.CONST_SCREEN_TILES_SIZE >> 1;
				
				m_pattern_image	= new Bitmap( scr_tile_size * platform_data.get_screen_tiles_width(), scr_tile_size * platform_data.get_screen_tiles_height(), PixelFormat.Format32bppPArgb );
				m_pattern_gfx	= Graphics.FromImage( m_pattern_image );
			}
			
			m_data_manager.SetEntitiesData	+= new EventHandler( on_entities_tree_view_update_data );
			m_data_manager.AddEntity		+= on_entities_tree_view_add_entity;
			m_data_manager.DeleteEntity		+= on_entities_tree_view_delete_entity;
			m_data_manager.AddGroup			+= on_entities_tree_view_add_group;
			m_data_manager.DeleteGroup		+= on_entities_tree_view_delete_group;
			
			m_entity_preview = new image_preview( PBoxEntityPreview, true );

			PBoxEntityPreview.MouseEnter += new EventHandler( on_entities_manager_mouse_enter );
			
			m_status_bar_label = StatusBarLabel;
			
			m_object_name_form					= new object_name_form();
			m_import_tiles_form					= new import_tiles_form();
			m_description_form					= new description_form();
			m_statistics_form					= new statistics_form( m_data_manager );
			m_create_layout_form				= new create_layout_form();
			m_export_active_tile_block_set_form	= new export_active_tile_block_set_form();
			m_reorder_CHR_banks_form			= new reorder_CHR_banks_form( m_data_manager );

			enable_update_gfx_btn( false );
			mark_update_screens_btn( true );

			m_tiles_palette_form = new tiles_palette_form( m_imagelist_manager.get_tiles_image_list(), ContextMenuTilesList, m_imagelist_manager.get_blocks_image_list(), ContextMenuBlocksList, m_tile_list_manager );
			m_tiles_palette_form.FormHided			+= new EventHandler( on_tiles_blocks_form_hided );
			m_tiles_palette_form.TileSelected 		+= new EventHandler( on_tiles_blocks_form_tile_selected );
			m_tiles_palette_form.BlockSelected 		+= new EventHandler( on_tiles_blocks_form_block_selected );
			m_tiles_palette_form.ResetActiveTile 	+= new EventHandler( BtnResetTileClick );
			
			m_optimization_form = new optimization_form( m_data_manager, progress_bar_show );
			m_optimization_form.UpdateGraphics += new EventHandler( on_update_gfx_after_optimization );
			
#if DEF_PALETTE16_PER_CHR
			m_tiles_processor.UpdatePaletteListPos	+= new EventHandler( on_update_palette_list_pos );
#endif
			m_tiles_processor.NeedGFXUpdate 	+= new EventHandler( on_enable_update_gfx_btn );
			
			TabTiles.Tag 		= new Point( TabTiles.Width,	TabTiles.Height		);
			TabLayout.Tag 		= new Point( TabLayout.Width,	TabLayout.Height	);
			TabTiles.Tag 		= new Point( TabLayout.Width,	TabLayout.Height	);
			
			CBoxPalettes.DrawItem += new DrawItemEventHandler( CBoxPalettesDrawItem );

			FormClosing += new FormClosingEventHandler( OnFormClosing );

			// setup tooltips
			{
				tooltip_data[] tooltips = new tooltip_data[]{ 	new tooltip_data( tabControlTilesLayout, "Double Click to detach the tab page" ),
																new tooltip_data( CheckBoxScreensAutoUpdate, "Automatically updates the screen list when \"Update GFX\" button is pressed" ),
																new tooltip_data( CheckBoxScreensShowAllBanks, "Show screens of all CHR banks" ),
																new tooltip_data( CheckBoxSelectTargetEntity, "Select target entity" ),
																new tooltip_data( BtnCopyCHRBank, "Add copy of active CHR bank" ),
																new tooltip_data( BtnAddCHRBank, "Add new CHR bank" ),
																new tooltip_data( BtnDeleteCHRBank, "Delete active CHR Bank" ),
																new tooltip_data( BtnCHRBankPrevPage, "Previous CHR Bank's page" ),
																new tooltip_data( BtnCHRBankNextPage, "Next CHR Bank's page" ),
																new tooltip_data( BtnUpdateGFX, "Update tiles\\blocks and screens ( if auto update is enabled )" ),
																new tooltip_data( BtnOptimization, "Delete unused screens\\tiles\\blocks\\CHRs" ),
																new tooltip_data( CheckBoxTileEditorLock, "Enable\\disable tile editing" ),
																new tooltip_data( BtnTilesBlocks, "Arrays of tiles and blocks for a map building" ),
																new tooltip_data( Palette0, "Shift+1 / Ctrl+1,2,3,4 to select a color" ),
																new tooltip_data( Palette1, "Shift+2 / Ctrl+1,2,3,4 to select a color" ),
#if DEF_ZX
																new tooltip_data( Palette2, "Shift+3" ),
																new tooltip_data( Palette3, "Shift+4" ),
																new tooltip_data( BtnSwapInkPaper, "Swap ink to paper. Visible in \'B/W\' mode." ),
																new tooltip_data( BtnInvInk, "Invert image" ),
#else
																new tooltip_data( Palette2, "Shift+3 / Ctrl+1,2,3,4 to select a color" ),
																new tooltip_data( Palette3, "Shift+4 / Ctrl+1,2,3,4 to select a color" ),
#endif
																new tooltip_data( CheckBoxShowMarks, "Show screen marks" ),
																new tooltip_data( CheckBoxShowEntities, "Show layout entities" ),
																new tooltip_data( CheckBoxShowTargets, "Show entity targets" ),
																new tooltip_data( CheckBoxShowCoords, "Show coordinates of a selected entity" ),
																new tooltip_data( CheckBoxShowGrid, "Show tile grid" ),
																new tooltip_data( CheckBoxPalettePerCHR, "MMC5 extended attributes mode" ),
																new tooltip_data( CBoxPalettes, "Palettes array" ),
#if DEF_FIXED_LEN_PALETTE16_ARR
																new tooltip_data( BtnPltCopy, "Copy palette to selected slot" ),
#else
																new tooltip_data( BtnPltCopy, "Add copy of active palette" ),
#endif
																new tooltip_data( BtnPltDelete, "Delete active palette" ),
																new tooltip_data( BtnSwapColors, "Swap two selected colors without changing graphics" ),
																new tooltip_data( BtnBlockReserveCHRs, "Make links to empty CHRs" ),
																new tooltip_data( BtnTileReserveBlocks, "Make links to empty blocks" ),
																new tooltip_data( BtnLayoutDeleteEmptyScreens, "Delete all one-block filled screens in active layout" ),
																new tooltip_data( BtnCreateLayoutWxH, "Create layout (width x height) filled with empty screens" ),
																new tooltip_data( BtnLayoutMoveUp, "Move selected layout up" ),
																new tooltip_data( BtnLayoutMoveDown, "Move selected layout down" ),
																new tooltip_data( CBoxBlockObjId, "Assign property to selected block or CHR" ),
																new tooltip_data( LabelObjId, "Assign property to selected block or CHR" ),
																new tooltip_data( LabelEntityProperty, "Entity properties are inherited by all instances" ),
																new tooltip_data( LabelEntityInstanceProperty, "Instance properties are unique for all instances" ),
																new tooltip_data( CheckBoxEntitySnapping, "Snap an entity to 8x8 grid" ),
																new tooltip_data( BtnPainterFillWithTile, "Fill selected screens with active tile" ),
																new tooltip_data( CheckBoxPainterReplaceTiles, "Replace a group of identical tiles on selected screens" ),
															};
				tooltip_data data;
				
				for( int i = 0; i < tooltips.Length; i++ )
				{
					data = tooltips[ i ];
					
					( new ToolTip(components) ).SetToolTip( data.m_cntrl, data.m_desc );
				}
			}

#if !DEF_NES
			CheckBoxPalettePerCHR.Visible = false; // NES:MMC5 feature
#endif

#if DEF_NES
			ProjectOpenFileDialog.DefaultExt = platform_data.CONST_SMS_FILE_EXT;
			ProjectOpenFileDialog.Filter = get_all_projects_open_file_filter( platform_data.e_platform_type.NES );
			
			BtnSwapColors.Visible = false;
#elif DEF_SMS
			ProjectSaveFileDialog.DefaultExt = platform_data.CONST_SMS_FILE_EXT;
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "NES", "SMS" );
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "nes", "sms" );

			ProjectOpenFileDialog.DefaultExt = platform_data.CONST_SMS_FILE_EXT;
			ProjectOpenFileDialog.Filter = get_all_projects_open_file_filter( platform_data.e_platform_type.SMS );
			
			ProjectExportFileDialog.Filter = ProjectExportFileDialog.Filter.Replace( "CA65\\NESasm", "WLA-DX" );

			ImportOpenFileDialog.Filter = ImportOpenFileDialog.Filter.Replace( "NES", "SMS" );
			ImportOpenFileDialog.Filter = ImportOpenFileDialog.Filter.Replace( "nes", "sms" );
			ImportOpenFileDialog.Filter = ImportOpenFileDialog.Filter.Replace( "Map 2/4 bpp", "Map 2/4/8 bpp" );
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false; 
#elif DEF_PCE
			ProjectSaveFileDialog.DefaultExt = platform_data.CONST_PCE_FILE_EXT;
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "NES", "PCE" );
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "nes", "pce" );

			ProjectOpenFileDialog.DefaultExt = platform_data.CONST_PCE_FILE_EXT;
			ProjectOpenFileDialog.Filter = get_all_projects_open_file_filter( platform_data.e_platform_type.PCE );
			
			ProjectExportFileDialog.Filter = ProjectExportFileDialog.Filter.Replace( "CA65\\NESasm (*.asm)", "CA65\\PCEAS\\HuC (*.asm;*.h)" );

			ImportOpenFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin|Palette (1536 bytes) (*.pal)|*.pal";
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false;
#elif DEF_ZX
			ProjectSaveFileDialog.DefaultExt = platform_data.CONST_ZX_FILE_EXT;
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "NES", "ZX" );
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "nes", "zx" );

			ProjectOpenFileDialog.DefaultExt = platform_data.CONST_ZX_FILE_EXT;
			ProjectOpenFileDialog.Filter = get_all_projects_open_file_filter( platform_data.e_platform_type.ZX );
			
			ProjectExportFileDialog.Filter = ProjectExportFileDialog.Filter.Substring( ProjectExportFileDialog.Filter.IndexOf( "Z" ) );
			ProjectExportFileDialog.DefaultExt = "zxa";

			ImportOpenFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin|Palette (48 bytes) (*.pal)|*.pal";
			
			BtnSwapColors.Visible = false;
			
			BtnPltCopy.Enabled = BtnPltDelete.Enabled = BtnSwapColors.Enabled = false;
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false;
			
			LabelPalette12.Visible	= true;
			LabelPalette34.Visible	= true;
			BtnSwapInkPaper.Visible	= true;
			BtnInvInk.Visible		= true;
			
			ZXToolStripSeparator.Visible = ZXSwapInkPaperToolStripMenuItem.Visible = ZXInvertImageToolStripMenuItem.Visible = true;
			
			CBoxTileViewType.Items.Add( "B/W" );
			CBoxTileViewType.Items.Add( "Inv B/W" );
#elif DEF_SMD
			ProjectSaveFileDialog.DefaultExt = platform_data.CONST_SMD_FILE_EXT;
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "NES", "SMD" );
			ProjectSaveFileDialog.Filter = ProjectSaveFileDialog.Filter.Replace( "nes", "smd" );

			ProjectOpenFileDialog.DefaultExt = platform_data.CONST_SMD_FILE_EXT;
			ProjectOpenFileDialog.Filter = get_all_projects_open_file_filter( platform_data.e_platform_type.SMD );
			
			ProjectExportFileDialog.Filter = ProjectExportFileDialog.Filter.Replace( "CA65\\NESasm (*.asm)", "vasm\\SGDK (*.asm;*.h,*.s)" );

			ImportOpenFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin|Palette (1536 bytes) (*.pal)|*.pal";
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false;
#endif

#if DEF_FIXED_LEN_PALETTE16_ARR
			BtnPltDelete.Enabled = false;
#endif
			if( platform_data.get_CHR_bank_pages_cnt() == 1 )
			{
				CHRBankPageBtnsToolStripSeparator.Visible = BtnCHRBankNextPage.Visible = BtnCHRBankPrevPage.Visible = false;
				prevPageToolStripMenuItem.Visible = nextPageToolStripMenuItem.Visible = false;
			}
			
			CheckBoxScreensAutoUpdate.Checked = true;

			CheckBoxShowMarks.Checked		= MAPeD.Properties.Settings.Default.layout_show_marks;
			CheckBoxShowEntities.Checked 	= MAPeD.Properties.Settings.Default.layout_show_entities;
			CheckBoxShowTargets.Checked 	= MAPeD.Properties.Settings.Default.layout_show_targets;
			CheckBoxShowCoords.Checked		= MAPeD.Properties.Settings.Default.layout_show_coords;
			CheckBoxShowGrid.Checked		= MAPeD.Properties.Settings.Default.layout_show_grid;
			
			reset();
			
			if( _args.Length > 0 )
			{
				project_load( _args[0] );
			}
			else
			{
				set_status_msg( "Set up screen dimensions and click the \"Bank+\" button to add a new CHR bank to begin! <F1> - Quick Guide" );
			}
		}

		private string get_all_projects_open_file_filter( platform_data.e_platform_type _type )
		{
			string filter_str = "";
			string platform_file_ext;
			
			int ind;
			int i 		= ( int )_type;
			int size 	= platform_data.get_platforms_cnt() + i;

			for( ; i < size; i++ )
			{
				ind = i % platform_data.get_platforms_cnt();
				
				platform_file_ext = platform_data.get_platform_file_ext( ( platform_data.e_platform_type )ind );
				
				filter_str += utils.CONST_FULL_APP_NAMES_ARR[ ind ] + " (*." + platform_file_ext + ")|*." + platform_file_ext;
				
				if( i != size - 1 )
				{
					filter_str += "|";
				}
			}
			
			return filter_str;
		}

		private void reset()
		{
			m_disable_status_wnd = true;
			
			if( tabControlTilesLayout.Contains( TabTiles ) )
			{
				tabControlTilesLayout.SelectTab( TabTiles );
			}
			
			if( tabControlLayoutTools.Contains( TabBuilder ) )
			{
				tabControlLayoutTools.SelectTab( TabBuilder );
			}
			
			m_data_manager.reset();
			fill_entity_data( null );
			
			m_tile_list_manager.select( tile_list.e_data_type.Tiles, -1 );
			m_tile_list_manager.select( tile_list.e_data_type.Blocks, -1 );
			
			entity_instance.reset_instances_counter();
			
			CBoxTileViewType.SelectedIndex = ( int )utils.e_tile_view_type.Graphics;
			
			CheckBoxEntitySnapping.Checked 	= true;
			RBtnMapScaleX1.Checked			= true;
			
			CheckBoxPalettePerCHR.Checked	= false;
			
			LayoutContextMenuEntityItemsEnable( false );
			
#if DEF_NES || DEF_SMS || DEF_ZX || DEF_PCE
			set_screen_data_type( data_sets_manager.e_screen_data_type.Tiles4x4 );
#else
			set_screen_data_type( data_sets_manager.e_screen_data_type.Blocks2x2 );
#endif
			NumericUpDownScrBlocksWidth.Maximum		= ( decimal )platform_data.get_screen_blocks_width( true );
			NumericUpDownScrBlocksHeight.Maximum	= ( decimal )platform_data.get_screen_blocks_height( true );

			NumericUpDownScrBlocksWidth.Value	= ( decimal )platform_data.get_screen_blocks_width();
			NumericUpDownScrBlocksHeight.Value	= ( decimal )platform_data.get_screen_blocks_height();

			update_screen_size_label();

			enable_main_UI( false );
			
			CheckBoxTileEditorLock.Checked = true;

			m_tiles_palette_form.reset();
			
			update_graphics( false, true, false );
			
			enable_update_gfx_btn( false );
			mark_update_screens_btn( false );

			clear_active_tile_img();
			
			CBoxCHRBanks.Items.Clear();
			CBoxPalettes.Items.Clear();
			ListBoxLayouts.Items.Clear();
			
			CBoxBlockObjId.SelectedIndex = 0;
			CBoxBlockObjId.Tag = null;
			
			m_entity_preview.reset_scale();
			m_pattern_preview.reset_scale();

			m_layout_editor.reset( false );
			m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data(), -1, m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
			
			enable_copy_paste_action( false, e_copy_paste_type.All );
			
			SelectCHRToolStripMenuItemClick( null, null );
			
			PropertyPerBlockToolStripMenuItemClick( null, null );
			
			builderToolStripMenuItem.Enabled	= 
			screensToolStripMenuItem.Enabled	= 
			entitiesToolStripMenuItem.Enabled	= 
			patternsToolStripMenuItem.Enabled	= false;
			
			set_title_name( null );
			
			m_description_form.edit_text = "";
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_palette_copy_ind = -1;
			BtnPltCopy.Text = "Copy";
#endif
			m_disable_status_wnd = false;
		}
		
		private void progress_bar_show( bool _on, string _operation = "", bool _show_progress_bar = true )
		{
			if( m_disable_status_wnd )
			{
				return;
			}
			
			if( _on )
			{
				m_progress_form.Left	= this.Left + ( this.Width >> 1 ) - ( m_progress_form.Width >> 1 );
				m_progress_form.Top		= this.Top + ( this.Height >> 1 ) - ( m_progress_form.Height >> 1 );

				if( _show_progress_bar )
				{
					this.Enabled = false;
				}
				
				m_progress_form.show_progress_bar	= _show_progress_bar;
				m_progress_form.operation_msg		= _operation;

				m_progress_form.Show( this );

				progress_bar_value( 0 );
				progress_bar_status( "Please wait..." );
				
				pause( 250 );
			}
			else
			{
				pause( 250 );
				
				m_progress_form.show_progress_bar = false;
				
				m_progress_form.Hide();
				
				if( !this.Enabled )
				{
					this.Enabled = true;
				}
			}
		}
		
		private void progress_bar_value( int _val )
		{
			if( m_disable_status_wnd )
			{
				return;
			}
			
			m_progress_form.progress_value = _val;
		}
		
		private void progress_bar_status( string _status )
		{
			if( m_disable_status_wnd )
			{
				return;
			}
			
			m_progress_form.status_msg = _status; 
			m_progress_form.Update();
		}
		
		private async void pause( int _msec )
		{
			await Task.Delay( _msec );
		}
		
		private void OnFormClosing( object sender, FormClosingEventArgs e )
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
		
		private void DescriptionToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_description_form.ShowDialog();
		}
		
		private void StatisticsToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_statistics_form.show_window();
		}
		
		private void ExitToolStripMenuItemClick( object sender, EventArgs e)
		{
			Close();
		}
		
		private void CloseToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?", "Close Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				progress_bar_show( true, "Closing project...", false );
				
				reset();
				
				set_status_msg( "Project closed" );
				
				progress_bar_show( false, "", false );
			}
		}

		private void ExportScriptEditorToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( !SPSeD.py_editor.is_active() )
			{
				m_py_editor = new SPSeD.py_editor( global::MAPeD.Properties.Resources.MAPeD_icon, new py_api( m_data_manager ), "MAPeD API Doc", System.Text.Encoding.Default.GetString( global::MAPeD.Properties.Resources.MAPeD_Data_Export_Python_API ), "MAPeD_Data_Export_Python_API.html" );
				m_py_editor.Show();
			}
			
			SPSeD.py_editor.set_focus();
		}
		
		private void AboutToolStripMenuItemClick( object sender, EventArgs e )
		{
			message_box( "Game Maps Editor (" + platform_data.CONST_PLATFORM + ")\n" + platform_data.CONST_PLATFORM_DESC + "\n\n" + utils.CONST_APP_VER + " " + utils.build_str + " pfv" + utils.CONST_PROJECT_FILE_VER + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		private void QuickGuideToolStripMenuItemClick( object sender, EventArgs e )
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
		
		private void set_title_name( string _msg )
		{
			Text = utils.CONST_FULL_APP_NAME + " " + utils.CONST_APP_VER + ( _msg != null ? " - " + _msg:"" );
		}
		
		public static void set_status_msg( string _msg )
		{
			UI_thread_safe_call( MainForm.form(), () => { m_status_bar_label.Text = _msg; } );
		}
		
		public static void UI_thread_safe_call( Control _cntrl, Action _act )
		{
			if( _cntrl.InvokeRequired )
			{
				_cntrl.Invoke( (MethodInvoker)( () => _act() ) );
			}
			else
			{
				_act();
			}
		}
		
		public static DialogResult message_box( string _msg, string _caption, MessageBoxButtons _buttons, MessageBoxIcon _icon = MessageBoxIcon.Warning )
		{
			return MessageBox.Show( MainForm.m_native_wnd, _msg, _caption, _buttons, _icon );
		}

		private void enable_main_UI( bool _on )
		{
			if( platform_data.get_CHR_bank_pages_cnt() > 1 )
			{
				BtnCHRBankNextPage.Enabled = BtnCHRBankPrevPage.Enabled = _on;
				prevPageToolStripMenuItem.Enabled = nextPageToolStripMenuItem.Enabled = _on;
			}
			
			PanelBlocks.Enabled = _on;
			
			if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				PanelTiles.Enabled	= _on;
			}
			
			m_tiles_palette_form.enable( _on );
			
			tabControlLayoutTools.Enabled = _on;
			
			if( _on )
			{
				builderToolStripMenuItem.Enabled = _on;
			}
			
			RBtnScreenDataTiles.Enabled = RBtnScreenDataBlocks.Enabled = _on;
			
			NumericUpDownScrBlocksWidth.Enabled = NumericUpDownScrBlocksHeight.Enabled = !_on;
		}

		private tiles_data get_curr_tiles_data()
		{
			return m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
		}
		
		public void OnKeyUp( object sender, KeyEventArgs e )
		{
			m_tiles_processor.on_key_up( sender, e );
			m_layout_editor.on_key_up( sender, e );
		}

		public void OnKeyDown( object sender, KeyEventArgs e )
		{
			m_layout_editor.on_key_down( sender, e );
		}
		
		private void TabCntrlDblClick( object sender, EventArgs e )
		{
			TabControl tab_cntrl = sender as TabControl;
			
			( new tab_page_container( tab_cntrl, this ) ).Show();
		}

		private void TabCntrlLayoutTilesChanged( object sender, EventArgs e )
		{
			TabPage curr_tab = ( sender as TabControl ).SelectedTab;
			
			if( curr_tab == TabLayout )
			{
				m_layout_editor.update();
			}
		}
	}
}