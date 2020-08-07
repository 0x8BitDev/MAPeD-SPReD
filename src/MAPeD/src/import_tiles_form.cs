/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2020 ( MIT license. See LICENSE.txt )
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
			get { return CheckBoxTiles.Checked; }
			set {}
		}

		public bool import_game_level
		{
			get { return CheckBoxGameLevel.Checked; }
			set {}
		}
		
		public bool delete_empty_screens
		{
			get { return CheckBoxDeleteEmptyScreens.Checked; }
			set {}
		}
		
		public bool skip_zero_CHR_Block
		{
			get { return CheckBoxSkipZeroCHRBlock.Checked; }
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
		
		void CheckBoxGameLevelChanged_Event(object sender, EventArgs e)
		{
			CheckBoxTiles.Checked = CheckBoxGameLevel.Checked ? true:CheckBoxTiles.Checked;
			CheckBoxDeleteEmptyScreens.Enabled = !( CheckBoxTiles.Enabled = CheckBoxGameLevel.Checked ? false:true );
			CheckBoxDeleteEmptyScreens.Checked = CheckBoxDeleteEmptyScreens.Enabled ? CheckBoxDeleteEmptyScreens.Checked:false;
		}
	}
}
