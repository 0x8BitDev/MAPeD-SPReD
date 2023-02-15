/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2019-2023 ( MIT license. See LICENSE.txt )
 * Date: 20.05.2019
 * Time: 16:19
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Threading;

namespace SPSeD
{
	/// <summary>
	/// Description of py_editor.
	/// </summary>
	/// 
	
	public partial class py_editor : Form
	{
		delegate void VoidFunc( string _msg, string _caption );
		
		private const string CONST_EDITOR_NAME		= "SPSeD";
		private const string CONST_NO_SCRIPT_MSG	= "no active_script!";

		private ScriptEngine	m_py_engine	= null;
		private ScriptScope		m_py_scope	= null;
		
		private i_py_api		m_py_api		= null;
		private py_api_doc		m_py_api_doc	= null;

		private py_output m_py_output = null;
		
		private Thread m_script_exec_thread = null;
		
		private static py_editor m_instance = null;
		
		private string m_api_doc_str			= null;
		private string m_api_doc_html_filename	= null;
		private string m_api_doc_title			= null;
		
		private const int CONST_OS_WIN		= 0x01;
		private const int CONST_OS_LINUX	= 0x02;
		private const int CONST_OS_MACOS	= 0x04;
		
		private static int m_os_flag	= 0x00;
		
		public py_editor( Icon _icon, i_py_api _api, string _api_doc_title, string _api_doc_str, string _api_doc_html_filename )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			if( m_instance == null )
			{
				m_instance = this;
			}
			
			check_os();
			
			this.Icon = _icon;
			
			m_api_doc_title			= _api_doc_title;
			m_api_doc_str			= _api_doc_str;
			m_api_doc_html_filename	= _api_doc_html_filename;
			
			this.Text = CONST_EDITOR_NAME + " " + get_app_version();
			
			m_py_api = _api;

			py_init();
			
			FormClosing += new FormClosingEventHandler( OnFormClosing );
			
			OutputTextBox.Text = "Simple Python script editor ( IronPython " + m_py_engine.LanguageVersion.ToString() + " )";
			
			// enable the 'Run' script button and disable the 'Stop' button
			run_script_btn_enable( true );
			
			py_editor_doc_page.static_data_init();
			
			NewToolStripMenuItemClick( null, null );
			
			update_status_msg( "ok!.." );
		}
		
		public static bool is_active()
		{
			return m_instance != null;
		}

		public static void set_focus()
		{
			if( is_active() )
			{
				m_instance.Focus();
			}
		}
		
		private void destroy()
		{
			foreach( TabPage page in DocPagesContainer.TabPages )
			{
				delete_page( page, true );
			}
			
			py_editor_doc_page.static_data_deinit();
			
			m_py_engine = null;
			m_py_scope 	= null;
			
			m_py_api.deinit();
			m_py_api = null;
			
			m_api_doc_title			= null;
			m_api_doc_str			= null;
			m_api_doc_html_filename	= null;
			
			if( m_instance == this )
			{
				m_instance = null;
			}
		}
		
		private void update_tab_page_text( TabPage _tab_page, bool _force_update )
		{
			bool has_data_changed_mark = ( _tab_page.Text.IndexOf( "*" ) >= 0 );
			
			py_editor_doc_page doc_page = _tab_page.Controls[ 0 ] as py_editor_doc_page;
			
			if( doc_page.get_data_changed_flag() != has_data_changed_mark || _force_update == true )
			{
				_tab_page.Text = get_doc_page_filename( doc_page ) + ( doc_page.get_data_changed_flag() ? "*":"" );
			}
		}
		
		private string get_doc_page_filename( py_editor_doc_page _doc_page )
		{
			return _doc_page.script_filename != null ? Path.GetFileName( _doc_page.script_filename ):"UNTITLED";
		}
		
		public DialogResult message_box( string _msg, string _caption, MessageBoxButtons _buttons, MessageBoxIcon _icon = MessageBoxIcon.Warning )
		{
			return MessageBox.Show( this, _msg, _caption, _buttons, _icon );
		}
		
		private py_editor_doc_page get_active_script()
		{
			if( DocPagesContainer.SelectedTab != null )
			{
				return DocPagesContainer.SelectedTab.Controls[ 0 ] as py_editor_doc_page;
			}
			
			return null;
		}
		
		private void py_msg_box( string _msg, string _caption )
		{
			message_box( _msg, _caption, MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		private void py_init()
		{
			m_py_engine	= Python.CreateEngine();
			m_py_scope	= m_py_engine.CreateScope();

			// search for modules in the application directory
			{
				System.Collections.Generic.List< string > search_paths = new System.Collections.Generic.List< string >();
				search_paths.Add( AppDomain.CurrentDomain.BaseDirectory );
				m_py_engine.SetSearchPaths( search_paths );
			}
			
			m_py_api.init( m_py_scope );
			
			// redirect Python output
			m_py_output = new py_output( OutputTextBox, ( m_os_flag & CONST_OS_WIN ) != 0 );
			m_py_engine.Runtime.IO.SetOutput( new MemoryStream(), m_py_output );
			
			// init scope
			m_py_scope.SetVariable( m_py_api.get_prefix() + "msg_box", new VoidFunc( py_msg_box ) );
		}

		private void delete_page( TabPage _tab_page, bool _force_delete )
		{
			py_editor_doc_page doc_page = _tab_page.Controls[ 0 ] as py_editor_doc_page;
			
			if( _force_delete == false && doc_page.get_data_changed_flag() )
			{
				if( message_box( "All unsaved data will be lost!\nDo you want to save the script?", _tab_page.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
				{
					if( doc_page.script_filename == null )
					{
						saveFileDialog.ShowDialog();
					}
					else
					{
						save_script( _tab_page );
					}
				}
			}
			
			DocPagesContainer.TabPages.Remove( _tab_page );
			
			doc_page.TextChangedEvent 		-= on_text_changed;
			doc_page.SelectionChangedEvent 	-= on_selection_changed;
			doc_page.UpdateLnColMsgEvent	-= on_update_ln_col_status_msg;
			
			doc_page.destroy();
			
			update_status_msg( "script deleted" );
		}
		
		private void NewToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = new py_editor_doc_page( StandardContextMenuStrip );
			
			TabPage new_page = new TabPage();
			new_page.Controls.Add( doc_page );
			
			doc_page.Dock = DockStyle.Fill;
			
			DocPagesContainer.SuspendLayout();
			{
				DocPagesContainer.TabPages.Add( new_page );
				DocPagesContainer.SelectTab( new_page );
				
				//new_page.UseVisualStyleBackColor = true;
			}
			DocPagesContainer.ResumeLayout();

			doc_page.script_filename = null;
			
			doc_page.script_text_box.Text = "";
			doc_page.script_text_box.Focus();
			
			doc_page.TextChangedEvent 		+= new EventHandler( on_text_changed );
			doc_page.SelectionChangedEvent 	+= new EventHandler( on_selection_changed );
			doc_page.UpdateLnColMsgEvent	+= new EventHandler( on_update_ln_col_status_msg );
			
			update_tab_page_text( new_page, true );
			
			update_undo_redo();
			update_cut_copy_paste_delete();
			
			update_status_msg( "new script" );
		}
		
		private void LoadToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( DocPagesContainer.TabPages.Count > 0 )
			{
				openFileDialog.ShowDialog();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}

		private void ReloadToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				if( doc_page.script_filename != null )
				{
					load_script( doc_page );
				}
				else
				{
					update_status_msg( "no file to reload!" );
				}
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}

		private void LoadOk( object sender, System.ComponentModel.CancelEventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_filename = ( ( FileDialog )sender ).FileName;
				
				load_script( doc_page );
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}

		private void load_script( py_editor_doc_page _doc_page )
		{
			try
			{
				_doc_page.script_text_box.Text = File.ReadAllText( _doc_page.script_filename );
				
				_doc_page.reset_data_changed_flag();
				update_tab_page_text( DocPagesContainer.SelectedTab, true );
				
				update_status_msg( "loaded" );
				
				on_update_ln_col_status_msg( null, null );
			}
			catch( System.Exception _err )
			{
				message_box( _err.Message, "Script Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void SaveAllToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page;
			
			foreach( TabPage page in DocPagesContainer.TabPages )
			{
				doc_page = page.Controls[ 0 ] as py_editor_doc_page;
				
				if( doc_page.get_data_changed_flag() == true && doc_page.script_filename != null )
				{
					save_script( page );
				}
			}
		}
		
		private void SaveToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				if( doc_page.script_filename == null )
				{
					saveFileDialog.ShowDialog();
				}
				else
				{
					save_script( DocPagesContainer.SelectedTab );
				}
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}

		private void SaveAsToolStripMenuItemClick( object sender, EventArgs e )
		{
			saveFileDialog.ShowDialog();
		}

		private void SaveOk( object sender, System.ComponentModel.CancelEventArgs e )
		{
			save_script( DocPagesContainer.SelectedTab, ( ( FileDialog )sender ).FileName );
		}

		private void save_script( TabPage _tab_page, string _filename = null )
		{
			try
			{
				py_editor_doc_page doc_page = _tab_page.Controls[ 0 ] as py_editor_doc_page;

				string filename = ( _filename != null ) ? _filename:doc_page.script_filename;
				
				File.WriteAllText( filename, doc_page.script_text_box.Text );

				doc_page.script_filename = filename;
				
				doc_page.reset_data_changed_flag();
				update_tab_page_text( _tab_page, true );
				
				update_status_msg( "saved" );
			}
			catch( System.Exception _err )
			{
				message_box( _err.Message, "Script Saving Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
		private void RunToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				SaveAllToolStripMenuItemClick( null, null );
				doc_page.script_text_box.Focus();
				
				m_script_exec_thread = new Thread(() =>
				{
					try
					{
						update_status_msg( "script is running..." );
						
						m_py_output.reset();
						
						ScriptSource src = m_py_engine.CreateScriptSourceFromString( UI_thread_safe_call_get_string( doc_page, () => { return doc_page.script_text_box.Text; } ), get_doc_page_filename(doc_page), SourceCodeKind.File );
						
						UI_thread_safe_call( this, () => { run_script_btn_enable( false ); } );
						
						object res = src.Execute( m_py_scope );
						
						if( res != null )
						{
							m_py_output.append_text( res.ToString() );
						}
						
						m_py_output.append_text( "\nScript execution success!" );
						
						update_status_msg( "ok!.." );
					}
					catch( ThreadAbortException /*_err*/ )
					{
						m_py_output.append_text( "\nScript execution aborted!\n\n" );
						
						update_status_msg( "ok!.." );
						
						Thread.ResetAbort();
					}
					catch( Exception _err )
					{
						m_py_output.append_text( "\nScript execution failure!\n\n" );
						update_status_msg( "error" );
						
						m_py_output.append_text( m_py_engine.GetService< ExceptionOperations >().FormatException( _err ) );
					}
					
					finally
					{
						UI_thread_safe_call( this, () => { run_script_btn_enable( true ); } );
					}
				});
				
				m_script_exec_thread.IsBackground = true;
				m_script_exec_thread.Start();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		
		private void StopToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( m_script_exec_thread.IsAlive )
			{
				abort_script_thread();
			}
		}
		
		private bool abort_script_thread()
		{
			// deprecated, but we need to suspend the working thread to ask user a question
			// just not to complicate the things. all .Suspend/.Resume calls in one place
			m_script_exec_thread.Suspend();
			
			if( MessageBox.Show( this, "Abort the script execution thread?\n\nWarning: This can negatively affect the resources generated in the script!", "Stop Script", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				// deprecated, but we need to resume the thread before aborting
				m_script_exec_thread.Resume();
				
				// this is a really bad way to stop the script thread, but the IronPython
				// doesn't provide any safe way to cancel a working script thread
				// but it is much better than when the script thread finished when application was closed...
				m_script_exec_thread.Abort();
				
				return true;
			}
			
			m_script_exec_thread.Resume();
			
			return false;
		}
		
		private void run_script_btn_enable( bool _on )
		{
			runToolStripMenuItem.Enabled	= runToolStripButton.Enabled	= _on;
			stopToolStripMenuItem.Enabled	= stopToolStripButton.Enabled	= !_on;
		}
		
		private void ExitToolStripMenuItemClick( object sender, EventArgs e )
		{
			Close();
		}
		
		private void OnFormClosing( object sender, FormClosingEventArgs e )
		{
			if( ( m_script_exec_thread != null ) && m_script_exec_thread.IsAlive )
			{
				if( abort_script_thread() == false )
				{
					e.Cancel = true;
					
					return;
				}
			}
			
			if ( DocPagesContainer.TabPages.Count == 0 )
			{
				e.Cancel = false;
			}
			else
			{
				e.Cancel = true;
				
				if( message_box( "All unsaved data will be lost!\nAre you sure?", "Exit " + CONST_EDITOR_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
				{
					e.Cancel = false;
				}
			}
			
			if( !e.Cancel )
			{
				destroy();
				
				if( py_api_doc.is_active() )
				{
					m_py_api_doc.Close();
					m_py_api_doc = null;
				}
			}
		}
		
		private void on_update_ln_col_status_msg( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page =  get_active_script();//sender as py_editor_doc_page;
			
			if( doc_page != null )
			{
				int column 	= doc_page.script_text_box.SelectionStart - doc_page.script_text_box.GetFirstCharIndexOfCurrentLine() + 1;
				int row 	= doc_page.script_text_box.GetLineFromCharIndex( doc_page.script_text_box.GetFirstCharIndexOfCurrentLine() ) + 1;
				
				toolStripLnCol.Text = "ln " + row.ToString() + "   col " + column.ToString();
			}
			else
			{
				toolStripLnCol.Text = "";
			}
		}
		
		private void update_status_msg( string _msg )
		{
			UI_thread_safe_call( this, () => { toolStripStatus.Text = _msg; } );
		}
		
		private void InAppDocToolStripMenuItemClick( object sender, EventArgs e )
		{
			if( !py_api_doc.is_active() )
			{
				m_py_api_doc = new py_api_doc( m_api_doc_str, this.Icon, m_api_doc_title, ( m_os_flag & CONST_OS_WIN ) != 0 );
				m_py_api_doc.Show();
			}
			
			py_api_doc.set_focus();
		}
		
		private void InBrowserDocToolStripMenuItemClick( object sender, EventArgs e )
		{
			string doc_path = Application.StartupPath.Substring( 0, Application.StartupPath.LastIndexOf( Path.DirectorySeparatorChar ) ) + Path.DirectorySeparatorChar + "doc" + Path.DirectorySeparatorChar + m_api_doc_html_filename;
			
			//message_box( doc_path, "path", MessageBoxButtons.OK, MessageBoxIcon.Information );//!!!
			
			if( ( m_os_flag & CONST_OS_WIN ) != 0 )
			{
				System.Diagnostics.Process.Start( doc_path );
			}
			else
			if( ( m_os_flag & CONST_OS_LINUX ) != 0 )
			{
				System.Diagnostics.Process.Start( "xdg-open", doc_path );
			}
			else
			if( ( m_os_flag & CONST_OS_MACOS ) != 0 )
			{
				// need to test it...
				System.Diagnostics.Process.Start( "open", doc_path );
			}
		}
		
		private void AboutToolStripMenuItemClick( object sender, EventArgs e )
		{
			Version ver			= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			DateTime build_date = new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds( ver.Revision * 2 );
			
			message_box( "Simple Python script editor" + "\n\n" + get_app_version() + " Build: " + ver.Build + "\nBuild date: " + build_date + "\n\nDeveloped by 0x8BitDev \u00A9 2019-" + DateTime.Now.Year, "About", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
		
		private string get_app_version()
		{
			Version ver	= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			return "v" + ver.Major + "." + ver.Minor + "b";
		}
		
		private void on_text_changed( object sender, EventArgs e )
		{
			update_status_msg( "..." );
			
			update_tab_page_text( DocPagesContainer.SelectedTab, false );
			
			update_undo_redo();
		}
		
		private void on_selection_changed( object sender, EventArgs e )
		{
			update_cut_copy_paste_delete();
		}
		
		#region context menu: cut copy paste delete
		private void ContextMenuStripOpening( object sender, System.ComponentModel.CancelEventArgs e )
		{
			update_cut_copy_paste_delete();
		}
		
		private void update_cut_copy_paste_delete()
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				deleteToolStripButton.Enabled		=
				deleteToolStripMenuItem.Enabled 	=
				deleteToolStripMenuItem1.Enabled	=
				copyToolStripButton.Enabled			=
				copyToolStripMenuItem.Enabled 		=
				copyToolStripMenuItem1.Enabled		=
				cutToolStripButton.Enabled			=
				cutToolStripMenuItem.Enabled 		=
				cutToolStripMenuItem1.Enabled		= ( doc_page.script_text_box.SelectedText.Length > 0 );
			}
			
			pasteToolStripButton.Enabled 	= 
			pasteToolStripMenuItem.Enabled 	= 
			pasteToolStripMenuItem1.Enabled = Clipboard.ContainsText();
		}

		private void update_undo_redo()
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				undoToolStripButton.Enabled = undoToolStripMenuItem.Enabled = doc_page.script_text_box.CanUndo;
				redoToolStripButton.Enabled = redoToolStripMenuItem.Enabled = doc_page.script_text_box.CanRedo;
			}
		}
		
		private void UndoToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Undo();
				
				update_undo_redo();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		
		private void RedoToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Redo();
				
				update_undo_redo();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		
		private void CutToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Cut();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		
		private void CopyToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Copy();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		
		private void PasteToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Paste();
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		
		private void DeleteToolStripMenuItemClick( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.SelectedText = "";
			}
			else
			{
				update_status_msg( CONST_NO_SCRIPT_MSG );
			}
		}
		#endregion
		
		private void OnShown( object sender, EventArgs e )
		{
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Focus();
			}
			
			if( Type.GetType( "Mono.Runtime") != null )
			{
				message_box( "It's not recommended to use the editor for script editing under Mono due to buggy implementation of RichTextBox. You can load your script into the editor and to any other editor where you can edit it. Then in this editor you can press Ctrl+R (reload script) and F5 (run script).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
			}
		}
		
		private void DocPagesContainerSelectedIndexChanged( object sender, EventArgs e )
		{
			update_cut_copy_paste_delete();
			update_undo_redo();
			
			on_update_ln_col_status_msg( sender, e );
			
			py_editor_doc_page doc_page = get_active_script();
			
			if( doc_page != null )
			{
				doc_page.script_text_box.Focus();
			}
		}
		
		private void DocPagesContainerDrawItem( object sender, DrawItemEventArgs e )
		{
			// draw close_tab image ( red cross )
			TabPage tab_page = DocPagesContainer.TabPages[ e.Index ];
			
			Rectangle tab_rect = DocPagesContainer.GetTabRect( e.Index );
			tab_rect.Inflate( -2, -2 );
			
			Bitmap close_img = Properties.Resources.close_tab;
			
			e.Graphics.DrawImage( close_img, ( tab_rect.Right - close_img.Width ), tab_rect.Top + ( tab_rect.Height - close_img.Height) >> 1 );
			TextRenderer.DrawText( e.Graphics, tab_page.Text, tab_page.Font, tab_rect, tab_page.ForeColor, TextFormatFlags.Left );
		}
		
		private void DocPagesContainerMouseUp( object sender, MouseEventArgs e )
		{
			Rectangle 	tab_rect;
			Rectangle 	img_rect;
			Bitmap		close_img;
			
			for( int i = 0; i < DocPagesContainer.TabPages.Count; i++)
			{
				tab_rect = DocPagesContainer.GetTabRect( i );
				
				tab_rect.Inflate( -2, -2 );
				
				close_img = Properties.Resources.close_tab;
				
				img_rect = new Rectangle( ( tab_rect.Right - close_img.Width ), tab_rect.Top + ( tab_rect.Height - close_img.Height ) >> 1, close_img.Width, close_img.Height );
				
				if( img_rect.Contains( e.Location ) )
				{
					delete_page( DocPagesContainer.TabPages[ i ], false );
					break;
				}
			}
		}
		
		// OS detection code implemented by jarik ( 100% managed code ) https://stackoverflow.com/a/38795621
		private void check_os()
		{
			m_os_flag = 0;
			
			string windir = Environment.GetEnvironmentVariable("windir");
			
			if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
			{
				m_os_flag = CONST_OS_WIN;
			}
			else if (File.Exists(@"/proc/sys/kernel/ostype"))
			{
				string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
				if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
				{
					// Note: Android gets here too
					m_os_flag = CONST_OS_LINUX;
				}
				else
				{
					throw new Exception( "Unsupported platform detected!" );
				}
			}
			else if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
			{
				// Note: iOS gets here too
				m_os_flag = CONST_OS_MACOS;
			}
			else
			{
				throw new Exception( CONST_EDITOR_NAME + ": Unsupported platform detected!" );
			}
		}
		
		public static void UI_thread_safe_call( Control _cntrl, Action _act )
		{
			if( _cntrl.InvokeRequired )
			{
				_cntrl.Invoke( (MethodInvoker)( () => _act() ) );
			}
			else
			{
				_act();
			}
		}
		
		public static string UI_thread_safe_call_get_string( Control _cntrl, Func< string > _act )
		{
			if( _cntrl.InvokeRequired )
			{
				return ( string )_cntrl.Invoke( _act );
			}
			else
			{
				return _act();
			}
		}
	}
	
	class py_output : TextWriter
	{
		private readonly RichTextBox	m_txt_box;
		private readonly bool			m_is_win;
		
		public py_output( RichTextBox _txt_box, bool _is_win )
		{
			m_txt_box	= _txt_box;
			m_is_win	= _is_win;
		}
		
		public override void Write( char _val )
		{
			base.Write( _val );
			
			if( m_is_win && _val == '\r' )
			{
				return;
			}
			
			append_text( _val.ToString() );
		}
		
		public void append_text( string _str )
		{
			py_editor.UI_thread_safe_call( m_txt_box, () => m_txt_box.AppendText(_str) );
		}
		
		public void reset()
		{
			py_editor.UI_thread_safe_call( m_txt_box, () => { m_txt_box.Text = ""; } );
		}
		
		public override System.Text.Encoding Encoding
		{
			get { return System.Text.Encoding.UTF8; }
		}
	}
}
