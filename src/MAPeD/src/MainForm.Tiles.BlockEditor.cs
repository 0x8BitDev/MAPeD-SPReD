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
		private void on_tiles_blocks_form_block_selected( object sender, EventArgs e )
		{
			select_block( ( sender as tiles_palette_form ).active_item_id, true, true );
		}
		
		private void PanelBlocksClick( object sender, EventArgs e )
		{
			select_block( get_sender_index( sender ), true, true );
		}
		
		private void select_block( int _id, bool _send_event, bool _update_active_tile )
		{
			if( _send_event && _id >= 0 )
			{
				m_tiles_processor.block_select_event( _id, get_curr_tiles_data() );
			}

			// update an object index
			CBoxBlockObjId.Tag = int.MaxValue;	// don't enable 'Update GFX' button
			CBoxBlockObjId.SelectedIndex = m_tiles_processor.get_block_flags_obj_id();
			CBoxBlockObjId.Tag = null;
			
			if( _update_active_tile )
			{
				update_active_block_img( _id );
			}
			
			m_tile_list_manager.select( tile_list.e_data_type.Blocks, _id );
		}
		
		private void update_selected_block()
		{
			// update selected block image
			int sel_block_id = m_tiles_processor.get_selected_block();
			
			if( sel_block_id >= 0 )
			{
				m_imagelist_manager.update_block( sel_block_id, m_view_type, get_curr_tiles_data(), PropertyPerBlockToolStripMenuItem.Checked, null, null, m_data_manager.screen_data_type );
				m_tile_list_manager.update_tile( tile_list.e_data_type.Blocks, sel_block_id );
				
				update_active_block_img( sel_block_id );
				patterns_manager_update_preview();
			}
		}
		
		private void on_block_quad_selected( object sender, EventArgs e )
		{
			// show property
			select_block( m_tiles_processor.get_selected_block(), false, true );
		}
		
		private void CBoxBlockObjIdChanged( object sender, EventArgs e )
		{
			m_tiles_processor.set_block_flags_obj_id( CBoxBlockObjId.SelectedIndex, PropertyPerBlockToolStripMenuItem.Checked );
			
			if( CBoxBlockObjId.Tag == null && m_view_type == utils.e_tile_view_type.ObjectId )
			{
				enable_update_gfx_btn( true );
			}
		}
		
		private void BtnBlockVFlipClick( object sender, EventArgs e )
		{
			m_tiles_processor.transform_block( utils.e_transform_type.VFlip );
		}
		
		private void BtnBlockHFlipClick( object sender, EventArgs e )
		{
			m_tiles_processor.transform_block( utils.e_transform_type.HFlip );
		}
		
		private void BtnBlockRotateClick( object sender, EventArgs e )
		{
			m_tiles_processor.transform_block( utils.e_transform_type.Rotate );
		}
		
		private void BtnSwapInkPaperClick( object sender, EventArgs e )
		{
#if DEF_ZX
			m_tiles_processor.zx_swap_ink_paper( true );
#endif
		}
		
		private void BtnInvInkClick( object sender, EventArgs e )
		{
#if DEF_ZX
			m_tiles_processor.zx_swap_ink_paper( false );
#endif		
		}
		
		private void BtnBlockReserveCHRsClick( object sender, EventArgs e )
		{
			if( m_tiles_processor.block_reserve_CHRs( m_tiles_processor.get_selected_block(), m_data_manager ) > 0 )
			{
				enable_update_gfx_btn( true );
				mark_update_screens_btn( true );
			}
		}
		
		private void PropertyPerBlockToolStripMenuItemClick( object sender, EventArgs e )
		{
			property_id_per_block( true );
			
			set_status_msg( "Property Id per BLOCK: on" );
		}
		
		private void PropertyPerCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			property_id_per_block( false );
			
			set_status_msg( "Property Id per CHR: on" );
		}
		
		private void property_id_per_block( bool _on )
		{
			PropIdPerBlockToolStripMenuItem.Checked = PropertyPerBlockToolStripMenuItem.Checked = _on;
			PropIdPerCHRToolStripMenuItem.Checked	= PropertyPerCHRToolStripMenuItem.Checked	= !_on;
		
			if( m_view_type == utils.e_tile_view_type.ObjectId )
			{
				mark_update_screens_btn( true );
				
				update_graphics( false, true, true );
			}
			
			select_block( m_tiles_processor.get_selected_block(), true, false );
		}
		
		private void SelectCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			block_editor_draw_mode( false );
		}
		
		private void DrawToolStripMenuItemClick( object sender, EventArgs e )
		{
			block_editor_draw_mode( true );
		}

		private void block_editor_draw_mode( bool _on )
		{
			SelectCHRToolStripMenuItem.Checked	= !_on;
			DrawToolStripMenuItem.Checked 		= _on;
			
			BtnEditModeDraw.Enabled 		= !_on;
			BtnEditModeSelectCHR.Enabled 	= _on;
			
			BlockEditorModeDrawToolStripMenuItem.Checked	= _on;
			BlockEditorModeSelectToolStripMenuItem.Checked	= !_on;
			
			m_tiles_processor.set_block_editor_mode( _on ?  block_editor.e_mode.Draw:block_editor.e_mode.CHRSelect );
		}
		
		private void CopyBlockToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( ( m_block_copy_item_ind = get_context_menu_sender_index( sender ) ) >= 0 )
			{
				enable_copy_paste_action( true, e_copy_paste_type.BlocksList );
			}
		}
		
		private void PasteBlockCloneToolStripMenuItemClick( object sender, EventArgs e )
		{
			paste_block( true, get_context_menu_sender_index( sender ) );
		}
		
		private void PasteBlockRefsToolStripMenuItemClickEvent( object sender, EventArgs e )
		{
			paste_block( false, get_context_menu_sender_index( sender ) );
		}

		private void paste_block( bool _paste_clone, int _sel_ind )
		{
			if( _sel_ind >= 0 && m_block_copy_item_ind >= 0 )
			{
				tiles_data data = get_curr_tiles_data();
				
				uint 	block_val;
				int 	chr_ind;
				int 	free_chr_ind 	= -1;
				
				if( _paste_clone )
				{
					free_chr_ind = data.get_first_free_spr8x8_id( false );
	
					if( free_chr_ind + utils.CONST_BLOCK_SIZE > platform_data.get_CHR_bank_max_sprites_cnt() )
					{
						message_box( "There is no free space in the CHR bank!", "Paste Block", MessageBoxButtons.OK );
						return;
					}
				}
				
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					block_val = data.blocks[ ( m_block_copy_item_ind << 2 ) + i ];
					
					if( _paste_clone )
					{
						chr_ind = tiles_data.get_block_CHR_id( block_val );
						
						data.from_CHR_bank_to_spr8x8( chr_ind, utils.tmp_spr8x8_buff );
						data.from_spr8x8_to_CHR_bank( free_chr_ind + i, utils.tmp_spr8x8_buff );
						
						block_val = tiles_data.set_block_CHR_id( free_chr_ind + i, block_val );
					}
					data.blocks[ ( _sel_ind << 2 ) + i ] = block_val;
				}

				if( _paste_clone )
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} cloned to block #{1:X2}", m_block_copy_item_ind, _sel_ind ) );
				}
				else
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} references are copied to block #{1:X2}", m_block_copy_item_ind, _sel_ind ) );
				}

				mark_update_screens_btn( true );
				
				// optimized update_graphics
				progress_bar_show( true, "Updating graphics...", false );
				{
					m_tile_list_manager.copy_tile( tile_list.e_data_type.Blocks, m_block_copy_item_ind, _sel_ind );
					
					if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
					{
						m_imagelist_manager.update_tiles( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );
						m_tile_list_manager.update_tiles( tile_list.e_data_type.Tiles );
					}
					
					update_active_block_img( _sel_ind );
					patterns_manager_update_preview();
					m_tiles_processor.update_graphics();
					
					if( CheckBoxScreensAutoUpdate.Checked )
					{
						update_screens( true );
					}
				}
				progress_bar_show( false );
			}
		}

		private void ClearCHRsBlockToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the block's CHRs will be cleared!", "Clear Selected Block CHRs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				clear_block( true, get_context_menu_sender_index( sender ) );
			}
		}
		
		private void ClearRefsBlockToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the block's CHR indices will be set to zero!", "Clear Selected Block Refs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				clear_block( false, get_context_menu_sender_index( sender ) );
			}
		}
		
		private void clear_block( bool _clear_CHRs, int _sel_ind )
		{
			if( _sel_ind >= 0 )
			{
				tiles_data data = get_curr_tiles_data();
				
				uint 	block_val;
				int 	chr_ind;
				
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					block_val = data.blocks[ ( _sel_ind << 2 ) + i ];
					{
						chr_ind = tiles_data.get_block_CHR_id( block_val );
						
						if( _clear_CHRs )
						{
							data.fill_CHR_bank_spr8x8_by_color_ind( chr_ind, 0 );
						}
						
						block_val = tiles_data.set_block_CHR_id( 0, block_val );
					}
					data.blocks[ ( _sel_ind << 2 ) + i ] = block_val;
				}

				if( _clear_CHRs )
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} is cleared", _sel_ind ) );
				}
				else
				{
					set_status_msg( String.Format( "Block List: block #{0:X2} references are cleared", _sel_ind ) );
				}
				
				clear_active_tile_img();
				
				mark_update_screens_btn( true );
				
				update_graphics( true, true, true );
			}
		}
		
		private void ClearPropertiesToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				if( message_box( "Are you sure?\n\nWARNING: ALL the blocks properties will be set to zero!", "Clear Blocks Properties", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					tiles_data data = get_curr_tiles_data();
					
					int ff_block = data.get_first_free_block_id( false );
					
					for( int block_n = 0; block_n < ff_block; block_n++ )
					{
						data.set_block_flags_obj_id( block_n, -1, 0, true );
					}
					
					if( CBoxTileViewType.SelectedIndex == ( int )utils.e_tile_view_type.ObjectId )
					{
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
					}
				}
			}
		}
		
		private void InsertLeftBlockToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = get_curr_tiles_data();

				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					int block_ind = sel_ind * utils.CONST_BLOCK_SIZE;
					
					Array.Copy( data.blocks, block_ind, data.blocks, block_ind + utils.CONST_BLOCK_SIZE, data.blocks.Length - block_ind - utils.CONST_BLOCK_SIZE );
					
					data.clear_block( sel_ind );
					
					if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
					{
						data.inc_tiles_blocks( ( ushort )sel_ind );
					}
					else
					{
						data.inc_screen_blocks( ( ushort )sel_ind );
						data.inc_patterns_tiles( ( ushort )sel_ind );
					}
					
					clear_active_tile_img();
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
			}
		}
		
		private void DeleteBlockToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?", "Delete Block", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = get_curr_tiles_data();
	
					int sel_ind = get_context_menu_sender_index( sender );
					
					if( sel_ind >= 0 )
					{
						int block_ind = sel_ind * utils.CONST_BLOCK_SIZE;
						
						Array.Copy( data.blocks, block_ind + utils.CONST_BLOCK_SIZE, data.blocks, block_ind, data.blocks.Length - block_ind - utils.CONST_BLOCK_SIZE );
						
						if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
						{
							data.dec_tiles_blocks( ( ushort )sel_ind );
						}
						else
						{
							data.dec_screen_blocks( ( ushort )sel_ind );
							data.dec_patterns_tiles( ( ushort )sel_ind );
						}
						data.clear_block( data.tiles.Length - 1 );
						
						clear_active_tile_img();
						
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
					}
				}
			}
		}
		
		private void ShiftColorsToolStripMenuItemClick( object sender, EventArgs e )
		{
#if DEF_NES
			m_tiles_processor.block_shift_colors( shiftTransparencyToolStripMenuItem.Checked );
#endif
		}
		
		private void on_enable_update_gfx_btn( object sender, EventArgs e )
		{
			enable_update_gfx_btn( true );
			
			mark_update_screens_btn( true );
		}
		
		private void enable_update_gfx_btn( bool _on )
		{
			if( _on && ( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Blocks2x2 ) )
			{
				update_selected_block();
			}
			
			BtnUpdateGFX.Enabled = _on;
		
			BtnUpdateGFX.UseVisualStyleBackColor = !_on;
		}
		
		private void BtnUpdateGFXClick( object sender, EventArgs e )
		{
			update_graphics( false, true, true );
			
			clear_active_tile_img();
			
			set_status_msg( "Graphics updated" );
		}
		
		private void update_graphics( bool _update_tile_processor_gfx, bool _update_screens, bool _show_status_wnd )
		{
			if( _show_status_wnd )
			{
				progress_bar_show( true, "Updating graphics...", false );
			}
			
			tiles_data data = get_curr_tiles_data();
			
			m_imagelist_manager.update_blocks( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );
			m_imagelist_manager.update_tiles( m_view_type, data, PropertyPerBlockToolStripMenuItem.Checked, m_data_manager.screen_data_type );	// called after update_blocks, because it uses updated blocks gfx to speed up drawing
			
			m_tile_list_manager.update_all();

			if( CheckBoxScreensAutoUpdate.Checked && _update_screens )
			{
				update_screens( true );
			}
			else
			{
				m_layout_editor.update();
			}
			
			if( _update_tile_processor_gfx )
			{
				m_tiles_processor.update_graphics();
			}
			
			m_tiles_palette_form.update();
			patterns_manager_update_preview();
			
			enable_update_gfx_btn( false );
			
			if( _show_status_wnd )
			{
				progress_bar_show( false );
			}
		}
	}
}
