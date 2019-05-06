/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 17.03.2017
 * Time: 16:59
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of palette_small.
	/// </summary>
	/// 
	
	public delegate void ActivePalette();

	public class palette_small : drawable_base
	{
		public event EventHandler ActivePalette;
		
		private bool 	m_active	= false;
		private int 	m_id 		= -1;
		
		public int id
		{
			get { return m_id; }
			set {}
		}
		
		public bool active
		{
			get { return m_active; }
			set 
			{ 
				m_active = value;
				
				if( m_active == false ) 
				{ 
					m_sel_clr_ind = -1; 
				} 
				else 
				{ 
					m_sel_clr_ind = ( m_sel_clr_ind < 0 ) ? 1:m_sel_clr_ind;	// the 1st color slot will be active by default
				}

				update();
			}
		}
		
		private int m_sel_clr_ind 	= -1;
		private byte[] m_clr_inds 	= null;
		
		public int color_slot
		{
			get { return m_sel_clr_ind; }
			set
			{
				if( value >= 0 && value < utils.CONST_PALETTE_SMALL_NUM_COLORS )
				{
					active = true;
					
					m_sel_clr_ind = value;
					
					dispatch_event_active_palette();
					
					update();
				}
			}
		}
		
		public palette_small( int _id, PictureBox _pbox ) : base( _pbox )
		{
			m_id = _id;
			
			m_pix_box.MouseClick += new MouseEventHandler( this.Layout_MouseClick );
			
			update();
		}
		
		private void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_clr_inds != null )
			{
				int clr;
				
				for( int i = 0; i < utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
				{
					clr = palette_group.main_palette[ m_clr_inds[ i ] ];
					
					m_gfx.FillRectangle( new SolidBrush( Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff ) ), ( i * 20 ), 0, 20, 20 );
				}
				
				if( m_sel_clr_ind >= 0 )
				{
					int x = ( m_sel_clr_ind * 20 );
					
					m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
					m_gfx.DrawRectangle( m_pen, x+2, 2, 17, 17 );
					
					m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER;
					m_gfx.DrawRectangle( m_pen, x+1, 1, 19, 19 );
				}
				
				disable( false );
			}
			else
			{
				disable( true );
			}
			
			draw_border( Color.Black );
			
			invalidate();
		}
		
		private void Layout_MouseClick(object sender, MouseEventArgs e)
		{
			active = true;
			
			m_sel_clr_ind = e.X / 20;
			
			dispatch_event_active_palette();

			update();
		}
		
		public void update_color( byte _clr_id, int _slot_id = -1 )
		{
			if( m_clr_inds != null )
			{
				// save the selected color index and update gfx
				if( _slot_id < 0 )
				{
					if( m_active == true )
					{
						m_clr_inds[ m_sel_clr_ind ] = _clr_id;
						
						update();
					}
				}
				else
				{
					m_clr_inds[ _slot_id ] = _clr_id;
					
					update();

				}
			}
		}
		
		public byte[] get_color_inds()
		{
			return m_clr_inds;
		}
		
		public void set_color_inds( byte[] _plt )
		{
			m_clr_inds = _plt;
			
			update();
		}		

		private void dispatch_event_active_palette()
		{
			if( ActivePalette != null )
			{
				ActivePalette( this, null );
			}
		}
	}
}
