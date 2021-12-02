/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
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
		// X - 0-2 bits: Left	Mid		Right
		// Y - 3-5 bits: Top	Mid		Bottom
		
		public const int CONST_ALIGN_X_LEFT		= 0x01;
		public const int CONST_ALIGN_X_MID		= 0x02;
		public const int CONST_ALIGN_X_RIGHT	= 0x04;
		
		public const int CONST_ALIGN_Y_TOP		= 0x08;
		public const int CONST_ALIGN_Y_MID		= 0x10;
		public const int CONST_ALIGN_Y_BOTTOM	= 0x20;
		
		public enum EScreensAlignMode
		{
			sam_MidTop		= CONST_ALIGN_X_MID | CONST_ALIGN_Y_TOP,
			sam_MidCenter	= CONST_ALIGN_X_MID | CONST_ALIGN_Y_MID,
			sam_MidBottom	= CONST_ALIGN_X_MID | CONST_ALIGN_Y_BOTTOM,
			
			sam_LeftTop		= CONST_ALIGN_X_LEFT | CONST_ALIGN_Y_TOP,
			sam_LeftCenter	= CONST_ALIGN_X_LEFT | CONST_ALIGN_Y_MID,
			sam_LeftBottom	= CONST_ALIGN_X_LEFT | CONST_ALIGN_Y_BOTTOM,
			
			sam_RightTop	= CONST_ALIGN_X_RIGHT | CONST_ALIGN_Y_TOP,
			sam_RightCenter	= CONST_ALIGN_X_RIGHT | CONST_ALIGN_Y_MID,
			sam_RightBottom	= CONST_ALIGN_X_RIGHT | CONST_ALIGN_Y_BOTTOM,
		};
		
		public EScreensAlignMode screens_align_mode
		{
			get 
			{ 
				if( RBtnScrAlignMidTop.Checked )
				{
					return EScreensAlignMode.sam_MidTop;
				}
				else
				if( RBtnScrAlignMidCenter.Checked )
				{
					return EScreensAlignMode.sam_MidCenter;
				}
				if( RBtnScrAlignMidBottom.Checked )
				{
					return EScreensAlignMode.sam_MidBottom;
				}
				else
				if( RBtnScrAlignLeftTop.Checked )
				{
					return EScreensAlignMode.sam_LeftTop;
				}
				else
				if( RBtnScrAlignLeftCenter.Checked )
				{
					return EScreensAlignMode.sam_LeftCenter;
				}
				else
				if( RBtnScrAlignLeftBottom.Checked )
				{
					return EScreensAlignMode.sam_LeftBottom;
				}
				else
				if( RBtnScrAlignRightTop.Checked )
				{
					return EScreensAlignMode.sam_RightTop;
				}
				else
				if( RBtnScrAlignRightCenter.Checked )
				{
					return EScreensAlignMode.sam_RightCenter;
				}
				else
				{
					return EScreensAlignMode.sam_RightBottom;
				}
			}
			set {}
		}
		
		public bool convert_colors
		{
			get { return CBoxConvertColors.Checked; }
			set {}
		}

		public bool use_file_screen_resolution
		{
			get { return CBoxUseFileScreenResolution.Checked; }
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
#if DEF_ZX
			CBoxConvertColors.Enabled = false;
#endif
		}
		
		public DialogResult ShowDialog( load_project_data _prj_data )
		{
			if( _prj_data.m_scr_blocks_width == 0xff && _prj_data.m_scr_blocks_height == 0xff )
			{
				CBoxUseFileScreenResolution.Checked = CBoxUseFileScreenResolution.Enabled = false;
			}
			else
			{
				CBoxUseFileScreenResolution.Enabled = true;
			}
#if !DEF_ZX
			CBoxConvertColors.Enabled = ( platform_data.get_platform_type() == platform_data.get_platform_type_by_file_ext( _prj_data.m_file_ext ) ) ? false:true;
#endif			
			return ShowDialog();
		}
	}
}
