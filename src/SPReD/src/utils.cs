﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 16.03.2017
 * Time: 16:35
 */
 
/*
defines:
~~~~~~~~
DEF_FIXED_LEN_PALETTE16_ARR		(PCE)
DEF_PALETTES_MANAGER			(PCE)
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of utils.
	/// </summary>
	public static class utils
	{
		public	const bool CONST_DEV_BUILD_FLAG	= true;
		private const bool CONST_BETA_FLAG		= true; 
		
#if	DEF_NES
		public const string	CONST_PLATFORM	= "NES";
		public const string	CONST_PRJ_EXT	= ".sprednes";
#elif DEF_SMS
		public const string	CONST_PLATFORM	= "SMS";
		public const string	CONST_PRJ_EXT	= ".spredsms";
#elif DEF_PCE
		public const string	CONST_PLATFORM	= "PCE";
		public const string	CONST_PRJ_EXT	= ".spredpce";
#else
		public const string	CONST_PLATFORM	= "UNKNOWN";
#endif

#if	DEBUG
		public const string	CONST_BUILD_CFG	= " [DEBUG]";
#else
		public const string	CONST_BUILD_CFG	= "";
#endif

		public static readonly bool is_win 	 = false;
		public static readonly bool is_linux = false;
		public static readonly bool is_macos = false;

		// OS detection code implemented by jarik ( 100% managed code ) https://stackoverflow.com/a/38795621
		static utils()
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

		private readonly static Version ver	= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		
		public readonly static string	build_str	= "Build: " + ver.Build;
		public readonly static DateTime	build_date 	= new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds( ver.Revision * 2 );

		public readonly static string CONST_APP_VER			= "v" + ver.Major + "." + ver.Minor + ( CONST_BETA_FLAG ? "b ":" " ) + ( CONST_DEV_BUILD_FLAG ? "Dev":"" ) + CONST_BUILD_CFG;
		public readonly static string CONST_APP_NAME	= "SPReD-" + CONST_PLATFORM + ( Environment.Is64BitProcess ? " (x64)":" (x86)" );
		
		public const uint CONST_PROJECT_FILE_MAGIC			= 'S'<<24 | 'N'<<16 | 'e'<<8 | 'S';
		public const byte CONST_PROJECT_FILE_VER			= 1;
		public const int  CONST_PROJECT_FILE_PALETTE_FLAG	= 8;
		
		public const int CONST_CHR_BANK_SIDE				= 256;
		
		// sprite export flags
		public const string CONST_SPR_EXPORT_SKIP_ALL		= "#se";	// skip graphics and palette(s) export
		public const string CONST_SPR_EXPORT_PALETTE_ONLY	= "#ep";	// skip graphics and export palette(s)
		
#if DEF_NES || DEF_SMS
		public const int CONST_CHR_SIDE_PIXELS_CNT			= 8;
		public const int CONST_CHR_SIDE_PIXELS_CNT_POW_BITS	= 3;
		
		public const int CONST_CHR_IMG_SIZE					= CONST_CHR_SIDE_PIXELS_CNT << 1;
		public const int CONST_CHR_BANK_SIDE_SPRITES_CNT	= CONST_CHR_BANK_SIDE / CONST_CHR_IMG_SIZE;

		public const int CONST_PALETTE_MAIN_NUM_COLORS		= 64;
		
		public const int CONST_SPRITE_MAX_NUM_ATTRS			= 64;	// the max number of attributes in a sprite

		public const int CONST_CHR8x16_SIDE_PIXELS_CNT		= CONST_CHR_SIDE_PIXELS_CNT;
#elif DEF_PCE
		public const int CONST_CHR_SIDE_PIXELS_CNT			= 16;
		public const int CONST_CHR_SIDE_PIXELS_CNT_POW_BITS	= 4;
		
		public const int CONST_CHR_IMG_SIZE					= CONST_CHR_SIDE_PIXELS_CNT;
		public const int CONST_CHR_BANK_SIDE_SPRITES_CNT	= CONST_CHR_BANK_SIDE / CONST_CHR_IMG_SIZE;
		
		public const int CONST_PALETTE_MAIN_NUM_COLORS		= 512;

		public const int CONST_SPRITE_MAX_NUM_ATTRS			= 64;	// the max number of attributes in a sprite
		
		public const int CONST_PALETTE16_ARR_LEN			= 16;
		
		public const int CONST_PCE_MAX_SPRITES_CNT			= 1024;
#endif
		public const int CONST_CHR_TOTAL_PIXELS_CNT			= CONST_CHR_SIDE_PIXELS_CNT * CONST_CHR_SIDE_PIXELS_CNT;

		public const int CONST_CHR_BANK_MAX_SPRITES_CNT		= CONST_CHR_BANK_SIDE_SPRITES_CNT * CONST_CHR_BANK_SIDE_SPRITES_CNT;

#if DEF_NES		
		public const int CONST_CHR_NATIVE_SIZE_IN_BYTES	= 16;
#elif DEF_SMS
		public const int CONST_CHR_NATIVE_SIZE_IN_BYTES	= 32;
#elif DEF_PCE
		public const int CONST_CHR_NATIVE_SIZE_IN_BYTES	= 128;
#endif

		public const int CONST_NUM_SMALL_PALETTES 			= 4;
		public const int CONST_PALETTE_SMALL_NUM_COLORS		= 4;
		
		public const int CONST_LAYOUT_WORKSPACE_HALF_SIDE	= 256;
		
		public static Pen			pen				= new Pen( Color.Black );
		public static SolidBrush 	brush		 	= new SolidBrush( Color.White );
		public static Font 			fnt10_Arial		= new Font( "Arial", 10, FontStyle.Bold );

		private static void flip_bmp( Bitmap _bmp, int _flags )
		{
			if( ( _flags & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) == CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP )
			{
				_bmp.RotateFlip( RotateFlipType.RotateNoneFlipX );
			}
			
			if( ( _flags & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) == CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP )
			{
				_bmp.RotateFlip( RotateFlipType.RotateNoneFlipY );
			}
		}

		public static Bitmap create_CHR_bitmap( CHR_data _chr_data, CHR_data_attr _attr, bool _alpha, int _plt_ind, palette_small[] _plt_arr = null )
		{
			Bitmap bmp = new Bitmap( CONST_CHR_SIDE_PIXELS_CNT, CONST_CHR_SIDE_PIXELS_CNT, PixelFormat.Format32bppPArgb );
			
			update_CHR_bitmap( _chr_data, bmp, _attr, _alpha, _plt_ind, _plt_arr );
			
			return bmp;
		}
		
		public static void update_CHR_bitmap( CHR_data _chr_data, Bitmap _bmp, CHR_data_attr _attr, bool  _alpha, int _plt_ind, palette_small[] _plt_arr )
		{
			BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, _bmp.Width, _bmp.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb );
			
			if( bmp_data != null )
			{
				fill_CHR_bmp_data( bmp_data.Scan0, 0, _chr_data, _attr, _alpha, _plt_ind, _plt_arr );
				
				_bmp.UnlockBits( bmp_data );
				
				flip_bmp( _bmp, ( _attr != null ? _attr.flip_flag:0 ) );
			}
		}

		public static Bitmap create_bitmap8x16( CHR_data _chr_data1, CHR_data _chr_data2, CHR_data_attr _attr, bool _alpha, int _plt_ind, palette_small[] _plt_arr = null )
		{
			Bitmap bmp = new Bitmap( 8, 16, PixelFormat.Format32bppPArgb );
			
			update_bitmap8x16( _chr_data1, _chr_data2, bmp, _attr, _alpha, _plt_ind, _plt_arr );
			
			return bmp;
		}
		
		public static void update_bitmap8x16( CHR_data _chr_data1, CHR_data _chr_data2, Bitmap _bmp, CHR_data_attr _attr, bool  _alpha, int _plt_ind, palette_small[] _plt_arr )
		{
			BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, _bmp.Width, _bmp.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb );
			
			if( bmp_data != null )
			{
				fill_CHR_bmp_data( bmp_data.Scan0, 0, _chr_data1, _attr, _alpha, _plt_ind, _plt_arr );
				
				if( _chr_data2 != null )
				{
					fill_CHR_bmp_data( bmp_data.Scan0, ( 8 * bmp_data.Stride ), _chr_data2, _attr, _alpha, _plt_ind, _plt_arr );
					
					_bmp.UnlockBits( bmp_data );
				}
				else
				{
					_bmp.UnlockBits( bmp_data );
					
					Graphics gfx = Graphics.FromImage( _bmp );
					
					utils.brush.Color = Color.Red;
					
					gfx.FillRectangle( utils.brush, 0, 8, 8, 8 );
					gfx.Flush();
				}
				
				flip_bmp( _bmp, ( _attr != null ? _attr.flip_flag:0 ) );
			}
		}
		
		private static void fill_CHR_bmp_data( IntPtr _data_ptr, int _data_offset, CHR_data _chr_data, CHR_data_attr _attr, bool  _alpha, int _plt_ind, palette_small[] _plt_arr )
		{
			unsafe
			{
				int* img_buff = ( int* )( _data_ptr + _data_offset );
				
#if DEF_NES
				bool apply_palette 	= ( _plt_arr != null && _plt_ind >= 0 );
				int[] clr_inds 		= apply_palette ? _plt_arr[ _plt_ind ].get_color_inds():null;
				int alpha;
#endif
				int clr = 0;
				int pix_ind;
				
				for( int p = 0; p < CONST_CHR_TOTAL_PIXELS_CNT; p++ )
				{
					pix_ind = _chr_data.get_data()[ p ];
					
#if DEF_NES
					if( apply_palette )
#endif						
					{
#if DEF_NES
						clr = palette_group.Instance.main_palette[ clr_inds[ pix_ind ] ];
#elif DEF_SMS
						clr = palette_group.Instance.main_palette[ palette_group.Instance.get_palettes_arr()[ pix_ind / CONST_NUM_SMALL_PALETTES ].get_color_inds()[ pix_ind % CONST_NUM_SMALL_PALETTES ] ];
#endif

#if DEF_FIXED_LEN_PALETTE16_ARR
						if( _attr != null )
						{
							clr = palette_group.Instance.main_palette[ palettes_array.Instance.get_color( ( _attr.palette_ind < 0 ? 0:_attr.palette_ind ), pix_ind ) ];
						}
						else
						{
							clr = palette_group.Instance.main_palette[ palette_group.Instance.get_palettes_arr()[ pix_ind / CONST_NUM_SMALL_PALETTES ].get_color_inds()[ pix_ind % CONST_NUM_SMALL_PALETTES ] ];
						}
#endif
						if( ( pix_ind != 0 && _alpha == true ) || _alpha == false )
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
		}
		
		public static int get_padding( int _size )
		{
			int padding = 0;
			
			if( _size <= 1024 )
			{
				padding = 1024 - _size; 
			}
			else
			if( _size <= 2048 )
			{
				padding = 2048 - _size;
			}
			else
			if( _size <= 4096 )
			{
				padding = 4096 - _size;
			}

			return padding;
		}
		
		public static int find_nearest_color_ind( int _color )
		{
			int platform_color;
			
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
				platform_color = main_palette[ i ];
				
				r = ( double )( ( platform_color >> 16 ) & 0xff );
				g = ( double )( ( platform_color >> 8 ) & 0xff );
				b = ( double )( platform_color & 0xff );
				
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
		
		public static string get_file_title( string _comment_smb )
		{
			string res;
			
			res = _comment_smb + "#######################################################\n" + _comment_smb + "\n";
			res += _comment_smb + " Generated by " + utils.CONST_APP_NAME + " Copyright 2017-" + DateTime.Now.Year + " 0x8BitDev\n" + _comment_smb + "\n";
			res += _comment_smb + "#######################################################\n";
			
			return res;
		}
		
		public static string hex( string _prefix, int _val )
		{
			return _prefix + String.Format( "{0:X2}", _val );
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
