/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 04.12.2018
 * Time: 16:42
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of image_export_options_form.
	/// </summary>
	public partial class image_export_options_form : Form
	{
		public bool alpha_channel
		{
			get { return checkBoxAlphaChannel.Checked; }
			set {}
		}
		
		public ImageFormat format
		{
			get
			{
				return RBtnFormatBMP.Checked ? ImageFormat.Bmp:ImageFormat.Png;
			}
			set {}
		}
		
		public image_export_options_form()
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
