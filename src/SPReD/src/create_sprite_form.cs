/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 24.04.2017
 * Time: 12:57
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SPReD
{
	/// <summary>
	/// Description of create_sprite_form.
	/// </summary>
	/// 
	
	public partial class create_sprite_form : Form
	{
		public string edit_str
		{
			get { return this.EditNewName.Text; }
			set { this.EditNewName.Text = value; }
		}
		
		public int sprite_width
		{
			get { return (int)numericNewSpriteWidth.Value; }
			set {}
		}

		public int sprite_height
		{
			get { return (int)numericNewSpriteHeight.Value; }
			set {}
		}
		
		public create_sprite_form()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			
			numericNewSpriteWidth.Value 	= 2;
			numericNewSpriteHeight.Value 	= 2;
			
			edit_str = "UNKNOWN";
		}
	}
}
