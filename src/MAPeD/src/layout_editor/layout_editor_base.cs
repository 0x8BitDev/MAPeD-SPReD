/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 20.10.2022
 * Time: 12:48
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

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
		public const uint	CONST_SET_ENT_SELECT_TARGET			= 0x000080;
		public const uint	CONST_SET_ENT_INST_RESET_IF_EQUAL	= 0x000100;
		public const uint	CONST_SET_ENT_SNAPPING				= 0x000200;
		public const uint	CONST_SET_ENT_UPDATE_ENTS_CNT		= 0x000400;

		public const uint	CONST_SET_SCR_ACTIVE				= 0x000800;
		
		public const uint	CONST_SET_PNT_CLEAR_ACTIVE_TILE		= 0x001000;
		public const uint	CONST_SET_PNT_UPD_ACTIVE_TILE		= 0x002000;
		public const uint	CONST_SET_PNT_UPD_ACTIVE_BLOCK		= 0x004000;

		public const uint	CONST_SET_PTTRN_EXTRACT_BEGIN		= 0x008000;
		public const uint	CONST_SET_PTTRN_PLACE				= 0x010000;
		public const uint	CONST_SET_PTTRN_IDLE_STATE			= 0x020000;
		
		public const uint	CONST_SET_BASE_MAP_SCALE_X1			= 0x040000;
		public const uint	CONST_SET_BASE_MAP_SCALE_X2			= 0x080000;
		public const uint	CONST_SET_BASE_SUBSCR_DATA_MNGR		= 0x100000;

		// get
		public const uint	CONST_GET_ENT_INST_SELECTED			= 0x01;
		public const uint	CONST_GET_ENT_MODE					= 0x02;
		
		// subscribe
		public const uint	CONST_SUBSCR_ENT_INST_SELECT		= 0x01;

		public const uint	CONST_SUBSCR_SCR_RESET_SELECTED		= 0x02;
		
		public const uint	CONST_SUBSCR_PNT_UPDATE_TILE_IMAGE	= 0x04;
		
		public const uint	CONST_SUBSCR_PTTRN_EXTRACT_END		= 0x08;
		public const uint	CONST_SUBSCR_PTTRN_PLACE_CANCEL		= 0x10;
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
		
		public abstract void	reset( bool _init );
		
		public abstract void	mouse_down( object sender, MouseEventArgs e );
		public abstract void	mouse_up( object sender, MouseEventArgs e );
		public abstract bool	mouse_move( object sender, MouseEventArgs e );	// OUT: true - update(); false - invalidate();
		public abstract void	mouse_enter( object sender, EventArgs e );
		public abstract void	mouse_leave( object sender, EventArgs e );
		public abstract void	mouse_wheel( object sender, EventArgs e );

		public abstract bool	force_map_drawing();
		
		public abstract void	draw( SKSurface _surface, SKPaint _line_paint, SKPaint _image_paint, float _scr_size_width, float _scr_size_height );

		public abstract Object	get_param( uint _param );
		public abstract bool	set_param( uint _param, Object _val );
		
		public abstract void	subscribe( uint _param, Action< object, EventArgs > _method );

		public abstract void	key_down_event( object sender, KeyEventArgs e );
		public abstract void	key_up_event( object sender, KeyEventArgs e );
		
		public abstract layout_editor_base.EHelper	default_helper();
	}

	/// <summary>
	/// Description of layout_editor_helper_base.
	/// </summary>
	/// 
	
	public abstract class layout_editor_helper_base
	{
		protected readonly layout_editor_shared_data	m_shared;
		protected readonly layout_editor_base 			m_owner;
		
		protected readonly string	m_name;
		
		public layout_editor_helper_base( string _name, layout_editor_shared_data _shared, layout_editor_base _owner )
		{
			m_name		= _name;
			m_shared	= _shared;
			m_owner		= _owner;
		}

		public string	name()	{ return m_name; }
		
		public abstract void	reset( bool _init );
		
		public abstract void	mouse_down( object sender, MouseEventArgs e );
		public abstract void	mouse_up( object sender, MouseEventArgs e );
		public abstract void	mouse_move( object sender, MouseEventArgs e );
		
		public abstract void	draw( SKSurface _surface, SKPaint _line_paint, float _scr_size_width, float _scr_size_height );
		
		public abstract bool	check_key_code( KeyEventArgs e );
	}

	public class layout_editor_shared_data
	{
		public int				m_sel_screen_slot_id	= -1;
		public HashSet< int >	m_sel_screens_slot_ids	= null;
		
		public layout_data		m_layout				= null;
		
		public i_screen_list	m_scr_list				= null;
		public List< Bitmap >	m_tiles_imagelist		= null;
		public List< Bitmap >	m_blocks_imagelist		= null;

		public SKColorFilter	m_color_filter			= null;
		
		public SKRect			m_rect					= new SKRect();
		
		public image_cache		m_image_cache			= new image_cache();
		
		public float 	m_scale 			= 1;
		public int 		m_offset_x			= 0;
		public int 		m_offset_y			= 0;
 
		public float	m_fl_offset_x		= 0;
		public float	m_fl_offset_y		= 0;
		
		public int		m_mouse_x			= 10000;
		public int		m_mouse_y			= 10000;
 
		public int		m_last_mouse_x		= 0;
		public int		m_last_mouse_y		= 0;
 
		public int		m_scr_half_width	= 0;
		public int		m_scr_half_height	= 0;
		
		public bool		m_high_quality_render	= true;

		public int			m_CHR_bank_ind	= -1;
		public tiles_data 	m_tiles_data 	= null;
		
		public data_sets_manager.EScreenDataType	m_screen_data_type = data_sets_manager.EScreenDataType.sdt_Tiles4x4;
		
		// methods
		public Action< bool >					set_high_quality_render_mode;
		public Action< string, int, int >		print;
		public Action							pix_box_reset_capture;
		public Action							clamp_offsets;
		public Action< string >					err_msg;
		public Action< string >					sys_msg;
		
		public Action< int, int, tiles_data, data_sets_manager.EScreenDataType >	update_active_bank_screen;
		public Action< Action< int, layout_screen_data, SKRect > >					visible_screens_data_proc;
		
		public Func< bool >					pix_box_captured;
		public Func< int >					pix_box_width;
		public Func< int >					pix_box_height;
		public Func< int >					get_sel_scr_pos_x;
		public Func< int >					get_sel_scr_pos_y;
		public Func< int, float >			screen_pos_x_by_slot_id;
		public Func< int, float >			screen_pos_y_by_slot_id;
		public Func< bool, int >			get_sel_screen_ind;
		
		public Func< int, int, float >		transform_to_scr_pos;
		public Func< int, float, int, int >	transform_to_img_pos;
		
		public Func< int, int, int >		get_local_screen_ind;
		public Func< int, int >				get_bank_ind_by_global_screen_ind;
		
		public Func< SKSurface >			gfx_context;
		public Func< SKPaint >				paint_line;
		public Func< SKPaint >				paint_image;
	}
	
	/// <summary>
	/// Description of layout_editor_base.
	/// </summary>
	
	public class layout_editor_base : drawable_SKGL
	{
		public event EventHandler MapScaleX1;
		public event EventHandler MapScaleX2;
		
		private readonly screen_mark_form			m_screen_mark_form	= new screen_mark_form();
		private readonly layout_editor_shared_data	m_shared;
		
		private readonly Label m_label;
		
		private string	m_err_msg			= "";
		private int		m_err_msg_upd_cnt	= 0;
		
		private string	m_sys_msg = "";
		
		private const int CONST_ERR_MSG_UPD_CNT	= 5;
		
		private float 	m_tmp_scale			= 1;
		
		private bool 	m_show_marks		= true;
		private bool 	m_show_entities		= true;
		private bool 	m_show_targets		= true;
		private bool 	m_show_coords		= true;
		private bool 	m_show_grid			= true;

		private readonly SKBitmap	m_scr_mark_img;
		private readonly SKCanvas	m_scr_mark_canvas;
		
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
			em_Patterns,
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
					m_behaviour.reset( false );
				}
				
				m_mode = value;
				
				m_behaviour = m_behaviour_arr[ ( int )value ];
				
				m_behaviour.reset( true );
				
				apply_default_helper();
				
				m_err_msg = m_sys_msg = "";

				update();
			}
		}
		
		private layout_editor_behaviour_base			m_behaviour		= null;
		private readonly layout_editor_behaviour_base[]	m_behaviour_arr;

		public enum EHelper
		{
			eh_Panning = 0,
			eh_ScrMultisel,
			eh_MAX,
			eh_Unknown,
		}
		
		private layout_editor_helper_base				m_helper		= null;
		private readonly layout_editor_helper_base[]	m_helper_arr;
		
		public layout_editor_base( data_sets_manager _data_mngr, SKGLControl _pbox, Label _label, imagelist_manager _img_list_mngr ) : base( _pbox )
		{
			m_label		= _label;
			
			// init shared data
			{
				m_shared = new layout_editor_shared_data();
				
				// data
				m_shared.m_scr_list			= _img_list_mngr.get_screen_list();
				m_shared.m_tiles_imagelist	= _img_list_mngr.get_tiles_image_list();
				m_shared.m_blocks_imagelist	= _img_list_mngr.get_blocks_image_list();
				
				m_shared.m_sel_screens_slot_ids			= new HashSet< int >();
				
				// method
				m_shared.set_high_quality_render_mode	= set_high_quality_render_mode;
				m_shared.pix_box_reset_capture			= pix_box_reset_capture;
				m_shared.pix_box_captured				= pix_box_captured;
				m_shared.pix_box_width					= pix_box_width;
				m_shared.pix_box_height					= pix_box_height;
				m_shared.print							= print;
				m_shared.clamp_offsets					= clamp_offsets;
				m_shared.err_msg						= err_msg;
				m_shared.sys_msg						= sys_msg;
				
				m_shared.update_active_bank_screen		= _img_list_mngr.update_active_bank_screen;
				m_shared.visible_screens_data_proc		= visible_screens_data_proc; 
				
				m_shared.get_sel_scr_pos_x				= get_sel_scr_pos_x; 
				m_shared.get_sel_scr_pos_y 				= get_sel_scr_pos_y;
				m_shared.get_sel_screen_ind				= get_sel_screen_ind;
				m_shared.screen_pos_x_by_slot_id		= screen_pos_x_by_slot_id;
				m_shared.screen_pos_y_by_slot_id		= screen_pos_y_by_slot_id;
				m_shared.transform_to_scr_pos			= transform_to_scr_pos;
				m_shared.transform_to_img_pos			= transform_to_img_pos;
				
				m_shared.get_local_screen_ind				= _data_mngr.get_local_screen_ind;
				m_shared.get_bank_ind_by_global_screen_ind	= _data_mngr.get_bank_ind_by_global_screen_ind;
				
				m_shared.gfx_context					= gfx_context;
				m_shared.paint_line						= paint_line;
				m_shared.paint_image					= paint_image;
			}
			
			m_pix_box.MouseDown 	+= new MouseEventHandler( Layout_MouseDown );
			m_pix_box.MouseUp		+= new MouseEventHandler( Layout_MouseUp );

			m_pix_box.MouseMove		+= new MouseEventHandler( Layout_MouseMove );
			m_pix_box.MouseWheel	+= new MouseEventHandler( Layout_MouseWheel );
			
			m_pix_box.MouseEnter 	+= new EventHandler( Layout_MouseEnter );
			m_pix_box.MouseLeave	+= new EventHandler( Layout_MouseLeave );
			
			m_pix_box.ContextMenuStrip.Opened			+= new EventHandler( Layout_ContextMenuOpened );
			m_pix_box.ContextMenuStrip.Closed			+= new ToolStripDropDownClosedEventHandler( Layout_ContextMenuClosed );
			m_pix_box.ContextMenuStrip.PreviewKeyDown	+= new PreviewKeyDownEventHandler( Layout_ContextMenuPreviewKeyDown );
			
			// screen mark data
			{
				m_scr_mark_img		= new SKBitmap( platform_data.get_screen_mark_image_size(), platform_data.get_screen_mark_image_size(), utils.CONST_BMP_FORMAT, SKAlphaType.Premul );
				m_scr_mark_canvas	= new SKCanvas( m_scr_mark_img );
			}
			
			m_shared.m_scr_half_width  = m_pix_box.Width >> 1;
			m_shared.m_scr_half_height = m_pix_box.Height >> 1;
			
			m_shared.m_color_filter = SKColorFilter.CreateBlendMode( SKColor.Parse( "#80ffffff" ), SKBlendMode.DstOut ); 
			
			// init helpers
			{
				m_helper_arr = new layout_editor_helper_base[ ( int )EHelper.eh_MAX ]
				{
					new layout_editor_helper_panning( "VIEWPORT PANNING MODE", m_shared, this ),
					new layout_editor_helper_scr_multisel( "MULTIPLE SCREEN SELECTION MODE", m_shared, this )
				};
			}
			
			// init available behaviours
			{
				m_behaviour_arr = new layout_editor_behaviour_base[ ( int )EMode.em_MAX ]
				{ 
					new layout_editor_builder( "builder", m_shared, this ),
					new layout_editor_painter( "painter", m_shared, this ),
					new layout_editor_screen_list( "screen list", m_shared, this ), 
					new layout_editor_entities( "entities", m_shared, this ),
					new layout_editor_patterns( "patterns", m_shared, this )
				};
			}
			
			reset( true );
		}
		
		protected override void Resize_Event(object sender, EventArgs e)
		{
			m_shared.m_scr_half_width  = m_pix_box.Width >> 1;
			m_shared.m_scr_half_height = m_pix_box.Height >> 1;
			
			m_pbox_rect.Right	= m_pix_box.Width;
			m_pbox_rect.Bottom	= m_pix_box.Height;
			
			clamp_offsets();
			
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
			
			m_shared.m_fl_offset_x = 0;
			m_shared.m_fl_offset_y = 0;
			
			set_high_quality_render_mode( true );
			
			// reset all behaviours
			foreach( var bhv in m_behaviour_arr )
			{
				bhv.reset( _init );
			}
			
			// reset helpers
			foreach( var hlp in m_helper_arr )
			{
				hlp.reset( _init );
			}
			
			m_err_msg = m_sys_msg = "";
			
			reset_selected_screens();
			
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
				
				if( m_helper != null )
				{
					m_helper.mouse_down( sender, e );
				}
				else
				{
					reset_selected_screens();
				}
			}
			else
			if( e.Button == MouseButtons.Right )
			{
				// to avoid one frame map image without grid (mouse_down->mouse_move->map_update)
				pix_box_reset_capture();
			}

			if( m_helper == null )
			{
				m_behaviour.mouse_down( sender, e );
			}
		}

		private void Layout_MouseUp(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				pix_box_reset_capture();
				
				set_high_quality_render_mode( true );
				
				if( m_helper != null )
				{
					m_helper.mouse_up( sender, e );
				}
			}
			
			if( m_helper == null )
			{
				m_behaviour.mouse_up( sender, e );
			}
			
			if( e.Button == MouseButtons.Left )
			{
				if( m_helper == null )
				{
					if( show_grid )
					{
						update();
					}
				}
			}
		}
		
		private void Layout_MouseMove(object sender, MouseEventArgs e)
		{
			m_shared.m_mouse_x = e.X;
			m_shared.m_mouse_y = e.Y;
			
			// calculate selected screen position
			if( m_shared.m_layout != null )
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
			
			if( m_helper == null )
			{
				if( m_pix_box.Capture && e.Button == MouseButtons.Left )
				{
					sys_msg( "Hold down: 'Ctrl' to pan the viewport, 'Shift' to select multiple screens" );
				}
				
				if( m_behaviour.mouse_move( sender, e ) )
				{
					update();
				}
				else
				{
					invalidate();
				}
			}
			else
			{
				m_helper.mouse_move( sender, e );
				
				update();
			}
		}
		
		private void Layout_MouseWheel(object sender, MouseEventArgs e)
		{
			m_tmp_scale += ( float )e.Delta / 6000;
			
			m_tmp_scale = m_shared.m_scale = m_tmp_scale < CONST_MIN_SCALE ? CONST_MIN_SCALE:m_tmp_scale;
			
			if( m_shared.m_scale > 1.02f )
			{
				m_shared.m_scale = 2;
				m_tmp_scale = 1.02f;
				
				enable_smoothing_mode( false );
			}
			else
			{
				enable_smoothing_mode( true );
			}
			
			if( e.Delta < 0 )
			{
				if( m_shared.m_scale < 1.02f && m_shared.m_scale > 1 )
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
		
		private void Layout_MouseLeave(object sender, EventArgs e)
		{
			m_label.Focus();
			
			// hide mouse position to hide an active object
			if( !m_pix_box.ContextMenuStrip.Visible )
			{
				m_shared.m_mouse_x = 10000;
				m_shared.m_mouse_y = 10000;
				
				m_behaviour.mouse_leave( sender, e );
				
				update();
			}
		}
		
		private void Layout_ContextMenuOpened(object sender, EventArgs e)
		{
			draw_sel_screen_border();
		}

		private void Layout_ContextMenuClosed(object sender, EventArgs e)
		{
			// hide the selected screen border
			update();
		}
		
		private void Layout_ContextMenuPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if( e.KeyCode == Keys.Escape )
			{
				// hide the selected screen border
				update();
			}
		}
		
		public void draw_sel_screen_border()
		{
			if( m_shared.m_sel_screens_slot_ids.Count == 0 && m_shared.m_sel_screen_slot_id >= 0 )
			{
				// draw a selected screen border
				int scr_size_width 	= ( int )( platform_data.get_screen_width_pixels() * m_shared.m_scale );
				int scr_size_height = ( int )( platform_data.get_screen_height_pixels() * m_shared.m_scale );
				
				int x = ( int )screen_pos_x_by_slot_id( get_sel_scr_pos_x() );
				int y = ( int )screen_pos_y_by_slot_id( get_sel_scr_pos_y() );
				
				m_line_paint.StrokeWidth = 2;
				m_line_paint.Color = utils.CONST_COLOR_SCREEN_SELECTED_BORDER;
				
				m_surface.Canvas.DrawRect( x, y, scr_size_width, scr_size_height, m_line_paint );
				
				invalidate();
			}
		}
		
		private void clamp_offsets()
		{
			if( m_shared.m_layout != null )
			{
				float width		= ( float )( get_width() * platform_data.get_screen_width_pixels() );
				float height	= ( float )( get_height() * platform_data.get_screen_height_pixels() );
				
				float width_scaled	= m_shared.m_scale * width;
				float height_scaled	= m_shared.m_scale * height;
				
				float tx = ( float )m_shared.m_scr_half_width - ( m_shared.m_scr_half_width / m_shared.m_scale );
				float ty = ( float )m_shared.m_scr_half_height - ( m_shared.m_scr_half_height / m_shared.m_scale );
				
				float pbox_width	= ( float )m_pix_box.Width;
				float pbox_height	= ( float )m_pix_box.Height;
	
				if( width_scaled < pbox_width )
				{
					m_shared.m_fl_offset_x = ( float )m_shared.m_scr_half_width - ( width * 0.5f );
				}
				else
				{
					m_shared.m_fl_offset_x = ( ( m_shared.m_fl_offset_x + width + tx ) < pbox_width ) ? ( pbox_width - width - tx ):m_shared.m_fl_offset_x;
					m_shared.m_fl_offset_x = m_shared.m_fl_offset_x > tx ? tx:m_shared.m_fl_offset_x;
				}
				
				if( height_scaled < pbox_height )
				{
					m_shared.m_fl_offset_y = ( float )m_shared.m_scr_half_height - ( height * 0.5f );
				}
				else
				{
					m_shared.m_fl_offset_y = ( ( m_shared.m_fl_offset_y + height + ty ) < pbox_height ) ? ( pbox_height - height - ty ):m_shared.m_fl_offset_y;
					m_shared.m_fl_offset_y = m_shared.m_fl_offset_y > ty ? ty:m_shared.m_fl_offset_y;
				}
				
				m_shared.m_offset_x = ( int )m_shared.m_fl_offset_x;
				m_shared.m_offset_y = ( int )m_shared.m_fl_offset_y;
			}
		}

		private float transform_to_scr_pos( int _pos, int _half_scr )
		{
			return ( ( _pos - _half_scr ) * m_shared.m_scale + _half_scr );
		}

		private int transform_to_img_pos( int _pos, float _offset, int _half_scr )
		{
			return ( int )( ( float )( _pos - _offset ) / m_shared.m_scale - ( m_shared.m_scale - 1 ) * ( ( float )( _offset - _half_scr ) / m_shared.m_scale ) );
		}
		
		private void visible_screens_data_proc( Action< int, layout_screen_data, SKRect > _act )
		{
			layout_screen_data scr_data;
			
			float scr_x;
			float scr_y;
			
			int beg_scr_x;
			int end_scr_x;
			int beg_scr_y;
			int end_scr_y;
			int i;
			int j;
			
			int scr_width	= get_width();
			int scr_height	= get_height();
			
			int scr_size_width 	= ( int )( platform_data.get_screen_width_pixels() * m_shared.m_scale );
			int scr_size_height = ( int )( platform_data.get_screen_height_pixels() * m_shared.m_scale );
			
			// calculate visible region of screens
			{
				int offs_x		= ( int )transform_to_scr_pos( m_shared.m_offset_x, m_shared.m_scr_half_width );
				int offs_y		= ( int )transform_to_scr_pos( m_shared.m_offset_y, m_shared.m_scr_half_height );
				
				int vp_width	= m_pix_box.Width;
				int vp_height	= m_pix_box.Height;
				
				float scr_size_width_flt	= ( float )scr_size_width;
				float scr_size_height_flt	= ( float )scr_size_height;
				
				beg_scr_x = Math.Max( 0, -( int )Math.Ceiling( offs_x / scr_size_width_flt ) );
				end_scr_x = Math.Min( scr_width - 1, beg_scr_x + ( int )Math.Ceiling( vp_width / scr_size_width_flt ) );
				beg_scr_y = Math.Max( 0, -( int )Math.Ceiling( offs_y / scr_size_height_flt ) );
				end_scr_y = Math.Min( scr_height - 1, beg_scr_y + ( int )Math.Ceiling( vp_height / scr_size_height_flt ) );
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
						
						m_shared.m_rect.Left	= scr_x;
						m_shared.m_rect.Top 	= scr_y;
						m_shared.m_rect.Right	= scr_x + scr_size_width;
						m_shared.m_rect.Bottom	= scr_y + scr_size_height;
						
						if( m_pbox_rect.IntersectsWith( m_shared.m_rect ) )
						{
							_act( ( ( i * scr_width ) + j ), scr_data, m_shared.m_rect );
						}
					}
				}
			}
		}
		
		private void draw_screen_data( int _scr_width, int _scr_height, float _scr_size_width, float _scr_size_height, Action< int, layout_screen_data, float, float > _act )
		{
			layout_screen_data scr_data;
			
			int beg_scr_x;
			int end_scr_x;
			int beg_scr_y;
			int end_scr_y;
			int i;
			int j;
			
			float scr_x;
			float scr_y;
			
			// calculate visible region of screens
			{
				float offs_x	= transform_to_scr_pos( m_shared.m_offset_x, m_shared.m_scr_half_width );
				float offs_y	= transform_to_scr_pos( m_shared.m_offset_y, m_shared.m_scr_half_height );
				
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

						m_shared.m_rect.Left	= scr_x;
						m_shared.m_rect.Top 	= scr_y;
						m_shared.m_rect.Right	= m_shared.m_rect.Left + _scr_size_width;
						m_shared.m_rect.Bottom	= m_shared.m_rect.Top + _scr_size_height;
						
						if( m_pbox_rect.IntersectsWith( m_shared.m_rect ) )
						{
							_act( ( ( i * _scr_width ) + j ), scr_data, scr_x, scr_y );
						}
					}
				}
			}
		}
		
		private void draw_targets( int _scr_width, int _scr_height )
		{
			layout_screen_data scr_data 	= null;
			entity_instance	ent 	= null;
			
			float targ_scr_x;
			float targ_scr_y;
			float scr_x;
			float scr_y;
			
			int i;
			int j;
			
			int scr_ind = 0;
			
			for( i = 0; i < _scr_height; i++ )
			{
				scr_y = screen_pos_y_by_slot_id( i );
				
				for( j = 0; j < _scr_width; j++ )
				{
					scr_data = m_shared.m_layout.get_data( j, i );
					
					if( scr_data.m_scr_ind != layout_data.CONST_EMPTY_CELL_ID )
					{
						scr_x = screen_pos_x_by_slot_id( j );
						
						scr_data.entities_proc( delegate( entity_instance _ent_inst )
						{
							if( _ent_inst.target_uid >= 0 )
							{
								if( m_shared.m_layout.get_entity_by_uid( _ent_inst.target_uid, ref scr_ind, ref ent ) )
								{
									targ_scr_x = screen_pos_x_by_slot_id( scr_ind % get_width() );
									targ_scr_y = screen_pos_y_by_slot_id( scr_ind / get_width() );
									
									m_line_paint.Color = utils.CONST_COLOR_TARGET_LINK;
									{
										m_surface.Canvas.DrawLine(	scr_x + ( ( _ent_inst.x + ( _ent_inst.base_entity.width >> 1 ) ) * m_shared.m_scale ),
																	scr_y + ( ( _ent_inst.y + ( _ent_inst.base_entity.height >> 1 ) ) * m_shared.m_scale ),
																	targ_scr_x + ( ( ent.x + ( ent.base_entity.width >> 1 ) ) * m_shared.m_scale ),
																	targ_scr_y + ( ( ent.y + ( ent.base_entity.height >> 1 ) ) * m_shared.m_scale ),
																	m_line_paint );
									}
								}
							}
						});
					}
				}
			}
		}

		private void print( string _text, int _x, int _y )
		{
			uint font_size = 11;
			
			SKPaint font_paint = utils.get_font( font_size, true );
			
			int font_height = ( int )font_paint.FontSpacing;
			
			font_paint.Color = utils.CONST_COLOR_LAYOUT_STRING_DEFAULT_SHADOW;
			m_surface.Canvas.DrawText( _text, _x + 1, _y + 1 + font_height, font_paint );
			
			font_paint.Color = utils.CONST_COLOR_LAYOUT_STRING_DEFAULT;
			m_surface.Canvas.DrawText( _text, _x, _y + font_height, font_paint );
		}
		
		private void err_msg( string _msg )
		{
			m_err_msg = _msg;
			m_err_msg_upd_cnt = CONST_ERR_MSG_UPD_CNT;
		}

		private void sys_msg( string _msg )
		{
			m_sys_msg = _msg;
		}
		
		public override void update()
		{
			if( m_surface == null )
			{
				return;
			}
			
			clear_background( CONST_BACKGROUND_COLOR );
			
			m_line_paint.StrokeWidth = 1;

			if( m_shared.m_layout != null )
			{
				int width	= get_width();
				int height	= get_height();
				
				float scr_size_width	= ( platform_data.get_screen_width_pixels() * m_shared.m_scale );
				float scr_size_height	= ( platform_data.get_screen_height_pixels() * m_shared.m_scale );
				
				float x;
				float y;
				int i;
				int j;
				
				bool line_antialias = m_line_paint.IsAntialias;
				
				if( m_shared.m_scr_list.count() > 0 )
				{
					draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_slot_ind, layout_screen_data _scr_data, float _x, float _y )
					{ 
						utils.draw_skbitmap( m_surface.Canvas, m_shared.m_scr_list.get_skbitmap( _scr_data.m_scr_ind ), _x, _y, scr_size_width, scr_size_height, m_image_paint );
					});
				}
				
				if( show_grid && ( !m_shared.pix_box_captured() || m_behaviour.force_map_drawing() ) )
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
						
						m_line_paint.Color = utils.CONST_COLOR_GRID_BLOCKS;
						
						bool tile4x4_grid = ( ( platform_data.get_screen_blocks_height( false ) & 0x01 ) != 0x01 ) && ( ( platform_data.get_screen_blocks_width( false ) & 0x01 ) != 0x01 ) && ( m_shared.m_screen_data_type != data_sets_manager.EScreenDataType.sdt_Blocks2x2 );
						
						for( i = x_line_beg; i < n_lines_x + x_line_beg; i++ )
						{
							x_pos = x_line_offset + i*step;
							
							offs_x = transform_to_scr_pos( x_pos, m_shared.m_scr_half_width );

							if( tile4x4_grid )
							{
								if( ( ( x_pos - m_shared.m_offset_x ) % utils.CONST_SCREEN_BLOCKS_SIZE ) == 0 )
								{
									m_line_paint.Color = utils.CONST_COLOR_GRID_TILES_BRIGHT;
								}
								else
								{
									m_line_paint.Color = utils.CONST_COLOR_GRID_TILES_DARK;
								}
							}
							
							m_surface.Canvas.DrawLine( offs_x, 0, offs_x, m_pix_box.Height, m_line_paint );
						}
						
						for( i = y_line_beg; i < n_lines_y + y_line_beg; i++ )
						{
							y_pos = y_line_offset + i*step;
							
							offs_y = transform_to_scr_pos( y_pos, m_shared.m_scr_half_height );
							
							if( tile4x4_grid )
							{
								if( ( ( y_pos - m_shared.m_offset_y ) % utils.CONST_SCREEN_BLOCKS_SIZE ) == 0 )
								{
									m_line_paint.Color = utils.CONST_COLOR_GRID_TILES_BRIGHT;
								}
								else
								{
									m_line_paint.Color = utils.CONST_COLOR_GRID_TILES_DARK;
								}
							}
							
							m_surface.Canvas.DrawLine( 0, offs_y, m_pix_box.Width, offs_y, m_line_paint );
						}
					}
				}
				
				// draw screens grid
				{
					m_line_paint.Color = ( m_shared.m_scr_list.count() > 0 ) ? utils.CONST_COLOR_SCREEN_LIST_NOT_EMPTY:utils.CONST_COLOR_SCREEN_LIST_EMPTY;
					m_line_paint.IsAntialias = m_shared.m_scale < 1;
					
					for( i = 0; i < height + 1; i++ )
					{
						y = screen_pos_y_by_slot_id( i );
						
						if( y >= 0 && y < m_pix_box.Height )
						{
							m_surface.Canvas.DrawLine( transform_to_scr_pos( m_shared.m_offset_x, m_shared.m_scr_half_width ), y, screen_pos_x_by_slot_id( width ), y, m_line_paint );
						}
					}
					
					for( j = 0; j < width + 1; j++ )
					{
						x = screen_pos_x_by_slot_id( j );
						
						if( x >= 0 && x < m_pix_box.Width )
						{
							m_surface.Canvas.DrawLine( x, transform_to_scr_pos( m_shared.m_offset_y, m_shared.m_scr_half_height ), x, screen_pos_y_by_slot_id( height ), m_line_paint );
						}
					}
					
					m_line_paint.IsAntialias = line_antialias;
				}

				if( show_entities )
				{
					if( show_targets )
					{
						m_line_paint.IsAntialias = true;
						
						draw_targets( width, height );
						
						m_line_paint.IsAntialias = line_antialias;
					}
					
					draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_slot_ind, layout_screen_data _scr_data, float _scr_x, float _scr_y ) 
					{ 
						_scr_data.entities_proc( delegate( entity_instance _ent_inst )
						{
							utils.draw_skbitmap( m_surface.Canvas, m_shared.m_image_cache.get( _ent_inst.base_entity.bitmap ), _scr_x + ( int )( _ent_inst.x * m_shared.m_scale ), _scr_y + ( int )( _ent_inst.y * m_shared.m_scale ), ( int )_ent_inst.base_entity.width * m_shared.m_scale, ( int )_ent_inst.base_entity.height * m_shared.m_scale, m_image_paint );
						});
					});
				}
				
				if( show_marks )
				{
					float scr_half_width	= scr_size_width * 0.5f;
					float scr_half_height	= scr_size_height * 0.5f;
					
					m_line_paint.IsAntialias = true;
					
					SKPaint start_scr_font	= utils.get_font( 82, true );
					SKPaint scr_mark_font	= utils.get_font( 58, true );
					
					draw_screen_data( width, height, scr_size_width, scr_size_height, delegate( int _scr_slot_ind, layout_screen_data _scr_data, float _x, float _y )
					{ 
						if( m_shared.m_layout.get_start_screen_ind() == _scr_slot_ind )
						{
							update_mark( utils.CONST_COLOR_MARK_START_SCREEN, delegate() { m_scr_mark_canvas.DrawText( "S", 36, 90, start_scr_font ); } );
							
							draw_mark( _x, _y, scr_half_width, scr_half_height );
						}
						
						if( _scr_data.mark > 0 )
						{
							update_mark( utils.CONST_COLOR_MARK_SCREEN_MARK, delegate() { m_scr_mark_canvas.DrawText( _scr_data.mark.ToString( "D2" ), 10, 55, scr_mark_font ); } );
							
							draw_mark( _x + scr_half_width, _y + scr_half_height, scr_half_width, scr_half_height );
						}
						
						if( _scr_data.adj_scr_mask > 0 )
						{
							update_mark( utils.CONST_COLOR_MARK_ADJ_SCREEN_MARK, delegate(){}, true );
							
							m_line_paint.Color = utils.CONST_COLOR_LAYOUT_PIXBOX_DEFAULT;
							{
								int img_center 	= platform_data.get_screen_mark_image_size() >> 1;
								int radius		= 12;
								int arrow_len	= 35;
								int offs		= 7;
								
								// o
								m_line_paint.StrokeWidth = 4;
								m_scr_mark_canvas.DrawCircle( img_center, img_center, radius, m_line_paint );
								
								m_line_paint.StrokeWidth	= 15;
								m_line_paint.StrokeCap		= SKStrokeCap.Square;
								
								if( ( _scr_data.adj_scr_mask & 0x01 ) != 0 )
								{
									// L
									m_scr_mark_canvas.DrawLine( img_center - radius - offs, img_center,  img_center - arrow_len - radius, img_center, m_line_paint );
								}
								if( ( _scr_data.adj_scr_mask & 0x02 ) != 0 )
								{
									// U
									m_scr_mark_canvas.DrawLine( img_center, img_center - radius - offs,  img_center, img_center - arrow_len - radius, m_line_paint );
								}
								if( ( _scr_data.adj_scr_mask & 0x04 ) != 0 )
								{
									// R
									m_scr_mark_canvas.DrawLine( img_center + radius + offs, img_center,  img_center + arrow_len + radius, img_center, m_line_paint );
								}
								if( ( _scr_data.adj_scr_mask & 0x08 ) != 0 )
								{
									// D
									m_scr_mark_canvas.DrawLine( img_center, img_center + radius + offs,  img_center, img_center + arrow_len + radius, m_line_paint );
								}
								
								m_line_paint.StrokeWidth	= 1;
							}
							
							draw_mark( _x + scr_half_width, _y, scr_half_width, scr_half_height );
						}
					});
					
					m_line_paint.IsAntialias = line_antialias;
				}
				
				// draw selected screens
				if( m_shared.m_sel_screens_slot_ids.Count > 0 )
				{
					m_line_paint.StrokeWidth = 2;
					m_line_paint.Color = utils.CONST_COLOR_SCREEN_SELECTED_BORDER;
					
					foreach( int scr_slot_ind in m_shared.m_sel_screens_slot_ids )
					{
						x = screen_pos_x_by_slot_id( scr_slot_ind % get_width() );
						y = screen_pos_y_by_slot_id( scr_slot_ind / get_width() );
						
						m_surface.Canvas.DrawRect( x, y, scr_size_width, scr_size_height, m_line_paint );
					}
				}
				
				// draw a helper specific data
				if( m_helper != null )
				{
					m_helper.draw( m_surface, m_line_paint, scr_size_width, scr_size_height );
				}
				
				// draw data specific to active behaviour
				m_behaviour.draw( m_surface, m_line_paint, m_image_paint, scr_size_width, scr_size_height );
				
				print( "mode: " + m_behaviour.name(), 2, 0 );
				
				SKPaint sys_font = utils.get_font( 11, true );
				
				// print system message
				{
					if( m_sys_msg.Length == 0 )
					{
						if( m_helper != null )
						{
							sys_msg( m_helper.name() );
						}
					}
					
					print( m_sys_msg, ( m_pix_box.Width >> 1 ) - ( ( int )( sys_font.MeasureText( m_sys_msg ) ) >> 1 ), 0 );
					
					sys_msg( "" );
				}
				
				// print error message
				if( m_err_msg_upd_cnt > 0 )
				{
					print( m_err_msg, ( m_pix_box.Width >> 1 ) - ( ( int )( sys_font.MeasureText( m_err_msg ) ) >> 1 ), m_pix_box.Height - ( ( int )sys_font.FontSpacing + 1 ) );
					
					--m_err_msg_upd_cnt;
				}
				
				if( disable( false ) )
				{
					invalidate();
				}
			}
			else
			{
				// draw the red cross as a sign of inactive state
				m_line_paint.Color = utils.CONST_COLOR_LAYOUT_PIXBOX_INACTIVE_CROSS;
				
				m_surface.Canvas.DrawLine( 0, 0, m_pix_box.Width, m_pix_box.Height, m_line_paint );
				m_surface.Canvas.DrawLine( m_pix_box.Width, 0, 0, m_pix_box.Height, m_line_paint );
				
				print( "For all the tabs:", 0, 0 );
				print( "- Use a mouse wheel to scale a map in the viewport", 0, 10 );
				print( "- A quick roll of the mouse wheel scales an active map up to 2x", 0, 20 );
				print( "- Hold down the 'Ctrl' key to pan a map in the viewport", 0, 30 );
				print( "- Hold down the 'Shift' key to select multiple screens", 0, 40 );
				print( "Use a mouse wheel to scale an entity/pattern preview", 0, 60 );
				
				if( !disable( true ) )
				{
					invalidate();
				}
			}
		}
		
		private void selected_screens_proc( Action< int > _act )
		{
			if( m_shared.m_sel_screens_slot_ids.Count > 0 )
			{
				foreach( int scr_ind in m_shared.m_sel_screens_slot_ids )
				{
					_act( scr_ind );
				}
				
				reset_selected_screens();
			}
			else
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				_act( m_shared.m_sel_screen_slot_id );
			}
		}
		
		private bool selected_screens()
		{
			return ( m_shared.m_sel_screens_slot_ids.Count > 0 ) || ( m_shared.m_sel_screen_slot_id >= 0 );
		}
		
		private void reset_selected_screens()
		{
			m_shared.m_sel_screens_slot_ids.Clear();
		}
		
		public bool delete_screen_from_layout()
		{
			bool res = false;
			
			if( selected_screens() )
			{
				draw_sel_screen_border();
				
				if( MainForm.message_box( "Are you sure?", "Delete Screens", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning ) == DialogResult.Yes )
				{
					selected_screens_proc( delegate( int _scr_slot_ind )
					{
						int scr_pos_x = _scr_slot_ind % get_width();
						int scr_pos_y = _scr_slot_ind / get_width();
						
						layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
						
						if( mode == EMode.em_Entities )
						{
							scr_data.entities_proc( delegate( entity_instance _ent_inst ) { set_param( layout_editor_param.CONST_SET_ENT_INST_RESET_IF_EQUAL, _ent_inst ); } );
						}
						
						m_shared.m_layout.delete_screen_by_pos( scr_pos_x, scr_pos_y );
					});
					
					res = true;
				}
			}
			else
			{
				MainForm.message_box( "Please, select screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			update();
			
			return res;
		}
		
		public bool delete_screen_entities()
		{
			bool res = false;
			
			if( selected_screens() )
			{
				draw_sel_screen_border();
				
				if( MainForm.message_box( "Are you sure?", "Delete Screen Entities", MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning ) == DialogResult.Yes )
				{
					selected_screens_proc( delegate( int _scr_slot_ind )
					{
						int scr_pos_x = _scr_slot_ind % get_width();
						int scr_pos_y = _scr_slot_ind / get_width();
						
						layout_screen_data scr_data = m_shared.m_layout.get_data( scr_pos_x, scr_pos_y );
						
						if( mode == EMode.em_Entities )
						{
							scr_data.entities_proc( delegate( entity_instance _ent_inst ) { set_param( layout_editor_param.CONST_SET_ENT_INST_RESET_IF_EQUAL, _ent_inst ); } );
						}
						
						m_shared.m_layout.delete_entity_instances( scr_data );
					});
					
					res = true;
				}
			}
			else
			{
				MainForm.message_box( "Please, select screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			update();
			
			return res;
		}

		private float screen_pos_x_by_slot_id( int _slot_id )
		{
			return transform_to_scr_pos( _slot_id * platform_data.get_screen_width_pixels() + m_shared.m_offset_x, m_shared.m_scr_half_width );
		}
		
		private float screen_pos_y_by_slot_id( int _slot_id )
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
		
		private int get_sel_screen_ind( bool _show_sys_msg )
		{
			int res = -1;
			
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				int scr_glob_ind = m_shared.m_layout.get_data( m_shared.get_sel_scr_pos_x(), m_shared.get_sel_scr_pos_y() ).m_scr_ind;
				
				if( scr_glob_ind != layout_data.CONST_EMPTY_CELL_ID )
				{
					if( m_shared.get_bank_ind_by_global_screen_ind( scr_glob_ind ) == m_shared.m_CHR_bank_ind )
					{
						return scr_glob_ind;
					}
					else
					{
						if( _show_sys_msg )
						{
							err_msg( "WRONG CHR BANK" );
						}
					}
				}
			}
			
			return res;
		}
		
		private int get_width()
		{
			return m_shared.m_layout.get_width();
		}

		private int get_height()
		{
			return m_shared.m_layout.get_height();
		}

		private void update_layout_data( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			m_shared.m_layout = data_mngr.get_layout_data( data_mngr.layouts_data_pos );
			
			reset_selected_screens();
			
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

		private void update_mark( SKColor _color, Action _act, bool _clear = true )
		{
			if( _clear )
			{
				m_scr_mark_canvas.Clear( _color );
			}
			
			_act();
		}
		
		private void draw_mark( float _x, float _y, float _width, float _height )
		{
			utils.draw_skbitmap( m_surface.Canvas, m_scr_mark_img, _x, _y, _width, _height, m_line_paint );
		}

		public bool set_start_screen_mark()
		{
			bool res = false;
			
			if( m_shared.m_sel_screen_slot_id >= 0 )
			{
				res = m_shared.m_layout.set_start_screen_ind( m_shared.m_sel_screen_slot_id );
				
				update();
			}
			
			if( res == false )
			{
				MainForm.message_box( "Please, select a valid screen!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			return res;
		}
		
		public bool set_screen_mark()
		{
			bool res = false;
		
			if( selected_screens() )
			{
				draw_sel_screen_border();
				
				if( m_screen_mark_form.ShowDialog() == DialogResult.OK )
				{
					selected_screens_proc( delegate( int _scr_slot_ind )
					{
						res |= m_shared.m_layout.set_screen_mark( _scr_slot_ind, m_screen_mark_form.mark );
					});
					
					update();
					
					if( res == false )
					{
						MainForm.message_box( "Please, select valid screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
				else
				{
					update();
				}
			}
			else
			{
				MainForm.message_box( "Please, select screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
			return res;
		}
		
		public bool set_adjacent_screen_mask( string _mask )
		{
			bool res = false;
			
			int mask = 0;
			
			if( selected_screens() )
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
				
				selected_screens_proc( delegate( int _scr_slot_ind )
				{
					res |= m_shared.m_layout.set_adjacent_screen_mask( _scr_slot_ind, mask );
				});
				
				update();
				
				if( res == false )
				{
					MainForm.message_box( "Please, select valid screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
			else
			{
				MainForm.message_box( "Please, select screen(s)!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
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
		
		private SKSurface gfx_context()
		{
			return m_surface;
		}
		
		private SKPaint	paint_line()
		{
			return m_line_paint;
		}
		
		private SKPaint	paint_image()
		{
			return m_image_paint;
		}
		
		public void set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			m_shared.m_screen_data_type = _type;
		}
		
		private void new_data_set( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			m_shared.m_CHR_bank_ind	= data_mngr.tiles_data_pos;
			m_shared.m_tiles_data	= data_mngr.get_tiles_data( data_mngr.tiles_data_pos );
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
					
				case layout_editor_param.CONST_SET_BASE_SUBSCR_DATA_MNGR:
					{
						( ( data_sets_manager )_val ).SetTilesData += new EventHandler( new_data_set );
						
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
			if( m_pix_box.Visible )
			{
				foreach( var helper in m_helper_arr )
				{
					if( helper.check_key_code( e ) )
					{
						if( m_helper == helper )
						{
							m_helper.reset( false );
							
							apply_default_helper();
							
							update();
						}
					}
				}
				
				if( m_behaviour != null )
				{
					m_behaviour.key_up_event( sender, e );
				}
			}
		}

		public void key_down_event( object sender, KeyEventArgs e )
		{
			if( m_pix_box.Visible )
			{
				foreach( var helper in m_helper_arr )
				{
					if( helper.check_key_code( e ) )
					{
						if( m_helper != helper )
						{
							m_helper = helper;
							
							m_helper.reset( true );
							
							update();
						}
					}
				}
				
				if( m_behaviour != null )
				{
					m_behaviour.key_down_event( sender, e );
				}
			}
		}
		
		private bool apply_default_helper()
		{
			layout_editor_helper_base def_helper = get_default_helper();
			
			if( def_helper != null )
			{
				m_helper = def_helper;
				
				m_helper.reset( true );
				
				return true;
			}
			else
			{
				m_helper = null;
			}
			
			return false;
		}
		
		private layout_editor_helper_base get_default_helper()
		{
			if( m_behaviour != null )
			{
				layout_editor_base.EHelper helper = m_behaviour.default_helper();
				
				if( helper != EHelper.eh_Unknown )
				{
					return m_helper_arr[ ( int )helper ];
				}
			}
			
			return null;
		}
	}
}
