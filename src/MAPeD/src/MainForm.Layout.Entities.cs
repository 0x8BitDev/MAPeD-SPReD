/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 16:35
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MAPeD
{
	partial class MainForm
	{
		private void on_edit_entity_cancel( object sender, EventArgs e )
		{
			switch( ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) )
			{
				case layout_editor_param.CONST_SET_ENT_EDIT:
				{
					BtnEntitiesEditInstancesModeClick( sender, e );
				}
				break;
				
				case layout_editor_param.CONST_SET_ENT_SELECT_TARGET:
				{
					CheckBoxSelectTargetEntity.Checked = false;
				}
				break;
			}
		}
		
		private void on_entities_counter_update( object sender, EventArgs e )
		{
			if( m_layout_editor.mode == layout_editor_base.e_mode.Entities )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_UPDATE_ENTS_CNT, null );
				
				TreeViewEntities.Refresh();
			}
		}
		
		private bool m_rename_ent_tree_node = true;
		
		private void TreeViewEntitiesDrawNode( object sender, DrawTreeNodeEventArgs e )
		{
			if( e.Node.Parent != null )
			{
				if( m_rename_ent_tree_node )
				{
					int ent_inst_cnt = ( ListBoxLayouts.SelectedIndex >= 0 ) ? m_data_manager.get_num_ent_instances_by_name( e.Node.Name ):0;
					
					string tag_str = "[" + ent_inst_cnt + "]  ";
					
					e.Node.Tag = tag_str;
					
					if( e.Node.Text.IndexOf( tag_str ) < 0 )
					{
						e.Node.Text = tag_str + e.Node.Name;
					}
				}
			}
			
			e.DrawDefault = true;
		}
		
		private void TreeViewEntitiesNodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
			{
				if( e.Node.IsSelected )
				{
					m_rename_ent_tree_node = false;
					{
						e.Node.Text = e.Node.Name;
						
						e.Node.TreeView.Refresh();
					}
					m_rename_ent_tree_node = true;
				}
			}
		}
		
		private void TreeViewEntitiesBeforeLabelEdit( object sender, NodeLabelEditEventArgs e )
		{
			m_rename_ent_tree_node = false;
			
			e.Node.Text = e.Node.Name;
			
			e.Node.TreeView.Refresh();
		}
		
		private void TreeViewEntitiesAfterLabelEdit( object sender, NodeLabelEditEventArgs e )
		{
			m_rename_ent_tree_node = true;
			
			if( e.Label != null )
			{
				string new_label = e.Label;
				
				// remove tag from the e.Label
				{
					string tag_str = e.Node.Tag.ToString();
					
					if( new_label.IndexOf( tag_str ) == 0 )
					{
						new_label = new_label.Substring( tag_str.Length );
					}
				}
				
				TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( new_label, true );
				
				if( ( nodes_arr.Length > 0 ) && ( nodes_arr[ 0 ] != e.Node ) )
				{
					e.CancelEdit = true;
					
					message_box( "An item with the same name (" + new_label +  ") is already exist!", "Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return;
				}
				
				if( e.Node.Parent == null )
				{
					m_data_manager.group_rename( e.Node.Text, new_label );
					e.Node.Name = new_label;
				}
				else
				{
					m_data_manager.entity_rename( e.Node.Parent.Text, e.Node.Text, new_label );
					e.Node.Name = new_label;
					
					fill_entity_data( get_selected_entity() );
				}
			}
		}

		private void TreeViewEntitiesSelect( object sender, TreeViewEventArgs e )
		{
			if( tabControlLayoutTools.SelectedTab == TabEntities )
			{
				entity_data ent = get_selected_entity();
				
				if( ent != null )
				{
					set_entity_mode( layout_editor_param.CONST_SET_ENT_EDIT );
				}
				
				fill_entity_data( ent );
			}
		}
		
		private void BtnEntityGroupAddClick( object sender, EventArgs e )
		{
			m_object_name_form.Text = "Add Group";
			
			m_object_name_form.edit_str = "GROUP";
			
			if( m_object_name_form.show_window() == DialogResult.OK )
			{
				m_data_manager.group_add( m_object_name_form.edit_str );
				
				set_status_msg( "Added group" );
			}
		}
		
		private void BtnEntityGroupDeleteClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent == null )
				{
					m_data_manager.group_delete( sel_node.Name );

					update_active_entity();
					
					set_status_msg( "Group deleted" );
				}
				else
				{
					message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewEntities.Nodes.Count > 0 )
				{
					message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		private void BtnEntityGroupRenameClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				sel_node.BeginEdit();
			}
			else
			{
				message_box( "Please, select a group!", "Group Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnEntityAddClick( object sender, EventArgs e )
		{
			m_object_name_form.Text = "Add Entity";
			
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node == null )
			{
				message_box( "Please, select a group!", "Entity Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return;
			}
			
			m_object_name_form.edit_str = "entity";
			
			if( m_object_name_form.show_window() == DialogResult.OK )
			{
				m_data_manager.entity_add( m_object_name_form.edit_str, ( sel_node.Parent != null ? sel_node.Parent.Name:sel_node.Name ) );
				
				set_status_msg( "Added entity" );
				
				set_entity_mode( layout_editor_param.CONST_SET_ENT_EDIT );
				
				fill_entity_data( get_selected_entity() );
			}
		}
		
		private void BtnEntityDeleteClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					m_data_manager.entity_delete( sel_node.Name, sel_node.Parent.Name );

					update_active_entity();
					
					set_status_msg( "Entity deleted" );
				}
				else
				{
					message_box( "Please, select an entity!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewEntities.Nodes.Count > 0 )
				{
					message_box( "Please, select an entity!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void BtnEntityRenameClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null )
			{
				m_rename_ent_tree_node = false;
				{
					sel_node.Text = sel_node.Name;
					
					sel_node.TreeView.Refresh();
				}
				m_rename_ent_tree_node = true;
				
				sel_node.BeginEdit();
			}
			else
			{
				message_box( "Please, select an item!", "Item Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void CheckBoxEntitySnappingChanged( object sender, EventArgs e )
		{
			m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_SNAPPING, ( sender as CheckBox ).Checked );
		}
		
		private void BtnEntitiesEditInstancesModeClick( object sender, EventArgs e )
		{
			if( tabControlLayoutTools.SelectedTab == TabEntities )
			{
				set_entity_mode( layout_editor_param.CONST_SET_ENT_INST_EDIT );
			}
		}
		
		private void on_entities_tree_view_update_data( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;  
			
			TreeViewEntities.BeginUpdate();
			{
				TreeViewEntities.Nodes.Clear();
				
				Dictionary< string, List< entity_data > > ents_data = data_mngr.entities_data;
				
				foreach( var key in data_mngr.entities_data.Keys ) 
				{
					on_entities_tree_view_add_group( null, new EventArg2Params( key, null ) );
					
					( data_mngr.entities_data[ key ] as List< entity_data > ).ForEach( delegate( entity_data _ent ) { on_entities_tree_view_add_entity( null, new EventArg2Params( _ent.name, key ) ); } );
				}
			}
			TreeViewEntities.EndUpdate();
		}

		private bool on_entities_tree_view_add_group( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string name = args.param1 as string;
			
			TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( name, true );
			
			if( nodes_arr.Length > 0 )
			{
				message_box( "An item with the same name (" + name +  ") is already exist!", "Group Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			TreeNode node = new TreeNode( name );
			node.Name = name;
			node.ContextMenuStrip = ContextMenuEntitiesTreeGroup;
				
			TreeViewEntities.BeginUpdate();
			{
				TreeViewEntities.Nodes.Add( node );
				
				TreeViewEntities.SelectedNode = node;
			}
			TreeViewEntities.EndUpdate();
			
			return true;
		}
		
		private bool on_entities_tree_view_delete_group( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string name = args.param1 as string;
			
			if( TreeViewEntities.Nodes.ContainsKey( name ) )
			{
				TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( name, true );
				
				if( nodes_arr.Length > 0 )
				{
					if( nodes_arr[ 0 ].FirstNode != null )
					{
						if( message_box( "The selected group is not empty!\n\nRemove all child entities?", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
						{
							TreeViewEntities.BeginUpdate();
							{
								TreeViewEntities.Nodes.RemoveByKey( name );
							}
							TreeViewEntities.EndUpdate();
							
							return true;
						}
					}
					else
					{
						TreeViewEntities.BeginUpdate();
						{
							TreeViewEntities.Nodes.RemoveByKey( name );
						}
						TreeViewEntities.EndUpdate();
						
						return true;
					}
				}
			}
			
			return false;
		}
		
		private bool on_entities_tree_view_add_entity( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string ent_name = args.param1 as string;
			string grp_name = args.param2 as string;
			
			TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( ent_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				message_box( "An item with the same name (" + ent_name +  ") is already exist!", "Entity Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			nodes_arr = TreeViewEntities.Nodes.Find( grp_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				TreeNode node = new TreeNode( ent_name );
				node.Name = ent_name;
				node.ContextMenuStrip = ContextMenuEntitiesTreeEntity;
				
				TreeViewEntities.BeginUpdate();
				{
					nodes_arr[ 0 ].Nodes.Add( node );
					
					nodes_arr[ 0 ].Expand();
					
					TreeViewEntities.SelectedNode = node;
				}
				TreeViewEntities.EndUpdate();
				
				return true;
			}
			
			return false;
		}

		private bool on_entities_tree_view_delete_entity( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			string ent_name = args.param1 as string;
			
			TreeNode[] nodes_arr = TreeViewEntities.Nodes.Find( ent_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				if( nodes_arr[ 0 ].Parent == null )
				{
					message_box( "Please, select an entity!", "Base Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return false;
				}
				
				if( message_box( "All the entity <" + ent_name + "> instances will be deleted from all maps!\nAre you sure?", "Delete Base Entity", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
				{
					TreeViewEntities.BeginUpdate();
					{
						nodes_arr[ 0 ].Remove();
					}
					TreeViewEntities.EndUpdate();
					
					return true;
				}
				
				return false;
			}
			
			message_box( "Can't find entity (" + ent_name +  ")!", "Base Entity Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			
			return false;
		}

		private void on_entity_instance_selected( object sender, EventArgs e )
		{
			EventArg2Params args = e as EventArg2Params;
			
			entity_instance ent_inst = args.param1 as entity_instance;
			
			uint ent_mode = ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE );
			
			if( ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT )
			{
				if( ent_inst != null )
				{
					fill_entity_data( ent_inst.base_entity, ent_inst.properties, ent_inst.name, ent_inst.target_uid );
				}
				else
				{
					fill_entity_data( null, null );
				}
			}
			else
			if( ent_mode == layout_editor_param.CONST_SET_ENT_SELECT_TARGET )
			{
				entity_instance edit_ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
				
				if( edit_ent_inst != null )
				{
					if( ent_inst == null )
					{
						// reset selected entity if user clicked on empty space
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, 0 );
						
						fill_entity_data( null, null );
						
						// auto disable the 'target entity' mode
						CheckBoxSelectTargetEntity.Checked = false;
					}
					else
					{
						edit_ent_inst.target_uid = ( ent_inst != edit_ent_inst ) ? ent_inst.uid:-1;

						fill_entity_data( edit_ent_inst.base_entity, edit_ent_inst.properties, edit_ent_inst.name, edit_ent_inst.target_uid );
					}
				}
			}
		}
		
		private void on_entities_manager_mouse_enter( object sender, EventArgs e )
		{
			if( TreeViewEntities.Enabled )
			{
				m_entity_preview.set_focus();
			}
		}
		
		private void update_active_entity()
		{
			entity_data sel_ent = get_selected_entity();
			
			if( sel_ent == null )
			{
				set_entity_mode( layout_editor_param.CONST_SET_ENT_INST_EDIT );
			}
			else
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, sel_ent );
				m_layout_editor.update();
			}
		}
		
		private void set_entity_mode( uint _mode )
		{
			switch( _mode )
			{
				case layout_editor_param.CONST_SET_ENT_EDIT:
					{
						CheckBoxSelectTargetEntity.Checked = false;
						
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, get_selected_entity() );
						
						m_layout_editor.set_param( _mode, null );
					}
					break;
					
				case layout_editor_param.CONST_SET_ENT_INST_EDIT:
					{
						TreeViewEntities.SelectedNode = null;
						
						CheckBoxSelectTargetEntity.Checked = false;
						
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, null );
						
						m_layout_editor.set_param( _mode, null );
						
						fill_entity_data( get_selected_entity() );
					}
					break;
					
				case layout_editor_param.CONST_SET_ENT_SELECT_TARGET:
					{
						m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, null );
						
						m_layout_editor.set_param( _mode, null );
					}
					break;
					
				default:
					{
						throw new Exception( "Unknown mode detected!\n\n[MainForm.layout_editor_set_entity_mode]" );
					}
			}
			
			m_layout_editor.update();
		}
		
		private entity_data get_selected_entity()
		{
			entity_data ent = null;
			
			TreeNode sel_node = TreeViewEntities.SelectedNode;
			
			if( sel_node != null && sel_node.Parent != null )
			{
				List< entity_data > ent_list = m_data_manager.entities_data[ sel_node.Parent.Name ] as List< entity_data >;
				
				ent_list.ForEach( delegate( entity_data _ent ) { if( _ent.name == sel_node.Name ) { ent = _ent; return; } } );
			}
			
			return ent;
		}
		
		private void DeleteAllInstancesToolStripMenuItemClick( object sender, EventArgs e )
		{
			string ent_name = TreeViewEntities.SelectedNode.Name;
			
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "All the entity <" + ent_name + "> instances will be deleted from the active map!\nAre you sure?", "Delete Entities", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
			{
				int ents_cnt = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_entity_instances( ent_name );
				
				m_layout_editor.update();
				
				set_status_msg( "Entities Editor: all the entity <" + ent_name + "> instances are deleted [" + ents_cnt + "]" );
			}
		}
		
		private void EntitiesDeleteInstancesOfAllEntitiesToolStripMenuItemClick( object sender, EventArgs e )
		{
			entity_instance ent_inst = null;
			
			if( m_layout_editor.mode == layout_editor_base.e_mode.Entities )
			{
				ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			}
			
			if( m_data_manager.layouts_data_cnt > 0 && message_box( "All the entities will be deleted from the active map!\nAre you sure?", "Delete All Entities", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
			{
				m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).delete_all_entities();
				
				if( ent_inst != null )
				{
					m_layout_editor.set_param( layout_editor_base.e_mode.Entities, layout_editor_param.CONST_SET_ENT_INST_RESET, null );
					
					fill_entity_data( null );
					
					CheckBoxSelectTargetEntity.Checked = false;
				}
				
				m_layout_editor.update();
				
				set_status_msg( "Entities Editor: the instances of all entities are deleted" );
			}
		}
	}
}
