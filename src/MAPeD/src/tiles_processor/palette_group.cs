/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
#endif
		
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
						m_plt_arr[ m_paper_active_plt_id ].color_slot = utils.CONST_ZX_DEFAULT_PAPER_COLOR & 0x03;
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
						m_sel_clr_ind = plt.get_color_inds()[ plt.color_slot ];
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
		
		private readonly palette_small[] m_plt_arr	= null;
		
		private readonly int[] m_main_palette = null;
		
		public int[] main_palette
		{
			get { return m_main_palette; }
		}

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
			int i;
				
			for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
			{
				m_plt_arr[ i ].ActivePalette += new EventHandler( update_palettes );
			}
			
			m_pix_box.MouseClick += new MouseEventHandler( this.Layout_MouseClick );
			
			m_main_palette = platform_data_provider.get_palette_by_file_ext( utils.CONST_FILE_EXT );
			
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
		
		public override void update()
		{
			int clr;
			
			for( int i = 0; i < utils.CONST_PALETTE_MAIN_NUM_COLORS; i++ )
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
		
		private void Layout_MouseClick(object sender, MouseEventArgs e)
		{
#if !DEF_ZX
		
#if DEF_NES	
			// row ordered data
			int sel_clr_ind = ( e.X >> 4 ) + ( ( e.Y >> 4 ) << 4 );
#elif DEF_SMS
			// column ordered data
			int sel_clr_ind = (( e.X >> 4 ) << 2 ) + ( e.Y >> 4 );
#elif DEF_PCE || DEF_SMD
			// column ordered data
			int sel_clr_ind = (( e.X >> 2 ) << 3 ) + ( e.Y >> 3 );
#endif
			MainForm.set_status_msg( utils.hex( "Selected color: #", sel_clr_ind ) );

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
				if( plt.color_slot == 0 )
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
				UpdateColor( this, System.Windows.Forms.MouseEventArgs.Empty );
			}
#endif
		}
		
		public void save_main_palette( BinaryWriter _bw )
		{
			int clr;
			
			for( int i = 0; i < utils.CONST_PALETTE_MAIN_NUM_COLORS; i++ )
			{
				clr = m_main_palette[ i ];
				
				_bw.Write( ( byte )( ( clr&0x00ff0000 ) >> 16 ) );
				_bw.Write( ( byte )( ( clr&0x0000ff00 ) >> 8 ) );
				_bw.Write( ( byte )( clr&0x000000ff ) );
			}
		}
		
		public void load_main_palette( BinaryReader _br )
		{
			for( int i = 0; i < utils.CONST_PALETTE_MAIN_NUM_COLORS; i++ )
			{
				m_main_palette[ i ] = _br.ReadByte() << 16 | _br.ReadByte() << 8 | _br.ReadByte();
			}
		}
		
		private void update_palettes(object sender, EventArgs e)
		{
			palette_small plt = sender as palette_small;
			
			active_palette = plt.active ? plt.id:-1;
			
			m_sel_clr_ind = plt.get_color_inds()[ plt.color_slot ];
			
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
#if DEF_ZX
					m_plt_arr[ paper_active_palette ].color_slot = m_plt_arr[ paper_active_palette ].color_slot;
#else
					m_plt_arr[ active_palette ].color_slot = m_plt_arr[ active_palette ].color_slot;
#endif
				}
				else
				if( e.KeyCode == Keys.D4 )
				{
					active_palette = 3;
#if DEF_ZX
					m_plt_arr[ paper_active_palette ].color_slot = m_plt_arr[ paper_active_palette ].color_slot;
#else
					m_plt_arr[ active_palette ].color_slot = m_plt_arr[ active_palette ].color_slot;
#endif
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
