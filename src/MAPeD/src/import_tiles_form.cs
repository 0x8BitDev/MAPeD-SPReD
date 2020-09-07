/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 14.12.2018
 * Time: 19:38
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MAPeD
{
	/// <summary>
	/// Description of import_tiles_form.
	/// </summary>
	public partial class import_tiles_form : Form
	{
		public bool import_tiles
		{
			get { return CheckBoxTiles.Checked; }
			set {}
		}

		public bool import_game_level
		{
			get { return CheckBoxGameLevel.Checked; }
			set {}
		}
		
		public bool delete_empty_screens
		{
			get { return CheckBoxDeleteEmptyScreens.Checked; }
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
#if DEF_SMS
			BtnApplyPaletteDesc.Visible = false;
			CheckBoxApplyPalette.Text = "Apply palette";
#endif
			CheckBoxApplyPalette.Checked = true;
		}
		
		void CheckBoxGameLevelChanged_Event(object sender, EventArgs e)
		{
			CheckBoxTiles.Checked = CheckBoxGameLevel.Checked ? true:CheckBoxTiles.Checked;
			CheckBoxDeleteEmptyScreens.Enabled = !( CheckBoxTiles.Enabled = CheckBoxGameLevel.Checked ? false:true );
			CheckBoxDeleteEmptyScreens.Checked = CheckBoxDeleteEmptyScreens.Enabled ? CheckBoxDeleteEmptyScreens.Checked:false;
		}

		void CheckBoxApplyPaletteChanged_Event(object sender, EventArgs e)
		{
#if DEF_NES			
			CheckBoxSkipZeroCHRBlock.Enabled = !CheckBoxApplyPalette.Checked;
			CheckBoxSkipZeroCHRBlock.Checked = CheckBoxSkipZeroCHRBlock.Enabled ? CheckBoxSkipZeroCHRBlock.Checked:false;
#endif			
		}
		
		private int get_local_scr_ind( int _global_scr_ind, data_sets_manager _data_manager )
		{
			return _data_manager.get_local_screen_ind( _data_manager.tiles_data_pos, _global_scr_ind );
		}
		
		public void data_processing( Bitmap _bmp, data_sets_manager _data_manager, Func< int, int, layout_data > _create_layout )
		{
			bool import_game_level_as_is = true;

			if( import_tiles )
			{
#if DEF_SCREEN_HEIGHT_7d5_TILES
				if( ( !import_game_level && ( _bmp.Width > 0 && ( _bmp.Width % 32 ) == 0 ) && ( _bmp.Height > 0 && ( _bmp.Height % 32 ) == 0 ) ) ||
					( import_game_level && ( _bmp.Width > 0 && ( _bmp.Width % 32 ) == 0 ) && ( _bmp.Height > 0 && ( _bmp.Height % utils.CONST_SCREEN_HEIGHT_PIXELS ) == 0 ) ) )
#else
				if( ( _bmp.Width > 0 && ( _bmp.Width % 32 ) == 0 ) && ( _bmp.Height > 0 && ( _bmp.Height % 32 ) == 0 ) )
#endif
				{
					if( import_game_level )
					{
						if( ( _bmp.Width > 0 && ( _bmp.Width % utils.CONST_SCREEN_WIDTH_PIXELS ) != 0 ) || ( _bmp.Height > 0 && ( _bmp.Height % utils.CONST_SCREEN_HEIGHT_PIXELS ) != 0 ) )
						{
							DialogResult dlg_res = MainForm.message_box( "To get the best result, it's recommended that an imported image size must be a multiple of the game screen size.\n\nCrop the imported game level or leave it 'as is'?\n\n[Yes] to crop the game level to fully filled screens\n[No] to import the game level 'as is'", "Game Level Import Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
							
							if( dlg_res == DialogResult.Cancel )
							{
								throw new Exception( "Operation aborted!" );
							}
							
							import_game_level_as_is = ( dlg_res == DialogResult.No ) ? true:false;
						}
					}

					import_image_data( import_game_level_as_is, _bmp, _data_manager.get_tiles_data( _data_manager.tiles_data_pos ), _data_manager, _create_layout );
				}
				else
				{
#if DEF_SCREEN_HEIGHT_7d5_TILES
					if( import_game_level )
					{
						throw new Exception( "To import a game level, the imported image width must be a multiple of 32, the image height must be a multiple of 240!" );
					}
					else
#endif
					throw new Exception( "The imported image size must be a multiple of 32!" );
				}
			}
			else
			{
				if( ( _bmp.Width > 0 && ( _bmp.Width % 16 ) == 0 ) && ( _bmp.Height > 0 && ( _bmp.Height % 16 ) == 0 ) )
				{
					import_image_data( false, _bmp, _data_manager.get_tiles_data( _data_manager.tiles_data_pos ), null, null );
				}
				else
				{
					throw new Exception( "The imported image size must be a multiple of 16!" );
				}
			}
		}
		
		public void import_image_data( 	bool 							_import_game_level_as_is,
										Bitmap							_bmp,		                              
		                              	tiles_data 						_data, 
		                              	data_sets_manager 				_data_manager, 
		                              	Func< int, int, layout_data > 	_create_layout )
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
				
				int dup_block_ind;
				int dup_tile_ind;
				
				int beg_CHR_ind 	= CHR_ind;
				int beg_block_ind 	= block_ind;
				
				int scr_tile_ind;

				if( import_tiles )
				{
					int tile_ind = 0;
					int beg_tile_ind	= _data.get_first_free_tile_id();
					
					if( beg_tile_ind < 0 )
					{
						throw new Exception( "There is no free space in the tiles list!" );
					}
					
					int block_offset_x = 0;
					int block_offset_y = 0;
					
					int block_num = 0;
					
					tile_ind = beg_tile_ind;
					
					int bmp_width	= ( import_game_level && !_import_game_level_as_is ) ? ( ( _bmp.Width / utils.CONST_SCREEN_WIDTH_PIXELS ) * utils.CONST_SCREEN_WIDTH_PIXELS ):_bmp.Width;
					int bmp_height	= ( import_game_level && !_import_game_level_as_is ) ? ( ( _bmp.Height / utils.CONST_SCREEN_HEIGHT_PIXELS ) * utils.CONST_SCREEN_HEIGHT_PIXELS ):_bmp.Height;
					
					int scr_x_cnt = 0;
					int scr_y_cnt = 0;

					int scr_x;
					int scr_y;
#if DEF_SCREEN_HEIGHT_7d5_TILES
					bool half_tile = false;
#endif
					layout_data level_layout = null;
					
					if( import_game_level )
					{
						bmp_width	= ( bmp_width == 0 ) ? utils.CONST_SCREEN_WIDTH_PIXELS:bmp_width;
						bmp_height	= ( bmp_height == 0 ) ? utils.CONST_SCREEN_HEIGHT_PIXELS:bmp_height;

						scr_x_cnt = ( ( bmp_width % utils.CONST_SCREEN_WIDTH_PIXELS ) == 0 ) ? ( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ):( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ) + 1;
						scr_y_cnt = ( ( bmp_height % utils.CONST_SCREEN_HEIGHT_PIXELS ) == 0 ) ? ( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ):( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ) + 1;
						
						level_layout = _create_layout( scr_x_cnt, scr_y_cnt );
						
						if( level_layout == null )
						{
							throw new Exception( "Can't create layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT );
						}
					}
#if DEF_SCREEN_HEIGHT_7d5_TILES
					if( import_game_level )
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
								block_offset_y = tile_y + block_y;
#if DEF_SCREEN_HEIGHT_7d5_TILES
								if( import_game_level )
								{
									block_offset_y -= ( ( ( tile_y & 0xff00 ) >> 8 ) << 4 );
								}
#endif
								for( int block_x = 0; block_x < 32; block_x += 16 )
								{
									block_offset_x = tile_x + block_x;
									
									for( int CHR_n = 0; CHR_n < 4; CHR_n++ )
									{
#if DEF_SCREEN_HEIGHT_7d5_TILES
										if( import_game_level )
										{
											// extract a CHR if it's not an invisible part of a tile ( the bottom blocks of the eigth tiles row ) 
											if( !( ( ( ( block_offset_y - block_y ) % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 5 ) == 7 && block_y > 0 ) )
											{
												extract_CHR( CHR_n, block_offset_x, block_offset_y, stride, utils.tmp_spr8x8_buff, data_ptr );
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
										extract_CHR( CHR_n, block_offset_x, block_offset_y, stride, utils.tmp_spr8x8_buff, data_ptr );
										
										if( !check_CHR_and_build_block( ref CHR_ind, ref block_ind, CHR_buff, _data ) )
										{
											MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
											
											MainForm.message_box( "The CHR Bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
											
											import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind );
											
											return;
										}
									}
									
									// check duplicate blocks
									{
										if( ( dup_block_ind = _data.contains_block( block_ind - 4 ) ) >= 0 )
										{
											_data.blocks[ --block_ind ] = 0;
											_data.blocks[ --block_ind ] = 0;
											_data.blocks[ --block_ind ] = 0;
											_data.blocks[ --block_ind ] = 0;
										}
									}
									
									if( tile_ind >= utils.CONST_MAX_TILES_CNT )
									{
										MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
										
										MainForm.message_box( "The tiles list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
										
										import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind );
										
										return;
									}
									
									_data.set_tile_block( tile_ind, block_num++, ( dup_block_ind >= 0 ? (byte)( dup_block_ind >> 2 ):( byte )( ( block_ind - 1 ) >> 2 ) ) );
									
									if( ( block_num & 0x03 ) == 0x00 )
									{
										scr_tile_ind = tile_ind;
#if DEF_SCREEN_HEIGHT_7d5_TILES
										if( ( dup_tile_ind = _data.contains_tile( tile_ind, half_tile ) ) >= 0 )
#else
										if( ( dup_tile_ind = _data.contains_tile( tile_ind ) ) >= 0 )
#endif											
										{
											_data.tiles[ tile_ind ] = 0;
											
											scr_tile_ind = dup_tile_ind; 
										}
										else
										{
											++tile_ind;
										}
										
										block_num = 0;

										if( import_game_level )
										{
											scr_x = tile_x / utils.CONST_SCREEN_WIDTH_PIXELS;											
#if DEF_SCREEN_HEIGHT_7d5_TILES
											scr_y = ( block_offset_y - block_y ) / utils.CONST_SCREEN_HEIGHT_PIXELS;

											_data.scr_data[ get_local_scr_ind( level_layout.get_data( scr_x, scr_y ).m_scr_ind, _data_manager ) ][ utils.CONST_SCREEN_NUM_WIDTH_TILES * ( ( ( block_offset_y - block_y ) % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 5 ) + ( ( tile_x >> 5 ) % utils.CONST_SCREEN_NUM_WIDTH_TILES ) ] = (byte)scr_tile_ind;
#else
											scr_y = tile_y / utils.CONST_SCREEN_HEIGHT_PIXELS;
											
											_data.scr_data[ get_local_scr_ind( level_layout.get_data( scr_x, scr_y ).m_scr_ind, _data_manager ) ][ utils.CONST_SCREEN_NUM_WIDTH_TILES * ( ( tile_y >> 5 ) % utils.CONST_SCREEN_NUM_HEIGHT_TILES ) + ( ( tile_x >> 5 ) % utils.CONST_SCREEN_NUM_WIDTH_TILES ) ] = (byte)scr_tile_ind;
#endif
										}
									}
									
									if( ( block_ind >> 2 ) >= utils.CONST_MAX_BLOCKS_CNT )
									{
										MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
										
										MainForm.message_box( "The blocks list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
										
										import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind );
										
										return;
									}
								}
							}
						}
					}
					
					MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
				}
				else
				{
					// blocks/CHRs
					for( int block_y = 0; block_y < _bmp.Height; block_y += 16 )
					{
						for( int block_x = 0; block_x < _bmp.Width; block_x += 16 )
						{
							for( int CHR_n = 0; CHR_n < 4; CHR_n++ )
							{
								extract_CHR( CHR_n, block_x, block_y, stride, utils.tmp_spr8x8_buff, data_ptr );
								
								if( !check_CHR_and_build_block( ref CHR_ind, ref block_ind, CHR_buff, _data ) )
								{
									MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
									
									MainForm.message_box( "The CHR Bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
									
									import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind );
									
									return;
								}
								
								if( ( block_ind >> 2 ) >= utils.CONST_MAX_BLOCKS_CNT )
								{
									MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
									
									MainForm.message_box( "The blocks list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
									
									import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind );
									
									return;
								}
							}
							
							// check duplicate blocks
							{
								if( ( dup_block_ind = _data.contains_block( block_ind - 4 ) ) >= 0 )
								{
									_data.blocks[ --block_ind ] = 0;
									_data.blocks[ --block_ind ] = 0;
									_data.blocks[ --block_ind ] = 0;
									_data.blocks[ --block_ind ] = 0;
								}
							}
						}
					}
					
					MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
				}
				
				_bmp.UnlockBits( bmp_data );
				
				import_bmp_palette( _data, _bmp, beg_CHR_ind, CHR_ind, beg_block_ind, block_ind );
			}
		}
		
		private bool check_CHR_and_build_block( ref int _CHR_ind, ref int _block_ind, byte[] _CHR_buff, tiles_data _data )
		{
			int dup_CHR_ind;
			
			if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
			{
				_data.blocks[ _block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, 0 );
			}
			else
			{
#if DEF_SMS
				Array.Copy( utils.tmp_spr8x8_buff, _CHR_buff, _CHR_buff.Length );
				
				tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP
				
				if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
				{
					_data.blocks[ _block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP, 0 ) );
				}
				else
				{
					tiles_data.hflip( utils.tmp_spr8x8_buff );
					tiles_data.vflip( utils.tmp_spr8x8_buff );	// VFLIP
					
					if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
					{
						_data.blocks[ _block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
					}
					else
					{
						tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP + VFLIP
						
						if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, _CHR_ind ) ) >= 0 )
						{
							_data.blocks[ _block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP|utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
						}
						else
						{
							if( _CHR_ind >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
							{			
								return false;
							}
							
							_data.blocks[ _block_ind++ ] = tiles_data.set_block_CHR_id( _CHR_ind, 0 );

							_data.from_spr8x8_to_CHR_bank( _CHR_ind++, _CHR_buff );
						}
					}
				}
#else
				if( _CHR_ind >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
				{			
					return false;
				}

				_data.blocks[ _block_ind++ ] = tiles_data.set_block_CHR_id( _CHR_ind, 0 );

				_data.from_spr8x8_to_CHR_bank( _CHR_ind++, utils.tmp_spr8x8_buff );
#endif									
			}
			
			return true;
		}
		
		private void import_bmp_palette( tiles_data _data, Bitmap _bmp, int _CHR_beg_ind, int _CHR_end_ind, int _block_beg_ind, int _block_end_ind )
		{
			if( CheckBoxApplyPalette.Checked )
			{
				Color[] plt = _bmp.Palette.Entries;
				
				List< byte[] > palettes = _data.palettes;
				
				int num_clrs = Math.Min( plt.Length, utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS );
				
				for( int i = 0; i < num_clrs; i++ )
				{
					palettes[ i >> 2 ][ i & 0x03 ] = ( byte )utils.find_nearest_color_ind( plt[ i ].ToArgb() );
				}
#if DEF_NES
				apply_palettes( _data, _CHR_beg_ind, _CHR_end_ind, _block_beg_ind, _block_end_ind );
				
				palettes[ 1 ][ 0 ] = palettes[ 2 ][ 0 ] = palettes[ 3 ][ 0 ] = palettes[ 0 ][ 0 ];
#endif
				for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
				{
					palette_group.Instance.get_palettes_arr()[ i ].update();
				}				
#if DEF_SMS
				palette_group.Instance.active_palette = 0;
#endif
			}
		}

		private void extract_CHR( int _CHR_n, int _block_offset_x, int _block_offset_y, int _stride, byte[] _img_buff, System.IntPtr _data_ptr )
		{
			int CHR_offset_x;
			int CHR_offset_y;
			
			byte index_byte;
			
			byte color_index;
#if DEF_NES
			bool need_remapping = CheckBoxApplyPalette.Checked ? false:true;
			SortedList< byte, byte > inds_remap_arr = new SortedList<byte, byte>();
#elif DEF_SMS
			bool need_remapping = false;
			SortedList< byte, byte > inds_remap_arr = null;
#endif			
			CHR_offset_x = _block_offset_x + ( ( _CHR_n & 0x01 ) == 0x01 ? 8:0 );
			CHR_offset_y = _block_offset_y + ( ( _CHR_n & 0x02 ) == 0x02 ? 8:0 );
			
			for( int CHR_y = 0; CHR_y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; CHR_y++ )
			{
				for( int CHR_x = 0; CHR_x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; CHR_x++ )
				{
					index_byte = Marshal.ReadByte( _data_ptr, ( CHR_offset_y + CHR_y ) * _stride + ( ( CHR_offset_x + CHR_x ) >> 1 ) );

					color_index = ( byte )( ( ( CHR_x & 0x01 ) == 0x01 ) ? ( index_byte & 0x0f ):( ( index_byte & 0xf0 ) >> 4 ) );

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
		
#if DEF_NES
		private void apply_palettes( tiles_data _data, int _CHR_beg_ind, int _CHR_end_ind, int _block_beg_ind, int _block_end_ind )
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
			bool more_than_4_color_in_palette 	= false;

			int max_weight 		= -1;
			int transp_clr_ind	= -1;
			
			// delete "broken" block data
			if( ( _block_end_ind & 0x03 ) != 0 )
			{
				Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
				
				for( block_n = _block_end_ind; block_n > _block_end_ind - ( _block_end_ind & 0x03 ); block_n-- )
				{
					_data.from_spr8x8_to_CHR_bank( tiles_data.get_block_CHR_id( _data.blocks[ block_n - 1 ] ), utils.tmp_spr8x8_buff );
					
					_data.blocks[ block_n - 1 ] = 0; 
				}
				
				_block_end_ind = _block_end_ind & ~0x03; 
			}

			int n_blocks = ( _block_end_ind - _block_beg_ind ) >> 2;
			
			if( n_blocks == 0 )
			{
				MainForm.message_box( "No data to import!\nCan't apply palettes!", "NES Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				
				return;
			}
			
			byte[] clr_inds = new byte[ utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ];
			Array.Clear( clr_inds, 0, clr_inds.Length );

			List< byte > block_plt_inds = new List< byte >( n_blocks );
			SortedSet< byte > plt_clr_inds = null;
			
			List< SortedSet< byte > > palettes = new List< SortedSet< byte > >( 100 );
			
			bool[] remapped_CHRs = new bool[ utils.CONST_CHR_BANK_MAX_SPRITES_CNT ];
			Array.Clear( remapped_CHRs, 0, remapped_CHRs.Length );
			
			// run through blocks
			for( block_n = _block_beg_ind; block_n < _block_end_ind; block_n += utils.CONST_BLOCK_SIZE )
			{
				plt_clr_inds = new SortedSet< byte >();
					
				for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
				{
					_data.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( _data.blocks[ block_n + CHR_n ] ), utils.tmp_spr8x8_buff );
					
					for( ind_n = 0; ind_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; ind_n++ )
					{
						clr_ind = ( byte )utils.tmp_spr8x8_buff[ ind_n ];
						
						if( !plt_clr_inds.Contains( clr_ind ) )
						{
							plt_clr_inds.Add( clr_ind );
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
					palettes[ plt_n ].ForEach( delegate( byte _val ) { ++clr_inds[ _val ]; });
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

			more_than_4_palettes = palettes.Count > utils.CONST_NUM_SMALL_PALETTES ? true:false;
			
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
				plt_clr_inds.ForEach( delegate( byte _val ) 
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
									utils.tmp_spr8x8_buff[ ind_n ] = clr_inds[ utils.tmp_spr8x8_buff[ ind_n ] ];
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
			
			// apply final palettes
			Array.Clear( clr_inds, 0, clr_inds.Length );
			
			int palettes_cnt = Math.Min( palettes.Count, utils.CONST_NUM_SMALL_PALETTES );
			
			for( plt_n = 0; plt_n < palettes_cnt; plt_n++ )
			{
				plt_clr_inds = palettes[ plt_n ];	// cut palettes more than 4
				
				clr_inds[ plt_n << 2 ] = _data.palettes[ transp_clr_ind >> 2 ][ transp_clr_ind & 0x03 ];

				ind_n = 0;
				plt_clr_inds.ForEach( delegate( byte _val ) 
				{ 
					if( ind_n < 3 )	// cut palette colors more than 4
					{
						clr_inds[ ( plt_n << 2 ) + ( ++ind_n ) ] = _data.palettes[ _val >> 2 ][ _val & 0x03 ];
					}
					else
					{
						more_than_4_color_in_palette = true;
					}
				});
			}
			
			for( ind_n = 0; ind_n < clr_inds.Length; ind_n++ )
			{
				_data.palettes[ ind_n >> 2 ][ ind_n & 0x03 ] = clr_inds[ ind_n ];
			}
			
			if( more_than_4_color_in_palette || more_than_4_palettes )
			{
				MainForm.message_box( "The imported image doesn't meet the requirements!\nSome color information will be lost!", "NES Palettes Import Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
#endif		
		void BtnApplyPaletteDescClick_Event(object sender, EventArgs e)
		{
			MainForm.message_box( "For best results, an importing image must meets the following requirements:\n\n- NES compatible graphics\n- tiles aligned\n- one CHR bank data\n- no more than 13 colors", "Automatic Applying of Palettes", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
	}
}
