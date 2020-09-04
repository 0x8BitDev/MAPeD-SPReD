/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 03.09.2020
 * Time: 16:34
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of presets_form.
	/// </summary>
	///
	
	public delegate void PresetsManagerClosed();
	
	public partial class presets_manager_form : Form
	{
		public event EventHandler PresetsManagerClosed;
		
		private tiles_data	m_data				= null;
		
		private ImageList 	m_tiles_image_list 	= null;
		
		private image_preview	m_preset_preview	= null;
		
		private object_name_form	m_object_name_form	= null;
		
		public presets_manager_form( ImageList _tiles_image_list )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_tiles_image_list 	= _tiles_image_list;
			
			m_preset_preview = new image_preview( PixBoxPreview );
			
			m_object_name_form = new object_name_form();
		}
		
		public void reset()
		{
			m_data = null;
			
			update();			
		}
		
		public void enable( bool _on )
		{
			TreeViewPresets.Enabled		= _on;
			
			BtnGroupAdd.Enabled		= _on;
			BtnGroupDelete.Enabled	= _on;
			
			CheckBoxAddPreset.Enabled	= _on;
			BtnPresetDelete.Enabled 	= _on;
			
			BtnRename.Enabled		= _on;
		}
		
		public void update()
		{
			if( TreeViewPresets.SelectedNode == null )
			{
				if( CheckBoxAddPreset.Checked )
				{
					// redraw a current selected preset
					m_preset_preview.update( null, -1, -1, -1, -1, -1, false, false );
					
					m_preset_preview.draw_string( "Select a rectangle area in the screen editor\nto create a preset.", 0, 0 );
				}
				else
				{
					// redraw a current selected preset
					m_preset_preview.update( null, -1, -1, -1, -1, -1, false, false );
					
					m_preset_preview.draw_string( "Presets are frequently used patterns of tiles.\nHere you can create and manage them.\n\n- Press the 'Add' preset button to add a new one.\n\n- Select a preset in the tree view to put it on\na game screen.", 0, 0 );
				}
			}
			
			m_preset_preview.invalidate();
		}
		
		public void set_data( tiles_data _data )
		{
			m_data = _data;
			
			update_tree_view();
			
			update();
		}
		
		private void update_tree_view()
		{
			//...
		}
		
		void BtnCloseClick_event(object sender, EventArgs e)
		{
			hide_wnd();
		}
		
		void Closing_event(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;

			hide_wnd();
		}
		
		private void hide_wnd()
		{
			Visible = false;
			
			if( PresetsManagerClosed != null )
			{
				PresetsManagerClosed( this, null );
			}
			
			CheckBoxAddPreset.Checked = false;
		}
		
		void BtnGroupAddClick_Event(object sender, EventArgs e)
		{
			m_object_name_form.Text = "Add Group";
			
			m_object_name_form.edit_str = "GROUP";
			
			if( m_object_name_form.ShowDialog() == DialogResult.OK )
			{
				group_add( m_object_name_form.edit_str );
			}
		}
		
		void BtnGroupDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPresets.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent == null )
				{
					group_delete( sel_node.Name );

					update();
				}
				else
				{
					MainForm.message_box( "Please, select a group!", "Delete Group Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPresets.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a group!", "Delete Group Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Delete Group Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void CheckBoxAddPresetChanged_Event(object sender, EventArgs e)
		{
			update();
		}
		
		void BtnPresetDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPresets.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					preset_delete( sel_node.Name, sel_node.Parent.Name );

					update();
				}
				else
				{
					MainForm.message_box( "Please, select a preset!", "Delete Preset Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPresets.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a preset!", "Delete Preset Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Delete Preset Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnRenameClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPresets.SelectedNode;
			
			if( sel_node != null )
			{
				sel_node.BeginEdit();
			}
			else
			{
				MainForm.message_box( "Please, select an item!", "Rename Item Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		void TreeViewNodeRename_Event(object sender, NodeLabelEditEventArgs e)
		{
			if( e.Label != null )
			{
				if( e.Node.Parent == null )
				{
					group_rename( e.Node.Text, e.Label );
					e.Node.Name = e.Label;
				}
				else
				{
					preset_rename( e.Node.Parent.Text, e.Node.Text, e.Label );
					e.Node.Name = e.Label;
					
					update();
				}
			}
		}
		
		void TreeViewNodeSelect_Event(object sender, TreeViewEventArgs e)
		{
			update();
		}
		
		public bool preset_rename( string _grp_name, string _old_name, string _new_name )
		{
			List< preset_data > presets = m_data.presets_data[ _grp_name ];
			
			if( presets != null )
			{
				presets.ForEach( delegate( preset_data _preset ) { if( _preset.name == _old_name ) { _preset.name = _new_name; return; } } );
				
				return true;
			}
			
			return false;
		}
		
		public bool preset_add( string _preset_name, string _grp_name, byte _width, byte _height, byte[] _data )
		{
			List< preset_data > presets = m_data.presets_data[ _grp_name ];
			
			if( presets != null )
			{
				presets.Add( new preset_data( _preset_name, _width, _height, _data ) );
				
				return true;
			}
			
			return false;
		}
		
		public bool preset_delete( string _preset_name, string _grp_name )
		{
			List< preset_data > presets = m_data.presets_data[ _grp_name ];
			
			if( presets != null )
			{
				preset_data preset;
				
				for( int ent_n = 0; ent_n < presets.Count; ent_n++ )				
	            {
					preset = presets[ ent_n ];
					
	             	if( preset.name == _preset_name ) 
	             	{
	             		preset.reset();
	             		
	             		presets.Remove( preset );
	             		
	             		break; 
	             	} 
				}
				
				return true;
			}
			
			return false;
		}
			
		private bool group_rename( string _old_name, string _new_name )
		{
			if( m_data.presets_data.ContainsKey( _old_name ) )
			{
				List< preset_data > presets = m_data.presets_data[ _old_name ];
	
				m_data.presets_data.Remove( _old_name );
				m_data.presets_data.Add( _new_name, presets );
					
				return true;
			}
			
			return false;
		}
		
		private bool group_add( string _name )
		{
			TreeNode[] nodes_arr = TreeViewPresets.Nodes.Find( _name, true );
			
			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "An item with the same name (" + _name +  ") is already exist!", "Add Group Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			TreeNode node = new TreeNode( _name );
			node.Name = _name;
			node.ContextMenuStrip = ContextMenuStripGroupItem;
				
			TreeViewPresets.BeginUpdate();
			{
				TreeViewPresets.Nodes.Add( node );
				
				TreeViewPresets.SelectedNode = node;
			}
			TreeViewPresets.EndUpdate();
			
			m_data.presets_data.Add( _name, null );
			
			return true;
		}
		
		public bool group_delete( string _name )
		{
			if( TreeViewPresets.Nodes.ContainsKey( _name ) )
			{
				TreeNode[] nodes_arr = TreeViewPresets.Nodes.Find( _name, true );
				
				if( nodes_arr.Length > 0 )
				{
					if( nodes_arr[ 0 ].FirstNode != null )
					{
						if( MainForm.message_box( "The selected group is not empty!\n\nRemove all child presets?", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
						{
							TreeViewPresets.BeginUpdate();
							{
								TreeViewPresets.Nodes.RemoveByKey( _name );
							}
							TreeViewPresets.EndUpdate();
						}
						else
						{
							return false;
						}
					}
					else
					{
						TreeViewPresets.BeginUpdate();
						{
							TreeViewPresets.Nodes.RemoveByKey( _name );
						}
						TreeViewPresets.EndUpdate();
					}
					
					if( m_data.presets_data.ContainsKey( _name ) )
					{
						List< preset_data > presets = m_data.presets_data[ _name ];

						m_data.presets_data.Remove( _name );
						
						if( presets != null )
						{
							presets.ForEach( delegate( preset_data _preset ) { _preset.reset(); } );
						}
						
						return true;
					}
				}
			}
			
			return false;
		}
	}
}
