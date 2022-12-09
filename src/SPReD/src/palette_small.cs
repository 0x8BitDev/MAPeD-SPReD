/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 17.03.2017
 * Time: 16:59
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of palette_small.
	/// </summary>
	/// 
	
	public delegate void ActivePalette();

	public class palette_small : drawable_base
	{
		public event EventHandler ActivePalette;
		
		private bool m_active = false;
		
		private readonly int m_id;
		
		public int id
		{
			get { return m_id; }
			set {}
		}
		
		public bool active
		{
			get { return m_active; }
			set 
			{ 
				m_active = value;
				
				if( m_active == false ) 
				{ 
					m_sel_clr_ind = -1; 
				} 
				else 
				{ 
					if( m_sel_clr_ind < 0 )
					{
						// the 1st color slot will be active by default
						m_sel_clr_ind = 1;
						
						if( ActivePalette != null )
						{
							ActivePalette( this, null );
						}
					}
				}

				update();
			}
		}
		
		private int m_sel_clr_ind 	= -1;
		private int[] m_clr_inds 	= null;
		
		public int color_slot
		{
			get { return m_sel_clr_ind; }
			set
			{
				if( value >= 0 && value < utils.CONST_PALETTE_SMALL_NUM_COLORS )
				{
					active = true;
					
					m_sel_clr_ind = value;
					
					if( ActivePalette != null )
					{
						ActivePalette( this, null );
					}
		
					update();
				}
			}
		}
		
		public palette_small( int _id, PictureBox _pbox ) : base( _pbox )
		{
			m_id = _id;
			
#if DEF_NES
			m_clr_inds = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ];
#elif DEF_SMS
			m_clr_inds = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ];
#elif DEF_PCE
			m_clr_inds = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ];
#endif
			m_pix_box.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Layout_MouseClick);
		}
		
		public void reset()
		{
#if DEF_NES
			m_clr_inds = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 16, ( 1 + m_id * 3 ) + 16, ( 1 + m_id * 3 ) + 32, ( 1 + m_id * 3 ) + 48 };
#elif DEF_SMS
			m_clr_inds = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ ( m_id == 0 ) ? 42:m_id*16 + m_id, m_id*16 + m_id + 3, m_id*16 + m_id + 7, m_id*16 + m_id + 11 };
#elif DEF_PCE
			m_clr_inds = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ]{ 0, 0, 0, 0 };
#endif
			update();
		}
		
		public void update()
		{
			int clr;
			
			for( int i = 0; i < utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
			{
				clr = palette_group.Instance.main_palette[ m_clr_inds[ i ] ];
				
				utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
				m_gfx.FillRectangle( utils.brush, ( i * 20 ), 0, 20, 20 );
			}
			
			if( m_sel_clr_ind >= 0 )
			{
				int x = ( m_sel_clr_ind * 20 );
				
				m_pen.Color = Color.White;
				m_gfx.DrawRectangle( m_pen, x+2, 2, 17, 17 );
				
				m_pen.Color = Color.Black;
				m_gfx.DrawRectangle( m_pen, x+1, 1, 19, 19 );
			}

			invalidate();
		}
		
		private void Layout_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			active = true;
			
			m_sel_clr_ind = e.X / 20;
			
			if( ActivePalette != null )
			{
				ActivePalette( this, null );
			}

			update();
		}
		
		public void update_color( int _clr_id, int _slot_id = -1 )
		{
			// save the selected color index and update gfx
			if( _slot_id < 0 )
			{
				if( m_active == true )
				{
					m_clr_inds[ m_sel_clr_ind ] = _clr_id;
					
					update();
				}
			}
			else
			{
				m_clr_inds[ _slot_id ] = _clr_id;
				
				update();
			}
		}
		
		public int[] get_color_inds()
		{
			return m_clr_inds;
		}
		
		public int get_selected_slot_color_id()
		{
			return m_clr_inds[ m_sel_clr_ind ];
		}
		
		public void save( BinaryWriter _bw )
		{
			for( int i = 0; i < utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
			{
				_bw.Write( m_clr_inds[ i ] );
			}
		}
		
		public void load( BinaryReader _br )
		{
			for( int i = 0; i < utils.CONST_PALETTE_SMALL_NUM_COLORS; i++ )
			{
				m_clr_inds[ i ] = _br.ReadInt32();
			}
			
			update();
		}
	}
}
