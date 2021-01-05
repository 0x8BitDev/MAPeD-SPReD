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
using System.Collections;
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
		private object	m_param1	= null;
		private object	m_param2	= null;
		
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
	
	public delegate void SetLayoutData();
	public delegate void SetTilesData();
	public delegate void SetScreenData();
	public delegate void SetEntitiesData();
	
	public delegate bool ReturnBoolEvent( object sender, EventArgs e );
	
	[DataContract]
	public class data_sets_manager
	{
		public event EventHandler SetLayoutData;
		public event EventHandler SetTilesData;
		public event EventHandler SetScreenData;
		public event EventHandler SetEntitiesData;
		
		public event ReturnBoolEvent AddEntity;
		public event ReturnBoolEvent DeleteEntity;
		public event ReturnBoolEvent AddGroup;
		public event ReturnBoolEvent DeleteGroup;

		[DataMember]
		private string data_desc = "CHR Data Size: " + ( utils.CONST_CHR_BANK_PAGE_SIZE * utils.CONST_CHR_BANK_PAGES_CNT ) + " | Tiles Data Size: " + utils.CONST_TILES_UINT_SIZE + " | Blocks Data Size: " + utils.CONST_BLOCKS_USHORT_SIZE + " | Screen Data Size: " + utils.CONST_SCREEN_TILES_CNT;
		[DataMember]
		private string block_desc = "(bits): 15-12 -> Obj id | [10-11 NES: -> Palette id | SMS: -> CHR bank page] | [8-9 NES: not used | SMS: -> Flip flags (01-HFlip | 02-VFlip)] | 7-0 -> CHR id";
		
		[DataMember]		
		private List< layout_data >	m_layouts_data	= null;
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
		private List< tiles_data >	m_tiles_data	= null;
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
			get { return ( tiles_data_pos >= 0 ) ? get_tiles_data( tiles_data_pos ).scr_data.Count:-1; }
			set {}
		}
		
		private Dictionary< string, List< entity_data > >	m_entities_data	= null;	// key = group name / value = List< entity_data >
		
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
				_layout.get_raw_data().ForEach( delegate( List< screen_data > _scr_data_list )
				{ 
					_scr_data_list.ForEach( delegate( screen_data _scr_data ) 
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
			
		public void layout_data_delete()
		{
			if( layouts_data_pos >= 0 )
			{
				m_layouts_data[ layouts_data_pos ].destroy();
				m_layouts_data.RemoveAt( layouts_data_pos-- );
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
			
			screen_data scr_data;
			
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
			
			screen_data scr_data;
			
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
				scr_ind += get_tiles_data()[ i ].scr_data.Count;
			}
			
			return scr_ind;
		}

		public int get_local_screen_ind( int _CHR_bank_ind, int _global_scr_ind )
		{
			int scr_ind = _global_scr_ind;
			
			for( int i = 0; i < _CHR_bank_ind; i++ )
			{
				scr_ind -= get_tiles_data()[ i ].scr_data.Count;
			}
			
			return scr_ind;
		}
		
		public bool screen_data_create()
		{
			if( tiles_data_pos >= 0 )
			{
				get_tiles_data( tiles_data_pos ).create_screen();
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
		
		public void screen_data_delete()
		{
			if( tiles_data_pos >= 0 && scr_data_pos >= 0 )
			{
				get_tiles_data( tiles_data_pos ).delete_screen( scr_data_pos-- );
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
					m_tiles_data[ i ].save( _bw );
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
				
				palette_group.Instance.save_main_palette( _bw );
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
		
		public void load( byte _ver, BinaryReader _br, string _file_ext, data_conversion_options_form.EScreensAlignMode _scr_align_mode, bool _convert_colors )
		{
			byte data_id = 0;
			
			do
			{
				data_id = _br.ReadByte();
				
				if( data_id == utils.CONST_IO_DATA_TILES_AND_SCREENS )
				{
					int tiles_data_cnt = _br.ReadInt32();
					
					for( int i = 0; i < tiles_data_cnt; i++ )
					{
						tiles_data_create();
						get_tiles_data( tiles_data_pos ).load( _ver, _br, _file_ext, _scr_align_mode );
					}
					
					tiles_data_pos = _br.ReadInt32();
				}
				else
				if( data_id == utils.CONST_IO_DATA_ENTITIES )
				{
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
					
					dispatch_event_set_entities_data();
				}
				else
				if( data_id == utils.CONST_IO_DATA_LAYOUT )
				{
					int layouts_data_cnt = _br.ReadInt32();
					
					for( int i = 0; i < layouts_data_cnt; i++ )
					{
						layout_data_create();
						get_layout_data( layouts_data_pos ).load( _ver, _br, get_entity_by_name, _file_ext, _scr_align_mode );
					}
					
					entity_instance.load_instances_counter( _br );
				}
				else				
				if( data_id == utils.CONST_IO_DATA_PALETTE )
				{
					int[] plt_main	= new int[ palette_group.Instance.main_palette.Length ];
#if DEF_NES								
					bool ignore_palette = ( _file_ext == utils.CONST_SMS_FILE_EXT ) ? true:false;
#elif DEF_SMS
					bool ignore_palette = ( _file_ext == utils.CONST_NES_FILE_EXT ) ? true:false;
#endif				
					if( ignore_palette )
					{
						if( _convert_colors )
						{
							// load main palette from the project file
							int data_pos = 0;
						
							do
							{
								plt_main[ data_pos ] = _br.ReadByte() << 16 | _br.ReadByte() << 8 | _br.ReadByte();
							}
							while( ++data_pos != plt_main.Length );
							
							List< byte[] > palettes = null;
							
							for( int data_n = 0; data_n < tiles_data_cnt; data_n++ )
							{
								palettes = get_tiles_data( data_n ).palettes;
								
								for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
								{
									palettes[ i >> 2 ][ i & 0x03 ] = ( byte )utils.find_nearest_color_ind( plt_main[ palettes[ i >> 2 ][ i & 0x03 ] ] );
								}
							}
							
							for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
							{
								palette_group.Instance.get_palettes_arr()[ i ].update();
							}
						}
						else
						{
							_br.ReadBytes( 192 );
						}
					}
					else
					{
						palette_group.Instance.load_main_palette( _br );
					}
				}
				else
				if( data_id == utils.CONST_IO_DATA_TILES_PATTERNS )
				{
					int tiles_data_cnt = _br.ReadInt32();
					
					for( int i = 0; i < tiles_data_cnt; i++ )
					{
						m_tiles_data[ i ].load_tiles_patterns( _ver, _br );
					}
				}
			}
			while( data_id != utils.CONST_IO_DATA_END );
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
			
			int chr_id = 0;
			
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
				
				if( chr_id + spr8x8_cnt > utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
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
						data.palettes[ plt_n >> 2 ][ plt_n & 0x03 ] = (byte)_br.ReadInt32();
					}
				}
			}
			
			return res ? CHR_cnt:-1;
		}
		
		public int merge_CHR_bin( BinaryReader _br )
		{
			tiles_data data = get_tiles_data( tiles_data_pos );
			byte[] tmp_arr 	= new byte[ utils.CONST_SPR8x8_TOTAL_PIXELS_CNT ];
			
			int i;
			int j;
			
			byte byte_0;
			byte byte_1;
#if DEF_SMS
			byte byte_2;
			byte byte_3;
			
			int ind_offset;
#endif
			int shift_7_cnt;
			
			if( _br.BaseStream.Length < utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES )
			{
				return -1;
			}
			
			int added_CHRs = 0;
			
			int chr_id = data.get_first_free_spr8x8_id();
			
			if( chr_id >= 0 )
			{
				do
				{
					tmp_arr = _br.ReadBytes( utils.CONST_SPR8x8_NATIVE_SIZE_IN_BYTES );
					
					for( i = 0; i < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; i++ )
					{
#if DEF_NES
						byte_0 	= tmp_arr[ i ];
						byte_1	= tmp_arr[ i + utils.CONST_SPR8x8_SIDE_PIXELS_CNT ];
#elif DEF_SMS
						ind_offset = i << 2;
						
						byte_0	= tmp_arr[ ind_offset ];
						byte_1 	= tmp_arr[ ind_offset + 1 ];
						byte_2	= tmp_arr[ ind_offset + 2 ];
						byte_3 	= tmp_arr[ ind_offset + 3 ];
#endif
						for( j = 0; j < utils.CONST_SPR8x8_SIDE_PIXELS_CNT; j++ )
						{
							shift_7_cnt = 7 - j;
#if DEF_NES							
							utils.tmp_spr8x8_buff[ j + ( i << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) ] = ( byte )( ( ( byte_0 & ( 1 << shift_7_cnt ) ) >> shift_7_cnt ) | ( ( ( byte_1 << 1 ) & ( 1 << ( 8 - j ) ) ) >> shift_7_cnt ) );
#elif DEF_SMS
							utils.tmp_spr8x8_buff[ j + ( i << utils.CONST_SPR8x8_SIDE_PIXELS_CNT_POW_BITS ) ] = ( byte )( ( ( byte_0 >> shift_7_cnt & 0x01 ) ) | ( ( ( byte_1 >> shift_7_cnt & 0x01 ) ) << 1 ) | ( ( ( byte_2 >> shift_7_cnt & 0x01 ) ) << 2 ) | ( ( ( byte_3 >> shift_7_cnt & 0x01 ) ) << 3 ) );
#endif
						}
					}
					
					data.from_spr8x8_to_CHR_bank( chr_id++, utils.tmp_spr8x8_buff );
					
					++added_CHRs;
					
					if( chr_id == utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
					{
						break;
					}
				}
				while( _br.BaseStream.Position + 16 <= _br.BaseStream.Length );
			}
			else
			{
				throw new Exception( "There is no free space in the active CHR bank!" );				
			}
			
			return added_CHRs;
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
		
	}
}
