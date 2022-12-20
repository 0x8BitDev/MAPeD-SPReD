/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 22.11.2022
 * Time: 12:45
 */
using System;
using System.Windows.Forms;

using SkiaSharp;

namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor_helper_panning.
	/// </summary>
	/// 
	
	public class layout_editor_helper_panning : layout_editor_helper_base
	{
		public layout_editor_helper_panning( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			//...
		}
		
		public override void	reset( bool _init )
		{
			//...
		}
		
		public override void	mouse_down( object sender, MouseEventArgs e )
		{
			//...
		}
		
		public override void	mouse_up( object sender, MouseEventArgs e )
		{
			//...
		}
		
		public override void	mouse_move( object sender, MouseEventArgs e )
		{
			if( m_shared.pix_box_captured() )
			{
				m_shared.m_fl_offset_x += ( m_shared.m_mouse_x - m_shared.m_last_mouse_x ) / m_shared.m_scale;
				m_shared.m_fl_offset_y += ( m_shared.m_mouse_y - m_shared.m_last_mouse_y ) / m_shared.m_scale;
				
				m_shared.m_offset_x = ( int )m_shared.m_fl_offset_x;
				m_shared.m_offset_y = ( int )m_shared.m_fl_offset_y;
				
				m_shared.clamp_offsets();
				
				m_shared.m_last_mouse_x	 = m_shared.m_mouse_x;
				m_shared.m_last_mouse_y	 = m_shared.m_mouse_y;
			}
		}
		
		public override void	draw( SKSurface _surface, SKPaint _line_paint, float _scr_size_width, float _scr_size_height )
		{
			//...
		}
		
		public override bool	check_key_code( KeyEventArgs e )
		{
			return e.KeyCode == Keys.ControlKey;
		}
	}
}
