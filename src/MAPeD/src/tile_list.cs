/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
		
		public void copy_tile( tile_list.EType _type, int _from_ind, int _to_ind )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.copy_tile( _from_ind, _to_ind );
				}
			}
		}
		
		public void update_tiles( tile_list.EType _type )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.update();
				}
			}
		}
		
		public void visible( tile_list.EType _type, bool _on )
		{
			foreach( var obj in m_objs )
			{
				if( obj.type == _type )
				{
					obj.visible( _on );
				}
			}
		}
	}
	
	public class tile_list : drawable_base
	{
		public enum EType
		{
			t_Blocks,
			t_Tiles,
		}
		
		private readonly EType			m_type;
		
		public EType	type
		{
			get { return m_type; }
			set {}
		}
		
		private readonly Control		m_owner;
		private readonly ImageList		m_img_list;
		private readonly EventHandler	m_event_handler;
		
		private readonly Label			m_label;

		private int m_sel_tile_ind		= -1;
		
		private int m_tiles_width_cnt	= -1;
		private int m_tiles_height_cnt	= -1;
		
		private bool m_need_update	= false;
		
		public tile_list( EType _type, FlowLayoutPanel _panel, ImageList _il, EventHandler _e, ContextMenuStrip _cm, tile_list_manager _tl_cntnr )
		{
			m_type			= _type;
			m_owner			= _panel;
			m_img_list		= _il;
			m_event_handler	= _e;
			
			_tl_cntnr.register( this );
			
			PictureBox pbox = new PictureBox();
			
			pbox.Cursor = Cursors.Hand;
			
			_panel.Controls.Clear();
			_panel.Controls.Add( pbox );

			calc_pix_box_size( pbox );
			set_pix_box( pbox );

			m_owner.Resize 		+= ResizeOwner_Event;
			
			// unsubscribe resize event to avoid recursive resizing
			pbox.Resize -= Resize_Event;

			pbox.MouseUp	+= TileList_MouseUp;
			pbox.MouseMove	+= TileList_MouseMove;

			pbox.VisibleChanged	+= VisibleChanged_Event;
			
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
			m_tiles_width_cnt	= ( m_owner.ClientSize.Width - 1 ) / ( m_img_list.ImageSize.Width + 1 );
			m_tiles_height_cnt	= ( m_img_list.Images.Count / m_tiles_width_cnt ) + ( ( m_img_list.Images.Count % m_tiles_width_cnt > 0 ) ? 1:0 );
			
			_pbox.Width		= ( m_tiles_width_cnt * ( m_img_list.ImageSize.Width + 1 ) ) + 1;	// +1 right vertical border line
			_pbox.Height	= ( m_tiles_height_cnt * ( m_img_list.ImageSize.Height + 1 ) ) + 1;	// +1 bottom horizontal border line    
		}
		
		private void ResizeOwner_Event( object sender, EventArgs e )
		{
			prepare_pix_box();
			
			update();
		}
		
		protected override void prepare_pix_box()
		{
			calc_pix_box_size( m_pix_box );
			
			base.prepare_pix_box();
		}
		
		public int selected_tile_ind()
		{
			return m_sel_tile_ind;
		}
		
		public void visible( bool _on )
		{
			m_pix_box.Visible = _on;
			
			m_label.Visible = !_on;
		}
		
		private void TileList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_event_handler != null )
			{
				m_event_handler( this, null );
			}
		}
		
		private void TileList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int pos_x = Math.Min( e.X, m_pix_box.Width - 2 );
			int pos_y = Math.Min( e.Y, m_pix_box.Height - 2 );
			
			if( m_sel_tile_ind >= 0 )
			{
				// draw tile to remove selection border
				update_tile( m_sel_tile_ind );
			}
			
			m_sel_tile_ind = ( pos_x / ( m_img_list.ImageSize.Width + 1 ) ) + ( pos_y / ( ( m_img_list.ImageSize.Height + 1 ) ) ) * m_tiles_width_cnt;
			
			m_sel_tile_ind = ( m_sel_tile_ind > m_img_list.Images.Count - 1 ) ? -1:m_sel_tile_ind;

			if( m_sel_tile_ind >= 0 )
			{
				// draw selection border
				draw_tile_border( m_sel_tile_ind );
			}
		}

		private void VisibleChanged_Event( object sender, System.EventArgs e )
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
			
			int tile_width	= m_img_list.ImageSize.Width;
			int tile_height	= m_img_list.ImageSize.Height;
				
			int pos_x = ( _ind % m_tiles_width_cnt ) * ( tile_width + 1 );
			int pos_y = ( _ind / m_tiles_width_cnt ) * ( tile_height + 1 );
			
			m_gfx.DrawRectangle( m_pen, pos_x + 2, pos_y + 2, tile_width - 1, tile_height - 1 );
			
			// draw info
			{
				int str_pos_y = pos_y + m_img_list.ImageSize.Height - utils.fnt10_Arial.Height;
				
				string info = String.Format( "{0:X2}", m_sel_tile_ind );
				
				utils.brush.Color = Color.FromArgb( unchecked( (int)0xff000000 ) );
				m_gfx.DrawString( info, utils.fnt10_Arial, utils.brush, pos_x + 2, str_pos_y + 1 );
				
				utils.brush.Color = Color.FromArgb( unchecked( (int)0xffffffff ) );
				m_gfx.DrawString( info, utils.fnt10_Arial, utils.brush, pos_x + 1, str_pos_y + 1 );
			}
			
			invalidate();
		}
		
		public void copy_tile( int _from_ind, int _to_ind )
		{
			int tile_width	= m_img_list.ImageSize.Width;
			int tile_height	= m_img_list.ImageSize.Height;
				
			int pos_x = ( _to_ind % m_tiles_width_cnt ) * ( tile_width + 1 );
			int pos_y = ( _to_ind / m_tiles_width_cnt ) * ( tile_height + 1 );
			
			m_img_list.Images[ _to_ind ].Dispose();
			m_img_list.Images[ _to_ind ] = ( Image )m_img_list.Images[ _from_ind ].Clone();
			
			m_gfx.DrawImageUnscaled( m_img_list.Images[ _to_ind ], pos_x + 1, pos_y + 1 );
			
			invalidate();
		}

		public void update_tile( int _ind )
		{
			int tile_width	= m_img_list.ImageSize.Width;
			int tile_height	= m_img_list.ImageSize.Height;
				
			int pos_x = ( _ind % m_tiles_width_cnt ) * ( tile_width + 1 );
			int pos_y = ( _ind / m_tiles_width_cnt ) * ( tile_height + 1 );
			
			m_gfx.DrawImageUnscaled( m_img_list.Images[ _ind ], pos_x + 1, pos_y + 1 );
			
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
			
			int i;
			
			// draw grid
			{
				int x;
				int y;
				
				m_pen.Width = 1;
				m_pen.Color = utils.CONST_COLOR_TILE_LIST_GRID;
				
				for( i = 0; i < m_tiles_width_cnt + 1; i++ )
				{
					x = i * ( m_img_list.ImageSize.Width + 1 ) + 1;
					
					m_gfx.DrawLine( m_pen, x, 0, x, m_pix_box.Height );
				}
				
				for( i = 0; i < m_tiles_height_cnt + 1; i++ )
				{
					y = i * ( m_img_list.ImageSize.Height + 1 ) + 1;
					
					m_gfx.DrawLine( m_pen, 0, y, m_pix_box.Width, y );
				}
			}
			
			// draw tiles
			{
				int step_x = m_img_list.ImageSize.Width + 1;
				int step_y = m_img_list.ImageSize.Height + 1;
				
				for( i = 0; i < m_img_list.Images.Count; i++ )
				{
					m_gfx.DrawImage( 	m_img_list.Images[ i ], 
					                	( ( i % m_tiles_width_cnt ) * ( step_x ) ) + 1,
					                	( ( i / m_tiles_width_cnt ) * ( step_y ) ) + 1,
										m_img_list.ImageSize.Width, 
										m_img_list.ImageSize.Height );
				}
			}
			
			invalidate();
		}
	}
}
