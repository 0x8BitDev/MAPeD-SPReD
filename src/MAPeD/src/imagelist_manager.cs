/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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

namespace MAPeD
{
	/// <summary>
	/// Description of imagelist_manager.
	/// </summary>
	public class imagelist_manager
	{
		private ListView	m_listview_screens	= null;
		
		private FlowLayoutPanel	m_panel_tiles	= null;
		private FlowLayoutPanel	m_panel_blocks	= null;
		
		private ImageList	m_imagelist_blocks	= null;
		private ImageList	m_imagelist_tiles	= null;
		
		private static Rectangle	m_block_rect	= new Rectangle( 0, 0, utils.CONST_BLOCKS_IMG_SIZE, utils.CONST_BLOCKS_IMG_SIZE );
		private static Rectangle	m_tile_rect		= new Rectangle( 0, 0, utils.CONST_TILES_IMG_SIZE, utils.CONST_TILES_IMG_SIZE );
		
		private static int[]	m_clrs_arr = new int[ 16 ]{ 0x00ffffff, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00ff4500, 0x00dc803c, 0x00406080, 0x00ffff00,	0x00ffa500, 0x0020b2aa, 0x0000ffff, 0x00808000,	0x00800080, 0x00c0c0c0,	0x007b68ee, 0xff1493 };
		
		private const int CONST_ALPHA = 0x60;
		
		public imagelist_manager( FlowLayoutPanel _panel_tiles, EventHandler _tiles_e, ContextMenuStrip _tiles_cm, FlowLayoutPanel _panel_blocks, EventHandler _blocks_e, ContextMenuStrip _blocks_cm, ListView _listview_screens )
		{
			m_panel_tiles		= _panel_tiles;
			m_panel_blocks		= _panel_blocks;
			
			m_listview_screens	= _listview_screens;
			
			m_imagelist_tiles 	= imagelist_init( utils.CONST_MAX_TILES_CNT, utils.CONST_TILES_IMG_SIZE );
			m_imagelist_blocks 	= imagelist_init( utils.CONST_MAX_BLOCKS_CNT, utils.CONST_BLOCKS_IMG_SIZE );
			
			utils.fill_buttons( m_panel_tiles, m_imagelist_tiles, _tiles_e, _tiles_cm, 5 );
			utils.fill_buttons( m_panel_blocks, m_imagelist_blocks, _blocks_e, _blocks_cm, 0 );
			
			listview_init_screens();
		}
		
		private void listview_init_screens()
		{
			ImageList il = new ImageList();
			il.ImageSize = new Size( utils.CONST_SCREEN_WIDTH_PIXELS, utils.CONST_SCREEN_HEIGHT_PIXELS );
			il.ColorDepth = ColorDepth.Depth24Bit;// 32bit - way too slow rendering... I don't know why...
			
			m_listview_screens.LargeImageList = il;
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
			
			Bitmap bmp = null;

			for( i = 0; i < _cnt; i++ )
			{
				bmp = new Bitmap( _size, _size, _size << 2, PixelFormat.Format32bppArgb, handle.AddrOfPinnedObject() );
				
				il.Images.Add( i.ToString(), bmp );				
			}
			
			handle.Free();
			
			return il;
		}
		
		public void update_tiles( int _view_type, tiles_data _tiles_data )
		{
			Image 		img;
			Graphics	gfx;
			
			for( int i = 0; i < utils.CONST_MAX_TILES_CNT; i++ )
			{
				img = m_imagelist_tiles.Images[ i ];
				
				gfx = Graphics.FromImage( img );
				
				if( _tiles_data != null )
				{
					update_tile( i, _view_type, _tiles_data, gfx, img );
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

		public void update_tile( int _tile_ind, int _view_type, tiles_data _tiles_data, Graphics _gfx, Image _img )
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
			gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;

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
			
			if( _view_type == 2 ) // tile id
			{
				draw_tile_info( String.Format( "{0:X2}", _tile_ind ), gfx );
			}
			else
			if( _view_type == 3 ) // usage
			{
				draw_tile_info( String.Format( "{0}", _tiles_data.get_tile_usage( ( byte )_tile_ind ) ), gfx );
			}
			
			if( _img == null )
			{
				m_imagelist_tiles.Images[ _tile_ind ] = img;
			}
			
			if( _gfx == null )
			{
				gfx.Dispose();
				
				( m_panel_tiles.Controls[ _tile_ind ] as Button ).Refresh();
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
		
		public void update_blocks( int _view_type, tiles_data _tiles_data, bool _prop_per_block )
		{
			Image 		img;
			Graphics	gfx;
			
			int 		obj_id;

			m_block_rect.Width = m_block_rect.Height = ( _view_type == 1 && !_prop_per_block ) ? ( utils.CONST_BLOCKS_IMG_SIZE >> 1 ):utils.CONST_BLOCKS_IMG_SIZE;
			
			for( int i = 0; i < utils.CONST_MAX_BLOCKS_CNT; i++ )
			{
				img = m_imagelist_blocks.Images[ i ];
				
				gfx = Graphics.FromImage( img );
				
				if( _tiles_data != null )
				{
					gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
					gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
					
					utils.update_block_gfx( i, _tiles_data, gfx, img.Width >> 1, img.Height >> 1 );
					
					if( _view_type == 1 ) // obj id
					{
						if( _prop_per_block )
						{
							obj_id = tiles_data.get_block_flags_obj_id( _tiles_data.blocks[ i << 2 ] );
							
							utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | m_clrs_arr[ obj_id ] );
							
							draw_block_info( String.Format( "{0}", obj_id ), gfx );
						}
						else
						{
							for( int chr_n = 0; chr_n < utils.CONST_BLOCK_SIZE; chr_n++ )
							{
								obj_id = tiles_data.get_block_flags_obj_id( _tiles_data.blocks[ ( i << 2 ) + chr_n ] );
								
								utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | m_clrs_arr[ obj_id ] );
								
								m_block_rect.X 	= ( ( chr_n&0x01 ) == 0x01 ) ? m_block_rect.Width:0;
								m_block_rect.Y 	= ( ( chr_n&0x02 ) == 0x02 ) ? m_block_rect.Height:0;
								
								gfx.FillRectangle( utils.brush, m_block_rect );
							}
						}
					}
					else
					if( _view_type == 4 ) // usage
					{
						utils.brush.Color = Color.FromArgb( ( CONST_ALPHA << 24 ) | 0x00ffffff );
						
						draw_block_info( String.Format( "{0}", _tiles_data.get_block_usage( ( byte )i ) ), gfx );
					}
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

		private void draw_block_info( string _info, Graphics _gfx )
		{
			m_block_rect.X = m_block_rect.Y = 0;
			_gfx.FillRectangle( utils.brush, m_block_rect );
			
			utils.brush.Color = Color.FromArgb( unchecked( (int)0xff000000 ) );
			_gfx.DrawString( _info, utils.fnt10_Arial, utils.brush, 2, 2 );
			
			utils.brush.Color = Color.FromArgb( unchecked( (int)0xffffffff ) );
			_gfx.DrawString( _info, utils.fnt10_Arial, utils.brush, 1, 1 );
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
	
					screens_cnt = data.scr_data.Count;
	
					if( _bank_id == bank_n )
					{
						for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
						{
							m_listview_screens.LargeImageList.Images.Add( ( Image )m_listview_screens.LargeImageList.Images[ screens_ind + screen_n ].Clone() );
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
		
		public void update_all_screens( List< tiles_data > _tiles_data )
		{
			m_listview_screens.BeginUpdate();
			
			// clear items
			{
				int size = m_listview_screens.Items.Count;
				
				for( int i = 0; i < size; i++ )
				{
					m_listview_screens.LargeImageList.Images[ i ].Dispose();
				}
				
				m_listview_screens.LargeImageList.Images.Clear();
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
				
				screens_cnt = data.scr_data.Count;
				
				for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
				{
					m_listview_screens.LargeImageList.Images.Add( create_screen_image( screen_n, data ) );
					
				 	lst 				= new ListViewItem();
				 	lst.Name = lst.Text = utils.get_screen_id_str( bank_n, screen_n );
					lst.ImageIndex		= img_ind;
 
					++img_ind;
 
					m_listview_screens.Items.Add( lst );
				}
			}
			
			m_listview_screens.EndUpdate();
		}

		public void update_screens( List< tiles_data > _tiles_data, bool _update_images, int _bank_id = -1 )
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

				screens_cnt = data.scr_data.Count;

				if( ( _bank_id >= 0 && _bank_id == bank_n ) || _bank_id < 0 )
				{
					palette_group.Instance.set_palette( data );
					
					for( int screen_n = 0; screen_n < screens_cnt; screen_n++ )
					{
						img_ind = screens_ind + screen_n;
						
						if( _update_images )
						{
							m_listview_screens.LargeImageList.Images[ img_ind ].Dispose();
							m_listview_screens.LargeImageList.Images[ img_ind ] = create_screen_image( screen_n, data );
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
		
		private Bitmap create_screen_image( int _screen_n, tiles_data _data )
		{
			int tile_id;
			int tile_offs_x;
			int tile_offs_y;
			
			ushort[] 	blocks_arr 	= _data.blocks;
			byte[] 		CHR_data 	= _data.CHR_bank;
			
			palette_group.Instance.set_palette( _data );

			Bitmap bmp = new Bitmap( utils.CONST_SCREEN_WIDTH_PIXELS, utils.CONST_SCREEN_HEIGHT_PIXELS, PixelFormat.Format32bppArgb );
			
			Graphics gfx = Graphics.FromImage( bmp );
			
			gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			
			gfx.Clear( utils.CONST_COLOR_SCREEN_CLEAR );
			
			for( int tile_n = 0; tile_n < utils.CONST_SCREEN_TILES_CNT; tile_n++ )
			{
				tile_id = _data.scr_data[ _screen_n ][ tile_n ];
				
				tile_offs_x = ( tile_n % utils.CONST_SCREEN_NUM_WIDTH_TILES ) << 5;
				tile_offs_y = ( tile_n / utils.CONST_SCREEN_NUM_WIDTH_TILES ) << 5;
				
				for( int block_n = 0; block_n < utils.CONST_BLOCK_SIZE; block_n++ )
				{
					utils.update_block_gfx( _data.get_tile_block( tile_id, block_n ), _data, gfx, 8, 8, ( ( block_n % 2 ) << 4 ) + tile_offs_x, ( ( block_n >> 1 ) << 4 ) + tile_offs_y );
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
					screens_cnt = data.scr_data.Count;
					
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
					Image img = m_listview_screens.LargeImageList.Images[ item_n ];
					img.Dispose();
					
					int remove_img_ind = m_listview_screens.Items[ item_n ].ImageIndex;
					
					for( int i = 0; i < m_listview_screens.Items.Count; i++ )
					{
						if( m_listview_screens.Items[ i ].ImageIndex > remove_img_ind )
						{
							--m_listview_screens.Items[ i ].ImageIndex;
						}
					}

					m_listview_screens.LargeImageList.Images.RemoveAt( remove_img_ind );
					m_listview_screens.Items.RemoveAt( item_n );
					
					res = true;
					
					break;
				}
			}
	
			return res;
		}
		
		public void insert_screen( bool _all_banks_list, int _CHR_bank_ind, int _scr_local_ind, int _scr_global_ind, List< tiles_data > _tiles_data )
		{
			int i;
			int size;
			
			m_listview_screens.BeginUpdate();

			Bitmap bmp = create_screen_image( _scr_local_ind, _tiles_data[ _CHR_bank_ind ] );
			
			if( _scr_global_ind == m_listview_screens.LargeImageList.Images.Count )
			{
				// add image
				m_listview_screens.LargeImageList.Images.Add( bmp );
			}
			else
			{
				// insert image
				m_listview_screens.LargeImageList.Images.Add( bmp );
				
				size = m_listview_screens.LargeImageList.Images.Count;
				
				for( i = size - 1; i > _scr_global_ind; i-- )
				{
					m_listview_screens.LargeImageList.Images[ i ] = m_listview_screens.LargeImageList.Images[ i - 1 ];
				}
				
				m_listview_screens.LargeImageList.Images[ _scr_global_ind ] = bmp;
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
	}
}