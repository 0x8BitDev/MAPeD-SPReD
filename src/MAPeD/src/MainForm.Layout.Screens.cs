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
		private void ScreensAutoUpdateToolStripMenuItemClick( object sender, EventArgs e )
		{
			bool on = ( sender as ToolStripMenuItem ).Checked;
			ScreensAutoUpdateToolStripMenuItem.Checked = CheckBoxScreensAutoUpdate.Checked = !on;
			
			set_status_msg( "Screens auto update " + ( !on ? "enabled":"disabled" ) );
		}
		
		private void CheckBoxScreensAutoUpdateChanged( object sender, EventArgs e )
		{
			CheckBox obj = sender as CheckBox;
			
			ScreensAutoUpdateToolStripMenuItem.Checked = obj.Checked;
			
			BtnUpdateScreens.Enabled = !obj.Checked;
			
			set_status_msg( "Screens auto update " + ( obj.Checked ? "enabled":"disabled" ) );
		}
		
		private void ScreensShowAllBanksToolStripMenuItemClick( object sender, EventArgs e )
		{
			CheckBoxScreensShowAllBanks.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		private void CheckBoxScreensShowAllBanksChanged( object sender, EventArgs e )
		{
			ScreensShowAllBanksToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			update_screens_by_bank_id( true, need_update_screens() );
			
			m_layout_editor.update();
		}
		
		private void ListViewScreensClick( object sender, EventArgs e )
		{
			ListView lv = sender as ListView;
			
			if( lv.SelectedItems.Count > 0 )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, lv.SelectedItems[ 0 ].ImageIndex );
			
				set_status_msg( "Layout Editor: screen - " + lv.SelectedItems[ 0 ].Text );
			}
			else
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_SCR_ACTIVE, layout_data.CONST_EMPTY_CELL_ID );
			
				set_status_msg( "" );
			}
			
			m_layout_editor.update();
		}
		
		private void on_reset_selected_screen( object sender, EventArgs e )
		{
			ListViewScreens.SelectedItems.Clear();
		}
		
		private void mark_update_screens_btn( bool _on )
		{
			BtnUpdateScreens.UseVisualStyleBackColor = !_on;
		}
		
		private bool need_update_screens()
		{
			return !BtnUpdateScreens.UseVisualStyleBackColor;
		}

		private bool update_screens_if_needed()
		{
			if( need_update_screens() )
			{
				progress_bar_show( true, "Updating the bank:" + m_data_manager.tiles_data_pos + " screens...", false );
				{
					m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, true, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, m_data_manager.tiles_data_pos );
					
					mark_update_screens_btn( false );
				}
				progress_bar_show( false );
				
				return true;
			}
			
			return false;
		}

		private void BtnUpdateScreensClick( object sender, EventArgs e )
		{
			progress_bar_show( true, "Updating screens...", false );
			{
				update_screens( true );
			}
			progress_bar_show( false );
		}
		
		private void update_screens( bool _unmark_upd_scr_btn, bool _show_status_msg = true )
		{
			// update_screens - may change a current palette
			update_screens_by_bank_id( _unmark_upd_scr_btn, true );
			
			m_layout_editor.update();
			
			if( _show_status_msg )
			{
				set_status_msg( "Screen list updated" );
			}
		}

		private void update_all_screens( bool _unmark_upd_scr_btn, bool _show_status_msg = true )
		{
			m_imagelist_manager.update_all_screens( m_data_manager.get_tiles_data(), CBoxCHRBanks.SelectedIndex, m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
			
			// renew a palette
			palette_group.Instance.set_palette( get_curr_tiles_data() );
			
			LabelLayoutEditorCHRBankID.Text = CheckBoxScreensShowAllBanks.Checked ? "XXX":CBoxCHRBanks.SelectedIndex.ToString();
		
			if( _unmark_upd_scr_btn )
			{
				mark_update_screens_btn( false );
			}
			
			m_layout_editor.update();
			
			if( _show_status_msg )
			{
				set_status_msg( "Screen list updated" );
			}
		}

		private void update_screens_by_bank_id( bool _unmark_upd_scr_btn, bool _update_images )
		{
			m_imagelist_manager.update_screens( m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, _update_images, m_view_type, PropertyPerBlockToolStripMenuItem.Checked, CBoxCHRBanks.SelectedIndex, CheckBoxScreensShowAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
			
			// renew a palette
			palette_group.Instance.set_palette( get_curr_tiles_data() );
			
			LabelLayoutEditorCHRBankID.Text = CheckBoxScreensShowAllBanks.Checked ? "XXX":CBoxCHRBanks.SelectedIndex.ToString();
		
			if( _unmark_upd_scr_btn )
			{
				mark_update_screens_btn( false );
			}
		}
		
		private void update_screens_labels_by_bank_id()
		{
			m_imagelist_manager.update_screens_labels( m_data_manager.get_tiles_data(), CheckBoxScreensShowAllBanks.Checked ? -1:CBoxCHRBanks.SelectedIndex );
		}
	}
}
