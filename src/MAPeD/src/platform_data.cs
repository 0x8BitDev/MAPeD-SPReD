/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 26.03.2021
 * Time: 12:02
 */
using System;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of platform_data.
	/// </summary>
	public static class platform_data
	{
		public const string CONST_PLATFORM_NES		= "NES";
		public const string CONST_PLATFORM_NES_DESC	= "Nintendo Intertainment System";
		
		public const string CONST_PLATFORM_SMS		= "SMS";
		public const string CONST_PLATFORM_SMS_DESC	= "Sega Master System";
		
		public const string CONST_PLATFORM_PCE		= "PCE";
		public const string CONST_PLATFORM_PCE_DESC	= "PC-Engine / TurboGrafx-16";

		public const string CONST_PLATFORM_ZX		= "ZX";
		public const string CONST_PLATFORM_ZX_DESC	= "ZX Spectrum";

		public const string CONST_PLATFORM_SMD		= "SMD";
		public const string CONST_PLATFORM_SMD_DESC	= "Sega Mega Drive / Genesis";
		
		public const string CONST_NES_FILE_EXT	= "mapednes";
		public const string CONST_SMS_FILE_EXT	= "mapedsms";
		public const string CONST_PCE_FILE_EXT	= "mapedpce";
		public const string CONST_ZX_FILE_EXT	= "mapedzx";
		public const string CONST_SMD_FILE_EXT	= "mapedsmd";

		public enum EPlatformType
		{
			pt_NES = 0,
			pt_SMS,
			pt_PCE,
			pt_ZX,
			pt_SMD,
			pt_UNKNOWN
		}
		
		private static readonly string[] CONST_PLATFORM_NAMES_ARR = new string[]
		{
			CONST_PLATFORM_NES,
			CONST_PLATFORM_SMS,
			CONST_PLATFORM_PCE,
			CONST_PLATFORM_ZX,
			CONST_PLATFORM_SMD,
		};
		
		public static readonly string[] CONST_PLATFORMS_FILE_EXT_ARR = new string[]
		{
			CONST_NES_FILE_EXT,
			CONST_SMS_FILE_EXT,
			CONST_PCE_FILE_EXT,
			CONST_ZX_FILE_EXT,
			CONST_SMD_FILE_EXT,
		};
		
#if DEF_NES
		public const string	CONST_PLATFORM		= CONST_PLATFORM_NES;
		public const string	CONST_PLATFORM_DESC	= CONST_PLATFORM_NES_DESC;
#elif DEF_SMS
		public const string	CONST_PLATFORM		= CONST_PLATFORM_SMS;
		public const string	CONST_PLATFORM_DESC	= CONST_PLATFORM_SMS_DESC;
#elif DEF_PCE
		public const string	CONST_PLATFORM		= CONST_PLATFORM_PCE;
		public const string	CONST_PLATFORM_DESC	= CONST_PLATFORM_PCE_DESC;
#elif DEF_ZX
		public const string	CONST_PLATFORM		= CONST_PLATFORM_ZX;
		public const string	CONST_PLATFORM_DESC	= CONST_PLATFORM_ZX_DESC;
#elif DEF_SMD
		public const string	CONST_PLATFORM		= CONST_PLATFORM_SMD;
		public const string	CONST_PLATFORM_DESC	= CONST_PLATFORM_SMD_DESC;
#else
		public const string	CONST_PLATFORM		= "UNKNOWN";
		public const string	CONST_PLATFORM_DESC	= "UNKNOWN";
#endif
		
		private static readonly EPlatformType	m_platform = EPlatformType.pt_UNKNOWN;
		
		private static readonly Dictionary< string, EPlatformType >	m_file_ext_platform_type_dict		= new Dictionary< string, EPlatformType >();
		private static readonly Dictionary< string, EPlatformType >	m_platform_name_platform_type_dict	= new Dictionary< string, EPlatformType >();
		
		private static readonly int[] m_platform_blocks_cnt = new int[]
		{
			utils.CONST_NES_MAX_BLOCKS_CNT,
			utils.CONST_SMS_MAX_BLOCKS_CNT,
			utils.CONST_PCE_MAX_BLOCKS_CNT,
			utils.CONST_ZX_MAX_BLOCKS_CNT,
			utils.CONST_SMD_MAX_BLOCKS_CNT,
		};

		private static readonly int[] m_platform_tiles_cnt = new int[]
		{
			utils.CONST_NES_MAX_TILES_CNT,
			utils.CONST_SMS_MAX_TILES_CNT,
			utils.CONST_PCE_MAX_TILES_CNT,
			utils.CONST_ZX_MAX_TILES_CNT,
			utils.CONST_SMD_MAX_TILES_CNT,
		};
		
		private static readonly int[] m_palette_colors_cnt = new int[]
		{
			utils.CONST_NES_PALETTE_NUM_COLORS,
			utils.CONST_SMS_PALETTE_NUM_COLORS,
			utils.CONST_PCE_SMD_PALETTE_NUM_COLORS,
			utils.CONST_ZX_PALETTE_NUM_COLORS,
			utils.CONST_PCE_SMD_PALETTE_NUM_COLORS,
		};
		
		private static readonly int[] m_screen_tiles_cnt = new int[]
		{
			utils.CONST_NES_SCREEN_NUM_WIDTH_TILES * utils.CONST_NES_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_PCE_SCREEN_NUM_WIDTH_TILES * utils.CONST_PCE_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_ZX_SCREEN_NUM_WIDTH_TILES * utils.CONST_ZX_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_SMD_SCREEN_NUM_WIDTH_TILES * utils.CONST_SMD_SCREEN_NUM_HEIGHT_TILES,
		};
		
		private static readonly int[] m_CHR_bank_pages_cnt = new int[]
		{
			utils.CONST_NES_CHR_BANK_NUM_PAGES,
			utils.CONST_SMS_CHR_BANK_NUM_PAGES,
			utils.CONST_PCE_CHR_BANK_NUM_PAGES,
			utils.CONST_ZX_CHR_BANK_NUM_PAGES,
			utils.CONST_SMD_CHR_BANK_NUM_PAGES,
		};

		private static readonly int[] m_platform_screen_width_tiles_cnt = new int[]
		{
			utils.CONST_NES_SCREEN_NUM_WIDTH_TILES,
			utils.CONST_SMS_SCREEN_NUM_WIDTH_TILES,
			utils.CONST_PCE_SCREEN_NUM_WIDTH_TILES,
			utils.CONST_ZX_SCREEN_NUM_WIDTH_TILES,
			utils.CONST_SMD_SCREEN_NUM_WIDTH_TILES,
		};

		private static readonly int[] m_platform_screen_height_tiles_cnt = new int[]
		{
			utils.CONST_NES_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_SMS_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_PCE_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_ZX_SCREEN_NUM_HEIGHT_TILES,
			utils.CONST_SMD_SCREEN_NUM_HEIGHT_TILES,
		};

		private static readonly int[] m_platform_screen_width_blocks_cnt = new int[]
		{
			utils.CONST_NES_SCREEN_NUM_WIDTH_BLOCKS,
			utils.CONST_SMS_SCREEN_NUM_WIDTH_BLOCKS,
			utils.CONST_PCE_SCREEN_NUM_WIDTH_BLOCKS,
			utils.CONST_ZX_SCREEN_NUM_WIDTH_BLOCKS,
			utils.CONST_SMD_SCREEN_NUM_WIDTH_BLOCKS,
		};

		private static readonly int[] m_platform_screen_height_blocks_cnt = new int[]
		{
			utils.CONST_NES_SCREEN_NUM_HEIGHT_BLOCKS,
			utils.CONST_SMS_SCREEN_NUM_HEIGHT_BLOCKS,
			utils.CONST_PCE_SCREEN_NUM_HEIGHT_BLOCKS,
			utils.CONST_ZX_SCREEN_NUM_HEIGHT_BLOCKS,
			utils.CONST_SMD_SCREEN_NUM_HEIGHT_BLOCKS,
		};
		
		static platform_data()
		{
			int	i;
			
			for( i = 0; i < CONST_PLATFORM_NAMES_ARR.Length; i++ )
			{
				m_platform_name_platform_type_dict[ CONST_PLATFORM_NAMES_ARR[ i ] ] = ( EPlatformType )i;
			}
			
			m_platform = m_platform_name_platform_type_dict[ CONST_PLATFORM ];
			
			for( i = 0; i < CONST_PLATFORMS_FILE_EXT_ARR.Length; i++ )
			{
				m_file_ext_platform_type_dict[ CONST_PLATFORMS_FILE_EXT_ARR[ i ] ] = ( EPlatformType )i;
			}
		}
		
		public static EPlatformType get_platform_type_by_file_ext( string _file_ext )
		{
			return m_file_ext_platform_type_dict[ _file_ext ];
		}

		public static EPlatformType get_platform_type_by_name( string _name )
		{
			return m_platform_name_platform_type_dict[ _name ];
		}
		
		public static string get_platform_name_by_type( EPlatformType _type )
		{
			return CONST_PLATFORM_NAMES_ARR[ ( int )_type ];
		}

		public static int get_blocks_cnt_by_file_ext( string _file_ext )
		{
			return m_platform_blocks_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
		}

		public static int get_tiles_cnt_by_file_ext( string _file_ext )
		{
			return m_platform_tiles_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
		}
		
		public static int get_palette_size_by_file_ext( string _file_ext )
		{
			return m_palette_colors_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
		}
		
		public static int get_screen_tiles_cnt_by_file_ext( string _file_ext )
		{
			return m_screen_tiles_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
		}

		public static int get_CHR_bank_pages_cnt_by_platform_type( EPlatformType _type )
		{
			return m_CHR_bank_pages_cnt[ ( int )_type ];
		}
		
		public static int get_screen_tiles_width_by_file_ext_uni( string _file_ext, data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return m_platform_screen_width_tiles_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
			}
			else
			{
				return m_platform_screen_width_blocks_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
			}
		}

		public static int get_screen_tiles_height_by_file_ext_uni( string _file_ext, data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return m_platform_screen_height_tiles_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
			}
			else
			{
				return m_platform_screen_height_blocks_cnt[ ( int )get_platform_type_by_file_ext( _file_ext ) ];
			}
		}

		public static int get_screen_tiles_width_by_platform_type_uni( EPlatformType _platform_type, data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return m_platform_screen_width_tiles_cnt[ ( int )_platform_type ];
			}
			else
			{
				return m_platform_screen_width_blocks_cnt[ ( int )_platform_type ];
			}
		}

		public static int get_screen_tiles_height_by_platform_type_uni( EPlatformType _platform_type, data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return m_platform_screen_height_tiles_cnt[ ( int )_platform_type ];
			}
			else
			{
				return m_platform_screen_height_blocks_cnt[ ( int )_platform_type ];
			}
		}
		
		public static int get_screen_tiles_cnt_uni( data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return utils.CONST_SCREEN_TILES_CNT;
			}
			else
			{
				return utils.CONST_SCREEN_BLOCKS_CNT;
			}
		}
		
		public static int get_screen_num_width_tiles_uni( data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return utils.CONST_SCREEN_NUM_WIDTH_TILES;
			}
			else
			{
				return utils.CONST_SCREEN_NUM_WIDTH_BLOCKS;
			}
		}
		
		public static int get_screen_num_height_tiles_uni( data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return utils.CONST_SCREEN_NUM_HEIGHT_TILES;
			}
			else
			{
				return utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS;
			}
		}
		
		public static int get_screen_tiles_size_uni( data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return utils.CONST_SCREEN_TILES_SIZE;
			}
			else
			{
				return utils.CONST_SCREEN_BLOCKS_SIZE;
			}
		}
		
		public static screen_editor.EFillMode get_screen_fill_mode_uni( data_sets_manager.EScreenDataType _type )
		{
			if( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				return screen_editor.EFillMode.efm_Tile;
			}
			else
			{
				return screen_editor.EFillMode.efm_Block;
			}
		}
		
		public static EPlatformType get_platform_type()
		{
			return m_platform;
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
		
		private static int[] PCE_SMD_palette = null;
		
		// 7: flashing, 6: brightness, 3-5: paper color, 0-2: ink color
		private static int[] ZX_palette = new int[]{  0x000000, 0x0022c7, 0xd62816, 0xd433c7, 0x00c525, 0x00c7c9, 0xccc82a, 0xcacaca,	// not bright
													  0x000000, 0x002bfb, 0xff331c, 0xff40fc, 0x00f92f, 0x00fbfe, 0xfffc36, 0xffffff };	// bright
		
		public static int[] get_palette_by_file_ext( string _file_ext )
		{
			switch( _file_ext )
			{
				case CONST_NES_FILE_EXT:
					{
						return NES_palette;
					}
					break;
					
				case CONST_SMS_FILE_EXT:
					{
						return SMS_palette;
					}
					break;
					
				case CONST_SMD_FILE_EXT:
				case CONST_PCE_FILE_EXT:
					{
						if( PCE_SMD_palette == null )
						{
							PCE_SMD_palette = new int[ utils.CONST_PCE_SMD_PALETTE_NUM_COLORS ];
							
							int r, g, b;
				
							for( int i = 0; i < utils.CONST_PCE_SMD_PALETTE_NUM_COLORS; i++ )
							{
							   b = 36 * ( i & 0x007 );
							   r = 36 * ( ( i & 0x038 ) >> 3 );
							   g = 36 * ( ( i & 0x1c0 ) >> 6 );
							   
							   PCE_SMD_palette[ i ] = ( r << 16 ) | ( g << 8 ) | b;
							}
						}
						
						return PCE_SMD_palette;
					}
					break;
					
				case CONST_ZX_FILE_EXT:
					{
						return ZX_palette;
					}
					break;
			}
			
			throw new Exception( "platform_data.get_palette_by_file_ext(...) : invalid parameter!" );
		}
	}
}
