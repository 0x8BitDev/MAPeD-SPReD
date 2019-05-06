/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 17.03.2017
 * Time: 14:57
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of palette_group.
	/// </summary>
	///

	public delegate void UpdateColor();	
	
	public class palette_group : drawable_base
	{
		public event EventHandler UpdateColor;
		
		private byte m_sel_clr_ind		= 13;
		private int m_active_plt_id		= -1;
		
		public int active_palette
		{
			get { return m_active_plt_id; }
			set 
			{
				m_active_plt_id = value;
				
				palette_small plt;
				
				// reset the active flag of the rest palettes, except of the current one
				for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
				{
					if( i == m_active_plt_id )
					{
						plt = m_plt_arr[ i ]; 
						plt.active = true;
						
						m_sel_clr_ind = (byte)plt.get_color_inds()[ plt.color_slot ];						
						
						update();
					}
					else
					{
						m_plt_arr[ i ].active = false;
					}
					
					//m_plt_arr[ i ].active = ( i == m_active_plt_id ) ? true:false;
				}
			}
		}
		
		private palette_small[] m_plt_arr	= null;
		
		public static readonly int[] main_palette = new int[]{ 	0x7C7C7C, 0x0000FC, 0x0000BC, 0x4428BC, 0x940084, 0xA80020, 0xA81000, 0x881400,
																0x503000, 0x007800, 0x006800, 0x005800, 0x004058, 0x000000, 0x000000, 0x000000,
																0xBCBCBC, 0x0078F8, 0x0058F8, 0x6844FC, 0xD800CC, 0xE40058, 0xF83800, 0xE45C10,
																0xAC7C00, 0x00B800, 0x00A800, 0x00A844, 0x008888, 0x000000, 0x000000, 0x000000,
																0xF8F8F8, 0x3CBCFC, 0x6888FC, 0x9878F8, 0xF878F8, 0xF85898, 0xF87858, 0xFCA044,
																0xF8B800, 0xB8F818, 0x58D854, 0x58F898, 0x00E8D8, 0x787878, 0x000000, 0x000000,
																0xFCFCFC, 0xA4E4FC, 0xB8B8F8, 0xD8B8F8, 0xF8B8F8, 0xF8A4C0, 0xF0D0B0, 0xFCE0A8,
																0xF8D878, 0xD8F878, 0xB8F8B8, 0xB8F8D8, 0x00FCFC, 0xF8D8F8, 0x000000, 0x000000 };
		// silly singleton...
		private static palette_group instance = null;
		
		public static  palette_group Instance
		{
			get
			{
				return instance;
			}
		}
		
		public palette_group( 	PictureBox _main_plt, 
								PictureBox _plt0, 
								PictureBox _plt1, 
								PictureBox _plt2, 
								PictureBox _plt3 ) : base( _main_plt )
		{
			instance = this;
			
			m_plt_arr = new palette_small[ utils.CONST_NUM_SMALL_PALETTES ] { 	new palette_small( 0, _plt0 ), 
																				new palette_small( 1, _plt1 ),
																			 	new palette_small( 2, _plt2 ),
																				new palette_small( 3, _plt3 ) };
									
			for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
			{
				m_plt_arr[ i ].ActivePalette += new EventHandler( update_palettes );
			}
			
			m_pix_box.MouseClick += new MouseEventHandler( this.Layout_MouseClick );
			
			update();
		}
		
		public void subscribe_event( data_sets_manager _data_mngr )
		{
			_data_mngr.SetTilesData += new EventHandler( update_data );
		}
		
		private void update_data( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			tiles_data data = data_mngr.get_tiles_data( data_mngr.tiles_data_pos );
			
			set_palette( data );

			dispatch_event_update_color();
			
			update();
		}
		
		public void set_palette( tiles_data _data )
		{
			if( _data != null )
			{
				m_plt_arr[ 0 ].set_color_inds( _data.palette0 );
				m_plt_arr[ 1 ].set_color_inds( _data.palette1 );
				m_plt_arr[ 2 ].set_color_inds( _data.palette2 );
				m_plt_arr[ 3 ].set_color_inds( _data.palette3 );
			}
			else
			{
				m_plt_arr[ 0 ].set_color_inds( null );
				m_plt_arr[ 1 ].set_color_inds( null );
				m_plt_arr[ 2 ].set_color_inds( null );
				m_plt_arr[ 3 ].set_color_inds( null );
			}
		}
		
		private void update()
		{
			int clr;
			
			for( int i = 0; i < utils.CONST_PALETTE_COMMON_NUM_COLORS; i++ )
			{
				clr = main_palette[ i ];
				
				m_gfx.FillRectangle( new SolidBrush( Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff ) ), ( i << 4 )%256, ( i>>4 ) << 4, 16, 16 );
			}
			
			if( m_sel_clr_ind >= 0 )
			{
				int x = ( ( m_sel_clr_ind % 16 ) << 4 );
				int y = ( ( m_sel_clr_ind >> 4 ) << 4 );
				
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+2, y+2, 13, 13 );
				
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+1, y+1, 15, 15 );
			}
			
			draw_border( Color.Black );
			
			invalidate();
		}
		
		private void Layout_MouseClick(object sender, MouseEventArgs e)
		{
			m_sel_clr_ind = (byte)(( e.X >> 4 ) + 16 * ( e.Y >> 4 ));
			
			update();
			
			// dispatch an index of a selected color to the active palette
			if( m_active_plt_id >= 0 )
			{
				palette_small plt = m_plt_arr[ m_active_plt_id ];
				
				if( plt.color_slot == 0 )
				{
					// if active slot is a zero number, fill zero slot of the all palettes by a selected color
					for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
					{
						m_plt_arr[ i ].update_color( m_sel_clr_ind, 0 );
					}
				}
				else
				{
					m_plt_arr[ m_active_plt_id ].update_color( m_sel_clr_ind );
				}
				
				dispatch_event_update_color();				
			}
		}
		
		private void dispatch_event_update_color()
		{
			// update color event
			if( UpdateColor != null )
			{
				UpdateColor( this, System.Windows.Forms.MouseEventArgs.Empty );
			}
		}
		
		private void update_palettes(object sender, EventArgs e)
		{
			palette_small plt = sender as palette_small;
			
			active_palette = plt.active ? plt.id:-1;
			
			m_sel_clr_ind = (byte)plt.get_color_inds()[ plt.color_slot ];
			
			update();
		}
		
		public palette_small[] get_palettes_arr()
		{
			return m_plt_arr;
		}
		
		public int get_selected_color_ind()
		{
			return m_sel_clr_ind;
		}
		
		public void key_event(object sender, KeyEventArgs e)
		{
			if( e.Modifiers == Keys.Shift )
			{
				if( e.KeyCode == Keys.D1 )
				{
					active_palette = 0;
					m_plt_arr[ active_palette ].color_slot = m_plt_arr[ active_palette ].color_slot;
				}
				else
				if( e.KeyCode == Keys.D2 )
				{
					active_palette = 1;
					m_plt_arr[ active_palette ].color_slot = m_plt_arr[ active_palette ].color_slot;
				}
				else
				if( e.KeyCode == Keys.D3 )
				{
					active_palette = 2;
					m_plt_arr[ active_palette ].color_slot = m_plt_arr[ active_palette ].color_slot;
				}
				else
				if( e.KeyCode == Keys.D4 )
				{
					active_palette = 3;
					m_plt_arr[ active_palette ].color_slot = m_plt_arr[ active_palette ].color_slot;
				}
			}
					
			if( e.Modifiers == Keys.Control )
			{
				if( active_palette >= 0 )
				{
					if( e.KeyCode == Keys.D1 )
					{
						this.m_plt_arr[ active_palette ].color_slot = 0;
					}
					else
					if( e.KeyCode == Keys.D2 )
					{
						this.m_plt_arr[ active_palette ].color_slot = 1;
					}
					else
					if( e.KeyCode == Keys.D3 )
					{
						this.m_plt_arr[ active_palette ].color_slot = 2;
					}
					else
					if( e.KeyCode == Keys.D4 )
					{
						this.m_plt_arr[ active_palette ].color_slot = 3;
					}
				}
			}
		}
	}
}
