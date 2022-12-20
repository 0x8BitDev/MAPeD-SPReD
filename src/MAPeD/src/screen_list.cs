/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 24.03.2021
 * Time: 11:54
 */
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace MAPeD
{
	/// <summary>
	/// Description of i_screen_list.
	/// </summary>
	public interface i_screen_list
	{
		void		set_image_list_size( Size _size );
		void 		add( Bitmap _img );
		void 		add( SKBitmap _img );
		bool		replace( int _ind, Bitmap _img, bool _dispose_old );
		bool		replace( int _ind, SKBitmap _img, bool _dispose_old );
		Bitmap		get( int _ind );
		SKBitmap	get_skbitmap( int _ind );
		bool		remove( int _ind );
		int			count();
		void		clear();
	}
	
	public class screen_list_normal : i_screen_list
	{
		protected readonly ImageList	m_img_list;

		public screen_list_normal( ImageList _img_list )
		{
			m_img_list = _img_list;
		}
		
		public virtual void	set_image_list_size( Size _size )
		{
			m_img_list.ImageSize = _size;
		}
		
		public virtual void	add( Bitmap _img )
		{
			m_img_list.Images.Add( _img );
		}
		
		public virtual void	add( SKBitmap _img )
		{
			m_img_list.Images.Add( Extensions.ToBitmap( _img ) );
		}
		
		public virtual bool	replace( int _ind, Bitmap _img, bool _dispose_old )
		{
			if( _ind >= 0 && _ind < m_img_list.Images.Count )
			{
				if( _dispose_old )
				{
					m_img_list.Images[ _ind ].Dispose();
				}
				
				m_img_list.Images[ _ind ] = _img;
				
				return true;
			}
			
			return false;
		}
		
		public virtual bool	replace( int _ind, SKBitmap _img, bool _dispose_old )
		{
			return replace( _ind, Extensions.ToBitmap( _img ), _dispose_old );
		}
		
		public virtual Bitmap get( int _ind )
		{
			return ( Bitmap )m_img_list.Images[ _ind ];
		}
		
		public virtual SKBitmap get_skbitmap( int _ind )
		{
			return Extensions.ToSKBitmap( ( Bitmap )m_img_list.Images[ _ind ] );
		}
		
		public virtual bool remove( int _ind )
		{
			if( _ind >= 0 && _ind < m_img_list.Images.Count )
			{
				m_img_list.Images[ _ind ].Dispose();
				
				m_img_list.Images.RemoveAt( _ind );
				
				return true;
			}
			
			return false;
		}
		
		public int count()
		{
			return m_img_list.Images.Count;
		}
			
		public virtual void	clear()
		{
			foreach( Bitmap img in m_img_list.Images )
			{
				img.Dispose();
			}
			
			m_img_list.Images.Clear();
		}
	}
	
	public class screen_list_scaled : screen_list_normal
	{
		private readonly List< SKBitmap >	m_os_img_list = new List< SKBitmap >();
			
		public screen_list_scaled( ImageList _img_list ) : base( _img_list )
		{
			//...
		}
		
		private Bitmap scale_img( Bitmap _img )
		{
			Bitmap bmp = new Bitmap( m_img_list.ImageSize.Width, m_img_list.ImageSize.Height, PixelFormat.Format32bppPArgb );
			
			Graphics gfx = Graphics.FromImage( bmp );
			
			gfx.InterpolationMode 	= InterpolationMode.High;
			gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			
			gfx.DrawImage( _img, 0, 0, bmp.Width, bmp.Height );
			gfx.Dispose();
			
			return bmp;
		}
		
		public override void add( Bitmap _img )
		{
			m_os_img_list.Add( Extensions.ToSKBitmap( _img ) );
			
			base.add( scale_img( _img ) );
		}
		
		public override void add( SKBitmap _img )
		{
			m_os_img_list.Add( _img );
			
			base.add( scale_img( Extensions.ToBitmap( _img ) ) );
		}
		
		public override bool replace( int _ind, Bitmap _img, bool _dispose_old )
		{
			if( base.replace( _ind, scale_img( _img ), _dispose_old ) )
			{
				if( _dispose_old )
				{
					m_os_img_list[ _ind ].Dispose();
				}
				
				m_os_img_list[ _ind ] = Extensions.ToSKBitmap( _img );
				
				return true;
			}
			
			return false;
		}
		
		public override bool replace( int _ind, SKBitmap _img, bool _dispose_old )
		{
			if( base.replace( _ind, scale_img( Extensions.ToBitmap( _img ) ), _dispose_old ) )
			{
				if( _dispose_old )
				{
					m_os_img_list[ _ind ].Dispose();
				}
				
				m_os_img_list[ _ind ] = _img;
				
				return true;
			}
			
			return false;
		}
		
		public override Bitmap get( int _ind )
		{
			return Extensions.ToBitmap( m_os_img_list[ _ind ] );
		}
		
		public override SKBitmap get_skbitmap( int _ind )
		{
			return m_os_img_list[ _ind ];
		}
		
		public override bool remove( int _ind )
		{
			if( base.remove( _ind ) )
			{
				m_os_img_list[ _ind ].Dispose();
				
				m_os_img_list.RemoveAt( _ind );
				
				return true;
			}
			
			return false;
		}
		
		public override void clear()
		{
			base.clear();
			
			foreach( SKBitmap img in m_os_img_list )
			{
				img.Dispose();
			}
			
			m_os_img_list.Clear();
		}
	}
}