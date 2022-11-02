/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 09.05.2017
 * Time: 12:35
 */
namespace MAPeD
{
	partial class tab_page_container
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// tab_page_container
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Icon = global::MAPeD.Properties.Resources.MAPeD_icon;
			this.KeyPreview = true;
			this.Name = "tab_page_container";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "TabPage Container";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Closed_event);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.key_down_event);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.key_up_event);
			this.ResumeLayout(false);
		}
	}
}
