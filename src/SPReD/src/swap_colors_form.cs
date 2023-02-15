/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 24.08.2020
 * Time: 18:09
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace SPReD
{
	/// <summary>
	/// Description of swap_colors_form.
	/// </summary>
	public partial class swap_colors_form : Form
	{
		private readonly Graphics	m_main_gfx;
		private readonly Pen		m_pen;
		
		public static readonly Color	CONST_COLOR_PALETTE_SELECTED_INNER_BORDER		= Color.White;
		public static readonly Color	CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER		= Color.Black; 
		public static readonly Color	CONST_COLOR_PALETTE_SWAP_COLOR_ACTIVE_BORDER	= Color.Red;
		public static readonly Color	CONST_COLOR_PALETTE_SWAP_COLOR_INACTIVE_BORDER	= Color.Black;
		public static readonly Color	CONST_COLOR_PALETTE_SWAP_COLOR_TEXT_DEFAULT		= Color.Black;
		public static readonly Color	CONST_COLOR_PIXBOX_DEFAULT						= Color.White;
		
		private ListBox.ObjectCollection	m_sprites_arr	= null;
		
		private int m_color_A	= -1;
		private int m_color_B	= -1;
		
		public swap_colors_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			PixBoxPalette.MouseClick	+= new MouseEventHandler( on_mouse_click );
			
			PixBoxPalette.Image	= new Bitmap( PixBoxPalette.Width, PixBoxPalette.Height, PixelFormat.Format32bppArgb );
			
			m_main_gfx		= Graphics.FromImage( PixBoxPalette.Image );
			
			m_pen			= new Pen( CONST_COLOR_PIXBOX_DEFAULT );
			m_pen.EndCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.StartCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.Width		= 1;
		}
		
		private void on_mouse_click( object sender, MouseEventArgs e )
		{
			int sel_ind = e.X >> 4;
			
			if( m_color_A >= 0 && m_color_B >= 0 )
			{
				m_color_A = m_color_B = -1; 
			}
			
			if( m_color_A == -1 )
			{
				m_color_A = sel_ind;
			}
			else
			if( m_color_B == -1 )
			{
				m_color_B = sel_ind;
			}
			
			update();
		}
		
		public DialogResult show_window( ListBox.ObjectCollection _obj_arr )
		{
			m_sprites_arr = _obj_arr;
			
			m_color_A = m_color_B = -1;
			
			update();
			
			return ShowDialog();
		}
		
		private void draw_sel_border( int _color_ind )
		{
			int x = _color_ind * 16;

			m_pen.Width = 1;
			
			m_pen.Color = CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
			m_main_gfx.DrawRectangle( m_pen, x + 2, 2, 13, 13 );
			
			m_pen.Color = CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER;
			m_main_gfx.DrawRectangle( m_pen, x + 1, 1, 15, 15 );
		}
		
		private void update()
		{
			if( m_sprites_arr != null )
			{
				int clr;

				palette_group plt_grp		= palette_group.Instance;
				palette_small[] sm_plts_arr	= plt_grp.get_palettes_arr();
				
				for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
				{
					clr = palette_group.Instance.main_palette[ sm_plts_arr[ i >> 2 ].get_color_inds()[ i & 0x03 ] ];
						
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					m_main_gfx.FillRectangle( utils.brush, ( i << 4 ) + 1, 1, 16, 16 );
				}
				
				if( ( m_color_A != -1 ) || ( m_color_B != -1 ) )
				{
					m_pen.Color = CONST_COLOR_PALETTE_SWAP_COLOR_ACTIVE_BORDER;
					m_pen.Width = 2;
					
					m_main_gfx.DrawRectangle( m_pen, 1, 1, PixBoxPalette.Width - 2, PixBoxPalette.Height - 2 );
				}
				else
				{
					m_pen.Color = CONST_COLOR_PALETTE_SWAP_COLOR_INACTIVE_BORDER;
					m_pen.Width = 1;
					
					m_main_gfx.DrawRectangle( m_pen, 0, 0, PixBoxPalette.Width - 1, PixBoxPalette.Height - 1  );
				}
			}

			if( m_color_A >= 0 )
			{
				draw_sel_border( m_color_A );
			}
			
			if( m_color_B >= 0 )
			{
				draw_sel_border( m_color_B );
			}
			
			PixBoxPalette.Invalidate();
		}
		
		private void BtnSwapClick( object sender, EventArgs e )
		{
			if( m_color_A >= 0 && m_color_B >= 0 )
			{
				uint		chr_key;
				CHR_data	chr_data;
				
#if DEF_FIXED_LEN_PALETTE16_ARR
				palettes_array plts_arr = palettes_array.Instance;
#endif
				SortedSet< uint > used_CHR_data = new SortedSet< uint >();
				
				foreach( sprite_data spr in m_sprites_arr )
				{
					foreach( CHR_data_attr attr in spr.get_CHR_attr() )
					{
#if DEF_FIXED_LEN_PALETTE16_ARR
						if( attr.palette_ind == plts_arr.palette_index )
						{
#endif
							chr_key = ( uint )( ( ( spr.get_CHR_data().id & 0x0000ffff ) << 16 ) | ( attr.CHR_ind & 0x0000ffff ) );
							
							if( !used_CHR_data.Contains( chr_key ) )
							{
								chr_data = spr.get_CHR_data().get_data()[ attr.CHR_ind ];
								
								for( int pix_n = 0; pix_n < utils.CONST_CHR_TOTAL_PIXELS_CNT; pix_n++ )
								{
									if( chr_data.get_data()[ pix_n ] == m_color_A )
									{
										chr_data.get_data()[ pix_n ] = ( byte )m_color_B;
									}
									else
									if( chr_data.get_data()[ pix_n ] == m_color_B )
									{
										chr_data.get_data()[ pix_n ] = ( byte )m_color_A;
									}
								}
								
								used_CHR_data.Add( chr_key );
							}
#if DEF_FIXED_LEN_PALETTE16_ARR
						}
#endif
					}
				}
			
#if DEF_FIXED_LEN_PALETTE16_ARR
				plts_arr.swap_colors( m_color_A, m_color_B );
				plts_arr.update_palette();
#else
				palette_group plt_grp		= palette_group.Instance;
				palette_small[] sm_plts_arr	= plt_grp.get_palettes_arr();
				
				int ind_A = sm_plts_arr[ m_color_A >> 2 ].get_color_inds()[ m_color_A & 0x03 ];
				int ind_B = sm_plts_arr[ m_color_B >> 2 ].get_color_inds()[ m_color_B & 0x03 ];
				
				sm_plts_arr[ m_color_A >> 2 ].get_color_inds()[ m_color_A & 0x03 ] = ind_B;
				sm_plts_arr[ m_color_B >> 2 ].get_color_inds()[ m_color_B & 0x03 ] = ind_A;
				
				for( int j = 0; j < utils.CONST_NUM_SMALL_PALETTES; j++ )
				{
					sm_plts_arr[ j ].update();
				}
#endif
				update();
			}
			else
			{
				MainForm.message_box( "Please, select two colors!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
	}
}
