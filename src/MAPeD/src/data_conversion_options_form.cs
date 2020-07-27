/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
 * User: sutr
 * Date: 24.07.2020
 * Time: 10:25
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of data_conversion_options_form.
	/// </summary>
	public partial class data_conversion_options_form : Form
	{
		public enum EScreensAlignMode
		{
			sam_Top,
			sam_Center,
			sam_Bottom,
		};
		
		public EScreensAlignMode screens_align_mode
		{
			get 
			{ 
				if( RBtnScrAlignTop.Checked )
				{
					return EScreensAlignMode.sam_Top;
				}
				else
				if( RBtnScrAlignCenter.Checked )
				{
					return EScreensAlignMode.sam_Center;
				}
				else
				{
					return EScreensAlignMode.sam_Bottom;
				}
			}
			set {}
		}
		
		public bool convert_colors
		{
			get { return CBoxConvertColors.Checked; }
			set {}
		}
		
		public data_conversion_options_form()
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
