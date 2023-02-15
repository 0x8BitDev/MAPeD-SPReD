/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
		public enum e_img_format
		{
			BMP,
			PNG,
			PCX,
			UNKNOWN,
		};
		
		private e_img_format m_img_fmt = e_img_format.UNKNOWN;
		
		public bool alpha_channel
		{
			get { return checkBoxAlphaChannel.Checked; }
			set {}
		}
		
		public e_img_format format
		{
			get
			{
				return m_img_fmt;
			}
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
#if DEF_PCE
			RBtnFormatPCX.Checked = true;
#else
			RBtnFormatPNG.Checked = true;
#endif
		}
		
		private void RBtnImgFormatChanged( object sender, EventArgs e )
		{
			if( RBtnFormatPNG.Checked )
			{
				checkBoxAlphaChannel.Enabled = true;
				m_img_fmt = e_img_format.PNG;
			}
			else
			if( RBtnFormatBMP.Checked )
			{
				checkBoxAlphaChannel.Enabled = false;
				m_img_fmt = e_img_format.BMP;
			}
			else
			{
				checkBoxAlphaChannel.Enabled = false;
				m_img_fmt = e_img_format.PCX;
			}
		}
	}
}
