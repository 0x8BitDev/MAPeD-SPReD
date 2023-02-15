/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 17.05.2022
 * Time: 14:59
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of reorder_CHR_banks_form.
	/// </summary>
	public partial class reorder_CHR_banks_form : Form
	{
		private readonly data_sets_manager	m_data_manager;
		private readonly image_preview		m_blocks_preview;
		
		private bool m_data_changed = false;
		
		public bool data_changed
		{
			get	{ return ( m_data_manager.tiles_data_cnt == 1 ) ? false:m_data_changed;	}
			set {}
		}
		
		public int selected_CHR_bank
		{
			get { return ListBoxCHRBanks.SelectedIndex; }
			set {}
		}
		
		public reorder_CHR_banks_form( data_sets_manager _data_manager )
		{
			m_data_manager		= _data_manager;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			m_blocks_preview	= new image_preview( PBoxBlocks2x2, false );
			m_blocks_preview.enable_smoothing_mode( false );
		}
		
		public DialogResult show_window()
		{
			update_UI();
			
			return ShowDialog();
		}
		
		private void update_UI()
		{
			ListBoxCHRBanks.BeginUpdate();
			
			ListBoxCHRBanks.Items.Clear();
			
			for( int i = 0; i < m_data_manager.tiles_data_cnt; i++ )
			{
				ListBoxCHRBanks.Items.Add( i );
			}

			ListBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;
			
			ListBoxCHRBanks.EndUpdate();
			
			update_bank_GFX();
			
			ListBoxCHRBanks.Select();
			ListBoxCHRBanks.Focus();
			
			m_data_changed = false;
		}
		
		private void update_bank_GFX()
		{
			tiles_data data = m_data_manager.get_tiles_data( ListBoxCHRBanks.SelectedIndex );
			
			int img_size		= utils.CONST_BLOCKS_IMG_SIZE;
			int half_img_size	= img_size >> 1;
			int side			= 5;
			int size			= side * side;
			
			Bitmap		bmp = new Bitmap( img_size, img_size );
			Graphics	gfx = Graphics.FromImage( bmp );
			
			gfx.SmoothingMode		= SmoothingMode.HighSpeed;
			gfx.InterpolationMode	= InterpolationMode.NearestNeighbor;
//			gfx.PixelOffsetMode		= PixelOffsetMode.HighQuality;
			
			for( int i = 0; i < size; i++ )
			{
				utils.update_block_gfx( i, data, gfx, half_img_size, half_img_size, 0, 0, 0 );
				
				m_blocks_preview.update( bmp, img_size, img_size, ( i % side ) * img_size, ( i / side ) * img_size, i == ( size - 1 ), i == 0 );
			}
			
			gfx.Dispose();
			bmp.Dispose();
			
			// update info
			{
				LabelCHRs.Text		= data.get_first_free_spr8x8_id( false ).ToString();
				LabelBlocks2x2.Text	= data.get_first_free_block_id( false ).ToString();
				LabelTiles4x4.Text	= data.get_first_free_tile_id( false ).ToString();
				LabelScreens.Text	= data.screen_data_cnt().ToString();
				LabelPalettes.Text	= data.palettes_arr.Count.ToString();
			}
		}

		private void ListBoxCHRBanksChanged( object sender, EventArgs e )
		{
			update_bank_GFX();
		}

		private void swap_layout_screens( int _from_ind, int _to_ind )
		{
			List< tiles_data > data_list	= m_data_manager.get_tiles_data();
		
			int from_offs		= m_data_manager.get_global_screen_ind( _from_ind, 0 );
			int from_scr_size	= data_list[ _from_ind ].screen_data_cnt();

			int to_offs			= m_data_manager.get_global_screen_ind( _to_ind, 0 );
			int to_scr_size		= data_list[ _to_ind ].screen_data_cnt();
			
			m_data_manager.layout_screens_proc( delegate( int _scr_ind )
			{
				if( ( _scr_ind >= to_offs ) && ( _scr_ind < ( to_offs + to_scr_size ) ) )
				{
					return ( to_offs + from_scr_size ) + ( _scr_ind - to_offs );
				}
				
				if( ( _scr_ind >= from_offs ) && ( _scr_ind < ( from_offs + from_scr_size ) ) )
				{
					return ( from_offs - to_scr_size ) + ( _scr_ind - from_offs );
				}
				
				return _scr_ind;
			});
		}
		
		private void BtnUpClick( object sender, EventArgs e )
		{
			if( ListBoxCHRBanks.SelectedIndex > 0 )
			{
				List< tiles_data > data_list	= m_data_manager.get_tiles_data();
				tiles_data data					= data_list[ ListBoxCHRBanks.SelectedIndex ];

				swap_layout_screens( ListBoxCHRBanks.SelectedIndex, ListBoxCHRBanks.SelectedIndex - 1 );

				data_list.RemoveAt( ListBoxCHRBanks.SelectedIndex );
				data_list.Insert( ListBoxCHRBanks.SelectedIndex - 1, data );

				--ListBoxCHRBanks.SelectedIndex;
				
				update_CHR_bank_names();
				
				m_data_changed = true;
			}
		}
		
		private void BtnDownClick( object sender, EventArgs e )
		{
			if( ListBoxCHRBanks.SelectedIndex >= 0 && ListBoxCHRBanks.SelectedIndex < ( ListBoxCHRBanks.Items.Count - 1 ) )
			{
				List< tiles_data > data_list	= m_data_manager.get_tiles_data();
				tiles_data data					= data_list[ ListBoxCHRBanks.SelectedIndex ];
				
				swap_layout_screens( ListBoxCHRBanks.SelectedIndex + 1, ListBoxCHRBanks.SelectedIndex );

				data_list.RemoveAt( ListBoxCHRBanks.SelectedIndex );
				data_list.Insert( ListBoxCHRBanks.SelectedIndex + 1, data );

				++ListBoxCHRBanks.SelectedIndex;
				
				update_CHR_bank_names();
				
				m_data_changed = true;
			}
		}
		
		private void update_CHR_bank_names()
		{
			for( int i = 0; i < m_data_manager.tiles_data_cnt; i++ )
			{
				m_data_manager.get_tiles_data( i ).name = tiles_data.build_name( i );
			}
		}
	}
}
