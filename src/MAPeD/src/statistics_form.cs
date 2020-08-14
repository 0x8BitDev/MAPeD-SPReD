/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
		private data_sets_manager m_data_manager	= null;
		
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
			
			richTextBox.Text = "Platform: " + utils.CONST_PLATFORM;
			richTextBox.Text += "\n---------------------------";
			
			richTextBox.Text += "\nScreen: " + utils.CONST_SCREEN_WIDTH_PIXELS + "x" + utils.CONST_SCREEN_HEIGHT_PIXELS + " pix\nScreen Tiles (2x2): " + ( utils.CONST_SCREEN_WIDTH_PIXELS / 16.0f ) + "x" + ( utils.CONST_SCREEN_HEIGHT_PIXELS / 16.0f ) + "\nScreen Tiles (4x4): " + ( utils.CONST_SCREEN_WIDTH_PIXELS / 32.0f ) + "x" + ( utils.CONST_SCREEN_HEIGHT_PIXELS / 32.0f );
			richTextBox.Text += "\n\nCHR bank: " + ( utils.CONST_CHR_BANK_MAX_SPRITES_CNT * utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES ) / 1024 + " KB (Max: " + utils.CONST_CHR_BANK_MAX_SPRITES_CNT + " CHRs)";  
			richTextBox.Text += "\nCHR size: " + utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES + " Bytes";
			
			richTextBox.Text += "\n---------------------------\nProject:";
			richTextBox.Text += "\nLayouts: " + m_data_manager.layouts_data_cnt + " (Max: " + utils.CONST_LAYOUT_MAX_CNT + ")";
			
			// calc project screens count
			{
				int screens_cnt = 0;
				
				for( CHR_bank_n = 0; CHR_bank_n < m_data_manager.tiles_data_cnt; CHR_bank_n++ )
				{
					screens_cnt += m_data_manager.get_tiles_data()[ CHR_bank_n ].scr_data.Count;
				}
				
				richTextBox.Text += "\nScreens: " + screens_cnt + " (Max: " + utils.CONST_SCREEN_MAX_CNT + ")";
			}
			
			richTextBox.Text += "\nCHR banks: " + m_data_manager.tiles_data_cnt + " (Max: " + utils.CONST_CHR_BANK_MAX_CNT + ")";

			// run through CHR banks
			{
				int ff_CHR;
				int ff_block;
				int ff_tile;
				
				tiles_data data;
				
				for( CHR_bank_n = 0; CHR_bank_n < m_data_manager.tiles_data_cnt; CHR_bank_n++ )
				{
					data = m_data_manager.get_tiles_data()[ CHR_bank_n ];
					
					richTextBox.Text += "\n\n*** CHR bank " + CHR_bank_n + ": ***";
					
					ff_CHR		= data.get_first_free_spr8x8_id();
					ff_block	= data.get_first_free_block_id();
					ff_tile		= data.get_first_free_tile_id();
					
					ff_CHR		= ff_CHR < 0 ? utils.CONST_CHR_BANK_MAX_SPRITES_CNT:ff_CHR;
					ff_block	= ff_block < 0 ? utils.CONST_MAX_BLOCKS_CNT:ff_block;
					ff_tile		= ff_tile < 0 ? utils.CONST_MAX_TILES_CNT:ff_tile;
					
					richTextBox.Text += "\nCHRs: " + ff_CHR + " / Blocks(2x2): " + ff_block + " / Tiles(4x4): " + ff_tile;
					richTextBox.Text += "\nScreens: " + data.scr_data.Count;
				}
			}
		}
	}
}
