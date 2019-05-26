/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
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
		private static ushort CONST_BLOCK_MASK_FLIP		= (ushort)( 3 << 8 );
		private static ushort CONST_BLOCK_MASK_PALETTE	= (ushort)( 3 << 10 );
		private static ushort CONST_BLOCK_MASK_OBJ_ID	= (ushort)( 15 << 12 );
		
		private static int m_id	= -1;
		
		private string m_name	= null;
		
		private byte[] m_CHR_bank	= new byte[ utils.CONST_CHR_BANK_SIZE ];
		private byte[] m_palette0	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 1+16 ), (byte)( 1+32 ), (byte)( 1+48 ) };
		private byte[] m_palette1	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 4+16 ), (byte)( 4+32 ), (byte)( 4+48 ) };
		private byte[] m_palette2	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 7+16 ), (byte)( 7+32 ), (byte)( 7+48 ) };
		private byte[] m_palette3	= new byte[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ((byte)14), (byte)( 11+16 ), (byte)( 11+32 ), (byte)( 11+48 ) };
		
		private ushort[] m_blocks	= new ushort[ utils.CONST_BLOCKS_USHORT_SIZE ];	// [ 15-8 -> property_id(4) palette ind(2) hv_flip(2) | CHR ind(8) <-- 7-0 ]
		private uint[] m_tiles		= new uint[ utils.CONST_TILES_UINT_SIZE ];
		
		private List< byte[] >	m_scr_data	= null;

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
			
			Array.Copy( m_CHR_bank, 0, data.CHR_bank,	0, utils.CONST_CHR_BANK_SIZE );
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
			var |= ( _flip_flag << 8 );
			
			return (ushort)var;
		}
		
		public static int get_block_flags_palette( ushort _block_chr_data )
		{
			return ( ( _block_chr_data & CONST_BLOCK_MASK_PALETTE ) >> 10 );
		}
		
		public static ushort set_block_flags_palette( int _plt_ind, ushort _block_chr_data )
		{
			int var = _block_chr_data;
			
			var &= ~CONST_BLOCK_MASK_PALETTE;
			var |= ( _plt_ind << 10 );
			
			return (ushort)var;
		}

		public static int get_block_flags_obj_id( ushort _block_chr_data )
		{
			return ( _block_chr_data & CONST_BLOCK_MASK_OBJ_ID ) >> 12;
		}
		
		public static ushort set_block_flags_obj_id( int _obj_id, ushort _block_chr_data )
		{
			int var = _block_chr_data;
			
			var &= ~CONST_BLOCK_MASK_OBJ_ID;
			var |= ( _obj_id << 12 );
			
			return (ushort)var;
		}
		
		public static int get_block_CHR_id( ushort _block_chr_data )
		{
			return _block_chr_data & 0x00ff;
		}
		
		public static ushort set_block_CHR_id( int _CHR_id, ushort _block_chr_data )
		{
			return (ushort)( ( _block_chr_data & 0xff00 ) | _CHR_id );
		}
		
		public static void vflip( byte[] _CHR_data, int _spr_ind )
		{
			byte a;
			byte b;
			
			int offset_y1;
			int offset_y2;
			
			int chr_x = _spr_ind % 16;
			int chr_y = _spr_ind >> 4;
				
			int offset_bytes = ( chr_x << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) + ( ( chr_y * utils.CONST_CHR_BANK_SIDE ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
			
			for( int x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
			{
				for( int y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT>>1; y++ )
				{
					offset_y1 = offset_bytes + x + ( y * utils.CONST_CHR_BANK_SIDE );
					offset_y2 = offset_bytes + x + ( ( utils.CONST_SPR8x8_SIDE_PIXELS_CNT - 1 - y ) * utils.CONST_CHR_BANK_SIDE );
						
					a = _CHR_data[ offset_y1 ];
					b = _CHR_data[ offset_y2 ];
					
					_CHR_data[ offset_y1 ] = b;
					_CHR_data[ offset_y2 ] = a;
				}
			}
		}
		
		public static void hflip( byte[] _CHR_data, int _spr_ind )
		{
			byte a;
			byte b;
			
			int offset_x;
			int offset_y;

			int chr_x = _spr_ind % 16;
			int chr_y = _spr_ind >> 4;
				
			int offset_bytes = ( chr_x << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) + ( ( chr_y * utils.CONST_CHR_BANK_SIDE ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
			
			for( int y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
			{
				offset_y = y * utils.CONST_CHR_BANK_SIDE;
				
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
		
		public static void rotate_cw( byte[] _CHR_data, int _spr_ind )
		{
			int i;
			int j;
			
			int im8;
			int jm8;

			int chr_x = _spr_ind % 16;
			int chr_y = _spr_ind >> 4;
			
			int offset_bytes = ( chr_x << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) + ( ( chr_y * utils.CONST_CHR_BANK_SIDE ) << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS );
			
			for( i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ ) 
			{
				im8 = offset_bytes + ( i * utils.CONST_CHR_BANK_SIDE );
				
		        for( j = i; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ ) 
		        {
		            if( i != j ) 
		            {
		            	jm8 = offset_bytes + ( j * utils.CONST_CHR_BANK_SIDE );
		            	
		                _CHR_data[ i + jm8 ] ^= _CHR_data[ j + im8 ];
		                _CHR_data[ j + im8 ] ^= _CHR_data[ i + jm8 ];
		                _CHR_data[ i + jm8 ] ^= _CHR_data[ j + im8 ];
		            }
		        }
		    }
			
			hflip( _CHR_data, _spr_ind );
		}
		
		public void from_CHR_bank_to_spr8x8( int _chr_id, byte[] _img_buff, int _img_buff_offset = 0 )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				Array.Copy( m_CHR_bank, chr_offset + i * utils.CONST_CHR_BANK_SIDE, _img_buff, ( i << 3 ) + _img_buff_offset, 8 );
			}
		}
		
		public void from_spr8x8_to_CHR_bank( int _chr_id, byte[] _img_buff )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				Array.Copy( _img_buff, i << 3, m_CHR_bank, chr_offset + i * utils.CONST_CHR_BANK_SIDE, 8 );
			}
		}
		
		public void fill_CHR_bank_spr8x8_by_color_ind( int _chr_id, int _clr_ind )
		{
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				for( int j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
				{
					m_CHR_bank[ chr_offset + i * utils.CONST_CHR_BANK_SIDE + j ] = (byte)_clr_ind;
				}
			}
		}
		
		public int spr8x8_sum( int _chr_id )
		{
			int sum = 0;
			
			int chr_offset = ( ( _chr_id >> 4 ) * ( utils.CONST_CHR_BANK_SIDE << 3 ) ) + ( ( _chr_id % 16 ) << 3 );
			
			for( int i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
			{
				for( int j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
				{
					sum += m_CHR_bank[ chr_offset + i * utils.CONST_CHR_BANK_SIDE + j ];
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
					
					if( chr_id >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1 )
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
					
					if( block_id >= utils.CONST_MAX_BLOCKS_CNT - 1 )
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
					
					if( tile_id >= utils.CONST_MAX_TILES_CNT - 1 )
					{
						return -1;
					}
					
					break;
				}
			}
			
			return tile_id;
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

		public long export_CHR( BinaryWriter _bw, bool _need_padding = false )
		{
			int i;
			int x;
			int y;
			int val;
			
			long padding;
			long data_size;
			
			byte data;
			
			byte[] img_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
			
			int num_CHR_sprites = get_first_free_spr8x8_id();
			
			num_CHR_sprites = num_CHR_sprites < 0 ? utils.CONST_CHR_BANK_MAX_SPRITES_CNT:num_CHR_sprites;
			
			for( i = 0; i < num_CHR_sprites; i++ )
			{
				from_CHR_bank_to_spr8x8( i, img_buff, 0 );
				
				// the first 8 bytes out of 16 ones
				for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
				{
					data = 0;
					
					for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
					{
						val = img_buff[ ( y << 3 ) + ( 7 - x ) ];
						
						data |= ( byte )( ( val & 0x01 ) << x );
					}
					
					_bw.Write( data );
				}
				
				// the second 8 bytes of CHR
				for( y = 0; y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; y++ )
				{
					data = 0;
					
					for( x = 0; x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; x++ )
					{
						val = img_buff[ ( y << 3 ) + ( 7 - x ) ];
						
						data |= ( byte )( ( val >> 1 ) << x );
					}
					
					_bw.Write( data );
				}
			}
			
			// save padding data to 1/2/4 KB
			if( _need_padding )
			{
				padding 	= 0;
				data_size 	= _bw.BaseStream.Length;
				
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
			
			return _bw.BaseStream.Length;
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
		
		public void load( BinaryReader _br )
		{
			int i;
			
			m_CHR_bank 	= _br.ReadBytes( utils.CONST_CHR_BANK_SIZE );
			m_palette0	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );
			m_palette1	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );
			m_palette2	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );
			m_palette3	= _br.ReadBytes( utils.CONST_PALETTE_SMALL_NUM_COLORS );

			for( i = 0; i < utils.CONST_BLOCKS_USHORT_SIZE; i++ )
			{
				m_blocks[ i ] = _br.ReadUInt16();
			}
			
			for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
			{
				m_tiles[ i ] = _br.ReadUInt32();
			}
			
			// load screens data
			int scr_cnt = _br.ReadInt32();
			
			byte[] scr_data;
			
			for( i = 0; i < scr_cnt; i++ )
			{
				scr_data = scr_alloc_data();
				scr_data = _br.ReadBytes( utils.CONST_SCREEN_TILES_CNT );
				
				m_scr_data.Add( scr_data );
			}
		}
	}
}
