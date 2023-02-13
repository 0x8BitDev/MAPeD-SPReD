/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 12.12.2022
 * Time: 21:00
 */
using System;
using System.Windows.Forms;

using SkiaSharp;
using SkiaSharp.Views.Desktop;


namespace MAPeD
{
	/// <summary>
	/// Description of drawable_skia.
	/// </summary>
	public class drawable_SKGL
	{
		protected SKGLControl	m_pix_box;
		protected SKPaint		m_line_paint;
		protected SKPaint		m_image_paint;
		
		protected readonly		SKColor	CONST_BACKGROUND_COLOR	= SKColor.Parse( "#78505050" );
		
		protected SKRect		m_pbox_rect	= new SKRect();
		
		protected SKSurface		m_surface	= null;
		
		private EventHandler< SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs > m_on_paint;
		
		public drawable_SKGL()
		{
			//...
		}
		
		public drawable_SKGL( SKGLControl _pbox )
		{
			set_pix_box( _pbox );
		}
		
		protected void set_pix_box( SKGLControl _pbox )
		{
			m_pix_box = _pbox;
		
			m_image_paint = new SKPaint();
			
			m_line_paint				= new SKPaint();
			m_line_paint.Color			= utils.CONST_COLOR_LAYOUT_PIXBOX_DEFAULT;
			m_line_paint.IsAntialias	= false;
			m_line_paint.StrokeCap		= SKStrokeCap.Square;
			m_line_paint.StrokeWidth	= 1.0f;
			m_line_paint.IsStroke		= true;
			
			prepare_pix_box();
			
			m_on_paint	= new EventHandler< SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs >( on_paint_surface );
			
			m_pix_box.Resize += on_resize;
		}
		
		protected virtual void on_resize( object sender, EventArgs e )
		{
			m_surface = null;
			
			m_pix_box.PaintSurface += m_on_paint;
			
			prepare_pix_box();
			
			// get valid surface
			invalidate();
			
			update();
		}
		
		private void on_paint_surface( object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e )
		{
			m_surface = e.Surface;
			
			m_pix_box.PaintSurface -= m_on_paint;
			
			update();
		}
		
		public virtual void update()
		{
			//...
		}
		
		protected virtual void prepare_pix_box()
		{
			m_pbox_rect.Left	= 0;
			m_pbox_rect.Top		= 0;
			m_pbox_rect.Right	= m_pix_box.Width;
			m_pbox_rect.Bottom	= m_pix_box.Height;
		}
		
		protected void clear_background( SKColor _color )
		{
			m_surface.Canvas.Clear( _color );
		}
		
		public void invalidate()
		{
			m_pix_box.Refresh();
		}
		
		public bool disable( bool _on )
		{
			bool prev_enabled = m_pix_box.Enabled;
			
			m_pix_box.Enabled = !_on;
			
			return prev_enabled == m_pix_box.Enabled; 
		}
		
		public void draw_border( SKColor _clr )
		{
			m_line_paint.Color = _clr;
			m_surface.Canvas.DrawRect( 1, 1, m_pix_box.Width - 1, m_pix_box.Height - 1, m_line_paint );
		}
		
		public void enable_smoothing_mode( bool _on )
		{
			if( _on )
			{
				m_image_paint.FilterQuality = SKFilterQuality.Medium;
			}
			else
			{
				m_image_paint.FilterQuality = SKFilterQuality.None;
			}
		}
	}
}
