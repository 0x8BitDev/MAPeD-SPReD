/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * Date: 15.07.2020
 * Time: 11:29
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of SMS_export_form.
	/// </summary>
	public partial class SMS_export_form : Form
	{
		public int bpp
		{
			get { return CBoxCHRsBpp.SelectedIndex + 1; }
			set {}
		}

		public int CHRs_offset
		{
			get { return (int)NumCHRsOffset.Value; }
			set {}
		}
		
		public SMS_export_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			CBoxCHRsBpp.SelectedIndex = 3;
		}
		
		void BtnOkClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void BtnTilesOffsetInfoClick(object sender, EventArgs e)
		{
			MainForm.message_box( "This value will be added to each CHR index in a sprite attributes.\nIn other words, it's a free space at the beginning of a CHR bank.", "CHRs Offset Description", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
	}
}
