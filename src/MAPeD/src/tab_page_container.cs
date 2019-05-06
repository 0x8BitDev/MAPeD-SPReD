/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 09.05.2017
 * Time: 12:35
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of tab_page_container.
	/// </summary>
	public partial class tab_page_container : Form
	{
		private TabPage		m_tab_page	= null;
		private TabControl	m_tab_cntrl	= null;
		
		private TabControl	m_inner_tab_cntrl	= null;
		
		private MainForm	m_parent	= null;
		
		public tab_page_container( TabControl _tab_cntrl, MainForm _parent )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			m_parent = _parent;
			
			if( _tab_cntrl.SelectedTab != null )
			{
				m_tab_cntrl = _tab_cntrl;
				m_tab_page	= _tab_cntrl.SelectedTab;
				
				_tab_cntrl.TabPages.Remove( _tab_cntrl.SelectedTab );
				
				m_inner_tab_cntrl = new System.Windows.Forms.TabControl();
				m_inner_tab_cntrl.TabPages.Add( m_tab_page );
				
				this.Controls.Add( m_inner_tab_cntrl );
				
				if( m_tab_page.Tag != null )
				{
					Point pt = (Point)( m_tab_page.Tag );
					
					m_tab_page.Width	= pt.X;
					m_tab_page.Height	= pt.Y;
				}

				m_inner_tab_cntrl.Width		= this.Width	= m_tab_page.Width;
				m_inner_tab_cntrl.Height	= this.Height	= m_tab_page.Height;
				
				this.Width	= m_inner_tab_cntrl.Width	+= 12;
				this.Height	= m_inner_tab_cntrl.Height	+= 48;
			}
		}
		
		void Close_event(object sender, FormClosedEventArgs e)
		{
			if( m_tab_page != null && m_tab_cntrl != null )
			{
				m_inner_tab_cntrl.TabPages.Remove( m_tab_page );
				this.Controls.Remove( m_inner_tab_cntrl );
				
				m_tab_cntrl.TabPages.Add( m_tab_page );
				
				m_tab_cntrl.SelectedTab = m_tab_page;
				
				m_tab_page 	= null;
				m_tab_cntrl = null;
				
				m_inner_tab_cntrl = null;
				
				m_parent = null;
			}
		}
		
		void key_up_event(object sender, KeyEventArgs e)
		{
			if( m_parent != null )
			{
				m_parent.KeyUp_Event( sender, e );
			}
		}
		
		void key_down_event(object sender, KeyEventArgs e)
		{
			if( m_parent != null )
			{
				m_parent.KeyDown_Event( sender, e );
			}
		}
	}
}
