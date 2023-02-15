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
		private void on_tiles_blocks_form_tile_selected( object sender, EventArgs e )
		{
			select_tile( ( sender as tiles_palette_form ).active_item_id );
		}
		
		private void PanelTilesClick( object sender, EventArgs e )
		{
			select_tile( get_sender_index( sender ) );
		}
		
		private void select_tile( int _id )
		{
			if( _id >= 0 )
			{
				m_tiles_processor.tile_select_event( _id, get_curr_tiles_data() );
			
				update_active_tile_img( _id );
			}
			
			m_tile_list_manager.select( tile_list.e_data_type.Tiles, _id );
		}

		private void TilesLockEditorToolStripMenuItemClick( object sender, EventArgs e )
		{
			TilesLockEditorToolStripMenuItem.Checked = CheckBoxTileEditorLock.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		private void CheckBoxTileEditorLockedChecked( object sender, EventArgs e ) 
		{
			bool checked_state = ( sender as CheckBox ).Checked;
			
			TilesLockEditorToolStripMenuItem.Checked = checked_state;
			
			m_tiles_processor.tile_editor_locked( checked_state );
			
			BtnTileReserveBlocks.Enabled = !checked_state;
		}
		
		private void CopyTileToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( ( m_tile_copy_item_ind = get_context_menu_sender_index( sender ) ) >= 0 )
			{
				enable_copy_paste_action( true, e_copy_paste_type.TilesList );
			}
		}
		
		private void PasteTileToolStripMenuItemClick( object sender, EventArgs e )
		{
			int sel_ind = get_context_menu_sender_index( sender );
			
			if( sel_ind >= 0 && m_tile_copy_item_ind >= 0 )
			{
				tiles_data data = get_curr_tiles_data();
				
				for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
				{
					data.set_tile_block( sel_ind, i, data.get_tile_block( m_tile_copy_item_ind, i ) );
				}

				set_status_msg( String.Format( "Tile List: tile #{0:X2} is copied to #{1:X2}", m_tile_copy_item_ind, sel_ind ) );
				
				mark_update_screens_btn( true );
				
				// optimized update_graphics
				progress_bar_show( true, "Updating graphics...", false );
				{
					m_tile_list_manager.copy_tile( tile_list.e_data_type.Tiles, m_tile_copy_item_ind, sel_ind );
					
					update_active_tile_img( sel_ind );
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
		
		private void ClearTileToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the tile's blocks references will be cleared!", "Clear Selected Tile Refs", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					tiles_data data = get_curr_tiles_data();

					data.clear_tile( sel_ind );
					
					clear_active_tile_img();
	
					set_status_msg( String.Format( "Tile List: tile #{0:X2} references are cleared", sel_ind ) );
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
			}
		}
		
		private void InsertLeftTileToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_pos >= 0 )
			{
				tiles_data data = get_curr_tiles_data();

				int sel_ind = get_context_menu_sender_index( sender );
				
				if( sel_ind >= 0 )
				{
					Array.Copy( data.tiles, sel_ind, data.tiles, sel_ind + 1, data.tiles.Length - sel_ind - 1 );
					
					data.clear_tile( sel_ind );
					data.inc_screen_tiles( ( ushort )sel_ind );
					data.inc_patterns_tiles( ( ushort )sel_ind );
					
					clear_active_tile_img();
					
					mark_update_screens_btn( true );
					
					update_graphics( true, true, true );
				}
			}
		}
		
		private void DeleteTileToolStripMenuItem3Click( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?", "Delete Tile", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.tiles_data_pos >= 0 )
				{
					tiles_data data = get_curr_tiles_data();
	
					int sel_ind = get_context_menu_sender_index( sender );
					
					if( sel_ind >= 0 )
					{
						Array.Copy( data.tiles, sel_ind + 1, data.tiles, sel_ind, data.tiles.Length - sel_ind - 1 );
						
						data.dec_screen_tiles( ( ushort )sel_ind );
						data.dec_patterns_tiles( ( ushort )sel_ind );
						data.clear_tile( data.tiles.Length - 1 );
						
						clear_active_tile_img();
						
						mark_update_screens_btn( true );
						
						update_graphics( true, true, true );
					}
				}
			}
		}
		
		private void ClearAllTileToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( message_box( "Are you sure?\n\nWARNING: ALL the blocks references for all the tiles will be set to zero!", "Clear All Tiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				tiles_data data = get_curr_tiles_data();
				
				data.clear_tiles();
				
				clear_active_tile_img();
				
				set_status_msg( String.Format( "Tile List: all the tile references are cleared" ) );
				
				mark_update_screens_btn( true );
				
				update_graphics( true, true, true );
			}
		}
		
		private void BtnTileReserveBlocksClick( object sender, EventArgs e )
		{
			if( m_tiles_processor.tile_reserve_blocks( m_data_manager ) >= 0 )
			{
				enable_update_gfx_btn( true );
				mark_update_screens_btn( true );
			}
		}
	}
}
