﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2017
 * Time: 11:07
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;// marshal...

using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace SPReD
{
	/// <summary>
	/// Description of CHR_data_group.
	/// </summary>
	///
	public struct sprite_params
	{
		public int m_offset_x;
		public int m_offset_y;
		public int m_size_x;
		public int m_size_y;
		
		public List< CHR_data_attr >	m_CHR_attr;
		
		public CHR_data_group m_CHR_data;
		
		public sprite_params( CHR_data_group _CHR_data )
		{
			m_CHR_attr = new List< CHR_data_attr >( 100 );
			
			m_CHR_data = _CHR_data;
			
			m_offset_x			=  0;
			m_offset_y			=  0;
			m_size_x			= -1;
			m_size_y			= -1;
		}
	}
	
	public class CHR_data_group
	{
		public enum e_CHR_packing_type
		{
			NoPacking = 0,
			_1KB,
			_2KB,
			_4KB,
#if DEF_SMS
			_8KB,
#elif DEF_PCE
			_8KB,
			_16KB,
			_32KB,
#endif
		};
		
		private readonly List< CHR_data > m_CHR_arr;
	
		private static int m_num_ids	= -1;
		
		private int m_id = -1;
		
		private int m_link_cnt 	= 0;
		
		public string name
		{
			get { return "chr" + id; }
			set {}
		}
		
		public int id
		{
			get { return m_id; }
			set { m_id = value; }
		}
		
		public CHR_data_group()
		{
			m_CHR_arr = new List< CHR_data >( 100 );
			
			id = ++m_num_ids;
		}
		
		public void reset()
		{
			m_CHR_arr.ForEach( delegate( CHR_data _obj ) { _obj.reset(); } );
			m_CHR_arr.Clear();
		}
		
		public static void reset_ids_cnt()
		{
			m_num_ids = -1;
		}
		
		public int get_size_bytes()
		{
			return m_CHR_arr.Count * utils.CONST_CHR_NATIVE_SIZE_IN_BYTES;
		}
		
		public string get_filename()
		{
			return name + ".bin";
		}
		
		public List< CHR_data > get_data()
		{
			return m_CHR_arr;
		}

		public int get_link_cnt()
		{
			return m_link_cnt;
		}
		
		public void link()
		{
			++m_link_cnt;
		}
		
		public int unlink()
		{
			return --m_link_cnt;
		}
		
#if DEF_SMS
		public bool swap_CHRs( int _ind1, int _ind2 )
		{
			if( _ind1 >= 0 && _ind1 < get_data().Count && _ind2 >= 0 && _ind2 < get_data().Count )
			{
				CHR_data data = get_data()[ _ind1 ];
				get_data()[ _ind1 ] = get_data()[ _ind2 ];
				get_data()[ _ind2 ] = data;
				
				return true;
			}
			
			return false;
		}
#endif

		public sprite_params setup( Bitmap _bmp, bool _apply_palette, int _palette_slot )
		{
			sprite_params spr_params 	= new sprite_params( this );
			spr_params.m_CHR_data		= this;
			
			int img_width 	= _bmp.Width;
			int img_height 	= _bmp.Height;

			List< byte[] > lines_arr = new List< byte[] >( img_height );
			
			int i;
			int j;
			
			int min_x =  0;
			int max_x = img_width - 1;

			int min_y = 0;
			int max_y = img_height - 1;
			
			byte index_byte;
			byte[] pixels_line  = null;

			Color[] plte = _bmp.Palette.Entries;
			
#if DEF_NES
			int index_clamp_val = 3;
			
			int clrs_cnt = utils.CONST_PALETTE_SMALL_NUM_COLORS;
#elif DEF_SMS || DEF_PCE
			int index_clamp_val = 15;
			
			int clrs_cnt = Math.Min( utils.CONST_PALETTE_SMALL_NUM_COLORS*utils.CONST_NUM_SMALL_PALETTES, plte.Length );
#endif

			// detect valid borders of an image
			{
				BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, img_width, img_height ), ImageLockMode.ReadOnly, _bmp.PixelFormat );
				
				if( bmp_data != null )
				{
					if( _apply_palette )// && plte.Length <= 16 ) <-- there are 256 colors palette on Linux here... why?!..
					{
						// find nearest colors
#if DEF_NES
						for( i = 0; i < clrs_cnt; i++ )
						{
							palette_group.Instance.get_palettes_arr()[ 0 ].get_color_inds()[ i ] = utils.find_nearest_color_ind( plte[ i % 4 ].ToArgb() );
						}
						
						palette_group.Instance.get_palettes_arr()[ 0 ].update();
						palette_group.Instance.active_palette = 0;
#elif DEF_SMS
						for( i = 0; i < clrs_cnt; i++ )
						{
							palette_group.Instance.get_palettes_arr()[ i / utils.CONST_NUM_SMALL_PALETTES ].get_color_inds()[ i % utils.CONST_NUM_SMALL_PALETTES ] = utils.find_nearest_color_ind( plte[ i % 16 ].ToArgb() );
						}

						for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
						{
							palette_group.Instance.get_palettes_arr()[ i ].update();
						}
#elif DEF_FIXED_LEN_PALETTE16_ARR
						palettes_array plt_arr = palettes_array.Instance;
						plt_arr.palette_index = _palette_slot;
						
						for( i = 0; i < clrs_cnt; i++ )
						{
							plt_arr.update_color( _palette_slot, i, utils.find_nearest_color_ind( plte[ i % 16 ].ToArgb() ) );
						}
						
						plt_arr.update_palette();
#endif
					}
					
					// fill the lines_arr
					// (indexed BMP has no transparency in fact)
					{
						IntPtr data_ptr = bmp_data.Scan0;
						
						for( i = 0; i < img_height; i++ )
						{
							pixels_line = new byte[ img_width ];
	
							for( j = 0; j < img_width; j++ )
							{
								if( _bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed )
								{
									index_byte = Marshal.ReadByte( data_ptr, j + ( i * bmp_data.Stride ) );
									
									pixels_line[ j ] = ( byte )( index_byte & index_clamp_val );
								}
								else
								{
									index_byte = Marshal.ReadByte( data_ptr, ( j >> 1 ) + ( i * bmp_data.Stride ) );
									
									pixels_line[ j ] = ( byte )( ( ( ( j & 0x01 ) == 0x01 ) ? ( index_byte & 0x0f ):( ( index_byte & 0xf0 ) >> 4 ) ) & index_clamp_val );
								}
							}
							
							lines_arr.Add( pixels_line );
						}
					}
					
					_bmp.UnlockBits( bmp_data );
				}
			}
			
			return cut_CHRs( spr_params, min_x, max_x, min_y, max_y, lines_arr, false, -1, -1, false, _palette_slot );
		}
		
		public sprite_params setup( PngReader _png_reader, bool _apply_palette, bool _crop_image, int _palette_slot )
		{
			sprite_params spr_params 	= new sprite_params( this );
			
			if( _png_reader == null )
			{
				return spr_params;
			}

			int img_width 	= _png_reader.ImgInfo.Cols;
			int img_height 	= _png_reader.ImgInfo.Rows;
			
			List< byte[] > lines_arr = new List< byte[] >( img_height );
			
			int i;
			int j;
			
			int min_x =  0;
			int max_x = img_width - 1;

			int min_y = 0;
			int max_y = img_height - 1;
			
			byte[] pixels_line  = null;

			PngChunkPLTE plte 	= _png_reader.GetMetadata().GetPLTE();
			PngChunkTRNS trns 	= _png_reader.GetMetadata().GetTRNS();
			ImageLine line 		= null;
			
			int alpha_ind	= -1;
			int num_colors	= plte.GetNentries();
			
			// detect useful image borders
			{
				if( trns != null )
				{
					int size;
					
					alpha_ind = trns.GetPalletteAlpha().Length - 1;
					
					if( _crop_image )
					{
						min_x = img_width + 1;
						max_x = -1;
			
						min_y = img_height + 1;
						max_y = -1;
					}
					
					bool transp_line = false;
					
					for( i = 0; i < img_height; i++ )
					{
						line = _png_reader.ReadRowByte( i );
						
						if( line.ImgInfo.Packed )
						{
							line = line.unpackToNewImageLine();
						}
						
						pixels_line = new byte[ img_width ];
						
						Array.Copy( line.GetScanlineByte(), pixels_line, img_width );
						
						lines_arr.Add( pixels_line );
						
						size 	= pixels_line.Length;
						
						transp_line = true; 
						
						for( j = 0; j < size; j++ )
						{
							// if pixel is not transparent
							if( _crop_image && ( pixels_line[ j ] != alpha_ind ) )
							{
								if( min_x > j )
								{
									min_x = j;
								}
								
								if( max_x < j )
								{
									max_x = j;
								}
								
								transp_line = false;
							}
						}
						
						// if line is not transparent
						if( _crop_image && !transp_line )
						{
							if( min_y > i )
							{
								min_y = i;
							}
							
							if( max_y < i )
							{
								max_y = i;
							}
						}
					}
				}

				// find nearest colors
				if( _apply_palette )
				{
#if DEF_FIXED_LEN_PALETTE16_ARR
					palettes_array plt_arr = palettes_array.Instance;
					plt_arr.palette_index = _palette_slot;
#endif
					for( i = 0; i < num_colors; i++ )
					{
#if DEF_FIXED_LEN_PALETTE16_ARR
						plt_arr.update_color( _palette_slot, i, utils.find_nearest_color_ind( plte.GetEntry( ( trns != null ) ? ( i + alpha_ind ) % num_colors:i ) ) );
#else
						palette_group.Instance.get_palettes_arr()[ i / utils.CONST_NUM_SMALL_PALETTES ].get_color_inds()[ i % utils.CONST_NUM_SMALL_PALETTES ] = utils.find_nearest_color_ind( plte.GetEntry( ( trns != null ) ? ( i + alpha_ind ) % num_colors:i ) );
#endif
					}
					
#if DEF_FIXED_LEN_PALETTE16_ARR
					plt_arr.update_palette();
#else
					for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
					{
						palette_group.Instance.get_palettes_arr()[ i ].update();
					}
#endif
#if DEF_NES							
					palette_group.Instance.active_palette = 0;
#endif
				}
				
				// fill the lines_arr if sprite has no transparency
				if( lines_arr.Count == 0 )
				{
					for( i = 0; i < img_height; i++ )
					{
						line = _png_reader.ReadRowByte( i );
						
						if( line.ImgInfo.Packed )
						{
							line = line.unpackToNewImageLine();
						}
						
						pixels_line = new byte[ img_width ];
						
						Array.Copy( line.GetScanlineByte(), pixels_line, img_width );
						
						lines_arr.Add( pixels_line );
					}
				}
			}

			return cut_CHRs( spr_params, min_x, max_x, min_y, max_y, lines_arr, ( trns != null ), alpha_ind, num_colors, _crop_image, _palette_slot );
		}		
		
		private sprite_params cut_CHRs( sprite_params _spr_params, int _min_x, int _max_x, int _min_y, int _max_y, List< byte[] > _lines_arr, bool _alpha, int _alpha_ind, int _num_colors, bool _crop_image, int _palette_slot )
		{
			// cut sprite by tiles 8x8
			{
				_spr_params.m_offset_x = _min_x;
				_spr_params.m_offset_y = _min_y;
				
				int dx_incr = _max_x - _min_x + 1;
				int dy_incr = _max_y - _min_y + 1;
				
				_spr_params.m_size_x = ( ( dx_incr % utils.CONST_CHR_SIDE_PIXELS_CNT != 0 ) ? ( ( ( dx_incr ) + utils.CONST_CHR_SIDE_PIXELS_CNT ) & -utils.CONST_CHR_SIDE_PIXELS_CNT ):dx_incr ) >> utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
				_spr_params.m_size_y = ( ( dy_incr % utils.CONST_CHR_SIDE_PIXELS_CNT != 0 ) ? ( ( ( dy_incr ) + utils.CONST_CHR_SIDE_PIXELS_CNT ) & -utils.CONST_CHR_SIDE_PIXELS_CNT ):dy_incr ) >> utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
				
				int x;
				int y;
				
				int i;
				
				byte[] pixels_line  = null;
				
				byte[] pixels_row  = new byte[ utils.CONST_CHR_SIDE_PIXELS_CNT ];
				
				int n_lines = _lines_arr.Count;
				
				CHR_data 		chr_data = null;
				CHR_data_attr 	chr_attr = null;
				
				int chr_pos_x = 0;
				int chr_pos_y = 0;
				
				byte pix_ind 	 = 0;
				int num_row_pixs = 0;
				
				int pix_acc = 0;
				
				int col_n;
				int row_n;
				int x_offset = 0;
				
				for( y = 0; y < _spr_params.m_size_y; y++ )
				{
					x_offset = 0;
					
					for( x = 0; x < _spr_params.m_size_x; x++ )
					{
						chr_pos_x = x*utils.CONST_CHR_SIDE_PIXELS_CNT;
						chr_pos_y = y*utils.CONST_CHR_SIDE_PIXELS_CNT;
						
						pix_acc = 0;
						
						chr_data = new CHR_data();
						chr_attr = new CHR_data_attr( chr_pos_x, chr_pos_y );
#if DEF_FIXED_LEN_PALETTE16_ARR
						chr_attr.palette_ind = _palette_slot;
#endif
						chr_pos_x += _min_x;
						chr_pos_y += _min_y;
						
						// calc first non zero column
						if( _alpha )
						{
							pix_acc = 0;
							
							chr_pos_x += x_offset;
							
							for( col_n = 0; col_n < utils.CONST_CHR_SIDE_PIXELS_CNT; col_n++ )
							{
								for( row_n = 0; row_n < utils.CONST_CHR_SIDE_PIXELS_CNT; row_n++ )
								{
									pixels_line = ( chr_pos_y + row_n < _lines_arr.Count ) ? _lines_arr[ chr_pos_y + row_n ]:null;
									
									pix_ind = ( chr_pos_x + col_n <= _max_x ) ? ( pixels_line != null ? pixels_line[ chr_pos_x + col_n ]:( _alpha ? (byte)_alpha_ind:(byte)0 ) ):( _alpha ? (byte)_alpha_ind:(byte)0 );
									
									if( _alpha && ( _alpha_ind != 0 ) )
									{
										pix_ind += ( byte )( _num_colors - _alpha_ind );
										pix_ind %= ( byte )_num_colors;
									}
									
									pix_acc += pix_ind;
								}
								
								if( pix_acc != 0 )
								{
									x_offset	+= col_n;
									chr_pos_x	+= col_n;
									
									chr_attr.x	+= x_offset;
									
									break;
								}
							}
							
							if( col_n == utils.CONST_CHR_SIDE_PIXELS_CNT )
							{
								continue;
							}
						}
						
						for( i = 0; i < utils.CONST_CHR_SIDE_PIXELS_CNT; i++ )
						{
							pixels_line = ( chr_pos_y + i < _lines_arr.Count ) ? _lines_arr[ chr_pos_y + i ]:null;
							
							// copy line
							num_row_pixs = 0;
							
							do
							{
								pix_ind = ( chr_pos_x + num_row_pixs <= _max_x ) ? ( pixels_line != null ? pixels_line[ chr_pos_x + num_row_pixs ]:( _alpha ? (byte)_alpha_ind:(byte)0 ) ):( _alpha ? (byte)_alpha_ind:(byte)0 );

								// fix the pixel index so that the first one will be transparency index 
								if( _alpha && ( _alpha_ind != 0 ) )
								{
									pix_ind += ( byte )( _num_colors - _alpha_ind );
									pix_ind %= ( byte )_num_colors;
								}

								pix_acc += pix_ind;
								
								pixels_row[ num_row_pixs ] = pix_ind;
							}
							while( ++num_row_pixs < utils.CONST_CHR_SIDE_PIXELS_CNT );
							
							chr_data.push_line( pixels_row );
						}
						
						// save non-zero tile 
						if( !_crop_image || ( pix_acc != 0 || !_alpha ) )
						{
							chr_attr.CHR_ind = m_CHR_arr.Count; 
							_spr_params.m_CHR_attr.Add( chr_attr );
							
							m_CHR_arr.Add( chr_data );
							
							if( m_CHR_arr.Count >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
							{
								break;
							}
						}
					}
					
					if( m_CHR_arr.Count >= utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
					{
						MainForm.message_box( "The CHR bank is full!", "Data Import", MessageBoxButtons.OK, MessageBoxIcon.Warning );
						break;
					}
				}
			}
			
			// convert tiles into pixels
			_spr_params.m_size_x <<= utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
			_spr_params.m_size_y <<= utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
			
			return _spr_params;
		}
		
		public void add_data_range( CHR_data_group _chr_data )
		{
			m_CHR_arr.InsertRange( m_CHR_arr.Count, _chr_data.get_data() );
		}
		
		public static bool can_merge( CHR_data_group _data1, CHR_data_group _data2, e_CHR_packing_type _packing_type )
		{
			if( _data1.id == _data2.id )
			{
				return false;
			}
			
			int total_size 	= _data1.get_size_bytes() + _data2.get_size_bytes();
			int max_size 	= 0;
			
			switch( _packing_type )
			{
				case CHR_data_group.e_CHR_packing_type._1KB:
					{
						max_size = 1024;
					}
					break;
					
				case CHR_data_group.e_CHR_packing_type._2KB:
					{
						max_size = 2048;
					}
					break;
					
				case CHR_data_group.e_CHR_packing_type._4KB:
					{
						max_size = 4096;
					}
					break;
#if DEF_SMS
				case CHR_data_group.e_CHR_packing_type._8KB:
					{
						max_size = 8192;
					}
					break;
#elif DEF_PCE
				case CHR_data_group.e_CHR_packing_type._8KB:
					{
						max_size = 8192;
					}
					break;
					
				case CHR_data_group.e_CHR_packing_type._16KB:
					{
						max_size = 16384;
					}
					break;
					
				case CHR_data_group.e_CHR_packing_type._32KB:
					{
						max_size = 32768;
					}
					break;
#endif
				default:
					{
						throw new Exception( "Unexpected data packing type detected!" );
					}
			}
			
			return total_size <= max_size ? true:false;
		}
		
		public void import( BinaryReader _br )
		{
			byte[] pix_arr = new byte[ utils.CONST_CHR_SIDE_PIXELS_CNT ];
			byte[] tmp_arr = new byte[ utils.CONST_CHR_NATIVE_SIZE_IN_BYTES ];
			
			int i;
			int j;
			
			byte[] data_arr = new byte[ utils.CONST_CHR_NATIVE_SIZE_IN_BYTES >> 3 ];
			
#if DEF_SMS || DEF_PCE
			int ind_offset;
#endif
			int shift_dec1_cnt;
			
			CHR_data chr_data;
			
			if( _br.BaseStream.Length < utils.CONST_CHR_NATIVE_SIZE_IN_BYTES )
			{
				_br.BaseStream.Position = _br.BaseStream.Length;
				
				return;
			}
			
			do
			{
				chr_data = new CHR_data();
				
				tmp_arr = _br.ReadBytes( utils.CONST_CHR_NATIVE_SIZE_IN_BYTES );
				
				for( i = 0; i < utils.CONST_CHR_SIDE_PIXELS_CNT; i++ )
				{
#if DEF_NES
					data_arr[ 0 ]	= tmp_arr[ i ];
					data_arr[ 1 ]	= tmp_arr[ i + utils.CONST_CHR_SIDE_PIXELS_CNT ];
#elif DEF_SMS
					ind_offset = i << 2;
					
					data_arr[ 0 ]	= tmp_arr[ ind_offset ];
					data_arr[ 1 ]	= tmp_arr[ ind_offset + 1 ];
					data_arr[ 2 ]	= tmp_arr[ ind_offset + 2 ];
					data_arr[ 3 ]	= tmp_arr[ ind_offset + 3 ];
#elif DEF_PCE
					ind_offset = i << 1;
					
					data_arr[ 0 ]	= tmp_arr[ ind_offset ];
					data_arr[ 1 ]	= tmp_arr[ ind_offset + 32 ];
					data_arr[ 2 ]	= tmp_arr[ ind_offset + 64 ];
					data_arr[ 3 ]	= tmp_arr[ ind_offset + 96 ];
					
					++ind_offset;
					
					data_arr[ 4 ]	= tmp_arr[ ind_offset ];
					data_arr[ 5 ]	= tmp_arr[ ind_offset + 32 ];
					data_arr[ 6 ]	= tmp_arr[ ind_offset + 64 ];
					data_arr[ 7 ]	= tmp_arr[ ind_offset + 96 ];
#else
...
#endif
					for( j = 0; j < utils.CONST_CHR_SIDE_PIXELS_CNT; j++ )
					{
#if DEF_NES || DEF_SMS
						shift_dec1_cnt = utils.CONST_CHR_SIDE_PIXELS_CNT - 1 - j;
#endif
#if DEF_NES
						pix_arr[ j ] = ( byte )( ( ( data_arr[ 0 ] & ( 1 << shift_dec1_cnt ) ) >> shift_dec1_cnt ) | ( ( ( data_arr[ 1 ] << 1 ) & ( 1 << ( utils.CONST_CHR_SIDE_PIXELS_CNT - j ) ) ) >> shift_dec1_cnt ) );
#elif DEF_SMS
						pix_arr[ j ] = ( byte )( ( ( data_arr[ 0 ] >> shift_dec1_cnt & 0x01 ) ) | ( ( ( data_arr[ 1 ] >> shift_dec1_cnt & 0x01 ) ) << 1 ) | ( ( ( data_arr[ 2 ] >> shift_dec1_cnt & 0x01 ) ) << 2 ) | ( ( ( data_arr[ 3 ] >> shift_dec1_cnt & 0x01 ) ) << 3 ) );
#elif DEF_PCE
						if( j < 8 )
						{
							shift_dec1_cnt = 7 - j;
							pix_arr[ j ] = ( byte )( ( ( data_arr[ 4 ] >> shift_dec1_cnt & 0x01 ) ) | ( ( ( data_arr[ 5 ] >> shift_dec1_cnt & 0x01 ) ) << 1 ) | ( ( ( data_arr[ 6 ] >> shift_dec1_cnt & 0x01 ) ) << 2 ) | ( ( ( data_arr[ 7 ] >> shift_dec1_cnt & 0x01 ) ) << 3 ) );
						}
						else
						{
							shift_dec1_cnt = 7 - ( j - 8 );
							pix_arr[ j ] = ( byte )( ( ( data_arr[ 0 ] >> shift_dec1_cnt & 0x01 ) ) | ( ( ( data_arr[ 1 ] >> shift_dec1_cnt & 0x01 ) ) << 1 ) | ( ( ( data_arr[ 2 ] >> shift_dec1_cnt & 0x01 ) ) << 2 ) | ( ( ( data_arr[ 3 ] >> shift_dec1_cnt & 0x01 ) ) << 3 ) );
						}
#else
...
#endif
					}
					
					chr_data.push_line( pix_arr );
				}

				m_CHR_arr.Add( chr_data );
				
				if( m_CHR_arr.Count == utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
				{
					break;
				}
			}
			while( _br.BaseStream.Position + utils.CONST_CHR_NATIVE_SIZE_IN_BYTES <= _br.BaseStream.Length );
			
			if( m_CHR_arr.Count < utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
			{
				// all data from the file has been read!
				// move data pointer to the end of the file
				// as a sign of the end of the reading,
				// so as not to try to create a sprite when
				// data in the file is less than one CHR
				_br.BaseStream.Position = _br.BaseStream.Length;
			}
		}
		
#if DEF_NES
		public long export( string _filename, bool _save_padding )
#elif DEF_SMS
		public long export( string _filename, int _bpp )
#elif DEF_PCE
		public long export( string _filename )
#endif
		{
			BinaryWriter bw = new BinaryWriter( File.Open( _filename, FileMode.Create ) );
			
			CHR_data chr_data;
			
#if DEF_SMS
			int max_clr_ind = ( 2 << ( _bpp - 1 ) ) - 1;
#endif
			int size = m_CHR_arr.Count;
			
			for( int i = 0; i < size; i++ )
			{
				chr_data = m_CHR_arr[ i ];
#if DEF_NES
				for( int j = 0; j < 2; j++ )
				{
					for( int y = 0; y < utils.CONST_CHR_SIDE_PIXELS_CNT; y++ )
					{
						byte	data = 0;
						int		val;
						
						for( int x = 0; x < utils.CONST_CHR_SIDE_PIXELS_CNT; x++ )
						{
							val = chr_data.get_data()[ ( y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS ) + ( ( utils.CONST_CHR_SIDE_PIXELS_CNT - 1 ) - x ) ];
							
							data |= ( byte )( ( ( val >> j ) & 0x01 ) << x );
						}
						
						bw.Write( data );
					}
				}
#elif DEF_SMS
				for( int y = 0; y < utils.CONST_CHR_SIDE_PIXELS_CNT; y++ )
				{
					for( int j = 0; j < _bpp; j++ )
					{
						byte	data = 0;
						int		val;
						
						for( int x = 0; x < utils.CONST_CHR_SIDE_PIXELS_CNT; x++ )
						{
							val = chr_data.get_data()[ ( y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS ) + ( ( utils.CONST_CHR_SIDE_PIXELS_CNT - 1 ) - x ) ];
							
							val = ( val > max_clr_ind ) ? 0:val;
							
							data |= ( byte )( ( ( val >> j ) & 0x01 ) << x );
						}
						
						bw.Write( data );
					}
				}
#elif DEF_PCE
				PCE_metasprite_exporter.save_CHR( bw, chr_data );
#else
...
#endif				
			}
#if DEF_NES
			// save padding data aligned to 1/2/4 KB
			if( _save_padding == true )
			{
				int CHR_data_bytes_size = get_size_bytes();
				
				int padding = utils.get_padding( CHR_data_bytes_size );
				
				if( padding != 0 )
				{
					byte data = 0;
					
					while( padding-- != 0 )
					{
						bw.Write( data );
					}
				}
			}
#endif
			long data_size = bw.BaseStream.Length;
			bw.Close();
			
			return data_size;
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_id );
			_bw.Write( m_link_cnt );

			if( m_CHR_arr != null )
			{
				int chrs_size = m_CHR_arr.Count;
				
				_bw.Write( chrs_size );
				
				for( int i = 0; i < chrs_size; i++ )
				{
					m_CHR_arr[ i ].save( _bw );
				}
			}
			else
			{
				_bw.Write( ( int )( 0 ) );
			}
		}
		
		public void load( BinaryReader _br )
		{
			m_id 			= _br.ReadInt32();
			m_link_cnt 		= _br.ReadInt32();

			CHR_data chr_data;
			
			int chrs_size = _br.ReadInt32();
			
			for( int i = 0; i < chrs_size; i++ )
			{
				chr_data = new CHR_data();
				chr_data.load( _br );
				
				m_CHR_arr.Add( chr_data );
			}
		}
	}
}
