/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
	
	public class palette_small : drawable_base
	{
		public event EventHandler ActivePalette;
		
#if DEF_COLORS_COPY_PASTE
		public event EventHandler NeedGFXUpdate;
#endif
		
		private bool 	m_active	= false;
		
		private readonly int m_id;
		
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
#if DEF_ZX
					m_sel_clr_ind = ( m_sel_clr_ind < 0 ) ? 0:m_sel_clr_ind;	// the 0 color slot will be active by default on ZX (no opacity color!)
#else
					m_sel_clr_ind = ( m_sel_clr_ind < 0 ) ? 1:m_sel_clr_ind;	// the 1st color slot will be active by default
#endif
				}

				update();
			}
		}
		
#if DEF_COLORS_COPY_PASTE
		private static int m_copied_clr_ind	 = -1;
#endif
		private int m_sel_clr_ind 	= -1;
		private int[] m_clr_inds 	= null;
		
		public palette_small( int _id, PictureBox _pbox ) : base( _pbox )
		{
			m_id = _id;
			
			m_pix_box.MouseClick += new MouseEventHandler( on_mouse_click );
			
			update();
			
#if DEF_COLORS_COPY_PASTE
			ToolStripItem item_copy_color	= utils.get_context_menu_item_by_name( _pbox.ContextMenuStrip, "Copy" );
			ToolStripItem item_paste_color	= utils.get_context_menu_item_by_name( _pbox.ContextMenuStrip, "Paste" );
			
			item_paste_color.Enabled = false;	// disabled by default, there is nothing to paste
			
			item_copy_color.Click	+= new EventHandler( on_context_menu_item_copy_click );
			item_paste_color.Click	+= new EventHandler( on_context_menu_item_paste_click );
			
			m_pix_box.MouseDown += new MouseEventHandler( on_mouse_down );
#else
			_pbox.ContextMenuStrip = null;
#endif
		}
#if DEF_COLORS_COPY_PASTE
		private void on_mouse_down( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				if( ( sender as PictureBox ) == m_pix_box )
				{
					on_mouse_click( sender, e );
					
					if( m_copied_clr_ind >= 0 )
					{
						utils.get_context_menu_item_by_name( m_pix_box.ContextMenuStrip, "Paste" ).Enabled = true;
					}
				}
			}
		}
		
		private void on_context_menu_item_copy_click( object sender, EventArgs e )
		{
			if( m_sel_clr_ind >= 0 )
			{
				m_copied_clr_ind = m_clr_inds[ m_sel_clr_ind ];
			}
		}
		
		private void on_context_menu_item_paste_click( object sender, EventArgs e )
		{
			if( m_sel_clr_ind >= 0 )
			{
				m_clr_inds[ m_sel_clr_ind ] = m_copied_clr_ind;
				
				dispatch_event_need_gfx_update();
				
				update();
			}
		}
#endif
		public int get_color_slot()
		{ 
			return m_sel_clr_ind; 
		}
		
		public void set_color_slot( int _val, bool _dispatch_event = true )
		{
			if( _val >= 0 && _val < utils.CONST_PALETTE_SMALL_NUM_COLORS )
			{
				active = true;
				
				m_sel_clr_ind = _val;
				
				if( _dispatch_event )
				{
					dispatch_event_active_palette();
				}
				
				update();
			}
		}
		
		public override void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_clr_inds != null )
			{
				int clr;
				
				for( int i = 0; i < utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
				{
					clr = palette_group.Instance.main_palette[ m_clr_inds[ i ] ];
					
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					m_gfx.FillRectangle( utils.brush, ( i * 20 ), 0, 20, 20 );
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
		
		private void on_mouse_click( object sender, MouseEventArgs e )
		{
			active = true;
			
			m_sel_clr_ind = e.X / 20;
			
			MainForm.set_status_msg( utils.hex( "Selected color: #", m_clr_inds[ m_sel_clr_ind ] ) );
			
			dispatch_event_active_palette();
			
			update();
		}
		
		public void update_color( int _clr_id, int _slot_id = -1 )
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
		
		public int[] get_color_inds()
		{
			return m_clr_inds;
		}

		public int get_selected_slot_color_id()
		{
			return m_clr_inds[ m_sel_clr_ind ];
		}
		
		public void set_color_inds( int[] _plt )
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
		
#if DEF_COLORS_COPY_PASTE
		private void dispatch_event_need_gfx_update()
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}
#endif
	}
}
