/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 14.03.2017
 * Time: 11:35
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SPReD
{
	/// <summary>
	/// Description of sprite_data.
	/// </summary>
	public class sprite_data
	{
		public enum EAxesFlipType
		{
			aft_LOCAL_AXES = 0,
			aft_GLOABL_AXES,
		};
#if DEF_NES
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
#endif
		private string 					m_name 				= null;
		private int						m_offset_x			= 0;
		private int						m_offset_y			= 0;
		private int						m_size_x			= 0;
		private int						m_size_y			= 0;
		private CHR_data_group			m_CHR_data			= null;
		private List< CHR_data_attr	>	m_CHR_attr			= null;
		
		public int size_x
		{
			get { return m_size_x; }
			set {}
		}

		public int size_y
		{
			get { return m_size_y; }
			set {}
		}

		public int offset_x
		{
			get { return m_offset_x; }
			set 
			{
				m_offset_x = value; 
				
				if( m_offset_x + size_x > utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE )
				{
					m_offset_x = utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE - size_x;
				}
				
				if( m_offset_x < -utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE )
				{
					m_offset_x = -utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE;
				}
			}
		}
		
		public int offset_y
		{
			get { return m_offset_y; }
			set 
			{ 
				m_offset_y = value;

				if( m_offset_y + size_y > utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE )
				{
					m_offset_y = utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE - size_y;
				}
				
				if( m_offset_y < -utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE )
				{
					m_offset_y = -utils.CONST_LAYOUT_WORKSPACE_HALF_SIDE;
				}
			}
		}
		
		public string name
		{
			get { return m_name; }
			set { m_name = value; }
		}
		
		public sprite_data( string _name )
		{
			m_name = _name;
			
			m_CHR_attr = new List< CHR_data_attr >( 100 );
		}

		public static sprite_data create( string _name )
		{
			return new sprite_data( _name );
		}
		
		public void reset()
		{
			name = null;
			
			m_CHR_data.unlink();
			m_CHR_data 	= null;
			
			m_CHR_attr.Clear();
			m_CHR_attr = null;
		}
		
		public override string ToString()
		{
			return name;
		}
		
		public CHR_data_group get_CHR_data()
		{
			return m_CHR_data;
		}
		
		public List< CHR_data_attr > get_CHR_attr()
		{
			return m_CHR_attr;
		}
		
		public void setup( sprite_params _params )
		{
			m_offset_x 			= _params.m_offset_x;
			m_offset_y 			= _params.m_offset_y;
			m_size_x			= _params.m_size_x;
			m_size_y			= _params.m_size_y;
			m_CHR_data			= _params.m_CHR_data;
			m_CHR_attr			= _params.m_CHR_attr;
			
			m_CHR_data.link();
		}
		
		public sprite_data copy( string _name, CHR_data_group _chr_data_group, List< CHR_data_attr > _chr_attrs )
		{
			sprite_params spr_params;
			
			spr_params.m_CHR_data 		= ( _chr_data_group != null ) ? _chr_data_group:m_CHR_data;
			spr_params.m_offset_x 		= m_offset_x;
			spr_params.m_offset_y 		= m_offset_y;
			spr_params.m_size_x 		= m_size_x;
			spr_params.m_size_y 		= m_size_y;
			spr_params.m_CHR_attr 		= new List<CHR_data_attr>( m_CHR_attr.Count );
			
			if( _chr_attrs == null )
			{
				m_CHR_attr.ForEach( delegate( CHR_data_attr _attr ) { spr_params.m_CHR_attr.Add( _attr.copy() ); } );
			}
			else
			{
				spr_params.m_CHR_attr = _chr_attrs;
			}
			
			sprite_data spr = new sprite_data( _name );
			
			spr.setup( spr_params );
			
			return spr;
		}
		
		public void set_CHR_data( CHR_data_group _new_data )
		{
			m_CHR_data.unlink();
			
			m_CHR_data = _new_data;
			
			m_CHR_data.link();
		}
		
#if DEF_NES		
		public void flip_vert( EAxesFlipType _ft, bool _8x16_mode )
#elif DEF_SMS
		public void flip_vert( EAxesFlipType _ft, bool _transform_pos, bool _8x16_mode )
#elif DEF_PCE
		public void flip_vert( EAxesFlipType _ft )
#endif			
		{
			m_CHR_attr.ForEach( delegate( CHR_data_attr _attr ) 
			{
#if DEF_NES	|| DEF_PCE
				_attr.vflip();
#elif DEF_SMS
				m_CHR_data.get_data()[ _attr.CHR_ind ].transform( CHR_data.ETransform.t_vflip );
				
				if( _8x16_mode && _attr.CHR_ind + 1 < m_CHR_data.get_data().Count )
				{
					m_CHR_data.get_data()[ _attr.CHR_ind + 1 ].transform( CHR_data.ETransform.t_vflip );
					
					m_CHR_data.swap_CHRs( _attr.CHR_ind, _attr.CHR_ind + 1 );
				}

				if( _transform_pos )
#endif
				{
					switch( _ft )
					{
						case sprite_data.EAxesFlipType.aft_LOCAL_AXES:
							{
								int center = m_size_y >> 1;
								_attr.y = center - ( _attr.y - center ) - utils.CONST_CHR_SIDE_PIXELS_CNT;
							}
							break;
							
						case sprite_data.EAxesFlipType.aft_GLOABL_AXES:
							{
								_attr.y = ( -_attr.y - utils.CONST_CHR_SIDE_PIXELS_CNT ) - m_offset_y;
#if !DEF_PCE
							 	if( _8x16_mode )
							 	{
							 		_attr.y -= utils.CONST_CHR_SIDE_PIXELS_CNT;
							 	}
#endif
							}
							break;
					}
				}
			} );

#if DEF_SMS
			if( _transform_pos )
#endif
			{
				if( _ft == sprite_data.EAxesFlipType.aft_GLOABL_AXES )
				{
					// find a minimal Y value
					int min_y = int.MaxValue;
					
					m_CHR_attr.ForEach( delegate( CHR_data_attr _attr )
					{
						if( min_y > _attr.y )
						{
							min_y = _attr.y;
						}
					} );
					
					// attr.y -= min_y
					m_CHR_attr.ForEach( delegate( CHR_data_attr _attr )
					{
					 	_attr.y -= min_y;
					} );
					
					m_offset_y = min_y;
				}
			}
		}
		
#if DEF_NES		
		public void flip_horiz( EAxesFlipType _ft )
#elif DEF_SMS
		public void flip_horiz( EAxesFlipType _ft, bool _transform_pos, bool _8x16_mode )
#elif DEF_PCE
		public void flip_horiz( EAxesFlipType _ft )
#endif			
		{
			m_CHR_attr.ForEach( delegate( CHR_data_attr _attr ) 
			{
#if DEF_NES || DEF_PCE
				_attr.hflip();
#elif DEF_SMS
				m_CHR_data.get_data()[ _attr.CHR_ind ].transform( CHR_data.ETransform.t_hflip );

				if( _8x16_mode && _attr.CHR_ind + 1 < m_CHR_data.get_data().Count )
				{
					m_CHR_data.get_data()[ _attr.CHR_ind + 1 ].transform( CHR_data.ETransform.t_hflip );
				}
				
				if( _transform_pos )
#endif			
				{
					switch( _ft )
					{
						case sprite_data.EAxesFlipType.aft_LOCAL_AXES:
							{
								int center = m_size_x >> 1;
								_attr.x = center - ( _attr.x - center ) - utils.CONST_CHR_SIDE_PIXELS_CNT;
							}
							break;
							
						case sprite_data.EAxesFlipType.aft_GLOABL_AXES:
							{
								_attr.x = ( -_attr.x - utils.CONST_CHR_SIDE_PIXELS_CNT ) - m_offset_x;
							}
							break;
					}
				}
			} );
			
#if DEF_SMS
			if( _transform_pos )
#endif
			{
				if( _ft == sprite_data.EAxesFlipType.aft_GLOABL_AXES )
				{
					// find a minimal X value
					int min_x = int.MaxValue;
					
					m_CHR_attr.ForEach( delegate( CHR_data_attr _attr )
					{
						if( min_x > _attr.x )
						{
							min_x = _attr.x;
						}
					} );
					
					// attr.x -= min_X
					m_CHR_attr.ForEach( delegate( CHR_data_attr _attr )
					{
					 	_attr.x -= min_x;
					} );
					
					m_offset_x = min_x;
				}
			}
		}
#if DEF_NES
		public void shift_colors( bool _shift_transp, bool _mode8x16 )
		{
			int i;
			int size = m_CHR_attr.Count;
			
			CHR_data_attr 	chr_attr;

			++clr_ind_remap_arr_pos;
			clr_ind_remap_arr_pos %= _shift_transp ? ( clr_ind_remap_transp_arr.Length >> 2 ):( clr_ind_remap_arr.Length >> 2 );
			
			byte[] remap_arr = _shift_transp ? clr_ind_remap_transp_arr:clr_ind_remap_arr;
			
			int start_clr_ind = clr_ind_remap_arr_pos << 2;
			
			for( i = 0; i < size; i++ )
			{
				chr_attr = m_CHR_attr[ i ];
				
				remap_color_inds( get_CHR_data().get_data()[ chr_attr.CHR_ind ], start_clr_ind, remap_arr );
				
				if( _mode8x16 && ( chr_attr.CHR_ind + 1 < get_CHR_data().get_data().Count ) )
				{
					remap_color_inds( get_CHR_data().get_data()[ chr_attr.CHR_ind + 1 ], start_clr_ind, remap_arr );
				}
			}
		}
		
		private void remap_color_inds( CHR_data _chr_data, int _start_clr_ind, byte[] _remap_arr )
		{
			int 	j;
			byte 	clr_ind;
			
			for( j = 0; j < utils.CONST_CHR_TOTAL_PIXELS_CNT; j++ )
			{
				clr_ind = _chr_data.get_data()[ j ];
				
				clr_ind = _remap_arr[ _start_clr_ind + clr_ind ];
					
				_chr_data.get_data()[ j ] = clr_ind;
			}
		}
#endif
		public void validate()
		{
			// all attributes must refer to existing CHRs
			for( int i = 0; i < get_CHR_attr().Count; i++ )
			{
				if( get_CHR_attr()[ i ].CHR_ind >= get_CHR_data().get_data().Count )
				{
					get_CHR_attr().RemoveAt( i );
					--i;
				}
			}
				
			update_dimensions();
		}
		
		public bool check( bool _8x16_mode, string _wnd_title )
		{
			bool even_chr_inds = true;
			bool diff_chr_inds = false;
			
			int attr_cnt = get_CHR_attr().Count;
			
			int chr_ind = ( attr_cnt > 0 ) ? get_CHR_attr()[ 0 ].CHR_ind:-1;
			int attr_CHR_ind = -1;
			
			for( int attr_n = 0; attr_n < attr_cnt; attr_n++ )
			{
				attr_CHR_ind = get_CHR_attr()[ attr_n ].CHR_ind;
				
				if( _8x16_mode )
				{
					if( ( attr_CHR_ind & 0x01 ) == 0x01 )
					{
						MainForm.message_box( "The sprite '" + name + "' has invalid data for the 8x16 mode!\n\nYou can't mix the 8x8 and 8x16 mode sprites in one project!\nPlease, check your sprites!\n\nOperation aborted!", _wnd_title + " [data validation]", MessageBoxButtons.OK, MessageBoxIcon.Error );
						
						return false;
					}
				}
				else
				{
					if( ( attr_CHR_ind & 0x01 ) == 0x01 )
					{
						even_chr_inds = false;
					}
					
					if( chr_ind != attr_CHR_ind )
					{
						chr_ind = attr_CHR_ind;
						
						diff_chr_inds = true;
					}
				}
			}
			
			if( !_8x16_mode && even_chr_inds && diff_chr_inds && attr_cnt >= 4 )
			{
				if( MainForm.message_box( "It seems like the sprite '" + name + "' was created using the 8x16 mode!\n\nYou can't mix the 8x8 and 8x16 mode sprites in one project!\nPlease, check your sprites!\n\nAbort operation?", _wnd_title + " [data validation]", MessageBoxButtons.YesNo, MessageBoxIcon.Error ) == DialogResult.Yes )
				{
					return false;
				}
			}
			
			/*
			if( ( get_CHR_data().get_data().Count & 0x01 ) == 0x01 )
			{
				MainForm.message_box( "The CHR bank [" + ( get_CHR_data().id + 1 ) +  "] data isn't compatible with the 8x16 mode!\n\nThe number of CHRs in a CHR bank for the 8x16 mode must be a multiple of two!\n\nPlease, check your project!\n\nOperation aborted!", _wnd_title + " [data validation]", MessageBoxButtons.OK, MessageBoxIcon.Error );
				
				return false;
			}*/
			
			return true;
		}
		
		public bool is_packed( bool _mode8x16 )
		{
			return get_CHR_data().get_data().Count > get_CHR_attr().Count * ( _mode8x16 ? 2:1 );
		}
		
		public bool merge_CHR( sprite_data _spr, SPReD.CHR_data_group.ECHRPackingType _packing_type, bool _mode8x16 )
		{
			// check if data were already packed
			if( _spr.is_packed( _mode8x16 ) )
			{
				return false;
			}

			int last_CHR_cnt = get_CHR_data().get_data().Count;
			
			CHR_data_group spr_chr_data = _spr.get_CHR_data();
			get_CHR_data().add_data_range( spr_chr_data );
			
			int i;
			int size = _spr.get_CHR_attr().Count;
			
			for( i = 0; i < size; i++ )
			{
				_spr.get_CHR_attr()[ i ].CHR_ind += last_CHR_cnt; 
			}
			
			_spr.set_CHR_data( this.get_CHR_data() );
			
			return true;
		}

		public bool relink_CHR_data( CHR_data_group _chr_data, int _old_chr_id )
		{
			if( _old_chr_id == get_CHR_data().id )
			{
				set_CHR_data( _chr_data );
				
				return true;
			}
			
			return false;
		}
		
		public bool split_CHR( bool _mode8x16 )
		{
			// create a new bank and transfer all sprite data from the common bank to it
			CHR_data_group new_chr_data = new CHR_data_group();
			
			CHR_data_attr attr;
			
			int size = m_CHR_attr.Count;
			
			for( int i = 0; i < size; i++ )
			{
				attr = m_CHR_attr[ i ];
				
				new_chr_data.get_data().Add( m_CHR_data.get_data()[ attr.CHR_ind ].copy() );
				
				if( _mode8x16 && attr.CHR_ind + 1 < m_CHR_data.get_data().Count )
				{
					new_chr_data.get_data().Add( m_CHR_data.get_data()[ attr.CHR_ind + 1 ].copy() );
					
					attr.CHR_ind = i << 1;
				}
				else
				{
					attr.CHR_ind = i;
				}
			}
			
			set_CHR_data( new_chr_data );
			
			return true;
		}

#if DEF_NES		
		public void export( StreamWriter _sw, string _data_prefix )
#elif DEF_SMS
		public void export( StreamWriter _sw, int _CHRs_offset, string _data_prefix )
#elif DEF_PCE
		public void export( StreamWriter _sw, int _CHRs_offset, int _palette_slot, string _data_prefix )
#else
...
#endif
		{
			int size = this.m_CHR_attr.Count;
			
			CHR_data_attr chr_attr;
			
			if( size > utils.CONST_SPRITE_MAX_NUM_ATTRS )
			{
				throw new Exception( "The sprite - " + name + " - has more than " + utils.CONST_SPRITE_MAX_NUM_ATTRS.ToString() + " tiles that exceed the hardware limit!\n Please, fix it to avoid the sprite drawing error in your project!" );
			}
			
			_sw.WriteLine( _data_prefix + name + ":" );
			
			for( int i = 0; i < size; i++ )
			{
				chr_attr = m_CHR_attr[ i ];
#if DEF_NES				
				if( chr_attr.palette_ind < 0 )
				{
					throw new Exception( "The sprite: " + name + " has uncolored part(s)!" );
				}
		
				int attr  = chr_attr.palette_ind; 
				attr |= ( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 ) ? 0x40:0;
				attr |= ( ( chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 ) ? 0x80:0;
				
				_sw.WriteLine( "\t.byte " + String.Format( "${0:X2}, ${1:X2}, ${2:X2}, ${3:X2}", unchecked( ( byte )( offset_y + chr_attr.y ) ), unchecked( ( byte )( chr_attr.CHR_ind ) ), unchecked( ( byte )( attr ) ), unchecked( ( byte )( offset_x + chr_attr.x ) ) ) );
#elif DEF_SMS	
				if( chr_attr.CHR_ind + _CHRs_offset > utils.CONST_CHR_BANK_MAX_SPRITES_CNT - 1 )
				{
					throw new Exception( "CHRs indices overflow! Invalid CHRs offset value!" );
				}

				_sw.WriteLine( "\t.byte " + String.Format( "${0:X2}, ${1:X2}, ${2:X2}", unchecked( ( byte )( offset_y + chr_attr.y ) ), unchecked( ( byte )( offset_x + chr_attr.x ) ), unchecked( ( byte )( chr_attr.CHR_ind + _CHRs_offset ) ) ) );
#elif DEF_PCE
				int CHR_ind_offset = 0;
				int cgx_cgy	= PCE_metasprite_exporter.get_CGX_CGY_flags( this, ref CHR_ind_offset );

				PCE_metasprite_exporter.save_attribute( _sw, this, chr_attr, cgx_cgy, _CHRs_offset, CHR_ind_offset, _palette_slot );
				
				if( cgx_cgy != 0 )
				{
					break;
				}
#else
...
#endif
			}
			
			_sw.WriteLine( _data_prefix + name + "_end:\n" );
		}
#if DEF_PCE
		public bool check_16x32_64_mode()
		{
			return m_name.Contains( "16x32_0" ) || m_name.Contains( "16x32_1" ) || m_name.Contains( "16x64_0" ) || m_name.Contains( "16x64_1" );
		}
#endif
		public Rectangle get_rect()
		{
			Rectangle rect_src = new Rectangle( 0, 0, 0, 0 );
			
			if( get_CHR_attr().Count > 0 )
			{
				CHR_data_attr chr_attr = get_CHR_attr()[ 0 ];
				
				Rectangle rect_dst = new Rectangle();
				
				rect_src.X = chr_attr.x + offset_x;
				rect_src.Y = chr_attr.y + offset_y;
				rect_src.Width	= utils.CONST_CHR_SIDE_PIXELS_CNT;
				rect_src.Height = utils.CONST_CHR_SIDE_PIXELS_CNT;
	
				rect_dst.Width 	= utils.CONST_CHR_SIDE_PIXELS_CNT;
				rect_dst.Height = utils.CONST_CHR_SIDE_PIXELS_CNT;
				
				int size = get_CHR_attr().Count;
				
				for( int i = 1; i < size; i++ )
				{
					chr_attr = get_CHR_attr()[ i ];
					
					rect_dst.X	= chr_attr.x + offset_x;
					rect_dst.Y	= chr_attr.y + offset_y;
					
					rect_src = Rectangle.Union( rect_src, rect_dst );
				}
			}
			
			return rect_src;
		}
		
		public void update_dimensions()
		{
			if( get_CHR_attr().Count > 0 )
			{
				Rectangle rect = get_rect();
			
				if( rect.IsEmpty == false )
				{
					m_size_x = rect.Width;
					m_size_y = rect.Height;
				}
				
				return;
			}			
			
			m_size_x = 0;
			m_size_y = 0;
		}
		
		public void save_image_BMP_PNG( string _path, bool _save_alpha, palette_small[] _plt_arr, image_export_options_form.EImgFormat _fmt, bool _mode8x16 )
		{
			Bitmap 		bmp;
			Bitmap 		draw_img;
			Graphics	gfx;
			
			CHR_data_attr 	chr_attr;
			CHR_data		chr_data;
			
			int x;
			int y;

			Rectangle rect = get_rect();
			
			if( _mode8x16 )
			{
				rect.Height += utils.CONST_CHR_SIDE_PIXELS_CNT;
			}
			
			Color opacity_color = Color.FromArgb( ( 0xff << 24 ) | palette_group.Instance.main_palette[ _plt_arr[ 0 ].get_color_inds()[ 0 ] ] );
			
			draw_img = new Bitmap( rect.Width, rect.Height, PixelFormat.Format32bppArgb );
			gfx = Graphics.FromImage( draw_img );
			gfx.Clear( opacity_color );
			
			int size = get_CHR_attr().Count;
			
			for( int i = 0; i < size; i++ )
			{
				chr_attr = get_CHR_attr()[ i ];
				chr_data = get_CHR_data().get_data()[ chr_attr.CHR_ind ];
				
				if( _mode8x16 )
				{
					bmp = utils.create_bitmap8x16( chr_data, ( chr_attr.CHR_ind + 1 ) >= get_CHR_data().get_data().Count ? null:get_CHR_data().get_data()[ chr_attr.CHR_ind + 1 ], chr_attr, false, chr_attr.palette_ind, _plt_arr );
				}
				else
				{
					bmp = utils.create_CHR_bitmap( chr_data, chr_attr, false, chr_attr.palette_ind, _plt_arr );
				}
				
				x = chr_attr.x - rect.X + offset_x;
				y = chr_attr.y - rect.Y + offset_y;
				
				gfx.DrawImage( bmp, x, y, utils.CONST_CHR_SIDE_PIXELS_CNT, _mode8x16 ? ( utils.CONST_CHR_SIDE_PIXELS_CNT << 1 ):utils.CONST_CHR_SIDE_PIXELS_CNT );
				
				bmp.Dispose();
			}
			
			if( _save_alpha )
			{
				Color alpha_color = Color.FromArgb( opacity_color.ToArgb() & 0x00ffffff );
				
				for( y = 0; y < rect.Height; y++ )
				{
					for( x = 0; x < rect.Width; x++ )
					{
						if( draw_img.GetPixel( x, y ) == opacity_color )
						{
							draw_img.SetPixel( x, y, alpha_color );
						}
					}
				}
			}
			
			if( _fmt == image_export_options_form.EImgFormat.BMP )
			{
				draw_img.Save( _path + Path.DirectorySeparatorChar + name + ".bmp", ImageFormat.Bmp );
			}
			else
			if( _fmt == image_export_options_form.EImgFormat.PNG )
			{
				draw_img.Save( _path + Path.DirectorySeparatorChar + name + ".png", ImageFormat.Png );
			}
			else
			{
				draw_img.Dispose();
				
				throw new Exception( "Unsupported image format: " + _fmt.ToString() );
			}
			
			draw_img.Dispose();
		}
		
		public void save_image_PCX( string _path, palette_small[] _plt_arr, bool _mode8x16 )
		{
			CHR_data_attr 	chr_attr;
			CHR_data		chr_data;
			
			int x;
			int y;
			
			Rectangle rect = get_rect();
			
			if( _mode8x16 )
			{
				rect.Height += utils.CONST_CHR_SIDE_PIXELS_CNT;
			}
			
			byte[] img_data = new byte[ rect.Width * rect.Height ];
			Array.Clear( img_data, 0, img_data.Length );
			
			int size = get_CHR_attr().Count;
			
			for( int i = 0; i < size; i++ )
			{
				chr_attr = get_CHR_attr()[ i ];
				chr_data = get_CHR_data().get_data()[ chr_attr.CHR_ind ];
				
				x = chr_attr.x - rect.X + offset_x;
				y = chr_attr.y - rect.Y + offset_y;
				
				PCX_fill_CHR_area( x, y, chr_attr, chr_data, ref img_data, rect.Width );
				
				if( _mode8x16 )
				{
					if( ( chr_attr.CHR_ind + 1 ) < get_CHR_data().get_data().Count )
					{
						PCX_fill_CHR_area( x, y + utils.CONST_CHR_SIDE_PIXELS_CNT, chr_attr, get_CHR_data().get_data()[ chr_attr.CHR_ind + 1 ], ref img_data, rect.Width );
					}
				}
			}
			
			// save data
			{
				FileStream		fs = new FileStream( _path + Path.DirectorySeparatorChar + name + ".pcx", FileMode.Create, FileAccess.Write );
				BinaryWriter	bw = new BinaryWriter( fs );
				
				int plt_clr;
				
				int img_width	= rect.Width - 1;
				int img_height	= rect.Height - 1;
				
				byte[] PCX_header =
				{
					0x0A,			// "PCX File"
					0x05,			// "Version 5"
					0x01,			// RLE Encoding
					0x08,			// 8 bit per pixel
					0x00, 0x00,		// XStart at 0
					0x00, 0x00,		// YStart at 0
					( byte )( img_width & 0xFF ), ( byte )( ( img_width >> 8 ) & 0xFF ),	// Xend
					( byte )( img_height & 0xFF ), ( byte )( ( img_height >> 8 ) & 0xFF ),	// Yend
					0x48, 0,		// Horizontal DPI
					0x48, 0,		// Vertical DPI
					0x0F, 0x0F, 0x0F, 0x0E, 0x0E, 0x0E, 0x0D, 0x0D, 0x0D, 0x0C, 0x0C, 0x0C,	//48-byte EGA palette info
					0x0B, 0x0B, 0x0B, 0x0A, 0x0A, 0x0A, 0x09, 0x09, 0x09, 0x08, 0x08, 0x08,
					0x07, 0x07, 0x07, 0x06, 0x06, 0x06, 0x05, 0x05, 0x05, 0x04, 0x04, 0x04,
					0x03, 0x03, 0x03, 0x02, 0x02, 0x02, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00,
					0x00,			// Reserved byte, always x00
					0x01,			// 1 bit plane
					( byte )( rect.Width & 0xFF ), ( byte )( ( rect.Width >> 8 ) & 0xFF ),	// Bytes per scan line: rect.Width
					0x01, 0x00,		// Palette type: 1 means color or monochrome
					0x00, 0x00,		// Horizontal screen size (not used)
					0x00, 0x00		// Vertical screen size (not used)
				};
				
				// 128 bytes header data
				bw.Write( PCX_header );
				bw.Write( new byte[ 54 ] );
				
				if( PCX_header[ 2 ] != 0 )
				{
					// compress and save image data
					for( int i = 0; i < rect.Height; i++ )
					{
						PCX_encode_and_save_line( img_data, i * rect.Width, rect.Width, bw );
					}
					bw.Write( img_data );
				}
				else
				{
					// uncompressed image data
					bw.Write( img_data );
				}
				
				// palette data flag
				bw.Write( ( byte )0x0c );
				
#if DEF_FIXED_LEN_PALETTE16_ARR
				palettes_array plts_arr = palettes_array.Instance;
				
				for( int plt_n = 0; plt_n < utils.CONST_PALETTE16_ARR_LEN; plt_n++ )
				{
					for( int clr_n = 0; clr_n < 16; clr_n++ )
					{
						plt_clr = palette_group.Instance.main_palette[ plts_arr.get_color( plt_n, clr_n ) ];
						
						bw.Write( ( byte )( ( plt_clr >> 16 ) & 0xff ) );
						bw.Write( ( byte )( ( plt_clr >> 8 ) & 0xff ) );
						bw.Write( ( byte )( plt_clr & 0xff ) );
					}
				}
#else
				for( int clr_n = 0; clr_n < ( utils.CONST_NUM_SMALL_PALETTES * utils.CONST_PALETTE_SMALL_NUM_COLORS ); clr_n++ )
				{
					plt_clr = palette_group.Instance.main_palette[ _plt_arr[ clr_n >> 2 ].get_color_inds()[ clr_n & 0x03 ] ];
					
					bw.Write( ( byte )( ( plt_clr >> 16 ) & 0xff ) );
					bw.Write( ( byte )( ( plt_clr >> 8 ) & 0xff ) );
					bw.Write( ( byte )( plt_clr & 0xff ) );
				}
				
				bw.Write( new byte[ 16 * 3 * 15 ] );
#endif
				bw.Dispose();
				fs.Dispose();
			}
		}

		private void PCX_fill_CHR_area( int _x, int _y, CHR_data_attr _chr_attr, CHR_data _chr_data, ref byte[] _img_buff, int _img_width )
		{
			int pix_x_pos;
			int pix_y_pos;
			
			for( int pix_h = 0; pix_h < utils.CONST_CHR_SIDE_PIXELS_CNT; pix_h++ )
			{
				for( int pix_w = 0; pix_w < utils.CONST_CHR_SIDE_PIXELS_CNT; pix_w++ )
				{
					pix_x_pos = pix_w;
					pix_y_pos = pix_h;
					
					if( ( _chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_HFLIP ) != 0 )
					{
						pix_x_pos = utils.CONST_CHR_SIDE_PIXELS_CNT - pix_x_pos - 1;
					}
					
					if( ( _chr_attr.flip_flag & CHR_data_attr.CONST_CHR_ATTR_FLAG_VFLIP ) != 0 )
					{
						pix_y_pos = utils.CONST_CHR_SIDE_PIXELS_CNT - pix_y_pos - 1;
					}
#if DEF_FIXED_LEN_PALETTE16_ARR
					_img_buff[ ( _x + pix_w ) + ( _y + pix_h ) * _img_width ] = ( byte )( _chr_data.get_data()[ pix_x_pos + ( pix_y_pos * utils.CONST_CHR_SIDE_PIXELS_CNT ) ] + ( _chr_attr.palette_ind << 4 ) );
#else
					_img_buff[ ( _x + pix_w ) + ( _y + pix_h ) * _img_width ] = ( byte )( _chr_data.get_data()[ pix_x_pos + ( pix_y_pos * utils.CONST_CHR_SIDE_PIXELS_CNT ) ] + ( _chr_attr.palette_ind << 2 ) );
#endif
				}
			}
		}
		
		private void PCX_encode_and_save_line( byte[] _img_buff, int _buff_pos, int _img_width, BinaryWriter _bw )
		{
			byte	curr, last;
			byte	run_cnt;	// max value 63
			
			last	= _img_buff[ _buff_pos ];
			run_cnt	= 1;
			
			for( int src_ind = 1; src_ind < _img_width; src_ind++ )
			{
				curr = _img_buff[ ++_buff_pos ];
				
				if( curr == last )
				{
					run_cnt++;
					
					if( run_cnt == 63 )
					{
						PCX_save_encoded_data( last, run_cnt, _bw );
						run_cnt = 0;
					}
				}
				else
				{
					if( run_cnt != 0 )
					{
						PCX_save_encoded_data( last, run_cnt, _bw );
					}
					
					last = curr;
					run_cnt = 1;
				}
			}
			
			if( run_cnt != 0 )
			{
				PCX_save_encoded_data( last, run_cnt, _bw );
			}
		}
		
		private int PCX_save_encoded_data( byte _data, byte _cnt, BinaryWriter _bw )
		{
			if( _cnt != 0 )
			{
				if( ( _cnt == 1 ) && ( 0xc0 != ( 0xc0 & _data ) ) )
				{
					_bw.Write( _data );
					
					return 1;
				}
				else
				{
					_bw.Write( ( byte )( 0xC0 | _cnt ) );
					_bw.Write( _data );
					
					return 2;
				}
			}
			
			throw new Exception( "PCX image encoding error!" );
		}

		public void save( BinaryWriter _bw )
		{
			update_dimensions();
			
			_bw.Write( name );
			
			_bw.Write( m_offset_x		);
			_bw.Write( m_offset_y		);
			_bw.Write( m_size_x			);
			_bw.Write( m_size_y			);
			_bw.Write( m_CHR_data.id	);
			
			if( m_CHR_attr != null )
			{
				int attrs_cnt = m_CHR_attr.Count;
				
				_bw.Write( attrs_cnt );
				
				for( int i = 0; i < attrs_cnt; i++ )
				{
					m_CHR_attr[ i ].save( _bw );
				}
			}
			else
			{
				_bw.Write( ( int )( 0 ) );
			}
			
			// reserved data for future purposes
			int reserved = 0;
			_bw.Write( reserved );
		}
		
		public void load( BinaryReader _br, CHR_data_storage _chr_storage )
		{
			m_name = _br.ReadString();
			
			m_offset_x			= _br.ReadInt32();
			m_offset_y			= _br.ReadInt32();
			m_size_x			= _br.ReadInt32();
			m_size_y			= _br.ReadInt32();
			
			int CHR_data_id		= _br.ReadInt32();
			
			CHR_data_group chr_data = _chr_storage.get_bank_by_id( CHR_data_id );
			
			if( chr_data != null )
			{
				m_CHR_data = chr_data;
			}
			else
			{
				throw new Exception( "Can't find CHR data [id: " + CHR_data_id + "]!/nThe sprite name is '" + name + "'" );
			}
			
			CHR_data_attr chr_attr;
			
			int attrs_cnt = _br.ReadInt32();
			
			for( int i = 0; i < attrs_cnt; i++ )
			{
				chr_attr = new CHR_data_attr( 0, 0 );
				chr_attr.load( _br );
				
				m_CHR_attr.Add( chr_attr );
			}
			
			// reserved data for future purposes
			int reserved = _br.ReadInt32();
		}
	}
}
