/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 09.05.2017
 * Time: 17:34
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of map_editor.
	/// </summary>
	///

	public class TilesPatternEventArg : EventArgs
	{
		private readonly pattern_data m_data 	= null;
		private readonly int m_CHR_bank_id		= -1;
		private readonly int m_pos_x			= -1;
		private readonly int m_pos_y			= -1;
		
		public pattern_data data
		{
			get { return m_data; }
			set {}
		}
		
		public int CHR_bank_id
		{
			get { return m_CHR_bank_id; }
			set {}
		}
		
		public int pos_x
		{
			get { return m_pos_x; }
			set {}
		}

		public int pos_y
		{
			get { return m_pos_y; }
			set {}
		}
		
		public TilesPatternEventArg( pattern_data _data, int _CHR_bank_id, int _pos_x, int _pos_y )
		{
			m_data 			= _data;
			m_CHR_bank_id 	= _CHR_bank_id;
			m_pos_x 		= _pos_x;
			m_pos_y 		= _pos_y;
		}
	}

	public class NewTileEventArg : EventArgs
	{
		private readonly tiles_data m_data 	= null;
		private readonly int m_tile_ind 	= -1;
		
		public int tile_ind
		{
			get { return m_tile_ind; }
			set {}
		}
		
		public tiles_data data
		{
			get { return m_data; }
			set {}
		}
		
		public NewTileEventArg( int _tile_ind, tiles_data _data )
		{
			m_tile_ind = _tile_ind; 
			m_data = _data;
		}
	}
	
	public class screen_editor : drawable_base
	{
		public event EventHandler ModeChanged;
		public event EventHandler NeedScreensUpdate;
		public event EventHandler RequestUpScreen;
		public event EventHandler RequestDownScreen;
		public event EventHandler RequestLeftScreen;
		public event EventHandler RequestRightScreen;
		public event EventHandler UpdateTileImage;
		public event EventHandler CreatePatternEnd;
		public event EventHandler PutTilesPattern;
		public event EventHandler CancelPatternPlacing;
		
		public enum EMode
		{
			em_Single,
			em_Layout,
		};
		
		private EMode 	m_mode	= EMode.em_Single;
		
		public EMode mode
		{
			get { return m_mode; }
			set 
			{ 
				m_mode = value;
				
				m_layout_mode_adj_scr	= null;
				m_scr_ind				= -1;

				if( ModeChanged != null )
				{
					ModeChanged( this, null );
				}
				
				update();
			}
		}
		
		public enum EFillMode
		{
			efm_Unknown,
			efm_Tile,
			efm_Block,
		};
		
		private EFillMode	m_fill_mode	= EFillMode.efm_Unknown;
		
		private Point		m_sel_rect_beg	= new Point();
		private Point		m_sel_rect_end	= new Point();
		
		private Rectangle m_rect_tile;
		
		private readonly Bitmap m_sel_area_tile	= null;
		
		public enum EState
		{
			es_Build,
			es_CreatePattern,
			es_PlacePattern,
		};
		
		private EState	m_state	= EState.es_Build;

		private pattern_data	m_active_pattern	= null;		

		private int 		m_curr_CHR_bank_id	= -1;
		
		private short[]		m_layout_mode_adj_scr	= null;
		
		private bool 		m_draw_grid			= true;
		
		private bool 		m_pbox_captured		= false; 
		
		private tiles_data 	m_tiles_data		= null;
		private int			m_scr_ind			= -1;
		
		private int			m_active_tile_id	= -1;
		
		private Image 			m_tile_img				= null;
		private int				m_tile_x				= -1;
		private int				m_tile_y				= -1;
		private Rectangle		m_tile_ghost_img_rect;
		private Rectangle		m_border_tile_img_rect;
		private readonly 		ImageAttributes m_tile_img_attr	= null;
		
		private readonly ImageList m_tiles_imagelist	= null;
		private readonly ImageList m_blocks_imagelist	= null;
		
		private readonly List< int >	m_block_tiles_cache			= null;
		private int						m_last_empty_tile_ind	= -1;

		private readonly PictureBox m_pbox_active_tile		= null;
		private readonly GroupBox	m_grp_box_active_tile	= null;	
		
		private readonly Pen 		m_pbox_active_tile_pen	= null;
		private readonly Graphics 	m_pbox_active_tile_gfx	= null;
		
		public bool draw_grid_flag
		{
			get { return m_draw_grid; }
			set { m_draw_grid = value; update(); }
		}
		
		private data_sets_manager.EScreenDataType	m_screen_data_type = data_sets_manager.EScreenDataType.sdt_Tiles4x4;
		
		public screen_editor( PictureBox _pbox, ImageList _tiles_imagelist, ImageList _blocks_imagelist, PictureBox _active_tile_pbox, GroupBox _active_tile_grp_box ) : base( _pbox )
		{
			m_tiles_imagelist	= _tiles_imagelist;
			m_blocks_imagelist	= _blocks_imagelist;
			
			_pbox.MouseDown += new MouseEventHandler( ScreenEditor_MouseDown );
			_pbox.MouseUp 	+= new MouseEventHandler( ScreenEditor_MouseUp );
			_pbox.MouseMove += new MouseEventHandler( ScreenEditor_MouseMove );
			_pbox.MouseLeave += new EventHandler( ScreenEditor_MouseLeave );
			
			float[][] pts_arr = {	new float[] {1, 0, 0, 0, 0},
									new float[] {0, 1, 0, 0, 0},
									new float[] {0, 0, 1, 0, 0},
									new float[] {0, 0, 0, 0.5f, 0}, 
									new float[] {0, 0, 0, 0, 1} };
			
			ColorMatrix clr_mtx = new ColorMatrix( pts_arr );
			m_tile_img_attr		= new ImageAttributes();
			m_tile_img_attr.SetColorMatrix( clr_mtx, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );
			
			m_tile_ghost_img_rect	= new Rectangle( 0, 0, utils.CONST_SCREEN_TILES_SIZE, utils.CONST_SCREEN_TILES_SIZE );
			m_border_tile_img_rect	= new Rectangle( 0, 0, utils.CONST_SCREEN_TILES_SIZE, utils.CONST_SCREEN_TILES_SIZE );
			
			m_block_tiles_cache = new List< int >( platform_data.get_max_tiles_cnt() );
			
			m_sel_area_tile = new Bitmap( utils.CONST_SCREEN_TILES_SIZE, utils.CONST_SCREEN_TILES_SIZE, PixelFormat.Format24bppRgb );
			Graphics.FromImage( m_sel_area_tile ).Clear( utils.CONST_COLOR_SCREEN_SELECTION_TILE );
			
			// active tile data
			{
				m_pbox_active_tile		= _active_tile_pbox;
				m_grp_box_active_tile	= _active_tile_grp_box;
				
				// prepare 'Active Tile' for drawing
				Bitmap bmp = new Bitmap( m_pbox_active_tile.Width, m_pbox_active_tile.Height );
				m_pbox_active_tile.Image = bmp;
				
				m_pbox_active_tile_gfx = Graphics.FromImage( bmp );
				
				m_pbox_active_tile_gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
				m_pbox_active_tile_gfx.PixelOffsetMode 		= PixelOffsetMode.HighQuality;

				m_pbox_active_tile_pen = new Pen( Color.White );
				m_pbox_active_tile_pen.EndCap 	= LineCap.NoAnchor;
				m_pbox_active_tile_pen.StartCap	= LineCap.NoAnchor;			
				
				clear_active_tile_img();
			}
			
			m_rect_tile	= new Rectangle();
			
			update();
		}

		public void subscribe_event( layout_editor _layout_editor )
		{
			_layout_editor.ScreenSelected += new EventHandler( screen_selected );
		}
		
		private void screen_selected( object sender, EventArgs e )
		{
			if( mode == EMode.em_Layout )
			{
				int 		CHR_bank_ind;
				tiles_data 	data;
				
				m_layout_mode_adj_scr	= null;
				
				( sender as layout_editor ).get_sel_screen_data( out CHR_bank_ind, out m_scr_ind, out data, ref m_layout_mode_adj_scr );
				
				if( CHR_bank_ind != m_curr_CHR_bank_id )
				{
					m_scr_ind = -1;
				}
				
				update();
			}
		}

		public void subscribe_event( patterns_manager_form _patterns_mngr )
		{
			_patterns_mngr.CreatePatternBegin				+= new EventHandler( create_pattern_begin );
			_patterns_mngr.ScreenEditorSwitchToBuildMode	+= new EventHandler( switch_to_build_mode );
			_patterns_mngr.EnablePlacePatternMode			+= new EventHandler( enable_place_pattern_mode );
		}
		
		private void enable_place_pattern_mode( object sender, EventArgs e )
		{
			PatternEventArg args = e as PatternEventArg;
			
			m_active_pattern = args.data;
			
			if( m_active_pattern == null )
			{
				MainForm.message_box( "Invalid pattern data!", "Screen Editor", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return;
			}

			m_tile_ghost_img_rect.Height = m_tile_ghost_img_rect.Width = platform_data.get_screen_tiles_size_uni( m_screen_data_type );
			
			clear_active_tile_img();
			
			m_state = EState.es_PlacePattern;
			
			m_pix_box.Cursor = Cursors.Arrow;
			
			m_tile_x = int.MinValue;
			m_tile_y = int.MinValue;
			
			m_fill_mode = platform_data.get_screen_fill_mode_uni( m_screen_data_type );
			
			update();
		}
		
		private void create_pattern_begin( object sender, EventArgs e )
		{
			m_tile_ghost_img_rect.Height = m_tile_ghost_img_rect.Width = platform_data.get_screen_tiles_size_uni( m_screen_data_type );
			
			clear_active_tile_img();
			
			m_state = EState.es_CreatePattern;
			
			m_pix_box.Cursor = Cursors.Cross;
			
			m_pbox_captured = false;
		
			update();
		}

		private void switch_to_build_mode( object sender, EventArgs e )
		{
			m_state = EState.es_Build;
			
			m_pix_box.Cursor = Cursors.Arrow;
			
			m_pbox_captured = false;
			
			m_active_pattern = null;

			update();
		}

		private void cancel_pattern_placing()
		{
			if( CancelPatternPlacing != null )
			{
				CancelPatternPlacing( this, null );
			}
		}
		
		private void calc_pattern_params(	ref int _tile_pos_x, 
											ref int _tile_pos_y, 
											ref int _tiles_width, 
											ref int _tiles_height, 
											ref int _min_x, 
											ref int _min_y, 
											ref int _dx, 
											ref int _dy )
		{
			int scr_tiles_size = platform_data.get_screen_tiles_size_uni( m_screen_data_type );
			
			_min_x = Math.Max( get_scr_offs_x(), Math.Min( m_sel_rect_beg.X, m_sel_rect_end.X ) );
			_min_y = Math.Max( get_scr_offs_y(), Math.Min( m_sel_rect_beg.Y, m_sel_rect_end.Y ) );

			int max_x = Math.Min( get_scr_offs_x() + ( platform_data.get_screen_width_pixels() << 1 ), Math.Max( m_sel_rect_beg.X, m_sel_rect_end.X ) );
			int max_y = Math.Min( get_scr_offs_y() + ( platform_data.get_screen_height_pixels() << 1 ), Math.Max( m_sel_rect_beg.Y, m_sel_rect_end.Y ) );

			_dx = max_x - _min_x;
			_dy = max_y - _min_y;
			
			_tile_pos_x = ( ( _min_x - get_scr_offs_x() ) / scr_tiles_size );
			_tile_pos_y = ( ( _min_y - get_scr_offs_y() ) / scr_tiles_size );
			
			_tiles_width	= ( ( max_x - get_scr_offs_x() ) / scr_tiles_size ) - _tile_pos_x + 1;
			_tiles_height	= ( ( max_y - get_scr_offs_y() ) / scr_tiles_size ) - _tile_pos_y + 1;
			
			_tiles_width	= Math.Min( platform_data.get_screen_tiles_width_uni( m_screen_data_type ) - _tile_pos_x, _tiles_width );
			_tiles_height	= Math.Min( platform_data.get_screen_tiles_height_uni( m_screen_data_type ) - _tile_pos_y, _tiles_height );
		}
		
		public void subscribe_event( data_sets_manager _data_mngr )
		{
			_data_mngr.SetScreenData += new EventHandler( new_data_set );
		}
		
		private void new_data_set( object sender, EventArgs e )
		{
			data_sets_manager data_mngr = sender as data_sets_manager;
			
			if( data_mngr.tiles_data_pos >= 0 )
			{
				m_tiles_data	= data_mngr.get_tiles_data( data_mngr.tiles_data_pos );
				m_scr_ind 		= data_mngr.scr_data_pos;

				update();
			}
			else
			{
				m_tiles_data 	= null;
				m_scr_ind		= -1;
			}
			
			m_curr_CHR_bank_id	= data_mngr.tiles_data_pos;
			
			m_pbox_captured 	= false;	// just in case
		}
		
		private void ScreenEditor_MouseLeave(object sender, System.EventArgs e)
		{
			if( m_state == EState.es_Build )
			{
				if( m_tile_img != null && m_tile_x >= 0 && m_tile_y >= 0 )
				{
					m_tile_x = -1;
					m_tile_y = -1;
					
					update();
				}
			}
		}
		
		private void ScreenEditor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_state == EState.es_Build )
			{
				if( ( m_tiles_data != null && m_scr_ind >= 0 && m_active_tile_id >= 0 ) && e.Button == MouseButtons.Left )
				{
					m_pbox_captured = true;
	
					if( put_tile( e.X, e.Y ) )
					{				
						m_tile_x = -1;
						m_tile_y = -1;
					}
				}
			}
			else
			if( m_state == EState.es_CreatePattern )
			{
				if( m_scr_ind >= 0 )
				{
					if( m_pbox_captured != true )
					{
						if( inside_screen( e.X, e.Y ) == true )
						{
							m_pbox_captured = true;
							
							m_sel_rect_end.X = m_sel_rect_beg.X = e.X;
							m_sel_rect_end.Y = m_sel_rect_beg.Y = e.Y;
						}
					}
				}
				else
				{
					MainForm.message_box( "Please, select a screen!", "Tiles Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			}
		}
		
		private void ScreenEditor_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_state == EState.es_Build )
			{
				m_pbox_captured = false;
			}
			
			if( m_state == EState.es_CreatePattern )
			{
				if( m_pbox_captured == true )
				{
					if( CreatePatternEnd != null )
					{
						int tile_pos_x		= 0;
						int tile_pos_y		= 0;
						int tiles_width		= 0;
						int tiles_height	= 0;
						
						int min_x	= 0;
						int min_y	= 0;
						int dx		= 0;
						int dy		= 0;
						
						calc_pattern_params( ref tile_pos_x, ref tile_pos_y, ref tiles_width, ref tiles_height, ref min_x, ref min_y, ref dx, ref dy );
						
						// extract tiles
						int tile_x;
						int tile_y;
						
						int pos = -1;
						
						int num_width_tiles = platform_data.get_screen_tiles_width_uni( m_screen_data_type );
						
						screen_data pttrn_data = new screen_data( tiles_width, tiles_height );
						
						for( tile_y = 0; tile_y < tiles_height; tile_y++ )
						{
							for( tile_x = 0; tile_x < tiles_width; tile_x++ )
							{
								pttrn_data.set_tile( ++pos, m_tiles_data.get_screen_tile( m_scr_ind, ( ( tile_pos_y + tile_y ) * num_width_tiles ) + tile_pos_x + tile_x ) );
							}
						}
						
						// send PATTERNS's data
						CreatePatternEnd( this, new PatternEventArg( new pattern_data( null, pttrn_data ) ) );
						
						return;
					}
				}
			}
			else
			if( m_state == EState.es_PlacePattern )
			{
				if( e.Button == MouseButtons.Right )
				{
					cancel_pattern_placing();
				}
				else
				if( m_tile_x != int.MinValue && m_tile_y != int.MinValue )
				{
					if( m_scr_ind >= 0 )
					{
						if( m_mode == EMode.em_Single )
						{
							int num_width_tiles = platform_data.get_screen_tiles_width_uni( m_screen_data_type );
							
							int y_pos = 0;
							
							if( m_tile_y < 0 )
							{
								y_pos = -m_tile_y;
								m_tile_y = 0;
							}
							
							for( int tile_y = y_pos; tile_y < m_active_pattern.height; tile_y++ )
							{
								for( int tile_x = 0; tile_x < m_active_pattern.width; tile_x++ )
								{
									m_tiles_data.set_screen_tile( m_scr_ind, ( ( m_tile_y + tile_y-y_pos ) * num_width_tiles ) + m_tile_x + tile_x, m_active_pattern.data.get_tile( tile_y * m_active_pattern.width + tile_x ) );
								}
							}
						}
						else
						{
							if( PutTilesPattern != null )
							{
								PutTilesPattern( this, new TilesPatternEventArg( m_active_pattern, m_curr_CHR_bank_id, m_tile_x, m_tile_y ) );
							}
						}
						
						update();
						
						if( NeedScreensUpdate != null )
						{
							NeedScreensUpdate( this, null );
						}
						
						MainForm.set_status_msg( "The pattern <" + m_active_pattern.name + "> added at pos: " + m_tile_x + "; " + m_tile_y );
					}
					else
					{
						MainForm.message_box( "Please, select a screen!", "Pattern Placing Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					}
				}
			}
			
			if( mode == EMode.em_Layout )
			{
				if( e.Y < get_scr_offs_y() )
				{
					if( RequestUpScreen != null )
					{
						RequestUpScreen( this, null );
					}
				}
				else
				if( e.Y > get_scr_offs_y() + ( platform_data.get_screen_blocks_height() * utils.CONST_SCREEN_BLOCKS_SIZE ) )
				{
					if( RequestDownScreen != null )
					{
						RequestDownScreen( this, null );
					}
				}
				else
				if( e.X < get_scr_offs_x() )
				{
					if( RequestLeftScreen != null )
					{
						RequestLeftScreen( this, null );
					}
				}
				else
				if( e.X > get_scr_offs_x() + ( platform_data.get_screen_blocks_width() * utils.CONST_SCREEN_BLOCKS_SIZE ) )
				{
					if( RequestRightScreen != null )
					{
						RequestRightScreen( this, null );
					}
				}
			}
		}
		
		private void ScreenEditor_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( m_state == EState.es_Build )
			{
				if( m_pbox_captured )
				{
					if( put_tile( e.X, e.Y ) )
					{
						m_tile_x = -1;
						m_tile_y = -1;
					}
				}
				else
				{
					int new_tile_x;
					int new_tile_y;
					
					if( get_tile_xy( e.X, e.Y, out new_tile_x, out new_tile_y ) == true )
					{
						m_pix_box.Cursor = Cursors.Arrow;
					}
					else
					{
						m_pix_box.Cursor = ( mode == EMode.em_Layout ) ? Cursors.Hand:Cursors.Arrow;
					}
	
					if( m_tile_img != null )
					{
						if( new_tile_x != m_tile_x || new_tile_y != m_tile_y && ( new_tile_x >= 0 && new_tile_y >= 0 ) )
						{
							m_tile_x = new_tile_x;
							m_tile_y = new_tile_y;
						
							update();
							
							if( m_tile_x >= 0 && m_tile_y >= 0 )
							{
								MainForm.set_status_msg( String.Format( "Screen Editor: " + get_fill_mode_str() + ": #{0:X2} \\ Pos: {1};{2}", m_active_tile_id, m_tile_x, m_tile_y ) );
							}
							else
							{
								MainForm.set_status_msg( String.Format( "Screen Editor: " + get_fill_mode_str() + ": #{0:X2}", m_active_tile_id ) );
							}
						}
					}
				}
			}
			else
			if( m_state == EState.es_CreatePattern )
			{
				if( m_pbox_captured == true )
				{
					m_sel_rect_end = e.Location;
					
					update();
				}
				
				if( inside_screen( e.X, e.Y ) == true )
				{
					m_pix_box.Cursor = Cursors.Cross;
				}
				else
				{
					m_pix_box.Cursor = ( mode == EMode.em_Layout ) ? Cursors.Hand:Cursors.Cross;
				}
			}
			if( m_state == EState.es_PlacePattern )
			{
				if( get_tile_xy( e.X, e.Y, out m_tile_x, out m_tile_y ) == false )
				{
					m_tile_x = int.MinValue;
					m_tile_y = int.MinValue;
				}
				else
				{
					int half_width	= m_active_pattern.width >> 1;
					int half_height	= m_active_pattern.height >> 1;
					
					if( m_mode == EMode.em_Single )
					{
						m_tile_x = m_tile_x - half_width < 0 ? 0:m_tile_x - half_width;
						m_tile_y = m_tile_y - half_height < 0 ? 0:m_tile_y - half_height;

						int num_width_tiles		= platform_data.get_screen_tiles_width_uni( m_screen_data_type );
						int num_height_tiles	= platform_data.get_screen_tiles_height_uni( m_screen_data_type );

						m_tile_x = m_tile_x + m_active_pattern.width > num_width_tiles - 1 ? num_width_tiles - m_active_pattern.width:m_tile_x;
						m_tile_y = m_tile_y + m_active_pattern.height > num_height_tiles - 1 ? num_height_tiles - m_active_pattern.height:m_tile_y;
					}
					else
					{
						m_tile_x -= half_width;
						m_tile_y -= half_height;
					}
				}
				
				update();
			}
		}

		public void update_adjacent_screens()
		{
			if( mode == EMode.em_Layout )
			{
				update();
			}
		}
		
		private string get_fill_mode_str()
		{
			return ( m_fill_mode == EFillMode.efm_Tile ) ? "Tile":( ( m_fill_mode == EFillMode.efm_Block ) ? "Block":"???" );
		}
		
		private bool inside_screen( int _x, int _y )
		{
			int tile_x;
			int tile_y;
			
			return get_tile_xy( _x, _y, out tile_x, out tile_y );
		}
		
		private bool get_tile_xy( int _x, int _y, out int _tile_x, out int _tile_y )
		{
			_tile_x = -1;
			_tile_y = -1;
			
			int offs_x = _x - get_scr_offs_x();
			int offs_y = _y - get_scr_offs_y();
			
			if( offs_x < 0 || offs_y < 0 )
			{
				return false;
			}
			
			int tile_size 			= ( m_fill_mode == EFillMode.efm_Tile ) ? utils.CONST_SCREEN_TILES_SIZE:utils.CONST_SCREEN_BLOCKS_SIZE;
			
			int scr_width_tiles		= ( m_fill_mode == EFillMode.efm_Tile ) ? platform_data.get_screen_tiles_width():platform_data.get_screen_blocks_width();
			int scr_height_tiles 	= ( m_fill_mode == EFillMode.efm_Tile ) ? platform_data.get_screen_tiles_height():platform_data.get_screen_blocks_height(); 
			
			int x = offs_x / tile_size;
			int y = offs_y / tile_size;
			
			if( x >= scr_width_tiles || y >= scr_height_tiles )
			{
				return false;
			}
			
			_tile_x = x;
			_tile_y = y;
			
			return true;
		}
		
		private bool put_tile( int _x, int _y )
		{
			if( get_tile_xy( _x, _y, out m_tile_x, out m_tile_y ) == false )
			{
				return false;
			}
		
			Region old_region = m_gfx.Clip;
			
			if( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				int tile_id = m_active_tile_id;
				
				if( m_fill_mode == EFillMode.efm_Block )
				{
					ulong new_tile = build_new_tile( m_tile_x, m_tile_y );
					
					int tile_ind = check_tile( new_tile );
					
					if( tile_ind >= 0 )
					{
						tile_id = tile_ind;
					}
					else
					{
						tile_id = save_new_tile( new_tile );
						
						if( tile_id < 0 )
						{
							return false;
						}
					}
					
					m_tile_y >>= 1;
					m_tile_x >>= 1;
				}
				
				m_tiles_data.set_screen_tile( m_scr_ind, ( m_tile_y * platform_data.get_screen_tiles_width() ) + m_tile_x, ( ushort )tile_id );
				
				draw_tile( m_tile_x, m_tile_y, tile_id );

				m_rect_tile.X = get_scr_offs_x() + ( m_tile_x * utils.CONST_SCREEN_TILES_SIZE );
				m_rect_tile.Y = get_scr_offs_y() + ( m_tile_y * utils.CONST_SCREEN_TILES_SIZE );
				m_rect_tile.Width = m_rect_tile.Height = utils.CONST_SCREEN_TILES_SIZE;
				
				m_gfx.Clip = new Region( m_rect_tile );
			}
			else
			{
				m_tiles_data.set_screen_tile( m_scr_ind, ( m_tile_y * platform_data.get_screen_blocks_width() ) + m_tile_x, ( ushort )m_active_tile_id );
				
				draw_block( m_tile_x, m_tile_y, m_active_tile_id );
				
				m_rect_tile.X = get_scr_offs_x() + ( m_tile_x * utils.CONST_SCREEN_BLOCKS_SIZE );
				m_rect_tile.Y = get_scr_offs_y() + ( m_tile_y * utils.CONST_SCREEN_BLOCKS_SIZE );
				m_rect_tile.Width = m_rect_tile.Height = utils.CONST_SCREEN_BLOCKS_SIZE;
				
				m_gfx.Clip = new Region( m_rect_tile );
			}
			
			draw_grid_and_border();
			invalidate();
			
			m_gfx.Clip = old_region;

			if( NeedScreensUpdate != null )
			{
				NeedScreensUpdate( this, null );
			}
			
			return true;
		}
		
		private int save_new_tile( ulong _tile )
		{
			if( m_last_empty_tile_ind >= 0 )
			{
				for( int i = m_last_empty_tile_ind; i < platform_data.get_max_tiles_cnt(); i++ )
				{
					if( m_tiles_data.tiles[ i ] == 0 )
					{
						m_tiles_data.tiles[ i ] = _tile;
						
						// add the tile to the cache
						m_block_tiles_cache.Add( i );
						
						// redraw tile image here...
						if( UpdateTileImage != null )
						{
							UpdateTileImage( this, new NewTileEventArg( i, m_tiles_data ) );
						}
						
						m_last_empty_tile_ind = i;
						
						return i;
					}
				}
			}
			
			m_pbox_captured = false;
			
			m_last_empty_tile_ind = -1;
				
			// TILES ARRAY OVERFLOW !!!
			MainForm.message_box( "Try to optimize the tiles data!", "Tiles Array Overflow", System.Windows.Forms.MessageBoxButtons.OK );
			
			return -1;
		}
		
		private int check_tile( ulong _tile_data )
		{
			int tile_ind;
			
			int size = m_block_tiles_cache.Count;
			
			for( int i = 0; i < size; i++ )
			{
				tile_ind = m_block_tiles_cache[ i ];
				
				if( m_tiles_data.tiles[ tile_ind ] == _tile_data )
				{
					return tile_ind;
				}
			}
			
			return -1;
		}
		
		private ulong build_new_tile( int _tile_x, int _tile_y )
		{
			ulong old_tile = m_tiles_data.tiles[ m_tiles_data.get_screen_tile( m_scr_ind, ( ( _tile_y >> 1 ) * platform_data.get_screen_tiles_width() ) + ( _tile_x >> 1 ) ) ];
			
			int block_ind = ( _tile_x & 0x01 ) + ( ( _tile_y & 0x01 ) << 1 );
			
			return utils.set_ushort_to_ulong( old_tile, block_ind, ( ushort )m_active_tile_id );
		}
		
		public void set_active_tile( int _tile_ind, EFillMode _fill_mode )
		{
			if( m_state == EState.es_Build )
			{
				if( m_tiles_data != null )
				{
					bool need_update = m_fill_mode != _fill_mode;
					
					m_fill_mode = _fill_mode;
					
					m_block_tiles_cache.Clear();

					Image img = null;
					
					switch( _fill_mode )
					{
						case EFillMode.efm_Tile:
							{
								img = m_tiles_imagelist.Images[ _tile_ind ];
								
								m_tile_ghost_img_rect.Width = utils.CONST_SCREEN_TILES_SIZE;
								m_tile_ghost_img_rect.Height = utils.CONST_SCREEN_TILES_SIZE;
								
								m_pbox_active_tile_gfx.DrawImage( img, 0, 0, img.Width, img.Height );
								m_pbox_active_tile.Invalidate();
								
								m_grp_box_active_tile.Text = "Tile: " + String.Format( "${0:X2}", _tile_ind );
							}
							break;
							
						case EFillMode.efm_Block:
							{
								img = m_blocks_imagelist.Images[ _tile_ind ];
								
								m_tile_ghost_img_rect.Width = utils.CONST_SCREEN_BLOCKS_SIZE;
								m_tile_ghost_img_rect.Height = utils.CONST_SCREEN_BLOCKS_SIZE;
								
								if( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
								{
									calc_common_blocks( ( byte )_tile_ind );
								}
								
								m_pbox_active_tile_gfx.DrawImage( img, 0, 0, m_pbox_active_tile.Width, m_pbox_active_tile.Height );
								m_pbox_active_tile.Invalidate();
								
								m_grp_box_active_tile.Text = "Block: " + String.Format( "${0:X2}", _tile_ind );
							}
							break;
					}

					m_active_tile_id 	= _tile_ind;
					m_tile_img			= img;
					
					if( m_tile_img == null )
					{
						m_tile_x = -1;
						m_tile_y = -1;
					}
		
					// draw image into 'Active Tile'
					if( img == null )
					{
						m_pbox_active_tile_gfx.Clear( utils.CONST_COLOR_ACTIVE_TILE_BACKGROUND );
						
						// red cross
						{
							// draw the red cross as a sign of inactive state
							m_pbox_active_tile_pen.Color = utils.CONST_COLOR_PIXBOX_INACTIVE_CROSS;
							
							m_pbox_active_tile_gfx.DrawLine( m_pbox_active_tile_pen, 0, 0, utils.CONST_TILES_IMG_SIZE, utils.CONST_TILES_IMG_SIZE );
							m_pbox_active_tile_gfx.DrawLine( m_pbox_active_tile_pen, utils.CONST_TILES_IMG_SIZE, 0, 0, utils.CONST_TILES_IMG_SIZE );
						}
						
						m_pbox_active_tile.Invalidate();
						
						m_grp_box_active_tile.Text = "...";
					}
					
					if( need_update )
					{
						update();
					}
				}
			}
		}
		
		private void calc_common_blocks( ushort _block_ind )
		{
			int i;
			int j;	

			ulong tile_data;
			
			m_last_empty_tile_ind = m_tiles_data.get_first_free_tile_id( false );
			m_last_empty_tile_ind = ( m_last_empty_tile_ind == 0 ) ? 1:m_last_empty_tile_ind;	// skip zero tile as reserved for an empty space
			
			m_block_tiles_cache.Clear();
			
			for( i = 0; i < platform_data.get_max_tiles_cnt(); i++ )
			{
				tile_data = m_tiles_data.tiles[ i ];

				for( j = 0; j < utils.CONST_TILE_SIZE; j++ )
				{
					if( utils.get_ushort_from_ulong( tile_data, j ) == _block_ind )
					{
						m_block_tiles_cache.Add( i );
			
						break;
					}
				}
			}
		}
		
		public override void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_tiles_data != null )
			{
				int i;
				screen_data scr;
				
				int half_tile_x = platform_data.get_half_tile_x();
				int half_tile_y = platform_data.get_half_tile_y();
			
				if( mode == EMode.em_Layout )
				{
					if( m_layout_mode_adj_scr == null )
					{
						disable( true );
						
						utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
						m_gfx.DrawString( "[ Select an active layout and click on the layout's screen to edit it ]", utils.fnt10_Arial, utils.brush, 0, 0 );
						
						invalidate();
						
						return;
					}
					
					const short zero_scr = unchecked( (short)0xffff );
					
					short	tile_data;
					
					int scr_width	= platform_data.get_screen_blocks_width() * utils.CONST_SCREEN_BLOCKS_SIZE;
					int scr_height	= platform_data.get_screen_blocks_height() * utils.CONST_SCREEN_BLOCKS_SIZE;
					
					for( int scr_n = 0; scr_n < 8; scr_n++ )
					{
						tile_data = m_layout_mode_adj_scr[ scr_n ];
							
						if( tile_data != zero_scr )
						{
							if( ( ( tile_data & 0xff00 ) >> 8 ) == m_curr_CHR_bank_id )
							{
								scr = m_tiles_data.get_screen_data( tile_data & 0x00ff );
								
								if( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
								{
									for( i = 0; i < platform_data.get_screen_tiles_cnt(); i++ )
									{
										draw_tile( 	i % platform_data.get_screen_tiles_width(),
										          	i / platform_data.get_screen_tiles_width(),
										          	scr.get_tile( i ),
										          	scr_width * layout_data.adj_scr_slots[ ( scr_n << 1 ) ],
										          	scr_height * layout_data.adj_scr_slots[ ( scr_n << 1 ) + 1 ] );
									}
								}
								else
								{
									for( i = 0; i < platform_data.get_screen_blocks_cnt(); i++ )
									{
										draw_block(	i % platform_data.get_screen_blocks_width(),
										          	i / platform_data.get_screen_blocks_width(),
										          	scr.get_tile( i ),
										          	scr_width * layout_data.adj_scr_slots[ ( scr_n << 1 ) ],
										          	scr_height * layout_data.adj_scr_slots[ ( scr_n << 1 ) + 1 ] );
									}
								}
							}
						}
					}
					
					// draw transparent fullscreen quad
					utils.brush.Color = utils.CONST_COLOR_SCREEN_TRANSLUCENT_QUAD;
					m_gfx.FillRectangle( utils.brush, 0, 0, m_pix_box.Width, m_pix_box.Height );
				}
				
				// draw a tiled screen
				if( m_scr_ind >= 0 )
				{
					scr = m_tiles_data.get_screen_data( m_scr_ind );
					
					if( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
					{
						for( i = 0; i < platform_data.get_screen_tiles_cnt(); i++ )
						{
							draw_tile( i % platform_data.get_screen_tiles_width(), i / platform_data.get_screen_tiles_width(), scr.get_tile( i ) );
						}
					}
					else
					{
						for( i = 0; i < platform_data.get_screen_blocks_cnt(); i++ )
						{
							draw_block( i % platform_data.get_screen_blocks_width(), i / platform_data.get_screen_blocks_width(), scr.get_tile( i ) );
						}
					}
					
					if( m_state == EState.es_Build )
					{
						// draw a ghost image
						if( m_tile_img != null && m_tile_x >= 0 && m_tile_y >= 0 )
						{
							switch( m_fill_mode )
							{
								case EFillMode.efm_Tile:
									{
										m_tile_ghost_img_rect.X = get_scr_offs_x() + ( m_tile_x * utils.CONST_SCREEN_TILES_SIZE );
										m_tile_ghost_img_rect.Y = get_scr_offs_y() + ( m_tile_y * utils.CONST_SCREEN_TILES_SIZE );
										
										m_gfx.DrawImage( m_tile_img, m_tile_ghost_img_rect, 0, 0, utils.CONST_SCREEN_TILES_SIZE, utils.CONST_SCREEN_TILES_SIZE, GraphicsUnit.Pixel, m_tile_img_attr );
									}
									break;
	
								case EFillMode.efm_Block:
									{
										m_tile_ghost_img_rect.X	= get_scr_offs_x() + ( m_tile_x * utils.CONST_SCREEN_BLOCKS_SIZE );
										m_tile_ghost_img_rect.Y	= get_scr_offs_y() + ( m_tile_y * utils.CONST_SCREEN_BLOCKS_SIZE );
										
										m_gfx.DrawImage( m_tile_img, m_tile_ghost_img_rect, 0, 0, utils.CONST_SCREEN_BLOCKS_SIZE, utils.CONST_SCREEN_BLOCKS_SIZE, GraphicsUnit.Pixel, m_tile_img_attr );
									}
									break;
							}
						}
					}
					else
					if( m_state == EState.es_CreatePattern )
					{
						// draw selection rectangle
						if( m_pbox_captured == true )
						{
							m_pen.Color		= utils.CONST_COLOR_SCREEN_SELECTION_RECTANGLE;
							m_pen.Width		= 1.0f;
							m_pen.DashStyle	= System.Drawing.Drawing2D.DashStyle.Dot;

							int tile_pos_x		= 0;
							int tile_pos_y		= 0;
							int tiles_width		= 0;
							int tiles_height	= 0;
							
							int min_x	= 0;
							int min_y	= 0;
							int dx		= 0;
							int dy		= 0;
							
							int tile_size = platform_data.get_screen_tiles_size_uni( m_screen_data_type );
							
							calc_pattern_params( ref tile_pos_x, ref tile_pos_y, ref tiles_width, ref tiles_height, ref min_x, ref min_y, ref dx, ref dy );
							
							for( int tile_y = 0; tile_y < tiles_height; tile_y++ )
							{
								for( int tile_x = 0; tile_x < tiles_width; tile_x++ )
								{
									m_tile_ghost_img_rect.X = ( get_scr_offs_x() + tile_pos_x * tile_size ) + ( tile_x * tile_size );
									m_tile_ghost_img_rect.Y = ( get_scr_offs_y() + tile_pos_y * tile_size ) + ( tile_y * tile_size );

									m_tile_ghost_img_rect.Width		= tile_size;
									m_tile_ghost_img_rect.Height	= tile_size;
									
									if( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
									{
										if( half_tile_x >= 0 )
										{
											m_tile_ghost_img_rect.Width = ( ( tile_pos_x + tile_x ) == half_tile_x ) ? tile_size >> 1:tile_size;
										}
										
										if( half_tile_y >= 0 )
										{
											m_tile_ghost_img_rect.Height = ( ( tile_pos_y + tile_y ) == half_tile_y ) ? tile_size >> 1:tile_size;
										}
									}

									m_gfx.DrawImage( m_sel_area_tile, m_tile_ghost_img_rect, 0, 0, tile_size, tile_size, GraphicsUnit.Pixel, m_tile_img_attr );
								}
							}
							
							m_gfx.DrawRectangle( m_pen, min_x, min_y, dx, dy );
							
							m_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
						}
					}
					else
					if( m_state == EState.es_PlacePattern )
					{
						if( ( m_tile_x != int.MinValue && m_tile_y != int.MinValue ) && ( m_active_pattern != null && m_active_pattern.data != null ) )
						{
							int 		tile_size	= platform_data.get_screen_tiles_size_uni( m_screen_data_type );
							ImageList 	img_list	= ( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ) ? m_tiles_imagelist:m_blocks_imagelist;
							
							for( int tile_y = 0; tile_y < m_active_pattern.height; tile_y++ )
							{
								for( int tile_x = 0; tile_x < m_active_pattern.width; tile_x++ )
								{
									m_tile_ghost_img_rect.X = ( get_scr_offs_x() + m_tile_x * tile_size ) + ( tile_x * tile_size );
									m_tile_ghost_img_rect.Y = ( get_scr_offs_y() + m_tile_y * tile_size ) + ( tile_y * tile_size );

									m_tile_ghost_img_rect.Width		= tile_size;
									m_tile_ghost_img_rect.Height	= tile_size;
									
									if( m_screen_data_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
									{
										if( half_tile_y >= 0 )
										{
											m_tile_ghost_img_rect.Y -= ( ( m_tile_y + tile_y ) > half_tile_y ) ? tile_size >> 1:0;
											m_tile_ghost_img_rect.Height = ( ( m_tile_y + tile_y ) == half_tile_y ) ? tile_size >> 1:tile_size;
										}
										
										if( half_tile_x >= 0 )
										{
											m_tile_ghost_img_rect.X -= ( ( m_tile_x + tile_x ) > half_tile_x ) ? tile_size >> 1:0;
											m_tile_ghost_img_rect.Width = ( ( m_tile_x + tile_x ) == half_tile_x ) ? tile_size >> 1:tile_size;
										}
									}
									
									m_gfx.DrawImage( img_list.Images[ m_active_pattern.data.get_tile( tile_y * m_active_pattern.width + tile_x ) ],
                									 m_tile_ghost_img_rect, 
                									 0, 
                									 0, 
                									 m_tile_ghost_img_rect.Width, 
                									 m_tile_ghost_img_rect.Height, 
                									 GraphicsUnit.Pixel, 
                									 m_tile_img_attr );
								}
							}
						}
					}
				}

				draw_grid_and_border();
			}
			else
			{
				disable( true );
			}
			
			invalidate();
		}
		
		public void clear_active_tile_img()
		{
			set_active_tile( -1, screen_editor.EFillMode.efm_Unknown );
			
			m_last_empty_tile_ind = -1;
		}
		
		public void set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			m_screen_data_type = _type;

			clear_active_tile_img();
			
			m_fill_mode = ( _type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 ) ? screen_editor.EFillMode.efm_Block:m_fill_mode;

			update();
		}
		
		private int get_scr_offs_x()
		{
			return ( m_pix_box.Width - ( platform_data.get_screen_blocks_width() * utils.CONST_SCREEN_BLOCKS_SIZE ) ) >> 1;
		}
		
		private int get_scr_offs_y()
		{
			return ( m_pix_box.Height - ( platform_data.get_screen_blocks_height() * utils.CONST_SCREEN_BLOCKS_SIZE ) ) >> 1;
		}

		private void draw_grid_and_border()
		{
			if( m_draw_grid )
			{
				int i;
				
				int offs_x;
				int offs_y;

				int scr_offs_x = get_scr_offs_x();
				int scr_offs_y = get_scr_offs_y();

				int scr_end_x = scr_offs_x + ( platform_data.get_screen_blocks_width() * utils.CONST_SCREEN_BLOCKS_SIZE );
				int scr_end_y = scr_offs_y + ( platform_data.get_screen_blocks_height() * utils.CONST_SCREEN_BLOCKS_SIZE );
				
				// blocks grid
				{
					m_pen.Color = utils.CONST_COLOR_SCREEN_GRID_THIN;
					
					int start_offs_x = ( scr_offs_x + ( utils.CONST_SCREEN_TILES_SIZE >> 1 ) );
					int start_offs_y = ( scr_offs_y + ( utils.CONST_SCREEN_TILES_SIZE >> 1 ) );
					
					for( i = 0; i < platform_data.get_screen_tiles_width(); i++ )
					{
						offs_x = start_offs_x + ( i * utils.CONST_SCREEN_TILES_SIZE );
						m_gfx.DrawLine( m_pen, offs_x, scr_offs_y, offs_x, scr_end_y );
					}
					
					for( i = 0; i < platform_data.get_screen_tiles_height(); i++ )
					{
						offs_y = start_offs_y + ( i * utils.CONST_SCREEN_TILES_SIZE );
						m_gfx.DrawLine( m_pen, scr_offs_x, offs_y, scr_end_x, offs_y );
					}
				}

				// tiles grid
				{
					m_pen.Color = ( m_fill_mode == EFillMode.efm_Tile ) ? utils.CONST_COLOR_SCREEN_GRID_THICK_TILE_MODE:utils.CONST_COLOR_SCREEN_GRID_THICK_BLOCK_MODE;
					
					for( i = 0; i < platform_data.get_screen_tiles_width(); i++ )
					{
						offs_x = get_scr_offs_x() + ( i * utils.CONST_SCREEN_TILES_SIZE );
						m_gfx.DrawLine( m_pen, offs_x, scr_offs_y, offs_x, scr_end_y );
					}
					
					for( i = 0; i < platform_data.get_screen_tiles_height(); i++ )
					{
						offs_y = get_scr_offs_y() + ( i * utils.CONST_SCREEN_TILES_SIZE );
						m_gfx.DrawLine( m_pen, scr_offs_x, offs_y, scr_end_x, offs_y );
					}
				}
			}

			// red rectangle
			{
				m_pen.Color = Color.Red;
				m_pen.Width = 2.0f;
				
				int width 	= platform_data.get_screen_blocks_width() * utils.CONST_SCREEN_BLOCKS_SIZE;
				int height	= platform_data.get_screen_blocks_height() * utils.CONST_SCREEN_BLOCKS_SIZE;
				
				m_gfx.DrawRectangle( m_pen, get_scr_offs_x(), get_scr_offs_y(), width, height );
				
				m_pen.Width = 1.0f;
			}
			
			disable( false );
		}

		private void draw_tile( int _x, int _y, int _tile_id, int _offs_x = 0, int _offs_y = 0 )
		{
			m_border_tile_img_rect.X		= 0;
			m_border_tile_img_rect.Y		= 0;
			m_border_tile_img_rect.Width	= ( _x == platform_data.get_half_tile_x() ) ? ( utils.CONST_SCREEN_TILES_SIZE >> 1 ):utils.CONST_SCREEN_TILES_SIZE;
			m_border_tile_img_rect.Height	= ( _y == platform_data.get_half_tile_y() ) ? ( utils.CONST_SCREEN_TILES_SIZE >> 1 ):utils.CONST_SCREEN_TILES_SIZE;
			
			m_gfx.DrawImage( 	m_tiles_imagelist.Images[ _tile_id ],
								_offs_x + get_scr_offs_x() + ( _x * utils.CONST_SCREEN_TILES_SIZE ),
								_offs_y + get_scr_offs_y() + ( _y * utils.CONST_SCREEN_TILES_SIZE ),
								m_border_tile_img_rect,
								GraphicsUnit.Pixel );
		}
		
		private void draw_block( int _x, int _y, int _tile_id, int _offs_x = 0, int _offs_y = 0 )
		{
			m_gfx.DrawImageUnscaled( 	m_blocks_imagelist.Images[ _tile_id ], 
										_offs_x + get_scr_offs_x() + ( _x * utils.CONST_SCREEN_BLOCKS_SIZE ),
										_offs_y + get_scr_offs_y() + ( _y * utils.CONST_SCREEN_BLOCKS_SIZE ),
										utils.CONST_SCREEN_BLOCKS_SIZE, 
										utils.CONST_SCREEN_BLOCKS_SIZE );
		}
	}
}