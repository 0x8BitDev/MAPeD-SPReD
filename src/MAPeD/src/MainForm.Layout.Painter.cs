/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 15.02.2023
 * Time: 16:35
 */
using System;
using System.Windows.Forms;

namespace MAPeD
{
	partial class MainForm
	{
		private void BtnPainterFillWithTileClick( object sender, EventArgs e )
		{
			CheckBoxPainterReplaceTiles.Checked = false;
			
			m_layout_editor.set_param( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SET_PNT_FILL_WITH_TILE, null );
		}
		
		private void CheckBoxPainterReplaceTilesChecked( object sender, EventArgs e )
		{
			m_layout_editor.set_param( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SET_PNT_REPLACE_TILES, ( sender as CheckBox ).Checked );
		}
		
		private void on_active_tile_cancel( object sender, EventArgs e )
		{
			clear_active_tile_img();
		}
		
		private void on_map_scale_x1( object sender, EventArgs e )
		{
			RBtnMapScaleX1.Checked = true;
		}
		
		private void on_map_scale_x2( object sender, EventArgs e )
		{
			RBtnMapScaleX2.Checked = true;
		}
		
		private void RBtnMapScaleX1CheckedChanged( object sender, EventArgs e )
		{
			if( RBtnMapScaleX1.Checked )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_BASE_MAP_SCALE_X1, null );
			}
		}
		
		private void RBtnMapScaleX2CheckedChanged( object sender, EventArgs e )
		{
			if( RBtnMapScaleX2.Checked )
			{
				m_layout_editor.set_param( layout_editor_param.CONST_SET_BASE_MAP_SCALE_X2, null );
			}
		}
		
		private void BtnTilesBlocksClick( object sender, EventArgs e )
		{
			m_tiles_palette_form.show_window();
			
			BtnTilesBlocks.Enabled = false;
		}
		
		private void on_tiles_blocks_form_hided( object sender, EventArgs e )
		{
			BtnTilesBlocks.Enabled = true;
		}
		
		private void on_update_tile_image( object sender, EventArgs e )
		{
			NewTileEventArg event_args = e as NewTileEventArg;
			
			int tile_ind 	= event_args.tile_ind;
			tiles_data data = event_args.data;
			
			m_imagelist_manager.update_tile( tile_ind, m_view_type, data, true, PropertyPerBlockToolStripMenuItem.Checked, null, null, m_data_manager.screen_data_type );
			m_tile_list_manager.update_tile( tile_list.e_data_type.Tiles, tile_ind );
		}
		
		private void BtnResetTileClick( object sender, EventArgs e )
		{
			clear_active_tile_img();
		}
		
		private void update_active_tile_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				m_layout_editor.set_param( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SET_PNT_UPD_ACTIVE_TILE, _ind );
				
				m_tile_preview.update( m_imagelist_manager.get_tiles_image_list()[ _ind ], PBoxActiveTile.Width, PBoxActiveTile.Height, 0, 0, true, true );
				GrpBoxActiveTile.Text = "Tile: " + String.Format( "${0:X2}", _ind );
				
				BtnResetTile.Enabled = true;
				
				BtnPainterFillWithTile.Enabled = CheckBoxPainterReplaceTiles.Enabled = true;
			}
		}
		
		private void update_active_block_img( int _ind )
		{
			if( _ind >= 0 && m_data_manager.tiles_data_pos >= 0 )
			{
				m_layout_editor.set_param( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SET_PNT_UPD_ACTIVE_BLOCK, _ind );
				
				m_tile_preview.update( m_imagelist_manager.get_blocks_image_list()[ _ind ], PBoxActiveTile.Width, PBoxActiveTile.Height, 0, 0, true, true );
				GrpBoxActiveTile.Text = "Block: " + String.Format( "${0:X2}", _ind );
				
				BtnResetTile.Enabled = true;
				
				if( m_data_manager.screen_data_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
				{
					// disable replacing of blocks when the Tiles4x4 mode is active
					// because it requires generating a lot of 4x4 tiles
					CheckBoxPainterReplaceTiles.Enabled = false;
				}
				else
				{
					CheckBoxPainterReplaceTiles.Enabled = true;
				}
				
				BtnPainterFillWithTile.Enabled = true;
			}
		}
		
		private void clear_active_tile_img()
		{
			m_layout_editor.set_param( layout_editor_base.e_mode.Painter, layout_editor_param.CONST_SET_PNT_CLEAR_ACTIVE_TILE, null );
			
			m_tile_preview.update( null, 0, 0, 0, 0, true, true );
			GrpBoxActiveTile.Text = "...";
			
			BtnResetTile.Enabled = false;
			
			m_layout_editor.update();
			
			CheckBoxPainterReplaceTiles.Checked = false;
			
			BtnPainterFillWithTile.Enabled = CheckBoxPainterReplaceTiles.Enabled = false;
		}
	}
}
