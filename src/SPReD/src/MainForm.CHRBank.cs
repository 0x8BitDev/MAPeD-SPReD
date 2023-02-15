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
		private void BtnAddCHRClick( object sender, EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data;
				
				m_sprites_proc.add_last_CHR( spr );
			}
		}
		
		private void BtnDeleteLastCHRClick( object sender, EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count == 1 )
			{
				sprite_data spr = SpriteList.Items[ SpriteList.SelectedIndices[ 0 ] ] as sprite_data;
				
				if( message_box( "Are you sure you want to delete the last CHR?", "Delete Last CHR", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					m_sprites_proc.delete_last_CHR( spr );
					
					// check all sprites with the same CHR index
					int size = SpriteList.Items.Count;
					
					sprite_data verif_spr;
					
					for( int i = 0; i < size; i++ )
					{
						verif_spr = SpriteList.Items[ i ] as sprite_data;
						
						if( verif_spr != spr && verif_spr.get_CHR_data().id == spr.get_CHR_data().id )
						{
							verif_spr.validate();
						}
					}
				}
			}
		}
		
		private void BtnDeleteCHRClick( object sender, EventArgs e )
		{
			if( m_sprites_proc.layout_get_mode() == sprite_layout_viewer.e_mode.Build )
			{
				m_sprites_proc.layout_delete_CHR();
			}
		}
		
		private void BtnCHREditorVFlipClick( object sender, EventArgs e )
		{
			m_sprites_proc.chr_transform( CHR_data.e_transform.VFlip );
		}
		
		private void BtnCHREditorHFlipClick( object sender, EventArgs e )
		{
			m_sprites_proc.chr_transform( CHR_data.e_transform.HFlip );
		}
		
		private void BtnCHREditorRotateClick( object sender, EventArgs e )
		{
			m_sprites_proc.chr_transform( CHR_data.e_transform.Rotate );
		}
		
		private void FillWithColorToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_sprites_proc.CHR_fill_with_color() == false )
			{
				message_box( "Please, select an active palette and a CHR!", "Fill With Color", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void CopyCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_sprites_proc.CHR_copy() == false )
			{
				message_box( "Please, select a CHR!", "Copy CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else
			{
				PasteCHRToolStripMenuItem.Enabled = true;
			}
		}
		
		private void PasteCHRToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_sprites_proc.CHR_paste() == false )
			{
				message_box( "Please, select a CHR to paste!", "Paste CHR", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
	}
}
