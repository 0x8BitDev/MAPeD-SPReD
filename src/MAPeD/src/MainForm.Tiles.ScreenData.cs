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
		private void update_screen_size( int _blocks_width = -1, int _blocks_height = -1 )
		{
			if( _blocks_width >= 0 && _blocks_height >= 0 )
			{
				NumericUpDownScrBlocksWidth.Value	= Math.Min( _blocks_width, NumericUpDownScrBlocksWidth.Maximum );
				NumericUpDownScrBlocksHeight.Value	= Math.Min( _blocks_height, NumericUpDownScrBlocksHeight.Maximum );
			}
			
			platform_data.set_screen_blocks_size( ( int )NumericUpDownScrBlocksWidth.Value, ( int )NumericUpDownScrBlocksHeight.Value );
			m_imagelist_manager.update_screen_image_size();
		}
		
		private void NumericUpDownScrBlocksChanged( object sender, EventArgs e )
		{
			 update_screen_size_label();
		}
		
		private void update_screen_size_label()
		{
			LabelScreenResolution.Text = "[" + ( ( int )NumericUpDownScrBlocksWidth.Value << 4 ).ToString() + "x" + ( ( int )NumericUpDownScrBlocksHeight.Value << 4 ).ToString() + "]";
		}
		
		private void BtnScreenDataInfoClick( object sender, EventArgs e )
		{
			message_box( strings.CONST_SCREEN_DATA_TYPE_INFO, strings.CONST_SCREEN_DATA_TYPE_INFO_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		private void RBtnScreenDataTilesClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				RadioButton rbtn = ( RadioButton )sender;
				
				if( !rbtn.Checked )
				{
					if( message_box( strings.CONST_SCREEN_DATA_CONV_BLOCKS2TILES, strings.CONST_SCREEN_DATA_CONV_BLOCKS2TILES_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						progress_bar_show( true, "Blocks (2x2) -> Tiles (4x4)", false );
						
						if( set_screen_data_type( data_sets_manager.e_screen_data_type.Tiles4x4 ) )
						{
							update_graphics( false, true, false );
						}
						
						progress_bar_show( false );
					}
				}
			}
		}
		
		private void RBtnScreenDataBlocksClick( object sender, EventArgs e )
		{
			if( m_data_manager.tiles_data_cnt > 0 )
			{
				RadioButton rbtn = ( RadioButton )sender;
				
				if( !rbtn.Checked )
				{
					if( message_box( strings.CONST_SCREEN_DATA_CONV_TILES2BLOCKS, strings.CONST_SCREEN_DATA_CONV_TILES2BLOCKS_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						progress_bar_show( true, "Tiles (4x4) -> Blocks (2x2)", false );
						
						if( set_screen_data_type( data_sets_manager.e_screen_data_type.Blocks2x2 ) )
						{
							update_graphics( false, true, false );
						}
						
						progress_bar_show( false );
					}
				}
			}
		}
		
		private bool set_screen_data_type( data_sets_manager.e_screen_data_type _type )
		{
			try
			{
				m_data_manager.screen_data_type = _type;
			}
			catch( Exception _err ) 
			{
				set_status_msg( "Screen data conversion canceled!" );
				message_box( _err.Message, "Data Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}
			
			switch( _type )
			{
				case data_sets_manager.e_screen_data_type.Tiles4x4:
					{
						RBtnScreenDataTiles.Checked		= true;
						RBtnScreenDataBlocks.Checked	= false;

						PanelTiles.Enabled = true;
						
						GrpBoxTileEditor.Enabled = true;
						
						GrpBoxPainter.Text = "Data Type: Tiles4x4";
						clear_active_tile_img();
						
						m_tile_list_manager.visible( tile_list.e_data_type.Tiles, true );
						m_tile_list_manager.reset( tile_list.e_data_type.Tiles );
					}
					break;

				case data_sets_manager.e_screen_data_type.Blocks2x2:
					{
						RBtnScreenDataTiles.Checked		= false;
						RBtnScreenDataBlocks.Checked	= true;
						
						PanelTiles.Enabled = false;
						
						GrpBoxTileEditor.Enabled = false;
						
						GrpBoxPainter.Text = "Data Type: Blocks2x2";
						clear_active_tile_img();
						
						m_tiles_processor.tile_select_event( -1, null );
						
						m_tile_list_manager.visible( tile_list.e_data_type.Tiles, false );
					}
					break;
			}
			
			m_tiles_palette_form.set_screen_data_type( _type );
			m_optimization_form.set_screen_data_type( _type );
			m_layout_editor.set_screen_data_type( _type );
			
			patterns_manager_reset_active_pattern();
			
			return true;
		}
	}
}
