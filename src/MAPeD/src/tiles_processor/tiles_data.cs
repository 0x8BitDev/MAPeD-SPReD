﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 03.05.2017
 * Time: 17:18
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MAPeD
{
	/// <summary>
	/// Description of tiles_data.
	/// </summary>
	///

	public class palette16_data
	{
#if DEF_NES		
		public int[] m_palette0	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 14, ( 1+16 ), ( 1+32 ), ( 1+48 ) };
		public int[] m_palette1	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 14, ( 4+16 ), ( 4+32 ), ( 4+48 ) };
		public int[] m_palette2	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 14, ( 7+16 ), ( 7+32 ), ( 7+48 ) };
		public int[] m_palette3	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 14, ( 11+16 ),( 11+32 ),( 11+48 ) };
#elif DEF_SMS
		public int[] m_palette0	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 0,  3,  7,  11 };
		public int[] m_palette1	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 17, 20, 24, 28 };
		public int[] m_palette2	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 34, 37, 41, 45 };
		public int[] m_palette3	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 51, 54, 58, 62 };
#elif DEF_PCE || DEF_SMD
		public int[] m_palette0	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 0, 73, 219, 511 };
		public int[] m_palette1	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 4, 15+64, 15+128, 15+192 };
		public int[] m_palette2	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 40, 40+72, 56+128, 56+192 };
		public int[] m_palette3	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 192, 192+64, 192+128, 192+192 };
#elif DEF_ZX
		public int[] m_palette0	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 0, 1, 2, 3 };
		public int[] m_palette1	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 4, 5, 6, 7 };
		public int[] m_palette2	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 0, 1, 2, 3 };
		public int[] m_palette3	= new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 4, 5, 6, 7 };
#endif
		private List< int[] > m_subpalettes	= null;
		
		[IgnoreDataMember]
		public List< int[] > subpalettes
		{
			get 
			{
				if( m_subpalettes == null )
				{
					m_subpalettes = new List< int[] >( 4 );
					
					m_subpalettes.Add( m_palette0 );
					m_subpalettes.Add( m_palette1 );
					m_subpalettes.Add( m_palette2 );
					m_subpalettes.Add( m_palette3 );
				}
				
				return m_subpalettes;  
			}
			set {}
		}
		
		public palette16_data()
		{
			//...
		}
		
		public palette16_data(	int[] _arr )
		{
			for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
			{
				subpalettes[ i >> 2 ][ i & 0x03 ] = _arr[ i ];
			}
		}
		
		public void reset()
		{
			m_palette0 = m_palette1 = m_palette2 = m_palette3 = null;
			
			if( m_subpalettes != null )
			{
				m_subpalettes.Clear();
				m_subpalettes = null;
			}
		}
		
		public palette16_data copy()
		{
			palette16_data data = new palette16_data();
			
			Array.Copy( this.m_palette0, data.m_palette0, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( this.m_palette1, data.m_palette1, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( this.m_palette2, data.m_palette2, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( this.m_palette3, data.m_palette3, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			
			return data;
		}
		
		public void copy( palette16_data _data )
		{
			Array.Copy( _data.m_palette0, this.m_palette0, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( _data.m_palette1, this.m_palette1, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( _data.m_palette2, this.m_palette2, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( _data.m_palette3, this.m_palette3, utils.CONST_PALETTE_SMALL_NUM_COLORS );
		}
	}
	
	[DataContract]
	public class tiles_data
	{
		private static int m_id	= -1;
		
		private readonly static map_data_config_base m_map_data_config_app 		= map_data_config_provider.config_app();
		private readonly static map_data_config_base m_map_data_config_native 	= map_data_config_provider.config_native();
		
		private string m_name	= null;
		
		private byte[] m_CHR_bank	= new byte[ utils.CONST_CHR_BANK_PAGE_SIZE * platform_data.get_CHR_bank_pages_cnt() ];
		
		[DataMember]
		private List< palette16_data > m_palettes	= null;
		private int	m_palette_pos					= -1;
		
		private uint[]	m_blocks	= new uint[ platform_data.get_max_blocks_UINT_cnt() ];
		private ulong[] m_tiles		= new ulong[ platform_data.get_max_tiles_cnt() ];
		
		[DataMember]
		private List< screen_data >	m_scr_data	= null;
		
		private readonly Dictionary< string, List< pattern_data > >	m_patterns_data;	// key = group name / value = List< pattern_data >

		public Dictionary< string, List< pattern_data > > patterns_data
		{
			get { return m_patterns_data; }
			set {}
		}
		
		[DataMember]
		public string name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		[DataMember]		
		public byte[] CHR_bank
		{
			get { return m_CHR_bank; }
			set {}
		}
		
		public List< palette16_data > palettes_arr
		{
			get { return m_palettes; }
			set {}
		}
		
		public int palette_pos
		{
			get { return m_palette_pos; }
			set 
			{
				if( value >= 0 && value < m_palettes.Count )
				{
					m_palette_pos = value;
				}
			}
		}
				
		public int[] palette0
		{
			get { return m_palettes[ m_palette_pos ].m_palette0; }
			set {}
		}

		public int[] palette1
		{
			get { return m_palettes[ m_palette_pos ].m_palette1; }
			set {}
		}
		
		public int[] palette2
		{
			get { return m_palettes[ m_palette_pos ].m_palette2; }
			set {}
		}
		
		public int[] palette3
		{
			get { return m_palettes[ m_palette_pos ].m_palette3; }
			set {}
		}
		
		public List< int[] > subpalettes
		{
			get 
			{
				return m_palettes[ m_palette_pos ].subpalettes;  
			}
			set {}
		}
		
		[DataMember]
		public uint[] blocks
		{
			get { return m_blocks; }
			set {}
		}
		
		[DataMember]
		public ulong[] tiles
		{
			get { return m_tiles; }
			set {}
		}
		
		public tiles_data( bool _temp_data = false )
		{
			if( !_temp_data )
			{
				++m_id;
				name = build_name( m_id );
			}
			else
			{
				name = "temp";
			}
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			palettes_create_fixed_arr();
#else
			m_palettes = new List< palette16_data >( 16 );
			m_palettes.Add( new palette16_data() );

			m_palette_pos = 0;
#endif
			
			m_scr_data = new List< screen_data >( 100 );
			
			m_patterns_data	= new Dictionary< string, List< pattern_data > >( 10 );
			m_patterns_data.Add( "MAIN", new List< pattern_data >() );
			
#if DEF_ZX
			for( int i = 0; i < m_CHR_bank.Length; i++ )
			{
				m_CHR_bank[ i ] = utils.CONST_ZX_DEFAULT_PAPER_COLOR;
			}
#endif
		}
		
		public void destroy( bool _temp_data = false )
		{
			if( !_temp_data )
			{
				--m_id;
			}
			
			m_CHR_bank 	= null;

			palettes_clear_arr();
			
			m_blocks 	= null;
			m_tiles 	= null;
			
			m_scr_data.ForEach( delegate( screen_data _scr ) { _scr.reset(); });
			m_scr_data.Clear();
			m_scr_data = null;
			
			delete_patterns();
		}
		
		public static string build_name( int _id )
		{
			return _id.ToString();
		}

#if DEF_FIXED_LEN_PALETTE16_ARR
		private void palettes_create_fixed_arr()
		{
			m_palettes = new List< palette16_data >( platform_data.get_fixed_palette16_cnt() );
			
			for( int i = 0; i < platform_data.get_fixed_palette16_cnt(); i++ )
			{
				m_palettes.Add( new palette16_data() );
			}
			
			m_palette_pos = 0;
#if DEF_ZX	
			// bright colors palette
			for( int i = 0; i < 16; i++ )
			{
				m_palettes[ 1 ].subpalettes[ i >> 2 ][ i & 0x03 ] += 8;
			}
#endif
		}
#endif
		private void palettes_clear_arr()
		{
			m_palettes.ForEach( delegate( palette16_data _obj ) { _obj.reset(); } );
			m_palettes.Clear();
		}

		public bool palette_copy()
		{
			if( m_palettes.Count < utils.CONST_PALETTES_MAX_CNT )
			{
				m_palettes.Add( m_palettes[ palette_pos ].copy() );
				m_palette_pos = m_palettes.Count - 1;
				
				return true;
			}
			
			return false;
		}
		
		public bool palette_delete()
		{
			if( m_palettes.Count > 1 )
			{
				m_palettes[ palette_pos ].reset();
				m_palettes.RemoveAt( palette_pos );
				
				m_palette_pos = Math.Min( m_palette_pos, m_palettes.Count - 1 );
				
				return true;
			}
			
			return false;
		}
		
		private void delete_patterns()
		{
			foreach( var key in m_patterns_data.Keys )
			{ 
				List< pattern_data > ptrn_list = m_patterns_data[ key ] as List< pattern_data >;
				
				ptrn_list.ForEach( delegate( pattern_data _pattern ) { _pattern.reset(); });
				ptrn_list.Clear();
			};
			
			m_patterns_data.Clear();
		}
		
		public pattern_data get_pattern_by_name( string _name )
		{
			pattern_data pattern = null;
			
			foreach( string key in patterns_data.Keys )
			{ 
				( patterns_data[ key ] as List< pattern_data > ).ForEach( delegate( pattern_data _pattern ) { if( _pattern.name == _name ) { pattern = _pattern; } } );
			}
			
			return pattern;
		}
		
		public void dec_patterns_tiles( ushort _start_ind )
		{
			patterns_tiles_proc( delegate( ushort _tile_ind ) 
			{
				if( _tile_ind >= _start_ind && _tile_ind > 0 )
				{
					--_tile_ind;
				}
				
				return _tile_ind;
			} );
		}

		public void inc_patterns_tiles( ushort _start_ind )
		{
			patterns_tiles_proc( delegate( ushort _tile_ind ) 
			{
				if( _tile_ind >= _start_ind && _tile_ind > 0 )
				{
					++_tile_ind;
				}
				
				return _tile_ind;
			} );
		}
		
		public void patterns_tiles_proc( Func< ushort, ushort > _func )
		{
			ushort tile_id;
			
			foreach( string key in patterns_data.Keys )
			{ 
				( patterns_data[ key ] as List< pattern_data > ).ForEach( delegate( pattern_data _pattern )
				{ 
					if( _pattern.data != null )
					{
						for( int tile_ind = 0; tile_ind < _pattern.data.length; tile_ind++ )
						{
							tile_id = _pattern.data.get_tile( tile_ind );
							
							tile_id = _func( tile_id );
							
							_pattern.data.set_tile( tile_ind, tile_id );
						}
					}
				});
			}
		}
		
		public void clear_tiles()
		{
			Array.Clear( m_tiles, 0, platform_data.get_max_tiles_cnt() );
		}
		
		public void clear_tile( int _ind )
		{
			for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
			{
				set_tile_block( _ind, i , 0 );
			}
		}

		public void clear_block( int _ind )
		{
			int offs = _ind << 2;
			
			for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
			{
				m_blocks[ offs + i ] = 0;
			}
		}
		
		public ushort get_tile_block( int _tile_id, int _block_n )
		{
			return utils.get_ushort_from_ulong( m_tiles[ _tile_id ], _block_n );
		}
		
		public ulong set_tile_block( int _tile_id, int _block_n, ushort _val )
		{
			ulong tile = utils.set_ushort_to_ulong( m_tiles[ _tile_id ], _block_n, _val );
			
			m_tiles[ _tile_id ] = tile;
			
			return tile;
		}
		
		public override string ToString()
		{
			return name;
		}
		
		public tiles_data copy()
		{
			tiles_data data = new tiles_data();
			
			Array.Copy( m_CHR_bank, data.CHR_bank,	m_CHR_bank.Length );
			
			data.palettes_clear_arr();
			
			for( int i = 0; i < m_palettes.Count; i++ )
			{
				data.m_palettes.Add( m_palettes[ i ].copy() );
			}
			
			Array.Copy( m_blocks,	data.m_blocks,	platform_data.get_max_blocks_UINT_cnt() );
			Array.Copy( m_tiles,	data.m_tiles,	platform_data.get_max_tiles_cnt() );

			// COPY SCREENS
			{
				for( int i = 0; i < screen_data_cnt(); i++ )
				{
					data.m_scr_data.Add( m_scr_data[ i ].copy() );
				}
			}
			
			// copy patterns
			if( patterns_data != null )
			{
				if( data.patterns_data == null )
				{
					data.patterns_data = new Dictionary< string, List< pattern_data > >( patterns_data.Count, patterns_data.Comparer );
				}
				else
				{
					data.patterns_data.Clear();
				}
				
				foreach( KeyValuePair< string, List< pattern_data > > entry in patterns_data )
				{
					List< pattern_data > list_data = new List< pattern_data >( entry.Value.Count );
					
					foreach( pattern_data pattern in entry.Value )
					{
						list_data.Add( pattern.copy() );
					}
					
					data.patterns_data.Add( entry.Key, list_data );
				}
			}
			
			return data;
		}

		public bool cmp_blocks( int _a, int _b )
		{
			int A_offs = _a << 2;
			int B_offs = _b << 2;
			
			for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
			{
				if( blocks[ A_offs + i ] != blocks[ B_offs + i ] )
				{
					return false;
				}
			}
			
			return true;
		}
		
		public void inc_blocks_CHRs( int _start_ind )
		{
			blocks_CHRs_proc( delegate( int _CHR_ind ) 
			{
				if( _CHR_ind >= _start_ind )
				{
					++_CHR_ind;
				}
				
				return _CHR_ind;
			} );
		}
		
		public void dec_blocks_CHRs( int _start_ind )
		{
			blocks_CHRs_proc( delegate( int _CHR_ind ) 
			{
				if( _CHR_ind > _start_ind )
				{
					--_CHR_ind;
				}
				
				return _CHR_ind;
			} );
		}
		
		private void blocks_CHRs_proc( Func< int, int > _func )
		{
			int		CHR_ind;
			uint	CHR_data;
			
			for( int i = 0; i < m_blocks.Length; i++ )
			{
				CHR_data = m_blocks[ i ];
				
				CHR_ind = get_block_CHR_id( CHR_data );
					
				CHR_ind = _func( CHR_ind );
					
				m_blocks[ i ] = set_block_CHR_id( CHR_ind, CHR_data );
			}
		}
		
		public void inc_tiles_blocks( ushort _start_ind )
		{
			tiles_blocks_proc( delegate( ushort _block_ind ) 
			{
				if( _block_ind >= _start_ind )
				{
					++_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		public void dec_tiles_blocks( ushort _start_ind )
		{
			tiles_blocks_proc( delegate( ushort _block_ind ) 
			{
				if( _block_ind > _start_ind )
				{
					--_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		private void tiles_blocks_proc( Func< ushort, ushort > _func )
		{
			ulong	tile;
			ushort	block_ind;
			
			for( int i = 0; i < m_tiles.Length; i++ )
			{
				tile = m_tiles[ i ];
				
				for( int j = 0; j < utils.CONST_TILE_SIZE; j++ )
				{
					block_ind = utils.get_ushort_from_ulong( tile, j );
					
					block_ind = _func( block_ind );
					
					tile = utils.set_ushort_to_ulong( tile, j, block_ind );
				}
				
				m_tiles[ i ] = tile;
			}
		}
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
		public static byte get_block_flags_flip( uint _block_chr_data )
		{
			return ( byte )m_map_data_config_app.unpack_data( map_data_config_base.e_data_type.VHFlip, _block_chr_data );
		}
		
		public static uint set_block_flags_flip( byte _flip_flag, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.e_data_type.VHFlip, ( int )_flip_flag, _block_chr_data );
		}
#endif
		public static int get_block_flags_palette( uint _block_chr_data )
		{
			return m_map_data_config_app.unpack_data( map_data_config_base.e_data_type.Palette, _block_chr_data );
		}

		public static uint set_block_flags_palette( int _plt_ind, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.e_data_type.Palette, _plt_ind, _block_chr_data );
		}

		public static int get_block_flags_obj_id( uint _block_chr_data )
		{
			return m_map_data_config_app.unpack_data( map_data_config_base.e_data_type.ObjId, _block_chr_data );
		}
		
		public static uint set_block_flags_obj_id( int _obj_id, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.e_data_type.ObjId, _obj_id, _block_chr_data );
		}

		public void set_block_flags_obj_id( int _block_id, int _CHR_id, int _prop_id, bool _per_block )
		{
			if( _block_id >= 0 )
			{
				int chr_data_ind;
				
				if( _per_block )
				{
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						chr_data_ind = ( _block_id << 2 ) + i;
					
						blocks[ chr_data_ind ] = set_block_flags_obj_id( _prop_id, blocks[ chr_data_ind ] );
					}
				}
				else
				{
					chr_data_ind = ( _block_id << 2 ) + _CHR_id;
				
					blocks[ chr_data_ind ] = set_block_flags_obj_id( _prop_id, blocks[ chr_data_ind ] );
				}
			}
		}
		
		public static int get_block_CHR_id( uint _block_chr_data )
		{
			return m_map_data_config_app.unpack_data( map_data_config_base.e_data_type.CHRId, _block_chr_data );
		}
		
		public static uint set_block_CHR_id( int _CHR_id, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.e_data_type.CHRId, _CHR_id, _block_chr_data );
		}
		
		public static uint block_data_repack_to_native( uint _block_chr_data, int _CHR_offset )
		{
			uint native_data = m_map_data_config_native.repack( m_map_data_config_app, _block_chr_data );
			
			bit_field bf_CHR_id = m_map_data_config_native.get_bit_field( map_data_config_base.e_data_type.CHRId );
			
			return bf_CHR_id.overwrite( bf_CHR_id.unpack( native_data ) + _CHR_offset, native_data );
		}
		
		public static void CHR_bank_vflip( byte[] _CHR_data, int _spr_ind )
		{
			byte a;
			byte b;
			
			int offset_y1;
			int offset_y2;
			
			int chr_x = _spr_ind % 16;
			int chr_y = _spr_ind >> 4;
				
			int offset_bytes = ( chr_x << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) + ( ( chr_y * utils.CONST_CHR_BANK_PAGE_SIDE ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
			
			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				for( int y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT>>1; y++ )
				{
					offset_y1 = offset_bytes + x + ( y * utils.CONST_CHR_BANK_PAGE_SIDE );
					offset_y2 = offset_bytes + x + ( ( utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - y ) * utils.CONST_CHR_BANK_PAGE_SIDE );
						
					a = _CHR_data[ offset_y1 ];
					b = _CHR_data[ offset_y2 ];
					
					_CHR_data[ offset_y1 ] = b;
					_CHR_data[ offset_y2 ] = a;
				}
			}
		}
		
		public static void CHR_bank_hflip( byte[] _CHR_data, int _spr_ind )
		{
			byte a;
			byte b;
			
			int offset_x;
			int offset_y;

			int chr_x = _spr_ind % 16;
			int chr_y = _spr_ind >> 4;
				
			int offset_bytes = ( chr_x << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) + ( ( chr_y * utils.CONST_CHR_BANK_PAGE_SIDE ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
			
			for( int y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
			{
				offset_y = y * utils.CONST_CHR_BANK_PAGE_SIDE;
				
				for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT>>1; x++ )
				{
					offset_x = offset_bytes + x + offset_y;
						
					a = _CHR_data[ offset_x ];
					b = _CHR_data[ offset_bytes + offset_y + utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - x ];
					
					_CHR_data[ offset_x ] = b;
					_CHR_data[ offset_bytes + offset_y + utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - x ] = a;
				}
			}
		}
		
		public static void CHR_bank_rotate_cw( byte[] _CHR_data, int _spr_ind )
		{
			int i;
			int j;
			
			int im8;
			int jm8;

			int chr_x = _spr_ind % 16;
			int chr_y = _spr_ind >> 4;
			
			int offset_bytes = ( chr_x << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) + ( ( chr_y * utils.CONST_CHR_BANK_PAGE_SIDE ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
			
			for( i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ ) 
			{
				im8 = offset_bytes + ( i * utils.CONST_CHR_BANK_PAGE_SIDE );
				
		        for( j = i; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ ) 
		        {
		            if( i != j ) 
		            {
		            	jm8 = offset_bytes + ( j * utils.CONST_CHR_BANK_PAGE_SIDE );
		            	
		                _CHR_data[ i + jm8 ] ^= _CHR_data[ j + im8 ];
		                _CHR_data[ j + im8 ] ^= _CHR_data[ i + jm8 ];
		                _CHR_data[ i + jm8 ] ^= _CHR_data[ j + im8 ];
		            }
		        }
		    }
			
			CHR_bank_hflip( _CHR_data, _spr_ind );
		}
		
		public static void hflip( byte[] _CHR8x8, int _offset = 0 )
		{
			byte a;
			byte b;
			
			int offset_x;
			int offset_y;
			
			for( int y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
			{
				offset_y = y << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS;
				
				for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT>>1; x++ )
				{
					offset_x = _offset + x + offset_y;
						
					a = _CHR8x8[ offset_x ];
					b = _CHR8x8[ _offset + offset_y + utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - x ];
					
					_CHR8x8[ offset_x ] = b;
					_CHR8x8[ _offset + offset_y + utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - x ] = a;
				}
			}
		}
		
		public static void vflip( byte[] _CHR8x8, int _offset = 0 )
		{
			byte a;
			byte b;
			
			int offset_y1;
			int offset_y2;
			
			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				for( int y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT>>1; y++ )
				{
					offset_y1 = _offset + x + ( y << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
					offset_y2 = _offset + x + ( ( utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - y ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
						
					a = _CHR8x8[ offset_y1 ];
					b = _CHR8x8[ offset_y2 ];
					
					_CHR8x8[ offset_y1 ] = b;
					_CHR8x8[ offset_y2 ] = a;
				}
			}
		}
		
		public static void rot_cw( byte[] _CHR8x8, int _offset = 0 )
		{
			int i;
			int j;
			
			int im8;
			int jm8;
					
			for( i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ ) 
			{
				im8 = _offset + ( i << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
				
		        for( j = i; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ ) 
		        {
		            if( i != j ) 
		            {
		            	jm8 = _offset + ( j << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
		            	
		                _CHR8x8[ i + jm8 ] ^= _CHR8x8[ j + im8 ];
		                _CHR8x8[ j + im8 ] ^= _CHR8x8[ i + jm8 ];
		                _CHR8x8[ i + jm8 ] ^= _CHR8x8[ j + im8 ];
		            }
		        }
		    }
			
			hflip( _CHR8x8, _offset );
		}
		
		public int contains_CHR( byte[] _CHR8x8, int _max_ind )
		{
			bool contains;
			
			int CHR_ind;
			int i;
			int j;
			
			int CHR_offset;
			
			int size = _max_ind < 0 ? ( m_CHR_bank.Length / utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ):Math.Min( _max_ind, m_CHR_bank.Length / utils.CONST_SPR8x8_TOTAL_PIXELS_CNT );
			
			for( CHR_ind = 0; CHR_ind < size; CHR_ind++ )
			{
				CHR_offset = ( ( CHR_ind >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( CHR_ind % 16 ) << 3 );

				contains = true;
								
				for( i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
				{
					for( j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
					{
						if( m_CHR_bank[ j + CHR_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE ] != _CHR8x8[ j + ( i << 3 ) ] )
						{
							contains = false;
						}
					}
				}
				
				if( contains == true )
				{
					return CHR_ind;
				}
			}
			
			return -1;
		}
		
		public int contains_block( uint[] _block_data, int _max_block )
		{
			int block_n;
			
			int size = _max_block < 0 ? blocks.Length:_max_block;
			
			for( block_n = 0; block_n < size; block_n += utils.CONST_BLOCK_SIZE )
			{
				if( blocks[ block_n ] == _block_data[ 0 ] && 
 				    blocks[ block_n + 1 ] == _block_data[ 1 ] && 
 				    blocks[ block_n + 2 ] == _block_data[ 2 ] && 
 				    blocks[ block_n + 3 ] == _block_data[ 3 ] )
				{
					return block_n;
				}
			}
			
			return -1;
		}

		public int contains_tile( int _max_tile_ind, ulong _tile_data, ulong _mask = 0xffffffffffffffff )
		{
			int tile_n;
			
			int size = ( _max_tile_ind < 0 || _max_tile_ind > tiles.Length ) ? tiles.Length:_max_tile_ind;
			
			for( tile_n = 0; tile_n < size; tile_n++ )
			{
				if( ( tiles[ tile_n ] & _mask ) == ( _tile_data & _mask ) )
				{
					return tile_n;
				}
			}
			
			return -1;
		}
#if DEF_ZX
		public void update_ink_paper_colors( int _chr_id, byte _ink_clr, byte _paper_clr )
		{
			// apply paper/ink color
			CHR_bank_spr8x8_change_proc( _chr_id, delegate( byte _pix ) 
			{
				if( ( ( int )_pix & 0x08 ) == 0x08 )
				{
					if( _paper_clr != 0xff )
					{
						return ( byte )_paper_clr;
					}
				}
				else
				{
					if( _ink_clr != 0xff )
					{
						return ( byte )_ink_clr;
					}
				}
				
				return _pix;
			});
		}
		
		public void get_ink_paper_colors( int _chr_id, ref byte _ink_clr, ref byte _paper_clr )
		{
			byte ink	= 0xff;
			byte paper	= 0xff;
			
			CHR_bank_spr8x8_enum_proc( _chr_id, delegate( byte _pix ) 
			{
				if( ( ( int )_pix & 0x08 ) == 0x08 )
				{
					paper = _pix;
				}
				else
				{
					ink = _pix;
				}
			});
			
			_ink_clr	= ink;
			_paper_clr	= paper;
		}
#endif		
		public void from_CHR_bank_to_spr8x8( int _chr_id, byte[] _img_buff, int _img_buff_offset = 0 )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				Array.Copy( m_CHR_bank, chr_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE, _img_buff, ( i << 3 ) + _img_buff_offset, 8 );
			}
		}
		
		public void from_spr8x8_to_CHR_bank( int _chr_id, byte[] _img_buff )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				Array.Copy( _img_buff, i << 3, m_CHR_bank, chr_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE, 8 );
			}
		}
		
		public void CHR_bank_spr8x8_change_proc( int _chr_id, Func< byte, byte > _func )
		{
			int pix_ind;
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				for( int j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
				{
					pix_ind = chr_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE + j;
					
					m_CHR_bank[ pix_ind ] = _func( m_CHR_bank[ pix_ind ] );
				}
			}
		}

		public void fill_CHR_bank_spr8x8_by_color_ind( int _chr_id, int _clr_ind )
		{
			CHR_bank_spr8x8_change_proc( _chr_id, delegate( byte _pix ) { return (byte)_clr_ind; } );
		}

		public void CHR_bank_spr8x8_enum_proc( int _chr_id, Action< byte > _func )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				for( int j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
				{
					_func( m_CHR_bank[ chr_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE + j ] );
				}
			}
		}
		
		public int spr8x8_sum( int _chr_id )
		{
			int sum = 0;
			
			CHR_bank_spr8x8_enum_proc( _chr_id, delegate( byte _pix )
			{
#if DEF_ZX
				sum += ( ( ( int )_pix & 0x08 ) == 0x08 ) ? 0:1;
#else
				sum += _pix;
#endif
			});
			
			return sum;
		}

		public int block_sum( int _block_id )
		{
			uint sum = 0;
			
			int block_offset = _block_id << 2;
			
			for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
			{
				sum += m_blocks[ block_offset + i ];
			}
			
			return ( int )sum;
		}

		public int tile_sum( int _tile_id )
		{
			int sum = 0;
			
			ulong tile_val = m_tiles[ _tile_id ];
			
			for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
			{
				sum += utils.get_ushort_from_ulong( tile_val, i );
			}
			
			return sum;
		}
		
		public int get_first_free_spr8x8_id( bool _neg_overflow )
		{
			int chr_id = 0;
			
			for( int k = platform_data.get_CHR_bank_max_sprites_cnt() - 1; k >= 0; k-- )
			{
				if( spr8x8_sum( k ) > 0 )
				{
					chr_id = k + 1;
					
					if( chr_id > platform_data.get_CHR_bank_max_sprites_cnt() - 1 )
					{
						return _neg_overflow ? -1:platform_data.get_CHR_bank_max_sprites_cnt();
					}
					
					break;
				}
			}
			
			return chr_id;
		}

		public int get_first_free_block_id( bool _neg_overflow )
		{
			int block_id = 0;
			
			for( int k = platform_data.get_max_blocks_cnt() - 1; k >= 0; k-- )
			{
				if( block_sum( k ) > 0 )
				{
					block_id = k + 1;
					
					if( block_id > platform_data.get_max_blocks_cnt() - 1 )
					{
						return _neg_overflow ? -1:platform_data.get_max_blocks_cnt();
					}
					
					break;
				}
			}
			
			return block_id;
		}

		public int get_first_free_tile_id( bool _neg_overflow )
		{
			int tile_id = 0;
			
			for( int k = platform_data.get_max_tiles_cnt() - 1; k >= 0; k-- )
			{
				if( m_tiles[ k ] > 0 )
				{
					tile_id = k + 1;
					
					if( tile_id > platform_data.get_max_tiles_cnt() - 1 )
					{
						return _neg_overflow ? -1:platform_data.get_max_tiles_cnt();
					}
					
					break;
				}
			}
			
			return tile_id;
		}
		
		public int get_tile_usage( ushort _tile_ind, data_sets_manager.e_screen_data_type _scr_type )
		{
			int res = 0;
			
			if( _scr_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				int scr_n;
				int tile_n;
				
				for( scr_n = 0; scr_n < screen_data_cnt(); scr_n++ )
				{
					for( tile_n = 0; tile_n < platform_data.get_screen_tiles_cnt(); tile_n++ )
					{
						if( m_scr_data[ scr_n ].get_tile( tile_n ) == _tile_ind )
						{
							++res;
						}
					}
				}
			}
			
			return res;
		}

		public int get_block_usage( ushort _block_ind, data_sets_manager.e_screen_data_type _scr_type )
		{
			int res = 0;
			
			int scr_n;
			int tile_n;
			int block_n;
			
			if( _scr_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				for( tile_n = 0; tile_n < platform_data.get_max_tiles_cnt(); tile_n++ )
				{
					for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
					{
						if( utils.get_ushort_from_ulong( m_tiles[ tile_n ], block_n ) == _block_ind )
						{
							++res;
						}
					}
				}
			}
			else
			{
				for( scr_n = 0; scr_n < screen_data_cnt(); scr_n++ )
				{
					for( block_n = 0; block_n < platform_data.get_screen_blocks_cnt(); block_n++ )
					{
						if( m_scr_data[ scr_n ].get_tile( block_n ) == _block_ind )
						{
							++res;
						}
					}
				}
			}
			
			return res;
		}
		
		public int screen_data_cnt()
		{
			return m_scr_data.Count;
		}
		
		public void set_screen_tile( int _scr_ind, int _ind, ushort _val )
		{
			// check all arguments out of the method(!)
			m_scr_data[ _scr_ind ].set_tile( _ind, _val );
		}

		public ushort get_screen_tile( int _scr_ind, int _ind )
		{
			// check all arguments out of the method(!)
			return m_scr_data[ _scr_ind ].get_tile( _ind );
		}

		public screen_data get_screen_data( int _scr_ind )
		{
			// check all arguments out of the method(!)
			return m_scr_data[ _scr_ind ];
		}
		
		public void screen_data_replace( int _scr_ind, screen_data _data )
		{
			// check all arguments out of the method(!)
			m_scr_data[ _scr_ind ].reset();
			m_scr_data[ _scr_ind ] = _data;
		}
		
		public screen_data create_screen( data_sets_manager.e_screen_data_type _type )
		{
			screen_data scr = new screen_data( _type );
			
			m_scr_data.Add( scr );
			
			return scr;
		}
		
		public void copy_screen( int _id )
		{
			m_scr_data.Add( m_scr_data[ _id ].copy() );
		}
		
		public void delete_screen( int _ind )
		{
			m_scr_data.RemoveAt( _ind );
		}		
		
		public void inc_screen_tiles( ushort _start_ind )
		{
			screen_tiles_proc( delegate( ushort _tile_ind ) 
			{
				if( _tile_ind >= _start_ind )
				{
					++_tile_ind;
				}
				
				return _tile_ind;
			} );
		}
		
		public void dec_screen_tiles( ushort _start_ind )
		{
			screen_tiles_proc( delegate( ushort _tile_ind ) 
			{
				if( _tile_ind > _start_ind )
				{
					--_tile_ind;
				}
				
				return _tile_ind;
			} );
		}
		
		private void screen_tiles_proc( Func< ushort, ushort > _func )
		{
			screen_data data;
				
			for( int i = 0; i < screen_data_cnt(); i++ )
			{
				data = m_scr_data[ i ];
				
				for( int j = 0; j < data.length; j++ )
				{
					data.set_tile( j, _func( data.get_tile( j ) ) );
				}
			}
		}

		public void inc_screen_blocks( ushort _start_ind )
		{
			screen_tiles_proc( delegate( ushort _block_ind ) 
			{
				if( _block_ind >= _start_ind )
				{
					++_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		public void dec_screen_blocks( ushort _start_ind )
		{
			screen_tiles_proc( delegate( ushort _block_ind ) 
			{
				if( _block_ind > _start_ind )
				{
					--_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
#if DEF_NES
		public long export_CHR( BinaryWriter _bw, bool _save_padding = false )
#elif DEF_SMS
		public long export_CHR( BinaryWriter _bw, int _bpp )
#elif DEF_PCE || DEF_ZX || DEF_SMD
		public long export_CHR( BinaryWriter _bw )
#endif			
		{
			int i;
			int x;
			int y;
			int val;
			int x_pos;

#if DEF_SMD
			int _bpp = 4;
#endif
			
#if DEF_NES || DEF_SMS || DEF_SMD
			int j;
			byte data;
#elif DEF_PCE
			int j;
			ushort data;
#elif DEF_ZX
			byte data;
#endif
#if DEF_SMS || DEF_SMD
			int max_clr_ind = ( 2 << ( _bpp - 1 ) ) - 1;
#endif			
			int num_CHR_sprites = get_first_free_spr8x8_id( false );
			
			for( i = 0; i < num_CHR_sprites; i++ )
			{
				from_CHR_bank_to_spr8x8( i, utils.tmp_spr8x8_buff, 0 );
#if DEF_NES				
				for( j = 0; j < 2; j++ )
				{
					for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
					{
						data = 0;
						
						for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
						{
							x_pos = 7 - x;
							
							val = utils.tmp_spr8x8_buff[ ( y << 3 ) + x_pos ];
							
							data |= ( byte )( ( ( val >> j ) & 0x01 ) << x );
						}
						
						_bw.Write( data );
					}
				}
#elif DEF_SMS || DEF_SMD
				for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
				{
					for( j = 0; j < _bpp; j++ )
					{
						data = 0;
						
						for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
						{
							x_pos = 7 - x;
							
							val = utils.tmp_spr8x8_buff[ ( y << 3 ) + x_pos ];
							
							val = ( val > max_clr_ind ) ? 0:val;
							
							data |= ( byte )( ( ( val >> j ) & 0x01 ) << x );
						}
						
						_bw.Write( data );
					}
				}
#elif DEF_PCE
				for( j = 0; j < 2; j++ )
				{
					for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
					{
						data = 0;
						
						for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
						{
							x_pos = 7 - x;
							
							val = utils.tmp_spr8x8_buff[ ( y << 3 ) + x_pos ];
							
							val = ( val >> ( j << 1 ) ) & 0x03;
							
							data |= ( ushort )( ( ( val & 0x01 ) << x ) | ( ( ( val >> 1 ) & 0x01 ) << ( 8 + x ) ) );
						}
						
						_bw.Write( data );
					}
				}
#elif DEF_ZX
				for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
				{
					data = 0;
					
					for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
					{
						x_pos = 7 - x;
						
						val = utils.tmp_spr8x8_buff[ ( y << 3 ) + x_pos ];
						
						data |= ( byte )( ( ( val & 0x08 ) != 0 ) ? 0:( 1 << x ) );
					}
					
					_bw.Write( data );
				}
#endif				
			}
#if DEF_NES			
			// save padding data to 1/2/4 KB
			if( _save_padding )
			{
				long padding 	= 0;
				long data_size 	= _bw.BaseStream.Length;
				
				if( data_size < 1024 )
				{
					padding = 1024 - data_size; 
				}
				else
				if( data_size < 2048 )
				{
					padding = 2048 - data_size;
				}
				else
				if( data_size < 4096 )
				{
					padding = 4096 - data_size;
				}
				
				if( padding != 0 )
				{
					data = 0;
					
					while( padding-- != 0 )
					{
						_bw.Write( data );
					}
				}
			}
#endif			
			return _bw.BaseStream.Length;
		}

		public static ulong tile_ulong_reverse( ulong _tile64 )
		{
			ulong tile_rev = 0;
			
			for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
			{
				tile_rev = utils.set_ushort_to_ulong( tile_rev, i, ( ushort )( ( _tile64 >> ( 16 * i ) ) & 0xffff ) );
			}
			
			return tile_rev;
		}
		
		public static uint tile_convert_ulong_to_uint_reverse( ulong _tile64 )
		{
			byte v0 = ( byte )( ( _tile64 >> 48 ) & 0xff );
			byte v1 = ( byte )( ( _tile64 >> 32 ) & 0xff );
			byte v2 = ( byte )( ( _tile64 >> 16 ) & 0xff );
			byte v3 = ( byte )( _tile64 & 0xff );
			
			return unchecked( ( uint )( v3 << 24 | v2 << 16 | v1 << 8 | v0 ) );
		}
		
		public static ulong tile_convert_uint_to_ulong( uint _tile32 )
		{
			ulong tile_data = 0;
			
			for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
			{
				tile_data = utils.set_ushort_to_ulong( tile_data, utils.CONST_TILE_SIZE - i - 1, ( ushort )( ( _tile32 >> ( 8 * i ) ) & 0xff ) );
			}
			
			return tile_data;
		}
		
		public static ulong set_tile_data( ushort _b0, ushort _b1, ushort _b2, ushort _b3 )
		{
			return ( ( ulong )_b0 << 48 | ( ulong )_b1 << 32 | ( ulong )_b2 << 16 | ( ulong )_b3 );
		}
		
		public void save_tiles_patterns( BinaryWriter _bw )
		{
			_bw.Write( m_patterns_data.Keys.Count );
			
			foreach( string key in m_patterns_data.Keys ) 
			{ 
				_bw.Write( key );
				
				List< pattern_data > pattrn_list = m_patterns_data[ key ] as List< pattern_data >;
				
				_bw.Write( pattrn_list.Count );
				
				pattrn_list.ForEach( delegate( pattern_data _pattern ) { _pattern.save( _bw ); } ); 
			}
		}

		public void load_tiles_patterns( BinaryReader _br, byte _ver )
		{
			List< pattern_data > pattrn_list;
			
			int patterns_cnt;
			int grps_cnt = _br.ReadInt32();
			
			string grp_name;
			
			m_patterns_data.Clear();
			
			for( int grp_n = 0; grp_n < grps_cnt; grp_n++ )
			{
				grp_name = _br.ReadString();

				pattrn_list = new List< pattern_data >();
				m_patterns_data.Add( grp_name, pattrn_list );
				
				patterns_cnt = _br.ReadInt32();
				
				for( int pattrn_n = 0; pattrn_n < patterns_cnt; pattrn_n++ )
				{
					pattrn_list.Add( pattern_data.load( _ver, _br ) );
				}
			}
		}
		
		public void save( BinaryWriter _bw, data_sets_manager.e_screen_data_type _scr_type )
		{
			int i;
			int size;
			
			_bw.Write( m_CHR_bank );

			// save palettes
			{
				_bw.Write( m_palettes.Count );
				
				for( i = 0; i < m_palettes.Count; i++ )
				{
					utils.write_int_arr( _bw, m_palettes[ i ].m_palette0 );
					utils.write_int_arr( _bw, m_palettes[ i ].m_palette1 );
					utils.write_int_arr( _bw, m_palettes[ i ].m_palette2 );
					utils.write_int_arr( _bw, m_palettes[ i ].m_palette3 );
				}
			}
			
			size = m_blocks.Length;
			_bw.Write( ( ushort )size );
			
			for( i = 0; i < size; i++ )
			{
				_bw.Write( m_blocks[ i ] );
			}

			if( _scr_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
			{
				size = m_tiles.Length;
				_bw.Write( ( ushort )size );
				
				for( i = 0; i < size; i++ )
				{
					_bw.Write( m_tiles[ i ] );
				}
			}
			
			// save screens data
			_bw.Write( screen_data_cnt() );
			
			for( i = 0; i < screen_data_cnt(); i++ )
			{
				m_scr_data[ i ].save( _bw, false );
			}
		}
		
		public void load( BinaryReader _br, project_data_desc _prj_data, data_sets_manager.e_screen_data_type _scr_type )
		{
			int i;
			uint val;

			platform_data.e_platform_type	prj_platform	= platform_data.get_platform_type_by_file_ext( _prj_data.m_file_ext );
			i_project_data_converter	data_converter	= project_data_converter_provider.get_converter();
			
			data_converter.load_CHR_bank( _br, _prj_data.m_ver, prj_platform, ref m_CHR_bank );

			palettes_clear_arr();
			data_converter.load_palettes( _br, _prj_data.m_ver, prj_platform, _prj_data.m_file_ext, this );
			
			// block 2x2 data
			{
				data_converter.pre_load_block_data( prj_platform );
				
				int blocks_cnt = ( _prj_data.m_ver <= 5 ) ? ( platform_data.get_max_blocks_cnt_by_file_ext( _prj_data.m_file_ext ) * utils.CONST_BLOCK_SIZE ):( int )_br.ReadUInt16();
#if DEF_PLATFORM_16BIT
				if( blocks_cnt > m_blocks.Length )
				{
					Array.Resize( ref m_blocks, blocks_cnt );
				}
#endif
				int read_blocks_cnt	= Math.Min( m_blocks.Length, blocks_cnt );
				
				for( i = 0; i < read_blocks_cnt; i++ )
				{
					if( _prj_data.m_ver <= 2 )
					{
						val = _br.ReadUInt16();
					}
					else
					{
						val = _br.ReadUInt32();
					}
					
					m_blocks[ i ] = data_converter.convert_block_data( _prj_data.m_ver, prj_platform, i, val );
				}
				
				int skip_data_size = blocks_cnt - m_blocks.Length;
				
				if( skip_data_size > 0 )
				{
					if( _prj_data.m_ver <= 2 )
					{
						_br.ReadBytes( skip_data_size * sizeof( UInt16 ) );
					}
					else
					{
						_br.ReadBytes( skip_data_size * sizeof( UInt32 ) ); 
					}
				}
				
				data_converter.post_load_block_data( this );
			}
			
			// tiles 4x4 data
			{
				if( _scr_type == data_sets_manager.e_screen_data_type.Tiles4x4 )
				{
					ulong tile_data;
					
					ushort b0;
					ushort b1;
					ushort b2;
					ushort b3;
					
					int tiles_cnt = ( _prj_data.m_ver <= 5 ) ? ( platform_data.get_max_tiles_cnt_by_file_ext( _prj_data.m_file_ext ) ):( int )_br.ReadUInt16();
#if DEF_PLATFORM_16BIT
					if( tiles_cnt > m_tiles.Length )
					{
						Array.Resize( ref m_tiles, tiles_cnt );
					}
#endif
					int read_tiles_cnt	= Math.Min( m_tiles.Length, tiles_cnt );
					read_tiles_cnt		= ( _prj_data.m_ver <= 5 ) ? 256:read_tiles_cnt;
					
					int blocks_cnt = m_blocks.Length >> 2;
					
					for( i = 0; i < read_tiles_cnt; i++ )
					{
						if( _prj_data.m_ver <= 5 )
						{
							m_tiles[ i ] = tile_convert_uint_to_ulong( _br.ReadUInt32() );
						}
						else
						{
							tile_data = _br.ReadUInt64();
							
							b0 = ( ushort )( ( tile_data >> 48 ) & 0xffff );
							b1 = ( ushort )( ( tile_data >> 32 ) & 0xffff );
							b2 = ( ushort )( ( tile_data >> 16 ) & 0xffff );
							b3 = ( ushort )( tile_data & 0xffff );
							
							if( b0 >= blocks_cnt )
							{
								b0 = 0;
							}

							if( b1 >= blocks_cnt )
							{
								b1 = 0;
							}

							if( b2 >= blocks_cnt )
							{
								b2 = 0;
							}

							if( b3 >= blocks_cnt )
							{
								b3 = 0;
							}
							
							m_tiles[ i ] = set_tile_data( b0, b1, b2, b3 ); 
						}
					}
					
					int skip_data_size = tiles_cnt - m_tiles.Length;
					
					if( skip_data_size > 0 )
					{
						if( _prj_data.m_ver <= 5 )
						{
							_br.ReadBytes( skip_data_size * sizeof( UInt32 ) );
						}
						else
						{
							_br.ReadBytes( skip_data_size * sizeof( UInt64 ) );
						}
					}
				}
			}
			
			// load screens data
			data_converter.load_screens( _br, _prj_data, _scr_type, this );
		}
	}
}
