/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 03.09.2020
 * Time: 16:34
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of presets_form.
	/// </summary>
	///

	public class PresetEventArg : EventArgs
	{
		private byte m_width	= 0;
		private byte m_height	= 0;
		
		private byte[] m_data	= null;
		
		public byte width
		{
			get { return m_width; }
			set {}
		}
		
		public byte height
		{
			get { return m_height; }
			set {}
		}
		
		public byte[] data
		{
			get { return m_data; }
			set {}
		}
		
		public PresetEventArg( byte _width, byte _height, byte[] _data )
		{
			m_width		= _width;
			m_height	= _height;
			m_data		= _data;
		}
	}
	
	public delegate void PresetsManagerClosed();
	public delegate void CreatePresetBegin();
	public delegate void CreatePresetCancel();
	
	public partial class presets_manager_form : Form
	{
		public event EventHandler PresetsManagerClosed;
		public event EventHandler CreatePresetBegin;		
		public event EventHandler CreatePresetCancel;
		
		private tiles_data	m_data				= null;
		
		private ImageList 	m_tiles_image_list 	= null;
		
		private image_preview		m_preset_preview	= null;
		private object_name_form	m_object_name_form	= null;
		
		private Bitmap		m_preset_image	= null;
		private Graphics	m_gfx			= null;
		
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
			
			m_preset_image = new Bitmap( utils.CONST_SCREEN_WIDTH_PIXELS, utils.CONST_SCREEN_HEIGHT_PIXELS, PixelFormat.Format32bppArgb );
			m_gfx = Graphics.FromImage( m_preset_image );
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
				// redraw a current selected preset
				m_preset_preview.update( null, -1, -1, -1, -1, -1, false, false );
				
				m_preset_preview.draw_string( "Presets are frequently used patterns of tiles.\nHere you can create and manage them.\n\n- Press the 'Add' preset button to add a new one.\n\n- Select a preset in the tree view to put it on\na game screen.", 0, 0 );
			}
			else
			{
				if( CheckBoxAddPreset.Checked )
				{
					// redraw a current selected preset
					m_preset_preview.update( null, -1, -1, -1, -1, -1, false, false );
					
					m_preset_preview.draw_string( "Select a rectangle area in the screen editor\nto create a preset.", 0, 0 );
				}
				else
				{
					TreeNode node = TreeViewPresets.SelectedNode;
					
					if( node == null || node.Parent == null )
					{
						m_preset_preview.update( null, -1, -1, -1, -1, -1, false, false );
					
						m_preset_preview.draw_string( "Select a preset.", 0, 0 );
					}
					else
					{
						// draw a preset
						preset_data preset = update_preset_image( node.Name );
						m_preset_preview.update( m_preset_image, m_preset_image.Width, m_preset_image.Height, 0, 0, 1, false, false );
						
						m_preset_preview.draw_string( "Put the <" + node.Name + "> on a game screen.", 0, 0 );
					}
				}
			}
			
			m_preset_preview.invalidate();
		}
		
		private preset_data update_preset_image( string _name )
		{
			preset_data preset = null;
			
			foreach( var key in m_data.presets_data.Keys )
			{ 
				( m_data.presets_data[ key ] as List< preset_data > ).ForEach( delegate( preset_data _preset ) { if( _preset.name == _name ) { preset = _preset; } } );
			}
			
			if( preset != null )
			{
				m_gfx.Clear( Color.FromArgb( 0 ) );

				int scr_tile_size = utils.CONST_SCREEN_TILES_SIZE >> 1;
				
				int start_pos_x = ( m_preset_image.Width >> 1 ) - ( ( preset.width * scr_tile_size ) >> 1 );
				int start_pos_y = ( m_preset_image.Height >> 1 ) - ( ( preset.height * scr_tile_size ) >> 1 );
				
				for( int tile_y = 0; tile_y < preset.height; tile_y++ )
				{
					for( int tile_x = 0; tile_x < preset.width; tile_x++ )
					{
						m_gfx.DrawImage( m_tiles_image_list.Images[ preset.data[ tile_y * preset.width + tile_x ] ], start_pos_x + tile_x * scr_tile_size, start_pos_y + tile_y * scr_tile_size, scr_tile_size, scr_tile_size );
					}
				}
				
				return preset;
			}
			
			MainForm.message_box( "UNEXPECTED ERROR!\n\nCan't find the preset <" + _name + ">!", "Updating Preset Image", MessageBoxButtons.OK, MessageBoxIcon.Error );
			
			return null;
		}
		
		public void set_data( tiles_data _data )
		{
			m_data = _data;
			
			update_tree_view();
			
			update();
		}
		
		private void update_tree_view()
		{
			TreeViewPresets.BeginUpdate();
			{
				TreeViewPresets.Nodes.Clear();
			}
			TreeViewPresets.EndUpdate();
			
			foreach( var key in m_data.presets_data.Keys )
			{ 
				group_add( key, false );
				
				( m_data.presets_data[ key ] as List< preset_data > ).ForEach( delegate( preset_data _preset ) { preset_add( _preset.name, key, _preset.width, _preset.height, _preset.data, false ); } );
			}
		}

		public void subscribe_event( screen_editor _scr_editor )
		{
			_scr_editor.CreatePresetEnd += new EventHandler( create_preset_end );
		}
		
		private void create_preset_end(object sender, EventArgs e)
		{
			enable( true );
			
			CheckBoxAddPreset.Checked = false;

			m_object_name_form.Text = "Add Preset";
			
			m_object_name_form.edit_str = "PRESET";
			
			if( m_object_name_form.ShowWindow() == DialogResult.OK )
			{
				PresetEventArg preset_event = e as PresetEventArg;
				
				preset_add( m_object_name_form.edit_str, TreeViewPresets.SelectedNode.Name, preset_event.width, preset_event.height, preset_event.data, true );
				
				MainForm.set_status_msg( "Added tiles preset <" + m_object_name_form.edit_str + ">" );
				
				update();
			}
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
			
			if( CheckBoxAddPreset.Checked )
			{
				if( CreatePresetCancel != null )
				{
					CreatePresetCancel( this, null );
				}
				
				create_preset_end( null, null );
			}
		}
		
		void BtnGroupAddClick_Event(object sender, EventArgs e)
		{
			m_object_name_form.Text = "Add Group";
			
			m_object_name_form.edit_str = "GROUP";
			
			if( m_object_name_form.ShowWindow() == DialogResult.OK )
			{
				group_add( m_object_name_form.edit_str, true );
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
					MainForm.message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPresets.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void CheckBoxAddPresetClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPresets.SelectedNode;
			
			if( sel_node == null || sel_node.Parent != null )
			{
				MainForm.message_box( "Please, select a group!", "Preset Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				CheckBoxAddPreset.Checked = false;
				
				return;
			}
			
			CheckBoxAddPreset.Checked ^= true; 

			if( CheckBoxAddPreset.Checked )
			{
				if( CreatePresetBegin != null )
				{
					CreatePresetBegin( this, null );
				}
			}
			else
			{
				if( CreatePresetCancel != null )
				{
					CreatePresetCancel( this, null );
				}
				
				create_preset_end( null, null );
			}
			
			enable( !CheckBoxAddPreset.Checked );
			CheckBoxAddPreset.Enabled = true;
			
			update();
		}
		
		void BtnPresetDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPresets.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					if( MainForm.message_box( "Are you sure?", "Delete Preset", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						if( preset_delete( sel_node.Name, sel_node.Parent.Name ) )
						{
							MainForm.set_status_msg( "Deleted tiles preset <" + sel_node.Name + ">" );
							
							update();
						}
					}
				}
				else
				{
					MainForm.message_box( "Please, select a preset!", "Preset Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPresets.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a preset!", "Preset Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Preset Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
				MainForm.message_box( "Please, select an item!", "Item Renaming Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
		
		public bool preset_add( string _preset_name, string _grp_name, byte _width, byte _height, byte[] _data, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPresets.Nodes.Find( _preset_name, true );

			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "A preset with the same name (" + _preset_name +  ") is already exist!", "Preset Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			nodes_arr = TreeViewPresets.Nodes.Find( _grp_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				if( _add_to_data_bank )
				{
					List< preset_data > presets = m_data.presets_data[ _grp_name ];
					
					if( presets != null )
					{
						presets.Add( new preset_data( _preset_name, _width, _height, _data ) );
					}
				}
				
				TreeNode node = new TreeNode( _preset_name );
				node.Name = _preset_name;
				node.ContextMenuStrip = ContextMenuStripPresetItem;
				
				TreeViewPresets.BeginUpdate();
				{
					nodes_arr[ 0 ].Nodes.Add( node );
					
					nodes_arr[ 0 ].Expand();
					
					TreeViewPresets.SelectedNode = node;
				}
				TreeViewPresets.EndUpdate();
				
				return true;
			}
			
			return false;
		}
		
		public bool preset_delete( string _preset_name, string _grp_name )
		{
			TreeNode[] nodes_arr = TreeViewPresets.Nodes.Find( _preset_name, true );
			
			if( nodes_arr.Length > 0 )
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
				}

				TreeNode node = new TreeNode( _preset_name );
				node.Name = _preset_name;
				node.ContextMenuStrip = ContextMenuStripPresetItem;
				
				TreeViewPresets.BeginUpdate();
				{
					nodes_arr[ 0 ].Remove();
				}
				TreeViewPresets.EndUpdate();
				
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
		
		private bool group_add( string _name, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPresets.Nodes.Find( _name, true );
			
			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "An item with the same name (" + _name +  ") is already exist!", "Group Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
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
			
			if( _add_to_data_bank )
			{
				m_data.presets_data.Add( _name, new List< preset_data >() );
			}
			
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
