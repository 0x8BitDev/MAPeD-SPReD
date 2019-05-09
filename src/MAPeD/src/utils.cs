/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 16.03.2017
 * Time: 16:35
 */
 
/* 
#define	DEF_SCREEN_HEIGHT_7d5_TILES
#define DEF_TILE_DRAW_FAST

NOT DEFINED:
DEF_FLIP_BLOCKS_SPR_BY_FLAGS
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

namespace MAPeD
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
		public const string CONST_APP_NAME	= "MAPeD(" + CONST_PLATFORM + ")";		
		
		public const uint CONST_PROJECT_FILE_MAGIC	= 'S'<<24 | 'N'<<16 | 'e'<<8 | 'M';
		public const byte CONST_PROJECT_FILE_VER	= 1; 
		
		public const uint CONST_SPREDNES_FILE_MAGIC			= 'S'<<24 | 'N'<<16 | 'e'<<8 | 'S';
		public const uint CONST_SPREDNES_PROJECT_FILE_VER	= 1;
		
		public const byte 	CONST_IO_DATA_TILES_AND_SCREENS	= 0x01;
		public const byte 	CONST_IO_DATA_LAYOUT			= 0x02;
		public const byte 	CONST_IO_DATA_ENTITIES			= 0x04;
		public const byte	CONST_IO_DATA_END				= 0xff;
		
		public const int CONST_SCREEN_NUM_SIDE_TILES	= 8;
		public const int CONST_SCREEN_NUM_SIDE_BLOCKS	= 16;
		public const int CONST_SCREEN_OFFSET			= 32;
		public const int CONST_SCREEN_TILES_SIZE		= 64;
		public const int CONST_SCREEN_BLOCKS_SIZE		= 32;
		public const int CONST_SCREEN_TILES_CNT			= CONST_SCREEN_NUM_SIDE_TILES * CONST_SCREEN_NUM_SIDE_TILES;
		public const int CONST_SCREEN_MAX_CNT			= 256;
		public const int CONST_SCREEN_WIDTH_PIXELS		= 32 * utils.CONST_SCREEN_NUM_SIDE_TILES;
		
		public const int CONST_SCREEN_MARK_IMAGE_SIZE	= CONST_SCREEN_WIDTH_PIXELS >> 1;
		
#if DEF_SCREEN_HEIGHT_7d5_TILES		
		public const int CONST_SCREEN_HEIGHT_PIXELS		= CONST_SCREEN_WIDTH_PIXELS - 16;
#else		
		public const int CONST_SCREEN_HEIGHT_PIXELS		= CONST_SCREEN_WIDTH_PIXELS;
#endif		
		public const int CONST_LAYOUT_MAX_CNT			= 128;
		
		public const int CONST_SPR8x8_SIDE_PIXELS_CNT			= 8;
		public const int CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS	= 3;	// 2->4->8 = 3bits
		public const int CONST_SPR8x8_TOTAL_PIXELS_CNT			= CONST_SPR8x8_SIDE_PIXELS_CNT * CONST_SPR8x8_SIDE_PIXELS_CNT;
		
		public const int CONST_BLOCKS_IMG_SIZE		= 32;
		public const int CONST_TILES_IMG_SIZE		= 64;
		
		public const int CONST_NUM_SMALL_PALETTES 			= 4;
		public const int CONST_PALETTE_SMALL_NUM_COLORS		= 4;
		public const int CONST_PALETTE_MAIN_NUM_COLORS		= 64;
		
		public const int CONST_BLOCKS_USHORT_SIZE			= 256 * CONST_BLOCK_SIZE;
		public const int CONST_TILES_UINT_SIZE				= 256;
		public const int CONST_CHR_BANK_SIZE				= CONST_CHR_BANK_SIDE * CONST_CHR_BANK_SIDE;
		public const int CONST_CHR_BANK_SIDE				= 128;
		public const int CONST_CHR_BANK_MAX_CNT				= 255;
		public const int CONST_CHR_BANK_MAX_SPR8X8_CNT		= CONST_CHR_BANK_SIZE / CONST_SPR8x8_TOTAL_PIXELS_CNT;
		
		public const int CONST_BLOCK_SIZE					= 4;	// ushorts
		public const int CONST_TILE_SIZE					= 4;	// bytes
		
		public const int CONST_CHR_BANK_MAX_SPRITES_CNT		= 256;
		
		public const int CONST_MAX_BLOCKS_CNT				= 256;
		public const int CONST_MAX_TILES_CNT				= 256;
		
		// the Dendy hardware doesn't support flipping of a background on per CHR's basis
		// so I left it just in case for using in other versions of the application for other consoles
		public const byte CONST_CHR_ATTR_FLAG_HFLIP			= 0x01;
		public const byte CONST_CHR_ATTR_FLAG_VFLIP			= 0x02;
		// UNDEFINED: DEF_FLIP_BLOCKS_SPR_BY_FLAGS		
		
		public static SolidBrush 	brush		 	= new SolidBrush( CONST_COLOR_BRUSH_DEFAULT );
		public static readonly Font fnt8_Arial		= new Font( "Arial", 8, 	FontStyle.Bold );
		public static readonly Font fnt10_Arial		= new Font( "Arial", 10, 	FontStyle.Bold );
		public static readonly Font fnt12_Arial		= new Font( "Arial", 12, 	FontStyle.Bold );
		public static readonly Font fnt42_Arial		= new Font( "Arial", 42, 	FontStyle.Bold );
		public static readonly Font fnt64_Arial		= new Font( "Arial", 64, 	FontStyle.Bold );
		
		public static byte[] tmp_spr8x8_buff = new byte[ CONST_SPR8x8_TOTAL_PIXELS_CNT ];

		//DEFAULT COLORS
		// entity editor
		public static readonly Color	CONST_COLOR_DEFAULT_ENTITY					= Color.Gold;
		public static readonly Color	CONST_COLOR_DEFAULT_ENTITY_INACTIVE			= Color.Black;
		public static readonly Color	CONST_COLOR_NULL							= Color.Empty;
		public static readonly Color	CONST_COLOR_ENTITY_PIVOT_CROSS				= Color.OrangeRed;
		public static readonly Color	CONST_COLOR_ENTITY_PIVOT_RECT 				= Color.LimeGreen;
		public static readonly Color	CONST_COLOR_ENTITY_PIXBOX_INACTIVE			= Color.Black;
		// layout
		public static readonly Color	CONST_COLOR_SIMPLE_SCREEN_CROSS				= Color.Red;
		public static readonly Color	CONST_COLOR_SCREEN_LIST_NOT_EMPTY			= Color.White;
		public static readonly Color	CONST_COLOR_SCREEN_LIST_EMPTY 				= Color.Red;
		public static readonly Color	CONST_COLOR_SCREEN_GHOST_IMAGE_INNER_BORDER	= Color.White;
		public static readonly Color	CONST_COLOR_SCREEN_GHOST_IMAGE_OUTER_BORDER	= Color.Red;
		public static readonly Color	CONST_COLOR_SCREEN_SELECTED_LAYOUT_MODE		= Color.LimeGreen;
		public static readonly Color	CONST_COLOR_STRING_DEFAULT					= Color.White;
		public static readonly Color	CONST_COLOR_ENTITY_BORDER_EDIT_ENT_MODE		= Color.LimeGreen;
		public static readonly Color	CONST_COLOR_SELECTED_ENTITY_BORDER			= Color.Red;
		public static readonly Color	CONST_COLOR_ENTITY_BORDER_EDIT_INST_MODE	= Color.Orange;
		public static readonly Color	CONST_COLOR_TARGET_LINK						= Color.LightGreen;
		public static readonly Color	CONST_COLOR_ENTITY_PIVOT					= Color.LightGreen;
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
		// block editor
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_GRID						= Color.FromArgb( 0x78808080 );
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_CHR_BORDER					= Color.White;
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_SELECTED_CHR_OUTER_BORDER	= Color.White;
		public static readonly Color	CONST_COLOR_BLOCK_EDITOR_SELECTED_CHR_INNER_BORDER	= Color.Red;
		// CHR bank viewer
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_DEFAULT					= Color.Black;
		public static readonly Color	CONST_COLOR_CHR_BANK_GRID								= Color.FromArgb( 0x78808080 );
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_OUTER_BORDER				= Color.Black;
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_BLOCK_CHR_BORDER			= Color.Yellow;
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_SELECT_MODE	= Color.Orange;
		public static readonly Color	CONST_COLOR_CHR_BANK_SELECTED_INNER_BORDER_DRAW_MODE	= Color.White;
		// palette group
		public static readonly Color	CONST_COLOR_PALETTE_SELECTED_OUTER_BORDER	= Color.Black; 
		public static readonly Color	CONST_COLOR_PALETTE_SELECTED_INNER_BORDER	= Color.White;
		// tile editor
		public static readonly Color	CONST_COLOR_TILE_EDITOR_TRANSLUCENT_QUAD		= Color.FromArgb( unchecked( (int)0x80000000 ) );
		public static readonly Color	CONST_COLOR_TILE_EDITOR_GRID					= Color.FromArgb( 0x78808080 );
		public static readonly Color	CONST_COLOR_TILE_EDITOR_BLOCK_BORDER			= Color.White;
		public static readonly Color	CONST_COLOR_TILE_EDITOR_SELECTED_INNER_BORDER	= Color.White;
		public static readonly Color	CONST_COLOR_TILE_EDITOR_SELECTED_OUTER_BORDER	= Color.Red;
		public static readonly Color	CONST_COLOR_TILE_EDITOR_LOCKED_LINE 			= Color.Red;
		// other
		public static readonly Color	CONST_COLOR_BRUSH_DEFAULT						= Color.White;
		public static readonly Color	CONST_COLOR_PIXBOX_INACTIVE_CROSS				= Color.Red;		
		public static readonly Color	CONST_COLOR_PIXBOX_DEFAULT						= Color.White;
		
		public enum ETransformType
		{
			tt_vflip,
			tt_hflip,
			tt_rotate,
		};
		
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
		
		public static Bitmap create_bitmap( byte[] _arr, int _width, int _height, byte _flags, bool _alpha, int _plt_ind, palette_small[] _plt_arr = null )
		{
			int img_size = _width * _height;
			
			int[] img_buff = new int[ img_size ];
			
			GCHandle handle = GCHandle.Alloc( img_buff, GCHandleType.Pinned );
			
			bool apply_palette 	= ( _plt_arr != null && _plt_ind >= 0 );
			byte[] clr_inds 	= apply_palette ? _plt_arr[ _plt_ind ].get_color_inds():null;
			
			apply_palette = (clr_inds != null) && apply_palette;
			
			int clr;
			int pix_ind;
			int alpha;

			if( apply_palette )
			{
				for( int p = 0; p < img_size; p++ )
				{
					pix_ind = _arr[ p ];
					
					clr = palette_group.Instance.main_palette[ ( int )clr_inds[ pix_ind ] ];
					
					if( ( clr != 0 && _alpha == true ) || _alpha == false )
					{
						clr |= 0xFF << 24;
					}
					
					img_buff[ p ] = clr;
				}
			}
			else
			{
				for( int p = 0; p < img_size; p++ )
				{
					pix_ind = _arr[ p ];
					
					alpha = ( pix_ind == 0 ) ? ( _alpha ? 0x00:0xFF ):0xFF;
					
					pix_ind <<= 6;
					clr = alpha << 24 | pix_ind << 16 | pix_ind << 8 | pix_ind;
					
					img_buff[ p ] = clr;
				}
			}
			
			Bitmap bmp = new Bitmap( _width, _height, _width<<2, PixelFormat.Format32bppArgb, handle.AddrOfPinnedObject() );

			handle.Free();
			
			flip_bmp( bmp, _flags );
			
			return bmp;
		}
		
		public static void update_block_gfx( int _block_id, tiles_data _data, Graphics _gfx, int _img_half_width, int _img_half_height, int _x_offs = 0, int _y_offs = 0 )
		{
			Bitmap 	bmp;
			
			ushort 	chr_data;
			byte 	flip_flag;
			int		plt_ind;
			
			for( int j = 0; j < utils.CONST_BLOCK_SIZE; j++ )
			{
				chr_data = _data.blocks[ ( _block_id << 2 ) + j ];
				
				flip_flag 	= tiles_data.get_block_flags_flip( chr_data );
				plt_ind		= tiles_data.get_block_flags_palette( chr_data );
				
				_data.from_CHR_bank_to_spr8x8( tiles_data.get_block_CHR_id( chr_data ), utils.tmp_spr8x8_buff );
				
				bmp = utils.create_bitmap( utils.tmp_spr8x8_buff, 8, 8, flip_flag, false, plt_ind, palette_group.Instance.get_palettes_arr() );
				
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
		
		public static int	get_byte_arr_max_ind( byte[] _arr, int _max = 0 )
		{
			byte max_ind = 0;
			
			int size = ( _max == 0 ) ? _arr.Length:_max;
			
			for( int i = 0; i < size; i++ )
			{
				if( _arr[ i ] > max_ind )
				{
					max_ind = _arr[ i ];
				}
			}
			
			return (int)max_ind;
		}
		
		public static int	get_uint_arr_max_ind( uint[] _arr, int _max = 0 )
		{
			uint max_ind = 0;
			
			int size = ( _max == 0 ) ? _arr.Length:_max;
			
			int i;
			int j;
			
			uint val;
			
			for( i = 0; i < size; i++ )
			{
				for( j = 0; j < 4; j++ )
				{
					val = ( _arr[ i ] >> ( ( 4 - j - 1 ) << 3 ) ) & 0xff;
					
					if( val > max_ind )
					{
						max_ind = val;
					}
				}
			}
			
			return (int)max_ind;
		}
		
		public static byte get_byte_from_uint( uint _uint, int _ind )
		{
			return ( byte )( ( _uint >> ( ( 4 - _ind - 1 ) << 3 ) ) & 0xff );			
		}
		
		public static uint set_byte_to_uint( uint _uint, int _ind, byte _val )
		{
			int byte_ind_shift = ( int )( ( 4 - _ind - 1 ) << 3 );
			int byte_mask		= ( int )( 0xff << byte_ind_shift );
			
			return ( ( uint )_val << ( byte_ind_shift ) ) | ( uint )( ~( _uint & ( byte_mask ) ) & _uint );
		}
		
		public static void fill_buttons( FlowLayoutPanel _panel, ImageList _il, EventHandler _e, ContextMenuStrip _cm, int _padding = 0 )
		{
			_panel.Controls.Clear();
			_panel.SuspendLayout();
			
			ToolTip tp;
			Button	btn;
			
		    for( int i = 0; i < _il.Images.Count; i++ )
		    {
		        btn = new Button();
		        
		        btn.FlatStyle 	= FlatStyle.Flat;
		        btn.Size 		= _il.ImageSize;
		        btn.ImageList 	= _il;
		        btn.ImageIndex 	= i;
		        btn.Click 		+= _e;
		        btn.Margin 		= new Padding( _padding );
		        btn.Padding 	= new Padding( 0 );
		        btn.TabStop 	= false;
		        
		        btn.ContextMenuStrip = _cm;
		        
		        tp		= new ToolTip();
		        tp.SetToolTip( btn, String.Format( "#{0:X2}", i ) );
		        
		        _panel.Controls.Add( btn );
		    }
			
			_panel.ResumeLayout();
		}
		
		public static void write_title( StreamWriter _sw )
		{
			_sw.WriteLine( ";#######################################################\n;" );
			_sw.WriteLine( "; Generated by " + utils.CONST_APP_NAME + " Copyright 2017-" + DateTime.Now.Year + " 0x8BitDev\n;" );
			_sw.WriteLine( ";#######################################################\n" );
		}
		
		public static string hex( string _prefix, int _val )
		{
			return _prefix + String.Format( "{0:X2}", _val );
		}
		
		public static string hex( string _prefix, uint _val )
		{
			return _prefix + String.Format( "{0:X2}", _val );
		}
		
		public static void save_prop_asm( StreamWriter _sw, string _db, string _num_pref, string _prop, bool _enable_comments )
		{
			if( _prop != null && _prop.Length > 0 )
			{
				int prop_n;
				int props_cnt;
				
				string		data_str;
				string[] 	ent_props;
				uint 		prop_val;
				
				char[] props_separator = new char[]{ ' ' };
				
				ent_props = _prop.Split( props_separator );
				
				props_cnt = ent_props.Length;
				
				if( props_cnt > 0 )
				{
					data_str = "\t" + _db + " ";
					
					for( prop_n = 0; prop_n < props_cnt; prop_n++ )
					{
						prop_val = UInt32.Parse( ent_props[ prop_n ] );
	
						data_str += utils.hex( _num_pref, ( prop_val & 0xff ) );
						
						if( prop_val > 0xff )
						{
							data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xff00 ) >> 8 ) );
							
							if( prop_val > 0xffff )
							{
								data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xff0000 ) >> 16 ) );
								
								if( prop_val > 0xffffff )
								{
									data_str += ", " + utils.hex( _num_pref, ( ( prop_val & 0xff000000 ) >> 24 ) );
								}
							}
						}
						
						data_str += ( prop_n < ( props_cnt - 1 ) ? ", ":"" );
					}
					
					_sw.WriteLine( data_str + ( _enable_comments ? "\t; properties":"" ) );
				}
			}
		}
		
		public static void swap_columns_rows_order( byte[] _arr, int _width, int _height )
		{	
			if( _arr.Length != _width * _height )
			{
				throw new Exception( "utils.swap_columns_rows( byte[] _arr, int _width, int _height )\n Invalid input arguments!" );
			}
			
			byte[] tmp_arr = new byte[ _arr.Length ];
			
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
	}
}

