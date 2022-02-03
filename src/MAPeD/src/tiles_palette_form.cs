/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 26.11.2018
 * Time: 17:38
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of tiles_palette.
	/// </summary>
	/// 
	
	public partial class tiles_palette_form : Form
	{
		public event EventHandler TilesBlocksClosed;
		public event EventHandler TileSelected;
		public event EventHandler BlockSelected;
		public event EventHandler ResetActiveTile;
		
		private readonly ImageList m_tiles_image_list 	= null;
		private readonly ImageList m_blocks_image_list	= null;
		
		private readonly ContextMenuStrip m_cm_tiles	= null;
		private readonly ContextMenuStrip m_cm_blocks	= null;
		
		private readonly tile_list	m_tile_list		= null;
		private readonly tile_list	m_block_list	= null;
		
		private int m_active_item_id = -1;
		
		public int active_item_id
		{
			get { return m_active_item_id; }
			set {}
		}
		
		public tiles_palette_form( ImageList _tiles_image_list, ContextMenuStrip _cm_tiles, ImageList _blocks_image_list, ContextMenuStrip _cm_blocks, tile_list_manager _tl_cntnr )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_tiles_image_list 	= _tiles_image_list; 
			m_blocks_image_list	= _blocks_image_list;
			
			m_cm_tiles	= _cm_tiles;
			m_cm_blocks	= _cm_blocks;
			
			m_tile_list		= new tile_list( tile_list.EType.t_Tiles, PanelPaletteTiles, m_tiles_image_list, BtnItemClick, m_cm_tiles, _tl_cntnr );
			m_block_list	= new tile_list( tile_list.EType.t_Blocks, PanelPaletteBlocks, m_blocks_image_list, BtnItemClick, m_cm_blocks, _tl_cntnr );
			
			BtnTilesClick_Event( null, null );
		}
		
		public void reset()
		{
			m_active_item_id = -1;
		}
		
		public void update()
		{
			if( Visible )
			{
				Refresh();
			}
		}
		
		void BtnTilesClick_Event(object sender, EventArgs e)
		{
			enable_tiles_panel( true );
		}
		
		void BtnBlocksClick_Event(object sender, EventArgs e)
		{
			enable_tiles_panel( false );
		}
		
		void enable_tiles_panel( bool _on )
		{
			BtnTiles.Enabled  = !_on;
			BtnBlocks.Enabled = _on;
			
			PanelPaletteTiles.Visible	= _on;
			PanelPaletteBlocks.Visible 	= !_on;
			
			if( ResetActiveTile != null )
			{
				ResetActiveTile( this, null );
			}
		}
		
		private void BtnItemClick( object sender, EventArgs e )
		{
			m_active_item_id = ( sender as tile_list ).cursor_tile_ind();
			
			if( BtnBlocks.Enabled )
			{
				if( TileSelected != null )
				{
					TileSelected( this, null );
				}
			}
			else
			{
				if( BlockSelected != null )
				{
					BlockSelected( this, null );
				}
			}
		}
		
		public void enable( bool _on )
		{
			PanelPaletteTiles.Enabled	= _on;
			PanelPaletteBlocks.Enabled	= _on;
		}

		void BtnCloseClick_Event(object sender, EventArgs e)
		{
			hide_wnd();
		}
		
		void Closing_Event(object sender, FormClosingEventArgs e)
		{
			e.Cancel = true;

			hide_wnd();
		}
		
		private void hide_wnd()
		{
			Visible = false;
			
			if( TilesBlocksClosed != null )
			{
				TilesBlocksClosed( this, null );
			}
		}
		
		public void set_screen_data_type( data_sets_manager.EScreenDataType _type )
		{
			PanelPaletteTiles.Enabled = ( _type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 );
			
			reset();
		}
	}
}