/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
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
		
		public tab_page_container( TabControl _tab_cntrl, MainForm _parent, bool _resizable )
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			m_parent = _parent;
			
			if( _resizable )
			{
				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.MaximizeBox = true;
				this.MinimizeBox = true;
			}
			
			if( _tab_cntrl.SelectedTab != null )
			{
				m_tab_cntrl = _tab_cntrl;
				m_tab_page	= _tab_cntrl.SelectedTab;

				// mono's "feature"
//				if( !utils.is_win )
				{
					if( _tab_cntrl.TabPages.Count > 1 )
					{
						_tab_cntrl.SelectTab( _tab_cntrl.SelectedIndex ^ 1 );
						_tab_cntrl.Update();
					}
				}
				
				_tab_cntrl.TabPages.Remove( m_tab_page );
			
				m_inner_tab_cntrl 		= new System.Windows.Forms.TabControl();
				m_inner_tab_cntrl.Dock 	= DockStyle.Fill; 
				m_inner_tab_cntrl.TabPages.Add( m_tab_page );

				// mono's "feature"
//				if( !utils.is_win )
				{
					m_tab_page.Show();
				}
				
				this.Controls.Add( m_inner_tab_cntrl );
				
				if( m_tab_page.Tag != null )
				{
					Point pt = (Point)( m_tab_page.Tag );
					
					m_tab_page.Width	= pt.X;
					m_tab_page.Height	= pt.Y;
				}

				this.ClientSize = new Size( m_tab_cntrl.Width, m_tab_page.Height + ( m_tab_cntrl.Height - m_tab_page.Height ) );
			}
		}
		
		void Closed_event(object sender, FormClosedEventArgs e)
		{
			if( m_tab_page != null && m_tab_cntrl != null )
			{
				this.Controls.Remove( m_inner_tab_cntrl );
				m_inner_tab_cntrl.TabPages.Remove( m_tab_page );
				
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
