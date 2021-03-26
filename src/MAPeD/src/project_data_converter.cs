/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 26.03.2021
 * Time: 13:05
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of project_data_converter.
	/// </summary>
	/// 
	
	public interface i_project_data_converter
	{
		void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank );
		
		void pre_load_block_data( utils.EPlatformType _prj_platform );
		void post_load_block_data( tiles_data _data );
		uint convert_block_data( byte _ver, utils.EPlatformType _prj_platform, uint _data );
	}

	// Base functionality
	public class project_data_converter_base : i_project_data_converter
	{
		private Dictionary< int, int >	m_dict_CHR_palette_ind = null;

		protected map_data_config_base m_map_data_app_src = null;
		protected map_data_config_base m_map_data_app_dst = null;

		public void pre_load_block_data( utils.EPlatformType _prj_platform )
		{
			if( _prj_platform == utils.EPlatformType.pt_NES && platform_data_provider.get_platform_type() != utils.EPlatformType.pt_NES )
			{
				m_dict_CHR_palette_ind = new Dictionary<int, int>( utils.CONST_BLOCKS_UINT_SIZE );
			}
			
			m_map_data_app_src = map_data_config_provider.config_app( _prj_platform );
			m_map_data_app_dst = map_data_config_provider.config_app( platform_data_provider.get_platform_type() );
		}
		
		public void post_load_block_data( tiles_data _data )
		{
			if( m_dict_CHR_palette_ind != null )
			{
				int i;
				byte[] img_buff = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
				
				foreach( var obj in m_dict_CHR_palette_ind ) 
				{
					_data.from_CHR_bank_to_spr8x8( obj.Key, img_buff );
					{
						for( i = 0; i < img_buff.Length; i++ )
						{
							img_buff[ i ] += ( byte )( obj.Value * utils.CONST_PALETTE_SMALL_NUM_COLORS );
						}
					}
					_data.from_spr8x8_to_CHR_bank( obj.Key, img_buff );
				}
				
				m_dict_CHR_palette_ind.Clear();
				m_dict_CHR_palette_ind = null;
			}
		}
		
		public virtual void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank ){}
		
		public uint convert_block_data( byte _ver, utils.EPlatformType _prj_platform, uint _data )
		{
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
			}
			
			return m_map_data_app_dst.repack( m_map_data_app_src, _data );
		}
	}
	
	// NES data converter
	public class project_data_converter_NES : project_data_converter_base
	{
		public project_data_converter_NES()
		{
			//...
		}
		
		public override void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank )
		{
			int i;
			int num_pages;
			
			switch( _prj_platform )
			{
				case utils.EPlatformType.pt_SMS:
				case utils.EPlatformType.pt_PCE:
				{
					for( i = 0; i < _CHR_bank.Length; i++ )
					{
						_CHR_bank[ i ] = ( byte )( _br.ReadByte() & 0x03 );
					}
					
					if( _prj_platform == utils.EPlatformType.pt_SMS )
					{
						num_pages = utils.CONST_SMS_CHR_BANK_NUM_PAGES;
					}
					else
					{
						num_pages = utils.CONST_PCE_CHR_BANK_NUM_PAGES;
					}
					
					// skip the rest data
					_br.ReadBytes( ( num_pages * utils.CONST_CHR_BANK_PAGE_SIZE ) - _CHR_bank.Length );
				}
				break;
				
				default:
				{
					_CHR_bank 	= _br.ReadBytes( _CHR_bank.Length );
				}
				break;
			}
		}
	}
	
	// SMS data converter
	public class project_data_converter_SMS : project_data_converter_base
	{
		public project_data_converter_SMS()
		{
			//...
		}
		
		public override void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank )
		{
			int i;
			
			switch( _prj_platform )
			{
				case utils.EPlatformType.pt_NES:
				{
					for( i = 0; i < ( utils.CONST_NES_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ); i++ )
					{
						_CHR_bank[ i ] = _br.ReadByte();
					}
				}
				break;

				case utils.EPlatformType.pt_PCE:
				{
					for( i = 0; i < _CHR_bank.Length; i++ )
					{
						_CHR_bank[ i ] = _br.ReadByte();
					}
					
					// skip the rest data
					_br.ReadBytes( ( utils.CONST_PCE_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ) - _CHR_bank.Length );
				}
				break;
				
				default:
				{
					_CHR_bank 	= _br.ReadBytes( _CHR_bank.Length );
				}
				break;
			}
		}
	}
	
	// PCE data converter
	public class project_data_converter_PCE : project_data_converter_base
	{
		public project_data_converter_PCE()
		{
			//...
		}
		
		public override void load_CHR_bank( byte _ver, BinaryReader _br, utils.EPlatformType _prj_platform, ref byte[] _CHR_bank )
		{
			int i;
			
			switch( _prj_platform )
			{
				case utils.EPlatformType.pt_NES:
				{
					for( i = 0; i < ( utils.CONST_NES_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ); i++ )
					{
						_CHR_bank[ i ] = _br.ReadByte();
					}
				}
				break;
				
				case utils.EPlatformType.pt_SMS:
				{
					for( i = 0; i < ( utils.CONST_SMS_CHR_BANK_NUM_PAGES * utils.CONST_CHR_BANK_PAGE_SIZE ); i++ )
					{
						_CHR_bank[ i ] = _br.ReadByte();
					}
				}
				break;
				
				default:
				{
					_CHR_bank 	= _br.ReadBytes( _CHR_bank.Length );
				}
				break;
			}
		}
	}
	
	public static class project_data_converter_provider
	{
		private static readonly project_data_converter_NES m_project_data_converter_NES = new project_data_converter_NES();
		private static readonly project_data_converter_SMS m_project_data_converter_SMS = new project_data_converter_SMS();
		private static readonly project_data_converter_PCE m_project_data_converter_PCE = new project_data_converter_PCE();
		
		private static readonly Dictionary< utils.EPlatformType, i_project_data_converter > m_project_data_converter_dict = new Dictionary< utils.EPlatformType, i_project_data_converter >();
		
		static project_data_converter_provider()
		{
			m_project_data_converter_dict.Add( utils.EPlatformType.pt_NES, new project_data_converter_NES() );
			m_project_data_converter_dict.Add( utils.EPlatformType.pt_SMS, new project_data_converter_SMS() );
			m_project_data_converter_dict.Add( utils.EPlatformType.pt_PCE, new project_data_converter_PCE() );
		}
		
		public static i_project_data_converter get_converter()
		{
			return m_project_data_converter_dict[ platform_data_provider.get_platform_type() ];
		}
	}
}
