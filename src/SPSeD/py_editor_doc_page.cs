﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 29.05.2019
 * Time: 18:27
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SPSeD
{
	/// <summary>
	/// Description of py_editor_doc_page.
	/// </summary>
	public partial class py_editor_doc_page : UserControl
	{
		public event EventHandler TextChangedEvent;		
		public event EventHandler SelectionChangedEvent;
		public event EventHandler UpdateLnColMsgEvent;
		
		private string	m_script_filename	= null;
		
		private bool	m_data_changed		= false;
		
		private static int m_line_height	= -1;
		
		private Graphics m_gfx 	= null;
		
		private static Brush 	m_brush_white 		= null; 
		private static Brush 	m_brush_gray 		= null;
		private static Brush 	m_brush_dark_gray 	= null;
		private static Point 	m_tmp_pos 			= new Point( 0, 0 );
		
		public RichTextBox script_text_box
		{
			get { return ScriptTextBox; }
			set {}
		}
		
		public PictureBox line_number_pix_box
		{
			get { return LineNumberPixBox; }
			set {}
		}
		
		public string script_filename
		{
			get { return m_script_filename; }
			set { m_script_filename = value; }
		}
		
		public py_editor_doc_page( ContextMenuStrip _cm )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			ScriptTextBox.ContextMenuStrip = _cm;
		}
		
		public void destroy()
		{
			if( m_gfx != null )
			{
				m_gfx.Dispose();
				m_gfx = null;
			}
			
			if( LineNumberPixBox.Image != null )
			{
				LineNumberPixBox.Image.Dispose();
				LineNumberPixBox.Image = null;
			}
			
			ScriptTextBox.ContextMenuStrip = null;
			
			m_script_filename = null;
		}
		
		public void first_init()
		{
			if( m_line_height < 0 )
			{
				ScriptTextBox.Text = "x\nx";	
				m_line_height = ScriptTextBox.GetPositionFromCharIndex( 2 ).Y - ScriptTextBox.GetPositionFromCharIndex( 0 ).Y;
				ScriptTextBox.Text = "";
			}

			update_line_number_pixbox_width();
		}
		
		public static void static_data_init()
		{
			if( m_brush_white == null && m_brush_gray == null && m_brush_dark_gray == null )
			{
				m_brush_white 		= new SolidBrush( Color.White );
				m_brush_gray 		= new SolidBrush( Color.FromArgb( unchecked( ( int )0xffe8e8e8 ) ) );
				m_brush_dark_gray 	= new SolidBrush( Color.FromArgb( unchecked( ( int )0xff707070 ) ) );
			}
		}
		
		public static void static_data_deinit()
		{
			if( m_brush_white != null )
			{
				m_brush_white.Dispose();
				m_brush_white = null;
			}
			
			if( m_brush_gray != null )
			{
				m_brush_gray.Dispose();
				m_brush_gray = null;
			}
			
			if( m_brush_dark_gray != null )
			{
				m_brush_dark_gray.Dispose();
				m_brush_dark_gray = null;
			}
		}
		
		void ScriptTextBoxVScroll(object sender, EventArgs e)
		{
			update_line_numbers();
		}
		
		void ScriptTextBoxTextChanged(object sender, EventArgs e)
		{
			m_data_changed = true;
			
			update_line_numbers();
			
			if( TextChangedEvent != null )
			{
				TextChangedEvent( this, null );
			}
		}
		
		void ScriptTextBoxSizeChanged(object sender, EventArgs e)
		{
			update_line_numbers();
		}
		
		void ScriptTextBoxSelectionChanged(object sender, EventArgs e)
		{
			if( SelectionChangedEvent != null )
			{
				SelectionChangedEvent( this, null );
			}
		}
		
		void ScriptTextBoxMouseDown(object sender, MouseEventArgs e)
		{
			update_ln_col_msg_event();
		}

		void ScriptTextBoxKeyUp(object sender, KeyEventArgs e)
		{
			update_ln_col_msg_event();
		}

		void ScriptTextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			update_ln_col_msg_event();
		}

		void update_ln_col_msg_event()
		{
			if( UpdateLnColMsgEvent != null )
			{
				UpdateLnColMsgEvent( this, null );
			}
		}
		
		void update_line_numbers()
		{
			if( LineNumberPixBox.Image == null || ( LineNumberPixBox.Width != LineNumberPixBox.Image.Width || LineNumberPixBox.Height != LineNumberPixBox.Image.Height ) )
			{
				if( LineNumberPixBox.Image != null )
				{
					LineNumberPixBox.Image.Dispose();
				}

				Bitmap pbox_bmp = new Bitmap( LineNumberPixBox.Width, LineNumberPixBox.Height, PixelFormat.Format32bppPArgb );
				LineNumberPixBox.Image = pbox_bmp;
				
				m_gfx = Graphics.FromImage( pbox_bmp );
			}
			
			m_gfx.Clear( Color.White );

			if( ScriptTextBox.Lines.Length > 0 && m_line_height > 0 )
			{
				update_line_number_pixbox_width();
				
				int pix_offset = ( ScriptTextBox.GetPositionFromCharIndex( 0 ).Y % m_line_height );
	
				m_tmp_pos.X = m_tmp_pos.Y = 0;
				int first_ind	= ScriptTextBox.GetCharIndexFromPosition( m_tmp_pos );
				int first_line	= ScriptTextBox.GetLineFromCharIndex( first_ind );

				m_tmp_pos.X = ClientRectangle.Width;
				m_tmp_pos.Y = ClientRectangle.Height;
				int last_ind	= ScriptTextBox.GetCharIndexFromPosition( m_tmp_pos );
				int last_line	= ScriptTextBox.GetLineFromCharIndex( last_ind );
				
				int line_offset;
				int y_step;

				for( int i = 0; i <= last_line - first_line; i++ )
				{
					line_offset = first_line + i;
					y_step 		= pix_offset + ( i * m_line_height );
					
					m_gfx.FillRectangle( ( ( line_offset & 0x01 ) == 0x01 ) ? m_brush_white:m_brush_gray, 0, y_step, LineNumberScriptFieldSplitContainer.SplitterDistance, m_line_height );
					m_gfx.DrawString( ( line_offset + 1 ).ToString(), ScriptTextBox.Font, m_brush_dark_gray, 0, y_step );
				}
			}
			
			LineNumberPixBox.Invalidate();
		}
		
		public void update_line_number_pixbox_width()
		{
			int width = ( int )( m_gfx.MeasureString( "0", ScriptTextBox.Font ).Width ) + 1;
			int mul;
			
			int lines = ScriptTextBox.Lines.Length;
	
			if( lines <= 99 )
			{
				mul = 2;
			}
			else 
			if( lines <= 999 )
			{
				mul = 3;
			}
			else
			{
				mul = 4;
			}

			width *= mul;
			
			LineNumberScriptFieldSplitContainer.IsSplitterFixed = false;
			{
				LineNumberPixBox.Width 									= width;
				LineNumberScriptFieldSplitContainer.Panel1MinSize 		= width;
				LineNumberScriptFieldSplitContainer.SplitterDistance 	= width;
			}
			LineNumberScriptFieldSplitContainer.IsSplitterFixed = true;
		}
		
		public bool get_data_changed_flag()
		{
			return m_data_changed;
		}
		
		public void reset_data_changed_flag()
		{
			m_data_changed = false;
		}
	}
}
