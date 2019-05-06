/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 21.12.2018
 * Time: 16:34
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace MAPeD
{
	/// <summary>
	/// Description of entity_preview.
	/// </summary>
	public class entity_preview : drawable_base
	{
		private int m_scr_half_width  = 0;
		private int m_scr_half_height = 0;
		
		public entity_preview( PictureBox _PBoxCHRBank ) : base( _PBoxCHRBank )
		{
			m_scr_half_width  = m_pix_box.Width >> 1;
			m_scr_half_height = m_pix_box.Height >> 1;
		}

		private float transform_to_scr_pos( int _pos, float _scale, int _half_scr )
		{
			return ( float )( ( _pos - _half_scr ) * _scale + ( float )_half_scr );
		}
		
		public void update( entity_data _ent, int _scale )
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( _ent != null )
			{
				Rectangle rect = new Rectangle( 0, 0, _ent.width, _ent.height );
				
				if( _ent.pivot_x < rect.X )
				{
					rect.X = _ent.pivot_x;
					rect.Width -= rect.X;
				}
				
				if( _ent.pivot_x > ( rect.X + rect.Width ) )
				{
					rect.Width = _ent.pivot_x; 
				}

				if( _ent.pivot_y < rect.Y )
				{
					rect.Y = _ent.pivot_y;
					rect.Height -= rect.Y;
				}
				
				if( _ent.pivot_y > ( rect.Y + rect.Height ) )
				{
					rect.Height = _ent.pivot_y; 
				}
				
				if( rect.IsEmpty == false )
				{
					// draw an entity image
					int rect_center_x = rect.Width >> 1;
					int rect_center_y = rect.Height >> 1;
					
					int offset_x = m_scr_half_width - ( rect_center_x + rect.X );
					int offset_y = m_scr_half_height - ( rect_center_y + rect.Y );
					
					int scr_pos_x = ( int )Math.Round( transform_to_scr_pos( offset_x, _scale, m_scr_half_width ) );
					int scr_pos_y = ( int )Math.Round( transform_to_scr_pos( offset_y, _scale, m_scr_half_height ) );
					
					m_gfx.DrawImage( _ent.bitmap, scr_pos_x, scr_pos_y, _scale * _ent.width, _scale * _ent.height );
					
					// draw a pivot
					offset_x += _ent.pivot_x;
					offset_y += _ent.pivot_y;
					
					int pivot_x = ( int )Math.Round( transform_to_scr_pos( offset_x, _scale, m_scr_half_width ) );
					int pivot_y = ( int )Math.Round( transform_to_scr_pos( offset_y, _scale, m_scr_half_height ) );
					
					m_pen.Width = ( float )_scale;
					
					m_pen.Color = utils.CONST_COLOR_ENTITY_PIVOT_CROSS;
					m_gfx.DrawLine( m_pen, pivot_x, pivot_y - 8, pivot_x, pivot_y + 8 );
					m_gfx.DrawLine( m_pen, pivot_x - 8, pivot_y, pivot_x + 8, pivot_y );					
					
					m_pen.Color = utils.CONST_COLOR_ENTITY_PIVOT_RECT;
					m_gfx.DrawRectangle( m_pen, pivot_x - 3, pivot_y - 3, 6, 6 );
				}
								
				m_pen.Width = 1;
				draw_border( Color.Black );
				
				disable( false );
			}
			else
			{
				disable( true );
			}
			
			invalidate();
		}
	}
}
