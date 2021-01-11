/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
			PixBoxPalette.MouseClick	+= new MouseEventHandler( swap_colors_MouseClock );
			
			PixBoxPalette.Image	= new Bitmap( PixBoxPalette.Width, PixBoxPalette.Height, PixelFormat.Format32bppArgb );
			
			m_main_gfx		= Graphics.FromImage( PixBoxPalette.Image );
			
			m_pen = new Pen( utils.CONST_COLOR_PIXBOX_DEFAULT );
			m_pen.EndCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.StartCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.Width		= 1;
		}
		
		private void swap_colors_MouseClock(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int sel_ind = e.X / 16;
			
			if( CheckBoxColorA.Checked )
			{
				if( sel_ind != m_color_B )
				{
					m_color_A = sel_ind;
					
					CheckBoxColorA.ForeColor = get_color( sel_ind );
					CheckBoxColorA.Checked = false;
				}
			}
			else
			if( CheckBoxColorB.Checked )
			{
				if( sel_ind != m_color_A )
				{
					m_color_B = sel_ind;
					
					CheckBoxColorB.ForeColor = get_color( sel_ind );
					CheckBoxColorB.Checked = false;
				}
			}
			
			update();
		}
		
		public DialogResult ShowDialog( tiles_data _data )
		{
			m_tiles_data = _data;
			
			m_color_A = m_color_B = -1;
			CheckBoxColorA.Checked = CheckBoxColorB.Checked = false;

			CheckBoxColorA.ForeColor = CheckBoxColorB.ForeColor = utils.CONST_COLOR_PALETTE_SWAP_COLOR_TEXT_DEFAULT;
			
			update();
			
			return ShowDialog();
		}
		
		void CheckBoxColorAChanged_Event(object sender, EventArgs e)
		{
			if( CheckBoxColorA.Checked )
			{
				CheckBoxColorB.Checked = false;
			}
			
			update();
		}
		
		void CheckBoxColorBChanged_Event(object sender, EventArgs e)
		{
			if( CheckBoxColorB.Checked )
			{
				CheckBoxColorA.Checked = false;
			}
			
			update();
		}
		
		private Color get_color( int _ind )
		{
			return Color.FromArgb( palette_group.Instance.main_palette[ m_tiles_data.subpalettes[ _ind >> 2 ][ _ind & 0x03 ] ] );
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
				
				if( CheckBoxColorA.Checked || CheckBoxColorB.Checked )
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
		
		void BtnOkClick_Event(object sender, EventArgs e)
		{
			if( m_color_A >= 0 && m_color_B >= 0 )
			{
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

				int ind_A = m_tiles_data.subpalettes[ m_color_A >> 2 ][ m_color_A & 0x03 ];
				int ind_B = m_tiles_data.subpalettes[ m_color_B >> 2 ][ m_color_B & 0x03 ];
				
				m_tiles_data.subpalettes[ m_color_A >> 2 ][ m_color_A & 0x03 ] = ind_B;
				m_tiles_data.subpalettes[ m_color_B >> 2 ][ m_color_B & 0x03 ] = ind_A;
				
				for( int j = 0; j < utils.CONST_NUM_SMALL_PALETTES; j++ )
				{
					palette_group.Instance.get_palettes_arr()[ j ].update();
				}
			}
			else
			{
				MainForm.message_box( "Please, select both Color A and Color B!", "Colors Swapping Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
	}
}
