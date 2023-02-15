/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 15:46
 */
using System;
using System.Windows.Forms;

namespace SPReD
{
	partial class MainForm
	{
		private void update_selected_color()
		{
			palette_group plt_grp = palette_group.Instance;
			
			plt_grp.get_palettes_arr()[ plt_grp.active_palette ].color_slot = plt_grp.get_palettes_arr()[ plt_grp.active_palette ].color_slot;
		}

		private void CBoxPalettesChanged( object sender, EventArgs e )
		{
#if DEF_FIXED_LEN_PALETTE16_ARR
			m_palettes_arr.update_palette();
			m_sprites_proc.apply_palette_to_selected_CHR( m_palettes_arr.palette_index );
			update_selected_color();
#endif
		}
		
		private void CBoxPalettesAdjustWidthDropDown( object sender, EventArgs e )
		{
#if DEF_FIXED_LEN_PALETTE16_ARR
			( sender as ComboBox ).DropDownWidth = 230;
#endif
		}
		
#if DEF_FIXED_LEN_PALETTE16_ARR
		public void SetCHRPalette( object sender, EventArgs e )
		{
			m_palettes_arr.palette_index = ( sender as sprite_layout_viewer ).get_selected_CHR_attr().palette_ind;
		}
		
		public void ApplyPaletteToCHR( object sender, EventArgs e )
		{
			m_sprites_proc.apply_palette_to_selected_CHR( m_palettes_arr.palette_index );
		}
#endif
		private void BtnSwapColorsClick( object sender, EventArgs e )
		{
#if DEF_SMS || DEF_PCE
			if( SpriteList.Items.Count > 0 )
			{
				m_swap_colors_form.show_window( SpriteList.Items );
				update_selected_sprite();
			}
#endif
		}

		private void PalettesManagerClick( object sender, EventArgs e )
		{
#if DEF_PCE
			if( SpriteList.Items.Count > 0 )
			{
				m_palettes_mngr_form.show_window();
			}
#endif
		}
	}
}
