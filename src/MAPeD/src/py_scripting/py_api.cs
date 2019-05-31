/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 22.05.2019
 * Time: 10:30
 */
using System;
using System.IO;
using Microsoft.Scripting.Hosting;

namespace MAPeD
{
	/// <summary>
	/// Description of py_api.
	/// </summary>
	public class py_api
	{
		private data_sets_manager	m_data_mngr = null;
		
		public const string CONST_PREFIX	= "mpd_"; 
		
		public py_api( ScriptScope	_py_scope, data_sets_manager _data_mngr )
		{
			m_data_mngr = _data_mngr;

			// Bank Data
			
			// int num_banks( void )
			_py_scope.SetVariable( CONST_PREFIX + "num_banks", new Func< int >( num_banks ) );
			
			// int get_active_bank( void )
			_py_scope.SetVariable( CONST_PREFIX + "get_active_bank", new Func< int >( get_active_bank ) );

			// uint[] get_tiles( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_tiles", new Func< int, uint[] >( get_tiles ) );
			
			// ushort[] get_blocks( int _bank_ind ) [ 15-8 -> obj_id(4)|plt(2)|not used(2)| 7-0 -> CHR ind(8) ]
			_py_scope.SetVariable( CONST_PREFIX + "get_blocks", new Func< int, ushort[] >( get_blocks ) );
			
			// byte[] get_CHR_data( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_CHR_data", new Func< int, byte[] >( get_CHR_data ) );
			
			// long export_CHR_data( int _bank_ind, string _filename, bool _need_padding )
			_py_scope.SetVariable( CONST_PREFIX + "export_CHR_data", new Func< int, string, bool, long >( export_CHR_data ) );
			
			// byte[] get_palette( int _bank_ind, int _plt_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_palette", new Func< int, int, byte[] >( get_palette ) );

			// int num_screens( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "num_screens", new Func< int, int >( num_screens ) );
			
			// byte[] get_screen_data( int _bank_ind, int _scr_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_screen_data", new Func< int, int, byte[] >( get_screen_data ) );
			
			// Layouts Data

			// int num_layouts()
			_py_scope.SetVariable( CONST_PREFIX + "num_layouts", new Func< int >( num_layouts ) );
			
			// int layout_width( int _layout_ind )
			_py_scope.SetVariable( CONST_PREFIX + "layout_width", new Func< int, int >( layout_width ) );
			
			// int layout_height( int _layout_ind )
			_py_scope.SetVariable( CONST_PREFIX + "layout_height", new Func< int, int >( layout_height ) );
			
			// int layout_get_start_screen_ind( int _layout_ind )
			_py_scope.SetVariable( CONST_PREFIX + "layout_start_screen_cell", new Func< int, int >( layout_start_screen_cell ) );
			
			// sbyte layout_screen_ind( int _layout_ind, int _scr_pos_x, int _scr_pos_y ) [ in common screens array from all banks ]
			_py_scope.SetVariable( CONST_PREFIX + "layout_screen_ind", new Func< int, int, int, sbyte >( layout_screen_ind ) );
			
			// ushort layout_screen_marks( int _layout_ind, int _scr_pos_x, int_scr_pos_y )
			_py_scope.SetVariable( CONST_PREFIX + "layout_screen_marks", new Func< int, int, int, ushort >( layout_screen_marks ) );
			
			// Entity Instances
			
			_py_scope.SetVariable( CONST_PREFIX + "base_entity", typeof( py_base_entity ) );
			_py_scope.SetVariable( CONST_PREFIX + "inst_entity", typeof( py_inst_entity ) );

			// int layout_screen_num_entities( int _layout_ind, int _scr_pos_x, int_scr_pos_y )
			_py_scope.SetVariable( CONST_PREFIX + "layout_screen_num_entities", new Func< int, int, int, int >( layout_screen_num_entities ) );
			
			// inst_entity layout_get_inst_entity( int _layout_ind, int _scr_pos_x, int_scr_pos_y, int _ent_ind )
			_py_scope.SetVariable( CONST_PREFIX + "layout_get_inst_entity", new Func< int, int, int, int, py_inst_entity >( layout_get_inst_entity ) );
			
			// Base Entities
			
			// string[] groups_names_of_entities()
			_py_scope.SetVariable( CONST_PREFIX + "group_names_of_entities", new Func< string[] >( group_names_of_entities ) );
			
			// int group_num_entities( string _group_name )
			_py_scope.SetVariable( CONST_PREFIX + "group_num_entities", new Func< string, int >( group_num_entities ) );
			
			// base_entity group_get_entity_by_ind( string _group_name, int _ent_ind )
			_py_scope.SetVariable( CONST_PREFIX + "group_get_entity_by_ind", new Func< string, int, py_base_entity >( group_get_entity_by_ind ) );
			
			// base_entity get_base_entity_by_name( string _entity_name )
			_py_scope.SetVariable( CONST_PREFIX + "get_base_entity_by_name", new Func< string, py_base_entity >( get_base_entity_by_name ) );
			
			// Miscellaneous
			
			// byte[] RLE( byte[] _arr )
			_py_scope.SetVariable( CONST_PREFIX + "RLE", new Func< byte[], byte[] >( RLE ) );
		}
		
		public int num_banks()
		{
			return m_data_mngr.tiles_data_cnt;
		}
		
		public int get_active_bank()
		{
			return m_data_mngr.tiles_data_pos;
		}
		
		public int num_screens( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return data.scr_data.Count;
			}
			
			return -1;
		}

		public int num_layouts()
		{
			return m_data_mngr.layouts_data_cnt;
		}
		
		public int layout_width( int _layout_ind )
		{
			if( _layout_ind >= 0 && _layout_ind < m_data_mngr.layouts_data_cnt )
			{
				return m_data_mngr.get_layout_data( _layout_ind ).get_width();
			}
			
			return -1;
		}

		public int layout_height( int _layout_ind )
		{
			if( _layout_ind >= 0 && _layout_ind < m_data_mngr.layouts_data_cnt )
			{
				return m_data_mngr.get_layout_data( _layout_ind ).get_height();
			}
			
			return -1;
		}

		public int layout_start_screen_cell( int _layout_ind )
		{
			if( _layout_ind >= 0 && _layout_ind < m_data_mngr.layouts_data_cnt )
			{
				return m_data_mngr.get_layout_data( _layout_ind ).get_start_screen_ind();
			}
			
			return -1;
		}

		private screen_data get_layout_screen_data( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			if( _layout_ind >= 0 && _layout_ind < m_data_mngr.layouts_data_cnt )
			{
				layout_data data = m_data_mngr.get_layout_data( _layout_ind );
				
				return data.get_data( _scr_pos_x, _scr_pos_y );
			}
			
			return null;
		}
		
		public sbyte layout_screen_ind( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null )
			{
				return data.m_scr_ind;
			}
			
			return -1;
		}

		public ushort layout_screen_marks( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null )
			{
				return data.mark_adj_scr_mask;
			}
			
			return ushort.MaxValue;
		}

		public int layout_screen_num_entities( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null )
			{
				return data.m_ents.Count;
			}
			
			return -1;
		}

		public py_inst_entity layout_get_inst_entity( int _layout_ind, int _scr_pos_x, int _scr_pos_y, int _ent_ind )		
		{
			screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null && _ent_ind >= 0 && _ent_ind < data.m_ents.Count )
			{
				entity_instance ent = data.m_ents[ _ent_ind ];
				
				py_inst_entity inst_ent = new py_inst_entity();
				
				inst_ent.uid 			= ent.uid;
				inst_ent.base_ent_uid 	= ent.base_entity.uid;
				inst_ent.target_uid 	= ent.target_uid;
				inst_ent.x 				= ent.x;
				inst_ent.y 				= ent.y;
				inst_ent.properties 	= ent.properties;
				
				return inst_ent;
			}
			
			return null;
		}

		public string[] group_names_of_entities()
		{
			string[] names = new string[ m_data_mngr.entities_data.Keys.Count ];
			
			m_data_mngr.entities_data.Keys.CopyTo( names, 0 );
			
			return names;
		}
		
		public int group_num_entities( string _group_name )
		{
			if( m_data_mngr.entities_data.ContainsKey( _group_name ) )
			{
				return m_data_mngr.entities_data[ _group_name ].Count;
			}
			
			return -1;
		}
		
		public py_base_entity group_get_entity_by_ind( string _group_name, int _ent_ind )
		{
			if( m_data_mngr.entities_data.ContainsKey( _group_name ) )
			{
				if( _ent_ind >= 0 && _ent_ind < m_data_mngr.entities_data[ _group_name ].Count )
				{
					return fill_py_base_entity( m_data_mngr.entities_data[ _group_name ][ _ent_ind ] );
				}
			}
			
			return null;
		}
		
		public py_base_entity get_base_entity_by_name( string _name )
		{
			entity_data ent = m_data_mngr.get_entity_by_name( _name );
			
			if( ent != null )
			{
				return fill_py_base_entity( ent );
			}
			
			return null;
		}

		private py_base_entity fill_py_base_entity( entity_data _ent )
		{
			py_base_entity base_ent = new py_base_entity();
			
			base_ent.uid 				= _ent.uid;
			base_ent.width				= _ent.width;
			base_ent.height				= _ent.height;
			base_ent.pivot_x 			= _ent.pivot_x;
			base_ent.pivot_y 			= _ent.pivot_y;
			base_ent.name				= _ent.name;
			base_ent.properties 		= _ent.properties;
			base_ent.inst_properties 	= _ent.inst_properties;
			
			return base_ent;
		}
		
		public uint[] get_tiles( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return data.tiles;
			}
			
			return null;
		}

		public ushort[] get_blocks( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return data.blocks;
			}
			
			return null;
		}
		
		public byte[] get_CHR_data( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return data.CHR_bank;
			}
			
			return null;
		}
		
		public long export_CHR_data( int _bank_ind, string _filename, bool _need_padding )
		{
			long data_size = 0;
			
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				try
				{
					BinaryWriter bw = new BinaryWriter( File.Open( _filename, FileMode.Create ) );
					
					data_size = data.export_CHR( bw, _need_padding );
					
					bw.Close();
				}
				catch( Exception _err )
				{
					throw new Exception( CONST_PREFIX + "export_CHR_data error! Can't save CHR data!\n" + _err.Message );
				}
			}
			
			return data_size;
		}
		
		public byte[] get_palette( int _bank_ind, int _plt_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				switch( _plt_ind )
				{
					case 0: { return data.palette0; }
					case 1: { return data.palette1; }
					case 2: { return data.palette2; }
					case 3: { return data.palette3; }
				}
			}
				
			return null;
		}

		public byte[] get_screen_data( int _bank_ind, int _scr_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				if( _scr_ind >= 0 && _scr_ind < data.scr_data.Count )
				{
					return data.scr_data[ _scr_ind ];
				}
			}
			
			return null;
		}

		public byte[] RLE( byte[] _arr )
		{
			byte[] res_arr = null;
			
			if( utils.RLE( _arr, ref res_arr ) < 0 )
			{
				return null;
			}
			
			return res_arr;
		}
	}

	public class py_base_entity
	{
		public byte uid	= 0;
		
		public byte width	= 16;
		public byte height	= 16;
		
		public sbyte pivot_x	= 8;
		public sbyte pivot_y	= 16;
		
		public string name				= "";
		public string properties		= "";
		public string inst_properties	= "";
	}
	
	public class py_inst_entity
	{
		public int uid			= -1;
		public int target_uid	= -1;

		public int x = 0;
		public int y = 0;
		
		public int base_ent_uid = 0;

		public string properties	= "";
	}
}
