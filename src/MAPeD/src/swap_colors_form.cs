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

namespace MAPeD
{
	/// <summary>
	/// Description of swap_colors_form.
	/// </summary>
	public partial class swap_colors_form : Form
	{
		protected Graphics 		m_main_gfx		= null;
		protected Graphics 		m_color_A_gfx	= null;
		protected Graphics 		m_color_B_gfx	= null;
		protected Pen			m_pen			= null;
		
		private tiles_data	m_tiles_data = null;
		
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
			
			m_pen = new Pen( utils.CONST_COLOR_PIXBOX_DEFAULT );
			m_pen.EndCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.StartCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.Width		= 1;
		}
		
		private void on_mouse_click( object sender, System.Windows.Forms.MouseEventArgs e )
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
		
		public DialogResult show_window( tiles_data _data )
		{
			m_tiles_data = _data;
			
			m_color_A = m_color_B = -1;
			
			update();
			
			return ShowDialog();
		}
		
		private void draw_sel_border( int _color_ind )
		{
			int x = _color_ind * 16;

			m_pen.Width = 1;
			
			m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_INNER_BORDER;
			m_main_gfx.DrawRectangle( m_pen, x + 2, 2, 13, 13 );
			
			m_pen.Color = utils.CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER;
			m_main_gfx.DrawRectangle( m_pen, x + 1, 1, 15, 15 );
		}
		
		private void update()
		{
			if( m_tiles_data != null )
			{
				int clr;
				
				for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
				{
					clr = palette_group.Instance.main_palette[ m_tiles_data.subpalettes[ i >> 2 ][ i & 0x03 ] ];
						
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					m_main_gfx.FillRectangle( utils.brush, ( i << 4 ) + 1, 1, 16, 16 );
				}
				
				if( ( m_color_A != -1 ) || ( m_color_B != -1 ) )
				{
					m_pen.Color = utils.CONST_COLOR_PALETTE_SWAP_COLOR_ACTIVE_BORDER;
					m_pen.Width = 2;
					
					m_main_gfx.DrawRectangle( m_pen, 1, 1, PixBoxPalette.Width - 2, PixBoxPalette.Height - 2 );
				}
				else
				{
					m_pen.Color = utils.CONST_COLOR_PALETTE_SWAP_COLOR_INACTIVE_BORDER;
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
#if DEF_PALETTE16_PER_CHR
				int pix_n;
				int CHR_id;
				int CHR_n;
				int block_n;
				
				bool[] CHR_flags = new bool[ platform_data.get_CHR_bank_max_sprites_cnt() ];
				Array.Clear( CHR_flags, 0, CHR_flags.Length );

				uint block_data;
				int ff_block_n = m_tiles_data.get_first_free_block_id( false );

				for( block_n = 0; block_n < ff_block_n; block_n++ )
				{
					for( CHR_n = 0; CHR_n < utils.CONST_BLOCK_SIZE; CHR_n++ )
					{
						block_data = m_tiles_data.blocks[ ( block_n << 2 ) + CHR_n ];
						
						CHR_id = tiles_data.get_block_CHR_id( block_data );
						
						if( CHR_flags[ CHR_id ] == false && ( tiles_data.get_block_flags_palette( block_data ) == m_tiles_data.palette_pos ) )
						{
							m_tiles_data.from_CHR_bank_to_spr8x8( CHR_id, utils.tmp_spr8x8_buff, 0 );
							
							for( pix_n = 0; pix_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; pix_n++ )
							{
								if( utils.tmp_spr8x8_buff[ pix_n ] == m_color_A )
								{
									utils.tmp_spr8x8_buff[ pix_n ] = ( byte )m_color_B;
								}
								else
								if( utils.tmp_spr8x8_buff[ pix_n ] == m_color_B )
								{
									utils.tmp_spr8x8_buff[ pix_n ] = ( byte )m_color_A;
								}
							}
							
							m_tiles_data.from_spr8x8_to_CHR_bank( CHR_id, utils.tmp_spr8x8_buff );
							
							CHR_flags[ CHR_id ] = true;
						}
					}
				}
#else					
				for( int i = 0; i < m_tiles_data.CHR_bank.Length; i++ )
				{
					if( m_tiles_data.CHR_bank[ i ] == m_color_A )
					{
						m_tiles_data.CHR_bank[ i ] = ( byte )m_color_B;
					}
					else
					if( m_tiles_data.CHR_bank[ i ] == m_color_B )
					{
						m_tiles_data.CHR_bank[ i ] = ( byte )m_color_A;
					}
				}
#endif
				int ind_A = m_tiles_data.subpalettes[ m_color_A >> 2 ][ m_color_A & 0x03 ];
				int ind_B = m_tiles_data.subpalettes[ m_color_B >> 2 ][ m_color_B & 0x03 ];
				
				m_tiles_data.subpalettes[ m_color_A >> 2 ][ m_color_A & 0x03 ] = ind_B;
				m_tiles_data.subpalettes[ m_color_B >> 2 ][ m_color_B & 0x03 ] = ind_A;
				for( int j = 0; j < utils.CONST_NUM_SMALL_PALETTES; j++ )
				{
					palette_group.Instance.get_palettes_arr()[ j ].update();
				}
				
				update();
			}
			else
			{
				MainForm.message_box( "Please, select two colors!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
	}
}
