/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 16.03.2017
 * Time: 16:35
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SPReD
{
	/// <summary>
	/// Description of utils.
	/// </summary>
	public static class utils
	{
		private const bool CONST_DEV_BUILD_FLAG	= true;
		private const bool CONST_BETA_FLAG		= true; 
		
#if	DEF_NES
		public const string	CONST_PLATFORM	= "NES";
#else
		public const string	CONST_PLATFORM	= "UNKNOWN";
#endif

#if	DEBUG
		public const string	CONST_BUILD_CFG	= "[DEBUG]";
#else
		public const string	CONST_BUILD_CFG	= "";
#endif

		private static Version ver			= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		public static string build_str		= "Build: " + ver.Build;
		public static DateTime build_date 	= new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds( ver.Revision * 2 );

		public static string CONST_APP_VER	= "v" + ver.Major + "." + ver.Minor + ( CONST_BETA_FLAG ? "b ":" " ) + ( CONST_DEV_BUILD_FLAG ? "Dev ":"" ) + CONST_BUILD_CFG;
		public const string CONST_APP_NAME	= "SPReD(" + CONST_PLATFORM + ")";		
		
		public const uint CONST_PROJECT_FILE_MAGIC	= 'S'<<24 | 'N'<<16 | 'e'<<8 | 'S';
		public const byte CONST_PROJECT_FILE_VER	= 1;
		
		public const int CONST_CHR8x8_SIDE_PIXELS_CNT			= 8;
		public const int CONST_CHR8x16_SIDE_PIXELS_CNT			= CONST_CHR8x8_SIDE_PIXELS_CNT << 1;
		public const int CONST_CHR8x8_SIDE_PIXELS_CNT_POW_BITS	= 3;
		public const int CONST_CHR8x8_TOTAL_PIXELS_CNT	= CONST_CHR8x8_SIDE_PIXELS_CNT * CONST_CHR8x8_SIDE_PIXELS_CNT;
		
		public const int CONST_NUM_SMALL_PALETTES 			= 4;
		public const int CONST_PALETTE_SMALL_NUM_COLORS		= 4;
		public const int CONST_PALETTE_MAIN_NUM_COLORS		= 64;
		
		public const int CONST_CHR_BANK_MAX_SPRITES_CNT		= 256;
		
		public const int CONST_LAYOUT_WORKSPACE_HALF_SIDE	= 256;
		
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

		public static Bitmap create_bitmap8x8( CHR8x8_data _chr_data, int _flags, bool _alpha, int _plt_ind, palette_small[] _plt_arr = null )
		{
			Bitmap bmp = new Bitmap( 8, 8, PixelFormat.Format32bppArgb );
			
			update_bitmap8x8( _chr_data, bmp, _flags, _alpha, _plt_ind, _plt_arr );
			
			return bmp;
		}
		
		public static void update_bitmap8x8( CHR8x8_data _chr_data, Bitmap _bmp, int _flags, bool  _alpha, int _plt_ind, palette_small[] _plt_arr )
		{
			BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, _bmp.Width, _bmp.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
			
			if( bmp_data != null )
			{
				fill_bmp_data8x8( bmp_data.Scan0, 0, _chr_data, _alpha, _plt_ind, _plt_arr );
				
				_bmp.UnlockBits( bmp_data );
				
				flip_bmp( _bmp, _flags );
			}
		}

		public static Bitmap create_bitmap8x16( CHR8x8_data _chr_data1, CHR8x8_data _chr_data2, int _flags, bool _alpha, int _plt_ind, palette_small[] _plt_arr = null )
		{
			Bitmap bmp = new Bitmap( 8, 16, PixelFormat.Format32bppArgb );
			
			update_bitmap8x16( _chr_data1, _chr_data2, bmp, _flags, _alpha, _plt_ind, _plt_arr );
			
			return bmp;
		}
		
		public static void update_bitmap8x16( CHR8x8_data _chr_data1, CHR8x8_data _chr_data2, Bitmap _bmp, int _flags, bool  _alpha, int _plt_ind, palette_small[] _plt_arr )
		{
			BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, _bmp.Width, _bmp.Height ), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb );
			
			if( bmp_data != null )
			{
				fill_bmp_data8x8( bmp_data.Scan0, 0, _chr_data1, _alpha, _plt_ind, _plt_arr );
				
				if( _chr_data2 != null )
				{
					fill_bmp_data8x8( bmp_data.Scan0, ( 8 * bmp_data.Stride ), _chr_data2, _alpha, _plt_ind, _plt_arr );
					
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
				
				flip_bmp( _bmp, _flags );
			}
		}
		
		private static void fill_bmp_data8x8( IntPtr _data_ptr, int _data_offset, CHR8x8_data _chr_data, bool  _alpha, int _plt_ind, palette_small[] _plt_arr )
		{
			unsafe
			{
				int* img_buff = ( int* )( _data_ptr + _data_offset );
				
				bool apply_palette 	= ( _plt_arr != null && _plt_ind >= 0 );
				int[] clr_inds 		= apply_palette ? _plt_arr[ _plt_ind ].get_color_inds():null;					

				int clr;
				int pix_ind;
				int alpha;
				
				for( int p = 0; p < CONST_CHR8x8_TOTAL_PIXELS_CNT; p++ )
				{
					pix_ind = _chr_data.get_data()[ p ];
					
					if( apply_palette )
					{
						clr = palette_group.Instance.main_palette[ clr_inds[ pix_ind ] ];
						
						if( ( pix_ind != 0 && _alpha == true ) || _alpha == false )
						{
							 clr |= 0xFF << 24;
						}
					}
					else
					{
						alpha = ( pix_ind == 0 ) ? ( _alpha ? 0x00:0xFF ):0xFF;
						
						pix_ind <<= 6;							
						clr = alpha << 24 | pix_ind << 16 | pix_ind << 8 | pix_ind;
					}
					
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
	}
}
