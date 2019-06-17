/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 15.03.2017
 * Time: 16:45
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace SPReD
{
	/// <summary>
	/// Description of CHR_bank_viewer.
	/// </summary>
	///
	
	public delegate void SelectCHR();
	public delegate void UpdatePixel();
	
	public class CHR_bank_viewer : drawable_base
	{
		public event EventHandler SelectCHR;
		public event EventHandler UpdatePixel;
		
		private palette_group	m_palette_group	= null;
		
		private List< CHR8x8_data > m_data_list = null;
		
		private int m_selected_ind	= -1;
		
		private Label m_label 		= null;
		
		private bool m_mode8x16		= false;
		
		private Point m_mouse_down_start;
		
		// local clipboard data
		private CHR8x8_data[] 	m_cb_data		= new CHR8x8_data[ 2 ]{ new CHR8x8_data(), new CHR8x8_data() };
		private int 			m_cb_data_cnt	= -1;
		
		public CHR_bank_viewer( PictureBox _pix_box, Label _label ) : base( _pix_box )
		{
			m_label = _label;
			
			m_pix_box.MouseClick 	+= new System.Windows.Forms.MouseEventHandler(this.CHRBank_MouseClick);
			m_pix_box.MouseDown		+= new System.Windows.Forms.MouseEventHandler(this.CHRBank_MouseDown);
			m_pix_box.MouseMove		+= new System.Windows.Forms.MouseEventHandler(this.CHRBank_MouseMove);
			
			update();
		}
		
		public void reset()
		{
			m_selected_ind = -1;
			
			m_data_list = null;
			
			update();
			
			m_label.Text = "...";
		}
		
		private void CHRBank_MouseDown( object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_selected_ind >= 0 && e.Button == MouseButtons.Right )
			{
				m_mouse_down_start = e.Location;
			}
		}

		private void CHRBank_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_selected_ind >= 0 && e.Button == MouseButtons.Right )
			{
				if( Math.Abs( e.Location.X - m_mouse_down_start.X ) + Math.Abs( e.Location.Y - m_mouse_down_start.Y ) > 3 )
				{
					m_pix_box.DoDragDrop( m_selected_ind.ToString(), DragDropEffects.All );
				}			
			}
		}
		
		private void CHRBank_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_data_list != null )
			{
				int sel_ind = ( e.X >> 4 ) + 16 * ( e.Y >> 4 );
				
				m_selected_ind = ( sel_ind >= m_data_list.Count ) ? -1:sel_ind;
				
				if( m_mode8x16 && m_selected_ind >= 0 )
				{
					m_selected_ind &= unchecked ( ( int )0xfffffffe );
				}
				
				update();
			}
		}
		
		public void init( List< CHR8x8_data > _data_list, int _bank_id, int _link_cnt, int _total_banks )
		{
			m_data_list = _data_list;
			
			// update a selected item
			if( ( m_data_list != null ) && ( m_selected_ind != -1 && m_selected_ind >= m_data_list.Count ) )
			{
				m_selected_ind = m_data_list.Count - 1;
			}
			else
			{
				m_selected_ind = -1;
			}

			if( update() == true )
			{
				if( m_data_list != null )
				{
					m_label.Text = "Tiles: " + _data_list.Count + " / Bytes: " + _data_list.Count * utils.CONST_CHR8x8_NATIVE_SIZE_IN_BYTES + " [BANK: " + _bank_id + "(" + _link_cnt + ") of " + _total_banks + "]";
					
					return;
				}
			}
			
			m_label.Text = "...";
		}

		private bool update()
		{
			bool res = false;
			
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_data_list != null )
			{
				disable( false );
				
				CHR8x8_data 	chr_data;
				Bitmap 			bmp;
			
				m_pen.Color = Color.White;
				
				bool draw_colored = m_palette_group != null && m_palette_group.active_palette >= 0;
				
				int size = m_data_list.Count;
				
				for( int i = 0; i < size; i++ )
				{
					chr_data = m_data_list[ i ];
					
					if( draw_colored == true )
					{
						bmp = utils.create_bitmap8x8( chr_data, 0, false, m_palette_group.active_palette, m_palette_group.get_palettes_arr() );
					}
					else
					{
						bmp = utils.create_bitmap8x8( chr_data, 0, false, -1 );
					}
					
					m_gfx.DrawImage( bmp, ( i<<4 ) % m_pix_box.Width, ( i>>4 ) << 4, 16, 16 );
					
					bmp.Dispose();
				}

				if( m_selected_ind >= 0 )
				{
					draw_selection_border( m_selected_ind );
					
					if( m_mode8x16 )
					{
						draw_selection_border( m_selected_ind + 1 );
					}
				}
				
				if( SelectCHR != null )
				{
					SelectCHR( this, null );
				}
				
				res = true;
			}
			
			if( m_data_list == null || ( m_data_list != null && m_data_list.Count == 0 ) )
			{
				disable( true );
				
				utils.brush.Color = Color.White;
				m_gfx.DrawString( "BUILD Mode:\n\n1) Select a CHR and drag and drop it\nto the LAYOUT window using a RIGHT\nmouse button", utils.fnt10_Arial, utils.brush, 0, 0 );
			}
			else
			{
				// draw grid
				m_pen.Color = Color.FromArgb( 0x78808080 );
				
				int n_lines 	= m_pix_box.Width >> 4;
				int pos;
				
				for( int i = 0; i < n_lines; i++ )
				{
					pos = i << 4;
					
					m_gfx.DrawLine( m_pen, pos, 0, pos, m_pix_box.Width );
					m_gfx.DrawLine( m_pen, 0, pos, m_pix_box.Height, pos );
				}
			}
			
			invalidate();
			
			return res;
		}

		private void draw_selection_border( int _ind )
		{
			if( _ind >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
			{
				return;
			}
			
			int x = ( ( _ind % 16 ) << 4 );
			int y = ( ( _ind >> 4 ) << 4 );
			
			m_pen.Color = Color.White;
			m_gfx.DrawRectangle( m_pen, x+2, y+2, 13, 13 );
			
			if( _ind >= get_data().Count )
			{
				utils.brush.Color = Color.Red;
				m_gfx.FillRectangle( utils.brush, x+1, y+1, 15, 15 );
			}
			else
			{
				m_pen.Color = Color.Black;
				m_gfx.DrawRectangle( m_pen, x+1, y+1, 15, 15 );
			}
		}
		
		public void subscribe_event( palette_group _plt )
		{
			_plt.UpdateColor += new EventHandler( update_color );
			
			m_palette_group = _plt;
						
			palette_small[] plt_arr = _plt.get_palettes_arr();
			
			plt_arr[ 0 ].ActivePalette += new EventHandler( update_color );
			plt_arr[ 1 ].ActivePalette += new EventHandler( update_color );
			plt_arr[ 2 ].ActivePalette += new EventHandler( update_color );
			plt_arr[ 3 ].ActivePalette += new EventHandler( update_color );			
		}
		
		public void subscribe_event( sprite_layout_viewer _sprite_layout_viewer )
		{
			_sprite_layout_viewer.SetSelectedCHR 	+= new EventHandler( select_CHR );
			_sprite_layout_viewer.UpdatePixel 		+= new EventHandler( update_pixel );
		}
		
		private void select_CHR(object sender, EventArgs e)
		{
			sprite_layout_viewer spr_layout = sender as sprite_layout_viewer;
			
			m_selected_ind = spr_layout.get_selected_CHR_ind();
			
			if( m_selected_ind >= 0 )
			{
				update();
			}
		}
		
		private void update_color(object sender, EventArgs e)
		{
			update();
		}
		
		private void update_pixel(object sender, EventArgs e)
		{
			sprite_layout_viewer spr_viewer = sender as sprite_layout_viewer;

			if( m_selected_ind >= 0 )
			{
				if( m_palette_group.active_palette != -1 )
				{
#if DEF_NES
					byte color_slot = ( byte )m_palette_group.get_palettes_arr()[ m_palette_group.active_palette ].color_slot;
#elif DEF_SMS
					byte color_slot = ( byte )( m_palette_group.active_palette * utils.CONST_NUM_SMALL_PALETTES + m_palette_group.get_palettes_arr()[ m_palette_group.active_palette ].color_slot );
#endif						
					if( m_mode8x16 && ( spr_viewer.changed_pix_y > utils.CONST_CHR8x8_SIDE_PIXELS_CNT - 1 ) )
					{
						if( ( m_selected_ind + 1 ) < m_data_list.Count )
						{
							m_data_list[ m_selected_ind + 1 ].get_data()[ spr_viewer.changed_pix_x + ( spr_viewer.changed_pix_y - utils.CONST_CHR8x8_SIDE_PIXELS_CNT ) * utils.CONST_CHR8x8_SIDE_PIXELS_CNT ] = color_slot;
						}
					}
					else
					{
						m_data_list[ m_selected_ind ].get_data()[ spr_viewer.changed_pix_x + spr_viewer.changed_pix_y * utils.CONST_CHR8x8_SIDE_PIXELS_CNT ] = color_slot;
					}
	
					update();
					
					if( UpdatePixel != null )
					{
						UpdatePixel( this, null );
					}
				}
				else
				{
					MainForm.message_box( "Please, select an active palette!", "WARNING!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error );
				}
			}
		}
		
		public int get_selected_CHR_ind()
		{
			return m_selected_ind;
		}
		
		public List< CHR8x8_data > get_data()
		{
			return m_data_list;
		}
		
		public void transform_CHR( CHR8x8_data.ETransform _type )
		{
			if( m_selected_ind >= 0 )
			{
				get_data()[ m_selected_ind ].transform( _type );
				
				update();
				
				if( UpdatePixel != null )
				{
					UpdatePixel( this, null );
				}
			}
		}
		
		public void set_mode8x16( bool _on )
		{
			m_mode8x16 = _on;
			
			update();
		}
		
		public bool CHR_fill_with_color()
		{
			palette_group plt = palette_group.Instance;
			
			if( plt.active_palette >= 0 )
			{
				if( m_selected_ind >= 0 && get_data() != null )
				{
#if DEF_NES					
					byte color_ind = ( byte )plt.get_palettes_arr()[ plt.active_palette ].color_slot;
#elif DEF_SMS
					byte color_ind = ( byte )( plt.active_palette * utils.CONST_NUM_SMALL_PALETTES + plt.get_palettes_arr()[ plt.active_palette ].color_slot );
#endif					
					get_data()[ m_selected_ind ].fill( color_ind );
					
					if( m_mode8x16 && m_selected_ind + 1 < get_data().Count )
					{
						get_data()[ m_selected_ind + 1 ].fill( color_ind );
					}
					
					update();
					
					if( UpdatePixel != null )
					{
						UpdatePixel( this, null );
					}
					
					return true;
				}
			}
			
			return false;
		}
		
		public bool CHR_copy()
		{
			if( m_selected_ind >= 0 )
			{
				m_cb_data_cnt = 0;
				
				Array.Copy( get_data()[ m_selected_ind ].get_data(), m_cb_data[ m_cb_data_cnt++ ].get_data(), utils.CONST_CHR8x8_TOTAL_PIXELS_CNT );
				
				if( m_mode8x16 )
				{
					if( m_mode8x16 && m_selected_ind + 1 < get_data().Count )
					{
						Array.Copy( get_data()[ m_selected_ind + 1 ].get_data(), m_cb_data[ m_cb_data_cnt++ ].get_data(), utils.CONST_CHR8x8_TOTAL_PIXELS_CNT );
					}
				}
				
				return true;
			}
			
			return false;
		}
		
		public bool CHR_paste()
		{
			if( m_selected_ind >= 0 )
			{
				int CHR_cnt = m_mode8x16 ? ( m_cb_data_cnt > 1 && ( m_selected_ind + 1 < get_data().Count ) ? m_cb_data_cnt:1 ):1;
				
				for( int i = 0; i < CHR_cnt; i++ )
				{
					Array.Copy( m_cb_data[ i ].get_data(), get_data()[ m_selected_ind + i ].get_data(), utils.CONST_CHR8x8_TOTAL_PIXELS_CNT );
				}

				update();
				
				if( UpdatePixel != null )
				{
					UpdatePixel( this, null );
				}
				
				return true;
			}
			
			return false;
		}
		
		public void key_event(object sender, KeyEventArgs e)
		{
			if( m_data_list != null )
			{
				bool pressed = false;
				
				if( e.KeyCode == Keys.Up )
				{
					if( m_selected_ind < 0 )
					{
						m_selected_ind = 0;
					}
					else
					{
						m_selected_ind -= 16;
					}
					
					pressed = true;
				}
	
				if( e.KeyCode == Keys.Down )
				{
					if( m_selected_ind < 0 )
					{
						m_selected_ind = 0;
					}
					else
					{
						m_selected_ind += 16;
					}
					
					pressed = true;
				}
	
				if( e.KeyCode == Keys.Left )
				{
					if( m_selected_ind < 0 )
					{
						m_selected_ind = 0;
					}
					else
					{
						m_selected_ind -= 1;
					}
					
					pressed = true;
				}
	
				if( e.KeyCode == Keys.Right )
				{
					if( m_selected_ind < 0 )
					{
						m_selected_ind = 0;
					}
					else
					{
						m_selected_ind += 1;
					}
					
					pressed = true;
				}
				
				if( pressed )
				{
					if( m_selected_ind < 0 )
					{
						m_selected_ind = 0;
					}
					
					if( m_selected_ind >= m_data_list.Count )
					{
						m_selected_ind = m_data_list.Count - 1;
					}
					
					update();
				}
			}
		}
	}
}