/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 04.09.2020
 * Time: 14:06
 */
using System;
using System.IO;

namespace MAPeD
{
	/// <summary>
	/// Description of pattern_data.
	/// </summary>
	///
	
	public class pattern_data
	{
		private string	m_name = null;
		
		private screen_data	m_data = null;
		
		public string name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		public int width
		{
			get { return m_data.width; }
		}

		public int height
		{
			get { return m_data.height; }
		}
		
		public screen_data data
		{
			get { return m_data; }
		}
		
		public pattern_data( string _name, screen_data _data )
		{
			m_name		= _name;
			m_data		= _data;
		}
		
		public void reset()
		{
			m_name		= null;
			
			if( m_data != null )
			{
				m_data.reset();
				m_data = null;
			}
		}
		
		public pattern_data copy()
		{
			if( data != null )
			{
				return new pattern_data( name, m_data.copy() );
			}
			
			throw new Exception( "pattern_data.copy() error! Can't copy an empty data!" );
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_name );
			
			m_data.save( _bw, true );
		}
		
		public static pattern_data load( byte _ver, BinaryReader _br )
		{
			string name	= _br.ReadString();

			return new pattern_data( name, screen_data.load( _br, _ver ) );
		}
	}
}
