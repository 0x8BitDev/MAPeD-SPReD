/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2017
 * Time: 11:07
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
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
		public enum ECHRPackingType
		{
			pt_NO_PACKING = 0,
			pt_1KB,
			pt_2KB,
			pt_4KB,
		};
		
		private List< CHR8x8_data > m_CHR_arr	= null;
	
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
			m_CHR_arr = new List< CHR8x8_data >( 100 );
			
			id = ++m_num_ids;
		}
		
		public void reset()
		{
			m_CHR_arr.ForEach( delegate( CHR8x8_data _obj ) { _obj.reset(); } );
			m_CHR_arr.Clear();
		}
		
		public static void reset_ids_cnt()
		{
			m_num_ids = -1;
		}
		
		public int get_size_bytes()
		{
			return m_CHR_arr.Count << 4;
		}
		
		public string get_filename()
		{
			return name + ".bin";
		}
		
		public List< CHR8x8_data > get_data()
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

		public sprite_params setup( Bitmap _bmp )
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
			
			// detect valid borders of an image
			{
				BitmapData bmp_data = _bmp.LockBits( new Rectangle( 0, 0, img_width, img_height ), ImageLockMode.ReadOnly, _bmp.PixelFormat );
				
				if( bmp_data != null )
				{
					IntPtr data_ptr = bmp_data.Scan0;

					if( plte.Length == 5 )
					{
						int size;
						
						min_x = img_width + 1;
						max_x = -1;
			
						min_y = img_height + 1;
						max_y = -1;
						
						bool transp_line = false;
						
						for( i = 0; i < img_height; i++ )
						{
							pixels_line = new byte[ img_width ];

							for( j = 0; j < img_width; j++ )
							{
								index_byte = Marshal.ReadByte( data_ptr, ( j >> 1 ) + ( i * bmp_data.Stride ) );
								
								pixels_line[ j ] = ( byte )( ( ( ( j & 0x01 ) == 0x01 ) ? ( index_byte & 0x0f ):( ( index_byte & 0xf0 ) >> 4 ) ) & 0x03 );
							}
							
							lines_arr.Add( pixels_line );
							
							size = img_width;
							
							transp_line = true; 
							
							for( j = 0; j < size; j++ )
							{
								// if pixel is transparent
								if( plte[ pixels_line[ j ] ] != plte[ 3 ] )
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
							
							// if line is transparent
							if(!transp_line )
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
					
					// fill the lines_arr if sprite has no transparency
					if( lines_arr.Count == 0 )
					{
						for( i = 0; i < img_height; i++ )
						{
							pixels_line = new byte[ img_width ];
	
							for( j = 0; j < img_width; j++ )
							{
								index_byte = Marshal.ReadByte( data_ptr, ( j >> 1 ) + ( i * bmp_data.Stride ) );
								
								pixels_line[ j ] = ( byte )( ( ( ( j & 0x01 ) == 0x01 ) ? ( index_byte & 0x0f ):( ( index_byte & 0xf0 ) >> 4 ) ) & 0x03 );
							}
							
							lines_arr.Add( pixels_line );
						}
					}
					
					_bmp.UnlockBits( bmp_data );
				}
			}
			
			return cut_CHRs( spr_params, min_x, max_x, min_y, max_y, lines_arr, ( plte.Length == 5 ) );
		}
		
		public sprite_params setup( PngReader _png_reader )
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
			
			PngChunkTRNS trns 	= _png_reader.GetMetadata().GetTRNS();
			ImageLine line 		= null;
			
			// detect useful borders of an image
			{
				if( trns != null )
				{
					int size;
					
					int[] plte_alpha = trns.GetPalletteAlpha();
					
					int alpha = plte_alpha[ 3 ] << 24 | plte_alpha[ 2 ] << 16 | plte_alpha[ 1 ] << 8 | plte_alpha[ 0 ];
					
					min_x = img_width + 1;
					max_x = -1;
		
					min_y = img_height + 1;
					max_y = -1;
					
					PngChunkPLTE plte 	= _png_reader.GetMetadata().GetPLTE();
					
					bool transp_line = false;
					
					for( i = 0; i < img_height; i++ )
					{
						line 		= _png_reader.ReadRowByte( i );
						pixels_line = new byte[ img_width ];
						
						Array.Copy( line.GetScanlineByte(), pixels_line, img_width );
						
						lines_arr.Add( pixels_line );
						
						size 	= pixels_line.Length;
						
						transp_line = true; 
						
						for( j = 0; j < size; j++ )
						{
							// if pixel is transparent 
							if( plte.GetEntry( pixels_line[ j ] ) != alpha )
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
						
						// if line is transparent
						if(!transp_line )
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
				
				// fill the lines_arr if sprite has no transparency
				if( lines_arr.Count == 0 )
				{
					for( i = 0; i < img_height; i++ )
					{
						line 		= _png_reader.ReadRowByte( i );
						pixels_line = new byte[ img_width ];
						
						Array.Copy( line.GetScanlineByte(), pixels_line, img_width );
						
						lines_arr.Add( pixels_line );
					}
				}
			}

			return cut_CHRs( spr_params, min_x, max_x, min_y, max_y, lines_arr, ( trns != null ) );
		}		
		
		private sprite_params cut_CHRs( sprite_params _spr_params, int _min_x, int _max_x, int _min_y, int _max_y, List< byte[] > _lines_arr, bool _alpha )
		{
			// cut sprite by tiles 8x8
			{
				_spr_params.m_offset_x = _min_x;
				_spr_params.m_offset_y = _min_y;
				
				int dx_incr = _max_x - _min_x + 1;
				int dy_incr = _max_y - _min_y + 1;
				
				_spr_params.m_size_x = ( ( dx_incr%utils.CONST_CHR8x8_SIDE_PIXELS_CNT != 0 ) ? ( ( ( dx_incr ) + utils.CONST_CHR8x8_SIDE_PIXELS_CNT ) & -utils.CONST_CHR8x8_SIDE_PIXELS_CNT ):dx_incr ) >> utils.CONST_CHR8x8_SIDE_PIXELS_CNT_POW_BITS;
				_spr_params.m_size_y = ( ( dy_incr%utils.CONST_CHR8x8_SIDE_PIXELS_CNT != 0 ) ? ( ( ( dy_incr ) + utils.CONST_CHR8x8_SIDE_PIXELS_CNT ) & -utils.CONST_CHR8x8_SIDE_PIXELS_CNT ):dy_incr ) >> utils.CONST_CHR8x8_SIDE_PIXELS_CNT_POW_BITS;
				
				int x;
				int y;
				
				int i;
				
				byte[] pixels_line  = null;
				
				byte[] pixels_row  = new byte[ utils.CONST_CHR8x8_SIDE_PIXELS_CNT ];
				
				int n_lines = _lines_arr.Count;
				
				CHR8x8_data 	chr_data = null;
				CHR_data_attr 	chr_attr = null;
				
				int chr_pos_x = 0;
				int chr_pos_y = 0;
				
				byte pix_ind 	 = 0;
				int num_row_pixs = 0;
				
				int pix_acc = 0;
				
				for( y = 0; y < _spr_params.m_size_y; y++ )
				{
					for( x = 0; x < _spr_params.m_size_x; x++ )
					{
						chr_pos_x = x*utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
						chr_pos_y = y*utils.CONST_CHR8x8_SIDE_PIXELS_CNT;
						
						pix_acc = 0;
						
						chr_data = new CHR8x8_data();
						chr_attr = new CHR_data_attr( chr_pos_x, chr_pos_y );
						
						chr_pos_x += _min_x;
						chr_pos_y += _min_y;
						
						for( i = 0; i < utils.CONST_CHR8x8_SIDE_PIXELS_CNT; i++ )
						{
							pixels_line = ( chr_pos_y + i < _lines_arr.Count ) ? _lines_arr[ chr_pos_y + i ]:null;
							
							// copy line
							num_row_pixs = 0;
							do
							{
								pix_ind = ( chr_pos_x + num_row_pixs <= _max_x ) ? ( pixels_line != null ? pixels_line[ chr_pos_x + num_row_pixs ]:( _alpha ? (byte)3:(byte)0 ) ):( _alpha ? (byte)3:(byte)0 );
									
								// if pixel has transparency
								// increment an index, so that the first one will be transparency index ( it last by default in 4bpp PNG\BMP, the third one )
								if( _alpha )
								{
									++pix_ind;
									pix_ind %= (byte)4;
								}
							
								pix_acc += pix_ind;
								
								pixels_row[ num_row_pixs ] = pix_ind;
							}
							while( ++num_row_pixs < utils.CONST_CHR8x8_SIDE_PIXELS_CNT );
							
							chr_data.push_line( pixels_row );
						}
						
						// save non-zero tile 
						if( pix_acc != 0 ||!_alpha )
						{
							chr_attr.CHR_id = m_CHR_arr.Count; 
							_spr_params.m_CHR_attr.Add( chr_attr );
							
							m_CHR_arr.Add( chr_data );
						}
					}
				}
			}
			
			// convert tiles into pixels
			_spr_params.m_size_x <<= utils.CONST_CHR8x8_SIDE_PIXELS_CNT_POW_BITS;
			_spr_params.m_size_y <<= utils.CONST_CHR8x8_SIDE_PIXELS_CNT_POW_BITS;
			
			return _spr_params;
		}
		
		public void add_data_range( CHR_data_group _chr_data )
		{
			m_CHR_arr.InsertRange( m_CHR_arr.Count, _chr_data.get_data() );
		}
		
		public static bool can_merge( CHR_data_group _data1, CHR_data_group _data2, ECHRPackingType _packing_type )
		{
			if( _data1.id == _data2.id )
			{
				return false;
			}
			
			int total_size 	= _data1.get_size_bytes() + _data2.get_size_bytes();
			int max_size 	= 0;
			
			switch( _packing_type )
			{
				case CHR_data_group.ECHRPackingType.pt_1KB:
					{
						max_size = 1024;
					}
					break;
					
				case CHR_data_group.ECHRPackingType.pt_2KB:
					{
						max_size = 2048;
					}
					break;
					
				case CHR_data_group.ECHRPackingType.pt_4KB:
					{
						max_size = 4096;
					}
					break;
					
				default:
					{
						return false;
					}
			}
			
			return total_size <= max_size ? true:false;;
		}
		
		public void import( BinaryReader _br )
		{
			byte[] pix_arr = new byte[ 8 ];
			byte[] tmp_arr = new byte[ 16 ];
			
			int i;
			int j;
			
			byte low_byte;
			byte high_byte;
			
			int shift_7_cnt;
			
			CHR8x8_data chr_data;
			
			if( _br.BaseStream.Length < 16 )
			{
				_br.BaseStream.Position = _br.BaseStream.Length;
				
				return;
			}
			
			do
			{
				chr_data = new CHR8x8_data();
				
				tmp_arr = _br.ReadBytes( 16 );
				
				for( i = 0; i < 8; i++ )
				{
					low_byte 	= tmp_arr[ i ];
					high_byte 	= tmp_arr[ i + 8 ];
					
					for( j = 0; j < 8; j++ )
					{
						shift_7_cnt = 7 - j;
						
						pix_arr[ j ] = ( byte )( ( ( low_byte  & ( 1 << shift_7_cnt ) ) >> shift_7_cnt ) | ( ( ( high_byte << 1 ) & ( 1 << ( 8 - j ) ) ) >> shift_7_cnt ) );
					}
					
					chr_data.push_line( pix_arr );
				}
				
				m_CHR_arr.Add( chr_data );
				
				if( m_CHR_arr.Count == utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
				{
					break;
				}
			}
			while( _br.BaseStream.Position + 16 <= _br.BaseStream.Length );
			
			if( m_CHR_arr.Count < utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
			{
				// all data from the file has been read!
				// move data pointer to the end of the file
				// as a sign of the end of the reading,
				// so as not to try to create a sprite when
				// the data in the file is less than one CHR
				_br.BaseStream.Position = _br.BaseStream.Length;
			}
		}
		
		public void export( string _path, string _filename )
		{
			BinaryWriter bw = new BinaryWriter( File.Open( _path + "\\" + _filename + "_" + get_filename(), FileMode.Create ) );
			
			CHR8x8_data chr_data;
			
			int i;
			int x;
			int y;
			
			int val;
			byte data;
			
			int size = m_CHR_arr.Count;
			
			for( i = 0; i < size; i++ )
			{
				chr_data = m_CHR_arr[ i ];
				
				// the first 8 bytes out of 16 ones
				for( y = 0; y < utils.CONST_CHR8x8_SIDE_PIXELS_CNT; y++ )
				{
					data = 0;
					
					for( x = 0; x < utils.CONST_CHR8x8_SIDE_PIXELS_CNT; x++ )
					{
						val = chr_data.get_data()[ ( y << 3 ) + ( 7 - x ) ];
						
						data |= ( byte )( ( val & 0x01 ) << x );
					}
					
					bw.Write( data );
				}
				
				// the second 8 bytes of CHR
				for( y = 0; y < utils.CONST_CHR8x8_SIDE_PIXELS_CNT; y++ )
				{
					data = 0;
					
					for( x = 0; x < utils.CONST_CHR8x8_SIDE_PIXELS_CNT; x++ )
					{
						val = chr_data.get_data()[ ( y << 3 ) + ( 7 - x ) ];
						
						data |= ( byte )( ( val >> 1 ) << x );
					}
					
					bw.Write( data );
				}
			}
			
			// save padding data to 1/2/4 KB
			int CHR_data_bytes_size = get_size_bytes();
			
			int padding = utils.get_padding( CHR_data_bytes_size );
			
			if( padding != 0 )
			{
				data = 0;
				
				while( padding-- != 0 )
				{
					bw.Write( data );
				}
			}
			
			bw.Close();
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

			CHR8x8_data chr_data;
			
			int chrs_size = _br.ReadInt32();
			
			for( int i = 0; i < chrs_size; i++ )
			{
				chr_data = new CHR8x8_data();
				chr_data.load( _br );
				
				m_CHR_arr.Add( chr_data );
			}
		}
	}
}
