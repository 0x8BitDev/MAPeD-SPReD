/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 13.03.2017
 * Time: 11:24
 */
using System;
using System.IO;
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
		private description_form			m_description_form			= new description_form();
		
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
			                                       	CHRBank,
			                                       	CHRBankLabel,
			                                       	PaletteMain,
													Palette0,
													Palette1,
													Palette2,
													Palette3 );
			
			CBoxCHRPackingType.SelectedIndex = 0;
			CBoxFlipType.SelectedIndex = 0;
			
			SpriteList.ContextMenuStrip = SpriteListContextMenu;
			CHRBank.ContextMenuStrip 	= ContextMenuCHRBank;
			
			SToolTipData[] tooltips = new SToolTipData[]{ 	new SToolTipData( BtnMoveItemUp, "Move selected item up" ),
															new SToolTipData( BtnMoveItemDown, "Move selected item down" ),
															new SToolTipData( BtnCHROptimization, "Remove unused/empty CHRs" ),
															new SToolTipData( BtnCHRPack, "Merge selected sprites data into common CHR bank(s)" ),
															new SToolTipData( BtnCHRSplit, "Extract selected sprites data into separate CHR banks" ),
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
															new SToolTipData( CBoxAxisLayout, "Show X/Y axes" ),
															new SToolTipData( CBoxGridLayout, "Show grid" ),
															new SToolTipData( CBoxSnapLayout, "Snap CHRs to 8x8 grid" ),
															new SToolTipData( CBoxMode8x16, "8x16 sprite mode" ),
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
			
			if( _args.Length > 0 )
			{
				project_load( _args[0] );
			}
		}
		
		public static System.Windows.Forms.DialogResult message_box( string _msg, string _title, System.Windows.Forms.MessageBoxButtons _buttons, System.Windows.Forms.MessageBoxIcon _icon = System.Windows.Forms.MessageBoxIcon.Warning )
		{
			return System.Windows.Forms.MessageBox.Show( _msg, _title, _buttons, _icon );
		}
		
		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;
			
		    if( message_box( "All unsaved progress will be lost!\nAre you sure?", "Exit App", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
		    {
				reset();
				m_sprites_proc = null;
				
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
			
			// delete the sprites...
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
		}
		
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
		
		private void update_selected_sprite( bool _new_sprite = false, bool _show_mode = true )
		{
			if( SpriteList.SelectedIndex >= 0 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndex ] as sprite_data;
				
				OffsetX.Value = spr.offset_x;
				OffsetY.Value = spr.offset_y;
				
				m_sprites_proc.update_sprite( spr, _new_sprite, _show_mode );
			}
		}
		
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
					m_description_form.ShowDialog( this );
				}
			}
		}

		void SaveToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			if( SpriteList.Items.Count == 0 )			
			{
				message_box( "There are no data to save!", "Save Project Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				Project_saveFileDialog.ShowDialog();
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
				SpriteList.BeginUpdate();
				
				int i;
				int j;
				int size = filenames.Length;
	
				string spr_name;
				string filename;
				
				string ext = Path.GetExtension( filenames[ 0 ] );
				
				switch( ext )
				{
					case ".pal":
						{
							fs = new FileStream( filenames[ 0 ], FileMode.Open, FileAccess.Read );
							
							br = new BinaryReader( fs );
							
							if( br.BaseStream.Length == 192 )
							{
								palette_group.Instance.load_main_palette( br );
							}
							else
							{
								throw new Exception( "The imported palette must be 192 bytes length!" );
							}
						}
						break;
			
					default:
						{
							for( i = 0; i < size; i++ )
							{
								filename = filenames[ i ];
			
								spr_name = System.IO.Path.GetFileNameWithoutExtension( filename );
								
								switch( ext )
								{
									case ".bmp":
									case ".png":
										{
											if(!check_duplicate( spr_name ) )
											{
												if( ext == ".png" )
												{
													SpriteList.Items.Add( m_sprites_proc.load_sprite_png( filename, spr_name ) );
												}
												else
												{	// otherwise - .bmp
													SpriteList.Items.Add( m_sprites_proc.load_sprite_bmp( filename, spr_name ) );
												}
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
											
											if( br.BaseStream.Length > 4096 )
											{
												j = 0;
												
												while( br.BaseStream.Position < br.BaseStream.Length - 1 )
												{
													if(!check_duplicate( spr_name + "_" + j ) )
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
												if(!check_duplicate( spr_name ) )
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
				br.Close();
			}
			
			SpriteList.EndUpdate();
		}
		
		void ExportImagesToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( SpriteList.Items.Count == 0 )			
			{
				message_box( "There are no data to export!", "Images Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
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
								
							spr.save_image( ExportImages_folderBrowserDialog.SelectedPath, m_img_export_options_form.alpha_channel, m_sprites_proc.get_palette_group().get_palettes_arr(), m_img_export_options_form.format, CBoxMode8x16.Checked );
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
				message_box( "There are no data to export!", "NESASM Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ExportASM_saveFileDialog.ShowDialog();
			}
		}
		
		void ExportASM_OK(object sender, System.ComponentModel.CancelEventArgs e)
		{
			System.String filename = ( ( System.Windows.Forms.FileDialog )sender ).FileName;
		
			data_export( filename, SpriteList.Items.Count, delegate( int _ind ) { return SpriteList.Items[ _ind ] as sprite_data; } );
		}
		
		private void data_export( string _filename, int _spr_cnt, Func< int, sprite_data > _get_spr )
		{
			StreamWriter sw = null;
			
			try
			{
				sprite_data spr;
				
				string path		= System.IO.Path.GetDirectoryName( _filename );
				string filename	= System.IO.Path.GetFileNameWithoutExtension( _filename );
				
				sw = File.CreateText( _filename );
				{
					sw.WriteLine( ";#######################################################\n;" );
					sw.WriteLine( "; Generated by " + utils.CONST_APP_NAME + " Copyright 2017-" + DateTime.Now.Year + " 0x8BitDev\n;" );
					sw.WriteLine( ";#######################################################\n" );

					sw.WriteLine( "SPR_MODE_8X16 = " + ( CBoxMode8x16.Checked ? "1":"0" ) + "\n\n" );
					
					// 'incbin's of CHRs 
					m_sprites_proc.rearrange_CHR_data_ids();
					m_sprites_proc.export_CHR( sw, filename, true );
					
					sw.WriteLine( "\n" );
					
					m_sprites_proc.export_palette( sw );
					
					// save common sprites data
					{
						// sprites array
						sw.WriteLine( "\nn_frames:\n\t." + ( _spr_cnt > 255 ? "word ":"byte " ) + String.Format( "${0:X2}\nframes_data:", _spr_cnt ) );
						
						bool enable_comments = true;
						
						for( int i = 0; i < _spr_cnt; i++ )
						{
							spr = _get_spr( i );
							
							sw.WriteLine( spr.name + "_frame:" );
							sw.WriteLine( "\t.word " + spr.name );
							sw.WriteLine( "\t.byte " + spr.name + "_end - " + spr.name + ( enable_comments ? "\t; data size":"" ) );
							
							sw.WriteLine( "\t.byte " + spr.get_CHR_data().id + ( enable_comments ? "\t\t; CHR bank index (" + spr.get_CHR_data().name + ")":"" ) );
							
							spr.get_CHR_data().export( path, filename );
							
							enable_comments = false;
						}
					}
					
					sw.WriteLine( "\n" );
					
					// save the sprites data
					for( int i = 0; i < _spr_cnt; i++ )
					{
						_get_spr( i ).export( sw );
					}
				}
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			if( sw != null )
			{
				sw.Close();
			}
		}
		
		void ExitToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			Close();
		}
		
		void MenuHelpAboutClick(object sender, System.EventArgs e)
		{
			message_box( "Sprites Editor (" + utils.CONST_PLATFORM + ")\n\n" + utils.CONST_APP_VER + " " + utils.build_str + "\nBuild date: " + utils.build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2017-" + DateTime.Now.Year, "About", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information );
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
						
						m_sprites_proc.update_sprite( spr, false, false );
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
					message_box( new_sprite_name + " - A sprite with the same name is already exists! Ignored!", _wnd_caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
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
					message_box( new_name + " - A sprite with the same name is already exists! Ignored!", "Copy Sprite", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
			
			m_sprites_proc.rearrange_CHR_data_ids();
			
			m_sprites_proc.update_sprite( SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data, false, false );
		}
		
		void BtnDelete_Event(object sender, System.EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to delete " + SpriteList.SelectedIndices.Count + " sprite(s)?", "Delete Selected Sprite(s)", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					sprite_data spr;

					int spr_CHR_id;
					
					do
					{
						spr = SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data;
						
						SpriteList.Items.RemoveAt( SpriteList.SelectedIndices[ 0 ] );
						
						spr_CHR_id = spr.get_CHR_data().id;
						
						m_sprites_proc.remove( spr );
						
						CHR_bank_optimization( spr_CHR_id );
					}
					while( SpriteList.SelectedIndices.Count != 0 );
					
					if( SpriteList.Items.Count == 0 )
					{
						reset();
						
						set_title_name( null );
					}
					else
					{
						spr = null;
						m_sprites_proc.update_sprite( spr );
					}
					
					m_sprites_proc.rearrange_CHR_data_ids();
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Delete Sprite(s)", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
		private void CHR_bank_optimization( int _CHR_bank_id )
		{
			CHR_data_group CHR_bank = m_sprites_proc.get_CHR_bank( _CHR_bank_id );
			
			if( CHR_bank == null )
			{
				// the bank had one sprite and the bank has been removed
				return;
			}
			
			int CHR_n;
			int CHR_cnt = CHR_bank.get_data().Count;
			
			int spr_n;
			int spr_cnt = SpriteList.Items.Count;

			int attr_n;
			int attr_cnt;
			
			sprite_data 	spr;
			CHR_data_attr 	attr;
			
			bool CHR_used;
			
			for( CHR_n = 0; CHR_n < CHR_cnt; CHR_n++ )
			{
				CHR_used = false;
				
				// go through all sprites wich used _CHR_bank_id
				// and check unused attributes
				for( spr_n = 0; spr_n < spr_cnt; spr_n++ )
				{
					spr = SpriteList.Items[ spr_n ] as sprite_data;
					
					if( spr.get_CHR_data().id == _CHR_bank_id )
					{
						attr_cnt = spr.get_CHR_attr().Count;
						
						for( attr_n = 0; attr_n < attr_cnt; attr_n++ )
						{
							if( spr.get_CHR_attr()[ attr_n ].CHR_id == CHR_n )
							{
								CHR_used = true;
								
								break;
							}
							
							if( CBoxMode8x16.Checked )
							{
								if( spr.get_CHR_attr()[ attr_n ].CHR_id + 1 == CHR_n )
								{
									CHR_used = true;
									
									break;
								}
							}
						}
						
						if( CHR_used == true )
						{
							break;
						}
					}
				}
				
				if( CHR_used == false )
				{
					// clear unused CHR
					CHR_bank.get_data()[ CHR_n ].clear();
				}
			}

			// delete empty CHRs and fix indices of all sprites that referring to them  
			for( CHR_n = 0; CHR_n < CHR_cnt; CHR_n++ )
			{
				if( CHR_bank.get_data()[ CHR_n ].is_empty() )
				{
					// go through all sprites wich used _CHR_bank_id
					// and check unused attributes
					for( spr_n = 0; spr_n < spr_cnt; spr_n++ )
					{
						spr = SpriteList.Items[ spr_n ] as sprite_data;
						
						if( spr.get_CHR_data().id == _CHR_bank_id )
						{
							attr_cnt = spr.get_CHR_attr().Count;
							
							for( attr_n = 0; attr_n < attr_cnt; attr_n++ )
							{
								attr = spr.get_CHR_attr()[ attr_n ];
								
								if( attr.CHR_id == CHR_n )
								{
									// удаляем пустой атрибут
									spr.get_CHR_attr().RemoveAt( attr_n );
									
									--attr_n;
									--attr_cnt;
								}
								else
								if( attr.CHR_id > CHR_n )
								{
									--attr.CHR_id;
								}
							}
							
							spr.update_dimensions();
						}
					}
					
					// delete empty CHR
					CHR_bank.get_data().RemoveAt( CHR_n );
					
					--CHR_cnt;
					--CHR_n;
				}
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
				
				update_selected_sprite( false, false );
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
				
				update_selected_sprite( false, false );
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
			}
			else
			{
				sprite_data spr = null;
				m_sprites_proc.update_sprite( spr, true, false );
				
				OffsetX.Value = 0;
				OffsetY.Value = 0;
			}
		}
		
		void CBoxAxisLayoutCheckedChanged_Event(object sender, EventArgs e)
		{
			m_sprites_proc.set_sprite_layout_viewer_flags( CBoxAxisLayout.Checked, CBoxGridLayout.Checked );
		}
		
		void CBoxGridLayoutCheckedChanged_Event(object sender, EventArgs e)
		{
			m_sprites_proc.set_sprite_layout_viewer_flags( CBoxAxisLayout.Checked, CBoxGridLayout.Checked );
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
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_vert( ( sprite_data.EFlipType )( CBoxFlipType.SelectedIndex ) ); } );
		}
		
		void BtnSpriteHFlip_Event(object sender, EventArgs e)
		{
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_horiz( ( sprite_data.EFlipType )( CBoxFlipType.SelectedIndex ) ); } );
		}
		
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
				update_selected_sprite( false, false );
			}
		}
		
		void BtnShiftColors_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndex >= 0 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndex ] as sprite_data;
				
				spr.shift_colors( CBoxShiftTransp.Checked, CBoxMode8x16.Checked );
				m_sprites_proc.update_sprite( spr, false, false );
			}
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
		
		void BtnCHRSplit_Event(object sender, EventArgs e)
		{
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure you want to split the CHR data?\n\nWARNING: ALL sprites, including Ref ones, will have unique CHR banks!", "CHR Data Splitting", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					int i;
					int sprites_cnt = SpriteList.Items.Count;					
					
					for( i = 0; i < sprites_cnt; i++ )
					{
						m_sprites_proc.split_CHR( SpriteList.Items[ i ] as sprite_data, CBoxMode8x16.Checked );
					}
					
					m_sprites_proc.rearrange_CHR_data_ids();
					
					// update data in the layout viewport
					update_selected_sprite( false, false );
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "CHR Data Splitting", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
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
				if( message_box( "Are you sure you want to pack the selected sprites?\n\nWARNING: Irreversible operation for Ref sprites!\nALL unused/empty CHRs will be lost!", "CHR Data Packing", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
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
					
					for( i = 0; i < tmp_inds_list.Count; i++ )
					{
						spr_1 = SpriteList.Items[ tmp_inds_list[ i ] ] as sprite_data;
						chr_1 = spr_1.get_CHR_data();
						
						spr_1.optimize( false, CBoxMode8x16.Checked );
						
						for( j = i+1; j < tmp_inds_list.Count; j++ )
						{
							spr_2 = SpriteList.Items[ tmp_inds_list[ j ] ] as sprite_data;
							chr_2 = spr_2.get_CHR_data();

							spr_2.optimize( false, CBoxMode8x16.Checked );

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
												spr_3.get_CHR_attr()[ m ].CHR_id += spr_1_last_CHR_cnt; 
											}
										}
										
										m_sprites_proc.relink_CHR_data( spr_3, new_chr, chr_1.id );
										m_sprites_proc.relink_CHR_data( spr_3, new_chr, chr_2.id );
									}
									
									tmp_inds_list.Remove( tmp_inds_list[ j ] );
								
									--j;
								}
							}
						}
					}

					m_sprites_proc.rearrange_CHR_data_ids();
					
					// update data in the layout viewport
					update_selected_sprite( false, false );
				}
			}
			else
			{
				message_box( "Please, select sprites!", "CHR Data Packing", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}

		void BtnCHROpt_Event(object sender, EventArgs e)
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to optimize the selected sprite(s)?", "CHR Data Optimization", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
					{
						CHR_bank_optimization( ( SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data ).get_CHR_data().id );
					}
				
					update_selected_sprite( false, false );
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "CHR Data Optimization", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );				
			}
		}
		
		void CBoxCHRPackingType_ChangedEvent(object sender, EventArgs e)
		{
			System.Windows.Forms.ComboBox cbox = sender as System.Windows.Forms.ComboBox;
			
			CHRPackToolStripMenuItem.Enabled = BtnCHRPack.Enabled = ( cbox.SelectedIndex != 0 ) ? true:false;
		}
		
		void BtnCreate_Event(object sender, EventArgs e)
		{
			m_create_sprite_form.Text = "Create Sprite [ mode: " + ( CBoxMode8x16.Checked ? "8x16":"8x8" ) + " ]";
			
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
			
			if(!check_duplicate( new_sprite_name ) )
			{
				SpriteList.Items.Add( m_sprites_proc.create_sprite( new_sprite_name, m_create_sprite_form.sprite_width, m_create_sprite_form.sprite_height, CBoxMode8x16.Checked ) );
			}
			else
			{
				message_box( new_sprite_name + " - A sprite with the same name is already exists! Ignored!", "Create Sprite Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
			}
		}
		
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
			m_sprites_proc.chr_transform( CHR8x8_data.ETransform.t_vflip );
		}
		
		void BtnCHREditorHFlipClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.chr_transform( CHR8x8_data.ETransform.t_hflip );
		}
		
		void BtnCHREditorRotateClick_Event(object sender, EventArgs e)
		{
			m_sprites_proc.chr_transform( CHR8x8_data.ETransform.t_rotate );
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
					
					uint flags = ( uint )( ( CBoxMode8x16.Checked ? 1:0 ) | ( CBoxGridLayout.Checked ? 2:0 ) | ( CBoxAxisLayout.Checked ? 4:0 ) ) | utils.CONST_PROJECT_FILE_PALETTE_FLAG;
					bw.Write( flags );
					
					int n_sprites = SpriteList.Items.Count;
					bw.Write( n_sprites );
					
					for( int i = 0; i < n_sprites; i++ )
					{
						( SpriteList.Items[ i ] as sprite_data ).save( bw );
					}
					
					palette_group.Instance.save_main_palette( bw );
					
					// save description
					bw.Write( m_description_form.edit_text );
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( filename ) );
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Save Project Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
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
							m_sprites_proc.load_CHR_storage_and_palette( br );
							
							uint flags = br.ReadUInt32();
							CBoxMode8x16.Checked 	= ( ( flags&0x01 ) == 0x01 ? true:false );
							CBoxGridLayout.Checked	= ( ( flags&0x02 ) == 0x02 ? true:false );
							CBoxAxisLayout.Checked	= ( ( flags&0x04 ) == 0x04 ? true:false );
							
							int n_sprites = br.ReadInt32();
							
							for( int i = 0; i < n_sprites; i++ )
							{
								SpriteList.Items.Add( m_sprites_proc.load_sprite( br ) );
							}
							
							if( ( flags&utils.CONST_PROJECT_FILE_PALETTE_FLAG ) == utils.CONST_PROJECT_FILE_PALETTE_FLAG )
							{
								palette_group.Instance.load_main_palette( br );
							}
							
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
				}
				
				set_title_name( Path.GetFileNameWithoutExtension( _filename ) );
				
				// select a first sprite
				if( SpriteList.Items.Count > 0 )
				{
					SpriteList.SelectedIndex = 0;
					
					m_sprites_proc.layout_sprite_centering();
				}
				
				m_project_loaded = true;
			}
			catch( Exception _err )
			{
				reset();
				
				message_box( _err.Message, "Load Project Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
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
		
		void CloseToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure you want to close the current project?", "Close Project", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
				{
					reset();
					
					set_title_name( null );
					
					m_description_form.edit_text = "";
				}
			}
		}
		
		void KeyUp_Event(object sender, KeyEventArgs e)
		{
			m_sprites_proc.key_event( sender, e );
		}

		void PreviewKeyDown_Event(object sender, PreviewKeyDownEventArgs e)
		{
			//...
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
		
		void DescriptionToolStripMenuItemClick_Event(object sender, EventArgs e)
		{
			m_description_form.ShowDialog();
		}
	}
}
