/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
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
		
		public sprite_params create( PngReader _png_reader, bool _apply_palette, bool _crop_image )
		{
			CHR_data_group chr_data = new CHR_data_group();
			
			add( chr_data );
			
			return chr_data.setup( _png_reader, _apply_palette, _crop_image );
		}

		public sprite_params create( Bitmap _bmp, bool _apply_palette )
		{
			CHR_data_group chr_data = new CHR_data_group();
			
			add( chr_data );
			
			return chr_data.setup( _bmp, _apply_palette );
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
		
		public void export( StreamWriter _sw, string _filename, bool _commented, bool _need_padding )
		{
			int CHR_data_size;
			
			int size = m_data.Count;
			
			for( int i = 0; i < size; i++ )
			{
				CHR_data_size = m_data[ i ].get_size_bytes();
				
				_sw.WriteLine( ( _commented ? ";":"" ) + m_data[ i ].name + ":\t.incbin \"" + _filename + "_" + m_data[ i ].get_filename() + "\"\t; " + CHR_data_size + ( _need_padding ? " of " + ( CHR_data_size + utils.get_padding( CHR_data_size ) ):"" ) + " bytes" );
			}
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
