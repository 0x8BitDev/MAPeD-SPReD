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
		private int create_screen()
		{
			int scr_glob_ind	= -1;
			int scr_local_ind;
			
			if( ( scr_local_ind = m_data_manager.screen_data_create() ) >= 0 )
			{
				if( ( scr_glob_ind = insert_screen_into_layouts( scr_local_ind ) ) < 0 )
				{
					m_data_manager.screen_data_delete( scr_local_ind );
					
					message_box( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT, "Failed to Create Screen", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}
			
			return scr_glob_ind;
		}

		private void delete_screen( layout_screen_data _scr_data )
		{
			if( _scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
			{
				int bank_ind = m_data_manager.get_bank_ind_by_global_screen_ind( _scr_data.m_scr_ind );
				
				delete_screen_by_bank_id( bank_ind, m_data_manager.get_local_screen_ind( bank_ind, _scr_data.m_scr_ind ) );
			}
		}
		
		private void delete_screen( int _scr_local_ind )
		{
			m_data_manager.screen_data_delete( _scr_local_ind );

			m_data_manager.remove_screen_from_layouts( CBoxCHRBanks.SelectedIndex, _scr_local_ind  );
			
			if( m_imagelist_manager.remove_screen( CBoxCHRBanks.SelectedIndex, _scr_local_ind ) )
			{
				m_layout_editor.set_param( layout_editor_base.e_mode.Screens, layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
				
				update_screens_labels_by_bank_id();
			}
		}

		private void delete_screen_by_bank_id( int _bank_ind, int _scr_local_ind )
		{
			bool all_banks_screens = CheckBoxLayoutEditorAllBanks.Checked;
			
			// unlock all the screens
			CheckBoxLayoutEditorAllBanks.Checked = true;
			
			if( m_imagelist_manager.remove_screen( _bank_ind, _scr_local_ind ) )
			{
				m_layout_editor.set_param( layout_editor_base.e_mode.Screens, layout_editor_param.CONST_SET_SCR_ACTIVE, -1 );
			}
			
			m_data_manager.get_tiles_data( _bank_ind ).delete_screen( _scr_local_ind );
			
			m_data_manager.remove_screen_from_layouts( _bank_ind, _scr_local_ind );
			
			update_screens_labels_by_bank_id();
			
			CheckBoxLayoutEditorAllBanks.Checked = all_banks_screens;
		}

		private bool check_empty_screen( ulong[] _tiles, screen_data _scr_data )
		{
			int tile_n;

			if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				int block_n;
				int tile_offs_x;
				int tile_offs_y;
				
				int half_tile_x = platform_data.get_half_tile_x();
				int half_tile_y = platform_data.get_half_tile_y();
				
				ulong	tile_ind		= _tiles[ _scr_data.get_tile( 0 ) ];
				ushort	first_block_ind	= utils.get_ushort_from_ulong( tile_ind, 0 );
				
				for( tile_n = 0; tile_n < _scr_data.m_arr.Length; tile_n++ )
				{
					tile_ind	= _tiles[ _scr_data.get_tile( tile_n ) ];
					
					tile_offs_x = ( tile_n % platform_data.get_screen_tiles_width() );
					tile_offs_y = ( tile_n / platform_data.get_screen_tiles_width() );

					for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
					{
						if( ( half_tile_x == tile_offs_x ) && ( block_n & 0x01 ) != 0 )
						{
							continue;
						}
						
						if( ( half_tile_y == tile_offs_y ) && ( block_n & 0x02 ) != 0 )
						{
							continue;
						}
						
						if( utils.get_ushort_from_ulong( tile_ind, block_n ) != first_block_ind )
						{
							return false;
						}
					}
				}
				
				return true;
			}
			else
			{
				int scr_first_tile_ind = _scr_data.get_tile( 0 );
				int num_tiles = platform_data.get_screen_tiles_cnt_uni( m_data_manager.screen_data_type );
				
				for( tile_n = 1; tile_n < num_tiles; tile_n++ )
				{
					if( scr_first_tile_ind != _scr_data.get_tile( tile_n ) )
					{
						break;
					}
				}
				
				if( tile_n == num_tiles )
				{
					return true;
				}
			}

			return false;
		}
		
		private int delete_empty_screens()
		{
			int res = 0;

			tiles_data data	= get_curr_tiles_data();

			int scr_n;
			
			for( scr_n = 0; scr_n < m_data_manager.scr_data_cnt; scr_n++ )
			{
				if( check_empty_screen( data.tiles, data.get_screen_data( scr_n ) ) )
				{
					delete_screen( scr_n );

					--scr_n;
					
					++res;
				}
			}
			
			return res;
		}

		private int layout_delete_empty_screens()
		{
			int res = 0;
			int bank_ind;

			bool all_banks_screens = CheckBoxLayoutEditorAllBanks.Checked;
			
			// unlock all the screens
			CheckBoxLayoutEditorAllBanks.Checked = true;
			
			m_data_manager.get_layout_data( m_data_manager.layouts_data_pos ).layout_data_proc( delegate( layout_screen_data _scr_data )
			{
				if( _scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					bank_ind = m_data_manager.get_bank_ind_by_global_screen_ind( _scr_data.m_scr_ind );
					
					tiles_data data	= m_data_manager.get_tiles_data( bank_ind );
	
					if( check_empty_screen( data.tiles, data.get_screen_data( m_data_manager.get_local_screen_ind( bank_ind, _scr_data.m_scr_ind ) ) ) )
					{
						delete_screen_by_bank_id( bank_ind, m_data_manager.get_local_screen_ind( bank_ind, _scr_data.m_scr_ind ) );
	
						++res;
					}
				}
			});
			
			CheckBoxLayoutEditorAllBanks.Checked = all_banks_screens;
			
			return res;
		}

		private void delete_last_layout_and_screens()
		{
			layout_screen_data lt_scr_data;
			
			List< int > scr_inds_data = new List< int >( m_data_manager.scr_data_cnt );
			
			layout_data layout = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
			
			for( int y = 0; y < layout.get_height(); y++ )
			{
				for( int x = 0; x < layout.get_width(); x++ )
				{
					lt_scr_data = layout.get_data( x, y );
					
					if( lt_scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_inds_data.Add( lt_scr_data.m_scr_ind );
					}
				}
			}
			
			if( scr_inds_data.Count > 0 )
			{
				scr_inds_data.Sort();
				scr_inds_data.Reverse();
				
				foreach( int scr_n in scr_inds_data )
				{
					delete_screen( m_data_manager.get_local_screen_ind( m_data_manager.tiles_data_pos, scr_n ) );
				}
			}
			
			m_data_manager.layout_data_delete( false );
		}
		
		private layout_data create_layout_with_empty_screens_beg( int _scr_width, int _scr_height )
		{
			if( m_data_manager.layout_data_create() == true )
			{
				layout_data layout = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				// create a level layout
				{
					for( int i = 0; i < _scr_width - 1; i++ )
					{
						layout.add_right();
					}
					
					for( int i = 0; i < _scr_height - 1; i++ )
					{
						layout.add_down();
					}
				}
				
				// create screens and fill the layout
				{
					int scr_global_ind;
					int scr_local_ind;
					
					layout_screen_data scr_data;
					
					for( int y = 0; y < _scr_height; y++ )
					{
						for( int x = 0; x < _scr_width; x++ )
						{
							scr_local_ind = m_data_manager.screen_data_create();
							
							scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, scr_local_ind );
							
							if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
							{
								m_data_manager.insert_screen_into_layouts( scr_global_ind );

								scr_data = layout.get_data( x, y );
								scr_data.m_scr_ind = scr_global_ind;
								layout.set_data( scr_data, x, y );
							}
							else
							{
								// delete the last screen manually, cause it wasn't added to the layout
								m_data_manager.screen_data_delete( scr_local_ind );
								
								// clear all created screens and layout
								delete_last_layout_and_screens();
								
								throw new Exception( "Can't create screen!\nThe maximum allowed number of screens - " + utils.CONST_SCREEN_MAX_CNT );
							}
						}
					}
				}
				
				return layout;
			}
			
			return null;
		}

		private bool create_layout_with_empty_screens_end( layout_data _data )
		{
			if( _data != null )
			{
				// create screens and fill the layout
				{
					int scr_local_ind;
					int scr_global_ind;
					
					for( int y = 0; y < _data.get_height(); y++ )
					{
						for( int x = 0; x < _data.get_width(); x++ )
						{
							scr_global_ind = _data.get_data( x, y ).m_scr_ind;
							scr_local_ind = m_data_manager.get_local_screen_ind( m_data_manager.tiles_data_pos, scr_global_ind );
							
							m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
						}
					}
				}
				
				m_data_manager.layouts_data_pos = m_data_manager.layouts_data_cnt - 1;
				
				ListBoxLayouts.Items.Add( m_data_manager.layouts_data_pos );
				ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
				
				palette_group.Instance.set_palette( get_curr_tiles_data() );
				
				return true;
			}
			
			return false;
		}
		
		private int insert_screen_into_layouts( int _scr_local_ind )
		{
			int scr_global_ind = m_data_manager.get_global_screen_ind( m_data_manager.tiles_data_pos, _scr_local_ind );
			
			if( scr_global_ind < utils.CONST_SCREEN_MAX_CNT )
			{
				m_imagelist_manager.insert_screen( CheckBoxLayoutEditorAllBanks.Checked, m_data_manager.tiles_data_pos, _scr_local_ind, scr_global_ind, m_data_manager.get_tiles_data(), m_data_manager.screen_data_type, m_view_type, PropertyPerBlockToolStripMenuItem.Checked );
				
				palette_group.Instance.set_palette( get_curr_tiles_data() );
				
				m_data_manager.insert_screen_into_layouts( scr_global_ind );
				
				return scr_global_ind;
			}
			
			return -1;
		}

		private void delete_layout_row_column( Func< layout_data, bool > _condition, Func< layout_data, bool, bool > _act, string _caption_msg )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null && _condition( data ) )
				{
					bool delete_scr_data = false;
					
					switch( message_box( "Delete the screens data?\n\n[YES] The screens data will be permanently deleted\n[NO] Delete the screen cells only", _caption_msg, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question ) )
					{
						case DialogResult.Yes:
							{
								delete_scr_data = true;
							}
							break;
							
						case DialogResult.Cancel:
							{
								return;
							}
					}
					
					if( _act( data, delete_scr_data ) )
					{
						m_layout_editor.update_dimension_changes();
					}
				}
			}
		}
		
		private void update_layouts_list_box()
		{
			ListBoxLayouts.Items.Clear();
			
			int size = m_data_manager.layouts_data_cnt;
			
			for( int i = 0; i < size; i++ )
			{
				ListBoxLayouts.Items.Add( i );
			}
		}

		private void BtnLayoutDeleteEmptyScreensClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 && message_box( "Delete all one-block filled screens?", "Clean Up", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				int deleted_screens_cnt;
				
				progress_bar_show( true, "Deleting empty screens...", false );
				
				if( ( deleted_screens_cnt = layout_delete_empty_screens() ) > 0 )
				{
					m_layout_editor.update_dimension_changes();
					
					set_status_msg( "Clean up: " + deleted_screens_cnt + " screens deleted" );
				}
				else
				{
					set_status_msg( "Clean up: no empty screens found" );
				}
				
				progress_bar_show( false );
			}
		}

		private void BtnLayoutAddUpRowClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_up();
					
					for( int cell_n = 0; cell_n < data.get_width(); cell_n++ )
					{
						data.get_data( cell_n, 0 ).m_scr_ind = create_screen();
					}
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		private void BtnLayoutRemoveTopRowClick( object sender, EventArgs e )
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_width(); cell_n++ )
					{
						delete_screen( _data.get_data( cell_n, 0 ) );
					}
				}
				
				return _data.remove_up();
			}, "Remove Top Row" );
		}
		
		private void BtnLayoutAddDownRowClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_down();

					for( int cell_n = 0; cell_n < data.get_width(); cell_n++ )
					{
						data.get_data( cell_n, data.get_height() - 1 ).m_scr_ind = create_screen();
					}
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		private void BtnLayoutRemoveBottomRowClick( object sender, EventArgs e )
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_height() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_width(); cell_n++ )
					{
						delete_screen( _data.get_data( cell_n, _data.get_height() - 1 ) );
					}
				}
				
				return _data.remove_down();
			}, "Remove Bottom Row" );
		}
		
		private void BtnLayoutAddLeftColumnClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_left();
					
					for( int cell_n = 0; cell_n < data.get_height(); cell_n++ )
					{
						data.get_data( 0, cell_n ).m_scr_ind = create_screen();
					}

					m_layout_editor.update_dimension_changes();
				}
			}
		}
		
		private void BtnLayoutRemoveLeftColumnClick( object sender, EventArgs e )
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_height(); cell_n++ )
					{
						delete_screen( _data.get_data( 0, cell_n ) );
					}
				}
				
				return _data.remove_left();
			}, "Remove Left Column" );
		}
		
		private void BtnLayoutAddRightColumnClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 )
			{
				layout_data data = m_data_manager.get_layout_data( m_data_manager.layouts_data_pos );
				
				if( data != null )
				{
					data.add_right();

					for( int cell_n = 0; cell_n < data.get_height(); cell_n++ )
					{
						data.get_data( data.get_width() - 1, cell_n ).m_scr_ind = create_screen();
					}
					
					m_layout_editor.update_dimension_changes();
				}
			}
		}
	
		private void BtnLayoutRemoveRightColumnClick( object sender, EventArgs e )
		{
			delete_layout_row_column( delegate( layout_data _data ) { return _data.get_width() > 1; }, delegate( layout_data _data, bool _delete_scr_data )
			{
				if( _delete_scr_data )
				{
					for( int cell_n = 0; cell_n < _data.get_height(); cell_n++ )
					{
						delete_screen( _data.get_data( _data.get_width() - 1, cell_n ) );
					}
				}
				
				return _data.remove_right();
			}, "Remove Right Column" );
		}

		private void BtnCreateLayoutWxHClick( object sender, EventArgs e )
		{
			try
			{
				if( m_data_manager.tiles_data_pos >= 0 && m_create_layout_form.ShowDialog() == DialogResult.OK )
				{
					m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_cnt - 1;
					
					if( create_layout_with_empty_screens_end( create_layout_with_empty_screens_beg( m_create_layout_form.layout_width, m_create_layout_form.layout_height ) ) != false )
					{
						m_layout_editor.update_dimension_changes();
							
						set_status_msg( "Added layout " + m_create_layout_form.layout_width + "x" + m_create_layout_form.layout_height );
					}
					else
					{
						throw new Exception( "Can't create layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT );
					}
				}
			}
			catch( Exception _err )
			{
				set_status_msg( "Failed to create layout or screen" );
				
				message_box( _err.Message, "Failed to Create Layout or Screen", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnDeleteLayoutClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 && ListBoxLayouts.Items.Count > 0 && message_box( "Are you sure?", "Delete Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				m_data_manager.layout_data_delete();

				update_layouts_list_box();
				
				ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
				
				set_status_msg( "Layout removed" );
			}
		}
		
		private void BtnCopyLayoutClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.SelectedIndex >= 0 && ListBoxLayouts.Items.Count > 0 && message_box( "Are you sure?", "Copy Layout", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				if( m_data_manager.layout_data_copy() == true )
				{
					update_layouts_list_box();
					
					m_data_manager.layouts_data_pos = ListBoxLayouts.SelectedIndex = m_data_manager.layouts_data_pos;
					
					set_status_msg( "Layout copied" );
				}
				else
				{
					set_status_msg( "Failed to copy layout" );
					
					message_box( "Can't copy layout!\nThe maximum allowed number of layouts - " + utils.CONST_LAYOUT_MAX_CNT, "Failed to Copy Layout", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		private void ListBoxLayoutsClick( object sender, EventArgs e )
		{
			m_data_manager.layouts_data_pos = ( sender as ListBox ).SelectedIndex;
		}
		
		private void BtnLayoutMoveDownClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.Items.Count > 1 )
			{
				if( ListBoxLayouts.SelectedIndex != -1 )
				{
					if( ListBoxLayouts.SelectedIndex >= 0 && ( ListBoxLayouts.SelectedIndex + 1 ) < ListBoxLayouts.Items.Count )
					{
						m_data_manager.layout_swap( ListBoxLayouts.SelectedIndex, ListBoxLayouts.SelectedIndex + 1 );
						
						++ListBoxLayouts.SelectedIndex;
					}
				}
				else
				{
					message_box( "Please, select a layout!", "Move Layout Down", MessageBoxButtons.OK );
				}
			}
		}
		
		private void BtnLayoutMoveUpClick( object sender, EventArgs e )
		{
			if( ListBoxLayouts.Items.Count > 1 )
			{
				if( ListBoxLayouts.SelectedIndex != -1 )
				{
					if( ListBoxLayouts.SelectedIndex > 0 )
					{
						m_data_manager.layout_swap( ListBoxLayouts.SelectedIndex, ListBoxLayouts.SelectedIndex - 1 );
						
						--ListBoxLayouts.SelectedIndex;
					}
				}
				else
				{
					message_box( "Please, select a layout!", "Move Layout Up", MessageBoxButtons.OK );
				}
			}
		}
		
		private void LayoutShowMarksToolStripMenuItemClick( object sender, EventArgs e )
		{
			LayoutShowMarksToolStripMenuItem.Checked = CheckBoxShowMarks.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		private void LayoutShowEntitiesToolStripMenuItemClick( object sender, EventArgs e )
		{
			LayoutShowEntitiesToolStripMenuItem.Checked = CheckBoxShowEntities.Checked = !( sender as ToolStripMenuItem ).Checked;
			
			LayoutShowTargetsToolStripMenuItem.Enabled = LayoutShowCoordsToolStripMenuItem.Enabled = LayoutShowEntitiesToolStripMenuItem.Checked; 
		}
		
		private void LayoutShowTargetsToolStripMenuItemClick( object sender, EventArgs e )
		{
			LayoutShowTargetsToolStripMenuItem.Checked = CheckBoxShowTargets.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		private void LayoutShowCoordsToolStripMenuItemClick( object sender, EventArgs e )
		{
			LayoutShowCoordsToolStripMenuItem.Checked = CheckBoxShowCoords.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		private void LayoutShowGridToolStripMenuItemClick( object sender, EventArgs e )
		{
			LayoutShowGridToolStripMenuItem.Checked = CheckBoxShowGrid.Checked = !( sender as ToolStripMenuItem ).Checked;
		}
		
		private void CheckBoxShowMarksChecked( object sender, EventArgs e )
		{
			bool show_marks = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.layout_show_marks = m_layout_editor.show_marks = LayoutShowMarksToolStripMenuItem.Checked = show_marks;
			
			MAPeD.Properties.Settings.Default.Save();
		}
		
		private void CheckBoxShowEntitiesChecked( object sender, EventArgs e )
		{
			bool show_ent = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.layout_show_entities = m_layout_editor.show_entities = LayoutShowEntitiesToolStripMenuItem.Checked = show_ent;
			
			MAPeD.Properties.Settings.Default.Save();
			
			LayoutShowTargetsToolStripMenuItem.Enabled = LayoutShowCoordsToolStripMenuItem.Enabled = CheckBoxShowTargets.Enabled = CheckBoxShowCoords.Enabled = show_ent; 
		}
		
		private void CheckBoxShowTargetsChecked( object sender, EventArgs e )
		{
			MAPeD.Properties.Settings.Default.layout_show_targets = m_layout_editor.show_targets = LayoutShowTargetsToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.Save();
		}
		
		private void CheckBoxShowCoordsChecked( object sender, EventArgs e )
		{
			MAPeD.Properties.Settings.Default.layout_show_coords = m_layout_editor.show_coords = LayoutShowCoordsToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.Save();
		}
		
		private void CheckBoxShowGridChecked( object sender, EventArgs e )
		{
			MAPeD.Properties.Settings.Default.layout_show_grid = m_layout_editor.show_grid = LayoutShowGridToolStripMenuItem.Checked = ( sender as CheckBox ).Checked;
			
			MAPeD.Properties.Settings.Default.Save();
		}
	}
}
