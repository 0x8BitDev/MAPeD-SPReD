/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
		
		public enum e_screen_align_mode
		{
			MidTop		= CONST_ALIGN_X_MID | CONST_ALIGN_Y_TOP,
			MidCenter	= CONST_ALIGN_X_MID | CONST_ALIGN_Y_MID,
			MidBottom	= CONST_ALIGN_X_MID | CONST_ALIGN_Y_BOTTOM,
			
			LeftTop		= CONST_ALIGN_X_LEFT | CONST_ALIGN_Y_TOP,
			LeftCenter	= CONST_ALIGN_X_LEFT | CONST_ALIGN_Y_MID,
			LeftBottom	= CONST_ALIGN_X_LEFT | CONST_ALIGN_Y_BOTTOM,
			
			RightTop	= CONST_ALIGN_X_RIGHT | CONST_ALIGN_Y_TOP,
			RightCenter	= CONST_ALIGN_X_RIGHT | CONST_ALIGN_Y_MID,
			RightBottom	= CONST_ALIGN_X_RIGHT | CONST_ALIGN_Y_BOTTOM,
		};
		
		public e_screen_align_mode screen_align_mode
		{
			get 
			{ 
				if( RBtnScrAlignMidTop.Checked )
				{
					return e_screen_align_mode.MidTop;
				}
				else
				if( RBtnScrAlignMidCenter.Checked )
				{
					return e_screen_align_mode.MidCenter;
				}
				if( RBtnScrAlignMidBottom.Checked )
				{
					return e_screen_align_mode.MidBottom;
				}
				else
				if( RBtnScrAlignLeftTop.Checked )
				{
					return e_screen_align_mode.LeftTop;
				}
				else
				if( RBtnScrAlignLeftCenter.Checked )
				{
					return e_screen_align_mode.LeftCenter;
				}
				else
				if( RBtnScrAlignLeftBottom.Checked )
				{
					return e_screen_align_mode.LeftBottom;
				}
				else
				if( RBtnScrAlignRightTop.Checked )
				{
					return e_screen_align_mode.RightTop;
				}
				else
				if( RBtnScrAlignRightCenter.Checked )
				{
					return e_screen_align_mode.RightCenter;
				}
				else
				{
					return e_screen_align_mode.RightBottom;
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
		
		public DialogResult ShowDialog( project_data_desc _prj_data )
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
