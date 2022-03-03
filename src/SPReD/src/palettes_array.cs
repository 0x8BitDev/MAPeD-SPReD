/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 24.02.2022
 * Time: 17:16
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace SPReD
{
	/// <summary>
	/// Description of palettes_array.
	/// </summary>
	public class palettes_array
	{
		private palette16_data[]	m_plts	= new palette16_data[ utils.CONST_PALETTE16_ARR_LEN ];
		
		private ComboBox	m_cbox	= null;
		
		public int palette_index
		{
			get { return m_cbox.SelectedIndex; }
			set { m_cbox.SelectedIndex = value; }
		}
		
		private static palettes_array instance = null;
		
		public static  palettes_array Instance
		{
			get
			{
				return instance;
			}
		}
		
		public palettes_array( ComboBox _cbox_palettes )
		{
			instance = this;
			
			m_cbox = _cbox_palettes;
			
			m_cbox.DrawItem	+= new DrawItemEventHandler( PaletteDrawItem_Event );
			
			m_cbox.Items.Clear();
			
			for( int i = 0; i < utils.CONST_PALETTE16_ARR_LEN; i++ )
			{
				m_cbox.Items.Add( String.Format( " #{0:d2}", i ) );
				
				m_plts[ i ] = new palette16_data();
#if DEF_PCE
				m_plts[ i ].assign( i == 0 ? 0x16f:0, 64, 128, 192, 64+20, 128+20, 192+20, 256+20, 128+40, 192+40, 256+40, 320+40, 192+60, 256+60, 320+60, 384+60 );
#else
...
#endif
			}
		}
		
		void PaletteDrawItem_Event( object sender, DrawItemEventArgs e )
		{
			if( e.Index >= 0 )
			{
				Graphics gfx = e.Graphics;
				e.DrawBackground();
				
				string item_txt		= m_cbox.Items[ e.Index ].ToString();
				SizeF txt_size		= gfx.MeasureString( item_txt, e.Font );
				int plt_offset		= ( int )txt_size.Width + 10;
					
				utils.brush.Color = e.ForeColor;
				gfx.DrawString( item_txt, e.Font, utils.brush, e.Bounds.X, e.Bounds.Y );
				
				palette16_data plt	= m_plts[ e.Index ];

				int clr;
				int clr_box_side	= 12;
				int clr_box_y_offs	= e.Bounds.Y + ( ( e.Bounds.Height - clr_box_side ) >> 1 );
				int clrs_cnt		= utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS;
				
				for( int i = 0; i < clrs_cnt; i++ )
				{
					clr = palette_group.Instance.main_palette[ plt.data[ i ] ];
					
					utils.brush.Color = Color.FromArgb( (clr&0xff0000)>>16, (clr&0xff00)>>8, clr&0xff );
					
					gfx.FillRectangle( utils.brush, plt_offset + ( i * clr_box_side ), clr_box_y_offs, clr_box_side, clr_box_side );
				}
				
				utils.pen.Color = Color.Black;
				gfx.DrawRectangle( utils.pen, plt_offset - 1, clr_box_y_offs - 1, ( clr_box_side * clrs_cnt ) + 1, clr_box_side + 1 );
			}
		}
		
		public void update_palette()
		{
			if( m_cbox.SelectedIndex >= 0 )
			{
				palette16_data plt = m_plts[ m_cbox.SelectedIndex ];
				
				palette_group	plt_grp	= palette_group.Instance;
				palette_small[]	plt_arr	= plt_grp.get_palettes_arr();
				
				for( int i = 0; i < ( utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ); i++ )
				{
					plt_arr[ i >> 2 ].get_color_inds()[ i & 0x03 ] = plt.data[ i ];
				}

				// update subpalettes
				plt_arr[ 0 ].update();
				plt_arr[ 1 ].update();
				plt_arr[ 2 ].update();
				plt_arr[ 3 ].update();
			}
		}
		
		public void update_color( int _clr_pos, int _clr_ind )
		{
			if( m_cbox.SelectedIndex >= 0 )
			{
				m_plts[ m_cbox.SelectedIndex ].data[ _clr_pos ] = _clr_ind;
			}
		}
		
		public int get_color( int _plt_ind, int _clr_pos )
		{
			return m_plts[ _plt_ind ].data[ _clr_pos ];
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_plts.Length );
			
			for( int i = 0; i < m_plts.Length; i++ )
			{
				m_plts[ i ].save( _bw );
			}
		}
		
		public void load( BinaryReader _br )
		{
			int size = _br.ReadInt32();
			
			for( int i = 0; i < m_plts.Length; i++ )
			{
				if( i < size )
				{
					 m_plts[ i ].load( _br );
				}
			}
		}

		public void export( StreamWriter _sw, string _prefix )
		{
			_sw.WriteLine( _prefix + "_palette:" );
			
			for( int i = 0; i < utils.CONST_PALETTE16_ARR_LEN; i++ )
			{
				m_plts[ i ].export( _sw );
			}
		}
	}

	public class palette16_data
	{
		private const int CONST_DATA_SIZE	= 16;
		
		private int[] m_data;
		
		public int[] data
		{
			get { return m_data; }
			set {}
		}
		
		public palette16_data()
		{
			m_data	= new int[ CONST_DATA_SIZE ];
			
			Array.Clear( m_data, 0, m_data.Length );
		}
		
		public void assign( int _01, int _02, int _03, int _04, int _05, int _06, int _07, int _08, 
							int _09, int _10, int _11, int _12, int _13, int _14, int _15, int _16 )
		{
			m_data[ 0 ]		= _01; m_data[ 1 ]	= _02; m_data[ 2 ]	= _03; m_data[ 3 ]	= _04;
			m_data[ 4 ] 	= _05; m_data[ 5 ]	= _06; m_data[ 6 ]	= _07; m_data[ 7 ]	= _08;
			m_data[ 8 ] 	= _09; m_data[ 9 ]	= _10; m_data[ 10 ]	= _11; m_data[ 11 ]	= _12;
			m_data[ 12 ]	= _13; m_data[ 13 ]	= _14; m_data[ 14 ]	= _15; m_data[ 15 ]	= _16;
		}
		
		public void save( BinaryWriter _bw )
		{
			for( int i = 0; i < CONST_DATA_SIZE; i++ )
			{
				_bw.Write( m_data[ i ] );
			}
		}
		
		public void load( BinaryReader _br )
		{
			for( int i = 0; i < CONST_DATA_SIZE; i++ )
			{
				 m_data[ i ] = _br.ReadInt32();
			}
		}
		
		public void export( StreamWriter _sw ) 
		{
			string plt_asm_data = "\t.word ";
				
			for( int i = 0; i < CONST_DATA_SIZE; i++ )
			{
				plt_asm_data += String.Format( "${0:X2}", data[ i ] ) + ( ( i != CONST_DATA_SIZE - 1 ) ? ", ":"" );
			}
			
			_sw.WriteLine( plt_asm_data );
		}
	}
}