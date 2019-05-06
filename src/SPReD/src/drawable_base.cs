/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 17.03.2017
 * Time: 15:20
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of drawable_base.
	/// </summary>
	public class drawable_base
	{
		protected PictureBox 	m_pix_box 	= null;
		protected Bitmap		m_main_bmp	= null;
		protected Graphics 		m_gfx		= null;
		protected Pen			m_pen		= null;
		
		protected const int	CONST_BACKGROUND_COLOR	= 0x78505050;
		
		public drawable_base( PictureBox _pbox )
		{
			m_pix_box = _pbox;
			
			m_main_bmp = new Bitmap( _pbox.Width, _pbox.Height );
			_pbox.Image = m_main_bmp;
			
			m_gfx = Graphics.FromImage( m_main_bmp );
			m_gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			m_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			
			m_pen = new Pen( Color.White );
			m_pen.EndCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.StartCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;			
		}
		
		protected void clear_background( int _color )
		{
			m_gfx.Clear( Color.FromArgb( _color ) );
		}
		
		protected void clear_background( Color _color )
		{
			m_gfx.Clear( _color );
		}
		
		public void invalidate()
		{
			draw_border( Color.Black );
			
			m_pix_box.Invalidate();
		}
		
		private void draw_border( Color _clr )
		{
			m_pen.Color = _clr;
			m_gfx.DrawRectangle( m_pen, 1, 1, m_pix_box.Width - 1, m_pix_box.Height - 1 );
		}
		
		public void disable( bool _on )
		{
			if( _on )
			{
				// draw the red cross as sign of the control is inactive
				m_pen.Color = Color.Red;
				
				m_gfx.DrawLine( m_pen, 0, 0, m_pix_box.Width, m_pix_box.Height );
				m_gfx.DrawLine( m_pen, m_pix_box.Width, 0, 0, m_pix_box.Height );			
			}
			
			m_pix_box.Enabled = !_on;
		}
	}
}
