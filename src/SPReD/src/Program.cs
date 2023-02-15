/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2022 ( MIT license. See LICENSE.txt )
 * Date: 13.03.2017
 * Time: 11:24
 */
using System;
using System.Windows.Forms;

namespace SPReD
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
			try
			{
				if( !Environment.Is64BitProcess && !utils.CONST_DEV_BUILD_FLAG )
				{
					throw new Exception( "Re-compile the project for the x64 architecture!" );
				}
				
				if( utils.is_win )
				{
					FileAssociations.EnsureAssociationsSet();
				}
				
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
			
				Application.Run(new MainForm( args ));
			}
			catch( System.Exception _err )
			{
				MainForm.message_box( _err.Message + ( _err.InnerException != null ? "\n\n" + _err.InnerException.Message:"" ), "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}
	}
}
