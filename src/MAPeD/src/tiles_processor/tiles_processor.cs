/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 12:36
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of tiles_processor.
	/// </summary>
	/// 
	
	public class tiles_processor
	{
		public event EventHandler NeedGFXUpdate;
		public event EventHandler GFXUpdate;
#if DEF_PALETTE16_PER_CHR		
		public event EventHandler UpdatePaletteListPos;
#endif		
		private readonly CHR_bank_viewer m_CHR_bank_viewer	= null;
		private readonly block_editor m_block_editor		= null;
		private readonly tile_editor m_tile_editor			= null;
		private readonly palette_group m_palette_grp		= null;
		
		public tiles_processor( PictureBox 			_PBoxCHRBank,
		                       GroupBox				_CHRBankGrpBox,
		                       PictureBox 			_PBoxBlockEditor,
		                       PictureBox 			_PBoxTilePreview,
		                       PictureBox 			_plt_main,
		                       PictureBox 			_plt0,
		                       PictureBox 			_plt1,
		                       PictureBox 			_plt2,
		                       PictureBox 			_plt3,
		                       data_sets_manager 	_data_mngr )
		{
			m_palette_grp		= new palette_group( _plt_main, _plt0, _plt1, _plt2, _plt3 );
			m_CHR_bank_viewer	= new CHR_bank_viewer( _PBoxCHRBank, _CHRBankGrpBox );
			m_block_editor		= new block_editor( _PBoxBlockEditor );
			m_tile_editor		= new tile_editor( _PBoxTilePreview );

			m_CHR_bank_viewer.subscribe_event( m_block_editor );
			m_CHR_bank_viewer.subscribe_event( _data_mngr );
			m_block_editor.subscribe_event( _data_mngr );
			m_tile_editor.subscribe_event( _data_mngr );
			m_palette_grp.subscribe_event( _data_mngr );
			
			m_CHR_bank_viewer.subscribe_event( m_palette_grp );
			
			m_block_editor.subscribe_event( m_CHR_bank_viewer );
			m_block_editor.subscribe_event( m_tile_editor );
			m_tile_editor.subscribe_event( m_block_editor );
			
			m_CHR_bank_viewer.NeedGFXUpdate	+= new EventHandler( need_gfx_update_event );
			m_block_editor.NeedGFXUpdate 	+= new EventHandler( need_gfx_update_event );
			m_tile_editor.NeedGFXUpdate		+= new EventHandler( need_gfx_update_event );
			m_palette_grp.NeedGFXUpdate		+= new EventHandler( need_gfx_update_event );
#if DEF_PALETTE16_PER_CHR
			m_block_editor.UpdatePaletteListPos	+= new EventHandler( update_palette_list_pos );
#endif
			m_CHR_bank_viewer.subscribe_event( this );
			m_block_editor.subscribe_event( this );
			m_tile_editor.subscribe_event( this );
		}
		
		private void need_gfx_update_event( object sender, EventArgs e )
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}
		
		public void update_graphics()
		{
			if( GFXUpdate != null )
			{
				GFXUpdate( this, null );
			}
		}
		
#if DEF_PALETTE16_PER_CHR
		private void update_palette_list_pos( object sender, EventArgs e )
		{
			if( UpdatePaletteListPos != null )
			{
				UpdatePaletteListPos( sender, e );
			}
		}
#endif		
		public void subscribe_block_quad_selected_event( EventHandler _e )
		{
			m_block_editor.BlockQuadSelected += new EventHandler( _e );
		}
		
		public void block_select_event( int _block_id, tiles_data _data )
		{
			m_block_editor.set_selected_block( _block_id, _data );
			m_tile_editor.set_selected_block( _block_id, _data );
		}

		public void tile_select_event( int _tile_id, tiles_data _data )
		{
			m_tile_editor.set_selected_tile( _tile_id, _data );
		}
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS		
		public void set_CHR_flag_vflip()
		{
			m_block_editor.set_CHR_flag_vflip();
		}
		
		public void set_CHR_flag_hflip()
		{
			m_block_editor.set_CHR_flag_hflip();
		}
#endif
		public void set_block_flags_obj_id( int _id, bool _per_block )
		{
			m_block_editor.set_block_flags_obj_id( _id, _per_block );
		}
		
		public int get_block_flags_obj_id()
		{
			return m_block_editor.get_block_flags_obj_id();
		}
		
		public int get_selected_block()
		{
			return m_block_editor.get_selected_block_id();
		}
		
		public void block_shift_colors( bool _shift_transp )
		{
			m_block_editor.shift_colors( _shift_transp );
		}
#if DEF_PALETTE16_PER_CHR
		public void block_editor_update_sel_CHR_palette()
		{
			m_block_editor.update_sel_CHR_palette();
		}
#endif
		public void transform_selected_CHR( utils.ETransformType _type )
		{
			m_CHR_bank_viewer.transform_selected_CHR( _type );
		}
		
		public void transform_block( utils.ETransformType _type )
		{
			m_block_editor.transform( _type );
		}
		
		public void key_event(object sender, KeyEventArgs e)
		{
			m_palette_grp.key_event( sender, e );
		}
		
		public void set_block_editor_mode( block_editor.EMode _mode )
		{
			m_block_editor.edit_mode = _mode;
			
			m_CHR_bank_viewer.selection_color = ( _mode == block_editor.EMode.bem_CHR_select ) ? utils.CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_SELECT_MODE:utils.CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_DRAW_MODE;
		}
		
		public void set_block_editor_palette_per_CHR_mode( bool _on )
		{
			m_block_editor.palette_per_CHR_mode = _on;
		}
		
		public void tile_editor_locked( bool _on )
		{
			m_tile_editor.locked( _on );
		}
		
		public int get_selected_tile()
		{
			return m_tile_editor.get_selected_tile_id();
		}
		
		public int tile_reserve_blocks( data_sets_manager _data_manager )
		{
			int block_pos 	= 0;
			int sel_tile 	= get_selected_tile();
			
			if( sel_tile >= 0 )
			{
				tiles_data data = _data_manager.get_tiles_data( _data_manager.tiles_data_pos );
				
				if( data.tiles[ sel_tile ] != 0 )
				{
					if( MainForm.message_box( "All the tile's block links will be replaced!", "Reserve Blocks", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning ) == DialogResult.Cancel )
					{
						return -1;
					}
				}
				
				bool reserve_blocks_CHRs = false;
				
				if( MainForm.message_box( "Reserve CHRs?", "Reserve Blocks", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					reserve_blocks_CHRs = true;
				}
				
				// reset tile's links
				if( reserve_blocks_CHRs )
				{
					int block_data_offs;
					
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						block_data_offs = data.get_tile_block( sel_tile, i ) << 2;
						
						data.blocks[ block_data_offs ] = 0;
						data.blocks[ block_data_offs + 1 ] = 0;
						data.blocks[ block_data_offs + 2 ] = 0;
						data.blocks[ block_data_offs + 3 ] = 0;
					}
				}
				
				data.tiles[ sel_tile ] = 0;
				
				int ff_tile_ind = data.get_first_free_tile_id();
				ff_tile_ind = ff_tile_ind < 0 ? utils.CONST_MAX_TILES_CNT:ff_tile_ind;
				int tile_n;
				int block_pos_n;
				
				for( int block_n = 1; block_n < utils.CONST_MAX_BLOCKS_CNT; block_n++ )
				{
					if( data.block_sum( block_n ) == 0 )
					{
						// check if 'zero' block is busy
						for( tile_n = 1; tile_n < ff_tile_ind; tile_n++ )
						{
							for( block_pos_n = 0; block_pos_n < utils.CONST_TILE_SIZE; block_pos_n++ )
							{
								if( data.get_tile_block( tile_n, block_pos_n ) == block_n )
								{	
									// 'zero' block is busy
									break;
								}
							}
							
							if( block_pos_n != utils.CONST_TILE_SIZE )
							{
								// 'zero' block is busy
								break;
							}
						}
						
						// 'zero' block isn't in use OR tiles list is empty
						if( tile_n == ff_tile_ind || ff_tile_ind == 0 )
						{
							data.set_tile_block( sel_tile, block_pos++, ( byte )block_n );
							
							if( reserve_blocks_CHRs )
							{
								block_reserve_CHRs( block_n, _data_manager );
							}
							
							if( block_pos == utils.CONST_TILE_SIZE )
							{
								m_tile_editor.set_selected_tile( sel_tile, data );
								
								MainForm.set_status_msg( String.Format( "Tile Editor: Tile #{0:X2} data reserved", sel_tile ) );
								
								return data.get_tile_block( sel_tile, m_tile_editor.get_selected_block_pos() );
							}
						}
					}
				}
				
				MainForm.message_box( "Tile Editor: Block list is full!", "Reserve Blocks", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			
			return -1;
		}

		public int block_reserve_CHRs( int _block_ind, data_sets_manager _data_manager )
		{
			int CHR_pos 	= 0;

			ushort block_data = 0;
			
			if( _block_ind >= 0 )
			{
				tiles_data data = _data_manager.get_tiles_data( _data_manager.tiles_data_pos );
				
				if( data.block_sum( _block_ind ) != 0 )
				{
					if( MainForm.message_box( "All the block's CHR links will be replaced!", "Reserve CHRs", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning ) == DialogResult.Cancel )
					{
						return -1;
					}
				}
				
				// reset block's links
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					data.blocks[ ( _block_ind << 2 ) + i ] = 0;
				}
				
				int ff_block_ind = data.get_first_free_block_id() << 2;
				ff_block_ind = ff_block_ind < 0 ? utils.CONST_MAX_BLOCKS_CNT:ff_block_ind;
				ff_block_ind = ff_block_ind < 4 ? 4:ff_block_ind;
				
				int block_n;
				
				for( int CHR_n = 1; CHR_n < utils.CONST_CHR_BANK_MAX_SPRITES_CNT; CHR_n++ )
				{
					if( data.spr8x8_sum( CHR_n ) == 0 )
					{
						for( block_n = 4; block_n < ff_block_ind; block_n++ )
						{
							if( tiles_data.get_block_CHR_id( data.blocks[ block_n ] ) == CHR_n )
							{
								break;
							}
						}
						
						if( block_n == ff_block_ind || ff_block_ind == 4  )
						{
							data.blocks[ ( _block_ind << 2 ) + CHR_pos++ ] = tiles_data.set_block_CHR_id( CHR_n, block_data );
							
							if( CHR_pos == utils.CONST_BLOCK_SIZE )
							{
								m_block_editor.set_selected_block( _block_ind, data );
								
								MainForm.set_status_msg( String.Format( "Block Editor: Block #{0:X2} data reserved", _block_ind ) );
								
								return 1;
							}
						}
					}
				}
				
				MainForm.message_box( "Block Editor: CHR bank is full!", "Reserve Blocks", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
			
			return -1;
		}
		
		public bool CHR_bank_copy_spr()
		{
			return m_CHR_bank_viewer.copy_spr();
		}

		public void CHR_bank_paste_spr()
		{
			m_CHR_bank_viewer.paste_spr();
		}
		
		public bool CHR_bank_fill_with_color_spr()
		{
			return m_CHR_bank_viewer.fill_with_color_spr();
		}
		
		public int CHR_bank_get_selected_CHR_ind()
		{
			return m_CHR_bank_viewer.get_selected_CHR_ind();
		}

		public void CHR_bank_next_page()
		{
			m_CHR_bank_viewer.next_page();
		}

		public void CHR_bank_prev_page()
		{
			m_CHR_bank_viewer.prev_page();
		}
	}
}
