/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 26.10.2022
 * Time: 13:23
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;


namespace MAPeD
{
	public class NewTileEventArg : EventArgs
	{
		private readonly tiles_data m_data 	= null;
		private readonly int m_tile_ind 	= -1;
		
		public int tile_ind
		{
			get { return m_tile_ind; }
			set {}
		}
		
		public tiles_data data
		{
			get { return m_data; }
			set {}
		}
		
		public NewTileEventArg( int _tile_ind, tiles_data _data )
		{
			m_tile_ind = _tile_ind; 
			m_data = _data;
		}
	}
	
	/// <summary>
	/// Description of layout_editor_painter.
	/// </summary>
	public class layout_editor_painter : layout_editor_behaviour_base
	{
		private event EventHandler UpdateTileImage;
		
		// screen editor
		private int	m_active_tile_id	= -1;
		
		private readonly List< int >	m_block_tiles_cache		= null;
		private int						m_last_empty_tile_ind	= -1;
		
		private readonly HashSet< int >	m_changed_screens		= null;
		
		private int			m_tile_x;
		private int			m_tile_y;
		
		private int			m_ghost_tile_block_x	= -1;
		private int			m_ghost_tile_block_y	= -1;
		
		private enum ETileMode
		{
			etm_Unknown,
			etm_Tile,
			etm_Block,
		};
		
		private ETileMode	m_tile_mode	= ETileMode.etm_Unknown;

		public layout_editor_painter( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			m_block_tiles_cache = new List< int >( platform_data.get_max_tiles_cnt() );
			
			m_changed_screens	= new HashSet< int >();
			
			hide_tile();
		}
		
		public override void reset( bool _init )
		{
			//...
		}
		
		public override void mouse_down( object sender, MouseEventArgs e )
		{
			if( ( m_shared.m_tiles_data != null && m_shared.get_sel_screen_ind( true ) >= 0 && m_active_tile_id >= 0 ) && e.Button == MouseButtons.Left )
			{
				m_changed_screens.Clear();
				
				put_tile( e.X, e.Y );
			}
		}
		
		public override void mouse_up( object sender, MouseEventArgs e )
		{
			m_shared.pix_box_reset_capture();
			
			update_changed_screens();
			
			m_owner.update();
		}
		
		public override bool mouse_move( object sender, MouseEventArgs e )
		{
			if( m_shared.pix_box_captured() )
			{
				if( e.Button == MouseButtons.Left )
				{
					if( put_tile( e.X, e.Y ) )
					{
						hide_tile();
						
						return false;
					}
				
					if( m_active_tile_id >= 0 )
					{
						return false;
					}
				}
				
				return true;
			}
			else
			{
				if( m_active_tile_id >= 0 )
				{
					int new_tile_x;
					int new_tile_y;
					
					get_tile_xy( e.X, e.Y, out new_tile_x, out new_tile_y );
					
					if( ( new_tile_x != m_tile_x || new_tile_y != m_tile_y ) )
					{
						m_tile_x = new_tile_x;
						m_tile_y = new_tile_y;
					}
				}
			}
			
			return true;
		}

		public override void mouse_enter( object sender, EventArgs e )
		{
			//...
		}
		
		public override void mouse_leave( object sender, EventArgs e )
		{
			if( m_active_tile_id >= 0 )
			{
				hide_tile();
				
				update_changed_screens();
				
				m_owner.update();
			}
		}
		
		public override void mouse_wheel( object sender, EventArgs e )
		{
			hide_tile();
		}
		
		private void update_changed_screens()
		{
			int glob_scr_ind;
			int local_scr_ind;
			
			foreach( int scr_id in m_changed_screens )
			{
				glob_scr_ind	= scr_id & 0x0000ffff;
				local_scr_ind	= scr_id >> 16;
				
				m_shared.update_active_bank_screen( glob_scr_ind, local_scr_ind, m_shared.m_tiles_data, m_shared.m_screen_data_type );
			}
			
			m_changed_screens.Clear();
		}
		
		private void hide_tile()
		{
			m_tile_x = -10000;
			m_tile_y = -10000;
		}
		
		private int mode_tiles4x4_get_tile4x4_ind_by_pos( int _x, int _y, ref int _scr_tile_pos_x, ref int _scr_tile_pos_y, ref int _block_ind )
		{
			int x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width );
			int y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height );
			
			int scr_pos_x = platform_data.get_screen_width_pixels( false ) * ( m_shared.m_sel_screen_slot_id % m_shared.m_layout.get_width() );
			int scr_pos_y = platform_data.get_screen_height_pixels( false ) * ( m_shared.m_sel_screen_slot_id / m_shared.m_layout.get_width() );
			
			_block_ind = ( ( x % 32 ) >> 4 ) + ( ( ( y % 32 ) >> 4 ) << 1 );
			
			_scr_tile_pos_x = ( x - scr_pos_x ) >> 5;
			_scr_tile_pos_y = ( y - scr_pos_y ) >> 5;
				
			return _scr_tile_pos_x + ( _scr_tile_pos_y * platform_data.get_screen_tiles_width( false ) );
		}

		private int mode_blocks2x2_get_block2x2_ind_by_pos( int _x, int _y )
		{
			int x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width );
			int y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height );
			
			int scr_pos_x = platform_data.get_screen_width_pixels( false ) * ( m_shared.m_sel_screen_slot_id % m_shared.m_layout.get_width() );
			int scr_pos_y = platform_data.get_screen_height_pixels( false ) * ( m_shared.m_sel_screen_slot_id / m_shared.m_layout.get_width() );
			
			return ( ( x - scr_pos_x ) >> 4 ) + ( ( ( y - scr_pos_y ) >> 4 ) * platform_data.get_screen_blocks_width( false ) );
		}
		
		private bool get_tile_xy( int _x, int _y, out int _tile_x, out int _tile_y )
		{
			_tile_x = -10000;
			_tile_y = -10000;
			
			if( m_shared.get_sel_screen_ind( true ) >= 0 )
			{
				// image space screen position
				int scr_pos_x = platform_data.get_screen_width_pixels() * m_shared.get_sel_scr_pos_x();
				int scr_pos_y = platform_data.get_screen_height_pixels() * m_shared.get_sel_scr_pos_y();
				
				// image space tile position
				_tile_x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width );
				_tile_y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height );

				_tile_x -= scr_pos_x;
				_tile_y -= scr_pos_y;
				
				if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
				{
					m_ghost_tile_block_x = _tile_x % 32;
					m_ghost_tile_block_y = _tile_y % 32;
					
					_tile_x = _tile_x - m_ghost_tile_block_x;
					_tile_y = _tile_y - m_ghost_tile_block_y;
					
					m_ghost_tile_block_x >>= 4;
					m_ghost_tile_block_y >>= 4;
				}
				else
				{
					_tile_x = _tile_x - ( _tile_x % 16 );
					_tile_y = _tile_y - ( _tile_y % 16 );
				}

				_tile_x += scr_pos_x;
				_tile_y += scr_pos_y;
				
				_tile_x = m_shared.transform_to_scr_pos( _tile_x + m_shared.m_offset_x, m_shared.m_scr_half_width );
				_tile_y = m_shared.transform_to_scr_pos( _tile_y + m_shared.m_offset_y, m_shared.m_scr_half_height );
				
				return true;
			}
			
			return false;
		}
		
		private bool put_tile( int _x, int _y )
		{
			if( m_active_tile_id < 0 || get_tile_xy( _x, _y, out m_tile_x, out m_tile_y ) == false )
			{
				return false;
			}

			Region old_region = m_shared.gfx_context().Clip;
			
			build_sel_screen_rect();
			
			m_shared.gfx_context().Clip = new Region( m_shared.m_scr_img_rect );
			
			int scr_glob_ind	= m_shared.get_sel_screen_ind( true );
			int scr_local_ind	= m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, scr_glob_ind );

			m_changed_screens.Add( scr_glob_ind | ( scr_local_ind << 16 ) );
			
			if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int scr_tile_pos_x	= 0;
				int scr_tile_pos_y	= 0;
				int block_ind		= 0;
				int tile_index		= mode_tiles4x4_get_tile4x4_ind_by_pos( _x, _y, ref scr_tile_pos_x, ref scr_tile_pos_y, ref block_ind );
				
				int tile_id = m_active_tile_id;
				
				if( m_tile_mode == ETileMode.etm_Block )
				{
					ulong old_tile = m_shared.m_tiles_data.tiles[ m_shared.m_tiles_data.get_screen_tile( scr_local_ind, tile_index ) ];
					ulong new_tile = utils.set_ushort_to_ulong( old_tile, block_ind, ( ushort )m_active_tile_id );

					int tile_ind = check_tile( new_tile );
					
					if( tile_ind >= 0 )
					{
						tile_id = tile_ind;
					}
					else
					{
						tile_id = save_new_tile( new_tile );
						
						if( tile_id < 0 )
						{
							m_shared.gfx_context().Clip = old_region;
							
							return false;
						}
					}
				}
				
				m_shared.m_tiles_data.set_screen_tile( scr_local_ind, tile_index, ( ushort )tile_id );
				
				draw_tile( m_tile_x, m_tile_y, scr_tile_pos_x, scr_tile_pos_y, tile_id );

				draw_tile_grid( 32, utils.CONST_COLOR_GRID_TILES_BRIGHT );
			}
			else
			{
				m_shared.m_tiles_data.set_screen_tile( scr_local_ind, mode_blocks2x2_get_block2x2_ind_by_pos( _x, _y ), ( ushort )m_active_tile_id );
				
				draw_block( m_tile_x, m_tile_y, m_active_tile_id );
				
				draw_tile_grid( 16, utils.CONST_COLOR_GRID_BLOCKS );
			}
			
			m_owner.invalidate();
				
			m_shared.gfx_context().Clip = old_region;

			return true;
		}
		
		private int save_new_tile( ulong _tile )
		{
			if( m_last_empty_tile_ind >= 0 )
			{
				for( int i = m_last_empty_tile_ind; i < platform_data.get_max_tiles_cnt(); i++ )
				{
					if( m_shared.m_tiles_data.tiles[ i ] == 0 )
					{
						m_shared.m_tiles_data.tiles[ i ] = _tile;
						
						// add the tile to the cache
						m_block_tiles_cache.Add( i );
						
						// redraw tile image here...
						if( UpdateTileImage != null )
						{
							UpdateTileImage( this, new NewTileEventArg( i, m_shared.m_tiles_data ) );
						}
						
						m_last_empty_tile_ind = i;
						
						return i;
					}
				}
			}
			
			m_shared.pix_box_reset_capture();
			
			m_last_empty_tile_ind = -1;
				
			// TILES ARRAY OVERFLOW !!!
			MainForm.message_box( "Try to optimize the tiles data!", "Tiles Array Overflow", System.Windows.Forms.MessageBoxButtons.OK );
			
			return -1;
		}
		
		private int check_tile( ulong _tile_data )
		{
			int tile_ind;
			
			int size = m_block_tiles_cache.Count;
			
			for( int i = 0; i < size; i++ )
			{
				tile_ind = m_block_tiles_cache[ i ];
				
				if( m_shared.m_tiles_data.tiles[ tile_ind ] == _tile_data )
				{
					return tile_ind;
				}
			}
			
			return -1;
		}
		
		private void calc_common_blocks( ushort _block_ind )
		{
			int i;
			int j;	

			ulong tile_data;
			
			m_last_empty_tile_ind = m_shared.m_tiles_data.get_first_free_tile_id( false );
			m_last_empty_tile_ind = ( m_last_empty_tile_ind == 0 ) ? 1:m_last_empty_tile_ind;	// skip zero tile as reserved for an empty space
			
			m_block_tiles_cache.Clear();
			
			for( i = 0; i < platform_data.get_max_tiles_cnt(); i++ )
			{
				tile_data = m_shared.m_tiles_data.tiles[ i ];

				for( j = 0; j < utils.CONST_TILE_SIZE; j++ )
				{
					if( utils.get_ushort_from_ulong( tile_data, j ) == _block_ind )
					{
						m_block_tiles_cache.Add( i );
			
						break;
					}
				}
			}
		}

		private void build_sel_screen_rect()
		{
			m_shared.m_scr_img_rect.X = m_shared.screen_pos_x_by_slot_id( m_shared.get_sel_scr_pos_x() );
			m_shared.m_scr_img_rect.Y = m_shared.screen_pos_y_by_slot_id( m_shared.get_sel_scr_pos_y() );						
			
			m_shared.m_scr_img_rect.Width	= ( int )( platform_data.get_screen_width_pixels() * m_shared.m_scale ); 
			m_shared.m_scr_img_rect.Height	= ( int )( platform_data.get_screen_height_pixels() * m_shared.m_scale );

			if( m_shared.m_scr_img_rect.X < 0 )
			{
				m_shared.m_scr_img_rect.Width -= -m_shared.m_scr_img_rect.X;
				m_shared.m_scr_img_rect.X = 0;
			}
			
			if( m_shared.m_scr_img_rect.Y < 0 )
			{
				m_shared.m_scr_img_rect.Height -= -m_shared.m_scr_img_rect.Y;
				m_shared.m_scr_img_rect.Y = 0;
			}
			
			if( m_shared.m_scr_img_rect.Width > m_shared.pix_box_width() )
			{
				m_shared.m_scr_img_rect.Width = m_shared.pix_box_width();
			}

			if( m_shared.m_scr_img_rect.Height > m_shared.pix_box_height() )
			{
				m_shared.m_scr_img_rect.Height = m_shared.pix_box_height();
			}
		}
		
		public override bool block_free_map_panning()
		{
			return true;
		}

		public override bool force_map_drawing()
		{
			return false;
		}

		public override void draw( Graphics _gfx, Pen _pen, int _scr_size_width, int _scr_size_height )
		{
			// draw ghost tile image
			if( m_active_tile_id >= 0 )
			{
				Region old_region = m_shared.gfx_context().Clip;
				
				build_sel_screen_rect();
				
				m_shared.gfx_context().Clip = new Region( m_shared.m_scr_img_rect );
				
				if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
				{
					if( m_tile_mode == ETileMode.etm_Tile )
					{
						draw_ghost_tile( _gfx );
					}
					else
					{
						draw_ghost_tile_block( _gfx );
					}
				}
				else
				{
					draw_ghost_block( _gfx );
				}
				
				m_shared.gfx_context().Clip = old_region;
			}
		}

		private void draw_ghost_tile( Graphics _gfx )
		{
			int tile_size = ( int )( m_shared.m_scale * 32.0f );
			
			m_shared.m_scr_img_rect.X = m_tile_x;
			m_shared.m_scr_img_rect.Y = m_tile_y;
			
			m_shared.m_scr_img_rect.Width = m_shared.m_scr_img_rect.Height = tile_size;
			
			_gfx.DrawImage( m_shared.m_tiles_imagelist.Images[ m_active_tile_id ], m_shared.m_scr_img_rect, 0, 0, utils.CONST_TILES_IMG_SIZE, utils.CONST_TILES_IMG_SIZE, GraphicsUnit.Pixel, m_shared.m_scr_img_attr );
		}
		
		private void draw_ghost_tile_block( Graphics _gfx )
		{
			int tile_size = ( int )( m_shared.m_scale * 16.0f );
			
			m_shared.m_scr_img_rect.X	= m_tile_x + ( m_ghost_tile_block_x * tile_size );
			m_shared.m_scr_img_rect.Y	= m_tile_y + ( m_ghost_tile_block_y * tile_size );
			
			m_shared.m_scr_img_rect.Width = m_shared.m_scr_img_rect.Height = tile_size;

			_gfx.DrawImage( m_shared.m_blocks_imagelist.Images[ m_active_tile_id ], m_shared.m_scr_img_rect, 0, 0, utils.CONST_BLOCKS_IMG_SIZE, utils.CONST_BLOCKS_IMG_SIZE, GraphicsUnit.Pixel, m_shared.m_scr_img_attr );
		}
		
		private void draw_ghost_block( Graphics _gfx )
		{
			int tile_size = ( int )( m_shared.m_scale * 16.0f );
			
			m_shared.m_scr_img_rect.X	= m_tile_x;
			m_shared.m_scr_img_rect.Y	= m_tile_y;
			
			m_shared.m_scr_img_rect.Width = m_shared.m_scr_img_rect.Height = tile_size;
			
			_gfx.DrawImage( m_shared.m_blocks_imagelist.Images[ m_active_tile_id ], m_shared.m_scr_img_rect, 0, 0, utils.CONST_BLOCKS_IMG_SIZE, utils.CONST_BLOCKS_IMG_SIZE, GraphicsUnit.Pixel, m_shared.m_scr_img_attr );
		}

		private void draw_tile( int _x, int _y, int _tile_pos_x, int _tile_pos_y, int _tile_id )
		{
			int size = ( int )( m_shared.m_scale * 32.0f );
			
			m_shared.gfx_context().DrawImage(	m_shared.m_tiles_imagelist.Images[ _tile_id ],
												_x,
												_y,
												size,
												size );
		}
		
		private void draw_block( int _x, int _y, int _tile_id )
		{
			int size = ( int )( m_shared.m_scale * 16.0f );
			
			m_shared.gfx_context().DrawImage(	m_shared.m_blocks_imagelist.Images[ _tile_id ],
												_x,
												_y,
												size,
												size );
		}
		
		private void draw_tile_grid( int _tile_size, Color _clr )
		{
			if( m_owner.show_grid && ( m_shared.m_scale >= 1.0 ) )
			{
				utils.pen.Width = 1;
				utils.pen.Color = _clr;
				
				int tile_size = ( int )( m_shared.m_scale * _tile_size );
				
				int pdx = m_tile_x + tile_size;
				int pdy = m_tile_y + tile_size;
				
				m_shared.gfx_context().DrawLine( utils.pen, m_tile_x, pdy, pdx, pdy );
				m_shared.gfx_context().DrawLine( utils.pen, pdx, m_tile_y, pdx, pdy );
			}
		}
		
		public override Object get_param( uint _param )
		{
			return null;
		}
		
		public override bool set_param( uint _param, Object _val )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SET_PNT_CLEAR_ACTIVE_TILE:
					{
						m_active_tile_id		= -1;
						m_last_empty_tile_ind	= -1;
					}
					break;
					
				case layout_editor_param.CONST_SET_PNT_UPD_ACTIVE_TILE:
					{
						m_active_tile_id = ( int )_val;
						
						m_tile_mode	= ETileMode.etm_Tile;
						
						hide_tile();
					}
					break;

				case layout_editor_param.CONST_SET_PNT_UPD_ACTIVE_BLOCK:
					{
						m_active_tile_id = ( int )_val;
						
						m_tile_mode	= ETileMode.etm_Block;

						hide_tile();
						
						if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
						{
							calc_common_blocks( ( byte )m_active_tile_id );
						}
					}
					break;
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_painter.set_param]" );
				}
			}
			
			return true;
		}
		
		public override void subscribe( uint _param, Action< object, EventArgs > _method )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SUBSCR_PNT_UPDATE_TILE_IMAGE:
					{
						this.UpdateTileImage += new EventHandler( _method );
					}
					break;
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_painter.subscribe]" );
				}
			}
		}
	}
}
