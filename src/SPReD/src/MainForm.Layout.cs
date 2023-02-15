/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 15:46
 */
using System;
using System.Windows.Forms;

namespace SPReD
{
	partial class MainForm
	{
		private void CBoxAxisLayoutCheckedChanged( object sender, EventArgs e )
		{
			m_sprites_proc.set_sprite_layout_viewer_flags( CBoxAxesLayout.Checked, CBoxGridLayout.Checked );
		}
		
		private void CBoxGridLayoutCheckedChanged( object sender, EventArgs e ) 
		{
			m_sprites_proc.set_sprite_layout_viewer_flags( CBoxAxesLayout.Checked, CBoxGridLayout.Checked );
		}
		
		private void CBoxSnapLayoutCheckedChanged( object sender, EventArgs e )
		{
			CheckBox cbox = sender as CheckBox;

			m_sprites_proc.layout_snap( cbox.Checked );
		}
		
		private void CBox8x16ModeCheckedChanged( object sender, EventArgs e )
		{
			m_sprites_proc.set_mode8x16( ( sender as CheckBox ).Checked );
		}

		private void BtnCenteringClick( object sender, EventArgs e )
		{
			m_sprites_proc.layout_sprite_centering();
		}
		
		private void BtnZoomInClick( object sender, EventArgs e )
		{
			m_sprites_proc.layout_zoom_in();
		}
		
		private void BtnZoomOutClick( object sender, EventArgs e )
		{
			m_sprites_proc.layout_zoom_out();
		}
		
		private void BtnSpriteVFlipClick( object sender, EventArgs e )
		{
#if DEF_NES
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_vert( ( sprite_data.e_axes_flip_type )( CBoxFlipType.SelectedIndex ), CBoxMode8x16.Checked ); } );
#elif DEF_SMS
			flip_sprites( "Vertical Flipping", true );
#elif DEF_PCE
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_vert( ( sprite_data.e_axes_flip_type )( CBoxFlipType.SelectedIndex ) ); } );
#endif
		}
		
		private void BtnSpriteHFlipClick( object sender, EventArgs e )
		{
#if DEF_NES || DEF_PCE
			flip_sprites( delegate( sprite_data _spr ) { _spr.flip_horiz( ( sprite_data.e_axes_flip_type )( CBoxFlipType.SelectedIndex ) ); } );
#elif DEF_SMS
			flip_sprites( "Horizontal Flipping", false );
#endif
		}

#if DEF_NES || DEF_PCE
		private void flip_sprites( Action< sprite_data > _act )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				sprite_data spr;
				
				for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
				{
					spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;
					
					_act( spr );
				}
				
				// update data in the layout viewport
				update_selected_sprite( false );
			}
		}
#elif DEF_SMS
		private void flip_sprites( string _title, bool _vert_flip )
		{
			if( SpriteList.SelectedItems.Count > 0 )
			{
				if( !check_selected_sprites_data( _title ) )
				{
					return;
				}
				
				m_SMS_sprite_flip_form.show_window( _title, _vert_flip, CBoxMode8x16.Checked, ( sprite_data.e_axes_flip_type )CBoxFlipType.SelectedIndex );

				if( m_SMS_sprite_flip_form.copy_CHR_data )
				{
					m_sprites_proc.CHR_bank_optimization_begin();
					
					for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
					{
						m_sprites_proc.CHR_bank_optimization( ( SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data ).get_CHR_data().id, SpriteList.Items, CBoxMode8x16.Checked );
					}
					
					m_sprites_proc.rearrange_CHR_data_ids();
					
					m_sprites_proc.CHR_bank_optimization_end( false );
				}
				
				// update data in the layout viewport
				update_selected_sprite( false );
			}
			else
			{
				message_box( "Please, select sprite(s)!", "Sprite(s) Flipping Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
#endif
		private void BtnShiftColorsClick( object sender, EventArgs e )
		{
#if DEF_NES
			if( SpriteList.SelectedIndex >= 0 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndex ] as sprite_data;
				
				spr.shift_colors( CBoxShiftTransp.Checked, CBoxMode8x16.Checked );
				m_sprites_proc.update_sprite( spr, false );
			}
#endif
		}

		private void BtnModeBuildClick( object sender, EventArgs e )
		{
			GroupBoxModeName.Text = "Mode: Build";
			
			drawModeToolStripMenuItem.Enabled = BtnLayoutModeDraw.Enabled 	= true;
			buildModeToolStripMenuItem.Enabled = BtnLayoutModeBuild.Enabled = false;
			
			horizontalFlippingToolStripMenuItem1.Enabled = BtnHFlip.Enabled	= true;
			verticalFlippingToolStripMenuItem1.Enabled = BtnVFlip.Enabled	= true;

			deleteCHRToolStripMenuItem.Enabled = BtnDeleteCHR.Enabled		= true;
			
			CBoxSnapLayout.Enabled = true;
			
			BtnLayoutModeDraw.Focus();
			
			m_sprites_proc.layout_set_mode( sprite_layout_viewer.e_mode.Build );
		}

		private void BtnModeDrawClick( object sender, EventArgs e )
		{
			GroupBoxModeName.Text = "Mode: Draw";
			
			drawModeToolStripMenuItem.Enabled = BtnLayoutModeDraw.Enabled 	= false;
			buildModeToolStripMenuItem.Enabled = BtnLayoutModeBuild.Enabled = true;
			
			horizontalFlippingToolStripMenuItem1.Enabled = BtnHFlip.Enabled	= false;
			verticalFlippingToolStripMenuItem1.Enabled = BtnVFlip.Enabled	= false;
			
			deleteCHRToolStripMenuItem.Enabled = BtnDeleteCHR.Enabled		= false;
			
			CBoxSnapLayout.Enabled = false;
			
			BtnLayoutModeBuild.Focus();
			
			m_sprites_proc.layout_set_mode( sprite_layout_viewer.e_mode.Draw );
		}
	}
}
