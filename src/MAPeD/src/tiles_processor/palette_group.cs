﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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

	public class palette_group : drawable_base
	{
		public event EventHandler NeedGFXUpdate;
#if !DEF_ZX
		public event EventHandler UpdateColor;
		
		private readonly ToolTip	m_clr_ttip;
		
		private Point	m_mouse_old_pos = Point.Empty;
#endif
		
		private bool m_mouse_capt		= false;
		
#if DEF_NES
		private int m_sel_clr_ind		= 13;
#else
		private int m_sel_clr_ind		= 0;
#endif		
		private int m_active_plt_id		= -1;
		
#if DEF_ZX
		private int m_paper_active_plt_id = -1;
		
		public int paper_active_palette
		{
			get { return m_paper_active_plt_id; }
		}
#endif		
		public int active_palette
		{
			get { return m_active_plt_id; }
			set 
			{
#if DEF_ZX
				if( value < 2 )
				{
					m_active_plt_id = value;
					
					if( value >= 0 && m_paper_active_plt_id < 0 )
					{
						m_paper_active_plt_id = utils.CONST_ZX_DEFAULT_PAPER_COLOR >> 2;
						m_plt_arr[ m_paper_active_plt_id ].set_color_slot( utils.CONST_ZX_DEFAULT_PAPER_COLOR & 0x03 );
					}
				}
				else
				{
					m_paper_active_plt_id = value;
				}
#else
				m_active_plt_id = value;
#endif
				palette_small plt;
				
				// reset the active flag of the rest palettes, except of the current one
				for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
				{
					if( i == value )
					{
						plt = m_plt_arr[ i ]; 
						plt.active = true;
#if !DEF_ZX
						m_sel_clr_ind = plt.get_color_inds()[ plt.get_color_slot() ];
#endif
						update();
					}
					else
					{
#if DEF_ZX
						if( ( value < 2 && i < 2 ) || ( value > 1 && i > 1 ) )
						{
							m_plt_arr[ i ].active = false;
						}
#else
						m_plt_arr[ i ].active = false;
#endif
					}
					
					//m_plt_arr[ i ].active = ( i == m_active_plt_id ) ? true:false;
				}
			}
		}
		
		private readonly palette_small[]	m_plt_arr;
		private readonly int[]				m_main_palette;
		
		public int[] main_palette
		{
			get { return m_main_palette; }
		}

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
			foreach( var plt in m_plt_arr )
			{
				plt.ActivePalette += new EventHandler( on_update_palettes );
			}
			
			m_pix_box.MouseDown		+= new MouseEventHandler( on_mouse_down );
			m_pix_box.MouseMove		+= new MouseEventHandler( on_mouse_move );
			m_pix_box.MouseUp		+= new MouseEventHandler( on_mouse_up );
			m_pix_box.MouseClick	+= new MouseEventHandler( on_mouse_click );
			
			m_main_palette = platform_data.get_palette_by_platform_type( platform_data.get_platform_type() );
			
#if 	!DEF_ZX
			m_pix_box.MouseLeave	+= new EventHandler( on_mouse_leave );
			
			m_clr_ttip = new ToolTip();
#endif	//!DEF_ZX
			
			update();
		}
		
		public void subscribe_need_gfx_update_event( Action< object, EventArgs > _method )
		{
			this.NeedGFXUpdate += new EventHandler( _method );
			
#if DEF_COLORS_COPY_PASTE
			foreach( var plt in m_plt_arr )
			{
				// to enable 'Update GFX' button when the 'Block Editor' has no active data to edit
				plt.NeedGFXUpdate += new EventHandler( _method );
			}
#endif
		}
		
		public void subscribe( data_sets_manager _data_mngr )
		{
			_data_mngr.SetTilesData += new EventHandler( on_update_data );
		}
		
		private void on_update_data( object sender, EventArgs e )
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
		
		public override void update()
		{
			int clr;
			
			int plt_clrs_cnt = platform_data.get_main_palette_colors_cnt();
			
			for( int i = 0; i < plt_clrs_cnt; i++ )
			{
				clr = main_palette[ i ];
				
				utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
#if DEF_NES		// row ordered data				
				m_gfx.FillRectangle( utils.brush, ( i << 4 )%256, ( i>>4 ) << 4, 16, 16 );
#elif DEF_SMS	// column ordered data
				m_gfx.FillRectangle( utils.brush, ( i >> 2 ) << 4, ( i & 0x03 ) << 4, 16, 16 );
#elif DEF_PCE || DEF_SMD
				// column ordered data
				m_gfx.FillRectangle( utils.brush, ( i >> 3 ) << 2, ( i & 0x07 ) << 3, 4, 8 );
#elif DEF_ZX	// row ordered data
				m_gfx.FillRectangle( utils.brush, ( i << 5 )%256, ( i >> 3 ) << 5, 32, 32 );
#endif
			}
			
			if( m_sel_clr_ind >= 0 )
			{
#if DEF_NES		// row ordered data				
				int x = ( ( m_sel_clr_ind % 16 ) << 4 );
				int y = ( ( m_sel_clr_ind >> 4 ) << 4 );
#elif DEF_SMS	// column ordered data
				int x = ( ( m_sel_clr_ind >> 2 ) << 4 );
				int y = ( ( m_sel_clr_ind % 4  ) << 4 );
#elif DEF_PCE || DEF_SMD
				// column ordered data
				int x = ( ( m_sel_clr_ind >> 3 ) << 2 );
				int y = ( ( m_sel_clr_ind % 8  ) << 3 );
#elif DEF_ZX	// row ordered data
				// draw 'disabled' red line
				{
					m_pen.Color = utils.CONST_COLOR_PIXBOX_INACTIVE_CROSS;
					m_gfx.DrawLine( m_pen, 0, 0, m_pix_box.Width, m_pix_box.Height );
				}
				int x = 0;
				int y = ( m_sel_clr_ind >> 3 ) << 5;
#endif

#if !DEF_PCE && !DEF_SMD
#if DEF_ZX
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+2, y+2, 253, 29 );
				
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+1, y+1, 255, 31 );
#else
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+2, y+2, 13, 13 );
				
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+1, y+1, 15, 15 );
#endif
#else
				m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
				m_gfx.DrawRectangle( m_pen, x+1, y+1, 3, 7 );
#endif
			}
			
			draw_border( Color.Black );
			
			invalidate();
		}
		
		private void on_mouse_down( object sender, MouseEventArgs e )
		{
			m_mouse_capt = true;
		}
		
		private void on_mouse_up( object sender, MouseEventArgs e )
		{
			m_mouse_capt = false;
		}
		
		private void on_mouse_move( object sender, MouseEventArgs e )
		{
			if( m_mouse_capt && ( e.X > 0 && e.X < m_pix_box.Bounds.Width && e.Y > 0 && e.Y < m_pix_box.Bounds.Height ) )
			{
				check_color( e.X, e.Y );
#if !DEF_ZX
				if( e.Location != m_mouse_old_pos )
				{
					m_mouse_old_pos = e.Location;
				
					m_clr_ttip.Show( utils.hex( "#", get_sel_clr_ind( e.X, e.Y ) ), m_pix_box, e.X, e.Y - 28 );
				}
#endif //!DEF_ZX
			}
		}
		
		private void on_mouse_click( object sender, MouseEventArgs e )
		{
			check_color( e.X, e.Y );
#if !DEF_ZX
			m_clr_ttip.Show( utils.hex( "#", get_sel_clr_ind( e.X, e.Y ) ), m_pix_box, e.X, e.Y - 28 );
#endif //!DEF_ZX
		}
		
#if !DEF_ZX
		private void on_mouse_leave( object sender, EventArgs e )
		{
			m_clr_ttip.Hide( m_pix_box );
		}
		
		private int get_sel_clr_ind( int _x, int _y )
		{
#if DEF_NES	
			// row ordered data
			return ( _x >> 4 ) + ( ( _y >> 4 ) << 4 );
#elif DEF_SMS
			// column ordered data
			return (( _x >> 4 ) << 2 ) + ( _y >> 4 );
#elif DEF_PCE || DEF_SMD
			// column ordered data
			return (( _x >> 2 ) << 3 ) + ( _y >> 3 );
#endif
		}
#endif	//!DEF_ZX
	
		private void check_color( int _x, int _y )
		{
			if( _x < 0 || _y < 0 || _x >= m_pix_box.Width || _y >= m_pix_box.Height )
			{
				return;
			}
			
#if !DEF_ZX
			int sel_clr_ind = get_sel_clr_ind( _x, _y );
	
			if( m_sel_clr_ind >= 0 && sel_clr_ind != m_sel_clr_ind && m_plt_arr[ 0 ].get_color_inds() != null )
			{
				dispatch_event_need_gfx_update();
			}
			
			m_sel_clr_ind = sel_clr_ind; 
				
			update();
			
			// dispatch an index of a selected color to the active palette
			if( m_active_plt_id >= 0 )
			{
				palette_small plt = m_plt_arr[ m_active_plt_id ];
#if DEF_NES				
				if( plt.get_color_slot() == 0 )
				{
					// if active slot is a zero number, fill zero slot of the all palettes by a selected color
					for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
					{
						m_plt_arr[ i ].update_color( m_sel_clr_ind, 0 );
					}
				}
				else
#endif
				{
					m_plt_arr[ m_active_plt_id ].update_color( m_sel_clr_ind );
				}
				dispatch_event_update_color();
			}
#endif //!DEF_ZX
		}

		private void dispatch_event_need_gfx_update()
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}
		
		private void dispatch_event_update_color()
		{
#if !DEF_ZX
			// update color event
			if( UpdateColor != null )
			{
				UpdateColor( this, MouseEventArgs.Empty );
			}
#endif
		}
		
		public void save_main_palette( BinaryWriter _bw )
		{
			int clr;
			
			int plt_clrs_cnt = platform_data.get_main_palette_colors_cnt();
			
			for( int i = 0; i < plt_clrs_cnt; i++ )
			{
				clr = m_main_palette[ i ];
				
				_bw.Write( ( byte )( ( clr&0x00ff0000 ) >> 16 ) );
				_bw.Write( ( byte )( ( clr&0x0000ff00 ) >> 8 ) );
				_bw.Write( ( byte )( clr&0x000000ff ) );
			}
		}
		
		public void load_main_palette( BinaryReader _br )
		{
			int plt_clrs_cnt = platform_data.get_main_palette_colors_cnt();
			
			for( int i = 0; i < plt_clrs_cnt; i++ )
			{
				m_main_palette[ i ] = _br.ReadByte() << 16 | _br.ReadByte() << 8 | _br.ReadByte();
			}
		}
		
		private void on_update_palettes( object sender, EventArgs e )
		{
			palette_small plt = sender as palette_small;
			
			active_palette = plt.active ? plt.id:-1;
			
			m_sel_clr_ind = plt.get_color_inds()[ plt.get_color_slot() ];
			
			update();
		}
		
		public void update_selected_color()
		{
			palette_small plt;
			
			for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
			{
				plt = m_plt_arr[ i ];
				
				if( plt.active == true )
				{
					m_sel_clr_ind = plt.get_selected_slot_color_id();
						
					update();
					
					break;
				}
			}
		}
		
		public palette_small[] get_palettes_arr()
		{
			return m_plt_arr;
		}
		
		public int get_selected_color_ind()
		{
			return m_sel_clr_ind;
		}
		
		public void on_key_up( object sender, KeyEventArgs e )
		{
			if( e.Modifiers == Keys.Shift )
			{
				if( e.KeyCode == Keys.D1 )
				{
					active_palette = 0;
					m_plt_arr[ active_palette ].set_color_slot( m_plt_arr[ active_palette ].get_color_slot() );
				}
				else
				if( e.KeyCode == Keys.D2 )
				{
					active_palette = 1;
					m_plt_arr[ active_palette ].set_color_slot( m_plt_arr[ active_palette ].get_color_slot() );
				}
				else
				if( e.KeyCode == Keys.D3 )
				{
					active_palette = 2;
#if DEF_ZX
					m_plt_arr[ paper_active_palette ].set_color_slot( m_plt_arr[ paper_active_palette ].get_color_slot() );
#else
					m_plt_arr[ active_palette ].set_color_slot( m_plt_arr[ active_palette ].get_color_slot() );
#endif
				}
				else
				if( e.KeyCode == Keys.D4 )
				{
					active_palette = 3;
#if DEF_ZX
					m_plt_arr[ paper_active_palette ].set_color_slot( m_plt_arr[ paper_active_palette ].get_color_slot() );
#else
					m_plt_arr[ active_palette ].set_color_slot( m_plt_arr[ active_palette ].get_color_slot() );
#endif
				}
			}
					
			if( e.Modifiers == Keys.Control )
			{
				if( active_palette >= 0 )
				{
					if( e.KeyCode == Keys.D1 )
					{
						this.m_plt_arr[ active_palette ].set_color_slot( 0, false );
					}
					else
					if( e.KeyCode == Keys.D2 )
					{
						this.m_plt_arr[ active_palette ].set_color_slot( 1, false );
					}
					else
					if( e.KeyCode == Keys.D3 )
					{
						this.m_plt_arr[ active_palette ].set_color_slot( 2, false );
					}
					else
					if( e.KeyCode == Keys.D4 )
					{
						this.m_plt_arr[ active_palette ].set_color_slot( 3, false );
					}
				}
			}
		}
	}
}
