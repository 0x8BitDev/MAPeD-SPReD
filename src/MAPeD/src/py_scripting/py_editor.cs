/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 20.05.2019
 * Time: 16:19
 */
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace MAPeD
{
	/// <summary>
	/// Description of py_editor.
	/// </summary>
	/// 
	
	public partial class py_editor : Form
	{
		delegate void VoidFunc( string _msg, string _caption );
		
		private const string CONST_EDITOR_NAME	= "SPSeD";

		private string	m_script_filename	= null;
		
		private int m_line_height	 = 0;
		
		private Graphics 	m_gfx 			= null;
		private Brush 		m_brush_white 	= null; 
		private Brush 		m_brush_gray 	= null;
		private Brush 		m_brush_black 	= null;
		private Point 		m_tmp_pos 		= new Point( 0, 0 );
		
		private ScriptEngine	m_py_engine	= null;
		private ScriptScope		m_py_scope	= null;
		
		private py_api			m_py_api	= null;
		
		public py_editor( data_sets_manager _data_mngr )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			py_init();
			
			m_py_api = new py_api( m_py_scope, _data_mngr );
			
			FormClosing += new System.Windows.Forms.FormClosingEventHandler( OnFormClosing );
			
			OutputTextBox.Text = "Simple Python script editor.\nPowered by IronPython " + m_py_engine.LanguageVersion.ToString();
			
			set_title( m_script_filename );
			
			m_brush_white 	= new SolidBrush( Color.White );
			m_brush_gray 	= new SolidBrush( Color.LightGray );
			m_brush_black 	= new SolidBrush( Color.Black );
		}
		
		void destroy()
		{
			m_gfx.Dispose();
			m_gfx = null;
			
			m_brush_white.Dispose();
			m_brush_white = null;
			
			m_brush_gray.Dispose();
			m_brush_gray = null;
			
			m_brush_black.Dispose();
			m_brush_black = null;
			
			LineNumberPixBox.Image.Dispose();
			LineNumberPixBox.Image = null;
			
			m_script_filename = null;
			
			m_py_engine = null;
			m_py_scope 	= null;
			
			m_py_api = null;
		}
		
		void set_title( string _filename, bool _script_changed_mark = false )
		{
			this.Text = CONST_EDITOR_NAME + " - " + ( _filename != null ? Path.GetFileName( _filename ):"UNTITLED" ) + ( _script_changed_mark ? "*":"" );
		}
		
		public DialogResult message_box( string _msg, string _caption, MessageBoxButtons _buttons, System.Windows.Forms.MessageBoxIcon _icon = System.Windows.Forms.MessageBoxIcon.Warning )
		{
			return MessageBox.Show( this, _msg, _caption, _buttons, _icon );
		}
		
		void py_msg_box( string _msg, string _caption )
		{
			message_box( _msg, _caption, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		void py_init()
		{
			m_py_engine	= Python.CreateEngine();
			m_py_scope	= m_py_engine.CreateScope();
			
			// redirect Python output
			m_py_engine.Runtime.IO.SetOutput( new MemoryStream(), new py_output( OutputTextBox ) );
			
			// init scope
			VoidFunc msg_box = new VoidFunc( py_msg_box );
			m_py_scope.SetVariable( "msg_box", msg_box );
		}

		void Py_editorShown(object sender, EventArgs e)
		{
			// calc line height
			{
				ScriptTextBox.Text = "x\nx";	
				m_line_height = ScriptTextBox.GetPositionFromCharIndex( 2 ).Y - ScriptTextBox.GetPositionFromCharIndex( 0 ).Y;
				ScriptTextBox.Text = "";
			}
			
			ScriptTextBox.Focus();
			update_status_msg( "ok!.." );
			
			update_line_number_pixbox_width();
		}

		void NewToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( ScriptTextBox.Text.Length != 0 )
			{
				if( message_box( "Start a new script?", "New Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					ScriptTextBox.Text = "";
					OutputTextBox.Text = "";
					
					m_script_filename = null;
					set_title( m_script_filename );
					
					ScriptTextBox.Focus();
					
					update_line_number_pixbox_width();
					
					update_status_msg( "new script" );
				}
			}
		}
		
		void LoadToolStripMenuItemClick(object sender, EventArgs e)
		{
			openFileDialog.ShowDialog();
		}

		void ReloadToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( m_script_filename != null )
			{
				load_script( m_script_filename );
			}
			else
			{
				update_status_msg( "can't reload, no file!" );
			}
		}

		void LoadOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			m_script_filename = ( ( FileDialog )sender ).FileName;

			load_script( m_script_filename );
		}

		void load_script( string _filename )
		{
			try
			{
				ScriptTextBox.Text = File.ReadAllText( _filename );
				OutputTextBox.Text = "";
				
				set_title( _filename );
				
				update_status_msg( "loaded" );
			}
			catch( System.Exception _err )
			{
				message_box( _err.Message, "Load Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void SaveToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( m_script_filename == null )
			{
				saveFileDialog.ShowDialog();
			}
			else
			{
				save_script( m_script_filename );
			}
		}

		void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
		{
			saveFileDialog.ShowDialog();
		}

		void SaveOk_Event(object sender, System.ComponentModel.CancelEventArgs e)
		{
			save_script( ( ( FileDialog )sender ).FileName );
		}

		void save_script( string _filename )
		{
			try
			{
				File.WriteAllText( _filename, ScriptTextBox.Text );

				m_script_filename = _filename;
				
				set_title( _filename );
				
				update_status_msg( "saved" );
			}
			catch( System.Exception _err )
			{
				message_box( _err.Message, "Save Script Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		void RunToolStripMenuItemClick(object sender, EventArgs e)
		{
			// save current script
			if( m_script_filename != null )
			{
				save_script( m_script_filename );
			}
			
			try
			{
				OutputTextBox.Text = "";
				ScriptTextBox.Focus();
				
				ScriptSource src = m_py_engine.CreateScriptSourceFromString( ScriptTextBox.Text, SourceCodeKind.File );
				
				object res = src.Execute( m_py_scope );

				OutputTextBox.Text += "\nScript execution success!";
				
				if( res != null )
				{
					OutputTextBox.Text += res.ToString();
				}
			}
			catch( Exception _err )
			{
				OutputTextBox.Text += "Script execution failed!\n\n";
				update_status_msg( "error" );
				
				OutputTextBox.Text += m_py_engine.GetService<ExceptionOperations>().FormatException( _err );
			   	return;
			}			
			
			update_status_msg( "ok!.." );
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			if( ScriptTextBox.Text.Length == 0 )
			{
				e.Cancel = false;
			}
			else
			{
				e.Cancel = true;
			
			    if( message_box( "All unsaved data will be lost!\nAre you sure?", "Exit " + CONST_EDITOR_NAME, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question ) == System.Windows.Forms.DialogResult.Yes )
			    {
			    	e.Cancel = false;
			    }
			}
			
			if( !e.Cancel )
			{
				destroy();
			}
		}
		
		void ScriptTextBoxMouseDown(object sender, MouseEventArgs e)
		{
			update_ln_col_status_msg();
		}

		void ScriptTextBoxKeyUp(object sender, KeyEventArgs e)
		{
			update_ln_col_status_msg();
		}

		void ScriptTextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			update_ln_col_status_msg();
		}
		
		void update_ln_col_status_msg()
		{
			int column 	= ScriptTextBox.SelectionStart - ScriptTextBox.GetFirstCharIndexOfCurrentLine() + 1;
			int row 	= ScriptTextBox.GetLineFromCharIndex( ScriptTextBox.GetFirstCharIndexOfCurrentLine() ) + 1;
			
			toolStripLnCol.Text = "ln " + row.ToString() + "   col " + column.ToString();
		}
		
		void update_status_msg( string _msg )
		{
			toolStripStatus.Text = _msg;
		}
		
		void ScriptTextBoxVScroll(object sender, EventArgs e)
		{
			update_line_numbers();
		}
		
		void ScriptTextBoxTextChanged(object sender, EventArgs e)
		{
			update_line_numbers();
			
			update_status_msg( "..." );
			
			set_title( m_script_filename, true );
		}
		
		void ScriptTextBoxSizeChanged(object sender, EventArgs e)
		{
			update_line_numbers();
		}
		
		void update_line_numbers()
		{
			Bitmap pbox_bmp = null;
			
			if( LineNumberPixBox.Image == null || ( LineNumberPixBox.Width != LineNumberPixBox.Image.Width || LineNumberPixBox.Height != LineNumberPixBox.Image.Height ) )
			{
				if( LineNumberPixBox.Image != null )
				{
					LineNumberPixBox.Image.Dispose();
				}
				
				pbox_bmp = new Bitmap( LineNumberPixBox.Width, LineNumberPixBox.Height, PixelFormat.Format32bppArgb );
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
				
				int y_step 		= 0;
				int line_offset = 0;
	    
				for( int i = 0; i <= last_line - first_line; i++ )
				{
					line_offset = first_line + i;
					y_step 		= pix_offset + ( i * m_line_height );
					
					m_gfx.FillRectangle( ( ( line_offset & 0x01 ) == 0x01 ) ? m_brush_white:m_brush_gray, 0, y_step, LineNumberScriptFieldSplitContainer.SplitterDistance, m_line_height );
					m_gfx.DrawString( ( line_offset + 1 ).ToString(), ScriptTextBox.Font, m_brush_black, 0, y_step );
				}
			}
			
			LineNumberPixBox.Invalidate();
		}
		
		void update_line_number_pixbox_width()    
        {    
			int width = ( int )( m_gfx.MeasureString( "0", ScriptTextBox.Font ).Width ) + 1;
			int mul = 0;
            
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
    
            width = width * mul;
            
            LineNumberScriptFieldSplitContainer.IsSplitterFixed = false;
            {
            	LineNumberPixBox.Width 									= width;
	            LineNumberScriptFieldSplitContainer.Panel1MinSize 		= width;
	            LineNumberScriptFieldSplitContainer.SplitterDistance 	= width;
            }
            LineNumberScriptFieldSplitContainer.IsSplitterFixed = true;
        }		
		
		void InAppDocToolStripMenuItemClick(object sender, EventArgs e)
		{
			if( !py_api_doc.is_active() )
			{
				( new py_api_doc() ).Show();
			}
			else
			{
				py_api_doc.set_focus();
			}
		}
		
		void InBrowserDocToolStripMenuItemClick(object sender, EventArgs e)
		{
			string doc_path = Application.StartupPath.Substring( 0, Application.StartupPath.LastIndexOf( Path.DirectorySeparatorChar ) ) + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + "MAPeD_Data_Export_Python_API.html";
			
			//message_box( doc_path, "path", MessageBoxButtons.OK, MessageBoxIcon.Information );//!!!
			
			if( utils.is_win )
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.Start( doc_path );
			}
			else
			if( utils.is_linux )
			{
				System.Diagnostics.Process process = System.Diagnostics.Process.Start( "xdg-open", doc_path );
			}
			else
			if( utils.is_macos )
			{
				// need to test it...
				System.Diagnostics.Process process = System.Diagnostics.Process.Start( "open", doc_path );
			}
		}
	}
	
	class py_output : TextWriter
	{
		private RichTextBox m_txt_box = null;
		
		public py_output( RichTextBox _txt_box )
		{
			m_txt_box = _txt_box;
		}
		
		public override void Write( char _val )
		{
			base.Write( _val );
			
			if( utils.is_win && _val == '\r' )
			{
				return;
			}
			
			m_txt_box.AppendText( _val.ToString() );
		}
		
		public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }		
	}	
}
