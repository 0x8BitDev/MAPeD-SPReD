/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 20.10.2022
 * Time: 16:08
 */
using System;
using System.Windows.Forms;

using SkiaSharp;

namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor_screen_list.
	/// </summary>
	public class layout_editor_screen_list : layout_editor_behaviour_base
	{
		private event EventHandler ResetSelectedScreen;
		
		private int	m_active_screen_index	= layout_data.CONST_EMPTY_CELL_ID;
		
		private string m_sel_scr_msg = "";
		
		public layout_editor_screen_list( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			//...
		}
		
		public override void reset( bool _init )
		{
			set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
		}
		
		private void reset_selected_screen()
		{
			if( ResetSelectedScreen != null )
			{
				ResetSelectedScreen( this, null );
				
				m_shared.set_high_quality_render_mode( true );
			}
		}
		
		public override bool mouse_down( object sender, MouseEventArgs e )
		{
			//...
			
			return true;
		}
		
		public override void mouse_up( object sender, MouseEventArgs e )
		{
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID )
				{
					layout_screen_data scr_data = m_shared.m_layout.get_data( m_shared.get_sel_scr_pos_x(), m_shared.get_sel_scr_pos_y() );
					
					scr_data.m_scr_ind = m_active_screen_index;
				}
			}
			else
			{
				// reset selection
				reset_selected_screen();
			}
		}
		
		public override bool mouse_move( object sender, MouseEventArgs e )
		{
			m_sel_scr_msg = "[...]";
			
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				int	scr_bank_ind;
				
				layout_screen_data scr_data = m_shared.m_layout.get_data( m_shared.m_sel_screen_slot_id % m_shared.m_layout.get_width(), m_shared.m_sel_screen_slot_id / m_shared.m_layout.get_width() );
				
				if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					scr_bank_ind	= m_shared.get_bank_ind_by_global_screen_ind( scr_data.m_scr_ind );
					m_sel_scr_msg	= "[" + utils.get_screen_id_str( scr_bank_ind, m_shared.get_local_screen_ind( scr_bank_ind, scr_data.m_scr_ind ) ) + "]";
				}

				if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID )
				{
					scr_bank_ind	= m_shared.get_bank_ind_by_global_screen_ind( m_active_screen_index );
					m_sel_scr_msg	= "[" + utils.get_screen_id_str( scr_bank_ind, m_shared.get_local_screen_ind( scr_bank_ind, m_active_screen_index ) ) + "]" + " -> " + m_sel_scr_msg;
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
			m_sel_scr_msg = "[...]";
		}
		
		public override void mouse_wheel( object sender, EventArgs e )
		{
			//...
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
			if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID )
			{
				m_shared.sys_msg( "'Esc' - cancel selection" );
				
				if( m_shared.m_sel_screen_slot_id >= 0 )
				{
					int x = ( int )m_shared.screen_pos_x_by_slot_id( m_shared.get_sel_scr_pos_x() );
					int y = ( int )m_shared.screen_pos_y_by_slot_id( m_shared.get_sel_scr_pos_y() );
					
					// draw a transparent screen image
					if( m_shared.m_scr_list.count() > 0 )
					{
						_image_paint.ColorFilter = m_shared.m_color_filter;
						
						utils.draw_skbitmap( _surface.Canvas, m_shared.m_scr_list.get_skbitmap( m_active_screen_index ), x, y, _scr_size_width, _scr_size_height, _image_paint );
						
						_image_paint.ColorFilter = null;
					}
					
					// draw screen border 
					_line_paint.StrokeWidth = 2;
					_line_paint.Color = utils.CONST_COLOR_SCREEN_TRANSPARENT_BORDER;
					
					_surface.Canvas.DrawRect( x, y, _scr_size_width, _scr_size_height, _line_paint );
				}
			}
			
			m_shared.print( m_sel_scr_msg, 0, 10 );
		}
		
		public override Object get_param( uint _param )
		{
			//throw new Exception( "Unknown parameter detected!\n\n[layout_editor_screen_list.get_param]" );
			return null;
		}
		
		public override bool set_param( uint _param, Object _val )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SET_SCR_ACTIVE:
					{
						m_active_screen_index = unchecked( Convert.ToInt32( _val ) );
						
						m_sel_scr_msg = "[...]";
					}
					break;
					
				default:
					{
						throw new Exception( "Unknown parameter detected!\n\n[layout_editor_screen_list.set_param]" );
					}
			}
			
			return true;
		}
		
		public override void subscribe( uint _param, Action< object, EventArgs > _method )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SUBSCR_SCR_RESET_SELECTED:
					{
						this.ResetSelectedScreen += new EventHandler( _method );
					}
					break;
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_screen_list.subscribe]" );
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
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				reset_selected_screen();
			}
		}
	}
}
