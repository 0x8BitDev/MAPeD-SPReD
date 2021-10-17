/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 29.11.2018
 * Time: 16:49
 */
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAPeD
{
	/// <summary>
	/// Description of optimization.
	/// </summary>
	
	public partial class optimization_form : Form
	{
		public event EventHandler UpdateGraphics;
		
		private readonly data_sets_manager m_data_sets 	= null;
		
		private readonly Action< bool, string, bool >	m_show_progress_wnd	= null;
		
		private bool m_need_update_screen_list	= false;
		
		public bool need_update_screen_list
		{
			get { return m_need_update_screen_list; }
			set {}
		}
		
		public optimization_form( data_sets_manager _data_sets, Action< bool, string, bool > _show_progress_wnd )
		{
			m_data_sets 		= _data_sets;
			m_show_progress_wnd = _show_progress_wnd;
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			NumUpDownMatcingPercent.Value = 90;
		}
		
		void BtnOkClick_event(object sender, EventArgs e)
		{
			Close();

			tiles_data data = m_data_sets.get_tiles_data( m_data_sets.tiles_data_pos );
			
			if( data != null )
			{
				if( CheckBoxOptimizeScreens.Checked || CheckBoxOptimizeTiles.Checked || CheckBoxOptimizeBlocks.Checked || CheckBoxOptimizeCHRs.Checked	)
				{
					int stat_screens	= 0;
					int stat_tiles		= 0;
					int stat_blocks		= 0;
					int stat_CHRs		= 0;
					
					m_need_update_screen_list = false;

					m_show_progress_wnd( true, "Data checking...", false );
					
					if( CheckBoxGlobalOptimization.Checked )
					{
						m_data_sets.get_tiles_data().ForEach( delegate( tiles_data _data ) 
						{
							optimization( true, _data, ref stat_screens, ref stat_tiles, ref stat_blocks, ref stat_CHRs );
						});
					}
					else
					{
						optimization( true, data, ref stat_screens, ref stat_tiles, ref stat_blocks, ref stat_CHRs );
					}
					
					m_show_progress_wnd( false, "", true );
					
					if( MainForm.message_box( String.Format( "As a result of the optimization the following data will be deleted:\n\nScreens: ~{0} \\ Tiles: ~{1} \\ Blocks: ~{2} \\ CHRs: ~{3}\n\nConfirm to continue...", stat_screens, stat_tiles, stat_blocks, stat_CHRs ), "Confirm The Optimization Results", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.No )
					{
						MainForm.set_status_msg( "Optimization canceled!" );
						return;
					}

					stat_screens	= 0;
					stat_tiles		= 0;
					stat_blocks		= 0;
					stat_CHRs		= 0;
					
					m_show_progress_wnd( true, "Optimization...", false );
					
					if( CheckBoxGlobalOptimization.Checked )
					{
						m_data_sets.get_tiles_data().ForEach( delegate( tiles_data _data ) 
						{
							optimization( false, _data, ref stat_screens, ref stat_tiles, ref stat_blocks, ref stat_CHRs );
						});
					}
					else
					{
						optimization( false, data, ref stat_screens, ref stat_tiles, ref stat_blocks, ref stat_CHRs );
					}
					
					m_show_progress_wnd( false, "", true );
					
					m_need_update_screen_list = ( stat_screens > 0 );
					
					if( UpdateGraphics != null )
					{
						UpdateGraphics( this, null );
					}
				
					MainForm.set_status_msg( String.Format( "Optimization stats: Screens: {0} \\ Tiles: {1} \\ Blocks: {2} \\ CHRs: {3}", stat_screens, stat_tiles, stat_blocks, stat_CHRs ) );
				}
				else
				{
					MainForm.set_status_msg( "Optimization status: no options selected!" );
				}
			}
		}
		
		void BtnCancelClick_event(object sender, EventArgs e)
		{
			Close();
		}
		
		void optimization( bool _check, tiles_data _data, ref int _stat_screens, ref int _stat_tiles, ref int _stat_blocks, ref int _stat_CHRs )
		{
			int res_screens	= 0;
			int res_tiles	= 0;
			int res_blocks	= 0;
			int res_CHRs	= 0;
			
			do
			{
				if( CheckBoxOptimizeScreens.Checked )
				{
					res_screens = Screens_optimization( _data, _check );
					
					_stat_screens += res_screens;
				}
				
				if( CheckBoxOptimizeTiles.Checked )
				{
					res_tiles = Tiles_optimization( _data, _check );
					
					_stat_tiles += res_tiles;
				}
				
				if( CheckBoxOptimizeBlocks.Checked )
				{
					res_blocks = Blocks_optimization( _data, _check );
					
					_stat_blocks += res_blocks;
				}

				if( CheckBoxOptimizeCHRs.Checked )
				{
					res_CHRs = CHRs_optimization( _data, _check );
					
					_stat_CHRs += res_CHRs;
				}
			}
			while( !_check && ( res_screens + res_tiles + res_blocks + res_CHRs ) != 0 );
		}

		int CHRs_optimization( tiles_data _data, bool _check )
		{
			int deleted_CHRs_cnt = 0;
			
			int CHR_data_sum;

			byte[] img_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
			
			int size_offs;			
			int size = _data.get_first_free_spr8x8_id();
			size = size < 0 ? platform_data.get_CHR_bank_max_sprites_cnt():size;
			
			for( int CHR_n = 0; CHR_n < size; CHR_n++ )
			{
				if( check_blocks_CHR( CHR_n, _data ) == false )
				{
					CHR_data_sum = _data.spr8x8_sum( CHR_n );
					
					if( CHR_data_sum != 0 )
					{
						++deleted_CHRs_cnt;
					}
					
					if( _check == true )
					{
						continue;
					}
					
					// delete useless CHR
					{
						size_offs = platform_data.get_CHR_bank_max_sprites_cnt() - 1;
						
						for( int i = CHR_n; i < size_offs; i++ )
						{
							_data.from_CHR_bank_to_spr8x8( i + 1, img_buff, 0 );
							_data.from_spr8x8_to_CHR_bank( i, img_buff );
						}
					}
					
					// the last CHR is an empty space
					{
#if DEF_ZX
						for( int i = 0; i < img_buff.Length; i++ )
						{
							img_buff[ i ] = utils.CONST_ZX_DEFAULT_PAPER_COLOR;
						}
#else
						Array.Clear( img_buff, 0, utils.CONST_SPR8x8_TOTAL_PIXELS_CNT );
#endif						
						_data.from_spr8x8_to_CHR_bank( platform_data.get_CHR_bank_max_sprites_cnt() - 1, img_buff  );
					}
					
					shift_CHRs_data( CHR_n, _data );
					
					--CHR_n;
					--size;
				}
			}
			
			return deleted_CHRs_cnt;
		}
		
		bool check_blocks_CHR( int _CHR_id, tiles_data _data )
		{
			int i;
			int block_n;
			
			uint block_data;
			
			byte[] img_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
			
			_data.from_CHR_bank_to_spr8x8( _CHR_id, img_buff, 0 );

			// check duplicate(s)
			for( int CHR_n = 0; CHR_n < _CHR_id; CHR_n++ )
			{
				_data.from_CHR_bank_to_spr8x8( CHR_n, utils.tmp_spr8x8_buff, 0 );
				
				if( System.Linq.Enumerable.SequenceEqual( img_buff, utils.tmp_spr8x8_buff ) == true )
				{
					// remove duplicate(s)
					for( block_n = 0; block_n < platform_data.get_max_blocks_UINT_cnt(); block_n += utils.CONST_BLOCK_SIZE )
					{
						for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
						{
							block_data = _data.blocks[ block_n + i ];
							
							if( _CHR_id == tiles_data.get_block_CHR_id( block_data ) )
							{
								_data.blocks[ block_n + i ] = tiles_data.set_block_CHR_id( CHR_n, block_data );
							}
						}
					}
				}
			}
			
			for( block_n = 0; block_n < platform_data.get_max_blocks_UINT_cnt(); block_n += utils.CONST_BLOCK_SIZE )
			{
				for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					if( _CHR_id == tiles_data.get_block_CHR_id( _data.blocks[ block_n + i ] ) )
					{
						return true;
					}
				}
			}
			
			return false;
		}

		void shift_CHRs_data( int _CHR_id, tiles_data _data )
		{
			int block_CHR_id;
			
			uint	raw_block_data;
			int 	CHR_offset;
			
			for( int block_n = 0; block_n < platform_data.get_max_blocks_UINT_cnt(); block_n += utils.CONST_BLOCK_SIZE )
			{
				for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					CHR_offset = block_n + i;
					
					raw_block_data = _data.blocks[ CHR_offset ];
					
					block_CHR_id = tiles_data.get_block_CHR_id( raw_block_data );
					
					if( block_CHR_id >= _CHR_id )
					{
						_data.blocks[ CHR_offset ] = tiles_data.set_block_CHR_id( block_CHR_id - 1, raw_block_data );
					}
				}
			}
		}
		
		int Blocks_optimization( tiles_data _data, bool _check )
		{
			int deleted_blocks_cnt = 0;
			
			uint sum;
			int block_offset;
			
			int size = _data.get_first_free_block_id();
			size = size < 0 ? platform_data.get_max_blocks_cnt():size;
			
			for( int block_n = 0; block_n < size; block_n++ )
			{
				if( check_tiles_block( block_n, _data ) == false )
				{
					sum = 0;
					
					block_offset = block_n << 2;
					
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						sum += _data.blocks[ block_offset + i ];
					}
					
					if( sum != 0 )
					{
						++deleted_blocks_cnt;
					}
					
					if( _check == true )
					{
						continue;
					}
					
					// delete the useless block
					{
						Array.Copy( _data.blocks, block_offset + utils.CONST_BLOCK_SIZE, _data.blocks, block_offset, ( platform_data.get_max_blocks_cnt() << 2 )/* * utils.CONST_BLOCK_SIZE*/ - ( block_offset + utils.CONST_BLOCK_SIZE ) );
					}
					
					// the last block is an empty space
					{
						int  last_block_ind = platform_data.get_max_blocks_cnt() << 2;
						
						_data.blocks[ last_block_ind - 1 ] = 0;
						_data.blocks[ last_block_ind - 2 ] = 0;
						_data.blocks[ last_block_ind - 3 ] = 0;
						_data.blocks[ last_block_ind - 4 ] = 0;
					}

					if( m_data_sets.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						shift_tiles_data( block_n, _data );
					}
					else
					{					
						_data.dec_screen_tiles( ( byte )block_n );
						_data.dec_patterns_tiles( ( byte )block_n );
					}
					
					--block_n;
					--size;
				}
			}
			
			return deleted_blocks_cnt;
		}

		bool check_tiles_block( int _block_id, tiles_data _data )
		{
			if( m_data_sets.screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int tile_n;
				int i;
				
				int _block_id_offset = _block_id << 2;
				int block_n_offset;
				
				// check duplicate(s)
				for( int block_n = 0; block_n < _block_id; block_n++ )
				{
					block_n_offset = block_n << 2; 
					
					for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						if( _data.blocks[ block_n_offset + i ] != _data.blocks[ _block_id_offset + i ] )
						{
							break;
						}
					}
					
					if( i == utils.CONST_BLOCK_SIZE )
					{
						// duplicate found
						for( tile_n = 0; tile_n < platform_data.get_max_tiles_cnt(); tile_n++ )
						{
							for( i = 0; i < utils.CONST_TILE_SIZE; i++ )
							{
								if( _block_id == _data.get_tile_block( tile_n, i ) )
								{
									// replace _block_id with block_n
									_data.set_tile_block( tile_n, i, ( byte )block_n );
								}
							}
						}
					}
				}
				
				for( tile_n = 0; tile_n < platform_data.get_max_tiles_cnt(); tile_n++ )
				{
					for( i = 0; i < utils.CONST_TILE_SIZE; i++ )
					{
						if( _block_id == _data.get_tile_block( tile_n, i ) )
						{
							return true;
						}
					}
				}
			}
			else
			{
				return check_screens_block( _block_id, _data );
			}
			
			return false;
		}
		
		bool check_screens_block( int _block_id, tiles_data _data )
		{
			int block_n;
			int scr_n;
			int scr_block_n;
			
			screen_data 	scr;
			
			// check duplicate(s)
			for( block_n = 0; block_n < _block_id; block_n++ )
			{
				if( _data.cmp_blocks( _block_id, block_n ) )
				{
					for( scr_n = 0; scr_n < _data.screen_data_cnt(); scr_n++ )
					{
						scr = _data.get_screen_data( scr_n );
						
						for( scr_block_n = 0; scr_block_n < scr.length; scr_block_n++ )
						{
							if( scr.get_tile( scr_block_n ) == _block_id )
							{
								// replace _block_id with block_n
								scr.set_tile( scr_block_n, ( ushort )block_n );
							}
						}
					}
				}
			}
			
			for( scr_n = 0; scr_n < _data.screen_data_cnt(); scr_n++ )
			{
				scr = _data.get_screen_data( scr_n );
				
				for( scr_block_n = 0; scr_block_n < scr.length; scr_block_n++ )
				{
					if( scr.get_tile( scr_block_n ) == _block_id )
					{
						return true;
					}
				}
			}

			return false;			
		}

		void shift_tiles_data( int _block_id, tiles_data _data )
		{
			int tile_block_id;
			
			for( int tile_n = 0; tile_n < platform_data.get_max_tiles_cnt(); tile_n++ )
			{
				for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
				{
					tile_block_id = _data.get_tile_block( tile_n, i );
					
					if( tile_block_id >= _block_id )
					{
						_data.set_tile_block( tile_n, i, ( byte )( tile_block_id - 1 ) );
					}
				}
			}
		}
		
		int Tiles_optimization( tiles_data _data, bool _check )
		{
			int deleted_tiles_cnt = 0;
			
			int size = _data.get_first_free_tile_id();
			size = size < 0 ? platform_data.get_max_tiles_cnt():size;
			
			for( int tile_n = 0; tile_n < size; tile_n++ )
			{
				if( check_screens_tile( tile_n, _data ) == false )
				{
					if( _data.tiles[ tile_n ] != 0 )
					{
						++deleted_tiles_cnt;
					}
					
					if( _check == true )
					{
						continue;
					}
					
					// delete the useless tile
					Array.Copy( _data.tiles, tile_n + 1, _data.tiles, tile_n, platform_data.get_max_tiles_cnt() - tile_n - 1 );
					
					// the last tile is an empty space
					_data.tiles[ platform_data.get_max_tiles_cnt() - 1 ] = 0;
					
					_data.dec_screen_tiles( ( byte )tile_n );
					_data.dec_patterns_tiles( ( byte )tile_n );
					
					--tile_n;
					--size;
				}
			}
			
			return deleted_tiles_cnt;
		}

		bool check_screens_tile( int _tile_id, tiles_data _data )
		{
			int tile_n;
			int scr_n;
			int scr_tile_n;
			
			screen_data 	scr;
			
			// check duplicate(s)
			for( tile_n = 0; tile_n < _tile_id; tile_n++ )
			{
				if( _data.tiles[ _tile_id ] == _data.tiles[ tile_n ] )
				{
					for( scr_n = 0; scr_n < _data.screen_data_cnt(); scr_n++ )
					{
						scr = _data.get_screen_data( scr_n );
						
						for( scr_tile_n = 0; scr_tile_n < scr.length; scr_tile_n++ )
						{
							if( scr.get_tile( scr_tile_n ) == _tile_id )
							{
								// replace _tile_id with tile_n
								scr.set_tile( scr_tile_n, ( ushort )tile_n );
							}
						}
					}
				}
			}
			
			for( scr_n = 0; scr_n < _data.screen_data_cnt(); scr_n++ )
			{
				scr = _data.get_screen_data( scr_n );
				
				for( scr_tile_n = 0; scr_tile_n < scr.length; scr_tile_n++ )
				{
					if( scr.get_tile( scr_tile_n ) == _tile_id )
					{
						return true;
					}
				}
			}

			return false;			
		}
		
		int Screens_optimization( tiles_data _data, bool _check )
		{
			int deleted_screens_cnt = 0;

			int bank_id = Convert.ToInt32( _data.name );
			
			int size = _data.screen_data_cnt();
			
			for( int scr_n = 0; scr_n < size; scr_n++ )
			{
				if( check_layouts_screen( scr_n, _data ) == false )
				{
					++deleted_screens_cnt;
					
					if( _check == true )
					{
						continue;
					}

					_data.delete_screen( scr_n );
					
					m_data_sets.remove_screen_from_layouts( bank_id, scr_n );
					
					--scr_n;
					--size;
				}
			}
			
			return deleted_screens_cnt;
		}
		
		bool check_layouts_screen( int _scr_n, tiles_data _data )
		{
			int layout_n;
			int tiles_data_n;
			int scr_n;
			int curr_common_scr_ind;
			
			bool res = false;
			
			layout_data ldata = null;

			int layouts_cnt = m_data_sets.layouts_data_cnt;
			
			int bank_id = Convert.ToInt32( _data.name );
			
			// calc common screen index
			int common_scr_ind = _scr_n;

			for( tiles_data_n = 0; tiles_data_n < bank_id; tiles_data_n++ )
			{
				common_scr_ind += m_data_sets.get_tiles_data( tiles_data_n ).screen_data_cnt();
			}
			
			// check duplicate(s)
			for( scr_n = 0; scr_n < _scr_n; scr_n++ )
			{
				if( _data.get_screen_data( scr_n ).equal( _data.get_screen_data( _scr_n ) ) == true )
				{
					curr_common_scr_ind = scr_n;
					
					for( tiles_data_n = 0; tiles_data_n < bank_id; tiles_data_n++ )
					{
						curr_common_scr_ind += m_data_sets.get_tiles_data( tiles_data_n ).screen_data_cnt();
					}
					
					// remove duplicate(s)
					for( layout_n = 0; layout_n < layouts_cnt; layout_n++ )
					{
						ldata = m_data_sets.get_layout_data( layout_n );
						
						ldata.get_raw_data().ForEach( delegate( List< layout_screen_data > _list ) 
						{ 
							_list.ForEach( delegate( layout_screen_data _scr_data ) 
							{
								if( _scr_data.m_scr_ind == common_scr_ind )
								{
									_scr_data.m_scr_ind = ( byte )curr_common_scr_ind;
								}
							});
						});
					}
				}
			}			
			
			for( layout_n = 0; layout_n < layouts_cnt; layout_n++ )
			{
				ldata = m_data_sets.get_layout_data( layout_n );
				
				ldata.get_raw_data().ForEach( delegate( List< layout_screen_data > _list ) 
				{ 
					_list.ForEach( delegate( layout_screen_data _scr_data ) 
					{
						if( _scr_data.m_scr_ind == common_scr_ind )
						{
							res = true;
							
							return;
						}
					});
				                             	
                 	if( res )
                 	{
                 		return;
                 	}
				});
			}
			
			return res;
		}
		
		private static void check_matched_blocks( tiles_data _data, float _mfactor )
		{
			if( _data != null )
			{
				float match_factor;
				int diff_pix_n;
	
				string res = "";
				
				byte[] A_CHR_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
				byte[] B_CHR_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
				
				int ff_block_id = _data.get_first_free_block_id();
				ff_block_id = ff_block_id < 0 ? platform_data.get_max_blocks_cnt():ff_block_id;
				
				for( int blockA_n = 0; blockA_n < ff_block_id; blockA_n++ )
				{
					for( int blockB_n = blockA_n; blockB_n < ff_block_id; blockB_n++ )
					{
						if( blockA_n != blockB_n )
						{
							match_factor = 0.0f;
							
							for( int CHR_n = 0; CHR_n < 4; CHR_n++ )
							{
								uint blockA = _data.blocks[ ( blockA_n << 2 ) + CHR_n ];
								uint blockB = _data.blocks[ ( blockB_n << 2 ) + CHR_n ];
#if !DEF_SMS
								if( tiles_data.get_block_flags_palette( blockA ) != tiles_data.get_block_flags_palette( blockB ) )
								{
									continue;
								}
#endif
								int A_CHR_id = tiles_data.get_block_CHR_id( blockA );
								int B_CHR_id = tiles_data.get_block_CHR_id( blockB );
	
								if( A_CHR_id != B_CHR_id )
								{
									_data.from_CHR_bank_to_spr8x8( A_CHR_id, A_CHR_buff, 0 );
									_data.from_CHR_bank_to_spr8x8( B_CHR_id, B_CHR_buff, 0 );
									
									diff_pix_n = 0;
									
									for( int pix_n = 0; pix_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; pix_n++ )
									{
										if( A_CHR_buff[ pix_n ] != B_CHR_buff[ pix_n ] )
										{
											++diff_pix_n;
										}
									}
									
									match_factor += 25.0f * ( 1.0f - ( diff_pix_n / ( float )utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ) );
								}
								else
								{
									match_factor += 25.0f;
								}
							}
							
							if( match_factor >= _mfactor )
							{
								res += utils.hex( "#", blockA_n ) + " <-> " + utils.hex( "#", blockB_n ) + " [" + ( int )match_factor + "%]\n";
							}
						}
					}
				}
				
				if( res.Length > 0 )
				{
					MainForm.message_box( res, "Matched Blocks", MessageBoxButtons.OK, MessageBoxIcon.Information );
				}
				else
				{
					MainForm.message_box( "No matches found!", "Matched Blocks", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}
		}
		
		void BtnCheckMatchedBlocksClick_Event(object sender, EventArgs e)
		{
			check_matched_blocks( m_data_sets.get_tiles_data( m_data_sets.tiles_data_pos ), ( float )NumUpDownMatcingPercent.Value );
		}
		
		void BtnMatchedBlocksInfoClick_Event(object sender, EventArgs e)
		{
			MainForm.message_box( "By checking the matched blocks, you can identify similar data.\n\nEnter the boundary value (1-100%) and click the \"Check\" button.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		public void set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			CheckBoxOptimizeTiles.Enabled = ( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
			CheckBoxOptimizeTiles.Checked = ( _type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 ) ? false:CheckBoxOptimizeTiles.Checked;
		}
	}
}
