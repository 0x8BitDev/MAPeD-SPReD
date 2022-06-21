/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 13.03.2017
 * Time: 11:24
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SPReD
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private sprite_processor m_sprites_proc = null;
		
		private create_sprite_form 			m_create_sprite_form 		= new create_sprite_form();
		private rename_sprite_form 			m_rename_sprite_form 		= new rename_sprite_form();
		private copy_sprite_new_name_form 	m_new_sprite_name_form 		= new copy_sprite_new_name_form();
		private image_export_options_form 	m_img_export_options_form	= new image_export_options_form();
		private image_import_options_form 	m_img_import_options_form	= new image_import_options_form();
		private description_form			m_description_form			= new description_form();
		
#if DEF_SMS
		private SMS_sprite_flipping_form	m_SMS_sprite_flip_form		= null;
		private SMS_export_form				m_SMS_export_form			= null;
		private swap_colors_form			m_swap_colors_form			= new swap_colors_form();
#elif DEF_PCE
		private PCE_export_form				m_PCE_export_form			= null;
		private swap_colors_form			m_swap_colors_form			= new swap_colors_form();
#endif

#if DEF_FIXED_LEN_PALETTE16_ARR
		private palettes_array				m_palettes_arr				= null;
#endif
		private SPSeD.py_editor	m_py_editor	= null;
		
		private bool m_project_loaded	= false;
		
		private struct SToolTipData
		{
			public System.Windows.Forms.Control m_cntrl;
			public string m_desc;
			
			public SToolTipData( System.Windows.Forms.Control _cntrl, string _desc )
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
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_sprites_proc = new sprite_processor(	SpriteLayout,
													SpriteLayoutLabel,
													LayoutGroupBox,
													CHRBank,
													CHRBankLabel,
													PaletteMain,
													Palette0,
													Palette1,
													Palette2,
													Palette3 );
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_sprites_proc.subscribe_event( this );
#endif
			CBoxCHRPackingType.SelectedIndex = 0;
			CBoxFlipType.SelectedIndex = 0;
			
			SpriteList.ContextMenuStrip = SpriteListContextMenu;
			CHRBank.ContextMenuStrip 	= ContextMenuCHRBank;
			
			SToolTipData[] tooltips = new SToolTipData[]{ 	new SToolTipData( BtnMoveItemUp, "Move selected item up" ),
															new SToolTipData( BtnMoveItemDown, "Move selected item down" ),
															new SToolTipData( BtnCHROptimization, "Remove unused/empty/duplicate CHRs in all CHR banks" ),
															new SToolTipData( BtnCHRPack, "Merge selected sprites data into common CHR bank(s)" ),
															new SToolTipData( BtnCHRSplit, "Extract all sprites data into separate CHR banks" ),
															new SToolTipData( BtnOffset, "Apply offset to all selected sprites" ),
															new SToolTipData( Palette0, "Shift+1 / Ctrl+1,2,3,4 to select a color" ),
															new SToolTipData( Palette1, "Shift+2 / Ctrl+1,2,3,4 to select a color" ),
															new SToolTipData( Palette2, "Shift+3 / Ctrl+1,2,3,4 to select a color" ),
															new SToolTipData( Palette3, "Shift+4 / Ctrl+1,2,3,4 to select a color" ),
															new SToolTipData( BtnZoomIn, "Zoom In" ),
															new SToolTipData( BtnZoomOut, "Zoom Out" ),
															new SToolTipData( BtnAddCHR, "Add new CHR" ),
															new SToolTipData( BtnDeleteLastCHR, "Delete last CHR" ),
															new SToolTipData( BtnDeleteCHR, "Delete selected CHR" ),
															new SToolTipData( BtnCentering, "Place an active sprite in the middle of the viewport" ),
															new SToolTipData( BtnShiftColors, "Cyclic shifting of active sprite color indices" ),
															new SToolTipData( CBoxShiftTransp, "Use transparency when shifting color indices" ),
															new SToolTipData( BtnApplyDefaultPalette, "Apply palette to all selected sprites" ),
															new SToolTipData( BtnCHRVFlip, "Vertical flipping of selected CHR" ),
															new SToolTipData( BtnCHRHFlip, "Horizontal flipping of selected CHR" ),
															new SToolTipData( BtnCHRRotate, "Clockwise rotation of selected CHR" ),
															new SToolTipData( BtnVFlip, "Vertical flipping of selected CHR" ),
															new SToolTipData( BtnHFlip, "Horizontal flipping of selected CHR" ),
															new SToolTipData( BtnSpriteVFlip, "Vertical flipping of all selected sprites" ),
															new SToolTipData( BtnSpriteHFlip, "Horizontal flipping of all selected sprites" ),
															new SToolTipData( CBoxAxesLayout, "Show X/Y axes" ),
															new SToolTipData( CBoxGridLayout, "Show grid" ),
															new SToolTipData( CBoxSnapLayout, "Snap CHRs to " + utils.CONST_CHR_SIDE_PIXELS_CNT + "x" + utils.CONST_CHR_SIDE_PIXELS_CNT + " grid" ),
															new SToolTipData( CBoxMode8x16, "8x16 sprite mode" ),
#if DEF_FIXED_LEN_PALETTE16_ARR
															new SToolTipData( CBoxPalettes, "Palettes array" ),
															new SToolTipData( BtnSwapColors, "Swap two selected colors without changing graphics" ),
#endif
														};
			SToolTipData data;
			
			for( int i = 0; i < tooltips.Length; i++ )
			{
				data = tooltips[ i ];
				
				( new System.Windows.Forms.ToolTip(this.components) ).SetToolTip( data.m_cntrl, data.m_desc );
			}
			
			// the build mode is active by default
			BtnModeBuild_Event( null, null );
			
			// disable PASTE action by default
			PasteCHRToolStripMenuItem.Enabled = false;
			
			FormClosing += new System.Windows.Forms.FormClosingEventHandler( OnFormClosing );
			
			set_title_name( null );
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_palettes_arr			= new palettes_array( CBoxPalettes );
			
			Palette0Label.Location	= new Point( Palette0Label.Location.X - 13, Palette0Label.Location.Y );
			Palette0.Location		= new Point( Palette0.Location.X - 14,		Palette0.Location.Y );
			Palette2Label.Location	= new Point( Palette2Label.Location.X - 13, Palette2Label.Location.Y );
			Palette2.Location		= new Point( Palette2.Location.X - 14,		Palette2.Location.Y );

			Palette1Label.Location	= new Point( Palette1Label.Location.X - 42, Palette1Label.Location.Y );
			Palette1.Location		= new Point( Palette1.Location.X - 42,		Palette1.Location.Y );
			Palette3Label.Location	= new Point( Palette3Label.Location.X - 42, Palette3Label.Location.Y );
			Palette3.Location		= new Point( Palette3.Location.X - 42,		Palette3.Location.Y );
#else
			CBoxPalettes.Visible	= false;
			BtnSwapColors.Visible	= false;
#endif

#if DEF_NES
			this.Project_openFileDialog.DefaultExt = "spredsms";
			this.Project_openFileDialog.Filter = this.Project_openFileDialog.Filter + "|SPReD-SMS (*.spredsms)|*.spredsms";
			
			paletteToolStripMenuItem.Visible = false;
			
			this.ExportCToolStripMenuItem.Text = "&CC65";
			this.ExportC_saveFileDialog.Title = "Export CC65: Select File";
			this.ExportC_saveFileDialog.Filter = this.ExportC_saveFileDialog.Filter.Replace( "C", "CC65");
#elif DEF_SMS
			this.Project_saveFileDialog.DefaultExt = "spredsms";
			this.Project_saveFileDialog.Filter = this.Project_saveFileDialog.Filter.Replace( "NES", "SMS" );
			this.Project_saveFileDialog.Filter = this.Project_saveFileDialog.Filter.Replace( "nes", "sms" );

			this.Project_openFileDialog.DefaultExt = "spredsms";
			this.Project_openFileDialog.Filter = "SPReD-SMS (*.spredsms)|*.spredsms|" + this.Project_openFileDialog.Filter;

			this.Import_openFileDialog.Filter = this.Import_openFileDialog.Filter.Replace( "4 colors", "16/4 colors" );
			
			this.ExportASMToolStripMenuItem.Text = "&WLA-DX asm";
			this.ExportASM_saveFileDialog.Filter = "WLA-DX (*.asm)|*.asm";

			this.ExportCToolStripMenuItem.Text = "&SDCC";
			this.ExportC_saveFileDialog.Title = "Export SDCC: Select File";
			this.ExportC_saveFileDialog.Filter = this.ExportC_saveFileDialog.Filter.Replace( "C", "SDCC");
			
			BtnApplyDefaultPalette.Enabled = applyPaletteToolStripMenuItem.Enabled = false;
			BtnShiftColors.Enabled = shiftColorsToolStripMenuItem.Enabled = CBoxShiftTransp.Enabled = false;
			
			CHRFlippingGroupBox.Enabled = false;
			
			m_SMS_sprite_flip_form  = new SMS_sprite_flipping_form( SpriteList, m_sprites_proc );
			m_SMS_export_form		= new SMS_export_form();
			
			CBoxCHRPackingType.Items.Add( "8KB" );
#elif DEF_PCE
			this.Project_saveFileDialog.DefaultExt = "spredpce";
			this.Project_saveFileDialog.Filter = this.Project_saveFileDialog.Filter.Replace( "NES", "PCE" );
			this.Project_saveFileDialog.Filter = this.Project_saveFileDialog.Filter.Replace( "nes", "pce" );

			this.Project_openFileDialog.DefaultExt = "spredpce";
			this.Project_openFileDialog.Filter = "SPReD-PCE (*.spredpce)|*.spredpce";//|" + this.Project_openFileDialog.Filter;

			this.Import_openFileDialog.Filter = this.Import_openFileDialog.Filter.Replace( "4 colors", "16/4 colors" );
			this.Import_openFileDialog.Filter = this.Import_openFileDialog.Filter.Replace( "Palette (192 bytes)", "Palette (1536 bytes)" );
			
			this.ExportASMToolStripMenuItem.Text = "&CA65/PCEAS";
			this.ExportASM_saveFileDialog.Filter = "CA65/PCEAS (*.asm)|*.asm";
			
			this.ExportCToolStripMenuItem.Text = "&HuC";
			this.ExportC_saveFileDialog.Title = "Export HuC: Select File";
			this.ExportC_saveFileDialog.Filter = this.ExportC_saveFileDialog.Filter.Replace( "C", "HuC");
			
			BtnShiftColors.Enabled = shiftColorsToolStripMenuItem.Enabled = CBoxShiftTransp.Enabled = false;

			CBoxMode8x16.Visible	= false;
			
			m_PCE_export_form		= new PCE_export_form();
			
			CBoxCHRPackingType.Items.Add( "8KB" );
			CBoxCHRPackingType.Items.Add( "16KB" );
			CBoxCHRPackingType.Items.Add( "32KB" );
#endif
			palette_group.Instance.active_palette = 0;
			
			if( _args.Length > 0 )
			{
				project_load( _args[0] );
			}
			else
			{
				reset();
			}
		}
		
		public static System.Windows.Forms.DialogResult message_box( string _msg, string _title, System.Windows.Forms.MessageBoxButtons _buttons, System.Windows.Forms.MessageBoxIcon _icon = System.Windows.Forms.MessageBoxIcon.Warning )
		{
			return System.Windows.Forms.MessageBox.Show( _msg, _title, _buttons, _icon );
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
			
		    if( message_box( "All unsaved progress will be lost!\nAre you sure?", "Exit App", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
		    {
				reset();
				m_sprites_proc = null;

				m_py_editor = null;
				
		    	e.Cancel = false;
		    }
		}			

		private void set_title_name( string _msg )
		{
			Text = utils.CONST_APP_NAME + " " + utils.CONST_APP_VER + ( _msg != null ? " - " + _msg:"" );
		}
		
		private void reset()
		{
			int size = SpriteList.Items.Count;
			
			sprite_data spr;
			
			// delete sprites...
			for( int i = 0; i < size; i++ )
			{
				spr = SpriteList.Items[ i ] as sprite_data;
				m_sprites_proc.remove( spr );
			}
			
			SpriteList.Items.Clear();
			
			m_sprites_proc.reset();
			
			// the build mode is active by defailt
			BtnModeBuild_Event( null, null );
			
			OffsetX.Value = 0;
			OffsetY.Value = 0;
			
			CBoxMode8x16.Checked = false;
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			CBoxPalettes.Enabled = false;
			CBoxPalettes.SelectedIndex = 0;
#endif
		}
		
		void ExitToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			Close();
		}
		
		void MenuHelpAboutClick(object sender, System.EventArgs e)
		{
			message_box( "Sprites Editor (" + utils.CONST_PLATFORM + ")\n\n" + utils.CONST_APP_VER + " " + utils.build_str + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information );
		}
		
		void MenuHelpQuickGuideClick(object sender, EventArgs e)
		{
			string doc_path = Application.StartupPath.Substring( 0, Application.StartupPath.LastIndexOf( Path.DirectorySeparatorChar ) ) + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + "SPReD" + Path.DirectorySeparatorChar + "Quick_Guide.html";
			
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
		
		void CloseToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure?", "Close Project", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					reset();
					
					set_title_name( null );
					
					m_description_form.edit_text = "";
				}
			}
		}
		
		void DescriptionToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_description_form.ShowDialog();
		}
		
		void StatisticsToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.show_statistics( SpriteList.Items );
		}
		
//		SPRITES PROCESSING		*****************************************************************************//
#region	Sprites processing
		private bool check_duplicate( String _spr_name )
		{
			int size = SpriteList.Items.Count;
			
			for( int i = 0; i < size; i++ )
			{
				if( SpriteList.Items[ i ].ToString() == _spr_name )
				{
					return true;
				}
			}
			
			return false;			
		}
		
		private void update_selected_sprite( bool _new_sprite = false )
		{
			if( SpriteList.SelectedIndex >= 0 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndex ] as sprite_data;
				
				OffsetX.Value = spr.offset_x;
				OffsetY.Value = spr.offset_y;
				
				m_sprites_proc.update_sprite( spr, _new_sprite );
			}
		}
		
		void BtnRename_Event(object sender, EventArgs e)
		{
			rename_copy_sprite( "Rename Sprite", null );
		}
		
		void rename_copy_sprite( string _wnd_caption, Action< string, sprite_data > _copy_act )
		{
			if( SpriteList.SelectedIndices.Count == 0 )
			{
				message_box( "Please, select a sprite!", _wnd_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				return;
			}
			
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				m_rename_sprite_form.edit_str = ( SpriteList.SelectedItem as sprite_data ).name;
				
				m_rename_sprite_form.Text = _wnd_caption;
				
				if( m_rename_sprite_form.ShowDialog() == DialogResult.Cancel )
				{
					return;
				}
				
				if( m_rename_sprite_form.edit_str == "" )
				{
					message_box( "The sprite name is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
					return;
				}
				
				string new_sprite_name = m_rename_sprite_form.edit_str;
				
				sprite_data spr = null;
				
				if(!check_duplicate( new_sprite_name ) )
				{
					if( _copy_act != null )
					{
						spr = SpriteList.SelectedItem as sprite_data;
						
						_copy_act( new_sprite_name, spr );
						
						m_sprites_proc.rearrange_CHR_data_ids();
						
						m_sprites_proc.update_sprite( spr, false );
					}
					else
					{
						SpriteList.BeginUpdate();
						{
							spr = ( SpriteList.SelectedItem as sprite_data );
							spr.name = new_sprite_name;
							
							SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] = spr;
						}
						SpriteList.EndUpdate();
					}
				}
				else
				{
					message_box( new_sprite_name + " - A sprite with the same name already exists! Ignored!", _wnd_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
			else
			{
				message_box( "Please, select one sprite!", _wnd_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
		void BtnCreateRef_Event(object sender, System.EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( SpriteList.SelectedIndices.Count == 1 )
				{
					rename_copy_sprite( "Create Ref", delegate( string _name, sprite_data _spr )
	                {
						SpriteList.Items.Add( _spr.copy( _name, null, null ) );
	                } );
				}
				else
				{
					m_new_sprite_name_form.Text = "Create Ref";
					
					sprite_names_processing( delegate( string _name, sprite_data _spr, int _ind )
	                {
						SpriteList.Items.Add( _spr.copy( _name, null, null ) );
	                });
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Create Ref", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}

		void BtnCreateCopy_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( SpriteList.SelectedIndices.Count == 1 )
				{
					rename_copy_sprite( "Create Copy", delegate( string _name, sprite_data _spr )
	                {
						SpriteList.Items.Add( _spr.copy( _name, 
						                                 m_sprites_proc.extract_and_create_CHR_data_group( _spr, CBoxMode8x16.Checked ), 
														 m_sprites_proc.extract_and_create_CHR_data_attr( _spr, CBoxMode8x16.Checked ) ) );
	                });
				}
				else
				{
					m_new_sprite_name_form.Text = "Create Copy";
					
					sprite_names_processing( delegate( string _name, sprite_data _spr, int _ind )
	                {
						SpriteList.Items.Add( _spr.copy( _name, 
						                                 m_sprites_proc.extract_and_create_CHR_data_group( _spr, CBoxMode8x16.Checked ), 
														 m_sprites_proc.extract_and_create_CHR_data_attr( _spr, CBoxMode8x16.Checked ) ) );
	                });
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Create Copy", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}		

		void BtnAddPrefixPostfix_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				m_new_sprite_name_form.Text = "Add Prefix\\Postfix";
				
				SpriteList.BeginUpdate();
				{
					sprite_names_processing( delegate( string _name, sprite_data _spr, int _spr_ind ) 
                    { 
						_spr.name = _name; 
						SpriteList.Items[ _spr_ind ] = _spr;
					});
				}
				SpriteList.EndUpdate();
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Add Prefix\\Postfix", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
		void sprite_names_processing( Action< string, sprite_data, int > _act )
		{
			if( m_new_sprite_name_form.ShowDialog() == DialogResult.Cancel )
			{
				return;
			}
			
			if( m_new_sprite_name_form.edit_str == "" )
			{
				MainForm.message_box( "The Prefix\\Postfix field is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				return;
			}
			
			string new_name;
			string pref_postf = m_new_sprite_name_form.edit_str;
			
			bool postfix = m_new_sprite_name_form.is_postfix_selected();
			
			sprite_data spr;
			
			int[] sel_inds = new int[ SpriteList.SelectedIndices.Count ];
			
			SpriteList.SelectedIndices.CopyTo( sel_inds, 0 );
			
			for( int i = 0; i < sel_inds.Length; i++ )
			{
				spr = SpriteList.Items[ sel_inds[ i ] ] as sprite_data;
				
				new_name = postfix ? ( spr.name + pref_postf ):( pref_postf + spr.name );
				
				if( !check_duplicate( new_name ) )
				{
					_act( new_name, spr, sel_inds[ i ] );
				}
				else
				{
					message_box( new_name + " - A sprite with the same name already exists! Ignored!", "Copy Sprite", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
			
			m_sprites_proc.rearrange_CHR_data_ids();
			
			m_sprites_proc.update_sprite( SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data, false );
		}
		
		void BtnDelete_Event(object sender, System.EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to delete " + SpriteList.SelectedIndices.Count + " sprite(s)?", "Delete Selected Sprite(s)", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					if( !check_selected_sprites_data( "Delete Selected Sprite(s)" ) )
					{
						return;
					}
					
					sprite_data spr;

					int spr_CHR_id;
					int i;
					
					List< sprite_data >	sprites = new List< sprite_data >( SpriteList.SelectedIndices.Count );
					
					for( i = 0; i < SpriteList.SelectedIndices.Count; i++ )
					{
						sprites.Add( SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data );				
					}
					
					SpriteList.BeginUpdate();

					m_sprites_proc.CHR_bank_optimization_begin();
					
					for( i = 0; i < sprites.Count; i++ )
					{
						spr = sprites[ i ];
						
						SpriteList.Items.Remove( spr );
						
						spr_CHR_id = spr.get_CHR_data().id;
						
						m_sprites_proc.remove( spr );
						
						m_sprites_proc.CHR_bank_optimization( spr_CHR_id, SpriteList.Items, CBoxMode8x16.Checked );
					}
					
					m_sprites_proc.CHR_bank_optimization_end( false );
					
					SpriteList.EndUpdate();
					
					SpriteList.SelectedIndices.Clear();
					
					if( SpriteList.Items.Count == 0 )
					{
						reset();
						
						set_title_name( null );
					}
					else
					{
						m_sprites_proc.update_sprite( null );
					}
					
					m_sprites_proc.rearrange_CHR_data_ids();
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Delete Sprite(s)", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
		void BtnApplyDefaultPalette_Event(object sender, System.EventArgs e)
		{
			int size = SpriteList.SelectedIndices.Count;
			
			if( size > 0 )
			{
				sprite_data spr;
				
				for( int i = 0; i < size; i++ )
				{
					spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;
					
					if( m_sprites_proc.apply_active_palette( spr ) == false )
					{
						message_box( "Please, select an active palette!", "Apply Default Palette", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
						break;
					}
				}
				
				update_selected_sprite( false );
			}
		}
		
		void BtnOffset_Event(object sender, System.EventArgs e)
		{
			int size = SpriteList.SelectedIndices.Count;
			
			if( size > 0 )
			{
				bool add_offset = false;
				
			    System.Windows.Forms.DialogResult dlg_res = message_box( "[Yes] - SET new value\n[No] - ADD offset", "Set Offset", System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Question );
			    
			    if( dlg_res == System.Windows.Forms.DialogResult.Cancel )
			    {
			    	return;
			    }
			    
			    if( dlg_res == System.Windows.Forms.DialogResult.No )
			    {
			    	add_offset = true;
			    }
				
				sprite_data spr;
				
				for( int i = 0; i < size; i++ )
				{
					spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;
					
					if( add_offset == true )
					{
						spr.offset_x += ( int )OffsetX.Value;
						spr.offset_y += ( int )OffsetY.Value;
					}
					else
					{
						spr.offset_x = ( int )OffsetX.Value;
						spr.offset_y = ( int )OffsetY.Value;
					}
				}
				
				update_selected_sprite( false );
			}
		}
		
		void BtnVFlip_Event(object sender, System.EventArgs e)
		{
			if( m_sprites_proc.layout_get_mode() == sprite_layout_viewer.EMode.m_build )
			{
				if( SpriteList.SelectedIndices.Count > 0 )
				{
					m_sprites_proc.flip_selected_CHR( CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP );
				}
				else
				{
					message_box( "Please, select a CHR!", "Vertical Flipping", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnHFlip_Event(object sender, System.EventArgs e)
		{
			if( m_sprites_proc.layout_get_mode() == sprite_layout_viewer.EMode.m_build )
			{
				if( SpriteList.SelectedIndices.Count > 0 )
				{
					m_sprites_proc.flip_selected_CHR( CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP );
				}
				else
				{
					message_box( "Please, select a CHR!", "Horizontal Flipping", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnSelectAll_Event(object sender, System.EventArgs e)
		{
			int size = SpriteList.Items.Count;
			
			for( int i = 0; i < size; i++ )
			{
				SpriteList.SetSelected( i, true );
			}
		}
		
		void BtnUp_Event(object sender, System.EventArgs e)
		{
			move_item( delegate( int _ind ) { return _ind-1 < 0 ? 0:_ind-1; } );
		}
		
		void BtnDown_Event(object sender, System.EventArgs e)
		{
			move_item( delegate( int _ind ) { return _ind+1 > SpriteList.Items.Count ? SpriteList.Items.Count:_ind+1; } );
		}
		
		void move_item( Func< int, int > _act )
		{
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				int sel_ind 	= SpriteList.SelectedIndex;
				sprite_data spr = SpriteList.Items[ sel_ind ] as sprite_data;
				
				SpriteList.Items.RemoveAt( SpriteList.SelectedIndex );
				
				sel_ind = _act( sel_ind );
					
				SpriteList.Items.Insert( sel_ind, spr );
				
				SpriteList.SetSelected( sel_ind, true );
			}
		}
		
		void SpriteListItemClick_Event(object sender, System.EventArgs e)
		{
			if( SpriteList.SelectedIndex >= 0 )
			{
				update_selected_sprite( true );
#if DEF_FIXED_LEN_PALETTE16_ARR
				CBoxPalettes.Enabled = true;
#endif
			}
			else
			{
				sprite_data spr = null;
				m_sprites_proc.update_sprite( spr, true );
				
				OffsetX.Value = 0;
				OffsetY.Value = 0;
#if DEF_FIXED_LEN_PALETTE16_ARR
				CBoxPalettes.Enabled = false;
#endif
			}
		}
		
		void BtnCreate_Event(object sender, EventArgs e)
		{
#if DEF_PCE
			m_create_sprite_form.Text = "Create Sprite";
#elif DEF_NES || DEF_SMS
			m_create_sprite_form.Text = "Create Sprite [ mode: " + ( CBoxMode8x16.Checked ? "8x16":"8x8" ) + " ]";
#endif
			if( !check_all_sprites_data( m_create_sprite_form.Text ) )
			{
				return;
			}
			
			if( m_create_sprite_form.ShowDialog() == DialogResult.Cancel )
			{
				return;
			}
			
			if( CBoxMode8x16.Checked && ( m_create_sprite_form.sprite_height&0x01 ) == 1 )
			{
				MainForm.message_box( "The sprite height must be an even number!", "8x16 Mode Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				return;
			}
			
			if( m_create_sprite_form.edit_str == "" )
			{
				MainForm.message_box( "The sprite name is empty!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				return;
			}
			
			string new_sprite_name = m_create_sprite_form.edit_str;
			
			if( !check_duplicate( new_sprite_name ) )
			{
				SpriteList.Items.Add( m_sprites_proc.create_sprite( new_sprite_name, m_create_sprite_form.sprite_width, m_create_sprite_form.sprite_height, CBoxMode8x16.Checked ) );
				
				select_last_sprite();
			}
			else
			{
				message_box( new_sprite_name + " - A sprite with the same name already exists! Ignored!", "Sprite Creating Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}

		void select_last_sprite()
		{
			SpriteList.ClearSelected();
			SpriteList.SetSelected( SpriteList.Items.Count - 1, true );
			
			m_sprites_proc.layout_sprite_centering();
			
			update_selected_sprite( true );
		}
#endregion
//		LAYOUT		*****************************************************************************************//		
#region	Layout		
		void CBoxAxisLayoutCheckedChanged_Event(object sender, EventArgs e)
		{
			m_sprites_proc.set_sprite_layout_viewer_flags( CBoxAxesLayout.Checked, CBoxGridLayout.Checked );
		}
		
		void CBoxGridLayoutCheckedChanged_Event(object sender, EventArgs e)
		{
			m_sprites_proc.set_sprite_layout_viewer_flags( CBoxAxesLayout.Checked, CBoxGridLayout.Checked );
		}
		
		void CBoxSnapLayoutCheckedChanged_Event(object sender, EventArgs e)
		{
			System.Windows.Forms.CheckBox cbox = sender as CheckBox;

			m_sprites_proc.layout_snap( cbox.Checked );
		}
		
		void CBox8x16ModeCheckedChanged_Event(object sender, EventArgs e)
		{
			m_sprites_proc.set_mode8x16( ( sender as CheckBox ).Checked );
		}

		void BtnCenteringClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.layout_sprite_centering();
		}
		
		void BtnZoomInClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.layout_zoom_in();
		}
		
		void BtnZoomOutClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.layout_zoom_out();
		}
		
		void BtnSpriteVFlip_Event(object sender, EventArgs e)
		{
#if DEF_NES
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_vert( ( sprite_data.EAxesFlipType )( CBoxFlipType.SelectedIndex ), CBoxMode8x16.Checked ); } );
#elif DEF_SMS
			flip_sprites( "Vertical Flipping", true );
#elif DEF_PCE
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_vert( ( sprite_data.EAxesFlipType )( CBoxFlipType.SelectedIndex ) ); } );
#endif
		}
		
		void BtnSpriteHFlip_Event(object sender, EventArgs e)
		{
#if DEF_NES || DEF_PCE
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_horiz( ( sprite_data.EAxesFlipType )( CBoxFlipType.SelectedIndex ) ); } );
#elif DEF_SMS
			flip_sprites( "Horizontal Flipping", false );
#endif
		}

#if DEF_NES || DEF_PCE
		private void flip_sprites( Action< sprite_data > _act )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				sprite_data spr;
				
				for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
				{
					spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;
					
					_act( spr );
				}
				
				// update data in the layout viewport
				update_selected_sprite( false );
			}
		}
#elif DEF_SMS
		private void flip_sprites( string _title, bool _vert_flip )
		{
			if( SpriteList.SelectedItems.Count > 0 )
			{
				if( !check_selected_sprites_data( _title ) )
				{
					return;
				}
				
				m_SMS_sprite_flip_form.ShowDialog( _title, _vert_flip, CBoxMode8x16.Checked, ( sprite_data.EAxesFlipType )CBoxFlipType.SelectedIndex );

				if( m_SMS_sprite_flip_form.copy_CHR_data )
				{
					m_sprites_proc.CHR_bank_optimization_begin();
					
					for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
					{
						m_sprites_proc.CHR_bank_optimization( ( SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data ).get_CHR_data().id, SpriteList.Items, CBoxMode8x16.Checked );
					}
					
					m_sprites_proc.rearrange_CHR_data_ids();
					
					m_sprites_proc.CHR_bank_optimization_end( false );
				}
				
				// update data in the layout viewport
				update_selected_sprite( false );
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Sprite(s) Flipping Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
#endif
		void BtnShiftColors_Event(object sender, EventArgs e)
		{
#if DEF_NES
			if( SpriteList.SelectedIndex >= 0 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndex ] as sprite_data;
				
				spr.shift_colors( CBoxShiftTransp.Checked, CBoxMode8x16.Checked );
				m_sprites_proc.update_sprite( spr, false );
			}
#endif
		}

		void BtnModeBuild_Event(object sender, EventArgs e)
		{
			GroupBoxModeName.Text = "Mode: Build";
			
			drawModeToolStripMenuItem.Enabled = BtnLayoutModeDraw.Enabled 	= true;
			buildModeToolStripMenuItem.Enabled = BtnLayoutModeBuild.Enabled = false;
			
			horizontalFlippingToolStripMenuItem1.Enabled = BtnHFlip.Enabled	= true;
			verticalFlippingToolStripMenuItem1.Enabled = BtnVFlip.Enabled	= true;

			deleteCHRToolStripMenuItem.Enabled = BtnDeleteCHR.Enabled		= true;
			
			CBoxSnapLayout.Enabled = true;
			
			BtnLayoutModeDraw.Focus();
			
			m_sprites_proc.layout_set_mode( sprite_layout_viewer.EMode.m_build );
		}

		void BtnModeDraw_Event(object sender, EventArgs e)
		{
			GroupBoxModeName.Text = "Mode: Draw";
			
			drawModeToolStripMenuItem.Enabled = BtnLayoutModeDraw.Enabled 	= false;
			buildModeToolStripMenuItem.Enabled = BtnLayoutModeBuild.Enabled = true;
			
			horizontalFlippingToolStripMenuItem1.Enabled = BtnHFlip.Enabled	= false;
			verticalFlippingToolStripMenuItem1.Enabled = BtnVFlip.Enabled	= false;
			
			deleteCHRToolStripMenuItem.Enabled = BtnDeleteCHR.Enabled		= false;
			
			CBoxSnapLayout.Enabled = false;
			
			BtnLayoutModeBuild.Focus();
			
			m_sprites_proc.layout_set_mode( sprite_layout_viewer.EMode.m_draw );
		}
#endregion		
//		CHR DATA PACKING, SPLITTING, OPTIMIZATION		*****************************************************//
#region CHR data packing, splitting, optimization	
		void BtnCHRSplit_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to split the CHR data?\n\nAll sprite groups with selected sprite(s) will be splitted!\n\nWARNING: ALL sprites, including Ref ones, will have their own unique CHR banks!", "CHR Data Splitting", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					if( check_selected_sprites_data( "CHR Data Splitting" ) )
					{
						sprite_data spr;
						sprite_data sel_spr;
						
						int spr_n;
						int sel_spr_n;

						int sel_spr_bank_ind;
						
						int sprites_cnt		= SpriteList.Items.Count;
						int sel_sprites_cnt	= SpriteList.SelectedIndices.Count;
						
						for( sel_spr_n = 0; sel_spr_n < sel_sprites_cnt; sel_spr_n++ )
						{
							sel_spr = SpriteList.Items[ SpriteList.SelectedIndices[ sel_spr_n ] ] as sprite_data;
							
							sel_spr_bank_ind = sel_spr.get_CHR_data().id;
							
							for( spr_n = 0; spr_n < sprites_cnt; spr_n++ )
							{
								spr = SpriteList.Items[ spr_n ] as sprite_data;
								
								if( spr.is_packed( CBoxMode8x16.Checked ) && spr.get_CHR_data().id == sel_spr_bank_ind )
								{
									m_sprites_proc.split_CHR( spr, CBoxMode8x16.Checked );
								}
							}
						}
						
						m_sprites_proc.rearrange_CHR_data_ids();
						
						// update data in the layout viewport
						update_selected_sprite( false );
					}
				}
			}
			else
			{
				message_box( "No data!", "CHR Data Splitting", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
		void BtnCHRPack_Event(object sender, EventArgs e)
		{
			if( CBoxCHRPackingType.SelectedIndex ==  0 )
			{
				message_box( "Please, select data block size!", "CHR Data Packing", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				
				return;
			}
			
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to pack the selected sprites?\n\nAlready packed sprites will be ignored!\n\nWARNING: Irreversible operation for Ref sprites!", "CHR Data Packing", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					if( !check_selected_sprites_data( "CHR Data Packing" ) )
					{
						return;
					}
					
					int i;
					int j;
					int k;
					
					sprite_data spr_1;
					sprite_data spr_2;
					sprite_data spr_3;
					
					CHR_data_group chr_1;
					CHR_data_group chr_2;
					CHR_data_group new_chr;
					
					int m;
					int size;
					int spr_1_last_CHR_cnt;
					
					SPReD.CHR_data_group.ECHRPackingType packing_type = ( SPReD.CHR_data_group.ECHRPackingType )( CBoxCHRPackingType.SelectedIndex );
					
					System.Windows.Forms.ListBox.SelectedIndexCollection tmp_inds_list = SpriteList.SelectedIndices;
					
//					m_sprites_proc.CHR_bank_optimization_begin();
					
					for( i = 0; i < tmp_inds_list.Count; i++ )
					{
						spr_1 = SpriteList.Items[ tmp_inds_list[ i ] ] as sprite_data;
						chr_1 = spr_1.get_CHR_data();
						
						for( j = i+1; j < tmp_inds_list.Count; j++ )
						{
							spr_2 = SpriteList.Items[ tmp_inds_list[ j ] ] as sprite_data;
							chr_2 = spr_2.get_CHR_data();

							spr_1_last_CHR_cnt = spr_1.get_CHR_data().get_data().Count;
							
							if( CHR_data_group.can_merge( chr_1, chr_2, packing_type ) )
							{
								if( m_sprites_proc.merge_CHR( spr_1, spr_2, packing_type, CBoxMode8x16.Checked ) )
								{
									new_chr = spr_1.get_CHR_data();
									
									for( k = j+1; k < tmp_inds_list.Count; k++ )
									{
										spr_3 = SpriteList.Items[ tmp_inds_list[ k ] ] as sprite_data;
										
										// if spr_3 and spr_2 have the same bank it means that spr_3 is REF
										// fix CHR indices in attributes data
										if( spr_3.get_CHR_data().id == chr_2.id )
										{
											size = spr_3.get_CHR_attr().Count;
											
											for( m = 0; m < size; m++ )
											{
												spr_3.get_CHR_attr()[ m ].CHR_ind += spr_1_last_CHR_cnt; 
											}
										}
										
										m_sprites_proc.relink_CHR_data( spr_3, new_chr, chr_1.id );
										m_sprites_proc.relink_CHR_data( spr_3, new_chr, chr_2.id );
									}
									
									tmp_inds_list.Remove( tmp_inds_list[ j ] );
								
									--j;
									
//									m_sprites_proc.CHR_bank_optimization( spr_1.get_CHR_data().id, SpriteList.Items, CBoxMode8x16.Checked );
								}
							}
						}
					}

					m_sprites_proc.rearrange_CHR_data_ids();
					
					// update data in the layout viewport
					update_selected_sprite( false );
					
//					m_sprites_proc.CHR_bank_optimization_end( true, "Packing Statistics" );
				}
			}
			else
			{
				if( SpriteList.Items.Count > 0 )
				{
					message_box( "Please, select sprites!", "CHR Data Packing", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "CHR Data Packing", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
		}

		void BtnCHROpt_Event(object sender, EventArgs e)
		{
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure you want to optimize all sprites data?\n\nWARNING: All unused/empty/duplicate CHRs will be lost!", "CHR Data Optimization", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					if( check_all_sprites_data( "CHR Data Optimization" ) )
					{
						m_sprites_proc.CHR_bank_optimization_begin();
						
						for( int i = 0; i < m_sprites_proc.get_CHR_banks().Count; i++ )
						{
							m_sprites_proc.CHR_bank_optimization( m_sprites_proc.get_CHR_banks()[ i ].id, SpriteList.Items, CBoxMode8x16.Checked );
						}
						
						m_sprites_proc.rearrange_CHR_data_ids();
					
						update_selected_sprite( false );
						
						m_sprites_proc.CHR_bank_optimization_end( true );
					}
				}
			}
			else
			{
				message_box( "No data!", "CHR Data Optimization", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
		void CBoxCHRPackingType_ChangedEvent(object sender, EventArgs e)
		{
			System.Windows.Forms.ComboBox cbox = sender as System.Windows.Forms.ComboBox;
			
			CHRPackToolStripMenuItem.Enabled = BtnCHRPack.Enabled = ( cbox.SelectedIndex != 0 ) ? true:false;
		}
		
		private bool check_all_sprites_data( string _wnd_title )
		{
			sprite_data	spr;
			
			for( int i = 0; i < SpriteList.Items.Count; i++ )
			{
				spr = SpriteList.Items[ i ] as sprite_data;

				if( !spr.check( CBoxMode8x16.Checked, _wnd_title ) )
				{
					return false;
				}
			}
			
			return true;
		}

		private bool check_selected_sprites_data( string _wnd_title )
		{
			sprite_data	spr;
			
			for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
			{
				spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;

				if( !spr.check( CBoxMode8x16.Checked, _wnd_title ) )
				{
					return false;
				}
			}
			
			return true;
		}
		
#endregion		
//		CHR TOOLS	*****************************************************************************************//
#region CHR tools		
		void BtnAddCHRClick_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data;
				
				m_sprites_proc.add_last_CHR( spr );
			}
		}
		
		void BtnDeleteLastCHRClick_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data;
				
				if( message_box( "Are you sure you want to delete the last CHR?", "Delete Last CHR", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					m_sprites_proc.delete_last_CHR( spr );
					
					// check all sprites with the same CHR index
					int size = SpriteList.Items.Count;
					
					sprite_data verif_spr = null;
					
					for( int i = 0; i < size; i++ )
					{
						verif_spr = SpriteList.Items[ i ] as sprite_data;
						
						if( verif_spr != spr && verif_spr.get_CHR_data().id == spr.get_CHR_data().id )
						{
							verif_spr.validate();
						}
					}
				}
			}
		}
		
		void BtnDeleteCHR_Event(object sender, EventArgs e)
		{
			if( m_sprites_proc.layout_get_mode() == sprite_layout_viewer.EMode.m_build )
			{
				m_sprites_proc.layout_delete_CHR();
			}
		}
		
		void BtnCHREditorVFlipClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.chr_transform( CHR_data.ETransform.t_vflip );
		}
		
		void BtnCHREditorHFlipClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.chr_transform( CHR_data.ETransform.t_hflip );
		}
		
		void BtnCHREditorRotateClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.chr_transform( CHR_data.ETransform.t_rotate );
		}
		
		void FillWithColorToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_sprites_proc.CHR_fill_with_color() == false )
			{
				message_box( "Please, select an active palette and a CHR!", "Fill With Color", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void CopyCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_sprites_proc.CHR_copy() == false )
			{
				message_box( "Please, select a CHR!", "Copy CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				PasteCHRToolStripMenuItem.Enabled = true;
			}
		}
		
		void PasteCHRToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			if( m_sprites_proc.CHR_paste() == false )
			{
				message_box( "Please, select a CHR to paste!", "Paste CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
#endregion		
//		PROJECT: LOAD, SAVE, IMPORT, EXPORT		*************************************************************//
#region project: load, save export, import
		void LoadToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			m_project_loaded = false;
			
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure you want to close the current project?", "Load Project", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
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
					m_description_form.ShowDialog();
				}
			}
		}

		void SaveToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			if( SpriteList.Items.Count == 0 )			
			{
				message_box( "There is no data to save!", "Project Saving Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				Project_saveFileDialog.ShowDialog();
			}
		}
		
		void ProjectSave_OK(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// SAVE PROJECT...
			System.String filename = ( ( System.Windows.Forms.FileDialog )sender ).FileName;
		
			FileStream 		fs = null;
			BinaryWriter 	bw = null;
			
			try
			{
				fs = new FileStream( filename, FileMode.Create, FileAccess.Write );
				{
					bw = new BinaryWriter( fs );
					bw.Write( utils.CONST_PROJECT_FILE_MAGIC );
					bw.Write( utils.CONST_PROJECT_FILE_VER );

					m_sprites_proc.save_CHR_storage_and_palette( bw );
					
					uint flags = ( uint )( ( CBoxMode8x16.Checked ? 1:0 ) | ( CBoxGridLayout.Checked ? 2:0 ) | ( CBoxAxesLayout.Checked ? 4:0 ) ) | utils.CONST_PROJECT_FILE_PALETTE_FLAG;
					bw.Write( flags );
					
					int n_sprites = SpriteList.Items.Count;
					bw.Write( n_sprites );
					
					for( int i = 0; i < n_sprites; i++ )
					{
						( SpriteList.Items[ i ] as sprite_data ).save( bw );
					}
					
					palette_group.Instance.save_main_palette( bw );
#if DEF_FIXED_LEN_PALETTE16_ARR
					palettes_array.Instance.save( bw );
#endif					
					// save description
					bw.Write( m_description_form.edit_text );
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( filename ) );
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Project Saving Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
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
		
		void ProjectLoad_OK(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// LOAD PROJECT...
			project_load( ( ( System.Windows.Forms.FileDialog )sender ).FileName );
		}

		void project_load( string _filename )
		{
			reset();
			
			FileStream 		fs = null;
			BinaryReader 	br = null;
			
			try
			{
				fs = new FileStream( _filename, FileMode.Open, FileAccess.Read );
				{
					br = new BinaryReader( fs );
					if( br.ReadUInt32() == utils.CONST_PROJECT_FILE_MAGIC )
					{
						byte ver = br.ReadByte();
						
						if( ver == utils.CONST_PROJECT_FILE_VER )
						{
							int[] plt_small = new int[ 16 ];
							int[] plt_main	= new int[ palette_group.Instance.main_palette.Length ];
#if DEF_NES								
							bool ignore_palette = ( Path.GetExtension( _filename ) == ".spredsms" ) ? true:false;
#elif DEF_SMS
							bool ignore_palette = ( Path.GetExtension( _filename ) == ".sprednes" ) ? true:false;
#elif DEF_PCE
							bool ignore_palette = false;
#endif								
							m_sprites_proc.load_CHR_storage_and_palette( br, ignore_palette );
							
							if( ignore_palette )
							{
								// load "ignored" pallete into temporary buffer
								int data_pos = 0;
							
								do
								{
									plt_small[ data_pos ] = br.ReadInt32();
								}
								while( ++data_pos != plt_small.Length );
#if DEF_NES
								plt_small[ 4 ] = plt_small[ 8 ] = plt_small[ 12 ] = plt_small[ 0 ];
#endif
							}
							
							uint flags = br.ReadUInt32();
							CBoxMode8x16.Checked 	= ( ( flags&0x01 ) == 0x01 ? true:false );
							CBoxGridLayout.Checked	= ( ( flags&0x02 ) == 0x02 ? true:false );
							CBoxAxesLayout.Checked	= ( ( flags&0x04 ) == 0x04 ? true:false );
							
							int n_sprites = br.ReadInt32();
							
							for( int i = 0; i < n_sprites; i++ )
							{
								SpriteList.Items.Add( m_sprites_proc.load_sprite( br ) );
							}
							
							if( ( flags&utils.CONST_PROJECT_FILE_PALETTE_FLAG ) == utils.CONST_PROJECT_FILE_PALETTE_FLAG )
							{
								if( ignore_palette )
								{
									if( message_box( "Convert colors?", "Load Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
									{
										// load main palette from the project file
										int data_pos = 0;
									
										do
										{
											plt_main[ data_pos ] = br.ReadByte() << 16 | br.ReadByte() << 8 | br.ReadByte();
										}
										while( ++data_pos != plt_main.Length );
										
										m_sprites_proc.convert_colors( plt_main, plt_small, SpriteList, CBoxMode8x16.Checked );
									}
									else
									{
										br.ReadBytes( plt_main.Length * 3 );
									}
								}
								else
								{
									palette_group.Instance.load_main_palette( br );
#if DEF_FIXED_LEN_PALETTE16_ARR
									palettes_array plt_arr = palettes_array.Instance;
									
									plt_arr.load( br );
									plt_arr.update_palette();
#endif
								}
							}
							
							update_selected_color();
							
							// Load description
							m_description_form.edit_text = br.ReadString();
#if DEF_SMS
							if( ignore_palette )
							{
								message_box( "In order to fix flipped NES sprites issue you can select the broken sprites and flip them again by pressing the \"VFlip\"\\\"HFlip\" button in the \"Sprite List\" area. Perform flipping with unchecked \"Transform positions\" option.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
							}
#endif
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
				}

				// select a first sprite
				if( SpriteList.Items.Count > 0 )
				{
					SpriteList.SetSelected( 0, true );
					
					m_sprites_proc.layout_sprite_centering();
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( _filename ) );
				
				m_project_loaded = true;
			}
			catch( Exception _err )
			{
				reset();
				
				message_box( _err.Message, "Project Loading Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
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

		void ImportToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			Import_openFileDialog.ShowDialog();
		}

		void Import_OK(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.String[] filenames = ( ( System.Windows.Forms.FileDialog )sender ).FileNames;
			
			FileStream 		fs = null;
			BinaryReader 	br = null;
			
			try
			{
				int i;
				int j;
				int size = filenames.Length;
	
				string spr_name;
				string filename;
				
				string ext = Path.GetExtension( filenames[ 0 ] );
				
				sprite_data spr = null;
				
				bool apply_palette 	= false;
				bool crop_images 	= false;
				int	palette_slot	= -1;
				
				if( ext == ".bmp" || ext == ".png" )
				{
					if( CBoxMode8x16.Checked )
					{
						throw new Exception( "At the moment, data import is only supported for 8x8 sprites!\n\nSwitch to 8x8 mode and try again!" );
					}

					if( m_img_import_options_form.ShowDialog() == DialogResult.Cancel )
					{
						return;
					}
					
					apply_palette	= m_img_import_options_form.apply_palette;
					crop_images		= m_img_import_options_form.crop_by_alpha;
					palette_slot	= m_img_import_options_form.palette_slot;
				}

				SpriteList.BeginUpdate();
				
				switch( ext )
				{
					case ".pal":
						{
							int plt_len_bytes = palette_group.Instance.main_palette.Length * 3;
							
							fs = new FileStream( filenames[ 0 ], FileMode.Open, FileAccess.Read );
							
							br = new BinaryReader( fs );
							
							if( br.BaseStream.Length == plt_len_bytes )
							{
								palette_group.Instance.load_main_palette( br );
							}
							else
							{
								throw new Exception( "The imported palette must be " + plt_len_bytes + " bytes long!" );
							}
						}
						break;
			
					default:
						{
							for( i = 0; i < size; i++ )
							{
								filename = filenames[ i ];
								
								ext = Path.GetExtension( filename );								
			
								spr_name = System.IO.Path.GetFileNameWithoutExtension( filename );
								
								switch( ext )
								{
									case ".bmp":
									case ".png":
										{
											if( !check_duplicate( spr_name ) )
											{
												if( ext == ".png" )
												{
													spr = m_sprites_proc.load_sprite_png( filename, spr_name, apply_palette, crop_images, palette_slot );
												}
												else
												{	// otherwise - .bmp
													spr = m_sprites_proc.load_sprite_bmp( filename, spr_name, apply_palette, palette_slot );
												}												
#if DEF_NES
												if( apply_palette )
												{
													m_sprites_proc.apply_active_palette( spr );
												}
#endif												
												SpriteList.Items.Add( spr );
											}
											else
											{
												throw new Exception( spr_name + " already exists in the sprite list! Ignored!" );
											}
										}
										break;
										
									case ".chr":
									case ".bin":
										{
											fs = new FileStream( filename, FileMode.Open, FileAccess.Read );
											
											br = new BinaryReader( fs );
					
											if( br.BaseStream.Length > ( utils.CONST_CHR_NATIVE_SIZE_IN_BYTES * utils.CONST_CHR_BANK_MAX_SPRITES_CNT ) )
											{
												j = 0;
												
												while( br.BaseStream.Position < br.BaseStream.Length - 1 )
												{
													if( !check_duplicate( spr_name + "_" + j ) )
													{
														SpriteList.Items.Add( m_sprites_proc.import_CHR_bank( br, spr_name + "_" + j ) );
														
														++j;
													}
													else
													{
														throw new Exception( spr_name + " already exists in the sprite list! Ignored!" );
													}
												}
											}
											else
											{
												if( !check_duplicate( spr_name ) )
												{
													SpriteList.Items.Add( m_sprites_proc.import_CHR_bank( br, spr_name ) );
												}
												else
												{
													throw new Exception( spr_name + " already exists in the sprite list! Ignored!" );
												}
											}
										}
										break;
								}
							}
							
							if( ext == ".bmp" || ext == ".png" )
							{
								select_last_sprite();
								
								palette_group plt_grp = palette_group.Instance;
								plt_grp.update_selected_color();
#if DEF_NES								
								// copy transparent color of the first palette to the rest transparent color slots
								palette_small[] plt_arr = plt_grp.get_palettes_arr();
								
								for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
								{
									plt_arr[ i ].update_color( plt_arr[ 0 ].get_color_inds()[0], 0 );
								}
#endif
							}
						}
						break;
				}
			}
			catch( System.Exception _err )
			{
				message_box( _err.Message, "Data Import Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			if( br != null )
			{
				br.Close();
			}
			
			if( fs != null )
			{
				fs.Close();
			}
			
			SpriteList.EndUpdate();
		}
		
		void ExportImagesToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( SpriteList.Items.Count == 0 )			
			{
				message_box( "There is no data to export!", "Images Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				if( ExportImages_folderBrowserDialog.ShowDialog() == DialogResult.OK && m_img_export_options_form.ShowDialog() == DialogResult.OK )
				{
					try
					{
						sprite_data spr;
						
						int size = SpriteList.Items.Count;
						
						for( int i = 0; i < size; i++ )
						{
							spr = SpriteList.Items[ i ] as sprite_data;
							
							if( m_img_export_options_form.format == image_export_options_form.EImgFormat.PCX )
							{
								spr.save_image_PCX( ExportImages_folderBrowserDialog.SelectedPath, m_sprites_proc.get_palette_group().get_palettes_arr(), CBoxMode8x16.Checked );
							}
							else
							{
								spr.save_image_BMP_PNG( ExportImages_folderBrowserDialog.SelectedPath, m_img_export_options_form.alpha_channel, m_sprites_proc.get_palette_group().get_palettes_arr(), m_img_export_options_form.format, CBoxMode8x16.Checked );
							}
						}
					}
					catch( Exception _err )
					{
						message_box( _err.Message, "Images Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
		}
		
		void ExportASMToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			if( SpriteList.Items.Count == 0 )
			{
				message_box( "There is no data to export!", utils.CONST_PLATFORM + " ASM Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ExportASM_saveFileDialog.ShowDialog();
			}
		}
		
		void ExportASM_OK(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.String filename = ( ( System.Windows.Forms.FileDialog )sender ).FileName;
		
			data_export( filename, SpriteList.Items.Count, delegate( int _ind ) { return SpriteList.Items[ _ind ] as sprite_data; }, true );
		}

		void ExportCToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			if( SpriteList.Items.Count == 0 )
			{
				message_box( "There is no data to export!", utils.CONST_PLATFORM + " C Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ExportC_saveFileDialog.ShowDialog();
			}
		}
		
		void ExportC_OK(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.String filename = ( ( System.Windows.Forms.FileDialog )sender ).FileName;
		
			data_export( filename, SpriteList.Items.Count, delegate( int _ind ) { return SpriteList.Items[ _ind ] as sprite_data; }, false );
		}

		private void data_export( string _filename, int _spr_cnt, Func< int, sprite_data > _get_spr, bool _asm_file )
		{
			StreamWriter sw		= null;
			StreamWriter c_sw	= null;
			
			sprite_data spr;
			
			string prefix_filename;
			string data_prefix		= "";
			string post_spr_data	= "";
			string data_dir			= "";
		
			try
			{
#if DEF_NES
				bool save_padding_data = false;
				
				if( message_box( "Save padding data aligned to 1/2/4 KB?", "Export CHR Bank(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					save_padding_data = true;
				}
#elif DEF_SMS
				if( m_SMS_export_form.ShowDialog() == DialogResult.Cancel )
				{
					return;
				}
#elif DEF_PCE
				if( m_PCE_export_form.ShowDialog( _asm_file ) == DialogResult.Cancel )
				{
					return;
				}
				
				data_dir = m_PCE_export_form.data_dir;
#else
...
#endif
				string path		= System.IO.Path.GetDirectoryName( _filename );
				string filename	= System.IO.Path.GetFileNameWithoutExtension( _filename );

				if( data_dir.Length > 0 )
				{
					if( data_dir.StartsWith( System.IO.Path.DirectorySeparatorChar.ToString() ) || data_dir.StartsWith( System.IO.Path.AltDirectorySeparatorChar.ToString() ) )
					{
						// cut off the first slash
						data_dir = data_dir.Substring( 1 );
					}
					
					if( data_dir.EndsWith( System.IO.Path.DirectorySeparatorChar.ToString() ) || data_dir.EndsWith( System.IO.Path.AltDirectorySeparatorChar.ToString() ) )
					{
						// cut off the last slash
						data_dir = data_dir.Substring( 0, data_dir.Length - 1 );
					}
					
					data_dir += "/";
					
					data_dir = data_dir.Replace( "\\", "/" );
					
					// create data directory if needed
					if( !System.IO.Directory.Exists( path + System.IO.Path.DirectorySeparatorChar + data_dir ) )
					{
						System.IO.Directory.CreateDirectory( path + System.IO.Path.DirectorySeparatorChar + data_dir );
					}
				}
				
				sw = File.CreateText( path + System.IO.Path.DirectorySeparatorChar + data_dir + filename + "." + this.ExportASM_saveFileDialog.DefaultExt );
				{
					sw.WriteLine( utils.get_file_title( ";" ) );
					
					if( _asm_file )
					{
#if DEF_NES
						sw.WriteLine( filename.ToUpper() + "_SPR_MODE_8X16 = " + ( CBoxMode8x16.Checked ? "1":"0" ) + "\n" );
#elif DEF_SMS
						sw.WriteLine( ".define\t" + filename.ToUpper() + "_SPR_MODE_8X16\t" + ( CBoxMode8x16.Checked ? "1":"0" ) );
						sw.WriteLine( ".define\t" + filename.ToUpper() + "_SPR_CHR_BPP\t" + m_SMS_export_form.bpp );
						sw.WriteLine( ".define\t" + filename.ToUpper() + "_SPR_CHRS_OFFSET\t" + m_SMS_export_form.CHRs_offset + "\t; first CHR index in a CHR bank\n\n" );
#elif DEF_PCE
						sw.WriteLine( filename.ToUpper() + "_SPR_VADDR\t= " + m_PCE_export_form.VADDR );
						sw.WriteLine( filename.ToUpper() + "_PALETTE_SLOT\t= " + m_PCE_export_form.palette_slot + "\n\n" );
#else
...
#endif
					}
					else
					{
						c_sw = File.CreateText( path + System.IO.Path.DirectorySeparatorChar + filename + "." + this.ExportC_saveFileDialog.DefaultExt );
						
						c_sw.WriteLine( utils.get_file_title( "//" ) );
#if DEF_NES
						c_sw.WriteLine( "#define " + filename.ToUpper() + "_SPR_MODE_8X16\t" + ( CBoxMode8x16.Checked ? "1":"0" ) + "\n" );
#elif DEF_SMS
						c_sw.WriteLine( "#define " + filename.ToUpper() + "_SPR_MODE_8X16\t" + ( CBoxMode8x16.Checked ? "1":"0" ) );
						c_sw.WriteLine( "#define " + filename.ToUpper() + "_SPR_CHR_BPP\t" + m_SMS_export_form.bpp );
						c_sw.WriteLine( "#define " + filename.ToUpper() + "_SPR_CHRS_OFFSET\t" + m_SMS_export_form.CHRs_offset + "\t// first CHR index in a CHR bank\n\n" );
#elif DEF_PCE
						c_sw.WriteLine( "#incasm( \"" + data_dir + filename + "." + this.ExportASM_saveFileDialog.DefaultExt + "\" )\n" );
						c_sw.WriteLine( "#define " + filename.ToUpper() + "_SPR_VADDR\t" + m_PCE_export_form.VADDR );
						c_sw.WriteLine( "#define " + filename.ToUpper() + "_PALETTE_SLOT\t" + ( m_PCE_export_form.palette_slot + 16 ) + "\n\n" );
#else
...
#endif

#if DEF_PCE
						c_sw.WriteLine( "#ifndef\tDEF_TYPE_SPD_SPRITE\n#define\tDEF_TYPE_SPD_SPRITE\ntypedef struct\n{\n\tconst unsigned short*\tattrs;\n\tunsigned char\t\tbank;\n\tunsigned short\t\tsize;\n\tunsigned char\t\tSG_ind;\n} spd_SPRITE;\n#endif\t//DEF_TYPE_SPD_SPRITE\n\n" );
#else
						c_sw.WriteLine( "#ifndef\tDEF_TYPE_SPD_SPRITE\n#define\tDEF_TYPE_SPD_SPRITE\ntypedef struct\n{\n\tconst unsigned short*\tattrs;\n\tunsigned short\t\tsize;\n\tunsigned char\t\tCHR_ind;\n} spd_SPRITE;\n#endif\t//DEF_TYPE_SPD_SPRITE\n\n" );
#endif
						data_prefix = "_";
					}
					prefix_filename = data_prefix + filename;
					
					// CHR incbins 
					m_sprites_proc.rearrange_CHR_data_ids();
					
#if DEF_NES
					m_sprites_proc.export_CHR( sw, data_dir, prefix_filename, true, save_padding_data );
#elif DEF_SMS
					m_sprites_proc.export_CHR( sw, data_dir, prefix_filename, m_SMS_export_form.comment_CHR_data, m_SMS_export_form.bpp << 3 );
#elif DEF_PCE
					m_sprites_proc.export_CHR( sw, data_dir, prefix_filename, m_PCE_export_form.comment_CHR_data, _asm_file );
#else
...
#endif
					if( !_asm_file )
					{
#if DEF_PCE
						c_sw.WriteLine( "extern unsigned short*\t" + filename + "_SG_arr;" );
						c_sw.WriteLine( "#define\t" + filename + "_SG_cnt\t" + m_sprites_proc.get_CHR_banks().Count + "\t// graphics banks count" );
#else
						c_sw.WriteLine( "extern unsigned short*\t" + filename + "_CHR_arr;" );
						c_sw.WriteLine( "#define\t" + filename + "_CHR_cnt\t" + m_sprites_proc.get_CHR_banks().Count + "\t// graphics banks count" );
#endif
					}
					
					sw.WriteLine( "\n" );
					
#if DEF_FIXED_LEN_PALETTE16_ARR
					int max_palettes = -1;
					
					for( int i = 0; i < _spr_cnt; i++ )
					{
						foreach( var attr in _get_spr( i ).get_CHR_attr() )
						{
							if( max_palettes < attr.palette_ind )
							{
								max_palettes = attr.palette_ind;
							}
						}
					}
					
					if( ( max_palettes + m_PCE_export_form.palette_slot ) >= utils.CONST_PALETTE16_ARR_LEN )
					{
						throw new Exception( "The palette data doesn't fit into" + utils.CONST_PALETTE16_ARR_LEN + "slots!" );
					}
					
					palettes_array.Instance.export( sw, prefix_filename, max_palettes + 1 );
					
					if( !_asm_file )
					{
						c_sw.WriteLine( "#define\t" + filename + "_palette_size\t" + ( max_palettes + 1 ) + "\t// active palettes\n" );
						
						for( int i = 0; i < max_palettes + 1; i++ )
						{
							c_sw.WriteLine( "extern unsigned short*\t" + filename + "_palette_slot" + i + ";" );
						}
						
						c_sw.WriteLine( "" );
					}
#else
					m_sprites_proc.export_palette( sw, prefix_filename );
#endif
					if( !_asm_file )
					{
						c_sw.WriteLine( "extern unsigned short*\t" + filename + "_palette;" );
					}
					
					// save common sprites data
					{
						// sprites array
						sw.WriteLine( "\n" + prefix_filename + "_num_frames:\n\t." + ( _spr_cnt > 255 ? "word ":"byte " ) + String.Format( "${0:X2}\n" + prefix_filename + "_frames_data:", _spr_cnt ) );
						
						if( !_asm_file )
						{
							c_sw.WriteLine( "#define\t" + filename + "_frames_cnt\t" + _spr_cnt );
							c_sw.WriteLine( "extern spd_SPRITE\t" + filename + "_frames_data[];\n" );
						}
						
						bool enable_comments = true;
						
						for( int i = 0; i < _spr_cnt; i++ )
						{
							spr = _get_spr( i );
							
							sw.WriteLine( data_prefix + spr.name + "_frame:" );
							sw.WriteLine( "\t.word " + data_prefix + spr.name );
#if DEF_NES || DEF_SMS
							sw.WriteLine( "\t.byte " + data_prefix + spr.name + "_end - " + spr.name + ( enable_comments ? "\t; data size":"" ) );
#elif DEF_PCE
							if( !_asm_file )
							{
								sw.WriteLine( "\t.byte bank(" + ( data_prefix + spr.name ) + ")" );
							}
							
							sw.WriteLine( "\t.word " + spr.name + "_end - " + data_prefix + spr.name + ( enable_comments ? "\t; data size":"" ) );
#else
...
#endif
							sw.WriteLine( "\t.byte " + spr.get_CHR_data().id + ( enable_comments ? "\t\t; GFX bank index (" + spr.get_CHR_data().name + ")":"" ) );
#if DEF_NES
							spr.get_CHR_data().export( path + Path.DirectorySeparatorChar + data_dir + prefix_filename + "_" + spr.get_CHR_data().get_filename(), save_padding_data );
#elif DEF_SMS
							spr.get_CHR_data().export( path + Path.DirectorySeparatorChar + data_dir + prefix_filename + "_" + spr.get_CHR_data().get_filename(), m_SMS_export_form.bpp );
#elif DEF_PCE
							spr.get_CHR_data().export( path + Path.DirectorySeparatorChar + data_dir + prefix_filename + "_" + spr.get_CHR_data().get_filename() );
#else
...
#endif
							if( !_asm_file )
							{
								c_sw.WriteLine( "#define\tSPR_" + spr.name.ToUpper() + "\t" + i );
								
								post_spr_data += "extern spd_SPRITE*\t" + spr.name + "_frame;\n";
							}

							enable_comments = false;
						}
					}
					
					if( !_asm_file )
					{
						c_sw.WriteLine( "\n" + post_spr_data );
					}
					
					sw.WriteLine( "\n" );
					
#if DEF_NES
					sw.WriteLine( "\t; #1: Y pos, #2: CHR index, #3: Attributes, #4: X pos\n" );
					
					// save the sprites data
					for( int i = 0; i < _spr_cnt; i++ )
					{
						_get_spr( i ).export( sw, data_prefix );
					}
#elif DEF_SMS
					sw.WriteLine( "\t; #1: Y pos, #2: X pos, #3: CHR index\n" );
					
					// save the sprites data
					for( int i = 0; i < _spr_cnt; i++ )
					{
						_get_spr( i ).export( sw, m_SMS_export_form.CHRs_offset, data_prefix );
					}
#elif DEF_PCE
					sw.WriteLine( "\t; #1: Y pos, #2: X pos, #3: CHR index, #4: CHR desc\n" );
					
					// save the sprites data
					for( int i = 0; i < _spr_cnt; i++ )
					{
						_get_spr( i ).export( sw, m_PCE_export_form.CHRs_offset, m_PCE_export_form.palette_slot, data_prefix );
					}
#else
...
#endif
				}
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Data Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			finally
			{
				if( sw != null )
				{
					sw.Dispose();
				}
				
				if( c_sw != null )
				{
					c_sw.Dispose();
				}
			}
		}
		
		void ExportScriptEditorToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( !SPSeD.py_editor.is_active() )
			{
#if DEF_PCE
				m_py_editor = new SPSeD.py_editor( global::SPReD.Properties.Resources.SPReD_icon, new py_api( CBoxMode8x16, SpriteList, m_palettes_arr ), "SPReD API Doc", System.Text.Encoding.Default.GetString( global::SPReD.Properties.Resources.SPReD_Data_Export_Python_API ), "SPReD_Data_Export_Python_API.html" );
#else
				m_py_editor = new SPSeD.py_editor( global::SPReD.Properties.Resources.SPReD_icon, new py_api( CBoxMode8x16, SpriteList ), "SPReD API Doc", System.Text.Encoding.Default.GetString( global::SPReD.Properties.Resources.SPReD_Data_Export_Python_API ), "SPReD_Data_Export_Python_API.html" );
#endif
				m_py_editor.Show();
			}
			
			SPSeD.py_editor.set_focus();
		}
#endregion
//		PALETTES ARRAY		*********************************************************************************//
#region	palettes array
		void update_selected_color()
		{
			palette_group plt_grp = palette_group.Instance;
			
			plt_grp.get_palettes_arr()[ plt_grp.active_palette ].color_slot = plt_grp.get_palettes_arr()[ plt_grp.active_palette ].color_slot;
		}

		void CBoxPalettesChanged_Event(object sender, EventArgs e)
		{
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_palettes_arr.update_palette();
			m_sprites_proc.apply_palette_to_selected_CHR( m_palettes_arr.palette_index );
			update_selected_color();
#endif
		}
		
		void CBoxPalettesAdjustWidthDropDown_Event(object sender, EventArgs e)
		{
#if DEF_FIXED_LEN_PALETTE16_ARR
			( sender as ComboBox ).DropDownWidth = 230;
#endif
		}
		
#if DEF_FIXED_LEN_PALETTE16_ARR
		public void SetCHRPalette_Event(object sender, EventArgs e)
		{
			m_palettes_arr.palette_index = ( sender as sprite_layout_viewer ).get_selected_CHR_attr().palette_ind;
		}
		
		public void ApplyPaletteToCHR_Event(object sender, EventArgs e)
		{
			m_sprites_proc.apply_palette_to_selected_CHR( m_palettes_arr.palette_index );
		}
#endif
		void BtnSwapColorsClick_Event(object sender, EventArgs e)
		{
#if DEF_SMS || DEF_PCE
			if( SpriteList.Items.Count > 0 )
			{
				m_swap_colors_form.ShowDialog( SpriteList.Items );
				update_selected_sprite();
			}
#endif
		}

#endregion

		void KeyUp_Event(object sender, KeyEventArgs e)
		{
			m_sprites_proc.key_event( sender, e );
		}

		void PreviewKeyDown_Event(object sender, PreviewKeyDownEventArgs e)
		{
			//...
		}
	}
}
