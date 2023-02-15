/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2017
 * Time: 18:13
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of copy_sprite_new_name_form.
	/// </summary>
	/// 
	
	public partial class copy_sprite_new_name_form : Form
	{
		public string edit_str
		{
			get { return this.EditNewName.Text; }
			set { this.EditNewName.Text = value; }
		}
		
		public copy_sprite_new_name_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public bool is_postfix_selected()
		{
			return RBtnPostfix.Checked;
		}
	}
}
