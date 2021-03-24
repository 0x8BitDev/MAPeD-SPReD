/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 17.03.2017
 * Time: 15:20
 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MAPeD
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
			
			prepare_pix_box();

			m_pix_box.Resize += new EventHandler( Resize_Event );
			
			m_pen = new Pen( utils.CONST_COLOR_PIXBOX_DEFAULT );
			m_pen.EndCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;
			m_pen.StartCap 	= System.Drawing.Drawing2D.LineCap.NoAnchor;			
		}
		
		protected virtual void Resize_Event( object sender, EventArgs e )
		{
			prepare_pix_box();
			
			update();
		}
		
		public virtual void update()
		{
			//...
		}
		
		private void prepare_pix_box()
		{
			if( m_main_bmp != null )
			{
				m_main_bmp.Dispose();
			}
			
			if( m_gfx != null )
			{
				m_gfx.Dispose();
			}
			
			m_main_bmp = new Bitmap( m_pix_box.Width, m_pix_box.Height, PixelFormat.Format32bppArgb );
			
			m_gfx = Graphics.FromImage( m_main_bmp );
			enable_smoothing_mode( false );
			
			m_pix_box.Image = m_main_bmp;
		}
		
		protected void clear_background( int _color )
		{
			m_gfx.Clear( Color.FromArgb( _color ) );
		}
		
		public void invalidate()
		{
			m_pix_box.Invalidate();
		}

		public void disable( bool _on )
		{
			if( _on )
			{
				// draw the red cross as a sign of inactive state
				m_pen.Color = utils.CONST_COLOR_PIXBOX_INACTIVE_CROSS;
				
				m_gfx.DrawLine( m_pen, 0, 0, m_pix_box.Width, m_pix_box.Height );
				m_gfx.DrawLine( m_pen, m_pix_box.Width, 0, 0, m_pix_box.Height );			
			}
			
			m_pix_box.Enabled = !_on;
		}

		public void draw_border( Color _clr )
		{
			m_pen.Color = _clr;
			m_gfx.DrawRectangle( m_pen, 1, 1, m_pix_box.Width - 1, m_pix_box.Height - 1 );
		}
		
		public void enable_smoothing_mode( bool _on )
		{
			if( _on )
			{
				m_gfx.SmoothingMode 	= SmoothingMode.HighSpeed;
				m_gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
				m_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			}
			else
			{
				m_gfx.SmoothingMode 	= SmoothingMode.HighSpeed;
				m_gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
				m_gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			}
		}
	}
}
