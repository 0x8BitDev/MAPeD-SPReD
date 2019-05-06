/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 14.12.2018
 * Time: 19:38
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of import_tiles_form.
	/// </summary>
	public partial class import_tiles_form : Form
	{
		public bool import_tiles
		{
			get { return checkBoxTiles.Checked; }
			set {}
		}
		
		public bool skip_zero_CHR_Block
		{
			get { return checkBoxSkipZeroCHRBlock.Checked; }
			set {}
		}
		
		public import_tiles_form()
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
