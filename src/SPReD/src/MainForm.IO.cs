/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 15:46
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace SPReD
{
	partial class MainForm
	{
		private void LoadToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			m_project_loaded = false;
			
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure you want to close the current project?", "Load Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					ProjectOpenFileDialog.ShowDialog();
				}
			}
			else
			{
				ProjectOpenFileDialog.ShowDialog();
			}
			
			if( m_project_loaded )
			{
				if( m_description_form.auto_show() && m_description_form.edit_text.Length > 0 )
				{
					m_description_form.ShowDialog();
				}
			}
		}

		private void SaveToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			if( SpriteList.Items.Count == 0 )
			{
				message_box( "There is no data to save!", "Project Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ProjectSaveFileDialog.ShowDialog();
			}
		}
		
		private void ProjectSaveOK( object sender, System.ComponentModel.CancelEventArgs e )
		{
			// SAVE PROJECT...
			System.String filename = ( ( FileDialog )sender ).FileName;
		
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
				message_box( _err.Message, "Project Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			if( bw != null )
			{
				bw.Dispose();
			}
			
			if( fs != null )
			{
				fs.Dispose();
			}
		}
		
		private void ProjectLoadOK( object sender, System.ComponentModel.CancelEventArgs e )
		{
			// LOAD PROJECT...
			project_load( ( ( FileDialog )sender ).FileName );
		}

		private void project_load( string _filename )
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
				
				message_box( _err.Message, "Project Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			if( br != null )
			{
				br.Dispose();
			}
			
			if( fs != null )
			{
				fs.Dispose();
			}
		}

		private void ImportToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			ImportOpenFileDialog.ShowDialog();
		}

		private void ImportOK( object sender, System.ComponentModel.CancelEventArgs e )
		{
			System.String[] filenames = ( ( FileDialog )sender ).FileNames;
			
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
				message_box( _err.Message, "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			if( br != null )
			{
				br.Dispose();
			}
			
			if( fs != null )
			{
				fs.Dispose();
			}
			
			SpriteList.EndUpdate();
		}
		
		private void ExportImagesToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( SpriteList.Items.Count == 0 )
			{
				message_box( "There is no data to export!", "Images Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				if( ExportImagesFolderBrowserDialog.ShowDialog() == DialogResult.OK && m_img_export_options_form.ShowDialog() == DialogResult.OK )
				{
					try
					{
						sprite_data spr;
						
						int size = SpriteList.Items.Count;
						
						for( int i = 0; i < size; i++ )
						{
							spr = SpriteList.Items[ i ] as sprite_data;
							
							if( m_img_export_options_form.format == image_export_options_form.e_img_format.PCX )
							{
								spr.save_image_PCX( ExportImagesFolderBrowserDialog.SelectedPath, m_sprites_proc.get_palette_group().get_palettes_arr(), CBoxMode8x16.Checked );
							}
							else
							{
								spr.save_image_BMP_PNG( ExportImagesFolderBrowserDialog.SelectedPath, m_img_export_options_form.alpha_channel, m_sprites_proc.get_palette_group().get_palettes_arr(), m_img_export_options_form.format, CBoxMode8x16.Checked );
							}
						}
					}
					catch( Exception _err )
					{
						message_box( _err.Message, "Images Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
		}
		
		private void ExportASMToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			if( SpriteList.Items.Count == 0 )
			{
				message_box( "There is no data to export!", utils.CONST_PLATFORM + "ASM Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ExportASMSaveFileDialog.ShowDialog();
			}
		}
		
		private void ExportASMOK( object sender, System.ComponentModel.CancelEventArgs e )
		{
			System.String filename = ( ( FileDialog )sender ).FileName;
		
			data_export( filename, SpriteList.Items.Count, delegate( int _ind ) { return SpriteList.Items[ _ind ] as sprite_data; }, true );
		}

		private void ExportCToolStripMenuItemClick( object sender, System.EventArgs e )
		{
			if( SpriteList.Items.Count == 0 )
			{
				message_box( "There is no data to export!", utils.CONST_PLATFORM + "C Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ExportCSaveFileDialog.ShowDialog();
			}
		}
		
		private void ExportCOK( object sender, System.ComponentModel.CancelEventArgs e )
		{
			System.String filename = ( ( FileDialog )sender ).FileName;
		
			data_export( filename, SpriteList.Items.Count, delegate( int _ind ) { return SpriteList.Items[ _ind ] as sprite_data; }, false );
		}

		private void data_export( string _filename, int _spr_cnt, Func< int, sprite_data > _get_spr, bool _asm_file )
		{
			StreamWriter sw		= null;
			StreamWriter c_sw	= null;
			
			sprite_data spr;
			
			string prefix_filename;
			string CHR_data_filename;
			string data_prefix		= "";
			string post_spr_data	= "";
			string data_dir			= "";
			string sprite_prefix	= "";
		
			bool comment_CHR_data	= false;
			
			try
			{
				string path				= System.IO.Path.GetDirectoryName( _filename );
				string filename			= System.IO.Path.GetFileNameWithoutExtension( _filename );
				string filename_upper	= filename.ToUpper();
				
#if DEF_NES
				bool save_padding_data = false;
				
				if( message_box( "Save padding data aligned to 1/2/4 KB?", "Export CHR Bank(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					save_padding_data = true;
				}
				
				comment_CHR_data = true;
#elif DEF_SMS
				if( m_SMS_export_form.ShowDialog() == DialogResult.Cancel )
				{
					return;
				}
				
				comment_CHR_data = m_SMS_export_form.comment_CHR_data;
#elif DEF_PCE
				if( m_PCE_export_form.show_window( _asm_file ) == DialogResult.Cancel )
				{
					return;
				}
				
				sprite_prefix = ( m_PCE_export_form.add_filename_to_sprite_names ? filename + "_":"" );
				
				comment_CHR_data = m_PCE_export_form.comment_CHR_data;
				
				data_dir = m_PCE_export_form.data_dir;
#else
...
#endif
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
				
				sw = File.CreateText( path + System.IO.Path.DirectorySeparatorChar + data_dir + filename + "." + this.ExportASMSaveFileDialog.DefaultExt );
				{
					sw.WriteLine( utils.get_file_title( ";" ) );
					
					if( _asm_file )
					{
#if DEF_NES
						sw.WriteLine( filename_upper + "_SPR_MODE_8X16 = " + ( CBoxMode8x16.Checked ? "1":"0" ) + "\n" );
#elif DEF_SMS
						sw.WriteLine( ".define\t" + filename_upper + "_SPR_MODE_8X16\t" + ( CBoxMode8x16.Checked ? "1":"0" ) );
						sw.WriteLine( ".define\t" + filename_upper + "_SPR_CHR_BPP\t" + m_SMS_export_form.bpp );
						sw.WriteLine( ".define\t" + filename_upper + "_SPR_CHRS_OFFSET\t" + m_SMS_export_form.CHRs_offset + "\t; first CHR index in a CHR bank\n\n" );
#elif DEF_PCE
						sw.WriteLine( filename_upper + "_SPR_VADDR\t= " + utils.hex( "$", m_PCE_export_form.VADDR ) + "\n" );
#else
...
#endif
					}
					else
					{
						c_sw = File.CreateText( path + System.IO.Path.DirectorySeparatorChar + filename + "." + this.ExportCSaveFileDialog.DefaultExt );
						
						c_sw.WriteLine( utils.get_file_title( "//" ) );
#if DEF_NES
						c_sw.WriteLine( "#define " + filename_upper + "_SPR_MODE_8X16\t" + ( CBoxMode8x16.Checked ? "1":"0" ) + "\n" );
#elif DEF_SMS
						c_sw.WriteLine( "#define " + filename_upper + "_SPR_MODE_8X16\t" + ( CBoxMode8x16.Checked ? "1":"0" ) );
						c_sw.WriteLine( "#define " + filename_upper + "_SPR_CHR_BPP\t" + m_SMS_export_form.bpp );
						c_sw.WriteLine( "#define " + filename_upper + "_SPR_CHRS_OFFSET\t" + m_SMS_export_form.CHRs_offset + "\t// first CHR index in a CHR bank\n\n" );
#elif DEF_PCE
						c_sw.WriteLine( "#incasm( \"" + data_dir + filename + "." + this.ExportASMSaveFileDialog.DefaultExt + "\" )\n" );
						c_sw.WriteLine( "#define " + filename_upper + "_SPR_VADDR\t" + utils.hex( "0x", m_PCE_export_form.VADDR ) + "\n" );
#else
...
#endif

#if DEF_PCE
						c_sw.WriteLine( "#ifndef\tDEF_TYPE_SPD_SPRITE\n#define\tDEF_TYPE_SPD_SPRITE\ntypedef struct\n{\n\tconst unsigned short\tsize;\n\tconst unsigned char\tSG_ind;\n\tconst unsigned short\tattrs[];\n} spd_SPRITE;\n#endif\t//DEF_TYPE_SPD_SPRITE\n\n" );
#else
						c_sw.WriteLine( "#ifndef\tDEF_TYPE_SPD_SPRITE\n#define\tDEF_TYPE_SPD_SPRITE\ntypedef struct\n{\n\tconst unsigned short\tsize;\n\tconst unsigned char\tCHR_ind;\n\tconst unsigned char\tattrs[];\n} spd_SPRITE;\n#endif\t//DEF_TYPE_SPD_SPRITE\n\n" );
#endif
						data_prefix = "_";
					}
					prefix_filename = data_prefix + filename;
					
					// CHR incbins 
					m_sprites_proc.rearrange_CHR_data_ids();
					
					HashSet< int > skipped_banks_id = get_skipped_banks_id( _spr_cnt, _get_spr );
					
#if DEF_NES
					m_sprites_proc.export_CHR( sw, data_dir, prefix_filename, comment_CHR_data, skipped_banks_id, save_padding_data );
#elif DEF_SMS
					m_sprites_proc.export_CHR( sw, data_dir, prefix_filename, comment_CHR_data, skipped_banks_id, m_SMS_export_form.bpp << 3 );
#elif DEF_PCE
					m_sprites_proc.export_CHR( sw, data_dir, prefix_filename, comment_CHR_data, skipped_banks_id, _asm_file );
#else
...
#endif
					if( !_asm_file )
					{
#if DEF_PCE
						c_sw.WriteLine( ( comment_CHR_data ? "//":"" ) + "#define\t" + filename_upper + "_SG_CNT\t" + m_sprites_proc.get_CHR_banks().Count + "\t// graphics banks count" );
						c_sw.WriteLine( ( comment_CHR_data ? "//":"" ) + "extern unsigned short*\t" + filename + "_SG_arr;\n" );
#else
						c_sw.WriteLine( ( comment_CHR_data ? "//":"" ) + "#define\t" + filename_upper + "_CHR_CNT\t" + m_sprites_proc.get_CHR_banks().Count + "\t// graphics banks count" );
						c_sw.WriteLine( ( comment_CHR_data ? "//":"" ) + "extern unsigned short*\t" + filename + "_CHR_arr;\n" );
#endif
					}
					
					sw.WriteLine( "\n" );
					
#if DEF_FIXED_LEN_PALETTE16_ARR
					int max_palettes	= -1;
					int min_palettes	= utils.CONST_PALETTE16_ARR_LEN;
					int slots_cnt		= 0;
					
					for( int i = 0; i < _spr_cnt; i++ )
					{
						spr = _get_spr( i );
						
						if( spr.export_palette() )
						{
							foreach( var attr in spr.get_CHR_attr() )
							{
								if( max_palettes < attr.palette_ind )
								{
									max_palettes = attr.palette_ind;
								}
								
								if( min_palettes > attr.palette_ind )
								{
									min_palettes = attr.palette_ind;
								}
							}
						}
					}
					
					slots_cnt = ( max_palettes - min_palettes ) + 1;
					
					if( ( max_palettes + m_PCE_export_form.palette_slot ) >= utils.CONST_PALETTE16_ARR_LEN )
					{
						throw new Exception( "The palette data doesn't fit into" + utils.CONST_PALETTE16_ARR_LEN + "slots!" );
					}
					
					palettes_array.Instance.export( sw, prefix_filename, min_palettes, slots_cnt );
					
					if( !_asm_file )
					{
						c_sw.WriteLine( "#define\t" + filename_upper + "_PALETTE_SIZE\t" + ( ( max_palettes - min_palettes ) + 1 ) + "\t// active palettes" );
						c_sw.WriteLine( "#define " + filename_upper + "_PALETTE_SLOT\t" + ( min_palettes + m_PCE_export_form.palette_slot + 16 ) + "\n" );
						
						for( int i = 0; i < slots_cnt; i++ )
						{
							c_sw.WriteLine( "extern unsigned short*\t" + filename + "_palette_slot" + i + ";" );
						}
					}
					else
					{
						sw.WriteLine( "\n" + filename_upper + "_PALETTE_SLOT\t= " + ( min_palettes + m_PCE_export_form.palette_slot ) + "\n" );
					}
#else
					m_sprites_proc.export_palette( sw, prefix_filename );
#endif
					if( !_asm_file )
					{
						c_sw.WriteLine( "extern unsigned short*\t" + filename + "_palette;\n" );
					}
					
					// save common sprites data
					{
						// sprites array
						sw.WriteLine( "\n" + prefix_filename + "_num_frames:\n\t." + ( _spr_cnt > 255 ? "word ":"byte " ) + String.Format( "${0:X2}\n" + prefix_filename + "_frames_data:", get_exported_sprites_cnt( _spr_cnt, _get_spr ) ) );
						
						if( !_asm_file )
						{
							c_sw.WriteLine( "#define\t" + filename_upper + "_FRAMES_CNT\t" + _spr_cnt );
							c_sw.WriteLine( "extern unsigned char*\t" + filename + "_frames_data;\n" );
						}
						
						string sprite_name;
						
						for( int i = 0; i < _spr_cnt; i++ )
						{
							spr = _get_spr( i );
							
							if( !spr.export_graphics() )
							{
								continue;
							}
							
							sprite_name = sprite_prefix + spr.name; 
							
							sw.WriteLine( data_prefix + sprite_name + "_frame:" );
							sw.WriteLine( "\t.word " + data_prefix + sprite_name );
#if DEF_PCE
							sw.WriteLine( "\t.byte bank(" + ( data_prefix + sprite_name ) + ")" );
#endif
							CHR_data_filename = path + Path.DirectorySeparatorChar + data_dir + prefix_filename + "_" + spr.get_CHR_data().get_filename();
#if DEF_NES
							spr.get_CHR_data().export( CHR_data_filename, save_padding_data );
#elif DEF_SMS
							spr.get_CHR_data().export( CHR_data_filename, m_SMS_export_form.bpp );
#elif DEF_PCE
							if( !m_PCE_export_form.non_packed_sprites_opt || ( spr.is_packed( CBoxMode8x16.Checked ) || spr.check_16x32_64_mode() ) )
							{
								spr.get_CHR_data().export( CHR_data_filename );
							}
							else
							{
								PCE_metasprite_exporter.export_CHR_data( spr, CHR_data_filename );
							}
#else
...
#endif
							if( !_asm_file )
							{
								c_sw.WriteLine( "#define\tSPR_" + sprite_name.ToUpper() + "\t" + i );
								
								post_spr_data += "extern spd_SPRITE*\t" + sprite_name + ";\n";
							}
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
						spr = _get_spr( i );
						
						if( spr.export_graphics() )
						{
							spr.export( sw, data_prefix + sprite_prefix );
						}
					}
#elif DEF_SMS
					sw.WriteLine( "\t; #1: Y pos, #2: X pos, #3: CHR index\n" );
					
					// save the sprites data
					for( int i = 0; i < _spr_cnt; i++ )
					{
						spr = _get_spr( i );
						
						if( spr.export_graphics() )
						{
							spr.export( sw, m_SMS_export_form.CHRs_offset, data_prefix + sprite_prefix );
						}
					}
#elif DEF_PCE
					sw.WriteLine( "\t; #1: Y pos, #2: X pos, #3: CHR index, #4: CHR desc\n" );
					
					// save the sprites data
					for( int i = 0; i < _spr_cnt; i++ )
					{
						spr = _get_spr( i );
						
						if( spr.export_graphics() )
						{
							if( !m_PCE_export_form.non_packed_sprites_opt || ( spr.is_packed( CBoxMode8x16.Checked ) || spr.check_16x32_64_mode() ) )
							{
								spr.export( sw, m_PCE_export_form.CHRs_offset, m_PCE_export_form.palette_slot, data_prefix + sprite_prefix );
							}
							else
							{
								PCE_metasprite_exporter.export_sprite( sw, spr, m_PCE_export_form.CHRs_offset, m_PCE_export_form.palette_slot, data_prefix + sprite_prefix );
							}
						}
					}
#else
...
#endif
				}
			}
			catch( Exception _err )
			{
				message_box( _err.Message, "Data Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
		
		private int get_exported_sprites_cnt( int _spr_cnt, Func< int, sprite_data > _get_spr )
		{
			int spr_cnt = 0;
			
			for( int i = 0; i < _spr_cnt; i++ )
			{
				if( _get_spr( i ).export_graphics() )
				{
					++spr_cnt;
				}
			}
			
			return spr_cnt;
		}
		
		private HashSet< int > get_skipped_banks_id( int _spr_cnt, Func< int, sprite_data > _get_spr )
		{
			int i;
			int spr_bank_id;
			
			sprite_data spr;
			
			Dictionary< int, int >	sprite_bank_id		= new Dictionary< int, int >( _spr_cnt );
			HashSet< int >			skipped_banks_id	= new HashSet< int >();
			
			for( i = 0; i < _spr_cnt; i++ )
			{
				spr = _get_spr( i );
				
				if( !spr.export_graphics() )
				{
					skipped_banks_id.Add( spr.get_CHR_data().id );
					
					sprite_bank_id[ spr.get_CHR_data().id ] = i;
				}
			}
			
			// check the skipped_banks_id data for sprites that share the same bank but one should be skipped while another one should not
			for( i = 0; i < _spr_cnt; i++ )
			{
				spr = _get_spr( i );
				
				if( spr.export_graphics() )
				{
					spr_bank_id = spr.get_CHR_data().id;
					
					foreach( int bank_id in skipped_banks_id )
					{
						if( spr_bank_id == bank_id )
						{
							skipped_banks_id.Remove( bank_id );
							
							message_box( "The following sprites share the same CHR bank:\n\n'" + _get_spr( sprite_bank_id[ bank_id ] ).name + "' and '" + spr.name + "'\n\nTip: Split the '" + _get_spr( sprite_bank_id[ bank_id ] ).name + "' sprite data for optimal data export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
							
							break;
						}
					}
				}
			}
			
			return skipped_banks_id;
		}
	}
}
