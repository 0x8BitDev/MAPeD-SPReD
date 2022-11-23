/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 22.11.2022
 * Time: 12:46
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor_helper_scr_multisel.
	/// </summary>
	/// 
	
	public class layout_editor_helper_scr_multisel : layout_editor_helper_base
	{
		private bool		m_scr_multiselect_start		= false;
		
		private int			m_scr_multisel_rect_end_x	= -1;
		private int			m_scr_multisel_rect_end_y	= -1;
		
		private Rectangle	m_selection_rect;
		
		public layout_editor_helper_scr_multisel( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			m_selection_rect = new Rectangle();
		}
		
		public override void	reset( bool _init )
		{
			m_scr_multiselect_start = false;
		}
		
		public override void	mouse_down( object sender, MouseEventArgs e )
		{
			m_scr_multiselect_start = true;
			
			m_shared.pix_box_reset_capture();
		
			m_scr_multisel_rect_end_x = e.X;
			m_scr_multisel_rect_end_y = e.Y;
		}
		
		public override void	mouse_up( object sender, MouseEventArgs e )
		{
			m_scr_multiselect_start = false;
			
			m_owner.update();
		}
		
		public override void	mouse_move( object sender, MouseEventArgs e )
		{
			if( m_scr_multiselect_start )
			{
				m_scr_multisel_rect_end_x = e.X;
				m_scr_multisel_rect_end_y = e.Y;
				
				m_selection_rect.X 		= Math.Min( m_shared.m_last_mouse_x, m_scr_multisel_rect_end_x );
				m_selection_rect.Y 		= Math.Min( m_shared.m_last_mouse_y, m_scr_multisel_rect_end_y );
				m_selection_rect.Width	= Math.Abs( m_shared.m_last_mouse_x - m_scr_multisel_rect_end_x );
				m_selection_rect.Height	= Math.Abs( m_shared.m_last_mouse_y - m_scr_multisel_rect_end_y );
				
				m_shared.m_sel_screens_slot_ids.Clear();
				
				m_shared.visible_screens_data_proc( delegate( int _scr_slot_ind, layout_screen_data _scr_data, Rectangle _scr_rect )
				{
					if( _scr_rect.IntersectsWith( m_selection_rect ) || _scr_rect.Contains( m_selection_rect ) )
					{
						m_shared.m_sel_screens_slot_ids.Add( _scr_slot_ind );
					}
				});
			}
		}
		
		public override void	draw( Graphics _gfx, Pen _pen, int _scr_size_width, int _scr_size_height )
		{
			// draw multiselection rectangle
			if( m_scr_multiselect_start )
			{
				_pen.Width = 2;
				_pen.Color = utils.CONST_COLOR_SCREEN_SELECTION_AREA;
				_pen.DashStyle = DashStyle.Dash; 
				
				_gfx.DrawRectangle( _pen, Math.Min( m_shared.m_last_mouse_x, m_scr_multisel_rect_end_x ), Math.Min( m_shared.m_last_mouse_y, m_scr_multisel_rect_end_y ), Math.Abs( m_shared.m_last_mouse_x - m_scr_multisel_rect_end_x ), Math.Abs( m_shared.m_last_mouse_y - m_scr_multisel_rect_end_y ) );
				
				_pen.DashStyle = DashStyle.Solid;
			}
		}
		
		public override bool	check_key_code( KeyEventArgs e )
		{
			return e.KeyCode == Keys.ShiftKey;
		}
	}
}
