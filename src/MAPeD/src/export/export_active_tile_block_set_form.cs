﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 28.12.2018
 * Time: 16:46
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MAPeD
{
	/// <summary>
	/// Description of export_active_tile_block_set_form.
	/// </summary>
	public partial class export_active_tile_block_set_form : Form
	{
		public export_active_tile_block_set_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public void export( string _filename, tiles_data _data, List< Bitmap > _tiles_imgs, List< Bitmap > _blocks_imgs )
		{
			if( RBtnTiles.Checked )
			{
				export_tiles_blocks_data( _data.get_first_free_tile_id( false ), 32, _tiles_imgs, _filename );
			}
			else
			{
				export_tiles_blocks_data( _data.get_first_free_block_id( false ), 16, _blocks_imgs, _filename );
			}
		}
		
		private void export_tiles_blocks_data( int _tiles_cnt, int _data_size, List< Bitmap > _image_list, string _filename )
		{
			int num_active_tiles;
			int tiles_width;
			int i;
			int x, y;
			
			// get a number of non zero tiles
			if( ( num_active_tiles = _tiles_cnt ) == 0 )
			{
				throw new Exception( "There is no data to export!" );
			}
			
			// draw images into bitmap as rectangle MxN ( suggested by codediy )
			tiles_width = ( int )NumericUpDownImgsCntWidth.Value;
			
			Bitmap bmp		= new Bitmap( _data_size * tiles_width, ( int )Math.Ceiling( ( float )num_active_tiles / tiles_width ) * _data_size );
			Graphics gfx	= Graphics.FromImage( bmp );
			
			gfx.InterpolationMode 	= InterpolationMode.NearestNeighbor;
			gfx.PixelOffsetMode 	= PixelOffsetMode.HighQuality;
			
			for( i = 0; i < num_active_tiles; i++ )
			{
				x = i % tiles_width;
				y = ( int )Math.Floor( ( float )i / tiles_width );
				
				gfx.DrawImage( _image_list[ i ], x * _data_size, y * _data_size, _data_size, _data_size );
			}
			
			bmp.Save( _filename, ImageFormat.Bmp );
			
			bmp.Dispose();
			gfx.Dispose();
		}
	}
}
