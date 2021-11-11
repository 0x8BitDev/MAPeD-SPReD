/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 03.06.2019
 * Time: 15:55
 */
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting;

namespace SPReD
{
	/// <summary>
	/// Description of py_api.
	/// </summary>
	public class py_api : global::SPSeD.i_py_api
	{
		private ListBox		m_spr_list	= null;
		private CheckBox	m_8x16_mode	= null;
		
		public const string CONST_PREFIX	= "spd_"; 
		
		public py_api( CheckBox _8x16_mode, ListBox _spr_list  ) : base()
		{
			m_8x16_mode	= _8x16_mode;
			m_spr_list	= _spr_list;
		}

		public string get_prefix()
		{
			return CONST_PREFIX;
		}
		
		public void init( ScriptScope _py_scope )
		{
			// Bool 8x16_mode_enabled()
			_py_scope.SetVariable( CONST_PREFIX + "8x16_mode_enabled", new Func< bool >( _8x16_mode_enabled ) );

			// Array[Int32] get_palette( Int32 _plt_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_palette", new Func< int, int[] >( get_palette ) );
			
			// Int32 num_sprites()
			_py_scope.SetVariable( CONST_PREFIX + "num_sprites", new Func< int >( num_sprites ) );

			// sprite_data get_sprite_data( Int32 _spr_ind )
			_py_scope.SetVariable( CONST_PREFIX + "get_sprite_data", new Func< int, spd_sprite_data >( get_sprite_data ) );

			// Long export_CHR_data( Int32 _spr_ind, String _filename, Bool _save_padding )
#if DEF_NES			
			_py_scope.SetVariable( CONST_PREFIX + "export_CHR_data", new Func< int, string, bool, long >( export_CHR_data ) );
#elif DEF_SMS
			_py_scope.SetVariable( CONST_PREFIX + "export_CHR_data", new Func< int, string, int, long >( export_CHR_data ) );
#endif			
			// some sprite related data structures
			_py_scope.SetVariable( CONST_PREFIX + "sprite_data", 	typeof( spd_sprite_data ) );
			_py_scope.SetVariable( CONST_PREFIX + "sprite_attr", 	typeof( spd_sprite_attr ) );
			_py_scope.SetVariable( CONST_PREFIX + "CHR_bank", 		typeof( spd_CHR_bank ) );
			_py_scope.SetVariable( CONST_PREFIX + "CHR_data", 		typeof( spd_CHR_data ) );

			// void msg_box( String _msg, String _caption )
		}

		public void deinit()
		{
			m_spr_list 	= null;
			m_8x16_mode	= null;
		}
		
		public bool _8x16_mode_enabled()
		{
			return m_8x16_mode.Checked;
		}
		
		public int[] get_palette( int _plt_ind )
		{
			if( _plt_ind >= 0 && _plt_ind < utils.CONST_NUM_SMALL_PALETTES )
			{
				int[] arr_copy = new int[ utils.CONST_PALETTE_SMALL_NUM_COLORS ];
				
				palette_group.Instance.get_palettes_arr()[ _plt_ind ].get_color_inds().CopyTo( arr_copy, 0 );
				
				return arr_copy;
			}
			
			return null;
		}
		
		public int num_sprites()
		{
			return m_spr_list.Items.Count;
		}
		
		public spd_sprite_data get_sprite_data( int _spr_ind )
		{
			spd_sprite_data py_data = null;
			
			if( _spr_ind >= 0 && _spr_ind < m_spr_list.Items.Count )
			{
				py_data = new spd_sprite_data( m_spr_list.Items[ _spr_ind ] as sprite_data );
			}
			else
			{
				throw new Exception( "Invalid sprite index ( " + _spr_ind + " )! Use " + CONST_PREFIX + "num_sprites() to get a valid range!" );
			}
			
			return py_data;
		}
		
#if DEF_NES
		public long export_CHR_data( int _spr_ind, string _filename, bool _save_padding )
#elif DEF_SMS
		public long export_CHR_data( int _spr_ind, string _filename, int _bpp )
#endif			
		{
			long data_size = -1;

			try
			{
				if( _spr_ind >= 0 && _spr_ind < m_spr_list.Items.Count )
				{
					sprite_data data = m_spr_list.Items[ _spr_ind ] as sprite_data;
#if DEF_NES					
					data_size = data.get_CHR_data().export( _filename, _save_padding );
#elif DEF_SMS
					if( _bpp < 1 || _bpp > 4 )
					{
						throw new Exception( "Invalid CHRs bpp value! The valid range is 1-4." );
					}
					
					data_size = data.get_CHR_data().export( _filename, false, _bpp );
#endif
				}
				else
				{
					throw new Exception( "Invalid sprite index ( " + _spr_ind + " )! Use " + CONST_PREFIX + "num_sprites() to get a valid range!" );
				}
			}
			catch( Exception _err )
			{
				throw new Exception( CONST_PREFIX + "export_CHR_data error! Can't save CHR data!\n" + _err.Message );
			}
			
			return data_size;
		}
	}
	
	public class spd_sprite_data
	{
		public string	name = "";
		
		public int		offset_x	= 0;
		public int		offset_y	= 0;
		
		public int		width		= 0;
		public int		height		= 0;
		
		public spd_sprite_attr[] 	attrs		= null;
		public spd_CHR_bank			CHR_bank	= null;
		
		public spd_sprite_data( sprite_data _data )
		{
			name 		= _data.name;
			
			offset_x 	= _data.offset_x;
			offset_y 	= _data.offset_y;
			
			width 		= _data.size_x;
			height 		= _data.size_y;
			
			int num_attrs = _data.get_CHR_attr().Count;
			
			if( num_attrs > 0 )
			{
				attrs = new spd_sprite_attr[ num_attrs ];
				
				for( int i = 0; i < num_attrs; i++ )
				{
					attrs[ i ] = new spd_sprite_attr( _data.get_CHR_attr()[ i ] );
				}
			}
			
			if( _data.get_CHR_data() != null )
			{
				CHR_bank = new spd_CHR_bank( _data.get_CHR_data() );
			}
		}
	}

	public class spd_sprite_attr
	{
		public int	x 			= 0;
		public int	y 			= 0;
		
		public int	palette_ind = -1;
		public int	CHR_ind		= -1;
		
		// HFLIP = 0x01; VFLIP = 0x02
		public int	flip_flag	= 0; 

		public spd_sprite_attr( CHR_data_attr _data )
		{
			x = _data.x;
			y = _data.y;
			
			palette_ind = _data.palette_ind;
			CHR_ind		= _data.CHR_ind;
			flip_flag	= _data.flip_flag;			
		}
	}
	
	public class spd_CHR_bank
	{
		public int	id			= -1;
		public int	link_cnt	= 0;
		
		public spd_CHR_data[] CHR_data	= null;
		
		public spd_CHR_bank( CHR_data_group _data )
		{
			id 			= _data.id;
			link_cnt	= _data.get_link_cnt();
			
			int num_CHRs = _data.get_data().Count; 
			
			if( num_CHRs > 0 )
			{
				CHR_data = new spd_CHR_data[ num_CHRs ];
				
				for( int i = 0; i < num_CHRs; i++ )
				{
					CHR_data[ i ] = new spd_CHR_data( _data.get_data()[ i ] );
				}
			}
		}
	}
	
	public class spd_CHR_data
	{
		public byte[] data	= null;
		
		public spd_CHR_data( CHR8x8_data _data )
		{
			data = new byte[ utils.CONST_CHR8x8_TOTAL_PIXELS_CNT ];			
			Array.Copy( _data.get_data(), data, utils.CONST_CHR8x8_TOTAL_PIXELS_CNT );
		}
	}
}
