/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 15:46
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SPReD
{
	partial class MainForm
	{
		private bool check_duplicate( String _spr_name )
		{
			int size = SpriteList.Items.Count;
			
			for( int i = 0; i < size; i++ )
			{
				if( SpriteList.Items[ i ].ToString() == _spr_name )
				{
					return true;
				}
			}
			
			return false;
		}
		
		private void update_selected_sprite( bool _new_sprite = false )
		{
			if( SpriteList.SelectedIndex >= 0 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndex ] as sprite_data;
				
				OffsetX.Value = spr.offset_x;
				OffsetY.Value = spr.offset_y;
				
				m_sprites_proc.update_sprite( spr, _new_sprite );
			}
		}
		
		private void BtnRenameClick( object sender, EventArgs e )
		{
			rename_copy_sprite( "Rename Sprite", null );
		}
		
		private void rename_copy_sprite( string _wnd_caption, Action< string, sprite_data > _copy_act )
		{
			if( SpriteList.SelectedIndices.Count == 0 )
			{
				message_box( "Please, select a sprite!", _wnd_caption, MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				m_rename_sprite_form.edit_str = ( SpriteList.SelectedItem as sprite_data ).name;
				
				m_rename_sprite_form.Text = _wnd_caption;
				
				if( m_rename_sprite_form.ShowDialog() == DialogResult.Cancel )
				{
					return;
				}
				
				if( m_rename_sprite_form.edit_str == "" )
				{
					message_box( "The sprite name is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
				
				string new_sprite_name = m_rename_sprite_form.edit_str;
				
				sprite_data spr;
				
				if(!check_duplicate( new_sprite_name ) )
				{
					if( _copy_act != null )
					{
						spr = SpriteList.SelectedItem as sprite_data;
						
						_copy_act( new_sprite_name, spr );
						
						m_sprites_proc.rearrange_CHR_data_ids();
						
						m_sprites_proc.update_sprite( spr, false );
					}
					else
					{
						SpriteList.BeginUpdate();
						{
							spr = SpriteList.SelectedItem as sprite_data;
							spr.name = new_sprite_name;
							
							SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] = spr;
						}
						SpriteList.EndUpdate();
					}
				}
				else
				{
					message_box( new_sprite_name + " - A sprite with the same name already exists! Ignored!", _wnd_caption, MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				message_box( "Please, select one sprite!", _wnd_caption, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnCreateRefClick( object sender, System.EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( SpriteList.SelectedIndices.Count == 1 )
				{
					rename_copy_sprite( "Create Ref", delegate( string _name, sprite_data _spr )
	                {
						SpriteList.Items.Add( _spr.copy( _name, null, null ) );
	                } );
				}
				else
				{
					m_new_sprite_name_form.Text = "Create Ref";
					
					sprite_names_processing( add_pref_postf_func, delegate( string _name, sprite_data _spr, int _ind )
	                {
						SpriteList.Items.Add( _spr.copy( _name, null, null ) );
	                });
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Create Ref", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void BtnCreateCopyClick( object sender, EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( SpriteList.SelectedIndices.Count == 1 )
				{
					rename_copy_sprite( "Create Copy", delegate( string _name, sprite_data _spr )
	                {
						SpriteList.Items.Add( _spr.copy( _name, 
						                                 m_sprites_proc.extract_and_create_CHR_data_group( _spr, CBoxMode8x16.Checked ), 
														 m_sprites_proc.extract_and_create_CHR_data_attr( _spr, CBoxMode8x16.Checked ) ) );
	                });
				}
				else
				{
					m_new_sprite_name_form.Text = "Create Copy";
					
					sprite_names_processing( add_pref_postf_func, delegate( string _name, sprite_data _spr, int _ind )
	                {
						SpriteList.Items.Add( _spr.copy( _name, 
						                                 m_sprites_proc.extract_and_create_CHR_data_group( _spr, CBoxMode8x16.Checked ), 
														 m_sprites_proc.extract_and_create_CHR_data_attr( _spr, CBoxMode8x16.Checked ) ) );
	                });
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Create Copy", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}		

		private void BtnAddPrefixPostfixClick( object sender, EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				m_new_sprite_name_form.Text = "Add Prefix\\Postfix";
				
				SpriteList.BeginUpdate();
				{
					sprite_names_processing( add_pref_postf_func, delegate( string _name, sprite_data _spr, int _spr_ind ) 
                    { 
						_spr.name = _name; 
						SpriteList.Items[ _spr_ind ] = _spr;
					});
				}
				SpriteList.EndUpdate();
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Add Prefix\\Postfix", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnRemovePrefixPostfixClick( object sender, EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				m_new_sprite_name_form.Text = "Remove Prefix\\Postfix";
				
				SpriteList.BeginUpdate();
				{
					sprite_names_processing( remove_pref_postf_func, delegate( string _name, sprite_data _spr, int _spr_ind ) 
					{
						_spr.name = _name; 
						SpriteList.Items[ _spr_ind ] = _spr;
					});
				}
				SpriteList.EndUpdate();
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Remove Prefix\\Postfix", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void sprite_names_processing( add_remove_pref_postf_func _pref_postf_func, Action< string, sprite_data, int > _act )
		{
			if( m_new_sprite_name_form.ShowDialog() == DialogResult.Cancel )
			{
				return;
			}
			
			if( m_new_sprite_name_form.edit_str == "" )
			{
				message_box( "The Prefix\\Postfix field is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			
			string new_name;
			string pref_postf = m_new_sprite_name_form.edit_str;
			
			bool postfix = m_new_sprite_name_form.is_postfix_selected();
			bool name_changed = false;
			
			sprite_data spr;
			
			int[] sel_inds = new int[ SpriteList.SelectedIndices.Count ];
			
			SpriteList.SelectedIndices.CopyTo( sel_inds, 0 );
			
			for( int i = 0; i < sel_inds.Length; i++ )
			{
				spr = SpriteList.Items[ sel_inds[ i ] ] as sprite_data;
				
				new_name = _pref_postf_func( postfix, pref_postf, spr, ref name_changed );
				
				if( !check_duplicate( new_name ) )
				{
					_act( new_name, spr, sel_inds[ i ] );
				}
				else
				{
					if( name_changed )
					{
						message_box( new_name + " - A sprite with the same name already exists! Ignored!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
			
			m_sprites_proc.rearrange_CHR_data_ids();
			
			m_sprites_proc.update_sprite( SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data, false );
		}
		
		private string add_pref_postf_func( bool _postfix, string _pref_postf, sprite_data _spr, ref bool _changed )
		{
			_changed = true;
			
			return _postfix ? ( _spr.name + _pref_postf ):( _pref_postf + _spr.name );
		}
		
		private string remove_pref_postf_func( bool _postfix, string _pref_postf, sprite_data _spr, ref bool _changed )
		{
			string spr_name = _spr.name;
			
			_changed = false;
			
			if( _postfix )
			{
				int clean_name_length = spr_name.Length - _pref_postf.Length;
				
				if( spr_name.LastIndexOf( _pref_postf ) == clean_name_length )
				{
					_changed = true;
					
					return spr_name.Substring( 0, clean_name_length );
				}
				
				return spr_name;
			}
			
			// check prefix
			if( spr_name.IndexOf( _pref_postf ) == 0 )
			{
				_changed = true;
				
				return spr_name.Substring( _pref_postf.Length );
			}
			
			return spr_name;
		}
		
		private void BtnDeleteClick( object sender, System.EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to delete " + SpriteList.SelectedIndices.Count + " sprite(s)?", "Delete Selected Sprite(s)", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					if( !check_selected_sprites_data( "Delete Selected Sprite(s)" ) )
					{
						return;
					}
					
					sprite_data spr;

					int spr_CHR_id;
					int i;
					
					List< sprite_data >	sprites = new List< sprite_data >( SpriteList.SelectedIndices.Count );
					
					for( i = 0; i < SpriteList.SelectedIndices.Count; i++ )
					{
						sprites.Add( SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data );
					}
					
					SpriteList.BeginUpdate();

					m_sprites_proc.CHR_bank_optimization_begin();
					
					for( i = 0; i < sprites.Count; i++ )
					{
						spr = sprites[ i ];
						
						SpriteList.Items.Remove( spr );
						
						spr_CHR_id = spr.get_CHR_data().id;
						
						m_sprites_proc.remove( spr );
						
						m_sprites_proc.CHR_bank_optimization( spr_CHR_id, SpriteList.Items, CBoxMode8x16.Checked );
					}
					
					m_sprites_proc.CHR_bank_optimization_end( false );
					
					SpriteList.EndUpdate();
					
					SpriteList.SelectedIndices.Clear();
					
					if( SpriteList.Items.Count == 0 )
					{
						reset();
						
						set_title_name( null );
					}
					else
					{
						m_sprites_proc.update_sprite( null );
					}
					
					m_sprites_proc.rearrange_CHR_data_ids();
				}
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Delete Sprite(s)", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnApplyDefaultPaletteClick( object sender, System.EventArgs e )
		{
			int size = SpriteList.SelectedIndices.Count;
			
			if( size > 0 )
			{
				sprite_data spr;
				
				for( int i = 0; i < size; i++ )
				{
					spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;
					
					if( m_sprites_proc.apply_active_palette( spr ) == false )
					{
						message_box( "Please, select an active palette!", "Apply Default Palette", MessageBoxButtons.OK, MessageBoxIcon.Error );
						break;
					}
				}
				
				update_selected_sprite( false );
			}
		}
		
		private void BtnOffsetClick( object sender, System.EventArgs e )
		{
			int size = SpriteList.SelectedIndices.Count;
			
			if( size > 0 )
			{
				bool add_offset = false;
				
			    DialogResult dlg_res = message_box( "[Yes] - SET new value\n[No] - ADD offset", "Set Offset", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
			    
			    if( dlg_res == DialogResult.Cancel )
			    {
			    	return;
			    }
			    
			    if( dlg_res == DialogResult.No )
			    {
			    	add_offset = true;
			    }
				
				sprite_data spr;
				
				for( int i = 0; i < size; i++ )
				{
					spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;
					
					if( add_offset == true )
					{
						spr.offset_x += ( int )OffsetX.Value;
						spr.offset_y += ( int )OffsetY.Value;
					}
					else
					{
						spr.offset_x = ( int )OffsetX.Value;
						spr.offset_y = ( int )OffsetY.Value;
					}
				}
				
				update_selected_sprite( false );
			}
		}
		
		private void BtnVFlipClick( object sender, System.EventArgs e )
		{
			if( m_sprites_proc.layout_get_mode() == sprite_layout_viewer.e_mode.Build )
			{
				if( SpriteList.SelectedIndices.Count > 0 )
				{
					m_sprites_proc.flip_selected_CHR( CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP );
				}
				else
				{
					message_box( "Please, select a CHR!", "Vertical Flipping", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void BtnHFlipClick( object sender, System.EventArgs e )
		{
			if( m_sprites_proc.layout_get_mode() == sprite_layout_viewer.e_mode.Build )
			{
				if( SpriteList.SelectedIndices.Count > 0 )
				{
					m_sprites_proc.flip_selected_CHR( CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP );
				}
				else
				{
					message_box( "Please, select a CHR!", "Horizontal Flipping", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void BtnSelectAllClick( object sender, System.EventArgs e )
		{
			int size = SpriteList.Items.Count;
			
			for( int i = 0; i < size; i++ )
			{
				SpriteList.SetSelected( i, true );
			}
		}
		
		private void BtnUpClick( object sender, System.EventArgs e )
		{
			move_item( delegate( int _ind ) { return _ind-1 < 0 ? 0:_ind-1; } );
		}
		
		private void BtnDownClick( object sender, System.EventArgs e )
		{
			move_item( delegate( int _ind ) { return _ind+1 > SpriteList.Items.Count ? SpriteList.Items.Count:_ind+1; } );
		}
		
		private void move_item( Func< int, int > _act )
		{
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				int sel_ind 	= SpriteList.SelectedIndex;
				sprite_data spr = SpriteList.Items[ sel_ind ] as sprite_data;
				
				SpriteList.Items.RemoveAt( SpriteList.SelectedIndex );
				
				sel_ind = _act( sel_ind );
					
				SpriteList.Items.Insert( sel_ind, spr );
				
				SpriteList.SetSelected( sel_ind, true );
			}
		}
		
		private void SpriteListItemClick( object sender, System.EventArgs e )
		{
			if( SpriteList.SelectedIndex >= 0 )
			{
				update_selected_sprite( true );
#if DEF_FIXED_LEN_PALETTE16_ARR
				CBoxPalettes.Enabled = true;
#endif
			}
			else
			{
				sprite_data spr = null;
				m_sprites_proc.update_sprite( spr, true );
				
				OffsetX.Value = 0;
				OffsetY.Value = 0;
#if DEF_FIXED_LEN_PALETTE16_ARR
				CBoxPalettes.Enabled = false;
#endif
			}
		}
		
		private void BtnCreateClick( object sender, EventArgs e )
		{
#if DEF_PCE
			m_create_sprite_form.Text = "Create Sprite";
#elif DEF_NES || DEF_SMS
			m_create_sprite_form.Text = "Create Sprite [ mode: " + ( CBoxMode8x16.Checked ? "8x16":"8x8" ) + " ]";
#endif
			if( !check_all_sprites_data( m_create_sprite_form.Text ) )
			{
				return;
			}
			
			if( m_create_sprite_form.ShowDialog() == DialogResult.Cancel )
			{
				return;
			}
			
			if( CBoxMode8x16.Checked && ( m_create_sprite_form.sprite_height&0x01 ) == 1 )
			{
				message_box( "The sprite height must be an even number!", "8x16 Mode Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			
			if( m_create_sprite_form.edit_str == "" )
			{
				message_box( "The sprite name is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}
			
			string new_sprite_name = m_create_sprite_form.edit_str;
			
			if( !check_duplicate( new_sprite_name ) )
			{
				SpriteList.Items.Add( m_sprites_proc.create_sprite( new_sprite_name, m_create_sprite_form.sprite_width, m_create_sprite_form.sprite_height, CBoxMode8x16.Checked ) );
				
				select_last_sprite();
			}
			else
			{
				message_box( new_sprite_name + " - A sprite with the same name already exists! Ignored!", "Sprite Creating Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void select_last_sprite()
		{
			SpriteList.ClearSelected();
			SpriteList.SetSelected( SpriteList.Items.Count - 1, true );
			
			m_sprites_proc.layout_sprite_centering();
			
			update_selected_sprite( true );
		}
	}
}
