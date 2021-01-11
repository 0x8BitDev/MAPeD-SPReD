/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_zx_sjasm.
	/// </summary>
	public partial class exporter_zx_sjasm : Form
	{
		private data_sets_manager m_data_mngr = null;
		
		private string 	m_path_filename_ext		= null;
		private string 	m_path_filename			= null;
		private string 	m_extra_path_filename	= null;
		
		private const string	CONST_FILENAME_LEVEL_POSTFIX	= "_Lev";
		private const string	CONST_FILENAME_LEVEL_PREFIX		= "Lev";
		
		private string	m_tiles_data_filename	= null;
		private string	m_tile_props_filename	= null;
		private string	m_tile_colrs_filename	= null;
		private string	m_map_data_filename		= null;
		private string	m_filename				= null;
		private string	m_path					= null;
				
		// 7: flashing, 6: brightness, 3-5: paper color, 0-2: ink color
		private static readonly int[] zx_palette = new int[]{ 0x000000, 0x0022c7, 0xd62816, 0xd433c7, 0x00c525, 0x00c7c9, 0xccc82a, 0xcacaca,	// not bright
															  0x000000, 0x002bfb, 0xff331c, 0xff40fc, 0x00f92f, 0x00fbfe, 0xfffc36, 0xffffff };	// bright
		
		public static int[] zx_clrs_conv_tbl = new int[]{ 	 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1,
															 -1, -1, -1, -1, -1, -1, -1, -1 };
		
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
			ExpZXAsmColorConversionModes.SelectedIndex = 0;
			
			update_desc();
		}
		
		void event_cancel(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void ParamChanged_Event(object sender, EventArgs e)
		{
			update_desc();
		}

		void ExpEntitiesChanged_Event(object sender, EventArgs e)
		{
			GrpBoxEntCoords.Enabled = ExpZXAsmExportEntities.Checked ? true:false;
			
			update_desc();
		}
		
		private void update_desc()
		{
			if( ExpZXAsmTiles2x2.Checked )
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_TILES_2X2;
			}
			else
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_TILES_4X4;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER;
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ZX_DATA_ORDER_COLS;

			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PROP;
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PROP_PER_BLOCK;
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE;
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE_MULTIDIR;
			
			if( ExpZXAsmExportMarks.Checked || ExpZXAsmExportEntities.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_LAYOUT;
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_LAYOUT_MATRIX;
			}
			
			if( ExpZXAsmExportMarks.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MARKS;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NO_MARKS;
			}
			
			if( ExpZXAsmExportEntities.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENTITIES;
				
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_COORDS;
				
				if( ExpZXAsmEntScreenCoords.Checked )
				{
					RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_COORDS_SCR;
				}
				else
				{
					RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_COORDS_MAP;
				}				
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NO_ENTITIES;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ZX_INK_FACTOR;
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_WARNING;
		}
		
		public void ShowDialog( string _full_path )
		{
			m_path_filename_ext 	= _full_path;
			m_filename				= Path.GetFileNameWithoutExtension( _full_path );
			m_path					= Path.GetDirectoryName( _full_path ) + Path.DirectorySeparatorChar;
			m_path_filename			= m_path + m_filename;
			m_extra_path_filename	= Path.GetDirectoryName( _full_path ) + Path.DirectorySeparatorChar + m_filename + "_Extra" + Path.DirectorySeparatorChar + m_filename;
			
			ShowDialog();				
		}
		
		void event_ok(object sender, EventArgs e)
		{
			this.Close();
			
			if( ExpZXAsmRenderLevelPNG.Checked || ExpZXAsmRenderTilesPNG.Checked )
			{
				System.IO.Directory.CreateDirectory( m_path_filename + "_Extra" + Path.DirectorySeparatorChar );
			}
			
			calc_colors_conversion_table();
			
			try
			{
				// .a file includes:
				// unique tiles2x2 arr
				// tile properties
				// tile colors
				// level map
				BinaryWriter 	bw = null;
				StreamWriter 	sw = null;
				
				List< ushort >	unique_tiles_arr	= new List< ushort >();
				ushort[]		map_tiles_arr		= null;
				byte[]			tile_props_arr		= null;
				byte[] 			tile_2x2_data_arr 	= new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT * utils.CONST_BLOCK_SIZE ];
//				byte[] 			tile_4x4_data_arr 	= new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT * ( utils.CONST_BLOCK_SIZE << 2 ) ];
				byte[]			tile_colors_arr		= null;
				
				layout_data level_data = null;
				
				int n_levels 	= m_data_mngr.layouts_data_cnt;
				int n_scr_X 	= 0;
				int n_scr_Y		= 0;
				int scr_ind		= 0;
				
				List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
				
				tiles_data tiles = null;
				
				int	map_data_size	= 0;
				int tile_offs_x		= 0;
				int tile_offs_y		= 0;
				int n_Y_tiles		= 0;
				int bank_ind		= 0;
				int n_banks			= scr_tiles_data.Count;
				int n_screens 		= 0;
				byte tile_id		= 0;
				ushort tile_data	= 0;
				uint block_data		= 0;
				
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

				double bright_val;
				
				byte   pixel		= 0;
				byte   pixel_ind	= 0;
				
				int[] 		img_buff 		= new int[ 64*4 ];
				int 		pix_val 		= 0;
				GCHandle 	tile_mem_handle;
				Bitmap 		tile_bmp		= null;
				Bitmap 		level_bmp 		= null;
				Graphics 	level_gfx		= null;
#if DEF_NES
				int[] palette 		= null;
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
				palette16_data plt16 = null;
#endif
				byte[] ink_clr_weights		= new byte[ utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ];
				byte[] paper_clr_weights	= new byte[ utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ];
				
				double dither_factor;
				double ink_factor	= ( double )( ExpZXAsmInkFactor.Value ) / 10.0;
				int max_weight_ind;
				int pix_n;
				int chr_n;
				
				bool no_ink;
				
				int ink_color_index 	= 0;
				int paper_color_index 	= 0;
				int zx_ink_color_index 		= 0;
				int zx_paper_color_index 	= 0;
				bool zx_bright_flag			= false;
				
				long level_data_size = 0;
				
				int scr_width_blocks 	= utils.CONST_SCREEN_NUM_WIDTH_BLOCKS;
				int scr_height_blocks 	= utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS;
				
				screen_data scr_data;
				
				sw = new StreamWriter( m_path_filename_ext );
				
				utils.write_title( sw );
				
				save_export_options( sw );
				
				if( ExpZXAsmExportEntities.Checked )
				{
					m_data_mngr.export_entity_asm( sw, "db", "#" );
				}
				
				sw.WriteLine( "\nscr_2x2tiles_w\tequ " + scr_width_blocks + "\t\t; number of screen tiles 2x2 in width" );
				
#if DEF_SCREEN_HEIGHT_7d5_TILES
				scr_height_blocks -= 1;
				
				sw.WriteLine( "scr_2x2tiles_h\tequ " + scr_height_blocks + "\t\t; number of screen tiles 2x2 in height\n" );
#else
				sw.WriteLine( "scr_2x2tiles_h\tequ " + scr_height_blocks + "\t\t; number of screen tiles 2x2 in height\n" );
#endif				

				for( int level_n = 0; level_n < n_levels; level_n++ )
				{
					m_tiles_data_filename	= m_filename + CONST_FILENAME_LEVEL_POSTFIX + level_n + "_Tiles.bin";
					m_tile_props_filename	= m_filename + CONST_FILENAME_LEVEL_POSTFIX + level_n + "_TilePrps.bin";
					m_tile_colrs_filename	= m_filename + CONST_FILENAME_LEVEL_POSTFIX + level_n + "_TileClrs.bin";			
					m_map_data_filename		= m_filename + CONST_FILENAME_LEVEL_POSTFIX + level_n + "_Data.bin";
					
					level_data = m_data_mngr.get_layout_data( level_n );
					
					n_scr_X = level_data.get_width();
					n_scr_Y = level_data.get_height();

					if( ExpZXAsmRenderLevelPNG.Checked )
					{
						level_bmp = new Bitmap( n_scr_X * utils.CONST_SCREEN_WIDTH_PIXELS, n_scr_Y * utils.CONST_SCREEN_HEIGHT_PIXELS );
	
						level_gfx 					= Graphics.FromImage( level_bmp );
						level_gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
						level_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
					}
					
#if DEF_SCREEN_HEIGHT_7d5_TILES
					n_Y_tiles = n_scr_Y * ( ( utils.CONST_SCREEN_NUM_HEIGHT_TILES * ( ExpZXAsmTiles2x2.Checked ? 2:1 ) ) - ( ExpZXAsmTiles2x2.Checked ? 1:0 ) );
#else
					n_Y_tiles = n_scr_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES * ( ExpZXAsmTiles2x2.Checked ? 2:1 );
#endif
					// initialize a tile map of a game level
					{
#if DEF_SCREEN_HEIGHT_7d5_TILES
						map_data_size = n_scr_X * n_scr_Y * ( ( utils.CONST_SCREEN_TILES_CNT * ( ExpZXAsmTiles2x2.Checked ? 4:1 ) ) - ( ExpZXAsmTiles2x2.Checked ? ( utils.CONST_SCREEN_NUM_WIDTH_BLOCKS ):0 ) );
#else
						map_data_size = n_scr_X * n_scr_Y * utils.CONST_SCREEN_TILES_CNT * ( ExpZXAsmTiles2x2.Checked ? 4:1 );
#endif						
						
						map_tiles_arr = new ushort[ map_data_size ];
						
						Array.Clear( map_tiles_arr, 0, map_data_size );
					}
					
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
									n_screens += scr_tiles_data[ bank_ind ].scr_data.Count;
									
									if( scr_data.m_scr_ind < n_screens )
									{
										break;
									}
								}
								
								// convert screen index to local bank index
								scr_ind = scr_data.m_scr_ind - ( n_screens - scr_tiles_data[ bank_ind ].scr_data.Count );
								
								tiles = scr_tiles_data[ bank_ind ];
								
								for( int tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
								{
									tile_offs_x = ( tile_n % utils.CONST_SCREEN_NUM_WIDTH_TILES );
									tile_offs_y = ( tile_n / utils.CONST_SCREEN_NUM_WIDTH_TILES );
									
									tile_id = tiles.scr_data[ scr_ind ][ tile_n ];
									
									if( ExpZXAsmTiles2x2.Checked )
									{
										tile_offs_x <<= 1;
										tile_offs_y <<= 1;
										
										// make a list of unique 2x2 tiles in the map
										for( int block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
										{
											tile_data = (ushort)( tiles.get_tile_block( tile_id, block_n ) | ( ( bank_ind + 1 ) << 8 ) );
											
											if( unique_tiles_arr.Contains( tile_data ) == false )
											{
												unique_tiles_arr.Add( tile_data );
											}
#if DEF_SCREEN_HEIGHT_7d5_TILES
											if( tile_offs_y < 14 || ( tile_offs_y >= 14 && block_n < 2 ) )
											{
												map_tiles_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + ( ( block_n & 0x01 ) == 0x01 ? n_Y_tiles:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ) ] = tile_data;
											}
#else
											map_tiles_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + ( ( block_n & 0x01 ) == 0x01 ? n_Y_tiles:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ) ] = tile_data;
#endif											
										}
									}
									else
									{
										// make a list of unique 4x4 tiles in the map
										tile_data = (ushort)(( tile_id | ( ( bank_ind + 1 ) << 8 )) );
										
										if( unique_tiles_arr.Contains( tile_data ) == false )
										{
											unique_tiles_arr.Add( tile_data );
										}
										
										map_tiles_arr[ scr_n_X * ( n_Y_tiles * utils.CONST_SCREEN_NUM_WIDTH_TILES ) + ( scr_n_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = tile_data;
									}
								}
							}
						}
					}
					
					// check the number of unique tiles in the map
					if( unique_tiles_arr.Count > 128 )
					{
						sw.Close();
						throw new System.Exception( String.Format( "The level{0} has more than 128 tiles 2x2!", level_n ) );
					}
					
					tile_colors_arr = new byte[ unique_tiles_arr.Count << 2 ];
					Array.Clear( tile_colors_arr, 0, unique_tiles_arr.Count << 2 );
					
					tile_props_arr = new byte[ unique_tiles_arr.Count * ( ExpZXAsmTiles2x2.Checked ? 4:16 ) ];
					Array.Clear( tile_props_arr, 0, unique_tiles_arr.Count * ( ExpZXAsmTiles2x2.Checked ? 4:16 ) );

					// open file to write data of all unique tiles
					bw = new BinaryWriter( File.Open( m_path + m_tiles_data_filename, FileMode.Create ) );
					
					// convert tiles data into indices
					for( int tile_n = 0; tile_n < unique_tiles_arr.Count; tile_n++ )
					{
						tile_data = unique_tiles_arr[ tile_n ];
						
						for( int map_tile_n = 0; map_tile_n < map_data_size; map_tile_n++ )
						{
							if( map_tiles_arr[ map_tile_n ] == tile_data )
							{
								map_tiles_arr[ map_tile_n ] = (ushort)tile_n;
							}
						}
						
						// tiles data processing
						if( ExpZXAsmTiles2x2.Checked )
						{
							ink_color_index 	= 0;
							paper_color_index 	= 0;
							
							for( chr_n = 0; chr_n < 4; chr_n++ )
							{
								tiles 		= scr_tiles_data[ ( (tile_data&0xff00) >> 8 ) - 1 ];
								block_data 	= tiles.blocks[ ( ( tile_data&0xff ) << 2 ) + chr_n ];
								
								tiles.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( block_data ), tile_2x2_data_arr, chr_n << 6 );

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
					           	tile_props_arr[ ( tile_n << 2 ) + chr_n ] = (byte)tiles_data.get_block_flags_obj_id( block_data );
					           	
					           	// draw ZX tile and get ink&paper colors
								Array.Clear( ink_clr_weights, 0, ink_clr_weights.Length );
								Array.Clear( paper_clr_weights, 0, paper_clr_weights.Length );								
#if DEF_NES
								palette = tiles.subpalettes[ tiles_data.get_block_flags_palette( block_data ) ];
#elif DEF_SMS
								plt16 = tiles.palettes_arr[ 0 ];
#elif DEF_PALETTE16_PER_CHR
								plt16 = tiles.palettes_arr[ tiles_data.get_block_flags_palette( block_data ) ];
#endif
								no_ink = true;
								
								for( pix_n = 0; pix_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; pix_n++ )
								{
									pixel_ind 	= ( byte )( ( chr_n << 6 ) + pix_n );
									pixel 		= tile_2x2_data_arr[ pixel_ind ];
#if DEF_NES
									bright_val = get_brightness( palette_group.Instance.main_palette[ palette[ pixel ] ] ) / 256.0;
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
									bright_val = get_brightness( palette_group.Instance.main_palette[ plt16.subpalettes[ pixel >> 2 ][ pixel & 0x03 ] ] ) / 256.0;
#endif
									if( ink_factor <= bright_val )
									{
										// paper
										if( ExpZXAsmGFXDithering.Checked )
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
								}
								
								max_weight_ind = utils.get_byte_arr_max_val_ind( paper_clr_weights );
#if DEF_NES
								paper_color_index |= palette[ max_weight_ind ] << ( chr_n << 3 );
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
								paper_color_index |= plt16.subpalettes[ max_weight_ind >> 2 ][ max_weight_ind & 0x03 ] << ( chr_n << 3 );
#endif								
								// reset paper color to avoid equal colors on both ink and paper
								ink_clr_weights[ max_weight_ind ] = 0;
								
								if( !no_ink )
								{
									max_weight_ind = utils.get_byte_arr_max_val_ind( ink_clr_weights );
#if DEF_NES
									ink_color_index |= palette[ max_weight_ind ] << ( chr_n << 3 );
#elif DEF_SMS || DEF_PALETTE16_PER_CHR
									ink_color_index |= plt16.subpalettes[ max_weight_ind >> 2 ][ max_weight_ind & 0x03 ] << ( chr_n << 3 );
#endif								
								}
#if DEF_NES									
								else
								{
									ink_color_index |= 13 << ( chr_n << 3 );
								}
#endif								
							}
							
							// convert a tile into a speccy format and write it to a file
							save_tiles_gfx( bw, tile_2x2_data_arr );
							
							// draw a tile into bitmap
							tile_mem_handle = GCHandle.Alloc( img_buff, GCHandleType.Pinned );

							if( ExpZXAsmTypeColor.Checked )
							{
								for( chr_n = 0; chr_n < 4; chr_n++ )
								{
									zx_paper_color_index 	= zx_clrs_conv_tbl[ ( paper_color_index >> ( chr_n << 3 ) ) & 0xff ];
									zx_ink_color_index 		= zx_clrs_conv_tbl[ ( ink_color_index >> ( chr_n << 3 ) ) & 0xff ];
									
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
									
									tile_colors_arr[ ( tile_n << 2 ) + chr_n ] |= (byte)( ( zx_bright_flag ? 64:0 ) | ( zx_paper_color_index << 3 ) | zx_ink_color_index );
									
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
										
										img_buff[ ( (chr_n&0x01) == 0x01 ? 8:0 ) + ( (chr_n&0x02) == 0x02 ? 128:0 ) + ( ( pix_n >> 3 ) << 4 ) + ( pix_n % 8 ) ] = pix_val;
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
									
										img_buff[ ( (chr_n&0x01) == 0x01 ? 8:0 ) + ( (chr_n&0x02) == 0x02 ? 128:0 ) + ( ( pix_n >> 3 ) << 4 ) + ( pix_n % 8 ) ] = 0xFF << 24 | pix_val << 16 | pix_val << 8 | pix_val;
									}
								}
							}
							
							tile_bmp = new Bitmap( 16, 16, 16<<2, PixelFormat.Format32bppArgb, tile_mem_handle.AddrOfPinnedObject() );
							
							if( ExpZXAsmRenderLevelPNG.Checked )
							{
								for( int map_tile_n = 0; map_tile_n < map_data_size; map_tile_n++ )
								{
									if( map_tiles_arr[ map_tile_n ] == tile_n )
									{
										level_gfx.DrawImage( tile_bmp, ( map_tile_n / ( n_scr_Y * scr_height_blocks ) ) << 4, ( map_tile_n % ( n_scr_Y * scr_height_blocks ) ) << 4 );
									}
								}
							}

							tile_mem_handle.Free();

							if( ExpZXAsmRenderTilesPNG.Checked )
							{
								tile_bmp.Save( m_extra_path_filename + CONST_FILENAME_LEVEL_POSTFIX + level_n + "_Tile" + tile_n + ".png", ImageFormat.Png );
							}
							tile_bmp.Dispose();
						}
						else
						{
							// tile_4x4_data_arr...
						}
					}
					
					// close tiles gfx data file
					level_data_size = bw.BaseStream.Length;
					bw.Close();
					
					// write tiles properties
					bw = new BinaryWriter( File.Open( m_path + m_tile_props_filename, FileMode.Create ) );
					bw.Write( tile_props_arr );
					level_data_size += bw.BaseStream.Length;
					bw.Close();

					// write tiles colors
					if( ExpZXAsmTypeColor.Checked )
					{
						bw = new BinaryWriter( File.Open( m_path + m_tile_colrs_filename, FileMode.Create ) );
						bw.Write( tile_colors_arr );
						level_data_size += bw.BaseStream.Length;
						bw.Close();
					}
					
					// write level map
					bw = new BinaryWriter( File.Open( m_path + m_map_data_filename, FileMode.Create ) );
					for( int map_tile_n = 0; map_tile_n < map_data_size; map_tile_n++ )
					{
						bw.Write( (byte)( map_tiles_arr[ map_tile_n ] << 1 ) );	// multiply the index by 2 to speed up lookup process of a tiles gfx in 16-bit address table
					}
					level_data_size += bw.BaseStream.Length;
					bw.Close();
					
					// write the data to assembly file
					{
						sw.WriteLine( "; *** " + CONST_FILENAME_LEVEL_PREFIX + level_n + " data (incbins: " + level_data_size + " bytes) ***\n" );
						
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_wscr\tequ " + n_scr_X + "\t\t; number of map screens in width" );
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_hscr\tequ " + n_scr_Y + "\t\t; number of map screens in height" );
						
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_wchr\tequ " + n_scr_X * ( utils.CONST_SCREEN_NUM_WIDTH_TILES << 2 ) + "\t\t; number of map CHRs in width" );
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_hchr\tequ " + n_scr_Y * ( scr_height_blocks << 1 ) + "\t\t; number of map CHRs in height" );

						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_wtls\tequ " + n_scr_X * scr_width_blocks + "\t\t; number of map tiles in width" );
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_htls\tequ " + n_scr_Y * scr_height_blocks + "\t\t; number of map tiles in height" );
						
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_u_tiles\tequ " + unique_tiles_arr.Count + "\t\t; number of unique tiles" );
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_t_tiles\tequ " + map_tiles_arr.Length + "\t\t; total number of tiles in whole map" );
						
						sw.WriteLine( "" );
						
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_map" + "\tincbin \"" + m_map_data_filename   + "\"\t;(" + map_tiles_arr.Length + ") array of map tile indices ( multiplied by 2 ), top down left to right" );
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_tl"  + "\t\tincbin \"" + m_tiles_data_filename + "\"\t;(" + ( unique_tiles_arr.Count * 32 ) + ") array of tile images 2x2 (32 bytes per tile)" );
						sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_tlp" + "\tincbin \"" + m_tile_props_filename + "\"\t;(" + tile_props_arr.Length + ") tile properties array (byte per tile)" );
						
						if( ExpZXAsmTypeColor.Checked )
						{
							sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_tlc" + "\tincbin \"" + m_tile_colrs_filename + "\"\t;(" + tile_colors_arr.Length + ") tile colors array (byte per tile)" );
						}
						
						sw.WriteLine( "\n" );
					}
					
					if( ExpZXAsmRenderLevelPNG.Checked )
					{
						level_bmp.Save( m_extra_path_filename + CONST_FILENAME_LEVEL_POSTFIX + level_n + ".png", ImageFormat.Png );
						
						level_bmp.Dispose();
						level_gfx.Dispose();
					}
					
					tile_colors_arr = null;
					tile_props_arr 	= null;
					map_tiles_arr 	= null;
					
					unique_tiles_arr.Clear();				

					// save layout and screens data
					if( ExpZXAsmExportMarks.Checked || ExpZXAsmExportEntities.Checked )
					{
						int start_scr_ind = level_data.get_start_screen_ind();
						
						if( start_scr_ind < 0 )
						{
							MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A first valid screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
						}
	
						// matrix layout by default
						{
							sw.WriteLine( CONST_FILENAME_LEVEL_PREFIX + level_n + "_StartScr\t = " + ( start_scr_ind < 0 ? 0:start_scr_ind ) + "\n" );
							level_data.export_asm( sw, CONST_FILENAME_LEVEL_PREFIX + level_n, "\tDEFINE", "db", "dw", "#", true, ExpZXAsmExportMarks.Checked, ExpZXAsmExportEntities.Checked, ExpZXAsmEntScreenCoords.Checked );
						}
					}
				}

				unique_tiles_arr = null;
				
				sw.Close();
			}
			catch( System.Exception _err )
			{
				MainForm.message_box( _err.Message, "ZX Asm Data Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error ); 
			}
		}
		
		double get_brightness( int _XRGB )
		{
			double r = ( double )( ( _XRGB >> 16 ) & 0xff );
			double g = ( double )( ( _XRGB >> 8 ) & 0xff );
			double b = ( double )( _XRGB & 0xff );
			
			return Math.Sqrt( ( 0.299 * r*r ) + ( 0.587 * g*g ) + ( 0.114 * b*b ) );
		}
		
		void calc_colors_conversion_table()
		{
			double r = 0.0;
			double g = 0.0;
			double b = 0.0;

			double zx_r = 	0.0;
			double zx_g = 	0.0;
			double zx_b = 	0.0;
			
			int color = 0;
			int zx_color  = 0;
			
			double 	fi			= 0.0;
			double 	fi_min 		= 0.0;
			int 	best_color 	= -1;			

			for( int color_n = 0; color_n < 64; color_n++ )
			{
				color = palette_group.Instance.main_palette[ color_n ];
				
				r = ( double )( ( color >> 16 ) & 0xff );
				g = ( double )( ( color >> 8 ) & 0xff );
				b = ( double )( color & 0xff );
				
				fi_min 		= 1000000.0;
				best_color 	= -1;
				
				for( int zx_color_n = 0; zx_color_n < 16; zx_color_n++ )
				{
					zx_color = zx_palette[ zx_color_n ];
					
					zx_r = ( double )( ( zx_color >> 16 ) & 0xff );
					zx_g = ( double )( ( zx_color >> 8 ) & 0xff );
					zx_b = ( double )( zx_color & 0xff );

					switch( ExpZXAsmColorConversionModes.SelectedIndex )
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
				
				zx_clrs_conv_tbl[ color_n ] = best_color;
				
				//palette_group.Instance.main_palette[ color_n ] = zx_palette[ best_color ];//!!!
			}
		}
		
		private void save_tiles_gfx( BinaryWriter _bw, byte[] _tile_2x2_data_arr )
		{
			save_tile_2x8( _bw, _tile_2x2_data_arr, 0 );
			save_tile_2x8( _bw, _tile_2x2_data_arr, 128 );
		}
		
		private void save_tile_2x8( BinaryWriter _bw, byte[] _tile_2x2_data_arr, int _offset )
		{
			ushort word;
			
			for( int i = 0; i < 8; i++ )
			{
				word = 0;
				
				for( int j = 0; j < 8; j++ )
				{
					word |= (ushort)( ( ( _tile_2x2_data_arr[ _offset + ( i << 3 ) + 7 - j ] & 0x01 ) << j ) | ( _tile_2x2_data_arr[ _offset + 64 + ( i << 3 ) + 7 - j ] & 0x01 ) << ( 8 + j ) );
				}
				
				_bw.Write( word );
			}
		}
		
		private void save_export_options( StreamWriter _sw )
		{
			// options
			{
				_sw.WriteLine( "; export options:" );

				_sw.WriteLine( ";\t- tiles " + ( ExpZXAsmTiles4x4.Checked ? "4x4":"2x2" ) + "/(columns)" );
				
				_sw.WriteLine( ";\t- properties per block" );
				
				_sw.WriteLine( ";\t- mode: multidirectional scrolling" );
				
				_sw.WriteLine( ";\t- layout: " + "matrix" + ( ExpZXAsmExportMarks.Checked ? " (marks)":" (no marks)" ) );
				
				_sw.WriteLine( ";\t- " + ( ExpZXAsmExportEntities.Checked ? "entities " + ( ExpZXAsmEntScreenCoords.Checked ? "(screen coordinates)":"(map coordinates)" ):"no entities" ) );
				_sw.WriteLine( "\n" );
			}
			
			_sw.WriteLine( "MAP_DATA_MAGIC = " + utils.hex( "$", ( ExpZXAsmExportEntities.Checked ? 1:0 ) |
			                                              		 ( ExpZXAsmExportEntities.Checked ? ( ExpZXAsmEntScreenCoords.Checked ? 2:4 ):0 ) |
			                                              		 ( ExpZXAsmExportMarks.Checked ? 8:0 ) |
			                                              		 ( ExpZXAsmTypeColor.Checked ? 16:0 ) ) );
			_sw.WriteLine( "\n; data flags:" );
			_sw.WriteLine( "MAP_FLAG_ENTITIES                 = " + utils.hex( "$", 1 ) );
			_sw.WriteLine( "MAP_FLAG_ENTITY_SCREEN_COORDS     = " + utils.hex( "$", 2 ) );
			_sw.WriteLine( "MAP_FLAG_ENTITY_MAP_COORS         = " + utils.hex( "$", 4 ) );
			_sw.WriteLine( "MAP_FLAG_MARKS                    = " + utils.hex( "$", 8 ) );
			_sw.WriteLine( "MAP_FLAG_TYPE_COLORED             = " + utils.hex( "$", 16 ) );
		}
	}
}