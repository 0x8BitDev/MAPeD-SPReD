/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 28.04.2017
 * Time: 17:45
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of rename_sprite_form.
	/// </summary>
	/// 
	
	public partial class rename_sprite_form : Form
	{
		public string edit_str
		{
			get { return this.EditNewName.Text; }
			set { this.EditNewName.Text = value; }
		}
		
		public rename_sprite_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
	}
}
