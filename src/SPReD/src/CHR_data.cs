/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 14.03.2017
 * Time: 16:35
 */
using System;
using System.IO;

namespace SPReD
{
	/// <summary>
	/// Description of CHR_data.
	/// </summary>
	
	public class CHR_data
	{
		public enum ETransform
		{
			t_hflip,
			t_vflip,
			t_rotate,
		};
		
		private byte[] 		m_data;
		private int 		m_data_pos = 0;
		
		public CHR_data()
		{
			m_data_pos = 0;
			
			m_data = new byte[ utils.CONST_CHR_TOTAL_PIXELS_CNT ];
		}
		
		public void reset()
		{
			m_data_pos = 0;
			
			Array.Clear( m_data, 0, utils.CONST_CHR_TOTAL_PIXELS_CNT );
			
			m_data = null;
		}
		
		public CHR_data copy()
		{
			CHR_data copy = new CHR_data();
			
			Array.Copy( m_data, copy.m_data, utils.CONST_CHR_TOTAL_PIXELS_CNT );
			
			return copy;
		}

		public void clear()
		{
			Array.Clear( m_data, 0, utils.CONST_CHR_TOTAL_PIXELS_CNT );
		}
		
		public void push_line( byte[] _data )
		{
			Array.Copy( _data, 0, m_data, m_data_pos, utils.CONST_CHR_SIDE_PIXELS_CNT );
			
			m_data_pos += utils.CONST_CHR_SIDE_PIXELS_CNT;
		}
		
		public byte[] get_data()
		{
			return m_data;
		}
		
		public bool is_empty()
		{
			for( int i = 0; i < utils.CONST_CHR_TOTAL_PIXELS_CNT; i++ )
			{
				if( m_data[ i ] > 0 )
				{
					return false;
				}
			}
			
			return true;
		}

		public void fill( byte _val )
		{
			for( int i = 0; i < utils.CONST_CHR_TOTAL_PIXELS_CNT; i++ )
			{
				m_data[ i ] = _val;
			}
		}
		
		private void hflip()
		{
			byte a;
			byte b;
			
			int offset_x;
			int offset_y;
			
			for( int y = 0; y < utils.CONST_CHR_SIDE_PIXELS_CNT; y++ )
			{
				offset_y = y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
				
				for( int x = 0; x < utils.CONST_CHR_SIDE_PIXELS_CNT>>1; x++ )
				{
					offset_x = x + offset_y;
						
					a = m_data[ offset_x ];
					b = m_data[ offset_y + utils.CONST_CHR_SIDE_PIXELS_CNT - 1 - x ];
					
					m_data[ offset_x ] = b;
					m_data[ offset_y + utils.CONST_CHR_SIDE_PIXELS_CNT - 1 - x ] = a;
				}
			}
		}
		
		private void vflip()
		{
			byte a;
			byte b;
			
			int offset_y1;
			int offset_y2;
			
			for( int x = 0; x < utils.CONST_CHR_SIDE_PIXELS_CNT; x++ )
			{
				for( int y = 0; y < utils.CONST_CHR_SIDE_PIXELS_CNT>>1; y++ )
				{
					offset_y1 = x + ( y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS );
					offset_y2 = x + ( ( utils.CONST_CHR_SIDE_PIXELS_CNT - 1 - y ) << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS );
						
					a = m_data[ offset_y1 ];
					b = m_data[ offset_y2 ];
					
					m_data[ offset_y1 ] = b;
					m_data[ offset_y2 ] = a;
				}
			}
		}
		
		private void rot_cw()
		{
			int i;
			int j;
			
			int im8;
			int jm8;
					
			for( i = 0; i < utils.CONST_CHR_SIDE_PIXELS_CNT; i++ ) 
			{
				im8 = i << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
				
		        for( j = i; j < utils.CONST_CHR_SIDE_PIXELS_CNT; j++ ) 
		        {
		            if( i != j ) 
		            {
		            	jm8 = j << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS;
		            	
		                m_data[ i + jm8 ] ^= m_data[ j + im8 ];
		                m_data[ j + im8 ] ^= m_data[ i + jm8 ];
		                m_data[ i + jm8 ] ^= m_data[ j + im8 ];
		            }
		        }
		    }
			
			hflip();
		}
		
		public void transform( ETransform _type )
		{
			switch( _type )
			{
				case ETransform.t_hflip: 	{ hflip(); } 	break;
				case ETransform.t_vflip:	{ vflip(); } 	break;
				case ETransform.t_rotate: 	{ rot_cw(); } 	break;
			}
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( this.m_data, 0, utils.CONST_CHR_TOTAL_PIXELS_CNT );
		}
		
		public void load( BinaryReader _br )
		{
#if DEF_NES
			for( int i = 0; i < utils.CONST_CHR_TOTAL_PIXELS_CNT; i++ )
			{
				this.m_data[ i ] = ( byte )( _br.ReadByte() & 0x03 );
			}
#elif DEF_SMS			
			_br.Read( this.m_data, 0, utils.CONST_CHR_TOTAL_PIXELS_CNT );
#endif
		}
		
		public bool equals( CHR_data _obj )
		{
			for( int i = 0; i < utils.CONST_CHR_TOTAL_PIXELS_CNT; i++ )
			{
				if( get_data()[ i ] != _obj.get_data()[ i ] )
				{
					return false;
				}
			}
			
			return true;
		}
	}
}
