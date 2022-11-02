/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 20.10.2022
 * Time: 12:48
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;


namespace MAPeD
{
	/// <summary>
	/// Description of layout_editor_param.
	/// </summary>
	/// 
	
	public static class layout_editor_param
	{
		// set
		public const uint	CONST_SET_ENT_INST_RESET			= 0x000001;
		public const uint	CONST_SET_ENT_ACTIVE				= 0x000002;
		public const uint	CONST_SET_ENT_SEL_BRING_FRONT		= 0x000004;
		public const uint	CONST_SET_ENT_SEL_SEND_BACK			= 0x000008;
		public const uint	CONST_SET_ENT_EDIT					= 0x000010;
		public const uint	CONST_SET_ENT_INST_EDIT				= 0x000020;
		public const uint	CONST_SET_ENT_INST_DELETE			= 0x000040;
		public const uint	CONST_SET_ENT_PICKUP_TARGET			= 0x000080;
		public const uint	CONST_SET_ENT_INST_RESET_IF_EQUAL	= 0x000100;
		public const uint	CONST_SET_ENT_SNAPPING				= 0x000200;

		public const uint	CONST_SET_SCR_ACTIVE				= 0x000400;
		
		public const uint	CONST_SET_PNT_CLEAR_ACTIVE_TILE		= 0x000800;
		public const uint	CONST_SET_PNT_UPD_ACTIVE_TILE		= 0x001000;
		public const uint	CONST_SET_PNT_UPD_ACTIVE_BLOCK		= 0x002000;
		public const uint	CONST_SET_PNT_SUBSCR_DATA_MNGR		= 0x004000;
		
		public const uint	CONST_SET_BASE_MAP_SCALE_X1			= 0x008000;
		public const uint	CONST_SET_BASE_MAP_SCALE_X2			= 0x010000;

		// get
		public const uint	CONST_GET_ENT_INST_SELECTED			= 0x01;
		public const uint	CONST_GET_ENT_MODE					= 0x02;
		
		// subscribe
		public const uint	CONST_SUBSCR_ENT_INST_SELECT		= 0x01;

		public const uint	CONST_SUBSCR_SCR_RESET_SELECTED		= 0x02;
		
		public const uint	CONST_SUBSCR_PNT_UPDATE_TILE_IMAGE	= 0x04;
	}
	
	/// <summary>
	/// Description of layout_editor_behaviour_base.
	/// </summary>
	/// 
	
	public abstract class layout_editor_behaviour_base
	{
		protected readonly layout_editor_shared_data	m_shared;
		protected readonly layout_editor_base 			m_owner;
		
		protected readonly string	m_name;
		
		public layout_editor_behaviour_base( string _name, layout_editor_shared_data _shared, layout_editor_base _owner )
		{
			m_name		= _name;
			m_shared	= _shared;
			m_owner		= _owner;
		}

		public string	name()	{ return m_name; }
		
		public abstract void	reset();
		
		public abstract void	mouse_down( object sender, MouseEventArgs e );
		public abstract void	mouse_up( object sender, MouseEventArgs e );
		public abstract bool	mouse_move( object sender, MouseEventArgs e );	// OUT: true - update(); false - invalidate();
		public abstract void	mouse_enter( object sender, EventArgs e );
		public abstract void	mouse_leave( object sender, EventArgs e );
		public abstract void	mouse_wheel( object sender, EventArgs e );
		
		public abstract bool	block_free_map_panning();
		
		public abstract void	draw( Graphics _gfx, Pen _pen, int _scr_size_width, int _scr_size_height );

		public abstract Object	get_param( uint _param );
		public abstract bool	set_param( uint _param, Object _val );
		
		public abstract void	subscribe( uint _param, Action< object, EventArgs > _method );
	}

	public class layout_editor_shared_data
	{
		public int				m_sel_screen_slot_id		= -1;
		
		public layout_data		m_layout				= null;
		
		public i_screen_list	m_scr_list				= null;
		public ImageList		m_tiles_imagelist		= null;
		public ImageList		m_blocks_imagelist		= null;

		public ImageAttributes	m_scr_img_attr			= null;
		public Rectangle 		m_scr_img_rect;
		
		public float 	m_scale 			= 1;
		public int 		m_offset_x			= 0;
		public int 		m_offset_y			= 0;
 
		public int		m_mouse_x			= 10000;
		public int		m_mouse_y			= 10000;
 
		public int		m_last_mouse_x		= 0;
		public int		m_last_mouse_y		= 0;
 
		public int		m_scr_half_width	= 0;
		public int		m_scr_half_height	= 0;
		
		public bool		m_high_quality_render	= true;

		public string	m_sys_msg = "";
		
		public data_sets_manager.EScreenDataType	m_screen_data_type = data_sets_manager.EScreenDataType.sdt_Tiles4x4;
		
		// methods
		public Action< bool >				set_high_quality_render_mode;
		public Action< int, int >			draw_pivot;
		public Action< int, int, int, int >	show_pivot_coords;
		public Action< string, int, int >	print;
		public Action						pix_box_reset_capture;
		
		public Action< int, int, tiles_data, data_sets_manager.EScreenDataType >	update_active_bank_screen;
		
		public Func< bool >					pix_box_captured;
		public Func< int >					pix_box_width;
		public Func< int >					pix_box_height;
		public Func< int >					get_sel_scr_pos_x;
		public Func< int >					get_sel_scr_pos_y;
		public Func< int, int >				screen_pos_x_by_slot_id;
		public Func< int, int >				screen_pos_y_by_slot_id;
		
		public Func< int, int, int >		transform_to_scr_pos;
		public Func< int, int, int, int >	transform_to_img_pos;
		
		public Func< int, int, int >		get_local_screen_ind;
		public Func< int, int >				get_bank_ind_by_global_screen_ind;
		
		public Func< Graphics >				gfx_context;
	}
	
	/// <summary>
	/// Description of layout_editor_base.
	/// </summary>
	
	public class layout_editor_base : drawable_base
	{
		public event EventHandler MapScaleX1;
		public event EventHandler MapScaleX2;
		
		private float 	m_tmp_scale		= 1;
		
		private bool	m_enable_map_panning	= false;
		
		private layout_editor_shared_data	m_shared = null;
		
		private Label	m_label				= null;
		
		private bool 	m_show_marks		= true;
		private bool 	m_show_entities		= true;
		private bool 	m_show_targets		= true;
		private bool 	m_show_coords		= true;
		private bool 	m_show_grid			= true;

		private Bitmap		m_scr_mark_img	= null;
		private Graphics	m_scr_mark_gfx	= null;
		
		private const float CONST_MIN_SCALE	= 0.1f;
		
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

		public bool show_grid
		{
			get { return m_show_grid; }
			set { m_show_grid = value; update(); }
		}
	
		public enum EMode
		{
			em_Builder = 0,
			em_Painter,
			em_Screens,
			em_Entities,
			em_MAX,
			em_Unknown,
		}
		
		private EMode	m_mode	= EMode.em_Unknown;
		
		public EMode mode
		{
			get { return m_mode; }
			set 
			{
				if( m_behaviour != null )
				{
					m_behaviour.reset();
				}
				
				m_mode = value;
				
				m_behaviour = m_behaviour_arr[ ( int )value ];
				
				m_shared.m_sys_msg = "";
				
				update();
			}
		}
		
		private layout_editor_behaviour_base	m_behaviour		= null;
		private layout_editor_behaviour_base[]	m_behaviour_arr	= null;
		
		public layout_editor_base( data_sets_manager _data_mngr, PictureBox _pbox, Label _label, imagelist_manager _img_list_mngr ) : base( _pbox )
		{
			m_label		= _label;
			
			// init shared data
			{
				m_shared = new layout_editor_shared_data();
				
				// data
				m_shared.m_scr_list			= _img_list_mngr.get_screen_list();
				m_shared.m_tiles_imagelist	= _img_list_mngr.get_tiles_image_list();
				m_shared.m_blocks_imagelist	= _img_list_mngr.get_blocks_image_list();
	
				// method
				m_shared.set_high_quality_render_mode	= set_high_quality_render_mode;
				m_shared.pix_box_reset_capture			= pix_box_reset_capture;
				m_shared.pix_box_captured				= pix_box_captured;
				m_shared.pix_box_width					= pix_box_width;
				m_shared.pix_box_height					= pix_box_height;
				m_shared.draw_pivot						= draw_pivot;
				m_shared.show_pivot_coords				= show_pivot_coords;
				m_shared.print							= print;
				m_shared.update_active_bank_screen		= _img_list_mngr.update_active_bank_screen;
				
				m_shared.get_sel_scr_pos_x				= get_sel_scr_pos_x; 
				m_shared.get_sel_scr_pos_y 				= get_sel_scr_pos_y;
				m_shared.screen_pos_x_by_slot_id		= screen_pos_x_by_slot_id;
				m_shared.screen_pos_y_by_slot_id		= screen_pos_y_by_slot_id;
				m_shared.transform_to_scr_pos			= transform_to_scr_pos;
				m_shared.transform_to_img_pos			= transform_to_img_pos;
				
				m_shared.get_local_screen_ind				= _data_mngr.get_local_screen_ind;
				m_shared.get_bank_ind_by_global_screen_ind	= _data_mngr.get_bank_ind_by_global_screen_ind;
				
				m_shared.gfx_context					= gfx_context;
			}
			
			m_pix_box.MouseDown 	+= new MouseEventHandler( Layout_MouseDown );
			m_pix_box.MouseUp		+= new MouseEventHandler( Layout_MouseUp );

			m_pix_box.MouseMove		+= new MouseEventHandler( Layout_MouseMove );
			m_pix_box.MouseWheel	+= new MouseEventHandler( Layout_MouseWheel );
			
			m_pix_box.MouseEnter 	+= new EventHandler( Layout_MouseEnter );			
			m_pix_box.MouseLeave	+= new EventHandler( Layout_MouseLeave );
			
			m_shared.m_scr_half_width  = m_pix_box.Width >> 1;
			m_shared.m_scr_half_height = m_pix_box.Height >> 1;
			
			float[][] pts_arr = {	new float[] {1, 0, 0, 0, 0},
									new float[] {0, 1, 0, 0, 0},
									new float[] {0, 0, 1, 0, 0},
									new float[] {0, 0, 0, 0.5f, 0},
									new float[] {0, 0, 0, 0, 1} };
			
			ColorMatrix clr_mtx = new ColorMatrix( pts_arr );
			m_shared.m_scr_img_attr		= new ImageAttributes();
			m_shared.m_scr_img_attr.SetColorMatrix( clr_mtx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );
			
			m_shared.m_scr_img_rect = new Rectangle();
			
			// init available behaviours
			{
				m_behaviour_arr = new layout_editor_behaviour_base[ ( int )EMode.em_MAX ]
				{ 
					new layout_editor_builder( "builder", m_shared, this ),
					new layout_editor_painter( "painter", m_shared, this ),
					new layout_editor_screen_list( "screen list", m_shared, this ), 
					new layout_editor_entities( "entities", m_shared, this )
				};
			}
			
			reset( true );
		}
		
		protected override void Resize_Event(object sender, EventArgs e)
		{
			m_shared.m_scr_half_width  = m_pix_box.Width >> 1;
			m_shared.m_scr_half_height = m_pix_box.Height >> 1;
			
			m_pbox_rect.Width	= m_pix_box.Width;
			m_pbox_rect.Height	= m_pix_box.Height;
			
			if( m_shared.m_layout != null )
			{
				clamp_offsets();
			}

			base.Resize_Event(sender, e);
		}

		public void reset( bool _init )
		{
			if( m_shared.m_layout != null )
			{
				m_shared.m_layout.reset( _init );
				
				clamp_offsets();
				update_label();
			}
			
			m_shared.m_last_mouse_x	 = 0;
			m_shared.m_last_mouse_y	 = 0;
		
			m_tmp_scale = 1;
			m_shared.m_scale 	= 1;
			m_shared.m_offset_x	= 0;
			m_shared.m_offset_y	= 0;

			set_high_quality_render_mode( true );

			// reset all behaviours
			foreach( var bhv in m_behaviour_arr )
			{
				bhv.reset();
			}
			
			update();
		}
		
		public void subscribe_event( data_sets_manager _data_mngr )
		{
			_data_mngr.SetLayoutData 	+= new EventHandler( update_layout_data );
		}
		
		private void Layout_MouseDown(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				m_shared.m_last_mouse_x	 = e.X;
				m_shared.m_last_mouse_y	 = e.Y;
				
				if( m_shared.m_scale < 1.0 )
				{
					set_high_quality_render_mode( false );
				}
			}

			if( !map_panning_enabled() )
			{
				m_behaviour.mouse_down( sender, e );
			}
		}

		private void Layout_MouseUp(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				set_high_quality_render_mode( true );
			}

			if( !map_panning_enabled() )
			{
				m_behaviour.mouse_up( sender, e );
			}
		}
		
		private void Layout_MouseMove(object sender, MouseEventArgs e)
		{
			m_shared.m_mouse_x = e.X;
			m_shared.m_mouse_y = e.Y;
			
			bool pan_viewport_user_input	= m_pix_box.Capture && e.Button == MouseButtons.Left;
			bool need_pan_viewport 			= pan_viewport_user_input && map_panning_enabled(); 
			
			if( need_pan_viewport )
			{
				m_shared.m_offset_x += m_shared.m_mouse_x - m_shared.m_last_mouse_x;
				m_shared.m_offset_y += m_shared.m_mouse_y - m_shared.m_last_mouse_y;
				
				clamp_offsets();
				
				m_shared.m_last_mouse_x	 = m_shared.m_mouse_x;
				m_shared.m_last_mouse_y	 = m_shared.m_mouse_y;
			}
			
			// calculate selected screen position
			{
				int x = transform_to_img_pos( m_shared.m_mouse_x, m_shared.m_offset_x, m_shared.m_scr_half_width );
				int y = transform_to_img_pos( m_shared.m_mouse_y, m_shared.m_offset_y, m_shared.m_scr_half_height );
				
				int layout_width	= get_width() * platform_data.get_screen_width_pixels();
				int layout_height	= get_height() * platform_data.get_screen_height_pixels();
				
				if( x > 0 && x < layout_width && y > 0 && y < layout_height )
				{
					m_shared.m_sel_screen_slot_id = ( x / platform_data.get_screen_width_pixels() ) + ( y / platform_data.get_screen_height_pixels() ) * get_width();
				
					MainForm.set_status_msg( String.Format( "Layout Editor: screen pos: {0};{1} / slot: {2}", get_sel_scr_pos_x(), get_sel_scr_pos_y(), m_shared.m_sel_screen_slot_id ) );
				}
				else
				{
					m_shared.m_sel_screen_slot_id = -1;
				}
			}
			
			m_shared.m_sys_msg = "";
			
			if( !map_panning_enabled() )
			{
				if( pan_viewport_user_input )
				{
					m_shared.m_sys_msg = "Hold down the 'Ctrl' key to pan the viewport";
				}

				if( m_behaviour.mouse_move( sender, e ) )
				{
					update();
				}
			}
			else
			{
				if( need_pan_viewport )
				{
					update();
				}
				else
				{
					invalidate();
				}
			}
		}
		
		private bool map_panning_enabled()
		{
			return ( m_enable_map_panning && m_behaviour.block_free_map_panning() ) || !m_behaviour.block_free_map_panning();
		}

		private void Layout_MouseWheel(object sender, MouseEventArgs e)
		{
			m_tmp_scale += ( float )e.Delta / 2000;
			
			m_tmp_scale = m_shared.m_scale = m_tmp_scale < CONST_MIN_SCALE ? CONST_MIN_SCALE:m_tmp_scale;
			
			if( m_shared.m_scale > 1.1 )
			{
				m_shared.m_scale = 2;
				m_tmp_scale = 1.1f;
				
				enable_smoothing_mode( false );
			}
			else
			{
				enable_smoothing_mode( true );
			}
			
			if( e.Delta < 0 )
			{
				if( m_shared.m_scale < 1.1f && m_shared.m_scale > 1 )
				{
					m_shared.m_scale = 1;
				}
			}
			
			if( m_shared.m_scale > 1 && m_shared.m_scale < 2 )
			{
				m_tmp_scale = m_shared.m_scale = 1;
			}
			
			if( m_shared.m_scale == 1 )
			{
				if( MapScaleX1 != null )
				{
					MapScaleX1( null, null );
				}
			}
			else
			if( m_shared.m_scale == 2 )
			{
				if( MapScaleX2 != null )
				{
					MapScaleX2( null, null );
				}
			}
			
			clamp_offsets();
			
			m_behaviour.mouse_wheel( sender, e );
			
			update();
		}

		private void Layout_MouseEnter(object sender, EventArgs e)
		{
			m_pix_box.Focus();
			
			m_behaviour.mouse_enter( sender, e );
		}
		
		protected virtual void Layout_MouseLeave(object sender, EventArgs e)
		{
			m_label.Focus();
			
			// hide mouse position to hide an active entity in the "edit entity" mode
			{
				m_shared.m_mouse_x = 10000;
				m_shared.m_mouse_y = 10000;
				
				m_behaviour.mouse_leave( sender, e );
				
				update();
			}
		}

		private void clamp_offsets()
		{
			int width	= get_width() * platform_data.get_screen_width_pixels();
			int height	= get_height() * platform_data.get_screen_height_pixels();
			
			int width_scaled	= ( int )( m_shared.m_scale * width );
			int height_scaled	= ( int )( m_shared.m_scale * height );
			
			int tx = m_shared.m_scr_half_width - ( int )( m_shared.m_scr_half_width / m_shared.m_scale );
			int ty = m_shared.m_scr_half_height - ( int )( m_shared.m_scr_half_height / m_shared.m_scale );

			if( width_scaled < m_pix_box.Width )
			{
				m_shared.m_offset_x = m_shared.m_scr_half_width - ( width >> 1 );
			}
			else
			{
				m_shared.m_offset_x = ( ( m_shared.m_offset_x + width + tx ) < m_pix_box.Width ) ? ( m_pix_box.Width - width - tx ):m_shared.m_offset_x;
				m_shared.m_offset_x = m_shared.m_offset_x > tx ? tx:m_shared.m_offset_x;
			}
			
			if( height_scaled < m_pix_box.Height )
			{
				m_shared.m_offset_y = m_shared.m_scr_half_height - ( height >> 1 );
			}
			else
			{
				m_shared.m_offset_y = ( ( m_shared.m_offset_y + height + ty ) < m_pix_box.Height ) ? ( m_pix_box.Height - height - ty ):m_shared.m_offset_y;
				m_shared.m_offset_y = m_shared.m_offset_y > ty ? ty:m_shared.m_offset_y;
			}
		}

		private int transform_to_scr_pos( int _pos, int _half_scr )
		{
			return ( int )( ( _pos - _half_scr ) * m_shared.m_scale + _half_scr );
		}

		private int transform_to_img_pos( int _pos, int _offset, int _half_scr )
		{
			return ( int )( ( float )( _pos - _offset ) / m_shared.m_scale - ( m_shared.m_scale - 1 ) * ( ( float )( _offset - _half_scr ) / m_shared.m_scale ) );
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
				int offs_x		= transform_to_scr_pos( m_shared.m_offset_x, m_shared.m_scr_half_width );
				int offs_y		= transform_to_scr_pos( m_shared.m_offset_y, m_shared.m_scr_half_height );
	
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
					scr_data = m_shared.m_layout.get_data( j, i );
					
					if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_x = screen_pos_x_by_slot_id( j );

						m_shared.m_scr_img_rect.X 		= scr_x;
						m_shared.m_scr_img_rect.Y 		= scr_y;
						m_shared.m_scr_img_rect.Width	= _scr_size_width;
						m_shared.m_scr_img_rect.Height	= _scr_size_height;
						
						if( m_pbox_rect.IntersectsWith( m_shared.m_scr_img_rect ) )
						{
							if( !m_shared.m_high_quality_render )
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
					scr_data = m_shared.m_layout.get_data( j, i );
					
					if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_x = screen_pos_x_by_slot_id( j );

						scr_data.m_ents.ForEach( delegate( entity_instance _ent_inst )
						{
							if( _ent_inst.target_uid >= 0 )
							{
								if( m_shared.m_layout.get_entity_by_uid( _ent_inst.target_uid, ref scr_ind, ref ent ) )
								{
									targ_scr_x = screen_pos_x_by_slot_id( scr_ind % get_width() );
									targ_scr_y = screen_pos_y_by_slot_id( scr_ind / get_width() );
									
									m_pen.Color = utils.CONST_COLOR_TARGET_LINK;
									{
										m_gfx.DrawLine( m_pen, 
										               scr_x + ( int )( ( _ent_inst.x + ( _ent_inst.base_entity.width >> 1 ) ) * m_shared.m_scale ),
										               scr_y + ( int )( ( _ent_inst.y + ( _ent_inst.base_entity.height >> 1 ) ) * m_shared.m_scale ),
										               targ_scr_x + ( int )( ( ent.x + ( ent.base_entity.width >> 1 ) ) * m_shared.m_scale ),
										               targ_scr_y + ( int )( ( ent.y + ( ent.base_entity.height >> 1 ) ) * m_shared.m_scale ) );
									}
								}
							}
						});
					}
				}
			}
		}

		private void show_pivot_coords( int _ent_pivot_x, int _ent_pivot_y, int _ent_scr_pos_x, int _ent_scr_pos_y )
		{
			print( "(" + _ent_pivot_x + "," + _ent_pivot_y + ")", _ent_scr_pos_x, _ent_scr_pos_y - 15 );
		}
		
		private void print( string _text, int _x, int _y )
		{
			utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT_SHADOW;
			m_gfx.DrawString( _text, utils.fnt8_Arial, utils.brush, _x + 1, _y + 1 );
			
			utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
			m_gfx.DrawString( _text, utils.fnt8_Arial, utils.brush, _x, _y );
		}

		private void draw_pivot( int _pivot_x, int _pivot_y )
		{
			m_pen.Color = utils.CONST_COLOR_ENTITY_PIVOT;
			{
				int line_scale = ( int )( 8 * m_shared.m_scale );
				
				m_gfx.DrawLine( m_pen, _pivot_x - line_scale, _pivot_y, _pivot_x + line_scale, _pivot_y );
				m_gfx.DrawLine( m_pen, _pivot_x, _pivot_y - line_scale, _pivot_x, _pivot_y + line_scale );
			}
		}

		public override void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			m_pen.Width = 1;

			if( m_shared.m_layout != null )
			{
				int width	= get_width();
				int height	= get_height();
				
				int scr_size_width 	= ( int )( platform_data.get_screen_width_pixels() * m_shared.m_scale );
				int scr_size_height = ( int )( platform_data.get_screen_height_pixels() * m_shared.m_scale );
				
				int x;
				int y;
				int i;
				int j;
				
				if( m_shared.m_scr_list.count() > 0 )
				{
					draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_ind, layout_screen_data _scr_data, int _x, int _y ) 
					{ 
						m_gfx.DrawImage( m_shared.m_scr_list.get( _scr_data.m_scr_ind ), _x, _y, scr_size_width, scr_size_height );
					});

					if( show_grid )
					{
						// draw tiles grid
						if( m_shared.m_scale >= 1.0 )
						{
							int x_pos;
							int y_pos;
							
							int step = utils.CONST_SCREEN_BLOCKS_SIZE >> 1;
							
							int x_max = m_pix_box.Width - m_shared.m_offset_x;
							int x_min = x_max - m_pix_box.Width;
							
							int y_max = m_pix_box.Height - m_shared.m_offset_y;
							int y_min = y_max - m_pix_box.Height;
							
							int n8_x = x_min & -( int )( step * m_shared.m_scale );
							int n8_y = y_min & -( int )( step * m_shared.m_scale );
							
							int x_line_offset = n8_x - x_min;
							int y_line_offset = n8_y - y_min;
							
							int n_lines_x = ( int )( m_pix_box.Width / step / m_shared.m_scale ) + 2;
							int n_lines_y = ( int )( m_pix_box.Height / step / m_shared.m_scale ) + 2;
							
							float offs_x = 0;
							float offs_y = 0;
	
							int x_line_beg = ( int )( ( ( m_shared.m_scale - 1.0f ) / 2.0f ) * n_lines_x );
							int y_line_beg = ( int )( ( ( m_shared.m_scale - 1.0f ) / 2.0f ) * n_lines_y );
							
							m_pen.Color = utils.CONST_COLOR_GRID_BLOCKS;
							
							bool tile4x4_grid = ( ( platform_data.get_screen_blocks_height( false ) & 0x01 ) != 0x01 ) && ( ( platform_data.get_screen_blocks_width( false ) & 0x01 ) != 0x01 ) && ( m_shared.m_screen_data_type != data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
							
							for( i = x_line_beg; i < n_lines_x + x_line_beg; i++ )
							{
								x_pos = x_line_offset + i*step;
								
								offs_x = transform_to_scr_pos( x_pos, m_shared.m_scr_half_width );
	
								if( tile4x4_grid )
								{
									if( ( ( x_pos - m_shared.m_offset_x ) % utils.CONST_SCREEN_BLOCKS_SIZE ) == 0 )
									{
										m_pen.Color = utils.CONST_COLOR_GRID_TILES_BRIGHT;
									}
									else
									{
										m_pen.Color = utils.CONST_COLOR_GRID_TILES_DARK;
									}
								}
								
								m_gfx.DrawLine( m_pen, offs_x, 0, offs_x, m_pix_box.Height );
							}
							
							for( i = y_line_beg; i < n_lines_y + y_line_beg; i++ )
							{
								y_pos = y_line_offset + i*step;
								
								offs_y = transform_to_scr_pos( y_pos, m_shared.m_scr_half_height );
	
								if( tile4x4_grid )
								{
									if( ( ( y_pos - m_shared.m_offset_y ) % utils.CONST_SCREEN_BLOCKS_SIZE ) == 0 )
									{
										m_pen.Color = utils.CONST_COLOR_GRID_TILES_BRIGHT;
									}
									else
									{
										m_pen.Color = utils.CONST_COLOR_GRID_TILES_DARK;
									}
								}
								
								m_gfx.DrawLine( m_pen, 0, offs_y, m_pix_box.Width, offs_y );
							}
						}
					}
					
					// draw screens grid
					{
						m_pen.Color = Color.White;
						
						for( i = 0; i < height + 1; i++ )
						{
							y = screen_pos_y_by_slot_id( i );
							
							if( y >= 0 && y < m_pix_box.Height )
							{
								m_gfx.DrawLine( m_pen, transform_to_scr_pos( m_shared.m_offset_x, m_shared.m_scr_half_width ), y, screen_pos_x_by_slot_id( width ), y );
							}
						}
						
						for( j = 0; j < width + 1; j++ )
						{
							x = screen_pos_x_by_slot_id( j );
							
							if( x >= 0 && x < m_pix_box.Width )
							{
								m_gfx.DrawLine( m_pen, x, transform_to_scr_pos( m_shared.m_offset_y, m_shared.m_scr_half_height ), x, screen_pos_y_by_slot_id( height ) );
							}
						}
					}

					if( show_entities )
					{
						if( show_targets && m_shared.m_high_quality_render )
						{
							draw_targets( width, height );
						}
						
						draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_ind, layout_screen_data _scr_data, int _scr_x, int _scr_y ) 
						{ 
							_scr_data.m_ents.ForEach( delegate( entity_instance _ent_inst )
							{
								m_gfx.DrawImage( _ent_inst.base_entity.bitmap, _scr_x + ( int )( _ent_inst.x * m_shared.m_scale ), _scr_y + ( int )( _ent_inst.y * m_shared.m_scale ), ( int )_ent_inst.base_entity.width * m_shared.m_scale, ( int )_ent_inst.base_entity.height * m_shared.m_scale );
							});
						});
					}
					
					if( show_marks )
					{
						draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_ind, layout_screen_data _scr_data, int _x, int _y )
						{ 
							if( m_shared.m_layout.get_start_screen_ind() == _scr_ind )
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

				// draw data specific to active behaviour
				m_behaviour.draw( m_gfx, m_pen, scr_size_width, scr_size_height );
				
				print( "mode: " + m_behaviour.name(), 0, 0 );

				// print system message
				print( m_shared.m_sys_msg, ( m_pix_box.Width >> 1 ) - ( ( int )( Graphics.FromImage( m_main_bmp ).MeasureString( m_shared.m_sys_msg, utils.fnt8_Arial ).Width ) >> 1 ), 0 );

				disable( false );
			}
			else
			{
				disable( true );
				
				print( "[ Pan the viewport using a LEFT mouse button and scale it using a mouse wheel ]", 0, 0 );
				print( "Painter/Screen List/Entities modes:", 0, 20 );
				print( "- Hold down the 'Ctrl' key to pan the viewport", 0, 30 );
			}
			
			invalidate();
		}
		
		public bool delete_screen_from_layout()
		{
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				if( MainForm.message_box( "Are you sure?", "Delete Screen", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					int scr_pos_x = get_sel_scr_pos_x();
					int scr_pos_y = get_sel_scr_pos_y();
					
					layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
					
					if( mode == EMode.em_Entities )
					{
						scr_data.m_ents.ForEach( delegate( entity_instance _ent_inst ) { set_param( layout_editor_param.CONST_SET_ENT_INST_RESET_IF_EQUAL, _ent_inst ); } );
					}
					
					m_shared.m_layout.delete_screen_by_pos( scr_pos_x, scr_pos_y );

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

		private int screen_pos_x_by_slot_id( int _slot_id )
		{
			return transform_to_scr_pos( _slot_id * platform_data.get_screen_width_pixels() + m_shared.m_offset_x, m_shared.m_scr_half_width );
		}
		
		private int screen_pos_y_by_slot_id( int _slot_id )
		{
			return transform_to_scr_pos( _slot_id * platform_data.get_screen_height_pixels() + m_shared.m_offset_y, m_shared.m_scr_half_height );
		}
		
		private int get_sel_scr_pos_x()
		{
			return ( m_shared.m_sel_screen_slot_id % get_width() );
		}

		private int get_sel_scr_pos_y()
		{
			return ( m_shared.m_sel_screen_slot_id / get_width() );
		}
		
		private int get_width()
		{
			return m_shared.m_layout.get_width();
		}

		private int get_height()
		{
			return m_shared.m_layout.get_height();
		}

		protected virtual void update_layout_data( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			m_shared.m_layout = data_mngr.get_layout_data( data_mngr.layouts_data_pos );
			
			if( mode == EMode.em_Entities )
			{
				set_param( layout_editor_param.CONST_SET_ENT_INST_RESET, null );
			}
			
			update_dimension_changes();
		}

		private void update_label()
		{
			if( m_shared.m_layout != null )
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
			m_shared.m_sel_screen_slot_id = -1;
			
			if( m_shared.m_layout != null )
			{
				clamp_offsets();
			}
			update();
			update_label();
		}

		private void set_high_quality_render_mode( bool _on )
		{
			m_shared.m_high_quality_render = ( m_shared.m_scale < 1.0 ) ? _on:true;
		
			enable_smoothing_mode( ( m_shared.m_scale < 1.0 ) ? _on:false );
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
			
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				res = m_shared.m_layout.set_start_screen_ind( m_shared.m_sel_screen_slot_id );
				
				update();
			}
			
			return res;
		}
		
		public bool set_screen_mark( int _mark )
		{
			bool res = false;
			
			if( m_shared.m_sel_screen_slot_id >= 0 && m_shared.m_layout != null )
			{
				res = m_shared.m_layout.set_screen_mark( m_shared.m_sel_screen_slot_id, _mark );
				
				update();
			}
			
			return res;
		}
		
		public bool set_adjacent_screen_mask( string _mask )
		{
			bool res = false;
			
			int mask = 0;
			
			if( m_shared.m_sel_screen_slot_id >= 0 && m_shared.m_layout != null )
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
				
				res = m_shared.m_layout.set_adjacent_screen_mask( m_shared.m_sel_screen_slot_id, mask );
				
				update();
			}
			
			return res;
		}

		private void pix_box_reset_capture()
		{
			m_pix_box.Capture = false;
		}
		
		private bool pix_box_captured()
		{
			return m_pix_box.Capture;
		}
		
		private int pix_box_width()
		{
			return m_pix_box.Width;
		}
		
		private int pix_box_height()
		{
			return m_pix_box.Height;
		}
		
		private Graphics gfx_context()
		{
			return m_gfx;
		}
		
		public void set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			m_shared.m_screen_data_type = _type;
		}

		public Object get_param( uint _param )
		{
			return m_behaviour.get_param( _param );
		}
		
		public bool set_param( uint _param, Object _val )
		{
			switch( _param )
			{
				case layout_editor_param.CONST_SET_BASE_MAP_SCALE_X1:
					{
						m_tmp_scale			= 1.0f;
						m_shared.m_scale	= 1.0f;

						set_high_quality_render_mode( false );
						
						clamp_offsets();
						
						update();
						
						return true;
					}

				case layout_editor_param.CONST_SET_BASE_MAP_SCALE_X2:
					{
						m_tmp_scale			= 1.1f;
						m_shared.m_scale	= 2.0f;
						
						set_high_quality_render_mode( false );
						
						clamp_offsets();
						
						update();
						
						return true;
					}
			}
		
			return m_behaviour.set_param( _param, _val );
		}
		
		public bool set_param( EMode _mode, uint _param, Object _val )
		{
			return m_behaviour_arr[ ( int )_mode ].set_param( _param, _val );
		}
		
		public void subscribe( EMode _mode, uint _param, Action< object, EventArgs > _method )
		{
			m_behaviour_arr[ ( int )_mode ].subscribe( _param, _method );
		}
		
		public void key_up_event( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.ControlKey )
			{
				m_enable_map_panning = false;
			}
		}

		public void key_down_event( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.ControlKey )
			{
				m_enable_map_panning = true;
			}
		}
	}
}
