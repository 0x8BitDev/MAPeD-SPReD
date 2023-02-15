/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 13.03.2017
 * Time: 16:51
 */
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;

namespace SPReD
{
	/// <summary>
	/// Description of sprites_processor
	/// </summary>
	public class sprite_processor
	{
		private readonly sprite_layout_viewer	m_sprite_layout_viewer;
		private readonly CHR_bank_viewer		m_CHR_bank_viewer;
		private readonly CHR_data_storage		m_CHR_data_storage;
		private readonly palette_group			m_palette_grp;
		
		int m_opt_stats_empty_CHRs 		= 0;
		int m_opt_stats_unused_CHRs 	= 0;
		int m_opt_stats_duplicate_CHRs 	= 0;
		int m_opt_stats_total_CHRs		= 0;
		
		public sprite_processor( PictureBox 	_spr_layout, 
		                          Label			_spr_layout_label,
		                          GroupBox		_spr_layout_grp_box,
		                          PictureBox 	_chr_bank,
		                          Label			_chr_bank_label,
								  PictureBox 	_plt_main,
								  PictureBox 	_plt0,
								  PictureBox 	_plt1,
								  PictureBox 	_plt2,
								  PictureBox 	_plt3 )
		{
			m_sprite_layout_viewer	= new sprite_layout_viewer( _spr_layout, _spr_layout_label, _spr_layout_grp_box );
			m_CHR_bank_viewer 		= new CHR_bank_viewer( _chr_bank, _chr_bank_label );
			m_CHR_data_storage		= new CHR_data_storage();
			m_palette_grp			= new palette_group( _plt_main, _plt0, _plt1, _plt2, _plt3 );
			
			m_CHR_bank_viewer.subscribe( m_palette_grp );
			m_CHR_bank_viewer.subscribe( m_sprite_layout_viewer );
			m_sprite_layout_viewer.subscribe( m_palette_grp );
			m_sprite_layout_viewer.subscribe( m_CHR_bank_viewer );
		}
#if DEF_FIXED_LEN_PALETTE16_ARR
		public void subscribe( MainForm _mf )
		{
			m_sprite_layout_viewer.SetCHRPalette		+= _mf.SetCHRPalette;
			m_sprite_layout_viewer.ApplyPaletteToCHR	+= _mf.ApplyPaletteToCHR;
		}
#endif
		public void reset()
		{
			{
				m_sprite_layout_viewer.reset();
				
				sprite_data spr = null;
				m_sprite_layout_viewer.init( spr );
			}
			{
				m_CHR_bank_viewer.reset();
			}
			
			m_CHR_data_storage.reset();
			
			CHR_data_group.reset_ids_cnt();
		}
		
		public sprite_data load_sprite_png( string _filename, string _name, bool _apply_palette, bool _crop_image, int _palette_slot )
		{
			PngReader png_reader = FileHelper.CreatePngReader( _filename );
			
			try
			{
				if(!png_reader.ImgInfo.Indexed )
				{
					throw new Exception( _filename + "\n\nNot indexed image!" );
				}
				
				if( png_reader.IsInterlaced() )
				{
					throw new Exception( _filename + "\n\nOnly non interlaced .PNG images are supported!" );
				}
	
//				if( ( png_reader.ImgInfo.Cols & 0x07 ) != 0 || ( png_reader.ImgInfo.Rows & 0x07 ) != 0 )
//				{
//					png_reader.End();
//					throw new Exception( _filename + "\n\nThe image size must be a multiple of 8 !" );
//				}
	
//				if( ( ( png_reader.ImgInfo.Cols >> 3 ) * ( png_reader.ImgInfo.Rows >> 3 ) ) > utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
//				{
//					png_reader.End();
//					throw new Exception( _filename + "\n\nThe imported image contains more than " + utils.CONST_CHR_BANK_MAX_SPRITES_CNT + " CHRs!" );
//				}
				
#if DEF_NES
				if( png_reader.GetMetadata().GetPLTE().MinBitDepth() != 2 )
				{
					throw new Exception( _filename + "\n\nThe PNG image must have a 4 colors palette!" );
				}
#elif DEF_SMS || DEF_PCE
				int img_bit_depth = png_reader.GetMetadata().GetPLTE().MinBitDepth();
	
				if( img_bit_depth != 4 && img_bit_depth != 2 )
				{
					throw new Exception( _filename + "\n\nThe PNG image must have a 4 or 2 bpp color depth!" );
				}
				
				if( png_reader.GetMetadata().GetPLTE().GetNentries() > 16 )
				{
					throw new Exception( _filename + "\n\nThe PNG image must have a 16 or 4 colors palette!" );
				}
#else
...
#endif
				sprite_params spr_params = m_CHR_data_storage.create( png_reader, _apply_palette, _crop_image, _palette_slot );
				
				sprite_data spr = new sprite_data( _name );
				spr.setup( spr_params );
				
				spr.update_dimensions();
				
				png_reader.End();
				
				return spr;
			}
			catch( System.Exception _err )
			{
				png_reader.End();
				throw _err;
			}
		}
		
		public sprite_data load_sprite_bmp( string _filename, string _name, bool _apply_palette, int _palette_slot )
		{	
			Bitmap bmp = new Bitmap( _filename );
			
			try
			{
				if( bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed && bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format4bppIndexed )
				{
					bmp.Dispose();
#if DEF_NES			
					throw new Exception( _filename + " - Pixel format: " + bmp.PixelFormat.ToString() + "\n\nThe BMP image must have a 4/8 bpp color depth \\ 4 colors palette (not RLE encoded)!" );
#elif DEF_SMS || DEF_PCE
					throw new Exception( _filename + " - Pixel format: " + bmp.PixelFormat.ToString() + "\n\nThe BMP image must have a 4/8 bpp color depth \\ 16 or 4 colors palette (not RLE encoded)!" );
#else
...
#endif
				}
				
//				if( ( bmp.Width & 0x07 ) != 0 || ( bmp.Height & 0x07 ) != 0 )
//				{
//					bmp.Dispose();
//					throw new Exception( _filename + "\n\nThe image size must be a multiple of 8 !" );
//				}
				
//				if( ( ( bmp.Width >> 3 ) * ( bmp.Height >> 3 ) ) > utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
//				{
//					bmp.Dispose();
//					throw new Exception( _filename + "\n\nThe imported image contains more than " + utils.CONST_CHR_BANK_MAX_SPRITES_CNT + " CHRs!" );
//				}
				
				sprite_params spr_params = m_CHR_data_storage.create( bmp, _apply_palette, _palette_slot );
				
				sprite_data spr = new sprite_data( _name );
				spr.setup( spr_params );
				
				spr.update_dimensions();
				
				bmp.Dispose();
				
				return spr;
			}
			catch( System.Exception _err )
			{
				bmp.Dispose();
				throw _err;
			}
		}
		
		public void update_sprite( sprite_data _spr, bool _new_sprite = false )
		{
			CHR_data_group chr_data = ( _spr != null ) ? _spr.get_CHR_data():null;
			
			List< CHR_data > sprite_chr_data = ( chr_data != null ) ? chr_data.get_data():null;
			
			m_sprite_layout_viewer.init( _spr );
			
			if( _new_sprite )
			{
				m_CHR_bank_viewer.reset();
			}
			m_CHR_bank_viewer.init( sprite_chr_data, ( chr_data != null ) ? chr_data.id + 1:-1, ( chr_data != null ) ? chr_data.get_link_cnt():-1, m_CHR_data_storage.get_banks_cnt() );
		}
		
		public void remove( sprite_data _spr )
		{
			CHR_data_group chr_data = _spr.get_CHR_data();
			
			_spr.reset();
			
			m_CHR_data_storage.remove( chr_data );
		}
		
		public bool apply_active_palette( sprite_data _spr )
		{
			if( m_palette_grp.active_palette >= 0 )
			{
				List< CHR_data_attr > chr_attr = _spr.get_CHR_attr();
				
				int size = chr_attr.Count;
				
				for( int i = 0; i < size; i++ )
				{
#if DEF_FIXED_LEN_PALETTE16_ARR
					chr_attr[ i ].palette_ind = palettes_array.Instance.palette_index;
#else
					chr_attr[ i ].palette_ind = m_palette_grp.active_palette;
#endif
				}
				
				return true;
			}
			
			return false;
		}
#if DEF_FIXED_LEN_PALETTE16_ARR
		public void apply_palette_to_selected_CHR( int _plt_ind )
		{
			m_sprite_layout_viewer.apply_palette_to_selected_CHR( _plt_ind );
			m_CHR_bank_viewer.update();
		}
#endif
		public void set_sprite_layout_viewer_flags( bool _show_axis, bool _show_grid )
		{
			m_sprite_layout_viewer.set_flags( _show_axis, _show_grid );
		}
		
		public void layout_sprite_centering()
		{
			m_sprite_layout_viewer.centering();
		}
		
		public void layout_zoom_in()
		{
			m_sprite_layout_viewer.zoom_in();
		}

		public void layout_zoom_out()
		{
			m_sprite_layout_viewer.zoom_out();
		}
		
		public void flip_selected_CHR( int _flag )
		{
			switch( _flag )
			{
				case CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP:
					{
						m_sprite_layout_viewer.flip_selected_horiz();
					}
					break;
					
				case CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP:
					{
						m_sprite_layout_viewer.flip_selected_vert();
					}
					break;
			}
		}
		
		public void export_palette( StreamWriter _sw, string _prefix )
		{
			m_palette_grp.export( _sw, _prefix );
		}
		
		public CHR_data_group extract_and_create_CHR_data_group( sprite_data _spr, bool _mode8x16 )
		{
			CHR_data_group chr_data = new CHR_data_group();
			
			List< CHR_data > data = _spr.get_CHR_data().get_data();
			
			int CHR_id;
			
			int size = _spr.get_CHR_attr().Count;
			
			for( int i = 0; i < size; i++ )
			{
				CHR_id = _spr.get_CHR_attr()[ i ].CHR_ind;
				
				chr_data.get_data().Add( data[ CHR_id ].copy() );
				
				if( _mode8x16 && ( ++CHR_id < data.Count ) )
				{
					chr_data.get_data().Add( data[ CHR_id ].copy() );
				}
			}
			
			m_CHR_data_storage.add( chr_data );
			
			return chr_data;
		}

		public List< CHR_data_attr > extract_and_create_CHR_data_attr( sprite_data _spr, bool _mode8x16 )
		{
			List< CHR_data_attr > chr_attrs = new List< CHR_data_attr >();
			
			CHR_data_attr attr;
			
			int size = _spr.get_CHR_attr().Count;
			
			for( int i = 0; i < size; i++ )
			{
				attr = _spr.get_CHR_attr()[ i ].copy();
				attr.CHR_ind = _mode8x16 ? ( i << 1 ):i;
				
				chr_attrs.Add( attr );
			}
			
			return chr_attrs;
		}
		
#if DEF_NES
		public void export_CHR( StreamWriter _sw, string _data_dir, string _filename, bool _commented, HashSet< int > skipped_banks_id, bool _need_padding )
		{
			m_CHR_data_storage.export( _sw, _data_dir, _filename, _commented, skipped_banks_id, _need_padding );
		}
#elif DEF_SMS
		public void export_CHR( StreamWriter _sw, string _data_dir, string _filename, bool _commented, HashSet< int > skipped_banks_id, int _CHR_size )
		{
			m_CHR_data_storage.export( _sw, _data_dir, _filename, _commented, skipped_banks_id, _CHR_size );
		}
#elif DEF_PCE
		public void export_CHR( StreamWriter _sw, string _data_dir, string _filename, bool _commented, HashSet< int > skipped_banks_id, bool _asm_data )
		{
			m_CHR_data_storage.export( _sw, _data_dir, _filename, _commented, skipped_banks_id, _asm_data );
		}
#else
...
#endif
		public void rearrange_CHR_data_ids()
		{
			m_CHR_data_storage.rearrange_CHR_data_ids();
		}
		
		public bool merge_CHR( 	sprite_data _spr_1, 
								sprite_data _spr_2, 
								bool _mode8x16 )
		{
			CHR_data_group spr2_chr_data = _spr_2.get_CHR_data();
			{
				if( _spr_1.merge_CHR( _spr_2, _mode8x16 ) )
				{
					m_CHR_data_storage.remove( spr2_chr_data );
					
					return true;
				}
			}
			
			return false;
		}
		
		public bool split_CHR( sprite_data _spr, bool _mode8x16 )
		{
			if( _spr.split_CHR( _mode8x16 ) )
			{
				// add new
				CHR_data_group chr_data = _spr.get_CHR_data();
				
				m_CHR_data_storage.add( chr_data );
				
				return true;
			}
			
			return false;
		}

		public void CHR_bank_optimization_begin()
		{
			m_opt_stats_empty_CHRs 		= 0;
			m_opt_stats_unused_CHRs 	= 0;
			m_opt_stats_duplicate_CHRs 	= 0;
			m_opt_stats_total_CHRs		= 0;
		}
		
		public void CHR_bank_optimization( int _CHR_bank_id, ListBox.ObjectCollection _sprites, bool _8x16_mode )
		{
			CHR_data_group CHR_bank = get_CHR_bank( _CHR_bank_id );
			
			if( CHR_bank == null )
			{
				// the bank had one sprite and the bank has been removed
				return;
			}
			
			int CHR_n;
			int CHR_cnt = CHR_bank.get_data().Count;
			
			int spr_n;
			int spr_cnt = _sprites.Count;

			int attr_n;
			int attr_cnt;
			
			sprite_data 	spr;
			CHR_data_attr 	attr;
			
			bool CHR_used;
			
			int nCHR_n;
			CHR_data CHR_a;
			CHR_data CHR_b;
			
			// remove not empty duplicates
			for( CHR_n = 0; CHR_n < CHR_cnt; CHR_n++ )
			{
				CHR_a = CHR_bank.get_data()[ CHR_n ];
				
				if( ( _8x16_mode == true && ( ( CHR_n & 0x01 ) == 0x01 ) ) || CHR_a.is_empty() )
				{
					continue;
				}
				
				for( nCHR_n = CHR_n + 1; nCHR_n < CHR_cnt; nCHR_n++ )
				{
					CHR_b = CHR_bank.get_data()[ nCHR_n ];
					
					if( ( _8x16_mode == false && CHR_a.equals( CHR_b ) ) || ( _8x16_mode == true && CHR_a.equals( CHR_b ) && CHR_bank.get_data()[ CHR_n + 1 ].equals( CHR_bank.get_data()[ nCHR_n + 1 ] ) ) )
					{
						// go through all sprites wich use _CHR_bank_id
						// and check duplicate CHRs
						for( spr_n = 0; spr_n < spr_cnt; spr_n++ )
						{
							spr = _sprites[ spr_n ] as sprite_data;
							
							if( spr.get_CHR_data().id == _CHR_bank_id )
							{
								attr_cnt = spr.get_CHR_attr().Count;
								
								for( attr_n = 0; attr_n < attr_cnt; attr_n++ )
								{
									attr = spr.get_CHR_attr()[ attr_n ];
									
									if( attr.CHR_ind == nCHR_n )
									{
										attr.CHR_ind = CHR_n;
										
										// count not empty CHR only
										if( !CHR_b.is_empty() )
										{
											// mark used CHR as empty
											CHR_b.clear();
											
											++m_opt_stats_duplicate_CHRs;
											--m_opt_stats_unused_CHRs;
											
											if( _8x16_mode )
											{
												++m_opt_stats_duplicate_CHRs;
												--m_opt_stats_unused_CHRs;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			
			for( CHR_n = 0; CHR_n < CHR_cnt; CHR_n++ )
			{
				CHR_used = false;
				
				// go through all sprites wich use _CHR_bank_id
				// and check for unused attributes
				for( spr_n = 0; spr_n < spr_cnt; spr_n++ )
				{
					spr = _sprites[ spr_n ] as sprite_data;
					
					if( spr.get_CHR_data().id == _CHR_bank_id )
					{
						attr_cnt = spr.get_CHR_attr().Count;
						
						for( attr_n = 0; attr_n < attr_cnt; attr_n++ )
						{
							attr = spr.get_CHR_attr()[ attr_n ];
							
							if( attr.CHR_ind == CHR_n )
							{
								CHR_used = true;
								
								break;
							}
							
							if( _8x16_mode )
							{
								if( attr.CHR_ind + 1 == CHR_n )
								{
									CHR_used = true;
									
									break;
								}
							}
						}
						
						if( CHR_used == true )
						{
							break;
						}
					}
				}
				
				if( CHR_used == false )
				{
					// clear unused CHR
					CHR_bank.get_data()[ CHR_n ].clear();
					
					++m_opt_stats_unused_CHRs;
					--m_opt_stats_empty_CHRs;
				}
			}

			// delete empty CHRs and fix indices of all sprites that referring to them  
			for( CHR_n = 0; CHR_n < CHR_cnt; CHR_n++ )
			{
				if( CHR_bank.get_data()[ CHR_n ].is_empty() )
				{
					// go through all sprites wich use _CHR_bank_id
					// and check unused attributes
					for( spr_n = 0; spr_n < spr_cnt; spr_n++ )
					{
						spr = _sprites[ spr_n ] as sprite_data;
						
						if( spr.get_CHR_data().id == _CHR_bank_id )
						{
							attr_cnt = spr.get_CHR_attr().Count;
							
							for( attr_n = 0; attr_n < attr_cnt; attr_n++ )
							{
								attr = spr.get_CHR_attr()[ attr_n ];
								
								if( attr.CHR_ind == CHR_n )
								{
									// удаляем пустой атрибут
									spr.get_CHR_attr().RemoveAt( attr_n );
									
									--attr_n;
									--attr_cnt;
								}
								else
								if( attr.CHR_ind > CHR_n )
								{
									--attr.CHR_ind;
								}
							}
							
							spr.update_dimensions();
						}
					}
					
					// delete empty CHR
					CHR_bank.get_data().RemoveAt( CHR_n );
					
					--CHR_cnt;
					--CHR_n;
					
					++m_opt_stats_empty_CHRs;
				}
			}
		}
		
		public void CHR_bank_optimization_end( bool _show_msg, string _wnd_title_msg = "Optimization Statistics" )
		{
			if( _show_msg )
			{
				m_opt_stats_total_CHRs = m_opt_stats_empty_CHRs + m_opt_stats_unused_CHRs + m_opt_stats_duplicate_CHRs;
			
				MainForm.message_box( String.Format( "Deleted tiles:\n\nEmpty:\t{0}\nUnused:\t{1}\nDuplicate:\t{2}\n----------------------\nTotal:\t{3} / {4} Bytes", m_opt_stats_empty_CHRs, m_opt_stats_unused_CHRs, m_opt_stats_duplicate_CHRs, ( m_opt_stats_total_CHRs > 0 ? -m_opt_stats_total_CHRs:m_opt_stats_total_CHRs ), m_opt_stats_total_CHRs * utils.CONST_CHR_NATIVE_SIZE_IN_BYTES ), _wnd_title_msg, MessageBoxButtons.OK, MessageBoxIcon.Information );
			}
		}
		
		public void relink_CHR_data( sprite_data _spr, CHR_data_group _chr_data, int _old_chr_id )
		{
			CHR_data_group spr_chr_data = _spr.get_CHR_data();
			
			if( _spr.relink_CHR_data( _chr_data, _old_chr_id ) )
			{
				m_CHR_data_storage.remove( spr_chr_data );
			}
		}
		
		public sprite_data create_sprite( string _name, int _width, int _height, bool _mode8x16 )
		{
			sprite_data spr = new sprite_data( _name );
			
			spr.setup( this.m_CHR_data_storage.create() );

			// create an empty sprite
			int x;
			int y;
			
			CHR_data_attr 	attr 		= null;
			CHR_data_group 	chr_data 	= spr.get_CHR_data();
		
			if( _mode8x16 )
			{
				for( y = 0; y < _height; y += 2 )
				{
					for( x = 0; x < _width; x++ )
					{
						attr = new CHR_data_attr( x << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS, y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS );
						attr.CHR_ind 		= ( x << 1 ) + ( y * _width );
						attr.palette_ind 	= palette_group.Instance.active_palette;
						attr.palette_ind 	= attr.palette_ind < 0 ? 0:attr.palette_ind;
						
						spr.get_CHR_attr().Add( attr );
						
						chr_data.get_data().Add( new CHR_data() );
						chr_data.get_data().Add( new CHR_data() );
					}
				}
			}
			else
			{
				for( y = 0; y < _height; y++ )
				{
					for( x = 0; x < _width; x++ )
					{
						attr = new CHR_data_attr( x << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS, y << utils.CONST_CHR_SIDE_PIXELS_CNT_POW_BITS );
						attr.CHR_ind 		= x + ( y * _width );
#if DEF_FIXED_LEN_PALETTE16_ARR
						attr.palette_ind 	= palettes_array.Instance.palette_index;
#else
						attr.palette_ind 	= palette_group.Instance.active_palette;
#endif
						attr.palette_ind 	= attr.palette_ind < 0 ? 0:attr.palette_ind;
						
						spr.get_CHR_attr().Add( attr );
						
						chr_data.get_data().Add( new CHR_data() );
					}
				}
			}
			
			spr.update_dimensions();
			
			return spr; 
		}
		
		public void convert_colors( int[] _plt_main, int[] _plt_small, ListBox _sprite_list, bool _8x16_mode )
		{
			for( int i = 0; i < _plt_small.Length; i++ )
			{
				palette_group.Instance.get_palettes_arr()[ i / utils.CONST_NUM_SMALL_PALETTES ].get_color_inds()[ i % utils.CONST_NUM_SMALL_PALETTES ] = utils.find_nearest_color_ind( _plt_main[ _plt_small[ i ] ] );
			}
			
			for( int i = 0; i < utils.CONST_NUM_SMALL_PALETTES; i++ )
			{
				palette_group.Instance.get_palettes_arr()[ i ].update();
			}

			// convert 4 colors palette indices to 16 colors palette
			sprite_data 	spr 	= null;
			CHR_data_attr 	attr	= null;
			CHR_data		CHR		= null;
			
			HashSet< CHR_data >	CHR_set = new HashSet<CHR_data>();
													
			for( int spr_n = 0; spr_n < _sprite_list.Items.Count; spr_n++ )
			{
				spr = _sprite_list.Items[ spr_n ] as sprite_data;
				
				for( int attr_n = 0; attr_n < spr.get_CHR_attr().Count; attr_n++ )
				{
					attr = spr.get_CHR_attr()[ attr_n ];
#if DEF_NES					
					if( attr.palette_ind >= 0 )
#elif DEF_SMS
					attr.reset_flip_flag();
					
					if( attr.palette_ind > 0 )
#endif						
					{
						CHR = spr.get_CHR_data().get_data()[ attr.CHR_ind ];
						
						if( !CHR_set.Contains( CHR ) )
						{
							for( int val_n = 0; val_n < utils.CONST_CHR_TOTAL_PIXELS_CNT; val_n++ )
							{
#if DEF_NES					
								CHR.get_data()[ val_n ] = ( byte )( CHR.get_data()[ val_n ] & 0x03 );
#elif DEF_SMS
								CHR.get_data()[ val_n ] = ( byte )( CHR.get_data()[ val_n ] + ( attr.palette_ind * 4 ) );
#endif
							}
							
							CHR_set.Add( CHR );
#if DEF_SMS
							if( _8x16_mode && attr.CHR_ind + 1 < spr.get_CHR_data().get_data().Count )
							{
								CHR = spr.get_CHR_data().get_data()[ attr.CHR_ind + 1 ];
								
								for( int val_n = 0; val_n < utils.CONST_CHR_TOTAL_PIXELS_CNT; val_n++ )
								{
									CHR.get_data()[ val_n ] = ( byte )( CHR.get_data()[ val_n ] + ( attr.palette_ind * 4 ) );
								}
							}
#endif							
						}
					}
					
					attr.palette_ind = 0;	// to remove "ghost" data on SMS and init a zero palette on NES
				}
			}
			
			CHR_set.Clear();
			CHR_set = null;
		}
		
		public void add_last_CHR( sprite_data _spr )
		{
			List< CHR_data > chr_list = _spr.get_CHR_data().get_data();
			
			if( chr_list.Count < utils.CONST_CHR_BANK_MAX_SPRITES_CNT )
			{
				_spr.get_CHR_data().get_data().Add( new CHR_data() );
				
				update_sprite( _spr );
			}
		}
		
		public void delete_last_CHR( sprite_data _spr )
		{
			List< CHR_data > 	chr_list 	= _spr.get_CHR_data().get_data();
			List< CHR_data_attr > 	attr_list 	= _spr.get_CHR_attr();
			
			if( chr_list.Count > 0 )
			{
				int last_chr = chr_list.Count - 1;
				
				int size = attr_list.Count;
				
				for( int i = 0; i < size; i++ )
				{
					if( attr_list[ i ].CHR_ind == last_chr )
					{
						attr_list.RemoveAt( i );
						
						size = attr_list.Count;
						--i;
					}
				}
				
				chr_list.RemoveAt( last_chr );

				update_sprite( _spr );
			}
		}
		
		public void layout_snap( bool _on )
		{
			m_sprite_layout_viewer.snap = _on;
		}
		
		public void layout_delete_CHR()
		{
			m_sprite_layout_viewer.delete_selected_CHR();
		}
		
		public void set_mode8x16( bool _on )
		{
			m_sprite_layout_viewer.set_mode8x16( _on );
			m_CHR_bank_viewer.set_mode8x16( _on );
		}
		
		public void chr_transform( CHR_data.e_transform _type )
		{
			m_CHR_bank_viewer.transform_CHR( _type );
		}
		
		public CHR_data_group get_CHR_bank( int _CHR_bank_id )
		{
			return m_CHR_data_storage.get_bank_by_id( _CHR_bank_id );
		}
		
		public List< CHR_data_group > get_CHR_banks()
		{
			return m_CHR_data_storage.get_banks();
		}

		public sprite_data import_CHR_bank( BinaryReader _br, string _name )
		{
			sprite_data spr = new sprite_data( _name );
			
			spr.setup( this.m_CHR_data_storage.create() );

			spr.update_dimensions();
			
			spr.get_CHR_data().import( _br );
			
			return spr; 
		}
		
		public void save_CHR_storage_and_palette( BinaryWriter _bw )
		{
			m_CHR_data_storage.save( _bw );
			m_palette_grp.save( _bw );
		}
		
		public void load_CHR_storage_and_palette( BinaryReader _br, bool _ignore_palette )
		{
			m_CHR_data_storage.load( _br );
			
			if( !_ignore_palette )
			{
				m_palette_grp.load( _br );
			}
		}

		public sprite_data load_sprite( BinaryReader _br )
		{
			string null_name = null;
			sprite_data spr = new sprite_data( null_name );

			spr.load( _br, m_CHR_data_storage );
			
			return spr;
		}
		
		public void on_key_up( object sender, KeyEventArgs e )
		{
			m_palette_grp.on_key_up( sender, e );
			m_CHR_bank_viewer.on_key_up( sender, e );
		}		
		
		public void on_key_down( object sender, PreviewKeyDownEventArgs e )
		{
			//...
		}
		
		public palette_group get_palette_group()
		{
			return m_palette_grp;
		}
		
		public void layout_set_mode( sprite_layout_viewer.e_mode _mode )
		{
			m_sprite_layout_viewer.mode = _mode;
		}
		
		public sprite_layout_viewer.e_mode layout_get_mode()
		{
			return m_sprite_layout_viewer.mode;
		}
		
		public bool CHR_fill_with_color()
		{
			return m_CHR_bank_viewer.CHR_fill_with_color();
		}
		
		public bool CHR_copy()
		{
			return m_CHR_bank_viewer.CHR_copy();
		}
		
		public bool CHR_paste()
		{
			return m_CHR_bank_viewer.CHR_paste();
		}

		public void show_statistics( ListBox.ObjectCollection _sprites )
		{
			int num_sprites 	= _sprites.Count;
			int num_ref_sprites = 0;
			int num_CHR_banks 	= m_CHR_data_storage.get_banks_cnt();
			int num_tiles 		= m_CHR_data_storage.get_num_tiles();
			int tiles_bytes 	= num_tiles * utils.CONST_CHR_NATIVE_SIZE_IN_BYTES;
			
			int i;
			sprite_data spr;
			
			for( i = 0; i < num_sprites; i++ )
			{
				spr = _sprites[ i ] as sprite_data;
				
				if( spr.get_CHR_data().get_link_cnt() > 1 )
				{
					++num_ref_sprites;
				}
			}
			
			MainForm.message_box( String.Format( "Sprites:\t\t{0}\nRef Sprites:\t{1}\nCHR Banks:\t{2}\nTiles:\t\t{3} / {4} Bytes\n------------------------------------\nNative CHR:\t{5} Bytes", num_sprites, num_ref_sprites, num_CHR_banks, num_tiles, tiles_bytes, utils.CONST_CHR_NATIVE_SIZE_IN_BYTES ), "Statistics", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
	}
}