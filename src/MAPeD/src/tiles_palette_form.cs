/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 26.11.2018
 * Time: 17:38
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of tiles_palette.
	/// </summary>
	/// 
	
	public partial class tiles_palette_form : Form
	{
		public event EventHandler FormHided;
		public event EventHandler TileSelected;
		public event EventHandler BlockSelected;
		public event EventHandler ResetActiveTile;
		
		private readonly List< Bitmap >	m_tiles_image_list;
		private readonly List< Bitmap >	m_blocks_image_list;
		
		private readonly ContextMenuStrip m_cm_tiles;
		private readonly ContextMenuStrip m_cm_blocks;
		
		private readonly tile_list	m_tile_list;
		private readonly tile_list	m_block_list;
		
		private int m_active_item_id = -1;
		
		public int active_item_id
		{
			get { return m_active_item_id; }
			set {}
		}
		
		public tiles_palette_form( List< Bitmap > _tiles_image_list, ContextMenuStrip _cm_tiles, List< Bitmap > _blocks_image_list, ContextMenuStrip _cm_blocks, tile_list_manager _tl_cntnr )
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
			
			m_tile_list		= new tile_list( tile_list.e_data_type.Tiles, PanelPaletteTiles, m_tiles_image_list, BtnItemClick, m_cm_tiles, _tl_cntnr );
			m_block_list	= new tile_list( tile_list.e_data_type.Blocks, PanelPaletteBlocks, m_blocks_image_list, BtnItemClick, m_cm_blocks, _tl_cntnr );
			
			BtnTilesClick( null, null );
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
		
		private void BtnTilesClick( object sender, EventArgs e )
		{
			enable_tiles_panel( true );
		}
		
		private void BtnBlocksClick( object sender, EventArgs e )
		{
			enable_tiles_panel( false );
		}
		
		private void enable_tiles_panel( bool _on )
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

		private void BtnCloseClick( object sender, EventArgs e )
		{
			hide_window();
		}
		
		private void OnClosing( object sender, FormClosingEventArgs e )
		{
			e.Cancel = true;

			hide_window();
		}
		
		private void hide_window()
		{
			Visible = false;
			
			if( FormHided != null )
			{
				FormHided( this, null );
			}
		}
		
		public void show_window()
		{
			Visible = true;

			this.WindowState = FormWindowState.Normal;
		}
		
		public void set_screen_data_type( data_sets_manager.e_screen_data_type _type )
		{
			PanelPaletteTiles.Enabled = ( _type == data_sets_manager.e_screen_data_type.Tiles4x4 );
			
			reset();
		}
	}
}