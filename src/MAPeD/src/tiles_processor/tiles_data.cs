/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
	
	[DataContract]
	public class tiles_data
	{
		private static ushort CONST_MASK_FLIP			= 0x03;
		private static ushort CONST_BLOCK_MASK_FLIP		= (ushort)( CONST_MASK_FLIP << 8 );
#if DEF_NES
		private static ushort CONST_MASK_PALETTE		= 0x03;
		private static ushort CONST_BLOCK_MASK_PALETTE	= (ushort)( CONST_MASK_PALETTE << 10 );
#elif DEF_SMS
		private static ushort CONST_MASK_CHR_BANK_PAGE			= 0x03; 
		private static ushort CONST_BLOCK_MASK_CHR_BANK_PAGE 	= (ushort)( CONST_MASK_CHR_BANK_PAGE << 10 );
#endif
		private static ushort CONST_MASK_OBJ_ID			= 0x0f; 
		private static ushort CONST_BLOCK_MASK_OBJ_ID	= (ushort)( CONST_MASK_OBJ_ID << 12 );
		
		private static int m_id	= -1;
		
		private string m_name	= null;
		
		private byte[] m_CHR_bank	= new byte[ utils.CONST_CHR_BANK_PAGE_SIZE * utils.CONST_CHR_BANK_PAGES_CNT ];
#if DEF_NES		
		private byte[] m_palette0	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 1+16 ), (byte)( 1+32 ), (byte)( 1+48 ) };
		private byte[] m_palette1	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 4+16 ), (byte)( 4+32 ), (byte)( 4+48 ) };
		private byte[] m_palette2	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 7+16 ), (byte)( 7+32 ), (byte)( 7+48 ) };
		private byte[] m_palette3	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 11+16 ), (byte)( 11+32 ), (byte)( 11+48 ) };
#elif DEF_SMS
		private byte[] m_palette0	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)0), (byte)( 3 ),  (byte)( 7 ),  (byte)( 11 ) };
		private byte[] m_palette1	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)17), (byte)( 20 ), (byte)( 24 ), (byte)( 28 ) };
		private byte[] m_palette2	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)34), (byte)( 37 ), (byte)( 41 ), (byte)( 45 ) };
		private byte[] m_palette3	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)51), (byte)( 54 ), (byte)( 58 ), (byte)( 62 ) };
#endif
		private ushort[] m_blocks	= new ushort[ utils.CONST_BLOCKS_USHORT_SIZE ];	// [ 15-8 -> property_id(4) [ NES:palette ind SMS:CHR bank page ](2) [ NES: not used SMS: hv_flip ](2) | CHR ind(8) <-- 7-0 ]
		private uint[] m_tiles		= new uint[ utils.CONST_TILES_UINT_SIZE ];
		
		private List< byte[] >	m_scr_data	= null;
		
		private Dictionary< string, List< pattern_data > >	m_patterns_data	= null;	// key = group name / value = List< pattern_data >

		[DataMember]
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
		
		[DataMember]
		public byte[] palette0
		{
			get { return m_palette0; }
			set {}
		}

		[DataMember]
		public byte[] palette1
		{
			get { return m_palette1; }
			set {}
		}
		
		[DataMember]
		public byte[] palette2
		{
			get { return m_palette2; }
			set {}
		}
		
		[DataMember]
		public byte[] palette3
		{
			get { return m_palette3; }
			set {}
		}
		
		public List< byte[] > palettes
		{
			get 
			{
				List< byte[] > palettes	= new List< byte[] >( 4 );
				
				palettes.Add( m_palette0 );
				palettes.Add( m_palette1 );
				palettes.Add( m_palette2 );
				palettes.Add( m_palette3 );
				
				return palettes;  
			}
			set {}
		}
		
		[DataMember]
		public ushort[] blocks
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
			
			m_scr_data = new List< byte[] >( 100 );
			
			m_patterns_data	= new Dictionary< string, List< pattern_data > >( 10 );
			m_patterns_data.Add( "MAIN", new List< pattern_data >() );
		}
		
		public void destroy()
		{
			--m_id;
			
			m_CHR_bank 	= null;
			m_palette0 	= null;
			m_palette1 	= null;
			m_palette2 	= null;
			m_palette3 	= null;
			m_blocks 	= null;
			m_tiles 	= null;
			
			m_scr_data.Clear();
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
		
		public void dec_patterns_tiles( byte _tile_id )
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
							
							if( tile_id >= _tile_id && tile_id > 0 )
							{
								_pattern.data[ tile_ind ] = ( byte )( tile_id - 1 );
							}
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
			
			Array.Copy( m_CHR_bank, 0, data.CHR_bank,	0, m_CHR_bank.Length );
			Array.Copy( m_palette0, 0, data.m_palette0, 0, utils.CONST_PALETTE_SMALL_NUM_COLORS );  
			Array.Copy( m_palette1, 0, data.m_palette1, 0, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( m_palette2, 0, data.m_palette2, 0, utils.CONST_PALETTE_SMALL_NUM_COLORS );
			Array.Copy( m_palette3, 0, data.m_palette3, 0, utils.CONST_PALETTE_SMALL_NUM_COLORS );
 
			Array.Copy( m_blocks, 	0, data.m_blocks, 	0, utils.CONST_BLOCKS_USHORT_SIZE );
			Array.Copy( m_tiles, 	0, data.m_tiles, 	0, utils.CONST_TILES_UINT_SIZE );

			// COPY SCREENS
			{
				byte[] screen_data = null; 
					
				for( int i = 0; i < scr_data.Count; i++ )
				{
					screen_data = scr_alloc_data();
					
					Array.Copy( m_scr_data[ i ], 0, screen_data, 0, utils.CONST_SCREEN_TILES_CNT );
					
					data.m_scr_data.Add( screen_data );
				}
			}
			
			// copy patters
			if( patterns_data != null )
			{
				data.patterns_data = new Dictionary< string, List< pattern_data > >( patterns_data.Count, patterns_data.Comparer );
				
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
			ushort	CHR_data;
			
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
		
		public static byte get_block_flags_flip( ushort _block_chr_data )
		{
			return (byte)( ( _block_chr_data & CONST_BLOCK_MASK_FLIP ) >> 8 );
		}
		
		public static ushort set_block_flags_flip( byte _flip_flag, ushort _block_chr_data )
		{
			int var = _block_chr_data;
			
			var &= ~CONST_BLOCK_MASK_FLIP;
			var |= ( ( _flip_flag & CONST_MASK_FLIP ) << 8 );
			
			return (ushort)var;
		}
		
#if DEF_NES		
		public static int get_block_flags_palette( ushort _block_chr_data )
		{
			return ( ( _block_chr_data & CONST_BLOCK_MASK_PALETTE ) >> 10 );
		}
		
		public static ushort set_block_flags_palette( int _plt_ind, ushort _block_chr_data )
		{
			int var = _block_chr_data;
			
			var &= ~CONST_BLOCK_MASK_PALETTE;
			var |= ( ( _plt_ind & CONST_MASK_PALETTE ) << 10 );
			
			return (ushort)var;
		}
#elif DEF_SMS
		private static int get_block_flags_CHR_bank_page( ushort _block_chr_data )
		{
			return ( ( _block_chr_data & CONST_BLOCK_MASK_CHR_BANK_PAGE ) >> 10 );
		}

		private static ushort set_block_flags_CHR_bank_page( int _CHR_bank_page, ushort _block_chr_data )
		{
			int var = _block_chr_data;
			
			var &= ~CONST_BLOCK_MASK_CHR_BANK_PAGE;
			var |= ( ( _CHR_bank_page & CONST_MASK_CHR_BANK_PAGE ) << 10 );
			
			return (ushort)var;
		}
		
#endif
		public static int get_block_flags_obj_id( ushort _block_chr_data )
		{
			return ( _block_chr_data & CONST_BLOCK_MASK_OBJ_ID ) >> 12;
		}
		
		public static ushort set_block_flags_obj_id( int _obj_id, ushort _block_chr_data )
		{
			int var = _block_chr_data;
			
			var &= ~CONST_BLOCK_MASK_OBJ_ID;
			var |= ( ( _obj_id & CONST_MASK_OBJ_ID ) << 12 );
			
			return (ushort)var;
		}
		
		public static int get_block_CHR_id( ushort _block_chr_data )
		{
#if DEF_NES
			return _block_chr_data & 0x00ff;
#elif DEF_SMS
			return ( _block_chr_data & 0x00ff ) | ( get_block_flags_CHR_bank_page( _block_chr_data ) << 8 );
#endif
		}
		
		public static ushort set_block_CHR_id( int _CHR_id, ushort _block_chr_data )
		{
#if DEF_NES
			return (ushort)( ( _block_chr_data & 0xff00 ) | _CHR_id );
#elif DEF_SMS
			return (ushort)( ( set_block_flags_CHR_bank_page( ( _CHR_id >> 8 ), _block_chr_data ) & 0xff00 ) | ( _block_chr_data & ( CONST_BLOCK_MASK_FLIP|CONST_BLOCK_MASK_OBJ_ID ) ) | ( _CHR_id & 0xff ) );
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
		
		public int contains_block( int _block_ind )
		{
			int block_n;
			
			int size = _block_ind < 0 ? blocks.Length:_block_ind;
			
			for( block_n = 0; block_n < size; block_n += utils.CONST_BLOCK_SIZE )
			{
				if( blocks[ block_n ] == blocks[ _block_ind ] && 
 				    blocks[ block_n + 1 ] == blocks[ _block_ind + 1 ] && 
 				    blocks[ block_n + 2 ] == blocks[ _block_ind + 2 ] && 
 				    blocks[ block_n + 3 ] == blocks[ _block_ind + 3 ] )
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
			int sum = 0;
			
			int block_offset = _block_id << 2;
			
			for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
			{
				sum += m_blocks[ block_offset + i ];
			}
			
			return sum;
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
		
		public int get_tile_usage( byte _tile_ind )
		{
			int res = 0;
			
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
			
			return res;
		}

		public int get_block_usage( byte _block_ind )
		{
			int res = 0;
			
			int tile_n;
			int block_n;
			
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
			
			return res;
		}
		
		private byte[] scr_alloc_data()
		{
			return new byte[ utils.CONST_SCREEN_TILES_CNT ];
		}
		
		public void create_screen()
		{
			m_scr_data.Add( scr_alloc_data() );
		}
		
		public void copy_screen( int _id )
		{
			byte[] scr_copy = scr_alloc_data();
			
			Array.Copy( m_scr_data[ _id ], scr_copy, utils.CONST_SCREEN_TILES_CNT );
			
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

#if DEF_NES		
		public long export_CHR( BinaryWriter _bw, bool _save_padding = false )
#elif DEF_SMS
		public long export_CHR( BinaryWriter _bw, int _bpp )
#endif			
		{
			int i;
			int j;
			int x;
			int y;
			int val;
			
			byte data;
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

		public void load_tiles_patterns( BinaryReader _br )
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
					pattrn_list.Add( pattern_data.load( _br ) );
				}
			}
		}
		
		public void save( BinaryWriter _bw )
		{
			int i;
			
			_bw.Write( m_CHR_bank );
			_bw.Write( m_palette0 );
			_bw.Write( m_palette1 );
			_bw.Write( m_palette2 );
			_bw.Write( m_palette3 );
			
			for( i = 0; i < utils.CONST_BLOCKS_USHORT_SIZE; i++ )
			{
				_bw.Write( m_blocks[ i ] );
			}

			for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
			{
				_bw.Write( m_tiles[ i ] );
			}
			
			// save screens data
			_bw.Write( scr_data.Count );
			
			for( i = 0; i < scr_data.Count; i++ )
			{
				_bw.Write( scr_data[ i ] );
			}
		}
		
		public void load( BinaryReader _br, string _file_ext, data_conversion_options_form.EScreensAlignMode _scr_align_mode )
		{
			int i;
			UInt16 val;
			
			bool nes_file = _file_ext == utils.CONST_NES_FILE_EXT ? true:false;
			bool sms_file = _file_ext == utils.CONST_SMS_FILE_EXT ? true:false;

#if DEF_SMS
			int CHR_id;
			int palette_ind;
			
			Dictionary< int, int >	dict_CHR_palette_ind = nes_file ? new Dictionary<int, int>( utils.CONST_BLOCKS_USHORT_SIZE ):null;
#endif

#if DEF_NES
			if( sms_file )
			{
				for( i = 0; i < m_CHR_bank.Length; i++ )
				{
					m_CHR_bank[ i ] = ( byte )( _br.ReadByte() & 0x03 );
				}
				
				// skip the rest data
				_br.ReadBytes( ( utils.CONST_SMS_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ) - m_CHR_bank.Length );
			}
#elif DEF_SMS			
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
		
			m_palette0	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );
			m_palette1	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );
			m_palette2	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );
			m_palette3	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );

#if DEF_NES
			if( sms_file )
			{
				m_palette1[ 0 ] = m_palette2[ 0 ] = m_palette3[ 0 ] = m_palette0[ 0 ];
			}
#endif
			for( i = 0; i < utils.CONST_BLOCKS_USHORT_SIZE; i++ )
			{
				val = _br.ReadUInt16();
#if DEF_NES
				if( sms_file )
				{
					// NES: palette instead of CHR bank page on SMS
					if( get_block_flags_palette( val ) > 0 )
					{
						val = set_block_flags_palette( 0, val );	// TODO: test it with a CHR_id > 255 !!!!!!!!!!!!!
						val = set_block_CHR_id( 0, val );
					}
				}
#elif DEF_SMS
				if( nes_file )
				{
					// NES: palette
					palette_ind = get_block_flags_CHR_bank_page( val );
					val = set_block_flags_CHR_bank_page( 0, val );
					
					CHR_id = get_block_CHR_id( val );
					
					if( !dict_CHR_palette_ind.ContainsKey( CHR_id ) )
					{
						// SMS: CHR bank page instead of palette on SMS
						dict_CHR_palette_ind.Add( CHR_id, palette_ind );
					}					
					
					val = set_block_flags_CHR_bank_page( 0, val );
				}
#endif
				m_blocks[ i ] = val;
			}
#if DEF_SMS
			if( nes_file && dict_CHR_palette_ind != null )
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
			for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
			{
				m_tiles[ i ] = _br.ReadUInt32();
			}
			
			// load screens data
			int scr_cnt = _br.ReadInt32();
			
			byte[] scr_data;
			
			byte[] sms_scr = new byte[ utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES ];
			
			int nes_sms_scr_diff = ( utils.CONST_NES_SCREEN_NUM_WIDTH_TILES * utils.CONST_NES_SCREEN_NUM_HEIGHT_TILES ) - ( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
			int nes_sms_scr_half_diff = nes_sms_scr_diff >> 1;
			
			for( i = 0; i < scr_cnt; i++ )
			{
				scr_data = scr_alloc_data();
#if DEF_NES
				if( sms_file )
#elif DEF_SMS			
				if( nes_file )
#endif			
				{
					switch( _scr_align_mode )
					{
						case data_conversion_options_form.EScreensAlignMode.sam_Top:
							{
#if DEF_NES
								sms_scr = _br.ReadBytes( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
								Array.Copy( sms_scr, 0, scr_data, 0, sms_scr.Length );								
#elif DEF_SMS
								scr_data = _br.ReadBytes( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
								
								// skip the rest
								_br.ReadBytes( nes_sms_scr_diff );
#endif
							}
							break;
							
						case data_conversion_options_form.EScreensAlignMode.sam_Center:
							{
#if DEF_NES
								sms_scr = _br.ReadBytes( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
								Array.Copy( sms_scr, 0, scr_data, ( nes_sms_scr_diff >> 1 ), sms_scr.Length );
#elif DEF_SMS
								// skip the first data
								_br.ReadBytes( nes_sms_scr_half_diff );

								scr_data = _br.ReadBytes( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
								
								// skip the rest
								_br.ReadBytes( nes_sms_scr_half_diff );
#endif							
							}
							break;
							
						case data_conversion_options_form.EScreensAlignMode.sam_Bottom:
							{
#if DEF_NES
								sms_scr = _br.ReadBytes( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
								Array.Copy( sms_scr, 0, scr_data, nes_sms_scr_diff, sms_scr.Length );
#elif DEF_SMS
								// skip the first data
								_br.ReadBytes( nes_sms_scr_diff );

								scr_data = _br.ReadBytes( utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES );
#endif							
							}
							break;
					}
				}
				else
				{
					scr_data = _br.ReadBytes( utils.CONST_SCREEN_TILES_CNT );
				}
				
				m_scr_data.Add( scr_data );
			}
		}
	}
}
