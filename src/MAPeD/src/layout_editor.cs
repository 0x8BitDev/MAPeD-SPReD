/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 24.05.2017
 * Time: 16:40
 */
#define DEF_SNAPPING_BY_PIVOT	// otherwise snapping by origin

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor.
	/// </summary>
	/// 
	
	public class layout_editor : drawable_base
	{
		public event EventHandler ScreenSelected;
		public event EventHandler EntityInstanceSelected;
		public event EventHandler ResetSelectedScreen;

		private bool 	m_dispatch_selection_mode			= false;
		private int		m_dispatch_mode_sel_screen_slot_id	= -1;

		private readonly short[]	m_adj_scr_data_arr	= null;
		private readonly int[] 		m_adj_scr_ind_arr	= null;

		private int 	m_offset_x 		= 0;
		private int 	m_offset_y 		= 0;
		private float 	m_scale 		= 1;
		private float 	m_tmp_scale		= 1;

		private int		m_mouse_x	 	= 0;
		private int		m_mouse_y	 	= 0;
		
		private int		m_last_mouse_x	 = 0;
		private int		m_last_mouse_y	 = 0;

		private int		m_scr_half_width  = 0;
		private int		m_scr_half_height = 0;
		
		private int		m_sel_screen_slot_id	= -1;
		private int		m_active_screen_index	= layout_data.CONST_EMPTY_CELL_ID;
		
		private readonly ImageAttributes m_scr_img_attr	= null;
		private Rectangle 			m_scr_img_rect;
		
		private layout_data			m_layout = null;
		
		private readonly List< tiles_data > 	m_tiles_data	= null;
		
		private readonly i_screen_list m_scr_list	= null;
		
		private readonly Label m_label				= null;
		
		private entity_data		m_ent_data			= null;
		private entity_instance	m_ent_inst			= null;

		private int 	m_ent_inst_screen_slot_id	= -1;
		
		private bool 	m_ent_inst_captured			= false;
		
		private bool	m_high_quality_render		= true;
		
		private int		m_ent_inst_capture_offs_x	= -1;
		private int		m_ent_inst_capture_offs_y	= -1;
		
		private bool 	m_show_marks				= true;
		private bool 	m_show_entities				= true;
		private bool 	m_show_targets				= true;
		private bool 	m_show_coords				= true;
		
		private Bitmap		m_scr_mark_img			= null;
		private Graphics	m_scr_mark_gfx			= null;
		
		private const float CONST_MIN_SCALE			= 0.1f;
		
		private bool	m_ent_snapping				= true;
		
		public bool entity_snapping
		{
			get { return m_ent_snapping; }
			set { m_ent_snapping = value; }
		}

		public bool show_marks
		{
			get { return m_show_marks; }
			set { m_show_marks = value; update(); }
		}
		
		public bool show_entities
		{
			get { return m_show_entities; }
			set { m_show_entities = value; update(); }
		}

		public bool show_targets
		{
			get { return m_show_targets; }
			set { m_show_targets = value; update(); }
		}
		
		public bool show_coords
		{
			get { return m_show_coords; }
			set { m_show_coords = value; update(); }
		}
		
		public enum EMode
		{
			em_Screens,
			em_EditInstances,
			em_EditEntities,
			em_PickupTargetEntity,
		}
		
		private EMode	m_mode	= EMode.em_Screens;
		
		public EMode mode
		{
			get { return m_mode; }
			set 
			{ 
				m_mode = value;
				
				update();
			}
		}

		private data_sets_manager.EScreenDataType	m_screen_data_type = data_sets_manager.EScreenDataType.sdt_Tiles4x4;

		public layout_editor( PictureBox _pbox, Label _label, List< tiles_data > _tiles_data, i_screen_list _scr_list ) : base( _pbox )
		{
			m_label			= _label;
			m_tiles_data 	= _tiles_data;
			m_scr_list		= _scr_list;
			
			m_pix_box.MouseDown 	+= new MouseEventHandler( Layout_MouseDown );
			m_pix_box.MouseUp		+= new MouseEventHandler( Layout_MouseUp );
			m_pix_box.MouseMove		+= new MouseEventHandler( Layout_MouseMove );
			m_pix_box.MouseWheel	+= new MouseEventHandler( Layout_MouseWheel );
			
			m_pix_box.MouseEnter 	+= new EventHandler( Layout_MouseEnter );			
			m_pix_box.MouseLeave	+= new EventHandler( Layout_MouseLeave );
			
			m_scr_half_width  = m_pix_box.Width >> 1;
			m_scr_half_height = m_pix_box.Height >> 1;
			
			float[][] pts_arr = {	new float[] {1, 0, 0, 0, 0},
									new float[] {0, 1, 0, 0, 0},
									new float[] {0, 0, 1, 0, 0},
									new float[] {0, 0, 0, 0.5f, 0},
									new float[] {0, 0, 0, 0, 1} };
			
			ColorMatrix clr_mtx = new ColorMatrix( pts_arr );
			m_scr_img_attr		= new ImageAttributes();
			m_scr_img_attr.SetColorMatrix( clr_mtx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );
			
			m_scr_img_rect = new Rectangle();

			reset( true );
			
			m_adj_scr_data_arr = new short[ layout_data.adj_scr_slots.Length ];
			
			m_adj_scr_ind_arr = new int[]{ -1, /*( -width - 1 )*/0, /*-width*/0, /*( -width + 1 )*/0, +1, /*( width + 1 )*/0, /*width*/0, /*( width - 1 )*/0, -1 };
		}
		
		protected override void Resize_Event(object sender, EventArgs e)
		{
			base.Resize_Event(sender, e);
			
			m_scr_half_width  = m_pix_box.Width >> 1;
			m_scr_half_height = m_pix_box.Height >> 1;
			
			m_pbox_rect.Width	= m_pix_box.Width;
			m_pbox_rect.Height	= m_pix_box.Height;
		}
		
		public void reset( bool _init )
		{
			if( m_layout != null )
			{
				m_layout.reset( _init );
				
				clamp_offsets();
				update_label();
			}
			
			m_last_mouse_x	 = 0;
			m_last_mouse_y	 = 0;
		
			m_tmp_scale = 1;
			m_scale 	= 1;
			m_offset_x	= 0;
			m_offset_y	= 0;

			set_high_quality_render_mode( true );
			
			m_dispatch_mode_sel_screen_slot_id = -1;
			
			set_active_screen( -1 );
			
			reset_entity_instance();
			
			update();
		}
		
		public void subscribe_event( screen_editor _scr_editor )
		{
			_scr_editor.ModeChanged += new EventHandler( screen_editor_mode_changed );
			
			_scr_editor.RequestUpScreen		+= new EventHandler( request_up_screen );
			_scr_editor.RequestDownScreen	+= new EventHandler( request_down_screen );
			_scr_editor.RequestLeftScreen	+= new EventHandler( request_left_screen );
			_scr_editor.RequestRightScreen	+= new EventHandler( request_right_screen );
			
			_scr_editor.PutTilesPattern		+= new EventHandler( put_tiles_pattern );
		}
		
		private void put_tiles_pattern( object sender, EventArgs e )
		{
			TilesPatternEventArg evn = e as TilesPatternEventArg;
			
			int pos_x = evn.pos_x;
			int pos_y = evn.pos_y;
			
			int CHR_bank_id = evn.CHR_bank_id;
			
			pattern_data data = evn.data;
			
			int scr_ind = m_dispatch_mode_sel_screen_slot_id;
			
			// current selected screen
			put_tiles_pattern_on_screen( scr_ind, CHR_bank_id, data, pos_x, pos_y );
			
			// run through adjacent screens
			for( int n = 0; n < 8; n++ )
			{
				put_tiles_pattern_on_screen( m_layout.get_adjacent_screen_index( scr_ind, layout_data.adj_scr_slots[ n << 1 ], layout_data.adj_scr_slots[ ( n << 1 ) + 1 ] ), CHR_bank_id, data, pos_x, pos_y );
			}
			
			// update border tiles
			dispatch_event_screen_selected();
		}
		
		void put_tiles_pattern_on_screen( int _scr_ind, int _CHR_bank_id, pattern_data _data, int _pos_x, int _pos_y )
		{
			int CHR_bank_id 		= -1;
			int bank_scr_ind 		= -1;
			tiles_data bank_data 	= null;
			
			if( _scr_ind >= 0 && get_screen_data( _scr_ind, ref CHR_bank_id, ref bank_scr_ind, ref bank_data ) )
			{
				if( CHR_bank_id == _CHR_bank_id )
				{
					int num_width_tiles		= platform_data.get_screen_tiles_width_uni( m_screen_data_type );
					int num_height_tiles	= platform_data.get_screen_tiles_height_uni( m_screen_data_type );
					
					int scr_x = ( _scr_ind % get_width() );
					int scr_y = ( _scr_ind / get_width() );
					
					int scr_pttrn_x = ( m_dispatch_mode_sel_screen_slot_id % get_width() ) * num_width_tiles + _pos_x;
					int scr_pttrn_y = ( m_dispatch_mode_sel_screen_slot_id / get_width() ) * num_height_tiles + _pos_y;
					
					int pttrn_offs_x = scr_pttrn_x - scr_x * num_width_tiles;
					int pttrn_offs_y = scr_pttrn_y - scr_y * num_height_tiles; 
		
					if( Math.Max( 0, pttrn_offs_x ) < Math.Min( num_width_tiles, pttrn_offs_x + _data.width ) &&
					  ( Math.Max( 0, pttrn_offs_y ) < Math.Min( num_height_tiles, pttrn_offs_y + _data.height ) ) )
					{
						for( int tile_y = 0; tile_y < _data.height; tile_y++ )
						{
							for( int tile_x = 0; tile_x < _data.width; tile_x++ )
							{
								if( ( pttrn_offs_x + tile_x >=0 && pttrn_offs_x + tile_x < num_width_tiles ) && ( pttrn_offs_y + tile_y >=0 && pttrn_offs_y + tile_y < num_height_tiles ) )
								{
									bank_data.set_screen_tile( bank_scr_ind, ( ( pttrn_offs_y + tile_y ) * num_width_tiles ) + pttrn_offs_x + tile_x, _data.data.get_tile( tile_y * _data.width + tile_x ) );
								}
							}
						}
					}
				}
			}
		}

		private void request_up_screen( object sender, EventArgs e )
		{
			if( m_dispatch_selection_mode && m_dispatch_mode_sel_screen_slot_id >= 0 )
			{
				select_screen( m_dispatch_mode_sel_screen_slot_id - get_width() );				
			}
		}
		
		private void request_down_screen( object sender, EventArgs e )
		{
			if( m_dispatch_selection_mode && m_dispatch_mode_sel_screen_slot_id >= 0 )
			{
				select_screen( m_dispatch_mode_sel_screen_slot_id + get_width() );				
			}
		}
		
		private void request_left_screen( object sender, EventArgs e )
		{
			if( m_dispatch_selection_mode && m_dispatch_mode_sel_screen_slot_id >= 0 )
			{
				select_screen( m_dispatch_mode_sel_screen_slot_id - 1 );
			}
		}
		
		private void request_right_screen( object sender, EventArgs e )
		{
			if( m_dispatch_selection_mode && m_dispatch_mode_sel_screen_slot_id >= 0 )
			{
				select_screen( m_dispatch_mode_sel_screen_slot_id + 1 );
			}
		}

		private void select_screen( int _scr_ind )
		{
			if( has_screen( _scr_ind ) )
			{
				m_dispatch_mode_sel_screen_slot_id = _scr_ind;
				
				dispatch_event_screen_selected();

				centering_sel_screen();
				
				update();
			}
		}
		
		private void centering_sel_screen()
		{
			int scr_x = platform_data.get_screen_width_pixels() * ( m_dispatch_mode_sel_screen_slot_id % get_width() );
			int scr_y = platform_data.get_screen_height_pixels() * ( m_dispatch_mode_sel_screen_slot_id / get_width() );

			m_offset_x	= m_scr_half_width - ( scr_x + ( platform_data.get_screen_width_pixels() >> 1 ) );
			m_offset_y	= m_scr_half_height - ( scr_y + ( platform_data.get_screen_height_pixels() >> 1 ) );
			
			clamp_offsets();
		}
		
		private void screen_editor_mode_changed( object sender, EventArgs e )
		{
			m_dispatch_selection_mode = ( ( sender as screen_editor ).mode == screen_editor.EMode.em_Layout );
			m_dispatch_mode_sel_screen_slot_id 	= -1;
			
			update();
		}
		
		public void subscribe_event( data_sets_manager _data_mngr )
		{
			_data_mngr.SetLayoutData 	+= new EventHandler( update_layout_data );
			_data_mngr.SetTilesData		+= new EventHandler( update_tiles_data );
		}
		
		private void Layout_MouseDown(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				m_ent_inst_captured = false;
				
				bool entity_picked = false;
				
				if( show_entities && mode == EMode.em_EditInstances || mode == EMode.em_PickupTargetEntity )
				{
					// select entity
					if( m_sel_screen_slot_id >= 0 )
					{
						int scr_pos_x = get_sel_scr_pos_x();
						int scr_pos_y = get_sel_scr_pos_y();
						
						int scr_pos_x_pix = screen_pos_x_by_slot_id( scr_pos_x );
						int scr_pos_y_pix = screen_pos_y_by_slot_id( scr_pos_y );
						
						int mouse_scr_pos_x = ( int )( ( e.X - scr_pos_x_pix ) / m_scale );
						int mouse_scr_pos_y = ( int )( ( e.Y - scr_pos_y_pix ) / m_scale );

						if( !( entity_picked = pickup_entity( mouse_scr_pos_x, mouse_scr_pos_y, scr_pos_x, scr_pos_y ) ) )
						{
							// try to pick at adjacent screens
							int scr_cnt_x = get_width();
							int scr_cnt_y = get_height();
							
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
							int sel_scr_mod_x_ind = m_sel_screen_slot_id % scr_cnt_x;
							int sel_scr_mod_y_ind = m_sel_screen_slot_id / scr_cnt_x;
							
							for( int i = arr_ind; i < last_arr_ind; i++ )
							{
								scr_ind = m_sel_screen_slot_id + m_adj_scr_ind_arr[ i ];
								
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
										scr_pos_x_pix = screen_pos_x_by_slot_id( scr_mod_x_ind );
										scr_pos_y_pix = screen_pos_y_by_slot_id( scr_mod_y_ind );
										
										mouse_scr_pos_x = ( int )( ( e.X - scr_pos_x_pix ) / m_scale );
										mouse_scr_pos_y = ( int )( ( e.Y - scr_pos_y_pix ) / m_scale );
										
										if( ( entity_picked = pickup_entity( mouse_scr_pos_x, mouse_scr_pos_y, scr_mod_x_ind, scr_mod_y_ind ) ) == true )
										{
											break;
										}
									}
								}
							}
						}
					}
					
					if( mode == EMode.em_PickupTargetEntity )
					{
						if( !entity_picked )
						{
	                		if( EntityInstanceSelected != null )
	                		{
	                			EntityInstanceSelected( this, new EventArg2Params( null, null ) );
	                		}
							
							update();
						}
					}
					else
					if( !m_ent_inst_captured && m_ent_inst != null )
					{
						reset_entity_instance();
						
                		if( EntityInstanceSelected != null )
                		{
                			EntityInstanceSelected( this, new EventArg2Params( m_ent_inst, null ) );
                		}
						
						update();
					}
				}				
				
				if( !m_ent_inst_captured )
				{
					m_last_mouse_x	 = e.X;
					m_last_mouse_y	 = e.Y;
					
					int width_scaled	= ( int )( m_scale * get_width() * platform_data.get_screen_width_pixels() );
					int height_scaled	= ( int )( m_scale * get_height() * platform_data.get_screen_height_pixels() );
					
					if( width_scaled > m_pix_box.Width || height_scaled > m_pix_box.Height )
					{
						set_high_quality_render_mode( false );
					}
				}
			}
		}

		private bool pickup_entity( int _cursor_pos_x, int _cursor_pos_y, int _scr_pos_x, int _scr_pos_y )
		{
			entity_instance ent_inst;
			
			bool res = false;  
			
			layout_screen_data scr_data = m_layout.get_data( _scr_pos_x, _scr_pos_y );

			bool valid_pos = ( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
			
			if( valid_pos == true )
			{
				for( int ent_n = 0; ent_n < scr_data.m_ents.Count; ent_n++ )				
                {
					ent_inst = scr_data.m_ents[ ent_n ];
					
                	if( _cursor_pos_x >= ent_inst.x && _cursor_pos_x <= ent_inst.x + ent_inst.base_entity.width )
                	{
                    	if( _cursor_pos_y >= ent_inst.y && _cursor_pos_y <= ent_inst.y + ent_inst.base_entity.height )
                    	{
                    		res = true;
                    		
                    		if( mode == EMode.em_EditInstances )
                    		{
                        		scr_data.m_ents.Remove( ent_inst );
                        		
                        		m_ent_inst = ent_inst;                        		
#if DEF_SNAPPING_BY_PIVOT
                        		m_ent_inst_capture_offs_x	= _cursor_pos_x - ( ent_inst.x + ent_inst.base_entity.pivot_x );
                                m_ent_inst_capture_offs_y	= _cursor_pos_y - ( ent_inst.y + ent_inst.base_entity.pivot_y );
#else
                        		m_ent_inst_capture_offs_x	= _cursor_pos_x - ent_inst.x;
                        		m_ent_inst_capture_offs_y	= _cursor_pos_y - ent_inst.y;
#endif                  		
                        		m_ent_inst_screen_slot_id = m_sel_screen_slot_id;
                        		
                        		m_ent_inst_captured = true;
                        		
                        		m_pix_box.Capture 	= false;
                    		}

                    		if( EntityInstanceSelected != null )
                    		{
                    			EntityInstanceSelected( this, new EventArg2Params( ent_inst, null ) );
                    		}
                    		
                    		update();
                    		
                    		break;
                    	}
                	}
				}
			}
			
			return res;
		}
		
		private void Layout_MouseUp(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				if( m_sel_screen_slot_id >= 0 )
				{
					if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID )
					{
						layout_screen_data scr_data = m_layout.get_data( get_sel_scr_pos_x(), get_sel_scr_pos_y() );
						
						scr_data.m_scr_ind = m_active_screen_index;
					}
				}
				else
				{
					// reset selection
					if( ResetSelectedScreen != null )
					{
						ResetSelectedScreen( this, null );
						
						set_high_quality_render_mode( true );
						return;
					}
				}
					
				if( can_dispatch_selection() )
				{
					m_dispatch_mode_sel_screen_slot_id = m_sel_screen_slot_id;

					dispatch_event_screen_selected();
				}
				
				set_high_quality_render_mode( true );
				
				if( show_entities )
				{
					if( mode == EMode.em_EditEntities )
					{
						place_new_entity_instance( m_ent_data );
					}
					else
					if( mode == EMode.em_EditInstances )
					{
						if( m_ent_inst != null )
						{
							m_ent_inst_captured = false;
							
							bool ent_placed = false;
							
							if( m_sel_screen_slot_id >= 0 )
							{
								if( place_old_entity_instance( m_ent_inst ) == true )
								{
									m_ent_inst_screen_slot_id = m_sel_screen_slot_id;
									
									ent_placed = true;
								}
							}
							
							if( !ent_placed )
							{
								reset_entity_instance();
								
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
					update();
				}
			}
		}

		private bool place_new_entity_instance( entity_data _ent_data )
		{
			if( _ent_data != null && m_sel_screen_slot_id >= 0 )
			{
#if DEF_SNAPPING_BY_PIVOT
				int ent_pos_x = m_mouse_x;
				int ent_pos_y = m_mouse_y;
#else
				int ent_pivot_x	= ( int )( _ent_data.pivot_x * m_scale );
				int ent_pivot_y = ( int )( _ent_data.pivot_y * m_scale );

				int ent_pos_x = m_mouse_x - ent_pivot_x;
				int ent_pos_y = m_mouse_y - ent_pivot_y;
#endif				
				int scr_pos_x = get_sel_scr_pos_x();
				int scr_pos_y = get_sel_scr_pos_y();
				
				layout_screen_data scr_data = m_layout.get_data( scr_pos_x, scr_pos_y );
				
				bool valid_pos = ( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
				
				if( valid_pos == true && check_entity_pos( ref ent_pos_x, ref ent_pos_y, _ent_data ) )
				{
					int scr_pos_x_pix = screen_pos_x_by_slot_id( scr_pos_x );
					int scr_pos_y_pix = screen_pos_y_by_slot_id( scr_pos_y );
					
					ent_pos_x = ( int )( ( ent_pos_x - scr_pos_x_pix ) / m_scale );
					ent_pos_y = ( int )( ( ent_pos_y - scr_pos_y_pix ) / m_scale );
					
					get_snapped_pos( ref ent_pos_x, ref ent_pos_y, false );

#if DEF_SNAPPING_BY_PIVOT
					ent_pos_x -= _ent_data.pivot_x;
					ent_pos_y -= _ent_data.pivot_y;
#endif					
					scr_data.m_ents.Add( new entity_instance( m_ent_data.inst_properties, ent_pos_x, ent_pos_y, m_ent_data ) );
					
					update();
					
					return true;
				}
			}
			
			return false;
		}

		private bool place_old_entity_instance( entity_instance _ent_inst )
		{
			if( _ent_inst != null && m_sel_screen_slot_id >= 0 )
			{
				int capt_pos_x = ( int )( m_ent_inst_capture_offs_x * m_scale );
				int capt_pos_y = ( int )( m_ent_inst_capture_offs_y * m_scale );
				
				int ent_pos_x = m_mouse_x - capt_pos_x;
				int ent_pos_y = m_mouse_y - capt_pos_y;

				get_snapped_pos( ref ent_pos_x, ref ent_pos_y );

				int scr_pos_x = get_sel_scr_pos_x();
				int scr_pos_y = get_sel_scr_pos_y();
				
				layout_screen_data scr_data = m_layout.get_data( scr_pos_x, scr_pos_y );
				
				bool valid_pos = ( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
				
				if( valid_pos == true && check_entity_pos( ref ent_pos_x, ref ent_pos_y, _ent_inst.base_entity ) )
				{
					int scr_pos_x_pix = screen_pos_x_by_slot_id( scr_pos_x );
					int scr_pos_y_pix = screen_pos_y_by_slot_id( scr_pos_y );
					
					ent_pos_x = ( int )( ( ent_pos_x - scr_pos_x_pix ) / m_scale );
					ent_pos_y = ( int )( ( ent_pos_y - scr_pos_y_pix ) / m_scale );
					
			    	get_snapped_pos( ref ent_pos_x, ref ent_pos_y, false );  

#if DEF_SNAPPING_BY_PIVOT
			    	_ent_inst.x = ent_pos_x - _ent_inst.base_entity.pivot_x;
			    	_ent_inst.y = ent_pos_y - _ent_inst.base_entity.pivot_y;
#else			    	
			    	_ent_inst.x = ent_pos_x;
			    	_ent_inst.y = ent_pos_y;
#endif					
					scr_data.m_ents.Add( _ent_inst );
					
					update();
					
					return true;
				}
			}
			
			return false;
		}
		
		private void Layout_MouseMove(object sender, MouseEventArgs e)
		{
			m_mouse_x = e.X;
			m_mouse_y = e.Y;
			
			if( m_pix_box.Capture && e.Button == MouseButtons.Left )
			{
				m_offset_x += m_mouse_x - m_last_mouse_x;
				m_offset_y += m_mouse_y - m_last_mouse_y;
				
				clamp_offsets();
				
				m_last_mouse_x	 = m_mouse_x;
				m_last_mouse_y	 = m_mouse_y;
			}
			
			{
				int x = transform_to_img_pos( m_mouse_x, m_offset_x, m_scr_half_width );
				int y = transform_to_img_pos( m_mouse_y, m_offset_y, m_scr_half_height );
				
				int layout_width	= get_width() * platform_data.get_screen_width_pixels();
				int layout_height	= get_height() * platform_data.get_screen_height_pixels();
				
				if( x > 0 && x < layout_width && y > 0 && y < layout_height )
				{
					m_sel_screen_slot_id = ( x / platform_data.get_screen_width_pixels() ) + ( y / platform_data.get_screen_height_pixels() ) * get_width();
					
					m_pix_box.Cursor = can_dispatch_selection() ? Cursors.Hand:Cursors.Arrow;
					
					MainForm.set_status_msg( String.Format( "Layout Editor: screen pos: {0};{1}", get_sel_scr_pos_x(), get_sel_scr_pos_y() ) );
				}
				else
				{
					m_pix_box.Cursor = Cursors.Arrow;
					
					m_sel_screen_slot_id = -1;
				}
			}

			if( mode == EMode.em_EditInstances && m_ent_inst_captured && m_ent_inst != null )
			{
				m_ent_inst_screen_slot_id = m_sel_screen_slot_id;
			}
			
			update();
		}
		
		private void Layout_MouseWheel(object sender, MouseEventArgs e)
		{
			m_tmp_scale += ( float )e.Delta / 2000;
			
			m_tmp_scale = m_scale = m_tmp_scale < CONST_MIN_SCALE ? CONST_MIN_SCALE:m_tmp_scale;			
			
			if( m_scale > 1.1 )
			{
				m_scale = 2;
				m_tmp_scale = 1.1f;
				
				enable_smoothing_mode( false );
			}
			else
			{
				enable_smoothing_mode( true );
			}
			
			if( e.Delta < 0 )
			{
				if( m_scale < 1.1f && m_scale > 1 )
				{
					m_scale = 1;
				}
			}
			
			if( m_scale > 1 && m_scale < 2 )
			{
				m_tmp_scale = m_scale = 1;
			}
			
			clamp_offsets();
			
			update();
		}

		private void Layout_MouseEnter(object sender, EventArgs e)
		{
			m_pix_box.Focus();
		}

		private void Layout_MouseLeave(object sender, EventArgs e)
		{
			m_label.Focus();
			
			if( show_entities && mode == EMode.em_EditInstances && m_ent_inst_captured == true )
			{
				reset_entity_instance();
				
        		if( EntityInstanceSelected != null )
        		{
        			EntityInstanceSelected( this, new EventArg2Params( m_ent_inst, null ) );
        		}
			}
			
			// hide mouse position to hide an active entity in the "edit entity" mode
			{
				m_mouse_x = 10000;
				m_mouse_y = 10000;
				
				update();
			}
		}

		private bool can_dispatch_selection()
		{
			if( m_dispatch_selection_mode && m_layout != null && m_sel_screen_slot_id >= 0 )
			{
				if( m_layout.get_data( get_sel_scr_pos_x(), get_sel_scr_pos_y() ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					return true;
				}
			}
			
			return false;
		}
		
		private void clamp_offsets()
		{
			int width	= get_width() * platform_data.get_screen_width_pixels();
			int height	= get_height() * platform_data.get_screen_height_pixels();
			
			int width_scaled	= ( int )( m_scale * width );
			int height_scaled	= ( int )( m_scale * height );
			
			int tx = m_scr_half_width - ( int )( m_scr_half_width / m_scale );
			int ty = m_scr_half_height - ( int )( m_scr_half_height / m_scale );

			if( width_scaled < m_pix_box.Width )
			{
				m_offset_x = m_scr_half_width - ( width >> 1 );
			}
			else
			{
				m_offset_x = ( ( m_offset_x + width + tx ) < m_pix_box.Width ) ? ( m_pix_box.Width - width - tx ):m_offset_x;
				m_offset_x = m_offset_x > tx ? tx:m_offset_x;
			}
			
			if( height_scaled < m_pix_box.Height )
			{
				m_offset_y = m_scr_half_height - ( height >> 1 );
			}
			else
			{
				m_offset_y = ( ( m_offset_y + height + ty ) < m_pix_box.Height ) ? ( m_pix_box.Height - height - ty ):m_offset_y;
				m_offset_y = m_offset_y > ty ? ty:m_offset_y;
			}
		}

		private int transform_to_scr_pos( int _pos, int _half_scr )
		{
			return ( int )( ( _pos - _half_scr ) * m_scale + _half_scr );
		}

		private int transform_to_img_pos( int _pos, int _offset, int _half_scr )
		{
			return ( int )( ( float )( _pos - _offset ) / m_scale - ( m_scale - 1 ) * ( ( float )( _offset - _half_scr ) / m_scale ) );
		}
		
		private void draw_screen_data( int _scr_width, int _scr_height, int _scr_size_width, int _scr_size_height, Action< int, layout_screen_data, int, int > _act )
		{
			layout_screen_data scr_data;
			
			int beg_scr_x;
			int end_scr_x;
			int beg_scr_y;
			int end_scr_y;
			int scr_x;
			int scr_y;
			int i;
			int j;
			
			// calculate visible region of screens
			{
				int offs_x		= transform_to_scr_pos( m_offset_x, m_scr_half_width );
				int offs_y		= transform_to_scr_pos( m_offset_y, m_scr_half_height );
	
				int vp_width	= m_pix_box.Width;
				int vp_height	= m_pix_box.Height;
				
				float scr_size_width_flt	= ( float )_scr_size_width;
				float scr_size_height_flt	= ( float )_scr_size_height;
				
				beg_scr_x = Math.Max( 0, -( int )Math.Ceiling( offs_x / scr_size_width_flt ) );
				end_scr_x = Math.Min( _scr_width - 1, beg_scr_x + ( int )Math.Ceiling( vp_width / scr_size_width_flt ) );
				beg_scr_y = Math.Max( 0, -( int )Math.Ceiling( offs_y / scr_size_height_flt ) );
				end_scr_y = Math.Min( _scr_height - 1, beg_scr_y + ( int )Math.Ceiling( vp_height / scr_size_height_flt ) );
			}
			
			for( i = beg_scr_y; i <= end_scr_y; i++ )
			{
				scr_y = screen_pos_y_by_slot_id( i );
				
				for( j = beg_scr_x; j <= end_scr_x; j++ )
				{
					scr_data = m_layout.get_data( j, i );
					
					if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_x = screen_pos_x_by_slot_id( j );

						m_scr_img_rect.X 		= scr_x;
						m_scr_img_rect.Y 		= scr_y;
						m_scr_img_rect.Width	= _scr_size_width;
						m_scr_img_rect.Height	= _scr_size_height;
						
						if( m_pbox_rect.IntersectsWith( m_scr_img_rect ) )
						{
							if( !m_high_quality_render )
							{
								m_pen.Color = utils.CONST_COLOR_SIMPLE_SCREEN_CROSS;
								
								m_gfx.DrawLine( m_pen, scr_x, scr_y, scr_x + _scr_size_width, scr_y + _scr_size_height );
								m_gfx.DrawLine( m_pen, scr_x + _scr_size_width, scr_y, scr_x, scr_y + _scr_size_height );			
							}
							else
							{
								_act( ( ( i * _scr_width ) + j ), scr_data, scr_x, scr_y );
							}
						}
					}
				}
			}
		}
		
		private void draw_targets( int _scr_width, int _scr_height )
		{
			layout_screen_data scr_data 	= null;
			entity_instance	ent 	= null;
			
			int targ_scr_x;
			int targ_scr_y;
			int i;
			int j;
			
			int scr_ind = 0;
			
			int scr_x;
			int scr_y;
			
			for( i = 0; i < _scr_height; i++ )
			{
				scr_y = screen_pos_y_by_slot_id( i );
				
				for( j = 0; j < _scr_width; j++ )
				{
					scr_data = m_layout.get_data( j, i );
					
					if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_x = screen_pos_x_by_slot_id( j );

						scr_data.m_ents.ForEach( delegate( entity_instance _ent_inst )
						{
							if( _ent_inst.target_uid >= 0 )
							{
								if( m_layout.get_entity_by_uid( _ent_inst.target_uid, ref scr_ind, ref ent ) )
								{
									targ_scr_x = screen_pos_x_by_slot_id( scr_ind % get_width() );
									targ_scr_y = screen_pos_y_by_slot_id( scr_ind / get_width() );
									
									m_pen.Color = utils.CONST_COLOR_TARGET_LINK;
									{
										m_gfx.DrawLine( m_pen, 
										               scr_x + ( int )( ( _ent_inst.x + ( _ent_inst.base_entity.width >> 1 ) ) * m_scale ),
										               scr_y + ( int )( ( _ent_inst.y + ( _ent_inst.base_entity.height >> 1 ) ) * m_scale ),
										               targ_scr_x + ( int )( ( ent.x + ( ent.base_entity.width >> 1 ) ) * m_scale ),
										               targ_scr_y + ( int )( ( ent.y + ( ent.base_entity.height >> 1 ) ) * m_scale ) );
									}
								}
							}
						});
					}
				}
			}
		}

		private bool check_entity_pos( ref int _ent_pos_x, ref int _ent_pos_y, entity_data _ent )
		{
			bool valid_pos = true;
			
			int layout_width 	= (int)( get_width() * platform_data.get_screen_width_pixels() );
			int layout_height 	= (int)( get_height() * platform_data.get_screen_height_pixels() );
			
			int ent_img_pos_x = transform_to_img_pos( _ent_pos_x, m_offset_x, m_scr_half_width );
			int ent_img_pos_y = transform_to_img_pos( _ent_pos_y, m_offset_y, m_scr_half_height );
			
			if( ( ent_img_pos_x - _ent.pivot_x ) < 0 )
			{
				_ent_pos_x = transform_to_scr_pos( _ent.pivot_x + m_offset_x, m_scr_half_width );
			}
			
			if( ( ent_img_pos_x - _ent.pivot_x ) + _ent.width > layout_width )
			{
				_ent_pos_x = transform_to_scr_pos( ( layout_width - _ent.width + _ent.pivot_x ) + m_offset_x, m_scr_half_width );
			}
			
			if( ( ent_img_pos_y - _ent.pivot_y ) < 0 )
			{
				_ent_pos_y = transform_to_scr_pos( _ent.pivot_y + m_offset_y, m_scr_half_height );
			}
			
			if( ( ent_img_pos_y - _ent.pivot_y ) + _ent.height > layout_height )
			{
				_ent_pos_y = transform_to_scr_pos( ( layout_height - _ent.height + _ent.pivot_y ) + m_offset_y, m_scr_half_height );
			}
			
			return valid_pos;
		}
		
		private void show_pivot_coords( int _ent_pivot_x, int _ent_pivot_y, int _ent_scr_pos_x, int _ent_scr_pos_y )
		{
			utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
			m_gfx.DrawString( "(" + _ent_pivot_x + "," + _ent_pivot_y + ")", utils.fnt8_Arial, utils.brush, _ent_scr_pos_x, _ent_scr_pos_y - 15 );
		}
		
		private void draw_pivot( int _pivot_x, int _pivot_y )
		{
			m_pen.Color = utils.CONST_COLOR_ENTITY_PIVOT;
			{
				int line_scale = ( int )( 8 * m_scale );
				
				m_gfx.DrawLine( m_pen, _pivot_x - line_scale, _pivot_y, _pivot_x + line_scale, _pivot_y );
				m_gfx.DrawLine( m_pen, _pivot_x, _pivot_y - line_scale, _pivot_x, _pivot_y + line_scale );
			}
		}
		
		public override void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			m_pen.Width = 1;

			if( m_layout != null )
			{
				int width	= get_width();
				int height	= get_height();
				
				int scr_size_width 	= (int)( platform_data.get_screen_width_pixels() * m_scale );
				int scr_size_height = (int)( platform_data.get_screen_height_pixels() * m_scale );
				
				layout_screen_data scr_data = null;
				int x;
				int y;
				int i;
				int j;
				
				if( m_scr_list.count() > 0 )
				{
					draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_ind, layout_screen_data _scr_data, int _x, int _y ) 
					{ 
						m_gfx.DrawImage( m_scr_list.get( _scr_data.m_scr_ind ), _x, _y, scr_size_width, scr_size_height );
					});

					if( show_entities )
					{
						if( show_targets && m_high_quality_render )
						{
							draw_targets( width, height );
						}
						
						draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_ind, layout_screen_data _scr_data, int _scr_x, int _scr_y ) 
						{ 
							_scr_data.m_ents.ForEach( delegate( entity_instance _ent_inst )
							{
								m_gfx.DrawImage( _ent_inst.base_entity.bitmap, _scr_x + ( int )( _ent_inst.x * m_scale ), _scr_y + ( int )( _ent_inst.y * m_scale ), ( int )_ent_inst.base_entity.width * m_scale, ( int )_ent_inst.base_entity.height * m_scale );
							});
						});
					}
					
					if( show_marks )
					{
						draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_ind, layout_screen_data _scr_data, int _x, int _y )
						{ 
		                 	if( m_layout.get_start_screen_ind() == _scr_ind )
		                 	{
		                 		update_mark( Color.FromArgb( 0x7fff0000 ), delegate() { m_scr_mark_gfx.DrawString( "S", utils.fnt64_Arial, Brushes.White, 20, 15 ); } );

		                 		draw_mark( m_scr_mark_img, _x, _y, scr_size_width >> 1, scr_size_height >> 1 );
		                 	}

	                 		int scr_half_width 	= scr_size_width >> 1;
	                 		int scr_half_height = scr_size_height >> 1;
		                 	
		                 	if( _scr_data.mark > 0 )
		                 	{
		                 		update_mark( Color.FromArgb( 0x7f0000ff ), delegate() { m_scr_mark_gfx.DrawString( _scr_data.mark.ToString( "D2" ), utils.fnt42_Arial, Brushes.White, 1, 1 ); } );
		                 		
		                 		draw_mark( m_scr_mark_img, _x + scr_half_width, _y + scr_half_height, scr_half_width, scr_half_height );
		                 	}
		                 	
		                 	if( _scr_data.adj_scr_mask > 0 )
		                 	{
		                 		update_mark( Color.FromArgb( 0x7f00ff00 ), delegate(){}, true );
         		            	
         		            	m_pen.Color = utils.CONST_COLOR_PIXBOX_DEFAULT;
         		            	{
         		            		int img_center 	= platform_data.get_screen_mark_image_size() >> 1;
         		            		int radius		= 12;
         		            		int arrow_len	= 45;
         		            		
	         		            	// o
	         		            	m_pen.Width = 4;
         		            		m_scr_mark_gfx.DrawEllipse( m_pen, img_center - radius, img_center - radius, radius << 1, radius << 1 );
         		            		
         		            		m_pen.Width = 15;
         		            		m_pen.EndCap = LineCap.ArrowAnchor;
         		            		
	         		            	if( ( _scr_data.adj_scr_mask & 0x01 ) != 0 )
	         		            	{
	         		            		// L
	         		            		m_scr_mark_gfx.DrawLine( m_pen, img_center - radius, img_center,  img_center - arrow_len - radius, img_center );
	         		            	}
	         		            	if( ( _scr_data.adj_scr_mask & 0x02 ) != 0 )
	         		            	{
	         		            		// U
	         		            		m_scr_mark_gfx.DrawLine( m_pen, img_center, img_center - radius,  img_center, img_center - arrow_len - radius );
	         		            	}
	         		            	if( ( _scr_data.adj_scr_mask & 0x04 ) != 0 )
	         		            	{
	         		            		// R
	         		            		m_scr_mark_gfx.DrawLine( m_pen, img_center + radius, img_center,  img_center + arrow_len + radius, img_center );
	         		            	}
	         		            	if( ( _scr_data.adj_scr_mask & 0x08 ) != 0 )
	         		            	{
	         		            		// D
										m_scr_mark_gfx.DrawLine( m_pen, img_center, img_center + radius,  img_center, img_center + arrow_len + radius );
	         		            	}
	         		            	
         		            		m_pen.EndCap	= LineCap.NoAnchor;
	         		            	m_pen.Width		= 1;
         		            	}
         		            	
		                 		draw_mark( m_scr_mark_img, _x + scr_half_width, _y, scr_half_width, scr_half_height );
		                 	}
						});
					}
					
					m_pen.Color = utils.CONST_COLOR_SCREEN_LIST_NOT_EMPTY;
				}
				else
				{
					m_pen.Color = utils.CONST_COLOR_SCREEN_LIST_EMPTY;
				}
				
				for( i = 0; i < height + 1; i++ )
				{
					y = screen_pos_y_by_slot_id( i );
					
					m_gfx.DrawLine( m_pen, transform_to_scr_pos( m_offset_x, m_scr_half_width ), y, screen_pos_x_by_slot_id( width ), y );
				}
				
				for( j = 0; j < width + 1; j++ )
				{
					x = screen_pos_x_by_slot_id( j );
					
					m_gfx.DrawLine( m_pen, x, transform_to_scr_pos( m_offset_y, m_scr_half_height ), x, screen_pos_y_by_slot_id( height ) );
				}
				
				if( mode == EMode.em_Screens )
				{
					if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID && m_sel_screen_slot_id >= 0 )
					{
						x = screen_pos_x_by_slot_id( get_sel_scr_pos_x() );
						y = screen_pos_y_by_slot_id( get_sel_scr_pos_y() );						
						
						m_pen.Color = utils.CONST_COLOR_SCREEN_GHOST_IMAGE_INNER_BORDER;
						m_gfx.DrawRectangle( m_pen, x+2, y+2, scr_size_width - 3, scr_size_height - 3 );
						
						m_pen.Color = utils.CONST_COLOR_SCREEN_GHOST_IMAGE_OUTER_BORDER;
						m_gfx.DrawRectangle( m_pen, x+1, y+1, scr_size_width - 1, scr_size_height - 1 );
						
						// draw ghost image
						if( m_active_screen_index != layout_data.CONST_EMPTY_CELL_ID && m_scr_list.count() > 0 )
						{
							m_scr_img_rect.X 		= x;
							m_scr_img_rect.Y 		= y;
							m_scr_img_rect.Width	= scr_size_width;
							m_scr_img_rect.Height 	= scr_size_height;
							
							m_gfx.DrawImage( m_scr_list.get( m_active_screen_index ), m_scr_img_rect, 0, 0, platform_data.get_screen_width_pixels(), platform_data.get_screen_height_pixels(), GraphicsUnit.Pixel, m_scr_img_attr );
						}
					}
					
					if( m_dispatch_selection_mode )
					{
						if( m_dispatch_mode_sel_screen_slot_id >= 0 )
						{
							x = screen_pos_x_by_slot_id( m_dispatch_mode_sel_screen_slot_id % get_width() );
							y = screen_pos_y_by_slot_id( m_dispatch_mode_sel_screen_slot_id / get_width() );
							
							m_pen.Color = utils.CONST_COLOR_SCREEN_SELECTED_LAYOUT_MODE;
							m_pen.Width = 2;
							m_gfx.DrawRectangle( m_pen, x + 2, y + 2, scr_size_width - 5, scr_size_height - 5 );
						}
						
						utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
						m_gfx.DrawString( "mode: layout", utils.fnt8_Arial, utils.brush, 0, 0 );
					}
					else
					{
						utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
						m_gfx.DrawString( "mode: single", utils.fnt8_Arial, utils.brush, 0, 0 );
					}
				}
				else
				if( show_entities )
				{
					if( mode == EMode.em_EditEntities )
					{
						if( m_ent_data != null )
						{
							int ent_width	= ( int )( m_ent_data.width * m_scale );
							int ent_height 	= ( int )( m_ent_data.height * m_scale );
	
							int ent_pivot_x	= ( int )( m_ent_data.pivot_x * m_scale );
							int ent_pivot_y = ( int )( m_ent_data.pivot_y * m_scale );
						
#if DEF_SNAPPING_BY_PIVOT
							int ent_pos_x = m_mouse_x;
							int ent_pos_y = m_mouse_y;
#else
							int ent_pos_x = m_mouse_x - ent_pivot_x;
							int ent_pos_y = m_mouse_y - ent_pivot_y;
#endif
							get_snapped_pos( ref ent_pos_x, ref ent_pos_y );

#if DEF_SNAPPING_BY_PIVOT							
							ent_pos_x -= ent_pivot_x;
							ent_pos_y -= ent_pivot_y;
#endif							
							if( m_sel_screen_slot_id >= 0 && m_layout.get_data( get_sel_scr_pos_x(), get_sel_scr_pos_y() ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
							{
								m_gfx.DrawImage( m_ent_data.bitmap, ent_pos_x, ent_pos_y, ent_width, ent_height );
								
								m_pen.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_ENT_MODE;
								m_pen.Width = 2;
								{
									m_gfx.DrawRectangle( m_pen, ent_pos_x, ent_pos_y, ent_width, ent_height );
								}
								
								draw_pivot( ent_pos_x + ent_pivot_x, ent_pos_y + ent_pivot_y );
								
								if( show_coords )
								{
									int img_ent_pos_x = transform_to_img_pos( ent_pos_x, m_offset_x, m_scr_half_width );
									int img_ent_pos_y = transform_to_img_pos( ent_pos_y, m_offset_y, m_scr_half_height );
									
									show_pivot_coords( img_ent_pos_x + m_ent_data.pivot_x, img_ent_pos_y + m_ent_data.pivot_y, ent_pos_x, ent_pos_y );
								}
							}						
							else
							{
								m_pen.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_ENT_MODE;
								m_pen.Width = 2;
								{
									m_gfx.DrawRectangle( m_pen, ent_pos_x, ent_pos_y, ent_width, ent_height );
								}
								
								draw_pivot( ent_pos_x + ent_pivot_x, ent_pos_y + ent_pivot_y );
							}
						}
						
						utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
						m_gfx.DrawString( "mode: edit entities", utils.fnt8_Arial, utils.brush, 0, 0 );
					}
					else
					if( mode == EMode.em_EditInstances )
					{
						if( m_ent_inst != null )
						{
							int capt_pos_x = m_ent_inst_captured ? ( m_mouse_x - ( int )( m_ent_inst_capture_offs_x * m_scale ) ):0;
							int capt_pos_y = m_ent_inst_captured ? ( m_mouse_y - ( int )( m_ent_inst_capture_offs_y * m_scale ) ):0;
							
							int pivot_x = ( int )( m_ent_inst.base_entity.pivot_x * m_scale );
							int pivot_y = ( int )( m_ent_inst.base_entity.pivot_y * m_scale );
							
							if( m_ent_inst_captured )
							{
								get_snapped_pos( ref capt_pos_x, ref capt_pos_y );
#if DEF_SNAPPING_BY_PIVOT								
								capt_pos_x -= pivot_x;
								capt_pos_y -= pivot_y;
#endif
							}
							
							int ent_width	= ( int )( m_ent_inst.base_entity.width * m_scale );
							int ent_height 	= ( int )( m_ent_inst.base_entity.height * m_scale );							
							
							if( m_ent_inst_screen_slot_id >= 0 )
							{
								m_pen.Width = 2;
								{
									int scr_pos_x = m_ent_inst_screen_slot_id % get_width();
									int scr_pos_y = m_ent_inst_screen_slot_id / get_width();
									
									scr_data = m_layout.get_data( scr_pos_x, scr_pos_y );
									
									if( m_ent_inst_captured )
									{
										if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
										{
											m_gfx.DrawImage( m_ent_inst.base_entity.bitmap, capt_pos_x, capt_pos_y, ent_width, ent_height );
										}
										
										m_pen.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_INST_MODE;
										m_gfx.DrawRectangle( m_pen, capt_pos_x, capt_pos_y, ent_width, ent_height );
										
										draw_pivot( capt_pos_x + pivot_x, capt_pos_y + pivot_y );
										
										if( show_coords )
										{
											int img_capt_pos_x = transform_to_img_pos( capt_pos_x, m_offset_x, m_scr_half_width );
											int img_capt_pos_y = transform_to_img_pos( capt_pos_y, m_offset_y, m_scr_half_height );
											
											show_pivot_coords( img_capt_pos_x + m_ent_inst.base_entity.pivot_x, img_capt_pos_y + m_ent_inst.base_entity.pivot_y, capt_pos_x, capt_pos_y );
										}
									}
									else
									{
										m_pen.Color = utils.CONST_COLOR_SELECTED_ENTITY_BORDER;
										
										int scr_pos_x_pix = screen_pos_x_by_slot_id( scr_pos_x );
										int scr_pos_y_pix = screen_pos_y_by_slot_id( scr_pos_y );
										
										int ent_scr_pos_x = scr_pos_x_pix + ( int )( m_ent_inst.x * m_scale );
										int ent_scr_pos_y = scr_pos_y_pix + ( int )( m_ent_inst.y * m_scale );
										
										m_gfx.DrawRectangle( m_pen, ent_scr_pos_x, ent_scr_pos_y, ent_width, ent_height );
										
										draw_pivot( ent_scr_pos_x + pivot_x, ent_scr_pos_y + pivot_y );
										
										if( show_coords )
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
									m_pen.Color = utils.CONST_COLOR_ENTITY_BORDER_EDIT_INST_MODE;
									m_pen.Width = 2;
									{
										m_gfx.DrawRectangle( m_pen, capt_pos_x, capt_pos_y, ent_width, ent_height );
									}
								}
							}
						}
						
						utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
						m_gfx.DrawString( "mode: edit instances", utils.fnt8_Arial, utils.brush, 0, 0 );
					}
					else
					if( mode == EMode.em_PickupTargetEntity )
					{
						if( m_ent_inst != null && m_high_quality_render )
						{
							int ent_width	= ( int )( m_ent_inst.base_entity.width * m_scale );
							int ent_height 	= ( int )( m_ent_inst.base_entity.height * m_scale );
							
							if( m_ent_inst_screen_slot_id >= 0 )
							{
								int scr_pos_x = m_ent_inst_screen_slot_id % get_width();
								int scr_pos_y = m_ent_inst_screen_slot_id / get_width();
								
								int scr_pos_x_pix = screen_pos_x_by_slot_id( scr_pos_x );									
								int scr_pos_y_pix = screen_pos_y_by_slot_id( scr_pos_y );
								
								m_pen.Color = utils.CONST_COLOR_SELECTED_ENTITY_BORDER;
								m_pen.Width = 2;
								{
									m_gfx.DrawRectangle( m_pen, scr_pos_x_pix + ( int )( m_ent_inst.x * m_scale ), scr_pos_y_pix + ( int )( m_ent_inst.y * m_scale ), ent_width, ent_height );
								}
							}
						}
						
						utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
						m_gfx.DrawString( "mode: pickup target entity", utils.fnt8_Arial, utils.brush, 0, 0 );
					}
				}
				else
				{
					utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
					m_gfx.DrawString( "entities: hidden", utils.fnt8_Arial, utils.brush, 0, 0 );
				}
				
				disable( false );
			}
			else
			{
				disable( true );
				
				utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
				m_gfx.DrawString( "[ Pan the viewport using a LEFT mouse button and scale it using a mouse wheel ]", utils.fnt10_Arial, utils.brush, 0, 0 );
			}
			
			invalidate();
		}

		private int screen_pos_x_by_slot_id( int _slot_id )
		{
			return transform_to_scr_pos( _slot_id * platform_data.get_screen_width_pixels() + m_offset_x, m_scr_half_width );
		}
		
		private int screen_pos_y_by_slot_id( int _slot_id )
		{
			return transform_to_scr_pos( _slot_id * platform_data.get_screen_height_pixels() + m_offset_y, m_scr_half_height );
		}
		
		private int get_sel_scr_pos_x()
		{
			return ( m_sel_screen_slot_id % get_width() );
		}

		private int get_sel_scr_pos_y()
		{
			return ( m_sel_screen_slot_id / get_width() );
		}
		
		public int get_width()
		{
			return m_layout.get_width();
		}

		public int get_height()
		{
			return m_layout.get_height();
		}
		
		public bool delete_screen_from_layout()
		{
			if( m_sel_screen_slot_id >= 0 )
			{
				if( MainForm.message_box( "Are you sure?", "Delete Screen", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					int scr_pos_x = get_sel_scr_pos_x();
					int scr_pos_y = get_sel_scr_pos_y();
					
					layout_screen_data scr_data = m_layout.get_data( scr_pos_x, scr_pos_y );
					
					scr_data.m_ents.ForEach( delegate( entity_instance _ent_inst ) { if( m_ent_inst != null && _ent_inst == m_ent_inst ) { reset_entity_instance(); } } );
					
					m_layout.delete_screen_by_pos( scr_pos_x, scr_pos_y );

					if( m_dispatch_selection_mode )
					{
						if( m_dispatch_mode_sel_screen_slot_id == m_sel_screen_slot_id )
						{
							m_dispatch_mode_sel_screen_slot_id = -1;
						}
						
						dispatch_event_screen_selected();
					}
					
					update();
					
					return true;
				}
			}
			else
			{
				MainForm.message_box( "Please, select a screen!", "Delete Screen", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			return false;
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
						layout_screen_data scr_data = m_layout.get_data( m_ent_inst_screen_slot_id % get_width(), m_ent_inst_screen_slot_id / get_width() );
					
						if( scr_data.m_ents.Remove( m_ent_inst ) == false )
						{
							MainForm.message_box( "Unexpected error!\n\nCan't delete the entity!", "Delete Entity Instance", MessageBoxButtons.OK, MessageBoxIcon.Error );
						}
						else
						{
							m_layout.entity_instance_reset_target_uid( m_ent_inst.uid );
							
							reset_entity_instance();
							
							res = true;
							
							update();
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

		public void reset_entity_instance()
		{
			m_ent_inst 					= null;
			m_ent_inst_screen_slot_id 	= -1;
			
			m_ent_inst_captured 		= false;
			
			m_ent_inst_capture_offs_x	= -1;
			m_ent_inst_capture_offs_y	= -1;
		}
		
		public entity_instance get_selected_entity_instance()
		{
			return m_ent_inst;
		}
		
		public void set_active_entity( entity_data _ent )
		{
			m_ent_data = _ent;
		}
		
		public void set_active_screen( int _index )
		{
			m_active_screen_index = unchecked( (byte)_index );
		}

		private void update_layout_data( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			reset_entity_instance();
			
			m_layout = data_mngr.get_layout_data( data_mngr.layouts_data_pos );
			
			if( m_dispatch_selection_mode && m_dispatch_mode_sel_screen_slot_id >= 0 )
			{
				m_dispatch_mode_sel_screen_slot_id = -1;
				
				dispatch_event_screen_selected();
			}
			
			update_dimension_changes();
		}
		
		private void update_tiles_data( object sender, EventArgs e )
		{
			dispatch_event_screen_selected();
			
			update();
		}
		
		private void update_label()
		{
			if( m_layout != null )
			{
				m_label.Text = "Dim: " + get_width().ToString() + " x " + get_height().ToString();
			}
			else
			{
				m_label.Text = "Dim: ? x ?";
			}
		}
		
		public void update_dimension_changes()
		{
			m_sel_screen_slot_id = -1;
			
			if( m_layout != null )
			{
				clamp_offsets();
			}
			update();
			update_label();
		}
		
		public void reset_selected_screen()
		{
			if( m_dispatch_selection_mode )
			{
				m_dispatch_mode_sel_screen_slot_id = -1;
				
				dispatch_event_screen_selected();
			}
		}

		private void dispatch_event_screen_selected()
		{
			if( ScreenSelected != null )
			{
				ScreenSelected( this, null );
			}
		}
		
		public void get_sel_screen_data( out int _CHR_bank_ind, out int _bank_scr_ind, out tiles_data _data, ref short[] _border_tiles )
		{
			_CHR_bank_ind	= -1;
			_bank_scr_ind	= -1;
			_data			= null;
			
			if( m_dispatch_mode_sel_screen_slot_id >= 0 )
			{
				get_screen_data( m_dispatch_mode_sel_screen_slot_id, ref _CHR_bank_ind, ref _bank_scr_ind, ref _data );
				
				// collect border tiles
				{
					int i;
					
					for( i = 0; i < m_adj_scr_data_arr.Length; i++ )
					{
						m_adj_scr_data_arr[ i ] = unchecked( (short)0xffff );
					}
					
					_border_tiles = m_adj_scr_data_arr;
					
					tiles_data 	btiles_data 	= null;
					int 		bCHR_ind		= -1;
					int 		bank_scr_ind	= -1;
					int			bscr_ind;
					
					int sel_scr_pos_x = m_dispatch_mode_sel_screen_slot_id % get_width();
					int sel_scr_pos_y = m_dispatch_mode_sel_screen_slot_id / get_width();
					
					for( i = 0; i < ( layout_data.adj_scr_slots.Length >> 1 ); i++ )
					{
						bscr_ind = m_dispatch_mode_sel_screen_slot_id + layout_data.adj_scr_slots[ i << 1 ] + ( layout_data.adj_scr_slots[ ( i << 1 ) + 1 ] * get_width() );
						
						if( has_screen( bscr_ind ) && valid_screen_slot( sel_scr_pos_x + layout_data.adj_scr_slots[ i << 1 ], sel_scr_pos_y + layout_data.adj_scr_slots[ ( i << 1 ) + 1 ] ) )
						{
							if( get_screen_data( bscr_ind, ref bCHR_ind, ref bank_scr_ind, ref btiles_data ) )
							{
								m_adj_scr_data_arr[ i ] = (short)( ( (byte)bCHR_ind << 8 ) | bank_scr_ind );
							}
						}
					}
				}
			}
		}
		
		private bool get_screen_data( int _sel_scr_ind, ref int _CHR_bank_ind, ref int _bank_scr_ind, ref tiles_data _data )
		{
			int screen_id = 1 + m_layout.get_data( _sel_scr_ind % get_width(), _sel_scr_ind / get_width() ).m_scr_ind;
			
			int screens_cnt;
			int total_screens	= 0;
			
			int size = m_tiles_data.Count;
			
			for( int i = 0; i < size; i++ )
			{
				screens_cnt = m_tiles_data[ i ].screen_data_cnt();
				
				total_screens += screens_cnt;
				
				if( total_screens >= screen_id )
				{
					_bank_scr_ind	= ( screens_cnt - ( total_screens - screen_id ) ) - 1;
					_CHR_bank_ind 	= i;
					_data 			= m_tiles_data[ i ];
					
					return true;
				}
			}
			
			return false;
		}
		
		private bool has_screen( int _scr_ind )
		{
			int width = get_width();
			
			if( _scr_ind >= 0 && _scr_ind < width * get_height() )
			{
				return ( m_layout.get_data( _scr_ind % width, _scr_ind / width ).m_scr_ind != layout_data.CONST_EMPTY_CELL_ID );
			}
			
			return false;
		}
		
		private bool valid_screen_slot( int _pos_x, int _pos_y )
		{
			return ( _pos_x >= 0 && _pos_x < get_width() && _pos_y >= 0 && _pos_y < get_height() );
		}
		
		private void get_snapped_pos( ref int _x, ref int _y, bool _pix_space = true )
		{
			if( entity_snapping )
			{
				int img_x = _x;
				int img_y = _y;
				
				if( _pix_space == true )
				{
					img_x = transform_to_img_pos( _x, m_offset_x, m_scr_half_width );
					img_y = transform_to_img_pos( _y, m_offset_y, m_scr_half_height );
				}
				
				img_x = ( ( img_x >> 3 ) + ( ( img_x % 8 ) > 4 ? 1:0 ) ) << 3;
				img_y = ( ( img_y >> 3 ) + ( ( img_y % 8 ) > 4 ? 1:0 ) ) << 3;
				
				if( _pix_space == true )
				{
					_x = transform_to_scr_pos( img_x, m_scr_half_width );
					_y = transform_to_scr_pos( img_y, m_scr_half_height );
					
					_x += ( int )( m_offset_x * m_scale );
					_y += ( int )( m_offset_y * m_scale );
				}
				else
				{
					_x = img_x;
					_y = img_y;
				}
			}
		}
		
		private void set_high_quality_render_mode( bool _on )
		{
			m_high_quality_render = ( m_scale < 1.0 ) ? _on:true;
		
			enable_smoothing_mode( ( m_scale < 1.0 ) ? _on:false );
		}

		private void update_mark( Color _color, Action _act, bool _clear = true )
		{
			if( m_scr_mark_img == null )
			{
				m_scr_mark_img = new Bitmap( platform_data.get_screen_mark_image_size(), platform_data.get_screen_mark_image_size(), PixelFormat.Format32bppPArgb );
				m_scr_mark_gfx = Graphics.FromImage( m_scr_mark_img );
				
				m_scr_mark_gfx.SmoothingMode 		= SmoothingMode.HighSpeed;
				m_scr_mark_gfx.InterpolationMode 	= InterpolationMode.HighQualityBilinear;
				m_scr_mark_gfx.PixelOffsetMode 		= PixelOffsetMode.HighQuality;
			}
				
			if( _clear )
			{
				m_scr_mark_gfx.Clear( _color );
			}
			
			_act();
			
			m_scr_mark_gfx.Flush();
		}
		
		private void draw_mark( Bitmap _bmp, int _x, int _y, int _width, int _height )
		{
			m_gfx.DrawImage( m_scr_mark_img, _x, _y, _width, _height );
		}

		public bool set_start_screen_mark()
		{
			bool res = false;
			
			if( m_sel_screen_slot_id >= 0 )
			{
				res = m_layout.set_start_screen_ind( m_sel_screen_slot_id );
				
				update();
			}
			
			return res;
		}
		
		public bool set_screen_mark( int _mark )
		{
			bool res = false;
			
			if( m_sel_screen_slot_id >= 0 && m_layout != null )
			{
				res = m_layout.set_screen_mark( m_sel_screen_slot_id, _mark );
				
				update();
			}
			
			return res;
		}
		
		public bool set_adjacent_screen_mask( string _mask )
		{
			bool res = false;
			
			int mask = 0;
			
			if( m_sel_screen_slot_id >= 0 && m_layout != null )
			{
				if( _mask.IndexOf( 'L' ) >= 0 )
				{
					mask |= 0x01;
				}

				if( _mask.IndexOf( 'U' ) >= 0 )
				{
					mask |= 0x02;
				}

				if( _mask.IndexOf( 'R' ) >= 0 )
				{
					mask |= 0x04;
				}

				if( _mask.IndexOf( 'D' ) >= 0 )
				{
					mask |= 0x08;
				}
				
				res = m_layout.set_adjacent_screen_mask( m_sel_screen_slot_id, mask );
				
				update();
			}
			
			return res;
		}
		
		public void set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			m_screen_data_type = _type;
		}
	}
}
