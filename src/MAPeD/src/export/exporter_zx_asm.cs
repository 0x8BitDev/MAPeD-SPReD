﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 29.08.2018
 * Time: 15:20
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_zx_sjasm.
	/// </summary>
	public partial class exporter_zx_sjasm : Form
	{
		private readonly data_sets_manager m_data_mngr;
		
		private string 	m_path_filename_ext		= null;
		private string 	m_path_filename			= null;
		private string 	m_extra_path_filename	= null;
		
		private const string	CONST_FILENAME_LEVEL_POSTFIX	= "_Lev";
		private const string	CONST_FILENAME_LEVEL_PREFIX		= "Lev";
		private const string	CONST_FILENAME_BANK_POSTFIX		= "_Bank";
		private const string	CONST_FILENAME_BANK_PREFIX		= "Bank";
		private const string	CONST_BIN_EXT					= ".bin";
		
		private const int		CONST_BIDIR_MAP_SCREEN_MAX_CNT	= 255;
		
		private string	m_gfx_data_filename		= null;
		private string	m_tile_props_filename	= null;
		private string	m_tile_colrs_filename	= null;
		private string	m_map_data_filename		= null;
		private string	m_tiles_data_filename	= null;
		private string	m_filename				= null;
		private string	m_path					= null;
		
		// 7: flashing, 6: brightness, 3-5: paper color, 0-2: ink color
		private readonly int[] zx_palette;
		
		private struct exp_screen_data
		{
			public int 			m_scr_ind;
			
			public tiles_data 	m_tiles;
			
			public screen_data	m_scr_tiles;
			public screen_data	m_scr_blocks;
			
			public int			m_tiles_offset;
			public int			m_blocks_offset;
			
			public static int	_tiles_offset;
			public static int	_blocks_offset;
			
			public exp_screen_data( int _scr_ind, tiles_data _tiles )
			{
				m_scr_ind		= _scr_ind;
				
				m_tiles 		= _tiles;
				
				m_scr_tiles		= new screen_data( platform_data.get_screen_tiles_width_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Tiles4x4 ), platform_data.get_screen_tiles_height_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Tiles4x4 ) );
				m_scr_blocks	= new screen_data( platform_data.get_screen_tiles_width_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Blocks2x2 ), platform_data.get_screen_tiles_height_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Blocks2x2 ) );
				
				m_tiles_offset	= _tiles_offset;
				m_blocks_offset	= _blocks_offset;
				
				_tiles_offset 	+= m_scr_tiles.length;
				_blocks_offset 	+= m_scr_blocks.length;
			}
			
			public void destroy()
			{
				m_tiles 		= null;
				
				m_scr_tiles 	= null;
				m_scr_blocks 	= null;
			}
		};	
		
		public exporter_zx_sjasm( data_sets_manager _data_mngr )
		{
			m_data_mngr = _data_mngr;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			CBoxColorConversionModes.SelectedIndex	= 0;
			CBoxEntSortingType.SelectedIndex		= 0;
			
			zx_palette = platform_data.get_palette_by_file_ext( platform_data.CONST_ZX_FILE_EXT );
			
#if DEF_ZX
			CheckBoxGFXDithering.Checked = CheckBoxGFXDithering.Enabled = false;
			groupBoxColorConversion.Enabled = false;
#endif
			RBtnModeMultidirScroll.Checked = true;
		}
		
		private void BtnCancelClick( object sender, EventArgs e )
		{
			this.Close();
		}
		
		private void CheckBoxExportEntitiesChanged( object sender, EventArgs e )
		{
			bool enabled = ( sender as CheckBox ).Checked;
			
			groupBoxEntityCoordinates.Enabled = enabled;
			
			CheckBoxExportMarks.Enabled = ( enabled == false && RBtnModeMultidirScroll.Checked ) ? false:true;
			CheckBoxExportMarks.Checked = CheckBoxExportMarks.Enabled ? CheckBoxExportMarks.Checked:false;
			
			update_desc();
		}

		private void RBtnModeMultidirScrollChanged( object sender, EventArgs e )
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = false;
			RBtnLayoutMatrix.Checked = true;
			
			CheckBoxExportMarks.Enabled = ( CheckBoxExportEntities.Checked == false && RBtnModeMultidirScroll.Checked ) ? false:true;
			CheckBoxExportMarks.Checked = CheckBoxExportMarks.Enabled ? CheckBoxExportMarks.Checked:false;
			
			CheckBoxRLE.Enabled = true;
			
			RBtnEntityCoordMap.Checked	= true;
			
			if( !RBtnModeMultidirScroll.Checked )
			{
				CheckBoxRenderLevelPNG.Checked = CheckBoxRenderTilesPNG.Checked = false;
			}
			
			GrpBoxExtraOutput.Enabled = RBtnModeMultidirScroll.Checked;
			
			update_desc();
		}
		
		private void RBtnModeScreenToScreenChanged( object sender, EventArgs e )
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			RBtnLayoutAdjacentScreens.Checked	= true;
			
			CheckBoxExportMarks.Enabled 	= true;
			RBtnEntityCoordScreen.Checked	= true;
			
			CheckBoxRLE.Checked = CheckBoxRLE.Enabled = false;
			
			update_desc();
		}

		private void OnParamChanged( object sender, EventArgs e )
		{
			update_desc();
		}

		private void update_desc()
		{
			if( RBtnTiles2x2.Checked )
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_ZX_TILES_2X2;
			}
			else
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_ZX_TILES_4X4;
			}
			
			if( CheckBoxRLE.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_RLE_COMPRESSION;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER;
			
			if( RBtnTilesDirColumns.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER_COLS;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER_ROWS;
			}

			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE;
			
			if( RBtnModeMultidirScroll.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE_MULTIDIR + strings.CONST_STR_EXP_ZX_MODE_MULTIDIR;
			}
			else
			{
				RichTextBoxExportDesc.Text += string.Format( strings.CONST_STR_EXP_MODE_BIDIR, CONST_BIDIR_MAP_SCREEN_MAX_CNT );
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PROP;
			
			if( RBtnPropPerBlock.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PROP_PER_BLOCK;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PROP_PER_CHR;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_LAYOUT;
			
			if( RBtnLayoutAdjacentScreens.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_LAYOUT_ADJ_SCR;
			}
			else
			if( RBtnLayoutAdjacentScreenIndices.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_LAYOUT_ADJ_SCR_INDS;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_LAYOUT_MATRIX;
			}
			
			if( CheckBoxExportMarks.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MARKS;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NO_MARKS;
			}
			
			if( CheckBoxExportEntities.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENTITIES;
				
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_COORDS;
				
				if( RBtnEntityCoordScreen.Checked )
				{
					RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_COORDS_SCR;
				}
				else
				{
					RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_COORDS_MAP;
				}
				
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_SORTING;
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_SORT_TYPES[ CBoxEntSortingType.SelectedIndex ];
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NO_ENTITIES;
			}
#if !DEF_ZX
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ZX_INK_FACTOR;
#endif
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_WARNING;
		}
		
		public DialogResult show_window( string _full_path )
		{
			m_path_filename_ext 	= _full_path;
			m_filename				= Path.GetFileNameWithoutExtension( _full_path );
			m_path					= Path.GetDirectoryName( _full_path ) + Path.DirectorySeparatorChar;
			m_path_filename			= m_path + m_filename;
			m_extra_path_filename	= Path.GetDirectoryName( _full_path ) + Path.DirectorySeparatorChar + m_filename + "_Extra" + Path.DirectorySeparatorChar + m_filename;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Blocks2x2 )
			{
				RBtnTiles4x4.Enabled = false;
				RBtnTiles2x2.Checked = true;
			}
			else
			{
				RBtnTiles4x4.Enabled = true; 
			}
			
			return ShowDialog();
		}
		
		private void BtnOkClick( object sender, EventArgs e )
		{
			this.Close();
			
			if( CheckBoxRenderLevelPNG.Checked || CheckBoxRenderTilesPNG.Checked )
			{
				System.IO.Directory.CreateDirectory( m_path_filename + "_Extra" + Path.DirectorySeparatorChar );
			}

			StreamWriter sw = null;
			
			try
			{
				sw = new StreamWriter( m_path_filename_ext );
				
				utils.write_title( sw );
				
				save_export_options( sw );
				
				if( CheckBoxExportEntities.Checked )
				{
					m_data_mngr.export_base_entity_asm( sw, "db", "$" );
				}
				
				if( RBtnModeMultidirScroll.Checked )
				{
					save_multidir_scroll_mode( sw );
				}
				else
				{
					save_single_screen_mode( sw );
				}
			}
			catch( System.Exception _err )
			{
				MainForm.message_box( _err.Message, "Data Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error ); 
			}
			
			finally
			{
				if( sw != null )
				{
					sw.Close();
				}
			}
		}

		private void save_multidir_scroll_mode( StreamWriter _sw )
		{
			// .a file includes:
			// unique tiles2x2 arr
			// tile properties
			// tile colors
			// level map
			
			BinaryWriter	bw;
			
			byte[]	map_tiles_arr;
			byte[]	tile_props_arr;
			byte[]	tile_colors_arr;
			
			int[]	img_buff = new int[ 64*4 ];

			layout_data level_data;
			
			Bitmap		level_bmp;
			Graphics	level_gfx;
			
			int n_levels = m_data_mngr.layouts_data_cnt;
			int n_scr_X;
			int n_scr_Y;
			int scr_ind;
			
			List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
			
			tiles_data bank_data;
			
			int n_banks	= scr_tiles_data.Count;
			int bank_n;
			int	map_data_size;
			int tile_offs_x;
			int tile_offs_y;
			int n_Y_tiles;
			int bank_ind;
			int chk_bank_ind;
			int n_screens;
			int	max_tile_ind;
			int	blocks_cnt;
			ushort scr_tile_id;
			ushort tile_data;
			
			long gfx_data_size;
			long map_size;
			
			string level_prefix_str;
			string level_postfix_str;
			string bank_prefix_str;
			string bank_postfix_str;
			string maps_CHRs_arr	= "\nMapsCHRBanks:\n";
			
			layout_screen_data scr_data;
			
			int scr_width_blocks 	= platform_data.get_screen_blocks_width();
			int scr_height_blocks 	= platform_data.get_screen_blocks_height();

			_sw.WriteLine( "\n; *** GLOBAL DATA ***\n" );
			
			for( bank_n = 0; bank_n < n_banks; bank_n++ )
			{
				bank_data = scr_tiles_data[ bank_n ];
				
				bank_postfix_str	= CONST_FILENAME_BANK_POSTFIX + bank_n;
				bank_prefix_str		= CONST_FILENAME_BANK_PREFIX + bank_n;
				
				m_gfx_data_filename		= m_filename + bank_postfix_str + "_GFX" + CONST_BIN_EXT;
				m_tile_props_filename	= m_filename + bank_postfix_str + "_TilePrps" + CONST_BIN_EXT;
				m_tile_colrs_filename	= m_filename + bank_postfix_str + "_TileClrs" + CONST_BIN_EXT;
				m_tiles_data_filename	= m_filename + bank_postfix_str + "_Tiles" + CONST_BIN_EXT;
				
				blocks_cnt = bank_data.get_first_free_block_id( false );
				
				tile_colors_arr = new byte[ blocks_cnt << 2 ];
				Array.Clear( tile_colors_arr, 0, tile_colors_arr.Length );
				
				tile_props_arr = new byte[ blocks_cnt * ( RBtnPropPerBlock.Checked ? 1:4 ) ];
				Array.Clear( tile_props_arr, 0, tile_props_arr.Length );

				if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
				{
					max_tile_ind = bank_data.get_first_free_tile_id( false );
				}
				else
				{
					max_tile_ind = bank_data.get_first_free_block_id( false );
				}

				// save tiles gfx
				{
					bw = new BinaryWriter( File.Open( m_path + m_gfx_data_filename, FileMode.Create ) );
					
					for( int block_n = 0; block_n < blocks_cnt; block_n++ )
					{
						build_and_save_blocks2x2_gfx_and_props( bw, bank_data, block_n, tile_props_arr, 0, tile_colors_arr, 0, img_buff );
					}
					
					gfx_data_size = bw.BaseStream.Length;
					bw.Close();
				}
				
				// write tiles properties
				{
					bw = new BinaryWriter( File.Open( m_path + m_tile_props_filename, FileMode.Create ) );
					bw.Write( tile_props_arr );
					bw.Close();
				}

				// write tiles colors
				if( RBtnTypeColor.Checked )
				{
					bw = new BinaryWriter( File.Open( m_path + m_tile_colrs_filename, FileMode.Create ) );
					bw.Write( tile_colors_arr );
					bw.Close();
				}
				
				if( RBtnTiles4x4.Checked )
				{
					// save tiles 4x4 data
					bw = new BinaryWriter( File.Open( m_path + m_tiles_data_filename, FileMode.Create ) );
					
					for( int tile_n = 0; tile_n < max_tile_ind; tile_n++ )
					{
						bw.Write( tiles_data.tile_convert_ulong_to_uint_reverse( bank_data.tiles[ tile_n ] ) );
					}
					
					bw.Close();
				}
				
				_sw.WriteLine( bank_prefix_str + "_Gfx"  + "\tincbin \"" + m_gfx_data_filename + "\"\t\t;(" + gfx_data_size + ") array of tile images 2x2 (32 bytes per tile)" );
				_sw.WriteLine( bank_prefix_str + "_Props" + "\tincbin \"" + m_tile_props_filename + "\"\t;(" + tile_props_arr.Length + ") tile properties array (byte per tile)" );
				
				if( RBtnTypeColor.Checked )
				{
					_sw.WriteLine( bank_prefix_str + "_Clrs" + "\tincbin \"" + m_tile_colrs_filename + "\"\t;(" + tile_colors_arr.Length + ") tile colors array (byte per tile)" );
				}
				
				if( RBtnTiles4x4.Checked )
				{
					_sw.WriteLine( bank_prefix_str + "_Tiles" + "\tincbin \"" + m_tiles_data_filename + "\"\t;(" + max_tile_ind + ") (4x4) 4 block indices per tile ( left to right, up to down )" );
				}
				
				_sw.WriteLine( "" );
			}
			
			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				level_prefix_str	= CONST_FILENAME_LEVEL_PREFIX + level_n;
				level_postfix_str	= CONST_FILENAME_LEVEL_POSTFIX + level_n;
				
				m_map_data_filename	= m_filename + level_postfix_str + "_Data" + CONST_BIN_EXT;
				
				level_data = m_data_mngr.get_layout_data( level_n );
				
				check_ent_instances_cnt( level_data, level_prefix_str );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				n_Y_tiles = n_scr_Y * ( RBtnTiles2x2.Checked ? platform_data.get_screen_blocks_height():platform_data.get_screen_tiles_height() );
				
				// initialize a tile map of a game level
				{
					map_data_size = n_scr_X * n_scr_Y * ( RBtnTiles2x2.Checked ? platform_data.get_screen_blocks_cnt():platform_data.get_screen_tiles_cnt() );

					map_tiles_arr = new byte[ map_data_size ];
					Array.Clear( map_tiles_arr, 0, map_data_size );
				}
				
				chk_bank_ind = -1;
				
				for( int scr_n_X = 0; scr_n_X < n_scr_X; scr_n_X++ )
				{
					for( int scr_n_Y = 0; scr_n_Y < n_scr_Y; scr_n_Y++ )
					{
						scr_data = level_data.get_data( scr_n_X, scr_n_Y );
						
						if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
						{
							// determine which bank the screen belongs to
							n_screens = 0;
							for( bank_ind = 0; bank_ind < n_banks; bank_ind++ )
							{
								n_screens += scr_tiles_data[ bank_ind ].screen_data_cnt();
								
								if( scr_data.m_scr_ind < n_screens )
								{
									break;
								}
							}
				
							if( chk_bank_ind >= 0 && bank_ind != chk_bank_ind )
							{
								_sw.Close();
								throw new System.Exception( String.Format( "Each level must use ONLY one CHR bank in the 'Multidirectional scrolling' mode!\n\nThe level_{0} export failed!", level_n ) );
							}
							
							chk_bank_ind = bank_ind;
							
							// convert screen index to local bank index
							scr_ind = scr_data.m_scr_ind - ( n_screens - scr_tiles_data[ bank_ind ].screen_data_cnt() );
							
							bank_data = scr_tiles_data[ bank_ind ];
							
							if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
							{
								for( int tile_n = 0; tile_n < platform_data.get_screen_tiles_cnt(); tile_n++ )
								{
									tile_offs_x = ( tile_n % platform_data.get_screen_tiles_width() );
									tile_offs_y = ( tile_n / platform_data.get_screen_tiles_width() );
									
									scr_tile_id = bank_data.get_screen_tile( scr_ind, tile_n );
									
									if( RBtnTiles2x2.Checked )
									{
										tile_offs_x <<= 1;
										tile_offs_y <<= 1;
										
										for( int block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
										{
											tile_data = bank_data.get_tile_block( scr_tile_id, block_n );
											
											if( ( tile_offs_x + ( block_n & 0x01 ) ) < platform_data.get_screen_blocks_width() && ( tile_offs_y + ( ( block_n & 0x02 ) >> 1 ) ) < platform_data.get_screen_blocks_height() )
											{
												map_tiles_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + ( ( block_n & 0x01 ) == 0x01 ? n_Y_tiles:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ) ] = ( byte )tile_data;
											}
										}
									}
									else
									{
										map_tiles_arr[ scr_n_X * ( n_Y_tiles * platform_data.get_screen_tiles_width() ) + ( scr_n_Y * platform_data.get_screen_tiles_height() ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = ( byte )scr_tile_id;
									}
								}
							}
							else
							{
								for( int block_n = 0; block_n < platform_data.get_screen_blocks_cnt(); block_n++ )
								{
									tile_offs_x = ( block_n % platform_data.get_screen_blocks_width() );
									tile_offs_y = ( block_n / platform_data.get_screen_blocks_width() );
									
									scr_tile_id = bank_data.get_screen_tile( scr_ind, block_n );
									
									map_tiles_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = ( byte )scr_tile_id;
								}
							}
						}
					}
				}

				if( chk_bank_ind < 0 )
				{
					continue;
				}
				
				bank_data = scr_tiles_data[ chk_bank_ind ];

				// render map image
				if( CheckBoxRenderLevelPNG.Checked )
				{
					level_bmp = new Bitmap( n_scr_X * platform_data.get_screen_width_pixels(), n_scr_Y * platform_data.get_screen_height_pixels() );

					level_gfx 					= Graphics.FromImage( level_bmp );
					level_gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
					level_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
					
					for( int block_n = 0; block_n < bank_data.get_first_free_block_id( false ); block_n++ )
					{
						build_and_save_blocks2x2_gfx_and_props( null, bank_data, block_n, null, 0, null, 0, img_buff );
							
						save_extra_gfx( img_buff, map_tiles_arr, bank_data, ( ushort )block_n, ( n_scr_Y * scr_height_blocks ), level_gfx, level_postfix_str );
					}
					
					level_bmp.Save( m_extra_path_filename + level_postfix_str + ".png", ImageFormat.Png );
					
					level_bmp.Dispose();
					level_gfx.Dispose();
				}

				// write collected data
				
				maps_CHRs_arr += "\t.byte " + chk_bank_ind + "\n";				
				
				// write level map
				{
					bw = new BinaryWriter( File.Open( m_path + m_map_data_filename, FileMode.Create ) );
					
					if( RBtnTilesDirRows.Checked )
					{
						utils.swap_columns_rows_order<byte>( map_tiles_arr, get_tiles_cnt_width( n_scr_X ), get_tiles_cnt_height( n_scr_Y ) );
					}
					
					compress_and_save_byte( bw, map_tiles_arr );
					
					map_size = bw.BaseStream.Length;
					
					bw.Close();
				}
				
				// write data to assembly file
				{
					_sw.WriteLine( "; *** " + level_prefix_str + " ***\n" );
					
					_sw.WriteLine( level_prefix_str + "_wscr\tequ " + n_scr_X + "\t\t; number of map screens in width" );
					_sw.WriteLine( level_prefix_str + "_hscr\tequ " + n_scr_Y + "\t\t; number of map screens in height" );
					
					_sw.WriteLine( level_prefix_str + "_wchr\tequ " + n_scr_X * ( platform_data.get_screen_blocks_width() << 1 ) + "\t\t; number of map CHRs in width" );
					_sw.WriteLine( level_prefix_str + "_hchr\tequ " + n_scr_Y * ( platform_data.get_screen_blocks_height() << 1 ) + "\t\t; number of map CHRs in height" );

					int map_tiles_width		= RBtnTiles2x2.Checked ? ( n_scr_X * scr_width_blocks ):( n_scr_X * ( scr_width_blocks >> 1 ) );
					int map_tiles_height	= RBtnTiles2x2.Checked ? ( n_scr_Y * scr_height_blocks ):( n_scr_Y * ( scr_height_blocks >> 1 ) );
					
					_sw.WriteLine( level_prefix_str + "_wtls\tequ " + map_tiles_width + "\t\t; number of map tiles in width" );
					_sw.WriteLine( level_prefix_str + "_htls\tequ " + map_tiles_height + "\t\t; number of map tiles in height" );
					
					_sw.WriteLine( level_prefix_str + "_u_tiles\tequ " + bank_data.get_first_free_block_id( false ) + "\t\t; number of unique tiles 2x2 (!)" );
					_sw.WriteLine( level_prefix_str + "_t_tiles\tequ " + map_tiles_arr.Length + "\t\t; total number of tiles in whole map" );
					
					_sw.WriteLine( "" );
					
					_sw.WriteLine( level_prefix_str + "_Map" + "\tincbin \"" + m_map_data_filename + "\"\t\t;" + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + map_size + ( CheckBoxRLE.Checked ? " / " + map_tiles_arr.Length:"" ) + ") game level " + ( RBtnTiles4x4.Checked ? "tiles (4x4)":"blocks (2x2)" ) + " array\n" );
				}
				
				// save layout and screens data
				{
					int start_scr_ind = level_data.get_start_screen_ind();
					
					if( start_scr_ind < 0 )
					{
						MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A first valid screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
					}

					// matrix layout by default
					{
						_sw.WriteLine( level_prefix_str + "_StartScr\tequ\t" + ( start_scr_ind < 0 ? 0:start_scr_ind ) + "\n" );
						
						if( CheckBoxExportMarks.Checked || CheckBoxExportEntities.Checked )
						{
							level_data.export_asm( _sw, level_prefix_str, "\tDEFINE", "db", "dw", "dw", "$", true, CheckBoxExportMarks.Checked, CheckBoxExportEntities.Checked, RBtnEntityCoordScreen.Checked, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex );
						}
					}
				}
			}
			
			// maps -> CHR bank
			{
				_sw.WriteLine( maps_CHRs_arr );
			}			
		}
		
		private void save_single_screen_mode( StreamWriter _sw )
		{
			BinaryWriter	bw;

			int[] img_buff = new int[ 64*4 ];
			
			layout_data level_data;
			
			List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
			
			int n_banks		= scr_tiles_data.Count;
			int n_levels 	= m_data_mngr.layouts_data_cnt;
			int n_scr_X;
			int n_scr_Y;
			int n_screens;
			int scr_ind;
			int scr_ind_opt;
			int bank_ind;
			int scr_key;
			int tile_n;
			int tile_offs_x;
			int tile_offs_y;
			int block_n;
			int n_blocks;
			int bank_n;
			int data_offset;
			int common_scr_ind;
			int max_scr_tile;
			int max_tile_ind;
			int start_scr_ind = -1;
			int ents_cnt;
			
			int scr_width_blocks	= platform_data.get_screen_tiles_width_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Blocks2x2 );
			int scr_height_blocks	= platform_data.get_screen_tiles_height_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Blocks2x2 );
			int scr_height_tiles	= platform_data.get_screen_tiles_height_by_platform_type_uni( platform_data.e_platform_type.ZX, data_sets_manager.e_screen_data_type.Tiles4x4 );

			ushort tile_id;
			ushort block_id;
			
			screen_data tile_inds;
			
			long data_size;

			bool valid_bank;
			bool enable_comments;
			
			string	chr_arr;
			string	clrs_offs;
			byte[]	clrs_arr;
			int		clr_size;
			int		clrs_arr_offset;
			
			byte[]	props_arr;
			int		props_arr_offset;
			int		props_size;
			
			string label;
			string level_prefix_str;
			string data_offset_str;
			string maps_arr;
			string scr_arr	= null;
			
			exp_screen_data			exp_scr;
			layout_screen_data		scr_data;
			tiles_data 				tiles;
			
			List< tiles_data > 	banks 			= new List< tiles_data >( 10 );
			List< int >			max_tile_inds	= new List< int >( 10 );
			
			exp_screen_data._tiles_offset  = 0;
			exp_screen_data._blocks_offset = 0;

			SortedDictionary< int, exp_screen_data > screens = new SortedDictionary< int, exp_screen_data >();

			scr_ind = 0;		// global screen index
			scr_ind_opt = 0;	// optimized screen index
#if !DEF_ZX
			platform_data.e_platform_type platform_type = platform_data.get_platform_type();
			
			int prj_scr_tiles_width		= platform_data.get_screen_tiles_width_uni( m_data_mngr.screen_data_type );
			int prj_scr_tiles_height	= platform_data.get_screen_tiles_height_uni( m_data_mngr.screen_data_type );
			
			int zx_scr_tiles_width		= platform_data.get_screen_tiles_width_by_platform_type_uni( platform_data.e_platform_type.ZX, m_data_mngr.screen_data_type );
			int zx_scr_tiles_height		= platform_data.get_screen_tiles_height_by_platform_type_uni( platform_data.e_platform_type.ZX, m_data_mngr.screen_data_type );
			
			bool crop_scr = false;
			
			if( ( zx_scr_tiles_width != prj_scr_tiles_width ) || ( zx_scr_tiles_height != prj_scr_tiles_height ) )
			{
				crop_scr = true;
				
				MainForm.message_box( "The " + platform_data.get_platform_name( platform_type ) + " screen resolution differs from the ZX one. Thus, the exported screens will be cropped to ZX resolution and aligned to the upper left corner.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			
			Func< int, int, bool > invalid_tile = ( int _x_pos, int _y_pos ) => ( crop_scr && ( ( _x_pos >= zx_scr_tiles_width ) || ( _y_pos >= zx_scr_tiles_height ) ) );
#endif
			// collect all banks & screens data in a project
			for( bank_n = 0; bank_n < m_data_mngr.tiles_data_cnt; bank_n++ )
			{
				tiles = m_data_mngr.get_tiles_data( bank_n );
				
				valid_bank = false;
				
				max_tile_ind = Int32.MinValue;
				
				for( int scr_n = 0; scr_n < tiles.screen_data_cnt(); scr_n++ )
				{
					if( check_screen_layouts( scr_ind ) == true )
					{
						if( scr_ind_opt > CONST_BIDIR_MAP_SCREEN_MAX_CNT - 1 )
						{
							throw new Exception( "The screen index is out of range!\nThe maximum number of screens allowed to export: " + CONST_BIDIR_MAP_SCREEN_MAX_CNT );
						}
						
						valid_bank = true;
						
						exp_scr = new exp_screen_data( scr_ind_opt++, tiles );
						
						screens[ ( bank_n << 8 ) | scr_n ] = exp_scr;
						
						if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
						{
							for( tile_n = 0; tile_n < platform_data.get_screen_tiles_cnt(); tile_n++ )
							{
								tile_offs_x = ( tile_n % platform_data.get_screen_tiles_width() );
								tile_offs_y = ( tile_n / platform_data.get_screen_tiles_width() );
#if !DEF_ZX
								if( invalid_tile( tile_offs_x, tile_offs_y ) )
								{
									continue;
								}
#endif
								tile_id = exp_scr.m_tiles.get_screen_tile( scr_n, tile_n );
		
								exp_scr.m_scr_tiles.set_tile( tile_offs_x * scr_height_tiles + tile_offs_y, tile_id );
								
								if( RBtnTiles2x2.Checked )
								{
									tile_offs_x <<= 1;
									tile_offs_y <<= 1;
									
									// fill the array of all tiles 2x2 in a level
									for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
									{
										block_id = ( byte )tiles.get_tile_block( tile_id, block_n );
										
										if( ( tile_offs_x + ( block_n & 0x01 ) ) < platform_data.get_screen_blocks_width() && ( tile_offs_y + ( ( block_n & 0x02 ) >> 1 ) ) < platform_data.get_screen_blocks_height() )
										{
											exp_scr.m_scr_blocks.set_tile( ( tile_offs_x * scr_height_blocks ) + ( ( block_n & 0x01 ) == 0x01 ? scr_height_blocks:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ), block_id );
										}
									}
								}
							}
						}
						else
						{
							for( block_n = 0; block_n < platform_data.get_screen_blocks_cnt(); block_n++ )
							{
								tile_offs_x = ( block_n % platform_data.get_screen_blocks_width() );
								tile_offs_y = ( block_n / platform_data.get_screen_blocks_width() );
#if !DEF_ZX
								if( invalid_tile( tile_offs_x, tile_offs_y ) )
								{
									continue;
								}
#endif								
								exp_scr.m_scr_blocks.set_tile( ( tile_offs_x * scr_height_blocks ) + tile_offs_y, tiles.get_screen_tile( scr_n, block_n ) );
							}
						}
						
						max_scr_tile = exp_scr.m_scr_tiles.max_val();
						
						if( max_tile_ind < max_scr_tile )
						{
							max_tile_ind = max_scr_tile;
						}
					}
					
					++scr_ind;
				}
				
				if( valid_bank == true )
				{
					banks.Add( tiles );
					
					max_tile_inds.Add( 1 + max_tile_ind );
				}
			}

			// global data writing
			{
				_sw.WriteLine( "\n; *** GLOBAL DATA ***\n" );
				
				chr_arr		= null;
				clrs_offs	= null;
				clrs_arr	= null;
				props_arr	= null;
				
				props_arr_offset	= 0;
				clrs_arr_offset		= 0;
				
				// CHR
				for( bank_n = 0; bank_n < banks.Count; bank_n++ )
				{
					tiles = banks[ bank_n ];
					
					// write CHR bank data
					label = "gfx" + bank_n;
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					{
						n_blocks = tiles.get_first_free_block_id( false );
						
						clr_size	= n_blocks << 2;
						props_size	= n_blocks * ( RBtnPropPerBlock.Checked ? 1:4 );

						if( clrs_arr == null )
						{
							clrs_arr = new byte[ clr_size ];
						}
						else
						{
							clrs_arr_offset = clrs_arr.Length;
							
							Array.Resize( ref clrs_arr, clrs_arr.Length + clr_size );
						}

						if( props_arr == null )
						{
							props_arr = new byte[ props_size ];
						}
						else
						{
							props_arr_offset = props_arr.Length;
							
							Array.Resize( ref props_arr, props_arr.Length + props_size );
						}
						
						for( block_n = 0; block_n < n_blocks; block_n++ )
						{
							build_and_save_blocks2x2_gfx_and_props( bw, tiles, block_n, props_arr, props_arr_offset, clrs_arr, clrs_arr_offset, img_buff );
						}
						
						data_size = n_blocks << 5;
						
						clrs_offs += "\tdw " + clrs_arr_offset + "\t\t; (" + label + ")\n";
					}
					bw.Close();
					
					_sw.WriteLine( label + ":\tincbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ") blocks 2x2  bank" + bank_n );
					
					chr_arr	 += "\tdw " + label + "\n";
				}

				_sw.WriteLine( "\n" + m_filename + "_Gfx:\n" + chr_arr );

				// blocks props
				{
					label = "_Props";
					
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					{
						bw.Write( props_arr );
					}
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ") block properties array of all exported data banks ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )" + ( RBtnPropPerBlock.Checked ? ", data offset = props offset / 4":"" ) + "\n" );
				}
				
				// blocks colors
				{
					label = "_Clrs";
					
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					{
						bw.Write( clrs_arr );
					}
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t\t; (" + clrs_arr.Length + ") blocks 2x2 color data of all exported data banks, 4 bytes per block ( left to right, top to bottom ), data offset = props offset" );
					
					_sw.WriteLine( "\n" + m_filename + "_BlocksPropsOffs:\n" + clrs_offs );
				}
				
				// tiles
				{
					if( RBtnTiles4x4.Checked )
					{
						// write tiles data
						label = "_Tiles";
						
						bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
						
						data_offset = 0;
						data_offset_str = "";
						
						// tiles
						for( bank_n = 0; bank_n < banks.Count; bank_n++ )
						{
							max_tile_ind = max_tile_inds[ bank_n ];	// one based index
	
							tiles = banks[ bank_n ];
							
							for( int i = 0; i < max_tile_ind; i++ )
							{
								bw.Write( tiles_data.tile_convert_ulong_to_uint_reverse( tiles.tiles[ i ] ) );
							}
							
							data_offset_str += "\tdw " + data_offset + "\t\t; (gfx" + bank_n + ")\n";
							
							data_offset += max_tile_inds[ bank_n ] << 2;
						}
						
						data_size = bw.BaseStream.Length;
						bw.Close();
					
						_sw.WriteLine( m_filename + label + ":\tincbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ") 4x4 tiles array of all exported data banks ( 4 bytes per tile )\n" );
						
						_sw.WriteLine( m_filename + "_TilesOffs:" );
	
						_sw.WriteLine( data_offset_str );
					}
				}
				
				// map
				{
					// tiles indices array for each screen
					label = "_TilesScr";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );

					foreach( var key in screens.Keys )
					{ 
						if( RBtnTiles4x4.Checked )
						{
							tile_inds = screens[ key ].m_scr_tiles;
						}
						else
						{
							tile_inds = screens[ key ].m_scr_blocks;
						}
						
						if( RBtnTilesDirRows.Checked )
						{
							tile_inds.swap_col_row_data();
						}
						
						for( int i = 0; i < tile_inds.length; i++ )
						{
							bw.Write( ( byte )tile_inds.get_tile( i ) );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") " + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + " tiles array for each screen ( " + ( RBtnTiles2x2.Checked ? ( scr_width_blocks * scr_height_blocks ):platform_data.get_screen_tiles_cnt() ) + " bytes per screen \\ 1 byte per tile )" );
				}
			}

			maps_arr = "\nMapsArr:\n";
			
			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				enable_comments = true;
				
				level_data = m_data_mngr.get_layout_data( level_n );
				
				level_prefix_str = CONST_FILENAME_LEVEL_PREFIX + level_n;
				
				maps_arr += "\tdw " + level_prefix_str + "_StartScr\n";
				
				check_ent_instances_cnt( level_data, level_prefix_str );

				_sw.WriteLine( "\n; *** " + level_prefix_str + " ***\n" );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				if( RBtnLayoutAdjacentScreenIndices.Checked )
				{
					scr_arr = level_prefix_str + "_ScrArr:";
				}
				
				ents_cnt = 0;
				
				for( int scr_n_Y = 0; scr_n_Y < n_scr_Y; scr_n_Y++ )
				{
					for( int scr_n_X = 0; scr_n_X < n_scr_X; scr_n_X++ )
					{
						scr_data = level_data.get_data( scr_n_X, scr_n_Y );
						
						if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
						{
							// determine which bank the screen belongs to
							n_screens = 0;
							for( bank_ind = 0; bank_ind < n_banks; bank_ind++ )
							{
								n_screens += scr_tiles_data[ bank_ind ].screen_data_cnt();
								
								if( scr_data.m_scr_ind < n_screens )
								{
									break;
								}
							}
							
							// convert a screen index into local index in the bank
							scr_ind = scr_data.m_scr_ind - ( n_screens - scr_tiles_data[ bank_ind ].screen_data_cnt() );
							
							scr_key = ( bank_ind << 8 ) | scr_ind;
							
							if( screens.ContainsKey( scr_key ) )
							{
								common_scr_ind = scr_n_Y * n_scr_X + scr_n_X;
								
								if( enable_comments )
								{
									start_scr_ind = level_data.get_start_screen_ind();
									
									if( start_scr_ind < 0 )
									{
										MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A first valid screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
									}
									
									start_scr_ind = ( start_scr_ind < 0 ) ? common_scr_ind:start_scr_ind;
								}
								
								exp_scr = screens[ scr_key ];
								
								_sw.WriteLine( level_prefix_str + "Scr" + common_scr_ind + ":" );
								_sw.WriteLine( "\tdb " + banks.IndexOf( exp_scr.m_tiles ) + ( enable_comments ? "\t; chr_id":"" ) );
								
								if( CheckBoxExportMarks.Checked )
								{
									_sw.WriteLine( "\tdb " + utils.hex( "$", scr_data.mark_adj_scr_mask ) + ( enable_comments ? "\t; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property":"" ) );
								}
								
								_sw.WriteLine( "\n\tdb " + exp_scr.m_scr_ind + ( enable_comments ? "\t; screen index":"" ) );
								
								_sw.WriteLine( "" );
								
								if( RBtnLayoutAdjacentScreens.Checked )
								{
									_sw.WriteLine( "\tdw " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, -1,	 0	) + ( enable_comments ? "\t; left adjacent screen":"" ) );
									_sw.WriteLine( "\tdw " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  0,	-1	) + ( enable_comments ? "\t; up adjacent screen":"" ) );
									_sw.WriteLine( "\tdw " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  1,	 0	) + ( enable_comments ? "\t; right adjacent screen":"" ) );
									_sw.WriteLine( "\tdw " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  0,	 1	) + ( enable_comments ? "\t; down adjacent screen\n":"\n" ) );
								}
								else
								if( RBtnLayoutAdjacentScreenIndices.Checked )
								{
									if( enable_comments )
									{
										_sw.WriteLine( "; adjacent screen indices ( the valid values are 0x00 - 0xFE, 0xFF - means no screen )" );
										_sw.WriteLine( "; use the " + level_prefix_str + "_ScrArr array to get a screen description by adjacent screen index" );
									}
									
									_sw.WriteLine( "\tdb " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, -1,	 0	) + ( enable_comments ? "\t; left adjacent screen index":"" ) );
									_sw.WriteLine( "\tdb " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  0,	-1	) + ( enable_comments ? "\t; up adjacent screen index":"" ) );
									_sw.WriteLine( "\tdb " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  1,	 0	) + ( enable_comments ? "\t; right adjacent screen index":"" ) );
									_sw.WriteLine( "\tdb " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  0,	 1	) + ( enable_comments ? "\t; down adjacent screen index\n":"\n" ) );
									
									scr_arr += "\n\tdw " + level_prefix_str + "Scr" + common_scr_ind;
								}
								
								if( CheckBoxExportEntities.Checked )
								{
									scr_data.export_entities_asm( _sw, ref ents_cnt, level_prefix_str + "Scr" + common_scr_ind + "EntsArr", "db", "dw", "dw", "$", RBtnEntityCoordScreen.Checked, scr_n_X, scr_n_Y, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex, enable_comments );
									
									_sw.WriteLine( "" );
								}
								
								enable_comments = false;
							}
							else
							{
								throw new Exception( "Unexpected error! Can't find a screen data!" );
							}
						}
					}
				}
				
				if( RBtnLayoutAdjacentScreenIndices.Checked )
				{
					_sw.WriteLine( "; screens array" );
					_sw.WriteLine( scr_arr + "\n" );
				}
				
				if( CheckBoxExportEntities.Checked )
				{
					_sw.WriteLine( "\tDEFINE " + level_prefix_str + "_EntInstCnt\t" + level_data.get_num_ent_instances() + "\t; number of entities instances\n" );
				}

				// set up start screen
				{
					if( RBtnLayoutMatrix.Checked )
					{
						_sw.WriteLine( level_prefix_str + "_StartScr\tequ\t" + start_scr_ind + "\n" );
						
						level_data.export_asm( _sw, level_prefix_str, "\tDEFINE", "db", "dw", "dw", "$", false, false, false, false, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex );
					}
					else
					{
						_sw.WriteLine( level_prefix_str + "_StartScr\t= " + level_prefix_str + "Scr" + start_scr_ind + "\n" );
					}
				}
			}
			
			// save MapsArr
			{
				_sw.WriteLine( maps_arr );
			}
			
			foreach( var key in screens.Keys ) { screens[ key ].destroy(); }
		}

		private string get_adjacent_screen_index( int _level_n, layout_data _data, int _scr_ind, int _x_offset, int _y_offset )
		{
			int adj_scr_ind = _data.get_adjacent_screen_index( _scr_ind, _x_offset, _y_offset, true );
			
			if( adj_scr_ind > 255 )
			{
				throw new Exception( "Layout: " + _level_n + " error!\nThe maximum number of cells in a layout must be 256 for the \"Adjacent Screen Indices\" mode!" );
			}
			
			return utils.hex( "$", ( adj_scr_ind >= 0 ? adj_scr_ind:255 ) );
		}
		
		private string get_adjacent_screen_label( int _level_n, layout_data _data, int _scr_ind, int _x_offset, int _y_offset )
		{
			int adj_scr_ind = _data.get_adjacent_screen_index( _scr_ind, _x_offset, _y_offset );
			
			return ( adj_scr_ind >= 0 ? CONST_FILENAME_LEVEL_PREFIX + _level_n + "Scr" + adj_scr_ind:"0" );
		}
		
		private bool check_screen_layouts( int _scr_ind )
		{
			layout_data data;
			
			int n_scr_X;
			int n_scr_Y;
			
			for( int layout_n = 0; layout_n < m_data_mngr.layouts_data_cnt; layout_n++ )
			{
				data = m_data_mngr.get_layout_data( layout_n );
				
				n_scr_X = data.get_width();
				n_scr_Y = data.get_height();
				
				for( int scr_n_X = 0; scr_n_X < n_scr_X; scr_n_X++ )
				{
					for( int scr_n_Y = 0; scr_n_Y < n_scr_Y; scr_n_Y++ )
					{
						if( data.get_data( scr_n_X, scr_n_Y ).m_scr_ind == _scr_ind )
						{
							return true;
						}
					}
				}
			}
			
			return false;
		}

		private void build_and_save_blocks2x2_gfx_and_props(	BinaryWriter	_bw,
																tiles_data		_data,
																int				_tile_n,
																byte[]			_tile_props_arr,
																int				_props_arr_offset,
																byte[]			_tile_colors_arr,
																int				_clrs_arr_offset,
																int[] 			_img_buff )
		{
			uint	block_data	= 0;
			byte	pixel		= 0;
			byte	pixel_ind	= 0;
			
			int		pix_val 	= 0;
#if DEF_NES
			int[] palette 		= null;
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
			palette16_data plt16 = null;
#endif
			byte[] ink_clr_weights		= new byte[ utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ];
			byte[] paper_clr_weights	= new byte[ utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ];

			double ink_factor	= ( double )( NumericUpDownInkFactor.Value ) / 10.0;
			
			int pix_n;
			int chr_n;
			
			int[] ink_color_index 		= new int[ 4 ];
			int[] paper_color_index 	= new int[ 4 ];
#if DEF_ZX
			bool[] bright_index			= new bool[ 4 ];
#else
			double	dither_factor;
			int		max_weight_ind;
			double	bright_val;
			bool	no_ink;
#endif
			int zx_ink_color_index 		= 0;
			int zx_paper_color_index 	= 0;
			bool zx_bright_flag			= false;
			
			byte[] mc_clr08 = new byte[64]{ 1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,1,1,1,1,1,1 };
											
			byte[] mc_clr07 = new byte[64]{ 0,1,1,1,0,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,0,1,1,1,0,1,
											1,1,1,1,1,1,1,1,
											0,1,1,1,0,1,1,1,
											1,1,1,1,1,1,1,1,
											1,1,0,1,1,1,0,1,
											1,1,1,1,1,1,1,1 };

			byte[] mc_clr06 = new byte[64]{ 0,1,0,1,0,1,0,1,
											1,1,1,1,1,1,1,1,
											0,1,0,1,0,1,0,1,
											1,1,1,1,1,1,1,1,
											0,1,0,1,0,1,0,1,
											1,1,1,1,1,1,1,1,
											0,1,0,1,0,1,0,1,
											1,1,1,1,1,1,1,1 };
											
			byte[] mc_clr05 = new byte[64]{ 0,1,0,1,0,1,0,1,
											1,1,1,0,1,1,1,0,
											0,1,0,1,0,1,0,1,
											1,0,1,1,1,0,1,1,
											0,1,0,1,0,1,0,1,
											1,1,1,0,1,1,1,0,
											0,1,0,1,0,1,0,1,
											1,0,1,1,1,0,1,1 };

			byte[] mc_clr04 = new byte[64]{ 0,1,0,1,0,1,0,1,
											1,0,1,0,1,0,1,0,
											0,1,0,1,0,1,0,1,
											1,0,1,0,1,0,1,0,
											0,1,0,1,0,1,0,1,
											1,0,1,0,1,0,1,0,
											0,1,0,1,0,1,0,1,
											1,0,1,0,1,0,1,0 };

			byte[] mc_clr03 = new byte[64]{ 0,1,0,1,0,1,0,1,
											1,0,0,0,1,0,0,0,
											0,1,0,1,0,1,0,1,
											0,0,1,0,0,0,1,0,
											0,1,0,1,0,1,0,1,
											1,0,0,0,1,0,0,0,
											0,1,0,1,0,1,0,1,
											0,0,1,0,0,0,1,0 };
											
			byte[] mc_clr02 = new byte[64]{ 0,1,0,1,0,1,0,1,
											0,0,0,0,0,0,0,0,
											0,1,0,1,0,1,0,1,
											0,0,0,0,0,0,0,0,
											0,1,0,1,0,1,0,1,
											0,0,0,0,0,0,0,0,
											0,1,0,1,0,1,0,1,
											0,0,0,0,0,0,0,0 };

			byte[] mc_clr01 = new byte[64]{ 0,1,0,0,0,1,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,1,0,0,0,1,
											0,0,0,0,0,0,0,0,
											0,1,0,0,0,1,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,1,0,0,0,1,
											0,0,0,0,0,0,0,0 };

			byte[] mc_clr00 = new byte[64]{ 0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0,
											0,0,0,0,0,0,0,0 };
											
			byte[][] dither_ptrn_arr = new byte[4][]{ mc_clr04, mc_clr02, mc_clr01, mc_clr00 };
			double[] bright_dither_ptrn_arr = new double[4]{ 0.25, 0.5, 0.75, 1.0 };

			byte[] 	tile_2x2_data_arr 	= new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT * utils.CONST_BLOCK_SIZE ];
			
			for( chr_n = 0; chr_n < 4; chr_n++ )
			{
				block_data 	= _data.blocks[ ( _tile_n << 2 ) + chr_n ];
				
				_data.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( block_data ), tile_2x2_data_arr, chr_n << 6 );
#if DEF_ZX
				byte ink_clr	= 0xff;
				byte paper_clr	= 0xff;
				
				_data.get_ink_paper_colors( tiles_data.get_block_CHR_id( block_data ), ref ink_clr, ref paper_clr );
				
				ink_color_index[ chr_n ]	= ink_clr;
				paper_color_index[ chr_n ]	= paper_clr;
				bright_index[ chr_n ] 		= tiles_data.get_block_flags_palette( block_data ) > 0 ? true:false;
#endif
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
				if( ( tiles_data.get_block_flags_flip( block_data ) & utils.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 )
				{
					tiles_data.hflip( tile_2x2_data_arr, chr_n << 6 );
				}
				
				if( ( tiles_data.get_block_flags_flip( block_data ) & utils.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 )
				{
					tiles_data.vflip( tile_2x2_data_arr, chr_n << 6 );
				}
#endif
				if( _tile_props_arr != null )
				{
					if( RBtnPropPerBlock.Checked )
					{
						if( chr_n == 0 )
						{
							_tile_props_arr[ _tile_n + _props_arr_offset ] = ( byte )tiles_data.get_block_flags_obj_id( block_data );
						}
					}
					else
					{
						_tile_props_arr[ ( _tile_n << 2 ) + chr_n + _props_arr_offset ] = ( byte )tiles_data.get_block_flags_obj_id( block_data );
					}
				}
#if !DEF_ZX
	           	// draw ZX tile and get ink&paper colors
				Array.Clear( ink_clr_weights, 0, ink_clr_weights.Length );
				Array.Clear( paper_clr_weights, 0, paper_clr_weights.Length );
				
				no_ink = true;
#endif
#if DEF_NES
				palette = _data.subpalettes[ tiles_data.get_block_flags_palette( block_data ) ];
#elif DEF_SMS
				plt16 = _data.palettes_arr[ 0 ];
#elif DEF_PALETTE16_PER_CHR
				plt16 = _data.palettes_arr[ tiles_data.get_block_flags_palette( block_data ) ];
#endif
				for( pix_n = 0; pix_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; pix_n++ )
				{
					pixel_ind 	= ( byte )( ( chr_n << 6 ) + pix_n );
					pixel 		= tile_2x2_data_arr[ pixel_ind ];
#if DEF_ZX
					if( pixel == ink_color_index[ chr_n ] )
					{
						tile_2x2_data_arr[ pixel_ind ] = ( byte )1;
					}
					else
					if( pixel == paper_color_index[ chr_n ] )
					{
						tile_2x2_data_arr[ pixel_ind ] = ( byte )0;
					}
#else
#if DEF_NES
					bright_val = get_brightness( palette_group.Instance.main_palette[ palette[ pixel ] ] ) / 256.0;
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
					bright_val = get_brightness( palette_group.Instance.main_palette[ plt16.subpalettes[ pixel >> 2 ][ pixel & 0x03 ] ] ) / 256.0;
#endif
					if( ink_factor <= bright_val )
					{
						// paper
						if( CheckBoxGFXDithering.Checked )
						{
							dither_factor = ( bright_val - ink_factor ) / ( 1.0 - ink_factor );
							
							for( int j = 0; j < dither_ptrn_arr.Length; j++ )
							{
								if( dither_factor < bright_dither_ptrn_arr[ j ] )
								{
									tile_2x2_data_arr[ pixel_ind ] = ( byte )dither_ptrn_arr[ j ][ pix_n ];
									break;
								}
							}
						}
						else
						{
							tile_2x2_data_arr[ pixel_ind ] = ( byte )0;
						}
						
						++paper_clr_weights[ pixel ];
					}
					else
					{
						// ink
						tile_2x2_data_arr[ pixel_ind ] = ( byte )1;
						
						++ink_clr_weights[ pixel ];
						
						no_ink = false;
					}
#endif //DEF_ZX
				}
#if !DEF_ZX
				max_weight_ind = Array.IndexOf( paper_clr_weights, paper_clr_weights.Max() );
#if DEF_NES
				paper_color_index[ chr_n ] = palette[ max_weight_ind ];
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
				paper_color_index[ chr_n ] = plt16.subpalettes[ max_weight_ind >> 2 ][ max_weight_ind & 0x03 ];
#endif								
				// reset paper color to avoid equal colors on both ink and paper
				ink_clr_weights[ max_weight_ind ] = 0;
				
				if( !no_ink )
				{
					max_weight_ind = Array.IndexOf( ink_clr_weights, ink_clr_weights.Max() );
#if DEF_NES
					ink_color_index[ chr_n ] = palette[ max_weight_ind ];
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
					ink_color_index[ chr_n ] = plt16.subpalettes[ max_weight_ind >> 2 ][ max_weight_ind & 0x03 ];
#endif								
				}
#if DEF_NES									
				else
				{
					ink_color_index[ chr_n ] |= 13;
				}
#endif
#endif //!DEF_ZX
			}
			
			// convert tile into speccy format and write it to a file
			if( _bw != null )
			{
				save_tiles_gfx( _bw, tile_2x2_data_arr );
			}
			
			if( RBtnTypeColor.Checked )
			{
				for( chr_n = 0; chr_n < 4; chr_n++ )
				{
#if DEF_ZX
					zx_paper_color_index 	= paper_color_index[ chr_n ] & 0x07;
					zx_ink_color_index 		= ink_color_index[ chr_n ] & 0x07;
					zx_bright_flag			= bright_index[ chr_n ];
					
					if( zx_paper_color_index == zx_ink_color_index )
					{
						zx_ink_color_index ^= 0x07;
					}
#else
					zx_paper_color_index 	= get_nearest_color( palette_group.Instance.main_palette[ paper_color_index[ chr_n ] ] );
					zx_ink_color_index 		= get_nearest_color( palette_group.Instance.main_palette[ ink_color_index[ chr_n ] ] );
					
					if( ( zx_paper_color_index & 0x07 ) == ( zx_ink_color_index & 0x07 ) )
					{
						if( ( zx_paper_color_index & 0x07 ) == 0 )
						{
							zx_paper_color_index = 1;
						}
						else
						{
							// reset ink color to avoid equal colors on both ink and paper
							zx_ink_color_index = 0;
						}
					}

					zx_bright_flag = false;
					
					if( zx_ink_color_index > 7 )
					{
						zx_ink_color_index &= 0x07;
						zx_bright_flag = true;
					}
					
					if( zx_paper_color_index > 7 )
					{
						zx_paper_color_index &= 0x07;
						zx_bright_flag = true;
					}
#endif //DEF_ZX
					if( _tile_colors_arr != null )
					{
						_tile_colors_arr[ ( _tile_n << 2 ) + chr_n + _clrs_arr_offset ] |= ( byte )( ( zx_bright_flag ? 64:0 ) | ( zx_paper_color_index << 3 ) | zx_ink_color_index );
					}
					
					for( pix_n = 0; pix_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; pix_n++ )
					{
						pix_val = tile_2x2_data_arr[ ( chr_n << 6 ) + pix_n ];
						
						if( pix_val == 0 )
						{
							// paper
							pix_val = 0xFF << 24 | zx_palette[ zx_paper_color_index + ( zx_bright_flag ? 8:0 ) ];
						}
						else
						{
							// ink
							pix_val = 0xFF << 24 | zx_palette[ zx_ink_color_index + ( zx_bright_flag ? 8:0 ) ];
						}
						
						_img_buff[ ( (chr_n&0x01) == 0x01 ? 8:0 ) + ( (chr_n&0x02) == 0x02 ? 128:0 ) + ( ( pix_n >> 3 ) << 4 ) + ( pix_n % 8 ) ] = pix_val;
					}
				}
			}
			else
			{
				for( chr_n = 0; chr_n < 4; chr_n++ )
				{
					for( pix_n = 0; pix_n < 64; pix_n++ )
					{
						pix_val = tile_2x2_data_arr[ ( chr_n << 6 ) + pix_n ] - 1;
					
						_img_buff[ ( (chr_n&0x01) == 0x01 ? 8:0 ) + ( (chr_n&0x02) == 0x02 ? 128:0 ) + ( ( pix_n >> 3 ) << 4 ) + ( pix_n % 8 ) ] = 0xFF << 24 | pix_val << 16 | pix_val << 8 | pix_val;
					}
				}
			}
		}

		private void save_extra_gfx(	int[]		_img_buff,
										byte[]		_map_data_arr,
										tiles_data	_data,
										ushort		_tile_n,
										int			_map_height_tiles_cnt,
										Graphics	_level_gfx,
										string		_level_postfix_str )
		{
			// draw tile into bitmap
			GCHandle	tile_mem_handle = GCHandle.Alloc( _img_buff, GCHandleType.Pinned );
			
			Bitmap		tile_bmp = new Bitmap( 16, 16, 16<<2, PixelFormat.Format32bppArgb, tile_mem_handle.AddrOfPinnedObject() );
			
			if( CheckBoxRenderLevelPNG.Checked )
			{
				if( RBtnTiles2x2.Checked )
				{
					for( int map_tile_n = 0; map_tile_n < _map_data_arr.Length; map_tile_n++ )
					{
						if( _map_data_arr[ map_tile_n ] == ( _tile_n & 0x00ff ) )
						{
							_level_gfx.DrawImage( tile_bmp, ( map_tile_n / _map_height_tiles_cnt ) << 4, ( map_tile_n % _map_height_tiles_cnt ) << 4 );
						}
					}
				}
				else
				{
					ulong tile_ind;
					
					_map_height_tiles_cnt >>= 1;
					
					for( int map_tile_n = 0; map_tile_n < _map_data_arr.Length; map_tile_n++ )
					{
						tile_ind = _data.tiles[ _map_data_arr[ map_tile_n ] & 0x00ff ];
						
						for( int block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
						{
							if( utils.get_ushort_from_ulong( tile_ind, block_n ) == ( _tile_n & 0x00ff ) )
							{
								_level_gfx.DrawImage( tile_bmp, ( ( map_tile_n / _map_height_tiles_cnt ) << 5 ) + ( ( block_n & 0x01 ) == 0x01 ? 16:0 ), ( ( map_tile_n % _map_height_tiles_cnt ) << 5 ) + ( ( block_n & 0x02 ) == 0x02 ? 16:0 ) );
							}
						}
					}
				}
			}

			tile_mem_handle.Free();

			if( CheckBoxRenderTilesPNG.Checked )
			{
				tile_bmp.Save( m_extra_path_filename + _level_postfix_str + "_Tile" + _tile_n + ".png", ImageFormat.Png );
			}
			tile_bmp.Dispose();
		}

		private double get_brightness( int _XRGB )
		{
			double r = ( double )( ( _XRGB >> 16 ) & 0xff );
			double g = ( double )( ( _XRGB >> 8 ) & 0xff );
			double b = ( double )( _XRGB & 0xff );
			
			return Math.Sqrt( ( 0.299 * r*r ) + ( 0.587 * g*g ) + ( 0.114 * b*b ) );
		}
		
		private int get_nearest_color( int _color )
		{
			double zx_r;
			double zx_g;
			double zx_b;
			
			int zx_color;
			
			double 	fi			= 0.0;
			double 	fi_min 		= 1000000.0;
			int 	best_color 	= -1;

			double r = ( double )( ( _color >> 16 ) & 0xff );
			double g = ( double )( ( _color >> 8 ) & 0xff );
			double b = ( double )( _color & 0xff );
			
			for( int zx_color_n = 0; zx_color_n < 16; zx_color_n++ )
			{
				zx_color = zx_palette[ zx_color_n ];
				
				zx_r = ( double )( ( zx_color >> 16 ) & 0xff );
				zx_g = ( double )( ( zx_color >> 8 ) & 0xff );
				zx_b = ( double )( zx_color & 0xff );

				switch( CBoxColorConversionModes.SelectedIndex )
				{
					case 0:
						{
							fi = 30.0 * Math.Pow( r - zx_r, 2.0 ) + 59.0 * Math.Pow( g - zx_g, 2.0 ) + 11.0 * Math.Pow( b - zx_b, 2.0 );
						}
						break;
						
					case 1:
						{
							fi = Math.Sqrt( Math.Pow( r - zx_r, 2.0 ) + Math.Pow( g - zx_g, 2.0 ) + Math.Pow( b - zx_b, 2.0 ) );
						}
						break;
				}
				
				if( fi < fi_min )
				{
					best_color 	= zx_color_n;
					fi_min 		= fi;
				}
			}
			
			return best_color;
		}
		
		private void save_tiles_gfx( BinaryWriter _bw, byte[] _tile_2x2_data_arr )
		{
			save_tile_2x8( _bw, _tile_2x2_data_arr, 0 );
			save_tile_2x8( _bw, _tile_2x2_data_arr, 128 );
		}
		
		private void save_tile_2x8( BinaryWriter _bw, byte[] _tile_2x2_data_arr, int _offset )
		{
			int		pix_offset;
			ushort	word;
			
			for( int i = 0; i < 8; i++ )
			{
				word = 0;
				
				for( int j = 0; j < 8; j++ )
				{
					pix_offset = _offset + ( i << 3 ) + 7 - j;
					
					word |= ( ushort )( ( ( _tile_2x2_data_arr[ pix_offset ] & 0x01 ) << j ) | ( _tile_2x2_data_arr[ 64 + pix_offset ] & 0x01 ) << ( 8 + j ) );
				}
				
				_bw.Write( word );
			}
		}

		private void compress_and_save_byte( BinaryWriter _bw, byte[] _data, ref int _data_offset )
		{
			try
			{
				if( CheckBoxRLE.Checked )
				{
					byte[] rle_data_arr	= null;
	
					int rle_data_size = utils.RLE8( _data, ref rle_data_arr, true );
	
					if( rle_data_size < 0 )
					{
						// save uncompressed data
						//...
					}
					else
					{
						_bw.Write( rle_data_arr, 0, rle_data_size );
						
						_data_offset += rle_data_size;
					}
					
					rle_data_arr = null;
				}
				else
				{
					_bw.Write( _data );
					
					_data_offset += _data.Length;
				}
			}
			catch( Exception )
			{
				_bw.Dispose();
				
				throw;
			}
		}

		private void compress_and_save_byte( BinaryWriter _bw, byte[] _data )
		{
			int offset = 0;
			
			compress_and_save_byte( _bw, _data, ref offset );
		}
		
		private int get_tiles_cnt_width( int _scr_cnt_x )
		{
			return RBtnTiles2x2.Checked ? _scr_cnt_x * platform_data.get_screen_blocks_width():_scr_cnt_x * platform_data.get_screen_tiles_width();
		}

		private int get_tiles_cnt_height( int _scr_cnt_y )
		{
			return RBtnTiles2x2.Checked ? _scr_cnt_y * platform_data.get_screen_blocks_height():_scr_cnt_y * platform_data.get_screen_tiles_height();
		}
		
		private void save_export_options( StreamWriter _sw )
		{
			// options
			{
				_sw.WriteLine( "; export options:" );

				_sw.WriteLine( ";\t- tiles " + ( RBtnTiles4x4.Checked ? "4x4":"2x2" ) + ( CheckBoxRLE.Checked ? " (RLE)":"" ) + ( RBtnTilesDirColumns.Checked ? "/(columns)":"/(rows)" ) + ( RBtnTypeColor.Checked ? "/color":"/monochrome" ) );
				
				_sw.WriteLine( ";\t- properties per " + ( RBtnPropPerBlock.Checked ? "block":"CHR" ) );
				
				if( RBtnModeMultidirScroll.Checked )
				{
					_sw.WriteLine( ";\t- mode: multidirectional scrolling" );
				}
				else
				{
					_sw.WriteLine( ";\t- mode: bidirectional scrolling" );
				}
				
				_sw.WriteLine( ";\t- layout: " + ( RBtnLayoutAdjacentScreens.Checked ? "adjacent screens":RBtnLayoutAdjacentScreenIndices.Checked ? "adjacent screen indices":"matrix" ) + ( CheckBoxExportMarks.Checked ? " (marks)":" (no marks)" ) );
				
				_sw.WriteLine( ";\t- " + ( CheckBoxExportEntities.Checked ? "entities " + ( RBtnEntityCoordScreen.Checked ? "(screen coordinates)":"(map coordinates)" ) + ( CBoxEntSortingType.SelectedIndex == 0 ? "/no sorting":( CBoxEntSortingType.SelectedIndex == 1 ) ? "/left-right":"/bottom-top" ):"no entities" ) );
				_sw.WriteLine( "\n" );
			}
			
			_sw.WriteLine( "MAP_DATA_MAGIC = " + utils.hex( "$", ( RBtnTiles2x2.Checked ? 1:2 ) | 
																( CheckBoxRLE.Checked ? 4:0 ) |
																( RBtnTilesDirColumns.Checked ? 8:16 ) |
																( RBtnModeMultidirScroll.Checked ? 32:RBtnModeBidirScroll.Checked ? 64:128 ) | 
																( CheckBoxExportEntities.Checked ? 256:0 ) |
																( CheckBoxExportEntities.Checked ? ( RBtnEntityCoordScreen.Checked ? 512:1024 ):0 ) |
																( RBtnLayoutAdjacentScreens.Checked ? 2048:RBtnLayoutAdjacentScreenIndices.Checked ? 4096:8192 ) |
																( CheckBoxExportMarks.Checked ? 16384:0 ) |
																( RBtnPropPerBlock.Checked ? 32768:65536 ) |
																( RBtnTypeColor.Checked ? 131072:0 ) |
																( CBoxEntSortingType.SelectedIndex == 0 ? 262144:0 ) |
																( CBoxEntSortingType.SelectedIndex == 1 ? 524288:0 ) |
																( CBoxEntSortingType.SelectedIndex == 2 ? 1048576:0 ) ) );
			_sw.WriteLine( "\n; data flags:" );
			_sw.WriteLine( "MAP_FLAG_TILES2X2                 = " + utils.hex( "$", 1 ) );
			_sw.WriteLine( "MAP_FLAG_TILES4X4                 = " + utils.hex( "$", 2 ) );
			_sw.WriteLine( "MAP_FLAG_RLE                      = " + utils.hex( "$", 4 ) );
			_sw.WriteLine( "MAP_FLAG_DIR_COLUMNS              = " + utils.hex( "$", 8 ) );
			_sw.WriteLine( "MAP_FLAG_DIR_ROWS                 = " + utils.hex( "$", 16 ) );
			_sw.WriteLine( "MAP_FLAG_MODE_MULTIDIR_SCROLL     = " + utils.hex( "$", 32 ) );
			_sw.WriteLine( "MAP_FLAG_MODE_BIDIR_SCROLL        = " + utils.hex( "$", 64 ) );
			_sw.WriteLine( "MAP_FLAG_MODE_STATIC_SCREENS      = " + utils.hex( "$", 128 ) );
			_sw.WriteLine( "MAP_FLAG_ENTITIES                 = " + utils.hex( "$", 256 ) );
			_sw.WriteLine( "MAP_FLAG_ENTITY_SCREEN_COORDS     = " + utils.hex( "$", 512 ) );
			_sw.WriteLine( "MAP_FLAG_ENTITY_MAP_COORS         = " + utils.hex( "$", 1024 ) );
			_sw.WriteLine( "MAP_FLAG_LAYOUT_ADJACENT_SCREENS  = " + utils.hex( "$", 2048 ) );
			_sw.WriteLine( "MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS = " + utils.hex( "$", 4096 ) );
			_sw.WriteLine( "MAP_FLAG_LAYOUT_MATRIX            = " + utils.hex( "$", 8192 ) );
			_sw.WriteLine( "MAP_FLAG_MARKS                    = " + utils.hex( "$", 16384 ) );
			_sw.WriteLine( "MAP_FLAG_PROP_ID_PER_BLOCK        = " + utils.hex( "$", 32768 ) );
			_sw.WriteLine( "MAP_FLAG_PROP_ID_PER_CHR          = " + utils.hex( "$", 65536 ) );
			_sw.WriteLine( "MAP_FLAG_COLOR_TILES              = " + utils.hex( "$", 131072 ) );
			_sw.WriteLine( "MAP_FLAG_ENT_SORTING_OFF          = " + utils.hex( "$", 262144 ) );
			_sw.WriteLine( "MAP_FLAG_ENT_SORTING_LT_RT        = " + utils.hex( "$", 524288 ) );
			_sw.WriteLine( "MAP_FLAG_ENT_SORTING_BTM_TOP      = " + utils.hex( "$", 1048576 ) );
			
			_sw.WriteLine( "\nSCR_BLOCKS2x2_WIDTH\tequ " + platform_data.get_screen_blocks_width() + "\t\t; number of screen blocks (2x2) in width" );
			_sw.WriteLine( "SCR_BLOCKS2x2_HEIGHT\tequ " + platform_data.get_screen_blocks_height() + "\t\t; number of screen blocks (2x2) in height\n" );
		}
		
		private void check_ent_instances_cnt( layout_data _layout, string _lev_pref_str )
		{
			if( CheckBoxExportEntities.Checked )
			{
				if( _layout.get_num_ent_instances() > utils.CONST_MAX_ENT_INST_CNT )
				{
					throw new Exception( "The number of entity instances is out of range!\nThe maximum number allowed to export: " + utils.CONST_MAX_ENT_INST_CNT + "\n\n[" + _lev_pref_str + "]" );
				}
			}
		}
	}
}