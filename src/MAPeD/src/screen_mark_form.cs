/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 08.01.2019
 * Time: 16:33
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of screen_mark_form.
	/// </summary>
	public partial class screen_mark_form : Form
	{
		public int mark
		{
			get { return ( int )NumericUpDownScreenMark.Value; }
			set {}
		}
		
		public screen_mark_form()
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
