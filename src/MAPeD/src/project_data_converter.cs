/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 26.03.2021
 * Time: 13:05
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace MAPeD
{
	/// <summary>
	/// Description of project_data_converter.
	/// </summary>
	/// 
	
	public interface i_project_data_converter
	{
		void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank );

		void load_palettes( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, string _file_ext, tiles_data _data );
		
		void pre_load_block_data( utils.EPlatformType _prj_platform );
		void post_load_block_data( tiles_data _data );
		uint convert_block_data( byte _ver, utils.EPlatformType _prj_platform, int _ind, uint _data );
		
		void palettes_processing( byte _ver, utils.EPlatformType _prj_platform, bool _convert_colors, data_sets_manager _data_mngr, int[] _plt_main );
		
		void load_screens(	byte 											_ver, 
                         	BinaryReader 									_br, 
                         	data_sets_manager.EScreenDataType 				_scr_type, 
                         	data_conversion_options_form.EScreensAlignMode 	_scr_align_mode, 
                         	int 											_prj_scr_tiles_width, 
                         	int 											_prj_scr_tiles_height, 
                         	tiles_data 										_data );
		
		Rectangle get_native_scr_rect();
		Rectangle get_prj_scr_rect();
		
		void post_load_data_cleanup();
		
		int merge_CHR_bin( BinaryReader _br, tiles_data _data );
	}

	// Base functionality
	public class project_data_converter_base : i_project_data_converter
	{
		private Dictionary< int, int >	m_dict_CHR_palette_ind = null;

		protected map_data_config_base m_map_data_app_src = null;
		protected map_data_config_base m_map_data_app_dst = null;
		
		private List< palette16_data >	m_palettes			= null;
		protected List< tiles_data >	m_inner_tiles_data	= null; 

		private Rectangle m_native_scr_rect	= new Rectangle( 0, 0, 0, 0 );
		private Rectangle m_prj_scr_rect	= new Rectangle( 0, 0, 0, 0 );
		
		public virtual void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank )
		{
			load_CHR_data( _ver, _br, _prj_platform, ref _CHR_bank, delegate( byte _val ) { return _val; });
		}
		
		protected void load_CHR_data( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank, Func< byte, byte > _func )
		{
			int app_data_len = platform_data_provider.get_CHR_bank_pages_cnt_by_platform_type( platform_data_provider.get_platform_type() ) * utils.CONST_CHR_BANK_PAGE_SIZE;
			int prj_data_len = platform_data_provider.get_CHR_bank_pages_cnt_by_platform_type( _prj_platform ) * utils.CONST_CHR_BANK_PAGE_SIZE;
			
			int load_data_len = Math.Min( app_data_len, prj_data_len );
			
			for( int i = 0; i < load_data_len; i++ )
			{
				_CHR_bank[ i ] = _func( _br.ReadByte() );
			}
			
			if( prj_data_len > load_data_len )
			{
				// skip the rest data
				_br.ReadBytes( prj_data_len - load_data_len );
			}
		}

		public virtual void load_palettes( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, string _file_ext, tiles_data _data )
		{
			int i;
#if DEF_NES
			bool nes_file = ( _file_ext == utils.CONST_NES_FILE_EXT );
#endif
			int palettes_cnt = ( _ver == 1 ) ? 1: _br.ReadInt32();

			if( m_palettes == null )
			{
				m_palettes = new List< palette16_data >( 16 );
			}
			
			palette16_data plt16 = null;
			
			for( i = 0; i < palettes_cnt; i++ )
			{
				plt16 = new palette16_data();
				
				if( _ver <= 2 )
				{
					plt16.m_palette0 = utils.read_byte_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
					plt16.m_palette1 = utils.read_byte_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
					plt16.m_palette2 = utils.read_byte_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
					plt16.m_palette3 = utils.read_byte_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
				}
				else
				{
					plt16.m_palette0 = utils.read_int_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
					plt16.m_palette1 = utils.read_int_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
					plt16.m_palette2 = utils.read_int_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
					plt16.m_palette3 = utils.read_int_arr( _br, utils.CONST_PALETTE_SMALL_NUM_COLORS );
				}
				
				m_palettes.Add( plt16.copy() );
#if DEF_NES
				if( !nes_file )
				{
					plt16.m_palette1[ 0 ] = plt16.m_palette2[ 0 ] = plt16.m_palette3[ 0 ] = plt16.m_palette0[ 0 ];
				}
#endif
#if DEF_FIXED_LEN_PALETTE16_ARR
				if( i >= utils.CONST_PALETTE16_ARR_LEN )
				{
					continue;
				}
#endif
				_data.palettes_arr.Add( plt16 );
			}
		}

		public void pre_load_block_data( utils.EPlatformType _prj_platform )
		{
			if( _prj_platform == utils.EPlatformType.pt_NES && platform_data_provider.get_platform_type() != utils.EPlatformType.pt_NES )
			{
				m_dict_CHR_palette_ind = new Dictionary<int, int>( utils.CONST_BLOCKS_UINT_SIZE );
			}
			
			m_map_data_app_src = map_data_config_provider.config_app( _prj_platform );
			m_map_data_app_dst = map_data_config_provider.config_app( platform_data_provider.get_platform_type() );

			if( m_inner_tiles_data == null )
			{
				m_inner_tiles_data = new List< tiles_data >( 16 );
			}
			
			tiles_data inner_tiles_data = new tiles_data( true );
			inner_tiles_data.palettes_arr.Clear();
			
			m_inner_tiles_data.Add( inner_tiles_data );
			
			if( m_palettes != null )
			{
				inner_tiles_data.palettes_arr.AddRange( m_palettes );
				
				m_palettes.Clear();
				m_palettes = null;
			}
		}
		
		public void post_load_block_data( tiles_data _data )
		{
			if( m_dict_CHR_palette_ind != null )
			{
				foreach( var obj in m_dict_CHR_palette_ind ) 
				{
					_data.CHR_bank_spr8x8_change_proc( obj.Key, delegate( byte _pix ) 
					{ 
						return ( byte )( _pix + ( obj.Value * utils.CONST_PALETTE_SMALL_NUM_COLORS ) );
					});
				}
				
				m_dict_CHR_palette_ind.Clear();
				m_dict_CHR_palette_ind = null;
			}
		}
		
		public uint convert_block_data( byte _ver, utils.EPlatformType _prj_platform, int _ind, uint _data )
		{
			tiles_data inner_data = get_last_tiles_data();

			if( _ver <= 2 && _prj_platform == utils.EPlatformType.pt_SMS )
			{
				// OLD SMS: [ property_id ](4) [ CHR bank ](2) [ hv_flip ](2) [CHR ind](8)
				// NEW SMS: [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9)
				// 11-12 (CHR bank) <-> 9-10 (hv_flip)
				_data = ( _data & 0xfffff0ff ) | ( ( ( _data & 0x00000300 ) << 2 ) | ( ( _data & 0x00000c00 ) >> 2 ) );
			}
			
			if( m_dict_CHR_palette_ind != null )
			{
				int CHR_id		= m_map_data_app_src.unpack_data( map_data_config_base.EData.ed_CHR_id,		_data );
				int palette_ind	= m_map_data_app_src.unpack_data( map_data_config_base.EData.ed_Palette,	_data );
				
				if( !m_dict_CHR_palette_ind.ContainsKey( CHR_id ) )
				{
					m_dict_CHR_palette_ind.Add( CHR_id, palette_ind );
				}
				
				// reset palette index
				_data = m_map_data_app_src.pack_data( map_data_config_base.EData.ed_Palette, 0, _data );
				
				inner_data.blocks[ _ind ] = _data;
			}
			else
			{
				inner_data.blocks[ _ind ] = _data;
			}
			
			return m_map_data_app_dst.repack( m_map_data_app_src, _data );
		}

		public virtual void palettes_processing( byte _ver, utils.EPlatformType _prj_platform, bool _convert_colors, data_sets_manager _data_mngr, int[] _plt_main )
		{
			int				i;
			int 			plt_n;
			int				data_n;
			tiles_data 		data;
			List< int[] > 	palettes = null;
			
			for( data_n = 0; data_n < _data_mngr.tiles_data_cnt; data_n++ )
			{
				data = _data_mngr.get_tiles_data( data_n );
				
				for( plt_n = 0; plt_n < data.palettes_arr.Count; plt_n++ )
				{
					palettes = data.palettes_arr[ plt_n ].subpalettes;
					
					for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
					{
						if( _convert_colors )
						{
							palettes[ i >> 2 ][ i & 0x03 ] = utils.find_nearest_color_ind( _plt_main[ palettes[ i >> 2 ][ i & 0x03 ] ] );
						}
						else
						{
							palettes[ i >> 2 ][ i & 0x03 ] = palettes[ i >> 2 ][ i & 0x03 ] & ( utils.CONST_PALETTE_MAIN_NUM_COLORS - 1 );
						}
					}
				}
			}
		}
		
		private tiles_data get_last_tiles_data()
		{
			return m_inner_tiles_data[ m_inner_tiles_data.Count - 1 ];
		}
		
		public void load_screens(	byte 											_ver, 
		                         	BinaryReader 									_br, 
		                         	data_sets_manager.EScreenDataType 				_scr_type, 
		                         	data_conversion_options_form.EScreensAlignMode 	_scr_align_mode, 
		                         	int 											_prj_scr_tiles_width, 
		                         	int 											_prj_scr_tiles_height, 
		                         	tiles_data 										_data )
		{
			screen_data scr;
			
			int tile_x;
			int tile_y;
			int tile_y_offset;
			
			int scr_data_len			= utils.get_screen_tiles_cnt_uni( _scr_type );
			int native_scr_tiles_width	= utils.get_screen_num_width_tiles_uni( _scr_type );
			int native_scr_tiles_height	= utils.get_screen_num_height_tiles_uni( _scr_type );

			m_native_scr_rect.X			= 0;
			m_native_scr_rect.Y			= 0;
			m_native_scr_rect.Width		= native_scr_tiles_width;
			m_native_scr_rect.Height	= native_scr_tiles_height;
			
			m_prj_scr_rect.X			= 0;
			m_prj_scr_rect.Y			= 0;
			m_prj_scr_rect.Width		= _prj_scr_tiles_width;
			m_prj_scr_rect.Height		= _prj_scr_tiles_height;
			
			// calc aligned prj screen pos
			{
				int scr_align_data = ( int )_scr_align_mode;
				
				if( ( scr_align_data & data_conversion_options_form.CONST_ALIGN_X_MID ) != 0 )
				{
					m_prj_scr_rect.X = ( m_native_scr_rect.Width >> 1 ) - ( _prj_scr_tiles_width >> 1 );
				}
				else
				if( ( scr_align_data & data_conversion_options_form.CONST_ALIGN_X_RIGHT ) != 0 )
				{
					m_prj_scr_rect.X = m_native_scr_rect.Width - m_prj_scr_rect.Width;
				}
					
				if( ( scr_align_data & data_conversion_options_form.CONST_ALIGN_Y_MID ) != 0 )
				{
					m_prj_scr_rect.Y = ( m_native_scr_rect.Height >> 1 ) - ( _prj_scr_tiles_height >> 1 );
				}
				else
				if( ( scr_align_data & data_conversion_options_form.CONST_ALIGN_Y_BOTTOM ) != 0 )
				{
					m_prj_scr_rect.Y = m_native_scr_rect.Height - m_prj_scr_rect.Height;
				}
			}
			
			bool native_scr_data = ( ( native_scr_tiles_width == _prj_scr_tiles_width ) && ( native_scr_tiles_height == _prj_scr_tiles_height ) );
			
			int scr_cnt = _br.ReadInt32();

			Action< int > skip_tiles = tiles_cnt => { if( _ver < 5 ) { _br.ReadBytes( tiles_cnt ); } else { _br.ReadBytes( tiles_cnt << 1 ); } };

			for( int i = 0; i < scr_cnt; i++ )
			{
				scr = _data.create_screen( _scr_type );
				
				if( native_scr_data )
				{
					scr.load( _ver, _br, scr_data_len, -1 );
				}
				else
				{
					for( tile_y = m_prj_scr_rect.Y; tile_y < ( m_prj_scr_rect.Y + m_prj_scr_rect.Height ); tile_y++ )
					{
						if( tile_y < 0 || tile_y >= native_scr_tiles_height )
						{
							skip_tiles( _prj_scr_tiles_width );
							continue;
						}
						
						tile_y_offset = native_scr_tiles_width * tile_y;
						
						for( tile_x = m_prj_scr_rect.X; tile_x < ( m_prj_scr_rect.X + m_prj_scr_rect.Width ); tile_x++ )
						{
							if( tile_x < 0 || tile_x >= native_scr_tiles_width )
							{
								skip_tiles( _prj_scr_tiles_width );
							}
							else
							{
								scr.load( _ver, _br, 1, tile_y_offset + tile_x );
							}
						}
					}
				}
			}
		}
		
		public Rectangle get_native_scr_rect()
		{
			return m_native_scr_rect;
		}
		
		public Rectangle get_prj_scr_rect()
		{
			return m_prj_scr_rect;
		}
		
		public void post_load_data_cleanup()
		{
			if( m_inner_tiles_data != null )
			{
				m_inner_tiles_data.ForEach( delegate( tiles_data _obj ) { _obj.destroy( true ); } );
				m_inner_tiles_data.Clear();
				
				m_inner_tiles_data = null;
			}
			
			if( m_palettes != null )
			{
				m_palettes.Clear();
				m_palettes = null;
			}
		}
		
		public int merge_CHR_bin( BinaryReader _br, tiles_data _data )
		{
			byte[] tmp_arr 	= new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
			
			int y;
			
			if( _br.BaseStream.Length < utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES )
			{
				return -1;
			}
			
			int added_CHRs = 0;
			
			int chr_id = _data.get_first_free_spr8x8_id();
			
			if( chr_id >= 0 )
			{
				do
				{
					tmp_arr = _br.ReadBytes( utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES );
					
					for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
					{
						convert_CHR( y, ref tmp_arr, ref utils.tmp_spr8x8_buff );
					}
					
					_data.from_spr8x8_to_CHR_bank( chr_id++, utils.tmp_spr8x8_buff );
					
					++added_CHRs;
					
					if( chr_id == utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
					{
						break;
					}
				}
				while( _br.BaseStream.Position + utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES <= _br.BaseStream.Length );
			}
			else
			{
				throw new Exception( "There is no free space in the active CHR bank!" );				
			}
			
			return added_CHRs;
		}
		
		protected virtual void convert_CHR( int _i, ref byte[] _tmp_data_arr, ref byte[] _CHR_data_arr )
		{
			throw new Exception( "Not implemented!" );				
		}
	}
#if DEF_NES
	// NES data converter
	public class project_data_converter : project_data_converter_base
	{
		public project_data_converter()
		{
			//...
		}
		
		public override void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank )
		{
			load_CHR_data( _ver, _br, _prj_platform, ref _CHR_bank, delegate( byte _val ) { return ( byte )( _val & 0x03 ); });
		}
		
		protected override void convert_CHR( int _y, ref byte[] _tmp_data_arr, ref byte[] _CHR_data_arr )
		{
			int shift_7_cnt;
			
			byte byte_0 = _tmp_data_arr[ _y ];
			byte byte_1	= _tmp_data_arr[ _y + utils.CONST_SPR8x8_SIDE_PIXELS_CNT ];
			
			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				shift_7_cnt = 7 - x;
				
				_CHR_data_arr[ x + ( _y << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) ] = ( byte )( ( ( byte_0 & ( 1 << shift_7_cnt ) ) >> shift_7_cnt ) | ( ( ( byte_1 << 1 ) & ( 1 << ( 8 - x ) ) ) >> shift_7_cnt ) );
			}
		}
	}
#elif DEF_SMS
	// SMS data converter
	public class project_data_converter : project_data_converter_base
	{
		public project_data_converter()
		{
			//...
		}
		
		protected override void convert_CHR( int _y, ref byte[] _tmp_data_arr, ref byte[] _CHR_data_arr )
		{
			int shift_7_cnt;
			
			int ind_offset = _y << 2;
			
			byte byte_0	= _tmp_data_arr[ ind_offset ];
			byte byte_1 = _tmp_data_arr[ ind_offset + 1 ];
			byte byte_2	= _tmp_data_arr[ ind_offset + 2 ];
			byte byte_3 = _tmp_data_arr[ ind_offset + 3 ];

			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				shift_7_cnt = 7 - x;
				
				_CHR_data_arr[ x + ( _y << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) ] = ( byte )( ( ( byte_0 >> shift_7_cnt ) & 0x01 ) | ( ( ( byte_1 >> shift_7_cnt ) & 0x01 ) << 1 ) | ( ( ( byte_2 >> shift_7_cnt ) & 0x01 ) << 2 ) | ( ( ( byte_3 >> shift_7_cnt ) & 0x01 ) << 3 ) );
			}
		}
	}
#elif DEF_PCE
	// PCE data converter
	public class project_data_converter : project_data_converter_base
	{
		public project_data_converter()
		{
			//...
		}
		
		protected override void convert_CHR( int _y, ref byte[] _tmp_data_arr, ref byte[] _CHR_data_arr )
		{
			int shift_7_cnt;
			
			int ind_offset = _y << 1;
			
			byte byte_0	= _tmp_data_arr[ ind_offset ]; 
			byte byte_1 = _tmp_data_arr[ ind_offset + 1 ];
			byte byte_2	= _tmp_data_arr[ ind_offset + 16 ];
			byte byte_3 = _tmp_data_arr[ ind_offset + 17 ];

			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				shift_7_cnt = 7 - x;
				
				_CHR_data_arr[ x + ( _y << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) ] = ( byte )( ( ( byte_0 >> shift_7_cnt ) & 0x01 ) | ( ( ( byte_1 >> shift_7_cnt ) & 0x01 ) << 1 ) | ( ( ( byte_2 >> shift_7_cnt ) & 0x01 ) << 2 ) | ( ( ( byte_3 >> shift_7_cnt ) & 0x01 ) << 3 ) );
			}
		}
	}
#elif DEF_SMD
	// SMD data converter
	public class project_data_converter : project_data_converter_base
	{
		public project_data_converter()
		{
			//...
		}
	}
#elif DEF_ZX
	// ZX data converter
	public class project_data_converter : project_data_converter_base
	{
		public project_data_converter()
		{
			//...
		}
		
		public override void palettes_processing( byte _ver, utils.EPlatformType _prj_platform, bool _convert_colors, data_sets_manager _data_mngr, int[] _plt_main )
		{
			int				i;
			int 			plt_n;
			int				block_n;
			int				blocks_cnt;
			uint			block_data;
			int				data_n;
			int				chr_id;
			int				pix_n;
			byte 			paper_clr;
			byte 			ink_clr;
			byte			max_pix_val_ind;
			int				pure_paper_clr;
			int				pure_ink_clr;
			byte			alt_ink_clr;
			bool			ink_paper_equal; 
			tiles_data 		data;
			tiles_data 		inner_data;
			List< int[] > 	palettes = null;
			
			byte[] grid_pattern = new byte[64]{ 0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0,
												0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0,
												0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0,
												0,1,0,1,0,1,0,1,
												1,0,1,0,1,0,1,0 };
			
			
			Dictionary< int, int >	palette_inds	= new Dictionary< int, int >( utils.CONST_PALETTE_MAIN_NUM_COLORS );
			Dictionary< byte, int >	pix_value		= new Dictionary< byte, int >( utils.CONST_PALETTE_MAIN_NUM_COLORS );
			bool[]					chr_id_flags	= null;
			
			for( data_n = 0; data_n < _data_mngr.tiles_data_cnt; data_n++ )
			{
				data 		= _data_mngr.get_tiles_data( data_n );
				inner_data	= m_inner_tiles_data[ data_n ];

				blocks_cnt = data.get_first_free_block_id() << 2;
				
				chr_id_flags = new bool[ blocks_cnt ];
				Array.Clear( chr_id_flags, 0, chr_id_flags.Length );
				
				for( plt_n = 0; plt_n < inner_data.palettes_arr.Count; plt_n++ )
				{
					palettes = inner_data.palettes_arr[ plt_n ].subpalettes;
					
					for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
					{
						palette_inds[ i ] = utils.find_nearest_color_ind( _plt_main[ palettes[ i >> 2 ][ i & 0x03 ] ] );
					}

					for( block_n = 0; block_n < blocks_cnt; block_n++ )
					{
						block_data = inner_data.blocks[ block_n ];
						
						if( m_map_data_app_src.unpack_data( map_data_config_base.EData.ed_Palette, block_data ) == plt_n )
						{
							chr_id = m_map_data_app_src.unpack_data( map_data_config_base.EData.ed_CHR_id, block_data );
							
							if( chr_id_flags[ chr_id ] == false )
							{
								// collect pixel weights data
								data.CHR_bank_spr8x8_change_proc( chr_id, delegate( byte _pix )
								{
									byte new_clr = ( byte )palette_inds[ ( int )_pix ];

									if( !pix_value.ContainsKey( new_clr ) )
									{
										pix_value[ new_clr ] = 1;
									}
									else
									{
										++pix_value[ new_clr ];
									}

									return new_clr; 
								});
								
								// get paper color ( max pixel value )
								{
									max_pix_val_ind = ( byte )pix_value.Values.ToList().IndexOf( pix_value.Values.Max() );

									paper_clr	= ( byte )pix_value.Keys.ToList()[ max_pix_val_ind ];
									pix_value.Remove( paper_clr );
								}
								
								// get ink color ( the next max pixel value )
								{
									if( pix_value.Count > 0 )
									{
										max_pix_val_ind = ( byte )pix_value.Values.ToList().IndexOf( pix_value.Values.Max() );
										ink_clr	= ( byte )pix_value.Keys.ToList()[ max_pix_val_ind ];
									}
									else
									{
										ink_clr	= 0xff;
									}
								}
								
								// check if paper color equal to ink one
								{
									pure_paper_clr	= paper_clr & 0x07;
									pure_ink_clr	= ink_clr & 0x07;
									alt_ink_clr		= 0;
									ink_paper_equal	= false; 
									
									if( pure_paper_clr == pure_ink_clr )
									{
										if( pure_ink_clr == 0 || pure_ink_clr == 0x07 )
										{
											alt_ink_clr = ( byte )( pure_ink_clr ^ 0x07 );
										}
											
										ink_paper_equal = true;
									}
								}
								
								pix_n = 0;
								
								// fix CHR to 2 colors
								data.CHR_bank_spr8x8_change_proc( chr_id, delegate( byte _pix )
								{
									if( _pix == paper_clr )
									{
										_pix |= 0x08;	// the second 8 colors are paper ones
									}
									else
									if( ink_clr != 0xff )
									{
										if( _pix != ink_clr )
										{
											if( grid_pattern[ pix_n ] != 0 )
											{
												if( ink_paper_equal )
												{
													_pix = alt_ink_clr;
												}
												else
												{
													_pix = ( byte )( ink_clr & 0x07 );	// reset paper flag
												}
											}
											else
											{
												// apply grid to mid colors
												_pix = ( byte )( 0x08 | paper_clr );
											}
										}
										else
										{
											if( ink_paper_equal )
											{
												_pix = alt_ink_clr;
											}
											else
											{
												_pix = ( byte )( ink_clr & 0x07 );	// reset paper flag
											}
										}
									}

									++pix_n;
									
									return _pix; 
								});
								
								pix_value.Clear();
								
								chr_id_flags[ chr_id ] = true;
							}
						}
					}
					
					palette_inds.Clear();
				}
				
				chr_id_flags = null;

				// add missing palette
				if( data.palettes_arr.Count < utils.CONST_PALETTE16_ARR_LEN )
				{
					data.palettes_arr.Add( new palette16_data() );
				}
				
				// restore original fixed ZX palette
				for( plt_n = 0; plt_n < utils.CONST_PALETTE16_ARR_LEN; plt_n++ )
				{
					for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
					{
						data.palettes_arr[ plt_n ].subpalettes[ i >> 2 ][ i & 0x03 ] = ( i & 0x07 ) | ( plt_n << 3 );
					}
				}
			}
		}
		
		protected override void convert_CHR( int _y, ref byte[] _tmp_data_arr, ref byte[] _CHR_data_arr )
		{
			int shift_7_cnt;
			
			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				shift_7_cnt = 7 - x;
				
				_CHR_data_arr[ x + ( _y << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) ] = ( byte )( ( ( _tmp_data_arr[ _y ] >> shift_7_cnt ) & 0x01 ) == 0x01 ? 0:15 );
			}
		}
	}
#endif
	public static class project_data_converter_provider
	{
		private static readonly project_data_converter	m_project_data_converter	= new project_data_converter();
		
		static project_data_converter_provider()
		{
			//...
		}
		
		public static i_project_data_converter get_converter()
		{
			return m_project_data_converter;
		}
	}
}
