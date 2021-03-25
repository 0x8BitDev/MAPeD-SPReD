/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 25.03.2021
 * Time: 12:48
 */
using System;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of map_data.
	/// </summary>
	/// 
	
	public struct bit_field
	{
		private readonly int	m_bits_cnt;
		private readonly int	m_offset;
		
		public bit_field( int _bits_cnt, int _offset )
		{
			m_bits_cnt	= _bits_cnt;
			m_offset	= _offset;
		}
		
		public uint pack( int _val )
		{
			return ( uint )( ( mask( false ) & _val ) << m_offset );
		}
		
		public int unpack( uint _val )
		{
			return ( int )( ( _val >> m_offset ) & mask( false ) );
		}
		
		// _val - without offset
		public uint overwrite( int _val, uint _data )
		{
			uint mask_offs = mask( true );
			
			return ( uint )( ( ~mask_offs & _data ) | ( ( _val << m_offset ) & mask_offs ) );
		}
		
		public uint mask( bool _need_offset )
		{
			uint mask = ( uint )( ( 0x01 << m_bits_cnt ) - 1 );
			
			if( _need_offset )
			{
				return mask << m_offset;
			}
			
			return mask;
		}
		
		public bool valid()
		{
			return m_bits_cnt != 0;
		}
	}
	
	public class map_data_config_base
	{
		public enum EData
		{
			ed_CHR_id = 0,
			ed_Palette,
			ed_VH_Flip,
			ed_Obj_id,
			ed_MAX
		};
		
		private bit_field[] m_data = null;
		
		protected string m_platform = null;
		
		protected map_data_config_base()
		{
			//...
		}
		
		public string platform()
		{
			return m_platform;
		}
		
		protected void set_data( bit_field[] _data )
		{
			m_data = _data;
		}
		
		public bit_field get_bit_field( EData _type )
		{
			return m_data[ ( int )_type ];
		}
		
		public uint pack_data( EData _id, int _val, uint _map_data )
		{
			bit_field bf = m_data[ ( int )_id ];
			
			if( bf.valid() )
			{
				return ( ~bf.mask( true ) & _map_data ) | bf.pack( _val ); 
			}
			
			return 0xffffffff;
		}
		
		public int unpack_data( EData _id, uint _map_data )
		{
			bit_field bf = m_data[ ( int )_id ];
			
			if( bf.valid() )
			{
				return bf.unpack( _map_data );
			}
			
			return -1;
		}
		
		public uint repack( map_data_config_base _src, uint _map_data )
		{
			uint res = 0;
			
			bit_field bf_src;
			bit_field bf_dst;
			
			for( int i = 0; i < m_data.Length; i++ )
			{
				bf_src = _src.m_data[ i ];
				bf_dst = m_data[ i ];
				
				if( bf_src.valid() && bf_dst.valid() )
				{
					res |= bf_dst.pack( bf_src.unpack( _map_data ) );
				}
			}
			
			return res;
		}
	}
	
	// NES: [ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)
	public class map_data_app_NES : map_data_config_base
	{
		public map_data_app_NES()
		{
			m_platform = utils.CONST_PLATFORM_NES;
			
			set_data( new bit_field[]{ 
			         	new bit_field( 8, 0 ), 		// CHR id
			         	new bit_field( 2, 10 ), 	// palette index
			         	new bit_field( 0, 0 ), 		// H/V flipping
			         	new bit_field( 4, 12 ) } );	// property id
		}
	}

	// NES: [ CHR ind ](8)
	public class map_data_native_NES : map_data_config_base
	{
		public map_data_native_NES()
		{
			m_platform = utils.CONST_PLATFORM_NES;
			
			set_data( new bit_field[]{ 
			         	new bit_field( 8, 0 ), 		// CHR id
			         	new bit_field( 0, 0 ), 		// palette index
			         	new bit_field( 0, 0 ), 		// H/V flipping
			         	new bit_field( 0, 0 ) } );	// property id
		}
	}
	
	// SMS: [ property_id ](4) [ hv_flip ](2) [ palette_ind ](1) [CHR ind](9)
	public class map_data_app_SMS : map_data_config_base
	{
		public map_data_app_SMS()
		{
			m_platform = utils.CONST_PLATFORM_SMS;
			
			set_data( new bit_field[]{ 
			         	new bit_field( 9, 0 ), 		// CHR id
			         	new bit_field( 1, 9 ), 		// palette index
			         	new bit_field( 2, 10 ), 	// H/V flipping
			         	new bit_field( 4, 12 ) } );	// property id
		}
	}
	
	// OLD SMS: [ property_id ](4) [x](1) [ CHR bank ](1) [ hv_flip ](2) [CHR ind](8)
	//...

	// SMS: [ property_id ](4) [ hv_flip ](2) [ palette_ind ](1) [CHR ind](9)
	public class map_data_native_SMS : map_data_config_base
	{
		public map_data_native_SMS()
		{
			m_platform = utils.CONST_PLATFORM_SMS;
			
			set_data( new bit_field[]{ 
			         	new bit_field( 9, 0 ), 		// CHR id
			         	new bit_field( 1, 11 ), 	// palette index
			         	new bit_field( 2, 9 ), 		// H/V flipping
			         	new bit_field( 0, 0 ) } );	// property id
		}
	}
	
	// PCE: [ property_id ](4) [ palette ind ](4) [CHR ind](12)
	public class map_data_app_PCE : map_data_config_base
	{
		public map_data_app_PCE()
		{
			m_platform = utils.CONST_PLATFORM_PCE;
			
			set_data( new bit_field[]{ 
			         	new bit_field( 12, 0 ), 	// CHR id
			         	new bit_field( 4, 12 ), 	// palette index
			         	new bit_field( 0, 0 ), 		// H/V flipping
			         	new bit_field( 4, 16 ) } );	// property id
		}
	}

	// PCE: [ palette ind ](4) [CHR ind](12)
	public class map_data_native_PCE : map_data_config_base
	{
		public map_data_native_PCE()
		{
			m_platform = utils.CONST_PLATFORM_PCE;
			
			set_data( new bit_field[]{ 
			         	new bit_field( 12, 0 ), 	// CHR id
			         	new bit_field( 4, 12 ), 	// palette index
			         	new bit_field( 0, 0 ), 		// H/V flipping
			         	new bit_field( 0, 0 ) } );	// property id
		}
	}
	
	public static class map_data_config_provider
	{
		private static readonly map_data_app_NES m_map_data_app_NES = new map_data_app_NES();
		private static readonly map_data_app_SMS m_map_data_app_SMS = new map_data_app_SMS();
		private static readonly map_data_app_PCE m_map_data_app_PCE = new map_data_app_PCE();
		
		private static readonly Dictionary< string, map_data_config_base > m_map_data_app_dict = new Dictionary< string, map_data_config_base >();

		private static readonly map_data_native_NES m_map_data_native_NES = new map_data_native_NES();
		private static readonly map_data_native_SMS m_map_data_native_SMS = new map_data_native_SMS();
		private static readonly map_data_native_PCE m_map_data_native_PCE = new map_data_native_PCE();
		
		private static readonly Dictionary< string, map_data_config_base > m_map_data_native_dict = new Dictionary< string, map_data_config_base >();
		
		static map_data_config_provider()
		{
			m_map_data_app_dict.Add( m_map_data_app_NES.platform(), m_map_data_app_NES );
			m_map_data_app_dict.Add( m_map_data_app_SMS.platform(), m_map_data_app_SMS );
			m_map_data_app_dict.Add( m_map_data_app_PCE.platform(), m_map_data_app_PCE );
			
			m_map_data_native_dict.Add( m_map_data_native_NES.platform(), m_map_data_native_NES );
			m_map_data_native_dict.Add( m_map_data_native_SMS.platform(), m_map_data_native_SMS );
			m_map_data_native_dict.Add( m_map_data_native_PCE.platform(), m_map_data_native_PCE );
		}
		
		public static map_data_config_base config_app()
		{
			return m_map_data_app_dict[ utils.CONST_PLATFORM ];
		}
		
		public static map_data_config_base config_native()
		{
			return m_map_data_native_dict[ utils.CONST_PLATFORM ];
		}
	}	
}
