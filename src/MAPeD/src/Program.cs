/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 01.05.2017
 * Time: 15:24
 */
using System;
using System.Windows.Forms;

namespace MAPeD
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			FileAssociations.EnsureAssociationsSet();
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			try
			{
				Application.Run(new MainForm( args ));
			}
			catch( System.Exception _err )
			{
				MainForm.message_box( _err.Message, "Unknown Error", System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
		
	}
}
