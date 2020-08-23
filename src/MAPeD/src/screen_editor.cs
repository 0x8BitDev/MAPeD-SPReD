/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 09.05.2017
 * Time: 17:34
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of map_editor.
	/// </summary>
	///

	public class NewTileEventArg : EventArgs
	{
		private tiles_data 	m_data 		= null;
		private int 		m_tile_ind 	= -1;
		
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
	
	public delegate void ModeChanged();
	public delegate void NeedScreensUpdate();
	public delegate void RequestUpScreen();
	public delegate void RequestDownScreen();
	public delegate void RequestLeftScreen();
	public delegate void RequestRightScreen();	
	public delegate void UpdateTileImage();
	
	public class screen_editor : drawable_base
	{
		public event EventHandler ModeChanged;
		public event EventHandler NeedScreensUpdate;
		public event EventHandler RequestUpScreen;
		public event EventHandler RequestDownScreen;
		public event EventHandler RequestLeftScreen;
		public event EventHandler RequestRightScreen;
		public event EventHandler UpdateTileImage;
		
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
				
				m_layout_mode_border_tiles	= null;
				m_tiles_data 				= null;
				m_scr_ind					= -1;

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

		private int 		m_curr_CHR_bank_id	= -1;
		
		private short[]		m_layout_mode_border_tiles	= null;
		
		private bool 		m_draw_grid			= true;
		
		private bool 		m_pbox_captured		= false; 
		
		private tiles_data 	m_tiles_data		= null;
		private int			m_scr_ind			= -1;
		
		private int			m_active_tile_id	= -1;
		
		private Image 			m_tile_img				= null;
		private int				m_tile_x				= -1;
		private int				m_tile_y				= -1;
		private ImageAttributes m_tile_img_attr			= null;
		private Rectangle		m_tile_ghost_img_rect;
		private Rectangle		m_border_tile_img_rect;
		
		private ImageList		m_tiles_imagelist		= null;
		
		private List< int >		m_block_tiles			= null;
		private int				m_last_empty_tile_ind	= -1;
		
		public bool draw_grid_flag
		{
			get { return m_draw_grid; }
			set { m_draw_grid = value; update(); }
		}
		
		public screen_editor( PictureBox _pbox, ImageList _tiles_imagelist ) : base( _pbox )
		{
			m_tiles_imagelist = _tiles_imagelist;
			
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
			
			m_block_tiles = new List< int >( utils.CONST_TILES_UINT_SIZE );
			
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
				int 		CHR_bank_ind 	= 0;
				tiles_data 	data			= null;
				
				m_layout_mode_border_tiles	= null;
				
				( sender as layout_editor ).get_sel_screen_data( out CHR_bank_ind, out m_scr_ind, out data, ref m_layout_mode_border_tiles );
				
				if( CHR_bank_ind != m_curr_CHR_bank_id )
				{
					m_scr_ind = -1;
				}
				
				m_tiles_data = data;
				
				update();
			}
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
			if( m_tile_img != null && m_tile_x >= 0 && m_tile_y >= 0 )
			{
				m_tile_x = -1;
			m_tile_y = -1;
				
				update();
			}
		}
		
		private void ScreenEditor_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if( ( m_tiles_data != null && m_scr_ind >= 0 && m_active_tile_id >= 0 ) && e.Button == MouseButtons.Left )
			{
				m_pbox_captured = true;

				if( put_tile( e.X, e.Y ) )
				{				
					m_tile_x = -1;
					m_tile_y = -1;
					
					update();
				}
			}
		}
		
		private void ScreenEditor_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_pbox_captured = false;
			
			if( mode == EMode.em_Layout )
			{
				if( e.Y < utils.CONST_SCREEN_OFFSET_Y )
				{
					if( RequestUpScreen != null )
					{
						RequestUpScreen( this, null );
					}
				}
				else
#if DEF_SCREEN_HEIGHT_7d5_TILES
				if( e.Y > utils.CONST_SCREEN_OFFSET_Y + ( utils.CONST_SCREEN_NUM_HEIGHT_TILES * utils.CONST_SCREEN_TILES_SIZE ) - ( utils.CONST_SCREEN_TILES_SIZE >> 1 ) )
#else
				if( e.Y > utils.CONST_SCREEN_OFFSET_Y + ( utils.CONST_SCREEN_NUM_HEIGHT_TILES * utils.CONST_SCREEN_TILES_SIZE ) )
#endif //DEF_SCREEN_HEIGHT_7d5_TILES						
				{
					if( RequestDownScreen != null )
					{
						RequestDownScreen( this, null );
					}
				}
				else
				if( e.X < utils.CONST_SCREEN_OFFSET_X )
				{
					if( RequestLeftScreen != null )
					{
						RequestLeftScreen( this, null );
					}
				}
				else
				if( e.X > utils.CONST_SCREEN_OFFSET_X + ( utils.CONST_SCREEN_NUM_WIDTH_TILES * utils.CONST_SCREEN_TILES_SIZE ) )
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
			if( m_pbox_captured )
			{
				if( put_tile( e.X, e.Y ) )
				{
					m_tile_x = -1;
					m_tile_y = -1;
					
					update();
				}
			}
			else
			{
				int new_tile_x = 0;
				int new_tile_y = 0;
				
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

		public void update_adjacent_screen_tiles()
		{
			if( mode == EMode.em_Layout )
			{
				if( RequestUpScreen != null )
				{
					RequestUpScreen( this, null );
				}
				
				if( RequestDownScreen != null )
				{
					RequestDownScreen( this, null );
				}
				
				if( RequestLeftScreen != null )
				{
					RequestLeftScreen( this, null );
				}
				
				if( RequestRightScreen != null )
				{
					RequestRightScreen( this, null );
				}
			}
		}
		
		private string get_fill_mode_str()
		{
			return ( m_fill_mode == EFillMode.efm_Tile ) ? "Tile":( ( m_fill_mode == EFillMode.efm_Block ) ? "Block":"???" );
		}
		
		private bool get_tile_xy( int _x, int _y, out int _tile_x, out int _tile_y )
		{
			_tile_x = -1;
			_tile_y = -1;
			
			int offs_x = _x - utils.CONST_SCREEN_OFFSET_X;
			int offs_y = _y - utils.CONST_SCREEN_OFFSET_Y;
			
			if( offs_x < 0 || offs_y < 0 )
			{
				return false;
			}
			
			int tile_size 			= ( m_fill_mode == EFillMode.efm_Tile ) ? utils.CONST_SCREEN_TILES_SIZE:utils.CONST_SCREEN_BLOCKS_SIZE;
			
			int scr_width_tiles		= ( m_fill_mode == EFillMode.efm_Tile ) ? utils.CONST_SCREEN_NUM_WIDTH_TILES:utils.CONST_SCREEN_NUM_WIDTH_BLOCKS;
			int scr_height_tiles 	= ( m_fill_mode == EFillMode.efm_Tile ) ? utils.CONST_SCREEN_NUM_HEIGHT_TILES:utils.CONST_SCREEN_NUM_HEIGHT_BLOCKS;
			
			int x = offs_x / tile_size;
			
#if DEF_SCREEN_HEIGHT_7d5_TILES
			int y = ( m_fill_mode == EFillMode.efm_Tile ) ? ( offs_y / ( tile_size >> 1 ) ):( offs_y / tile_size );

			bool out_of_bound = ( m_fill_mode == EFillMode.efm_Tile ) ? ( x >= scr_width_tiles || y >= ( scr_height_tiles << 1 ) - 1 ):( x >= scr_width_tiles || y >= ( scr_height_tiles - 1 ) );
			
			if( out_of_bound )
#else
			int y = offs_y / tile_size;
				
			if( x >= scr_width_tiles || y >= scr_height_tiles )
#endif //DEF_SCREEN_HEIGHT_7d5_TILES
			{
				return false;
			}
			
			_tile_x = x;
			
#if DEF_SCREEN_HEIGHT_7d5_TILES			
			_tile_y = offs_y / tile_size;
#else
			_tile_y = y;
#endif //DEF_SCREEN_HEIGHT_7d5_TILES
			
			return true;
		}
		
		private bool put_tile( int _x, int _y )
		{
			if( get_tile_xy( _x, _y, out m_tile_x, out m_tile_y ) == false )
			{
				return false;
			}
			
			int tile_id = m_active_tile_id;
			
			if( m_fill_mode == EFillMode.efm_Block )
			{
				uint new_tile = build_new_tile( m_tile_x, m_tile_y );
				
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
			
			m_tiles_data.scr_data[ m_scr_ind ][ ( m_tile_y * utils.CONST_SCREEN_NUM_WIDTH_TILES ) + m_tile_x ] = (byte)tile_id;
			
			if( NeedScreensUpdate != null )
			{
				NeedScreensUpdate( this, null );
			}
			
			return true;
		}
		
		public void reset_last_empty_tile_ind()
		{
			m_last_empty_tile_ind = -1;
		}
		
		private int save_new_tile( uint _tile )
		{
			m_last_empty_tile_ind = m_last_empty_tile_ind < 0 ? 1:m_last_empty_tile_ind;	// skip zero tile as reserved clear space tile
			
			for( int i = m_last_empty_tile_ind; i < utils.CONST_TILES_UINT_SIZE; i++ )
			{
				if( m_tiles_data.tiles[ i ] == 0 )
				{
					m_tiles_data.tiles[ i ] = _tile;
					
					// add the tile to the cache
					m_block_tiles.Add( i );
					
					// redraw tile image here...
					if( UpdateTileImage != null )
					{
						UpdateTileImage( this, new NewTileEventArg( i, m_tiles_data ) );
					}
					
					m_last_empty_tile_ind = i;
					
					return i;
				}
			}
			
			m_pbox_captured = false;
			
			m_last_empty_tile_ind = -1;
				
			// TILES ARRAY OVERFLOW !!!
			MainForm.message_box( "Try to optimize the tiles data!", "Tiles Array Overflow", System.Windows.Forms.MessageBoxButtons.OK );
			
			return -1;
		}
		
		private int check_tile( uint _tile_data )
		{
			int tile_ind;
			
			int size = m_block_tiles.Count;
			
			for( int i = 0; i < size; i++ )
			{
				tile_ind = m_block_tiles[ i ];
				
				if( m_tiles_data.tiles[ tile_ind ] == _tile_data )
				{
					return tile_ind;
				}
			}
			
			return -1;
		}
		
		private uint build_new_tile( int _tile_x, int _tile_y )
		{
			uint old_tile = m_tiles_data.tiles[ m_tiles_data.scr_data[ m_scr_ind ][ ( ( _tile_y >> 1 ) * utils.CONST_SCREEN_NUM_WIDTH_TILES ) + ( _tile_x >> 1 ) ] ];
			
			int block_ind = ( _tile_x & 0x01 ) + ( ( _tile_y & 0x01 ) << 1 );
			
			return utils.set_byte_to_uint( old_tile, block_ind, ( byte )m_active_tile_id );
		}
		
		public void set_active_tile( int _tile_ind, Image _img, EFillMode _fill_mode )
		{
			if( m_tiles_data != null )
			{
				bool need_update = m_fill_mode != _fill_mode;
				
				m_fill_mode = _fill_mode;
				
				m_block_tiles.Clear();
				
				switch( _fill_mode )
				{
					case EFillMode.efm_Tile:
						{
							m_tile_ghost_img_rect.Width = utils.CONST_SCREEN_TILES_SIZE;
							m_tile_ghost_img_rect.Height = utils.CONST_SCREEN_TILES_SIZE;
						}
						break;
						
					case EFillMode.efm_Block:
						{
							m_tile_ghost_img_rect.Width = utils.CONST_SCREEN_BLOCKS_SIZE;
							m_tile_ghost_img_rect.Height = utils.CONST_SCREEN_BLOCKS_SIZE;
							
							calc_common_blocks( ( byte )_tile_ind );
						}
						break;
				}
				
				m_active_tile_id 	= _tile_ind;
				m_tile_img			= _img;
				
				if( m_tile_img == null )
				{
					m_tile_x = -1;
					m_tile_y = -1;
				}
				
				if( need_update )
				{
					update();
				}
			}
		}
		
		private void calc_common_blocks( byte _block_ind )
		{
			int i;
			int j;	

			uint tile_data;
			
			m_last_empty_tile_ind = -1;
			
			m_block_tiles.Clear();
			
			for( i = 0; i < utils.CONST_TILES_UINT_SIZE; i++ )
			{
				tile_data = m_tiles_data.tiles[ i ];
				
				for( j = 0; j < utils.CONST_TILE_SIZE; j++ )
				{
					if( i > 0 && m_last_empty_tile_ind < 0 && tile_data == 0 )
					{
						m_last_empty_tile_ind = i;
					}
					
					if( utils.get_byte_from_uint( tile_data, j ) == _block_ind )
					{
						m_block_tiles.Add( i );
			
						break;
					}
				}
			}
		}
		
		public void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_tiles_data != null )
			{
				int i;
			
				if( mode == EMode.em_Layout && m_layout_mode_border_tiles != null )
				{
					const short zero_tile = unchecked( (short)0xffff );
					
					short	tile_data;
					int 	tile_offset1_x = utils.CONST_SCREEN_OFFSET_X - utils.CONST_SCREEN_TILES_SIZE;
					int 	tile_offset1_y = utils.CONST_SCREEN_OFFSET_Y - utils.CONST_SCREEN_TILES_SIZE;
					
					int 	tile_offset2_x = utils.CONST_SCREEN_OFFSET_X + ( utils.CONST_SCREEN_NUM_WIDTH_TILES * utils.CONST_SCREEN_TILES_SIZE );
//					int 	tile_offset2_y = utils.CONST_SCREEN_OFFSET_Y + ( utils.CONST_SCREEN_NUM_HEIGHT_TILES * utils.CONST_SCREEN_TILES_SIZE );

					// draw border tiles
					// upper/lower line
					{
						for( i = 0; i < utils.CONST_SCREEN_NUM_WIDTH_TILES + 2; i++ )
						{
							tile_data = m_layout_mode_border_tiles[ i ];
							
							if( tile_data != zero_tile )
							{
								if( ( ( tile_data & 0xff00 ) >> 8 ) == m_curr_CHR_bank_id )
								{
#if DEF_SCREEN_HEIGHT_7d5_TILES
									m_border_tile_img_rect.X		= tile_offset1_x + ( i * utils.CONST_SCREEN_TILES_SIZE );
									m_border_tile_img_rect.Y		= tile_offset1_y + ( utils.CONST_SCREEN_TILES_SIZE >> 1 );
									m_border_tile_img_rect.Height	= utils.CONST_SCREEN_TILES_SIZE >> 1;

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
													m_border_tile_img_rect,                
									                0, 
									                0, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                m_border_tile_img_rect.Height, 
									                GraphicsUnit.Pixel, 
									                m_tile_img_attr );
#else
									m_border_tile_img_rect.X = tile_offset1_x + ( i * utils.CONST_SCREEN_TILES_SIZE );
									m_border_tile_img_rect.Y = tile_offset1_y;

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
													m_border_tile_img_rect,									                
									                0, 
									                0, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                GraphicsUnit.Pixel, 
									                m_tile_img_attr );
#endif //DEF_SCREEN_HEIGHT_7d5_TILES
								}
							}
							
							tile_data = m_layout_mode_border_tiles[ i + utils.CONST_ADJ_SCR_TILE_IND_DOWN_LEFT ];
							
							if( tile_data != zero_tile )
							{
								if( ( ( tile_data & 0xff00 ) >> 8 ) == m_curr_CHR_bank_id )
								{
#if DEF_SCREEN_HEIGHT_7d5_TILES
									m_border_tile_img_rect.X		= tile_offset1_x + ( i * utils.CONST_SCREEN_TILES_SIZE );
									m_border_tile_img_rect.Y		= tile_offset1_y + ( ( utils.CONST_SCREEN_NUM_HEIGHT_TILES + 1 ) * utils.CONST_SCREEN_TILES_SIZE ) - ( utils.CONST_SCREEN_TILES_SIZE >> 1 );
									m_border_tile_img_rect.Height	= utils.CONST_SCREEN_TILES_SIZE;

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
													m_border_tile_img_rect,									                
									                0, 
									                0,
									                utils.CONST_SCREEN_TILES_SIZE, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                GraphicsUnit.Pixel, 
									                m_tile_img_attr );
#else
									m_border_tile_img_rect.X = tile_offset1_x + ( i * utils.CONST_SCREEN_TILES_SIZE );
									m_border_tile_img_rect.Y = tile_offset1_y + ( ( utils.CONST_SCREEN_NUM_HEIGHT_TILES + 1 ) * utils.CONST_SCREEN_TILES_SIZE );

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
													m_border_tile_img_rect,                
									                0, 
									                0, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                GraphicsUnit.Pixel, 
									                m_tile_img_attr );
#endif //DEF_SCREEN_HEIGHT_7d5_TILES									                
								}
							}
						}
					}
					
					// left/right
					{
						for( i = 0; i < utils.CONST_SCREEN_NUM_HEIGHT_TILES; i++ )
						{
							tile_data = m_layout_mode_border_tiles[ i + utils.CONST_ADJ_SCR_TILE_IND_LEFT ];
							
							if( tile_data != zero_tile )
							{
								if( ( ( tile_data & 0xff00 ) >> 8 ) == m_curr_CHR_bank_id )
								{
#if DEF_SCREEN_HEIGHT_7d5_TILES
									m_border_tile_img_rect.X		= tile_offset1_x;
									m_border_tile_img_rect.Y		= utils.CONST_SCREEN_OFFSET_Y + ( i * utils.CONST_SCREEN_TILES_SIZE );
									m_border_tile_img_rect.Height	= ( i < 7 ) ? utils.CONST_SCREEN_TILES_SIZE:utils.CONST_SCREEN_TILES_SIZE >> 1;

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
									                m_border_tile_img_rect,
													0, 
													0,
													utils.CONST_SCREEN_TILES_SIZE,
													m_border_tile_img_rect.Height,
													GraphicsUnit.Pixel,
													m_tile_img_attr );
#else								
									m_border_tile_img_rect.X = tile_offset1_x;
									m_border_tile_img_rect.Y = utils.CONST_SCREEN_OFFSET_Y + ( i * utils.CONST_SCREEN_TILES_SIZE );

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
													m_border_tile_img_rect,									                
									                0, 
									                0, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                utils.CONST_SCREEN_TILES_SIZE,
									                GraphicsUnit.Pixel,
									                m_tile_img_attr );
#endif //DEF_SCREEN_HEIGHT_7d5_TILES									                
								}
							}
							
							tile_data = m_layout_mode_border_tiles[ i + utils.CONST_ADJ_SCR_TILE_IND_RIGHT ];
							
							if( tile_data != zero_tile )
							{
								if( ( ( tile_data & 0xff00 ) >> 8 ) == m_curr_CHR_bank_id )
								{
#if DEF_SCREEN_HEIGHT_7d5_TILES
									m_border_tile_img_rect.X		= tile_offset2_x;
									m_border_tile_img_rect.Y		= utils.CONST_SCREEN_OFFSET_Y + ( i * utils.CONST_SCREEN_TILES_SIZE );
									m_border_tile_img_rect.Height	= ( i < 7 ) ? utils.CONST_SCREEN_TILES_SIZE:utils.CONST_SCREEN_TILES_SIZE >> 1;

									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
									                m_border_tile_img_rect,
													0, 
													0,
													utils.CONST_SCREEN_TILES_SIZE,
													m_border_tile_img_rect.Height,
													GraphicsUnit.Pixel,
													m_tile_img_attr );
#else									
									m_border_tile_img_rect.X = tile_offset2_x;
									m_border_tile_img_rect.Y = utils.CONST_SCREEN_OFFSET_Y + ( i * utils.CONST_SCREEN_TILES_SIZE );
									
									m_gfx.DrawImage( m_tiles_imagelist.Images[ tile_data & 0x00ff ],
													m_border_tile_img_rect,
									                0,
									                0, 
									                utils.CONST_SCREEN_TILES_SIZE, 
									                utils.CONST_SCREEN_TILES_SIZE,
									                GraphicsUnit.Pixel,
									                m_tile_img_attr );
#endif //DEF_SCREEN_HEIGHT_7d5_TILES									                
								}
							}
						}
					}
				}
				
				// draw a tiled screen
				if( m_scr_ind >= 0 )
				{
					byte[] tile_ids = m_tiles_data.scr_data[ m_scr_ind ];
					for( i = 0; i < utils.CONST_SCREEN_TILES_CNT; i++ )
					{
#if DEF_SCREEN_HEIGHT_7d5_TILES
						m_border_tile_img_rect.X		= 0;
						m_border_tile_img_rect.Y		= 0;
						m_border_tile_img_rect.Height	= ( i / utils.CONST_SCREEN_NUM_WIDTH_TILES == 7 ) ? utils.CONST_SCREEN_TILES_SIZE >> 1:utils.CONST_SCREEN_TILES_SIZE;
						
						m_gfx.DrawImage( 	m_tiles_imagelist.Images[ tile_ids[ i ] ],
                							utils.CONST_SCREEN_OFFSET_X + ( i % utils.CONST_SCREEN_NUM_WIDTH_TILES ) * utils.CONST_SCREEN_TILES_SIZE, 
                							utils.CONST_SCREEN_OFFSET_Y + ( i / utils.CONST_SCREEN_NUM_WIDTH_TILES ) * utils.CONST_SCREEN_TILES_SIZE,
                							m_border_tile_img_rect,
                							GraphicsUnit.Pixel );
#else
						m_gfx.DrawImage( 	m_tiles_imagelist.Images[ tile_ids[ i ] ], 
                							utils.CONST_SCREEN_OFFSET_X + ( i % utils.CONST_SCREEN_NUM_WIDTH_TILES ) * utils.CONST_SCREEN_TILES_SIZE, 
                							utils.CONST_SCREEN_OFFSET_Y + ( i / utils.CONST_SCREEN_NUM_WIDTH_TILES ) * utils.CONST_SCREEN_TILES_SIZE, 
                							utils.CONST_SCREEN_TILES_SIZE, 
                							utils.CONST_SCREEN_TILES_SIZE );
#endif	//DEF_SCREEN_HEIGHT_7d5_TILES						
					}
					
					// draw a ghost image
					if( m_tile_img != null && m_tile_x >= 0 && m_tile_y >= 0 )
					{
						switch( m_fill_mode )
						{
							case EFillMode.efm_Tile:
								{
									m_tile_ghost_img_rect.X 		= utils.CONST_SCREEN_OFFSET_X + ( m_tile_x * utils.CONST_SCREEN_TILES_SIZE );
									m_tile_ghost_img_rect.Y 		= utils.CONST_SCREEN_OFFSET_Y + ( m_tile_y * utils.CONST_SCREEN_TILES_SIZE );
									
									m_gfx.DrawImage( m_tile_img, m_tile_ghost_img_rect, 0, 0, utils.CONST_SCREEN_TILES_SIZE, utils.CONST_SCREEN_TILES_SIZE, GraphicsUnit.Pixel, m_tile_img_attr );
								}
								break;

							case EFillMode.efm_Block:
								{
									m_tile_ghost_img_rect.X			= utils.CONST_SCREEN_OFFSET_X + ( m_tile_x * utils.CONST_SCREEN_BLOCKS_SIZE );
									m_tile_ghost_img_rect.Y			= utils.CONST_SCREEN_OFFSET_Y + ( m_tile_y * utils.CONST_SCREEN_BLOCKS_SIZE );
									
									m_gfx.DrawImage( m_tile_img, m_tile_ghost_img_rect, 0, 0, utils.CONST_SCREEN_BLOCKS_SIZE, utils.CONST_SCREEN_BLOCKS_SIZE, GraphicsUnit.Pixel, m_tile_img_attr );
								}
								break;
						}
					}
				}
				
				if( m_draw_grid )
				{
					m_pen.Color = ( m_fill_mode == EFillMode.efm_Block ) ? utils.CONST_COLOR_SCREEN_GRID_THICK_BLOCK_MODE:utils.CONST_COLOR_SCREEN_GRID_THICK_TILE_MODE;
					
					int offs_x;
					int offs_y;
					
					for( i = 0; i < utils.CONST_SCREEN_NUM_WIDTH_TILES + 1; i++ )
					{
						offs_x = utils.CONST_SCREEN_OFFSET_X + ( i * utils.CONST_SCREEN_TILES_SIZE );
						offs_y = ( utils.CONST_SCREEN_OFFSET_Y + ( i * utils.CONST_SCREEN_TILES_SIZE ) ) % m_pix_box.Height;
						
						m_gfx.DrawLine( m_pen, offs_x, 0, offs_x, m_pix_box.Height );
						
#if DEF_SCREEN_HEIGHT_7d5_TILES
						offs_y = ( i < 8 ) ? offs_y:offs_y - ( utils.CONST_SCREEN_TILES_SIZE >> 1 );
#endif //DEF_SCREEN_HEIGHT_7d5_TILES

						m_gfx.DrawLine( m_pen, 0, offs_y, m_pix_box.Width, offs_y );
					}
					
					// blocks grid
					{
						m_pen.Color = utils.CONST_COLOR_SCREEN_GRID_THIN;
						
						int start_offs_x = ( utils.CONST_SCREEN_OFFSET_X + ( utils.CONST_SCREEN_TILES_SIZE >> 1 ) );
						int start_offs_y = ( utils.CONST_SCREEN_OFFSET_Y + ( utils.CONST_SCREEN_TILES_SIZE >> 1 ) );
						
						for( i = 0; i < utils.CONST_SCREEN_NUM_WIDTH_TILES + 1; i++ )
						{
							offs_x = start_offs_x + ( i * utils.CONST_SCREEN_TILES_SIZE );
							offs_y = ( start_offs_y + ( i * utils.CONST_SCREEN_TILES_SIZE ) ) % m_pix_box.Height;
							
							m_gfx.DrawLine( m_pen, offs_x, 0, offs_x, m_pix_box.Height );
							
#if DEF_SCREEN_HEIGHT_7d5_TILES
							offs_y = ( i < 7 ) ? offs_y:offs_y + ( utils.CONST_SCREEN_TILES_SIZE >> 1 );
#endif //DEF_SCREEN_HEIGHT_7d5_TILES							
							
							m_gfx.DrawLine( m_pen, 0, offs_y, m_pix_box.Width, offs_y );
						}
					}
				}

				// red rectangle
				{
					m_pen.Color = Color.Red;
					m_pen.Width = 2.0f;
					
					int width 	= utils.CONST_SCREEN_NUM_WIDTH_TILES * utils.CONST_SCREEN_TILES_SIZE;
					int height	= utils.CONST_SCREEN_NUM_HEIGHT_TILES * utils.CONST_SCREEN_TILES_SIZE;
					
#if DEF_SCREEN_HEIGHT_7d5_TILES
					m_gfx.DrawRectangle( m_pen, utils.CONST_SCREEN_OFFSET_X, utils.CONST_SCREEN_OFFSET_Y, width, height - ( utils.CONST_SCREEN_TILES_SIZE >> 1 ) );
#else
					m_gfx.DrawRectangle( m_pen, utils.CONST_SCREEN_OFFSET_X, utils.CONST_SCREEN_OFFSET_Y, width, height );
#endif					
					m_pen.Width = 1.0f;
				}
				
				disable( false );
			}
			else
			{
				disable( true );
				
				if( mode == EMode.em_Layout )
				{
					utils.brush.Color = utils.CONST_COLOR_STRING_DEFAULT;
					m_gfx.DrawString( "[ Select an active layout and click on the layout's screen to edit it ]", utils.fnt10_Arial, utils.brush, 0, 0 );
				}
			}
			
			invalidate();
		}
	}
}
