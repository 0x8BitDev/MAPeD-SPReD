/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
	public class py_api : global::SPSeD.i_py_api
	{
		private data_sets_manager	m_data_mngr = null;
		
		public const string CONST_PREFIX	= "mpd_"; 
		
		public py_api( data_sets_manager _data_mngr ) : base()
		{
			m_data_mngr = _data_mngr;			
		}

		public string get_prefix()
		{
			return CONST_PREFIX;
		}
		
		public void init( ScriptScope _py_scope )
		{
			// Bank Data

			// ushort api_ver( void )
			_py_scope.SetVariable( CONST_PREFIX + "api_ver", new Func< ushort >( api_ver ) );
			
			// int num_banks( void )
			_py_scope.SetVariable( CONST_PREFIX + "num_banks", new Func< int >( num_banks ) );
			
			// int get_active_bank( void )
			_py_scope.SetVariable( CONST_PREFIX + "get_active_bank", new Func< int >( get_active_bank ) );

			// int get_tiles_cnt( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_tiles_cnt", new Func< int, int >( get_tiles_cnt ) );

			// int get_blocks_cnt( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_blocks_cnt", new Func< int, int >( get_blocks_cnt ) );
			
			// int get_CHRs_cnt( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_CHRs_cnt", new Func< int, int >( get_CHRs_cnt ) );

			// uint[] get_tiles( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_tiles", new Func< int, uint[] >( get_tiles ) );
			
			// uint[] get_blocks( int _bank_ind ) [ 15-8 -> obj_id(4)|plt(2)|not used(2)| 7-0 -> CHR ind(8) ]
			_py_scope.SetVariable( CONST_PREFIX + "get_blocks", new Func< int, uint[] >( get_blocks ) );
			
			// byte[] get_CHRs_data( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_CHRs_data", new Func< int, byte[] >( get_CHRs_data ) );

			// byte[] get_CHR( int _bank_ind, int _CHR_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_CHR", new Func< int, int, byte[] >( get_CHR ) );
			
			// NES:		long export_CHR_data( int _bank_ind, string _filename, bool _save_padding )
			// SMS:		long export_CHR_data( int _bank_ind, string _filename, int _bpp )
			// PCE/ZX:	long export_CHR_data( int _bank_ind, string _filename )
#if DEF_NES			
			_py_scope.SetVariable( CONST_PREFIX + "export_CHR_data", new Func< int, string, bool, long >( export_CHR_data ) );
#elif DEF_SMS
			_py_scope.SetVariable( CONST_PREFIX + "export_CHR_data", new Func< int, string, int, long >( export_CHR_data ) );
#elif DEF_PCE || DEF_ZX
			_py_scope.SetVariable( CONST_PREFIX + "export_CHR_data", new Func< int, string, long >( export_CHR_data ) );
#endif			
			// int get_palette_slots( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_palette_slots", new Func< int, int >( get_palette_slots ) );
	
			// int[] get_palette( int _bank_ind, int _plt_ind, int _slot_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_palette", new Func< int, int, int, int[] >( get_palette ) );

			// int num_screens( int _bank_ind )
			_py_scope.SetVariable( CONST_PREFIX + "num_screens", new Func< int, int >( num_screens ) );

			// int screen_mode( void )
			_py_scope.SetVariable( CONST_PREFIX + "screen_mode", new Func< int >( screen_mode ) );
			
			// ushort[] get_screen_data( int _bank_ind, int _scr_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_screen_data", new Func< int, int, ushort[] >( get_screen_data ) );
			
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
			_py_scope.SetVariable( CONST_PREFIX + "layout_screen_ind", new Func< int, int, int, int >( layout_screen_ind ) );
			
			// ushort layout_screen_marks( int _layout_ind, int _scr_pos_x, int_scr_pos_y )
			_py_scope.SetVariable( CONST_PREFIX + "layout_screen_marks", new Func< int, int, int, ushort >( layout_screen_marks ) );
			
			// Entity Instances
			
			_py_scope.SetVariable( CONST_PREFIX + "base_entity", typeof( mpd_base_entity ) );
			_py_scope.SetVariable( CONST_PREFIX + "inst_entity", typeof( mpd_inst_entity ) );

			// int layout_screen_num_entities( int _layout_ind, int _scr_pos_x, int_scr_pos_y )
			_py_scope.SetVariable( CONST_PREFIX + "layout_screen_num_entities", new Func< int, int, int, int >( layout_screen_num_entities ) );
			
			// inst_entity layout_get_inst_entity( int _layout_ind, int _scr_pos_x, int_scr_pos_y, int _ent_ind )
			_py_scope.SetVariable( CONST_PREFIX + "layout_get_inst_entity", new Func< int, int, int, int, mpd_inst_entity >( layout_get_inst_entity ) );
			
			// Base Entities
			
			// string[] groups_names_of_entities()
			_py_scope.SetVariable( CONST_PREFIX + "group_names_of_entities", new Func< string[] >( group_names_of_entities ) );
			
			// int group_num_entities( string _group_name )
			_py_scope.SetVariable( CONST_PREFIX + "group_num_entities", new Func< string, int >( group_num_entities ) );
			
			// base_entity group_get_entity_by_ind( string _group_name, int _ent_ind )
			_py_scope.SetVariable( CONST_PREFIX + "group_get_entity_by_ind", new Func< string, int, mpd_base_entity >( group_get_entity_by_ind ) );
			
			// base_entity get_base_entity_by_name( string _entity_name )
			_py_scope.SetVariable( CONST_PREFIX + "get_base_entity_by_name", new Func< string, mpd_base_entity >( get_base_entity_by_name ) );
			
			// Miscellaneous
			
			// byte[] RLE( byte[] _arr )
			_py_scope.SetVariable( CONST_PREFIX + "RLE", new Func< byte[], byte[] >( RLE ) );
		}

		public void deinit()
		{
			m_data_mngr = null;
		}

		public ushort api_ver()
		{
			return 0x0106;
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
				return data.screen_data_cnt();
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

		private layout_screen_data get_layout_screen_data( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			if( _layout_ind >= 0 && _layout_ind < m_data_mngr.layouts_data_cnt )
			{
				layout_data data = m_data_mngr.get_layout_data( _layout_ind );
				
				return data.get_data( _scr_pos_x, _scr_pos_y );
			}
			
			return null;
		}
		
		public int layout_screen_ind( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			layout_screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null )
			{
				return ( int )data.m_scr_ind;
			}
			
			return -1;
		}

		public ushort layout_screen_marks( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			layout_screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null )
			{
				return data.mark_adj_scr_mask;
			}
			
			return ushort.MaxValue;
		}

		public int layout_screen_num_entities( int _layout_ind, int _scr_pos_x, int _scr_pos_y )
		{
			layout_screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null )
			{
				return data.m_ents.Count;
			}
			
			return -1;
		}

		public mpd_inst_entity layout_get_inst_entity( int _layout_ind, int _scr_pos_x, int _scr_pos_y, int _ent_ind )		
		{
			layout_screen_data data = get_layout_screen_data( _layout_ind, _scr_pos_x, _scr_pos_y );
			
			if( data != null && _ent_ind >= 0 && _ent_ind < data.m_ents.Count )
			{
				entity_instance ent = data.m_ents[ _ent_ind ];
				
				mpd_inst_entity inst_ent = new mpd_inst_entity();
				
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
		
		public mpd_base_entity group_get_entity_by_ind( string _group_name, int _ent_ind )
		{
			if( m_data_mngr.entities_data.ContainsKey( _group_name ) )
			{
				if( _ent_ind >= 0 && _ent_ind < m_data_mngr.entities_data[ _group_name ].Count )
				{
					return fill_mpd_base_entity( m_data_mngr.entities_data[ _group_name ][ _ent_ind ] );
				}
			}
			
			return null;
		}
		
		public mpd_base_entity get_base_entity_by_name( string _name )
		{
			entity_data ent = m_data_mngr.get_entity_by_name( _name );
			
			if( ent != null )
			{
				return fill_mpd_base_entity( ent );
			}
			
			return null;
		}

		private mpd_base_entity fill_mpd_base_entity( entity_data _ent )
		{
			mpd_base_entity base_ent = new mpd_base_entity();
			
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

		public int get_tiles_cnt( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				int cnt = data.get_first_free_tile_id();
				
				return cnt >= 0 ? cnt:utils.CONST_MAX_TILES_CNT;
			}
			
			return -1;
		}

		public int get_blocks_cnt( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				int cnt = data.get_first_free_block_id();
				
				return cnt >= 0 ? cnt:utils.CONST_MAX_BLOCKS_CNT;
			}
			
			return -1;
		}
			
		public int get_CHRs_cnt( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				int cnt = data.get_first_free_spr8x8_id();
				
				return cnt >= 0 ? cnt:utils.CONST_CHR_BANK_MAX_SPRITES_CNT;
			}
			
			return -1;
		}

		public byte[] get_CHR( int _bank_ind, int _CHR_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				if( _CHR_ind >= 0 && _CHR_ind < utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
				{
					byte[] CHR_data = new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
					
					data.from_CHR_bank_to_spr8x8( _CHR_ind, CHR_data, 0 );
					
					return CHR_data;
				}
				else
				{
					throw new Exception( CONST_PREFIX + "get_CHRs_cnt error! Invalid argument: _CHR_ind!\n" );
				}
			}
			
			return null;
		}
		
		public uint[] get_tiles( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return copy_arr< uint >( data.tiles );
			}
			
			return null;
		}

		public uint[] get_blocks( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return copy_arr< uint >( data.blocks );
			}
			
			return null;
		}
		
		public byte[] get_CHRs_data( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return copy_arr< byte >( data.CHR_bank );
			}
			
			return null;
		}
		
#if DEF_NES
		public long export_CHR_data( int _bank_ind, string _filename, bool _save_padding )
#elif DEF_SMS || DEF_SMD
		public long export_CHR_data( int _bank_ind, string _filename, int _bpp )
#elif DEF_PCE || DEF_ZX
		public long export_CHR_data( int _bank_ind, string _filename )
#endif			
		{
			long data_size = -1;
			
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			BinaryWriter bw = null;
			
			try
			{
				if( data != null )
				{
					bw = new BinaryWriter( File.Open( _filename, FileMode.Create ) );
#if DEF_NES					
					data_size = data.export_CHR( bw, _save_padding );
#elif DEF_SMS
					if( _bpp < 1 || _bpp > 4 )
					{
						throw new Exception( "Invalid CHRs bpp value! The valid range is 1-4." );
					}
					
					data_size = data.export_CHR( bw, _bpp );
#elif DEF_PCE || DEF_ZX || DEF_SMD
					data_size = data.export_CHR( bw );
#endif
				}
				else
				{
					throw new Exception( "Invalid bank index ( " + _bank_ind + " )! Use " + CONST_PREFIX + "num_banks() to get a valid range!" );
				}
			}
			catch( Exception _err )
			{
				throw new Exception( CONST_PREFIX + "export_CHR_data error! Can't save CHR data!\n" + _err.Message );
			}
			
			finally
			{
				if( bw != null )
				{
					bw.Close();
				}
			}
			
			return data_size;
		}

		public int get_palette_slots( int _bank_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				return data.palettes_arr.Count; 
			}
			
			return -1;
		}
		
		public int[] get_palette( int _bank_ind, int _plt_ind, int _slot_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				if( _plt_ind >= 0 && _plt_ind < utils.CONST_NUM_SMALL_PALETTES )
				{
					if( _slot_ind >= 0 && _slot_ind < data.palettes_arr.Count )
					{
						int[] plt = null; 
						
						switch( _plt_ind )
						{
							case 0: { plt = data.palettes_arr[ _slot_ind ].m_palette0; } break;
							case 1: { plt = data.palettes_arr[ _slot_ind ].m_palette1; } break;
							case 2: { plt = data.palettes_arr[ _slot_ind ].m_palette2; } break;
							case 3: { plt = data.palettes_arr[ _slot_ind ].m_palette3; } break;
						}
						
						return copy_arr< int >( plt );
					}
					else
						throw new Exception( CONST_PREFIX + "get_palette error! Invalid argument: _slot_ind!\n" );
				}
				else
					throw new Exception( CONST_PREFIX + "get_palette error! Invalid argument: _plt_ind!\n" );
			}
				
			return null;
		}

		public int screen_mode()
		{
			return ( int )m_data_mngr.screen_data_type;
		}
		
		public ushort[] get_screen_data( int _bank_ind, int _scr_ind )
		{
			tiles_data data = m_data_mngr.get_tiles_data( _bank_ind );
			
			if( data != null )
			{
				if( _scr_ind >= 0 && _scr_ind < data.screen_data_cnt() )
				{
					return copy_arr< ushort >( data.get_screen_data( _scr_ind ).m_arr );
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
		
		private T[] copy_arr<T>( T[] _arr )
		{
			T[] out_arr = new T[ _arr.Length ];
			Array.Copy( _arr, out_arr, _arr.Length );
			
			return out_arr;
		}
	}

	public class mpd_base_entity
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
	
	public class mpd_inst_entity
	{
		public int uid			= -1;
		public int target_uid	= -1;

		public int x = 0;
		public int y = 0;
		
		public int base_ent_uid = 0;

		public string properties	= "";
	}
}
