/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
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
	
	public delegate void PaletteClosed();
	public delegate void TileSelected();
	public delegate void BlockSelected();
	public delegate void ResetActiveTile();
	
	public partial class tiles_palette_form : Form
	{
		public event EventHandler PaletteClosed;
		public event EventHandler TileSelected;
		public event EventHandler BlockSelected;
		public event EventHandler ResetActiveTile;
		
		private ImageList m_tiles_image_list 	= null;
		private ImageList m_blocks_image_list	= null;
		
		private ContextMenuStrip	m_cm_tiles	= null;
		private ContextMenuStrip	m_cm_blocks	= null;
		
		private int m_active_item_id = -1;
		
		public int active_item_id
		{
			get { return m_active_item_id; }
			set {}
		}
		
		public tiles_palette_form( ImageList _tiles_image_list, ContextMenuStrip _cm_tiles, ImageList _blocks_image_list, ContextMenuStrip _cm_blocks )
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
			
			utils.fill_buttons( PanelPaletteTiles, m_tiles_image_list, BtnItemClick, m_cm_tiles );
			utils.fill_buttons( PanelPaletteBlocks, m_blocks_image_list, BtnItemClick, m_cm_blocks );

			BtnTilesClick_event( null, null );
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
		
		void BtnCloseClick_event(object sender, EventArgs e)
		{
			Visible = false;
			
			if( PaletteClosed != null )
			{
				PaletteClosed( this, null );
			}
		}
		
		void BtnTilesClick_event(object sender, EventArgs e)
		{
			enable_tiles_panel( true );
		}
		
		void BtnBlocksClick_event(object sender, EventArgs e)
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
			m_active_item_id = ( sender as Button ).ImageIndex;
			
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
	}
}