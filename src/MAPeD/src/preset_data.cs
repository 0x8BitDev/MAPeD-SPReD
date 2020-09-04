/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 04.09.2020
 * Time: 14:06
 */
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MAPeD
{
	/// <summary>
	/// Description of preset_data.
	/// </summary>
	///
	
	[DataContract]
	public class preset_data
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
		
		public preset_data( string _name, byte _width, byte _height, byte[] _data )
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
	}
}
