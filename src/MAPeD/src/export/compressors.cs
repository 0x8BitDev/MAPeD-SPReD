/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2023 ( MIT license. See LICENSE.txt )
 * Date: 01.05.2023
 * Time: 19:24
 */
using System;

namespace MAPeD
{
	/// <summary>
	/// Description of compressors.
	/// </summary>
	public interface i_compressor
	{
		int	encode8( byte[] _src_arr, ref byte[] _encoded_arr );
		int	encode16( ushort[] _src_arr, ref ushort[] _encoded_arr );
	}
	
	public class compressor_RLE : i_compressor
	{
		public compressor_RLE()
		{
			//...
		}
		
		// RLE routine from NESst tool by Shiru
		public int encode8( byte[] _arr, ref byte[] _rle_arr )
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
				return -1;
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
						return -1;
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

		public int encode16( ushort[] _arr, ref ushort[] _rle_arr )
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
				return -1;
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
						return -1;
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
	}
}
