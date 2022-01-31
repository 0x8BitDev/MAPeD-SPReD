/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 16:20
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Generic;

//#define DEF_SCREEN_DRAW_FAST

namespace MAPeD
{
	/// <summary>
	/// Description of imagelist_manager.
	/// </summary>
	public class imagelist_manager
	{
		private readonly ListView	m_listview_screens	= null;
		private i_screen_list		m_scr_list			= null;
		private Size				m_scr_img_size		= new Size( 0, 0 );
		
		private readonly FlowLayoutPanel m_panel_tiles	= null;
		private readonly FlowLayoutPanel m_panel_blocks	= null;
		
		private readonly ImageList m_imagelist_blocks	= null;
		private readonly ImageList m_imagelist_tiles	= null;
		
		private readonly tile_list	m_tile_list		= null;
		private readonly tile_list	m_block_list	= null;
		
		private static Rectangle	m_block_rect	= new Rectangle( 0, 0, utils.CONST_BLOCKS_IMG_SIZE, utils.CONST_BLOCKS_IMG_SIZE );
		private static Rectangle	m_tile_rect		= new Rectangle( 0, 0, utils.CONST_TILES_IMG_SIZE, utils.CONST_TILES_IMG_SIZE );
		
		private readonly static int[]	m_clrs_arr = new int[ 16 ]{ 0x00ffffff, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00ff4500, 0x00dc803c, 0x00406080, 0x00ffff00,	0x00ffa500, 0x0020b2aa, 0x0000ffff, 0x00808000,	0x00800080, 0x00c0c0c0,	0x007b68ee, 0xff1493 };
		
		private const int CONST_ALPHA = 0x60;
		
		public imagelist_manager(	FlowLayoutPanel		_panel_tiles,
		                         	EventHandler		_tiles_e, 
		                         	ContextMenuStrip	_tiles_cm, 
		                         	FlowLayoutPanel		_panel_blocks, 
		                         	EventHandler		_blocks_e, 
		                         	ContextMenuStrip	_blocks_cm, 
		                         	ListView			_listview_screens,
									tile_list_manager	_tl_cntnr )
		{
			m_panel_tiles		= _panel_tiles;
			m_panel_blocks		= _panel_blocks;
			
			m_listview_screens	= _listview_screens;
			
			m_imagelist_tiles 	= imagelist_init( platform_data.get_max_tiles_cnt(), utils.CONST_TILES_IMG_SIZE );
			m_imagelist_blocks 	= imagelist_init( platform_data.get_max_blocks_cnt(), utils.CONST_BLOCKS_IMG_SIZE );
			
			m_tile_list		= new tile_list( tile_list.EType.t_Tiles, m_panel_tiles, m_imagelist_tiles, _tiles_e, _tiles_cm, _tl_cntnr );
			m_block_list	= new tile_list( tile_list.EType.t_Blocks, m_panel_blocks, m_imagelist_blocks, _blocks_e, _blocks_cm, _tl_cntnr );
			
			listview_init_screens();
		}
		
		private void listview_init_screens()
		{
			ImageList il	= new ImageList();
			
			update_screen_image_size();
			
			il.ImageSize	= m_scr_img_size; 
			il.ColorDepth	= ColorDepth.Depth24Bit;// 32bit - way too slow rendering... I don't know why...

			m_scr_list = new screen_list_scaled( il );
			
			m_listview_screens.LargeImageList = il;
		}
	
		public void update_screen_image_size()
		{
			float ratio	= platform_data.get_screen_height_pixels() / ( float )platform_data.get_screen_width_pixels();
			
			m_scr_img_size.Width	= 256;
			m_scr_img_size.Height	= ( int )( 256 * ratio );
			
			if( m_scr_img_size.Height > 256 )
			{
				ratio = platform_data.get_screen_width_pixels() / ( float )platform_data.get_screen_height_pixels();
				
				m_scr_img_size.Width	= ( int )( 256 * ratio );
				m_scr_img_size.Height	= 256;
			}
			
			if( m_scr_list != null )
			{
				m_scr_list.set_image_list_size( m_scr_img_size );
			}
		}
		
		private ImageList imagelist_init( int _cnt, int _size )
		{
			int i;
			
			int img_length = _size * _size;
			
			int[] img_buff = new int[ img_length ];
			
			for( i = 0; i < img_length; i++ )
			{
				img_buff[ i ] = 0xff<<24;
			}
			
			GCHandle handle = GCHandle.Alloc( img_buff, GCHandleType.Pinned );
			
			ImageList il = new ImageList();
			il.ImageSize = new Size( _size, _size );
			il.ColorDepth = ColorDepth.Depth24Bit;// 32bit - way too slow rendering... I don't know why...
			
			Bitmap bmp;

			for( i = 0; i < _cnt; i++ )
			{
				bmp = new Bitmap( _size, _size, _size << 2, PixelFormat.Format32bppArgb, handle.AddrOfPinnedObject() );
				
				il.Images.Add( i.ToString(), bmp );				
			}
			
			handle.Free();
			
			return il;
		}
		
		public void update_tiles( utils.ETileViewType _view_type, tiles_data _tiles_data, data_sets_manager.EScreenDataType _scr_type )
		{
			Image 		img;
			Graphics	gfx;
			
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				for( int i = 0; i < platform_data.get_max_tiles_cnt(); i++ )
				{
					img = m_imagelist_tiles.Images[ i ];
					
					gfx = Graphics.FromImage( img );
					
					if( _tiles_data != null )
					{
						update_tile( i, _view_type, _tiles_data, gfx, img, _scr_type );
					}
					else
					{
						gfx.Clear( utils.CONST_COLOR_TILE_CLEAR );
					}
					
					m_imagelist_tiles.Images[ i ] = img;
					
					gfx.Dispose();
				}
				
				m_panel_tiles.Refresh();
			}
		}

		public void update_tile( int _tile_ind, utils.ETileViewType _view_type, tiles_data _tiles_data, Graphics _gfx, Image _img, data_sets_manager.EScreenDataType _scr_type )
		{
#if DEF_TILE_DRAW_FAST
			Image	block_img;
#else
			ushort[] 	blocks_arr 	= null;
			byte[] 		CHR_data 	= null;

			// draw a block from CHR bank sprites
			if( _tiles_data != null )
			{
				blocks_arr 	= _tiles_data.blocks;
				CHR_data 	= _tiles_data.CHR_bank;
			}
#endif //DEF_TILE_DRAW_FAST
			
			Image img;
			
			if( _img != null )
			{
				img = _img;
			}
			else
			{
				img = m_imagelist_tiles.Images[ _tile_ind ];
			}
			
			Graphics gfx;
			
			if( _gfx != null )
			{
				gfx = _gfx;
			}
			else
			{
				gfx = Graphics.FromImage( img );
			}
			
			gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode 	= PixelOffsetMode.None;

#if DEF_TILE_DRAW_FAST					
			for( int j = 0; j < utils.CONST_TILE_SIZE; j++ )
			{
				block_img = m_imagelist_blocks.Images[ _tiles_data.get_tile_block( _tile_ind, j ) ];
				
				gfx.DrawImage( block_img, ( ( j % 2 ) << 5 ), ( ( j >> 1 ) << 5 ), block_img.Width, block_img.Height );
			}
#else
			// draw a block from CHR bank sprites
			for( int j = 0; j < utils.CONST_TILE_SIZE; j++ )
			{
				utils.update_block_gfx( _tiles_data.get_tile_block( _tile_ind, j ), blocks_arr, CHR_data, gfx, utils.CONST_BLOCKS_IMG_SIZE >> 1, utils.CONST_BLOCKS_IMG_SIZE >> 1, ( ( j % 2 ) << 5 ), ( ( j >> 1 ) << 5 ) );
			}
#endif //DEF_TILE_DRAW_FAST

			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				if( _view_type == utils.ETileViewType.tvt_Number ) // tile id
				{
					draw_tile_info( String.Format( "{0:X2}", _tile_ind ), gfx );
				}
				else
				if( _view_type == utils.ETileViewType.tvt_TilesUsage ) // usage
				{
					draw_tile_info( String.Format( "{0}", _tiles_data.get_tile_usage( ( ushort )_tile_ind, _scr_type ) ), gfx );
				}
			}
			
			if( _img == null )
			{
				m_imagelist_tiles.Images[ _tile_ind ] = img;
			}
			
			if( _gfx == null )
			{
				gfx.Dispose();
			}
		}
		
		private void draw_tile_info( string _info, Graphics _gfx )
		{
			utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | 0x00ffffff );
			_gfx.FillRectangle( utils.brush, m_tile_rect );

			utils.brush.Color = Color.FromArgb( unchecked( (int)0xff000000 ) );
			_gfx.DrawString( _info, utils.fnt12_Arial, utils.brush, 3, 2 );
			
			utils.brush.Color = Color.FromArgb( unchecked( (int)0xffffffff ) );
			_gfx.DrawString( _info, utils.fnt12_Arial, utils.brush, 1, 1 );
		}
		
		public void update_blocks( utils.ETileViewType _view_type, tiles_data _tiles_data, bool _prop_per_block, data_sets_manager.EScreenDataType _scr_type )
		{
			Image 		img;
			Graphics	gfx;
			
			m_block_rect.Width = m_block_rect.Height = ( _view_type == utils.ETileViewType.tvt_ObjectId && !_prop_per_block ) ? ( utils.CONST_BLOCKS_IMG_SIZE >> 1 ):utils.CONST_BLOCKS_IMG_SIZE;
			
			for( int i = 0; i < platform_data.get_max_blocks_cnt(); i++ )
			{
				img = m_imagelist_blocks.Images[ i ];
				
				gfx = Graphics.FromImage( img );
				
				if( _tiles_data != null )
				{
					update_block( i, _view_type, _tiles_data, _prop_per_block, gfx, img, _scr_type );
				}
				else
				{
					gfx.Clear( utils.CONST_COLOR_BLOCK_CLEAR );
				}
				
				m_imagelist_blocks.Images[ i ] = img;
				
				gfx.Dispose();
			}
			
			m_panel_blocks.Refresh();
		}

		public void update_block( int _block_ind, utils.ETileViewType _view_type, tiles_data _tiles_data, bool _prop_per_block, Graphics _gfx, Image _img, data_sets_manager.EScreenDataType _scr_type )
		{
			int obj_id;
			
			Image img;
			
			if( _img != null )
			{
				img = _img;
			}
			else
			{
				img = m_imagelist_blocks.Images[ _block_ind ];
			}
			
			Graphics gfx;
			
			if( _gfx != null )
			{
				gfx = _gfx;
			}
			else
			{
				gfx = Graphics.FromImage( img );
			}
			
			gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode 	= PixelOffsetMode.Half;
			
#if DEF_ZX
			utils.update_block_gfx( _block_ind, _tiles_data, gfx, img.Width >> 1, img.Height >> 1, 0, 0, utils.get_draw_block_flags_by_view_type( _view_type ) );
#else
			utils.update_block_gfx( _block_ind, _tiles_data, gfx, img.Width >> 1, img.Height >> 1 );
#endif
			if( _view_type == utils.ETileViewType.tvt_ObjectId ) // obj id
			{
				if( _prop_per_block )
				{
					obj_id = tiles_data.get_block_flags_obj_id( _tiles_data.blocks[ _block_ind << 2 ] );
					
					utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | m_clrs_arr[ obj_id ] );
					
					draw_block_info( String.Format( "{0}", obj_id ), gfx );
				}
				else
				{
					for( int chr_n = 0; chr_n < utils.CONST_BLOCK_SIZE; chr_n++ )
					{
						obj_id = tiles_data.get_block_flags_obj_id( _tiles_data.blocks[ ( _block_ind << 2 ) + chr_n ] );
						
						utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | m_clrs_arr[ obj_id ] );
						
						m_block_rect.X 	= ( ( chr_n&0x01 ) == 0x01 ) ? m_block_rect.Width:0;
						m_block_rect.Y 	= ( ( chr_n&0x02 ) == 0x02 ) ? m_block_rect.Height:0;
						
						gfx.FillRectangle( utils.brush, m_block_rect );
					}
				}
			}
			else
			if( _view_type == utils.ETileViewType.tvt_BlocksUsage ) // usage
			{
				utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | 0x00ffffff );
				
				draw_block_info( String.Format( "{0}", _tiles_data.get_block_usage( ( ushort )_block_ind, _scr_type ) ), gfx );
			}
			else
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Blocks2x2 )
			{
				if( _view_type == utils.ETileViewType.tvt_Number ) // block id
				{
					utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | 0x00ffffff );
					
					draw_block_info( String.Format( "{0:X2}", _block_ind ), gfx );
				}
			}
			
			if( _img == null )
			{
				m_imagelist_blocks.Images[ _block_ind ] = img;
			}
			
			if( _gfx == null )
			{
				gfx.Dispose();
			}
		}

		private void draw_block_info( string _info, Graphics _gfx )
		{
			m_block_rect.X = m_block_rect.Y = 0;
			_gfx.FillRectangle( utils.brush, m_block_rect );
			
			utils.brush.Color = Color.FromArgb( unchecked( (int)0xff000000 ) );
			_gfx.DrawString( _info, utils.fnt10_Arial, utils.brush, 1, 0 );
			
			utils.brush.Color = Color.FromArgb( unchecked( (int)0xffffffff ) );
			_gfx.DrawString( _info, utils.fnt10_Arial, utils.brush, 0, 0 );
		}

		public bool copy_screens_to_the_end( List< tiles_data > _tiles_data, int _bank_id )
		{
			if( _bank_id >= 0 )
			{
				m_listview_screens.BeginUpdate();
				
				tiles_data 		data;
	
				int screens_cnt;
				int screens_ind = 0;
				int banks_cnt 	= _tiles_data.Count;
				
				for( int bank_n = 0; bank_n < banks_cnt; bank_n++ )
				{
					data = _tiles_data[ bank_n ];
	
					screens_cnt = data.screen_data_cnt();
	
					if( _bank_id == bank_n )
					{
						for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
						{
							m_scr_list.add( ( Image )m_scr_list.get( screens_ind + screen_n ).Clone() );
						}
						
						break;
					}
					
					screens_ind += screens_cnt;
				}
				
				m_listview_screens.EndUpdate();
				
				return true;
			}
			
			return false;
		}
		
		public void update_all_screens( List< tiles_data > _tiles_data, data_sets_manager.EScreenDataType _scr_type, utils.ETileViewType _view_type )
		{
			m_listview_screens.BeginUpdate();
			
			// clear items
			{
				m_scr_list.clear();
				m_listview_screens.Items.Clear();
			}
			
			tiles_data 		data;
			ListViewItem	lst;

			int screens_cnt;
			int img_ind 	= 0;
			int banks_cnt 	= _tiles_data.Count;
			
			for( int bank_n = 0; bank_n < banks_cnt; bank_n++ )
			{
				data = _tiles_data[ bank_n ];

				palette_group.Instance.set_palette( data );
				
				screens_cnt = data.screen_data_cnt();
				
				for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
				{
					m_scr_list.add( create_screen_image( screen_n, data, _scr_type, _view_type ) );
					
				 	lst 				= new ListViewItem();
				 	lst.Name = lst.Text = utils.get_screen_id_str( bank_n, screen_n );
					lst.ImageIndex		= img_ind;
 
					++img_ind;
 
					m_listview_screens.Items.Add( lst );
				}
			}
			
			m_listview_screens.EndUpdate();
		}

		public void update_screens( List< tiles_data > _tiles_data, data_sets_manager.EScreenDataType _scr_type, bool _update_images, utils.ETileViewType _view_type, int _bank_id = -1 )
		{
			m_listview_screens.BeginUpdate();
			
			m_listview_screens.Items.Clear();
			
			tiles_data 		data;
			ListViewItem	lst;

			int img_ind;
			int screens_cnt;
			int screens_ind = 0;
			int banks_cnt 	= _tiles_data.Count;
			
			for( int bank_n = 0; bank_n < banks_cnt; bank_n++ )
			{
				data = _tiles_data[ bank_n ];

				screens_cnt = data.screen_data_cnt();

				if( ( _bank_id >= 0 && _bank_id == bank_n ) || _bank_id < 0 )
				{
					palette_group.Instance.set_palette( data );
					
					for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
					{
						img_ind = screens_ind + screen_n;
						
						if( _update_images )
						{
							m_scr_list.get( img_ind ).Dispose();
							m_scr_list.replace( img_ind, create_screen_image( screen_n, data, _scr_type, _view_type ) );
						}
						
					 	lst 				= new ListViewItem();
					 	lst.Name = lst.Text = utils.get_screen_id_str( bank_n, screen_n );
						lst.ImageIndex		= img_ind;

						m_listview_screens.Items.Add( lst );
					}
				}
				
				screens_ind += screens_cnt;
			}
			
			m_listview_screens.EndUpdate();
		}
		
		private Bitmap create_screen_image( int _screen_n, tiles_data _data, data_sets_manager.EScreenDataType _scr_type, utils.ETileViewType _view_type )
		{
#if DEF_SCREEN_DRAW_FAST
			Image	block_img;
#endif			
			int tile_n;
			int block_n;
			
			ushort tile_id;
			
			int tile_offs_x;
			int tile_offs_y;
			
			palette_group.Instance.set_palette( _data );

			Bitmap bmp = new Bitmap( platform_data.get_screen_width_pixels(), platform_data.get_screen_height_pixels(), PixelFormat.Format32bppArgb );
			
			Graphics gfx = Graphics.FromImage( bmp );
			
			gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode 	= PixelOffsetMode.None;
			
			gfx.Clear( utils.CONST_COLOR_SCREEN_CLEAR );
			
			if( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 )
			{
				for( tile_n = 0; tile_n < platform_data.get_screen_tiles_cnt(); tile_n++ )
				{
					tile_id = _data.get_screen_tile( _screen_n, tile_n );
					
					tile_offs_x = ( tile_n % platform_data.get_screen_tiles_width() ) << 5;
					tile_offs_y = ( tile_n / platform_data.get_screen_tiles_width() ) << 5;
					
					for( block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
					{
#if DEF_SCREEN_DRAW_FAST
						block_img = m_imagelist_blocks.Images[ _data.get_tile_block( tile_id, block_n ) ];
						
						gfx.DrawImage( block_img, ( ( block_n % 2 ) << 4 ) + tile_offs_x, ( ( block_n >> 1 ) << 4 ) + tile_offs_y, block_img.Width >> 1, block_img.Height >> 1 );
#else
#if DEF_ZX
						utils.update_block_gfx( _data.get_tile_block( tile_id, block_n ), _data, gfx, 8, 8, ( ( block_n % 2 ) << 4 ) + tile_offs_x, ( ( block_n >> 1 ) << 4 ) + tile_offs_y, utils.get_draw_block_flags_by_view_type( _view_type ) );
#else
						utils.update_block_gfx( _data.get_tile_block( tile_id, block_n ), _data, gfx, 8, 8, ( ( block_n % 2 ) << 4 ) + tile_offs_x, ( ( block_n >> 1 ) << 4 ) + tile_offs_y );
#endif
#endif
					}
				}
			}
			else
			{
				for( tile_n = 0; tile_n < platform_data.get_screen_blocks_cnt(); tile_n++ )
				{
#if DEF_SCREEN_DRAW_FAST
					block_img = m_imagelist_blocks.Images[ _data.get_screen_tile( _screen_n, tile_n ) ];
					
					gfx.DrawImage( block_img, ( ( tile_n % platform_data.get_screen_blocks_width() ) << 4 ), ( ( tile_n / platform_data.get_screen_blocks_width() ) << 4 ), block_img.Width >> 1, block_img.Height >> 1 );
#else
#if DEF_ZX
					utils.update_block_gfx( _data.get_screen_tile( _screen_n, tile_n ), _data, gfx, 8, 8, ( ( tile_n % platform_data.get_screen_blocks_width() ) << 4 ), ( ( tile_n / platform_data.get_screen_blocks_width() ) << 4 ), utils.get_draw_block_flags_by_view_type( _view_type ) );
#else
					utils.update_block_gfx( _data.get_screen_tile( _screen_n, tile_n ), _data, gfx, 8, 8, ( ( tile_n % platform_data.get_screen_blocks_width() ) << 4 ), ( ( tile_n / platform_data.get_screen_blocks_width() ) << 4 ) );
#endif
#endif
				}
			}
			
			return bmp;
		}
		
		public void update_screens_labels( List< tiles_data > _tiles_data, int _bank_id = -1 )
		{
			if( m_listview_screens.Items.Count > 0 )
			{
				tiles_data 		data;
	
				int screens_cnt;
				int item_ind 	= 0;
				int banks_cnt 	= _tiles_data.Count;
				
				for( int bank_n = 0; bank_n < banks_cnt; bank_n++ )
				{
					data 		= _tiles_data[ bank_n ];
					screens_cnt = data.screen_data_cnt();
					
					for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
					{
						if( _bank_id == -1 )
						{
							m_listview_screens.Items[ item_ind++ ].Text = utils.get_screen_id_str( bank_n, screen_n );
						}
						else
						{
							if( bank_n == _bank_id )
							{
								m_listview_screens.Items[ item_ind++ ].Text = utils.get_screen_id_str( bank_n, screen_n );
							}
						}
					}
				}
			}
		}

		public bool remove_screen( int _CHR_bank_ind, int _scr_id )
		{
			bool res = false;
			
			string scr_id_str = utils.get_screen_id_str( _CHR_bank_ind, _scr_id );
			
			int size = m_listview_screens.Items.Count;
			
			for( int item_n = 0; item_n < size; item_n++ )
			{
				if( m_listview_screens.Items[ item_n ].Text == scr_id_str )
				{
					int remove_img_ind = m_listview_screens.Items[ item_n ].ImageIndex;
					
					for( int i = 0; i < m_listview_screens.Items.Count; i++ )
					{
						if( m_listview_screens.Items[ i ].ImageIndex > remove_img_ind )
						{
							--m_listview_screens.Items[ i ].ImageIndex;
						}
					}

					m_scr_list.remove( remove_img_ind );
					m_listview_screens.Items.RemoveAt( item_n );
					
					res = true;
					
					break;
				}
			}
	
			return res;
		}
		
		public void insert_screen( bool _all_banks_list, int _CHR_bank_ind, int _scr_local_ind, int _scr_global_ind, List< tiles_data > _tiles_data, data_sets_manager.EScreenDataType _scr_type, utils.ETileViewType _view_type )
		{
			int i;
			int size;
			
			m_listview_screens.BeginUpdate();

			Bitmap bmp = create_screen_image( _scr_local_ind, _tiles_data[ _CHR_bank_ind ], _scr_type, _view_type );
			
			if( _scr_global_ind == m_scr_list.count() )
			{
				// add image
				m_scr_list.add( bmp );
			}
			else
			{
				// insert image
				m_scr_list.add( bmp );
				
				size = m_scr_list.count();
				
				for( i = size - 1; i > _scr_global_ind; i-- )
				{
					m_scr_list.replace( i, m_scr_list.get( i - 1 ) );
				}
				
				m_scr_list.replace( _scr_global_ind, bmp );
			}			
					
		 	ListViewItem lst 	= new ListViewItem();
		 	lst.Name = lst.Text = utils.get_screen_id_str( _CHR_bank_ind, _scr_local_ind );
            lst.ImageIndex		= _scr_global_ind;
            
            int insert_ind = _all_banks_list ? _scr_global_ind:_scr_local_ind;
            
            m_listview_screens.Items.Insert( insert_ind, lst );
            
            size = m_listview_screens.Items.Count;
            
            for( i = insert_ind + 1; i < size; i++ )
            {
            	lst = m_listview_screens.Items[ i ];
            	
            	++lst.ImageIndex;
            }
            
            m_listview_screens.EndUpdate();
		}
		
		public ImageList get_blocks_image_list()
		{
			return m_imagelist_blocks;
		}
		
		public ImageList get_tiles_image_list()
		{
			return m_imagelist_tiles;
		}
		
		public i_screen_list	get_screen_list()
		{
			return m_scr_list;
		}
	}
}