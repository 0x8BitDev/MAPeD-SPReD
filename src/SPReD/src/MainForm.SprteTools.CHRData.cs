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
		private void BtnCHRSplitClick( object sender, EventArgs e )
		{
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to split the CHR data?\n\nAll sprite groups with selected sprite(s) will be splitted!\n\nWARNING: ALL sprites, including Ref ones, will have their own unique CHR banks!", "CHR Data Splitting", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					if( check_selected_sprites_data( "CHR Data Splitting" ) )
					{
						sprite_data spr;
						sprite_data sel_spr;
						
						int spr_n;
						int sel_spr_n;

						int sel_spr_bank_ind;
						
						int sprites_cnt		= SpriteList.Items.Count;
						int sel_sprites_cnt	= SpriteList.SelectedIndices.Count;
						
						for( sel_spr_n = 0; sel_spr_n < sel_sprites_cnt; sel_spr_n++ )
						{
							sel_spr = SpriteList.Items[ SpriteList.SelectedIndices[ sel_spr_n ] ] as sprite_data;
							
							sel_spr_bank_ind = sel_spr.get_CHR_data().id;
							
							for( spr_n = 0; spr_n < sprites_cnt; spr_n++ )
							{
								spr = SpriteList.Items[ spr_n ] as sprite_data;
								
								if( spr.is_packed( CBoxMode8x16.Checked ) && spr.get_CHR_data().id == sel_spr_bank_ind )
								{
									m_sprites_proc.split_CHR( spr, CBoxMode8x16.Checked );
								}
							}
						}
						
						m_sprites_proc.rearrange_CHR_data_ids();
						
						// update data in the layout viewport
						update_selected_sprite( false );
					}
				}
			}
			else
			{
				message_box( "No data!", "CHR Data Splitting", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void BtnCHRPackClick( object sender, EventArgs e )
		{
			if( CBoxCHRPackingType.SelectedIndex ==  0 )
			{
				message_box( "Please, select data block size!", "CHR Data Packing", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return;
			}
			
			if( SpriteList.SelectedIndices.Count > 0 )
			{
				if( message_box( "Are you sure you want to pack the selected sprites?\n\nAlready packed sprites will be ignored!\n\nWARNING: Irreversible operation for Ref sprites!", "CHR Data Packing", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					if( !check_selected_sprites_data( "CHR Data Packing" ) )
					{
						return;
					}
					
					int i;
					int j;
					int k;
					
					sprite_data spr_1;
					sprite_data spr_2;
					sprite_data spr_3;
					
					CHR_data_group chr_1;
					CHR_data_group chr_2;
					CHR_data_group new_chr;
					
					int m;
					int size;
					int spr_1_last_CHR_cnt;
					
					SPReD.CHR_data_group.e_CHR_packing_type packing_type = ( SPReD.CHR_data_group.e_CHR_packing_type )( CBoxCHRPackingType.SelectedIndex );
					
					ListBox.SelectedIndexCollection tmp_inds_list = SpriteList.SelectedIndices;
					
//					m_sprites_proc.CHR_bank_optimization_begin();
					
					for( i = 0; i < tmp_inds_list.Count; i++ )
					{
						spr_1 = SpriteList.Items[ tmp_inds_list[ i ] ] as sprite_data;
						chr_1 = spr_1.get_CHR_data();
						
						for( j = i+1; j < tmp_inds_list.Count; j++ )
						{
							spr_2 = SpriteList.Items[ tmp_inds_list[ j ] ] as sprite_data;
							chr_2 = spr_2.get_CHR_data();

							spr_1_last_CHR_cnt = spr_1.get_CHR_data().get_data().Count;
							
							if( CHR_data_group.can_merge( chr_1, chr_2, packing_type ) )
							{
								if( m_sprites_proc.merge_CHR( spr_1, spr_2, CBoxMode8x16.Checked ) )
								{
									new_chr = spr_1.get_CHR_data();
									
									for( k = j+1; k < tmp_inds_list.Count; k++ )
									{
										spr_3 = SpriteList.Items[ tmp_inds_list[ k ] ] as sprite_data;
										
										// if spr_3 and spr_2 have the same bank it means that spr_3 is REF
										// fix CHR indices in attributes data
										if( spr_3.get_CHR_data().id == chr_2.id )
										{
											size = spr_3.get_CHR_attr().Count;
											
											for( m = 0; m < size; m++ )
											{
												spr_3.get_CHR_attr()[ m ].CHR_ind += spr_1_last_CHR_cnt; 
											}
										}
										
										m_sprites_proc.relink_CHR_data( spr_3, new_chr, chr_1.id );
										m_sprites_proc.relink_CHR_data( spr_3, new_chr, chr_2.id );
									}
									
									tmp_inds_list.Remove( tmp_inds_list[ j ] );
								
									--j;
									
//									m_sprites_proc.CHR_bank_optimization( spr_1.get_CHR_data().id, SpriteList.Items, CBoxMode8x16.Checked );
								}
							}
						}
					}

					m_sprites_proc.rearrange_CHR_data_ids();
					
					// update data in the layout viewport
					update_selected_sprite( false );
					
//					m_sprites_proc.CHR_bank_optimization_end( true, "Packing Statistics" );
				}
			}
			else
			{
				if( SpriteList.Items.Count > 0 )
				{
					message_box( "Please, select sprites!", "CHR Data Packing", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
				else
				{
					message_box( "No data!", "CHR Data Packing", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}

		private void BtnCHROptClick( object sender, EventArgs e )
		{
			if( SpriteList.Items.Count > 0 )
			{
				if( message_box( "Are you sure you want to optimize all sprites data?\n\nWARNING: All unused/empty/duplicate CHRs will be lost!", "CHR Data Optimization", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					if( check_all_sprites_data( "CHR Data Optimization" ) )
					{
						m_sprites_proc.CHR_bank_optimization_begin();
						
						for( int i = 0; i < m_sprites_proc.get_CHR_banks().Count; i++ )
						{
							m_sprites_proc.CHR_bank_optimization( m_sprites_proc.get_CHR_banks()[ i ].id, SpriteList.Items, CBoxMode8x16.Checked );
						}
						
						m_sprites_proc.rearrange_CHR_data_ids();
					
						update_selected_sprite( false );
						
						m_sprites_proc.CHR_bank_optimization_end( true );
					}
				}
			}
			else
			{
				message_box( "No data!", "CHR Data Optimization", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void CBoxCHRPackingTypeChanged( object sender, EventArgs e )
		{
			ComboBox cbox = sender as ComboBox;
			
			CHRPackToolStripMenuItem.Enabled = BtnCHRPack.Enabled = ( cbox.SelectedIndex != 0 ) ? true:false;
		}
		
		private bool check_all_sprites_data( string _wnd_title )
		{
			sprite_data	spr;
			
			for( int i = 0; i < SpriteList.Items.Count; i++ )
			{
				spr = SpriteList.Items[ i ] as sprite_data;

				if( !spr.check( CBoxMode8x16.Checked, _wnd_title ) )
				{
					return false;
				}
			}
			
			return true;
		}

		private bool check_selected_sprites_data( string _wnd_title )
		{
			sprite_data	spr;
			
			for( int i = 0; i < SpriteList.SelectedIndices.Count; i++ )
			{
				spr = SpriteList.Items[ SpriteList.SelectedIndices[ i ] ] as sprite_data;

				if( !spr.check( CBoxMode8x16.Checked, _wnd_title ) )
				{
					return false;
				}
			}
			
			return true;
		}
	}
}
