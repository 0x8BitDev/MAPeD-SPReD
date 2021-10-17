/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 12:17
 */
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MAPeD
{
	/// <summary>
	/// Description of data_sets_manager.
	/// </summary>
	/// 

	public class EventArg2Params : EventArgs
	{
		private readonly object	m_param1	= null;
		private readonly object	m_param2	= null;
		
		public object param1
		{
			get { return m_param1; }
			set {}
		}
		
		public object param2
		{
			get { return m_param2; }
			set {}
		}
		
		public EventArg2Params( object _param1, object _param2 )
		{
			m_param1 = _param1; 
			m_param2 = _param2;
		}
	}
	
	[DataContract]
	public class data_sets_manager
	{
		public delegate bool ReturnBoolEvent( object sender, EventArgs e );
		
		public event EventHandler SetLayoutData;
		public event EventHandler SetTilesData;
		public event EventHandler SetScreenData;
		public event EventHandler SetEntitiesData;
		
		public event ReturnBoolEvent AddEntity;
		public event ReturnBoolEvent DeleteEntity;
		public event ReturnBoolEvent AddGroup;
		public event ReturnBoolEvent DeleteGroup;

		[DataMember]
		private readonly string data_desc = "CHR Data Size: " + ( utils.CONST_CHR_BANK_PAGE_SIZE * platform_data.get_CHR_bank_pages_cnt() ) + " | Tiles Data Size: " + platform_data.get_max_tiles_cnt() + " | Blocks Data Size: " + platform_data.get_max_blocks_UINT_cnt() + " | Screen Data (Tiles4x4): " + utils.CONST_SCREEN_TILES_CNT + " | Screen Data (Blocks2x2): " + utils.CONST_SCREEN_BLOCKS_CNT;
		[DataMember]
		private readonly string NES_block_desc_bits = "[ property_id ](4) [ palette ind ](2) [X](2) [ CHR ind ](8)";
		[DataMember]
		private readonly string SMS_block_desc_bits = "[ property_id ](4) [ hv_flip ](2) [ palette ind ](1) [ CHR ind ](9)";
		[DataMember]
		private readonly string PCE_block_desc_bits = "[ property_id ](4) [ palette ind ](4) [ CHR ind ](12)";
		[DataMember]
		private readonly string ZX_block_desc_bits = "[ property_id ](4) [ palette ind ][1] [CHR ind](11)";
		[DataMember]
		private readonly string SMD_block_desc_bits = "[ property_id ](4) [x](1) [ palette ind ](2) [ hv_flip ](2) [CHR ind](11)";
		
		public enum EScreenDataType
		{
			sdt_Tiles4x4 = 0,
			sdt_Blocks2x2,
		};
		
		private EScreenDataType	m_screen_data_type	= EScreenDataType.sdt_Tiles4x4;
		
		[DataMember]
		public EScreenDataType screen_data_type
		{
			get { return m_screen_data_type; }
			set 
			{
				if( m_screen_data_type != value )
				{
					// convert screen data
					if( tiles_data_cnt > 0 )
					{
						switch( value )
						{
							case EScreenDataType.sdt_Tiles4x4:
								{
									blocks_to_tiles();
								}
								break;
								
							case EScreenDataType.sdt_Blocks2x2:
								{
									tiles_to_blocks();
								}
								break;
						}
					}
					
					m_screen_data_type = value;
				}
			}
		}
		
		[DataMember]
		private readonly List< layout_data >	m_layouts_data	= null;
		private int m_layouts_data_pos				= -1;
		
		public int layouts_data_pos
		{
			get { return m_layouts_data_pos; }
			set
			{
				m_layouts_data_pos = value;
				
				dispatch_event_set_layout_data();
			}
		}

		public int layouts_data_cnt
		{
			get { return m_layouts_data.Count; }
			set {}
		}
		
		[DataMember]
		private readonly List< tiles_data >	m_tiles_data	= null;
		private int m_tiles_data_pos				= -1;
		private int m_scr_data_pos					= -1;
		
		public int tiles_data_pos
		{
			get { return m_tiles_data_pos; }
			set 
			{ 
				if( value >= 0 && value < tiles_data_cnt ) 
				{ 
					m_tiles_data_pos = value; 
					
					dispatch_event_set_tiles_data();
				} 
			}
		}
		
		public int tiles_data_cnt
		{
			get { return m_tiles_data.Count; }
			set {}
		}

		public int scr_data_pos
		{
			get 
			{
				if( tiles_data_pos < 0 )
				{
					m_scr_data_pos = -1;
				}
				
				return m_scr_data_pos; 
			}
			set 
			{
				m_scr_data_pos = value; 
	
				dispatch_event_set_screen_data(); 
			}
		}
		
		public int scr_data_cnt
		{
			get { return ( tiles_data_pos >= 0 ) ? get_tiles_data( tiles_data_pos ).screen_data_cnt():-1; }
			set {}
		}
		
		private readonly Dictionary< string, List< entity_data > >	m_entities_data	= null;	// key = group name / value = List< entity_data >
		
		[DataMember]
		public Dictionary< string, List< entity_data > > entities_data
		{
			get { return m_entities_data; }
			set {}
		}

		public data_sets_manager()
		{
			m_layouts_data	= new List< layout_data >( 10 );
			m_tiles_data 	= new List< tiles_data >( 10 );
			
			m_entities_data	= new Dictionary< string, List< entity_data > >( 10 );
		}
		
		public void reset()
		{
			foreach( var key in entities_data.Keys ) { ( entities_data[ key ] as List< entity_data > ).ForEach( delegate( entity_data _ent ) { _ent.destroy(); } ); }
			m_entities_data.Clear();

			m_layouts_data.ForEach( delegate( layout_data _obj ) { _obj.destroy(); } );
			m_layouts_data.Clear();

			m_tiles_data.ForEach( delegate( tiles_data _obj ) { _obj.destroy(); } );
			m_tiles_data.Clear();
			
			m_layouts_data_pos	= -1;
			m_tiles_data_pos 	= -1;
			m_scr_data_pos		= -1;
			
			dispatch_event_set_layout_data();
			dispatch_event_set_tiles_data();
			dispatch_event_set_entities_data();

			group_add( "PLAYER" );
			group_add( "ENEMIES" );
			group_add( "BONUSES" );
			group_add( "POWER-UPS" );
			
			screen_data_type = EScreenDataType.sdt_Tiles4x4;
		}
		
		public entity_data get_entity_by_name( string _name )
		{
			entity_data ent = null;
			
			foreach( var key in entities_data.Keys ) { ( entities_data[ key ] as List< entity_data > ).ForEach( delegate( entity_data _ent ) { if( _ent.name == _name ) { ent = _ent; return; } } ); }
			
			return ent;
		}
		
		private bool delete_entity_instances( string _name )
		{
			bool res = false;

			entity_instance ent_inst;
			
			m_layouts_data.ForEach( delegate( layout_data _layout )
			{
				_layout.get_raw_data().ForEach( delegate( List< layout_screen_data > _scr_data_list )
				{ 
					_scr_data_list.ForEach( delegate( layout_screen_data _scr_data ) 
					{
			            for( int ent_n = 0; ent_n < _scr_data.m_ents.Count; ent_n++ )
			            {
			            	ent_inst = _scr_data.m_ents[ ent_n ];
			            	
							if( ent_inst.base_entity.name == _name )
							{
								_layout.entity_instance_reset_target_uid( ent_inst.uid );
								
								ent_inst.reset();
								_scr_data.m_ents.Remove( ent_inst );
								
								--ent_n;
								
								res = true;
							}
			            }
					}); 
				});
			});
			
			return res;
		}
		
		public bool entity_add( string _ent_name, string _grp_name )
		{
			bool res = false;
			
			if( AddEntity != null )
			{
				res = AddEntity( this, new EventArg2Params( _ent_name, _grp_name ) );
			}
			
			if( res == true )
			{
				List< entity_data > ents = m_entities_data[ _grp_name ] as List< entity_data >;
				
				ents.Add( new entity_data( _ent_name ) );
			}
			
			return res;
		}
		
		public bool entity_delete( string _ent_name, string _grp_name )
		{
			bool res = false;
			
			if( DeleteEntity != null )
			{
				res = DeleteEntity( this, new EventArg2Params( _ent_name, _grp_name ) );
			}
			
			if( res == true )
			{
				List< entity_data > ents = m_entities_data[ _grp_name ] as List< entity_data >;
				
				entity_data ent;
				
				for( int ent_n = 0; ent_n < ents.Count; ent_n++ )				
                {
					ent = ents[ ent_n ];
					
	             	if( ent.name == _ent_name ) 
	             	{ 
	             		delete_entity_instances( ent.name ); 
	             		ents.Remove( ent );
	             		
	             		break; 
	             	} 
				}
			}
			
			return res;
		}
		
		public bool entity_rename( string _grp_name, string _old_name, string _new_name )
		{
			List< entity_data > ents = m_entities_data[ _grp_name ] as List< entity_data >;
			
			if( ents != null )
			{
				ents.ForEach( delegate( entity_data _ent ) { if( _ent.name == _old_name ) { _ent.name = _new_name; return; } } );
				
				return true;
			}
			
			return false;
		}

		public bool group_add( string _name )
		{
			bool res = false;
			
			if( AddGroup != null )
			{
				res = AddGroup( this, new EventArg2Params( _name, null ) );
			}
			
			if( res == true )
			{
				m_entities_data.Add( _name, new List< entity_data >() );
			}

			return res;
		}
		
		public bool group_delete( string _name )
		{
			bool res = false;
			
			if( DeleteGroup != null )
			{
				res = DeleteGroup( this, new EventArg2Params( _name, null ) );
			}
			
			if( res == true )
			{
				List< entity_data > ents = m_entities_data[ _name ] as List< entity_data >;
				
				if( ents != null )
				{
					ents.ForEach( delegate( entity_data _ent ) { delete_entity_instances( _ent.name ); _ent.destroy(); } );
					ents.Clear();
				}
				
				m_entities_data.Remove( _name );
			}
			
			return res;
		}

		public bool group_rename( string _old_name, string _new_name )
		{
			List< entity_data > ents = m_entities_data[ _old_name ] as List< entity_data >;
			
			if( ents != null )
			{
				m_entities_data.Remove( _old_name );
				
				m_entities_data.Add( _new_name, ents );
				
				return true;
			}
			
			return false;
		}
		
		public bool layout_data_create()
		{
			if( tiles_data_pos == -1 )
			{
				return false;
			}
			
			if( m_layouts_data_pos < utils.CONST_LAYOUT_MAX_CNT - 1 )
			{
				m_layouts_data.Add( new layout_data() );
				++m_layouts_data_pos;
				
				return true;
			}
			
			return false;
		}
			
		public void layout_data_delete( bool _global_update = true )
		{
			if( layouts_data_pos >= 0 )
			{
				m_layouts_data[ layouts_data_pos ].destroy();
				m_layouts_data.RemoveAt( layouts_data_pos );
				
				if( _global_update )
				{
					--layouts_data_pos;		// here is a global data update
				}
				else
				{
					--m_layouts_data_pos;	// just a class's variable decrement ( silent layout deletion )
				}
			}
		}

		public bool layout_data_copy()
		{
			if( layouts_data_pos >= 0 )
			{
				if( m_layouts_data_pos < utils.CONST_LAYOUT_MAX_CNT - 1 )
				{
					m_layouts_data.Add( m_layouts_data[ layouts_data_pos ].copy() );
					m_layouts_data_pos = m_layouts_data.Count - 1;
					
					return true;
				}
			}
			
			return false;
		}
		
		public void layout_swap( int _layout1, int _layout2 )
		{
			layout_data temp_data = m_layouts_data[ _layout1 ];
			m_layouts_data[ _layout1 ] = m_layouts_data[ _layout2 ];
			m_layouts_data[ _layout2 ] = temp_data;
		}

		public void remove_screen_from_layouts( int _CHR_bank_ind, int _scr_id )
		{
			int 	width;
			int 	height;
			
			layout_screen_data scr_data;
			
			layout_data data;
			
			int scr_global_ind = get_global_screen_ind( _CHR_bank_ind, _scr_id );
			
			int layouts_cnt = m_layouts_data.Count;
			
			for( int layout_n = 0; layout_n < layouts_cnt; layout_n++ )
			{
				data = m_layouts_data[ layout_n ];
				
				width	= data.get_width();
				height	= data.get_height();
				
				for( int i = 0; i < height; i++ )
				{
					for( int j = 0; j < width; j++ )
					{
						scr_data = data.get_data( j, i );
						
						if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
						{
							if( scr_data.m_scr_ind == scr_global_ind )
							{
								data.delete_screen_by_data( scr_data, ( i * data.get_width() + j ) );
							}
							else
							if( scr_data.m_scr_ind > scr_global_ind )
							{
								--scr_data.m_scr_ind;
							}
						}
					}
				}
			}
		}
		
		public void insert_screen_into_layouts( int _scr_ind )
		{
			int 	width;
			int 	height;
			
			layout_screen_data scr_data;
			
			layout_data data;
			
			for( int layout_n = 0; layout_n < layouts_data_cnt; layout_n++ )
			{
				data = get_layout_data( layout_n );
				
				width	= data.get_width();
				height	= data.get_height();
				
				for( int i = 0; i < height; i++ )
				{
					for( int j = 0; j < width; j++ )
					{
						scr_data = data.get_data( j, i );
						
						if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID && scr_data.m_scr_ind >= _scr_ind )
						{
							++scr_data.m_scr_ind;
							data.set_data( scr_data, j, i );
						}
					}
				}
			}
		}

		// get a common screen index ( not just in a current bank )
		public int get_global_screen_ind( int _CHR_bank_ind, int _local_scr_ind )
		{
			int scr_ind = _local_scr_ind;
			
			for( int i = 0; i < _CHR_bank_ind; i++ )
			{
				scr_ind += get_tiles_data()[ i ].screen_data_cnt();
			}
			
			return scr_ind;
		}

		public int get_local_screen_ind( int _CHR_bank_ind, int _global_scr_ind )
		{
			int scr_ind = _global_scr_ind;
			
			for( int i = 0; i < _CHR_bank_ind; i++ )
			{
				scr_ind -= get_tiles_data()[ i ].screen_data_cnt();
			}
			
			return scr_ind;
		}
		
		public bool screen_data_create()
		{
			if( tiles_data_pos >= 0 )
			{
				get_tiles_data( tiles_data_pos ).create_screen( screen_data_type );
				++m_scr_data_pos;
				
				return true;
			}
			
			return false;
		}
		
		public bool screen_data_copy()
		{
			if( tiles_data_pos >= 0 && scr_data_pos >= 0 && scr_data_cnt > 0 )
			{
				get_tiles_data( tiles_data_pos ).copy_screen( scr_data_pos );
				
				return true;
			}
			
			return false;
		}
		
		public void screen_data_delete( bool _global_update = true )
		{
			if( tiles_data_pos >= 0 && scr_data_pos >= 0 )
			{
				get_tiles_data( tiles_data_pos ).delete_screen( scr_data_pos );
				
				if( _global_update )
				{
					--scr_data_pos;		// here is a global data update
				}
				else
				{
					--m_scr_data_pos;	// just a class's variable decrement ( silent screen deletion )
				}
			}
		}
		
		public bool tiles_data_copy()
		{
			if( tiles_data_pos >= 0 && m_tiles_data.Count < utils.CONST_CHR_BANK_MAX_CNT )
			{
				m_tiles_data.Add( get_tiles_data( tiles_data_pos ).copy() );
				
				return true;
			}
			
			return false;
		}
		
		public bool tiles_data_create()
		{
			if( m_tiles_data.Count < utils.CONST_CHR_BANK_MAX_CNT )
			{
				m_tiles_data.Add( new tiles_data() );
				++m_tiles_data_pos;
				
				return true;
			}
			
			return false;
		}
		
		public void tiles_data_destroy()
		{
			m_tiles_data[ m_tiles_data_pos ].destroy();
			m_tiles_data.RemoveAt( m_tiles_data_pos-- );
			
			if( m_tiles_data_pos < 0 )
			{
				if( tiles_data_cnt > 0 )
				{
					m_tiles_data_pos = 0;
				}
				else
				{
					// if there are no items, dispatch the event to reset controls
					dispatch_event_set_tiles_data();
				}
			}
			
			rearrange_tile_data_inds();
		}
		
		private void rearrange_tile_data_inds()
		{
			int size = m_tiles_data.Count;
			
			for( int i = 0; i < size; i++ )
			{
				m_tiles_data[ i ].name = i.ToString();
			}
		}

		private void dispatch_event_set_layout_data()
		{
			if( SetLayoutData != null )
			{
				SetLayoutData( this, null );
			}
		}
		
		private void dispatch_event_set_tiles_data()
		{
			dispatch_event_set_screen_data();			
			
			if( SetTilesData != null )
			{
				SetTilesData( this, null );
			}
		}
		
		private void dispatch_event_set_screen_data()
		{
			if( SetScreenData != null )
			{
				SetScreenData( this, null );
			}
		}
		
		private void dispatch_event_set_entities_data()
		{
			if( SetEntitiesData != null )
			{
				SetEntitiesData( this, null );
			}
		}
		
		public tiles_data get_tiles_data( int _pos )
		{
			return ( _pos >= 0 && _pos < tiles_data_cnt ) ? m_tiles_data[ _pos ]:null;
		}

		public List< tiles_data > get_tiles_data()
		{
			return m_tiles_data;
		}
		
		public layout_data get_layout_data( int _pos )
		{
			return ( _pos >= 0 && _pos < layouts_data_cnt ) ? m_layouts_data[ _pos ]:null;
		}
		
		public void save( BinaryWriter _bw )
		{
			// TILES&SCREENS
			{
				_bw.Write( utils.CONST_IO_DATA_TILES_AND_SCREENS );
				
				_bw.Write( m_tiles_data.Count );
				
				for( int i = 0; i < m_tiles_data.Count; i++ )
				{
					m_tiles_data[ i ].save( _bw, screen_data_type );
				}
				
				_bw.Write( m_tiles_data_pos );
			}
			
			// ENTITIES
			{
				_bw.Write( utils.CONST_IO_DATA_ENTITIES );
				
				_bw.Write( entities_data.Keys.Count );
				
				foreach( string key in entities_data.Keys ) 
				{ 
					_bw.Write( key );
					
					List< entity_data > ent_list = entities_data[ key ] as List< entity_data >;
					
					_bw.Write( ent_list.Count );
					
					ent_list.ForEach( delegate( entity_data _ent ) { _ent.save( _bw ); } ); 
				}
			}
			
			// LAYOUTS
			{
				_bw.Write( utils.CONST_IO_DATA_LAYOUT );

				_bw.Write( m_layouts_data.Count );
				
				for( int i = 0; i < m_layouts_data.Count; i++ )
				{
					m_layouts_data[ i ].save( _bw );
				}
				
				entity_instance.save_instances_counter( _bw );
			}
			
			// PALETTE
			{
				_bw.Write( utils.CONST_IO_DATA_PALETTE );
				
				// save the NES\SMS palettes only,
				// cause they can be imported to
				// save in a project file
				// the other ones can be generated
				
#if DEF_NES || DEF_SMS				
				palette_group.Instance.save_main_palette( _bw );
#endif
			}
			
			// TILES PATTERNS
			{
				_bw.Write( utils.CONST_IO_DATA_TILES_PATTERNS );

				_bw.Write( m_tiles_data.Count );
				
				for( int i = 0; i < m_tiles_data.Count; i++ )
				{
					m_tiles_data[ i ].save_tiles_patterns( _bw );
				}
			}
			
			_bw.Write( utils.CONST_IO_DATA_END );
		}
		
		public int load( byte _ver, BinaryReader _br, string _file_ext, data_conversion_options_form.EScreensAlignMode _scr_align_mode, bool _convert_colors, IProgress< int > _progress, IProgress< string > _status )
		{
			int load_progress = 0;
			
			byte data_id = 0;
			int data_set_pos = 0;
			
			do
			{
				data_id = _br.ReadByte();
				
				if( data_id == utils.CONST_IO_DATA_TILES_AND_SCREENS )
				{
					_status.Report( "Tiles && screens" );
					
					int data_cnt = _br.ReadInt32();
					
					for( int i = 0; i < data_cnt; i++ )
					{
						tiles_data_create();
						get_tiles_data( tiles_data_pos ).load( _ver, _br, _file_ext, _scr_align_mode, screen_data_type );
						
						_progress.Report( utils.calc_progress_val( ref load_progress, data_cnt, i ) );
					}
					
					data_set_pos = _br.ReadInt32();
				}
				else
				if( data_id == utils.CONST_IO_DATA_ENTITIES )
				{
					_status.Report( "Entities" );
					
					List< entity_data > ent_list;
					
					entity_data ent;
					
					int ents_cnt;
					int grps_cnt = _br.ReadInt32();
					
					string grp_name;
					
					entities_data.Clear();
					
					for( int grp_n = 0; grp_n < grps_cnt; grp_n++ )
					{
						grp_name = _br.ReadString();

						ent_list = new List< entity_data >();
						entities_data.Add( grp_name, ent_list );
						
						ents_cnt = _br.ReadInt32();
						
						for( int ent_n = 0; ent_n < ents_cnt; ent_n++ )
						{
							ent = new entity_data();
							ent.load( _ver, _br );
							
							ent_list.Add( ent );
						}
					}
					
					// dispatch_event_set_entities_data(); - moved to the post_load_update()
					
					_progress.Report( utils.calc_progress_val_half( ref load_progress ) );
				}
				else
				if( data_id == utils.CONST_IO_DATA_LAYOUT )
				{
					_status.Report( "Layouts" );
					
					int layouts_data_cnt = _br.ReadInt32();
					
					for( int i = 0; i < layouts_data_cnt; i++ )
					{
						layout_data_create();
						get_layout_data( layouts_data_pos ).load( _ver, _br, get_entity_by_name, _file_ext, _scr_align_mode, screen_data_type );
					}
					
					entity_instance.load_instances_counter( _br );
					
					_progress.Report( utils.calc_progress_val_half( ref load_progress ) );
				}
				else				
				if( data_id == utils.CONST_IO_DATA_PALETTE )
				{
					_status.Report( "Palette" );
					
					int i;
					
					int plt_len 	= platform_data.get_main_palette_colors_cnt( _file_ext );
					int[] plt_main	= null;
					
					bool ignore_palette = ( _file_ext != platform_data.get_platform_file_ext( platform_data.get_platform_type() ) );
					
					if( ignore_palette )
					{
						if( _convert_colors )
						{
							plt_main = platform_data.get_palette_by_file_ext( _file_ext );
							
							if( _file_ext == platform_data.CONST_NES_FILE_EXT || _file_ext == platform_data.CONST_SMS_FILE_EXT )
							{
								// load main palette from the project file
								int data_pos = 0;
								
								do
								{
									plt_main[ data_pos ] = _br.ReadByte() << 16 | _br.ReadByte() << 8 | _br.ReadByte();
								}
								while( ++data_pos != plt_len );
							}
						}
							
						project_data_converter_provider.get_converter().palettes_processing( _ver, platform_data.get_platform_type_by_file_ext( _file_ext ), _convert_colors, this, plt_main );
							
						if( _convert_colors )
						{
							for( i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
							{
								palette_group.Instance.get_palettes_arr()[ i ].update();
							}
						}
						else
						{
							if( _file_ext == platform_data.CONST_NES_FILE_EXT || _file_ext == platform_data.CONST_SMS_FILE_EXT )
							{
								// skip palette
								_br.ReadBytes( plt_len * 3 );
							}
						}
					}
#if DEF_NES || DEF_SMS
					else
					{
						palette_group.Instance.load_main_palette( _br );
					}
#endif
#if DEF_FIXED_LEN_PALETTE16_ARR
					// fill missing palette(s)
					{
						tiles_data data;
						
						for( int data_n = 0; data_n < tiles_data_cnt; data_n++ )
						{
							data = get_tiles_data( data_n );
							
							for( i = data.palettes_arr.Count; i < platform_data.get_fixed_palette16_cnt(); i++ )
							{
								data.palettes_arr.Add( new palette16_data() );
							}
						}
					}
#endif
					_progress.Report( utils.calc_progress_val_half( ref load_progress ) );
				}
				else
				if( data_id == utils.CONST_IO_DATA_TILES_PATTERNS )
				{
					_status.Report( "Tiles Patterns" );
					
					int data_cnt = _br.ReadInt32();
					
					for( int i = 0; i < data_cnt; i++ )
					{
						m_tiles_data[ i ].load_tiles_patterns( _ver, _br );
					}

					_progress.Report( utils.calc_progress_val_half( ref load_progress ) );
				}
			}
			while( data_id != utils.CONST_IO_DATA_END );
			
			return data_set_pos;
		}
		
		public void post_load_update()
		{
			project_data_converter_provider.get_converter().post_load_data_cleanup();
			
			if( entities_data.Count > 0 )
			{
				dispatch_event_set_entities_data();
			}
		}
		
		public void save_JSON( TextWriter _tw )
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer( typeof( data_sets_manager ) );
			
			using( MemoryStream s = new MemoryStream() )
			{
				serializer.WriteObject( s, this );
				
				_tw.WriteLine( utils.format_Json( Encoding.UTF8.GetString( s.GetBuffer() ) ) );
				_tw.Flush();
			}			
		}
		
		public int merge_CHR_spred( BinaryReader _br )
		{
			int CHR_cnt = _br.ReadInt32();
			int spr8x8_cnt;

			bool res = true;
			int copied_CHRs = 0;

			tiles_data data = get_tiles_data( tiles_data_pos );
			
			int chr_id;
			
			// get a first free spr8x8 position
			{
				chr_id = data.get_first_free_spr8x8_id();
				
				if( chr_id < 0 )
				{
					throw new Exception( "There is no free space in the active CHR bank!" );
				}
			}
			
			for( int i = 0; i < CHR_cnt; i++ )
			{
				_br.ReadInt32();	// skip id
				_br.ReadInt32();	// skip link_cnt
				
				spr8x8_cnt = _br.ReadInt32();
				
				if( chr_id + spr8x8_cnt > platform_data.get_CHR_bank_max_sprites_cnt() )
				{
					MainForm.set_status_msg( String.Format( "Merged: {0} of {1} CHR banks", i, CHR_cnt ) );
					
					res = false;
					
					if( copied_CHRs == 0 )
						return -1;
				}
				
				for( int j = 0; j < spr8x8_cnt; j++ )
				{
					_br.Read( utils.tmp_spr8x8_buff, 0, utils.CONST_SPR8x8_TOTAL_PIXELS_CNT );
					
					if( res )
					{
						data.from_spr8x8_to_CHR_bank( chr_id++, utils.tmp_spr8x8_buff );
						
						++copied_CHRs;
					}
				}
			}			
			
			if( copied_CHRs != 0 )
			{
				if( MainForm.message_box( "Import palette ?", "SPReD Data Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					for( int plt_n = 0; plt_n < utils.CONST_PALETTE_SMALL_NUM_COLORS * utils.CONST_NUM_SMALL_PALETTES; plt_n++ )
					{
						data.subpalettes[ plt_n >> 2 ][ plt_n & 0x03 ] = (byte)_br.ReadInt32();
					}
				}
			}
			
			return res ? CHR_cnt:-1;
		}
		
		public void export_entity_asm( StreamWriter _sw, string _db, string _num_pref )
		{
			bool enable_comments = true;
			
			_sw.WriteLine( "\n; *** BASE ENTITIES ***\n" );
			
			foreach( var key in entities_data.Keys ) 
			{ 
				( entities_data[ key ] as List< entity_data > ).ForEach( delegate( entity_data _ent ) 
				{ 
					_sw.WriteLine( _ent.name + ":" );
					_sw.WriteLine( "\t" + _db + " " + utils.hex( _num_pref, _ent.uid ) + ( enable_comments ? "\t; uid":"" ) );
					_sw.WriteLine( "\t" + _db + " " + utils.hex( _num_pref, _ent.width ) + ( enable_comments ? "\t; width":"" ) );
					_sw.WriteLine( "\t" + _db + " " + utils.hex( _num_pref, _ent.height ) + ( enable_comments ? "\t; height":"" ) );
					_sw.WriteLine( "\t" + _db + " " + utils.hex( _num_pref, _ent.pivot_x ) + ( enable_comments ? "\t; pivot x":"" ) );
					_sw.WriteLine( "\t" + _db + " " + utils.hex( _num_pref, _ent.pivot_y ) + ( enable_comments ? "\t; pivot y":"" ) );
		
					utils.save_prop_asm( _sw, _db, _num_pref, _ent.properties, enable_comments );
					
					_sw.WriteLine( "" );
					
					enable_comments = false;
				});
			}			
		}

		private void tiles_to_blocks()
		{
			tiles_data data = null;
			
			screen_data		new_pattern	= null;
			pattern_data	pattern		= null;
			
			screen_data new_scr;
			
			int data_n;
			int ptrn_n;
			int scr_n;
			int tile_n;
			int block_n;
			int y_pos;
			
			for( data_n = 0; data_n < tiles_data_cnt; data_n++ )
			{
				data = get_tiles_data( data_n );
				
				// convert screens
				for( scr_n = 0; scr_n < data.screen_data_cnt(); scr_n++ )
				{
					new_scr = new screen_data( EScreenDataType.sdt_Blocks2x2 );
					
					for( tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
					{
						for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
						{
							y_pos = ( ( tile_n / utils.CONST_SCREEN_NUM_WIDTH_TILES ) << 1 ) + ( ( block_n & 0x02 ) >> 1 );
							
#if DEF_SCREEN_HEIGHT_7d5_TILES
							if( y_pos <= 14 )
#endif
								new_scr.set_tile( ( y_pos * utils.CONST_SCREEN_NUM_WIDTH_BLOCKS ) + ( ( tile_n % utils.CONST_SCREEN_NUM_WIDTH_TILES ) << 1 ) + ( block_n & 0x01 ), utils.get_ushort_from_ulong( data.tiles[ data.get_screen_tile( scr_n, tile_n ) ], block_n ) );
						}
					}
					
					data.screen_data_replace( scr_n, new_scr );
				}
				
				// convert tiles patterns
				foreach( string key in data.patterns_data.Keys ) 
				{ 
					List< pattern_data > pattrn_list = data.patterns_data[ key ] as List< pattern_data >;
					
					for( ptrn_n = 0; ptrn_n < pattrn_list.Count; ptrn_n++ )
					{
						pattern = pattrn_list[ ptrn_n ];

						new_pattern = new screen_data( pattern.width << 1, pattern.height << 1 );

						for( tile_n = 0; tile_n < pattern.data.length; tile_n++ )
						{
							for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
							{
								new_pattern.set_tile( ( ( ( ( tile_n / pattern.width ) << 1 ) + ( ( block_n & 0x02 ) >> 1 ) ) * ( pattern.width << 1 ) ) + ( ( tile_n % pattern.width ) << 1 ) + ( block_n & 0x01 ), utils.get_ushort_from_ulong( data.tiles[ pattern.data.get_tile( tile_n ) ], block_n ) );
							}
						}
						
						pattrn_list[ ptrn_n ] = new pattern_data( pattern.name, new_pattern );
						pattern.reset();
					}
				}
				
				Array.Clear( data.tiles, 0, data.tiles.Length );
			}
		}

		private void blocks_to_tiles()
		{
			tiles_data		data;
			pattern_data	pattern;
			screen_data		new_pattern;
			screen_data		new_scr;
			
			int ptrn_width;
			int ptrn_height;
			
			int i;
			int scr_n;
			int data_n;
			int ptrn_n;
			int tile_n;
			int tile_x;
			int tile_y;
			int tile_offs;
			ulong tile;
			ushort tile_val0 = 0;
			ushort tile_val1 = 0;
			ushort tile_val2 = 0;
			ushort tile_val3 = 0;
			
			bool tiles_arr_overflow	= false;

			string ptrn_invalid_size_str = "";
			string ptrn_invalid_tile_str = "";
			
			Dictionary< int, List< screen_data > > 	bank_id_screens	= new Dictionary< int, List< screen_data > >( tiles_data_cnt );
			Dictionary< int, ulong[] >				bank_id_tiles	= new Dictionary< int, ulong[] >( tiles_data_cnt );
 
			for( data_n = 0; data_n < tiles_data_cnt; data_n++ )
			{
				data = get_tiles_data( data_n );
				
				List< screen_data >	screens	= new List< screen_data >( data.screen_data_cnt() );
				ulong[] 			tiles	= new ulong[ platform_data.get_max_tiles_cnt() ];
				Array.Clear( tiles, 0, tiles.Length );
				
				ushort tile_ind = 0;
				
				// convert screens and fill tile arrays
				for( scr_n = 0; scr_n < data.screen_data_cnt(); scr_n++ )
				{
					new_scr = new screen_data( EScreenDataType.sdt_Tiles4x4 );
					
					for( tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
					{
						tile_x = tile_n % utils.CONST_SCREEN_NUM_WIDTH_TILES;
						tile_y = tile_n / utils.CONST_SCREEN_NUM_WIDTH_TILES;
						
						tile_offs = ( tile_x << 1 ) + ( ( tile_y << 1 ) * utils.CONST_SCREEN_NUM_WIDTH_BLOCKS );
						
						tile_val0 = data.get_screen_tile( scr_n, tile_offs );
						tile_val1 = data.get_screen_tile( scr_n, tile_offs + 1 );
						
#if DEF_SCREEN_HEIGHT_7d5_TILES
						if( tile_y < 7 )
#endif
						{
							tile_val2 = data.get_screen_tile( scr_n, tile_offs + utils.CONST_SCREEN_NUM_WIDTH_BLOCKS );
							tile_val3 = data.get_screen_tile( scr_n, tile_offs + utils.CONST_SCREEN_NUM_WIDTH_BLOCKS + 1 );
						}
#if DEF_SCREEN_HEIGHT_7d5_TILES
						else
						{
							tile_val2 = tile_val3 = 0;
						}
#endif						
						tile = tiles_data.set_tile_data( tile_val0, tile_val1, tile_val2, tile_val3 );

						// check new tile
						for( i = 0; i < tile_ind; i++ )
						{
							if( tiles[ i ] == tile )
							{
								break;
							}
						}
						
						if( i == tile_ind )
						{
							if( tile_ind == platform_data.get_max_tiles_cnt() )
							{
								tiles_arr_overflow = true;
								goto free_data;
							}
							
							new_scr.set_tile( tile_x + tile_y * utils.CONST_SCREEN_NUM_WIDTH_TILES, tile_ind );
							tiles[ tile_ind++ ] = tile;
						}
						else
						{
							new_scr.set_tile( tile_x + tile_y * utils.CONST_SCREEN_NUM_WIDTH_TILES, ( ushort )i );
						}
					}
					
					screens.Add( new_scr );
				}
				
				bank_id_screens[ data_n ]	= screens;
				bank_id_tiles[ data_n ]		= tiles;

				// convert tiles patterns
				foreach( string key in data.patterns_data.Keys ) 
				{ 
					List< pattern_data > pattrn_list = data.patterns_data[ key ] as List< pattern_data >;
					
					for( ptrn_n = 0; ptrn_n < pattrn_list.Count; ptrn_n++ )
					{
						pattern = pattrn_list[ ptrn_n ];
						
						if( ( pattern.width & 0x01 ) != 0 || ( pattern.height & 0x01 ) != 0 )
						{
							ptrn_invalid_size_str += pattern.name + " / CHR bank: " + data_n + "\n";
							pattern.name = "BAD~" + pattern.name; 
						}
						else
						{
							ptrn_width	= pattern.width >> 1;
							ptrn_height	= pattern.height >> 1;							
							
							new_pattern = new screen_data( ptrn_width, ptrn_height );
	
							for( tile_n = 0; tile_n < new_pattern.length; tile_n++ )
							{
								tile_x = tile_n % ptrn_width;
								tile_y = tile_n / ptrn_width;
								
								tile_offs = ( tile_x << 1 ) + ( ( tile_y << 1 ) * pattern.width );
								
								tile_val0 = pattern.data.get_tile( tile_offs );
								tile_val1 = pattern.data.get_tile( tile_offs + 1 );
								tile_val2 = pattern.data.get_tile( tile_offs + pattern.width );
								tile_val3 = pattern.data.get_tile( tile_offs + pattern.width + 1 );
								
								tile = tiles_data.set_tile_data( tile_val0, tile_val1, tile_val2, tile_val3 );

								// get tile index
								for( i = 0; i < tile_ind; i++ )
								{
									if( tiles[ i ] == tile )
									{
										break;
									}
								}
								
								if( i == tile_ind )
								{
									ptrn_invalid_tile_str += pattern.name + " / CHR bank: " + data_n + "\n";
									pattern.name = "BAD~" + pattern.name;
								}
								else
								{
									new_pattern.set_tile( tile_x + tile_y * ptrn_width, ( ushort )i );
								}
							}
							
							pattrn_list[ ptrn_n ] = new pattern_data( pattern.name, new_pattern );
							pattern.reset();
						}
					}
				}
				
				if( ptrn_invalid_size_str.Length > 0 || ptrn_invalid_tile_str.Length > 0 )
				{
					MainForm.message_box( ( ptrn_invalid_size_str.Length > 0 ? "Invalid size:\n\n" + ptrn_invalid_size_str + "\n":"" ) + ( ptrn_invalid_tile_str.Length > 0 ? "Invalid data:\n\n" + ptrn_invalid_tile_str + "\n":"" ) + "Invalid pattern(s) will be market as 'BAD~'", "Invalid Tiles Patterns Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}				
			}

			// commit new data
			for( data_n = 0; data_n < tiles_data_cnt; data_n++ )
			{
				data = get_tiles_data( data_n );

				// update tiles & screens
				Array.Copy( bank_id_tiles[ data_n ] as ulong[], data.tiles, platform_data.get_max_tiles_cnt() );
				
				for( scr_n = 0; scr_n < data.screen_data_cnt(); scr_n++ )
				{
					data.screen_data_replace( scr_n, bank_id_screens[ data_n ][ scr_n ] );
					bank_id_screens[ data_n ][ scr_n ] = null;
				}
				
				bank_id_screens[ data_n ].Clear();
			}
			
			free_data:
			{
				foreach( var key in bank_id_screens.Keys ) { ( bank_id_screens[ key ] as List< screen_data > ).Clear(); };
				bank_id_screens.Clear();

				bank_id_tiles.Clear();
			}
			
			if( tiles_arr_overflow )
			{
				throw new Exception( "Tiles array overflow!\nCHR bank: " + data_n );
			}
		}
	}
}
