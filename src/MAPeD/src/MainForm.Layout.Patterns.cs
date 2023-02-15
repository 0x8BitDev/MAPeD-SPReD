/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 16:35
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

namespace MAPeD
{
	partial class MainForm
	{
		private void patterns_manager_reset_active_pattern()
		{
			patterns_manager_reset_state();
			
			TreeViewPatterns.SelectedNode = null;
			
			m_layout_editor.set_param( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			
			patterns_manager_update_preview();
			
			m_layout_editor.update();
		}
		
		private void patterns_manager_update_data()
		{
			m_pattern_preview.reset_scale();
			
			patterns_manager_reset_state();
			
			patterns_manager_update_tree_view();
			
			patterns_manager_update_preview();
		}

		private void patterns_manager_reset_state()
		{
			CheckBoxPatternAdd.Checked = false;
			
			patterns_manager_enable( true );
		}
		
		private void patterns_manager_enable( bool _on )
		{
			TreeViewPatterns.Enabled		= _on;
			
			BtnPatternGroupAdd.Enabled		= _on;
			BtnPatternGroupDelete.Enabled	= _on;
			
			CheckBoxPatternAdd.Enabled		= _on;
			BtnPatternDelete.Enabled 		= _on;
			
			BtnPatternRename.Enabled		= _on;
			
			BtnPatternReset.Enabled			= _on;
		}

		private void patterns_manager_update_tree_view()
		{
			TreeViewPatterns.BeginUpdate();
			{
				TreeViewPatterns.Nodes.Clear();
			}
			TreeViewPatterns.EndUpdate();

			int ptrns_cnt = 0;
			
			tiles_data data = get_curr_tiles_data();
			
			if( data != null )
			{
				foreach( var key in data.patterns_data.Keys )
				{ 
					patterns_manager_pattern_group_add( key, false );
					
					( data.patterns_data[ key ] as List< pattern_data > ).ForEach( delegate( pattern_data _pattern ) { ++ptrns_cnt; patterns_manager_pattern_add( key, _pattern, false ); } );
				}
			}
			
			// on Linux TreeViewPatterns.SelectedNode is not reset when clearing nodes
			if( ptrns_cnt > 0 )
			{
				patterns_manager_reset_active_pattern();
			}
			else
			{
				TreeViewPatterns.SelectedNode = null;
			}
		}
		
		private bool patterns_manager_pattern_rename( string _grp_name, string _old_name, string _new_name )
		{
			tiles_data data = get_curr_tiles_data();
			
			List< pattern_data > patterns = data.patterns_data[ _grp_name ];
			
			if( patterns != null )
			{
				patterns.ForEach( delegate( pattern_data _pattern ) { if( _pattern.name == _old_name ) { _pattern.name = _new_name; return; } } );
				
				return true;
			}
			
			return false;
		}
		
		private bool patterns_manager_pattern_add( string _grp_name, pattern_data _pattern, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _pattern.name, true );

			if( nodes_arr.Length > 0 )
			{
				message_box( "A pattern with the same name (" + _pattern.name +  ") is already exist!", "Add Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			nodes_arr = TreeViewPatterns.Nodes.Find( _grp_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				if( _add_to_data_bank )
				{
					List< pattern_data > patterns = get_curr_tiles_data().patterns_data[ _grp_name ];
					
					if( patterns != null )
					{
						patterns.Add( _pattern );
					}
				}
				
				TreeNode node = new TreeNode( _pattern.name );
				node.Name = _pattern.name;
				node.ContextMenuStrip = ContextMenuStripPatternItem;
				
				TreeViewPatterns.BeginUpdate();
				{
					nodes_arr[ 0 ].Nodes.Add( node );
					
					nodes_arr[ 0 ].Expand();
					
					TreeViewPatterns.SelectedNode = node;
				}
				TreeViewPatterns.EndUpdate();
				
				return true;
			}
			
			return false;
		}
		
		private bool patterns_manager_pattern_delete( string _pattern_name, string _grp_name )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _pattern_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				List< pattern_data > patterns = get_curr_tiles_data().patterns_data[ _grp_name ];
				
				if( patterns != null )
				{
					pattern_data pattern;
					
					for( int ent_n = 0; ent_n < patterns.Count; ent_n++ )
					{
						pattern = patterns[ ent_n ];
						
						if( pattern.name == _pattern_name ) 
						{
							pattern.reset();

							patterns.Remove( pattern );

							break; 
						} 
					}
				}

				TreeNode node = new TreeNode( _pattern_name );
				node.Name = _pattern_name;
				node.ContextMenuStrip = ContextMenuStripPatternItem;
				
				TreeViewPatterns.BeginUpdate();
				{
					nodes_arr[ 0 ].Remove();
				}
				TreeViewPatterns.EndUpdate();
				
				return true;
			}
			
			return false;
		}
		
		private bool patterns_manager_pattern_group_rename( string _old_name, string _new_name )
		{
			tiles_data data = get_curr_tiles_data();
			
			if( data.patterns_data.ContainsKey( _old_name ) )
			{
				List< pattern_data > patterns = data.patterns_data[ _old_name ];
	
				data.patterns_data.Remove( _old_name );
				data.patterns_data.Add( _new_name, patterns );
					
				return true;
			}
			
			return false;
		}
		
		private bool patterns_manager_pattern_group_add( string _name, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _name, true );
			
			if( nodes_arr.Length > 0 )
			{
				message_box( "An item with the same name (" + _name +  ") is already exist!", "Add Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			TreeNode node = new TreeNode( _name );
			node.Name = _name;
			node.ContextMenuStrip = ContextMenuStripGroupItem;
				
			TreeViewPatterns.BeginUpdate();
			{
				TreeViewPatterns.Nodes.Add( node );
				
				TreeViewPatterns.SelectedNode = node;
			}
			TreeViewPatterns.EndUpdate();
			
			if( _add_to_data_bank )
			{
				get_curr_tiles_data().patterns_data.Add( _name, new List< pattern_data >() );
			}
			
			return true;
		}
		
		public bool patterns_manager_pattern_group_delete( string _name )
		{
			if( TreeViewPatterns.Nodes.ContainsKey( _name ) )
			{
				tiles_data data = get_curr_tiles_data();
					
				TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _name, true );
				
				if( nodes_arr.Length > 0 )
				{
					if( nodes_arr[ 0 ].FirstNode != null )
					{
						if( message_box( "The selected group is not empty!\n\nRemove all child patterns?", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
						{
							TreeViewPatterns.BeginUpdate();
							{
								TreeViewPatterns.Nodes.RemoveByKey( _name );
							}
							TreeViewPatterns.EndUpdate();
						}
						else
						{
							return false;
						}
					}
					else
					{
						TreeViewPatterns.BeginUpdate();
						{
							TreeViewPatterns.Nodes.RemoveByKey( _name );
						}
						TreeViewPatterns.EndUpdate();
					}
					
					if( data.patterns_data.ContainsKey( _name ) )
					{
						List< pattern_data > patterns = data.patterns_data[ _name ];

						data.patterns_data.Remove( _name );
						
						if( patterns != null )
						{
							patterns.ForEach( delegate( pattern_data _pattern ) { _pattern.reset(); } );
						}
						
						return true;
					}
				}
			}
			
			return false;
		}

		private void patterns_manager_update_preview()
		{
			m_pattern_preview.set_scaled_image( null );
			m_pattern_preview.scale_enabled( false );
			
			if( TreeViewPatterns.SelectedNode == null )
			{
				// redraw a current selected pattern
				m_pattern_preview.update_scaled( true );
				
				m_pattern_preview.draw_string( "Tile patterns are often-used combinations of tiles.\nHere you can create and manage them.\n\n- Select or create a new patterns group\n\n- Click the 'Add' pattern button to add a new one.\n\n- Select a pattern in the tree view to place it on a map.\n\n- Scale selected pattern using a mouse wheel.", 0, 0 );
			}
			else
			{
				if( CheckBoxPatternAdd.Checked )
				{
					// redraw a current selected pattern
					m_pattern_preview.update_scaled( true );
				}
				else
				{
					TreeNode node = TreeViewPatterns.SelectedNode;
					
					if( node == null || node.Parent == null )
					{
						m_pattern_preview.update_scaled( true );
					
						m_pattern_preview.draw_string( "Select a pattern or create a new one.", 0, 0 );
					}
					else
					{
						// draw a pattern
						pattern_data pattern = get_curr_tiles_data().get_pattern_by_name( node.Name );

						if( pattern == null )
						{
							throw new Exception( "UNEXPECTED ERROR!\n\nCan't find the pattern <" + node.Name + ">!" );
						}
						
						patterns_manager_update_pattern_image( pattern );
						
						m_pattern_preview.set_scaled_image( m_pattern_image );
						m_pattern_preview.scale_enabled( true );
						m_pattern_preview.update_scaled( true );
						
						m_pattern_preview.draw_string( "Place the <" + node.Name + " [" + pattern.width + "x" + pattern.height + "]> on a map.", 0, 0 );
					}
				}
			}
		}
		
		private void patterns_manager_update_pattern_image( pattern_data _pttrn )
		{
			m_pattern_gfx.Clear( Color.FromArgb( 0 ) );

			int scr_tile_size	= utils.CONST_SCREEN_TILES_SIZE >> 1;
			List< Bitmap > img_list;
			
			if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				img_list = m_imagelist_manager.get_tiles_image_list();
			}
			else
			{
				img_list = m_imagelist_manager.get_blocks_image_list();
				scr_tile_size >>= 1;
			}
			
			int start_pos_x = ( m_pattern_image.Width >> 1 ) - ( ( _pttrn.width * scr_tile_size ) >> 1 );
			int start_pos_y = ( m_pattern_image.Height >> 1 ) - ( ( _pttrn.height * scr_tile_size ) >> 1 );
			
			for( int tile_y = 0; tile_y < _pttrn.height; tile_y++ )
			{
				for( int tile_x = 0; tile_x < _pttrn.width; tile_x++ )
				{
					m_pattern_gfx.DrawImage( img_list[ _pttrn.data.get_tile( tile_y * _pttrn.width + tile_x ) ], start_pos_x + tile_x * scr_tile_size, start_pos_y + tile_y * scr_tile_size, scr_tile_size, scr_tile_size );
				}
			}
		}
		
		private void BtnPatternGroupAddClick( object sender, EventArgs e )
		{
			m_object_name_form.Text = "Add Group";
			
			m_object_name_form.edit_str = "GROUP";
			
			if( m_object_name_form.show_window() == DialogResult.OK )
			{
				patterns_manager_pattern_group_add( m_object_name_form.edit_str, true );
			}
		}
		
		private void BtnPatternGroupDeleteClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent == null )
				{
					patterns_manager_pattern_group_delete( sel_node.Name );

					patterns_manager_update_preview();
				}
				else
				{
					message_box( "Please, select a group!", "Delete Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPatterns.Nodes.Count > 0 )
				{
					message_box( "Please, select a group!", "Delete Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "Delete Group", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void BtnPatternAddClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node == null || sel_node.Parent != null )
			{
				message_box( "Please, select a group!", "Add Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				CheckBoxPatternAdd.Checked = false;
				
				return;
			}
			
			CheckBoxPatternAdd.Checked ^= true;
			
			patterns_manager_enable( !CheckBoxPatternAdd.Checked );
			CheckBoxPatternAdd.Enabled = true;
			
			patterns_manager_update_preview();
		}

		private void BtnPatternAddChanged( object sender, EventArgs e )
		{
			if( CheckBoxPatternAdd.Checked )
			{
				CheckBoxPatternAdd.FlatStyle = FlatStyle.Standard;
				CheckBoxPatternAdd.Text = "Cancel";
				
				m_layout_editor.set_param( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SET_PTTRN_EXTRACT_BEGIN, null );
			}
			else
			{
				CheckBoxPatternAdd.FlatStyle = FlatStyle.System;
				CheckBoxPatternAdd.Text = "Add";
				
				m_layout_editor.set_param( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			}
			
			m_layout_editor.update();
		}
		
		private void on_pattern_extract_end( object sender, EventArgs e )
		{
			m_object_name_form.Text = "Add Pattern";
			
			m_object_name_form.edit_str = "PATTERN";
			
			if( m_object_name_form.show_window() == DialogResult.OK )
			{
				patterns_manager_reset_state();
				
				PatternEventArg pattern_event = e as PatternEventArg;
				
				pattern_data data = pattern_event.data;
				data.name = m_object_name_form.edit_str;
				
				patterns_manager_pattern_add( TreeViewPatterns.SelectedNode.Name, data, true );
				
				MainForm.set_status_msg( "Added tiles pattern <" + m_object_name_form.edit_str + ">" );
				
				patterns_manager_update_preview();
			}
			else
			{
				patterns_manager_reset_state();
			}
		}
		
		private void on_pattern_place_cancel( object sender, EventArgs e )
		{
			patterns_manager_reset_active_pattern();
		}
		
		private void BtnPatternDeleteClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					if( message_box( "Are you sure?", "Delete Pattern", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						if( patterns_manager_pattern_delete( sel_node.Name, sel_node.Parent.Name ) )
						{
							MainForm.set_status_msg( "Deleted tiles pattern <" + sel_node.Name + ">" );
							
							patterns_manager_update_preview();
						}
					}
				}
				else
				{
					message_box( "Please, select a pattern!", "Delete Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPatterns.Nodes.Count > 0 )
				{
					message_box( "Please, select a pattern!", "Delete Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "Delete Pattern", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void BtnPatternRenameClick( object sender, EventArgs e )
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				sel_node.BeginEdit();
			}
			else
			{
				message_box( "Please, select an item!", "Rename Item", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnPatternResetClick( object sender, EventArgs e )
		{
			patterns_manager_reset_active_pattern();
		}
		
		private void PatternsTreeViewNodeSelect( object sender, TreeViewEventArgs e )
		{
			if( TreeViewPatterns.SelectedNode.Parent != null )
			{
				m_layout_editor.set_param( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SET_PTTRN_PLACE, get_curr_tiles_data().get_pattern_by_name( TreeViewPatterns.SelectedNode.Name ) );
			}
			else
			{
				m_layout_editor.set_param( layout_editor_base.e_mode.Patterns, layout_editor_param.CONST_SET_PTTRN_IDLE_STATE, null );
			}
			
			patterns_manager_update_preview();
			
			m_layout_editor.update();
		}
		
		private void PatternsTreeViewNodeRename( object sender, NodeLabelEditEventArgs e )
		{
			if( e.Label != null )
			{
				TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( e.Label, true );
				
				if( ( nodes_arr.Length > 0 ) && ( nodes_arr[ 0 ] != e.Node ) )
				{
					e.CancelEdit = true;
					
					message_box( "An item with the same name (" + e.Label +  ") is already exist!", "Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					
					return;
				}
				
				if( e.Node.Parent == null )
				{
					patterns_manager_pattern_group_rename( e.Node.Text, e.Label );
					e.Node.Name = e.Label;
				}
				else
				{
					patterns_manager_pattern_rename( e.Node.Parent.Text, e.Node.Text, e.Label );
					e.Node.Name = e.Label;
					
					patterns_manager_update_preview();
				}
			}
		}

		private void on_patterns_manager_mouse_enter( object sender, EventArgs e )
		{
			if( TreeViewPatterns.Enabled )
			{
				m_pattern_preview.set_focus();
			}
		}
	}
}
