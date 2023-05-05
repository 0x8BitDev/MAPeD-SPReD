/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 05.05.2023
 * Time: 10:58
 */
using System;
using System.IO;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_data_builder_base.
	/// </summary>
	public abstract class exporter_data_builder_base
	{
		protected readonly string				m_name;
		
		protected readonly exporter_statistics_form	m_stats;	// for stats accumulation
		protected readonly exporter_config			m_config;	// stores export options
		protected readonly exporter_storage			m_storage;	// main data storage; the object doesn't hold any data
		
		protected readonly data_sets_manager	m_data_mngr;
		
		protected i_compressor	m_compressor		= null;
		protected bool m_compressor_save_ctrl_val	= false;

		protected Func< ushort, ushort >	m_data_converter16	= null;
		
		public Func< ushort, ushort > data_converter16
		{
			set { m_data_converter16 = value; }
		}		
		
		public exporter_data_builder_base( string _name, data_sets_manager _data_mngr, exporter_statistics_form _stats, exporter_config _config, exporter_storage _storage )
		{
			m_name		= _name;
			
			m_stats		= _stats;
			m_config	= _config;
			m_storage	= _storage;
			
			m_data_mngr	= _data_mngr;
		}

		public virtual void reset()
		{
			m_data_converter16	= null;
			m_compressor		= null;
		}
		
		public void set_compressor( i_compressor _compressor, bool _save_ctrl_val )
		{
			m_compressor = _compressor;
			m_compressor_save_ctrl_val = _save_ctrl_val;
		}
		
		public abstract void export(	tiles_data	_data,
										string		_data_name );	// null - m_name will be used
	}
	
	public abstract class exporter_data_builder<T> : exporter_data_builder_base
	{
		public exporter_data_builder( string _name, data_sets_manager _data_mngr, exporter_statistics_form _stats, exporter_config _config, exporter_storage _storage ) : base( _name, _data_mngr, _stats, _config, _storage )
		{
			//...
		}
		
		private void check_and_save_ctrl_val( ref T[] _arr, int val )
		{
			if( m_compressor_save_ctrl_val )
			{
				T[] new_arr = new T[ _arr.Length + 1 ];
				
				new_arr[ 0 ] = (T)Convert.ChangeType( val, typeof(T) );
				
				Array.Copy( _arr, 0, new_arr, 1, _arr.Length );
				
				_arr = new_arr;
			}
		}
		
		private void update_stats( int _uncompressed_size, int _compressed_size )
		{
			if( m_compressor != null && _compressed_size > 0 )
			{
				m_stats.add_compressor_stats( m_compressor.name(), _uncompressed_size, _compressed_size );
			}
			
			m_stats.inc_bin_data( _uncompressed_size );
		}
		
		protected abstract T[] build_data_arr( tiles_data _data, ref string _comment );

		public override void export(	tiles_data	_data,
		                            	string		_data_name )	// null - m_name will be used
		{
			string data_name = ( _data_name != null ) ? _data_name:m_name;
			string comment = "";

			T[] data_arr = build_data_arr( _data, ref comment );

			Type value_type = typeof( T );
			
			switch( Type.GetTypeCode( value_type ) )
			{
				case TypeCode.Byte:
					{
						byte[] u8_data_arr = ( byte[] )( object )data_arr;
							
						if( m_compressor != null )
						{
							byte[] encoded_data = null;

							if( m_compressor.encode8( u8_data_arr, ref encoded_data ) > 0 )
							{
								check_and_save_ctrl_val( ref data_arr, 1 );
								
								m_storage.data8.add_data( data_name, encoded_data, comment, m_compressor.name(), data_arr.Length );
								
								update_stats( data_arr.Length, u8_data_arr.Length * sizeof( ushort ) );
							}
							else
							{
								check_and_save_ctrl_val( ref data_arr, 0 );
								
								m_storage.data8.add_data( data_name, u8_data_arr, comment, m_compressor.name(), data_arr.Length );
								
								update_stats( data_arr.Length, u8_data_arr.Length * sizeof( ushort ) );
							}
						}
						else
						{
							m_storage.data8.add_data( data_name, u8_data_arr, comment, null, data_arr.Length );
							
							update_stats( data_arr.Length, -1 );
						}
					}
					break;

				case TypeCode.UInt16:
					{
						ushort[] u16_data_arr = ( ushort[] )( object )data_arr;
						
						if( m_compressor != null )
						{
							ushort[] encoded_data = null;
							
							if( m_compressor.encode16( u16_data_arr, ref encoded_data ) > 0 )
							{
								check_and_save_ctrl_val( ref data_arr, 1 );
								
								m_storage.data16.add_data( data_name, encoded_data, comment, m_compressor.name(), data_arr.Length );
								
								update_stats( data_arr.Length, u16_data_arr.Length * sizeof( ushort ) );
							}
							else
							{
								check_and_save_ctrl_val( ref data_arr, 0 );
								
								m_storage.data16.add_data( data_name, u16_data_arr, comment, m_compressor.name(), data_arr.Length );
								
								update_stats( data_arr.Length, u16_data_arr.Length * sizeof( ushort ) );
							}
						}
						else
						{
							m_storage.data16.add_data( data_name, u16_data_arr, comment, null, data_arr.Length );
							
							update_stats( data_arr.Length, -1 );
						}
					}
					break;
			}
		}
		
		public virtual void export(	layout_data		_data,
									StreamWriter	_asmw,
									StreamWriter	_cw )
		{
			throw new Exception( "[exporter_data_builder<T>.export(layout, ...)] Not implemented!" );
		}
	}
	
	public class exporter_data_TEST8 : exporter_data_builder< byte >
	{
		public exporter_data_TEST8( string _name, data_sets_manager _data_mngr, exporter_statistics_form _stats, exporter_config _config, exporter_storage _storage ) : base( _name, _data_mngr, _stats, _config, _storage )
		{
			//...
		}
		
		protected override byte[] build_data_arr( tiles_data _data, ref string _comment )
		{
			byte[] data_arr = new byte[ 768 ];
			
			for( int i = 0; i < data_arr.Length; i += 32 )
			{
				data_arr[ i ] = ( byte )i;
			}
			
			_comment = "*** TEST-8 COMMENT! ***";
			
			return data_arr;
		}
	}

	public class exporter_data_TEST16 : exporter_data_builder< ushort >
	{
		public exporter_data_TEST16( string _name, data_sets_manager _data_mngr, exporter_statistics_form _stats, exporter_config _config, exporter_storage _storage ) : base( _name, _data_mngr, _stats, _config, _storage )
		{
			//...
		}
		
		protected override ushort[] build_data_arr( tiles_data _data, ref string _comment )
		{
			ushort[] data_arr = new ushort[ 768 ];
			
			for( int i = 0; i < data_arr.Length; i += 32 )
			{
				data_arr[ i ] = ( ushort )i;
			}
			
			_comment = "*** TEST-16 COMMENT! ***";
			
			return data_arr;
		}
	}
}
