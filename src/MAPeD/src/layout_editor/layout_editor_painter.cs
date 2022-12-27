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

using SkiaSharp;

namespace MAPeD
{
	public class NewTileEventArg : EventArgs
	{
		private readonly tiles_data	m_data;
		private readonly int		m_tile_ind;
		
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
		private event EventHandler CancelActiveTile;
		
		// screen editor
		private int	m_active_tile_id	= -1;
		
		private readonly List< int >	m_block_tiles_cache;
		private int						m_last_empty_tile_ind	= -1;
		
		private readonly HashSet< int >	m_changed_screens;
		
		private int			m_tile_x;
		private int			m_tile_y;
		
		private int			m_transp_tile_block_x	= -1;
		private int			m_transp_tile_block_y	= -1;
		
		private SKRectI		m_recti			= new SKRectI();
		private SKRectI		m_tile_recti	= new SKRectI();
		
		private enum ETileMode
		{
			etm_Unknown,
			etm_Tile,
			etm_Block,
		};
		
		private ETileMode	m_tile_mode	= ETileMode.etm_Unknown;
		
		private enum EEditMode
		{
			em_Unknown,
			em_Painting,
			em_ReplaceTiles,
		};
		
		private EEditMode	m_edit_mode = EEditMode.em_Unknown;
		
		public layout_editor_painter( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			m_block_tiles_cache = new List< int >( platform_data.get_max_tiles_cnt() );
			
			m_changed_screens	= new HashSet< int >();
			
			hide_tile();
			
			m_edit_mode = EEditMode.em_Painting;
		}
		
		public override void reset( bool _init )
		{
			//...
		}
		
		public override bool mouse_down( object sender, MouseEventArgs e )
		{
			if( m_edit_mode == EEditMode.em_Painting )
			{
				if( m_shared.m_tiles_data != null && m_shared.get_sel_screen_ind( true ) >= 0 && m_active_tile_id >= 0 )
				{
					m_changed_screens.Clear();
					
					put_tile( e.X, e.Y );
				}
			}
			else
			if( m_edit_mode == EEditMode.em_ReplaceTiles )
			{
				selected_screens_replace_tiles();
				
				return false;
			}
			
			return true;
		}
		
		public override void mouse_up( object sender, MouseEventArgs e )
		{
			if( m_edit_mode == EEditMode.em_Painting )
			{
				m_shared.pix_box_reset_capture();
				
				update_changed_screens();
				
				m_shared.sys_msg( "" );
				
				m_owner.update();
			}
		}
		
		public override bool mouse_move( object sender, MouseEventArgs e )
		{
			if( m_edit_mode == EEditMode.em_Painting )
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
			}
			
			if( !m_shared.pix_box_captured() )
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
		
		private int mode_tiles4x4_get_tile4x4_ind_by_pos( int _x, int _y, ref int _block_ind )
		{
			int x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width );
			int y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height );
			
			int scr_pos_x = platform_data.get_screen_width_pixels( false ) * ( m_shared.m_sel_screen_slot_id % m_shared.m_layout.get_width() );
			int scr_pos_y = platform_data.get_screen_height_pixels( false ) * ( m_shared.m_sel_screen_slot_id / m_shared.m_layout.get_width() );
			
			int dx = x - scr_pos_x;
			int dy = y - scr_pos_y;
			
			_block_ind = ( ( dx % 32 ) >> 4 ) + ( ( ( dy % 32 ) >> 4 ) << 1 );
			
			int scr_tile_pos_x = dx >> 5;
			int scr_tile_pos_y = dy >> 5;
				
			return scr_tile_pos_x + ( scr_tile_pos_y * platform_data.get_screen_tiles_width( false ) );
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
					m_transp_tile_block_x = _tile_x % 32;
					m_transp_tile_block_y = _tile_y % 32;
					
					_tile_x -= m_transp_tile_block_x;
					_tile_y -= m_transp_tile_block_y;
					
					m_transp_tile_block_x >>= 4;
					m_transp_tile_block_y >>= 4;
				}
				else
				{
					_tile_x -= ( _tile_x % 16 );
					_tile_y -= ( _tile_y % 16 );
				}

				_tile_x += scr_pos_x;
				_tile_y += scr_pos_y;
				
				_tile_x = ( int )m_shared.transform_to_scr_pos( _tile_x + m_shared.m_offset_x, m_shared.m_scr_half_width );
				_tile_y = ( int )m_shared.transform_to_scr_pos( _tile_y + m_shared.m_offset_y, m_shared.m_scr_half_height );
				
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

			m_shared.gfx_context().Canvas.Save();
			
			apply_sel_screen_rect( true );
			
			int scr_glob_ind	= m_shared.get_sel_screen_ind( true );
			int scr_local_ind	= m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, scr_glob_ind );

			m_changed_screens.Add( scr_glob_ind | ( scr_local_ind << 16 ) );
			
			if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int block_ind		= 0;
				int tile_index		= mode_tiles4x4_get_tile4x4_ind_by_pos( _x, _y, ref block_ind );
				
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
							m_shared.gfx_context().Canvas.Restore();
							
							return false;
						}
					}
				}
				
				m_shared.m_tiles_data.set_screen_tile( scr_local_ind, tile_index, ( ushort )tile_id );
				
				// fill the first back buffer
				draw_tile( m_tile_x, m_tile_y, tile_id, m_shared.gfx_context().Canvas, m_shared.paint_image() );
				draw_tile_grid( 32, utils.CONST_COLOR_GRID_TILES_BRIGHT, m_shared.gfx_context().Canvas, m_shared.paint_line() );
				
				// show the first back buffer and switch to the second one
				m_owner.invalidate();
				
				// fill the second back buffer
				draw_tile( m_tile_x, m_tile_y, tile_id, m_shared.gfx_context().Canvas, m_shared.paint_image() );
				draw_tile_grid( 32, utils.CONST_COLOR_GRID_TILES_BRIGHT, m_shared.gfx_context().Canvas, m_shared.paint_line() );
			}
			else
			{
				m_shared.m_tiles_data.set_screen_tile( scr_local_ind, mode_blocks2x2_get_block2x2_ind_by_pos( _x, _y ), ( ushort )m_active_tile_id );
				
				// fill the first back buffer
				draw_block( m_tile_x, m_tile_y, m_active_tile_id, m_shared.gfx_context().Canvas, m_shared.paint_image() );
				draw_tile_grid( 16, utils.CONST_COLOR_GRID_BLOCKS, m_shared.gfx_context().Canvas, m_shared.paint_line() );
				
				// show the first back buffer and switch to the second one
				m_owner.invalidate();
				
				draw_block( m_tile_x, m_tile_y, m_active_tile_id, m_shared.gfx_context().Canvas, m_shared.paint_image() );
				draw_tile_grid( 16, utils.CONST_COLOR_GRID_BLOCKS, m_shared.gfx_context().Canvas, m_shared.paint_line() );
			}
			
			m_shared.gfx_context().Canvas.Restore();
			
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
			MainForm.message_box( "Try to optimize the tiles data!", "Tiles Array Overflow", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			
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

		private void apply_sel_screen_rect( bool _apply_rect_to_canvas )
		{
			m_recti.Left	= ( int )m_shared.screen_pos_x_by_slot_id( m_shared.get_sel_scr_pos_x() );
			m_recti.Top		= ( int )m_shared.screen_pos_y_by_slot_id( m_shared.get_sel_scr_pos_y() );
			
			m_recti.Right	= m_recti.Left + ( int )( platform_data.get_screen_width_pixels() * m_shared.m_scale );
			m_recti.Bottom	= m_recti.Top + ( int )( platform_data.get_screen_height_pixels() * m_shared.m_scale );
			
			if( _apply_rect_to_canvas )
			{
				m_shared.gfx_context().Canvas.ClipRect( m_recti );
			}
		}
		
		public override layout_editor_base.EHelper	default_helper()
		{
			return layout_editor_base.EHelper.eh_Unknown;
		}
		
		public override bool force_map_drawing()
		{
			return false;
		}

		public override void draw( SKSurface _surface, SKPaint _line_paint, SKPaint _image_paint, float _scr_size_width, float _scr_size_height )
		{
			// draw ghost tile image
			if( m_active_tile_id >= 0 )
			{
				if( m_edit_mode == EEditMode.em_Painting )
				{
					m_shared.sys_msg( "'Esc' - cancel tile" );
					
					_surface.Canvas.Save();
					
					apply_sel_screen_rect( true );
					
					if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						if( m_tile_mode == ETileMode.etm_Tile )
						{
							draw_transparent_tile( _surface.Canvas, _image_paint );
						}
						else
						{
							draw_transparent_tile_block( _surface.Canvas, _image_paint );
						}
					}
					else
					{
						draw_transparent_block( _surface.Canvas, _image_paint );
					}
					
					_surface.Canvas.Restore();
				}
				else
				if( m_edit_mode == EEditMode.em_ReplaceTiles )
				{
					int tile_size;
					
					m_shared.sys_msg( "SELECT A TILE TO REPLACE, 'Esc' - cancel" );
					
					apply_sel_screen_rect( false );
					
					if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						if( m_tile_mode == ETileMode.etm_Tile )
						{
							tile_size = ( int )( m_shared.m_scale * 32.0f );
						}
						else
						{
							return;
						}
					}
					else
					{
						tile_size = ( int )( m_shared.m_scale * 16.0f );
					}
					
					m_tile_recti.Left	= m_tile_x;
					m_tile_recti.Top	= m_tile_y;
					m_tile_recti.Right	= m_tile_x + tile_size;
					m_tile_recti.Bottom	= m_tile_y + tile_size;
					
					SKRectI int_rect = SKRectI.Intersect( m_tile_recti, m_recti );
					
					_line_paint.StrokeWidth	= 2;
					_line_paint.Color		= utils.CONST_COLOR_TILE_BORDER;
					
					_surface.Canvas.DrawRect( int_rect.Left, int_rect.Top, int_rect.Width, int_rect.Height, _line_paint );
				}
			}
		}

		private void draw_transparent_tile( SKCanvas _canvas, SKPaint _image_paint )
		{
			int tile_size = ( int )( m_shared.m_scale * 32.0f );
			
			_image_paint.ColorFilter = m_shared.m_color_filter;
			{
				utils.draw_skbitmap( _canvas, m_shared.m_image_cache.get( m_shared.m_tiles_imagelist[ m_active_tile_id ] ), m_tile_x, m_tile_y, tile_size, tile_size, _image_paint );
			}
			_image_paint.ColorFilter = null;
		}
		
		private void draw_transparent_tile_block( SKCanvas _canvas, SKPaint _image_paint )
		{
			int tile_size = ( int )( m_shared.m_scale * 16.0f );
			
			_image_paint.ColorFilter = m_shared.m_color_filter;
			{
				utils.draw_skbitmap(_canvas,
									m_shared.m_image_cache.get( m_shared.m_blocks_imagelist[ m_active_tile_id ] ),
									m_tile_x + ( m_transp_tile_block_x * tile_size ),
									m_tile_y + ( m_transp_tile_block_y * tile_size ),
									tile_size,
									tile_size,
									_image_paint );
			}
			_image_paint.ColorFilter = null;
		}
		
		private void draw_transparent_block( SKCanvas _canvas, SKPaint _image_paint )
		{
			int tile_size = ( int )( m_shared.m_scale * 16.0f );
			
			_image_paint.ColorFilter = m_shared.m_color_filter;
			{
				utils.draw_skbitmap( _canvas, m_shared.m_image_cache.get( m_shared.m_blocks_imagelist[ m_active_tile_id ] ), m_tile_x, m_tile_y, tile_size, tile_size, _image_paint );
			}
			_image_paint.ColorFilter = null;
		}

		private void draw_tile( int _x, int _y, int _tile_id, SKCanvas _canvas, SKPaint _image_paint )
		{
			int size = ( int )( m_shared.m_scale * 32.0f );
			
			utils.draw_skbitmap( _canvas, m_shared.m_image_cache.get( m_shared.m_tiles_imagelist[ _tile_id ] ), _x, _y, size, size, _image_paint );
		}
		
		private void draw_block( int _x, int _y, int _tile_id, SKCanvas _canvas, SKPaint _image_paint )
		{
			int size = ( int )( m_shared.m_scale * 16.0f );
			
			utils.draw_skbitmap( _canvas, m_shared.m_image_cache.get( m_shared.m_blocks_imagelist[ _tile_id ] ), _x, _y, size, size, _image_paint );
		}
		
		private void draw_tile_grid( int _tile_size, SKColor _clr, SKCanvas _canvas, SKPaint _line_paint )
		{
			if( m_owner.show_grid && ( m_shared.m_scale >= 1.0 ) )
			{
				_line_paint.StrokeWidth = 1;
				_line_paint.Color = _clr;
				
				int tile_size = ( int )( m_shared.m_scale * _tile_size );
				
				int pdx = m_tile_x + tile_size;
				int pdy = m_tile_y + tile_size;
				
				_canvas.DrawLine( m_tile_x, m_tile_y, pdx, m_tile_y, _line_paint );
				_canvas.DrawLine( pdx, m_tile_y, pdx, pdy, _line_paint );
			}
		}
		
		private bool selected_screens_fill_with_tile()
		{
			bool res = false;
			
			if( m_shared.selected_screens() )
			{
				if( MainForm.message_box( "All selected screens will be filled with the active tile!\n\nAre you sure?", "Fill With Tiles", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning ) == DialogResult.Yes )
				{
					bool ignore_scr_data_enum = false;
					
					m_shared.selected_screens_proc( delegate( int _scr_slot_ind )
					{
						int scr_pos_x = _scr_slot_ind % m_shared.m_layout.get_width();
						int scr_pos_y = _scr_slot_ind / m_shared.m_layout.get_width();
						
						layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
						
						if( m_shared.get_bank_ind_by_global_screen_ind( scr_data.m_scr_ind ) == m_shared.m_CHR_bank_ind )
						{
							if( ignore_scr_data_enum )
							{
								return;
							}
							
							int scr_local_ind	= m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, scr_data.m_scr_ind );
							
							if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
							{
								if( m_tile_mode == ETileMode.etm_Tile )
								{
									ushort[] scr_arr = m_shared.m_tiles_data.get_screen_data( scr_local_ind ).m_arr;
									
									for( int arr_pos = 0; arr_pos < scr_arr.Length; arr_pos++ )
									{
										scr_arr[ arr_pos ] = ( ushort )m_active_tile_id;
									}
								}
								else
								{
									// blocks
									// generate a new tile from the current block
									ulong block		= ( ushort )m_active_tile_id;
									ulong new_tile	= ( block << 48 ) | ( block << 32 ) | ( block << 16 ) | block;
									
									int tile_ind = check_tile( new_tile );
									int new_tile_id;
									
									if( tile_ind >= 0 )
									{
										new_tile_id = tile_ind;
									}
									else
									{
										new_tile_id = save_new_tile( new_tile );
										
										if( new_tile_id < 0 )
										{
											ignore_scr_data_enum = true;
											
											return;
										}
									}
									
									ushort[] scr_arr = m_shared.m_tiles_data.get_screen_data( scr_local_ind ).m_arr;
									
									for( int arr_pos = 0; arr_pos < scr_arr.Length; arr_pos++ )
									{
										scr_arr[ arr_pos ] = ( ushort )new_tile_id;
									}
								}
							}
							else
							{
								// blocks
								ushort[] scr_arr = m_shared.m_tiles_data.get_screen_data( scr_local_ind ).m_arr;
								
								for( int arr_pos = 0; arr_pos < scr_arr.Length; arr_pos++ )
								{
									scr_arr[ arr_pos ] = ( ushort )m_active_tile_id;
								}
							}
							
							m_shared.update_active_bank_screen( scr_data.m_scr_ind, scr_local_ind, m_shared.m_tiles_data, m_shared.m_screen_data_type );
						}
					});
					
					res = true;
				}
			}
			else
			{
				MainForm.message_box( "Please, select screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			m_owner.update();
			
			return res;
		}
		
		private void enable_selected_screens_replace_tiles( bool _on )
		{
			if( _on )
			{
				m_edit_mode = EEditMode.em_ReplaceTiles;
			}
			else
			{
				m_edit_mode = EEditMode.em_Painting;
			}
			
			m_owner.update();
		}
		
		private bool selected_screens_replace_tiles()
		{
			bool res = false;
			
			if( m_shared.m_sel_screens_slot_ids.Count > 0 )
			{
				int tile_pos;
				
				// get a tile position in screen space the cursor points to
				{
					if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						int block_ind = 0;
						
						tile_pos = mode_tiles4x4_get_tile4x4_ind_by_pos( m_shared.m_mouse_x, m_shared.m_mouse_y, ref block_ind );
					}
					else
					{
						tile_pos = mode_blocks2x2_get_block2x2_ind_by_pos( m_shared.m_mouse_x, m_shared.m_mouse_y );
					}
				}
				
				int scr_glob_ind = m_shared.get_sel_screen_ind( true );
				
				if( scr_glob_ind >= 0 )
				{
					// wrong CHR bank already checked in the 'get_sel_screen_ind'
					int scr_local_ind = m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, scr_glob_ind );
					
					// get the tile index the cursor points to
					ushort tile_ind = m_shared.m_tiles_data.get_screen_data( scr_local_ind ).m_arr[ tile_pos ];
					
					m_shared.selected_screens_proc( delegate( int _scr_slot_ind )
					{
						int scr_pos_x = _scr_slot_ind % m_shared.m_layout.get_width();
						int scr_pos_y = _scr_slot_ind / m_shared.m_layout.get_width();
						
						layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
						
						if( m_shared.get_bank_ind_by_global_screen_ind( scr_data.m_scr_ind ) == m_shared.m_CHR_bank_ind )
						{
							scr_local_ind = m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, scr_data.m_scr_ind );
							
							if( m_shared.m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
							{
								if( m_tile_mode == ETileMode.etm_Tile )
								{
									ushort[] scr_arr = m_shared.m_tiles_data.get_screen_data( scr_local_ind ).m_arr;
									
									for( int arr_pos = 0; arr_pos < scr_arr.Length; arr_pos++ )
									{
										if( scr_arr[ arr_pos ] == tile_ind )
										{
											scr_arr[ arr_pos ] = ( ushort )m_active_tile_id;
										}
									}
								}
								else
								{
									// blocks
									throw new Exception( "UNSUPPORTED BEHAVIOUR!" );
								}
							}
							else
							{
								// blocks
								ushort[] scr_arr = m_shared.m_tiles_data.get_screen_data( scr_local_ind ).m_arr;
								
								for( int arr_pos = 0; arr_pos < scr_arr.Length; arr_pos++ )
								{
									if( scr_arr[ arr_pos ] == tile_ind )
									{
										scr_arr[ arr_pos ] = ( ushort )m_active_tile_id;
									}
								}
							}
							
							m_shared.update_active_bank_screen( scr_data.m_scr_ind, scr_local_ind, m_shared.m_tiles_data, m_shared.m_screen_data_type );
						}
					});
					
					res = true;
				}
			}
			else
			{
				MainForm.message_box( "Please, select screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			m_owner.update();
			
			return res;
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
					
				case layout_editor_param.CONST_SET_PNT_FILL_WITH_TILE:
					{
						return selected_screens_fill_with_tile();
					}
					
				case layout_editor_param.CONST_SET_PNT_REPLACE_TILES:
					{
						enable_selected_screens_replace_tiles( ( bool )_val );
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
					
				case layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION:
					{
						this.CancelActiveTile += new EventHandler( _method );
					}
					break;
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_painter.subscribe]" );
				}
			}
		}

		public override void key_down_event( object sender, KeyEventArgs e )
		{
			//...
		}
		
		public override void key_up_event( object sender, KeyEventArgs e )
		{
			base.key_up_event( sender, e );
		}
		
		protected override void cancel_operation()
		{
			if( m_active_tile_id >= 0 )
			{
				if( CancelActiveTile != null )
				{
					CancelActiveTile( this, null );
				}
			}
		}
	}
}
