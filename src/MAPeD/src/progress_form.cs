/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2021 ( MIT license. See LICENSE.txt )
 * Date: 22.02.2021
 * Time: 18:33
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Description of progress_form.
	/// </summary>
	public partial class progress_form : Form
	{
		public string operation_msg
		{
			set { OperationLabel.Text = value; }
		}
		
		public string status_msg
		{
			set { StatusLabel.Text = value; }
		}
		
		public int progress_value
		{
			set 
			{
				int val = Math.Min( value, ProgressBar.Maximum );
				
				ProgressBar.Value = val;
				ProgressBar.Increment( val );
				ProgressBar.Refresh();
			}
		}
		
		public bool show_progress_bar
		{
			set
			{
				ProgressBar.Visible = value;
			}
		}
		
		public progress_form()
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
