/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 24.05.2019
 * Time: 18:57
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of py_api_doc.
	/// </summary>
	public partial class py_api_doc : Form
	{
		private static py_api_doc m_instance = null;
		
		public py_api_doc()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			//this.HTMLBrowser.DocumentText = System.Text.Encoding.Default.GetString( ( byte[] )resources.GetObject( "MAPeD_Data_Export_Python_API" ) );
			
			if( m_instance == null )
			{
				m_instance = this;
			}
			
			try
			{
				// check if browser component exists
				HTMLBrowser.Refresh();
			}
			catch( Exception /*_err*/ )
			{
				if( utils.is_win )
				{
					this.Text = "Ooops!.. Something went wrong... Please, try to select 'In-Browser Doc' option or press F1";
				}
				else
				{
					this.Text = "Ooops!.. It seems like you need to install 'libgluezilla' to view the in-app document or try to select 'In-Browser Doc' option or press F1";
				}
			}
		}

		public static bool is_active()
		{
			return m_instance != null ? true:false;
		}
		
		public static void set_focus()
		{
			if( m_instance != null )
			{
				m_instance.Focus();
			}
		}
		
		void OnFormClosed(object sender, FormClosedEventArgs e)
		{
			if( m_instance == this )
			{
				m_instance = null;
			}
		}
	}
}
