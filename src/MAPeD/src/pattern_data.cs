/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 04.09.2020
 * Time: 14:06
 */
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace MAPeD
{
	/// <summary>
	/// Description of pattern_data.
	/// </summary>
	///
	
	[DataContract]
	public class pattern_data
	{
		private string	m_name = null;
		
		private byte m_width	= 0xff;
		private byte m_height	= 0xff;
		
		private byte[]	m_data = null;
		
		[DataMember]
		public string name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		[DataMember]
		public byte width
		{
			get { return m_width; }
			set { m_width = value; }
		}

		[DataMember]
		public byte height
		{
			get { return m_height; }
			set { m_height = value; }
		}
		
		[DataMember]
		public byte[] data
		{
			get { return m_data; }
			set {}
		}
		
		public pattern_data( string _name, byte _width, byte _height, byte[] _data )
		{
			m_name		= _name;
			m_width		= _width;
			m_height	= _height;
			m_data		= _data;
		}
		
		public void reset()
		{
			m_name		= null;
			m_data		= null;
		}
		
		public pattern_data copy()
		{
			byte[] arr = null;
			
			if( data != null )
			{
				arr = new byte[ data.Length ];
				
				Array.Copy( data, arr, data.Length );
			}
			
			return new pattern_data( name, width, height, arr );
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_name );
			_bw.Write( m_width );
			_bw.Write( m_height );
			
			_bw.Write( m_data, 0, m_width * m_height );
		}
		
		public static pattern_data load( byte _ver, BinaryReader _br )
		{
			string name		= _br.ReadString();
			byte width		= _br.ReadByte();
			byte height		= _br.ReadByte();

			return new pattern_data( name, width, height, _br.ReadBytes( width * height ) );
		}
	}
}
