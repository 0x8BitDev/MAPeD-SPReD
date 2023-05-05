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
		private readonly Dictionary< string, exporter_data_builder_base >	m_data_builders;
/*/!!!
		public const string	EXP_BLOCKS8		= "Blocks";//????
		public const string	EXP_BLOCKS16	= "Blocks";
		public const string	EXP_TILES		= "Tiles";
		public const string	EXP_PALETTE		= "Plt";
		public const string	EXP_LAYOUT		= "Layout";
		public const string	EXP_TILEMAP		= "Map";
//*/		
		public const string	EXP_TEST8		= "Test8";
		public const string	EXP_TEST16		= "Test16";

		private readonly exporter_config			m_config;
		private readonly exporter_storage			m_storage;
		private readonly exporter_statistics_form	m_stats;
		
		public exporter_config config
		{
			set {}
			get { return m_config; }
		}

		public exporter_storage storage
		{
			set {}
			get { return m_storage; }
		}
		
		public exporter_main( data_sets_manager _data_mngr )
		{
			m_config	= new exporter_config();
			m_storage	= new exporter_storage();
			m_stats		= new exporter_statistics_form();
			
			m_data_builders = new Dictionary< string, exporter_data_builder_base >();
			
			m_data_builders.Add( EXP_TEST8,		new exporter_data_TEST8( EXP_TEST8, _data_mngr, m_stats, m_config, m_storage ) );
			m_data_builders.Add( EXP_TEST16,	new exporter_data_TEST16( EXP_TEST16, _data_mngr, m_stats, m_config, m_storage ) );
			
			// register( EXP_BLOCKS8, ... );
			// register( EXP_BLOCKS16, ... );
			// register( EXP_TILES, ... );
			// register( EXP_PALETTE, ... );
			// register( EXP_LAYOUT, ... );
			// register( EXP_TILEMAP, ... );
		}
		
		public void reset()
		{
			m_storage.reset();
			m_stats.reset();
			
			foreach( var item in m_data_builders )
			{
				item.Value.reset();
			}
			
			m_data_builders.Clear();
		}
		
		public exporter_data_builder_base get_data_exporter( string _name )
		{
			return m_data_builders[ _name ];
		}
	}
}
