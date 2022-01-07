/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 28.12.2018
 * Time: 16:46
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of export_active_tile_block_set_form.
	/// </summary>
	public partial class export_active_tile_block_set_form : Form
	{
		public bool export_tiles
		{
			get { return RBtnTiles.Checked; }
			set {}
		}
		
		public enum EImgForm
		{
			img_Rect16xN,
			img_Line,
		}
		
		public EImgForm image_form()
		{
			return RBtnImgFormRect16xN.Checked ? EImgForm.img_Rect16xN:EImgForm.img_Line;
		}
		
		public export_active_tile_block_set_form()
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
