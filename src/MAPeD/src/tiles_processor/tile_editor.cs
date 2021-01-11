/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 13:12
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace MAPeD
{
	/// <summary>
	/// Description of tile_editor.
	/// </summary>
	/// 
	
	public class tile_editor : drawable_base
	{
		public event EventHandler NeedGFXUpdate;
		public event EventHandler UpdateSelectedBlock;
		
		private tiles_data	m_tiles_data	= null;
		
		private int m_selected_quad_ind	= -1;
		private int m_selected_tile_id	= -1;
		
		private bool m_locked	= false;
		
		private SolidBrush 	m_brush 		= new SolidBrush( utils.CONST_COLOR_TILE_EDITOR_TRANSLUCENT_QUAD );
		private Region 		m_tile_region	= null;
		
		public tile_editor( PictureBox _pbox ) : base( _pbox )
		{
			m_pix_box.MouseClick 	+= new MouseEventHandler( this.TileEditor_MouseClick );
			
			m_tile_region = new Region( new Rectangle( 0, 0, m_pix_box.Width, m_pix_box.Height ) );
			
			update();
		}
		
		public void locked( bool _on )
		{
			m_locked = _on;
			
			update();
		}
		
		private void TileEditor_MouseClick(object sender, MouseEventArgs e)
		{
			if( m_locked == false )
			{
				m_selected_quad_ind = ( e.X >> 6 ) + 2 * ( e.Y >> 6 );
	
				if( UpdateSelectedBlock != null )
				{
					UpdateSelectedBlock( this, new EventArg2Params( m_tiles_data.get_tile_block( m_selected_tile_id, m_selected_quad_ind ), m_tiles_data ) );
				}
				
				update();
				
				update_status_bar();
			}
		}
		
		public void subscribe_event( block_editor _block_editor )
		{
			_block_editor.DataChanged += new EventHandler( update_data );
		}
		
		private void update_data( object sender, EventArgs e )
		{
			update();
			
			update_status_bar();
		}
		
		public void subscribe_event( tiles_processor _tiles_proc )
		{
			_tiles_proc.GFXUpdate += new EventHandler( update_gfx );
		}
		
		private void update_gfx( object sender, EventArgs e )
		{
			update();
		}
		
		public void subscribe_event( data_sets_manager _data_mngr )
		{
			_data_mngr.SetTilesData += new EventHandler( new_data_set );
		}
		
		private void new_data_set( object sender, EventArgs e )
		{
			tiles_data data = null;
			
			set_selected_tile( 0, data );
		}
		
		private void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_selected_tile_id >= 0 )
			{
				// draw images...
				{
					byte tile_block;
					
					for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
					{
						tile_block = m_tiles_data.get_tile_block( m_selected_tile_id, i );
						
						utils.update_block_gfx( tile_block, m_tiles_data, m_gfx, m_pix_box.Width >> 2, m_pix_box.Height >> 2, ( ( i % 2 ) << 6 ), ( ( i >> 1 ) << 6 ) );
					}
				}			
			
				// draw grid
				{
					m_pen.Color = utils.CONST_COLOR_TILE_EDITOR_GRID;
					
					int n_lines 	= m_pix_box.Width >> 5;
					int pos;
					
					for( int i = 0; i < n_lines; i++ )
					{
						pos = i << 5;
						
						m_gfx.DrawLine( m_pen, pos, 0, pos, m_pix_box.Width );
						m_gfx.DrawLine( m_pen, 0, pos, m_pix_box.Height, pos );
					}
					
					m_pen.Color = utils.CONST_COLOR_TILE_EDITOR_BLOCK_BORDER;
					
					pos = 64;//8 << 4;
					m_gfx.DrawLine( m_pen, pos, 0, pos, m_pix_box.Width );
					m_gfx.DrawLine( m_pen, 0, pos, m_pix_box.Height, pos );			
				}
				
				draw_border( Color.Black );
				
				if( m_locked != true && m_selected_quad_ind >= 0 )
				{
					int x = ( ( m_selected_quad_ind % 2 ) << 6 );
					int y = ( ( m_selected_quad_ind >> 1 ) << 6 );
					
					int quad_width = m_pix_box.Width >> 1;
					
					m_pen.Color = utils.CONST_COLOR_TILE_EDITOR_SELECTED_INNER_BORDER;
					m_gfx.DrawRectangle( m_pen, x+2, y+2, quad_width - 3, quad_width - 3 );
					
					m_pen.Color = utils.CONST_COLOR_TILE_EDITOR_SELECTED_OUTER_BORDER;
					m_gfx.DrawRectangle( m_pen, x+1, y+1, quad_width - 1, quad_width - 1 );
				}
				
				if( m_locked )
				{
					m_gfx.FillRegion( m_brush, m_tile_region );
					
					m_pen.Color = utils.CONST_COLOR_TILE_EDITOR_LOCKED_LINE;
					
					m_gfx.DrawLine( m_pen, 0, 0, m_pix_box.Width, m_pix_box.Height );
				}
				
				disable( false );
			}
			else
			{
				disable( true );
			}
			
			invalidate();
		}
		
		public int get_selected_tile_id()
		{
			return m_selected_tile_id;
		}
		
		public int get_selected_block_pos()
		{
			return m_selected_quad_ind;
		}
		
		public void set_selected_tile( int _tile_id, tiles_data _data )
		{
			if( _data != null )
			{
				m_selected_tile_id = _tile_id;
				
				m_tiles_data = _data;
				
				m_selected_quad_ind	= 0;
				
				update_status_bar();

				if( m_locked == false && ( UpdateSelectedBlock != null ) )
				{
					UpdateSelectedBlock( this, new EventArg2Params( m_tiles_data.get_tile_block( m_selected_tile_id, m_selected_quad_ind ), m_tiles_data ) );
				}
			}
			else
			{
				m_selected_tile_id = -1;

				m_tiles_data = null;				
			}
			
			update();
		}

		public void set_selected_block( int _block_id, tiles_data _data )
		{
			if( m_locked == false && _data != null && m_selected_tile_id >= 0 )
			{
				_data.set_tile_block( m_selected_tile_id, m_selected_quad_ind, (byte)_block_id );

				dispatch_event_need_gfx_update();
				
				update_status_bar();
				
				update();				
			}
		}
		
		private void dispatch_event_need_gfx_update()
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}
		
		private void update_status_bar()
		{
			if( m_tiles_data != null && m_tiles_data.tiles != null )
			{
				MainForm.set_status_msg( String.Format( "Tile Editor: Tile: #{0:X2} \\ Block: #{1:X2}", m_selected_tile_id, m_tiles_data.get_tile_block( m_selected_tile_id, m_selected_quad_ind ) ) );
			}
		}
	}
}
