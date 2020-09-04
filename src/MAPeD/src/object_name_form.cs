/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 12.12.2018
 * Time: 15:03
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of object_name_form.
	/// </summary>
	public partial class object_name_form : Form
	{
		public string edit_str
		{
			get { return textBox.Text; }
			set { textBox.Text = value; }
		}
		
		public object_name_form()
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
