/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 21.07.2022
 * Time: 16:45
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of palette_manager_form.
	/// </summary>
	public partial class palettes_manager_form : Form
	{
		private palettes_array				m_palettes_array	= null;
		private ListBox.ObjectCollection 	m_sprites			= null;
		
		private int	m_copy_slot_ind	= -1;
		
		private Action m_upd_viewport	= null;
		
		public palettes_manager_form( palettes_array _plts_arr, ListBox.ObjectCollection _sprites, Action _upd_viewport )
		{
			m_palettes_array	= _plts_arr;
			m_sprites			= _sprites;
			
			m_upd_viewport		= _upd_viewport;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			// list box init
			ListBoxPalettes.DrawItem	+= new DrawItemEventHandler( PaletteDrawItem_Event );
			
			ListBoxPalettes.Items.Clear();
			
			for( int i = 0; i < utils.CONST_PALETTE16_ARR_LEN; i++ )
			{
				ListBoxPalettes.Items.Add( String.Format( " #{0:d2}", i ) );
			}
		}
		
		private void PaletteDrawItem_Event( object sender, DrawItemEventArgs e )
		{
			if( e.Index >= 0 )
			{
				Graphics gfx = e.Graphics;
				e.DrawBackground();
				
				string item_txt		= ListBoxPalettes.Items[ e.Index ].ToString();
				SizeF txt_size		= gfx.MeasureString( item_txt, e.Font );
				int plt_offset		= ( int )txt_size.Width + 10;
					
				utils.brush.Color = e.ForeColor;
				gfx.DrawString( item_txt, e.Font, utils.brush, e.Bounds.X, e.Bounds.Y );
				
				palette16_data plt	= m_palettes_array.get_palette( e.Index );

				int clr;
				int clr_box_side	= 12;
				int clr_box_y_offs	= e.Bounds.Y + ( ( e.Bounds.Height - clr_box_side ) >> 1 );
				int clrs_cnt		= utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS;
				
				for( int i = 0; i < clrs_cnt; i++ )
				{
					clr = palette_group.Instance.main_palette[ plt.data[ i ] ];
					
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					
					gfx.FillRectangle( utils.brush, plt_offset + ( i * clr_box_side ), clr_box_y_offs, clr_box_side, clr_box_side );
				}

				utils.pen.Color = Color.Black;
				gfx.DrawRectangle( utils.pen, plt_offset - 1, clr_box_y_offs - 1, ( clr_box_side * clrs_cnt ) + 1, clr_box_side + 1 );
				
				if( m_copy_slot_ind == e.Index )
				{
					// mark copied palette
					utils.brush.Color = Color.Cyan;
				}
				else
				if( active_palette( e.Index ) )
				{
					// mark palette in use
					utils.brush.Color = Color.Red;
				}
				else
				{
					return;
				}
				
				gfx.FillRectangle( utils.brush, plt_offset - 9, clr_box_y_offs + 3, 5, 5 );
			}
		}
		
		private bool active_palette( int _ind )
		{
			sprite_data spr;
			
			for( int i = 0; i < m_sprites.Count; i++ )
			{
				spr = m_sprites[ i ] as sprite_data;
				
				foreach( var attr in spr.get_CHR_attr() )
				{
					if( attr.palette_ind == _ind )
					{
						return true;
					}
				}
			}
			
			return false;
		}

		public DialogResult Open()
		{
			ListBoxPalettes.SelectedIndex = m_palettes_array.palette_index;
			
			m_copy_slot_ind		= -1;
			BtnPaste.Enabled	= false;
			
			return ShowDialog();
		}
		
		private void BtnCopyClick_Event(object sender, EventArgs e)
		{
			m_copy_slot_ind		= ListBoxPalettes.SelectedIndex;
			BtnPaste.Enabled	= true;
			
			ListBoxPalettes.Refresh();
		}
		
		private void BtnPasteClick_Event(object sender, EventArgs e)
		{
			if( m_copy_slot_ind >= 0 )
			{
				palette16_data plt_src = m_palettes_array.get_palette( m_copy_slot_ind );
				palette16_data plt_dst = m_palettes_array.get_palette( ListBoxPalettes.SelectedIndex );
				
				Array.Copy( plt_src.data, plt_dst.data, plt_src.data.Length );
	
				m_copy_slot_ind = -1;
				
				ListBoxPalettes.Refresh();
				
				m_palettes_array.update_palette();

				if( m_upd_viewport != null )
				{
					m_upd_viewport();
				}
				
				BtnPaste.Enabled = false;
			}
		}
		
		private void BtnMoveUpClick_Event(object sender, EventArgs e)
		{
			if( ListBoxPalettes.SelectedIndex > 0 )
			{
				int plt_A = ListBoxPalettes.SelectedIndex;
				int plt_B = plt_A - 1;
				
				ListBoxPalettes.SelectedIndex = plt_B;
				
				m_palettes_array.swap_palettes( plt_A, plt_B );
				
				swap_sprites_palettes( plt_A, plt_B );
				
				ListBoxPalettes.Refresh();
				
				m_palettes_array.update_palette();
			}
		}
		
		private void BtnMoveDownClick_Event(object sender, EventArgs e)
		{
			if( ListBoxPalettes.SelectedIndex < ( ListBoxPalettes.Items.Count - 1 ) )
			{
				int plt_A = ListBoxPalettes.SelectedIndex;
				int plt_B = plt_A + 1;
				
				ListBoxPalettes.SelectedIndex = plt_B; 
				
				m_palettes_array.swap_palettes( plt_A, plt_B );
				
				swap_sprites_palettes( plt_A, plt_B );
				
				ListBoxPalettes.Refresh();
				
				m_palettes_array.update_palette();
			}
		}
		
		private void swap_sprites_palettes( int _plt_A, int _plt_B )
		{
			sprite_data spr;
			
			for( int i = 0; i < m_sprites.Count; i++ )
			{
				spr = m_sprites[ i ] as sprite_data;
				
				foreach( var attr in spr.get_CHR_attr() )
				{
					if( attr.palette_ind == _plt_A )
					{
						attr.palette_ind = _plt_B;
					}
					else
					if( attr.palette_ind == _plt_B )
					{
						attr.palette_ind = _plt_A;
					}
				}
			}
		}
	}
}
