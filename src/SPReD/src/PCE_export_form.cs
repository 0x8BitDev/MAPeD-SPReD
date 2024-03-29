﻿/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2023 ( MIT license. See LICENSE.txt )
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
		
		public bool non_packed_sprites_opt
		{
			get {return CheckBoxNonPackedSpritesOpt.Checked; }
			set {}
		}
		
		public bool add_filename_to_sprite_names
		{
			get { return CheckBoxAddFileNameToSpriteName.Checked; }
			set {}
		}
		
		public bool comment_CHR_data
		{
			get { return CheckBoxCommentCHRData.Checked; }
			set {}
		}
		
		public string data_dir
		{
			get { return TextBoxDataDir.Text; }
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

		private void NumVADDRChanged( object sender, EventArgs e )
		{
			update_chr_offset_label();
		}

		private void NumVADDRKeyUp( object sender, KeyEventArgs e )
		{
			if( NumVADDR.Text.Length > 2 )
			{
				update_chr_offset_label();
			}
		}
		
		private void NumCHRsOffsetChanged( object sender, EventArgs e )
		{
			update_vaddr_label();
		}
		
		private void update_vaddr_label()
		{
			NumVADDR.Value = CHRs_offset << 6;
		}
		
		private void update_chr_offset_label()
		{
			NumCHRsOffset.Value = VADDR >> 6;
		}
		
		public DialogResult show_window( bool _asm_data )
		{
			CheckBoxCommentCHRData.Checked = _asm_data;
			
			return ShowDialog();
		}
	}
}
