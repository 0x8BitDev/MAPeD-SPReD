/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 04.05.2017
 * Time: 13:12
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of block_editor.
	/// </summary>
	/// 
	
	public class block_editor : drawable_base
	{
		public event EventHandler PixelChanged;
		public event EventHandler DataChanged;
		public event EventHandler NeedGFXUpdate;
		public event EventHandler BlockQuadSelected;
#if DEF_PALETTE16_PER_CHR		
		public event EventHandler UpdatePaletteListPos;
#endif		
		public enum EMode
		{
			bem_CHR_select,
			bem_draw,
		}
		
		private EMode m_edit_mode	= EMode.bem_CHR_select;
		
		public EMode edit_mode
		{
			get { return m_edit_mode; }
			set 
			{
				string mode = "";
				
				m_edit_mode = value;

				switch( m_edit_mode )
				{
					case EMode.bem_CHR_select:	{ mode = "Select CHRs"; } break;
					case EMode.bem_draw:		{ mode = "Draw"; } break;
				}
				
				MainForm.set_status_msg( "Block Editor mode: " + mode );
				
				update();
			}
		}
#if DEF_ZX
		private utils.ETileViewType	m_view_type		= utils.ETileViewType.tvt_Unknown;
		
		public utils.ETileViewType view_type
		{
			get { return m_view_type; }
			set { m_view_type = value; update(); }
		}
#endif
		private tiles_data	m_data	= null;

		private int m_sel_quad_ind	= -1;
		private int m_sel_block_id	= -1;
		
		private bool m_drawing_state	= false;

		private int m_CHR_pix_offset = -1;
		
		private static readonly byte[] clr_ind_remap_arr = new byte[]
		{
			0,1,2,3,
			0,3,1,2,
			0,2,3,1,
			0,1,3,2,
			0,3,2,1,
			0,2,1,3,
		};

		private static readonly byte[] clr_ind_remap_transp_arr = new byte[]
		{
			0,1,2,3,
			1,2,3,0,
			2,3,0,1,
			3,0,1,2,
		};
		
		private static int clr_ind_remap_arr_pos = 0;
		
		private readonly List< int > m_CHR_ids = null;		
		
		private bool m_palette_per_CHR_mode	= false;
		
		public bool palette_per_CHR_mode
		{
			get { return m_palette_per_CHR_mode; }
			set { m_palette_per_CHR_mode = value; }
		}
		
		public block_editor( PictureBox _pbox ) : base( _pbox )
		{
			m_pix_box.MouseDown 	+= new MouseEventHandler( this.BlockEditor_MouseDown );
			m_pix_box.MouseUp 		+= new MouseEventHandler( this.BlockEditor_MouseUp );
			m_pix_box.MouseMove		+= new MouseEventHandler( this.BlockEditor_MouseMove );
			
			m_pix_box.MouseClick	+= new MouseEventHandler( this.BlockEditor_MouseClick );
			
			m_CHR_ids = new List< int >( 4 );
			
			update();
		}
		
		private void BlockEditor_MouseDown(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left && edit_mode == EMode.bem_draw )
			{
				m_drawing_state = true;
				
				sel_quad_and_draw( e.X, e.Y, true );
			}
		}
		
		private void BlockEditor_MouseUp(object sender, MouseEventArgs e)
		{
			if( e.Button == MouseButtons.Left )
			{
				m_drawing_state = false;
				
				m_CHR_pix_offset = -1;
			}
		}

		private void BlockEditor_MouseMove(object sender, MouseEventArgs e)
		{
			if( m_drawing_state )
			{
				sel_quad_and_draw( e.X, e.Y, true );
			}
		}
		
		private void BlockEditor_MouseClick(object sender, MouseEventArgs e)
		{
			sel_quad_and_draw( e.X, e.Y, false );
		}

		private void sel_quad_and_draw( int _x, int _y, bool _need_draw )
		{
			int last_sel_quad_ind = m_sel_quad_ind;
			
			m_sel_quad_ind = ( _x >> 7 ) + ( ( _y >> 7 ) << 1 );

			if( palette_per_CHR_mode )
			{
				if( last_sel_quad_ind != m_sel_quad_ind )
				{
					set_active_palette();
				}
			}

			dispatch_event_quad_selected();
			
			if( _need_draw && m_data != null && palette_group.Instance.active_palette != -1 )
			{
				int x = _x >> 4;
				int y = _y >> 4;
				
				int local_x = ( m_sel_quad_ind == 1 || m_sel_quad_ind == 3 ) ? ( x - 8 ):x;
				int local_y = ( m_sel_quad_ind == 2 || m_sel_quad_ind == 3 ) ? ( y - 8 ):y;
				
				if( local_x >= 0 && local_y >= 0 && local_x < utils.CONST_SPR8x8_SIDE_PIXELS_CNT && local_y < utils.CONST_SPR8x8_SIDE_PIXELS_CNT )
				{
					uint block_data = m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ];
					                               
					int chr_id	= tiles_data.get_block_CHR_id( block_data );
					
					int chr_x = chr_id % 16;
					int chr_y = chr_id >> 4;
					
					palette_group plt = palette_group.Instance;					
#if DEF_NES
					byte clr_slot = (byte)plt.get_palettes_arr()[ plt.active_palette ].color_slot;
#else
					byte clr_slot = (byte)( ( plt.active_palette * utils.CONST_PALETTE_SMALL_NUM_COLORS ) + plt.get_palettes_arr()[ plt.active_palette ].color_slot );
#endif

#if DEF_ZX
					// paper color
					byte paper_clr_slot = (byte)( ( plt.paper_active_palette * utils.CONST_PALETTE_SMALL_NUM_COLORS ) + plt.get_palettes_arr()[ plt.paper_active_palette ].color_slot );
					
					// apply paper/ink color
					m_data.update_ink_paper_colors( get_selected_quad_CHR_id(), clr_slot, paper_clr_slot );
					
					if( m_data.CHR_bank[ ( ( chr_x << 3 ) + local_x ) + ( ( ( chr_y * utils.CONST_CHR_BANK_PAGE_SIDE ) << 3 ) + local_y * utils.CONST_CHR_BANK_PAGE_SIDE ) ] == clr_slot )
					{
						clr_slot = paper_clr_slot;
					}
#endif

#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
					byte flip_flags = tiles_data.get_block_flags_flip( block_data );

					if( ( flip_flags & utils.CONST_CHR_ATTR_FLAG_HFLIP ) == utils.CONST_CHR_ATTR_FLAG_HFLIP )
					{
						local_x = utils.CONST_SPR8x8_SIDE_PIXELS_CNT - local_x - 1;
					}
					
					if( ( flip_flags & utils.CONST_CHR_ATTR_FLAG_VFLIP ) == utils.CONST_CHR_ATTR_FLAG_VFLIP )
					{
						local_y = utils.CONST_SPR8x8_SIDE_PIXELS_CNT - local_y - 1;
					}
#endif
					int pix_offset = ( ( chr_x << 3 ) + local_x ) + ( ( ( chr_y * utils.CONST_CHR_BANK_PAGE_SIDE ) << 3 ) + local_y * utils.CONST_CHR_BANK_PAGE_SIDE );
					
					if( pix_offset == m_CHR_pix_offset )
					{
						return;
					}
					else
					{
						m_data.CHR_bank[ pix_offset ] = clr_slot;
						
						m_CHR_pix_offset = pix_offset;
					}
					
					dispatch_event_pixel_changed();
					dispatch_event_data_changed();
					dispatch_event_need_gfx_update();
				}
			}

#if DEF_PALETTE16_PER_CHR
			if( _need_draw )
			{
				update_sel_CHR_palette();
			}
			else
			{
#if DEF_ZX
				set_active_palette();
#else
				update_palette_list_pos();
#endif
			}
#endif			
			update();
			
			update_status_bar();
		}
		
		private void set_active_palette()
		{
			if( m_sel_quad_ind >= 0 && m_sel_block_id >= 0 )
			{
#if DEF_NES
				palette_group.Instance.active_palette = tiles_data.get_block_flags_palette( m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ] );
#elif DEF_PALETTE16_PER_CHR
				update_palette_list_pos();
#endif

#if DEF_ZX
				byte ink_clr	= 0xff;
				byte paper_clr	= 0xff;
				
				m_data.get_ink_paper_colors( get_selected_quad_CHR_id(), ref ink_clr, ref paper_clr );
				
				palette_group plt = palette_group.Instance;
				
				if( ink_clr != 0xff )
				{
					plt.active_palette = ink_clr >> 2;
					plt.get_palettes_arr()[ plt.active_palette ].color_slot = ink_clr & 0x03; 
				}
				
				if( paper_clr != 0xff )
				{
					plt.active_palette = paper_clr >> 2;
					plt.get_palettes_arr()[ plt.paper_active_palette ].color_slot = paper_clr & 0x03; 
				}
#endif
			}
			else
			{
				palette_group.Instance.active_palette = -1;
			}
		}

#if DEF_PALETTE16_PER_CHR
		private void update_palette_list_pos()
		{
			if( m_data != null && m_sel_quad_ind >= 0 && m_sel_block_id >= 0 )
			{
				m_data.palette_pos = tiles_data.get_block_flags_palette( m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ] );
				
				if( UpdatePaletteListPos != null )
				{
					UpdatePaletteListPos( this, null );
				}
			}
		}
		
		public void update_sel_CHR_palette()
		{
			if( m_data != null && m_sel_quad_ind >= 0 && m_sel_block_id >= 0 )
			{
				int block_data_ind = ( m_sel_block_id << 2 ) + m_sel_quad_ind;
				
				m_data.blocks[ block_data_ind ] = tiles_data.set_block_flags_palette( m_data.palette_pos, m_data.blocks[ block_data_ind ] );
			}
		}
#endif
		public void subscribe_event( CHR_bank_viewer _chr_bank )
		{
			_chr_bank.DataChanged += new EventHandler( update_data );
			_chr_bank.CHRSelected += new EventHandler( CHR_selected );
		}

		public void subscribe_event( tiles_processor _tiles_proc )
		{
			_tiles_proc.GFXUpdate += new EventHandler( update_gfx );
		}
		
		private void update_gfx( object sender, EventArgs e )
		{
			if( m_sel_quad_ind >= 0 )
			{
				// CHR indices may be changed, so we try to update them
				dispatch_event_quad_selected();
			}
			
#if DEF_PALETTE16_PER_CHR
			update_palette_list_pos();
#endif			
			update();
		}
		
		public void subscribe_event( data_sets_manager _data_mngr )
		{
			_data_mngr.SetTilesData += new EventHandler( new_data_set );
		}
		
		public void subscribe_event( tile_editor _tile_editor )
		{
			_tile_editor.UpdateSelectedBlock += new EventHandler( update_block );
		}
		
		private void update_block( object sender, EventArgs e )
		{
			EventArg2Params args = ( e as EventArg2Params );
			
			int 		block_id 	= Convert.ToInt32( args.param1 );
			tiles_data 	data 		= args.param2 as tiles_data;
			
			set_selected_block( block_id, data );
		}
		
		private void new_data_set( object sender, EventArgs e )
		{
			tiles_data data = null;
			
			set_selected_block( 0, data );
		}
		
		private void update_data( object sender, EventArgs e )
		{
			if( m_sel_block_id >= 0 && palette_group.Instance.active_palette >= 0 )
			{
#if DEF_NES		
				int chr_data_ind;
				
				if( palette_per_CHR_mode )
				{
					// palette per CHR8x8
					chr_data_ind = ( m_sel_block_id << 2 ) + m_sel_quad_ind;
					
					m_data.blocks[ chr_data_ind ] = tiles_data.set_block_flags_palette( palette_group.Instance.active_palette, m_data.blocks[ chr_data_ind ] );
				}
				else
				{
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						chr_data_ind = ( m_sel_block_id << 2 ) + i;
						
						m_data.blocks[ chr_data_ind ] = tiles_data.set_block_flags_palette( palette_group.Instance.active_palette, m_data.blocks[ chr_data_ind ] );
					}
				}
#elif DEF_ZX
				palette_group plt = palette_group.Instance;
				
				byte ink_clr_slot	= ( byte )( ( plt.active_palette * utils.CONST_PALETTE_SMALL_NUM_COLORS ) + plt.get_palettes_arr()[ plt.active_palette ].color_slot );
				byte paper_clr_slot	= ( byte )( ( plt.paper_active_palette * utils.CONST_PALETTE_SMALL_NUM_COLORS ) + plt.get_palettes_arr()[ plt.paper_active_palette ].color_slot );

				// apply paper/ink color
				m_data.update_ink_paper_colors( get_selected_quad_CHR_id(), ink_clr_slot, paper_clr_slot );
#endif
				dispatch_event_data_changed();
				dispatch_event_need_gfx_update();
			}
			
			update();
		}

		private void CHR_selected( object sender, EventArgs e )
		{
			if( edit_mode == EMode.bem_CHR_select && m_sel_block_id >= 0 )
			{
				int selected_CHR = ( sender as CHR_bank_viewer ).get_selected_CHR_ind();

				int chr_data_ind = ( m_sel_block_id << 2 ) + m_sel_quad_ind;
				
				m_data.blocks[ chr_data_ind ] = tiles_data.set_block_CHR_id( selected_CHR, m_data.blocks[ chr_data_ind ] );

				dispatch_event_data_changed();
				dispatch_event_need_gfx_update();
				
				dispatch_event_quad_selected();
#if DEF_ZX
				set_active_palette();
#endif
				update();
				
				update_status_bar();
			}
		}
		
		public int get_selected_quad_CHR_id()
		{
			if( m_sel_quad_ind >= 0 && m_sel_block_id >= 0 )
			{
				return tiles_data.get_block_CHR_id( m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ] );
			}
			
			return -1;
		}

		private void update_changes()
		{
			dispatch_event_pixel_changed();
			dispatch_event_need_gfx_update();
			dispatch_event_data_changed();
			
			update();
		}
		
		public override void update()
		{
			clear_background( CONST_BACKGROUND_COLOR );
			
			if( m_sel_block_id >= 0 )
			{
				// draw images...
				{
#if DEF_ZX
					utils.update_block_gfx( m_sel_block_id, m_data, m_gfx, m_pix_box.Width >> 1, m_pix_box.Height >> 1, 0, 0, utils.get_draw_block_flags_by_view_type( view_type ) );
#else
					utils.update_block_gfx( m_sel_block_id, m_data, m_gfx, m_pix_box.Width >> 1, m_pix_box.Height >> 1 );
#endif
				}			
			
				// draw grid
				{
					m_pen.Color = utils.CONST_COLOR_BLOCK_EDITOR_GRID;
					
					int n_lines 	= m_pix_box.Width >> 4;
					int pos;
					
					for( int i = 0; i < n_lines; i++ )
					{
						pos = i << 4;
						
						m_gfx.DrawLine( m_pen, pos, 0, pos, m_pix_box.Width );
						m_gfx.DrawLine( m_pen, 0, pos, m_pix_box.Height, pos );
					}
					
					if( m_edit_mode == EMode.bem_CHR_select )
					{
						m_pen.Color = utils.CONST_COLOR_BLOCK_EDITOR_CHR_BORDER;
					}
					else
					{
						m_pen.Color = utils.CONST_COLOR_BLOCK_EDITOR_DRAW_MODE_CHR_SPLITTER;
					}
					
					pos = 8 << 4;
					m_gfx.DrawLine( m_pen, pos, 0, pos, m_pix_box.Width );
					m_gfx.DrawLine( m_pen, 0, pos, m_pix_box.Height, pos );			
				}
				
				draw_border( Color.Black );
				
				if( ( m_edit_mode == EMode.bem_CHR_select ) && ( m_sel_quad_ind >= 0 ) )
				{
					int x = ( ( m_sel_quad_ind % 2 ) << 7 );
					int y = ( ( m_sel_quad_ind >> 1 ) << 7 );
					
					int quad_width = m_pix_box.Width >> 1;

					m_pen.Color = utils.CONST_COLOR_BLOCK_EDITOR_SELECTED_CHR_INNER_BORDER;
					m_gfx.DrawRectangle( m_pen, x+2, y+2, quad_width - 3, quad_width - 3 );
					
					m_pen.Color = utils.CONST_COLOR_BLOCK_EDITOR_SELECTED_CHR_OUTER_BORDER;					
					m_gfx.DrawRectangle( m_pen, x+1, y+1, quad_width - 1, quad_width - 1 );
				}
				
				disable( false );
			}
			else
			{
				disable( true );
			}
			
			invalidate();
		}
		
		public void set_selected_block( int _block_id, tiles_data _data )
		{
			if( _data != null )
			{
				m_sel_block_id = _block_id;
				
				m_data	= _data;
				
				m_sel_quad_ind	= 0;
				
				dispatch_event_data_changed();
				
				update_status_bar();				
			}
			else
			{
				m_sel_block_id = -1;

				m_data	= null;				
			}

			set_active_palette();

			dispatch_event_quad_selected();
			
			update();
		}
		
		public int get_selected_block_id()
		{
			return m_sel_block_id;
		}
		
		public ulong get_selected_block_CHRs()
		{
			if( m_sel_block_id >= 0 && m_data != null )
			{
				int block_data_offs = m_sel_block_id << 2;

				return 	(ulong)( 	(ulong)tiles_data.get_block_CHR_id( m_data.blocks[ block_data_offs ] ) ) |
								( ( (ulong)tiles_data.get_block_CHR_id( m_data.blocks[ block_data_offs + 1 ] ) ) << 16 ) |
								( ( (ulong)tiles_data.get_block_CHR_id( m_data.blocks[ block_data_offs + 2 ] ) ) << 32 ) |
								( ( (ulong)tiles_data.get_block_CHR_id( m_data.blocks[ block_data_offs + 3 ] ) ) << 48 );
			}
			
			return 0;
		}
			
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
		public void set_CHR_flag_vflip()
		{
			set_flip_flag( utils.CONST_CHR_ATTR_FLAG_VFLIP, m_sel_block_id, m_sel_quad_ind );
		}

		public void set_CHR_flag_hflip()
		{
			set_flip_flag( utils.CONST_CHR_ATTR_FLAG_HFLIP, m_sel_block_id, m_sel_quad_ind );
		}
		
		private void set_flip_flag( byte _flip_flag, int _block_id, int _quad_id )
		{
			if( _block_id >= 0 )
			{
				int chr_data_ind 	= ( _block_id << 2 ) + _quad_id;
				uint chr_data 		= m_data.blocks[ chr_data_ind ];
				
				m_data.blocks[ chr_data_ind ] = tiles_data.set_block_flags_flip( (byte)( tiles_data.get_block_flags_flip( chr_data ) ^ _flip_flag ), chr_data );
			
				update();
			}
		}
#endif
		public void set_block_flags_obj_id( int _id, bool _per_block )
		{
			if( m_sel_block_id >= 0 )
			{
				int chr_data_ind;
				
				if( _per_block )
				{
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						chr_data_ind = ( m_sel_block_id << 2 ) + i;
					
						m_data.blocks[ chr_data_ind ] = tiles_data.set_block_flags_obj_id( _id, m_data.blocks[ chr_data_ind ] );
					}
				}
				else
				{
					chr_data_ind = ( m_sel_block_id << 2 ) + m_sel_quad_ind;
				
					m_data.blocks[ chr_data_ind ] = tiles_data.set_block_flags_obj_id( _id, m_data.blocks[ chr_data_ind ] );
				}
			}
		}
		
		public int get_block_flags_obj_id()
		{
			if( m_data != null && m_sel_block_id >= 0 )
			{
				return tiles_data.get_block_flags_obj_id( m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ] );
			}
			
			return 0;
		}

		private void dispatch_event_pixel_changed()
		{
			if( PixelChanged != null )
			{
				PixelChanged( this, null );
			}
		}
		
		private void dispatch_event_data_changed()
		{
			if( DataChanged != null )
			{
				DataChanged( this, null );
			}
		}
		
		private void dispatch_event_need_gfx_update()
		{
			if( NeedGFXUpdate != null )
			{
				NeedGFXUpdate( this, null );
			}
		}

		private void dispatch_event_quad_selected()		
		{
			if( BlockQuadSelected != null )
			{
				BlockQuadSelected( this, null );
			}
		}
		
		private void update_status_bar()
		{
			if( m_data != null && m_sel_quad_ind >= 0 && m_sel_block_id >= 0 )
			{
				MainForm.set_status_msg( String.Format( "Block Editor: Block: #{0:X2} \\ CHR: #{1:X2}", m_sel_block_id, tiles_data.get_block_CHR_id( m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ] ) ) );
			}
		}

		public void transform( utils.ETransformType _type )
		{
			if( m_sel_block_id >= 0 )
			{
				switch( _type )
				{
					case utils.ETransformType.tt_vflip: 	
						{ 
							int[] vflip_remap = { 2, 3, 0, 1 };
							
							block_transform( _type, vflip_remap );
						} 	
						break;
						
					case utils.ETransformType.tt_hflip: 	
						{ 
							int[] hflip_remap = { 1, 0, 3, 2 };
							
							block_transform( _type, hflip_remap ); 
						} 	
						break;
						
					case utils.ETransformType.tt_rotate:	
						{ 
							int[] rotate_remap = { 2, 0, 3, 1 };
							
							block_transform( _type, rotate_remap ); 
						}	
						break;
				}
			}
		}
		
		private void block_transform( utils.ETransformType _type, int[] _remap_arr )
		{
			int i;
			
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
			if( m_edit_mode	== EMode.bem_CHR_select )
			{
				switch( _type )
				{
					case utils.ETransformType.tt_vflip: 	{ set_flip_flag( utils.CONST_CHR_ATTR_FLAG_VFLIP, m_sel_block_id, m_sel_quad_ind );	} 	dispatch_event_need_gfx_update(); return;
					case utils.ETransformType.tt_hflip: 	{ set_flip_flag( utils.CONST_CHR_ATTR_FLAG_HFLIP, m_sel_block_id, m_sel_quad_ind );	} 	dispatch_event_need_gfx_update(); return;
				}
			}
			else
			{
				for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					switch( _type )
					{
						case utils.ETransformType.tt_vflip: 	{ set_flip_flag( utils.CONST_CHR_ATTR_FLAG_VFLIP, m_sel_block_id, i );	} 	break;
						case utils.ETransformType.tt_hflip: 	{ set_flip_flag( utils.CONST_CHR_ATTR_FLAG_HFLIP, m_sel_block_id, i );	} 	break;
					}
				}
			}
#endif
			
			uint[] blocks_data = { 0, 0, 0, 0 };
			
			for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
			{
				blocks_data[ i ] = m_data.blocks[ ( m_sel_block_id << 2 ) + i ];
				
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS
				if( _type == utils.ETransformType.tt_rotate && tiles_data.get_block_flags_flip( blocks_data[ i ] ) != 0 )
				{
					MainForm.message_box( "You can't rotate flipped CHRs!", "Block Transformation Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
#endif
			}
			
#if DEF_FLIP_BLOCKS_SPR_BY_FLAGS			
			bool flip_by_flags = true;
#else
			bool flip_by_flags = false;
#endif //DEF_FLIP_BLOCKS_SPR_BY_FLAGS

			if( flip_by_flags == false || _type == utils.ETransformType.tt_rotate )
			{
				for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					switch( _type )
					{
						case utils.ETransformType.tt_vflip: 	{ tiles_data.CHR_bank_vflip( m_data.CHR_bank, tiles_data.get_block_CHR_id( blocks_data[ i ] ) );	} 		break;
						case utils.ETransformType.tt_hflip: 	{ tiles_data.CHR_bank_hflip( m_data.CHR_bank, tiles_data.get_block_CHR_id( blocks_data[ i ] ) );	} 		break;
						case utils.ETransformType.tt_rotate: 	{ tiles_data.CHR_bank_rotate_cw( m_data.CHR_bank, tiles_data.get_block_CHR_id( blocks_data[ i ] ) );	} 	break;
					}
				}
				
				for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
				{
					blocks_data[ i ] = m_data.blocks[ ( m_sel_block_id << 2 ) + i ];
				}
			}

			for( i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
			{
				m_data.blocks[ ( m_sel_block_id << 2 ) + i ] = blocks_data[ _remap_arr[ i ] ];
			}
			
			update_changes();			
		}
		
		public void shift_colors( bool _shift_transp )
		{
			if( m_data != null )
			{
				m_CHR_ids.Clear();
					
				if( edit_mode == EMode.bem_CHR_select )
				{
					m_CHR_ids.Add( tiles_data.get_block_CHR_id( m_data.blocks[ ( m_sel_block_id << 2 ) + m_sel_quad_ind ] ) );
				}
				else
				{
					int CHR_id;
					
					for( int quad_n = 0; quad_n < utils.CONST_BLOCK_SIZE; quad_n++ )
					{
						CHR_id = tiles_data.get_block_CHR_id( m_data.blocks[ ( m_sel_block_id << 2 ) + quad_n ] );
						
						if( !m_CHR_ids.Contains( CHR_id ) )
						{
							m_CHR_ids.Add( CHR_id );
						}
					}
				}
				
				int CHR_n;
				int pix_n;
				int size = m_CHR_ids.Count;
				
				byte clr_ind;
	
				++clr_ind_remap_arr_pos;
				clr_ind_remap_arr_pos %= _shift_transp ? ( clr_ind_remap_transp_arr.Length >> 2 ):( clr_ind_remap_arr.Length >> 2 );
				
				byte[] remap_arr = _shift_transp ? clr_ind_remap_transp_arr:clr_ind_remap_arr;
				
				int start_clr_ind = clr_ind_remap_arr_pos << 2;
				
				for( CHR_n = 0; CHR_n < size; CHR_n++ )
				{
					m_data.from_CHR_bank_to_spr8x8( m_CHR_ids[ CHR_n ], utils.tmp_spr8x8_buff, 0 );
					
					for( pix_n = 0; pix_n < utils.CONST_SPR8x8_TOTAL_PIXELS_CNT; pix_n++ )
					{
						clr_ind = utils.tmp_spr8x8_buff[ pix_n ];
						
						clr_ind = remap_arr[ start_clr_ind + clr_ind ];
							
						utils.tmp_spr8x8_buff[ pix_n ] = clr_ind;
					}
					
					m_data.from_spr8x8_to_CHR_bank( m_CHR_ids[ CHR_n ], utils.tmp_spr8x8_buff );
				}
				
				dispatch_event_pixel_changed();
				dispatch_event_data_changed();
				dispatch_event_need_gfx_update();
				
				update();
			}
		}
#if DEF_ZX
		public void zx_swap_ink_paper( bool _inv_ink_paper )
		{
			if( m_sel_quad_ind >= 0 && m_sel_block_id >= 0 )
			{
				Action< int > swap_ink_paper = chr_id => 
				{
					byte ink_clr	= 0xff;
					byte paper_clr	= 0xff;

					m_data.get_ink_paper_colors( chr_id, ref ink_clr, ref paper_clr );
					
					if( paper_clr != 0xff )
					{
						paper_clr &= 0x07;
					}

					if( ink_clr != 0xff )
					{
						ink_clr |= 0x08;
					}
					
					if( _inv_ink_paper )
					{
						m_data.update_ink_paper_colors( chr_id, ink_clr, paper_clr );
					}
					else
					{
						m_data.update_ink_paper_colors( chr_id, paper_clr, ink_clr );
					}
				};
				
				if( m_edit_mode == EMode.bem_CHR_select )
				{
					swap_ink_paper( get_selected_quad_CHR_id() );
				}
				else
				{
					ulong chr_ids = get_selected_block_CHRs();
					
					for( int i = 0; i < utils.CONST_BLOCK_SIZE; i++ )
					{
						swap_ink_paper( ( int )( ( chr_ids >> ( i << 4 ) ) & 0xffff ) );
					}
				}
				
				// update palette slots
				set_active_palette();
				
				dispatch_event_pixel_changed();
				dispatch_event_data_changed();
				dispatch_event_need_gfx_update();
				
				update();
			}
		}
#endif
	}
}