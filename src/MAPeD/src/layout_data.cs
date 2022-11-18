/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 28.05.2017
 * Time: 15:12
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MAPeD
{
	/// <summary>
	/// Description of layout_data.
	/// </summary>
	/// 
	
	[DataContract]
	public class layout_screen_data
	{
		public static event EventHandler EntityAdded;
		public static event EventHandler EntityRemoved;
		
		[DataMember]
		private ushort	m_marks = 0;	// 8 bits currently in use. the rest are reserved for future purposes.
		
		public ushort mark
		{
			get { return ( ushort )( m_marks & 0x0f ); }
			set { m_marks &= unchecked( ( ushort )~0x0f ); m_marks |= value; }
		}
		
		public ushort adj_scr_mask
		{
			get { return ( ushort )( ( m_marks & 0xf0 ) >> 4 ); }
			set { m_marks &= unchecked( ( ushort )~0xf0 ); m_marks |= ( ushort )( ( value & 0x0f ) << 4 ); }
		}
		
		public ushort mark_adj_scr_mask
		{
			get { return m_marks; }
			set {}
		}
		
		public enum EEntSortType
		{
			est_NoSorting	= 0,
			est_LeftToRight = 1,
			est_BottomToTop = 2,
		};
		
		[DataMember]
		public int 					m_scr_ind;
		
		[DataMember]
		private List< entity_instance > 	m_ents;
		
		public layout_screen_data( int _scr_ind )
		{
			m_scr_ind = _scr_ind;
			m_ents = new List< entity_instance >();
		}
		
		public int get_num_entities()
		{
			return m_ents.Count;
		}
		
		public entity_instance get_entity( int _ent_id )
		{
			return m_ents[ _ent_id ];
		}
		
		public void add_entity( entity_instance _ent )
		{
			m_ents.Add( _ent );
			
			dispatch_event_entity_added( _ent );
		}
		
		public void remove_entity( int _pos )
		{
			entity_instance ent = m_ents[ _pos ];
			
			m_ents.RemoveAt( _pos );
			
			dispatch_event_entity_removed( ent );
		}

		public bool remove_entity( entity_instance _ent )
		{
			if( m_ents.Remove( _ent ) )
			{
				dispatch_event_entity_removed( _ent );
				
				return true;
			}
			
			return false;
		}
		
		public void insert_entity( int _ind, entity_instance _ent )
		{
			m_ents.Insert( _ind, _ent );
			
			dispatch_event_entity_added( _ent );
		}
		
		public void clear_entities( bool _null )
		{
			m_ents.Clear();
			
			if( _null )
			{
				m_ents = null;
			}
			
			dispatch_event_entity_removed( null );
		}
		
		public void entities_proc( Action< entity_instance > _act )
		{
			m_ents.ForEach( _act );
		}
		
		public int entities_find_index( Predicate< entity_instance > _pred )
		{
			return m_ents.FindIndex( _pred );
		}
		
		private void dispatch_event_entity_added( entity_instance _ent )
		{
			if( EntityAdded != null )
			{
				EntityAdded( _ent, null );
			}
		}
		
		private void dispatch_event_entity_removed( entity_instance _ent )
		{
			if( EntityRemoved != null )
			{
				EntityRemoved( _ent, null );
			}
		}
		
		public layout_screen_data copy()
		{
			layout_screen_data new_data = new layout_screen_data( m_scr_ind );
			
			new_data.mark = mark;
			
			m_ents.ForEach( delegate( entity_instance _ent_inst ) { new_data.m_ents.Add( _ent_inst.copy() ); } );
			
			return new_data;
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_marks );
			
			_bw.Write( m_scr_ind );
			
			_bw.Write( m_ents.Count );
			
			m_ents.ForEach( delegate( entity_instance _ent_inst )
			{
               	_ent_inst.save( _bw );
			});
		}
		
		public void load( BinaryReader _br, byte _ver, Func< string, entity_data > _get_ent, data_sets_manager.EScreenDataType _scr_type, bool _fix_inside_scr )
		{
			m_marks	  = _br.ReadUInt16();
			
			if( _ver <= 7 )
			{
				m_scr_ind = _br.ReadByte();
				m_scr_ind = ( m_scr_ind == 0xff ) ? -1:m_scr_ind; 
			}
			else
			{
				m_scr_ind = _br.ReadInt32();
			}
			
			int ents_cnt = _br.ReadInt32();
			
			entity_instance 	ent_inst;
			
			for( int ent_n = 0; ent_n < ents_cnt; ent_n++ )
			{
				ent_inst = new entity_instance();
				ent_inst.load( _br, _ver, _get_ent, _scr_type, _fix_inside_scr );
				
				m_ents.Add( ent_inst );
			}
		}

		public int export_entities_asm( StreamWriter _sw, ref int _ent_id, string _label, string _def_num, string _def_addr, string _def_coord, string _num_pref, bool _ent_coords_scr, int _x, int _y, EEntSortType _sort_type, bool _enable_comments )
		{
			int max_props_cnt = -1;
			int props_cnt;
			
			entity_instance ent_inst;
			
			_sw.WriteLine( _label + ":" );
			_sw.WriteLine( "\t" + _def_num + " " + m_ents.Count );
			
			// entities sorting left to right and bottom to top
			if( _sort_type == EEntSortType.est_LeftToRight )
			{
				m_ents.Sort( delegate( entity_instance _ent1, entity_instance _ent2 )
				{
					return ( ( _ent1.base_entity.pivot_x + _ent1.x ).CompareTo( _ent2.base_entity.pivot_x + _ent2.x ) );
				});
			}
			else
			if( _sort_type == EEntSortType.est_BottomToTop )
			{
				m_ents.Sort( delegate( entity_instance _ent1, entity_instance _ent2 )
				{
					return ( -( _ent1.base_entity.pivot_y + _ent1.y ).CompareTo( _ent2.base_entity.pivot_y + _ent2.y ) );
				});
			}
			
			// ent labels arr
			for( int ent_n = 0; ent_n < m_ents.Count; ent_n++ )
			{
				ent_inst = m_ents[ ent_n ];
				
				_sw.WriteLine( "\t" + _def_addr + " " + ent_inst.name );
			}
			
			// ent data arr			
			for( int ent_n = 0; ent_n < m_ents.Count; ent_n++ )
			{
				ent_inst = m_ents[ ent_n ];
				
				_sw.WriteLine( ent_inst.name + ":" );
				_sw.WriteLine( "\t" + _def_num + " " + utils.hex( _num_pref, _ent_id++ ) + ( _enable_comments ? "\t; entity instance number (0..n)":"" ) );
				_sw.WriteLine( "\t" + _def_addr + " " + ent_inst.base_entity.name + ( _enable_comments ? "\t; base entity":"" ) );
				_sw.WriteLine( "\t" + _def_addr + " " + ( ent_inst.target_uid >= 0 ? ( "Instance" + ent_inst.target_uid.ToString() ):( utils.hex( _num_pref, 0 ) ) ) + ( _enable_comments ? "\t; target entity":"" ) );
				_sw.WriteLine( "\t" + _def_coord + " " + utils.hex( _num_pref, ent_inst.x + ent_inst.base_entity.pivot_x + ( _ent_coords_scr ? 0:_x * platform_data.get_screen_width_pixels() ) ) + ( _enable_comments ? ( "\t; " + ( _ent_coords_scr ? "scr":"map" ) + " X" ):"" ) );
				_sw.WriteLine( "\t" + _def_coord + " " + utils.hex( _num_pref, ent_inst.y + ent_inst.base_entity.pivot_y + ( _ent_coords_scr ? 0:_y * platform_data.get_screen_height_pixels() ) ) + ( _enable_comments ? ( "\t; " + ( _ent_coords_scr ? "scr":"map" ) + " Y" ):"" ) );
					
				props_cnt = utils.save_prop_asm( _sw, _def_num, _num_pref, ent_inst.properties, _enable_comments );
				
				if( max_props_cnt < props_cnt )
				{
					max_props_cnt = props_cnt;
				}
			}
			
			return max_props_cnt;
		}
	}
	
	[DataContract]
	public class layout_data
	{
		[DataMember]
		private int m_start_scr_ind	= -1;

		[DataMember]
		private List< List< layout_screen_data > >	m_layout	= null;

		public const int	CONST_EMPTY_CELL_ID		= -1;
		
		public static readonly int[] adj_scr_slots	= new int[]{ -1, -1, 0, -1, 1, -1, -1, 1, 0, 1, 1, 1, -1, 0, 1, 0 };
		
		public layout_data()
		{
			m_layout = new List< List< layout_screen_data > >();
			
			reset( true );
		}
		
		public void reset( bool _init )
		{
			m_layout.ForEach( delegate( List< layout_screen_data > _list ) 
			{ 
				_list.ForEach( delegate( layout_screen_data _data ) 
				{
					_data.entities_proc( delegate( entity_instance _ent_inst ) 
					{
						_ent_inst.reset();
					});
					
					_data.clear_entities( true );
				}); 
				
				_list.Clear(); 
			});
			
			m_layout.Clear();
			
			m_start_scr_ind = -1;
			
			if( _init )
			{
				List< layout_screen_data > row = new List< layout_screen_data >();
				row.Add( new layout_screen_data( CONST_EMPTY_CELL_ID ) );
				m_layout.Add( row );
			}
		}
		
		public void destroy()
		{
			reset( false );
			
			m_layout = null;
		}
		
		public layout_data copy()
		{
			layout_data new_data = new layout_data();
			new_data.m_layout.Clear();
			
			m_layout.ForEach( delegate( List< layout_screen_data > _list ) 
			{ 
			    new_data.m_layout.Add( new List< layout_screen_data >() );
			    
				_list.ForEach( delegate( layout_screen_data _data ) 
				{
					new_data.m_layout[ new_data.m_layout.Count - 1 ].Add( _data.copy() );
				}); 
			});

			// copy targets
			{
				int scr_y;
				int scr_x;
				int ent_n;
				int targ_scr_y;
				int targ_scr_x;
				int targ_ent_n;
				
				bool target_find;
				
				entity_instance ent;
				entity_instance targ_ent;
				
				layout_screen_data scr_data;
				layout_screen_data targ_scr_data;
				
				for( scr_y = 0; scr_y < m_layout.Count; scr_y++ )
				{
					for( scr_x = 0; scr_x < m_layout[ scr_y ].Count; scr_x++ )
					{
						scr_data = m_layout[ scr_y ][ scr_x ];
						
						for( ent_n = 0; ent_n < scr_data.get_num_entities(); ent_n++ )
						{
							ent = scr_data.get_entity( ent_n );
							
							if( ent.target_uid >= 0 )
							{
								target_find = false;
								
								// get target id and screen pos
								for( targ_scr_y = 0; targ_scr_y < m_layout.Count; targ_scr_y++ )
								{
									for( targ_scr_x = 0; targ_scr_x < m_layout[ targ_scr_y ].Count; targ_scr_x++ )
									{
										targ_scr_data = m_layout[ targ_scr_y ][ targ_scr_x ];
										
										for( targ_ent_n = 0; targ_ent_n < targ_scr_data.get_num_entities(); targ_ent_n++ )
										{
											targ_ent = targ_scr_data.get_entity( targ_ent_n );
											
											if( ent.target_uid == targ_ent.uid )
											{
												new_data.m_layout[ scr_y ][ scr_x ].get_entity( ent_n ).target_uid = new_data.m_layout[ targ_scr_y ][ targ_scr_x ].get_entity( targ_ent_n ).uid;

												target_find = true;
												
												break;
											}
										}
										
										if( target_find )
										{
											break;
										}
									}
									
									if( target_find )
									{
										break;
									}
								}
							}
						}
					}
				}
			}
			
			new_data.set_start_screen_ind( get_start_screen_ind() );
			
			return new_data;
		}
		
		public int get_start_screen_ind()
		{
			return m_start_scr_ind;
		}
		
		public bool set_start_screen_ind( int _scr_ind )
		{
			if( _scr_ind >= 0 )
			{
				layout_screen_data scr_data = get_data( _scr_ind % get_width(), _scr_ind / get_width() );
				
				if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					m_start_scr_ind = _scr_ind;
					
					return true;
				}
			}
			else
			{
				m_start_scr_ind = _scr_ind;
				
				return true;
			}
			
			return false;
		}

		public bool set_screen_mark( int _scr_ind, int _mark )
		{
			if( _scr_ind >= 0 )
			{
				layout_screen_data scr_data = get_data( _scr_ind % get_width(), _scr_ind / get_width() );
				
				if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					scr_data.mark = ( ushort )_mark;
					
					return true;
				}
			}
			
			return false;
		}

		public bool set_adjacent_screen_mask( int _scr_ind, int _adj_scr_mask )
		{
			if( _scr_ind >= 0 )
			{
				layout_screen_data scr_data = get_data( _scr_ind % get_width(), _scr_ind / get_width() );
				
				if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					scr_data.adj_scr_mask = ( ushort )_adj_scr_mask;
					
					return true;
				}
			}
			
			return false;
		}
		
		public bool get_entity_by_uid( int _uid, ref int _scr_ind, ref entity_instance _ent )
		{
			bool res = false;
			
			entity_instance ent 		= null;
			int 			ent_scr_ind = 0;
			int 			scr_ind 	= 0;
			
			m_layout.ForEach( delegate( List< layout_screen_data > _list ) 
			{ 
				_list.ForEach( delegate( layout_screen_data _scr_data ) 
				{
					_scr_data.entities_proc( delegate( entity_instance _ent_inst ) 
					{
						if( _ent_inst.uid == _uid )
						{
							ent			= _ent_inst;
							ent_scr_ind	= scr_ind; 

							res = true;

							return;
						}
					});
					
					if( res )
					{
						return;
					}
					
					++scr_ind;
				});
				
				if( res )
				{
					return;
				}
			});
			
			_ent 		= ent;
			_scr_ind 	= ent_scr_ind;
			
			return res;
		}
		
		public void entity_instance_reset_target_uid( int _uid )
		{
			get_raw_data().ForEach( delegate( List< layout_screen_data > _scr_data_list )
			{ 
				_scr_data_list.ForEach( delegate( layout_screen_data _scr_data ) 
				{
					_scr_data.entities_proc( delegate( entity_instance _ent_inst )
					{
						if( _ent_inst.target_uid == _uid )
						{
							_ent_inst.target_uid = -1;
						}
					});
				}); 
			});
		}
		
		public void delete_screen_by_ind( int _scr_id )
		{
			delete_screen_by_pos( _scr_id % get_width(), _scr_id / get_width() );
		}
			
		public void delete_screen_by_pos( int _x, int _y )
		{
			delete_screen_by_data( get_data( _x, _y ), ( _y * get_width() + _x ) );
		}

		public void delete_screen_by_data( layout_screen_data _data, int _scr_ind )
		{
			_data.m_scr_ind = layout_data.CONST_EMPTY_CELL_ID;
			_data.mark = _data.adj_scr_mask = 0;
			
			_data.entities_proc( delegate( entity_instance _ent_inst ) { entity_instance_reset_target_uid( _ent_inst.uid ); _ent_inst.reset(); } );
			_data.clear_entities( false );

			if( get_start_screen_ind() == _scr_ind )
			{
				set_start_screen_ind( -1 );
			}
		}
		
		private List< layout_screen_data > allocate_row( int _width = -1 )
		{
			int width = ( _width < 0 ) ? get_width():_width;

			List< layout_screen_data > row = new List< layout_screen_data >( width );
			
			for( int i = 0; i < width; i++ )
			{
				row.Add( new layout_screen_data( CONST_EMPTY_CELL_ID ) );
			}
			
			return row;
		}
		
		public void  add_up()
		{
			if( m_start_scr_ind >= 0 )
			{
				m_start_scr_ind += get_width();
			}
			
			m_layout.Insert( 0, allocate_row() );
		}

		public bool remove_up()
		{
			if( get_height() > 1 )
			{
				int width = get_width();
				
				int start_scr_val = -1;
				
				if( m_start_scr_ind >= 0 )
				{
					if( m_start_scr_ind < width )
					{
						m_start_scr_ind = -1;
					}
					else
					{
						start_scr_val = m_start_scr_ind - width;
					}
				}

				for( int i = 0; i < width; i++ )
				{
					delete_screen_by_pos( i, 0 );
				}
				
				m_layout.RemoveAt( 0 );
				
				if( start_scr_val >= 0 )
				{
					m_start_scr_ind = start_scr_val; 
				}
				
				return true;
			}
			return false;
		}
		
		public void  add_down()
		{
			m_layout.Insert( get_height(), allocate_row() );
		}
		
		public bool  remove_down()
		{
			int width	= get_width();
			int height	= get_height();
			
			if( height > 1 )
			{
				if( m_start_scr_ind >= 0 )
				{
					if( ( m_start_scr_ind / height ) == height )
					{
						m_start_scr_ind = -1;
					}
				}
				
				for( int i = 0; i < width; i++ )
				{
					delete_screen_by_pos( i, height - 1 );
				}
				
				m_layout.RemoveAt( height - 1 );
				
				return true;
			}
			
			return false;
		}

		public void  add_left()
		{
			int height	= get_height();

			if( m_start_scr_ind >= 0 )
			{
				m_start_scr_ind += ( m_start_scr_ind / get_width() ) + 1;
			}
			
			for( int i = 0; i < height; i++ )
			{
				m_layout[ i ].Insert( 0, new layout_screen_data( CONST_EMPTY_CELL_ID ) );
			}
		}

		public bool remove_left()
		{
			int width	= get_width();
			int height	= get_height();
			
			if( width > 1 )
			{
				if( m_start_scr_ind >= 0 )
				{
					if( m_start_scr_ind % width == 0 )
					{
						m_start_scr_ind = -1;
					}
				}
				
				for( int i = 0; i < height; i++ )
				{
					delete_screen_by_pos( 0, i );
					
					m_layout[ i ].RemoveAt( 0 );
				}

				if( m_start_scr_ind >= 0 )
				{
					m_start_scr_ind -= ( m_start_scr_ind / ( get_width() + 1 ) ) + 1;
				}
				
				return true;
			}
			
			return false;
		}
		
		public void  add_right()
		{
			int width	= get_width();
			int height	= get_height();

			if( m_start_scr_ind >= 0 )
			{
				m_start_scr_ind += ( m_start_scr_ind / width );
			}
			
			for( int i = 0; i < height; i++ )
			{
				m_layout[ i ].Insert( width, new layout_screen_data( CONST_EMPTY_CELL_ID ) );
			}
		}
		
		public bool remove_right()
		{
			int width	= get_width();
			int height	= get_height();
			
			if( width > 1 )
			{
				int start_scr_val = -1;
				
				if( m_start_scr_ind >= 0 )
				{
					if( m_start_scr_ind % width == width - 1 )
					{
						m_start_scr_ind = -1;
					}
					else
					{
						start_scr_val = m_start_scr_ind - ( m_start_scr_ind / width );
					}
				}
				
				for( int i = 0; i < height; i++ )
				{
					delete_screen_by_pos( width - 1, i );
					
					m_layout[ i ].RemoveAt( width - 1 );
				}

				if( start_scr_val >= 0 )
				{
					m_start_scr_ind = start_scr_val; 
				}
				
				return true;
			}
			
			return false;
		}
		
		public int get_width()
		{
			return m_layout[ 0 ].Count;
		}

		public int get_height()
		{
			return m_layout.Count;
		}
		
		public List< List< layout_screen_data > > get_raw_data()
		{
			return m_layout;
		}
		
		public layout_screen_data get_data( int _x, int _y )
		{
			return m_layout[ _y ][ _x ];
		}

		public void set_data( layout_screen_data _data, int _x, int _y )
		{
			m_layout[ _y ][ _x ] = _data;
		}
		
		public void delete_all_entities()
		{
			layout_data_proc( delegate( layout_screen_data _data ) 
			{
				_data.entities_proc( delegate( entity_instance _ent_inst ) { _ent_inst.reset(); } );
				_data.clear_entities( false );
			}); 
		}
		
		public int delete_entity_instances( int _scr_slot_ind )
		{
			return delete_entity_instances( get_data( _scr_slot_ind % get_width(), _scr_slot_ind / get_width() ) );
		}
		
		public int delete_entity_instances( layout_screen_data _data )
		{
			int ent_n;
			int ents_cnt = _data.get_num_entities();
			
			entity_instance ent_inst;
			
			for( ent_n = 0; ent_n < ents_cnt; ent_n++ )
			{
				ent_inst = _data.get_entity( ent_n );
			
				entity_instance_reset_target_uid( ent_inst.uid );
				
				ent_inst.reset();
			}
			
			_data.clear_entities( false );
			
			return ents_cnt;
		}
		
		public int delete_entity_instances( string _base_ent_name )
		{
			int size;
			int ent_n;
			int ent_ind;
			int ents_cnt = 0;
			
			entity_instance ent_inst;
			
			List< int > ent_inds = new List< int >( 200 );
			
			layout_data_proc( delegate( layout_screen_data _data ) 
			{
				ent_inds.Clear();
				
				for( ent_n = 0; ent_n < _data.get_num_entities(); ent_n++ )
				{
					if( _data.get_entity( ent_n ).base_entity.name == _base_ent_name )
					{
						ent_inds.Add( ent_n );
					}
				}
				
				if( ent_inds.Count > 0 )
				{
					ents_cnt += ent_inds.Count;
					
					ent_inds.Reverse();
					
					size = ent_inds.Count;
					
					for( ent_n = 0; ent_n < size; ent_n++ )
					{
						ent_ind = ent_inds[ ent_n ];
						
						ent_inst = _data.get_entity( ent_ind );
					
						entity_instance_reset_target_uid( ent_inst.uid );
						
						ent_inst.reset();
						_data.remove_entity( ent_ind );
					}
				}
			}); 
			
			return ents_cnt;
		}
		
		public void delete_all_screen_marks()
		{
			m_start_scr_ind = -1;
			
			layout_data_proc( delegate( layout_screen_data _data )
			{ 
				_data.mark = 0;
				_data.adj_scr_mask = 0; }
			);
		}

		public int get_num_ent_instances()
		{
			int ent_cnt = 0;
			
			layout_data_proc( delegate( layout_screen_data _data )
			{ 
				ent_cnt += _data.get_num_entities();
			});
			
			return ent_cnt;
		}
		
		public int get_num_ent_instances_by_name( string _base_ent_name )
		{
			int cnt = 0;
			
			layout_data_proc( delegate( layout_screen_data _data )
			{
				for( int ent_n = 0; ent_n < _data.get_num_entities(); ent_n++ )
				{
					if( _data.get_entity( ent_n ).base_entity.name == _base_ent_name )
					{
						++cnt;
					}
				}
			});
			
			return cnt;
		}
		
		public void layout_data_proc( Action< layout_screen_data > _act )
		{
			m_layout.ForEach( delegate( List< layout_screen_data > _list ) 
			{ 
				_list.ForEach( delegate( layout_screen_data _data ) 
				{
					_act( _data );
				}); 
			});
		}
		
		public int export_asm( StreamWriter _sw, string _data_mark, string _define, string _def_num, string _def_addr, string _def_coord, string _num_pref, bool _export_scr_desc, bool _export_marks, bool _export_entities, bool _ent_coords_scr, layout_screen_data.EEntSortType _sort_type, bool _compact_layout = true )
		{
			int max_props_cnt = 0;
			int props_cnt;
			
			int x;
			int y;

			int width	= get_width();
			int height	= get_height();
			
			string data_str = "";

			// export layout
			{
				if( _define != null )
				{
					_sw.WriteLine( _define + " " + _data_mark + "_WScrCnt\t" + width + "\t; number of screens in width" );
					_sw.WriteLine( _define + " " + _data_mark + "_HScrCnt\t" + height + "\t; number of screens in height" );
				}
				else
				{
					_sw.WriteLine( _data_mark + "_WScrCnt\t= " + width + "\t; number of screens in width" );
					_sw.WriteLine( _data_mark + "_HScrCnt\t= " + height + "\t; number of screens in height" );
				}
				
				_sw.WriteLine( "\n" + _data_mark + "_Layout:\t" );

				for( y = 0; y < height; y++ )
				{
					data_str = "\t" + _def_addr + " ";
					
					if( _compact_layout )
					{
						for( x = 0; x < width; x++ )
						{
							data_str += ( get_data( x, y ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID ? _data_mark + "Scr" + ( y * width + x ):"0" ) + ( x < ( width - 1 ) ? ", ":"" );
						}
						
						_sw.WriteLine( data_str );
					}
					else
					{
						for( x = 0; x < width; x++ )
						{
							_sw.WriteLine( data_str + ( get_data( x, y ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID ? _data_mark + "Scr" + ( y * width + x ):"0" ) );
						}
					}
				}
				
				_sw.WriteLine( "" );
			}
			
			// export entities
			if( _export_scr_desc )
			{
				string lev_scr_id;
				
				bool enable_comments = true;
				
				int ent_n = 0;
				
				layout_screen_data scr_data;
				
				for( y = 0; y < height; y++ )
				{
					for( x = 0; x < width; x++ )
					{
						scr_data = get_data( x, y );
						
						if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
						{
							lev_scr_id 	= _data_mark + "Scr" + ( y * width + x );
							
							_sw.WriteLine( lev_scr_id + ":"  );
							
							if( _export_marks )
							{
								_sw.WriteLine( "\t" + _def_num + " " + utils.hex( _num_pref, scr_data.mark_adj_scr_mask ) + ( enable_comments ? "\t; (marks) bits: 7-4 - bit mask of user defined adjacent screens ( Down(7)-Right(6)-Up(5)-Left(4) ); 3-0 - screen property":"" )  );
							}
	
							if( _export_entities )
							{
								props_cnt = scr_data.export_entities_asm( _sw, ref ent_n, lev_scr_id + "EntsArr", _def_num, _def_addr, _def_coord, _num_pref, _ent_coords_scr, x, y, _sort_type, enable_comments );
								
								if( max_props_cnt < props_cnt )
								{
									max_props_cnt = props_cnt;
								}
							}
							
							enable_comments = false;
							
							_sw.WriteLine( "" );
						}
					}
				}
				
				if( _export_entities )
				{
					if( _define != null )
					{
						_sw.WriteLine( _define + " " + _data_mark + "_EntInstCnt\t" + get_num_ent_instances() + "\t; number of entities instances" );
					}
					else
					{
						_sw.WriteLine( _data_mark + "_EntInstCnt = " + get_num_ent_instances() + "\t; number of entities instances" );
					}
				}
			}
			
			return max_props_cnt;
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_start_scr_ind );
			
			int width	= get_width();
			int height	= get_height();

			_bw.Write( width );
			_bw.Write( height );
			
			for( int i = 0; i < height; i++ )
			{
				for( int j = 0; j < width; j++ )
				{
					m_layout[ i ][ j ].save( _bw );
				}
			}
		}
		
		public void load( BinaryReader _br, byte _ver, Func< string, entity_data > _get_ent, data_sets_manager.EScreenDataType _scr_type, bool _fix_ent_inside_scr )
		{
			int i;
			int j;
			
			m_start_scr_ind = _br.ReadInt32();
			
			int width	= _br.ReadInt32();
			int height	= _br.ReadInt32();
			
			m_layout.Clear();
			
			for( i = 0; i < height; i++ )
			{
				m_layout.Add( allocate_row( width ) );
			}
			
			for( i = 0; i < height; i++ )
			{
				for( j = 0; j < width; j++ )
				{
					m_layout[ i ][ j ].load( _br, _ver, _get_ent, _scr_type, _fix_ent_inside_scr );
				}
			}
		}

		public int get_adjacent_screen_index( int _scr_ind, int _x_offset, int _y_offset, bool _optimized_ind = false )
		{
			int scr_cnt_x = get_width();
			int scr_cnt_y = get_height();
			
			int scr_ind = _scr_ind + _x_offset + ( _y_offset * scr_cnt_x );
			
			if( scr_ind < 0 || scr_ind >= ( scr_cnt_x * scr_cnt_y ) )
			{
				return -1;
			}
			
			int base_scr_mod_x_ind = _scr_ind % scr_cnt_x;
			int base_scr_mod_y_ind = _scr_ind / scr_cnt_x;
			
			int scr_mod_x_ind = scr_ind % scr_cnt_x;
			int scr_mod_y_ind = scr_ind / scr_cnt_x;

			if( ( ( _x_offset == 1 && ( base_scr_mod_x_ind + _x_offset ) == scr_cnt_x ) || ( _x_offset == -1 && ( base_scr_mod_x_ind + _x_offset ) < 0 ) ) )
			{
				return -1;
			}
			
			if( scr_mod_x_ind == ( base_scr_mod_x_ind - 1 ) || scr_mod_x_ind == base_scr_mod_x_ind || scr_mod_x_ind == ( base_scr_mod_x_ind + 1 ) )
			{
				if( scr_mod_y_ind == ( base_scr_mod_y_ind - 1 ) || scr_mod_y_ind == base_scr_mod_y_ind || scr_mod_y_ind == ( base_scr_mod_y_ind + 1 ) )
				{
					if( get_data( scr_mod_x_ind, scr_mod_y_ind ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						if( _optimized_ind )
						{
							// get optimized screen index without 'holes'
							return get_screen_index_opt( scr_mod_y_ind * scr_cnt_x + scr_mod_x_ind );
						}
						else
						{
							return scr_mod_y_ind * scr_cnt_x + scr_mod_x_ind;
						}
					}
				}
			}
			
			return -1;
		}
		
		private int get_screen_index_opt( int _scr_ind )
		{
			int i;
			int j;
			
			int opt_scr_ind = -1;
			
			int width	= get_width();
			int height	= get_height();
			
			for( i = 0; i < height; i++ )
			{
				for( j = 0; j < width; j++ )
				{
					if( m_layout[ i ][ j ].m_scr_ind != CONST_EMPTY_CELL_ID )
					{
						++opt_scr_ind;

						if( ( ( i * width ) + j ) == _scr_ind )
						{
							return opt_scr_ind; 
						}
					}
				}
			}
			
			return -1;
		}
	}
}
