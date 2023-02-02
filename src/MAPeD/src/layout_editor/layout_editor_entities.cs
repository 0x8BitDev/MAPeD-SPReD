/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 20.10.2022
 * Time: 16:09
 */
#define DEF_SNAPPING_BY_PIVOT	// otherwise snapping by origin
 
using System;
using System.Windows.Forms;

using SkiaSharp;

namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor_entities.
	/// </summary>
	public class layout_editor_entities : layout_editor_behaviour_base
	{
		private event EventHandler EntityInstanceSelected;
		private event EventHandler EditEntityCancel;

		private entity_data			m_ent_data						= null;
		private entity_instance		m_ent_inst						= null;
		private int					m_ent_inst_init_screen_slot_id	= -1;

		private int 	m_ent_inst_screen_slot_id	= -1;
		
		private bool 	m_ent_inst_captured			= false;

		private int		m_ent_inst_capture_offs_x	= -1;
		private int		m_ent_inst_capture_offs_y	= -1;

		private readonly int[] 	m_adj_scr_ind_arr;

		private bool	m_ent_snapping				= true;
		
		private bool entity_snapping
		{
			get { return m_ent_snapping; }
			set { m_ent_snapping = value; }
		}

		private uint	m_ent_mode			= 0xff;
		private int		m_map_ents_cnt		= 0;
		private bool	m_need_upd_ents_cnt	= false;
		
		public layout_editor_entities( string _name, layout_editor_shared_data _shared, layout_editor_base _owner ) : base( _name, _shared, _owner )
		{
			m_adj_scr_ind_arr = new int[]{ -1, /*( -width - 1 )*/0, /*-width*/0, /*( -width + 1 )*/0, +1, /*( width + 1 )*/0, /*width*/0, /*( width - 1 )*/0, -1 };
		}
		
		public override void reset( bool _init )
		{
			if( _init )
			{
				set_param( layout_editor_param.CONST_SET_ENT_UPDATE_ENTS_CNT, null );
			}
			
			set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
			set_param( layout_editor_param.CONST_SET_ENT_ACTIVE, null );
			set_param( layout_editor_param.CONST_SET_ENT_INST_EDIT, null );
		}
		
		public override bool mouse_down( object sender, MouseEventArgs e )
		{
			m_ent_inst_captured = false;
			
			bool entity_picked = false;
			
			uint last_ent_mode = m_ent_mode;
			
			if( m_owner.show_entities && ( last_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT || last_ent_mode == layout_editor_param.CONST_SET_ENT_SELECT_TARGET ) )
			{
				// select entity
				if( m_shared.m_sel_screen_slot_id >= 0 )
				{
					int scr_pos_x = m_shared.get_sel_scr_pos_x();
					int scr_pos_y = m_shared.get_sel_scr_pos_y();
					
					int scr_pos_x_pix = ( int )m_shared.screen_pos_x_by_slot_id( scr_pos_x );
					int scr_pos_y_pix = ( int )m_shared.screen_pos_y_by_slot_id( scr_pos_y );
					
					int mouse_scr_pos_x = ( int )( ( e.X - scr_pos_x_pix ) / m_shared.m_scale );
					int mouse_scr_pos_y = ( int )( ( e.Y - scr_pos_y_pix ) / m_shared.m_scale );
					
					if( !( entity_picked = select_entity( mouse_scr_pos_x, mouse_scr_pos_y, scr_pos_x, scr_pos_y, m_shared.m_sel_screen_slot_id ) ) )
					{
						// try to pick at adjacent screens
						int scr_cnt_x = m_shared.m_layout.get_width();
						int scr_cnt_y = m_shared.m_layout.get_height();
						
						int max_scr_cnt = scr_cnt_x * scr_cnt_y;
						
						m_adj_scr_ind_arr[ 1 ] = -scr_cnt_x - 1;
						m_adj_scr_ind_arr[ 2 ] = -scr_cnt_x;
						m_adj_scr_ind_arr[ 3 ] = -scr_cnt_x + 1;

						m_adj_scr_ind_arr[ 5 ] = scr_cnt_x + 1;
						m_adj_scr_ind_arr[ 6 ] = scr_cnt_x;
						m_adj_scr_ind_arr[ 7 ] = scr_cnt_x - 1;
						
						int arr_ind = 0;
						
						if( mouse_scr_pos_x > platform_data.get_screen_width_pixels() >> 1 )
						{
							arr_ind |= 0x01;
						}
						
						if( mouse_scr_pos_y > platform_data.get_screen_height_pixels() >> 1 )
						{
							arr_ind |= 0x02;
							
							arr_ind ^= 0x01;
						}
						
						arr_ind <<= 1;
						
						int last_arr_ind = arr_ind + 3;
						
						int scr_ind;
						int scr_mod_x_ind;
						int scr_mod_y_ind;
						int sel_scr_mod_x_ind = m_shared.m_sel_screen_slot_id % scr_cnt_x;
						int sel_scr_mod_y_ind = m_shared.m_sel_screen_slot_id / scr_cnt_x;
						
						for( int i = arr_ind; i < last_arr_ind; i++ )
						{
							scr_ind = m_shared.m_sel_screen_slot_id + m_adj_scr_ind_arr[ i ];
							
							if( scr_ind < 0 || scr_ind >= max_scr_cnt )
							{
								continue;
							}
							
							scr_mod_x_ind = scr_ind % scr_cnt_x;
							scr_mod_y_ind = scr_ind / scr_cnt_x;
							
							if( scr_mod_x_ind == ( sel_scr_mod_x_ind - 1 ) || scr_mod_x_ind == sel_scr_mod_x_ind || scr_mod_x_ind == ( sel_scr_mod_x_ind + 1 ) )
							{
								if( scr_mod_y_ind == ( sel_scr_mod_y_ind - 1 ) || scr_mod_y_ind == sel_scr_mod_y_ind || scr_mod_y_ind == ( sel_scr_mod_y_ind + 1 ) )
								{
									//System.Diagnostics.Debug.WriteLine( scr_ind );
									
									// convert a cursor position into checked screen space
									scr_pos_x_pix = ( int )m_shared.screen_pos_x_by_slot_id( scr_mod_x_ind );
									scr_pos_y_pix = ( int )m_shared.screen_pos_y_by_slot_id( scr_mod_y_ind );
									
									mouse_scr_pos_x = ( int )( ( e.X - scr_pos_x_pix ) / m_shared.m_scale );
									mouse_scr_pos_y = ( int )( ( e.Y - scr_pos_y_pix ) / m_shared.m_scale );
									
									if( ( entity_picked = select_entity( mouse_scr_pos_x, mouse_scr_pos_y, scr_mod_x_ind, scr_mod_y_ind, scr_ind ) ) == true )
									{
										break;
									}
								}
							}
						}
					}
				}
				
				if( last_ent_mode == layout_editor_param.CONST_SET_ENT_SELECT_TARGET )
				{
					if( !entity_picked )
					{
                		if( EntityInstanceSelected != null )
                		{
                			EntityInstanceSelected( this, new EventArg2Params( null, null ) );
                		}
						
						m_owner.update();
					}
				}
				else
				if( !m_ent_inst_captured && m_ent_inst != null )
				{
					set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
					
            		if( EntityInstanceSelected != null )
            		{
            			EntityInstanceSelected( this, new EventArg2Params( m_ent_inst, null ) );
            		}
					
					m_owner.update();
				}
			}
			/*
			if( !m_ent_inst_captured )
			{
				m_shared.m_last_mouse_x	 = e.X;
				m_shared.m_last_mouse_y	 = e.Y;
				
				int width_scaled	= ( int )( m_shared.m_scale * m_shared.m_layout.get_width() * platform_data.get_screen_width_pixels() );
				int height_scaled	= ( int )( m_shared.m_scale * m_shared.m_layout.get_height() * platform_data.get_screen_height_pixels() );
				
				if( width_scaled > m_shared.pix_box_width() || height_scaled > m_shared.pix_box_height() )
				{
					m_shared.set_high_quality_render_mode( false );
				}
			}*/
			
			return true;
		}
		
		private bool select_entity( int _cursor_pos_x, int _cursor_pos_y, int _scr_pos_x, int _scr_pos_y, int _scr_ind )
		{
			entity_instance ent_inst;
			
			bool res = false;  
			
			layout_screen_data scr_data = m_shared.m_layout.get_data( _scr_pos_x, _scr_pos_y );

			bool valid_pos = ( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
			
			if( valid_pos == true )
			{
				for( int ent_n = scr_data.get_num_entities() - 1; ent_n >= 0; --ent_n )
                {
					ent_inst = scr_data.get_entity( ent_n );
					
                	if( _cursor_pos_x >= ent_inst.x && _cursor_pos_x <= ent_inst.x + ent_inst.base_entity.width )
                	{
                    	if( _cursor_pos_y >= ent_inst.y && _cursor_pos_y <= ent_inst.y + ent_inst.base_entity.height )
                    	{
                    		res = true;
                    		
                    		if( m_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT )
                    		{
								m_ent_inst						= ent_inst;
								
								m_ent_inst_init_screen_slot_id	= _scr_ind;
								m_ent_inst_screen_slot_id 		= m_shared.m_sel_screen_slot_id;
#if DEF_SNAPPING_BY_PIVOT
								m_ent_inst_capture_offs_x	= _cursor_pos_x - ( ent_inst.x + ent_inst.base_entity.pivot_x );
								m_ent_inst_capture_offs_y	= _cursor_pos_y - ( ent_inst.y + ent_inst.base_entity.pivot_y );
#else
								m_ent_inst_capture_offs_x	= _cursor_pos_x - ent_inst.x;
								m_ent_inst_capture_offs_y	= _cursor_pos_y - ent_inst.y;
#endif                  		
								m_ent_inst_captured = true;
								
								m_shared.pix_box_reset_capture();
							}

                    		if( EntityInstanceSelected != null )
                    		{
                    			EntityInstanceSelected( this, new EventArg2Params( ent_inst, null ) );
                    		}
                    		
                    		m_owner.update();
                    		
                    		break;
                    	}
                	}
				}
			}
			
			return res;
		}
		
		public override void mouse_up( object sender, MouseEventArgs e )
		{
			if( m_shared.m_sel_screen_slot_id < 0 )
			{
				if( m_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT )
				{
					// delete dragged entity if it releases out of a map
					if( m_ent_inst != null )
					{
						get_ent_inst_init_screen_data().remove_entity( m_ent_inst );
						
						set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
						
						if( EntityInstanceSelected != null )
						{
							EntityInstanceSelected( this, new EventArg2Params( m_ent_inst, null ) );
						}
					}
				}
			}
			
			if( m_owner.show_entities )
			{
				if( m_ent_mode == layout_editor_param.CONST_SET_ENT_EDIT )
				{
					place_new_entity_instance( m_ent_data );
				}
				else
				if( m_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT )
				{
					if( m_ent_inst != null )
					{
						m_ent_inst_captured = false;
						
						bool ent_placed = false;
						
						if( m_shared.m_sel_screen_slot_id >= 0 )
						{
							if( place_old_entity_instance( m_ent_inst ) == true )
							{
								m_ent_inst_init_screen_slot_id = m_ent_inst_screen_slot_id = m_shared.m_sel_screen_slot_id;
								
								ent_placed = true;
							}
						}
						
						if( !ent_placed )
						{
							set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
							
							if( EntityInstanceSelected != null )
							{
								EntityInstanceSelected( this, new EventArg2Params( m_ent_inst, null ) );
							}
						}
					}
				}
			}
			else
			{
				m_owner.update();
			}
		}
		
		private bool place_new_entity_instance( entity_data _ent_data )
		{
			if( _ent_data != null && m_shared.m_sel_screen_slot_id >= 0 )
			{
#if DEF_SNAPPING_BY_PIVOT
				int ent_pos_x = m_shared.m_mouse_x;
				int ent_pos_y = m_shared.m_mouse_y;
#else
				int ent_pivot_x	= ( int )( _ent_data.pivot_x * m_scale );
				int ent_pivot_y = ( int )( _ent_data.pivot_y * m_scale );

				int ent_pos_x = m_mouse_x - ent_pivot_x;
				int ent_pos_y = m_mouse_y - ent_pivot_y;
#endif				
				int scr_pos_x = m_shared.get_sel_scr_pos_x();
				int scr_pos_y = m_shared.get_sel_scr_pos_y();
				
				layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
				
				bool valid_pos = ( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
				
				if( valid_pos == true && check_entity_pos( ref ent_pos_x, ref ent_pos_y, _ent_data ) )
				{
					int scr_pos_x_pix = ( int )m_shared.screen_pos_x_by_slot_id( scr_pos_x );
					int scr_pos_y_pix = ( int )m_shared.screen_pos_y_by_slot_id( scr_pos_y );
					
					ent_pos_x = ( int )( ( ent_pos_x - scr_pos_x_pix ) / m_shared.m_scale );
					ent_pos_y = ( int )( ( ent_pos_y - scr_pos_y_pix ) / m_shared.m_scale );
					
					get_snapped_pos( ref ent_pos_x, ref ent_pos_y, false );

#if DEF_SNAPPING_BY_PIVOT
					ent_pos_x -= _ent_data.pivot_x;
					ent_pos_y -= _ent_data.pivot_y;
#endif					
					scr_data.add_entity( new entity_instance( m_ent_data.inst_properties, ent_pos_x, ent_pos_y, m_ent_data ) );
					
					m_owner.update();
					
					return true;
				}
			}
			
			return false;
		}

		private bool place_old_entity_instance( entity_instance _ent_inst )
		{
			if( _ent_inst != null && m_shared.m_sel_screen_slot_id >= 0 )
			{
				int capt_pos_x = ( int )( m_ent_inst_capture_offs_x * m_shared.m_scale );
				int capt_pos_y = ( int )( m_ent_inst_capture_offs_y * m_shared.m_scale );
				
				int ent_pos_x = m_shared.m_mouse_x - capt_pos_x;
				int ent_pos_y = m_shared.m_mouse_y - capt_pos_y;

				get_snapped_pos( ref ent_pos_x, ref ent_pos_y );

				int scr_pos_x = m_shared.get_sel_scr_pos_x();
				int scr_pos_y = m_shared.get_sel_scr_pos_y();
				
				layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
				
				bool valid_pos = ( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
				
				if( valid_pos == true && check_entity_pos( ref ent_pos_x, ref ent_pos_y, _ent_inst.base_entity ) )
				{
					int scr_pos_x_pix = ( int )m_shared.screen_pos_x_by_slot_id( scr_pos_x );
					int scr_pos_y_pix = ( int )m_shared.screen_pos_y_by_slot_id( scr_pos_y );
					
					ent_pos_x = ( int )( ( ent_pos_x - scr_pos_x_pix ) / m_shared.m_scale );
					ent_pos_y = ( int )( ( ent_pos_y - scr_pos_y_pix ) / m_shared.m_scale );
					
			    	get_snapped_pos( ref ent_pos_x, ref ent_pos_y, false );  

#if DEF_SNAPPING_BY_PIVOT
			    	_ent_inst.x = ent_pos_x - _ent_inst.base_entity.pivot_x;
			    	_ent_inst.y = ent_pos_y - _ent_inst.base_entity.pivot_y;
#else			    	
			    	_ent_inst.x = ent_pos_x;
			    	_ent_inst.y = ent_pos_y;
#endif
					layout_screen_data ent_inst_init_scr_data = get_ent_inst_init_screen_data();
					
					if( ent_inst_init_scr_data != scr_data )
					{
						// remove an entity from the old screen...
						ent_inst_init_scr_data.remove_entity( _ent_inst );
						// ... and add to the new one
						scr_data.add_entity( _ent_inst );
					}
					
					m_owner.update();
					
					return true;
				}
			}
			
			return false;
		}

		private bool check_entity_pos( ref int _ent_pos_x, ref int _ent_pos_y, entity_data _ent )
		{
			bool valid_pos = true;
			
			int layout_width 	= (int)( m_shared.m_layout.get_width() * platform_data.get_screen_width_pixels() );
			int layout_height 	= (int)( m_shared.m_layout.get_height() * platform_data.get_screen_height_pixels() );
			
			int ent_img_pos_x = m_shared.transform_to_img_pos( _ent_pos_x, m_shared.m_offset_x, m_shared.m_scr_half_width );
			int ent_img_pos_y = m_shared.transform_to_img_pos( _ent_pos_y, m_shared.m_offset_y, m_shared.m_scr_half_height );
			
			if( ( ent_img_pos_x - _ent.pivot_x ) < 0 )
			{
				_ent_pos_x = ( int )m_shared.transform_to_scr_pos( _ent.pivot_x + m_shared.m_offset_x, m_shared.m_scr_half_width );
			}
			
			if( ( ent_img_pos_x - _ent.pivot_x ) + _ent.width > layout_width )
			{
				_ent_pos_x = ( int )m_shared.transform_to_scr_pos( ( layout_width - _ent.width + _ent.pivot_x ) + m_shared.m_offset_x, m_shared.m_scr_half_width );
			}
			
			if( ( ent_img_pos_y - _ent.pivot_y ) < 0 )
			{
				_ent_pos_y = ( int )m_shared.transform_to_scr_pos( _ent.pivot_y + m_shared.m_offset_y, m_shared.m_scr_half_height );
			}
			
			if( ( ent_img_pos_y - _ent.pivot_y ) + _ent.height > layout_height )
			{
				_ent_pos_y = ( int )m_shared.transform_to_scr_pos( ( layout_height - _ent.height + _ent.pivot_y ) + m_shared.m_offset_y, m_shared.m_scr_half_height );
			}
			
			return valid_pos;
		}

		private void get_snapped_pos( ref int _x, ref int _y, bool _pix_space = true )
		{
			if( entity_snapping )
			{
				int img_x = _x;
				int img_y = _y;
				
				if( _pix_space == true )
				{
					img_x = m_shared.transform_to_img_pos( _x, m_shared.m_offset_x, m_shared.m_scr_half_width );
					img_y = m_shared.transform_to_img_pos( _y, m_shared.m_offset_y, m_shared.m_scr_half_height );
				}
				
				img_x = ( ( img_x >> 3 ) + ( ( img_x % 8 ) > 4 ? 1:0 ) ) << 3;
				img_y = ( ( img_y >> 3 ) + ( ( img_y % 8 ) > 4 ? 1:0 ) ) << 3;
				
				if( _pix_space == true )
				{
					_x = ( int )m_shared.transform_to_scr_pos( img_x, m_shared.m_scr_half_width );
					_y = ( int )m_shared.transform_to_scr_pos( img_y, m_shared.m_scr_half_height );
					
					_x += ( int )( m_shared.m_offset_x * m_shared.m_scale );
					_y += ( int )( m_shared.m_offset_y * m_shared.m_scale );
				}
				else
				{
					_x = img_x;
					_y = img_y;
				}
			}
		}

		private layout_screen_data get_ent_inst_init_screen_data()
		{
			return m_shared.m_layout.get_data( m_ent_inst_init_screen_slot_id % m_shared.m_layout.get_width(), m_ent_inst_init_screen_slot_id / m_shared.m_layout.get_width() );
		}

		public override bool mouse_move( object sender, MouseEventArgs e )
		{
			if( m_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT && m_ent_inst_captured && m_ent_inst != null )
			{
				m_ent_inst_screen_slot_id = m_shared.m_sel_screen_slot_id;
			}
			
			return true;
		}
		
		public override void mouse_enter( object sender, EventArgs e )
		{
			//...
		}

		public override void mouse_leave( object sender, EventArgs e )
		{
			if( m_owner.show_entities )
			{
				if(	m_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT && m_ent_inst_captured == true )
				{
					set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
					
	        		if( EntityInstanceSelected != null )
	        		{
	        			EntityInstanceSelected( this, new EventArg2Params( m_ent_inst, null ) );
	        		}
				}
			}
		}
		
		public override void mouse_wheel( object sender, EventArgs e )
		{
			//...
		}
		
		public override layout_editor_base.EHelper	default_helper()
		{
			return layout_editor_base.EHelper.eh_Unknown;
		}
		
		public override bool force_map_drawing()
		{
			return false;
		}

		private void draw_pivot( int _pivot_x, int _pivot_y, SKCanvas _canvas, SKPaint _line_paint )
		{
			_line_paint.Color = utils.CONST_COLOR_ENTITY_PIVOT;
			{
				int line_scale = ( int )( 8 * m_shared.m_scale );
				
				_canvas.DrawLine( _pivot_x - line_scale, _pivot_y, _pivot_x + line_scale, _pivot_y, _line_paint );
				_canvas.DrawLine( _pivot_x, _pivot_y - line_scale, _pivot_x, _pivot_y + line_scale, _line_paint );
			}
		}
		
		private void show_pivot_coords( int _ent_pivot_x, int _ent_pivot_y, int _ent_scr_pos_x, int _ent_scr_pos_y )
		{
			m_shared.print( "(" + _ent_pivot_x + "," + _ent_pivot_y + ")", _ent_scr_pos_x, _ent_scr_pos_y - 15 );
		}
		
		public override void draw( SKSurface _surface, SKPaint _line_paint, SKPaint _image_paint, float _scr_size_width, float _scr_size_height )
		{
			int x;
			int y;
			
			if( m_owner.show_entities )
			{
				if( m_ent_mode == layout_editor_param.CONST_SET_ENT_EDIT )
				{
					if( m_ent_data != null )
					{
						m_shared.sys_msg( "'Esc' - edit instances" );
						
						int ent_width	= ( int )( m_ent_data.width * m_shared.m_scale );
						int ent_height 	= ( int )( m_ent_data.height * m_shared.m_scale );
						
						int ent_pivot_x	= ( int )( m_ent_data.pivot_x * m_shared.m_scale );
						int ent_pivot_y = ( int )( m_ent_data.pivot_y * m_shared.m_scale );
					
#if DEF_SNAPPING_BY_PIVOT
						int ent_pos_x = m_shared.m_mouse_x;
						int ent_pos_y = m_shared.m_mouse_y;
#else
						int ent_pos_x = m_mouse_x - ent_pivot_x;
						int ent_pos_y = m_mouse_y - ent_pivot_y;
#endif
						get_snapped_pos( ref ent_pos_x, ref ent_pos_y );
						
#if DEF_SNAPPING_BY_PIVOT
						ent_pos_x -= ent_pivot_x;
						ent_pos_y -= ent_pivot_y;
#endif					
						if( m_shared.m_sel_screen_slot_id >= 0 && m_shared.m_layout.get_data( m_shared.get_sel_scr_pos_x(), m_shared.get_sel_scr_pos_y() ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
						{
							utils.draw_skbitmap( _surface.Canvas, m_shared.m_image_cache.get( m_ent_data.bitmap ), ent_pos_x, ent_pos_y, ent_width, ent_height, _image_paint );
							
							_line_paint.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_ENT_MODE;
							_line_paint.StrokeWidth = 2;
							{
								_surface.Canvas.DrawRect( ent_pos_x, ent_pos_y, ent_width, ent_height, _line_paint );
							}
							
							draw_pivot( ent_pos_x + ent_pivot_x, ent_pos_y + ent_pivot_y, _surface.Canvas, _line_paint );
							
							if( m_owner.show_coords )
							{
								int img_ent_pos_x = m_shared.transform_to_img_pos( ent_pos_x, m_shared.m_offset_x, m_shared.m_scr_half_width );
								int img_ent_pos_y = m_shared.transform_to_img_pos( ent_pos_y, m_shared.m_offset_y, m_shared.m_scr_half_height );
								
								show_pivot_coords( img_ent_pos_x + m_ent_data.pivot_x, img_ent_pos_y + m_ent_data.pivot_y, ent_pos_x, ent_pos_y );
							}
						}
						else
						{
							_line_paint.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_ENT_MODE;
							_line_paint.StrokeWidth = 2;
							{
								_surface.Canvas.DrawRect( ent_pos_x, ent_pos_y, ent_width, ent_height, _line_paint );
							}
							
							draw_pivot( ent_pos_x + ent_pivot_x, ent_pos_y + ent_pivot_y, _surface.Canvas, _line_paint );
						}
					}
					
					m_shared.print( "edit: entities", 0, 10 );
				}
				else
				if( m_ent_mode == layout_editor_param.CONST_SET_ENT_INST_EDIT )
				{
					if( m_ent_inst != null )
					{
						int capt_pos_x = m_ent_inst_captured ? ( m_shared.m_mouse_x - ( int )( m_ent_inst_capture_offs_x * m_shared.m_scale ) ):0;
						int capt_pos_y = m_ent_inst_captured ? ( m_shared.m_mouse_y - ( int )( m_ent_inst_capture_offs_y * m_shared.m_scale ) ):0;
						
						int pivot_x = ( int )( m_ent_inst.base_entity.pivot_x * m_shared.m_scale );
						int pivot_y = ( int )( m_ent_inst.base_entity.pivot_y * m_shared.m_scale );
						
						if( m_ent_inst_captured )
						{
							get_snapped_pos( ref capt_pos_x, ref capt_pos_y );
#if DEF_SNAPPING_BY_PIVOT
							capt_pos_x -= pivot_x;
							capt_pos_y -= pivot_y;
#endif
						}
						
						int ent_width	= ( int )( m_ent_inst.base_entity.width * m_shared.m_scale );
						int ent_height 	= ( int )( m_ent_inst.base_entity.height * m_shared.m_scale );
						
						if( m_ent_inst_screen_slot_id >= 0 )
						{
							_line_paint.StrokeWidth = 2;
							{
								int scr_pos_x = m_ent_inst_screen_slot_id % m_shared.m_layout.get_width();
								int scr_pos_y = m_ent_inst_screen_slot_id / m_shared.m_layout.get_width();
								
								if( m_ent_inst_captured )
								{
									// draw active screen border
									{
										x = ( int )m_shared.screen_pos_x_by_slot_id( m_shared.get_sel_scr_pos_x() );
										y = ( int )m_shared.screen_pos_y_by_slot_id( m_shared.get_sel_scr_pos_y() );
										
										_line_paint.Color = utils.CONST_COLOR_SCREEN_ACTIVE;
										_surface.Canvas.DrawRect( x, y, _scr_size_width, _scr_size_height, _line_paint );
									}

									// draw initial entity position as a border
									{
										_line_paint.Color = utils.CONST_COLOR_SELECTED_ENTITY_BORDER;
										
										x = ( int )m_shared.screen_pos_x_by_slot_id( m_ent_inst_init_screen_slot_id % m_shared.m_layout.get_width() );
										y = ( int )m_shared.screen_pos_y_by_slot_id( m_ent_inst_init_screen_slot_id / m_shared.m_layout.get_width() );
										
										_surface.Canvas.DrawRect( x + ( m_ent_inst.x * m_shared.m_scale ), y + ( m_ent_inst.y * m_shared.m_scale ), ent_width, ent_height, _line_paint );
									}
									
									_line_paint.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_INST_MODE;
									_surface.Canvas.DrawRect( capt_pos_x, capt_pos_y, ent_width, ent_height, _line_paint );
									
									draw_pivot( capt_pos_x + pivot_x, capt_pos_y + pivot_y, _surface.Canvas, _line_paint );
									
									if( m_owner.show_coords )
									{
										int img_capt_pos_x = m_shared.transform_to_img_pos( capt_pos_x, m_shared.m_offset_x, m_shared.m_scr_half_width );
										int img_capt_pos_y = m_shared.transform_to_img_pos( capt_pos_y, m_shared.m_offset_y, m_shared.m_scr_half_height );
										
										show_pivot_coords( img_capt_pos_x + m_ent_inst.base_entity.pivot_x, img_capt_pos_y + m_ent_inst.base_entity.pivot_y, capt_pos_x, capt_pos_y );
									}
								}
								else
								{
									_line_paint.Color = utils.CONST_COLOR_SELECTED_ENTITY_BORDER;
									
									int scr_pos_x_pix = ( int )m_shared.screen_pos_x_by_slot_id( scr_pos_x );
									int scr_pos_y_pix = ( int )m_shared.screen_pos_y_by_slot_id( scr_pos_y );
									
									int ent_scr_pos_x = scr_pos_x_pix + ( int )( m_ent_inst.x * m_shared.m_scale );
									int ent_scr_pos_y = scr_pos_y_pix + ( int )( m_ent_inst.y * m_shared.m_scale );
									
									_surface.Canvas.DrawRect( ent_scr_pos_x, ent_scr_pos_y, ent_width, ent_height, _line_paint );
									
									draw_pivot( ent_scr_pos_x + pivot_x, ent_scr_pos_y + pivot_y, _surface.Canvas, _line_paint );
									
									if( m_owner.show_coords )
									{
										show_pivot_coords( ( scr_pos_x * platform_data.get_screen_width_pixels() ) + m_ent_inst.x + m_ent_inst.base_entity.pivot_x, ( scr_pos_y * platform_data.get_screen_height_pixels() ) + m_ent_inst.y + m_ent_inst.base_entity.pivot_y, ent_scr_pos_x, ent_scr_pos_y );
									}
								}
							}
						}
						else
						{
							if( m_ent_inst_captured )
							{
								_line_paint.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_INST_MODE;
								_line_paint.StrokeWidth = 2;
								{
									_surface.Canvas.DrawRect( capt_pos_x, capt_pos_y, ent_width, ent_height, _line_paint );
								}
							}
						}
					}
					
					m_shared.print( "edit: instances", 0, 10 );
				}
				else
				if( m_ent_mode == layout_editor_param.CONST_SET_ENT_SELECT_TARGET )
				{
					m_shared.sys_msg( "'Esc' - edit instances" );
					
					if( m_ent_inst != null && m_shared.m_high_quality_render )
					{
						int ent_width	= ( int )( m_ent_inst.base_entity.width * m_shared.m_scale );
						int ent_height 	= ( int )( m_ent_inst.base_entity.height * m_shared.m_scale );
						
						if( m_ent_inst_screen_slot_id >= 0 )
						{
							int scr_pos_x = m_ent_inst_screen_slot_id % m_shared.m_layout.get_width();
							int scr_pos_y = m_ent_inst_screen_slot_id / m_shared.m_layout.get_width();
							
							int scr_pos_x_pix = ( int )m_shared.screen_pos_x_by_slot_id( scr_pos_x );
							int scr_pos_y_pix = ( int )m_shared.screen_pos_y_by_slot_id( scr_pos_y );
							
							int ent_scr_pos_x = scr_pos_x_pix + ( int )( m_ent_inst.x * m_shared.m_scale );
							int ent_scr_pos_y = scr_pos_y_pix + ( int )( m_ent_inst.y * m_shared.m_scale );
							
							_line_paint.Color = utils.CONST_COLOR_SELECTED_ENTITY_BORDER;
							_line_paint.StrokeWidth = 2;
							{
								_surface.Canvas.DrawRect( ent_scr_pos_x, ent_scr_pos_y, ent_width, ent_height, _line_paint );
							}
							
							int pivot_x = ( int )( m_ent_inst.base_entity.pivot_x * m_shared.m_scale );
							int pivot_y = ( int )( m_ent_inst.base_entity.pivot_y * m_shared.m_scale );
							
							draw_pivot( ent_scr_pos_x + pivot_x, ent_scr_pos_y + pivot_y, _surface.Canvas, _line_paint );
							
							if( m_owner.show_coords )
							{
								show_pivot_coords( ( scr_pos_x * platform_data.get_screen_width_pixels() ) + m_ent_inst.x + m_ent_inst.base_entity.pivot_x, ( scr_pos_y * platform_data.get_screen_height_pixels() ) + m_ent_inst.y + m_ent_inst.base_entity.pivot_y, ent_scr_pos_x, ent_scr_pos_y );
							}
						}
					}
					
					m_shared.print( "select target entity", 0, 10 );
				}
				
				if( m_need_upd_ents_cnt )
				{
					m_map_ents_cnt = m_shared.m_layout.get_num_ent_instances();
					
					m_need_upd_ents_cnt = false;
				}
				
				m_shared.print( "[" + m_map_ents_cnt + "]", 80, 0 );
			}
			else
			{
				m_shared.print( "entities: off", 0, 10 );
			}
		}

		public bool delete_entity_instance_from_layout()
		{
			bool res = false;
			
			if( m_ent_inst != null )
			{
				if( m_ent_inst_screen_slot_id >= 0 )
				{
					if( MainForm.message_box( "Are you sure?", "Delete Entity Instance", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == DialogResult.Yes )
					{
						layout_screen_data scr_data = m_shared.m_layout.get_data( m_ent_inst_screen_slot_id % m_shared.m_layout.get_width(), m_ent_inst_screen_slot_id / m_shared.m_layout.get_width() );
					
						if( scr_data.remove_entity( m_ent_inst ) == false )
						{
							MainForm.message_box( "Unexpected error!\n\nCan't delete the entity!", "Delete Entity Instance", MessageBoxButtons.OK, MessageBoxIcon.Error );
						}
						else
						{
							m_shared.m_layout.entity_instance_reset_target_uid( m_ent_inst.uid );
							
							set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
							
							res = true;
							
							m_owner.update();
						}
					}
				}
			}
			else
			{
				MainForm.message_box( "Please, select an entity!", "Delete Entity Instance", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			return res;
		}

		public override Object get_param( uint _param )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_GET_ENT_INST_SELECTED:
				{
					return m_ent_inst;
				}
				
				case layout_editor_param.CONST_GET_ENT_MODE:
				{
					return m_ent_mode;
				}
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_entities.get_param]" );
				}
			}
		}
		
		public override bool set_param( uint _param, Object _val )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SET_ENT_INST_RESET:
					{
						m_ent_inst 						= null;
						m_ent_inst_init_screen_slot_id	= -1;
						m_ent_inst_screen_slot_id		= -1;
						
						m_ent_inst_captured 		= false;
						
						m_ent_inst_capture_offs_x	= -1;
						m_ent_inst_capture_offs_y	= -1;
					}
					break;
				
				case layout_editor_param.CONST_SET_ENT_ACTIVE:
					{
						m_ent_data = ( entity_data )_val;
					}
					break;
				
				case layout_editor_param.CONST_SET_ENT_SEL_BRING_FRONT:
					{
						layout_screen_data scr_data = get_ent_inst_init_screen_data();
						
						scr_data.remove_entity( scr_data.entities_find_index( delegate( entity_instance _ent ) { return m_ent_inst == _ent; } ) );
						scr_data.add_entity( m_ent_inst );
						
						m_owner.update();
					}
					break;

				case layout_editor_param.CONST_SET_ENT_SEL_SEND_BACK:
					{
						layout_screen_data scr_data = get_ent_inst_init_screen_data();
						
						scr_data.remove_entity( scr_data.entities_find_index( delegate( entity_instance _ent ) { return m_ent_inst == _ent; } ) );
						scr_data.insert_entity( 0, m_ent_inst );
						
						m_owner.update();
					}
					break;
				
				case layout_editor_param.CONST_SET_ENT_EDIT:
				case layout_editor_param.CONST_SET_ENT_INST_EDIT:
				case layout_editor_param.CONST_SET_ENT_SELECT_TARGET:
					{
						m_ent_mode = _param;
						
						m_owner.update();
					}
					break;
				
				case layout_editor_param.CONST_SET_ENT_INST_DELETE:
					{
						return delete_entity_instance_from_layout();
					}
				
				case layout_editor_param.CONST_SET_ENT_INST_RESET_IF_EQUAL:
					{
						if( m_ent_inst != null && m_ent_inst == ( entity_instance )_val )
						{
							set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
						}
					}
					break;
				
				case layout_editor_param.CONST_SET_ENT_SNAPPING:
					{
						entity_snapping = ( bool )_val;
					}
					break;
					
				case layout_editor_param.CONST_SET_ENT_UPDATE_ENTS_CNT:
					{
						m_need_upd_ents_cnt = true;
					}
					break;
				
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_entities.set_param]" );
				}
			}
			
			return true;
		}
		
		public override void subscribe( uint _param, Action< object, EventArgs > _method )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SUBSCR_ENT_INST_SELECT:
					{
						this.EntityInstanceSelected += new EventHandler( _method );
					}
					break;
					
				case layout_editor_param.CONST_SUBSCR_CANCEL_OPERATION:
					{
						this.EditEntityCancel += new EventHandler( _method );
					}
					break;
					
				default:
				{
					throw new Exception( "Unknown parameter detected!\n\n[layout_editor_entities.subscribe]" );
				}
			}
		}
		
		public override void key_down_event( object sender, KeyEventArgs e )
		{
			//...
		}
		
		public override void key_up_event( object sender, KeyEventArgs e )
		{
			base.key_up_event( sender, e );
		}
		
		protected override void cancel_operation()
		{
			if( m_ent_mode == layout_editor_param.CONST_SET_ENT_EDIT || m_ent_mode == layout_editor_param.CONST_SET_ENT_SELECT_TARGET )
			{
				if( EditEntityCancel != null )
				{
					EditEntityCancel( this, null );
				}
			}
		}
	}
}
