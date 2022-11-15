/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 14.08.2020
 * Time: 16:25
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of statistics_form.
	/// </summary>
	public partial class statistics_form : Form
	{
		private readonly data_sets_manager m_data_manager	= null;
		
		public statistics_form( data_sets_manager _data_manager )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_data_manager = _data_manager;
		}
		
		public DialogResult ShowStats()
		{
			update_stats();
			
			return ShowDialog();
		}
		
		public void update_stats()
		{
			int CHR_bank_n;
			
			richTextBox.Text = "Platform: " + platform_data.CONST_PLATFORM_DESC;
			richTextBox.Text += "\n---------------------------";
			
			richTextBox.Text += "\nScreen: " + platform_data.get_screen_width_pixels() + "x" + platform_data.get_screen_height_pixels() + " pix / Native: " + platform_data.get_screen_width_pixels( true ) + "x" + platform_data.get_screen_height_pixels( true ) + " pix\nScreen Tiles (2x2): " + ( platform_data.get_screen_width_pixels() / 16.0f ) + "x" + ( platform_data.get_screen_height_pixels() / 16.0f ) + "\nScreen Tiles (4x4): " + ( platform_data.get_screen_width_pixels() / 32.0f ) + "x" + ( platform_data.get_screen_height_pixels() / 32.0f );
			richTextBox.Text += "\n\nCHR bank: " + ( platform_data.get_CHR_bank_max_sprites_cnt() * platform_data.get_native_CHR_size_bytes() ) / 1024 + " KB (Max: " + platform_data.get_CHR_bank_max_sprites_cnt() + " CHRs)";  
			richTextBox.Text += "\nCHR size: " + platform_data.get_native_CHR_size_bytes() + " Bytes";
			richTextBox.Text += "\n\nTiles (4x4): " + platform_data.get_max_tiles_cnt();
			richTextBox.Text += "\nBlocks (2x2): " + platform_data.get_max_blocks_cnt();
			
			richTextBox.Text += "\n---------------------------\nProject:";
			richTextBox.Text += "\nLayouts: " + m_data_manager.layouts_data_cnt + " (Max: " + utils.CONST_LAYOUT_MAX_CNT + ")";
			
			// calc project screens count
			{
				int screens_cnt = 0;
				
				for( CHR_bank_n = 0; CHR_bank_n < m_data_manager.tiles_data_cnt; CHR_bank_n++ )
				{
					screens_cnt += m_data_manager.get_tiles_data()[ CHR_bank_n ].screen_data_cnt();
				}
				
				richTextBox.Text += "\nScreens: " + screens_cnt + " (Max: " + utils.CONST_SCREEN_MAX_CNT + ")";
			}
			
			richTextBox.Text += "\nCHR banks: " + m_data_manager.tiles_data_cnt + " (Max: " + utils.CONST_CHR_BANK_MAX_CNT + ")\n";

			// run through layouts
			{
				layout_data lt_data;
				
				for( int layout_n = 0; layout_n < m_data_manager.layouts_data_cnt; layout_n++ )
				{
					lt_data = m_data_manager.get_layout_data( layout_n );
					
					richTextBox.Text += "\n*** Layout " + layout_n + ": [" + lt_data.get_width() + "x" + lt_data.get_height() + "]\tEntities: " + lt_data.get_ent_instances_cnt();
				}
			}
			
			// run through CHR banks
			{
				int ff_CHR;
				int ff_block;
				int ff_tile;
				
				tiles_data data;
				
				for( CHR_bank_n = 0; CHR_bank_n < m_data_manager.tiles_data_cnt; CHR_bank_n++ )
				{
					data = m_data_manager.get_tiles_data()[ CHR_bank_n ];
					
					richTextBox.Text += "\n\n*** CHR bank " + CHR_bank_n + ":";
					
					ff_CHR		= data.get_first_free_spr8x8_id( false );
					ff_block	= data.get_first_free_block_id( false );
					ff_tile		= data.get_first_free_tile_id( false );
					
					richTextBox.Text += "\nCHRs: " + ff_CHR + " / Blocks(2x2): " + ff_block + " / Tiles(4x4): " + ff_tile;
					richTextBox.Text += "\nScreens: " + data.screen_data_cnt();
					richTextBox.Text += "\nPalettes: " + data.palettes_arr.Count;
				}
			}
		}
	}
}
