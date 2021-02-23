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
		private static uint CONST_OBJ_ID_MASK	= 0x0f;
		
		// NES
		private static uint CONST_NES_PALETTE_MASK			= 0x03;
		private static uint CONST_NES_BLOCK_PALETTE_MASK	= ( CONST_NES_PALETTE_MASK << 10 );
		private static uint CONST_NES_BLOCK_CHR_ID_MASK		= 0x000000ff; 
		
		// SMS
		private static uint CONST_SMS_PALETTE_MASK			= 0x01;
		private static uint CONST_SMS_BLOCK_PALETTE_MASK	= ( CONST_SMS_PALETTE_MASK << 9 );
		private static uint CONST_SMS_BLOCK_CHR_ID_MASK		= 0x000001ff;
		
		// NES/SMS
		private static uint CONST_NES_SMS_BLOCK_OBJ_ID_MASK	= ( CONST_OBJ_ID_MASK << 12 );
		
		// PCE
		private static uint CONST_PCE_PALETTE_MASK			= 0x0f;
		private static uint CONST_PCE_BLOCK_PALETTE_MASK	= ( CONST_PCE_PALETTE_MASK << 12 );
		private static uint CONST_PCE_BLOCK_OBJ_ID_MASK		= ( CONST_OBJ_ID_MASK << 16 );
		private static uint CONST_PCE_BLOCK_CHR_ID_MASK			= 0x00000fff;		
		
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
		private static uint CONST_FLIP_MASK			= 0x03;
		private static uint CONST_BLOCK_FLIP_MASK	= ( CONST_FLIP_MASK << 10 );
#endif
#if DEF_NES
		private static uint CONST_PALETTE_MASK			= CONST_NES_PALETTE_MASK;
		private static uint CONST_BLOCK_PALETTE_MASK	= CONST_NES_BLOCK_PALETTE_MASK;
#elif DEF_SMS
		private static uint CONST_PALETTE_MASK			= CONST_SMS_PALETTE_MASK;
		private static uint CONST_BLOCK_PALETTE_MASK	= CONST_SMS_BLOCK_PALETTE_MASK;
#elif DEF_PCE
		private static uint CONST_PALETTE_MASK			= CONST_PCE_PALETTE_MASK;
		private static uint CONST_BLOCK_PALETTE_MASK	= CONST_PCE_BLOCK_PALETTE_MASK;
#endif
#if DEF_PCE
		private static uint CONST_BLOCK_OBJ_ID_MASK	= CONST_PCE_BLOCK_OBJ_ID_MASK;
#else
		private static uint CONST_BLOCK_OBJ_ID_MASK	= CONST_NES_SMS_BLOCK_OBJ_ID_MASK;
#endif
		private static int m_id	= -1;
		
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
		
		private List< byte[] >	m_scr_data	= null;
		
		private Dictionary< string, List< pattern_data > >	m_patterns_data	= null;	// key = group name / value = List< pattern_data >

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
		public List< byte[] > scr_data
		{
			get { return m_scr_data; }
			set {}
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
		
		public tiles_data()
		{
			++m_id;
			
			name = m_id.ToString();
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			palettes_create_fixed_arr();
#else
			m_palettes = new List< palette16_data >( 16 );
			m_palettes.Add( new palette16_data() );

			m_palette_pos = 0;
#endif
			
			m_scr_data = new List< byte[] >( 100 );
			
			m_patterns_data	= new Dictionary< string, List< pattern_data > >( 10 );
			m_patterns_data.Add( "MAIN", new List< pattern_data >() );
		}
		
		public void destroy()
		{
			--m_id;
			
			m_CHR_bank 	= null;

			palettes_clear_arr();
			
			m_blocks 	= null;
			m_tiles 	= null;
			
			m_scr_data.Clear();
			
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
		
		public void dec_patterns_tiles( byte _start_ind )
		{
			patterns_tiles_proc( delegate( byte _tile_ind ) 
			{
				if( _tile_ind >= _start_ind && _tile_ind > 0 )
				{
					--_tile_ind;
				}
				
				return _tile_ind;
			} );
		}

		public void inc_patterns_tiles( byte _start_ind )
		{
			patterns_tiles_proc( delegate( byte _tile_ind ) 
			{
				if( _tile_ind >= _start_ind && _tile_ind > 0 )
				{
					++_tile_ind;
				}
				
				return _tile_ind;
			} );
		}
		
		public void patterns_tiles_proc( Func< byte, byte > _func )
		{
			byte tile_id;
			
			foreach( string key in patterns_data.Keys )
			{ 
				( patterns_data[ key ] as List< pattern_data > ).ForEach( delegate( pattern_data _pattern )
				{ 
					if( _pattern.data != null )
					{
						for( int tile_ind = 0; tile_ind < _pattern.data.Length; tile_ind++ )
						{
							tile_id = _pattern.data[ tile_ind ];
							
							tile_id = _func( tile_id );
							
							_pattern.data[ tile_ind ] = tile_id;
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
		
		public tiles_data copy( data_sets_manager.EScreenDataType _scr_type )
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
				byte[] screen_data = null; 
					
				for( int i = 0; i < scr_data.Count; i++ )
				{
					screen_data = scr_alloc_data( _scr_type );
					
					Array.Copy( m_scr_data[ i ], 0, screen_data, 0, screen_data.Length );
					
					data.m_scr_data.Add( screen_data );
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
			return (byte)( ( _block_chr_data & CONST_BLOCK_FLIP_MASK ) >> 10 );
		}
		
		public static uint set_block_flags_flip( byte _flip_flag, uint _block_chr_data )
		{
			uint var = _block_chr_data;
			
			var &= ~CONST_BLOCK_FLIP_MASK;
			var |= ( ( _flip_flag & CONST_FLIP_MASK ) << 10 );
			
			return var;
		}
#endif
		public static int get_block_flags_palette( uint _block_chr_data )
		{
#if DEF_NES
			return ( int )( ( _block_chr_data & CONST_BLOCK_PALETTE_MASK ) >> 10 );
#elif DEF_SMS
			return ( int )( ( _block_chr_data & CONST_BLOCK_PALETTE_MASK ) >> 9 );
#elif DEF_PCE
			return ( int )( ( _block_chr_data & CONST_BLOCK_PALETTE_MASK ) >> 12 );
#endif
		}

		public static uint set_block_flags_palette( int _plt_ind, uint _block_chr_data )
		{
			uint var = _block_chr_data;
			
			var &= ~CONST_BLOCK_PALETTE_MASK;
#if DEF_NES
			var |= ( uint )( ( _plt_ind & CONST_PALETTE_MASK ) << 10 );
#elif DEF_SMS
			var |= ( uint )( ( _plt_ind & CONST_PALETTE_MASK ) << 9 );
#elif DEF_PCE
			var |= ( uint )( ( _plt_ind & CONST_PALETTE_MASK ) << 12 );
#endif
			return var;
		}

		public static int get_block_flags_obj_id( uint _block_chr_data )
		{
#if DEF_PCE
			return ( int )( ( _block_chr_data & CONST_BLOCK_OBJ_ID_MASK ) >> 16 );
#else
			return ( int )( ( _block_chr_data & CONST_BLOCK_OBJ_ID_MASK ) >> 12 );
#endif
		}
		
		public static uint set_block_flags_obj_id( int _obj_id, uint _block_chr_data )
		{
			uint var = _block_chr_data;
			
			var &= ~CONST_BLOCK_OBJ_ID_MASK;
#if DEF_PCE
			var |= ( uint )( ( _obj_id & CONST_OBJ_ID_MASK ) << 16 );
#else
			var |= ( uint )( ( _obj_id & CONST_OBJ_ID_MASK ) << 12 );
#endif		
			return var;
		}
		
		public static int get_block_CHR_id( uint _block_chr_data )
		{
#if DEF_NES
			return ( int )( _block_chr_data & CONST_NES_BLOCK_CHR_ID_MASK );
#elif DEF_SMS
			return ( int )( _block_chr_data & CONST_SMS_BLOCK_CHR_ID_MASK );
#elif DEF_PCE
			return ( int )( _block_chr_data & CONST_PCE_BLOCK_CHR_ID_MASK );
#endif
		}
		
		public static uint set_block_CHR_id( int _CHR_id, uint _block_chr_data )
		{
#if DEF_NES
			return ( ( _block_chr_data & ~CONST_NES_BLOCK_CHR_ID_MASK ) | ( uint )_CHR_id );
#elif DEF_SMS
			return ( ( _block_chr_data & ~CONST_SMS_BLOCK_CHR_ID_MASK ) | ( uint )_CHR_id );
#elif DEF_PCE
			return ( ( _block_chr_data & ~CONST_PCE_BLOCK_CHR_ID_MASK ) | ( uint )_CHR_id );
#endif
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
			bool contains = true;
			
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
		
		public void fill_CHR_bank_spr8x8_by_color_ind( int _chr_id, int _clr_ind )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				for( int j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
				{
					m_CHR_bank[ chr_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE + j ] = (byte)_clr_ind;
				}
			}
		}
		
		public int spr8x8_sum( int _chr_id )
		{
			int sum = 0;
			
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_PAGE_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				for( int j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
				{
					sum += m_CHR_bank[ chr_offset + i * utils.CONST_CHR_BANK_PAGE_SIDE + j ];
				}
			}
			
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
		
		public int get_tile_usage( byte _tile_ind, data_sets_manager.EScreenDataType _scr_type )
		{
			int res = 0;
			
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int scr_n;
				int tile_n;
				
				for( scr_n = 0; scr_n < scr_data.Count; scr_n++ )
				{
					for( tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
					{
						if( scr_data[ scr_n ][ tile_n ] == _tile_ind )
						{
							++res;
						}
					}
				}
			}
			
			return res;
		}

		public int get_block_usage( byte _block_ind, data_sets_manager.EScreenDataType _scr_type )
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
				for( scr_n = 0; scr_n < scr_data.Count; scr_n++ )
				{
					for( block_n = 0; block_n < utils.CONST_SCREEN_BLOCKS_CNT; block_n++ )
					{
						if( scr_data[ scr_n ][ block_n ] == _block_ind )
						{
							++res;
						}
					}
				}
			}
			
			return res;
		}
		
		private byte[] scr_alloc_data( data_sets_manager.EScreenDataType _type )
		{
			return new byte[ utils.get_screen_tiles_cnt_uni( _type ) ];
		}
		
		public void create_screen( data_sets_manager.EScreenDataType _type )
		{
			m_scr_data.Add( scr_alloc_data( _type ) );
		}
		
		public void copy_screen( int _id, data_sets_manager.EScreenDataType _type )
		{
			byte[] scr_copy = scr_alloc_data( _type );
			
			Array.Copy( m_scr_data[ _id ], scr_copy, scr_copy.Length );
			
			m_scr_data.Add( scr_copy );
		}
		
		public void delete_screen( int _ind )
		{
			m_scr_data.RemoveAt( _ind );
		}		
		
		public void inc_screen_tiles( byte _start_ind )
		{
			screen_tiles_proc( delegate( byte _tile_ind ) 
			{
				if( _tile_ind >= _start_ind )
				{
					++_tile_ind;
				}
				
				return _tile_ind;
			} );
		}
		
		public void dec_screen_tiles( byte _start_ind )
		{
			screen_tiles_proc( delegate( byte _tile_ind ) 
			{
				if( _tile_ind > _start_ind )
				{
					--_tile_ind;
				}
				
				return _tile_ind;
			} );
		}
		
		private void screen_tiles_proc( Func< byte, byte > _func )
		{
			byte[] screen_data = null; 
				
			for( int i = 0; i < scr_data.Count; i++ )
			{
				screen_data = scr_data[ i ];
				
				for( int j = 0; j < screen_data.Length; j++ )
				{
					screen_data[ j ] = _func( screen_data[ j ] );
				}
			}
		}

		public void inc_screen_blocks( byte _start_ind )
		{
			screen_tiles_proc( delegate( byte _block_ind ) 
			{
				if( _block_ind >= _start_ind )
				{
					++_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		public void dec_screen_blocks( byte _start_ind )
		{
			screen_tiles_proc( delegate( byte _block_ind ) 
			{
				if( _block_ind > _start_ind )
				{
					--_block_ind;
				}
				
				return _block_ind;
			} );
		}
		
		private void screen_blocks_proc( Func< byte, byte > _func )
		{
			byte[] screen_data = null; 
				
			for( int i = 0; i < scr_data.Count; i++ )
			{
				screen_data = scr_data[ i ];
				
				for( int j = 0; j < screen_data.Length; j++ )
				{
					screen_data[ j ] = _func( screen_data[ j ] );
				}
			}
		}

#if DEF_NES
		public long export_CHR( BinaryWriter _bw, bool _save_padding = false )
#elif DEF_SMS
		public long export_CHR( BinaryWriter _bw, int _bpp )
#elif DEF_PCE
		public long export_CHR( BinaryWriter _bw )
#endif			
		{
			int i;
			int j;
			int x;
			int y;
			int val;

#if DEF_NES || DEF_SMS			
			byte data;
#elif DEF_PCE
			ushort data;
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
							
							data |= ( ushort )( ( ( val & 0x01 ) << ( 8 + x_pos ) ) | ( ( ( val >> 1 ) & 0x01 ) << x_pos ) );
						}
						
						_bw.Write( data );
					}
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
			_bw.Write( scr_data.Count );
			
			for( i = 0; i < scr_data.Count; i++ )
			{
				_bw.Write( scr_data[ i ] );
			}
		}
		
		public void load( byte _ver, BinaryReader _br, string _file_ext, data_conversion_options_form.EScreensAlignMode _scr_align_mode, data_sets_manager.EScreenDataType _scr_type )
		{
			int i;
			uint val;

			int CHR_id;
			int prop_id;
			
			bool nes_file = _file_ext == utils.CONST_NES_FILE_EXT ? true:false;
			bool sms_file = _file_ext == utils.CONST_SMS_FILE_EXT ? true:false;
			bool pce_file = _file_ext == utils.CONST_PCE_FILE_EXT ? true:false;

#if DEF_SMS || DEF_PCE
			int palette_ind;

			Dictionary< int, int >	dict_CHR_palette_ind = nes_file ? new Dictionary<int, int>( utils.CONST_BLOCKS_UINT_SIZE ):null;
#endif

#if DEF_NES
			int num_pages;

			if( sms_file || pce_file )
			{
				for( i = 0; i < m_CHR_bank.Length; i++ )
				{
					m_CHR_bank[ i ] = ( byte )( _br.ReadByte() & 0x03 );
				}
				
				if( sms_file )
				{
					num_pages = utils.CONST_SMS_CHR_BANK_NUM_PAGES;
				}
				else
				{
					num_pages = utils.CONST_PCE_CHR_BANK_NUM_PAGES;
				}
				
				// skip the rest data
				_br.ReadBytes( ( num_pages * utils.CONST_CHR_BANK_PAGE_SIZE ) - m_CHR_bank.Length );
			}
#elif DEF_SMS			
			if( nes_file )
			{
				for( i = 0; i < ( utils.CONST_NES_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ); i++ )
				{
					m_CHR_bank[ i ] = _br.ReadByte();
				}
			}
			else
			if( pce_file )
			{
				for( i = 0; i < m_CHR_bank.Length; i++ )
				{
					m_CHR_bank[ i ] = _br.ReadByte();
				}
				
				// skip the rest data
				_br.ReadBytes( ( utils.CONST_PCE_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ) - m_CHR_bank.Length );
			}
#elif DEF_PCE
			if( sms_file )
			{
				for( i = 0; i < ( utils.CONST_SMS_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ); i++ )
				{
					m_CHR_bank[ i ] = _br.ReadByte();
				}
			}
			else
			if( nes_file )
			{
				for( i = 0; i < ( utils.CONST_NES_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ); i++ )
				{
					m_CHR_bank[ i ] = _br.ReadByte();
				}
			}
#endif			
			else
			{
				m_CHR_bank 	= _br.ReadBytes( m_CHR_bank.Length );
			}

			// load palette(s)			
			{
				int palettes_cnt = ( _ver == 1 ) ? 1: _br.ReadInt32();

				palettes_clear_arr();
				
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
#if DEF_NES
					if( !nes_file )
					{
						plt16.m_palette1[ 0 ] = plt16.m_palette2[ 0 ] = plt16.m_palette3[ 0 ] = plt16.m_palette0[ 0 ];
					}
#endif

#if DEF_SMS
					if( i >= utils.CONST_PALETTE16_ARR_LEN )
					{
						continue;
					}
#endif
					m_palettes.Add( plt16 );
				}
#if DEF_SMS
				// fill missing palette(s)
				for( i = m_palettes.Count; i < utils.CONST_PALETTE16_ARR_LEN; i++ )
				{
					m_palettes.Add( new palette16_data() );
				}
#endif
			}

			for( i = 0; i < utils.CONST_BLOCKS_UINT_SIZE; i++ )
			{
				if( _ver <= 2 )
				{
					val = _br.ReadUInt16();

					if( sms_file )
					{
						// OLD SMS: [ property_id ](4) [ CHR bank ](2) [ hv_flip ](2) [CHR ind](8)
						// NEW SMS: [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9)
						// 11-12 (CHR bank) <-> 9-10 (hv_flip)
						val = ( val & 0xfffff0ff ) | ( ( ( val & 0x00000300 ) << 2 ) | ( ( val & 0x00000c00 ) >> 2 ) );
					}
				}
				else
				{
					val = _br.ReadUInt32();
				}
#if DEF_NES
				if( sms_file )
				{
					// NES: [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)
					// SMS: [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9)

					// check CHR bank overflow
					if( ( val & 0x000000100 ) != 0 )
					{
						// clear overflowed data
						val = set_block_flags_palette( 0, val );
						val = set_block_CHR_id( 0, val );
					}
				}
				else
				if( pce_file )
				{
					// NES: [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)
					// PCE: [ property_id ](4) [ palette ind ](4) [CHR ind](12)
					CHR_id	= ( int )( val & CONST_PCE_BLOCK_CHR_ID_MASK );
					prop_id = ( int )( ( val & CONST_PCE_BLOCK_OBJ_ID_MASK ) >> 16 );
					
					val = 0;
					val = set_block_CHR_id( CHR_id, val );
					val = set_block_flags_obj_id( prop_id, val );
				}
#elif DEF_SMS
				if( nes_file )
				{
					// NES: [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)
					// SMS: [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9)
					
					// NES: palette -> SMS: flip flags
					palette_ind	= get_block_flags_flip( val );
					val 		= set_block_flags_flip( 0, val );
					
					CHR_id 		= get_block_CHR_id( val );
					
					if( !dict_CHR_palette_ind.ContainsKey( CHR_id ) )
					{
						// SMS: flip flags instead of palette on SMS
						dict_CHR_palette_ind.Add( CHR_id, palette_ind );
					}					
				}
				else
				if( pce_file )
				{
					// SMS: [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9)
					// PCE: [ property_id ](4) [ palette ind ](4) [CHR ind](12)
					CHR_id		= ( int )( val & CONST_PCE_BLOCK_CHR_ID_MASK );
					prop_id 	= ( int )( ( val & CONST_PCE_BLOCK_OBJ_ID_MASK ) >> 16 );
					palette_ind = ( int )( ( ( val & CONST_PCE_BLOCK_PALETTE_MASK ) >> 12 ) & CONST_SMS_PALETTE_MASK ); // clamp PCE palette by SMS palette mask
					
					val = 0;
					val = set_block_CHR_id( CHR_id, val );
					val = set_block_flags_palette( palette_ind, val );
					val = set_block_flags_obj_id( prop_id, val );
				}
#elif DEF_PCE
				if( sms_file )
				{
					// SMS: [ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [CHR ind](9)
					// PCE: [ property_id ](4) [ palette ind ](4) [CHR ind](12)
					CHR_id		= ( int )( val & CONST_SMS_BLOCK_CHR_ID_MASK );
					prop_id 	= ( int )( ( val & CONST_NES_SMS_BLOCK_OBJ_ID_MASK ) >> 12 );
					palette_ind = ( int )( ( val & CONST_SMS_BLOCK_PALETTE_MASK ) >> 9 );
					
					val = 0;
					val = set_block_CHR_id( CHR_id, val );
					val = set_block_flags_palette( palette_ind, val );
					val = set_block_flags_obj_id( prop_id, val );
				}
				else
				if( nes_file )
				{
					// NES: [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)
					// PCE: [ property_id ](4) [ palette ind ](4) [CHR ind](12)
					
					// NES: palette -> PCE: CHR id
					palette_ind	= ( int )( ( val & CONST_NES_BLOCK_PALETTE_MASK ) >> 10 );
					prop_id 	= ( int )( ( val & CONST_NES_SMS_BLOCK_OBJ_ID_MASK ) >> 12 );
					CHR_id 		= ( int )( val & CONST_NES_BLOCK_CHR_ID_MASK );
					
					val = 0;
					val = set_block_CHR_id( CHR_id, val );
					val = set_block_flags_obj_id( prop_id, val );
					
					if( !dict_CHR_palette_ind.ContainsKey( CHR_id ) )
					{
						// SMS: flip flags instead of palette on SMS
						dict_CHR_palette_ind.Add( CHR_id, palette_ind );
					}					
				}
#endif
				m_blocks[ i ] = val;
			}
#if DEF_SMS || DEF_PCE
			if( dict_CHR_palette_ind != null )
			{
				byte[] img_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
				
				foreach( var obj in dict_CHR_palette_ind ) 
				{
					from_CHR_bank_to_spr8x8( obj.Key, img_buff );
					{
						for( i = 0; i < img_buff.Length; i++ )
						{
							img_buff[ i ] += ( byte )( obj.Value * utils.CONST_PALETTE_SMALL_NUM_COLORS );
						}
					}
					from_spr8x8_to_CHR_bank( obj.Key, img_buff );
				}
				
				dict_CHR_palette_ind.Clear();
			}
#endif
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
				{
					m_tiles[ i ] = _br.ReadUInt32();
				}
			}
			
			// load screens data
			int scr_cnt = _br.ReadInt32();
			
			byte[] scr_data;
			
			int loaded_scr_data_len = utils.get_scr_tiles_cnt_by_file_ext( _file_ext ) * ( _scr_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 ? 4:1 );
			int scr_data_len 		= utils.get_screen_tiles_cnt_uni( _scr_type );
			
			byte[] loaded_scr = new byte[ loaded_scr_data_len ];
			
			int data_diff_half = 0;
			
			for( i = 0; i < scr_cnt; i++ )
			{
				scr_data = scr_alloc_data( _scr_type );
				
				if( loaded_scr_data_len != scr_data_len )
				{
					switch( _scr_align_mode )
					{
						case data_conversion_options_form.EScreensAlignMode.sam_Top:
							{
								if( scr_data_len > loaded_scr_data_len )
								{
									loaded_scr = _br.ReadBytes( loaded_scr_data_len );
									Array.Copy( loaded_scr, 0, scr_data, 0, loaded_scr.Length );								
								}
								else
								{
									scr_data = _br.ReadBytes( scr_data_len );
									
									// skip the rest
									_br.ReadBytes( loaded_scr_data_len - scr_data_len );
								}
							}
							break;
							
						case data_conversion_options_form.EScreensAlignMode.sam_Center:
							{
								if( scr_data_len > loaded_scr_data_len )
								{
									loaded_scr = _br.ReadBytes( loaded_scr_data_len );
									Array.Copy( loaded_scr, 0, scr_data, ( ( ( ( scr_data_len - loaded_scr_data_len ) / utils.CONST_SCREEN_NUM_WIDTH_TILES ) >> 1 ) * utils.CONST_SCREEN_NUM_WIDTH_TILES ), loaded_scr.Length );
								}
								else
								{
									data_diff_half = ( ( ( loaded_scr_data_len - scr_data_len ) / utils.CONST_SCREEN_NUM_WIDTH_TILES ) >> 1 ) * utils.CONST_SCREEN_NUM_WIDTH_TILES;
									
									// skip the first data
									_br.ReadBytes( data_diff_half );
	
									scr_data = _br.ReadBytes( scr_data_len );
									
									// skip the rest
									_br.ReadBytes( ( data_diff_half == 0 ) ? utils.CONST_SCREEN_NUM_WIDTH_TILES:data_diff_half );
								}
							}
							break;
							
						case data_conversion_options_form.EScreensAlignMode.sam_Bottom:
							{
								if( scr_data_len > loaded_scr_data_len )
								{
									loaded_scr = _br.ReadBytes( loaded_scr_data_len );
									Array.Copy( loaded_scr, 0, scr_data, ( scr_data_len - loaded_scr_data_len ), loaded_scr.Length );
								}
								else
								{
									// skip the first data
									_br.ReadBytes( loaded_scr_data_len - scr_data_len );
	
									scr_data = _br.ReadBytes( scr_data_len );
								}
							}
							break;
					}
				}
				else
				{
					scr_data = _br.ReadBytes( scr_data_len );
				}
				
				m_scr_data.Add( scr_data );
			}
		}
	}
}
