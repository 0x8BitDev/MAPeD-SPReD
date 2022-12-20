/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 16.12.2022
 * Time: 20:39
 */
using System;
using System.Drawing;
using System.Collections.Generic;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace MAPeD
{
	/// <summary>
	/// Description of image_cache.
	/// </summary>
	public class image_cache
	{
		private readonly Dictionary< int, SKBitmap >	m_cache = new Dictionary< int, SKBitmap >( 256 );
		
		public image_cache()
		{
			//...
		}
		
		public void reset()
		{
			foreach( SKBitmap bmp in m_cache.Values )
			{
				bmp.Dispose();
			}
			
			m_cache.Clear();
		}
		
		public SKBitmap get( Bitmap _bmp )
		{
			int hash_code = _bmp.GetHashCode();
			
			if( m_cache.ContainsKey( hash_code ) )
			{
				return m_cache[ hash_code ];
			}
			
			SKBitmap bmp = Extensions.ToSKBitmap( _bmp );
			
			m_cache[ hash_code ] = bmp;
			
			return bmp;
		}
	}
}
