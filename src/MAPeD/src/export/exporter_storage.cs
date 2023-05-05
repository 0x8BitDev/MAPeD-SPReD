/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 05.05.2023
 * Time: 10:58
 */
using System;
using System.IO;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_storage.
	/// </summary>
	public class exporter_storage
	{
		private exporter_data_holder< byte >	m_data8		= new exporter_data_holder< byte >();
		private exporter_data_holder< ushort >	m_data16	= new exporter_data_holder< ushort >();
		
		public exporter_data_holder< byte > data8
		{
			get { return m_data8; }
		}

		public exporter_data_holder< ushort > data16
		{
			get { return m_data16; }
		}
		
		public exporter_storage()
		{
			//...
		}
		
		public void reset()
		{
			m_data8.reset();
			m_data16.reset();
		}
	}

	public class exporter_data_holder <T>
	{
		private readonly Dictionary< string, List< data_desc > >	m_storage;
		
		private struct data_desc
		{
			public T[]		m_data;

			public string	m_comment;
			public string	m_compressor_name;
			
			public int		m_uncompressed_size;
			
			public data_desc( T[] _data, string _comment, string _compressor_name, int _uncompressed_size )
			{
				m_data				= _data;
				
				m_comment			= _comment;
				m_compressor_name	= _compressor_name;
				
				m_uncompressed_size	= _uncompressed_size;
			}
			
			public void reset()
			{
				m_data				= null;
				
				m_comment			= null;
				m_compressor_name	= null;
				
				m_uncompressed_size	= -1;
			}
		}
		
		public exporter_data_holder()
		{
			m_storage = new Dictionary< string, List< data_desc > >( 10 );
		}
		
		public void reset()
		{
			foreach( var item in m_storage )
			{
				foreach( var desc in item.Value )
				{
					desc.reset();
				}
				item.Value.Clear();
			}
			
			m_storage.Clear();
		}
		
		public void add_data( string _data_name, T[] _data, string _comment, string _compressor_name, int _uncompressed_size )
		{
			data_desc desc = new exporter_data_holder< T >.data_desc( _data, _comment, _compressor_name, _uncompressed_size );
			
			if( !m_storage.ContainsKey( _data_name ) )
			{
				m_storage.Add( _data_name, new List< data_desc >() );
			}
			
			m_storage[ _data_name ].Add( desc );
		}
		
		private void write_data_arr( BinaryWriter _bw, T[] _data_arr )
		{
			Type value_type = typeof( T );
			
			switch( Type.GetTypeCode( value_type ) )
			{
				case TypeCode.Byte:
					{
						for( int i = 0; i < _data_arr.Length; i++ )
						{
							_bw.Write( ( byte )( object )_data_arr[ i ] );
						}
					}
					break;

				case TypeCode.UInt16:
					{
						for( int i = 0; i < _data_arr.Length; i++ )
						{
							_bw.Write( ( ushort )( object )_data_arr[ i ] );
						}
					}
					break;
					
				default:
					{
						throw new Exception( "[exporter_data_holder.write_data_arr] Unsupported type: " + value_type.Name );
					}
			}
		}
		
		private string build_comment( data_desc _desc )
		{
			return ( _desc.m_compressor_name != null ? ( _desc.m_compressor_name + " (" + _desc.m_data.Length + "/" + _desc.m_uncompressed_size + ") " ):"" ) + ( _desc.m_comment != null ? _desc.m_comment:"" );
		}
		
		public void save_data( string _data_name, StreamWriter _asmw, StreamWriter _cw, exporter_config _config, Action< StreamWriter, string > _write_offset_act )
		{
			if( m_storage.ContainsKey( _data_name ) )
			{
				// write data align
				_asmw.WriteLine( "\t" + _config.data_align_alias );
				
				_asmw.WriteLine( _config.get_data_prefix() + _data_name + ":" );
				
				T[] data = null;
				
				List< data_desc > data_desc_arr = m_storage[ _data_name ];

				string label_name = _config.get_exp_prefix() + _config.get_data_prefix() + _data_name; 
				string filename;

				if( _cw != null )
				{
					_cw.WriteLine( "extern " + _config.C_byte_type_alias + "*\t" + exporter_config.skip_exp_prefix( _config.get_data_prefix() ) + _data_name + ";" );
					
					if( data_desc_arr.Count > 1 )
					{
						_cw.WriteLine( "extern " + _config.C_word_type_alias + "*\t" + exporter_config.skip_exp_prefix( _config.get_data_prefix() ) + _data_name + _config.data_offset_alias + ";" );
					}
				}
				
				if( _write_offset_act == null )
				{
					// merge all data arrays into single binary file
					int offset = 0;
					
					string offsets_txt = "";
					string comment;
					
					filename = _config.filename + "_" + _data_name + exporter_config.CONST_BIN_EXT;
					
					using( BinaryWriter bw = new BinaryWriter( File.Open( _config.path + Path.DirectorySeparatorChar + filename, FileMode.Create ) ) )
					{
						for( int i = 0; i < data_desc_arr.Count; i++ )
						{
							data = data_desc_arr[ i ].m_data;
							write_data_arr( bw, data );
			
							if( data_desc_arr.Count > 1 )
							{
								offsets_txt += "\t" + _config.word_alias + " " + offset + "\n";
							
								offset += data.Length;
							}
						}

					
						if( data_desc_arr.Count > 1 )
						{
							comment = "(" + offset + ")";
						}
						else
						{
							comment = build_comment( data_desc_arr[ 0 ] );
						}
						
						// write .INCBIN
						_asmw.WriteLine( label_name + ":\t.incbin\"" + filename + "\"\t" + _config.comment_token + " " + comment + "\n" );
						
						if( data_desc_arr.Count > 1 )
						{
							offsets_txt += "\t" + _config.word_alias + " " + offset + "\t" + _config.comment_token + " data end\n";

							// write data align
							_asmw.WriteLine( "\t" + _config.data_align_alias );
							
							// write offsets
							_asmw.WriteLine( label_name + _config.data_offset_alias + ":\n" + offsets_txt );
						}
					}
				}
				else
				{
					data_desc desc;
					
					// write multiple binary files with user-defined data offsets
					for( int i = 0; i < data_desc_arr.Count; i++ )
					{
						desc = data_desc_arr[ i ];
						data = desc.m_data;
						
						filename = _config.filename + "_" + _data_name + i + exporter_config.CONST_BIN_EXT;
						
						using( BinaryWriter bw = new BinaryWriter( File.Open( _config.path + Path.DirectorySeparatorChar + filename, FileMode.Create ) ) )
						{
							write_data_arr( bw, data );
						}
						
						// write data align
						_asmw.WriteLine( "\t" + _config.data_align_alias );
						
						// write .INCBIN
						_asmw.WriteLine( label_name + i + ":\t.incbin\"" + filename + "\"\t" + _config.comment_token + " " + build_comment( desc ) );
					}

					// write data align
					_asmw.WriteLine( "\t" + _config.data_align_alias );
					
					// write offsets
					_asmw.WriteLine( label_name + _config.data_offset_alias + ":" );
					
					for( int i = 0; i < data_desc_arr.Count; i++ )
					{
						_write_offset_act( _asmw, label_name + i );
					}
				}
			}
			else
			{
				throw new Exception( "[exporter_storage.save_storage] Unknown storage name: " + _data_name );
			}
		}
	}
}
