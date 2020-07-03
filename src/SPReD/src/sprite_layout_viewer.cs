/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 16.03.2017
 * Time: 12:32
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SPReD
{
	/// <summary>
	/// Description of sprite_layout_viewer.
	/// </summary>
	/// 
	
	public delegate void SetSelectedCHR();
	
	public class sprite_layout_viewer : drawable_base
	{
		public event EventHandler SetSelectedCHR;
		public event EventHandler UpdatePixel;
		
		private EMode	m_mode = EMode.m_unknown;
		
		public EMode mode
		{
			get { return m_mode; }
			set
			{
				m_mode = value;
				update_status_label();
			}
		}
		
		private bool m_mouse_in_area_bounds = false;
		
		private bool m_show_axis	= true;
		private bool m_show_grid	= true;
		
		private bool m_mode8x16		= false;
		
		private palette_group	m_palette_group	= null;
		
		private Label 		m_label 	= null;
		private GroupBox	m_grp_box 	= null;
		
		private sprite_data 			m_spr_data	= null;
		private List< Bitmap > 			m_bmp_list	= null;
		
		private int m_selected_CHR = -1;

		private int m_changed_pixel_x = -1;
		private int m_changed_pixel_y = -1;

		public int changed_pix_x
		{
			get { return m_changed_pixel_x; }
			set {}
		}
		
		public int changed_pix_y
		{
			get { return m_changed_pixel_y; }
			set {}
		}
		
		private bool m_snap	= true;
		
		public bool snap
		{
			get { return m_snap; }
			set { m_snap = value; }
		}
			
		private int m_offset_x = 0;
		private int m_offset_y = 0;
		
		private float	m_scale 	= 2;
		private float	m_CHR_size	= 0;	// current calculated CHR side size depending on a current m_scale
		
		private int m_scr_half_width  = 0;
		private int m_scr_half_height = 0;
		
		private int	m_last_mouse_x	 = 0;
		private int	m_last_mouse_y	 = 0;
		
		private const float	CONST_ZOOM_STEP				= 0.5f;
		
		private const string CONST_MODE_STR_BUILD		= "MODE:BUILD";
		private const string CONST_MODE_STR_EDIT		= "MODE:DRAW";
		
		private string m_mode_str	= null;

		private bool m_update_busy	= false;
			
		public enum EMode
		{
			m_unknown = 0,
			m_build,
			m_draw,
		}
		
		public sprite_layout_viewer( PictureBox _spr_layout, Label _label, GroupBox _spr_layout_grp_box ): base( _spr_layout )
		{
			m_label 	= _label;
			m_grp_box	= _spr_layout_grp_box;
			
			m_bmp_list = new List< Bitmap >( 100 );
			
			m_pix_box.MouseDown 	+= new MouseEventHandler(this.Layout_MouseDown);
			m_pix_box.MouseMove 	+= new MouseEventHandler(this.Layout_MouseMove);
			m_pix_box.MouseUp 		+= new MouseEventHandler(this.Layout_MouseUp);
			m_pix_box.MouseWheel	+= new MouseEventHandler(this.Layout_MouseWheel);
			m_pix_box.MouseEnter 	+= new EventHandler(this.Layout_MouseEnter);			
			m_pix_box.MouseLeave	+= new EventHandler(this.Layout_MouseLeave);
			m_pix_box.MouseClick	+= new MouseEventHandler(this.Layout_MouseClick);
			
			m_scr_half_width  = m_pix_box.Width >> 1;
			m_scr_half_height = m_pix_box.Height >> 1;
			
			m_pix_box.AllowDrop = true;
			m_pix_box.DragEnter += new DragEventHandler(this.Layout_DragEnter);
			m_pix_box.DragDrop	+= new DragEventHandler(this.Layout_DragDrop);

			reset();
			update();
		}
		
		public void reset()
		{
			m_offset_x = m_scr_half_width;
			m_offset_y = m_scr_half_height;

			m_changed_pixel_x = -1;
			m_changed_pixel_y = -1;
			
			m_scale = 2;
			update_status_label();
			
			m_selected_CHR = -1;
			
			clear_background( CONST_BACKGROUND_COLOR );
			
			m_spr_data = null;
			
			m_bmp_list.ForEach( delegate( Bitmap _bmp ) { _bmp.Dispose(); } );
			m_bmp_list.Clear();
			
			m_label.Text = "...";
			
			m_update_busy = false;
		}
		
		private void Layout_MouseDown(object sender, MouseEventArgs e)
		{
			if( m_mouse_in_area_bounds )
			{
				if( m_mode == EMode.m_build )
				{
					if( e.Button == MouseButtons.Left )
					{
						m_last_mouse_x	 = e.X;
						m_last_mouse_y	 = e.Y;
					}
				
					if( m_selected_CHR >= 0 && e.Button == MouseButtons.Right )
					{
						CHR_data_attr chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];
						
						int chr_pos_x = ( int )transform_to_scr_pos( chr_attr.x + m_offset_x + m_spr_data.offset_x, m_scr_half_width );
						int chr_pos_y = ( int )transform_to_scr_pos( chr_attr.y + m_offset_y + m_spr_data.offset_y, m_scr_half_height );
						
						if( e.X > chr_pos_x && e.X < chr_pos_x + m_CHR_size &&
						   e.Y > chr_pos_y && e.Y < chr_pos_y + get_CHR_height() )
						{
							m_last_mouse_x	 = transform_to_img_pos( e.X, m_scr_half_width, m_offset_x );
							m_last_mouse_y	 = transform_to_img_pos( e.Y, m_scr_half_height, m_offset_y );
						}
						else
						{
							m_selected_CHR = -1;
							
							dispatch_event_set_selected_CHR();
							
							update();
						}
					}
				}
			}
		}
		
		private void Layout_MouseMove(object sender, MouseEventArgs e)
		{
			if( m_mouse_in_area_bounds )
			{
				if( m_pix_box.Capture && e.Button == MouseButtons.Left )
				{
					if( m_mode == EMode.m_build )
					{
						m_offset_x += e.X - m_last_mouse_x;
						m_offset_y += e.Y - m_last_mouse_y;

						clamp_offsets();
		
						m_last_mouse_x	 = e.X;
						m_last_mouse_y	 = e.Y;
						
						update();
					}
					else
					if( m_mode == EMode.m_draw && m_spr_data != null )
					{
						check_and_update_pixel( e.X, e.Y );
					}
				}
				
				if( m_mode == EMode.m_build )
				{
					if( m_pix_box.Capture && m_selected_CHR >= 0 && e.Button == MouseButtons.Right )
					{
						CHR_data_attr chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];
						
						int x_pos = transform_to_img_pos( e.X, m_scr_half_width, m_offset_x );
						int y_pos = transform_to_img_pos( e.Y, m_scr_half_height, m_offset_y );
						
						chr_attr.x += x_pos - m_last_mouse_x;
						chr_attr.y += y_pos - m_last_mouse_y;
						
						m_last_mouse_x	 = x_pos;
						m_last_mouse_y	 = y_pos;
						
						update();
					}
				}
			}
		}
		
		private void Layout_MouseUp(object sender, MouseEventArgs e)
		{
			if( m_mouse_in_area_bounds )
			{
				if( m_mode == EMode.m_draw )
				{
					if( m_pix_box.Capture && e.Button == MouseButtons.Left )
					{
						m_changed_pixel_x = -1;
						m_changed_pixel_y = -1;
					}
				}
				else
				if( m_mode == EMode.m_build )
				{
					if( m_pix_box.Capture && m_selected_CHR >= 0 && e.Button == MouseButtons.Right )
					{
						CHR_data_attr chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];
						
						int x_pos = chr_attr.x + m_spr_data.offset_x + ( ( snap == true ) ? ( utils.CONST_CHR8x8_SIDE_PIXELS_CNT >> 1 ):0 );
						int y_pos = chr_attr.y + m_spr_data.offset_y + ( ( snap == true ) ? ( utils.CONST_CHR8x8_SIDE_PIXELS_CNT >> 1 ):0 );
						
						get_snapped_pos( x_pos, y_pos, out x_pos, out y_pos );
						
						chr_attr.x = x_pos - m_spr_data.offset_x;
						chr_attr.y = y_pos - m_spr_data.offset_y;
						
						m_spr_data.update_dimensions();
						
						update();
					}
				}
			}
		}

		private void Layout_MouseEnter(object sender, EventArgs e)
		{
			if( m_spr_data != null )
			{
				m_mouse_in_area_bounds = true;
				m_pix_box.Focus();
			}
			
			update_cursor();
		}

		private void Layout_MouseLeave(object sender, EventArgs e)
		{
			if( m_spr_data != null )
			{
				m_mouse_in_area_bounds = false;
				m_label.Focus();
			}
		}
		
		private void Layout_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_mouse_in_area_bounds )
			{
				if( m_mode == EMode.m_build )
				{
					if( m_spr_data != null && e.Button == MouseButtons.Left )
					{
						calc_selected_CHR( e.X, e.Y );
					}
					
					update();
				}
				else
				if( m_mode == EMode.m_draw )
				{
					check_and_update_pixel( e.X, e.Y );
				}
			}
		}
		
		private void Layout_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_mouse_in_area_bounds )
			{
				m_scale += Math.Sign( e.Delta ) * CONST_ZOOM_STEP;

				update_scale();
			}
		}

		public void zoom_in()
		{
			m_scale += CONST_ZOOM_STEP;
			
			update_scale();
		}

		public void zoom_out()
		{
			m_scale -= CONST_ZOOM_STEP;
			
			update_scale();
		}
		
		private void update_scale()
		{
			m_scale = m_scale < 1.0f ? 1.0f:m_scale;
			m_scale = m_scale > 10.0f ? 10.0f:m_scale;
			
			update_status_label();
			
			// fix offset_x/y
			clamp_offsets();
			
			calc_CHR_size_and_draw_grid( false );
			update();
		}
		
		private void update_status_label()
		{
			m_grp_box.Text = "Layout: [ " + ( ( m_mode == EMode.m_build ) ? CONST_MODE_STR_BUILD:CONST_MODE_STR_EDIT ) + " / x" + m_scale.ToString( "0.0" ) + " ]";
		}

		private void clamp_offsets()
		{
			int ws_half_side_scaled = ( int )( utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE * m_scale );
			
			int width_border_val 	= transform_to_img_pos( ws_half_side_scaled, m_scr_half_width, 0 );
			int height_border_val 	= transform_to_img_pos( ws_half_side_scaled, m_scr_half_height, 0 );
			
			int width_pbox_border_dt	= m_pix_box.Width - width_border_val;
			int height_pbox_border_dt 	= m_pix_box.Height - height_border_val;
			
			m_offset_x = m_offset_x > width_border_val ? width_border_val:m_offset_x;
			m_offset_x = m_offset_x < width_pbox_border_dt ? width_pbox_border_dt:m_offset_x;

			m_offset_y = m_offset_y > height_border_val ? height_border_val:m_offset_y;
			m_offset_y = m_offset_y < height_pbox_border_dt ? height_pbox_border_dt:m_offset_y;
		}
		
		private void check_and_update_pixel( int _X, int _Y )
		{
			calc_selected_CHR( _X, _Y );
			
			if( m_selected_CHR >= 0 )
			{
	           	int img_x_pos = transform_to_img_pos( _X, m_scr_half_width, m_offset_x );
	           	int img_y_pos = transform_to_img_pos( _Y, m_scr_half_height, m_offset_y );
	           	
	           	CHR_data_attr chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];
	           	
	           	int spr_height = m_mode8x16 ? utils.CONST_CHR8x8_SIDE_PIXELS_CNT << 1:utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
	           	
	           	m_changed_pixel_x = ( img_x_pos - ( m_spr_data.offset_x + chr_attr.x ) ) % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
	           	m_changed_pixel_y = ( img_y_pos - ( m_spr_data.offset_y + chr_attr.y ) ) % spr_height;
	           	
	           	if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) > 0 )
	           	{
	           		m_changed_pixel_x = utils.CONST_CHR8x8_SIDE_PIXELS_CNT - m_changed_pixel_x - 1;
	           	}

	           	if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) > 0 )
	           	{
           			m_changed_pixel_y = ( m_mode8x16 ? ( utils.CONST_CHR8x8_SIDE_PIXELS_CNT << 1 ):utils.CONST_CHR8x8_SIDE_PIXELS_CNT ) - m_changed_pixel_y - 1;
	           	}
	           	
				if( UpdatePixel != null )
				{
					UpdatePixel( this, null );
				}
				
				update();
			}
		}
		
		private void calc_selected_CHR( int _X, int _Y )
		{
			m_selected_CHR = -1;
			
			// run through all sprite's CHRs and find the hit
			CHR_data_attr 	chr_attr;
			
			int size = m_spr_data.get_CHR_attr().Count;
			
			float chr_pos_x;
			float chr_pos_y;
			
			for( int i = 0; i < size; i++ )
			{
				chr_attr = m_spr_data.get_CHR_attr()[ i ];
				
				chr_pos_x = transform_to_scr_pos( chr_attr.x + m_offset_x + m_spr_data.offset_x, m_scr_half_width );
				chr_pos_y = transform_to_scr_pos( chr_attr.y + m_offset_y + m_spr_data.offset_y, m_scr_half_height );
				
				if( _X > chr_pos_x && _X < chr_pos_x + m_CHR_size &&
				   _Y > chr_pos_y && _Y < chr_pos_y + get_CHR_height() )
				{
					m_selected_CHR = i;
					
					// highlight the palette
					if( m_palette_group.active_palette != chr_attr.palette_ind )
					{
						int color_slot = ( m_palette_group.active_palette >= 0 ) ? m_palette_group.get_palettes_arr()[ m_palette_group.active_palette ].color_slot:-1;
#if DEF_NES						
						m_palette_group.active_palette = chr_attr.palette_ind;
#endif						
						if( m_palette_group.active_palette >= 0 )
						{
							// set the same color slot that was in the last palette
							m_palette_group.get_palettes_arr()[ m_palette_group.active_palette ].color_slot = color_slot;
						}
					}
					
					break;
				}
			}
			
			dispatch_event_set_selected_CHR();
			
			/* reset palette when clicked outside of CHRs
			if( m_mode == EMode.m_build && m_selected_CHR < 0 )
			{
				m_palette_group.active_palette = -1;
			}
			*/
		}
		
		private void update()
		{
			if( m_update_busy == true )
			{
				// if there was a call from the timer thread, then we exit to avoid an error
				return;
			}
			
			m_update_busy = true;
			
			clear_background( CONST_BACKGROUND_COLOR );
			
			// sprite data drawing
			if( m_spr_data != null )
			{
				disable( false );
				
				draw_chars();
				
				if( m_selected_CHR >= 0 && m_mode == EMode.m_build )
				{
					// draw a frame of the selected CHR
					CHR_data_attr 	chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];
					CHR8x8_data 	chr_data = m_spr_data.get_CHR_data().get_data()[ chr_attr.CHR_ind ];
					
					float chr_scr_pos_x = transform_to_scr_pos( chr_attr.x + m_offset_x + m_spr_data.offset_x, m_scr_half_width );
					float chr_scr_pos_y = transform_to_scr_pos( chr_attr.y + m_offset_y + m_spr_data.offset_y, m_scr_half_height );
					
					float CHR_height = get_CHR_height();
					
					m_pen.Color = Color.Red;
					m_gfx.DrawRectangle( m_pen, chr_scr_pos_x, chr_scr_pos_y, m_CHR_size, CHR_height );
					
					m_pen.Color = Color.White;
					m_gfx.DrawRectangle( m_pen, chr_scr_pos_x-1, chr_scr_pos_y-1, m_CHR_size + 2, CHR_height + 2 );
					
#if DEF_NES					
					m_label.Text = "Pos: " + chr_attr.x + ";" + chr_attr.y + " / Palette: " + ( chr_attr.palette_ind + 1 ) + " / Id: " + chr_attr.CHR_ind + " / Tiles: " + m_spr_data.get_CHR_attr().Count;
#elif DEF_SMS
					m_label.Text = "Pos: " + chr_attr.x + ";" + chr_attr.y + " / Id: " + chr_attr.CHR_ind + " / Tiles: " + m_spr_data.get_CHR_attr().Count;
#endif
				}
				else
				{
					m_label.Text = " Size: " + m_spr_data.size_x + "x" + ( m_spr_data.size_y + ( m_mode8x16 ? utils.CONST_CHR8x8_SIDE_PIXELS_CNT:0 ) ) + " / Offset: " + m_spr_data.offset_x + "x" + m_spr_data.offset_y + " / Tiles: " + m_spr_data.get_CHR_attr().Count;
				}
				
				calc_CHR_size_and_draw_grid( m_show_grid );
					
				// coordinate axes
				if( m_show_axis )
				{
					m_pen.Color = Color.White;
					
					float offs_x = transform_to_scr_pos( m_offset_x, m_scr_half_width );
					float offs_y = transform_to_scr_pos( m_offset_y, m_scr_half_height );
					
					m_gfx.DrawLine( m_pen, offs_x, 0, offs_x, m_pix_box.Height );
					m_gfx.DrawLine( m_pen, 0, offs_y, m_pix_box.Height, offs_y );
				}
				
				if( m_mode_str != null )
				{
					utils.brush.Color = Color.White;
					m_gfx.DrawString( m_mode_str, utils.fnt10_Arial, utils.brush, 0, 0 );
				}
			}
			else
			{
				m_label.Text = "...";
				
				disable( true );
				
				utils.brush.Color = Color.White;
				m_gfx.DrawString( "BUILD Mode:\n\n1) Pan the viewport using a LEFT\nmouse button and scale it using\na mouse wheel\n\n2) Select a CHR and drag and drop it\nusing a RIGHT mouse button\n\nDRAW Mode:\n\n1) Scale the viewport using a mouse\nwheel", utils.fnt10_Arial, utils.brush, 0, 0 );
			}
			
			invalidate();
			
			m_update_busy = false;
		}

		private void calc_CHR_size_and_draw_grid( bool _draw_grid )
		{
			// the grid with the step of 8 pixels
			int step = ( m_scale < 5.0f ) ? 8:1;
			
			int x_max = m_pix_box.Width - m_offset_x;
			int x_min = x_max - m_pix_box.Width;
			
			int y_max = m_pix_box.Height - m_offset_y;
			int y_min = y_max - m_pix_box.Height;
			
			int n8_x = x_min & -step;
			int n8_y = y_min & -step;
			
			int x_line_offset = n8_x - x_min;
			int y_line_offset = n8_y - y_min;
			
			int n_lines_x = ( m_pix_box.Width / step ) + 1;
			int n_lines_y = ( m_pix_box.Height / step ) + 1;
			
			m_pen.Color = Color.FromArgb( 0x78808080 );

			int x_pos = 0;
			int y_pos = 0;
				
			float offs_x = 0;
			float offs_y = 0;
			
			float last_offs_x = 0;
			
			int spr_offs_x = m_offset_x + ( ( m_spr_data != null ) ? m_spr_data.offset_x:0 );
			int spr_offs_y = m_offset_y + ( ( m_spr_data != null ) ? m_spr_data.offset_y:0 );
			
			for( int i = 0; i < n_lines_x; i++ )
			{
				x_pos = x_line_offset + i*step;
				
				offs_x = transform_to_scr_pos( x_pos, m_scr_half_width );

				if( _draw_grid )
				{
					if( step < 8 )
					{
						// highlight lines that are multiples of 8 by more brighter color
						// to highlight CHRs aligned by sprite's offset.
						// draw the main grid by a more darker color
						if( ( ( x_pos - spr_offs_x ) % 8 ) == 0 )
						{
							m_pen.Color = Color.FromArgb( unchecked( (int)0xa0f0f0f0 ) );
						}
						else
						{
							m_pen.Color = Color.FromArgb( 0x78808080 );
						}
					}
					
					m_gfx.DrawLine( m_pen, offs_x, 0, offs_x, m_pix_box.Height );
				}
				else
				{
					m_CHR_size = ( 8 / step ) * ( offs_x - last_offs_x );
					
					if( i > 1 )
					{
						// exit the loop if we need just to calculate a size of CHR
						return;
					}
				}
				
				last_offs_x = offs_x;
			}

			if( _draw_grid )
			{
				for( int i = 0; i < n_lines_y; i++ )
				{
					y_pos = y_line_offset + i*step;
					
					offs_y = transform_to_scr_pos( y_pos, m_scr_half_height ) - 0.5f;
					
					if( step < 8 )
					{
						// highlight lines that are multiples of 8 by more brighter color
						// to highlight CHRs aligned by sprite's offset.
						// draw the main grid by a more darker color
						if( ( ( y_pos - spr_offs_y ) % 8 ) == 0 )
						{
							m_pen.Color = Color.FromArgb( unchecked( (int)0xa0f0f0f0 ) );
						}
						else
						{
							m_pen.Color = Color.FromArgb( 0x78808080 );
						}
					}
					
					m_gfx.DrawLine( m_pen, 0, offs_y, m_pix_box.Height, offs_y );
				}
			}
		}
		
		private float transform_to_scr_pos( int _pos, int _half_scr )
		{
			return ( float )( ( _pos - _half_scr ) * m_scale + ( float )_half_scr );
		}

		private int transform_to_img_pos( int _pos, int _half_scr, int _offset )
		{
			return ( int )Math.Round( ( ( ( _pos - _offset ) ) / m_scale - ( m_scale - 1.0f ) * ( ( ( _offset - _half_scr ) ) / m_scale ) ) - 0.5f );
		}
		
		public void centering()
		{
			if( m_spr_data != null )
			{
				Rectangle rect = m_spr_data.get_rect();
				
				if( rect.IsEmpty == false )
				{
					if( m_mode8x16 )
					{
						rect.Height += utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
					}
					
					int spr_center_x = rect.X + ( rect.Width >> 1 );
					int spr_center_y = rect.Y + ( rect.Height >> 1 );
					
					m_offset_x = m_scr_half_width - spr_center_x;
					m_offset_y = m_scr_half_height - spr_center_y;
					
					update();
				}
			}
		}
		
		private void draw_chars()
		{
			int size = m_bmp_list.Count;
			
			for( int i = 0; i < size; i++ )
			{
				draw_CHR( i );
			}
		}
		
		private void draw_CHR( int _ind )
		{
			float offs_x;
			float offs_y;
			
			CHR_data_attr 	chr_attr;
			
			chr_attr = m_spr_data.get_CHR_attr()[ _ind ];
			
			offs_x = ( float )Math.Round( transform_to_scr_pos( ( chr_attr.x + m_offset_x + m_spr_data.offset_x ), m_scr_half_width ) - 0.5 );
			offs_y = ( float )Math.Round( transform_to_scr_pos( ( chr_attr.y + m_offset_y + m_spr_data.offset_y ), m_scr_half_height ) - 0.5 );
			
			if( m_mode8x16 )
			{
				m_gfx.DrawImage( m_bmp_list[ _ind ], offs_x, offs_y, m_CHR_size, get_CHR_height() );
			}
			else
			{
				m_gfx.DrawImage( m_bmp_list[ _ind ], offs_x, offs_y, m_CHR_size, m_CHR_size );
			}
		}
		
		public void init( sprite_data _spr_data )
		{
			m_spr_data = _spr_data;
			
			// reset the last selection and active palette
			m_selected_CHR = -1;
			
			if( m_bmp_list.Count != 0 )
			{
				m_bmp_list.ForEach( delegate( Bitmap _bmp ) { _bmp.Dispose(); } );
				m_bmp_list.Clear();
			}
			
			// creating list of bitmaps
			if( m_spr_data != null )
			{
				int size = m_spr_data.get_CHR_attr().Count;
				
				for( int i = 0; i < size; i++ )
				{
					m_bmp_list.Add( create_bitmap( m_spr_data.get_CHR_attr()[ i ] ) );
				}
			}

			// calculate a CHR size for the first sprite rendering
			calc_CHR_size_and_draw_grid( false );
			
			update();
		}

		private float get_CHR_height()
		{
			return m_mode8x16 == true ? ( m_CHR_size * 2f ):m_CHR_size;
		}
		
		private Bitmap create_bitmap( CHR_data_attr _chr_attr )
		{
			CHR8x8_data chr_data = m_spr_data.get_CHR_data().get_data()[ _chr_attr.CHR_ind ];
			
			if( m_mode8x16 )
			{
				return utils.create_bitmap8x16( chr_data, ( _chr_attr.CHR_ind + 1 ) >= m_spr_data.get_CHR_data().get_data().Count ? null:m_spr_data.get_CHR_data().get_data()[ _chr_attr.CHR_ind + 1 ], _chr_attr.flip_flag, false, _chr_attr.palette_ind, m_palette_group.get_palettes_arr() );
			}
			
			return utils.create_bitmap8x8( chr_data, _chr_attr.flip_flag, false, _chr_attr.palette_ind, m_palette_group.get_palettes_arr() );
		}
		
		private void update_bitmap( CHR_data_attr _chr_attr, Bitmap _bmp )
		{
			CHR8x8_data chr_data = m_spr_data.get_CHR_data().get_data()[ _chr_attr.CHR_ind ];
			
			if( m_mode8x16 )
			{
				utils.update_bitmap8x16( chr_data, ( _chr_attr.CHR_ind + 1 ) >= m_spr_data.get_CHR_data().get_data().Count ? null:m_spr_data.get_CHR_data().get_data()[ _chr_attr.CHR_ind + 1 ], _bmp, _chr_attr.flip_flag, false, _chr_attr.palette_ind, m_palette_group.get_palettes_arr() );
			}
			else
			{
				utils.update_bitmap8x8( chr_data, _bmp, _chr_attr.flip_flag, false, _chr_attr.palette_ind, m_palette_group.get_palettes_arr() );
			}
		}

		public void subscribe_event( palette_group _plt )
		{
			_plt.UpdateColor += new EventHandler( update_color );

			m_palette_group = _plt;
						
			palette_small[] plt_arr = _plt.get_palettes_arr();
			
			plt_arr[ 0 ].ActivePalette += new EventHandler( update_palette );
			plt_arr[ 1 ].ActivePalette += new EventHandler( update_palette );
			plt_arr[ 2 ].ActivePalette += new EventHandler( update_palette );
			plt_arr[ 3 ].ActivePalette += new EventHandler( update_palette );			
		}
		
		public void subscribe_event( CHR_bank_viewer _chr_bank )
		{
			_chr_bank.SetSelectedCHR += new EventHandler( update_CHR );
			_chr_bank.UpdatePixel += new EventHandler( update_color );
		}

		private void dispatch_event_set_selected_CHR()
		{
			// update selected CHR in the bank
			if( SetSelectedCHR != null )
			{
				SetSelectedCHR( this, null );
			}
		}
	
		private void update_palette(object sender, EventArgs e)
		{
			if( m_selected_CHR >= 0 )
			{
				CHR_data_attr chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];

				chr_attr.palette_ind = ( sender as palette_small ).id;

				update_bitmap( chr_attr, m_bmp_list[ m_selected_CHR ] );
				
				update();
			}
		}

		private void update_CHR(object sender, EventArgs e)
		{
			int sel_CHR_ind = ( sender as CHR_bank_viewer ).get_selected_CHR_ind();
			
			if( sel_CHR_ind < 0 )
			{
				m_selected_CHR = sel_CHR_ind;
			}
			else
			{
				int size = m_spr_data.get_CHR_attr().Count;
				
				m_selected_CHR = -1;
				
				for( int i = 0; i < size; i++ )
				{
					if( m_spr_data.get_CHR_attr()[ i ].CHR_ind == sel_CHR_ind )
					{
						m_selected_CHR = i;
						
						break;
					}
				}
			}
			
			update();
		}
		
		private void update_color(object sender, EventArgs e)
		{
			if( m_spr_data != null )
			{
				int size = m_bmp_list.Count;
				
				for( int i = 0; i < size; i++ )
				{
					update_bitmap( m_spr_data.get_CHR_attr()[ i ], m_bmp_list[ i ] );
				}
				
				update();
			}
		}
		
		public void set_flags( bool _show_axis, bool _show_grid )
		{
			m_show_axis = _show_axis;
			m_show_grid = _show_grid;
			
			update();
		}
		
		public void flip_selected_vert()
		{
			flip_selected_chr( delegate( CHR_data_attr _attr ) { _attr.vflip(); } );
		}
		
		public void flip_selected_horiz()
		{
			flip_selected_chr( delegate( CHR_data_attr _attr ) { _attr.hflip(); } );
		}
		
		private void flip_selected_chr( Action< CHR_data_attr > _act )
		{
			if( m_selected_CHR >= 0 )
			{
				CHR_data_attr chr_attr = m_spr_data.get_CHR_attr()[ m_selected_CHR ];
				
				_act( chr_attr );
				
				update_bitmap( chr_attr, m_bmp_list[ m_selected_CHR ] );
				
				update();
			}
		}
		
		private void Layout_DragEnter(object sender, DragEventArgs e) 
		{
			if( m_mode == EMode.m_build )
			{
		        if( e.Data.GetDataPresent( DataFormats.Text ) )
		        {
		        	e.Effect = DragDropEffects.Move;
		        }
			}
	    }
	
	    private void Layout_DragDrop(object sender, DragEventArgs e) 
	    {
	    	if( m_mode == EMode.m_build )
	    	{
		    	int chr_ind = Int32.Parse( ( string )e.Data.GetData( DataFormats.Text ) );
		    	
		    	if( chr_ind >= 0 && chr_ind < m_spr_data.get_CHR_data().get_data().Count )
		    	{
		           	// get client coordinates in a moment of releasing a button
		           	Point pt = m_pix_box.PointToClient( new Point( e.X, e.Y ) );
		           
		           	int x_pos = transform_to_img_pos( pt.X, m_scr_half_width, m_offset_x );
		           	int y_pos = transform_to_img_pos( pt.Y, m_scr_half_height, m_offset_y );
	
		           	get_snapped_pos( x_pos, y_pos, out x_pos, out y_pos );
		           	
		           	x_pos -= m_spr_data.offset_x;
		           	y_pos -= m_spr_data.offset_y;
		    		
		    		CHR_data_attr chr_attr = new CHR_data_attr( x_pos, y_pos );
		    		chr_attr.CHR_ind = chr_ind;
		    		
		    		chr_attr.palette_ind = m_palette_group.active_palette; 
					
					m_bmp_list.Add( create_bitmap( chr_attr ) );
	
		    		m_spr_data.get_CHR_attr().Add( chr_attr );
		    		
		    		m_spr_data.update_dimensions();
					
		    		m_selected_CHR = m_spr_data.get_CHR_attr().Count - 1;
		    		
		    		update();
		    	}
	    	}
	    }

	    public int get_selected_CHR_ind()
	    {
	    	if( m_selected_CHR >= 0 )
	    	{
	    		return m_spr_data.get_CHR_attr()[ m_selected_CHR ].CHR_ind;
	    	}
	    	
	    	return -1;
	    }
	    
	    private void get_snapped_pos( int _x_pos, int _y_pos, out int _res_x_pos, out int _res_y_pos )
	    {
	    	_res_x_pos = _x_pos;
	    	_res_y_pos = _y_pos;
	    	
           	if( snap == true )
           	{
           		if( m_spr_data != null )
           		{
           			_res_x_pos -= m_spr_data.offset_x % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
					_res_y_pos -= m_spr_data.offset_y % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
           		}
           		
           		int xo = Math.Abs( _res_x_pos ) % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
           		int yo = Math.Abs( _res_y_pos ) % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
           		
           		_res_x_pos -= ( ( _res_x_pos < 0.0 ) ? -xo + utils.CONST_CHR8x8_SIDE_PIXELS_CNT:xo );
           		_res_y_pos -= ( ( _res_y_pos < 0.0 ) ? -yo + utils.CONST_CHR8x8_SIDE_PIXELS_CNT:yo );
           		
           		if( m_spr_data != null )
           		{
           			_res_x_pos += m_spr_data.offset_x % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
					_res_y_pos += m_spr_data.offset_y % utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
           		}
           	}
	    }
	    
	    public void delete_selected_CHR()
	    {
	    	if( m_selected_CHR >= 0 )
	    	{
    			m_spr_data.get_CHR_attr().RemoveAt( m_selected_CHR );
    			m_spr_data.update_dimensions();
	    		
	    		m_bmp_list[ m_selected_CHR ].Dispose();
	    		m_bmp_list.RemoveAt( m_selected_CHR );
	    		
	    		m_selected_CHR = -1;
	    		
	    		update();
	    	}
	    }
	    
		public void set_mode8x16( bool _on )
		{
			m_mode8x16 = _on;
			
			init( m_spr_data );
		}
		
		private void update_cursor()
		{
			if( m_spr_data != null )
			{
				m_pix_box.Cursor = ( m_mode == EMode.m_build ) ? Cursors.Hand:Cursors.Arrow;
			}
			else
			{
				m_pix_box.Cursor = Cursors.Arrow;
			}
		}
	}
}