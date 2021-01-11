/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
	/// Description of patterns_manager_form.
	/// </summary>
	///

	public class PatternEventArg : EventArgs
	{
		private pattern_data m_data = null;
		
		public pattern_data data
		{
			get
			{
				pattern_data data = m_data;
				m_data = null;
				
				return data;
			}
			
			set {}
		}
		
		public PatternEventArg( pattern_data _data )
		{
			m_data = _data;
		}
	}
	
	public partial class patterns_manager_form : Form
	{
		public event EventHandler PatternsManagerClosed;
		public event EventHandler CreatePatternBegin;		
		public event EventHandler ScreenEditorSwitchToBuildMode;
		public event EventHandler EnablePlacePatternMode;
		
		private tiles_data	m_data				= null;
		
		private ImageList 	m_tiles_image_list 	= null;
		
		private image_preview		m_pattern_preview	= null;
		private object_name_form	m_object_name_form	= null;
		
		private Bitmap		m_pattern_image	= null;
		private Graphics	m_gfx			= null;
		
		private int			m_scale_pow		= 0;
		
		public patterns_manager_form( ImageList _tiles_image_list )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_tiles_image_list 	= _tiles_image_list;
			
			m_pattern_preview = new image_preview( PixBoxPreview );
			
			PixBoxPreview.MouseWheel += new MouseEventHandler( PatternsMngr_MouseWheel );
			PixBoxPreview.MouseEnter += new EventHandler( PatternsMngr_MouseEnter );
			PixBoxPreview.MouseLeave += new EventHandler( PatternsMngr_MouseLeave );
			
			m_object_name_form = new object_name_form();
			
			// create graphics for drawing patterns
			{
				int scr_tile_size = utils.CONST_SCREEN_TILES_SIZE >> 1;
				
				m_pattern_image = new Bitmap( scr_tile_size * utils.CONST_SCREEN_NUM_WIDTH_TILES, scr_tile_size * utils.CONST_SCREEN_NUM_HEIGHT_TILES, PixelFormat.Format32bppArgb );
				m_gfx = Graphics.FromImage( m_pattern_image );
			}
		}
		
		public void reset()
		{
			m_scale_pow = 0;
			
			set_data( null );
		}
		
		public void enable( bool _on )
		{
			TreeViewPatterns.Enabled		= _on;
			
			BtnGroupAdd.Enabled		= _on;
			BtnGroupDelete.Enabled	= _on;
			
			CheckBoxAddPattern.Enabled	= _on;
			BtnPatternDelete.Enabled 	= _on;
			
			BtnRename.Enabled		= _on;
			
			BtnResetPattern.Enabled	= _on;
		}
		
		private void PatternsMngr_MouseWheel(object sender, MouseEventArgs e)
		{
			if( TreeViewPatterns.Enabled )
			{
				m_scale_pow += Math.Sign( e.Delta );
				
				m_scale_pow = m_scale_pow < 0 ? 0:m_scale_pow;
				m_scale_pow = m_scale_pow > 3 ? 3:m_scale_pow;
				
				update();
			}
		}
		
		private void PatternsMngr_MouseEnter(object sender, EventArgs e)
		{
			if( TreeViewPatterns.Enabled )
			{
				m_pattern_preview.set_focus();
			}
		}

		private void PatternsMngr_MouseLeave(object sender, EventArgs e)
		{
			if( TreeViewPatterns.Enabled )
			{
				TreeViewPatterns.Focus();
			}
		}
		
		public void update()
		{
			if( this.Visible )
			{
				if( TreeViewPatterns.SelectedNode == null )
				{
					// redraw a current selected pattern
					m_pattern_preview.update( null, -1, -1, -1, -1, -1, false, false );
					
					m_pattern_preview.draw_string( "Patterns are often-used combinations of tiles.\nHere you can create and manage them.\n\n- Click the 'Add' pattern button to add a new one.\n\n- Click the 'Add' button again to cancel the operation.\n\n- Select a pattern in the tree view to put it on\na game screen.\n\n- Scale a selected pattern using a mouse wheel.", 0, 0 );
				}
				else
				{
					if( CheckBoxAddPattern.Checked )
					{
						// redraw a current selected pattern
						m_pattern_preview.update( null, -1, -1, -1, -1, -1, false, false );
						
						m_pattern_preview.draw_string( "Select a rectangle area in the screen editor\nto create a pattern.", 0, 0 );
					}
					else
					{
						TreeNode node = TreeViewPatterns.SelectedNode;
						
						if( node == null || node.Parent == null )
						{
							m_pattern_preview.update( null, -1, -1, -1, -1, -1, false, false );
						
							m_pattern_preview.draw_string( "Select a pattern.", 0, 0 );
						}
						else
						{
							// draw a pattern
							update_pattern_image( node.Name );
							m_pattern_preview.update( m_pattern_image, m_pattern_image.Width, m_pattern_image.Height, 0, 0, ( int )Math.Pow( 2.0, ( double )m_scale_pow ), false, false );
							
							m_pattern_preview.draw_string( "Put the <" + node.Name + "> on a game screen.", 0, 0 );
						}
					}
				}
				
				m_pattern_preview.invalidate();
			}
		}
		
		private pattern_data update_pattern_image( string _name )
		{
			pattern_data pattern = m_data.get_pattern_by_name( _name );
			
			if( pattern != null )
			{
				m_gfx.Clear( Color.FromArgb( 0 ) );

				int scr_tile_size = utils.CONST_SCREEN_TILES_SIZE >> 1;
				
				int start_pos_x = ( m_pattern_image.Width >> 1 ) - ( ( pattern.width * scr_tile_size ) >> 1 );
				int start_pos_y = ( m_pattern_image.Height >> 1 ) - ( ( pattern.height * scr_tile_size ) >> 1 );
				
				for( int tile_y = 0; tile_y < pattern.height; tile_y++ )
				{
					for( int tile_x = 0; tile_x < pattern.width; tile_x++ )
					{
						m_gfx.DrawImage( m_tiles_image_list.Images[ pattern.data[ tile_y * pattern.width + tile_x ] ], start_pos_x + tile_x * scr_tile_size, start_pos_y + tile_y * scr_tile_size, scr_tile_size, scr_tile_size );
					}
				}
				
				return pattern;
			}
			
			MainForm.message_box( "UNEXPECTED ERROR!\n\nCan't find the pattern <" + _name + ">!", "Updating Pattern Image", MessageBoxButtons.OK, MessageBoxIcon.Error );
			
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
			TreeViewPatterns.BeginUpdate();
			{
				TreeViewPatterns.Nodes.Clear();
			}
			TreeViewPatterns.EndUpdate();
			
			if( m_data != null )
			{
				foreach( var key in m_data.patterns_data.Keys )
				{ 
					group_add( key, false );
					
					( m_data.patterns_data[ key ] as List< pattern_data > ).ForEach( delegate( pattern_data _pattern ) { pattern_add( key, _pattern, false ); } );
				}
			}
			
			// on Linux TreeViewPatterns.SelectedNode is not reset when clearing nodes
			TreeViewPatterns.SelectedNode = null; 
		}

		public void subscribe_event( screen_editor _scr_editor )
		{
			_scr_editor.CreatePatternEnd += new EventHandler( create_pattern_end );
		}
		
		private void create_pattern_end(object sender, EventArgs e)
		{
			enable( true );
			
			CheckBoxAddPattern.Checked = false;
			
			if( sender != this )
			{
				m_object_name_form.Text = "Add Pattern";
				
				m_object_name_form.edit_str = "PATTERN";
				
				if( m_object_name_form.ShowWindow() == DialogResult.OK )
				{
					PatternEventArg pattern_event = e as PatternEventArg;
					
					pattern_data data = pattern_event.data;
					data.name = m_object_name_form.edit_str;
					
					pattern_add( TreeViewPatterns.SelectedNode.Name, data, true );
					
					MainForm.set_status_msg( "Added tiles pattern <" + m_object_name_form.edit_str + ">" );
					
					update();
					
					Focus();
				}
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
			
			if( ScreenEditorSwitchToBuildMode != null )
			{
				ScreenEditorSwitchToBuildMode( this, null );
			}
			
			if( CheckBoxAddPattern.Checked )
			{
				create_pattern_end( this, null );
			}
			
			if( PatternsManagerClosed != null )
			{
				PatternsManagerClosed( this, null );
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
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
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
				if( TreeViewPatterns.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a group!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Group Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void CheckBoxAddPatternClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node == null || sel_node.Parent != null )
			{
				MainForm.message_box( "Please, select a group!", "Pattern Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				CheckBoxAddPattern.Checked = false;
				
				return;
			}
			
			CheckBoxAddPattern.Checked ^= true; 

			if( CheckBoxAddPattern.Checked )
			{
				if( CreatePatternBegin != null )
				{
					CreatePatternBegin( this, null );
				}
			}
			else
			{
				if( ScreenEditorSwitchToBuildMode != null )
				{
					ScreenEditorSwitchToBuildMode( this, null );
				}
				
				create_pattern_end( this, null );
			}
			
			enable( !CheckBoxAddPattern.Checked );
			CheckBoxAddPattern.Enabled = true;
			
			update();
		}
		
		void BtnPatternDeleteClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
			if( sel_node != null )
			{
				if( sel_node.Parent != null )
				{
					if( MainForm.message_box( "Are you sure?", "Delete Pattern", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						if( pattern_delete( sel_node.Name, sel_node.Parent.Name ) )
						{
							MainForm.set_status_msg( "Deleted tiles pattern <" + sel_node.Name + ">" );
							
							update();
						}
					}
				}
				else
				{
					MainForm.message_box( "Please, select a pattern!", "Pattern Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				if( TreeViewPatterns.Nodes.Count > 0 )
				{
					MainForm.message_box( "Please, select a pattern!", "Pattern Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					MainForm.message_box( "No data!", "Pattern Deletion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		void BtnRenameClick_Event(object sender, EventArgs e)
		{
			TreeNode sel_node = TreeViewPatterns.SelectedNode;
			
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
					pattern_rename( e.Node.Parent.Text, e.Node.Text, e.Label );
					e.Node.Name = e.Label;
					
					update();
				}
			}
		}
		
		void TreeViewNodeSelect_Event(object sender, TreeViewEventArgs e)
		{
			if( TreeViewPatterns.SelectedNode.Parent == null )
			{
				if( ScreenEditorSwitchToBuildMode != null )
				{
					ScreenEditorSwitchToBuildMode( this, null );
				}
			}
			else
			{
				if( EnablePlacePatternMode != null )
				{
					EnablePlacePatternMode( this, new PatternEventArg( m_data.get_pattern_by_name( TreeViewPatterns.SelectedNode.Name ) ) );
				}
			}
			
			update();
		}
		
		public bool pattern_rename( string _grp_name, string _old_name, string _new_name )
		{
			List< pattern_data > patterns = m_data.patterns_data[ _grp_name ];
			
			if( patterns != null )
			{
				patterns.ForEach( delegate( pattern_data _pattern ) { if( _pattern.name == _old_name ) { _pattern.name = _new_name; return; } } );
				
				return true;
			}
			
			return false;
		}
		
		public bool pattern_add( string _grp_name, pattern_data _pattern, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _pattern.name, true );

			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "A pattern with the same name (" + _pattern.name +  ") is already exist!", "Pattern Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			nodes_arr = TreeViewPatterns.Nodes.Find( _grp_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				if( _add_to_data_bank )
				{
					List< pattern_data > patterns = m_data.patterns_data[ _grp_name ];
					
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
		
		public bool pattern_delete( string _pattern_name, string _grp_name )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _pattern_name, true );
			
			if( nodes_arr.Length > 0 )
			{
				List< pattern_data > patterns = m_data.patterns_data[ _grp_name ];
				
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
			
		private bool group_rename( string _old_name, string _new_name )
		{
			if( m_data.patterns_data.ContainsKey( _old_name ) )
			{
				List< pattern_data > patterns = m_data.patterns_data[ _old_name ];
	
				m_data.patterns_data.Remove( _old_name );
				m_data.patterns_data.Add( _new_name, patterns );
					
				return true;
			}
			
			return false;
		}
		
		private bool group_add( string _name, bool _add_to_data_bank )
		{
			TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _name, true );
			
			if( nodes_arr.Length > 0 )
			{
				MainForm.message_box( "An item with the same name (" + _name +  ") is already exist!", "Group Adding Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
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
				m_data.patterns_data.Add( _name, new List< pattern_data >() );
			}
			
			return true;
		}
		
		public bool group_delete( string _name )
		{
			if( TreeViewPatterns.Nodes.ContainsKey( _name ) )
			{
				TreeNode[] nodes_arr = TreeViewPatterns.Nodes.Find( _name, true );
				
				if( nodes_arr.Length > 0 )
				{
					if( nodes_arr[ 0 ].FirstNode != null )
					{
						if( MainForm.message_box( "The selected group is not empty!\n\nRemove all child patterns?", "Delete Group", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
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
					
					if( m_data.patterns_data.ContainsKey( _name ) )
					{
						List< pattern_data > patterns = m_data.patterns_data[ _name ];

						m_data.patterns_data.Remove( _name );
						
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
		
		public void visible( bool _on )
		{
			this.Visible = _on;
			
			if( _on )
			{
				TreeViewPatterns.SelectedNode = null;
				
				update();
			}
		}
		
		void BtnResetPatternClick_Event(object sender, EventArgs e)
		{
			TreeViewPatterns.SelectedNode = TreeViewPatterns.TopNode;
		}
	}
}
