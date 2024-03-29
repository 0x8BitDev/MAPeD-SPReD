﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 16.03.2017
 * Time: 16:35
 */
 
/*
defines:
~~~~~~~~
DEF_TILE_DRAW_FAST				- DEPRECATED!
DEF_FLIP_BLOCKS_SPR_BY_FLAGS	(SMS/SMD)
DEF_PALETTE16_PER_CHR			(PCE/SMS/ZX/SMD)
DEF_FIXED_LEN_PALETTE16_ARR		(PCE/SMS/ZX/SMD)
DEF_PLATFORM_16BIT				(SMD)
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

// Json formatter
using System.Text;
using System.Linq;
using System.Collections.Generic;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace MAPeD
{
	/// <summary>
	/// Description of utils.
	/// </summary>
	public static class utils
	{
		public	const bool CONST_DEV_BUILD_FLAG	= true;
		private	const bool CONST_BETA_FLAG		= true; 

		public const string CONST_APP_NAME		= "MAPeD";
		
		public static readonly string[] CONST_FULL_APP_NAMES_ARR = new string[]
		{
			utils.CONST_APP_NAME + "-" + platform_data.CONST_PLATFORM_NES,
			utils.CONST_APP_NAME + "-" + platform_data.CONST_PLATFORM_SMS,
			utils.CONST_APP_NAME + "-" + platform_data.CONST_PLATFORM_PCE,
			utils.CONST_APP_NAME + "-" + platform_data.CONST_PLATFORM_ZX,
			utils.CONST_APP_NAME + "-" + platform_data.CONST_PLATFORM_SMD,
		};
		
#if	DEBUG
		public const string	CONST_BUILD_CFG	= " [DEBUG]";
#else
		public const string	CONST_BUILD_CFG	= "";
#endif

		// OS detection code implemented by jarik ( 100% managed code ) https://stackoverflow.com/a/38795621
		public static bool is_win	= false;
		public static bool is_linux = false;
		public static bool is_macos = false;
		
		public static void check_os()
		{
			string windir = Environment.GetEnvironmentVariable("windir");
			
			if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
			{
				is_win = true;
			}
			else if (File.Exists(@"/proc/sys/kernel/ostype"))
			{
			    string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
			    if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
			    {
			        // Note: Android gets here too
			        is_linux = true;
			    }
			    else
			    {
			        throw new Exception( "Unsupported platform detected!" );
			    }
			}
			else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
			{
			    // Note: iOS gets here too
			    is_macos = true;
			}
		    else
		    {
		        throw new Exception( "Unsupported platform detected!" );
		    }
		}

		private static readonly	Version 	ver			= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly	string		build_str	= "Build: " + ver.Build;
		public static readonly	DateTime 	build_date 	= new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds( ver.Revision * 2 );

		public static readonly string CONST_APP_VER			= "v" + ver.Major + "." + ver.Minor + ( CONST_BETA_FLAG ? "b ":" " ) + ( CONST_DEV_BUILD_FLAG ? "Dev":"" ) + CONST_BUILD_CFG;
		public static readonly string CONST_FULL_APP_NAME	= CONST_FULL_APP_NAMES_ARR[ ( int )platform_data.get_platform_type() ] + ( Environment.Is64BitProcess ? " (x64)":" (x86)" );
		
		public const uint CONST_PROJECT_FILE_MAGIC	= 'S'<<24 | 'N'<<16 | 'e'<<8 | 'M';
		public const byte CONST_PROJECT_FILE_VER	= 8;
		// v1: initial data format
		// v2: added palettes array
		// v3: blocks data USHORT -> UINT; palette index BYTE -> INT
		// v4: pre data flags: screen data type ( Tiles4X4 / Blocks2X2 )
		// v5: screen data -> USHORT
		// v6: tiles 4x4 data -> ULONG
		// v7: save palette in a project file for all platforms
		// v8: layout screen index BYTE -> INT

		public const uint CONST_SPRED_FILE_MAGIC		= 'S'<<24 | 'N'<<16 | 'e'<<8 | 'S';
		public const uint CONST_SPRED_PROJECT_FILE_VER	= 1;
		
		public const byte 	CONST_IO_DATA_TILES_AND_SCREENS	= 0x01;
		public const byte 	CONST_IO_DATA_LAYOUT			= 0x02;
		public const byte 	CONST_IO_DATA_ENTITIES			= 0x04;
		public const byte	CONST_IO_DATA_PALETTE			= 0x08;
		public const byte	CONST_IO_DATA_TILES_PATTERNS	= 0x10;
		public const byte	CONST_IO_DATA_END				= 0xff;

		public const uint	CONST_IO_DATA_PRE_FLAG_SCR_TILES4X4		= 0x01;
		public const uint	CONST_IO_DATA_PRE_FLAG_SCR_BLOCKS_WH	= 0x02;
		
		public const uint	CONST_IO_DATA_POST_FLAG_MMC5			= 0x01;
		public const uint	CONST_IO_DATA_POST_FLAG_PROP_PER_CHR	= 0x02;
		
#if DEF_ZX
		public const byte CONST_ZX_DEFAULT_PAPER_COLOR		= 15;

		private const uint CONST_DRAW_BLOCK_FLAGS_BW		= 0x01;
		private const uint CONST_DRAW_BLOCK_FLAGS_INV_BW	= 0x02;
		
		private const uint CONST_ZX_PALETTE_BW				= 0;	// black and white mode
		private const uint CONST_ZX_PALETTE_INV_BW			= 1;	// inverse black and white mode
		
		private readonly static palette16_data[] zx_alt_palettes	= new palette16_data[ 2 ]{	new palette16_data( new int[ 16 ]{ 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 7, 7, 7, 7, 7 } ),
																								new palette16_data( new int[ 16 ]{ 7, 7, 7, 7, 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 0, 0 } ) };
		public static uint get_draw_block_flags_by_view_type( e_tile_view_type _view_type )
		{
			return ( _view_type == e_tile_view_type.BW ) ? CONST_DRAW_BLOCK_FLAGS_BW:( _view_type == e_tile_view_type.InvBW ) ? CONST_DRAW_BLOCK_FLAGS_INV_BW:0;
		}
		
		public static palette16_data get_draw_block_palette_by_view_type( e_tile_view_type _view_type, palette16_data _default_plt )
		{
			return ( _view_type == e_tile_view_type.BW ) ? zx_alt_palettes[ CONST_ZX_PALETTE_BW ]:( _view_type == e_tile_view_type.InvBW ) ? zx_alt_palettes[ CONST_ZX_PALETTE_INV_BW ]:_default_plt;
		}
		
		public static palette16_data get_draw_block_palette_by_draw_block_flags( uint _flags )
		{
			return ( _flags == CONST_DRAW_BLOCK_FLAGS_BW ) ? zx_alt_palettes[ CONST_ZX_PALETTE_BW ]:( _flags == CONST_DRAW_BLOCK_FLAGS_INV_BW ) ? zx_alt_palettes[ CONST_ZX_PALETTE_INV_BW ]:null;
		}
#endif

		public const int CONST_SCREEN_TILES_SIZE		= 64;	// pixels
		public const int CONST_SCREEN_BLOCKS_SIZE		= 32;	// pixels
		
		public const int CONST_SCREEN_MAX_CNT			= 4096;
		
		public const int CONST_LAYOUT_MAX_CNT			= 256;

		public const int CONST_SPR8x8_SIDE_PIXELS_CNT			= 8;
		public const int CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS	= 3;	// 2->4->8 = 3bits
		public const int CONST_SPR8x8_TOTAL_PIXELS_CNT			= CONST_SPR8x8_SIDE_PIXELS_CNT * CONST_SPR8x8_SIDE_PIXELS_CNT;
		
		public const int CONST_TILES_IMG_SIZE		= 64;
		public const int CONST_BLOCKS_IMG_SIZE		= 32;
		
		public const int CONST_NUM_SMALL_PALETTES 			= 4;
		public const int CONST_PALETTE_SMALL_NUM_COLORS		= 4;
		
		public const int CONST_PALETTES_MAX_CNT				= 16;
		
		public const int CONST_CHR_BANK_PAGE_SIDE			= 128;
		public const int CONST_CHR_BANK_PAGE_SIZE			= CONST_CHR_BANK_PAGE_SIDE * CONST_CHR_BANK_PAGE_SIDE;
		public const int CONST_CHR_BANK_PAGE_SPRITES_CNT	= 256;
		public const int CONST_CHR_BANK_MAX_CNT				= 256;
		
		public const int CONST_BLOCK_SIZE					= 4;	// uints
		public const int CONST_TILE_SIZE					= 4;	// ushorts
		
#if DEF_PLATFORM_16BIT
		public const int CONST_MAX_ENT_INST_CNT				= 0x10000;	// max number of entities instances in a map
#else
		public const int CONST_MAX_ENT_INST_CNT				= 0x100;	// max number of entities instances in a map
#endif

		public const byte CONST_CHR_ATTR_FLAG_HFLIP			= 0x01;
		public const byte CONST_CHR_ATTR_FLAG_VFLIP			= 0x02;
		
		public const SKColorType CONST_BMP_FORMAT			= SKColorType.Rgba8888;
		
		public static Pen			pen				= new Pen( CONST_COLOR_PEN_DEFAULT );
		public static SolidBrush 	brush		 	= new SolidBrush( CONST_COLOR_BRUSH_DEFAULT );
		public static readonly Font fnt8_Arial		= new Font( "Arial", 8, 	FontStyle.Bold );
		public static readonly Font fnt10_Arial		= new Font( "Arial", 10, 	FontStyle.Bold );
		public static readonly Font fnt12_Arial		= new Font( "Arial", 12, 	FontStyle.Bold );
		public static readonly Font fnt42_Arial		= new Font( "Arial", 42, 	FontStyle.Bold );
		public static readonly Font fnt64_Arial		= new Font( "Arial", 64, 	FontStyle.Bold );
		
		private static readonly Dictionary< uint, SKPaint > _fonts = new Dictionary< uint, SKPaint >( 10 );
		
		public static SKPaint get_font( uint _size, bool _bold )
		{
			uint key = ( uint )( ( int )_size | ( ( _bold ? 1:0 ) << 8 ) );
			
			if( _fonts.ContainsKey( key ) )
			{
				return _fonts[ key ];
			}
			
			SKFontStyle font_style = new SKFontStyle( SKFontStyleWeight.Bold, SKFontStyleWidth.ExtraExpanded, SKFontStyleSlant.Upright );
			
			SKTypeface type_face = SKFontManager.Default.MatchCharacter( "Arial", font_style, new string[]{}, 'a' );
			
			SKFont fnt = new SKFont( type_face, _size, 1 );
			
			SKPaint font_paint		= new SKPaint( fnt );
			font_paint.Color		= new SKColor( 0xffffffff );
			font_paint.IsAntialias	= true;
			font_paint.SubpixelText	= true;
			
			_fonts[ key ] = font_paint;
			
			return font_paint;
		}
		
		public static byte[] tmp_spr8x8_buff = new byte[ CONST_SPR8x8_TOTAL_PIXELS_CNT ];

		//DEFAULT COLORS
		// entity editor
		public static readonly Color	CONST_COLOR_DEFAULT_ENTITY					= Color.Gold;
		public static readonly Color	CONST_COLOR_DEFAULT_ENTITY_INACTIVE			= Color.Black;
		public static readonly Color	CONST_COLOR_NULL							= Color.Empty;
		public static readonly Color	CONST_COLOR_ENTITY_PIXBOX_INACTIVE			= Color.Black;
		// layout
		public static readonly SKColor	CONST_COLOR_SIMPLE_SCREEN_CROSS				= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_SCREEN_LIST_NOT_EMPTY			= Extensions.ToSKColor( Color.White );
		public static readonly SKColor	CONST_COLOR_SCREEN_LIST_EMPTY 				= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_SCREEN_TRANSPARENT_BORDER		= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_SCREEN_SELECTED_BORDER			= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_SCREEN_ACTIVE					= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_SCREEN_SELECTION_AREA			= Extensions.ToSKColor( Color.SkyBlue );
		public static readonly SKColor	CONST_COLOR_LAYOUT_STRING_DEFAULT			= Extensions.ToSKColor( Color.White );
		public static readonly SKColor	CONST_COLOR_LAYOUT_STRING_DEFAULT_SHADOW	= Extensions.ToSKColor( Color.Black );
		public static readonly SKColor	CONST_COLOR_ENTITY_BORDER_EDIT_ENT_MODE		= Extensions.ToSKColor( Color.LimeGreen );
		public static readonly SKColor	CONST_COLOR_SELECTED_ENTITY_BORDER			= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_ENTITY_BORDER_EDIT_INST_MODE	= Extensions.ToSKColor( Color.Orange );
		public static readonly SKColor	CONST_COLOR_TARGET_LINK						= Extensions.ToSKColor( Color.LightGreen );
		public static readonly SKColor	CONST_COLOR_ENTITY_PIVOT					= Extensions.ToSKColor( Color.LightGreen );
		public static readonly SKColor	CONST_COLOR_GRID_BLOCKS						= new SKColor( 0x90a0a0a0 );
		public static readonly SKColor	CONST_COLOR_GRID_TILES_BRIGHT				= new SKColor( 0x90e0e0e0 );
		public static readonly SKColor	CONST_COLOR_GRID_TILES_DARK					= new SKColor( 0x88a0a0a0 );
		public static readonly SKColor	CONST_COLOR_MARK_START_SCREEN				= new SKColor( 0x7fff0000 );
		public static readonly SKColor	CONST_COLOR_MARK_SCREEN_MARK				= new SKColor( 0x7f0000ff );
		public static readonly SKColor	CONST_COLOR_MARK_ADJ_SCREEN_MARK			= new SKColor( 0x7f00ff00 );
		public static readonly SKColor	CONST_COLOR_LAYOUT_PIXBOX_DEFAULT			= Extensions.ToSKColor( Color.White );
		public static readonly SKColor	CONST_COLOR_LAYOUT_PIXBOX_INACTIVE_CROSS	= Extensions.ToSKColor( Color.Red );
		public static readonly SKColor	CONST_COLOR_TILE_BORDER						= Extensions.ToSKColor( Color.Red );
		// image list manager
		public static readonly Color	CONST_COLOR_TILE_CLEAR						= Color.Black;
		public static readonly Color	CONST_COLOR_BLOCK_CLEAR						= Color.Black;
		public static readonly Color	CONST_COLOR_SCREEN_CLEAR					= Color.Black;
		// screen editor active tile 
		public static readonly Color	CONST_COLOR_ACTIVE_TILE_BACKGROUND			= Color.Black;
		// screen editor
		public static readonly Color	CONST_COLOR_SCREEN_GRID_THICK_BLOCK_MODE	= Color.FromArgb( 0x7fffffff );
		public static readonly Color	CONST_COLOR_SCREEN_GRID_THICK_TILE_MODE		= Color.White;
		public static readonly Color	CONST_COLOR_SCREEN_GRID_THIN				= Color.FromArgb( 0x30ffffff );
		public static readonly Color	CONST_COLOR_SCREEN_BORDER					= Color.Red;
		public static readonly Color	CONST_COLOR_SCREEN_PATTERN_BORDER			= Color.Orange;
		public static readonly Color	CONST_COLOR_SCREEN_SELECTION_RECTANGLE		= Color.White;
		public static readonly SKColor	CONST_COLOR_SCREEN_SELECTION_TILE			= Extensions.ToSKColor( Color.DeepSkyBlue );
		public static readonly Color	CONST_COLOR_SCREEN_TRANSLUCENT_QUAD			= Color.FromArgb( unchecked( (int)0x40000000 ) );
		// block editor
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_GRID						= Color.FromArgb( 0x78808080 );
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_CHR_BORDER					= Color.White;
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_DRAW_MODE_CHR_SPLITTER		= Color.DarkGray;
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_SELECTED_CHR_OUTER_BORDER	= Color.Red;
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_SELECTED_CHR_INNER_BORDER	= Color.White;
		// CHR bank viewer
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_DEFAULT					= Color.Black;
		public static readonly Color	CONST_COLOR_CHR_BANK_GRID								= Color.FromArgb( 0x78808080 );
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER				= Color.Black;
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_BLOCK_CHR_BORDER			= Color.Yellow;
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_SELECT_MODE	= Color.Orange;
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_DRAW_MODE	= Color.White;
		// palette group
		public static readonly Color	CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER		= Color.Black; 
		public static readonly Color	CONST_COLOR_PALETTE_SELECTED_INNER_BORDER		= Color.White;
		public static readonly Color	CONST_COLOR_PALETTE_SWAP_COLOR_ACTIVE_BORDER	= Color.Red;
		public static readonly Color	CONST_COLOR_PALETTE_SWAP_COLOR_INACTIVE_BORDER	= Color.Black;
		public static readonly Color	CONST_COLOR_PALETTE_SWAP_COLOR_TEXT_DEFAULT		= Color.Black;
		// tile editor
		public static readonly Color	CONST_COLOR_TILE_EDITOR_TRANSLUCENT_QUAD		= Color.FromArgb( unchecked( (int)0x80000000 ) );
		public static readonly Color	CONST_COLOR_TILE_EDITOR_GRID					= Color.FromArgb( 0x78808080 );
		public static readonly Color	CONST_COLOR_TILE_EDITOR_BLOCK_BORDER			= Color.White;
		public static readonly Color	CONST_COLOR_TILE_EDITOR_SELECTED_INNER_BORDER	= Color.White;
		public static readonly Color	CONST_COLOR_TILE_EDITOR_SELECTED_OUTER_BORDER	= Color.Red;
		public static readonly Color	CONST_COLOR_TILE_EDITOR_LOCKED_LINE 			= Color.Red;
		// other
		public static readonly Color	CONST_COLOR_BRUSH_DEFAULT						= Color.White;
		public static readonly Color	CONST_COLOR_PEN_DEFAULT							= Color.Black;
		public static readonly Color	CONST_COLOR_PIXBOX_INACTIVE_CROSS				= Color.Red;
		public static readonly Color	CONST_COLOR_PIXBOX_DEFAULT						= Color.White;
		public static readonly Color	CONST_COLOR_IMG_PREVIEW_PIVOT_CROSS				= Color.OrangeRed;
		public static readonly Color	CONST_COLOR_IMG_PREVIEW_PIVOT_RECT 				= Color.LimeGreen;
		public static readonly Color	CONST_COLOR_STRING_DEFAULT						= Color.White;
		// tile list
		public static readonly Color	CONST_COLOR_TILE_LIST_BACKGROUND				= Color.LightGray;
		public static readonly Color	CONST_COLOR_TILE_LIST_GRID						= Color.White;
		public static readonly Color	CONST_COLOR_TILE_LIST_SELECTION					= Color.Red;
		
		public enum e_transform_type
		{
			VFlip,
			HFlip,
			Rotate,
		};
		
		public enum e_tile_view_type
		{
			UNKNOWN		= -1,
			
			Graphics	= 0,
			ObjectId	= 1,
			Number		= 2,
			TilesUsage	= 3,
			BlocksUsage	= 4,
#if DEF_ZX
			BW			= 5,
			InvBW		= 6,
#endif
		};
		
		private static SKRect tmp_rect = new SKRect();
		
		public static void draw_skbitmap( SKCanvas _canvas, SKBitmap _bmp, float _x, float _y, float _w, float _h, SKPaint _paint )
		{
			tmp_rect.Left	= _x;
			tmp_rect.Top	= _y;
			tmp_rect.Right	= _x + _w;
			tmp_rect.Bottom	= _y + _h;
			
			_canvas.DrawBitmap( _bmp, tmp_rect, _paint );
		}
		
		private static void flip_bmp( Bitmap _bmp, byte _flags )
		{
			if( ( _flags & CONST_CHR_ATTR_FLAG_HFLIP ) == CONST_CHR_ATTR_FLAG_HFLIP )
			{
				_bmp.RotateFlip( RotateFlipType.RotateNoneFlipX );
			}
			
			if( ( _flags & CONST_CHR_ATTR_FLAG_VFLIP ) == CONST_CHR_ATTR_FLAG_VFLIP )
			{
				_bmp.RotateFlip( RotateFlipType.RotateNoneFlipY );
			}
		}
		
		public static Bitmap create_bitmap( byte[] _arr, int _width, int _height, byte _flags, bool _alpha, int _plt_ind, palette16_data _plt = null, int _arr_offset = 0 )
		{
			int img_size = _width * _height;
			
			int[] img_buff = new int[ img_size ];
			
			GCHandle handle = GCHandle.Alloc( img_buff, GCHandleType.Pinned );
#if DEF_NES
			bool apply_palette 	= ( _plt != null && _plt_ind >= 0 );
			int[] clr_inds 	= apply_palette ? _plt.subpalettes[ _plt_ind ]:null;
			int alpha;
			
			apply_palette = (clr_inds != null) && apply_palette;
#endif			
			int clr;
			int pix_ind;

#if !DEF_NES
			// valid palette ?
			if( _plt != null )
#endif			
			{
				for( int p = 0; p < img_size; p++ )
				{
					pix_ind = _arr[ p + _arr_offset ];
#if DEF_NES				
					if( apply_palette )
#endif					
					{
#if DEF_NES					
						clr = palette_group.Instance.main_palette[ clr_inds[ pix_ind ] ];
#else
						clr = palette_group.Instance.main_palette[ _plt.subpalettes[ pix_ind / CONST_NUM_SMALL_PALETTES ][ pix_ind % CONST_NUM_SMALL_PALETTES ] ];
#endif
						if( ( clr != 0 && _alpha == true ) || _alpha == false )
						{
							clr |= 0xFF << 24;
						}
					}
#if DEF_NES				
					else
					{
						alpha = ( pix_ind == 0 ) ? ( _alpha ? 0x00:0xFF ):0xFF;
						
						pix_ind <<= 6;
						clr = alpha << 24 | pix_ind << 16 | pix_ind << 8 | pix_ind;
					}
#endif				
					img_buff[ p ] = clr;
				}
			}
			
			Bitmap bmp = new Bitmap( _width, _height, _width<<2, PixelFormat.Format32bppPArgb, handle.AddrOfPinnedObject() );

			handle.Free();
			
			flip_bmp( bmp, _flags );
			
			return bmp;
		}
		
		public static void update_block_gfx( int _block_id, tiles_data _data, Graphics _gfx, int _img_half_width, int _img_half_height, int _x_offs = 0, int _y_offs = 0, uint _flags = 0 )
		{
			Bitmap 	bmp;
			
			uint 	chr_data;
			byte 	flip_flag	= 0;
			int		plt_ind		= -1;
#if DEF_ZX
			palette16_data zx_plt = get_draw_block_palette_by_draw_block_flags( _flags );
#endif
			palette16_data plt16 = null;
			
			for( int j = 0; j < utils.CONST_BLOCK_SIZE; j++ )
			{
				chr_data = _data.blocks[ ( _block_id << 2 ) + j ];
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
				flip_flag 	= tiles_data.get_block_flags_flip( chr_data );
#endif
#if DEF_NES				
				plt_ind		= tiles_data.get_block_flags_palette( chr_data );
#endif

#if DEF_PALETTE16_PER_CHR
				plt16 = _data.palettes_arr[ tiles_data.get_block_flags_palette( chr_data ) ];
#if DEF_ZX
				plt16 = ( zx_plt != null ) ? zx_plt:plt16;
#endif
#else
				plt16 = _data.palettes_arr[ _data.palette_pos ];
#endif
				_data.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( chr_data ), utils.tmp_spr8x8_buff );
				
				bmp = utils.create_bitmap( utils.tmp_spr8x8_buff, 8, 8, flip_flag, false, plt_ind, plt16 );
				
				_gfx.DrawImage( bmp, _x_offs + ( ( j % 2 ) * _img_half_width ), _y_offs + ( ( j >> 1 ) * _img_half_height ), _img_half_width, _img_half_height );
				
				bmp.Dispose();
			}
		}
		
		public static string get_screen_id_str( int _bank_n, int _screen_n )
		{
			return String.Format( "B:{0}/S:{1}", _bank_n, _screen_n );
		}

		// JSON formatter implemented by Peter Long (https://stackoverflow.com/a/6237866)
		private const string INDENT_STRING = "    ";
		public static string format_Json( string str )
		{
			var indent = 0;
			var quoted = false;
			var sb = new StringBuilder();
			for (var i = 0; i < str.Length; i++)
			{
				var ch = str[i];
				switch (ch)
				{
					case '{':
					case '[':
						sb.Append(ch);
						if (!quoted)
						{
							sb.AppendLine();
							Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
						}
						break;
					case '}':
					case ']':
						if (!quoted)
						{
							sb.AppendLine();
							Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
						}
						sb.Append(ch);
						break;
					case '"':
						sb.Append(ch);
						bool escaped = false;
						var index = i;
						while (index > 0 && str[--index] == '\\')
							escaped = !escaped;
						if (!escaped)
							quoted = !quoted;
						break;
						/*	                case ',':
	                    sb.Append(ch);
	                    if (!quoted)
	                    {
	                        sb.AppendLine();
	                        Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
	                    }
	                    break;
					 */	                case ':':
						sb.Append(ch);
						if (!quoted)
							sb.Append(" ");
						break;
					default:
						sb.Append(ch);
						break;
				}
			}
			return sb.ToString();
		}
		
		public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
		{
			foreach (var i in ie)
			{
				action(i);
			}
		}
		// JSON formatter END
		
		public static int	get_ulong_arr_max_val( ulong[] _arr, int _max = 0 )
		{
			ushort max_val = 0;
			
			int size = ( _max == 0 ) ? _arr.Length:_max;
			
			int i;
			int j;
			
			ushort val;
			
			for( i = 0; i < size; i++ )
			{
				for( j = 0; j < 4; j++ )
				{
					val = ( ushort )( ( _arr[ i ] >> ( ( 4 - j - 1 ) << 4 ) ) & 0xffff );
					
					if( val > max_val )
					{
						max_val = val;
					}
				}
			}
			
			return (int)max_val;
		}

		public static ushort get_ushort_from_ulong( ulong _ulong, int _ind )
		{
			return ( ushort )( ( _ulong >> ( ( 4 - _ind - 1 ) << 4 ) ) & 0xffff );
		}
		
		public static ulong set_ushort_to_ulong( ulong _ulong, int _ind, ushort _val )
		{
			byte ushort_ind_shift	= ( byte )( ( 4 - _ind - 1 ) << 4 );
			ulong ushort_mask		= ( ulong )( ( ulong )0xffff << ushort_ind_shift );
			
			return ( ( ulong )_val << ( ushort_ind_shift ) ) | ( ~( _ulong & ( ushort_mask ) ) & _ulong );
		}
		
		public static void write_title( StreamWriter _sw, bool _C_exp = false )
		{
			string comment = ( _C_exp == true ) ? "//":";";
			
			_sw.WriteLine( comment + "############################################################\n" + comment );
			_sw.WriteLine( comment + " Generated by " + utils.CONST_FULL_APP_NAME + " Copyright 2017-" + DateTime.Now.Year + " 0x8BitDev\n" + comment );
			_sw.WriteLine( comment + "############################################################\n" );
		}
		
		public static string hex( string _prefix, int _val )
		{
			return _prefix + String.Format( "{0:X2}", _val );
		}
		
		public static string hex( string _prefix, uint _val )
		{
			return _prefix + String.Format( "{0:X2}", _val );
		}
		
		public static int save_prop_asm( StreamWriter _sw, string _db, string _num_pref, string _prop, bool _enable_comments )
		{
			int		all_props_cnt	= 0;
			string	data_str		= "";
			
			if( _prop != null && _prop.Length > 0 )
			{
				int prop_n;
				int props_cnt;
				
				string[] 	ent_props;
				uint 		prop_val;
				
				ent_props = _prop.Split( ' ' );
				
				props_cnt = ent_props.Length;

				if( props_cnt > 0 )
				{
					data_str = "\t" + _db + " ";
					
					for( prop_n = 0; prop_n < props_cnt; prop_n++ )
					{
						prop_val = UInt32.Parse( ent_props[ prop_n ], System.Globalization.NumberStyles.HexNumber );	
#if DEF_PLATFORM_16BIT
						data_str += utils.hex( _num_pref, ( prop_val & 0xffff ) );
						++all_props_cnt;
						
						if( prop_val > 0xffff )
						{
							data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xffff0000 ) >> 16 ) );
							++all_props_cnt;
						}
#else
						data_str += utils.hex( _num_pref, ( prop_val & 0xff ) );
						++all_props_cnt;
						
						if( prop_val > 0xff )
						{
							data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xff00 ) >> 8 ) );
							++all_props_cnt;
							
							if( prop_val > 0xffff )
							{
								data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xff0000 ) >> 16 ) );
								++all_props_cnt;
								
								if( prop_val > 0xffffff )
								{
									data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xff000000 ) >> 24 ) );
									++all_props_cnt;
								}
							}
						}
#endif
						data_str += ( prop_n < ( props_cnt - 1 ) ? ", ":"" );
					}
				}
			}
			
			_sw.WriteLine( "\t" + _db + " " + utils.hex( _num_pref, all_props_cnt ) + ( _enable_comments ? "\t; properties count":"" ) );
			
			if( data_str.Length > 0 )
			{
				_sw.WriteLine( data_str + ( _enable_comments ? "\t; properties data":"" ) );
			}
			
			return all_props_cnt;
		}
		
		public static void swap_columns_rows_order<T>( T[] _arr, int _width, int _height )
		{	
			if( _arr.Length != _width * _height )
			{
				throw new Exception( "utils.swap_columns_rows< T >( T[] _arr, int _width, int _height )\n Invalid input arguments!" );
			}
			
			T[] tmp_arr = new T[ _arr.Length ];
			
			int ind = 0;
			
			for( int y = 0; y < _height; y++ )
			{
				for( int x = 0; x < _width; x++ )
				{
					tmp_arr[ ind++ ] = _arr[ x * _height + y ];
				}
			}

			Array.Copy( tmp_arr, _arr, _arr.Length );
		}

		// RLE routine from NESst tool by Shiru
		public static int RLE8( byte[] _arr, ref byte[] _rle_arr, bool _throw_exception )
		{
			_rle_arr = new byte[ _arr.Length ];
			
			int[] stat = new int[ byte.MaxValue + 1 ];
			int i,tag,sym,sym_prev,len,ptr;
			
			int size = _arr.Length;
			
			Array.Clear( stat, 0, stat.Length );
			
			for( i = 0; i < size; ++i ) ++stat[ _arr[ i ] ];
		
			tag = -1;
		
			for( i = 0; i < stat.Length; ++i )
			{
				if( stat[ i ] == 0 )
				{
					tag = i;
					break;
				}
			}
			
			if( tag < 0 )
			{
				if( _throw_exception )
				{
					throw new Exception( "No unused data found, can't be saved as RLE (limitation of this RLE format)" );
				}
				else
				{
					return -1;
				}
			}
		
			ptr = 0;
			len = 1;
			sym_prev = -1;
		
			_rle_arr[ ptr++ ] = ( byte )tag;
			
			for( i = 0; i < size; ++i )
			{
				sym = _arr[ i ];
		
				if( sym_prev != sym || len >= byte.MaxValue || i == size - 1 )
				{
					if( ptr >= _arr.Length )
					{
						if( _throw_exception )
						{
							throw new Exception( "Negative compression warning!\nThe data can't be compressed!" );
						}
						else
						{
							return -1;
						}
					}
					
					if( len > 1 )
					{
						if( len == 2 )
						{
							_rle_arr[ ptr++ ] = ( byte )sym_prev;
						}
						else
						{
							_rle_arr[ ptr++ ] = ( byte )tag;
							_rle_arr[ ptr++ ] = ( byte )( len - 1 );
						}
					}
		
					_rle_arr[ ptr++ ] = ( byte )sym;
		
					sym_prev = sym;
		
					len = 1;
				}
				else
				{
					++len;
				}
			}
		
			_rle_arr[ ptr++ ] = ( byte )tag;	//end of file marked with zero length rle
			_rle_arr[ ptr++ ] = 0;
			
			return ptr;
		}

		public static int RLE16( ushort[] _arr, ref ushort[] _rle_arr, bool _throw_exception )
		{
			_rle_arr = new ushort[ _arr.Length ];
			
			int[] stat = new int[ ushort.MaxValue + 1 ];
			int i, tag, sym, sym_prev, len, ptr;
			
			int size = _arr.Length;
			
			Array.Clear( stat, 0, stat.Length );
			
			for( i = 0; i < size; ++i ) ++stat[ _arr[ i ] ];
		
			tag = -1;
		
			for( i = 0; i < stat.Length; ++i )
			{
				if( stat[ i ] == 0 )
				{
					tag = i;
					break;
				}
			}
			
			if( tag < 0 )
			{
				if( _throw_exception )
				{
					throw new Exception( "No unused data found, can't be saved as RLE (limitation of this RLE format)" );
				}
				else
				{
					return -1;
				}
			}
		
			ptr = 0;
			len = 1;
			sym_prev = -1;
		
			_rle_arr[ ptr++ ] = ( ushort )tag;
			
			for( i = 0; i < size; ++i )
			{
				sym = _arr[ i ];
		
				if( sym_prev != sym || len >= ushort.MaxValue || i == size - 1 )
				{
					if( ptr >= _arr.Length )
					{
						if( _throw_exception )
						{
							throw new Exception( "Negative compression warning!\nThe data can't be compressed!" );
						}
						else
						{
							return -1;
						}
					}
					
					if( len > 1 )
					{
						if( len == 2 )
						{
							_rle_arr[ ptr++ ] = ( ushort )sym_prev;
						}
						else
						{
							_rle_arr[ ptr++ ] = ( ushort )tag;
							_rle_arr[ ptr++ ] = ( ushort )( len - 1 );
						}
					}
		
					_rle_arr[ ptr++ ] = ( ushort )sym;
		
					sym_prev = sym;
		
					len = 1;
				}
				else
				{
					++len;
				}
			}
		
			_rle_arr[ ptr++ ] = ( ushort )tag;	//end of file marked with zero length rle
			_rle_arr[ ptr++ ] = 0;
			
			return ptr;
		}
		
		public static int find_nearest_color_ind( int _color )
		{
			int app_color;
			
			double 	fi;
			double 	fi_min 			= 1000000.0;
			int 	best_color_ind 	= -1;
			
			double r;
			double g;
			double b;
			
			double in_r = ( double )( ( _color >> 16 ) & 0xff );
			double in_g = ( double )( ( _color >> 8 ) & 0xff );
			double in_b = ( double )( _color & 0xff );
			
			int[] main_palette = palette_group.Instance.main_palette;
			
			for( int i = 0; i < main_palette.Length; i++ )
			{
				app_color = main_palette[ i ];
				
				r = ( double )( ( app_color >> 16 ) & 0xff );
				g = ( double )( ( app_color >> 8 ) & 0xff );
				b = ( double )( app_color & 0xff );
				
				fi = 30.0 * Math.Pow( r - in_r, 2.0 ) + 59.0 * Math.Pow( g - in_g, 2.0 ) + 11.0 * Math.Pow( b - in_b, 2.0 );
				//fi = Math.Sqrt( Math.Pow( r - in_r, 2.0 ) + Math.Pow( g - in_g, 2.0 ) + Math.Pow( b - in_b, 2.0 ) );
				
				if( fi < fi_min )
				{
					best_color_ind	= i;
					fi_min 			= fi;
				}
			}
			
			return best_color_ind;
		}
		
		public static int[] read_byte_arr( BinaryReader _br, int _size )
		{
			int[] arr = new int[ _size ];
			
			for( int i = 0; i < _size; i++ )
			{
				arr[ i ] = ( int )_br.ReadByte();
			}
			
			return arr;
		}
		
		public static int[] read_int_arr( BinaryReader _br, int _size )
		{
			int[] arr = new int[ _size ];
			
			for( int i = 0; i < _size; i++ )
			{
				arr[ i ] = _br.ReadInt32();
			}
			
			return arr;
		}
		
		public static void write_int_arr( BinaryWriter _bw, int[] _arr )
		{
			for( int i = 0; i < _arr.Length; i++ )
			{
				_bw.Write( _arr[ i ] );
			}
		}

		public static void write_int_as_ushort_arr( BinaryWriter _bw, int[] _arr )
		{
			for( int i = 0; i < _arr.Length; i++ )
			{
				_bw.Write( ( ushort )_arr[ i ] );
			}
		}
		
		public static void write_int_as_byte_arr( BinaryWriter _bw, int[] _arr )
		{
			for( int i = 0; i < _arr.Length; i++ )
			{
				_bw.Write( ( byte )_arr[ i ] );
			}
		}
		
		public static void write_ushort_arr( BinaryWriter _bw, ushort[] _arr, int _len )
		{
			for( int i = 0; i < _len; i++ )
			{
				_bw.Write( _arr[ i ] );
			}
		}

		public static int calc_progress_val( ref int _progress_val, int _max_parts, int _part_n )
		{
			_progress_val = ( int )( ( 100 / ( float )( _max_parts + 1 ) ) * ( float )( _part_n + 1 ) );
			
			return _progress_val;
		}
		
		public static int calc_progress_val_half( ref int _progress_val )
		{
			_progress_val += ( 100 - _progress_val ) >> 1;
			
			return _progress_val;
		}
		
		public static ToolStripItem get_context_menu_item_by_name( ContextMenuStrip _context_menu, string _item_name )
		{
			foreach( ToolStripItem item in _context_menu.Items ) 
			{
				if( item.Text == _item_name )
				{
					return item;
				}
			}
			
			return null;
		}
	}
}