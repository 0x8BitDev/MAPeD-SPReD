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
		public Label operation_label
		{
			get { return OperationLabel; }
			set {}
		}
		
		public Label status_label
		{
			get { return StatusLabel; }
			set {}
		}
		
		public ProgressBar progress_bar
		{
			get { return ProgressBar; }
			set {}
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
