﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 13.09.2018
 * Time: 17:59
 */
 
//#define DEF_DBG_PPU_READY_DATA_SAVE_IMG
 
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
	/// Description of exporter_asm.
	/// </summary>
	/// 

	public partial class exporter_asm : Form
	{
		private readonly data_sets_manager m_data_mngr;
		
		private string	m_filename				= null;
		private string	m_path					= null;
		private string 	m_path_filename_ext		= null;
		private string 	m_data_path_filename	= null;
		
		private const string	CONST_FILENAME_LEVEL_PREFIX		= "Lev";
		private const string	CONST_FILENAME_BANK_PREFIX		= "Bank";
		private const string	CONST_BIN_EXT					= ".bin";
		
		private const string	CONST_DATA_ALIGN				= "\t.align 2";
		private const string	CONST_C_DATA_PREFIX				= "mpd_";
		
		private const string	CONST_C_STRUCT_ARR_PU16			= CONST_C_DATA_PREFIX + "ARR_PU16";
		private const string	CONST_C_STRUCT_ARR_U8			= CONST_C_DATA_PREFIX + "ARR_U8";
		private const string	CONST_C_STRUCT_ARR_U16			= CONST_C_DATA_PREFIX + "ARR_U16";
		private const string	CONST_C_STRUCT_ARR_U32			= CONST_C_DATA_PREFIX + "ARR_U32";
		private const string	CONST_C_STRUCT_ARR_SCR			= CONST_C_DATA_PREFIX + "ARR_SCREEN";
		
		private const string	CONST_BIN_FILE_DIR				= "data";
		private const string	CONST_S_FILE_DIR				= "src";
		private const string	CONST_H_FILE_DIR				= "inc";
		
		private const int		CONST_BIDIR_MAP_SCREEN_MAX_CNT	= 255;	// 1...255 [zero-based index 0..254; 255 - empty screen]
		
		private int m_VDP_ready_scr_data_size 	= -1;

		private StreamWriter	m_C_writer	= null;
		
		private struct exp_scr_data
		{
			public int 			m_scr_ind;
			
			public tiles_data 	m_tiles;
			
			public screen_data	m_scr_tiles;
			public screen_data	m_scr_blocks;
			
			public int			m_tiles_offset;
			public int			m_blocks_offset;
			
			public int			m_VDP_scr_offset;
			
			public static int	_tiles_offset;
			public static int	_blocks_offset;
			
			public exp_scr_data( int _scr_ind, tiles_data _tiles )
			{
				m_scr_ind		= _scr_ind;
				
				m_tiles 		= _tiles;
				
				m_scr_tiles		= new screen_data( data_sets_manager.e_screen_data_type.Tiles4x4 );
				m_scr_blocks	= new screen_data( data_sets_manager.e_screen_data_type.Blocks2x2 );
				
				m_tiles_offset	= _tiles_offset;
				m_blocks_offset	= _blocks_offset;
				
				_tiles_offset 	+= m_scr_tiles.length;
				_blocks_offset 	+= m_scr_blocks.length;
				
				m_VDP_scr_offset = 0;
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
			
			ComboBoxInFrontOfSpritesProp.SelectedIndex = 0;
			
			CBoxEntSortingType.SelectedIndex = 0;
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
			
			update_desc();
		}
		
		private void RBtnModeScreenToScreenChanged( object sender, EventArgs e )
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			RBtnLayoutAdjacentScreens.Checked	= true;
			
			CheckBoxExportMarks.Enabled		= true;
			RBtnEntityCoordScreen.Checked	= true;
			
			CheckBoxRLE.Checked = CheckBoxRLE.Enabled = false;
			
			update_desc();
		}

		private void RBtnModeStaticScreensChanged( object sender, EventArgs e )
		{
			RBtnLayoutMatrix.Enabled = RBtnLayoutAdjacentScreenIndices.Enabled = RBtnLayoutAdjacentScreens.Enabled = true;
			RBtnLayoutAdjacentScreens.Checked	= true;
			
			CheckBoxExportMarks.Enabled		= true;
			RBtnEntityCoordScreen.Checked	= true;
			
			CheckBoxRLE.Enabled = true;
			
			update_desc();
		}

		private void OnParamChanged( object sender, EventArgs e )
		{
			update_desc();
		}
		
		private string get_alignment_data()
		{
			if( CheckBoxExportSGDKData.Checked )
			{
				return CONST_DATA_ALIGN;
			}
			
			return null;
		}
		
		private void write_alignment_data( StreamWriter _sw )
		{
			string align_data = get_alignment_data();
			
			if( align_data != null )
			{
				_sw.WriteLine( align_data );
			}
		}
		
		private void save_global_data( ref string _buff, string _data_name, long _data_len )
		{
			if( CheckBoxExportSGDKData.Checked )
			{
				_buff += CONST_DATA_ALIGN + "\n";
				_buff += "\t.global " + CONST_C_DATA_PREFIX + _data_name + "\n";
				_buff += CONST_C_DATA_PREFIX + _data_name + ":\n";
				_buff += "\tdc.w " + _data_len + "\n";
				_buff += "\tdc.l " + _data_name + "\n\n";
			}
		}
		
		private string get_data_subdir()
		{
			if( CheckBoxExportSGDKData.Checked )
			{
				return CONST_BIN_FILE_DIR + Path.DirectorySeparatorChar;
			}
			
			return "";
		}
		
		private string get_h_file_subdir()
		{
			if( CheckBoxExportSGDKData.Checked )
			{
				return CONST_H_FILE_DIR + Path.DirectorySeparatorChar;
			}
			
			return "";
		}
		
		private string get_s_file_subdir()
		{
			if( CheckBoxExportSGDKData.Checked )
			{
				return CONST_S_FILE_DIR + Path.DirectorySeparatorChar;
			}
			
			return "";
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
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_RLE_COMPRESSION;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_DATA_ORDER;
			
			if( RBtnTilesDirColumns.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_SMD_DATA_ORDER_COLS;
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
				RichTextBoxExportDesc.Text += string.Format( strings.CONST_STR_EXP_MODE_BIDIR, CONST_BIDIR_MAP_SCREEN_MAX_CNT );
			}
			else
			{
				RichTextBoxExportDesc.Text += string.Format( strings.CONST_STR_EXP_MODE_STAT_SCR, CONST_BIDIR_MAP_SCREEN_MAX_CNT ).Replace( "<data>", "VDP-ready - " + m_VDP_ready_scr_data_size.ToString() );
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
				
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_SORTING;
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_ENT_SORT_TYPES[ CBoxEntSortingType.SelectedIndex ];
			}
			else
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_NO_ENTITIES;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_CHR_OFFSET;
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_SMD_PROP_IN_FRONT_OF_SPRITES;
			
			if( CheckBoxExportSGDKData.Checked )
			{
				RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_SMD_SGDK_DATA;
			}
			
			RichTextBoxExportDesc.Text += strings.CONST_STR_EXP_WARNING;
		}

		private void BtnCancelClick( object sender, System.EventArgs e )
		{
			this.Close();
		}
		
		public DialogResult show_window( string _full_path )
		{
			m_path_filename_ext = _full_path;
			
			if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Blocks2x2 )
			{
				RBtnTiles2x2.Checked = true;
				RBtnTiles2x2.Enabled = RBtnTiles4x4.Enabled = false;
			}
			else
			{
				RBtnTiles2x2.Enabled = RBtnTiles4x4.Enabled = true;
			}
			
			m_VDP_ready_scr_data_size = platform_data.get_screen_blocks_cnt() << 3;
			update_desc();
			
			return ShowDialog();
		}
		
		private void BtnOkClick( object sender, System.EventArgs e )
		{
			this.Close();

			m_filename				= Path.GetFileNameWithoutExtension( m_path_filename_ext );
			m_path					= Path.GetDirectoryName( m_path_filename_ext ) + Path.DirectorySeparatorChar;
			m_data_path_filename	= m_path + get_data_subdir() + m_filename;
			
			StreamWriter sw = null;
			
			try
			{
				sw = new StreamWriter( m_path_filename_ext );
				
				utils.write_title( sw );

				if( CheckBoxExportSGDKData.Checked )
				{
					// create output directories
					Directory.CreateDirectory( m_path + get_data_subdir() );
					Directory.CreateDirectory( m_path + get_h_file_subdir() );
					Directory.CreateDirectory( m_path + get_s_file_subdir() );
					
					m_C_writer = new StreamWriter( m_path + get_h_file_subdir() + Path.GetFileNameWithoutExtension( m_path_filename_ext ) + ".h" );
					{
						utils.write_title( m_C_writer, true );
						
						m_C_writer.WriteLine( "#ifndef _" + m_filename.ToUpper() + "_H_" );
						m_C_writer.WriteLine( "#define _" + m_filename.ToUpper() + "_H_\n" );
					}
					
					sw.WriteLine( ".section\t.rodata_binf\t; 'FAR' data are located in the end of the ROM and can require bank switch mechanism if the ROM is larger than 4MB\n" );
				}
				
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
					
					sw.WriteLine( ";\t- " + ( CheckBoxExportEntities.Checked ? "entities " + ( RBtnEntityCoordScreen.Checked ? "(screen coordinates)":"(map coordinates)" ) + ( CBoxEntSortingType.SelectedIndex == 0 ? "/no sorting":( CBoxEntSortingType.SelectedIndex == 1 ) ? "/left-right":"/bottom-top" ):"no entities" ) );
					
					if( m_C_writer != null )
					{
						sw.WriteLine( ";\t- generate C header file" );
					}
					
					sw.WriteLine( "\n" );
				}
				
				write_map_flags( CheckBoxExportSGDKData.Checked ? m_C_writer:sw );
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "\n// see the '" + m_filename + ".s' file for details" );
				}
				
				if( CheckBoxExportEntities.Checked )
				{
					write_alignment_data( sw );
					m_data_mngr.export_base_entity_asm( sw, "dc.w", "$" );
				}
				
				if( RBtnModeMultidirScroll.Checked )
				{
					save_multidir_scroll_mode( sw );
				}
				else
				{
					save_single_screen_mode( sw );
				}
				
				sw.Close();
				
				if( CheckBoxExportSGDKData.Checked )
				{
					m_C_writer.WriteLine( "\n#endif // _" + m_filename.ToUpper() + "_H_" );
					m_C_writer.Close();
					
					// move and rename the main ASM file
					{
						string s_file			= m_path + m_filename + ".s";
						string new_file_path	= m_path + get_s_file_subdir() + m_filename + ".s";
						
						if( File.Exists( s_file ) )
						{
							File.Delete( s_file );
						}

						if( File.Exists( new_file_path ) )
						{
							File.Delete( new_file_path );
						}
						
						File.Move( m_path_filename_ext, new_file_path );
						
						// load the ASM file and replace some data
						{
							string file_data = File.ReadAllText( new_file_path );
							
							file_data = file_data.Replace( ';', '|' ).Replace( '\\', '/' ).Replace( "$", "0x" ).Replace( "incbin", ".incbin" );
							
							File.WriteAllText( new_file_path, file_data );
						}
					}
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
					sw.Dispose();
				}
				
				if( m_C_writer != null )
				{
					m_C_writer.Dispose();
					m_C_writer = null;
				}
			}
		}

		private void write_map_flags( StreamWriter _sw )
		{
			string c_def 		= CheckBoxExportSGDKData.Checked ? "#define ":"";
			string c_comment	= CheckBoxExportSGDKData.Checked ? "//":";";
			string c_hex_pref	= CheckBoxExportSGDKData.Checked ? "0x":"$";
			string c_def_eq		= CheckBoxExportSGDKData.Checked ? "":"= ";
			
			_sw.WriteLine( c_def + "MAP_DATA_MAGIC " + c_def_eq + utils.hex( c_hex_pref, ( RBtnTiles2x2.Checked ? 1:2 ) |
			                                              		( CheckBoxRLE.Checked ? 4:0 ) |
			                                              		( RBtnTilesDirColumns.Checked ? 8:16 ) |
			                                              		( RBtnModeMultidirScroll.Checked ? 32:RBtnModeBidirScroll.Checked ? 64:128 ) | 
			                                              		( CheckBoxExportEntities.Checked ? 256:0 ) |
			                                              		( CheckBoxExportEntities.Checked ? ( RBtnEntityCoordScreen.Checked ? 512:1024 ):0 ) |
			                                              		( RBtnLayoutAdjacentScreens.Checked ? 2048:RBtnLayoutAdjacentScreenIndices.Checked ? 4096:8192 ) |
			                                              		( CheckBoxExportMarks.Checked ? 16384:0 ) |
			                                              		( RBtnPropPerBlock.Checked ? 32768:65536 ) |
																( CBoxEntSortingType.SelectedIndex == 0 ? 131072:0 ) |
																( CBoxEntSortingType.SelectedIndex == 1 ? 262144:0 ) |
																( CBoxEntSortingType.SelectedIndex == 2 ? 524288:0 ) ) );
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
			_sw.WriteLine( c_def + "MAP_FLAG_ENT_SORTING_OFF          " + c_def_eq + utils.hex( c_hex_pref, 131072 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_ENT_SORTING_LT_RT        " + c_def_eq + utils.hex( c_hex_pref, 262144 ) );
			_sw.WriteLine( c_def + "MAP_FLAG_ENT_SORTING_BTM_TOP      " + c_def_eq + utils.hex( c_hex_pref, 524288 ) );
			
			_sw.WriteLine( "\n" + c_def + "MAPS_CNT\t" + c_def_eq + m_data_mngr.layouts_data_cnt );
			
			_sw.WriteLine( "\n" + c_def + "CHRS_OFFSET\t" + c_def_eq + get_CHR_offset() + "\t" + c_comment + " first CHR index from the begining of VRAM" );

			_sw.WriteLine( "\n" + c_def + "SCR_BLOCKS2x2_WIDTH\t" + c_def_eq + platform_data.get_screen_blocks_width() + "\t" + c_comment + " number of screen blocks (2x2) in width" );
			_sw.WriteLine( c_def + "SCR_BLOCKS2x2_HEIGHT\t" + c_def_eq + platform_data.get_screen_blocks_height() + "\t" + c_comment + " number of screen blocks (2x2) in height" );
			
			_sw.WriteLine( "\n" + c_def + "ScrTilesWidth\t" + c_def_eq + get_tiles_cnt_width( 1 ) + "\t" + c_comment + " number of screen tiles (" + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + ") in width" );
			_sw.WriteLine( c_def + "ScrTilesHeight\t" + c_def_eq + get_tiles_cnt_height( 1 ) + "\t" + c_comment + " number of screen tiles (" + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + ") in height" );

			_sw.WriteLine( "\n" + c_def + "ScrPixelsWidth\t" + c_def_eq + get_tiles_cnt_width( 1 ) * ( RBtnTiles2x2.Checked ? 16:32 ) + "\t" + c_comment + " screen width in pixels" );
			_sw.WriteLine( c_def + "ScrPixelsHeight\t" + c_def_eq + get_tiles_cnt_height( 1 ) * ( RBtnTiles2x2.Checked ? 16:32 ) + "\t" + c_comment + " screen height in pixels" );
		}
		
		private void save_single_screen_mode( StreamWriter _sw )
		{
			BinaryWriter 	bw;
			BinaryWriter	bw_props;
			
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
			int tiles_cnt;
			int tile_offs_x;
			int tile_offs_y;
			int block_n;
			int bank_n;
			int blocks_props_size;
			int blocks_size;
			int data_offset;
			int common_scr_ind;
			int max_scr_tile;
			int max_tile_ind;
			int max_scr_block;
			int max_block_ind;
			int start_scr_ind;
			int ents_cnt;
			
			int scr_width_blocks_mul2;
			int scr_height_blocks_mul2;
			int scr_height_blocks_mul4;
			
			int scr_width_blocks 	= platform_data.get_screen_blocks_width();
			int scr_height_blocks 	= platform_data.get_screen_blocks_height();

			uint block_data;
			
			ushort tile_id;
			ushort block_id;
			
			screen_data tile_inds;
			ushort[] tile_attrs_arr		= new ushort[ 16 ];
			ushort[] block_attrs_arr	= new ushort[ 8 ];
			
			long data_size;
			long exp_data_size	= 0;

			bool valid_bank;
			bool enable_comments;
			
			string label;
			string level_prefix_str;
			string label_props;
			string data_offset_str;
			string maps_arr;
			string scr_arr	= null;
			
			string global_data_decl = "";
			
			exp_scr_data		exp_scr;
			layout_screen_data	scr_data;
			tiles_data 			tiles;
			
			List< tiles_data > 	banks 			= new List< tiles_data >( 10 );
			List< int >			max_tile_inds	= new List< int >( 10 );
			List< int >			max_block_inds	= new List< int >( 10 );
			
			scr_width_blocks_mul2	= scr_width_blocks << 1;
			scr_height_blocks_mul2	= scr_height_blocks << 1;
			scr_height_blocks_mul4	= scr_height_blocks << 2;

			ushort[] attrs_chr = new ushort[ ( scr_width_blocks * scr_height_blocks ) << 2 ];
						
			exp_scr_data._tiles_offset  = 0;
			exp_scr_data._blocks_offset = 0;

			Dictionary< int, exp_scr_data > screens = new Dictionary< int, exp_scr_data >( 100 );

			scr_ind = 0;		// global screen index
			scr_ind_opt = 0;	// optimized screen index
			
			// collect all banks & screens data in a project
			for( bank_n = 0; bank_n < m_data_mngr.tiles_data_cnt; bank_n++ )
			{
				tiles = m_data_mngr.get_tiles_data( bank_n );
				
				valid_bank = false;
				
				max_tile_ind = max_block_ind = Int32.MinValue;
				
				for( int scr_n = 0; scr_n < tiles.screen_data_cnt(); scr_n++ )
				{
					if( check_screen_layouts( scr_ind ) == true )
					{
						if( scr_ind_opt > CONST_BIDIR_MAP_SCREEN_MAX_CNT - 1 )
						{
							throw new Exception( "The screen index is out of range!\nThe maximum number of screens allowed to export: " + CONST_BIDIR_MAP_SCREEN_MAX_CNT );
						}
						
						valid_bank = true;
						
						exp_scr = new exp_scr_data( scr_ind_opt++, tiles );
						
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
										block_id = tiles.get_tile_block( tile_id, block_n );

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
						max_block_inds.Add( ( 1 + max_block_ind ) << 3 );
					}
					else
					{
						max_block_ind = tiles.get_first_free_block_id( false );
						
						max_block_inds.Add( max_block_ind << 3 );
					}
				}
			}

			var screen_sorted_keys_list = screens.Keys.ToList();
			screen_sorted_keys_list.Sort();
			
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
					label = "chr" + bank_n;
					bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					{
						data_size = tiles.export_CHR( bw );
					}
					bw.Close();
					
					write_alignment_data( _sw );
					_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ")\n" );
					
					chr_arr	 += "\tdc.l " + label + "\n";
					chr_size += "\tdc.w " + ( data_size >> 1 ) + "\t; (" + label + ")\n";
					
					exp_data_size += data_size;
				}
				
				save_global_data( ref global_data_decl, ( m_filename + "_CHRs" ), banks.Count ); // num CHR banks
				save_global_data( ref global_data_decl, ( m_filename + "_CHRs_size" ), banks.Count ); // num CHR banks
				
				write_alignment_data( _sw );
				_sw.WriteLine( m_filename + "_CHRs:\n" + chr_arr );
				write_alignment_data( _sw );
				_sw.WriteLine( m_filename + "_CHRs_size:\n" + chr_size );
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_PU16 + "\t" + CONST_C_DATA_PREFIX + m_filename + "_CHRs;\t\t// array of pointers to CHRs data of all CHR banks" );
					m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + "_CHRs_size;\t\t// array of CHRs data size in words for all CHR banks" );
				}
				
				// static screens ( VDP-READY DATA ):
				// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
				// [SKIP] attrs map arr - attributes array for each screen
				// [SKIP] blocks arr	- blocks data array
				// map arr				- GFX array for each screen (RLE) (VDP-ready) [CHRs]
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
					bw_props = new BinaryWriter( File.Open( m_data_path_filename + label_props + CONST_BIN_EXT, FileMode.Create ) );
				
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
							
							if( RBtnPropPerBlock.Checked && ( block_n % 4 ) != 0 )
							{
								continue;
							}
							
							bw_props.Write( ( byte )tiles_data.get_block_flags_obj_id( block_data ) );
						}
						
						data_offset_str += "\tdc.w " + ( data_offset / ( RBtnPropPerBlock.Checked ? 4:1 ) ) + "\t; (chr" + bank_n + ")\n";

						data_offset += blocks_props_size;
					}

					data_size = bw_props.BaseStream.Length;
					bw_props.Close();
					
					save_global_data( ref global_data_decl, m_filename + label_props, data_size ); // bytes array
					
					exp_data_size += data_size;

					write_alignment_data( _sw );
					_sw.WriteLine( m_filename + label_props + ":\tincbin \"" + get_data_subdir() + m_filename + label_props + CONST_BIN_EXT + "\"\t; (" + data_size + ") block properties array of all exported data banks ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )\n" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U8 + "\t\t" + CONST_C_DATA_PREFIX + m_filename + label_props + ";\t\t// block properties array of all exported data banks ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )" + ( RBtnPropPerBlock.Checked ? ", data offset = props offset / 4":"" ) );
					}
					
					write_alignment_data( _sw );
					{
						_sw.WriteLine( m_filename + "_PropsOffs:" );
						
						save_global_data( ref global_data_decl, ( m_filename + "_PropsOffs" ), banks.Count ); // CHR banks count
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + "_PropsOffs;\t\t// array of block properties data offsets per CHR banks" );
						}
					}
					
					_sw.WriteLine( data_offset_str );
				}
				
				// tiles
				{
					if( RBtnTiles4x4.Checked )
					{
						// write tiles data
						label = "_Tiles";

						bw = new BinaryWriter( File.Open( m_data_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
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
							
							data_offset_str += "\tdc.w " + data_offset + "\t; (chr" + bank_n + ")\n";
							
							data_offset += max_tile_inds[ bank_n ] << 2;
						}
						
						data_size = bw.BaseStream.Length;
						bw.Close();
						
						save_global_data( ref global_data_decl, ( m_filename + label ), data_size >> 2 ); // dwords array
					
						exp_data_size += data_size;
						
						write_alignment_data( _sw );
						_sw.WriteLine( m_filename + label + ":\tincbin \"" + get_data_subdir() + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") 4x4 tiles array of all exported data banks ( 4 bytes per tile )\n" );
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U32 + "\t" + CONST_C_DATA_PREFIX + m_filename + label + ";\t\t// (4x4) 4 block indices per tile, 1 byte per index ( left to right, up to down )" );
						}
						
						save_global_data( ref global_data_decl, ( m_filename + "_TilesOffs" ), banks.Count ); // CHR banks count
						
						write_alignment_data( _sw );
						_sw.WriteLine( m_filename + "_TilesOffs:" );
	
						_sw.WriteLine( data_offset_str );
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + "_TilesOffs;\t\t// array of tile data offsets per CHR banks" );
						}
					}
					
					// save blocks offsets by CHR bank
					{
						data_offset = 0;
						data_offset_str = "";
						
						// tiles
						for( bank_n = 0; bank_n < banks.Count; bank_n++ )
						{
							tiles = banks[ bank_n ];
							
							data_offset_str += "\tdc.w " + data_offset + "\t; (chr" + bank_n + ")\n";
							
							if( m_data_mngr.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
							{
								data_offset += ( 1 + utils.get_ulong_arr_max_val( tiles.tiles, max_tile_inds[ bank_n ] ) ) << 3;
							}
							else
							{
								blocks_size = tiles.get_first_free_block_id( false );
								data_offset += blocks_size << 3;
							}
						}
						
						save_global_data( ref global_data_decl, ( m_filename + "_BlocksOffs" ), banks.Count ); // CHR banks count

						write_alignment_data( _sw );
						_sw.WriteLine( m_filename + "_BlocksOffs:" );
	
						_sw.WriteLine( data_offset_str );
						
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + "_BlocksOffs;\t\t// array of block data offsets per CHR banks" );
						}
					}
				}
				
				// save VDP-ready data for STATIC SCREENS mode
				if( RBtnModeStaticScreen.Checked )
				{
					label = "_VDPScr";
					bw = new BinaryWriter( File.Open( m_data_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );

					data_offset = 0;
					
					foreach( var key in screen_sorted_keys_list )
					{
						exp_scr = screens[ key ];
						
						tile_inds	= exp_scr.m_scr_tiles;
						tiles_cnt 	= tile_inds.length;
						tiles 		= exp_scr.m_tiles;

#if DEF_DBG_PPU_READY_DATA_SAVE_IMG
						Bitmap 	tile_bmp	= null;
						Bitmap 	scr_bmp 	= new Bitmap( platform_data.get_screen_width_pixels(), platform_data.get_screen_height_pixels() );
	
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
						scr_bmp.Save( m_data_path_filename + "_Scr" + exp_scr.m_scr_ind + ".png" );
						scr_bmp.Dispose();
						tile_bmp.Dispose();
#endif
						exp_scr.m_VDP_scr_offset = data_offset;
						screens[ key ] = exp_scr;
						
						compress_and_save_ushort( bw, attrs_chr, ref data_offset );
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, ( m_filename + label ), ( data_size >> 1 ) ); // words array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + get_data_subdir() + m_filename + label + CONST_BIN_EXT + "\"\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_offset + ( CheckBoxRLE.Checked ? " / " + ( screens.Count * m_VDP_ready_scr_data_size ):"" ) + ") VDP-ready data array for each screen (" + m_VDP_ready_scr_data_size + " bytes per screen)" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t\t" + CONST_C_DATA_PREFIX + m_filename + label + ";\t\t// " + ( CheckBoxRLE.Checked ? "[compressed] ":"" ) + "VDP-ready data array for each screen in words" );
					}
					
					if( !CheckBoxRLE.Checked )
					{
						if( m_C_writer != null )
						{
							m_C_writer.WriteLine( "\nconst u16\tScrGfxDataSize = " + m_VDP_ready_scr_data_size + ";\t// VDP-ready screen data in bytes\n" );
						}
						else
						{
							_sw.WriteLine( "\nScrGfxDataSize = " + m_VDP_ready_scr_data_size + "\n" );
						}
					}
				}
				
				// attributes array of 4x4 tiles ONLY and for the BIDIRECTIONAL SCROLLING mode !!!
				if( RBtnModeBidirScroll.Checked )
				{
					label = "_Attrs";
					bw = new BinaryWriter( File.Open( m_data_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
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
							
							bw.Write( get_screen_attribute( block_data ) );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, ( m_filename + label ), data_size >> 1 ); // words array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + get_data_subdir() + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") 2x2 tiles attributes array of all exported data banks ( 2 bytes per attribute )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + label + ";\t\t// map attributes array of all exported CHR banks ( 1 word per attribute; 4 words per block )" );
					}
				}
				
				// map
				{
					// tiles indices array for each screen
					label = "_TilesScr";
					bw = new BinaryWriter( File.Open( m_data_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );

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
							bw.Write( tile_inds.get_tile( i ) );
						}
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, ( m_filename + label ), data_size >> 1 ); // words array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + get_data_subdir() + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") " + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + " tiles array for each screen ( " + ( ( RBtnTiles2x2.Checked ? ( scr_width_blocks * scr_height_blocks ):platform_data.get_screen_tiles_cnt() ) << 1 ) + " bytes per screen \\ 2 bytes per tile )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + label + ";\t\t// " + ( RBtnTiles2x2.Checked ? "2x2":"4x4" ) + " tiles array for each screen ( " + ( ( RBtnTiles2x2.Checked ? ( scr_width_blocks * scr_height_blocks ):platform_data.get_screen_tiles_cnt() ) << 1 ) + " bytes per screen \\ 2 bytes per tile )" );
					}
				}
				
				// palettes
				{
					label = "_Plts";
					bw = new BinaryWriter( File.Open( m_data_path_filename + label + CONST_BIN_EXT, FileMode.Create ) );
					
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
					
					save_global_data( ref global_data_decl, ( m_filename + label ), data_size >> 1 ); // words array 
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( m_filename + label + ":\tincbin \"" + get_data_subdir() + m_filename + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") palettes array of all exported data banks ( data offset = chr_id * " + ( ( platform_data.get_fixed_palette16_cnt() << 4 ) << 1 ) + " )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + m_filename + label + ";\t\t// palettes array of all exported data banks ( data offset = chr_id * " + ( ( platform_data.get_fixed_palette16_cnt() << 4 ) << 1 ) + " )" );
					}
				}
			}

			maps_arr = "\n" + m_filename + "_MapsArr:\n";
			
			for( int level_n = 0; level_n < n_levels; level_n++ )
			{
				enable_comments = true;
				
				level_data = m_data_mngr.get_layout_data( level_n );
				
				level_prefix_str = CONST_FILENAME_LEVEL_PREFIX + level_n;
				
				maps_arr += "\tdc.l " + level_prefix_str + "_StartScr\n";

				_sw.WriteLine( "\n; *** " + level_prefix_str + " ***\n" );
				
				check_ent_instances_cnt( level_data, level_prefix_str );
				
				n_scr_X = level_data.get_width();
				n_scr_Y = level_data.get_height();

				if( RBtnLayoutAdjacentScreenIndices.Checked )
				{
					scr_arr = level_prefix_str + "_ScrArr:";
					
					save_global_data( ref global_data_decl, ( level_prefix_str + "_ScrArr" ), level_data.get_width() * level_data.get_height() );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_SCR + "\t" + CONST_C_DATA_PREFIX + level_prefix_str + "_ScrArr;\t\t// screens array" );
					}
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
									
									if( RBtnLayoutMatrix.Checked )
									{
										if( m_C_writer != null )
										{
											m_C_writer.WriteLine( "\nconst u16\t" + level_prefix_str + "_StartScr\t= " + start_scr_ind + ";\t// start screen index\n" );
										}
										else
										{
											_sw.WriteLine( level_prefix_str + "_StartScr:\tdc.w " + start_scr_ind + "\n" );
										}
										
										save_global_data( ref global_data_decl, ( level_prefix_str + "_Layout" ), ( level_data.get_width() * level_data.get_height() ) ); // screens array
										
										level_data.export_asm( _sw, level_prefix_str, null, "dc.w", "dc.l", "dc.w", "$", false, false, false, false, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex );
										
										if( m_C_writer != null )
										{
											m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_SCR + "\t" + ( CONST_C_DATA_PREFIX + level_prefix_str ) + "_Layout;\t\t// screens matrix of a game map ( num_scr_W x num_scr_H )\n" );
											m_C_writer.WriteLine( "const u16\t" + level_prefix_str + "_WScrCnt = " + level_data.get_width() + ";\t// number of screens in width" );
											m_C_writer.WriteLine( "const u16\t" + level_prefix_str + "_HScrCnt = " + level_data.get_height() + ";\t// number of screens in height" );
										}
									}
									else
									{
										_sw.WriteLine( level_prefix_str + "_StartScr:\tdc.l " + level_prefix_str + "Scr" + start_scr_ind + "\n" );
										
										if( m_C_writer != null )
										{
											save_global_data( ref global_data_decl, ( level_prefix_str + "_StartScr" ), 1 );
											
											m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_SCR + "\t" + CONST_C_DATA_PREFIX + level_prefix_str + "_StartScr;\t\t// start screen" );
										}
									}
								}
								
								exp_scr = screens[ scr_key ];
								
								_sw.WriteLine( level_prefix_str + "Scr" + common_scr_ind + ":" );
								_sw.WriteLine( "\tdc.w " + banks.IndexOf( exp_scr.m_tiles ) + ( enable_comments ? "\t; chr_id":"" ) );
								
								if( CheckBoxExportMarks.Checked )
								{
									_sw.WriteLine( "\tdc.w " + utils.hex( "$", scr_data.mark_adj_scr_mask ) + ( enable_comments ? "\t; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property":"" ) );
								}
								
								if( RBtnModeStaticScreen.Checked )
								{
									_sw.WriteLine( "\n\tdc.w " + exp_scr.m_VDP_scr_offset + ( enable_comments ? "\t; " + m_filename + "_VDPScr offset":"" ) );
								}
								
								_sw.WriteLine( "\n\tdc.w " + exp_scr.m_scr_ind + ( enable_comments ? "\t; screen index":"" ) );
								
								_sw.WriteLine( "" );
								
								if( RBtnLayoutAdjacentScreens.Checked )
								{
									_sw.WriteLine( "\tdc.l " + get_adjacent_screen_label( level_n, level_data, common_scr_ind, -1,	 0	) + ( enable_comments ? "\t; left adjacent screen":"" ) );
									_sw.WriteLine( "\tdc.l " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  0,	-1	) + ( enable_comments ? "\t; up adjacent screen":"" ) );
									_sw.WriteLine( "\tdc.l " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  1,	 0	) + ( enable_comments ? "\t; right adjacent screen":"" ) );
									_sw.WriteLine( "\tdc.l " + get_adjacent_screen_label( level_n, level_data, common_scr_ind,  0,   1	) + ( enable_comments ? "\t; down adjacent screen\n":"\n" ) );
								}
								else
								if( RBtnLayoutAdjacentScreenIndices.Checked )
								{
									if( enable_comments )
									{
										_sw.WriteLine( "; adjacent screen indices ( the valid values are $00 - $FE, $FF - means no screen )" );
										_sw.WriteLine( "; use the " + level_prefix_str + "_ScrArr array to get a screen description by adjacent screen index" );
									}
									
									_sw.WriteLine( "\tdc.b " + get_adjacent_screen_index( level_n, level_data, common_scr_ind, -1,	 0	) + ( enable_comments ? "\t; left adjacent screen index":"" ) );
									_sw.WriteLine( "\tdc.b " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  0,	-1	) + ( enable_comments ? "\t; up adjacent screen index":"" ) );
									_sw.WriteLine( "\tdc.b " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  1,	 0	) + ( enable_comments ? "\t; right adjacent screen index":"" ) );
									_sw.WriteLine( "\tdc.b " + get_adjacent_screen_index( level_n, level_data, common_scr_ind,  0,   1	) + ( enable_comments ? "\t; down adjacent screen index\n":"\n" ) );
									
									scr_arr += "\n\tdc.l " + level_prefix_str + "Scr" + common_scr_ind;
								}
								
								if( CheckBoxExportEntities.Checked )
								{
									scr_data.export_entities_asm( _sw, ref ents_cnt, level_prefix_str + "Scr" + common_scr_ind + "EntsArr", "dc.w", "dc.l", "dc.w", "$", RBtnEntityCoordScreen.Checked, scr_n_X, scr_n_Y, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex, enable_comments );
									
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
					_sw.WriteLine( level_prefix_str + "_EntInstCnt = " + ents_cnt + "\t; number of entities instances\n" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "\nconst u16\t" + level_prefix_str + "_EntInstCnt = " + ents_cnt + ";\t// number of entities instances" );
					}
				}
			}
			
			// save MapsArr and global exported data
			{
				_sw.WriteLine( maps_arr );
				
				if( CheckBoxExportSGDKData.Checked )
				{
					save_global_data( ref global_data_decl, m_filename + "_MapsArr", n_levels );

					_sw.Write( "\n; Global data exported to the \'" + m_filename + ".h\'\n\n.section\t.rodata\n\n" + global_data_decl );
				}
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern const mpd_ARR_MAP\tmpd_" + m_filename + "_MapsArr;\t\t// array of all exported maps" );
				}
			}
			
			foreach( var key in screens.Keys ) { screens[ key ].destroy(); }
		}

		private void compress_and_save_ushort( BinaryWriter _bw, ushort[] _data, ref int _data_offset )
		{
			try
			{
				if( CheckBoxRLE.Checked )
				{
					ushort[] rle_data_arr	= null;
	
					int rle_data_size = utils.RLE16( _data, ref rle_data_arr, true );
					
					if( rle_data_size < 0 )
					{
						// save uncompressed data
						//...
					}
					else
					{
						utils.write_ushort_arr( _bw, rle_data_arr, rle_data_size );
						
						_data_offset += rle_data_size * sizeof( ushort );
					}
					
					rle_data_arr = null;
				}
				else
				{
					utils.write_ushort_arr( _bw, _data, _data.Length );
					
					_data_offset += _data.Length * sizeof( ushort );
				}
			}
			catch( Exception )
			{
				_bw.Dispose();
				
				throw;
			}
			
		}

		private void compress_and_save_ushort( BinaryWriter _bw, ushort[] _data )
		{
			int offset = 0;
			
			compress_and_save_ushort( _bw, _data, ref offset );
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
								_attrs_chr[ tile_x + tile_y + block_x + block_y + chr_x + chr_y ] = get_screen_attribute( _tiles, tile_id, block_n, chr_n );
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
						_attrs_chr[ block_x + block_y + chr_x + chr_y ] = get_screen_attribute( _tiles, _block_inds.get_tile( block_n ), chr_n );
					}
				}
			}
		
			if( RBtnTilesDirRows.Checked || _force_swapping )
			{
				utils.swap_columns_rows_order< ushort >( _attrs_chr, _scr_width_blocks_mul2, _scr_height_blocks_mul2 );
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
			ushort native_data = ( ushort )tiles_data.block_data_repack_to_native( _block_data, get_CHR_offset() );

			int block_prop = tiles_data.get_block_flags_obj_id( _block_data );

			// add priority bit
			if( ComboBoxInFrontOfSpritesProp.SelectedIndex > 0 && block_prop == ( ComboBoxInFrontOfSpritesProp.SelectedIndex - 1 ) )
			{
				native_data |= ( ushort )( 1 << 15 );
			}
			
			return native_data;
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
		
		private int get_CHR_offset()
		{
			return ( int )NumericUpDownCHROffset.Value;
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
			BinaryWriter 	bw;
			
			layout_data level_data;
			
			ushort[]	map_data_arr;
			ushort[]	map_tiles_arr;
			byte[]		block_props_arr;
			ushort[]	map_blocks_arr		= null;
			
			List< tiles_data > scr_tiles_data = m_data_mngr.get_tiles_data();
			
			int n_levels 	= m_data_mngr.layouts_data_cnt;
			int n_banks		= scr_tiles_data.Count;
			int n_scr_X;
			int n_scr_Y;
			int tile_n;
			int block_n;
			int n_Y_tiles;
			int n_screens;
			int scr_ind;
			int bank_ind;
			int chk_bank_ind;
			int bank_n;
			int tile_offs_x;
			int tile_offs_y;
			int max_tile_ind;
			int max_block_ind;
			int blocks_props_size;
			ushort tile_id;
			byte block_id;
			uint block_data;
			
			layout_screen_data	scr_data;
			
			long 	data_size;
			long	exp_data_size	= 0;
			
			string 	label;
			string 	palette_str		= null;
			
			string level_prefix_str;
			string bank_prefix_str;
			string maps_CHRs_arr	= "\nMapsCHRBanks:\n";
			
			string global_data_decl = "";
			
			int scr_width_blocks 	= platform_data.get_screen_blocks_width();
			int scr_height_blocks 	= platform_data.get_screen_blocks_height();
			
			tiles_data tiles;

			StreamWriter def_sw = _sw;
			
			string c_u16 = "";
			string c_code_comm_delim = "\t; ";
			
			if( m_C_writer != null )
			{
				c_u16				= "const u16\t";
				c_code_comm_delim 	= ";\t// ";
				
				def_sw	= m_C_writer;
			}
			
			_sw.WriteLine( "\n; *** GLOBAL DATA ***\n" );
			
			save_global_data( ref global_data_decl, "MapsCHRBanks", n_levels );
			
			for( bank_n = 0; bank_n < n_banks; bank_n++ )
			{
				tiles = scr_tiles_data[ bank_n ]; 
				
				bank_prefix_str = CONST_FILENAME_BANK_PREFIX + bank_n;
				
				// write CHR banks data
				label = bank_prefix_str + "_CHR";
				bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
				{
					tiles.export_CHR( bw );
				}
				long CHR_data_size = data_size = bw.BaseStream.Length;
				bw.Close();
				
				save_global_data( ref global_data_decl, label, ( CHR_data_size >> 1 ) ); // words array
				
				exp_data_size += data_size;
				
				write_alignment_data( _sw );
				def_sw.WriteLine( c_u16 + bank_prefix_str + "_CHR_data_size\t= " + CHR_data_size + c_code_comm_delim + "map CHRs size in bytes" );

				_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; (" + data_size + ")" );
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + label + ";\t\t// sprites 8x8 data array; 32 bytes per sprite" );	// words array
				}

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
					bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
					for( int i = 0; i < max_tile_ind; i++ )
					{
						bw.Write( tiles_data.tile_convert_ulong_to_uint_reverse( tiles.tiles[ i ] ) );
					}
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, label, data_size >> 2 ); // dwords array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") (4x4) 4 block indices per tile ( left to right, up to down )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U32 + "\t" + CONST_C_DATA_PREFIX + label + ";\t// (4x4) 4 block indices per tile, 1 byte per index ( left to right, up to down )" ); // Tiles4x4 are 4-bytes data per tile => u32 array
					}
				}
				
				// tiles 2x2
				{
					max_block_ind = tiles.get_first_free_block_id( false ) << 2;	// 4 ushorts per block
					
					label = bank_prefix_str + "_Attrs";
					bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
	
					for( block_n = 0; block_n < max_block_ind; block_n++ )
					{
						block_data = tiles.blocks[ block_n ];
						
						bw.Write( get_screen_attribute( block_data ) );
					}
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, label, data_size >> 1 ); // words array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; " + "(" + data_size + ") map attributes array per block ( 2 bytes per attribute; 8 bytes per block )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + label + ";\t// map attributes array per block ( 1 word per attribute; 4 words per block )" );
					}
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
					label = bank_prefix_str + "_Props";
					bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					bw.Write( block_props_arr );
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, label, data_size ); // bytes array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") block properties array ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U8 + "\t\t" + CONST_C_DATA_PREFIX + label + ";\t// block properties array ( " + ( RBtnPropPerCHR.Checked ? "4 bytes":"1 byte" ) + " per block )" );
					}
				}

				palette_str = bank_prefix_str + "_Palette:";
			
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + bank_prefix_str + "_Palette;\t// 4 palettes by 16 colors\n" );
				}
				
				for( int i = 0; i < tiles.palettes_arr.Count; i++ )
				{
					palette_str += "\n\t\tdc.w ";
					
					fill_palette_str( tiles.palettes_arr[ i ].m_palette0, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette1, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette2, ref palette_str, false );
					fill_palette_str( tiles.palettes_arr[ i ].m_palette3, ref palette_str, true );
				}
				
				_sw.WriteLine( palette_str + "\n" + ( bank_prefix_str + "_Palette_End:\n" ) );
				
				save_global_data( ref global_data_decl, ( bank_prefix_str + "_Palette" ), tiles.palettes_arr.Count << 4 ); // *16
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
					map_tiles_arr = new ushort[ n_scr_X * n_scr_Y * platform_data.get_screen_tiles_cnt() ];
					
					Array.Clear( map_tiles_arr, 0, map_tiles_arr.Length );
				}
				
				if( RBtnTiles2x2.Checked )
				{
					map_blocks_arr = new ushort[ n_Y_tiles * n_scr_X * ( platform_data.get_screen_width_pixels() >> 4 ) ];
					
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
										
										// make a list of 2x2 tiles of the current map
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
				
				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( "//*** " + level_prefix_str + " ***\n" );
				}
				
				maps_CHRs_arr += "\tdc.w " + chk_bank_ind + "\n";
				
				// write map
				{
					label = level_prefix_str + "_Map";
					bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
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
						utils.swap_columns_rows_order< ushort >( map_data_arr, get_tiles_cnt_width( n_scr_X ), get_tiles_cnt_height( n_scr_Y ) );
					}
					
					compress_and_save_ushort( bw, map_data_arr );
					
					data_size = bw.BaseStream.Length;
					bw.Close();
					
					save_global_data( ref global_data_decl, label, data_size >> 1 ); // words array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t\t; " + ( CheckBoxRLE.Checked ? "compressed ":"" ) + "(" + data_size + ( CheckBoxRLE.Checked ? " / " + ( map_data_arr.Length << 1 ):"" ) + ") game map " + ( RBtnTiles4x4.Checked ? "tiles (4x4)":"blocks (2x2)" ) + " array" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + label + ";\t\t// " + ( CheckBoxRLE.Checked ? "[compressed] ":"" ) + "game map " + ( RBtnTiles4x4.Checked ? "tiles (4x4)":"blocks (2x2)" ) + " array" );
					}
				}
				
				// tiles lookup table
				{
					label = level_prefix_str + "_MapTbl";
					bw = new BinaryWriter( File.Open( m_data_path_filename + "_" + label + CONST_BIN_EXT, FileMode.Create ) );
					
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
					
					save_global_data( ref global_data_decl, label, data_size >> 1 ); // words array
					
					exp_data_size += data_size;
					
					write_alignment_data( _sw );
					_sw.WriteLine( label + ":\tincbin \"" + get_data_subdir() + m_filename + "_" + label + CONST_BIN_EXT + "\"\t; (" + data_size + ") lookup table for fast calculation of tile addresses; " + ( RBtnTilesDirColumns.Checked ? "columns by X coordinate":"rows by Y coordinate" ) + " ( 16 bit offset per " + ( RBtnTilesDirColumns.Checked ? "column":"row" ) + " of tiles )\n" );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_U16 + "\t" + CONST_C_DATA_PREFIX + label + ";\t// lookup table for fast calculation of tile addresses; " + ( RBtnTilesDirColumns.Checked ? "columns by X coordinate":"rows by Y coordinate" ) + " ( 16-bit offset per " + ( RBtnTilesDirColumns.Checked ? "column":"row" ) + " of tiles )" );
					}
				}
				
				write_alignment_data( _sw );
				
				int start_scr_ind = level_data.get_start_screen_ind();
				
				if( start_scr_ind < 0 )
				{
					MainForm.message_box( "The start screen wasn't assigned to layout: " + level_n + "\n\nWARNING: A zero screen will be used as a start one.", "Start Screen Warning", MessageBoxButtons.OK );
					
					start_scr_ind = 0;
				}

				if( m_C_writer != null )
				{
					m_C_writer.WriteLine( c_u16 + level_prefix_str + "_StartScr\t= " + start_scr_ind + c_code_comm_delim + "start screen" );
				}
				else
				{
					_sw.WriteLine( level_prefix_str + "_StartScr:\tdc.w " + start_scr_ind + "\t; start screen\n" );
				}
				
				def_sw.WriteLine( c_u16 + level_prefix_str + "_WTilesCnt\t= " + get_tiles_cnt_width( n_scr_X ) + c_code_comm_delim + "number of level tiles in width" );
				def_sw.WriteLine( c_u16 + level_prefix_str + "_HTilesCnt\t= " + get_tiles_cnt_height( n_scr_Y ) + c_code_comm_delim + "number of level tiles in height" );
				
				def_sw.WriteLine( c_u16 + level_prefix_str + "_WPixelsCnt\t= " + get_tiles_cnt_width( n_scr_X ) * ( RBtnTiles2x2.Checked ? 16:32 ) + c_code_comm_delim + "map width in pixels" );
				def_sw.WriteLine( c_u16 + level_prefix_str + "_HPixelsCnt\t= " + get_tiles_cnt_height( n_scr_Y ) * ( RBtnTiles2x2.Checked ? 16:32 ) + c_code_comm_delim + "map height in pixels" );
					
				def_sw.WriteLine( c_u16 + level_prefix_str + "_TilesCnt\t= " + ( ( RBtnTiles2x2.Checked ? map_blocks_arr:map_tiles_arr ).Max() + 1 ) + c_code_comm_delim + "map blocks count\n" );

				if( CheckBoxExportEntities.Checked )
				{
					save_global_data( ref global_data_decl, ( level_prefix_str + "_Layout" ), ( level_data.get_width() * level_data.get_height() ) ); // screens array
					
					level_data.export_asm( _sw, level_prefix_str, null, "dc.w", "dc.l", "dc.w", "$", true, CheckBoxExportMarks.Checked, CheckBoxExportEntities.Checked, RBtnEntityCoordScreen.Checked, ( layout_screen_data.e_entity_sort_type )CBoxEntSortingType.SelectedIndex );
					
					if( m_C_writer != null )
					{
						m_C_writer.WriteLine( "extern const " + CONST_C_STRUCT_ARR_SCR + "\t" + ( CONST_C_DATA_PREFIX + level_prefix_str ) + "_Layout;\t// screens matrix of a game map ( num_scr_W x num_scr_H )\n" );
						m_C_writer.WriteLine( "const u16\t" + level_prefix_str + "_WScrCnt = " + level_data.get_width() + ";\t// number of screens in width" );
						m_C_writer.WriteLine( "const u16\t" + level_prefix_str + "_HScrCnt = " + level_data.get_height() + ";\t// number of screens in height" );
						
						m_C_writer.WriteLine( "const u16\t" + level_prefix_str + "_EntInstCnt = " + level_data.get_num_ent_instances() + ";\t// number of entities instances" );
					}
				}
				else
				{
					def_sw.WriteLine( c_u16 + level_prefix_str + "_WScrCnt\t= " + n_scr_X + c_code_comm_delim + "number of screens in width" );
					def_sw.WriteLine( c_u16 + level_prefix_str + "_HScrCnt\t= " + n_scr_Y + c_code_comm_delim + "number of screens in height\n" );
				}
				
				map_data_arr 	= null;
				map_tiles_arr 	= null;
				map_blocks_arr	= null;
			}
			
			// maps -> CHR bank
			{
				_sw.WriteLine( maps_CHRs_arr );
			}
			
			if( CheckBoxExportSGDKData.Checked )
			{
				_sw.Write( "\n; Global data exported to the \'" + m_filename + ".h\'\n\n.section\t.rodata\n\n" + global_data_decl );
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
	}
}