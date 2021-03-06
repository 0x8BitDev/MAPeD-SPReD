﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 13.09.2018
 * Time: 17:59
 */
 
//#define DEF_DBG_PPU_READY_DATA_SAVE_IMG
 
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_pce_asm.
	/// </summary>
	/// 

	public partial class exporter_asm : Form
	{
#if DEF_PCE
		private readonly data_sets_manager m_data_mngr = null;
		
		private string	m_filename			= null;
		private string	m_path				= null;
		private string 	m_path_filename_ext	= null;
		private string 	m_path_filename		= null;
		
		private string	m_level_prefix		= null;
		
		private const string	CONST_FILENAME_LEVEL_PREFIX		= "Lev";
		private const string	CONST_BIN_EXT					= ".bin";

		private int[] m_CHR_offset	= { 64, 128, 256, 128, 256, 512 };
		private int[] m_BAT_index	= { 0, 1, 3, 4, 5, 7 };
		
		private StreamWriter	m_C_writer	= null;
		
		private struct exp_scr_data
		{
			public int 			m_scr_ind;
			
			public tiles_data 	m_tiles;
			
			public screen_data	m_scr_tiles;
			public screen_data	m_scr_blocks;
			
			public int			m_tiles_offset;
			public int			m_blocks_offset;			
			
			public int			m_VDC_scr_offset;
			
			public static int	_tiles_offset;
			public static int	_blocks_offset;
			
			public exp_scr_data( int _scr_ind, tiles_data _tiles )
			{
				m_scr_ind		= _scr_ind;
				
				m_tiles 		= _tiles;
				
				m_scr_tiles		= new screen_data( data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
				m_scr_blocks	= new screen_data( data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
				
				m_tiles_offset	= _tiles_offset;
				m_blocks_offset	= _blocks_offset;
				
				_tiles_offset 	+= m_scr_tiles.length;
				_blocks_offset 	+= m_scr_blocks.length;
				
				m_VDC_scr_offset = 0;
			}
			
			public void destroy()
			{
				m_tiles 		= null;
				
				m_scr_tiles 	= null;
				m_scr_blocks 	= null;
			}
		};	
		
		public exporter_asm( data_sets_manager _data_mngr )
		{
			m_data_mngr = _data_mngr;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = false;
			
			ComboBoxBAT.SelectedIndex = 0;
			
			update_desc();
		}

		void CheckBoxExportEntitiesChanged_Event(object sender, EventArgs e)
		{
			bool enabled = ( sender as CheckBox ).Checked;
			
			groupBoxEntityCoordinates.Enabled = enabled;
			
			CheckBoxExportMarks.Enabled = ( enabled == false && RBtnModeMultidirScroll.Checked ) ? false:true;
			CheckBoxExportMarks.Checked = CheckBoxExportMarks.Enabled ? CheckBoxExportMarks.Checked:false;
			
			update_desc();
		}

		void RBtnModeMultidirScrollChanged_Event(object sender, EventArgs e)
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = false;
			RBtnLayoutMatrix.Checked 	= true;
			
			CheckBoxExportMarks.Enabled = ( CheckBoxExportEntities.Checked == false && RBtnModeMultidirScroll.Checked ) ? false:true;
			CheckBoxExportMarks.Checked = CheckBoxExportMarks.Enabled ? CheckBoxExportMarks.Checked:false;
			
			CheckBoxRLE.Enabled = true;
			
			update_desc();
		}
		
		void RBtnModeScreenToScreenChanged_Event(object sender, EventArgs e)
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			RBtnLayoutAdjacentScreens.Checked	= true;
			
			CheckBoxExportMarks.Enabled = true;
			
			CheckBoxRLE.Checked = CheckBoxRLE.Enabled = false;
			
			update_desc();
		}

		void RBtnModeStaticScreensChanged_Event(object sender, EventArgs e)
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			
			CheckBoxExportMarks.Enabled = true;
			
			CheckBoxRLE.Checked = CheckBoxRLE.Enabled = false;
			
			update_desc();
		}

		void ParamChanged_Event(object sender, EventArgs e)
		{
			update_desc();
		}
		
		void update_desc()
		{
			if( RBtnTiles2x2.Checked )
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_TILES_2X2;
			}
			else
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_TILES_4X4;
			}
			
			if( CheckBoxRLE.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_RLE_COMPRESSION;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER;
			
			if( RBtnTilesDirColumns.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PCE_DATA_ORDER_COLS;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER_ROWS;
			}

			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE;
			
			if( RBtnModeMultidirScroll.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE_MULTIDIR;
			}
			else
			if( RBtnModeBidirScroll.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE_BIDIR;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PCE_MODE_STAT_SCR;
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
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PROP_IN_FRONT_OF_SPRITES;
			
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
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NO_ENTITIES;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_PCE_CHR_OFFSET;
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_WARNING;
		}

		void event_cancel(object sender, System.EventArgs e)
		{
			this.Close();
		}
		
		private string get_exp_prefix()
		{
			return utils.get_exp_prefix( CheckBoxGenerateHFile.Checked );
		}
		
		public void ShowDialog( string _full_path )
		{
			m_path_filename_ext = _full_path;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 )
			{
				RBtnTiles2x2.Checked = true;
				RBtnTiles2x2.Enabled = RBtnTiles4x4.Enabled = false;
			}
			else
			{
				RBtnTiles2x2.Enabled = RBtnTiles4x4.Enabled = true;
			}
			
			ShowDialog();
		}
		
		void event_ok(object sender, System.EventArgs e)
		{
			this.Close();

			m_filename		= get_exp_prefix() + Path.GetFileNameWithoutExtension( m_path_filename_ext );
			m_path			= Path.GetDirectoryName( m_path_filename_ext ) + Path.DirectorySeparatorChar;
			m_path_filename	= m_path + m_filename;

			m_level_prefix 	= get_exp_prefix() + CONST_FILENAME_LEVEL_PREFIX;
			
			StreamWriter sw = null;
			
			try
			{
				if( CheckBoxGenerateHFile.Checked )
				{
					m_C_writer = new StreamWriter( m_path + Path.GetFileNameWithoutExtension( m_path_filename_ext ) + ".h" );
					{
						utils.write_title( m_C_writer, true );
						
						m_C_writer.WriteLine( "\n//#incasm( \"" + Path.GetFileName( m_path_filename_ext ) + "\" )\n\n" );
					}
				}
				
				sw = new StreamWriter( m_path_filename_ext );
				
				utils.write_title( sw );

				// options
				{
					sw.WriteLine( "; export options:" );

					sw.WriteLine( ";\t- tiles " + ( RBtnTiles4x4.Checked ? "4x4":"2x2" ) + ( CheckBoxRLE.Checked ? " (RLE)":"" ) + ( RBtnTilesDirColumns.Checked ? "/(columns)":"/(rows)" ) );
					
					sw.WriteLine( ";\t- properties per " + ( RBtnPropPerBlock.Checked ? "block":"CHR" ) );
					
					if( RBtnModeMultidirScroll.Checked )
					{
						sw.WriteLine( ";\t- mode: multidirectional scrolling" );
					}
					else
					if( RBtnModeBidirScroll.Checked )
					{
						sw.WriteLine( ";\t- mode: bidirectional scrolling" );
					}
					else
					if( RBtnModeStaticScreen.Checked )
					{
						sw.WriteLine( ";\t- mode: static screens" );
					}
					
					sw.WriteLine( ";\t- layout: " + ( RBtnLayoutAdjacentScreens.Checked ? "adjacent screens":RBtnLayoutAdjacentScreenIndices.Checked ? "adjacent screen indices":"matrix" ) + ( CheckBoxExportMarks.Checked ? " (marks)":" (no marks)" ) );
					
					sw.WriteLine( ";\t- " + ( CheckBoxExportEntities.Checked ? "entities " + ( RBtnEntityCoordScreen.Checked ? "(screen coordinates)":"(map coordinates)" ):"no entities" ) );
					
					if( m_C_writer != null )
					{
						sw.WriteLine( ";\t- generate C header file" );
					}
					
					sw.WriteLine( "\n" );
				}
				
				write_map_flags( CheckBoxGenerateHFile.Checked ? m_C_writer:sw );
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "\n// see .asm file for details" );
				}
				
				if( CheckBoxExportEntities.Checked )
				{
					m_data_mngr.export_entity_asm( sw, ".byte", "$" );
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
				MainForm.message_box( _err.Message, "Data Export Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error ); 
			}
			
			finally
			{
				if( sw != null )
				{
					sw.Dispose();
				}
				
				if( m_C_writer != null )
				{
					m_C_writer.Dispose();
					m_C_writer = null;
				}
			}
		}

		void write_map_flags( StreamWriter _sw )
		{
			string c_def 		= CheckBoxGenerateHFile.Checked ? "#define ":"";
			string c_comment	= CheckBoxGenerateHFile.Checked ? "//":";";
			string c_hex_pref	= CheckBoxGenerateHFile.Checked ? "0x":"$";
			string c_def_eq		= CheckBoxGenerateHFile.Checked ? "":"= ";
			
			_sw.WriteLine( c_def + "MAP_DATA_MAGIC " + c_def_eq + utils.hex( c_hex_pref, ( RBtnTiles2x2.Checked ? 1:2 ) |
			                                              		( CheckBoxRLE.Checked ? 4:0 ) |
			                                              		( RBtnTilesDirColumns.Checked ? 8:16 ) |
			                                              		( RBtnModeMultidirScroll.Checked ? 32:RBtnModeBidirScroll.Checked ? 64:128 ) | 
			                                              		( CheckBoxExportEntities.Checked ? 256:0 ) |
			                                              		( CheckBoxExportEntities.Checked ? ( RBtnEntityCoordScreen.Checked ? 512:1024 ):0 ) |
			                                              		( RBtnLayoutAdjacentScreens.Checked ? 2048:RBtnLayoutAdjacentScreenIndices.Checked ? 4096:8192 ) |
			                                              		( CheckBoxExportMarks.Checked ? 16384:0 ) |
			                                              		( RBtnPropPerBlock.Checked ? 32768:65536 ) ) );
			_sw.WriteLine( "\n" + c_comment + " data flags:" );
			_sw.WriteLine( c_def + "MAP_FLAG_TILES2X2                 " + c_def_eq + utils.hex( c_hex_pref, 1 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_TILES4X4                 " + c_def_eq + utils.hex( c_hex_pref, 2 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_RLE                      " + c_def_eq + utils.hex( c_hex_pref, 4 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_DIR_COLUMNS              " + c_def_eq + utils.hex( c_hex_pref, 8 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_DIR_ROWS                 " + c_def_eq + utils.hex( c_hex_pref, 16 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_MODE_MULTIDIR_SCROLL     " + c_def_eq + utils.hex( c_hex_pref, 32 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_MODE_BIDIR_SCROLL        " + c_def_eq + utils.hex( c_hex_pref, 64 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_MODE_STATIC_SCREENS      " + c_def_eq + utils.hex( c_hex_pref, 128 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_ENTITIES                 " + c_def_eq + utils.hex( c_hex_pref, 256 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_ENTITY_SCREEN_COORDS     " + c_def_eq + utils.hex( c_hex_pref, 512 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_ENTITY_MAP_COORS         " + c_def_eq + utils.hex( c_hex_pref, 1024 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_LAYOUT_ADJACENT_SCREENS  " + c_def_eq + utils.hex( c_hex_pref, 2048 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS " + c_def_eq + utils.hex( c_hex_pref, 4096 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_LAYOUT_MATRIX            " + c_def_eq + utils.hex( c_hex_pref, 8192 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_MARKS                    " + c_def_eq + utils.hex( c_hex_pref, 16384 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_PROP_ID_PER_BLOCK        " + c_def_eq + utils.hex( c_hex_pref, 32768 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_PROP_ID_PER_CHR          " + c_def_eq + utils.hex( c_hex_pref, 65536 ) );
			
			_sw.WriteLine( "\n" + c_def + "BAT_INDEX\t" + c_def_eq + m_BAT_index[ ComboBoxBAT.SelectedIndex ] );
			_sw.WriteLine( c_def + "CHRS_OFFSET\t" + c_def_eq + get_CHR_offset() + "\t" + c_comment + " first CHR index from the begining of VRAM" );

			_sw.WriteLine( "\n" + c_def + "ScrTilesWidth\t" + c_def_eq + get_tiles_cnt_width( 1 ) + "\t" + c_comment + " number of screen tiles (" + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + ") in width" );
			_sw.WriteLine( c_def + "ScrTilesHeight\t" + c_def_eq + get_tiles_cnt_height( 1 ) + "\t" + c_comment + " number of screen tiles (" + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + ") in height" );

			_sw.WriteLine( "\n" + c_def + "ScrPixelsWidth\t" + c_def_eq + get_tiles_cnt_width( 1 ) * ( RBtnTiles2x2.Checked ? 16:32 ) + "\t" + c_comment + " screen width in pixels" );
			_sw.WriteLine( c_def + "ScrPixelsHeight\t" + c_def_eq + get_tiles_cnt_height( 1 ) * ( RBtnTiles2x2.Checked ? 16:32 ) + "\t" + c_comment + " screen height in pixels" );
		}
		
		private void save_single_screen_mode( StreamWriter _sw )
		{
			BinaryWriter 	bw 			= null;
			BinaryWriter	bw_props	= null;
			
			layout_data level_data = null;

			List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
			
			int n_banks				= scr_tiles_data.Count;
			int n_levels 			= m_data_mngr.layouts_data_cnt;
			int n_scr_X 			= 0;
			int n_scr_Y				= 0;
			int n_screens 			= 0;
			int scr_ind				= 0;
			int scr_ind_opt			= 0;
			int bank_ind			= 0;
			int scr_key				= 0;
			int tile_n				= 0;
			int tiles_cnt			= 0;
			int tile_offs_x			= 0;
			int tile_offs_y			= 0;
			int block_n				= 0;
			int bank_n				= 0;
			int blocks_props_size	= 0;
			int data_offset 		= 0;
			int common_scr_ind		= 0;
			int max_scr_tile		= 0;
			int max_tile_ind		= 0;
			int max_scr_block		= 0;
			int max_block_ind		= 0;
			int start_scr_ind		= 0;
			int ent_n				= 0;
			
			int scr_width_blocks_mul2 	= 0;
			int scr_height_blocks_mul2 	= 0;
			int scr_height_blocks_mul4 	= 0;
			
			int scr_width_blocks 	= utils.CONST_SCREEN_NUM_WIDTH_BLOCKS;
			int scr_height_blocks 	= utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS;

			uint block_data		= 0;
			
			ushort tile_id			= 0;
			ushort block_id			= 0;
			
			screen_data tile_inds		= null;
			ushort[] tile_attrs_arr		= new ushort[ 16 ];
			ushort[] block_attrs_arr	= new ushort[ 8 ];
			
			long data_size 			= 0;
			long exp_data_size		= 0;

			bool valid_bank;
			bool enable_comments;
			
			string label 			= null;
			string level_prefix_str	= null;
			string label_props		= null;
			string scr_arr			= null;
			string data_offset_str	= null;
			
			exp_scr_data		exp_scr;
			layout_screen_data	scr_data;
			tiles_data 			tiles = null;
			
			ConcurrentDictionary< int, exp_scr_data >	screens	= null;	// ConcurrentDictionary for changing values in foreach
			
			List< tiles_data > 	banks 			= new List< tiles_data >( 10 );			
			List< int >			max_tile_inds	= new List< int >( 10 );
			List< int >			max_block_inds	= new List< int >( 10 );
			
			int[] banks_size_arr	= new int[ m_data_mngr.tiles_data_cnt + 1 ];
			banks_size_arr[ 0 ] 	= 0;

			scr_width_blocks_mul2	= scr_width_blocks << 1;
			scr_height_blocks_mul2	= scr_height_blocks << 1;
			scr_height_blocks_mul4	= scr_height_blocks << 2;

			ushort[] attrs_chr = new ushort[ ( scr_width_blocks * scr_height_blocks ) << 2 ];
						
			exp_scr_data._tiles_offset  = 0;
			exp_scr_data._blocks_offset = 0;

			screens = new ConcurrentDictionary< int, exp_scr_data >( 1, 100 );

			scr_ind = 0;		// global screen index
			scr_ind_opt = 0;	// optimized screen index
			
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
						if( scr_ind_opt > utils.CONST_SCREEN_MAX_CNT - 1 )
						{
							throw new Exception( "The screen index is out of range!\nThe maximum number of screens allowed to export: " + utils.CONST_SCREEN_MAX_CNT );
						}
						
						valid_bank = true;
						
						exp_scr = new exp_scr_data( scr_ind_opt++, tiles );
						
						screens[ ( bank_n << 8 ) | scr_n ] = exp_scr;
						
						if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ) 
						{
							for( tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
							{
								tile_offs_x = ( tile_n % utils.CONST_SCREEN_NUM_WIDTH_TILES );
								tile_offs_y = ( tile_n / utils.CONST_SCREEN_NUM_WIDTH_TILES );
								
								tile_id = exp_scr.m_tiles.get_screen_tile( scr_n, tile_n );
		
								exp_scr.m_scr_tiles.set_tile( tile_offs_x * utils.CONST_SCREEN_NUM_HEIGHT_TILES + tile_offs_y, tile_id );
								
								if( RBtnTiles2x2.Checked )
								{
									tile_offs_x <<= 1;
									tile_offs_y <<= 1;
									
									// fill the array of all tiles 2x2 in a level
									for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
									{
										block_id = tiles.get_tile_block( tile_id, block_n );
										{
											exp_scr.m_scr_blocks.set_tile( ( tile_offs_x * scr_height_blocks ) + ( ( block_n & 0x01 ) == 0x01 ? scr_height_blocks:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ), block_id );
										}
									}
								}
							}
						}
						else
						{
							for( block_n = 0; block_n < utils.CONST_SCREEN_BLOCKS_CNT; block_n++ )
							{
								tile_offs_x = ( block_n % utils.CONST_SCREEN_NUM_WIDTH_BLOCKS );
								tile_offs_y = ( block_n / utils.CONST_SCREEN_NUM_WIDTH_BLOCKS );
								
								exp_scr.m_scr_blocks.set_tile( ( tile_offs_x * scr_height_blocks ) + tile_offs_y, tiles.get_screen_tile( scr_n, block_n ) );
							}
						}
						
						max_scr_tile = exp_scr.m_scr_tiles.max_val();
						
						if( max_tile_ind < max_scr_tile )
						{
							max_tile_ind = max_scr_tile;
						}
						
						if( RBtnTiles2x2.Checked )
						{
							max_scr_block = exp_scr.m_scr_blocks.max_val();
							
							if( max_block_ind < max_scr_block )
							{
								max_block_ind = max_scr_block;
							}
						}
					}
					
					++scr_ind;
				}
				
				if( valid_bank == true )
				{
					banks.Add( tiles );
					
					max_tile_inds.Add( 1 + max_tile_ind );
					
					if( RBtnTiles2x2.Checked )
					{
						max_block_inds.Add( ( 1 + max_block_ind ) << 2 );
					}
				}
			}

			// global data writing
			{
				_sw.WriteLine( "\n; *** GLOBAL DATA ***\n" );
				
				string chr_arr	= null;
				string chr_size	= null;
				
				// CHR
				for( bank_n = 0; bank_n < banks.Count; bank_n++ )
				{
					tiles = banks[ bank_n ];
					
					// write CHR bank data
					label = get_exp_prefix() + "chr" + bank_n;
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					{
						banks_size_arr[ bank_n + 1 ] += banks_size_arr[ bank_n ] + ( int )( data_size = tiles.export_CHR( bw ) );
					}
					bw.Close();
					
					_sw.WriteLine( ";" + label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ")" );
					
					chr_arr	 += "\t.word " + label + "\n";
					chr_size += "\t.word " + data_size + "\t\t;(" + label + ")\n";
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( label ) + ";" );
					}
					
					exp_data_size += data_size;
				}
				
				_sw.WriteLine( "\n" + m_filename + "_CHRs:\n" + chr_arr );
				_sw.WriteLine( m_filename + "_CHRs_size:\n" + chr_size );
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( m_filename ) + "_CHRs;" );
					m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( m_filename ) + "_CHRs_size;" );
				}
				
				// static screens ( VDC-READY DATA ):
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// [SKIP] attrs map arr - attributes array for each screen
				// [SKIP] blocks arr	- blocks data array
				// map arr				- GFX array for each screen (RLE) (VDC-ready) [CHRs]
				// plts arr 			- bank palettes
				// props arr 			- bank props
				//
				// 2x2:
				// ~~~~
				//   map arr (props)	- props indices array for each screen
				//
				// 4x4:
				// ~~~~
				//   [SKIP] attrs		- array of attributes
				//   map arr (tiles)	- tiles 4x4 indices array for each screen
				//   tiles arr (props)	- tiles array with props(blocks) indices 
				
				// blocks&props
				{
					label_props = "_Props";
					bw_props = new BinaryWriter( File.Open( m_path_filename + label_props + CONST_BIN_EXT, FileMode.Create ) );
				
					data_offset = 0;
					data_offset_str = "";
					
					for( bank_n = 0; bank_n < banks.Count; bank_n++ )
					{
						tiles = banks[ bank_n ];

						if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
						{
							blocks_props_size = ( 1 + utils.get_uint_arr_max_val( tiles.tiles, max_tile_inds[ bank_n ] ) ) << 2;
						}
						else
						{
							blocks_props_size = ( 1 + tiles.get_first_free_block_id() ) << 2;
						}
						
						for( block_n = 0; block_n < blocks_props_size; block_n++ )
						{
							block_data = tiles.blocks[ block_n ];
							
							if( RBtnPropPerBlock.Checked && ( block_n % 4 ) != 0 )
							{
								continue;
							}
							
							bw_props.Write( ( byte )tiles_data.get_block_flags_obj_id( block_data ) );
						}
						
						data_offset_str += "\t.word " + data_offset + "\t; (chr" + bank_n + ")\n";

						data_offset += blocks_props_size;
					}

					data_size = bw_props.BaseStream.Length;
					bw_props.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( m_filename + label_props + ":\t.incbin \"" + m_filename + label_props + CONST_BIN_EXT + "\"\t; (" + data_size + ") block properties array of all exported data banks ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )" + ( RBtnPropPerBlock.Checked ? ", data offset = props offset / 4":"" ) + "\n" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + label_props + ";" );
					}
					
					if( RBtnModeBidirScroll.Checked )
					{
						_sw.WriteLine( m_filename + "_BlocksPropsOffs:" );
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + "_BlocksPropsOffs;" );
						}
					}
					else
					{
						_sw.WriteLine( m_filename + "_PropsOffs:" );
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + "_PropsOffs;" );
						}
					}
					
					_sw.WriteLine( data_offset_str );
				}
				
				// tiles
				{
					if( RBtnModeBidirScroll.Checked )
					{
						// write tiles data
						label = "_Tiles";
						
						if( RBtnTiles4x4.Checked )
						{
							bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
						}
						
						data_offset = 0;
						data_offset_str = "";
						
						// tiles
						for( bank_n = 0; bank_n < banks.Count; bank_n++ )
						{
							max_tile_ind = max_tile_inds[ bank_n ];	// one based index

							if( RBtnTiles4x4.Checked )
							{
								tiles = banks[ bank_n ];
								
								for( int i = 0; i < max_tile_ind; i++ )
								{
									bw.Write( rearrange_tile( tiles.tiles[ i ] ) );
								}
							}
							
							data_offset_str += "\t.word " + data_offset + "\t\t; (chr" + bank_n + ")\n";
							
							data_offset += max_tile_inds[ bank_n ] << 2;
						}
						
						if( RBtnTiles4x4.Checked )
						{
							data_size = bw.BaseStream.Length;
							bw.Close();
						
							exp_data_size += data_size;
							
							_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") 4x4 tiles array of all exported data banks ( 4 bytes per tile )\n" );
							
							if( m_C_writer != null )
							{
								m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + label + ";" );
							}
						}
						
						_sw.WriteLine( m_filename + "_TilesOffs:" );
	
						_sw.WriteLine( data_offset_str );
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + "_TilesOffs;" );
						}
					}
				}
				
				// save VDC-ready data for STATIC SCREENS mode
				if( RBtnModeStaticScreen.Checked )
				{
					label = "_VDCScr";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );

					data_offset = 0;
					
					foreach( var key in screens.Keys )
					{
						exp_scr = screens[ key ];
						
						tile_inds	= exp_scr.m_scr_tiles;
						tiles_cnt 	= tile_inds.length;
						tiles 		= exp_scr.m_tiles;

#if DEF_DBG_PPU_READY_DATA_SAVE_IMG
						Bitmap 	tile_bmp	= null;
						Bitmap 	scr_bmp 	= new Bitmap( utils.CONST_SCREEN_WIDTH_PIXELS, utils.CONST_SCREEN_HEIGHT_PIXELS );
	
						Graphics scr_gfx 			= Graphics.FromImage( scr_bmp );
						scr_gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
						scr_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
#endif
						// save CHR ids
						fill_screen_attrs_per_CHR( 	attrs_chr,
						                          	tiles, 
						                          	tile_inds, 
						                          	true,//force_swapping 
						                          	scr_width_blocks_mul2, 
						                          	scr_height_blocks_mul2, 
						                          	scr_height_blocks_mul4,
													exp_scr.m_scr_blocks );
#if DEF_DBG_PPU_READY_DATA_SAVE_IMG
						for( int i = 0; i < attrs_chr.Length; i++ )
						{
							int plt_ind	= 0;
							tiles.from_CHR_bank_to_spr8x8( CHRs_arr[ i ], utils.tmp_spr8x8_buff );
							
							tile_bmp = utils.create_bitmap( utils.tmp_spr8x8_buff, 8, 8, 0, false, plt_ind, palette_group.Instance.get_palettes_arr() );
							
							scr_gfx.DrawImage( tile_bmp, ( i % 32 ) << 3, ( i / 32 ) << 3 );
						}
						scr_bmp.Save( m_path_filename + "_Scr" + exp_scr.m_scr_ind + ".png" );
						scr_bmp.Dispose();
						tile_bmp.Dispose();
#endif
						exp_scr.m_VDC_scr_offset = data_offset;
						screens[ key ] = exp_scr;
						
						if( compress_and_save_ushort( bw, attrs_chr, ref data_offset ) == false )
						{
							_sw.Close();
							bw.Close();
							throw new System.Exception( "Can't compress an empty data!" );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") VDC-ready data array for each screen (" + ( utils.CONST_SCREEN_TILES_CNT << 5 ) + " bytes per screen)" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( m_filename ) + label + ";" );
					}
					
					if( !CheckBoxRLE.Checked )
					{
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "int ScrGfxDataSize = " + ( utils.CONST_SCREEN_TILES_CNT << 5 ) );
						}
						else
						{
							_sw.WriteLine( "\nScrGfxDataSize = " + ( utils.CONST_SCREEN_TILES_CNT << 5 ) + "\n" );
						}
					}
				}
				
				// attributes array of 4x4 tiles ONLY and for the BIDIRECTIONAL SCROLLING mode !!!
				if( RBtnModeBidirScroll.Checked )
				{
					label = "_Attrs";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( bank_n = 0; bank_n < banks.Count; bank_n++ )
					{
						tiles = banks[ bank_n ];
						
						if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
						{
							blocks_props_size = ( 1 + utils.get_uint_arr_max_val( tiles.tiles, max_tile_inds[ bank_n ] ) ) << 2;
						}
						else
						{
							blocks_props_size = ( 1 + tiles.get_first_free_block_id() ) << 2;
						}
							
						for( block_n = 0; block_n < blocks_props_size; block_n++ )
						{
							block_data = tiles.blocks[ block_n ];
							
							bw.Write( get_screen_attribute( block_data ) );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") 2x2 tiles attributes array of all exported data banks ( 2 bytes per attribute ), data offset = tiles offset * 4" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + label + ";" );
					}
				}
				
				// map
				if( RBtnModeBidirScroll.Checked )
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
					
					exp_data_size += data_size;
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") " + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + " tiles array for each screen ( " + ( RBtnTiles2x2.Checked ? ( scr_width_blocks * scr_height_blocks ):utils.CONST_SCREEN_TILES_CNT ) + " bytes per screen \\ 1 byte per tile )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( m_filename ) + label + ";" );
					}
				}
				
				// palettes
				{
					label = "_Plts";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( bank_n = 0; bank_n < banks.Count; bank_n++ )
					{
						tiles = banks[ bank_n ];
						
						for( int i = 0; i < tiles.palettes_arr.Count; i++ )
						{
							utils.write_int_as_ushort_arr( bw, tiles.palettes_arr[ i ].m_palette0 );
							utils.write_int_as_ushort_arr( bw, tiles.palettes_arr[ i ].m_palette1 );
							utils.write_int_as_ushort_arr( bw, tiles.palettes_arr[ i ].m_palette2 );
							utils.write_int_as_ushort_arr( bw, tiles.palettes_arr[ i ].m_palette3 );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") palettes array of all exported data banks ( data offset = chr_id * " + ( ( utils.CONST_PALETTE16_ARR_LEN << 4 ) << 1 ) + " )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( m_filename ) + label + ";" );
					}
				}
			}

			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				enable_comments = true;
				
				_sw.WriteLine( "\n; *** " + CONST_FILENAME_LEVEL_PREFIX + level_n + " ***\n" );
				
				level_data = m_data_mngr.get_layout_data( level_n );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				if( RBtnLayoutAdjacentScreenIndices.Checked )
				{
					scr_arr = m_level_prefix + level_n + "_ScrArr:";
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( m_level_prefix ) + level_n + "_ScrArr;" );
					}
				}
				
				ent_n = 0;
				
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
								
								level_prefix_str = m_level_prefix + level_n;
								
								if( enable_comments )
								{
									start_scr_ind = level_data.get_start_screen_ind();
									
									if( start_scr_ind < 0 )
									{
										MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A first valid screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
									}
									
									if( RBtnLayoutMatrix.Checked )
									{
										_sw.WriteLine( level_prefix_str + "_StartScr:\t.word " + ( start_scr_ind < 0 ? common_scr_ind:start_scr_ind ) + "\n" );
									}
									else
									{
										_sw.WriteLine( level_prefix_str + "_StartScr:\t.word " + ( start_scr_ind < 0 ? level_prefix_str + "Scr" + common_scr_ind:level_prefix_str + "Scr" + start_scr_ind ) + "\n" );
									}
									
									if( m_C_writer != null )
									{
										m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( level_prefix_str ) + "_StartScr;" );
									}
									
									if( RBtnLayoutMatrix.Checked )
									{
										level_data.export_asm( m_C_writer, _sw, m_level_prefix + level_n, null, ".byte", ".word", "$", false, false, false, false );
									}
								}
								
								exp_scr = screens[ scr_key ];
								
								_sw.WriteLine( level_prefix_str + "Scr" + common_scr_ind + ":" );
								_sw.WriteLine( "\t.byte " + banks.IndexOf( exp_scr.m_tiles ) + ( enable_comments ? "\t; chr_id":"" ) );
								
								if( CheckBoxExportMarks.Checked )
								{
									_sw.WriteLine( "\t.byte " + utils.hex( "$", scr_data.mark_adj_scr_mask ) + ( enable_comments ? "\t; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property":"" ) );
								}
								
								if( RBtnModeStaticScreen.Checked )
								{
									_sw.WriteLine( "\n\t.word " + exp_scr.m_VDC_scr_offset + ( enable_comments ? "\t; " + m_filename + "_VDCScr offset":"" ) );
								}
								
								_sw.WriteLine( "\n\t.byte " + exp_scr.m_scr_ind + ( enable_comments ? "\t; screen index":"" ) );
								
								_sw.WriteLine( "" );
								
								if( RBtnLayoutAdjacentScreens.Checked )
								{
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, -1 	 	) + ( enable_comments ? "\t; left adjacent screen":"" ) );
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, -n_scr_X	) + ( enable_comments ? "\t; up adjacent screen":"" ) );
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, 1 		) + ( enable_comments ? "\t; right adjacent screen":"" ) );
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, n_scr_X	) + ( enable_comments ? "\t; down adjacent screen\n":"\n" ) );
								}
								else
								if( RBtnLayoutAdjacentScreenIndices.Checked )
								{
									if( enable_comments )
									{
										_sw.WriteLine( "; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )" );
										_sw.WriteLine( "; use the " + m_level_prefix + level_n + "_ScrArr array to get a screen description by adjacent screen index" );
									}
									
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, -1 	 	) + ( enable_comments ? "\t; left adjacent screen index":"" ) );
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, -n_scr_X	) + ( enable_comments ? "\t; up adjacent screen index":"" ) );
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, 1 		) + ( enable_comments ? "\t; right adjacent screen index":"" ) );
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, n_scr_X	) + ( enable_comments ? "\t; down adjacent screen index\n":"\n" ) );
									
									scr_arr += "\n\t.word " + m_level_prefix + level_n + "Scr" + common_scr_ind;
								}
								
								if( CheckBoxExportEntities.Checked )
								{
									scr_data.export_entities_asm( _sw, ref ent_n, level_prefix_str + "Scr" + common_scr_ind + "EntsArr", ".byte", ".word", "$", RBtnEntityCoordScreen.Checked, scr_n_X, scr_n_Y, enable_comments );
									
									_sw.WriteLine( "" );
								}
								
								enable_comments = false;
							}
							else
							{
								throw new Exception( "Unexpected error! Can't find a screen data!" );
							}
						}
						else
						{
							if( RBtnLayoutAdjacentScreenIndices.Checked )
							{
								scr_arr += "\n\t.word $00";
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
					_sw.WriteLine( ".define " + CONST_FILENAME_LEVEL_PREFIX + level_n + "_EntInstCnt\t" + ent_n + "\t; number of entities instances\n" );
				}				
			}
			
			foreach( var key in screens.Keys ) { screens[ key ].destroy(); }
			
			if( exp_data_size > 8192 )
			{
				MainForm.message_box( "The exported binary data size exceeds 8K ( " + exp_data_size + " B ) !", "Data Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}

		private bool compress_and_save_byte( BinaryWriter _bw, byte[] _data, ref int _data_offset )
		{
			if( CheckBoxRLE.Checked )
			{
				byte[] rle_data_arr	= null;

				int rle_data_size = utils.RLE( _data, ref rle_data_arr );
				
				if( rle_data_size < 0 )
				{
					return false;
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
			
			return true;
		}

		private bool compress_and_save_ushort( BinaryWriter _bw, ushort[] _data, ref int _data_offset )
		{
			byte[] data_copy = new byte[ _data.Length << 1 ];
			
			Buffer.BlockCopy( _data, 0, data_copy, 0, data_copy.Length );
			
			if( CheckBoxRLE.Checked )
			{
				byte[] rle_data_arr	= null;

				int rle_data_size = utils.RLE( data_copy, ref rle_data_arr );
				
				if( rle_data_size < 0 )
				{
					return false;
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
				_bw.Write( data_copy );
				
				_data_offset += _data.Length * sizeof( ushort );
			}
			
			return true;
		}
		
		private bool compress_and_save_byte( BinaryWriter _bw, byte[] _data )
		{
			int offset = 0;
			
			return compress_and_save_byte( _bw, _data, ref offset );
		}

		private bool compress_and_save_ushort( BinaryWriter _bw, ushort[] _data )
		{
			int offset = 0;
			
			return compress_and_save_ushort( _bw, _data, ref offset );
		}
		
		private void fill_screen_attrs_per_CHR( ushort[] 		_attrs_chr,
		                                       	tiles_data 		_tiles, 
		                                       	screen_data 		_tile_inds, 
		                                       	bool 			_force_swapping, 
		                                       	int 			_scr_width_blocks_mul2, 
		                                       	int 			_scr_height_blocks_mul2, 
		                                       	int 			_scr_height_blocks_mul4,
		                                       	screen_data		_block_inds )
		{
			ushort tile_id			= 0;
			
			int tile_x				= 0;
			int tile_y				= 0;
			int block_x				= 0;
			int block_y				= 0;
			int chr_x				= 0;
			int chr_y				= 0;
			int tile_offs_x			= 0;
			int tile_offs_y			= 0;
			
			int tile_n;
			int block_n;
			int chr_n;
			int tiles_cnt = _tile_inds.length;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				for( tile_n = 0; tile_n < tiles_cnt; tile_n++ )
				{
					tile_id = _tile_inds.get_tile( tile_n );
	
					tile_offs_x = ( tile_n / utils.CONST_SCREEN_NUM_HEIGHT_TILES ) << 1;
					tile_offs_y = ( tile_n % utils.CONST_SCREEN_NUM_HEIGHT_TILES ) << 1;
	
					tile_x	= ( ( tile_offs_x << 1 ) * _scr_height_blocks_mul2 );
					tile_y	= ( tile_offs_y << 1 );
					
					for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
					{
						block_x = ( ( block_n & 0x01 ) == 0x01 ? _scr_height_blocks_mul4:0 );
						block_y = ( ( block_n & 0x02 ) == 0x02 ? 2:0 );
						
						for( chr_n = 0; chr_n < 4; chr_n++ )
						{
							chr_x	= ( ( chr_n & 0x01 ) == 0x01 ? _scr_height_blocks_mul2:0 );
							chr_y	= ( ( chr_n & 0x02 ) == 0x02 ? 1:0 );
							
							// column order by default
							_attrs_chr[ tile_x + tile_y + block_x + block_y + chr_x + chr_y ] = get_screen_attribute( _tiles, tile_id, block_n, chr_n );
						}
					}
				}
			}
			else
			{
				for( block_n = 0; block_n < utils.CONST_SCREEN_BLOCKS_CNT; block_n++ )
				{
					block_x = ( ( block_n / utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS ) << 1 ) * _scr_height_blocks_mul2;
					block_y = ( block_n % utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS ) << 1;

					for( chr_n = 0; chr_n < 4; chr_n++ )
					{
						chr_x	= ( ( chr_n & 0x01 ) == 0x01 ? _scr_height_blocks_mul2:0 );
						chr_y	= ( ( chr_n & 0x02 ) == 0x02 ? 1:0 );
						
						// column order by default
						_attrs_chr[ block_x + block_y + chr_x + chr_y ] = get_screen_attribute( _tiles, _block_inds.get_tile( block_n ), chr_n );
					}
				}
			}
		
			if( RBtnTilesDirRows.Checked || _force_swapping )
			{
				utils.swap_columns_rows_order_ushort( _attrs_chr, _scr_width_blocks_mul2, _scr_height_blocks_mul2 );
			}
		}
		
		private ushort get_screen_attribute( tiles_data _tiles, int _tile_id, int _block_n, int _chr_n )
		{
			return get_screen_attribute( _tiles, _tiles.get_tile_block( _tile_id, _block_n ), _chr_n );
		}
		
		private ushort get_screen_attribute( tiles_data _tiles, int _block_n, int _chr_n )
		{
			uint block_data = _tiles.blocks[ ( _block_n << 2 ) + _chr_n ];
			
			return get_screen_attribute( block_data );
		}
		
		private ushort get_screen_attribute( uint _block_data )
		{
			return ( ushort )tiles_data.block_data_repack_to_native( _block_data, get_CHR_offset() );
		}
		
		private string get_adjacent_screen_index( int _level_n, layout_data _data, int _scr_ind, int _offset )
		{
			int adj_scr_ind = _data.get_adjacent_screen_index( _scr_ind, _offset );
			
			if( adj_scr_ind > 255 )
			{
				throw new Exception( "Layout: " + _level_n + " error!\nThe maximum number of cells in a layout must be 256 for the \"Adjacent Screen Indices\" mode!" );
			}
			
			return utils.hex( "$", ( adj_scr_ind >= 0 ? adj_scr_ind:255 ) );
		}
		
		private int get_CHR_offset()
		{
			return m_CHR_offset[ ComboBoxBAT.SelectedIndex ] + ( int )NumericUpDownCHROffset.Value;
		}
		
		private string get_adjacent_screen_label( int _level_n, layout_data _data, int _scr_ind, int _offset )
		{
			int adj_scr_ind = _data.get_adjacent_screen_index( _scr_ind, _offset );
			
			return ( adj_scr_ind >= 0 ? m_level_prefix + _level_n + "Scr" + adj_scr_ind:"0" );
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
		
		private void save_multidir_scroll_mode( StreamWriter _sw )
		{
			BinaryWriter 	bw = null;
			
			layout_data level_data = null;
			
			byte[]	map_data_arr		= null;
			byte[]	map_tiles_arr		= null;
			byte[]	map_blocks_arr		= null;
			byte[]	block_props_arr		= null;

			List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
			
			int n_levels 			= m_data_mngr.layouts_data_cnt;
			int n_scr_X 			= 0;
			int n_scr_Y				= 0;
			int tile_n				= 0;
			int block_n				= 0;
			int n_Y_tiles			= 0;
			int n_screens 			= 0;
			int scr_ind				= 0;
			int bank_ind			= 0;
			int chk_bank_ind		= 0;
			int n_banks				= scr_tiles_data.Count;
			int tile_offs_x			= 0;
			int tile_offs_y			= 0;
			int max_tile_ind 		= 0;
			int max_block_ind 		= 0;
			int blocks_props_size	= 0;
			ushort tile_id			= 0;
			byte block_id			= 0;
			uint block_data			= 0;
			
			layout_screen_data	scr_data;
			
			long 	data_size 		= 0;
			long	exp_data_size	= 0;
			
			string 	label 		= null;
			string 	palette_str	= null;
			
			string level_prefix_str	= null;
			
			int scr_width_blocks 	= utils.CONST_SCREEN_NUM_WIDTH_BLOCKS;
			int scr_height_blocks 	= utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS;
			
			tiles_data tiles = null;

			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				level_data = m_data_mngr.get_layout_data( level_n );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				n_Y_tiles = n_scr_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES * ( RBtnTiles4x4.Checked ? 1:2 );
				
				// game level tilemap analysing
				{
					map_tiles_arr = new byte[ n_scr_X * n_scr_Y * utils.CONST_SCREEN_TILES_CNT ];
					
					Array.Clear( map_tiles_arr, 0, map_tiles_arr.Length );
				}
				
				if( RBtnTiles2x2.Checked )
				{
					map_blocks_arr = new byte[ n_Y_tiles * n_scr_X * ( utils.CONST_SCREEN_WIDTH_PIXELS >> 4 ) ];
					
					Array.Clear( map_blocks_arr, 0, map_blocks_arr.Length );
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
							
							// convert a screen index into local index in the bank
							scr_ind = scr_data.m_scr_ind - ( n_screens - scr_tiles_data[ bank_ind ].screen_data_cnt() );
							
							// fill the map by tiles of a current screen
							tiles = scr_tiles_data[ bank_ind ];
							
							if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
							{
								for( tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
								{
									tile_offs_x = ( tile_n % utils.CONST_SCREEN_NUM_WIDTH_TILES );
									tile_offs_y = ( tile_n / utils.CONST_SCREEN_NUM_WIDTH_TILES );
									
									tile_id = tiles.get_screen_tile( scr_ind, tile_n );
									
									if( RBtnTiles2x2.Checked )
									{
										map_tiles_arr[ scr_n_X * ( ( n_scr_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES ) * utils.CONST_SCREEN_NUM_WIDTH_TILES ) + ( scr_n_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES ) + ( tile_offs_x * ( n_scr_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES ) ) + tile_offs_y ] = ( byte )tile_id;
										
										tile_offs_x <<= 1;
										tile_offs_y <<= 1;
										
										// make a list of 2x2 tiles of the current map
										for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
										{
											block_id = ( byte )tiles.get_tile_block( tile_id, block_n );
											{
												map_blocks_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + ( ( block_n & 0x01 ) == 0x01 ? n_Y_tiles:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ) ] = block_id;
											}
										}
									}
									else
									{
										map_tiles_arr[ scr_n_X * ( n_Y_tiles * utils.CONST_SCREEN_NUM_WIDTH_TILES ) + ( scr_n_Y * utils.CONST_SCREEN_NUM_HEIGHT_TILES ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = ( byte )tile_id;
									}
								}
							}
							else
							{
								// make a list of 2x2 tiles of the current map
								for( block_n = 0; block_n < utils.CONST_SCREEN_BLOCKS_CNT; block_n++ )
								{
									tile_offs_x = ( block_n % utils.CONST_SCREEN_NUM_WIDTH_BLOCKS );
									tile_offs_y = ( block_n / utils.CONST_SCREEN_NUM_WIDTH_BLOCKS );
									
									map_blocks_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = ( byte )tiles.get_screen_tile( scr_ind, block_n );
								}
							}
						}
					}
				}

				// write collected data
				_sw.WriteLine( "\n; *** " + CONST_FILENAME_LEVEL_PREFIX + level_n + " ***\n" );
				
				level_prefix_str = m_level_prefix + level_n;
				
				tiles = scr_tiles_data[ chk_bank_ind ]; 
				
				// write CHR banks data
				label = level_prefix_str + "_CHR";
				bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
				{
					tiles.export_CHR( bw );
				}
				long CHR_data_size = data_size = bw.BaseStream.Length;
				bw.Close();
				
				exp_data_size += data_size;
				
				_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ")" );
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( label ) + ";" );
				}

				if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
				{
					max_tile_ind = 1 + map_tiles_arr.Max();	// one based index
				}
				else
				{
					max_tile_ind = 1 + tiles.get_first_free_block_id();
				}
				
				if( RBtnTiles4x4.Checked )
				{
					// write tiles
					label = level_prefix_str + "_Tiles";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( int i = 0; i < max_tile_ind; i++ )
					{
						bw.Write( rearrange_tile( tiles.tiles[ i ] ) );
					}
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") (4x4) 4 block indices per tile ( left to right, up to down )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( label ) + ";" );
					}
				}
				
				// tiles 2x2
				{
					max_block_ind = tiles.get_first_free_block_id() << 2;	// 4 ushorts per block
					max_block_ind = max_block_ind < 0 ? utils.CONST_MAX_BLOCKS_CNT:max_block_ind;
					
					label = level_prefix_str + "_Attrs";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
	
					for( block_n = 0; block_n < max_block_ind; block_n++ )
					{
						block_data = tiles.blocks[ block_n ];
						
						bw.Write( get_screen_attribute( block_data ) );
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; " + "(" + data_size + ") attributes array per block ( 2 bytes per attribute; 8 bytes per block )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( label ) + ";" );
					}
				}
				
				// blocks and properties
				{
					if( m_data_mngr.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						blocks_props_size = ( 1 + utils.get_uint_arr_max_val( tiles.tiles, max_tile_ind ) ) << 2;//max_tile_ind << 2 ) ) << 2;
					}
					else
					{
						blocks_props_size = ( 1 + tiles.get_first_free_block_id() ) << 2;
					}
					
					block_props_arr = new byte[ RBtnPropPerBlock.Checked ? ( blocks_props_size >> 2 ):blocks_props_size ];
					Array.Clear( block_props_arr, 0, block_props_arr.Length );
					
					for( block_n = 0; block_n < blocks_props_size; block_n++ )
					{
						block_data = tiles.blocks[ block_n ];
						
						if( RBtnPropPerBlock.Checked && ( block_n % 4 ) != 0 )
						{
							continue;
						}
						
						block_props_arr[ RBtnPropPerBlock.Checked ? ( ( block_n + 1 ) >> 2 ):block_n ]	= (byte)tiles_data.get_block_flags_obj_id( block_data );
					}
					
					// save properties
					label = level_prefix_str + "_Props";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					bw.Write( block_props_arr );
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") blocks properties array ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( label ) + ";" );
					}
				}

				// write map
				{
					label = level_prefix_str + "_Map";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					if( RBtnTiles2x2.Checked )
					{
						map_data_arr = map_blocks_arr;
					}
					else
					{
						map_data_arr = map_tiles_arr;
					}
					
					if( RBtnTilesDirRows.Checked )
					{
						utils.swap_columns_rows_order<byte>( map_data_arr, get_tiles_cnt_width( n_scr_X ), get_tiles_cnt_height( n_scr_Y ) );
					}
					
					if( compress_and_save_byte( bw, map_data_arr ) == false )
					{
						_sw.Close();
						bw.Close();
						throw new System.Exception( "Can't compress an empty data!" );
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_size + ( CheckBoxRLE.Checked ? " / " + map_data_arr.Length:"" ) + ") game level " + ( RBtnTiles4x4.Checked ? "tiles (4x4)":"blocks (2x2)" ) + " array" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern char*\t" + utils.skip_exp_pref( label ) + ";" );
					}
				}
				
				// tiles lookup table
				{
					label = level_prefix_str + "_MapTbl";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					int w_tiles_cnt = get_tiles_cnt_width( n_scr_X );
					int h_tiles_cnt = get_tiles_cnt_height( n_scr_Y );
					
					if( RBtnTilesDirColumns.Checked )
					{
						for( int i = 0; i < w_tiles_cnt; i++ )
						{
							bw.Write( ( ushort )( i * h_tiles_cnt ) );
						}
					}
					else
					{
						for( int i = 0; i < h_tiles_cnt; i++ )
						{
							bw.Write( ( ushort )( i * w_tiles_cnt ) );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					exp_data_size += data_size;
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") lookup table for fast calculation of tile addresses; " + ( RBtnTilesDirColumns.Checked ? "columns by X coordinate":"rows by Y coordinate" ) + " ( 16 bit offset per " + ( RBtnTilesDirColumns.Checked ? "column":"row" ) + " of tiles )\n" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( label ) + ";" );
					}
				}
				
				palette_str = level_prefix_str + "_Palette:";
			
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern int*\t" + utils.skip_exp_pref( level_prefix_str ) + "_Palette;" );
				}
				
				for( int i = 0; i < tiles.palettes_arr.Count; i++ )
				{
					palette_str += "\n\t\t.word ";
					
					fill_palette_str( tiles.palettes_arr[ i ].m_palette0, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette1, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette2, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette3, ref palette_str, true );
				}
				
				_sw.WriteLine( palette_str + "\n" + ( level_prefix_str + "_Palette_End:\n" ) );

				int start_scr_ind = level_data.get_start_screen_ind();
				
				if( start_scr_ind < 0 )
				{
					MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A zero screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
					
					start_scr_ind = 0;
				}

				StreamWriter def_sw = _sw;
				
				string c_char		= "";
				string c_int		= "";
				string c_code_comm_delim = "\t; ";
				
				if( m_C_writer != null )
				{
					level_prefix_str 	= utils.skip_exp_pref( level_prefix_str );
					c_char				= "char\t";
					c_int				= "int\t";
					c_code_comm_delim 	= ";\t// ";
					
					def_sw	= m_C_writer;
				}

				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( c_char + level_prefix_str + "_StartScr\t= " + start_scr_ind + c_code_comm_delim + "start screen" );
				}
				else
				{
					_sw.WriteLine( level_prefix_str + "_StartScr:\t.word " + start_scr_ind + "\t; start screen\n" );
				}
				
				def_sw.WriteLine( c_int + level_prefix_str + "_CHR_data_size\t= " + CHR_data_size + c_code_comm_delim + "map CHRs size in bytes" );
				
				def_sw.WriteLine( c_int + level_prefix_str + "_WTilesCnt\t= " + get_tiles_cnt_width( n_scr_X ) + c_code_comm_delim + "number of level tiles in width" );
				def_sw.WriteLine( c_int + level_prefix_str + "_HTilesCnt\t= " + get_tiles_cnt_height( n_scr_Y ) + c_code_comm_delim + "number of level tiles in height" );
				
				def_sw.WriteLine( c_int + level_prefix_str + "_WPixelsCnt\t= " + get_tiles_cnt_width( n_scr_X ) * ( RBtnTiles2x2.Checked ? 16:32 ) + c_code_comm_delim + "map width in pixels" );
				def_sw.WriteLine( c_int + level_prefix_str + "_HPixelsCnt\t= " + get_tiles_cnt_height( n_scr_Y ) * ( RBtnTiles2x2.Checked ? 16:32 ) + c_code_comm_delim + "map height in pixels" );
					
				if( RBtnTiles4x4.Checked )
				{
					def_sw.WriteLine( c_char + level_prefix_str + "_TilesCnt\t= " + max_tile_ind + c_code_comm_delim + "map tiles count" );
				}
				
				def_sw.WriteLine( c_char + level_prefix_str + "_BlocksCnt\t= " + ( max_block_ind >> 2 ) + c_code_comm_delim + "map blocks count\n" );

				if( CheckBoxExportEntities.Checked )
				{
					level_data.export_asm( m_C_writer, _sw, get_exp_prefix() + level_prefix_str, null, ".byte", ".word", "$", true, CheckBoxExportMarks.Checked, CheckBoxExportEntities.Checked, RBtnEntityCoordScreen.Checked );
				}
				else
				{
					def_sw.WriteLine( c_char + level_prefix_str + "_WScrCnt\t= " + n_scr_X + c_code_comm_delim + "number of screens in width" );
					def_sw.WriteLine( c_char + level_prefix_str + "_HScrCnt\t= " + n_scr_Y + c_code_comm_delim + "number of screens in height\n" );
				}
				
				map_data_arr 	= null;
				map_tiles_arr 	= null;
				map_blocks_arr	= null;
				block_props_arr	= null;
			}
			
			if( exp_data_size > 8192 )
			{
				MainForm.message_box( "The exported binary data size exceeds 8K ( " + exp_data_size + " B ) !", "Data Export Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
		
		int get_tiles_cnt_width( int _scr_cnt_x )
		{
			return RBtnTiles2x2.Checked ? _scr_cnt_x * utils.CONST_SCREEN_NUM_WIDTH_BLOCKS:_scr_cnt_x * utils.CONST_SCREEN_NUM_WIDTH_TILES;
		}

		int get_tiles_cnt_height( int _scr_cnt_y )
		{
			return RBtnTiles2x2.Checked ? _scr_cnt_y * utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS:_scr_cnt_y * utils.CONST_SCREEN_NUM_HEIGHT_TILES;
		}
		
		void fill_palette_str( int[] _plt, ref string _str, bool _end )
		{
			for( int j = 0; j < utils.CONST_PALETTE_SMALL_NUM_COLORS; j++ )
			{
				_str += String.Format( "${0:X2}", _plt[ j ] ) + ( !( _end && j == 3 ) ? ", ":"" );
			}
		}
		
		private uint rearrange_tile( uint _val )
		{
			byte v0 = ( byte )( ( _val >> 24 ) & 0xff );
			byte v1 = ( byte )( ( _val >> 16 ) & 0xff );
           	byte v2 = ( byte )( ( _val >> 8 ) & 0xff );
          	byte v3 = ( byte )( _val & 0xff );
			
          	return unchecked( ( uint )( v3 << 24 | v2 << 16 | v1 << 8 | v0 ) );
		}
#endif	//DEF_PCE
	}
}