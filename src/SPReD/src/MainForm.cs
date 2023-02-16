/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 13.03.2017
 * Time: 11:24
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		private delegate string add_remove_pref_postf_func( bool _postfix, string _pref_postf, sprite_data _spr, ref bool _changed );
		
		private sprite_processor m_sprites_proc = null;
		
		private readonly create_sprite_form			m_create_sprite_form 		= new create_sprite_form();
		private readonly rename_sprite_form			m_rename_sprite_form 		= new rename_sprite_form();
		private readonly copy_sprite_new_name_form	m_new_sprite_name_form 		= new copy_sprite_new_name_form();
		private readonly image_export_options_form	m_img_export_options_form	= new image_export_options_form();
		private readonly image_import_options_form	m_img_import_options_form	= new image_import_options_form();
		private readonly description_form			m_description_form			= new description_form();

#if DEF_SMS
		private readonly SMS_sprite_flipping_form	m_SMS_sprite_flip_form;
		private readonly SMS_export_form			m_SMS_export_form			= new SMS_export_form();
		private readonly swap_colors_form			m_swap_colors_form			= new swap_colors_form();
#elif DEF_PCE
		private readonly PCE_export_form			m_PCE_export_form			= new PCE_export_form();
		private readonly swap_colors_form			m_swap_colors_form			= new swap_colors_form();
#endif

#if DEF_FIXED_LEN_PALETTE16_ARR
		private readonly palettes_array				m_palettes_arr;
		private readonly palettes_manager_form		m_palettes_mngr_form;
#endif
		private SPSeD.py_editor	m_py_editor	= null;
		
		private bool m_project_loaded	= false;
		
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
			m_sprites_proc.subscribe( this );
#endif
			CBoxCHRPackingType.SelectedIndex = 0;
			CBoxFlipType.SelectedIndex = 0;
			
			SpriteList.ContextMenuStrip = SpriteListContextMenu;
			CHRBank.ContextMenuStrip 	= ContextMenuCHRBank;
			
			tooltip_data[] tooltips = new tooltip_data[]{ 	new tooltip_data( BtnMoveItemUp, "Move selected item up" ),
															new tooltip_data( BtnMoveItemDown, "Move selected item down" ),
															new tooltip_data( BtnCHROptimization, "Remove unused/empty/duplicate CHRs in all CHR banks" ),
															new tooltip_data( BtnCHRPack, "Merge selected sprites data into common CHR bank(s)" ),
															new tooltip_data( BtnCHRSplit, "Extract all sprites data into separate CHR banks" ),
															new tooltip_data( BtnOffset, "Apply offset to all selected sprites" ),
															new tooltip_data( Palette0, "Shift+1 / Ctrl+1,2,3,4 to select a color" ),
															new tooltip_data( Palette1, "Shift+2 / Ctrl+1,2,3,4 to select a color" ),
															new tooltip_data( Palette2, "Shift+3 / Ctrl+1,2,3,4 to select a color" ),
															new tooltip_data( Palette3, "Shift+4 / Ctrl+1,2,3,4 to select a color" ),
															new tooltip_data( BtnZoomIn, "Zoom In" ),
															new tooltip_data( BtnZoomOut, "Zoom Out" ),
															new tooltip_data( BtnAddCHR, "Add new CHR" ),
															new tooltip_data( BtnDeleteLastCHR, "Delete last CHR" ),
															new tooltip_data( BtnDeleteCHR, "Delete selected CHR" ),
															new tooltip_data( BtnCentering, "Place an active sprite in the middle of the viewport" ),
															new tooltip_data( BtnShiftColors, "Cyclic shifting of active sprite color indices" ),
															new tooltip_data( CBoxShiftTransp, "Use transparency when shifting color indices" ),
															new tooltip_data( BtnApplyDefaultPalette, "Apply palette to all selected sprites" ),
															new tooltip_data( BtnCHRVFlip, "Vertical flipping of selected CHR" ),
															new tooltip_data( BtnCHRHFlip, "Horizontal flipping of selected CHR" ),
															new tooltip_data( BtnCHRRotate, "Clockwise rotation of selected CHR" ),
															new tooltip_data( BtnVFlip, "Vertical flipping of selected CHR" ),
															new tooltip_data( BtnHFlip, "Horizontal flipping of selected CHR" ),
															new tooltip_data( BtnSpriteVFlip, "Vertical flipping of all selected sprites" ),
															new tooltip_data( BtnSpriteHFlip, "Horizontal flipping of all selected sprites" ),
															new tooltip_data( CBoxAxesLayout, "Show X/Y axes" ),
															new tooltip_data( CBoxGridLayout, "Show grid" ),
															new tooltip_data( CBoxSnapLayout, "Snap CHRs to " + utils.CONST_CHR_SIDE_PIXELS_CNT + "x" + utils.CONST_CHR_SIDE_PIXELS_CNT + " grid" ),
															new tooltip_data( CBoxMode8x16, "8x16 sprite mode" ),
#if DEF_FIXED_LEN_PALETTE16_ARR
															new tooltip_data( CBoxPalettes, "Palettes array" ),
															new tooltip_data( BtnSwapColors, "Swap two selected colors without changing graphics" ),
#endif
														};
			tooltip_data data;
			
			for( int i = 0; i < tooltips.Length; i++ )
			{
				data = tooltips[ i ];
				
				( new ToolTip(this.components) ).SetToolTip( data.m_cntrl, data.m_desc );
			}
			
			// the build mode is active by default
			BtnModeBuildClick( null, null );
			
			// disable PASTE action by default
			PasteCHRToolStripMenuItem.Enabled = false;
			
			FormClosing += new FormClosingEventHandler( OnFormClosing );
			
			set_title_name( null );
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_palettes_arr			= new palettes_array( CBoxPalettes, SpriteList.Items );
			
			m_palettes_mngr_form	= new palettes_manager_form( m_palettes_arr, SpriteList.Items, delegate() { update_selected_sprite(); } );
			
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

#if !DEF_PALETTES_MANAGER
			palettesManagerToolStripMenuItem.Visible = false;
#endif

#if DEF_NES
			this.ProjectOpenFileDialog.DefaultExt = "spredsms";
			this.ProjectOpenFileDialog.Filter += "|SPReD-SMS (*.spredsms)|*.spredsms";
			
			paletteToolStripMenuItem.Visible = false;
			
			this.ExportCToolStripMenuItem.Text = "&CC65";
			this.ExportCSaveFileDialog.Title = "Export CC65: Select File";
			this.ExportCSaveFileDialog.Filter = this.ExportCSaveFileDialog.Filter.Replace( "C", "CC65");
#elif DEF_SMS
			this.ProjectSaveFileDialog.DefaultExt = "spredsms";
			this.ProjectSaveFileDialog.Filter = this.ProjectSaveFileDialog.Filter.Replace( "NES", "SMS" );
			this.ProjectSaveFileDialog.Filter = this.ProjectSaveFileDialog.Filter.Replace( "nes", "sms" );

			this.ProjectOpenFileDialog.DefaultExt = "spredsms";
			this.ProjectOpenFileDialog.Filter = "SPReD-SMS (*.spredsms)|*.spredsms|" + this.ProjectOpenFileDialog.Filter;

			this.ImportOpenFileDialog.Filter = this.ImportOpenFileDialog.Filter.Replace( "4 colors", "16/4 colors" );
			
			this.ExportASMToolStripMenuItem.Text = "&WLA-DX asm";
			this.ExportASMSaveFileDialog.Filter = "WLA-DX (*.asm)|*.asm";

			this.ExportCToolStripMenuItem.Text = "&SDCC";
			this.ExportCSaveFileDialog.Title = "Export SDCC: Select File";
			this.ExportCSaveFileDialog.Filter = this.ExportCSaveFileDialog.Filter.Replace( "C", "SDCC");
			
			BtnApplyDefaultPalette.Enabled = applyPaletteToolStripMenuItem.Enabled = false;
			BtnShiftColors.Enabled = shiftColorsToolStripMenuItem.Enabled = CBoxShiftTransp.Enabled = false;
			
			CHRFlippingGroupBox.Enabled = false;
			
			m_SMS_sprite_flip_form  = new SMS_sprite_flipping_form( SpriteList, m_sprites_proc );
			
			CBoxCHRPackingType.Items.Add( "8KB" );
#elif DEF_PCE
			this.ProjectSaveFileDialog.DefaultExt = "spredpce";
			this.ProjectSaveFileDialog.Filter = this.ProjectSaveFileDialog.Filter.Replace( "NES", "PCE" );
			this.ProjectSaveFileDialog.Filter = this.ProjectSaveFileDialog.Filter.Replace( "nes", "pce" );

			this.ProjectOpenFileDialog.DefaultExt = "spredpce";
			this.ProjectOpenFileDialog.Filter = "SPReD-PCE (*.spredpce)|*.spredpce";//|" + this.ProjectOpenFileDialog.Filter;

			this.ImportOpenFileDialog.Filter = this.ImportOpenFileDialog.Filter.Replace( "4 colors", "16/4 colors" );
			this.ImportOpenFileDialog.Filter = this.ImportOpenFileDialog.Filter.Replace( "Palette (192 bytes)", "Palette (1536 bytes)" );
			
			this.ExportASMToolStripMenuItem.Text = "&CA65/PCEAS";
			this.ExportASMSaveFileDialog.Filter = "CA65/PCEAS (*.asm)|*.asm";
			
			this.ExportCToolStripMenuItem.Text = "&HuC";
			this.ExportCSaveFileDialog.Title = "Export HuC: Select File";
			this.ExportCSaveFileDialog.Filter = this.ExportCSaveFileDialog.Filter.Replace( "C", "HuC");
			
			BtnShiftColors.Enabled = shiftColorsToolStripMenuItem.Enabled = CBoxShiftTransp.Enabled = false;

			CBoxMode8x16.Visible	= false;
			
			CBoxCHRPackingType.Items.Add( "8KB" );
			CBoxCHRPackingType.Items.Add( "16KB" );
			CBoxCHRPackingType.Items.Add( "32KB" );
#endif
			if( _args.Length > 0 )
			{
				project_load( _args[0] );
			}
			else
			{
				reset();
			}
		}
		
		public static DialogResult message_box( string _msg, string _title, MessageBoxButtons _buttons, MessageBoxIcon _icon = MessageBoxIcon.Warning )
		{
			return MessageBox.Show( _msg, _title, _buttons, _icon );
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
			
			// the build mode is active by default
			BtnModeBuildClick( null, null );
			
			OffsetX.Value = 0;
			OffsetY.Value = 0;
			
			CBoxMode8x16.Checked = false;
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			palettes_array.Instance.reset();
			palette_group.Instance.active_palette = 0;
			
			CBoxPalettes.Enabled = false;
			CBoxPalettes.SelectedIndex = 0;
#else
			palette_group.Instance.reset();
			palette_group.Instance.active_palette = 0;
#endif
		}
		
		private void ExitToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			Close();
		}
		
		private void AboutToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			message_box( "Sprites Editor (" + utils.CONST_PLATFORM + ")\n\n" + utils.CONST_APP_VER + " " + utils.build_str + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		private void QuickGuideToolStripMenuItemClick( object sender, EventArgs e )
		{
			string doc_path = Application.StartupPath.Substring( 0, Application.StartupPath.LastIndexOf( Path.DirectorySeparatorChar ) ) + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + "SPReD" + Path.DirectorySeparatorChar + "Quick_Guide.html";
			
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
		
		private void CloseToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure?", "Close Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					reset();
					
					set_title_name( null );
					
					m_description_form.edit_text = "";
				}
			}
		}
		
		private void DescriptionToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_description_form.ShowDialog();
		}
		
		private void StatisticsToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_sprites_proc.show_statistics( SpriteList.Items );
		}

		private void ExportScriptEditorToolStripMenuItemClick( object sender, EventArgs e )
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
		
		private void OnKeyUp( object sender, KeyEventArgs e )
		{
			m_sprites_proc.on_key_up( sender, e );
		}
	}
}
