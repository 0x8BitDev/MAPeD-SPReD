/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
		
		private utils.ETileViewType m_view_type	= utils.ETileViewType.tvt_Unknown;

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
		
		private static Form m_form = null;
		
		private static Form form()
		{
			return m_form;
		}
		
		public MainForm( string[] _args )
		{
			m_form = this;
			
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
			
			m_tiles_processor.subscribe_block_quad_selected_event( BlockQuadSelected_Event );

			m_tile_list_manager = new tile_list_manager();
			
			m_imagelist_manager	= new imagelist_manager( PanelTiles, PanelTilesClick_Event, ContextMenuTilesList, PanelBlocks, PanelBlocksClick_Event, ContextMenuBlocksList, ListViewScreens, m_tile_list_manager );
			
			// layout editor init
			{
				m_layout_editor = new layout_editor_base( m_data_manager, PBoxLayout, LayoutLabel, m_imagelist_manager );
				m_layout_editor.subscribe_event( m_data_manager );
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Entities, layout_editor_param.CONST_SUBSCR_ENT_INST_SELECT, EntityInstanceSelected_Event );
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Entities, layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION, EditEntityCancel_Event );
				
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Screens, layout_editor_param.CONST_SUBSCR_SCR_RESET_SELECTED, ResetSelectedScreen_Event );
				
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SUBSCR_PNT_UPDATE_TILE_IMAGE, update_tile_image );
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION, ActiveTileCancel_Event );
				
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SUBSCR_PTTRN_EXTRACT_END, PatternExtractEnd_Event );
				m_layout_editor.subscribe( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION, PatternPlaceCancel_Event );
				
				m_layout_editor.set_param( layout_editor_param.CONST_SET_BASE_SUBSCR_DATA_MNGR, m_data_manager );
				
				m_layout_editor.MapScaleX1 += new EventHandler( MapScaleX1_Event );
				m_layout_editor.MapScaleX2 += new EventHandler( MapScaleX2_Event );
				
				// set the builder mode by default
				m_layout_editor.mode = layout_editor_base.EMode.em_Builder;
				
				m_tile_preview = new image_preview( PBoxActiveTile, false );

				layout_screen_data.EntityAdded		+= new EventHandler( EntitiesCounterUpdate_Event );
				layout_screen_data.EntityRemoved	+= new EventHandler( EntitiesCounterUpdate_Event );
			}
			
			// patterns manager
			{
				m_pattern_preview = new image_preview( PBoxPatternPreview, true );
				
				PBoxPatternPreview.MouseEnter += new EventHandler( PatternsMngr_MouseEnter );
				
				// create graphics for drawing patterns
				int scr_tile_size = utils.CONST_SCREEN_TILES_SIZE >> 1;
				
				m_pattern_image	= new Bitmap( scr_tile_size * platform_data.get_screen_tiles_width(), scr_tile_size * platform_data.get_screen_tiles_height(), PixelFormat.Format32bppPArgb );
				m_pattern_gfx	= Graphics.FromImage( m_pattern_image );
			}
			
			m_data_manager.SetEntitiesData += new EventHandler( TreeViewEntities_update_data );
			m_data_manager.AddEntity 	+= TreeViewEntities_add_entity;
			m_data_manager.DeleteEntity += TreeViewEntities_delete_entity;
			m_data_manager.AddGroup		+= TreeViewEntities_add_group;
			m_data_manager.DeleteGroup 	+= TreeViewEntities_delete_group;
			
			m_entity_preview = new image_preview( PBoxEntityPreview, true );

			PBoxEntityPreview.MouseEnter += new EventHandler( EntitiesMngr_MouseEnter );
			
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
			m_tiles_palette_form.TilesBlocksClosed 	+= new EventHandler( TilesBlocksClosed_Event );
			m_tiles_palette_form.TileSelected 		+= new EventHandler( TileSelected_Event );
			m_tiles_palette_form.BlockSelected 		+= new EventHandler( BlockSelected_Event );
			m_tiles_palette_form.ResetActiveTile 	+= new EventHandler( BtnResetTileClick_Event );
			
			m_optimization_form = new optimization_form( m_data_manager, progress_bar_show );
			m_optimization_form.UpdateGraphics += new EventHandler( UpdateGraphicsAfterOptimization_Event );
			
#if DEF_PALETTE16_PER_CHR
			m_tiles_processor.UpdatePaletteListPos	+= new EventHandler( update_palette_list_pos );
#endif
			m_tiles_processor.NeedGFXUpdate 	+= new EventHandler( enable_update_gfx_btn_Event );
			
			TabTiles.Tag 		= new Point( TabTiles.Width,	TabTiles.Height		);
			TabLayout.Tag 		= new Point( TabLayout.Width,	TabLayout.Height	);
			TabTiles.Tag 		= new Point( TabLayout.Width,	TabLayout.Height	);
			
			CBoxPalettes.DrawItem += new DrawItemEventHandler( CBoxPalettesDrawItem_Event );

			FormClosing += new FormClosingEventHandler( OnFormClosing );

			// setup tooltips
			{
				SToolTipData[] tooltips = new SToolTipData[]{ 	new SToolTipData( tabControlTilesLayout, "Double Click to detach the tab page" ),
																new SToolTipData( CheckBoxScreensAutoUpdate, "Automatically updates the screen list when \"Update GFX\" button is pressed" ),
																new SToolTipData( CheckBoxLayoutEditorAllBanks, "Show screens of all CHR banks" ),
																new SToolTipData( CheckBoxSelectTargetEntity, "Select target entity" ),
																new SToolTipData( BtnCopyCHRBank, "Add copy of active CHR bank" ),
																new SToolTipData( BtnAddCHRBank, "Add new CHR bank" ),
																new SToolTipData( BtnDeleteCHRBank, "Delete active CHR Bank" ),
																new SToolTipData( BtnCHRBankPrevPage, "Previous CHR Bank's page" ),
																new SToolTipData( BtnCHRBankNextPage, "Next CHR Bank's page" ),
																new SToolTipData( BtnUpdateGFX, "Update tiles\\blocks and screens ( if auto update is enabled )" ),
																new SToolTipData( BtnOptimization, "Delete unused screens\\tiles\\blocks\\CHRs" ),
																new SToolTipData( CheckBoxTileEditorLock, "Enable\\disable tile editing" ),
																new SToolTipData( BtnTilesBlocks, "Arrays of tiles and blocks for a map building" ),
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
																new SToolTipData( CheckBoxShowGrid, "Show tile grid" ),
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
																new SToolTipData( BtnLayoutDeleteEmptyScreens, "Delete all one-block filled screens in active layout" ),
																new SToolTipData( BtnCreateLayoutWxH, "Create layout (width x height) filled with empty screens" ),
																new SToolTipData( BtnLayoutMoveUp, "Move selected layout up" ),
																new SToolTipData( BtnLayoutMoveDown, "Move selected layout down" ),
																new SToolTipData( CBoxBlockObjId, "Assign property to selected block or CHR" ),
																new SToolTipData( LabelObjId, "Assign property to selected block or CHR" ),
																new SToolTipData( LabelEntityProperty, "Entity properties are inherited by all instances" ),
																new SToolTipData( LabelEntityInstanceProperty, "Instance properties are unique for all instances" ),
																new SToolTipData( CheckBoxEntitySnapping, "Snap an entity to 8x8 grid" ),
																new SToolTipData( BtnPainterFillWithTile, "Fill selected screens with active tile" ),
																new SToolTipData( CheckBoxPainterReplaceTiles, "Replace a group of identical tiles on selected screens" ),
															};
				SToolTipData data;
				
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
			Project_openFileDialog.DefaultExt = platform_data.CONST_SMS_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( platform_data.EPlatformType.pt_NES );
			
			BtnSwapColors.Visible = false;
#elif DEF_SMS
			Project_saveFileDialog.DefaultExt = platform_data.CONST_SMS_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "SMS" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "sms" );

			Project_openFileDialog.DefaultExt = platform_data.CONST_SMS_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( platform_data.EPlatformType.pt_SMS );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Replace( "CA65\\NESasm", "WLA-DX" );

			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "NES", "SMS" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "nes", "sms" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "Map 2/4 bpp", "Map 2/4/8 bpp" );
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false; 
#elif DEF_PCE
			Project_saveFileDialog.DefaultExt = platform_data.CONST_PCE_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "PCE" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "pce" );

			Project_openFileDialog.DefaultExt = platform_data.CONST_PCE_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( platform_data.EPlatformType.pt_PCE );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Replace( "CA65\\NESasm (*.asm)", "CA65\\PCEAS\\HuC (*.asm;*.h)" );

			Import_openFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin|Palette (1536 bytes) (*.pal)|*.pal";
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false;
#elif DEF_ZX
			Project_saveFileDialog.DefaultExt = platform_data.CONST_ZX_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "ZX" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "zx" );

			Project_openFileDialog.DefaultExt = platform_data.CONST_ZX_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( platform_data.EPlatformType.pt_ZX );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Substring( Project_exportFileDialog.Filter.IndexOf( "Z" ) );
			Project_exportFileDialog.DefaultExt = "zxa";

			Import_openFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin|Palette (48 bytes) (*.pal)|*.pal";
			
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
			Project_saveFileDialog.DefaultExt = platform_data.CONST_SMD_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "SMD" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "smd" );

			Project_openFileDialog.DefaultExt = platform_data.CONST_SMD_FILE_EXT;
			Project_openFileDialog.Filter = get_all_projects_open_file_filter( platform_data.EPlatformType.pt_SMD );
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Replace( "CA65\\NESasm (*.asm)", "vasm\\SGDK (*.asm;*.h,*.s)" );

			Import_openFileDialog.Filter = "Tiles/Game Map 4/8 bpp (*.bmp)|*.bmp|Raw CHR Data (*.chr,*.bin)|*.chr;*.bin|Palette (1536 bytes) (*.pal)|*.pal";
			
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

		private string get_all_projects_open_file_filter( platform_data.EPlatformType _type )
		{
			string filter_str = "";
			string platform_file_ext;
			
			int ind;
			int i 		= ( int )_type;
			int size 	= platform_data.get_platforms_cnt() + i;

			for( ; i < size; i++ )
			{
				ind = i % platform_data.get_platforms_cnt();
				
				platform_file_ext = platform_data.get_platform_file_ext( ( platform_data.EPlatformType )ind );
				
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
		
		async private void pause( int _msec )
		{
			await Task.Delay( _msec );
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
			return MessageBox.Show( MainForm.form(), _msg, _caption, _buttons, _icon );
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
			
			mark_update_screens_btn( true );
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
			if( platform_data.get_CHR_bank_pages_cnt() > 1 )
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
			
			tabControlLayoutTools.Enabled = _on;
			
			if( _on )
			{
				builderToolStripMenuItem.Enabled = _on;
			}
			
			RBtnScreenDataTiles.Enabled = RBtnScreenDataBlocks.Enabled = _on;
			
			NumericUpDownScrBlocksWidth.Enabled = NumericUpDownScrBlocksHeight.Enabled = !_on;
		}

		private void update_screen_size( int _blocks_width = -1, int _blocks_height = -1 )
		{
			if( _blocks_width >= 0 && _blocks_height >= 0 )
			{
				NumericUpDownScrBlocksWidth.Value	= Math.Min( _blocks_width, NumericUpDownScrBlocksWidth.Maximum );
				NumericUpDownScrBlocksHeight.Value	= Math.Min( _blocks_height, NumericUpDownScrBlocksHeight.Maximum );
			}
			
			platform_data.set_screen_blocks_size( ( int )NumericUpDownScrBlocksWidth.Value, ( int )NumericUpDownScrBlocksHeight.Value );
			m_imagelist_manager.update_screen_image_size();
		}
		
		void NumericUpDownScrBlocksChanged_Event(object sender, EventArgs e)
		{
			 update_screen_size_label();
		}
		
		void update_screen_size_label()
		{
			LabelScreenResolution.Text = "[" + ( ( int )NumericUpDownScrBlocksWidth.Value << 4 ).ToString() + "x" + ( ( int )NumericUpDownScrBlocksHeight.Value << 4 ).ToString() + "]";
		}
		
		tiles_data get_curr_tiles_data()
		{
			return m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
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
			
			m_tile_list_manager.select( tile_list.EType.t_Tiles, -1 );
			m_tile_list_manager.select( tile_list.EType.t_Blocks, -1 );
			
			entity_instance.reset_instances_counter();
			
			CBoxTileViewType.SelectedIndex = ( int )utils.ETileViewType.tvt_Graphics;
			
			CheckBoxEntitySnapping.Checked 	= true;
			RBtnMapScaleX1.Checked			= true;
			
			CheckBoxPalettePerCHR.Checked	= false;
			
#if DEF_NES || DEF_SMS || DEF_ZX || DEF_PCE
			set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
#else
			set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
#endif
			NumericUpDownScrBlocksWidth.Maximum		= ( decimal )platform_data.get_screen_blocks_width( true );
			NumericUpDownScrBlocksHeight.Maximum	= ( decimal )platform_data.get_screen_blocks_height( true );

			NumericUpDownScrBlocksWidth.Value	= ( decimal )platform_data.get_screen_blocks_width();
			NumericUpDownScrBlocksHeight.Value	= ( decimal )platform_data.get_screen_blocks_height();

			update_screen_size_label();

			enable_main_UI( false );
			
			TilesLockEditorToolStripMenuItem.Checked = CheckBoxTileEditorLock.Checked = true;

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
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_All );
			
			SelectCHRToolStripMenuItemClick_Event( null, null );
			
			PropertyPerBlockToolStripMenuItemClick_Event( null, null );
			
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
		
		void ExitToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			Close();
		}
		
		void CloseToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Close Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				progress_bar_show( true, "Closing project...", false );
				
				reset();
				
				set_status_msg( "Project closed" );
				
				progress_bar_show( false, "", false );
			}
		}

		void CBoxTileViewTypeChanged_Event(object sender, EventArgs e)
		{
			m_view_type = ( utils.ETileViewType )CBoxTileViewType.SelectedIndex;
			
			mark_update_screens_btn( true );
#if DEF_ZX
			m_tiles_processor.set_view_type( m_view_type );
#endif
			update_graphics( false, true, true );
			
			clear_active_tile_img();

			set_status_msg( "View type changed" );
		}

		void TreeViewMouseDown_Event(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Right )
			{
				TreeView tree_view = sender as TreeView;
				
				if( tree_view != null )
				{
					tree_view.SelectedNode = tree_view.GetNodeAt( e.X, e.Y );
				}
			}
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
				project_data_desc prj_data = new project_data_desc();
				
				// confirm the current screen size
				update_screen_size();

				prj_data.m_file_ext						= Path.GetExtension( _filename ).Substring( 1 );
				prj_data.m_use_file_screen_resolution	= platform_data.get_platform_type() == platform_data.get_platform_type_by_file_ext( prj_data.m_file_ext );

				int load_scr_data_len 	= platform_data.get_screen_tiles_cnt( prj_data.m_file_ext, true );
				int scr_data_len 		= platform_data.get_screen_tiles_cnt();
				
				progress_bar_show( true, "Loading project..." );

				reset();
				
				fs = new FileStream( _filename, FileMode.Open, FileAccess.Read );
				{
					br = new BinaryReader( fs );
					if( br.ReadUInt32() == utils.CONST_PROJECT_FILE_MAGIC )
					{
						prj_data.m_ver = br.ReadByte();
						
						if( prj_data.m_ver <= utils.CONST_PROJECT_FILE_VER )
						{
							if( prj_data.m_ver >= 4 )
							{
								uint pre_flags = br.ReadUInt32();
								
								prj_data.m_scr_data_tiles4x4 = ( ( pre_flags & utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4 ) == utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4 );
								
								set_screen_data_type( prj_data.m_scr_data_tiles4x4 ? data_sets_manager.EScreenDataType.sdt_Tiles4x4:data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
								
								if( ( pre_flags & utils.CONST_IO_DATA_PRE_FLAG_SCR_BLOCKS_WH ) == utils.CONST_IO_DATA_PRE_FLAG_SCR_BLOCKS_WH )
								{
									prj_data.m_scr_blocks_width		= br.ReadByte();
									prj_data.m_scr_blocks_height	= br.ReadByte();
									
									load_scr_data_len = ( ( prj_data.m_scr_blocks_width + 1 ) >> 1 ) * ( ( prj_data.m_scr_blocks_height + 1 ) >> 1 );
								}
							}
							else
							{
								// early versions always work in the Tiles4x4 mode
								set_screen_data_type( data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
							}

							// check screen resolutions
							{
								if( load_scr_data_len != scr_data_len )
								{
									if( m_data_conversion_options_form.ShowDialog( prj_data ) == DialogResult.Cancel )
									{
										reset();
										return;
									}
									
									prj_data.m_scr_align					= m_data_conversion_options_form.screens_align_mode;
									prj_data.m_convert_colors				= m_data_conversion_options_form.convert_colors;
									prj_data.m_use_file_screen_resolution	= m_data_conversion_options_form.use_file_screen_resolution;
								}
								
								if( prj_data.m_use_file_screen_resolution && ( ( prj_data.m_scr_blocks_width != 0xff ) && ( prj_data.m_scr_blocks_height != 0xff ) ) )
								{
									update_screen_size( prj_data.m_scr_blocks_width, prj_data.m_scr_blocks_height );
								}
							}
							
							m_data_manager.tiles_data_pos = await Task.Run( () => m_data_manager.load( br, prj_data, m_progress_val, m_progress_status ) );
							
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
							throw new Exception( "Invalid file version (" + prj_data.m_ver + ")!" );
						}
					}
					else
					{
						throw new Exception( "Invalid file!" );
					}
					
					// update data
					{
						progress_bar_status( "Updating data..." );

						// update tiles and screens
						{
							tiles_data data = get_curr_tiles_data();
							
							m_imagelist_manager.update_blocks( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );
							m_imagelist_manager.update_tiles( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );	// called after update_blocks, because it uses updated blocks gfx to speed up drawing
							
							m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data(), m_data_manager.tiles_data_pos, m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
						}
						
						int tiles_cnt = m_data_manager.tiles_data_cnt;
						
						for( int i = 0; i < tiles_cnt; i++ )
						{
							CBoxCHRBanks.Items.Add( m_data_manager.get_tiles_data( i ) );
						}

						CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;
						
						enable_main_UI( true );
						
						update_layouts_list_box();
					
						m_layout_editor.update();
					}
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
				progress_bar_show( true, "Saving project...", false );
				
				fs = new FileStream( filename, FileMode.Create, FileAccess.Write );
				{
					bw = new BinaryWriter( fs );
					bw.Write( utils.CONST_PROJECT_FILE_MAGIC );
					bw.Write( utils.CONST_PROJECT_FILE_VER );

					uint pre_flags = ( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ) ? utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4:0;
					pre_flags |= utils.CONST_IO_DATA_PRE_FLAG_SCR_BLOCKS_WH;
					
					bw.Write( pre_flags );

					bw.Write( ( byte )platform_data.get_screen_blocks_width() );
					bw.Write( ( byte )platform_data.get_screen_blocks_height() );
					
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
										// switch to the builder tab
										if( tabControlLayoutTools.Contains( TabBuilder ) )
										{
											tabControlLayoutTools.SelectTab( TabBuilder );
										}
										
										progress_bar_show( true, "Importing image data..." );
										
										// needed to properly remove screens of invalid layout
										m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_cnt - 1;										
										
										await Task.Run( () => m_import_tiles_form.data_processing( bmp, m_data_manager, create_layout_with_empty_screens_beg, m_progress_val, m_progress_status ) );
										
										if( m_import_tiles_form.import_game_map )
										{
											progress_bar_status( "Initializing layout..." );
											
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
												progress_bar_status( "Deleting empty screens..." );
												
												delete_empty_screens();
											}
											
											progress_bar_status( "Initializing screens data..." );
											
											update_graphics( true, false, false );

											update_screens( true, false );
											
											m_layout_editor.update_dimension_changes();
										}
										else
										{
											progress_bar_status( "Updating data..." );

											update_graphics( true, true, false );
										}
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
								
								mark_update_screens_btn( true );
								
								// update the tiles processor data and screens data too
								// you could import a palette into a current active map
								update_graphics( true, true, true );
							}
							break;
#endif //DEF_NES || DEF_SMS
						case ".chr":
						case ".bin":
							{
								if( br.BaseStream.Length < platform_data.get_native_CHR_size_bytes() )
								{
									throw new Exception( "Invalid file!" );
								}
						
								int CHR_banks	= 1;
								int added_CHRs	= 0;
								
								while( true )
								{
									added_CHRs += project_data_converter_provider.get_converter().merge_CHR_bin( br, get_curr_tiles_data() );

									if( br.BaseStream.Position + platform_data.get_native_CHR_size_bytes() <= br.BaseStream.Length )
									{
										if( add_CHR_bank() == false )
										{
											throw new Exception( "Operation aborted!" );
										}
										
										++CHR_banks;
									}
									else
									{
										break;
									}
								}

								if( added_CHRs > 0 )
								{
									set_status_msg( string.Format( "Merged: {0} CHRs / {1} CHR bank(s)", added_CHRs, CHR_banks ) );
								}
								
								// there is no need to update screens
								// so update the tiles processor data only
								update_graphics( true, false, false );
							}
							break;
							
						case ".pal":
							{
								int plt_len_bytes = palette_group.Instance.main_palette.Length * 3;
								
								if( br.BaseStream.Length == plt_len_bytes )
								{
									palette_group plt_grp = palette_group.Instance;
									
									plt_grp.load_main_palette( br );
									
									// update selected palette color
									plt_grp.active_palette = 0;
									
									progress_bar_show( true, "Updating graphics...", false );
									{
										// update the tiles processor only
										update_graphics( true, false, false );
										// and then update all the available screens
										update_all_screens( true );
									}
									progress_bar_show( false );
									
									clear_active_tile_img();
									
									set_status_msg( "Palette imported" );
								}
								else
								{
									throw new Exception( "The imported palette must be " + plt_len_bytes + " bytes length!" );
								}
							}
							break;
					}
				}
			}
			catch( Exception _err )
			{
				set_status_msg( "Data import error" );
				
				message_box( _err.Message, "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				if( bmp != null )
				{
					bmp.Dispose();
					
					// there was an image import, so we need to destroy an invalid layout
					layout_data layout = m_import_tiles_form.level_layout;
					
					if( layout != null )
					{
						progress_bar_status( "Deleting invalid layout..." );
						
						delete_last_layout_and_screens();
					}
				}
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
								// update screens data if some changes has been made in the tiles editor
								update_graphics( false, BtnUpdateGFX.Enabled, false );
								
								m_export_active_tile_block_set_form.export( filename, get_curr_tiles_data(), m_imagelist_manager.get_tiles_image_list(), m_imagelist_manager.get_blocks_image_list() );
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
								int scr_width_pix	= platform_data.get_screen_width_pixels();
								int scr_height_pix	= platform_data.get_screen_height_pixels();
								
								int scr_ind;
								
								// draw images into bitmap
								Bitmap bmp = new Bitmap( layout_width * scr_width_pix, layout_height * scr_height_pix );

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
											gfx.DrawImage( m_imagelist_manager.get_screen_list().get( scr_ind ), j * scr_width_pix, i * scr_height_pix, scr_width_pix, scr_height_pix );
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
				
				set_status_msg( "Data exported" );
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
			message_box( "Game Maps Editor (" + platform_data.CONST_PLATFORM + ")\n" + platform_data.CONST_PLATFORM_DESC + "\n\n" + utils.CONST_APP_VER + " " + utils.build_str + " pfv" + utils.CONST_PROJECT_FILE_VER + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", MessageBoxButtons.OK, MessageBoxIcon.Information );
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
			m_layout_editor.key_up_event( sender, e );
		}

		public void KeyDown_Event(object sender, KeyEventArgs e)
		{
			m_layout_editor.key_down_event( sender, e );
		}
		
		void TabCntrlDblClick_Event(object sender, EventArgs e)
		{
			TabControl tab_cntrl = sender as TabControl;
			
			( new tab_page_container( tab_cntrl, this ) ).Show();
		}
#endregion		
// TILES EDITOR **************************************************************************************//
#region tiles editor
		private void enable_update_gfx_btn( bool _on )
		{
			if( _on && ( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 ) )
			{
				update_selected_block();
			}
			
			BtnUpdateGFX.Enabled = _on;
		
			BtnUpdateGFX.UseVisualStyleBackColor = !_on;
		}
		
		void CHRBankChanged_Event(object sender, EventArgs e)
		{
			ComboBox chr_bank_cbox = sender as ComboBox;
			
			// force update of OLD screens if needed
			update_screens_if_needed();

			m_data_manager.tiles_data_pos 	= chr_bank_cbox.SelectedIndex;
			
			palette_group.Instance.active_palette = 0;
			
			// update palettes
			{
				update_palettes_arr( get_curr_tiles_data(), false );
				
				CBoxPalettes.Tag = get_curr_tiles_data().palettes_arr;
				CBoxPalettes.SelectedIndex = 0;
			}

			patterns_manager_update_data();
			
			update_graphics( false, false, false );
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_All );
			
			clear_active_tile_img();
			
			update_screens_by_bank_id( true, false );
		}

		void BtnUpdateGFXClick_Event(object sender, EventArgs e)
		{
			update_graphics( false, true, true );
			
			clear_active_tile_img();
			
			set_status_msg( "Graphics updated" );
		}
		
		private void update_graphics( bool _update_tile_processor_gfx, bool _update_screens, bool _show_status_wnd )
		{
			if( _show_status_wnd )
			{
				progress_bar_show( true, "Updating graphics...", false );
			}
			
			tiles_data data = get_curr_tiles_data();
			
			m_imagelist_manager.update_blocks( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );
			m_imagelist_manager.update_tiles( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );	// called after update_blocks, because it uses updated blocks gfx to speed up drawing
			
			m_tile_list_manager.update_all();

			if( CheckBoxScreensAutoUpdate.Checked && _update_screens )
			{
				update_screens( true );
			}
			else
			{
				m_layout_editor.update();
			}
			
			if( _update_tile_processor_gfx )
			{
				m_tiles_processor.update_graphics();
			}
			
			m_tiles_palette_form.update();
			patterns_manager_update_preview();
			
			enable_update_gfx_btn( false );
			
			if( _show_status_wnd )
			{
				progress_bar_show( false );
			}
		}
		
		void BtnCHRBankNextPageClick_Event(object sender, EventArgs e)
		{
			if( platform_data.get_CHR_bank_pages_cnt() > 1 )
			{
				m_tiles_processor.CHR_bank_next_page();
			}
		}
		
		void BtnCHRBankPrevPageClick_Event(object sender, EventArgs e)
		{
			if( platform_data.get_CHR_bank_pages_cnt() > 1 )
			{
				m_tiles_processor.CHR_bank_prev_page();
			}
		}
		
		void BtnCopyCHRBankClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( true, "Copying CHR bank...", false );
				}
				
				if( m_data_manager.tiles_data_copy() == true )
				{
					int prev_tiles_data_pos	= m_data_manager.tiles_data_pos;
					
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_cnt - 1 );
					
					CBoxCHRBanks.Items.Add( data );
					CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_cnt - 1;

					// copy screens of the previous CHR bank 
					// to the end of the screen images list
					m_imagelist_manager.copy_screens_to_the_end( m_data_manager.get_tiles_data(), prev_tiles_data_pos );
					
					set_status_msg( "CHR bank copied" );
				}
				else
				{
					set_status_msg( "Failed to copy CHR bank" );
					
					message_box( "Can't copy CHR bank!\nThe maximum allowed number of CHR banks - " + utils.CONST_CHR_BANK_MAX_CNT, "Failed to Copy CHR Bank", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( false, "", false );
				}
			}
		}
		
		void BtnAddCHRBankClick_Event(object sender, EventArgs e)
		{
			add_CHR_bank();
		}
		
		bool add_CHR_bank()
		{
			if( platform_data.get_max_blocks_cnt() > 256 )
			{
				progress_bar_show( true, "Initializing CHR bank...", false );
			}
			
			if( m_data_manager.tiles_data_create() )
			{
				enable_main_UI( true );
				update_screen_size();
				
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_cnt - 1 );

				CBoxCHRBanks.Items.Add( data );
				CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_cnt - 1;
	
				palette_group.Instance.active_palette = 0;
	
				enable_copy_paste_action( false, ECopyPasteType.cpt_All );
				
				set_status_msg( "Added CHR bank" );
			}
			else
			{
				set_status_msg( "Failed to create CHR bank" );
				
				message_box( "Can't create CHR bank!\nThe maximum allowed number of CHR banks - " + utils.CONST_CHR_BANK_MAX_CNT, "Failed to Create CHR Bank", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			if( platform_data.get_max_blocks_cnt() > 256 )
			{
				progress_bar_show( false, "", false );
			}
			
			return true;
		}
		
		void BtnDeleteCHRBankClick_Event(object sender, EventArgs e)
		{
			if( CBoxCHRBanks.Items.Count > 0 && message_box( "Are you sure?", "Remove CHR Bank", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( true, "Deleting CHR bank...", false );
				}
				
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
							
							m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
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
				
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( false, "", false );
				}
			}
		}
		
		void BtnReorderCHRBanksClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				m_reorder_CHR_banks_form.show_dialog();
				
				if( ( CBoxCHRBanks.SelectedIndex != m_reorder_CHR_banks_form.selected_CHR_bank ) || m_reorder_CHR_banks_form.data_changed )
				{
					progress_bar_show( true, "Updating data...", false );

					CBoxCHRBanks.SelectedIndex = -1;	// this guarantees switching between different banks with the same index
					CBoxCHRBanks.SelectedIndex = m_reorder_CHR_banks_form.selected_CHR_bank;
					
					if( m_reorder_CHR_banks_form.data_changed )
					{
						bool all_banks_flag_state = CheckBoxLayoutEditorAllBanks.Checked;
						
						CheckBoxLayoutEditorAllBanks.Checked = true;
						
						update_all_screens( true, true );
						
						CheckBoxLayoutEditorAllBanks.Checked = all_banks_flag_state;
					}
					
					progress_bar_show( false );
				}
			}
			else
			{
				message_box( "No data!", "Reorder CHR Banks", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void PanelBlocksClick_Event(object sender, EventArgs e)
		{
			select_block( get_sender_index( sender ), true, true );
		}
		
		void select_block( int _id, bool _send_event, bool _update_active_tile )
		{
			if( _send_event && _id >= 0 )
			{
				m_tiles_processor.block_select_event( _id, get_curr_tiles_data() );
			}

			// update an object index
			CBoxBlockObjId.Tag = int.MaxValue;	// don't enable 'Update GFX' button
			CBoxBlockObjId.SelectedIndex = m_tiles_processor.get_block_flags_obj_id();
			CBoxBlockObjId.Tag = null;
			
			if( _update_active_tile )
			{
				update_active_block_img( _id );
			}
			
			m_tile_list_manager.select( tile_list.EType.t_Blocks, _id );
		}
		
		void PanelTilesClick_Event(object sender, EventArgs e)
		{
			select_tile( get_sender_index( sender ) );
		}
		
		void select_tile( int _id )
		{
			if( _id >= 0 )
			{
				m_tiles_processor.tile_select_event( _id, get_curr_tiles_data() );
			
				update_active_tile_img( _id );
			}
			
			m_tile_list_manager.select( tile_list.EType.t_Tiles, _id );
		}

		private void update_selected_block()
		{
			// update selected block image
			int sel_block_id = m_tiles_processor.get_selected_block();
			
			if( sel_block_id >= 0 )
			{
				m_imagelist_manager.update_block( sel_block_id, m_view_type, get_curr_tiles_data(), PropertyPerBlockToolStripMenuItem.Checked, null, null, m_data_manager.screen_data_type );
				m_tile_list_manager.update_tile( tile_list.EType.t_Blocks, sel_block_id );
				
				update_active_block_img( sel_block_id );
				patterns_manager_update_preview();
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
				mark_update_screens_btn( true );
				
				update_graphics( false, true, true );
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
				tiles_data data = get_curr_tiles_data();

				int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
				
				if( sel_ind >= 0 )
				{
					for( int i = platform_data.get_CHR_bank_max_sprites_cnt() - 1; i > sel_ind; i-- )
					{
						data.from_CHR_bank_to_spr8x8( i - 1, utils.tmp_spr8x8_buff );
						data.from_spr8x8_to_CHR_bank( i, utils.tmp_spr8x8_buff );
					}
					
					Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
					data.from_spr8x8_to_CHR_bank( sel_ind, utils.tmp_spr8x8_buff );
					
					data.inc_blocks_CHRs( sel_ind );
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
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
				tiles_data data = get_curr_tiles_data();

				int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
				
				if( sel_ind >= 0 )
				{
					if( message_box( "Are you sure?", "Delete CHR", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						for( int i = sel_ind; i < platform_data.get_CHR_bank_max_sprites_cnt() - 1; i++ )
						{
							data.from_CHR_bank_to_spr8x8( i + 1, utils.tmp_spr8x8_buff );
							data.from_spr8x8_to_CHR_bank( i, utils.tmp_spr8x8_buff );
						}
						
						Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
						data.from_spr8x8_to_CHR_bank( platform_data.get_CHR_bank_max_sprites_cnt() - 1, utils.tmp_spr8x8_buff );
						
						data.dec_blocks_CHRs( sel_ind );
						
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
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
			
			return ( cntrl.Tag as tile_list ).cursor_tile_ind();
		}

		int get_sender_index( object sender )
		{
			return ( sender as tile_list ).cursor_tile_ind();
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
				tiles_data data = get_curr_tiles_data();
				
				uint 	block_val;
				int 	chr_ind;
				int 	free_chr_ind 	= -1;
				
				if( _paste_clone )
				{
					free_chr_ind = data.get_first_free_spr8x8_id( false );
	
					if( free_chr_ind + utils.CONST_BLOCK_SIZE > platform_data.get_CHR_bank_max_sprites_cnt() )
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

				mark_update_screens_btn( true );
				
				// optimized update_graphics
				progress_bar_show( true, "Updating graphics...", false );
				{
					m_tile_list_manager.copy_tile( tile_list.EType.t_Blocks, m_block_copy_item_ind, _sel_ind );
					
					if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						m_imagelist_manager.update_tiles( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );
						m_tile_list_manager.update_tiles( tile_list.EType.t_Tiles );
					}
					
					update_active_block_img( _sel_ind );
					patterns_manager_update_preview();
					m_tiles_processor.update_graphics();
					
					if( CheckBoxScreensAutoUpdate.Checked )
					{
						update_screens( true );
					}
				}
				progress_bar_show( false );
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
				tiles_data data = get_curr_tiles_data();
				
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
				
				clear_active_tile_img();
				
				mark_update_screens_btn( true );
				
				update_graphics( true, true, true );
			}
		}
		
		void clearPropertiesToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				if( message_box( "Are you sure?\n\nWARNING: ALL the blocks properties will be set to zero!", "Clear Blocks Properties", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					tiles_data data = get_curr_tiles_data();
					
					int ff_block = data.get_first_free_block_id( false );
					
					for( int block_n = 0; block_n < ff_block; block_n++ )
					{
						data.set_block_flags_obj_id( block_n, -1, 0, true );
					}
					
					if( CBoxTileViewType.SelectedIndex == ( int )utils.ETileViewType.tvt_ObjectId )
					{
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
					}
				}
			}
		}
		
		void InsertLeftBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = get_curr_tiles_data();

				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					int block_ind = sel_ind * utils.CONST_BLOCK_SIZE;
					
					Array.Copy( data.blocks, block_ind, data.blocks, block_ind + utils.CONST_BLOCK_SIZE, data.blocks.Length - block_ind - utils.CONST_BLOCK_SIZE );
					
					data.clear_block( sel_ind );
					
					if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						data.inc_tiles_blocks( ( ushort )sel_ind );
					}
					else
					{
						data.inc_screen_blocks( ( ushort )sel_ind );
						data.inc_patterns_tiles( ( ushort )sel_ind );
					}
					
					clear_active_tile_img();
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
			}
		}
		
		void DeleteBlockToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Delete Block", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = get_curr_tiles_data();
	
					int sel_ind = get_context_menu_sender_index( sender );
					
					if( sel_ind >= 0 )
					{
						int block_ind = sel_ind * utils.CONST_BLOCK_SIZE;
						
						Array.Copy( data.blocks, block_ind + utils.CONST_BLOCK_SIZE, data.blocks, block_ind, data.blocks.Length - block_ind - utils.CONST_BLOCK_SIZE );
						
						if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
						{
							data.dec_tiles_blocks( ( ushort )sel_ind );
						}
						else
						{
							data.dec_screen_blocks( ( ushort )sel_ind );
							data.dec_patterns_tiles( ( ushort )sel_ind );
						}
						data.clear_block( data.tiles.Length - 1 );
						
						clear_active_tile_img();
						
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
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
				tiles_data data = get_curr_tiles_data();
				
				for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
				{
					data.set_tile_block( sel_ind, i, data.get_tile_block( m_tile_copy_item_ind, i ) );
				}

				set_status_msg( String.Format( "Tile List: tile #{0:X2} is copied to #{1:X2}", m_tile_copy_item_ind, sel_ind ) );
				
				mark_update_screens_btn( true );
				
				// optimized update_graphics
				progress_bar_show( true, "Updating graphics...", false );
				{
					m_tile_list_manager.copy_tile( tile_list.EType.t_Tiles, m_tile_copy_item_ind, sel_ind );
					
					update_active_tile_img( sel_ind );
					patterns_manager_update_preview();
					m_tiles_processor.update_graphics();
					
					if( CheckBoxScreensAutoUpdate.Checked )
					{
						update_screens( true );
					}
				}
				progress_bar_show( false );
			}
		}
		
		void ClearTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the tile's blocks references will be cleared!", "Clear Selected Tile Refs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					tiles_data data = get_curr_tiles_data();

					data.clear_tile( sel_ind );
					
					clear_active_tile_img();
	
					set_status_msg( String.Format( "Tile List: tile #{0:X2} references are cleared", sel_ind ) );
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
			}
		}
		
		void ClearAllTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the blocks references for all the tiles will be set to zero!", "Clear All Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				tiles_data data = get_curr_tiles_data();
				
				data.clear_tiles();
				
				clear_active_tile_img();
				
				set_status_msg( String.Format( "Tile List: all the tile references are cleared" ) );
				
				mark_update_screens_btn( true );
				
				update_graphics( true, true, true );
			}
		}

		void InsertLeftTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = get_curr_tiles_data();

				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					Array.Copy( data.tiles, sel_ind, data.tiles, sel_ind + 1, data.tiles.Length - sel_ind - 1 );
					
					data.clear_tile( sel_ind );
					data.inc_screen_tiles( ( ushort )sel_ind );
					data.inc_patterns_tiles( ( ushort )sel_ind );
					
					clear_active_tile_img();
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
			}
		}
		
		void DeleteTileToolStripMenuItem3Click_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Delete Tile", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = get_curr_tiles_data();
	
					int sel_ind = get_context_menu_sender_index( sender );
					
					if( sel_ind >= 0 )
					{
						Array.Copy( data.tiles, sel_ind + 1, data.tiles, sel_ind, data.tiles.Length - sel_ind - 1 );
						
						data.dec_screen_tiles( ( ushort )sel_ind );
						data.dec_patterns_tiles( ( ushort )sel_ind );
						data.clear_tile( data.tiles.Length - 1 );
						
						clear_active_tile_img();
						
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
					}
				}
			}
		}

		void BtnOptimizationClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.tiles_data_cnt == 0 )
			{
				message_box( "No data!", "Data Optimization", MessageBoxButtons.OK );
			}
			else
			{
				if( m_optimization_form.ShowDialog() == DialogResult.OK )
				{
					ListViewScreens.BeginUpdate();
					{
						if( m_optimization_form.need_update_screen_list )
						{
							progress_bar_show( true, "Updating screens...", false );
							
							// update a global screen list
							m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, -1 );
							
							// update a local screen list if needed
							if( !CheckBoxLayoutEditorAllBanks.Checked )
							{
								m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, CBoxCHRBanks.SelectedIndex );
							}
	
							mark_update_screens_btn( false );
							
							if( m_layout_editor.mode == layout_editor_base.EMode.em_Screens )
							{
								m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
							}
							
							m_layout_editor.update();
							
							progress_bar_show( false );
							
							m_optimization_form.need_update_screen_list = false;
						}
						
						// need to be reset to avoid incorrect tiles array filling
						// when drawing by blocks 2x2 when the 'Tiles (4x4)' mode is active
						clear_active_tile_img();
					}
					ListViewScreens.EndUpdate();
				}
			}
		}
		
		void ShiftColorsToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
#if DEF_NES
			m_tiles_processor.block_shift_colors( shiftTransparencyToolStripMenuItem.Checked );
#endif
		}

		void BtnBlockReserveCHRsClick_Event(object sender, EventArgs e)
		{
			if( m_tiles_processor.block_reserve_CHRs( m_tiles_processor.get_selected_block(), m_data_manager ) > 0 )
			{
				enable_update_gfx_btn( true );
				mark_update_screens_btn( true );
			}
		}
		
		void BtnTileReserveBlocksClick_Event(object sender, EventArgs e)
		{
			if( m_tiles_processor.tile_reserve_blocks( m_data_manager ) >= 0 )
			{
				enable_update_gfx_btn( true );
				mark_update_screens_btn( true );
			}
		}
#endregion		
// LAYOUT PAINTER ************************************************************************************//
#region layout painter
		void BtnPainterFillWithTileClick_Event(object sender, EventArgs e)
		{
			CheckBoxPainterReplaceTiles.Checked = false;
			
			m_layout_editor.set_param( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SET_PNT_FILL_WITH_TILE, null );
		}
		
		void CheckBoxPainterReplaceTilesChecked_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_param( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SET_PNT_REPLACE_TILES, ( sender as CheckBox ).Checked );
		}
		
		void ActiveTileCancel_Event(object sender, EventArgs e)
		{
			clear_active_tile_img();
		}
		
		void MapScaleX1_Event(object sender, EventArgs e)
		{
			RBtnMapScaleX1.Checked = true;
		}
		
		void MapScaleX2_Event(object sender, EventArgs e)
		{
			RBtnMapScaleX2.Checked = true;
		}
		
		void RBtnMapScaleX1CheckedChanged_Event(object sender, EventArgs e)
		{
			if( RBtnMapScaleX1.Checked )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_BASE_MAP_SCALE_X1, null );
			}
		}
		
		void RBtnMapScaleX2CheckedChanged_Event(object sender, EventArgs e)
		{
			if( RBtnMapScaleX2.Checked )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_BASE_MAP_SCALE_X2, null );
			}
		}
		
		void BtnTilesBlocksClick_Event(object sender, EventArgs e)
		{
			m_tiles_palette_form.show_wnd();
			
			BtnTilesBlocks.Enabled = false;
		}
		
		void TilesBlocksClosed_Event(object sender, EventArgs e)
		{
			BtnTilesBlocks.Enabled = true;
		}
		
		void TileSelected_Event(object sender, EventArgs e)
		{
			select_tile( ( sender as tiles_palette_form ).active_item_id );
		}

		void BlockSelected_Event(object sender, EventArgs e)
		{
			select_block( ( sender as tiles_palette_form ).active_item_id, true, true );
		}
		
		void BlockQuadSelected_Event(object sender, EventArgs e)
		{
			// show property
			select_block( m_tiles_processor.get_selected_block(), false, true );
		}
		
		private void update_tile_image( object sender, EventArgs e )
		{
			NewTileEventArg event_args = e as NewTileEventArg;
			
			int tile_ind 	= event_args.tile_ind;
			tiles_data data = event_args.data;
			
			m_imagelist_manager.update_tile( tile_ind, m_view_type, data, true, PropertyPerBlockToolStripMenuItem.Checked, null, null, m_data_manager.screen_data_type );
			m_tile_list_manager.update_tile( tile_list.EType.t_Tiles, tile_ind );
		}
		
		void UpdateGraphicsAfterOptimization_Event(object sender, EventArgs e)
		{
			mark_update_screens_btn( true );
			
			update_graphics( true, true, true );
		}
		
		void BtnResetTileClick_Event(object sender, EventArgs e)
		{
			clear_active_tile_img();
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
		
		private void update_active_tile_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				m_layout_editor.set_param( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SET_PNT_UPD_ACTIVE_TILE, _ind );
				
				m_tile_preview.update( m_imagelist_manager.get_tiles_image_list()[ _ind ], PBoxActiveTile.Width, PBoxActiveTile.Height, 0, 0, true, true );
				GrpBoxActiveTile.Text = "Tile: " + String.Format( "${0:X2}", _ind );
				
				BtnResetTile.Enabled = true;
				
				BtnPainterFillWithTile.Enabled = CheckBoxPainterReplaceTiles.Enabled = true;
			}
		}
		
		private void update_active_block_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				m_layout_editor.set_param( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SET_PNT_UPD_ACTIVE_BLOCK, _ind );
				
				m_tile_preview.update( m_imagelist_manager.get_blocks_image_list()[ _ind ], PBoxActiveTile.Width, PBoxActiveTile.Height, 0, 0, true, true );
				GrpBoxActiveTile.Text = "Block: " + String.Format( "${0:X2}", _ind );
				
				BtnResetTile.Enabled = true;
				
				if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
				{
					// disable replacing of blocks when the Tiles4x4 mode is active
					// because it requires generating a lot of 4x4 tiles
					CheckBoxPainterReplaceTiles.Enabled = false;
				}
				else
				{
					CheckBoxPainterReplaceTiles.Enabled = true;
				}
				
				BtnPainterFillWithTile.Enabled = true;
			}
		}
		
		private void clear_active_tile_img()
		{
			m_layout_editor.set_param( layout_editor_base.EMode.em_Painter, layout_editor_param.CONST_SET_PNT_CLEAR_ACTIVE_TILE, null );
			
			m_tile_preview.update( null, 0, 0, 0, 0, true, true );
			GrpBoxActiveTile.Text = "...";
			
			BtnResetTile.Enabled = false;
			
			m_layout_editor.update();
			
			CheckBoxPainterReplaceTiles.Checked = false;
			
			BtnPainterFillWithTile.Enabled = CheckBoxPainterReplaceTiles.Enabled = false;
		}
#endregion
// LAYOUT EDITOR *************************************************************************************//
#region layout editor
		void TabCntrlLayoutTilesChanged_Event(object sender, EventArgs e)
		{
			TabPage curr_tab = ( sender as TabControl ).SelectedTab;
			
			if( curr_tab == TabLayout )
			{
				m_layout_editor.update();
			}
		}
		
		void TabControlLayoutToolsSelected_Event(object sender, TabControlEventArgs e)
		{
			TabPage curr_tab = ( sender as TabControl ).SelectedTab;
			
			// reset common states
			{
				LayoutDeleteEntityToolStripMenuItem.Enabled = LayoutEntityOrderToolStripMenuItem.Enabled = false;
				
				screensToolStripMenuItem.Enabled	= 
				builderToolStripMenuItem.Enabled	= 
				entitiesToolStripMenuItem.Enabled	= 
				patternsToolStripMenuItem.Enabled	= false;
				
				ListViewScreens.SelectedItems.Clear();
				
				TreeViewEntities.SelectedNode = null;
				
				CheckBoxSelectTargetEntity.Checked = false;
				
				CheckBoxPainterReplaceTiles.Checked = false;
				
				patterns_manager_reset_active_pattern();
			}
			
			if( curr_tab == TabBuilder )
			{
				builderToolStripMenuItem.Enabled = true;

				m_layout_editor.mode = layout_editor_base.EMode.em_Builder;
			}
			else
			if( curr_tab == TabPainter )
			{
				m_layout_editor.mode = layout_editor_base.EMode.em_Painter;
			}
			else
			if( curr_tab == TabScreenList )
			{
				screensToolStripMenuItem.Enabled = true;
				
				m_layout_editor.mode = layout_editor_base.EMode.em_Screens;
			}
			else
			if( curr_tab == TabEntities )
			{
				entitiesToolStripMenuItem.Enabled = true;
				
				m_layout_editor.mode = layout_editor_base.EMode.em_Entities;
				
				fill_entity_data( get_selected_entity() );
			}
			else
			if( curr_tab == TabPatterns )
			{
				patternsToolStripMenuItem.Enabled = true;
				
				m_layout_editor.mode = layout_editor_base.EMode.em_Patterns;
			}
			else
			{
				throw new Exception( "Unknown mode detected!\n\n[MainForm.TabControlLayoutToolsSelected_Event]" );
			}
		}
		
		void EntitiesCounterUpdate_Event(object sender, EventArgs e)
		{
			if( m_layout_editor.mode == layout_editor_base.EMode.em_Entities )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_UPDATE_ENTS_CNT, null );
				
				TreeViewEntities.Refresh();
			}
		}
		
		int create_screen()
		{
			int scr_glob_ind	= -1;
			int scr_local_ind;
			
			if( ( scr_local_ind = m_data_manager.screen_data_create() ) >= 0 )
			{
				if( ( scr_glob_ind = insert_screen_into_layouts( scr_local_ind ) ) < 0 )
				{
					m_data_manager.screen_data_delete( scr_local_ind );
					
					message_box( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT, "Failed to Create Screen", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}
			
			return scr_glob_ind;
		}

		void delete_screen( layout_screen_data _scr_data )
		{
			if( _scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
			{
				int bank_ind = m_data_manager.get_bank_ind_by_global_screen_ind( _scr_data.m_scr_ind );
				
				delete_screen_by_bank_id( bank_ind, m_data_manager.get_local_screen_ind( bank_ind, _scr_data.m_scr_ind ) );
			}
		}
		
		void delete_screen( int _scr_local_ind )
		{
			m_data_manager.screen_data_delete( _scr_local_ind );

			m_data_manager.remove_screen_from_layouts( CBoxCHRBanks.SelectedIndex, _scr_local_ind  );
			
			if( m_imagelist_manager.remove_screen( CBoxCHRBanks.SelectedIndex, _scr_local_ind ) )
			{
				m_layout_editor.set_param( layout_editor_base.EMode.em_Screens, layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
				
				update_screens_labels_by_bank_id();
			}
		}

		void delete_screen_by_bank_id( int _bank_ind, int _scr_local_ind )
		{
			bool all_banks_screens = CheckBoxLayoutEditorAllBanks.Checked;
			
			// unlock all the screens
			CheckBoxLayoutEditorAllBanks.Checked = true;
			
			if( m_imagelist_manager.remove_screen( _bank_ind, _scr_local_ind ) )
			{
				m_layout_editor.set_param( layout_editor_base.EMode.em_Screens, layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
			}
			
			m_data_manager.get_tiles_data( _bank_ind ).delete_screen( _scr_local_ind );
			
			m_data_manager.remove_screen_from_layouts( _bank_ind, _scr_local_ind );
			
			update_screens_labels_by_bank_id();
			
			CheckBoxLayoutEditorAllBanks.Checked = all_banks_screens;
		}

		bool check_empty_screen( ulong[] _tiles, screen_data _scr_data )
		{
			int tile_n;

			if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int block_n;
				int tile_offs_x;
				int tile_offs_y;
				
				int half_tile_x = platform_data.get_half_tile_x();
				int half_tile_y = platform_data.get_half_tile_y();
				
				ulong	tile_ind		= _tiles[ _scr_data.get_tile( 0 ) ];
				ushort	first_block_ind	= utils.get_ushort_from_ulong( tile_ind, 0 );
				
				for( tile_n = 0; tile_n < _scr_data.m_arr.Length; tile_n++ )
				{
					tile_ind	= _tiles[ _scr_data.get_tile( tile_n ) ];
					
					tile_offs_x = ( tile_n % platform_data.get_screen_tiles_width() );
					tile_offs_y = ( tile_n / platform_data.get_screen_tiles_width() );

					for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
					{
						if( ( half_tile_x == tile_offs_x ) && ( block_n & 0x01 ) != 0 )
						{
							continue;
						}
						
						if( ( half_tile_y == tile_offs_y ) && ( block_n & 0x02 ) != 0 )
						{
							continue;
						}
						
						if( utils.get_ushort_from_ulong( tile_ind, block_n ) != first_block_ind )
						{
							return false;
						}
					}
				}
				
				return true;
			}
			else
			{
				int scr_first_tile_ind = _scr_data.get_tile( 0 );
				int num_tiles = platform_data.get_screen_tiles_cnt_uni( m_data_manager.screen_data_type );
				
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

			tiles_data data	= get_curr_tiles_data();

			int scr_n;
			
			for( scr_n = 0; scr_n < m_data_manager.scr_data_cnt; scr_n++ )
			{
				if( check_empty_screen( data.tiles, data.get_screen_data( scr_n ) ) )
				{
					delete_screen( scr_n );

					--scr_n;
					
					++res;
				}
			}
			
			return res;
		}

		int layout_delete_empty_screens()
		{
			int res = 0;
			int bank_ind;

			bool all_banks_screens = CheckBoxLayoutEditorAllBanks.Checked;
			
			// unlock all the screens
			CheckBoxLayoutEditorAllBanks.Checked = true;
			
			m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).layout_data_proc( delegate( layout_screen_data _scr_data )
			{
				if( _scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					bank_ind = m_data_manager.get_bank_ind_by_global_screen_ind( _scr_data.m_scr_ind );
					
					tiles_data data	= m_data_manager.get_tiles_data( bank_ind );
	
					if( check_empty_screen( data.tiles, data.get_screen_data( m_data_manager.get_local_screen_ind( bank_ind, _scr_data.m_scr_ind ) ) ) )
					{
						delete_screen_by_bank_id( bank_ind, m_data_manager.get_local_screen_ind( bank_ind, _scr_data.m_scr_ind ) );
	
						++res;
					}
				}
			});
			
			CheckBoxLayoutEditorAllBanks.Checked = all_banks_screens;
			
			return res;
		}

		void delete_last_layout_and_screens()
		{
			layout_screen_data lt_scr_data;
			
			List< int > scr_inds_data = new List< int >( m_data_manager.scr_data_cnt );
			
			layout_data layout = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			for( int y = 0; y < layout.get_height(); y++ )
			{
				for( int x = 0; x < layout.get_width(); x++ )
				{
					lt_scr_data = layout.get_data( x, y );
					
					if( lt_scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_inds_data.Add( lt_scr_data.m_scr_ind );
					}
				}
			}
			
			if( scr_inds_data.Count > 0 )
			{
				scr_inds_data.Sort();
				scr_inds_data.Reverse();
				
				foreach( int scr_n in scr_inds_data )
				{
					delete_screen( m_data_manager.get_local_screen_ind( m_data_manager.tiles_data_pos, scr_n ) );
				}
			}
			
			m_data_manager.layout_data_delete( false );
		}
		
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
					int scr_local_ind;
					
					layout_screen_data scr_data;
					
					for( int y = 0; y < _scr_height; y++ )
					{
						for( int x = 0; x < _scr_width; x++ )
						{
							scr_local_ind = m_data_manager.screen_data_create();
							
							scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, scr_local_ind );
							
							if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
							{
								m_data_manager.insert_screen_into_layouts( scr_global_ind );

								scr_data = layout.get_data( x, y );
								scr_data.m_scr_ind = scr_global_ind;
								layout.set_data( scr_data, x, y );
							}
							else
							{
								// delete the last screen manually, cause it wasn't added to the layout
								m_data_manager.screen_data_delete( scr_local_ind );
								
								// clear all created screens and layout
								delete_last_layout_and_screens();
								
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
							
							m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
						}
					}
				}
				
				m_data_manager.layouts_data_pos = m_data_manager.layouts_data_cnt - 1;
				
				ListBoxLayouts.Items.Add( m_data_manager.layouts_data_pos );
				ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
				
				palette_group.Instance.set_palette( get_curr_tiles_data() );
				
				return true;
			}
			
			return false;
		}
		
		private int insert_screen_into_layouts( int _scr_local_ind )
		{
			int scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, _scr_local_ind );
			
			if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
			{
				m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, _scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
				
				palette_group.Instance.set_palette( get_curr_tiles_data() );
				
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

		void BtnLayoutDeleteEmptyScreensClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 && message_box( "Delete all one-block filled screens?", "Clean Up", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				int deleted_screens_cnt;
				
				progress_bar_show( true, "Deleting empty screens...", false );
				
				if( ( deleted_screens_cnt = layout_delete_empty_screens() ) > 0 )
				{
					m_layout_editor.update_dimension_changes();
					
					set_status_msg( "Clean up: " + deleted_screens_cnt + " screens deleted" );
				}
				else
				{
					set_status_msg( "Clean up: no empty screens found" );
				}
				
				progress_bar_show( false );
			}
		}

		void BtnLayoutAddUpRowClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_up();
					
					for( int cell_n = 0; cell_n < data.get_width(); cell_n++ )
					{
						data.get_data( cell_n, 0 ).m_scr_ind = create_screen();
					}
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		void BtnLayoutRemoveTopRowClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_width(); cell_n++ )
					{
						delete_screen( _data.get_data( cell_n, 0 ) );
					}
				}
				
				return _data.remove_up();
			}, "Remove Top Row" );
		}
		
		void BtnLayoutAddDownRowClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_down();

					for( int cell_n = 0; cell_n < data.get_width(); cell_n++ )
					{
						data.get_data( cell_n, data.get_height() - 1 ).m_scr_ind = create_screen();
					}
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		void BtnLayoutRemoveBottomRowClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_width(); cell_n++ )
					{
						delete_screen( _data.get_data( cell_n, _data.get_height() - 1 ) );
					}
				}
				
				return _data.remove_down();
			}, "Remove Bottom Row" );
		}
		
		void BtnLayoutAddLeftColumnClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_left();
					
					for( int cell_n = 0; cell_n < data.get_height(); cell_n++ )
					{
						data.get_data( 0, cell_n ).m_scr_ind = create_screen();
					}

					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		void BtnLayoutRemoveLeftColumnClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_height(); cell_n++ )
					{
						delete_screen( _data.get_data( 0, cell_n ) );
					}
				}
				
				return _data.remove_left();
			}, "Remove Left Column" );
		}
		
		void BtnLayoutAddRightColumnClick_Event(object sender, EventArgs e)
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_right();

					for( int cell_n = 0; cell_n < data.get_height(); cell_n++ )
					{
						data.get_data( data.get_width() - 1, cell_n ).m_scr_ind = create_screen();
					}
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
	
		void BtnLayoutRemoveRightColumnClick_Event(object sender, EventArgs e)
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_height(); cell_n++ )
					{
						delete_screen( _data.get_data( _data.get_width() - 1, cell_n ) );
					}
				}
				
				return _data.remove_right();
			}, "Remove Right Column" );
		}

		void delete_layout_row_column( Func< layout_data, bool > _condition, Func< layout_data, bool, bool > _act, string _caption_msg )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null && _condition( data ) )
				{
					bool delete_scr_data = false;
					
					switch( message_box( "Delete the screens data?\n\n[YES] The screens data will be permanently deleted\n[NO] Delete the screen cells only", _caption_msg, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question ) )
					{
						case DialogResult.Yes:
							{
								delete_scr_data = true;
							}
							break;
							
						case DialogResult.Cancel:
							{
								return;
							}
					}
					
					if( _act( data, delete_scr_data ) )
					{
						m_layout_editor.update_dimension_changes();
					}
				}
			}
		}
		
		void LayoutDeleteAllScreenMarksToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "Are you sure?", "Delete All Screen Marks", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_all_screen_marks();
				
				m_layout_editor.update();
				
				set_status_msg( "Layout Editor: all screen marks deleted" );
			}
		}
		
		void LayoutBringFrontToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_SEL_BRING_FRONT, 0 );
		}
		
		void LayoutSendBackToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_SEL_SEND_BACK, 0 );
		}
		
		void LayoutDeleteScreenToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			entity_instance ent_inst = null;
			
			if( m_layout_editor.mode == layout_editor_base.EMode.em_Entities )
			{
				ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			}

			if( m_layout_editor.delete_screen_from_layout( delegate( layout_screen_data _scr_data ) { delete_screen( _scr_data ); } ) == true )
			{
				if( ent_inst != ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED ) )
				{
					// selected entity has been deleted
					fill_entity_data( null );
					
					CheckBoxSelectTargetEntity.Checked = false;
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
		
		void LayoutDeleteScreenEntitiesToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			entity_instance ent_inst = null;
			
			if( m_layout_editor.mode == layout_editor_base.EMode.em_Entities )
			{
				ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			}

			if( m_layout_editor.delete_screen_entities() == true )
			{
				if( ent_inst != ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED ) )
				{
					// selected entity has been deleted
					fill_entity_data( null );
					
					CheckBoxSelectTargetEntity.Checked = false;
				}
				else
				{
					if( ent_inst != null )
					{
						fill_entity_data( ent_inst.base_entity, ent_inst.properties, ent_inst.name, ent_inst.target_uid );
					}
				}
				
				set_status_msg( "Entities Editor: all the screen entities are deleted" );
			}
		}
		
		void LayoutDeleteEntityToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_DELETE, null ) == true )
			{
				fill_entity_data( null );
				
				CheckBoxSelectTargetEntity.Checked = false;
				
				set_status_msg( "Layout Editor: entity deleted" );
			}
		}
		
		void SetStartScreenMarkToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_start_screen_mark();
		}
		
		void SetScreenMarkToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_screen_mark();
		}
		
		void AdjScrMaskClick_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_adjacent_screen_mask( ( sender as ToolStripMenuItem ).Text );
		}
		
		void BtnCreateLayoutWxHClick_Event(object sender, EventArgs e)
		{
			try
			{
				if( m_data_manager.tiles_data_pos >= 0 && m_create_layout_form.ShowDialog() == DialogResult.OK )
				{
					m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_cnt - 1;
					
					if( create_layout_with_empty_screens_end( create_layout_with_empty_screens_beg( m_create_layout_form.layout_width, m_create_layout_form.layout_height ) ) != false )
					{
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
		
		void CheckBoxLayoutEditorAllBanksCheckChanged_Event(object sender, EventArgs e)
		{
			ScreensShowAllBanksToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			update_screens_by_bank_id( true, need_update_screens() );
			
			m_layout_editor.update();
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
		
		void LayoutShowGridToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			LayoutShowGridToolStripMenuItem.Checked = CheckBoxShowGrid.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void CheckBoxShowMarksChecked_Event(object sender, EventArgs e)
		{
			bool show_marks = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.layout_show_marks = m_layout_editor.show_marks = LayoutShowMarksToolStripMenuItem.Checked = show_marks;
			
			MAPeD.Properties.Settings.Default.Save();
		}
		
		void CheckBoxShowEntitiesChecked_Event(object sender, EventArgs e)
		{
			bool show_ent = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.layout_show_entities = m_layout_editor.show_entities = LayoutShowEntitiesToolStripMenuItem.Checked = show_ent;
			
			MAPeD.Properties.Settings.Default.Save();
			
			LayoutShowTargetsToolStripMenuItem.Enabled = LayoutShowCoordsToolStripMenuItem.Enabled = CheckBoxShowTargets.Enabled = CheckBoxShowCoords.Enabled = show_ent; 
		}
		
		void CheckBoxShowTargetsChecked_Event(object sender, EventArgs e)
		{
			MAPeD.Properties.Settings.Default.layout_show_targets = m_layout_editor.show_targets = LayoutShowTargetsToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.Save();
		}
		
		void CheckBoxShowCoordsChecked_Event(object sender, EventArgs e)
		{
			MAPeD.Properties.Settings.Default.layout_show_coords = m_layout_editor.show_coords = LayoutShowCoordsToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.Save();
		}
		
		void CheckBoxShowGridChecked_Event(object sender, EventArgs e)
		{
			MAPeD.Properties.Settings.Default.layout_show_grid = m_layout_editor.show_grid = LayoutShowGridToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.Save();
		}
#endregion
// LAYOUT SCREENS ************************************************************************************//
#region layout screens
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
		
		void ScreensShowAllBanksToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			ScreensShowAllBanksToolStripMenuItem.Checked = CheckBoxLayoutEditorAllBanks.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		void ListViewScreensClick_Event(object sender, EventArgs e)
		{
			ListView lv = sender as ListView;
			
			if( lv.SelectedItems.Count > 0 )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, lv.SelectedItems[ 0 ].ImageIndex );
			
				set_status_msg( "Layout Editor: screen - " + lv.SelectedItems[ 0 ].Text );
			}
			else
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, layout_data.CONST_EMPTY_CELL_ID );
			
				set_status_msg( "" );
			}
			
			m_layout_editor.update();
		}
		
		void ResetSelectedScreen_Event(object sender, EventArgs e)
		{
			ListViewScreens.SelectedItems.Clear();
		}
		
		private void mark_update_screens_btn( bool _on )
		{
			BtnUpdateScreens.UseVisualStyleBackColor = !_on;
		}
		
		private bool need_update_screens()
		{
			return !BtnUpdateScreens.UseVisualStyleBackColor;
		}

		private bool update_screens_if_needed()
		{
			if( need_update_screens() )
			{
				progress_bar_show( true, "Updating the bank:" + m_data_manager.tiles_data_pos + " screens...", false );
				{
					m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, m_data_manager.tiles_data_pos );
					
					mark_update_screens_btn( false );
				}
				progress_bar_show( false );
				
				return true;
			}
			
			return false;
		}

		void BtnUpdateScreensClick_Event(object sender, EventArgs e)
		{
			progress_bar_show( true, "Updating screens...", false );
			{
				update_screens( true );
			}
			progress_bar_show( false );
		}
		
		void update_screens( bool _unmark_upd_scr_btn, bool _show_status_msg = true )
		{
			// update_screens - may change a current palette
			update_screens_by_bank_id( _unmark_upd_scr_btn, true );
			
			m_layout_editor.update();
			
			if( _show_status_msg )
			{
				set_status_msg( "Screen list updated" );
			}
		}

		void update_all_screens( bool _unmark_upd_scr_btn, bool _show_status_msg = true )
		{
			m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data(), CBoxCHRBanks.SelectedIndex, m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
			
			// renew a palette
			palette_group.Instance.set_palette( get_curr_tiles_data() );
			
			LabelLayoutEditorCHRBankID.Text = CheckBoxLayoutEditorAllBanks.Checked ? "XXX":CBoxCHRBanks.SelectedIndex.ToString();
		
			if( _unmark_upd_scr_btn )
			{
				mark_update_screens_btn( false );
			}
			
			m_layout_editor.update();
			
			if( _show_status_msg )
			{
				set_status_msg( "Screen list updated" );
			}
		}

		void update_screens_by_bank_id( bool _unmark_upd_scr_btn, bool _update_images )
		{
			m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, _update_images, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, CheckBoxLayoutEditorAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
			
			// renew a palette
			palette_group.Instance.set_palette( get_curr_tiles_data() );
			
			LabelLayoutEditorCHRBankID.Text = CheckBoxLayoutEditorAllBanks.Checked ? "XXX":CBoxCHRBanks.SelectedIndex.ToString();
		
			if( _unmark_upd_scr_btn )
			{
				mark_update_screens_btn( false );
			}
		}
		
		void update_screens_labels_by_bank_id()
		{
			m_imagelist_manager.update_screens_labels( m_data_manager.get_tiles_data(), CheckBoxLayoutEditorAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
		}
#endregion
// ENTITY EDITOR *************************************************************************************//
#region entity editor
		void EditEntityCancel_Event( object sender, EventArgs e )
		{
			BtnEntitiesEditInstancesModeClick_Event( sender, e );
		}
		
		private bool m_rename_ent_tree_node = true;
		
		void TreeViewEntitiesDrawNode_Event(object sender, DrawTreeNodeEventArgs e)
		{
			if( e.Node.Parent != null )
			{
				if( m_rename_ent_tree_node )
				{
					int ent_inst_cnt = ( ListBoxLayouts.SelectedIndex >= 0 ) ? m_data_manager.get_num_ent_instances_by_name( e.Node.Name ):0;
					
					string tag_str = "[" + ent_inst_cnt + "]  ";
					
					e.Node.Tag = tag_str;
					
					if( e.Node.Text.IndexOf( tag_str ) < 0 )
					{
						e.Node.Text = tag_str + e.Node.Name;
					}
				}
			}
			
			e.DrawDefault = true;
		}
		
		void TreeViewEntitiesNodeMouseClick_Event(object sender, TreeNodeMouseClickEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				if( e.Node.IsSelected )
				{
					m_rename_ent_tree_node = false;
					{
						e.Node.Text = e.Node.Name;
						
						e.Node.TreeView.Refresh();
					}
					m_rename_ent_tree_node = true;
				}
			}
		}
		
		void TreeViewEntitiesBeforeLabelEdit_Event(object sender, NodeLabelEditEventArgs e)
		{
			m_rename_ent_tree_node = false;
			
			e.Node.Text = e.Node.Name;
			
			e.Node.TreeView.Refresh();
		}
		
		void TreeViewEntitiesAfterLabelEdit_Event(object sender, NodeLabelEditEventArgs e)
		{
			m_rename_ent_tree_node = true;
			
			if( e.Label != null )
			{
				string new_label = e.Label;
				
				// remove tag from the e.Label
				{
					string tag_str = e.Node.Tag.ToString();
					
					if( new_label.IndexOf( tag_str ) == 0 )
					{
						new_label = new_label.Substring( tag_str.Length );
					}
				}
				
				TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( new_label, true );
				
				if( ( nodes_arr.Length > 0 ) && ( nodes_arr[ 0 ] != e.Node ) )
				{
					e.CancelEdit = true;
					
					message_box( "An item with the same name (" + new_label +  ") is already exist!", "Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return;
				}
				
				if( e.Node.Parent == null )
				{
					m_data_manager.group_rename( e.Node.Text, new_label );
					e.Node.Name = new_label;
				}
				else
				{
					m_data_manager.entity_rename( e.Node.Parent.Text, e.Node.Text, new_label );
					e.Node.Name = new_label;
					
					fill_entity_data( get_selected_entity() );
				}
			}
		}

		void TreeViewEntitiesSelect_Event(object sender, TreeViewEventArgs e)
		{
			if( tabControlLayoutTools.SelectedTab == TabEntities )
			{
				entity_data ent = get_selected_entity();
				
				if( ent != null )
				{
					layout_editor_set_entity_mode( layout_editor_param.CONST_SET_ENT_EDIT );
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
				
				layout_editor_set_entity_mode( layout_editor_param.CONST_SET_ENT_EDIT );
				
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
				m_rename_ent_tree_node = false;
				{
					sel_node.Text = sel_node.Name;
					
					sel_node.TreeView.Refresh();
				}
				m_rename_ent_tree_node = true;
				
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
					message_box( "Please, select an entity!", "Base Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return false;
				}
				
				if( message_box( "All the entity <" + ent_name + "> instances will be deleted from all maps!\nAre you sure?", "Delete Base Entity", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
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
			
			message_box( "Can't find entity (" + ent_name +  ")!", "Base Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			
			return false;
		}

		void CheckBoxEntitySnappingChanged_Event(object sender, EventArgs e)
		{
			m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_SNAPPING, ( sender as CheckBox ).Checked );
		}
#endregion
// ENTITY PROPERTIES EDITOR **************************************************************************//
#region entity properties editor
		void fill_entity_data( entity_data _ent, string _inst_prop = "", string _inst_name = "", int _targ_uid = -1 )
		{
			groupBoxEntityEditor.Enabled = ( _ent != null );
			
			if( m_layout_editor.mode == layout_editor_base.EMode.em_Entities )
			{
				if( ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) == layout_editor_param.CONST_SET_ENT_INST_EDIT )
				{
					LayoutDeleteEntityToolStripMenuItem.Enabled = LayoutEntityOrderToolStripMenuItem.Enabled = ( _ent != null );
				}
				else
				{
					LayoutDeleteEntityToolStripMenuItem.Enabled = LayoutEntityOrderToolStripMenuItem.Enabled = false;
				}
				
				if( _ent != null )
				{
					bool edit_inst_mode = ( ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) == layout_editor_param.CONST_SET_ENT_INST_EDIT || ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) == layout_editor_param.CONST_SET_ENT_SELECT_TARGET );
					
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
					
					CheckBoxSelectTargetEntity.Enabled	= edit_inst_mode;
					
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
#if DEF_PLATFORM_16BIT
					TextBoxEntityInstanceProp.Text	= TextBoxEntityProperties.Text	= "space separated 16-bit HEX values: 20a8 1f00 ...";
#else
					TextBoxEntityInstanceProp.Text	= TextBoxEntityProperties.Text	= "space separated 8-bit HEX values: 20 a8 ...";
#endif
					LabelEntityName.Text			= "ENTITY NAME";
				}
	
				CheckBoxSelectTargetEntity.Text		= "Target UID: " + ( _targ_uid < 0 ? "none":_targ_uid.ToString() );
				
				update_entity_preview( _ent == null );
			}
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
			entity_instance ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			
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
			if( !char.IsControl( e.KeyChar ) && "0123456789abcdefABCDEF".IndexOf( e.KeyChar ) < 0 && ( e.KeyChar != ' ' ) )
		    {
		    	e.Handled = true;
		    }
		}

		void update_entity_preview( bool _force_disable = false )
		{
			entity_instance ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			
			entity_data ent = ( ent_inst != null && _force_disable == false ) ? ent_inst.base_entity:get_selected_entity();
			
			if( ent != null )
			{
				m_entity_preview.set_scaled_image( ent.bitmap );
				m_entity_preview.set_scaled_image_pivot( ent.pivot_x, ent.pivot_y );
				m_entity_preview.scale_enabled( true );
				m_entity_preview.update_scaled( true );
			}
			else
			{
				m_entity_preview.set_scaled_image( null );
				m_entity_preview.scale_enabled( false );
				m_entity_preview.update_scaled( true );
			}
			
			if( CheckBoxShowEntities.Checked )
			{
				m_layout_editor.update();
			}
		}
		
		void BtnEntitiesEditInstancesModeClick_Event(object sender, EventArgs e)
		{
			if( tabControlLayoutTools.SelectedTab == TabEntities )
			{
				layout_editor_set_entity_mode( layout_editor_param.CONST_SET_ENT_INST_EDIT );
			}
		}
		
		void EntitiesDeleteInstancesOfAllEntitiesToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			entity_instance ent_inst = null;
			
			if( m_layout_editor.mode == layout_editor_base.EMode.em_Entities )
			{
				ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			}
			
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "All the entities will be deleted from the active map!\nAre you sure?", "Delete All Entities", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
			{
				m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_all_entities();
				
				if( ent_inst != null )
				{
					m_layout_editor.set_param( layout_editor_base.EMode.em_Entities, layout_editor_param.CONST_SET_ENT_INST_RESET, null );
					
					fill_entity_data( null );
					
					CheckBoxSelectTargetEntity.Checked = false;
				}
				
				m_layout_editor.update();
				
				set_status_msg( "Entities Editor: the instances of all entities are deleted" );
			}
		}
		
		void DeleteAllInstancesToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			string ent_name = TreeViewEntities.SelectedNode.Name;
			
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "All the entity <" + ent_name + "> instances will be deleted from the active map!\nAre you sure?", "Delete Entities", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
			{
				int ents_cnt = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_entity_instances( ent_name );
				
				m_layout_editor.update();
				
				set_status_msg( "Entities Editor: all the entity <" + ent_name + "> instances are deleted [" + ents_cnt + "]" );
			}
		}
		
		void update_active_entity()
		{
			entity_data sel_ent = get_selected_entity();
			
			if( sel_ent == null )
			{
				layout_editor_set_entity_mode( layout_editor_param.CONST_SET_ENT_INST_EDIT );
			}
			else
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, sel_ent );
				m_layout_editor.update();
			}
		}
		
		void layout_editor_set_entity_mode( uint _mode )
		{
			switch( _mode )
			{
				case layout_editor_param.CONST_SET_ENT_EDIT:
					{
						CheckBoxSelectTargetEntity.Checked = false;
						
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, get_selected_entity() );
						
						m_layout_editor.set_param( _mode, null );
					}
					break;
					
				case layout_editor_param.CONST_SET_ENT_INST_EDIT:
					{
						TreeViewEntities.SelectedNode = null;
						
						CheckBoxSelectTargetEntity.Checked = false;
						
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, null );
						
						m_layout_editor.set_param( _mode, null );
						
						fill_entity_data( get_selected_entity() );
					}
					break;
					
				case layout_editor_param.CONST_SET_ENT_SELECT_TARGET:
					{
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, null );
						
						m_layout_editor.set_param( _mode, null );
					}
					break;
					
				default:
					{
						throw new Exception( "Unknown mode detected!\n\n[MainForm.layout_editor_set_entity_mode]" );
					}
			}
			
			m_layout_editor.update();
		}
		
		void EntityInstanceSelected_Event( object sender, EventArgs e )
		{
			EventArg2Params args = e as EventArg2Params;
			
			entity_instance ent_inst = args.param1 as entity_instance;
			
			uint ent_mode = ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE );
			
			if( ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT )
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
			if( ent_mode == layout_editor_param.CONST_SET_ENT_SELECT_TARGET )
			{
				entity_instance edit_ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
				
				if( edit_ent_inst != null )
				{
					if( ent_inst == null )
					{
						// reset selected entity if user clicked on empty space
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, 0 );
						
						fill_entity_data( null, null );
						
						// auto disable the 'target entity' mode
						CheckBoxSelectTargetEntity.Checked = false;
					}
					else
					{
						edit_ent_inst.target_uid = ( ent_inst != edit_ent_inst ) ? ent_inst.uid:-1;

						fill_entity_data( edit_ent_inst.base_entity, edit_ent_inst.properties, edit_ent_inst.name, edit_ent_inst.target_uid );
					}
				}
			}
		}
		
		void CheckBoxSelectTargetEntityChanged_Event(object sender, EventArgs e)
		{
			if( m_layout_editor.mode == layout_editor_base.EMode.em_Entities )
			{
				if( ( sender as CheckBox ).Checked )
				{
					CheckBoxSelectTargetEntity.FlatStyle = FlatStyle.Standard;

					layout_editor_set_entity_mode( layout_editor_param.CONST_SET_ENT_SELECT_TARGET );
				}
				else
				{
					CheckBoxSelectTargetEntity.FlatStyle = FlatStyle.System;
					
					// return to the edit instances mode
					m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_EDIT, null );
				}
			}
		}
		
		private void EntitiesMngr_MouseEnter(object sender, EventArgs e)
		{
			if( TreeViewEntities.Enabled )
			{
				m_entity_preview.set_focus();
			}
		}
#endregion
// PALETTE *******************************************************************************************//
#region palette
		void CheckBoxPalettePerCHRChecked_Event(object sender, EventArgs e)
		{
#if DEF_NES
			m_tiles_processor.set_block_editor_palette_per_CHR_mode( ( sender as CheckBox ).Checked );
#endif
		}
		
		void BtnSwapColorsClick_Event(object sender, EventArgs e)
		{
#if !DEF_NES
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				if( m_swap_colors_form.ShowDialog( get_curr_tiles_data() ) == DialogResult.OK )
				{
					BtnUpdateGFXClick_Event( null, null );
					
					palette_group.Instance.update_selected_color();
				}
			}
#endif
		}
		
		void CBoxPalettesChanged_Event(object sender, EventArgs e)
		{
			tiles_data data = get_curr_tiles_data();
			data.palette_pos = CBoxPalettes.SelectedIndex; 
			
			update_palette_related_data( data );
			
			set_status_msg( "Palette changed" );
		}
		
		void BtnPltCopyClick_Event(object sender, EventArgs e)
		{
			tiles_data data = get_curr_tiles_data();
			
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
			tiles_data data = get_curr_tiles_data();
			
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
			
#if DEF_PALETTE16_PER_CHR
			m_tiles_processor.block_editor_update_sel_CHR_palette();
#else
			enable_update_gfx_btn( true );
			mark_update_screens_btn( true );
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
			tiles_data data = get_curr_tiles_data();
			
			if( data != null )
			{
				CBoxPalettes.SelectedIndex = data.palette_pos;
			}
		}
#endif
		void CBocPalettesAdjustWidthDropDown_Event( object sender, EventArgs e )
		{
			( sender as ComboBox ).DropDownWidth = 240;
		}
		
		void CBoxPalettesDrawItem_Event( object sender, DrawItemEventArgs e )
		{
			if( e.Index >= 0 )
			{
				Graphics gfx = e.Graphics;
				e.DrawBackground();
				
				ComboBox cb = sender as ComboBox;
				
				palette16_data plt	= ( ( List< palette16_data > )cb.Tag )[ e.Index ];
				string item_txt		= cb.Items[ e.Index ].ToString();
				SizeF txt_size		= gfx.MeasureString( item_txt, e.Font );
				int plt_offset		= ( int )txt_size.Width + 7;
					
				utils.brush.Color = e.ForeColor;
				gfx.DrawString( item_txt, e.Font, utils.brush, e.Bounds.X, e.Bounds.Y );
				
				int clr;
				int clr_box_side	= 12;
				int clr_box_y_offs	= e.Bounds.Y + ( ( e.Bounds.Height - clr_box_side ) >> 1 );
				int clrs_cnt		= utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS;
				
				for( int i = 0; i < clrs_cnt; i++ )
				{
					clr = palette_group.Instance.main_palette[ plt.subpalettes[ i >> 2 ][ i & 0x03 ] ];
					
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					
					gfx.FillRectangle( utils.brush, plt_offset + ( i * clr_box_side ), clr_box_y_offs, clr_box_side, clr_box_side );
				}
				
				utils.pen.Color = utils.CONST_COLOR_PEN_DEFAULT;
				gfx.DrawRectangle( utils.pen, plt_offset - 1, clr_box_y_offs - 1, ( clr_box_side * clrs_cnt ) + 1, clr_box_side + 1 );
			}
		}
#endregion		
// SCREEN DATA CONVERTER *****************************************************************************//
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
							update_graphics( false, true, false );
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
							update_graphics( false, true, false );
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
				set_status_msg( "Screen data conversion canceled!" );
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
						
						GrpBoxPainter.Text = "Data Type: Tiles4x4";
						clear_active_tile_img();
						
						m_tile_list_manager.visible( tile_list.EType.t_Tiles, true );
						m_tile_list_manager.reset( tile_list.EType.t_Tiles );
					}
					break;

				case data_sets_manager.EScreenDataType.sdt_Blocks2x2:
					{
						RBtnScreenDataTiles.Checked		= false;
						RBtnScreenDataBlocks.Checked	= true;
						
						PanelTiles.Enabled = false;
						
						GrpBoxTileEditor.Enabled = false;
						
						GrpBoxPainter.Text = "Data Type: Blocks2x2";
						clear_active_tile_img();
						
						m_tiles_processor.tile_select_event( -1, null );
						
						m_tile_list_manager.visible( tile_list.EType.t_Tiles, false );
					}
					break;
			}
			
			m_tiles_palette_form.set_screen_data_type( _type );
			m_optimization_form.set_screen_data_type( _type );
			m_layout_editor.set_screen_data_type( _type );
			
			patterns_manager_reset_active_pattern();
			
			return true;
		}
		
#endregion
// PATTERNS MANAGER **********************************************************************************//
#region patterns manager
		void patterns_manager_reset_active_pattern()
		{
			patterns_manager_reset_state();
			
			TreeViewPatterns.SelectedNode = null;
			
			m_layout_editor.set_param( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			
			patterns_manager_update_preview();
			
			m_layout_editor.update();
		}
		
		void patterns_manager_update_data()
		{
			m_pattern_preview.reset_scale();
			
			patterns_manager_reset_state();
			
			patterns_manager_update_tree_view();
			
			patterns_manager_update_preview();
		}

		void patterns_manager_reset_state()
		{
			CheckBoxPatternAdd.Checked = false;
			
			patterns_manager_enable( true );
		}
		
		void patterns_manager_enable( bool _on )
		{
			TreeViewPatterns.Enabled		= _on;
			
			BtnPatternGroupAdd.Enabled		= _on;
			BtnPatternGroupDelete.Enabled	= _on;
			
			CheckBoxPatternAdd.Enabled		= _on;
			BtnPatternDelete.Enabled 		= _on;
			
			BtnPatternRename.Enabled		= _on;
			
			BtnPatternReset.Enabled			= _on;
		}

		void patterns_manager_update_tree_view()
		{
			TreeViewPatterns.BeginUpdate();
			{
				TreeViewPatterns.Nodes.Clear();
			}
			TreeViewPatterns.EndUpdate();

			int ptrns_cnt = 0;
			
			tiles_data data = get_curr_tiles_data();
			
			if( data != null )
			{
				foreach( var key in data.patterns_data.Keys )
				{ 
					patterns_manager_pattern_group_add( key, false );
					
					( data.patterns_data[ key ] as List< pattern_data > ).ForEach( delegate( pattern_data _pattern ) { ++ptrns_cnt; patterns_manager_pattern_add( key, _pattern, false ); } );
				}
			}
			
			// on Linux TreeViewPatterns.SelectedNode is not reset when clearing nodes
			if( ptrns_cnt > 0 )
			{
				patterns_manager_reset_active_pattern();
			}
			else
			{
				TreeViewPatterns.SelectedNode = null;
			}
		}
		
		bool patterns_manager_pattern_rename( string _grp_name, string _old_name, string _new_name )
		{
			tiles_data data = get_curr_tiles_data();
			
			List< pattern_data > patterns = data.patterns_data[ _grp_name ];
			
			if( patterns != null )
			{
				patterns.ForEach( delegate( pattern_data _pattern ) { if( _pattern.name == _old_name ) { _pattern.name = _new_name; return; } } );
				
				return true;
			}
			
			return false;
		}
		
		bool patterns_manager_pattern_add( string _grp_name, pattern_data _pattern, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _pattern.name, true );

			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "A pattern with the same name (" + _pattern.name +  ") is already exist!", "Add Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			nodes_arr = TreeViewPatterns.Nodes.Find( _grp_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				if( _add_to_data_bank )
				{
					List< pattern_data > patterns = get_curr_tiles_data().patterns_data[ _grp_name ];
					
					if( patterns != null )
					{
						patterns.Add( _pattern );
					}
				}
				
				TreeNode node = new TreeNode( _pattern.name );
				node.Name = _pattern.name;
				node.ContextMenuStrip = ContextMenuStripPatternItem;
				
				TreeViewPatterns.BeginUpdate();
				{
					nodes_arr[ 0 ].Nodes.Add( node );
					
					nodes_arr[ 0 ].Expand();
					
					TreeViewPatterns.SelectedNode = node;
				}
				TreeViewPatterns.EndUpdate();
				
				return true;
			}
			
			return false;
		}
		
		bool patterns_manager_pattern_delete( string _pattern_name, string _grp_name )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _pattern_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				List< pattern_data > patterns = get_curr_tiles_data().patterns_data[ _grp_name ];
				
				if( patterns != null )
				{
					pattern_data pattern;
					
					for( int ent_n = 0; ent_n < patterns.Count; ent_n++ )
					{
						pattern = patterns[ ent_n ];
						
						if( pattern.name == _pattern_name ) 
						{
							pattern.reset();

							patterns.Remove( pattern );

							break; 
						} 
					}
				}

				TreeNode node = new TreeNode( _pattern_name );
				node.Name = _pattern_name;
				node.ContextMenuStrip = ContextMenuStripPatternItem;
				
				TreeViewPatterns.BeginUpdate();
				{
					nodes_arr[ 0 ].Remove();
				}
				TreeViewPatterns.EndUpdate();
				
				return true;
			}
			
			return false;
		}
		
		bool patterns_manager_pattern_group_rename( string _old_name, string _new_name )
		{
			tiles_data data = get_curr_tiles_data();
			
			if( data.patterns_data.ContainsKey( _old_name ) )
			{
				List< pattern_data > patterns = data.patterns_data[ _old_name ];
	
				data.patterns_data.Remove( _old_name );
				data.patterns_data.Add( _new_name, patterns );
					
				return true;
			}
			
			return false;
		}
		
		bool patterns_manager_pattern_group_add( string _name, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _name, true );
			
			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "An item with the same name (" + _name +  ") is already exist!", "Add Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			TreeNode node = new TreeNode( _name );
			node.Name = _name;
			node.ContextMenuStrip = ContextMenuStripGroupItem;
				
			TreeViewPatterns.BeginUpdate();
			{
				TreeViewPatterns.Nodes.Add( node );
				
				TreeViewPatterns.SelectedNode = node;
			}
			TreeViewPatterns.EndUpdate();
			
			if( _add_to_data_bank )
			{
				get_curr_tiles_data().patterns_data.Add( _name, new List< pattern_data >() );
			}
			
			return true;
		}
		
		public bool patterns_manager_pattern_group_delete( string _name )
		{
			if( TreeViewPatterns.Nodes.ContainsKey( _name ) )
			{
				tiles_data data = get_curr_tiles_data();
					
				TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _name, true );
				
				if( nodes_arr.Length > 0 )
				{
					if( nodes_arr[ 0 ].FirstNode != null )
					{
						if( MainForm.message_box( "The selected group is not empty!\n\nRemove all child patterns?", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
						{
							TreeViewPatterns.BeginUpdate();
							{
								TreeViewPatterns.Nodes.RemoveByKey( _name );
							}
							TreeViewPatterns.EndUpdate();
						}
						else
						{
							return false;
						}
					}
					else
					{
						TreeViewPatterns.BeginUpdate();
						{
							TreeViewPatterns.Nodes.RemoveByKey( _name );
						}
						TreeViewPatterns.EndUpdate();
					}
					
					if( data.patterns_data.ContainsKey( _name ) )
					{
						List< pattern_data > patterns = data.patterns_data[ _name ];

						data.patterns_data.Remove( _name );
						
						if( patterns != null )
						{
							patterns.ForEach( delegate( pattern_data _pattern ) { _pattern.reset(); } );
						}
						
						return true;
					}
				}
			}
			
			return false;
		}

		void patterns_manager_update_preview()
		{
			m_pattern_preview.set_scaled_image( null );
			m_pattern_preview.scale_enabled( false );
			
			if( TreeViewPatterns.SelectedNode == null )
			{
				// redraw a current selected pattern
				m_pattern_preview.update_scaled( true );
				
				m_pattern_preview.draw_string( "Tile patterns are often-used combinations of tiles.\nHere you can create and manage them.\n\n- Select or create a new patterns group\n\n- Click the 'Add' pattern button to add a new one.\n\n- Select a pattern in the tree view to place it on a map.\n\n- Scale selected pattern using a mouse wheel.", 0, 0 );
			}
			else
			{
				if( CheckBoxPatternAdd.Checked )
				{
					// redraw a current selected pattern
					m_pattern_preview.update_scaled( true );
				}
				else
				{
					TreeNode node = TreeViewPatterns.SelectedNode;
					
					if( node == null || node.Parent == null )
					{
						m_pattern_preview.update_scaled( true );
					
						m_pattern_preview.draw_string( "Select a pattern or create a new one.", 0, 0 );
					}
					else
					{
						// draw a pattern
						pattern_data pattern = get_curr_tiles_data().get_pattern_by_name( node.Name );

						if( pattern == null )
						{
							throw new Exception( "UNEXPECTED ERROR!\n\nCan't find the pattern <" + node.Name + ">!" );
						}
						
						patterns_manager_update_pattern_image( pattern );
						
						m_pattern_preview.set_scaled_image( m_pattern_image );
						m_pattern_preview.scale_enabled( true );
						m_pattern_preview.update_scaled( true );
						
						m_pattern_preview.draw_string( "Place the <" + node.Name + " [" + pattern.width + "x" + pattern.height + "]> on a map.", 0, 0 );
					}
				}
			}
		}
		
		private void patterns_manager_update_pattern_image( pattern_data _pttrn )
		{
			m_pattern_gfx.Clear( Color.FromArgb( 0 ) );

			int scr_tile_size	= utils.CONST_SCREEN_TILES_SIZE >> 1;
			List< Bitmap > img_list;
			
			if( m_data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				img_list = m_imagelist_manager.get_tiles_image_list();
			}
			else
			{
				img_list = m_imagelist_manager.get_blocks_image_list();
				scr_tile_size >>= 1;
			}
			
			int start_pos_x = ( m_pattern_image.Width >> 1 ) - ( ( _pttrn.width * scr_tile_size ) >> 1 );
			int start_pos_y = ( m_pattern_image.Height >> 1 ) - ( ( _pttrn.height * scr_tile_size ) >> 1 );
			
			for( int tile_y = 0; tile_y < _pttrn.height; tile_y++ )
			{
				for( int tile_x = 0; tile_x < _pttrn.width; tile_x++ )
				{
					m_pattern_gfx.DrawImage( img_list[ _pttrn.data.get_tile( tile_y * _pttrn.width + tile_x ) ], start_pos_x + tile_x * scr_tile_size, start_pos_y + tile_y * scr_tile_size, scr_tile_size, scr_tile_size );
				}
			}
		}
		
		void BtnPatternGroupAddClick_Event(object sender, EventArgs e)
		{
			m_object_name_form.Text = "Add Group";
			
			m_object_name_form.edit_str = "GROUP";
			
			if( m_object_name_form.ShowWindow() == DialogResult.OK )
			{
				patterns_manager_pattern_group_add( m_object_name_form.edit_str, true );
			}
		}
		
		void BtnPatternGroupDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent == null )
				{
					patterns_manager_pattern_group_delete( sel_node.Name );

					patterns_manager_update_preview();
				}
				else
				{
					MainForm.message_box( "Please, select a group!", "Delete Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPatterns.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a group!", "Delete Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Delete Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnPatternAddClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node == null || sel_node.Parent != null )
			{
				MainForm.message_box( "Please, select a group!", "Add Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				CheckBoxPatternAdd.Checked = false;
				
				return;
			}
			
			CheckBoxPatternAdd.Checked ^= true;
			
			patterns_manager_enable( !CheckBoxPatternAdd.Checked );
			CheckBoxPatternAdd.Enabled = true;
			
			patterns_manager_update_preview();
		}

		void BtnPatternAddChanged_Event(object sender, EventArgs e)
		{
			if( CheckBoxPatternAdd.Checked )
			{
				CheckBoxPatternAdd.FlatStyle = FlatStyle.Standard;
				CheckBoxPatternAdd.Text = "Cancel";
				
				m_layout_editor.set_param( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SET_PTTRN_EXTRACT_BEGIN, null );
			}
			else
			{
				CheckBoxPatternAdd.FlatStyle = FlatStyle.System;
				CheckBoxPatternAdd.Text = "Add";
				
				m_layout_editor.set_param( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			}
			
			m_layout_editor.update();
		}
		
		void PatternExtractEnd_Event(object sender, EventArgs e)
		{
			m_object_name_form.Text = "Add Pattern";
			
			m_object_name_form.edit_str = "PATTERN";
			
			if( m_object_name_form.ShowWindow() == DialogResult.OK )
			{
				patterns_manager_reset_state();
				
				PatternEventArg pattern_event = e as PatternEventArg;
				
				pattern_data data = pattern_event.data;
				data.name = m_object_name_form.edit_str;
				
				patterns_manager_pattern_add( TreeViewPatterns.SelectedNode.Name, data, true );
				
				MainForm.set_status_msg( "Added tiles pattern <" + m_object_name_form.edit_str + ">" );
				
				patterns_manager_update_preview();
			}
			else
			{
				patterns_manager_reset_state();
			}
		}
		
		void PatternPlaceCancel_Event(object sender, EventArgs e)
		{
			patterns_manager_reset_active_pattern();
		}
		
		void BtnPatternDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					if( MainForm.message_box( "Are you sure?", "Delete Pattern", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						if( patterns_manager_pattern_delete( sel_node.Name, sel_node.Parent.Name ) )
						{
							MainForm.set_status_msg( "Deleted tiles pattern <" + sel_node.Name + ">" );
							
							patterns_manager_update_preview();
						}
					}
				}
				else
				{
					MainForm.message_box( "Please, select a pattern!", "Delete Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPatterns.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a pattern!", "Delete Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Delete Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnPatternRenameClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				sel_node.BeginEdit();
			}
			else
			{
				MainForm.message_box( "Please, select an item!", "Rename Item", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void BtnPatternResetClick_Event(object sender, EventArgs e)
		{
			patterns_manager_reset_active_pattern();
		}
		
		void PatternsTreeViewNodeSelect_Event(object sender, TreeViewEventArgs e)
		{
			if( TreeViewPatterns.SelectedNode.Parent != null )
			{
				m_layout_editor.set_param( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SET_PTTRN_PLACE, get_curr_tiles_data().get_pattern_by_name( TreeViewPatterns.SelectedNode.Name ) );
			}
			else
			{
				m_layout_editor.set_param( layout_editor_base.EMode.em_Patterns, layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			}
			
			patterns_manager_update_preview();
			
			m_layout_editor.update();
		}
		
		void PatternsTreeViewNodeRename_Event(object sender, NodeLabelEditEventArgs e)
		{
			if( e.Label != null )
			{
				TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( e.Label, true );
				
				if( ( nodes_arr.Length > 0 ) && ( nodes_arr[ 0 ] != e.Node ) )
				{
					e.CancelEdit = true;
					
					message_box( "An item with the same name (" + e.Label +  ") is already exist!", "Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return;
				}
				
				if( e.Node.Parent == null )
				{
					patterns_manager_pattern_group_rename( e.Node.Text, e.Label );
					e.Node.Name = e.Label;
				}
				else
				{
					patterns_manager_pattern_rename( e.Node.Parent.Text, e.Node.Text, e.Label );
					e.Node.Name = e.Label;
					
					patterns_manager_update_preview();
				}
			}
		}

		private void PatternsMngr_MouseEnter(object sender, EventArgs e)
		{
			if( TreeViewPatterns.Enabled )
			{
				m_pattern_preview.set_focus();
			}
		}
#endregion
	}
}