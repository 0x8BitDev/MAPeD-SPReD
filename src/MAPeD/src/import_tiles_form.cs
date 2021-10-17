/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 14.12.2018
 * Time: 19:38
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;

namespace MAPeD
{
	/// <summary>
	/// Description of import_tiles_form.
	/// </summary>
	public partial class import_tiles_form : Form
	{
		delegate void update_progress_bar();
		
		private layout_data	m_level_layout = null;
		
		public layout_data level_layout
		{
			get
			{
				layout_data data	= m_level_layout;
				m_level_layout		= null;
				
				return data;
			}
			set {}
		}
		
		public bool import_tiles
		{
			get { return CheckBoxTiles.Checked; }
			set {}
		}

		public bool import_game_map
		{
			get { return CheckBoxGameMap.Checked; }
			set {}
		}
		
		public bool delete_empty_screens
		{
			get { return CheckBoxDeleteEmptyScreens.Checked; }
			set {}
		}
		
		public bool apply_palette
		{
			get { return CheckBoxApplyPalette.Checked; }
			set {}
		}
		
		public bool skip_zero_CHR_Block
		{
			get { return CheckBoxSkipZeroCHRBlock.Checked; }
			set {}
		}
		
		public import_tiles_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
#if DEF_FIXED_LEN_PALETTE16_ARR
			CheckBoxApplyPalette.Text = "Apply palette";
#endif
			CheckBoxApplyPalette.Checked = true;
#if DEF_ZX
			CheckBoxApplyPalette.Enabled = false;
#endif

#if !DEF_PALETTE16_PER_CHR || DEF_ZX
			BtnImportPaletteASIS.Enabled = CheckBoxImportPaletteASIS.Enabled = false;
#endif
		}
		
		void CheckBoxBlocksChanged_Event(object sender, EventArgs e)
		{
			CheckBoxApplyPalette.Enabled = CheckBoxBlocks.Checked;
			CheckBoxApplyPalette.Checked = false;
			CheckBoxTiles.Checked = CheckBoxBlocks.Checked ? CheckBoxTiles.Checked:false;
		}
		
		void CheckBoxTilesChanged_Event(object sender, EventArgs e)
		{
			CheckBoxBlocks.Enabled = !CheckBoxTiles.Checked;
			CheckBoxBlocks.Checked =  CheckBoxTiles.Checked ? true:CheckBoxBlocks.Checked;
		}
		
		void CheckBoxGameMapChanged_Event(object sender, EventArgs e)
		{
			CheckBoxBlocks.Checked = CheckBoxTiles.Checked = CheckBoxGameMap.Checked ? true:CheckBoxTiles.Checked;
			CheckBoxApplyPalette.Checked = CheckBoxGameMap.Checked ? true:CheckBoxApplyPalette.Checked;
			CheckBoxDeleteEmptyScreens.Enabled = !( CheckBoxTiles.Enabled = CheckBoxGameMap.Checked ? false:true );
			CheckBoxDeleteEmptyScreens.Checked = CheckBoxDeleteEmptyScreens.Enabled ? CheckBoxDeleteEmptyScreens.Checked:false;
		}

		void CheckBoxApplyPaletteChanged_Event(object sender, EventArgs e)
		{
#if DEF_NES			
			CheckBoxSkipZeroCHRBlock.Enabled = !CheckBoxApplyPalette.Checked;
			CheckBoxSkipZeroCHRBlock.Checked = CheckBoxSkipZeroCHRBlock.Enabled ? CheckBoxSkipZeroCHRBlock.Checked:false;
#endif

#if DEF_PALETTE16_PER_CHR && !DEF_ZX
			CheckBoxImportPaletteASIS.Enabled = CheckBoxApplyPalette.Checked;
			CheckBoxImportPaletteASIS.Checked = !CheckBoxApplyPalette.Checked ? false:CheckBoxImportPaletteASIS.Checked;
#endif
		}
		
		private int get_local_scr_ind( int _global_scr_ind, data_sets_manager _data_manager )
		{
			return _data_manager.get_local_screen_ind( _data_manager.tiles_data_pos, _global_scr_ind );
		}

		public void data_processing( Bitmap _bmp, data_sets_manager _data_manager, Func< int, int, layout_data > _create_layout, IProgress< int > _progress_val, IProgress< string > _progress_status )
		{
			bool import_game_map_as_is = true;

#if DEF_SCREEN_HEIGHT_7d5_TILES
			int tile_size = 16;
#else
			int tile_size = _data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ? 32:16;
#endif
			if( ( _bmp.Width % tile_size ) != 0 || ( _bmp.Height % tile_size ) != 0 )
			{
				MainForm.message_box( "The imported image isn't multiple to a tile size: " + tile_size + "\n\nThe imported data will be trimmed.", "Image Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
			
			if( import_tiles )
			{
				if( import_game_map )
				{
					if( ( _bmp.Width > 0 && ( _bmp.Width % utils.CONST_SCREEN_WIDTH_PIXELS ) != 0 ) || ( _bmp.Height > 0 && ( _bmp.Height % utils.CONST_SCREEN_HEIGHT_PIXELS ) != 0 ) )
					{
						DialogResult dlg_res = MainForm.message_box( "To get the best result, it's recommended that an imported image size must be a multiple of the game screen size.\n\nCrop the imported game map or leave it 'as is'?\n\n[Yes] Crop the game map to fully filled screens\n[No] Import the game map 'as is'", "Game map Import Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
						
						if( dlg_res == DialogResult.Cancel )
						{
							throw new Exception( "Operation aborted!" );
						}
						
						import_game_map_as_is = ( dlg_res == DialogResult.No );
					}
				}

				import_image_data( import_game_map_as_is, _bmp, _data_manager.get_tiles_data( _data_manager.tiles_data_pos ), _data_manager, _create_layout, _progress_val, _progress_status );
			}
			else
			{
				import_image_data( false, _bmp, _data_manager.get_tiles_data( _data_manager.tiles_data_pos ), _data_manager, null, _progress_val, _progress_status );
			}
		}
		
		public void import_image_data( 	bool 							_import_game_map_as_is,
										Bitmap							_bmp,
										tiles_data 						_data, 
										data_sets_manager 				_data_manager, 
										Func< int, int, layout_data > 	_create_layout,
										IProgress< int > 				_progress_val,
										IProgress< string > 			_progress_status )
		{
			BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, _bmp.Width, _bmp.Height ), ImageLockMode.ReadOnly, _bmp.PixelFormat );
			
			if( bmp_data != null )
			{
				IntPtr data_ptr = bmp_data.Scan0;
				
				int stride = bmp_data.Stride;
				
				int CHR_ind 	= _data.get_first_free_spr8x8_id();
				
				if( CHR_ind < 0 )
				{
					throw new Exception( "There is no free space in the active CHR bank!" );
				}
				
				int block_ind 	= _data.get_first_free_block_id();
				
				if( block_ind < 0 )
				{
					throw new Exception( "There is no free space in the blocks list!" );
				}
				
				// convert to array index
				block_ind <<= 2;
				
				CHR_ind = ( CHR_ind == 0 && skip_zero_CHR_Block == true ) ? 1:CHR_ind;
				block_ind = ( block_ind == 0 && skip_zero_CHR_Block == true ) ? 4:block_ind;
				
				byte[] CHR_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
				
				uint[] block_data = new uint[ utils.CONST_BLOCK_SIZE ];
				
				int dup_block_ind;
				int dup_tile_ind;
				
				int beg_tile_ind	= -1;
				int tile_ind		= -1;
				
				int beg_CHR_ind 	= CHR_ind;
				int beg_block_ind 	= block_ind;
				
				int scr_tile_ind;
				int scr_block_ind;

				int bmp_width	= _bmp.Width - ( _data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ? ( _bmp.Width % 32 ):( _bmp.Width % 16 ) );
#if DEF_SCREEN_HEIGHT_7d5_TILES				
				int bmp_height	= _bmp.Height - ( _bmp.Height % 16 );
#else
				int bmp_height	= _bmp.Height - ( _data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ? ( _bmp.Height % 32 ):( _bmp.Height % 16 ) );
#endif
				int tile_cnt	= ( bmp_width >> 4 ) * ( bmp_height >> 4 );
				int tile_n		= 0;
				int progress	= 0;
				
				update_progress_bar upd_progress = delegate() 
				{
					if( ( tile_n % 50 ) == 0 )
					{
						_progress_val.Report( utils.calc_progress_val( ref progress, tile_cnt, tile_n ) );
					}
					++tile_n;									
				};
				
				if( import_tiles )
				{
					ulong tile_data = 0;
					
					tile_ind 		= 0;
					beg_tile_ind	= _data.get_first_free_tile_id();
					
					if( beg_tile_ind < 0 )
					{
						throw new Exception( "There is no free space in the tiles list!" );
					}
					
					int block_offset_x = 0;
					int block_offset_y = 0;
					
					int block_num = 0;
					
					tile_ind = beg_tile_ind;
					
					bmp_width	= ( import_game_map && !_import_game_map_as_is ) ? ( ( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ) * utils.CONST_SCREEN_WIDTH_PIXELS ):bmp_width;
					bmp_height	= ( import_game_map && !_import_game_map_as_is ) ? ( ( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ) * utils.CONST_SCREEN_HEIGHT_PIXELS ):bmp_height;
					
					int scr_x_cnt = 0;
					int scr_y_cnt = 0;

					int scr_x;
					int scr_y;
#if DEF_SCREEN_HEIGHT_7d5_TILES
					bool half_tile = false;
#endif
					if( import_game_map )
					{
						bmp_width	= ( bmp_width == 0 ) ? utils.CONST_SCREEN_WIDTH_PIXELS:bmp_width;
						bmp_height	= ( bmp_height == 0 ) ? utils.CONST_SCREEN_HEIGHT_PIXELS:bmp_height;

						scr_x_cnt = ( ( bmp_width % utils.CONST_SCREEN_WIDTH_PIXELS ) == 0 ) ? ( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ):( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ) + 1;
						scr_y_cnt = ( ( bmp_height % utils.CONST_SCREEN_HEIGHT_PIXELS ) == 0 ) ? ( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ):( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ) + 1;
						
						m_level_layout = _create_layout( scr_x_cnt, scr_y_cnt );
						
						if( m_level_layout == null )
						{
							throw new Exception( "Can't create layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT );
						}
						
						_progress_status.Report( "Map data" );
					}
					else
					{
						_progress_status.Report( "Tiles data" );
					}
#if DEF_SCREEN_HEIGHT_7d5_TILES
					if( import_game_map )
					{
						bmp_height += ( ( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 1 ) << 5;
					}
#endif
					// tiles/blocks/CHRs
					for( int tile_y = 0; tile_y < bmp_height; tile_y += 32 )
					{
						for( int tile_x = 0; tile_x < bmp_width; tile_x += 32 )
						{
#if DEF_SCREEN_HEIGHT_7d5_TILES
							half_tile = false;
#endif
							for( int block_y = 0; block_y < 32; block_y += 16 )
							{
								if( _data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 )
								{
									if( tile_y + block_y >= bmp_height )
									{
										break;
									}
								}
								
								block_offset_y = tile_y + block_y;
#if DEF_SCREEN_HEIGHT_7d5_TILES
								if( import_game_map )
								{
									block_offset_y -= ( ( ( tile_y & 0xff00 ) >> 8 ) << 4 );
								}
#endif
								for( int block_x = 0; block_x < 32; block_x += 16 )
								{
									if( _data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 )
									{
										if( tile_x + block_x >= bmp_width )
										{
											break;
										}
									}
									
									block_offset_x = tile_x + block_x;
									
									for( int CHR_n = 0; CHR_n < 4; CHR_n++ )
									{
#if DEF_SCREEN_HEIGHT_7d5_TILES
										if( import_game_map )
										{
											// extract a CHR if it's not an invisible part of a tile ( the bottom blocks of the eigth tiles row ) 
											if( !( ( ( ( block_offset_y - block_y ) % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 5 ) == 7 && block_y > 0 ) )
											{
												extract_CHR( _bmp.PixelFormat, CHR_n, block_offset_x, block_offset_y, stride, utils.tmp_spr8x8_buff, data_ptr );
											}
											else
											{
#if DEF_SCREEN_HEIGHT_7d5_TILES
												half_tile = true;
#endif
												// clear an invisible part of a tile
												Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
											}
										}
										else
#endif											
										extract_CHR( _bmp.PixelFormat, CHR_n, block_offset_x, block_offset_y, stride, utils.tmp_spr8x8_buff, data_ptr );
										
										if( !check_CHR_and_build_block( ref CHR_ind, ref block_ind, CHR_buff, block_data, CHR_n, _data ) )
										{
											MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
											
											MainForm.message_box( "The CHR Bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
											
											import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind, beg_tile_ind, tile_ind, _progress_status );
											
											return;
										}
									}
									
									// check duplicate blocks
									{
										if( ( dup_block_ind = _data.contains_block( block_data, block_ind ) ) < 0 )
										{
											if( ( block_ind >> 2 ) >= platform_data.get_max_blocks_cnt() )
											{
												MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
												
												MainForm.message_box( "The blocks list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
												
												import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind, beg_tile_ind, tile_ind, _progress_status );
												
												return;
											}
											
											_data.blocks[ block_ind++ ] = block_data[ 0 ];
											_data.blocks[ block_ind++ ] = block_data[ 1 ];
											_data.blocks[ block_ind++ ] = block_data[ 2 ];
											_data.blocks[ block_ind++ ] = block_data[ 3 ];
										}
									}
									
									if( _data_manager.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
									{
										tile_data = utils.set_ushort_to_ulong( tile_data, block_num++, ( dup_block_ind >= 0 ? ( ushort )( dup_block_ind >> 2 ):( ushort )( ( block_ind - 1 ) >> 2 ) ) );
										
										if( ( block_num & 0x03 ) == 0x00 )
										{
											scr_tile_ind = tile_ind;
#if DEF_SCREEN_HEIGHT_7d5_TILES
											if( ( dup_tile_ind = _data.contains_tile( tile_ind, tile_data, half_tile ) ) >= 0 )
#else
											if( ( dup_tile_ind = _data.contains_tile( tile_ind, tile_data ) ) >= 0 )
#endif											
											{
												scr_tile_ind = dup_tile_ind; 
											}
											else
											{
												if( tile_ind >= platform_data.get_max_tiles_cnt() )
												{
													MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
													
													MainForm.message_box( "The tiles list is full!\n\nTry switching to the Blocks (2x2) mode.", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
													
													import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind, beg_tile_ind, tile_ind, _progress_status );
													
													return;
												}
												
												_data.tiles[ tile_ind++ ] = tile_data;
											}
											
											block_num = 0;
	
											if( import_game_map )
											{
												scr_x = tile_x / utils.CONST_SCREEN_WIDTH_PIXELS;											
#if DEF_SCREEN_HEIGHT_7d5_TILES
												scr_y = ( block_offset_y - block_y ) / utils.CONST_SCREEN_HEIGHT_PIXELS;
	
												_data.set_screen_tile( get_local_scr_ind( m_level_layout.get_data( scr_x, scr_y ).m_scr_ind, _data_manager ), utils.CONST_SCREEN_NUM_WIDTH_TILES * ( ( ( block_offset_y - block_y ) % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 5 ) + ( ( tile_x >> 5 ) % utils.CONST_SCREEN_NUM_WIDTH_TILES ), ( ushort )scr_tile_ind );
#else
												scr_y = tile_y / utils.CONST_SCREEN_HEIGHT_PIXELS;
												
												_data.set_screen_tile( get_local_scr_ind( m_level_layout.get_data( scr_x, scr_y ).m_scr_ind, _data_manager ), utils.CONST_SCREEN_NUM_WIDTH_TILES * ( ( tile_y >> 5 ) % utils.CONST_SCREEN_NUM_HEIGHT_TILES ) + ( ( tile_x >> 5 ) % utils.CONST_SCREEN_NUM_WIDTH_TILES ), ( ushort )scr_tile_ind );
#endif
											}
										}
									}
									else
									{
										if( import_game_map )
										{
											scr_block_ind = ( dup_block_ind >= 0 ? ( dup_block_ind >> 2 ):( ( block_ind - 1 ) >> 2 ) );
											
											scr_x = ( tile_x + block_x ) / utils.CONST_SCREEN_WIDTH_PIXELS;
#if DEF_SCREEN_HEIGHT_7d5_TILES
											if( !half_tile )
											{
												scr_y = ( block_offset_y - block_y ) / utils.CONST_SCREEN_HEIGHT_PIXELS;
	
												_data.set_screen_tile( get_local_scr_ind( m_level_layout.get_data( scr_x, scr_y ).m_scr_ind, _data_manager ), utils.CONST_SCREEN_NUM_WIDTH_BLOCKS * ( ( block_offset_y % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 4 ) + ( ( ( tile_x + block_x ) >> 4 ) % utils.CONST_SCREEN_NUM_WIDTH_BLOCKS ), ( ushort )scr_block_ind );
											}
#else
											scr_y = ( tile_y + block_y ) / utils.CONST_SCREEN_HEIGHT_PIXELS;
											
											_data.set_screen_tile( get_local_scr_ind( m_level_layout.get_data( scr_x, scr_y ).m_scr_ind, _data_manager ), utils.CONST_SCREEN_NUM_WIDTH_BLOCKS * ( ( ( tile_y + block_y ) >> 4 ) % utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS ) + ( ( ( tile_x + block_x ) >> 4 ) % utils.CONST_SCREEN_NUM_WIDTH_BLOCKS ), ( ushort )scr_block_ind );
#endif
										}
									}

									upd_progress();
								}
							}
						}
					}
					
					MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
				}
				else
				{
					_progress_status.Report( CheckBoxBlocks.Checked ? "Blocks data":"CHR data" );
					
					// blocks/CHRs
					for( int block_y = 0; block_y < bmp_height; block_y += 16 )
					{
						for( int block_x = 0; block_x < bmp_width; block_x += 16 )
						{
							for( int CHR_n = 0; CHR_n < 4; CHR_n++ )
							{
								extract_CHR( _bmp.PixelFormat, CHR_n, block_x, block_y, stride, utils.tmp_spr8x8_buff, data_ptr );
								
								if( !check_CHR_and_build_block( ref CHR_ind, ref block_ind, CHR_buff, block_data, CHR_n, _data ) )
								{
									MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
									
									MainForm.message_box( "The CHR Bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
									
									import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind, beg_tile_ind, tile_ind, _progress_status );
									
									return;
								}
							}
							
							// check duplicate blocks
							if( CheckBoxBlocks.Checked )
							{
								if( ( dup_block_ind = _data.contains_block( block_data, block_ind ) ) < 0 )
								{
									if( ( block_ind >> 2 ) >= platform_data.get_max_blocks_cnt() )
									{
										MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
										
										MainForm.message_box( "The blocks list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
										
										import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind, beg_tile_ind, tile_ind, _progress_status );
										
										return;
									}
									
									_data.blocks[ block_ind++ ] = block_data[ 0 ];
									_data.blocks[ block_ind++ ] = block_data[ 1 ];
									_data.blocks[ block_ind++ ] = block_data[ 2 ];
									_data.blocks[ block_ind++ ] = block_data[ 3 ];
								}
							}

							upd_progress();
						}
					}
					
					MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
				}
				
				_bmp.UnlockBits( bmp_data );
				
				import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind, beg_tile_ind, tile_ind, _progress_status );
			}
		}
		
		private bool check_CHR_and_build_block( ref int _CHR_ind, ref int _block_ind, byte[] _CHR_buff, uint[] _block_data, int _CHR_n, tiles_data _data )
		{
			int dup_CHR_ind;
			
			if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
			{
				if( CheckBoxBlocks.Checked )
				{
					_block_data[ _CHR_n ] = tiles_data.set_block_CHR_id( dup_CHR_ind, 0 );
				}
			}
			else
			{
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
				Array.Copy( utils.tmp_spr8x8_buff, _CHR_buff, _CHR_buff.Length );
				
				tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP
				
				if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
				{
					if( CheckBoxBlocks.Checked )
					{
						_block_data[ _CHR_n ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP, 0 ) );
					}
				}
				else
				{
					tiles_data.hflip( utils.tmp_spr8x8_buff );
					tiles_data.vflip( utils.tmp_spr8x8_buff );	// VFLIP
					
					if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
					{
						if( CheckBoxBlocks.Checked )
						{
							_block_data[ _CHR_n ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
						}
					}
					else
					{
						tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP + VFLIP
						
						if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
						{
							if( CheckBoxBlocks.Checked )
							{
								_block_data[ _CHR_n ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP|utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
							}
						}
						else
						{
							if( _CHR_ind >= platform_data.get_CHR_bank_max_sprites_cnt() )
							{			
								return false;
							}
							
							if( CheckBoxBlocks.Checked )
							{
								_block_data[ _CHR_n ] = tiles_data.set_block_CHR_id( _CHR_ind, 0 );
							}

							_data.from_spr8x8_to_CHR_bank( _CHR_ind++, _CHR_buff );
						}
					}
				}
#else
				if( _CHR_ind >= platform_data.get_CHR_bank_max_sprites_cnt() )
				{			
					return false;
				}

				if( CheckBoxBlocks.Checked )
				{
					_block_data[ _CHR_n ] = tiles_data.set_block_CHR_id( _CHR_ind, 0 );
				}

				_data.from_spr8x8_to_CHR_bank( _CHR_ind++, utils.tmp_spr8x8_buff );
#endif									
			}
			
			return true;
		}
		
		void fix_CHR_inds( byte[] _arr )
		{
			for( int ind_n = 0; ind_n < _arr.Length; ind_n++ )
			{
#if DEF_NES
				_arr[ ind_n ] = ( byte )( _arr[ ind_n ] & 0x03 );
#else
				_arr[ ind_n ] = ( byte )( _arr[ ind_n ] & 0x0f );
#endif
			}
		}
		
		private void import_bmp_palette( tiles_data _data, Bitmap _bmp, int _CHR_beg_ind, int _CHR_end_ind, int _block_beg_ind, int _block_end_ind, int _tile_beg_ind, int _tile_end_ind, IProgress< string > _progress_status )
		{
			if( CheckBoxApplyPalette.Checked )
			{
				_progress_status.Report( "Palettes applying" );
				
				Color[] plt = _bmp.Palette.Entries;
#if DEF_NES
				if( fix_broken_blocks( _data, _block_beg_ind, ref _block_end_ind ) == true )
				{
					List< int[] > palettes = _data.subpalettes;
					
					int num_clrs = Math.Min( plt.Length, utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS );
					
					for( int i = 0; i < num_clrs; i++ )
					{
						palettes[ i >> 2 ][ i & 0x03 ] = utils.find_nearest_color_ind( plt[ i ].ToArgb() );
					}
					
					NES_apply_palettes( _data, _block_beg_ind, _block_end_ind );
					
					palettes[ 1 ][ 0 ] = palettes[ 2 ][ 0 ] = palettes[ 3 ][ 0 ] = palettes[ 0 ][ 0 ];
				}				
#elif DEF_FIXED_LEN_PALETTE16_ARR
#if DEF_ZX
				if( fix_broken_blocks( _data, _block_beg_ind, ref _block_end_ind ) == true )
				{
					ZX_apply_palettes( plt, _data, _block_beg_ind, _block_end_ind );
				}
#else
				if( fix_broken_blocks( _data, _block_beg_ind, ref _block_end_ind ) == true )
				{
					if( CheckBoxImportPaletteASIS.Checked )
					{
						apply_16colors_palettes_arr_as_is( plt, _data, _CHR_beg_ind, _CHR_end_ind, _block_beg_ind, _block_end_ind, _tile_beg_ind, _tile_end_ind );
					}
					else
					{
						apply_16colors_palettes_arr( plt, _data, _block_beg_ind, _block_end_ind );
					}
				}
#endif
#endif
			}
			else
			{
				fix_CHR_inds( _data.CHR_bank );
			}
		}

		private void extract_CHR( PixelFormat _pix_fmt, int _CHR_n, int _block_offset_x, int _block_offset_y, int _stride, byte[] _img_buff, System.IntPtr _data_ptr )
		{
			int CHR_offset_x;
			int CHR_offset_y;
			
			byte index_byte;
			
			byte color_index = 0;
#if DEF_NES
			bool need_remapping = CheckBoxApplyPalette.Checked ? false:true;
			SortedList< byte, byte > inds_remap_arr = new SortedList<byte, byte>();
#else
			bool need_remapping = false;
			SortedList< byte, byte > inds_remap_arr = null;
#endif			
			CHR_offset_x = _block_offset_x + ( ( _CHR_n & 0x01 ) == 0x01 ? 8:0 );
			CHR_offset_y = _block_offset_y + ( ( _CHR_n & 0x02 ) == 0x02 ? 8:0 );
			
			for( int CHR_y = 0; CHR_y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; CHR_y++ )
			{
				for( int CHR_x = 0; CHR_x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; CHR_x++ )
				{
					if( _pix_fmt == PixelFormat.Format4bppIndexed )
					{
						index_byte = Marshal.ReadByte( _data_ptr, ( CHR_offset_y + CHR_y ) * _stride + ( ( CHR_offset_x + CHR_x ) >> 1 ) );	
						
						color_index = ( byte )( ( ( CHR_x & 0x01 ) == 0x01 ) ? ( index_byte & 0x0f ):( ( index_byte & 0xf0 ) >> 4 ) );
					}
					else
					if( _pix_fmt == PixelFormat.Format8bppIndexed )
					{
						color_index = Marshal.ReadByte( _data_ptr, ( CHR_offset_y + CHR_y ) * _stride + ( CHR_offset_x + CHR_x ) );
					}

					if( need_remapping )
					{
						if( inds_remap_arr.ContainsKey( color_index ) == false )
						{
							inds_remap_arr.Add( color_index, 0 );
						}
					}

					_img_buff[ CHR_y * utils.CONST_SPR8x8_SIDE_PIXELS_CNT + CHR_x ] = color_index;
				}
			}
			
			if( need_remapping )
			{
				color_index = 0;
				
				IList< byte > ind_keys 	= inds_remap_arr.Keys;
				
				for( byte key_n = 0; key_n < ind_keys.Count; key_n++ )
				{
					inds_remap_arr[ ind_keys[ key_n ] ] = ( byte )( ( color_index++ ) & 0x03 );
				}
				
				for( int pix_n = 0; pix_n < utils.CONST_SPR8x8_SIDE_PIXELS_CNT * utils.CONST_SPR8x8_SIDE_PIXELS_CNT; pix_n++ )
				{
					_img_buff[ pix_n ] = inds_remap_arr[ _img_buff[ pix_n ] ];
				}
			}
		}
		
		private bool fix_broken_blocks( tiles_data _data, int _block_beg_ind, ref int _block_end_ind )
		{
			int block_n;
			int CHR_ind;
			
			// delete "broken" block data
			if( ( _block_end_ind & 0x03 ) != 0 )
			{
				int blocks_CHR_n;
				int blocks_CHR_cnt = _block_end_ind - ( _block_end_ind & 0x03 );
				
				Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
				
				for( block_n = _block_end_ind; block_n > blocks_CHR_cnt; block_n-- )
				{
					CHR_ind = tiles_data.get_block_CHR_id( _data.blocks[ block_n - 1 ] );
					
					for( blocks_CHR_n = _block_beg_ind; blocks_CHR_n < blocks_CHR_cnt; blocks_CHR_n++ )
					{
						if( tiles_data.get_block_CHR_id( _data.blocks[ blocks_CHR_n ] ) == CHR_ind )
						{
							break;
						}
					}
					
					if( blocks_CHR_n == blocks_CHR_cnt )
					{
						_data.from_spr8x8_to_CHR_bank( CHR_ind, utils.tmp_spr8x8_buff );
						
						_data.blocks[ block_n - 1 ] = 0;
					}
				}
				
				_block_end_ind &= ~0x03; 
			}

			if( ( _block_end_ind - _block_beg_ind ) >> 2 == 0 )
			{
				MainForm.message_box( "No data to import!\nCan't apply palettes!", "Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				
				return false;
			}
			
			return true;
		}
#if DEF_NES
		private void NES_apply_palettes( tiles_data _data, int _block_beg_ind, int _block_end_ind )
		{
			int block_n;
			int CHR_n;
			int ind_n;
			int plt_n;
			
			int plt1_n;
			int plt2_n;

			byte clr_ind;
			int CHR_ind;
			int block_ind;
			
			bool more_than_4_palettes 			= false;
			bool more_than_4_colors_in_palette 	= false;

			int max_weight 		= -1;
			int transp_clr_ind	= -1;
			
			int n_blocks = ( _block_end_ind - _block_beg_ind ) >> 2;
			
			int[] clr_inds = new int[ utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ];
			Array.Clear( clr_inds, 0, clr_inds.Length );

			List< byte > block_plt_inds = new List< byte >( n_blocks );
			SortedSet< int > plt_clr_inds = null;
			
			List< SortedSet< int > > palettes = new List< SortedSet< int > >( 100 );
			
			bool[] remapped_CHRs = new bool[ platform_data.get_CHR_bank_max_sprites_cnt() ];
			Array.Clear( remapped_CHRs, 0, remapped_CHRs.Length );
			
			string invalid_data_msg = "";
			
			// run through blocks
			for( block_n = _block_beg_ind; block_n < _block_end_ind; block_n += utils.CONST_BLOCK_SIZE )
			{
				plt_clr_inds = new SortedSet< int >();
					
				for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
				{
					_data.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( _data.blocks[ block_n + CHR_n ] ), utils.tmp_spr8x8_buff );
					
					for( ind_n = 0; ind_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; ind_n++ )
					{
						clr_ind = ( byte )utils.tmp_spr8x8_buff[ ind_n ];
						
						if( !plt_clr_inds.Contains( clr_ind ) )
						{
							plt_clr_inds.Add( clr_ind );
							
							if( plt_clr_inds.Count > 4 )
							{
								invalid_data_msg += utils.hex( "$", block_n >> 2 ) + " | CHR: " + utils.hex( "$", CHR_n ) + " | pix: " + ind_n + "\n";
							}
						}
					}
				}
				
				for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
				{
					if( palettes[ plt_n ].IsSupersetOf( plt_clr_inds ) )
					{
						block_plt_inds.Add( ( byte )plt_n );
						
						break;
					}
				}
				
				if( plt_n == palettes.Count )
				{
					palettes.Add( plt_clr_inds );
					
					block_plt_inds.Add( ( byte )( palettes.Count - 1 ) );
				}
			}
			
			for( plt1_n = 0; plt1_n < palettes.Count; plt1_n++ )
			{
				for( plt2_n = plt1_n; plt2_n < palettes.Count; plt2_n++ )
				{
					if( plt1_n != plt2_n )
					{
/*						if( palettes[ plt1_n ].IsSupersetOf( palettes[ plt2_n ] ) )
						{
							// remove palettes[ plt2_n ]
							palettes.RemoveAt( plt2_n );
							
							for( ind_n = 0; ind_n < block_plt_inds.Count; ind_n++ )
							{
								if( block_plt_inds[ ind_n ] == plt2_n )
								{
									block_plt_inds[ ind_n ] = ( byte )plt1_n;
								}
								
								if( block_plt_inds[ ind_n ] >= plt2_n )
								{
									--block_plt_inds[ ind_n ];
								}
							}
							
							--plt2_n;
						}
						else
*/						if( palettes[ plt2_n ].IsSupersetOf( palettes[ plt1_n ] ) )
						{
							// remove palettes[ plt1_n ]
							palettes.RemoveAt( plt1_n );
							
							for( ind_n = 0; ind_n < block_plt_inds.Count; ind_n++ )
							{
								if( block_plt_inds[ ind_n ] == plt1_n )
								{
									block_plt_inds[ ind_n ] = ( byte )plt2_n;
								}
								
								if( block_plt_inds[ ind_n ] >= plt1_n )
								{
									--block_plt_inds[ ind_n ];
								}
							}

							--plt1_n;
							
							break;
						}
					}
				}
			}
			
			// get a transparent color index
			{
				// calc color weights
				for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
				{
					palettes[ plt_n ].ForEach( delegate( int _val ) { ++clr_inds[ _val ]; });
				}			
				
				// get max weight index
				for( ind_n = 0; ind_n < clr_inds.Length; ind_n++ )
				{
					if( clr_inds[ ind_n ] > max_weight )
					{
						max_weight = clr_inds[ ind_n ];
						
						transp_clr_ind = ind_n;						
					}
				}
			}

			if( palettes.Count > utils.CONST_NUM_SMALL_PALETTES )
			{
				// remove palettes without a transparent color
				for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
				{
					if( !palettes[ plt_n ].Contains( ( byte )transp_clr_ind ) )
					{
						for( block_n = 0; block_n < n_blocks; block_n++ )
						{
							if( block_plt_inds[ block_n ] >= plt_n )
							{
								if( block_plt_inds[ block_n ] != 0 )
								{
									--block_plt_inds[ block_n ];
								}
							}
						}
						
						palettes.RemoveAt( plt_n );
						--plt_n;
					}
				}
			}

			more_than_4_palettes = palettes.Count > utils.CONST_NUM_SMALL_PALETTES ? true:false;
			
			// check colors overflow
			for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
			{
				if( palettes[ plt_n ].Count > utils.CONST_PALETTE_SMALL_NUM_COLORS )
				{
					more_than_4_colors_in_palette = true;
				}
			}
			
			if( more_than_4_palettes || more_than_4_colors_in_palette )
			{
				// convert palettes array into string format
				{
					string plt_str = "Palettes:";
					
					for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
					{
						plt_clr_inds = palettes[ plt_n ];
						
						plt_str += "\n" + ( plt_n + 1 ) + ": [ ";
						
						plt_clr_inds.ForEach( delegate( int _val ) 
						{
							plt_str += _val.ToString() + " ";
						});
						
						plt_str += "]";
					}
					
					if( invalid_data_msg.Length > 0 )
					{
						invalid_data_msg = invalid_data_msg.Insert( 0, "\n\nBlocks:\n" );
					}
					
					invalid_data_msg = invalid_data_msg.Insert( 0, plt_str );
				}
			}
			
			// remove transparent color inds
			for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
			{
				palettes[ plt_n ].Remove( ( byte )transp_clr_ind );
			}

			// CHRs remapping and palettes applying
			for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
			{
				// build remapping table
//				Array.Clear( clr_inds, 0, clr_inds.Length );
				for( ind_n = 0; ind_n < clr_inds.Length; ind_n++ ) { clr_inds[ind_n ] = 3; }
				
				clr_inds[ transp_clr_ind ] = 0;

				plt_clr_inds = palettes[ plt_n & 0x03 ];	// cut palettes more than 4
				
				ind_n = 0;
				plt_clr_inds.ForEach( delegate( int _val ) 
				{
				    if( ind_n < 3 )	// cut color indices more than 4
				    {
						clr_inds[ _val ] = ( byte )( ++ind_n );
				    }
				});
				
				// run through all blocks and remap inds belonging to a current palette
				for( block_n = 0; block_n < n_blocks; block_n++ )
				{
					if( block_plt_inds[ block_n ] == plt_n )
					{
						for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
						{
							block_ind	= _block_beg_ind + ( block_n << 2 ) + CHR_n;
							CHR_ind		= tiles_data.get_block_CHR_id( _data.blocks[ block_ind ] );
							
							if( !remapped_CHRs[ CHR_ind ] )
							{
								_data.from_CHR_bank_to_spr8x8( CHR_ind, utils.tmp_spr8x8_buff );
								
								for( ind_n = 0; ind_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; ind_n++ )
								{
									utils.tmp_spr8x8_buff[ ind_n ] = ( byte )clr_inds[ utils.tmp_spr8x8_buff[ ind_n ] ];
								}
								
								_data.from_spr8x8_to_CHR_bank( CHR_ind, utils.tmp_spr8x8_buff );
								
								remapped_CHRs[ CHR_ind ] = true;
							}
							
							// apply palette
							_data.blocks[ block_ind ] = tiles_data.set_block_flags_palette( plt_n, _data.blocks[ block_ind ] );
						}
					}
				}
			}
			
			fix_unremapped_CHRs( _data, remapped_CHRs );
			
			// apply final palettes
			Array.Clear( clr_inds, 0, clr_inds.Length );
			
			int palettes_cnt = Math.Min( palettes.Count, utils.CONST_NUM_SMALL_PALETTES );
			
			for( plt_n = 0; plt_n < palettes_cnt; plt_n++ )
			{
				plt_clr_inds = palettes[ plt_n ];	// cut palettes more than 4
				
				clr_inds[ plt_n << 2 ] = _data.subpalettes[ transp_clr_ind >> 2 ][ transp_clr_ind & 0x03 ];

				ind_n = 0;
				plt_clr_inds.ForEach( delegate( int _val ) 
				{ 
					if( ind_n < 3 )	// cut palette colors more than 4
					{
						clr_inds[ ( plt_n << 2 ) + ( ++ind_n ) ] = _data.subpalettes[ _val >> 2 ][ _val & 0x03 ];
					}
				});
			}
			
			for( ind_n = 0; ind_n < clr_inds.Length; ind_n++ )
			{
				_data.subpalettes[ ind_n >> 2 ][ ind_n & 0x03 ] = clr_inds[ ind_n ];
			}
			
			if( more_than_4_colors_in_palette || more_than_4_palettes )
			{
				string reason_str = ( more_than_4_palettes ? "\n- more than 4 palettes":"" ) + ( more_than_4_colors_in_palette ? "\n- more than 4 colors in a palette":"" );
				
				MainForm.message_box( "The imported image doesn't meet the requirements!\nSome color information will be lost!\n\nREASON: " + reason_str, "NES Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				
				MainForm.message_box( "Invalid data:\n\n" + invalid_data_msg, "NES Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
#endif //DEF_NES
#if DEF_FIXED_LEN_PALETTE16_ARR
#if DEF_ZX
		private void ZX_apply_palettes( Color[] _plt, tiles_data _data, int _block_beg_ind, int _block_end_ind )
		{
			int				block_n;
			uint			block_data;
			int				chr_id;
			int				pix_n;
			byte 			paper_clr;
			byte 			ink_clr;
			byte			max_pix_val_ind;
			int				pure_paper_clr;
			int				pure_ink_clr;
			byte			alt_ink_clr;
			bool			ink_paper_equal; 
			
			byte[] grid_pattern = new byte[64]{ 0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0,
												0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0,
												0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0,
												0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0 };
			
			
			Dictionary< int, int >	palette_inds	= new Dictionary< int, int >( platform_data.get_main_palette_colors_cnt() );
			Dictionary< byte, int >	pix_value		= new Dictionary< byte, int >( platform_data.get_main_palette_colors_cnt() );
			byte[]					chr_id_flags	= null;
			
			const byte CONST_FLAG_USED_CHR	= 0x01;
			const byte CONST_FLAG_BRIGHT	= 0x02;

			// convert colors to the local palette indices
			for( int ind_n = 0; ind_n < _plt.Length; ind_n++ )
			{
				palette_inds[ ind_n ] = utils.find_nearest_color_ind( _plt[ ind_n ].ToArgb() );
			}
			
			chr_id_flags = new byte[ platform_data.get_CHR_bank_max_sprites_cnt() ];
			Array.Clear( chr_id_flags, 0, chr_id_flags.Length );
			
			for( block_n = _block_beg_ind; block_n < _block_end_ind; block_n++ )
			{
				block_data = _data.blocks[ block_n ];
				
				chr_id = tiles_data.get_block_CHR_id( block_data );
				
				if( ( chr_id_flags[ chr_id ] & CONST_FLAG_USED_CHR ) == 0 )
				{
					chr_id_flags[ chr_id ] |= CONST_FLAG_USED_CHR;
					
					// collect pixel weights data
					_data.CHR_bank_spr8x8_change_proc( chr_id, delegate( byte _pix )
					{
						byte new_clr = ( byte )palette_inds[ ( int )_pix ];

						if( !pix_value.ContainsKey( new_clr ) )
						{
							pix_value[ new_clr ] = 1;
						}
						else
						{
							++pix_value[ new_clr ];
						}

						return new_clr; 
					});
					
					// get paper color ( max pixel value )
					{
						max_pix_val_ind = ( byte )pix_value.Values.ToList().IndexOf( pix_value.Values.Max() );

						paper_clr	= ( byte )pix_value.Keys.ToList()[ max_pix_val_ind ];
						pix_value.Remove( paper_clr );
					}
					
					// get ink color ( the next max pixel value )
					{
						if( pix_value.Count > 0 )
						{
							max_pix_val_ind = ( byte )pix_value.Values.ToList().IndexOf( pix_value.Values.Max() );
							ink_clr	= ( byte )pix_value.Keys.ToList()[ max_pix_val_ind ];
						}
						else
						{
							ink_clr	= 0xff;
						}
					}

					// check bright color
					{
						if( ( ( ink_clr != 0xff ) && ( ( ink_clr & 0x08 ) != 0 ) ) || ( ( paper_clr & 0x08 ) != 0 ) )
						{
							_data.blocks[ block_n ] = tiles_data.set_block_flags_palette( 1, block_data );
							
							chr_id_flags[ chr_id ] |= CONST_FLAG_BRIGHT;
						}
					}
					
					// check if paper color is equal to ink one
					{
						pure_paper_clr	= paper_clr & 0x07;
						pure_ink_clr	= ink_clr & 0x07;
						alt_ink_clr		= 0;
						ink_paper_equal	= false; 
						
						if( pure_paper_clr == pure_ink_clr )
						{
							if( pure_ink_clr == 0 || pure_ink_clr == 0x07 )
							{
								alt_ink_clr = ( byte )( pure_ink_clr ^ 0x07 );
							}
								
							ink_paper_equal = true;
						}
					}
					
					pix_n = 0;
					
					// fix CHR to 2 colors
					_data.CHR_bank_spr8x8_change_proc( chr_id, delegate( byte _pix )
					{
						if( _pix == paper_clr )
						{
							_pix |= 0x08;	// the second 8 colors are paper ones
						}
						else
						if( ink_clr != 0xff )
						{
							if( _pix != ink_clr )
							{
								if( grid_pattern[ pix_n ] != 0 )
								{
									if( ink_paper_equal )
									{
										_pix = alt_ink_clr;
									}
									else
									{
										_pix = ( byte )( ink_clr & 0x07 );	// reset paper flag
									}
								}
								else
								{
									// apply grid to mid colors
									_pix = ( byte )( 0x08 | paper_clr );
								}
							}
							else
							{
								if( ink_paper_equal )
								{
									_pix = alt_ink_clr;
								}
								else
								{
									_pix = ( byte )( ink_clr & 0x07 );	// reset paper flag
								}
							}
						}

						++pix_n;
						
						return _pix; 
					});
					
					pix_value.Clear();
				}
				else
				{
					if( ( chr_id_flags[ chr_id ] & CONST_FLAG_BRIGHT ) != 0 )
					{
						_data.blocks[ block_n ] = tiles_data.set_block_flags_palette( 1, block_data );
					}
				}
			}
				
			palette_inds.Clear();
		}
#else
		private void apply_16colors_palettes_arr_as_is( Color[] _plt, tiles_data _data, int _CHR_beg_ind, int _CHR_end_ind, int _block_beg_ind, int _block_end_ind, int _tile_beg_ind, int _tile_end_ind )
		{
			int block_n;
			int CHR_n;
			int CHR_ind;
			int ind_n;
			int plt_n;
			int plt_ind;
			int plt_max = -1;
			int pix_n;
			int palettes_cnt;
			int clr_ind;
			
			byte pix_val;
			
			uint block_data;

			palette16_data plt16;

			int[] CHRs_plt = new int[ platform_data.get_CHR_bank_max_sprites_cnt() ];
			Array.Clear( CHRs_plt, 0, CHRs_plt.Length );
			
			bool[] used_CHRs = new bool[ platform_data.get_CHR_bank_max_sprites_cnt() ];
			Array.Clear( used_CHRs, 0, used_CHRs.Length );
			
			// run through blocks and CHRs
			for( block_n = _block_beg_ind; block_n < _block_end_ind; block_n += utils.CONST_BLOCK_SIZE )
			{
				for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
				{
					block_data = _data.blocks[ block_n + CHR_n ];
					
					CHR_ind = tiles_data.get_block_CHR_id( block_data );
					
					if( used_CHRs[ CHR_ind ] == false )
					{
						plt_ind	= -1;
						pix_n	= 0;
						
						_data.CHR_bank_spr8x8_change_proc( CHR_ind, delegate( byte _pix ) 
						{
							plt_n = _pix >> 4;
							
							pix_val = ( byte )( _pix - ( plt_n << 4 ) );

							if( pix_val != 0 )
							{
								if( plt_max < plt_n )
								{
									plt_max = plt_n;
								}
							
								if( plt_ind < 0 )
								{
									plt_ind = plt_n;
								}
								else
								{
									if( plt_n != plt_ind )
									{
										reset_CHRs_tiles_data( _data, _CHR_beg_ind, _CHR_end_ind, _block_beg_ind, _block_end_ind, _tile_beg_ind, _tile_end_ind );
										
										throw new Exception( "The input image doesn't contain an array of ordered 16-color palettes!\n\nPlease, uncheck the 'Import palette 'AS IS'' flag and try again." );
									}
								}
							}
							
							++pix_n;
							
							return pix_val;
						});
						
						plt_ind = ( plt_ind < 0 ) ? 0:plt_ind;
						
						CHRs_plt[ CHR_ind ]		= plt_ind;  
						used_CHRs[ CHR_ind ]	= true;
					}
					else
					{
						plt_ind = CHRs_plt[ CHR_ind ];
					}
					
					block_data = tiles_data.set_block_flags_palette( Math.Min( platform_data.get_fixed_palette16_cnt(), plt_ind ), block_data );
					_data.blocks[ block_n + CHR_n ] = block_data;
				}
			}

			// apply palettes
			palettes_cnt = Math.Min( platform_data.get_fixed_palette16_cnt(), plt_max + 1 );
			
			for( plt_n = 0; plt_n < palettes_cnt; plt_n++ )
			{
				plt16 = _data.palettes_arr[ plt_n ];
				
				for( ind_n = 0; ind_n < 16; ind_n++ )
				{
					clr_ind = ( plt_n << 4 ) + ind_n;
					
					if( clr_ind < _plt.Length )
					{
						plt16.subpalettes[ ind_n >> 2 ][ ind_n & 0x03 ] = utils.find_nearest_color_ind( _plt[ clr_ind ].ToArgb() );
					}
				}
			}
		}
		
		private void reset_CHRs_tiles_data( tiles_data _data, int _CHR_beg_ind, int _CHR_end_ind, int _block_beg_ind, int _block_end_ind, int _tile_beg_ind, int _tile_end_ind )
		{
			int block_n;
			int tile_n;
			int CHR_n;
			
			for( block_n = _block_beg_ind; block_n < _block_end_ind; block_n++ )
			{
				_data.blocks[ block_n ] = 0;
			}
			
			for( CHR_n = _CHR_beg_ind; CHR_n < _CHR_end_ind; CHR_n++ )
			{
				_data.CHR_bank_spr8x8_change_proc( CHR_n, delegate( byte _pix ) 
				{
					return 0;
				});
			}
			
			if( _tile_beg_ind >= 0 )
			{
				for( tile_n = _tile_beg_ind; tile_n < _tile_end_ind; tile_n++ )
				{
					_data.tiles[ tile_n ] = 0;
				}
			}
		}
		
		private void apply_16colors_palettes_arr( Color[] _plt, tiles_data _data, int _block_beg_ind, int _block_end_ind )
		{
			int block_n;
			int CHR_n;
			int ind_n;
			int plt_n;
			int val;
			
			int plt1_n;
			int plt2_n;

			byte clr_ind;
			int CHR_ind;
			int block_ind;
			
			int transp_clr_ind = -1;
			int transp_clr_pos = -1;
			
			int palettes_cnt;
			
			palette16_data plt16;
			
			bool more_than_max_palettes 		= false;
			bool more_than_16_colors_in_palette = false;

			int n_CHRs		= _block_end_ind - _block_beg_ind;
			int n_blocks	= n_CHRs >> 2;
			
			int[] clr_inds = new int[ platform_data.get_main_palette_colors_cnt() ];
			Array.Clear( clr_inds, 0, clr_inds.Length );

			List< byte > CHR_plt_inds = new List< byte >( n_CHRs );
			SortedSet< int > plt_clr_inds = null;
			
			List< SortedSet< int > > palettes = new List< SortedSet< int > >( 256 );
			
			bool[] remapped_CHRs = new bool[ platform_data.get_CHR_bank_max_sprites_cnt() ];
			Array.Clear( remapped_CHRs, 0, remapped_CHRs.Length );
			
			string invalid_data_msg = "";
			
			// convert colors to the local palette indices
			int plt_clrs = _plt.Length;
			int[] img_clr_inds = new int[ _plt.Length ];
			
			for( ind_n = 0; ind_n < plt_clrs; ind_n++ )
			{
				img_clr_inds[ ind_n ] = utils.find_nearest_color_ind( _plt[ ind_n ].ToArgb() );
			}
			
			// run through blocks and CHRs
			for( block_n = _block_beg_ind; block_n < _block_end_ind; block_n += utils.CONST_BLOCK_SIZE )
			{
				for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
				{
					plt_clr_inds = new SortedSet< int >();
					
					_data.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( _data.blocks[ block_n + CHR_n ] ), utils.tmp_spr8x8_buff );
					
					for( ind_n = 0; ind_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; ind_n++ )
					{
						clr_ind = ( byte )utils.tmp_spr8x8_buff[ ind_n ];
						
						if( !plt_clr_inds.Contains( clr_ind ) )
						{
							plt_clr_inds.Add( clr_ind );
							
							if( plt_clr_inds.Count > 16 )
							{
								invalid_data_msg += utils.hex( "$", block_n >> 2 ) + " | CHR: " + utils.hex( "$", CHR_n ) + " | pix: " + ind_n + "\n";
							}
						}
					}
					
					for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
					{
						if( palettes[ plt_n ].IsSupersetOf( plt_clr_inds ) )
						{
							CHR_plt_inds.Add( ( byte )plt_n );
							
							break;
						}
					}
					
					if( plt_n == palettes.Count )
					{
						palettes.Add( plt_clr_inds );
						
						CHR_plt_inds.Add( ( byte )( palettes.Count - 1 ) );
					}
				}
			}

			// palettes packing
			{
				SortedSet< int > tmp_palette = new SortedSet<int>();
				
				// remove supersets
				for( plt1_n = 0; plt1_n < palettes.Count; plt1_n++ )
				{
					for( plt2_n = plt1_n; plt2_n < palettes.Count; plt2_n++ )
					{
						if( plt1_n != plt2_n )
						{
							if( palettes[ plt2_n ].IsSupersetOf( palettes[ plt1_n ] ) )
							{
								// remove palettes[ plt1_n ]
								palettes.RemoveAt( plt1_n );
								
								for( ind_n = 0; ind_n < CHR_plt_inds.Count; ind_n++ )
								{
									if( CHR_plt_inds[ ind_n ] == plt1_n )
									{
										CHR_plt_inds[ ind_n ] = ( byte )plt2_n;
									}
									
									if( CHR_plt_inds[ ind_n ] >= plt1_n )
									{
										--CHR_plt_inds[ ind_n ];
									}
								}
	
								--plt1_n;
								
								break;
							}
						}
					}
				}

				// unite the rest
				while( get_best_union_palettes_pair( palettes, out plt1_n, out plt2_n ) == true )
				{
					palettes[ plt2_n ].UnionWith( palettes[ plt1_n ] );
					
					palettes.RemoveAt( plt1_n );
					
					for( ind_n = 0; ind_n < CHR_plt_inds.Count; ind_n++ )
					{
						if( CHR_plt_inds[ ind_n ] == plt1_n )
						{
							CHR_plt_inds[ ind_n ] = ( byte )plt2_n;
						}
						
						if( CHR_plt_inds[ ind_n ] >= plt1_n )
						{
							--CHR_plt_inds[ ind_n ];
						}
					}
				}
			}

			// get a transparent color
			palettes_cnt = palettes.Count;
			
			if( palettes_cnt > 0 )
			{
				SortedSet< int > plt_a = new SortedSet<int>();
				SortedSet< int > plt_b = new SortedSet<int>();
				
				// get color index belonging to most palettes
				{
					for( plt_n = 0; plt_n < palettes_cnt; plt_n++ )
					{
						plt_a.UnionWith( palettes[ plt_n ] );
					}
					
					int ind_cnt;
					int max_cnt = int.MinValue;
					int max_val = -1;
					
					plt_a.ForEach( delegate( int _val )
					{
						ind_cnt = 0;
						
						palettes.ForEach( delegate( SortedSet< int > _palette )
						{
	  	                 	if( _palette.Contains( _val ) )
	  	                 	{
	  	                 		++ind_cnt;
	  	                 	}
						});
						
						if( ind_cnt > max_cnt )
						{
							max_cnt = ind_cnt;
							max_val = _val;
						}
					});
					
					if( max_cnt < palettes_cnt )
					{
						if( max_cnt > ( palettes_cnt >> 1 ) )
						{
							// add missing transparent color value
							palettes.ForEach( delegate( SortedSet< int > _palette )
							{
								if( !_palette.Contains( max_val ) && _palette.Count < 16 )
		  	                 	{
		  	                 		_palette.Add( max_val );
		  	                 	}
							});
						}
					}
				}
				
				plt_a.Clear();
				plt_b.Clear();
				
				palettes[ 0 ].ForEach( delegate( int _val ) { plt_a.Add( _val ); });
				
				for( plt_n = 1; plt_n < palettes.Count; plt_n++ )
				{
					plt_b.Clear();
					palettes[ plt_n ].ForEach( delegate( int _val ) { plt_b.Add( _val ); });
					
					plt_a.IntersectWith( plt_b );
				}
				
				// get min color index
				if( plt_a.Count > 0 )
				{
					int min_index = int.MaxValue;
					clr_ind = 0;
					plt_a.ForEach( delegate( int _val ) 
					{
						if( min_index > img_clr_inds[ _val ] )
						{
							min_index = _val;
						}
						++clr_ind;
					});
					
					transp_clr_ind	= min_index;
				}
				else
				{
					MainForm.message_box( "Can't find a transparent color!\nPlease, fix it manually.", "Palettes Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}
			
			more_than_max_palettes = ( palettes.Count > platform_data.get_fixed_palette16_cnt() );
			
			// check colors overflow
			for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
			{
				if( palettes[ plt_n ].Count > ( utils.CONST_PALETTE_SMALL_NUM_COLORS * utils.CONST_NUM_SMALL_PALETTES ) )
				{
					more_than_16_colors_in_palette = true;
				}
			}
			
			if( more_than_max_palettes || more_than_16_colors_in_palette )
			{
				// convert palettes array into string format
				{
					string plt_str = "Palettes:";
					
					for( plt_n = 0; plt_n < palettes.Count; plt_n++ )
					{
						plt_clr_inds = palettes[ plt_n ];
						
						plt_str += "\n" + ( plt_n + 1 ) + ": [ ";
						
						plt_clr_inds.ForEach( delegate( int _val ) 
						{
							plt_str += _val.ToString() + " ";
						});
						
						plt_str += "]";
					}
					
					if( invalid_data_msg.Length > 0 )
					{
						invalid_data_msg = invalid_data_msg.Insert( 0, "\n\nBlocks:\n" );
					}
					
					invalid_data_msg = invalid_data_msg.Insert( 0, plt_str );
				}
			}
			
			// CHRs remapping and palettes applying
			palettes_cnt = Math.Min( palettes.Count, platform_data.get_fixed_palette16_cnt() );
			
			for( plt_n = 0; plt_n < palettes_cnt; plt_n++ )
			{
				// build remapping table
				Array.Clear( clr_inds, 0, clr_inds.Length );
				
				plt_clr_inds = palettes[ plt_n ];
				
				ind_n = 0;
				plt_clr_inds.ForEach( delegate( int _val ) 
				{
				    if( ind_n < 16 )	// cut color indices more than 16
				    {
						clr_inds[ _val ] = ( byte )( ind_n++ );
				    }
				});

				if( transp_clr_ind >= 0 )
				{
					val = clr_inds[ plt_clr_inds.Min ];
					clr_inds[ plt_clr_inds.Min ]	= clr_inds[ transp_clr_ind ];
					clr_inds[ transp_clr_ind ]		= val;
				}
				
				// run through all blocks and remap inds belonging to a current palette
				for( block_n = 0; block_n < n_blocks; block_n++ )
				{
					for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
					{
						if( CHR_plt_inds[ ( block_n << 2 ) + CHR_n ] == plt_n )
						{
							block_ind	= _block_beg_ind + ( block_n << 2 ) + CHR_n;
							CHR_ind		= tiles_data.get_block_CHR_id( _data.blocks[ block_ind ] );
							
							if( !remapped_CHRs[ CHR_ind ] )
							{
								_data.from_CHR_bank_to_spr8x8( CHR_ind, utils.tmp_spr8x8_buff );
								
								for( ind_n = 0; ind_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; ind_n++ )
								{
									utils.tmp_spr8x8_buff[ ind_n ] = ( byte )clr_inds[ utils.tmp_spr8x8_buff[ ind_n ] ];
								}
								
								_data.from_spr8x8_to_CHR_bank( CHR_ind, utils.tmp_spr8x8_buff );
								
								remapped_CHRs[ CHR_ind ] = true;
							}
							
							// apply palette
							_data.blocks[ block_ind ] = tiles_data.set_block_flags_palette( plt_n, _data.blocks[ block_ind ] );
						}
					}
				}
			}

			fix_unremapped_CHRs( _data, remapped_CHRs );

			// apply final palettes
			palettes_cnt = Math.Min( palettes.Count, platform_data.get_fixed_palette16_cnt() );
			
			for( plt_n = 0; plt_n < palettes_cnt; plt_n++ )
			{
				plt_clr_inds = palettes[ plt_n ];

				plt16 = _data.palettes_arr[ plt_n ];

				ind_n = 0;
				plt_clr_inds.ForEach( delegate( int _val ) 
				{ 
					if( ind_n < 16 )	// cut palette colors more than 16
					{
						plt16.subpalettes[ ind_n >> 2 ][ ind_n & 0x03 ] = img_clr_inds[ _val ];
						
						if( _val == transp_clr_ind )
						{
							transp_clr_pos = ind_n;
						}
						
						++ind_n;
					}
				});
				
				if( transp_clr_ind >= 0 )
				{
					val = plt16.subpalettes[ 0 ][ 0 ];
					plt16.subpalettes[ 0 ][ 0 ]	= plt16.subpalettes[ transp_clr_pos >> 2 ][ transp_clr_pos & 0x03 ];
					plt16.subpalettes[ transp_clr_pos >> 2 ][ transp_clr_pos & 0x03 ]	= val;
				}
			}
			
			if( more_than_16_colors_in_palette || more_than_max_palettes )
			{
				string reason_str = ( more_than_max_palettes ? "\n- more than " + platform_data.get_fixed_palette16_cnt() + " palettes":"" ) + ( more_than_16_colors_in_palette ? "\n- more than 16 colors in a palette":"" );
				
				MainForm.message_box( "The imported image doesn't meet the requirements!\nSome color information will be lost!\n\nREASON: " + reason_str, "Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				
				MainForm.message_box( "Invalid data:\n\n" + invalid_data_msg, "Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
		
		bool get_best_union_palettes_pair( List< SortedSet< int > > _palettes, out int _plt1_n, out int _plt2_n )
		{
			SortedSet< int > tmp_palette = new SortedSet<int>();
			
			int min_union	= int.MinValue;
			int union_diff;

			_plt1_n = -1;
			_plt2_n = -1;
			
			int i;
			int j;
			
			for( i = 0; i < _palettes.Count; i++ )
			{
				for( j = i; j < _palettes.Count; j++ )
				{
					if( i != j )
					{
						tmp_palette.Clear();
						tmp_palette.UnionWith( _palettes[ i ] );
						tmp_palette.UnionWith( _palettes[ j ] );
						
						if( tmp_palette.Count <= 16 )
						{
							union_diff = ( _palettes[ i ].Count + _palettes[ j ].Count ) - tmp_palette.Count;
							
							if( union_diff > min_union )
							{
								min_union = union_diff;
								
								_plt1_n = i;
								_plt2_n = j;
							}
						}
					}
				}
			}
			
			return ( min_union == int.MinValue ) ? false:true;
		}
#endif //!DEF_ZX
#endif //DEF_FIXED_LEN_PALETTE16_ARR
		void BtnApplyPaletteDescClick_Event(object sender, EventArgs e)
		{
#if DEF_NES
			MainForm.message_box( "For best results, an importing image must meets the following requirements:\n\n- NES compatible graphics\n- no more than 13 colors\n- one CHR bank data\n- static palette\n- tiles aligned", "Automatic Applying of Palettes", MessageBoxButtons.OK, MessageBoxIcon.Information );
#elif DEF_FIXED_LEN_PALETTE16_ARR
			MainForm.message_box( "For best results, an importing image must meets the following requirements:\n\n- " + platform_data.CONST_PLATFORM + " compatible graphics\n- colors are not duplicated in a palette\n- tiles aligned", "Automatic Applying of Palettes", MessageBoxButtons.OK, MessageBoxIcon.Information );
#endif
		}

		void BtnImportPaletteASISDescClick_Event(object sender, EventArgs e)
		{
			MainForm.message_box( "Checked: For indexed images with an ordered palettes array of 16-colors. So prepared palettes will be imported 'AS IS'.\n\nUnchecked: For indexed images with unordered palette colors. The whole palette will be optimized to minimize the number of 16-color palettes and colors, if it's possible.", "Applying of Image Palette", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		void fix_unremapped_CHRs( tiles_data _data, bool[] _remapped_CHRs )
		{
			// fix unremapped CHR's to avoid errors
			int max_CHRs_cnt = _data.get_first_free_spr8x8_id();			
			max_CHRs_cnt = ( max_CHRs_cnt < 0 ) ? platform_data.get_CHR_bank_max_sprites_cnt():max_CHRs_cnt;
			
			for( int CHR_n = 0; CHR_n < max_CHRs_cnt; CHR_n++ )
			{
				if( !_remapped_CHRs[ CHR_n ] )
				{
					_data.from_CHR_bank_to_spr8x8( CHR_n, utils.tmp_spr8x8_buff );

					fix_CHR_inds( utils.tmp_spr8x8_buff );
					
					_data.from_spr8x8_to_CHR_bank( CHR_n, utils.tmp_spr8x8_buff );
					
					_remapped_CHRs[ CHR_n ] = true;
				}
			}
		}
	}
}
