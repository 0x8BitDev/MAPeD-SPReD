/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 02.04.2021
 * Time: 18:24
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of tile_list.
	/// </summary>
	/// 
	
	public class tile_list_manager
	{
		public readonly List< tile_list >	m_objs; 
		
		public tile_list_manager()
		{
			m_objs = new List<tile_list>( 4 );
		}
		
		public void update_all()
		{
			foreach( var obj in m_objs )
			{
				obj.update();
			}
		}
		
		public void register( tile_list _tl )
		{
			m_objs.Add( _tl );
		}
		
		public void copy_tile( tile_list.e_data_type _type, int _from_ind, int _to_ind )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.copy_tile( _from_ind, _to_ind );
				}
			}
		}
		
		public void update_tiles( tile_list.e_data_type _type )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.update();
				}
			}
		}

		public void update_tile( tile_list.e_data_type _type, int _tile_id )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.update_tile( _tile_id );
				}
			}
		}
		
		public void visible( tile_list.e_data_type _type, bool _on )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.visible( _on );
				}
			}
		}
		
		public void reset( tile_list.e_data_type _type )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.reset();
				}
			}
		}

		public void select( tile_list.e_data_type _type, int _id )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.select( _id );
				}
			}
		}
	}
	
	public class tile_list : drawable_base
	{
		public enum e_data_type
		{
			Blocks,
			Tiles,
		}
		
		private readonly e_data_type	m_type;
		
		public e_data_type	type
		{
			get { return m_type; }
			set {}
		}
		
		private readonly Control		m_owner;
		private readonly List< Bitmap >	m_img_list;
		private readonly EventHandler	m_event_handler;
		
		private readonly Label			m_label;
		
		private readonly int	m_img_width;
		private readonly int	m_img_height;

		private int m_cursor_tile_ind		= -1;
		private int m_sel_tile_ind			= -1;
		
		private int m_tiles_width_cnt	= -1;
		private int m_tiles_height_cnt	= -1;
		
		private bool m_need_update	= false;
		
		public tile_list( e_data_type _type, FlowLayoutPanel _panel, List< Bitmap > _il, EventHandler _e, ContextMenuStrip _cm, tile_list_manager _tl_cntnr )
		{
			m_type			= _type;
			m_owner			= _panel;
			m_img_list		= _il;
			m_event_handler	= _e;
			
			m_img_width		= _il[ 0 ].Width;
			m_img_height	= _il[ 0 ].Height;
			
			_tl_cntnr.register( this );
			
			PictureBox pbox = new PictureBox();
			
			pbox.Cursor = Cursors.Hand;
			
			_panel.Controls.Clear();
			_panel.Controls.Add( pbox );

			calc_pix_box_size( pbox );
			set_pix_box( pbox );

			m_owner.Resize 		+= on_resize_owner;
			
			// unsubscribe resize event to avoid recursive resizing
			pbox.Resize -= on_resize;

			pbox.MouseUp	+= on_mouse_up;
			pbox.MouseMove	+= on_mouse_move;
			pbox.MouseLeave	+= on_mouse_leave;

			pbox.VisibleChanged	+= on_visible_changed;
			
			pbox.ContextMenuStrip = _cm;
			
			pbox.Tag = this;
			
			// create label
			m_label			= new Label();
			m_label.Text	= "NO DATA";
			m_label.Visible	= false;
			_panel.Controls.Add( m_label );
			
			update();
		}
		
		private void calc_pix_box_size( PictureBox _pbox )
		{
			m_tiles_width_cnt	= ( m_owner.ClientSize.Width - 1 ) / ( m_img_width + 1 );
			m_tiles_width_cnt	= ( m_tiles_width_cnt > 0 ) ? m_tiles_width_cnt:1;
			m_tiles_height_cnt	= ( m_img_list.Count / m_tiles_width_cnt ) + ( ( m_img_list.Count % m_tiles_width_cnt > 0 ) ? 1:0 );
			
			_pbox.Width		= ( m_tiles_width_cnt * ( m_img_width + 1 ) ) + 1;	// +1 right vertical border line
			_pbox.Height	= ( m_tiles_height_cnt * ( m_img_height + 1 ) ) + 1;	// +1 bottom horizontal border line
		}
		
		private void on_resize_owner( object sender, EventArgs e )
		{
			prepare_pix_box();
			
			update();
		}
		
		protected override void prepare_pix_box()
		{
			calc_pix_box_size( m_pix_box );
			
			base.prepare_pix_box();
		}
		
		public int cursor_tile_ind()
		{
			return m_cursor_tile_ind;
		}
		
		public void select( int _id )
		{
			if( m_sel_tile_ind >= 0 )
			{
				// reset selection
				update_tile( m_sel_tile_ind, false );
			}
			
			m_sel_tile_ind = _id;
			
			draw_selection();
		}
		
		public void visible( bool _on )
		{
			m_pix_box.Visible = _on;
			
			m_label.Visible = !_on;
		}
		
		private void on_mouse_up( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			if( m_event_handler != null )
			{
				m_event_handler( this, null );
			}
		}
		
		private void on_mouse_move( object sender, System.Windows.Forms.MouseEventArgs e )
		{
			int pos_x = Math.Min( e.X, m_pix_box.Width - 2 );
			int pos_y = Math.Min( e.Y, m_pix_box.Height - 2 );
			
			if( ( m_cursor_tile_ind >= 0 ) && ( m_sel_tile_ind != m_cursor_tile_ind ) )
			{
				// draw tile to remove selection border
				update_tile( m_cursor_tile_ind, false );
			}
			
			m_cursor_tile_ind = ( pos_x / ( m_img_width + 1 ) ) + ( pos_y / ( ( m_img_height + 1 ) ) ) * m_tiles_width_cnt;
			
			m_cursor_tile_ind = ( m_cursor_tile_ind > m_img_list.Count - 1 ) ? -1:m_cursor_tile_ind;

			if( ( m_cursor_tile_ind >= 0 ) && ( m_sel_tile_ind != m_cursor_tile_ind ) )
			{
				// draw selection border
				draw_tile_border( m_cursor_tile_ind );
			}
		}
		
		private void on_mouse_leave( object sender, System.EventArgs e )
		{
			if( ( m_cursor_tile_ind >= 0 ) && ( m_sel_tile_ind != m_cursor_tile_ind ) )
			{
				// remove selection border
				update_tile( m_cursor_tile_ind );
				
				m_cursor_tile_ind = -1;
			}
		}

		private void on_visible_changed( object sender, System.EventArgs e )
		{
			if( m_pix_box.Visible && m_need_update )
			{
				update();
				
				m_need_update = false;
			}
		}

		private void draw_tile_border( int _ind )
		{
			m_pen.Width = 1.0f;
			m_pen.Color = utils.CONST_COLOR_TILE_LIST_SELECTION;
			
			int pos_x = ( _ind % m_tiles_width_cnt ) * ( m_img_width + 1 );
			int pos_y = ( _ind / m_tiles_width_cnt ) * ( m_img_height + 1 );
			
			m_gfx.DrawRectangle( m_pen, pos_x + 2, pos_y + 2, m_img_width - 1, m_img_height - 1 );
			
			// draw info
			{
				int str_pos_y = pos_y + m_img_height - utils.fnt10_Arial.Height;
				
				string info = String.Format( "{0:X2}", _ind );
				
				utils.brush.Color = Color.FromArgb( unchecked( (int)0xff000000 ) );
				m_gfx.DrawString( info, utils.fnt10_Arial, utils.brush, pos_x + 2, str_pos_y + 1 );
				
				utils.brush.Color = Color.FromArgb( unchecked( (int)0xffffffff ) );
				m_gfx.DrawString( info, utils.fnt10_Arial, utils.brush, pos_x + 1, str_pos_y + 1 );
			}
			
			invalidate();
		}
		
		public void copy_tile( int _from_ind, int _to_ind )
		{
			int pos_x = ( _to_ind % m_tiles_width_cnt ) * ( m_img_width + 1 );
			int pos_y = ( _to_ind / m_tiles_width_cnt ) * ( m_img_height + 1 );
			
			m_img_list[ _to_ind ].Dispose();
			m_img_list[ _to_ind ] = ( Bitmap )m_img_list[ _from_ind ].Clone();
			
			m_gfx.DrawImageUnscaled( m_img_list[ _to_ind ], pos_x + 1, pos_y + 1 );
			
			invalidate();
		}

		public void update_tile( int _ind, bool _draw_selection = true )
		{
			int pos_x = ( _ind % m_tiles_width_cnt ) * ( m_img_width + 1 );
			int pos_y = ( _ind / m_tiles_width_cnt ) * ( m_img_height + 1 );
			
			m_gfx.DrawImageUnscaled( m_img_list[ _ind ], pos_x + 1, pos_y + 1 );
			
			if( _draw_selection )
			{
				draw_selection();
			}
			
			invalidate();
		}
		
		private void draw_selection()
		{
			if( m_sel_tile_ind >= 0 )
			{
				draw_tile_border( m_sel_tile_ind );
			}
		}
		
		public void reset()
		{
			clear_background( utils.CONST_COLOR_TILE_LIST_BACKGROUND );
			
			draw_grid();
			
			invalidate();
		}
		
		public override void update()
		{
			if( m_pix_box.Visible == false )
			{
				m_need_update = true;
				
				return;
			}
			
			clear_background( utils.CONST_COLOR_TILE_LIST_BACKGROUND );
			
			draw_grid();
			
			// draw tiles
			{
				int step_x = m_img_width + 1;
				int step_y = m_img_height + 1;
				
				for( int i = 0; i < m_img_list.Count; i++ )
				{
					m_gfx.DrawImage( 	m_img_list[ i ], 
										( ( i % m_tiles_width_cnt ) * ( step_x ) ) + 1,
										( ( i / m_tiles_width_cnt ) * ( step_y ) ) + 1,
										m_img_width, 
										m_img_height );
				}

				draw_selection();
			}
			
			invalidate();
		}

		private void draw_grid()
		{
			int i;
			int x;
			int y;
			
			m_pen.Width = 1;
			m_pen.Color = utils.CONST_COLOR_TILE_LIST_GRID;
			
			for( i = 0; i < m_tiles_width_cnt + 1; i++ )
			{
				x = i * ( m_img_width + 1 ) + 1;
				
				m_gfx.DrawLine( m_pen, x, 0, x, m_pix_box.Height );
			}
			
			for( i = 0; i < m_tiles_height_cnt + 1; i++ )
			{
				y = i * ( m_img_height + 1 ) + 1;
				
				m_gfx.DrawLine( m_pen, 0, y, m_pix_box.Width, y );
			}
		}
	}
}
