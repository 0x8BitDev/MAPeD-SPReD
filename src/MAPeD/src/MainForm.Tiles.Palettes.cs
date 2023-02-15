/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 16:35
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace MAPeD
{
	partial class MainForm
	{
		private void CheckBoxPalettePerCHRChecked( object sender, EventArgs e )
		{
#if DEF_NES
			m_tiles_processor.set_block_editor_palette_per_CHR_mode( ( sender as CheckBox ).Checked );
#endif
		}
		
		private void BtnSwapColorsClick( object sender, EventArgs e )
		{
#if !DEF_NES
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				if( m_swap_colors_form.show_window( get_curr_tiles_data() ) == DialogResult.OK )
				{
					BtnUpdateGFXClick( null, null );
					
					palette_group.Instance.update_selected_color();
				}
			}
#endif
		}
		
		private void CBoxPalettesChanged( object sender, EventArgs e )
		{
			tiles_data data = get_curr_tiles_data();
			data.palette_pos = CBoxPalettes.SelectedIndex; 
			
			update_palette_related_data( data );
			
			set_status_msg( "Palette changed" );
		}
		
		private void BtnPltCopyClick( object sender, EventArgs e )
		{
			tiles_data data = get_curr_tiles_data();
			
			if( data != null )
			{
#if DEF_FIXED_LEN_PALETTE16_ARR
				if( m_palette_copy_ind < 0 )
				{
					// copy
					BtnPltCopy.Text = "Paste";
					m_palette_copy_ind = data.palette_pos;
				}
				else
				{
					// paste
					data.palettes_arr[ data.palette_pos ].copy( data.palettes_arr[ m_palette_copy_ind ] );
					
					BtnPltCopy.Text = "Copy";
					m_palette_copy_ind = -1;

					update_palette_related_data( data );
				}
#else
				if( data.palette_copy() )
				{
					palette_group.Instance.set_palette( data );
					
					update_palettes_arr( data, true );
					on_enable_update_gfx_btn( this, null );
				}
				else
				{
					message_box( "The maximum allowed number of palettes - " + utils.CONST_PALETTES_MAX_CNT, "Copy Active Palette", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
#endif
			}
		}
		
		private void BtnPltDeleteClick( object sender, EventArgs e )
		{
			tiles_data data = get_curr_tiles_data();
			
			if( data != null )
			{
				if( message_box( "Are you sure?", "Delete Active Palette", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					if( data.palette_delete() )
					{
						palette_group.Instance.set_palette( data );
						
						update_palettes_arr( data, true );
						on_enable_update_gfx_btn( this, null );
					}
					else
					{
						message_box( "Can't delete the last palette!", "Delete Active Palette", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
		}

		private void update_palette_related_data( tiles_data _data )
		{
			palette_group plt_grp = palette_group.Instance;
			plt_grp.set_palette( _data );
			plt_grp.update_selected_color();
			
#if DEF_PALETTE16_PER_CHR
			m_tiles_processor.block_editor_update_sel_CHR_palette();
#else
			enable_update_gfx_btn( true );
			mark_update_screens_btn( true );
#endif
			m_tiles_processor.update_graphics();
		}
		
		private void update_palettes_arr( tiles_data _data, bool _update_pos )
		{
			CBoxPalettes.Items.Clear();
			
			for( int i = 0; i < _data.palettes_arr.Count; i++ )
			{
				CBoxPalettes.Items.Add( String.Format( "plt#{0:d2}", i ) );
			}

			if( _update_pos )
			{
				CBoxPalettes.SelectedIndex = _data.palette_pos;
			}
		}
		
#if DEF_PALETTE16_PER_CHR
		private void on_update_palette_list_pos( object sender, EventArgs e )
		{
			tiles_data data = get_curr_tiles_data();
			
			if( data != null )
			{
				CBoxPalettes.SelectedIndex = data.palette_pos;
			}
		}
#endif
		private void CBoxPalettesAdjustWidthDropDown( object sender, EventArgs e )
		{
			( sender as ComboBox ).DropDownWidth = 240;
		}
		
		private void CBoxPalettesDrawItem( object sender, DrawItemEventArgs e )
		{
			if( e.Index >= 0 )
			{
				Graphics gfx = e.Graphics;
				e.DrawBackground();
				
				ComboBox cb = sender as ComboBox;
				
				palette16_data plt	= ( ( List< palette16_data > )cb.Tag )[ e.Index ];
				string item_txt		= cb.Items[ e.Index ].ToString();
				SizeF txt_size		= gfx.MeasureString( item_txt, e.Font );
				int plt_offset		= ( int )txt_size.Width + 7;
					
				utils.brush.Color = e.ForeColor;
				gfx.DrawString( item_txt, e.Font, utils.brush, e.Bounds.X, e.Bounds.Y );
				
				int clr;
				int clr_box_side	= 12;
				int clr_box_y_offs	= e.Bounds.Y + ( ( e.Bounds.Height - clr_box_side ) >> 1 );
				int clrs_cnt		= utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS;
				
				for( int i = 0; i < clrs_cnt; i++ )
				{
					clr = palette_group.Instance.main_palette[ plt.subpalettes[ i >> 2 ][ i & 0x03 ] ];
					
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					
					gfx.FillRectangle( utils.brush, plt_offset + ( i * clr_box_side ), clr_box_y_offs, clr_box_side, clr_box_side );
				}
				
				utils.pen.Color = utils.CONST_COLOR_PEN_DEFAULT;
				gfx.DrawRectangle( utils.pen, plt_offset - 1, clr_box_y_offs - 1, ( clr_box_side * clrs_cnt ) + 1, clr_box_side + 1 );
			}
		}
	}
}
