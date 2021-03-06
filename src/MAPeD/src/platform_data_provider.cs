﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 26.03.2021
 * Time: 12:02
 */
using System;

namespace MAPeD
{
	/// <summary>
	/// Description of platform_data_provider.
	/// </summary>
	public static class platform_data_provider
	{
		public static int get_palette_size_by_file_ext( string _file_ext )
		{
			switch( _file_ext )
			{
				case utils.CONST_NES_FILE_EXT:
					{
						return utils.CONST_NES_PALETTE_NUM_COLORS;
					}
					break;
					
				case utils.CONST_SMS_FILE_EXT:
					{
						return utils.CONST_SMS_PALETTE_NUM_COLORS;
					}
					break;
					
				case utils.CONST_PCE_FILE_EXT:
					{
						return utils.CONST_PCE_PALETTE_NUM_COLORS;
					}
					break;
			}
			
			throw new Exception( "platform_data_provider.get_palette_size_by_file_ext(...) : invalid parameter!" );
		}
		
		public static int get_scr_tiles_cnt_by_file_ext( string _file_ext )
		{
			switch( _file_ext )
			{
				case utils.CONST_NES_FILE_EXT:
					{
						return utils.CONST_NES_SCREEN_NUM_WIDTH_TILES * utils.CONST_NES_SCREEN_NUM_HEIGHT_TILES;
					}
					break;
					
				case utils.CONST_SMS_FILE_EXT:
					{
						return utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES;
					}
					break;
					
				case utils.CONST_PCE_FILE_EXT:
					{
						return utils.CONST_PCE_SCREEN_NUM_WIDTH_TILES * utils.CONST_PCE_SCREEN_NUM_HEIGHT_TILES;
					}
					break;
			}
			
			throw new Exception( "platform_data_provider.get_scr_tiles_cnt_by_file_ext(...) : invalid parameter!" );
		}
		
		private readonly static int[] NES_palette = new int[]{ 	0x7C7C7C, 0x0000FC, 0x0000BC, 0x4428BC, 0x940084, 0xA80020, 0xA81000, 0x881400,
																0x503000, 0x007800, 0x006800, 0x005800, 0x004058, 0x000000, 0x000000, 0x000000,
																0xBCBCBC, 0x0078F8, 0x0058F8, 0x6844FC, 0xD800CC, 0xE40058, 0xF83800, 0xE45C10,
																0xAC7C00, 0x00B800, 0x00A800, 0x00A844, 0x008888, 0x000000, 0x000000, 0x000000,
																0xF8F8F8, 0x3CBCFC, 0x6888FC, 0x9878F8, 0xF878F8, 0xF85898, 0xF87858, 0xFCA044,
																0xF8B800, 0xB8F818, 0x58D854, 0x58F898, 0x00E8D8, 0x787878, 0x000000, 0x000000,
																0xFCFCFC, 0xA4E4FC, 0xB8B8F8, 0xD8B8F8, 0xF8B8F8, 0xF8A4C0, 0xF0D0B0, 0xFCE0A8,
																0xF8D878, 0xD8F878, 0xB8F8B8, 0xB8F8D8, 0x00FCFC, 0xF8D8F8, 0x000000, 0x000000 };
		
		private readonly static int[] SMS_palette = new int[]{ 	0x000000, 0x550000, 0xaa0000, 0xff0000,	0x005500, 0x555500, 0xaa5500, 0xff5500,
																0x00aa00, 0x55aa00, 0xaaaa00, 0xffaa00, 0x00ff00, 0x55ff00, 0xaaff00, 0xffff00,
																0x000055, 0x550055, 0xaa0055, 0xff0055, 0x005555, 0x555555, 0xaa5555, 0xff5555,
																0x00aa55, 0x55aa55, 0xaaaa55, 0xffaa55, 0x00ff55, 0x55ff55, 0xaaff55, 0xffff55,
																0x0000aa, 0x5500aa, 0xaa00aa, 0xff00aa, 0x0055aa, 0x5555aa, 0xaa55aa, 0xff55aa,
																0x00aaaa, 0x55aaaa, 0xaaaaaa, 0xffaaaa, 0x00ffaa, 0x55ffaa, 0xaaffaa, 0xffffaa,
																0x0000ff, 0x5500ff, 0xaa00ff, 0xff00ff, 0x0055ff, 0x5555ff, 0xaa55ff, 0xff55ff,
																0x00aaff, 0x55aaff, 0xaaaaff, 0xffaaff, 0x00ffff, 0x55ffff, 0xaaffff, 0xffffff };
		
		private static int[] PCE_palette = null;
		
		public static int[] get_palette_by_file_ext( string _file_ext )
		{
			switch( _file_ext )
			{
				case utils.CONST_NES_FILE_EXT:
					{
						return NES_palette;
					}
					break;
					
				case utils.CONST_SMS_FILE_EXT:
					{
						return SMS_palette;
					}
					break;
					
				case utils.CONST_PCE_FILE_EXT:
					{
						if( PCE_palette == null )
						{
							PCE_palette = new int[ utils.CONST_PCE_PALETTE_NUM_COLORS ];
							
							int r, g, b;
				
							for( int i = 0; i < utils.CONST_PCE_PALETTE_NUM_COLORS; i++ )
							{
							   b = 36 * ( i & 0x007 );
							   r = 36 * ( ( i & 0x038 ) >> 3 );
							   g = 36 * ( ( i & 0x1c0 ) >> 6 );
							   
							   PCE_palette[ i ] = ( r << 16 ) | ( g << 8 ) | b;
							}
						}
						
						return PCE_palette;
					}
					break;
			}
			
			throw new Exception( "platform_data_provider.get_palette_by_file_ext(...) : invalid parameter!" );
		}
		
		public static utils.EPlatformType get_platform_by_ext( string _file_ext )
		{
			switch( _file_ext )
			{
				case utils.CONST_NES_FILE_EXT:
					{
						return utils.EPlatformType.pt_NES;
					}
					break;
					
				case utils.CONST_SMS_FILE_EXT:
					{
						return utils.EPlatformType.pt_SMS;
					}
					break;
					
				case utils.CONST_PCE_FILE_EXT:
					{
						return utils.EPlatformType.pt_PCE; 
					}
					break;
			}
			
			throw new Exception( "platform_data_provider.get_platform_by_ext(...) : invalid parameter!" );
		}
		
		private static utils.EPlatformType	m_platform = utils.EPlatformType.pt_UNKNOWN;

		public static utils.EPlatformType get_platform_type()
		{
			return m_platform;
		}
		
		static platform_data_provider()
		{
			switch( utils.CONST_PLATFORM )
			{
				case utils.CONST_PLATFORM_NES:
					{
						m_platform = utils.EPlatformType.pt_NES;
					}
					break;
					
				case utils.CONST_PLATFORM_SMS:
					{
						m_platform = utils.EPlatformType.pt_SMS;
					}
					break;
					
				case utils.CONST_PLATFORM_PCE:
					{
						m_platform = utils.EPlatformType.pt_PCE; 
					}
					break;
					
				default:
					{
						throw new Exception( "platform_data_provider : unknown platform detected!" );
					}
					break;
			}
		}
	}
}
