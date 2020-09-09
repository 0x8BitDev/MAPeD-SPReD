/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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

namespace MAPeD
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	/// 
	
	public partial class MainForm : Form
	{
		private exporter_zx_sjasm	m_exp_zx_asm	= null;
#if DEF_NES
		private exporter_nes_asm	m_exp_nes_asm	= null;
#elif DEF_SMS
		private exporter_sms_asm	m_exp_sms_asm		= null;
		private swap_colors_form	m_swap_colors_form	= null;
#endif		
		private data_conversion_options_form	m_data_conversion_options_form	= null;
		
		private Pen			m_pen						= null;
		private Graphics 	m_pbox_active_tile_gfx		= null;
		
		private data_sets_manager	m_data_manager		= null;
		private tiles_processor		m_tiles_processor	= null;		
		private screen_editor		m_screen_editor		= null;
		private layout_editor		m_layout_editor		= null;
		
		private tiles_palette_form		m_tiles_palette_form	= null;
		private presets_manager_form	m_presets_manager_form	= null;
		private optimization_form		m_optimization_form		= null;
		private object_name_form		m_object_name_form		= null;
		private import_tiles_form		m_import_tiles_form		= null;
		private screen_mark_form		m_screen_mark_form		= null;
		private description_form		m_description_form		= null;
		private statistics_form			m_statistics_form		= null;
		private create_layout_form		m_create_layout_form	= null;
		
		private SPSeD.py_editor		m_py_editor	= null;

		private bool m_project_loaded	= false;
		
		private export_active_tile_block_set_form	m_export_active_tile_block_set_form	= null;
		
		private image_preview		m_entity_preview	= null;
		
		private int	m_block_copy_item_ind	= -1;
		private int	m_tile_copy_item_ind	= -1;
		
		private imagelist_manager	m_imagelist_manager	= null;
		
		private static ToolStripStatusLabel	m_status_bar_label = null;
		
		enum ETileViewType
		{
			tvt_Graphics = 0,
			tvt_ObjectId = 1,
			tvt_Number	 = 2,
		};
		
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

			m_data_manager 		= new data_sets_manager();
			
			m_exp_zx_asm		= new exporter_zx_sjasm( m_data_manager );
#if DEF_NES			
			m_exp_nes_asm		= new exporter_nes_asm( m_data_manager );
#elif DEF_SMS
			m_exp_sms_asm		= new exporter_sms_asm( m_data_manager );
			
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

			m_imagelist_manager	= new imagelist_manager( PanelTiles, PanelTilesClick_Event, ContextMenuTilesList, PanelBlocks, PanelBlocksClick_Event, ContextMenuBlocksList, ListViewScreens );
			
			m_screen_editor = new screen_editor( PBoxScreen, m_imagelist_manager.get_tiles_image_list() );
			m_screen_editor.subscribe_event( m_data_manager );
			
			m_layout_editor = new layout_editor( PBoxLayout, LayoutLabel, m_data_manager.get_tiles_data(), ListViewScreens );
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
			
			m_object_name_form = new object_name_form();
			
			m_import_tiles_form = new import_tiles_form();
			
			m_export_active_tile_block_set_form = new export_active_tile_block_set_form();
			
			m_screen_mark_form = new screen_mark_form();
			
			m_description_form = new description_form();
			
			m_statistics_form = new statistics_form( m_data_manager );
			
			m_create_layout_form = new create_layout_form();

			enable_update_gfx_btn( false );
			enable_update_screens_btn( true );

			m_tiles_palette_form = new tiles_palette_form( m_imagelist_manager.get_tiles_image_list(), ContextMenuTilesList, m_imagelist_manager.get_blocks_image_list(), ContextMenuBlocksList );
			m_tiles_palette_form.TilesBlocksClosed 	+= new EventHandler( MainForm_TilesBlocksClosed );
			m_tiles_palette_form.TileSelected 		+= new EventHandler( MainForm_TileSelected );
			m_tiles_palette_form.BlockSelected 		+= new EventHandler( MainForm_BlockSelected );
			m_tiles_palette_form.ResetActiveTile 	+= new EventHandler( MainForm_ResetActiveTile );
			
			m_presets_manager_form = new presets_manager_form( m_imagelist_manager.get_tiles_image_list() );
			m_presets_manager_form.PresetsManagerClosed 	+= new EventHandler( MainForm_PresetsManagerClosed );
			
			m_optimization_form = new optimization_form( m_data_manager );
			m_optimization_form.UpdateGraphics += new EventHandler( MainForm_UpdateGraphicsAfterOptimization );
			
			// prepare 'Active Tile' for drawing
			{
				Bitmap bmp = new Bitmap( PBoxActiveTile.Width, PBoxActiveTile.Height );
				PBoxActiveTile.Image = bmp;
				
				m_pbox_active_tile_gfx = Graphics.FromImage( bmp );
				
				m_pbox_active_tile_gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
				m_pbox_active_tile_gfx.PixelOffsetMode 		= PixelOffsetMode.HighQuality;

				m_pen = new Pen( Color.White );
				m_pen.EndCap 	= LineCap.NoAnchor;
				m_pen.StartCap 	= LineCap.NoAnchor;			
				
				clear_active_tile_img();
			}

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
																new SToolTipData( BtnCopyCHRBank, "Copy active CHR bank" ),
																new SToolTipData( BtnAddCHRBank, "Add new CHR bank" ),
																new SToolTipData( BtnDeleteCHRBank, "Delete active CHR Bank" ),																
																new SToolTipData( BtnCHRBankPrevPage, "Previous CHR Bank's page" ),
																new SToolTipData( BtnCHRBankNextPage, "Next CHR Bank's page" ),																
																new SToolTipData( BtnUpdateGFX, "Update tiles\\blocks and screens ( if auto update is enabled )" ),
																new SToolTipData( BtnOptimization, "Delete unused screens\\tiles\\blocks\\CHRs" ),
																new SToolTipData( CheckBoxTileEditorLock, "Enable\\disable tile editing" ),
																new SToolTipData( BtnTilesBlocks, "Arrays of tiles and blocks to build screens" ),
																new SToolTipData( Palette0, "Shift+1 / Ctrl+1,2,3,4 to select a color" ),
																new SToolTipData( Palette1, "Shift+2 / Ctrl+1,2,3,4 to select a color" ),
																new SToolTipData( Palette2, "Shift+3 / Ctrl+1,2,3,4 to select a color" ),
																new SToolTipData( Palette3, "Shift+4 / Ctrl+1,2,3,4 to select a color" ),																
																new SToolTipData( CheckBoxShowMarks, "Show screen marks" ),
																new SToolTipData( CheckBoxShowEntities, "Show layout entities" ),
																new SToolTipData( CheckBoxShowTargets, "Show entity targets" ),
																new SToolTipData( CheckBoxShowCoords, "Show coordinates of a selected entity" ),
																new SToolTipData( CheckBoxPalettePerCHR, "MMC5 extended attributes mode" ),
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
			Project_openFileDialog.Filter = Project_openFileDialog.Filter + "|MAPeD-SMS (*." + utils.CONST_SMS_FILE_EXT + ")|*." + utils.CONST_SMS_FILE_EXT;
			
			BtnSwapColors.Enabled = false;
#elif DEF_SMS
			Project_saveFileDialog.DefaultExt = utils.CONST_SMS_FILE_EXT;
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "NES", "SMS" );
			Project_saveFileDialog.Filter = Project_saveFileDialog.Filter.Replace( "nes", "sms" );

			Project_openFileDialog.DefaultExt = utils.CONST_SMS_FILE_EXT;
			Project_openFileDialog.Filter = "MAPeD-SMS (*." + utils.CONST_SMS_FILE_EXT + ")|*." + utils.CONST_SMS_FILE_EXT + "|" + Project_openFileDialog.Filter;
			
			Project_exportFileDialog.Filter = Project_exportFileDialog.Filter.Replace( "CA65\\NESasm", "WLA-DX" );

			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "NES", "SMS" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "nes", "sms" );
			Import_openFileDialog.Filter = Import_openFileDialog.Filter.Replace( "SMS CHR Bank", "SMS CHR Bank 4bpp" );
			
			CheckBoxPalettePerCHR.Visible = false;
			
			toolStripSeparatorShiftTransp.Visible = shiftTransparencyToolStripMenuItem.Visible = shiftColorsToolStripMenuItem.Visible = false; 
#endif

			if( utils.CONST_CHR_BANK_PAGES_CNT == 1 )
			{
				BtnCHRBankNextPage.Enabled = BtnCHRBankPrevPage.Enabled = false;
				prevPageToolStripMenuItem.Enabled = nextPageToolStripMenuItem.Enabled = false;
			}
			
			if( _args.Length > 0 )
			{
				project_load( _args[0] );
			}
			else
			{
				set_title_name( null );
				
				reset();
				set_status_msg( "Add a new CHR bank to begin. Press the \"Bank+\" button! <F1> - Quick Guide" );
			}
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
			Text = utils.CONST_APP_NAME + " " + utils.CONST_APP_VER + ( _msg != null ? " - " + _msg:"" );
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
		
		private void clear_active_tile_img()
		{
			m_pbox_active_tile_gfx.Clear( utils.CONST_COLOR_ACTIVE_TILE_BACKGROUND );
			
			// red cross
			{
				// draw the red cross as a sign of inactive state
				m_pen.Color = utils.CONST_COLOR_PIXBOX_INACTIVE_CROSS;
				
				m_pbox_active_tile_gfx.DrawLine( m_pen, 0, 0, utils.CONST_TILES_IMG_SIZE, utils.CONST_TILES_IMG_SIZE );
				m_pbox_active_tile_gfx.DrawLine( m_pen, utils.CONST_TILES_IMG_SIZE, 0, 0, utils.CONST_TILES_IMG_SIZE );
			}
			
			PBoxActiveTile.Invalidate();
			
			m_screen_editor.set_active_tile( -1, null, screen_editor.EFillMode.efm_Unknown );
			
			GrpBoxActiveTile.Text = "...";
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
			PanelTiles.Enabled	= _on;
			
			m_tiles_palette_form.enable( _on );
			m_presets_manager_form.enable( _on );
			
			tabControlScreensEntities.Enabled = _on;
		}
		
		private void reset()
		{
			m_data_manager.reset();
			fill_entity_data( null );

			entity_instance.reset_instances_counter();
			
			CBoxTileViewType.SelectedIndex = ( int )ETileViewType.tvt_Graphics;
			
			CheckBoxEntitySnapping.Checked 	= true;
			CheckBoxShowMarks.Checked		= true;
			CheckBoxShowEntities.Checked 	= true;
			CheckBoxShowTargets.Checked 	= true;
			CheckBoxShowCoords.Checked		= true;
			
			CheckBoxPalettePerCHR.Checked	= false;

			enable_main_UI( false );
			
			CheckBoxScreenShowGrid.Checked = true;
			
			TilesLockEditorToolStripMenuItem.Checked = CheckBoxTileEditorLock.Checked = true;

			update_graphics( false );
			
			enable_update_gfx_btn( false );
			enable_update_screens_btn( false );

			clear_active_tile_img();
			
			CBoxCHRBanks.Items.Clear();
			ListBoxScreens.Items.Clear();
			ListBoxLayouts.Items.Clear();
			
			CBoxBlockObjId.SelectedIndex = 0;
			CBoxBlockObjId.Tag = null;
			
			ComboBoxEntityZoom.SelectedIndex = 0;

			m_layout_editor.reset( false );
			m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data() );
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_All );
			
			SelectCHRToolStripMenuItemClick_Event( null, null );
			
			PropertyPerBlockToolStripMenuItemClick_Event( null, null );
			
			RBtnScreenEditModeSingle.Checked = true;
			
			m_tiles_palette_form.reset();
			m_presets_manager_form.reset();
			
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
				
				set_title_name( null );
				
				m_description_form.edit_text = "";
			}
		}

		void CBoxTileViewTypeChanged_Event(object sender, EventArgs e)
		{
			update_graphics( false );
			
			clear_active_tile_img();
		}

#region load save import export
		void LoadToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_project_loaded = false;
			
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
			
			if( m_project_loaded )
			{
				if( m_description_form.auto_show() && m_description_form.edit_text.Length > 0 )
				{
					m_description_form.ShowDialog( this );
				}
			}
		}

		void ProjectLoadOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// LOAD PROJECT...
			project_load( ( ( FileDialog )sender ).FileName );
		}

		void project_load( string _filename )
		{
			reset();
			
			FileStream 		fs = null;
			BinaryReader 	br = null;
			
			try
			{
				string file_ext = Path.GetExtension( _filename ).Substring( 1 );
#if DEF_NES
				if( file_ext == utils.CONST_SMS_FILE_EXT )
#elif DEF_SMS
				if( file_ext == utils.CONST_NES_FILE_EXT )
#endif
				{
					if( m_data_conversion_options_form.ShowDialog() == DialogResult.Cancel )
					{
						return;
					}
				}
				
				fs = new FileStream( _filename, FileMode.Open, FileAccess.Read );
				{
					br = new BinaryReader( fs );
					if( br.ReadUInt32() == utils.CONST_PROJECT_FILE_MAGIC )
					{
						byte ver = br.ReadByte();
						
						if( ver == utils.CONST_PROJECT_FILE_VER )
						{
							m_data_manager.load( br, file_ext, m_data_conversion_options_form.screens_align_mode, m_data_conversion_options_form.convert_colors );
							
							uint flags = br.ReadUInt32();
#if DEF_NES							
							CheckBoxPalettePerCHR.Checked 	= ( ( flags&0x01 ) == 0x01 ? true:false );
#endif							
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
						int tiles_cnt = m_data_manager.tiles_data_cnt;
						
						for( int i = 0; i < tiles_cnt; i++ )
						{
							CBoxCHRBanks.Items.Add( m_data_manager.get_tiles_data( i ) );
						}

						m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data() );
						
						CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;
						
						enable_main_UI( true );
						
						update_layouts_list_box();
					}
					
					m_layout_editor.update();
					update_graphics( false );
#if DEF_SMS
					palette_group.Instance.active_palette = 0;
#endif
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( _filename ) );
				
				set_status_msg( "Project loaded" );
				
				m_project_loaded = true;
			}
			catch( Exception _err )
			{
				reset();
				
				message_box( _err.Message, "Project Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			if( br != null )
			{
				br.Close();
			}
			
			if( fs != null )
			{
				fs.Close();
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
				fs = new FileStream( filename, FileMode.Create, FileAccess.Write );
				{
					bw = new BinaryWriter( fs );
					bw.Write( utils.CONST_PROJECT_FILE_MAGIC );
					bw.Write( utils.CONST_PROJECT_FILE_VER );

					m_data_manager.save( bw );
					
					uint flags = ( uint )( CheckBoxPalettePerCHR.Checked ? 1:0 );
					bw.Write( flags );
					
					// save description
					bw.Write( m_description_form.edit_text );
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( filename ) );
				
				set_status_msg( "Project saved" );
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Project Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			if( bw != null )
			{
				bw.Close();
			}
			
			if( fs != null )
			{
				fs.Close();
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

		void DataImportOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
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
								
								if( bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format4bppIndexed )
								{
									if( m_import_tiles_form.ShowDialog() == DialogResult.OK )
									{
										m_import_tiles_form.data_processing( bmp, m_data_manager, create_layout_with_empty_screens );
										
										if( m_import_tiles_form.import_game_level )
										{
											if( m_import_tiles_form.delete_empty_screens )
											{
												if( delete_empty_screens() > 0 )
												{
													update_screens_list_box();
													
													ListBoxScreens.SelectedIndex = m_data_manager.scr_data_pos;
												}
											}
											
											update_screens( true, false );
											
											m_layout_editor.update_dimension_changes();
										}
										
										update_graphics( true );
									}
								}
								else
								{
									throw new Exception( "The imported image must have 4bpp color depth!" );
								}
								
								bmp.Dispose();
								bmp = null;
							}
							break;							
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
									palette_group.Instance.load_main_palette( br );
									
									update_graphics( true );
									update_screens( false );
									
									clear_active_tile_img();
								}
								else
								{
									throw new Exception( "The imported palette must be 192 bytes length!" );
								}
							}
							break;
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
			
			if( br != null )
			{
				br.Close();
			}
			
			if( fs != null )
			{
				br.Close();
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

		void export_tiles_blocks_data( int _max_data_cnt, Func< int, int > _act_data_sum, int _data_size, ImageList _image_list, string _filename )
		{
			update_graphics( false );
			
			// get a number of non zero tiles
			int num_active_tiles = 0;
			int i;
			
			int tile_sum;
			
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
																return ( int )m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ).tiles[ _data_n ];
									                         }, 32, m_imagelist_manager.get_tiles_image_list(), filename );
								}
								else
								{
									export_tiles_blocks_data( utils.CONST_MAX_BLOCKS_CNT, delegate( int _data_n ) 
									                         {
																tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
									                         		
									                         	int block_sum = 0;
									                         	
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
							TextWriter tw = new StreamWriter( filename );
							{
								m_data_manager.save_JSON( tw );
							}
							tw.Close();
                		}
						break;
						
					case ".asm":
						{
#if DEF_NES							
							m_exp_nes_asm.ShowDialog( filename );
#endif							
#if DEF_SMS
							m_exp_sms_asm.ShowDialog( filename );
#endif
						}
						break;
						
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
			message_box( "Game Maps Editor (" + utils.CONST_PLATFORM + ") \n\n" + utils.CONST_APP_VER + " " + utils.build_str + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}

		void MenuHelpQuickGuideClick_Event(object sender, EventArgs e)
		{
			string doc_path = Application.StartupPath.Substring( 0, Application.StartupPath.LastIndexOf( Path.DirectorySeparatorChar ) ) + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + "MAPeD" + Path.DirectorySeparatorChar + "Quick_Guide.html";
			
			//message_box( doc_path, "path", MessageBoxButtons.OK, MessageBoxIcon.Information );//!!!
			
			if( utils.is_win )
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.Start( doc_path );
			}
			else
			if( utils.is_linux )
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.Start( "xdg-open", doc_path );
			}
			else
			if( utils.is_macos )
			{
				// need to test it...
				System.Diagnostics.Process process = System.Diagnostics.Process.Start( "open", doc_path );
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
			
			m_data_manager.scr_data_pos 	= -1;
			m_data_manager.tiles_data_pos 	= chr_bank_cbox.SelectedIndex;
			
			palette_group.Instance.active_palette = 0;
			
			update_graphics( false );
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_All );
			
			// reset the screen editor controls
			{
				clear_active_tile_img();
				
				update_screens_list_box();
				
				ListBoxScreens.SelectedIndex = -1;
			}
			
			update_screens_by_bank_id( true, false );
			
			m_presets_manager_form.set_data( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) );
		}

		void BtnUpdateGFXClick_Event(object sender, EventArgs e)
		{
			update_graphics( false );
			
			clear_active_tile_img();
			
			set_status_msg( "GFX updated" );
		}
		
		private void update_graphics( bool _update_tile_processor_gfx )
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			
			m_imagelist_manager.update_blocks( CBoxTileViewType.SelectedIndex, data, PropertyPerBlockToolStripMenuItem.Checked );
			m_imagelist_manager.update_tiles( CBoxTileViewType.SelectedIndex, data );	// called after update_blocks, because it uses updated gfx of blocks to speed up drawing

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
			m_presets_manager_form.update();
			
			enable_update_gfx_btn( false );
//			enable_update_screens_btn( false );			
		}
		
		void BtnCHRBankNextPageClick_Event(object sender, EventArgs e)
		{
#if DEF_SMS			
			m_tiles_processor.CHR_bank_next_page();
#endif			
		}
		
		void BtnCHRBankPrevPageClick_Event(object sender, EventArgs e)
		{
#if DEF_SMS			
			m_tiles_processor.CHR_bank_prev_page();
#endif			
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

				CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;
				
				enable_copy_paste_action( false, ECopyPasteType.cpt_All );
				
				if( size == 0 )
				{
					enable_main_UI( false );
					
					CBoxBlockObjId.SelectedIndex = 0;
					
					ListBoxScreens.Items.Clear();
					
					update_graphics( false );
					
					set_status_msg( "No data" );
					
					set_title_name( null );
				}
				else
				{
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
		
		void select_block_button( int _btn_ind )
		{
			PanelBlocks.Controls[ _btn_ind ].Select();		
		}

		void PanelTilesClick_Event(object sender, EventArgs e)
		{
			select_tile( get_sender_index( sender ) );
		}
		
		void select_tile( int _id )
		{
			tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
			
			m_tiles_processor.tile_select_event( _id, data );
			
			update_active_tile_img( _id );
		}

		private void update_active_tile_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				// draw image into 'Active Tile'
				Image img = m_imagelist_manager.get_tiles_image_list().Images[ _ind ];
				
				m_pbox_active_tile_gfx.DrawImage( img, 0, 0, img.Width, img.Height );
				PBoxActiveTile.Invalidate();
				
				m_screen_editor.set_active_tile( _ind, img, screen_editor.EFillMode.efm_Tile );
				
				GrpBoxActiveTile.Text = "Tile: " + String.Format( "${0:X2}", _ind );
			}
		}

		private void update_active_block_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				// draw image into 'Active Tile'
				Image img = m_imagelist_manager.get_blocks_image_list().Images[ _ind ];
				
				m_pbox_active_tile_gfx.DrawImage( img, 0, 0, PBoxActiveTile.Width, PBoxActiveTile.Height );
				PBoxActiveTile.Invalidate();
				
				m_screen_editor.set_active_tile( _ind, img, screen_editor.EFillMode.efm_Block );
				
				GrpBoxActiveTile.Text = "Block: " + String.Format( "${0:X2}", _ind );
			}
		}
		
		void CBoxBlockObjIdChanged_Event(object sender, EventArgs e)
		{
			m_tiles_processor.set_block_flags_obj_id( CBoxBlockObjId.SelectedIndex, PropertyPerBlockToolStripMenuItem.Checked );
			
			if( CBoxBlockObjId.Tag == null && CBoxTileViewType.SelectedIndex == ( int )ETileViewType.tvt_ObjectId )
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
		
			if( CBoxTileViewType.SelectedIndex == ( int )ETileViewType.tvt_ObjectId )
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
			
			enable_copy_paste_action( false, ECopyPasteType.cpt_CHR_bank );
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
			}
		}
		
		void DeleteCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?", "Delete CHR", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
	
					int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
					
					if( sel_ind >= 0 )
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
			}
		}
		
		int get_context_menu_sender_index( object sender )
		{
			Control cntrl = ( ( sender as ToolStripDropDownItem ).Owner as ContextMenuStrip ).SourceControl;
				
			Button sender_btn 			= cntrl as Button;
			ListView sender_list_view 	= cntrl as ListView;

			if( sender_btn != null )
			{
				return sender_btn.ImageIndex;
			}
			
			if( sender_list_view.SelectedIndices.Count > 0 )
			{
				return sender_list_view.SelectedIndices[ 0 ];
			}
			
			return -1;
		}

		int get_sender_index( object sender )
		{
			Button sender_btn 			= sender as Button;
			ListView sender_list_view 	= sender as ListView;

			if( sender_btn != null )
			{
				return sender_btn.ImageIndex;
			}
			
			if( sender_list_view.SelectedIndices.Count > 0 )
			{
				return sender_list_view.SelectedIndices[ 0 ];
			}
			
			return -1;
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
				
				ushort 	block_val;
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

				enable_copy_paste_action( false, ECopyPasteType.cpt_Blocks_list );
				
				if( _paste_clone )
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} cloned to block #{1:X2}", m_block_copy_item_ind, _sel_ind ) );
				}
				else
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} references are copied to block #{1:X2}", m_block_copy_item_ind, _sel_ind ) );
				}

				enable_update_screens_btn( true );
				
				update_graphics( true );
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
				
				byte[] 	CHR_data = data.CHR_bank;
				ushort 	block_val;
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
					data.inc_tiles_blocks( ( byte )sel_ind );
					
					clear_active_tile_img();
					
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
						
						data.dec_tiles_blocks( ( byte )sel_ind );
						data.clear_block( data.tiles.Length - 1 );
						
						clear_active_tile_img();
						
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

				enable_copy_paste_action( false, ECopyPasteType.cpt_Tiles_list );

				set_status_msg( String.Format( "Tile List: tile #{0:X2} is copied to #{1:X2}", m_tile_copy_item_ind, sel_ind ) );
				
				enable_update_screens_btn( true );
				update_graphics( true );
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
					
					clear_active_tile_img();
	
					set_status_msg( String.Format( "Tile List: tile #{0:X2} references are cleared", sel_ind ) );
					
					enable_update_screens_btn( true );
					update_graphics( true );
				}
			}
		}
		
		void ClearAllTileToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the blocks references for all the tiles will be set to zero!", "Clear Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );
				
				data.clear_tiles();
				
				clear_active_tile_img();
				
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
					
					clear_active_tile_img();
					
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
						data.clear_tile( data.tiles.Length - 1 );
						
						clear_active_tile_img();
						
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
						// update screens images, just for testing purposes
						m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), true, CheckBoxLayoutEditorAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
						
						m_layout_editor.set_active_screen( -1 );
					}
					
					m_screen_editor.reset_last_empty_tile_ind();
					
					m_screen_editor.update_adjacent_screen_tiles();
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
			}
		}
		
		void BtnTileReserveBlocksClick_Event(object sender, EventArgs e)
		{
			int block_ind = -1;
			
			if( ( block_ind = m_tiles_processor.tile_reserve_blocks( m_data_manager ) ) >= 0 )
			{
				select_block_button( block_ind );
				
				enable_update_gfx_btn( true );
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

#if DEF_NES
		bool check_empty_screen( uint[] _tiles, byte[] _scr_data )
#elif DEF_SMS
		bool check_empty_screen( byte[] _scr_data )
#endif
		{
			int tile_n;
#if DEF_NES
			uint tile_ind;
			
			int scr_first_tile_ind	= _scr_data[ 0 ];
			
			for( tile_n = 1; tile_n < utils.CONST_SCREEN_TILES_CNT - utils.CONST_SCREEN_NUM_WIDTH_TILES; tile_n++ )
			{
				if( scr_first_tile_ind != _scr_data[ tile_n ] )
				{
					break;
				}
			}
			
			if( tile_n != utils.CONST_SCREEN_TILES_CNT - utils.CONST_SCREEN_NUM_WIDTH_TILES )
			{
				return false;
			}

			// check the last upper half of the tiles line
			int scr_block_ind	= utils.get_byte_from_uint( _tiles[ _scr_data[ 0 ] ], 0 );

			for( tile_n = utils.CONST_SCREEN_TILES_CNT - utils.CONST_SCREEN_NUM_WIDTH_TILES; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
			{
				tile_ind = _tiles[ _scr_data[ tile_n ] ];
				
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
			
#elif DEF_SMS
			int scr_first_tile_ind = _scr_data[ 0 ];
			
			for( tile_n = 1; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
			{
				if( scr_first_tile_ind != _scr_data[ tile_n ] )
				{
					break;
				}
			}
			
			if( tile_n == utils.CONST_SCREEN_TILES_CNT )
			{
				return true;
			}
#endif
			return false;
		}
		
		int delete_empty_screens()
		{
			int res = 0;

			tiles_data data	= m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos );

			int scr_n;
			
			for( scr_n = 0; scr_n < m_data_manager.scr_data_cnt; scr_n++ )
			{
#if DEF_NES
				if( check_empty_screen( data.tiles, data.scr_data[ scr_n ] ) )
#elif DEF_SMS
				if( check_empty_screen( data.scr_data[ scr_n ] ) )
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
				int deleted_screens_cnt = 0;
				
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
		
		void BtnPresetsClick_Event(object sender, EventArgs e)
		{
			m_presets_manager_form.Visible = true;
			m_presets_manager_form.update();
			
			BtnPresets.Enabled = false;
		}

		void MainForm_PresetsManagerClosed(object sender, EventArgs e)
		{
			BtnPresets.Enabled = true;
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
			
			// highlight selected block button
			if( sel_block_ind >= 0 )
			{
				select_block_button( m_tiles_processor.get_selected_block() );
			}
		}
		
		void MainForm_ResetActiveTile(object sender, EventArgs e)
		{
			clear_active_tile_img();
		}

		private void update_tile_image( object sender, EventArgs e )
		{
			NewTileEventArg event_args = e as NewTileEventArg;
			
			int tile_ind 	= event_args.tile_ind;
			tiles_data data = event_args.data;
			
			m_imagelist_manager.update_tile( tile_ind, CBoxTileViewType.SelectedIndex, data, null, null );
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
			clear_active_tile_img();
		}
		
#endregion		
// LAYOUT EDITOR *************************************************************************************//		
#region layout editor
		layout_data create_layout_with_empty_screens( int _scr_width, int _scr_height )
		{
			if( m_data_manager.layout_data_create() == true )
			{
				ListBoxLayouts.Items.Add( m_data_manager.layouts_data_cnt - 1 );
				m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_cnt - 1;
				
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
					screen_data scr_data;
					
					for( int y = 0; y < _scr_height; y++ )
					{
						for( int x = 0; x < _scr_width; x++ )
						{
							if( m_data_manager.screen_data_create() == true )
							{
								scr_global_ind = insert_screen_into_layouts( m_data_manager.scr_data_cnt - 1 );

								if( scr_global_ind >= 0 )
								{
									ListBoxScreens.Items.Add( m_data_manager.scr_data_cnt - 1 );
									m_data_manager.scr_data_pos = ListBoxScreens.SelectedIndex = m_data_manager.scr_data_cnt - 1;
								
									scr_data = layout.get_data( x, y );
									scr_data.m_scr_ind = (byte)scr_global_ind;
									layout.set_data( scr_data, x, y );
								}
								else
								{
									m_data_manager.screen_data_delete();
									
									throw new Exception( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT );
								}
							}
							else
							{
								throw new Exception( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT );
							}
						}
					}
					
				}
				
				return layout;
			}
			
			return null;
		}

		private int insert_screen_into_layouts( int _scr_local_ind )
		{
			int scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, _scr_local_ind );
			
			if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
			{
				m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, _scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data() );			
				
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
				m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), true, m_data_manager.tiles_data_pos );
				
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
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1 ? true:false; }, delegate( layout_data _data ) { return _data.remove_up(); }, "Remove Top Row" );
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
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1 ? true:false; }, delegate( layout_data _data ) { return _data.remove_down(); }, "Remove Bottom Row" );
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
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1 ? true:false; }, delegate( layout_data _data ) { return _data.remove_left(); }, "Remove Left Column" );
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
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1 ? true:false; }, delegate( layout_data _data ) { return _data.remove_right(); }, "Remove Right Column" );				
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
					if( create_layout_with_empty_screens( m_create_layout_form.layout_width, m_create_layout_form.layout_height ) != null )
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
						
						ListBoxLayouts.SelectedIndex = ListBoxLayouts.SelectedIndex + 1;
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
						
						ListBoxLayouts.SelectedIndex = ListBoxLayouts.SelectedIndex - 1;
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
			
			update_screens_by_bank_id( true, need_update_screens() ? true:false );
			
			m_layout_editor.update();
		}
		
		void update_screens_by_bank_id( bool _disable_upd_scr_btn, bool _update_images )
		{
			m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), _update_images, CheckBoxLayoutEditorAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
			
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
			
			if( m_object_name_form.ShowDialog() == DialogResult.OK )
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
			
			if( m_object_name_form.ShowDialog() == DialogResult.OK )
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
			
			TreeNode[] nodes_arr = null;
			
			nodes_arr = TreeViewEntities.Nodes.Find( ent_name, true );
			
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
			groupBoxEntityEditor.Enabled = ( _ent != null ) ? true:false;
			
			if( _ent != null )
			{
				bool edit_inst_mode = ( m_layout_editor.mode == layout_editor.EMode.em_EditInstances || m_layout_editor.mode == layout_editor.EMode.em_PickupTargetEntity ) ? true:false;

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
			
			update_entity_preview( ( _ent == null ) ? true:false );
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
					bmp = null;
				}

				if( unlocked_bmp != null )
				{
					unlocked_bmp.Dispose();
					unlocked_bmp = null;
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

		void CheckBoxPalettePerCHRChecked_Event(object sender, EventArgs e)
		{
			m_tiles_processor.set_block_editor_palette_per_CHR_mode( ( sender as CheckBox ).Checked );
		}
		
		void BtnSwapColorsClick_Event(object sender, EventArgs e)
		{
#if DEF_SMS			
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				if( m_swap_colors_form.ShowDialog( m_data_manager.get_tiles_data( m_data_manager.tiles_data_pos ) ) == DialogResult.OK )
				{
					BtnUpdateGFXClick_Event( null, null );
				}
			}
#endif
		}
	}
}