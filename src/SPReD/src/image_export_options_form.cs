/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
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
		public enum EImgFormat
		{
			BMP,
			PNG,
			PCX,
			UNKNOWN,
		};
		
		private EImgFormat m_img_fmt = EImgFormat.UNKNOWN;
		
		public bool alpha_channel
		{
			get { return checkBoxAlphaChannel.Checked; }
			set {}
		}
		
		public EImgFormat format
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
		
		void BtnImgFormatChanged_Event(object sender, EventArgs e)
		{
			if( RBtnFormatPNG.Checked )
			{
				checkBoxAlphaChannel.Enabled = true;
				m_img_fmt = EImgFormat.PNG;
			}
			else
			if( RBtnFormatBMP.Checked )
			{
				checkBoxAlphaChannel.Enabled = false;
				m_img_fmt = EImgFormat.BMP;
			}
			else
			{
				checkBoxAlphaChannel.Enabled = false;
				m_img_fmt = EImgFormat.PCX;
			}
		}
	}
}
