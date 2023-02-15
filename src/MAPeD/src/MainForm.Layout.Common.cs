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
		private void TreeViewMouseDown( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				TreeView tree_view = sender as TreeView;
				
				if( tree_view != null )
				{
					tree_view.SelectedNode = tree_view.GetNodeAt( e.X, e.Y );
				}
			}
		}
		
		private void TabControlLayoutToolsSelected( object sender, TabControlEventArgs e )
		{
			TabPage curr_tab = ( sender as TabControl ).SelectedTab;
			
			// reset common states
			{
				LayoutContextMenuEntityItemsEnable( false );
				
				screensToolStripMenuItem.Enabled	= 
				builderToolStripMenuItem.Enabled	= 
				entitiesToolStripMenuItem.Enabled	= 
				patternsToolStripMenuItem.Enabled	= false;
				
				ListViewScreens.SelectedItems.Clear();
				
				TreeViewEntities.SelectedNode = null;
				
				CheckBoxSelectTargetEntity.Checked = false;
				
				CheckBoxPainterReplaceTiles.Checked = false;
				
				patterns_manager_reset_active_pattern();
			}
			
			if( curr_tab == TabBuilder )
			{
				builderToolStripMenuItem.Enabled = true;

				m_layout_editor.mode = layout_editor_base.e_mode.Builder;
			}
			else
			if( curr_tab == TabPainter )
			{
				m_layout_editor.mode = layout_editor_base.e_mode.Painter;
			}
			else
			if( curr_tab == TabScreenList )
			{
				screensToolStripMenuItem.Enabled = true;
				
				m_layout_editor.mode = layout_editor_base.e_mode.Screens;
			}
			else
			if( curr_tab == TabEntities )
			{
				entitiesToolStripMenuItem.Enabled = true;
				
				m_layout_editor.mode = layout_editor_base.e_mode.Entities;
				
				fill_entity_data( get_selected_entity() );
			}
			else
			if( curr_tab == TabPatterns )
			{
				patternsToolStripMenuItem.Enabled = true;
				
				m_layout_editor.mode = layout_editor_base.e_mode.Patterns;
			}
			else
			{
				throw new Exception( "Unknown mode detected!\n\n[MainForm.TabControlLayoutToolsSelected]" );
			}
		}
		
		private void LayoutContextMenuEntityItemsEnable( bool _on )
		{
			LayoutDeleteEntityToolStripMenuItem.Enabled = LayoutEntityOrderToolStripMenuItem.Enabled = _on;
		}
		
		private void LayoutDeleteAllScreenMarksToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "Are you sure?", "Delete All Screen Marks", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_all_screen_marks();
				
				m_layout_editor.update();
				
				set_status_msg( "Layout Editor: all screen marks deleted" );
			}
		}
		
		private void LayoutEntityBringFrontToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_SEL_BRING_FRONT, 0 );
		}
		
		private void LayoutEntitySendBackToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_SEL_SEND_BACK, 0 );
		}
		
		private void LayoutDeleteScreenToolStripMenuItemClick( object sender, EventArgs e )
		{
			entity_instance ent_inst = null;
			
			if( m_layout_editor.mode == layout_editor_base.e_mode.Entities )
			{
				ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			}

			if( m_layout_editor.delete_screen_from_layout( delegate( layout_screen_data _scr_data ) { delete_screen( _scr_data ); } ) == true )
			{
				if( ent_inst != ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED ) )
				{
					// selected entity has been deleted
					fill_entity_data( null );
					
					CheckBoxSelectTargetEntity.Checked = false;
				}
				else
				{
					if( ent_inst != null )
					{
						fill_entity_data( ent_inst.base_entity, ent_inst.properties, ent_inst.name, ent_inst.target_uid );
					}
				}
				
				set_status_msg( "Layout Editor: screen deleted" );
			}
		}
		
		private void LayoutDeleteScreenEntitiesToolStripMenuItemClick( object sender, EventArgs e )
		{
			entity_instance ent_inst = null;
			
			if( m_layout_editor.mode == layout_editor_base.e_mode.Entities )
			{
				ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			}

			if( m_layout_editor.delete_screen_entities() == true )
			{
				if( ent_inst != ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED ) )
				{
					// selected entity has been deleted
					fill_entity_data( null );
					
					CheckBoxSelectTargetEntity.Checked = false;
				}
				else
				{
					if( ent_inst != null )
					{
						fill_entity_data( ent_inst.base_entity, ent_inst.properties, ent_inst.name, ent_inst.target_uid );
					}
				}
				
				set_status_msg( "Entities Editor: all the screen entities are deleted" );
			}
		}
		
		private void LayoutDeleteEntityToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_DELETE, null ) == true )
			{
				fill_entity_data( null );
				
				CheckBoxSelectTargetEntity.Checked = false;
				
				set_status_msg( "Layout Editor: entity deleted" );
			}
		}
		
		private void SetStartScreenMarkToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_layout_editor.set_start_screen_mark();
		}
		
		private void SetScreenMarkToolStripMenuItemClick( object sender, EventArgs e )
		{
			m_layout_editor.set_screen_mark();
		}
		
		private void AdjScrMaskClick( object sender, EventArgs e )
		{
			m_layout_editor.set_adjacent_screen_mask( ( sender as ToolStripMenuItem ).Text );
		}
	}
}
