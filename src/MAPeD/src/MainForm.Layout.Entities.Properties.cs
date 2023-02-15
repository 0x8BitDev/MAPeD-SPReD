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
		private void fill_entity_data( entity_data _ent, string _inst_prop = "", string _inst_name = "", int _targ_uid = -1 )
		{
			groupBoxEntityEditor.Enabled = ( _ent != null );
			
			if( m_layout_editor.mode == layout_editor_base.e_mode.Entities )
			{
				if( ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) == layout_editor_param.CONST_SET_ENT_INST_EDIT )
				{
					LayoutContextMenuEntityItemsEnable( _ent != null );
				}
				else
				{
					LayoutContextMenuEntityItemsEnable( false );
				}
				
				if( _ent != null )
				{
					bool edit_inst_mode = ( ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) == layout_editor_param.CONST_SET_ENT_INST_EDIT || ( uint )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_MODE ) == layout_editor_param.CONST_SET_ENT_SELECT_TARGET );
					
					NumericUpDownEntityUID.Value 	= _ent.uid;
					NumericUpDownEntityWidth.Value 	= _ent.width;
					NumericUpDownEntityHeight.Value = _ent.height;
					NumericUpDownEntityPivotX.Value = _ent.pivot_x;
					NumericUpDownEntityPivotY.Value = _ent.pivot_y;
					PBoxColor.BackColor				= _ent.color;
					TextBoxEntityProperties.Text	= _ent.properties;
					LabelEntityName.Text			= edit_inst_mode ? _ent.name + "/" + _inst_name:_ent.name;
					
					TextBoxEntityInstanceProp.Text	= edit_inst_mode ? _inst_prop:_ent.inst_properties;
					
					TextBoxEntityProperties.BackColor = edit_inst_mode ? Color.Gainsboro:Color.FromName( "Window" );
					
					NumericUpDownEntityUID.Enabled 		= !edit_inst_mode;
					NumericUpDownEntityPivotX.Enabled 	= !edit_inst_mode;
					NumericUpDownEntityPivotY.Enabled 	= !edit_inst_mode;
					PBoxColor.Enabled					= !edit_inst_mode;
					TextBoxEntityProperties.Enabled		= !edit_inst_mode;
					BtnEntityLoadBitmap.Enabled			= !edit_inst_mode;
					
					CheckBoxSelectTargetEntity.Enabled	= edit_inst_mode;
					
					NumericUpDownEntityWidth.Enabled = NumericUpDownEntityHeight.Enabled = edit_inst_mode ? false:( _ent.image_flag ? false:true );
				}
				else
				{
					NumericUpDownEntityUID.Value 	= 0;
					NumericUpDownEntityWidth.Value 	= 1;
					NumericUpDownEntityHeight.Value = 1;
					NumericUpDownEntityPivotX.Value = 0;
					NumericUpDownEntityPivotY.Value = 0;
					PBoxColor.BackColor				= utils.CONST_COLOR_ENTITY_PIXBOX_INACTIVE;
#if DEF_PLATFORM_16BIT
					TextBoxEntityInstanceProp.Text	= TextBoxEntityProperties.Text	= "space separated 16-bit HEX values: 20a8 1f00 ...";
#else
					TextBoxEntityInstanceProp.Text	= TextBoxEntityProperties.Text	= "space separated 8-bit HEX values: 20 a8 ...";
#endif
					LabelEntityName.Text			= "ENTITY NAME";
				}
	
				CheckBoxSelectTargetEntity.Text		= "Target UID: " + ( _targ_uid < 0 ? "none":_targ_uid.ToString() );
				
				update_entity_preview( _ent == null );
			}
		}

		private void PBoxColorClick( object sender, EventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null && ( ent.image_flag == false || ( ent.image_flag == true && message_box( "Delete the entity image and use the color box instead?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes ) ) )
			{
				if( ColorDialogEntity.ShowDialog() == DialogResult.OK )
				{
					ent.image_flag 	= false;
					
					ent.color = ColorDialogEntity.Color;
					
					fill_entity_data( ent );
				}
			}
		}
		
		private void NumericUpDownEntityUIDChanged( object sender, EventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				ent.uid = ( byte )NumericUpDownEntityUID.Value;
			}
		}
		
		private void NumericUpDownEntityWidthChanged( object sender, EventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.width != ( byte )NumericUpDownEntityWidth.Value )
				{
					ent.width = ( byte )NumericUpDownEntityWidth.Value;
					
					update_entity_preview();
				}
			}
		}
		
		private void NumericUpDownEntityHeightChanged( object sender, EventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.height != ( byte )NumericUpDownEntityHeight.Value )
				{
					ent.height = ( byte )NumericUpDownEntityHeight.Value;
					
					update_entity_preview();
				}
			}
		}
		
		private void NumericUpDownEntityPivotXChanged( object sender, EventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.pivot_x != ( sbyte )NumericUpDownEntityPivotX.Value )
				{
					ent.pivot_x = ( sbyte )NumericUpDownEntityPivotX.Value;
					
					update_entity_preview();
				}
			}
		}
		
		private void NumericUpDownEntityPivotYChanged( object sender, EventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				if( ent.pivot_y != ( sbyte )NumericUpDownEntityPivotY.Value )
				{
					ent.pivot_y = ( sbyte )NumericUpDownEntityPivotY.Value;
					
					update_entity_preview();
				}
			}
		}
		
		private void BtnEntityLoadBitmapClick( object sender, EventArgs e )
		{
			EntityLoadBitmapOpenFileDialog.ShowDialog();
		}
		
		private void EntityLoadBitmapOpenFileDialogFileOk( object sender, System.ComponentModel.CancelEventArgs e )
		{
			String filename = ( ( FileDialog )sender ).FileName;
			
			Bitmap bmp			= null;
			Bitmap unlocked_bmp	= null;
			
			try
			{
				bmp = new Bitmap( filename );
				
				// unlock the bmp
				unlocked_bmp = new Bitmap( bmp, bmp.Width, bmp.Height );
				bmp.Dispose();
				bmp = null;
				
				entity_data ent = get_selected_entity();
				
				if( ent.width != unlocked_bmp.Width || ent.height != unlocked_bmp.Height )
				{
					if( message_box( "Rescale the loaded image to the entity size?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
					{
						Bitmap rescaled_bmp = new Bitmap( unlocked_bmp, new Size( ent.width, ent.height ) );
						
						unlocked_bmp.Dispose();
						unlocked_bmp = rescaled_bmp;
					}
					else
					{
						if( unlocked_bmp.Width > 255 || unlocked_bmp.Height > 255 )
						{
							throw new Exception( "Invalid image size! The width and height must be less than 256!" );
						}
						
						ent.width 	= (byte)unlocked_bmp.Width;
						ent.height 	= (byte)unlocked_bmp.Height;
						ent.pivot_x = 0;
						ent.pivot_y = 0;
					}
				}
				
				if( ent != null )
				{
					ent.bitmap 		= unlocked_bmp;
					ent.image_flag 	= true;
				}

				fill_entity_data( ent );
			}
			catch( Exception _err )
			{
				if( bmp != null )
				{
					bmp.Dispose();
				}

				if( unlocked_bmp != null )
				{
					unlocked_bmp.Dispose();
				}
				
				message_box( _err.Message, "Entity Image Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void TextBoxEntityInstancePropTextKeyUp( object sender, KeyEventArgs e )
		{
			entity_instance ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			
			entity_data ent = get_selected_entity();

			TextBox text_box = sender as TextBox;

			if( ent_inst != null )
			{
				ent_inst.properties = text_box.Text;
			}
			else
			if( ent != null )
			{
				ent.inst_properties = text_box.Text;
			}
		}

		private void TextBoxEntityPropertiesTextKeyUp( object sender, KeyEventArgs e )
		{
			entity_data ent = get_selected_entity();
			
			if( ent != null )
			{
				TextBox text_box = sender as TextBox;
				
				ent.properties = text_box.Text;
			}
		}
		
		private void TextBoxEntityPropertiesKeyPress( object sender, KeyPressEventArgs e )
		{
			if( !char.IsControl( e.KeyChar ) && "0123456789abcdefABCDEF".IndexOf( e.KeyChar ) < 0 && ( e.KeyChar != ' ' ) )
		    {
		    	e.Handled = true;
		    }
		}

		private void CheckBoxSelectTargetEntityChanged( object sender, EventArgs e )
		{
			if( m_layout_editor.mode == layout_editor_base.e_mode.Entities )
			{
				if( ( sender as CheckBox ).Checked )
				{
					CheckBoxSelectTargetEntity.FlatStyle = FlatStyle.Standard;

					set_entity_mode( layout_editor_param.CONST_SET_ENT_SELECT_TARGET );
				}
				else
				{
					CheckBoxSelectTargetEntity.FlatStyle = FlatStyle.System;
					
					// return to the edit instances mode
					m_layout_editor.set_param( layout_editor_param.CONST_SET_ENT_INST_EDIT, null );
				}
			}
		}

		private void update_entity_preview( bool _force_disable = false )
		{
			entity_instance ent_inst = ( entity_instance )m_layout_editor.get_param( layout_editor_param.CONST_GET_ENT_INST_SELECTED );
			
			entity_data ent = ( ent_inst != null && _force_disable == false ) ? ent_inst.base_entity:get_selected_entity();
			
			if( ent != null )
			{
				m_entity_preview.set_scaled_image( ent.bitmap );
				m_entity_preview.set_scaled_image_pivot( ent.pivot_x, ent.pivot_y );
				m_entity_preview.scale_enabled( true );
				m_entity_preview.update_scaled( true );
			}
			else
			{
				m_entity_preview.set_scaled_image( null );
				m_entity_preview.scale_enabled( false );
				m_entity_preview.update_scaled( true );
			}
			
			if( CheckBoxShowEntities.Checked )
			{
				m_layout_editor.update();
			}
		}
	}
}
