/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 21.12.2018
 * Time: 16:34
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace MAPeD
{
	/// <summary>
	/// Description of image_preview.
	/// </summary>
	public class image_preview : drawable_base
	{
		private int m_scr_half_width  = 0;
		private int m_scr_half_height = 0;
		
		private Rectangle m_rect;
		
		public image_preview( PictureBox _PBoxCHRBank ) : base( _PBoxCHRBank )
		{
			m_scr_half_width  = m_pix_box.Width >> 1;
			m_scr_half_height = m_pix_box.Height >> 1;
			
			m_rect = new Rectangle( 0, 0, 0, 0 );
		}

		private float transform_to_scr_pos( int _pos, float _scale, int _half_scr )
		{
			return ( float )( ( _pos - _half_scr ) * _scale + ( float )_half_scr );
		}
		
		public void update( Bitmap _bmp, int _width, int _height, int _pivot_x, int _pivot_y, int _scale, bool _draw_pivot, bool _invalidate = true )
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( _bmp != null )
			{
				m_rect.Width	= _width;
				m_rect.Height	= _height;
				
				if( _pivot_x < m_rect.X )
				{
					m_rect.X = _pivot_x;
					m_rect.Width -= m_rect.X;
				}
				
				if( _pivot_x > ( m_rect.X + m_rect.Width ) )
				{
					m_rect.Width = _pivot_x; 
				}

				if( _pivot_y < m_rect.Y )
				{
					m_rect.Y = _pivot_y;
					m_rect.Height -= m_rect.Y;
				}
				
				if( _pivot_y > ( m_rect.Y + m_rect.Height ) )
				{
					m_rect.Height = _pivot_y; 
				}
				
				if( m_rect.IsEmpty == false )
				{
					// draw an entity image
					int rect_center_x = m_rect.Width >> 1;
					int rect_center_y = m_rect.Height >> 1;
					
					int offset_x = m_scr_half_width - ( rect_center_x + m_rect.X );
					int offset_y = m_scr_half_height - ( rect_center_y + m_rect.Y );
					
					int scr_pos_x = ( int )Math.Round( transform_to_scr_pos( offset_x, _scale, m_scr_half_width ) );
					int scr_pos_y = ( int )Math.Round( transform_to_scr_pos( offset_y, _scale, m_scr_half_height ) );
					
					m_gfx.DrawImage( _bmp, scr_pos_x, scr_pos_y, _scale * _width, _scale * _height );
					
					// draw a pivot
					if( _draw_pivot )
					{
						offset_x += _pivot_x;
						offset_y += _pivot_y;
						
						int pivot_x = ( int )Math.Round( transform_to_scr_pos( offset_x, _scale, m_scr_half_width ) );
						int pivot_y = ( int )Math.Round( transform_to_scr_pos( offset_y, _scale, m_scr_half_height ) );
						
						m_pen.Width = ( float )( _scale > 2 ? 2:_scale );
						
						m_pen.Color = utils.CONST_COLOR_IMG_PREVIEW_PIVOT_CROSS;
						m_gfx.DrawLine( m_pen, pivot_x, pivot_y - 8, pivot_x, pivot_y + 8 );
						m_gfx.DrawLine( m_pen, pivot_x - 8, pivot_y, pivot_x + 8, pivot_y );					
						
						m_pen.Color = utils.CONST_COLOR_IMG_PREVIEW_PIVOT_RECT;
						m_gfx.DrawRectangle( m_pen, pivot_x - 3, pivot_y - 3, 6, 6 );
					}
				}
								
				m_pen.Width = 1;
				draw_border( Color.Black );
				
				disable( false );
			}
			else
			{
				disable( true );
			}
			
			if( _invalidate )
			{
				invalidate();
			}
		}
		
		public void draw_string( string _text, float _x, float _y )
		{
			utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
			m_gfx.DrawString( _text, utils.fnt8_Arial, utils.brush, _x, _y );
		}
	}
}
