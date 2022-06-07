/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 01.03.2022
 * Time: 18:28
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of PCE_export_form.
	/// </summary>
	public partial class PCE_export_form : Form
	{
		public int CHRs_offset
		{
			get { return (int)NumCHRsOffset.Value; }
			set {}
		}
		
		public int VADDR
		{
			get { return (int)NumVADDR.Value; }
			set {}
		}
		
		public int palette_slot
		{
			get { return (int)NumPaletteSlot.Value; }
		}
		
		public bool comment_CHR_data
		{
			get { return CheckBoxCommentCHRData.Checked; }
			set {}
		}
		
		public PCE_export_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			NumCHRsOffset.Value	= 128;
			
			update_vaddr_label();
		}

		void NumVADDRChanged_Event(object sender, EventArgs e)
		{
			update_chr_offset_label();
		}

		void NumVADDRKeyUp_Event(object sender, KeyEventArgs e)
		{
			if( NumVADDR.Text.Length > 2 )
			{
				update_chr_offset_label();
			}
		}
		
		void NumCHRsOffsetChanged_Event(object sender, EventArgs e)
		{
			update_vaddr_label();
		}
		
		void update_vaddr_label()
		{
			NumVADDR.Value = CHRs_offset << 6;
		}
		
		void update_chr_offset_label()
		{
			NumCHRsOffset.Value = VADDR >> 6;
		}
		
		public DialogResult ShowDialog( bool _asm_data )
		{
			CheckBoxCommentCHRData.Checked = _asm_data;
			
			return base.ShowDialog();
		}
	}
}
