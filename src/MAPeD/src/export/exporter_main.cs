/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 30.04.2023
 * Time: 12:43
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_main.
	/// </summary>
	public class exporter_main
	{
		private readonly Dictionary< int, exporter_data_chunk_base >	m_data_chunks_exporters;
		
		public const int	EXP_BLOCKS8		= 0x01;
		public const int	EXP_BLOCKS16	= 0x02;
		public const int	EXP_TILES		= 0x03;
		public const int	EXP_PALETTE		= 0x04;
		public const int	EXP_LAYOUT		= 0x05;
		public const int	EXP_TILEMAP		= 0x06;
		public const int	EXP_MAX			= 0x06; 
		
		private exporter_config m_config;
		
		public exporter_config config
		{
			set {}
			get { return m_config; }
		}
		
		private exporter_stats	m_stats;
		
		public exporter_main( data_sets_manager _data_mngr )
		{
			m_config = new exporter_config();
			
			m_data_chunks_exporters = new Dictionary< int, exporter_data_chunk_base >( EXP_MAX );
			
			// register( EXP_BLOCKS8, ... );
			// register( EXP_BLOCKS16, ... );
			// register( EXP_TILES, ... );
			// register( EXP_PALETTE, ... );
			// register( EXP_LAYOUT, ... );
			// register( EXP_TILEMAP, ... );
		}
	}
	
	public class exporter_config
	{
		//...
		
		public exporter_config()
		{
			//...
		}
	}

	public class exporter_stats
	{
		public int bin_data_length = 0;
		
		// compression data stats
		//...
		
		public exporter_stats()
		{
			//...
		}
	}
	
	public abstract class exporter_data_chunk_base
	{
		protected readonly string				m_name;
		
		protected readonly exporter_stats		m_stats;
		protected readonly exporter_config		m_config;
		
		protected readonly data_sets_manager	m_data_mngr;
		
		private i_compressor	m_compressor;

		public i_compressor compressor
		{
			set { m_compressor = value; }
		}
		
		private Func< ushort, ushort >	m_data_converter16;
		
		public Func< ushort, ushort > data_converter16
		{
			set { m_data_converter16 = value; }
		}		
		
		public exporter_data_chunk_base( string _name, data_sets_manager _data_mngr, exporter_stats _stats, exporter_config _config )
		{
			m_name		= _name;
			
			m_stats		= _stats;
			m_config	= _config;
			
			m_data_mngr	= _data_mngr;
		}
		
		public abstract void export(	tiles_data		_data,
										StreamWriter	_asmw,
										BinaryWriter	_bw,
										StreamWriter	_cw );
		
		public abstract void export(	layout_data		_data,
										StreamWriter	_asmw,
										BinaryWriter	_bw,
										StreamWriter	_cw );
	}
}
