/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2017
 * Time: 11:00
 */
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

using Hjg.Pngcs;


namespace SPReD
{
	/// <summary>
	/// Description of CHR_data_storage.
	/// </summary>
	public class CHR_data_storage
	{
		private List< CHR_data_group >	m_data	= null;
		
		public CHR_data_storage()
		{
			m_data = new List< CHR_data_group >( 100 );
		}
		
		public void reset()
		{
			m_data.ForEach( delegate( CHR_data_group _obj ) { _obj.reset(); } );
			m_data.Clear();
		}

		public sprite_params create()
		{
			CHR_data_group chr_data = new CHR_data_group();
			
			add( chr_data );
			
			return new sprite_params( chr_data );
		}
		
		public sprite_params create( PngReader _png_reader, bool _apply_palette, bool _crop_image, int _palette_slot )
		{
			CHR_data_group chr_data = new CHR_data_group();
			
			add( chr_data );
			
			return chr_data.setup( _png_reader, _apply_palette, _crop_image, _palette_slot );
		}

		public sprite_params create( Bitmap _bmp, bool _apply_palette, int _palette_slot )
		{
			CHR_data_group chr_data = new CHR_data_group();
			
			add( chr_data );
			
			return chr_data.setup( _bmp, _apply_palette, _palette_slot );
		}
		
		public void add( CHR_data_group _chr_data )
		{
			m_data.Add( _chr_data );
		}
		
		public bool remove( CHR_data_group _data )
		{
			if( _data.get_link_cnt() <= 0 )
			{
				int size = m_data.Count;
				
				for( int i = 0; i < size; i++ )
				{
					if( m_data[ i ] == _data )
					{
						m_data.Remove( _data );
						
						return true;
					}
				}
			}
			
			return false;
		}
		
		public int get_banks_cnt()
		{
			return m_data.Count;
		}
		
		public List< CHR_data_group > get_banks()
		{
			return m_data;
		}
		
		public int get_num_tiles()
		{
			int num_tiles = 0;
			
			int size = m_data.Count;
			
			for( int i = 0; i < size; i++ )
			{
				num_tiles += m_data[ i ].get_data().Count;
			}
			
			return num_tiles;
		}
		
		public CHR_data_group get_bank_by_id( int _id )
		{
			int size = m_data.Count;
			
			for( int i = 0; i < size; i++ )
			{
				if( m_data[ i ].id == _id )
				{
					return m_data[ i ];
				}
			}
			
			return null;
		}
		
#if DEF_NES
		public void export( StreamWriter _sw, string _filename, bool _commented, bool _need_padding )
#elif DEF_SMS
		public void export( StreamWriter _sw, string _filename, bool _commented, int _CHR_size )
#elif DEF_PCE
		public void export( StreamWriter _sw, string _filename, bool _commented )
#else
...
#endif
		{
			int CHR_data_size;
			
			int size = m_data.Count;
			
#if DEF_SMS || DEF_PCE
			string CHR_data_arr = ( _commented ? ";":"" ) + _filename + "_CHR_arr:\t";
#endif
			for( int i = 0; i < size; i++ )
			{
#if DEF_NES || DEF_PCE
				CHR_data_size = m_data[ i ].get_size_bytes();
#elif DEF_SMS
				CHR_data_size = m_data[ i ].get_data().Count * _CHR_size;
#else
...
#endif

#if DEF_NES
				_sw.WriteLine( ( _commented ? ";":"" ) + m_data[ i ].name + ":\t.incbin \"" + _filename + "_" + m_data[ i ].get_filename() + "\"\t; " + CHR_data_size + ( _need_padding ? " of " + ( CHR_data_size + utils.get_padding( CHR_data_size ) ):"" ) + " bytes" );
#elif DEF_SMS || DEF_PCE
				_sw.WriteLine( ( _commented ? ";":"" ) + m_data[ i ].name + ":\t.incbin \"" + _filename + "_" + m_data[ i ].get_filename() + "\"\t; " + CHR_data_size + " bytes" );
#else
...
#endif

#if DEF_SMS || DEF_PCE
				CHR_data_arr += "\n" + ( _commented ? ";":"" ) + "\t.word " + CHR_data_size + ", " + m_data[ i ].name;
#endif
			}
			
#if DEF_SMS || DEF_PCE
			_sw.WriteLine( "\n" + CHR_data_arr );
#endif
		}
		
		public void rearrange_CHR_data_ids()
		{
			for( int i = 0; i < m_data.Count; i++ )
			{
				if( m_data[ i ].get_link_cnt() == 0 )
				{
					if( remove( m_data[ i ] ) )
					{
						--i;
						
						continue;
					}
				}
				
				m_data[ i ].id = i;
			}
		}
		
		public void save( BinaryWriter _bw )
		{
			rearrange_CHR_data_ids();
			
			int chr_data_size = m_data.Count;
			
			_bw.Write( chr_data_size );
			
			for( int i = 0; i < chr_data_size; i++ )
			{
				m_data[ i ].save( _bw );
			}
		}
		
		public void load( BinaryReader _br )
		{
			CHR_data_group chr_data;
			
			int chr_data_size = _br.ReadInt32();
			
			for( int i = 0; i < chr_data_size; i++ )
			{
				chr_data = new CHR_data_group();
				chr_data.load( _br );
				
				add( chr_data );
			}
		}
	}
}
