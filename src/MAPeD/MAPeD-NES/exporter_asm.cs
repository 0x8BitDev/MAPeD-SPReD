/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 13.09.2018
 * Time: 17:59
 */
 
//#define DEF_DBG_PPU_READY_DATA_SAVE_IMG
//#define DEF_MMC5_CHR_DATA_COMPACTION
 
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
	/// Description of exporter_nes_asm.
	/// </summary>
	/// 

	public partial class exporter_asm : Form
	{
		private readonly data_sets_manager m_data_mngr;
		
		private string	m_filename			= null;
		private string	m_path				= null;
		private string 	m_path_filename_ext	= null;
		private string 	m_path_filename		= null;
		
		private const string	CONST_FILENAME_LEVEL_PREFIX		= "Lev";
		private const string	CONST_FILENAME_BANK_PREFIX		= "Bank";
		private const string	CONST_BIN_EXT					= ".bin";
		
		private const int		CONST_BIDIR_MAP_SCREEN_MAX_CNT	= 255;	// 1...255 [zero-based index 0..254; 255 - empty screen]
	
		private struct exp_screen_data
		{
			public int 			m_scr_ind;
			
			public tiles_data 	m_tiles;
			
			public screen_data	m_scr_tiles;
			public screen_data	m_scr_blocks;
			
			public int			m_tiles_offset;
			public int			m_blocks_offset;
			
			public int			m_PPU_scr_offset;
			public int			m_MMC5_scr_attrs_offset;
			
			public static int	_tiles_offset;
			public static int	_blocks_offset;
			
			public exp_screen_data( int _scr_ind, tiles_data _tiles )
			{
				m_scr_ind		= _scr_ind;
				
				m_tiles 		= _tiles;
				
				m_scr_tiles		= new screen_data( data_sets_manager.e_screen_data_type.Tiles4x4 );
				m_scr_blocks	= new screen_data( data_sets_manager.e_screen_data_type.Blocks2x2 );
				
				m_tiles_offset	= _tiles_offset;
				m_blocks_offset	= _blocks_offset;
				
				_tiles_offset 	+= m_scr_tiles.length;
				_blocks_offset 	+= m_scr_blocks.length;
				
				m_PPU_scr_offset		= 0;
				m_MMC5_scr_attrs_offset	= 0;
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
			
			RBtnAttrsPerCHR.Enabled = NumericUpDownCHRBankIndex.Enabled = false;
			
			CBoxEntSortingType.SelectedIndex = 0;
			
			update_desc();
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
			RBtnLayoutMatrix.Checked 	= true;
			
			CheckBoxExportMarks.Enabled = ( CheckBoxExportEntities.Checked == false && RBtnModeMultidirScroll.Checked ) ? false:true;
			CheckBoxExportMarks.Checked = CheckBoxExportMarks.Enabled ? CheckBoxExportMarks.Checked:false;
			
			RBtnEntityCoordMap.Checked	= true;
			
			CheckBoxRLE.Enabled = true;
			
			RBtnAttrsPerBlock.Checked = true;
			RBtnAttrsPerCHR.Enabled = NumericUpDownCHRBankIndex.Enabled = false;
			
			update_desc();
		}
		
		private void RBtnModeScreenToScreenChanged( object sender, EventArgs e )
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			RBtnLayoutAdjacentScreens.Checked	= true;
			
			CheckBoxExportMarks.Enabled = true;
			
			RBtnEntityCoordScreen.Checked	= true;
			
			CheckBoxRLE.Checked = CheckBoxRLE.Enabled = false;

			RBtnAttrsPerCHR.Enabled = true;
			
			update_desc();
		}

		private void RBtnModeStaticScreensChanged( object sender, EventArgs e ) 
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			RBtnLayoutAdjacentScreens.Checked	= true;
			
			CheckBoxExportMarks.Enabled = true;
			
			RBtnEntityCoordScreen.Checked	= true;
			
			CheckBoxRLE.Enabled = true;
			
			RBtnAttrsPerCHR.Enabled = true;
			
			update_desc();
		}

		private void RBtnAttrsPerChanged( object sender, EventArgs e )
		{
			NumericUpDownCHRBankIndex.Enabled = ( sender as RadioButton ) == RBtnAttrsPerBlock ? false:true;
			
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
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_TILES_2X2;
			}
			else
			{
				RichTextBoxExportDesc.Text = strings.CONST_STR_EXP_TILES_4X4;
			}
			
			if( CheckBoxRLE.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NES_RLE_COMPRESSION;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER;
			
			if( RBtnTilesDirColumns.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NES_DATA_ORDER_COLS;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NES_DATA_ORDER_ROWS;
			}

			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NES_ATTRS;
			
			if( RBtnAttrsPerBlock.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NES_ATTRS_PER_BLOCK;
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NES_ATTRS_PER_CHR;
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
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE;
			
			if( RBtnModeMultidirScroll.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_MODE_MULTIDIR;
			}
			else
			if( RBtnModeBidirScroll.Checked )
			{
				RichTextBoxExportDesc.Text += string.Format( strings.CONST_STR_EXP_MODE_BIDIR, CONST_BIDIR_MAP_SCREEN_MAX_CNT );
			}
			else
			{
				RichTextBoxExportDesc.Text += string.Format( strings.CONST_STR_EXP_NES_MODE_STAT_SCR, CONST_BIDIR_MAP_SCREEN_MAX_CNT );
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
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_WARNING;
		}

		private void BtnCancelClick( object sender, System.EventArgs e )
		{
			this.Close();
		}

		public DialogResult show_window( string _full_path )
		{
			m_path_filename_ext 	= _full_path;
			m_filename				= Path.GetFileNameWithoutExtension( _full_path );
			m_path					= Path.GetDirectoryName( _full_path ) + Path.DirectorySeparatorChar;
			m_path_filename			= m_path + m_filename;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Blocks2x2 )
			{
				RBtnTiles2x2.Checked = true;
				RBtnTiles2x2.Enabled = RBtnTiles4x4.Enabled = false;
			}
			else
			{
				RBtnTiles2x2.Enabled = RBtnTiles4x4.Enabled = true;
			}
			
			return ShowDialog();
		}
		
		private void BtnOkClick( object sender, System.EventArgs e )
		{
			this.Close();

			StreamWriter sw = null;
			
			try
			{
				sw = new StreamWriter( m_path_filename_ext );
				
				utils.write_title( sw );

				// options
				{
					sw.WriteLine( "; export options:" );

					sw.WriteLine( ";\t- tiles " + ( RBtnTiles4x4.Checked ? "4x4":"2x2" ) + ( CheckBoxRLE.Checked ? " (RLE)":"" ) + ( RBtnTilesDirColumns.Checked ? "/(columns)":"/(rows)" ) );
					
					sw.WriteLine( ";\t- attributes per " + ( RBtnAttrsPerBlock.Checked ? "block":"CHR (MMC5)" ) );
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
					
					sw.WriteLine( ";\t- " + ( CheckBoxExportEntities.Checked ? "entities " + ( RBtnEntityCoordScreen.Checked ? "(screen coordinates)":"(map coordinates)" ) + ( CBoxEntSortingType.SelectedIndex == 0 ? "/no sorting":( CBoxEntSortingType.SelectedIndex == 1 ) ? "/left-right":"/bottom-top" ):"no entities" ) );
					sw.WriteLine( "\n" );
				}
				
				sw.WriteLine( "MAP_DATA_MAGIC = " + utils.hex( "$", ( RBtnTiles2x2.Checked ? 1:2 ) | 
				                                              		( CheckBoxRLE.Checked ? 4:0 ) |
				                                              		( RBtnTilesDirColumns.Checked ? 8:16 ) |
				                                              		( RBtnModeMultidirScroll.Checked ? 32:RBtnModeBidirScroll.Checked ? 64:128 ) | 
				                                              		( CheckBoxExportEntities.Checked ? 256:0 ) |
				                                              		( CheckBoxExportEntities.Checked ? ( RBtnEntityCoordScreen.Checked ? 512:1024 ):0 ) |
				                                              		( RBtnLayoutAdjacentScreens.Checked ? 2048:RBtnLayoutAdjacentScreenIndices.Checked ? 4096:8192 ) |
				                                              		( CheckBoxExportMarks.Checked ? 16384:0 ) |
 				                                              		( RBtnAttrsPerBlock.Checked ? 32768:65536 ) |
 				                                              		( RBtnPropPerBlock.Checked ? 131072:262144 ) |
																	( CBoxEntSortingType.SelectedIndex == 0 ? 524288:0 ) |
																	( CBoxEntSortingType.SelectedIndex == 1 ? 1048576:0 ) |
																	( CBoxEntSortingType.SelectedIndex == 2 ? 2097152:0 ) ) );
				sw.WriteLine( "\n; data flags:" );
				sw.WriteLine( "MAP_FLAG_TILES2X2                 = " + utils.hex( "$", 1 ) );
				sw.WriteLine( "MAP_FLAG_TILES4X4                 = " + utils.hex( "$", 2 ) );
				sw.WriteLine( "MAP_FLAG_RLE                      = " + utils.hex( "$", 4 ) );
				sw.WriteLine( "MAP_FLAG_DIR_COLUMNS              = " + utils.hex( "$", 8 ) );
				sw.WriteLine( "MAP_FLAG_DIR_ROWS                 = " + utils.hex( "$", 16 ) );
				sw.WriteLine( "MAP_FLAG_MODE_MULTIDIR_SCROLL     = " + utils.hex( "$", 32 ) );
				sw.WriteLine( "MAP_FLAG_MODE_BIDIR_SCROLL        = " + utils.hex( "$", 64 ) );
				sw.WriteLine( "MAP_FLAG_MODE_STATIC_SCREENS      = " + utils.hex( "$", 128 ) );
				sw.WriteLine( "MAP_FLAG_ENTITIES                 = " + utils.hex( "$", 256 ) );
				sw.WriteLine( "MAP_FLAG_ENTITY_SCREEN_COORDS     = " + utils.hex( "$", 512 ) );
				sw.WriteLine( "MAP_FLAG_ENTITY_MAP_COORS         = " + utils.hex( "$", 1024 ) );
				sw.WriteLine( "MAP_FLAG_LAYOUT_ADJACENT_SCREENS  = " + utils.hex( "$", 2048 ) );
				sw.WriteLine( "MAP_FLAG_LAYOUT_ADJACENT_SCR_INDS = " + utils.hex( "$", 4096 ) );
				sw.WriteLine( "MAP_FLAG_LAYOUT_MATRIX            = " + utils.hex( "$", 8192 ) );
				sw.WriteLine( "MAP_FLAG_MARKS                    = " + utils.hex( "$", 16384 ) );
				sw.WriteLine( "MAP_FLAG_ATTRS_PER_BLOCK          = " + utils.hex( "$", 32768 ) );
				sw.WriteLine( "MAP_FLAG_ATTRS_PER_CHR            = " + utils.hex( "$", 65536 ) );
				sw.WriteLine( "MAP_FLAG_PROP_ID_PER_BLOCK        = " + utils.hex( "$", 131072 ) );
				sw.WriteLine( "MAP_FLAG_PROP_ID_PER_CHR          = " + utils.hex( "$", 262144 ) );
				sw.WriteLine( "MAP_FLAG_ENT_SORTING_OFF          = " + utils.hex( "$", 524288 ) );
				sw.WriteLine( "MAP_FLAG_ENT_SORTING_LT_RT        = " + utils.hex( "$", 1048576 ) );
				sw.WriteLine( "MAP_FLAG_ENT_SORTING_BTM_TOP      = " + utils.hex( "$", 2097152 ) );
				
				sw.WriteLine( "\nSCR_BLOCKS2x2_WIDTH\t= " + platform_data.get_screen_blocks_width() + "\t; number of screen blocks (2x2) in width" );
				sw.WriteLine( "SCR_BLOCKS2x2_HEIGHT\t= " + platform_data.get_screen_blocks_height() + "\t; number of screen blocks (2x2) in height" );
				
				if( CheckBoxExportEntities.Checked )
				{
					m_data_mngr.export_base_entity_asm( sw, ".byte", "$" );
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
				MainForm.message_box( _err.Message, "CA65 / NES Asm Data Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error ); 
			}
			
			finally
			{
				if( sw != null )
				{
					sw.Close();
				}
			}
		}
		
		private void save_single_screen_mode( StreamWriter _sw )
		{
			BinaryWriter 	bw;
			BinaryWriter	bw_props;
			BinaryWriter	bw_blocks	= null;
			
			layout_data level_data;

			List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
			
			int n_banks		= scr_tiles_data.Count;
			int n_levels	= m_data_mngr.layouts_data_cnt;
			int n_scr_X;
			int n_scr_Y;
			int n_screens;
			int scr_ind;
			int scr_ind_opt;
			int bank_ind;
			int scr_key;
			int tile_n;
			int tiles_cnt;
			int tile_offs_x;
			int tile_offs_y;
			int block_n;
			int chr_n;
			int bank_n;
			int blocks_props_size;
			int data_offset;
			int attr;
			int common_scr_ind;
			int max_scr_tile;
			int max_tile_ind;
			int start_scr_ind;
			int CHR_id;
			int ents_cnt;
			
			int block_x;
			int block_y;
			int chr_x;
			int chr_y;
			
			int scr_width_blocks_mul2;
			int scr_height_blocks_mul2;
			int scr_height_blocks_mul4;
			
			int scr_width_blocks 	= platform_data.get_screen_blocks_width();
			int scr_height_blocks 	= platform_data.get_screen_blocks_height();

			uint block_data;
			
			ushort tile_id	= 0;
			ushort block_id	= 0;
			
			screen_data tile_inds;
			byte[] tile_attrs_arr	= new byte[ 16 ];
			
			long data_size 	= 0;

			bool valid_bank;
			bool enable_comments;
			
			string label;
			string level_prefix_str;
			string label_props;
			string data_offset_str;
			string maps_arr;
			string label_blocks	= null;
			string scr_arr		= null;
			
			exp_screen_data		exp_scr;
			layout_screen_data	scr_data;
			tiles_data 			tiles;
			
			tiles_data			cntrl_tiles_data;
			
			bool diff_banks_in_layout 		= false;
			bool more_than_1_plt_in_bank 	= false;
			
			List< tiles_data > 	banks 			= new List< tiles_data >( 10 );
			List< int >			max_tile_inds	= new List< int >( 10 );
			List< int >			max_block_inds	= new List< int >( 10 );
			
			int[] banks_size_arr	= new int[ m_data_mngr.tiles_data_cnt + 1 ];
			banks_size_arr[ 0 ] 	= 0;

			scr_width_blocks_mul2	= scr_width_blocks << 1;
			scr_height_blocks_mul2	= scr_height_blocks << 1;
			scr_height_blocks_mul4	= scr_height_blocks << 2;

			byte[] attrs_chr = new byte[ ( RBtnModeStaticScreen.Checked && RBtnAttrsPerCHR.Checked ) ? ( 16 * 15 * 4 ):( scr_width_blocks * scr_height_blocks ) << 2 ];
						
			exp_screen_data._tiles_offset  = 0;
			exp_screen_data._blocks_offset = 0;

			Dictionary< int, exp_screen_data > screens = new Dictionary< int, exp_screen_data >( 100 );

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
								
								tile_id = exp_scr.m_tiles.get_screen_tile( scr_n, tile_n );
		
								exp_scr.m_scr_tiles.set_tile( tile_offs_x * platform_data.get_screen_tiles_height() + tile_offs_y, tile_id );
								
								if( RBtnTiles2x2.Checked )
								{
									tile_offs_x <<= 1;
									tile_offs_y <<= 1;
									
									// fill the array of all tiles 2x2 in a level
									for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
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
			
			var screen_sorted_keys_list = screens.Keys.ToList();
			screen_sorted_keys_list.Sort();

			// global data writing
			{
				_sw.WriteLine( "\n; *** GLOBAL DATA ***\n" );
				
				// CHR
				for( bank_n = 0; bank_n < banks.Count; bank_n++ )
				{
					tiles = banks[ bank_n ];
					
					// write CHR bank data
					label = "chr" + bank_n;
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					{
						banks_size_arr[ bank_n + 1 ] += banks_size_arr[ bank_n ] + ( int )( data_size = tiles.export_CHR( bw ) );
					}
					bw.Close();
					
					_sw.WriteLine( "; " + label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ")" );
				}
				
				// static screens ( PPU-READY DATA ):
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// [SKIP] attrs map arr - attributes array for each screen
				// [SKIP] blocks arr	- blocks data array
				// map arr + attrs		- GFX array for each screen (RLE) (PPU ready) [CHRs+Attrs]
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
					_sw.WriteLine( "" );
					
					if( RBtnModeBidirScroll.Checked )
					{
						label_blocks = "_Blocks";
						bw_blocks = new BinaryWriter( File.Open( m_path_filename + label_blocks + CONST_BIN_EXT, FileMode.Create ) );
					}

					label_props = "_Props";
					bw_props = new BinaryWriter( File.Open( m_path_filename + label_props + CONST_BIN_EXT, FileMode.Create ) );
					
					data_offset = 0;
					data_offset_str = "";
					
					for( bank_n = 0; bank_n < banks.Count; bank_n++ )
					{
						tiles = banks[ bank_n ];

						if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
						{
							blocks_props_size = ( 1 + utils.get_ulong_arr_max_val( tiles.tiles, max_tile_inds[ bank_n ] ) ) << 2;
						}
						else
						{
							blocks_props_size = tiles.get_first_free_block_id( false ) << 2;
						}
						
						for( block_n = 0; block_n < blocks_props_size; block_n++ )
						{
							block_data = tiles.blocks[ block_n ];
							
							if( RBtnModeBidirScroll.Checked )
							{
								bw_blocks.Write( ( byte )tiles_data.get_block_CHR_id( block_data ) );
							}

							if( RBtnPropPerBlock.Checked && ( block_n % 4 ) != 0 )
							{
								continue;
							}
							
							bw_props.Write( ( byte )tiles_data.get_block_flags_obj_id( block_data ) );
						}
						
						data_offset_str += "\t.word " + data_offset + "\t\t; (chr" + bank_n + ")\n";

						data_offset += blocks_props_size;
					}

					if( RBtnModeBidirScroll.Checked )
					{
						data_size = bw_blocks.BaseStream.Length;
						bw_blocks.Close();
						
						_sw.WriteLine( m_filename + label_blocks + ":\t.incbin \"" + m_filename + label_blocks + CONST_BIN_EXT + "\"\t; (" + data_size + ") blocks array of all exported data banks ( 4 bytes per block )" );
					}
					
					data_size = bw_props.BaseStream.Length;
					bw_props.Close();
					
					_sw.WriteLine( m_filename + label_props + ":\t.incbin \"" + m_filename + label_props + CONST_BIN_EXT + "\"\t; (" + data_size + ") block properties array of all exported data banks ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block" + ( RBtnPropPerBlock.Checked ? "; data offset = props offset / 4":"" ) + " )\n" );
					
					if( RBtnModeBidirScroll.Checked )
					{
						_sw.WriteLine( m_filename + "_BlocksPropsOffs:" );
					}
					else
					{
						_sw.WriteLine( m_filename + "_PropsOffs:" );
					}
					
					_sw.WriteLine( data_offset_str );
				}
				
				// tiles
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
						tiles = banks[ bank_n ];
						
						max_tile_ind = max_tile_inds[ bank_n ];	// one based index
						
						for( int i = 0; i < max_tile_ind; i++ )
						{
							bw.Write( tiles_data.tile_convert_ulong_to_uint_reverse( tiles.tiles[ i ] ) );
						}
						
						data_offset_str += "\t.word " + data_offset + "\t\t; (chr" + bank_n + ")\n";
						
						data_offset += max_tile_inds[ bank_n ] << 2;
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") tiles (4x4) array of all exported data banks ( 4 bytes per tile )\n" );
					
					_sw.WriteLine( m_filename + "_TilesOffs:" );

					_sw.WriteLine( data_offset_str );
				}
				else
				{
					if( RBtnModeBidirScroll.Checked )
					{
						// write attributes map
						label = "_AttrsScr";
						bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
						
						foreach( int key in screen_sorted_keys_list ) 
						{
							exp_scr = screens[ key ];
							
							tile_inds	= exp_scr.m_scr_tiles;
							tiles_cnt 	= tile_inds.length;
							tiles 		= exp_scr.m_tiles;

							if( RBtnAttrsPerCHR.Checked )
							{
								fill_screen_attrs_per_CHR( 	attrs_chr, 
								                          	banks.IndexOf( tiles ),
								                          	banks_size_arr, 
								                          	tiles, 
								                          	tile_inds, 
								                          	false/*force_swapping*/, 
								                          	scr_width_blocks_mul2,
								                          	scr_height_blocks_mul2, 
								                          	scr_height_blocks_mul4,
								                          	exp_scr.m_scr_blocks );
								
								bw.Write( attrs_chr );
							}
							else
							{
								if( RBtnTilesDirRows.Checked )
								{
									if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
									{
										tile_inds.swap_col_row_data();
									}
									else
									{
										exp_scr.m_scr_blocks.swap_col_row_data();
									}
								}						
								
								for( tile_n = 0; tile_n < tiles_cnt; tile_n++ )
								{
									bw.Write( get_tile_attribute( tile_n, exp_scr, scr_height_blocks ) );
								}
							}
						}

						data_size = bw.BaseStream.Length;
						bw.Close();
						
						if( RBtnAttrsPerCHR.Checked )
						{
							_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") attributes array of all exported screens ( " + attrs_chr.Length + " bytes per screen \\ MMC5 )" );
						}
						else
						{
							_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") attributes array of all exported screens ( " + platform_data.get_screen_tiles_cnt() + " bytes per screen \\ 1 byte per attribute )" );
						}
					}
				}
				
				// save PPU-ready data for STATIC SCREENS mode
				if( RBtnModeStaticScreen.Checked )
				{
					label = "_PPUScr";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );

					byte[] CHRs_arr = new byte[ ( 16 * 16 * 4 ) - ( RBtnAttrsPerCHR.Checked ? 64:0 ) ];

					data_offset = 0;
					
					foreach( var key in screen_sorted_keys_list )
					{
						exp_scr = screens[ key ];
						
						tile_inds	= exp_scr.m_scr_tiles;
						tiles_cnt 	= tile_inds.length;
						tiles 		= exp_scr.m_tiles;
						
						Array.Clear( CHRs_arr, 0, CHRs_arr.Length );

#if DEF_DBG_PPU_READY_DATA_SAVE_IMG
						Bitmap 	tile_bmp	= null;
						Bitmap 	scr_bmp 	= new Bitmap( platform_data.get_screen_width_pixels(), platform_data.get_screen_height_pixels() );
	
						Graphics scr_gfx 			= Graphics.FromImage( scr_bmp );
						scr_gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
						scr_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
#endif
						
						// save CHR ids
						for( tile_n = 0; tile_n < tiles_cnt; tile_n++ )
						{
							if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
							{
								tile_id = tile_inds.get_tile( tile_n );
							}
						
							tile_offs_x = ( tile_n / platform_data.get_screen_tiles_height() ) << 1;
							tile_offs_y = ( tile_n % platform_data.get_screen_tiles_height() ) << 1;
							
							for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
							{
								if( ( tile_offs_x + ( block_n & 0x01 ) ) < platform_data.get_screen_blocks_width() && ( tile_offs_y + ( ( block_n & 0x02 ) >> 1 ) ) < platform_data.get_screen_blocks_height() )
								{
									if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
									{
										block_id = tiles.get_tile_block( tile_id, block_n );
									}
									else
									{
										block_id = exp_scr.m_scr_blocks.get_tile( ( tile_offs_x * scr_height_blocks ) + tile_offs_y + ( block_n >> 1 ) + ( ( block_n & 0x01 ) * scr_height_blocks ) );
									}
									
									for( chr_n = 0; chr_n < 4; chr_n++ )
									{
										CHR_id = tiles_data.get_block_CHR_id( tiles.blocks[ ( block_id << 2 ) + chr_n ] );
#if DEF_MMC5_CHR_DATA_COMPACTION
										if( RBtnAttrsPerCHR.Checked )
										{
											CHR_id += banks_size_arr[ banks.IndexOf( tiles ) ] >> 4;
											
											if( CHR_id >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
											{
												CHR_id -= utils.CONST_CHR_BANK_MAX_SPRITES_CNT * ( CHR_id / utils.CONST_CHR_BANK_MAX_SPRITES_CNT );
											}
										}
#endif
										// place CHRs in a row order
										CHRs_arr[ ( ( tile_offs_x << 1 ) + ( ( block_n & 0x01 ) << 1 ) + ( chr_n & 0x01 ) ) + ( ( tile_offs_y << 1 ) + ( block_n & 0x02 ) + ( chr_n >> 1 ) ) * ( platform_data.get_screen_tiles_width( true ) << 2 ) ] = ( byte )CHR_id;
									}
								}
							}
						}
						
#if DEF_DBG_PPU_READY_DATA_SAVE_IMG
						for( int i = 0; i < CHRs_arr.Length; i++ )
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
						
						if( RBtnAttrsPerBlock.Checked )
						{
							// save attributes
							for( tile_n = 0; tile_n < tiles_cnt; tile_n++ )
							{
								// place attributes in a row order
								CHRs_arr[ 960 + ( ( ( tile_n % platform_data.get_screen_tiles_height() ) << 3 ) + ( tile_n / platform_data.get_screen_tiles_height() ) ) ] = get_tile_attribute( tile_n, exp_scr, scr_height_blocks );
							}
						}
						
						exp_scr.m_PPU_scr_offset = data_offset;
						screens[ key ] = exp_scr;
						
						compress_and_save( bw, CHRs_arr, ref data_offset );
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_size + ( CheckBoxRLE.Checked ? " / " + ( CHRs_arr.Length * screens.Count ):"" ) + ") PPU-ready data array for each screen ( " + ( RBtnAttrsPerBlock.Checked ? "1024 bytes per screen: 960 bytes for CHRs and 64 bytes for attributes":"960 bytes per screen" ) + " )" );
					
					if( RBtnAttrsPerCHR.Checked )
					{
						int native_scr_width_blocks_mul2	= platform_data.get_screen_blocks_width( true ) << 1;
						int native_scr_height_blocks		= platform_data.get_screen_blocks_height( true );
						int native_scr_height_blocks_mul2	= native_scr_height_blocks << 1; 
						int native_scr_height_blocks_mul4	= native_scr_height_blocks << 2;
						
						data_offset	= 0;
						
						label = "_AttrsScr";
						bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
						
						foreach( int key in screen_sorted_keys_list ) 
						{
							exp_scr = screens[ key ];
							
							tile_inds	= screens[ key ].m_scr_tiles;
							tiles_cnt 	= tile_inds.length;
							tiles 		= screens[ key ].m_tiles;

							Array.Clear( attrs_chr, 0, attrs_chr.Length );
							
							fill_screen_attrs_per_CHR( 	attrs_chr,
														banks.IndexOf( tiles ),
														banks_size_arr, 
														tiles, 
														tile_inds, 
														true/*force_swapping*/,
														native_scr_width_blocks_mul2,
														native_scr_height_blocks_mul2,
														native_scr_height_blocks_mul4,
														exp_scr.m_scr_blocks );
							
							exp_scr.m_MMC5_scr_attrs_offset = data_offset;
							screens[ key ] = exp_scr;
							
							compress_and_save( bw, attrs_chr, ref data_offset );
						}
						
						data_size = bw.BaseStream.Length;
						bw.Close();
						
						_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_size + ( CheckBoxRLE.Checked ? " / " + ( CHRs_arr.Length * screens.Count ):"" ) + ") MMC5-ready attributes array for each screen ( " + attrs_chr.Length + " bytes per screen )" );
					}
					
					if( !CheckBoxRLE.Checked )
					{
						_sw.WriteLine( "\nScrGfxDataSize\t= " + ( RBtnAttrsPerBlock.Checked ? 1024:960 ) + "\t;\n" );
					}
				}
				
				// attributes array of 4x4 tiles ONLY and for the BIDIRECTIONAL SCROLLING mode !!!
				if( RBtnTiles4x4.Checked && RBtnModeBidirScroll.Checked )
				{
					label = "_Attrs";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( bank_n = 0; bank_n < banks.Count; bank_n++ )
					{
						tiles = banks[ bank_n ];
						
						max_tile_ind = max_tile_inds[ bank_n ];	// one based index
						
						for( tile_n = 0; tile_n < max_tile_ind; tile_n++ )
						{
							if( RBtnAttrsPerCHR.Checked )
							{
								for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
								{
									block_x = ( ( block_n & 0x01 ) == 0x01 ? 8:0 );
									block_y = ( ( block_n & 0x02 ) == 0x02 ? 2:0 );
									
									for( chr_n = 0; chr_n < 4; chr_n++ )
									{
										chr_x	= ( ( chr_n & 0x01 ) == 0x01 ? 4:0 );
										chr_y	= ( ( chr_n & 0x02 ) == 0x02 ? 1:0 );
										
										tile_attrs_arr[ block_x + block_y + chr_x + chr_y ] = get_MMC5_attribute( bank_n, tiles, tile_n, block_n, chr_n, banks_size_arr[ bank_n ] );
									}
								}
									
								if( RBtnTilesDirRows.Checked )
								{
									utils.swap_columns_rows_order<byte>( tile_attrs_arr, 4, 4 );
								}
								
								bw.Write( tile_attrs_arr );
							}
							else
							{
								attr = 0;
								
								for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
								{
									attr |= ( ( tiles_data.get_block_flags_palette( tiles.blocks[ tiles.get_tile_block( tile_n, block_n ) << 2 ] ) ) & 0x03 ) << ( block_n << 1 );
								}
								
								bw.Write( (byte)attr );
							}
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") attributes array of all exported data banks " + ( RBtnAttrsPerCHR.Checked ? "( 16 bytes per tile \\ MMC5 ), data offset = tiles offset * 4":"( 1 byte per attribute ), data offset = tiles offset / 4" ) );
				}
				
				// map
				{
					// tiles indices array for each screen
					label = "_TilesScr";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );

					foreach( var key in screen_sorted_keys_list ) 
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
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") tiles " + ( RBtnTiles2x2.Checked ? "(2x2)":"(4x4)" ) + " array for each screen ( " + ( RBtnTiles2x2.Checked ? ( scr_width_blocks * scr_height_blocks ):platform_data.get_screen_tiles_cnt() ) + " bytes per screen \\ 1 byte per tile )" );
				}
				
				// palettes
				{
					label = "_Plts";
					bw = new BinaryWriter( File.Open( m_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( bank_n = 0; bank_n < banks.Count; bank_n++ )
					{
						tiles = banks[ bank_n ];
						
						more_than_1_plt_in_bank |= ( tiles.palettes_arr.Count > 1 ) ? true:false;
						
						for( int i = 0; i < tiles.palettes_arr.Count; i++ )
						{
							utils.write_int_as_byte_arr( bw, tiles.palettes_arr[ i ].m_palette0 );
							utils.write_int_as_byte_arr( bw, tiles.palettes_arr[ i ].m_palette1 );
							utils.write_int_as_byte_arr( bw, tiles.palettes_arr[ i ].m_palette2 );
							utils.write_int_as_byte_arr( bw, tiles.palettes_arr[ i ].m_palette3 );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( m_filename + label + ":\t.incbin \"" + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") palettes array of all exported data banks ( data offset = chr_id * 16 )" );
				}
			}

			maps_arr = "\nMapsArr:\n";
			
			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				enable_comments = true;
				
				level_data = m_data_mngr.get_layout_data( level_n );
				
				level_prefix_str = CONST_FILENAME_LEVEL_PREFIX + level_n;
				
				maps_arr += "\t.word " + level_prefix_str + "_StartScr\n";
				
				check_ent_instances_cnt( level_data, level_prefix_str );
				
				_sw.WriteLine( "\n; *** " + level_prefix_str + " ***\n" );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				if( RBtnLayoutAdjacentScreenIndices.Checked )
				{
					scr_arr = level_prefix_str + "_ScrArr:";
				}
				
				cntrl_tiles_data = null;
				
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
									
									if( RBtnLayoutMatrix.Checked )
									{
										_sw.WriteLine( level_prefix_str + "_StartScr\t = " + start_scr_ind + "\n" );
										
										level_data.export_asm( _sw, level_prefix_str, ".define", ".byte", ".word", ".word", "$", false, false, false, false, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex );
									}
									else
									{
										_sw.WriteLine( level_prefix_str + "_StartScr:\t.word " + level_prefix_str + "Scr" + start_scr_ind + "\n" );
									}
								}
								
								exp_scr = screens[ scr_key ];
								
								if( cntrl_tiles_data == null )
								{
									cntrl_tiles_data = exp_scr.m_tiles;
								}
								else
								if( cntrl_tiles_data != exp_scr.m_tiles )
								{
									diff_banks_in_layout |= true;
								}
								
								_sw.WriteLine( level_prefix_str + "Scr" + common_scr_ind + ":" );
								_sw.WriteLine( "\t.byte " + banks.IndexOf( exp_scr.m_tiles ) + ( enable_comments ? "\t; chr_id":"" ) );
								
								if( CheckBoxExportMarks.Checked )
								{
									_sw.WriteLine( "\t.byte " + utils.hex( "$", scr_data.mark_adj_scr_mask ) + ( enable_comments ? "\t; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property":"" ) );
								}
								
								if( RBtnModeStaticScreen.Checked )
								{
									_sw.WriteLine( "\n\t.word " + exp_scr.m_PPU_scr_offset + ( enable_comments ? "\t; " + m_filename + "_PPUScr" + ( ( RBtnAttrsPerCHR.Checked && !CheckBoxRLE.Checked ) ? "\\" + m_filename + "_AttrsScr":"" ) + " offset":"" ) );
									
									if( RBtnAttrsPerCHR.Checked && CheckBoxRLE.Checked )
									{
										_sw.WriteLine( "\t.word " + exp_scr.m_MMC5_scr_attrs_offset + ( enable_comments ? "\t; " + m_filename + "_AttrsScr offset":"" ) );										
									}
								}
								
								_sw.WriteLine( "\n\t.byte " + exp_scr.m_scr_ind + ( enable_comments ? "\t; screen index":"" ) );
								
								_sw.WriteLine( "" );
								
								if( RBtnLayoutAdjacentScreens.Checked )
								{
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, -1,	 0	) + ( enable_comments ? "\t; left adjacent screen":"" ) );
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  0,	-1	) + ( enable_comments ? "\t; up adjacent screen":"" ) );
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  1,	 0	) + ( enable_comments ? "\t; right adjacent screen":"" ) );
									_sw.WriteLine( "\t.word " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  0,  1	) + ( enable_comments ? "\t; down adjacent screen\n":"\n" ) );
								}
								else
								if( RBtnLayoutAdjacentScreenIndices.Checked )
								{
									if( enable_comments )
									{
										_sw.WriteLine( "; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )" );
										_sw.WriteLine( "; use the " + level_prefix_str + "_ScrArr array to get a screen description by adjacent screen index" );
									}
									
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, -1,	 0	) + ( enable_comments ? "\t; left adjacent screen index":"" ) );
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  0,	-1	) + ( enable_comments ? "\t; up adjacent screen index":"" ) );
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  1,	 0	) + ( enable_comments ? "\t; right adjacent screen index":"" ) );
									_sw.WriteLine( "\t.byte " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  0,  1	) + ( enable_comments ? "\t; down adjacent screen index\n":"\n" ) );
									
									scr_arr += "\n\t.word " + level_prefix_str + "Scr" + common_scr_ind;
								}
								
								if( CheckBoxExportEntities.Checked )
								{
									scr_data.export_entities_asm( _sw, ref ents_cnt, level_prefix_str + "Scr" + common_scr_ind + "EntsArr", ".byte", ".word", ".word", "$", RBtnEntityCoordScreen.Checked, scr_n_X, scr_n_Y, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex, enable_comments );
									
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
					_sw.WriteLine( ".define " + level_prefix_str + "_EntInstCnt\t" + level_data.get_num_ent_instances() + "\t; number of entities instances\n" );
				}
			}
			
			// save MapsArr
			{
				_sw.WriteLine( maps_arr );
			}
			
			foreach( var key in screens.Keys ) { screens[ key ].destroy(); }
			
			if( diff_banks_in_layout && more_than_1_plt_in_bank )
			{
				MainForm.message_box( "The exported layout contains more than one CHR bank data and at least one CHR bank has more than one palette!\n\nThis can make it difficult to get correct palette data in your code!", "Data Export", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			
		}

		private byte get_tile_attribute( int _tile_n, exp_screen_data _exp_scr, int _scr_height_blocks )
		{
			return get_tile_attribute( _tile_n, 
										delegate( int _tile_id ) { return _exp_scr.m_scr_tiles.get_tile( _tile_id ); }, 
										delegate( int _tile_id ) { return _exp_scr.m_scr_blocks.get_tile( _tile_id ); }, 
										_exp_scr.m_tiles, 
										_scr_height_blocks, 
										1 );
		}
		
		private byte get_tile_attribute( 	int 				_tile_n, 
											Func< int, ushort > _tiles_arr_get_tile, 
											Func< int, ushort > _blocks_arr_get_tile, 
											tiles_data 			_data, 
											int 				_scr_height_blocks, 
											int 				_Y_scr_cnt )
		{
			int attr = 0;
			
			int tile_id;
			int block_n;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				tile_id = _tiles_arr_get_tile( _tile_n );
				
				for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
				{
					attr |= ( ( tiles_data.get_block_flags_palette( _data.blocks[ _data.get_tile_block( tile_id, block_n ) << 2 ] ) ) & 0x03 ) << ( block_n << 1 );
				}
			}
			else
			{
				int half_tile_x = platform_data.get_half_tile_x();
				int half_tile_y = platform_data.get_half_tile_y();
				
				block_n = _tile_n << 1;
				
				if( RBtnModeMultidirScroll.Checked )
				{
					block_n += ( _Y_scr_cnt * _scr_height_blocks ) * ( block_n / ( ( platform_data.get_screen_tiles_height() << 1 ) * _Y_scr_cnt ) );
					
					if( half_tile_y >= 0 )
					{
						block_n -= _tile_n / platform_data.get_screen_tiles_height();
					}
				}
				else
				{
					if( half_tile_y >= 0 )
					{
						block_n -= _tile_n / platform_data.get_screen_tiles_height();
					}
					
					block_n += _scr_height_blocks * ( _tile_n / platform_data.get_screen_tiles_height() );
				}
				
				attr |= ( ( tiles_data.get_block_flags_palette( _data.blocks[ _blocks_arr_get_tile( block_n ) << 2 ] ) ) & 0x03 ) << 0;
				
				if( ( half_tile_x < 0 ) || ( half_tile_x >= 0 && ( _tile_n / platform_data.get_screen_tiles_height() ) != half_tile_x ) )
				{
					attr |= ( ( tiles_data.get_block_flags_palette( _data.blocks[ _blocks_arr_get_tile( ( block_n + _scr_height_blocks * _Y_scr_cnt ) ) << 2 ] ) ) 	& 0x03 ) << 2;
				}
				
				if( ( half_tile_y < 0 ) || ( half_tile_y >= 0 && ( ( _tile_n + 1 ) % ( half_tile_y + 1 ) ) != 0 ) )
				{
					attr |= ( ( tiles_data.get_block_flags_palette( _data.blocks[ _blocks_arr_get_tile( block_n + 1 ) << 2 ] ) ) & 0x03 ) << 4;
					
					if( ( half_tile_x < 0 ) || ( half_tile_x >= 0 && ( _tile_n / platform_data.get_screen_tiles_height() ) != half_tile_x ) )
					{
						attr |= ( ( tiles_data.get_block_flags_palette( _data.blocks[ _blocks_arr_get_tile( ( block_n + _scr_height_blocks * _Y_scr_cnt + 1 ) ) << 2 ] ) ) 	& 0x03 ) << 6;
					}
				}
			}
			
			return ( byte )attr;
		}
		
		private byte get_MMC5_attribute( int _bank_n, tiles_data _tiles, int _tile_id, int _block_n, int _chr_n, int _CHR_data_size )
		{
			return get_MMC5_attribute( _bank_n, _tiles, _tiles.get_tile_block( _tile_id, _block_n ), _chr_n, _CHR_data_size );
		}

		private byte get_MMC5_attribute( int _bank_n, tiles_data _tiles, int _block_n, int _chr_n, int _CHR_data_size )
		{
			uint block_data = _tiles.blocks[ ( _block_n << 2 ) + _chr_n ];
			
			return get_screen_attribute( _bank_n, block_data, _CHR_data_size );
		}

		private byte get_screen_attribute( int _bank_n, uint _block_data, int _CHR_data_size )
		{
#if DEF_MMC5_CHR_DATA_COMPACTION
			return ( byte )( ( ( ( tiles_data.get_block_flags_palette( _block_data ) ) & 0x03 ) << 6 ) | ( ( int )NumericUpDownCHRBankIndex.Value + ( ( ( _CHR_data_size >> 4 ) + tiles_data.get_block_CHR_id( _block_data ) ) / utils.CONST_CHR_BANK_MAX_SPRITES_CNT ) ) );
#else
			return ( byte )( ( ( ( tiles_data.get_block_flags_palette( _block_data ) ) & 0x03 ) << 6 ) | ( ( int )NumericUpDownCHRBankIndex.Value + _bank_n ) );
#endif			
		}

		private void compress_and_save( BinaryWriter _bw, byte[] _data, ref int _data_offset )
		{
			try
			{
				if( CheckBoxRLE.Checked )
				{
					byte[] rle_data_arr	= null;
	
					int rle_data_size = utils.RLE8( _data, ref rle_data_arr );
					
					if( rle_data_size < 0 )
					{
						throw new System.Exception( "Can't compress an empty data!" );
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

		private void compress_and_save( BinaryWriter _bw, byte[] _data )
		{
			int offset = 0;
			
			compress_and_save( _bw, _data, ref offset );
		}
		
		private void fill_screen_attrs_per_CHR( byte[] 			_attrs_chr,
												int 			_bank_ind, 
												int[] 			_banks_size_arr, 
												tiles_data 		_tiles, 
												screen_data		_tile_inds, 
												bool 			_force_swapping,
												int 			_scr_width_blocks_mul2, 
												int 			_scr_height_blocks_mul2, 
												int 			_scr_height_blocks_mul4,
												screen_data		_block_inds )
		{
			ushort tile_id;
			
			int tile_x;
			int tile_y;
			int block_x;
			int block_y;
			int chr_x;
			int chr_y;
			int tile_offs_x;
			int tile_offs_y;
			
			int tile_n;
			int block_n;
			int chr_n;
			int tiles_cnt = _tile_inds.length;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				// MMC5 extended attributes data
				for( tile_n = 0; tile_n < tiles_cnt; tile_n++ )
				{
					tile_id = _tile_inds.get_tile( tile_n );
	
					tile_offs_x = ( tile_n / platform_data.get_screen_tiles_height() ) << 1;
					tile_offs_y = ( tile_n % platform_data.get_screen_tiles_height() ) << 1;
	
					tile_x	= ( ( tile_offs_x << 1 ) * _scr_height_blocks_mul2 );
					tile_y	= ( tile_offs_y << 1 );
					
					for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
					{
						if( ( tile_offs_x + ( block_n & 0x01 ) ) < platform_data.get_screen_blocks_width() && ( tile_offs_y + ( ( block_n & 0x02 ) >> 1 ) ) < platform_data.get_screen_blocks_height() )
						{
							block_x = ( ( block_n & 0x01 ) == 0x01 ? _scr_height_blocks_mul4:0 );
							block_y = block_n & 0x02;
							
							for( chr_n = 0; chr_n < 4; chr_n++ )
							{
								chr_x	= ( ( chr_n & 0x01 ) == 0x01 ? _scr_height_blocks_mul2:0 );
								chr_y	= ( chr_n & 0x02 ) >> 1;
								
								// column order by default
								_attrs_chr[ tile_x + tile_y + block_x + block_y + chr_x + chr_y ] = get_MMC5_attribute( _bank_ind, _tiles, tile_id, block_n, chr_n, _banks_size_arr[ _bank_ind ] );
							}
						}
					}
				}
			}
			else
			{
				for( block_n = 0; block_n < platform_data.get_screen_blocks_cnt(); block_n++ )
				{
					block_x = ( ( block_n / platform_data.get_screen_blocks_height() ) << 1 ) * _scr_height_blocks_mul2;
					block_y = ( block_n % platform_data.get_screen_blocks_height() ) << 1;

					for( chr_n = 0; chr_n < 4; chr_n++ )
					{
						chr_x	= ( ( chr_n & 0x01 ) == 0x01 ? _scr_height_blocks_mul2:0 );
						chr_y	= ( chr_n & 0x02 ) >> 1;
						
						// column order by default
						_attrs_chr[ block_x + block_y + chr_x + chr_y ] = get_MMC5_attribute( _bank_ind, _tiles, _block_inds.get_tile( block_n ), chr_n, _banks_size_arr[ _bank_ind ] );
					}
				}
			}
			
			if( RBtnTilesDirRows.Checked || _force_swapping )
			{
				utils.swap_columns_rows_order<byte>( _attrs_chr, _scr_width_blocks_mul2, _scr_height_blocks_mul2 );
			}
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
		
		private void save_multidir_scroll_mode( StreamWriter _sw )
		{
			BinaryWriter 	bw = null;
			
			layout_data level_data = null;
			
			byte[]	map_data_arr		= null;
			byte[]	map_tiles_arr		= null;
			byte[]	map_blocks_arr		= null;
			byte[]	blocks_arr			= null;
			byte[]	block_props_arr		= null;
			byte[]	tile_attrs_arr		= null; 

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
			int bank_n				= 0;
			int tile_offs_x			= 0;
			int tile_offs_y			= 0;
			int max_tile_ind 		= 0;
			int blocks_props_size	= 0;
			int attr				= 0;
			ushort tile_id			= 0;
			byte block_id			= 0;
			uint block_data			= 0;
			
			layout_screen_data	scr_data;
			
			long 	data_size 	= 0;
			string 	label 		= null;
			string 	palette_str	= null;
			
			string level_prefix_str	= null;
			string bank_prefix_str	= null;
			string maps_CHRs_arr	= "\nMapsCHRBanks:\n";
			
			int scr_width_blocks 	= platform_data.get_screen_blocks_width();
			int scr_height_blocks 	= platform_data.get_screen_blocks_height();
			
			tiles_data tiles = null;

			_sw.WriteLine( "\n; *** GLOBAL DATA ***\n" );
			
			for( bank_n = 0; bank_n < n_banks; bank_n++ )
			{
				tiles = scr_tiles_data[ bank_n ];
				
				bank_prefix_str = CONST_FILENAME_BANK_PREFIX + bank_n;
				
				// write CHR banks data
				label = bank_prefix_str + "_CHR";
				bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
				{
					tiles.export_CHR( bw );
				}
				data_size = bw.BaseStream.Length;
				bw.Close();
				
				_sw.WriteLine( "; " + label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ")" );

				if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
				{
					max_tile_ind = tiles.get_first_free_tile_id( false );
				}
				else
				{
					max_tile_ind = tiles.get_first_free_block_id( false );
				}
				
				if( RBtnTiles4x4.Checked )
				{
					// write tiles
					label = bank_prefix_str + "_Tiles";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( int i = 0; i < max_tile_ind; i++ )
					{
						bw.Write( tiles_data.tile_convert_ulong_to_uint_reverse( tiles.tiles[ i ] ) );
					}
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") (4x4) 4 block indices per tile ( left to right, up to down )" );
				
					// attributes array for 4x4 tiles only
					label = bank_prefix_str + "_Attrs";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					tile_attrs_arr = new byte[ max_tile_ind ];
					
					for( tile_n = 0; tile_n < max_tile_ind; tile_n++ )
					{
						attr = 0;
						
						for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
						{
							attr |= ( ( tiles_data.get_block_flags_palette( tiles.blocks[ tiles.get_tile_block( tile_n, block_n ) << 2 ] ) ) & 0x03 ) << ( block_n << 1 );
						}
						
						tile_attrs_arr[ tile_n ] = (byte)attr;
					}
					
					bw.Write( tile_attrs_arr );
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") attributes array ( 1 byte per tile )" );
				}

				// blocks and properties
				{
					if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
					{
						blocks_props_size = ( 1 + utils.get_ulong_arr_max_val( tiles.tiles, max_tile_ind ) ) << 2;
					}
					else
					{
						blocks_props_size = tiles.get_first_free_block_id( false ) << 2;
					}
					
					block_props_arr = new byte[ RBtnPropPerBlock.Checked ? ( blocks_props_size >> 2 ):blocks_props_size ];
					Array.Clear( block_props_arr, 0, block_props_arr.Length );
					
					blocks_arr = new byte[ blocks_props_size ];
					Array.Clear( blocks_arr, 0, blocks_props_size );
					
					for( block_n = 0; block_n < blocks_props_size; block_n++ )
					{
						block_data = tiles.blocks[ block_n ];
							
						blocks_arr[ block_n ]	= (byte)tiles_data.get_block_CHR_id( block_data );
						
						if( RBtnPropPerBlock.Checked && ( block_n % 4 ) != 0 )
						{
							continue;
						}
						
						block_props_arr[ RBtnPropPerBlock.Checked ? ( ( block_n + 1 ) >> 2 ):block_n ]	= (byte)tiles_data.get_block_flags_obj_id( block_data );
					}
					
					// save blocks
					label = bank_prefix_str + "_Blocks";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					bw.Write( blocks_arr );
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") blocks data ( 4 CHR indices per block; left to right, up to down )" );
					
					// save properties
					label = bank_prefix_str + "_Props";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					bw.Write( block_props_arr );
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") block properties array ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )\n" );
				}
				
				palette_str = bank_prefix_str + "_Palette:";
				
				for( int i = 0; i < tiles.palettes_arr.Count; i++ )
				{
					palette_str += "\n\t\t.byte ";
					
					fill_palette_str( tiles.palettes_arr[ i ].m_palette0, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette1, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette2, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette3, ref palette_str, true );
				}
				
				_sw.WriteLine( palette_str + "\n" );
			}
			
			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				level_data = m_data_mngr.get_layout_data( level_n );
				
				level_prefix_str = CONST_FILENAME_LEVEL_PREFIX + level_n;
				
				check_ent_instances_cnt( level_data, level_prefix_str );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				n_Y_tiles = n_scr_Y * ( RBtnTiles2x2.Checked ? platform_data.get_screen_blocks_height():platform_data.get_screen_tiles_height() );
				
				// game level tilemap analysing
				{
					map_tiles_arr = new byte[ n_scr_X * n_scr_Y * platform_data.get_screen_tiles_cnt() ];
					
					Array.Clear( map_tiles_arr, 0, map_tiles_arr.Length );
				}
				
				if( RBtnTiles2x2.Checked )
				{
					map_blocks_arr = new byte[ n_Y_tiles * n_scr_X * platform_data.get_screen_blocks_width() ];
					
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
							
							if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
							{
								for( tile_n = 0; tile_n < platform_data.get_screen_tiles_cnt(); tile_n++ )
								{
									tile_offs_x = ( tile_n % platform_data.get_screen_tiles_width() );
									tile_offs_y = ( tile_n / platform_data.get_screen_tiles_width() );
									
									tile_id = tiles.get_screen_tile( scr_ind, tile_n );
									
									if( RBtnTiles2x2.Checked )
									{
										map_tiles_arr[ scr_n_X * ( ( n_scr_Y * platform_data.get_screen_tiles_height() ) * platform_data.get_screen_tiles_width() ) + ( scr_n_Y * platform_data.get_screen_tiles_height() ) + ( tile_offs_x * ( n_scr_Y * platform_data.get_screen_tiles_height() ) ) + tile_offs_y ] = ( byte )tile_id;
										
										tile_offs_x <<= 1;
										tile_offs_y <<= 1;
										
										// make a list of 2x2 tiles in the current map
										for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
										{
											block_id = ( byte )tiles.get_tile_block( tile_id, block_n );

											if( ( tile_offs_x + ( block_n & 0x01 ) ) < platform_data.get_screen_blocks_width() && ( tile_offs_y + ( ( block_n & 0x02 ) >> 1 ) ) < platform_data.get_screen_blocks_height() )
											{
												map_blocks_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + ( ( block_n & 0x01 ) == 0x01 ? n_Y_tiles:0 ) + tile_offs_y + ( ( block_n & 0x02 ) == 0x02 ? 1:0 ) ] = block_id;
											}
										}
									}
									else
									{
										map_tiles_arr[ scr_n_X * ( n_Y_tiles * platform_data.get_screen_tiles_width() ) + ( scr_n_Y * platform_data.get_screen_tiles_height() ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = ( byte )tile_id;
									}
								}
							}
							else
							{
								// make a list of 2x2 tiles of the current map
								for( block_n = 0; block_n < platform_data.get_screen_blocks_cnt(); block_n++ )
								{
									tile_offs_x = ( block_n % platform_data.get_screen_blocks_width() );
									tile_offs_y = ( block_n / platform_data.get_screen_blocks_width() );
									
									map_blocks_arr[ scr_n_X * ( n_Y_tiles * scr_width_blocks ) + ( scr_n_Y * scr_height_blocks ) + ( tile_offs_x * n_Y_tiles ) + tile_offs_y ] = ( byte )tiles.get_screen_tile( scr_ind, block_n );
								}
							}
						}
					}
				}
				
				if( chk_bank_ind < 0 )
				{
					continue;
				}

				// write collected data
				_sw.WriteLine( "\n; *** " + level_prefix_str + " ***\n" );
				
				maps_CHRs_arr += "\t.byte " + chk_bank_ind + "\n";
				
				// write map
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
				
				compress_and_save( bw, map_data_arr );
				
				data_size = bw.BaseStream.Length;
				bw.Close();
				
				_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_size + ( CheckBoxRLE.Checked ? " / " + map_data_arr.Length:"" ) + ") game level " + ( RBtnTiles4x4.Checked ? "tiles (4x4)":"blocks (2x2)" ) + " array" );
				
				// tiles lookup table
				label = level_prefix_str + "_MapTbl";
				bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
				{
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
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") lookup table for fast calculation of tile addresses " + ( RBtnTilesDirColumns.Checked ? "columns by X coordinate":"rows by Y coordinate" ) + " ( 16 bit offset per " + ( RBtnTilesDirColumns.Checked ? "column":"row" ) + " of tiles )" );
				}
				
				if( RBtnTiles2x2.Checked )
				{
					// tiles 2x2
					// write attributes map
					label = level_prefix_str + "_AttrsMap";
					bw = new BinaryWriter( File.Open( m_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );

					for( tile_n = 0; tile_n < map_tiles_arr.Length; tile_n++ )
					{
						map_tiles_arr[ tile_n ] = get_tile_attribute( 	tile_n, 
																		delegate( int _tile_id ) { return map_tiles_arr[ _tile_id ]; },
																		delegate( int _tile_id ) { return map_blocks_arr[ _tile_id ]; },
																		scr_tiles_data[ chk_bank_ind ],
																		scr_height_blocks, 
																		n_scr_Y );
					}
					
					if( RBtnTilesDirRows.Checked )
					{
						utils.swap_columns_rows_order<byte>( map_tiles_arr, n_scr_X * platform_data.get_screen_tiles_width(), n_scr_Y * platform_data.get_screen_tiles_height() );
					}
					
					compress_and_save( bw, map_tiles_arr );
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					_sw.WriteLine( label + ":\t.incbin \"" + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_size + ( CheckBoxRLE.Checked ? " / " + map_tiles_arr.Length:"" ) + ") map of attributes ( 1 byte per attribute )" );
				}
				
				int start_scr_ind = level_data.get_start_screen_ind();
				
				if( start_scr_ind < 0 )
				{
					MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A zero screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
					
					start_scr_ind = 0;
				}
				
				_sw.WriteLine( "\n" + level_prefix_str + "_StartScr\t=\t" + start_scr_ind + "\t; start screen" );

				_sw.WriteLine( level_prefix_str + "_WTilesCnt\t=\t" + get_tiles_cnt_width( n_scr_X ) + "\t; number of level tiles in width" );
				_sw.WriteLine( level_prefix_str + "_HTilesCnt\t=\t" + get_tiles_cnt_height( n_scr_Y ) + "\t; number of level tiles in height" );
				
				_sw.WriteLine( level_prefix_str + "_TilesCnt\t=\t" + ( ( RBtnTiles2x2.Checked ? map_blocks_arr:map_tiles_arr ).Max() + 1 ) + "\t; map tiles count" );

				if( CheckBoxExportEntities.Checked )
				{
					level_data.export_asm( _sw, level_prefix_str, ".define", ".byte", ".word", ".word", "$", true, CheckBoxExportMarks.Checked, CheckBoxExportEntities.Checked, RBtnEntityCoordScreen.Checked, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex );
				}
				else
				{
					_sw.WriteLine( level_prefix_str + "_WScrCnt\t=\t" + n_scr_X + "\t; number of screens in width" );
					_sw.WriteLine( level_prefix_str + "_HScrCnt\t=\t" + n_scr_Y + "\t; number of screens in height\n" );
				}
				
				map_data_arr 	= null;
				map_tiles_arr 	= null;
				map_blocks_arr	= null;
				tile_attrs_arr	= null;
			}
			
			// maps -> CHR bank
			{
				_sw.WriteLine( maps_CHRs_arr );
			}
		}
		
		private int get_tiles_cnt_width( int _scr_cnt_x )
		{
			return RBtnTiles2x2.Checked ? _scr_cnt_x * platform_data.get_screen_blocks_width():_scr_cnt_x * platform_data.get_screen_tiles_width();
		}

		private int get_tiles_cnt_height( int _scr_cnt_y )
		{
			return RBtnTiles2x2.Checked ? _scr_cnt_y * platform_data.get_screen_blocks_height():_scr_cnt_y * platform_data.get_screen_tiles_height();
		}
		
		private void fill_palette_str( int[] _plt, ref string _str, bool _end )
		{
			for( int j = 0; j < utils.CONST_PALETTE_SMALL_NUM_COLORS; j++ )
			{
				_str += String.Format( "${0:X2}", _plt[ j ] ) + ( !( _end && j == 3 ) ? ", ":"" );
			}
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