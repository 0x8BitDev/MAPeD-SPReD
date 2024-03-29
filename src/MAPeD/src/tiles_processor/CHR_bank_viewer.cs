﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 13:11
 */
using System;
using System.Windows.Forms;
using System.Drawing;

namespace MAPeD
{
	/// <summary>
	/// Description of CHR_bank_viewer.
	/// </summary>
	/// 
	
	public class CHR_bank_viewer : drawable_base
	{
		public event EventHandler DataChanged;
		public event EventHandler CHRSelected;
		public event EventHandler NeedGFXUpdate;

#if DEF_ZX
		private utils.e_tile_view_type	m_view_type		= utils.e_tile_view_type.UNKNOWN;
		
		public utils.e_tile_view_type view_type
		{
			get { return m_view_type; }
			set { m_view_type = value; update(); }
		}
#endif
		private int m_copy_spr_ind = -1;
		
		private int 	m_sel_ind			= -1;
		private ulong 	m_sel_block_CHRs	= 0;
		
		private Color m_sel_clr	= utils.CONST_COLOR_CHR_BANK_SELECTED_DEFAULT;
		
		public Color selection_color
		{
			get { return m_sel_clr; }
			set { m_sel_clr = value; update(); }
		}
		
		private tiles_data m_data	= null;
		
		private int m_active_page	= 0;
		
		private readonly GroupBox m_CHR_bank_grp_box;
		
		public CHR_bank_viewer( PictureBox _PBoxCHRBank, GroupBox _grp_box ) : base( _PBoxCHRBank )
		{
			m_CHR_bank_grp_box = _grp_box;
			
			m_pix_box.MouseClick 	+= new MouseEventHandler( on_mouse_click );
			
			update();
		}
		
		private void on_mouse_click( object sender, MouseEventArgs e )
		{
			m_sel_ind = ( ( e.X >> 4 ) + 16 * ( e.Y >> 4 ) ) + ( utils.CONST_CHR_BANK_PAGE_SPRITES_CNT * m_active_page );
			
//			update();	- will update in 'CHRSelected -> block_quad_selected'
			
			dispatch_event_CHR_selected();
			
			if( m_data != null )
			{
				MainForm.set_status_msg( String.Format( "CHR Bank: Sprite #{0:X2}", m_sel_ind ) );
				
				update();
			}
		}

		public void subscribe( block_editor _block_editor )
		{
			_block_editor.PixelChanged 		+= new EventHandler( on_pixel_changed );
			_block_editor.BlockQuadSelected	+= new EventHandler( on_block_quad_selected );
		}
		
		public void subscribe( tiles_processor _tiles_proc )
		{
			_tiles_proc.GFXUpdate += new EventHandler( on_update_gfx );
		}
		
		private void on_update_gfx( object sender, EventArgs e )
		{
			update();
		}
		
		private void on_block_quad_selected( object sender, EventArgs e )
		{
			block_editor block_ed = sender as block_editor;
			
			m_sel_ind 			= block_ed.get_selected_quad_CHR_id();
			m_sel_block_CHRs 	= block_ed.get_selected_block_CHRs();
			
			update();
		}
			
		private void on_pixel_changed( object sender, EventArgs e )
		{
			update();
		}
		
		public void subscribe( data_sets_manager _data_mngr )
		{
			_data_mngr.SetTilesData += new EventHandler( on_new_data_set );
		}
		
		private void on_new_data_set( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			tiles_data data = data_mngr.get_tiles_data( data_mngr.tiles_data_pos );

			m_data = data;
			
			m_sel_ind 			= -1;
			m_sel_block_CHRs 	= 0;

			m_active_page = 0;
			
			if( m_data != null )
			{
				if( platform_data.get_CHR_bank_pages_cnt() > 1 )
				{
					update_active_page_text();
				}
			}
			else
			{
				m_CHR_bank_grp_box.Text = "CHR Bank:";
			}

			update();
		}
		
		public override void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_data != null && m_data.CHR_bank != null )
			{
				// draw image...
				{
					bool draw_colored = palette_group.Instance.active_palette >= 0;
					
					Bitmap bmp = null;
					
					if( draw_colored == true )
					{
#if DEF_ZX
						bmp = utils.create_bitmap( m_data.CHR_bank, utils.CONST_CHR_BANK_PAGE_SIDE, utils.CONST_CHR_BANK_PAGE_SIDE, 0, false, palette_group.Instance.active_palette, utils.get_draw_block_palette_by_view_type( view_type, m_data.palettes_arr[ m_data.palette_pos ] ), utils.CONST_CHR_BANK_PAGE_SIZE * m_active_page );
#else
						bmp = utils.create_bitmap( m_data.CHR_bank, utils.CONST_CHR_BANK_PAGE_SIDE, utils.CONST_CHR_BANK_PAGE_SIDE, 0, false, palette_group.Instance.active_palette, m_data.palettes_arr[ m_data.palette_pos ], utils.CONST_CHR_BANK_PAGE_SIZE * m_active_page );
#endif
					}
					else
					{
						bmp = utils.create_bitmap( m_data.CHR_bank, utils.CONST_CHR_BANK_PAGE_SIDE, utils.CONST_CHR_BANK_PAGE_SIDE, 0, false, -1, null, utils.CONST_CHR_BANK_PAGE_SIZE * m_active_page );
					}
					
					m_gfx.DrawImage( bmp, 0, 0, m_pix_box.Width, m_pix_box.Height );
#if DEF_SMS
					if( m_active_page == 1 )
					{
						utils.brush.Color = Color.FromArgb( 0x40ff0000 );
						m_gfx.FillRectangle( utils.brush, 0, ( utils.CONST_CHR_BANK_PAGE_SIDE << 1 ) - ( 16 << 2 ), utils.CONST_CHR_BANK_PAGE_SIDE << 1, 16 << 2 );
					}
#endif
					bmp.Dispose();
				}
				
				// draw grid
				{
					m_pen.Color = utils.CONST_COLOR_CHR_BANK_GRID;
					
					int n_lines 	= m_pix_box.Width >> 4;
					int pos;
					
					for( int i = 0; i < n_lines; i++ )
					{
						pos = i << 4;
						
						m_gfx.DrawLine( m_pen, pos, 0, pos, m_pix_box.Width );
						m_gfx.DrawLine( m_pen, 0, pos, m_pix_box.Height, pos );
					}
				}
				
				draw_border( Color.Black );
				
				if( m_sel_ind >= 0 )
				{
					draw_CHR_border( ( int )( m_sel_block_CHRs & 0x0000ffff ),  			utils.CONST_COLOR_CHR_BANK_SELECTED_BLOCK_CHR_BORDER, utils.CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER );
					draw_CHR_border( ( int )( ( m_sel_block_CHRs >> 16 ) & 0x0000ffff ),  	utils.CONST_COLOR_CHR_BANK_SELECTED_BLOCK_CHR_BORDER, utils.CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER );
					draw_CHR_border( ( int )( ( m_sel_block_CHRs >> 32 ) & 0x0000ffff ),  	utils.CONST_COLOR_CHR_BANK_SELECTED_BLOCK_CHR_BORDER, utils.CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER );
					draw_CHR_border( ( int )( ( m_sel_block_CHRs >> 48 ) & 0x0000ffff ),  	utils.CONST_COLOR_CHR_BANK_SELECTED_BLOCK_CHR_BORDER, utils.CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER );
					
					draw_CHR_border( m_sel_ind, m_sel_clr, utils.CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER );
				}
				
				disable( false );
			}
			else
			{
				disable( true );
			}
			
			invalidate();
		}
		
		private void draw_CHR_border( int _ind, Color _color1, Color _color2 )
		{
			if( ( _ind >> 8 ) == m_active_page )
			{
				_ind &= 0x0000ff;
				
				int x = ( ( _ind % 16 ) << 4 );
				int y = ( ( _ind >> 4 ) << 4 );
				
				m_pen.Color = _color1;
				m_gfx.DrawRectangle( m_pen, x+2, y+2, 13, 13 );
				
				m_pen.Color = _color2;
				m_gfx.DrawRectangle( m_pen, x+1, y+1, 15, 15 );
			}
		}
		
		public void subscribe( palette_group _plt )
		{
#if !DEF_ZX
			_plt.UpdateColor += new EventHandler( on_update_color );
#endif
			
			foreach( var plt in _plt.get_palettes_arr() )
			{
#if DEF_NES || DEF_ZX
				plt.ActivePalette += new EventHandler( on_update_color );
#endif
#if DEF_COLORS_COPY_PASTE
				plt.NeedGFXUpdate += new EventHandler( on_update_color );
#endif
			}
		}
		
		private void on_update_color( object sender, EventArgs e )
		{
			dispatch_event_data_changed();
			
			update();
		}
		
		private void dispatch_event_data_changed()
		{
			if( DataChanged != null )
			{
				DataChanged( this, null );
			}
		}
		
		private void dispatch_event_need_gfx_update()
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}
		
		private void dispatch_event_CHR_selected()
		{
			if( CHRSelected != null )
			{
				CHRSelected( this, null );
			}
		}

		public int get_selected_CHR_ind()
		{
			return m_sel_ind;
		}
		
		public void transform_selected_CHR( utils.e_transform_type _type )
		{
			if( m_sel_ind >= 0 )
			{
				switch( _type )
				{
					case utils.e_transform_type.VFlip: 	{ tiles_data.CHR_bank_vflip( m_data.CHR_bank, m_sel_ind );  	} 	break;
					case utils.e_transform_type.HFlip: 	{ tiles_data.CHR_bank_hflip( m_data.CHR_bank, m_sel_ind );  	} 	break;
					case utils.e_transform_type.Rotate:	{ tiles_data.CHR_bank_rotate_cw( m_data.CHR_bank, m_sel_ind ); 	}	break;
					
				default:
					return;
				}
				
				dispatch_event_data_changed();
				dispatch_event_need_gfx_update();	// just in case, when the Block editor is inactive
				
				update();
			}
		}
		
		public bool copy_spr()
		{
			if( m_data != null )
			{
				if( m_sel_ind >= 0 )
				{
					m_copy_spr_ind = m_sel_ind;
					
					return true;
				}
				else
				{
					MainForm.message_box( "Please, select a CHR!", "Copy CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			
			return false;
		}
		
		public void paste_spr()
		{
			if( m_copy_spr_ind >= 0 && m_data != null )
			{
				if( m_sel_ind >= 0 )
				{
					m_data.from_CHR_bank_to_spr8x8( m_copy_spr_ind, utils.tmp_spr8x8_buff );
					m_data.from_spr8x8_to_CHR_bank( m_sel_ind, utils.tmp_spr8x8_buff );
					
					MainForm.set_status_msg( String.Format( "CHR Bank: Sprite #{0:X2} copied to #{1:X2}", m_copy_spr_ind, m_sel_ind ) );
					
					update();
					
					dispatch_event_need_gfx_update();
#if DEF_ZX
					dispatch_event_CHR_selected();
#else
					dispatch_event_data_changed();
#endif
				}
				else
				{
					MainForm.message_box( "Please, select a CHR!", "Paste CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		public bool fill_with_color_spr()
		{
			palette_group plt = palette_group.Instance;
			
			if( plt.active_palette >= 0 )
			{
				if( m_data != null )
				{
					if( m_sel_ind >= 0 )
					{
#if DEF_NES					
						m_data.fill_CHR_bank_spr8x8_by_color_ind( m_sel_ind, plt.get_palettes_arr()[ plt.active_palette ].get_color_slot() );
#else
						m_data.fill_CHR_bank_spr8x8_by_color_ind( m_sel_ind, plt.active_palette * utils.CONST_NUM_SMALL_PALETTES + plt.get_palettes_arr()[ plt.active_palette ].get_color_slot() );
#endif
					
						MainForm.set_status_msg( String.Format( "CHR Bank: Sprite #{0:X2} is filled with a palette slot index: {1}", m_sel_ind, plt.get_palettes_arr()[ plt.active_palette ].get_color_slot() ) );
					
						update();
					}
					else
					{
						MainForm.message_box( "Please, select a CHR!", "Fill CHR With Color", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
				
				dispatch_event_need_gfx_update();
				dispatch_event_data_changed();
				
				return true;
			}
			
			return false;
		}

		public void next_page()
		{
			if( m_active_page + 1 < platform_data.get_CHR_bank_pages_cnt() )
			{
				++m_active_page;
				
				update_active_page_text();
				
				update();
			}
		}
		
		public void prev_page()
		{
			if( m_active_page - 1 >= 0 )
			{
				--m_active_page;
				
				update_active_page_text();
				
				update();
			}
		}
		
		private void update_active_page_text()
		{
			m_CHR_bank_grp_box.Text = "CHR Bank: " + "[" + ( m_active_page + 1 ) + "/" + platform_data.get_CHR_bank_pages_cnt() + "]";
		}
	}
}