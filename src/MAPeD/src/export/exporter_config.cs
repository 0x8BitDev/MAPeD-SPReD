/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 05.05.2023
 * Time: 10:57
 */
using System;
using System.IO;

namespace MAPeD
{
	/// <summary>
	/// Description of exporter_config.
	/// </summary>
	public class exporter_config
	{
		public const string	CONST_BIN_EXT	= ".bin";
		
		private string	m_data_offset_alias	= "Offs";	// Offs
		private string	m_name_alias		= null;		// mpd or filename
		
		private string	m_data_align_alias	= "";		// .align 2 ...

		private string	m_filename			= null;
		private string	m_path				= null;
		private string 	m_path_filename_ext	= null;
		private string 	m_path_filename		= null;

		private string	m_comment_token		= ";";
		
		private string	m_byte_alias		= null;		// .db .byte
		private string	m_word_alias		= null;		// .dw .word
		
		private string	m_C_byte_type_alias	= null;		// u8, unsigned char
		private string	m_C_word_type_alias	= null;		// u16, unsigned short
		
		// export options
		public bool m_generate_H_file	= false;
		public bool m_data_order_rows	= false;  
		//...
		
		public string data_offset_alias
		{
			set { m_data_offset_alias = value; }
			get { return m_data_offset_alias; }
		}

		public string name_alias
		{
			set { m_name_alias = value; }
			get { return m_name_alias; }
		}

		public string data_align_alias
		{
			set { m_data_align_alias = value; }
			get { return m_data_align_alias; }
		}
		
		public string filename
		{
			get { return m_filename; }
		}

		public string path
		{
			get { return m_path; }
		}
		
		public string path_filename_ext
		{
			get { return m_path_filename_ext; }
		}
		
		public string path_filename
		{
			get { return m_path_filename; }
		}

		public string comment_token
		{
			set { m_comment_token = value; }
			get { return m_comment_token; }
		}
		
		public string byte_alias
		{
			set { m_byte_alias = value; }
			get { return m_byte_alias; }
		}

		public string word_alias
		{
			set { m_word_alias = value; }
			get { return m_word_alias; }
		}

		public string C_byte_type_alias
		{
			set { m_C_byte_type_alias = value; }
			get { return m_C_byte_type_alias; }
		}

		public string C_word_type_alias
		{
			set { m_C_word_type_alias = value; }
			get { return m_C_word_type_alias; }
		}
		
		public exporter_config()
		{
			//...
		}
		
		public void parse_path( string _full_path )
		{
			m_path_filename_ext	= _full_path;
			m_filename			= get_exp_prefix() + Path.GetFileNameWithoutExtension( m_path_filename_ext );
			m_path				= Path.GetDirectoryName( m_path_filename_ext ) + Path.DirectorySeparatorChar;
			m_path_filename		= m_path + m_filename;
			
			m_name_alias		= m_filename;
		}
		
		public string get_data_prefix()
		{
			return m_name_alias + "_";
		}
		
		public static string skip_exp_prefix( string _str )
		{
			int offs = _str.IndexOf( "_" );
			offs = ( offs == 0 ) ? ( offs + 1 ):0;
			
			return _str.Substring( offs );
		}
		
		public string get_exp_prefix()
		{
			return m_generate_H_file ? "_":"";
		}
	}
}
