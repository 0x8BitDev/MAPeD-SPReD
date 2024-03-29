﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 03.11.2022
 * Time: 13:35
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using SkiaSharp;

namespace MAPeD
{
	/// <summary>
	/// Description of patterns_manager_form.
	/// </summary>
	///

	public class PatternEventArg : EventArgs
	{
		private pattern_data m_data = null;
		
		public pattern_data data
		{
			get
			{
				pattern_data data = m_data;
				m_data = null;
				
				return data;
			}
			
			set {}
		}
		
		public PatternEventArg( pattern_data _data )
		{
			m_data = _data;
		}
	}
	
	/// <summary>
	/// Description of layout_editor_patterns.
	/// </summary>
	public class layout_editor_patterns : layout_editor_behaviour_base
	{
		private event EventHandler PatternExtractEnd;
		private event EventHandler PatternPutCancel;

		private pattern_data	m_pattern_data	= null;
		
		private enum e_mode
		{
			Idle,
			PatternPut,
			PatternExtractBegin,
			PatternExtractResize,
		}
		
		private e_mode m_mode = e_mode.Idle;
		
		private int m_tile_x	= -10000;
		private int m_tile_y	= -10000;
		
		private int m_pttrn_rect_beg_x	= -1;
		private int m_pttrn_rect_beg_y	= -1;

		private int m_pttrn_rect_end_x	= -1;
		private int m_pttrn_rect_end_y	= -1;
		
		private readonly SKBitmap m_sel_area_tile;
		
		private bool m_force_map_drawing	= false;
		
		public layout_editor_patterns( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			m_sel_area_tile = new SKBitmap( utils.CONST_SCREEN_TILES_SIZE, utils.CONST_SCREEN_TILES_SIZE, utils.CONST_BMP_FORMAT, SKAlphaType.Premul );
			using( SKCanvas canvas = new SKCanvas( m_sel_area_tile ) )
			{
				canvas.Clear( utils.CONST_COLOR_SCREEN_SELECTION_TILE );
			}
		}
		
		public override void reset( bool _init )
		{
			set_param( layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			
			m_force_map_drawing = false;
		}
		
		public override bool on_mouse_down( object sender, MouseEventArgs e )
		{
			if( m_mode == e_mode.PatternPut )
			{
				if( pattern_put( e.X, e.Y ) == false )
				{
					// update scene with error message
					m_owner.update();
				}
			}
			else
			if( m_mode == e_mode.PatternExtractBegin )
			{
				m_pttrn_rect_beg_x = m_pttrn_rect_end_x = e.X;
				m_pttrn_rect_beg_y = m_pttrn_rect_end_y = e.Y;
				
				m_mode = e_mode.PatternExtractResize;
				
				m_force_map_drawing = true;
				
				if( m_shared.m_scale < 1.0 )
				{
					m_shared.set_high_quality_render_mode( true );
				}
			}
			
			return true;
		}
		
		public override void on_mouse_up( object sender, MouseEventArgs e )
		{
			if( m_mode == e_mode.PatternExtractResize )
			{
				pattern_data pttrn;
				
				if( ( pttrn = pattern_extract() ) != null )
				{
					if( this.PatternExtractEnd != null )
					{
						this.PatternExtractEnd( this, new PatternEventArg( pttrn ) );
					}
				}
				else
				{
					// empty pattern - try again!
					m_mode = e_mode.PatternExtractBegin;
					
					// update scene with error message
					m_owner.update();
				}
				
				m_force_map_drawing = false;
			}
		}
		
		public override bool on_mouse_move( object sender, MouseEventArgs e )
		{
			if( m_mode == e_mode.PatternPut )
			{
				if( !m_shared.pix_box_captured() )
				{
					get_tile_xy( true, false, e.X, e.Y, out m_tile_x, out m_tile_y );
				}
			}
			else
			if( m_mode == e_mode.PatternExtractResize )
			{
				m_pttrn_rect_end_x = e.X;
				m_pttrn_rect_end_y = e.Y;
				
				pattern_check_size();
			}
			
			return true;
		}

		public override void on_mouse_enter( object sender, EventArgs e )
		{
			//...
		}
		
		public override void on_mouse_leave( object sender, EventArgs e )
		{
			if( m_mode == e_mode.PatternPut )
			{
				if( m_pattern_data != null )
				{
					hide_tile();
					
					m_owner.update();
				}
			}
		}
		
		public override void on_mouse_wheel( object sender, EventArgs e )
		{
			hide_tile();
		}
		
		private void hide_tile()
		{
			m_tile_x = -10000;
			m_tile_y = -10000;
		}

		private void get_tile_xy( bool _scr_space, bool _add_img_space_delta, int _x, int _y, out int _tile_x, out int _tile_y )
		{
			int tile_size_uni = platform_data.get_screen_tiles_size_uni( m_shared.m_screen_data_type ) >> 1;
			
			int img_space_delta = _add_img_space_delta ? 2:0;
			
			// image space tile position
			_tile_x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width ) + img_space_delta;
			_tile_y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height ) + img_space_delta;
			
			int scr_pos_x = _tile_x / platform_data.get_screen_width_pixels();
			int scr_pos_y = _tile_y / platform_data.get_screen_height_pixels();

			scr_pos_x += ( _tile_x < 0 ) ? -1:0;
			scr_pos_y += ( _tile_y < 0 ) ? -1:0;
			
			scr_pos_x *= platform_data.get_screen_width_pixels();
			scr_pos_y *= platform_data.get_screen_height_pixels();
			
			_tile_x -= scr_pos_x;
			_tile_y -= scr_pos_y;
			
			_tile_x -= ( _tile_x % tile_size_uni );
			_tile_y -= ( _tile_y % tile_size_uni );

			_tile_x += scr_pos_x;
			_tile_y += scr_pos_y;
			
			if( _scr_space )
			{
				_tile_x = ( int )m_shared.transform_to_scr_pos( _tile_x + m_shared.m_offset_x, m_shared.m_scr_half_width );
				_tile_y = ( int )m_shared.transform_to_scr_pos( _tile_y + m_shared.m_offset_y, m_shared.m_scr_half_height );
			}
		}
		
		private void pattern_check_size()
		{
			int beg_x;
			int beg_y;

			int end_x;
			int end_y;
			
			get_tile_xy( false, true, m_pttrn_rect_beg_x, m_pttrn_rect_beg_y, out beg_x, out beg_y );
			get_tile_xy( false, true, m_pttrn_rect_end_x, m_pttrn_rect_end_y, out end_x, out end_y );

			int tile_size_uni = platform_data.get_screen_tiles_size_uni( m_shared.m_screen_data_type ) >> 1;
			
			int scr_max_tiles_width		= platform_data.get_screen_tiles_width_uni( m_shared.m_screen_data_type );
			int scr_max_tiles_height	= platform_data.get_screen_tiles_height_uni( m_shared.m_screen_data_type );
			
			if( beg_x < end_x )
			{
				if( ( ( ( end_x - beg_x ) / tile_size_uni ) + 1 ) > scr_max_tiles_width )
				{
					end_x = beg_x + ( tile_size_uni * ( scr_max_tiles_width - 1 ) );
				}
			}
			else
			{
				if( ( ( ( beg_x - end_x ) / tile_size_uni ) + 1 ) > scr_max_tiles_width )
				{
					end_x = beg_x - ( tile_size_uni * ( scr_max_tiles_width - 1 ) );
				}
			}

			if( beg_y < end_y )
			{
				if( ( ( ( end_y - beg_y ) / tile_size_uni ) + 1 ) > scr_max_tiles_height )
				{
					end_y = beg_y + ( tile_size_uni * ( scr_max_tiles_height - 1 ) );
				}
			}
			else
			{
				if( ( ( ( beg_y - end_y ) / tile_size_uni ) + 1 ) > scr_max_tiles_height )
				{
					end_y = beg_y - ( tile_size_uni * ( scr_max_tiles_height - 1 ) );
				}
			}
			
			// transform tile coords back into screen space
			{
				m_pttrn_rect_beg_x = ( int )m_shared.transform_to_scr_pos( beg_x + m_shared.m_offset_x, m_shared.m_scr_half_width );
				m_pttrn_rect_beg_y = ( int )m_shared.transform_to_scr_pos( beg_y + m_shared.m_offset_y, m_shared.m_scr_half_height );
				
				m_pttrn_rect_end_x = ( int )m_shared.transform_to_scr_pos( end_x + m_shared.m_offset_x, m_shared.m_scr_half_width );
				m_pttrn_rect_end_y = ( int )m_shared.transform_to_scr_pos( end_y + m_shared.m_offset_y, m_shared.m_scr_half_height );
			}
		}
		
		private bool pattern_put( int _x, int _y )
		{
			bool res = true;
			
			ushort pttrn_tile_id;
			
			int tile_ind;
			int tile_pos_x;
			int tile_pos_y;
			
			int local_scr_ind;
			int glob_scr_ind;
			int scr_pos_x;
			int scr_pos_y;
			
			int map_size_x = platform_data.get_screen_width_pixels() * m_shared.m_layout.get_width();
			int map_size_y = platform_data.get_screen_height_pixels() * m_shared.m_layout.get_height();

			// image space tile position
			int img_tile_x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width );
			int img_tile_y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height );
			
			int tile_size_uni	= platform_data.get_screen_tiles_size_uni( m_shared.m_screen_data_type ) >> 1;
			int scr_tiles_width = platform_data.get_screen_tiles_width_uni( m_shared.m_screen_data_type );
			
			bool put_tiles		= false;
			bool wrong_CHR_bank	= false;

			HashSet< int > changed_screens = new HashSet< int >();
			
			tile_ind = 0;
			
			for( int i = 0; i < m_pattern_data.height; i++ )
			{
				for( int j = 0; j < m_pattern_data.width; j++ )
				{
					pttrn_tile_id = m_pattern_data.data.get_tile( tile_ind++ );
					
					tile_pos_x	= img_tile_x + ( j * tile_size_uni );
					tile_pos_y	= img_tile_y + ( i * tile_size_uni );
				
					if( ( tile_pos_x >= 0 && tile_pos_y >= 0 ) && ( tile_pos_x < map_size_x && tile_pos_y < map_size_y ) )
					{
						glob_scr_ind = get_screen_ind( tile_pos_x, tile_pos_y );
						
						if( glob_scr_ind >= 0 )
						{
							if( m_shared.m_CHR_bank_ind == m_shared.get_bank_ind_by_global_screen_ind( glob_scr_ind ) )
							{
								scr_pos_x = tile_pos_x / platform_data.get_screen_width_pixels();
								scr_pos_y = tile_pos_y / platform_data.get_screen_height_pixels();
					
								scr_pos_x *= platform_data.get_screen_width_pixels();
								scr_pos_y *= platform_data.get_screen_height_pixels();
								
								tile_pos_x -= scr_pos_x;
								tile_pos_y -= scr_pos_y;
								
								tile_pos_x /= tile_size_uni;
								tile_pos_y /= tile_size_uni;
								
								local_scr_ind = m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, glob_scr_ind );
								
								changed_screens.Add( glob_scr_ind | ( local_scr_ind << 16 ) );
								
								m_shared.m_tiles_data.set_screen_tile( local_scr_ind, tile_pos_x + ( tile_pos_y * scr_tiles_width ), pttrn_tile_id );
								
								put_tiles = true;
							}
							else
							{
								wrong_CHR_bank |= true;
							}
						}
					}
				}
			}
			
			// update changed screens
			foreach( int scr_id in changed_screens )
			{
				glob_scr_ind	= scr_id & 0x0000ffff;
				local_scr_ind	= scr_id >> 16;
				
				m_shared.update_active_bank_screen( glob_scr_ind, local_scr_ind, m_shared.m_tiles_data, m_shared.m_screen_data_type );
			}
			
			if( !put_tiles && wrong_CHR_bank )
			{
				m_shared.err_msg( "WRONG CHR BANK" );
				
				res = false;
			}
			else
			{
				m_shared.err_msg( "" );
			}
			
			return res;
		}
		
		private pattern_data pattern_extract()
		{
			int tile_pos_x;
			int tile_pos_y;
			
			int local_scr_ind;
			int glob_scr_ind;
			int scr_pos_x;
			int scr_pos_y;
			
			int map_size_x = platform_data.get_screen_width_pixels() * m_shared.m_layout.get_width();
			int map_size_y = platform_data.get_screen_height_pixels() * m_shared.m_layout.get_height();

			int tile_size_uni	= platform_data.get_screen_tiles_size_uni( m_shared.m_screen_data_type ) >> 1;
			int scr_tiles_width = platform_data.get_screen_tiles_width_uni( m_shared.m_screen_data_type );
			
			// image space tile position
			int img_tile_x = m_shared.transform_to_img_pos( Math.Min( m_pttrn_rect_beg_x, m_pttrn_rect_end_x ), m_shared.m_offset_x, m_shared.m_scr_half_width ) + ( tile_size_uni >> 1 );
			int img_tile_y = m_shared.transform_to_img_pos( Math.Min( m_pttrn_rect_beg_y, m_pttrn_rect_end_y ), m_shared.m_offset_y, m_shared.m_scr_half_height ) + ( tile_size_uni >> 1 );
			
			int tiles_width_cnt		= ( ( int )( Math.Round( 0.5f + Math.Abs( m_pttrn_rect_end_x - m_pttrn_rect_beg_x ) / m_shared.m_scale ) ) / tile_size_uni ) + 1;
			int tiles_height_cnt	= ( ( int )( Math.Round( 0.5f + Math.Abs( m_pttrn_rect_end_y - m_pttrn_rect_beg_y ) / m_shared.m_scale ) ) / tile_size_uni ) + 1;
			
			List< ushort > pttrn_tiles_arr		= new List< ushort >( tiles_width_cnt * tiles_height_cnt );
			
			HashSet< int > pttrn_height_rows	= new HashSet< int >();
			HashSet< int > pttrn_width_cols		= new HashSet< int >();

			bool wrong_CHR_bank	= false;
			
			for( int i = 0; i < tiles_height_cnt; i++ )
			{
				for( int j = 0; j < tiles_width_cnt; j++ )
				{
					tile_pos_x	= img_tile_x + ( j * tile_size_uni );
					tile_pos_y	= img_tile_y + ( i * tile_size_uni );
				
					if( ( tile_pos_x >= 0 && tile_pos_y >= 0 ) && ( tile_pos_x < map_size_x && tile_pos_y < map_size_y ) )
					{
						glob_scr_ind = get_screen_ind( tile_pos_x, tile_pos_y );
						
						if( glob_scr_ind >= 0 )
						{
							if( m_shared.m_CHR_bank_ind == m_shared.get_bank_ind_by_global_screen_ind( glob_scr_ind ) )
							{
								scr_pos_x = tile_pos_x / platform_data.get_screen_width_pixels();
								scr_pos_y = tile_pos_y / platform_data.get_screen_height_pixels();
					
								scr_pos_x *= platform_data.get_screen_width_pixels();
								scr_pos_y *= platform_data.get_screen_height_pixels();
								
								tile_pos_x -= scr_pos_x;
								tile_pos_y -= scr_pos_y;
								
								tile_pos_x /= tile_size_uni;
								tile_pos_y /= tile_size_uni;
								
								local_scr_ind = m_shared.get_local_screen_ind( m_shared.m_CHR_bank_ind, glob_scr_ind );
								
								pttrn_tiles_arr.Add( m_shared.m_tiles_data.get_screen_tile( local_scr_ind, tile_pos_x + ( tile_pos_y * scr_tiles_width ) ) );
								
								pttrn_height_rows.Add( i );
								pttrn_width_cols.Add( j );
							}
							else
							{
								wrong_CHR_bank |= true;
							}
						}
					}
				}
			}
			
			pattern_data pttrn = null;
			
			if( pttrn_tiles_arr.Count > 0 )
			{
				if( ( pttrn_width_cols.Count * pttrn_height_rows.Count ) == pttrn_tiles_arr.Count )
				{
					screen_data scr_data = new screen_data( pttrn_width_cols.Count, pttrn_height_rows.Count );
					
					for( int tile_n = 0; tile_n < pttrn_tiles_arr.Count; tile_n++ )
					{
						scr_data.set_tile( tile_n, pttrn_tiles_arr[ tile_n ] );
					}
					
					pttrn = new pattern_data( null, scr_data );
				}
				else
				{
					m_shared.err_msg( "INVALID PATTERN DATA" );
				}
			}
			else
			if( wrong_CHR_bank )
			{
				m_shared.err_msg( "WRONG CHR BANK" );
			}
			else
			{
				m_shared.err_msg( "" );
			}
			
			return pttrn;
		}
		
		private int get_screen_ind( int _x, int _y )
		{
			int slot_ind = ( _x / platform_data.get_screen_width_pixels() ) + ( _y / platform_data.get_screen_height_pixels() ) * m_shared.m_layout.get_width();
			
			return m_shared.m_layout.get_data( slot_ind % m_shared.m_layout.get_width(), slot_ind / m_shared.m_layout.get_width() ).m_scr_ind;
		}
		
		public override layout_editor_base.e_helper	default_helper()
		{
			return layout_editor_base.e_helper.UNKNOWN;
		}
		
		public override bool force_map_drawing()
		{
			return m_force_map_drawing;
		}
	
		public override void draw( SKSurface _surface, SKPaint _line_paint, SKPaint _image_paint, float _scr_size_width, float _scr_size_height )
		{
			int tile_size_uni	= platform_data.get_screen_tiles_size_uni( m_shared.m_screen_data_type );
			int tile_size		= ( int )( m_shared.m_scale * ( tile_size_uni >> 1 ) );
			
			if( m_mode == e_mode.PatternPut )
			{
				if( m_pattern_data != null )
				{
					m_shared.sys_msg( "'Esc' - cancel tile pattern" );
					
					Bitmap bmp;
					
					int pttrn_tile_id;
					int tile_ind = 0;
					
					_image_paint.ColorFilter = m_shared.m_color_filter;
					
					for( int i = 0; i < m_pattern_data.height; i++ )
					{
						for( int j = 0; j < m_pattern_data.width; j++ )
						{
							pttrn_tile_id = m_pattern_data.data.get_tile( tile_ind++ );
							
							bmp = ( m_shared.m_screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 ) ? m_shared.m_tiles_imagelist[ pttrn_tile_id ]:m_shared.m_blocks_imagelist[ pttrn_tile_id ];
							
							utils.draw_skbitmap(	_surface.Canvas,
													m_shared.m_image_cache.get( bmp ),
													m_tile_x + ( j * tile_size ),
													m_tile_y + ( i * tile_size ),
													tile_size,
													tile_size,
													_image_paint );
						}
					}
					
					_image_paint.ColorFilter = null;
				}
			}
			else
			if( m_mode == e_mode.PatternExtractResize )
			{
				_image_paint.ColorFilter = m_shared.m_color_filter;
				
				utils.draw_skbitmap(	_surface.Canvas, 
										m_sel_area_tile, 
										Math.Min( m_pttrn_rect_beg_x, m_pttrn_rect_end_x ),
										Math.Min( m_pttrn_rect_beg_y, m_pttrn_rect_end_y ),
										Math.Abs( m_pttrn_rect_beg_x - m_pttrn_rect_end_x ) + tile_size,
										Math.Abs( m_pttrn_rect_beg_y - m_pttrn_rect_end_y ) + tile_size,
										_image_paint );
				
				_image_paint.ColorFilter = null;
			}
			else
			if( m_mode == e_mode.PatternExtractBegin )
			{
				m_shared.sys_msg( "SELECT RECTANGULAR AREA, 'Esc' - cancel" );
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
				case layout_editor_param.CONST_SET_PTTRN_EXTRACT_BEGIN:
					{
						m_mode = e_mode.PatternExtractBegin;
					}
					break;
					
				case layout_editor_param.CONST_SET_PTTRN_PLACE:
					{
						m_mode = e_mode.PatternPut;
						
						m_pattern_data = ( pattern_data )_val;
					}
					break;
					
				case layout_editor_param.CONST_SET_PTTRN_IDLE_STATE:
					{
						m_mode = e_mode.Idle;
						
						m_pattern_data = null;
					}
					break;

				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_patterns.set_param]" );
				}
			}
			
			return true;
		}
		
		public override void subscribe( uint _param, Action< object, EventArgs > _method )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SUBSCR_PTTRN_EXTRACT_END:
					{
						this.PatternExtractEnd += new EventHandler( _method );
					}
					break;
					
				case layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION:
					{
						this.PatternPutCancel += new EventHandler( _method );
					}
					break;
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_patterns.subscribe]" );
				}
			}
		}
		
		public override void on_key_down( object sender, KeyEventArgs e )
		{
			//...
		}
		
		public override void on_key_up( object sender, KeyEventArgs e )
		{
			base.on_key_up( sender, e );
		}
		
		protected override void cancel_operation()
		{
			if( m_mode == e_mode.PatternExtractBegin || m_mode == e_mode.PatternPut )
			{
				hide_tile();
				
				if( PatternPutCancel != null )
				{
					PatternPutCancel( this, null );
				}
			}
		}
	}
}
