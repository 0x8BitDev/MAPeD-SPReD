/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 12:36
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of tiles_processor.
	/// </summary>
	/// 
	
	public class tiles_processor
	{
		public event EventHandler NeedGFXUpdate;
		public event EventHandler GFXUpdate;
		
		private CHR_bank_viewer	m_CHR_bank_viewer	= null;
		private block_editor	m_block_editor		= null;
		private tile_editor		m_tile_editor		= null;
		private palette_group 	m_palette_grp		= null;
		
		public tiles_processor( PictureBox 			_PBoxCHRBank,
		                       GroupBox				_CHRBankGrpBox,
		                       PictureBox 			_PBoxBlockEditor,
		                       PictureBox 			_PBoxTilePreview,
		                       PictureBox 			_plt_main,
		                       PictureBox 			_plt0,
		                       PictureBox 			_plt1,
		                       PictureBox 			_plt2,
		                       PictureBox 			_plt3,
		                       data_sets_manager 	_data_mngr )
		{
			m_palette_grp		= new palette_group( _plt_main, _plt0, _plt1, _plt2, _plt3 );
			m_CHR_bank_viewer	= new CHR_bank_viewer( _PBoxCHRBank, _CHRBankGrpBox );
			m_block_editor		= new block_editor( _PBoxBlockEditor );
			m_tile_editor		= new tile_editor( _PBoxTilePreview );

			m_CHR_bank_viewer.subscribe_event( m_block_editor );
			m_CHR_bank_viewer.subscribe_event( _data_mngr );
			m_block_editor.subscribe_event( _data_mngr );
			m_tile_editor.subscribe_event( _data_mngr );
			m_palette_grp.subscribe_event( _data_mngr );
			
			m_CHR_bank_viewer.subscribe_event( m_palette_grp );
			
			m_block_editor.subscribe_event( m_CHR_bank_viewer );
			m_block_editor.subscribe_event( m_tile_editor );
			m_tile_editor.subscribe_event( m_block_editor );
			
			m_CHR_bank_viewer.NeedGFXUpdate	+= new EventHandler( need_gfx_update_event );
			m_block_editor.NeedGFXUpdate 	+= new EventHandler( need_gfx_update_event );
			m_tile_editor.NeedGFXUpdate		+= new EventHandler( need_gfx_update_event );
			m_palette_grp.NeedGFXUpdate		+= new EventHandler( need_gfx_update_event );
			
			m_CHR_bank_viewer.subscribe_event( this );
			m_block_editor.subscribe_event( this );
			m_tile_editor.subscribe_event( this );
		}
		
		private void need_gfx_update_event( object sender, EventArgs e )
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}
		
		public void update_graphics()
		{
			if( GFXUpdate != null )
			{
				GFXUpdate( this, null );
			}
		}
		
		public void subscribe_block_quad_selected_event( EventHandler _e )
		{
			m_block_editor.BlockQuadSelected += new EventHandler( _e );
		}
		
		public void block_select_event( int _block_id, tiles_data _data )
		{
			m_block_editor.set_selected_block( _block_id, _data );
			m_tile_editor.set_selected_block( _block_id, _data );
		}

		public void tile_select_event( int _tile_id, tiles_data _data )
		{
			m_tile_editor.set_selected_tile( _tile_id, _data );
		}
		
		public void set_CHR_flag_vflip()
		{
			m_block_editor.set_CHR_flag_vflip();
		}
		
		public void set_CHR_flag_hflip()
		{
			m_block_editor.set_CHR_flag_hflip();
		}

		public void set_block_flags_obj_id( int _id, bool _per_block )
		{
			m_block_editor.set_block_flags_obj_id( _id, _per_block );
		}
		
		public int get_block_flags_obj_id()
		{
			return m_block_editor.get_block_flags_obj_id();
		}
		
		public int get_selected_block()
		{
			return m_block_editor.get_selected_block_id();
		}
		
		public void block_shift_colors( bool _shift_transp )
		{
			m_block_editor.shift_colors( _shift_transp );
		}
		
		public void transform_selected_CHR( utils.ETransformType _type )
		{
			m_CHR_bank_viewer.transform_selected_CHR( _type );
		}
		
		public void transform_block( utils.ETransformType _type )
		{
			m_block_editor.transform( _type );
		}
		
		public void key_event(object sender, KeyEventArgs e)
		{
			m_palette_grp.key_event( sender, e );
		}
		
		public void set_block_editor_mode( block_editor.EMode _mode )
		{
			m_block_editor.edit_mode = _mode;
			
			m_CHR_bank_viewer.selection_color = ( _mode == block_editor.EMode.bem_CHR_select ) ? utils.CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_SELECT_MODE:utils.CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_DRAW_MODE;
		}
		
		public void set_block_editor_palette_per_CHR_mode( bool _on )
		{
			m_block_editor.palette_per_CHR_mode = _on;
		}
		
		public void tile_editor_locked( bool _on )
		{
			m_tile_editor.locked( _on );
		}
		
		public int get_selected_tile()
		{
			return m_tile_editor.get_selected_tile_id();
		}
		
		public int tile_reserve_blocks( data_sets_manager _data_manager )
		{
			int block_pos 	= 0;
			int sel_tile 	= get_selected_tile();
			
			if( sel_tile >= 0 )
			{
				tiles_data data = _data_manager.get_tiles_data( _data_manager.tiles_data_pos );
				
				if( data.tiles[ sel_tile ] != 0 )
				{
					if( MainForm.message_box( "All the tile's block links will be replaced!", "Reserve Blocks", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning ) == DialogResult.Cancel )
					{
						return -1;
					}
				}
				
				bool reserve_blocks_CHRs = false;
				
				if( MainForm.message_box( "Reserve CHRs?", "Reserve Blocks", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					reserve_blocks_CHRs = true;
				}
				
				// reset tile's links
				if( reserve_blocks_CHRs )
				{
					int block_data_offs = 0;
					
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						block_data_offs = data.get_tile_block( sel_tile, i ) << 2;
						
						data.blocks[ block_data_offs ] = 0;
						data.blocks[ block_data_offs + 1 ] = 0;
						data.blocks[ block_data_offs + 2 ] = 0;
						data.blocks[ block_data_offs + 3 ] = 0;
					}
				}
				
				data.tiles[ sel_tile ] = 0;
				
				int ff_tile_ind = data.get_first_free_tile_id();
				int tile_n;
				int block_pos_n;
				
				for( int block_n = 1; block_n < utils.CONST_MAX_BLOCKS_CNT; block_n++ )
				{
					if( data.block_sum( block_n ) == 0 )
					{
						for( tile_n = 1; tile_n < ff_tile_ind; tile_n++ )
						{
							for( block_pos_n = 0; block_pos_n < utils.CONST_TILE_SIZE; block_pos_n++ )
							{
								if( data.get_tile_block( tile_n, block_pos_n ) == block_n )
								{
									break;
								}
							}
							
							if( block_pos_n != utils.CONST_TILE_SIZE )
							{
								break;
							}
						}
						
						if( tile_n == ff_tile_ind || ff_tile_ind == 0 )
						{
							data.set_tile_block( sel_tile, block_pos++, ( byte )block_n );
							
							if( reserve_blocks_CHRs )
							{
								block_reserve_CHRs( block_n, _data_manager );
							}
							
							if( block_pos == utils.CONST_TILE_SIZE )
							{
								m_tile_editor.set_selected_tile( sel_tile, data );
								
								MainForm.set_status_msg( String.Format( "Tile Editor: Tile #{0:X2} data reserved", sel_tile ) );
								
								return data.get_tile_block( sel_tile, m_tile_editor.get_selected_block_pos() );
							}
						}
					}
				}
				
				MainForm.message_box( "Tile Editor: Block list is full!", "Reserve Blocks", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			
			return -1;
		}

		public int block_reserve_CHRs( int _block_ind, data_sets_manager _data_manager )
		{
			int CHR_pos 	= 0;

			ushort block_data = 0;
			
			if( _block_ind >= 0 )
			{
				tiles_data data = _data_manager.get_tiles_data( _data_manager.tiles_data_pos );
				
				if( data.block_sum( _block_ind ) != 0 )
				{
					if( MainForm.message_box( "All the block's CHR links will be replaced!", "Reserve CHRs", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning ) == DialogResult.Cancel )
					{
						return -1;
					}
				}
				
				// reset block's links
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					data.blocks[ ( _block_ind << 2 ) + i ] = 0;
				}
				
				int ff_block_ind = data.get_first_free_block_id() << 2;
				ff_block_ind = ff_block_ind < 4 ? 4:ff_block_ind;
				int block_n = 0;
				
				for( int CHR_n = 1; CHR_n < utils.CONST_CHR_BANK_MAX_SPRITES_CNT; CHR_n++ )
				{
					if( data.spr8x8_sum( CHR_n ) == 0 )
					{
						for( block_n = 4; block_n < ff_block_ind; block_n++ )
						{
							if( tiles_data.get_block_CHR_id( data.blocks[ block_n ] ) == CHR_n )
							{
								break;
							}
						}
						
						if( block_n == ff_block_ind || ff_block_ind == 4  )
						{
							data.blocks[ ( _block_ind << 2 ) + CHR_pos++ ] = tiles_data.set_block_CHR_id( CHR_n, block_data );
							
							if( CHR_pos == utils.CONST_BLOCK_SIZE )
							{
								m_block_editor.set_selected_block( _block_ind, data );
								
								MainForm.set_status_msg( String.Format( "Block Editor: Block #{0:X2} data reserved", _block_ind ) );
								
								return 1;
							}
						}
					}
				}
				
				MainForm.message_box( "Block Editor: CHR bank is full!", "Reserve Blocks", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			
			return -1;
		}
		
		public bool CHR_bank_copy_spr()
		{
			return m_CHR_bank_viewer.copy_spr();
		}

		public void CHR_bank_paste_spr()
		{
			m_CHR_bank_viewer.paste_spr();
		}
		
		public bool CHR_bank_fill_with_color_spr()
		{
			return m_CHR_bank_viewer.fill_with_color_spr();
		}
		
		public int CHR_bank_get_selected_CHR_ind()
		{
			return m_CHR_bank_viewer.get_selected_CHR_ind();
		}
#if DEF_SMS		
		public void CHR_bank_next_page()
		{
			m_CHR_bank_viewer.next_page();
		}

		public void CHR_bank_prev_page()
		{
			m_CHR_bank_viewer.prev_page();
		}
#endif
		public static void import_image_data( 	bool							_import_tiles, 
												bool							_skip_zero_CHR_Block, 
												bool							_import_game_level, 
												bool							_import_game_level_as_is, 
												Bitmap							_bmp, 
												tiles_data						_data, 
												Func< int, int, layout_data > 	_create_layout,
												Func< int, int > 				_get_local_scr_ind )
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
				
				CHR_ind = ( CHR_ind == 0 && _skip_zero_CHR_Block == true ) ? 1:CHR_ind;
				block_ind = ( block_ind == 0 && _skip_zero_CHR_Block == true ) ? 4:block_ind;
				
				byte[] CHR_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
				
				int dup_CHR_ind;
				int dup_block_ind;
				int dup_tile_ind;
				
				int beg_CHR_ind 	= CHR_ind;
				int beg_block_ind 	= block_ind;
				
				int scr_tile_ind;

				if( _import_tiles )
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
					
					int bmp_width	= ( _import_game_level && !_import_game_level_as_is ) ? ( ( _bmp.Width / utils.CONST_SCREEN_WIDTH_PIXELS ) * utils.CONST_SCREEN_WIDTH_PIXELS ):_bmp.Width;
					int bmp_height	= ( _import_game_level && !_import_game_level_as_is ) ? ( ( _bmp.Height / utils.CONST_SCREEN_HEIGHT_PIXELS ) * utils.CONST_SCREEN_HEIGHT_PIXELS ):_bmp.Height;
					
					int scr_x_cnt = 0;
					int scr_y_cnt = 0;

					int scr_x;
					int scr_y;
					
					layout_data level_layout = null;
					
					if( _import_game_level )
					{
						bmp_width	= ( bmp_width == 0 ) ? utils.CONST_SCREEN_WIDTH_PIXELS:bmp_width;
						bmp_height	= ( bmp_height == 0 ) ? utils.CONST_SCREEN_HEIGHT_PIXELS:bmp_height;

						scr_x_cnt = ( ( bmp_width % utils.CONST_SCREEN_WIDTH_PIXELS ) == 0 ) ? ( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ):( bmp_width / utils.CONST_SCREEN_WIDTH_PIXELS ) + 1;
						scr_y_cnt = ( ( bmp_height % utils.CONST_SCREEN_HEIGHT_PIXELS ) == 0 ) ? ( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ):( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ) + 1;
						
						level_layout = _create_layout( scr_x_cnt, scr_y_cnt );
						
						if( level_layout == null )
						{
							throw new Exception( "Can't create a layout! The maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT );
						}
					}
#if DEF_SCREEN_HEIGHT_7d5_TILES
					if( _import_game_level )
					{
						bmp_height += ( ( bmp_height / utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 1 ) << 5;
					}
#endif
					// tiles/blocks/CHRs
					for( int tile_y = 0; tile_y < bmp_height; tile_y += 32 )
					{
						for( int tile_x = 0; tile_x < bmp_width; tile_x += 32 )
						{
							for( int block_y = 0; block_y < 32; block_y += 16 )
							{
								block_offset_y = tile_y + block_y;
#if DEF_SCREEN_HEIGHT_7d5_TILES
								if( _import_game_level )
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
										if( _import_game_level )
										{
											// extract a CHR if it's not an invisible part of a tile ( the bottom blocks of the eigth tiles row ) 
											if( !( ( ( ( block_offset_y - block_y ) % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 5 ) == 7 && block_y > 0 ) )
											{
												extract_CHR( CHR_n, block_offset_x, block_offset_y, stride, utils.tmp_spr8x8_buff, data_ptr );
											}
											else
											{
												// clear an invisible part of a tile
												Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
											}
										}
										else
#endif											
										extract_CHR( CHR_n, block_offset_x, block_offset_y, stride, utils.tmp_spr8x8_buff, data_ptr );
										
										if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
										{
											_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, 0 );
										}
										else
										{
#if DEF_SMS
											Array.Copy( utils.tmp_spr8x8_buff, CHR_buff, CHR_buff.Length );
											
											tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP
											
											if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
											{
												_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP, 0 ) );
											}
											else
											{
												tiles_data.hflip( utils.tmp_spr8x8_buff );
												tiles_data.vflip( utils.tmp_spr8x8_buff );	// VFLIP
												
												if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
												{
													_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
												}
												else
												{
													tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP + VFLIP
													
													if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
													{
														_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP|utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
													}
													else
													{
														_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( CHR_ind, 0 );
						
														_data.from_spr8x8_to_CHR_bank( CHR_ind++, CHR_buff );
													}
												}
											}
#else
											_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( CHR_ind, 0 );
			
											_data.from_spr8x8_to_CHR_bank( CHR_ind++, utils.tmp_spr8x8_buff );
#endif									
										}
										
										if( CHR_ind >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
										{
											MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
											
											MainForm.message_box( "The CHR Bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
											
											import_bmp_palette( _bmp, _data );
											
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
										
										import_bmp_palette( _bmp, _data );
										
										return;
									}
									
									_data.set_tile_block( tile_ind, block_num++, ( dup_block_ind >= 0 ? (byte)( dup_block_ind >> 2 ):( byte )( ( block_ind - 1 ) >> 2 ) ) );
									
									if( ( block_num & 0x03 ) == 0x00 )
									{
										scr_tile_ind = tile_ind;
										
										if( ( dup_tile_ind = _data.contains_tile( tile_ind ) ) >= 0 )
										{
											_data.tiles[ tile_ind ] = 0;
											
											scr_tile_ind = dup_tile_ind; 
										}
										else
										{
											++tile_ind;
										}
										
										block_num = 0;

										if( _import_game_level )
										{
											scr_x = tile_x / utils.CONST_SCREEN_WIDTH_PIXELS;											
#if DEF_SCREEN_HEIGHT_7d5_TILES
											scr_y = ( block_offset_y - block_y ) / utils.CONST_SCREEN_HEIGHT_PIXELS;

											_data.scr_data[ _get_local_scr_ind( level_layout.get_data( scr_x, scr_y ).m_scr_ind ) ][ utils.CONST_SCREEN_NUM_WIDTH_TILES * ( ( ( block_offset_y - block_y ) % utils.CONST_SCREEN_HEIGHT_PIXELS ) >> 5 ) + ( ( tile_x >> 5 ) % utils.CONST_SCREEN_NUM_WIDTH_TILES ) ] = (byte)scr_tile_ind;
#else
											scr_y = tile_y / utils.CONST_SCREEN_HEIGHT_PIXELS;
											
											_data.scr_data[ _get_local_scr_ind( level_layout.get_data( scr_x, scr_y ).m_scr_ind ) ][ utils.CONST_SCREEN_NUM_WIDTH_TILES * ( ( tile_y >> 5 ) % utils.CONST_SCREEN_NUM_HEIGHT_TILES ) + ( ( tile_x >> 5 ) % utils.CONST_SCREEN_NUM_WIDTH_TILES ) ] = (byte)scr_tile_ind;
#endif
										}
									}
									
									if( ( block_ind >> 2 ) >= utils.CONST_MAX_BLOCKS_CNT )
									{
										MainForm.set_status_msg( string.Format( "Merged: Tiles: {0} \\ Blocks: {1} \\ CHRs: {2}", tile_ind - beg_tile_ind, ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
										
										MainForm.message_box( "The blocks list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
										
										import_bmp_palette( _bmp, _data );
										
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
								
								if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
								{
									_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, 0 );
								}
								else
								{
#if DEF_SMS
									Array.Copy( utils.tmp_spr8x8_buff, CHR_buff, CHR_buff.Length );
									
									tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP
									
									if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
									{
										_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP, 0 ) );
									}
									else
									{
										tiles_data.hflip( utils.tmp_spr8x8_buff );
										tiles_data.vflip( utils.tmp_spr8x8_buff );	// VFLIP
										
										if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
										{
											_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
										}
										else
										{
											tiles_data.hflip( utils.tmp_spr8x8_buff );	// HFLIP + VFLIP
											
											if( ( dup_CHR_ind = _data.contains_CHR( utils.tmp_spr8x8_buff, CHR_ind ) ) >= 0 )
											{
												_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( dup_CHR_ind, tiles_data.set_block_flags_flip( utils.CONST_CHR_ATTR_FLAG_HFLIP|utils.CONST_CHR_ATTR_FLAG_VFLIP, 0 ) );
											}
											else
											{
												_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( CHR_ind, 0 );
				
												_data.from_spr8x8_to_CHR_bank( CHR_ind++, CHR_buff );
											}
										}
									}
#else
									_data.blocks[ block_ind++ ] = tiles_data.set_block_CHR_id( CHR_ind, 0 );
	
									_data.from_spr8x8_to_CHR_bank( CHR_ind++, utils.tmp_spr8x8_buff );
#endif									
								}

								if( CHR_ind >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT || ( block_ind >> 2 ) >= utils.CONST_MAX_BLOCKS_CNT )
								{
									MainForm.set_status_msg( string.Format( "Merged: Blocks: {0} \\ CHRs: {1}", ( block_ind - beg_block_ind ) >> 2, CHR_ind - beg_CHR_ind ) );
									
									if( CHR_ind >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
									{
										MainForm.message_box( "The CHR Bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
									}
									else
									{
										MainForm.message_box( "The blocks list is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
									}
									
									import_bmp_palette( _bmp, _data );
									
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
			}
			
			import_bmp_palette( _bmp, _data );
		}
		
		private static void import_bmp_palette( Bitmap _bmp, tiles_data _data )
		{
			if( MainForm.message_box( "Import colors?", "Image import", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				Color[] plt = _bmp.Palette.Entries;
				
				List< byte[] > palettes = _data.palettes;
				
				int num_clrs = Math.Min( plt.Length, utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS );
				
				for( int i = 0; i < num_clrs; i++ )
				{
					palettes[ i >> 2 ][ i & 0x03 ] = ( byte )utils.find_nearest_color_ind( plt[ i ].ToArgb() );
				}
#if DEF_NES
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

		private static void extract_CHR( int _CHR_n, int _block_offset_x, int _block_offset_y, int _stride, byte[] _img_buff, System.IntPtr _data_ptr )
		{
			int CHR_offset_x;
			int CHR_offset_y;
			
			byte index_byte;
			
			byte color_index;
#if DEF_NES
			bool need_remapping = true;
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
	}
}
