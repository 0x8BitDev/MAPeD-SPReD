/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 21.03.2022
 * Time: 13:20
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of image_import_options_form.
	/// </summary>
	public partial class image_import_options_form : Form
	{
		public bool crop_by_alpha
		{
			get { return CheckBoxCropByAlpha.Checked; }
			set {}
		}
		
		public bool apply_palette
		{
			get { return CheckBoxApplyPalette.Checked; }
			set {}
		}
		
		public int palette_slot
		{
			get { return CBoxPaletteSlot.SelectedIndex; }
			set {}
		}
		
		public image_import_options_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
#if DEF_FIXED_LEN_PALETTE16_ARR
			for( int i = 0; i < utils.CONST_PALETTE16_ARR_LEN; i++ )
			{
				CBoxPaletteSlot.Items.Add( String.Format( " #{0:d2}", i ) );
			}
			
			CBoxPaletteSlot.SelectedIndex = 0;
#else
			CBoxPaletteSlot.Items.Add( String.Format( " #{0:d2}", 0 ) );
			
			CBoxPaletteSlot.SelectedIndex	= 0;
			CBoxPaletteSlot.Enabled			= false;
#endif
		}
		
		void show_help(object sender, EventArgs e)
		{
			MainForm.message_box( "'Apply palette' - apply the nearest colors to the imported sprite(s)\nNote: This will modify the palette!\n\n'Crop by alpha' - crop the imported sprites to their minimum size by alpha channel\nNote: BMP files have no alpha channel!", "Image Import Help", MessageBoxButtons.OK, MessageBoxIcon.Information );
		}
	}
}
