/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 22.03.2017
 * Time: 10:35
 */
using System;
using System.IO;

namespace SPReD
{
	/// <summary>
	/// Description of CHR_data_attr.
	/// </summary>
	public class CHR_data_attr
	{
		private int		m_x 			= 0;
		private int		m_y 			= 0;
		private int		m_palette_ind 	= -1;	// ignored on SMS
		private int		m_CHR_ind		= -1;
		private int		m_flip_flag		= 0;	// ignored on SMS
		
		public int flip_flag
		{
			get { return m_flip_flag; }
			set {}
		}
		
		public const int CONST_CHR_ATTR_FLAG_HFLIP		= 0x01;
		public const int CONST_CHR_ATTR_FLAG_VFLIP		= 0x02;
		public const int CONST_CHR_ATTR_FLAG_FLIP_MASK	= 0x03;
		
		public int CHR_ind
		{
			get { return m_CHR_ind; }
			set { m_CHR_ind = value; }
		}
		
		public int x
		{
			get { return m_x; }
			set { m_x = value; }
		}
		
		public int y
		{
			get { return m_y; }
			set { m_y = value; }
		}
		
		public int palette_ind
		{
			get { return m_palette_ind; }
			set { m_palette_ind = value; }
		}
		
		public CHR_data_attr( int _x, int _y )
		{
			x = _x;
			y = _y;
		}
		
		public CHR_data_attr copy()
		{
			CHR_data_attr attr = new CHR_data_attr( x, y );
			
			attr.m_flip_flag 	= flip_flag;
			attr.palette_ind 	= palette_ind;
			attr.CHR_ind		= CHR_ind;
			
			return attr;
		}
		
		public void hflip()
		{
			m_flip_flag ^= CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP;
		}
		
		public void vflip()
		{
			m_flip_flag ^= CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP;
		}
		
		public void reset_flip_flag()
		{
			m_flip_flag = 0;
		}

		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_x 				);
			_bw.Write( m_y 				);
			_bw.Write( m_palette_ind 	);
			_bw.Write( m_CHR_ind		);
			_bw.Write( m_flip_flag		);
		}
		
		public void load( BinaryReader _br )
		{
			m_x 			= _br.ReadInt32();
			m_y 			= _br.ReadInt32();
			m_palette_ind 	= _br.ReadInt32();
			m_CHR_ind		= _br.ReadInt32();
			m_flip_flag		= _br.ReadInt32();
		}
	}
}
