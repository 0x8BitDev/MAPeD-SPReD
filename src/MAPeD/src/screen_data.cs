/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 29.03.2021
 * Time: 15:12
 */
using System;
using System.IO;
using System.Linq;

namespace MAPeD
{
	/// <summary>
	/// Description of screen_data.
	/// </summary>
	/// 
	
	public class screen_data
	{
		public ushort[] m_arr = null;
		
		private int m_width = 0;
		private int m_height = 0;

		public int width
		{
			get{ return m_width; }
		}
		
		public int height
		{
			get{ return m_height; }
		}
		
		public int length
		{
			get { return m_arr.Length; }
		}

		public screen_data( data_sets_manager.EScreenDataType _type )
		{
			alloc( utils.get_screen_num_width_tiles_uni( _type ), utils.get_screen_num_height_tiles_uni( _type ) );
		}

		public screen_data( int _width, int _height )
		{
			alloc( _width, _height );
		}
		
		public void reset()
		{
			m_arr = null;
		}
		
		private void alloc( int _width, int _height )
		{
			m_width		= _width;
			m_height	= _height;
			
			m_arr = new ushort[ m_width * m_height ];
		}
		
		public ushort get_tile( int _ind )
		{
			return m_arr[ _ind ];
		}
		
		public void set_tile( int _ind, ushort _val )
		{
			m_arr[ _ind ] = _val;
		}
		
		public ushort max_val()
		{
			return m_arr.Max();
		}
		
		public ushort min_val()
		{
			return m_arr.Min();
		}
		
		public bool equal( screen_data _scr )
		{
			return System.Linq.Enumerable.SequenceEqual( m_arr, _scr.m_arr );
		}
		
		public screen_data copy()
		{
			screen_data data = new screen_data( m_width, m_height );
			
			Array.Copy( m_arr, data.m_arr, length );
			
			return data;
		}
		
		public void swap_col_row_data()
		{
			utils.swap_columns_rows_order< ushort >( m_arr, m_width, m_height );
		}
		
		public void save( BinaryWriter _bw, bool _save_size )
		{
			if( _save_size )
			{
				_bw.Write( m_width );
				_bw.Write( m_height );
			}

			for( int i = 0; i < length; i++ )
			{
				_bw.Write( m_arr[ i ] );
			}
		}
		
		public static screen_data load( int _ver, BinaryReader _br )
		{
			int width;
			int height;
			
			if( _ver < 5 )
			{
				width	= _br.ReadByte();
				height	= _br.ReadByte();
			}
			else
			{
				width	= _br.ReadInt32();
				height	= _br.ReadInt32();
			}
				
			screen_data data = new screen_data( width, height );
			data.load( _ver, _br, -1, -1 );
			
			return data;
		}
		
		public void load( int _ver, BinaryReader _br, int _size, int _offset )
		{
			int i;
			
			int size	= ( _size < 0 ) ? length:_size;
			int offset	= ( _offset < 0 ) ? 0:_offset;
			
			if( _ver < 5 )
			{
				for( i = 0; i < size; i++ )
				{
					m_arr[ i + offset ] = ( ushort )_br.ReadByte();
				}
			}
			else
			{
				for( i = 0; i < size; i++ )
				{
					m_arr[ i + offset ] = _br.ReadUInt16();
				}
			}
		}
	}
}
