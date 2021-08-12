/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
#elif DEF_PCE
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
		
		private byte[] m_CHR_bank	= new byte[ utils.CONST_CHR_BANK_PAGE_SIZE * utils.CONST_CHR_BANK_PAGES_CNT ];
		
		[DataMember]
		private List< palette16_data > m_palettes	= null;
		private int	m_palette_pos					= -1;
		
		// NES: [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)
		// SMS: [ property_id ](4) [ hv_flip ](2) [ palette_ind ](1) [CHR ind](9)
		// PCE: [ property_id ](4) [ palette ind ](4) [CHR ind](12)
		private uint[] m_blocks	= new uint[ utils.CONST_BLOCKS_UINT_SIZE ];
		private uint[] m_tiles	= new uint[ utils.CONST_TILES_UINT_SIZE ];
		
		[DataMember]
		private List< screen_data >	m_scr_data	= null;
		
		private readonly Dictionary< string, List< pattern_data > >	m_patterns_data	= null;	// key = group name / value = List< pattern_data >

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
		public uint[] tiles
		{
			get { return m_tiles; }
			set {}
		}
		
		public tiles_data( bool _temp_data = false )
		{
			if( !_temp_data )
			{
				++m_id;
				name = m_id.ToString();
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

#if DEF_FIXED_LEN_PALETTE16_ARR
		private void palettes_create_fixed_arr()
		{
			m_palettes = new List< palette16_data >( utils.CONST_PALETTE16_ARR_LEN );
			
			for( int i = 0; i < utils.CONST_PALETTE16_ARR_LEN; i++ )
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
			Array.Clear( m_tiles, 0, utils.CONST_TILES_UINT_SIZE );
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
		
		public byte get_tile_block( int _tile_id, int _block_n )
		{
			return utils.get_byte_from_uint( m_tiles[ _tile_id ], _block_n );
		}
		
		public uint set_tile_block( int _tile_id, int _block_n, byte _val )
		{
			uint tile = utils.set_byte_to_uint( m_tiles[ _tile_id ], _block_n, _val );
			
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
			
			Array.Copy( m_blocks,	data.m_blocks,	utils.CONST_BLOCKS_UINT_SIZE );
			Array.Copy( m_tiles,	data.m_tiles,	utils.CONST_TILES_UINT_SIZE );

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
		
		public void inc_tiles_blocks( byte _start_ind )
		{
			tiles_blocks_proc( delegate( byte _block_ind ) 
			{
				if( _block_ind >= _start_ind )
				{
					++_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		public void dec_tiles_blocks( byte _start_ind )
		{
			tiles_blocks_proc( delegate( byte _block_ind ) 
			{
				if( _block_ind > _start_ind )
				{
					--_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		private void tiles_blocks_proc( Func< byte, byte > _func )
		{
			uint tile;
			byte block_ind;
			
			for( int i = 0; i < m_tiles.Length; i++ )
			{
				tile = m_tiles[ i ];
				
				for( int j = 0; j < utils.CONST_TILE_SIZE; j++ )
				{
					block_ind = utils.get_byte_from_uint( tile, j );
					
					block_ind = _func( block_ind );
					
					tile = utils.set_byte_to_uint( tile, j, block_ind );
				}
				
				m_tiles[ i ] = tile;
			}
		}
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
		public static byte get_block_flags_flip( uint _block_chr_data )
		{
			return ( byte )m_map_data_config_app.unpack_data( map_data_config_base.EData.ed_VH_Flip, _block_chr_data );
		}
		
		public static uint set_block_flags_flip( byte _flip_flag, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.EData.ed_VH_Flip, ( int )_flip_flag, _block_chr_data );
		}
#endif
		public static int get_block_flags_palette( uint _block_chr_data )
		{
			return m_map_data_config_app.unpack_data( map_data_config_base.EData.ed_Palette, _block_chr_data );
		}

		public static uint set_block_flags_palette( int _plt_ind, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.EData.ed_Palette, _plt_ind, _block_chr_data );
		}

		public static int get_block_flags_obj_id( uint _block_chr_data )
		{
			return m_map_data_config_app.unpack_data( map_data_config_base.EData.ed_Obj_id, _block_chr_data );
		}
		
		public static uint set_block_flags_obj_id( int _obj_id, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.EData.ed_Obj_id, _obj_id, _block_chr_data );
		}
		
		public static int get_block_CHR_id( uint _block_chr_data )
		{
			return m_map_data_config_app.unpack_data( map_data_config_base.EData.ed_CHR_id, _block_chr_data );
		}
		
		public static uint set_block_CHR_id( int _CHR_id, uint _block_chr_data )
		{
			return m_map_data_config_app.pack_data( map_data_config_base.EData.ed_CHR_id, _CHR_id, _block_chr_data );
		}
		
		public static uint block_data_repack_to_native( uint _block_chr_data, int _CHR_offset )
		{
			uint native_data = m_map_data_config_native.repack( m_map_data_config_app, _block_chr_data );
			
			bit_field bf_CHR_id = m_map_data_config_native.get_bit_field( map_data_config_base.EData.ed_CHR_id );
			
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
#if DEF_SCREEN_HEIGHT_7d5_TILES
		public int contains_tile( int _max_tile_ind, uint _tile_data, bool half_tile )
#else
		public int contains_tile( int _max_tile_ind, uint _tile_data )
#endif
		{
			int tile_n;
			
			int size = ( _max_tile_ind < 0 || _max_tile_ind > tiles.Length ) ? tiles.Length:_max_tile_ind;
			
			for( tile_n = 0; tile_n < size; tile_n++ )
			{
#if DEF_SCREEN_HEIGHT_7d5_TILES				
				if( half_tile )
				{
					if( ( tiles[ tile_n ] & 0xffff0000 ) == ( _tile_data & 0xffff0000 ) )
					{
						return tile_n;
					}
				}
				else
#endif
				if( tiles[ tile_n ] == _tile_data )
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
			
			uint tile_val = m_tiles[ _tile_id ];
			
			for( int i = 0; i < utils.CONST_TILE_SIZE; i++ )
			{
				sum += utils.get_byte_from_uint( tile_val, i );
			}
			
			return sum;
		}
		
		public int get_first_free_spr8x8_id()
		{
			int chr_id = 0;
			
			for( int k = utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1; k >= 0; k-- )
			{
				if( spr8x8_sum( k ) > 0 )
				{
					chr_id = k + 1;
					
					if( chr_id > utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1 )
					{
						return -1;
					}
					
					break;
				}
			}
			
			return chr_id;
		}

		public int get_first_free_block_id()
		{
			int block_id = 0;
			
			for( int k = utils.CONST_MAX_BLOCKS_CNT - 1; k >= 0; k-- )
			{
				if( block_sum( k ) > 0 )
				{
					block_id = k + 1;
					
					if( block_id > utils.CONST_MAX_BLOCKS_CNT - 1 )
					{
						return -1;
					}
					
					break;
				}
			}
			
			return block_id;
		}

		public int get_first_free_tile_id()
		{
			int tile_id = 0;
			
			for( int k = utils.CONST_MAX_TILES_CNT - 1; k >= 0; k-- )
			{
				if( m_tiles[ k ] > 0 )
				{
					tile_id = k + 1;
					
					if( tile_id > utils.CONST_MAX_TILES_CNT - 1 )
					{
						return -1;
					}
					
					break;
				}
			}
			
			return tile_id;
		}
		
		public int get_tile_usage( ushort _tile_ind, data_sets_manager.EScreenDataType _scr_type )
		{
			int res = 0;
			
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int scr_n;
				int tile_n;
				
				for( scr_n = 0; scr_n < screen_data_cnt(); scr_n++ )
				{
					for( tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
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

		public int get_block_usage( ushort _block_ind, data_sets_manager.EScreenDataType _scr_type )
		{
			int res = 0;
			
			int scr_n;
			int tile_n;
			int block_n;
			
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				for( tile_n = 0; tile_n < utils.CONST_MAX_TILES_CNT; tile_n++ )
				{
					for( block_n = 0; block_n < utils.CONST_TILE_SIZE; block_n++ )
					{
						if( utils.get_byte_from_uint( m_tiles[ tile_n ], block_n ) == _block_ind )
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
					for( block_n = 0; block_n < utils.CONST_SCREEN_BLOCKS_CNT; block_n++ )
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
		
		public void create_screen( data_sets_manager.EScreenDataType _type )
		{
			m_scr_data.Add( new screen_data( _type ) );
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
#elif DEF_PCE || DEF_ZX
		public long export_CHR( BinaryWriter _bw )
#endif			
		{
			int i;
			int x;
			int y;
			int val;

#if DEF_NES || DEF_SMS
			int j;
			byte data;
#elif DEF_PCE
			int j;
			ushort data;
			int x_pos;
#elif DEF_ZX
			byte data;
			int x_pos;
#endif
#if DEF_SMS
			int max_clr_ind = ( 2 << ( _bpp - 1 ) ) - 1;
#endif			
			int num_CHR_sprites = get_first_free_spr8x8_id();
			
			num_CHR_sprites = num_CHR_sprites < 0 ? utils.CONST_CHR_BANK_MAX_SPRITES_CNT:num_CHR_sprites;
			
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
							val = utils.tmp_spr8x8_buff[ ( y << 3 ) + ( 7 - x ) ];
							
							data |= ( byte )( ( ( val >> j ) & 0x01 ) << x );
						}
						
						_bw.Write( data );
					}
				}
#elif DEF_SMS
				for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
				{
					for( j = 0; j < _bpp; j++ )
					{
						data = 0;
						
						for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
						{
							val = utils.tmp_spr8x8_buff[ ( y << 3 ) + ( 7 - x ) ];
							
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

		public void load_tiles_patterns( byte _ver, BinaryReader _br )
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
		
		public void save( BinaryWriter _bw, data_sets_manager.EScreenDataType _scr_type )
		{
			int i;
			
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
			
			for( i = 0; i < utils.CONST_BLOCKS_UINT_SIZE; i++ )
			{
				_bw.Write( m_blocks[ i ] );
			}

			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
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
		
		public void load( byte _ver, BinaryReader _br, string _file_ext, data_conversion_options_form.EScreensAlignMode _scr_align_mode, data_sets_manager.EScreenDataType _scr_type )
		{
			int i;
			uint val;

			utils.EPlatformType			prj_platform	= platform_data_provider.get_platform_by_ext( _file_ext );
			i_project_data_converter	data_converter	= project_data_converter_provider.get_converter();
			
			data_converter.load_CHR_bank( _ver, _br, prj_platform, ref m_CHR_bank );

			palettes_clear_arr();
			data_converter.load_palettes( _ver, _br, prj_platform, _file_ext, this );
			
			// block 2x2 data
			{
				data_converter.pre_load_block_data( prj_platform );
				
				for( i = 0; i < utils.CONST_BLOCKS_UINT_SIZE; i++ )
				{
					if( _ver <= 2 )
					{
						val = _br.ReadUInt16();
					}
					else
					{
						val = _br.ReadUInt32();
					}
					
					m_blocks[ i ] = data_converter.convert_block_data( _ver, prj_platform, i, val );
				}
				
				data_converter.post_load_block_data( this );
			}
			
			// tiles 4x4 data
			{
				if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
				{
					for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
					{
						m_tiles[ i ] = _br.ReadUInt32();
					}
				}
			}
			
			// load screens data
			int scr_cnt = _br.ReadInt32();
			
			screen_data scr;
			
			int loaded_scr_data_len = platform_data_provider.get_scr_tiles_cnt_by_file_ext( _file_ext ) * ( _scr_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 ? 4:1 );
			int scr_data_len 		= utils.get_screen_tiles_cnt_uni( _scr_type );
			int num_width_tiles 	= utils.get_screen_num_width_tiles_uni( _scr_type );
			
			int data_diff_half = 0;
			
			Action< int > skip_data = data_len => { if( _ver < 5 ) { _br.ReadBytes( data_len ); } else { _br.ReadBytes( data_len << 1 ); } };
			
			for( i = 0; i < scr_cnt; i++ )
			{
				scr = new screen_data( _scr_type );
				
				if( loaded_scr_data_len != scr_data_len )
				{
					switch( _scr_align_mode )
					{
						case data_conversion_options_form.EScreensAlignMode.sam_Top:
							{
								if( scr_data_len > loaded_scr_data_len )
								{
									scr.load( _ver, _br, loaded_scr_data_len, -1 );
								}
								else
								{
									scr.load( _ver, _br, scr_data_len, -1 );
									
									// skip the rest
									skip_data( loaded_scr_data_len - scr_data_len );
								}
							}
							break;
							
						case data_conversion_options_form.EScreensAlignMode.sam_Center:
							{
								if( scr_data_len > loaded_scr_data_len )
								{
									scr.load( _ver, _br, loaded_scr_data_len, ( ( ( ( scr_data_len - loaded_scr_data_len ) / num_width_tiles ) >> 1 ) * num_width_tiles ) );
								}
								else
								{
									data_diff_half = ( ( ( loaded_scr_data_len - scr_data_len ) / num_width_tiles ) >> 1 ) * num_width_tiles;
									
									// skip the first data
									skip_data( data_diff_half );
	
									scr.load( _ver, _br, scr_data_len, -1 );
									
									// skip the rest
									skip_data( ( data_diff_half == 0 ) ? num_width_tiles:data_diff_half );
								}
							}
							break;
							
						case data_conversion_options_form.EScreensAlignMode.sam_Bottom:
							{
								if( scr_data_len > loaded_scr_data_len )
								{
									scr.load( _ver, _br, loaded_scr_data_len, ( scr_data_len - loaded_scr_data_len ) );
								}
								else
								{
									// skip the first data
									skip_data( loaded_scr_data_len - scr_data_len );
	
									scr.load( _ver, _br, scr_data_len, -1 );
								}
							}
							break;
					}
				}
				else
				{
					scr.load( _ver, _br, scr_data_len, -1 );
				}
				
				m_scr_data.Add( scr );
			}
		}
	}
}
