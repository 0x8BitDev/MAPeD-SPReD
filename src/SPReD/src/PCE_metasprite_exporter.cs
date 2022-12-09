/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 05.07.2022
 * Time: 13:03
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace SPReD
{
	/// <summary>
	/// Description of PCE_metasprite_exporter.
	/// </summary>
	public static class PCE_metasprite_exporter
	{
		private readonly static int[] pattern_arr = 
		{	
			0, 0,  16, 0,
			0, 16, 16, 16,
			0, 32, 16, 32,
			0, 48, 16, 48
		};

		public static void export_CHR_data( sprite_data _spr, string _filename )
		{
			int	i;
			
			List< CHR_data_attr > attrs	= _spr.get_CHR_attr();
			int[]	exp_attrs			= new int[ attrs.Count ];
			int		exp_attrs_pos		= 0;
			bool[]	attr_usage			= new bool[ attrs.Count ];	// used attributes
			
			Array.Clear( attr_usage, 0, attr_usage.Length );
			
			// collect sprite patterns
			{
				// 32x64
				for( i = 0; i < attrs.Count; i++ )
				{
					search_pattern( _spr, attrs[ i ], exp_attrs, ref exp_attrs_pos, attr_usage, 8 );
				}
				
				// 32x32
				for( i = 0; i < attrs.Count; i++ )
				{
					search_pattern( _spr, attrs[ i ], exp_attrs, ref exp_attrs_pos, attr_usage, 4 );
				}

				// 32x16
				for( i = 0; i < attrs.Count; i++ )
				{
					search_pattern( _spr, attrs[ i ], exp_attrs, ref exp_attrs_pos, attr_usage, 2 );
				}

				// 16x16
				for( i = 0; i < attrs.Count; i++ )
				{
					search_pattern( _spr, attrs[ i ], exp_attrs, ref exp_attrs_pos, attr_usage, 1 );
				}
			}
			
			// save collected patterns
			{
				BinaryWriter bw = new BinaryWriter( File.Open( _filename, FileMode.Create ) );
	
				for( i = 0; i < exp_attrs.Length; i++ )
				{
					save_CHR( bw, _spr.get_CHR_data().get_data()[ attrs[ exp_attrs[ i ] ].CHR_ind ] );
				}
				
				bw.Dispose();
			}
		}

		public static void save_CHR( BinaryWriter _bw, CHR_data _chr_data )
		{
			int		j;
			int		x;
			int		y;
			int		val;
			
			ushort	data;
			
			for( j = 0; j < 4; j++ )
			{
				for( y = 0; y < utils.CONST_CHR_SIDE_PIXELS_CNT; y++ )
				{
					data = 0;
					
					for( x = 0; x < utils.CONST_CHR_SIDE_PIXELS_CNT; x++ )
					{
						val = _chr_data.get_data()[ ( y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS ) + ( ( utils.CONST_CHR_SIDE_PIXELS_CNT - 1 ) - x ) ];
						
						data |= ( ushort )( ( ( val >> j ) & 0x01 ) << x );
					}
					
					_bw.Write( data );
				}
			}
		}
		
		private static bool search_pattern( sprite_data _spr, CHR_data_attr _attr, int[] _exp_attrs, ref int _exp_attrs_pos, bool[] _attr_usage, int _max_pttrn_sprts )
		{
			CHR_data_attr	attr;
			
			int		i;
			int[]	pttrn_inds	= new int[ 8 ];	// max 32x64
			int		pttrn_pos	= 0;
			
			int		offset_x;
			int		offset_y;
			int		ptrn_pos_x;
			int		ptrn_pos_y;
			
			List< CHR_data_attr > attrs	= _spr.get_CHR_attr();
			
			for( i = 0; i < attrs.Count; i++ )
			{
				attr = attrs[ i ];
				
				if( ( attr.palette_ind == _attr.palette_ind ) && ( _attr_usage[ i ] == false ) )
				{
					offset_x = attr.x - _attr.x;
					offset_y = attr.y - _attr.y;
					
					ptrn_pos_x	= pattern_arr[ pttrn_pos << 1 ];
					ptrn_pos_y	= pattern_arr[ ( pttrn_pos << 1 ) + 1 ];
					
					if( ( attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 )
					{
						if( ( pttrn_pos & 0x01 ) != 0 )
						{
							offset_x = -offset_x;
						}
					}
					
					if( ( attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 )
					{
						offset_y = -offset_y;
					}
					
					if( ptrn_pos_x == offset_x && ptrn_pos_y == offset_y )
					{
						pttrn_inds[ pttrn_pos ] = i;
						++pttrn_pos;
						
						if( pttrn_pos == _max_pttrn_sprts )
						{
							break;
						}
					}
				}
			}
			
			if( pttrn_pos == _max_pttrn_sprts )
			{
				for( i = 0; i < _max_pttrn_sprts; i++ )
				{
					_attr_usage[ pttrn_inds[ i ] ]	= true;
					
					_exp_attrs[ _exp_attrs_pos ]	= pttrn_inds[ i ];

					++_exp_attrs_pos;					
				}
				
				return true;
			}
			
			return false;
		}
		
		public static void export_sprite( StreamWriter _sw, sprite_data _spr, int _CHRs_offset, int _palette_slot, string _data_prefix )
		{
			int	i;
			int num_sprites = 0;
			
			List< CHR_data_attr > attrs	= _spr.get_CHR_attr();
			int[]	exp_attrs			= new int[ attrs.Count ];
			int		exp_attrs_pos		= 0;
			bool[]	attr_usage			= new bool[ attrs.Count ];	// used attributes
			
			CHR_data_attr	chr_attr;
			
			Array.Clear( attr_usage, 0, attr_usage.Length );
			
			_sw.WriteLine( _data_prefix + _spr.name + ":" );
			
			_sw.WriteLine( "\t.word " + _data_prefix + _spr.name + "_end - " + _data_prefix + _spr.name + " - 3\t; data size" );
			_sw.WriteLine( "\t.byte " + _spr.get_CHR_data().id + "\t\t; GFX bank index (" + _spr.get_CHR_data().name + ")\n" );
			
			// collect sprite patterns
			{
				// 32x64
				for( i = 0; i < attrs.Count; i++ )
				{
					chr_attr = attrs[ i ];
					
					if( search_pattern( _spr, chr_attr, exp_attrs, ref exp_attrs_pos, attr_usage, 8 ) )
					{
						chr_attr			= chr_attr.copy();
						chr_attr.CHR_ind	= exp_attrs_pos - 8;
						
						if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 )
						{
							chr_attr.x -= utils.CONST_CHR_SIDE_PIXELS_CNT;
						}

						if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 )
						{
							chr_attr.y -= utils.CONST_CHR_SIDE_PIXELS_CNT * 3;
						}
						
						save_attribute( _sw, _spr, chr_attr, 0x31 << 8, _CHRs_offset, 0, _palette_slot );
						
						++num_sprites;
					}
				}
				
				// 32x32
				for( i = 0; i < attrs.Count; i++ )
				{
					chr_attr = attrs[ i ];
					
					if( search_pattern( _spr, chr_attr, exp_attrs, ref exp_attrs_pos, attr_usage, 4 ) )
					{
						chr_attr			= chr_attr.copy();
						chr_attr.CHR_ind	= exp_attrs_pos - 4;

						if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 )
						{
							chr_attr.x -= utils.CONST_CHR_SIDE_PIXELS_CNT;
						}

						if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 )
						{
							chr_attr.y -= utils.CONST_CHR_SIDE_PIXELS_CNT;
						}
						
						save_attribute( _sw, _spr, chr_attr, 0x11 << 8, _CHRs_offset, 0, _palette_slot );
						
						++num_sprites;
					}
				}

				// 32x16
				for( i = 0; i < attrs.Count; i++ )
				{
					chr_attr = attrs[ i ];
					
					if( search_pattern( _spr, chr_attr, exp_attrs, ref exp_attrs_pos, attr_usage, 2 ) )
					{
						chr_attr			= chr_attr.copy();
						chr_attr.CHR_ind	= exp_attrs_pos - 2;

						if( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 )
						{
							chr_attr.x -= utils.CONST_CHR_SIDE_PIXELS_CNT;
						}
						
						save_attribute( _sw, _spr, chr_attr, 0x01 << 8, _CHRs_offset,  0, _palette_slot );
						
						++num_sprites;
					}
				}

				// 16x16
				for( i = 0; i < attrs.Count; i++ )
				{
					chr_attr = attrs[ i ];
					
					if( search_pattern( _spr, chr_attr, exp_attrs, ref exp_attrs_pos, attr_usage, 1 ) )
					{
						chr_attr			= chr_attr.copy();
						chr_attr.CHR_ind	= exp_attrs_pos - 1;
						
						save_attribute( _sw, _spr, chr_attr, 0, _CHRs_offset, 0, _palette_slot );
						
						++num_sprites;
					}
				}
			}
			
			_sw.WriteLine( _data_prefix + _spr.name + "_end:\n" );
			
			if( num_sprites > utils.CONST_SPRITE_MAX_NUM_ATTRS )
			{
				throw new Exception( "The sprite - " + _spr.name + " - has more than " + utils.CONST_SPRITE_MAX_NUM_ATTRS.ToString() + " tiles that exceed the hardware limit!\n Please, fix it to avoid the sprite drawing error in your project!" );
			}
		}
		
		public static void save_attribute( StreamWriter _sw, sprite_data _spr, CHR_data_attr _chr_attr, int _cgx_cgy, int _CHRs_offset, int _CHR_ind_offset, int _palette_slot )
		{
			if( _chr_attr.CHR_ind + _CHRs_offset >= utils.CONST_PCE_MAX_SPRITES_CNT )
			{
				throw new Exception( "CHRs indices overflow! Invalid CHRs offset value!" );
			}
				
			int attr	= ( _chr_attr.palette_ind + _palette_slot ) & 0x0f;
			
			attr |= _cgx_cgy;
			attr |= ( ( _chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 ) ? ( 1 << 11 ):0;
			attr |= ( ( _chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 ) ? ( 1 << 15 ):0;
			attr |= 1 << 7; // sprite priority

			_sw.WriteLine( "\t.word " + String.Format( "${0:X2}, ${1:X2}, ${2:X2}, ${3:X2}", unchecked( ( ushort )( _spr.offset_y + _chr_attr.y ) ), unchecked( ( ushort )( _spr.offset_x + _chr_attr.x - ( _CHR_ind_offset << 4 ) ) ), unchecked( ( ushort )( ( _chr_attr.CHR_ind + _CHRs_offset ) << 1 ) ), ( ushort )attr ) );
		}
		
		public static int get_CGX_CGY_flags( sprite_data _spr, ref int _CHR_ind_offset )
		{
			int res = 0;			
			
			int size_x	= _spr.size_x;
			int size_y	= _spr.size_y;
			
			List< CHR_data_attr > attrs	= _spr.get_CHR_attr();
						
			if( size_x == 32 )
			{
				string lower_name = _spr.name.ToLower();
				
				bool s16x_0 = lower_name.Contains( "16x32_0" ) || lower_name.Contains( "16x64_0" );
				bool s16x_1 = lower_name.Contains( "16x32_1" ) || lower_name.Contains( "16x64_1" );
				
				if( s16x_0 || s16x_1 )
				{
					if( size_y == 32 && attrs.Count == 4 )
					{
						res = 0x10;
					}
					else
					if( size_y == 64 && attrs.Count == 8 )
					{
						res = 0x30;
					}
					
					if( res != 0 )
					{
						if( s16x_1 )
						{
							_CHR_ind_offset = 1;
						}
						else
						{
							_CHR_ind_offset = 0;
						}
					}
				}
				else
				{
					if( size_y == 16 && attrs.Count == 2 )
					{
						res = 0x01; 
					}
					else
					if( size_y == 32 && attrs.Count == 4 )
					{
						res = 0x11;
					}
					else
					if( size_y == 64 && attrs.Count == 8 )
					{
						res = 0x31;
					}
				}
			}
			
			if( res != 0 )
			{
				// check coordinates multiples of 16
				CHR_data_attr attr;
				
				int x;
				int y;
				
				int offs_x = attrs[ 0 ].x;
				int offs_y = attrs[ 0 ].y;
					
				for( int i = 1; i < attrs.Count; i++ )
				{
					attr = attrs[ i ];
					
					x = attr.x - offs_x;
					y = attr.y - offs_y;
					
					if( ( x != 0 && ( ( x - 1 ) & 0x0f ) != 0x0f ) || ( y != 0 && ( ( y - 1 ) & 0x0f ) != 0x0f ) )
					{
						res = 0;
						_CHR_ind_offset = 0;
						
						break;
					}
				}
			}
				
			return res << 8;
		}
	}
}
