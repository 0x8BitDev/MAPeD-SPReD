/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 16:35
 */
using System;
using System.Windows.Forms;

namespace MAPeD
{
	partial class MainForm
	{
		private void CBoxCHRBanksChanged( object sender, EventArgs e )
		{
			ComboBox chr_bank_cbox = sender as ComboBox;
			
			// force update of OLD screens if needed
			update_screens_if_needed();

			m_data_manager.tiles_data_pos 	= chr_bank_cbox.SelectedIndex;
			
			palette_group.Instance.active_palette = 0;
			
			// update palettes
			{
				update_palettes_arr( get_curr_tiles_data(), false );
				
				CBoxPalettes.Tag = get_curr_tiles_data().palettes_arr;
				CBoxPalettes.SelectedIndex = 0;
			}

			patterns_manager_update_data();
			
			update_graphics( false, false, false );
			
			enable_copy_paste_action( false, e_copy_paste_type.All );
			
			clear_active_tile_img();
			
			update_screens_by_bank_id( true, false );
		}

		private void BtnCHRBankNextPageClick( object sender, EventArgs e )
		{
			if( platform_data.get_CHR_bank_pages_cnt() > 1 )
			{
				m_tiles_processor.CHR_bank_next_page();
			}
		}
		
		private void BtnCHRBankPrevPageClick( object sender, EventArgs e )
		{
			if( platform_data.get_CHR_bank_pages_cnt() > 1 )
			{
				m_tiles_processor.CHR_bank_prev_page();
			}
		}
		
		private void BtnCopyCHRBankClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( true, "Copying CHR bank...", false );
				}
				
				if( m_data_manager.tiles_data_copy() == true )
				{
					int prev_tiles_data_pos	= m_data_manager.tiles_data_pos;
					
					tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_cnt - 1 );
					
					CBoxCHRBanks.Items.Add( data );
					CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_cnt - 1;

					// copy screens of the previous CHR bank 
					// to the end of the screen images list
					m_imagelist_manager.copy_screens_to_the_end( m_data_manager.get_tiles_data(), prev_tiles_data_pos );
					
					set_status_msg( "CHR bank copied" );
				}
				else
				{
					set_status_msg( "Failed to copy CHR bank" );
					
					message_box( "Can't copy CHR bank!\nThe maximum allowed number of CHR banks - " + utils.CONST_CHR_BANK_MAX_CNT, "Failed to Copy CHR Bank", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( false, "", false );
				}
			}
		}
		
		private void BtnAddCHRBankClick( object sender, EventArgs e )
		{
			add_CHR_bank();
		}
		
		private bool add_CHR_bank()
		{
			if( platform_data.get_max_blocks_cnt() > 256 )
			{
				progress_bar_show( true, "Initializing CHR bank...", false );
			}
			
			if( m_data_manager.tiles_data_create() )
			{
				enable_main_UI( true );
				update_screen_size();
				
				tiles_data data = m_data_manager.get_tiles_data( m_data_manager.tiles_data_cnt - 1 );

				CBoxCHRBanks.Items.Add( data );
				CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_cnt - 1;
	
				palette_group.Instance.active_palette = 0;
	
				enable_copy_paste_action( false, e_copy_paste_type.All );
				
				set_status_msg( "Added CHR bank" );
			}
			else
			{
				set_status_msg( "Failed to create CHR bank" );
				
				message_box( "Can't create CHR bank!\nThe maximum allowed number of CHR banks - " + utils.CONST_CHR_BANK_MAX_CNT, "Failed to Create CHR Bank", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			if( platform_data.get_max_blocks_cnt() > 256 )
			{
				progress_bar_show( false, "", false );
			}
			
			return true;
		}
		
		private void BtnDeleteCHRBankClick( object sender, EventArgs e )
		{
			if( CBoxCHRBanks.Items.Count > 0 && message_box( "Are you sure?", "Remove CHR Bank", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( true, "Deleting CHR bank...", false );
				}
				
				if( m_data_manager.tiles_data_cnt == 1 )
				{
					reset();

					set_status_msg( "No data" );
				}
				else
				{
					ListViewScreens.BeginUpdate();
					{
						bool res = false;

						int screens_cnt = m_data_manager.scr_data_cnt;
						
						for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
						{
							m_data_manager.remove_screen_from_layouts( CBoxCHRBanks.SelectedIndex, 0 );
							
							res |= m_imagelist_manager.remove_screen( CBoxCHRBanks.SelectedIndex, screen_n );
						}

						m_data_manager.tiles_data_destroy();
						
						if( res )
						{
							update_screens_labels_by_bank_id();
							
							m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
						}

						m_layout_editor.update_dimension_changes();
					}
					ListViewScreens.EndUpdate();

					// update CHR bank labels
					int size;
					{
						CBoxCHRBanks.Items.Clear();

						size = m_data_manager.tiles_data_cnt;

						for( int i = 0; i < size; i++ )
						{
							CBoxCHRBanks.Items.Add( m_data_manager.get_tiles_data( i ) );
						}
					}

					CBoxPalettes.Items.Clear();
					CBoxCHRBanks.SelectedIndex = m_data_manager.tiles_data_pos;

					enable_copy_paste_action( false, e_copy_paste_type.All );

					palette_group.Instance.active_palette = 0;
					
					set_status_msg( "CHR bank removed" );
				}
				
				if( platform_data.get_max_blocks_cnt() > 256 )
				{
					progress_bar_show( false, "", false );
				}
			}
		}
		
		private void BtnCHRVFlipClick( object sender, EventArgs e )
		{
			m_tiles_processor.transform_selected_CHR( utils.e_transform_type.VFlip );
		}
		
		private void BtnCHRHFlipClick( object sender, EventArgs e )
		{
			m_tiles_processor.transform_selected_CHR( utils.e_transform_type.HFlip );
		}
		
		private void BtnCHRRotateClick( object sender, EventArgs e )
		{
			m_tiles_processor.transform_selected_CHR( utils.e_transform_type.Rotate );
		}

		private void CopyCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_tiles_processor.CHR_bank_copy_spr() )
			{
				enable_copy_paste_action( true, e_copy_paste_type.CHRBank );
			}
		}
		
		private void PasteCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_tiles_processor.CHR_bank_paste_spr();
			
//!!!		enable_copy_paste_action( false, e_copy_paste_type.CHRBank );
		}
		
		private void FillWithColorCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_tiles_processor.CHR_bank_fill_with_color_spr() == false )
			{
				message_box( "Please, select an active palette", "Fill With Color", MessageBoxButtons.OK );
			}
		}

		private void InsertLeftCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = get_curr_tiles_data();

				int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
				
				if( sel_ind >= 0 )
				{
					for( int i = platform_data.get_CHR_bank_max_sprites_cnt() - 1; i > sel_ind; i-- )
					{
						data.from_CHR_bank_to_spr8x8( i - 1, utils.tmp_spr8x8_buff );
						data.from_spr8x8_to_CHR_bank( i, utils.tmp_spr8x8_buff );
					}
					
					Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
					data.from_spr8x8_to_CHR_bank( sel_ind, utils.tmp_spr8x8_buff );
					
					data.inc_blocks_CHRs( sel_ind );
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
				else
				{
					message_box( "Please, select a CHR!", "Insert CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void DeleteCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = get_curr_tiles_data();

				int sel_ind = m_tiles_processor.CHR_bank_get_selected_CHR_ind();
				
				if( sel_ind >= 0 )
				{
					if( message_box( "Are you sure?", "Delete CHR", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						for( int i = sel_ind; i < platform_data.get_CHR_bank_max_sprites_cnt() - 1; i++ )
						{
							data.from_CHR_bank_to_spr8x8( i + 1, utils.tmp_spr8x8_buff );
							data.from_spr8x8_to_CHR_bank( i, utils.tmp_spr8x8_buff );
						}
						
						Array.Clear( utils.tmp_spr8x8_buff, 0, utils.tmp_spr8x8_buff.Length );
						data.from_spr8x8_to_CHR_bank( platform_data.get_CHR_bank_max_sprites_cnt() - 1, utils.tmp_spr8x8_buff );
						
						data.dec_blocks_CHRs( sel_ind );
						
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
					}
				}
				else
				{
					message_box( "Please, select a CHR!", "Delete CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void BtnReorderCHRBanksClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				m_reorder_CHR_banks_form.show_window();
				
				if( ( CBoxCHRBanks.SelectedIndex != m_reorder_CHR_banks_form.selected_CHR_bank ) || m_reorder_CHR_banks_form.data_changed )
				{
					progress_bar_show( true, "Updating data...", false );

					CBoxCHRBanks.SelectedIndex = -1;	// this guarantees switching between different banks with the same index
					CBoxCHRBanks.SelectedIndex = m_reorder_CHR_banks_form.selected_CHR_bank;
					
					if( m_reorder_CHR_banks_form.data_changed )
					{
						bool all_banks_flag_state = CheckBoxScreensShowAllBanks.Checked;
						
						CheckBoxScreensShowAllBanks.Checked = true;
						
						update_all_screens( true, true );
						
						CheckBoxScreensShowAllBanks.Checked = all_banks_flag_state;
					}
					
					progress_bar_show( false );
				}
			}
			else
			{
				message_box( "No data!", "Reorder CHR Banks", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
	}
}
