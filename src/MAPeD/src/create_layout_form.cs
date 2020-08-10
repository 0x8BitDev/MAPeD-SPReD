/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 10.08.2020
 * Time: 18:54
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of create_layout_form.
	/// </summary>
	public partial class create_layout_form : Form
	{
		public int layout_width
		{
			get { return ( int )numericUpDownWidth.Value; }
			set {}
		}

		public int layout_height
		{
			get { return ( int )numericUpDownHeight.Value; }
			set {}
		}
		
		public create_layout_form()
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
