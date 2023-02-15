/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 16:35
 */
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace MAPeD
{
	partial class MainForm
	{
		private void LoadToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_cnt > 0 )
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
		}

		private void ProjectLoadOk( object sender, System.ComponentModel.CancelEventArgs e )
		{
			// LOAD PROJECT...
			project_load( ( ( FileDialog )sender ).FileName );
		}

		private async void project_load( string _filename )
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
								
								set_screen_data_type( prj_data.m_scr_data_tiles4x4 ? data_sets_manager.e_screen_data_type.Tiles4x4:data_sets_manager.e_screen_data_type.Blocks2x2 );
								
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
								set_screen_data_type( data_sets_manager.e_screen_data_type.Tiles4x4 );
							}

							// check screen resolutions
							{
								if( load_scr_data_len != scr_data_len )
								{
									if( m_data_conversion_options_form.show_window( prj_data ) == DialogResult.Cancel )
									{
										reset();
										return;
									}
									
									prj_data.m_scr_align					= m_data_conversion_options_form.screen_align_mode;
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
		
		private void SaveToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_cnt == 0 )
			{
				message_box( "There is no data to save!", "Project Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				ProjectSaveFileDialog.ShowDialog();
			}
		}
		
		private void ProjectSaveOk( object sender, System.ComponentModel.CancelEventArgs e )
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

					uint pre_flags = ( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 ) ? utils.CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4:0;
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
		
		private void ImportToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( CBoxCHRBanks.SelectedIndex >= 0 )
			{
				ImportOpenFileDialog.ShowDialog();
			}
			else
			{
				message_box( "There is no active CHR bank!", "Data Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private async void DataImportOk( object sender, System.ComponentModel.CancelEventArgs e )
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

		private void ExportToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( CBoxCHRBanks.Items.Count > 0 )
			{
				ProjectExportFileDialog.ShowDialog();
			}
			else
			{
				message_box( "There is no data to export!", "Project Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void ProjectExportOk( object sender, System.ComponentModel.CancelEventArgs e )
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
							m_exp_asm.show_window( filename );
						}
						break;
#endif
					case ".zxa":
						{
							m_exp_zx_asm.show_window( filename );
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
	}
}
