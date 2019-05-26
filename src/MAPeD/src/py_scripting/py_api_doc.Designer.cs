/*
 * Created by SharpDevelop.
 * User: 0x8BitDev Copyright 2017-2019 ( MIT license. See LICENSE.txt )
 * Date: 24.05.2019
 * Time: 18:57
 */
namespace MAPeD
{
	partial class py_api_doc
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(py_api_doc));
			this.HTMLBrowser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// HTMLBrowser
			// 
			this.HTMLBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.HTMLBrowser.Location = new System.Drawing.Point(0, 0);
			this.HTMLBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.HTMLBrowser.Name = "HTMLBrowser";
			this.HTMLBrowser.Size = new System.Drawing.Size(960, 500);
			this.HTMLBrowser.TabIndex = 0;
			this.HTMLBrowser.DocumentText = System.Text.Encoding.Default.GetString( ( byte[] )resources.GetObject( "MAPeD_Data_Export_Python_API" ) );
			// 
			// py_api_doc
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(960, 500);
			this.Controls.Add(this.HTMLBrowser);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "py_api_doc";
			this.Text = "MAPeD API Doc";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.WebBrowser HTMLBrowser;
	}
}
