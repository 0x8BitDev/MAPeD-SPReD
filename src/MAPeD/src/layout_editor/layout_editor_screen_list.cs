/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 20.10.2022
 * Time: 16:08
 */
using System;
using System.Windows.Forms;
using System.Drawing;


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
		
		public override void mouse_down( object sender, MouseEventArgs e )
		{
			//...
		}
		
		public override void mouse_up( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
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
					if( ResetSelectedScreen != null )
					{
						ResetSelectedScreen( this, null );
						
						m_shared.set_high_quality_render_mode( true );
						return;
					}
				}
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
			//...
		}
		
		public override void mouse_wheel( object sender, EventArgs e )
		{
			//...
		}
		
		public override bool block_free_map_panning()
		{
			return true;
		}

		public override bool force_map_drawing()
		{
			return false;
		}

		public override void draw( Graphics _gfx, Pen _pen, int _scr_size_width, int _scr_size_height )
		{
			if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID && m_shared.m_sel_screen_slot_id >= 0 )
			{
				int x = m_shared.screen_pos_x_by_slot_id( m_shared.get_sel_scr_pos_x() );
				int y = m_shared.screen_pos_y_by_slot_id( m_shared.get_sel_scr_pos_y() );
				
				_pen.Color = utils.CONST_COLOR_SCREEN_GHOST_IMAGE_INNER_BORDER;
				_gfx.DrawRectangle( _pen, x+2, y+2, _scr_size_width - 3, _scr_size_height - 3 );
				
				_pen.Color = utils.CONST_COLOR_SCREEN_GHOST_IMAGE_OUTER_BORDER;
				_gfx.DrawRectangle( _pen, x+1, y+1, _scr_size_width - 1, _scr_size_height - 1 );
				
				// draw ghost image
				if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID && m_shared.m_scr_list.count() > 0 )
				{
					m_shared.m_scr_img_rect.X 		= x;
					m_shared.m_scr_img_rect.Y 		= y;
					m_shared.m_scr_img_rect.Width	= _scr_size_width;
					m_shared.m_scr_img_rect.Height 	= _scr_size_height;
					
					_gfx.DrawImage( m_shared.m_scr_list.get( m_active_screen_index ), m_shared.m_scr_img_rect, 0, 0, platform_data.get_screen_width_pixels() << 1, platform_data.get_screen_height_pixels() << 1, GraphicsUnit.Pixel, m_shared.m_scr_img_attr );
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
	}
}
