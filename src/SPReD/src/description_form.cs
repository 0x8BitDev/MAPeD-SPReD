/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 01.02.2019
 * Time: 19:34
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of description_form.
	/// </summary>
	public partial class description_form : Form
	{
		public string edit_text
		{
			get { return richTextBox.Text; }
			set { richTextBox.Text = value; }
		}
		
		public description_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			CBoxShowAfterLoading.Checked = SPReD_CFG.Default.auto_show_description;
		}
		
		void BtnCloseClick_Event(object sender, EventArgs e)
		{
			Close();
		}
		
		void CBoxShowAfterLoadingChanged_Event(object sender, EventArgs e)
		{
			// save state
			SPReD_CFG.Default.auto_show_description = ( sender as CheckBox ).Checked;
			
			SPReD_CFG.Default.Save();
		}
		
		public bool auto_show()
		{
			return CBoxShowAfterLoading.Checked;
		}
	}
}
