/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 12.12.2018
 * Time: 18:27
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace MAPeD
{
	/// <summary>
	/// Description of entity_data.
	/// </summary>
	/// 
	
	[DataContract]
	public class entity_instance
	{
		private string m_properties	= "";
		
		private int m_x = 0;
		private int m_y = 0;
		
		private entity_data	m_base	= null;
		
		private int m_uid			= -1;
		private int m_target_uid	= -1;
		
		private static int __instances_cnt	= 0;
		
		[DataMember]
		public int uid
		{
			get { return m_uid; }
			set {}
		}
		
		[DataMember]
		public int target_uid
		{
			get { return m_target_uid; }
			set { m_target_uid = value; }
		}
		
		public static void reset_instances_counter() { __instances_cnt = 0; }
		
		public static void load_instances_counter( BinaryReader _br ) { __instances_cnt = _br.ReadInt32(); }
		public static void save_instances_counter( BinaryWriter _bw ) { _bw.Write( __instances_cnt ); }
		
		public string name
		{
			get { return "Instance" + uid; }
			set {}
		}

		[DataMember]
		public string properties
		{
			get { return m_properties; }
			set { m_properties = value; }
		}
		
		[DataMember]
		public int x
		{
			get { return m_x; }
			set { m_x = value; }
		}
		
		[DataMember]
		public int y
		{
			get { return m_y; }
			set { m_y = value; }
		}
		
		[DataMember]
		public entity_data base_entity
		{
			get { return m_base; }
			set { m_base = value; }
		}

		public entity_instance( string _prop, int _x, int _y, entity_data _base )
		{
			properties 	= _prop;
			
			x = _x;
			y = _y;
			
			base_entity = _base;
			
			m_uid = ++__instances_cnt;
		}

		public entity_instance()
		{
			m_uid = ++__instances_cnt;
		}
		
		public void reset()
		{
			properties 	= null;
			base_entity = null;
		}
		
		public entity_instance copy()
		{
			return new entity_instance( properties, x, y, base_entity );
		}
		
		public void save( BinaryWriter _bw )
		{
		    _bw.Write( base_entity.name );
		    _bw.Write( properties );
		    _bw.Write( x );
		    _bw.Write( y );
		    _bw.Write( uid );
		    _bw.Write( target_uid );
		    
			// extra user defined data ( reserved for future purposes )
			int extra_data_size = 0;
			_bw.Write( extra_data_size );
		}
		
		public void load(	BinaryReader 						_br,
							byte								_ver,
							Func< string, entity_data > 		_get_ent, 
							data_sets_manager.EScreenDataType 	_scr_type )
		{
			string base_ent_name;
			
			base_ent_name 	= _br.ReadString();
			base_entity		= _get_ent( base_ent_name );
			
			if( base_entity == null )
			{
				throw new Exception( "Can't find base entity: " + base_ent_name + "\n\nEntity instance loading error!" );
			}
			
			properties	= _br.ReadString();
			x 			= _br.ReadInt32();
			y 			= _br.ReadInt32();
			m_uid 		= _br.ReadInt32();
			target_uid 	= _br.ReadInt32();
		
			{
				int			CHRs_in_tile = ( _scr_type == data_sets_manager.EScreenDataType.sdt_Tiles4x4 ) ? 32:16;
				Rectangle	prj_scr_rect = project_data_converter_provider.get_converter().get_prj_scr_rect();
				
				y += prj_scr_rect.Y * CHRs_in_tile;
				x += prj_scr_rect.X * CHRs_in_tile;
				
				y = y < 0 ? 0:y;
				y = ( y >= platform_data.get_screen_height_pixels() ) ? ( platform_data.get_screen_height_pixels() - 1 ):y;
				
				x = x < 0 ? 0:x;
				x = ( x >= platform_data.get_screen_width_pixels() ) ? ( platform_data.get_screen_width_pixels() - 1 ):x;
			}
			
			// extra data ( reserved for future purposes )
			int extra_data_size = _br.ReadInt32();
		}
	}
	
	[DataContract]
	public class entity_data
	{
		private string m_name				= "";
		private string m_properties			= "";
		private string m_inst_properties	= "";
		
		private byte m_uid	= 0;
		
		private byte m_width	= 16;
		private byte m_height	= 16;
		
		private sbyte m_pivot_x	= 8;
		private sbyte m_pivot_y	= 16;
		
		private Color	m_color	= utils.CONST_COLOR_DEFAULT_ENTITY;
		private Bitmap	m_bmp	= null;
		
		private bool	m_image	= false;
		
		public bool image_flag
		{
			get { return m_image; }
			set { m_image = value; color = utils.CONST_COLOR_DEFAULT_ENTITY_INACTIVE; }
		}
		
		[DataMember]
		public string name
		{
			get { return m_name; }
			set { m_name = value; }
		}

		[DataMember]
		public string properties
		{
			get { return m_properties; }
			set { m_properties = value; }
		}

		[DataMember]
		public string inst_properties
		{
			get { return m_inst_properties; }
			set { m_inst_properties = value; }
		}
		
		[DataMember]
		public byte uid
		{
			get { return m_uid; }
			set { m_uid = value; }
		}
		
		[DataMember]
		public byte width
		{
			get { return m_width; }
			set 
			{ 
				if( m_width != value )
				{
					m_width = value;					
					update_bitmap( false );
				}
			}
		}
		[DataMember]
		public byte height
		{
			get { return m_height; }
			set 
			{
				if( m_height != value )
				{
					m_height = value;					
					update_bitmap( false );
				}
			}
		}
		
		[DataMember]
		public sbyte pivot_x
		{
			get { return m_pivot_x; }
			set { m_pivot_x = value; }
		}
		[DataMember]
		public sbyte pivot_y
		{
			get { return m_pivot_y; }
			set { m_pivot_y = value; }
		}
		
		[DataMember]
		public Color color
		{
			get { return m_color; }
			set 
			{ 
				if( m_color != value )
				{
					m_color = value; 
					update_bitmap( true );
				}
			}
		}
		
		[DataMember]
		public Bitmap bitmap
		{
			get { return m_bmp; }
			set { m_bmp = value; check_indexed_bitmap(); }
		}
		
		public entity_data( string _name )
		{
			name = _name;
			
			update_bitmap( false );
		}

		public entity_data()
		{
			update_bitmap( false );
		}
		
		public void destroy()
		{
			name = null;
			
			if( m_bmp != null )
			{
				m_bmp.Dispose();
				m_bmp = null;
			}
			
			m_color = utils.CONST_COLOR_NULL; 
		}
		
		private void update_bitmap( bool _only_color )
		{
			if( image_flag == false )
			{
				if( m_bmp != null && _only_color == false )
				{
					m_bmp.Dispose();
					m_bmp = null;
				}
				
				if( m_bmp == null )
				{
					m_bmp = new Bitmap( width, height, PixelFormat.Format24bppRgb );
				}
				
				Graphics.FromImage( m_bmp ).Clear( m_color );
			}
		}
		
		private void check_indexed_bitmap()
		{
			if( m_bmp.PixelFormat == PixelFormat.Format8bppIndexed || m_bmp.PixelFormat == PixelFormat.Format4bppIndexed || m_bmp.PixelFormat == PixelFormat.Format1bppIndexed )
			{
				Bitmap new_bmp = new Bitmap( m_bmp.Width, m_bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb );
				
		        using( Graphics gfx = Graphics.FromImage( new_bmp ) ) 
		        {
		            gfx.PixelOffsetMode 	= System.Drawing.Drawing2D.PixelOffsetMode.None;
		            gfx.InterpolationMode 	= System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
		            
		            gfx.DrawImage( m_bmp, new Rectangle( 0, 0, m_bmp.Width, m_bmp.Height ) );
		            
		            m_bmp.Dispose();
		            
		            m_bmp = new_bmp;
		        }				
			}
		}
		
		public void save( BinaryWriter _bw )
		{
			_bw.Write( m_name );
			_bw.Write( m_properties );
			_bw.Write( m_inst_properties );
			_bw.Write( m_uid );
			_bw.Write( m_width );
			_bw.Write( m_height );
			_bw.Write( m_pivot_x );
			_bw.Write( m_pivot_y );
			_bw.Write( m_color.ToArgb() );
			_bw.Write( m_image );
			
			if( image_flag )
			{
				MemoryStream mem_stream = new MemoryStream();
				bitmap.Save( mem_stream, ImageFormat.Bmp );
				
				byte[] byte_img = mem_stream.ToArray();
				
				_bw.Write( byte_img.Length );
				_bw.Write( byte_img );
			}
			
			// extra user defined data ( reserved for future purposes )
			int extra_data_size = 0;
			_bw.Write( extra_data_size );
		}
		
		public void load( BinaryReader _br, byte _ver )
		{
			m_name 				= _br.ReadString();
			m_properties 		= _br.ReadString();
			m_inst_properties 	= _br.ReadString();
			m_uid 				= _br.ReadByte();
			m_width 			= _br.ReadByte();
			m_height 			= _br.ReadByte();
			m_pivot_x			= _br.ReadSByte();
			m_pivot_y			= _br.ReadSByte();
			m_color				= Color.FromArgb( _br.ReadInt32() );
			m_image		 		= _br.ReadBoolean();

			if( m_bmp != null )
			{
				m_bmp.Dispose();
				m_bmp = null;
			}
			
			if( m_image )
			{
				bitmap = new Bitmap( new MemoryStream( _br.ReadBytes( _br.ReadInt32() ) ) );
			}
			else
			{
				update_bitmap( true );
			}
			
			// extra data ( reserved for future purposes )
			int extra_data_size = _br.ReadInt32();
		}
	}
}
