/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 05.05.2023
 * Time: 11:00
 */
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_statistics_form.
	/// </summary>
	public partial class exporter_statistics_form : Form
	{
		private readonly Dictionary< string, compressor_info >	m_compressors_stats;

		private class compressor_info
		{
			public int		m_uncompressed_size;
			public int		m_compressed_size;
			
			public compressor_info()
			{
				m_uncompressed_size	= 0;
				m_compressed_size	= 0;
			}
		};
		
		private int m_bin_data_length	= 0;
		private int m_layouts_cnt		= 0;
		private int m_banks_cnt			= 0;
		
		public exporter_statistics_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_compressors_stats = new Dictionary< string, compressor_info >();
		}
	
		public void reset()
		{
			m_bin_data_length	= 0;
			m_layouts_cnt		= 0;
			m_banks_cnt			= 0;
			
			m_compressors_stats.Clear();
		}
		
		public void add_compressor_stats( string _name, int _uncompressed_size, int _compressed_size )
		{
			if( !m_compressors_stats.ContainsKey( _name ) )
			{
				m_compressors_stats.Add( _name, new compressor_info() );
			}
			
			m_compressors_stats[ _name ].m_compressed_size		+= _compressed_size;
			m_compressors_stats[ _name ].m_uncompressed_size	+= _uncompressed_size;
		}

		public void inc_bin_data( int _val )
		{
			m_bin_data_length += _val; 
		}

		public void inc_layouts_cnt( int _val )
		{
			m_layouts_cnt += _val;
		}

		public void inc_banks_cnt( int _val )
		{
			m_banks_cnt += _val;
		}
	}
}
