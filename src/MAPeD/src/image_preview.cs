/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
		private readonly int m_scr_half_width;
		private readonly int m_scr_half_height;

		private int		m_scale_pow		= 0;
		private bool	m_scale_enabled	= false;
		
		private Image	m_scaled_img	= null;
		
		private int m_scaled_img_pivot_x = -1;
		private int m_scaled_img_pivot_y = -1;
		
		private bool m_scaled_img_draw_pivot = false;
		
		private Rectangle m_rect;
		
		public image_preview( PictureBox _pbox, bool _need_mouse_events ) : base( _pbox )
		{
			m_scr_half_width  = m_pix_box.Width >> 1;
			m_scr_half_height = m_pix_box.Height >> 1;
			
			m_rect = new Rectangle( 0, 0, 0, 0 );
			
			if( _need_mouse_events )
			{
				_pbox.MouseWheel += new MouseEventHandler( on_mouse_wheel );

				//...
			}
		}

		private void on_mouse_wheel( object sender, MouseEventArgs e )
		{
			if( m_scale_enabled )
			{
				m_scale_pow += Math.Sign( e.Delta );
				
				m_scale_pow = m_scale_pow < 0 ? 0:m_scale_pow;
				m_scale_pow = m_scale_pow > 3 ? 3:m_scale_pow;
				
				update_scaled( true );
			}
		}
		
		public void reset_scale()
		{
			m_scale_pow = 0;
			m_scale_enabled = false;
		}
		
		public void set_scaled_image( Image _img )
		{
			m_scaled_img = _img;
		}
		
		public void set_scaled_image_pivot( int _px, int _py )
		{
			m_scaled_img_draw_pivot = true;
			
			m_scaled_img_pivot_x = _px;
			m_scaled_img_pivot_y = _py;
		}
		
		public void scale_enabled( bool _on )
		{
			m_scale_enabled = _on;
		}
		
		private float transform_to_scr_pos( int _pos, float _scale, int _half_scr )
		{
			return ( float )( ( _pos - _half_scr ) * _scale + ( float )_half_scr );
		}
		
		public void update_scaled( bool _invalidate = true )
		{
			int scale = ( int )Math.Pow( 2.0, ( double )m_scale_pow );
			
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_scaled_img != null )
			{
				m_rect.Width	= m_scaled_img.Width;
				m_rect.Height	= m_scaled_img.Height;
				
				if( m_scaled_img_pivot_x < m_rect.X )
				{
					m_rect.X = m_scaled_img_pivot_x;
					m_rect.Width -= m_rect.X;
				}
				
				if( m_scaled_img_pivot_x > ( m_rect.X + m_rect.Width ) )
				{
					m_rect.Width = m_scaled_img_pivot_x; 
				}

				if( m_scaled_img_pivot_y < m_rect.Y )
				{
					m_rect.Y = m_scaled_img_pivot_y;
					m_rect.Height -= m_rect.Y;
				}
				
				if( m_scaled_img_pivot_y > ( m_rect.Y + m_rect.Height ) )
				{
					m_rect.Height = m_scaled_img_pivot_y; 
				}
				
				if( m_rect.IsEmpty == false )
				{
					// draw an entity image
					int rect_center_x = m_rect.Width >> 1;
					int rect_center_y = m_rect.Height >> 1;
					
					int offset_x = m_scr_half_width - ( rect_center_x + m_rect.X );
					int offset_y = m_scr_half_height - ( rect_center_y + m_rect.Y );
					
					int scr_pos_x = ( int )Math.Round( transform_to_scr_pos( offset_x, scale, m_scr_half_width ) );
					int scr_pos_y = ( int )Math.Round( transform_to_scr_pos( offset_y, scale, m_scr_half_height ) );
					
					m_gfx.DrawImage( m_scaled_img, scr_pos_x, scr_pos_y, scale * m_scaled_img.Width, scale * m_scaled_img.Height );
					
					// draw a pivot
					if( m_scaled_img_draw_pivot )
					{
						offset_x += m_scaled_img_pivot_x;
						offset_y += m_scaled_img_pivot_y;
						
						int pivot_x = ( int )Math.Round( transform_to_scr_pos( offset_x, scale, m_scr_half_width ) );
						int pivot_y = ( int )Math.Round( transform_to_scr_pos( offset_y, scale, m_scr_half_height ) );
						
						m_pen.Width = ( float )( scale > 2 ? 2:scale );
						
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

		public void update( Image _img, int _width, int _height, int _x, int _y, bool _invalidate = true, bool _clear_background = true )
		{
			if( _clear_background )
			{
				clear_background( CONST_BACKGROUND_COLOR );
			}
			
			if( _img != null )
			{
				m_gfx.DrawImage( _img, _x, _y, _width, _height );
				
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
		
		public void set_focus()
		{
			m_pix_box.Focus();
		}
	}
}
