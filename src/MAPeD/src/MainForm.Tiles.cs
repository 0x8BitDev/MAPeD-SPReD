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
		private int get_context_menu_sender_index( object sender )
		{
			Control cntrl = ( ( sender as ToolStripDropDownItem ).Owner as ContextMenuStrip ).SourceControl;
			
			return ( cntrl.Tag as tile_list ).cursor_tile_ind();
		}
		
		private int get_sender_index( object sender )
		{
			return ( sender as tile_list ).cursor_tile_ind();
		}
		
		private void enable_copy_paste_action( bool _on, e_copy_paste_type _type )
		{
			if( _type == e_copy_paste_type.CHRBank || _type == e_copy_paste_type.All )
			{
				pasteCHRToolStripMenuItem.Enabled = _on;
			}
			
			if( _type == e_copy_paste_type.BlocksList || _type == e_copy_paste_type.All )
			{
				pasteBlockCloneToolStripMenuItem.Enabled 	= _on; 
				pasteBlockRefsToolStripMenuItem.Enabled 	= _on;
			}
			
			if( _type == e_copy_paste_type.TilesList || _type == e_copy_paste_type.All )
			{
				pasteTileToolStripMenuItem.Enabled = _on; 
			}
		}
		
		private void CBoxTileViewTypeChanged( object sender, EventArgs e )
		{
			m_view_type = ( utils.e_tile_view_type )CBoxTileViewType.SelectedIndex;
			
			mark_update_screens_btn( true );
#if DEF_ZX
			m_tiles_processor.set_view_type( m_view_type );
#endif
			update_graphics( false, true, true );
			
			clear_active_tile_img();

			set_status_msg( "View type changed" );
		}
		
		private void BtnOptimizationClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_cnt == 0 )
			{
				message_box( "No data!", "Data Optimization", MessageBoxButtons.OK );
			}
			else
			{
				if( m_optimization_form.ShowDialog() == DialogResult.OK )
				{
					ListViewScreens.BeginUpdate();
					{
						if( m_optimization_form.need_update_screen_list )
						{
							progress_bar_show( true, "Updating screens...", false );
							
							// update a global screen list
							m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, -1 );
							
							// update a local screen list if needed
							if( !CheckBoxLayoutEditorAllBanks.Checked )
							{
								m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, CBoxCHRBanks.SelectedIndex );
							}
	
							mark_update_screens_btn( false );
							
							if( m_layout_editor.mode == layout_editor_base.e_mode.Screens )
							{
								m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
							}
							
							m_layout_editor.update();
							
							progress_bar_show( false );
							
							m_optimization_form.need_update_screen_list = false;
						}
						
						// need to be reset to avoid incorrect tiles array filling
						// when drawing by blocks 2x2 when the 'Tiles (4x4)' mode is active
						clear_active_tile_img();
					}
					ListViewScreens.EndUpdate();
				}
			}
		}
		
		private void on_update_gfx_after_optimization( object sender, EventArgs e )
		{
			mark_update_screens_btn( true );
			
			update_graphics( true, true, true );
		}
	}
}
